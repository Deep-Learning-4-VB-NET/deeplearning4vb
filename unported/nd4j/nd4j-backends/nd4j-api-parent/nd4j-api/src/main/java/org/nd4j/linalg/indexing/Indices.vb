Imports System
Imports System.Collections.Generic
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports Longs = org.nd4j.shade.guava.primitives.Longs
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports NDArrayFactory = org.nd4j.linalg.factory.NDArrayFactory
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports LongUtils = org.nd4j.linalg.util.LongUtils

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

Namespace org.nd4j.linalg.indexing


	''' <summary>
	''' Indexing util.
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class Indices
		''' <summary>
		''' Compute the linear offset
		''' for an index in an ndarray.
		''' 
		''' For c ordering this is just the index itself.
		''' For fortran ordering, the following algorithm is used.
		''' 
		''' Assuming an ndarray is a list of vectors.
		''' The index of the vector relative to the given index is calculated.
		''' 
		''' vectorAlongDimension is then used along the last dimension
		''' using the computed index.
		''' 
		''' The offset + the computed column wrt the index: (index % the size of the last dimension)
		''' will render the given index in fortran ordering </summary>
		''' <param name="index"> the index </param>
		''' <param name="arr"> the array </param>
		''' <returns> the linear offset </returns>
		Public Shared Function rowNumber(ByVal index As Integer, ByVal arr As INDArray) As Integer
			Dim otherTest As Double = (CDbl(index)) / arr.size(-1)
			Dim test As Integer = CInt(Math.Truncate(Math.Floor(otherTest)))

			If arr.vectorsAlongDimension(-1) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim vectors As Integer = CInt(arr.vectorsAlongDimension(-1))
			If test >= vectors Then
				Return vectors - 1
			End If
			Return test
		End Function

		''' <summary>
		''' Compute the linear offset
		''' for an index in an ndarray.
		''' 
		''' For c ordering this is just the index itself.
		''' For fortran ordering, the following algorithm is used.
		''' 
		''' Assuming an ndarray is a list of vectors.
		''' The index of the vector relative to the given index is calculated.
		''' 
		''' vectorAlongDimension is then used along the last dimension
		''' using the computed index.
		''' 
		''' The offset + the computed column wrt the index: (index % the size of the last dimension)
		''' will render the given index in fortran ordering </summary>
		''' <param name="index"> the index </param>
		''' <param name="arr"> the array </param>
		''' <returns> the linear offset </returns>
		Public Shared Function linearOffset(ByVal index As Integer, ByVal arr As INDArray) As Long
			If arr.ordering() = NDArrayFactory.C Then
				Dim otherTest As Double = (CDbl(index)) Mod arr.size(-1)
				Dim test As Integer = CInt(Math.Truncate(Math.Floor(otherTest)))
				Dim vec As INDArray = arr.vectorAlongDimension(test, -1)
				Dim otherDim As Long = arr.vectorAlongDimension(test, -1).offset() + index
				Return otherDim
			Else
				Dim majorStride As Integer = arr.stride(-2)
				Dim vectorsAlongDimension As Long = arr.vectorsAlongDimension(-1)
				Dim rowCalc As Double = CDbl(index * majorStride) / CDbl(arr.length())
				Dim floor As Integer = CInt(Math.Truncate(Math.Floor(rowCalc)))

				Dim arrVector As INDArray = arr.vectorAlongDimension(floor, -1)

				Dim columnIndex As Long = index Mod arr.size(-1)
				Dim retOffset As Long = arrVector.linearIndex(columnIndex)
				Return retOffset



			End If
		End Function



		''' <summary>
		''' The offsets (begin index) for each index
		''' </summary>
		''' <param name="indices"> the indices </param>
		''' <returns> the offsets for the given set of indices </returns>
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function offsets(ByVal shape() As Long, ParamArray ByVal indices_Conflict() As INDArrayIndex) As Long()
			'offset of zero for every new axes
			Dim ret(shape.Length - 1) As Long

			If indices_Conflict.Length = shape.Length Then
				For i As Integer = 0 To indices_Conflict.Length - 1
					ret(i) = indices_Conflict(i).offset()
				Next i

				If ret.Length = 1 Then
					ret = New Long() {ret(0), 0}
				End If

			Else
				Dim numPoints As Integer = NDArrayIndex.numPoints(indices_Conflict)
				If numPoints > 0 Then
					Dim nonZeros As IList(Of Long) = New List(Of Long)()
					For i As Integer = 0 To indices_Conflict.Length - 1
						If indices_Conflict(i).offset() > 0 Then
							nonZeros.Add(indices_Conflict(i).offset())
						End If
					Next i
					If nonZeros.Count > shape.Length Then
						Throw New System.InvalidOperationException("Non zeros greater than shape unable to continue")
					Else
						'push all zeros to the back
						For i As Integer = 0 To nonZeros.Count - 1
							ret(i) = nonZeros(i)
						Next i
					End If
				Else
					Dim shapeIndex As Integer = 0
					For i As Integer = 0 To indices_Conflict.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ret[i] = indices[shapeIndex++].offset();
						ret(i) = indices_Conflict(shapeIndex).offset()
							shapeIndex += 1
					Next i
				End If


				If ret.Length = 1 Then
					ret = New Long() {ret(0), 0}
				End If
			End If



			Return ret
		End Function


		''' <summary>
		''' Fill in the missing indices to be the
		''' same length as the original shape.
		''' <p/>
		''' Think of this as what fills in the indices for numpy or matlab:
		''' Given a which is (4,3,2) in numpy:
		''' <p/>
		''' a[1:3] is filled in by the rest
		''' to give back the full slice
		''' <p/>
		''' This algorithm fills in that delta
		''' </summary>
		''' <param name="shape">   the original shape </param>
		''' <param name="indexes"> the indexes to start from </param>
		''' <returns> the filled in indices </returns>
		Public Shared Function fillIn(ByVal shape() As Integer, ParamArray ByVal indexes() As INDArrayIndex) As INDArrayIndex()
			If shape.Length = indexes.Length Then
				Return indexes
			End If

			Dim newIndexes(shape.Length - 1) As INDArrayIndex
			Array.Copy(indexes, 0, newIndexes, 0, indexes.Length)

			For i As Integer = indexes.Length To shape.Length - 1
				newIndexes(i) = NDArrayIndex.interval(0, shape(i))
			Next i
			Return newIndexes

		End Function

		''' <summary>
		''' Prunes indices of greater length than the shape
		''' and fills in missing indices if there are any
		''' </summary>
		''' <param name="originalShape"> the original shape to adjust to </param>
		''' <param name="indexes">       the indexes to adjust </param>
		''' <returns> the  adjusted indices </returns>
		Public Shared Function adjustIndices(ByVal originalShape() As Integer, ParamArray ByVal indexes() As INDArrayIndex) As INDArrayIndex()
			If Shape.isVector(originalShape) AndAlso indexes.Length = 1 Then
				Return indexes
			End If

			If indexes.Length < originalShape.Length Then
				indexes = fillIn(originalShape, indexes)
			End If
			If indexes.Length > originalShape.Length Then
				Dim ret(originalShape.Length - 1) As INDArrayIndex
				Array.Copy(indexes, 0, ret, 0, originalShape.Length)
				Return ret
			End If

			If indexes.Length = originalShape.Length Then
				Return indexes
			End If
			For i As Integer = 0 To indexes.Length - 1
				If indexes(i).end() >= originalShape(i) OrElse TypeOf indexes(i) Is NDArrayIndexAll Then
					indexes(i) = NDArrayIndex.interval(0, originalShape(i) - 1)
				End If
			Next i

			Return indexes
		End Function


		''' <summary>
		''' Calculate the strides based on the given indices
		''' </summary>
		''' <param name="ordering"> the ordering to calculate strides for </param>
		''' <param name="indexes">  the indices to calculate stride for </param>
		''' <returns> the strides for the given indices </returns>
		Public Shared Function strides(ByVal ordering As Char, ParamArray ByVal indexes() As NDArrayIndex) As Integer()
			Return Nd4j.getStrides(shape(indexes), ordering)
		End Function

		''' <summary>
		''' Calculate the shape for the given set of indices.
		''' <p/>
		''' The shape is defined as (for each dimension)
		''' the difference between the end index + 1 and
		''' the begin index
		''' </summary>
		''' <param name="indices"> the indices to calculate the shape for </param>
		''' <returns> the shape for the given indices </returns>
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function shape(ParamArray ByVal indices_Conflict() As INDArrayIndex) As Integer()
			Dim ret(indices_Conflict.Length - 1) As Integer
			For i As Integer = 0 To ret.Length - 1
				' FIXME: LONG
				ret(i) = CInt(indices_Conflict(i).length())
			Next i

			Dim nonZeros As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To ret.Length - 1
				If ret(i) > 0 Then
					nonZeros.Add(ret(i))
				End If
			Next i

			Return ArrayUtil.toArray(nonZeros)
		End Function



		''' <summary>
		''' Returns whether indices are contiguous
		''' by a certain amount or not
		''' </summary>
		''' <param name="indices"> the indices to test </param>
		''' <param name="diff">    the difference considered to be contiguous </param>
		''' <returns> whether the given indices are contiguous or not </returns>
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function isContiguous(ByVal indices_Conflict() As Integer, ByVal diff As Integer) As Boolean
			If indices_Conflict.Length < 1 Then
				Return True
			End If
			For i As Integer = 1 To indices_Conflict.Length - 1
				If Math.Abs(indices_Conflict(i) - indices_Conflict(i - 1)) > diff Then
					Return False
				End If
			Next i

			Return True
		End Function


		''' <summary>
		''' Create an n dimensional index
		''' based on the given interval indices.
		''' Start and end represent the begin and
		''' end of each interval </summary>
		''' <param name="start"> the start indexes </param>
		''' <param name="end"> the end indexes </param>
		''' <returns> the interval index relative to the given
		''' start and end indices </returns>
		Public Shared Function createFromStartAndEnd(ByVal start As INDArray, ByVal [end] As INDArray) As INDArrayIndex()
			If start.length() <> [end].length() Then
				Throw New System.ArgumentException("Start length must be equal to end length")
			Else
				If start.length() > Integer.MaxValue Then
					Throw New ND4JIllegalStateException("Can't proceed with INDArray with length > Integer.MAX_VALUE")
				End If

				Dim indexes(CInt(start.length()) - 1) As INDArrayIndex
				For i As Integer = 0 To indexes.Length - 1
					indexes(i) = NDArrayIndex.interval(start.getInt(i), [end].getInt(i))
				Next i
				Return indexes
			End If
		End Function


		''' <summary>
		''' Create indices representing intervals
		''' along each dimension </summary>
		''' <param name="start"> the start index </param>
		''' <param name="end"> the end index </param>
		''' <param name="inclusive"> whether the last
		'''                  index should be included </param>
		''' <returns> the ndarray indexes covering
		''' each dimension </returns>
		Public Shared Function createFromStartAndEnd(ByVal start As INDArray, ByVal [end] As INDArray, ByVal inclusive As Boolean) As INDArrayIndex()
			If start.length() <> [end].length() Then
				Throw New System.ArgumentException("Start length must be equal to end length")
			Else
				If start.length() > Integer.MaxValue Then
					Throw New ND4JIllegalStateException("Can't proceed with INDArray with length > Integer.MAX_VALUE")
				End If

				Dim indexes(CInt(start.length()) - 1) As INDArrayIndex
				For i As Integer = 0 To indexes.Length - 1
					indexes(i) = NDArrayIndex.interval(start.getInt(i), [end].getInt(i), inclusive)
				Next i
				Return indexes
			End If
		End Function


		''' <summary>
		''' Calculate the shape for the given set of indices and offsets.
		''' <p/>
		''' The shape is defined as (for each dimension)
		''' the difference between the end index + 1 and
		''' the begin index
		''' <p/>
		''' If specified, this will check for whether any of the indices are >= to end - 1
		''' and if so, prune it down
		''' </summary>
		''' <param name="shape">   the original shape </param>
		''' <param name="indices"> the indices to calculate the shape for </param>
		''' <returns> the shape for the given indices </returns>
