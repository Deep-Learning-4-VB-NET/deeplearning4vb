Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.nd4j.linalg.lapack

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class LapackTestsF extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LapackTestsF
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			Nd4j.DataType = DataType.DOUBLE
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			Nd4j.DataType = initialType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRF1DifferentOrders(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testGetRF1DifferentOrders(ByVal backend As Nd4jBackend)
			Dim a As INDArray = Nd4j.create(New Double() {1, 2, 3, 4, 5, 6, 7, 8, 9}, New Integer() {3, 3}, "c"c).dup("f"c)
			Dim exp As INDArray = Nd4j.create(New Double() {7.0, 8.0, 9.0, 0.14285715, 0.85714287, 1.7142857, 0.5714286, 0.5, 0.0}, New Integer() {3, 3}, "c"c).dup("f"c)

			Dim r As INDArray = Nd4j.NDArrayFactory.lapack().getrf(a)

			assertEquals(exp, a)
		  End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace