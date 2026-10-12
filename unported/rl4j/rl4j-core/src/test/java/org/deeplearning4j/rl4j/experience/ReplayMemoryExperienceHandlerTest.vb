Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.learning.sync
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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

Namespace org.deeplearning4j.rl4j.experience


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class ReplayMemoryExperienceHandlerTest
	Public Class ReplayMemoryExperienceHandlerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IExpReplay<Integer> expReplayMock;
		Friend expReplayMock As IExpReplay(Of Integer)

		Private Function buildConfiguration() As ReplayMemoryExperienceHandler.Configuration
			Return ReplayMemoryExperienceHandler.Configuration.builder().maxReplayMemorySize(10).batchSize(5).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_addingFirstExperience_expect_notAddedToStoreBeforeNextObservationIsAdded()
		Public Overridable Sub when_addingFirstExperience_expect_notAddedToStoreBeforeNextObservationIsAdded()
			' Arrange
			[when](expReplayMock.DesignatedBatchSize).thenReturn(10)

			Dim sut As New ReplayMemoryExperienceHandler(expReplayMock)

			' Act
			sut.addExperience(New Observation(Nd4j.create(New Double() { 1.0 })), 1, 1.0, False)
			Dim isStoreCalledAfterFirstAdd As Boolean = mockingDetails(expReplayMock).getInvocations().Any(Function(x) x.getMethod().getName() = "store")
			sut.addExperience(New Observation(Nd4j.create(New Double() { 2.0 })), 2, 2.0, False)
			Dim isStoreCalledAfterSecondAdd As Boolean = mockingDetails(expReplayMock).getInvocations().Any(Function(x) x.getMethod().getName() = "store")

			' Assert
			assertFalse(isStoreCalledAfterFirstAdd)
			assertTrue(isStoreCalledAfterSecondAdd)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_addingExperience_expect_transitionsAreCorrect()
		Public Overridable Sub when_addingExperience_expect_transitionsAreCorrect()
			' Arrange
			Dim sut As New ReplayMemoryExperienceHandler(expReplayMock)

			' Act
			sut.addExperience(New Observation(Nd4j.create(New Double() { 1.0 })), 1, 1.0, False)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 2.0 })), 2, 2.0, False)
			sut.setFinalObservation(New Observation(Nd4j.create(New Double() { 3.0 })))

			' Assert
			Dim argument As ArgumentCaptor(Of StateActionRewardState(Of Integer)) = ArgumentCaptor.forClass(GetType(StateActionRewardState))
			verify(expReplayMock, times(2)).store(argument.capture())
			Dim stateActionRewardStates As IList(Of StateActionRewardState(Of Integer)) = argument.getAllValues()

			assertEquals(1.0, stateActionRewardStates(0).getObservation().getData().getDouble(0), 0.00001)
			assertEquals(1, CInt(Math.Truncate(stateActionRewardStates(0).getAction())))
			assertEquals(1.0, stateActionRewardStates(0).getReward(), 0.00001)
			assertEquals(2.0, stateActionRewardStates(0).getNextObservation().getChannelData(0).getDouble(0), 0.00001)

			assertEquals(2.0, stateActionRewardStates(1).getObservation().getData().getDouble(0), 0.00001)
			assertEquals(2, CInt(Math.Truncate(stateActionRewardStates(1).getAction())))
			assertEquals(2.0, stateActionRewardStates(1).getReward(), 0.00001)
			assertEquals(3.0, stateActionRewardStates(1).getNextObservation().getChannelData(0).getDouble(0), 0.00001)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_settingFinalObservation_expect_nextAddedExperienceDoNotUsePreviousObservation()
		Public Overridable Sub when_settingFinalObservation_expect_nextAddedExperienceDoNotUsePreviousObservation()
			' Arrange
			Dim sut As New ReplayMemoryExperienceHandler(expReplayMock)

			' Act
			sut.addExperience(New Observation(Nd4j.create(New Double() { 1.0 })), 1, 1.0, False)
			sut.setFinalObservation(New Observation(Nd4j.create(New Double() { 2.0 })))
			sut.addExperience(New Observation(Nd4j.create(New Double() { 3.0 })), 3, 3.0, False)

			' Assert
			Dim argument As ArgumentCaptor(Of StateActionRewardState(Of Integer)) = ArgumentCaptor.forClass(GetType(StateActionRewardState))
			verify(expReplayMock, times(1)).store(argument.capture())
			Dim stateActionRewardState As StateActionRewardState(Of Integer) = argument.getValue()

			assertEquals(1, CInt(Math.Truncate(stateActionRewardState.getAction())))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_addingExperience_expect_getTrainingBatchSizeReturnSize()
		Public Overridable Sub when_addingExperience_expect_getTrainingBatchSizeReturnSize()
			' Arrange
			Dim sut As New ReplayMemoryExperienceHandler(buildConfiguration(), Nd4j.Random)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 1.0 })), 1, 1.0, False)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 2.0 })), 2, 2.0, False)
			sut.setFinalObservation(New Observation(Nd4j.create(New Double() { 3.0 })))

			' Act
			Dim size As Integer = sut.getTrainingBatchSize()

			' Assert
			assertEquals(2, size)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_experienceSizeIsSmallerThanBatchSize_expect_TrainingBatchIsNotReady()
		Public Overridable Sub when_experienceSizeIsSmallerThanBatchSize_expect_TrainingBatchIsNotReady()
			' Arrange
			Dim sut As New ReplayMemoryExperienceHandler(buildConfiguration(), Nd4j.Random)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 1.0 })), 1, 1.0, False)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 2.0 })), 2, 2.0, False)
			sut.setFinalObservation(New Observation(Nd4j.create(New Double() { 3.0 })))

			' Act

			' Assert
			assertFalse(sut.isTrainingBatchReady())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_experienceSizeIsGreaterOrEqualToBatchSize_expect_TrainingBatchIsReady()
		Public Overridable Sub when_experienceSizeIsGreaterOrEqualToBatchSize_expect_TrainingBatchIsReady()
			' Arrange
			Dim sut As New ReplayMemoryExperienceHandler(buildConfiguration(), Nd4j.Random)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 1.0 })), 1, 1.0, False)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 2.0 })), 2, 2.0, False)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 3.0 })), 3, 3.0, False)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 4.0 })), 4, 4.0, False)
			sut.addExperience(New Observation(Nd4j.create(New Double() { 5.0 })), 5, 5.0, False)
			sut.setFinalObservation(New Observation(Nd4j.create(New Double() { 6.0 })))

			' Act

			' Assert
			assertTrue(sut.isTrainingBatchReady())
		End Sub

	End Class

End Namespace