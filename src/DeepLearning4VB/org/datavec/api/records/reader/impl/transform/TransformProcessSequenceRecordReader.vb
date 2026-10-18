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
Imports Configurable = DeepLearning4VB.org.datavec.api.conf.Configurable
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordListener = DeepLearning4VB.org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl.transform

    ''' <summary>
    ''' A sequence record reader that executes a <see cref="TransformProcessT"/> on the sequences (and, through the
    ''' <see cref="RecordReader"/> methods, on the single records) of another sequence record reader. Unlike
    ''' <see cref="TransformProcessRecordReader"/>, filtered-out sequences are returned as Nothing, as in Java.
    ''' </summary>
    <Serializable>
    Public Class TransformProcessSequenceRecordReader
        Implements SequenceRecordReader

        Protected Friend sequenceRecordReader As SequenceRecordReader
        Protected Friend transformProcess As TransformProcessT

        ''' <summary>Lombok <c>@AllArgsConstructor</c>.</summary>
        Public Sub New(ByVal sequenceRecordReader As SequenceRecordReader, ByVal transformProcess As TransformProcessT)
            Me.sequenceRecordReader = sequenceRecordReader
            Me.transformProcess = transformProcess
        End Sub

        ''' <summary>
        ''' The configuration of the underlying sequence record reader.
        ''' </summary>
        Public Overridable Property Conf As Configuration Implements Configurable.Conf
            Set(ByVal value As Configuration)
                sequenceRecordReader.Conf = value
            End Set
            Get
                Return sequenceRecordReader.Conf
            End Get
        End Property

        Public Overridable Sub setConf(ByVal conf As Configuration) Implements Configurable.setConf
            Me.Conf = conf
        End Sub

        Public Overridable Function getConf() As Configuration Implements Configurable.getConf
            Return Me.Conf
        End Function

        ''' <summary>
        ''' Returns a sequence record.
        ''' </summary>
        ''' <returns> a sequence of records </returns>
        Public Overridable Function sequenceRecord() As IList(Of IList(Of WritableT)) Implements SequenceRecordReader.sequenceRecord
            Return transformProcess.executeSequence(sequenceRecordReader.sequenceRecord())
        End Function

        Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
            Return False
        End Function

        Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of WritableT)) Implements RecordReader.next
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Load a sequence record from the given DataInputStream and transform it.
        ''' Unlike <see cref="nextSequence()"/> the internal state of the RecordReader is not modified
        ''' </summary>
        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of WritableT)) Implements SequenceRecordReader.sequenceRecord
            Return transformProcess.executeSequence(sequenceRecordReader.sequenceRecord(uri, dataInputStream))
        End Function

        ''' <summary>
        ''' Similar to <see cref="sequenceRecord()"/>, but returns a <see cref="SequenceRecord"/> object, that may include
        ''' metadata such as the source of the data
        ''' </summary>
        ''' <returns> next sequence record </returns>
        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            Dim [next] As SequenceRecord = sequenceRecordReader.nextSequence()
            [next].SequenceRecord = transformProcess.executeSequence([next].getSequenceRecord())
            Return [next]
        End Function

        ''' <summary>
        ''' Load a single sequence record from the given <see cref="RecordMetaData"/> instance and transform it.
        ''' </summary>
        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Dim [next] As SequenceRecord = sequenceRecordReader.loadSequenceFromMetaData(recordMetaData)
            [next].SequenceRecord = transformProcess.executeSequence([next].getSequenceRecord())
            Return [next]
        End Function

        ''' <summary>
        ''' Not supported for several records at once: returns Nothing, as in Java.
        ''' </summary>
        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Return Nothing
        End Function

        ''' <summary>
        ''' Does nothing, as in Java: initialize the underlying sequence record reader instead.
        ''' </summary>
        Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize

        End Sub

        ''' <summary>
        ''' Does nothing, as in Java: initialize the underlying sequence record reader instead.
        ''' </summary>
        Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize

        End Sub

        ''' <summary>
        ''' The next record of the underlying reader, transformed as a single record.
        ''' </summary>
        Public Overridable Function [next]() As IList(Of WritableT) Implements RecordReader.next
            Return transformProcess.execute(sequenceRecordReader.next())
        End Function

        Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
            Return sequenceRecordReader.hasNext()
        End Function

        Public Overridable ReadOnly Property Labels As IList(Of String) Implements RecordReader.Labels
            Get
                Return sequenceRecordReader.Labels
            End Get
        End Property

        Public Overridable Sub reset() Implements RecordReader.reset
            sequenceRecordReader.reset()
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
            Return sequenceRecordReader.resetSupported()
        End Function

        Public Overridable Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of WritableT) Implements RecordReader.record
            Return transformProcess.execute(sequenceRecordReader.record(uri, dataInputStream))
        End Function

        Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
            Dim [next] As Record = sequenceRecordReader.nextRecord()
            [next].Record = transformProcess.execute([next].getRecord())
            Return [next]
        End Function

        Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
            Dim load As Record = sequenceRecordReader.loadFromMetaData(recordMetaData)
            load.Record = transformProcess.execute(load.getRecord())
            Return load
        End Function

        Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData
            Dim records As IList(Of Record) = sequenceRecordReader.loadFromMetaData(recordMetaDatas)
            For Each r As Record In records
                r.Record = transformProcess.execute(r.getRecord())
            Next r
            Return records
        End Function

        Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.Listeners
            Get
                Return sequenceRecordReader.Listeners
            End Get
            Set(ByVal value As IList(Of RecordListener))
                sequenceRecordReader.Listeners = value
            End Set
        End Property

        Public Overridable Sub setListeners(ByVal ParamArray listeners() As RecordListener) Implements RecordReader.setListeners
            sequenceRecordReader.setListeners(listeners)
        End Sub

        Public Overridable Sub setListeners(ByVal listeners As ICollection(Of RecordListener)) Implements RecordReader.setListeners
            sequenceRecordReader.setListeners(listeners)
        End Sub

        ''' <summary>
        ''' Closes the underlying sequence record reader.
        ''' </summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            sequenceRecordReader.Dispose()
        End Sub

        ''' <summary>Java <c>close()</c>.</summary>
        Public Sub close()
            Dispose()
        End Sub

    End Class

End Namespace
