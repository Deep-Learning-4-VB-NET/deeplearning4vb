Imports System
Imports val = lombok.val
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports MaskedReductionUtil = org.deeplearning4j.util.MaskedReductionUtil
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastCopyOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastCopyOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports IsMax = org.nd4j.linalg.api.ops.impl.transforms.any.IsMax
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType

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

Namespace org.deeplearning4j.nn.layers.pooling


	<Serializable>
	Public Class GlobalPoolingLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer)

		Private Shared ReadOnly DEFAULT_TIMESERIES_POOL_DIMS() As Integer = {2}
		Private Shared ReadOnly DEFAULT_CNN_POOL_DIMS() As Integer = {2, 3}
		Private Shared ReadOnly DEFAULT_CNN3D_POOL_DIMS() As Integer = {2, 3, 4}


		Private ReadOnly poolingDimensions() As Integer
		Private ReadOnly poolingType As PoolingType
		Private ReadOnly pNorm As Integer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)

			Dim layerConf As org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.GlobalPoolingLayer)

			poolingDimensions = layerConf.getPoolingDimensions()
			poolingType = layerConf.getPoolingType()
			pNorm = layerConf.getPnorm()
		End Sub

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'No op
		End Sub

		Public Overrides Function type() As Type
			Return Type.SUBSAMPLING
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)

			Dim poolDim() As Integer
			If input_Conflict.rank() = 3 Then
				'TODO validation on pooling dimensions

				If poolingDimensions Is Nothing Then
					'Use default pooling dimensions;
					poolDim = DEFAULT_TIMESERIES_POOL_DIMS
				Else
					poolDim = poolingDimensions
				End If

			ElseIf input_Conflict.rank() = 4 Then
				'CNN activations
				If poolingDimensions Is Nothing Then
					'Use default pooling dimensions;
					poolDim = DEFAULT_CNN_POOL_DIMS
				Else
					poolDim = poolingDimensions
				End If
			ElseIf input_Conflict.rank() = 5 Then
				'CNN3D activations
				If poolingDimensions Is Nothing Then
					'Use default pooling dimensions;
					poolDim = DEFAULT_CNN3D_POOL_DIMS
				Else
					poolDim = poolingDimensions
				End If
			Else
				Throw New System.NotSupportedException("Received rank " & input_Conflict.rank() & " input (shape = " & Arrays.toString(input_Conflict.shape()) & "). Only rank 3 (time series), rank 4 (images" & "/CNN data) and rank 5 (volumetric / CNN3D data)  are currently supported for " & "global pooling " & layerId())
			End If

			' TODO: masking for CNN3D case
			Dim reduced2d As INDArray
			If maskArray_Conflict Is Nothing Then
				'Standard 'full array' global pooling op
				reduced2d = activateHelperFullArray(input_Conflict, poolDim)
			Else
				If input_Conflict.rank() = 3 Then
					'Masked time series

					reduced2d = MaskedReductionUtil.maskedPoolingTimeSeries(poolingType, input_Conflict, maskArray_Conflict, pNorm, dataType)
				ElseIf input_Conflict.rank() = 4 Then
					'Masked convolutions. 4d convolution data, shape [minibatch, channels, h, w]
					'and 2d mask array.
					'Because of this: for now we'll support *masked* CNN global pooling on either
					' [minibatch, channels, 1, X] or [minibatch, channels, X, 1] data
					' with a mask array of shape [minibatch, X]

					If maskArray_Conflict.rank() <> 4 Then
						Throw New System.NotSupportedException("Only 4d mask arrays are currently supported for masked global reductions " & "on CNN data. Got 4d activations array (shape " & Arrays.toString(input_Conflict.shape()) & ") and " & maskArray_Conflict.rank() & "d mask array (shape " & Arrays.toString(maskArray_Conflict.shape()) & ") " & " - when used in conjunction with input data of shape [batch,channels,h,w]=" & Arrays.toString(input_Conflict.shape()) & " 4d masks should have shape [batchSize,1,h,1] or [batchSize,1,w,1] or [batchSize,1,h,w]" & layerId())
					End If

					reduced2d = MaskedReductionUtil.maskedPoolingConvolution(poolingType, input_Conflict, maskArray_Conflict, pNorm, dataType)
				Else
					Throw New System.NotSupportedException("Invalid input: is rank " & input_Conflict.rank() & " " & layerId())
				End If
			End If

			'TODO optimize without leverage
			If layerConf().isCollapseDimensions() Then
				'Standard/common case
				Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, reduced2d)
			Else
				Dim inputShape As val = input_Conflict.shape()
				If input_Conflict.rank() = 3 Then
					Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, reduced2d.reshape(reduced2d.ordering(), inputShape(0), inputShape(1), 1))
				ElseIf input_Conflict.rank() = 4 Then
					Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, reduced2d.reshape(reduced2d.ordering(), inputShape(0), inputShape(1), 1, 1))
				Else
					Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, reduced2d.reshape(reduced2d.ordering(), inputShape(0), inputShape(1), 1, 1, 1))
				End If
			End If
		End Function

		Public Overrides Function clone() As Layer
			Return New GlobalPoolingLayer(conf_Conflict, dataType)
		End Function

		Private Function activateHelperFullArray(ByVal inputArray As INDArray, ByVal poolDim() As Integer) As INDArray
			Select Case poolingType
				Case PoolingType.MAX
					Return inputArray.max(poolDim)
				Case PoolingType.AVG
					Return inputArray.mean(poolDim)
				Case PoolingType.SUM
					Return inputArray.sum(poolDim)
				Case PoolingType.PNORM
					'P norm: https://arxiv.org/pdf/1311.1780.pdf
					'out = (1/N * sum( |in| ^ p) ) ^ (1/p)
