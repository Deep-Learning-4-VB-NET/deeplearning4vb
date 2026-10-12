Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Friend Module CollectionHelper
	<Extension> _
	Public Function ContainsAll(Of T)(ByVal c1 As ICollection(Of T), ByVal c2 As ICollection(Of T)) As Boolean
		If c2 Is Nothing Then
			Throw New NullReferenceException()
		End If

		For Each item As T In c2
			If Not c1.Contains(item) Then
				Return False
			End If
		Next item
		Return True
	End Function

	<Extension> _
	Public Function RemoveAll(Of T)(ByVal c1 As ICollection(Of T), ByVal c2 As ICollection(Of T)) As Boolean
		If c2 Is Nothing Then
			Throw New NullReferenceException()
		End If

		Dim changed As Boolean = False
		For Each item As T In c2
			If c1.Contains(item) Then
				c1.Remove(item)
				changed = True
			End If
		Next item
		Return changed
	End Function

	<Extension> _
	Public Function RetainAll(Of T)(ByVal c1 As ICollection(Of T), ByVal c2 As ICollection(Of T)) As Boolean
		If c2 Is Nothing Then
			Throw New NullReferenceException()
		End If

		Dim changed As Boolean = False
		Dim arrayCopy(c1.Count - 1) As T
		c1.CopyTo(arrayCopy, 0)
		For Each item As T In arrayCopy
			If Not c2.Contains(item) Then
				c1.Remove(item)
				changed = True
			End If
		Next item
		Return changed
	End Function
End Module