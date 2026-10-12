Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Value = lombok.Value
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports DQNDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.DQNDenseNetworkConfiguration
Imports DQNDenseNetworkConfigurationBuilder = org.deeplearning4j.rl4j.network.configuration.DQNDenseNetworkConfiguration.DQNDenseNetworkConfigurationBuilder
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
'ORIGINAL LINE: @Value public class DQNFactoryStdDense implements DQNFactory
	Public Class DQNFactoryStdDense
		Implements DQNFactory

		Friend conf As DQNDenseNetworkConfiguration

		Public Overridable Function buildDQN(ByVal numInputs() As Integer, ByVal numOutputs As Integer) As DQN
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

			confB.layer(conf.getNumLayers(), (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nIn(conf.getNumHiddenNodes()).nOut(numOutputs).build())


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
'ORIGINAL LINE: @AllArgsConstructor @Value @Builder @Deprecated public static class Configuration
		<Obsolete>
		Public Class Configuration

			Friend numLayer As Integer
			Friend numHiddenNodes As Integer
			Friend l2 As Double
			Friend updater As IUpdater
			Friend listeners() As TrainingListener

			''' <summary>
			''' Converts the deprecated Configuration to the new NetworkConfiguration format
			''' </summary>
			Public Overridable Function toNetworkConfiguration() As DQNDenseNetworkConfiguration
				Dim builder As DQNDenseNetworkConfiguration.DQNDenseNetworkConfigurationBuilder = DQNDenseNetworkConfiguration.builder().numHiddenNodes(numHiddenNodes).numLayers(numLayer).l2(l2).updater(updater)

				If listeners IsNot Nothing Then
					builder.listeners(Arrays.asList(listeners))
				End If

				Return builder.build()
			End Function
		End Class

	End Class

End Namespace