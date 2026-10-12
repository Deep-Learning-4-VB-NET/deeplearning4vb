Imports System
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports TestMultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.TestMultiDataSetIterator
Imports MultiNormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerMinMaxScaler
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
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class MultiNormalizerMinMaxScalerTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class MultiNormalizerMinMaxScalerTest
		Inherits BaseNd4jTestWithBackends

		Private Const TOLERANCE_PERC As Double = 0.01 ' 0.01% of correct value
		Private Const INPUT1_SCALE As Integer = 1, INPUT2_SCALE As Integer = 2, OUTPUT1_SCALE As Integer = 3, OUTPUT2_SCALE As Integer = 4

		Private SUT As MultiNormalizerMinMaxScaler
		Private data As MultiDataSet

		Private naturalMin As Double
		Private naturalMax As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			SUT = New MultiNormalizerMinMaxScaler()
			SUT.fitLabel(True)

			' Prepare test data
			Dim nSamples As Integer = 5120

			Dim values As INDArray = Nd4j.linspace(1, nSamples, nSamples, Nd4j.dataType()).reshape(ChrW(1), -1).transpose()
			Dim input1 As INDArray = values.mul(INPUT1_SCALE)
			Dim input2 As INDArray = values.mul(INPUT2_SCALE)
			Dim output1 As INDArray = values.mul(OUTPUT1_SCALE)
			Dim output2 As INDArray = values.mul(OUTPUT2_SCALE)

			data = New MultiDataSet(New INDArray() {input1, input2}, New INDArray() {output1, output2})

			naturalMin = 1
			naturalMax = nSamples
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultipleInputsAndOutputsWithDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultipleInputsAndOutputsWithDataSet(ByVal backend As Nd4jBackend)
			SUT.fit(data)
			assertExpectedMinMax()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultipleInputsAndOutputsWithIterator(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMultipleInputsAndOutputsWithIterator(ByVal backend As Nd4jBackend)
			Dim iter As MultiDataSetIterator = New TestMultiDataSetIterator(1, data)
			SUT.fit(iter)
			assertExpectedMinMax()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRevertFeaturesINDArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRevertFeaturesINDArray(ByVal backend As Nd4jBackend)
			SUT.fit(data)

			Dim transformed As MultiDataSet = data.copy()
			SUT.preProcess(transformed)

			Dim reverted As INDArray = transformed.getFeatures(0).dup()
			SUT.revertFeatures(reverted, Nothing, 0)

			assertNotEquals(reverted, transformed.getFeatures(0))

			SUT.revert(transformed)
			assertEquals(reverted, transformed.getFeatures(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRevertLabelsINDArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRevertLabelsINDArray(ByVal backend As Nd4jBackend)
			SUT.fit(data)

			Dim transformed As MultiDataSet = data.copy()
			SUT.preProcess(transformed)

			Dim reverted As INDArray = transformed.getLabels(0).dup()
			SUT.revertLabels(reverted, Nothing, 0)

			assertNotEquals(reverted, transformed.getLabels(0))

			SUT.revert(transformed)
			assertEquals(reverted, transformed.getLabels(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRevertMultiDataSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRevertMultiDataSet(ByVal backend As Nd4jBackend)
			SUT.fit(data)

			Dim transformed As MultiDataSet = data.copy()
			SUT.preProcess(transformed)

			Dim diffBeforeRevert As Double = getMaxRelativeDifference(data, transformed)
			assertTrue(diffBeforeRevert > TOLERANCE_PERC)

			SUT.revert(transformed)

			Dim diffAfterRevert As Double = getMaxRelativeDifference(data, transformed)
			assertTrue(diffAfterRevert < TOLERANCE_PERC)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFullyMaskedData()
		Public Overridable Sub testFullyMaskedData()
			Dim iter As MultiDataSetIterator = New TestMultiDataSetIterator(1, New MultiDataSet(New INDArray() {Nd4j.create(New Single() {1}).reshape(ChrW(1), 1, 1)}, New INDArray() {Nd4j.create(New Single() {2}).reshape(ChrW(1), 1, 1)}), New MultiDataSet(New INDArray() {Nd4j.create(New Single() {2}).reshape(ChrW(1), 1, 1)}, New INDArray() {Nd4j.create(New Single() {4}).reshape(ChrW(1), 1, 1)}, Nothing, New INDArray() {Nd4j.create(New Single() {0}).reshape(ChrW(1), 1)}))

			SUT.fit(iter)

			' The label min value should be 2, as the second row with 4 is masked.
			assertEquals(2f, SUT.getLabelMin(0).getFloat(0), 1e-6)
		End Sub

		Private Function getMaxRelativeDifference(ByVal a As MultiDataSet, ByVal b As MultiDataSet) As Double
			Dim max As Double = 0
			Dim i As Integer = 0
			Do While i < a.Features.Length
				Dim inputA As INDArray = a.Features(i)
				Dim inputB As INDArray = b.Features(i)
				Dim delta As INDArray = Transforms.abs(inputA.sub(inputB)).div(inputB)
				Dim maxdeltaPerc As Double = delta.max(0, 1).mul(100).getDouble(0)
				If maxdeltaPerc > max Then
					max = maxdeltaPerc
				End If
				i += 1
			Loop
			Return max
		End Function

		Private Sub assertExpectedMinMax()
			assertSmallDifference(naturalMin * INPUT1_SCALE, SUT.getMin(0).getDouble(0))
			assertSmallDifference(naturalMax * INPUT1_SCALE, SUT.getMax(0).getDouble(0))

			assertSmallDifference(naturalMin * INPUT2_SCALE, SUT.getMin(1).getDouble(0))
			assertSmallDifference(naturalMax * INPUT2_SCALE, SUT.getMax(1).getDouble(0))

			assertSmallDifference(naturalMin * OUTPUT1_SCALE, SUT.getLabelMin(0).getDouble(0))
			assertSmallDifference(naturalMax * OUTPUT1_SCALE, SUT.getLabelMax(0).getDouble(0))

			assertSmallDifference(naturalMin * OUTPUT2_SCALE, SUT.getLabelMin(1).getDouble(0))
			assertSmallDifference(naturalMax * OUTPUT2_SCALE, SUT.getLabelMax(1).getDouble(0))
		End Sub

		Private Sub assertSmallDifference(ByVal expected As Double, ByVal actual As Double)
			Dim delta As Double = Math.Abs(expected - actual)
			Dim deltaPerc As Double = (delta / expected) * 100
			assertTrue(deltaPerc < TOLERANCE_PERC)
		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace