Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.learning.async
Imports A3CLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.A3CLearningConfiguration
Imports TrainingListenerList = org.deeplearning4j.rl4j.learning.listener.TrainingListenerList
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network.ac
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Random = org.nd4j.linalg.api.rng.Random

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

	Public Class A3CThreadDiscrete(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits AsyncThreadDiscrete(Of OBSERVATION, IActorCritic)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final org.deeplearning4j.rl4j.learning.configuration.A3CLearningConfiguration configuration;
		Protected Friend ReadOnly configuration As A3CLearningConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final IAsyncGlobal<org.deeplearning4j.rl4j.network.ac.IActorCritic> asyncGlobal;
		Protected Friend ReadOnly asyncGlobal As IAsyncGlobal(Of IActorCritic)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final int threadNumber;
		Protected Friend ReadOnly threadNumber As Integer

		Private ReadOnly rnd As Random

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal asyncGlobal As IAsyncGlobal(Of IActorCritic), ByVal a3cc As A3CLearningConfiguration, ByVal deviceNum As Integer, ByVal listeners As TrainingListenerList, ByVal threadNumber As Integer)
			MyBase.New(asyncGlobal, mdp, listeners, threadNumber, deviceNum)
			Me.configuration = a3cc
			Me.asyncGlobal = asyncGlobal
			Me.threadNumber = threadNumber

			Dim seed As Long? = configuration.getSeed()
			rnd = Nd4j.Random
			If seed IsNot Nothing Then
				rnd.setSeed(seed.Value + threadNumber)
			End If

			setUpdateAlgorithm(buildUpdateAlgorithm())
		End Sub

		Protected Friend Overrides Function getPolicy(ByVal net As IActorCritic) As Policy(Of Integer)
			Return New ACPolicy(Of OBSERVATION)(net, True, rnd)
		End Function

		''' <summary>
		''' calc the gradients based on the n-step rewards
		''' </summary>
		Protected Friend Overrides Function buildUpdateAlgorithm() As UpdateAlgorithm(Of IActorCritic)
			Dim shape() As Integer = If(HistoryProcessor Is Nothing, getMdp().getObservationSpace().Shape, HistoryProcessor.getConf().getShape())
			Return New AdvantageActorCriticUpdateAlgorithm(asyncGlobal.Target.isRecurrent(), shape, getMdp().ActionSpace.getSize(), configuration.Gamma)
		End Function
	End Class

End Namespace