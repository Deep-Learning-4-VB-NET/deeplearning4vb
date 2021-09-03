Imports ILearningConfiguration = org.deeplearning4j.rl4j.learning.configuration.ILearningConfiguration
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.space
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable

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

Namespace org.deeplearning4j.rl4j.learning

	Public Interface ILearning(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable, A, [AS] As org.deeplearning4j.rl4j.space.ActionSpace(Of A))

		ReadOnly Property Policy As IPolicy(Of A)

		Sub train()

		ReadOnly Property StepCount As Integer

		ReadOnly Property Configuration As ILearningConfiguration

		ReadOnly Property Mdp As MDP(Of OBSERVATION, A, [AS])

		ReadOnly Property HistoryProcessor As IHistoryProcessor



	End Interface

End Namespace