Imports org.deeplearning4j.rl4j.environment
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

	Public Interface IAgent(Of ACTION)
		''' <summary>
		''' Will play a single episode
		''' </summary>
		Sub run()

		''' <returns> A user-supplied id to identify the IAgent instance. </returns>
		ReadOnly Property Id As String

		''' <returns> The <seealso cref="Environment"/> instance being used by the agent. </returns>
		ReadOnly Property Environment As Environment(Of ACTION)

		''' <returns> The <seealso cref="IPolicy"/> instance being used by the agent. </returns>
		ReadOnly Property Policy As IPolicy(Of ACTION)

		''' <returns> The step count taken in the current episode. </returns>
		ReadOnly Property EpisodeStepCount As Integer

		''' <returns> The cumulative reward received in the current episode. </returns>
		ReadOnly Property Reward As Double
	End Interface

End Namespace