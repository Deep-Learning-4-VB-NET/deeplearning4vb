Imports System
Imports System.Collections.Generic
Imports System.Linq

''' <summary>.NET equivalents of the <c>java.util.Collections</c> helpers used by the ported code.</summary>
Friend Module JavaCollections

    Private ReadOnly sharedRandom As New Random()

    ''' <summary>Shuffles the list in place (Fisher-Yates), like <c>JavaCollections.shuffle(list)</c>.</summary>
    Public Sub shuffle(Of T)(ByVal list As IList(Of T))
        SyncLock sharedRandom
            shuffle(list, sharedRandom)
        End SyncLock
    End Sub

    ''' <summary>Shuffles the list in place using the supplied random source.</summary>
    Public Sub shuffle(Of T)(ByVal list As IList(Of T), ByVal random As Random)
        If list Is Nothing Then Throw New ArgumentNullException(NameOf(list))
        If random Is Nothing Then Throw New ArgumentNullException(NameOf(random))
        For i As Integer = list.Count - 1 To 1 Step -1
            Dim j As Integer = random.Next(i + 1)
            Dim tmp As T = list(i)
            list(i) = list(j)
            list(j) = tmp
        Next i
    End Sub

    Public Function emptyList(Of T)() As IList(Of T)
        Return New List(Of T)()
    End Function

    Public Function singletonList(Of T)(ByVal item As T) As IList(Of T)
        Return New List(Of T)() From {item}
    End Function

End Module

''' <summary>.NET equivalents of the <c>java.util.Objects</c> helpers used by the ported code.</summary>
Friend Module Objects

    Public Function equals(ByVal a As Object, ByVal b As Object) As Boolean
        Return Object.Equals(a, b)
    End Function

    ''' <summary>Java-compatible combined hash of the values (<c>Arrays.hashCode(values)</c>).</summary>
    Public Function hash(ByVal ParamArray values() As Object) As Integer
        Return Arrays.hashCode(values)
    End Function

    Public Function hashCode(ByVal o As Object) As Integer
        Return If(o Is Nothing, 0, o.GetHashCode())
    End Function

    Public Function requireNonNull(Of T)(ByVal o As T, ByVal message As String) As T
        If o Is Nothing Then Throw New NullReferenceException(message)
        Return o
    End Function

    Public Function toString(ByVal o As Object) As String
        Return Arrays.FormatElement(o)
    End Function

End Module

''' <summary>Equivalents of the Guava <c>Ints</c> helpers used by the ported code.</summary>
Friend Module Ints

    Public Function contains(ByVal array() As Integer, ByVal target As Integer) As Boolean
        For Each value As Integer In array
            If value = target Then Return True
        Next value
        Return False
    End Function

    Public Function indexOf(ByVal array() As Integer, ByVal target As Integer) As Integer
        For i As Integer = 0 To array.Length - 1
            If array(i) = target Then Return i
        Next i
        Return -1
    End Function

    Public Function max(ByVal ParamArray array() As Integer) As Integer
        If array Is Nothing OrElse array.Length = 0 Then Throw New ArgumentException("Array must not be empty")
        Dim m As Integer = array(0)
        For i As Integer = 1 To array.Length - 1
            If array(i) > m Then m = array(i)
        Next i
        Return m
    End Function

    Public Function min(ByVal ParamArray array() As Integer) As Integer
        If array Is Nothing OrElse array.Length = 0 Then Throw New ArgumentException("Array must not be empty")
        Dim m As Integer = array(0)
        For i As Integer = 1 To array.Length - 1
            If array(i) < m Then m = array(i)
        Next i
        Return m
    End Function

    Public Function concat(ByVal ParamArray arrays()() As Integer) As Integer()
        Dim length As Integer = 0
        For Each a In arrays
            length += a.Length
        Next a
        Dim result(length - 1) As Integer
        Dim pos As Integer = 0
        For Each a In arrays
            Array.Copy(a, 0, result, pos, a.Length)
            pos += a.Length
        Next a
        Return result
    End Function

    Public Function compare(ByVal a As Integer, ByVal b As Integer) As Integer
        Return a.CompareTo(b)
    End Function

    Public Function toArray(ByVal collection As IEnumerable(Of Integer)) As Integer()
        Return collection.ToArray()
    End Function

    Public Function asList(ByVal ParamArray array() As Integer) As IList(Of Integer)
        Return New List(Of Integer)(array)
    End Function

    Public Function checkedCast(ByVal value As Long) As Integer
        If value < Integer.MinValue OrElse value > Integer.MaxValue Then
            Throw New ArgumentException("Out of range: " & value)
        End If
        Return CInt(value)
    End Function

End Module

''' <summary>Equivalents of the Guava <c>Longs</c> helpers used by the ported code.</summary>
Friend Module Longs

    Public Function contains(ByVal array() As Long, ByVal target As Long) As Boolean
        For Each value As Long In array
            If value = target Then Return True
        Next value
        Return False
    End Function

    Public Function indexOf(ByVal array() As Long, ByVal target As Long) As Integer
        For i As Integer = 0 To array.Length - 1
            If array(i) = target Then Return i
        Next i
        Return -1
    End Function

    Public Function max(ByVal ParamArray array() As Long) As Long
        If array Is Nothing OrElse array.Length = 0 Then Throw New ArgumentException("Array must not be empty")
        Dim m As Long = array(0)
        For i As Integer = 1 To array.Length - 1
            If array(i) > m Then m = array(i)
        Next i
        Return m
    End Function

    Public Function min(ByVal ParamArray array() As Long) As Long
        If array Is Nothing OrElse array.Length = 0 Then Throw New ArgumentException("Array must not be empty")
        Dim m As Long = array(0)
        For i As Integer = 1 To array.Length - 1
            If array(i) < m Then m = array(i)
        Next i
        Return m
    End Function

    Public Function concat(ByVal ParamArray arrays()() As Long) As Long()
        Dim length As Integer = 0
        For Each a In arrays
            length += a.Length
        Next a
        Dim result(length - 1) As Long
        Dim pos As Integer = 0
        For Each a In arrays
            Array.Copy(a, 0, result, pos, a.Length)
            pos += a.Length
        Next a
        Return result
    End Function

    Public Function compare(ByVal a As Long, ByVal b As Long) As Integer
        Return a.CompareTo(b)
    End Function

    Public Function toArray(ByVal collection As IEnumerable(Of Long)) As Long()
        Return collection.ToArray()
    End Function

    Public Function asList(ByVal ParamArray array() As Long) As IList(Of Long)
        Return New List(Of Long)(array)
    End Function

End Module
