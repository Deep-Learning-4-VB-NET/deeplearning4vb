Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' JUnit-style assertion helpers, so tests ported from the Java sources keep their original
''' shape. Equality follows Java semantics for collections and arrays: dictionaries, lists,
''' sets and arrays are compared by content (recursively), everything else with Equals.
''' </summary>
Friend Module Assertions

    Public Sub assertTrue(ByVal condition As Boolean)
        Assert.IsTrue(condition)
    End Sub

    Public Sub assertTrue(ByVal condition As Boolean, ByVal message As String)
        Assert.IsTrue(condition, message)
    End Sub

    Public Sub assertFalse(ByVal condition As Boolean)
        Assert.IsFalse(condition)
    End Sub

    Public Sub assertFalse(ByVal condition As Boolean, ByVal message As String)
        Assert.IsFalse(condition, message)
    End Sub

    Public Sub assertNull(ByVal value As Object)
        Assert.IsNull(value)
    End Sub

    Public Sub assertNotNull(ByVal value As Object)
        Assert.IsNotNull(value)
    End Sub

    Public Sub fail(ByVal message As String)
        Assert.Fail(message)
    End Sub

    Public Sub assertEquals(ByVal expected As Double, ByVal actual As Double, ByVal delta As Double)
        Assert.AreEqual(expected, actual, delta)
    End Sub

    Public Sub assertEquals(ByVal expected As Double, ByVal actual As Double, ByVal delta As Double, ByVal message As String)
        Assert.AreEqual(expected, actual, delta, message)
    End Sub

    Public Sub assertEquals(ByVal expected As Single, ByVal actual As Single, ByVal delta As Single)
        Assert.AreEqual(expected, actual, delta)
    End Sub

    Public Sub assertEquals(ByVal expected As Object, ByVal actual As Object)
        If Not DeepEquals(expected, actual) Then
            Assert.Fail("Expected: <" & Describe(expected) & "> but was: <" & Describe(actual) & ">")
        End If
    End Sub

    Public Sub assertEquals(ByVal expected As Object, ByVal actual As Object, ByVal message As String)
        If Not DeepEquals(expected, actual) Then
            Assert.Fail(message & " - Expected: <" & Describe(expected) & "> but was: <" & Describe(actual) & ">")
        End If
    End Sub

    Public Sub assertNotEquals(ByVal unexpected As Object, ByVal actual As Object)
        If DeepEquals(unexpected, actual) Then
            Assert.Fail("Expected values to differ but both were: <" & Describe(actual) & ">")
        End If
    End Sub

    Public Sub assertArrayEquals(ByVal expected As Array, ByVal actual As Array)
        assertEquals(expected, actual)
    End Sub

    Public Sub assertArrayEquals(ByVal expected() As Double, ByVal actual() As Double, ByVal delta As Double)
        Assert.IsNotNull(actual, "actual array was Nothing")
        Assert.AreEqual(expected.Length, actual.Length, "array lengths differ")
        For i As Integer = 0 To expected.Length - 1
            Assert.AreEqual(expected(i), actual(i), delta, "element " & i)
        Next i
    End Sub

    Public Sub assertArrayEquals(ByVal expected() As Single, ByVal actual() As Single, ByVal delta As Single)
        Assert.IsNotNull(actual, "actual array was Nothing")
        Assert.AreEqual(expected.Length, actual.Length, "array lengths differ")
        For i As Integer = 0 To expected.Length - 1
            Assert.AreEqual(expected(i), actual(i), delta, "element " & i)
        Next i
    End Sub

    ''' <summary>Runs the action and asserts that it throws <typeparamref name="T"/> (or a subclass). Returns the exception.</summary>
    Public Function assertThrows(Of T As Exception)(ByVal action As Action) As T
        Try
            action()
        Catch ex As T
            Return ex
        Catch ex As Exception
            Assert.Fail("Expected exception of type " & GetType(T).Name & " but got " & ex.GetType().Name & ": " & ex.Message)
        End Try
        Assert.Fail("Expected exception of type " & GetType(T).Name & " but nothing was thrown")
        Return Nothing
    End Function

    ''' <summary>Java-style equality: collections and arrays by content, scalars by Equals.</summary>
    Public Function DeepEquals(ByVal a As Object, ByVal b As Object) As Boolean
        If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
        If TypeOf a Is String OrElse TypeOf b Is String Then Return Object.Equals(a, b)
        If TypeOf a Is IDictionary AndAlso TypeOf b Is IDictionary Then
            Return DictionaryEquals(DirectCast(a, IDictionary), DirectCast(b, IDictionary))
        End If
        If IsGenericDictionary(a) AndAlso IsGenericDictionary(b) Then
            Return GenericDictionaryEquals(a, b)
        End If
        If TypeOf a Is IEnumerable AndAlso TypeOf b Is IEnumerable Then
            If IsSetLike(a) AndAlso IsSetLike(b) Then
                Return SetEquals(DirectCast(a, IEnumerable), DirectCast(b, IEnumerable))
            End If
            Return SequenceEquals(DirectCast(a, IEnumerable), DirectCast(b, IEnumerable))
        End If
        If IsNumeric(a) AndAlso IsNumeric(b) AndAlso a.GetType() IsNot b.GetType() Then
            Return Convert.ToDecimal(a) = Convert.ToDecimal(b)
        End If
        Return Object.Equals(a, b)
    End Function

    Private Function IsNumeric(ByVal o As Object) As Boolean
        Return TypeOf o Is Byte OrElse TypeOf o Is SByte OrElse TypeOf o Is Short OrElse TypeOf o Is UShort OrElse TypeOf o Is Integer OrElse TypeOf o Is UInteger OrElse TypeOf o Is Long OrElse TypeOf o Is ULong OrElse TypeOf o Is Single OrElse TypeOf o Is Double OrElse TypeOf o Is Decimal
    End Function

    Private Function IsSetLike(ByVal o As Object) As Boolean
        For Each i As Type In o.GetType().GetInterfaces()
            If i.IsGenericType AndAlso i.GetGenericTypeDefinition() Is GetType(ISet(Of )) Then Return True
        Next
        Return False
    End Function

    Private Function IsGenericDictionary(ByVal o As Object) As Boolean
        For Each i As Type In o.GetType().GetInterfaces()
            If i.IsGenericType AndAlso i.GetGenericTypeDefinition() Is GetType(IDictionary(Of ,)) Then Return True
        Next
        Return False
    End Function

    Private Function SequenceEquals(ByVal a As IEnumerable, ByVal b As IEnumerable) As Boolean
        Dim ea As IEnumerator = a.GetEnumerator()
        Dim eb As IEnumerator = b.GetEnumerator()
        Do
            Dim ha As Boolean = ea.MoveNext()
            Dim hb As Boolean = eb.MoveNext()
            If ha <> hb Then Return False
            If Not ha Then Return True
            If Not DeepEquals(ea.Current, eb.Current) Then Return False
        Loop
    End Function

    Private Function SetEquals(ByVal a As IEnumerable, ByVal b As IEnumerable) As Boolean
        Dim la As New List(Of Object)()
        For Each x In a
            la.Add(x)
        Next
        Dim lb As New List(Of Object)()
        For Each x In b
            lb.Add(x)
        Next
        If la.Count <> lb.Count Then Return False
        For Each x In la
            Dim found As Boolean = False
            For Each y In lb
                If DeepEquals(x, y) Then
                    found = True
                    Exit For
                End If
            Next
            If Not found Then Return False
        Next
        Return True
    End Function

    Private Function DictionaryEquals(ByVal a As IDictionary, ByVal b As IDictionary) As Boolean
        If a.Count <> b.Count Then Return False
        For Each key In a.Keys
            If Not b.Contains(key) Then Return False
            If Not DeepEquals(a(key), b(key)) Then Return False
        Next
        Return True
    End Function

    Private Function GenericDictionaryEquals(ByVal a As Object, ByVal b As Object) As Boolean
        ' Enumerate KeyValuePair(Of K, V) entries through reflection-free dynamic access.
        Dim entriesA As New List(Of KeyValuePair(Of Object, Object))()
        For Each kv In DirectCast(a, IEnumerable)
            entriesA.Add(New KeyValuePair(Of Object, Object)(kv.Key, kv.Value))
        Next
        Dim entriesB As New List(Of KeyValuePair(Of Object, Object))()
        For Each kv In DirectCast(b, IEnumerable)
            entriesB.Add(New KeyValuePair(Of Object, Object)(kv.Key, kv.Value))
        Next
        If entriesA.Count <> entriesB.Count Then Return False
        For Each ea In entriesA
            Dim matched As Boolean = False
            For Each eb In entriesB
                If DeepEquals(ea.Key, eb.Key) Then
                    If Not DeepEquals(ea.Value, eb.Value) Then Return False
                    matched = True
                    Exit For
                End If
            Next
            If Not matched Then Return False
        Next
        Return True
    End Function

    Private Function Describe(ByVal o As Object) As String
        If o Is Nothing Then Return "null"
        If TypeOf o Is String Then Return DirectCast(o, String)
        If TypeOf o Is IEnumerable Then
            Dim parts As New List(Of String)()
            For Each x In DirectCast(o, IEnumerable)
                parts.Add(Describe(x))
            Next
            Return "[" & String.Join(", ", parts) & "]"
        End If
        Return o.ToString()
    End Function

End Module
