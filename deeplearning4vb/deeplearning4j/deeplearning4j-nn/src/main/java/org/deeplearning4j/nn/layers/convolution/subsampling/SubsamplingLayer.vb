Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports HelperUtils = org.deeplearning4j.nn.layers.HelperUtils
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports MKLDNNSubsamplingHelper = org.deeplearning4j.nn.layers.mkldnn.MKLDNNSubsamplingHelper
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports org.nd4j.common.primitives
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
'ORIGINAL LINE: @Slf4j public class SubsamplingLayer extends org.deeplearning4j.nn.layers.AbstractLayer<org.deeplearning4j.nn.conf.layers.SubsamplingLayer>
	<Serializable>
	Public Class SubsamplingLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.SubsamplingLayer)

'JAVA TO VB CONVERTER NOTE: The field helper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend helper_Conflict As SubsamplingHelper = Nothing
		Protected Friend helperCountFail As Integer = 0
		Protected Friend convolutionMode As ConvolutionMode
		Public Const CUDNN_SUBSAMPLING_HELPER_CLASS_NAME As String = "org.deeplearning4j.cuda.convolution.subsampling.CudnnSubsamplingHelper"
		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			initializeHelper()
			Me.convolutionMode = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.SubsamplingLayer).getConvolutionMode()
		End Sub

		Friend Overridable Sub initializeHelper()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			helper_Conflict = HelperUtils.createHelper(CUDNN_SUBSAMPLING_HELPER_CLASS_NAME, GetType(MKLDNNSubsamplingHelper).FullName, GetType(SubsamplingHelper), layerConf().LayerName, dataType)
		End Sub

		Public Overrides Function calcRegularizationScore(ByVal backpropOnlyParams As Boolean) As Double
			Return 0
		End Function

		Public Overrides Function type() As Type
			Return Type.SUBSAMPLING
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)
			If epsilon.dataType() <> dataType Then
				epsilon = epsilon.castTo(dataType)
			End If

			Dim dataFormat As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim hIdx As Integer = 2
			Dim wIdx As Integer = 3
			If dataFormat = CNN2DFormat.NHWC Then
				hIdx = 1
				wIdx = 2
			End If

			Dim inH As Integer = CInt(input.size(hIdx))
			Dim inW As Integer = CInt(input.size(wIdx))

			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim dilation() As Integer = layerConf().getDilation()

			Dim pad() As Integer
			Dim outSizeFwd() As Integer = {CInt(epsilon.size(hIdx)), CInt(epsilon.size(wIdx))} 'NCHW
			Dim same As Boolean = convolutionMode = ConvolutionMode.Same
			If same Then
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSizeFwd, New Integer() {inH, inW}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
			End If

			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not layerConf().isCudnnAllowFallback()) Then
				Dim ret As Pair(Of Gradient, INDArray) = Nothing
				Try
					ret = helper_Conflict.backpropGradient(input, epsilon, kernel, strides, pad, layerConf().getPoolingType(), convolutionMode, dilation, dataFormat, workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message IsNot Nothing AndAlso e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						If TypeOf helper_Conflict Is MKLDNNSubsamplingHelper Then
							log.warn("MKL-DNN execution failed - falling back on built-in implementation",e)
						Else
							log.warn("CuDNN execution failed - falling back on built-in implementation",e)
						End If
					Else
						Throw New Exception(e)
					End If
				End Try
				If ret IsNot Nothing Then
					Return ret
				End If
			End If

			'subsampling doesn't have weights and thus gradients are not calculated for this layer
			'only scale and reshape epsilon
			Dim retGradient As Gradient = New DefaultGradient()


			Dim epsAtInput As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape(), "c"c)
			Dim b As DynamicCustomOp.DynamicCustomOpsBuilder
			Dim extra As Integer = 0
			Select Case layerConf().getPoolingType()
				Case MAX
					b = DynamicCustomOp.builder("maxpool2d_bp")
				Case AVG
					b = DynamicCustomOp.builder("avgpool2d_bp")
					If layerConf().isAvgPoolIncludePadInDivisor() Then
						'Mostly this is a legacy case - beta4 and earlier models.
						extra = 1 'Divide by "number present" excluding padding
					Else
						'Default behaviour
						extra = 0 'Divide by kH*kW not "number present"
					End If

				Case PNORM
					b = DynamicCustomOp.builder("pnormpool2d_bp")
					extra = layerConf().getPnorm()
					b.addFloatingPointArguments(layerConf().getEps())
				Case Else
					Throw New System.NotSupportedException("Pooling mode not supported in SubsamplingLayer: " & layerConf().getPoolingType())
			End Select

			b.addInputs(input, epsilon).addOutputs(epsAtInput).addIntegerArguments(kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), (If(same, 1, 0)), extra,If(dataFormat = CNN2DFormat.NCHW, 0, 1)) '0 = NCHW, 1=NHWC

			Nd4j.exec(b.build())

			Return New Pair(Of Gradient, INDArray)(retGradient, epsAtInput)
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			'Normally we would apply dropout first. However, dropout on subsampling layers is not something that users typically expect
			' consequently, we'll skip it here

			'Input validation: expect rank 4 matrix
			If input_Conflict.rank() <> 4 Then
				Throw New DL4JInvalidInputException("Got rank " & input_Conflict.rank() & " array as input to SubsamplingLayer with shape " & Arrays.toString(input_Conflict.shape()) & ". Expected rank 4 array with shape " & layerConf().getCnn2dDataFormat().dimensionNames() & ". " & layerId())
			End If

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)

			Dim chIdx As Integer = 1
			Dim hIdx As Integer = 2
			Dim wIdx As Integer = 3
			If layerConf().getCnn2dDataFormat() = CNN2DFormat.NHWC Then
				chIdx = 3
				hIdx = 1
				wIdx = 2
			End If

			Dim dataFormat As CNN2DFormat = layerConf().getCnn2dDataFormat()
			Dim miniBatch As Long = input.size(0)
			Dim inDepth As Long = input.size(chIdx)
			Dim inH As Integer = CInt(input.size(hIdx))
			Dim inW As Integer = CInt(input.size(wIdx))

			Dim kernel() As Integer = layerConf().getKernelSize()
			Dim strides() As Integer = layerConf().getStride()
			Dim dilation() As Integer = layerConf().getDilation()
			Dim pad() As Integer
			Dim outSize() As Integer
			Dim same As Boolean = convolutionMode = ConvolutionMode.Same
			If same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, layerConf().getCnn2dDataFormat()) 'Also performs validation
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {inH, inW}, kernel, strides, dilation)
			Else
				pad = layerConf().getPadding()
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, layerConf().getCnn2dDataFormat()) 'Also performs validation
			End If

			Dim outH As Long = outSize(0)
			Dim outW As Long = outSize(1)


			If helper_Conflict IsNot Nothing AndAlso (helperCountFail = 0 OrElse Not layerConf().isCudnnAllowFallback()) Then
				Dim ret As INDArray = Nothing
				Try
					ret = helper_Conflict.activate(input, training, kernel, strides, pad, layerConf().getPoolingType(), convolutionMode, dilation, dataFormat, workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If layerConf().isCudnnAllowFallback() Then
						helperCountFail += 1
						If TypeOf helper_Conflict Is MKLDNNSubsamplingHelper Then
							log.warn("MKL-DNN execution failed - falling back on built-in implementation",e)
						Else
							log.warn("CuDNN execution failed - falling back on built-in implementation",e)
						End If
					Else
						Throw New Exception(e)
					End If
				End Try
				If ret IsNot Nothing Then
					Return ret
				End If
			End If

			Dim outShape() As Long = If(layerConf().getCnn2dDataFormat() = CNN2DFormat.NCHW, New Long(){miniBatch, inDepth, outH, outW}, New Long()){miniBatch, outH, outW, inDepth}

			Dim output As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input.dataType(), outShape, "c"c)
			Dim b As DynamicCustomOp.DynamicCustomOpsBuilder
			Dim extra As Integer = 0
			Select Case layerConf().getPoolingType()
				Case MAX
					b = DynamicCustomOp.builder("maxpool2d")
				Case AVG
					b = DynamicCustomOp.builder("avgpool2d")
					If layerConf().isAvgPoolIncludePadInDivisor() Then
						'Mostly this is a legacy case - beta4 and earlier models.
						extra = 1 'Divide by "number present" excluding padding
					Else
						'Default behaviour
						extra = 0 'Divide by kH*kW not "number present"
					End If
				Case PNORM
					b = DynamicCustomOp.builder("pnormpool2d")
					extra = layerConf().getPnorm()
				Case Else
					Throw New System.NotSupportedException("Not supported: " & layerConf().getPoolingType())
			End Select

			b.addInputs(input).addOutputs(output).addIntegerArguments(kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), (If(same, 1, 0)), extra,If(layerConf().getCnn2dDataFormat() = CNN2DFormat.NCHW, 0, 1)) '0: NCHW, 1=NHWC

			Nd4j.exec(b.build())

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

		Public Overrides ReadOnly Property Helper As LayerHelper
			Get
				Return helper_Conflict
			End Get
		End Property

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

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArray Is Nothing Then
				'For same mode (with stride 1): output activations size is always same size as input activations size -> mask array is same size
				Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
			End If

			Dim outMask As INDArray = ConvolutionUtils.cnn2dMaskReduction(maskArray, layerConf().getKernelSize(), layerConf().getStride(), layerConf().getPadding(), layerConf().getDilation(), layerConf().getConvolutionMode())
			Return MyBase.feedForwardMaskArray(outMask, currentMaskState, minibatchSize)
		End Function
	End Class

End Namespace