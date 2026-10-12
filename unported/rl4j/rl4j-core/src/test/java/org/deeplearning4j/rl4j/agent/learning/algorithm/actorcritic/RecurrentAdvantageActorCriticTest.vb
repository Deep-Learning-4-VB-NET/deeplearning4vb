Imports System.Collections.Generic
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports org.deeplearning4j.rl4j.experience
Imports CommonLabelNames = org.deeplearning4j.rl4j.network.CommonLabelNames
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports org.deeplearning4j.rl4j.network
Imports NeuralNetOutput = org.deeplearning4j.rl4j.network.NeuralNetOutput
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
Imports Mock = org.mockito.Mock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
import static org.junit.jupiter.api.Assertions.assertEquals
Imports org.mockito.Mockito

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

Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.actorcritic


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class RecurrentAdvantageActorCriticTest
	Public Class RecurrentAdvantageActorCriticTest
		Private Const ACTION_SPACE_SIZE As Integer = 2
		Private Const GAMMA As Double = 0.99

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet threadCurrentMock;
		Friend threadCurrentMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock AdvantageActorCritic.Configuration configurationMock;
		Friend configurationMock As AdvantageActorCritic.Configuration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock NeuralNetOutput neuralNetOutputMock;
		Friend neuralNetOutputMock As NeuralNetOutput

		Private sut As AdvantageActorCritic

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void init()
		Public Overridable Sub init()
			[when](neuralNetOutputMock.get(CommonOutputNames.ActorCritic.Value)).thenReturn(Nd4j.create(New Double() { 123.0 }))
			[when](configurationMock.getGamma()).thenReturn(GAMMA)
			[when](threadCurrentMock.isRecurrent()).thenReturn(True)

			sut = New AdvantageActorCritic(threadCurrentMock, ACTION_SPACE_SIZE, configurationMock)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_observationIsTerminal_expect_initialRIsZero()
		Public Overridable Sub when_observationIsTerminal_expect_initialRIsZero()
			' Arrange
			Dim action As Integer = 0
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray data = org.nd4j.linalg.factory.Nd4j.zeros(1, 2, 1);
			Dim data As INDArray = Nd4j.zeros(1, 2, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.rl4j.observation.Observation observation = new org.deeplearning4j.rl4j.observation.Observation(data);
			Dim observation As New Observation(data)
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass(Me, action, observation)
			[when](threadCurrentMock.output(observation)).thenReturn(neuralNetOutputMock)

			' Act
			sut.compute(experience)

			' Assert
			Dim argument As ArgumentCaptor(Of FeaturesLabels) = ArgumentCaptor.forClass(GetType(FeaturesLabels))
			verify(threadCurrentMock, times(1)).computeGradients(argument.capture())

			Dim featuresLabels As FeaturesLabels = argument.getValue()
			assertEquals(0.0, featuresLabels.getLabels(CommonLabelNames.ActorCritic.Value).getDouble(0), 0.000001)
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As RecurrentAdvantageActorCriticTest

			Private action As Integer
			Private observation As Observation

			Public Sub New(ByVal outerInstance As RecurrentAdvantageActorCriticTest, ByVal action As Integer, ByVal observation As Observation)
				Me.outerInstance = outerInstance
				Me.action = action
				Me.observation = observation

				Me.add(New StateActionReward(Of Integer)(observation, action, 0.0, True))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_observationNonTerminal_expect_initialRIsGammaTimesOutputOfValue()
		Public Overridable Sub when_observationNonTerminal_expect_initialRIsGammaTimesOutputOfValue()
			' Arrange
			Dim action As Integer = 0
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray data = org.nd4j.linalg.factory.Nd4j.zeros(1, 2, 1);
			Dim data As INDArray = Nd4j.zeros(1, 2, 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.rl4j.observation.Observation observation = new org.deeplearning4j.rl4j.observation.Observation(data);
			Dim observation As New Observation(data)
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass2(Me, action, observation)
			[when](threadCurrentMock.output(observation)).thenReturn(neuralNetOutputMock)

			' Act
			sut.compute(experience)

			' Assert
			Dim argument As ArgumentCaptor(Of FeaturesLabels) = ArgumentCaptor.forClass(GetType(FeaturesLabels))
			verify(threadCurrentMock, times(1)).computeGradients(argument.capture())

			Dim featuresLabels As FeaturesLabels = argument.getValue()
			assertEquals(0.0 + GAMMA * 123.0, featuresLabels.getLabels(CommonLabelNames.ActorCritic.Value).getDouble(0), 0.00001)
		End Sub

		Private Class ArrayListAnonymousInnerClass2
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As RecurrentAdvantageActorCriticTest

			Private action As Integer
			Private observation As Observation

			Public Sub New(ByVal outerInstance As RecurrentAdvantageActorCriticTest, ByVal action As Integer, ByVal observation As Observation)
				Me.outerInstance = outerInstance
				Me.action = action
				Me.observation = observation

				Me.add(New StateActionReward(Of Integer)(observation, action, 0.0, False))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCompute_expect_valueAndPolicyComputedCorrectly()
		Public Overridable Sub when_callingCompute_expect_valueAndPolicyComputedCorrectly()
			' Arrange
			Dim action As Integer = 0
			[when](threadCurrentMock.output(any(GetType(Observation)))).thenAnswer(Function(invocation)
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.ActorCritic.Value, invocation.getArgument(0, GetType(Observation)).getData().get(NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.all()).mul(-1.0))
			result.put(CommonOutputNames.ActorCritic.Policy, invocation.getArgument(0, GetType(Observation)).getData().mul(-0.1))
			Return result
			End Function)
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass3(Me)

			' Act
			sut.compute(experience)

			' Assert
			Dim argument As ArgumentCaptor(Of FeaturesLabels) = ArgumentCaptor.forClass(GetType(FeaturesLabels))
			verify(threadCurrentMock, times(1)).computeGradients(argument.capture())

			' input side -- should be a stack of observations
			Dim featuresValues As INDArray = argument.getValue().getFeatures().get(0)
			assertEquals(-1.1, featuresValues.getDouble(0, 0, 0), 0.00001)
			assertEquals(-1.2, featuresValues.getDouble(0, 1, 0), 0.00001)
			assertEquals(-2.1, featuresValues.getDouble(0, 0, 1), 0.00001)
			assertEquals(-2.2, featuresValues.getDouble(0, 1, 1), 0.00001)

			' Value
			Dim valueLabels As INDArray = argument.getValue().getLabels(CommonLabelNames.ActorCritic.Value)
			assertEquals(1.0 + GAMMA * (2.0 + GAMMA * 2.1), valueLabels.getDouble(0, 0, 0), 0.00001)
			assertEquals(2.0 + GAMMA * 2.1, valueLabels.getDouble(0, 0, 1), 0.00001)

			' Policy
			Dim policyLabels As INDArray = argument.getValue().getLabels(CommonLabelNames.ActorCritic.Policy)
			assertEquals((1.0 + GAMMA * (2.0 + GAMMA * 2.1)) - 1.1, policyLabels.getDouble(0, 0, 0), 0.00001)
			assertEquals((2.0 + GAMMA * 2.1) - 2.1, policyLabels.getDouble(0, 1, 1), 0.00001)

		End Sub

		Private Class ArrayListAnonymousInnerClass3
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As RecurrentAdvantageActorCriticTest

			Public Sub New(ByVal outerInstance As RecurrentAdvantageActorCriticTest)
				Me.outerInstance = outerInstance

				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { -1.1, -1.2 }).reshape(ChrW(1), 2, 1)), 0, 1.0, False))
				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { -2.1, -2.2 }).reshape(ChrW(1), 2, 1)), 1, 2.0, False))
			End Sub

		End Class
	End Class

End Namespace