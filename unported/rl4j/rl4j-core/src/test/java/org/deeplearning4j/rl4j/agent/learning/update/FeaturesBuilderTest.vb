Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports IObservationSource = org.deeplearning4j.rl4j.observation.IObservationSource
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.rl4j.agent.learning.update


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class FeaturesBuilderTest
	Public Class FeaturesBuilderTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_creatingFeaturesWithObservationSourceAndNonRecurrent_expect_correctlyShapedFeatures()
		Public Overridable Sub when_creatingFeaturesWithObservationSourceAndNonRecurrent_expect_correctlyShapedFeatures()
			' Arrange
			Dim trainingBatch As IList(Of IObservationSource) = New List(Of IObservationSource)()
			Dim observation1 As New Observation(New INDArray() { Nd4j.create(New Double() { 1.0 }).reshape(ChrW(1), 1), Nd4j.create(New Double() { 2.0, 3.0 }).reshape(ChrW(1), 2)})
			trainingBatch.Add(New StateActionReward(Of Integer)(observation1, 0, 0.0, False))
			Dim observation2 As New Observation(New INDArray() { Nd4j.create(New Double() { 4.0 }).reshape(ChrW(1), 1), Nd4j.create(New Double() { 5.0, 6.0 }).reshape(ChrW(1), 2)})
			trainingBatch.Add(New StateActionReward(Of Integer)(observation2, 0, 0.0, False))
			Dim sut As New FeaturesBuilder(False)

			' Act
			Dim result As Features = sut.build(trainingBatch)

			' Assert
			assertEquals(2, result.getBatchSize())
			assertEquals(1.0, result.get(0).getDouble(0, 0), 0.00001)
			assertEquals(4.0, result.get(0).getDouble(1, 0), 0.00001)

			assertEquals(2.0, result.get(1).getDouble(0, 0), 0.00001)
			assertEquals(3.0, result.get(1).getDouble(0, 1), 0.00001)
			assertEquals(5.0, result.get(1).getDouble(1, 0), 0.00001)
			assertEquals(6.0, result.get(1).getDouble(1, 1), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_creatingFeaturesWithStreamAndNonRecurrent_expect_correctlyShapedFeatures()
		Public Overridable Sub when_creatingFeaturesWithStreamAndNonRecurrent_expect_correctlyShapedFeatures()
			' Arrange
			Dim trainingBatch As IList(Of StateActionRewardState(Of Integer)) = New List(Of StateActionRewardState(Of Integer))()
			Dim observation1 As New Observation(New INDArray() { Nd4j.create(New Double() { 1.0 }).reshape(ChrW(1), 1), Nd4j.create(New Double() { 2.0, 3.0 }).reshape(ChrW(1), 2)})
			Dim stateActionRewardState1 As New StateActionRewardState(Of Integer)(Nothing, 0, 0.0, False)
			stateActionRewardState1.setNextObservation(observation1)
			trainingBatch.Add(stateActionRewardState1)
			Dim observation2 As New Observation(New INDArray() { Nd4j.create(New Double() { 4.0 }).reshape(ChrW(1), 1), Nd4j.create(New Double() { 5.0, 6.0 }).reshape(ChrW(1), 2)})
			Dim stateActionRewardState2 As New StateActionRewardState(Of Integer)(Nothing, 0, 0.0, False)
			stateActionRewardState2.setNextObservation(observation2)
			trainingBatch.Add(stateActionRewardState2)

			Dim sut As New FeaturesBuilder(False)

			' Act
			Dim result As Features = sut.build(trainingBatch.Select(Function(e) e.getNextObservation()), trainingBatch.Count)

			' Assert
			assertEquals(2, result.getBatchSize())
			assertEquals(1.0, result.get(0).getDouble(0, 0), 0.00001)
			assertEquals(4.0, result.get(0).getDouble(1, 0), 0.00001)

			assertEquals(2.0, result.get(1).getDouble(0, 0), 0.00001)
			assertEquals(3.0, result.get(1).getDouble(0, 1), 0.00001)
			assertEquals(5.0, result.get(1).getDouble(1, 0), 0.00001)
			assertEquals(6.0, result.get(1).getDouble(1, 1), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_creatingFeaturesWithObservationSourceAndRecurrent_expect_correctlyShapedFeatures()
		Public Overridable Sub when_creatingFeaturesWithObservationSourceAndRecurrent_expect_correctlyShapedFeatures()
			' Arrange
			Dim trainingBatch As IList(Of IObservationSource) = New List(Of IObservationSource)()
			Dim observation1 As New Observation(New INDArray() { Nd4j.create(New Double() { 1.0, 2.0 }).reshape(ChrW(1), 2, 1), Nd4j.create(New Double() { 3.0, 4.0, 5.0, 6.0 }).reshape(ChrW(1), 2, 2, 1)})
			trainingBatch.Add(New StateActionReward(Of Integer)(observation1, 0, 0.0, False))
			Dim observation2 As New Observation(New INDArray() { Nd4j.create(New Double() { 7.0, 8.0 }).reshape(ChrW(1), 2, 1), Nd4j.create(New Double() { 9.0, 10.0, 11.0, 12.0 }).reshape(ChrW(1), 2, 2, 1)})
			trainingBatch.Add(New StateActionReward(Of Integer)(observation2, 0, 0.0, False))

			Dim sut As New FeaturesBuilder(True)

			' Act
			Dim result As Features = sut.build(trainingBatch)

			' Assert
			assertEquals(1, result.getBatchSize()) ' With recurrent, batch size is always 1; examples are stacked on the time-serie dimension
			assertArrayEquals(New Long() { 1, 2, 2 }, result.get(0).shape())
			assertEquals(1.0, result.get(0).getDouble(0, 0, 0), 0.00001)
			assertEquals(7.0, result.get(0).getDouble(0, 0, 1), 0.00001)
			assertEquals(2.0, result.get(0).getDouble(0, 1, 0), 0.00001)
			assertEquals(8.0, result.get(0).getDouble(0, 1, 1), 0.00001)

			assertArrayEquals(New Long() { 1, 2, 2, 2 }, result.get(1).shape())
			assertEquals(3.0, result.get(1).getDouble(0, 0, 0, 0), 0.00001)
			assertEquals(9.0, result.get(1).getDouble(0, 0, 0, 1), 0.00001)
			assertEquals(4.0, result.get(1).getDouble(0, 0, 1, 0), 0.00001)
			assertEquals(10.0, result.get(1).getDouble(0, 0, 1, 1), 0.00001)

			assertEquals(5.0, result.get(1).getDouble(0, 1, 0, 0), 0.00001)
			assertEquals(11.0, result.get(1).getDouble(0, 1, 0, 1), 0.00001)
			assertEquals(6.0, result.get(1).getDouble(0, 1, 1, 0), 0.00001)
			assertEquals(12.0, result.get(1).getDouble(0, 1, 1, 1), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_creatingFeaturesWithStreamAndRecurrent_expect_correctlyShapedFeatures()
		Public Overridable Sub when_creatingFeaturesWithStreamAndRecurrent_expect_correctlyShapedFeatures()
			' Arrange
			Dim trainingBatch As IList(Of StateActionRewardState(Of Integer)) = New List(Of StateActionRewardState(Of Integer))()
			Dim observation1 As New Observation(New INDArray() { Nd4j.create(New Double() { 1.0, 2.0 }).reshape(ChrW(1), 2, 1), Nd4j.create(New Double() { 3.0, 4.0, 5.0, 6.0 }).reshape(ChrW(1), 2, 2, 1)})
			Dim stateActionRewardState1 As New StateActionRewardState(Of Integer)(Nothing, 0, 0.0, False)
			stateActionRewardState1.setNextObservation(observation1)
			trainingBatch.Add(stateActionRewardState1)
			Dim observation2 As New Observation(New INDArray() { Nd4j.create(New Double() { 7.0, 8.0 }).reshape(ChrW(1), 2, 1), Nd4j.create(New Double() { 9.0, 10.0, 11.0, 12.0 }).reshape(ChrW(1), 2, 2, 1)})
			Dim stateActionRewardState2 As New StateActionRewardState(Of Integer)(Nothing, 0, 0.0, False)
			stateActionRewardState2.setNextObservation(observation2)
			trainingBatch.Add(stateActionRewardState2)

			Dim sut As New FeaturesBuilder(True)

			' Act
			Dim result As Features = sut.build(trainingBatch.Select(Function(e) e.getNextObservation()), trainingBatch.Count)

			' Assert
			assertEquals(1, result.getBatchSize()) ' With recurrent, batch size is always 1; examples are stacked on the time-serie dimension
			assertArrayEquals(New Long() { 1, 2, 2 }, result.get(0).shape())
			assertEquals(1.0, result.get(0).getDouble(0, 0, 0), 0.00001)
			assertEquals(7.0, result.get(0).getDouble(0, 0, 1), 0.00001)
			assertEquals(2.0, result.get(0).getDouble(0, 1, 0), 0.00001)
			assertEquals(8.0, result.get(0).getDouble(0, 1, 1), 0.00001)

			assertArrayEquals(New Long() { 1, 2, 2, 2 }, result.get(1).shape())
			assertEquals(3.0, result.get(1).getDouble(0, 0, 0, 0), 0.00001)
			assertEquals(9.0, result.get(1).getDouble(0, 0, 0, 1), 0.00001)
			assertEquals(4.0, result.get(1).getDouble(0, 0, 1, 0), 0.00001)
			assertEquals(10.0, result.get(1).getDouble(0, 0, 1, 1), 0.00001)

			assertEquals(5.0, result.get(1).getDouble(0, 1, 0, 0), 0.00001)
			assertEquals(11.0, result.get(1).getDouble(0, 1, 0, 1), 0.00001)
			assertEquals(6.0, result.get(1).getDouble(0, 1, 1, 0), 0.00001)
			assertEquals(12.0, result.get(1).getDouble(0, 1, 1, 1), 0.00001)
		End Sub
	End Class

End Namespace