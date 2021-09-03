Imports System.Collections.Generic

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
Namespace org.deeplearning4j.rl4j.environment


	Public Interface Environment(Of ACTION)

		''' <returns> The <seealso cref="Schema"/> of the environment </returns>
		ReadOnly Property Schema As Schema(Of ACTION)

		''' <summary>
		''' Reset the environment's state to start a new episode.
		''' @return
		''' </summary>
		Function reset() As IDictionary(Of String, Object)

		''' <summary>
		''' Perform a single step.
		''' </summary>
		''' <param name="action"> The action taken </param>
		''' <returns> A <seealso cref="StepResult"/> describing the result of the step. </returns>
		Function [step](ByVal action As ACTION) As StepResult

		''' <returns> True if the episode is finished </returns>
		ReadOnly Property EpisodeFinished As Boolean

		''' <summary>
		''' Called when the agent is finished using this environment instance.
		''' </summary>
		Sub close()
	End Interface

End Namespace