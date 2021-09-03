Imports System
Imports System.Collections.Generic
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = org.datavec.api.records.metadata.RecordMetaDataLine
Imports LineRecordReader = org.datavec.api.records.reader.impl.LineRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable

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
	Public Class RegexLineRecordReader
		Inherits LineRecordReader

		Public Shared ReadOnly SKIP_NUM_LINES As String = NAME_SPACE & ".skipnumlines"

		Private regex As String
		Private skipNumLines As Integer
		Private pattern As Pattern
		Private numLinesSkipped As Integer
		Private currLine As Integer = 0

		Public Sub New(ByVal regex As String, ByVal skipNumLines As Integer)
			Me.regex = regex
			Me.skipNumLines = skipNumLines
			Me.pattern = Pattern.compile(regex)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			MyBase.initialize(conf, split)
			Me.skipNumLines = conf.getInt(SKIP_NUM_LINES, Me.skipNumLines)
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
			If numLinesSkipped < skipNumLines Then
				Dim i As Integer = numLinesSkipped
				Do While i < skipNumLines
					If Not hasNext() Then
						Return New List(Of Writable)()
					End If
					MyBase.next()
					i += 1
					numLinesSkipped += 1
				Loop
			End If
			Dim t As Text = CType(MyBase.next().GetEnumerator().next(), Text)
			Dim val As String = t.ToString()
			Return parseLine(val)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Dim w As Writable = MyBase.record(uri, dataInputStream)(0)
			Return parseLine(w.ToString())
		End Function

		Private Function parseLine(ByVal line As String) As IList(Of Writable)
			Dim m As Matcher = pattern.matcher(line)

			Dim ret As IList(Of Writable)
			If m.matches() Then
				Dim count As Integer = m.groupCount()
				ret = New List(Of Writable)(count)
				For i As Integer = 1 To count 'Note: Matcher.group(0) is the entire sequence; we only care about groups 1 onward
					ret.Add(New Text(m.group(i)))
				Next i
			Else
				Throw New System.InvalidOperationException("Invalid line: line does not match regex (line #" & currLine & ", regex=""" & regex & """; line=""" & line & """")
			End If

			Return ret
		End Function

		Public Overrides Sub reset()
			MyBase.reset()
			numLinesSkipped = 0
		End Sub

		Public Overrides Function nextRecord() As Record
			Dim [next] As IList(Of Writable) = Me.next()
			Dim uri As URI = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
			Dim meta As RecordMetaData = New RecordMetaDataLine(Me.lineIndex - 1, uri, GetType(RegexLineRecordReader)) '-1 as line number has been incremented already...
			Return New org.datavec.api.records.impl.Record([next], meta)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Return loadFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Dim list As IList(Of Record) = MyBase.loadFromMetaData(recordMetaDatas)

			For Each r As Record In list
				Dim line As String = r.getRecord()(0).ToString()
				r.Record = parseLine(line)
			Next r

			Return list
		End Function
	End Class

End Namespace