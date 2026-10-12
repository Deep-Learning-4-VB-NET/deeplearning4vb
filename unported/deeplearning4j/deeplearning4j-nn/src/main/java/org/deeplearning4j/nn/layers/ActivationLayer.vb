Imports System
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.layers



	<Serializable>
	Public Class ActivationLayer
		Inherits AbstractLayer(Of org.deeplearning4j.nn.conf.layers.ActivationLayer)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function

		Public Overrides Function type() As Type
			Return Type.FEED_FORWARD
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim temp As INDArray = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, input_Conflict, input_Conflict.ordering())
			Dim delta As INDArray = layerConf().getActivationFn().backprop(temp, epsilon).getFirst() 'TODO handle activation function params
			If delta Is epsilon Then
				'Edge case: identity activation + external errors -> no-op
				delta = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, delta)
			End If

			delta = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, delta) 'Usually a no-op (except for perhaps identity)
			Dim ret As Gradient = New DefaultGradient()
			Return New Pair(Of Gradient, INDArray)(ret, delta)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal mgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)

			Dim [in] As INDArray
			If training Then
				'dup required: need to keep original input for backprop
				[in] = mgr.dup(ArrayType.ACTIVATIONS, input_Conflict, input_Conflict.ordering())
			Else
				[in] = mgr.leverageTo(ArrayType.ACTIVATIONS, input_Conflict)
			End If

			Return layerConf().getActivationFn().getActivation([in], training)
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Sub clearNoiseWeightParams()
			'No op
		End Sub


		Public Overrides Function params() As INDArray
			Return Nothing
		End Function

	End Class

End Namespace