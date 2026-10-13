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
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl

    ''' <summary>
    ''' Combines several record readers: each record is the concatenation of one record from every reader.
    '''
    ''' @author sonali
    ''' </summary>
    <Serializable>
    Public Class ComposableRecordReader
        Inherits BaseRecordReader

        Private readers() As RecordReader

        Public Sub New(ByVal ParamArray readers() As RecordReader)
            Me.readers = readers
        End Sub

        Public Overrides Sub initialize(ByVal split As InputSplit)

        End Sub

        Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)

        End Sub

        Public Overrides Function [next]() As IList(Of Writable)
            Dim ret As List(Of Writable) = New List(Of Writable)()
            If Me.hasNext() Then
                For Each reader As RecordReader In readers
                    ret.AddRange(reader.next())
                Next reader
            End If
            invokeListeners(ret)
            Return ret
        End Function

        Public Overrides Function hasNext() As Boolean
            Dim readersHasNext As Boolean = True
            For Each reader As RecordReader In readers
                readersHasNext = readersHasNext AndAlso reader.hasNext()
            Next reader
            Return readersHasNext
        End Function

        Public Overrides ReadOnly Property Labels As IList(Of String)
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides Sub Dispose()
            For Each reader As RecordReader In readers
                reader.Dispose()
            Next reader
        End Sub

        Public Overrides Property Conf As Configuration
            Set(ByVal value As Configuration)
                For Each reader As RecordReader In readers
                    reader.Conf = value
                Next reader
            End Set
            Get
                For Each reader As RecordReader In readers
                    Return reader.Conf
                Next reader
                Return Nothing
            End Get
        End Property

        Public Overrides Sub reset()
            For Each reader As RecordReader In readers
                reader.reset()
            Next reader

        End Sub

        Public Overrides Function resetSupported() As Boolean
            For Each rr As RecordReader In readers
                If Not rr.resetSupported() Then
                    Return False
                End If
            Next rr
            Return True
        End Function

        Public Overrides Function record(ByVal uri As Uri, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
            Throw New NotSupportedException("Generating records from DataInputStream not supported for ComposableRecordReader")
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

    End Class

End Namespace
