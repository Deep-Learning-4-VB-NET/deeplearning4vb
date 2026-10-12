Imports System
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
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

Namespace org.nd4j.linalg

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class MmulBug extends BaseNd4jTestWithBackends
	Public Class MmulBug
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void simpleTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub simpleTest(ByVal backend As Nd4jBackend)
			Dim m1 As INDArray = Nd4j.create(New Double()() {
				New Double() {1.0},
				New Double() {2.0},
				New Double() {3.0},
				New Double() {4.0}
			})

			m1 = m1.reshape(ChrW(2), 2)

			Dim m2 As INDArray = Nd4j.create(New Double()() {
				New Double() {1.0, 2.0, 3.0, 4.0}
			})
			m2 = m2.reshape(ChrW(2), 2)
			m2.Order = "f"c

			'mmul gives the correct result
			Dim correctResult As INDArray
			correctResult = m1.mmul(m2)
			Console.WriteLine("================")
			Console.WriteLine(m1)
			Console.WriteLine(m2)
			Console.WriteLine(correctResult)
			Console.WriteLine("================")
			Dim newResult As INDArray = Nd4j.create(DataType.DOUBLE, correctResult.shape(), "c"c)
			m1.mmul(m2, newResult)
			assertEquals(correctResult, newResult)

			'But not so mmuli (which is somewhat mixed)
			Dim target As INDArray = Nd4j.linspace(1, 4, 4).reshape(ChrW(2), 2)
			target = m1.mmuli(m2, m1)
			assertEquals(True, target.Equals(correctResult))
			assertEquals(True, m1.Equals(correctResult))
		End Sub
	End Class

End Namespace