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
Namespace org.deeplearning4j.rl4j.observation.transform


	Public Interface FilterOperation
		''' <summary>
		''' The logic that determines if the observation should be skipped.
		''' </summary>
		''' <param name="channelsData"> the name of the channel </param>
		''' <param name="currentObservationStep"> The step number if the observation in the current episode. </param>
		''' <param name="isFinalObservation"> true if this is the last observation of the episode </param>
		''' <returns> true if the observation should be skipped </returns>
		Function isSkipped(ByVal channelsData As IDictionary(Of String, Object), ByVal currentObservationStep As Integer, ByVal isFinalObservation As Boolean) As Boolean
	End Interface

End Namespace