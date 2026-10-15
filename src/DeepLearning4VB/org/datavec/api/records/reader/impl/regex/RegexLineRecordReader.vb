Imports System
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

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

    ''' <summary>
    ''' Reads lines and splits each one into the capture groups of a regular expression (Java
    ''' <c>Matcher.matches()</c>: the whole line has to match). <c>Pattern</c> is <see cref="Regex"/>.
    ''' </summary>
    <Serializable>
    Public Class RegexLineRecordReader
        Inherits LineRecordReader

        Public Const SKIP_NUM_LINES As String = NAME_SPACE & ".skipnumlines"

        Private regex As String
        Private skipNumLines As Integer
        Private pattern As System.Text.RegularExpressions.Regex
        Private numLinesSkipped As Integer
        Private currLine As Integer = 0

        Public Sub New(ByVal regex As String, ByVal skipNumLines As Integer)
            Me.regex = regex
            Me.skipNumLines = skipNumLines
            Me.pattern = compileFullMatch(regex)
        End Sub

        ''' <summary>A regex that, like Java's <c>Matcher.matches()</c>, only matches the entire input.</summary>
        Friend Shared Function compileFullMatch(ByVal regex As String) As System.Text.RegularExpressions.Regex
            Return New System.Text.RegularExpressions.Regex("\A(?:" & regex & ")\z")
        End Function

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            MyBase.initialize(conf, split)
            Me.skipNumLines = conf.getInt(SKIP_NUM_LINES, Me.skipNumLines)
        End Sub

        Public Overrides Function [next]() As IList(Of writable.Writable)
            If numLinesSkipped < skipNumLines Then
                Dim i As Integer = numLinesSkipped
                Do While i < skipNumLines
                    If Not hasNext() Then
                        Return New List(Of writable.Writable)()
                    End If
                    MyBase.next()
                    i += 1
                    numLinesSkipped += 1
                Loop
            End If
            Dim t As Text = CType(MyBase.next()(0), Text)
            Dim val As String = t.ToString()
            Return parseLine(val)
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            Dim w As writable.Writable = MyBase.record(uri, dataInputStream)(0)
            Return parseLine(w.ToString())
        End Function

        Private Function parseLine(ByVal line As String) As IList(Of writable.Writable)
            Dim m As Match = pattern.Match(line)
            Dim ret As IList(Of writable.Writable)
            If m.Success Then
                Dim count As Integer = m.Groups.Count - 1
                ret = New List(Of writable.Writable)(count)
                For i As Integer = 1 To count 'Note: Matcher.group(0) is the entire sequence; we only care about groups 1 onward
                    ret.Add(New Text(m.Groups(i).Value))
                Next i
            Else
                Throw New InvalidOperationException("Invalid line: line does not match regex (line #" & currLine & ", regex=""" & regex & """; line=""" & line & """")
            End If
            Return ret
        End Function

        Public Overrides Sub reset()
            MyBase.reset()
            numLinesSkipped = 0
        End Sub

        Public Overrides Function nextRecord() As Record
            Dim [next] As IList(Of writable.Writable) = Me.next()
            Dim uri As Uri = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
            Dim meta As RecordMetaData = New RecordMetaDataLine(Me.lineIndex - 1, uri, GetType(RegexLineRecordReader)) '-1 as line number has been incremented already...
            Return New RecordImpl([next], meta)
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Return loadFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

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
