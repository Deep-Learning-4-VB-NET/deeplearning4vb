Imports System
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.rl4j.environment
Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports org.deeplearning4j.rl4j.mdp
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
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

Namespace org.deeplearning4j.rl4j.policy

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EpsGreedy<A> extends Policy<A>
	Public Class EpsGreedy(Of A)
		Inherits Policy(Of A)

		Private ReadOnly policy As INeuralNetPolicy(Of A)
		Private ReadOnly annealingStart As Integer
		Private ReadOnly epsilonNbStep As Integer
		Private ReadOnly rnd As Random
		Private ReadOnly minEpsilon As Double

		Private ReadOnly actionSchema As IActionSchema(Of A)

		Private ReadOnly mdp As MDP(Of Encodable, A, ActionSpace(Of A))
		Private ReadOnly learning As IEpochTrainer

		' Using agent's (learning's) step count is incorrect; frame skipping makes epsilon's value decrease too quickly
		Private annealingStep As Integer = 0

		<Obsolete>
		Public Sub New(Of OBSERVATION As Encodable, [AS] As ActionSpace(Of A))(ByVal policy As Policy(Of A), ByVal mdp As MDP(Of Encodable, A, ActionSpace(Of A)), ByVal annealingStart As Integer, ByVal epsilonNbStep As Integer, ByVal rnd As Random, ByVal minEpsilon As Double, ByVal learning As IEpochTrainer)
			Me.policy = policy
			Me.mdp = mdp
			Me.annealingStart = annealingStart
			Me.epsilonNbStep = epsilonNbStep
			Me.rnd = rnd
			Me.minEpsilon = minEpsilon
			Me.learning = learning

			Me.actionSchema = Nothing
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EpsGreedy(@NonNull Policy<A> policy, @NonNull IActionSchema<A> actionSchema, double minEpsilon, int annealingStart, int epsilonNbStep)
		Public Sub New(ByVal policy As Policy(Of A), ByVal actionSchema As IActionSchema(Of A), ByVal minEpsilon As Double, ByVal annealingStart As Integer, ByVal epsilonNbStep As Integer)
			Me.New(policy, actionSchema, minEpsilon, annealingStart, epsilonNbStep, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public EpsGreedy(@NonNull INeuralNetPolicy<A> policy, @NonNull IActionSchema<A> actionSchema, double minEpsilon, int annealingStart, int epsilonNbStep, org.nd4j.linalg.api.rng.Random rnd)
		Public Sub New(ByVal policy As INeuralNetPolicy(Of A), ByVal actionSchema As IActionSchema(Of A), ByVal minEpsilon As Double, ByVal annealingStart As Integer, ByVal epsilonNbStep As Integer, ByVal rnd As Random)
			Me.policy = policy

			Me.rnd = If(rnd Is Nothing, Nd4j.Random, rnd)
			Me.minEpsilon = minEpsilon
			Me.annealingStart = annealingStart
			Me.epsilonNbStep = epsilonNbStep
			Me.actionSchema = actionSchema

			Me.mdp = Nothing
			Me.learning = Nothing
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EpsGreedy(INeuralNetPolicy<A> policy, org.deeplearning4j.rl4j.environment.IActionSchema<A> actionSchema, @NonNull Configuration configuration, org.nd4j.linalg.api.rng.Random rnd)
		Public Sub New(ByVal policy As INeuralNetPolicy(Of A), ByVal actionSchema As IActionSchema(Of A), ByVal configuration As Configuration, ByVal rnd As Random)
			Me.New(policy, actionSchema, configuration.getMinEpsilon(), configuration.getAnnealingStart(), configuration.getEpsilonNbStep(), rnd)
		End Sub

		Public Overrides ReadOnly Property NeuralNet As IOutputNeuralNet
			Get
				Return policy.NeuralNet
			End Get
		End Property

		<Obsolete>
		Public Overridable Overloads Function nextAction(ByVal input As INDArray) As A

			Dim ep As Double = Epsilon
			If actionSchema IsNot Nothing Then
				' Only legacy classes should pass here.
				Throw New Exception("nextAction(Observation observation) should be called when using a AgentLearner")
			End If

			If learning.StepCount Mod 500 = 1 Then
				log.info("EP: " & ep & " " & learning.StepCount)
			End If
			If rnd.nextDouble() > ep Then
				Return policy.nextAction(input)
			Else
				Return mdp.ActionSpace.randomAction()
			End If
		End Function

		Public Overrides Function nextAction(ByVal observation As Observation) As A
			' FIXME: remove if() and content once deprecated methods are removed.
			If actionSchema Is Nothing Then
				Return Me.nextAction(observation.getChannelData(0))
			End If

			Dim ep As Double = Epsilon
			If annealingStep Mod 500 = 1 Then
				log.info("EP: " & ep & " " & annealingStep)
			End If

			annealingStep += 1

			' TODO: This is a temporary solution while something better is developed
			If rnd.nextDouble() > ep Then
				Return policy.nextAction(observation)
			End If
			' With RNNs the neural net must see *all* observations
			If NeuralNet.Recurrent Then
				policy.nextAction(observation) ' Make the RNN see the observation
			End If
			Return actionSchema.RandomAction
		End Function

		Public Overridable ReadOnly Property Epsilon As Double
			Get
				Dim [step] As Integer = If(actionSchema IsNot Nothing, annealingStep, learning.StepCount)
				Return Math.Min(1.0, Math.Max(minEpsilon, 1.0 - ([step] - annealingStart) * 1.0 / epsilonNbStep))
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration
		Public Class Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default final int annealingStart = 0;
			Friend ReadOnly annealingStart As Integer = 0

			Friend ReadOnly epsilonNbStep As Integer
			Friend ReadOnly minEpsilon As Double
		End Class
	End Class

End Namespace