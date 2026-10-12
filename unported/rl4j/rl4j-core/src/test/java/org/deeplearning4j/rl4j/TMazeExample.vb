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
Imports NStepQLearning = org.deeplearning4j.rl4j.agent.learning.algorithm.nstepqlearning.NStepQLearning
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports org.deeplearning4j.rl4j.agent.listener
Imports AdvantageActorCriticBuilder = org.deeplearning4j.rl4j.builder.AdvantageActorCriticBuilder
Imports NStepQLearningBuilder = org.deeplearning4j.rl4j.builder.NStepQLearningBuilder
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
Imports org.deeplearning4j.rl4j.experience
Imports TMazeEnvironment = org.deeplearning4j.rl4j.mdp.TMazeEnvironment
Imports ActorCriticNetwork = org.deeplearning4j.rl4j.network.ActorCriticNetwork
Imports org.deeplearning4j.rl4j.network
Imports QNetwork = org.deeplearning4j.rl4j.network.QNetwork
Imports ActorCriticLoss = org.deeplearning4j.rl4j.network.ac.ActorCriticLoss
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports ArrayToINDArrayTransform = org.deeplearning4j.rl4j.observation.transform.operation.ArrayToINDArrayTransform
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.trainer
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


	Public Class TMazeExample

		Private Const IS_ASYNC As Boolean = False
		Private Const NUM_THREADS As Integer = 2

		Private Const TMAZE_LENGTH As Integer = 10

		Private Const NUM_INPUTS As Integer = 5
		Private Const NUM_ACTIONS As Integer = 4

		Private Const MIN_EPSILON As Double = 0.1

		Private Const NUM_EPISODES As Integer = 3000

		Public Shared Sub Main(ByVal args() As String)

			Dim rnd As Random = Nd4j.RandomFactory.getNewRandomInstance(123)

			Dim environmentBuilder As Builder(Of Environment(Of Integer)) = Function() New TMazeEnvironment(TMAZE_LENGTH, rnd)
			Dim transformProcessBuilder As Builder(Of TransformProcess) = Function() TransformProcess.builder().transform("data", New ArrayToINDArrayTransform(1, NUM_INPUTS, 1)).build("data")

			Dim listeners As IList(Of AgentListener(Of Integer)) = New ArrayListAnonymousInnerClass()

	'        Builder<IAgentLearner<Integer>> builder = setupNStepQLearning(environmentBuilder, transformProcessBuilder, listeners, rnd);
			Dim builder As Builder(Of IAgentLearner(Of Integer)) = setupAdvantageActorCritic(environmentBuilder, transformProcessBuilder, listeners, rnd)

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
	'			add(New EpisodeScorePrinter(25)); ' compute the success rate with the trailing 25 episodes.
	'		}
		End Class

		Private Shared Function setupNStepQLearning(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildQNetwork()

			Dim configuration As NStepQLearningBuilder.Configuration = NStepQLearningBuilder.Configuration.builder().policyConfiguration(EpsGreedy.Configuration.builder().epsilonNbStep(25000 \ (If(IS_ASYNC, NUM_THREADS, 1))).minEpsilon(MIN_EPSILON).build()).neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(25).build()).nstepQLearningConfiguration(NStepQLearning.Configuration.builder().gamma(0.99).build()).experienceHandlerConfiguration(StateActionExperienceHandler.Configuration.builder().batchSize(Integer.MaxValue).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(40).build()).agentLearnerListeners(listeners).asynchronous(IS_ASYNC).build()
			Return New NStepQLearningBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function setupAdvantageActorCritic(ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal listeners As IList(Of AgentListener(Of Integer)), ByVal rnd As Random) As Builder(Of IAgentLearner(Of Integer))
			Dim network As ITrainableNeuralNet = buildActorCriticNetwork()

			Dim configuration As AdvantageActorCriticBuilder.Configuration = AdvantageActorCriticBuilder.Configuration.builder().neuralNetUpdaterConfiguration(NeuralNetUpdaterConfiguration.builder().build()).advantageActorCriticConfiguration(AdvantageActorCritic.Configuration.builder().gamma(0.99).build()).experienceHandlerConfiguration(StateActionExperienceHandler.Configuration.builder().batchSize(Integer.MaxValue).build()).agentLearnerConfiguration(AgentLearner.Configuration.builder().maxEpisodeSteps(40).build()).agentLearnerListeners(listeners).asynchronous(IS_ASYNC).build()
			Return New AdvantageActorCriticBuilder(configuration, network, environmentBuilder, transformProcessBuilder, rnd)
		End Function

		Private Shared Function buildBaseNetworkConfiguration() As ComputationGraphConfiguration.GraphBuilder
			Return (New NeuralNetConfiguration.Builder()).seed(Constants.NEURAL_NET_SEED).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Adam()).weightInit(WeightInit.XAVIER).graphBuilder().setInputTypes(InputType.recurrent(NUM_INPUTS)).addInputs("input").addLayer("goal", (New LSTM.Builder()).nOut(40).activation(Activation.TANH).build(), "input").addLayer("corridor", (New DenseLayer.Builder()).nOut(40).activation(Activation.RELU).build(), "input", "goal").addLayer("corridor-1", (New DenseLayer.Builder()).nOut(20).activation(Activation.RELU).build(), "corridor").addVertex("corridor-rnn", New PreprocessorVertex(New FeedForwardToRnnPreProcessor()), "corridor-1")
		End Function

		Private Shared Function buildQNetwork() As ITrainableNeuralNet
			Dim conf As ComputationGraphConfiguration = buildBaseNetworkConfiguration().addLayer("output", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(NUM_ACTIONS).build(), "goal", "corridor-rnn").setOutputs("output").build()

			Dim model As New ComputationGraph(conf)
			model.init()
			Return QNetwork.builder().withNetwork(model).build()
		End Function

		Private Shared Function buildActorCriticNetwork() As ITrainableNeuralNet
			Dim conf As ComputationGraphConfiguration = buildBaseNetworkConfiguration().addLayer("value", (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.IDENTITY).nOut(1).build(), "goal", "corridor-rnn").addLayer("softmax", (New RnnOutputLayer.Builder(New ActorCriticLoss())).activation(Activation.SOFTMAX).nOut(NUM_ACTIONS).build(), "goal", "corridor-rnn").setOutputs("value", "softmax").build()

			Dim model As New ComputationGraph(conf)
			model.init()

			Return ActorCriticNetwork.builder().withCombinedNetwork(model).build()
		End Function

		Private Class EpisodeScorePrinter
			Implements AgentListener(Of Integer)

			Friend ReadOnly results() As Boolean
			Friend ReadOnly episodeCount As New AtomicInteger(0)
			Friend ReadOnly trailingNum As Integer

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
				Dim environment As TMazeEnvironment = CType(agent.getEnvironment(), TMazeEnvironment)
				Dim currentEpisodeCount As Integer = episodeCount.getAndIncrement()
				results(currentEpisodeCount Mod trailingNum) = environment.hasNavigatedToSolution()

				Dim stateAtEnd As String
				If environment.hasNavigatedToSolution() Then
					stateAtEnd = "Reached GOAL"
				ElseIf environment.isEpisodeFinished() Then
					stateAtEnd = "Reached TRAP"
				Else
					stateAtEnd = "Did not finish"
				End If

				If currentEpisodeCount >= trailingNum Then
					Dim successCount As Integer = 0
					For i As Integer = 0 To trailingNum - 1
						successCount += If(results(i), 1, 0)
					Next i
					Dim successRatio As Double = successCount / CDbl(trailingNum)
					Console.WriteLine(String.Format("[{0}] Episode {1,4:D} : score = {2,6:F2} success ratio = {3,4:F2} {4}", agent.getId(), currentEpisodeCount, agent.getReward(), successRatio, stateAtEnd))
				Else
					Console.WriteLine(String.Format("[{0}] Episode {1,4:D} : score = {2,6:F2} {3}", agent.getId(), currentEpisodeCount, agent.getReward(), stateAtEnd))
				End If
			End Sub
		End Class
	End Class
End Namespace