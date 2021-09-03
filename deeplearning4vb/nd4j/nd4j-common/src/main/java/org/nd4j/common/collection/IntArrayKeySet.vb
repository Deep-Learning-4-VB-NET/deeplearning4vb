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

Namespace org.nd4j.common.collection


	Public Class IntArrayKeySet
		Implements ISet(Of Integer())

		Private set As ISet(Of IntArrayKeyMap.IntArray) = New LinkedHashSet(Of IntArrayKeyMap.IntArray)()
		Public Overrides Function size() As Integer
			Return set.Count
		End Function

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Return set.Count = 0
			End Get
		End Property

		Public Overrides Function contains(ByVal o As Object) As Boolean
			Return set.Contains(New IntArrayKeyMap.IntArray(DirectCast(o, Integer())))
		End Function

		Public Overrides Function iterator() As IEnumerator(Of Integer())
			Dim ret As IList(Of Integer()) = New List(Of Integer())()
			For Each arr As IntArrayKeyMap.IntArray In set
				ret.Add(arr.getBackingArray())
			Next arr

			Return ret.GetEnumerator()
		End Function

		Public Overrides Function toArray() As Object()
			Dim ret(Count - 1) As Object
			Dim count As Integer = 0
			For Each intArray As IntArrayKeyMap.IntArray In set
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[count++] = intArray.getBackingArray();
				ret(count) = intArray.getBackingArray()
					count += 1
			Next intArray

			Return ret
		End Function

		Public Overrides Function toArray(Of T)(ByVal ts() As T) As T()
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function add(ByVal ints() As Integer) As Boolean
			Return set.Add(New IntArrayKeyMap.IntArray(ints))
		End Function

		Public Overrides Function remove(ByVal o As Object) As Boolean
			Return set.remove(New IntArrayKeyMap.IntArray(DirectCast(o, Integer())))
		End Function

		Public Overrides Function containsAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Return set.ContainsAll(getCollection(collection))

		End Function

		Public Overrides Function addAll(Of T1 As Integer()(ByVal collection As ICollection(Of T1)) As Boolean
			Return set.addAll(getCollection(collection))
		End Function

		Public Overrides Function retainAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Return set.RetainAll(getCollection(collection))
		End Function

		Public Overrides Function removeAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Return set.RemoveAll(getCollection(collection))
		End Function

		Public Overrides Sub clear()
			set.Clear()
		End Sub

		Private Function getCollection(Of T1)(ByVal coll As ICollection(Of T1)) As ICollection(Of IntArrayKeyMap.IntArray)
			Dim ret As IList(Of IntArrayKeyMap.IntArray) = New List(Of IntArrayKeyMap.IntArray)()
			Dim casted As ICollection(Of Integer()) = CType(coll, ICollection(Of Integer()))
			For Each arr As Integer() In casted
				ret.Add(New IntArrayKeyMap.IntArray(arr))
			Next arr
			Return ret
		End Function

	End Class

End Namespace