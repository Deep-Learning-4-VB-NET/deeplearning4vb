Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.learning.listener
Imports org.deeplearning4j.rl4j.network
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
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

Namespace org.deeplearning4j.rl4j.learning.sync

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class SyncLearning<OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable, ACTION, ACTION_SPACE extends org.deeplearning4j.rl4j.space.ActionSpace<ACTION>, NN extends org.deeplearning4j.rl4j.network.NeuralNet> extends org.deeplearning4j.rl4j.learning.Learning<OBSERVATION, ACTION, ACTION_SPACE, NN> implements org.deeplearning4j.rl4j.learning.IEpochTrainer
	Public MustInherit Class SyncLearning(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, ACTION, ACTION_SPACE As org.deeplearning4j.rl4j.space.ActionSpace(Of ACTION), NN As org.deeplearning4j.rl4j.network.NeuralNet)
		Inherits Learning(Of OBSERVATION, ACTION, ACTION_SPACE, NN)
		Implements IEpochTrainer

		Public MustOverride ReadOnly Property CurrentEpisodeStepCount As Integer Implements IEpochTrainer.getCurrentEpisodeStepCount
		Public MustOverride ReadOnly Property EpisodeCount As Integer Implements IEpochTrainer.getEpisodeCount
		Public MustOverride ReadOnly Property EpochCount As Integer Implements IEpochTrainer.getEpochCount
		Public Overrides MustOverride ReadOnly Property HistoryProcessor As IHistoryProcessor Implements IEpochTrainer.getHistoryProcessor
		Public Overrides MustOverride ReadOnly Property Mdp As org.deeplearning4j.rl4j.mdp.MDP(Of OBSERVATION, A, [AS])
		Public Overrides MustOverride ReadOnly Property Configuration As org.deeplearning4j.rl4j.learning.configuration.ILearningConfiguration
		Public Overrides MustOverride ReadOnly Property StepCount As Integer Implements IEpochTrainer.getStepCount
		Public Overrides MustOverride ReadOnly Property Policy As org.deeplearning4j.rl4j.policy.IPolicy(Of A)

		Private ReadOnly listeners As New TrainingListenerList()

		''' <summary>
		''' Add a listener at the end of the listener list.
		''' </summary>
		''' <param name="listener"> The listener to add </param>
		Public Overridable Sub addListener(ByVal listener As TrainingListener)
			listeners.add(listener)
		End Sub

		''' <summary>
		''' Number of epochs between calls to onTrainingProgress. Default is 5
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int progressMonitorFrequency = 5;
'JAVA TO VB CONVERTER NOTE: The field progressMonitorFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private progressMonitorFrequency_Conflict As Integer = 5

		Public Overridable WriteOnly Property ProgressMonitorFrequency As Integer
			Set(ByVal value As Integer)
				If value = 0 Then
					Throw New System.ArgumentException("The progressMonitorFrequency cannot be 0")
				End If
    
				progressMonitorFrequency_Conflict = value
			End Set
		End Property

		''' <summary>
		''' This method will train the model<para>
		''' The training stop when:<br>
		''' - the number of steps reaches the maximum defined in the configuration (see <seealso cref="ILearningConfiguration.getMaxStep() LConfiguration.getMaxStep()"/>)<br>
		''' OR<br>
		''' - a listener explicitly stops it<br>
		''' </para>
		''' <para>
		''' Listeners<br>
		''' For a given event, the listeners are called sequentially in same the order as they were added. If one listener
		''' returns <seealso cref="TrainingListener.ListenerResponse SyncTrainingListener.ListenerResponse.STOP"/>, the remaining listeners in the list won't be called.<br>
		''' Events:
		''' <ul>
		'''   <li><seealso cref="TrainingListener.onTrainingStart() onTrainingStart()"/> is called once when the training starts.</li>
		'''   <li><seealso cref="TrainingListener.onNewEpoch(IEpochTrainer) onNewEpoch()"/> and <seealso cref="TrainingListener.onEpochTrainingResult(IEpochTrainer, IDataManager.StatEntry) onEpochTrainingResult()"/>  are called for every epoch. onEpochTrainingResult will not be called if onNewEpoch stops the training</li>
		'''   <li><seealso cref="TrainingListener.onTrainingProgress(ILearning) onTrainingProgress()"/> is called after onEpochTrainingResult()</li>
		'''   <li><seealso cref="TrainingListener.onTrainingEnd() onTrainingEnd()"/> is always called at the end of the training, even if the training was cancelled by a listener.</li>
		''' </ul>
		''' </para>
		''' </summary>
		Public Overrides Sub train()

			log.info("training starting.")

			Dim canContinue As Boolean = listeners.notifyTrainingStarted()
			If canContinue Then
				Do While Me.StepCount < Configuration.MaxStep
					preEpoch()
					canContinue = listeners.notifyNewEpoch(Me)
					If Not canContinue Then
						Exit Do
					End If

					Dim statEntry As IDataManager.StatEntry = trainEpoch()
					canContinue = listeners.notifyEpochTrainingResult(Me, statEntry)
					If Not canContinue Then
						Exit Do
					End If

					postEpoch()

					If EpochCount Mod progressMonitorFrequency_Conflict = 0 Then
						canContinue = listeners.notifyTrainingProgress(Me)
						If Not canContinue Then
							Exit Do
						End If
					End If

					log.info("Epoch: " & EpochCount & ", reward: " & statEntry.Reward)
					incrementEpoch()
				Loop
			End If

			listeners.notifyTrainingFinished()
		End Sub

		Protected Friend MustOverride Sub preEpoch()

		Protected Friend MustOverride Sub postEpoch()

		Protected Friend MustOverride Function trainEpoch() As IDataManager.StatEntry ' TODO: finish removal of IDataManager from Learning
	End Class

End Namespace