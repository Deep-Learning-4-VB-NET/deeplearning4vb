Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.nd4j.linalg.dataset.api.preprocessor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class RGBtoGrayscaleDataSetPreProcessorTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class RGBtoGrayscaleDataSetPreProcessorTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsNull_expect_NullPointerException(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIsNull_expect_NullPointerException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.NullReferenceException),Sub()
			Dim sut As New RGBtoGrayscaleDataSetPreProcessor()
			sut.preProcess(Nothing)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsEmpty_expect_EmptyDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIsEmpty_expect_EmptyDataSet(ByVal backend As Nd4jBackend)
			' Assemble
			Dim sut As New RGBtoGrayscaleDataSetPreProcessor()
			Dim ds As New DataSet(Nothing, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			assertTrue(ds.Empty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_colorsAreConverted_expect_grayScaleResult(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_colorsAreConverted_expect_grayScaleResult(ByVal backend As Nd4jBackend)
			' Assign
			Dim numChannels As Integer = 3
			Dim height As Integer = 1
			Dim width As Integer = 5

			Dim sut As New RGBtoGrayscaleDataSetPreProcessor()
			Dim input As INDArray = Nd4j.create(2, numChannels, height, width)

			' Black, Example 1
			input.putScalar(0, 0, 0, 0, 0.0)
			input.putScalar(0, 1, 0, 0, 0.0)
			input.putScalar(0, 2, 0, 0, 0.0)

			' White, Example 1
			input.putScalar(0, 0, 0, 1, 255.0)
			input.putScalar(0, 1, 0, 1, 255.0)
			input.putScalar(0, 2, 0, 1, 255.0)

			' Red, Example 1
			input.putScalar(0, 0, 0, 2, 255.0)
			input.putScalar(0, 1, 0, 2, 0.0)
			input.putScalar(0, 2, 0, 2, 0.0)

			' Green, Example 1
			input.putScalar(0, 0, 0, 3, 0.0)
			input.putScalar(0, 1, 0, 3, 255.0)
			input.putScalar(0, 2, 0, 3, 0.0)

			' Blue, Example 1
			input.putScalar(0, 0, 0, 4, 0.0)
			input.putScalar(0, 1, 0, 4, 0.0)
			input.putScalar(0, 2, 0, 4, 255.0)


			' Black, Example 2
			input.putScalar(1, 0, 0, 4, 0.0)
			input.putScalar(1, 1, 0, 4, 0.0)
			input.putScalar(1, 2, 0, 4, 0.0)

			' White, Example 2
			input.putScalar(1, 0, 0, 3, 255.0)
			input.putScalar(1, 1, 0, 3, 255.0)
			input.putScalar(1, 2, 0, 3, 255.0)

			' Red, Example 2
			input.putScalar(1, 0, 0, 2, 255.0)
			input.putScalar(1, 1, 0, 2, 0.0)
			input.putScalar(1, 2, 0, 2, 0.0)

			' Green, Example 2
			input.putScalar(1, 0, 0, 1, 0.0)
			input.putScalar(1, 1, 0, 1, 255.0)
			input.putScalar(1, 2, 0, 1, 0.0)

			' Blue, Example 2
			input.putScalar(1, 0, 0, 0, 0.0)
			input.putScalar(1, 1, 0, 0, 0.0)
			input.putScalar(1, 2, 0, 0, 255.0)

			Dim ds As New DataSet(input, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			Dim result As INDArray = ds.Features
			Dim shape() As Long = result.shape()

			assertEquals(3, shape.Length)
			assertEquals(2, shape(0))
			assertEquals(1, shape(1))
			assertEquals(5, shape(2))

			assertEquals(0.0, result.getDouble(0, 0, 0), 0.05)
			assertEquals(255.0, result.getDouble(0, 0, 1), 0.05)
			assertEquals(255.0 * 0.3, result.getDouble(0, 0, 2), 0.05)
			assertEquals(255.0 * 0.59, result.getDouble(0, 0, 3), 0.05)
			assertEquals(255.0 * 0.11, result.getDouble(0, 0, 4), 0.05)

			assertEquals(0.0, result.getDouble(1, 0, 4), 0.05)
			assertEquals(255.0, result.getDouble(1, 0, 3), 0.05)
			assertEquals(255.0 * 0.3, result.getDouble(1, 0, 2), 0.05)
			assertEquals(255.0 * 0.59, result.getDouble(1, 0, 1), 0.05)
			assertEquals(255.0 * 0.11, result.getDouble(1, 0, 0), 0.05)

		End Sub
	End Class

End Namespace