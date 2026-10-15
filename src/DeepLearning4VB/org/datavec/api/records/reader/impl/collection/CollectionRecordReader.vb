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
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataIndex
Imports BaseRecordReader = DeepLearning4VB.org.datavec.api.records.reader.BaseRecordReader
Imports RecordIterator = DeepLearning4VB.org.datavec.api.records.reader.JavaIterator(Of System.Collections.Generic.IEnumerable(Of DeepLearning4VB.org.datavec.api.writable.Writable))
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit

Namespace org.datavec.api.records.reader.impl.collection

    ''' <summary>
    ''' A record reader over an in-memory collection of records. Java's
    ''' <c>Collection&lt;? extends Collection&lt;writable.Writable&gt;&gt;</c> is <c>IEnumerable(Of IEnumerable(Of writable.Writable))</c>
    ''' (the only covariant collection interface in .NET), so any list of lists of writables can be passed.
    ''' </summary>
    <Serializable>
    Public Class CollectionRecordReader
        Inherits BaseRecordReader

        <NonSerialized>
        Private records As RecordIterator
        Private ReadOnly original As IEnumerable(Of IEnumerable(Of writable.Writable))
        Private count As Integer = 0

        Public Sub New(ByVal records As IEnumerable(Of IEnumerable(Of writable.Writable)))
            Me.records = New RecordIterator(records.GetEnumerator())
            Me.original = records
        End Sub

        Public Overrides Sub initialize(ByVal split As InputSplit)

        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
            initialize(split)
        End Sub

        Public Overrides Function [next]() As IList(Of writable.Writable)
            Dim nextRecord As IEnumerable(Of writable.Writable) = records.next()
            Dim record As IList(Of writable.Writable) = If(TypeOf nextRecord Is IList(Of writable.Writable), DirectCast(nextRecord, IList(Of writable.Writable)), New List(Of writable.Writable)(nextRecord))
            invokeListeners(record)
            count += 1
            Return record
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
            Me.records = New RecordIterator(original.GetEnumerator())
            Me.count = 0
        End Sub

        Public Overrides Function resetSupported() As Boolean
            Return True
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of writable.Writable)
            Throw New NotSupportedException("Generating records from DataInputStream not supported for CollectionRecordReader")
        End Function

        Public Overrides Function nextRecord() As Record
            Return New RecordImpl([next](), New RecordMetaDataIndex(count - 1, Nothing, GetType(CollectionRecordReader)))
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
            Return loadFromMetaData(JavaCollections.singletonList(recordMetaData))(0)
        End Function

        Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
            Dim size As Integer = original.Count()
            Dim toLoad As ISet(Of Integer) = New HashSet(Of Integer)()
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

            Dim [out] As IList(Of Record) = New List(Of Record)()
            If TypeOf original Is System.Collections.IList Then
                Dim asList As System.Collections.IList = DirectCast(original, System.Collections.IList)
                For Each i As Integer In toLoad
                    Dim l As IList(Of writable.Writable) = New List(Of writable.Writable)(DirectCast(asList(i), IEnumerable(Of writable.Writable)))
                    Dim r As Record = New RecordImpl(l, New RecordMetaDataIndex(i, Nothing, GetType(CollectionRecordReader)))
                    [out].Add(r)
                Next i
            Else
                Dim i As Integer = 0
                For Each c As IEnumerable(Of writable.Writable) In original
                    If Not toLoad.Contains(i) Then
                        i += 1
                        Continue For
                    Else
                        i += 1
                    End If
                    Dim l As IList(Of writable.Writable) = If(TypeOf c Is IList(Of writable.Writable), DirectCast(c, IList(Of writable.Writable)), New List(Of writable.Writable)(c))
                    Dim r As Record = New RecordImpl(l, New RecordMetaDataIndex(i - 1, Nothing, GetType(CollectionRecordReader)))
                    [out].Add(r)
                Next c
            End If
            Return [out]
        End Function
    End Class

End Namespace
