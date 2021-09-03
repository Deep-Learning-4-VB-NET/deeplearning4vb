Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.learning.async
Imports TrainingListener = org.deeplearning4j.rl4j.learning.listener.TrainingListener

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
'ORIGINAL LINE: @Slf4j public class DataManagerTrainingListener implements org.deeplearning4j.rl4j.learning.listener.TrainingListener
	Public Class DataManagerTrainingListener
		Implements TrainingListener

'JAVA TO VB CONVERTER NOTE: The variable dataManager was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private ReadOnly dataManager_Conflict As IDataManager

		Private lastSave As Integer = -Constants.MODEL_SAVE_FREQ

'JAVA TO VB CONVERTER NOTE: The parameter dataManager was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Sub New(ByVal dataManager_Conflict As IDataManager)
			Me.dataManager_Conflict = dataManager_Conflict
		End Sub

		Public Overridable Function onTrainingStart() As ListenerResponse Implements TrainingListener.onTrainingStart
			Return ListenerResponse.CONTINUE
		End Function

		Public Overridable Sub onTrainingEnd() Implements TrainingListener.onTrainingEnd

		End Sub

		Public Overridable Function onNewEpoch(ByVal trainer As IEpochTrainer) As ListenerResponse Implements TrainingListener.onNewEpoch
			Dim hp As IHistoryProcessor = trainer.HistoryProcessor
			If hp IsNot Nothing Then
				Dim shape() As Integer = trainer.Mdp.getObservationSpace().Shape
				Dim filename As String = dataManager_Conflict.VideoDir & "/video-"
				If TypeOf trainer Is AsyncThread Then
					filename &= DirectCast(trainer, AsyncThread).getThreadNumber() & "-"
				End If
				filename &= trainer.EpochCount & "-" & trainer.StepCount & ".mp4"
				hp.startMonitor(filename, shape)
			End If

			Return ListenerResponse.CONTINUE
		End Function

		Public Overridable Function onEpochTrainingResult(ByVal trainer As IEpochTrainer, ByVal statEntry As IDataManager.StatEntry) As ListenerResponse
			Dim hp As IHistoryProcessor = trainer.HistoryProcessor
			If hp IsNot Nothing Then
				hp.stopMonitor()
			End If
			Try
				dataManager_Conflict.appendStat(statEntry)
			Catch e As Exception
				log.error("Training failed.", e)
				Return ListenerResponse.STOP
			End Try

			Return ListenerResponse.CONTINUE
		End Function

		Public Overridable Function onTrainingProgress(ByVal learning As ILearning) As ListenerResponse Implements TrainingListener.onTrainingProgress
			Try
				Dim stepCounter As Integer = learning.getStepCount()
				If stepCounter - lastSave >= Constants.MODEL_SAVE_FREQ Then
					dataManager_Conflict.save(learning)
					lastSave = stepCounter
				End If

				dataManager_Conflict.writeInfo(learning)
			Catch e As Exception
				log.error("Training failed.", e)
				Return ListenerResponse.STOP
			End Try

			Return ListenerResponse.CONTINUE
		End Function
	End Class

End Namespace