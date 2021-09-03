Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
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

Namespace org.nd4j.linalg.api.blas

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class Level3Test extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class Level3Test
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm1(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 100, 100).reshape(ChrW(1), 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape(ChrW(100), 1)

			Dim array3 As INDArray = array1.mmul(array2)

			assertEquals(338350f, array3.getFloat(0), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm2(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 100, 100).reshape("f"c, 1, 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape("f"c, 100, 1)

			Dim array3 As INDArray = array1.mmul(array2)

			assertEquals(338350f, array3.getFloat(0), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm3(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape(ChrW(10), 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape(ChrW(100), 10)

			Dim array3 As INDArray = array1.mmul(array2, Nd4j.createUninitialized(New Long(){10, 10}, "f"c))


			'System.out.println("Array3: " + Arrays.toString(array3.data().asFloat()));

			assertEquals(3338050.0f, array3.data().getFloat(0), 0.001f)
			assertEquals(8298050.0f, array3.data().getFloat(1), 0.001f)
			assertEquals(3343100.0f, array3.data().getFloat(10), 0.001f)
			assertEquals(8313100.0f, array3.data().getFloat(11), 0.001f)
			assertEquals(3348150.0f, array3.data().getFloat(20), 0.001f)
			assertEquals(8328150.0f, array3.data().getFloat(21), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm4(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape(ChrW(10), 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 100, 10)

			Dim array3 As INDArray = array1.mmul(array2)

			'System.out.println("Array3: " + Arrays.toString(array3.data().asFloat()));

			assertEquals(338350f, array3.data().getFloat(0), 0.001f)
			assertEquals(843350f, array3.data().getFloat(1), 0.001f)
			assertEquals(843350f, array3.data().getFloat(10), 0.001f)
			assertEquals(2348350f, array3.data().getFloat(11), 0.001f)
			assertEquals(1348350f, array3.data().getFloat(20), 0.001f)
			assertEquals(3853350f, array3.data().getFloat(21), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm5(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 10, 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape(ChrW(100), 10)

			Dim array3 As INDArray = array1.mmul(array2)

			'System.out.println("Array3: " + Arrays.toString(array3.data().asFloat()));

			'assertEquals(3.29341E7f, array3.data().getFloat(0),10f);
			assertEquals(3.29837E7f, array3.data().getFloat(1), 10f)
			assertEquals(3.3835E7f, array3.data().getFloat(99), 10f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemm6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemm6(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 10, 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 100, 10)

			Dim array3 As INDArray = array1.mmul(array2)

			'System.out.println("Array3: " + Arrays.toString(array3.data().asFloat()));

			assertEquals(3338050.0f, array3.data().getFloat(0), 0.001f)
			assertEquals(3343100f, array3.data().getFloat(1), 0.001f)
			assertEquals(8298050f, array3.data().getFloat(10), 0.001f)
			assertEquals(8313100.0f, array3.data().getFloat(11), 0.001f)
			assertEquals(1.325805E7f, array3.data().getFloat(20), 5f)
			assertEquals(1.32831E7f, array3.data().getFloat(21), 5f)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace