Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.experience
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports NeuralNetOutput = org.deeplearning4j.rl4j.network.NeuralNetOutput
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class NonRecurrentNStepQLearningHelperTest
	Public Class NonRecurrentNStepQLearningHelperTest

		Private ReadOnly sut As New NonRecurrentNStepQLearningHelper(3)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCreateValueLabels_expect_INDArrayWithCorrectShape()
		Public Overridable Sub when_callingCreateValueLabels_expect_INDArrayWithCorrectShape()
			' Arrange

			' Act
			Dim result As INDArray = sut.createLabels(4)

			' Assert
			assertArrayEquals(New Long() { 4, 3 }, result.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingGetExpectedQValues_expect_INDArrayWithCorrectShape()
		Public Overridable Sub when_callingGetExpectedQValues_expect_INDArrayWithCorrectShape()
			' Arrange
			Dim allExpectedQValues As INDArray = Nd4j.create(New Double() { 1.1, 1.2, 2.1, 2.2 }).reshape(ChrW(2), 2)

			' Act
			Dim result As INDArray = sut.getExpectedQValues(allExpectedQValues, 1)

			' Assert
			assertEquals(2.1, result.getDouble(0), 0.00001)
			assertEquals(2.2, result.getDouble(1), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingSetLabels_expect_INDArrayWithCorrectShape()
		Public Overridable Sub when_callingSetLabels_expect_INDArrayWithCorrectShape()
			' Arrange
			Dim labels As INDArray = Nd4j.zeros(2, 2)
			Dim data As INDArray = Nd4j.create(New Double() { 1.1, 1.2 })

			' Act
			sut.setLabels(labels, 1, data)

			' Assert
			assertEquals(0.0, labels.getDouble(0, 0), 0.00001)
			assertEquals(0.0, labels.getDouble(0, 1), 0.00001)
			assertEquals(1.1, labels.getDouble(1, 0), 0.00001)
			assertEquals(1.2, labels.getDouble(1, 1), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingGetTargetExpectedQValuesOfLast_expect_INDArrayWithCorrectShape()
		Public Overridable Sub when_callingGetTargetExpectedQValuesOfLast_expect_INDArrayWithCorrectShape()
			' Arrange
			Dim targetMock As IOutputNeuralNet = mock(GetType(IOutputNeuralNet))
			Dim experience As IList(Of StateActionReward(Of Integer)) = New ArrayListAnonymousInnerClass(Me)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.rl4j.network.NeuralNetOutput neuralNetOutput = new org.deeplearning4j.rl4j.network.NeuralNetOutput();
			Dim neuralNetOutput As New NeuralNetOutput()
			neuralNetOutput.put(CommonOutputNames.QValues, Nd4j.create(New Double() { -4.1, -4.2 }).reshape(ChrW(1), 2))
			[when](targetMock.output(any(GetType(Observation)))).thenReturn(neuralNetOutput)

			' Act
			Dim result As INDArray = sut.getTargetExpectedQValuesOfLast(targetMock, experience, Nothing)

			' Assert
			Dim observationCaptor As ArgumentCaptor(Of Observation) = ArgumentCaptor.forClass(GetType(Observation))
			verify(targetMock, times(1)).output(observationCaptor.capture())
			Dim observation As Observation = observationCaptor.getValue()
			assertEquals(4.1, observation.Data.getDouble(0), 0.00001)
			assertEquals(4.2, observation.Data.getDouble(1), 0.00001)

			assertEquals(-4.1, result.getDouble(0), 0.00001)
			assertEquals(-4.2, result.getDouble(1), 0.00001)
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of StateActionReward(Of Integer))

			Private ReadOnly outerInstance As NonRecurrentNStepQLearningHelperTest

			Public Sub New(ByVal outerInstance As NonRecurrentNStepQLearningHelperTest)
				Me.outerInstance = outerInstance

				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { 1.1, 1.2 }).reshape(ChrW(1), 2)), 0, 1.0, False))
				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { 2.1, 2.2 }).reshape(ChrW(1), 2)), 1, 2.0, False))
				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { 3.1, 3.2 }).reshape(ChrW(1), 2)), 2, 3.0, False))
				Me.add(New StateActionReward(Of Integer)(New Observation(Nd4j.create(New Double() { 4.1, 4.2 }).reshape(ChrW(1), 2)), 3, 4.0, False))
			End Sub

		End Class
	End Class

End Namespace