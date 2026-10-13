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
Imports System.Text.Json.Serialization
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.JavaCompat
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataInterval = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataInterval
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions

Namespace org.datavec.api.records.reader.impl.csv

    ''' <summary>
    ''' A sequence reader for CSV files holding several sequences, separated by lines matching a
    ''' regular expression. In <see cref="Mode.CONCAT"/> mode all values of a sequence's lines are concatenated
    ''' into one univariate sequence; in <see cref="Mode.EQUAL_LENGTH"/> and <see cref="Mode.PAD"/> modes each line is
    ''' one column (time series) and the lines are transposed into time steps, padding short lines with
    ''' the pad value in <see cref="Mode.PAD"/> mode. The separator regex must match a whole line, like Java's
    ''' <c>String.matches</c>.
    ''' </summary>
    <Serializable>
    Public Class CSVMultiSequenceRecordReader
        Inherits CSVRecordReader
        Implements SequenceRecordReader

        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum Mode
            CONCAT
            EQUAL_LENGTH
            PAD
        End Enum

        Private sequenceSeparatorRegex As String
        <NonSerialized>
        Private separatorPattern As Regex
        Private modeValue As Mode
        Private padValue As Writable

        ''' <summary>
        ''' Create a sequence reader using the default value for skip lines (0), the default delimiter (',') and the default
        ''' quote character ('"').<br/>
        ''' Note that this constructor cannot be used with <see cref="Mode.PAD"/> as the padding value cannot be specified
        ''' </summary>
        ''' <param name="sequenceSeparatorRegex"> The sequence separator regex. Use "^$" for "sequences are separated by an empty line </param>
        ''' <param name="mode">                   Mode: see <see cref="CSVMultiSequenceRecordReader"/> javadoc </param>
        Public Sub New(ByVal sequenceSeparatorRegex As String, ByVal mode As Mode)
            Me.New(0, DEFAULT_DELIMITER, DEFAULT_QUOTE, sequenceSeparatorRegex, mode, Nothing)
        End Sub

        ''' <summary>
        ''' Create a sequence reader using the default value for skip lines (0), the default delimiter (',') and the default
        ''' quote character ('"')
        ''' </summary>
        ''' <param name="sequenceSeparatorRegex"> The sequence separator regex. Use "^$" for "sequences are separated by an empty line </param>
        ''' <param name="mode">                   Mode: see <see cref="CSVMultiSequenceRecordReader"/> javadoc </param>
        ''' <param name="padValue">               Padding value for padding short sequences. Only used/allowable with <see cref="Mode.PAD"/>,
        '''                               should be null otherwise </param>
        Public Sub New(ByVal sequenceSeparatorRegex As String, ByVal mode As Mode, ByVal padValue As Writable)
            Me.New(0, DEFAULT_DELIMITER, DEFAULT_QUOTE, sequenceSeparatorRegex, mode, padValue)
        End Sub

        ''' <summary>
        ''' Create a sequence reader using the default value for skip lines (0), the default delimiter (',') and the default
        ''' quote character ('"')
        ''' </summary>
        ''' <param name="skipNumLines">           Number of lines to skip </param>
        ''' <param name="elementDelimiter">       Delimiter for elements - i.e., ',' if lines are comma separated </param>
        ''' <param name="sequenceSeparatorRegex"> The sequence separator regex. Use "^$" for "sequences are separated by an empty line </param>
        ''' <param name="mode">                   Mode: see <see cref="CSVMultiSequenceRecordReader"/> javadoc </param>
        ''' <param name="padValue">               Padding value for padding short sequences. Only used/allowable with <see cref="Mode.PAD"/>,
        '''                               should be null otherwise </param>
        Public Sub New(ByVal skipNumLines As Integer, ByVal elementDelimiter As Char, ByVal quote As Char, ByVal sequenceSeparatorRegex As String, ByVal mode As Mode, ByVal padValue As Writable)
            MyBase.New(skipNumLines, elementDelimiter, quote)
            Preconditions.checkState(mode <> Mode.PAD OrElse padValue IsNot Nothing, "Cannot use Mode.PAD with a null padding value. " & "Padding value must be passed to constructor ")
            Me.sequenceSeparatorRegex = sequenceSeparatorRegex
            Me.modeValue = mode
            Me.padValue = padValue
        End Sub

        ''' <summary>Java <c>String.matches(sequenceSeparatorRegex)</c>: the whole line must match.</summary>
        Private Function isSeparator(ByVal line As String) As Boolean
            If separatorPattern Is Nothing Then
                separatorPattern = New Regex("\A(?:" & sequenceSeparatorRegex & ")\z")
            End If
            Return separatorPattern.IsMatch(line)
        End Function

        Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Return nextSequence().getSequenceRecord()
        End Function

        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            If Not hasNext() Then
                Throw New InvalidOperationException("No next element")
            End If

            Dim lines As IList(Of String) = New List(Of String)()
            Dim firstLine As Integer = lineIndex
            Dim lastLine As Integer = lineIndex
            Do While MyBase.hasNext()
                Dim line As String = readStringLine()
                If isSeparator(line) Then
                    lastLine = lineIndex
                    Exit Do
                End If
                lines.Add(line)
            Loop

            'Process lines
            Dim uri As Uri = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
            Dim [out] As IList(Of IList(Of Writable)) = parseLines(lines, uri, firstLine, lastLine)


            Return New SequenceRecordImpl([out], New RecordMetaDataInterval(firstLine, lastLine, uri))
        End Function

        Private Function parseLines(ByVal lines As IList(Of String), ByVal uri As Uri, ByVal firstLine As Integer, ByVal lastLine As Integer) As IList(Of IList(Of Writable))
            Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
            Select Case modeValue
                Case Mode.CONCAT
                    'Output is univariate sequence - concat all lines
                    For Each s As String In lines
                        Dim parsed As IList(Of Writable) = MyBase.parseLine(s)
                        For Each w As Writable In parsed
                            [out].Add(JavaCollections.singletonList(w))
                        Next w
                    Next s
                Case Mode.EQUAL_LENGTH, Mode.PAD
                    Dim columnWise As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
                    Dim length As Integer = -1
                    Dim lineNum As Integer = 0
                    For Each s As String In lines
                        Dim parsed As IList(Of Writable) = MyBase.parseLine(s) 'This is one COLUMN
                        columnWise.Add(parsed)
                        lineNum += 1
                        If modeValue = Mode.PAD Then
                            length = Math.Max(length, parsed.Count)
                        ElseIf length < 0 Then
                            length = parsed.Count
                        ElseIf modeValue = Mode.EQUAL_LENGTH Then
                            Preconditions.checkState(parsed.Count = length, "Invalid state: When using CSVMultiSequenceRecordReader, " & "all lines (columns) must be the same length. Prior columns had " & length & " elements, line " & lineNum & " in sequence has length " & parsed.Count & " (Sequence position: " & URIUtil.toUriString(uri) & ", lines " & firstLine & " to " & lastLine & ")")
                        End If
                    Next s

                    If modeValue = Mode.PAD Then
                        For Each w As IList(Of Writable) In columnWise
                            Do While w.Count < length
                                w.Add(padValue)
                            Loop
                        Next w
                    End If

                    'Transpose: from column-wise to row-wise
                    For i As Integer = 0 To length - 1
                        Dim [step] As IList(Of Writable) = New List(Of Writable)()
                        For j As Integer = 0 To columnWise.Count - 1
                            [step].Add(columnWise(j)(i))
                        Next j
                        [out].Add([step])
                    Next i
            End Select
            Return [out]
        End Function

        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Dim lines As IList(Of String) = New List(Of String)()
            Using br As New StreamReader(dataInputStream.BaseStream, charsetEncoding(), True, 4096, True)
                Dim line As String
                line = br.ReadLine()
                Do While line IsNot Nothing AndAlso Not isSeparator(line)
                    lines.Add(line)
                    line = br.ReadLine()
                Loop
            End Using

            Return parseLines(lines, uri, 0, lines.Count)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Throw New NotSupportedException("Not yet supported")
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Throw New NotSupportedException("Not yet supported")
        End Function

        Public Overrides Function batchesSupported() As Boolean
            Return False
        End Function
    End Class

End Namespace
