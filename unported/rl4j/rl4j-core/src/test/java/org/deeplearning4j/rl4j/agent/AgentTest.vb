Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.agent.listener
Imports org.deeplearning4j.rl4j.environment
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports org.junit.jupiter.api.Assertions
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports org.mockito
Imports MockitoException = org.mockito.exceptions.base.MockitoException
Imports org.mockito.junit
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.rl4j.agent

	'import org.junit.platform.runner.JUnitPlatform;


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class AgentTest
	Public Class AgentTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Environment environmentMock;
		Friend environmentMock As Environment
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock TransformProcess transformProcessMock;
		Friend transformProcessMock As TransformProcess
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IPolicy policyMock;
		Friend policyMock As IPolicy
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock AgentListener listenerMock;
		Friend listenerMock As AgentListener



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildingWithNullEnvironment_expect_exception()
		Public Overridable Sub when_buildingWithNullEnvironment_expect_exception()
			Try
				Dim tempVar As New Agent(Nothing, Nothing, Nothing, Nothing, Nothing)
				fail("NullPointerException should have been thrown")
			Catch exception As System.NullReferenceException
				Dim expectedMessage As String = "environment is marked non-null but is null"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildingWithNullTransformProcess_expect_exception()
		Public Overridable Sub when_buildingWithNullTransformProcess_expect_exception()
			Try
				Dim tempVar As New Agent(environmentMock, Nothing, Nothing, Nothing, Nothing)
				fail("NullPointerException should have been thrown")
			Catch exception As System.NullReferenceException
				Dim expectedMessage As String = "transformProcess is marked non-null but is null"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildingWithNullPolicy_expect_exception()
		Public Overridable Sub when_buildingWithNullPolicy_expect_exception()
			Try
				Dim tempVar As New Agent(environmentMock, transformProcessMock, Nothing, Nothing, Nothing)
				fail("NullPointerException should have been thrown")
			Catch exception As System.NullReferenceException
				Dim expectedMessage As String = "policy is marked non-null but is null"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildingWithNullConfiguration_expect_exception()
		Public Overridable Sub when_buildingWithNullConfiguration_expect_exception()
			Try
				Dim tempVar As New Agent(environmentMock, transformProcessMock, policyMock, Nothing, Nothing)
				fail("NullPointerException should have been thrown")
			Catch exception As System.NullReferenceException
				Dim expectedMessage As String = "configuration is marked non-null but is null"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildingWithInvalidMaxSteps_expect_exception()
		Public Overridable Sub when_buildingWithInvalidMaxSteps_expect_exception()
			Try
				Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(0).build()
				Dim tempVar As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "Configuration: maxEpisodeSteps must be null (no maximum) or greater than 0, got [0]"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildingWithId_expect_idSetInAgent()
		Public Overridable Sub when_buildingWithId_expect_idSetInAgent()
			' Arrange
			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, "TestAgent")

			' Assert
			assertEquals("TestAgent", sut.getId())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_runIsCalled_expect_agentIsReset()
		Public Overridable Sub when_runIsCalled_expect_agentIsReset()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))
			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(1)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			[when](listenerMock.onBeforeStep(any(GetType(Agent)), any(GetType(Observation)), anyInt())).thenReturn(AgentListener.ListenerResponse.STOP)
			sut.addListener(listenerMock)

			' Act
			sut.run()

			' Assert
			assertEquals(0, sut.getEpisodeStepCount())
			verify(transformProcessMock).transform(envResetResult, 0, False)
			verify(policyMock, times(1)).reset()
			assertEquals(0.0, sut.getReward(), 0.00001)
			verify(environmentMock, times(1)).reset()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_runIsCalled_expect_onBeforeAndAfterEpisodeCalled()
		Public Overridable Sub when_runIsCalled_expect_onBeforeAndAfterEpisodeCalled()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))
			[when](environmentMock.isEpisodeFinished()).thenReturn(True)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)
			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(spy, times(1)).onBeforeEpisode()
			verify(spy, times(1)).onAfterEpisode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_onBeforeEpisodeReturnsStop_expect_performStepAndOnAfterEpisodeNotCalled()
		Public Overridable Sub when_onBeforeEpisodeReturnsStop_expect_performStepAndOnAfterEpisodeNotCalled()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			[when](listenerMock.onBeforeEpisode(any(GetType(Agent)))).thenReturn(AgentListener.ListenerResponse.STOP)
			sut.addListener(listenerMock)

			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(spy, times(1)).onBeforeEpisode()
			verify(spy, never()).performStep()
			verify(spy, never()).onAfterStep(any(GetType(StepResult)))
			verify(spy, never()).onAfterEpisode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_runIsCalledWithoutMaxStep_expect_agentRunUntilEpisodeIsFinished()
		Public Overridable Sub when_runIsCalledWithoutMaxStep_expect_agentRunUntilEpisodeIsFinished()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Agent spy = Mockito.spy(sut);
			Dim spy As Agent = Mockito.spy(sut)

			doAnswer(Function(invocation)
			CType(invocation.getMock(), Agent).incrementEpisodeStepCount()
			Return Nothing
			End Function).when(spy).performStep()
			[when](environmentMock.isEpisodeFinished()).thenAnswer(Function(invocation) spy.getEpisodeStepCount() >= 5)

			' Act
			spy.run()

			' Assert
			verify(spy, times(1)).onBeforeEpisode()
			verify(spy, times(5)).performStep()
			verify(spy, times(1)).onAfterEpisode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_maxStepsIsReachedBeforeEposideEnds_expect_runTerminated()
		Public Overridable Sub when_maxStepsIsReachedBeforeEposideEnds_expect_runTerminated()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(3).build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Agent spy = Mockito.spy(sut);
			Dim spy As Agent = Mockito.spy(sut)

			doAnswer(Function(invocation)
			CType(invocation.getMock(), Agent).incrementEpisodeStepCount()
			Return Nothing
			End Function).when(spy).performStep()

			' Act
			spy.run()

			' Assert
			verify(spy, times(1)).onBeforeEpisode()
			verify(spy, times(3)).performStep()
			verify(spy, times(1)).onAfterEpisode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_initialObservationsAreSkipped_expect_performNoOpAction()
		Public Overridable Sub when_initialObservationsAreSkipped_expect_performNoOpAction()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(Observation.SkippedObservation)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			[when](listenerMock.onBeforeStep(any(GetType(Agent)), any(GetType(Observation)), any())).thenReturn(AgentListener.ListenerResponse.STOP)
			sut.addListener(listenerMock)

			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(listenerMock).onBeforeStep(any(), any(), eq(-1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_initialObservationsAreSkipped_expect_performNoOpActionAnd()
		Public Overridable Sub when_initialObservationsAreSkipped_expect_performNoOpActionAnd()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(Observation.SkippedObservation)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			[when](listenerMock.onBeforeStep(any(GetType(Agent)), any(GetType(Observation)), any())).thenReturn(AgentListener.ListenerResponse.STOP)
			sut.addListener(listenerMock)

			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(listenerMock).onBeforeStep(any(), any(), eq(-1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_observationsIsSkipped_expect_performLastAction()
		Public Overridable Sub when_observationsIsSkipped_expect_performLastAction()
			' Arrange
			Dim envResetResult As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(envResetResult)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(New StepResult(envResetResult, 0.0, False))
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](policyMock.nextAction(any(GetType(Observation)))).thenAnswer(Function(invocation) CInt(Math.Truncate(CType(invocation.getArgument(0), Observation).Data.getDouble(0))))

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(3).build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			Dim spy As Agent = Mockito.spy(sut)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenAnswer(Function(invocation)
			Dim stepNumber As Integer = CInt(Math.Truncate(invocation.getArgument(1)))
			Return If(stepNumber Mod 2 = 1, Observation.SkippedObservation, New Observation(Nd4j.create(New Double() { stepNumber })))
			End Function)

			sut.addListener(listenerMock)

			' Act
			spy.run()

			' Assert
			verify(policyMock, times(2)).nextAction(any(GetType(Observation)))

			Dim agentCaptor As ArgumentCaptor(Of Agent) = ArgumentCaptor.forClass(GetType(Agent))
			Dim observationCaptor As ArgumentCaptor(Of Observation) = ArgumentCaptor.forClass(GetType(Observation))
			Dim actionCaptor As ArgumentCaptor(Of Integer) = ArgumentCaptor.forClass(GetType(Integer))
			verify(listenerMock, times(3)).onBeforeStep(agentCaptor.capture(), observationCaptor.capture(), actionCaptor.capture())
			Dim capturedActions As IList(Of Integer) = actionCaptor.getAllValues()
			assertEquals(0, CInt(capturedActions(0)))
			assertEquals(0, CInt(capturedActions(1)))
			assertEquals(2, CInt(capturedActions(2)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_onBeforeStepReturnsStop_expect_performStepAndOnAfterEpisodeNotCalled()
		Public Overridable Sub when_onBeforeStepReturnsStop_expect_performStepAndOnAfterEpisodeNotCalled()
			' Arrange
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			[when](listenerMock.onBeforeStep(any(GetType(Agent)), any(GetType(Observation)), any())).thenReturn(AgentListener.ListenerResponse.STOP)
			sut.addListener(listenerMock)

			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(spy, times(1)).onBeforeEpisode()
			verify(spy, times(1)).onBeforeStep()
			verify(spy, never()).act(any())
			verify(spy, never()).onAfterStep(any(GetType(StepResult)))
			verify(spy, never()).onAfterEpisode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_observationIsNotSkipped_expect_policyActionIsSentToEnvironment()
		Public Overridable Sub when_observationIsNotSkipped_expect_policyActionIsSentToEnvironment()
			' Arrange
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(New StepResult(New Dictionary(Of )(), 0.0, False))

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(123)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(1).build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			' Act
			sut.run()

			' Assert
			verify(environmentMock, times(1)).step(123)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_stepResultIsReceived_expect_observationAndRewardUpdated()
		Public Overridable Sub when_stepResultIsReceived_expect_observationAndRewardUpdated()
			' Arrange
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(New StepResult(New Dictionary(Of )(), 234.0, False))

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(123)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(1).build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)

			' Act
			sut.run()

			' Assert
			assertEquals(123.0, sut.getObservation().getData().getDouble(0), 0.00001)
			assertEquals(234.0, sut.getReward(), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_stepIsDone_expect_onAfterStepAndWithStepResult()
		Public Overridable Sub when_stepIsDone_expect_onAfterStepAndWithStepResult()
			' Arrange
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)
			Dim stepResult As New StepResult(New Dictionary(Of )(), 234.0, False)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(stepResult)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(123)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(1).build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)
			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(spy).onAfterStep(stepResult)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_onAfterStepReturnsStop_expect_onAfterEpisodeNotCalled()
		Public Overridable Sub when_onAfterStepReturnsStop_expect_onAfterEpisodeNotCalled()
			' Arrange
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)
			Dim stepResult As New StepResult(New Dictionary(Of )(), 234.0, False)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(stepResult)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(123)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(1).build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)
			[when](listenerMock.onAfterStep(any(GetType(Agent)), any(GetType(StepResult)))).thenReturn(AgentListener.ListenerResponse.STOP)
			sut.addListener(listenerMock)

			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(spy, never()).onAfterEpisode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_runIsCalled_expect_onAfterEpisodeIsCalled()
		Public Overridable Sub when_runIsCalled_expect_onAfterEpisodeIsCalled()
			' Arrange
			Dim schema As New Schema(New IntegerActionSchema(0, -1))
			[when](environmentMock.reset()).thenReturn(New Dictionary(Of )())
			[when](environmentMock.getSchema()).thenReturn(schema)
			Dim stepResult As New StepResult(New Dictionary(Of )(), 234.0, False)
			[when](environmentMock.step(any(GetType(Integer)))).thenReturn(stepResult)

			[when](transformProcessMock.transform(any(GetType(System.Collections.IDictionary)), anyInt(), anyBoolean())).thenReturn(New Observation(Nd4j.create(New Double() { 123.0 })))

			[when](policyMock.nextAction(any(GetType(Observation)))).thenReturn(123)

			Dim configuration As Agent.Configuration = Agent.Configuration.builder().maxEpisodeSteps(1).build()
			Dim sut As New Agent(environmentMock, transformProcessMock, policyMock, configuration, Nothing)
			sut.addListener(listenerMock)
			Dim spy As Agent = Mockito.spy(sut)

			' Act
			spy.run()

			' Assert
			verify(spy, times(1)).onAfterEpisode()
			verify(listenerMock, times(1)).onAfterEpisode(any())
		End Sub
	End Class

End Namespace