Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastCopyOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastCopyOp
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports [Not] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Not
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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


	Public Class MaskedReductionUtil

		Private Shared ReadOnly CNN_DIM_MASK_H() As Integer = {0, 2}
		Private Shared ReadOnly CNN_DIM_MASK_W() As Integer = {0, 3}

		Private Sub New()
		End Sub

		Public Shared Function maskedPoolingTimeSeries(ByVal poolingType As PoolingType, ByVal toReduce As INDArray, ByVal mask As INDArray, ByVal pnorm As Integer, ByVal dataType As DataType) As INDArray
			If toReduce.rank() <> 3 Then
				Throw New System.ArgumentException("Expect rank 3 array: got " & toReduce.rank())
			End If
			If mask.rank() <> 2 Then
				Throw New System.ArgumentException("Expect rank 2 array for mask: got " & mask.rank())
			End If

			toReduce = toReduce.castTo(dataType)
			mask = mask.castTo(dataType)

			'Sum pooling: easy. Multiply by mask, then sum as normal
			'Average pooling: as above, but do a broadcast element-wise divi by mask.sum(1)
			'Max pooling: set to -inf if mask is 0, then do max as normal

			Select Case poolingType
				Case PoolingType.MAX
					Dim negInfMask As INDArray = mask.castTo(dataType).rsub(1.0)
					BooleanIndexing.replaceWhere(negInfMask, Double.NegativeInfinity, Conditions.equals(1.0))

					Dim withInf As INDArray = Nd4j.createUninitialized(dataType, toReduce.shape())
					Nd4j.Executioner.exec(New BroadcastAddOp(toReduce, negInfMask, withInf, 0, 2))
					'At this point: all the masked out steps have value -inf, hence can't be the output of the MAX op

					Return withInf.max(2)
				Case PoolingType.AVG, SUM
					Dim masked As INDArray = Nd4j.createUninitialized(dataType, toReduce.shape())
					Nd4j.Executioner.exec(New BroadcastMulOp(toReduce, mask, masked, 0, 2))
					Dim summed As INDArray = masked.sum(2)
					If poolingType = PoolingType.SUM Then
						Return summed
					End If

					Dim maskCounts As INDArray = mask.sum(1)
					summed.diviColumnVector(maskCounts)
					Return summed
				Case PoolingType.PNORM
					'Similar to average and sum pooling: there's no N term here, so we can just set the masked values to 0
					Dim masked2 As INDArray = Nd4j.createUninitialized(dataType, toReduce.shape())
					Nd4j.Executioner.exec(New BroadcastMulOp(toReduce, mask, masked2, 0, 2))

					Dim abs As INDArray = Transforms.abs(masked2, True)
					Transforms.pow(abs, pnorm, False)
