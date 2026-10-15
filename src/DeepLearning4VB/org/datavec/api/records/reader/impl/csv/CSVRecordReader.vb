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

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions

Namespace org.datavec.api.records.reader.impl.csv

    ''' <summary>Reads CSV files line by line, splitting each line into <see cref="Text"/> values.</summary>
    <Serializable>
    Public Class CSVRecordReader
        Inherits LineRecordReader

        Private skippedLines As Boolean = False
        Protected Friend skipNumLines As Integer = 0
        Public Const DEFAULT_DELIMITER As Char = ","c
        Public Const DEFAULT_QUOTE As Char = """"c
        Public Const SKIP_NUM_LINES As String = NAME_SPACE & ".skipnumlines"
        Public Const DELIMITER As String = NAME_SPACE & ".delimiter"
        Public Const QUOTE As String = NAME_SPACE & ".quote"

        Private csvParser As SerializableCSVParser

        ''' <summary>
        ''' Skip first n lines </summary>
        ''' <param name="skipNumLines"> the number of lines to skip </param>
        Public Sub New(ByVal skipNumLines As Integer)
            Me.New(skipNumLines, DEFAULT_DELIMITER)
        End Sub

        ''' <summary>
        ''' Create a CSVRecordReader with the specified delimiter </summary>
        ''' <param name="delimiter"> Delimiter character for CSV </param>
        Public Sub New(ByVal delimiter As Char)
            Me.New(0, delimiter)
        End Sub

        ''' <summary>
        ''' Skip lines and use delimiter </summary>
        ''' <param name="skipNumLines"> the number of lines to skip </param>
        ''' <param name="delimiter"> the delimiter </param>
        Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As Char)
            Me.New(skipNumLines, delimiter, """"c)
        End Sub

        ''' <param name="skipNumLines"> Number of lines to skip </param>
        ''' <param name="delimiter">    Delimiter to use </param>
        <Obsolete("This constructor is deprecated; use CSVRecordReader(Integer, Char) or CSVRecordReader(Integer, Char, Char)")>
        Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As String)
            Me.New(skipNumLines, stringDelimToChar(delimiter))
        End Sub

        Private Shared Function stringDelimToChar(ByVal delimiter As String) As Char
            If delimiter.Length > 1 Then
                Throw New NotSupportedException("Multi-character delimiters have been deprecated. For quotes, " & "use CSVRecordReader(int skipNumLines, char delimiter, char quote)")
            End If
            Return delimiter.Chars(0)
        End Function

        ''' <summary>
        ''' Skip lines, use delimiter, and strip quotes </summary>
        ''' <param name="skipNumLines"> the number of lines to skip </param>
        ''' <param name="delimiter"> the delimiter </param>
        ''' <param name="quote"> the quote to strip </param>
        Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As Char, ByVal quote As Char)
            Me.skipNumLines = skipNumLines
            Me.csvParser = New SerializableCSVParser(delimiter, quote)
        End Sub

        ''' <summary>
        ''' Skip lines, use delimiter, and strip quotes </summary>
        ''' <param name="skipNumLines"> the number of lines to skip </param>
        ''' <param name="delimiter"> the delimiter </param>
        ''' <param name="quote"> the quote to strip </param>
        <Obsolete("This constructor is deprecated; use CSVRecordReader(Integer, Char) or CSVRecordReader(Integer, Char, Char)")>
        Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As String, ByVal quote As String)
            Me.New(skipNumLines, stringDelimToChar(delimiter), stringDelimToChar(quote))
        End Sub

        Public Sub New()
            Me.New(0, DEFAULT_DELIMITER)
        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            MyBase.initialize(conf, split)
            Me.skipNumLines = conf.getInt(SKIP_NUM_LINES, Me.skipNumLines)
            Me.csvParser = New SerializableCSVParser(conf.getChar(DELIMITER, DEFAULT_DELIMITER), conf.getChar(QUOTE, DEFAULT_QUOTE))
        End Sub

        Private Function skipLines() As Boolean
            If Not skippedLines AndAlso skipNumLines > 0 Then
                For i As Integer = 0 To skipNumLines - 1
                    If Not MyBase.hasNext() Then
                        Return False
                    End If
                    MyBase.next()
                Next i
                skippedLines = True
            End If
            Return True
        End Function

        Public Overrides Function batchesSupported() As Boolean
            Return True
        End Function

        Public Overrides Function hasNext() As Boolean
            Return skipLines() AndAlso MyBase.hasNext()
        End Function

        Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of writable.Writable))
            Dim ret As IList(Of IList(Of writable.Writable)) = New List(Of IList(Of writable.Writable))(Math.Min(num, 10000))
            Dim recordsRead As Integer = 0
            Do While hasNext() AndAlso recordsRead < num
                recordsRead += 1
                ret.Add([next]())
            Loop

            Return ret
        End Function

        Public Overrides Function [next]() As IList(Of writable.Writable)
            If Not skipLines() Then
                Throw New InvalidOperationException("No next element found!")
            End If
            Dim val As String = readStringLine()
            Return parseLine(val)
        End Function

        Protected Friend Overridable Function parseLine(ByVal line As String) As IList(Of writable.Writable)
            Dim split() As String
            Try
                split = csvParser.parseLine(line)
            Catch e As IOException
                Throw New Exception(e.Message, e)
            End Try
            Dim ret As IList(Of writable.Writable) = New List(Of writable.Writable)()
            For Each s As String In split
                ret.Add(New Text(s))
            Next s
            Return ret
        End Function

        Protected Friend Overridable Function readStringLine() As String
            Preconditions.checkState(initialized, "RecordReader has not been initialized before use")
            Dim t As Text = CType(MyBase.next()(0), Text)
            Return t.ToString()
        End Function

        Public Overrides Function nextRecord() As Record
            Dim [next] As IList(Of writable.Writable) = Me.next()
            Dim uri As Uri = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
            Dim meta As RecordMetaData = New RecordMetaDataLine(Me.lineIndex - 1, uri, GetType(CSVRecordReader)) '-1 as line number has been incremented already...
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

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            Dim br As New StreamReader(dataInputStream.BaseStream, charsetEncoding(), True, 4096, True)
            For i As Integer = 0 To skipNumLines - 1
                br.ReadLine()
            Next i
            Dim line As String = br.ReadLine()
            Return parseLine(line)
        End Function

        Public Overrides Sub reset()
            MyBase.reset()
            skippedLines = False
        End Sub

        Protected Friend Overrides Sub onLocationOpen(ByVal location As Uri)
            skippedLines = False
        End Sub
    End Class

End Namespace
