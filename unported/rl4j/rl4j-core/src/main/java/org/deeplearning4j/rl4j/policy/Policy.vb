Imports System
Imports org.deeplearning4j.gym
Imports HistoryProcessor = org.deeplearning4j.rl4j.learning.HistoryProcessor
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.mdp
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
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

Namespace org.deeplearning4j.rl4j.policy

	Public MustInherit Class Policy(Of A)
		Implements INeuralNetPolicy(Of A)

		Public MustOverride ReadOnly Property NeuralNet As IOutputNeuralNet Implements INeuralNetPolicy(Of A).getNeuralNet

		Public MustOverride Function nextAction(ByVal obs As Observation) As A

		<Obsolete>
		Public Overridable Function play(Of O As Encodable, [AS] As ActionSpace(Of A))(ByVal mdp As MDP(Of O, A, [AS])) As Double
			Return play(mdp, DirectCast(Nothing, IHistoryProcessor))
		End Function

		<Obsolete>
		Public Overridable Function play(Of O As Encodable, [AS] As ActionSpace(Of A))(ByVal mdp As MDP(Of O, A, [AS]), ByVal conf As HistoryProcessor.Configuration) As Double
			Return play(mdp, New HistoryProcessor(conf))
		End Function

		<Obsolete>
		Public Overridable Function play(Of O As Encodable, [AS] As ActionSpace(Of A))(ByVal mdp As MDP(Of O, A, [AS]), ByVal hp As IHistoryProcessor) As Double Implements org.deeplearning4j.rl4j.policy.IPolicy(Of A).play
			resetNetworks()

			Dim mdpWrapper As New LegacyMDPWrapper(Of O, A, [AS])(mdp, hp)

			Dim initMdp As Learning.InitMdp(Of Observation) = refacInitMdp(mdpWrapper, hp)
			Dim obs As Observation = initMdp.getLastObs()

			Dim reward As Double = initMdp.getReward()

			Dim lastAction As A = mdpWrapper.ActionSpace.noOp()
			Dim action As A

			Do While Not mdpWrapper.Done

				If obs.Skipped Then
					action = lastAction
				Else
					action = nextAction(obs)
				End If

				lastAction = action

				Dim stepReply As StepReply(Of Observation) = mdpWrapper.step(action)
				reward += stepReply.getReward()

				obs = stepReply.getObservation()
			Loop

			Return reward
		End Function

		Protected Friend Overridable Sub resetNetworks()
			NeuralNet.reset()
		End Sub
		Public Overridable Sub reset() Implements org.deeplearning4j.rl4j.policy.IPolicy(Of A).reset
			resetNetworks()
		End Sub

		Protected Friend Overridable Function refacInitMdp(Of O As Encodable, [AS] As ActionSpace(Of A))(ByVal mdpWrapper As LegacyMDPWrapper(Of O, A, [AS]), ByVal hp As IHistoryProcessor) As Learning.InitMdp(Of Observation)

			Dim reward As Double = 0

			Dim observation As Observation = mdpWrapper.reset()

			Dim action As A = mdpWrapper.ActionSpace.noOp() 'by convention should be the NO_OP
			Do While observation.Skipped AndAlso Not mdpWrapper.Done

				Dim stepReply As StepReply(Of Observation) = mdpWrapper.step(action)

				reward += stepReply.getReward()
				observation = stepReply.getObservation()

			Loop

			Return New Learning.InitMdp(0, observation, reward)
		End Function
	End Class

End Namespace