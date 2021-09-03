Imports System.Collections.Generic
Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports InputStreamInputSplit = org.datavec.api.split.InputStreamInputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
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
'ORIGINAL LINE: @DisplayName("Line Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class LineReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class LineReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Line Reader") void testLineReader(@TempDir Path tmpDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLineReader(ByVal tmpDir As Path)
'JAVA TO VB CONVERTER NOTE: The variable tmpdir was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim tmpdir_Conflict As File = tmpDir.toFile()
			If tmpdir_Conflict.exists() Then
				tmpdir_Conflict.delete()
			End If
			tmpdir_Conflict.mkdir()
			Dim tmp1 As New File(FilenameUtils.concat(tmpdir_Conflict.getPath(), "tmp1.txt"))
			Dim tmp2 As New File(FilenameUtils.concat(tmpdir_Conflict.getPath(), "tmp2.txt"))
			Dim tmp3 As New File(FilenameUtils.concat(tmpdir_Conflict.getPath(), "tmp3.txt"))
			FileUtils.writeLines(tmp1, Arrays.asList("1", "2", "3"))
			FileUtils.writeLines(tmp2, Arrays.asList("4", "5", "6"))
			FileUtils.writeLines(tmp3, Arrays.asList("7", "8", "9"))
			Dim split As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(tmpdir_Conflict)
			Dim reader As RecordReader = New LineRecordReader()
			reader.initialize(split)
			Dim count As Integer = 0
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While reader.hasNext()
				Dim l As IList(Of Writable) = reader.next()
				assertEquals(1, l.Count)
				list.Add(l)
				count += 1
			Loop
			assertEquals(9, count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Line Reader Meta Data") void testLineReaderMetaData(@TempDir Path tmpDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLineReaderMetaData(ByVal tmpDir As Path)
'JAVA TO VB CONVERTER NOTE: The variable tmpdir was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim tmpdir_Conflict As File = tmpDir.toFile()
			Dim tmp1 As New File(FilenameUtils.concat(tmpdir_Conflict.getPath(), "tmp1.txt"))
			Dim tmp2 As New File(FilenameUtils.concat(tmpdir_Conflict.getPath(), "tmp2.txt"))
			Dim tmp3 As New File(FilenameUtils.concat(tmpdir_Conflict.getPath(), "tmp3.txt"))
			FileUtils.writeLines(tmp1, Arrays.asList("1", "2", "3"))
			FileUtils.writeLines(tmp2, Arrays.asList("4", "5", "6"))
			FileUtils.writeLines(tmp3, Arrays.asList("7", "8", "9"))
			Dim split As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.FileSplit(tmpdir_Conflict)
			Dim reader As RecordReader = New LineRecordReader()
			reader.initialize(split)
			Dim list As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Do While reader.hasNext()
				list.Add(reader.next())
			Loop
			assertEquals(9, list.Count)
			Dim out2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim out3 As IList(Of Record) = New List(Of Record)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			reader.reset()
			Dim count As Integer = 0
			Do While reader.hasNext()
				Dim r As Record = reader.nextRecord()
				out2.Add(r.getRecord())
				out3.Add(r)
				meta.Add(r.MetaData)
				Dim fileIdx As Integer = count \ 3
				Dim uri As URI = r.MetaData.URI
				assertEquals(uri, split.locations()(fileIdx))
				count += 1
			Loop
			assertEquals(list, out2)
			Dim fromMeta As IList(Of Record) = reader.loadFromMetaData(meta)
			assertEquals(out3, fromMeta)
			' try: second line of second and third files only...
			Dim subsetMeta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			subsetMeta.Add(meta(4))
			subsetMeta.Add(meta(7))
			Dim subset As IList(Of Record) = reader.loadFromMetaData(subsetMeta)
			assertEquals(2, subset.Count)
			assertEquals(out3(4), subset(0))
			assertEquals(out3(7), subset(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Line Reader With Input Stream Input Split") void testLineReaderWithInputStreamInputSplit(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLineReaderWithInputStreamInputSplit(ByVal testDir As Path)
			Dim tmpdir As File = testDir.toFile()
			Dim tmp1 As New File(tmpdir, "tmp1.txt.gz")
			Dim os As Stream = New GZIPOutputStream(New FileStream(tmp1, False))
			IOUtils.writeLines(Arrays.asList("1", "2", "3", "4", "5", "6", "7", "8", "9"), Nothing, os)
			os.Flush()
			os.Close()
			Dim split As org.datavec.api.Split.InputSplit = New org.datavec.api.Split.InputStreamInputSplit(New GZIPInputStream(New FileStream(tmp1, FileMode.Open, FileAccess.Read)))
			Dim reader As RecordReader = New LineRecordReader()
			reader.initialize(split)
			Dim count As Integer = 0
			Do While reader.hasNext()
				assertEquals(1, reader.next().Count)
				count += 1
			Loop
			assertEquals(9, count)
		End Sub
	End Class

End Namespace