'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function shape(ByVal shape_Conflict() As Integer, ParamArray ByVal indices_Conflict() As INDArrayIndex) As Integer()
			Return LongUtils.toInts(Indices.shape(LongUtils.toLongs(shape_Conflict), indices_Conflict))
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter shape was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function shape(ByVal shape_Conflict() As Long, ParamArray ByVal indices_Conflict() As INDArrayIndex) As Long()
			Dim newAxesPrepend As Integer = 0
			Dim encounteredAll As Boolean = False
			Dim accumShape As IList(Of Long) = New List(Of Long)()
			'bump number to read from the shape
			Dim shapeIndex As Integer = 0
			'list of indexes to prepend to for new axes
			'if all is encountered
			Dim prependNewAxes As IList(Of Integer) = New List(Of Integer)()
			For i As Integer = 0 To indices_Conflict.Length - 1
				Dim idx As INDArrayIndex = indices_Conflict(i)
				If TypeOf idx Is NDArrayIndexAll Then
					encounteredAll = True
				End If
				'point: do nothing but move the shape counter
				If TypeOf idx Is PointIndex Then
					shapeIndex += 1
					Continue For
				'new axes encountered, need to track whether to prepend or
				'to set the new axis in the middle
				ElseIf TypeOf idx Is NewAxis Then
					'prepend the new axes at different indexes
					If encounteredAll Then
						prependNewAxes.Add(i)
					'prepend to the beginning
					'rather than a set index
					Else
						newAxesPrepend += 1
					End If
					Continue For


				'points and intervals both have a direct desired length

				ElseIf TypeOf idx Is IntervalIndex AndAlso Not (TypeOf idx Is NDArrayIndexAll) OrElse TypeOf idx Is SpecifiedIndex Then
					accumShape.Add(idx.length())
					shapeIndex += 1
					Continue For
				End If

				accumShape.Add(shape_Conflict(shapeIndex))
				shapeIndex += 1

			Next i

			Do While shapeIndex < shape_Conflict.Length
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: accumShape.add(shape[shapeIndex++]);
				accumShape.Add(shape_Conflict(shapeIndex))
					shapeIndex += 1
			Loop


			Do While accumShape.Count < 2
				accumShape.Add(1L)
			Loop

			'only one index and matrix, remove the first index rather than the last
			'equivalent to this is reversing the list with the prepended one
			If indices_Conflict.Length = 1 AndAlso TypeOf indices_Conflict(0) Is PointIndex AndAlso shape_Conflict.Length = 2 Then
				accumShape.Reverse()
			End If

			'prepend for new axes; do this first before
			'doing the indexes to prepend to
			If newAxesPrepend > 0 Then
				For i As Integer = 0 To newAxesPrepend - 1
					accumShape.Insert(0, 1L)
				Next i
			End If

			''' <summary>
			''' For each dimension
			''' where we want to prepend a dimension
			''' we need to add it at the index such that
			''' we account for the offset of the number of indexes
			''' added up to that point.
			''' 
			''' We do this by doing an offset
			''' for each item added "so far"
			''' 
			''' Note that we also have an offset of - 1
			''' because we want to prepend to the given index.
			''' 
			''' When prepend new axes for in the middle is triggered
			''' i is already > 0
			''' </summary>
			For i As Integer = 0 To prependNewAxes.Count - 1
				accumShape.Insert(prependNewAxes(i) - i, 1L)
			Next i



			Return Longs.toArray(accumShape)
		End Function



		''' <summary>
		''' Return the stride to be used for indexing </summary>
		''' <param name="arr"> the array to get the strides for </param>
		''' <param name="indexes"> the indexes to use for computing stride </param>
		''' <param name="shape"> the shape of the output </param>
		''' <returns> the strides used for indexing </returns>
		Public Shared Function stride(ByVal arr As INDArray, ByVal indexes() As INDArrayIndex, ParamArray ByVal shape() As Integer) As Integer()
			Dim strides As IList(Of Integer) = New List(Of Integer)()
			Dim strideIndex As Integer = 0
			'list of indexes to prepend to for new axes
			'if all is encountered
			Dim prependNewAxes As IList(Of Integer) = New List(Of Integer)()

			For i As Integer = 0 To indexes.Length - 1
				'just like the shape, drops the stride
				If TypeOf indexes(i) Is PointIndex Then
					strideIndex += 1
					Continue For
				ElseIf TypeOf indexes(i) Is NewAxis Then

				End If
			Next i

			''' <summary>
			''' For each dimension
			''' where we want to prepend a dimension
			''' we need to add it at the index such that
			''' we account for the offset of the number of indexes
			''' added up to that point.
			''' 
			''' We do this by doing an offset
			''' for each item added "so far"
			''' 
			''' Note that we also have an offset of - 1
			''' because we want to prepend to the given index.
			''' 
			''' When prepend new axes for in the middle is triggered
			''' i is already > 0
			''' </summary>
			For i As Integer = 0 To prependNewAxes.Count - 1
				strides.Insert(prependNewAxes(i) - i, 1)
			Next i

			Return Ints.toArray(strides)

		End Function


		''' <summary>
		''' Check if the given indexes
		''' over the specified array
		''' are searching for a scalar </summary>
		''' <param name="indexOver"> the array to index over </param>
		''' <param name="indexes"> the index query </param>
		''' <returns> true if the given indexes are searching
		''' for a scalar false otherwise </returns>
		Public Shared Function isScalar(ByVal indexOver As INDArray, ParamArray ByVal indexes() As INDArrayIndex) As Boolean
			Dim allOneLength As Boolean = True
			For i As Integer = 0 To indexes.Length - 1
				allOneLength = allOneLength AndAlso indexes(i).length() = 1
			Next i

			Dim numNewAxes As Integer = NDArrayIndex.numNewAxis(indexes)
			If allOneLength AndAlso numNewAxes = 0 AndAlso indexes.Length = indexOver.rank() Then
				Return True
			ElseIf allOneLength AndAlso indexes.Length = indexOver.rank() - numNewAxes Then
				Return allOneLength
			End If

			Return allOneLength
		End Function


	End Class

End Namespace