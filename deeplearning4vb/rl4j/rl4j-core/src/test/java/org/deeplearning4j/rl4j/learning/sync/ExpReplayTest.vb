Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.experience
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports MockRandom = org.deeplearning4j.rl4j.support.MockRandom
Imports Test = org.junit.jupiter.api.Test
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.rl4j.learning.sync


	Public Class ExpReplayTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_storingElementWithStorageNotFull_expect_elementStored()
		Public Overridable Sub when_storingElementWithStorageNotFull_expect_elementStored()
			' Arrange
			Dim randomMock As New MockRandom(Nothing, New Integer() { 0 })
			Dim sut As New ExpReplay(Of Integer)(2, 1, randomMock)

			' Act
			Dim stateActionRewardState As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 123, 234, New Observation(Nd4j.create(1)))
			sut.store(stateActionRewardState)
			Dim results As IList(Of StateActionRewardState(Of Integer)) = sut.getBatch(1)

			' Assert
			assertEquals(1, results.Count)
			assertEquals(123, CInt(Math.Truncate(results(0).getAction())))
			assertEquals(234, CInt(Math.Truncate(results(0).getReward())))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_storingElementWithStorageFull_expect_oldestElementReplacedByStored()
		Public Overridable Sub when_storingElementWithStorageFull_expect_oldestElementReplacedByStored()
			' Arrange
			Dim randomMock As New MockRandom(Nothing, New Integer() { 0, 1 })
			Dim sut As New ExpReplay(Of Integer)(2, 1, randomMock)

			' Act
			Dim stateActionRewardState1 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 1, 2, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState2 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 3, 4, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState3 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 5, 6, New Observation(Nd4j.create(1)))
			sut.store(stateActionRewardState1)
			sut.store(stateActionRewardState2)
			sut.store(stateActionRewardState3)
			Dim results As IList(Of StateActionRewardState(Of Integer)) = sut.getBatch(2)

			' Assert
			assertEquals(2, results.Count)

			assertEquals(3, CInt(Math.Truncate(results(0).getAction())))
			assertEquals(4, CInt(Math.Truncate(results(0).getReward())))

			assertEquals(5, CInt(Math.Truncate(results(1).getAction())))
			assertEquals(6, CInt(Math.Truncate(results(1).getReward())))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_askBatchSizeZeroAndStorageEmpty_expect_emptyBatch()
		Public Overridable Sub when_askBatchSizeZeroAndStorageEmpty_expect_emptyBatch()
			' Arrange
			Dim randomMock As New MockRandom(Nothing, New Integer() { 0 })
			Dim sut As New ExpReplay(Of Integer)(5, 1, randomMock)

			' Act
			Dim results As IList(Of StateActionRewardState(Of Integer)) = sut.getBatch(0)

			' Assert
			assertEquals(0, results.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_askBatchSizeZeroAndStorageNotEmpty_expect_emptyBatch()
		Public Overridable Sub when_askBatchSizeZeroAndStorageNotEmpty_expect_emptyBatch()
			' Arrange
			Dim randomMock As New MockRandom(Nothing, New Integer() { 0 })
			Dim sut As New ExpReplay(Of Integer)(5, 1, randomMock)

			' Act
			Dim stateActionRewardState1 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 1, 2, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState2 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 3, 4, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState3 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 5, 6, New Observation(Nd4j.create(1)))
			sut.store(stateActionRewardState1)
			sut.store(stateActionRewardState2)
			sut.store(stateActionRewardState3)
			Dim results As IList(Of StateActionRewardState(Of Integer)) = sut.getBatch(0)

			' Assert
			assertEquals(0, results.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_askBatchSizeGreaterThanStoredCount_expect_batchWithStoredCountElements()
		Public Overridable Sub when_askBatchSizeGreaterThanStoredCount_expect_batchWithStoredCountElements()
			' Arrange
			Dim randomMock As New MockRandom(Nothing, New Integer() { 0, 1, 2 })
			Dim sut As New ExpReplay(Of Integer)(5, 1, randomMock)

			' Act
			Dim stateActionRewardState1 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 1, 2, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState2 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 3, 4, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState3 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 5, 6, New Observation(Nd4j.create(1)))
			sut.store(stateActionRewardState1)
			sut.store(stateActionRewardState2)
			sut.store(stateActionRewardState3)
			Dim results As IList(Of StateActionRewardState(Of Integer)) = sut.getBatch(10)

			' Assert
			assertEquals(3, results.Count)

			assertEquals(1, CInt(Math.Truncate(results(0).getAction())))
			assertEquals(2, CInt(Math.Truncate(results(0).getReward())))

			assertEquals(3, CInt(Math.Truncate(results(1).getAction())))
			assertEquals(4, CInt(Math.Truncate(results(1).getReward())))

			assertEquals(5, CInt(Math.Truncate(results(2).getAction())))
			assertEquals(6, CInt(Math.Truncate(results(2).getReward())))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_askBatchSizeSmallerThanStoredCount_expect_batchWithAskedElements()
		Public Overridable Sub when_askBatchSizeSmallerThanStoredCount_expect_batchWithAskedElements()
			' Arrange
			Dim randomMock As New MockRandom(Nothing, New Integer() { 0, 1, 2, 3, 4 })
			Dim sut As New ExpReplay(Of Integer)(5, 1, randomMock)

			' Act
			Dim stateActionRewardState1 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 1, 2, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState2 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 3, 4, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState3 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 5, 6, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState4 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 7, 8, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState5 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 9, 10, New Observation(Nd4j.create(1)))
			sut.store(stateActionRewardState1)
			sut.store(stateActionRewardState2)
			sut.store(stateActionRewardState3)
			sut.store(stateActionRewardState4)
			sut.store(stateActionRewardState5)
			Dim results As IList(Of StateActionRewardState(Of Integer)) = sut.getBatch(3)

			' Assert
			assertEquals(3, results.Count)

			assertEquals(1, CInt(Math.Truncate(results(0).getAction())))
			assertEquals(2, CInt(Math.Truncate(results(0).getReward())))

			assertEquals(3, CInt(Math.Truncate(results(1).getAction())))
			assertEquals(4, CInt(Math.Truncate(results(1).getReward())))

			assertEquals(5, CInt(Math.Truncate(results(2).getAction())))
			assertEquals(6, CInt(Math.Truncate(results(2).getReward())))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_randomGivesDuplicates_expect_noDuplicatesInBatch()
		Public Overridable Sub when_randomGivesDuplicates_expect_noDuplicatesInBatch()
			' Arrange
			Dim randomMock As New MockRandom(Nothing, New Integer() { 0, 1, 2, 1, 3, 1, 4 })
			Dim sut As New ExpReplay(Of Integer)(5, 1, randomMock)

			' Act
			Dim stateActionRewardState1 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 1, 2, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState2 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 3, 4, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState3 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 5, 6, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState4 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 7, 8, New Observation(Nd4j.create(1)))
			Dim stateActionRewardState5 As StateActionRewardState(Of Integer) = buildTransition(buildObservation(), 9, 10, New Observation(Nd4j.create(1)))
			sut.store(stateActionRewardState1)
			sut.store(stateActionRewardState2)
			sut.store(stateActionRewardState3)
			sut.store(stateActionRewardState4)
			sut.store(stateActionRewardState5)
			Dim results As IList(Of StateActionRewardState(Of Integer)) = sut.getBatch(3)

			' Assert
			assertEquals(3, results.Count)

			assertEquals(1, CInt(Math.Truncate(results(0).getAction())))
			assertEquals(2, CInt(Math.Truncate(results(0).getReward())))

			assertEquals(3, CInt(Math.Truncate(results(1).getAction())))
			assertEquals(4, CInt(Math.Truncate(results(1).getReward())))

			assertEquals(5, CInt(Math.Truncate(results(2).getAction())))
			assertEquals(6, CInt(Math.Truncate(results(2).getReward())))
		End Sub

		Private Function buildTransition(ByVal observation As Observation, ByVal action As Integer?, ByVal reward As Double, ByVal nextObservation As Observation) As StateActionRewardState(Of Integer)
			Dim result As New StateActionRewardState(Of Integer)(observation, action, reward, False)
			result.setNextObservation(nextObservation)

			Return result
		End Function

		Private Function buildObservation() As Observation
			Return New Observation(Nd4j.create(1, 1))
		End Function
	End Class

End Namespace