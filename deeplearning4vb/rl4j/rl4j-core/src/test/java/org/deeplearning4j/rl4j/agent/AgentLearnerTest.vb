Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.agent.learning.behavior
Imports org.deeplearning4j.rl4j.environment
Imports IntegerActionSchema = org.deeplearning4j.rl4j.environment.IntegerActionSchema
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
Imports Mock = org.mockito.Mock
Imports InvocationOnMock = org.mockito.invocation.InvocationOnMock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports Answer = org.mockito.stubbing.Answer
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.mockito.ArgumentMatchers
Imports org.mockito.Mockito
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

Namespace org.deeplearning4j.rl4j.agent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class AgentLearnerTest
	Public Class AgentLearnerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Environment<Integer> environmentMock;
		Friend environmentMock As Environment(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock TransformProcess transformProcessMock;
		Friend transformProcessMock As TransformProcess

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IPolicy<Integer> policyMock;
		Friend policyMock As IPolicy(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock LearningBehavior<Integer, Object> learningBehaviorMock;
		Friend learningBehaviorMock As LearningBehavior(Of Integer, Object)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_episodeIsStarted_expect_learningBehaviorHandleEpisodeStartCalled()
		Public Overridable Sub when_episodeIsStarted_expect_learningBehaviorHandleEpisodeStartCalled()
			' Arrange
			Dim configuration As AgentLearner.Configuration = AgentLearner.Configuration.builder().maxEpisodeSteps(3).build()
			Dim sut As AgentLearner(Of Integer) = New AgentLearner(environmentMock, transformProcessMock, policyMock, configuration, Nothing, learningBehaviorMock)

			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)
			Dim stepResult As New StepResult(New Dictionary(Of )(), 234.0, False)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(stepResult)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(123)

			' Act
			sut.run()

			' Assert
			verify(learningBehaviorMock, times(1)).handleEpisodeStart()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_runIsCalled_expect_experienceHandledWithLearningBehavior()
		Public Overridable Sub when_runIsCalled_expect_experienceHandledWithLearningBehavior()
			' Arrange
			Dim configuration As AgentLearner.Configuration = AgentLearner.Configuration.builder().maxEpisodeSteps(4).build()
			Dim sut As AgentLearner(Of Integer) = New AgentLearner(environmentMock, transformProcessMock, policyMock, configuration, Nothing, learningBehaviorMock)

			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.getSchema()).thenReturn(schema)
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())

			Dim reward() As Double = { 0.0 }
			reward(0) += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: when(environmentMock.step(any(Integer.class))).thenAnswer(a -> new org.deeplearning4j.rl4j.environment.StepResult(new java.util.HashMap<>(), ++reward[0], reward[0] == 4.0));
			[when](environmentMock.step(any(GetType(Integer)))).thenAnswer(Function(a) New StepResult(New Dictionary(Of )(), reward(0), reward(0) = 4.0))

			[when](environmentMock.EpisodeFinished).thenAnswer(Function(x) reward(0) = 4.0)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenAnswer(New AnswerAnonymousInnerClass(Me))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenAnswer(Function(x) CInt(Math.Truncate(reward(0))))

			' Act
			sut.run()

			' Assert
			Dim observationCaptor As ArgumentCaptor(Of Observation) = ArgumentCaptor.forClass(GetType(Observation))
			Dim actionCaptor As ArgumentCaptor(Of Integer) = ArgumentCaptor.forClass(GetType(Integer))
			Dim rewardCaptor As ArgumentCaptor(Of Double) = ArgumentCaptor.forClass(GetType(Double))
			Dim isTerminalCaptor As ArgumentCaptor(Of Boolean) = ArgumentCaptor.forClass(GetType(Boolean))

			verify(learningBehaviorMock, times(2)).handleNewExperience(observationCaptor.capture(), actionCaptor.capture(), rewardCaptor.capture(), isTerminalCaptor.capture())
			Dim observations As IList(Of Observation) = observationCaptor.getAllValues()
			Dim actions As IList(Of Integer) = actionCaptor.getAllValues()
			Dim rewards As IList(Of Double) = rewardCaptor.getAllValues()
			Dim isTerminalList As IList(Of Boolean) = isTerminalCaptor.getAllValues()

			assertEquals(0.0, observations(0).getData().getDouble(0), 0.00001)
			assertEquals(0, CInt(actions(0)))
			assertEquals(0.0 + 1.0, rewards(0), 0.00001)
			assertFalse(isTerminalList(0))

			assertEquals(2.2, observations(1).getData().getDouble(0), 0.00001)
			assertEquals(2, CInt(actions(1)))
			assertEquals(2.0 + 3.0, rewards(1), 0.00001)
			assertFalse(isTerminalList(1))

			Dim finalObservationCaptor As ArgumentCaptor(Of Observation) = ArgumentCaptor.forClass(GetType(Observation))
			verify(learningBehaviorMock, times(1)).handleEpisodeEnd(finalObservationCaptor.capture())
			assertEquals(4.4, finalObservationCaptor.getValue().getData().getDouble(0), 0.00001)
		End Sub

		Private Class AnswerAnonymousInnerClass
			Inherits Answer(Of Observation)

			Private ReadOnly outerInstance As AgentLearnerTest

			Public Sub New(ByVal outerInstance As AgentLearnerTest)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.rl4j.observation.Observation answer(org.mockito.invocation.InvocationOnMock invocation) throws Throwable
			Public Function answer(ByVal invocation As InvocationOnMock) As Observation
				Dim [step] As Integer = CInt(Math.Truncate(invocation.getArgument(1)))
				Dim isTerminal As Boolean = CBool(invocation.getArgument(2))
				Return If([step] Mod 2 = 0 OrElse isTerminal, New Observation(Nd4j.create(New Double() { [step] * 1.1 })), Observation.SkippedObservation)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_runIsCalledMultipleTimes_expect_rewardSentToLearningBehaviorToBeCorrect()
		Public Overridable Sub when_runIsCalledMultipleTimes_expect_rewardSentToLearningBehaviorToBeCorrect()
			' Arrange
			Dim configuration As AgentLearner.Configuration = AgentLearner.Configuration.builder().maxEpisodeSteps(4).build()
			Dim sut As AgentLearner(Of Integer) = New AgentLearner(environmentMock, transformProcessMock, policyMock, configuration, Nothing, learningBehaviorMock)

			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.getSchema()).thenReturn(schema)
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())

			Dim reward() As Double = { 0.0 }
			reward(0) += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: when(environmentMock.step(any(Integer.class))).thenAnswer(a -> new org.deeplearning4j.rl4j.environment.StepResult(new java.util.HashMap<>(), ++reward[0], reward[0] == 4.0));
			[when](environmentMock.step(any(GetType(Integer)))).thenAnswer(Function(a) New StepResult(New Dictionary(Of )(), reward(0), reward(0) = 4.0))

			[when](environmentMock.EpisodeFinished).thenAnswer(Function(x) reward(0) = 4.0)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenAnswer(New AnswerAnonymousInnerClass2(Me))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenAnswer(Function(x) CInt(Math.Truncate(reward(0))))

			' Act
			sut.run()
			reward(0) = 0.0
			sut.run()

			' Assert
			Dim rewardCaptor As ArgumentCaptor(Of Double) = ArgumentCaptor.forClass(GetType(Double))

			verify(learningBehaviorMock, times(4)).handleNewExperience(any(GetType(Observation)), any(GetType(Integer)), rewardCaptor.capture(), any(GetType(Boolean)))
			Dim rewards As IList(Of Double) = rewardCaptor.getAllValues()

			' rewardAtLastExperience at the end of 1st call to .run() should not leak into 2nd call.
			assertEquals(0.0 + 1.0, rewards(2), 0.00001)
			assertEquals(2.0 + 3.0, rewards(3), 0.00001)
		End Sub

		Private Class AnswerAnonymousInnerClass2
			Inherits Answer(Of Observation)

			Private ReadOnly outerInstance As AgentLearnerTest

			Public Sub New(ByVal outerInstance As AgentLearnerTest)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.rl4j.observation.Observation answer(org.mockito.invocation.InvocationOnMock invocation) throws Throwable
			Public Function answer(ByVal invocation As InvocationOnMock) As Observation
				Dim [step] As Integer = CInt(Math.Truncate(invocation.getArgument(1)))
				Dim isTerminal As Boolean = CBool(invocation.getArgument(2))
				Return If([step] Mod 2 = 0 OrElse isTerminal, New Observation(Nd4j.create(New Double() { [step] * 1.1 })), Observation.SkippedObservation)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_aStepWillBeTaken_expect_learningBehaviorNotified()
		Public Overridable Sub when_aStepWillBeTaken_expect_learningBehaviorNotified()
			' Arrange
			Dim configuration As AgentLearner.Configuration = AgentLearner.Configuration.builder().maxEpisodeSteps(1).build()
			Dim sut As AgentLearner(Of Integer) = New AgentLearner(environmentMock, transformProcessMock, policyMock, configuration, Nothing, learningBehaviorMock)

			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)
			Dim stepResult As New StepResult(New Dictionary(Of )(), 234.0, False)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(stepResult)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(123)

			' Act
			sut.run()

			' Assert
			verify(learningBehaviorMock, times(1)).notifyBeforeStep()
		End Sub

	End Class
End Namespace