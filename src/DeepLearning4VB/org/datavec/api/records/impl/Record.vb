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
Imports System.Collections
Imports System.Collections.Generic
Imports DeepLearning4VB.JavaCompat
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData

Namespace org.datavec.api.records.impl

    ''' <summary>
    ''' Default <see cref="org.datavec.api.records.Record"/> implementation (Lombok <c>@AllArgsConstructor @Data</c>
    ''' in Java: constructor, accessors, <c>Equals</c>, <c>GetHashCode</c> and <c>ToString</c> are written out).
    ''' </summary>
    <Serializable>
    Public Class Record
        Implements org.datavec.api.records.Record

        Private recordValue As IList(Of writable.Writable)
        Private metaDataValue As RecordMetaData

        Public Sub New(ByVal record As IList(Of writable.Writable), ByVal metaData As RecordMetaData)
            Me.recordValue = record
            Me.metaDataValue = metaData
        End Sub

        Public Overridable Function getRecord() As IList(Of writable.Writable) Implements org.datavec.api.records.Record.getRecord
            Return recordValue
        End Function

        Public Overridable Sub setRecord(ByVal record As IList(Of writable.Writable)) Implements org.datavec.api.records.Record.setRecord
            Me.recordValue = record
        End Sub

        Public Overridable Function getMetaData() As RecordMetaData Implements org.datavec.api.records.Record.getMetaData
            Return metaDataValue
        End Function

        Public Overridable Sub setMetaData(ByVal metaData As RecordMetaData) Implements org.datavec.api.records.Record.setMetaData
            Me.metaDataValue = metaData
        End Sub

        Public Overridable Property Record As IList(Of writable.Writable) Implements org.datavec.api.records.Record.Record
            Get
                Return recordValue
            End Get
            Set(ByVal value As IList(Of writable.Writable))
                recordValue = value
            End Set
        End Property

        Public Overridable Property MetaData As RecordMetaData Implements org.datavec.api.records.Record.MetaData
            Get
                Return metaDataValue
            End Get
            Set(ByVal value As RecordMetaData)
                metaDataValue = value
            End Set
        End Property

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then Return True
            Dim other = TryCast(o, Record)
            If other Is Nothing OrElse Not other.canEqual(Me) Then Return False
            Return RecordEquality.listEquals(recordValue, other.recordValue) AndAlso Objects.equals(metaDataValue, other.metaDataValue)
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is Record
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + RecordEquality.listHashCode(recordValue)
            result = result * 59 + If(metaDataValue Is Nothing, 43, metaDataValue.GetHashCode())
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "Record(record=" & RecordEquality.listToString(recordValue) & ", metaData=" & Objects.toString(metaDataValue) & ")"
        End Function

    End Class

    ''' <summary>
    ''' Java <c>List.equals</c>/<c>hashCode</c>/<c>toString</c> semantics for the (possibly nested) writable lists
    ''' held by records: lists are compared element by element, recursively.
    ''' </summary>
    Friend Module RecordEquality

        Public Function listEquals(ByVal a As IEnumerable, ByVal b As IEnumerable) As Boolean
            If a Is b Then Return True
            If a Is Nothing OrElse b Is Nothing Then Return False
            Dim ea As IEnumerator = a.GetEnumerator()
            Dim eb As IEnumerator = b.GetEnumerator()
            Do
                Dim ha As Boolean = ea.MoveNext()
                Dim hb As Boolean = eb.MoveNext()
                If ha <> hb Then Return False
                If Not ha Then Return True
                If Not elementEquals(ea.Current, eb.Current) Then Return False
            Loop
        End Function

        Private Function elementEquals(ByVal x As Object, ByVal y As Object) As Boolean
            If x Is Nothing OrElse y Is Nothing Then Return x Is y
            If TypeOf x Is String OrElse TypeOf y Is String Then Return x.Equals(y)
            If TypeOf x Is IEnumerable AndAlso TypeOf y Is IEnumerable Then Return listEquals(DirectCast(x, IEnumerable), DirectCast(y, IEnumerable))
            Return x.Equals(y)
        End Function

        Public Function listHashCode(ByVal list As IEnumerable) As Integer
            If list Is Nothing Then Return 0
            Dim h As Integer = 1
            For Each e As Object In list
                Dim eh As Integer
                If e Is Nothing Then
                    eh = 0
                ElseIf TypeOf e Is IEnumerable AndAlso Not TypeOf e Is String Then
                    eh = listHashCode(DirectCast(e, IEnumerable))
                Else
                    eh = e.GetHashCode()
                End If
                h = 31 * h + eh
            Next e
            Return h
        End Function

        Public Function listToString(ByVal list As IEnumerable) As String
            If list Is Nothing Then Return "null"
            Dim parts As New List(Of String)()
            For Each e As Object In list
                If e IsNot Nothing AndAlso TypeOf e Is IEnumerable AndAlso Not TypeOf e Is String Then
                    parts.Add(listToString(DirectCast(e, IEnumerable)))
                Else
                    parts.Add(Objects.toString(e))
                End If
            Next e
            Return "[" & String.Join(", ", parts) & "]"
        End Function

    End Module

End Namespace
