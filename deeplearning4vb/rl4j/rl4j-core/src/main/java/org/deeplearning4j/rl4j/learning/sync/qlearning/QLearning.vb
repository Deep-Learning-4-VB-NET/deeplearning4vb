Imports System
Imports System.Collections.Generic
Imports JsonDeserialize = com.fasterxml.jackson.databind.annotation.JsonDeserialize
Imports JsonPOJOBuilder = com.fasterxml.jackson.databind.annotation.JsonPOJOBuilder
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Value = lombok.Value
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.gym
Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports QLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.QLearningConfiguration
Imports org.deeplearning4j.rl4j.learning.sync
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network.dqn
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.space
Imports StatEntry = org.deeplearning4j.rl4j.util.IDataManager.StatEntry
Imports org.deeplearning4j.rl4j.util

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

Namespace org.deeplearning4j.rl4j.learning.sync.qlearning


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class QLearning<O extends org.deeplearning4j.rl4j.space.Encodable, A, @AS extends org.deeplearning4j.rl4j.space.ActionSpace<A>> extends org.deeplearning4j.rl4j.learning.sync.SyncLearning<O, A, @AS, org.deeplearning4j.rl4j.network.dqn.IDQN> implements org.deeplearning4j.rl4j.learning.IEpochTrainer
	Public MustInherit Class QLearning(Of O As org.deeplearning4j.rl4j.space.Encodable, A, [AS] As org.deeplearning4j.rl4j.space.ActionSpace(Of A))
		Inherits SyncLearning(Of O, A, [AS], IDQN)
		Implements IEpochTrainer

		Public Overrides MustOverride ReadOnly Property CurrentEpisodeStepCount As Integer Implements IEpochTrainer.getCurrentEpisodeStepCount
		Public Overrides MustOverride ReadOnly Property EpisodeCount As Integer Implements IEpochTrainer.getEpisodeCount
		Public Overrides MustOverride ReadOnly Property EpochCount As Integer Implements IEpochTrainer.getEpochCount
		Public Overrides MustOverride ReadOnly Property HistoryProcessor As IHistoryProcessor Implements IEpochTrainer.getHistoryProcessor
		Public Overrides MustOverride ReadOnly Property StepCount As Integer Implements IEpochTrainer.getStepCount
		Public Overrides MustOverride ReadOnly Property Policy As org.deeplearning4j.rl4j.policy.IPolicy(Of A)

		Protected Friend MustOverride ReadOnly Property LegacyMDPWrapper As LegacyMDPWrapper(Of O, A, [AS])

		Protected Friend MustOverride ReadOnly Property EgPolicy As EpsGreedy(Of A)

		Public Overrides MustOverride ReadOnly Property Mdp As MDP(Of O, A, [AS])

		Public MustOverride ReadOnly Property QNetwork As IDQN

		Public Overrides ReadOnly Property NeuralNet As IDQN
			Get
				Return QNetwork
			End Get
		End Property

		Public Overrides MustOverride ReadOnly Property Configuration As QLearningConfiguration

		Protected Friend Overrides MustOverride Sub preEpoch()

		Protected Friend Overrides MustOverride Sub postEpoch()

		Protected Friend MustOverride Function trainStep(ByVal obs As Observation) As QLStepReturn(Of Observation)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int episodeCount;
		Private episodeCount As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int currentEpisodeStepCount = 0;
		Private currentEpisodeStepCount As Integer = 0

		Protected Friend Overrides Function trainEpoch() As StatEntry
			resetNetworks()

			Dim initMdp As InitMdp(Of Observation) = refacInitMdp()
			Dim obs As Observation = initMdp.getLastObs()

			Dim reward As Double = initMdp.getReward()

			Dim startQ As Double? = Double.NaN
			Dim meanQ As Double = 0
			Dim numQ As Integer = 0
			Dim scores As IList(Of Double) = New List(Of Double)()
			Do While currentEpisodeStepCount < Configuration.MaxEpochStep AndAlso Not getMdp().Done
				Dim stepR As QLStepReturn(Of Observation) = trainStep(obs)

				If Not stepR.getMaxQ().isNaN() Then
					If startQ.isNaN() Then
						startQ = stepR.getMaxQ()
					End If
					numQ += 1
					meanQ += stepR.getMaxQ()
				End If

				If stepR.getScore() <> 0 Then
					scores.Add(stepR.getScore())
				End If

				reward += stepR.getStepReply().getReward()
				obs = stepR.getStepReply().getObservation()
				incrementStep()
			Loop

			finishEpoch(obs)

			meanQ /= (numQ + 0.001) 'avoid div zero


			Dim statEntry As StatEntry = New QLStatEntry(Me.StepCount, EpochCount, reward, currentEpisodeStepCount, scores, getEgPolicy().Epsilon, startQ, meanQ)

			Return statEntry
		End Function

		Protected Friend Overridable Sub finishEpoch(ByVal observation As Observation)
			episodeCount += 1
		End Sub

		Public Overrides Sub incrementStep()
			MyBase.incrementStep()
			currentEpisodeStepCount += 1
		End Sub

		Protected Friend Overridable Sub resetNetworks()
			QNetwork.reset()
		End Sub

		Private Function refacInitMdp() As InitMdp(Of Observation)
			currentEpisodeStepCount = 0

			Dim reward As Double = 0

			Dim mdp As LegacyMDPWrapper(Of O, A, [AS]) = getLegacyMDPWrapper()
			Dim observation As Observation = mdp.reset()

			Dim action As A = mdp.ActionSpace.noOp() 'by convention should be the NO_OP
			Do While observation.Skipped AndAlso Not mdp.Done
				Dim stepReply As StepReply(Of Observation) = mdp.step(action)

				reward += stepReply.getReward()
				observation = stepReply.getObservation()

				incrementStep()
			Loop

			Return New InitMdp(0, observation, reward)

		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Value public static class QLStatEntry implements org.deeplearning4j.rl4j.util.IDataManager.StatEntry
		Public Class QLStatEntry
			Implements StatEntry

			Friend stepCounter As Integer
			Friend epochCounter As Integer
			Friend reward As Double
			Friend episodeLength As Integer
			Friend scores As IList(Of Double)
			Friend epsilon As Double
			Friend startQ As Double
			Friend meanQ As Double
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Value public static class QLStepReturn<O>
		Public Class QLStepReturn(Of O)
			Friend maxQ As Double?
			Friend score As Double
			Friend stepReply As StepReply(Of O)

		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Builder @Deprecated @EqualsAndHashCode(callSuper = false) @JsonDeserialize(builder = QLConfiguration.QLConfigurationBuilder.class) public static class QLConfiguration
		<Obsolete>
		Public Class QLConfiguration

			Friend seed As Integer?
			Friend maxEpochStep As Integer
			Friend maxStep As Integer
			Friend expRepMaxSize As Integer
			Friend batchSize As Integer
			Friend targetDqnUpdateFreq As Integer
			Friend updateStart As Integer
			Friend rewardFactor As Double
			Friend gamma As Double
			Friend errorClamp As Double
			Friend minEpsilon As Single
			Friend epsilonNbStep As Integer
			Friend doubleDQN As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonPOJOBuilder(withPrefix = "") public static final class QLConfigurationBuilder
			Public NotInheritable Class QLConfigurationBuilder
			End Class

			Public Overridable Function toLearningConfiguration() As QLearningConfiguration

				Return QLearningConfiguration.builder().seed(seed.Value).maxEpochStep(maxEpochStep).maxStep(maxStep).expRepMaxSize(expRepMaxSize).batchSize(batchSize).targetDqnUpdateFreq(targetDqnUpdateFreq).updateStart(updateStart).rewardFactor(rewardFactor).gamma(gamma).errorClamp(errorClamp).minEpsilon(minEpsilon).epsilonNbStep(epsilonNbStep).doubleDQN(doubleDQN).build()
			End Function
		End Class

	End Class

End Namespace