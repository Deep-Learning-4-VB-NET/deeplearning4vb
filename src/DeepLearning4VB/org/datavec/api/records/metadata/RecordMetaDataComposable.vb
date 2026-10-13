Imports System
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

    ''' <summary>Record metadata composed of several other metadata objects (Lombok <c>@Data</c> in Java).</summary>
    <Serializable>
    Public Class RecordMetaDataComposable
        Implements RecordMetaData

        Private readerClassValue As Type
        Private metaValue() As RecordMetaData

        Public Sub New(ByVal ParamArray recordMetaDatas() As RecordMetaData)
            Me.New(DirectCast(Nothing, Type), recordMetaDatas)
        End Sub

        Public Sub New(ByVal readerClass As Type, ByVal ParamArray recordMetaDatas() As RecordMetaData)
            Me.readerClassValue = readerClass
            Me.metaValue = recordMetaDatas
        End Sub

        Public Function getMeta() As RecordMetaData()
            Return metaValue
        End Function

        Public Sub setMeta(ByVal meta() As RecordMetaData)
            metaValue = meta
        End Sub

        Public Sub setReaderClass(ByVal readerClass As Type)
            readerClassValue = readerClass
        End Sub

        Public Property Meta As RecordMetaData()
            Get
                Return metaValue
            End Get
            Set(ByVal value As RecordMetaData())
                metaValue = value
            End Set
        End Property

        Public Overridable Function getLocation() As String Implements RecordMetaData.getLocation
            Dim sb As New StringBuilder()
            sb.Append("locations(")
            Dim first As Boolean = True
            For Each rmd As RecordMetaData In metaValue
                If Not first Then
                    sb.Append(",")
                End If
                sb.Append(rmd.getLocation())
                first = False
            Next rmd
            sb.Append(")")
            Return sb.ToString()
        End Function

        Public Overridable Function getURI() As Uri Implements RecordMetaData.getURI
            Return metaValue(0).getURI()
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
            Dim other As RecordMetaDataComposable = TryCast(o, RecordMetaDataComposable)
            If other Is Nothing Then Return False
            If Not other.canEqual(Me) Then Return False
            Return Objects.equals(readerClassValue, other.readerClassValue) AndAlso Arrays.equals(metaValue, other.metaValue)
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is RecordMetaDataComposable
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(readerClassValue Is Nothing, 43, readerClassValue.GetHashCode())
            result = result * 59 + Arrays.hashCode(metaValue)
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "RecordMetaDataComposable(readerClass=" & If(readerClassValue Is Nothing, "null", readerClassValue.FullName) & ", meta=" & Arrays.deepToString(metaValue) & ")"
        End Function

    End Class

End Namespace
