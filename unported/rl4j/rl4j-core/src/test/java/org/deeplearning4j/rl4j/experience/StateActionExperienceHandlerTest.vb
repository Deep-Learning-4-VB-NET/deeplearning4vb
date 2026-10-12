Imports System
Imports System.Collections.Generic
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.rl4j.experience


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class StateActionExperienceHandlerTest
	Public Class StateActionExperienceHandlerTest

		Private Function buildConfiguration(ByVal batchSize As Integer) As StateActionExperienceHandler.Configuration
			Return StateActionExperienceHandler.Configuration.builder().batchSize(batchSize).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_addingExperience_expect_generateTrainingBatchReturnsIt()
		Public Overridable Sub when_addingExperience_expect_generateTrainingBatchReturnsIt()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(Integer.MaxValue))
			sut.reset()
			Dim observation As New Observation(Nd4j.zeros(1))
			sut.addExperience(observation, 123, 234.0, True)

			' Act
			Dim result As IList(Of StateActionReward(Of Integer)) = sut.generateTrainingBatch()

			' Assert
			assertEquals(1, result.Count)
			assertSame(observation, result(0).getObservation())
			assertEquals(123, CInt(Math.Truncate(result(0).getAction())))
			assertEquals(234.0, result(0).getReward(), 0.00001)
			assertTrue(result(0).isTerminal())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_addingMultipleExperiences_expect_generateTrainingBatchReturnsItInSameOrder()
		Public Overridable Sub when_addingMultipleExperiences_expect_generateTrainingBatchReturnsItInSameOrder()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(Integer.MaxValue))
			sut.reset()
			sut.addExperience(Nothing, 1, 1.0, False)
			sut.addExperience(Nothing, 2, 2.0, False)
			sut.addExperience(Nothing, 3, 3.0, False)

			' Act
			Dim result As IList(Of StateActionReward(Of Integer)) = sut.generateTrainingBatch()

			' Assert
			assertEquals(3, result.Count)
			assertEquals(1, CInt(Math.Truncate(result(0).getAction())))
			assertEquals(2, CInt(Math.Truncate(result(1).getAction())))
			assertEquals(3, CInt(Math.Truncate(result(2).getAction())))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_gettingExperience_expect_experienceStoreIsCleared()
		Public Overridable Sub when_gettingExperience_expect_experienceStoreIsCleared()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(Integer.MaxValue))
			sut.reset()
			sut.addExperience(Nothing, 1, 1.0, False)

			' Act
			Dim firstResult As IList(Of StateActionReward(Of Integer)) = sut.generateTrainingBatch()
			Dim secondResult As IList(Of StateActionReward(Of Integer)) = sut.generateTrainingBatch()

			' Assert
			assertEquals(1, firstResult.Count)
			assertEquals(0, secondResult.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_addingExperience_expect_getTrainingBatchSizeReturnSize()
		Public Overridable Sub when_addingExperience_expect_getTrainingBatchSizeReturnSize()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(Integer.MaxValue))
			sut.reset()
			sut.addExperience(Nothing, 1, 1.0, False)
			sut.addExperience(Nothing, 2, 2.0, False)
			sut.addExperience(Nothing, 3, 3.0, False)

			' Act
			Dim size As Integer = sut.getTrainingBatchSize()

			' Assert
			assertEquals(3, size)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_experienceIsEmpty_expect_TrainingBatchNotReady()
		Public Overridable Sub when_experienceIsEmpty_expect_TrainingBatchNotReady()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(5))
			sut.reset()

			' Act
			Dim isTrainingBatchReady As Boolean = sut.isTrainingBatchReady()

			' Assert
			assertFalse(isTrainingBatchReady)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_experienceSizeIsGreaterOrEqualToThanBatchSize_expect_TrainingBatchIsReady()
		Public Overridable Sub when_experienceSizeIsGreaterOrEqualToThanBatchSize_expect_TrainingBatchIsReady()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(5))
			sut.reset()
			sut.addExperience(Nothing, 1, 1.0, False)
			sut.addExperience(Nothing, 2, 2.0, False)
			sut.addExperience(Nothing, 3, 3.0, False)
			sut.addExperience(Nothing, 4, 4.0, False)
			sut.addExperience(Nothing, 5, 5.0, False)

			' Act
			Dim isTrainingBatchReady As Boolean = sut.isTrainingBatchReady()

			' Assert
			assertTrue(isTrainingBatchReady)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_experienceSizeIsSmallerThanBatchSizeButFinalObservationIsSet_expect_TrainingBatchIsReady()
		Public Overridable Sub when_experienceSizeIsSmallerThanBatchSizeButFinalObservationIsSet_expect_TrainingBatchIsReady()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(5))
			sut.reset()
			sut.addExperience(Nothing, 1, 1.0, False)
			sut.addExperience(Nothing, 2, 2.0, False)
			sut.setFinalObservation(Nothing)

			' Act
			Dim isTrainingBatchReady As Boolean = sut.isTrainingBatchReady()

			' Assert
			assertTrue(isTrainingBatchReady)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_experienceSizeIsZeroAndFinalObservationIsSet_expect_TrainingBatchIsNotReady()
		Public Overridable Sub when_experienceSizeIsZeroAndFinalObservationIsSet_expect_TrainingBatchIsNotReady()
			' Arrange
			Dim sut As New StateActionExperienceHandler(buildConfiguration(5))
			sut.reset()
			sut.setFinalObservation(Nothing)

			' Act
			Dim isTrainingBatchReady As Boolean = sut.isTrainingBatchReady()

			' Assert
			assertFalse(isTrainingBatchReady)
		End Sub

	End Class

End Namespace