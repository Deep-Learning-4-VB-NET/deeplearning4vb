Imports System.Collections.Generic
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVNLinesSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVNLinesSequenceRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
import static org.junit.jupiter.api.Assertions.assertEquals
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
'ORIGINAL LINE: @DisplayName("Csvn Lines Sequence Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CSVNLinesSequenceRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class CSVNLinesSequenceRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CSV Lines Sequence Record Reader") void testCSVNLinesSequenceRecordReader() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCSVNLinesSequenceRecordReader()
			Dim nLinesPerSequence As Integer = 10
			Dim seqRR As SequenceRecordReader = New CSVNLinesSequenceRecordReader(nLinesPerSequence)
			seqRR.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim rr As New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim count As Integer = 0
			Do While seqRR.hasNext()
				Dim [next] As IList(Of IList(Of Writable)) = seqRR.sequenceRecord()
				Dim expected As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				For i As Integer = 0 To nLinesPerSequence - 1
					expected.Add(rr.next())
				Next i
				assertEquals(10, [next].Count)
				assertEquals(expected, [next])
				count += 1
			Loop
			assertEquals(150 \ nLinesPerSequence, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CSV Nlines Sequence Record Reader Meta Data") void testCSVNlinesSequenceRecordReaderMetaData() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCSVNlinesSequenceRecordReaderMetaData()
			Dim nLinesPerSequence As Integer = 10
			Dim seqRR As SequenceRecordReader = New CSVNLinesSequenceRecordReader(nLinesPerSequence)
			seqRR.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim rr As New CSVRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
			Dim [out] As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Do While seqRR.hasNext()
				Dim [next] As IList(Of IList(Of Writable)) = seqRR.sequenceRecord()
				[out].Add([next])
			Loop
			seqRR.reset()
			Dim out2 As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim out3 As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			Do While seqRR.hasNext()
				Dim seq As SequenceRecord = seqRR.nextSequence()
				out2.Add(seq.getSequenceRecord())
				meta.Add(seq.MetaData)
				out3.Add(seq)
			Loop
			assertEquals([out], out2)
			Dim out4 As IList(Of SequenceRecord) = seqRR.loadSequenceFromMetaData(meta)
			assertEquals(out3, out4)
		End Sub
	End Class

End Namespace