Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports org.deeplearning4j.rl4j.learning
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports DQN = org.deeplearning4j.rl4j.network.dqn.DQN
Imports org.deeplearning4j.rl4j.network.dqn
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class DQNPolicy<OBSERVATION> extends Policy<Integer>
	Public Class DQNPolicy(Of OBSERVATION)
		Inherits Policy(Of Integer)

'JAVA TO VB CONVERTER NOTE: The field neuralNet was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly neuralNet_Conflict As IOutputNeuralNet

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <OBSERVATION extends org.deeplearning4j.rl4j.space.Encodable> DQNPolicy<OBSERVATION> load(String path) throws java.io.IOException
		Public Shared Function load(Of OBSERVATION As Encodable)(ByVal path As String) As DQNPolicy(Of OBSERVATION)
			Return New DQNPolicy(Of OBSERVATION)(DQN.load(path))
		End Function

		Public Overrides ReadOnly Property NeuralNet As IOutputNeuralNet
			Get
				Return neuralNet_Conflict
			End Get
		End Property

		Public Overrides Function nextAction(ByVal obs As Observation) As Integer?
			Dim output As INDArray = neuralNet_Conflict.output(obs).get(CommonOutputNames.QValues)
			Return Learning.getMaxAction(output)
		End Function

		<Obsolete>
		Public Overridable Overloads Function nextAction(ByVal input As INDArray) As Integer?
			Dim output As INDArray = neuralNet_Conflict.output(input).get(CommonOutputNames.QValues)
			Return Learning.getMaxAction(output)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(String filename) throws java.io.IOException
		Public Overridable Sub save(ByVal filename As String)
			' TODO: refac load & save. Code below should continue to work in the meantime because it's only called by the lecacy code and it's only using a DQN network with DQNPolicy
			DirectCast(neuralNet_Conflict, IDQN).save(filename)
		End Sub

	End Class

End Namespace