Imports System
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.layers
Imports PretrainParamInitializer = org.deeplearning4j.nn.params.PretrainParamInitializer
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

Namespace org.deeplearning4j.nn.layers.feedforward.autoencoder

	<Serializable>
	Public Class AutoEncoder
		Inherits BasePretrainNetwork(Of org.deeplearning4j.nn.conf.layers.AutoEncoder)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function sampleHiddenGivenVisible(ByVal v As INDArray) As Pair(Of INDArray, INDArray)
			setInput(v, LayerWorkspaceMgr.noWorkspaces()) 'TODO
			Dim ret As INDArray = encode(v, True, LayerWorkspaceMgr.noWorkspaces()) 'TODO
			Return New Pair(Of INDArray, INDArray)(ret, ret)
		End Function

		Public Overrides Function sampleVisibleGivenHidden(ByVal h As INDArray) As Pair(Of INDArray, INDArray)
			Dim ret As INDArray = decode(h, LayerWorkspaceMgr.noWorkspaces()) 'TODO
			Return New Pair(Of INDArray, INDArray)(ret, ret)
		End Function

		' Encode
		Public Overridable Function encode(ByVal v As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim W As INDArray = getParamWithNoise(PretrainParamInitializer.WEIGHT_KEY, training, workspaceMgr)
			Dim hBias As INDArray = getParamWithNoise(PretrainParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim ret As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, W.dataType(), v.size(0), W.size(1))
			Dim preAct As INDArray = v.castTo(W.dataType()).mmuli(W, ret).addiRowVector(hBias)
			ret = layerConf().getActivationFn().getActivation(preAct, training)
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, ret)
		End Function

		' Decode
		Public Overridable Function decode(ByVal y As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim W As INDArray = getParamWithNoise(PretrainParamInitializer.WEIGHT_KEY, True, workspaceMgr)
			Dim vBias As INDArray = getParamWithNoise(PretrainParamInitializer.VISIBLE_BIAS_KEY, True, workspaceMgr)
			Dim preAct As INDArray = y.mmul(W.transpose()).addiRowVector(vBias)
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, layerConf().getActivationFn().getActivation(preAct, True))

		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Return encode(input, training, workspaceMgr)
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return encode(input_Conflict, training, workspaceMgr)
		End Function

		Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			Dim W As INDArray = getParamWithNoise(PretrainParamInitializer.WEIGHT_KEY, True, workspaceMgr)
			Dim input As INDArray = Me.input_Conflict.castTo(dataType)

			Dim corruptionLevel As Double = layerConf().getCorruptionLevel()

			Dim corruptedX As INDArray = If(corruptionLevel > 0, getCorruptedInput(input, corruptionLevel), input)
			setInput(corruptedX, workspaceMgr)

			Dim y As INDArray = encode(corruptedX, True, workspaceMgr)
			Dim z As INDArray = decode(y, workspaceMgr)

			Dim visibleLoss As INDArray = input.sub(z)
			Dim hiddenLoss As INDArray = If(layerConf().getSparsity() = 0, visibleLoss.mmul(W).muli(y).muli(y.rsub(1)), visibleLoss.mmul(W).muli(y).muli(y.add(-layerConf().getSparsity())))

			Dim wGradient As INDArray = corruptedX.transpose().mmul(hiddenLoss).addi(visibleLoss.transpose().mmul(y))
			Dim hBiasGradient As INDArray = hiddenLoss.sum(0)
			Dim vBiasGradient As INDArray = visibleLoss.sum(0)

			gradient_Conflict = createGradient(wGradient, vBiasGradient, hBiasGradient)
			ScoreWithZ = z

		End Sub


	End Class

End Namespace