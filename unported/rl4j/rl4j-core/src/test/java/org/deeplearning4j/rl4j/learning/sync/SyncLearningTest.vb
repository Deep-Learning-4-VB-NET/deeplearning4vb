Imports ILearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.ILearningConfiguration
Imports TrainingListener = org.deeplearning4j.rl4j.learning.listener.TrainingListener
Imports MockStatEntry = org.deeplearning4j.rl4j.learning.sync.support.MockStatEntry
Imports org.deeplearning4j.rl4j.network
Imports org.deeplearning4j.rl4j.space
Imports Box = org.deeplearning4j.rl4j.space.Box
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports Mockito = org.mockito.Mockito
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.mockito.ArgumentMatchers.any
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

Namespace org.deeplearning4j.rl4j.learning.sync

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) public class SyncLearningTest
	Public Class SyncLearningTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock TrainingListener mockTrainingListener;
		Friend mockTrainingListener As TrainingListener

		Friend syncLearning As SyncLearning(Of Box, INDArray, ActionSpace(Of INDArray), NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ILearningConfiguration mockLearningConfiguration;
		Friend mockLearningConfiguration As ILearningConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()

			syncLearning = mock(GetType(SyncLearning), Mockito.withSettings().useConstructor().defaultAnswer(Mockito.CALLS_REAL_METHODS))

			syncLearning.addListener(mockTrainingListener)

			[when](syncLearning.trainEpoch()).thenAnswer(Function(invocation)
			syncLearning.incrementStep()
			Return New MockStatEntry(syncLearning.getEpochCount(), syncLearning.StepCount, 1.0)
			End Function)

			[when](syncLearning.Configuration).thenReturn(mockLearningConfiguration)
			[when](mockLearningConfiguration.MaxStep).thenReturn(100)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_training_expect_listenersToBeCalled()
		Public Overridable Sub when_training_expect_listenersToBeCalled()

			' Act
			syncLearning.train()

			verify(mockTrainingListener, times(1)).onTrainingStart()
			verify(mockTrainingListener, times(100)).onNewEpoch(eq(syncLearning))
			verify(mockTrainingListener, times(100)).onEpochTrainingResult(eq(syncLearning), any(GetType(IDataManager.StatEntry)))
			verify(mockTrainingListener, times(1)).onTrainingEnd()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_trainingStartCanContinueFalse_expect_trainingStopped()
		Public Overridable Sub when_trainingStartCanContinueFalse_expect_trainingStopped()
			' Arrange
			[when](mockTrainingListener.onTrainingStart()).thenReturn(TrainingListener.ListenerResponse.STOP)

			' Act
			syncLearning.train()

			verify(mockTrainingListener, times(1)).onTrainingStart()
			verify(mockTrainingListener, times(0)).onNewEpoch(eq(syncLearning))
			verify(mockTrainingListener, times(0)).onEpochTrainingResult(eq(syncLearning), any(GetType(IDataManager.StatEntry)))
			verify(mockTrainingListener, times(1)).onTrainingEnd()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_newEpochCanContinueFalse_expect_trainingStopped()
		Public Overridable Sub when_newEpochCanContinueFalse_expect_trainingStopped()
			' Arrange
			[when](mockTrainingListener.onNewEpoch(eq(syncLearning))).thenReturn(TrainingListener.ListenerResponse.CONTINUE).thenReturn(TrainingListener.ListenerResponse.CONTINUE).thenReturn(TrainingListener.ListenerResponse.STOP)

			' Act
			syncLearning.train()

			verify(mockTrainingListener, times(1)).onTrainingStart()
			verify(mockTrainingListener, times(3)).onNewEpoch(eq(syncLearning))
			verify(mockTrainingListener, times(2)).onEpochTrainingResult(eq(syncLearning), any(GetType(IDataManager.StatEntry)))
			verify(mockTrainingListener, times(1)).onTrainingEnd()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_epochTrainingResultCanContinueFalse_expect_trainingStopped()
		Public Overridable Sub when_epochTrainingResultCanContinueFalse_expect_trainingStopped()
			' Arrange
			[when](mockTrainingListener.onEpochTrainingResult(eq(syncLearning), any(GetType(IDataManager.StatEntry)))).thenReturn(TrainingListener.ListenerResponse.CONTINUE).thenReturn(TrainingListener.ListenerResponse.CONTINUE).thenReturn(TrainingListener.ListenerResponse.STOP)

			' Act
			syncLearning.train()

			verify(mockTrainingListener, times(1)).onTrainingStart()
			verify(mockTrainingListener, times(3)).onNewEpoch(eq(syncLearning))
			verify(mockTrainingListener, times(3)).onEpochTrainingResult(eq(syncLearning), any(GetType(IDataManager.StatEntry)))
			verify(mockTrainingListener, times(1)).onTrainingEnd()
		End Sub
	End Class

End Namespace