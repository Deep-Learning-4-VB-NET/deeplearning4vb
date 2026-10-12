Imports System
Imports System.Threading
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.rl4j.learning
Imports AsyncQLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.AsyncQLearningConfiguration
Imports IAsyncLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.IAsyncLearningConfiguration
Imports TrainingListener = org.deeplearning4j.rl4j.learning.listener.TrainingListener
Imports TrainingListenerList = org.deeplearning4j.rl4j.learning.listener.TrainingListenerList
Imports org.deeplearning4j.rl4j.network
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
'ORIGINAL LINE: @Slf4j public abstract class AsyncLearning<OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable, ACTION, ACTION_SPACE extends org.deeplearning4j.rl4j.space.ActionSpace<ACTION>, NN extends org.deeplearning4j.rl4j.network.NeuralNet> extends org.deeplearning4j.rl4j.learning.Learning<OBSERVATION, ACTION, ACTION_SPACE, NN> implements IAsyncLearning
	Public MustInherit Class AsyncLearning(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, ACTION, ACTION_SPACE As org.deeplearning4j.rl4j.space.ActionSpace(Of ACTION), NN As org.deeplearning4j.rl4j.network.NeuralNet)
		Inherits Learning(Of OBSERVATION, ACTION, ACTION_SPACE, NN)
		Implements IAsyncLearning

		Public Overrides MustOverride ReadOnly Property HistoryProcessor As IHistoryProcessor
		Public Overrides MustOverride ReadOnly Property Mdp As org.deeplearning4j.rl4j.mdp.MDP(Of OBSERVATION, A, [AS])
		Public Overrides MustOverride ReadOnly Property Policy As org.deeplearning4j.rl4j.policy.IPolicy(Of A)

		Private monitorThread As Thread = Nothing

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private final org.deeplearning4j.rl4j.learning.listener.TrainingListenerList listeners = new org.deeplearning4j.rl4j.learning.listener.TrainingListenerList();
		Private ReadOnly listeners As New TrainingListenerList()

		''' <summary>
		''' Add a <seealso cref="TrainingListener"/> listener at the end of the listener list.
		''' </summary>
		''' <param name="listener"> the listener to be added </param>
		Public Overridable Sub addListener(ByVal listener As TrainingListener)
			listeners.add(listener)
		End Sub

		''' <summary>
		''' Returns the configuration
		''' </summary>
		''' <returns> the configuration (see <seealso cref="AsyncQLearningConfiguration"/>) </returns>
		Public Overrides MustOverride ReadOnly Property Configuration As IAsyncLearningConfiguration

		Protected Friend MustOverride Function newThread(ByVal i As Integer, ByVal deviceAffinity As Integer) As AsyncThread

		Protected Friend MustOverride ReadOnly Property AsyncGlobal As IAsyncGlobal(Of NN)

		Protected Friend Overridable ReadOnly Property TrainingComplete As Boolean
			Get
				Return getAsyncGlobal().TrainingComplete
			End Get
		End Property

		Private canContinue As Boolean = True

		''' <summary>
		''' Number of milliseconds between calls to onTrainingProgress
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int progressMonitorFrequency = 20000;
		Private progressMonitorFrequency As Integer = 20000

		Private Sub launchThreads()
			Dim i As Integer = 0
			Do While i < Configuration.NumThreads
				Dim t As Thread = newThread(i, i Mod Nd4j.AffinityManager.NumberOfDevices)
				t.Start()
				i += 1
			Loop
			log.info("Threads launched.")
		End Sub

		''' <returns> The current step </returns>
		Public Overrides ReadOnly Property StepCount As Integer
			Get
				Return getAsyncGlobal().StepCount
			End Get
		End Property

		''' <summary>
		''' This method will train the model<para>
		''' The training stop when:<br>
		''' - A worker thread terminate the AsyncGlobal thread (see <seealso cref="AsyncGlobal"/>)<br>
		''' OR<br>
		''' - a listener explicitly stops it<br>
		''' </para>
		''' <para>
		''' Listeners<br>
		''' For a given event, the listeners are called sequentially in same the order as they were added. If one listener
		''' returns <seealso cref="org.deeplearning4j.rl4j.learning.listener.TrainingListener.ListenerResponse TrainingListener.ListenerResponse.STOP"/>, the remaining listeners in the list won't be called.<br>
		''' Events:
		''' <ul>
		'''   <li><seealso cref="TrainingListener.onTrainingStart() onTrainingStart()"/> is called once when the training starts.</li>
		'''   <li><seealso cref="TrainingListener.onTrainingEnd() onTrainingEnd()"/>  is always called at the end of the training, even if the training was cancelled by a listener.</li>
		''' </ul>
		''' </para>
		''' </summary>
		Public Overrides Sub train()

			log.info("AsyncLearning training starting.")

			canContinue = listeners.notifyTrainingStarted()
			If canContinue Then
				launchThreads()
				monitorTraining()
			End If

			listeners.notifyTrainingFinished()
		End Sub

		Protected Friend Overridable Sub monitorTraining()
			Try
				monitorThread = Thread.CurrentThread
				Do While canContinue AndAlso Not TrainingComplete
					canContinue = listeners.notifyTrainingProgress(Me)
					If Not canContinue Then
						Return
					End If

					SyncLock Me
						Monitor.Wait(Me, TimeSpan.FromMilliseconds(progressMonitorFrequency))
					End SyncLock
				Loop
			Catch e As InterruptedException
				log.error("Training interrupted.", e)
			End Try
			monitorThread = Nothing
		End Sub

		''' <summary>
		''' Force the immediate termination of the learning. All learning threads, the AsyncGlobal thread and the monitor thread will be terminated.
		''' </summary>
		Public Overridable Sub terminate() Implements IAsyncLearning.terminate
			If canContinue Then
				canContinue = False

				Dim safeMonitorThread As Thread = monitorThread
				If safeMonitorThread IsNot Nothing Then
					safeMonitorThread.Interrupt()
				End If
			End If
		End Sub
	End Class

End Namespace