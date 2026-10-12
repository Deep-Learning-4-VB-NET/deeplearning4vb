Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.specials

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class CudaTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CudaTests
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
				Nd4j.DataType = DataType.FLOAT
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void setDown()
		Public Overridable Sub setDown()
				Nd4j.DataType = initialType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMGrid_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMGrid_1(ByVal backend As Nd4jBackend)
			If Not (TypeOf Nd4j.Executioner Is GridExecutioner) Then
				Return
			End If

			Dim arrayA As val = Nd4j.create(128, 128)
			Dim arrayB As val = Nd4j.create(128, 128)
			Dim arrayC As val = Nd4j.create(128, 128)

			arrayA.muli(arrayB)

			Dim executioner As val = DirectCast(Nd4j.Executioner, GridExecutioner)

			assertEquals(1, executioner.getQueueLength())

			arrayA.addi(arrayC)

			assertEquals(1, executioner.getQueueLength())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMGrid_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMGrid_2(ByVal backend As Nd4jBackend)
			If Not (TypeOf Nd4j.Executioner Is GridExecutioner) Then
				Return
			End If

			Dim exp As val = Nd4j.create(128, 128).assign(2.0)
			Nd4j.Executioner.commit()

			Dim arrayA As val = Nd4j.create(128, 128)
			Dim arrayB As val = Nd4j.create(128, 128)
			arrayA.muli(arrayB)

			Dim executioner As val = DirectCast(Nd4j.Executioner, GridExecutioner)

			assertEquals(1, executioner.getQueueLength())

			arrayA.addi(2.0f)

			assertEquals(0, executioner.getQueueLength())

			Nd4j.Executioner.commit()

			assertEquals(exp, arrayA)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace