Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports TestDataSetIterator = org.nd4j.linalg.dataset.api.iterator.TestDataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class NormalizerStandardizeLabelsTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NormalizerStandardizeLabelsTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBruteForce(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBruteForce(ByVal backend As Nd4jBackend)
	'         This test creates a dataset where feature values are multiples of consecutive natural numbers
	'           The obtained values are compared to the theoretical mean and std dev
	'         
			Dim tolerancePerc As Double = 0.01
			Dim nSamples As Integer = 5120
			Dim x As Integer = 1, y As Integer = 2, z As Integer = 3

			Dim featureX As INDArray = Nd4j.linspace(1, nSamples, nSamples).reshape(ChrW(nSamples), 1).mul(x)
			Dim featureY As INDArray = featureX.mul(y)
			Dim featureZ As INDArray = featureX.mul(z)
			Dim featureSet As INDArray = Nd4j.concat(1, featureX, featureY, featureZ)
			Dim labelSet As INDArray = featureSet.dup().getColumns(0)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			Dim meanNaturalNums As Double = (nSamples + 1) / 2.0
			Dim theoreticalMean As INDArray = Nd4j.create(New Double() {meanNaturalNums * x, meanNaturalNums * y, meanNaturalNums * z}).reshape(ChrW(1), -1).castTo(Nd4j.defaultFloatingPointType())
			Dim theoreticallabelMean As INDArray = theoreticalMean.dup().getColumns(0)
			Dim stdNaturalNums As Double = Math.Sqrt((nSamples * nSamples - 1) / 12.0)
			Dim theoreticalStd As INDArray = Nd4j.create(New Double() {stdNaturalNums * x, stdNaturalNums * y, stdNaturalNums * z}).reshape(ChrW(1), -1).castTo(Nd4j.defaultFloatingPointType())
			Dim theoreticallabelStd As INDArray = theoreticalStd.dup().getColumns(0)

			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fitLabel(True)
			myNormalizer.fit(sampleDataSet)

			Dim meanDelta As INDArray = Transforms.abs(theoreticalMean.sub(myNormalizer.Mean))
			Dim labelDelta As INDArray = Transforms.abs(theoreticallabelMean.sub(myNormalizer.LabelMean))
			Dim meanDeltaPerc As INDArray = meanDelta.div(theoreticalMean).mul(100)
			Dim labelDeltaPerc As INDArray = labelDelta.div(theoreticallabelMean).mul(100)
			Dim maxMeanDeltaPerc As Double = meanDeltaPerc.max(1).getDouble(0)
			assertTrue(maxMeanDeltaPerc < tolerancePerc)
			assertTrue(labelDeltaPerc.max(1).getDouble(0) < tolerancePerc)

			Dim stdDelta As INDArray = Transforms.abs(theoreticalStd.sub(myNormalizer.Std))
			Dim stdDeltaPerc As INDArray = stdDelta.div(theoreticalStd).mul(100)
			Dim stdlabelDeltaPerc As INDArray = Transforms.abs(theoreticallabelStd.sub(myNormalizer.LabelStd)).div(theoreticallabelStd)
			Dim maxStdDeltaPerc As Double = stdDeltaPerc.max(1).mul(100).getDouble(0)
			Dim maxlabelStdDeltaPerc As Double = stdlabelDeltaPerc.max(1).getDouble(0)
			assertTrue(maxStdDeltaPerc < tolerancePerc)
			assertTrue(maxlabelStdDeltaPerc < tolerancePerc)


			' SAME TEST WITH THE ITERATOR
			Dim bSize As Integer = 10
			tolerancePerc = 0.1 ' 1% of correct value
			Dim sampleIter As DataSetIterator = New TestDataSetIterator(sampleDataSet, bSize)
			myNormalizer.fit(sampleIter)

			meanDelta = Transforms.abs(theoreticalMean.sub(myNormalizer.Mean))
			meanDeltaPerc = meanDelta.div(theoreticalMean).mul(100)
			maxMeanDeltaPerc = meanDeltaPerc.max(1).getDouble(0)
			assertTrue(maxMeanDeltaPerc < tolerancePerc)

			stdDelta = Transforms.abs(theoreticalMean.sub(myNormalizer.Mean))
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
			Dim expectedData As New genRandomDataSet(Me, nSamples, nFeatures, 1, 0, randSeed)
			Dim beforeTransformData As New genRandomDataSet(Me, nSamples, nFeatures, a, b, randSeed)

			Dim myNormalizer As New NormalizerStandardize()
			myNormalizer.fitLabel(True)
			Dim normIterator As DataSetIterator = normData.getIter(bsize)
			Dim expectedIterator As DataSetIterator = expectedData.getIter(bsize)
			Dim beforeTransformIterator As DataSetIterator = beforeTransformData.getIter(bsize)

			myNormalizer.fit(normIterator)

			Dim tolerancePerc As Double = 0.5 'within 0.5%
			sampleMean = myNormalizer.Mean
			sampleMeanDelta = Transforms.abs(sampleMean.sub(normData.theoreticalMean))
			assertTrue(sampleMeanDelta.mul(100).div(normData.theoreticalMean).max().getDouble(0) < tolerancePerc)
			'sanity check to see if it's within the theoretical standard error of mean
			sampleMeanSEM = sampleMeanDelta.div(normData.theoreticalSEM).max().getDouble(0)
			assertTrue(sampleMeanSEM < 2.6,sampleMeanSEM.ToString()) '99% of the time it should be within this many SEMs

			tolerancePerc = 5 'within 5%
			sampleStd = myNormalizer.Std
			sampleStdDelta = Transforms.abs(sampleStd.sub(normData.theoreticalStd))
			assertTrue(sampleStdDelta.div(normData.theoreticalStd).max().mul(100).getDouble(0) < tolerancePerc)

			tolerancePerc = 1 'within 1%
			normIterator.PreProcessor = myNormalizer
			Do While normIterator.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim before As INDArray = beforeTransformIterator.next().getFeatures()
				Dim here As DataSet = normIterator.Current
				assertEquals(here.Features, here.Labels) 'bootstrapping existing test on features
				Dim after As INDArray = here.Features
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim expected As INDArray = expectedIterator.next().getFeatures()
				delta = Transforms.abs(after.sub(expected))
				deltaPerc = delta.div(before.sub(expected))
				deltaPerc.muli(100)
				maxDeltaPerc = deltaPerc.max(0, 1).getDouble(0)
				'System.out.println("=== BEFORE ===");
				'System.out.println(before);
				'System.out.println("=== AFTER ===");
				'System.out.println(after);
				'System.out.println("=== SHOULD BE ===");
				'System.out.println(expected);
				assertTrue(maxDeltaPerc < tolerancePerc)
			Loop
		End Sub


		Public Class genRandomDataSet
			Private ReadOnly outerInstance As NormalizerStandardizeLabelsTest

	'         generate random dataset from normally distributed mean 0, std 1
	'        based on given seed and scaling constants
	'         
			Friend sampleDataSet As DataSet
			Friend theoreticalMean As INDArray
			Friend theoreticalStd As INDArray
			Friend theoreticalSEM As INDArray

			Public Sub New(ByVal outerInstance As NormalizerStandardizeLabelsTest, ByVal nSamples As Integer, ByVal nFeatures As Integer, ByVal a As Integer, ByVal b As Integer, ByVal randSeed As Long)
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
				Do While i < nFeatures
					Dim randomSlice As INDArray = Nd4j.randn(randSeed, New Long(){nSamples, 1})
					randomSlice.muli(aA.getScalar(0, i))
					randomSlice.addi(bB.getScalar(0, i))
					randomFeatures.putColumn(i, randomSlice)
					i += 1
				Loop
				Dim randomLabels As INDArray = randomFeatures.dup()
				Me.sampleDataSet = New DataSet(randomFeatures, randomLabels)
				Me.theoreticalMean = bB.dup()
				Me.theoreticalStd = aA.dup()
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