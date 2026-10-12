Imports System
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports Convolution3DParamInitializer = org.deeplearning4j.nn.params.Convolution3DParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Convolution3DUtils = org.deeplearning4j.util.Convolution3DUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
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
	Public Class Convolution3DLayer
		Inherits ConvolutionLayer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub


		Friend Overrides Sub initializeHelper()
			' no op
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)

			If input.rank() <> 5 Then
				Throw New DL4JInvalidInputException("Got rank " & input.rank() & " array as input to SubsamplingLayer with shape " & Arrays.toString(input.shape()) & ". Expected rank 5 array with shape [minibatchSize, channels, " & "inputHeight, inputWidth, inputDepth]. " & layerId())
			End If

			Dim input As INDArray = Me.input.castTo(dataType)
			Dim weights As INDArray = getParamWithNoise(Convolution3DParamInitializer.WEIGHT_KEY, True, workspaceMgr)

			Dim layerConfig As Convolution3D = CType(layerConf(), Convolution3D)

			Dim isNCDHW As Boolean = layerConfig.getDataFormat() = Convolution3D.DataFormat.NCDHW

			Dim miniBatch As Long = input.size(0)
			Dim inD As Integer = CInt(If(isNCDHW, input.size(2), input.size(1)))
			Dim inH As Integer = CInt(If(isNCDHW, input.size(3), input.size(2)))
			Dim inW As Integer = CInt(If(isNCDHW, input.size(4), input.size(3)))

			Dim outEpsChannels As Integer = CInt(Math.Truncate(layerConf().getNIn()))

			Dim dilation() As Integer = layerConfig.getDilation()
			Dim kernel() As Integer = layerConfig.getKernelSize()
			Dim strides() As Integer = layerConfig.getStride()
			Dim pad() As Integer
			Dim outSize() As Integer

			If convolutionMode = ConvolutionMode.Same Then
				outSize = Convolution3DUtils.get3DOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, isNCDHW)
				pad = Convolution3DUtils.get3DSameModeTopLeftPadding(outSize, New Integer(){inD, inH, inW}, kernel, strides, dilation)
			Else
				pad = layerConfig.getPadding()
			End If

			Dim weightGradView As INDArray = gradientViews.get(Convolution3DParamInitializer.WEIGHT_KEY)

			Dim outEpsilon As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, weights.dataType(), miniBatch * outEpsChannels * inD * inH * inW)
			If isNCDHW Then
				outEpsilon = outEpsilon.reshape("c"c, miniBatch, outEpsChannels, inD, inH, inW)
			Else
				outEpsilon = outEpsilon.reshape("c"c, miniBatch, inD, inH, inW, outEpsChannels)
			End If


			Dim intArgs() As Integer = {kernel(0), kernel(1), kernel(2), strides(0), strides(1), strides(2), pad(0), pad(1), pad(2), dilation(0), dilation(1), dilation(2), If(convolutionMode = ConvolutionMode.Same, 1, 0), If(isNCDHW, 0, 1)}

			Dim delta As INDArray
			Dim activation As IActivation = layerConfig.getActivationFn()
			Dim p As Pair(Of INDArray, INDArray) = preOutput(True, True, workspaceMgr)

			delta = activation.backprop(p.First, epsilon).First

			Dim bias As INDArray
			Dim biasGradView As INDArray = Nothing

			'DL4J conv3d weights: val weightsShape = new long[]{outputDepth, inputDepth, kernel[0], kernel[1], kernel[2]};
			'libnd4j conv3d weights: [kD, kH, kW, iC, oC]
			weights = weights.permute(2, 3, 4, 1, 0)
			Dim opWeightGradView As INDArray = weightGradView.permute(2, 3, 4, 1, 0)

			Dim inputs() As INDArray
			Dim outputs() As INDArray
			If layerConfig.hasBias() Then
				biasGradView = gradientViews.get(Convolution3DParamInitializer.BIAS_KEY)
				bias = getParamWithNoise(Convolution3DParamInitializer.BIAS_KEY, True, workspaceMgr)
				inputs = New INDArray(){input, weights, bias, delta}
				outputs = New INDArray(){outEpsilon, opWeightGradView, biasGradView}
			Else
				inputs = New INDArray(){input, weights, delta}
				outputs = New INDArray(){outEpsilon, opWeightGradView}
			End If

			Dim op As CustomOp = DynamicCustomOp.builder("conv3dnew_bp").addInputs(inputs).addIntegerArguments(intArgs).addOutputs(outputs).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			Dim retGradient As Gradient = New DefaultGradient()
			If layerConfig.hasBias() Then
				retGradient.setGradientFor(Convolution3DParamInitializer.BIAS_KEY, biasGradView)
			End If
			retGradient.setGradientFor(Convolution3DParamInitializer.WEIGHT_KEY, weightGradView, "c"c)
			weightNoiseParams.clear()

			Return New Pair(Of Gradient, INDArray)(retGradient, outEpsilon)
		End Function


		Public Overrides Function preOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return preOutput(training, False, workspaceMgr).First
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)

			Dim layerConfig As Convolution3D = CType(layerConf(), Convolution3D)

			Dim mode As ConvolutionMode = layerConfig.getConvolutionMode()
			Dim isNCDHW As Boolean = layerConfig.getDataFormat() = Convolution3D.DataFormat.NCDHW

			Dim input As INDArray = Me.input.castTo(dataType)
			Dim weights As INDArray = getParamWithNoise(Convolution3DParamInitializer.WEIGHT_KEY, training, workspaceMgr)

			If input.rank() <> 5 Then
				Dim layerName As String = conf.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If
				Throw New DL4JInvalidInputException("Got rank " & input.rank() & " array as input to Convolution3DLayer (layer name = " & layerName & ", layer index = " & index & ") with shape " & Arrays.toString(input.shape()) & ". " & "Expected rank 5 array with shape [minibatchSize, numChannels, inputHeight, " & "inputWidth, inputDepth]." & (If(input.rank() = 2, " (Wrong input type (see InputType.convolutionalFlat()) or wrong data type?)", "")) & " " & layerId())
			End If

			Dim miniBatch As Long = input.size(0)
			Dim inputChannels As Integer = CInt(If(isNCDHW, input.size(1), input.size(4)))
			Dim inD As Integer =CInt(If(isNCDHW, input.size(2), input.size(1)))
			Dim inH As Integer = CInt(If(isNCDHW, input.size(3), input.size(2)))
			Dim inW As Integer = CInt(If(isNCDHW, input.size(4), input.size(3)))

			Dim outWeightChannels As Integer = CInt(Math.Truncate(layerConf().getNOut()))
			Dim inWeightChannels As Integer = CInt(Math.Truncate(layerConf().getNIn()))

			If inputChannels <> inWeightChannels Then
				Dim layerName As String = conf.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If
				Dim dataInCh As Long = If(isNCDHW, input.size(1), input.size(4))
				Dim df As String
				If isNCDHW Then
					df = ", dataFormat=NCDHW, [minibatch, inputChannels, depth, height, width]="
				Else
					df = ", dataFormat=NDHWC, [minibatch, depth, height, width, inputChannels]="
				End If
				Throw New DL4JInvalidInputException("Cannot do forward pass in Convolution3D layer (layer name = " & layerName & ", layer index = " & index & "): number of input array channels does not match " & "CNN layer configuration" & " (data input channels = " & dataInCh + df & Arrays.toString(input.shape()) & "; expected" & " input channels = " & inWeightChannels & ") " & layerId())
			End If


			Dim kernel() As Integer = layerConfig.getKernelSize()
			Dim dilation() As Integer = layerConfig.getDilation()
			Dim strides() As Integer = layerConfig.getStride()

			Dim pad() As Integer
			Dim outSize() As Integer
			If mode = ConvolutionMode.Same Then
				outSize = Convolution3DUtils.get3DOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, isNCDHW)
				Dim inSize() As Integer = {inD, inH, inW}
				pad = Convolution3DUtils.get3DSameModeTopLeftPadding(outSize, inSize, kernel, strides, dilation)
			Else
				pad = layerConfig.getPadding()
				outSize = Convolution3DUtils.get3DOutputSize(input, kernel, strides, pad, convolutionMode, dilation, isNCDHW)
			End If
			Dim outD As Integer = outSize(0)
			Dim outH As Integer = outSize(1)
			Dim outW As Integer = outSize(2)

			Dim output As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, weights.dataType(),miniBatch*outWeightChannels*outD*outH*outW)
			If isNCDHW Then
				output = output.reshape("c"c, miniBatch, outWeightChannels, outD, outH, outW)
			Else
				output = output.reshape("c"c, miniBatch, outD, outH, outW, outWeightChannels)
			End If

			Dim intArgs() As Integer = {kernel(0), kernel(1), kernel(2), strides(0), strides(1), strides(2), pad(0), pad(1), pad(2), dilation(0), dilation(1), dilation(2), If(mode = ConvolutionMode.Same, 1, 0), If(isNCDHW, 0, 1)}

			'DL4J conv3d weights: val weightsShape = new long[]{outputDepth, inputDepth, kernel[0], kernel[1], kernel[2]};
			'libnd4j conv3d weights: [kD, kH, kW, iC, oC]
			weights = weights.permute(2, 3, 4, 1, 0)

			Dim inputs() As INDArray
			If layerConfig.hasBias() Then
				Dim bias As INDArray = getParamWithNoise(Convolution3DParamInitializer.BIAS_KEY, training, workspaceMgr)
				inputs = New INDArray(){input, weights, bias}
			Else
				inputs = New INDArray(){input, weights}
			End If

			Dim op As CustomOp = DynamicCustomOp.builder("conv3dnew").addInputs(inputs).addIntegerArguments(intArgs).addOutputs(output).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			Return New Pair(Of INDArray, INDArray)(output, Nothing)
		End Function
	End Class
End Namespace