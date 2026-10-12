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

Namespace org.deeplearning4j.rl4j.trainer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class SyncTrainerTest
	Public Class SyncTrainerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IAgentLearner agentLearnerMock;
		Friend agentLearnerMock As IAgentLearner

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Builder<org.deeplearning4j.rl4j.agent.IAgentLearner> agentLearnerBuilder;
		Friend agentLearnerBuilder As Builder(Of IAgentLearner)

		Friend sut As SyncTrainer

		Public Overridable Sub setup(ByVal stoppingCondition As System.Predicate(Of SyncTrainer))
			[when](agentLearnerBuilder.build()).thenReturn(agentLearnerMock)
			[when](agentLearnerMock.getEpisodeStepCount()).thenReturn(10)

			sut = New SyncTrainer(agentLearnerBuilder, stoppingCondition)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_training_expect_stoppingConditionWillStopTraining()
		Public Overridable Sub when_training_expect_stoppingConditionWillStopTraining()
			' Arrange
			Dim stoppingCondition As System.Predicate(Of SyncTrainer) = Function(t) t.getEpisodeCount() >= 5 ' Stop after 5 episodes
			setup(stoppingCondition)

			' Act
			sut.train()

			' Assert
			assertEquals(5, sut.EpisodeCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_training_expect_agentIsRun()
		Public Overridable Sub when_training_expect_agentIsRun()
			' Arrange
			Dim stoppingCondition As System.Predicate(Of SyncTrainer) = Function(t) t.getEpisodeCount() >= 5 ' Stop after 5 episodes
			setup(stoppingCondition)

			' Act
			sut.train()

			' Assert
			verify(agentLearnerMock, times(5)).run()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_training_expect_countsAreReset()
		Public Overridable Sub when_training_expect_countsAreReset()
			' Arrange
			Dim stoppingCondition As System.Predicate(Of SyncTrainer) = Function(t) t.getEpisodeCount() >= 5 ' Stop after 5 episodes
			setup(stoppingCondition)

			' Act
			sut.train()
			sut.train()

			' Assert
			assertEquals(5, sut.EpisodeCount)
			assertEquals(50, sut.StepCount)
		End Sub

	End Class

End Namespace