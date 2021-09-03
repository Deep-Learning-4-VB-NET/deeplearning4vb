Imports IAsyncLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.IAsyncLearningConfiguration
Imports TrainingListenerList = org.deeplearning4j.rl4j.learning.listener.TrainingListenerList
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.space
Imports Box = org.deeplearning4j.rl4j.space.Box
Imports org.deeplearning4j.rl4j.space
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports Mockito = org.mockito.Mockito
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.mockito.ArgumentMatchers.any
import static org.mockito.ArgumentMatchers.anyInt
import static org.mockito.ArgumentMatchers.eq
import static org.mockito.Mockito.doAnswer
import static org.mockito.Mockito.mock
import static org.mockito.Mockito.times
import static org.mockito.Mockito.verify
import static org.mockito.Mockito.when

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

Namespace org.deeplearning4j.rl4j.learning.async

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class AsyncThreadTest
	Public Class AsyncThreadTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ActionSpace<org.nd4j.linalg.api.ndarray.INDArray> mockActionSpace;
		Friend mockActionSpace As ActionSpace(Of INDArray)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ObservationSpace<org.deeplearning4j.rl4j.space.Box> mockObservationSpace;
		Friend mockObservationSpace As ObservationSpace(Of Box)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IAsyncLearningConfiguration mockAsyncConfiguration;
		Friend mockAsyncConfiguration As IAsyncLearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock NeuralNet mockNeuralNet;
		Friend mockNeuralNet As NeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IAsyncGlobal<org.deeplearning4j.rl4j.network.NeuralNet> mockAsyncGlobal;
		Friend mockAsyncGlobal As IAsyncGlobal(Of NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock MDP<org.deeplearning4j.rl4j.space.Box, org.nd4j.linalg.api.ndarray.INDArray, org.deeplearning4j.rl4j.space.ActionSpace<org.nd4j.linalg.api.ndarray.INDArray>> mockMDP;
		Friend mockMDP As MDP(Of Box, INDArray, ActionSpace(Of INDArray))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock TrainingListenerList mockTrainingListeners;
'JAVA TO VB CONVERTER NOTE: The field mockTrainingListeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend mockTrainingListeners_Conflict As TrainingListenerList

		Friend observationShape() As Integer = {3, 10, 10}
		Friend actionSize As Integer = 4

		Friend thread As AsyncThread(Of Box, INDArray, ActionSpace(Of INDArray), NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			setupMDPMocks()
			setupThreadMocks()
		End Sub

		Private Sub setupThreadMocks()

			thread = mock(GetType(AsyncThread), Mockito.withSettings().useConstructor(mockMDP, mockTrainingListeners_Conflict, 0, 0).defaultAnswer(Mockito.CALLS_REAL_METHODS))

			[when](thread.getAsyncGlobal()).thenReturn(mockAsyncGlobal)
			[when](thread.Current).thenReturn(mockNeuralNet)
		End Sub

		Private Sub setupMDPMocks()

			[when](mockObservationSpace.Shape).thenReturn(observationShape)
			[when](mockActionSpace.noOp()).thenReturn(Nd4j.zeros(actionSize))

			[when](mockMDP.getObservationSpace()).thenReturn(mockObservationSpace)
			[when](mockMDP.ActionSpace).thenReturn(mockActionSpace)

			Dim dataLength As Integer = 1
			For Each d As Integer In observationShape
				dataLength *= d
			Next d

			[when](mockMDP.reset()).thenReturn(New Box(New Double(dataLength - 1){}))
		End Sub

		Private Sub mockTrainingListeners()
			mockTrainingListeners(False, False)
		End Sub

		Private Sub mockTrainingListeners(ByVal stopOnNotifyNewEpoch As Boolean, ByVal stopOnNotifyEpochTrainingResult As Boolean)
			[when](mockTrainingListeners_Conflict.notifyNewEpoch(eq(thread))).thenReturn(Not stopOnNotifyNewEpoch)
			[when](mockTrainingListeners_Conflict.notifyEpochTrainingResult(eq(thread), any(GetType(IDataManager.StatEntry)))).thenReturn(Not stopOnNotifyEpochTrainingResult)
		End Sub

		Private Sub mockTrainingContext()
			mockTrainingContext(1000, 100, 10)
		End Sub

		Private Sub mockTrainingContext(ByVal maxSteps As Integer, ByVal maxStepsPerEpisode As Integer, ByVal nstep As Integer)

			' Some conditions of this test harness
			Preconditions.checkArgument(maxStepsPerEpisode >= nstep, "episodeLength must be greater than or equal to nstep")
			Preconditions.checkArgument(maxStepsPerEpisode Mod nstep = 0, "episodeLength must be a multiple of nstep")

			Dim mockObs As New Observation(Nd4j.zeros(observationShape))

			[when](mockAsyncConfiguration.MaxEpochStep).thenReturn(maxStepsPerEpisode)
			[when](mockAsyncConfiguration.NStep).thenReturn(nstep)
			[when](thread.Configuration).thenReturn(mockAsyncConfiguration)

			' if we hit the max step count
			[when](mockAsyncGlobal.TrainingComplete).thenAnswer(Function(invocation) thread.getStepCount() >= maxSteps)

			[when](thread.trainSubEpoch(any(GetType(Observation)), anyInt())).thenAnswer(Function(invocationOnMock)
			Dim steps As Integer = invocationOnMock.getArgument(1)
			thread.stepCount += steps
			thread.currentEpisodeStepCount += steps
			Dim isEpisodeComplete As Boolean = thread.getCurrentEpisodeStepCount() Mod maxStepsPerEpisode = 0
			Return New AsyncThread.SubEpochReturn(steps, mockObs, 0.0, 0.0, isEpisodeComplete)
			End Function)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_episodeComplete_expect_neuralNetworkReset()
		Public Overridable Sub when_episodeComplete_expect_neuralNetworkReset()

			' Arrange
			mockTrainingContext(100, 10, 10)
			mockTrainingListeners()

			' Act
			thread.run()

			' Assert
			verify(mockNeuralNet, times(10)).reset() ' there are 10 episodes so the network should be reset between each
			assertEquals(10, thread.getEpochCount()) ' We are performing a training iteration every 10 steps, so there should be 10 epochs
			assertEquals(10, thread.getEpisodeCount()) ' There should be 10 completed episodes
			assertEquals(100, thread.getStepCount()) ' 100 steps overall
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notifyNewEpochReturnsStop_expect_threadStopped()
		Public Overridable Sub when_notifyNewEpochReturnsStop_expect_threadStopped()
			' Arrange
			mockTrainingContext()
			mockTrainingListeners(True, False)

			' Act
			thread.run()

			' Assert
			assertEquals(0, thread.getEpochCount())
			assertEquals(1, thread.getEpisodeCount())
			assertEquals(0, thread.getStepCount())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notifyEpochTrainingResultReturnsStop_expect_threadStopped()
		Public Overridable Sub when_notifyEpochTrainingResultReturnsStop_expect_threadStopped()
			' Arrange
			mockTrainingContext()
			mockTrainingListeners(False, True)

			' Act
			thread.run()

			' Assert
			assertEquals(1, thread.getEpochCount())
			assertEquals(1, thread.getEpisodeCount())
			assertEquals(10, thread.getStepCount()) ' one epoch is by default 10 steps
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_run_expect_preAndPostEpisodeCalled()
		Public Overridable Sub when_run_expect_preAndPostEpisodeCalled()
			' Arrange
			mockTrainingContext(100, 10, 5)
			mockTrainingListeners(False, False)

			' Act
			thread.run()

			' Assert
			assertEquals(20, thread.getEpochCount())
			assertEquals(10, thread.getEpisodeCount())
			assertEquals(100, thread.getStepCount())

			verify(thread, times(10)).preEpisode() ' over 100 steps there will be 10 episodes
			verify(thread, times(10)).postEpisode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_run_expect_trainSubEpochCalledAndResultPassedToListeners()
		Public Overridable Sub when_run_expect_trainSubEpochCalledAndResultPassedToListeners()
			' Arrange
			mockTrainingContext(100, 10, 5)
			mockTrainingListeners(False, False)

			' Act
			thread.run()

			' Assert
			assertEquals(20, thread.getEpochCount())
			assertEquals(10, thread.getEpisodeCount())
			assertEquals(100, thread.getStepCount())

			' Over 100 steps there will be 20 training iterations, so there will be 20 calls to notifyEpochTrainingResult
			verify(mockTrainingListeners_Conflict, times(20)).notifyEpochTrainingResult(eq(thread), any(GetType(IDataManager.StatEntry)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_run_expect_trainSubEpochCalled()
		Public Overridable Sub when_run_expect_trainSubEpochCalled()
			' Arrange
			mockTrainingContext(100, 10, 5)
			mockTrainingListeners(False, False)

			' Act
			thread.run()

			' Assert
			assertEquals(20, thread.getEpochCount())
			assertEquals(10, thread.getEpisodeCount())
			assertEquals(100, thread.getStepCount())

			' There should be 20 calls to trainsubepoch with 5 steps per epoch
			verify(thread, times(20)).trainSubEpoch(any(GetType(Observation)), eq(5))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_remainingEpisodeLengthSmallerThanNSteps_expect_trainSubEpochCalledWithMinimumValue()
		Public Overridable Sub when_remainingEpisodeLengthSmallerThanNSteps_expect_trainSubEpochCalledWithMinimumValue()

			Dim currentEpisodeSteps As Integer = 95
			mockTrainingContext(1000, 100, 10)
			mockTrainingListeners(False, True)

			' want to mock that we are 95 steps into the episode
			doAnswer(Function(invocationOnMock)
			For i As Integer = 0 To currentEpisodeSteps - 1
				thread.incrementSteps()
			Next i
			Return Nothing
			End Function).when(thread).preEpisode()

			mockTrainingListeners(False, True)

			' Act
			thread.run()

			' Assert
			assertEquals(1, thread.getEpochCount())
			assertEquals(1, thread.getEpisodeCount())
			assertEquals(100, thread.getStepCount())

			' There should be 1 call to trainsubepoch with 5 steps as this is the remaining episode steps
			verify(thread, times(1)).trainSubEpoch(any(GetType(Observation)), eq(5))
		End Sub

	End Class

End Namespace