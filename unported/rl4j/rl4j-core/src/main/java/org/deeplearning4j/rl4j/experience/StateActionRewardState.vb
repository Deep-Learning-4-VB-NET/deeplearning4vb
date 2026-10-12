Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
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
'ORIGINAL LINE: @Data public class StateActionRewardState<A> implements org.deeplearning4j.rl4j.observation.IObservationSource
	Public Class StateActionRewardState(Of A)
		Implements IObservationSource

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter Observation observation;
		Friend observation As Observation

		Friend action As A
		Friend reward As Double
		Friend isTerminal As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter Observation nextObservation;
		Friend nextObservation As Observation

		Public Sub New(ByVal observation As Observation, ByVal action As A, ByVal reward As Double, ByVal isTerminal As Boolean)
			Me.observation = observation
			Me.action = action
			Me.reward = reward
			Me.isTerminal = isTerminal
			Me.nextObservation = Nothing
		End Sub

		Private Sub New(ByVal observation As Observation, ByVal action As A, ByVal reward As Double, ByVal isTerminal As Boolean, ByVal nextObservation As Observation)
			Me.observation = observation
			Me.action = action
			Me.reward = reward
			Me.isTerminal = isTerminal
			Me.nextObservation = nextObservation
		End Sub

		''' <returns> a duplicate of this instance </returns>
		Public Overridable Function dup() As StateActionRewardState(Of A)
			Dim dupObservation As Observation = observation.dup()
			Dim nextObs As Observation = nextObservation.dup()

			Return New StateActionRewardState(Of A)(dupObservation, action, reward, isTerminal, nextObs)
		End Function
	End Class

End Namespace