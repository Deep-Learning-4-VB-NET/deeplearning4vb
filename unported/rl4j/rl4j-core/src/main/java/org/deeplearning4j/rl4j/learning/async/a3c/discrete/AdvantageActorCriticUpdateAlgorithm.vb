Imports System.Collections.Generic
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.network.ac
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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
Namespace org.deeplearning4j.rl4j.learning.async.a3c.discrete


	Public Class AdvantageActorCriticUpdateAlgorithm
		Implements UpdateAlgorithm(Of IActorCritic)

		Private ReadOnly shape() As Integer
		Private ReadOnly actionSpaceSize As Integer
		Private ReadOnly gamma As Double
		Private ReadOnly recurrent As Boolean

		Public Sub New(ByVal recurrent As Boolean, ByVal shape() As Integer, ByVal actionSpaceSize As Integer, ByVal gamma As Double)

			'if recurrent then train as a time serie with a batch size of 1
			Me.recurrent = recurrent
			Me.shape = shape
			Me.actionSpaceSize = actionSpaceSize
			Me.gamma = gamma
		End Sub

		Public Overridable Function computeGradients(ByVal current As IActorCritic, ByVal experience As IList(Of StateActionReward(Of Integer))) As Gradient()
			Dim size As Integer = experience.Count

			Dim nshape() As Integer = If(recurrent, Learning.makeShape(1, shape, size), Learning.makeShape(size, shape))

			Dim input As INDArray = Nd4j.create(nshape)
			Dim targets As INDArray = If(recurrent, Nd4j.create(1, 1, size), Nd4j.create(size, 1))
			Dim logSoftmax As INDArray = If(recurrent, Nd4j.zeros(1, actionSpaceSize, size), Nd4j.zeros(size, actionSpaceSize))

			Dim stateActionReward As StateActionReward(Of Integer) = experience(size - 1)
			Dim value As Double
			If stateActionReward.isTerminal() Then
				value = 0
			Else
				Dim output() As INDArray = current.outputAll(stateActionReward.getObservation().getChannelData(0))
				value = output(0).getDouble(0)
			End If

			For i As Integer = size - 1 To 0 Step -1
				stateActionReward = experience(i)

				Dim observationData As INDArray = stateActionReward.getObservation().getChannelData(0)

				Dim output() As INDArray = current.outputAll(observationData)

				value = stateActionReward.getReward() + gamma * value
				If recurrent Then
					input.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(i)).assign(observationData)
				Else
					input.putRow(i, observationData)
				End If

				'the critic
				targets.putScalar(i, value)

				'the actor
				Dim expectedV As Double = output(0).getDouble(0)
				Dim advantage As Double = value - expectedV
				If recurrent Then
					logSoftmax.putScalar(0, stateActionReward.getAction(), i, advantage)
				Else
					logSoftmax.putScalar(i, stateActionReward.getAction(), advantage)
				End If
			Next i

			' targets -> value, critic
			' logSoftmax -> policy, actor
			Return current.gradient(input, New INDArray(){targets, logSoftmax})
		End Function
	End Class

End Namespace