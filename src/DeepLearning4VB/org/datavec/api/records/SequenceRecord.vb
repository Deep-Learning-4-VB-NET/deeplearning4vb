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

Namespace org.datavec.api.records

    ''' <summary>
    ''' A sequence record: a list of time steps, each a list of writable values, plus optional metadata.
    ''' Each accessor is available both as the Java-style method (<c>getSequenceRecord()</c>) and as a
    ''' VB property (<c>SequenceRecord</c>).
    ''' </summary>
    Public Interface SequenceRecord

        ''' <summary>
        ''' Get the sequence record values
        ''' </summary>
        ''' <returns> Sequence record values </returns>
        Function getSequenceRecord() As IList(Of IList(Of writable.Writable))

        ''' <summary>Set the sequence record values</summary>
        Sub setSequenceRecord(ByVal sequenceRecord As IList(Of IList(Of writable.Writable)))

        ''' <summary>
        ''' Get the overall length of the sequence record (number of time/sequence steps, etc).
        ''' Equivalent to <c>getSequenceRecord().size()</c>
        ''' </summary>
        ''' <returns> Length of sequence record </returns>
        Function getSequenceLength() As Integer

        ''' <summary>
        ''' Get a single time step. Equivalent to <c>getSequenceRecord().get(timeStep)</c>
        ''' </summary>
        ''' <param name="timeStep"> Time step to get. Must be <c>0 &lt;= timeStep &lt; getSequenceLength()</c> </param>
        ''' <returns> Values for a single time step </returns>
        Function getTimeStep(ByVal timeStep As Integer) As IList(Of writable.Writable)

        ''' <summary>
        ''' Get the RecordMetaData for this record
        ''' </summary>
        ''' <returns> Metadata for this record (or null, if none has been set) </returns>
        Function getMetaData() As RecordMetaData

        ''' <summary>Set the RecordMetaData for this record</summary>
        Sub setMetaData(ByVal metaData As RecordMetaData)

        ''' <summary>The sequence record values (<c>getSequenceRecord()</c>/<c>setSequenceRecord()</c> in Java)</summary>
        Property SequenceRecord As IList(Of IList(Of writable.Writable))

        ''' <summary>The number of time steps (<c>getSequenceLength()</c> in Java)</summary>
        ReadOnly Property SequenceLength As Integer

        ''' <summary>The metadata for this record, or Nothing if none has been set (<c>getMetaData()</c>/<c>setMetaData()</c> in Java)</summary>
        Property MetaData As RecordMetaData

    End Interface

End Namespace
