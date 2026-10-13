Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

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

Namespace org.datavec.api.records.metadata

    ''' <summary>Record metadata composed of named metadata objects (Lombok <c>@Data</c> in Java).</summary>
    <Serializable>
    Public Class RecordMetaDataComposableMap
        Implements RecordMetaData

        Private readerClassValue As Type
        Private metaValue As IDictionary(Of String, RecordMetaData)

        Public Sub New(ByVal recordMetaDatas As IDictionary(Of String, RecordMetaData))
            Me.New(Nothing, recordMetaDatas)
        End Sub

        Public Sub New(ByVal readerClass As Type, ByVal recordMetaDatas As IDictionary(Of String, RecordMetaData))
            Me.readerClassValue = readerClass
            Me.metaValue = recordMetaDatas
        End Sub

        Public Function getMeta() As IDictionary(Of String, RecordMetaData)
            Return metaValue
        End Function

        Public Sub setMeta(ByVal meta As IDictionary(Of String, RecordMetaData))
            metaValue = meta
        End Sub

        Public Sub setReaderClass(ByVal readerClass As Type)
            readerClassValue = readerClass
        End Sub

        Public Property Meta As IDictionary(Of String, RecordMetaData)
            Get
                Return metaValue
            End Get
            Set(ByVal value As IDictionary(Of String, RecordMetaData))
                metaValue = value
            End Set
        End Property

        Public Overridable Function getLocation() As String Implements RecordMetaData.getLocation
            Dim sb As New StringBuilder()
            sb.Append("locations(")
            Dim first As Boolean = True
            For Each rmd As KeyValuePair(Of String, RecordMetaData) In metaValue
                If Not first Then
                    sb.Append(",")
                End If
                sb.Append(rmd.Key).Append("=")
                sb.Append(rmd.Value.getLocation())
                first = False
            Next rmd
            sb.Append(")")
            Return sb.ToString()
        End Function

        Public Overridable Function getURI() As Uri Implements RecordMetaData.getURI
            Dim first As String = metaValue.Keys.First()
            Return metaValue(first).getURI()
        End Function

        Public Overridable Function getReaderClass() As Type Implements RecordMetaData.getReaderClass
            Return readerClassValue
        End Function

        Public ReadOnly Property Location As String Implements RecordMetaData.Location
            Get
                Return getLocation()
            End Get
        End Property

        Public ReadOnly Property URI As Uri Implements RecordMetaData.URI
            Get
                Return getURI()
            End Get
        End Property

        Public Property ReaderClass As Type Implements RecordMetaData.ReaderClass
            Get
                Return getReaderClass()
            End Get
            Set(ByVal value As Type)
                setReaderClass(value)
            End Set
        End Property

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then Return True
            Dim other As RecordMetaDataComposableMap = TryCast(o, RecordMetaDataComposableMap)
            If other Is Nothing Then Return False
            If Not other.canEqual(Me) Then Return False
            Return Objects.equals(readerClassValue, other.readerClassValue) AndAlso mapEquals(metaValue, other.metaValue)
        End Function

        Private Shared Function mapEquals(ByVal a As IDictionary(Of String, RecordMetaData), ByVal b As IDictionary(Of String, RecordMetaData)) As Boolean
            If a Is b Then Return True
            If a Is Nothing OrElse b Is Nothing Then Return False
            If a.Count <> b.Count Then Return False
            For Each kv As KeyValuePair(Of String, RecordMetaData) In a
                Dim v As RecordMetaData = Nothing
                If Not b.TryGetValue(kv.Key, v) Then Return False
                If Not Objects.equals(kv.Value, v) Then Return False
            Next kv
            Return True
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is RecordMetaDataComposableMap
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(readerClassValue Is Nothing, 43, readerClassValue.GetHashCode())
            Dim mapHash As Integer = 0
            If metaValue IsNot Nothing Then
                ' java.util.Map.hashCode: sum of (key.hashCode ^ value.hashCode)
                For Each kv As KeyValuePair(Of String, RecordMetaData) In metaValue
                    mapHash += Objects.hashCode(kv.Key) Xor Objects.hashCode(kv.Value)
                Next kv
            Else
                mapHash = 43
            End If
            result = result * 59 + mapHash
            Return result
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("RecordMetaDataComposableMap(readerClass=").Append(If(readerClassValue Is Nothing, "null", readerClassValue.FullName)).Append(", meta=")
            If metaValue Is Nothing Then
                sb.Append("null")
            Else
                sb.Append("{")
                Dim first As Boolean = True
                For Each kv As KeyValuePair(Of String, RecordMetaData) In metaValue
                    If Not first Then sb.Append(", ")
                    sb.Append(kv.Key).Append("=").Append(Objects.toString(kv.Value))
                    first = False
                Next kv
                sb.Append("}")
            End If
            sb.Append(")")
            Return sb.ToString()
        End Function

    End Class

End Namespace
