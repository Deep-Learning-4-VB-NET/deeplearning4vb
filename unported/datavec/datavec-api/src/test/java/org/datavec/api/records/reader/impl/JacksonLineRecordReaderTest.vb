Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports FieldSelection = org.datavec.api.records.reader.impl.jackson.FieldSelection
Imports JacksonLineRecordReader = org.datavec.api.records.reader.impl.jackson.JacksonLineRecordReader
Imports JacksonLineSequenceRecordReader = org.datavec.api.records.reader.impl.jackson.JacksonLineSequenceRecordReader
Imports CollectionInputSplit = org.datavec.api.split.CollectionInputSplit
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports JsonFactory = org.nd4j.shade.jackson.core.JsonFactory
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
import static org.junit.jupiter.api.Assertions.assertEquals
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
Namespace org.datavec.api.records.reader.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Jackson Line Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class JacksonLineRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class JacksonLineRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

		Public Sub New()
		End Sub

		Private Shared ReadOnly Property FieldSelection As FieldSelection
			Get
				Return (New FieldSelection.Builder()).addField("value1").addField("value2").addField("value3").addField("value4").addField("value5").addField("value6").addField("value7").addField("value8").addField("value9").addField("value10").build()
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Read JSON") void testReadJSON() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReadJSON()
			Dim rr As RecordReader = New JacksonLineRecordReader(FieldSelection, New ObjectMapper(New JsonFactory()))
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("datavec-api/json/json_test_3.txt")).File))
			testJacksonRecordReader(rr)
		End Sub

		Private Shared Sub testJacksonRecordReader(ByVal rr As RecordReader)
			Do While rr.hasNext()
				Dim json0 As IList(Of Writable) = rr.next()
				' System.out.println(json0);
				Debug.Assert((json0.Count > 0))
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Jackson Line Sequence Record Reader") void testJacksonLineSequenceRecordReader(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJacksonLineSequenceRecordReader(ByVal testDir As Path)
			Dim dir As File = testDir.toFile()
			Call (New ClassPathResource("datavec-api/JacksonLineSequenceRecordReaderTest/")).copyDirectory(dir)
			Dim f As FieldSelection = (New FieldSelection.Builder()).addField("a").addField(New Text("MISSING_B"), "b").addField(New Text("MISSING_CX"), "c", "x").build()
			Dim rr As New JacksonLineSequenceRecordReader(f, New ObjectMapper(New JsonFactory()))
			Dim files() As File = dir.listFiles()
			Array.Sort(files)
			Dim u(files.Length - 1) As URI
			For i As Integer = 0 To files.Length - 1
				u(i) = files(i).toURI()
			Next i
			rr.initialize(New org.datavec.api.Split.CollectionInputSplit(u))
			Dim expSeq0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expSeq0.Add(New List(Of Writable) From {DirectCast(New Text("aValue0"), Writable), New Text("bValue0"), New Text("cxValue0")})
			expSeq0.Add(New List(Of Writable) From {DirectCast(New Text("aValue1"), Writable), New Text("MISSING_B"), New Text("cxValue1")})
			expSeq0.Add(New List(Of Writable) From {DirectCast(New Text("aValue2"), Writable), New Text("bValue2"), New Text("MISSING_CX")})
			Dim expSeq1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expSeq1.Add(New List(Of Writable) From {DirectCast(New Text("aValue3"), Writable), New Text("bValue3"), New Text("cxValue3")})
			Dim count As Integer = 0
			Do While rr.hasNext()
				Dim [next] As IList(Of IList(Of Writable)) = rr.sequenceRecord()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (count++ == 0)
				If count = 0 Then
						count += 1
					assertEquals(expSeq0, [next])
				Else
						count += 1
					assertEquals(expSeq1, [next])
				End If
			Loop
			assertEquals(2, count)
		End Sub
	End Class

End Namespace