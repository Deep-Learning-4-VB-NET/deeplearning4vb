Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Friend Module HashMapHelper
	<Extension> _
	Public Function SetOfKeyValuePairs(Of TKey, TValue)(ByVal dictionary As IDictionary(Of TKey, TValue)) As HashSet(Of KeyValuePair(Of TKey, TValue))
		Dim entries As HashSet(Of KeyValuePair(Of TKey, TValue)) = New HashSet(Of KeyValuePair(Of TKey, TValue))()
		For Each keyValuePair As KeyValuePair(Of TKey, TValue) In dictionary
			entries.Add(keyValuePair)
		Next keyValuePair
		Return entries
	End Function

	<Extension> _
	Public Function GetValueOrNull(Of TKey, TValue)(ByVal dictionary As IDictionary(Of TKey, TValue), ByVal key As TKey) As TValue
		Dim ret As TValue
		dictionary.TryGetValue(key, ret)
		Return ret
	End Function

	<Extension> _
	Public Function GetOrDefault(Of TKey, TValue)(ByVal dictionary As IDictionary(Of TKey, TValue), ByVal key As TKey, ByVal defaultValue As TValue) As TValue
		Dim ret As TValue
		If dictionary.TryGetValue(key, ret) Then
			Return ret
		Else
			Return defaultValue
		End If
	End Function

	<Extension> _
	Public Sub PutAll(Of TKey, TValue)(ByVal d1 As IDictionary(Of TKey, TValue), ByVal d2 As IDictionary(Of TKey, TValue))
		If d2 Is Nothing Then
			Throw New NullReferenceException()
		End If

		For Each key As TKey In d2.Keys
			d1(key) = d2(key)
		Next key
	End Sub
End Module