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
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataURI
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports StringIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of String)
Imports FileRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.FileRecordReader
Imports LineIterator = DeepLearning4VB.org.datavec.api.records.reader.impl.LineIterator
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl.csv

    ''' <summary>A sequence reader where each CSV file of the split is one sequence and each line one time step.</summary>
    <Serializable>
    Public Class CSVSequenceRecordReader
        Inherits FileRecordReader
        Implements SequenceRecordReader

        Private skipNumLines As Integer = 0
        Private delimiter As String = ","

        Public Sub New()
            Me.New(0, ",")
        End Sub

        Public Sub New(ByVal skipNumLines As Integer)
            Me.New(skipNumLines, ",")
        End Sub

        Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As String)
            Me.skipNumLines = skipNumLines
            Me.delimiter = delimiter
        End Sub

        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            invokeListeners(uri)
            Return loadAndClose(dataInputStream.BaseStream)
        End Function

        Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Return nextSequence().getSequenceRecord()
        End Function

        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            If Not hasNext() Then
                Throw New InvalidOperationException("No next element")
            End If

            Dim [next] As Uri = locationsIterator.next()
            currentUri = [next]
            invokeListeners([next])

            Dim [out] As IList(Of IList(Of Writable)) = loadAndClose(streamCreatorFn.apply([next]))
            Return New SequenceRecordImpl([out], New RecordMetaDataURI([next]))
        End Function

        Private Function loadAndClose(ByVal inputStream As Stream) As IList(Of IList(Of Writable))
            Dim lineIter As LineIterator = Nothing
            Try
                lineIter = New LineIterator(inputStream, charsetEncoding())
                Return load(lineIter)
            Finally
                If lineIter IsNot Nothing Then
                    lineIter.close()
                End If
                Try
                    inputStream.Dispose()
                Catch
                    ' IOUtils.closeQuietly
                End Try
            End Try
        End Function

        Private Function load(ByVal lineIter As StringIterator) As IList(Of IList(Of Writable))
            If skipNumLines > 0 Then
                Dim count As Integer = 0
                Do While count < skipNumLines AndAlso lineIter.hasNext()
                    count += 1
                    lineIter.next()
                Loop
            End If

            Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
            Do While lineIter.hasNext()
                Dim line As String = lineIter.next()
                Dim split() As String = StringHelper.Split(line, delimiter, True)
                Dim list As New List(Of Writable)()
                For Each s As String In split
                    list.Add(New Text(s))
                Next s
                [out].Add(list)
            Loop
            Return [out]
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Return loadSequenceFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            For Each meta As RecordMetaData In recordMetaDatas
                Dim [next] As String = URIUtil.toFilePath(meta.URI)
                Dim sequence As IList(Of IList(Of Writable)) = loadAndClose(New FileStream([next], FileMode.Open, FileAccess.Read))
                [out].Add(New SequenceRecordImpl(sequence, meta))
            Next meta
            Return [out]
        End Function
    End Class

End Namespace
