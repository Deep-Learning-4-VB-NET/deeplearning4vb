Imports System.Collections.Generic
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports TestDataSetConsumer = org.deeplearning4j.nn.util.TestDataSetConsumer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resources = org.nd4j.common.resources.Resources
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
'ORIGINAL LINE: @DisplayName("Multiple Epochs Iterator Test") @NativeTag @Tag(TagNames.FILE_IO) class MultipleEpochsIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class MultipleEpochsIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next And Reset") void testNextAndReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextAndReset()
			Dim epochs As Integer = 3
			Dim rr As RecordReader = New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 150)
			Dim multiIter As New MultipleEpochsIterator(epochs, iter)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(multiIter.hasNext())
			Do While multiIter.MoveNext()
				Dim path As DataSet = multiIter.Current
				assertFalse(path Is Nothing)
			Loop
			assertEquals(epochs, multiIter.epochs)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Load Full Data Set") void testLoadFullDataSet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLoadFullDataSet()
			Dim epochs As Integer = 3
			Dim rr As RecordReader = New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit(Resources.asFile("iris.txt")))
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 150)
			Dim ds As DataSet = iter.next(50)
			assertEquals(50, ds.Features.size(0))
			Dim multiIter As New MultipleEpochsIterator(epochs, ds)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(multiIter.hasNext())
			Dim count As Integer = 0
			Do While multiIter.MoveNext()
				Dim path As DataSet = multiIter.Current
				assertNotNull(path)
				assertEquals(50, path.numExamples(), 0)
				count += 1
			Loop
			assertEquals(epochs, count)
			assertEquals(epochs, multiIter.epochs)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Load Batch Data Set") void testLoadBatchDataSet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLoadBatchDataSet()
			Dim epochs As Integer = 2
			Dim rr As RecordReader = New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("iris.txt")).File))
			Dim iter As DataSetIterator = New RecordReaderDataSetIterator(rr, 150, 4, 3)
			Dim ds As DataSet = iter.next(20)
			assertEquals(20, ds.Features.size(0))
			Dim multiIter As New MultipleEpochsIterator(epochs, ds)
			Do While multiIter.MoveNext()
				Dim path As DataSet = multiIter.next(10)
				assertNotNull(path)
				assertEquals(10, path.numExamples(), 0.0)
			Loop
			assertEquals(epochs, multiIter.epochs)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MEDI With Load 1") void testMEDIWithLoad1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMEDIWithLoad1()
			Dim iter As New ExistingDataSetIterator(New IterableWithoutException(Me, 100))
			Dim iterator As New MultipleEpochsIterator(10, iter, 24)
			Dim consumer As New TestDataSetConsumer(iterator, 1)
			Dim num As Long = consumer.consumeWhileHasNext(True)
			assertEquals(10 * 100, num)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MEDI With Load 2") void testMEDIWithLoad2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMEDIWithLoad2()
			Dim iter As New ExistingDataSetIterator(New IterableWithoutException(Me, 100))
			Dim iterator As New MultipleEpochsIterator(10, iter, 24)
			Dim consumer As New TestDataSetConsumer(iterator, 2)
			Dim num1 As Long = 0
			Do While num1 < 150
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				consumer.consumeOnce(iterator.next(), True)
				num1 += 1
			Loop
			iterator.reset()
			Dim num2 As Long = consumer.consumeWhileHasNext(True)
			assertEquals((10 * 100) + 150, num1 + num2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MEDI With Load 3") void testMEDIWithLoad3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMEDIWithLoad3()
			Dim iter As New ExistingDataSetIterator(New IterableWithoutException(Me, 10000))
			Dim iterator As New MultipleEpochsIterator(iter, 24, 136)
			Dim consumer As New TestDataSetConsumer(iterator, 2)
			Dim num1 As Long = 0
			Do While iterator.MoveNext()
				consumer.consumeOnce(iterator.Current, True)
				num1 += 1
			Loop
			assertEquals(136, num1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Iterable Without Exception") private class IterableWithoutException implements Iterable<org.nd4j.linalg.dataset.DataSet>
		Private Class IterableWithoutException
			Implements IEnumerable(Of DataSet)

			Private ReadOnly outerInstance As MultipleEpochsIteratorTest


			Friend ReadOnly counter As New AtomicLong(0)

			Friend ReadOnly datasets As Integer

			Public Sub New(ByVal outerInstance As MultipleEpochsIteratorTest, ByVal datasets As Integer)
				Me.outerInstance = outerInstance
				Me.datasets = datasets
			End Sub

			Public Overridable Function GetEnumerator() As IEnumerator(Of DataSet) Implements IEnumerator(Of DataSet).GetEnumerator
				counter.set(0)
				Return New IteratorAnonymousInnerClass(Me)
			End Function

			Private Class IteratorAnonymousInnerClass
				Implements IEnumerator(Of DataSet)

				Private ReadOnly outerInstance As IterableWithoutException

				Public Sub New(ByVal outerInstance As IterableWithoutException)
					Me.outerInstance = outerInstance
				End Sub


				Public Function hasNext() As Boolean
					Return outerInstance.counter.get() < outerInstance.datasets
				End Function

				Public Function [next]() As DataSet
					outerInstance.counter.incrementAndGet()
					Return New DataSet(Nd4j.create(100), Nd4j.create(10))
				End Function

				Public Sub remove()
				End Sub
			End Class
		End Class
	End Class

End Namespace