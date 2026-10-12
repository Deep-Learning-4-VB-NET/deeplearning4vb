Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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
	''' NDArray indexing
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class NDArrayIndex implements INDArrayIndex
	Public MustInherit Class NDArrayIndex
		Implements INDArrayIndex

		Public MustOverride ReadOnly Property Interval As Boolean Implements INDArrayIndex.isInterval

'JAVA TO VB CONVERTER NOTE: The variable indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private indices_Conflict() As Long
		Private Shared NEW_AXIS As New NewAxis()


		''' <summary>
		''' Returns a point index </summary>
		''' <param name="point"> the point index </param>
		''' <returns> the point index based
		''' on the specified point </returns>
'JAVA TO VB CONVERTER NOTE: The parameter point was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Shared Function point(ByVal point_Conflict As Long) As INDArrayIndex
			Return New PointIndex(point_Conflict)
		End Function

		''' <summary>
		''' Add indexes for the given shape </summary>
		''' <param name="shape"> the shape ot convert to indexes </param>
		''' <returns> the indexes for the given shape </returns>
		Public Shared Function indexesFor(ParamArray ByVal shape() As Long) As INDArrayIndex()
			Dim ret(shape.Length - 1) As INDArrayIndex
			For i As Integer = 0 To shape.Length - 1
				ret(i) = NDArrayIndex.point(shape(i))
			Next i

			Return ret
		End Function

		''' <summary>
		''' Compute the offset given an array of offsets.
		''' The offset is computed(for both fortran an d c ordering) as:
		''' sum from i to n - 1 o[i] * s[i]
		''' where i is the index o is the offset and s is the stride
		''' Notice the -1 at the end. </summary>
		''' <param name="arr"> the array to compute the offset for </param>
		''' <param name="offsets"> the offsets for each dimension </param>
		''' <returns> the offset that should be used for indexing </returns>
		Public Shared Function offset(ByVal arr As INDArray, ParamArray ByVal offsets() As Long) As Long
			Return offset(arr.stride(), offsets)
		End Function

		''' <summary>
		''' Compute the offset given an array of offsets.
		''' The offset is computed(for both fortran an d c ordering) as:
		''' sum from i to n - 1 o[i] * s[i]
		''' where i is the index o is the offset and s is the stride
		''' Notice the -1 at the end. </summary>
		''' <param name="arr"> the array to compute the offset for </param>
		''' <param name="indices"> the offsets for each dimension </param>
		''' <returns> the offset that should be used for indexing </returns>
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function offset(ByVal arr As INDArray, ParamArray ByVal indices_Conflict() As INDArrayIndex) As Long
			Return offset(arr.stride(), Indices.offsets(arr.shape(), indices_Conflict))
		End Function

		''' <summary>
		''' Compute the offset given an array of offsets.
		''' The offset is computed(for both fortran an d c ordering) as:
		''' sum from i to n - 1 o[i] * s[i]
		''' where i is the index o is the offset and s is the stride
		''' Notice the -1 at the end. </summary>
		''' <param name="strides"> the strides to compute the offset for </param>
		''' <param name="offsets"> the offsets for each dimension </param>
		''' <returns> the offset that should be used for indexing </returns>
		Public Shared Function offset(ByVal strides() As Long, ByVal offsets() As Long) As Long
			Dim ret As Integer = 0

			If ArrayUtil.prod(offsets) = 1 Then
				For i As Integer = 0 To offsets.Length - 1
					ret += offsets(i) * strides(i)
				Next i
			Else
				For i As Integer = 0 To offsets.Length - 1
					ret += offsets(i) * strides(i)
				Next i

			End If

			Return ret
		End Function

		Public Shared Function offset(ByVal strides() As Integer, ByVal offsets() As Long) As Long
			Dim ret As Integer = 0

			If ArrayUtil.prodLong(offsets) = 1 Then
				For i As Integer = 0 To offsets.Length - 1
					ret += offsets(i) * strides(i)
				Next i
			Else
				For i As Integer = 0 To offsets.Length - 1
					ret += offsets(i) * strides(i)
				Next i

			End If

			Return ret
		End Function


		''' <summary>
		''' Repeat a copy of copy n times </summary>
		''' <param name="copy"> the ndarray index to copy </param>
		''' <param name="n"> the number of times to copy </param>
		''' <returns> an array of length n containing copies of
		''' the given ndarray index </returns>
		Public Shared Function nTimes(ByVal copy As INDArrayIndex, ByVal n As Integer) As INDArrayIndex()
			Dim ret(n - 1) As INDArrayIndex
			For i As Integer = 0 To n - 1
				ret(i) = copy
			Next i

			Return ret
		End Function

		''' <summary>
		''' NDArrayIndexing based on the given
		''' indexes </summary>
		''' <param name="indices"> </param>
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Sub New(ParamArray ByVal indices_Conflict() As Long)
			Me.indices_Conflict = indices_Conflict
		End Sub

		''' <summary>
		''' Represents collecting all elements
		''' </summary>
		''' <returns> an ndarray index
		''' meaning collect
		''' all elements </returns>
		Public Shared Function all() As INDArrayIndex
			Return New NDArrayIndexAll()
		End Function

		''' <summary>
		''' Returns an instance of <seealso cref="SpecifiedIndex"/>.
		''' Note that SpecifiedIndex works differently than the other indexing options, in that it always returns a copy
		''' of the (subset of) the underlying array, for get operations. This means that INDArray.get(..., indices(x,y,z), ...)
		''' will be a copy of the relevant subset of the array. </summary>
		''' <param name="indices"> Indices to get </param>
'JAVA TO VB CONVERTER NOTE: The parameter indices was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function indices(ParamArray ByVal indices_Conflict() As Long) As INDArrayIndex
			Return New SpecifiedIndex(indices_Conflict)
		End Function


		''' <summary>
		''' Represents adding a new dimension </summary>
		''' <returns> the indexing for
		''' adding a new dimension </returns>
		Public Shared Function newAxis() As INDArrayIndex
			Return NEW_AXIS
		End Function

		''' <summary>
		''' Given an all index and
		''' the intended indexes, return an
		''' index array containing a combination of all elements
		''' for slicing and overriding particular indexes where necessary </summary>
		''' <param name="arr"> the array to resolve indexes for </param>
		''' <param name="intendedIndexes"> the indexes specified by the user </param>
		''' <returns> the resolved indexes (containing all where nothing is specified, and the intended index
		''' for a particular dimension otherwise) </returns>
		Public Shared Function resolve(ByVal arr As INDArray, ParamArray ByVal intendedIndexes() As INDArrayIndex) As INDArrayIndex()
			Return resolve(NDArrayIndex.allFor(arr), intendedIndexes)
		End Function

		''' <summary>
		''' Number of point indexes </summary>
		''' <param name="indexes"> the indexes
		'''                to count for points </param>
		''' <returns> the number of point indexes
		''' in the array </returns>
		Public Shared Function numPoints(ParamArray ByVal indexes() As INDArrayIndex) As Integer
			Dim ret As Integer = 0
			For i As Integer = 0 To indexes.Length - 1
				If TypeOf indexes(i) Is PointIndex Then
					ret += 1
				End If
			Next i
			Return ret
		End Function

		''' <summary>
		''' Given an all index and
		''' the intended indexes, return an
		''' index array containing a combination of all elements
		''' for slicing and overriding particular indexes where necessary </summary>
		''' <param name="shapeInfo"> the index containing all elements </param>
		''' <param name="intendedIndexes"> the indexes specified by the user </param>
		''' <returns> the resolved indexes (containing all where nothing is specified, and the intended index
		''' for a particular dimension otherwise) </returns>
		Public Shared Function resolveLong(ByVal shapeInfo() As Long, ParamArray ByVal intendedIndexes() As INDArrayIndex) As INDArrayIndex()
			Dim numSpecified As Integer = 0
			For i As Integer = 0 To intendedIndexes.Length - 1
				If TypeOf intendedIndexes(i) Is SpecifiedIndex Then
					numSpecified += 1
				End If
			Next i

			If numSpecified > 0 Then
				Dim shape As val = Shape.shapeOf(shapeInfo)
				Dim ret(intendedIndexes.Length - 1) As INDArrayIndex
				For i As Integer = 0 To intendedIndexes.Length - 1
					If TypeOf intendedIndexes(i) Is SpecifiedIndex Then
						ret(i) = intendedIndexes(i)
					Else
						If TypeOf intendedIndexes(i) Is NDArrayIndexAll Then
'JAVA TO VB CONVERTER NOTE: The variable specifiedIndex was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
							Dim specifiedIndex_Conflict As New SpecifiedIndex(ArrayUtil.range(0L, shape(i)))
							ret(i) = specifiedIndex_Conflict
						ElseIf TypeOf intendedIndexes(i) Is IntervalIndex Then
							Dim intervalIndex As IntervalIndex = DirectCast(intendedIndexes(i), IntervalIndex)
							ret(i) = New SpecifiedIndex(ArrayUtil.range(intervalIndex.begin, intervalIndex.end(), intervalIndex.stride()))
						ElseIf TypeOf intendedIndexes(i) Is PointIndex Then
							ret(i) = intendedIndexes(i)
						End If
					End If
				Next i

				Return ret
			End If


			''' <summary>
			''' If it's a vector and index asking
			''' for a scalar just return the array
			''' </summary>
			Dim rank As Integer = Shape.rank(shapeInfo)
			Dim shape As val = Shape.shapeOf(shapeInfo)
			If intendedIndexes.Length >= rank OrElse Shape.isVector(shapeInfo) AndAlso intendedIndexes.Length = 1 Then
				If Shape.rank(shapeInfo) = 1 Then
					'1D edge case, with 1 index
					Return intendedIndexes
				End If

				If Shape.isRowVectorShape(shapeInfo) AndAlso intendedIndexes.Length = 1 Then
					Dim ret(1) As INDArrayIndex
					ret(0) = NDArrayIndex.point(0)
					Dim size As Long
					If 1 = shape(0) AndAlso rank = 2 Then
						size = shape(1)
					Else
						size = shape(0)
					End If
					ret(1) = validate(size, intendedIndexes(0))
					Return ret
				End If
				Dim retList As IList(Of INDArrayIndex) = New List(Of INDArrayIndex)(intendedIndexes.Length)
				For i As Integer = 0 To intendedIndexes.Length - 1
					If i < rank Then
						retList.Add(validate(shape(i), intendedIndexes(i)))
					Else
						retList.Add(intendedIndexes(i))
					End If
				Next i
				Return CType(retList, List(Of INDArrayIndex)).ToArray()
			End If

			Dim retList As IList(Of INDArrayIndex) = New List(Of INDArrayIndex)(intendedIndexes.Length + 1)
			Dim numNewAxes As Integer = 0

			If Shape.isMatrix(shape) AndAlso intendedIndexes.Length = 1 Then
				retList.Add(validate(shape(0), intendedIndexes(0)))
				retList.Add(NDArrayIndex.all())
			Else
				For i As Integer = 0 To intendedIndexes.Length - 1
					retList.Add(validate(shape(i), intendedIndexes(i)))
					If TypeOf intendedIndexes(i) Is NewAxis Then
						numNewAxes += 1
					End If
				Next i
			End If

			Dim length As Integer = rank + numNewAxes
			'fill the rest with all
			Do While retList.Count < length
				retList.Add(NDArrayIndex.all())
			Loop

			Return CType(retList, List(Of INDArrayIndex)).ToArray()
		End Function

		''' <summary>
		''' Given an all index and
		''' the intended indexes, return an
		''' index array containing a combination of all elements
		''' for slicing and overriding particular indexes where necessary </summary>
		''' <param name="shape"> the index containing all elements </param>
		''' <param name="intendedIndexes"> the indexes specified by the user </param>
		''' <returns> the resolved indexes (containing all where nothing is specified, and the intended index
		''' for a particular dimension otherwise) </returns>
		Public Shared Function resolve(ByVal shape() As Integer, ParamArray ByVal intendedIndexes() As INDArrayIndex) As INDArrayIndex()
			Return resolve(ArrayUtil.toLongArray(shape), intendedIndexes)
		End Function

		Public Shared Function resolve(ByVal shape() As Long, ParamArray ByVal intendedIndexes() As INDArrayIndex) As INDArrayIndex()
			''' <summary>
			''' If it's a vector and index asking for a scalar just return the array
			''' </summary>
			If intendedIndexes.Length >= shape.Length OrElse Shape.isVector(shape) AndAlso intendedIndexes.Length = 1 Then
				If Shape.isRowVectorShape(shape) AndAlso intendedIndexes.Length = 1 Then
					Dim ret(1) As INDArrayIndex
					ret(0) = NDArrayIndex.point(0)
					Dim size As Long
					If 1 = shape(0) AndAlso shape.Length = 2 Then
						size = shape(1)
					Else
						size = shape(0)
					End If
					ret(1) = validate(size, intendedIndexes(0))
					Return ret
				End If
				Dim retList As IList(Of INDArrayIndex) = New List(Of INDArrayIndex)(intendedIndexes.Length)
				For i As Integer = 0 To intendedIndexes.Length - 1
					If i < shape.Length Then
						retList.Add(validate(shape(i), intendedIndexes(i)))
					Else
						retList.Add(intendedIndexes(i))
					End If
				Next i
				Return CType(retList, List(Of INDArrayIndex)).ToArray()
			End If

			Dim retList As IList(Of INDArrayIndex) = New List(Of INDArrayIndex)(intendedIndexes.Length + 1)
			Dim numNewAxes As Integer = 0

			If Shape.isMatrix(shape) AndAlso intendedIndexes.Length = 1 Then
				retList.Add(validate(shape(0), intendedIndexes(0)))
				retList.Add(NDArrayIndex.all())
			Else
				For i As Integer = 0 To intendedIndexes.Length - 1
					retList.Add(validate(shape(i), intendedIndexes(i)))
					If TypeOf intendedIndexes(i) Is NewAxis Then
						numNewAxes += 1
					End If
				Next i
			End If

			Dim length As Integer = shape.Length + numNewAxes
			'fill the rest with all
			Do While retList.Count < length
				retList.Add(NDArrayIndex.all())
			Loop



			Return CType(retList, List(Of INDArrayIndex)).ToArray()
		End Function

		Protected Friend Shared Function validate(ByVal size As Long, ByVal index As INDArrayIndex) As INDArrayIndex
			If (TypeOf index Is IntervalIndex OrElse TypeOf index Is PointIndex) AndAlso size <= index.offset() Then
				Throw New System.ArgumentException("NDArrayIndex is out of range. Beginning index: " & index.offset() & " must be less than its size: " & size)
			End If
			If TypeOf index Is IntervalIndex AndAlso index.end() > size Then
				Throw New System.ArgumentException("NDArrayIndex is out of range. End index: " & index.end() & " must be less than its size: " & size)
			End If
			If TypeOf index Is IntervalIndex AndAlso size < index.end() Then
				Dim begin As Long = DirectCast(index, IntervalIndex).begin
				index = NDArrayIndex.interval(begin, index.stride(), size)
			End If
			Return index
		End Function


		''' <summary>
		''' Given an all index and
		''' the intended indexes, return an
		''' index array containing a combination of all elements
		''' for slicing and overriding particular indexes where necessary </summary>
		''' <param name="allIndex"> the index containing all elements </param>
		''' <param name="intendedIndexes"> the indexes specified by the user </param>
		''' <returns> the resolved indexes (containing all where nothing is specified, and the intended index
		''' for a particular dimension otherwise) </returns>
		Public Shared Function resolve(ByVal allIndex() As INDArrayIndex, ParamArray ByVal intendedIndexes() As INDArrayIndex) As INDArrayIndex()

			Dim numNewAxes As Integer = numNewAxis(intendedIndexes)
			Dim all((allIndex.Length + numNewAxes) - 1) As INDArrayIndex
			Arrays.Fill(all, NDArrayIndex.all())
			For i As Integer = 0 To allIndex.Length - 1
				'collapse single length indexes in to point indexes
				If i >= intendedIndexes.Length Then
					Exit For
				End If

				If TypeOf intendedIndexes(i) Is NDArrayIndex Then
					Dim idx As NDArrayIndex = DirectCast(intendedIndexes(i), NDArrayIndex)
					If idx.indices_Conflict.Length = 1 Then
						intendedIndexes(i) = New PointIndex(idx.indices_Conflict(0))
					End If
				End If
				all(i) = intendedIndexes(i)
			Next i

			Return all
		End Function

		''' <summary>
		''' Given an array of indexes
		''' return the number of new axis elements
		''' in teh array </summary>
		''' <param name="axes"> the indexes to get the number
		'''             of new axes for </param>
		''' <returns> the number of new axis elements in the given array </returns>
		Public Shared Function numNewAxis(ParamArray ByVal axes() As INDArrayIndex) As Integer
			Dim ret As Integer = 0
			For Each index As INDArrayIndex In axes
				If TypeOf index Is NewAxis Then
					ret += 1
				End If
			Next index
			Return ret
		End Function


		''' <summary>
		''' Generate an all index
		''' equal to the rank of the given array </summary>
		''' <param name="arr"> the array to generate the all index for </param>
		''' <returns> an ndarray index array containing of length
		''' arr.rank() containing all elements </returns>
		Public Shared Function allFor(ByVal arr As INDArray) As INDArrayIndex()
			Dim ret(arr.rank() - 1) As INDArrayIndex
			For i As Integer = 0 To ret.Length - 1
				ret(i) = NDArrayIndex.all()
			Next i

			Return ret
		End Function

		''' <summary>
		''' Creates an index covering the given shape
		''' (for each dimension 0,shape[i]) </summary>
		''' <param name="shape"> the shape to cover </param>
		''' <returns> the ndarray indexes to cover </returns>
		Public Shared Function createCoveringShape(ByVal shape() As Integer) As INDArrayIndex()
			Dim ret(shape.Length - 1) As INDArrayIndex
			For i As Integer = 0 To ret.Length - 1
				ret(i) = NDArrayIndex.interval(0, shape(i))
			Next i
			Return ret
		End Function

		Public Shared Function createCoveringShape(ByVal shape() As Long) As INDArrayIndex()
			Dim ret(shape.Length - 1) As INDArrayIndex
			For i As Integer = 0 To ret.Length - 1
				ret(i) = NDArrayIndex.interval(0, shape(i))
			Next i
			Return ret
		End Function


		''' <summary>
		''' Create a range based on the given indexes.
		''' This is similar to create covering shape in that it approximates
		''' the length of each dimension (ignoring elements) and
		''' reproduces an index of the same dimension and length.
		''' </summary>
		''' <param name="indexes"> the indexes to create the range for </param>
		''' <returns> the index ranges. </returns>
		Public Shared Function rangeOfLength(ByVal indexes() As INDArrayIndex) As INDArrayIndex()
			Dim indexesRet(indexes.Length - 1) As INDArrayIndex
			For i As Integer = 0 To indexes.Length - 1
				indexesRet(i) = NDArrayIndex.interval(0, indexes(i).length())
			Next i
			Return indexesRet
		End Function

		''' <summary>
		''' Generates an interval from begin (inclusive) to end (exclusive)
		''' </summary>
		''' <param name="begin"> the begin </param>
		''' <param name="stride">  the stride at which to increment </param>
		''' <param name="end">   the end index </param>
		''' <param name="max"> the max length for this domain </param>
		''' <returns> the interval </returns>
		Public Shared Function interval(ByVal begin As Long, ByVal stride As Long, ByVal [end] As Long, ByVal max As Long) As INDArrayIndex
			If begin < 0 Then
				begin += max
			End If

			If [end] < 0 Then
				[end] += max
			End If

			If Math.Abs(begin - [end]) < 1 Then
				[end] += 1
			End If
			If stride > 1 AndAlso Math.Abs(begin - [end]) = 1 Then
				[end] *= stride
			End If
			Return interval(begin, stride, [end], False)
		End Function

		''' <summary>
		''' Generates an interval from begin (inclusive) to end (exclusive)
		''' </summary>
		''' <param name="begin"> the begin </param>
		''' <param name="stride">  the stride at which to increment </param>
		''' <param name="end">   the end index </param>
		''' <returns> the interval </returns>
		Public Shared Function interval(ByVal begin As Long, ByVal stride As Long, ByVal [end] As Long) As INDArrayIndex
			If Math.Abs(begin - [end]) < 1 Then
				[end] += 1
			End If
			If stride > 1 AndAlso Math.Abs(begin - [end]) = 1 Then
				[end] *= stride
			End If
			Return interval(begin, stride, [end], False)
		End Function

		''' <summary>
		''' Generates an interval from begin (inclusive) to end (exclusive)
		''' </summary>
		''' <param name="begin">     the begin </param>
		''' <param name="stride"> the stride at which to increment </param>
		''' <param name="end">       the end index </param>
		''' <param name="inclusive"> whether the end should be inclusive or not </param>
		''' <returns> the interval </returns>
		Public Shared Function interval(ByVal begin As Integer, ByVal stride As Integer, ByVal [end] As Integer, ByVal inclusive As Boolean) As INDArrayIndex
			Preconditions.checkArgument(begin <= [end], "Beginning index (%s) in range must be less than or equal to end (%s)", begin, [end])
			Dim index As INDArrayIndex = New IntervalIndex(inclusive, stride)
			index.init(begin, [end])
			Return index
		End Function



		Public Shared Function interval(ByVal begin As Long, ByVal stride As Long, ByVal [end] As Long, ByVal max As Long, ByVal inclusive As Boolean) As INDArrayIndex
			Preconditions.checkArgument(begin <= [end], "Beginning index (%s) in range must be less than or equal to end (%s)", begin, [end])
			Dim index As INDArrayIndex = New IntervalIndex(inclusive, stride)
			index.init(begin, [end])
			Return index
		End Function


		Public Shared Function interval(ByVal begin As Long, ByVal stride As Long, ByVal [end] As Long, ByVal inclusive As Boolean) As INDArrayIndex
			Preconditions.checkArgument(begin <= [end], "Beginning index (%s) in range must be less than or equal to end (%s)", begin, [end])
			Dim index As INDArrayIndex = New IntervalIndex(inclusive, stride)
			index.init(begin, [end])
			Return index
		End Function


		''' <summary>
		''' Generates an interval from begin (inclusive) to end (exclusive)
		''' </summary>
		''' <param name="begin"> the begin </param>
		''' <param name="end">   the end index </param>
		''' <returns> the interval </returns>
		Public Shared Function interval(ByVal begin As Integer, ByVal [end] As Integer) As INDArrayIndex
			Return interval(begin, 1, [end], False)
		End Function

		Public Shared Function interval(ByVal begin As Long, ByVal [end] As Long) As INDArrayIndex
			Return interval(begin, 1, [end], False)
		End Function

		''' <summary>
		''' Generates an interval from begin (inclusive) to end (exclusive)
		''' </summary>
		''' <param name="begin">     the begin </param>
		''' <param name="end">       the end index </param>
		''' <param name="inclusive"> whether the end should be inclusive or not </param>
		''' <returns> the interval </returns>
		Public Shared Function interval(ByVal begin As Long, ByVal [end] As Long, ByVal inclusive As Boolean) As INDArrayIndex
			Return interval(begin, 1, [end], inclusive)
		End Function

		Public Overridable Function [end]() As Long Implements INDArrayIndex.end
			If indices_Conflict IsNot Nothing AndAlso indices_Conflict.Length > 0 Then
				Return indices_Conflict(indices_Conflict.Length - 1)
			End If
			Return 0
		End Function

		Public Overridable Function offset() As Long Implements INDArrayIndex.offset
			If indices_Conflict.Length < 1 Then
				Return 0
			End If
			Return indices_Conflict(0)
		End Function

		''' <summary>
		''' Returns the length of the indices
		''' </summary>
		''' <returns> the length of the range </returns>
		Public Overridable Function length() As Long Implements INDArrayIndex.length
			Return indices_Conflict.Length
		End Function

		Public Overridable Function stride() As Long Implements INDArrayIndex.stride
			Return 1
		End Function

		Public Overridable Sub reverse() Implements INDArrayIndex.reverse
			ArrayUtil.reverse(indices_Conflict)
		End Sub

		Public Overrides Function ToString() As String
			Return "NDArrayIndex{" & "indices=" & Arrays.toString(indices_Conflict) & "}"c
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If Not (TypeOf o Is INDArrayIndex) Then
				Return False
			End If

			Dim that As NDArrayIndex = DirectCast(o, NDArrayIndex)

			If Not indices_Conflict.SequenceEqual(that.indices_Conflict) Then
				Return False
			End If
			Return True
		End Function


		Public Overrides Function GetHashCode() As Integer
			Return Arrays.hashCode(indices_Conflict)
		End Function

		Public Overridable Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal arr As INDArray, ByVal dimension As Integer) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long) Implements INDArrayIndex.init

		End Sub

		Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long) Implements INDArrayIndex.init

		End Sub

	End Class

End Namespace