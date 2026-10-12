Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports FileBatchRecordReader = org.datavec.api.records.reader.impl.filebatch.FileBatchRecordReader
Imports FileBatchSequenceRecordReader = org.datavec.api.records.reader.impl.filebatch.FileBatchSequenceRecordReader
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports FileBatch = org.nd4j.common.loader.FileBatch
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend

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
'ORIGINAL LINE: @DisplayName("File Batch Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class FileBatchRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class FileBatchRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Csv") void testCsv(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCsv(ByVal backend As Nd4jBackend)
			' This is an unrealistic use case - one line/record per CSV
			Dim baseDir As File = testDir.toFile()
			Dim fileList As IList(Of File) = New List(Of File)()
			For i As Integer = 0 To 9
				Dim s As String = "file_" & i & "," & i & "," & i
				Dim f As New File(baseDir, "origFile" & i & ".csv")
				FileUtils.writeStringToFile(f, s, StandardCharsets.UTF_8)
				fileList.Add(f)
			Next i
			Dim fb As FileBatch = FileBatch.forFiles(fileList)
			Dim rr As RecordReader = New CSVRecordReader()
			Dim fbrr As New FileBatchRecordReader(rr, fb)
			For test As Integer = 0 To 2
				For i As Integer = 0 To 9
					assertTrue(fbrr.hasNext())
					Dim [next] As IList(Of Writable) = fbrr.next()
					assertEquals(3, [next].Count)
					Dim s1 As String = "file_" & i
					assertEquals(s1, [next](0).ToString())
					assertEquals(i.ToString(), [next](1).ToString())
					assertEquals(i.ToString(), [next](2).ToString())
				Next i
				assertFalse(fbrr.hasNext())
				assertTrue(fbrr.resetSupported())
				fbrr.reset()
			Next test
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @DisplayName("Test Csv Sequence") void testCsvSequence(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCsvSequence(ByVal backend As Nd4jBackend)
			' CSV sequence - 3 lines per file, 10 files
			Dim baseDir As File = testDir.toFile()
			Dim fileList As IList(Of File) = New List(Of File)()
			For i As Integer = 0 To 9
				Dim sb As New StringBuilder()
				For j As Integer = 0 To 2
					If j > 0 Then
						sb.Append(vbLf)
					End If
					sb.Append("file_" & i & "," & i & "," & j)
				Next j
				Dim f As New File(baseDir, "origFile" & i & ".csv")
				FileUtils.writeStringToFile(f, sb.ToString(), StandardCharsets.UTF_8)
				fileList.Add(f)
			Next i
			Dim fb As FileBatch = FileBatch.forFiles(fileList)
			Dim rr As SequenceRecordReader = New CSVSequenceRecordReader()
			Dim fbrr As New FileBatchSequenceRecordReader(rr, fb)
			For test As Integer = 0 To 2
				For i As Integer = 0 To 9
					assertTrue(fbrr.hasNext())
					Dim [next] As IList(Of IList(Of Writable)) = fbrr.sequenceRecord()
					assertEquals(3, [next].Count)
					Dim count As Integer = 0
					For Each [step] As IList(Of Writable) In [next]
						Dim s1 As String = "file_" & i
						assertEquals(s1, [step](0).ToString())
						assertEquals(i.ToString(), [step](1).ToString())
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: assertEquals(String.valueOf(count++), step.get(2).toString());
						assertEquals(count.ToString(), [step](2).ToString())
							count += 1
					Next [step]
				Next i
				assertFalse(fbrr.hasNext())
				assertTrue(fbrr.resetSupported())
				fbrr.reset()
			Next test
		End Sub
	End Class

End Namespace