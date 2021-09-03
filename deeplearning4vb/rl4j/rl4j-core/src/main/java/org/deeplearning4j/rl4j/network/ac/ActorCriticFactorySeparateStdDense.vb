Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Value = lombok.Value
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports ActorCriticDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.ActorCriticDenseNetworkConfiguration
Imports ActorCriticDenseNetworkConfigurationBuilder = org.deeplearning4j.rl4j.network.configuration.ActorCriticDenseNetworkConfiguration.ActorCriticDenseNetworkConfigurationBuilder
Imports Constants = org.deeplearning4j.rl4j.util.Constants
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
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
'ORIGINAL LINE: @Value public class ActorCriticFactorySeparateStdDense implements ActorCriticFactorySeparate
	Public Class ActorCriticFactorySeparateStdDense
		Implements ActorCriticFactorySeparate

		Friend conf As ActorCriticDenseNetworkConfiguration

		Public Overridable Function buildActorCritic(ByVal numInputs() As Integer, ByVal numOutputs As Integer) As ActorCriticSeparate
			Dim nIn As Integer = 1
			For Each i As Integer In numInputs
				nIn *= i
			Next i
			Dim confB As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(If(conf.getUpdater() IsNot Nothing, conf.getUpdater(), New Adam())).weightInit(WeightInit.XAVIER).l2(conf.getL2()).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(conf.getNumHiddenNodes()).activation(Activation.RELU).build())


			Dim i As Integer = 1
			Do While i < conf.getNumLayers()
				confB.layer(i, (New DenseLayer.Builder()).nIn(conf.getNumHiddenNodes()).nOut(conf.getNumHiddenNodes()).activation(Activation.RELU).build())
				i += 1
			Loop

			If conf.isUseLSTM() Then
				confB.layer(conf.getNumLayers(), (New LSTM.Builder()).nOut(conf.getNumHiddenNodes()).activation(Activation.TANH).build())

				confB.layer(conf.getNumLayers() + 1, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(conf.getNumHiddenNodes()).nOut(1).build())
			Else
				confB.layer(conf.getNumLayers(), (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(conf.getNumHiddenNodes()).nOut(1).build())
			End If

			confB.InputType = If(conf.isUseLSTM(), InputType.recurrent(nIn), InputType.feedForward(nIn))
			Dim mlnconf2 As MultiLayerConfiguration = confB.build()
			Dim model As New MultiLayerNetwork(mlnconf2)
			model.init()
			If conf.getListeners() IsNot Nothing Then
				model.setListeners(conf.getListeners())
			Else
				model.setListeners(New ScoreIterationListener(Constants.NEURAL_NET_ITERATION_LISTENER))
			End If

			Dim confB2 As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(If(conf.getUpdater() IsNot Nothing, conf.getUpdater(), New Adam())).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(conf.getNumHiddenNodes()).activation(Activation.RELU).build())


			i = 1
			Do While i < conf.getNumLayers()
				confB2.layer(i, (New DenseLayer.Builder()).nIn(conf.getNumHiddenNodes()).nOut(conf.getNumHiddenNodes()).activation(Activation.RELU).build())
				i += 1
			Loop

			If conf.isUseLSTM() Then
				confB2.layer(conf.getNumLayers(), (New LSTM.Builder()).nOut(conf.getNumHiddenNodes()).activation(Activation.TANH).build())

				confB2.layer(conf.getNumLayers() + 1, (New RnnOutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nIn(conf.getNumHiddenNodes()).nOut(numOutputs).build())
			Else
				confB2.layer(conf.getNumLayers(), (New OutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nIn(conf.getNumHiddenNodes()).nOut(numOutputs).build())
			End If

			confB2.InputType = If(conf.isUseLSTM(), InputType.recurrent(nIn), InputType.feedForward(nIn))
			Dim mlnconf As MultiLayerConfiguration = confB2.build()
			Dim model2 As New MultiLayerNetwork(mlnconf)
			model2.init()
			If conf.getListeners() IsNot Nothing Then
				model2.setListeners(conf.getListeners())
			Else
				model2.setListeners(New ScoreIterationListener(Constants.NEURAL_NET_ITERATION_LISTENER))
			End If


			Return New ActorCriticSeparate(model, model2)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Value @Builder @Deprecated public static class Configuration
		<Obsolete>
		Public Class Configuration

			Friend numLayer As Integer
			Friend numHiddenNodes As Integer
			Friend l2 As Double
			Friend updater As IUpdater
			Friend listeners() As TrainingListener
			Friend useLSTM As Boolean

			Public Overridable Function toNetworkConfiguration() As ActorCriticDenseNetworkConfiguration
				Dim builder As ActorCriticDenseNetworkConfiguration.ActorCriticDenseNetworkConfigurationBuilder = ActorCriticDenseNetworkConfiguration.builder().numHiddenNodes(numHiddenNodes).numLayers(numLayer).l2(l2).updater(updater).useLSTM(useLSTM)

				If listeners IsNot Nothing Then
					builder.listeners(Arrays.asList(listeners))
				End If

				Return builder.build()

			End Function
		End Class


	End Class

End Namespace