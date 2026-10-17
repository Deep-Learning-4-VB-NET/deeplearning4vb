Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions

Namespace org.datavec.api.transform.transforms

    ''' <summary>
    ''' Java semantics needed by the transform implementations: <c>String.valueOf</c> and the collection
    ''' <c>toString</c> formats used by the Lombok <c>toString()</c> methods, content-based <c>equals</c>/<c>hashCode</c>
    ''' of lists, sets, maps and arrays (Lombok <c>@EqualsAndHashCode</c>), <c>String.split</c> and
    ''' <c>String.replaceAll</c>. Candidate for hoisting into JavaCompat.
    ''' </summary>
    Friend Module JavaTransformHelpers

        ''' <summary>Java <c>String.valueOf(o)</c>, rendering collections as <c>[a, b]</c> and maps as <c>{k=v}</c>.</summary>
        Public Function javaToString(ByVal o As Object) As String
            If o Is Nothing Then Return "null"
            If TypeOf o Is String Then Return DirectCast(o, String)
            If TypeOf o Is Array Then Return Arrays.toString(o)
            If isMap(o) Then
                Dim sb As New StringBuilder("{")
                Dim first As Boolean = True
                For Each kv As KeyValuePair(Of Object, Object) In mapEntries(o)
                    If Not first Then sb.Append(", ")
                    first = False
                    sb.Append(javaToString(kv.Key)).Append("="c).Append(javaToString(kv.Value))
                Next kv
                Return sb.Append("}"c).ToString()
            End If
            If TypeOf o Is IEnumerable Then
                Dim sb As New StringBuilder("[")
                Dim first As Boolean = True
                For Each e As Object In DirectCast(o, IEnumerable)
                    If Not first Then sb.Append(", ")
                    first = False
                    sb.Append(javaToString(e))
                Next e
                Return sb.Append("]"c).ToString()
            End If
            Return Arrays.FormatElement(o)
        End Function

        ''' <summary>
        ''' Java <c>equals</c> with collection semantics: lists and arrays element by element, sets and maps by
        ''' content, everything else with <c>Equals</c>. Null-safe.
        ''' </summary>
        Public Function javaEquals(ByVal a As Object, ByVal b As Object) As Boolean
            If a Is b Then Return True
            If a Is Nothing OrElse b Is Nothing Then Return False
            If TypeOf a Is String OrElse TypeOf b Is String Then Return a.Equals(b)
            Dim aMap As Boolean = isMap(a)
            Dim bMap As Boolean = isMap(b)
            If aMap OrElse bMap Then
                If Not (aMap AndAlso bMap) Then Return False
                Dim ea As List(Of KeyValuePair(Of Object, Object)) = mapEntries(a)
                Dim eb As List(Of KeyValuePair(Of Object, Object)) = mapEntries(b)
                If ea.Count <> eb.Count Then Return False
                For Each x As KeyValuePair(Of Object, Object) In ea
                    Dim found As Boolean = False
                    For Each y As KeyValuePair(Of Object, Object) In eb
                        If javaEquals(x.Key, y.Key) Then
                            If Not javaEquals(x.Value, y.Value) Then Return False
                            found = True
                            Exit For
                        End If
                    Next y
                    If Not found Then Return False
                Next x
                Return True
            End If
            Dim aSet As Boolean = isSet(a)
            Dim bSet As Boolean = isSet(b)
            If aSet OrElse bSet Then
                If Not (aSet AndAlso bSet) Then Return False
                Dim la As List(Of Object) = toList(DirectCast(a, IEnumerable))
                Dim lb As List(Of Object) = toList(DirectCast(b, IEnumerable))
                If la.Count <> lb.Count Then Return False
                For Each x As Object In la
                    Dim found As Boolean = False
                    For Each y As Object In lb
                        If javaEquals(x, y) Then
                            found = True
                            Exit For
                        End If
                    Next y
                    If Not found Then Return False
                Next x
                Return True
            End If
            If TypeOf a Is IEnumerable AndAlso TypeOf b Is IEnumerable Then
                Dim ia As IEnumerator = DirectCast(a, IEnumerable).GetEnumerator()
                Dim ib As IEnumerator = DirectCast(b, IEnumerable).GetEnumerator()
                Do
                    Dim ha As Boolean = ia.MoveNext()
                    Dim hb As Boolean = ib.MoveNext()
                    If ha <> hb Then Return False
                    If Not ha Then Return True
                    If Not javaEquals(ia.Current, ib.Current) Then Return False
                Loop
            End If
            Return a.Equals(b)
        End Function

        ''' <summary>
        ''' Java <c>hashCode</c> consistent with <see cref="javaEquals"/>: <c>List.hashCode</c>/<c>Arrays.hashCode</c>
        ''' for lists and arrays, the sum of the element hashes for sets, the sum of <c>key ^ value</c> for maps.
        ''' </summary>
        Public Function javaHashCode(ByVal o As Object) As Integer
            If o Is Nothing Then Return 0
            If TypeOf o Is String Then Return o.GetHashCode()
            If isMap(o) Then
                Dim h As Integer = 0
                For Each kv As KeyValuePair(Of Object, Object) In mapEntries(o)
                    h += javaHashCode(kv.Key) Xor javaHashCode(kv.Value)
                Next kv
                Return h
            End If
            If isSet(o) Then
                Dim h As Integer = 0
                For Each e As Object In DirectCast(o, IEnumerable)
                    h += javaHashCode(e)
                Next e
                Return h
            End If
            If TypeOf o Is IEnumerable Then
                Dim h As Integer = 1
                For Each e As Object In DirectCast(o, IEnumerable)
                    h = 31 * h + javaHashCode(e)
                Next e
                Return h
            End If
            Return o.GetHashCode()
        End Function

        ''' <summary>The hash Lombok <c>@EqualsAndHashCode</c> generates: <c>result = result * 59 + hash(field)</c> (43 for null).</summary>
        Public Function lombokHash(ByVal ParamArray values() As Object) As Integer
            Dim result As Integer = 1
            For Each v As Object In values
                result = result * 59 + If(v Is Nothing, 43, javaHashCode(v))
            Next v
            Return result
        End Function

        ''' <summary>
        ''' Java <c>String.split(regex)</c>: trailing empty strings are removed, a zero-width match at the start never
        ''' produces a leading empty string, capture groups are not included in the result.
        ''' </summary>
        Public Function javaSplit(ByVal s As String, ByVal regex As String) As String()
            Dim parts As New List(Of String)()
            Dim index As Integer = 0
            For Each m As Match In System.Text.RegularExpressions.Regex.Matches(s, regex)
                If m.Index = 0 AndAlso m.Length = 0 Then Continue For
                If m.Length = 0 AndAlso m.Index >= s.Length Then Continue For
                If index = 0 AndAlso m.Index = 0 AndAlso m.Length = 0 Then Continue For
                parts.Add(s.Substring(index, m.Index - index))
                index = m.Index + m.Length
            Next m
            If parts.Count = 0 Then Return New String() {s}
            parts.Add(s.Substring(index))
            Dim n As Integer = parts.Count
            While n > 0 AndAlso parts(n - 1).Length = 0
                n -= 1
            End While
            Return parts.GetRange(0, n).ToArray()
        End Function

        ''' <summary>
        ''' Java <c>String.replaceAll(regex, replacement)</c>: the Java replacement syntax (<c>$1</c>, <c>${name}</c>,
        ''' <c>\$</c> for a literal dollar, <c>\\</c> for a backslash) is translated to the .NET one.
        ''' </summary>
        Public Function javaReplaceAll(ByVal s As String, ByVal regex As String, ByVal replacement As String) As String
            Dim sb As New StringBuilder()
            Dim i As Integer = 0
            While i < replacement.Length
                Dim c As Char = replacement(i)
                If c = "\"c Then
                    i += 1
                    If i >= replacement.Length Then Throw New ArgumentException("character to be escaped is missing")
                    If replacement(i) = "$"c Then
                        sb.Append("$$")
                    Else
                        sb.Append(replacement(i))
                    End If
                ElseIf c = "$"c Then
                    If i + 1 >= replacement.Length OrElse Not (Char.IsDigit(replacement(i + 1)) OrElse replacement(i + 1) = "{"c) Then
                        Throw New ArgumentException("Illegal group reference")
                    End If
                    sb.Append(c)
                Else
                    sb.Append(c)
                End If
                i += 1
            End While
            Return System.Text.RegularExpressions.Regex.Replace(s, regex, sb.ToString())
        End Function

        ''' <summary>The error message the Java transforms throw when a record does not match the input schema.</summary>
        Public Function badRecordLengthMessage(ByVal actual As Integer, ByVal expected As Integer, ByVal transform As Object) As String
            Return "Cannot execute transform: input writables list length (" & actual & ") does not " &
                   "match expected number of elements (schema: " & expected & "). Transform = " & javaToString(transform)
        End Function

        Private Function isMap(ByVal o As Object) As Boolean
            If TypeOf o Is IDictionary Then Return True
            Return implementsGeneric(o.GetType(), GetType(IDictionary(Of ,)))
        End Function

        Private Function isSet(ByVal o As Object) As Boolean
            Return implementsGeneric(o.GetType(), GetType(ISet(Of )))
        End Function

        Private Function implementsGeneric(ByVal t As Type, ByVal genericInterface As Type) As Boolean
            For Each i As Type In t.GetInterfaces()
                If i.IsGenericType AndAlso i.GetGenericTypeDefinition() Is genericInterface Then Return True
            Next i
            Return False
        End Function

        Private Function toList(ByVal e As IEnumerable) As List(Of Object)
            Dim l As New List(Of Object)()
            For Each x As Object In e
                l.Add(x)
            Next x
            Return l
        End Function

        ''' <summary>The entries of a generic or non-generic dictionary, in its enumeration order.</summary>
        Private Function mapEntries(ByVal o As Object) As List(Of KeyValuePair(Of Object, Object))
            Dim ret As New List(Of KeyValuePair(Of Object, Object))()
            For Each e As Object In DirectCast(o, IEnumerable)
                If TypeOf e Is DictionaryEntry Then
                    Dim de As DictionaryEntry = DirectCast(e, DictionaryEntry)
                    ret.Add(New KeyValuePair(Of Object, Object)(de.Key, de.Value))
                Else
                    Dim t As Type = e.GetType()
                    Dim k As Object = t.GetProperty("Key", BindingFlags.Public Or BindingFlags.Instance).GetValue(e, Nothing)
                    Dim v As Object = t.GetProperty("Value", BindingFlags.Public Or BindingFlags.Instance).GetValue(e, Nothing)
                    ret.Add(New KeyValuePair(Of Object, Object)(k, v))
                End If
            Next e
            Return ret
        End Function

    End Module

End Namespace
