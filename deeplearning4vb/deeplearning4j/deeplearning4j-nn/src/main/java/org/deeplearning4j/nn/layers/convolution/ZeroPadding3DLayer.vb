Imports System
Imports val = lombok.val
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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
	Public Class ZeroPadding3DLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.ZeroPadding3DLayer)

		Private padding() As Integer ' [padLeft1, padRight1, padLeft2, padRight2, padLeft3, padRight3]

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
			Me.padding = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.ZeroPadding3DLayer).getPadding()
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

			Dim epsNext As INDArray = epsilon.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(padding(0), padding(0) + inShape(2)), NDArrayIndex.interval(padding(2), padding(2) + inShape(3)), NDArrayIndex.interval(padding(4), padding(4) + inShape(4)))

			epsNext = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsNext)
			Return New Pair(Of Gradient, INDArray)(DirectCast(New DefaultGradient(), Gradient), epsNext)
		End Function


		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			Dim inShape As val = input_Conflict.shape()
			Dim outD As val = inShape(2) + padding(0) + padding(1)
			Dim outH As val = inShape(3) + padding(2) + padding(3)
			Dim outW As val = inShape(4) + padding(4) + padding(5)
			Dim outShape As val = New Long() {inShape(0), inShape(1), outD, outH, outW}

			Dim [out] As INDArray = workspaceMgr.create(ArrayType.ACTIVATIONS, input_Conflict.dataType(), outShape, "c"c)

			[out].put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(padding(0), padding(0) + inShape(2)), NDArrayIndex.interval(padding(2), padding(2) + inShape(3)), NDArrayIndex.interval(padding(4), padding(4) + inShape(4))}, input_Conflict)

			Return [out]
		End Function

		Public Overrides Function clone() As Layer
			Return New ZeroPadding3DLayer(conf_Conflict.clone(), dataType)
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function
	End Class

End Namespace