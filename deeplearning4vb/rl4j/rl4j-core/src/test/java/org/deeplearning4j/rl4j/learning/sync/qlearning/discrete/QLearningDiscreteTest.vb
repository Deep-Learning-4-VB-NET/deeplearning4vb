Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.agent.learning.behavior
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports QLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.QLearningConfiguration
Imports org.deeplearning4j.rl4j.learning.sync.qlearning
Imports org.deeplearning4j.rl4j.mdp
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports NeuralNetOutput = org.deeplearning4j.rl4j.network.NeuralNetOutput
Imports org.deeplearning4j.rl4j.network.dqn
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports org.deeplearning4j.rl4j.space
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports Mockito = org.mockito.Mockito
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.mockito.ArgumentMatchers.any
import static org.mockito.ArgumentMatchers.anyInt
import static org.mockito.ArgumentMatchers.eq
import static org.mockito.Mockito.mock
import static org.mockito.Mockito.never
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

Namespace org.deeplearning4j.rl4j.learning.sync.qlearning.discrete

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) public class QLearningDiscreteTest
	Public Class QLearningDiscreteTest

		Friend qLearningDiscrete As QLearningDiscrete(Of Encodable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IHistoryProcessor mockHistoryProcessor;
'JAVA TO VB CONVERTER NOTE: The field mockHistoryProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Friend mockHistoryProcessor_Conflict As IHistoryProcessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IHistoryProcessor.Configuration mockHistoryConfiguration;
		Friend mockHistoryConfiguration As IHistoryProcessor.Configuration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock MDP<org.deeplearning4j.rl4j.space.Encodable, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace> mockMDP;
		Friend mockMDP As MDP(Of Encodable, Integer, DiscreteSpace)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock DiscreteSpace mockActionSpace;
		Friend mockActionSpace As DiscreteSpace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ObservationSpace<org.deeplearning4j.rl4j.space.Encodable> mockObservationSpace;
		Friend mockObservationSpace As ObservationSpace(Of Encodable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IDQN mockDQN;
		Friend mockDQN As IDQN

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock QLearningConfiguration mockQlearningConfiguration;
		Friend mockQlearningConfiguration As QLearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ILearningBehavior<Integer> learningBehavior;
		Friend learningBehavior As ILearningBehavior(Of Integer)

		' HWC
		Friend observationShape() As Integer = {3, 10, 10}
		Friend totalObservationSize As Integer = 1

		Private Sub setupMDPMocks()

			[when](mockObservationSpace.Shape).thenReturn(observationShape)

			[when](mockMDP.getObservationSpace()).thenReturn(mockObservationSpace)
			[when](mockMDP.ActionSpace).thenReturn(mockActionSpace)

			Dim dataLength As Integer = 1
			For Each d As Integer In observationShape
				dataLength *= d
			Next d
		End Sub


		Private Sub mockTestContext(ByVal maxSteps As Integer, ByVal updateStart As Integer, ByVal batchSize As Integer, ByVal rewardFactor As Double, ByVal maxExperienceReplay As Integer, ByVal learningBehavior As ILearningBehavior(Of Integer))
			[when](mockQlearningConfiguration.getBatchSize()).thenReturn(batchSize)
			[when](mockQlearningConfiguration.RewardFactor).thenReturn(rewardFactor)
			[when](mockQlearningConfiguration.getExpRepMaxSize()).thenReturn(maxExperienceReplay)
			[when](mockQlearningConfiguration.getSeed()).thenReturn(123L)
			[when](mockQlearningConfiguration.getTargetDqnUpdateFreq()).thenReturn(1)
			[when](mockDQN.clone()).thenReturn(mockDQN)

			If learningBehavior IsNot Nothing Then
				qLearningDiscrete = mock(GetType(QLearningDiscrete), Mockito.withSettings().useConstructor(mockMDP, mockDQN, mockQlearningConfiguration, 0, learningBehavior, Nd4j.Random).defaultAnswer(Mockito.CALLS_REAL_METHODS))
			Else
				qLearningDiscrete = mock(GetType(QLearningDiscrete), Mockito.withSettings().useConstructor(mockMDP, mockDQN, mockQlearningConfiguration, 0).defaultAnswer(Mockito.CALLS_REAL_METHODS))
			End If
		End Sub

		Private Sub mockHistoryProcessor(ByVal skipFrames As Integer)
			[when](mockHistoryConfiguration.getRescaledHeight()).thenReturn(observationShape(1))
			[when](mockHistoryConfiguration.getRescaledWidth()).thenReturn(observationShape(2))

			[when](mockHistoryConfiguration.getOffsetX()).thenReturn(0)
			[when](mockHistoryConfiguration.getOffsetY()).thenReturn(0)

			[when](mockHistoryConfiguration.getCroppingHeight()).thenReturn(observationShape(1))
			[when](mockHistoryConfiguration.getCroppingWidth()).thenReturn(observationShape(2))
			[when](mockHistoryConfiguration.getSkipFrame()).thenReturn(skipFrames)
			[when](mockHistoryConfiguration.getHistoryLength()).thenReturn(1)
			[when](mockHistoryProcessor_Conflict.Conf).thenReturn(mockHistoryConfiguration)

			qLearningDiscrete.HistoryProcessor = mockHistoryProcessor_Conflict
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			setupMDPMocks()

			For Each i As Integer In observationShape
				totalObservationSize *= i
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_singleTrainStep_expect_correctValues()
		Public Overridable Sub when_singleTrainStep_expect_correctValues()

			' Arrange
			mockTestContext(100,0,2,1.0, 10, Nothing)

			' An example observation and 2 Q values output (2 actions)
			Dim observation As New Observation(Nd4j.zeros(observationShape))
			Dim netOutputResult As New NeuralNetOutput()
			netOutputResult.put(CommonOutputNames.QValues, Nd4j.create(New Single() {1.0f, 0.5f}))
			[when](mockDQN.output(eq(observation))).thenReturn(netOutputResult)

			[when](mockMDP.step(anyInt())).thenReturn(New StepReply(Of )(New Observation(Nd4j.zeros(observationShape)), 0, False, Nothing))

			' Act
			Dim stepReturn As QLearning.QLStepReturn(Of Observation) = qLearningDiscrete.trainStep(observation)

			' Assert
			assertEquals(1.0, stepReturn.getMaxQ(), 1e-5)

			Dim stepReply As StepReply(Of Observation) = stepReturn.getStepReply()

			assertEquals(0, stepReply.getReward(), 1e-5)
			assertFalse(stepReply.isDone())
			assertFalse(stepReply.getObservation().isSkipped())
			assertEquals(observation.Data.reshape(observationShape), stepReply.getObservation().getData().reshape(observationShape))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_singleTrainStepSkippedFrames_expect_correctValues()
		Public Overridable Sub when_singleTrainStepSkippedFrames_expect_correctValues()
			' Arrange
			mockTestContext(100,0,2,1.0, 10, learningBehavior)

			Dim skippedObservation As Observation = Observation.SkippedObservation
			Dim nextObservation As New Observation(Nd4j.zeros(observationShape))

			[when](mockMDP.step(anyInt())).thenReturn(New StepReply(Of )(nextObservation, 0, False, Nothing))

			' Act
			Dim stepReturn As QLearning.QLStepReturn(Of Observation) = qLearningDiscrete.trainStep(skippedObservation)

			' Assert
			assertEquals(Double.NaN, stepReturn.getMaxQ(), 1e-5)

			Dim stepReply As StepReply(Of Observation) = stepReturn.getStepReply()

			assertEquals(0, stepReply.getReward(), 1e-5)
			assertFalse(stepReply.isDone())
			assertFalse(stepReply.getObservation().isSkipped())

			verify(learningBehavior, never()).handleNewExperience(any(GetType(Observation)), any(GetType(Integer)), any(GetType(Double)), any(GetType(Boolean)))
			verify(mockDQN, never()).output(any(GetType(INDArray)))

		End Sub

		'TODO: there are much more test cases here that can be improved upon

	End Class

End Namespace