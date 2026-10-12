Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports EqualsCondition = org.nd4j.linalg.indexing.conditions.EqualsCondition

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

Namespace org.nd4j.list


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public abstract class BaseNDArrayList<X extends Number> extends AbstractList<X>
	Public MustInherit Class BaseNDArrayList(Of X As Number)
		Inherits System.Collections.ObjectModel.Collection(Of X)

		Protected Friend container As INDArray
'JAVA TO VB CONVERTER NOTE: The field size was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend size_Conflict As Integer

		Friend Sub New()
			Me.container = Nd4j.create(10)
		End Sub

		''' <summary>
		''' Get a view of the underlying array
		''' relative to the size of the actual array.
		''' (Sometimes there are overflows in the internals
		''' but you want to use the internal INDArray for computing something
		''' directly, this gives you the relevant subset that reflects the content of the list) </summary>
		''' <returns> the view of the underlying ndarray relative to the collection's real size </returns>
		Public Overridable Function array() As INDArray
			Return container.get(NDArrayIndex.interval(0,size_Conflict)).reshape(1,size_Conflict)
		End Function

		Public Overrides Function size() As Integer
			Return size_Conflict
		End Function

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				Return size_Conflict = 0
			End Get
		End Property

		Public Overrides Function contains(ByVal o As Object) As Boolean
			Return indexOf(o) >= 0
		End Function

		Public Overrides Function iterator() As IEnumerator(Of X)
			Return New NDArrayListIterator(Me)
		End Function

		Public Overrides Function toArray() As Object()
			Dim number As Number = get(0)
			If TypeOf number Is Integer? Then
				Dim ret(size() - 1) As Integer?
				For i As Integer = 0 To ret.Length - 1
					ret(i) = CType(get(i), Integer?)
				Next i

				Return ret
			ElseIf TypeOf number Is Double? Then
				Dim ret(size() - 1) As Double?
				For i As Integer = 0 To ret.Length - 1
					ret(i) = CType(get(i), Double?)
				Next i

				Return ret
			ElseIf TypeOf number Is Single? Then
				Dim ret(size() - 1) As Single?
				For i As Integer = 0 To ret.Length - 1
					ret(i) = CType(get(i), Single?)
				Next i

				Return ret
			End If

			Throw New System.NotSupportedException("Unable to convert to array")
		End Function

		Public Overrides Function toArray(Of T)(ByVal ts() As T) As T()
			Dim number As Number = get(0)
			If TypeOf number Is Integer? Then
				Dim ret() As Integer? = CType(ts, Integer?())
				For i As Integer = 0 To ret.Length - 1
					ret(i) = CType(get(i), Integer?)
				Next i

				Return CType(ret, T())
			ElseIf TypeOf number Is Double? Then
				Dim ret(size() - 1) As Double?
				For i As Integer = 0 To ret.Length - 1
					ret(i) = CType(get(i), Double?)
				Next i

				Return CType(ret, T())
			ElseIf TypeOf number Is Single? Then
				Dim ret(size() - 1) As Single?
				For i As Integer = 0 To ret.Length - 1
					ret(i) = CType(get(i), Single?)
				Next i

				Return CType(ret, T())
			End If

			Throw New System.NotSupportedException("Unable to convert to array")
		End Function

		Public Overrides Function add(ByVal aX As X) As Boolean
			If container Is Nothing Then
				container = Nd4j.create(10)
			ElseIf size_Conflict = container.length() Then
				growCapacity(size_Conflict * 2)
			End If
			If DataTypeUtil.DtypeFromContext = DataType.DOUBLE Then
				container.putScalar(size_Conflict,aX.doubleValue())
			Else
				container.putScalar(size_Conflict,aX.floatValue())

			End If

			size_Conflict += 1
			Return True
		End Function

		Public Overrides Function remove(ByVal o As Object) As Boolean
			Dim idx As Integer = BooleanIndexing.firstIndex(container,New EqualsCondition(DirectCast(o, Double))).getInt(0)
			If idx < 0 Then
				Return False
			End If
			container.put(New INDArrayIndex(){NDArrayIndex.interval(idx,container.length())},container.get(NDArrayIndex.interval(idx + 1,container.length())))
			container = container.reshape(ChrW(1), size_Conflict)
			Return True
		End Function

		Public Overrides Function containsAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			For Each d As Object In collection
				If Not contains(d) Then
					Return False
				End If
			Next d

			Return True
		End Function

		Public Overrides Function addAll(Of T1 As X)(ByVal collection As ICollection(Of T1)) As Boolean
			If TypeOf collection Is BaseNDArrayList Then
				Dim ndArrayList As BaseNDArrayList = CType(collection, BaseNDArrayList)
				ndArrayList.growCapacity(Me.size() + collection.Count)

			Else
				For Each d As X In collection
					add(d)
				Next d
			End If
			Return True
		End Function

		Public Overrides Function addAll(Of T1 As X)(ByVal i As Integer, ByVal collection As ICollection(Of T1)) As Boolean

			For Each d As X In collection
				add(i,d)
			Next d

			Return True
		End Function

		Public Overrides Function removeAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			For Each d As Object In collection
				remove(d)
			Next d

			Return True
		End Function

		Public Overrides Function retainAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			Return False
		End Function

		Public Overrides Sub clear()
			size_Conflict = 0
			container = Nothing
		End Sub

		Public Overrides Function get(ByVal i As Integer) As X
			Dim ret As Number = container.getDouble(i)
			Return CType(ret, X)
		End Function

		Public Overrides Function set(ByVal i As Integer, ByVal aX As X) As X
			If DataTypeUtil.DtypeFromContext = DataType.DOUBLE Then
				container.putScalar(i,aX.doubleValue())
			Else
				container.putScalar(i,aX.floatValue())

			End If


			Return aX
		End Function

		Public Overrides Sub add(ByVal i As Integer, ByVal aX As X)
			rangeCheck(i)
			growCapacity(i)
			moveForward(i)
			If DataTypeUtil.DtypeFromContext = DataType.DOUBLE Then
				container.putScalar(i,aX.doubleValue())
			Else
				container.putScalar(i,aX.floatValue())

			End If

			size_Conflict += 1
		End Sub

		Public Overrides Function remove(ByVal i As Integer) As X
			rangeCheck(i)
			Dim numMoved As Integer = Me.size_Conflict - i - 1
			If numMoved > 0 Then
				Dim move As Number = container.getDouble(i)
				moveBackward(i)
				size_Conflict -= 1
				Return CType(move, X)
			End If

			Return Nothing
		End Function

		Public Overrides Function indexOf(ByVal o As Object) As Integer
			Return BooleanIndexing.firstIndex(container,New EqualsCondition(DirectCast(o, Double))).getInt(0)
		End Function

		Public Overrides Function lastIndexOf(ByVal o As Object) As Integer
			Return BooleanIndexing.lastIndex(container,New EqualsCondition(DirectCast(o, Double))).getInt(0)
		End Function

		Public Overrides Function listIterator() As IEnumerator(Of X)
			Return New NDArrayListIterator(Me)
		End Function

		Public Overrides Function listIterator(ByVal i As Integer) As IEnumerator(Of X)
			Return New NDArrayListIterator(Me, i)
		End Function



		Public Overrides Function ToString() As String
			Return container.get(NDArrayIndex.interval(0,size_Conflict)).ToString()
		End Function

		Private Class NDArrayListIterator
			Implements IEnumerator(Of X)

			Private ReadOnly outerInstance As BaseNDArrayList(Of X)

			Friend curr As Integer = 0

			Friend Sub New(ByVal outerInstance As BaseNDArrayList(Of X), ByVal curr As Integer)
				Me.outerInstance = outerInstance
				Me.curr = curr
			End Sub

			Friend Sub New(ByVal outerInstance As BaseNDArrayList(Of X))
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function hasNext() As Boolean
				Return curr < outerInstance.size_Conflict
			End Function

			Public Overrides Function [next]() As X
				Dim ret As X = outerInstance.get(curr)
				curr += 1
				Return ret
			End Function

			Public Overrides Function hasPrevious() As Boolean
				Return curr > 0
			End Function

			Public Overrides Function previous() As X
				Dim ret As X = outerInstance.get(curr - 1)
				curr -= 1
				Return ret
			End Function

			Public Overrides Function nextIndex() As Integer
				Return curr + 1
			End Function

			Public Overrides Function previousIndex() As Integer
				Return curr - 1
			End Function

			Public Overrides Sub remove()
				Throw New System.NotSupportedException()

			End Sub

			Public Overrides Sub set(ByVal aX As X)
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub add(ByVal aX As X)
				Throw New System.NotSupportedException()
			End Sub
		End Class



		Private Sub growCapacity(ByVal idx As Integer)
			If container Is Nothing Then
				container = Nd4j.create(10)
			ElseIf idx >= container.length() Then
				Dim max As val = Math.Max(container.length() * 2,idx)
				Dim newContainer As INDArray = Nd4j.create(max)
				newContainer.put(New INDArrayIndex(){NDArrayIndex.interval(0,container.length())},container)
				container = newContainer
			End If
		End Sub



		Private Sub rangeCheck(ByVal idx As Integer)
			If idx < 0 OrElse idx > size_Conflict Then
				Throw New System.ArgumentException("Illegal index " & idx)
			End If
		End Sub

		Private Sub moveBackward(ByVal index As Integer)
			Dim numMoved As Integer = size_Conflict - index - 1
			Dim first() As INDArrayIndex = {NDArrayIndex.point(0), NDArrayIndex.interval(index,index + numMoved)}
			Dim getRange() As INDArrayIndex = {NDArrayIndex.point(0), NDArrayIndex.interval(index + 1,index + 1 + numMoved)}
			Dim get As INDArray = container.get(getRange)
			container.put(first,get)
		End Sub

		Private Sub moveForward(ByVal index As Integer)
			Dim numMoved As Integer = size_Conflict - index - 1
			Dim getRange() As INDArrayIndex = {NDArrayIndex.point(0), NDArrayIndex.interval(index,index + numMoved)}
			Dim get As INDArray = container.get(getRange)
			Dim first() As INDArrayIndex = {NDArrayIndex.point(0), NDArrayIndex.interval(index + 1,index + 1 + get.length())}
			container.put(first,get)
		End Sub

	End Class

End Namespace