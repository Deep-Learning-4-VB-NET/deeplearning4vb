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
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLineInterval = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLineInterval
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl.csv

    ''' <summary>
    ''' A sequence reader that slides a window of at most <c>maxLinesPerSequence</c> lines over a CSV file,
    ''' moving <c>stride</c> lines at a time; the window grows at the start and shrinks at the end of the file.
    ''' </summary>
    <Serializable>
    Public Class CSVVariableSlidingWindowRecordReader
        Inherits CSVRecordReader
        Implements SequenceRecordReader

        Public Const LINES_PER_SEQUENCE As String = NAME_SPACE & ".nlinespersequence"

        Private maxLinesPerSequence As Integer
        Private delimiter As String
        Private stride As Integer
        Private queue As LinkedList(Of IList(Of Writable))
        Private exhausted As Boolean

        ''' <summary>
        ''' No-arg constructor with the default number of lines per sequence (10)
        ''' </summary>
        Public Sub New()
            Me.New(10, 1)
        End Sub

        ''' <param name="maxLinesPerSequence"> Number of lines in each sequence, use default delemiter(,) between entries in the same line </param>
        Public Sub New(ByVal maxLinesPerSequence As Integer)
            Me.New(maxLinesPerSequence, 0, 1, CSVRecordReader.DEFAULT_DELIMITER.ToString())
        End Sub

        ''' <param name="maxLinesPerSequence"> Number of lines in each sequence, use default delemiter(,) between entries in the same line </param>
        ''' <param name="stride"> Number of lines between records (increment window > 1 line) </param>
        Public Sub New(ByVal maxLinesPerSequence As Integer, ByVal stride As Integer)
            Me.New(maxLinesPerSequence, 0, stride, CSVRecordReader.DEFAULT_DELIMITER.ToString())
        End Sub

        ''' <param name="maxLinesPerSequence"> Number of lines in each sequence, use default delemiter(,) between entries in the same line </param>
        ''' <param name="stride"> Number of lines between records (increment window > 1 line) </param>
        Public Sub New(ByVal maxLinesPerSequence As Integer, ByVal stride As Integer, ByVal delimiter As String)
            Me.New(maxLinesPerSequence, 0, stride, CSVRecordReader.DEFAULT_DELIMITER.ToString())
        End Sub

        ''' <param name="maxLinesPerSequence"> Number of lines in each sequences </param>
        ''' <param name="skipNumLines"> Number of lines to skip at the start of the file (only skipped once, not per sequence) </param>
        ''' <param name="stride"> Number of lines between records (increment window > 1 line) </param>
        ''' <param name="delimiter"> Delimiter between entries in the same line, for example "," </param>
        Public Sub New(ByVal maxLinesPerSequence As Integer, ByVal skipNumLines As Integer, ByVal stride As Integer, ByVal delimiter As String)
            MyBase.New(skipNumLines)
            If stride < 1 Then
                Throw New ArgumentException("Stride must be greater than 1")
            End If

            Me.delimiter = delimiter
            Me.maxLinesPerSequence = maxLinesPerSequence
            Me.stride = stride
            Me.queue = New LinkedList(Of IList(Of Writable))()
            Me.exhausted = False
        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            MyBase.initialize(conf, split)
            Me.maxLinesPerSequence = conf.getInt(LINES_PER_SEQUENCE, maxLinesPerSequence)
        End Sub

        Public Overrides Function hasNext() As Boolean
            Dim moreInCsv As Boolean = MyBase.hasNext()
            Dim moreInQueue As Boolean = queue.Count > 0
            Return moreInCsv OrElse moreInQueue
        End Function

        Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            ' try polling next(), otherwise empty the queue
            ' loop according to stride size
            For i As Integer = 0 To stride - 1
                If MyBase.hasNext() Then
                    queue.AddFirst(MyBase.next())
                Else
                    exhausted = True
                End If

                If exhausted AndAlso queue.Count < 1 Then
                    Throw New InvalidOperationException("No next element")
                End If

                If queue.Count > maxLinesPerSequence OrElse exhausted Then
                    If queue.Count > 0 Then queue.RemoveLast() ' LinkedList.pollLast()
                End If
            Next i

            Dim sequence As List(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
            sequence.AddRange(queue)

            If exhausted AndAlso queue.Count = 1 Then
                queue.RemoveLast()
            End If

            Return sequence
        End Function

        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Throw New NotSupportedException("Reading CSV data from DataInputStream not yet implemented")
        End Function

        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            Dim lineBefore As Integer = lineIndex
            Dim record As IList(Of IList(Of Writable)) = sequenceRecord()
            Dim lineAfter As Integer = lineIndex + queue.Count
            Dim uri As Uri = (If(locations Is Nothing OrElse locations.Length < 1, Nothing, locations(splitIndex)))
            Dim meta As RecordMetaData = New RecordMetaDataLineInterval(lineBefore, lineAfter - 1, uri, GetType(CSVVariableSlidingWindowRecordReader))
            Return New SequenceRecordImpl(record, meta)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Return loadSequenceFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overrides Sub reset()
            MyBase.reset()
            queue = New LinkedList(Of IList(Of Writable))()
            exhausted = False
        End Sub
    End Class

End Namespace
