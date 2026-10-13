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
    ''' A reusable <see cref="DataInputStream"/> over a byte array that can be re-pointed at new data
    ''' with <see cref="reset(Byte(), Integer, Integer)"/> without allocating a new stream.
    ''' </summary>
    Public Class DataInputBuffer
        Inherits DataInputStream

        ''' <summary>The Java <c>ByteArrayInputStream</c> whose backing array can be swapped.</summary>
        Private Class Buffer
            Inherits Stream

            Friend buf() As Byte = New Byte(-1) {}
            Friend count As Integer
            Friend pos As Integer
            Friend markPos As Integer

            Public Sub reset(ByVal input() As Byte, ByVal start As Integer, ByVal length As Integer)
                Me.buf = input
                Me.count = start + length
                Me.markPos = start
                Me.pos = start
            End Sub

            Public Overrides ReadOnly Property CanRead As Boolean
                Get
                    Return True
                End Get
            End Property

            Public Overrides ReadOnly Property CanSeek As Boolean
                Get
                    Return True
                End Get
            End Property

            Public Overrides ReadOnly Property CanWrite As Boolean
                Get
                    Return False
                End Get
            End Property

            Public Overrides ReadOnly Property Length As Long
                Get
                    Return count
                End Get
            End Property

            Public Overrides Property Position As Long
                Get
                    Return pos
                End Get
                Set(ByVal value As Long)
                    pos = CInt(value)
                End Set
            End Property

            Public Overrides Sub Flush()
            End Sub

            Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal length As Integer) As Integer
                Dim n As Integer = Math.Min(length, count - pos)
                If n <= 0 Then Return 0
                Array.Copy(buf, pos, buffer, offset, n)
                pos += n
                Return n
            End Function

            Public Overrides Function ReadByte() As Integer
                If pos >= count Then Return -1
                Dim b As Integer = buf(pos)
                pos += 1
                Return b
            End Function

            Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
                Select Case origin
                    Case SeekOrigin.Begin
                        pos = CInt(offset)
                    Case SeekOrigin.Current
                        pos = CInt(pos + offset)
                    Case Else
                        pos = CInt(count + offset)
                End Select
                Return pos
            End Function

            Public Overrides Sub SetLength(ByVal value As Long)
                Throw New NotSupportedException()
            End Sub

            Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
                Throw New NotSupportedException()
            End Sub
        End Class

        Private ReadOnly backing As Buffer

        ''' <summary>
        ''' Constructs a new empty backing. </summary>
        Public Sub New()
            Me.New(New Buffer())
        End Sub

        Private Sub New(ByVal buffer As Buffer)
            MyBase.New(buffer, True)
            Me.backing = buffer
        End Sub

        ''' <summary>
        ''' Resets the data that the buffer reads. </summary>
        Public Overridable Sub reset(ByVal input() As Byte, ByVal length As Integer)
            backing.reset(input, 0, length)
        End Sub

        ''' <summary>
        ''' Resets the data that the buffer reads. </summary>
        Public Overridable Sub reset(ByVal input() As Byte, ByVal start As Integer, ByVal length As Integer)
            backing.reset(input, start, length)
        End Sub

        ''' <summary>The backing array (valid between the start given to <c>reset</c> and <see cref="Length"/>).</summary>
        Public Overridable ReadOnly Property Data As Byte()
            Get
                Return backing.buf
            End Get
        End Property

        Public Function getData() As Byte()
            Return Data
        End Function

        ''' <summary>
        ''' Returns the current position in the input. </summary>
        Public Overridable ReadOnly Property Position As Integer
            Get
                Return backing.pos
            End Get
        End Property

        Public Function getPosition() As Integer
            Return Position
        End Function

        ''' <summary>
        ''' Returns the length of the input. </summary>
        Public Overridable ReadOnly Property Length As Integer
            Get
                Return backing.count
            End Get
        End Property

        Public Function getLength() As Integer
            Return Length
        End Function

    End Class

End Namespace
