Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
Imports org.junit.jupiter.api.Assertions
Imports org.nd4j.linalg.indexing.NDArrayIndex

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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class DataSetTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DataSetTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testViewIterator(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testViewIterator(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim iter As DataSetIterator = New ViewIterator((New IrisDataSetIterator(150, 150)).next(), 10)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iter.hasNext())
			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim [next] As DataSet = iter.Current
				count += 1
				assertArrayEquals(New Long() {10, 4}, [next].Features.shape())
			Loop

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(iter.hasNext())
			assertEquals(15, count)
			iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(iter.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testViewIterator2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testViewIterator2(ByVal backend As Nd4jBackend)

			Dim f As INDArray = Nd4j.linspace(1,100,100, DataType.DOUBLE).reshape("c"c, 10, 10)
			Dim ds As New DataSet(f, f)
			Dim iter As DataSetIterator = New ViewIterator(ds, 1)
			For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(iter.hasNext())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim d As DataSet = iter.next()
				Dim exp As INDArray = f.getRow(i, True)
				assertEquals(exp, d.Features)
				assertEquals(exp, d.Labels)
			Next i
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(iter.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testViewIterator3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testViewIterator3(ByVal backend As Nd4jBackend)

			Dim f As INDArray = Nd4j.linspace(1,100,100, DataType.DOUBLE).reshape("c"c, 10, 10)
			Dim ds As New DataSet(f, f)
			Dim iter As DataSetIterator = New ViewIterator(ds, 6)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim d1 As DataSet = iter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim d2 As DataSet = iter.next()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertFalse(iter.hasNext())
			Dim e1 As INDArray = f.get(interval(0,6), all())
			Dim e2 As INDArray = f.get(interval(6,10), all())

			assertEquals(e1, d1.Features)
			assertEquals(e2, d2.Features)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSplitTestAndTrain(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSplitTestAndTrain(ByVal backend As Nd4jBackend)
			Dim labels As INDArray = FeatureUtil.toOutcomeMatrix(New Integer() {0, 0, 0, 0, 0, 0, 0, 0}, 1)
			Dim data As New DataSet(Nd4j.rand(8, 1), labels)

			Dim train As SplitTestAndTrain = data.splitTestAndTrain(6, New Random(1))
			assertEquals(train.Train.Labels.length(), 6)

			Dim train2 As SplitTestAndTrain = data.splitTestAndTrain(6, New Random(1))
			assertEquals(train.Train.Features, train2.Train.Features,getFailureMessage(backend))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim x0 As DataSet = (New IrisDataSetIterator(150, 150)).next()
			Dim testAndTrain As SplitTestAndTrain = x0.splitTestAndTrain(10)
			assertArrayEquals(New Long() {10, 4}, testAndTrain.Train.Features.shape())
			assertEquals(x0.Features.getRows(ArrayUtil.range(0, 10)), testAndTrain.Train.Features)
			assertEquals(x0.Labels.getRows(ArrayUtil.range(0, 10)), testAndTrain.Train.Labels)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSplitTestAndTrainRng(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSplitTestAndTrainRng(ByVal backend As Nd4jBackend)

			Dim rngHere As Random

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim x1 As DataSet = (New IrisDataSetIterator(150, 150)).next() 'original
			Dim x2 As DataSet = x1.copy() 'call split test train with rng

			'Manual shuffle
			x1.shuffle((New Random(123)).nextLong())
			Dim testAndTrain As SplitTestAndTrain = x1.splitTestAndTrain(10)
			' Pass rng with splt test train
			rngHere = New Random(123)
			Dim testAndTrainRng As SplitTestAndTrain = x2.splitTestAndTrain(10, rngHere)

			assertArrayEquals(testAndTrainRng.Train.Features.shape(), testAndTrain.Train.Features.shape())
			assertEquals(testAndTrainRng.Train.Features, testAndTrain.Train.Features)
			assertEquals(testAndTrainRng.Train.Labels, testAndTrain.Train.Labels)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLabelCounts(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLabelCounts(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim x0 As DataSet = (New IrisDataSetIterator(150, 150)).next()
			assertEquals(0, x0.get(0).outcome(),getFailureMessage(backend))
			assertEquals(0, x0.get(1).outcome(),getFailureMessage(backend))
			assertEquals(2, x0.get(149).outcome(),getFailureMessage(backend))
			Dim counts As IDictionary(Of Integer, Double) = x0.labelCounts()
			assertEquals(50, counts(0), 1e-1,getFailureMessage(backend))
			assertEquals(50, counts(1), 1e-1,getFailureMessage(backend))
			assertEquals(50, counts(2), 1e-1,getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTimeSeriesMerge(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTimeSeriesMerge(ByVal backend As Nd4jBackend)
			'Basic test for time series, all of the same length + no masking arrays
			Dim numExamples As Integer = 10
			Dim inSize As Integer = 13
			Dim labelSize As Integer = 5
			Dim tsLength As Integer = 15

			Nd4j.Random.setSeed(12345)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(New Long() {1, inSize, tsLength})
				Dim [out] As INDArray = Nd4j.rand(New Long() {1, labelSize, tsLength})
				list.Add(New DataSet([in], [out]))
			Next i

			Dim merged As DataSet = DataSet.merge(list)
			assertEquals(numExamples, merged.numExamples())

			Dim f As INDArray = merged.Features
			Dim l As INDArray = merged.Labels
			assertArrayEquals(New Long() {numExamples, inSize, tsLength}, f.shape())
			assertArrayEquals(New Long() {numExamples, labelSize, tsLength}, l.shape())

			For i As Integer = 0 To numExamples - 1
				Dim exp As DataSet = list(i)
				Dim expIn As INDArray = exp.Features
				Dim expL As INDArray = exp.Labels

				Dim fSubset As INDArray = f.get(interval(i, i + 1), all(), all())
				Dim lSubset As INDArray = l.get(interval(i, i + 1), all(), all())

				assertEquals(expIn, fSubset)
				assertEquals(expL, lSubset)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTimeSeriesMergeDifferentLength(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTimeSeriesMergeDifferentLength(ByVal backend As Nd4jBackend)
			'Test merging of time series with different lengths -> no masking arrays on the input DataSets

			Dim numExamples As Integer = 10
			Dim inSize As Integer = 13
			Dim labelSize As Integer = 5
			Dim minTSLength As Integer = 10 'Lengths 10, 11, ..., 19

			Nd4j.Random.setSeed(12345)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(New Long() {1, inSize, minTSLength + i})
				Dim [out] As INDArray = Nd4j.rand(New Long() {1, labelSize, minTSLength + i})
				list.Add(New DataSet([in], [out]))
			Next i

			Dim merged As DataSet = DataSet.merge(list)
			assertEquals(numExamples, merged.numExamples())

			Dim f As INDArray = merged.Features
			Dim l As INDArray = merged.Labels
			Dim expectedLength As Integer = minTSLength + numExamples - 1
			assertArrayEquals(New Long() {numExamples, inSize, expectedLength}, f.shape())
			assertArrayEquals(New Long() {numExamples, labelSize, expectedLength}, l.shape())

			assertTrue(merged.hasMaskArrays())
			assertNotNull(merged.FeaturesMaskArray)
			assertNotNull(merged.LabelsMaskArray)
			Dim featuresMask As INDArray = merged.FeaturesMaskArray
			Dim labelsMask As INDArray = merged.LabelsMaskArray
			assertArrayEquals(New Long() {numExamples, expectedLength}, featuresMask.shape())
			assertArrayEquals(New Long() {numExamples, expectedLength}, labelsMask.shape())

			'Check each row individually:
			For i As Integer = 0 To numExamples - 1
				Dim exp As DataSet = list(i)
				Dim expIn As INDArray = exp.Features
				Dim expL As INDArray = exp.Labels

				Dim thisRowOriginalLength As Integer = minTSLength + i

				Dim fSubset As INDArray = f.get(interval(i, i + 1), all(), all())
				Dim lSubset As INDArray = l.get(interval(i, i + 1), all(), all())

				For j As Integer = 0 To inSize - 1
					For k As Integer = 0 To thisRowOriginalLength - 1
						Dim expected As Double = expIn.getDouble(0, j, k)
						Dim act As Double = fSubset.getDouble(0, j, k)
						If Math.Abs(expected - act) > 1e-3 Then
							Console.WriteLine(expIn)
							Console.WriteLine(fSubset)
						End If
						assertEquals(expected, act, 1e-3f)
					Next k

					'Padded values: should be exactly 0.0
					For k As Integer = thisRowOriginalLength To expectedLength - 1
						assertEquals(0.0, fSubset.getDouble(0, j, k), 0.0)
					Next k
				Next j

				For j As Integer = 0 To labelSize - 1
					For k As Integer = 0 To thisRowOriginalLength - 1
						Dim expected As Double = expL.getDouble(0, j, k)
						Dim act As Double = lSubset.getDouble(0, j, k)
						assertEquals(expected, act, 1e-3f)
					Next k

					'Padded values: should be exactly 0.0
					For k As Integer = thisRowOriginalLength To expectedLength - 1
						assertEquals(0.0, lSubset.getDouble(0, j, k), 0.0)
					Next k
				Next j

				'Check mask values:
				For j As Integer = 0 To expectedLength - 1
					Dim expected As Double = (If(j >= thisRowOriginalLength, 0.0, 1.0))
					Dim actFMask As Double = featuresMask.getDouble(i, j)
					Dim actLMask As Double = labelsMask.getDouble(i, j)

					If expected <> actFMask Then
						Console.WriteLine(featuresMask)
						Console.WriteLine(j)
					End If

					assertEquals(expected, actFMask, 0.0)
					assertEquals(expected, actLMask, 0.0)
				Next j
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTimeSeriesMergeWithMasking(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTimeSeriesMergeWithMasking(ByVal backend As Nd4jBackend)
			'Test merging of time series with (a) different lengths, and (b) mask arrays in the input DataSets

			Dim numExamples As Integer = 10
			Dim inSize As Integer = 13
			Dim labelSize As Integer = 5
			Dim minTSLength As Integer = 10 'Lengths 10, 11, ..., 19

			Dim r As New Random(12345)

			Nd4j.Random.setSeed(12345)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(New Long() {1, inSize, minTSLength + i})
				Dim [out] As INDArray = Nd4j.rand(New Long() {1, labelSize, minTSLength + i})

				Dim inMask As INDArray = Nd4j.create(1, minTSLength + i)
				Dim outMask As INDArray = Nd4j.create(1, minTSLength + i)
				Dim j As Integer = 0
				Do While j < inMask.size(1)
					inMask.putScalar(j, (If(r.nextBoolean(), 1.0, 0.0)))
					outMask.putScalar(j, (If(r.nextBoolean(), 1.0, 0.0)))
					j += 1
				Loop

				list.Add(New DataSet([in], [out], inMask, outMask))
			Next i

			Dim merged As DataSet = DataSet.merge(list)
			assertEquals(numExamples, merged.numExamples())

			Dim f As INDArray = merged.Features
			Dim l As INDArray = merged.Labels
			Dim expectedLength As Integer = minTSLength + numExamples - 1
			assertArrayEquals(New Long() {numExamples, inSize, expectedLength}, f.shape())
			assertArrayEquals(New Long() {numExamples, labelSize, expectedLength}, l.shape())

			assertTrue(merged.hasMaskArrays())
			assertNotNull(merged.FeaturesMaskArray)
			assertNotNull(merged.LabelsMaskArray)
			Dim featuresMask As INDArray = merged.FeaturesMaskArray
			Dim labelsMask As INDArray = merged.LabelsMaskArray
			assertArrayEquals(New Long() {numExamples, expectedLength}, featuresMask.shape())
			assertArrayEquals(New Long() {numExamples, expectedLength}, labelsMask.shape())

			'Check each row individually:
			For i As Integer = 0 To numExamples - 1
				Dim original As DataSet = list(i)
				Dim expIn As INDArray = original.Features
				Dim expL As INDArray = original.Labels
				Dim origMaskF As INDArray = original.FeaturesMaskArray
				Dim origMaskL As INDArray = original.LabelsMaskArray

				Dim thisRowOriginalLength As Integer = minTSLength + i

				Dim fSubset As INDArray = f.get(interval(i, i + 1), all(), all())
				Dim lSubset As INDArray = l.get(interval(i, i + 1), all(), all())

				For j As Integer = 0 To inSize - 1
					For k As Integer = 0 To thisRowOriginalLength - 1
						Dim expected As Double = expIn.getDouble(0, j, k)
						Dim act As Double = fSubset.getDouble(0, j, k)
						If Math.Abs(expected - act) > 1e-3 Then
							Console.WriteLine(expIn)
							Console.WriteLine(fSubset)
						End If
						assertEquals(expected, act, 1e-3f)
					Next k

					'Padded values: should be exactly 0.0
					For k As Integer = thisRowOriginalLength To expectedLength - 1
						assertEquals(0.0, fSubset.getDouble(0, j, k), 0.0)
					Next k
				Next j

				For j As Integer = 0 To labelSize - 1
					For k As Integer = 0 To thisRowOriginalLength - 1
						Dim expected As Double = expL.getDouble(0, j, k)
						Dim act As Double = lSubset.getDouble(0, j, k)
						assertEquals(expected, act, 1e-3f)
					Next k

					'Padded values: should be exactly 0.0
					For k As Integer = thisRowOriginalLength To expectedLength - 1
						assertEquals(0.0, lSubset.getDouble(0, j, k), 0.0)
					Next k
				Next j

				'Check mask values:
				For j As Integer = 0 To expectedLength - 1
					Dim expectedF As Double
					Dim expectedL As Double
					If j >= thisRowOriginalLength Then
						'Outside of original data bounds -> should be 0
						expectedF = 0.0
						expectedL = 0.0
					Else
						'Value should be same as original mask array value
						expectedF = origMaskF.getDouble(j)
						expectedL = origMaskL.getDouble(j)
					End If

					Dim actFMask As Double = featuresMask.getDouble(i, j)
					Dim actLMask As Double = labelsMask.getDouble(i, j)
					assertEquals(expectedF, actFMask, 0.0)
					assertEquals(expectedL, actLMask, 0.0)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCnnMerge(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCnnMerge(ByVal backend As Nd4jBackend)
			'Test merging of CNN data sets
			Dim nOut As Integer = 3
			Dim width As Integer = 5
			Dim height As Integer = 4
			Dim depth As Integer = 3
			Dim nExamples1 As Integer = 2
			Dim nExamples2 As Integer = 1

			Dim length1 As Integer = width * height * depth * nExamples1
			Dim length2 As Integer = width * height * depth * nExamples2

			Dim first As INDArray = Nd4j.linspace(1, length1, length1, DataType.DOUBLE).reshape("c"c, nExamples1, depth, width, height)
			Dim second As INDArray = Nd4j.linspace(1, length2, length2, DataType.DOUBLE).reshape("c"c, nExamples2, depth, width, height).addi(0.1)

			Dim labels1 As INDArray = Nd4j.linspace(1, nExamples1 * nOut, nExamples1 * nOut, DataType.DOUBLE).reshape("c"c, nExamples1, nOut)
			Dim labels2 As INDArray = Nd4j.linspace(1, nExamples2 * nOut, nExamples2 * nOut, DataType.DOUBLE).reshape("c"c, nExamples2, nOut)

			Dim ds1 As New DataSet(first, labels1)
			Dim ds2 As New DataSet(second, labels2)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged = DataSet.merge(Arrays.asList(ds1, ds2));
			Dim merged As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, ds2})

			Dim fMerged As INDArray = merged.Features
			Dim lMerged As INDArray = merged.Labels

			assertArrayEquals(New Long() {nExamples1 + nExamples2, depth, width, height}, fMerged.shape())
			assertArrayEquals(New Long() {nExamples1 + nExamples2, nOut}, lMerged.shape())


			assertEquals(first, fMerged.get(interval(0, nExamples1), all(), all(), all()))
			assertEquals(second, fMerged.get(interval(nExamples1, nExamples1 + nExamples2), all(), all(), all()))
			assertEquals(labels1, lMerged.get(interval(0, nExamples1), all()))
			assertEquals(labels2, lMerged.get(interval(nExamples1, nExamples1 + nExamples2), all()))


			'Test merging with an empty DataSet (this should be ignored)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged2 = DataSet.merge(Arrays.asList(ds1, new DataSet(), ds2));
			Dim merged2 As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, New DataSet(), ds2})
			assertEquals(merged, merged2)

			'Test merging with no features in one of the DataSets
			Dim temp As INDArray = ds1.Features
			ds1.Features = Nothing
			Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds1, ds2));
				DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, ds2})
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				'OK
				assertTrue(e.Message.contains("Cannot merge"))
			End Try

			Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds2, ds1));
				DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds2, ds1})
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				'OK
				assertTrue(e.Message.contains("Cannot merge"))
			End Try

			ds1.Features = temp
			ds2.Labels = Nothing
			Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds1, ds2));
				DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, ds2})
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				'OK
				assertTrue(e.Message.contains("Cannot merge"))
			End Try

			Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds2, ds1));
				DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds2, ds1})
				fail("Expected exception")
			Catch e As System.InvalidOperationException
				'OK
				assertTrue(e.Message.contains("Cannot merge"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCnnMergeFeatureMasks(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCnnMergeFeatureMasks(ByVal backend As Nd4jBackend)
			'Tests merging of different CNN masks: [mb,1,h,1], [mb,1,1,w], [mb,1,h,w]

			For t As Integer = 0 To 2
	'            log.info("Starting test: {}", t);
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

				Dim ds1 As New DataSet(first, labels1, fm1, Nothing)
				Dim ds2 As New DataSet(second, labels2, fm2, Nothing)
				Dim ds3 As New DataSet(third, labels3, fm3, Nothing)

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged = DataSet.merge(Arrays.asList(ds1, ds2, ds3));
				Dim merged As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, ds2, ds3})

				Dim fMerged As INDArray = merged.Features
				Dim lMerged As INDArray = merged.Labels
				Dim fmMerged As INDArray = merged.FeaturesMaskArray

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
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged2 = DataSet.merge(Arrays.asList(ds1, new DataSet(), ds2, ds3));
				Dim merged2 As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, New DataSet(), ds2, ds3})
				assertEquals(merged, merged2)

				'Test merging with no features in one of the DataSets
				Dim temp As INDArray = ds1.Features
				ds1.Features = Nothing
				Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds1, ds2));
					DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, ds2})
					fail("Expected exception")
				Catch e As System.InvalidOperationException
					'OK
					assertTrue(e.Message.contains("Cannot merge"))
				End Try

				Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds2, ds1));
					DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds2, ds1})
					fail("Expected exception")
				Catch e As System.InvalidOperationException
					'OK
					assertTrue(e.Message.contains("Cannot merge"))
				End Try

				ds1.Features = temp
				ds2.Labels = Nothing
				Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds1, ds2));
					DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds1, ds2})
					fail("Expected exception")
				Catch e As System.InvalidOperationException
					'OK
					assertTrue(e.Message.contains("Cannot merge"))
				End Try

				Try
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet.merge(Arrays.asList(ds2, ds1));
					DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds2, ds1})
					fail("Expected exception")
				Catch e As System.InvalidOperationException
					'OK
					assertTrue(e.Message.contains("Cannot merge"))
				End Try
			Next t
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMixedRnn2dMerging(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMixedRnn2dMerging(ByVal backend As Nd4jBackend)
			'RNN input with 2d label output
			'Basic test for time series, all of the same length + no masking arrays
			Dim numExamples As Integer = 10
			Dim inSize As Integer = 13
			Dim labelSize As Integer = 5
			Dim tsLength As Integer = 15

			Nd4j.Random.setSeed(12345)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(New Long() {1, inSize, tsLength})
				Dim [out] As INDArray = Nd4j.rand(New Long() {1, labelSize})
				list.Add(New DataSet([in], [out]))
			Next i

			Dim merged As DataSet = DataSet.merge(list)
			assertEquals(numExamples, merged.numExamples())

			Dim f As INDArray = merged.Features
			Dim l As INDArray = merged.Labels
			assertArrayEquals(New Long() {numExamples, inSize, tsLength}, f.shape())
			assertArrayEquals(New Long() {numExamples, labelSize}, l.shape())

			For i As Integer = 0 To numExamples - 1
				Dim exp As DataSet = list(i)
				Dim expIn As INDArray = exp.Features
				Dim expL As INDArray = exp.Labels

				Dim fSubset As INDArray = f.get(interval(i, i + 1), all(), all())
				Dim lSubset As INDArray = l.get(interval(i, i + 1), all())

				assertEquals(expIn, fSubset)
				assertEquals(expL, lSubset)
			Next i
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

			Dim mds2d1 As New DataSet(f2d1, l2d1, fm2d1, lm2d1)
			Dim mds2d2 As New DataSet(f2d2, l2d2, fm2d2, lm2d2)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged = DataSet.merge(Arrays.asList(mds2d1, mds2d2));
			Dim merged As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {mds2d1, mds2d2})

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

			Dim dsExp2d As New DataSet(expFeatures2d, expLabels2d, expFM2d, expLM2d)
			assertEquals(dsExp2d, merged)

			'Test 4d features, 2d labels, 2d masks
			Dim f4d1 As INDArray = Nd4j.create(1, 3, 5, 5)
			Dim f4d2 As INDArray = Nd4j.create(2, 3, 5, 5)
			Dim ds4d1 As New DataSet(f4d1, l2d1, Nothing, lm2d1)
			Dim ds4d2 As New DataSet(f4d2, l2d2, Nothing, lm2d2)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged4d = DataSet.merge(Arrays.asList(ds4d1, ds4d2));
			Dim merged4d As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds4d1, ds4d2})
			assertEquals(expLabels2d, merged4d.Labels)
			assertEquals(expLM2d, merged4d.LabelsMaskArray)

			'Test 3d mask merging, 3d data
			Dim f3d1 As INDArray = Nd4j.create(1, 3, 4)
			Dim f3d2 As INDArray = Nd4j.create(1, 3, 3)
			Dim l3d1 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(1, 3, 4), 0.5))
			Dim l3d2 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(2, 3, 3), 0.5))
			Dim lm3d1 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(1, 3, 4), 0.5))
			Dim lm3d2 As INDArray = Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.create(2, 3, 3), 0.5))
			Dim ds3d1 As New DataSet(f3d1, l3d1, Nothing, lm3d1)
			Dim ds3d2 As New DataSet(f3d2, l3d2, Nothing, lm3d2)

			Dim expLabels3d As INDArray = Nd4j.create(3, 3, 4)
			expLabels3d.put(New INDArrayIndex() {interval(0,1), all(), interval(0, 4)}, l3d1)
			expLabels3d.put(New INDArrayIndex() {interval(1, 2, True), all(), interval(0, 3)}, l3d2)
			Dim expLM3d As INDArray = Nd4j.create(3, 3, 4)
			expLM3d.put(New INDArrayIndex() {interval(0,1), all(), interval(0, 4)}, lm3d1)
			expLM3d.put(New INDArrayIndex() {interval(1, 2, True), all(), interval(0, 3)}, lm3d2)


