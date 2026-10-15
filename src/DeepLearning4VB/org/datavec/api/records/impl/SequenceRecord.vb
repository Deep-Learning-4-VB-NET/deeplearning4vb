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
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData

Namespace org.datavec.api.records.impl

    ''' <summary>
    ''' Default <see cref="org.datavec.api.records.SequenceRecord"/> implementation (Lombok <c>@AllArgsConstructor @Data</c>
    ''' in Java: constructor, accessors, <c>Equals</c>, <c>GetHashCode</c> and <c>ToString</c> are written out).
    ''' </summary>
    <Serializable>
    Public Class SequenceRecord
        Implements org.datavec.api.records.SequenceRecord

        Private sequenceRecordValue As IList(Of IList(Of writable.Writable))
        Private metaDataValue As RecordMetaData

        Public Sub New(ByVal sequenceRecord As IList(Of IList(Of writable.Writable)), ByVal metaData As RecordMetaData)
            Me.sequenceRecordValue = sequenceRecord
            Me.metaDataValue = metaData
        End Sub

        Public Overridable Function getSequenceRecord() As IList(Of IList(Of writable.Writable)) Implements org.datavec.api.records.SequenceRecord.getSequenceRecord
            Return sequenceRecordValue
        End Function

        Public Overridable Sub setSequenceRecord(ByVal sequenceRecord As IList(Of IList(Of writable.Writable))) Implements org.datavec.api.records.SequenceRecord.setSequenceRecord
            Me.sequenceRecordValue = sequenceRecord
        End Sub

        Public Overridable Function getMetaData() As RecordMetaData Implements org.datavec.api.records.SequenceRecord.getMetaData
            Return metaDataValue
        End Function

        Public Overridable Sub setMetaData(ByVal metaData As RecordMetaData) Implements org.datavec.api.records.SequenceRecord.setMetaData
            Me.metaDataValue = metaData
        End Sub

        Public Overridable Property SequenceRecord As IList(Of IList(Of writable.Writable)) Implements org.datavec.api.records.SequenceRecord.SequenceRecord
            Get
                Return sequenceRecordValue
            End Get
            Set(ByVal value As IList(Of IList(Of writable.Writable)))
                sequenceRecordValue = value
            End Set
        End Property

        Public Overridable Property MetaData As RecordMetaData Implements org.datavec.api.records.SequenceRecord.MetaData
            Get
                Return metaDataValue
            End Get
            Set(ByVal value As RecordMetaData)
                metaDataValue = value
            End Set
        End Property

        Public Overridable Function getSequenceLength() As Integer Implements org.datavec.api.records.SequenceRecord.getSequenceLength
            If sequenceRecordValue Is Nothing Then
                Return 0
            End If
            Return sequenceRecordValue.Count
        End Function

        Public Overridable ReadOnly Property SequenceLength As Integer Implements org.datavec.api.records.SequenceRecord.SequenceLength
            Get
                Return getSequenceLength()
            End Get
        End Property

        Public Overridable Function getTimeStep(ByVal timeStep As Integer) As IList(Of writable.Writable) Implements org.datavec.api.records.SequenceRecord.getTimeStep
            If timeStep < 0 OrElse timeStep > sequenceRecordValue.Count Then
                Throw New ArgumentException("Invalid input: " & sequenceRecordValue.Count & " time steps available; cannot get " & timeStep)
            End If
            Return sequenceRecordValue(timeStep)
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then Return True
            Dim other = TryCast(o, SequenceRecord)
            If other Is Nothing OrElse Not other.canEqual(Me) Then Return False
            Return RecordEquality.listEquals(sequenceRecordValue, other.sequenceRecordValue) AndAlso Objects.equals(metaDataValue, other.metaDataValue)
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is SequenceRecord
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + RecordEquality.listHashCode(sequenceRecordValue)
            result = result * 59 + If(metaDataValue Is Nothing, 43, metaDataValue.GetHashCode())
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "SequenceRecord(sequenceRecord=" & RecordEquality.listToString(sequenceRecordValue) & ", metaData=" & Objects.toString(metaDataValue) & ")"
        End Function

    End Class

End Namespace
