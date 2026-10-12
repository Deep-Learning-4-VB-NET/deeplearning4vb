Imports System
Imports System.Collections.Generic
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports TestDataSetIterator = org.nd4j.linalg.dataset.api.iterator.TestDataSetIterator
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports ImageMultiPreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImageMultiPreProcessingScaler
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports MultiDataNormalization = org.nd4j.linalg.dataset.api.preprocessor.MultiDataNormalization
Imports MultiNormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerMinMaxScaler
Imports MultiNormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerStandardize
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports VGG16ImagePreProcessor = org.nd4j.linalg.dataset.api.preprocessor.VGG16ImagePreProcessor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class NormalizerTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NormalizerTests
		Inherits BaseNd4jTestWithBackends


		Private stdScaler As NormalizerStandardize
		Private minMaxScaler As NormalizerMinMaxScaler
		Private data As DataSet
		Private batchSize As Integer
		Private batchCount As Integer
		Private lastBatch As Integer
		Private ReadOnly thresholdPerc As Single = 2.0f 'this is the difference in percentage!

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void randomData()
		Public Overridable Sub randomData()
			Nd4j.Random.setSeed(12345)
			batchSize = 13
			batchCount = 20
			lastBatch = batchSize \ 2
			Dim origFeatures As INDArray = Nd4j.rand(batchCount * batchSize + lastBatch, 10)
			Dim origLabels As INDArray = Nd4j.rand(batchCount * batchSize + lastBatch, 3)
			data = New DataSet(origFeatures, origLabels)
			stdScaler = New NormalizerStandardize()
			minMaxScaler = New NormalizerMinMaxScaler()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPreProcessors(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPreProcessors(ByVal backend As Nd4jBackend)
			Console.WriteLine("Running iterator vs non-iterator std scaler..")
			Dim d1 As Double = testItervsDataset(stdScaler)
			assertTrue(d1 < thresholdPerc,d1 & " < " & thresholdPerc)
			Console.WriteLine("Running iterator vs non-iterator min max scaler..")
			Dim d2 As Double = testItervsDataset(minMaxScaler)
			assertTrue(d2 < thresholdPerc,d2 & " < " & thresholdPerc)
		End Sub

		Public Overridable Function testItervsDataset(ByVal preProcessor As DataNormalization) As Single
			Dim dataCopy As DataSet = data.copy()
			Dim dataIter As DataSetIterator = New TestDataSetIterator(dataCopy, batchSize)
			preProcessor.fit(dataCopy)
			preProcessor.transform(dataCopy)
			Dim transformA As INDArray = dataCopy.Features

			preProcessor.fit(dataIter)
			dataIter.PreProcessor = preProcessor
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = dataIter.next()
			Dim transformB As INDArray = [next].Features

			Do While dataIter.MoveNext()
				[next] = dataIter.Current
'JAVA TO VB CONVERTER NOTE: The variable transformb was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
				Dim transformb_Conflict As INDArray = [next].Features
				transformB = Nd4j.vstack(transformB, transformb_Conflict)
			Loop

			Return Transforms.abs(transformB.div(transformA).rsub(1)).maxNumber().floatValue()
		End Function



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMasking(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(235)

			Dim normalizers() As DataNormalization = {
				New NormalizerMinMaxScaler(),
				New NormalizerStandardize()
			}

			Dim normalizersNoMask() As DataNormalization = {
				New NormalizerMinMaxScaler(),
				New NormalizerStandardize()
			}

			Dim normalizersByRow() As DataNormalization = {
				New NormalizerMinMaxScaler(),
				New NormalizerStandardize()
			}


			For i As Integer = 0 To normalizers.Length - 1
				'First: check that normalization is the same with/without masking arrays
				Dim norm As DataNormalization = normalizers(i)
				Dim normFitSubset As DataNormalization = normalizersNoMask(i)
				Dim normByRow As DataNormalization = normalizersByRow(i)

				Console.WriteLine(norm.GetType())


				Dim arr As INDArray = Nd4j.rand("c"c, New Integer() {2, 3, 5}).muli(100).addi(100)
				arr.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(3, 5)).assign(0)
				Dim arrCopy As INDArray = arr.dup()

				Dim arrPt1 As INDArray = arr.get(NDArrayIndex.interval(0, 0, True), NDArrayIndex.all(), NDArrayIndex.all()).dup()
				Dim arrPt2 As INDArray = arr.get(NDArrayIndex.interval(1, 1, True), NDArrayIndex.all(), NDArrayIndex.interval(0, 3)).dup()


				Dim mask As INDArray = Nd4j.create(New Double()() {
					New Double() {1, 1, 1, 1, 1},
					New Double() {1, 1, 1, 0, 0}
				}).castTo(Nd4j.defaultFloatingPointType())

				Dim ds As New DataSet(arr, Nothing, mask, Nothing)
				Dim dsCopy1 As New DataSet(arr.dup(), Nothing, mask, Nothing)
				Dim dsCopy2 As New DataSet(arr.dup(), Nothing, mask, Nothing)
				norm.fit(ds)

				'Check that values aren't modified by fit op
				assertEquals(arrCopy, arr)

				Dim toFitTimeSeries1Ex As IList(Of DataSet) = New List(Of DataSet)()
				toFitTimeSeries1Ex.Add(New DataSet(arrPt1, arrPt1))
				toFitTimeSeries1Ex.Add(New DataSet(arrPt2, arrPt2))
				normFitSubset.fit(New TestDataSetIterator(toFitTimeSeries1Ex, 1))

				Dim toFitRows As IList(Of DataSet) = New List(Of DataSet)()
				For j As Integer = 0 To 4
					Dim row As INDArray = arr.get(NDArrayIndex.point(0), NDArrayIndex.all(), NDArrayIndex.interval(j, j, True)).transpose()
					assertTrue(row.RowVector)
					toFitRows.Add(New DataSet(row, row))
				Next j

				For j As Integer = 0 To 2
					Dim row As INDArray = arr.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(j, j, True)).transpose()
					assertTrue(row.RowVector)
					toFitRows.Add(New DataSet(row, row))
				Next j

				normByRow.fit(New TestDataSetIterator(toFitRows, 1))

				norm.transform(ds)
				normFitSubset.transform(dsCopy1)
				normByRow.transform(dsCopy2)

				assertEquals(ds.Features, dsCopy1.Features)
				assertEquals(ds.Labels, dsCopy1.Labels)
				assertEquals(ds.FeaturesMaskArray, dsCopy1.FeaturesMaskArray)
				assertEquals(ds.LabelsMaskArray, dsCopy1.LabelsMaskArray)

				assertEquals(ds, dsCopy1)
				assertEquals(ds, dsCopy2)

				'Second: ensure time steps post normalization (and post revert) are 0.0
				Dim shouldBe0_1 As INDArray = ds.Features.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(3, 5))
				Dim shouldBe0_2 As INDArray = dsCopy1.Features.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(3, 5))
				Dim shouldBe0_3 As INDArray = dsCopy2.Features.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(3, 5))

				Dim zeros As INDArray = Nd4j.zeros(shouldBe0_1.shape())

	'            for (int j = 0; j < 2; j++) {
	'                System.out.println(ds.getFeatures().get(NDArrayIndex.point(j), NDArrayIndex.all(),
	'                                NDArrayIndex.all()));
	'                System.out.println();
	'            }

				assertEquals(zeros, shouldBe0_1)
				assertEquals(zeros, shouldBe0_2)
				assertEquals(zeros, shouldBe0_3)

				'Check same thing after reverting:
				norm.revert(ds)
				normFitSubset.revert(dsCopy1)
				normByRow.revert(dsCopy2)
				shouldBe0_1 = ds.Features.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(3, 5))
				shouldBe0_2 = dsCopy1.Features.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(3, 5))
				shouldBe0_3 = dsCopy2.Features.get(NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.interval(3, 5))

				assertEquals(zeros, shouldBe0_1)
				assertEquals(zeros, shouldBe0_2)
				assertEquals(zeros, shouldBe0_3)


			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormalizerToStringHashCode()
		Public Overridable Sub testNormalizerToStringHashCode()
			'https://github.com/eclipse/deeplearning4j/issues/8565

			testNormalizer(New NormalizerMinMaxScaler())
			Dim n1 As New NormalizerMinMaxScaler()
			n1.fitLabel(True)
			testNormalizer(n1)

			testNormalizer(New NormalizerStandardize())
			Dim n2 As New NormalizerStandardize()
			n2.fitLabel(True)
			testNormalizer(n2)

			testNormalizer(New ImagePreProcessingScaler())
			Dim n3 As New ImagePreProcessingScaler()
			n3.fitLabel(True)
			testNormalizer(n3)

			testNormalizer(New VGG16ImagePreProcessor())
			Dim n4 As New VGG16ImagePreProcessor()
			n4.fitLabel(True)
			testNormalizer(n4)
		End Sub

		Private Shared Sub testNormalizer(ByVal n As DataNormalization)
			n.ToString()
			n.GetHashCode()

			n.fit(New IrisDataSetIterator(30, 150))

			n.ToString()
			n.GetHashCode()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerToStringHashCode()
		Public Overridable Sub testMultiNormalizerToStringHashCode()
			'https://github.com/eclipse/deeplearning4j/issues/8565

			testMultiNormalizer(New MultiNormalizerMinMaxScaler())
			Dim n1 As New MultiNormalizerMinMaxScaler()
			n1.fitLabel(True)
			testMultiNormalizer(n1)

			testMultiNormalizer(New MultiNormalizerStandardize())
			Dim n2 As New MultiNormalizerStandardize()
			n2.fitLabel(True)
			testMultiNormalizer(n2)

			testMultiNormalizer(New ImageMultiPreProcessingScaler(0))
		End Sub

		Private Shared Sub testMultiNormalizer(ByVal n As MultiDataNormalization)
			n.ToString()
			n.GetHashCode()

			n.fit(New MultiDataSetIteratorAdapter(New IrisDataSetIterator(30, 150)))

			n.ToString()
			n.GetHashCode()
		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace