Imports System.Collections.Generic
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
Namespace org.deeplearning4j.rl4j.experience


	Public Interface ExperienceHandler(Of A, E)
		Sub addExperience(ByVal observation As Observation, ByVal action As A, ByVal reward As Double, ByVal isTerminal As Boolean)

		''' <summary>
		''' Called when the episode is done with the last observation </summary>
		''' <param name="observation"> </param>
		WriteOnly Property FinalObservation As Observation

		''' <returns> The size of the list that will be returned by generateTrainingBatch(). </returns>
		ReadOnly Property TrainingBatchSize As Integer

		''' <returns> True if a batch is ready for training. </returns>
		ReadOnly Property TrainingBatchReady As Boolean

		''' <summary>
		''' The elements are returned in the historical order (i.e. in the order they happened) </summary>
		''' <returns> The list of experience elements </returns>
		Function generateTrainingBatch() As IList(Of E)

		''' <summary>
		''' Signal the experience handler that a new episode is starting
		''' </summary>
		Sub reset()
	End Interface

End Namespace