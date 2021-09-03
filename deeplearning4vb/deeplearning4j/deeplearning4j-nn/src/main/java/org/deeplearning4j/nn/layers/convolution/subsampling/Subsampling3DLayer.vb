Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Convolution3D = org.deeplearning4j.nn.conf.layers.Convolution3D
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Convolution3DUtils = org.deeplearning4j.util.Convolution3DUtils
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

Namespace org.deeplearning4j.nn.layers.convolution.subsampling



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Subsampling3DLayer extends org.deeplearning4j.nn.layers.AbstractLayer<org.deeplearning4j.nn.conf.layers.Subsampling3DLayer>
	<Serializable>
	Public Class Subsampling3DLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.Subsampling3DLayer)

		Protected Friend convolutionMode As ConvolutionMode

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			Me.convolutionMode = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.Subsampling3DLayer).getConvolutionMode()
		End Sub


		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function

		Public Overrides Function type() As Type
			Return Type.SUBSAMPLING
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			Dim isNCDHW As Boolean = layerConf().getDataFormat() = Convolution3D.DataFormat.NCDHW

			Dim miniBatch As Long = input_Conflict.size(0)
			Dim inChannels As Long = If(isNCDHW, input_Conflict.size(1), input_Conflict.size(4))
			Dim inD As Integer = CInt(If(isNCDHW, input_Conflict.size(2), input_Conflict.size(1)))
			Dim inH As Integer = CInt(If(isNCDHW, input_Conflict.size(3), input_Conflict.size(2)))
			Dim inW As Integer = CInt(If(isNCDHW, input_Conflict.size(4), input_Conflict.size(3)))

			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim dilation() As Integer = layerConf().getDilation()

			Dim pad() As Integer
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = Convolution3DUtils.get3DOutputSize(input_Conflict, kernel, strides, Nothing, convolutionMode, dilation, isNCDHW)
				pad = Convolution3DUtils.get3DSameModeTopLeftPadding(outSize, New Integer(){inD, inH, inW}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
			End If

			Dim outEpsilon As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, epsilon.dataType(),If(isNCDHW, New Long(){miniBatch, inChannels, inD, inH, inW}, New Long()){miniBatch, inD, inH, inW, inChannels}, "c"c)


			Dim intArgs() As Integer = {kernel(0), kernel(1), kernel(2), strides(0), strides(1), strides(2), pad(0), pad(1), pad(2), dilation(0), dilation(1), dilation(2), If(convolutionMode = ConvolutionMode.Same, 1, 0), 0, If(isNCDHW, 0, 1)}

			Dim opName As String = If(layerConf().getPoolingType() = PoolingType.MAX, "maxpool3dnew_bp", "avgpool3dnew_bp")

			Dim op As CustomOp = DynamicCustomOp.builder(opName).addInputs(input_Conflict, epsilon).addIntegerArguments(intArgs).addOutputs(outEpsilon).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			Dim retGradient As Gradient = New DefaultGradient()
			outEpsilon = backpropDropOutIfPresent(outEpsilon)
			Return New Pair(Of Gradient, INDArray)(retGradient, outEpsilon)
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			If training AndAlso Not dropoutApplied AndAlso layerConf().getIDropout() IsNot Nothing Then
				applyDropOutIfNecessary(True, workspaceMgr)
			End If

			Dim isNCDHW As Boolean = layerConf().getDataFormat() = Convolution3D.DataFormat.NCDHW

			If input_Conflict.rank() <> 5 Then
				If isNCDHW Then
					Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Subsampling3DLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 5 array with shape [minibatchSize, channels, " & "inputDepth, inputHeight, inputWidth] when dataFormat=NCDHW. " & layerId())
				Else
					Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to Subsampling3DLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 5 array with shape [minibatchSize, inputDepth, inputHeight, inputWidth, channels] when dataFormat=NDHWC. " & layerId())
				End If
			End If

			Dim miniBatch As Long = input_Conflict.size(0)
			Dim inChannels As Long = If(isNCDHW, input_Conflict.size(1), input_Conflict.size(4))
			Dim inD As Integer = CInt(If(isNCDHW, input_Conflict.size(2), input_Conflict.size(1)))
			Dim inH As Integer = CInt(If(isNCDHW, input_Conflict.size(3), input_Conflict.size(2)))
			Dim inW As Integer = CInt(If(isNCDHW, input_Conflict.size(4), input_Conflict.size(3)))

			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim dilation() As Integer = layerConf().getDilation()
			Dim pad() As Integer
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				Dim inShape() As Integer = {inD, inH, inW}
				outSize = Convolution3DUtils.get3DOutputSize(input_Conflict, kernel, strides, Nothing, convolutionMode, dilation, isNCDHW)
				pad = Convolution3DUtils.get3DSameModeTopLeftPadding(outSize, inShape, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = Convolution3DUtils.get3DOutputSize(input_Conflict, kernel, strides, pad, convolutionMode, dilation, isNCDHW)
			End If
			Dim outD As Long = outSize(0)
			Dim outH As Long = outSize(1)
			Dim outW As Long = outSize(2)

			Dim opName As String = If(layerConf().getPoolingType() = PoolingType.MAX, "maxpool3dnew", "avgpool3dnew")

			Dim output As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input_Conflict.dataType(),If(isNCDHW, New Long(){miniBatch, inChannels, outD, outH, outW}, New Long()){miniBatch, outD, outH, outW, inChannels}, "c"c)

			Dim intArgs() As Integer = {kernel(0), kernel(1), kernel(2), strides(0), strides(1), strides(2), pad(0), pad(1), pad(2), dilation(0), dilation(1), dilation(2), If(convolutionMode = ConvolutionMode.Same, 1, 0), 0, If(isNCDHW, 0, 1)}

			Dim op As CustomOp = DynamicCustomOp.builder(opName).addInputs(input_Conflict).addIntegerArguments(intArgs).addOutputs(output).callInplace(False).build()

			Nd4j.Executioner.exec(op)

			Return output
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'no op
		End Sub

		Public Overrides Function gradient() As Gradient
			Throw New System.NotSupportedException("Not supported - no parameters")
		End Function

		Public Overrides Sub fit()

		End Sub

		Public Overrides Function numParams() As Long
			Return 0
		End Function

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
		End Sub

		Public Overrides Function score() As Double
			Return 0
		End Function

		Public Overrides Sub update(ByVal gradient As INDArray, ByVal paramType As String)

		End Sub

		Public Overrides Function params() As INDArray
			Return Nothing
		End Function

		Public Overrides Function getParam(ByVal param As String) As INDArray
			Return params()
		End Function

		Public Overrides WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
    
			End Set
		End Property
	End Class

End Namespace