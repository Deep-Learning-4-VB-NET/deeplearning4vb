Imports System
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports org.deeplearning4j.rl4j.network.dqn
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
import static org.nd4j.linalg.ops.transforms.Transforms.exp

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

	Public Class BoltzmannQ(Of OBSERVATION As org.deeplearning4j.rl4j.space.Encodable)
		Inherits Policy(Of Integer)

		Private ReadOnly dqn As IDQN
		Private ReadOnly rnd As Random

		Public Sub New(ByVal dqn As IDQN, ByVal random As Random)
			Me.dqn = dqn
			Me.rnd = random
		End Sub

		Public Overrides ReadOnly Property NeuralNet As IDQN
			Get
				Return dqn
			End Get
		End Property

		Public Overrides Function nextAction(ByVal obs As Observation) As Integer?
			Dim output As INDArray = dqn.output(obs).get(CommonOutputNames.QValues)
			Dim exp As INDArray = exp(output)

			Dim sum As Double = exp.sum(1).getDouble(0)
			Dim picked As Double = rnd.nextDouble() * sum
			Dim i As Integer = 0
			Do While i < exp.columns()
				If picked < exp.getDouble(i) Then
					Return i
				End If
				i += 1
			Loop
			Return -1
		End Function

		<Obsolete>
		Public Overridable Overloads Function nextAction(ByVal input As INDArray) As Integer?
			Dim output As INDArray = dqn.output(input).get(CommonOutputNames.QValues)
			Dim exp As INDArray = exp(output)

			Dim sum As Double = exp.sum(1).getDouble(0)
			Dim picked As Double = rnd.nextDouble() * sum
			Dim i As Integer = 0
			Do While i < exp.columns()
				If picked < exp.getDouble(i) Then
					Return i
				End If
				i += 1
			Loop
			Return -1
		End Function


	End Class

End Namespace