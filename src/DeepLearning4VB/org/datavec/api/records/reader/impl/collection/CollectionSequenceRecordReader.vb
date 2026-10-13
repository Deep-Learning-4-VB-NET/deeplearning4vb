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
Imports System.Linq
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordImpl = DeepLearning4VB.org.datavec.api.records.impl.Record
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports SequenceRecordImpl = DeepLearning4VB.org.datavec.api.records.impl.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataIndex
Imports BaseRecordReader = DeepLearning4VB.org.datavec.api.records.reader.BaseRecordReader
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports SequenceIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of System.Collections.Generic.IEnumerable(Of System.Collections.Generic.IEnumerable(Of DeepLearning4VB.org.datavec.api.writable.Writable)))
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl.collection

    ''' <summary>
    ''' A sequence record reader over an in-memory collection of sequences. Java's
    ''' <c>Collection&lt;? extends Collection&lt;? extends Collection&lt;Writable&gt;&gt;&gt;</c> is
    ''' <c>IEnumerable(Of IEnumerable(Of IEnumerable(Of Writable)))</c> (the only covariant collection interface
    ''' in .NET), so any list of lists of lists of writables can be passed.
    ''' </summary>
    <Serializable>
    Public Class CollectionSequenceRecordReader
        Inherits BaseRecordReader
        Implements SequenceRecordReader

        <NonSerialized>
        Private records As SequenceIterator
        Private ReadOnly original As IEnumerable(Of IEnumerable(Of IEnumerable(Of Writable)))
        Private count As Integer = 0

        ''' <param name="records">    Collection of sequences. For example, <c>List&lt;List&lt;List&lt;Writable&gt;&gt;&gt;</c> where the inner  two lists
        '''                   are a sequence, and the outer list/collection is a list of sequences </param>
        Public Sub New(ByVal records As IEnumerable(Of IEnumerable(Of IEnumerable(Of Writable))))
            Me.records = New SequenceIterator(records.GetEnumerator())
            Me.original = records
        End Sub

        Public Overrides Sub initialize(ByVal split As InputSplit)

        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            initialize(split)
        End Sub

        Public Overrides Function [next]() As IList(Of Writable)
            Throw New NotSupportedException("next() not supported for CollectionSequencRecordReader; use sequenceRecord()")
        End Function

        Public Overrides Function hasNext() As Boolean
            Return records.hasNext()
        End Function

        Public Overrides Sub Dispose()

        End Sub

        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)

            End Set
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides ReadOnly Property Labels As IList(Of String)
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides Sub reset()
            Me.records = New SequenceIterator(original.GetEnumerator())
            Me.count = 0
        End Sub

        Public Overrides Function resetSupported() As Boolean
            Return True
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
            Throw New NotSupportedException("Generating records from DataInputStream not supported for SequenceCollectionRecordReader")
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

        Public Overridable Function sequenceRecord() As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Dim record As IList(Of IList(Of Writable)) = toList(records.next())
            invokeListeners(record)
            count += 1
            Return record
        End Function

        Public Overridable Function sequenceRecord(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of IList(Of Writable)) Implements SequenceRecordReader.sequenceRecord
            Throw New NotSupportedException("Generating records from DataInputStream not supported for SequenceCollectionRecordReader")
        End Function

        Public Overridable Function nextSequence() As SequenceRecord Implements SequenceRecordReader.nextSequence
            Return New SequenceRecordImpl(sequenceRecord(), New RecordMetaDataIndex(count - 1, Nothing, GetType(CollectionSequenceRecordReader)))
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaData As RecordMetaData) As SequenceRecord Implements SequenceRecordReader.loadSequenceFromMetaData
            Return loadSequenceFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overridable Function loadSequenceFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of SequenceRecord) Implements SequenceRecordReader.loadSequenceFromMetaData
            Dim size As Integer = original.Count()
            Dim toLoad As ISet(Of Integer) = New LinkedHashSet(Of Integer)()
            For Each recordMetaData As RecordMetaData In recordMetaDatas
                If Not (TypeOf recordMetaData Is RecordMetaDataIndex) Then
                    Throw New ArgumentException("Expected RecordMetaDataIndex; got: " & Objects.toString(recordMetaData))
                End If
                Dim idx As Long = DirectCast(recordMetaData, RecordMetaDataIndex).getIndex()
                If idx >= size Then
                    Throw New InvalidOperationException("Cannot get index " & idx & " from collection: contains " & size & " elements")
                End If
                toLoad.Add(CInt(idx))
            Next recordMetaData

            Dim [out] As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            Dim i As Integer = 0
            For Each c As IEnumerable(Of IEnumerable(Of Writable)) In original
                If Not toLoad.Contains(i) Then
                    i += 1
                    Continue For
                Else
                    i += 1
                End If
                Dim record As IList(Of IList(Of Writable)) = toList(c)
                Dim r As SequenceRecord = New SequenceRecordImpl(record, New RecordMetaDataIndex(i - 1, Nothing, GetType(CollectionSequenceRecordReader)))
                [out].Add(r)
            Next c
            Return [out]
        End Function

        Private Shared Function toList(ByVal [next] As IEnumerable(Of IEnumerable(Of Writable))) As IList(Of IList(Of Writable))
            Dim record As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
            For Each c As IEnumerable(Of Writable) In [next]
                record.Add(New List(Of Writable)(c))
            Next c
            Return record
        End Function
    End Class

End Namespace
