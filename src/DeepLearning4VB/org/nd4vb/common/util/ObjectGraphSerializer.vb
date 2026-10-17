Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Numerics
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.RegularExpressions

' DeepLearning4VB: a replacement for the BinaryFormatter, which Microsoft marks unsafe for untrusted input and
' which .NET 9 and later removed. It serves SerializationUtils, the .NET counterpart of Java object
' serialization (ObjectOutputStream/ObjectInputStream) in the ported code.

Namespace org.nd4vb.common.util

    ''' <summary>
    ''' Binary object-graph serializer behind <see cref="SerializationUtils"/>. It writes the instance fields of
    ''' <c>[Serializable]</c> types (honouring <c>[NonSerialized]</c> and the <c>OnSerializing</c>,
    ''' <c>OnSerialized</c>, <c>OnDeserializing</c>, <c>OnDeserialized</c> and <see cref="IDeserializationCallback"/>
    ''' callbacks), keeps object identity and cycles, and handles primitives, strings, enums, arrays, the standard
    ''' generic collections (<c>List</c>, <c>HashSet</c>, <c>SortedSet</c>, <c>LinkedList</c>, <c>Queue</c>,
    ''' <c>Stack</c>, <c>Dictionary</c>, <c>SortedDictionary</c>, <c>SortedList</c>, <c>ConcurrentDictionary</c>,
    ''' <c>ArrayList</c>, <c>Hashtable</c>) and <see cref="Type"/>, <see cref="Uri"/>, <see cref="CultureInfo"/>,
    ''' <see cref="TimeZoneInfo"/>, <see cref="Encoding"/>, <see cref="Regex"/>, <see cref="BigInteger"/> and
    ''' <see cref="Random"/> values explicitly. Type names are written without assembly names, so data written on
    ''' .NET Framework reads on .NET 10 and the other way round (a <see cref="Random"/> is the exception: its
    ''' state only round-trips on the same runtime, otherwise a new unseeded instance is returned).
    ''' Delegate-typed fields are not written (they read back as <c>Nothing</c>).
    ''' Deserialization only instantiates <c>[Serializable]</c> types and the types listed above, but it still
    ''' runs code of those types (callbacks, collection comparers): only read data from trusted sources.
    ''' </summary>
    Friend NotInheritable Class ObjectGraphSerializer

        Private Shared ReadOnly Magic As Byte() = Encoding.ASCII.GetBytes("DL4VBOBJ")
        Private Const FormatVersion As Byte = 1

        Private Const T_NULL As Byte = 0
        Private Const T_REF As Byte = 1
        Private Const T_OBJECT As Byte = 2
        Private Const T_STRUCT As Byte = 3
        Private Const T_BOOL As Byte = 4
        Private Const T_BYTE As Byte = 5
        Private Const T_SBYTE As Byte = 6
        Private Const T_INT16 As Byte = 7
        Private Const T_UINT16 As Byte = 8
        Private Const T_INT32 As Byte = 9
        Private Const T_UINT32 As Byte = 10
        Private Const T_INT64 As Byte = 11
        Private Const T_UINT64 As Byte = 12
        Private Const T_SINGLE As Byte = 13
        Private Const T_DOUBLE As Byte = 14
        Private Const T_DECIMAL As Byte = 15
        Private Const T_CHAR As Byte = 16
        Private Const T_STRING As Byte = 17
        Private Const T_DATETIME As Byte = 18
        Private Const T_TIMESPAN As Byte = 19
        Private Const T_GUID As Byte = 20
        Private Const T_ENUM As Byte = 21
        Private Const T_ARRAY As Byte = 22
        Private Const T_TYPE As Byte = 23
        Private Const T_URI As Byte = 24
        Private Const T_CULTURE As Byte = 25
        Private Const T_TIMEZONE As Byte = 26
        Private Const T_ENCODING As Byte = 27
        Private Const T_REGEX As Byte = 28
        Private Const T_BIGINT As Byte = 29
        Private Const T_DTOFFSET As Byte = 30
        Private Const T_LIST As Byte = 31
        Private Const T_DICT As Byte = 32
        Private Const T_STRCOMPARER As Byte = 33
        Private Const T_RANDOM As Byte = 34

#If NETFRAMEWORK Then
        Private Const RuntimeFlavor As String = ".NETFramework"
#Else
        Private Const RuntimeFlavor As String = ".NETCore"
