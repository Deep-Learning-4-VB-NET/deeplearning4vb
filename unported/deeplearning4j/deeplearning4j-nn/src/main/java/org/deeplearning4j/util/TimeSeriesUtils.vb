Imports val = lombok.val
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports BaseRecurrentLayer = org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports LastTimeStep = org.deeplearning4j.nn.conf.layers.recurrent.LastTimeStep
Imports TimeDistributed = org.deeplearning4j.nn.conf.layers.recurrent.TimeDistributed
Imports MaskZeroLayer = org.deeplearning4j.nn.conf.layers.util.MaskZeroLayer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.util


	Public Class TimeSeriesUtils


		Private Sub New()
		End Sub

		''' <summary>
		''' Calculate a moving average given the length </summary>
		''' <param name="toAvg"> the array to average </param>
		''' <param name="n"> the length of the moving window </param>
		''' <returns> the moving averages for each row </returns>
		Public Shared Function movingAverage(ByVal toAvg As INDArray, ByVal n As Integer) As INDArray
			Dim ret As INDArray = Nd4j.cumsum(toAvg)
			Dim ends() As INDArrayIndex = {NDArrayIndex.interval(n, toAvg.columns())}
			Dim begins() As INDArrayIndex = {NDArrayIndex.interval(0, toAvg.columns() - n, False)}
			Dim nMinusOne() As INDArrayIndex = {NDArrayIndex.interval(n - 1, toAvg.columns())}
			ret.put(ends, ret.get(ends).sub(ret.get(begins)))
			Return ret.get(nMinusOne).divi(n)
		End Function

		''' <summary>
		''' Reshape time series mask arrays. This should match the assumptions (f order, etc) in RnnOutputLayer </summary>
		''' <param name="timeSeriesMask">    Mask array to reshape to a column vector </param>
		''' <returns>                  Mask array as a column vector </returns>
		Public Shared Function reshapeTimeSeriesMaskToVector(ByVal timeSeriesMask As INDArray) As INDArray
			If timeSeriesMask.rank() <> 2 Then
				Throw New System.ArgumentException("Cannot reshape mask: rank is not 2")
			End If

			If timeSeriesMask.ordering() <> "f"c Then
				timeSeriesMask = timeSeriesMask.dup("f"c)
			End If

			Return timeSeriesMask.reshape("f"c, timeSeriesMask.length(), 1)
		End Function


		''' <summary>
		''' Reshape time series mask arrays. This should match the assumptions (f order, etc) in RnnOutputLayer </summary>
		''' <param name="timeSeriesMask">    Mask array to reshape to a column vector </param>
		''' <returns>                  Mask array as a column vector </returns>
		Public Shared Function reshapeTimeSeriesMaskToVector(ByVal timeSeriesMask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If timeSeriesMask.rank() <> 2 Then
				Throw New System.ArgumentException("Cannot reshape mask: rank is not 2")
			End If

			If timeSeriesMask.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(timeSeriesMask) Then
				timeSeriesMask = workspaceMgr.dup(arrayType, timeSeriesMask, "f"c)
			End If

			Return workspaceMgr.leverageTo(arrayType, timeSeriesMask.reshape("f"c, timeSeriesMask.length(), 1))
		End Function

		''' <summary>
		''' Reshape time series mask arrays. This should match the assumptions (f order, etc) in RnnOutputLayer
		''' This reshapes from [X,1] to [X,1,1,1] suitable for per-example masking in CNNs </summary>
		''' <param name="timeSeriesMask">    Mask array to reshape for CNN per-example masking </param>
		''' <returns>                  Mask array as 4D CNN mask array: [X, 1, 1, 1] </returns>
		Public Shared Function reshapeTimeSeriesMaskToCnn4dMask(ByVal timeSeriesMask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If timeSeriesMask.rank() <> 2 Then
				Throw New System.ArgumentException("Cannot reshape mask: rank is not 2")
			End If

			If timeSeriesMask.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(timeSeriesMask) Then
				timeSeriesMask = workspaceMgr.dup(arrayType, timeSeriesMask, "f"c)
			End If

			Return workspaceMgr.leverageTo(arrayType, timeSeriesMask.reshape("f"c, timeSeriesMask.length(), 1, 1, 1))
		End Function

		''' <summary>
		''' Reshape time series mask arrays. This should match the assumptions (f order, etc) in RnnOutputLayer </summary>
		''' <param name="timeSeriesMaskAsVector">    Mask array to reshape to a column vector </param>
		''' <returns>                  Mask array as a column vector </returns>
		Public Shared Function reshapeVectorToTimeSeriesMask(ByVal timeSeriesMaskAsVector As INDArray, ByVal minibatchSize As Integer) As INDArray
			If Not timeSeriesMaskAsVector.Vector Then
				Throw New System.ArgumentException("Cannot reshape mask: expected vector")
			End If

			Dim timeSeriesLength As val = timeSeriesMaskAsVector.length() \ minibatchSize

			Return timeSeriesMaskAsVector.reshape("f"c, minibatchSize, timeSeriesLength)
		End Function

		''' <summary>
		''' Reshape CNN-style 4d mask array of shape [seqLength*minibatch,1,1,1] to time series mask [mb,seqLength]
		''' This should match the assumptions (f order, etc) in RnnOutputLayer </summary>
		''' <param name="timeSeriesMaskAsCnnMask">    Mask array to reshape </param>
		''' <returns>                  Sequence mask array - [minibatch, sequenceLength] </returns>
		Public Shared Function reshapeCnnMaskToTimeSeriesMask(ByVal timeSeriesMaskAsCnnMask As INDArray, ByVal minibatchSize As Integer) As INDArray
			Preconditions.checkArgument(timeSeriesMaskAsCnnMask.rank() = 4 OrElse timeSeriesMaskAsCnnMask.size(1) <> 1 OrElse timeSeriesMaskAsCnnMask.size(2) <> 1 OrElse timeSeriesMaskAsCnnMask.size(3) <> 1, "Expected rank 4 mask with shape [mb*seqLength, 1, 1, 1]. Got rank %s mask array with shape %s", timeSeriesMaskAsCnnMask.rank(), timeSeriesMaskAsCnnMask.shape())

			Dim timeSeriesLength As val = timeSeriesMaskAsCnnMask.length() \ minibatchSize

			Return timeSeriesMaskAsCnnMask.reshape("f"c, minibatchSize, timeSeriesLength)
		End Function

		Public Shared Function reshapePerOutputTimeSeriesMaskTo2d(ByVal perOutputTimeSeriesMask As INDArray) As INDArray
			If perOutputTimeSeriesMask.rank() <> 3 Then
				Throw New System.ArgumentException("Cannot reshape per output mask: rank is not 3 (is: " & perOutputTimeSeriesMask.rank() & ", shape = " & Arrays.toString(perOutputTimeSeriesMask.shape()) & ")")
			End If

			Return reshape3dTo2d(perOutputTimeSeriesMask)
		End Function

		Public Shared Function reshapePerOutputTimeSeriesMaskTo2d(ByVal perOutputTimeSeriesMask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If perOutputTimeSeriesMask.rank() <> 3 Then
				Throw New System.ArgumentException("Cannot reshape per output mask: rank is not 3 (is: " & perOutputTimeSeriesMask.rank() & ", shape = " & Arrays.toString(perOutputTimeSeriesMask.shape()) & ")")
			End If

			Return reshape3dTo2d(perOutputTimeSeriesMask, workspaceMgr, arrayType)
		End Function

		Public Shared Function reshape3dTo2d(ByVal [in] As INDArray) As INDArray
			If [in].rank() <> 3 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 3")
			End If
			Dim shape As val = [in].shape()
			If shape(0) = 1 Then
				Return [in].tensorAlongDimension(0, 1, 2).permutei(1, 0) 'Edge case: miniBatchSize==1
			End If
			If shape(2) = 1 Then
				Return [in].tensorAlongDimension(0, 1, 0) 'Edge case: timeSeriesLength=1
			End If
			Dim permuted As INDArray = [in].permute(0, 2, 1) 'Permute, so we get correct order after reshaping
			Return permuted.reshape("f"c, shape(0) * shape(2), shape(1))
		End Function

		Public Shared Function reshape3dTo2d(ByVal [in] As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If [in].rank() <> 3 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 3")
			End If
			Dim shape As val = [in].shape()
			Dim ret As INDArray
			If shape(0) = 1 Then
				ret = [in].tensorAlongDimension(0, 1, 2).permutei(1, 0) 'Edge case: miniBatchSize==1
			ElseIf shape(2) = 1 Then
				ret = [in].tensorAlongDimension(0, 1, 0) 'Edge case: timeSeriesLength=1
			Else
				Dim permuted As INDArray = [in].permute(0, 2, 1) 'Permute, so we get correct order after reshaping
				ret = permuted.reshape("f"c, shape(0) * shape(2), shape(1))
			End If
			Return workspaceMgr.leverageTo(arrayType, ret)
		End Function

		Public Shared Function reshape2dTo3d(ByVal [in] As INDArray, ByVal miniBatchSize As Integer) As INDArray
			If [in].rank() <> 2 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 2")
			End If
			'Based on: RnnToFeedForwardPreProcessor
			Dim shape As val = [in].shape()
			If [in].ordering() <> "f"c Then
				[in] = Shape.toOffsetZeroCopy([in], "f"c)
			End If
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim reshaped As INDArray = [in].reshape("f"c, miniBatchSize, shape(0) / miniBatchSize, shape(1))
			Return reshaped.permute(0, 2, 1)
		End Function


		Public Shared Function reshape2dTo3d(ByVal [in] As INDArray, ByVal miniBatchSize As Long, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If [in].rank() <> 2 Then
				Throw New System.ArgumentException("Invalid input: expect NDArray with rank 2")
			End If
			'Based on: RnnToFeedForwardPreProcessor
			Dim shape As val = [in].shape()
			If [in].ordering() <> "f"c Then
				[in] = workspaceMgr.dup(arrayType, [in], "f"c)
			End If
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim reshaped As INDArray = [in].reshape("f"c, miniBatchSize, shape(0) / miniBatchSize, shape(1))
			Return workspaceMgr.leverageTo(arrayType, reshaped.permute(0, 2, 1))
		End Function

		''' <summary>
		''' Reverse an input time series along the time dimension
		''' </summary>
		''' <param name="in"> Input activations to reverse, with shape [minibatch, size, timeSeriesLength] </param>
		''' <returns> Reversed activations </returns>
		Public Shared Function reverseTimeSeries(ByVal [in] As INDArray) As INDArray
			If [in] Is Nothing Then
				Return Nothing
			End If

			If [in].ordering() <> "f"c OrElse [in].View OrElse Not Shape.strideDescendingCAscendingF([in]) Then
				[in] = [in].dup("f"c)
			End If

			Dim idxs(CInt([in].size(2)) - 1) As Integer
			Dim j As Integer=0
			For i As Integer = idxs.Length-1 To 0 Step -1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idxs[j++] = i;
				idxs(j) = i
					j += 1
			Next i

			Dim inReshape As INDArray = [in].reshape("f"c, [in].size(0)*[in].size(1), [in].size(2))

			Dim outReshape As INDArray = Nd4j.pullRows(inReshape, 0, idxs, "f"c)
			Return outReshape.reshape("f"c, [in].size(0), [in].size(1), [in].size(2))
		End Function

		Public Shared Function reverseTimeSeries(ByVal [in] As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType, ByVal dataFormat As RNNFormat) As INDArray
			If dataFormat = RNNFormat.NCW Then
				Return reverseTimeSeries([in], workspaceMgr, arrayType)
			End If
			Return reverseTimeSeries([in].permute(0, 2, 1), workspaceMgr, arrayType).permute(0, 2, 1)
		End Function
		''' <summary>
		''' Reverse an input time series along the time dimension
		''' </summary>
		''' <param name="in"> Input activations to reverse, with shape [minibatch, size, timeSeriesLength] </param>
		''' <returns> Reversed activations </returns>
		Public Shared Function reverseTimeSeries(ByVal [in] As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If [in] Is Nothing Then
				Return Nothing
			End If

			If [in].ordering() <> "f"c OrElse [in].View OrElse Not Shape.strideDescendingCAscendingF([in]) Then
				[in] = workspaceMgr.dup(arrayType, [in], "f"c)
			End If

			If [in].size(2) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim idxs(CInt([in].size(2)) - 1) As Integer
			Dim j As Integer=0
			For i As Integer = idxs.Length-1 To 0 Step -1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idxs[j++] = i;
				idxs(j) = i
					j += 1
			Next i

			Dim inReshape As INDArray = [in].reshape("f"c, [in].size(0)*[in].size(1), [in].size(2))

			Dim outReshape As INDArray = workspaceMgr.create(arrayType, [in].dataType(), New Long(){inReshape.size(0), idxs.Length}, "f"c)
			Nd4j.pullRows(inReshape, outReshape, 0, idxs)
			Return workspaceMgr.leverageTo(arrayType, outReshape.reshape("f"c, [in].size(0), [in].size(1), [in].size(2)))

	'        
	'        INDArray out = Nd4j.createUninitialized(in.shape(), 'f');
	'        CustomOp op = DynamicCustomOp.builder("reverse")
	'                .addIntegerArguments(new int[]{0,1})
	'                .addInputs(in)
	'                .addOutputs(out)
	'                .callInplace(false)
	'                .build();
	'        Nd4j.getExecutioner().exec(op);
	'        return out;
	'        
		End Function

		''' <summary>
		''' Reverse a (per time step) time series mask, with shape [minibatch, timeSeriesLength] </summary>
		''' <param name="mask"> Mask to reverse along time dimension </param>
		''' <returns> Mask after reversing </returns>
		Public Shared Function reverseTimeSeriesMask(ByVal mask As INDArray) As INDArray
			If mask Is Nothing Then
				Return Nothing
			End If
			If mask.rank() = 3 Then
				'Should normally not be used - but handle the per-output masking case
				Return reverseTimeSeries(mask)
			ElseIf mask.rank() <> 2 Then
				Throw New System.ArgumentException("Invalid mask rank: must be rank 2 or 3. Got rank " & mask.rank() & " with shape " & Arrays.toString(mask.shape()))
			End If

			If mask.size(1) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim idxs(CInt(mask.size(1)) - 1) As Integer
			Dim j As Integer=0
			For i As Integer = idxs.Length-1 To 0 Step -1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idxs[j++] = i;
				idxs(j) = i
					j += 1
			Next i

			Return Nd4j.pullRows(mask, 0, idxs)
		End Function


		''' <summary>
		''' Reverse a (per time step) time series mask, with shape [minibatch, timeSeriesLength] </summary>
		''' <param name="mask"> Mask to reverse along time dimension </param>
		''' <returns> Mask after reversing </returns>
		Public Shared Function reverseTimeSeriesMask(ByVal mask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			If mask Is Nothing Then
				Return Nothing
			End If
			If mask.rank() = 3 Then
				'Should normally not be used - but handle the per-output masking case
				Return reverseTimeSeries(mask, workspaceMgr, arrayType)
			ElseIf mask.rank() <> 2 Then
				Throw New System.ArgumentException("Invalid mask rank: must be rank 2 or 3. Got rank " & mask.rank() & " with shape " & Arrays.toString(mask.shape()))
			End If

			If mask.size(1) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim idxs(CInt(mask.size(1)) - 1) As Integer
			Dim j As Integer=0
			For i As Integer = idxs.Length-1 To 0 Step -1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idxs[j++] = i;
				idxs(j) = i
					j += 1
			Next i

			Dim ret As INDArray = workspaceMgr.createUninitialized(arrayType, mask.dataType(), New Long(){mask.size(0), idxs.Length}, "f"c)

			Return Nd4j.pullRows(mask, ret, 0, idxs)

	'        
	'        //Assume input mask is 2d: [minibatch, tsLength]
	'        INDArray out = Nd4j.createUninitialized(mask.shape(), 'f');
	'        CustomOp op = DynamicCustomOp.builder("reverse")
	'                .addIntegerArguments(new int[]{1})
	'                .addInputs(mask)
	'                .addOutputs(out)
	'                .callInplace(false)
	'                .build();
	'        Nd4j.getExecutioner().exec(op);
	'        return out;
	'        
		End Function

		''' <summary>
		''' Extract out the last time steps (2d array from 3d array input) accounting for the mask layer, if present.
		''' </summary>
		''' <param name="pullFrom"> Input time series array (rank 3) to pull the last time steps from </param>
		''' <param name="mask">     Mask array (rank 2). May be null </param>
		''' <returns>         2d array of the last time steps </returns>
		Public Shared Function pullLastTimeSteps(ByVal pullFrom As INDArray, ByVal mask As INDArray) As Pair(Of INDArray, Integer())
			'Then: work out, from the mask array, which time step of activations we want, extract activations
			'Also: record where they came from (so we can do errors later)
			Dim fwdPassTimeSteps() As Integer
			Dim [out] As INDArray
			If mask Is Nothing Then

				'No mask array -> extract same (last) column for all
				Dim lastTS As Long = pullFrom.size(2) - 1
				[out] = pullFrom.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(lastTS))
				fwdPassTimeSteps = Nothing 'Null -> last time step for all examples
			Else
				Dim outShape As val = New Long() {pullFrom.size(0), pullFrom.size(1)}
				[out] = Nd4j.create(outShape)

				'Want the index of the last non-zero entry in the mask array
				Dim lastStepArr As INDArray = BooleanIndexing.lastIndex(mask, Conditions.epsNotEquals(0.0), 1)
				fwdPassTimeSteps = lastStepArr.data().asInt()

				'Now, get and assign the corresponding subsets of 3d activations:
				For i As Integer = 0 To fwdPassTimeSteps.Length - 1
					'TODO can optimize using reshape + pullRows
					[out].putRow(i, pullFrom.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(fwdPassTimeSteps(i))))
				Next i
			End If

			Return New Pair(Of INDArray, Integer())([out], fwdPassTimeSteps)
		End Function

		''' <summary>
		''' Extract out the last time steps (2d array from 3d array input) accounting for the mask layer, if present.
		''' </summary>
		''' <param name="pullFrom"> Input time series array (rank 3) to pull the last time steps from </param>
		''' <param name="mask">     Mask array (rank 2). May be null </param>
		''' <returns>         2d array of the last time steps </returns>
		Public Shared Function pullLastTimeSteps(ByVal pullFrom As INDArray, ByVal mask As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As Pair(Of INDArray, Integer())
			'Then: work out, from the mask array, which time step of activations we want, extract activations
			'Also: record where they came from (so we can do errors later)
			Dim fwdPassTimeSteps() As Integer
			Dim [out] As INDArray
			If mask Is Nothing Then

				'No mask array -> extract same (last) column for all
				Dim lastTS As Long = pullFrom.size(2) - 1
				[out] = pullFrom.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(lastTS))
				fwdPassTimeSteps = Nothing 'Null -> last time step for all examples
			Else
				Dim outShape As val = New Long() {pullFrom.size(0), pullFrom.size(1)}
				[out] = Nd4j.create(outShape)

				'Want the index of the last non-zero entry in the mask array
				Dim lastStepArr As INDArray = BooleanIndexing.lastIndex(mask, Conditions.epsNotEquals(0.0), 1)
				fwdPassTimeSteps = lastStepArr.data().asInt()

				'Now, get and assign the corresponding subsets of 3d activations:
				For i As Integer = 0 To fwdPassTimeSteps.Length - 1
					Dim lastStepIdx As Integer = fwdPassTimeSteps(i)
					Preconditions.checkState(lastStepIdx >= 0, "Invalid last time step index: example %s in minibatch is entirely masked out" & " (input mask is all 0s, meaning no input data is present for this example)", i)
					'TODO can optimize using reshape + pullRows
					[out].putRow(i, pullFrom.get(NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.point(lastStepIdx)))
				Next i
			End If

			Return New Pair(Of INDArray, Integer())(workspaceMgr.leverageTo(arrayType, [out]), fwdPassTimeSteps)
		End Function

		''' <summary>
		''' Get the <seealso cref="RNNFormat"/> from the RNN layer, accounting for the presence of wrapper layers like Bidirectional,
		''' LastTimeStep, etc </summary>
		''' <param name="layer"> Layer to get the RNNFormat from </param>
		Public Shared Function getFormatFromRnnLayer(ByVal layer As Layer) As RNNFormat
			If TypeOf layer Is BaseRecurrentLayer Then
				Return DirectCast(layer, BaseRecurrentLayer).getRnnDataFormat()
			ElseIf TypeOf layer Is MaskZeroLayer Then
				Return getFormatFromRnnLayer(DirectCast(layer, MaskZeroLayer).getUnderlying())
			ElseIf TypeOf layer Is Bidirectional Then
				Return getFormatFromRnnLayer(DirectCast(layer, Bidirectional).getFwd())
			ElseIf TypeOf layer Is LastTimeStep Then
				Return getFormatFromRnnLayer(DirectCast(layer, LastTimeStep).Underlying)
			ElseIf TypeOf layer Is TimeDistributed Then
				Return DirectCast(layer, TimeDistributed).getRnnDataFormat()
			Else
				Throw New System.InvalidOperationException("Unable to get RNNFormat from layer of type: " & layer)
			End If
		End Function
	End Class

End Namespace