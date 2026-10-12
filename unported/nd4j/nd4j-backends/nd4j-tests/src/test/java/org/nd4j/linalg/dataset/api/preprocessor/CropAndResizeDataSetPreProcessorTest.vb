Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class CropAndResizeDataSetPreProcessorTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CropAndResizeDataSetPreProcessorTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_originalHeightIsZero_expect_IllegalArgumentException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_originalHeightIsZero_expect_IllegalArgumentException(ByVal backend As Nd4jBackend)
		   assertThrows(GetType(System.ArgumentException),Sub()
		   Dim sut As New CropAndResizeDataSetPreProcessor(0, 15, 5, 5, 4, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
		   End Sub)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_originalWidthIsZero_expect_IllegalArgumentException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_originalWidthIsZero_expect_IllegalArgumentException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 0, 5, 5, 4, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			End Sub)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_yStartIsNegative_expect_IllegalArgumentException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_yStartIsNegative_expect_IllegalArgumentException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 15, -1, 5, 4, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			End Sub)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_xStartIsNegative_expect_IllegalArgumentException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_xStartIsNegative_expect_IllegalArgumentException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 15, 5, -1, 4, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			End Sub)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_heightIsNotGreaterThanZero_expect_IllegalArgumentException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_heightIsNotGreaterThanZero_expect_IllegalArgumentException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 15, 5, 5, 0, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			End Sub)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_widthIsNotGreaterThanZero_expect_IllegalArgumentException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_widthIsNotGreaterThanZero_expect_IllegalArgumentException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 15, 5, 5, 4, 0, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			End Sub)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_numChannelsIsNotGreaterThanZero_expect_IllegalArgumentException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_numChannelsIsNotGreaterThanZero_expect_IllegalArgumentException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 15, 5, 5, 4, 3, 0, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			End Sub)
		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsNull_expect_NullPointerException(org.nd4j.linalg.factory.Nd4jBackend backend)
		 Public Overridable Sub when_dataSetIsNull_expect_NullPointerException(ByVal backend As Nd4jBackend)
			' Assemble
			assertThrows(GetType(System.NullReferenceException),Sub()
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 15, 5, 5, 4, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			sut.preProcess(Nothing)
			End Sub)

		 End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsEmpty_expect_emptyDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIsEmpty_expect_emptyDataSet(ByVal backend As Nd4jBackend)
			' Assemble
			Dim sut As New CropAndResizeDataSetPreProcessor(10, 15, 5, 5, 4, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			Dim ds As New DataSet(Nothing, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			assertTrue(ds.Empty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIs15wx10h_expect_3wx4hDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIs15wx10h_expect_3wx4hDataSet(ByVal backend As Nd4jBackend)
			' Assemble
			Dim numChannels As Integer = 3
			Dim height As Integer = 10
			Dim width As Integer = 15
			Dim sut As New CropAndResizeDataSetPreProcessor(height, width, 5, 5, 4, 3, 3, CropAndResizeDataSetPreProcessor.ResizeMethod.NearestNeighbor)
			Dim input As INDArray = Nd4j.create(LongShapeDescriptor.fromShape(New Integer() { 1, height, width, numChannels }, DataType.FLOAT), True)
			For c As Integer = 0 To numChannels - 1
				For h As Integer = 0 To height - 1
					For w As Integer = 0 To width - 1
						input.putScalar(0, h, w, c, c*100 + h*10 + w)
					Next w
				Next h
			Next c

			Dim ds As New DataSet(input, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			Dim results As INDArray = ds.Features
			Dim shape() As Long = results.shape()
			assertArrayEquals(New Long(){1, 4, 3, 3}, shape)

			' Test a few values
			assertEquals(55.0, results.getDouble(0, 0, 0, 0), 0.0)
			assertEquals(155.0, results.getDouble(0, 0, 0, 1), 0.0)
			assertEquals(255.0, results.getDouble(0, 0, 0, 2), 0.0)

			assertEquals(56.0, results.getDouble(0, 0, 1, 0), 0.0)
			assertEquals(156.0, results.getDouble(0, 0, 1, 1), 0.0)
			assertEquals(256.0, results.getDouble(0, 0, 1, 2), 0.0)

			assertEquals(57.0, results.getDouble(0, 0, 2, 0), 0.0)
			assertEquals(157.0, results.getDouble(0, 0, 2, 1), 0.0)
			assertEquals(257.0, results.getDouble(0, 0, 2, 2), 0.0)

			assertEquals(65.0, results.getDouble(0, 1, 0, 0), 0.0)
			assertEquals(165.0, results.getDouble(0, 1, 0, 1), 0.0)
			assertEquals(265.0, results.getDouble(0, 1, 0, 2), 0.0)

			assertEquals(66.0, results.getDouble(0, 1, 1, 0), 0.0)
			assertEquals(166.0, results.getDouble(0, 1, 1, 1), 0.0)
			assertEquals(266.0, results.getDouble(0, 1, 1, 2), 0.0)

			assertEquals(75.0, results.getDouble(0, 2, 0, 0), 0.0)
			assertEquals(175.0, results.getDouble(0, 2, 0, 1), 0.0)
			assertEquals(275.0, results.getDouble(0, 2, 0, 2), 0.0)

			assertEquals(76.0, results.getDouble(0, 2, 1, 0), 0.0)
			assertEquals(176.0, results.getDouble(0, 2, 1, 1), 0.0)
			assertEquals(276.0, results.getDouble(0, 2, 1, 2), 0.0)
		End Sub

	End Class

End Namespace