#End If

        Private Sub New()
        End Sub

        ''' <summary>Writes the object graph rooted at <paramref name="graph"/> to the stream (left open).</summary>
        Public Shared Sub Serialize(ByVal stream As Stream, ByVal graph As Object)
            If stream Is Nothing Then Throw New ArgumentNullException(NameOf(stream))
            Using bw As New BinaryWriter(stream, Encoding.UTF8, True)
                bw.Write(Magic)
                bw.Write(FormatVersion)
                Dim w As New GraphWriter(bw, False)
                w.WriteValue(graph)
                bw.Flush()
            End Using
        End Sub

        ''' <summary>Reads an object graph written by <see cref="Serialize"/> from the stream (left open).</summary>
        Public Shared Function Deserialize(ByVal stream As Stream) As Object
            If stream Is Nothing Then Throw New ArgumentNullException(NameOf(stream))
            Using br As New BinaryReader(stream, Encoding.UTF8, True)
                Dim head As Byte() = br.ReadBytes(Magic.Length)
                If head.Length <> Magic.Length OrElse Not head.SequenceEqual(Magic) Then
                    Throw New SerializationException("The data was not written by DeepLearning4VB's SerializationUtils (unknown header). Data written by the BinaryFormatter in earlier versions cannot be read.")
                End If
                Dim version As Byte = br.ReadByte()
                If version <> FormatVersion Then
                    Throw New SerializationException("Unsupported serialization format version " & version & ".")
                End If
                Dim r As New GraphReader(br, False)
                Dim result As Object = r.ReadValue()
                r.RunDeserializedCallbacks()
                Return result
            End Using
        End Function

        ' ------------------------------------------------------------------ shared reflection helpers

        Private NotInheritable Class IdentityComparer
            Implements IEqualityComparer(Of Object)

            Public Shared ReadOnly Instance As New IdentityComparer()

            Public Overloads Function Equals(ByVal x As Object, ByVal y As Object) As Boolean Implements IEqualityComparer(Of Object).Equals
                Return Object.ReferenceEquals(x, y)
            End Function

            Public Overloads Function GetHashCode(ByVal obj As Object) As Integer Implements IEqualityComparer(Of Object).GetHashCode
                Return RuntimeHelpers.GetHashCode(obj)
            End Function
        End Class

        Private Shared ReadOnly FieldCache As New ConcurrentDictionary(Of Type, FieldInfo())()
        Private Shared ReadOnly FieldKeyCache As New ConcurrentDictionary(Of Type, Dictionary(Of String, FieldInfo))()
        Private Shared ReadOnly CallbackCache As New ConcurrentDictionary(Of String, MethodInfo())()
        Private Shared ReadOnly TypeNameCache As New ConcurrentDictionary(Of String, Type)()

        Private Shared ReadOnly ListLikeDefinitions As Type() = {GetType(List(Of )), GetType(HashSet(Of )), GetType(SortedSet(Of )), GetType(LinkedList(Of )), GetType(Queue(Of )), GetType(Stack(Of ))}
        Private Shared ReadOnly DictLikeDefinitions As Type() = {GetType(Dictionary(Of ,)), GetType(SortedDictionary(Of ,)), GetType(SortedList(Of ,)), GetType(ConcurrentDictionary(Of ,))}

        Private Shared ReadOnly ProbeAssemblies As Assembly() = New Assembly() {
            GetType(ObjectGraphSerializer).Assembly, GetType(Object).Assembly, GetType(Uri).Assembly,
            GetType(LinkedList(Of )).Assembly, GetType(HashSet(Of )).Assembly, GetType(SortedSet(Of )).Assembly,
            GetType(Queue(Of )).Assembly, GetType(ConcurrentDictionary(Of ,)).Assembly, GetType(BigInteger).Assembly,
            GetType(Regex).Assembly}.Distinct().ToArray()

        ''' <summary>
        ''' True when the type carries <c>[Serializable]</c> in its metadata (Type.IsSerializable is obsolete on .NET 10).
        ''' Primitives, strings and arrays are handled before this check.
        ''' </summary>
        Friend Shared Function IsMarkedSerializable(ByVal t As Type) As Boolean
#Disable Warning SYSLIB0050 ' reading the [Serializable] metadata flag is this serializer's purpose
            Return (t.Attributes And TypeAttributes.Serializable) <> 0
#Enable Warning SYSLIB0050
        End Function

        ''' <summary>True when the field carries <c>[NonSerialized]</c> in its metadata.</summary>
        Private Shared Function IsMarkedNotSerialized(ByVal f As FieldInfo) As Boolean
#Disable Warning SYSLIB0050
            Return (f.Attributes And FieldAttributes.NotSerialized) <> 0
#Enable Warning SYSLIB0050
        End Function

        Private Shared Function IsDelegateType(ByVal t As Type) As Boolean
            Return GetType([Delegate]).IsAssignableFrom(t)
        End Function

        Private Shared Function FieldKey(ByVal f As FieldInfo) As String
            Return f.DeclaringType.Name & ":" & f.Name
        End Function

        ''' <summary>The serializable instance fields of the type and its base types, base types first.</summary>
        Private Shared Function SerializableFields(ByVal t As Type) As FieldInfo()
            Return FieldCache.GetOrAdd(t, Function(type)
                                              Dim chain As New List(Of Type)()
                                              Dim cur As Type = type
                                              While cur IsNot Nothing AndAlso cur IsNot GetType(Object) AndAlso cur IsNot GetType(ValueType)
                                                  chain.Add(cur)
                                                  cur = cur.BaseType
                                              End While
                                              chain.Reverse()
                                              Dim result As New List(Of FieldInfo)()
                                              For Each ct As Type In chain
                                                  Dim declared = ct.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.DeclaredOnly).
                                                      Where(Function(f) Not IsMarkedNotSerialized(f) AndAlso Not IsDelegateType(f.FieldType)).
                                                      OrderBy(Function(f) f.Name, StringComparer.Ordinal)
                                                  result.AddRange(declared)
                                              Next
                                              Return result.ToArray()
                                          End Function)
        End Function

        Private Shared Function FieldsByKey(ByVal t As Type) As Dictionary(Of String, FieldInfo)
            Return FieldKeyCache.GetOrAdd(t, Function(type)
                                                 Dim d As New Dictionary(Of String, FieldInfo)(StringComparer.Ordinal)
                                                 For Each f As FieldInfo In SerializableFields(type)
                                                     d(FieldKey(f)) = f
                                                 Next
                                                 Return d
                                             End Function)
        End Function

        ''' <summary>Methods marked with the given callback attribute, base types first.</summary>
        Private Shared Function Callbacks(ByVal t As Type, ByVal attribute As Type) As MethodInfo()
            Return CallbackCache.GetOrAdd(t.AssemblyQualifiedName & "|" & attribute.Name, Function(key)
                                                                                                Dim chain As New List(Of Type)()
                                                                                                Dim cur As Type = t
                                                                                                While cur IsNot Nothing AndAlso cur IsNot GetType(Object)
                                                                                                    chain.Add(cur)
                                                                                                    cur = cur.BaseType
                                                                                                End While
                                                                                                chain.Reverse()
                                                                                                Dim result As New List(Of MethodInfo)()
                                                                                                For Each ct As Type In chain
                                                                                                    For Each m As MethodInfo In ct.GetMethods(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.DeclaredOnly)
                                                                                                        If m.IsDefined(attribute, False) Then result.Add(m)
                                                                                                    Next
                                                                                                Next
                                                                                                Return result.ToArray()
                                                                                            End Function)
        End Function

        Private Shared Sub InvokeCallbacks(ByVal target As Object, ByVal attribute As Type)
            Dim methods As MethodInfo() = Callbacks(target.GetType(), attribute)
            If methods.Length = 0 Then Return
            Dim ctx As StreamingContext = Nothing ' the callbacks of the ported types ignore the context
            For Each m As MethodInfo In methods
                m.Invoke(target, New Object() {ctx})
            Next
        End Sub

        ''' <summary>0: not a handled collection, 1: list-like, 2: dictionary-like.</summary>
        Private Shared Function CollectionKind(ByVal t As Type) As Integer
            If t Is GetType(ArrayList) Then Return 1
            If t Is GetType(Hashtable) Then Return 2
            If Not t.IsGenericType Then Return 0
            Dim def As Type = t.GetGenericTypeDefinition()
            If ListLikeDefinitions.Contains(def) Then Return 1
            If DictLikeDefinitions.Contains(def) Then Return 2
            Return 0
        End Function

        Private Shared Function IsPrimitiveElement(ByVal t As Type) As Boolean
            If t.IsEnum Then Return False
            Select Case Type.GetTypeCode(t)
                Case TypeCode.Boolean, TypeCode.Char, TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16,
                     TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal
                    Return True
            End Select
            Return False
        End Function

        Private Shared Function CreateUninitialized(ByVal t As Type) As Object
