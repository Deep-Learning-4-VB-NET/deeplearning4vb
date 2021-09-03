Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports org.nd4j.common.util

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

Namespace org.nd4j.common.io



	Public MustInherit Class CollectionUtils
		Public Sub New()
		End Sub

		Public Shared Function isEmpty(ByVal collection As System.Collections.ICollection) As Boolean
			Return collection Is Nothing OrElse collection.Count = 0
		End Function

		Public Shared Function isEmpty(ByVal map As System.Collections.IDictionary) As Boolean
			Return map Is Nothing OrElse map.Count = 0
		End Function

		Public Shared Function arrayToList(ByVal source As Object) As System.Collections.IList
			Return java.util.Arrays.asList(ObjectUtils.toObjectArray(source))
		End Function

		Public Shared Sub mergeArrayIntoCollection(ByVal array As Object, ByVal collection As System.Collections.ICollection)
			If collection Is Nothing Then
				Throw New System.ArgumentException("Collection must not be null")
			Else
				Dim arr() As Object = ObjectUtils.toObjectArray(array)
				Dim arr$() As Object = arr
				Dim len$ As Integer = arr.Length

				For i$ As Integer = 0 To len$ - 1
					Dim elem As Object = arr$(i$)
					collection.Add(elem)
				Next i$

			End If
		End Sub

		Public Shared Sub mergePropertiesIntoMap(ByVal props As Properties, ByVal map As System.Collections.IDictionary)
			If map Is Nothing Then
				Throw New System.ArgumentException("Map must not be null")
			Else
				Dim key As String
				Dim value As Object
				If props IsNot Nothing Then
					Dim en As System.Collections.IEnumerator = props.propertyNames()
					Do While en.MoveNext()
						key = CStr(en.Current)
						value = props.getProperty(key)
						If value Is Nothing Then
							value = props.get(key)
						End If
						map(key) = value
					Loop
				End If

			End If
		End Sub

		Public Shared Function contains(ByVal iterator As System.Collections.IEnumerator, ByVal element As Object) As Boolean
			If iterator IsNot Nothing Then
				Do While iterator.MoveNext()
					Dim candidate As Object = iterator.Current
					If ObjectUtils.nullSafeEquals(candidate, element) Then
						Return True
					End If
				Loop
			End If

			Return False
		End Function

		Public Shared Function contains(ByVal enumeration As System.Collections.IEnumerator, ByVal element As Object) As Boolean
			If enumeration IsNot Nothing Then
				Do While enumeration.MoveNext()
					Dim candidate As Object = enumeration.Current
					If ObjectUtils.nullSafeEquals(candidate, element) Then
						Return True
					End If
				Loop
			End If

			Return False
		End Function

		Public Shared Function containsInstance(ByVal collection As System.Collections.ICollection, ByVal element As Object) As Boolean
			If collection IsNot Nothing Then
				Dim i$ As System.Collections.IEnumerator = collection.GetEnumerator()

				Do While i$.MoveNext()
					Dim candidate As Object = i$.Current
					If candidate Is element Then
						Return True
					End If
				Loop
			End If

			Return False
		End Function

		Public Shared Function containsAny(ByVal source As System.Collections.ICollection, ByVal candidates As System.Collections.ICollection) As Boolean
			If Not isEmpty(source) AndAlso Not isEmpty(candidates) Then
				Dim i$ As System.Collections.IEnumerator = candidates.GetEnumerator()

				Dim candidate As Object
				Do
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If Not i$.hasNext() Then
						Return False
					End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					candidate = i$.next()
				Loop While Not source.Contains(candidate)

				Return True
			Else
				Return False
			End If
		End Function

		Public Shared Function findFirstMatch(ByVal source As System.Collections.ICollection, ByVal candidates As System.Collections.ICollection) As Object
			If Not isEmpty(source) AndAlso Not isEmpty(candidates) Then
				Dim i$ As System.Collections.IEnumerator = candidates.GetEnumerator()

				Dim candidate As Object
				Do
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If Not i$.hasNext() Then
						Return Nothing
					End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					candidate = i$.next()
				Loop While Not source.Contains(candidate)

				Return candidate
			Else
				Return Nothing
			End If
		End Function

		Public Shared Function findValueOfType(Of T, T1)(ByVal collection As ICollection(Of T1), ByVal type As Type(Of T)) As T
			If isEmpty(CType(collection, System.Collections.ICollection)) Then
				Return Nothing
			Else
				Dim value As Object = Nothing
				Dim i$ As System.Collections.IEnumerator = collection.GetEnumerator()

				Do While i$.MoveNext()
					Dim element As Object = i$.Current
					If type Is Nothing OrElse type.IsInstanceOfType(element) Then
						If value IsNot Nothing Then
							Return Nothing
						End If

						value = element
					End If
				Loop

				Return DirectCast(value, T)
			End If
		End Function

		Public Shared Function findValueOfType(Of T1)(ByVal collection As ICollection(Of T1), ByVal types() As Type) As Object
			If Not isEmpty(CType(collection, System.Collections.ICollection)) AndAlso Not ObjectUtils.isEmpty(types) Then
				Dim arr$() As Type = types
				Dim len$ As Integer = types.Length

				For i$ As Integer = 0 To len$ - 1
					Dim type As Type = arr$(i$)
					Dim value As Object = findValueOfType(collection, type)
					If value IsNot Nothing Then
						Return value
					End If
				Next i$

				Return Nothing
			Else
				Return Nothing
			End If
		End Function

		Public Shared Function hasUniqueObject(ByVal collection As System.Collections.ICollection) As Boolean
			If isEmpty(collection) Then
				Return False
			Else
				Dim hasCandidate As Boolean = False
				Dim candidate As Object = Nothing
				Dim i$ As System.Collections.IEnumerator = collection.GetEnumerator()

				Do While i$.MoveNext()
					Dim elem As Object = i$.Current
					If Not hasCandidate Then
						hasCandidate = True
						candidate = elem
					ElseIf candidate IsNot elem Then
						Return False
					End If
				Loop

				Return True
			End If
		End Function

		Public Shared Function findCommonElementType(ByVal collection As System.Collections.ICollection) As Type
			If isEmpty(collection) Then
				Return Nothing
			Else
				Dim candidate As Type = Nothing
				Dim i$ As System.Collections.IEnumerator = collection.GetEnumerator()

				Do While i$.MoveNext()
					Dim val As Object = i$.Current
					If val IsNot Nothing Then
						If candidate Is Nothing Then
							candidate = val.GetType()
						ElseIf candidate <> val.GetType() Then
							Return Nothing
						End If
					End If
				Loop

				Return candidate
			End If
		End Function

		Public Shared Function toArray(Of A, E As A)(ByVal enumeration As IEnumerator(Of E), ByVal array() As A) As A()
			Dim elements As New ArrayList()

			Do While enumeration.MoveNext()
				elements.Add(enumeration.Current)
			Loop

			Return CType(elements.toArray(array), A())
		End Function

		Public Shared Function toIterator(Of E)(ByVal enumeration As IEnumerator(Of E)) As IEnumerator(Of E)
			Return New CollectionUtils.EnumerationIterator(enumeration)
		End Function

		Public Shared Function toMultiValueMap(Of K, V)(ByVal map As IDictionary(Of K, IList(Of V))) As MultiValueMap(Of K, V)
			Return New CollectionUtils.MultiValueMapAdapter(map)
		End Function

		Public Shared Function unmodifiableMultiValueMap(Of K, V, T1 As K, T2 As V)(ByVal map As MultiValueMap(Of T1, T2)) As MultiValueMap(Of K, V)
			Assert.notNull(map, "'map' must not be null")
			Dim result As New LinkedHashMap(map.Count)
			Dim unmodifiableMap As System.Collections.IEnumerator = map.SetOfKeyValuePairs().GetEnumerator()

			Do While unmodifiableMap.MoveNext()
				Dim entry As DictionaryEntry = CType(unmodifiableMap.Current, DictionaryEntry)
				Dim values As System.Collections.IList = Collections.unmodifiableList(CType(entry.Value, System.Collections.IList))
				result.put(entry.Key, values)
			Loop

			Dim unmodifiableMap1 As System.Collections.IDictionary = Collections.unmodifiableMap(result)
			Return toMultiValueMap(unmodifiableMap1)
		End Function

		<Serializable>
		Private Class MultiValueMapAdapter(Of K, V)
			Implements MultiValueMap(Of K, V)

			Friend ReadOnly map As IDictionary(Of K, IList(Of V))

			Public Sub New(ByVal map As IDictionary(Of K, IList(Of V)))
				Assert.notNull(map, "'map' must not be null")
				Me.map = map
			End Sub

			Public Overridable Sub add(ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).add
				Dim values As IList(Of V) = Me.map(key)
				If values Is Nothing Then
					values = New LinkedList(Of V)()
					Me.map(key) = values
				End If

				values.Add(value)
			End Sub

			Public Overridable Function getFirst(ByVal key As K) As V Implements MultiValueMap(Of K, V).getFirst
				Dim values As System.Collections.IList = Me.map(key)
				Return If(values IsNot Nothing, CType(values(0), V), Nothing)
			End Function

			Public Overridable Sub set(ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).set
				Dim values As New LinkedList()
				values.AddLast(value)
				Me.map(key) = values
			End Sub

			Public Overridable WriteOnly Property All As IDictionary(Of K, V)
				Set(ByVal values As IDictionary(Of K, V))
					Dim i$ As System.Collections.IEnumerator = values.SetOfKeyValuePairs().GetEnumerator()
    
					Do While i$.MoveNext()
						Dim entry As DictionaryEntry = CType(i$.Current, DictionaryEntry)
						Me.set(CType(entry.Key, K), CType(entry.Value, V))
					Loop
    
				End Set
			End Property

			Public Overridable Function toSingleValueMap() As IDictionary(Of K, V)
				Dim singleValueMap As New LinkedHashMap(Me.map.Count)
				Dim i$ As System.Collections.IEnumerator = Me.map.SetOfKeyValuePairs().GetEnumerator()

				Do While i$.MoveNext()
					Dim entry As DictionaryEntry = CType(i$.Current, DictionaryEntry)
					singleValueMap.put(entry.Key, CType(entry.Value, System.Collections.IList)(0))
				Loop

				Return singleValueMap
			End Function

			Public Overridable ReadOnly Property Count As Integer
				Get
					Return Me.map.Count
				End Get
			End Property

			Public Overridable ReadOnly Property Empty As Boolean
				Get
					Return Me.map.Count = 0
				End Get
			End Property

			Public Overridable Function ContainsKey(ByVal key As Object) As Boolean
				Return Me.map.ContainsKey(key)
			End Function

			Public Overridable Function containsValue(ByVal value As Object) As Boolean
				Return Me.map.ContainsValue(value)
			End Function

			Public Overridable Function get(ByVal key As Object) As IList(Of V)
				Return Me.map(key)
			End Function

			Public Overridable Function put(ByVal key As K, ByVal value As IList(Of V)) As IList(Of V)
					Me.map(key) = value
					Return Me.map(key)
			End Function

			Public Overridable Function remove(ByVal key As Object) As IList(Of V)
				Return Me.map.Remove(key)
			End Function

			Public Overridable Sub putAll(Of T1 As K, T2 As IList(Of V)(ByVal m As IDictionary(Of T1, T2))
				Me.map.PutAll(m)
			End Sub

			Public Overridable Sub Clear()
				Me.map.Clear()
			End Sub

			Public Overridable Function keySet() As ISet(Of K)
				Return Me.map.Keys
			End Function

			Public Overridable ReadOnly Property Values As ICollection(Of IList(Of V))
				Get
					Return Me.map.Values
				End Get
			End Property

			Public Overridable Function entrySet() As ISet(Of KeyValuePair(Of K, IList(Of V)))
				Return Me.map.SetOfKeyValuePairs()
			End Function

			Public Overrides Function Equals(ByVal other As Object) As Boolean
				Return If(Me Is other, True, Me.map.Equals(other))
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Me.map.GetHashCode()
			End Function

			Public Overrides Function ToString() As String
				Return Me.map.ToString()
			End Function
		End Class

		Private Class EnumerationIterator(Of E)
			Implements IEnumerator(Of E)

			Friend enumeration As IEnumerator(Of E)

			Public Sub New(ByVal enumeration As IEnumerator(Of E))
				Me.enumeration = enumeration
			End Sub

			Public Overridable Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return Me.enumeration.hasMoreElements()
			End Function

			Public Overridable Function [next]() As E
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return Me.enumeration.nextElement()
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void remove() throws UnsupportedOperationException
			Public Overridable Sub remove()
				Throw New System.NotSupportedException("Not supported")
			End Sub
		End Class
	End Class

End Namespace