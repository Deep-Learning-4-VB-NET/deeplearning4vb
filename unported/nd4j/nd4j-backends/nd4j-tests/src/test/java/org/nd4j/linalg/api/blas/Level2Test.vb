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
'ORIGINAL LINE: @NativeTag public class Level2Test extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class Level2Test
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemv1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemv1(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape(ChrW(10), 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape(ChrW(100), 1)

			Dim array3 As INDArray = array1.mmul(array2)

			assertEquals(10, array3.length())
			assertEquals(338350f, array3.getFloat(0), 0.001f)
			assertEquals(843350f, array3.getFloat(1), 0.001f)
			assertEquals(1348350f, array3.getFloat(2), 0.001f)
			assertEquals(1853350f, array3.getFloat(3), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemv2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemv2(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape(ChrW(10), 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape("f"c, 100, 1)

			Dim array3 As INDArray = array1.mmul(array2)

			assertEquals(10, array3.length())
			assertEquals(338350f, array3.getFloat(0), 0.001f)
			assertEquals(843350f, array3.getFloat(1), 0.001f)
			assertEquals(1348350f, array3.getFloat(2), 0.001f)
			assertEquals(1853350f, array3.getFloat(3), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemv3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemv3(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 10, 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape("f"c, 100, 1)

			Dim array3 As INDArray = array1.mmul(array2)

			assertEquals(10, array3.length())
			assertEquals(3338050f, array3.getFloat(0), 0.001f)
			assertEquals(3343100f, array3.getFloat(1), 0.001f)
			assertEquals(3348150f, array3.getFloat(2), 0.001f)
			assertEquals(3353200f, array3.getFloat(3), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemv4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemv4(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 10, 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape(ChrW(100), 1)

			Dim array3 As INDArray = array1.mmul(array2)

			assertEquals(10, array3.length())
			assertEquals(3338050f, array3.getFloat(0), 0.001f)
			assertEquals(3343100f, array3.getFloat(1), 0.001f)
			assertEquals(3348150f, array3.getFloat(2), 0.001f)
			assertEquals(3353200f, array3.getFloat(3), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemv5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemv5(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape(ChrW(10), 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape(ChrW(100), 1)

			Dim array3 As INDArray = Nd4j.create(10,1)

			array1.mmul(array2, array3)

			assertEquals(10, array3.length())
			assertEquals(338350f, array3.getFloat(0), 0.001f)
			assertEquals(843350f, array3.getFloat(1), 0.001f)
			assertEquals(1348350f, array3.getFloat(2), 0.001f)
			assertEquals(1853350f, array3.getFloat(3), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemv6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemv6(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 10, 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape(ChrW(100), 1)

			Dim array3 As INDArray = Nd4j.create(10,1)

			array1.mmul(array2, array3)

			assertEquals(10, array3.length())
			assertEquals(3338050f, array3.getFloat(0), 0.001f)
			assertEquals(3343100f, array3.getFloat(1), 0.001f)
			assertEquals(3348150f, array3.getFloat(2), 0.001f)
			assertEquals(3353200f, array3.getFloat(3), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGemv7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGemv7(ByVal backend As Nd4jBackend)
			Dim array1 As INDArray = Nd4j.linspace(1, 1000, 1000).reshape("f"c, 10, 100)
			Dim array2 As INDArray = Nd4j.linspace(1, 100, 100).reshape(ChrW(100), 1)

			Dim array3 As INDArray = Nd4j.create(10,1)

			array1.mmul(array2, array3)

			assertEquals(10, array3.length())
			assertEquals(3338050f, array3.getFloat(0), 0.001f)
			assertEquals(3343100f, array3.getFloat(1), 0.001f)
			assertEquals(3348150f, array3.getFloat(2), 0.001f)
			assertEquals(3353200f, array3.getFloat(3), 0.001f)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace