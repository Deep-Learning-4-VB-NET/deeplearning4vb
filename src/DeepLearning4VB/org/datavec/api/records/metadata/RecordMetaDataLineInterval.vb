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

    ''' <summary>Record metadata for a range of lines of a text source (Lombok <c>@AllArgsConstructor @Data</c> in Java).</summary>
    <Serializable>
    Public Class RecordMetaDataLineInterval
        Implements RecordMetaData

        Private lineNumberStartValue As Integer
        Private lineNumberEndValue As Integer
        Private uriValue As Uri
        Private readerClassValue As Type

        Public Sub New(ByVal lineNumberStart As Integer, ByVal lineNumberEnd As Integer, ByVal uri As Uri, ByVal readerClass As Type)
            Me.lineNumberStartValue = lineNumberStart
            Me.lineNumberEndValue = lineNumberEnd
            Me.uriValue = uri
            Me.readerClassValue = readerClass
        End Sub

        Public Function getLineNumberStart() As Integer
            Return lineNumberStartValue
        End Function

        Public Function getLineNumberEnd() As Integer
            Return lineNumberEndValue
        End Function

        Public Sub setLineNumberStart(ByVal lineNumberStart As Integer)
            lineNumberStartValue = lineNumberStart
        End Sub

        Public Sub setLineNumberEnd(ByVal lineNumberEnd As Integer)
            lineNumberEndValue = lineNumberEnd
        End Sub

        Public Sub setUri(ByVal uri As Uri)
            uriValue = uri
        End Sub

        Public Sub setReaderClass(ByVal readerClass As Type)
            readerClassValue = readerClass
        End Sub

        Public Property LineNumberStart As Integer
            Get
                Return lineNumberStartValue
            End Get
            Set(ByVal value As Integer)
                lineNumberStartValue = value
            End Set
        End Property

        Public Property LineNumberEnd As Integer
            Get
                Return lineNumberEndValue
            End Get
            Set(ByVal value As Integer)
                lineNumberEndValue = value
            End Set
        End Property

        Public Overridable Function getLocation() As String Implements RecordMetaData.getLocation
            Dim filename As String
            If uriValue IsNot Nothing Then
                filename = RecordMetaDataLine.baseNameAndExtension(uriValue.ToString()) & " "
            Else
                filename = ""
            End If
            Return filename & "lines " & lineNumberStartValue & "-" & lineNumberEndValue
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

        Public Property URI As Uri Implements RecordMetaData.URI
            Get
                Return getURI()
            End Get
            Set(ByVal value As Uri)
                setUri(value)
            End Set
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
            Dim other As RecordMetaDataLineInterval = TryCast(o, RecordMetaDataLineInterval)
            If other Is Nothing Then Return False
            If Not other.canEqual(Me) Then Return False
            Return lineNumberStartValue = other.lineNumberStartValue AndAlso lineNumberEndValue = other.lineNumberEndValue AndAlso
                Objects.equals(uriValue, other.uriValue) AndAlso Objects.equals(readerClassValue, other.readerClassValue)
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is RecordMetaDataLineInterval
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + lineNumberStartValue
            result = result * 59 + lineNumberEndValue
            result = result * 59 + If(uriValue Is Nothing, 43, uriValue.GetHashCode())
            result = result * 59 + If(readerClassValue Is Nothing, 43, readerClassValue.GetHashCode())
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "RecordMetaDataLineInterval(lineNumberStart=" & lineNumberStartValue & ", lineNumberEnd=" & lineNumberEndValue &
                ", uri=" & Objects.toString(uriValue) & ", readerClass=" & If(readerClassValue Is Nothing, "null", readerClassValue.FullName) & ")"
        End Function

    End Class

End Namespace
