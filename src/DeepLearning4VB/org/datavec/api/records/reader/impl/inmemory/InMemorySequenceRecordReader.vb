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
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordEquality = DeepLearning4VB.org.datavec.api.records.impl.RecordEquality
Imports RecordListener = DeepLearning4VB.org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports SequenceIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of System.Collections.Generic.IList(Of System.Collections.Generic.IList(Of DeepLearning4VB.org.datavec.api.writable.Writable)))
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl.inmemory

    ''' <summary>
    ''' A sequence record reader over an in-memory list of sequences (Lombok <c>@Data</c> in Java: accessors,
    ''' <c>Equals</c>, <c>GetHashCode</c> and <c>ToString</c> are written out).
    ''' </summary>
    <Serializable>
    Public Class InMemorySequenceRecordReader
        Implements SequenceRecordReader

        Private recordList As IList(Of IList(Of IList(Of Writable)))
        <NonSerialized>
        Private iter As SequenceIterator
        Private labelList As IList(Of String)
        Private recordListeners As IList(Of RecordListener)
        <NonSerialized>
        Private configuration As Configuration

        Public Sub New(ByVal records As IList(Of IList(Of IList(Of Writable))))
            Me.recordList = records
            Me.iter = New SequenceIterator(records.GetEnumerator())
        End Sub

        ''' <summary>The sequences (Lombok <c>getRecords()</c>/<c>setRecords()</c>); setting them restarts the iteration.</summary>
        Public Overridable Property Records As IList(Of IList(Of IList(Of Writable)))
            Get
                Return recordList
            End Get
            Set(ByVal value As IList(Of IList(Of IList(Of Writable))))
                recordList = value
                iter = If(value Is Nothing, Nothing, New SequenceIterator(value.GetEnumerator()))
            End Set
        End Property

        Public Function getRecords() As IList(Of IList(Of IList(Of Writable)))
            Return recordList
        End Function

        Public Sub setRecords(ByVal records As IList(Of IList(Of IList(Of Writable))))
            Me.Records = records
        End Sub

        Public Sub setLabels(ByVal labels As IList(Of String))
            Me.labelList = labels
        End Sub

        Public Function getRecordListeners() As IList(Of RecordListener)
            Return recordListeners
        End Function

        Public Sub setRecordListeners(ByVal recordListeners As IList(Of RecordListener))
            Me.recordListeners = recordListeners
        End Sub

        ''' <summary>
        ''' Set the configuration to be used by this object.
        ''' </summary>
        Public Overridable Property Conf As Configuration Implements Configurable.Conf
            Set(ByVal value As Configuration)
                Me.configuration = value
            End Set
            Get
                Return configuration
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
        Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Return iter.next()
        End Function

        Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
            Return False
        End Function

        Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of Writable)) Implements RecordReader.next
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Load a sequence record from the given DataInputStream
        ''' Unlike <see cref="next()"/> the internal state of the RecordReader is not modified
        ''' Implementations of this method should not close the DataInputStream
        ''' </summary>
        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Similar to <see cref="sequenceRecord()"/>, but returns a <see cref="SequenceRecord"/> object, that may include metadata such as the source
        ''' of the data
        ''' </summary>
        ''' <returns> next sequence record </returns>
        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            Return New SequenceRecordImpl(sequenceRecord(), Nothing)
        End Function

        ''' <summary>
        ''' Load a single sequence record from the given <see cref="RecordMetaData"/> instance<br/>
        ''' </summary>
        ''' <param name="recordMetaData"> Metadata for the sequence record that we want to load from </param>
        ''' <returns> Single sequence record for the given RecordMetaData instance </returns>
        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Load multiple sequence records from the given a list of <see cref="RecordMetaData"/> instances<br/>
        ''' </summary>
        ''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
        ''' <returns> Multiple sequence record for the given RecordMetaData instances </returns>
        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize

        End Sub

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="conf">  a configuration for initialization </param>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Public Overridable Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize

        End Sub

        ''' <summary>
        ''' Get the next record
        ''' </summary>
        Public Overridable Function [next]() As IList(Of Writable) Implements RecordReader.next
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Whether there are anymore records
        ''' </summary>
        Public Overridable Function hasNext() As Boolean Implements RecordReader.hasNext
            Return iter.hasNext()
        End Function

        ''' <summary>
        ''' List of label strings
        ''' </summary>
        Public Overridable ReadOnly Property Labels As IList(Of String) Implements RecordReader.Labels
            Get
                Return labelList
            End Get
        End Property

        ''' <summary>
        ''' Reset record reader iterator
        ''' </summary>
        Public Overridable Sub reset() Implements RecordReader.reset
            Me.iter = New SequenceIterator(recordList.GetEnumerator())
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements RecordReader.resetSupported
            Return True
        End Function

        ''' <summary>
        ''' Load the record from the given DataInputStream
        ''' Unlike <see cref="next()"/> the internal state of the RecordReader is not modified
        ''' Implementations of this method should not close the DataInputStream
        ''' </summary>
        Public Overridable Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of Writable) Implements RecordReader.record
            Return Nothing
        End Function

        ''' <summary>
        ''' Similar to <see cref="next()"/>, but returns a <see cref="Record"/> object, that may include metadata such as the source
        ''' of the data
        ''' </summary>
        ''' <returns> next record </returns>
        Public Overridable Function nextRecord() As Record Implements RecordReader.nextRecord
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Load a single record from the given <see cref="RecordMetaData"/> instance<br/>
        ''' </summary>
        ''' <param name="recordMetaData"> Metadata for the record that we want to load from </param>
        ''' <returns> Single record for the given RecordMetaData instance </returns>
        Public Overridable Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Load multiple records from the given a list of <see cref="RecordMetaData"/> instances<br/>
        ''' </summary>
        ''' <param name="recordMetaDatas"> Metadata for the records that we want to load from </param>
        ''' <returns> Multiple records for the given RecordMetaData instances </returns>
        Public Overridable Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' Get the record listeners for this record reader.
        ''' </summary>
        Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.Listeners
            Get
                Return recordListeners
            End Get
            Set(ByVal value As IList(Of RecordListener))
                Me.recordListeners = New List(Of RecordListener)(value)
            End Set
        End Property

        Public Overridable Sub setListeners(ByVal ParamArray listeners() As RecordListener) Implements RecordReader.setListeners
            Me.recordListeners = New List(Of RecordListener)(listeners)
        End Sub

        ''' <summary>
        ''' Set the record listeners for this record reader.
        ''' </summary>
        Public Overridable Sub setListeners(ByVal listeners As ICollection(Of RecordListener)) Implements RecordReader.setListeners
            Me.recordListeners = New List(Of RecordListener)(listeners)
        End Sub

        ''' <summary>
        ''' Closes this stream and releases any system resources associated
        ''' with it. If the stream is already closed then invoking this
        ''' method has no effect.
        ''' </summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose

        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then Return True
            Dim other = TryCast(o, InMemorySequenceRecordReader)
            If other Is Nothing Then Return False
            Return RecordEquality.listEquals(recordList, other.recordList) AndAlso
                RecordEquality.listEquals(labelList, other.labelList) AndAlso
                RecordEquality.listEquals(recordListeners, other.recordListeners) AndAlso
                Objects.equals(configuration, other.configuration)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + RecordEquality.listHashCode(recordList)
            result = result * 59 + RecordEquality.listHashCode(labelList)
            result = result * 59 + RecordEquality.listHashCode(recordListeners)
            result = result * 59 + If(configuration Is Nothing, 43, configuration.GetHashCode())
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "InMemorySequenceRecordReader(records=" & RecordEquality.listToString(recordList) & ", labels=" & RecordEquality.listToString(labelList) &
                ", recordListeners=" & RecordEquality.listToString(recordListeners) & ", configuration=" & Objects.toString(configuration) & ")"
        End Function
    End Class

End Namespace
