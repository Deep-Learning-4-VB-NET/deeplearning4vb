Imports System.Collections.Generic
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports ConvolutionMode = org.deeplearning4j.nn.conf.ConvolutionMode
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ConvolutionHelper = org.deeplearning4j.nn.layers.convolution.ConvolutionHelper
Imports ConvolutionParamInitializer = org.deeplearning4j.nn.params.ConvolutionParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ConvolutionUtils = org.deeplearning4j.util.ConvolutionUtils
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports Conv2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2D
Imports Conv2DDerivative = org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2DDerivative
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


	Public Class MKLDNNConvHelper
		Implements ConvolutionHelper

		Protected Friend context As OpContext
		Protected Friend contextBwd As OpContext

		Public Sub New(ByVal dataType As DataType)

		End Sub

		Public Overridable Function checkSupported() As Boolean Implements ConvolutionHelper.checkSupported
			Return BaseMKLDNNHelper.mklDnnEnabled()
		End Function

		Public Overridable Function backpropGradient(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal delta As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal biasGradView As INDArray, ByVal weightGradView As INDArray, ByVal afn As IActivation, ByVal mode As ConvolutionLayer.AlgoMode, ByVal bwdFilterAlgo As ConvolutionLayer.BwdFilterAlgo, ByVal bwdDataAlgo As ConvolutionLayer.BwdDataAlgo, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements ConvolutionHelper.backpropGradient
			If input.dataType() <> DataType.FLOAT OrElse weights.dataType() <> DataType.FLOAT Then
				Return Nothing 'MKL-DNN only supports floating point dtype
			End If

			Dim hDim As Integer = 2
			Dim wDim As Integer = 3
			If format = CNN2DFormat.NHWC Then
				hDim = 1
				wDim = 2
			End If

			If convolutionMode = ConvolutionMode.Same Then
				pad = ConvolutionUtils.getSameModeTopLeftPadding(New Integer(){CInt(delta.size(hDim)), CInt(delta.size(wDim))}, New Integer() {CInt(input.size(hDim)), CInt(input.size(wDim))}, kernel, strides, dilation)
			End If

			If contextBwd Is Nothing Then
				contextBwd = Nd4j.Executioner.buildContext()
				contextBwd.setIArguments(kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), ArrayUtil.fromBoolean(convolutionMode = ConvolutionMode.Same),If(format = CNN2DFormat.NCHW, 0, 1), 1)
			End If

			Dim gradAtInput As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape())

			Dim inputsArr() As INDArray = If(biasGradView Is Nothing, New INDArray(){input, weights, delta}, New INDArray()){input, weights, bias, delta}
			Dim outputArr() As INDArray = If(biasGradView Is Nothing, New INDArray(){gradAtInput, weightGradView}, New INDArray()){gradAtInput, weightGradView, biasGradView}
			contextBwd.purge()
			For i As Integer = 0 To inputsArr.Length - 1
				contextBwd.setInputArray(i, inputsArr(i))
			Next i
			For i As Integer = 0 To outputArr.Length - 1
				contextBwd.setOutputArray(i, outputArr(i))
			Next i

			Dim op As New Conv2DDerivative()
			Nd4j.exec(op, contextBwd)

			Dim g As Gradient = New DefaultGradient()
			If biasGradView IsNot Nothing Then
				g.gradientForVariable()(ConvolutionParamInitializer.BIAS_KEY) = biasGradView
			End If
			g.gradientForVariable()(ConvolutionParamInitializer.WEIGHT_KEY) = weightGradView

			Return New Pair(Of Gradient, INDArray)(g, gradAtInput)
		End Function

		Public Overridable Function preOutput(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray, ByVal kernel() As Integer, ByVal strides() As Integer, ByVal pad() As Integer, ByVal mode As ConvolutionLayer.AlgoMode, ByVal fwdAlgo As ConvolutionLayer.FwdAlgo, ByVal convolutionMode As ConvolutionMode, ByVal dilation() As Integer, ByVal format As CNN2DFormat, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements ConvolutionHelper.preOutput
			If input.dataType() <> DataType.FLOAT OrElse weights.dataType() <> DataType.FLOAT Then
				Return Nothing 'MKL-DNN only supports floating point dtype
			End If


			Dim hDim As Integer = 2
			Dim wDim As Integer = 3
			If format = CNN2DFormat.NHWC Then
				hDim = 1
				wDim = 2
			End If

			Dim inH As Integer = CInt(input.size(hDim))
			Dim inW As Integer = CInt(input.size(wDim))
			Dim outSize() As Integer
			If convolutionMode = ConvolutionMode.Same Then
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, Nothing, convolutionMode, dilation, format) 'Also performs validation
				pad = ConvolutionUtils.getSameModeTopLeftPadding(outSize, New Integer() {inH, inW}, kernel, strides, dilation)
			Else
				outSize = ConvolutionUtils.getOutputSize(input, kernel, strides, pad, convolutionMode, dilation, format) 'Also performs validation
			End If

			If context Is Nothing Then
				context = Nd4j.Executioner.buildContext()
				context.setIArguments(kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), ArrayUtil.fromBoolean(convolutionMode = ConvolutionMode.Same),If(format = CNN2DFormat.NCHW, 0, 1), 1)
			End If

			Dim outDepth As Integer = CInt(weights.size(0))
			Dim outShape() As Long = If(format = CNN2DFormat.NCHW, New Long(){input.size(0), outDepth, outSize(0), outSize(1)}, New Long()){input.size(0), outSize(0), outSize(1), outDepth}
			Dim [out] As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input.dataType(), outShape)

			Dim inputsArr() As INDArray = If(bias Is Nothing, New INDArray(){input, weights}, New INDArray()){input, weights, bias}
			context.purge()
			For i As Integer = 0 To inputsArr.Length - 1
				context.setInputArray(i, inputsArr(i))
			Next i

			context.setOutputArray(0, [out])
			Dim op As New Conv2D()
			Nd4j.exec(op, context)

			Return [out]
		End Function

		Public Overridable Function activate(ByVal z As INDArray, ByVal afn As IActivation, ByVal training As Boolean) As INDArray Implements ConvolutionHelper.activate
			Return afn.getActivation(z, training)
		End Function

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			Return Collections.emptyMap()
		End Function
	End Class

End Namespace