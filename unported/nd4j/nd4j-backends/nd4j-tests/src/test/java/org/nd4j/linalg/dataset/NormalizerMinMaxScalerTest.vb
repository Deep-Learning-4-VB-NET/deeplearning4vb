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
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class NormalizerMinMaxScalerTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NormalizerMinMaxScalerTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBruteForce(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBruteForce(ByVal backend As Nd4jBackend)
			'X_std = (X - X.min(axis=0)) / (X.max(axis=0) - X.min(axis=0))
			'X_scaled = X_std * (max - min) + min
			' Dataset features are scaled consecutive natural numbers
			Dim nSamples As Integer = 500
			Dim x As Integer = 4, y As Integer = 2, z As Integer = 3

			Dim featureX As INDArray = Nd4j.linspace(1, nSamples, nSamples).reshape(ChrW(nSamples), 1)
			Dim featureY As INDArray = featureX.mul(y)
			Dim featureZ As INDArray = featureX.mul(z)
			featureX.muli(x)
			Dim featureSet As INDArray = Nd4j.concat(1, featureX, featureY, featureZ)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			'expected min and max
			Dim theoreticalMin As INDArray = Nd4j.create(New Double() {x, y, z}, New Long(){1, 3})
			Dim theoreticalMax As INDArray = Nd4j.create(New Double() {nSamples * x, nSamples * y, nSamples * z}, New Long(){1, 3})
			Dim theoreticalRange As INDArray = theoreticalMax.sub(theoreticalMin)

			Dim myNormalizer As New NormalizerMinMaxScaler()
			myNormalizer.fit(sampleDataSet)

			Dim minDataSet As INDArray = myNormalizer.Min
			Dim maxDataSet As INDArray = myNormalizer.Max
			Dim minDiff As INDArray = minDataSet.sub(theoreticalMin).max()
			Dim maxDiff As INDArray = maxDataSet.sub(theoreticalMax).max()
			assertEquals(minDiff.getDouble(0), 0.0, 0.000000001)
			assertEquals(maxDiff.max().getDouble(0), 0.0, 0.000000001)

			' SAME TEST WITH THE ITERATOR
			Dim bSize As Integer = 1
			Dim sampleIter As DataSetIterator = New TestDataSetIterator(sampleDataSet, bSize)
			myNormalizer.fit(sampleIter)
			minDataSet = myNormalizer.Min
			maxDataSet = myNormalizer.Max
			assertEquals(minDataSet.sub(theoreticalMin).max(1).getDouble(0), 0.0, 0.000000001)
			assertEquals(maxDataSet.sub(theoreticalMax).max(1).getDouble(0), 0.0, 0.000000001)

			sampleIter.PreProcessor = myNormalizer
			Dim actual, expected, delta As INDArray
			Dim i As Integer = 1
			Do While sampleIter.MoveNext()
				expected = theoreticalMin.mul(i - 1).div(theoreticalRange)
				actual = sampleIter.Current.getFeatures()
				delta = Transforms.abs(actual.sub(expected))
				assertTrue(delta.max(1).getDouble(0) < 0.0001)
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRevert(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRevert(ByVal backend As Nd4jBackend)
			Dim tolerancePerc As Double = 1 ' 1% of correct value
			Dim nSamples As Integer = 500
			Dim nFeatures As Integer = 3

			Nd4j.Random.setSeed(12345)
			Dim featureSet As INDArray = Nd4j.rand(nSamples, nFeatures)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			Dim myNormalizer As New NormalizerMinMaxScaler()
			Dim transformed As DataSet = sampleDataSet.copy()

			myNormalizer.fit(sampleDataSet)
			myNormalizer.transform(transformed)
			myNormalizer.revert(transformed)
			Dim delta As INDArray = Transforms.abs(transformed.Features.sub(sampleDataSet.Features)).div(sampleDataSet.Features)
			Dim maxdeltaPerc As Double = delta.max(0, 1).mul(100).getDouble(0)
			Console.WriteLine("Delta: " & maxdeltaPerc)
			assertTrue(maxdeltaPerc < tolerancePerc)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGivenMaxMin(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGivenMaxMin(ByVal backend As Nd4jBackend)
			Dim tolerancePerc As Double = 1 ' 1% of correct value
			Dim nSamples As Integer = 500
			Dim nFeatures As Integer = 3

			Nd4j.Random.setSeed(12345)
			Dim featureSet As INDArray = Nd4j.rand(nSamples, nFeatures)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			Dim givenMin As Double = -1
			Dim givenMax As Double = 1
			Dim myNormalizer As New NormalizerMinMaxScaler(givenMin, givenMax)
			Dim transformed As DataSet = sampleDataSet.copy()

			myNormalizer.fit(sampleDataSet)
			myNormalizer.transform(transformed)

			myNormalizer.revert(transformed)
			Dim delta As INDArray = Transforms.abs(transformed.Features.sub(sampleDataSet.Features)).div(sampleDataSet.Features)
			Dim maxdeltaPerc As Double = delta.max(0, 1).mul(100).getDouble(0)
			Console.WriteLine("Delta: " & maxdeltaPerc)
			assertTrue(maxdeltaPerc < tolerancePerc)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGivenMaxMinConstant(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGivenMaxMinConstant(ByVal backend As Nd4jBackend)
			Dim tolerancePerc As Double = 1 ' 1% of correct value
			Dim nSamples As Integer = 500
			Dim nFeatures As Integer = 3

			Dim featureSet As INDArray = Nd4j.rand(nSamples, nFeatures).mul(0.1).add(10)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			Dim givenMin As Double = -1000
			Dim givenMax As Double = 1000
			Dim myNormalizer As DataNormalization = New NormalizerMinMaxScaler(givenMin, givenMax)
			Dim transformed As DataSet = sampleDataSet.copy()

			myNormalizer.fit(sampleDataSet)
			myNormalizer.transform(transformed)

			'feature set is basically all 10s -> should transform to the min
			Dim expected As INDArray = Nd4j.ones(nSamples, nFeatures).mul(givenMin)
			Dim delta As INDArray = Transforms.abs(transformed.Features.sub(expected)).div(expected)
			Dim maxdeltaPerc As Double = delta.max(0, 1).mul(100).getDouble(0)
			assertTrue(maxdeltaPerc < tolerancePerc)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConstant(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConstant(ByVal backend As Nd4jBackend)
			Dim tolerancePerc As Double = 0.01 ' 0.01% of correct value
			Dim nSamples As Integer = 500
			Dim nFeatures As Integer = 3

			Dim featureSet As INDArray = Nd4j.zeros(nSamples, nFeatures).add(100)
			Dim labelSet As INDArray = Nd4j.zeros(nSamples, 1)
			Dim sampleDataSet As New DataSet(featureSet, labelSet)

			Dim myNormalizer As New NormalizerMinMaxScaler()
			myNormalizer.fit(sampleDataSet)
			myNormalizer.transform(sampleDataSet)
			assertFalse(Double.IsNaN(sampleDataSet.Features.min(0, 1).getDouble(0)))
			assertEquals(sampleDataSet.Features.sumNumber().doubleValue(), 0, 0.00001)
			myNormalizer.revert(sampleDataSet)
			assertFalse(Double.IsNaN(sampleDataSet.Features.min(0, 1).getDouble(0)))
			assertEquals(sampleDataSet.Features.sumNumber().doubleValue(), 100 * nFeatures * nSamples, 0.00001)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class


End Namespace