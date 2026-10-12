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

	Public Interface ILearningBehavior(Of ACTION)

		''' <summary>
		''' This method is called when a new episode has been started.
		''' </summary>
		Sub handleEpisodeStart()

		''' <summary>
		''' This method is called when new experience is generated.
		''' </summary>
		''' <param name="observation"> The observation prior to taking the action </param>
		''' <param name="action"> The action that has been taken </param>
		''' <param name="reward"> The reward received by taking the action </param>
		''' <param name="isTerminal"> True if the episode ended after taking the action </param>
		Sub handleNewExperience(ByVal observation As Observation, ByVal action As ACTION, ByVal reward As Double, ByVal isTerminal As Boolean)

		''' <summary>
		''' This method is called when the episode ends or the maximum number of episode steps is reached.
		''' </summary>
		''' <param name="finalObservation"> The observation after the last action of the episode has been taken. </param>
		Sub handleEpisodeEnd(ByVal finalObservation As Observation)

		''' <summary>
		''' Notify the learning behavior that a step will be taken.
		''' </summary>
		Sub notifyBeforeStep()
	End Interface

End Namespace