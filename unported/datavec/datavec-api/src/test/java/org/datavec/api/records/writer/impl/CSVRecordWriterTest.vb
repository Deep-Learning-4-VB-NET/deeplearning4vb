Imports System.Collections.Generic
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVRecordWriter = org.datavec.api.records.writer.impl.csv.CSVRecordWriter
Imports FileSplit = org.datavec.api.split.FileSplit
Imports NumberOfRecordsPartitioner = org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
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
Namespace org.datavec.api.records.writer.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Csv Record Writer Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CSVRecordWriterTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class CSVRecordWriterTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub setUp()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Write") void testWrite() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWrite()
			Dim tempFile As File = File.createTempFile("datavec", "writer")
			tempFile.deleteOnExit()
			Dim fileSplit As New org.datavec.api.Split.FileSplit(tempFile)
			Dim writer As New CSVRecordWriter()
			writer.initialize(fileSplit, New org.datavec.api.Split.partition.NumberOfRecordsPartitioner())
			Dim collection As IList(Of Writable) = New List(Of Writable)()
			collection.Add(New Text("12"))
			collection.Add(New Text("13"))
			collection.Add(New Text("14"))
			writer.write(collection)
			Dim reader As New CSVRecordReader(0)
			reader.initialize(New org.datavec.api.Split.FileSplit(tempFile))
			Dim cnt As Integer = 0
			Do While reader.hasNext()
				Dim line As IList(Of Writable) = New List(Of Writable)(reader.next())
				assertEquals(3, line.Count)
				assertEquals(12, line(0).toInt())
				assertEquals(13, line(1).toInt())
				assertEquals(14, line(2).toInt())
				cnt += 1
			Loop
			assertEquals(1, cnt)
		End Sub
	End Class

End Namespace