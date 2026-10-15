Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.slf4j
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataURI
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports FileRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.FileRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions

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
    ''' Reads each file of the split as a sequence: one time step per line, each line split into the capture
    ''' groups of a regular expression (Java <c>Matcher.matches()</c>: the whole line has to match).
    ''' <c>Charset</c> is <see cref="Encoding"/> and is kept as the charset name in the base class
    ''' (<see cref="FileRecordReader.charset"/>), which is how it is serialized.
    ''' </summary>
    <Serializable>
    Public Class RegexSequenceRecordReader
        Inherits FileRecordReader
        Implements SequenceRecordReader

        Public Const SKIP_NUM_LINES As String = NAME_SPACE & ".skipnumlines"
        Public Shared ReadOnly DEFAULT_CHARSET As Encoding = Encoding.UTF8
        Public Const DEFAULT_ERROR_HANDLING As LineErrorHandling = LineErrorHandling.FailOnInvalid

        ''' <summary>
        '''Error handling mode: How should invalid lines (i.e., those that don't match the provided regex) be handled?<br/>
        ''' FailOnInvalid: Throw an IllegalStateException when an invalid line is found<br/>
        ''' SkipInvalid: Skip invalid lines (quietly, with no warning)<br/>
        ''' SkipInvalidWithWarning: Skip invalid lines, but log a warning<br/>
        ''' </summary>
        Public Enum LineErrorHandling
            FailOnInvalid
            SkipInvalid
            SkipInvalidWithWarning
        End Enum

        Public Shared ReadOnly LOG As Logger = LoggerFactory.getLogger(GetType(RegexSequenceRecordReader))

        Private regex As String
        Private skipNumLines As Integer
        Private pattern As Regex
        Private errorHandling As LineErrorHandling

        Public Sub New(ByVal regex As String, ByVal skipNumLines As Integer)
            Me.New(regex, skipNumLines, DEFAULT_CHARSET, DEFAULT_ERROR_HANDLING)
        End Sub

        Public Sub New(ByVal regex As String, ByVal skipNumLines As Integer, ByVal encoding As Encoding, ByVal errorHandling As LineErrorHandling)
            Me.regex = regex
            Me.skipNumLines = skipNumLines
            Me.pattern = RegexLineRecordReader.compileFullMatch(regex)
            Me.charset = If(encoding, DEFAULT_CHARSET).WebName
            Me.errorHandling = errorHandling
        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            MyBase.initialize(conf, split)
            Me.skipNumLines = conf.getInt(SKIP_NUM_LINES, Me.skipNumLines)
        End Sub

        Public Overridable Function sequenceRecord() As IList(Of IList(Of writable.Writable)) Implements SequenceRecordReader.sequenceRecord
            Return nextSequence().getSequenceRecord()
        End Function

        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of writable.Writable)) Implements SequenceRecordReader.sequenceRecord
            Dim fileContents As String
            Using reader As New StreamReader(dataInputStream.BaseStream, charsetEncoding(), True, 4096, True)
                fileContents = reader.ReadToEnd()
            End Using
            Return loadSequence(fileContents, uri)
        End Function

        Private Function loadSequence(ByVal fileContents As String, ByVal uri As Uri) As IList(Of IList(Of writable.Writable))
            Dim lines() As String = StringHelper.Split(fileContents, "(?:" & vbCrLf & ")|" & vbLf, True) 'TODO this won't work if regex allows for a newline
            Dim numLinesSkipped As Integer = 0
            Dim [out] As IList(Of IList(Of writable.Writable)) = New List(Of IList(Of writable.Writable))()
            Dim lineCount As Integer = 0
            For Each line As String In lines
                lineCount += 1
                If numLinesSkipped < skipNumLines Then
                    numLinesSkipped += 1
                    Continue For
                End If
                'Split line using regex matcher
                Dim m As Match = pattern.Match(line)
                Dim timeStep As IList(Of writable.Writable)
                If m.Success Then
                    Dim count As Integer = m.Groups.Count - 1
                    timeStep = New List(Of writable.Writable)(count)
                    For i As Integer = 1 To count 'Note: Matcher.group(0) is the entire sequence; we only care about groups 1 onward
                        timeStep.Add(New Text(m.Groups(i).Value))
                    Next i
                Else
                    Select Case errorHandling
                        Case LineErrorHandling.FailOnInvalid
                            Throw New InvalidOperationException("Invalid line: line does not match regex (line #" & lineCount & ", uri=""" & URIUtil.toUriString(uri) & """), " & """, regex=" & regex & """; line=""" & line & """")
                        Case LineErrorHandling.SkipInvalid
                            Continue For
                        Case LineErrorHandling.SkipInvalidWithWarning
                            Dim warnMsg As String = "Skipping invalid line: line does not match regex (line #" & lineCount & ", uri=""" & URIUtil.toUriString(uri) & """), " & """; line=""" & line & """"
                            LOG.warn(warnMsg)
                            Continue For
                        Case Else
                            Throw New Exception("Unknown error handling mode: " & errorHandling.ToString())
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
            Dim [next] As Uri = locationsIterator.next()
            Dim fileContents As String
            Try
                Using s As Stream = streamCreatorFn.apply([next])
                    Using reader As New StreamReader(s, charsetEncoding())
                        fileContents = reader.ReadToEnd()
                    End Using
                End Using
            Catch e As IOException
                Throw New Exception(e.Message, e)
            End Try

            Dim sequence As IList(Of IList(Of writable.Writable)) = loadSequence(fileContents, [next])
            Return New SequenceRecordImpl(sequence, New RecordMetaDataURI([next], GetType(RegexSequenceRecordReader)))
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Return loadSequenceFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            For Each meta As RecordMetaData In recordMetaDatas
                Dim uri As Uri = meta.URI
                Dim fileContents As String = File.ReadAllText(URIUtil.toFilePath(uri), charsetEncoding())
                Dim sequence As IList(Of IList(Of writable.Writable)) = loadSequence(fileContents, uri)
                [out].Add(New SequenceRecordImpl(sequence, meta))
            Next meta

            Return [out]
        End Function

    End Class

End Namespace
