Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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

Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.actorcritic

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class RecurrentActorCriticHelperTest
	Public Class RecurrentActorCriticHelperTest

		Private ReadOnly sut As New RecurrentActorCriticHelper(3)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCreateValueLabels_expect_INDArrayWithCorrectShape()
		Public Overridable Sub when_callingCreateValueLabels_expect_INDArrayWithCorrectShape()
			' Arrange

			' Act
			Dim result As INDArray = sut.createValueLabels(4)

			' Assert
			assertArrayEquals(New Long() { 1, 1, 4 }, result.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCreatePolicyLabels_expect_ZeroINDArrayWithCorrectShape()
		Public Overridable Sub when_callingCreatePolicyLabels_expect_ZeroINDArrayWithCorrectShape()
			' Arrange

			' Act
			Dim result As INDArray = sut.createPolicyLabels(4)

			' Assert
			assertArrayEquals(New Long() { 1, 3, 4 }, result.shape())
			For j As Integer = 0 To 2
				For i As Integer = 0 To 3
					assertEquals(0.0, result.getDouble(0, j, i), 0.00001)
				Next i
			Next j
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingSetPolicy_expect_advantageSetAtCorrectLocation()
		Public Overridable Sub when_callingSetPolicy_expect_advantageSetAtCorrectLocation()
			' Arrange
			Dim policyArray As INDArray = Nd4j.zeros(1, 3, 3)

			' Act
			sut.setPolicy(policyArray, 1, 2, 123.0)

			' Assert
			For j As Integer = 0 To 2
				For i As Integer = 0 To 2
					If j = 2 AndAlso i = 1 Then
						assertEquals(123.0, policyArray.getDouble(0, j, i), 0.00001)
					Else
						assertEquals(0.0, policyArray.getDouble(0, j, i), 0.00001)
					End If
				Next i
			Next j
		End Sub

	End Class

End Namespace