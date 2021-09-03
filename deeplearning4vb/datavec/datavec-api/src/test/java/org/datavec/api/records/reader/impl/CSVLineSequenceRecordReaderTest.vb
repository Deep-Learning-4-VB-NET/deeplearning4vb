Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVLineSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVLineSequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Text = org.datavec.api.writable.Text
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
'ORIGINAL LINE: @DisplayName("Csv Line Sequence Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CSVLineSequenceRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class CSVLineSequenceRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test") void test(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub test(ByVal testDir As Path)
			Dim f As File = testDir.toFile()
			Dim source As New File(f, "temp.csv")
			Dim str As String = "a,b,c" & vbLf & "1,2,3,4"
			FileUtils.writeStringToFile(source, str, StandardCharsets.UTF_8)
			Dim rr As SequenceRecordReader = New CSVLineSequenceRecordReader()
			rr.initialize(New org.datavec.api.Split.FileSplit(source))
			Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Collections.singletonList(New Text("a")), Collections.singletonList(New Text("b")), Collections.singletonList(Of Writable)(New Text("c"))}
			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Collections.singletonList(New Text("1")), Collections.singletonList(New Text("2")), Collections.singletonList(Of Writable)(New Text("3")), Collections.singletonList(Of Writable)(New Text("4"))}
			For i As Integer = 0 To 2
				Dim count As Integer = 0
				Do While rr.hasNext()
					Dim [next] As IList(Of IList(Of Writable)) = rr.sequenceRecord()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (count++ == 0)
					If count = 0 Then
							count += 1
						assertEquals(exp0, [next])
					Else
							count += 1
						assertEquals(exp1, [next])
					End If
				Loop
				assertEquals(2, count)
				rr.reset()
			Next i
		End Sub

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return Long.MaxValue
			End Get
		End Property
	End Class

End Namespace