'JAVA TO VB CONVERTER NOTE: The variable pnorm was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim pnorm_Conflict As Integer = layerConf().getPnorm()

					Dim abs As INDArray = Transforms.abs(inputArray, True)
					Transforms.pow(abs, pnorm_Conflict, False)
'JAVA TO VB CONVERTER NOTE: The variable pNorm was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
					Dim pNorm_Conflict As INDArray = abs.sum(poolDim)

					Return Transforms.pow(pNorm_Conflict, 1.0 / pnorm_Conflict, False)
				Case Else
					Throw New Exception("Unknown or not supported pooling type: " & poolingType & " " & layerId())
			End Select
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			If Not layerConf().isCollapseDimensions() AndAlso epsilon.rank() <> 2 Then
				Dim origShape As val = epsilon.shape()
				'Don't collapse dims case: error should be [minibatch, vectorSize, 1] or [minibatch, channels, 1, 1]
				'Reshape it to 2d, to get rid of the 1s
				epsilon = epsilon.reshape(epsilon.ordering(), origShape(0), origShape(1))
			End If

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'No-op if already correct dtype

			Dim retGradient As Gradient = New DefaultGradient() 'Empty: no params

			Dim poolDim() As Integer = Nothing
			If input.rank() = 3 Then
				If poolingDimensions Is Nothing Then
					'Use default pooling dimensions;
					poolDim = DEFAULT_TIMESERIES_POOL_DIMS
				Else
					poolDim = poolingDimensions
				End If

			ElseIf input.rank() = 4 Then
				'CNN activations
				If poolingDimensions Is Nothing Then
					'Use default pooling dimensions;
					poolDim = DEFAULT_CNN_POOL_DIMS
				Else
					poolDim = poolingDimensions
				End If
			ElseIf input.rank() = 5 Then
				'CNN activations
				If poolingDimensions Is Nothing Then
					'Use default pooling dimensions;
					poolDim = DEFAULT_CNN3D_POOL_DIMS
				Else
					poolDim = poolingDimensions
				End If
			End If

			' TODO: masking for CNN3D case
			Dim epsilonNd As INDArray
			If maskArray_Conflict Is Nothing Then
				'Standard 'full array' global pooling op
				epsilonNd = epsilonHelperFullArray(input, epsilon, poolDim)
			Else
				If input.rank() = 3 Then
					epsilonNd = MaskedReductionUtil.maskedPoolingEpsilonTimeSeries(poolingType, input, maskArray_Conflict, epsilon, pNorm)
				ElseIf input.rank() = 4 Then
					epsilonNd = MaskedReductionUtil.maskedPoolingEpsilonCnn(poolingType, input, maskArray_Conflict, epsilon, pNorm, dataType)
				Else
					Throw New System.NotSupportedException(layerId())
				End If

			End If

			'TODO optimize without leverage
			epsilonNd = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilonNd)
			Return New Pair(Of Gradient, INDArray)(retGradient, epsilonNd)
		End Function

		Private Function epsilonHelperFullArray(ByVal inputArray As INDArray, ByVal epsilon As INDArray, ByVal poolDim() As Integer) As INDArray

			'Broadcast: occurs on the remaining dimensions, after the pool dimensions have been removed.
			'TODO find a more efficient way to do this
			Dim broadcastDims((inputArray.rank() - poolDim.Length) - 1) As Integer
			Dim count As Integer = 0
			Dim i As Integer = 0
			Do While i < inputArray.rank()
				If ArrayUtils.contains(poolDim, i) Then
					i += 1
					Continue Do
				End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: broadcastDims[count++] = i;
				broadcastDims(count) = i
					count += 1
				i += 1
			Loop

			Select Case poolingType
				Case PoolingType.MAX
					Dim isMax As INDArray = Nd4j.exec(New IsMax(inputArray, inputArray.ulike(), poolDim))(0)
					Return Nd4j.Executioner.exec(New BroadcastMulOp(isMax, epsilon, isMax, broadcastDims))
				Case PoolingType.AVG
					'if out = avg(in,dims) then dL/dIn = 1/N * dL/dOut
					Dim n As Integer = 1
					For Each d As Integer In poolDim
						n *= inputArray.size(d)
					Next d
					Dim ret As INDArray = inputArray.ulike()
					Nd4j.Executioner.exec(New BroadcastCopyOp(ret, epsilon, ret, broadcastDims))
					ret.divi(n)

					Return ret
				Case PoolingType.SUM
					Dim retSum As INDArray = inputArray.ulike()
					Nd4j.Executioner.exec(New BroadcastCopyOp(retSum, epsilon, retSum, broadcastDims))
					Return retSum
				Case PoolingType.PNORM
