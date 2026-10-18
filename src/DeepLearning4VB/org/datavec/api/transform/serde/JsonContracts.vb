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
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports System.Text.Json.Serialization
Imports DateTimeFieldTypeDeserializer = DeepLearning4VB.org.datavec.api.util.jackson.DateTimeFieldTypeDeserializer
Imports DateTimeFieldTypeSerializer = DeepLearning4VB.org.datavec.api.util.jackson.DateTimeFieldTypeSerializer
Imports DateTimeFieldType = DeepLearning4VB.org.joda.time.DateTimeFieldType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest

Namespace org.datavec.api.transform.serde

    ''' <summary>
    ''' How one field of a class is mapped to a JSON property (Jackson's field visibility: every non-static,
    ''' non-transient field under its Java field name).
    ''' </summary>
    Friend NotInheritable Class FieldContract

        Friend ReadOnly Field As FieldInfo
        Friend ReadOnly JsonName As String
        ''' <summary>The CLR type exposed to System.Text.Json: the field type, or the adapter's JSON type.</summary>
        Friend ReadOnly JsonType As Type
        Friend ReadOnly Adapter As ValueAdapter
        ''' <summary>True when a null value is not written (Java <c>@JsonInclude(NON_NULL)</c>).</summary>
        Friend ReadOnly OmitNull As Boolean

        Friend Sub New(ByVal field As FieldInfo, ByVal jsonName As String, ByVal adapter As ValueAdapter, ByVal omitNull As Boolean)
            Me.Field = field
            Me.JsonName = jsonName
            Me.Adapter = adapter
            Me.JsonType = If(adapter Is Nothing, field.FieldType, adapter.JsonType)
            Me.OmitNull = omitNull
        End Sub

        ''' <summary>The value written to JSON for the field of <paramref name="obj"/>.</summary>
        Friend Function getJsonValue(ByVal obj As Object, ByVal options As JsonSerializerOptions) As Object
            Dim v As Object = Field.GetValue(obj)
            If Adapter Is Nothing OrElse v Is Nothing Then Return v
            Return Adapter.ToJson(v, options)
        End Function

        ''' <summary>Stores a value read from JSON into the field of <paramref name="obj"/>.</summary>
        Friend Sub setJsonValue(ByVal obj As Object, ByVal value As Object, ByVal options As JsonSerializerOptions)
            If Adapter IsNot Nothing AndAlso value IsNot Nothing Then
                value = Adapter.FromJson(value, options)
            End If
            If value Is Nothing AndAlso Field.FieldType.IsValueType AndAlso Nullable.GetUnderlyingType(Field.FieldType) Is Nothing Then
                ' Jackson: a JSON null for a primitive leaves the default value
                value = Activator.CreateInstance(Field.FieldType)
            End If
            Field.SetValue(obj, value)
        End Sub

    End Class

    ''' <summary>
    ''' Converts a field value to and from the shape Java DataVec wrote for it, for types System.Text.Json cannot be
    ''' given a converter for from VB (Joda time zones and field types, locales, wrapper-object type ids, t-digests).
    ''' </summary>
    Friend NotInheritable Class ValueAdapter

        Friend ReadOnly JsonType As Type
        Friend ReadOnly ToJson As Func(Of Object, JsonSerializerOptions, Object)
        Friend ReadOnly FromJson As Func(Of Object, JsonSerializerOptions, Object)

        Friend Sub New(ByVal jsonType As Type, ByVal toJson As Func(Of Object, JsonSerializerOptions, Object), ByVal fromJson As Func(Of Object, JsonSerializerOptions, Object))
            Me.JsonType = jsonType
            Me.ToJson = toJson
            Me.FromJson = fromJson
        End Sub

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ValueAdapter))

        ''' <summary>The adapter for a field of the given type, or Nothing when the field is written as it is.</summary>
        Friend Shared Function forFieldType(ByVal fieldType As Type) As ValueAdapter
            If fieldType Is GetType(DateTimeZone) Then
                ' Jackson's Joda module writes a time zone as its id
                Return New ValueAdapter(GetType(String),
                    Function(v, o) DirectCast(v, DateTimeZone).getID(),
                    Function(v, o) DateTimeZone.forID(DirectCast(v, String)))
            End If
            If fieldType Is GetType(DateTimeFieldType) Then
                ' util/jackson/DateTimeFieldTypeSerializer: {"fieldType" : "hourOfDay"}
                Return New ValueAdapter(GetType(JsonNode),
                    Function(v, o) DateTimeFieldTypeSerializer.serialize(DirectCast(v, DateTimeFieldType)),
                    Function(v, o) DateTimeFieldTypeDeserializer.deserialize(DirectCast(v, JsonNode)))
            End If
            If fieldType Is GetType(CultureInfo) Then
                ' java.util.Locale is written with toString(): "en_US", "fr", ""
                Return New ValueAdapter(GetType(String),
                    Function(v, o) DirectCast(v, CultureInfo).Name.Replace("-"c, "_"c),
                    Function(v, o) CultureInfo.GetCultureInfo(DirectCast(v, String).Replace("_"c, "-"c)))
            End If
            If fieldType Is GetType(TDigest) Then
                Return New ValueAdapter(GetType(JsonNode), AddressOf digestToJson, AddressOf digestFromJson)
            End If
            If JsonMappers.isWrapperRoot(fieldType) Then
                Return New ValueAdapter(GetType(JsonNode),
                    Function(v, o) JsonMappers.wrapValue(v, fieldType, o),
                    Function(v, o) JsonMappers.unwrapValue(DirectCast(v, JsonNode), fieldType, o))
            End If
            Dim elementType As Type = Nothing
            If isListOf(fieldType, elementType) AndAlso JsonMappers.isWrapperRoot(elementType) Then
                Dim et As Type = elementType
                Return New ValueAdapter(GetType(JsonArray),
                    Function(v, o)
                        Dim arr As New JsonArray()
                        For Each item As Object In DirectCast(v, IEnumerable)
                            arr.Add(JsonMappers.wrapValue(item, et, o))
                        Next
                        Return arr
                    End Function,
                    Function(v, o)
                        Dim items As New List(Of Object)()
                        For Each n As JsonNode In DirectCast(v, JsonArray)
                            items.Add(JsonMappers.unwrapValue(n, et, o))
                        Next
                        Return JsonContracts.convertTo(items, fieldType)
                    End Function)
            End If
            Return Nothing
        End Function

        ''' <summary>True for arrays and the list/collection interfaces and classes of an element type.</summary>
        Friend Shared Function isListOf(ByVal t As Type, ByRef elementType As Type) As Boolean
            If t.IsArray AndAlso t.GetArrayRank() = 1 Then
                elementType = t.GetElementType()
                Return True
            End If
            If t.IsGenericType Then
                Dim def As Type = t.GetGenericTypeDefinition()
                If def Is GetType(IList(Of )) OrElse def Is GetType(List(Of )) OrElse def Is GetType(ICollection(Of )) OrElse def Is GetType(IEnumerable(Of )) Then
                    elementType = t.GetGenericArguments()(0)
                    Return True
                End If
            End If
            Return False
        End Function

        Private Shared Function digestToJson(ByVal v As Object, ByVal o As JsonSerializerOptions) As Object
            Return System.Text.Json.JsonSerializer.SerializeToNode(v, GetType(TDigest), o)
        End Function

        Private Shared Function digestFromJson(ByVal v As Object, ByVal o As JsonSerializerOptions) As Object
            Dim node As JsonNode = DirectCast(v, JsonNode)
            Dim obj As JsonObject = TryCast(node, JsonObject)
            If obj IsNot Nothing AndAlso obj.Count = 1 AndAlso obj.ContainsKey("digest") Then
                ' Java's TDigestSerializer wrote the Java serialization of a MergingDigest in base64: not readable here
                log.warn("Ignoring a t-digest written by Java DataVec (Java object serialization cannot be read); quantiles are not available for this column")
                Return Nothing
            End If
            Return node.Deserialize(GetType(TDigest), o)
        End Function

    End Class

    ''' <summary>
    ''' The JSON contract of a class: its serialized fields in Jackson's order, the constructor Jackson would use
    ''' (the one marked <c>&lt;JsonConstructor&gt;</c>) and the post-deserialization hooks.
    ''' </summary>
    Friend NotInheritable Class TypeContract

        Private Shared ReadOnly cache As New ConcurrentDictionary(Of Type, TypeContract)()
        Private Shared ReadOnly ownAssembly As Assembly = GetType(TypeContract).Assembly

        ''' <summary>JSON property names set from JSON on objects whose creator has not run yet.</summary>
        Private Shared ReadOnly presentNames As New ConditionalWeakTable(Of Object, HashSet(Of String))()

        Friend ReadOnly Type As Type
        Friend ReadOnly Fields As IList(Of FieldContract)
        Friend ReadOnly ByName As IDictionary(Of String, FieldContract)
        Friend ReadOnly Creator As ConstructorInfo
        Private ReadOnly creatorParams As FieldContract()
        Private ReadOnly creatorParamSet As HashSet(Of FieldContract)
        Private ReadOnly defaultCtor As ConstructorInfo
        Private ReadOnly deserializedHooks As MethodInfo()

        Friend Shared Function forType(ByVal t As Type) As TypeContract
            Return cache.GetOrAdd(t, Function(k) New TypeContract(k))
        End Function

        Private Sub New(ByVal t As Type)
            Me.Type = t
            Dim omitNulls As Boolean = hasIncludeNonNull(t)
            Dim ignored As New HashSet(Of String)(StringComparer.Ordinal)
            Dim ignoreAttr As JsonIgnorePropertiesAttribute = t.GetCustomAttribute(Of JsonIgnorePropertiesAttribute)(True)
            If ignoreAttr IsNot Nothing Then
                For Each n As String In ignoreAttr.Names
                    ignored.Add(n)
                Next
            End If

            ' Fields of the class and its base classes; a field of a derived class hides a base field of the same name
            Dim found As New Dictionary(Of String, FieldContract)(StringComparer.Ordinal)
            Dim current As Type = t
            Do While current IsNot Nothing AndAlso current IsNot GetType(Object)
                For Each f As FieldInfo In current.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.DeclaredOnly)
                    Dim fieldOmitNull As Boolean = omitNulls
                    Dim name As String = jsonNameOf(f, fieldOmitNull)
                    If name Is Nothing OrElse ignored.Contains(name) OrElse found.ContainsKey(name) Then Continue For
                    found(name) = New FieldContract(f, name, ValueAdapter.forFieldType(f.FieldType), fieldOmitNull)
                Next
                current = current.BaseType
            Loop

            ' The creator: the constructor marked <JsonConstructor> (Jackson's @JsonProperty constructor)
            For Each c As ConstructorInfo In t.GetConstructors(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                If c.IsDefined(GetType(JsonConstructorAttribute), False) Then
                    Creator = c
                    Exit For
                End If
            Next
            defaultCtor = t.GetConstructor(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic, Nothing, Type.EmptyTypes, Nothing)

            Dim creatorNames As New HashSet(Of String)(StringComparer.Ordinal)
            creatorParamSet = New HashSet(Of FieldContract)()
            If Creator IsNot Nothing Then
                Dim ps As ParameterInfo() = Creator.GetParameters()
                creatorParams = New FieldContract(ps.Length - 1) {}
                For i As Integer = 0 To ps.Length - 1
                    Dim fc As FieldContract = lookup(found, parameterJsonName(ps(i)))
                    creatorParams(i) = fc
                    If fc IsNot Nothing Then
                        creatorNames.Add(fc.JsonName)
                        creatorParamSet.Add(fc)
                    End If
                Next
            End If

            ' Jackson (SORT_PROPERTIES_ALPHABETICALLY): creator properties first, then the others, each group sorted
            Dim ordered As New List(Of FieldContract)()
            ordered.AddRange(found.Values.Where(Function(fc) creatorNames.Contains(fc.JsonName)).OrderBy(Function(fc) fc.JsonName, StringComparer.Ordinal))
            ordered.AddRange(found.Values.Where(Function(fc) Not creatorNames.Contains(fc.JsonName)).OrderBy(Function(fc) fc.JsonName, StringComparer.Ordinal))
            Fields = ordered.AsReadOnly()
            ByName = found

            ' <OnDeserialized> methods (the BinaryFormatter hooks rebuild transient state), base class first
            Dim hooks As New List(Of MethodInfo)()
            Dim chain As New List(Of Type)()
            current = t
            Do While current IsNot Nothing AndAlso current IsNot GetType(Object)
                chain.Insert(0, current)
                current = current.BaseType
            Loop
            For Each c As Type In chain
                For Each m As MethodInfo In c.GetMethods(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.DeclaredOnly)
                    If m.IsDefined(GetType(OnDeserializedAttribute), False) Then
                        Dim mps As ParameterInfo() = m.GetParameters()
                        If mps.Length = 1 AndAlso mps(0).ParameterType Is GetType(StreamingContext) Then hooks.Add(m)
                    End If
                Next
            Next
            deserializedHooks = hooks.ToArray()
        End Sub

        ''' <summary>True when the field values of this class are tracked while it is read (it has a creator).</summary>
        Friend ReadOnly Property TracksPresence As Boolean
            Get
                Return Creator IsNot Nothing
            End Get
        End Property

        Private Shared Function lookup(ByVal found As Dictionary(Of String, FieldContract), ByVal name As String) As FieldContract
            Dim fc As FieldContract = Nothing
            If found.TryGetValue(name, fc) Then Return fc
            For Each kv In found
                If String.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase) Then Return kv.Value
            Next
            Return Nothing
        End Function

        Private Shared Function parameterJsonName(ByVal p As ParameterInfo) As String
            Dim attr As JsonPropertyAttribute = p.GetCustomAttribute(Of JsonPropertyAttribute)()
            Return If(attr IsNot Nothing, attr.Name, p.Name)
        End Function

        ''' <summary>True when the type, a base class or an implemented interface has <see cref="JsonIncludeNonNullAttribute"/>.</summary>
        Friend Shared Function hasIncludeNonNull(ByVal t As Type) As Boolean
            If t.GetCustomAttribute(Of JsonIncludeNonNullAttribute)(True) IsNot Nothing Then Return True
            For Each i As Type In t.GetInterfaces()
                If i.IsDefined(GetType(JsonIncludeNonNullAttribute), False) Then Return True
            Next
            Return False
        End Function

        ''' <summary>
        ''' The JSON name of a field, or Nothing when the field is not serialized. Rules, in order: transient
        ''' (<c>&lt;NonSerialized&gt;</c>), static-like and delegate fields are skipped; <c>&lt;JsonIgnore&gt;</c> skips the
        ''' field (with <c>Condition:=WhenWritingNull</c> it only omits nulls); <c>&lt;JsonPropertyName&gt;</c> names it;
        ''' the backing field of an auto-property takes the attributes of the property and its name with a lower-case
        ''' first letter; in this library, the converter's renames <c>xField</c> and <c>x_Conflict</c> become <c>x</c>.
        ''' </summary>
        Private Shared Function jsonNameOf(ByVal f As FieldInfo, ByRef omitNull As Boolean) As String
            ' <NonSerialized> is a pseudo-attribute (FieldInfo.IsNotSerialized is obsolete on .NET 10)
            If f.IsDefined(GetType(NonSerializedAttribute), False) OrElse f.IsLiteral Then Return Nothing
            If GetType([Delegate]).IsAssignableFrom(f.FieldType) Then Return Nothing
            Dim name As String = f.Name
            Dim attrSource As MemberInfo = f
            Dim isOwn As Boolean = f.DeclaringType.Assembly Is ownAssembly
            If f.IsDefined(GetType(CompilerGeneratedAttribute), False) Then
                ' VB auto-property backing field "_Name"
                If Not name.StartsWith("_", StringComparison.Ordinal) Then Return Nothing
                Dim prop As PropertyInfo = f.DeclaringType.GetProperty(name.Substring(1), BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.DeclaredOnly)
                If prop Is Nothing Then Return Nothing
                attrSource = prop
                name = decapitalize(prop.Name)
            ElseIf name.IndexOf("$"c) >= 0 OrElse name.IndexOf("<"c) >= 0 Then
                Return Nothing
            ElseIf isOwn Then
                If name.EndsWith("_Conflict", StringComparison.Ordinal) AndAlso name.Length > 9 Then
                    name = name.Substring(0, name.Length - 9)
                ElseIf name.EndsWith("Field", StringComparison.Ordinal) AndAlso name.Length > 5 Then
                    name = name.Substring(0, name.Length - 5)
                End If
            End If
            Dim ignore As JsonIgnoreAttribute = attrSource.GetCustomAttribute(Of JsonIgnoreAttribute)()
            If ignore IsNot Nothing Then
                Select Case ignore.Condition
                    Case JsonIgnoreCondition.Always
                        Return Nothing
                    Case JsonIgnoreCondition.WhenWritingNull, JsonIgnoreCondition.WhenWritingDefault
                        omitNull = True
                    Case JsonIgnoreCondition.Never
                        omitNull = False
                End Select
            End If
            Dim nameAttr As JsonPropertyNameAttribute = attrSource.GetCustomAttribute(Of JsonPropertyNameAttribute)()
            If nameAttr IsNot Nothing Then name = nameAttr.Name
            Return name
        End Function

        Private Shared Function decapitalize(ByVal s As String) As String
            If String.IsNullOrEmpty(s) Then Return s
            Return Char.ToLowerInvariant(s(0)) & s.Substring(1)
        End Function

        ''' <summary>A new instance to read JSON into: uninitialized when a creator will run, else the no-arg constructor.</summary>
        Friend Function createInstance() As Object
            If Creator Is Nothing AndAlso defaultCtor IsNot Nothing Then
                Try
                    Return defaultCtor.Invoke(Nothing)
                Catch e As TargetInvocationException
                    Throw New JsonException("The constructor of " & Type.FullName & " failed: " & e.InnerException.Message, e.InnerException)
                End Try
            End If
            Dim obj As Object = uninitialized(Type)
            If Creator IsNot Nothing Then presentNames.Add(obj, New HashSet(Of String)(StringComparer.Ordinal))
            Return obj
        End Function

        Private Shared Function uninitialized(ByVal t As Type) As Object
#If NETFRAMEWORK Then
            Return FormatterServices.GetUninitializedObject(t)
#Else
            Return RuntimeHelpers.GetUninitializedObject(t)
#End If
        End Function

        ''' <summary>Records that a property was read from JSON (only for classes with a creator).</summary>
        Friend Shared Sub markPresent(ByVal obj As Object, ByVal name As String)
            Dim names As HashSet(Of String) = Nothing
            If presentNames.TryGetValue(obj, names) Then names.Add(name)
        End Sub

        ''' <summary>
        ''' Completes an object read from JSON: runs the creator on it (as Jackson calls the <c>@JsonProperty</c>
        ''' constructor) with the values read for its parameters, then restores the other fields read from JSON (Jackson
        ''' sets them after the creator), and finally calls the <c>&lt;OnDeserialized&gt;</c> hooks.
        ''' </summary>
        Friend Sub afterDeserialize(ByVal obj As Object)
            If Creator IsNot Nothing Then
                Dim names As HashSet(Of String) = Nothing
                If Not presentNames.TryGetValue(obj, names) Then names = New HashSet(Of String)(StringComparer.Ordinal)
                presentNames.Remove(obj)
                Dim ps As ParameterInfo() = Creator.GetParameters()
                Dim args(ps.Length - 1) As Object
                For i As Integer = 0 To ps.Length - 1
                    Dim fc As FieldContract = creatorParams(i)
                    Dim v As Object = Nothing
                    If fc IsNot Nothing AndAlso names.Contains(fc.JsonName) Then v = fc.Field.GetValue(obj)
                    args(i) = JsonContracts.convertTo(v, ps(i).ParameterType)
                Next
                Dim restore As New List(Of KeyValuePair(Of FieldContract, Object))()
                For Each fc As FieldContract In Fields
                    If names.Contains(fc.JsonName) AndAlso Not creatorParamSet.Contains(fc) Then
                        restore.Add(New KeyValuePair(Of FieldContract, Object)(fc, fc.Field.GetValue(obj)))
                    End If
                Next
                Try
                    Creator.Invoke(obj, args)
                Catch e As TargetInvocationException
                    Throw New JsonException("Could not create " & Type.FullName & " from JSON: " & e.InnerException.Message, e.InnerException)
                End Try
                For Each kv In restore
                    kv.Key.Field.SetValue(obj, kv.Value)
                Next
            End If
            For Each hook As MethodInfo In deserializedHooks
                Try
                    hook.Invoke(obj, New Object() {New StreamingContext()})
                Catch e As TargetInvocationException
                    Throw New JsonException("Could not complete " & Type.FullName & " after reading it from JSON: " & e.InnerException.Message, e.InnerException)
                End Try
            Next
            Dim callback As IJsonOnDeserialized = TryCast(obj, IJsonOnDeserialized)
            If callback IsNot Nothing Then callback.OnDeserialized()
        End Sub

    End Class

    ''' <summary>Value conversions shared by the contracts.</summary>
    Friend NotInheritable Class JsonContracts

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Converts a field value to the type of a creator parameter (or of a field): collections are copied into the
        ''' requested array, list or set type, numbers are converted, Nothing becomes the default value.
        ''' </summary>
        Friend Shared Function convertTo(ByVal value As Object, ByVal target As Type) As Object
            If value Is Nothing Then
                If target.IsValueType AndAlso Nullable.GetUnderlyingType(target) Is Nothing Then Return Activator.CreateInstance(target)
                Return Nothing
            End If
            If target.IsInstanceOfType(value) Then Return value
            Dim underlying As Type = Nullable.GetUnderlyingType(target)
            If underlying IsNot Nothing Then target = underlying
            If target.IsInstanceOfType(value) Then Return value
            If TypeOf value Is IEnumerable AndAlso Not TypeOf value Is String Then
                Dim items As New List(Of Object)()
                For Each item As Object In DirectCast(value, IEnumerable)
                    items.Add(item)
                Next
                Dim elementType As Type = Nothing
                If target.IsArray Then
                    elementType = target.GetElementType()
                    Dim arr As Array = Array.CreateInstance(elementType, items.Count)
                    For i As Integer = 0 To items.Count - 1
                        arr.SetValue(convertTo(items(i), elementType), i)
                    Next
                    Return arr
                End If
                If target.IsGenericType Then
                    Dim def As Type = target.GetGenericTypeDefinition()
                    elementType = target.GetGenericArguments()(0)
                    Dim coll As Object = Nothing
                    If def Is GetType(ISet(Of )) OrElse def Is GetType(HashSet(Of )) Then
                        coll = Activator.CreateInstance(GetType(HashSet(Of )).MakeGenericType(elementType))
                    ElseIf def Is GetType(IList(Of )) OrElse def Is GetType(List(Of )) OrElse def Is GetType(ICollection(Of )) OrElse def Is GetType(IEnumerable(Of )) OrElse def Is GetType(IReadOnlyList(Of )) OrElse def Is GetType(IReadOnlyCollection(Of )) Then
                        coll = Activator.CreateInstance(GetType(List(Of )).MakeGenericType(elementType))
                    ElseIf Not target.IsAbstract AndAlso Not target.IsInterface AndAlso target.GetConstructor(Type.EmptyTypes) IsNot Nothing Then
                        coll = Activator.CreateInstance(target)
                    End If
                    If coll IsNot Nothing Then
                        Dim add As MethodInfo = coll.GetType().GetMethod("Add", New Type() {elementType})
                        If add IsNot Nothing Then
                            For Each item As Object In items
                                add.Invoke(coll, New Object() {convertTo(item, elementType)})
                            Next
                            If target.IsInstanceOfType(coll) Then Return coll
                        End If
                    End If
                End If
            End If
            If target.IsEnum AndAlso TypeOf value Is String Then Return [Enum].Parse(target, DirectCast(value, String))
            If TypeOf value Is IConvertible AndAlso GetType(IConvertible).IsAssignableFrom(target) Then
                Return Convert.ChangeType(value, target, CultureInfo.InvariantCulture)
            End If
            Throw New JsonException("Cannot convert a value of type " & value.GetType().FullName & " to " & target.FullName)
        End Function

    End Class

End Namespace
