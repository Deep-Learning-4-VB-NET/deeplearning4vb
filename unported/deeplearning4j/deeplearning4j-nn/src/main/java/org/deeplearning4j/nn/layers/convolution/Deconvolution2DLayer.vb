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
Imports DeconvolutionParamInitializer = org.deeplearning4j.nn.params.DeconvolutionParamInitializer
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Shape = org.nd4j.linalg.api.shape.Shape
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
	Public Class Deconvolution2DLayer
		Inherits ConvolutionLayer

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub


		Friend Overrides Sub initializeHelper()
			' no op
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			If input_Conflict.rank() <> 4 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Deconvolution2DLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 4 array with shape " & layerConf().getCnn2dDataFormat().dimensionNames() & ". " & layerId())
			End If

			Dim weights As INDArray = getParamWithNoise(DeconvolutionParamInitializer.WEIGHT_KEY, True, workspaceMgr)

			Dim format As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim hDim As Integer = If(nchw, 2, 1)
			Dim wDim As Integer = If(nchw, 3, 2)

			Dim miniBatch As Long = input_Conflict.size(0)
			Dim inH As Long = input_Conflict.size(hDim)
			Dim inW As Long = input_Conflict.size(wDim)

			Dim inDepth As Long = weights.size(0)

			Dim kH As Long = weights.size(2)
			Dim kW As Long = weights.size(3)

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim pad() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				Dim outSize() As Integer = {CInt(epsilon.size(hDim)), CInt(epsilon.size(wDim))}
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {CInt(inH), CInt(inW)}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
			End If

			Dim biasGradView As INDArray = gradientViews(DeconvolutionParamInitializer.BIAS_KEY)
			Dim weightGradView As INDArray = gradientViews(DeconvolutionParamInitializer.WEIGHT_KEY)

			Dim epsShape() As Long = If(nchw, New Long(){miniBatch, inDepth, inH, inW}, New Long()){miniBatch, inH, inW, inDepth}
			Dim outEps As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, weights.dataType(), epsShape, "c"c)

			Dim sameMode As Integer? = If(convolutionMode = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = {CInt(kH), CInt(kW), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), sameMode, If(nchw, 0, 1)}

			Dim delta As INDArray
			Dim afn As IActivation = layerConf().getActivationFn()
			Dim p As Pair(Of INDArray, INDArray) = preOutput4d(True, True, workspaceMgr)
			delta = afn.backprop(p.First, epsilon).First

			'DL4J Deconv weights: [inputDepth, outputDepth, kH, kW]
			'libnd4j weights: [kH, kW, oC, iC]
			weights = weights.permute(2, 3, 1, 0)
			Dim weightGradViewOp As INDArray = weightGradView.permute(2, 3, 1, 0)

			Dim opInputs() As INDArray
			Dim opOutputs() As INDArray
			If layerConf().hasBias() Then
				Dim bias As INDArray = getParamWithNoise(DeconvolutionParamInitializer.BIAS_KEY, True, workspaceMgr)
				opInputs = New INDArray(){input_Conflict, weights, bias, delta}
				opOutputs = New INDArray(){outEps, weightGradViewOp, biasGradView}
			Else
				opInputs = New INDArray(){input_Conflict, weights, delta}
				opOutputs = New INDArray(){outEps, weightGradViewOp}
			End If
			Dim op As CustomOp = DynamicCustomOp.builder("deconv2d_bp").addInputs(opInputs).addIntegerArguments(args).addOutputs(opOutputs).callInplace(False).build()
			Nd4j.Executioner.exec(op)


			Dim retGradient As Gradient = New DefaultGradient()
			If layerConf().hasBias() Then
				retGradient.setGradientFor(DeconvolutionParamInitializer.BIAS_KEY, biasGradView)
			End If
			retGradient.setGradientFor(DeconvolutionParamInitializer.WEIGHT_KEY, weightGradView, "c"c)
			weightNoiseParams.Clear()

			Return New Pair(Of Gradient, INDArray)(retGradient, outEps)
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of INDArray, INDArray)

			Dim bias As INDArray = getParamWithNoise(DeconvolutionParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim weights As INDArray = getParamWithNoise(DeconvolutionParamInitializer.WEIGHT_KEY, training, workspaceMgr)

			'Input validation: expect rank 4 matrix
			If input_Conflict.rank() <> 4 Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Deconvolution2D (layer name = " & layerName & ", layer index = " & index_Conflict & ") with shape " & Arrays.toString(input_Conflict.shape()) & ". " & "Expected rank 4 array with shape [minibatchSize, layerInputDepth, inputHeight, inputWidth]." & (If(input_Conflict.rank() = 2, " (Wrong input type (see InputType.convolutionalFlat()) or wrong data type?)", "")) & " " & layerId())
			End If

			Dim format As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim nchw As Boolean = format = CNN2DFormat.NCHW
			Dim cDim As Integer = If(nchw, 1, 3)
			Dim hDim As Integer = If(nchw, 2, 1)
			Dim wDim As Integer = If(nchw, 3, 2)

			Dim inDepth As Long = weights.size(0)
			Dim outDepth As Long = weights.size(1)

			If input_Conflict.size(cDim) <> inDepth Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If

				Dim s As String = "Cannot do forward pass in Deconvolution2D layer (layer name = " & layerName & ", layer index = " & index_Conflict & "): input array channels does not match CNN layer configuration" & " (data format = " & format & ", data input channels = " & input_Conflict.size(cDim) & ", " & (If(nchw, "[minibatch,inputDepth,height,width]", "[minibatch,height,width,inputDepth]")) & "=" & Arrays.toString(input_Conflict.shape()) & "; expected" & " input channels = " & inDepth & ") " & layerId()

				Dim dimIfWrongFormat As Integer = If(format = CNN2DFormat.NHWC, 1, 3)
				If input_Conflict.size(dimIfWrongFormat) = inDepth Then
					'User might have passed NCHW data to a NHWC net, or vice versa?
					s &= vbLf & ConvolutionUtils.NCHW_NHWC_ERROR_MSG
				End If

				Throw New DL4JInvalidInputException(s)
			End If
			Dim kH As Integer = CInt(weights.size(2))
			Dim kW As Integer = CInt(weights.size(3))

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()

			Dim pad() As Integer
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getDeconvolutionOutputSize(input_Conflict, kernel, strides, Nothing, convolutionMode, dilation, format) 'Also performs validation
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {CInt(input_Conflict.size(hDim)), CInt(input_Conflict.size(wDim))}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = ConvolutionUtils.getDeconvolutionOutputSize(input_Conflict, kernel, strides, pad, convolutionMode, dilation, format) 'Also performs validation
			End If

			Dim outH As Long = outSize(0)
			Dim outW As Long = outSize(1)


			Dim miniBatch As val = input_Conflict.size(0)
			Dim outShape() As Long = If(nchw, New Long(){miniBatch, outDepth, outH, outW}, New Long()){miniBatch, outH, outW, outDepth}
			Dim output As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, input_Conflict.dataType(), outShape, "c"c)

			Dim sameMode As Integer = If(convolutionMode = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = {kH, kW, strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), sameMode, If(nchw, 0, 1)}

			'DL4J Deconv weights: [inputDepth, outputDepth, kH, kW]
			'libnd4j weights: [kH, kW, oC, iC]
			weights = weights.permute(2, 3, 1, 0)

			Dim opInputs() As INDArray
			If layerConf().hasBias() Then
				opInputs = New INDArray(){input_Conflict, weights, bias}
			Else
				opInputs = New INDArray(){input_Conflict, weights}
			End If
			Dim op As CustomOp = DynamicCustomOp.builder("deconv2d").addInputs(opInputs).addIntegerArguments(args).addOutputs(output).callInplace(False).build()
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

			Dim afn As IActivation = layerConf().getActivationFn()

			If helper_Conflict IsNot Nothing AndAlso Shape.strideDescendingCAscendingF(z) Then
				Dim ret As INDArray = helper_Conflict.activate(z, layerConf().getActivationFn(), training)
				If ret IsNot Nothing Then
					Return ret
				End If
			End If

			Dim activation As INDArray = afn.getActivation(z, training)
			Return activation
		End Function
	End Class
End Namespace