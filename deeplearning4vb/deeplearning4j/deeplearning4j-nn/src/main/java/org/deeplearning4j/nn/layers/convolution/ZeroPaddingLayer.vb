Imports System
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports CNN2DFormat = org.deeplearning4j.nn.conf.CNN2DFormat
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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
	Public Class ZeroPaddingLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.ZeroPaddingLayer)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
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
			Return Type.CONVOLUTIONAL
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim inShape As val = input_Conflict.shape()

			Dim nchw As Boolean = layerConf().getDataFormat() = CNN2DFormat.NCHW
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			Dim epsNext As INDArray
			Dim padding() As Integer = layerConf().getPadding()
			If layerConf().getDataFormat() = CNN2DFormat.NCHW Then
				epsNext = epsilon.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(padding(0), padding(0) + inShape(hIdx)), NDArrayIndex.interval(padding(2), padding(2) + inShape(wIdx)))
			Else
				'NHWC
				epsNext = epsilon.get(NDArrayIndex.all(), NDArrayIndex.interval(padding(0), padding(0) + inShape(hIdx)), NDArrayIndex.interval(padding(2), padding(2) + inShape(wIdx)), NDArrayIndex.all())
			End If

			epsNext = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsNext)
			Return New Pair(Of Gradient, INDArray)(DirectCast(New DefaultGradient(), Gradient), epsNext)
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			Dim nchw As Boolean = layerConf().getDataFormat() = CNN2DFormat.NCHW
			Dim hIdx As Integer = If(nchw, 2, 1)
			Dim wIdx As Integer = If(nchw, 3, 2)

			Dim padding() As Integer = layerConf().getPadding()
			Dim inShape As val = input_Conflict.shape()
			Dim outH As val = inShape(hIdx) + padding(0) + padding(1)
			Dim outW As val = inShape(wIdx) + padding(2) + padding(3)
			Dim outShape As val = If(nchw, New Long() {inShape(0), inShape(1), outH, outW}, New Long()){inShape(0), outH, outW, inShape(3)}

			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, input_Conflict.dataType(), outShape, "c"c)

			If nchw Then
				[out].put(New INDArrayIndex(){NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(padding(0), padding(0) + inShape(hIdx)), NDArrayIndex.interval(padding(2), padding(2) + inShape(wIdx))}, input_Conflict)
			Else
				[out].put(New INDArrayIndex(){NDArrayIndex.all(), NDArrayIndex.interval(padding(0), padding(0) + inShape(hIdx)), NDArrayIndex.interval(padding(2), padding(2) + inShape(wIdx)), NDArrayIndex.all()}, input_Conflict)
			End If

			Return [out]
		End Function

		Public Overrides Function clone() As Layer
			Return New ZeroPaddingLayer(conf_Conflict.clone(), dataType)
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function
	End Class

End Namespace