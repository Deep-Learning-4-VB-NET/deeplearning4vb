Imports Value = lombok.Value
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports ActorCriticDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.ActorCriticDenseNetworkConfiguration
Imports Constants = org.deeplearning4j.rl4j.util.Constants
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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

Namespace org.deeplearning4j.rl4j.network.ac

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value public class ActorCriticFactoryCompGraphStdDense implements ActorCriticFactoryCompGraph
	Public Class ActorCriticFactoryCompGraphStdDense
		Implements ActorCriticFactoryCompGraph

		Friend conf As ActorCriticDenseNetworkConfiguration

		Public Overridable Function buildActorCritic(ByVal numInputs() As Integer, ByVal numOutputs As Integer) As ActorCriticCompGraph
			Dim nIn As Integer = 1
			For Each i As Integer In numInputs
				nIn *= i
			Next i
			Dim confB As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(If(conf.getUpdater() IsNot Nothing, conf.getUpdater(), New Adam())).weightInit(WeightInit.XAVIER).l2(conf.getL2()).graphBuilder().setInputTypes(If(conf.isUseLSTM(), InputType.recurrent(nIn), InputType.feedForward(nIn))).addInputs("input").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(conf.getNumHiddenNodes()).activation(Activation.RELU).build(), "input")


			Dim i As Integer = 1
			Do While i < conf.getNumLayers()
				confB.addLayer(i & "", (New DenseLayer.Builder()).nIn(conf.getNumHiddenNodes()).nOut(conf.getNumHiddenNodes()).activation(Activation.RELU).build(), (i - 1) & "")
				i += 1
			Loop


			If conf.isUseLSTM() Then
				confB.addLayer(getConf().getNumLayers() & "", (New LSTM.Builder()).activation(Activation.TANH).nOut(conf.getNumHiddenNodes()).build(), (getConf().getNumLayers() - 1) & "")

				confB.addLayer("value", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(1).build(), getConf().getNumLayers() & "")

				confB.addLayer("softmax", (New RnnOutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nOut(numOutputs).build(), getConf().getNumLayers() & "")
			Else
				confB.addLayer("value", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(1).build(), (getConf().getNumLayers() - 1) & "")

				confB.addLayer("softmax", (New OutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nOut(numOutputs).build(), (getConf().getNumLayers() - 1) & "")
			End If

			confB.setOutputs("value", "softmax")


			Dim cgconf As ComputationGraphConfiguration = confB.build()
			Dim model As New ComputationGraph(cgconf)
			model.init()
			If conf.getListeners() IsNot Nothing Then
				model.setListeners(conf.getListeners())
			Else
				model.setListeners(New ScoreIterationListener(Constants.NEURAL_NET_ITERATION_LISTENER))
			End If

			Return New ActorCriticCompGraph(model)
		End Function

	End Class

End Namespace