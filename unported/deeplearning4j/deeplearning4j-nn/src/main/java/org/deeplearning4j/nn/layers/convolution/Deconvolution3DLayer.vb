Imports System
Imports val = lombok.val
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports Deconvolution3D = org.deeplearning4j.nn.conf.layers.Deconvolution3D
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DeconvolutionParamInitializer = org.deeplearning4j.nn.params.DeconvolutionParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.nn.layers.convolution


	<Serializable>
	Public Class Deconvolution3DLayer
		Inherits BaseLayer(Of Deconvolution3D)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			If input_Conflict.rank() <> 5 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Deconvolution3DLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 5 array with shape [minibatchSize, channels, inputHeight, inputWidth, inputDepth] or" & " [minibatchSize, inputHeight, inputWidth, inputDepth, channels]. " & layerId())
			End If

			Dim weights As INDArray = getParamWithNoise(DeconvolutionParamInitializer.WEIGHT_KEY, True, workspaceMgr)

			Dim df As Convolution3D.DataFormat = layerConf().getDataFormat()
			Dim cm As ConvolutionMode = layerConf().getConvolutionMode()

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim pad() As Integer = layerConf().getPadding()

			Dim biasGradView As INDArray = gradientViews(DeconvolutionParamInitializer.BIAS_KEY)
			Dim weightGradView As INDArray = gradientViews(DeconvolutionParamInitializer.WEIGHT_KEY)

			Dim outEps As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, weights.dataType(), input_Conflict.shape(), "c"c)

			Dim sameMode As Integer? = If(layerConf().getConvolutionMode() = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = {kernel(0), kernel(1), kernel(2), strides(0), strides(1), strides(2), pad(0), pad(1), pad(2), dilation(0), dilation(1), dilation(2), sameMode, If(df = Convolution3D.DataFormat.NCDHW, 0, 1)}

			Dim delta As INDArray
			Dim afn As IActivation = layerConf().getActivationFn()
			Dim preOutput As INDArray = Me.preOutput(True, workspaceMgr)
			delta = afn.backprop(preOutput, epsilon).First

			Dim opInputs() As INDArray
			Dim opOutputs() As INDArray
			If layerConf().hasBias() Then
				Dim bias As INDArray = getParamWithNoise(DeconvolutionParamInitializer.BIAS_KEY, True, workspaceMgr)
				opInputs = New INDArray(){input_Conflict, weights, bias, delta}
				opOutputs = New INDArray(){outEps, weightGradView, biasGradView}
			Else
				opInputs = New INDArray(){input_Conflict, weights, delta}
				opOutputs = New INDArray(){outEps, weightGradView}
			End If
			Dim op As CustomOp = DynamicCustomOp.builder("deconv3d_bp").addInputs(opInputs).addIntegerArguments(args).addOutputs(opOutputs).callInplace(False).build()
			Nd4j.Executioner.exec(op)


			Dim retGradient As Gradient = New DefaultGradient()
			If layerConf().hasBias() Then
				retGradient.setGradientFor(DeconvolutionParamInitializer.BIAS_KEY, biasGradView)
			End If
			retGradient.setGradientFor(DeconvolutionParamInitializer.WEIGHT_KEY, weightGradView, "c"c)
			weightNoiseParams.Clear()

			Return New Pair(Of Gradient, INDArray)(retGradient, outEps)
		End Function

		Protected Friend Overrides Function preOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray

			Dim bias As INDArray = getParamWithNoise(DeconvolutionParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim weights As INDArray = getParamWithNoise(DeconvolutionParamInitializer.WEIGHT_KEY, training, workspaceMgr)

			'Input validation: expect rank 5 matrix
			If input_Conflict.rank() <> 5 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Deconvolution3DLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 5 array with shape [minibatchSize, channels, inputHeight, inputWidth, inputDepth] or" & " [minibatchSize, inputHeight, inputWidth, inputDepth, channels]. " & layerId())
			End If

			Dim df As Convolution3D.DataFormat = layerConf().getDataFormat()
			Dim ncdhw As Boolean = layerConf().getDataFormat() = Convolution3D.DataFormat.NCDHW
			Dim chDim As Integer = If(ncdhw, 1, 4)
			If input_Conflict.size(chDim) <> layerConf().getNIn() Then
				Dim layerName As String = conf_Conflict.getLayer().getLayerName()
				If layerName Is Nothing Then
					layerName = "(not named)"
				End If
				Throw New DL4JInvalidInputException("Cannot do forward pass in Deconvolution3D layer (layer name = " & layerName & ", layer index = " & index_Conflict & "): input array channels does not match CNN layer configuration" & " (data input channels = " & input_Conflict.size(chDim) & ", " & (If(ncdhw, "[minibatch,channels,height,width,depth]=", "[minibatch,height,width,depth,channels]=")) & Arrays.toString(input_Conflict.shape()) & "; expected" & " input channels = " & layerConf().getNIn() & ") " & layerId())
			End If

			Dim dilation() As Integer = layerConf().getDilation()
			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()

			Dim pad() As Integer
			Dim cm As ConvolutionMode = layerConf().getConvolutionMode()
			Dim outSize() As Long
			Dim inSize() As Integer = If(df = Convolution3D.DataFormat.NCDHW, New Integer(){CInt(input_Conflict.size(2)), CInt(input_Conflict.size(3)), CInt(input_Conflict.size(4))}, New Integer()){CInt(input_Conflict.size(1)), CInt(input_Conflict.size(2)), CInt(input_Conflict.size(3))}
			If cm = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getDeconvolution3DOutputSize(input_Conflict, kernel, strides, Nothing, dilation, cm, layerConf().getDataFormat()) 'Also performs validation
				pad = ConvolutionUtils.getSameModeTopLeftPadding(ArrayUtil.toInts(outSize), inSize, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = ConvolutionUtils.getDeconvolution3DOutputSize(input_Conflict, kernel, strides, pad, dilation, cm, layerConf().getDataFormat()) 'Also performs validation
			End If

			Dim outH As Long = outSize(0)
			Dim outW As Long = outSize(1)
			Dim outD As Long = outSize(2)


			Dim miniBatch As val = input_Conflict.size(0)
			Dim outShape() As Long = If(df = Convolution3D.DataFormat.NCDHW, New Long(){miniBatch, layerConf().getNOut(), outH, outW, outD}, New Long()){miniBatch, outH, outW, outD, layerConf().getNOut()}
			Dim output As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, input_Conflict.dataType(), outShape, "c"c)

			Dim sameMode As Integer = If(cm = ConvolutionMode.Same, 1, 0)

			Dim args() As Integer = {kernel(0), kernel(1), kernel(2), strides(0), strides(1), strides(2), pad(0), pad(1), pad(2), dilation(0), dilation(1), dilation(2), sameMode, If(df = Convolution3D.DataFormat.NCDHW, 0, 1)}

			Dim opInputs() As INDArray
			If layerConf().hasBias() Then
				opInputs = New INDArray(){input_Conflict, weights, bias}
			Else
				opInputs = New INDArray(){input_Conflict, weights}
			End If
			Dim op As CustomOp = DynamicCustomOp.builder("deconv3d").addInputs(opInputs).addIntegerArguments(args).addOutputs(output).callInplace(False).build()
			Nd4j.Executioner.exec(op)

			Return output
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)

			If cacheMode_Conflict = Nothing Then
				cacheMode_Conflict = CacheMode.NONE
			End If

			applyDropOutIfNecessary(training, workspaceMgr)

			Dim z As INDArray = preOutput(training, workspaceMgr)

			Dim afn As IActivation = layerConf().getActivationFn()

			Dim activation As INDArray = afn.getActivation(z, training)
			Return activation
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property
	End Class
End Namespace