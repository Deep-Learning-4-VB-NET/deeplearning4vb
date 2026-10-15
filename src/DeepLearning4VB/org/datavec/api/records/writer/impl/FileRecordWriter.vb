Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports DeepLearning4VB.JavaCompat
Imports Configurable = DeepLearning4VB.org.datavec.api.conf.Configurable
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData
Imports Partitioner = DeepLearning4VB.org.datavec.api.split.partition.Partitioner

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

Namespace org.datavec.api.records.writer.impl

    ''' <summary>
    ''' Base class for record writers that write text to the output stream of a <see cref="Partitioner"/>.
    ''' <c>Charset</c> is <see cref="Encoding"/>; the output is a <see cref="DataOutputStream"/> like in Java.
    ''' The <c>APPEND</c> constant the Java <c>RecordWriter</c> interface declared is kept here (VB interfaces
    ''' cannot hold fields).
    ''' </summary>
    Public MustInherit Class FileRecordWriter
        Implements RecordWriter

        ''' <summary>Configuration key: whether to append to an existing file (<c>RecordWriter.APPEND</c> in Java).</summary>
        Public Const APPEND As String = "org.datavec.api.records.writer.RecordWriter.append"

        ''' <summary>UTF-8 (<c>StandardCharsets.UTF_8</c> in Java); no byte order mark is written.</summary>
        Public Shared ReadOnly DEFAULT_CHARSET As Encoding = New UTF8Encoding(False)

        Protected Friend [out] As DataOutputStream
        Public Const NEW_LINE As String = vbLf

        Protected Friend encoding As Encoding = DEFAULT_CHARSET

        Protected Friend partitioner As Partitioner

        ''' <summary>The configuration (the field is called <c>conf</c> in Java; renamed because of the <c>Conf</c> property).</summary>
        Protected Friend confValue As Configuration

        Public Sub New()
        End Sub

        Public MustOverride Function supportsBatch() As Boolean Implements RecordWriter.supportsBatch

        Public MustOverride Function write(ByVal record As IList(Of writable.Writable)) As PartitionMetaData Implements RecordWriter.write

        Public MustOverride Function writeBatch(ByVal batch As IList(Of IList(Of writable.Writable))) As PartitionMetaData Implements RecordWriter.writeBatch

        Public Overridable Sub initialize(ByVal inputSplit As InputSplit, ByVal partitioner As Partitioner) Implements RecordWriter.initialize
            partitioner.init(inputSplit)
            [out] = New DataOutputStream(partitioner.currentOutputStream())
            Me.partitioner = partitioner
        End Sub

        Public Overridable Sub initialize(ByVal configuration As Configuration, ByVal split As InputSplit, ByVal partitioner As Partitioner) Implements RecordWriter.initialize
            Conf = configuration
            partitioner.init(configuration, split)
            initialize(split, partitioner)
        End Sub

        ''' <summary>Flushes and closes the output stream (Java <c>close()</c>). Closing twice is a no-op, as in Java.</summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            If [out] IsNot Nothing Then
                Dim stream = [out]
                [out] = Nothing
                Try
                    stream.flush()
                    stream.Close()
                Catch e As ObjectDisposedException
                    ' the underlying stream was already closed elsewhere
                Catch e As IOException
                    Throw New InvalidOperationException(e.Message, e)
                End Try
            End If
        End Sub

        ''' <summary>Java-style alias of <see cref="Dispose"/>.</summary>
        Public Sub close() Implements RecordWriter.close
            Dispose()
        End Sub

        Public Overridable Property Conf As Configuration Implements Configurable.Conf
            Set(ByVal value As Configuration)
                Me.confValue = value
            End Set
            Get
                Return confValue
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
