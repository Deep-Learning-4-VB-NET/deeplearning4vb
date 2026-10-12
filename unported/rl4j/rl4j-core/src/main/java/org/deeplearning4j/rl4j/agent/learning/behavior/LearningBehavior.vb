Imports Builder = lombok.Builder
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.rl4j.agent.learning.update
Imports org.deeplearning4j.rl4j.experience
Imports Observation = org.deeplearning4j.rl4j.observation.Observation

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
Namespace org.deeplearning4j.rl4j.agent.learning.behavior

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public class LearningBehavior<ACTION, EXPERIENCE_TYPE> implements ILearningBehavior<ACTION>
	Public Class LearningBehavior(Of ACTION, EXPERIENCE_TYPE)
		Implements ILearningBehavior(Of ACTION)

		Private hasBatchChanged As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private final org.deeplearning4j.rl4j.experience.ExperienceHandler<ACTION, EXPERIENCE_TYPE> experienceHandler;
		Private ReadOnly experienceHandler As ExperienceHandler(Of ACTION, EXPERIENCE_TYPE)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private final org.deeplearning4j.rl4j.agent.learning.update.IUpdateRule<EXPERIENCE_TYPE> updateRule;
		Private ReadOnly updateRule As IUpdateRule(Of EXPERIENCE_TYPE)

		Public Overridable Sub handleEpisodeStart() Implements ILearningBehavior(Of ACTION).handleEpisodeStart
			experienceHandler.reset()
		End Sub

		Public Overridable Sub handleNewExperience(ByVal observation As Observation, ByVal action As ACTION, ByVal reward As Double, ByVal isTerminal As Boolean) Implements ILearningBehavior(Of ACTION).handleNewExperience
			experienceHandler.addExperience(observation, action, reward, isTerminal)
			If experienceHandler.TrainingBatchReady Then
				handleBatch()
			End If
		End Sub

		Public Overridable Sub handleEpisodeEnd(ByVal finalObservation As Observation) Implements ILearningBehavior(Of ACTION).handleEpisodeEnd
			experienceHandler.FinalObservation = finalObservation
			If experienceHandler.TrainingBatchReady Then
				handleBatch()
			End If
		End Sub

		Private Sub handleBatch()
			updateRule.update(experienceHandler.generateTrainingBatch())
			hasBatchChanged = True
		End Sub

		''' <summary>
		''' Will notify the update rule if a new training batch has been started
		''' </summary>
		Public Overridable Sub notifyBeforeStep() Implements ILearningBehavior(Of ACTION).notifyBeforeStep
			If hasBatchChanged Then
				updateRule.notifyNewBatchStarted()
				hasBatchChanged = False
			End If
		End Sub
	End Class

End Namespace