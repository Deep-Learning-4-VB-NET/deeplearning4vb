Imports System
Imports Getter = lombok.Getter
Imports org.deeplearning4j.gym
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


	Public Class HardDeteministicToy
		Implements MDP(Of HardToyState, Integer, DiscreteSpace)

		Private Const MAX_STEP As Integer = 20
		Private Const SEED As Integer = 1234
		Private Const ACTION_SIZE As Integer = 10
		Private Shared ReadOnly states() As HardToyState = genToyStates(MAX_STEP, SEED)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.DiscreteSpace actionSpace = new org.deeplearning4j.rl4j.space.DiscreteSpace(ACTION_SIZE);
		Private actionSpace As New DiscreteSpace(ACTION_SIZE)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.ObservationSpace<HardToyState> observationSpace = new org.deeplearning4j.rl4j.space.ArrayObservationSpace(new int[] {ACTION_SIZE});
		Private observationSpace As ObservationSpace(Of HardToyState) = New ArrayObservationSpace(New Integer() {ACTION_SIZE})
		Private hardToyState As HardToyState

		Public Shared Sub printTest(ByVal idqn As IDQN)
			Dim input As INDArray = Nd4j.create(MAX_STEP, ACTION_SIZE)
			For i As Integer = 0 To MAX_STEP - 1
				input.putRow(i, Nd4j.create(states(i).toArray()))
			Next i
			Dim output As INDArray = Nd4j.max(idqn.output(input).get(CommonOutputNames.QValues), 1)
			Logger.getAnonymousLogger().info(output.ToString())
		End Sub

		Public Shared Function maxIndex(ByVal values() As Double) As Integer
			Dim maxValue As Double = -Double.Epsilon
'JAVA TO VB CONVERTER NOTE: The local variable maxIndex was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim maxIndex_Conflict As Integer = -1
			For i As Integer = 0 To values.Length - 1
				If values(i) > maxValue Then
					maxValue = values(i)
					maxIndex_Conflict = i
				End If
			Next i
			Return maxIndex_Conflict
		End Function

		Public Shared Function genToyStates(ByVal size As Integer, ByVal seed As Integer) As HardToyState()

			Dim rd As New Random(seed)
			Dim hardToyStates(size - 1) As HardToyState
			For i As Integer = 0 To size - 1

				Dim values(ACTION_SIZE - 1) As Double

				For j As Integer = 0 To ACTION_SIZE - 1
					values(j) = rd.NextDouble()
				Next j
				hardToyStates(i) = New HardToyState(values, i)
			Next i

			Return hardToyStates
		End Function

		Public Overridable Sub close() Implements MDP(Of HardToyState, Integer, DiscreteSpace).close
		End Sub

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of HardToyState, Integer, DiscreteSpace).isDone
			Get
    
				Return hardToyState.getStep() = MAX_STEP - 1
			End Get
		End Property

		Public Overridable Function reset() As HardToyState

				hardToyState = states(0)
				Return hardToyState
		End Function

		Public Overridable Function [step](ByVal a As Integer?) As StepReply(Of HardToyState)
			Dim reward As Double = 0
			If a = maxIndex(hardToyState.getValues()) Then
				reward += 1
			End If
			hardToyState = states(hardToyState.getStep() + 1)
			Return New StepReply(hardToyState, reward, Done, Nothing)
		End Function

		Public Overridable Function newInstance() As HardDeteministicToy
			Return New HardDeteministicToy()
		End Function

	End Class

End Namespace