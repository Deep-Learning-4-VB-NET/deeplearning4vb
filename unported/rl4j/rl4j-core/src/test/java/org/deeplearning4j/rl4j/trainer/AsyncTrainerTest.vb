Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.rl4j.trainer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class AsyncTrainerTest
	Public Class AsyncTrainerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Builder<org.deeplearning4j.rl4j.agent.IAgentLearner<Integer>> agentLearnerBuilderMock;
		Friend agentLearnerBuilderMock As Builder(Of IAgentLearner(Of Integer))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Predicate<AsyncTrainer<Integer>> stoppingConditionMock;
		Friend stoppingConditionMock As Predicate(Of AsyncTrainer(Of Integer))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IAgentLearner<Integer> agentLearnerMock;
		Friend agentLearnerMock As IAgentLearner(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			[when](agentLearnerBuilderMock.build()).thenReturn(agentLearnerMock)
			[when](agentLearnerMock.EpisodeStepCount).thenReturn(100)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_ctorIsCalledWithInvalidNumberOfThreads_expect_Exception()
		Public Overridable Sub when_ctorIsCalledWithInvalidNumberOfThreads_expect_Exception()
			Try
				Dim sut As New AsyncTrainer(agentLearnerBuilderMock, stoppingConditionMock, 0)
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "numThreads must be greater than 0, got:  [0]"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_runningWith2Threads_expect_2AgentLearnerCreated()
		Public Overridable Sub when_runningWith2Threads_expect_2AgentLearnerCreated()
			' Arrange
			Dim stoppingCondition As System.Predicate(Of AsyncTrainer(Of Integer)) = Function(t) True
			Dim sut As New AsyncTrainer(agentLearnerBuilderMock, stoppingCondition, 2)

			' Act
			sut.train()

			' Assert
			verify(agentLearnerBuilderMock, times(2)).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_stoppingConditionTriggered_expect_agentLearnersStopsAndCountersAreCorrect()
		Public Overridable Sub when_stoppingConditionTriggered_expect_agentLearnersStopsAndCountersAreCorrect()
			' Arrange
			Dim stoppingConditionHitCount As New AtomicInteger(0)
			Dim stoppingCondition As System.Predicate(Of AsyncTrainer(Of Integer)) = Function(t) stoppingConditionHitCount.incrementAndGet() >= 5
			Dim sut As New AsyncTrainer(Of Integer)(agentLearnerBuilderMock, stoppingCondition, 2)

			' Act
			sut.train()

			' Assert
			assertEquals(6, stoppingConditionHitCount.get())
			assertEquals(6, sut.EpisodeCount)
			assertEquals(600, sut.StepCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_training_expect_countsAreReset()
		Public Overridable Sub when_training_expect_countsAreReset()
			' Arrange
			Dim stoppingConditionHitCount As New AtomicInteger(0)
			Dim stoppingCondition As System.Predicate(Of AsyncTrainer(Of Integer)) = Function(t) stoppingConditionHitCount.incrementAndGet() >= 5
			Dim sut As New AsyncTrainer(Of Integer)(agentLearnerBuilderMock, stoppingCondition, 2)

			' Act
			sut.train()
			stoppingConditionHitCount.set(0)
			sut.train()

			' Assert
			assertEquals(6, sut.EpisodeCount)
			assertEquals(600, sut.StepCount)
		End Sub
	End Class

End Namespace