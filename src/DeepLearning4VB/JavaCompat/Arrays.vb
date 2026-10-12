Imports System

Friend Module Arrays
	Public Function CopyOf(Of T)(ByVal original() As T, ByVal newLength As Integer) As T()
		Dim dest(newLength - 1) As T
		Array.Copy(original, dest, newLength)
		Return dest
	End Function

	Public Function CopyOfRange(Of T)(ByVal original() As T, ByVal fromIndex As Integer, ByVal toIndex As Integer) As T()
		Dim length As Integer = toIndex - fromIndex
		Dim dest(length - 1) As T
		Array.Copy(original, fromIndex, dest, 0, length)
		Return dest
	End Function

	Public Sub Fill(Of T)(ByVal array() As T, ByVal value As T)
		For i As Integer = 0 To array.Length - 1
			array(i) = value
		Next i
	End Sub

	Public Sub Fill(Of T)(ByVal array() As T, ByVal fromIndex As Integer, ByVal toIndex As Integer, ByVal value As T)
		For i As Integer = fromIndex To toIndex - 1
			array(i) = value
		Next i
	End Sub
End Module