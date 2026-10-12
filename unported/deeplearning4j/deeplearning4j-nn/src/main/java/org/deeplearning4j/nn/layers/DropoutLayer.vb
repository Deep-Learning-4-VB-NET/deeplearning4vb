Imports System
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.layers

	<Serializable>
	Public Class DropoutLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.DropoutLayer)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function

		Public Overrides Function type() As Type
			Return Type.FEED_FORWARD
		End Function

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim delta As INDArray = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, epsilon)

			If maskArray_Conflict IsNot Nothing Then
				delta.muliColumnVector(maskArray_Conflict)
			End If

			Dim ret As Gradient = New DefaultGradient()
			delta = backpropDropOutIfPresent(delta)
			Return New Pair(Of Gradient, INDArray)(ret, delta)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)

			Dim ret As INDArray
			If Not training Then
				ret = input_Conflict
			Else
				If layerConf().getIDropout() IsNot Nothing Then
					Dim result As INDArray
					If inputModificationAllowed Then
						result = input_Conflict
					Else
						result = workspaceMgr.createUninitialized(ArrayType.INPUT, input_Conflict.dataType(), input_Conflict.shape(), input_Conflict.ordering())
					End If

					ret = layerConf().getIDropout().applyDropout(input_Conflict, result, IterationCount, EpochCount, workspaceMgr)
				Else
					ret = workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, input_Conflict)
				End If
			End If

			If maskArray_Conflict IsNot Nothing Then
				ret.muliColumnVector(maskArray_Conflict)
			End If

			ret = workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, ret)
			Return ret
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function params() As INDArray
			Return Nothing
		End Function
	End Class

End Namespace