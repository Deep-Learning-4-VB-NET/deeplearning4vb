Imports System
Imports System.Collections.Generic
Imports System.IO
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
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
'ORIGINAL LINE: @DisplayName("Csv Sequence Record Reader Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CSVSequenceRecordReaderTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class CSVSequenceRecordReaderTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path tempDir;
		Public tempDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test") void test() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub test()
			Dim seqReader As New CSVSequenceRecordReader(1, ",")
			seqReader.initialize(New TestInputSplit())
			Dim sequenceCount As Integer = 0
			Do While seqReader.hasNext()
				Dim sequence As IList(Of IList(Of Writable)) = seqReader.sequenceRecord()
				' 4 lines, plus 1 header line
				assertEquals(4, sequence.Count)
				Dim timeStepIter As IEnumerator(Of IList(Of Writable)) = sequence.GetEnumerator()
				Dim lineCount As Integer = 0
				Do While timeStepIter.MoveNext()
					Dim timeStep As IList(Of Writable) = timeStepIter.Current
					assertEquals(3, timeStep.Count)
					Dim lineIter As IEnumerator(Of Writable) = timeStep.GetEnumerator()
					Dim countInLine As Integer = 0
					Do While lineIter.MoveNext()
						Dim entry As Writable = lineIter.Current
						Dim expValue As Integer = 100 * sequenceCount + 10 * lineCount + countInLine
						assertEquals(expValue.ToString(), entry.ToString())
						countInLine += 1
					Loop
					lineCount += 1
				Loop
				sequenceCount += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reset") void testReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReset()
			Dim seqReader As New CSVSequenceRecordReader(1, ",")
			seqReader.initialize(New TestInputSplit())
			Dim nTests As Integer = 5
			For i As Integer = 0 To nTests - 1
				seqReader.reset()
				Dim sequenceCount As Integer = 0
				Do While seqReader.hasNext()
					Dim sequence As IList(Of IList(Of Writable)) = seqReader.sequenceRecord()
					' 4 lines, plus 1 header line
					assertEquals(4, sequence.Count)
					Dim timeStepIter As IEnumerator(Of IList(Of Writable)) = sequence.GetEnumerator()
					Dim lineCount As Integer = 0
					Do While timeStepIter.MoveNext()
						timeStepIter.Current
						lineCount += 1
					Loop
					sequenceCount += 1
					assertEquals(4, lineCount)
				Loop
				assertEquals(3, sequenceCount)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Meta Data") void testMetaData() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testMetaData()
			Dim seqReader As New CSVSequenceRecordReader(1, ",")
			seqReader.initialize(New TestInputSplit())
			Dim l As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Do While seqReader.hasNext()
				Dim sequence As IList(Of IList(Of Writable)) = seqReader.sequenceRecord()
				' 4 lines, plus 1 header line
				assertEquals(4, sequence.Count)
				Dim timeStepIter As IEnumerator(Of IList(Of Writable)) = sequence.GetEnumerator()
				Dim lineCount As Integer = 0
				Do While timeStepIter.MoveNext()
					timeStepIter.Current
					lineCount += 1
				Loop
				assertEquals(4, lineCount)
				l.Add(sequence)
			Loop
			Dim l2 As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
			seqReader.reset()
			Do While seqReader.hasNext()
				Dim sr As SequenceRecord = seqReader.nextSequence()
				l2.Add(sr)
				meta.Add(sr.MetaData)
			Loop
			assertEquals(3, l2.Count)
			Dim fromMeta As IList(Of SequenceRecord) = seqReader.loadSequenceFromMetaData(meta)
			For i As Integer = 0 To 2
				assertEquals(l(i), l2(i).getSequenceRecord())
				assertEquals(l(i), fromMeta(i).getSequenceRecord())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Test Input Split") private static class TestInputSplit implements org.datavec.api.split.InputSplit
		Private Class TestInputSplit
			Implements InputSplit

			Public Overridable Function canWriteToLocation(ByVal location As URI) As Boolean Implements InputSplit.canWriteToLocation
				Return False
			End Function

			Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
				Return Nothing
			End Function

			Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
				Return Nothing
			End Function

			Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
			End Sub

			Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
				Return False
			End Function

			Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.OutputStream openOutputStreamFor(String location) throws Exception
			Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
				Return Nothing
			End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.io.InputStream openInputStreamFor(String location) throws Exception
			Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
				Return Nothing
			End Function

			Public Overridable Function length() As Long Implements InputSplit.length
				Return 3
			End Function

			Public Overridable Function locations() As URI() Implements InputSplit.locations
				Dim arr(2) As URI
				Try
					arr(0) = (New ClassPathResource("datavec-api/csvsequence_0.txt")).File.toURI()
					arr(1) = (New ClassPathResource("datavec-api/csvsequence_1.txt")).File.toURI()
					arr(2) = (New ClassPathResource("datavec-api/csvsequence_2.txt")).File.toURI()
				Catch e As Exception
					Throw New Exception(e)
				End Try
				Return arr
			End Function

			Public Overridable Function locationsIterator() As IEnumerator(Of URI) Implements InputSplit.locationsIterator
				Return Arrays.asList(locations()).GetEnumerator()
			End Function

			Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
				Dim loc() As URI = locations()
				Dim arr(loc.Length - 1) As String
				For i As Integer = 0 To loc.Length - 1
					arr(i) = loc(i).ToString()
				Next i
				Return Arrays.asList(arr).GetEnumerator()
			End Function

			Public Overridable Sub reset() Implements InputSplit.reset
				' No op
			End Sub

			Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
				Return True
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Csv Seq And Numbered File Split") void testCsvSeqAndNumberedFileSplit(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCsvSeqAndNumberedFileSplit(ByVal tempDir As Path)
			Dim baseDir As File = tempDir.toFile()
			' Simple sanity check unit test
			For i As Integer = 0 To 2
				Call (New ClassPathResource(String.Format("csvsequence_{0:D}.txt", i))).getTempFileFromArchive(baseDir)
			Next i
			' Load time series from CSV sequence files; compare to SequenceRecordReaderDataSetIterator
			Dim resource As New ClassPathResource("csvsequence_0.txt")
			Dim featuresPath As String = (New File(baseDir, "csvsequence_%d.txt")).getAbsolutePath()
			Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
			featureReader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(featuresPath, 0, 2))
			Do While featureReader.hasNext()
				featureReader.nextSequence()
			Loop
		End Sub
	End Class

End Namespace