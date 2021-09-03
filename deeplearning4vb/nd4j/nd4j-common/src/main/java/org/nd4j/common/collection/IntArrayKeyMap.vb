Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports Getter = lombok.Getter
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.common.collection


	Public Class IntArrayKeyMap(Of V)
		Implements IDictionary(Of Integer(), V)

		Private map As IDictionary(Of IntArray, V) = New LinkedHashMap(Of IntArray, V)()

		Public Overridable ReadOnly Property Count As Integer Implements ICollection(Of Integer(), V).Count
			Get
				Return map.Count
			End Get
		End Property

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Return map.Count = 0
			End Get
		End Property

		Public Overridable Function ContainsKey(ByVal o As Object) As Boolean Implements IDictionary(Of Integer(), V).ContainsKey
			Return map.ContainsKey(New IntArray(DirectCast(o, Integer())))
		End Function

		Public Overrides Function containsValue(ByVal o As Object) As Boolean
			Return map.ContainsValue(New IntArray(DirectCast(o, Integer())))
		End Function

		Public Overrides Function get(ByVal o As Object) As V
			Return map(New IntArray(DirectCast(o, Integer())))
		End Function

		Public Overrides Function put(ByVal ints() As Integer, ByVal v As V) As V
				map(New IntArray(ints)) = v
				Return map(New IntArray(ints))
		End Function

		Public Overrides Function remove(ByVal o As Object) As V
			Return map.Remove(New IntArray(DirectCast(o, Integer())))
		End Function

		Public Overrides Sub putAll(Of T1 As Integer(), T2 As V)(ByVal map As IDictionary(Of T1, T2))
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			for(Entry<? extends int(), ? extends V> entry : map.entrySet())
				Me.map(New IntArray(entry.getKey())) = entry.getValue()
			Next entry
		End Sub

		Public Overridable Sub Clear() Implements ICollection(Of Integer(), V).Clear
			map.Clear()
		End Sub

		Public Overrides Function keySet() As ISet(Of Integer())
			Dim intArrays As ISet(Of IntArray) = map.Keys
			Dim ret As ISet(Of Integer()) = New LinkedHashSet(Of Integer())()
			For Each intArray As IntArray In intArrays
				ret.Add(intArray.backingArray)
			Next intArray
			Return ret
		End Function

		Public Overridable Function getValues() As ICollection(Of V) Implements IDictionary(Of Integer(), V).Values
			Return map.Values
		End Function

		Public Overrides Function entrySet() As ISet(Of Entry(Of Integer(), V))
			Dim intArrays As ISet(Of KeyValuePair(Of IntArray, V)) = map.SetOfKeyValuePairs()
			Dim ret As ISet(Of Entry(Of Integer(), V)) = New LinkedHashSet(Of Entry(Of Integer(), V))()
			For Each intArray As KeyValuePair(Of IntArray, V) In intArrays
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Map.Entry<IntArray,V> intArray2 = intArray;
				Dim intArray2 As KeyValuePair(Of IntArray, V) = intArray
				ret.Add(New EntryAnonymousInnerClass(Me))
			Next intArray
			Return ret
		End Function

		Private Class EntryAnonymousInnerClass
			Inherits KeyValuePair(Of Integer(), V)

			Private ReadOnly outerInstance As IntArrayKeyMap(Of V)

			Public Sub New(ByVal outerInstance As IntArrayKeyMap(Of V))
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides ReadOnly Property Key As Integer()
				Get
					Return intArray2.getKey().backingArray
				End Get
			End Property

			Public Overrides ReadOnly Property Value As V
				Get
					Return intArray2.getValue()
				End Get
			End Property

			Public Overrides Function setValue(ByVal v As V) As V
				Return intArray2.setValue(v)
			End Function
		End Class


		Public Class IntArray
			Implements IComparable(Of IntArray)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int[] backingArray;
			Friend backingArray() As Integer

			Public Sub New(ByVal backingArray() As Integer)
				Preconditions.checkNotNull(backingArray,"Backing array must not be null!")
				Me.backingArray = Ints.toArray(New LinkedHashSet(Of )(Ints.asList(backingArray)))
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Me Is o Then
					Return True
				End If
				If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
					Return False
				End If

				Dim intArray As IntArray = DirectCast(o, IntArray)

				Return intArray.backingArray.SequenceEqual(backingArray)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return java.util.Arrays.hashCode(backingArray)
			End Function

			Public Overridable Function CompareTo(ByVal intArray As IntArray) As Integer Implements IComparable(Of IntArray).CompareTo
				If Me.backingArray.Length = 0 OrElse intArray.backingArray.Length = 0 Then
					Return 1

				ElseIf backingArray.SequenceEqual(intArray.backingArray) Then
					Return 1
				End If

				Return Ints.compare(Ints.max(backingArray),Ints.max(intArray.backingArray))
			End Function
		End Class


	End Class

End Namespace