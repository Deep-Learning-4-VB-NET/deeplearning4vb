Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports org.deeplearning4j.rl4j.agent.listener
Imports org.deeplearning4j.rl4j.agent.listener
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy
Imports Preconditions = org.nd4j.common.base.Preconditions

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
Namespace org.deeplearning4j.rl4j.agent


	Public Class Agent(Of ACTION)
		Implements IAgent(Of ACTION)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final String id;
		Private ReadOnly id As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.environment.Environment<ACTION> environment;
		Private ReadOnly environment As Environment(Of ACTION)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.policy.IPolicy<ACTION> policy;
		Private ReadOnly policy As IPolicy(Of ACTION)

		Private ReadOnly transformProcess As TransformProcess

		Protected Friend ReadOnly listeners As AgentListenerList(Of ACTION)

		Private ReadOnly maxEpisodeSteps As Integer?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.observation.Observation observation;
		Private observation As Observation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private ACTION lastAction;
		Private lastAction As ACTION

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int episodeStepCount;
		Private episodeStepCount As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private double reward;
		Private reward As Double

		Protected Friend canContinue As Boolean

		''' <param name="environment"> The <seealso cref="Environment"/> to be used </param>
		''' <param name="transformProcess"> The <seealso cref="TransformProcess"/> to be used to transform the raw observations into usable ones. </param>
		''' <param name="policy"> The <seealso cref="IPolicy"/> to be used </param>
		''' <param name="configuration"> The configuration for the agent </param>
		''' <param name="id"> A user-supplied id to identify the instance. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Agent(@NonNull Environment<ACTION> environment, @NonNull TransformProcess transformProcess, @NonNull IPolicy<ACTION> policy, @NonNull Configuration configuration, String id)
		Public Sub New(ByVal environment As Environment(Of ACTION), ByVal transformProcess As TransformProcess, ByVal policy As IPolicy(Of ACTION), ByVal configuration As Configuration, ByVal id As String)
			Preconditions.checkArgument(configuration.getMaxEpisodeSteps() Is Nothing OrElse configuration.getMaxEpisodeSteps() > 0, "Configuration: maxEpisodeSteps must be null (no maximum) or greater than 0, got", configuration.getMaxEpisodeSteps())

			Me.environment = environment
			Me.transformProcess = transformProcess
			Me.policy = policy
			Me.maxEpisodeSteps = configuration.getMaxEpisodeSteps()
			Me.id = id

			listeners = buildListenerList()
		End Sub

		Protected Friend Overridable Function buildListenerList() As AgentListenerList(Of ACTION)
			Return New AgentListenerList(Of ACTION)()
		End Function

		''' <summary>
		''' Add a <seealso cref="AgentListener"/> that will be notified when agent events happens </summary>
		''' <param name="listener"> </param>
		Public Overridable Sub addListener(ByVal listener As AgentListener)
			listeners.add(listener)
		End Sub

		''' <summary>
		''' This will run a single episode
		''' </summary>
		Public Overridable Sub run() Implements IAgent(Of ACTION).run
			runEpisode()
		End Sub

		Protected Friend Overridable Sub onBeforeEpisode()
			' Do Nothing
		End Sub

		Protected Friend Overridable Sub onAfterEpisode()
			' Do Nothing
		End Sub

		Protected Friend Overridable Sub runEpisode()
			reset()
			onBeforeEpisode()

			canContinue = listeners.notifyBeforeEpisode(Me)

			Do While canContinue AndAlso Not environment.EpisodeFinished AndAlso (maxEpisodeSteps Is Nothing OrElse episodeStepCount < maxEpisodeSteps)
				performStep()
			Loop

			If Not canContinue Then
				Return
			End If

			onAfterEpisode()
			listeners.notifyAfterEpisode(Me)
		End Sub

		Protected Friend Overridable Sub reset()
			resetEnvironment()
			resetPolicy()
			reward = 0
			lastAction = InitialAction
			canContinue = True
		End Sub

		Protected Friend Overridable Sub resetEnvironment()
			episodeStepCount = 0
			Dim channelsData As IDictionary(Of String, Object) = environment.reset()
			Me.observation = transformProcess.transform(channelsData, episodeStepCount, False)
		End Sub

		Protected Friend Overridable Sub resetPolicy()
			policy.reset()
		End Sub

		Protected Friend Overridable ReadOnly Property InitialAction As ACTION
			Get
				Return environment.getSchema().getActionSchema().getNoOp()
			End Get
		End Property

		Protected Friend Overridable Sub performStep()

			onBeforeStep()

			Dim action As ACTION = decideAction(observation)

			canContinue = listeners.notifyBeforeStep(Me, observation, action)
			If Not canContinue Then
				Return
			End If

			Dim stepResult As StepResult = act(action)

			onAfterStep(stepResult)

			canContinue = listeners.notifyAfterStep(Me, stepResult)
			If Not canContinue Then
				Return
			End If

			incrementEpisodeStepCount()
		End Sub

		Protected Friend Overridable Sub incrementEpisodeStepCount()
			episodeStepCount += 1
		End Sub

		Protected Friend Overridable Function decideAction(ByVal observation As Observation) As ACTION
			If Not observation.Skipped Then
				lastAction = policy.nextAction(observation)
			End If

			Return lastAction
		End Function

		Protected Friend Overridable Function act(ByVal action As ACTION) As StepResult
			Dim observationBeforeAction As Observation = observation

			Dim stepResult As StepResult = environment.step(action)
			observation = convertChannelDataToObservation(stepResult, episodeStepCount + 1)
			reward += computeReward(stepResult)

			onAfterAction(observationBeforeAction, action, stepResult)

			Return stepResult
		End Function

		Protected Friend Overridable Function convertChannelDataToObservation(ByVal stepResult As StepResult, ByVal episodeStepNumberOfObs As Integer) As Observation
			Return transformProcess.transform(stepResult.getChannelsData(), episodeStepNumberOfObs, stepResult.isTerminal())
		End Function

		Protected Friend Overridable Function computeReward(ByVal stepResult As StepResult) As Double
			Return stepResult.getReward()
		End Function

		Protected Friend Overridable Sub onAfterAction(ByVal observationBeforeAction As Observation, ByVal action As ACTION, ByVal stepResult As StepResult)
			' Do Nothing
		End Sub

		Protected Friend Overridable Sub onAfterStep(ByVal stepResult As StepResult)
			' Do Nothing
		End Sub

		Protected Friend Overridable Sub onBeforeStep()
			' Do Nothing
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration
		Public Class Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @lombok.Builder.@Default Integer maxEpisodeSteps = null;
			Friend maxEpisodeSteps As Integer = Nothing ' Default, no max
		End Class
	End Class
End Namespace