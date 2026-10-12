Imports FilterOperation = org.deeplearning4j.rl4j.observation.transform.FilterOperation
Imports Test = org.junit.jupiter.api.Test
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.rl4j.observation.transform.filter

	Public Class UniformSkippingFilterTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_negativeSkipFrame_expect_exception()
		Public Overridable Sub when_negativeSkipFrame_expect_exception()
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim tempVar As New UniformSkippingFilter(-1)
		   End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_skippingIs4_expect_firstNotSkippedOther3Skipped()
		Public Overridable Sub when_skippingIs4_expect_firstNotSkippedOther3Skipped()
			' Assemble
			Dim sut As FilterOperation = New UniformSkippingFilter(4)
			Dim isSkipped(7) As Boolean

			' Act
			For i As Integer = 0 To 7
				isSkipped(i) = sut.isSkipped(Nothing, i, False)
			Next i

			' Assert
			assertFalse(isSkipped(0))
			assertTrue(isSkipped(1))
			assertTrue(isSkipped(2))
			assertTrue(isSkipped(3))

			assertFalse(isSkipped(4))
			assertTrue(isSkipped(5))
			assertTrue(isSkipped(6))
			assertTrue(isSkipped(7))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_isLastObservation_expect_observationNotSkipped()
		Public Overridable Sub when_isLastObservation_expect_observationNotSkipped()
			' Assemble
			Dim sut As FilterOperation = New UniformSkippingFilter(4)

			' Act
			Dim isSkippedNotLastObservation As Boolean = sut.isSkipped(Nothing, 1, False)
			Dim isSkippedLastObservation As Boolean = sut.isSkipped(Nothing, 1, True)

			' Assert
			assertTrue(isSkippedNotLastObservation)
			assertFalse(isSkippedLastObservation)
		End Sub

	End Class

End Namespace