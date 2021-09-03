Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.dqn

	Public Class StandardDQN
		Inherits BaseDQNAlgorithm

		Private Const ACTION_DIMENSION_IDX As Integer = 1

		''' <summary>
		''' In literature, this corresponds to: max<sub>a</sub> Q<sub>tar</sub>(s<sub>t+1</sub>, a)
		''' </summary>
		Private maxActionsFromQTargetNextObservation As INDArray

		Public Sub New(ByVal qNetwork As IOutputNeuralNet, ByVal targetQNetwork As IOutputNeuralNet, ByVal configuration As Configuration)
			MyBase.New(qNetwork, targetQNetwork, configuration)
		End Sub


		Protected Friend Overrides Sub initComputation(ByVal features As Features, ByVal nextFeatures As Features)
			MyBase.initComputation(features, nextFeatures)

			maxActionsFromQTargetNextObservation = Nd4j.max(targetQNetworkNextFeatures, ACTION_DIMENSION_IDX)
		End Sub

		''' <summary>
		''' In literature, this corresponds to:<br />
		'''      Q(s<sub>t</sub>, a<sub>t</sub>) = R<sub>t+1</sub> + &gamma; * max<sub>a</sub> Q<sub>tar</sub>(s<sub>t+1</sub>, a) </summary>
		''' <param name="batchIdx"> The index in the batch of the current transition </param>
		''' <param name="reward"> The reward of the current transition </param>
		''' <param name="isTerminal"> True if it's the last transition of the "game" </param>
		''' <returns> The estimated Q-Value </returns>
		Protected Friend Overrides Function computeTarget(ByVal batchIdx As Integer, ByVal reward As Double, ByVal isTerminal As Boolean) As Double
			Dim yTarget As Double = reward
			If Not isTerminal Then
				yTarget += gamma * maxActionsFromQTargetNextObservation.getDouble(batchIdx)
			End If

			Return yTarget
		End Function
	End Class

End Namespace