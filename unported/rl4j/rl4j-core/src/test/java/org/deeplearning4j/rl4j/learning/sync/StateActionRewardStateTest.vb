Imports org.deeplearning4j.rl4j.experience
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Test = org.junit.jupiter.api.Test
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.rl4j.learning.sync

	Public Class StateActionRewardStateTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCtorWithoutHistory_expect_2DObservationAndNextObservation()
		Public Overridable Sub when_callingCtorWithoutHistory_expect_2DObservationAndNextObservation()
			' Arrange
			Dim obs() As Double = { 1.0, 2.0, 3.0 }
			Dim observation As Observation = buildObservation(obs)

			Dim nextObs() As Double = { 10.0, 20.0, 30.0 }
			Dim nextObservation As Observation = buildObservation(nextObs)

			' Act
			Dim stateActionRewardState As StateActionRewardState = buildTransition(observation, 123, 234.0, nextObservation)

			' Assert
			Dim expectedObservation()() As Double = { obs }
			assertExpected(expectedObservation, stateActionRewardState.Observation.getChannelData(0))

			Dim expectedNextObservation()() As Double = { nextObs }
			assertExpected(expectedNextObservation, stateActionRewardState.getNextObservation().getChannelData(0))

			assertEquals(123, stateActionRewardState.getAction())
			assertEquals(234.0, stateActionRewardState.getReward(), 0.0001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCtorWithHistory_expect_ObservationAndNextWithHistory()
		Public Overridable Sub when_callingCtorWithHistory_expect_ObservationAndNextWithHistory()
			' Arrange
			Dim obs()() As Double = {
				New Double() { 0.0, 1.0, 2.0 },
				New Double() { 3.0, 4.0, 5.0 },
				New Double() { 6.0, 7.0, 8.0 }
			}
			Dim observation As Observation = buildObservation(obs)

			Dim nextObs()() As Double = {
				New Double() { 10.0, 11.0, 12.0 },
				New Double() { 0.0, 1.0, 2.0 },
				New Double() { 3.0, 4.0, 5.0 }
			}
			Dim nextObservation As Observation = buildObservation(nextObs)

			' Act
			Dim stateActionRewardState As StateActionRewardState = buildTransition(observation, 123, 234.0, nextObservation)

			' Assert
			assertExpected(obs, stateActionRewardState.Observation.getChannelData(0))

			assertExpected(nextObs, stateActionRewardState.getNextObservation().getChannelData(0))

			assertEquals(123, stateActionRewardState.getAction())
			assertEquals(234.0, stateActionRewardState.getReward(), 0.0001)
		End Sub

		Private Function buildObservation(ByVal obs()() As Double) As Observation
			Dim history() As INDArray = { Nd4j.create(obs(0)).reshape(1, 3), Nd4j.create(obs(1)).reshape(1, 3), Nd4j.create(obs(2)).reshape(1, 3)}
			Return New Observation(Nd4j.concat(0, history))
		End Function

		Private Function buildObservation(ByVal obs() As Double) As Observation
			Return New Observation(Nd4j.create(obs).reshape(ChrW(1), 3))
		End Function

		Private Function buildNextObservation(ByVal obs()() As Double, ByVal nextObs() As Double) As Observation
			Dim nextHistory() As INDArray = { Nd4j.create(nextObs).reshape(ChrW(1), 3), Nd4j.create(obs(0)).reshape(1, 3), Nd4j.create(obs(1)).reshape(1, 3)}
			Return New Observation(Nd4j.concat(0, nextHistory))
		End Function

		Private Function buildTransition(ByVal observation As Observation, ByVal action As Integer, ByVal reward As Double, ByVal nextObservation As Observation) As StateActionRewardState
			Dim result As New StateActionRewardState(observation, action, reward, False)
			result.setNextObservation(nextObservation)

			Return result
		End Function

		Private Sub assertExpected(ByVal expected() As Double, ByVal actual As INDArray)
			Dim shape() As Long = actual.shape()
			assertEquals(2, shape.Length)
			assertEquals(1, shape(0))
			assertEquals(expected.Length, shape(1))
			For i As Integer = 0 To expected.Length - 1
				assertEquals(expected(i), actual.getDouble(0, i), 0.0001)
			Next i
		End Sub

		Private Sub assertExpected(ByVal expected()() As Double, ByVal actual As INDArray)
			Dim shape() As Long = actual.shape()
			assertEquals(2, shape.Length)
			assertEquals(expected.Length, shape(0))
			assertEquals(expected(0).Length, shape(1))

			For i As Integer = 0 To expected.Length - 1
				Dim expectedLine() As Double = expected(i)
				For j As Integer = 0 To expectedLine.Length - 1
					assertEquals(expectedLine(j), actual.getDouble(i, j), 0.0001)
				Next j
			Next i
		End Sub

		Private Sub assertExpected(ByVal expected()()() As Double, ByVal actual As INDArray)
			Dim shape() As Long = actual.shape()
			assertEquals(3, shape.Length)
			assertEquals(expected.Length, shape(0))
			assertEquals(expected(0).Length, shape(1))
			assertEquals(expected(0)(0).Length, shape(2))

			For i As Integer = 0 To expected.Length - 1
				Dim expected2D()() As Double = expected(i)
				For j As Integer = 0 To expected2D.Length - 1
					Dim expectedLine() As Double = expected2D(j)
					For k As Integer = 0 To expectedLine.Length - 1
						assertEquals(expectedLine(k), actual.getDouble(i, j, k), 0.0001)
					Next k
				Next j
			Next i

		End Sub
	End Class

End Namespace