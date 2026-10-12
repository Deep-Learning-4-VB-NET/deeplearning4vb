Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.learning.async
Imports A3CLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.A3CLearningConfiguration
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network.ac
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.policy
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
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

Namespace org.deeplearning4j.rl4j.learning.async.a3c.discrete

	Public MustInherit Class A3CDiscrete(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits AsyncLearning(Of OBSERVATION, Integer, DiscreteSpace, IActorCritic)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public final org.deeplearning4j.rl4j.learning.configuration.A3CLearningConfiguration configuration;
		Public ReadOnly configuration As A3CLearningConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final org.deeplearning4j.rl4j.mdp.MDP<OBSERVATION, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace> mdp;
		Protected Friend ReadOnly mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace)
		Private ReadOnly iActorCritic As IActorCritic
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.learning.async.AsyncGlobal asyncGlobal;
		Private ReadOnly asyncGlobal As AsyncGlobal
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.policy.ACPolicy<OBSERVATION> policy;
		Private ReadOnly policy As ACPolicy(Of OBSERVATION)

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal iActorCritic As IActorCritic, ByVal conf As A3CLearningConfiguration)
			Me.iActorCritic = iActorCritic
			Me.mdp = mdp
			Me.configuration = conf
			asyncGlobal = New AsyncGlobal(Of )(iActorCritic, conf)

			Dim seed As Long? = conf.getSeed()
			Dim rnd As Random = Nd4j.Random
			If seed IsNot Nothing Then
				rnd.Seed = seed
			End If

			policy = New ACPolicy(Of OBSERVATION)(iActorCritic, True, rnd)
		End Sub

		Protected Friend Overrides Function newThread(ByVal i As Integer, ByVal deviceNum As Integer) As AsyncThread
			Return New A3CThreadDiscrete(mdp.newInstance(), asyncGlobal, Me.Configuration, deviceNum, getListeners(), i)
		End Function

		Public Overrides ReadOnly Property NeuralNet As IActorCritic
			Get
				Return iActorCritic
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Builder @EqualsAndHashCode(callSuper = false) public static class A3CConfiguration
		Public Class A3CConfiguration

			Friend seed As Integer
			Friend maxEpochStep As Integer
			Friend maxStep As Integer
			Friend numThread As Integer
			Friend nstep As Integer
			Friend updateStart As Integer
			Friend rewardFactor As Double
			Friend gamma As Double
			Friend errorClamp As Double

			''' <summary>
			''' Converts the deprecated A3CConfiguration to the new LearningConfiguration format
			''' </summary>
			Public Overridable Function toLearningConfiguration() As A3CLearningConfiguration
				Return A3CLearningConfiguration.builder().seed(New Long?(seed)).maxEpochStep(maxEpochStep).maxStep(maxStep).numThreads(numThread).nStep(nstep).rewardFactor(rewardFactor).gamma(gamma).build()

			End Function

		End Class
	End Class

End Namespace