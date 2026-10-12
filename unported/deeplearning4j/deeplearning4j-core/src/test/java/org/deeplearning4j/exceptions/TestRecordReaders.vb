Imports System
Imports System.Collections.Generic
Imports CollectionRecordReader = org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports CollectionSequenceRecordReader = org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports RecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderDataSetIterator
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
import static junit.framework.TestCase.fail
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

Namespace org.deeplearning4j.exceptions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) public class TestRecordReaders extends org.deeplearning4j.BaseDL4JTest
	Public Class TestRecordReaders
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClassIndexOutsideOfRangeRRDSI()
		Public Overridable Sub testClassIndexOutsideOfRangeRRDSI()
			Dim c As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			c.Add(Arrays.asList(New DoubleWritable(0.5), New IntWritable(0)))
			c.Add(Arrays.asList(New DoubleWritable(1.0), New IntWritable(2)))

			Dim crr As New CollectionRecordReader(c)

			Dim iter As New RecordReaderDataSetIterator(crr, 2, 1, 2)

			Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = iter.next()
				fail("Expected exception")
			Catch e As Exception
				assertTrue(e.Message.contains("to one-hot"),e.Message)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClassIndexOutsideOfRangeRRMDSI()
		Public Overridable Sub testClassIndexOutsideOfRangeRRMDSI()

			Dim c As ICollection(Of ICollection(Of ICollection(Of Writable))) = New List(Of ICollection(Of ICollection(Of Writable)))()
			Dim seq1 As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			seq1.Add(Arrays.asList(New DoubleWritable(0.0), New IntWritable(0)))
			seq1.Add(Arrays.asList(New DoubleWritable(0.0), New IntWritable(1)))
			c.Add(seq1)

			Dim seq2 As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			seq2.Add(Arrays.asList(New DoubleWritable(0.0), New IntWritable(0)))
			seq2.Add(Arrays.asList(New DoubleWritable(0.0), New IntWritable(2)))
			c.Add(seq2)

			Dim csrr As New CollectionSequenceRecordReader(c)
			Dim dsi As DataSetIterator = New SequenceRecordReaderDataSetIterator(csrr, 2, 2, 1)

			Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = dsi.next()
				fail("Expected exception")
			Catch e As Exception
				assertTrue(e.Message.contains("to one-hot"),e.Message)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClassIndexOutsideOfRangeRRMDSI_MultipleReaders()
		Public Overridable Sub testClassIndexOutsideOfRangeRRMDSI_MultipleReaders()

			Dim c1 As ICollection(Of ICollection(Of ICollection(Of Writable))) = New List(Of ICollection(Of ICollection(Of Writable)))()
			Dim seq1 As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			seq1.Add(Arrays.asList(New DoubleWritable(0.0)))
			seq1.Add(Arrays.asList(New DoubleWritable(0.0)))
			c1.Add(seq1)

			Dim seq2 As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			seq2.Add(Arrays.asList(New DoubleWritable(0.0)))
			seq2.Add(Arrays.asList(New DoubleWritable(0.0)))
			c1.Add(seq2)

			Dim c2 As ICollection(Of ICollection(Of ICollection(Of Writable))) = New List(Of ICollection(Of ICollection(Of Writable)))()
			Dim seq1a As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			seq1a.Add(Arrays.asList(New IntWritable(0)))
			seq1a.Add(Arrays.asList(New IntWritable(1)))
			c2.Add(seq1a)

			Dim seq2a As ICollection(Of ICollection(Of Writable)) = New List(Of ICollection(Of Writable))()
			seq2a.Add(Arrays.asList(New IntWritable(0)))
			seq2a.Add(Arrays.asList(New IntWritable(2)))
			c2.Add(seq2a)

			Dim csrr As New CollectionSequenceRecordReader(c1)
			Dim csrrLabels As New CollectionSequenceRecordReader(c2)
			Dim dsi As DataSetIterator = New SequenceRecordReaderDataSetIterator(csrr, csrrLabels, 2, 2)

			Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = dsi.next()
				fail("Expected exception")
			Catch e As Exception
				assertTrue(e.Message.contains("to one-hot"),e.Message)
			End Try
		End Sub

	End Class

End Namespace