Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Getter = lombok.Getter
Imports IObservationSource = org.deeplearning4j.rl4j.observation.IObservationSource
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class StateActionReward<A> implements org.deeplearning4j.rl4j.observation.IObservationSource
	Public Class StateActionReward(Of A)
		Implements IObservationSource

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.observation.Observation observation;
		Private ReadOnly observation As Observation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final A action;
		Private ReadOnly action As A

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final double reward;
		Private ReadOnly reward As Double

		''' <summary>
		''' True if the episode ended after the action has been taken.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final boolean terminal;
		Private ReadOnly terminal As Boolean
	End Class

End Namespace