Imports System.Collections.Generic
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.rl4j.experience
Imports INDArrayHelper = org.deeplearning4j.rl4j.helper.INDArrayHelper
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.network.dqn
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
Namespace org.deeplearning4j.rl4j.learning.async.nstep.discrete


	Public Class QLearningUpdateAlgorithm
		Implements UpdateAlgorithm(Of IDQN)

		Private ReadOnly actionSpaceSize As Integer
		Private ReadOnly gamma As Double

		Public Sub New(ByVal actionSpaceSize As Integer, ByVal gamma As Double)

			Me.actionSpaceSize = actionSpaceSize
			Me.gamma = gamma
		End Sub

		Public Overridable Function computeGradients(ByVal current As IDQN, ByVal experience As IList(Of StateActionReward(Of Integer))) As Gradient()
			Dim size As Integer = experience.Count

			Dim stateActionReward As StateActionReward(Of Integer) = experience(size - 1)

			Dim data As INDArray = stateActionReward.getObservation().getChannelData(0)
			Dim features As INDArray = INDArrayHelper.createBatchForShape(size, data.shape())
			Dim targets As INDArray = Nd4j.create(size, actionSpaceSize)

			Dim r As Double
			If stateActionReward.isTerminal() Then
				r = 0
			Else
				Dim output() As INDArray = Nothing
				output = current.outputAll(data)
				r = Nd4j.max(output(0)).getDouble(0)
			End If

			For i As Integer = size - 1 To 0 Step -1
				stateActionReward = experience(i)
				data = stateActionReward.getObservation().getChannelData(0)

				features.putRow(i, data)

				r = stateActionReward.getReward() + gamma * r
				Dim output() As INDArray = current.outputAll(data)
				Dim row As INDArray = output(0)
				row = row.putScalar(stateActionReward.getAction(), r)
				targets.putRow(i, row)
			Next i

			Return current.gradient(features, targets)
		End Function
	End Class

End Namespace