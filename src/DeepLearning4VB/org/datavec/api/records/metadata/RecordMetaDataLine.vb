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

    ''' <summary>Record metadata for one line of a text source (Lombok <c>@AllArgsConstructor @Data</c> in Java).</summary>
    <Serializable>
    Public Class RecordMetaDataLine
        Implements RecordMetaData

        Private lineNumberValue As Integer
        Private uriValue As Uri
        Private readerClassValue As Type

        Public Sub New(ByVal lineNumber As Integer, ByVal uri As Uri, ByVal readerClass As Type)
            Me.lineNumberValue = lineNumber
            Me.uriValue = uri
            Me.readerClassValue = readerClass
        End Sub

        Public Function getLineNumber() As Integer
            Return lineNumberValue
        End Function

        Public Sub setLineNumber(ByVal lineNumber As Integer)
            lineNumberValue = lineNumber
        End Sub

        Public Sub setUri(ByVal uri As Uri)
            uriValue = uri
        End Sub

        Public Sub setReaderClass(ByVal readerClass As Type)
            readerClassValue = readerClass
        End Sub

        Public Property LineNumber As Integer
            Get
                Return lineNumberValue
            End Get
            Set(ByVal value As Integer)
                lineNumberValue = value
            End Set
        End Property

        Public Overridable Function getLocation() As String Implements RecordMetaData.getLocation
            Dim filename As String
            If uriValue IsNot Nothing Then
                filename = baseNameAndExtension(uriValue.ToString()) & " "
            Else
                filename = ""
            End If
            Return filename & "line " & lineNumberValue
        End Function

        ''' <summary>Replaces <c>FilenameUtils.getBaseName(str) + "." + FilenameUtils.getExtension(str)</c>.</summary>
        Friend Shared Function baseNameAndExtension(ByVal str As String) As String
            Dim name As String = str
            Dim slash As Integer = Math.Max(name.LastIndexOf("/"c), name.LastIndexOf("\"c))
            If slash >= 0 Then name = name.Substring(slash + 1)
            Dim dot As Integer = name.LastIndexOf("."c)
            Dim baseName As String = If(dot >= 0, name.Substring(0, dot), name)
            Dim extension As String = If(dot >= 0, name.Substring(dot + 1), "")
            Return baseName & "." & extension
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
            Dim other As RecordMetaDataLine = TryCast(o, RecordMetaDataLine)
            If other Is Nothing Then Return False
            If Not other.canEqual(Me) Then Return False
            Return lineNumberValue = other.lineNumberValue AndAlso Objects.equals(uriValue, other.uriValue) AndAlso Objects.equals(readerClassValue, other.readerClassValue)
        End Function

        Protected Overridable Function canEqual(ByVal other As Object) As Boolean
            Return TypeOf other Is RecordMetaDataLine
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = 1
            result = result * 59 + lineNumberValue
            result = result * 59 + If(uriValue Is Nothing, 43, uriValue.GetHashCode())
            result = result * 59 + If(readerClassValue Is Nothing, 43, readerClassValue.GetHashCode())
            Return result
        End Function

        Public Overrides Function ToString() As String
            Return "RecordMetaDataLine(lineNumber=" & lineNumberValue & ", uri=" & Objects.toString(uriValue) & ", readerClass=" & If(readerClassValue Is Nothing, "null", readerClassValue.FullName) & ")"
        End Function

    End Class

End Namespace
