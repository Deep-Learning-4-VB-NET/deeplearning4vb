Imports System
Imports System.Collections.Generic
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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
Imports CartpoleEnvironment = org.deeplearning4j.rl4j.mdp.CartpoleEnvironment
Imports ActorCriticNetwork = org.deeplearning4j.rl4j.network.ActorCriticNetwork
Imports org.deeplearning4j.rl4j.network
Imports QNetwork = org.deeplearning4j.rl4j.network.QNetwork
Imports ActorCriticCompGraph = org.deeplearning4j.rl4j.network.ac.ActorCriticCompGraph
Imports ActorCriticFactoryCompGraphStdDense = org.deeplearning4j.rl4j.network.ac.ActorCriticFactoryCompGraphStdDense
Imports ActorCriticFactorySeparateStdDense = org.deeplearning4j.rl4j.network.ac.ActorCriticFactorySeparateStdDense
Imports org.deeplearning4j.rl4j.network.ac
Imports ActorCriticDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.ActorCriticDenseNetworkConfiguration
Imports DQNDenseNetworkConfiguration = org.deeplearning4j.rl4j.network.configuration.DQNDenseNetworkConfiguration
Imports DQNFactory = org.deeplearning4j.rl4j.network.dqn.DQNFactory
Imports DQNFactoryStdDense = org.deeplearning4j.rl4j.network.dqn.DQNFactoryStdDense
Imports org.deeplearning4j.rl4j.network.dqn
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports ArrayToINDArrayTransform = org.deeplearning4j.rl4j.observation.transform.operation.ArrayToINDArrayTransform
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.trainer
Imports ITrainer = org.deeplearning4j.rl4j.trainer.ITrainer
Imports org.deeplearning4j.rl4j.trainer
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam

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


	Public Class AgentLearnerCartpole

		Private Const IS_ASYNC As Boolean = False
		Private Const NUM_THREADS As Integer = 2
		Private Const USE_SEPARATE_NETWORKS As Boolean = False

		Private Const NUM_EPISODES As Integer = 3000

		Public Shared Sub Main(ByVal args() As String)

