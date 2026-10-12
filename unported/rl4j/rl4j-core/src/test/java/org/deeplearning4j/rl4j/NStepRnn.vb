Imports System
Imports System.Collections.Generic
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports PreprocessorVertex = org.deeplearning4j.nn.conf.graph.PreprocessorVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports RnnOutputLayer = org.deeplearning4j.nn.conf.layers.RnnOutputLayer
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent
Imports AdvantageActorCritic = org.deeplearning4j.rl4j.agent.learning.algorithm.actorcritic.AdvantageActorCritic
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports org.deeplearning4j.rl4j.agent.listener
Imports AdvantageActorCriticBuilder = org.deeplearning4j.rl4j.builder.AdvantageActorCriticBuilder
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
Imports org.deeplearning4j.rl4j.experience
Imports DoAsISayOrDont = org.deeplearning4j.rl4j.mdp.DoAsISayOrDont
Imports ActorCriticNetwork = org.deeplearning4j.rl4j.network.ActorCriticNetwork
Imports org.deeplearning4j.rl4j.network
Imports ActorCriticLoss = org.deeplearning4j.rl4j.network.ac.ActorCriticLoss
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports ArrayToINDArrayTransform = org.deeplearning4j.rl4j.observation.transform.operation.ArrayToINDArrayTransform
Imports ITrainer = org.deeplearning4j.rl4j.trainer.ITrainer
Imports org.deeplearning4j.rl4j.trainer
Imports Constants = org.deeplearning4j.rl4j.util.Constants
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.rl4j


	Public Class NStepRnn

		Private Const USE_SEPARATE_NETWORKS As Boolean = True
		Private Const NUM_EPISODES As Integer = 3000

		Private Const COMBINED_LSTM_LAYER_SIZE As Integer = 20
		Private Const COMBINED_DL1_LAYER_SIZE As Integer = 20
		Private Const COMBINED_DL2_LAYER_SIZE As Integer = 60

		Private Const SEPARATE_LSTM_LAYER_SIZE As Integer = 10
		Private Const SEPARATE_DL1_LAYER_SIZE As Integer = 10
		Private Const SEPARATE_DL2_LAYER_SIZE As Integer = 10

		Private Const NUM_INPUTS As Integer = 4
		Private Const NUM_ACTIONS As Integer = 2



		Public Shared Sub Main(ByVal args() As String)

			Dim rnd As Random = Nd4j.RandomFactory.getNewRandomInstance(123)

			Dim environmentBuilder As Builder(Of Environment(Of Integer)) = Function() New DoAsISayOrDont(rnd)
			Dim transformProcessBuilder As Builder(Of TransformProcess) = Function() TransformProcess.builder().transform("data", New ArrayToINDArrayTransform(1, NUM_INPUTS, 1)).build("data")

			Dim listeners As IList(Of AgentListener(Of Integer)) = New ArrayListAnonymousInnerClass()

			Dim builder As Builder(Of IAgentLearner(Of Integer)) = setupAdvantageActorCritic(environmentBuilder, transformProcessBuilder, listeners, rnd)

			Dim trainer As ITrainer = SyncTrainer.builder(Of Integer)().agentLearnerBuilder(builder).stoppingCondition(Function(t) t.getEpisodeCount() >= NUM_EPISODES).build()

			Dim before As Long = System.nanoTime()
			trainer.train()
			Dim after As Long = System.nanoTime()

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Console.WriteLine(String.Format("Total time for {0:D} episodes: {1:F}ms", NUM_EPISODES, (after - before) / 1e6))
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of AgentListener(Of Integer))

	'		{
	'			add(New EpisodeScorePrinter());
	'		}
		End Class

		Private Shared Function setupAdvantageActorCritic(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = If(USE_SEPARATE_NETWORKS, buildSeparateActorCriticNetwork(), buildActorCriticNetwork())

			Dim configuration As AdvantageActorCriticBuilder.Configuration = AdvantageActorCriticBuilder.Configuration.builder().neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().build()).advantageActorCriticConfiguration(AdvantageActorCritic.Configuration.builder().gamma(0.99).build()).experienceHandlerConfiguration(StateActionExperienceHandler.Configuration.builder().batchSize(20).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(200).build()).agentLearnerListeners(listeners).build()
			Return New AdvantageActorCriticBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function buildBaseNetworkConfiguration(ByVal lstmLayerSize As Integer, ByVal dl1Size As Integer, ByVal dl2Size As Integer) As ComputationGraphConfiguration.GraphBuilder
			Return (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Adam()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("input").setInputTypes(InputType.recurrent(NUM_INPUTS)).addLayer("lstm", (New LSTM.Builder()).nOut(lstmLayerSize).activation(Activation.TANH).build(), "input").addLayer("dl", (New DenseLayer.Builder()).nOut(dl1Size).activation(Activation.RELU).build(), "input", "lstm").addLayer("dl-1", (New DenseLayer.Builder()).nOut(dl2Size).activation(Activation.RELU).build(), "dl").addVertex("dl-rnn", New PreprocessorVertex(New FeedForwardToRnnPreProcessor()), "dl-1")
		End Function

		Private Shared Function buildActorCriticNetwork() As ITrainableNeuralNet
			Dim valueConfiguration As ComputationGraphConfiguration = buildBaseNetworkConfiguration(COMBINED_LSTM_LAYER_SIZE, COMBINED_DL1_LAYER_SIZE, COMBINED_DL2_LAYER_SIZE).addLayer("value", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(1).build(), "dl-rnn", "lstm").addLayer("softmax", (New RnnOutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nOut(NUM_ACTIONS).build(), "dl-rnn", "lstm").setOutputs("value", "softmax").build()

			Dim model As New ComputationGraph(valueConfiguration)
			model.init()

			Return ActorCriticNetwork.builder().withCombinedNetwork(model).build()
		End Function

		Private Shared Function buildSeparateActorCriticNetwork() As ITrainableNeuralNet
			Dim valueConfiguration As ComputationGraphConfiguration = buildBaseNetworkConfiguration(SEPARATE_LSTM_LAYER_SIZE, SEPARATE_DL1_LAYER_SIZE, SEPARATE_DL2_LAYER_SIZE).addLayer("value", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(1).build(), "dl-rnn", "lstm").setOutputs("value").build()

			Dim policyConfiguration As ComputationGraphConfiguration = buildBaseNetworkConfiguration(SEPARATE_LSTM_LAYER_SIZE, SEPARATE_DL1_LAYER_SIZE, SEPARATE_DL2_LAYER_SIZE).addLayer("softmax", (New RnnOutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nOut(NUM_ACTIONS).build(), "dl-rnn", "lstm").setOutputs("softmax").build()

			Dim valueModel As New ComputationGraph(valueConfiguration)
			valueModel.init()

			Dim policyModel As New ComputationGraph(policyConfiguration)
			policyModel.init()

			Return ActorCriticNetwork.builder().withSeparateNetworks(valueModel, policyModel).build()
		End Function

		Private Class EpisodeScorePrinter
			Implements AgentListener(Of Integer)

			Friend episodeCount As Integer = 0

			Public Overridable Function onBeforeEpisode(ByVal agent As Agent) As ListenerResponse
				Return ListenerResponse.CONTINUE
			End Function

			Public Overridable Function onBeforeStep(ByVal agent As Agent, ByVal observation As Observation, ByVal [integer] As Integer?) As ListenerResponse
				Return ListenerResponse.CONTINUE
			End Function

			Public Overridable Function onAfterStep(ByVal agent As Agent, ByVal stepResult As StepResult) As ListenerResponse
				Return ListenerResponse.CONTINUE
			End Function

			Public Overridable Sub onAfterEpisode(ByVal agent As Agent) Implements AgentListener(Of Integer).onAfterEpisode
				episodeCount += 1
				Console.WriteLine(String.Format("Episode {0,4:D} : score = {1,3:D}", episodeCount, CInt(Math.Truncate(agent.getReward()))))
			End Sub
		End Class
	End Class
End Namespace