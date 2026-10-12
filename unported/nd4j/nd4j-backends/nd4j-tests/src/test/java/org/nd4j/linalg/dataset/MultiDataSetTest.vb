Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.interval

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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class MultiDataSetTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class MultiDataSetTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMerging2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMerging2d(ByVal backend As Nd4jBackend)
			'Simple test: single input/output arrays; 5 MultiDataSets to merge
			Dim nCols As Integer = 3
			Dim nRows As Integer = 5
			Dim expIn As INDArray = Nd4j.linspace(0, nCols * nRows - 1, nCols * nRows, DataType.DOUBLE).reshape(ChrW(nRows), nCols)
			Dim expOut As INDArray = Nd4j.linspace(100, 100 + nCols * nRows - 1, nCols * nRows, DataType.DOUBLE).reshape(ChrW(nRows), nCols)

			Dim [in](nRows - 1) As INDArray
			Dim [out](nRows - 1) As INDArray
			For i As Integer = 0 To nRows - 1
				[in](i) = expIn.getRow(i, True).dup()
			Next i
			For i As Integer = 0 To nRows - 1
				[out](i) = expOut.getRow(i, True).dup()
			Next i

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				list.Add(New MultiDataSet([in](i), [out](i)))
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)
			assertEquals(1, merged.Features.Length)
			assertEquals(1, merged.Labels.Length)

			assertEquals(expIn, merged.getFeatures(0))
			assertEquals(expOut, merged.getLabels(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMerging2dMultipleInOut(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMerging2dMultipleInOut(ByVal backend As Nd4jBackend)
			'Test merging: Multiple input/output arrays; 5 MultiDataSets to merge

			Dim nRows As Integer = 5
			Dim nColsIn0 As Integer = 3
			Dim nColsIn1 As Integer = 4
			Dim nColsOut0 As Integer = 5
			Dim nColsOut1 As Integer = 6

			Dim expIn0 As INDArray = Nd4j.linspace(0, nRows * nColsIn0 - 1, nRows * nColsIn0, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn0)
			Dim expIn1 As INDArray = Nd4j.linspace(0, nRows * nColsIn1 - 1, nRows * nColsIn1, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn1)
			Dim expOut0 As INDArray = Nd4j.linspace(0, nRows * nColsOut0 - 1, nRows * nColsOut0, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut0)
			Dim expOut1 As INDArray = Nd4j.linspace(0, nRows * nColsOut1 - 1, nRows * nColsOut1, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut1)

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				If i = 0 Then
					'For first MultiDataSet: have 2 rows, not just 1
					Dim in0 As INDArray = expIn0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim in1 As INDArray = expIn1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim out0 As INDArray = expOut0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim out1 As INDArray = expOut1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}))
					i += 1
				Else
					Dim in0 As INDArray = expIn0.getRow(i, True).dup()
					Dim in1 As INDArray = expIn1.getRow(i, True).dup()
					Dim out0 As INDArray = expOut0.getRow(i, True).dup()
					Dim out1 As INDArray = expOut1.getRow(i, True).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}))
				End If
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)
			assertEquals(2, merged.Features.Length)
			assertEquals(2, merged.Labels.Length)

			assertEquals(expIn0, merged.getFeatures(0))
			assertEquals(expIn1, merged.getFeatures(1))
			assertEquals(expOut0, merged.getLabels(0))
			assertEquals(expOut1, merged.getLabels(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMerging2dMultipleInOut2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMerging2dMultipleInOut2(ByVal backend As Nd4jBackend)
			'Test merging: Multiple input/output arrays; 5 MultiDataSets to merge

			Dim nRows As Integer = 10
			Dim nColsIn0 As Integer = 3
			Dim nColsIn1 As Integer = 4
			Dim nColsIn2 As Integer = 5
			Dim nColsOut0 As Integer = 6
			Dim nColsOut1 As Integer = 7
			Dim nColsOut2 As Integer = 8

			Dim expIn0 As INDArray = Nd4j.linspace(0, nRows * nColsIn0 - 1, nRows * nColsIn0, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn0)
			Dim expIn1 As INDArray = Nd4j.linspace(0, nRows * nColsIn1 - 1, nRows * nColsIn1, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn1)
			Dim expIn2 As INDArray = Nd4j.linspace(0, nRows * nColsIn2 - 1, nRows * nColsIn2, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn2)
			Dim expOut0 As INDArray = Nd4j.linspace(0, nRows * nColsOut0 - 1, nRows * nColsOut0, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut0)
			Dim expOut1 As INDArray = Nd4j.linspace(0, nRows * nColsOut1 - 1, nRows * nColsOut1, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut1)
			Dim expOut2 As INDArray = Nd4j.linspace(0, nRows * nColsOut2 - 1, nRows * nColsOut2, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut2)

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				If i = 0 Then
					'For first MultiDataSet: have 2 rows, not just 1
					Dim in0 As INDArray = expIn0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim in1 As INDArray = expIn1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim in2 As INDArray = expIn2.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim out0 As INDArray = expOut0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim out1 As INDArray = expOut1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim out2 As INDArray = expOut2.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1, in2}, New INDArray() {out0, out1, out2}))
					i += 1
				Else
					Dim in0 As INDArray = expIn0.getRow(i, True).dup()
					Dim in1 As INDArray = expIn1.getRow(i, True).dup()
					Dim in2 As INDArray = expIn2.getRow(i, True).dup()
					Dim out0 As INDArray = expOut0.getRow(i, True).dup()
					Dim out1 As INDArray = expOut1.getRow(i, True).dup()
					Dim out2 As INDArray = expOut2.getRow(i, True).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1, in2}, New INDArray() {out0, out1, out2}))
				End If
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)
			assertEquals(3, merged.Features.Length)
			assertEquals(3, merged.Labels.Length)

			assertEquals(expIn0, merged.getFeatures(0))
			assertEquals(expIn1, merged.getFeatures(1))
			assertEquals(expIn2, merged.getFeatures(2))
			assertEquals(expOut0, merged.getLabels(0))
			assertEquals(expOut1, merged.getLabels(1))
			assertEquals(expOut2, merged.getLabels(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMerging2dMultipleInOut3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMerging2dMultipleInOut3(ByVal backend As Nd4jBackend)
			'Test merging: fewer rows than output arrays...

			Dim nRows As Integer = 2
			Dim nColsIn0 As Integer = 3
			Dim nColsIn1 As Integer = 4
			Dim nColsIn2 As Integer = 5
			Dim nColsOut0 As Integer = 6
			Dim nColsOut1 As Integer = 7
			Dim nColsOut2 As Integer = 8

			Dim expIn0 As INDArray = Nd4j.linspace(0, nRows * nColsIn0 - 1, nRows * nColsIn0, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn0)
			Dim expIn1 As INDArray = Nd4j.linspace(0, nRows * nColsIn1 - 1, nRows * nColsIn1, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn1)
			Dim expIn2 As INDArray = Nd4j.linspace(0, nRows * nColsIn2 - 1, nRows * nColsIn2, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn2)
			Dim expOut0 As INDArray = Nd4j.linspace(0, nRows * nColsOut0 - 1, nRows * nColsOut0, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut0)
			Dim expOut1 As INDArray = Nd4j.linspace(0, nRows * nColsOut1 - 1, nRows * nColsOut1, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut1)
			Dim expOut2 As INDArray = Nd4j.linspace(0, nRows * nColsOut2 - 1, nRows * nColsOut2, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut2)

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				Dim in0 As INDArray = expIn0.getRow(i, True).dup()
				Dim in1 As INDArray = expIn1.getRow(i, True).dup()
				Dim in2 As INDArray = expIn2.getRow(i, True).dup()
				Dim out0 As INDArray = expOut0.getRow(i, True).dup()
				Dim out1 As INDArray = expOut1.getRow(i, True).dup()
				Dim out2 As INDArray = expOut2.getRow(i, True).dup()
				list.Add(New MultiDataSet(New INDArray() {in0, in1, in2}, New INDArray() {out0, out1, out2}))
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)
			assertEquals(3, merged.Features.Length)
			assertEquals(3, merged.Labels.Length)

			assertEquals(expIn0, merged.getFeatures(0))
			assertEquals(expIn1, merged.getFeatures(1))
			assertEquals(expIn2, merged.getFeatures(2))
			assertEquals(expOut0, merged.getLabels(0))
			assertEquals(expOut1, merged.getLabels(1))
			assertEquals(expOut2, merged.getLabels(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMerging4dMultipleInOut(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMerging4dMultipleInOut(ByVal backend As Nd4jBackend)
			Dim nRows As Integer = 5
			Dim depthIn0 As Integer = 3
			Dim widthIn0 As Integer = 4
			Dim heightIn0 As Integer = 5

			Dim depthIn1 As Integer = 4
			Dim widthIn1 As Integer = 5
			Dim heightIn1 As Integer = 6

			Dim nColsOut0 As Integer = 5
			Dim nColsOut1 As Integer = 6

			Dim lengthIn0 As Integer = nRows * depthIn0 * widthIn0 * heightIn0
			Dim lengthIn1 As Integer = nRows * depthIn1 * widthIn1 * heightIn1
			Dim expIn0 As INDArray = Nd4j.linspace(0, lengthIn0 - 1, lengthIn0, DataType.DOUBLE).reshape(ChrW(nRows), depthIn0, widthIn0, heightIn0)
			Dim expIn1 As INDArray = Nd4j.linspace(0, lengthIn1 - 1, lengthIn1, DataType.DOUBLE).reshape(ChrW(nRows), depthIn1, widthIn1, heightIn1)
			Dim expOut0 As INDArray = Nd4j.linspace(0, nRows * nColsOut0 - 1, nRows * nColsOut0, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut0)
			Dim expOut1 As INDArray = Nd4j.linspace(0, nRows * nColsOut1 - 1, nRows * nColsOut1, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut1)

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				If i = 0 Then
					'For first MultiDataSet: have 2 rows, not just 1
					Dim in0 As INDArray = expIn0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim in1 As INDArray = expIn1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim out0 As INDArray = expOut0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					Dim out1 As INDArray = expOut1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all()).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}))
					i += 1
				Else
					Dim in0 As INDArray = expIn0.get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim in1 As INDArray = expIn1.get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim out0 As INDArray = expOut0.getRow(i, True).dup()
					Dim out1 As INDArray = expOut1.getRow(i, True).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}))
				End If
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)
			assertEquals(2, merged.Features.Length)
			assertEquals(2, merged.Labels.Length)

			assertEquals(expIn0, merged.getFeatures(0))
			assertEquals(expIn1, merged.getFeatures(1))
			assertEquals(expOut0, merged.getLabels(0))
			assertEquals(expOut1, merged.getLabels(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergingTimeSeriesEqualLength(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergingTimeSeriesEqualLength(ByVal backend As Nd4jBackend)
			Dim tsLength As Integer = 8
			Dim nRows As Integer = 5
			Dim nColsIn0 As Integer = 3
			Dim nColsIn1 As Integer = 4
			Dim nColsOut0 As Integer = 5
			Dim nColsOut1 As Integer = 6

			Dim n0 As Integer = nRows * nColsIn0 * tsLength
			Dim n1 As Integer = nRows * nColsIn1 * tsLength
			Dim nOut0 As Integer = nRows * nColsOut0 * tsLength
			Dim nOut1 As Integer = nRows * nColsOut1 * tsLength
			Dim expIn0 As INDArray = Nd4j.linspace(0, n0 - 1, n0, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn0, tsLength)
			Dim expIn1 As INDArray = Nd4j.linspace(0, n1 - 1, n1, DataType.DOUBLE).reshape(ChrW(nRows), nColsIn1, tsLength)
			Dim expOut0 As INDArray = Nd4j.linspace(0, nOut0 - 1, nOut0, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut0, tsLength)
			Dim expOut1 As INDArray = Nd4j.linspace(0, nOut1 - 1, nOut1, DataType.DOUBLE).reshape(ChrW(nRows), nColsOut1, tsLength)

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				If i = 0 Then
					'For first MultiDataSet: have 2 rows, not just 1
					Dim in0 As INDArray = expIn0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim in1 As INDArray = expIn1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim out0 As INDArray = expOut0.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim out1 As INDArray = expOut1.get(NDArrayIndex.interval(0, 1, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}))
					i += 1
				Else
					Dim in0 As INDArray = expIn0.get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim in1 As INDArray = expIn1.get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim out0 As INDArray = expOut0.get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					Dim out1 As INDArray = expOut1.get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
					list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}))
				End If
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)
			assertEquals(2, merged.Features.Length)
			assertEquals(2, merged.Labels.Length)

			assertEquals(expIn0, merged.getFeatures(0))
			assertEquals(expIn1, merged.getFeatures(1))
			assertEquals(expOut0, merged.getLabels(0))
			assertEquals(expOut1, merged.getLabels(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergingTimeSeriesWithMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergingTimeSeriesWithMasking(ByVal backend As Nd4jBackend)
			'Mask arrays, and different lengths

			Dim tsLengthIn0 As Integer = 8
			Dim tsLengthIn1 As Integer = 9
			Dim tsLengthOut0 As Integer = 10
			Dim tsLengthOut1 As Integer = 11

			Dim nRows As Integer = 5
			Dim nColsIn0 As Integer = 3
			Dim nColsIn1 As Integer = 4
			Dim nColsOut0 As Integer = 5
			Dim nColsOut1 As Integer = 6

			Dim expectedIn0 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, nColsIn0, tsLengthIn0)
			Dim expectedIn1 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, nColsIn1, tsLengthIn1)
			Dim expectedOut0 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, nColsOut0, tsLengthOut0)
			Dim expectedOut1 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, nColsOut1, tsLengthOut1)

			Dim expectedMaskIn0 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, tsLengthIn0)
			Dim expectedMaskIn1 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, tsLengthIn1)
			Dim expectedMaskOut0 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, tsLengthOut0)
			Dim expectedMaskOut1 As INDArray = Nd4j.create(DataType.DOUBLE, nRows, tsLengthOut1)


			Dim r As New Random(12345)
			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				Dim thisRowIn0Length As Integer = tsLengthIn0 - i
				Dim thisRowIn1Length As Integer = tsLengthIn1 - i
				Dim thisRowOut0Length As Integer = tsLengthOut0 - i
				Dim thisRowOut1Length As Integer = tsLengthOut1 - i

				Dim in0NumElem As Integer = thisRowIn0Length * nColsIn0
				Dim in0 As INDArray = Nd4j.linspace(0, in0NumElem - 1, in0NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsIn0, thisRowIn0Length)

				Dim in1NumElem As Integer = thisRowIn1Length * nColsIn1
				Dim in1 As INDArray = Nd4j.linspace(0, in1NumElem - 1, in1NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsIn1, thisRowIn1Length)

				Dim out0NumElem As Integer = thisRowOut0Length * nColsOut0
				Dim out0 As INDArray = Nd4j.linspace(0, out0NumElem - 1, out0NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsOut0, thisRowOut0Length)

				Dim out1NumElem As Integer = thisRowOut1Length * nColsOut1
				Dim out1 As INDArray = Nd4j.linspace(0, out1NumElem - 1, out1NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsOut1, thisRowOut1Length)

				Dim maskIn0 As INDArray = Nothing
				Dim maskIn1 As INDArray = Nd4j.zeros(1, thisRowIn1Length)
				For j As Integer = 0 To thisRowIn1Length - 1
					If r.nextBoolean() Then
						maskIn1.putScalar(j, 1.0)
					End If
				Next j
				Dim maskOut0 As INDArray = Nothing
				Dim maskOut1 As INDArray = Nd4j.zeros(1, thisRowOut1Length)
				For j As Integer = 0 To thisRowOut1Length - 1
					If r.nextBoolean() Then
						maskOut1.putScalar(j, 1.0)
					End If
				Next j

				expectedIn0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowIn0Length)}, in0)
				expectedIn1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowIn1Length)}, in1)
				expectedOut0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowOut0Length)}, out0)
				expectedOut1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowOut1Length)}, out1)

				expectedMaskIn0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowIn0Length)}, Nd4j.ones(1, thisRowIn0Length))
				expectedMaskIn1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowIn1Length)}, maskIn1)
				expectedMaskOut0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowOut0Length)}, Nd4j.ones(1, thisRowOut0Length))
				expectedMaskOut1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowOut1Length)}, maskOut1)

				list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}, New INDArray() {maskIn0, maskIn1}, New INDArray() {maskOut0, maskOut1}))
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)

			assertEquals(2, merged.Features.Length)
			assertEquals(2, merged.Labels.Length)
			assertEquals(2, merged.FeaturesMaskArrays.Length)
			assertEquals(2, merged.LabelsMaskArrays.Length)

			assertEquals(expectedIn0, merged.getFeatures(0))
			assertEquals(expectedIn1, merged.getFeatures(1))
			assertEquals(expectedOut0, merged.getLabels(0))
			assertEquals(expectedOut1, merged.getLabels(1))

			assertEquals(expectedMaskIn0, merged.getFeaturesMaskArray(0))
			assertEquals(expectedMaskIn1, merged.getFeaturesMaskArray(1))
			assertEquals(expectedMaskOut0, merged.getLabelsMaskArray(0))
			assertEquals(expectedMaskOut1, merged.getLabelsMaskArray(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMergingWithPerOutputMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMergingWithPerOutputMasking(ByVal backend As Nd4jBackend)

			'Test 2d mask merging, 2d data
			'features
			Dim f2d1 As INDArray = Nd4j.create(New Double() {1, 2, 3}).reshape(ChrW(1), -1)
			Dim f2d2 As INDArray = Nd4j.create(New Double()() {
				New Double() {4, 5, 6},
				New Double() {7, 8, 9}
			})
			'labels
			Dim l2d1 As INDArray = Nd4j.create(New Double() {1.5, 2.5, 3.5}).reshape(ChrW(1), -1)
			Dim l2d2 As INDArray = Nd4j.create(New Double()() {
				New Double() {4.5, 5.5, 6.5},
				New Double() {7.5, 8.5, 9.5}
			})
			'feature masks
			Dim fm2d1 As INDArray = Nd4j.create(New Double() {0, 1, 1}).reshape(ChrW(1), -1)
			Dim fm2d2 As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0, 1},
				New Double() {0, 1, 0}
			})
			'label masks
			Dim lm2d1 As INDArray = Nd4j.create(New Double() {1, 1, 0}).reshape(ChrW(1), -1)
			Dim lm2d2 As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 0, 0},
				New Double() {0, 1, 1}
			})

			Dim mds2d1 As New MultiDataSet(f2d1, l2d1, fm2d1, lm2d1)
			Dim mds2d2 As New MultiDataSet(f2d2, l2d2, fm2d2, lm2d2)
			Dim merged As MultiDataSet = MultiDataSet.merge(Arrays.asList(mds2d1, mds2d2))

			Dim expFeatures2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 2, 3},
				New Double() {4, 5, 6},
				New Double() {7, 8, 9}
			})
			Dim expLabels2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1.5, 2.5, 3.5},
				New Double() {4.5, 5.5, 6.5},
				New Double() {7.5, 8.5, 9.5}
			})
			Dim expFM2d As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 1},
				New Double() {1, 0, 1},
				New Double() {0, 1, 0}
			})
			Dim expLM2d As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 1, 0},
				New Double() {1, 0, 0},
				New Double() {0, 1, 1}
			})

			Dim mdsExp2d As New MultiDataSet(expFeatures2d, expLabels2d, expFM2d, expLM2d)
			assertEquals(mdsExp2d, merged)

			'Test 4d features, 2d labels, 2d masks
			Dim f4d1 As INDArray = Nd4j.create(1, 3, 5, 5)
			Dim f4d2 As INDArray = Nd4j.create(2, 3, 5, 5)
			Dim mds4d1 As New MultiDataSet(f4d1, l2d1, Nothing, lm2d1)
			Dim mds4d2 As New MultiDataSet(f4d2, l2d2, Nothing, lm2d2)
			Dim merged4d As MultiDataSet = MultiDataSet.merge(Arrays.asList(mds4d1, mds4d2))
			assertEquals(expLabels2d, merged4d.getLabels(0))
			assertEquals(expLM2d, merged4d.getLabelsMaskArray(0))

			'Test 3d mask merging, 3d data
			Dim f3d1 As INDArray = Nd4j.create(1, 3, 4)
			Dim f3d2 As INDArray = Nd4j.create(1, 3, 3)
			Dim l3d1 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(1, 3, 4), 0.5))
			Dim l3d2 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(2, 3, 3), 0.5))
			Dim lm3d1 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(1, 3, 4), 0.5))
			Dim lm3d2 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(2, 3, 3), 0.5))
			Dim mds3d1 As New MultiDataSet(f3d1, l3d1, Nothing, lm3d1)
			Dim mds3d2 As New MultiDataSet(f3d2, l3d2, Nothing, lm3d2)

			Dim expLabels3d As INDArray = Nd4j.create(3, 3, 4)
			expLabels3d.put(New INDArrayIndex() {NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)}, l3d1)
			expLabels3d.put(New INDArrayIndex() {NDArrayIndex.interval(1, 2, True), NDArrayIndex.all(), NDArrayIndex.interval(0, 3)}, l3d2)
			Dim expLM3d As INDArray = Nd4j.create(3, 3, 4)
			expLM3d.put(New INDArrayIndex() {NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.interval(0, 4)}, lm3d1)
			expLM3d.put(New INDArrayIndex() {NDArrayIndex.interval(1, 2, True), NDArrayIndex.all(), NDArrayIndex.interval(0, 3)}, lm3d2)


			Dim merged3d As MultiDataSet = MultiDataSet.merge(Arrays.asList(mds3d1, mds3d2))
			assertEquals(expLabels3d, merged3d.getLabels(0))
			assertEquals(expLM3d, merged3d.getLabelsMaskArray(0))

			'Test 3d features, 2d masks, 2d output (for example: RNN -> global pooling w/ per-output masking)
			Dim mds3d2d1 As New MultiDataSet(f3d1, l2d1, Nothing, lm2d1)
			Dim mds3d2d2 As New MultiDataSet(f3d2, l2d2, Nothing, lm2d2)
			Dim merged3d2d As MultiDataSet = MultiDataSet.merge(Arrays.asList(mds3d2d1, mds3d2d2))

			assertEquals(expLabels2d, merged3d2d.getLabels(0))
			assertEquals(expLM2d, merged3d2d.getLabelsMaskArray(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSplit(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSplit(ByVal backend As Nd4jBackend)

			Dim features(2) As INDArray
			features(0) = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape("c"c, 3, 10)
			features(1) = Nd4j.linspace(1, 300, 300, DataType.DOUBLE).reshape("c"c, 3, 10, 10)
			features(2) = Nd4j.linspace(1, 3 * 5 * 10 * 10, 3 * 5 * 10 * 10, DataType.DOUBLE).reshape("c"c, 3, 5, 10, 10)

			Dim labels(2) As INDArray
			labels(0) = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape("c"c, 3, 10).addi(0.5)
			labels(1) = Nd4j.linspace(1, 300, 300, DataType.DOUBLE).reshape("c"c, 3, 10, 10).addi(0.3)
			labels(2) = Nd4j.linspace(1, 3 * 5 * 10 * 10, 3 * 5 * 10 * 10, DataType.DOUBLE).reshape("c"c, 3, 5, 10, 10).addi(0.1)

			Dim fMask(2) As INDArray
			fMask(1) = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape("f"c, 3, 10)

			Dim lMask(2) As INDArray
			lMask(1) = Nd4j.linspace(1, 30, 30, DataType.DOUBLE).reshape("f"c, 3, 10).addi(0.5)

			Dim mds As New MultiDataSet(features, labels, fMask, lMask)

			Dim list As IList(Of org.nd4j.linalg.dataset.api.MultiDataSet) = mds.asList()

			assertEquals(3, list.Count)
			For i As Integer = 0 To 2
				Dim m As MultiDataSet = DirectCast(list(i), MultiDataSet)
				assertEquals(2, m.getFeatures(0).rank())
				assertEquals(3, m.getFeatures(1).rank())
				assertEquals(4, m.getFeatures(2).rank())

				assertArrayEquals(New Long() {1, 10}, m.getFeatures(0).shape())
				assertArrayEquals(New Long() {1, 10, 10}, m.getFeatures(1).shape())
				assertArrayEquals(New Long() {1, 5, 10, 10}, m.getFeatures(2).shape())

				assertEquals(features(0).get(NDArrayIndex.interval(i,i,True), NDArrayIndex.all()), m.getFeatures(0))
				assertEquals(features(1).get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all()), m.getFeatures(1))
				assertEquals(features(2).get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()), m.getFeatures(2))

				assertEquals(2, m.getLabels(0).rank())
				assertEquals(3, m.getLabels(1).rank())
				assertEquals(4, m.getLabels(2).rank())

				assertArrayEquals(New Long() {1, 10}, m.getLabels(0).shape())
				assertArrayEquals(New Long() {1, 10, 10}, m.getLabels(1).shape())
				assertArrayEquals(New Long() {1, 5, 10, 10}, m.getLabels(2).shape())

				assertEquals(labels(0).get(NDArrayIndex.interval(i,i,True), NDArrayIndex.all()), m.getLabels(0))
				assertEquals(labels(1).get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all()), m.getLabels(1))
				assertEquals(labels(2).get(NDArrayIndex.interval(i, i, True), NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()), m.getLabels(2))

				assertNull(m.getFeaturesMaskArray(0))
				assertEquals(fMask(1).get(NDArrayIndex.interval(i,i,True), NDArrayIndex.all()), m.getFeaturesMaskArray(1))

				assertNull(m.getLabelsMaskArray(0))
				assertEquals(lMask(1).get(NDArrayIndex.interval(i,i,True), NDArrayIndex.all()), m.getLabelsMaskArray(1))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToString(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToString(ByVal backend As Nd4jBackend)
			'Mask arrays, and different lengths

			Dim tsLengthIn0 As Integer = 8
			Dim tsLengthIn1 As Integer = 9
			Dim tsLengthOut0 As Integer = 10
			Dim tsLengthOut1 As Integer = 11

			Dim nRows As Integer = 5
			Dim nColsIn0 As Integer = 3
			Dim nColsIn1 As Integer = 4
			Dim nColsOut0 As Integer = 5
			Dim nColsOut1 As Integer = 6

			Dim expectedIn0 As INDArray = Nd4j.zeros(nRows, nColsIn0, tsLengthIn0)
			Dim expectedIn1 As INDArray = Nd4j.zeros(nRows, nColsIn1, tsLengthIn1)
			Dim expectedOut0 As INDArray = Nd4j.zeros(nRows, nColsOut0, tsLengthOut0)
			Dim expectedOut1 As INDArray = Nd4j.zeros(nRows, nColsOut1, tsLengthOut1)

			Dim expectedMaskIn0 As INDArray = Nd4j.zeros(nRows, tsLengthIn0)
			Dim expectedMaskIn1 As INDArray = Nd4j.zeros(nRows, tsLengthIn1)
			Dim expectedMaskOut0 As INDArray = Nd4j.zeros(nRows, tsLengthOut0)
			Dim expectedMaskOut1 As INDArray = Nd4j.zeros(nRows, tsLengthOut1)


			Dim r As New Random(12345)
			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(nRows)
			For i As Integer = 0 To nRows - 1
				Dim thisRowIn0Length As Integer = tsLengthIn0 - i
				Dim thisRowIn1Length As Integer = tsLengthIn1 - i
				Dim thisRowOut0Length As Integer = tsLengthOut0 - i
				Dim thisRowOut1Length As Integer = tsLengthOut1 - i

				Dim in0NumElem As Integer = thisRowIn0Length * nColsIn0
				Dim in0 As INDArray = Nd4j.linspace(0, in0NumElem - 1, in0NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsIn0, thisRowIn0Length)

				Dim in1NumElem As Integer = thisRowIn1Length * nColsIn1
				Dim in1 As INDArray = Nd4j.linspace(0, in1NumElem - 1, in1NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsIn1, thisRowIn1Length)

				Dim out0NumElem As Integer = thisRowOut0Length * nColsOut0
				Dim out0 As INDArray = Nd4j.linspace(0, out0NumElem - 1, out0NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsOut0, thisRowOut0Length)

				Dim out1NumElem As Integer = thisRowOut1Length * nColsOut1
				Dim out1 As INDArray = Nd4j.linspace(0, out1NumElem - 1, out1NumElem, DataType.DOUBLE).reshape(ChrW(1), nColsOut1, thisRowOut1Length)

				Dim maskIn0 As INDArray = Nothing
				Dim maskIn1 As INDArray = Nd4j.zeros(1, thisRowIn1Length)
				For j As Integer = 0 To thisRowIn1Length - 1
					If r.nextBoolean() Then
						maskIn1.putScalar(j, 1.0)
					End If
				Next j
				Dim maskOut0 As INDArray = Nothing
				Dim maskOut1 As INDArray = Nd4j.zeros(1, thisRowOut1Length)
				For j As Integer = 0 To thisRowOut1Length - 1
					If r.nextBoolean() Then
						maskOut1.putScalar(j, 1.0)
					End If
				Next j

				expectedIn0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowIn0Length)}, in0)
				expectedIn1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowIn1Length)}, in1)
				expectedOut0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowOut0Length)}, out0)
				expectedOut1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.all(), NDArrayIndex.interval(0, thisRowOut1Length)}, out1)

				expectedMaskIn0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowIn0Length)}, Nd4j.ones(1, thisRowIn0Length))
				expectedMaskIn1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowIn1Length)}, maskIn1)
				expectedMaskOut0.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowOut0Length)}, Nd4j.ones(1, thisRowOut0Length))
				expectedMaskOut1.put(New INDArrayIndex() {NDArrayIndex.point(i), NDArrayIndex.interval(0, thisRowOut1Length)}, maskOut1)

				list.Add(New MultiDataSet(New INDArray() {in0, in1}, New INDArray() {out0, out1}, New INDArray() {maskIn0, maskIn1}, New INDArray() {maskOut0, maskOut1}))
			Next i

			Dim merged As MultiDataSet = MultiDataSet.merge(list)
			Console.WriteLine(merged)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void multiDataSetSaveLoadTest() throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub multiDataSetSaveLoadTest()

			Dim max As Integer = 3

			Nd4j.Random.setSeed(12345)

			For numF As Integer = 0 To max
				For numL As Integer = 0 To max
					Dim f() As INDArray = (If(numF > 0, New INDArray(numF - 1){}, Nothing))
					Dim l() As INDArray = (If(numL > 0, New INDArray(numL - 1){}, Nothing))
					Dim fm() As INDArray = (If(numF > 0, New INDArray(numF - 1){}, Nothing))
					Dim lm() As INDArray = (If(numL > 0, New INDArray(numL - 1){}, Nothing))

					If numF > 0 Then
						For i As Integer = 0 To f.Length - 1
							f(i) = Nd4j.rand(New Integer() {3, 4, 5})
						Next i
					End If
					If numL > 0 Then
						For i As Integer = 0 To l.Length - 1
							l(i) = Nd4j.rand(New Integer() {2, 3, 4})
						Next i
					End If
					If numF > 0 Then
						For i As Integer = 0 To Math.Min(fm.Length, 2) - 1
							fm(i) = Nd4j.rand(New Integer() {3, 5})
						Next i
					End If
					If numL > 0 Then
						For i As Integer = 0 To Math.Min(lm.Length, 2) - 1
							lm(i) = Nd4j.rand(New Integer() {2, 4})
						Next i
					End If

					Dim mds As New MultiDataSet(f, l, fm, lm)

					Dim baos As New MemoryStream()
					Dim dos As New DataOutputStream(baos)

					mds.save(dos)

					Dim asBytes() As SByte = baos.toByteArray()

					Dim bais As New MemoryStream(asBytes)
					Dim dis As New DataInputStream(bais)

					Dim mds2 As New MultiDataSet()
					mds2.load(dis)

					assertEquals(mds, mds2,"Failed at [" & numF & "]/[" & numL & "]")
				Next numL
			Next numF
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCnnMergeFeatureMasks(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCnnMergeFeatureMasks(ByVal backend As Nd4jBackend)
			'Tests merging of different CNN masks: [mb,1,h,1], [mb,1,1,w], [mb,1,h,w]

			For t As Integer = 0 To 2
				log.info("Starting test: {}", t)
				Dim nOut As Integer = 3
				Dim width As Integer = 5
				Dim height As Integer = 4
				Dim depth As Integer = 3
				Dim nExamples1 As Integer = 2
				Dim nExamples2 As Integer = 1

				Dim length1 As Integer = width * height * depth * nExamples1
				Dim length2 As Integer = width * height * depth * nExamples2

				Dim first As INDArray = Nd4j.linspace(1, length1, length1, DataType.DOUBLE).reshape("c"c, nExamples1, depth, height, width)
				Dim second As INDArray = Nd4j.linspace(1, length2, length2, DataType.DOUBLE).reshape("c"c, nExamples2, depth, height, width).addi(0.1)
				Dim third As INDArray = Nd4j.linspace(1, length2, length2, DataType.DOUBLE).reshape("c"c, nExamples2, depth, height, width).addi(0.2)

				Dim fm1 As INDArray = Nothing
				Dim fm2 As INDArray
				Dim fm3 As INDArray
				Select Case t
					Case 0
						fm2 = Nd4j.ones(1,1,height,1)
						fm3 = Nd4j.zeros(1,1,height,1)
						fm3.get(all(), all(), interval(0,2), all()).assign(1.0)
					Case 1
						fm2 = Nd4j.ones(1,1,1,width)
						fm3 = Nd4j.zeros(1,1,1,width)
						fm3.get(all(), all(), all(), interval(0,3)).assign(1.0)
					Case 2
						fm2 = Nd4j.ones(1,1,height,width)
						fm3 = Nd4j.zeros(1,1,height,width)
						fm3.get(all(), all(), interval(0,2), interval(0,3)).assign(1.0)
					Case Else
						Throw New Exception()
				End Select

				Dim fmExpected As INDArray = Nd4j.concat(0, Nd4j.ones(2, 1, (If(t = 1, 1, height)), (If(t = 0, 1, width))), fm2, fm3)

				Dim labels1 As INDArray = Nd4j.linspace(1, nExamples1 * nOut, nExamples1 * nOut, DataType.DOUBLE).reshape("c"c, nExamples1, nOut)
				Dim labels2 As INDArray = Nd4j.linspace(1, nExamples2 * nOut, nExamples2 * nOut, DataType.DOUBLE).reshape("c"c, nExamples2, nOut).addi(0.1)
				Dim labels3 As INDArray = Nd4j.linspace(1, nExamples2 * nOut, nExamples2 * nOut, DataType.DOUBLE).reshape("c"c, nExamples2, nOut).addi(0.2)

				Dim ds1 As New MultiDataSet(first, labels1, fm1, Nothing)
				Dim ds2 As New MultiDataSet(second, labels2, fm2, Nothing)
				Dim ds3 As New MultiDataSet(third, labels3, fm3, Nothing)

				Dim merged As MultiDataSet = MultiDataSet.merge(Arrays.asList(ds1, ds2, ds3))

				Dim fMerged As INDArray = merged.getFeatures(0)
				Dim lMerged As INDArray = merged.getLabels(0)
				Dim fmMerged As INDArray = merged.getFeaturesMaskArray(0)

				assertArrayEquals(New Long(){nExamples1 + 2*nExamples2, depth, height, width}, fMerged.shape())
				assertArrayEquals(New Long(){nExamples1 + 2*nExamples2, nOut}, lMerged.shape())
				assertArrayEquals(New Long(){nExamples1 + 2*nExamples2, 1, (If(t = 1, 1, height)), (If(t = 0, 1, width))}, fmMerged.shape())


				assertEquals(first, fMerged.get(interval(0, nExamples1), all(), all(), all()))
				Dim secondExp As INDArray = fMerged.get(interval(nExamples1, nExamples1 + nExamples2), all(), all(), all())
				assertEquals(second, secondExp)
				assertEquals(third, fMerged.get(interval(nExamples1 + nExamples2, nExamples1 + 2*nExamples2), all(), all(), all()))
				assertEquals(labels1, lMerged.get(interval(0, nExamples1), all()))
				assertEquals(labels2, lMerged.get(interval(nExamples1, nExamples1 + nExamples2), all()))
				assertEquals(labels3, lMerged.get(interval(nExamples1 + nExamples2, nExamples1 + 2*nExamples2), all()))

				assertEquals(fmExpected, fmMerged)

				'Test merging with an empty DataSet (this should be ignored)
				Dim merged2 As MultiDataSet = MultiDataSet.merge(Arrays.asList(ds1, New MultiDataSet(), ds2, ds3))
				assertEquals(merged, merged2)

				'Test merging with no features in one of the DataSets
				Dim temp As INDArray = ds1.getFeatures(0)
				ds1.setFeatures(0,Nothing)
				Try
					MultiDataSet.merge(Arrays.asList(ds1, ds2))
					fail("Expected exception")
				Catch e As System.NullReferenceException
					'OK
					assertTrue(e.Message.contains("null feature array"))
				End Try

				Try
					MultiDataSet.merge(Arrays.asList(ds2, ds1))
					fail("Expected exception")
				Catch e As System.NullReferenceException
					'OK
					assertTrue(e.Message.contains("merging"))
				End Try

				ds1.setFeatures(0, temp)
				ds2.setLabels(0, Nothing)
				Try
					MultiDataSet.merge(Arrays.asList(ds1, ds2))
					fail("Expected exception")
				Catch e As System.NullReferenceException
					'OK
					assertTrue(e.Message.contains("merging"))
				End Try

				Try
					MultiDataSet.merge(Arrays.asList(ds2, ds1))
					fail("Expected exception")
				Catch e As System.NullReferenceException
					'OK
					assertTrue(e.Message.contains("merge"))
				End Try
			Next t
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace