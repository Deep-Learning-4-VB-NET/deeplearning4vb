Imports org.deeplearning4j.rl4j.agent
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
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
Namespace org.deeplearning4j.rl4j.agent.listener

	''' <summary>
	''' The base definition of all <seealso cref="Agent"/> event listeners
	''' </summary>
	Public Interface AgentListener(Of ACTION)
		Friend Enum ListenerResponse
			''' <summary>
			''' Tell the <seealso cref="Agent"/> to continue calling the listeners and the processing.
			''' </summary>
			[CONTINUE]

			''' <summary>
			''' Tell the <seealso cref="Agent"/> to interrupt calling the listeners and stop the processing.
			''' </summary>
			[STOP]
		End Enum

		''' <summary>
		''' Called when a new episode is about to start. </summary>
		''' <param name="agent"> The agent that generated the event
		''' </param>
		''' <returns> A <seealso cref="ListenerResponse"/>. </returns>
		Function onBeforeEpisode(ByVal agent As Agent) As AgentListener.ListenerResponse

		''' <summary>
		''' Called when a step is about to be taken.
		''' </summary>
		''' <param name="agent"> The agent that generated the event </param>
		''' <param name="observation"> The observation before the action is taken </param>
		''' <param name="action"> The action that will be performed
		''' </param>
		''' <returns> A <seealso cref="ListenerResponse"/>. </returns>
		Function onBeforeStep(ByVal agent As Agent, ByVal observation As Observation, ByVal action As ACTION) As AgentListener.ListenerResponse

		''' <summary>
		''' Called after a step has been taken.
		''' </summary>
		''' <param name="agent"> The agent that generated the event </param>
		''' <param name="stepResult"> The <seealso cref="StepResult"/> result of the step.
		''' </param>
		''' <returns> A <seealso cref="ListenerResponse"/>. </returns>
		Function onAfterStep(ByVal agent As Agent, ByVal stepResult As StepResult) As AgentListener.ListenerResponse

		''' <summary>
		''' Called after the episode has ended.
		''' </summary>
		''' <param name="agent"> The agent that generated the event
		'''  </param>
		Sub onAfterEpisode(ByVal agent As Agent)
	End Interface

End Namespace