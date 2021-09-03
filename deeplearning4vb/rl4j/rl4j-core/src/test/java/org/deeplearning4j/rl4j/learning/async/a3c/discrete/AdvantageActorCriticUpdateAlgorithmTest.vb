Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.learning.async
Imports org.deeplearning4j.rl4j.network
Imports org.deeplearning4j.rl4j.network.ac
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
Imports Mock = org.mockito.Mock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.mockito.ArgumentMatchers.any
import static org.mockito.Mockito.times
import static org.mockito.Mockito.verify
import static org.mockito.Mockito.when

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class AdvantageActorCriticUpdateAlgorithmTest
	Public Class AdvantageActorCriticUpdateAlgorithmTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock AsyncGlobal<org.deeplearning4j.rl4j.network.NeuralNet> mockAsyncGlobal;
		Friend mockAsyncGlobal As AsyncGlobal(Of NeuralNet)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IActorCritic mockActorCritic;
		Friend mockActorCritic As IActorCritic

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void refac_calcGradient_non_terminal()
		Public Overridable Sub refac_calcGradient_non_terminal()
			' Arrange
			Dim observationShape() As Integer = {5}
			Dim gamma As Double = 0.9
			Dim algorithm As New AdvantageActorCriticUpdateAlgorithm(False, observationShape, 1, gamma)

			Dim originalObservations() As INDArray = { Nd4j.create(New Double(){0.0, 0.1, 0.2, 0.3, 0.4}), Nd4j.create(New Double(){1.0, 1.1, 1.2, 1.3, 1.4}), Nd4j.create(New Double(){2.0, 2.1, 2.2, 2.3, 2.4}), Nd4j.create(New Double(){3.0, 3.1, 3.2, 3.3, 3.4})}

			Dim actions() As Integer = {0, 1, 2, 1}
			Dim rewards() As Double = {0.1, 1.0, 10.0, 100.0}

			Dim experience As IList(Of StateActionReward(Of Integer)) = New List(Of StateActionReward(Of Integer))()
			For i As Integer = 0 To originalObservations.Length - 1
				experience.Add(New StateActionReward(Of Integer)(New Observation(originalObservations(i)), actions(i), rewards(i), False))
			Next i

			[when](mockActorCritic.outputAll(any(GetType(INDArray)))).thenAnswer(Function(invocation)
			Dim batch As INDArray = invocation.getArgument(0)
			Return New INDArray(){batch.mul(-1.0)}
			End Function)

			Dim inputArgumentCaptor As ArgumentCaptor(Of INDArray) = ArgumentCaptor.forClass(GetType(INDArray))
			Dim criticActorArgumentCaptor As ArgumentCaptor(Of INDArray()) = ArgumentCaptor.forClass(GetType(INDArray()))

			' Act
			algorithm.computeGradients(mockActorCritic, experience)

			verify(mockActorCritic, times(1)).gradient(inputArgumentCaptor.capture(), criticActorArgumentCaptor.capture())

			assertEquals(Nd4j.stack(0, originalObservations), inputArgumentCaptor.getValue())

			'TODO: the actual AdvantageActorCritic Algo is not implemented correctly, so needs to be fixed, then we can test these
	'        assertEquals(Nd4j.zeros(1), criticActorArgumentCaptor.getValue()[0]);
	'        assertEquals(Nd4j.zeros(1), criticActorArgumentCaptor.getValue()[1]);

		End Sub

	End Class

End Namespace