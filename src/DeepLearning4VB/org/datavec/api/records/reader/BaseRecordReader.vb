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
Imports DeepLearning4VB.org.nd4j.common.function
Imports Configurable = DeepLearning4VB.org.datavec.api.conf.Configurable
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordListener = DeepLearning4VB.org.datavec.api.records.listener.RecordListener
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports StreamInputSplit = DeepLearning4VB.org.datavec.api.split.StreamInputSplit
Imports FileStreamCreatorFunction = DeepLearning4VB.org.datavec.api.split.streams.FileStreamCreatorFunction

Namespace org.datavec.api.records.reader

    ''' <summary>
    ''' Base class for record readers: keeps the input split, the record listeners and the stream creator
    ''' function. The constants Java declared on the <see cref="RecordReader"/> interface are kept here
    ''' (VB interfaces cannot hold fields); they are the Java class name based keys, so configurations are
    ''' interchangeable with the Java version.
    ''' </summary>
    <Serializable>
    Public MustInherit Class BaseRecordReader
        Implements RecordReader

        ''' <summary>Java: <c>RecordReader.class.getName()</c>; kept as the Java name so configuration keys match.</summary>
        Public Const NAME_SPACE As String = "org.datavec.api.records.reader.RecordReader"

        ''' <summary>Configuration key: whether to append the label to the records read.</summary>
        Public Const APPEND_LABEL As String = NAME_SPACE & ".appendlabel"

        ''' <summary>
        ''' Configuration key: the labels (<c>LABELS</c> in Java; renamed because VB is case-insensitive and
        ''' the name would clash with the <see cref="Labels"/> property).
        ''' </summary>
        Public Const LABELS_KEY As String = NAME_SPACE & ".labels"

        Protected Friend inputSplit As InputSplit
        ''' <summary>The record listeners (the field is called <c>listeners</c> in Java; renamed because of the <c>Listeners</c> property).</summary>
        Protected Friend listenerList As IList(Of RecordListener) = New List(Of RecordListener)()
        Protected Friend streamCreatorFn As [Function](Of Uri, Stream) = New FileStreamCreatorFunction()

        Public MustOverride Property Conf As Configuration Implements Configurable.Conf

        Public Overridable Sub setConf(ByVal conf As Configuration) Implements Configurable.setConf
            Me.Conf = conf
        End Sub

        Public Overridable Function getConf() As Configuration Implements Configurable.getConf
            Return Me.Conf
        End Function

        Public MustOverride Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit) Implements RecordReader.initialize

        Public MustOverride Function [next]() As IList(Of writable.Writable) Implements RecordReader.next

        Public MustOverride Function hasNext() As Boolean Implements RecordReader.hasNext

        Public MustOverride ReadOnly Property Labels As IList(Of String) Implements RecordReader.Labels

        Public MustOverride Sub reset() Implements RecordReader.reset

        Public MustOverride Function resetSupported() As Boolean Implements RecordReader.resetSupported

        Public MustOverride Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable) Implements RecordReader.record

        Public MustOverride Function nextRecord() As Record Implements RecordReader.nextRecord

        Public MustOverride Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record Implements RecordReader.loadFromMetaData

        Public MustOverride Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record) Implements RecordReader.loadFromMetaData

        ''' <summary>
        ''' Invokes <see cref="RecordListener.recordRead(RecordReader, Object)"/> on all listeners. </summary>
        Protected Friend Overridable Sub invokeListeners(ByVal record As Object)
            For Each listener As RecordListener In listenerList
                listener.recordRead(Me, record)
            Next listener
        End Sub

        Public Overridable Sub initialize(ByVal split As InputSplit) Implements RecordReader.initialize
            Me.inputSplit = split
            If TypeOf split Is StreamInputSplit Then
                Dim s As StreamInputSplit = DirectCast(split, StreamInputSplit)
                If s.getStreamCreatorFn() IsNot Nothing Then
                    Me.streamCreatorFn = s.getStreamCreatorFn()
                End If
            End If
        End Sub

        Public Overridable Property Listeners As IList(Of RecordListener) Implements RecordReader.Listeners
            Get
                Return listenerList
            End Get
            Set(ByVal value As IList(Of RecordListener))
                Me.listenerList = value
            End Set
        End Property

        Public Overridable Sub setListeners(ByVal ParamArray listeners() As RecordListener) Implements RecordReader.setListeners
            setListeners(DirectCast(Arrays.asList(listeners), ICollection(Of RecordListener)))
        End Sub

        Public Overridable Sub setListeners(ByVal listeners As ICollection(Of RecordListener)) Implements RecordReader.setListeners
            Me.listenerList = If(TypeOf listeners Is IList(Of RecordListener), DirectCast(listeners, IList(Of RecordListener)), New List(Of RecordListener)(listeners))
        End Sub

        Public Overridable Function batchesSupported() As Boolean Implements RecordReader.batchesSupported
            Return False
        End Function

        Public Overridable Function [next](ByVal num As Integer) As IList(Of IList(Of writable.Writable)) Implements RecordReader.next
            Throw New NotSupportedException()
        End Function

        ''' <summary>Releases the resources held by the reader (Java <c>close()</c>). The default does nothing.</summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
        End Sub

        ''' <summary>Java-style alias of <see cref="Dispose"/>.</summary>
        Public Sub close()
            Dispose()
        End Sub

    End Class

End Namespace
