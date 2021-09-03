Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports org.nd4j.common.primitives
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.nd4j.linalg.dataset.api


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DataSetUtil
	Public Class DataSetUtil
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray tailor2d(@NonNull DataSet dataSet, boolean areFeatures)
		Public Shared Function tailor2d(ByVal dataSet As DataSet, ByVal areFeatures As Boolean) As INDArray
			Return tailor2d(If(areFeatures, dataSet.Features, dataSet.Labels),If(areFeatures, dataSet.FeaturesMaskArray, dataSet.LabelsMaskArray))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray tailor2d(@NonNull INDArray data, org.nd4j.linalg.api.ndarray.INDArray mask)
		Public Shared Function tailor2d(ByVal data As INDArray, ByVal mask As INDArray) As INDArray
			Select Case data.rank()
				Case 1, 2
					Return data
				Case 3
					Return tailor3d2d(data, mask)
				Case 4
					Return tailor4d2d(data)
				Case Else
					Throw New Exception("Unsupported data rank")
			End Select
		End Function

		''' <summary>
		''' @deprecated
		''' </summary>
		Public Shared Function tailor3d2d(ByVal dataset As DataSet, ByVal areFeatures As Boolean) As INDArray
			Dim data As INDArray = If(areFeatures, dataset.Features, dataset.Labels)
			Dim mask As INDArray = If(areFeatures, dataset.FeaturesMaskArray, dataset.LabelsMaskArray)
			Return tailor3d2d(data, mask)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray tailor3d2d(@NonNull INDArray data, org.nd4j.linalg.api.ndarray.INDArray mask)
		Public Shared Function tailor3d2d(ByVal data As INDArray, ByVal mask As INDArray) As INDArray
			'Check mask shapes:
			If mask IsNot Nothing Then
				If data.size(0) <> mask.size(0) OrElse data.size(2) <> mask.size(1) Then
					Throw New System.ArgumentException("Invalid mask array/data combination: got data with shape [minibatch, vectorSize, timeSeriesLength] = " & Arrays.toString(data.shape()) & "; got mask with shape [minibatch,timeSeriesLength] = " & Arrays.toString(mask.shape()) & "; minibatch and timeSeriesLength dimensions must match")
				End If
			End If


			If data.ordering() <> "f"c OrElse data.isView() OrElse Not Shape.strideDescendingCAscendingF(data) Then
				data = data.dup("f"c)
			End If
			'F order: strides are like [1, miniBatch, minibatch*size] - i.e., each time step array is contiguous in memory
			'This can be reshaped to 2d with a no-copy op
			'Same approach as RnnToFeedForwardPreProcessor in DL4J
			'I.e., we're effectively stacking time steps for all examples

			Dim shape As val = data.shape()
			Dim as2d As INDArray
			If shape(0) = 1 Then
				as2d = data.tensorAlongDimension(0, 1, 2).permutei(1, 0) 'Edge case: miniBatchSize==1
			ElseIf shape(2) = 1 Then
				as2d = data.tensorAlongDimension(0, 1, 0) 'Edge case: timeSeriesLength=1
			Else
				Dim permuted As INDArray = data.permute(0, 2, 1) 'Permute, so we get correct order after reshaping
				as2d = permuted.reshape("f"c, shape(0) * shape(2), shape(1))
			End If

			If mask Is Nothing Then
				Return as2d
			End If

			'With stride 1 along the examples (dimension 0), we are concatenating time series - same as the
			If mask.ordering() <> "f"c OrElse mask.View OrElse Not Shape.strideDescendingCAscendingF(mask) Then
				mask = mask.dup("f"c)
			End If

			Dim mask1d As INDArray = mask.reshape("f"c, New Long() {mask.length(), 1})

			'Assume masks are 0s and 1s: then sum == number of elements
			Dim numElements As Integer = mask.sumNumber().intValue()
			If numElements = mask.length() Then
				Return as2d 'All are 1s
			End If
			If numElements = 0 Then
				Return Nothing
			End If

			Dim rowsToPull(numElements - 1) As Integer
			Dim floatMask1d() As Single = mask1d.data().asFloat()
			Dim currCount As Integer = 0
			For i As Integer = 0 To floatMask1d.Length - 1
				If floatMask1d(i) <> 0.0f Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: rowsToPull[currCount++] = i;
					rowsToPull(currCount) = i
						currCount += 1
				End If
			Next i

			Dim subset As INDArray = Nd4j.pullRows(as2d, 1, rowsToPull) 'Tensor along dimension 1 == rows
			Return subset
		End Function

		Public Shared Function tailor4d2d(ByVal dataset As DataSet, ByVal areFeatures As Boolean) As INDArray
			Return tailor4d2d(If(areFeatures, dataset.Features, dataset.Labels))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray tailor4d2d(@NonNull INDArray data)
		Public Shared Function tailor4d2d(ByVal data As INDArray) As INDArray
			Dim instances As Long = data.size(0)
			Dim channels As Long = data.size(1)
			Dim height As Long = data.size(2)
			Dim width As Long = data.size(3)

			Dim in2d As INDArray = Nd4j.create(channels, height * width * instances)

			Dim tads As Long = data.tensorsAlongDimension(3, 2, 0)
			For i As Integer = 0 To tads - 1
				Dim thisTAD As INDArray = data.tensorAlongDimension(i, 3, 2, 0)
				in2d.putRow(i, Nd4j.toFlattened(thisTAD))
			Next i
			Return in2d.transposei()
		End Function

		Public Shared Sub setMaskedValuesToZero(ByVal data As INDArray, ByVal mask As INDArray)
			If mask Is Nothing OrElse data.rank() <> 3 Then
				Return
			End If

			Nd4j.Executioner.exec(New BroadcastMulOp(data, mask, data, 0, 2))
		End Sub

		''' <summary>
		''' Merge all of the features arrays into one minibatch.
		''' </summary>
		''' <param name="featuresToMerge">     features to merge. Note that first index is the input array (example) index, the second
		'''                            index is the input array.
		'''                            Thus to merge 10 examples with 3 input arrays each, featuresToMerge will be indexed
		'''                            like featuresToMerge[0..9][0..2] </param>
		''' <param name="featureMasksToMerge"> May be null. If non-null: feature masks to merge </param>
		''' <returns> Merged features, and feature masks. Note that feature masks may be added automatically, if required - even
		''' if no feature masks were present originally </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]> mergeFeatures(@NonNull INDArray[][] featuresToMerge, org.nd4j.linalg.api.ndarray.INDArray[][] featureMasksToMerge)
		Public Shared Function mergeFeatures(ByVal featuresToMerge()() As INDArray, ByVal featureMasksToMerge()() As INDArray) As Pair(Of INDArray(), INDArray())
			Dim nInArrs As Integer = featuresToMerge(0).Length
			Dim outF(nInArrs - 1) As INDArray
			Dim outM() As INDArray = Nothing

			For i As Integer = 0 To nInArrs - 1
				Dim p As Pair(Of INDArray, INDArray) = mergeFeatures(featuresToMerge, featureMasksToMerge, i)
				outF(i) = p.First
				If p.Second IsNot Nothing Then
					If outM Is Nothing Then
						outM = New INDArray(nInArrs - 1){}
					End If
					outM(i) = p.Second
				End If
			Next i
			Return New Pair(Of INDArray(), INDArray())(outF, outM)
		End Function

		''' <summary>
		''' Merge the specified features and mask arrays (i.e., concatenate the examples)
		''' </summary>
		''' <param name="featuresToMerge">     Features to merge </param>
		''' <param name="featureMasksToMerge"> Mask arrays to merge. May be null </param>
		''' <returns> Merged features and mask. Mask may be null </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray> mergeFeatures(@NonNull INDArray[] featuresToMerge, org.nd4j.linalg.api.ndarray.INDArray[] featureMasksToMerge)
		Public Shared Function mergeFeatures(ByVal featuresToMerge() As INDArray, ByVal featureMasksToMerge() As INDArray) As Pair(Of INDArray, INDArray)
			Preconditions.checkNotNull(featuresToMerge(0), "Encountered null feature array when merging")
			Dim rankFeatures As Integer = featuresToMerge(0).rank()

			Select Case rankFeatures
				Case 2
					Return DataSetUtil.merge2d(featuresToMerge, featureMasksToMerge)
				Case 3
					Return DataSetUtil.mergeTimeSeries(featuresToMerge, featureMasksToMerge)
				Case 4
					Return DataSetUtil.merge4d(featuresToMerge, featureMasksToMerge)
				Case Else
					Throw New System.InvalidOperationException("Cannot merge examples: features rank must be in range 2 to 4" & " inclusive. First example features shape: " & Arrays.toString(featuresToMerge(0).shape()))
			End Select
		End Function

		''' <summary>
		''' Extract out the specified column, and merge the specified features and mask arrays (i.e., concatenate the examples)
		''' </summary>
		''' <param name="featuresToMerge">     Features to merge. Will use featuresToMerge[all][inOutIdx] </param>
		''' <param name="featureMasksToMerge"> Mask arrays to merge. May be null </param>
		''' <returns> Merged features and mask. Mask may be null </returns>
		Public Shared Function mergeFeatures(ByVal featuresToMerge()() As INDArray, ByVal featureMasksToMerge()() As INDArray, ByVal inOutIdx As Integer) As Pair(Of INDArray, INDArray)
			Dim p As Pair(Of INDArray(), INDArray()) = selectColumnFromMDSData(featuresToMerge, featureMasksToMerge, inOutIdx)
			Return mergeFeatures(p.First, p.Second)
		End Function

		''' <summary>
		''' Merge the specified labels and label mask arrays (i.e., concatenate the examples)
		''' </summary>
		''' <param name="labelsToMerge">     Features to merge </param>
		''' <param name="labelMasksToMerge"> Mask arrays to merge. May be null </param>
		''' <returns> Merged features and mask. Mask may be null </returns>
		Public Shared Function mergeLabels(ByVal labelsToMerge() As INDArray, ByVal labelMasksToMerge() As INDArray) As Pair(Of INDArray, INDArray)
			Preconditions.checkNotNull(labelsToMerge(0), "Cannot merge data: Encountered null labels array")
			Dim rankFeatures As Integer = labelsToMerge(0).rank()

			Select Case rankFeatures
				Case 2
					Return DataSetUtil.merge2d(labelsToMerge, labelMasksToMerge)
				Case 3
					Return DataSetUtil.mergeTimeSeries(labelsToMerge, labelMasksToMerge)
				Case 4
					Return DataSetUtil.merge4d(labelsToMerge, labelMasksToMerge)
				Case Else
					Throw New ND4JIllegalStateException("Cannot merge examples: labels rank must be in range 2 to 4" & " inclusive. First example features shape: " & Arrays.toString(labelsToMerge(0).shape()))
			End Select
		End Function

		''' <summary>
		''' Extract out the specified column, and merge the specified label and label mask arrays
		''' (i.e., concatenate the examples)
		''' </summary>
		''' <param name="labelsToMerge">     Features to merge. Will use featuresToMerge[all][inOutIdx] </param>
		''' <param name="labelMasksToMerge"> Mask arrays to merge. May be null </param>
		''' <returns> Merged features and mask. Mask may be null </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray> mergeLabels(@NonNull INDArray[][] labelsToMerge, org.nd4j.linalg.api.ndarray.INDArray[][] labelMasksToMerge, int inOutIdx)
		Public Shared Function mergeLabels(ByVal labelsToMerge()() As INDArray, ByVal labelMasksToMerge()() As INDArray, ByVal inOutIdx As Integer) As Pair(Of INDArray, INDArray)
			Dim p As Pair(Of INDArray(), INDArray()) = selectColumnFromMDSData(labelsToMerge, labelMasksToMerge, inOutIdx)
			Return mergeLabels(p.First, p.Second)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[], org.nd4j.linalg.api.ndarray.INDArray[]> selectColumnFromMDSData(@NonNull INDArray[][] arrays, org.nd4j.linalg.api.ndarray.INDArray[][] masks, int inOutIdx)
		Private Shared Function selectColumnFromMDSData(ByVal arrays()() As INDArray, ByVal masks()() As INDArray, ByVal inOutIdx As Integer) As Pair(Of INDArray(), INDArray())
			Dim a(arrays.Length - 1) As INDArray
			Dim m(a.Length - 1) As INDArray
			For i As Integer = 0 To a.Length - 1
				a(i) = arrays(i)(inOutIdx)
				If masks IsNot Nothing AndAlso masks(i) IsNot Nothing Then
					m(i) = masks(i)(inOutIdx)
				End If
			Next i
			Return New Pair(Of INDArray(), INDArray())(a, m)
		End Function

		''' <summary>
		''' Merge the specified 2d arrays and masks. See <seealso cref="mergeFeatures(INDArray[], INDArray[])"/>
		''' and <seealso cref="mergeLabels(INDArray[], INDArray[])"/>
		''' </summary>
		''' <param name="arrays">   Arrays to merge </param>
		''' <param name="masks">    Mask arrays to merge </param>
		''' <param name="inOutIdx"> Index to extract out before merging </param>
		''' <returns> Merged arrays and mask </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray> merge2d(@NonNull INDArray[][] arrays, org.nd4j.linalg.api.ndarray.INDArray[][] masks, int inOutIdx)
		Public Shared Function merge2d(ByVal arrays()() As INDArray, ByVal masks()() As INDArray, ByVal inOutIdx As Integer) As Pair(Of INDArray, INDArray)
			Dim p As Pair(Of INDArray(), INDArray()) = selectColumnFromMDSData(arrays, masks, inOutIdx)
			Return merge2d(p.First, p.Second)
		End Function

		''' <summary>
		''' Merge the specified 2d arrays and masks. See <seealso cref="mergeFeatures(INDArray[], INDArray[])"/>
		''' and <seealso cref="mergeLabels(INDArray[], INDArray[])"/>
		''' </summary>
		''' <param name="arrays">   Arrays to merge </param>
		''' <param name="masks">    Mask arrays to merge </param>
		''' <returns> Merged arrays and mask </returns>
		Public Shared Function merge2d(ByVal arrays() As INDArray, ByVal masks() As INDArray) As Pair(Of INDArray, INDArray)
			Dim cols As Long = arrays(0).columns()

			Dim temp(arrays.Length - 1) As INDArray
			Dim hasMasks As Boolean = False
			For i As Integer = 0 To arrays.Length - 1
				Preconditions.checkNotNull(arrays(i), "Encountered null array at position %s when merging data", i)
				If arrays(i).columns() <> cols Then
					Throw New System.InvalidOperationException("Cannot merge 2d arrays with different numbers of columns (firstNCols=" & cols & ", ithNCols=" & arrays(i).columns() & ")")
				End If

				temp(i) = arrays(i)

				If masks IsNot Nothing AndAlso masks(i) IsNot Nothing AndAlso masks(i) IsNot Nothing Then
					hasMasks = True
				End If
			Next i

			Dim [out] As INDArray = Nd4j.specialConcat(0, temp)
			Dim outMask As INDArray = Nothing
			If hasMasks Then
				outMask = DataSetUtil.mergePerOutputMasks2d([out].shape(), arrays, masks)
			End If

			Return New Pair(Of INDArray, INDArray)([out], outMask)
		End Function


		Public Shared Function mergePerOutputMasks2d(ByVal outShape() As Long, ByVal arrays()() As INDArray, ByVal masks()() As INDArray, ByVal inOutIdx As Integer) As INDArray
			Dim p As Pair(Of INDArray(), INDArray()) = selectColumnFromMDSData(arrays, masks, inOutIdx)
			Return mergePerOutputMasks2d(outShape, p.First, p.Second)
		End Function

		''' @deprecated USe <seealso cref="mergeMasks2d(Long[], INDArray[], INDArray[])"/> 
		<Obsolete("USe <seealso cref=""mergeMasks2d(Long[], INDArray[], INDArray[])""/>")>
		Public Shared Function mergePerOutputMasks2d(ByVal outShape() As Long, ByVal arrays() As INDArray, ByVal masks() As INDArray) As INDArray
			Return mergeMasks2d(outShape, arrays, masks)
		End Function

		Public Shared Function mergeMasks2d(ByVal outShape() As Long, ByVal arrays() As INDArray, ByVal masks() As INDArray) As INDArray
			Dim numExamplesPerArr As val = New Long(arrays.Length - 1){}
			For i As Integer = 0 To numExamplesPerArr.length - 1
				numExamplesPerArr(i) = arrays(i).size(0)
			Next i

			Dim outMask As INDArray = Nd4j.ones(arrays(0).dataType(), outShape) 'Initialize to 'all present' (1s)

			Dim rowsSoFar As Integer = 0
			For i As Integer = 0 To masks.Length - 1
				Dim thisRows As Long = numExamplesPerArr(i) 'Mask itself may be null -> all present, but may include multiple examples
				If masks(i) Is Nothing Then
					Continue For
				End If

				outMask.put(New INDArrayIndex() {NDArrayIndex.interval(rowsSoFar, rowsSoFar + thisRows), NDArrayIndex.all()}, masks(i))
				rowsSoFar += thisRows
			Next i
			Return outMask
		End Function

		Public Shared Function mergeMasks4d(ByVal featuresOrLabels() As INDArray, ByVal masks() As INDArray) As INDArray
			Dim outShape() As Long = Nothing
			Dim mbCountNoMask As Long = 0
			For i As Integer = 0 To masks.Length - 1
				If masks(i) Is Nothing Then
					mbCountNoMask += featuresOrLabels(i).size(0)
					Continue For
				End If
				If masks(i).rank() <> 4 Then
					Throw New System.InvalidOperationException("Cannot merge mask arrays: expected mask array of rank 4. Got mask array of rank " & masks(i).rank() & " with shape " & Arrays.toString(masks(i).shape()))
				End If
				If outShape Is Nothing Then
					outShape = CType(masks(i).shape().Clone(), Long())
				Else
					Dim m As INDArray = masks(i)
					If m.size(1) <> outShape(1) OrElse m.size(2) <> outShape(2) OrElse m.size(3) <> outShape(3) Then
						Throw New System.InvalidOperationException("Mismatched mask shapes: masks should have same depth/height/width for all examples." & " Prior examples had shape [mb," & masks(1) & "," & masks(2) & "," & masks(3) & "], next example has shape " & Arrays.toString(m.shape()))
					End If
					outShape(0) += m.size(0)
				End If
			Next i

			If outShape Is Nothing Then
				Return Nothing 'No masks to merge
			End If

			outShape(0) += mbCountNoMask

			Dim outMask As INDArray = Nd4j.ones(outShape) 'Initialize to 'all present' (1s)

			Dim exSoFar As Integer = 0
			For i As Integer = 0 To masks.Length - 1
				If masks(i) Is Nothing Then
					exSoFar += featuresOrLabels(i).size(0)
					Continue For
				End If
				Dim nEx As Long = masks(i).size(0)

				outMask.put(New INDArrayIndex() {NDArrayIndex.interval(exSoFar, exSoFar + nEx), NDArrayIndex.all()}, masks(i))
				exSoFar += nEx
			Next i
			Return outMask
		End Function

		''' <summary>
		''' Merge the specified time series (3d) arrays and masks. See <seealso cref="mergeFeatures(INDArray[], INDArray[])"/>
		''' and <seealso cref="mergeLabels(INDArray[], INDArray[])"/>
		''' </summary>
		''' <param name="arrays">   Arrays to merge </param>
		''' <param name="masks">    Mask arrays to merge </param>
		''' <param name="inOutIdx"> Index to extract out before merging </param>
		''' <returns> Merged arrays and mask </returns>
		Public Shared Function mergeTimeSeries(ByVal arrays()() As INDArray, ByVal masks()() As INDArray, ByVal inOutIdx As Integer) As Pair(Of INDArray, INDArray)
			Dim p As Pair(Of INDArray(), INDArray()) = selectColumnFromMDSData(arrays, masks, inOutIdx)
			Return mergeTimeSeries(p.First, p.Second)
		End Function

		''' <summary>
		''' Merge the specified time series (3d) arrays and masks. See <seealso cref="mergeFeatures(INDArray[], INDArray[])"/>
		''' and <seealso cref="mergeLabels(INDArray[], INDArray[])"/>
		''' </summary>
		''' <param name="arrays">   Arrays to merge </param>
		''' <param name="masks">    Mask arrays to merge </param>
		''' <returns> Merged arrays and mask </returns>
		Public Shared Function mergeTimeSeries(ByVal arrays() As INDArray, ByVal masks() As INDArray) As Pair(Of INDArray, INDArray)
			'Merge time series data, and handle masking etc for different length arrays

			'Complications with time series:
			'(a) They may have different lengths (if so: need input + output masking arrays)
			'(b) Even if they are all the same length, they may have masking arrays (if so: merge the masking arrays too)
			'(c) Furthermore: mask arrays can be per-time-step (2d) or per output (3d). Per-input masks (3d feature masks)
			'    are not supported, however

			Dim firstLength As Long = arrays(0).size(2)
			Dim size As Long = arrays(0).size(1)
			Dim maxLength As Long = firstLength

			Dim hasMask As Boolean = False
			Dim maskRank As Integer = -1
			Dim lengthsDiffer As Boolean = False
			Dim totalExamples As Integer = 0
			For i As Integer = 0 To arrays.Length - 1
				totalExamples += arrays(i).size(0)
				Dim thisLength As Long = arrays(i).size(2)
				maxLength = Math.Max(maxLength, thisLength)
				If thisLength <> firstLength Then
					lengthsDiffer = True
				End If
				If masks IsNot Nothing AndAlso masks(i) IsNot Nothing AndAlso masks(i) IsNot Nothing Then
					maskRank = masks(i).rank()
					hasMask = True
				End If

				If arrays(i).size(1) <> size Then
					Throw New System.InvalidOperationException("Cannot merge time series with different size for dimension 1 (first shape: " & Arrays.toString(arrays(0).shape()) & ", " & i & "th shape: " & Arrays.toString(arrays(i).shape()))
				End If
			Next i

			Dim needMask As Boolean = hasMask OrElse lengthsDiffer
			Dim arr As INDArray = Nd4j.create(arrays(0).dataType(), totalExamples, size, maxLength)
			Dim mask As INDArray = (If(needMask AndAlso maskRank <> 3, Nd4j.ones(arrays(0).dataType(), totalExamples, maxLength), Nothing))

			'Now, merge the time series (and if necessary, mask arrays):
			Dim examplesSoFar As Integer = 0
			If Not lengthsDiffer AndAlso Not needMask Then
				'Simplest case: same length, no mask arrays
				For i As Integer = 0 To arrays.Length - 1
					Dim thisNExamples As Long = arrays(i).size(0)
					arr.put(New INDArrayIndex() {NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.all(), NDArrayIndex.all()}, arrays(i))
					examplesSoFar += thisNExamples
				Next i
				Return New Pair(Of INDArray, INDArray)(arr, Nothing)
			Else
				'Either different length, or have mask arrays (or, both)
				If (lengthsDiffer AndAlso Not hasMask) OrElse maskRank = 2 Then
					'Standard per-example masking required
					For i As Integer = 0 To arrays.Length - 1
						Dim a As INDArray = arrays(i)
						Dim thisNExamples As Long = a.size(0)
						Dim thisLength As Long = a.size(2)
						arr.put(New INDArrayIndex() {NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.all(), NDArrayIndex.interval(0, thisLength)}, a)

						If masks IsNot Nothing AndAlso masks(i) IsNot Nothing AndAlso masks(i) IsNot Nothing Then
							Dim origMask As INDArray = masks(i)
							Dim maskLength As Long = origMask.size(1)
							mask.put(New INDArrayIndex() { NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.interval(0, maskLength)}, origMask)
							If maskLength < maxLength Then
								'Set end mask array to zero...
								mask.put(New INDArrayIndex() { NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.interval(maskLength, maxLength)}, Nd4j.zeros(thisNExamples, maxLength - maskLength))
							End If
						Else
							If thisLength < maxLength Then
								'Mask the end
								mask.put(New INDArrayIndex() { NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.interval(thisLength, maxLength)}, Nd4j.zeros(thisNExamples, maxLength - thisLength))
							End If
						End If

						examplesSoFar += thisNExamples
					Next i
				ElseIf maskRank = 3 Then
					'Per output masking required. May also be variable length
					mask = Nd4j.create(arr.dataType(), arr.shape())
					For i As Integer = 0 To arrays.Length - 1
						Dim m As INDArray = masks(i)
						Dim a As INDArray = arrays(i)
						Dim thisNExamples As Long = a.size(0)
						Dim thisLength As Long = a.size(2)
						arr.put(New INDArrayIndex() {NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.all(), NDArrayIndex.interval(0, thisLength)}, a)

						If m Is Nothing Then
							'This mask is null -> equivalent to "all present"
							mask.get(NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.all(), NDArrayIndex.interval(0, thisLength)).assign(1)
						Else
							mask.put(New INDArrayIndex() { NDArrayIndex.interval(examplesSoFar, examplesSoFar + thisNExamples), NDArrayIndex.all(), NDArrayIndex.interval(0, thisLength)}, m)
						End If

						examplesSoFar += thisNExamples
					Next i
				Else
					Throw New System.NotSupportedException("Cannot merge time series with mask rank " & maskRank)
				End If
			End If

			Return New Pair(Of INDArray, INDArray)(arr, mask)
		End Function

		''' <summary>
		''' Merge the specified 4d arrays and masks. See <seealso cref="mergeFeatures(INDArray[], INDArray[])"/>
		''' and <seealso cref="mergeLabels(INDArray[], INDArray[])"/>
		''' </summary>
		''' <param name="arrays">   Arrays to merge </param>
		''' <param name="masks">    Mask arrays to merge </param>
		''' <param name="inOutIdx"> Index to extract out before merging </param>
		''' <returns> Merged arrays and mask </returns>
		Public Shared Function merge4d(ByVal arrays()() As INDArray, ByVal masks()() As INDArray, ByVal inOutIdx As Integer) As Pair(Of INDArray, INDArray)
			Dim p As Pair(Of INDArray(), INDArray()) = selectColumnFromMDSData(arrays, masks, inOutIdx)
			Return merge4d(p.First, p.Second)
		End Function

		''' <summary>
		''' Merge the specified 4d arrays and masks. See <seealso cref="mergeFeatures(INDArray[], INDArray[])"/>
		''' and <seealso cref="mergeLabels(INDArray[], INDArray[])"/>
		''' </summary>
		''' <param name="arrays">   Arrays to merge </param>
		''' <param name="masks">    Mask arrays to merge </param>
		''' <returns> Merged arrays and mask </returns>
		Public Shared Function merge4d(ByVal arrays() As INDArray, ByVal masks() As INDArray) As Pair(Of INDArray, INDArray)
			'4d -> images. In principle: could have 2d mask arrays (per-example masks)

			Dim nExamples As Integer = 0
			Dim shape() As Long = arrays(0).shape()
			Dim temp(arrays.Length - 1) As INDArray
			Dim hasMasks As Boolean = False
			Dim maskRank As Integer = -1
			For i As Integer = 0 To arrays.Length - 1
				Preconditions.checkNotNull(arrays(i), "Encountered null array when merging data at position %s", i)
				nExamples += arrays(i).size(0)
				Dim thisShape() As Long = arrays(i).shape()
				If thisShape.Length <> 4 Then
					Throw New System.InvalidOperationException("Cannot merge 4d arrays with non 4d arrays")
				End If
				For j As Integer = 1 To 3
					If thisShape(j) <> shape(j) Then
						Throw New System.InvalidOperationException("Cannot merge 4d arrays with different shape (other than # examples): " & " data[0].shape = " & Arrays.toString(shape) & ", data[" & i & "].shape = " & Arrays.toString(thisShape))
					End If
				Next j

				temp(i) = arrays(i)
				If masks IsNot Nothing AndAlso masks(i) IsNot Nothing Then
					hasMasks = True
					maskRank = masks(i).rank()
				End If
			Next i

			Dim [out] As INDArray = Nd4j.specialConcat(0, temp)
			Dim outMask As INDArray = Nothing
			If hasMasks Then
				If maskRank = 2 Then
					outMask = DataSetUtil.mergeMasks2d([out].shape(), arrays, masks)
				ElseIf maskRank = 4 Then
					outMask = DataSetUtil.mergeMasks4d(arrays, masks)
				End If
			End If

			Return New Pair(Of INDArray, INDArray)([out], outMask)
		End Function
	End Class

End Namespace