#If NETFRAMEWORK Then
            Return FormatterServices.GetUninitializedObject(t)
#Else
            Return RuntimeHelpers.GetUninitializedObject(t)
#End If
        End Function

        Private Shared Function ResolveTypeName(ByVal name As String) As Type
            Return TypeNameCache.GetOrAdd(name, Function(n)
                                                    Dim t As Type = Type.GetType(n, False)
                                                    If t IsNot Nothing Then Return t
                                                    For Each asm As Assembly In ProbeAssemblies
                                                        t = asm.GetType(n, False)
                                                        If t IsNot Nothing Then Return t
                                                    Next
                                                    For Each asm As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                                                        If asm.IsDynamic Then Continue For
                                                        t = asm.GetType(n, False)
                                                        If t IsNot Nothing Then Return t
                                                    Next
                                                    Throw New SerializationException("Cannot find the type '" & n & "' in the loaded assemblies.")
                                                End Function)
        End Function

        ''' <summary>The comparer of a handled collection, or Nothing when it is the default comparer.</summary>
        Private Shared Function NonDefaultComparer(ByVal collection As Object) As Object
            Dim t As Type = collection.GetType()
            If Not t.IsGenericType Then Return Nothing
            Dim def As Type = t.GetGenericTypeDefinition()
            Dim prop As PropertyInfo = t.GetProperty("Comparer", BindingFlags.Instance Or BindingFlags.Public)
            If prop Is Nothing Then Return Nothing
            Dim cmp As Object = prop.GetValue(collection, Nothing)
            If cmp Is Nothing Then Return Nothing
            Dim keyType As Type = t.GetGenericArguments()(0)
            Dim defaultCmp As Object
            If def Is GetType(SortedSet(Of )) OrElse def Is GetType(SortedDictionary(Of ,)) OrElse def Is GetType(SortedList(Of ,)) Then
                defaultCmp = GetType(Comparer(Of )).MakeGenericType(keyType).GetProperty("Default").GetValue(Nothing, Nothing)
            Else
                defaultCmp = GetType(EqualityComparer(Of )).MakeGenericType(keyType).GetProperty("Default").GetValue(Nothing, Nothing)
            End If
            If cmp.Equals(defaultCmp) Then Return Nothing
            Return cmp
        End Function

        Private Shared ReadOnly KnownStringComparers As KeyValuePair(Of String, StringComparer)() = {
            New KeyValuePair(Of String, StringComparer)("Ordinal", StringComparer.Ordinal),
            New KeyValuePair(Of String, StringComparer)("OrdinalIgnoreCase", StringComparer.OrdinalIgnoreCase),
            New KeyValuePair(Of String, StringComparer)("InvariantCulture", StringComparer.InvariantCulture),
            New KeyValuePair(Of String, StringComparer)("InvariantCultureIgnoreCase", StringComparer.InvariantCultureIgnoreCase),
            New KeyValuePair(Of String, StringComparer)("CurrentCulture", StringComparer.CurrentCulture),
            New KeyValuePair(Of String, StringComparer)("CurrentCultureIgnoreCase", StringComparer.CurrentCultureIgnoreCase)}

        ' ------------------------------------------------------------------ writer

        Private NotInheritable Class GraphWriter
            Private ReadOnly bw As BinaryWriter
            Private ReadOnly trusted As Boolean
            Private ReadOnly objectIds As New Dictionary(Of Object, Integer)(IdentityComparer.Instance)
            Private ReadOnly typeIds As New Dictionary(Of Type, Integer)()
            Private ReadOnly layoutIds As New Dictionary(Of Type, Integer)()

            Public Sub New(ByVal bw As BinaryWriter, ByVal trusted As Boolean)
                Me.bw = bw
                Me.trusted = trusted
            End Sub

            Private Sub WriteString(ByVal s As String)
                bw.Write(s.Length)
                For Each c As Char In s
                    bw.Write(CUShort(AscW(c) And &HFFFF))
                Next
            End Sub

            Private Sub WriteTypeRef(ByVal t As Type)
                Dim id As Integer
                If typeIds.TryGetValue(t, id) Then
                    bw.Write(id)
                    Return
                End If
                bw.Write(-1)
                If t.IsArray Then
                    bw.Write(CByte(1))
                    Dim elem As Type = t.GetElementType()
                    bw.Write(t.GetArrayRank())
                    bw.Write(t Is elem.MakeArrayType())
                    WriteTypeRef(elem)
                ElseIf t.IsGenericType Then
                    bw.Write(CByte(2))
                    WriteString(t.GetGenericTypeDefinition().FullName)
                    Dim args As Type() = t.GetGenericArguments()
                    bw.Write(args.Length)
                    For Each a As Type In args
                        WriteTypeRef(a)
                    Next
                Else
                    bw.Write(CByte(0))
                    WriteString(t.FullName)
                End If
                typeIds(t) = typeIds.Count
            End Sub

            Private Function TryWriteRef(ByVal o As Object) As Boolean
                Dim id As Integer
                If objectIds.TryGetValue(o, id) Then
                    bw.Write(T_REF)
                    bw.Write(id)
                    Return True
                End If
                objectIds(o) = objectIds.Count
                Return False
            End Function

            Public Sub WriteValue(ByVal v As Object)
                If v Is Nothing Then
                    bw.Write(T_NULL)
                    Return
                End If
                Dim t As Type = v.GetType()

                If t.IsEnum Then
                    bw.Write(T_ENUM)
                    WriteTypeRef(t)
                    bw.Write(Convert.ToInt64(v, CultureInfo.InvariantCulture))
                    Return
                End If

                Select Case Type.GetTypeCode(t)
                    Case TypeCode.Boolean : bw.Write(T_BOOL) : bw.Write(CBool(v)) : Return
                    Case TypeCode.Byte : bw.Write(T_BYTE) : bw.Write(CByte(v)) : Return
                    Case TypeCode.SByte : bw.Write(T_SBYTE) : bw.Write(CSByte(v)) : Return
                    Case TypeCode.Int16 : bw.Write(T_INT16) : bw.Write(CShort(v)) : Return
                    Case TypeCode.UInt16 : bw.Write(T_UINT16) : bw.Write(CUShort(v)) : Return
                    Case TypeCode.Int32 : bw.Write(T_INT32) : bw.Write(CInt(v)) : Return
                    Case TypeCode.UInt32 : bw.Write(T_UINT32) : bw.Write(CUInt(v)) : Return
                    Case TypeCode.Int64 : bw.Write(T_INT64) : bw.Write(CLng(v)) : Return
                    Case TypeCode.UInt64 : bw.Write(T_UINT64) : bw.Write(CULng(v)) : Return
                    Case TypeCode.Single : bw.Write(T_SINGLE) : bw.Write(CSng(v)) : Return
                    Case TypeCode.Double : bw.Write(T_DOUBLE) : bw.Write(CDbl(v)) : Return
                    Case TypeCode.Decimal : bw.Write(T_DECIMAL) : bw.Write(CDec(v)) : Return
                    Case TypeCode.Char : bw.Write(T_CHAR) : bw.Write(CUShort(AscW(CChar(v)) And &HFFFF)) : Return
                    Case TypeCode.String : bw.Write(T_STRING) : WriteString(CStr(v)) : Return
                    Case TypeCode.DateTime : bw.Write(T_DATETIME) : bw.Write(CDate(v).ToBinary()) : Return
                End Select

                If t Is GetType(TimeSpan) Then
                    bw.Write(T_TIMESPAN) : bw.Write(DirectCast(v, TimeSpan).Ticks) : Return
                End If
                If t Is GetType(Guid) Then
                    bw.Write(T_GUID) : bw.Write(DirectCast(v, Guid).ToByteArray()) : Return
                End If
                If t Is GetType(DateTimeOffset) Then
                    Dim dto As DateTimeOffset = DirectCast(v, DateTimeOffset)
                    bw.Write(T_DTOFFSET) : bw.Write(dto.Ticks) : bw.Write(dto.Offset.Ticks) : Return
                End If
                If t Is GetType(BigInteger) Then
                    Dim bytes As Byte() = DirectCast(v, BigInteger).ToByteArray()
                    bw.Write(T_BIGINT) : bw.Write(bytes.Length) : bw.Write(bytes) : Return
                End If
                If TypeOf v Is Type Then
                    bw.Write(T_TYPE) : WriteTypeRef(DirectCast(v, Type)) : Return
                End If
                If TypeOf v Is Uri Then
                    Dim u As Uri = DirectCast(v, Uri)
                    bw.Write(T_URI) : bw.Write(u.IsAbsoluteUri) : WriteString(u.OriginalString) : Return
                End If
                If TypeOf v Is CultureInfo Then
                    bw.Write(T_CULTURE) : WriteString(DirectCast(v, CultureInfo).Name) : Return
                End If
                If TypeOf v Is TimeZoneInfo Then
                    Dim tz As TimeZoneInfo = DirectCast(v, TimeZoneInfo)
                    bw.Write(T_TIMEZONE) : WriteString(tz.Id) : WriteString(tz.ToSerializedString()) : Return
                End If
                If TypeOf v Is Encoding Then
                    Dim enc As Encoding = DirectCast(v, Encoding)
                    bw.Write(T_ENCODING) : bw.Write(enc.CodePage) : bw.Write(enc.GetPreamble().Length > 0) : Return
                End If
                If TypeOf v Is Regex Then
                    Dim re As Regex = DirectCast(v, Regex)
                    bw.Write(T_REGEX) : WriteString(re.ToString()) : bw.Write(CInt(re.Options)) : bw.Write(re.MatchTimeout.Ticks) : Return
                End If
                If TypeOf v Is StringComparer Then
                    For Each kv As KeyValuePair(Of String, StringComparer) In KnownStringComparers
                        If kv.Value.Equals(v) Then
                            bw.Write(T_STRCOMPARER) : WriteString(kv.Key) : Return
                        End If
                    Next
                End If

                If TypeOf v Is Random AndAlso t Is GetType(Random) Then
                    If TryWriteRef(v) Then Return
                    bw.Write(T_RANDOM)
                    WriteString(RuntimeFlavor)
                    Using ms As New MemoryStream()
                        Using inner As New BinaryWriter(ms, Encoding.UTF8, True)
                            Dim nested As New GraphWriter(inner, True)
                            nested.WriteObjectBody(v, t)
                            inner.Flush()
                        End Using
                        Dim blob As Byte() = ms.ToArray()
                        bw.Write(blob.Length)
                        bw.Write(blob)
                    End Using
                    Return
                End If

                If t.IsArray Then
                    If TryWriteRef(v) Then Return
                    WriteArray(DirectCast(v, Array), t)
                    Return
                End If

                Dim kind As Integer = CollectionKind(t)
                If kind <> 0 Then
                    If TryWriteRef(v) Then Return
                    WriteCollection(v, t, kind)
                    Return
                End If

                If IsDelegateType(t) Then
                    Throw New SerializationException("Delegates cannot be serialized (" & t.FullName & ").")
                End If
                If Not trusted AndAlso Not IsMarkedSerializable(t) Then
                    Throw New SerializationException("Type '" & t.FullName & "' is not marked as serializable.")
                End If

                If t.IsValueType Then
                    bw.Write(T_STRUCT)
                    WriteObjectFields(v, t)
                    Return
                End If

                If TryWriteRef(v) Then Return
                WriteObjectBody(v, t)
            End Sub

            ''' <summary>Writes T_OBJECT, the type, the field layout and the field values (the id is already assigned).</summary>
            Friend Sub WriteObjectBody(ByVal v As Object, ByVal t As Type)
                If Not objectIds.ContainsKey(v) Then objectIds(v) = objectIds.Count
                bw.Write(T_OBJECT)
                InvokeCallbacks(v, GetType(OnSerializingAttribute))
                WriteObjectFields(v, t)
                InvokeCallbacks(v, GetType(OnSerializedAttribute))
            End Sub

            Private Sub WriteObjectFields(ByVal v As Object, ByVal t As Type)
                WriteTypeRef(t)
                Dim fields As FieldInfo() = SerializableFields(t)
                Dim layout As Integer
                If layoutIds.TryGetValue(t, layout) Then
                    bw.Write(layout)
                Else
                    bw.Write(-1)
                    bw.Write(fields.Length)
                    For Each f As FieldInfo In fields
                        WriteString(FieldKey(f))
                    Next
                    layoutIds(t) = layoutIds.Count
                End If
                For Each f As FieldInfo In fields
                    WriteValue(f.GetValue(v))
                Next
            End Sub

            Private Sub WriteArray(ByVal arr As Array, ByVal t As Type)
                bw.Write(T_ARRAY)
                WriteTypeRef(t)
                Dim rank As Integer = arr.Rank
                For d As Integer = 0 To rank - 1
                    bw.Write(arr.GetLength(d))
                Next
                Dim elem As Type = t.GetElementType()
                If elem Is GetType(Byte) AndAlso rank = 1 Then
                    bw.Write(DirectCast(arr, Byte()))
                    Return
                End If
                If IsPrimitiveElement(elem) Then
                    Dim code As TypeCode = Type.GetTypeCode(elem)
                    For Each e As Object In arr
                        WritePrimitiveRaw(e, code)
                    Next
                    Return
                End If
                For Each e As Object In arr
                    WriteValue(e)
                Next
            End Sub

            Private Sub WritePrimitiveRaw(ByVal e As Object, ByVal code As TypeCode)
                Select Case code
                    Case TypeCode.Boolean : bw.Write(CBool(e))
                    Case TypeCode.Char : bw.Write(CUShort(AscW(CChar(e)) And &HFFFF))
                    Case TypeCode.SByte : bw.Write(CSByte(e))
                    Case TypeCode.Byte : bw.Write(CByte(e))
                    Case TypeCode.Int16 : bw.Write(CShort(e))
                    Case TypeCode.UInt16 : bw.Write(CUShort(e))
                    Case TypeCode.Int32 : bw.Write(CInt(e))
                    Case TypeCode.UInt32 : bw.Write(CUInt(e))
                    Case TypeCode.Int64 : bw.Write(CLng(e))
                    Case TypeCode.UInt64 : bw.Write(CULng(e))
                    Case TypeCode.Single : bw.Write(CSng(e))
                    Case TypeCode.Double : bw.Write(CDbl(e))
                    Case TypeCode.Decimal : bw.Write(CDec(e))
                End Select
            End Sub

            Private Sub WriteCollection(ByVal v As Object, ByVal t As Type, ByVal kind As Integer)
                bw.Write(If(kind = 1, T_LIST, T_DICT))
                WriteTypeRef(t)
                WriteValue(NonDefaultComparer(v))
                If kind = 1 Then
                    Dim items As New List(Of Object)()
                    For Each e As Object In DirectCast(v, IEnumerable)
                        items.Add(e)
                    Next
                    bw.Write(items.Count)
                    For Each e As Object In items
                        WriteValue(e)
                    Next
                Else
                    Dim keys As New List(Of Object)()
                    Dim values As New List(Of Object)()
                    If TypeOf v Is IDictionary Then
                        For Each de As DictionaryEntry In DirectCast(v, IDictionary)
                            keys.Add(de.Key)
                            values.Add(de.Value)
                        Next
                    Else
                        ' ConcurrentDictionary implements IDictionary too; this branch is a safety net.
                        For Each kv As Object In DirectCast(v, IEnumerable)
                            Dim kvt As Type = kv.GetType()
                            keys.Add(kvt.GetProperty("Key").GetValue(kv, Nothing))
                            values.Add(kvt.GetProperty("Value").GetValue(kv, Nothing))
                        Next
                    End If
                    bw.Write(keys.Count)
                    For i As Integer = 0 To keys.Count - 1
                        WriteValue(keys(i))
                        WriteValue(values(i))
                    Next
                End If
            End Sub
        End Class

        ' ------------------------------------------------------------------ reader

        Private NotInheritable Class GraphReader
            Private ReadOnly br As BinaryReader
            Private ReadOnly trusted As Boolean
            Private ReadOnly objects As New List(Of Object)()
            Private ReadOnly types As New List(Of Type)()
            Private ReadOnly layouts As New List(Of String())()
            Private ReadOnly completed As New List(Of Object)()

            Public Sub New(ByVal br As BinaryReader, ByVal trusted As Boolean)
                Me.br = br
                Me.trusted = trusted
            End Sub

            Private Function ReadString() As String
                Dim n As Integer = br.ReadInt32()
                If n < 0 Then Throw New SerializationException("Corrupt data: negative string length.")
                Dim chars(n - 1) As Char
                For i As Integer = 0 To n - 1
                    chars(i) = ChrW(br.ReadUInt16())
                Next
                Return New String(chars)
            End Function

            Private Function ReadTypeRef() As Type
                Dim id As Integer = br.ReadInt32()
                If id >= 0 Then
                    If id >= types.Count Then Throw New SerializationException("Corrupt data: unknown type reference " & id & ".")
                    Return types(id)
                End If
                Dim kind As Byte = br.ReadByte()
                Dim t As Type
                Select Case kind
                    Case 1
                        Dim rank As Integer = br.ReadInt32()
                        Dim isVector As Boolean = br.ReadBoolean()
                        Dim elem As Type = ReadTypeRef()
                        t = If(isVector, elem.MakeArrayType(), elem.MakeArrayType(rank))
                    Case 2
                        Dim defName As String = ReadString()
                        Dim n As Integer = br.ReadInt32()
                        Dim args(n - 1) As Type
                        For i As Integer = 0 To n - 1
                            args(i) = ReadTypeRef()
                        Next
                        t = ResolveTypeName(defName).MakeGenericType(args)
                    Case 0
                        t = ResolveTypeName(ReadString())
                    Case Else
                        Throw New SerializationException("Corrupt data: unknown type descriptor " & kind & ".")
                End Select
                types.Add(t)
                Return t
            End Function

            Private Function Reserve() As Integer
                objects.Add(Nothing)
                Return objects.Count - 1
            End Function

            Public Function ReadValue() As Object
                Dim tag As Byte = br.ReadByte()
                Select Case tag
                    Case T_NULL : Return Nothing
                    Case T_REF
                        Dim id As Integer = br.ReadInt32()
                        If id < 0 OrElse id >= objects.Count Then Throw New SerializationException("Corrupt data: unknown object reference " & id & ".")
                        Return objects(id)
                    Case T_BOOL : Return br.ReadBoolean()
                    Case T_BYTE : Return br.ReadByte()
                    Case T_SBYTE : Return br.ReadSByte()
                    Case T_INT16 : Return br.ReadInt16()
                    Case T_UINT16 : Return br.ReadUInt16()
                    Case T_INT32 : Return br.ReadInt32()
                    Case T_UINT32 : Return br.ReadUInt32()
                    Case T_INT64 : Return br.ReadInt64()
                    Case T_UINT64 : Return br.ReadUInt64()
                    Case T_SINGLE : Return br.ReadSingle()
                    Case T_DOUBLE : Return br.ReadDouble()
                    Case T_DECIMAL : Return br.ReadDecimal()
                    Case T_CHAR : Return ChrW(br.ReadUInt16())
                    Case T_STRING : Return ReadString()
                    Case T_DATETIME : Return Date.FromBinary(br.ReadInt64())
                    Case T_TIMESPAN : Return New TimeSpan(br.ReadInt64())
                    Case T_GUID : Return New Guid(br.ReadBytes(16))
                    Case T_DTOFFSET
                        Dim ticks As Long = br.ReadInt64()
                        Return New DateTimeOffset(ticks, New TimeSpan(br.ReadInt64()))
                    Case T_BIGINT
                        Dim n As Integer = br.ReadInt32()
                        Return New BigInteger(br.ReadBytes(n))
                    Case T_ENUM
                        Dim et As Type = ReadTypeRef()
                        If Not et.IsEnum Then Throw New SerializationException("Corrupt data: " & et.FullName & " is not an enum.")
                        Return [Enum].ToObject(et, br.ReadInt64())
                    Case T_TYPE : Return ReadTypeRef()
                    Case T_URI
                        Dim absolute As Boolean = br.ReadBoolean()
                        Return New Uri(ReadString(), If(absolute, UriKind.Absolute, UriKind.Relative))
                    Case T_CULTURE : Return CultureInfo.GetCultureInfo(ReadString())
                    Case T_TIMEZONE
                        Dim id As String = ReadString()
                        Return RestoreTimeZone(id, ReadString())
                    Case T_ENCODING
                        Dim codePage As Integer = br.ReadInt32()
                        Return RestoreEncoding(codePage, br.ReadBoolean())
                    Case T_REGEX
                        Dim pattern As String = ReadString()
                        Dim options As RegexOptions = CType(br.ReadInt32(), RegexOptions)
                        Return New Regex(pattern, options, New TimeSpan(br.ReadInt64()))
                    Case T_STRCOMPARER
                        Dim name As String = ReadString()
                        For Each kv As KeyValuePair(Of String, StringComparer) In KnownStringComparers
                            If kv.Key = name Then Return kv.Value
                        Next
                        Throw New SerializationException("Unknown string comparer " & name & ".")
                    Case T_RANDOM : Return ReadRandom()
                    Case T_ARRAY : Return ReadArray()
                    Case T_LIST, T_DICT : Return ReadCollection(tag)
                    Case T_STRUCT
                        Dim st As Type = ReadTypeRef()
                        CheckAllowed(st)
                        Dim fieldValues As List(Of KeyValuePair(Of FieldInfo, Object)) = ReadFieldValues(st)
                        ' VB copies a boxed value type whenever it passes through an Object-typed variable, so the
                        ' box is kept in a ValueType variable while its fields are set.
                        Dim boxed As ValueType = DirectCast(CreateUninitialized(st), ValueType)
                        For Each fv As KeyValuePair(Of FieldInfo, Object) In fieldValues
                            fv.Key.SetValue(boxed, ConvertForField(fv.Key, fv.Value))
                        Next
                        Return boxed
                    Case T_OBJECT
                        Dim slot As Integer = Reserve()
                        Return ReadObjectBody(slot)
                    Case Else
                        Throw New SerializationException("Corrupt data: unknown value tag " & tag & ".")
                End Select
            End Function

            Private Sub CheckAllowed(ByVal t As Type)
                If IsDelegateType(t) Then Throw New SerializationException("Refusing to create the delegate type " & t.FullName & ".")
                If Not trusted AndAlso Not IsMarkedSerializable(t) Then
                    Throw New SerializationException("Refusing to create '" & t.FullName & "': it is not marked as serializable.")
                End If
                If t.IsAbstract OrElse t.IsInterface Then Throw New SerializationException("Corrupt data: cannot create the abstract type " & t.FullName & ".")
            End Sub

            ''' <summary>Reads the type, layout and fields of an object whose slot is already reserved.</summary>
            Private Function ReadObjectBody(ByVal slot As Integer) As Object
                Dim t As Type = ReadTypeRef()
                CheckAllowed(t)
                Dim obj As Object = CreateUninitialized(t)
                objects(slot) = obj
                InvokeCallbacks(obj, GetType(OnDeserializingAttribute))
                ReadFields(obj, t)
                completed.Add(obj)
                Return obj
            End Function

            ''' <summary>Sets the fields of a reference-type instance.</summary>
            Private Sub ReadFields(ByVal target As Object, ByVal t As Type)
                For Each fv As KeyValuePair(Of FieldInfo, Object) In ReadFieldValues(t)
                    fv.Key.SetValue(target, ConvertForField(fv.Key, fv.Value))
                Next
            End Sub

            ''' <summary>Reads the layout and the field values of one object or struct.</summary>
            Private Function ReadFieldValues(ByVal t As Type) As List(Of KeyValuePair(Of FieldInfo, Object))
                Dim layoutRef As Integer = br.ReadInt32()
                Dim keys As String()
                If layoutRef >= 0 Then
                    If layoutRef >= layouts.Count Then Throw New SerializationException("Corrupt data: unknown layout reference " & layoutRef & ".")
                    keys = layouts(layoutRef)
                Else
                    Dim n As Integer = br.ReadInt32()
                    keys = New String(n - 1) {}
                    For i As Integer = 0 To n - 1
                        keys(i) = ReadString()
                    Next
                    layouts.Add(keys)
                End If
                Dim byKey As Dictionary(Of String, FieldInfo) = FieldsByKey(t)
                Dim result As New List(Of KeyValuePair(Of FieldInfo, Object))(keys.Length)
                For Each key As String In keys
                    Dim value As Object = ReadValue()
                    Dim f As FieldInfo = Nothing
                    If Not byKey.TryGetValue(key, f) Then Continue For ' field removed since the data was written
                    result.Add(New KeyValuePair(Of FieldInfo, Object)(f, value))
                Next
                Return result
            End Function

            ''' <summary>Converts a stored value to the field's type when the field type changed (enums, widened numbers).</summary>
            Private Shared Function ConvertForField(ByVal f As FieldInfo, ByVal value As Object) As Object
                If value IsNot Nothing AndAlso Not f.FieldType.IsInstanceOfType(value) Then
                    Dim ft As Type = If(Nullable.GetUnderlyingType(f.FieldType), f.FieldType)
                    If ft.IsEnum Then
                        Return [Enum].ToObject(ft, value)
                    ElseIf TypeOf value Is IConvertible AndAlso GetType(IConvertible).IsAssignableFrom(ft) Then
                        Return Convert.ChangeType(value, ft, CultureInfo.InvariantCulture)
                    End If
                End If
                Return value
            End Function

            Private Function ReadArray() As Object
                Dim slot As Integer = Reserve()
                Dim t As Type = ReadTypeRef()
                If Not t.IsArray Then Throw New SerializationException("Corrupt data: " & t.FullName & " is not an array type.")
                Dim rank As Integer = t.GetArrayRank()
                Dim lengths(rank - 1) As Integer
                For d As Integer = 0 To rank - 1
                    lengths(d) = br.ReadInt32()
                Next
                Dim elem As Type = t.GetElementType()
                Dim arr As Array
                If t Is elem.MakeArrayType() Then
                    arr = Array.CreateInstance(elem, lengths(0))
                Else
                    arr = Array.CreateInstance(elem, lengths)
                End If
                objects(slot) = arr
                If elem Is GetType(Byte) AndAlso t Is elem.MakeArrayType() Then
                    Dim bytes As Byte() = br.ReadBytes(lengths(0))
                    Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length)
                    Return arr
                End If
                Dim total As Integer = arr.Length
                Dim indices(rank - 1) As Integer
                Dim primitive As Boolean = IsPrimitiveElement(elem)
                Dim code As TypeCode = Type.GetTypeCode(elem)
                For i As Integer = 0 To total - 1
                    Dim value As Object = If(primitive, ReadPrimitiveRaw(code), ReadValue())
                    If rank = 1 Then
                        arr.SetValue(value, i)
                    Else
                        arr.SetValue(value, indices)
                        For d As Integer = rank - 1 To 0 Step -1
                            indices(d) += 1
                            If indices(d) < lengths(d) Then Exit For
                            indices(d) = 0
                        Next
                    End If
                Next
                Return arr
            End Function

            Private Function ReadPrimitiveRaw(ByVal code As TypeCode) As Object
                Select Case code
                    Case TypeCode.Boolean : Return br.ReadBoolean()
                    Case TypeCode.Char : Return ChrW(br.ReadUInt16())
                    Case TypeCode.SByte : Return br.ReadSByte()
                    Case TypeCode.Byte : Return br.ReadByte()
                    Case TypeCode.Int16 : Return br.ReadInt16()
                    Case TypeCode.UInt16 : Return br.ReadUInt16()
                    Case TypeCode.Int32 : Return br.ReadInt32()
                    Case TypeCode.UInt32 : Return br.ReadUInt32()
                    Case TypeCode.Int64 : Return br.ReadInt64()
                    Case TypeCode.UInt64 : Return br.ReadUInt64()
                    Case TypeCode.Single : Return br.ReadSingle()
                    Case TypeCode.Double : Return br.ReadDouble()
                    Case TypeCode.Decimal : Return br.ReadDecimal()
                End Select
                Throw New SerializationException("Corrupt data: unexpected primitive type " & code.ToString() & ".")
            End Function

            Private Function ReadCollection(ByVal tag As Byte) As Object
                Dim slot As Integer = Reserve()
                Dim t As Type = ReadTypeRef()
                Dim kind As Integer = CollectionKind(t)
                If kind = 0 OrElse (kind = 1) <> (tag = T_LIST) Then
                    Throw New SerializationException("Refusing to create '" & t.FullName & "' as a collection.")
                End If
                Dim comparer As Object = ReadValue()
                Dim coll As Object = If(comparer Is Nothing, Activator.CreateInstance(t), Activator.CreateInstance(t, comparer))
                objects(slot) = coll
                Dim count As Integer = br.ReadInt32()
                If kind = 1 Then
                    Dim items As New List(Of Object)(Math.Max(0, Math.Min(count, 1 << 16)))
                    For i As Integer = 0 To count - 1
                        items.Add(ReadValue())
                    Next
                    Dim def As Type = If(t.IsGenericType, t.GetGenericTypeDefinition(), Nothing)
                    Dim adder As MethodInfo
                    If def Is GetType(LinkedList(Of )) Then
                        adder = t.GetMethods().First(Function(m) m.Name = "AddLast" AndAlso m.GetParameters().Length = 1 AndAlso m.GetParameters()(0).ParameterType Is t.GetGenericArguments()(0))
                    ElseIf def Is GetType(Queue(Of )) Then
                        adder = t.GetMethod("Enqueue")
                    ElseIf def Is GetType(Stack(Of )) Then
                        adder = t.GetMethod("Push")
                        items.Reverse() ' a stack enumerates from the top
                    Else
                        adder = t.GetMethod("Add", If(t.IsGenericType, t.GetGenericArguments(), New Type() {GetType(Object)}))
                    End If
                    For Each e As Object In items
                        adder.Invoke(coll, New Object() {e})
                    Next
                Else
                    Dim adder As MethodInfo
                    If t.IsGenericType AndAlso t.GetGenericTypeDefinition() Is GetType(ConcurrentDictionary(Of ,)) Then
                        adder = t.GetMethod("TryAdd", t.GetGenericArguments())
                    Else
                        adder = t.GetMethod("Add", If(t.IsGenericType, t.GetGenericArguments(), New Type() {GetType(Object), GetType(Object)}))
                    End If
                    For i As Integer = 0 To count - 1
                        Dim k As Object = ReadValue()
                        Dim v As Object = ReadValue()
                        adder.Invoke(coll, New Object() {k, v})
                    Next
                End If
                Return coll
            End Function

            Private Function ReadRandom() As Object
                Dim slot As Integer = Reserve()
                Dim flavor As String = ReadString()
                Dim n As Integer = br.ReadInt32()
                Dim blob As Byte() = br.ReadBytes(n)
                Dim result As Random
                If flavor = RuntimeFlavor Then
                    Using ms As New MemoryStream(blob)
                        Using inner As New BinaryReader(ms, Encoding.UTF8, True)
                            Dim nested As New GraphReader(inner, True)
                            Dim tag As Byte = inner.ReadByte()
                            If tag <> T_OBJECT Then Throw New SerializationException("Corrupt data: bad Random state.")
                            Dim innerSlot As Integer = nested.Reserve()
                            result = DirectCast(nested.ReadObjectBody(innerSlot), Random)
                            nested.RunDeserializedCallbacks()
                        End Using
                    End Using
                Else
                    result = New Random() ' the internal state of System.Random differs between runtimes
                End If
                objects(slot) = result
                Return result
            End Function

            Private Shared Function RestoreTimeZone(ByVal id As String, ByVal serialized As String) As TimeZoneInfo
                If id = TimeZoneInfo.Utc.Id Then Return TimeZoneInfo.Utc
                Dim custom As TimeZoneInfo = TimeZoneInfo.FromSerializedString(serialized)
                Try
                    Dim sys As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(id)
                    If sys.HasSameRules(custom) Then Return sys
                Catch ex As TimeZoneNotFoundException
                Catch ex As InvalidTimeZoneException
                End Try
                Return custom
            End Function

            Private Shared Function RestoreEncoding(ByVal codePage As Integer, ByVal preamble As Boolean) As Encoding
                Select Case codePage
                    Case 65001 : Return New UTF8Encoding(preamble)
                    Case 1200 : Return New UnicodeEncoding(False, preamble)
                    Case 1201 : Return New UnicodeEncoding(True, preamble)
                    Case 12000 : Return New UTF32Encoding(False, preamble)
                    Case 12001 : Return New UTF32Encoding(True, preamble)
                    Case Else : Return Encoding.GetEncoding(codePage)
                End Select
            End Function

            ''' <summary>Runs the <c>OnDeserialized</c> methods and <see cref="IDeserializationCallback"/>s once the graph is complete.</summary>
            Public Sub RunDeserializedCallbacks()
                For Each o As Object In completed
                    InvokeCallbacks(o, GetType(OnDeserializedAttribute))
                Next
                For Each o As Object In completed
                    Dim cb As IDeserializationCallback = TryCast(o, IDeserializationCallback)
                    If cb IsNot Nothing Then cb.OnDeserialization(Nothing)
                Next
            End Sub
        End Class

    End Class

End Namespace
