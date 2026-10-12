Imports System
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports PReLUParamInitializer = org.deeplearning4j.nn.params.PReLUParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationPReLU = org.nd4j.linalg.activations.impl.ActivationPReLU
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.layers.feedforward

	<Serializable>
	Public Class PReLU
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.PReLULayer)

		Friend axes() As Long = layerConf().getSharedAxes()


		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function type() As Type
			Return Type.FEED_FORWARD
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal mgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			applyDropOutIfNecessary(training, mgr)

			Dim [in] As INDArray
			If training Then
				[in] = mgr.dup(ArrayType.ACTIVATIONS, input_Conflict, input_Conflict.ordering())
			Else
				[in] = mgr.leverageTo(ArrayType.ACTIVATIONS, input_Conflict)
			End If

			Dim alpha As INDArray = getParam(PReLUParamInitializer.WEIGHT_KEY)

			Return (New ActivationPReLU(alpha, axes)).getActivation([in], training)
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim layerInput As INDArray = workspaceMgr.dup(ArrayType.ACTIVATION_GRAD, input_Conflict, input_Conflict.ordering())

			Dim alpha As INDArray = getParam(PReLUParamInitializer.WEIGHT_KEY)
			Dim prelu As IActivation = New ActivationPReLU(alpha, axes)

			Dim deltas As Pair(Of INDArray, INDArray) = prelu.backprop(layerInput, epsilon)
			Dim delta As INDArray = deltas.First
			Dim weightGrad As INDArray = deltas.Second
			Dim weightGradView As INDArray = gradientViews(PReLUParamInitializer.WEIGHT_KEY)
			weightGradView.assign(weightGrad)


			delta = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, delta) 'Usually a no-op (except for perhaps identity)
			delta = backpropDropOutIfPresent(delta)
			Dim ret As Gradient = New DefaultGradient()
			ret.setGradientFor(PReLUParamInitializer.WEIGHT_KEY, weightGradView, "c"c)

			Return New Pair(Of Gradient, INDArray)(ret, delta)
		End Function


		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

	End Class
End Namespace