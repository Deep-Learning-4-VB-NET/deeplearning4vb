Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Value = lombok.Value
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToCnnPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports ActorCriticNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.ActorCriticNetworkConfiguration
Imports ActorCriticNetworkConfigurationBuilder = org.deeplearning4j.rl4j.network.configuration.ActorCriticNetworkConfiguration.ActorCriticNetworkConfigurationBuilder
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
'ORIGINAL LINE: @Value public class ActorCriticFactoryCompGraphStdConv implements ActorCriticFactoryCompGraph
	Public Class ActorCriticFactoryCompGraphStdConv
		Implements ActorCriticFactoryCompGraph

		Friend conf As ActorCriticNetworkConfiguration

		Public Overridable Function buildActorCritic(ByVal shapeInputs() As Integer, ByVal numOutputs As Integer) As ActorCriticCompGraph

			If shapeInputs.Length = 1 Then
				Throw New AssertionError("Impossible to apply convolutional layer on a shape == 1")
			End If

			Dim h As Integer = (((shapeInputs(1) - 8) \ 4 + 1) - 4) \ 2 + 1
			Dim w As Integer = (((shapeInputs(2) - 8) \ 4 + 1) - 4) \ 2 + 1

			Dim confB As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(If(conf.getUpdater() IsNot Nothing, conf.getUpdater(), New Adam())).weightInit(WeightInit.XAVIER).l2(conf.getL2()).graphBuilder().addInputs("input").addLayer("0", (New ConvolutionLayer.Builder(8, 8)).nIn(shapeInputs(0)).nOut(16).stride(4, 4).activation(Activation.RELU).build(), "input")

			confB.addLayer("1", (New ConvolutionLayer.Builder(4, 4)).nIn(16).nOut(32).stride(2, 2).activation(Activation.RELU).build(), "0")

			confB.addLayer("2", (New DenseLayer.Builder()).nIn(w * h * 32).nOut(256).activation(Activation.RELU).build(), "1")

			If conf.isUseLSTM() Then
				confB.addLayer("3", (New LSTM.Builder()).nIn(256).nOut(256).activation(Activation.TANH).build(), "2")

				confB.addLayer("value", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(256).nOut(1).build(), "3")

				confB.addLayer("softmax", (New RnnOutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nIn(256).nOut(numOutputs).build(), "3")
			Else
				confB.addLayer("value", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(256).nOut(1).build(), "2")

				confB.addLayer("softmax", (New OutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nIn(256).nOut(numOutputs).build(), "2")
			End If

			confB.setOutputs("value", "softmax")

			If conf.isUseLSTM() Then
				confB.inputPreProcessor("0", New RnnToCnnPreProcessor(shapeInputs(1), shapeInputs(2), shapeInputs(0)))
				confB.inputPreProcessor("2", New CnnToFeedForwardPreProcessor(h, w, 32))
				confB.inputPreProcessor("3", New FeedForwardToRnnPreProcessor())
			Else
				confB.InputTypes = InputType.convolutional(shapeInputs(1), shapeInputs(2), shapeInputs(0))
			End If

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Value @Deprecated public static class Configuration
		<Obsolete>
		Public Class Configuration

			Friend l2 As Double
			Friend updater As IUpdater
			Friend listeners() As TrainingListener
			Friend useLSTM As Boolean

			''' <summary>
			''' Converts the deprecated Configuration to the new NetworkConfiguration format
			''' </summary>
			Public Overridable Function toNetworkConfiguration() As ActorCriticNetworkConfiguration
				Dim builder As ActorCriticNetworkConfiguration.ActorCriticNetworkConfigurationBuilder = ActorCriticNetworkConfiguration.builder().l2(l2).updater(updater).useLSTM(useLSTM)

				If listeners IsNot Nothing Then
					builder.listeners(Arrays.asList(listeners))
				End If

				Return builder.build()

			End Function
		End Class

	End Class

End Namespace