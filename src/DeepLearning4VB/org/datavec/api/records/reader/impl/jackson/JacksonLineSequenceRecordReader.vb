Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DeepLearning4VB.JavaCompat
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataURI
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports FileRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.FileRecordReader
Imports LineIterator = DeepLearning4VB.org.datavec.api.records.reader.impl.LineIterator

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

Namespace org.datavec.api.records.reader.impl.jackson

    ''' <summary>
    ''' Reads each file of the split as a sequence with one JSON document per line (one time step per line),
    ''' extracting the fields of a <see cref="FieldSelection"/>. The Java constructor also took an
    ''' <c>ObjectMapper</c> (JSON, YAML or XML); this port reads JSON only with System.Text.Json (see
    ''' <see cref="JacksonReaderUtils"/>).
    ''' </summary>
    <Serializable>
    Public Class JacksonLineSequenceRecordReader
        Inherits FileRecordReader
        Implements SequenceRecordReader

        Private selection As FieldSelection

        ''' <param name="selection"> Fields to return </param>
        Public Sub New(ByVal selection As FieldSelection)
            Me.selection = selection
        End Sub

        Public Overridable Function sequenceRecord() As IList(Of IList(Of writable.Writable)) Implements SequenceRecordReader.sequenceRecord
            Return nextSequence().getSequenceRecord()
        End Function

        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of writable.Writable)) Implements SequenceRecordReader.sequenceRecord
            Return loadAndClose(dataInputStream.BaseStream)
        End Function

        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            If Not hasNext() Then
                Throw New InvalidOperationException("No next element")
            End If

            Dim [next] As Uri = locationsIterator.next()
            Dim [out] As IList(Of IList(Of writable.Writable)) = loadAndClose(streamCreatorFn.apply([next]))
            Return New SequenceRecordImpl([out], New RecordMetaDataURI([next]))
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Return Nothing
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            For Each m As RecordMetaData In recordMetaDatas
                [out].Add(loadSequenceFromMetaData(m))
            Next m
            Return [out]
        End Function

        Private Function loadAndClose(ByVal inputStream As Stream) As IList(Of IList(Of writable.Writable))
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
                Catch e As Exception
                    ' IOUtils.closeQuietly
                End Try
            End Try
        End Function

        Private Function load(ByVal lineIter As LineIterator) As IList(Of IList(Of writable.Writable))
            Dim [out] As IList(Of IList(Of writable.Writable)) = New List(Of IList(Of writable.Writable))()
            Do While lineIter.hasNext()
                [out].Add(JacksonReaderUtils.parseRecord(lineIter.next(), selection))
            Loop
            Return [out]
        End Function

    End Class

End Namespace
