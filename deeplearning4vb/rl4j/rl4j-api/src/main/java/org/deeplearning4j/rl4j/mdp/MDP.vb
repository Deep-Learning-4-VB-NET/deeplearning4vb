Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.space

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

Namespace org.deeplearning4j.rl4j.mdp


	Public Interface MDP(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, ACTION, ACTION_SPACE As org.deeplearning4j.rl4j.space.ActionSpace(Of ACTION))

		ReadOnly Property ObservationSpace As ObservationSpace(Of OBSERVATION)

		ReadOnly Property ActionSpace As ACTION_SPACE

		Function reset() As OBSERVATION

		Sub close()

		Function [step](ByVal action As ACTION) As StepReply(Of OBSERVATION)

		ReadOnly Property Done As Boolean

		Function newInstance() As MDP(Of OBSERVATION, ACTION, ACTION_SPACE)

	End Interface

End Namespace