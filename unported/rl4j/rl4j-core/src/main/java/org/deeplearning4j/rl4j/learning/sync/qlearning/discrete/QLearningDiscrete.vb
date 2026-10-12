Imports Getter = lombok.Getter
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports BaseTransitionTDAlgorithm = org.deeplearning4j.rl4j.agent.learning.algorithm.dqn.BaseTransitionTDAlgorithm
Imports DoubleDQN = org.deeplearning4j.rl4j.agent.learning.algorithm.dqn.DoubleDQN
Imports StandardDQN = org.deeplearning4j.rl4j.agent.learning.algorithm.dqn.StandardDQN
Imports org.deeplearning4j.rl4j.agent.learning.behavior
Imports org.deeplearning4j.rl4j.agent.learning.behavior
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports org.deeplearning4j.rl4j.agent.learning.update
Imports org.deeplearning4j.rl4j.agent.learning.update
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports SyncLabelsNeuralNetUpdater = org.deeplearning4j.rl4j.agent.learning.update.updater.sync.SyncLabelsNeuralNetUpdater
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.learning
Imports QLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.QLearningConfiguration
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.learning.sync.qlearning
Imports org.deeplearning4j.rl4j.mdp
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports org.deeplearning4j.rl4j.network
Imports org.deeplearning4j.rl4j.network.dqn
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.util
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.rl4j.learning.sync.qlearning.discrete


	Public MustInherit Class QLearningDiscrete(Of O As org.deeplearning4j.rl4j.space.Encodable)
		Inherits QLearning(Of O, Integer, DiscreteSpace)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.learning.configuration.QLearningConfiguration configuration;
		Private ReadOnly configuration As QLearningConfiguration
		Private ReadOnly mdp As LegacyMDPWrapper(Of O, Integer, DiscreteSpace)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.policy.DQNPolicy<O> policy;
		Private policy As DQNPolicy(Of O)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.policy.EpsGreedy<Integer> egPolicy;
		Private egPolicy As EpsGreedy(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.network.dqn.IDQN qNetwork;
		Private ReadOnly qNetwork As IDQN

		Private lastAction As Integer
		Private accuReward As Double = 0

		Private ReadOnly learningBehavior As ILearningBehavior(Of Integer)

		Protected Friend Overrides ReadOnly Property LegacyMDPWrapper As LegacyMDPWrapper(Of O, Integer, DiscreteSpace)
			Get
				Return mdp
			End Get
		End Property

		Public Sub New(ByVal mdp As MDP(Of O, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal conf As QLearningConfiguration, ByVal epsilonNbStep As Integer)
			Me.New(mdp, dqn, conf, epsilonNbStep, Nd4j.RandomFactory.getNewRandomInstance(conf.getSeed()))
		End Sub

		Public Sub New(ByVal mdp As MDP(Of O, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal conf As QLearningConfiguration, ByVal epsilonNbStep As Integer, ByVal random As Random)
			Me.New(mdp, dqn, conf, epsilonNbStep, buildLearningBehavior(dqn, conf, random), random)
		End Sub

		Public Sub New(ByVal mdp As MDP(Of O, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal conf As QLearningConfiguration, ByVal epsilonNbStep As Integer, ByVal learningBehavior As ILearningBehavior(Of Integer), ByVal random As Random)
			Me.configuration = conf
			Me.mdp = New LegacyMDPWrapper(Of O, Integer, DiscreteSpace)(mdp, Nothing)
			qNetwork = dqn
			policy = New DQNPolicy(QNetwork)
			egPolicy = New EpsGreedy(policy, mdp, conf.getUpdateStart(), epsilonNbStep, random, conf.getMinEpsilon(), Me)

			Me.learningBehavior = learningBehavior
		End Sub

		Private Shared Function buildLearningBehavior(ByVal qNetwork As IDQN, ByVal conf As QLearningConfiguration, ByVal random As Random) As ILearningBehavior(Of Integer)
			Dim target As ITrainableNeuralNet = qNetwork.clone()
			Dim aglorithmConfiguration As BaseTransitionTDAlgorithm.Configuration = BaseTransitionTDAlgorithm.Configuration.builder().gamma(conf.Gamma).errorClamp(conf.getErrorClamp()).build()
			Dim updateAlgorithm As IUpdateAlgorithm(Of FeaturesLabels, StateActionRewardState(Of Integer)) = If(conf.isDoubleDQN(), New DoubleDQN(qNetwork, target, aglorithmConfiguration), New StandardDQN(qNetwork, target, aglorithmConfiguration))

			Dim neuralNetUpdaterConfiguration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(conf.getTargetDqnUpdateFreq()).build()
			Dim updater As INeuralNetUpdater(Of FeaturesLabels) = New SyncLabelsNeuralNetUpdater(qNetwork, target, neuralNetUpdaterConfiguration)
			Dim updateRule As IUpdateRule(Of StateActionRewardState(Of Integer)) = New UpdateRule(Of FeaturesLabels, StateActionRewardState(Of Integer))(updateAlgorithm, updater)

			Dim experienceHandlerConfiguration As ReplayMemoryExperienceHandler.Configuration = ReplayMemoryExperienceHandler.Configuration.builder().maxReplayMemorySize(conf.getExpRepMaxSize()).batchSize(conf.getBatchSize()).build()
			Dim experienceHandler As ExperienceHandler(Of Integer, StateActionRewardState(Of Integer)) = New ReplayMemoryExperienceHandler(experienceHandlerConfiguration, random)
			Return LearningBehavior.builder(Of Integer, StateActionRewardState(Of Integer))().experienceHandler(experienceHandler).updateRule(updateRule).build()
		End Function

		Public Overrides ReadOnly Property Mdp As MDP(Of O, Integer, DiscreteSpace)
			Get
				Return mdp.getWrappedMDP()
			End Get
		End Property

		Public Overrides Sub postEpoch()

			If HistoryProcessor IsNot Nothing Then
				HistoryProcessor.stopMonitor()
			End If

		End Sub

		Public Overrides Sub preEpoch()
			lastAction = mdp.ActionSpace.noOp()
			accuReward = 0
			learningBehavior.handleEpisodeStart()
		End Sub

		Public Overrides WriteOnly Property HistoryProcessor As IHistoryProcessor
			Set(ByVal historyProcessor As IHistoryProcessor)
				MyBase.HistoryProcessor = historyProcessor
				mdp.HistoryProcessor = historyProcessor
			End Set
		End Property

		''' <summary>
		''' Single step of training
		''' </summary>
		''' <param name="obs"> last obs </param>
		''' <returns> relevant info for next step </returns>
		Protected Friend Overrides Function trainStep(ByVal obs As Observation) As QLStepReturn(Of Observation)

			Dim maxQ As Double? = Double.NaN 'ignore if Nan for stats

			'if step of training, just repeat lastAction
			If Not obs.Skipped Then
				Dim qs As INDArray = QNetwork.output(obs).get(CommonOutputNames.QValues)
				Dim maxAction As Integer = Learning.getMaxAction(qs)
				maxQ = qs.getDouble(maxAction)

				lastAction = getEgPolicy().nextAction(obs)
			End If

			Dim stepReply As StepReply(Of Observation) = mdp.step(lastAction)
			accuReward += stepReply.getReward() * configuration.RewardFactor

			'if it's not a skipped frame, you can do a step of training
			If Not obs.Skipped Then

				' Add experience
				learningBehavior.handleNewExperience(obs, lastAction, accuReward, stepReply.isDone())
				accuReward = 0
			End If

			Return New QLStepReturn(Of Observation)(maxQ, QNetwork.getLatestScore(), stepReply)
		End Function

		Protected Friend Overrides Sub finishEpoch(ByVal observation As Observation)
			learningBehavior.handleEpisodeEnd(observation)
		End Sub
	End Class

End Namespace