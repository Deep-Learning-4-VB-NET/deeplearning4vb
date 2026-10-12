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

Namespace org.nd4j.linalg.compression

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.COMPRESSION) public class CompressionMagicTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CompressionMagicTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMagicDecompression1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMagicDecompression1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 100, 2500, DataType.FLOAT)

			Dim compressed As INDArray = Nd4j.Compressor.compress(array, "GZIP")

			assertTrue(compressed.Compressed)
			compressed.muli(1.0)

			assertFalse(compressed.Compressed)
			assertEquals(array, compressed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMagicDecompression4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMagicDecompression4(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 100, 2500, DataType.FLOAT)

			Dim compressed As INDArray = Nd4j.Compressor.compress(array, "GZIP")

			For cnt As Integer = 0 To array.length() - 1
				Dim a As Single = array.getFloat(cnt)
				Dim c As Single = compressed.getFloat(cnt)
				assertEquals(a, c, 0.01f)
			Next cnt

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupSkipDecompression1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupSkipDecompression1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 100, 2500, DataType.FLOAT)

			Dim compressed As INDArray = Nd4j.Compressor.compress(array, "GZIP")

			Dim newArray As INDArray = compressed.dup()
			assertTrue(newArray.Compressed)

			Nd4j.Compressor.decompressi(compressed)
			Nd4j.Compressor.decompressi(newArray)

			assertEquals(array, compressed)
			assertEquals(array, newArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupSkipDecompression2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupSkipDecompression2(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 100, 2500, DataType.FLOAT)

			Dim compressed As INDArray = Nd4j.Compressor.compress(array, "GZIP")

			Dim newArray As INDArray = compressed.dup("c"c)
			assertTrue(newArray.Compressed)

			Nd4j.Compressor.decompressi(compressed)
			Nd4j.Compressor.decompressi(newArray)

			assertEquals(array, compressed)
			assertEquals(array, newArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupSkipDecompression3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupSkipDecompression3(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 100, 2500, DataType.FLOAT)

			Dim compressed As INDArray = Nd4j.Compressor.compress(array, "GZIP")

			Dim newArray As INDArray = compressed.dup("f"c)
			assertFalse(newArray.Compressed)

			Nd4j.Compressor.decompressi(compressed)
			'        Nd4j.getCompressor().decompressi(newArray);

			assertEquals(array, compressed)
			assertEquals(array, newArray)
			assertEquals("f"c, newArray.ordering())
			assertEquals("c"c, compressed.ordering())
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace