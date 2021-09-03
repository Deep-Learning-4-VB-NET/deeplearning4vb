Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports SVMLightRecordReader = org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertThrows

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
Namespace org.datavec.api.records.reader.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Svm Light Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class SVMLightRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class SVMLightRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Basic Record") void testBasicRecord() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testBasicRecord()
			Dim correct As IDictionary(Of Integer, IList(Of Writable)) = New Dictionary(Of Integer, IList(Of Writable))()
			' 7 2:1 4:2 6:3 8:4 10:5
			correct(0) = New List(Of Writable) From {ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), New IntWritable(7)}
			' 2 qid:42 1:0.1 2:2 6:6.6 8:80
			correct(1) = New List(Of Writable) From {
				New DoubleWritable(0.1),
				New DoubleWritable(2),
				ZERO,
				ZERO,
				ZERO,
				New DoubleWritable(6.6),
				ZERO,
				New DoubleWritable(80),
				ZERO,
				ZERO,
				New IntWritable(2)
			}
			' 33
			correct(2) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, New IntWritable(33)}
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setBoolean(ZERO_BASED_INDEXING, False)
			config.setInt(NUM_FEATURES, 10)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/basic.txt")).File))
			Dim i As Integer = 0
			Do While rr.hasNext()
				Dim record As IList(Of Writable) = rr.next()
				assertEquals(correct(i), record)
				i += 1
			Loop
			assertEquals(i, correct.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test No Append Label") void testNoAppendLabel() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNoAppendLabel()
			Dim correct As IDictionary(Of Integer, IList(Of Writable)) = New Dictionary(Of Integer, IList(Of Writable))()
			' 7 2:1 4:2 6:3 8:4 10:5
			correct(0) = New List(Of Writable) From {ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5)}
			' 2 qid:42 1:0.1 2:2 6:6.6 8:80
			correct(1) = New List(Of Writable) From {
				New DoubleWritable(0.1),
				New DoubleWritable(2),
				ZERO,
				ZERO,
				ZERO,
				New DoubleWritable(6.6),
				ZERO,
				New DoubleWritable(80),
				ZERO,
				ZERO
			}
			' 33
			correct(2) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO}
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setBoolean(ZERO_BASED_INDEXING, False)
			config.setInt(NUM_FEATURES, 10)
			config.setBoolean(SVMLightRecordReader.APPEND_LABEL, False)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/basic.txt")).File))
			Dim i As Integer = 0
			Do While rr.hasNext()
				Dim record As IList(Of Writable) = rr.next()
				assertEquals(correct(i), record)
				i += 1
			Loop
			assertEquals(i, correct.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test No Label") void testNoLabel() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNoLabel()
			Dim correct As IDictionary(Of Integer, IList(Of Writable)) = New Dictionary(Of Integer, IList(Of Writable))()
			' 2:1 4:2 6:3 8:4 10:5
			correct(0) = New List(Of Writable) From {ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5)}
			' qid:42 1:0.1 2:2 6:6.6 8:80
			correct(1) = New List(Of Writable) From {
				New DoubleWritable(0.1),
				New DoubleWritable(2),
				ZERO,
				ZERO,
				ZERO,
				New DoubleWritable(6.6),
				ZERO,
				New DoubleWritable(80),
				ZERO,
				ZERO
			}
			' 1:1.0
			correct(2) = New List(Of Writable) From {
				New DoubleWritable(1.0),
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO
			}
			' 
			correct(3) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO}
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setBoolean(ZERO_BASED_INDEXING, False)
			config.setInt(NUM_FEATURES, 10)
			config.setBoolean(SVMLightRecordReader.APPEND_LABEL, True)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/noLabels.txt")).File))
			Dim i As Integer = 0
			Do While rr.hasNext()
				Dim record As IList(Of Writable) = rr.next()
				assertEquals(correct(i), record)
				i += 1
			Loop
			assertEquals(i, correct.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multioutput Record") void testMultioutputRecord() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultioutputRecord()
			Dim correct As IDictionary(Of Integer, IList(Of Writable)) = New Dictionary(Of Integer, IList(Of Writable))()
			' 7 2.45,9 2:1 4:2 6:3 8:4 10:5
			correct(0) = New List(Of Writable) From {ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), New IntWritable(7), New DoubleWritable(2.45), New IntWritable(9)}
			' 2,3,4 qid:42 1:0.1 2:2 6:6.6 8:80
			correct(1) = New List(Of Writable) From {
				New DoubleWritable(0.1),
				New DoubleWritable(2),
				ZERO,
				ZERO,
				ZERO,
				New DoubleWritable(6.6),
				ZERO,
				New DoubleWritable(80),
				ZERO,
				ZERO,
				New IntWritable(2),
				New IntWritable(3),
				New IntWritable(4)
			}
			' 33,32.0,31.9
			correct(2) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, New IntWritable(33), New DoubleWritable(32.0), New DoubleWritable(31.9)}
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setBoolean(ZERO_BASED_INDEXING, False)
			config.setInt(NUM_FEATURES, 10)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/multioutput.txt")).File))
			Dim i As Integer = 0
			Do While rr.hasNext()
				Dim record As IList(Of Writable) = rr.next()
				assertEquals(correct(i), record)
				i += 1
			Loop
			assertEquals(i, correct.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multilabel Record") void testMultilabelRecord() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultilabelRecord()
			Dim correct As IDictionary(Of Integer, IList(Of Writable)) = New Dictionary(Of Integer, IList(Of Writable))()
			' 1,3 2:1 4:2 6:3 8:4 10:5
			correct(0) = New List(Of Writable) From {ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), LABEL_ONE, LABEL_ZERO, LABEL_ONE, LABEL_ZERO}
			' 2 qid:42 1:0.1 2:2 6:6.6 8:80
			correct(1) = New List(Of Writable) From {
				New DoubleWritable(0.1),
				New DoubleWritable(2),
				ZERO,
				ZERO,
				ZERO,
				New DoubleWritable(6.6),
				ZERO,
				New DoubleWritable(80),
				ZERO,
				ZERO,
				LABEL_ZERO,
				LABEL_ONE,
				LABEL_ZERO,
				LABEL_ZERO
			}
			' 1,2,4
			correct(2) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ONE, LABEL_ONE, LABEL_ZERO, LABEL_ONE}
			' 1:1.0
			correct(3) = New List(Of Writable) From {
				New DoubleWritable(1.0),
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				ZERO,
				LABEL_ZERO,
				LABEL_ZERO,
				LABEL_ZERO,
				LABEL_ZERO
			}
			' 
			correct(4) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO}
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setBoolean(ZERO_BASED_INDEXING, False)
			config.setInt(NUM_FEATURES, 10)
			config.setBoolean(MULTILABEL, True)
			config.setInt(NUM_LABELS, 4)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/multilabel.txt")).File))
			Dim i As Integer = 0
			Do While rr.hasNext()
				Dim record As IList(Of Writable) = rr.next()
				assertEquals(correct(i), record)
				i += 1
			Loop
			assertEquals(i, correct.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Zero Based Indexing") void testZeroBasedIndexing() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testZeroBasedIndexing()
			Dim correct As IDictionary(Of Integer, IList(Of Writable)) = New Dictionary(Of Integer, IList(Of Writable))()
			' 1,3 2:1 4:2 6:3 8:4 10:5
			correct(0) = New List(Of Writable) From {ZERO, ZERO, ONE, ZERO, New DoubleWritable(2), ZERO, New DoubleWritable(3), ZERO, New DoubleWritable(4), ZERO, New DoubleWritable(5), LABEL_ZERO, LABEL_ONE, LABEL_ZERO, LABEL_ONE, LABEL_ZERO}
			' 2 qid:42 1:0.1 2:2 6:6.6 8:80
			correct(1) = New List(Of Writable) From {ZERO, New DoubleWritable(0.1), New DoubleWritable(2), ZERO, ZERO, ZERO, New DoubleWritable(6.6), ZERO, New DoubleWritable(80), ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ONE, LABEL_ZERO, LABEL_ZERO}
			' 1,2,4
			correct(2) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ONE, LABEL_ONE, LABEL_ZERO, LABEL_ONE}
			' 1:1.0
			correct(3) = New List(Of Writable) From {ZERO, New DoubleWritable(1.0), ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO}
			' 
			correct(4) = New List(Of Writable) From {ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO, LABEL_ZERO}
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			' Zero-based indexing is default
			' NOT STANDARD!
			config.setBoolean(ZERO_BASED_LABEL_INDEXING, True)
			config.setInt(NUM_FEATURES, 11)
			config.setBoolean(MULTILABEL, True)
			config.setInt(NUM_LABELS, 5)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/multilabel.txt")).File))
			Dim i As Integer = 0
			Do While rr.hasNext()
				Dim record As IList(Of Writable) = rr.next()
				assertEquals(correct(i), record)
				i += 1
			Loop
			assertEquals(i, correct.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next Record") void testNextRecord() throws IOException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextRecord()
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setBoolean(ZERO_BASED_INDEXING, False)
			config.setInt(NUM_FEATURES, 10)
			config.setBoolean(SVMLightRecordReader.APPEND_LABEL, False)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/basic.txt")).File))
			Dim record As Record = rr.nextRecord()
			Dim recordList As IList(Of Writable) = record.getRecord()
			assertEquals(New DoubleWritable(1.0), recordList(1))
			assertEquals(New DoubleWritable(3.0), recordList(5))
			assertEquals(New DoubleWritable(4.0), recordList(7))
			record = rr.nextRecord()
			recordList = record.getRecord()
			assertEquals(New DoubleWritable(0.1), recordList(0))
			assertEquals(New DoubleWritable(6.6), recordList(5))
			assertEquals(New DoubleWritable(80.0), recordList(7))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test No Such Element Exception") void testNoSuchElementException()
		Friend Overridable Sub testNoSuchElementException()
			assertThrows(GetType(NoSuchElementException), Sub()
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setInt(NUM_FEATURES, 11)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/basic.txt")).File))
			Do While rr.hasNext()
				rr.next()
			Loop
			rr.next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Failed To Set Num Features Exception") void failedToSetNumFeaturesException()
		Friend Overridable Sub failedToSetNumFeaturesException()
			assertThrows(GetType(System.NotSupportedException), Sub()
		Dim rr As New SVMLightRecordReader()
		Dim config As New Configuration()
		rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/basic.txt")).File))
		Do While rr.hasNext()
			rr.next()
		Loop
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Inconsistent Num Labels Exception") void testInconsistentNumLabelsException()
		Friend Overridable Sub testInconsistentNumLabelsException()
			assertThrows(GetType(System.NotSupportedException), Sub()
		Dim rr As New SVMLightRecordReader()
		Dim config As New Configuration()
		config.setBoolean(ZERO_BASED_INDEXING, False)
		rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/inconsistentNumLabels.txt")).File))
		Do While rr.hasNext()
			rr.next()
		Loop
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Failed To Set Num Multiabels Exception") void failedToSetNumMultiabelsException()
		Friend Overridable Sub failedToSetNumMultiabelsException()
			assertThrows(GetType(System.NotSupportedException), Sub()
		Dim rr As New SVMLightRecordReader()
		Dim config As New Configuration()
		rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/multilabel.txt")).File))
		Do While rr.hasNext()
			rr.next()
		Loop
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Feature Index Exceeds Num Features") void testFeatureIndexExceedsNumFeatures()
		Friend Overridable Sub testFeatureIndexExceedsNumFeatures()
			assertThrows(GetType(System.IndexOutOfRangeException), Sub()
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setInt(NUM_FEATURES, 9)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/basic.txt")).File))
			rr.next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Label Index Exceeds Num Labels") void testLabelIndexExceedsNumLabels()
		Friend Overridable Sub testLabelIndexExceedsNumLabels()
			assertThrows(GetType(System.IndexOutOfRangeException), Sub()
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setInt(NUM_FEATURES, 10)
			config.setInt(NUM_LABELS, 6)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/basic.txt")).File))
			rr.next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Zero Index Feature Without Using Zero Indexing") void testZeroIndexFeatureWithoutUsingZeroIndexing()
		Friend Overridable Sub testZeroIndexFeatureWithoutUsingZeroIndexing()
			assertThrows(GetType(System.IndexOutOfRangeException), Sub()
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setBoolean(ZERO_BASED_INDEXING, False)
			config.setInt(NUM_FEATURES, 10)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/zeroIndexFeature.txt")).File))
			rr.next()
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Zero Index Label Without Using Zero Indexing") void testZeroIndexLabelWithoutUsingZeroIndexing()
		Friend Overridable Sub testZeroIndexLabelWithoutUsingZeroIndexing()
			assertThrows(GetType(System.IndexOutOfRangeException), Sub()
			Dim rr As New SVMLightRecordReader()
			Dim config As New Configuration()
			config.setInt(NUM_FEATURES, 10)
			config.setBoolean(MULTILABEL, True)
			config.setInt(NUM_LABELS, 2)
			rr.initialize(config, New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/svmlight/zeroIndexLabel.txt")).File))
			rr.next()
			End Sub)
		End Sub
	End Class

End Namespace