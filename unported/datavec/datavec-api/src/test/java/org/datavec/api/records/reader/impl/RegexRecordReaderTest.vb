Imports System.Collections.Generic
Imports Record = org.datavec.api.records.Record
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = org.datavec.api.records.metadata.RecordMetaDataLine
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports RegexLineRecordReader = org.datavec.api.records.reader.impl.regex.RegexLineRecordReader
Imports RegexSequenceRecordReader = org.datavec.api.records.reader.impl.regex.RegexSequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
'ORIGINAL LINE: @DisplayName("Regex Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class RegexRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class RegexRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regex Line Record Reader") void testRegexLineRecordReader() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRegexLineRecordReader()
			Dim regex As String = "(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) (\d+) ([A-Z]+) (.*)"
			Dim rr As RecordReader = New RegexLineRecordReader(regex, 1)
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/logtestdata/logtestfile0.txt")).File))
			Dim exp0 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.001"), Writable), New Text("1"), New Text("DEBUG"), New Text("First entry message!")}
			Dim exp1 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.002"), Writable), New Text("2"), New Text("INFO"), New Text("Second entry message!")}
			Dim exp2 As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.003"), Writable), New Text("3"), New Text("WARN"), New Text("Third entry message!")}
			assertEquals(exp0, rr.next())
			assertEquals(exp1, rr.next())
			assertEquals(exp2, rr.next())
			assertFalse(rr.hasNext())
			' Test reset:
			rr.reset()
			assertEquals(exp0, rr.next())
			assertEquals(exp1, rr.next())
			assertEquals(exp2, rr.next())
			assertFalse(rr.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regex Line Record Reader Meta") void testRegexLineRecordReaderMeta() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRegexLineRecordReaderMeta()
			Dim regex As String = "(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) (\d+) ([A-Z]+) (.*)"
			Dim rr As RecordReader = New RegexLineRecordReader(regex, 1)
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/logtestdata/logtestfile0.txt")).File))
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While rr.hasNext()
				list.Add(rr.next())
			Loop
			assertEquals(3, list.Count)
			Dim list2 As IList(Of Record) = New List(Of Record)()
			Dim list3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			rr.reset()
			' Start by skipping 1 line
			Dim count As Integer = 1
			Do While rr.hasNext()
				Dim r As Record = rr.nextRecord()
				list2.Add(r)
				list3.Add(r.getRecord())
				meta.Add(r.MetaData)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(count++, ((org.datavec.api.records.metadata.RecordMetaDataLine) r.getMetaData()).getLineNumber());
				assertEquals(count, DirectCast(r.MetaData, RecordMetaDataLine).getLineNumber())
					count += 1
			Loop
			Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)
			assertEquals(list, list3)
			assertEquals(list2, fromMeta)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regex Sequence Record Reader") void testRegexSequenceRecordReader(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRegexSequenceRecordReader(ByVal testDir As Path)
			Dim regex As String = "(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) (\d+) ([A-Z]+) (.*)"
			Dim cpr As New ClassPathResource("datavec-api/logtestdata/")
			Dim f As File = testDir.toFile()
			cpr.copyDirectory(f)
			Dim path As String = (New File(f, "logtestfile%d.txt")).getAbsolutePath()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 1)
			Dim rr As SequenceRecordReader = New RegexSequenceRecordReader(regex, 1)
			rr.initialize([is])
			Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp0.Add(New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.001"), Writable), New Text("1"), New Text("DEBUG"), New Text("First entry message!")})
			exp0.Add(New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.002"), Writable), New Text("2"), New Text("INFO"), New Text("Second entry message!")})
			exp0.Add(New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.003"), Writable), New Text("3"), New Text("WARN"), New Text("Third entry message!")})
			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			exp1.Add(New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.011"), Writable), New Text("11"), New Text("DEBUG"), New Text("First entry message!")})
			exp1.Add(New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.012"), Writable), New Text("12"), New Text("INFO"), New Text("Second entry message!")})
			exp1.Add(New List(Of Writable) From {DirectCast(New Text("2016-01-01 23:59:59.013"), Writable), New Text("13"), New Text("WARN"), New Text("Third entry message!")})
			assertEquals(exp0, rr.sequenceRecord())
			assertEquals(exp1, rr.sequenceRecord())
			assertFalse(rr.hasNext())
			' Test resetting:
			rr.reset()
			assertEquals(exp0, rr.sequenceRecord())
			assertEquals(exp1, rr.sequenceRecord())
			assertFalse(rr.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Regex Sequence Record Reader Meta") void testRegexSequenceRecordReaderMeta(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRegexSequenceRecordReaderMeta(ByVal testDir As Path)
			Dim regex As String = "(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) (\d+) ([A-Z]+) (.*)"
			Dim cpr As New ClassPathResource("datavec-api/logtestdata/")
			Dim f As File = testDir.toFile()
			cpr.copyDirectory(f)
			Dim path As String = (New File(f, "logtestfile%d.txt")).getAbsolutePath()
			Dim [is] As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.NumberedFileInputSplit(path, 0, 1)
			Dim rr As SequenceRecordReader = New RegexSequenceRecordReader(regex, 1)
			rr.initialize([is])
			Dim [out] As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Do While rr.hasNext()
				[out].Add(rr.sequenceRecord())
			Loop
			assertEquals(2, [out].Count)
			Dim out2 As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim out3 As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			rr.reset()
			Do While rr.hasNext()
				Dim seqr As SequenceRecord = rr.nextSequence()
				out2.Add(seqr.getSequenceRecord())
				out3.Add(seqr)
				meta.Add(seqr.MetaData)
			Loop
			Dim fromMeta As IList(Of SequenceRecord) = rr.loadSequenceFromMetaData(meta)
			assertEquals([out], out2)
			assertEquals(out3, fromMeta)
		End Sub
	End Class

End Namespace