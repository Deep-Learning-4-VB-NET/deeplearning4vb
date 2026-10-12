Imports System.Collections.Generic
Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports org.deeplearning4j.rl4j.learning
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager

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


	Public Class TrainingListenerList
		Protected Friend ReadOnly listeners As IList(Of TrainingListener) = New List(Of TrainingListener)()

		''' <summary>
		''' Add a listener at the end of the list </summary>
		''' <param name="listener"> The listener to be added </param>
		Public Overridable Sub add(ByVal listener As TrainingListener)
			listeners.Add(listener)
		End Sub

		''' <summary>
		''' Notify the listeners that the training has started. Will stop early if a listener returns <seealso cref="org.deeplearning4j.rl4j.learning.listener.TrainingListener.ListenerResponse.STOP"/> </summary>
		''' <returns> whether or not the source training should be stopped </returns>
		Public Overridable Function notifyTrainingStarted() As Boolean
			For Each listener As TrainingListener In listeners
				If listener.onTrainingStart() = TrainingListener.ListenerResponse.STOP Then
					Return False
				End If
			Next listener

			Return True
		End Function

		''' <summary>
		''' Notify the listeners that the training has finished.
		''' </summary>
		Public Overridable Sub notifyTrainingFinished()
			For Each listener As TrainingListener In listeners
				listener.onTrainingEnd()
			Next listener
		End Sub

		''' <summary>
		''' Notify the listeners that a new epoch has started. Will stop early if a listener returns <seealso cref="org.deeplearning4j.rl4j.learning.listener.TrainingListener.ListenerResponse.STOP"/> </summary>
		''' <returns> whether or not the source training should be stopped </returns>
		Public Overridable Function notifyNewEpoch(ByVal trainer As IEpochTrainer) As Boolean
			For Each listener As TrainingListener In listeners
				If listener.onNewEpoch(trainer) = TrainingListener.ListenerResponse.STOP Then
					Return False
				End If
			Next listener

			Return True
		End Function

		''' <summary>
		''' Notify the listeners that an epoch has been completed and the training results are available. Will stop early if a listener returns <seealso cref="org.deeplearning4j.rl4j.learning.listener.TrainingListener.ListenerResponse.STOP"/> </summary>
		''' <returns> whether or not the source training should be stopped </returns>
		Public Overridable Function notifyEpochTrainingResult(ByVal trainer As IEpochTrainer, ByVal statEntry As IDataManager.StatEntry) As Boolean
			For Each listener As TrainingListener In listeners
				If listener.onEpochTrainingResult(trainer, statEntry) = TrainingListener.ListenerResponse.STOP Then
					Return False
				End If
			Next listener

			Return True
		End Function

		''' <summary>
		''' Notify the listeners that they update the progress ot the trainning.
		''' </summary>
		Public Overridable Function notifyTrainingProgress(ByVal learning As ILearning) As Boolean
			For Each listener As TrainingListener In listeners
				If listener.onTrainingProgress(learning) = TrainingListener.ListenerResponse.STOP Then
					Return False
				End If
			Next listener

			Return True
		End Function
	End Class

End Namespace