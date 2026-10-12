Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports org.deeplearning4j.rl4j.agent.learning.behavior
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy

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

	Public Class AgentLearner(Of ACTION)
		Inherits Agent(Of ACTION)
		Implements IAgentLearner(Of ACTION)

		Private ReadOnly learningBehavior As ILearningBehavior(Of ACTION)
		Private rewardAtLastExperience As Double

		''' 
		''' <param name="environment"> The <seealso cref="Environment"/> to be used </param>
		''' <param name="transformProcess"> The <seealso cref="TransformProcess"/> to be used to transform the raw observations into usable ones. </param>
		''' <param name="policy"> The <seealso cref="IPolicy"/> to be used </param>
		''' <param name="configuration"> The configuration for the AgentLearner </param>
		''' <param name="id"> A user-supplied id to identify the instance. </param>
		''' <param name="learningBehavior"> The <seealso cref="ILearningBehavior"/> that will be used to supervise the learning. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AgentLearner(org.deeplearning4j.rl4j.environment.Environment<ACTION> environment, org.deeplearning4j.rl4j.observation.transform.TransformProcess transformProcess, org.deeplearning4j.rl4j.policy.IPolicy<ACTION> policy, Configuration configuration, String id, @NonNull ILearningBehavior<ACTION> learningBehavior)
		Public Sub New(ByVal environment As Environment(Of ACTION), ByVal transformProcess As TransformProcess, ByVal policy As IPolicy(Of ACTION), ByVal configuration As Configuration, ByVal id As String, ByVal learningBehavior As ILearningBehavior(Of ACTION))
			MyBase.New(environment, transformProcess, policy, configuration, id)

			Me.learningBehavior = learningBehavior
		End Sub

		Protected Friend Overrides Sub reset()
			MyBase.reset()

			rewardAtLastExperience = 0
		End Sub

		Protected Friend Overrides Sub onBeforeEpisode()
			MyBase.onBeforeEpisode()

			learningBehavior.handleEpisodeStart()
		End Sub

		Protected Friend Overrides Sub onAfterAction(ByVal observationBeforeAction As Observation, ByVal action As ACTION, ByVal stepResult As StepResult)
			If Not observationBeforeAction.Skipped Then
				Dim rewardSinceLastExperience As Double = Reward - rewardAtLastExperience
				learningBehavior.handleNewExperience(observationBeforeAction, action, rewardSinceLastExperience, stepResult.isTerminal())

				rewardAtLastExperience = Reward
			End If
		End Sub

		Protected Friend Overrides Sub onAfterEpisode()
			learningBehavior.handleEpisodeEnd(getObservation())
		End Sub

		Protected Friend Overrides Sub onBeforeStep()
			learningBehavior.notifyBeforeStep()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration extends Agent.Configuration
		Public Class Configuration
			Inherits Agent.Configuration

		End Class
	End Class

End Namespace