'JAVA TO VB CONVERTER NOTE: The variable pNorm was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
					Dim MaskedReductionUtil.pNorm_Conflict As INDArray = abs.sum(2)

					Return Transforms.pow(MaskedReductionUtil.pNorm_Conflict, 1.0 / pnorm)
				Case Else
					Throw New System.NotSupportedException("Unknown or not supported pooling type: " & poolingType)
			End Select
		End Function

		Public Shared Function maskedPoolingEpsilonTimeSeries(ByVal poolingType As PoolingType, ByVal input As INDArray, ByVal mask As INDArray, ByVal epsilon2d As INDArray, ByVal pnorm As Integer) As INDArray

			If input.rank() <> 3 Then
				Throw New System.ArgumentException("Expect rank 3 input activation array: got " & input.rank())
			End If
			If mask.rank() <> 2 Then
				Throw New System.ArgumentException("Expect rank 2 array for mask: got " & mask.rank())
			End If


			'Mask: [minibatch, tsLength]
			'Epsilon: [minibatch, vectorSize]

			mask = mask.castTo(input.dataType())

			Select Case poolingType
				Case PoolingType.MAX
					Dim negInfMask As INDArray = mask.rsub(1.0)
					BooleanIndexing.replaceWhere(negInfMask, Double.NegativeInfinity, Conditions.equals(1.0))

					Dim withInf As INDArray = Nd4j.createUninitialized(input.dataType(), input.shape())
					Nd4j.Executioner.exec(New BroadcastAddOp(input, negInfMask, withInf, 0, 2))
					'At this point: all the masked out steps have value -inf, hence can't be the output of the MAX op

					Dim isMax As INDArray = Nd4j.exec(New IsMax(withInf, withInf.ulike(), 2))(0)

					Return Nd4j.Executioner.exec(New BroadcastMulOp(isMax, epsilon2d, isMax, 0, 1))
				Case PoolingType.AVG, SUM
					'if out = sum(in,dims) then dL/dIn = dL/dOut -> duplicate to each step and mask
					'if out = avg(in,dims) then dL/dIn = 1/N * dL/dOut
					'With masking: N differs for different time series

					Dim [out] As INDArray = Nd4j.createUninitialized(input.dataType(), input.shape(), "f"c)

					'Broadcast copy op, then divide and mask to 0 as appropriate
					Nd4j.Executioner.exec(New BroadcastCopyOp([out], epsilon2d, [out], 0, 1))
					Nd4j.Executioner.exec(New BroadcastMulOp([out], mask, [out], 0, 2))

					If poolingType = PoolingType.SUM Then
						Return [out]
					End If

					Dim nEachTimeSeries As INDArray = mask.sum(1) '[minibatchSize,tsLength] -> [minibatchSize,1]
					Nd4j.Executioner.exec(New BroadcastDivOp([out], nEachTimeSeries, [out], 0))

					Return [out]

				Case PoolingType.PNORM
					'Similar to average and sum pooling: there's no N term here, so we can just set the masked values to 0
					Dim masked2 As INDArray = Nd4j.createUninitialized(input.dataType(), input.shape())
					Nd4j.Executioner.exec(New BroadcastMulOp(input, mask, masked2, 0, 2))

					Dim abs As INDArray = Transforms.abs(masked2, True)
					Transforms.pow(abs, pnorm, False)
