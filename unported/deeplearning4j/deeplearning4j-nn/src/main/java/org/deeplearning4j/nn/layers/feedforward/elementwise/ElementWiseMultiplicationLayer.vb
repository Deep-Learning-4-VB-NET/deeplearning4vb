Imports System
Imports ElementWiseParamInitializer = org.deeplearning4j.nn.params.ElementWiseParamInitializer
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
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

Namespace org.deeplearning4j.nn.layers.feedforward.elementwise



	<Serializable>
	Public Class ElementWiseMultiplicationLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.misc.ElementWiseMultiplicationLayer)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			'If this layer is layer L, then epsilon for this layer is ((w^(L+1)*(delta^(L+1))^T))^T (or equivalent)
			Dim z As INDArray = preOutput(True, workspaceMgr) 'Note: using preOutput(INDArray) can't be used as this does a setInput(input) and resets the 'appliedDropout' flag
			Dim delta As INDArray = layerConf().getActivationFn().backprop(z, epsilon).getFirst() 'TODO handle activation function params

			If maskArray_Conflict IsNot Nothing Then
				applyMask(delta)
			End If

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)

			Dim ret As Gradient = New DefaultGradient()

			Dim weightGrad As INDArray = gradientViews(ElementWiseParamInitializer.WEIGHT_KEY)
			weightGrad.subi(weightGrad)

			weightGrad.addi(input.mul(delta).sum(0))

			Dim biasGrad As INDArray = gradientViews(ElementWiseParamInitializer.BIAS_KEY)
			delta.sum(biasGrad, 0) 'biasGrad is initialized/zeroed first

			ret.gradientForVariable()(ElementWiseParamInitializer.WEIGHT_KEY) = weightGrad
			ret.gradientForVariable()(ElementWiseParamInitializer.BIAS_KEY) = biasGrad

	'      epsilonNext is a 2d matrix
			Dim epsilonNext As INDArray = delta.mulRowVector(params(ElementWiseParamInitializer.WEIGHT_KEY))
			epsilonNext = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, epsilonNext)

			epsilonNext = backpropDropOutIfPresent(epsilonNext)
			Return New Pair(Of Gradient, INDArray)(ret, epsilonNext)
		End Function


		''' <summary>
		''' Returns true if the layer can be trained in an unsupervised/pretrain manner (VAE, RBMs etc)
		''' </summary>
		''' <returns> true if the layer can be pretrained (using fit(INDArray), false otherwise </returns>
		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function preOutput(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim b As INDArray = getParam(DefaultParamInitializer.BIAS_KEY)
			Dim W As INDArray = getParam(DefaultParamInitializer.WEIGHT_KEY)

			If input_Conflict.columns() <> W.columns() Then
				Throw New DL4JInvalidInputException("Input size (" & input_Conflict.columns() & " columns; shape = " & Arrays.toString(input_Conflict.shape()) & ") is invalid: does not match layer input size (layer # inputs = " & W.shapeInfoToString() & ") " & layerId())
			End If

			Dim input As INDArray = Me.input_Conflict.castTo(dataType)

			applyDropOutIfNecessary(training, workspaceMgr)

			Dim ret As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, input.dataType(), input.shape(), "c"c)

			ret.assign(input.mulRowVector(W).addiRowVector(b))

			If maskArray_Conflict IsNot Nothing Then
				applyMask(ret)
			End If

			Return ret
		End Function

	End Class
End Namespace