'JAVA TO VB CONVERTER NOTE: The variable pnorm was renamed since Visual Basic does not handle local variables named the same as class members well:
					Dim pnorm_Conflict As Integer = layerConf().getPnorm()

					'First: do forward pass to get pNorm array
					Dim abs As INDArray = Transforms.abs(inputArray, True)
					Transforms.pow(abs, pnorm_Conflict, False)

'JAVA TO VB CONVERTER NOTE: The variable pNorm was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
					Dim pNorm_Conflict As INDArray = Transforms.pow(abs.sum(poolDim), 1.0 / pnorm_Conflict)

					'dL/dIn = dL/dOut * dOut/dIn
					'dOut/dIn = in .* |in|^(p-2) /  ||in||_p^(p-1), where ||in||_p is the output p-norm

					Dim numerator As INDArray
					If pnorm_Conflict = 2 Then
						numerator = inputArray.dup()
					Else
						Dim absp2 As INDArray = Transforms.pow(Transforms.abs(inputArray, True), pnorm_Conflict - 2, False)
						numerator = inputArray.mul(absp2)
					End If

					Dim denom As INDArray = Transforms.pow(pNorm_Conflict, pnorm_Conflict - 1, False)
					'2 and 3d case
					If denom.rank() <> epsilon.rank() AndAlso denom.length() = epsilon.length() Then
						denom = denom.reshape(epsilon.shape())
					End If
					denom.rdivi(epsilon)
					Nd4j.Executioner.execAndReturn(New BroadcastMulOp(numerator, denom, numerator, broadcastDims))

					Return numerator
				Case Else
					Throw New Exception("Unknown or not supported pooling type: " & poolingType & " " & layerId())
			End Select
		End Function

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'Global pooling layer: no masking is possible after this point... i.e., masks have been taken into account
			' as part of the pooling
			Me.maskArray_Conflict = maskArray
			Me.maskState = Nothing 'Not used in global pooling - always applied

			Return Nothing
		End Function
	End Class

End Namespace