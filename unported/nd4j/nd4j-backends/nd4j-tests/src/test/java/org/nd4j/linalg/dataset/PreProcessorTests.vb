Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports LabelLastTimeStepPreProcessor = org.nd4j.linalg.dataset.api.preprocessor.LabelLastTimeStepPreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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

Namespace org.nd4j.linalg.dataset

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class PreProcessorTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class PreProcessorTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLabelLastTimeStepPreProcessor(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLabelLastTimeStepPreProcessor(ByVal backend As Nd4jBackend)

			Dim f As INDArray = Nd4j.rand(DataType.FLOAT, 3, 5, 8)
			Dim l As INDArray = Nd4j.rand(DataType.FLOAT, 3, 4, 8)

			'First test: no mask
			Dim dsNoMask As New DataSet(f, l)

			Dim preProc As DataSetPreProcessor = New LabelLastTimeStepPreProcessor()
			preProc.preProcess(dsNoMask)

			assertSame(f, dsNoMask.Features) 'Should be exact same object (not modified)

			Dim l2d As INDArray = dsNoMask.Labels
			Dim l2dExp As INDArray = l.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.point(7))
			assertEquals(l2dExp, l2d)


			'Second test: mask, but only 1 value at last time step


			Dim lmSingle As INDArray = Nd4j.createFromArray(New Single()(){
				New Single() {0, 0, 0, 1, 0, 0, 0, 0},
				New Single() {0, 0, 0, 1, 0, 0, 1, 0},
				New Single() {0, 0, 0, 0, 0, 0, 0, 1}
			})

			Dim fm As INDArray = Nd4j.createFromArray(New Single()(){
				New Single() {1, 1, 1, 1, 0, 0, 0, 0},
				New Single() {1, 1, 1, 1, 1, 1, 1, 0},
				New Single() {1, 1, 1, 1, 1, 1, 1, 1}
			})

			Dim dsMask1 As New DataSet(f, l, fm, lmSingle)
			preProc.preProcess(dsMask1)

			Dim expL As INDArray = Nd4j.create(DataType.FLOAT, 3, 4)
			expL.putRow(0, l.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.point(3)))
			expL.putRow(1, l.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.point(6)))
			expL.putRow(2, l.get(NDArrayIndex.point(2), NDArrayIndex.all(), NDArrayIndex.point(7)))

			Dim exp1 As New DataSet(f, expL, fm, Nothing)
			assertEquals(exp1, dsMask1)

			'Third test: mask, but multiple values in label mask
			Dim lmMultiple As INDArray = Nd4j.createFromArray(New Single()(){
				New Single() {1, 1, 1, 1, 0, 0, 0, 0},
				New Single() {1, 1, 1, 1, 1, 1, 1, 0},
				New Single() {1, 1, 1, 1, 1, 1, 1, 1}
			})

			Dim dsMask2 As New DataSet(f, l, fm, lmMultiple)
			preProc.preProcess(dsMask2)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

	End Class

End Namespace