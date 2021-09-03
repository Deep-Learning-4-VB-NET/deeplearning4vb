Imports System
Imports IHistoryProcessor = org.deeplearning4j.rl4j.learning.IHistoryProcessor
Imports org.deeplearning4j.rl4j.mdp
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.policy

	Public Interface IPolicy(Of ACTION)
		 <Obsolete>
		 Function play(Of O As Encodable, [AS] As ActionSpace(Of ACTION))(ByVal mdp As MDP(Of O, ACTION, [AS]), ByVal hp As IHistoryProcessor) As Double

		<Obsolete>
		Function nextAction(ByVal input As INDArray) As ACTION

		Function nextAction(ByVal observation As Observation) As ACTION

		Sub reset()
	End Interface

End Namespace