Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.learning.listener
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager
Imports Test = org.junit.jupiter.api.Test
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.rl4j.learning.async.listener

	Public Class AsyncTrainingListenerListTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_listIsEmpty_expect_notifyTrainingStartedReturnTrue()
		Public Overridable Sub when_listIsEmpty_expect_notifyTrainingStartedReturnTrue()
			' Arrange
			Dim sut As New TrainingListenerList()

			' Act
			Dim resultTrainingStarted As Boolean = sut.notifyTrainingStarted()
			Dim resultNewEpoch As Boolean = sut.notifyNewEpoch(Nothing)
			Dim resultEpochTrainingResult As Boolean = sut.notifyEpochTrainingResult(Nothing, Nothing)

			' Assert
			assertTrue(resultTrainingStarted)
			assertTrue(resultNewEpoch)
			assertTrue(resultEpochTrainingResult)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_firstListerStops_expect_othersListnersNotCalled()
		Public Overridable Sub when_firstListerStops_expect_othersListnersNotCalled()
			' Arrange
			Dim listener1 As New MockTrainingListener()
			listener1.onTrainingResultResponse = TrainingListener.ListenerResponse.STOP
			Dim listener2 As New MockTrainingListener()
			Dim sut As New TrainingListenerList()
			sut.add(listener1)
			sut.add(listener2)

			' Act
			sut.notifyEpochTrainingResult(Nothing, Nothing)

			' Assert
			assertEquals(1, listener1.onEpochTrainingResultCallCount)
			assertEquals(0, listener2.onEpochTrainingResultCallCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_allListenersContinue_expect_listReturnsTrue()
		Public Overridable Sub when_allListenersContinue_expect_listReturnsTrue()
			' Arrange
			Dim listener1 As New MockTrainingListener()
			Dim listener2 As New MockTrainingListener()
			Dim sut As New TrainingListenerList()
			sut.add(listener1)
			sut.add(listener2)

			' Act
			Dim resultTrainingProgress As Boolean = sut.notifyEpochTrainingResult(Nothing, Nothing)

			' Assert
			assertTrue(resultTrainingProgress)
		End Sub

		Private Class MockTrainingListener
			Implements TrainingListener

			Public onEpochTrainingResultCallCount As Integer = 0
			Public onTrainingResultResponse As ListenerResponse = ListenerResponse.CONTINUE
			Public onTrainingProgressCallCount As Integer = 0
			Public onTrainingProgressResponse As ListenerResponse = ListenerResponse.CONTINUE

			Public Overridable Function onTrainingStart() As ListenerResponse Implements TrainingListener.onTrainingStart
				Return ListenerResponse.CONTINUE
			End Function

			Public Overridable Sub onTrainingEnd() Implements TrainingListener.onTrainingEnd

			End Sub

			Public Overridable Function onNewEpoch(ByVal trainer As IEpochTrainer) As ListenerResponse Implements TrainingListener.onNewEpoch
				Return ListenerResponse.CONTINUE
			End Function

			Public Overridable Function onEpochTrainingResult(ByVal trainer As IEpochTrainer, ByVal statEntry As IDataManager.StatEntry) As ListenerResponse Implements TrainingListener.onEpochTrainingResult
				onEpochTrainingResultCallCount += 1
				Return onTrainingResultResponse
			End Function

			Public Overridable Function onTrainingProgress(ByVal learning As ILearning) As ListenerResponse Implements TrainingListener.onTrainingProgress
				onTrainingProgressCallCount += 1
				Return onTrainingProgressResponse
			End Function
		End Class

	End Class

End Namespace