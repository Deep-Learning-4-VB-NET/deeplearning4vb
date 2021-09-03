Imports IAsyncLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.IAsyncLearningConfiguration
Imports TrainingListener = org.deeplearning4j.rl4j.learning.listener.TrainingListener
Imports org.deeplearning4j.rl4j.network
Imports org.deeplearning4j.rl4j.space
Imports Box = org.deeplearning4j.rl4j.space.Box
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
import static org.mockito.ArgumentMatchers.eq
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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class AsyncLearningTest
	Public Class AsyncLearningTest

		Friend asyncLearning As AsyncLearning(Of Box, INDArray, ActionSpace(Of INDArray), NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock TrainingListener mockTrainingListener;
		Friend mockTrainingListener As TrainingListener

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock AsyncGlobal<org.deeplearning4j.rl4j.network.NeuralNet> mockAsyncGlobal;
		Friend mockAsyncGlobal As AsyncGlobal(Of NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IAsyncLearningConfiguration mockConfiguration;
		Friend mockConfiguration As IAsyncLearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			asyncLearning = mock(GetType(AsyncLearning), Mockito.withSettings().useConstructor().defaultAnswer(Mockito.CALLS_REAL_METHODS))

			asyncLearning.addListener(mockTrainingListener)

			[when](asyncLearning.getAsyncGlobal()).thenReturn(mockAsyncGlobal)
			[when](asyncLearning.Configuration).thenReturn(mockConfiguration)

			' Don't actually start any threads in any of these tests
			[when](mockConfiguration.NumThreads).thenReturn(0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_trainStartReturnsStop_expect_noTraining()
		Public Overridable Sub when_trainStartReturnsStop_expect_noTraining()
			' Arrange
			[when](mockTrainingListener.onTrainingStart()).thenReturn(TrainingListener.ListenerResponse.STOP)

			' Act
			asyncLearning.train()

			' Assert
			verify(mockTrainingListener, times(1)).onTrainingStart()
			verify(mockTrainingListener, times(1)).onTrainingEnd()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_trainingIsComplete_expect_trainingStop()
		Public Overridable Sub when_trainingIsComplete_expect_trainingStop()
			' Arrange
			[when](mockAsyncGlobal.TrainingComplete).thenReturn(True)

			' Act
			asyncLearning.train()

			' Assert
			verify(mockTrainingListener, times(1)).onTrainingStart()
			verify(mockTrainingListener, times(1)).onTrainingEnd()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_training_expect_onTrainingProgressCalled()
		Public Overridable Sub when_training_expect_onTrainingProgressCalled()
			' Arrange
			asyncLearning.setProgressMonitorFrequency(100)
			[when](mockTrainingListener.onTrainingProgress(eq(asyncLearning))).thenReturn(TrainingListener.ListenerResponse.STOP)

			' Act
			asyncLearning.train()

			' Assert
			verify(mockTrainingListener, times(1)).onTrainingStart()
			verify(mockTrainingListener, times(1)).onTrainingEnd()
			verify(mockTrainingListener, times(1)).onTrainingProgress(eq(asyncLearning))
		End Sub
	End Class

End Namespace