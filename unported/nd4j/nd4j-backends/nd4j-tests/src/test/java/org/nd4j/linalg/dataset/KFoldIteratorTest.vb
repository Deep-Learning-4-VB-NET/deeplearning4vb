Imports System
Imports System.Collections.Generic
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports KFoldIterator = org.nd4j.linalg.dataset.api.iterator.KFoldIterator
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

Namespace org.nd4j.linalg.dataset


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class KFoldIteratorTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class KFoldIteratorTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void checkTestFoldContent(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub checkTestFoldContent(ByVal backend As Nd4jBackend)

			Const numExamples As Integer = 42
			Const numFeatures As Integer = 3
			Dim features As INDArray = Nd4j.rand(New Integer() {numExamples, numFeatures})
			Dim labels As INDArray = Nd4j.linspace(1, numExamples, numExamples, DataType.DOUBLE).reshape(ChrW(-1), 1)

'JAVA TO VB CONVERTER NOTE: The variable dataSet was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim dataSet_Conflict As New DataSet(features, labels)

			For k As Integer = 2 To numExamples
				Dim kFoldIterator As New KFoldIterator(k, dataSet_Conflict)
				Dim testLabels As New HashSet(Of Double)()
				For i As Integer = 0 To k - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					kFoldIterator.next()
					Dim testFold As DataSet = kFoldIterator.testFold()
					For Each testExample As DataSet In testFold
						''' <summary>
						''' Check that the current example has not been in the test set before
						''' </summary>
						Dim testedLabel As INDArray = testExample.Labels
						assertTrue(testLabels.Add(testedLabel.getDouble(0)))
					Next testExample
				Next i
				''' <summary>
				''' Check that the sum of the number of test examples in all folds equals to the number of examples
				''' </summary>
				assertEquals(numExamples, testLabels.Count)
			Next k
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void checkFolds(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub checkFolds(ByVal backend As Nd4jBackend)
			' Expected batch sizes: 3+3+3+2 = 11 total examples
			Dim batchSizesExp() As Integer = {3, 3, 3, 2}
			Dim randomDS As New KBatchRandomDataSet(Me, New Integer() {2, 3}, batchSizesExp)
			Dim allData As DataSet = randomDS.AllBatches
			Dim kiter As New KFoldIterator(4, allData)
			Dim i As Integer = 0
			Do While kiter.MoveNext()
				Dim now As DataSet = kiter.Current
				Dim test As DataSet = kiter.testFold()

				Dim fExp As INDArray = randomDS.getBatchButK(i, True)
				assertEquals(fExp, now.Features)
				Dim lExp As INDArray = randomDS.getBatchButK(i, False)
				assertEquals(lExp, now.Labels)

				assertEquals(randomDS.getBatchK(i, True), test.Features)
				assertEquals(randomDS.getBatchK(i, False), test.Labels)

				assertEquals(batchSizesExp(i), test.Labels.length())
				i += 1
			Loop
			assertEquals(i, 4)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void checkCornerCaseException(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub checkCornerCaseException(ByVal backend As Nd4jBackend)
			assertThrows(GetType(System.ArgumentException),Sub()
			Dim allData As New DataSet(Nd4j.linspace(1,99,99, DataType.DOUBLE).reshape(ChrW(-1), 1), Nd4j.linspace(1,99,99, DataType.DOUBLE).reshape(ChrW(-1), 1))
			Dim k As Integer = 1
			Dim tempVar As New KFoldIterator(k, allData)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void checkCornerCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub checkCornerCase(ByVal backend As Nd4jBackend)
			' Expected batch sizes: 2+1 = 3 total examples
			Dim batchSizesExp() As Integer = {2, 1}
			Dim randomDS As New KBatchRandomDataSet(Me, New Integer() {2, 3}, batchSizesExp)
			Dim allData As DataSet = randomDS.AllBatches
			Dim kiter As New KFoldIterator(2, allData)
			Dim i As Integer = 0
			Do While kiter.MoveNext()
				Dim now As DataSet = kiter.Current
				Dim test As DataSet = kiter.testFold()

				assertEquals(now.Features, randomDS.getBatchButK(i, True))
				assertEquals(now.Labels, randomDS.getBatchButK(i, False))

				assertEquals(randomDS.getBatchK(i, True), test.Features)
				assertEquals(randomDS.getBatchK(i, False), test.Labels)

				assertEquals(batchSizesExp(i), test.Labels.length())
				i += 1
			Loop
			assertEquals(i, 2)
		End Sub


		''' <summary>
		''' Dataset built from given sized batches of random data
		''' @author susaneraly created RandomDataSet
		''' @author Tamas Fenyvesi renamed RandomDataSet to KBatchRandomDataSet (December 2018)
		''' 
		''' </summary>
		Public Class KBatchRandomDataSet
			Private ReadOnly outerInstance As KFoldIteratorTest

			'only one label
			Friend dataShape() As Integer
			Friend dataRank As Integer
			Friend batchSizes() As Integer
'JAVA TO VB CONVERTER NOTE: The field allBatches was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend allBatches_Conflict As DataSet
			Friend allFeatures As INDArray
			Friend allLabels As INDArray
			Friend kBatchFeats() As INDArray
			Friend kBatchLabels() As INDArray

			''' <summary>
			''' Creates a dataset built from given sized batches of random data, with given shape of features and 1D labels </summary>
			''' <param name="dataShape"> shape of features </param>
			''' <param name="batchSizes"> sizes of consecutive batches </param>
			Public Sub New(ByVal outerInstance As KFoldIteratorTest, ByVal dataShape() As Integer, ByVal batchSizes() As Integer)
				Me.outerInstance = outerInstance
				Me.dataShape = dataShape
				Me.dataRank = Me.dataShape.Length
				Me.batchSizes = batchSizes
				Dim eachBatchSize(dataRank) As Integer
				eachBatchSize(0) = 0
				kBatchFeats = New INDArray(batchSizes.Length - 1){}
				kBatchLabels = New INDArray(batchSizes.Length - 1){}
				Array.Copy(dataShape, 0, eachBatchSize, 1, dataRank)
				For i As Integer = 0 To batchSizes.Length - 1
					eachBatchSize(0) = batchSizes(i)
					Dim currentBatchF As INDArray = Nd4j.rand(eachBatchSize)
					Dim currentBatchL As INDArray = Nd4j.rand(batchSizes(i), 1)
					kBatchFeats(i) = currentBatchF
					kBatchLabels(i) = currentBatchL
					If i = 0 Then
						allFeatures = currentBatchF.dup()
						allLabels = currentBatchL.dup()
					Else
						allFeatures = Nd4j.vstack(allFeatures, currentBatchF).dup()
						allLabels = Nd4j.vstack(allLabels, currentBatchL).dup()
					End If
				Next i
				allBatches_Conflict = New DataSet(allFeatures, allLabels.reshape(ChrW(-1), 1))
			End Sub

			Public Overridable ReadOnly Property AllBatches As DataSet
				Get
					Return allBatches_Conflict
				End Get
			End Property

			''' <summary>
			''' Get features or labels for batch k </summary>
			''' <param name="k"> index of batch </param>
			''' <param name="feat"> true if we want to get features, false if we want to get labels </param>
			Public Overridable Function getBatchK(ByVal k As Integer, ByVal feat As Boolean) As INDArray
				Return If(feat, kBatchFeats(k), kBatchLabels(k))
			End Function

			''' <summary>
			''' Get features or labels for all batches except for k </summary>
			''' <param name="k"> index of excluded batch </param>
			''' <param name="feat"> true if we want to get features, false if we want to get labels </param>
			Public Overridable Function getBatchButK(ByVal k As Integer, ByVal feat As Boolean) As INDArray
				Dim batches As INDArray = Nothing
				Dim notInit As Boolean = True
				For i As Integer = 0 To batchSizes.Length - 1
					If i = k Then
						Continue For
					End If
					If notInit Then
						batches = getBatchK(i, feat)
						notInit = False
					Else
						batches = Nd4j.vstack(batches, getBatchK(i, feat)).dup()
					End If
				Next i
				Return batches
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void test5974(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub test5974(ByVal backend As Nd4jBackend)
			Dim ds As New DataSet(Nd4j.linspace(1,99,99, DataType.DOUBLE).reshape(ChrW(-1), 1), Nd4j.linspace(1,99,99, DataType.DOUBLE).reshape(ChrW(-1), 1))

			Dim iter As New KFoldIterator(10, ds)

			Dim count As Integer = 0
			Do While iter.MoveNext()
				Dim fold As DataSet = iter.Current
				Dim testFold As INDArray
				Dim countTrain As Integer
				If count < 9 Then
					'Folds 0 to 8: should have 10 examples for test
					testFold = Nd4j.linspace(10*count+1, 10*count+10, 10, DataType.DOUBLE).reshape(ChrW(-1), 1)
					countTrain = 99 - 10
				Else
					'Fold 9 should have 9 examples for test
					testFold = Nd4j.linspace(10*count+1, 10*count+9, 9, DataType.DOUBLE).reshape(ChrW(-1), 1)
					countTrain = 99-9
				End If
				Dim s As String = count.ToString()
				Dim test As DataSet = iter.testFold()
				assertEquals(testFold, test.Features,s)
				assertEquals(testFold, test.Labels,s)
				assertEquals(countTrain, fold.Features.length(),s)
				assertEquals(countTrain, fold.Labels.length(),s)
				count += 1
			Loop
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace