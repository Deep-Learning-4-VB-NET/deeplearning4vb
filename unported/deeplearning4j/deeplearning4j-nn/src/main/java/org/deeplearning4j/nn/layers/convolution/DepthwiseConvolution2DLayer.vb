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
Imports DepthwiseConvolutionParamInitializer = org.deeplearning4j.nn.params.DepthwiseConvolutionParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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
	Public Class DepthwiseConvolution2DLayer
		Inherits ConvolutionLayer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub


		Friend Overrides Sub initializeHelper()
			'No op - no separable conv implementation in cudnn
		End Sub


		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim format As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			If input_Conflict.rank() <> 4 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Convolution layer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 4 array with shape " & layerConf().getCnn2dDataFormat().dimensionNames() & ". " & layerId())
			End If
			Dim bias As INDArray
			Dim depthWiseWeights As INDArray = getParamWithNoise(DepthwiseConvolutionParamInitializer.WEIGHT_KEY, True, workspaceMgr)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'No-op if correct type

			Dim miniBatch As Long = input.size(0)
			Dim inH As Integer = CInt(input.size(If(nchw, 2, 1)))
			Dim inW As Integer = CInt(input.size(If(nchw, 3, 2)))

			Dim inDepth As Long = depthWiseWeights.size(2)
			Dim kH As Integer = CInt(depthWiseWeights.size(0))
			Dim kW As Integer = CInt(depthWiseWeights.size(1))

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim pad() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				Dim outSize() As Integer = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, format)
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer(){inH, inW}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, format)
			End If

			Dim biasGradView As INDArray = gradientViews(DepthwiseConvolutionParamInitializer.BIAS_KEY)
			Dim weightGradView As INDArray = gradientViews(DepthwiseConvolutionParamInitializer.WEIGHT_KEY)

			Dim epsShape() As Long = If(nchw, New Long(){miniBatch, inDepth, inH, inW}, New Long()){miniBatch, inH, inW, inDepth}
			Dim outEpsilon As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, depthWiseWeights.dataType(), epsShape, "c"c)

			Dim sameMode As Integer = If(convolutionMode = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = { kH, kW, strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), sameMode, (If(nchw, 0, 1)) }

			Dim delta As INDArray
			Dim afn As IActivation = layerConf().getActivationFn()
			Dim p As Pair(Of INDArray, INDArray) = preOutput4d(True, True, workspaceMgr)
			delta = afn.backprop(p.First, epsilon).First

			Dim inputs() As INDArray
			Dim outputs() As INDArray
			If layerConf().hasBias() Then
				bias = getParamWithNoise(DepthwiseConvolutionParamInitializer.BIAS_KEY, True, workspaceMgr)
				inputs = New INDArray(){input, depthWiseWeights, bias, delta}
				outputs = New INDArray(){outEpsilon, weightGradView, biasGradView}
			Else
				inputs = New INDArray(){input, depthWiseWeights, delta}
				outputs = New INDArray(){outEpsilon, weightGradView}
			End If

			Dim op As CustomOp = DynamicCustomOp.builder("depthwise_conv2d_bp").addInputs(inputs).addIntegerArguments(args).addOutputs(outputs).callInplace(False).build()
			Nd4j.Executioner.exec(op)

			Dim retGradient As Gradient = New DefaultGradient()
			If layerConf().hasBias() Then
				retGradient.setGradientFor(DepthwiseConvolutionParamInitializer.BIAS_KEY, biasGradView)
			End If
			retGradient.setGradientFor(DepthwiseConvolutionParamInitializer.WEIGHT_KEY, weightGradView, "c"c)

			weightNoiseParams.Clear()

			outEpsilon = backpropDropOutIfPresent(outEpsilon)
			Return New Pair(Of Gradient, INDArray)(retGradient, outEpsilon)
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)
			assertInputSet(False)
			Dim bias As INDArray = getParamWithNoise(DepthwiseConvolutionParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim depthWiseWeights As INDArray = getParamWithNoise(DepthwiseConvolutionParamInitializer.WEIGHT_KEY, training, workspaceMgr)

			If input_Conflict.rank() <> 4 Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to DepthwiseConvolution2D (layer name = " & layerName & ", layer index = " & index_Conflict & ") with shape " & Arrays.toString(input_Conflict.shape()) & ". " & "Expected rank 4 array with shape " & layerConf().getCnn2dDataFormat().dimensionNames() & "." & (If(input_Conflict.rank() = 2, " (Wrong input type (see InputType.convolutionalFlat()) or wrong data type?)", "")) & " " & layerId())
			End If

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'no-op if correct dtype

			Dim format As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW

			Dim inDepth As Long = depthWiseWeights.size(2)
			Dim depthMultiplier As Long = depthWiseWeights.size(3)
			Dim outDepth As Long = depthMultiplier * inDepth

			If input.size(If(nchw, 1, 3)) <> inDepth Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If

				Dim s As String = "Cannot do forward pass in DepthwiseConvolution2D layer " & "(layer name = " & layerName & ", layer index = " & index_Conflict & "): input array channels does not match CNN layer configuration" & " (data format = " & format & ", data input channels = " & input.size(1) & ", " & (If(nchw, "[minibatch,inputDepth,height,width]=", "[minibatch,height,width,inputDepth]=")) & Arrays.toString(input.shape()) & "; expected" & " input channels = " & inDepth & ") " & layerId()
				Dim dimIfWrongFormat As Integer = If(format = CNN2DFormat.NHWC, 1, 3)
				If input.size(dimIfWrongFormat) = inDepth Then
					'User might have passed NCHW data to a NHWC net, or vice versa?
					s &= vbLf & ConvolutionUtils.NCHW_NHWC_ERROR_MSG
				End If

				Throw New DL4JInvalidInputException(s)
			End If
			Dim kH As Integer = CInt(depthWiseWeights.size(0))
			Dim kW As Integer = CInt(depthWiseWeights.size(1))

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()

			Dim pad() As Integer
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, format)

				If input.size(2) > Integer.MaxValue OrElse input.size(3) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer(){CInt(input.size(If(nchw, 2, 1))), CInt(input.size(If(nchw, 3, 2)))}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, format)
			End If

			Dim outH As Long = outSize(0)
			Dim outW As Long = outSize(1)

			Dim miniBatch As val = input.size(0)
			Dim outShape() As Long = If(nchw, New Long(){miniBatch, outDepth, outH, outW}, New Long()){miniBatch, outH, outW, outDepth}
			Dim output As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, depthWiseWeights.dataType(), outShape, "c"c)

			Dim sameMode As Integer = If(convolutionMode = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = { kH, kW, strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), sameMode, (If(nchw, 0, 1)) }

			Dim inputs() As INDArray
			If layerConf().hasBias() Then
				inputs = New INDArray(){input, depthWiseWeights, bias}
			Else
				inputs = New INDArray(){input, depthWiseWeights}

			End If
			Dim op As CustomOp = DynamicCustomOp.builder("depthwise_conv2d").addInputs(inputs).addIntegerArguments(args).addOutputs(output).callInplace(False).build()
			Nd4j.Executioner.exec(op)

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