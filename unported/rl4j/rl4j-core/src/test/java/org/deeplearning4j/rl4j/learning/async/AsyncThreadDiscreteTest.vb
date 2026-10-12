Imports org.deeplearning4j.gym
Imports IAsyncLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.IAsyncLearningConfiguration
Imports TrainingListenerList = org.deeplearning4j.rl4j.learning.listener.TrainingListenerList
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.policy
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.space
Imports org.deeplearning4j.rl4j.util
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports Mockito = org.mockito.Mockito
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.mockito.ArgumentMatchers.any
import static org.mockito.ArgumentMatchers.anyInt
import static org.mockito.ArgumentMatchers.eq
import static org.mockito.Mockito.mock
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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class AsyncThreadDiscreteTest
	Public Class AsyncThreadDiscreteTest


		Friend asyncThreadDiscrete As AsyncThreadDiscrete(Of Encodable, NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IAsyncLearningConfiguration mockAsyncConfiguration;
		Friend mockAsyncConfiguration As IAsyncLearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock UpdateAlgorithm<org.deeplearning4j.rl4j.network.NeuralNet> mockUpdateAlgorithm;
		Friend mockUpdateAlgorithm As UpdateAlgorithm(Of NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IAsyncGlobal<org.deeplearning4j.rl4j.network.NeuralNet> mockAsyncGlobal;
		Friend mockAsyncGlobal As IAsyncGlobal(Of NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Policy<Integer> mockGlobalCurrentPolicy;
		Friend mockGlobalCurrentPolicy As Policy(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock NeuralNet mockGlobalTargetNetwork;
		Friend mockGlobalTargetNetwork As NeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock MDP<org.deeplearning4j.rl4j.space.Encodable, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace> mockMDP;
		Friend mockMDP As MDP(Of Encodable, Integer, DiscreteSpace)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock LegacyMDPWrapper<org.deeplearning4j.rl4j.space.Encodable, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace> mockLegacyMDPWrapper;
		Friend mockLegacyMDPWrapper As LegacyMDPWrapper(Of Encodable, Integer, DiscreteSpace)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock DiscreteSpace mockActionSpace;
		Friend mockActionSpace As DiscreteSpace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ObservationSpace<org.deeplearning4j.rl4j.space.Encodable> mockObservationSpace;
		Friend mockObservationSpace As ObservationSpace(Of Encodable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock TrainingListenerList mockTrainingListenerList;
		Friend mockTrainingListenerList As TrainingListenerList

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Observation mockObservation;
		Friend mockObservation As Observation

		Friend observationShape() As Integer = {3, 10, 10}
		Friend actionSize As Integer = 4

		Private Sub setupMDPMocks()

			[when](mockActionSpace.noOp()).thenReturn(0)
			[when](mockMDP.ActionSpace).thenReturn(mockActionSpace)

			[when](mockObservationSpace.Shape).thenReturn(observationShape)
			[when](mockMDP.getObservationSpace()).thenReturn(mockObservationSpace)

		End Sub

		Private Sub setupNNMocks()
			[when](mockAsyncGlobal.Target).thenReturn(mockGlobalTargetNetwork)
			[when](mockGlobalTargetNetwork.clone()).thenReturn(mockGlobalTargetNetwork)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()

			setupMDPMocks()
			setupNNMocks()

			asyncThreadDiscrete = mock(GetType(AsyncThreadDiscrete), Mockito.withSettings().useConstructor(mockAsyncGlobal, mockMDP, mockTrainingListenerList, 0, 0).defaultAnswer(Mockito.CALLS_REAL_METHODS))

			asyncThreadDiscrete.setUpdateAlgorithm(mockUpdateAlgorithm)

			[when](asyncThreadDiscrete.Configuration).thenReturn(mockAsyncConfiguration)
			[when](mockAsyncConfiguration.RewardFactor).thenReturn(1.0)
			[when](asyncThreadDiscrete.getAsyncGlobal()).thenReturn(mockAsyncGlobal)
			[when](asyncThreadDiscrete.getPolicy(eq(mockGlobalTargetNetwork))).thenReturn(mockGlobalCurrentPolicy)

			[when](mockGlobalCurrentPolicy.nextAction(any(GetType(Observation)))).thenReturn(0)

			[when](asyncThreadDiscrete.getLegacyMDPWrapper()).thenReturn(mockLegacyMDPWrapper)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_episodeCompletes_expect_stepsToBeInLineWithEpisodeLenth()
		Public Overridable Sub when_episodeCompletes_expect_stepsToBeInLineWithEpisodeLenth()

			' Arrange
			Dim episodeRemaining As Integer = 5
			Dim remainingTrainingSteps As Integer = 10

			' return done after 4 steps (the episode finishes before nsteps)
			[when](mockMDP.Done).thenAnswer(Function(invocation) asyncThreadDiscrete.StepCount = episodeRemaining)

			[when](mockLegacyMDPWrapper.step(0)).thenReturn(New StepReply(Of )(mockObservation, 0.0, False, Nothing))

			' Act
			Dim subEpochReturn As AsyncThread.SubEpochReturn = asyncThreadDiscrete.trainSubEpoch(mockObservation, remainingTrainingSteps)

			' Assert
			assertTrue(subEpochReturn.isEpisodeComplete())
			assertEquals(5, subEpochReturn.getSteps())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_episodeCompletesDueToMaxStepsReached_expect_isEpisodeComplete()
		Public Overridable Sub when_episodeCompletesDueToMaxStepsReached_expect_isEpisodeComplete()

			' Arrange
			Dim remainingTrainingSteps As Integer = 50

			' Episode does not complete due to MDP
			[when](mockMDP.Done).thenReturn(False)

			[when](mockLegacyMDPWrapper.step(0)).thenReturn(New StepReply(Of )(mockObservation, 0.0, False, Nothing))

			[when](mockAsyncConfiguration.MaxEpochStep).thenReturn(50)

			' Act
			Dim subEpochReturn As AsyncThread.SubEpochReturn = asyncThreadDiscrete.trainSubEpoch(mockObservation, remainingTrainingSteps)

			' Assert
			assertTrue(subEpochReturn.isEpisodeComplete())
			assertEquals(50, subEpochReturn.getSteps())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_episodeLongerThanNsteps_expect_returnNStepLength()
		Public Overridable Sub when_episodeLongerThanNsteps_expect_returnNStepLength()

			' Arrange
			Dim episodeRemaining As Integer = 5
			Dim remainingTrainingSteps As Integer = 4

			' return done after 4 steps (the episode finishes before nsteps)
			[when](mockMDP.Done).thenAnswer(Function(invocation) asyncThreadDiscrete.StepCount = episodeRemaining)

			[when](mockLegacyMDPWrapper.step(0)).thenReturn(New StepReply(Of )(mockObservation, 0.0, False, Nothing))

			' Act
			Dim subEpochReturn As AsyncThread.SubEpochReturn = asyncThreadDiscrete.trainSubEpoch(mockObservation, remainingTrainingSteps)

			' Assert
			assertFalse(subEpochReturn.isEpisodeComplete())
			assertEquals(remainingTrainingSteps, subEpochReturn.getSteps())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_framesAreSkipped_expect_proportionateStepCounterUpdates()
		Public Overridable Sub when_framesAreSkipped_expect_proportionateStepCounterUpdates()
			Dim skipFrames As Integer = 2
			Dim remainingTrainingSteps As Integer = 10

			' Episode does not complete due to MDP
			[when](mockMDP.Done).thenReturn(False)

			Dim stepCount As New AtomicInteger()

			' Use skipFrames to return if observations are skipped or not
			[when](mockLegacyMDPWrapper.step(anyInt())).thenAnswer(Function(invocationOnMock)
			Dim isSkipped As Boolean = stepCount.incrementAndGet() Mod skipFrames <> 0
			Dim mockObs As Observation = If(isSkipped, Observation.SkippedObservation, New Observation(Nd4j.create(observationShape)))
			Return New StepReply(Of )(mockObs, 0.0, False, Nothing)
			End Function)


			' Act
			Dim subEpochReturn As AsyncThread.SubEpochReturn = asyncThreadDiscrete.trainSubEpoch(mockObservation, remainingTrainingSteps)

			' Assert
			assertFalse(subEpochReturn.isEpisodeComplete())
			assertEquals(remainingTrainingSteps, subEpochReturn.getSteps())
			assertEquals((remainingTrainingSteps - 1) * skipFrames + 1, stepCount.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_preEpisodeCalled_expect_experienceHandlerReset()
		Public Overridable Sub when_preEpisodeCalled_expect_experienceHandlerReset()

			' Arrange
			Dim trainingSteps As Integer = 100
			For i As Integer = 0 To trainingSteps - 1
				asyncThreadDiscrete.getExperienceHandler().addExperience(mockObservation, 0, 0.0, False)
			Next i

			Dim experienceHandlerSizeBeforeReset As Integer = asyncThreadDiscrete.getExperienceHandler().getTrainingBatchSize()

			' Act
			asyncThreadDiscrete.preEpisode()

			' Assert
			assertEquals(100, experienceHandlerSizeBeforeReset)
			assertEquals(0, asyncThreadDiscrete.getExperienceHandler().getTrainingBatchSize())


		End Sub

	End Class

End Namespace