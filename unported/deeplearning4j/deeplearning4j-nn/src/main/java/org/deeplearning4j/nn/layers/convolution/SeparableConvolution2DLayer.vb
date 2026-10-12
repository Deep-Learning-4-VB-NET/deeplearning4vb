Imports System
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports SeparableConvolutionParamInitializer = org.deeplearning4j.nn.params.SeparableConvolutionParamInitializer
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.layers.convolution


	<Serializable>
	Public Class SeparableConvolution2DLayer
		Inherits ConvolutionLayer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub


		Friend Overrides Sub initializeHelper()
			'No op - no separable conv implementation in cudnn
		End Sub


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			If input_Conflict.rank() <> 4 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to SubsamplingLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 4 array with shape " & layerConf().getCnn2dDataFormat().dimensionNames() & ". " & layerId())
			End If
			Dim bias As INDArray
			Dim depthWiseWeights As INDArray = getParamWithNoise(SeparableConvolutionParamInitializer.DEPTH_WISE_WEIGHT_KEY, True, workspaceMgr)
			Dim pointWiseWeights As INDArray = getParamWithNoise(SeparableConvolutionParamInitializer.POINT_WISE_WEIGHT_KEY, True, workspaceMgr)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)

			Dim format As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW

			Dim miniBatch As Long = input.size(0)
			Dim inH As Integer = CInt(input.size(If(nchw, 2, 1)))
			Dim inW As Integer = CInt(input.size(If(nchw, 3, 2)))

			Dim inDepth As Integer = CInt(depthWiseWeights.size(1))
			Dim kH As Integer = CInt(depthWiseWeights.size(2))
			Dim kW As Integer = CInt(depthWiseWeights.size(3))

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim pad() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				Dim outSize() As Integer = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, format) 'Also performs validation
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {inH, inW}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, format) 'Also performs validation
			End If

			Dim biasGradView As INDArray = gradientViews(SeparableConvolutionParamInitializer.BIAS_KEY)
			Dim depthWiseWeightGradView As INDArray = gradientViews(SeparableConvolutionParamInitializer.DEPTH_WISE_WEIGHT_KEY)
			Dim pointWiseWeightGradView As INDArray = gradientViews(SeparableConvolutionParamInitializer.POINT_WISE_WEIGHT_KEY)

			Dim epsShape() As Long = If(nchw, New Long(){miniBatch, inDepth, inH, inW}, New Long()){miniBatch, inH, inW, inDepth}
			Dim outEpsilon As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, depthWiseWeights.dataType(), epsShape, "c"c)

			Dim sameMode As Integer = If(convolutionMode = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = {kH, kW, strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), sameMode, If(nchw, 0, 1)}

			Dim delta As INDArray
			Dim afn As IActivation = layerConf().getActivationFn()
			Dim p As Pair(Of INDArray, INDArray) = preOutput4d(True, True, workspaceMgr)
			delta = afn.backprop(p.First, epsilon).First

			'dl4j weights: depth [depthMultiplier, nIn, kH, kW], point [nOut, nIn * depthMultiplier, 1, 1]
			'libnd4j weights: depth [kH, kW, iC, mC], point [1, 1, iC*mC, oC]
			depthWiseWeights = depthWiseWeights.permute(2, 3, 1, 0)
			pointWiseWeights = pointWiseWeights.permute(2, 3, 1, 0)
			Dim opDepthWiseWeightGradView As INDArray = depthWiseWeightGradView.permute(2, 3, 1, 0)
			Dim opPointWiseWeightGradView As INDArray = pointWiseWeightGradView.permute(2, 3, 1, 0)

			Dim op As CustomOp
			If layerConf().hasBias() Then
				bias = getParamWithNoise(SeparableConvolutionParamInitializer.BIAS_KEY, True, workspaceMgr)

				op = DynamicCustomOp.builder("sconv2d_bp").addInputs(input, delta, depthWiseWeights, pointWiseWeights, bias).addIntegerArguments(args).addOutputs(outEpsilon, opDepthWiseWeightGradView, opPointWiseWeightGradView, biasGradView).callInplace(False).build()
			Else
				op = DynamicCustomOp.builder("sconv2d_bp").addInputs(input, delta, depthWiseWeights, pointWiseWeights).addIntegerArguments(args).addOutputs(outEpsilon, opDepthWiseWeightGradView, opPointWiseWeightGradView).callInplace(False).build()
			End If
			Nd4j.Executioner.exec(op)

			Dim retGradient As Gradient = New DefaultGradient()
			If layerConf().hasBias() Then
				retGradient.setGradientFor(ConvolutionParamInitializer.BIAS_KEY, biasGradView)
			End If
			retGradient.setGradientFor(SeparableConvolutionParamInitializer.DEPTH_WISE_WEIGHT_KEY, depthWiseWeightGradView, "c"c)
			retGradient.setGradientFor(SeparableConvolutionParamInitializer.POINT_WISE_WEIGHT_KEY, pointWiseWeightGradView, "c"c)

			weightNoiseParams.Clear()

			outEpsilon = backpropDropOutIfPresent(outEpsilon)
			Return New Pair(Of Gradient, INDArray)(retGradient, outEpsilon)
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)
			assertInputSet(False)
			Dim bias As INDArray = getParamWithNoise(SeparableConvolutionParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim depthWiseWeights As INDArray = getParamWithNoise(SeparableConvolutionParamInitializer.DEPTH_WISE_WEIGHT_KEY, training, workspaceMgr)
			Dim pointWiseWeights As INDArray = getParamWithNoise(SeparableConvolutionParamInitializer.POINT_WISE_WEIGHT_KEY, training, workspaceMgr)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)
			If layerConf().getCnn2dDataFormat() = CNN2DFormat.NHWC Then
				input = input.permute(0,3,1,2).dup()
			End If

			Dim chIdx As Integer = 1
			Dim hIdx As Integer = 2
			Dim wIdx As Integer = 3

			If input.rank() <> 4 Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If
				Throw New DL4JInvalidInputException("Got rank " & input.rank() & " array as input to SeparableConvolution2D (layer name = " & layerName & ", layer index = " & index_Conflict & ") with shape " & Arrays.toString(input.shape()) & ". " & "Expected rank 4 array with shape " & layerConf().getCnn2dDataFormat().dimensionNames() & "." & (If(input.rank() = 2, " (Wrong input type (see InputType.convolutionalFlat()) or wrong data type?)", "")) & " " & layerId())
			End If

			Dim inDepth As Long = depthWiseWeights.size(1)
			Dim outDepth As Long = pointWiseWeights.size(0)

			If input.size(chIdx) <> inDepth Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If

				Dim s As String = "Cannot do forward pass in SeparableConvolution2D layer (layer name = " & layerName & ", layer index = " & index_Conflict & "): input array channels does not match CNN layer configuration" & " (data format = " & layerConf().getCnn2dDataFormat() & ", data input channels = " & input.size(1) & ", [minibatch,inputDepth,height,width]=" & Arrays.toString(input.shape()) & "; expected" & " input channels = " & inDepth & ") " & layerId()

				Dim dimIfWrongFormat As Integer = 1
				If input.size(dimIfWrongFormat) = inDepth Then
					'User might have passed NCHW data to a NHWC net, or vice versa?
					s &= vbLf & ConvolutionUtils.NCHW_NHWC_ERROR_MSG
				End If

				Throw New DL4JInvalidInputException(s)
			End If
			Dim kH As Integer = CInt(depthWiseWeights.size(2))
			Dim kW As Integer = CInt(depthWiseWeights.size(3))

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()

			Dim pad() As Integer
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, CNN2DFormat.NCHW) 'Also performs validation, note: hardcoded due to above permute

				If input.size(2) > Integer.MaxValue OrElse input.size(3) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {CInt(input.size(hIdx)), CInt(input.size(wIdx))}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, CNN2DFormat.NCHW) 'Also performs validation, note hardcoded due to permute above
			End If

			Dim outH As Integer = outSize(0)
			Dim outW As Integer = outSize(1)

			Dim miniBatch As val = input.size(0)
			Dim outShape() As Long = {miniBatch, outDepth, outH, outW}
			Dim output As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, depthWiseWeights.dataType(), outShape, "c"c)

			Dim sameMode As Integer? = If(convolutionMode = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = { kH, kW, strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), sameMode, 0 }

			'dl4j weights: depth [depthMultiplier, nIn, kH, kW], point [nOut, nIn * depthMultiplier, 1, 1]
			'libnd4j weights: depth [kH, kW, iC, mC], point [1, 1, iC*mC, oC]
			depthWiseWeights = depthWiseWeights.permute(2, 3, 1, 0)
			pointWiseWeights = pointWiseWeights.permute(2, 3, 1, 0)

			Dim opInputs() As INDArray
			If layerConf().hasBias() Then
				opInputs = New INDArray(){input, depthWiseWeights, pointWiseWeights, bias}
			Else
				opInputs = New INDArray(){input, depthWiseWeights, pointWiseWeights}

			End If

			Dim op As CustomOp = DynamicCustomOp.builder("sconv2d").addInputs(opInputs).addIntegerArguments(args).addOutputs(output).callInplace(False).build()
			Nd4j.Executioner.exec(op)

			If layerConf().getCnn2dDataFormat() = CNN2DFormat.NHWC Then
				output = output.permute(0,2,3,1) 'NCHW to NHWC

			End If
			Return New Pair(Of INDArray, INDArray)(output, Nothing)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)

			If cacheMode_Conflict = Nothing Then
				cacheMode_Conflict = CacheMode.NONE
			End If

			applyDropOutIfNecessary(training, workspaceMgr)

			Dim z As INDArray = preOutput(training, False, workspaceMgr).First

			'String afn = conf.getLayer().getActivationFunction();
			Dim afn As IActivation = layerConf().getActivationFn()

			Dim activation As INDArray = afn.getActivation(z, training)
			Return activation
		End Function
	End Class

End Namespace