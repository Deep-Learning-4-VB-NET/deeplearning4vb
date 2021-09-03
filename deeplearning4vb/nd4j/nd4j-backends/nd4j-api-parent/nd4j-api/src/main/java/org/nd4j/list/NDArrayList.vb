Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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


	Public Class NDArrayList
		Inherits BaseNDArrayList(Of Double)

		Private Shadows container As INDArray
'JAVA TO VB CONVERTER NOTE: The field size was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shadows size_Conflict As Integer


		''' <summary>
		''' Initialize with the desired size.
		''' This will set the list.size()
		''' to be equal to the passed in size </summary>
		''' <param name="size"> the initial size of the array </param>
		Public Sub New(ByVal size As Integer)
			Me.New(DataType.DOUBLE, size)
		End Sub

		Public Sub New(ByVal dataType As DataType, ByVal size As Integer)
			Preconditions.checkState(size >= 0, "Size must be non-negative - got %s", size)
			Me.container = Nd4j.create(dataType, Math.Max(10L, size))
			Me.size_Conflict = size
		End Sub

		''' <summary>
		''' Specify the underlying ndarray for this list. </summary>
		''' <param name="container"> the underlying array. </param>
		''' <param name="size"> the initial size of the array. This will set list.size()
		'''             to be equal to the passed in size. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayList(@NonNull INDArray container,int size)
		Public Sub New(ByVal container As INDArray, ByVal size As Integer)
			Preconditions.checkState(container Is Nothing OrElse container.rank() = 1, "Container must be rank 1: is rank %s",If(container Is Nothing, 0, container.rank()))
			Me.container = container
			Me.size_Conflict = size
		End Sub

		Public Sub New()
			Me.New(0)
		End Sub

		''' <summary>
		''' Specify the underlying ndarray for this list. </summary>
		''' <param name="container"> the underlying array. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayList(@NonNull INDArray container)
		Public Sub New(ByVal container As INDArray)
			Me.New(container,0)
		End Sub


		''' <summary>
		''' Get a view of the underlying array
		''' relative to the size of the actual array.
		''' (Sometimes there are overflows in the internals
		''' but you want to use the internal INDArray for computing something
		''' directly, this gives you the relevant subset that reflects the content of the list) </summary>
		''' <returns> the view of the underlying ndarray relative to the collection's real size </returns>
		Public Overrides Function array() As INDArray
			If Empty Then
				Throw New ND4JIllegalStateException("Array is empty!")
			End If

			Return container.get(NDArrayIndex.interval(0,size_Conflict))
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
			Return IndexOf(o) >= 0
		End Function

		Public Overrides Function iterator() As IEnumerator(Of Double)
			Return New NDArrayListIterator(Me, Me)
		End Function

		Public Overrides Function toArray() As Object()
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function toArray(Of T)(ByVal ts() As T) As T()
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function add(ByVal aDouble As Double?) As Boolean
			If container Is Nothing Then
				container = Nd4j.create(10L)
			ElseIf size_Conflict = container.length() Then
				Dim newContainer As INDArray = Nd4j.create(container.length() * 2L)
				newContainer.put(New INDArrayIndex(){NDArrayIndex.interval(0,container.length())},container)
				container = newContainer
			End If

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: container.putScalar(size++,aDouble);
			container.putScalar(size_Conflict,aDouble)
				size_Conflict += 1
			Return True
		End Function



		Public Overrides Function remove(ByVal o As Object) As Boolean
			Dim idx As Integer = BooleanIndexing.firstIndex(container,New EqualsCondition(DirectCast(o, Double))).getInt(0)
			If idx < 0 Then
				Return False
			End If
			container.put(New INDArrayIndex(){NDArrayIndex.interval(idx,container.length())},container.get(NDArrayIndex.interval(idx + 1,container.length())))
			container = container.reshape(ChrW(size_Conflict))
			Return True
		End Function

		Public Overrides Function containsAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			For Each d As Object In collection
				If Not Contains(d) Then
					Return False
				End If
			Next d

			Return True
		End Function

		Public Overrides Function addAll(Of T1 As Double)(ByVal collection As ICollection(Of T1)) As Boolean
			If TypeOf collection Is NDArrayList Then
				Dim ndArrayList As NDArrayList = CType(collection, NDArrayList)
				growCapacity(Me.Count + collection.Count)
				container.put(New INDArrayIndex(){NDArrayIndex.interval(size_Conflict,size_Conflict + collection.Count)},ndArrayList.container.get(NDArrayIndex.interval(0,ndArrayList.Count)))
				size_Conflict += ndArrayList.Count
			Else
				For Each d As Double? In collection
					Add(d)
				Next d
			End If
			Return True
		End Function

		Public Overrides Function addAll(Of T1 As Double)(ByVal i As Integer, ByVal collection As ICollection(Of T1)) As Boolean
			For Each d As Double? In collection
				Insert(i,d)
			Next d

			Return True
		End Function

		Public Overrides Function removeAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
			For Each d As Object In collection
				Remove(d)
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

		Public Overrides Function get(ByVal i As Integer) As Double?
			Return container.getDouble(i)
		End Function

		Public Overrides Function set(ByVal i As Integer, ByVal aDouble As Double?) As Double?
			container.putScalar(i,aDouble)
			Return aDouble
		End Function

		Public Overrides Sub add(ByVal i As Integer, ByVal aDouble As Double?)
			rangeCheck(i)
			growCapacity(i)
			moveForward(i)
			container.putScalar(i,aDouble)
			size_Conflict += 1

		End Sub

		Public Overrides Function remove(ByVal i As Integer) As Double?
			rangeCheck(i)
			Dim numMoved As Integer = Me.size_Conflict - i - 1
			If numMoved > 0 Then
				Dim move As Double = container.getDouble(i)
				moveBackward(i)
				size_Conflict -= 1
				Return move
			End If

			Return Nothing
		End Function

		Public Overrides Function indexOf(ByVal o As Object) As Integer
			Return BooleanIndexing.firstIndex(container,New EqualsCondition(DirectCast(o, Double))).getInt(0)
		End Function

		Public Overrides Function lastIndexOf(ByVal o As Object) As Integer
			Return BooleanIndexing.lastIndex(container,New EqualsCondition(DirectCast(o, Double))).getInt(0)
		End Function

		Public Overrides Function listIterator() As IEnumerator(Of Double)
			Return New NDArrayListIterator(Me, Me)
		End Function

		Public Overrides Function listIterator(ByVal i As Integer) As IEnumerator(Of Double)
			Return New NDArrayListIterator(Me, Me, i)
		End Function

		Public Overrides Function subList(ByVal i As Integer, ByVal i1 As Integer) As IList(Of Double)
			Return New NDArrayList(container.get(NDArrayIndex.interval(i,i1)))
		End Function

		Public Overrides Function ToString() As String
			Return container.get(NDArrayIndex.interval(0,size_Conflict)).ToString()
		End Function

		Private Class NDArrayListIterator
			Implements IEnumerator(Of Double)

			Private ReadOnly outerInstance As NDArrayList

			Friend curr As Integer = 0

			Friend Sub New(ByVal outerInstance As NDArrayList, ByVal curr As Integer)
				Me.outerInstance = outerInstance
				Me.curr = curr
			End Sub

			Friend Sub New(ByVal outerInstance As NDArrayList)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function hasNext() As Boolean
				Return curr < outerInstance.size_Conflict
			End Function

			Public Overrides Function [next]() As Double?
				Dim ret As Double = outerInstance.get(curr)
				curr += 1
				Return ret
			End Function

			Public Overrides Function hasPrevious() As Boolean
				Return curr > 0
			End Function

			Public Overrides Function previous() As Double?
				Dim ret As Double = outerInstance.get(curr - 1)
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

			Public Overrides Sub set(ByVal aDouble As Double?)
				Throw New System.NotSupportedException()
			End Sub

			Public Overrides Sub add(ByVal aDouble As Double?)
				Throw New System.NotSupportedException()
			End Sub
		End Class



		Private Sub growCapacity(ByVal idx As Integer)
			If container Is Nothing Then
				container = Nd4j.create(10L)
			ElseIf idx >= container.length() Then
				Dim max As Long = Math.Max(container.length() * 2L,idx)
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
			Dim first() As INDArrayIndex = {NDArrayIndex.interval(index,index + numMoved)}
			Dim getRange() As INDArrayIndex = {NDArrayIndex.interval(index + 1,index + 1 + numMoved)}
			container.put(first,container.get(getRange))
		End Sub

		Private Sub moveForward(ByVal index As Integer)
			Dim numMoved As Integer = size_Conflict - index - 1
			Dim getRange() As INDArrayIndex = {NDArrayIndex.interval(index,index + numMoved)}
			Dim get As INDArray = container.get(getRange).dup()
			Dim first() As INDArrayIndex = {NDArrayIndex.interval(index + 1,index + 1 + get.length())}
			container.put(first,get)
		End Sub

	End Class

End Namespace