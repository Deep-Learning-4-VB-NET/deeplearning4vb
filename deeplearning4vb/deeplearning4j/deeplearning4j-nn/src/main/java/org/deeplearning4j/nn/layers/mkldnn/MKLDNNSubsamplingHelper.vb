Imports System.Collections.Generic
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports SubsamplingHelper = org.deeplearning4j.nn.layers.convolution.subsampling.SubsamplingHelper
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports AvgPooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.AvgPooling2D
Imports MaxPooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.MaxPooling2D
Imports Pooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports Pooling2DDerivative = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2DDerivative
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
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

Namespace org.deeplearning4j.nn.layers.mkldnn


	Public Class MKLDNNSubsamplingHelper
		Implements SubsamplingHelper

		Protected Friend context As OpContext

		Public Sub New(ByVal dataType As DataType)

		End Sub

		Public Overridable Function checkSupported() As Boolean
			Return BaseMKLDNNHelper.mklDnnEnabled()
		End Function

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal epsilon As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal poolingType As PoolingType, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements SubsamplingHelper.backpropGradient
			If poolingType = PoolingType.SUM OrElse poolingType = PoolingType.PNORM Then
				Return Nothing
			End If

			Dim gradAtInput As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape())

			Dim hIdx As Integer = 2
			Dim wIdx As Integer = 3
			If format = CNN2DFormat.NHWC Then
				hIdx = 1
				wIdx = 2
			End If

			If convolutionMode = ConvolutionMode.Same Then
				pad = ConvolutionUtils.getSameModeTopLeftPadding(New Integer(){CInt(epsilon.size(hIdx)), CInt(epsilon.size(wIdx))}, New Integer() {CInt(input.size(hIdx)), CInt(input.size(wIdx))}, kernel, strides, dilation)
			End If

			Dim conf As Pooling2DConfig = Pooling2DConfig.builder().isSameMode(convolutionMode = ConvolutionMode.Same).kH(kernel(0)).kW(kernel(1)).sH(strides(0)).sW(strides(1)).dH(dilation(0)).dW(dilation(1)).pH(pad(0)).pW(pad(1)).isNHWC(format = CNN2DFormat.NHWC).build()

			Select Case poolingType
				Case PoolingType.MAX
					conf.setType(Pooling2D.Pooling2DType.MAX)
				Case PoolingType.AVG
					conf.setType(Pooling2D.Pooling2DType.AVG)
			End Select

			Dim d As New Pooling2DDerivative(input, epsilon, gradAtInput, conf)

			Nd4j.exec(d)
			Return New Pair(Of Gradient, INDArray)(New DefaultGradient(), gradAtInput)
		End Function

		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal poolingType As PoolingType, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements SubsamplingHelper.activate

			Dim hIdx As Integer = 2
			Dim wIdx As Integer = 3
			If format = CNN2DFormat.NHWC Then
				hIdx = 1
				wIdx = 2
			End If

			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, format) 'Also performs validation
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {CInt(input.size(hIdx)), CInt(input.size(wIdx))}, kernel, strides, dilation)
			Else
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, format) 'Also performs validation
			End If

			Dim outShape() As Long = If(format = CNN2DFormat.NCHW, New Long(){input.size(0), input.size(1), outSize(0), outSize(1)}, New Long()){input.size(0), outSize(0), outSize(1), input.size(3)}
			Dim output As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input.dataType(), outShape)

			If context Is Nothing Then
				context = Nd4j.Executioner.buildContext()
				context.setIArguments(kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), ArrayUtil.fromBoolean(convolutionMode = ConvolutionMode.Same), 0,If(format = CNN2DFormat.NCHW, 0, 1)) '0 = NCHW, 1=NHWC
			End If

			Dim op As DynamicCustomOp
			Select Case poolingType
				Case PoolingType.MAX
					op = New MaxPooling2D()
				Case PoolingType.AVG
					op = New AvgPooling2D()
				Case Else
					Return Nothing
			End Select

			context.purge()
			context.setInputArray(0, input)
			context.setOutputArray(0, output)

			Nd4j.exec(op, context)

			Return output
		End Function

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			Return Collections.emptyMap()
		End Function
	End Class

End Namespace