'JAVA TO VB CONVERTER TODO TASK: Method reference constructor syntax is not converted by Java to VB Converter:
			Dim environmentBuilder As Builder(Of Environment(Of Integer)) = CartpoleEnvironment::New
			Dim transformProcessBuilder As Builder(Of TransformProcess) = Function() TransformProcess.builder().transform("data", New ArrayToINDArrayTransform()).build("data")

			Dim rnd As Random = Nd4j.RandomFactory.getNewRandomInstance(123)

			Dim listeners As IList(Of AgentListener(Of Integer)) = New ArrayListAnonymousInnerClass()

			'Builder<IAgentLearner<Integer>> builder = setupDoubleDQN(environmentBuilder, transformProcessBuilder, listeners, rnd);
			Dim builder As Builder(Of IAgentLearner(Of Integer)) = setupNStepQLearning(environmentBuilder, transformProcessBuilder, listeners, rnd)
			'Builder<IAgentLearner<Integer>> builder = setupAdvantageActorCritic(environmentBuilder, transformProcessBuilder, listeners, rnd);

			Dim trainer As ITrainer
			If IS_ASYNC Then
				trainer = AsyncTrainer.builder(Of Integer)().agentLearnerBuilder(builder).numThreads(NUM_THREADS).stoppingCondition(Function(t) t.getEpisodeCount() >= NUM_EPISODES).build()
			Else
				trainer = SyncTrainer.builder(Of Integer)().agentLearnerBuilder(builder).stoppingCondition(Function(t) t.getEpisodeCount() >= NUM_EPISODES).build()
			End If

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

		Private Shared Function setupDoubleDQN(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildDQNNetwork()

			Dim configuration As DoubleDQNBuilder.Configuration = DoubleDQNBuilder.Configuration.builder().policyConfiguration(EpsGreedy.Configuration.builder().epsilonNbStep(3000).minEpsilon(0.1).build()).experienceHandlerConfiguration(ReplayMemoryExperienceHandler.Configuration.builder().maxReplayMemorySize(10000).batchSize(64).build()).neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(50).build()).updateAlgorithmConfiguration(BaseTransitionTDAlgorithm.Configuration.builder().gamma(0.99).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(200).build()).agentLearnerListeners(listeners).asynchronous(IS_ASYNC).build()
			Return New DoubleDQNBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function setupNStepQLearning(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildDQNNetwork()

			Dim configuration As NStepQLearningBuilder.Configuration = NStepQLearningBuilder.Configuration.builder().policyConfiguration(EpsGreedy.Configuration.builder().epsilonNbStep(75000 \ (If(IS_ASYNC, NUM_THREADS, 1))).minEpsilon(0.1).build()).neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(50).build()).nstepQLearningConfiguration(NStepQLearning.Configuration.builder().build()).experienceHandlerConfiguration(StateActionExperienceHandler.Configuration.builder().batchSize(5).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(200).build()).agentLearnerListeners(listeners).asynchronous(IS_ASYNC).build()
			Return New NStepQLearningBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function setupAdvantageActorCritic(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildActorCriticNetwork()

			Dim configuration As AdvantageActorCriticBuilder.Configuration = AdvantageActorCriticBuilder.Configuration.builder().neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().build()).advantageActorCriticConfiguration(AdvantageActorCritic.Configuration.builder().gamma(0.99).build()).experienceHandlerConfiguration(StateActionExperienceHandler.Configuration.builder().batchSize(5).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(200).build()).agentLearnerListeners(listeners).asynchronous(IS_ASYNC).build()
			Return New AdvantageActorCriticBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function buildDQNNetwork() As ITrainableNeuralNet
			Dim netConf As DQNDenseNetworkConfiguration = DQNDenseNetworkConfiguration.builder().updater(New Adam()).numHiddenNodes(40).numLayers(2).build()
			Dim factory As DQNFactory = New DQNFactoryStdDense(netConf)
			Dim dqnNetwork As IDQN = factory.buildDQN(New Integer() { 4 }, 2)
			Return QNetwork.builder().withNetwork(CType(dqnNetwork.getNeuralNetworks()(0), MultiLayerNetwork)).build()
		End Function

		Private Shared Function buildActorCriticNetwork() As ITrainableNeuralNet
			Dim netConf As ActorCriticDenseNetworkConfiguration = ActorCriticDenseNetworkConfiguration.builder().updater(New Adam()).numHiddenNodes(40).numLayers(2).build()

			If USE_SEPARATE_NETWORKS Then
				Dim factory As New ActorCriticFactorySeparateStdDense(netConf)
				Dim network As ActorCriticSeparate = factory.buildActorCritic(New Integer() { 4 }, 2)
				Return ActorCriticNetwork.builder().withSeparateNetworks(CType(network.getNeuralNetworks()(0), MultiLayerNetwork), CType(network.getNeuralNetworks()(1), MultiLayerNetwork)).build()
			End If

			Dim factory As New ActorCriticFactoryCompGraphStdDense(netConf)
			Dim network As ActorCriticCompGraph = factory.buildActorCritic(New Integer() { 4 }, 2)
			Return ActorCriticNetwork.builder().withCombinedNetwork(CType(network.NeuralNetworks(0), ComputationGraph)).build()
		End Function

		Private Class EpisodeScorePrinter
			Implements AgentListener(Of Integer)

			Friend episodeCount As Integer
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
				Console.WriteLine(String.Format("[{0}] Episode {1,4:D} : score = {2,3:D}", agent.getId(), episodeCount, CInt(Math.Truncate(agent.getReward()))))
				episodeCount += 1
			End Sub
		End Class
	End Class
End Namespace