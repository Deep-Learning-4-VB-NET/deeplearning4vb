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
Imports System.IO
Imports DeepLearning4VB.JavaCompat

Namespace org.datavec.api.io

    ''' <summary>
    ''' A reusable <see cref="DataOutputStream"/> over a growable byte array. <see cref="Data"/> returns
    ''' the backing array itself (valid up to <see cref="Length"/>), like the Java original.
    ''' <see cref="DataOutputStream.size"/> keeps counting across <see cref="reset"/> (the counter is
    ''' private to the base class); use <see cref="Length"/> for the valid data length.
    ''' </summary>
    Public Class DataOutputBuffer
        Inherits DataOutputStream

        ''' <summary>The Java <c>ByteArrayOutputStream</c> with direct access to its backing.</summary>
        Private Class Buffer
            Inherits Stream

            Friend buf() As Byte
            Friend count As Integer

            Public Sub New()
                Me.New(32)
            End Sub

            Public Sub New(ByVal size As Integer)
                If size < 0 Then Throw New ArgumentException("Negative initial size: " & size)
                buf = New Byte(size - 1) {}
            End Sub

            Public Sub writeFrom(ByVal [in] As DataInput, ByVal len As Integer)
                Dim newcount As Integer = count + len
                ensureCapacity(newcount)
                [in].readFully(buf, count, len)
                count = newcount
            End Sub

            Private Sub ensureCapacity(ByVal newcount As Integer)
                If newcount > buf.Length Then
                    Dim newbuf(Math.Max(buf.Length << 1, newcount) - 1) As Byte
                    Array.Copy(buf, 0, newbuf, 0, count)
                    buf = newbuf
                End If
            End Sub

            Public Sub reset()
                count = 0
            End Sub

            Public Sub writeTo(ByVal [out] As Stream)
                [out].Write(buf, 0, count)
            End Sub

            Public Overrides ReadOnly Property CanRead As Boolean
                Get
                    Return False
                End Get
            End Property

            Public Overrides ReadOnly Property CanSeek As Boolean
                Get
                    Return False
                End Get
            End Property

            Public Overrides ReadOnly Property CanWrite As Boolean
                Get
                    Return True
                End Get
            End Property

            Public Overrides ReadOnly Property Length As Long
                Get
                    Return count
                End Get
            End Property

            Public Overrides Property Position As Long
                Get
                    Return count
                End Get
                Set(ByVal value As Long)
                    Throw New NotSupportedException()
                End Set
            End Property

            Public Overrides Sub Flush()
            End Sub

            Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
                Throw New NotSupportedException()
            End Function

            Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
                Throw New NotSupportedException()
            End Function

            Public Overrides Sub SetLength(ByVal value As Long)
                Throw New NotSupportedException()
            End Sub

            Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal length As Integer)
                ensureCapacity(count + length)
                Array.Copy(buffer, offset, buf, count, length)
                count += length
            End Sub

            Public Overrides Sub WriteByte(ByVal value As Byte)
                ensureCapacity(count + 1)
                buf(count) = value
                count += 1
            End Sub
        End Class

        Private ReadOnly backing As Buffer

        ''' <summary>
        ''' Constructs a new empty backing. </summary>
        Public Sub New()
            Me.New(New Buffer())
        End Sub

        Public Sub New(ByVal size As Integer)
            Me.New(New Buffer(size))
        End Sub

        Private Sub New(ByVal buffer As Buffer)
            MyBase.New(buffer, True)
            Me.backing = buffer
        End Sub

        ''' <summary>
        ''' Returns the current contents of the backing.
        '''  Data is only valid to <seealso cref="Length"/>.
        ''' </summary>
        Public Overridable ReadOnly Property Data As Byte()
            Get
                Return backing.buf
            End Get
        End Property

        Public Function getData() As Byte()
            Return Data
        End Function

        ''' <summary>
        ''' Returns the length of the valid data currently in the backing. </summary>
        Public Overridable ReadOnly Property Length As Integer
            Get
                Return backing.count
            End Get
        End Property

        Public Function getLength() As Integer
            Return Length
        End Function

        ''' <summary>
        ''' Resets the buffer to empty. </summary>
        Public Overridable Function reset() As DataOutputBuffer
            backing.reset()
            Return Me
        End Function

        ''' <summary>
        ''' Writes bytes from a DataInput directly into the backing. </summary>
        Public Overloads Sub write(ByVal [in] As DataInput, ByVal length As Integer)
            backing.writeFrom([in], length)
        End Sub

        ''' <summary>
        ''' Write to a file stream </summary>
        Public Overridable Sub writeTo(ByVal [out] As Stream)
            backing.writeTo([out])
        End Sub
    End Class

End Namespace
