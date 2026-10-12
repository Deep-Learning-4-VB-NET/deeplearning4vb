Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.learning.async
Imports AsyncQLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.AsyncQLearningConfiguration
Imports TrainingListenerList = org.deeplearning4j.rl4j.learning.listener.TrainingListenerList
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network.dqn
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
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

Namespace org.deeplearning4j.rl4j.learning.async.nstep.discrete

	Public Class AsyncNStepQLearningThreadDiscrete(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits AsyncThreadDiscrete(Of OBSERVATION, IDQN)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final org.deeplearning4j.rl4j.learning.configuration.AsyncQLearningConfiguration configuration;
		Protected Friend ReadOnly configuration As AsyncQLearningConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final org.deeplearning4j.rl4j.learning.async.IAsyncGlobal<org.deeplearning4j.rl4j.network.dqn.IDQN> asyncGlobal;
		Protected Friend ReadOnly asyncGlobal As IAsyncGlobal(Of IDQN)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final int threadNumber;
		Protected Friend ReadOnly threadNumber As Integer

		Private ReadOnly rnd As Random

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal asyncGlobal As IAsyncGlobal(Of IDQN), ByVal configuration As AsyncQLearningConfiguration, ByVal listeners As TrainingListenerList, ByVal threadNumber As Integer, ByVal deviceNum As Integer)
			MyBase.New(asyncGlobal, mdp, listeners, threadNumber, deviceNum)
			Me.configuration = configuration
			Me.asyncGlobal = asyncGlobal
			Me.threadNumber = threadNumber
			rnd = Nd4j.Random

			Dim seed As Long? = configuration.getSeed()
			If seed IsNot Nothing Then
				rnd.setSeed(seed.Value + threadNumber)
			End If

			setUpdateAlgorithm(buildUpdateAlgorithm())
		End Sub

		Public Overridable Overloads Function getPolicy(ByVal nn As IDQN) As Policy(Of Integer)
			Return New EpsGreedy(New DQNPolicy(nn), getMdp(), configuration.getUpdateStart(), configuration.getEpsilonNbStep(), rnd, configuration.getMinEpsilon(), Me)
		End Function

		Protected Friend Overrides Function buildUpdateAlgorithm() As UpdateAlgorithm(Of IDQN)
			Return New QLearningUpdateAlgorithm(getMdp().ActionSpace.getSize(), configuration.Gamma)
		End Function
	End Class

End Namespace