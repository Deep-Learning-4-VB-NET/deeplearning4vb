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
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.writer

    ''' <summary>
    ''' A writer of sequence records. Java's <c>Closeable</c> is <see cref="IDisposable"/>; the <c>APPEND</c>
    ''' constant the Java interface declared lives on
    ''' <see cref="org.datavec.api.records.writer.impl.FileRecordWriter"/> (VB interfaces cannot hold fields).
    ''' </summary>
    Public Interface SequenceRecordWriter
        Inherits Configurable, IDisposable

        ''' <summary>
        ''' Write a record </summary>
        ''' <param name="sequence"> the record to write </param>
        Function write(ByVal sequence As IList(Of IList(Of Writable))) As PartitionMetaData

        ''' <summary>
        ''' Close the sequence record writer
        ''' </summary>
        Sub close()

    End Interface

End Namespace
