Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports FileUtils = org.apache.commons.io.FileUtils
Imports RecordMapper = org.datavec.api.records.mapper.RecordMapper
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVRecordWriter = org.datavec.api.records.writer.impl.csv.CSVRecordWriter
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports NumberOfRecordsPartitioner = org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports Schema = org.datavec.api.transform.schema.Schema
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports ArrowRecordReader = org.datavec.arrow.recordreader.ArrowRecordReader
Imports ArrowRecordWriter = org.datavec.arrow.recordreader.ArrowRecordWriter
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports org.nd4j.common.primitives
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
Namespace org.datavec.arrow

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Record Mapper Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class RecordMapperTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class RecordMapperTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Write") void testMultiWrite() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMultiWrite()
			Dim recordsPair As val = records()
			Dim p As Path = Files.createTempFile("arrowwritetest", ".arrow")
			FileUtils.write(p.toFile(), recordsPair.getFirst())
			p.toFile().deleteOnExit()
			Dim numReaders As Integer = 2
			Dim readers(numReaders - 1) As RecordReader
			Dim splits(numReaders - 1) As org.datavec.api.Split.InputSplit
			For i As Integer = 0 To readers.Length - 1
				Dim split As New org.datavec.api.Split.FileSplit(p.toFile())
				Dim arrowRecordReader As New ArrowRecordReader()
				readers(i) = arrowRecordReader
				splits(i) = split
			Next i
			Dim arrowRecordWriter As New ArrowRecordWriter(recordsPair.getMiddle())
			Dim split As New org.datavec.api.Split.FileSplit(p.toFile())
			arrowRecordWriter.initialize(split, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
			arrowRecordWriter.writeBatch(recordsPair.getRight())
			Dim csvRecordWriter As New CSVRecordWriter()
			Dim p2 As Path = Files.createTempFile("arrowwritetest", ".csv")
			FileUtils.write(p2.toFile(), recordsPair.getFirst())
			p.toFile().deleteOnExit()
			Dim outputCsv As New org.datavec.api.Split.FileSplit(p2.toFile())
			Dim mapper As RecordMapper = RecordMapper.builder().batchSize(10).inputUrl(split).outputUrl(outputCsv).partitioner(New org.datavec.api.Split.partition.NumberOfRecordsPartitioner()).readersToConcat(readers).splitPerReader(splits).recordWriter(csvRecordWriter).build()
			mapper.copy()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Copy From Arrow To Csv") void testCopyFromArrowToCsv() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCopyFromArrowToCsv()
			Dim recordsPair As val = records()
			Dim p As Path = Files.createTempFile("arrowwritetest", ".arrow")
			FileUtils.write(p.toFile(), recordsPair.getFirst())
			p.toFile().deleteOnExit()
			Dim arrowRecordWriter As New ArrowRecordWriter(recordsPair.getMiddle())
			Dim split As New org.datavec.api.Split.FileSplit(p.toFile())
			arrowRecordWriter.initialize(split, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
			arrowRecordWriter.writeBatch(recordsPair.getRight())
			Dim arrowRecordReader As New ArrowRecordReader()
			arrowRecordReader.initialize(split)
			Dim csvRecordWriter As New CSVRecordWriter()
			Dim p2 As Path = Files.createTempFile("arrowwritetest", ".csv")
			FileUtils.write(p2.toFile(), recordsPair.getFirst())
			p.toFile().deleteOnExit()
			Dim outputCsv As New org.datavec.api.Split.FileSplit(p2.toFile())
			Dim mapper As RecordMapper = RecordMapper.builder().batchSize(10).inputUrl(split).outputUrl(outputCsv).partitioner(New org.datavec.api.Split.partition.NumberOfRecordsPartitioner()).recordReader(arrowRecordReader).recordWriter(csvRecordWriter).build()
			mapper.copy()
			Dim recordReader As New CSVRecordReader()
			recordReader.initialize(outputCsv)
			Dim loadedCSvRecords As IList(Of IList(Of Writable)) = recordReader.next(10)
			assertEquals(10, loadedCSvRecords.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Copy From Csv To Arrow") void testCopyFromCsvToArrow() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCopyFromCsvToArrow()
			Dim recordsPair As val = records()
			Dim p As Path = Files.createTempFile("csvwritetest", ".csv")
			FileUtils.write(p.toFile(), recordsPair.getFirst())
			p.toFile().deleteOnExit()
			Dim recordReader As New CSVRecordReader()
			Dim fileSplit As New org.datavec.api.Split.FileSplit(p.toFile())
			Dim arrowRecordWriter As New ArrowRecordWriter(recordsPair.getMiddle())
			Dim outputFile As File = Files.createTempFile("outputarrow", "arrow").toFile()
			Dim outputFileSplit As New org.datavec.api.Split.FileSplit(outputFile)
			Dim mapper As RecordMapper = RecordMapper.builder().batchSize(10).inputUrl(fileSplit).outputUrl(outputFileSplit).partitioner(New org.datavec.api.Split.partition.NumberOfRecordsPartitioner()).recordReader(recordReader).recordWriter(arrowRecordWriter).build()
			mapper.copy()
			Dim arrowRecordReader As New ArrowRecordReader()
			arrowRecordReader.initialize(outputFileSplit)
			Dim [next] As IList(Of IList(Of Writable)) = arrowRecordReader.next(10)
			Console.WriteLine([next])
			assertEquals(10, [next].Count)
		End Sub

		Private Function records() As Triple(Of String, Schema, IList(Of IList(Of Writable)))
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim sb As New StringBuilder()
			Dim numColumns As Integer = 3
			For i As Integer = 0 To 9
				Dim temp As IList(Of Writable) = New List(Of Writable)()
				For j As Integer = 0 To numColumns - 1
					Dim v As Integer = 100 * i + j
					temp.Add(New IntWritable(v))
					sb.Append(v)
					If j < 2 Then
						sb.Append(",")
					ElseIf i <> 9 Then
						sb.Append(vbLf)
					End If
				Next j
				list.Add(temp)
			Next i
			Dim schemaBuilder As New Schema.Builder()
			For i As Integer = 0 To numColumns - 1
				schemaBuilder.addColumnInteger(i.ToString())
			Next i
			Return Triple.of(sb.ToString(), schemaBuilder.build(), list)
		End Function
	End Class

End Namespace