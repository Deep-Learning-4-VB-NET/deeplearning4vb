Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports IAsyncLearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.IAsyncLearningConfiguration
Imports TrainingListenerList = org.deeplearning4j.rl4j.learning.listener.TrainingListenerList
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.network
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.policy
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace

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

	Public MustInherit Class AsyncThreadDiscrete(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, NN As org.deeplearning4j.rl4j.network.NeuralNet)
		Inherits AsyncThread(Of OBSERVATION, Integer, DiscreteSpace, NN)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private NN current;
		Private current As NN

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.@PROTECTED) private UpdateAlgorithm<NN> updateAlgorithm;
		Private updateAlgorithm As UpdateAlgorithm(Of NN)

		' TODO: Make it configurable with a builder
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.@PROTECTED) @Getter private org.deeplearning4j.rl4j.experience.ExperienceHandler experienceHandler;
		Private experienceHandler As ExperienceHandler

		Public Sub New(ByVal asyncGlobal As IAsyncGlobal(Of NN), ByVal mdp As MDP(Of OBSERVATION, Integer, DiscreteSpace), ByVal listeners As TrainingListenerList, ByVal threadNumber As Integer, ByVal deviceNum As Integer)
			MyBase.New(mdp, listeners, threadNumber, deviceNum)
			SyncLock asyncGlobal
				current = CType(asyncGlobal.Target.clone(), NN)
			End SyncLock

			Dim experienceHandlerConfiguration As StateActionExperienceHandler.Configuration = StateActionExperienceHandler.Configuration.builder().batchSize(NStep).build()
			experienceHandler = New StateActionExperienceHandler(experienceHandlerConfiguration)
		End Sub

		Private ReadOnly Property NStep As Integer
			Get
				Dim configuration As IAsyncLearningConfiguration = Me.Configuration
				If configuration Is Nothing Then
					Return Integer.MaxValue
				End If
    
				Return configuration.NStep
			End Get
		End Property

		' TODO: Add an actor-learner class and be able to inject the update algorithm
		Protected Friend MustOverride Function buildUpdateAlgorithm() As UpdateAlgorithm(Of NN)

		Public Overrides WriteOnly Property HistoryProcessor As IHistoryProcessor
			Set(ByVal historyProcessor As IHistoryProcessor)
				MyBase.HistoryProcessor = historyProcessor
				updateAlgorithm = buildUpdateAlgorithm()
			End Set
		End Property

		Protected Friend Overrides Sub preEpisode()
			experienceHandler.reset()
		End Sub


		''' <summary>
		''' "Subepoch"  correspond to the t_max-step iterations
		''' that stack rewards with t_max MiniTrans
		''' </summary>
		''' <param name="sObs">  the obs to start from </param>
		''' <param name="trainingSteps"> the number of training steps </param>
		''' <returns> subepoch training informations </returns>
		Public Overrides Function trainSubEpoch(ByVal sObs As Observation, ByVal trainingSteps As Integer) As SubEpochReturn

			current.copyFrom(getAsyncGlobal().Target)

			Dim obs As Observation = sObs
			Dim policy As IPolicy(Of Integer) = getPolicy(current)

			Dim action As Integer? = getMdp().ActionSpace.noOp()

			Dim reward As Double = 0
			Dim accuReward As Double = 0

			Do While Not getMdp().Done AndAlso experienceHandler.getTrainingBatchSize() <> trainingSteps

				'if step of training, just repeat lastAction
				If Not obs.Skipped Then
					action = policy.nextAction(obs)
				End If

				Dim stepReply As StepReply(Of Observation) = getLegacyMDPWrapper().step(action)
				accuReward += stepReply.getReward() * Configuration.RewardFactor

				If Not obs.Skipped Then
					experienceHandler.addExperience(obs, action, accuReward, stepReply.isDone())
					accuReward = 0

					incrementSteps()
				End If

				obs = stepReply.getObservation()
				reward += stepReply.getReward()

			Loop

			Dim episodeComplete As Boolean = getMdp().Done OrElse Configuration.MaxEpochStep = currentEpisodeStepCount

			If episodeComplete AndAlso experienceHandler.getTrainingBatchSize() <> trainingSteps Then
				experienceHandler.setFinalObservation(obs)
			End If

			Dim experienceSize As Integer = experienceHandler.getTrainingBatchSize()

			getAsyncGlobal().applyGradient(updateAlgorithm.computeGradients(current, experienceHandler.generateTrainingBatch()), experienceSize)

			Return New SubEpochReturn(experienceSize, obs, reward, current.getLatestScore(), episodeComplete)
		End Function

	End Class

End Namespace