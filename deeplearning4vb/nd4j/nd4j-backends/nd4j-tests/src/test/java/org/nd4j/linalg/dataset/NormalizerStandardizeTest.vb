Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports TestDataSetIterator = org.nd4j.linalg.dataset.api.iterator.TestDataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class NormalizerStandardizeTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NormalizerStandardizeTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 60_000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBruteForce(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBruteForce(ByVal backend As Nd4jBackend)
	'         This test creates a dataset where feature values are multiples of consecutive natural numbers
	'           The obtained values are compared to the theoretical mean and std dev
	'         
			Dim tolerancePerc As Double = 0.01 ' 0.01% of correct value
			Dim nSamples As Integer = 5120
			Dim x As Integer = 1, y As Integer = 2, z As Integer = 3

			Dim featureX As INDArray = Nd4j.linspace(1, nSamples, nSamples, DataType.DOUBLE).reshape(ChrW(nSamples), 1).mul(x)
			Dim featureY As INDArray = featureX.mul(y)
			Dim featureZ As INDArray = featureX.mul(z)
			Dim featureSet As INDArray = Nd4j.concat(1, featureX, featureY, featureZ)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			Dim meanNaturalNums As Double = (nSamples + 1) / 2.0
			Dim theoreticalMean As INDArray = Nd4j.create(New Double() {meanNaturalNums * x, meanNaturalNums * y, meanNaturalNums * z}).reshape(ChrW(1), -1)
			Dim stdNaturalNums As Double = Math.Sqrt((nSamples * nSamples - 1) / 12.0)
			Dim theoreticalStd As INDArray = Nd4j.create(New Double() {stdNaturalNums * x, stdNaturalNums * y, stdNaturalNums * z}).reshape(ChrW(1), -1)

			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fit(sampleDataSet)

			Dim meanDelta As INDArray = Transforms.abs(theoreticalMean.sub(myNormalizer.Mean))
			Dim meanDeltaPerc As INDArray = meanDelta.div(theoreticalMean).mul(100)
			Dim maxMeanDeltaPerc As Double = meanDeltaPerc.max(1).getDouble(0)
			assertTrue(maxMeanDeltaPerc < tolerancePerc)

			Dim stdDelta As INDArray = Transforms.abs(theoreticalStd.sub(myNormalizer.Std))
			Dim stdDeltaPerc As INDArray = stdDelta.div(theoreticalStd).mul(100)
			Dim maxStdDeltaPerc As Double = stdDeltaPerc.max(1).getDouble(0)
			assertTrue(maxStdDeltaPerc < tolerancePerc)

			' SAME TEST WITH THE ITERATOR
			Dim bSize As Integer = 10
			tolerancePerc = 0.1 ' 0.1% of correct value
			Dim sampleIter As DataSetIterator = New TestDataSetIterator(sampleDataSet, bSize)
			myNormalizer.fit(sampleIter)

			meanDelta = Transforms.abs(theoreticalMean.sub(myNormalizer.Mean))
			meanDeltaPerc = meanDelta.div(theoreticalMean).mul(100)
			maxMeanDeltaPerc = meanDeltaPerc.max(1).getDouble(0)
			assertTrue(maxMeanDeltaPerc < tolerancePerc)

			stdDelta = Transforms.abs(theoreticalStd.sub(myNormalizer.Std))
			stdDeltaPerc = stdDelta.div(theoreticalStd).mul(100)
			maxStdDeltaPerc = stdDeltaPerc.max(1).getDouble(0)
			assertTrue(maxStdDeltaPerc < tolerancePerc)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTransform(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTransform(ByVal backend As Nd4jBackend)
	'        Random dataset is generated such that
	'            AX + B where X is from a normal distribution with mean 0 and std 1
	'            The mean of above will be B and std A
	'            Obtained mean and std dev are compared to theoretical
	'            Transformed values should be the same as X with the same seed.
	'         
			Dim randSeed As Long = 12345

			Dim nFeatures As Integer = 2
			Dim nSamples As Integer = 6400
			Dim bsize As Integer = 8
			Dim a As Integer = 5
			Dim b As Integer = 100
			Dim sampleMean, sampleStd, sampleMeanDelta, sampleStdDelta, delta, deltaPerc As INDArray
			Dim maxDeltaPerc, sampleMeanSEM As Double

			Dim normData As New genRandomDataSet(Me, nSamples, nFeatures, a, b, randSeed)
			Dim genRandExpected As DataSet = normData.theoreticalTransform
			Dim expectedData As New genRandomDataSet(Me, nSamples, nFeatures, 1, 0, randSeed)
			Dim beforeTransformData As New genRandomDataSet(Me, nSamples, nFeatures, a, b, randSeed)

			Dim myNormalizer As New NormalizerStandardize()
			Dim normIterator As DataSetIterator = normData.getIter(bsize)
			Dim genRandExpectedIter As DataSetIterator = New TestDataSetIterator(genRandExpected, bsize)
			Dim expectedIterator As DataSetIterator = expectedData.getIter(bsize)
			Dim beforeTransformIterator As DataSetIterator = beforeTransformData.getIter(bsize)

			myNormalizer.fit(normIterator)

			Dim tolerancePerc As Double = 0.10 'within 0.1%
			sampleMean = myNormalizer.Mean
			sampleMeanDelta = Transforms.abs(sampleMean.sub(normData.theoreticalMean))
			assertTrue(sampleMeanDelta.mul(100).div(normData.theoreticalMean).max().getDouble(0) < tolerancePerc)
			'sanity check to see if it's within the theoretical standard error of mean
			sampleMeanSEM = sampleMeanDelta.div(normData.theoreticalSEM).max().getDouble(0)
			assertTrue(sampleMeanSEM < 2.6) '99% of the time it should be within this many SEMs

			tolerancePerc = 1 'within 1% - std dev value
			sampleStd = myNormalizer.Std
			sampleStdDelta = Transforms.abs(sampleStd.sub(normData.theoreticalStd))

			Dim actualmaxDiff As Double = sampleStdDelta.div(normData.theoreticalStd).max().mul(100).getDouble(0)
			assertTrue(actualmaxDiff < tolerancePerc)

			tolerancePerc = 1 'within 1%
			normIterator.PreProcessor = myNormalizer
			Do While normIterator.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim before As INDArray = beforeTransformIterator.next().getFeatures()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim origBefore As INDArray = genRandExpectedIter.next().getFeatures()
				Dim after As INDArray = normIterator.Current.getFeatures()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim expected As INDArray = expectedIterator.next().getFeatures()
				delta = Transforms.abs(after.sub(expected))
				deltaPerc = delta.div(Transforms.abs(before.sub(expected)))
				deltaPerc.muli(100)
				maxDeltaPerc = deltaPerc.max(0, 1).getDouble(0)
	'            
	'            System.out.println("=== BEFORE ===");
	'            System.out.println(before);
	'            System.out.println("=== ORIG BEFORE ===");
	'            System.out.println(origBefore);
	'            System.out.println("=== AFTER ===");
	'            System.out.println(after);
	'            System.out.println("=== SHOULD BE ===");
	'            System.out.println(expected);
	'            System.out.println("% diff, "+ maxDeltaPerc);
	'            
				assertTrue(maxDeltaPerc < tolerancePerc)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDifferentBatchSizes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDifferentBatchSizes(ByVal backend As Nd4jBackend)
			' Create 6x1 matrix of the numbers 1 through 6
			Dim values As INDArray = Nd4j.linspace(1, 6, 6, DataType.DOUBLE).reshape(ChrW(1), -1).transpose()
'JAVA TO VB CONVERTER NOTE: The variable dataSet was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim dataSet_Conflict As New DataSet(values, values)

			' Test fitting a DataSet
			Dim norm1 As New NormalizerStandardize()
			norm1.fit(dataSet_Conflict)
			assertEquals(3.5f, norm1.Mean.getFloat(0), 1e-6)
			assertEquals(1.70783f, norm1.Std.getFloat(0), 1e-4)

			' Test fitting an iterator with equal batch sizes
			Dim testIter1 As DataSetIterator = New TestDataSetIterator(dataSet_Conflict, 3) ' Will yield 2 batches of 3 rows
			Dim norm2 As New NormalizerStandardize()
			norm2.fit(testIter1)
			assertEquals(3.5f, norm2.Mean.getFloat(0), 1e-6)
			assertEquals(1.70783f, norm2.Std.getFloat(0), 1e-4)

			' Test fitting an iterator with varying batch sizes
			Dim testIter2 As DataSetIterator = New TestDataSetIterator(dataSet_Conflict, 4) ' Will yield batch of 4 and batch of 2 rows
			Dim norm3 As New NormalizerStandardize()
			norm3.fit(testIter2)
			assertEquals(3.5f, norm3.Mean.getFloat(0), 1e-6)
			assertEquals(1.70783f, norm3.Std.getFloat(0), 1e-4)

			' Test fitting an iterator with batches of single rows
			Dim testIter3 As DataSetIterator = New TestDataSetIterator(dataSet_Conflict, 1) ' Will yield 6 batches of 1 row
			Dim norm4 As New NormalizerStandardize()
			norm4.fit(testIter3)
			assertEquals(3.5f, norm4.Mean.getFloat(0), 1e-6)
			assertEquals(1.70783f, norm4.Std.getFloat(0), 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testUnderOverflow(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testUnderOverflow(ByVal backend As Nd4jBackend)
			' This dataset will be basically constant with a small std deviation
			' And the constant is large. Checking if algorithm can handle
			Dim tolerancePerc As Double = 1 'Within 1 %
			Dim toleranceAbs As Double = 0.0005
			Dim nSamples As Integer = 1000
			Dim bSize As Integer = 10
			Dim x As Integer = -1000000, y As Integer = 1000000
			Dim z As Double = 1000000

			Dim featureX As INDArray = Nd4j.rand(nSamples, 1).mul(1).add(x)
			Dim featureY As INDArray = Nd4j.rand(nSamples, 1).mul(2).add(y)
			Dim featureZ As INDArray = Nd4j.rand(nSamples, 1).mul(3).add(z)
			Dim featureSet As INDArray = Nd4j.concat(1, featureX, featureY, featureZ)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)
			Dim sampleIter As DataSetIterator = New TestDataSetIterator(sampleDataSet, bSize)

			Dim theoreticalMean As INDArray = Nd4j.create(New Single() {x, y, CSng(z)}).castTo(Nd4j.defaultFloatingPointType()).reshape(ChrW(1), -1)

			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fit(sampleIter)

			Dim meanDelta As INDArray = Transforms.abs(theoreticalMean.sub(myNormalizer.Mean))
			Dim meanDeltaPerc As INDArray = meanDelta.mul(100).div(theoreticalMean)
			assertTrue(meanDeltaPerc.max(1).getDouble(0) < tolerancePerc)

			'this just has to not barf
			'myNormalizer.transform(sampleIter);
			myNormalizer.transform(sampleDataSet)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRevert(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRevert(ByVal backend As Nd4jBackend)
			Dim tolerancePerc As Double = 0.01 ' 0.01% of correct value
			Dim nSamples As Integer = 500
			Dim nFeatures As Integer = 3

			Dim featureSet As INDArray = Nd4j.randn(nSamples, nFeatures)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fit(sampleDataSet)
			Dim transformed As DataSet = sampleDataSet.copy()
			myNormalizer.transform(transformed)
			'System.out.println(transformed.getFeatures());
			myNormalizer.revert(transformed)
			'System.out.println(transformed.getFeatures());
			Dim delta As INDArray = Transforms.abs(transformed.Features.sub(sampleDataSet.Features)).div(sampleDataSet.Features)
			Dim maxdeltaPerc As Double = delta.max(0, 1).mul(100).getDouble(0)
			assertTrue(maxdeltaPerc < tolerancePerc)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConstant(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConstant(ByVal backend As Nd4jBackend)
			Dim tolerancePerc As Double = 10.0 ' 10% of correct value
			Dim nSamples As Integer = 500
			Dim nFeatures As Integer = 3
			Dim constant As Integer = 100

			Dim featureSet As INDArray = Nd4j.zeros(nSamples, nFeatures).add(constant)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)


			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fit(sampleDataSet)
			'Checking if we gets nans
			assertFalse(Double.IsNaN(myNormalizer.Std.getDouble(0)))

			myNormalizer.transform(sampleDataSet)
			'Checking if we gets nans, because std dev is zero
			assertFalse(Double.IsNaN(sampleDataSet.Features.min(0, 1).getDouble(0)))
			'Checking to see if transformed values are close enough to zero
			assertEquals(Transforms.abs(sampleDataSet.Features).max(0, 1).getDouble(0), 0, constant * tolerancePerc / 100.0)

			myNormalizer.revert(sampleDataSet)
			'Checking if we gets nans, because std dev is zero
			assertFalse(Double.IsNaN(sampleDataSet.Features.min(0, 1).getDouble(0)))
			assertEquals(Transforms.abs(sampleDataSet.Features.sub(featureSet)).min(0, 1).getDouble(0), 0, constant * tolerancePerc / 100.0)
		End Sub

		Public Class genRandomDataSet
			Private ReadOnly outerInstance As NormalizerStandardizeTest

	'         generate random dataset from normally distributed mean 0, std 1
	'        based on given seed and scaling constants
	'         
			Friend sampleDataSet As DataSet
			Friend theoreticalMean As INDArray
			Friend theoreticalStd As INDArray
			Friend theoreticalSEM As INDArray
			Friend theoreticalTransform As DataSet

			Public Sub New(ByVal outerInstance As NormalizerStandardizeTest, ByVal nSamples As Integer, ByVal nFeatures As Integer, ByVal a As Integer, ByVal b As Integer, ByVal randSeed As Long)
				Me.outerInstance = outerInstance
	'             if a =1 and b = 0,normal distribution
	'                otherwise with some random mean and some random distribution
	'             
				Dim i As Integer = 0
				' Randomly generate scaling constants and add offsets
				' to get aA and bB
				Dim aA As INDArray = If(a = 1, Nd4j.ones(1, nFeatures), Nd4j.rand(New Integer(){1, nFeatures}, randSeed).mul(a)) 'a = 1, don't scale
				Dim bB As INDArray = Nd4j.rand(New Integer(){1, nFeatures}, randSeed).mul(b) 'b = 0 this zeros out
				' transform ndarray as X = aA + bB * X
				Dim randomFeatures As INDArray = Nd4j.zeros(nSamples, nFeatures)
				Dim randomFeaturesTransform As INDArray = Nd4j.zeros(nSamples, nFeatures)
				Do While i < nFeatures
					Dim randomSlice As INDArray = Nd4j.randn(randSeed, New Long(){nSamples, 1})
					randomFeaturesTransform.putColumn(i, randomSlice)
					randomSlice.muli(aA.getScalar(0, i))
					randomSlice.addi(bB.getScalar(0, i))
					randomFeatures.putColumn(i, randomSlice)
					i += 1
				Loop
				Dim randomLabels As INDArray = Nd4j.zeros(nSamples, 1)
				Me.sampleDataSet = New DataSet(randomFeatures, randomLabels)
				Me.theoreticalTransform = New DataSet(randomFeaturesTransform, randomLabels)
				Me.theoreticalMean = bB
				Me.theoreticalStd = aA
				Me.theoreticalSEM = Me.theoreticalStd.div(Math.Sqrt(nSamples))
			End Sub

			Public Overridable Function getIter(ByVal bsize As Integer) As DataSetIterator
				Return New TestDataSetIterator(sampleDataSet, bsize)
			End Function
		End Class


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace