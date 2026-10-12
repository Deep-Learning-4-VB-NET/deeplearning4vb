Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text

''' <summary>
''' .NET equivalents of the <c>java.util.Arrays</c> methods used by the ported code.
''' Semantics follow Java: <see cref="CopyOf"/> pads with default values when the new
''' length is larger, and <see cref="toString"/> renders <c>[1, 2, 3]</c> style output.
''' </summary>
Friend Module Arrays

    ''' <summary>Copies the array, truncating or padding with default values so the copy has the specified length.</summary>
    Public Function CopyOf(Of T)(ByVal original() As T, ByVal newLength As Integer) As T()
        If original Is Nothing Then Throw New ArgumentNullException(NameOf(original))
        If newLength < 0 Then Throw New ArgumentOutOfRangeException(NameOf(newLength))
        Dim dest(newLength - 1) As T
        Array.Copy(original, dest, Math.Min(original.Length, newLength))
        Return dest
    End Function

    ''' <summary>Copies the range [fromIndex, toIndex) of the array, padding with default values past the end.</summary>
    Public Function CopyOfRange(Of T)(ByVal original() As T, ByVal fromIndex As Integer, ByVal toIndex As Integer) As T()
        If original Is Nothing Then Throw New ArgumentNullException(NameOf(original))
        If fromIndex > toIndex Then Throw New ArgumentException(fromIndex & " > " & toIndex)
        If fromIndex < 0 OrElse fromIndex > original.Length Then Throw New ArgumentOutOfRangeException(NameOf(fromIndex))
        Dim length As Integer = toIndex - fromIndex
        Dim dest(length - 1) As T
        Array.Copy(original, fromIndex, dest, 0, Math.Min(original.Length - fromIndex, length))
        Return dest
    End Function

    Public Sub Fill(Of T)(ByVal array() As T, ByVal value As T)
        For i As Integer = 0 To array.Length - 1
            array(i) = value
        Next i
    End Sub

    Public Sub Fill(Of T)(ByVal array() As T, ByVal fromIndex As Integer, ByVal toIndex As Integer, ByVal value As T)
        If fromIndex > toIndex Then Throw New ArgumentException(fromIndex & " > " & toIndex)
        If fromIndex < 0 OrElse toIndex > array.Length Then Throw New ArgumentOutOfRangeException()
        For i As Integer = fromIndex To toIndex - 1
            array(i) = value
        Next i
    End Sub

    ''' <summary>Returns a fixed-content, resizable list wrapping a copy of the items (Java's <c>Arrays.asList</c>).</summary>
    Public Function asList(Of T)(ByVal ParamArray items() As T) As IList(Of T)
        If items Is Nothing Then Return New List(Of T)()
        Return New List(Of T)(items)
    End Function

    Public Sub sort(Of T)(ByVal a() As T)
        Array.Sort(a)
    End Sub

    Public Function equals(Of T)(ByVal a() As T, ByVal b() As T) As Boolean
        If a Is b Then Return True
        If a Is Nothing OrElse b Is Nothing Then Return False
        If a.Length <> b.Length Then Return False
        Dim cmp = EqualityComparer(Of T).Default
        For i As Integer = 0 To a.Length - 1
            If Not cmp.Equals(a(i), b(i)) Then Return False
        Next i
        Return True
    End Function

    ''' <summary>Java-compatible array hash code: <c>31 * h + elementHash</c> over the elements.</summary>
    Public Function hashCode(Of T)(ByVal a() As T) As Integer
        If a Is Nothing Then Return 0
        Dim result As Integer = 1
        For Each element As T In a
            Dim h As Integer = If(element Is Nothing, 0, element.GetHashCode())
            result = WrapToInt32(31L * result + h)
        Next element
        Return result
    End Function

    ''' <summary>Wraps a 64-bit value to 32 bits with two's-complement overflow, as Java int arithmetic does.</summary>
    Public Function WrapToInt32(ByVal value As Long) As Integer
        value = value And &HFFFFFFFFL
        If value > Integer.MaxValue Then value -= &H100000000L
        Return CInt(value)
    End Function

    ''' <summary>Renders the array Java-style, e.g. <c>[1, 2, 3]</c>; nested arrays are rendered recursively.</summary>
    Public Function toString(Of T)(ByVal a() As T) As String
        If a Is Nothing Then Return "null"
        Dim sb As New StringBuilder("[")
        For i As Integer = 0 To a.Length - 1
            If i > 0 Then sb.Append(", ")
            sb.Append(FormatElement(a(i)))
        Next i
        Return sb.Append("]").ToString()
    End Function

    ''' <summary>Renders any array object Java-style; non-arrays fall back to <c>ToString</c>.</summary>
    Public Function toString(ByVal a As Object) As String
        If a Is Nothing Then Return "null"
        Dim arr = TryCast(a, Array)
        If arr Is Nothing Then Return FormatElement(a)
        Dim sb As New StringBuilder("[")
        Dim first As Boolean = True
        For Each element As Object In arr
            If Not first Then sb.Append(", ")
            first = False
            sb.Append(FormatElement(element))
        Next element
        Return sb.Append("]").ToString()
    End Function

    Public Function deepToString(ByVal a As Object) As String
        Return toString(a)
    End Function

    ''' <summary>Formats a single value the way Java's <c>String.valueOf</c> would (<c>null</c>, <c>true</c>, <c>1.0</c>).</summary>
    Public Function FormatElement(ByVal o As Object) As String
        If o Is Nothing Then Return "null"
        If TypeOf o Is Array Then Return toString(o)
        If TypeOf o Is Boolean Then Return If(CBool(o), "true", "false")
        If TypeOf o Is Double Then Return FormatFloating(CDbl(o))
        If TypeOf o Is Single Then Return FormatFloating(CDbl(CSng(o)))
        If TypeOf o Is IFormattable Then Return DirectCast(o, IFormattable).ToString(Nothing, CultureInfo.InvariantCulture)
        Return o.ToString()
    End Function

    Private Function FormatFloating(ByVal d As Double) As String
        If Double.IsNaN(d) Then Return "NaN"
        If Double.IsPositiveInfinity(d) Then Return "Infinity"
        If Double.IsNegativeInfinity(d) Then Return "-Infinity"
        Dim s As String = d.ToString("R", CultureInfo.InvariantCulture)
        If s.IndexOf("."c) < 0 AndAlso s.IndexOf("E"c) < 0 Then s &= ".0"
        Return s
    End Function

End Module
