Imports System.Collections.Generic
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


	Public Class AgentListenerList(Of ACTION)
		Protected Friend ReadOnly listeners As IList(Of AgentListener(Of ACTION)) = New List(Of AgentListener(Of ACTION))()

		''' <summary>
		''' Add a listener at the end of the list </summary>
		''' <param name="listener"> The listener to be added </param>
		Public Overridable Sub add(ByVal listener As AgentListener(Of ACTION))
			listeners.Add(listener)
		End Sub

		''' <summary>
		''' This method will notify all listeners that an episode is about to start. If a listener returns
		''' <seealso cref="AgentListener.ListenerResponse STOP"/>, any following listener is skipped.
		''' </summary>
		''' <param name="agent"> The agent that generated the event. </param>
		''' <returns> False if the processing should be stopped </returns>
		Public Overridable Function notifyBeforeEpisode(ByVal agent As Agent(Of ACTION)) As Boolean
			For Each listener As AgentListener(Of ACTION) In listeners
				If listener.onBeforeEpisode(agent) = AgentListener.ListenerResponse.STOP Then
					Return False
				End If
			Next listener

			Return True
		End Function

		''' 
		''' <param name="agent"> The agent that generated the event. </param>
		''' <param name="observation"> The observation before the action is taken </param>
		''' <param name="action"> The action that will be performed </param>
		''' <returns> False if the processing should be stopped </returns>
		Public Overridable Function notifyBeforeStep(ByVal agent As Agent(Of ACTION), ByVal observation As Observation, ByVal action As ACTION) As Boolean
			For Each listener As AgentListener(Of ACTION) In listeners
				If listener.onBeforeStep(agent, observation, action) = AgentListener.ListenerResponse.STOP Then
					Return False
				End If
			Next listener

			Return True
		End Function

		''' 
		''' <param name="agent"> The agent that generated the event. </param>
		''' <param name="stepResult"> The <seealso cref="StepResult"/> result of the step. </param>
		''' <returns> False if the processing should be stopped </returns>
		Public Overridable Function notifyAfterStep(ByVal agent As Agent(Of ACTION), ByVal stepResult As StepResult) As Boolean
			For Each listener As AgentListener(Of ACTION) In listeners
				If listener.onAfterStep(agent, stepResult) = AgentListener.ListenerResponse.STOP Then
					Return False
				End If
			Next listener

			Return True
		End Function

		''' <summary>
		''' This method will notify all listeners that an episode has finished.
		''' </summary>
		''' <param name="agent"> The agent that generated the event. </param>
		Public Overridable Sub notifyAfterEpisode(ByVal agent As Agent(Of ACTION))
			For Each listener As AgentListener(Of ACTION) In listeners
				listener.onAfterEpisode(agent)
			Next listener
		End Sub

	End Class

End Namespace