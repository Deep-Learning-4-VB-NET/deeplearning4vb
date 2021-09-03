Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Configuration = org.datavec.api.conf.Configuration
Imports SequenceRecord = org.datavec.api.records.SequenceRecord
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = org.datavec.api.records.metadata.RecordMetaDataURI
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports FileRecordReader = org.datavec.api.records.reader.impl.FileRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.datavec.api.records.reader.impl.regex


	<Serializable>
	Public Class RegexSequenceRecordReader
		Inherits FileRecordReader
		Implements SequenceRecordReader

		Public Shared ReadOnly SKIP_NUM_LINES As String = NAME_SPACE & ".skipnumlines"
		Public Shared ReadOnly DEFAULT_CHARSET As Charset = StandardCharsets.UTF_8
		Public Const DEFAULT_ERROR_HANDLING As LineErrorHandling = LineErrorHandling.FailOnInvalid

		''' <summary>
		'''Error handling mode: How should invalid lines (i.e., those that don't match the provided regex) be handled?<br>
		''' FailOnInvalid: Throw an IllegalStateException when an invalid line is found<br>
		''' SkipInvalid: Skip invalid lines (quietly, with no warning)<br>
		''' SkipInvalidWithWarning: Skip invalid lines, but log a warning<br>
		''' </summary>
		Public Enum LineErrorHandling
			FailOnInvalid
			SkipInvalid
			SkipInvalidWithWarning
		End Enum

		Public Shared ReadOnly LOG As Logger = LoggerFactory.getLogger(GetType(RegexSequenceRecordReader))

		Private regex As String
		Private skipNumLines As Integer
		Private pattern As Pattern
		<NonSerialized>
		Private Shadows charset As Charset
		Private errorHandling As LineErrorHandling

		Public Sub New(ByVal regex As String, ByVal skipNumLines As Integer)
			Me.New(regex, skipNumLines, DEFAULT_CHARSET, DEFAULT_ERROR_HANDLING)
		End Sub

		Public Sub New(ByVal regex As String, ByVal skipNumLines As Integer, ByVal encoding As Charset, ByVal errorHandling As LineErrorHandling)
			Me.regex = regex
			Me.skipNumLines = skipNumLines
			Me.pattern = Pattern.compile(regex)
			Me.charset = encoding
			Me.errorHandling = errorHandling
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			MyBase.initialize(conf, split)
			Me.skipNumLines = conf.getInt(SKIP_NUM_LINES, Me.skipNumLines)
		End Sub

		Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
			Return nextSequence().getSequenceRecord()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> sequenceRecord(java.net.URI uri, DataInputStream dataInputStream) throws IOException
		Public Overridable Function sequenceRecord(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable))
			Dim fileContents As String = IOUtils.toString(New BufferedInputStream(dataInputStream), charset.name())
			Return loadSequence(fileContents, uri)
		End Function

		Private Function loadSequence(ByVal fileContents As String, ByVal uri As URI) As IList(Of IList(Of Writable))
			Dim lines() As String = fileContents.Split("(" & vbCrLf & ")|" & vbLf, True) 'TODO this won't work if regex allows for a newline

			Dim numLinesSkipped As Integer = 0
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim lineCount As Integer = 0
			For Each line As String In lines
				lineCount += 1
				If numLinesSkipped < skipNumLines Then
					numLinesSkipped += 1
					Continue For
				End If
				'Split line using regex matcher
				Dim m As Matcher = pattern.matcher(line)
				Dim timeStep As IList(Of Writable)
				If m.matches() Then
					Dim count As Integer = m.groupCount()
					timeStep = New List(Of Writable)(count)
					For i As Integer = 1 To count 'Note: Matcher.group(0) is the entire sequence; we only care about groups 1 onward
						timeStep.Add(New Text(m.group(i)))
					Next i
				Else
					Select Case errorHandling
						Case org.datavec.api.records.reader.impl.regex.RegexSequenceRecordReader.LineErrorHandling.FailOnInvalid
							Throw New System.InvalidOperationException("Invalid line: line does not match regex (line #" & lineCount & ", uri=""" & uri & """), " & """, regex=" & regex & """; line=""" & line & """")
						Case org.datavec.api.records.reader.impl.regex.RegexSequenceRecordReader.LineErrorHandling.SkipInvalid
							Continue For
						Case org.datavec.api.records.reader.impl.regex.RegexSequenceRecordReader.LineErrorHandling.SkipInvalidWithWarning
							Dim warnMsg As String = "Skipping invalid line: line does not match regex (line #" & lineCount & ", uri=""" & uri & """), " & """; line=""" & line & """"
							LOG.warn(warnMsg)
							Continue For
						Case Else
							Throw New Exception("Unknown error handling mode: " & errorHandling)
					End Select
				End If
				[out].Add(timeStep)
			Next line

			Return [out]
		End Function

		Public Overrides Sub reset()
			MyBase.reset()
		End Sub

		Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
			Preconditions.checkState(hasNext(), "No next element available")
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As URI = locationsIterator.next()

			Dim fileContents As String
			Try
					Using s As Stream = streamCreatorFn.apply([next])
					fileContents = IOUtils.toString(s, charset)
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try
			Dim sequence As IList(Of IList(Of Writable)) = loadSequence(fileContents, [next])
			Return New org.datavec.api.records.impl.SequenceRecord(sequence, New RecordMetaDataURI([next], GetType(RegexSequenceRecordReader)))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.SequenceRecord loadSequenceFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
			Return loadSequenceFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.SequenceRecord> loadSequenceFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws IOException
		Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
			Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
			For Each meta As RecordMetaData In recordMetaDatas
				Dim [next] As New File(meta.URI)
				Dim uri As URI = [next].toURI()
				Dim fileContents As String = FileUtils.readFileToString([next], charset.name())
				Dim sequence As IList(Of IList(Of Writable)) = loadSequence(fileContents, uri)
				[out].Add(New org.datavec.api.records.impl.SequenceRecord(sequence, meta))
			Next meta
			Return [out]
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(ObjectInputStream ois) throws ClassNotFoundException, IOException
		Private Sub readObject(ByVal ois As ObjectInputStream)
			ois.defaultReadObject()
			Dim s As String = ois.readUTF()
			charset = Charset.forName(s)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(ObjectOutputStream oos) throws IOException
		Private Sub writeObject(ByVal oos As ObjectOutputStream)
			oos.defaultWriteObject()
			oos.writeUTF(charset.name())
		End Sub
	End Class

End Namespace