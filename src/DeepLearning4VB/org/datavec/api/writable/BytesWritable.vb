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
Imports System.Linq
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat

Namespace org.datavec.api.writable

    ''' <summary>
    ''' A writable over a raw byte array, indexable as big-endian doubles, floats, ints and longs
    ''' (Java's <c>ByteBuffer.wrap</c> default order). The Java <c>asNd4jBuffer</c> method needs
    ''' the ND4J <c>DataBuffer</c> API and is not ported yet.
    ''' </summary>
    <Serializable>
    Public Class BytesWritable
        Inherits ArrayWritable

        Private contentField() As Byte

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Pass in the content for this writable </summary>
        ''' <param name="content"> the content for this writable </param>
        <JsonConstructor>
        Public Sub New(ByVal content() As Byte)
            Me.contentField = content
        End Sub

        <JsonPropertyName("content")>
        Public Property Content As Byte()
            Get
                Return contentField
            End Get
            Set(ByVal value As Byte())
                contentField = value
            End Set
        End Property

        Public Function getContent() As Byte()
            Return contentField
        End Function

        Public Sub setContent(ByVal content() As Byte)
            contentField = content
        End Sub

        Public Overrides Function length() As Long
            Return contentField.Length
        End Function

        Public Overrides Function getDouble(ByVal i As Long) As Double
            Return BitConverter.Int64BitsToDouble(ReadLongAt(CInt(i) * 8))
        End Function

        Public Overrides Function getFloat(ByVal i As Long) As Single
            Return BitConverter.ToSingle(BitConverter.GetBytes(ReadIntAt(CInt(i) * 4)), 0)
        End Function

        Public Overrides Function getInt(ByVal i As Long) As Integer
            Return ReadIntAt(CInt(i) * 4)
        End Function

        Public Overrides Function getLong(ByVal i As Long) As Long
            Return ReadLongAt(CInt(i) * 8)
        End Function

        Private Function ReadIntAt(ByVal offset As Integer) As Integer
            If offset < 0 OrElse offset + 4 > contentField.Length Then Throw New IndexOutOfRangeException()
            Return (CInt(contentField(offset)) << 24) Or (CInt(contentField(offset + 1)) << 16) Or (CInt(contentField(offset + 2)) << 8) Or CInt(contentField(offset + 3))
        End Function

        Private Function ReadLongAt(ByVal offset As Integer) As Long
            If offset < 0 OrElse offset + 8 > contentField.Length Then Throw New IndexOutOfRangeException()
            Dim v As Long = 0
            For k As Integer = 0 To 7
                v = (v << 8) Or CLng(contentField(offset + k))
            Next k
            Return v
        End Function

        ''' <summary>Writes the raw bytes (no length prefix, like the Java original).</summary>
        Public Overrides Sub write(ByVal [out] As DataOutput)
            [out].write(contentField)
        End Sub

        ''' <summary>Fills the existing content array, which must already have the right length (Java behaviour).</summary>
        Public Overrides Sub readFields(ByVal [in] As DataInput)
            [in].readFully(contentField)
        End Sub

        Public Overrides Sub writeType(ByVal [out] As DataOutput)
            [out].writeShort([getType]().typeIdx())
        End Sub

        Public Overrides Function [getType]() As WritableType
            Return WritableType.Bytes
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            If o Is Nothing OrElse DirectCast(Me, Object).GetType() IsNot o.GetType() Then
                Return False
            End If
            Dim that As BytesWritable = DirectCast(o, BytesWritable)
            If contentField Is Nothing OrElse that.contentField Is Nothing Then Return contentField Is that.contentField
            Return contentField.SequenceEqual(that.contentField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return WritableHelpers.javaByteArrayHash(contentField)
        End Function

        Public Overrides Function ToString() As String
            Return "BytesWritable(" & If(contentField Is Nothing, "null", contentField.Length & " bytes") & ")"
        End Function
    End Class

End Namespace