'JAVA TO VB CONVERTER NOTE: The variable pNorm was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
					Dim MaskedReductionUtil.pNorm_Conflict As INDArray = Transforms.pow(abs.sum(2), 1.0 / pnorm)

					Dim numerator As INDArray
					If pnorm = 2 Then
						numerator = input.dup()
					Else
						Dim absp2 As INDArray = Transforms.pow(Transforms.abs(input, True), pnorm - 2, False)
						numerator = input.mul(absp2)
					End If

					Dim denom As INDArray = Transforms.pow(MaskedReductionUtil.pNorm_Conflict, pnorm - 1, False)
					'3d shape with trailing dimension of 1
					If epsilon2d.rank() <> denom.rank() AndAlso denom.length() = epsilon2d.length() Then
						epsilon2d = epsilon2d.reshape(denom.shape())
					End If
					denom.rdivi(epsilon2d)
					Nd4j.Executioner.execAndReturn(New BroadcastMulOp(numerator, denom, numerator, 0, 1))
					Nd4j.Executioner.exec(New BroadcastMulOp(numerator, mask, numerator, 0, 2)) 'Apply mask

					Return numerator
				Case Else
					Throw New System.NotSupportedException("Unknown or not supported pooling type: " & poolingType)
			End Select
		End Function


		Public Shared Function maskedPoolingConvolution(ByVal poolingType As PoolingType, ByVal toReduce As INDArray, ByVal mask As INDArray, ByVal pnorm As Integer, ByVal dataType As DataType) As INDArray
			If mask.rank() <> 4 Then
				'TODO BETTER ERROR MESSAGE EXPLAINING FORMAT
				'TODO ALSO HANDLE LEGACY FORMAT WITH WARNING WHERE POSSIBLE
				Throw New System.InvalidOperationException("Expected rank 4 mask array: Got array with shape " & Arrays.toString(mask.shape()))
			End If

			mask = mask.castTo(dataType) 'no-op if already correct dtype

			' [minibatch, channels, h, w] data with a mask array of shape [minibatch, 1, X, Y]
			' where X=(1 or inH) and Y=(1 or inW)

			'General case: must be equal or 1 on each dimension
			Dim dimensions(3) As Integer
			Dim count As Integer = 0
			For i As Integer = 0 To 3
				If toReduce.size(i) = mask.size(i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: dimensions[count++] = i;
					dimensions(count) = i
						count += 1
				End If
			Next i
			If count < 4 Then
				dimensions = Arrays.CopyOfRange(dimensions, 0, count)
			End If

			Select Case poolingType
				Case PoolingType.MAX
					'TODO This is ugly - replace it with something better... Need something like a Broadcast CAS op
					Dim negInfMask As INDArray
					If mask.dataType() = DataType.BOOL Then
						negInfMask = Transforms.not(mask).castTo(dataType)
					Else
						negInfMask = mask.rsub(1.0)
					End If
					BooleanIndexing.replaceWhere(negInfMask, Double.NegativeInfinity, Conditions.equals(1.0))

					Dim withInf As INDArray = Nd4j.createUninitialized(dataType, toReduce.shape())
					Nd4j.Executioner.exec(New BroadcastAddOp(toReduce, negInfMask, withInf, dimensions))
					'At this point: all the masked out steps have value -inf, hence can't be the output of the MAX op

					Return withInf.max(2, 3)
				Case PoolingType.AVG, SUM
					Dim masked As INDArray = Nd4j.createUninitialized(dataType, toReduce.shape())
					Nd4j.Executioner.exec(New BroadcastMulOp(toReduce, mask, masked, dimensions))

					Dim summed As INDArray = masked.sum(2, 3)
					If poolingType = PoolingType.SUM Then
						Return summed
					End If
					Dim maskCounts As INDArray = mask.sum(1,2,3)
					summed.diviColumnVector(maskCounts)
					Return summed

				Case PoolingType.PNORM
					'Similar to average and sum pooling: there's no N term here, so we can just set the masked values to 0
					Dim masked2 As INDArray = Nd4j.createUninitialized(dataType, toReduce.shape())
					Nd4j.Executioner.exec(New BroadcastMulOp(toReduce, mask, masked2, dimensions))

					Dim abs As INDArray = Transforms.abs(masked2, True)
					Transforms.pow(abs, pnorm, False)
'JAVA TO VB CONVERTER NOTE: The variable pNorm was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
					Dim MaskedReductionUtil.pNorm_Conflict As INDArray = abs.sum(2, 3)

					Return Transforms.pow(MaskedReductionUtil.pNorm_Conflict, 1.0 / pnorm)
				Case Else
					Throw New System.NotSupportedException("Unknown or not supported pooling type: " & poolingType)
			End Select
		End Function


		Public Shared Function maskedPoolingEpsilonCnn(ByVal poolingType As PoolingType, ByVal input As INDArray, ByVal mask As INDArray, ByVal epsilon2d As INDArray, ByVal pnorm As Integer, ByVal dataType As DataType) As INDArray

			' [minibatch, channels, h=1, w=X] or [minibatch, channels, h=X, w=1] data
			' with a mask array of shape [minibatch, X]

			'If masking along height: broadcast dimensions are [0,2]
			'If masking along width: broadcast dimensions are [0,3]

			mask = mask.castTo(dataType) 'No-op if correct type

			'General case: must be equal or 1 on each dimension
			Dim dimensions(3) As Integer
			Dim count As Integer = 0
			For i As Integer = 0 To 3
				If input.size(i) = mask.size(i) Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: dimensions[count++] = i;
					dimensions(count) = i
						count += 1
				End If
			Next i
			If count < 4 Then
				dimensions = Arrays.CopyOfRange(dimensions, 0, count)
			End If

			Select Case poolingType
				Case PoolingType.MAX
					'TODO This is ugly - replace it with something better... Need something like a Broadcast CAS op
					Dim negInfMask As INDArray
					If mask.dataType() = DataType.BOOL Then
						negInfMask = Transforms.not(mask).castTo(dataType)
					Else
						negInfMask = mask.rsub(1.0)
					End If
					BooleanIndexing.replaceWhere(negInfMask, Double.NegativeInfinity, Conditions.equals(1.0))

					Dim withInf As INDArray = Nd4j.createUninitialized(dataType, input.shape())
					Nd4j.Executioner.exec(New BroadcastAddOp(input, negInfMask, withInf, dimensions))
					'At this point: all the masked out steps have value -inf, hence can't be the output of the MAX op

					Dim isMax As INDArray = Nd4j.exec(New IsMax(withInf, withInf.ulike(), 2, 3))(0)

					Return Nd4j.Executioner.exec(New BroadcastMulOp(isMax, epsilon2d, isMax, 0, 1))
				Case PoolingType.AVG, SUM
					'if out = sum(in,dims) then dL/dIn = dL/dOut -> duplicate to each step and mask
					'if out = avg(in,dims) then dL/dIn = 1/N * dL/dOut
					'With masking: N differs for different time series

					Dim [out] As INDArray = Nd4j.createUninitialized(dataType, input.shape(), "f"c)

					'Broadcast copy op, then divide and mask to 0 as appropriate
					Nd4j.Executioner.exec(New BroadcastCopyOp([out], epsilon2d, [out], 0, 1))
					Nd4j.Executioner.exec(New BroadcastMulOp([out], mask, [out], dimensions))

					If poolingType = PoolingType.SUM Then
						Return [out]
					End If

					'Note that with CNNs, current design is restricted to [minibatch, channels, 1, W] ot [minibatch, channels, H, 1]
					Dim nEachTimeSeries As INDArray = mask.sum(1,2,3) '[minibatchSize,tsLength] -> [minibatchSize,1]
					Nd4j.Executioner.exec(New BroadcastDivOp([out], nEachTimeSeries, [out], 0))

					Return [out]

				Case PoolingType.PNORM
					'Similar to average and sum pooling: there's no N term here, so we can just set the masked values to 0
					Dim masked2 As INDArray = Nd4j.createUninitialized(dataType, input.shape())
					Nd4j.Executioner.exec(New BroadcastMulOp(input, mask, masked2, dimensions))

					Dim abs As INDArray = Transforms.abs(masked2, True)
					Transforms.pow(abs, pnorm, False)
'JAVA TO VB CONVERTER NOTE: The variable pNorm was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
					Dim MaskedReductionUtil.pNorm_Conflict As INDArray = Transforms.pow(abs.sum(2, 3), 1.0 / pnorm)

					Dim numerator As INDArray
					If pnorm = 2 Then
						numerator = input.dup()
					Else
						Dim absp2 As INDArray = Transforms.pow(Transforms.abs(input, True), pnorm - 2, False)
						numerator = input.mul(absp2)
					End If

					Dim denom As INDArray = Transforms.pow(MaskedReductionUtil.pNorm_Conflict, pnorm - 1, False)
					denom.rdivi(epsilon2d)
					Nd4j.Executioner.execAndReturn(New BroadcastMulOp(numerator, denom, numerator, 0, 1))
					Nd4j.Executioner.exec(New BroadcastMulOp(numerator, mask, numerator, dimensions)) 'Apply mask

					Return numerator
				Case Else
					Throw New System.NotSupportedException("Unknown or not supported pooling type: " & poolingType)

			End Select
		End Function
	End Class

End Namespace