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
Imports Configurable = DeepLearning4VB.org.datavec.api.conf.Configurable
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData
Imports Partitioner = DeepLearning4VB.org.datavec.api.split.partition.Partitioner
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.writer

    ''' <summary>
    ''' A record writer: writes records (lists of writables) to the locations of an <see cref="InputSplit"/>,
    ''' partitioned by a <see cref="Partitioner"/>. Java's <c>Closeable</c> is <see cref="IDisposable"/>; the
    ''' <c>APPEND</c> constant the Java interface declared lives on
    ''' <see cref="org.datavec.api.records.writer.impl.FileRecordWriter"/> (VB interfaces cannot hold fields).
    ''' </summary>
    Public Interface RecordWriter
        Inherits Configurable, IDisposable

        ''' <summary>
        ''' Returns true if this record writer
        ''' supports efficient batch writing using <see cref="writeBatch(IList(Of IList(Of Writable)))"/>
        ''' </summary>
        Function supportsBatch() As Boolean

        ''' <summary>
        ''' Initialize a record writer with the given input split </summary>
        ''' <param name="inputSplit"> the input split to initialize with </param>
        ''' <param name="partitioner"> </param>
        Sub initialize(ByVal inputSplit As InputSplit, ByVal partitioner As Partitioner)

        ''' <summary>
        ''' Initialize the record reader with the given configuration
        ''' and <see cref="InputSplit"/> </summary>
        ''' <param name="configuration"> the configuration to iniailize with </param>
        ''' <param name="split"> the split to use </param>
        ''' <param name="partitioner"> </param>
        Sub initialize(ByVal configuration As Configuration, ByVal split As InputSplit, ByVal partitioner As Partitioner)

        ''' <summary>
        ''' Write a record </summary>
        ''' <param name="record"> the record to write </param>
        Function write(ByVal record As IList(Of Writable)) As PartitionMetaData

        ''' <summary>
        ''' Write a batch of records </summary>
        ''' <param name="batch"> the batch to write </param>
        Function writeBatch(ByVal batch As IList(Of IList(Of Writable))) As PartitionMetaData

        ''' <summary>
        ''' Close the recod reader
        ''' </summary>
        Sub close()

    End Interface

End Namespace