'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged3d = DataSet.merge(Arrays.asList(ds3d1, ds3d2));
			Dim merged3d As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds3d1, ds3d2})
			assertEquals(expLabels3d, merged3d.Labels)
			assertEquals(expLM3d, merged3d.LabelsMaskArray)

			'Test 3d features, 2d masks, 2d output (for example: RNN -> global pooling w/ per-output masking)
			Dim ds3d2d1 As New DataSet(f3d1, l2d1, Nothing, lm2d1)
			Dim ds3d2d2 As New DataSet(f3d2, l2d2, Nothing, lm2d2)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: DataSet merged3d2d = DataSet.merge(Arrays.asList(ds3d2d1, ds3d2d2));
			Dim merged3d2d As DataSet = DataSet.merge(New List(Of org.nd4j.linalg.dataset.api.DataSet) From {ds3d2d1, ds3d2d2})

			assertEquals(expLabels2d, merged3d2d.Labels)
			assertEquals(expLM2d, merged3d2d.LabelsMaskArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShuffle4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShuffle4d(ByVal backend As Nd4jBackend)
			Dim nSamples As Integer = 10
			Dim nChannels As Integer = 3
			Dim imgRows As Integer = 4
			Dim imgCols As Integer = 2

			Dim nLabels As Integer = 5
			Dim shape As val = New Long() {nSamples, nChannels, imgRows, imgCols}

			Dim entries As Integer = nSamples * nChannels * imgRows * imgCols
			Dim labels As Integer = nSamples * nLabels

			Dim ds_data As INDArray = Nd4j.linspace(1, entries, entries, DataType.INT).reshape(ChrW(nSamples), nChannels, imgRows, imgCols)
			Dim ds_labels As INDArray = Nd4j.linspace(1, labels, labels, DataType.INT).reshape(ChrW(nSamples), nLabels)
			Dim ds As New DataSet(ds_data, ds_labels)
			ds.shuffle()

			For [dim] As Integer = 1 To 3
				'get tensor along dimension - the order in every dimension but zero should be preserved
				Dim tensorNum As Integer = 0
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Do While tensorNum < entries / shape([dim])
					Dim i As Integer = 0
					Dim j As Integer = 1
					Do While j < shape([dim])
						Dim f_element As Integer = ds.Features.tensorAlongDimension(tensorNum, [dim]).getInt(i)
						Dim f_next_element As Integer = ds.Features.tensorAlongDimension(tensorNum, [dim]).getInt(j)
						Dim f_element_diff As Integer = f_next_element - f_element
						assertEquals(f_element_diff, ds_data.stride([dim]))
						i += 1
						j += 1
					Loop
					tensorNum += 1
				Loop
			Next [dim]
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShuffleNd(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShuffleNd(ByVal backend As Nd4jBackend)
			Dim numDims As Integer = 7
			Dim nLabels As Integer = 3
			Dim r As New Random()


			Dim shape(numDims - 1) As Integer
			Dim entries As Integer = 1
			For i As Integer = 0 To numDims - 1
				'randomly generating shapes bigger than 1
				shape(i) = r.Next(4) + 2
				entries *= shape(i)
			Next i
			Dim labels As Integer = shape(0) * nLabels

			Dim ds_data As INDArray = Nd4j.linspace(1, entries, entries, DataType.INT).reshape(shape)
			Dim ds_labels As INDArray = Nd4j.linspace(1, labels, labels, DataType.INT).reshape(ChrW(shape(0)), nLabels)

			Dim ds As New DataSet(ds_data, ds_labels)
			ds.shuffle()

			'Checking Nd dataset which is the data
			For [dim] As Integer = 1 To numDims - 1
				'get tensor along dimension - the order in every dimension but zero should be preserved
				Dim tensorNum As Integer = 0
				Do While tensorNum < ds_data.tensorsAlongDimension([dim])
					'the difference between consecutive elements should be equal to the stride
					Dim i As Integer = 0
					Dim j As Integer = 1
					Do While j < shape([dim])
						Dim f_element As Integer = ds.Features.tensorAlongDimension(tensorNum, [dim]).getInt(i)
						Dim f_next_element As Integer = ds.Features.tensorAlongDimension(tensorNum, [dim]).getInt(j)
						Dim f_element_diff As Integer = f_next_element - f_element
						assertEquals(f_element_diff, ds_data.stride([dim]))
						i += 1
						j += 1
					Loop
					tensorNum += 1
				Loop
			Next [dim]

			'Checking 2d, features
			Dim [dim] As Integer = 1
			'get tensor along dimension - the order in every dimension but zero should be preserved
			Dim tensorNum As Integer = 0
			Do While tensorNum < ds_labels.tensorsAlongDimension([dim])
				'the difference between consecutive elements should be equal to the stride
				Dim i As Integer = 0
				Dim j As Integer = 1
				Do While j < nLabels
					Dim l_element As Integer = ds.Labels.tensorAlongDimension(tensorNum, [dim]).getInt(i)
					Dim l_next_element As Integer = ds.Labels.tensorAlongDimension(tensorNum, [dim]).getInt(j)
					Dim l_element_diff As Integer = l_next_element - l_element
					assertEquals(l_element_diff, ds_labels.stride([dim]))
					i += 1
					j += 1
				Loop
				tensorNum += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShuffleMeta(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShuffleMeta(ByVal backend As Nd4jBackend)
			Dim nExamples As Integer = 20
			Dim nColumns As Integer = 4

			Dim f As INDArray = Nd4j.zeros(nExamples, nColumns)
			Dim l As INDArray = Nd4j.zeros(nExamples, nColumns)
			Dim meta As IList(Of Integer) = New List(Of Integer)()

			For i As Integer = 0 To nExamples - 1
				f.getRow(i).assign(i)
				l.getRow(i).assign(i)
				meta.Add(i)
			Next i

			Dim ds As New DataSet(f, l)
			ds.ExampleMetaData = meta

			For i As Integer = 0 To 9
				ds.shuffle()
				Dim fCol As INDArray = f.getColumn(0)
				Dim lCol As INDArray = l.getColumn(0)
	'            System.out.println(fCol + "\t" + ds.getExampleMetaData());
				For j As Integer = 0 To nExamples - 1
					Dim fVal As Integer = CInt(Math.Truncate(fCol.getDouble(j)))
					Dim lVal As Integer = CInt(Math.Truncate(lCol.getDouble(j)))
					Dim metaVal As Integer = CType(ds.getExampleMetaData()(j), Integer?)

					assertEquals(fVal, lVal)
					assertEquals(fVal, metaVal)
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLabelNames(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLabelNames(ByVal backend As Nd4jBackend)
			Dim names As IList(Of String) = New List(Of String) From {"label1", "label2", "label3", "label0"}
			Dim features As INDArray = Nd4j.ones(10)
			Dim labels As INDArray = Nd4j.linspace(0, 3, 4, DataType.DOUBLE)
			Dim ds As org.nd4j.linalg.dataset.api.DataSet = New DataSet(features, labels)
			ds.LabelNames = names
			assertEquals("label1", ds.getLabelName(0))
			assertEquals(4, ds.getLabelNamesList().Count)
			assertEquals(names, ds.getLabelNames(labels))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToString(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToString(ByVal backend As Nd4jBackend)
			Dim ds As org.nd4j.linalg.dataset.api.DataSet = New DataSet()
			'this should not throw a null pointer
	'        System.out.println(ds);
			ds.ToString()

			'Checking printing of masks
			Dim numExamples As Integer = 10
			Dim inSize As Integer = 13
			Dim labelSize As Integer = 5
			Dim minTSLength As Integer = 10 'Lengths 10, 11, ..., 19

			Nd4j.Random.setSeed(12345)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(New Long() {1, inSize, minTSLength + i})
				Dim [out] As INDArray = Nd4j.rand(New Long() {1, labelSize, minTSLength + i})
				list.Add(New DataSet([in], [out]))
			Next i

			ds = DataSet.merge(list)
	'        System.out.println(ds);
			ds.ToString()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRangeMask(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRangeMask(ByVal backend As Nd4jBackend)
			Dim ds As org.nd4j.linalg.dataset.api.DataSet = New DataSet()
			'Checking printing of masks
			Dim numExamples As Integer = 10
			Dim inSize As Integer = 13
			Dim labelSize As Integer = 5
			Dim minTSLength As Integer = 10 'Lengths 10, 11, ..., 19

			Nd4j.Random.setSeed(12345)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(DataType.DOUBLE, 1, inSize, minTSLength + i)
				Dim [out] As INDArray = Nd4j.rand(DataType.DOUBLE, 1, labelSize, minTSLength + i)
				list.Add(New DataSet([in], [out]))
			Next i

			Dim from As Integer = 3
			Dim [to] As Integer = 9
			ds = DataSet.merge(list)
			Dim newDs As org.nd4j.linalg.dataset.api.DataSet = ds.getRange(from, [to])
			'The feature mask does not have to be equal to the label mask, just in this ex it should be
			assertEquals(newDs.LabelsMaskArray, newDs.FeaturesMaskArray)
			'System.out.println(newDs);
			Dim exp As INDArray = Nd4j.linspace(numExamples + from, numExamples + [to] - 1, [to] - from, DataType.DOUBLE)
			Dim act As INDArray = newDs.LabelsMaskArray.sum(1)
			assertEquals(exp, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAsList(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAsList(ByVal backend As Nd4jBackend)
			Dim ds As org.nd4j.linalg.dataset.api.DataSet
			'Comparing merge with asList
			Dim numExamples As Integer = 10
			Dim inSize As Integer = 13
			Dim labelSize As Integer = 5
			Dim minTSLength As Integer = 10 'Lengths 10, 11, ..., 19

			Nd4j.Random.setSeed(12345)
			Dim list As IList(Of DataSet) = New List(Of DataSet)(numExamples)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(New Long() {1, inSize, minTSLength + i})
				Dim [out] As INDArray = Nd4j.rand(New Long() {1, labelSize, minTSLength + i})
				list.Add(New DataSet([in], [out]))
			Next i

			'Merged dataset and dataset list
			ds = DataSet.merge(list)
			Dim dsList As IList(Of DataSet) = ds.asList()
			'Reset seed
			Nd4j.Random.setSeed(12345)
			For i As Integer = 0 To numExamples - 1
				Dim [in] As INDArray = Nd4j.rand(New Long() {1, inSize, minTSLength + i})
				Dim [out] As INDArray = Nd4j.rand(New Long() {1, labelSize, minTSLength + i})
				Dim iDataSet As New DataSet([in], [out])

				'Checking if the features and labels are equal
				assertEquals(iDataSet.Features, dsList(i).getFeatures().get(all(), all(), interval(0, minTSLength + i)))
				assertEquals(iDataSet.Labels, dsList(i).getLabels().get(all(), all(), interval(0, minTSLength + i)))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDataSetSaveLoad(org.nd4j.linalg.factory.Nd4jBackend backend) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataSetSaveLoad(ByVal backend As Nd4jBackend)

			Dim b() As Boolean = {True, False}

			Dim f As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape("c"c, 4, 3, 2)
			Dim l As INDArray = Nd4j.linspace(24, 48, 24, DataType.DOUBLE).reshape("c"c, 4, 3, 2)
			Dim fm As INDArray = Nd4j.linspace(100, 108, 8, DataType.DOUBLE).reshape("c"c, 4, 2)
			Dim lm As INDArray = Nd4j.linspace(108, 116, 8, DataType.DOUBLE).reshape("c"c, 4, 2)

			For Each features As Boolean In b
				For Each labels As Boolean In b
					For Each labelsSameAsFeatures As Boolean In b
						If labelsSameAsFeatures AndAlso (Not features OrElse Not labels) Then
							Continue For 'Can't have "labels same as features" if no features, or if no labels
						End If

						For Each fMask As Boolean In b
							For Each lMask As Boolean In b

								Dim ds As New DataSet((If(features, f, Nothing)), (If(labels, (If(labelsSameAsFeatures, f, l)), Nothing)), (If(fMask, fm, Nothing)), (If(lMask, lm, Nothing)))

								Dim baos As New MemoryStream()
								Dim dos As New DataOutputStream(baos)

								ds.save(dos)

								Dim asBytes() As SByte = baos.toByteArray()

								Dim bais As New MemoryStream(asBytes)
								Dim dis As New DataInputStream(bais)

								Dim ds2 As New DataSet()
								ds2.load(dis)
								dis.close()

								assertEquals(ds, ds2)

								If labelsSameAsFeatures Then
									assertTrue(ds2.Features Is ds2.Labels) 'Expect same object
								End If
							Next lMask
						Next fMask
					Next labelsSameAsFeatures
				Next labels
			Next features
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDataSetSaveLoadSingle(org.nd4j.linalg.factory.Nd4jBackend backend) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataSetSaveLoadSingle(ByVal backend As Nd4jBackend)

			Dim f As INDArray = Nd4j.linspace(1, 24, 24, DataType.DOUBLE).reshape("c"c, 4, 3, 2)
			Dim l As INDArray = Nd4j.linspace(24, 48, 24, DataType.DOUBLE).reshape("c"c, 4, 3, 2)
			Dim fm As INDArray = Nd4j.linspace(100, 108, 8, DataType.DOUBLE).reshape("c"c, 4, 2)
			Dim lm As INDArray = Nd4j.linspace(108, 116, 8, DataType.DOUBLE).reshape("c"c, 4, 2)

			Dim features As Boolean = True
			Dim labels As Boolean = False
			Dim labelsSameAsFeatures As Boolean = False
			Dim fMask As Boolean = True
			Dim lMask As Boolean = True

			Dim ds As New DataSet((If(features, f, Nothing)), (If(labels, (If(labelsSameAsFeatures, f, l)), Nothing)), (If(fMask, fm, Nothing)), (If(lMask, lm, Nothing)))

			Dim baos As New MemoryStream()
			Dim dos As New DataOutputStream(baos)

			ds.save(dos)
			dos.close()

			Dim asBytes() As SByte = baos.toByteArray()

			Dim bais As New MemoryStream(asBytes)
			Dim dis As New DataInputStream(bais)

			Dim ds2 As New DataSet()
			ds2.load(dis)
			dis.close()

			assertEquals(ds, ds2)

			If labelsSameAsFeatures Then
				assertTrue(ds2.Features Is ds2.Labels) 'Expect same object
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMdsShuffle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMdsShuffle(ByVal backend As Nd4jBackend)

			Dim orig As New MultiDataSet(Nd4j.linspace(1,100,100, DataType.DOUBLE).reshape("c"c,10,10), Nd4j.linspace(100,200,100, DataType.DOUBLE).reshape("c"c,10,10))

			Dim mds As New MultiDataSet(Nd4j.linspace(1,100,100, DataType.DOUBLE).reshape("c"c,10,10), Nd4j.linspace(100,200,100, DataType.DOUBLE).reshape("c"c,10,10))
			mds.shuffle()

			assertNotEquals(orig, mds)

			Dim foundF(9) As Boolean
			Dim foundL(9) As Boolean

			For i As Integer = 0 To 9
				Dim f As Double = mds.getFeatures(0).getDouble(i,0)
				Dim l As Double = mds.getLabels(0).getDouble(i,0)

				Dim fi As Integer = CInt(Math.Truncate(f/10.0)) '21.0 -> 2, etc
				Dim li As Integer = CInt(Math.Truncate((l-100)/10.0)) '121.0 -> 2

				foundF(fi) = True
				foundL(li) = True
			Next i

			Dim allF As Boolean = True
			Dim allL As Boolean = True
			For i As Integer = 0 To 9
				allF = allF And foundF(i)
				allL = allL And foundL(i)
			Next i

			assertTrue(allF)
			assertTrue(allL)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSample4d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSample4d(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			Dim next1 As Integer = Nd4j.Random.nextInt(4)
			Dim next2 As Integer = Nd4j.Random.nextInt(4)

			assertNotEquals(next1, next2)

			Dim arr As INDArray = Nd4j.create(DataType.DOUBLE, 4,1,5,5)
			For i As Integer = 0 To 3
				arr.get(point(i), all(), all(), all()).assign(i)
			Next i

			Dim ds As New DataSet(arr, arr)

			Nd4j.Random.setSeed(12345)
			Dim ds2 As DataSet = ds.sample(2)

			assertEquals(Nd4j.valueArrayOf(New Long(){1, 5, 5}, CDbl(next1)), ds2.Features.get(point(0), all(), all(), all()))
			assertEquals(Nd4j.valueArrayOf(New Long(){1, 5, 5}, CDbl(next2)), ds2.Features.get(point(1), all(), all(), all()))

			assertEquals(Nd4j.valueArrayOf(New Long(){1, 5, 5}, CDbl(next1)), ds2.Labels.get(point(0), all(), all(), all()))
			assertEquals(Nd4j.valueArrayOf(New Long(){1, 5, 5}, CDbl(next2)), ds2.Labels.get(point(1), all(), all(), all()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDataSetMetaDataSerialization(org.nd4j.linalg.factory.Nd4jBackend backend) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDataSetMetaDataSerialization(ByVal backend As Nd4jBackend)

			For Each withMeta As Boolean In New Boolean(){False, True}
				' create simple data set with meta data object
				Dim f As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE).reshape(ChrW(3), 1)
				Dim l As INDArray = Nd4j.linspace(10, 30, 3, DataType.DOUBLE).reshape(ChrW(3), 1)
				Dim ds As New DataSet(f, l)

				If withMeta Then
					Dim metaData As IList(Of String) = New List(Of String) From {"1", "2", "3"}
					ds.ExampleMetaData = metaData
				End If

				' check if the meta data was serialized and deserialized
				Dim dir As File = testDir.toFile()
				Dim saved As New File(dir, "ds.bin")
				ds.save(saved)
				Dim loaded As New DataSet()
				loaded.load(saved)
				If withMeta Then
					Dim metaData As IList(Of String) = New List(Of String) From {"1", "2", "3"}
					assertNotNull(loaded.getExampleMetaData())
					assertEquals(metaData, loaded.getExampleMetaData())
				End If
				assertEquals(f, loaded.Features)
				assertEquals(l, loaded.Labels)
			Next withMeta
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiDataSetMetaDataSerialization(org.nd4j.linalg.factory.Nd4jBackend nd4jBackend) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiDataSetMetaDataSerialization(ByVal nd4jBackend As Nd4jBackend)

			For Each withMeta As Boolean In New Boolean(){False, True}
				' create simple data set with meta data object
				Dim f As INDArray = Nd4j.linspace(1, 3, 3, DataType.DOUBLE).reshape(ChrW(3), 1)
				Dim l As INDArray = Nd4j.linspace(10, 30, 3, DataType.DOUBLE).reshape(ChrW(3), 1)
				Dim ds As New MultiDataSet(f, l)
				If withMeta Then
					Dim metaData As IList(Of String) = New List(Of String) From {"1", "2", "3"}
					ds.ExampleMetaData = metaData
				End If

				' check if the meta data was serialized and deserialized
				Dim dir As File = testDir.toFile()
				Dim saved As New File(dir, "ds.bin")
				ds.save(saved)
				Dim loaded As New MultiDataSet()
				loaded.load(saved)

				If withMeta Then
					Dim metaData As IList(Of String) = New List(Of String) From {"1", "2", "3"}
					assertNotNull(loaded.getExampleMetaData())
					assertEquals(metaData, loaded.getExampleMetaData())
				End If
				assertEquals(f, loaded.getFeatures(0))
				assertEquals(l, loaded.getLabels(0))
			Next withMeta

		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace