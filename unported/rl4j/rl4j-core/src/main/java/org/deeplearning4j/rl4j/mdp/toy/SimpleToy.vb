Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.learning
Imports org.deeplearning4j.rl4j.mdp
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports org.deeplearning4j.rl4j.network.dqn
Imports org.deeplearning4j.rl4j.space
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports org.deeplearning4j.rl4j.space
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

Namespace org.deeplearning4j.rl4j.mdp.toy


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SimpleToy implements org.deeplearning4j.rl4j.mdp.MDP<SimpleToyState, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace>
	Public Class SimpleToy
		Implements MDP(Of SimpleToyState, Integer, DiscreteSpace)

		Private ReadOnly maxStep As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.DiscreteSpace actionSpace = new org.deeplearning4j.rl4j.space.DiscreteSpace(2);
		Private actionSpace As New DiscreteSpace(2)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.ObservationSpace<SimpleToyState> observationSpace = new org.deeplearning4j.rl4j.space.ArrayObservationSpace(new int[] {1});
		Private observationSpace As ObservationSpace(Of SimpleToyState) = New ArrayObservationSpace(New Integer() {1})
		Private simpleToyState As SimpleToyState
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private org.deeplearning4j.rl4j.learning.NeuralNetFetchable<org.deeplearning4j.rl4j.network.dqn.IDQN> fetchable;
		Private fetchable As NeuralNetFetchable(Of IDQN)

		Public Sub New(ByVal maxStep As Integer)
			Me.maxStep = maxStep
		End Sub

		Public Overridable Sub printTest(ByVal maxStep As Integer)
			Dim input As INDArray = Nd4j.create(maxStep, 1)
			For i As Integer = 0 To maxStep - 1
				input.putRow(i, Nd4j.create((New SimpleToyState(i, i)).toArray()))
			Next i
			Dim output As INDArray = fetchable.NeuralNet.output(input).get(CommonOutputNames.QValues)
			log.info(output.ToString())
		End Sub

		Public Overridable Sub close() Implements MDP(Of SimpleToyState, Integer, DiscreteSpace).close
		End Sub

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of SimpleToyState, Integer, DiscreteSpace).isDone
			Get
				Return simpleToyState.getStep() = maxStep
			End Get
		End Property

		Public Overridable Function reset() As SimpleToyState
			If fetchable IsNot Nothing Then
				printTest(maxStep)
			End If

				simpleToyState = New SimpleToyState(0, 0)
				Return simpleToyState
		End Function

		Public Overridable Function [step](ByVal a As Integer?) As StepReply(Of SimpleToyState)
			Dim reward As Double = If(simpleToyState.getStep() Mod 2 = 0, 1 - a.Value, a)
			simpleToyState = New SimpleToyState(simpleToyState.getI() + 1, simpleToyState.getStep() + 1)
			Return New StepReply(Of SimpleToyState)(simpleToyState, reward, Done, Nothing)
		End Function

		Public Overridable Function newInstance() As SimpleToy
			Dim simpleToy As New SimpleToy(maxStep)
			simpleToy.setFetchable(fetchable)
			Return simpleToy
		End Function

	End Class

End Namespace