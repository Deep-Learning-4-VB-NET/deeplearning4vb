Imports System

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

    ''' <summary>Record metadata keyed by the URI of the source (Lombok <c>@AllArgsConstructor @Data</c> in Java).</summary>
    <Serializable>
    Public Class RecordMetaDataURI
        Implements RecordMetaData

        Private ReadOnly uriValue As Uri
        Private ReadOnly readerClassValue As Type

        Public Sub New(ByVal uri As Uri)
            Me.New(uri, Nothing)
        End Sub

        Public Sub New(ByVal uri As Uri, ByVal readerClass As Type)
            Me.uriValue = uri
            Me.readerClassValue = readerClass
        End Sub

        Public Overridable Function getLocation() As String Implements RecordMetaData.getLocation
            Return uriValue.ToString()
        End Function

        Public Overridable Function getURI() As Uri Implements RecordMetaData.getURI
            Return uriValue
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

        Public ReadOnly Property ReaderClass As Type Implements RecordMetaData.ReaderClass
            Get
                Return getReaderClass()
            End Get
        End Property

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then Return True
            Dim other As RecordMetaDataURI = TryCast(o, RecordMetaDataURI)
            If other Is Nothing Then Return False
            If Not other.canEqual(Me) Then Return False
            Return Objects.equals(uriValue, other.uriValue) AndAlso Objects.equals(readerClassValue, other.readerClassValue)
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is RecordMetaDataURI
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + If(uriValue Is Nothing, 43, uriValue.GetHashCode())
            result = result * 59 + If(readerClassValue Is Nothing, 43, readerClassValue.GetHashCode())
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "RecordMetaDataURI(uri=" & Objects.toString(uriValue) & ", readerClass=" & If(readerClassValue Is Nothing, "null", readerClassValue.FullName) & ")"
        End Function

    End Class

End Namespace
