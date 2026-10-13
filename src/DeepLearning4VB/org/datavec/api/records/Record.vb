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

Imports System.Collections.Generic
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records

    ''' <summary>
    ''' A record: a list of writable values plus optional metadata about where it came from. Each accessor
    ''' is available both as the Java-style method (<c>getRecord()</c>/<c>setRecord()</c>) and as a VB
    ''' property (<c>Record</c>).
    ''' </summary>
    Public Interface Record

        ''' <summary>
        ''' Get the record values, as a <c>List&lt;Writable&gt;</c>
        ''' </summary>
        ''' <returns> Record values </returns>
        Function getRecord() As IList(Of Writable)

        ''' <summary>Set the record values</summary>
        Sub setRecord(ByVal record As IList(Of Writable))

        ''' <summary>
        ''' Get the RecordMetaData for this record
        ''' </summary>
        ''' <returns> Metadata for this record (or null, if none has been set) </returns>
        Function getMetaData() As RecordMetaData

        ''' <summary>Set the RecordMetaData for this record</summary>
        Sub setMetaData(ByVal metaData As RecordMetaData)

        ''' <summary>The record values (<c>getRecord()</c>/<c>setRecord()</c> in Java)</summary>
        Property Record As IList(Of Writable)

        ''' <summary>The metadata for this record, or Nothing if none has been set (<c>getMetaData()</c>/<c>setMetaData()</c> in Java)</summary>
        Property MetaData As RecordMetaData

    End Interface

End Namespace
