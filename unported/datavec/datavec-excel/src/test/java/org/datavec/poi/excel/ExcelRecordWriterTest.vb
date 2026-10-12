Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports FileSplit = org.datavec.api.split.FileSplit
Imports NumberOfRecordsPartitioner = org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports Schema = org.datavec.api.transform.schema.Schema
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
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
Namespace org.datavec.poi.excel

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Excel Record Writer Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) class ExcelRecordWriterTest
	Friend Class ExcelRecordWriterTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Writer") void testWriter() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWriter()
'JAVA TO VB CONVERTER NOTE: The variable excelRecordWriter was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim excelRecordWriter_Conflict As New ExcelRecordWriter()
			Dim records As val = Me.records()
			Dim tmpDir As File = testDir.toFile()
			Dim outputFile As New File(tmpDir, "testexcel.xlsx")
			outputFile.deleteOnExit()
			Dim fileSplit As New org.datavec.api.Split.FileSplit(outputFile)
			excelRecordWriter_Conflict.initialize(fileSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
			excelRecordWriter_Conflict.writeBatch(records.getRight())
			excelRecordWriter_Conflict.Dispose()
			Dim parentFile As File = outputFile.getParentFile()
			assertEquals(1, parentFile.list().length)
'JAVA TO VB CONVERTER NOTE: The variable excelRecordReader was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim excelRecordReader_Conflict As New ExcelRecordReader()
			excelRecordReader_Conflict.initialize(fileSplit)
			Dim [next] As IList(Of IList(Of Writable)) = excelRecordReader_Conflict.next(10)
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