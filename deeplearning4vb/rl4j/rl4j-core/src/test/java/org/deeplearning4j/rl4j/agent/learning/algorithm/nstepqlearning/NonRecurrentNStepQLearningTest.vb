Imports System.Collections.Generic
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.network
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports RunWith = org.junit.runner.RunWith
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
Imports Mock = org.mockito.Mock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.nstepqlearning


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) @Disabled("mockito") @Tag(TagNames.FILE_IO) @NativeTag public class NonRecurrentNStepQLearningTest
	Public Class NonRecurrentNStepQLearningTest

		Private Const ACTION_SPACE_SIZE As Integer = 2

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet threadCurrentMock;
		Friend threadCurrentMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IOutputNeuralNet targetMock;
		Friend targetMock As IOutputNeuralNet

		Friend sut As NStepQLearning

		Private Sub setup(ByVal gamma As Double)
			[when](threadCurrentMock.output(any(GetType(Observation)))).thenAnswer(Function(invocation)
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, invocation.getArgument(0, GetType(Observation)).getChannelData(0).mul(-1.0))
			Return result
			End Function)

			[when](targetMock.output(any(GetType(Observation)))).thenAnswer(Function(invocation)
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, invocation.getArgument(0, GetType(Observation)).getData().mul(-2.0))
			Return result
			End Function)
			[when](threadCurrentMock.isRecurrent()).thenReturn(False)

			Dim configuration As NStepQLearning.Configuration = NStepQLearning.Configuration.builder().gamma(gamma).build()
			sut = New NStepQLearning(threadCurrentMock, targetMock, ACTION_SPACE_SIZE, configuration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_isTerminal_expect_initRewardIs0()
		Public Overridable Sub when_isTerminal_expect_initRewardIs0()
			' Arrange
			Dim action As Integer = 0
			setup(1.0)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.rl4j.observation.Observation observation = new org.deeplearning4j.rl4j.observation.Observation(org.nd4j.linalg.factory.Nd4j.zeros(1, 2));
			Dim observation As New Observation(Nd4j.zeros(1, 2))
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass(Me, action, observation)

			' Act
			Dim result As Gradients = sut.compute(experience)

			' Assert
			Dim argument As ArgumentCaptor(Of FeaturesLabels) = ArgumentCaptor.forClass(GetType(FeaturesLabels))
			verify(threadCurrentMock, times(1)).computeGradients(argument.capture())

			Dim featuresLabels As FeaturesLabels = argument.getValue()
			assertEquals(0.0, featuresLabels.getLabels(CommonLabelNames.QValues).getDouble(0), 0.000001)
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As NonRecurrentNStepQLearningTest

			Private action As Integer
			Private observation As Observation

			Public Sub New(ByVal outerInstance As NonRecurrentNStepQLearningTest, ByVal action As Integer, ByVal observation As Observation)
				Me.outerInstance = outerInstance
				Me.action = action
				Me.observation = observation

				Me.add(New StateActionReward(Of Integer)(observation, action, 0.0, True))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notTerminal_expect_initRewardWithMaxQFromTarget()
		Public Overridable Sub when_notTerminal_expect_initRewardWithMaxQFromTarget()
			' Arrange
			Dim action As Integer = 0
			setup(1.0)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.rl4j.observation.Observation observation = new org.deeplearning4j.rl4j.observation.Observation(org.nd4j.linalg.factory.Nd4j.create(new double[] { -123.0, -234.0 }).reshape(1, 2));
			Dim observation As New Observation(Nd4j.create(New Double() { -123.0, -234.0 }).reshape(ChrW(1), 2))
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass2(Me, action, observation)

			' Act
			Dim result As Gradients = sut.compute(experience)

			' Assert
			Dim argument As ArgumentCaptor(Of FeaturesLabels) = ArgumentCaptor.forClass(GetType(FeaturesLabels))
			verify(threadCurrentMock, times(1)).computeGradients(argument.capture())

			Dim featuresLabels As FeaturesLabels = argument.getValue()
			assertEquals(-2.0 * observation.Data.getDouble(0, 1), featuresLabels.getLabels(CommonLabelNames.QValues).getDouble(0), 0.000001)
		End Sub

		Private Class ArrayListAnonymousInnerClass2
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As NonRecurrentNStepQLearningTest

			Private action As Integer
			Private observation As Observation

			Public Sub New(ByVal outerInstance As NonRecurrentNStepQLearningTest, ByVal action As Integer, ByVal observation As Observation)
				Me.outerInstance = outerInstance
				Me.action = action
				Me.observation = observation

				Me.add(New StateActionReward(Of Integer)(observation, action, 0.0, False))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingWithMultipleExperiences_expect_gradientsAreValid()
		Public Overridable Sub when_callingWithMultipleExperiences_expect_gradientsAreValid()
			' Arrange
			Dim gamma As Double = 0.9
			setup(gamma)

			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass3(Me)

			' Act
			sut.compute(experience)

			' Assert
			Dim argument As ArgumentCaptor(Of FeaturesLabels) = ArgumentCaptor.forClass(GetType(FeaturesLabels))
			verify(threadCurrentMock, times(1)).computeGradients(argument.capture())

			' input side -- should be a stack of observations
			Dim featuresValues As INDArray = argument.getValue().getFeatures().get(0)
			assertEquals(-1.1, featuresValues.getDouble(0, 0), 0.00001)
			assertEquals(-1.2, featuresValues.getDouble(0, 1), 0.00001)
			assertEquals(-2.1, featuresValues.getDouble(1, 0), 0.00001)
			assertEquals(-2.2, featuresValues.getDouble(1, 1), 0.00001)

			' target side
			Dim labels As INDArray = argument.getValue().getLabels(CommonLabelNames.QValues)
			assertEquals(1.0 + gamma * 2.0, labels.getDouble(0, 0), 0.00001)
			assertEquals(1.2, labels.getDouble(0, 1), 0.00001)
			assertEquals(2.1, labels.getDouble(1, 0), 0.00001)
			assertEquals(2.0, labels.getDouble(1, 1), 0.00001)
		End Sub

		Private Class ArrayListAnonymousInnerClass3
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As NonRecurrentNStepQLearningTest

			Public Sub New(ByVal outerInstance As NonRecurrentNStepQLearningTest)
				Me.outerInstance = outerInstance

				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { -1.1, -1.2 }).reshape(ChrW(1), 2)), 0, 1.0, False))
				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { -2.1, -2.2 }).reshape(ChrW(1), 2)), 1, 2.0, True))
			End Sub

		End Class
	End Class

End Namespace