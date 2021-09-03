Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.network.dqn
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
Imports Mock = org.mockito.Mock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.mockito.ArgumentMatchers.any
import static org.mockito.ArgumentMatchers.argThat
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

Namespace org.deeplearning4j.rl4j.learning.async.nstep.discrete


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class QLearningUpdateAlgorithmTest
	Public Class QLearningUpdateAlgorithmTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock AsyncGlobal mockAsyncGlobal;
		Friend mockAsyncGlobal As AsyncGlobal

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IDQN dqnMock;
		Friend dqnMock As IDQN

		Private sut As UpdateAlgorithm

		Private Sub setup(ByVal gamma As Double)
			' mock a neural net output -- just invert the sign of the input
			[when](dqnMock.outputAll(any(GetType(INDArray)))).thenAnswer(Function(invocation) New INDArray() { invocation.getArgument(0, GetType(INDArray)).mul(-1.0) })

			sut = New QLearningUpdateAlgorithm(2, gamma)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_isTerminal_expect_initRewardIs0()
		Public Overridable Sub when_isTerminal_expect_initRewardIs0()
			' Arrange
			setup(1.0)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.rl4j.observation.Observation observation = new org.deeplearning4j.rl4j.observation.Observation(org.nd4j.linalg.factory.Nd4j.zeros(1, 2));
			Dim observation As New Observation(Nd4j.zeros(1, 2))
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass(Me, observation)

			' Act
			sut.computeGradients(dqnMock, experience)

			' Assert
			verify(dqnMock, times(1)).gradient(any(GetType(INDArray)), argThat(Function(x As INDArray) x.getDouble(0) = 0.0))
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As QLearningUpdateAlgorithmTest

			Private observation As Observation

			Public Sub New(ByVal outerInstance As QLearningUpdateAlgorithmTest, ByVal observation As Observation)
				Me.outerInstance = outerInstance
				Me.observation = observation

				Me.add(New StateActionReward(Of Integer)(observation, 0, 0.0, True))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_terminalAndNoTargetUpdate_expect_initRewardWithMaxQFromCurrent()
		Public Overridable Sub when_terminalAndNoTargetUpdate_expect_initRewardWithMaxQFromCurrent()
			' Arrange
			setup(1.0)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.rl4j.observation.Observation observation = new org.deeplearning4j.rl4j.observation.Observation(org.nd4j.linalg.factory.Nd4j.create(new double[] { -123.0, -234.0 }).reshape(1, 2));
			Dim observation As New Observation(Nd4j.create(New Double() { -123.0, -234.0 }).reshape(ChrW(1), 2))
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass2(Me, observation)

			' Act
			sut.computeGradients(dqnMock, experience)

			' Assert
			Dim argument As ArgumentCaptor(Of INDArray) = ArgumentCaptor.forClass(GetType(INDArray))

			verify(dqnMock, times(2)).outputAll(argument.capture())
			Dim values As IList(Of INDArray) = argument.getAllValues()
			assertEquals(-123.0, values(0).getDouble(0, 0), 0.00001)
			assertEquals(-123.0, values(1).getDouble(0, 0), 0.00001)

			verify(dqnMock, times(1)).gradient(any(GetType(INDArray)), argThat(Function(x As INDArray) x.getDouble(0) = 234.0))
		End Sub

		Private Class ArrayListAnonymousInnerClass2
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As QLearningUpdateAlgorithmTest

			Private observation As Observation

			Public Sub New(ByVal outerInstance As QLearningUpdateAlgorithmTest, ByVal observation As Observation)
				Me.outerInstance = outerInstance
				Me.observation = observation

				Me.add(New StateActionReward(Of Integer)(observation, 0, 0.0, False))
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
			sut.computeGradients(dqnMock, experience)

			' Assert
			Dim features As ArgumentCaptor(Of INDArray) = ArgumentCaptor.forClass(GetType(INDArray))
			Dim targets As ArgumentCaptor(Of INDArray) = ArgumentCaptor.forClass(GetType(INDArray))
			verify(dqnMock, times(1)).gradient(features.capture(), targets.capture())

			' input side -- should be a stack of observations
			Dim featuresValues As INDArray = features.getValue()
			assertEquals(-1.1, featuresValues.getDouble(0, 0), 0.00001)
			assertEquals(-1.2, featuresValues.getDouble(0, 1), 0.00001)
			assertEquals(-2.1, featuresValues.getDouble(1, 0), 0.00001)
			assertEquals(-2.2, featuresValues.getDouble(1, 1), 0.00001)

			' target side
			Dim targetsValues As INDArray = targets.getValue()
			assertEquals(1.0 + gamma * 2.0, targetsValues.getDouble(0, 0), 0.00001)
			assertEquals(1.2, targetsValues.getDouble(0, 1), 0.00001)
			assertEquals(2.1, targetsValues.getDouble(1, 0), 0.00001)
			assertEquals(2.0, targetsValues.getDouble(1, 1), 0.00001)
		End Sub

		Private Class ArrayListAnonymousInnerClass3
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As QLearningUpdateAlgorithmTest

			Public Sub New(ByVal outerInstance As QLearningUpdateAlgorithmTest)
				Me.outerInstance = outerInstance

				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { -1.1, -1.2 }).reshape(ChrW(1), 2)), 0, 1.0, False))
				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { -2.1, -2.2 }).reshape(ChrW(1), 2)), 1, 2.0, True))
			End Sub

		End Class
	End Class

End Namespace