Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Value = lombok.Value
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports NetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.NetworkConfiguration
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

Namespace org.deeplearning4j.rl4j.network.dqn


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value public class DQNFactoryStdConv implements DQNFactory
	Public Class DQNFactoryStdConv
		Implements DQNFactory


		Friend conf As NetworkConfiguration

		Public Overridable Function buildDQN(ByVal shapeInputs() As Integer, ByVal numOutputs As Integer) As DQN

			If shapeInputs.Length = 1 Then
				Throw New AssertionError("Impossible to apply convolutional layer on a shape == 1")
			End If


			Dim confB As NeuralNetConfiguration.ListBuilder = (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).l2(conf.getL2()).updater(If(conf.getUpdater() IsNot Nothing, conf.getUpdater(), New Adam())).weightInit(WeightInit.XAVIER).l2(conf.getL2()).list().layer(0, (New ConvolutionLayer.Builder(8, 8)).nIn(shapeInputs(0)).nOut(16).stride(4, 4).activation(Activation.RELU).build())


			confB.layer(1, (New ConvolutionLayer.Builder(4, 4)).nOut(32).stride(2, 2).activation(Activation.RELU).build())

			confB.layer(2, (New DenseLayer.Builder()).nOut(256).activation(Activation.RELU).build())

			confB.layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(numOutputs).build())

			confB.InputType = InputType.convolutional(shapeInputs(1), shapeInputs(2), shapeInputs(0))
			Dim mlnconf As MultiLayerConfiguration = confB.build()
			Dim model As New MultiLayerNetwork(mlnconf)
			model.init()
			If conf.getListeners() IsNot Nothing Then
				model.setListeners(conf.getListeners())
			Else
				model.setListeners(New ScoreIterationListener(Constants.NEURAL_NET_ITERATION_LISTENER))
			End If

			Return New DQN(model)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Value public static class Configuration
		Public Class Configuration

			Friend learningRate As Double
			Friend l2 As Double
			Friend updater As IUpdater
			Friend listeners() As TrainingListener

			''' <summary>
			''' Converts the deprecated Configuration to the new NetworkConfiguration format
			''' </summary>
			Public Overridable Function toNetworkConfiguration() As NetworkConfiguration
				Dim builder As NetworkConfiguration.NetworkConfigurationBuilder = NetworkConfiguration.builder().learningRate(learningRate).l2(l2).updater(updater)

				If listeners IsNot Nothing Then
					builder.listeners(Arrays.asList(listeners))
				End If

				Return builder.build()

			End Function
		End Class

	End Class

End Namespace