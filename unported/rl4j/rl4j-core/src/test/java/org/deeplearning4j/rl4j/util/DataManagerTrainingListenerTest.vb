Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.learning
Imports ILearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.ILearningConfiguration
Imports TrainingListener = org.deeplearning4j.rl4j.learning.listener.TrainingListener
Imports MockStatEntry = org.deeplearning4j.rl4j.learning.sync.support.MockStatEntry
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.policy
Imports MockDataManager = org.deeplearning4j.rl4j.support.MockDataManager
Imports MockHistoryProcessor = org.deeplearning4j.rl4j.support.MockHistoryProcessor
Imports MockMDP = org.deeplearning4j.rl4j.support.MockMDP
Imports MockObservationSpace = org.deeplearning4j.rl4j.support.MockObservationSpace
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertSame

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

Namespace org.deeplearning4j.rl4j.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class DataManagerTrainingListenerTest
	Public Class DataManagerTrainingListenerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOnNewEpochWithoutHistoryProcessor_expect_noException()
		Public Overridable Sub when_callingOnNewEpochWithoutHistoryProcessor_expect_noException()
			' Arrange
			Dim trainer As New TestTrainer()
			Dim sut As New DataManagerTrainingListener(New MockDataManager(False))

			' Act
			Dim response As TrainingListener.ListenerResponse = sut.onNewEpoch(trainer)

			' Assert
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOnNewEpochWithHistoryProcessor_expect_startMonitorNotCalled()
		Public Overridable Sub when_callingOnNewEpochWithHistoryProcessor_expect_startMonitorNotCalled()
			' Arrange
			Dim trainer As New TestTrainer()
			Dim hpConf As New IHistoryProcessor.Configuration(5, 4, 4, 4, 4, 0, 0, 2)
			Dim hp As New MockHistoryProcessor(hpConf)
			trainer.setHistoryProcessor(hp)
			Dim sut As New DataManagerTrainingListener(New MockDataManager(False))

			' Act
			Dim response As TrainingListener.ListenerResponse = sut.onNewEpoch(trainer)

			' Assert
			assertEquals(1, hp.startMonitorCallCount)
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOnEpochTrainingResultWithoutHistoryProcessor_expect_noException()
		Public Overridable Sub when_callingOnEpochTrainingResultWithoutHistoryProcessor_expect_noException()
			' Arrange
			Dim trainer As New TestTrainer()
			Dim sut As New DataManagerTrainingListener(New MockDataManager(False))

			' Act
			Dim response As TrainingListener.ListenerResponse = sut.onEpochTrainingResult(trainer, Nothing)

			' Assert
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOnNewEpochWithHistoryProcessor_expect_stopMonitorNotCalled()
		Public Overridable Sub when_callingOnNewEpochWithHistoryProcessor_expect_stopMonitorNotCalled()
			' Arrange
			Dim trainer As New TestTrainer()
			Dim hpConf As New IHistoryProcessor.Configuration(5, 4, 4, 4, 4, 0, 0, 2)
			Dim hp As New MockHistoryProcessor(hpConf)
			trainer.setHistoryProcessor(hp)
			Dim sut As New DataManagerTrainingListener(New MockDataManager(False))

			' Act
			Dim response As TrainingListener.ListenerResponse = sut.onEpochTrainingResult(trainer, Nothing)

			' Assert
			assertEquals(1, hp.stopMonitorCallCount)
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOnEpochTrainingResult_expect_callToDataManagerAppendStat()
		Public Overridable Sub when_callingOnEpochTrainingResult_expect_callToDataManagerAppendStat()
			' Arrange
			Dim trainer As New TestTrainer()
			Dim dm As New MockDataManager(False)
			Dim sut As New DataManagerTrainingListener(dm)
			Dim statEntry As New MockStatEntry(0, 0, 0.0)

			' Act
			Dim response As TrainingListener.ListenerResponse = sut.onEpochTrainingResult(trainer, statEntry)

			' Assert
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response)
			assertEquals(1, dm.statEntries.Count)
			assertSame(statEntry, dm.statEntries(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOnTrainingProgress_expect_callToDataManagerSaveAndWriteInfo()
		Public Overridable Sub when_callingOnTrainingProgress_expect_callToDataManagerSaveAndWriteInfo()
			' Arrange
			Dim learning As New TestTrainer()
			Dim dm As New MockDataManager(False)
			Dim sut As New DataManagerTrainingListener(dm)

			' Act
			Dim response As TrainingListener.ListenerResponse = sut.onTrainingProgress(learning)

			' Assert
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response)
			assertEquals(1, dm.writeInfoCallCount)
			assertEquals(1, dm.saveCallCount)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_stepCounterCloseToLastSave_expect_dataManagerSaveNotCalled()
		Public Overridable Sub when_stepCounterCloseToLastSave_expect_dataManagerSaveNotCalled()
			' Arrange
			Dim learning As New TestTrainer()
			Dim dm As New MockDataManager(False)
			Dim sut As New DataManagerTrainingListener(dm)

			' Act
			Dim response As TrainingListener.ListenerResponse = sut.onTrainingProgress(learning)
			Dim response2 As TrainingListener.ListenerResponse = sut.onTrainingProgress(learning)

			' Assert
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response)
			assertEquals(TrainingListener.ListenerResponse.CONTINUE, response2)
			assertEquals(1, dm.saveCallCount)
		End Sub

		Private Class TestTrainer
			Implements IEpochTrainer, ILearning

			Public Overridable ReadOnly Property StepCount As Integer Implements IEpochTrainer.getStepCount
				Get
					Return 0
				End Get
			End Property

			Public Overridable ReadOnly Property EpochCount As Integer Implements IEpochTrainer.getEpochCount
				Get
					Return 0
				End Get
			End Property

			Public Overridable ReadOnly Property EpisodeCount As Integer Implements IEpochTrainer.getEpisodeCount
				Get
					Return 0
				End Get
			End Property

			Public Overridable ReadOnly Property CurrentEpisodeStepCount As Integer Implements IEpochTrainer.getCurrentEpisodeStepCount
				Get
					Return 0
				End Get
			End Property


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.deeplearning4j.rl4j.learning.IHistoryProcessor historyProcessor;
			Friend historyProcessor As IHistoryProcessor

			Public Overridable ReadOnly Property Policy As IPolicy
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Sub train()

			End Sub

			Public Overridable ReadOnly Property Configuration As ILearningConfiguration
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable ReadOnly Property Mdp As MDP Implements IEpochTrainer.getMdp
				Get
					Return New MockMDP(New MockObservationSpace())
				End Get
			End Property
		End Class
	End Class

End Namespace