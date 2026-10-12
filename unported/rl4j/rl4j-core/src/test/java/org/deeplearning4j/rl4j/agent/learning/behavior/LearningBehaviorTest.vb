Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.agent.learning.update
Imports org.deeplearning4j.rl4j.experience
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
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
import static org.junit.jupiter.api.Assertions.assertFalse
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

Namespace org.deeplearning4j.rl4j.agent.learning.behavior


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) @Tag(TagNames.FILE_IO) @NativeTag public class LearningBehaviorTest
	Public Class LearningBehaviorTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ExperienceHandler<Integer, Object> experienceHandlerMock;
		Friend experienceHandlerMock As ExperienceHandler(Of Integer, Object)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IUpdateRule<Object> updateRuleMock;
		Friend updateRuleMock As IUpdateRule(Of Object)

		Friend sut As LearningBehavior(Of Integer, Object)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			sut = LearningBehavior.builder(Of Integer, Object)().experienceHandler(experienceHandlerMock).updateRule(updateRuleMock).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingHandleEpisodeStart_expect_experienceHandlerResetCalled()
		Public Overridable Sub when_callingHandleEpisodeStart_expect_experienceHandlerResetCalled()
			' Arrange
			Dim sut As LearningBehavior(Of Integer, Object) = LearningBehavior.builder(Of Integer, Object)().experienceHandler(experienceHandlerMock).updateRule(updateRuleMock).build()

			' Act
			sut.handleEpisodeStart()

			' Assert
			verify(experienceHandlerMock, times(1)).reset()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingHandleNewExperience_expect_experienceHandlerAddExperienceCalled()
		Public Overridable Sub when_callingHandleNewExperience_expect_experienceHandlerAddExperienceCalled()
			' Arrange
			Dim observationData As INDArray = Nd4j.rand(1, 1)
			[when](experienceHandlerMock.TrainingBatchReady).thenReturn(False)

			' Act
			sut.handleNewExperience(New Observation(observationData), 1, 2.0, False)

			' Assert
			Dim observationCaptor As ArgumentCaptor(Of Observation) = ArgumentCaptor.forClass(GetType(Observation))
			Dim actionCaptor As ArgumentCaptor(Of Integer) = ArgumentCaptor.forClass(GetType(Integer))
			Dim rewardCaptor As ArgumentCaptor(Of Double) = ArgumentCaptor.forClass(GetType(Double))
			Dim isTerminatedCaptor As ArgumentCaptor(Of Boolean) = ArgumentCaptor.forClass(GetType(Boolean))
			verify(experienceHandlerMock, times(1)).addExperience(observationCaptor.capture(), actionCaptor.capture(), rewardCaptor.capture(), isTerminatedCaptor.capture())

			assertEquals(observationData.getDouble(0, 0), observationCaptor.getValue().getData().getDouble(0, 0), 0.00001)
			assertEquals(1, CInt(Math.Truncate(actionCaptor.getValue())))
			assertEquals(2.0, CDbl(rewardCaptor.getValue()), 0.00001)
			assertFalse(isTerminatedCaptor.getValue())

			verify(updateRuleMock, never()).update(any(GetType(System.Collections.IList)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingHandleNewExperienceAndTrainingBatchIsReady_expect_updateRuleUpdateWithTrainingBatch()
		Public Overridable Sub when_callingHandleNewExperienceAndTrainingBatchIsReady_expect_updateRuleUpdateWithTrainingBatch()
			' Arrange
			Dim observationData As INDArray = Nd4j.rand(1, 1)
			[when](experienceHandlerMock.TrainingBatchReady).thenReturn(True)
			Dim trainingBatch As IList(Of Object) = New List(Of Object)()
			[when](experienceHandlerMock.generateTrainingBatch()).thenReturn(trainingBatch)

			' Act
			sut.handleNewExperience(New Observation(observationData), 1, 2.0, False)

			' Assert
			verify(updateRuleMock, times(1)).update(trainingBatch)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingHandleEpisodeEnd_expect_experienceHandlerSetFinalObservationCalled()
		Public Overridable Sub when_callingHandleEpisodeEnd_expect_experienceHandlerSetFinalObservationCalled()
			' Arrange
			Dim observationData As INDArray = Nd4j.rand(1, 1)
			[when](experienceHandlerMock.TrainingBatchReady).thenReturn(False)

			' Act
			sut.handleEpisodeEnd(New Observation(observationData))

			' Assert
			Dim observationCaptor As ArgumentCaptor(Of Observation) = ArgumentCaptor.forClass(GetType(Observation))
			verify(experienceHandlerMock, times(1)).setFinalObservation(observationCaptor.capture())

			assertEquals(observationData.getDouble(0, 0), observationCaptor.getValue().getData().getDouble(0, 0), 0.00001)

			verify(updateRuleMock, never()).update(any(GetType(System.Collections.IList)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingHandleEpisodeEndAndTrainingBatchIsNotEmpty_expect_updateRuleUpdateWithTrainingBatch()
		Public Overridable Sub when_callingHandleEpisodeEndAndTrainingBatchIsNotEmpty_expect_updateRuleUpdateWithTrainingBatch()
			' Arrange
			Dim observationData As INDArray = Nd4j.rand(1, 1)
			[when](experienceHandlerMock.TrainingBatchReady).thenReturn(True)
			Dim trainingBatch As IList(Of Object) = New List(Of Object)()
			[when](experienceHandlerMock.generateTrainingBatch()).thenReturn(trainingBatch)

			' Act
			sut.handleEpisodeEnd(New Observation(observationData))

			' Assert
			Dim observationCaptor As ArgumentCaptor(Of Observation) = ArgumentCaptor.forClass(GetType(Observation))
			verify(experienceHandlerMock, times(1)).setFinalObservation(observationCaptor.capture())

			assertEquals(observationData.getDouble(0, 0), observationCaptor.getValue().getData().getDouble(0, 0), 0.00001)

			verify(updateRuleMock, times(1)).update(trainingBatch)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notifyBeforeStepAndBatchUnchanged_expect_notifyNewBatchStartedNotCalled()
		Public Overridable Sub when_notifyBeforeStepAndBatchUnchanged_expect_notifyNewBatchStartedNotCalled()
			' Arrange

			' Act
			sut.notifyBeforeStep()

			' Assert
			verify(updateRuleMock, never()).notifyNewBatchStarted()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notifyBeforeStepAndBatchChanged_expect_notifyNewBatchStartedCalledOnce()
		Public Overridable Sub when_notifyBeforeStepAndBatchChanged_expect_notifyNewBatchStartedCalledOnce()
			' Arrange
			[when](experienceHandlerMock.TrainingBatchReady).thenReturn(True)

			' Act
			sut.handleNewExperience(Nothing, 0, 0, False) ' mark as batch has changed
			sut.notifyBeforeStep() ' Should call notify
			sut.notifyBeforeStep() ' Should not call notify

			' Assert
			verify(updateRuleMock, times(1)).notifyNewBatchStarted()
		End Sub

	End Class

End Namespace