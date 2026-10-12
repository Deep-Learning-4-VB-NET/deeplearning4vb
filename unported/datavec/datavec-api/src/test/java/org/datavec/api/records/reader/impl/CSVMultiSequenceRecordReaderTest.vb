Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVMultiSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVMultiSequenceRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
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
'ORIGINAL LINE: @DisplayName("Csv Multi Sequence Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CSVMultiSequenceRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class CSVMultiSequenceRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Concat Mode") @Disabled void testConcatMode() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testConcatMode()
			For i As Integer = 0 To 2
				Dim seqSep As String
				Dim seqSepRegex As String
				Select Case i
					Case 0
						seqSep = ""
						seqSepRegex = "^$"
					Case 1
						seqSep = "---"
						seqSepRegex = seqSep
					Case 2
						seqSep = "&"
						seqSepRegex = seqSep
					Case Else
						Throw New Exception()
				End Select
				Dim str As String = "a,b,c" & vbLf & "1,2,3,4" & vbLf & "x,y" & vbLf & seqSep & vbLf & "A,B,C"
				Dim f As File = testDir.toFile()
				FileUtils.writeStringToFile(f, str, StandardCharsets.UTF_8)
				Dim seqRR As SequenceRecordReader = New CSVMultiSequenceRecordReader(seqSepRegex, CSVMultiSequenceRecordReader.Mode.CONCAT)
				seqRR.initialize(New org.datavec.api.Split.FileSplit(f))
				Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				For Each s As String In "a,b,c,1,2,3,4,x,y".Split(",", True)
					exp0.Add(Collections.singletonList(Of Writable)(New Text(s)))
				Next s
				Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				For Each s As String In "A,B,C".Split(",", True)
					exp1.Add(Collections.singletonList(Of Writable)(New Text(s)))
				Next s
				assertEquals(exp0, seqRR.sequenceRecord())
				assertEquals(exp1, seqRR.sequenceRecord())
				assertFalse(seqRR.hasNext())
				seqRR.reset()
				assertEquals(exp0, seqRR.sequenceRecord())
				assertEquals(exp1, seqRR.sequenceRecord())
				assertFalse(seqRR.hasNext())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Equal Length") @Disabled void testEqualLength() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testEqualLength()
			For i As Integer = 0 To 2
				Dim seqSep As String
				Dim seqSepRegex As String
				Select Case i
					Case 0
						seqSep = ""
						seqSepRegex = "^$"
					Case 1
						seqSep = "---"
						seqSepRegex = seqSep
					Case 2
						seqSep = "&"
						seqSepRegex = seqSep
					Case Else
						Throw New Exception()
				End Select
				Dim str As String = "a,b" & vbLf & "1,2" & vbLf & "x,y" & vbLf & seqSep & vbLf & "A" & vbLf & "B" & vbLf & "C"
				Dim f As File = testDir.toFile()
				FileUtils.writeStringToFile(f, str, StandardCharsets.UTF_8)
				Dim seqRR As SequenceRecordReader = New CSVMultiSequenceRecordReader(seqSepRegex, CSVMultiSequenceRecordReader.Mode.EQUAL_LENGTH)
				seqRR.initialize(New org.datavec.api.Split.FileSplit(f))
				Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New Text("1"), New Text("x")), Arrays.asList(Of Writable)(New Text("b"), New Text("2"), New Text("y"))}
				Dim exp1 As IList(Of IList(Of Writable)) = Collections.singletonList(Arrays.asList(Of Writable)(New Text("A"), New Text("B"), New Text("C")))
				assertEquals(exp0, seqRR.sequenceRecord())
				assertEquals(exp1, seqRR.sequenceRecord())
				assertFalse(seqRR.hasNext())
				seqRR.reset()
				assertEquals(exp0, seqRR.sequenceRecord())
				assertEquals(exp1, seqRR.sequenceRecord())
				assertFalse(seqRR.hasNext())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Padding") @Disabled void testPadding() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPadding()
			For i As Integer = 0 To 2
				Dim seqSep As String
				Dim seqSepRegex As String
				Select Case i
					Case 0
						seqSep = ""
						seqSepRegex = "^$"
					Case 1
						seqSep = "---"
						seqSepRegex = seqSep
					Case 2
						seqSep = "&"
						seqSepRegex = seqSep
					Case Else
						Throw New Exception()
				End Select
				Dim str As String = "a,b" & vbLf & "1" & vbLf & "x" & vbLf & seqSep & vbLf & "A" & vbLf & "B" & vbLf & "C"
				Dim f As File = testDir.toFile()
				FileUtils.writeStringToFile(f, str, StandardCharsets.UTF_8)
				Dim seqRR As SequenceRecordReader = New CSVMultiSequenceRecordReader(seqSepRegex, CSVMultiSequenceRecordReader.Mode.PAD, New Text("PAD"))
				seqRR.initialize(New org.datavec.api.Split.FileSplit(f))
				Dim exp0 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New Text("1"), New Text("x")), Arrays.asList(Of Writable)(New Text("b"), New Text("PAD"), New Text("PAD"))}
				Dim exp1 As IList(Of IList(Of Writable)) = Collections.singletonList(Arrays.asList(Of Writable)(New Text("A"), New Text("B"), New Text("C")))
				assertEquals(exp0, seqRR.sequenceRecord())
				assertEquals(exp1, seqRR.sequenceRecord())
				assertFalse(seqRR.hasNext())
				seqRR.reset()
				assertEquals(exp0, seqRR.sequenceRecord())
				assertEquals(exp1, seqRR.sequenceRecord())
				assertFalse(seqRR.hasNext())
			Next i
		End Sub
	End Class

End Namespace