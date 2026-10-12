Imports System
Imports System.Collections.Generic
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent
Imports AdvantageActorCritic = org.deeplearning4j.rl4j.agent.learning.algorithm.actorcritic.AdvantageActorCritic
Imports BaseTransitionTDAlgorithm = org.deeplearning4j.rl4j.agent.learning.algorithm.dqn.BaseTransitionTDAlgorithm
Imports NStepQLearning = org.deeplearning4j.rl4j.agent.learning.algorithm.nstepqlearning.NStepQLearning
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports org.deeplearning4j.rl4j.agent.listener
Imports AdvantageActorCriticBuilder = org.deeplearning4j.rl4j.builder.AdvantageActorCriticBuilder
Imports DoubleDQNBuilder = org.deeplearning4j.rl4j.builder.DoubleDQNBuilder
Imports NStepQLearningBuilder = org.deeplearning4j.rl4j.builder.NStepQLearningBuilder
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports RobotLake = org.deeplearning4j.rl4j.mdp.robotlake.RobotLake
Imports ActorCriticNetwork = org.deeplearning4j.rl4j.network.ActorCriticNetwork
Imports ChannelToNetworkInputMapper = org.deeplearning4j.rl4j.network.ChannelToNetworkInputMapper
Imports org.deeplearning4j.rl4j.network
Imports QNetwork = org.deeplearning4j.rl4j.network.QNetwork
Imports org.deeplearning4j.rl4j.network.ac
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports ArrayToINDArrayTransform = org.deeplearning4j.rl4j.observation.transform.operation.ArrayToINDArrayTransform
Imports org.deeplearning4j.rl4j.policy
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


	Public Class RobotLakeExample
		Private Const FROZEN_LAKE_SIZE As Integer = 5

		Private Const NUM_EPISODES As Integer = 10000

		Private Shared ReadOnly INPUT_CHANNEL_BINDINGS() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("tracker-in", "tracker"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("radar-in", "radar")}
		Private Shared ReadOnly TRANSFORM_PROCESS_OUTPUT_CHANNELS() As String = { "tracker", "radar" }

		Public Shared Sub Main(ByVal args() As String)
			Dim rnd As Random = Nd4j.RandomFactory.getNewRandomInstance(123)
			Dim environmentBuilder As Builder(Of Environment(Of Integer)) = Function() New RobotLake(FROZEN_LAKE_SIZE, False, rnd)
			Dim transformProcessBuilder As Builder(Of TransformProcess) = Function() TransformProcess.builder().transform("tracker", New ArrayToINDArrayTransform()).transform("radar", New ArrayToINDArrayTransform()).build(TRANSFORM_PROCESS_OUTPUT_CHANNELS)

			Dim listeners As IList(Of AgentListener(Of Integer)) = New ArrayListAnonymousInnerClass()

			Dim builder As Builder(Of IAgentLearner(Of Integer)) = setupDoubleDQN(environmentBuilder, transformProcessBuilder, listeners, rnd)
			'Builder<IAgentLearner<Integer>> builder = setupNStepQLearning(environmentBuilder, transformProcessBuilder, listeners, rnd);
			'Builder<IAgentLearner<Integer>> builder = setupAdvantageActorCritic(environmentBuilder, transformProcessBuilder, listeners, rnd);

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
	'			add(New EpisodeScorePrinter(100));
	'		}
		End Class

		Private Shared Function setupDoubleDQN(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildQNetwork()

			Dim configuration As DoubleDQNBuilder.Configuration = DoubleDQNBuilder.Configuration.builder().policyConfiguration(EpsGreedy.Configuration.builder().epsilonNbStep(3000).minEpsilon(0.1).build()).experienceHandlerConfiguration(ReplayMemoryExperienceHandler.Configuration.builder().maxReplayMemorySize(10000).batchSize(64).build()).neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(50).build()).updateAlgorithmConfiguration(BaseTransitionTDAlgorithm.Configuration.builder().gamma(0.99).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(200).build()).agentLearnerListeners(listeners).build()
			Return New DoubleDQNBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function


		Private Shared Function setupNStepQLearning(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildQNetwork()

			Dim configuration As NStepQLearningBuilder.Configuration = NStepQLearningBuilder.Configuration.builder().policyConfiguration(EpsGreedy.Configuration.builder().epsilonNbStep(75000).minEpsilon(0.1).build()).neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(50).build()).nstepQLearningConfiguration(NStepQLearning.Configuration.builder().build()).experienceHandlerConfiguration(StateActionExperienceHandler.Configuration.builder().batchSize(Integer.MaxValue).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(100).build()).agentLearnerListeners(listeners).build()
			Return New NStepQLearningBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function setupAdvantageActorCritic(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildActorCriticNetwork()

			Dim configuration As AdvantageActorCriticBuilder.Configuration = AdvantageActorCriticBuilder.Configuration.builder().neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().build()).advantageActorCriticConfiguration(AdvantageActorCritic.Configuration.builder().gamma(0.99).build()).experienceHandlerConfiguration(StateActionExperienceHandler.Configuration.builder().batchSize(Integer.MaxValue).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(100).build()).agentLearnerListeners(listeners).build()
			Return New AdvantageActorCriticBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function buildBaseNetworkConfiguration() As ComputationGraphConfiguration.GraphBuilder
			Return (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Adam()).weightInit(WeightInit.XAVIER).graphBuilder().setInputTypes(InputType.feedForward(2), InputType.feedForward(4)).addInputs("tracker-in", "radar-in").layer("dl_1", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(40).build(), "tracker-in", "radar-in").layer("dl_out", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(40).build(), "dl_1")
		End Function

		Private Shared Function buildQNetwork() As ITrainableNeuralNet
			Dim conf As ComputationGraphConfiguration = buildBaseNetworkConfiguration().addLayer("output", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(RobotLake.NUM_ACTIONS).build(), "dl_out").setOutputs("output").build()

			Dim model As New ComputationGraph(conf)
			model.init()
			Return QNetwork.builder().withNetwork(model).inputBindings(INPUT_CHANNEL_BINDINGS).channelNames(TRANSFORM_PROCESS_OUTPUT_CHANNELS).build()
		End Function

		Private Shared Function buildActorCriticNetwork() As ITrainableNeuralNet
			Dim conf As ComputationGraphConfiguration = buildBaseNetworkConfiguration().addLayer("value", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(1).build(), "dl_out").addLayer("softmax", (New OutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nOut(RobotLake.NUM_ACTIONS).build(), "dl_out").setOutputs("value", "softmax").build()

			Dim model As New ComputationGraph(conf)
			model.init()

			Return ActorCriticNetwork.builder().withCombinedNetwork(model).inputBindings(INPUT_CHANNEL_BINDINGS).channelNames(TRANSFORM_PROCESS_OUTPUT_CHANNELS).build()
		End Function

		Private Class EpisodeScorePrinter
			Implements AgentListener(Of Integer)

			Friend ReadOnly trailingNum As Integer
			Friend ReadOnly results() As Boolean
			Friend episodeCount As Integer

			Public Sub New(ByVal trailingNum As Integer)
				Me.trailingNum = trailingNum
				results = New Boolean(trailingNum - 1){}
			End Sub

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
				Dim environment As RobotLake = CType(agent.getEnvironment(), RobotLake)
				Dim isSuccess As Boolean = False

				Dim result As String
				If environment.isGoalReached() Then
					result = "GOAL REACHED ******"
					isSuccess = True
				ElseIf environment.isEpisodeFinished() Then
					result = "FAILED"
				Else
					result = "DID NOT FINISH"
				End If

				results(episodeCount Mod trailingNum) = isSuccess

				If episodeCount >= trailingNum Then
					Dim successCount As Integer = 0
					For i As Integer = 0 To trailingNum - 1
						successCount += If(results(i), 1, 0)
					Next i
					Dim successRatio As Double = successCount / CDbl(trailingNum)

					Console.WriteLine(String.Format("[{0}] Episode {1,4:D} : score = {2,4:F2}, success ratio = {3,4:F2}, result = {4}", agent.getId(), episodeCount, agent.getReward(), successRatio, result))
				Else
					Console.WriteLine(String.Format("[{0}] Episode {1,4:D} : score = {2,4:F2}, result = {3}", agent.getId(), episodeCount, agent.getReward(), result))
				End If


				episodeCount += 1
			End Sub
		End Class
	End Class
End Namespace