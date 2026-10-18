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
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordListener = DeepLearning4VB.org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl.transform

    ''' <summary>
    ''' A record reader that executes a <see cref="TransformProcessT"/> on the records of another record reader.
    ''' Records removed by a filter of the process are skipped (the next record is prefetched). Java's
    ''' <c>NoSuchElementException</c> is <see cref="InvalidOperationException"/>.
    ''' </summary>
    <Serializable>
    Public Class TransformProcessRecordReader
        Implements RecordReader

        Protected Friend recordReader As RecordReader
        Protected Friend transformProcess As TransformProcessT

        'Cached/prefetched values, in case of filtering
        Protected Friend nextRecordValue As Record

        Public Sub New(ByVal recordReader As RecordReader, ByVal transformProcess As TransformProcessT)
            Me.recordReader = recordReader
            Me.transformProcess = transformProcess
        End Sub

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize
            recordReader.initialize(split)
        End Sub

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="conf">  a configuration for initialization </param>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize
            recordReader.initialize(conf, split)
        End Sub

        Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
            Return True
        End Function

        Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of WritableT)) Implements RecordReader.next
            If Not hasNext() Then
                Throw New InvalidOperationException("No next element")
            End If

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim i As Integer = 0
            Do While i < num AndAlso hasNext()
                [out].Add([next]())
                i += 1
            Loop
            Return [out]
        End Function

        ''' <summary>
        ''' Get the next record
        ''' </summary>
        Public Overridable Function [next]() As IList(Of WritableT) Implements RecordReader.next
            If Not hasNext() Then 'Also triggers prefetch
                Throw New InvalidOperationException("No next element")
            End If
            Dim [out] As IList(Of WritableT) = nextRecordValue.getRecord()
            nextRecordValue = Nothing
            Return [out]
        End Function

        ''' <summary>
        ''' Whether there are anymore records
        ''' </summary>
        Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
            If nextRecordValue IsNot Nothing Then
                Return True
            End If
            If Not recordReader.hasNext() Then
                Return False
            End If

            'Prefetch, until we find one that isn't filtered out - or we run out of data
            Do While nextRecordValue Is Nothing AndAlso recordReader.hasNext()
                Dim r As Record = recordReader.nextRecord()
                Dim temp As IList(Of WritableT) = transformProcess.execute(r.getRecord())
                If temp Is Nothing Then
                    Continue Do
                End If
                nextRecordValue = New RecordImpl(temp, r.MetaData)
            Loop

            Return nextRecordValue IsNot Nothing
        End Function

        ''' <summary>
        ''' List of label strings
        ''' </summary>
        Public Overridable ReadOnly Property Labels As IList(Of String) Implements RecordReader.Labels
            Get
                Return recordReader.Labels
            End Get
        End Property

        ''' <summary>
        ''' Reset record reader iterator
        ''' </summary>
        Public Overridable Sub reset() Implements RecordReader.reset
            nextRecordValue = Nothing
            recordReader.reset()
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
            Return recordReader.resetSupported()
        End Function

        ''' <summary>
        ''' Load the record from the given DataInputStream and transform it.
        ''' Unlike <see cref="next()"/> the internal state of the RecordReader is not modified
        ''' Implementations of this method should not close the DataInputStream
        ''' </summary>
        Public Overridable Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of WritableT) Implements RecordReader.record
            Return transformProcess.execute(recordReader.record(uri, dataInputStream))
        End Function

        ''' <summary>
        ''' Similar to <see cref="next()"/>, but returns a <see cref="Record"/> object, that may include metadata such as the source
        ''' of the data
        ''' </summary>
        ''' <returns> next record </returns>
        Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
            If Not hasNext() Then 'Also triggers prefetch
                Throw New InvalidOperationException("No next element")
            End If
            Dim toRet As Record = nextRecordValue
            nextRecordValue = Nothing
            Return toRet
        End Function

        ''' <summary>
        ''' Load a single record from the given <see cref="RecordMetaData"/> instance (as in Java, the record is not
        ''' transformed).
        ''' </summary>
        ''' <param name="recordMetaData"> Metadata for the record that we want to load from </param>
        ''' <returns> Single record for the given RecordMetaData instance </returns>
        Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
            Return recordReader.loadFromMetaData(recordMetaData)
        End Function

        ''' <summary>
        ''' Load multiple records from the given a list of <see cref="RecordMetaData"/> instances (as in Java, the
        ''' records are not transformed).
        ''' </summary>
        ''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
        ''' <returns> Multiple records for the given RecordMetaData instances </returns>
        Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData
            Return recordReader.loadFromMetaData(recordMetaDatas)
        End Function

        ''' <summary>
        ''' The record listeners of the underlying record reader.
        ''' </summary>
        Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.Listeners
            Get
                Return recordReader.Listeners
            End Get
            Set(ByVal value As IList(Of RecordListener))
                recordReader.Listeners = value
            End Set
        End Property

        ''' <summary>
        ''' Set the record listeners for the underlying record reader.
        ''' </summary>
        Public Overridable Sub setListeners(ByVal ParamArray listeners() As RecordListener) Implements RecordReader.setListeners
            recordReader.setListeners(listeners)
        End Sub

        ''' <summary>
        ''' Set the record listeners for the underlying record reader.
        ''' </summary>
        Public Overridable Sub setListeners(ByVal listeners As ICollection(Of RecordListener)) Implements RecordReader.setListeners
            recordReader.setListeners(listeners)
        End Sub

        ''' <summary>
        ''' Closes the underlying record reader.
        ''' </summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            recordReader.Dispose()
        End Sub

        ''' <summary>Java <c>close()</c>.</summary>
        Public Sub close()
            Dispose()
        End Sub

        ''' <summary>
        ''' The configuration of the underlying record reader. As in Java, setting it has no effect.
        ''' </summary>
        Public Overridable Property Conf As Configuration Implements Configurable.Conf
            Set(ByVal value As Configuration)

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
