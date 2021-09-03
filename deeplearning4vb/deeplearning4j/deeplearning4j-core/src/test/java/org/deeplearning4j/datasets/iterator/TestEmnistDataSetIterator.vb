Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports EmnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.EmnistDataSetIterator
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_ETL) @Disabled("Fails on an edge case (last test batch?)") public class TestEmnistDataSetIterator extends org.deeplearning4j.BaseDL4JTest
	Public Class TestEmnistDataSetIterator
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testEmnistDataSetIterator() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEmnistDataSetIterator()
			Dim batchSize As Integer = 128

			Dim sets() As EmnistDataSetIterator.Set
			If IntegrationTests Then
				sets = System.Enum.GetValues(GetType(EmnistDataSetIterator.Set))
			Else
				sets = New EmnistDataSetIterator.Set(){EmnistDataSetIterator.Set.MNIST, EmnistDataSetIterator.Set.LETTERS}
			End If

			For Each s As EmnistDataSetIterator.Set In sets
				Dim isBalanced As Boolean = EmnistDataSetIterator.isBalanced(s)
				Dim numLabels As Integer = EmnistDataSetIterator.numLabels(s)
				Dim labelCounts As INDArray = Nothing
				For Each train As Boolean In New Boolean() {True, False}
					If isBalanced AndAlso train Then
						labelCounts = Nd4j.create(numLabels)
					Else
						labelCounts = Nothing
					End If

					log.info("Starting test: {}, {}", s, (If(train, "train", "test")))
					Dim iter As New EmnistDataSetIterator(s, batchSize, train, 12345)

					assertTrue(iter.asyncSupported())
					assertTrue(iter.resetSupported())

					Dim expNumExamples As Integer
					If train Then
						expNumExamples = EmnistDataSetIterator.numExamplesTrain(s)
					Else
						expNumExamples = EmnistDataSetIterator.numExamplesTest(s)
					End If



					assertEquals(numLabels, iter.getLabels().Count)
					assertEquals(numLabels, iter.LabelsArrays.Length)

					Dim labelArr() As Char = iter.LabelsArrays
					For Each c As Char In labelArr
						Dim isExpected As Boolean = (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c)
						assertTrue(isExpected)
					Next c

					Dim totalCount As Integer = 0
					Do While iter.MoveNext()
						Dim ds As DataSet = iter.Current
						assertNotNull(ds.Features)
						assertNotNull(ds.Labels)
						assertEquals(ds.Features.size(0), ds.Labels.size(0))

						totalCount += ds.Features.size(0)

						assertEquals(784, ds.Features.size(1))
						assertEquals(numLabels, ds.Labels.size(1))

						If isBalanced AndAlso train Then
							labelCounts.addi(ds.Labels.sum(0))
						End If
					Loop

					assertEquals(expNumExamples, totalCount)

					If isBalanced AndAlso train Then
						Dim min As Integer = labelCounts.minNumber().intValue()
						Dim max As Integer = labelCounts.maxNumber().intValue()
						Dim exp As Integer = expNumExamples \ numLabels

						assertTrue(min > 0)
						assertEquals(exp, min)
						assertEquals(exp, max)
					End If
				Next train
			Next s
		End Sub
	End Class

End Namespace