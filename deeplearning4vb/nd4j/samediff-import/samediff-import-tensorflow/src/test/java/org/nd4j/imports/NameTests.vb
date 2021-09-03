﻿Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.imports

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NameTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NameTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNameExtraction_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNameExtraction_1(ByVal backend As Nd4jBackend)
			Dim str As val = "Name"
			Dim exp As val = "Name"

			Dim pair As val = SameDiff.parseVariable(str)
			assertEquals(exp, pair.getFirst())
			assertEquals(0, pair.getSecond().intValue())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNameExtraction_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNameExtraction_2(ByVal backend As Nd4jBackend)
			Dim str As val = "Name_2"
			Dim exp As val = "Name_2"

			Dim pair As val = SameDiff.parseVariable(str)
			assertEquals(exp, pair.getFirst())
			assertEquals(0, pair.getSecond().intValue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNameExtraction_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNameExtraction_3(ByVal backend As Nd4jBackend)
			Dim str As val = "Name_1:2"
			Dim exp As val = "Name_1"

			Dim pair As val = SameDiff.parseVariable(str)
			assertEquals(exp, pair.getFirst())
			assertEquals(2, pair.getSecond().intValue())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNameExtraction_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNameExtraction_4(ByVal backend As Nd4jBackend)
			Dim str As val = "Name_1:1:2"
			Dim exp As val = "Name_1:1"

			Dim pair As val = SameDiff.parseVariable(str)
			assertEquals(exp, pair.getFirst())
			assertEquals(2, pair.getSecond().intValue())
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace