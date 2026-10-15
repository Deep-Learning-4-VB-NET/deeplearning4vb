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
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports BaseRecordReader = DeepLearning4VB.org.datavec.api.records.reader.BaseRecordReader
Imports ListIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of System.Collections.Generic.IList(Of String))
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports ListStringSplit = DeepLearning4VB.org.datavec.api.split.ListStringSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.records.reader.impl.collection

    ''' <summary>Reads records from a <see cref="ListStringSplit"/>: each inner list of strings becomes a record of <see cref="Text"/>s.</summary>
    <Serializable>
    Public Class ListStringRecordReader
        Inherits BaseRecordReader

        Private delimitedData As IList(Of IList(Of String))
        <NonSerialized>
        Private dataIter As ListIterator
        ''' <summary>The configuration (the field is called <c>conf</c> in Java; renamed because of the <c>Conf</c> property).</summary>
        <NonSerialized>
        Private confValue As Configuration

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Public Overrides Sub initialize(ByVal split As InputSplit)
            If TypeOf split Is ListStringSplit Then
                Dim listStringSplit As ListStringSplit = DirectCast(split, ListStringSplit)
                delimitedData = listStringSplit.getData()
                dataIter = New ListIterator(delimitedData.GetEnumerator())
            Else
                Throw New ArgumentException("Illegal type of input split " & split.GetType().FullName)
            End If
        End Sub

        ''' <summary>
        ''' Called once at initialization.
        ''' </summary>
        ''' <param name="conf">  a configuration for initialization </param>
        ''' <param name="split"> the split that defines the range of records to read </param>
        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            initialize(split)
        End Sub

        ''' <summary>
        ''' Get the next record
        ''' </summary>
        ''' <returns> The list of next record </returns>
        Public Overrides Function [next]() As IList(Of writable.Writable)
            Dim nextRecord As IList(Of String) = dataIter.next()
            invokeListeners(nextRecord)
            Dim ret As IList(Of writable.Writable) = New List(Of writable.Writable)()
            For Each s As String In nextRecord
                ret.Add(New Text(s))
            Next s
            Return ret
        End Function

        ''' <summary>
        ''' Check whether there are anymore records
        ''' </summary>
        ''' <returns> Whether there are more records </returns>
        Public Overrides Function hasNext() As Boolean
            Return dataIter.hasNext()
        End Function

        ''' <summary>
        ''' List of label strings
        ''' </summary>
        Public Overrides ReadOnly Property Labels As IList(Of String)
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Reset record reader iterator
        ''' </summary>
        Public Overrides Sub reset()
            dataIter = New ListIterator(delimitedData.GetEnumerator())
        End Sub

        Public Overrides Function resetSupported() As Boolean
            Return True
        End Function

        ''' <summary>
        ''' Load the record from the given DataInputStream
        ''' Unlike <see cref="next()"/> the internal state of the RecordReader is not modified
        ''' Implementations of this method should not close the DataInputStream
        ''' </summary>
        ''' <param name="uri"> </param>
        ''' <param name="dataInputStream"> </param>
        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            Return Nothing
        End Function

        Public Overrides Function nextRecord() As Record
            Return New RecordImpl([next](), Nothing)
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Throw New NotSupportedException("Loading from metadata not yet implemented")
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
            Throw New NotSupportedException("Loading from metadata not yet implemented")
        End Function

        ''' <summary>
        ''' Closes this stream and releases any system resources associated
        ''' with it. If the stream is already closed then invoking this
        ''' method has no effect.
        ''' </summary>
        Public Overrides Sub Dispose()

        End Sub

        ''' <summary>
        ''' Set the configuration to be used by this object.
        ''' </summary>
        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)
                Me.confValue = value
            End Set
            Get
                Return confValue
            End Get
        End Property

    End Class

End Namespace
