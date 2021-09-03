Imports System
Imports System.Collections.Generic

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

Namespace org.nd4j.common.util


	<Serializable>
	Public Class LinkedMultiValueMap(Of K, V)
		Implements MultiValueMap(Of K, V)

		Private Const serialVersionUID As Long = 3801124242820219131L
		Private ReadOnly targetMap As IDictionary(Of K, IList(Of V))

		Public Sub New()
			Me.targetMap = New LinkedHashMap()
		End Sub

		Public Sub New(ByVal initialCapacity As Integer)
			Me.targetMap = New LinkedHashMap(initialCapacity)
		End Sub

		Public Sub New(ByVal otherMap As IDictionary(Of K, IList(Of V)))
			Me.targetMap = New LinkedHashMap(otherMap)
		End Sub

		Public Overridable Sub add(ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).add
			Dim values As IList(Of V) = Me.targetMap(key)
			If values Is Nothing Then
				values = New LinkedList(Of V)()
				Me.targetMap(key) = values
			End If

			values.Add(value)
		End Sub

		Public Overridable Function getFirst(ByVal key As K) As V Implements MultiValueMap(Of K, V).getFirst
			Dim values As IList(Of V) = Me.targetMap(key)
			Return If(values IsNot Nothing, values(0), Nothing)
		End Function

		Public Overridable Sub set(ByVal key As K, ByVal value As V) Implements MultiValueMap(Of K, V).set
			Dim values As New LinkedList()
			values.AddLast(value)
			Me.targetMap(key) = values
		End Sub

		Public Overridable WriteOnly Property All As IDictionary(Of K, V)
			Set(ByVal values As IDictionary(Of K, V))
				Dim i$ As System.Collections.IEnumerator = values.SetOfKeyValuePairs().GetEnumerator()
    
				Do While i$.MoveNext()
					Dim entry As Entry(Of K, V) = CType(i$.Current, Entry)
					Me.set(entry.getKey(), entry.getValue())
				Loop
    
			End Set
		End Property

		Public Overridable Function toSingleValueMap() As IDictionary(Of K, V)
			Dim singleValueMap As New LinkedHashMap(Me.targetMap.Count)
			Dim i$ As System.Collections.IEnumerator = Me.targetMap.SetOfKeyValuePairs().GetEnumerator()

			Do While i$.MoveNext()
				Dim entry As Entry = CType(i$.Current, Entry)
				singleValueMap.put(entry.getKey(), CType(entry.getValue(), System.Collections.IList)(0))
			Loop

			Return singleValueMap
		End Function

		Public Overridable ReadOnly Property Count As Integer
			Get
				Return Me.targetMap.Count
			End Get
		End Property

		Public Overridable ReadOnly Property Empty As Boolean
			Get
				Return Me.targetMap.Count = 0
			End Get
		End Property

		Public Overridable Function ContainsKey(ByVal key As Object) As Boolean
			Return Me.targetMap.ContainsKey(key)
		End Function

		Public Overridable Function containsValue(ByVal value As Object) As Boolean
			Return Me.targetMap.ContainsValue(value)
		End Function

		Public Overridable Function get(ByVal key As Object) As IList(Of V)
			Return Me.targetMap(key)
		End Function

		Public Overridable Function put(ByVal key As K, ByVal value As IList(Of V)) As IList(Of V)
				Me.targetMap(key) = value
				Return Me.targetMap(key)
		End Function

		Public Overridable Function remove(ByVal key As Object) As IList(Of V)
			Return Me.targetMap.Remove(key)
		End Function

		Public Overridable Sub putAll(Of T1 As K, T2 As IList(Of V)(ByVal m As IDictionary(Of T1, T2))
			Me.targetMap.PutAll(m)
		End Sub

		Public Overridable Sub Clear()
			Me.targetMap.Clear()
		End Sub

		Public Overridable Function keySet() As ISet(Of K)
			Return Me.targetMap.Keys
		End Function

		Public Overridable ReadOnly Property Values As ICollection(Of IList(Of V))
			Get
				Return Me.targetMap.Values
			End Get
		End Property

		Public Overridable Function entrySet() As ISet(Of Entry(Of K, IList(Of V)))
			Return Me.targetMap.SetOfKeyValuePairs()
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.targetMap.Equals(obj)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.targetMap.GetHashCode()
		End Function

		Public Overrides Function ToString() As String
			Return Me.targetMap.ToString()
		End Function
	End Class

End Namespace