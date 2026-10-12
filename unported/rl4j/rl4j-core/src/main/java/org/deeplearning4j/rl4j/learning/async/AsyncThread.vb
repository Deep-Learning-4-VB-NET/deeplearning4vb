Imports System
Imports System.Threading
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Value = lombok.Value
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.gym
Imports HistoryProcessor = org.deeplearning4j.rl4j.learning.HistoryProcessor
Imports IEpochTrainer = org.deeplearning4j.rl4j.learning.IEpochTrainer
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.learning
Imports IAsyncLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.IAsyncLearningConfiguration
Imports TrainingListener = org.deeplearning4j.rl4j.learning.listener.TrainingListener
Imports TrainingListenerList = org.deeplearning4j.rl4j.learning.listener.TrainingListenerList
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports IDataManager = org.deeplearning4j.rl4j.util.IDataManager
Imports org.deeplearning4j.rl4j.util
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
'ORIGINAL LINE: @Slf4j public abstract class AsyncThread<OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable, ACTION, ACTION_SPACE extends org.deeplearning4j.rl4j.space.ActionSpace<ACTION>, NN extends org.deeplearning4j.rl4j.network.NeuralNet> extends Thread implements org.deeplearning4j.rl4j.learning.IEpochTrainer
	Public MustInherit Class AsyncThread(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, ACTION, ACTION_SPACE As org.deeplearning4j.rl4j.space.ActionSpace(Of ACTION), NN As org.deeplearning4j.rl4j.network.NeuralNet)
		Inherits Thread
		Implements IEpochTrainer

		Public MustOverride ReadOnly Property HistoryProcessor As IHistoryProcessor
		Public MustOverride ReadOnly Property CurrentEpisodeStepCount As Integer Implements IEpochTrainer.getCurrentEpisodeStepCount
		Public MustOverride ReadOnly Property EpisodeCount As Integer Implements IEpochTrainer.getEpisodeCount
		Public MustOverride ReadOnly Property EpochCount As Integer Implements IEpochTrainer.getEpochCount
		Public MustOverride ReadOnly Property StepCount As Integer Implements IEpochTrainer.getStepCount
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int threadNumber;
		Private threadNumber As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final int deviceNum;
		Protected Friend ReadOnly deviceNum As Integer

		''' <summary>
		''' The number of steps that this async thread has produced
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int stepCount = 0;
		Protected Friend stepCount As Integer = 0

		''' <summary>
		''' The number of epochs (updates) that this thread has sent to the global learner
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int epochCount = 0;
		Protected Friend epochCount As Integer = 0

		''' <summary>
		''' The number of environment episodes that have been played out
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected int episodeCount = 0;
		Protected Friend episodeCount As Integer = 0

		''' <summary>
		''' The number of steps in the current episode
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected int currentEpisodeStepCount = 0;
		Protected Friend currentEpisodeStepCount As Integer = 0

		''' <summary>
		''' If the current episode needs to be reset
		''' </summary>
		Friend episodeComplete As Boolean = True

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.deeplearning4j.rl4j.learning.IHistoryProcessor historyProcessor;
'JAVA TO VB CONVERTER NOTE: The field historyProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private historyProcessor_Conflict As IHistoryProcessor

		Private isEpisodeStarted As Boolean = False
		Private ReadOnly mdp As LegacyMDPWrapper(Of OBSERVATION, ACTION, ACTION_SPACE)

		Private ReadOnly listeners As TrainingListenerList

		Public Sub New(ByVal mdp As MDP(Of OBSERVATION, ACTION, ACTION_SPACE), ByVal listeners As TrainingListenerList, ByVal threadNumber As Integer, ByVal deviceNum As Integer)
			Me.mdp = New LegacyMDPWrapper(Of OBSERVATION, ACTION, ACTION_SPACE)(mdp, Nothing)
			Me.listeners = listeners
			Me.threadNumber = threadNumber
			Me.deviceNum = deviceNum
		End Sub

		Public Overridable ReadOnly Property Mdp As MDP(Of OBSERVATION, ACTION, ACTION_SPACE)
			Get
				Return mdp.getWrappedMDP()
			End Get
		End Property
		Protected Friend Overridable ReadOnly Property LegacyMDPWrapper As LegacyMDPWrapper(Of OBSERVATION, ACTION, ACTION_SPACE)
			Get
				Return mdp
			End Get
		End Property

		Public Overridable WriteOnly Property HistoryProcessor As IHistoryProcessor.Configuration
			Set(ByVal conf As IHistoryProcessor.Configuration)
				setHistoryProcessor(New HistoryProcessor(conf))
			End Set
		End Property

		Public Overridable WriteOnly Property HistoryProcessor As IHistoryProcessor
			Set(ByVal historyProcessor As IHistoryProcessor)
				Me.historyProcessor_Conflict = historyProcessor
				mdp.HistoryProcessor = historyProcessor
			End Set
		End Property

		Protected Friend Overridable Sub postEpisode()
			If HistoryProcessor IsNot Nothing Then
				HistoryProcessor.stopMonitor()
			End If

		End Sub

		Protected Friend Overridable Sub preEpisode()
			' Do nothing
		End Sub

		''' <summary>
		''' This method will start the worker thread<para>
		''' The thread will stop when:<br>
		''' - The AsyncGlobal thread terminates or reports that the training is complete
		''' (see <seealso cref="AsyncGlobal.isTrainingComplete()"/>). In such case, the currently running epoch will still be handled normally and
		''' events will also be fired normally.<br>
		''' OR<br>
		''' - a listener explicitly stops it, in which case, the AsyncGlobal thread will be terminated along with
		''' all other worker threads <br>
		''' </para>
		''' <para>
		''' Listeners<br>
		''' For a given event, the listeners are called sequentially in same the order as they were added. If one listener
		''' returns {@link org.deeplearning4j.rl4j.learning.listener.TrainingListener.ListenerResponse
		''' TrainingListener.ListenerResponse.STOP}, the remaining listeners in the list won't be called.<br>
		''' Events:
		''' <ul>
		'''   <li><seealso cref="TrainingListener.onNewEpoch(IEpochTrainer) onNewEpoch()"/> is called when a new epoch is started.</li>
		'''   <li><seealso cref="TrainingListener.onEpochTrainingResult(IEpochTrainer, IDataManager.StatEntry) onEpochTrainingResult()"/> is called at the end of every
		'''   epoch. It will not be called if onNewEpoch() stops the training.</li>
		''' </ul>
		''' </para>
		''' </summary>
		Public Overrides Sub run()
			Dim context As New RunContext()
			Nd4j.AffinityManager.unsafeSetDevice(deviceNum)

			log.info("ThreadNum-" & threadNumber & " Started!")

			Do While Not getAsyncGlobal().TrainingComplete

				If episodeComplete Then
					startEpisode(context)
				End If

				If Not startEpoch(context) Then
					Exit Do
				End If

				episodeComplete = handleTraining(context)

				If Not finishEpoch(context) Then
					Exit Do
				End If

				If episodeComplete Then
					finishEpisode(context)
				End If
			Loop
		End Sub

		Private Function finishEpoch(ByVal context As RunContext) As Boolean
			epochCount += 1
			Dim statEntry As IDataManager.StatEntry = New AsyncStatEntry(stepCount, epochCount, context.rewards, currentEpisodeStepCount, context.score)
			Return listeners.notifyEpochTrainingResult(Me, statEntry)
		End Function

		Private Function startEpoch(ByVal context As RunContext) As Boolean
			Return listeners.notifyNewEpoch(Me)
		End Function

		Private Function handleTraining(ByVal context As RunContext) As Boolean
			Dim maxTrainSteps As Integer = Math.Min(Configuration.NStep, Configuration.MaxEpochStep - currentEpisodeStepCount)
			Dim subEpochReturn As SubEpochReturn = trainSubEpoch(context.obs, maxTrainSteps)

			context.obs = subEpochReturn.getLastObs()
			context.rewards += subEpochReturn.getReward()
			context.score = subEpochReturn.getScore()

			Return subEpochReturn.isEpisodeComplete()
		End Function

		Private Sub startEpisode(ByVal context As RunContext)
			Current.reset()
			Dim initMdp As Learning.InitMdp(Of Observation) = refacInitMdp()

			context.obs = initMdp.getLastObs()
			context.rewards = initMdp.getReward()

			preEpisode()
			episodeCount += 1
		End Sub

		Private Sub finishEpisode(ByVal context As RunContext)
			postEpisode()

			log.info("ThreadNum-{} Episode step: {}, Episode: {}, Epoch: {}, reward: {}", threadNumber, currentEpisodeStepCount, episodeCount, epochCount, context.rewards)
		End Sub

		Protected Friend MustOverride ReadOnly Property Current As NN

		Protected Friend MustOverride ReadOnly Property AsyncGlobal As IAsyncGlobal(Of NN)

		Protected Friend MustOverride ReadOnly Property Configuration As IAsyncLearningConfiguration

		Protected Friend MustOverride Function getPolicy(ByVal net As NN) As IPolicy(Of ACTION)

		Protected Friend MustOverride Function trainSubEpoch(ByVal obs As Observation, ByVal nstep As Integer) As SubEpochReturn

		Private Function refacInitMdp() As Learning.InitMdp(Of Observation)
			currentEpisodeStepCount = 0

			Dim reward As Double = 0

			Dim mdp As LegacyMDPWrapper(Of OBSERVATION, ACTION, ACTION_SPACE) = getLegacyMDPWrapper()
			Dim observation As Observation = mdp.reset()

			Dim action As ACTION = mdp.ActionSpace.noOp() 'by convention should be the NO_OP
			Do While observation.Skipped AndAlso Not mdp.Done
				Dim stepReply As StepReply(Of Observation) = mdp.step(action)

				reward += stepReply.getReward()
				observation = stepReply.getObservation()

				incrementSteps()
			Loop

			Return New Learning.InitMdp(0, observation, reward)

		End Function

		Public Overridable Sub incrementSteps()
			stepCount += 1
			currentEpisodeStepCount += 1
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Value public static class SubEpochReturn
		Public Class SubEpochReturn
			Friend steps As Integer
			Friend lastObs As Observation
			Friend reward As Double
			Friend score As Double
			Friend episodeComplete As Boolean
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Value public static class AsyncStatEntry implements org.deeplearning4j.rl4j.util.IDataManager.StatEntry
		Public Class AsyncStatEntry
			Implements IDataManager.StatEntry

			Friend stepCounter As Integer
			Friend epochCounter As Integer
			Friend reward As Double
			Friend episodeLength As Integer
			Friend score As Double
		End Class

		Private Class RunContext
			Friend obs As Observation
			Friend rewards As Double
			Friend score As Double
		End Class

	End Class

End Namespace