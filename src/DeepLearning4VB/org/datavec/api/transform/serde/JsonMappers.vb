'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports System.Text.Json.Serialization
Imports System.Text.Json.Serialization.Metadata
Imports LegacyJsonFormat = DeepLearning4VB.org.datavec.api.transform.serde.legacy.LegacyJsonFormat
Imports ND4VBClassLoading = DeepLearning4VB.org.nd4vb.common.config.ND4VBClassLoading

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' The JSON configuration of the DataVec object model: System.Text.Json options that read and write the JSON of
    ''' Java DataVec (Jackson with field visibility, <c>"@class"</c> type ids, alphabetically sorted properties).
    ''' <para>
    ''' The rules, mirroring the Jackson configuration of the Java original:
    ''' <list type="bullet">
    ''' <item>Classes of <c>org.datavec</c>, implementations of the polymorphic DataVec types and registered classes are
    ''' written field by field: every instance field that is not <c>&lt;NonSerialized&gt;</c> (Java <c>transient</c>),
    ''' <c>&lt;JsonIgnore&gt;</c> or listed in the class's <see cref="JsonIgnorePropertiesAttribute"/>, under its Java
    ''' field name (<c>&lt;JsonPropertyName&gt;</c>, else the field name; in this library the converter's renames
    ''' <c>xField</c>/<c>x_Conflict</c> map to <c>x</c>, and the backing field of an auto-property <c>X</c> to <c>x</c>).
    ''' Properties are ordered as Jackson orders them: creator properties first, then the others, each group sorted.</item>
    ''' <item>A type marked <see cref="JsonClassTypeAttribute"/> (Transform, Condition, Filter, ColumnMetaData, Schema,
    ''' ...) and all its implementations carry <c>"@class" : "&lt;Java class name&gt;"</c>; the Java class name of a type
    ''' of this library is its full name without the leading <c>DeepLearning4VB.</c> (with the package
    ''' <c>org.datavec.api.transform.transform</c> for the VB namespace <c>...transform.transforms</c>, <c>org.nd4j</c> for
    ''' <c>org.nd4vb</c>, and <c>$</c> for nested types). A type marked <see cref="JsonWrapperTypeAttribute"/>
    ''' (IAssociativeReducer) is written as <c>{"Reducer" : { ... }}</c>.</item>
    ''' <item>Reading uses the constructor marked <c>&lt;JsonConstructor&gt;</c> (Jackson's <c>@JsonProperty</c>
    ''' creator) with the JSON values of its parameters and then sets the remaining fields, or else the no-argument
    ''' constructor; methods marked <c>&lt;OnDeserialized&gt;</c> run afterwards. Unknown properties are ignored.</item>
    ''' <item>Joda <c>DateTimeZone</c> values are written as their id, <c>DateTimeFieldType</c> as
    ''' <c>{"fieldType" : "hourOfDay"}</c>, locales as <c>en_US</c>, enums by name, NaN and infinities as strings.</item>
    ''' </list>
    ''' </para>
    ''' <para>
    ''' Implementations are discovered in the loaded assemblies that reference this library (again whenever such an
    ''' assembly is loaded). <see cref="registerSubtypes"/> and <see cref="registerSubtype"/> register further types or
    ''' set the Java class name written for a type, for example to exchange JSON with a Java custom transform:
    ''' <c>JsonMappers.registerSubtype(GetType(MyTransform), "com.example.MyTransform")</c>.
    ''' </para>
    ''' <para>
    ''' <see cref="writeValueAsString(Object)"/> and <see cref="readValue(Of T)(String)"/> produce and read the text
    ''' itself: Jackson's pretty printer layout (<c>"name" : value</c>, two-space indentation, <c>[ a, b ]</c>) and Java's
    ''' <c>Double.toString</c> number format (<c>1.0</c>, <c>1.0E-5</c>); reading also accepts the pre-1.0 (1.0.0-alpha)
    ''' wrapper-object format (see <see cref="LegacyJsonFormat"/>). The <see cref="Mapper"/> options can also be passed to
    ''' System.Text.Json directly, which writes compact JSON with the same structure.
    ''' </para>
    ''' </summary>
    Public NotInheritable Class JsonMappers

        Private Shared ReadOnly gate As New Object()
        Private Shared ReadOnly registeredTypes As New HashSet(Of Type)()
        Private Shared ReadOnly registeredNames As New Dictionary(Of Type, String)()
        Private Shared current As JsonSerializerOptions
        Private Shared stale As Boolean = True

        Shared Sub New()
            AddHandler AppDomain.CurrentDomain.AssemblyLoad, AddressOf onAssemblyLoad
        End Sub

        Private Sub New()
        End Sub

        Private Shared Sub onAssemblyLoad(ByVal sender As Object, ByVal args As AssemblyLoadEventArgs)
            If TypeCatalog.referencesThisLibrary(args.LoadedAssembly) Then stale = True
        End Sub

        ''' <summary>The options for the DataVec object model (Java: the Jackson <c>ObjectMapper</c>). Treat as read-only.</summary>
        Public Shared ReadOnly Property Mapper As JsonSerializerOptions
            Get
                Dim o As JsonSerializerOptions = current
                If o IsNot Nothing AndAlso Not stale Then Return o
                SyncLock gate
                    If current Is Nothing OrElse stale Then
                        stale = False
                        current = createOptions(TypeCatalog.build(registeredTypes, registeredNames))
                    End If
                    Return current
                End SyncLock
            End Get
        End Property

        ''' <summary>Java <c>getMapper()</c>.</summary>
        Public Shared Function getMapper() As JsonSerializerOptions
            Return Mapper
        End Function

        ''' <summary>
        ''' Registers implementations of the polymorphic DataVec types (or any class to be written field by field) that
        ''' discovery does not find; they are written with their default Java class name.
        ''' </summary>
        Public Shared Sub registerSubtypes(ByVal ParamArray types() As Type)
            If types Is Nothing Then Return
            SyncLock gate
                For Each t As Type In types
                    If t Is Nothing Then Continue For
                    registeredTypes.Add(t)
                Next
                stale = True
            End SyncLock
        End Sub

        ''' <summary>
        ''' Registers a type with the name written for it: its <c>"@class"</c> value (normally the Java class name), or
        ''' its wrapper property name for an implementation of a wrapper-object type such as IAssociativeReducer.
        ''' </summary>
        Public Shared Sub registerSubtype(ByVal type As Type, ByVal name As String)
            If type Is Nothing Then Throw New ArgumentNullException(NameOf(type))
            If String.IsNullOrEmpty(name) Then Throw New ArgumentException("A type name is required", NameOf(name))
            SyncLock gate
                registeredTypes.Add(type)
                registeredNames(type) = name
                stale = True
            End SyncLock
        End Sub

        ''' <summary>The <c>"@class"</c> name written for a type (its Java class name).</summary>
        Public Shared Function getJavaClassName(ByVal type As Type) As String
            Return catalogOf(Mapper).javaName(type)
        End Function

        ''' <summary>The type registered or discovered for a <c>"@class"</c> name, or Nothing.</summary>
        Public Shared Function getClassForJavaClassName(ByVal name As String) As Type
            Return catalogOf(Mapper).typeForName(name)
        End Function

        ''' <summary>
        ''' Writes a value as JSON, in the layout of Java DataVec. As with Jackson's <c>writeValueAsString</c>, the
        ''' runtime type decides what is written (with its <c>"@class"</c> or wrapper object when it is polymorphic).
        ''' </summary>
        Public Shared Function writeValueAsString(ByVal value As Object) As String
            If value Is Nothing Then Return "null"
            Dim t As Type = value.GetType()
            Return writeValueAsString(value, If(wrapperRootOf(t), t))
        End Function

        ''' <summary>Writes a value of the given declared type as JSON, in the layout of Java DataVec.</summary>
        Public Shared Function writeValueAsString(ByVal value As Object, ByVal declaredType As Type) As String
            Return writeValueAsString(value, declaredType, Mapper)
        End Function

        ''' <summary>Writes a value of the given declared type as JSON with options created by <see cref="JsonMappers"/>.</summary>
        Public Shared Function writeValueAsString(ByVal value As Object, ByVal declaredType As Type, ByVal options As JsonSerializerOptions) As String
            If value Is Nothing Then Return "null"
            If declaredType Is Nothing Then Throw New ArgumentNullException(NameOf(declaredType))
            If options Is Nothing Then Throw New ArgumentNullException(NameOf(options))
            Return JavaJsonWriter.write(toNode(value, declaredType, options), declaredType, options)
        End Function

        ''' <summary>Reads JSON (current or pre-1.0 Java DataVec format) as a value of type <typeparamref name="T"/>.</summary>
        Public Shared Function readValue(Of T)(ByVal json As String) As T
            Return DirectCast(readValue(json, GetType(T)), T)
        End Function

        ''' <summary>Reads JSON (current or pre-1.0 Java DataVec format) as a value of the given type.</summary>
        Public Shared Function readValue(ByVal json As String, ByVal declaredType As Type) As Object
            Return readValue(json, declaredType, Mapper)
        End Function

        ''' <summary>Reads JSON as a value of the given type with options created by <see cref="JsonMappers"/>.</summary>
        Public Shared Function readValue(ByVal json As String, ByVal declaredType As Type, ByVal options As JsonSerializerOptions) As Object
            If json Is Nothing Then Throw New ArgumentNullException(NameOf(json))
            If declaredType Is Nothing Then Throw New ArgumentNullException(NameOf(declaredType))
            If options Is Nothing Then Throw New ArgumentNullException(NameOf(options))
            Return fromNode(JsonNode.Parse(json), declaredType, options)
        End Function

        ''' <summary>The wrapper-object type (<see cref="JsonWrapperTypeAttribute"/>) a type implements, or Nothing.</summary>
        Friend Shared Function wrapperRootOf(ByVal t As Type) As Type
            If isWrapperRoot(t) Then Return t
            For Each i As Type In t.GetInterfaces()
                If isWrapperRoot(i) Then Return i
            Next
            Dim b As Type = t.BaseType
            Do While b IsNot Nothing
                If isWrapperRoot(b) Then Return b
                b = b.BaseType
            Loop
            Return Nothing
        End Function

        ''' <summary>The JSON tree of a value (before formatting).</summary>
        Friend Shared Function toNode(ByVal value As Object, ByVal declaredType As Type, ByVal options As JsonSerializerOptions) As JsonNode
            If value Is Nothing Then Return Nothing
            If isWrapperRoot(declaredType) Then Return wrapValue(value, declaredType, options)
            Return System.Text.Json.JsonSerializer.SerializeToNode(value, declaredType, options)
        End Function

        ''' <summary>Reads a JSON tree as a value of the given type (converting the pre-1.0 format first).</summary>
        Friend Shared Function fromNode(ByVal node As JsonNode, ByVal declaredType As Type, ByVal options As JsonSerializerOptions) As Object
            If node Is Nothing Then Return Nothing
            node = LegacyJsonFormat.convertLegacyJson(node, declaredType, options)
            If isWrapperRoot(declaredType) Then Return unwrapValue(node, declaredType, options)
            Return System.Text.Json.JsonSerializer.Deserialize(node, declaredType, options)
        End Function

        Private Shared Function createOptions(ByVal catalog As TypeCatalog) As JsonSerializerOptions
            Dim o As New JsonSerializerOptions()
            o.TypeInfoResolver = New DataVecTypeInfoResolver(catalog)
            o.Converters.Add(New JsonStringEnumConverter())
            o.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            o.AllowOutOfOrderMetadataProperties = True
            o.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            o.MaxDepth = 256
            o.MakeReadOnly()
            Return o
        End Function

        Friend Shared Function catalogOf(ByVal options As JsonSerializerOptions) As TypeCatalog
            Dim r As DataVecTypeInfoResolver = TryCast(options.TypeInfoResolver, DataVecTypeInfoResolver)
            If r Is Nothing Then Throw New InvalidOperationException("The options were not created by JsonMappers")
            Return r.Catalog
        End Function

        ' ---------------------------------------------------------------- polymorphic types

        Private Shared ReadOnly classRootCache As New ConcurrentDictionary(Of Type, Boolean)()

        ''' <summary>True when the type is (or derives from, or implements) a type marked <see cref="JsonClassTypeAttribute"/>.</summary>
        Friend Shared Function hasClassRoot(ByVal t As Type) As Boolean
            Return classRootCache.GetOrAdd(t, Function(k)
                                                   Dim c As Type = k
                                                   Do While c IsNot Nothing
                                                       If c.IsDefined(GetType(JsonClassTypeAttribute), False) Then Return True
                                                       c = c.BaseType
                                                   Loop
                                                   For Each i As Type In k.GetInterfaces()
                                                       If i.IsDefined(GetType(JsonClassTypeAttribute), False) Then Return True
                                                   Next
                                                   Return False
                                               End Function)
        End Function

        ''' <summary>True for a type whose values are written as wrapper objects (<see cref="JsonWrapperTypeAttribute"/>).</summary>
        Friend Shared Function isWrapperRoot(ByVal t As Type) As Boolean
            Return t IsNot Nothing AndAlso t.IsDefined(GetType(JsonWrapperTypeAttribute), False)
        End Function

        ''' <summary>Writes <c>{"Name" : { ...fields... }}</c> for a value of a wrapper-object type.</summary>
        Friend Shared Function wrapValue(ByVal value As Object, ByVal root As Type, ByVal options As JsonSerializerOptions) As JsonNode
            If value Is Nothing Then Return Nothing
            Dim t As Type = value.GetType()
            Dim name As String = catalogOf(options).wrapperName(root, t)
            Dim inner As JsonNode = System.Text.Json.JsonSerializer.SerializeToNode(value, t, options)
            Dim obj As New JsonObject()
            obj.Add(name, inner)
            Return obj
        End Function

        ''' <summary>Reads a value of a wrapper-object type (also accepts an object with an <c>"@class"</c> property).</summary>
        Friend Shared Function unwrapValue(ByVal node As JsonNode, ByVal root As Type, ByVal options As JsonSerializerOptions) As Object
            If node Is Nothing Then Return Nothing
            Dim obj As JsonObject = TryCast(node, JsonObject)
            If obj Is Nothing Then Throw New JsonException("Expected a JSON object for a value of type " & root.FullName)
            Dim catalog As TypeCatalog = catalogOf(options)
            If obj.Count = 1 Then
                Dim kv As KeyValuePair(Of String, JsonNode) = obj.First()
                Dim t As Type = catalog.wrapperType(root, kv.Key)
                If t IsNot Nothing Then
                    If kv.Value Is Nothing Then Return Nothing
                    Return System.Text.Json.JsonSerializer.Deserialize(kv.Value, t, options)
                End If
            End If
            Dim cls As String = classNameOf(obj)
            If cls IsNot Nothing Then
                Dim t As Type = catalog.typeForName(cls)
                If t IsNot Nothing AndAlso root.IsAssignableFrom(t) Then
                    Dim copy As JsonObject = DirectCast(JsonNode.Parse(obj.ToJsonString()), JsonObject)
                    copy.Remove("@class")
                    Return System.Text.Json.JsonSerializer.Deserialize(copy, t, options)
                End If
            End If
            Throw New JsonException("Unknown type id for " & root.FullName & ": " & obj.ToJsonString())
        End Function

        ''' <summary>The <c>"@class"</c> value of a JSON object, or Nothing.</summary>
        Friend Shared Function classNameOf(ByVal obj As JsonObject) As String
            Dim cls As JsonNode = Nothing
            If obj IsNot Nothing AndAlso obj.TryGetPropertyValue("@class", cls) AndAlso cls IsNot Nothing AndAlso cls.GetValueKind() = JsonValueKind.String Then
                Return cls.GetValue(Of String)()
            End If
            Return Nothing
        End Function

        ' ---------------------------------------------------------------- navigation of JSON trees by type

        ''' <summary>The declared type of a JSON property of type <paramref name="t"/> (Object when unknown).</summary>
        Friend Shared Function propertyTypeOf(ByVal t As Type, ByVal name As String, ByVal options As JsonSerializerOptions) As Type
            If t Is Nothing OrElse t Is GetType(Object) Then Return GetType(Object)
            Dim valueType As Type = dictionaryValueType(t)
            If valueType IsNot Nothing Then Return valueType
            If catalogOf(options).isManaged(t) Then
                Dim fc As FieldContract = Nothing
                If TypeContract.forType(t).ByName.TryGetValue(name, fc) Then Return fc.Field.FieldType
                Return GetType(Object)
            End If
            Try
                Dim ti As JsonTypeInfo = options.GetTypeInfo(t)
                If ti.Kind = JsonTypeInfoKind.Object Then
                    For Each p As JsonPropertyInfo In ti.Properties
                        If p.Name = name Then Return p.PropertyType
                    Next
                End If
            Catch e As Exception
                ' types System.Text.Json cannot describe: no type information
            End Try
            Return GetType(Object)
        End Function

        ''' <summary>The element type of an array or collection type (Object when unknown).</summary>
        Friend Shared Function elementTypeOf(ByVal t As Type) As Type
            If t Is Nothing Then Return GetType(Object)
            If t.IsArray Then Return t.GetElementType()
            Dim candidates As IEnumerable(Of Type) = t.GetInterfaces()
            If t.IsInterface Then candidates = candidates.Concat(New Type() {t})
            For Each i As Type In candidates
                If i.IsGenericType AndAlso i.GetGenericTypeDefinition() Is GetType(IEnumerable(Of )) Then Return i.GetGenericArguments()(0)
            Next
            Return GetType(Object)
        End Function

        ''' <summary>The value type of a dictionary type, or Nothing.</summary>
        Friend Shared Function dictionaryValueType(ByVal t As Type) As Type
            Dim candidates As IEnumerable(Of Type) = t.GetInterfaces()
            If t.IsInterface Then candidates = candidates.Concat(New Type() {t})
            For Each i As Type In candidates
                If i.IsGenericType AndAlso i.GetGenericTypeDefinition() Is GetType(IDictionary(Of ,)) Then Return i.GetGenericArguments()(1)
            Next
            Return Nothing
        End Function

        ''' <summary>The runtime type named by the <c>"@class"</c> property of a JSON object, else the declared type.</summary>
        Friend Shared Function runtimeTypeOf(ByVal obj As JsonObject, ByVal declared As Type, ByVal options As JsonSerializerOptions) As Type
            Dim cls As String = classNameOf(obj)
            If cls IsNot Nothing Then
                Dim t As Type = catalogOf(options).typeForName(cls)
                If t IsNot Nothing Then Return t
            End If
            Return declared
        End Function

    End Class

    ''' <summary>
    ''' A snapshot of the polymorphic DataVec types: the types marked as polymorphic, their known implementations
    ''' (discovered or registered) and the names written for them.
    ''' </summary>
    Friend NotInheritable Class TypeCatalog

        Private Shared ReadOnly ownAssembly As Assembly = GetType(TypeCatalog).Assembly

        Private ReadOnly concrete As New List(Of Type)()
        Private ReadOnly concreteSet As New HashSet(Of Type)()
        Private ReadOnly registered As HashSet(Of Type)
        Private ReadOnly nameByType As New Dictionary(Of Type, String)()
        Private ReadOnly typeByName As New Dictionary(Of String, Type)(StringComparer.Ordinal)
        Private ReadOnly explicitNames As Dictionary(Of Type, String)
        Private ReadOnly derivedCache As New ConcurrentDictionary(Of Type, Type())()

        Private Sub New(ByVal registered As HashSet(Of Type), ByVal explicitNames As Dictionary(Of Type, String))
            Me.registered = registered
            Me.explicitNames = explicitNames
        End Sub

        ''' <summary>Discovers the implementations in the assemblies that reference this library, plus the registered types.</summary>
        Friend Shared Function build(ByVal registeredTypes As HashSet(Of Type), ByVal registeredNames As Dictionary(Of Type, String)) As TypeCatalog
            Dim c As New TypeCatalog(New HashSet(Of Type)(registeredTypes), New Dictionary(Of Type, String)(registeredNames))
            Dim assemblies As New List(Of Assembly) From {ownAssembly}
            Try
                For Each asm As Assembly In ND4VBClassLoading.Nd4vbAssemblies.Invoke()
                    If asm IsNot ownAssembly AndAlso Not asm.IsDynamic AndAlso referencesThisLibrary(asm) Then assemblies.Add(asm)
                Next
            Catch e As Exception
                ' discovery is best effort; registered types are always known
            End Try
            Dim candidates As New List(Of Type)()
            For Each asm As Assembly In assemblies
                candidates.AddRange(safeGetTypes(asm))
            Next
            candidates.AddRange(registeredTypes)
            For Each t As Type In candidates
                If t Is Nothing OrElse Not isConcreteCandidate(t) OrElse c.concreteSet.Contains(t) Then Continue For
                If c.registered.Contains(t) OrElse JsonMappers.hasClassRoot(t) OrElse hasWrapperRoot(t) Then
                    c.concrete.Add(t)
                    c.concreteSet.Add(t)
                End If
            Next
            For Each t As Type In c.concrete
                Dim name As String = c.javaName(t)
                If Not c.typeByName.ContainsKey(name) Then c.typeByName(name) = t
            Next
            Return c
        End Function

        Friend Shared Function referencesThisLibrary(ByVal asm As Assembly) As Boolean
            If asm Is ownAssembly Then Return True
            Try
                Dim own As String = ownAssembly.GetName().Name
                Return asm.GetReferencedAssemblies().Any(Function(n) String.Equals(n.Name, own, StringComparison.OrdinalIgnoreCase))
            Catch e As Exception
                Return False
            End Try
        End Function

        Private Shared Function safeGetTypes(ByVal asm As Assembly) As IEnumerable(Of Type)
            Try
                Return asm.GetTypes()
            Catch ex As ReflectionTypeLoadException
                Return ex.Types.Where(Function(t) t IsNot Nothing)
            Catch ex As Exception
                Return Enumerable.Empty(Of Type)()
            End Try
        End Function

        Private Shared Function isConcreteCandidate(ByVal t As Type) As Boolean
            If Not t.IsClass OrElse t.IsAbstract OrElse t.IsGenericTypeDefinition OrElse t.ContainsGenericParameters Then Return False
            If t.IsDefined(GetType(CompilerGeneratedAttribute), False) Then Return False
            If t.Name.IndexOf("<"c) >= 0 OrElse t.Name.IndexOf("$"c) >= 0 Then Return False
            If GetType([Delegate]).IsAssignableFrom(t) OrElse GetType(Attribute).IsAssignableFrom(t) Then Return False
            Return True
        End Function

        Private Shared Function hasWrapperRoot(ByVal t As Type) As Boolean
            For Each i As Type In t.GetInterfaces()
                If JsonMappers.isWrapperRoot(i) Then Return True
            Next
            Dim b As Type = t.BaseType
            Do While b IsNot Nothing
                If JsonMappers.isWrapperRoot(b) Then Return True
                b = b.BaseType
            Loop
            Return False
        End Function

        ''' <summary>
        ''' True when values of the type are written field by field: classes of <c>org.datavec</c> in this library, the
        ''' implementations of the polymorphic types and registered classes (but not collections).
        ''' </summary>
        Friend Function isManaged(ByVal t As Type) As Boolean
            If t Is Nothing OrElse Not t.IsClass OrElse t.IsAbstract OrElse t.IsGenericTypeDefinition OrElse t.ContainsGenericParameters Then Return False
            If t Is GetType(String) OrElse GetType(IEnumerable).IsAssignableFrom(t) Then Return False
            If GetType([Delegate]).IsAssignableFrom(t) OrElse GetType(Exception).IsAssignableFrom(t) Then Return False
            If concreteSet.Contains(t) OrElse registered.Contains(t) Then Return True
            Return t.Assembly Is ownAssembly AndAlso t.Namespace IsNot Nothing AndAlso t.Namespace.StartsWith("DeepLearning4VB.org.datavec.", StringComparison.Ordinal)
        End Function

        ''' <summary>The known concrete types assignable to <paramref name="t"/> (including <paramref name="t"/> when concrete).</summary>
        Friend Function derivedTypes(ByVal t As Type) As Type()
            Return derivedCache.GetOrAdd(t, Function(k)
                                                Dim list As New List(Of Type)()
                                                If isManaged(k) AndAlso Not concreteSet.Contains(k) Then list.Add(k)
                                                For Each d As Type In concrete
                                                    If k.IsAssignableFrom(d) Then list.Add(d)
                                                Next
                                                Return list.ToArray()
                                            End Function)
        End Function

        ''' <summary>The <c>"@class"</c> name of a type: registered name, else its Java class name.</summary>
        Friend Function javaName(ByVal t As Type) As String
            Dim n As String = Nothing
            If explicitNames.TryGetValue(t, n) Then Return n
            Return defaultJavaName(t)
        End Function

        Friend Shared Function defaultJavaName(ByVal t As Type) As String
            Dim n As String = t.FullName.Replace("+"c, "$"c)
            If t.Assembly Is ownAssembly AndAlso n.StartsWith("DeepLearning4VB.", StringComparison.Ordinal) Then
                n = n.Substring("DeepLearning4VB.".Length)
                Const vbTransforms As String = "org.datavec.api.transform.transforms."
                If n.StartsWith(vbTransforms, StringComparison.Ordinal) Then
                    n = "org.datavec.api.transform.transform." & n.Substring(vbTransforms.Length)
                ElseIf n.StartsWith("org.nd4vb.", StringComparison.Ordinal) Then
                    n = "org.nd4j." & n.Substring("org.nd4vb.".Length)
                ElseIf n.StartsWith("org.deeplearning4vb.", StringComparison.Ordinal) Then
                    n = "org.deeplearning4j." & n.Substring("org.deeplearning4vb.".Length)
                End If
            End If
            Return n
        End Function

        ''' <summary>The type for a <c>"@class"</c> name, or Nothing.</summary>
        Friend Function typeForName(ByVal name As String) As Type
            If name Is Nothing Then Return Nothing
            Dim t As Type = Nothing
            If typeByName.TryGetValue(name, t) Then Return t
            Return Nothing
        End Function

        ''' <summary>The wrapper property name of an implementation of a wrapper-object type.</summary>
        Friend Function wrapperName(ByVal root As Type, ByVal t As Type) As String
            Dim n As String = Nothing
            If explicitNames.TryGetValue(t, n) Then Return n
            For Each a As JsonWrapperTypeAttribute In root.GetCustomAttributes(Of JsonWrapperTypeAttribute)(False)
                If a.SubType Is t Then Return a.Name
            Next
            Return t.Name
        End Function

        ''' <summary>The implementation of a wrapper-object type for a wrapper property name, or Nothing.</summary>
        Friend Function wrapperType(ByVal root As Type, ByVal name As String) As Type
            For Each a As JsonWrapperTypeAttribute In root.GetCustomAttributes(Of JsonWrapperTypeAttribute)(False)
                If a.Name = name AndAlso a.SubType IsNot Nothing AndAlso Not explicitNames.ContainsKey(a.SubType) Then Return a.SubType
            Next
            For Each d As Type In derivedTypes(root)
                If wrapperName(root, d) = name Then Return d
            Next
            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' Creates the System.Text.Json contracts: field-based contracts for the DataVec classes (see
    ''' <see cref="JsonMappers"/>) and <c>"@class"</c> polymorphism for the polymorphic types; everything else is
    ''' left to the default reflection resolver.
    ''' </summary>
    Friend NotInheritable Class DataVecTypeInfoResolver
        Implements IJsonTypeInfoResolver

        Private ReadOnly fallback As New DefaultJsonTypeInfoResolver()
        Private ReadOnly catalogValue As TypeCatalog
        Private Shared ReadOnly createMethod As MethodInfo = GetType(DataVecTypeInfoResolver).GetMethod("createObjectInfo", BindingFlags.NonPublic Or BindingFlags.Static)

        Friend Sub New(ByVal catalog As TypeCatalog)
            Me.catalogValue = catalog
        End Sub

        Friend ReadOnly Property Catalog As TypeCatalog
            Get
                Return catalogValue
            End Get
        End Property

        Public Function GetTypeInfo(ByVal type As Type, ByVal options As JsonSerializerOptions) As JsonTypeInfo Implements IJsonTypeInfoResolver.GetTypeInfo
            If catalogValue.isManaged(type) Then Return createManaged(type, options)
            Dim ti As JsonTypeInfo = fallback.GetTypeInfo(type, options)
            If ti IsNot Nothing AndAlso ti.Kind = JsonTypeInfoKind.Object AndAlso (type.IsInterface OrElse type.IsAbstract) AndAlso JsonMappers.hasClassRoot(type) Then
                applyPolymorphism(ti)
            End If
            Return ti
        End Function

        Private Function createManaged(ByVal type As Type, ByVal options As JsonSerializerOptions) As JsonTypeInfo
            Dim contract As TypeContract = TypeContract.forType(type)
            Dim ti As JsonTypeInfo = DirectCast(createMethod.MakeGenericMethod(type).Invoke(Nothing, New Object() {options, contract}), JsonTypeInfo)
            Dim tracks As Boolean = contract.TracksPresence
            For Each fc As FieldContract In contract.Fields
                Dim f As FieldContract = fc
                Dim p As JsonPropertyInfo = ti.CreateJsonPropertyInfo(f.JsonType, f.JsonName)
                p.Get = Function(o) f.getJsonValue(o, options)
                If tracks Then
                    p.Set = Sub(o, v)
                                f.setJsonValue(o, v, options)
                                TypeContract.markPresent(o, f.JsonName)
                            End Sub
                Else
                    p.Set = Sub(o, v) f.setJsonValue(o, v, options)
                End If
                If f.OmitNull Then p.ShouldSerialize = Function(o, v) v IsNot Nothing
                p.AttributeProvider = f.Field
                ti.Properties.Add(p)
            Next
            ti.OnDeserialized = Sub(o) contract.afterDeserialize(o)
            If JsonMappers.hasClassRoot(type) Then applyPolymorphism(ti)
            Return ti
        End Function

        ''' <summary>A contract with System.Text.Json's default object converter, whatever constructors the type has.</summary>
        Private Shared Function createObjectInfo(Of T)(ByVal options As JsonSerializerOptions, ByVal contract As TypeContract) As JsonTypeInfo
            Dim values As New JsonObjectInfoValues(Of T) With {
                .ObjectCreator = Function() DirectCast(contract.createInstance(), T),
                .PropertyMetadataInitializer = Function(ctx) Array.Empty(Of JsonPropertyInfo)(),
                .NumberHandling = options.NumberHandling
            }
            Return JsonMetadataServices.CreateObjectInfo(Of T)(options, values)
        End Function

        Private Sub applyPolymorphism(ByVal ti As JsonTypeInfo)
            Dim derived As Type() = catalogValue.derivedTypes(ti.Type)
            If derived.Length = 0 Then Return
            Dim opts As New JsonPolymorphismOptions()
            opts.TypeDiscriminatorPropertyName = "@class"
            opts.UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            opts.IgnoreUnrecognizedTypeDiscriminators = False
            For Each d As Type In derived
                opts.DerivedTypes.Add(New JsonDerivedType(d, catalogValue.javaName(d)))
            Next
            ti.PolymorphismOptions = opts
        End Sub

    End Class

End Namespace
