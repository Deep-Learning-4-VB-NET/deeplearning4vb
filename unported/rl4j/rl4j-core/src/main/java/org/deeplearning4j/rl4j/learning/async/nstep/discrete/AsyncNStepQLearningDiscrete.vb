Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.learning.async
Imports AsyncQLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.AsyncQLearningConfiguration
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network.dqn
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace

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

Namespace org.deeplearning4j.rl4j.learning.async.nstep.discrete

	Public MustInherit Class AsyncNStepQLearningDiscrete(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits AsyncLearning(Of OBSERVATION, Integer, DiscreteSpace, IDQN)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public final org.deeplearning4j.rl4j.learning.configuration.AsyncQLearningConfiguration configuration;
		Public ReadOnly configuration As AsyncQLearningConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.mdp.MDP<OBSERVATION, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace> mdp;
		Private ReadOnly mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.learning.async.AsyncGlobal<org.deeplearning4j.rl4j.network.dqn.IDQN> asyncGlobal;
		Private ReadOnly asyncGlobal As AsyncGlobal(Of IDQN)


		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal dqn As IDQN, ByVal conf As AsyncQLearningConfiguration)
			Me.mdp = mdp
			Me.configuration = conf
			Me.asyncGlobal = New AsyncGlobal(Of IDQN)(dqn, conf)
		End Sub

		Public Overrides Function newThread(ByVal i As Integer, ByVal deviceNum As Integer) As AsyncThread
			Return New AsyncNStepQLearningThreadDiscrete(mdp.newInstance(), asyncGlobal, configuration, getListeners(), i, deviceNum)
		End Function

		Public Overrides ReadOnly Property NeuralNet As IDQN
			Get
				Return asyncGlobal.Target
			End Get
		End Property

		Public Overrides ReadOnly Property Policy As IPolicy(Of Integer)
			Get
				Return New DQNPolicy(Of OBSERVATION)(NeuralNet)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @AllArgsConstructor @Builder @EqualsAndHashCode(callSuper = false) public static class AsyncNStepQLConfiguration
		Public Class AsyncNStepQLConfiguration

			Friend seed As Integer?
			Friend maxEpochStep As Integer
			Friend maxStep As Integer
			Friend numThread As Integer
			Friend nstep As Integer
			Friend targetDqnUpdateFreq As Integer
			Friend updateStart As Integer
			Friend rewardFactor As Double
			Friend gamma As Double
			Friend errorClamp As Double
			Friend minEpsilon As Single
			Friend epsilonNbStep As Integer

			Public Overridable Function toLearningConfiguration() As AsyncQLearningConfiguration
				Return AsyncQLearningConfiguration.builder().seed(New Long?(seed)).maxEpochStep(maxEpochStep).maxStep(maxStep).numThreads(numThread).nStep(nstep).targetDqnUpdateFreq(targetDqnUpdateFreq).updateStart(updateStart).rewardFactor(rewardFactor).gamma(gamma).errorClamp(errorClamp).minEpsilon(minEpsilon).build()
			End Function

		End Class

	End Class

End Namespace