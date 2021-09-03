Imports Tag = org.junit.jupiter.api.Tag
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Test = org.junit.jupiter.api.Test
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag public class PermuteDataSetPreProcessorTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class PermuteDataSetPreProcessorTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsNull_expect_NullPointerException(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIsNull_expect_NullPointerException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.NullReferenceException),Sub()
			Dim sut As New PermuteDataSetPreProcessor(PermuteDataSetPreProcessor.PermutationTypes.NCHWtoNHWC)
			sut.preProcess(Nothing)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_emptyDatasetInInputdataSetIsNCHW_expect_emptyDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_emptyDatasetInInputdataSetIsNCHW_expect_emptyDataSet(ByVal backend As Nd4jBackend)
			' Assemble
			Dim sut As New PermuteDataSetPreProcessor(PermuteDataSetPreProcessor.PermutationTypes.NCHWtoNHWC)
			Dim ds As New DataSet(Nothing, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			assertTrue(ds.Empty)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsNCHW_expect_dataSetTransformedToNHWC(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIsNCHW_expect_dataSetTransformedToNHWC(ByVal backend As Nd4jBackend)
			' Assemble
			Dim numChannels As Integer = 3
			Dim height As Integer = 5
			Dim width As Integer = 4
			Dim sut As New PermuteDataSetPreProcessor(PermuteDataSetPreProcessor.PermutationTypes.NCHWtoNHWC)
			Dim input As INDArray = Nd4j.create(1, numChannels, height, width)
			For c As Integer = 0 To numChannels - 1
				For h As Integer = 0 To height - 1
					For w As Integer = 0 To width - 1
						input.putScalar(0, c, h, w, c*100.0 + h*10.0 + w)
					Next w
				Next h
			Next c
			Dim ds As New DataSet(input, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			Dim result As INDArray = ds.Features
			Dim shape() As Long = result.shape()
			assertEquals(1, shape(0))
			assertEquals(height, shape(1))
			assertEquals(width, shape(2))
			assertEquals(numChannels, shape(3))

			assertEquals(0.0, result.getDouble(0, 0, 0, 0), 0.0)
			assertEquals(1.0, result.getDouble(0, 0, 1, 0), 0.0)
			assertEquals(2.0, result.getDouble(0, 0, 2, 0), 0.0)
			assertEquals(3.0, result.getDouble(0, 0, 3, 0), 0.0)

			assertEquals(110.0, result.getDouble(0, 1, 0, 1), 0.0)
			assertEquals(111.0, result.getDouble(0, 1, 1, 1), 0.0)
			assertEquals(112.0, result.getDouble(0, 1, 2, 1), 0.0)
			assertEquals(113.0, result.getDouble(0, 1, 3, 1), 0.0)

			assertEquals(210.0, result.getDouble(0, 1, 0, 2), 0.0)
			assertEquals(211.0, result.getDouble(0, 1, 1, 2), 0.0)
			assertEquals(212.0, result.getDouble(0, 1, 2, 2), 0.0)
			assertEquals(213.0, result.getDouble(0, 1, 3, 2), 0.0)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void when_dataSetIsNHWC_expect_dataSetTransformedToNCHW(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub when_dataSetIsNHWC_expect_dataSetTransformedToNCHW(ByVal backend As Nd4jBackend)
			' Assemble
			Dim numChannels As Integer = 3
			Dim height As Integer = 5
			Dim width As Integer = 4
			Dim sut As New PermuteDataSetPreProcessor(PermuteDataSetPreProcessor.PermutationTypes.NHWCtoNCHW)
			Dim input As INDArray = Nd4j.create(1, height, width, numChannels)
			For c As Integer = 0 To numChannels - 1
				For h As Integer = 0 To height - 1
					For w As Integer = 0 To width - 1
						input.putScalar(New Integer() { 0, h, w, c }, c*100.0 + h*10.0 + w)
					Next w
				Next h
			Next c
			Dim ds As New DataSet(input, Nothing)

			' Act
			sut.preProcess(ds)

			' Assert
			Dim result As INDArray = ds.Features
			Dim shape() As Long = result.shape()
			assertEquals(1, shape(0))
			assertEquals(numChannels, shape(1))
			assertEquals(height, shape(2))
			assertEquals(width, shape(3))

			assertEquals(0.0, result.getDouble(0, 0, 0, 0), 0.0)
			assertEquals(1.0, result.getDouble(0, 0, 0, 1), 0.0)
			assertEquals(2.0, result.getDouble(0, 0, 0, 2), 0.0)
			assertEquals(3.0, result.getDouble(0, 0, 0, 3), 0.0)

			assertEquals(110.0, result.getDouble(0, 1, 1, 0), 0.0)
			assertEquals(111.0, result.getDouble(0, 1, 1, 1), 0.0)
			assertEquals(112.0, result.getDouble(0, 1, 1, 2), 0.0)
			assertEquals(113.0, result.getDouble(0, 1, 1, 3), 0.0)

			assertEquals(210.0, result.getDouble(0, 2, 1, 0), 0.0)
			assertEquals(211.0, result.getDouble(0, 2, 1, 1), 0.0)
			assertEquals(212.0, result.getDouble(0, 2, 1, 2), 0.0)
			assertEquals(213.0, result.getDouble(0, 2, 1, 3), 0.0)

		End Sub
	End Class

End Namespace