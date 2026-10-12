Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports org.deeplearning4j.rl4j.learning
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports Mock = org.mockito.Mock
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports org.junit.jupiter.api.Assertions
import static org.mockito.ArgumentMatchers.eq
import static org.mockito.Mockito.mock
import static org.mockito.Mockito.never
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

Namespace org.deeplearning4j.rl4j.learning.listener

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class TrainingListenerListTest
	Public Class TrainingListenerListTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IEpochTrainer mockTrainer;
		Friend mockTrainer As IEpochTrainer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ILearning mockLearning;
		Friend mockLearning As ILearning

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IDataManager.StatEntry mockStatEntry;
		Friend mockStatEntry As IDataManager.StatEntry

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_listIsEmpty_expect_notifyReturnTrue()
		Public Overridable Sub when_listIsEmpty_expect_notifyReturnTrue()
			' Arrange
			Dim trainingListenerList As New TrainingListenerList()

			' Act
			Dim resultTrainingStarted As Boolean = trainingListenerList.notifyTrainingStarted()
			Dim resultNewEpoch As Boolean = trainingListenerList.notifyNewEpoch(Nothing)
			Dim resultEpochFinished As Boolean = trainingListenerList.notifyEpochTrainingResult(Nothing, Nothing)

			' Assert
			assertTrue(resultTrainingStarted)
			assertTrue(resultNewEpoch)
			assertTrue(resultEpochFinished)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_firstListerStops_expect_othersListnersNotCalled()
		Public Overridable Sub when_firstListerStops_expect_othersListnersNotCalled()
			' Arrange
			Dim listener1 As TrainingListener = mock(GetType(TrainingListener))
			Dim listener2 As TrainingListener = mock(GetType(TrainingListener))
			Dim trainingListenerList As New TrainingListenerList()
			trainingListenerList.add(listener1)
			trainingListenerList.add(listener2)

			[when](listener1.onTrainingStart()).thenReturn(TrainingListener.ListenerResponse.STOP)
			[when](listener1.onNewEpoch(eq(mockTrainer))).thenReturn(TrainingListener.ListenerResponse.STOP)
			[when](listener1.onEpochTrainingResult(eq(mockTrainer), eq(mockStatEntry))).thenReturn(TrainingListener.ListenerResponse.STOP)
			[when](listener1.onTrainingProgress(eq(mockLearning))).thenReturn(TrainingListener.ListenerResponse.STOP)

			' Act
			trainingListenerList.notifyTrainingStarted()
			trainingListenerList.notifyNewEpoch(mockTrainer)
			trainingListenerList.notifyEpochTrainingResult(mockTrainer, Nothing)
			trainingListenerList.notifyTrainingProgress(mockLearning)
			trainingListenerList.notifyTrainingFinished()

			' Assert

			verify(listener1, times(1)).onTrainingStart()
			verify(listener2, never()).onTrainingStart()

			verify(listener1, times(1)).onNewEpoch(eq(mockTrainer))
			verify(listener2, never()).onNewEpoch(eq(mockTrainer))

			verify(listener1, times(1)).onEpochTrainingResult(eq(mockTrainer), eq(mockStatEntry))
			verify(listener2, never()).onEpochTrainingResult(eq(mockTrainer), eq(mockStatEntry))

			verify(listener1, times(1)).onTrainingProgress(eq(mockLearning))
			verify(listener2, never()).onTrainingProgress(eq(mockLearning))

			verify(listener1, times(1)).onTrainingEnd()
			verify(listener2, times(1)).onTrainingEnd()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_allListenersContinue_expect_listReturnsTrue()
		Public Overridable Sub when_allListenersContinue_expect_listReturnsTrue()
			' Arrange
			Dim listener1 As TrainingListener = mock(GetType(TrainingListener))
			Dim listener2 As TrainingListener = mock(GetType(TrainingListener))
			Dim trainingListenerList As New TrainingListenerList()
			trainingListenerList.add(listener1)
			trainingListenerList.add(listener2)

			' Act
			Dim resultTrainingStarted As Boolean = trainingListenerList.notifyTrainingStarted()
			Dim resultNewEpoch As Boolean = trainingListenerList.notifyNewEpoch(Nothing)
			Dim resultEpochTrainingResult As Boolean = trainingListenerList.notifyEpochTrainingResult(Nothing, Nothing)
			Dim resultProgress As Boolean = trainingListenerList.notifyTrainingProgress(Nothing)

			' Assert
			assertTrue(resultTrainingStarted)
			assertTrue(resultNewEpoch)
			assertTrue(resultEpochTrainingResult)
			assertTrue(resultProgress)
		End Sub
	End Class

End Namespace