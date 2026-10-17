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
Imports Configurable = DeepLearning4VB.org.datavec.api.conf.Configurable
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordListener = DeepLearning4VB.org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil

Imports FileBatchT = DeepLearning4VB.org.nd4vb.common.loader.FileBatch
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions

Namespace org.datavec.api.records.reader.impl.filebatch

    ''' <summary>
    ''' Reads the files held in memory by a <see cref="FileBatch"/> through an underlying record reader's
    ''' <see cref="RecordReader.record"/> method.
    ''' </summary>
    <Serializable>
    Public Class FileBatchRecordReader
        Implements RecordReader

        Private ReadOnly recordReader As RecordReader
        Private ReadOnly fileBatch As FileBatchT
        Private position As Integer = 0

        ''' <param name="rr">        Underlying record reader to read files from </param>
        ''' <param name="fileBatch"> File batch to read files from </param>
        Public Sub New(ByVal rr As RecordReader, ByVal fileBatch As FileBatchT)
            Me.recordReader = rr
            Me.fileBatch = fileBatch
        End Sub

        Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize
            'No op
        End Sub

        Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize
            'No op
        End Sub

        Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
            Return False
        End Function

        Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of writable.Writable)) Implements RecordReader.next
            Dim [out] As IList(Of IList(Of writable.Writable)) = New List(Of IList(Of writable.Writable))(Math.Min(num, 10000))
            Dim i As Integer = 0
            Do While i < num AndAlso hasNext()
                [out].Add([next]())
                i += 1
            Loop
            Return [out]
        End Function

        Public Overridable Function [next]() As IList(Of writable.Writable) Implements RecordReader.next
            Preconditions.checkState(hasNext(), "No next element")

            Dim fileBytes() As Byte = fileBatch.getFileBytes()(position)
            Dim origPath As String = fileBatch.getOriginalUris()(position)

            Dim [out] As IList(Of writable.Writable)
            Try
                Using dis As New DataInputStream(New MemoryStream(fileBytes))
                    [out] = recordReader.record(URIUtil.createURI(origPath), dis)
                End Using
            Catch e As IOException
                Throw New Exception("Error reading from file bytes", e)
            End Try

            position += 1
            Return [out]
        End Function

        Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
            Return position < fileBatch.getFileBytes().Count
        End Function

        Public Overridable ReadOnly Property Labels As IList(Of String) Implements RecordReader.Labels
            Get
                Return recordReader.Labels
            End Get
        End Property

        Public Overridable Sub reset() Implements RecordReader.reset
            position = 0
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
            Return True
        End Function

        Public Overridable Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable) Implements RecordReader.record
            Throw New NotSupportedException("Not supported")
        End Function

        Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
            Return New RecordImpl([next](), Nothing)
        End Function

        Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
            Return recordReader.loadFromMetaData(recordMetaData)
        End Function

        Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData
            Return recordReader.loadFromMetaData(recordMetaDatas)
        End Function

        Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.Listeners
            Get
                Return Nothing
            End Get
            Set(ByVal value As IList(Of RecordListener))
                recordReader.setListeners(DirectCast(value, ICollection(Of RecordListener)))
            End Set
        End Property

        Public Overridable Sub setListeners(ByVal ParamArray listeners() As RecordListener) Implements RecordReader.setListeners
            recordReader.setListeners(listeners)
        End Sub

        Public Overridable Sub setListeners(ByVal listeners As ICollection(Of RecordListener)) Implements RecordReader.setListeners
            recordReader.setListeners(listeners)
        End Sub

        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            recordReader.Dispose()
        End Sub

        Public Overridable Property Conf As Configuration Implements Configurable.Conf
            Set(ByVal value As Configuration)
                recordReader.Conf = value
            End Set
            Get
                Return recordReader.Conf
            End Get
        End Property

        Public Overridable Sub setConf(ByVal conf As Configuration) Implements Configurable.setConf
            Me.Conf = conf
        End Sub

        Public Overridable Function getConf() As Configuration Implements Configurable.getConf
            Return Me.Conf
        End Function

    End Class

End Namespace
