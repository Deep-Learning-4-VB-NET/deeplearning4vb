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
Imports System.Linq
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLineInterval = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLineInterval
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports StringIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of String)
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.records.reader.impl.csv

    ''' <summary>A sequence reader where every N consecutive lines of a CSV file form one sequence.</summary>
    <Serializable>
    Public Class CSVNLinesSequenceRecordReader
        Inherits CSVRecordReader
        Implements SequenceRecordReader

        Public Const LINES_PER_SEQUENCE As String = NAME_SPACE & ".nlinespersequence"

        Private nLinesPerSequence As Integer
        Private Shadows delimiter As String

        ''' <summary>
        ''' No-arg constructor with the default number of lines per sequence (10)
        ''' </summary>
        Public Sub New()
            Me.New(10)
        End Sub

        ''' <param name="nLinesPerSequence">    Number of lines in each sequence, use default delemiter(,) between entries in the same line </param>
        Public Sub New(ByVal nLinesPerSequence As Integer)
            Me.New(nLinesPerSequence, 0, CSVRecordReader.DEFAULT_DELIMITER.ToString())
        End Sub

        ''' <param name="nLinesPerSequence">    Number of lines in each sequences </param>
        ''' <param name="skipNumLines">         Number of lines to skip at the start of the file (only skipped once, not per sequence) </param>
        ''' <param name="delimiter">            Delimiter between entries in the same line, for example "," </param>
        Public Sub New(ByVal nLinesPerSequence As Integer, ByVal skipNumLines As Integer, ByVal delimiter As String)
            MyBase.New(skipNumLines)
            Me.delimiter = delimiter
            Me.nLinesPerSequence = nLinesPerSequence
        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            MyBase.initialize(conf, split)
            Me.nLinesPerSequence = conf.getInt(LINES_PER_SEQUENCE, nLinesPerSequence)
        End Sub

        Public Overridable Function sequenceRecord() As IList(Of IList(Of writable.Writable)) Implements SequenceRecordReader.sequenceRecord
            If Not MyBase.hasNext() Then
                Throw New InvalidOperationException("No next element")
            End If

            Dim sequence As IList(Of IList(Of writable.Writable)) = New List(Of IList(Of writable.Writable))()
            Dim count As Integer = 0
            Do While count < nLinesPerSequence AndAlso MyBase.hasNext()
                count += 1
                sequence.Add(MyBase.next())
            Loop

            Return sequence
        End Function

        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of writable.Writable)) Implements SequenceRecordReader.sequenceRecord
            Throw New NotSupportedException("Reading CSV data from DataInputStream not yet implemented")
        End Function

        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            Dim lineBefore As Integer = lineIndex
            Dim record As IList(Of IList(Of writable.Writable)) = sequenceRecord()
            Dim lineAfter As Integer = lineIndex
            Dim uri As Uri = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
            Dim meta As RecordMetaData = New RecordMetaDataLineInterval(lineBefore, lineAfter - 1, uri, GetType(CSVNLinesSequenceRecordReader))
            Return New SequenceRecordImpl(record, meta)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Return loadSequenceFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            'First: create a sorted list of the RecordMetaData
            Dim list As List(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of writable.Writable)))) = New List(Of Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of writable.Writable))))()
            Dim count As Integer = 0
            For Each rmd As RecordMetaData In recordMetaDatas
                If Not (TypeOf rmd Is RecordMetaDataLineInterval) Then
                    Throw New ArgumentException("Invalid metadata; expected RecordMetaDataLineInterval instance; got: " & Objects.toString(rmd))
                End If
                list.Add(New Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of writable.Writable)))(count, DirectCast(rmd, RecordMetaDataLineInterval), New List(Of IList(Of writable.Writable))()))
                count += 1
            Next rmd

            'Sort by starting line number (a stable sort, like Java's Collections.sort):
            list = list.OrderBy(Function(t) t.getSecond().getLineNumberStart()).ToList()

            Dim lineIter As StringIterator = getIterator(0) 'TODO handle multi file case...
            Dim currentLineIdx As Integer = 0
            Dim line As String = lineIter.next()
            Do While currentLineIdx < skipNumLines
                line = lineIter.next()
                currentLineIdx += 1
            Loop
            For Each [next] As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of writable.Writable))) In list
                Dim nextStartLine As Integer = [next].getSecond().getLineNumberStart()
                Dim nextEndLine As Integer = [next].getSecond().getLineNumberEnd()
                Do While currentLineIdx < nextStartLine AndAlso lineIter.hasNext()
                    line = lineIter.next()
                    currentLineIdx += 1
                Loop
                Do While currentLineIdx <= nextEndLine AndAlso (lineIter.hasNext() OrElse currentLineIdx = nextEndLine)
                    Dim split() As String = StringHelper.Split(line, Me.delimiter, True)
                    Dim writables As IList(Of writable.Writable) = New List(Of writable.Writable)()
                    For Each s As String In split
                        writables.Add(New Text(s))
                    Next s
                    [next].getThird().Add(writables)
                    currentLineIdx += 1
                    If lineIter.hasNext() Then
                        line = lineIter.next()
                    End If
                Loop
            Next [next]
            closeIfRequired(lineIter)

            'Now, sort by the original order:
            list.Sort(Function(o1, o2) Ints.compare(o1.getFirst(), o2.getFirst()))

            'And return...
            Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            For Each t As Triple(Of Integer, RecordMetaDataLineInterval, IList(Of IList(Of writable.Writable))) In list
                [out].Add(New SequenceRecordImpl(t.getThird(), t.getSecond()))
            Next t

            Return [out]
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
            Throw New NotSupportedException("Not supported")
        End Function
    End Class

End Namespace
