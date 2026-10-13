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
Imports System.Text

Namespace org.datavec.api.records

    ''' <summary>
    ''' A byte sequence that is usable as a key or value. The bytes are <c>Byte()</c> (Java <c>byte[]</c>);
    ''' <c>compareTo</c> orders the bytes as unsigned values, like the Java original.
    ''' </summary>
    Public Class Buffer
        Implements IComparable, ICloneable

        ''' <summary>
        ''' Number of valid bytes in this.bytes (the field is called <c>count</c> in Java; renamed because of the <c>Count</c> property). </summary>
        Private countValue As Integer
        ''' <summary>
        ''' Backing store for Buffer. </summary>
        Private bytes() As Byte = Nothing

        ''' <summary>
        ''' Create a zero-count sequence.
        ''' </summary>
        Public Sub New()
            Me.countValue = 0
        End Sub

        ''' <summary>
        ''' Create a Buffer using the byte array as the initial value.
        ''' </summary>
        ''' <param name="bytes"> This array becomes the backing storage for the object. </param>
        Public Sub New(ByVal bytes() As Byte)
            Me.bytes = bytes
            Me.countValue = If(bytes Is Nothing, 0, bytes.Length)
        End Sub

        ''' <summary>
        ''' Create a Buffer using the byte range as the initial value.
        ''' </summary>
        ''' <param name="bytes"> Copy of this array becomes the backing storage for the object. </param>
        ''' <param name="offset"> offset into byte array </param>
        ''' <param name="length"> length of data </param>
        Public Sub New(ByVal bytes() As Byte, ByVal offset As Integer, ByVal length As Integer)
            copy(bytes, offset, length)
        End Sub

        ''' <summary>
        ''' Use the specified bytes array as underlying sequence.
        ''' </summary>
        ''' <param name="bytes"> byte sequence </param>
        Public Overridable Sub [set](ByVal bytes() As Byte)
            Me.countValue = If(bytes Is Nothing, 0, bytes.Length)
            Me.bytes = bytes
        End Sub

        ''' <summary>
        ''' Copy the specified byte array to the Buffer. Replaces the current buffer.
        ''' </summary>
        ''' <param name="bytes"> byte array to be assigned </param>
        ''' <param name="offset"> offset into byte array </param>
        ''' <param name="length"> length of data </param>
        Public Sub copy(ByVal bytes() As Byte, ByVal offset As Integer, ByVal length As Integer)
            If Me.bytes Is Nothing OrElse Me.bytes.Length < length Then
                Me.bytes = New Byte(length - 1) {}
            End If
            Array.Copy(bytes, offset, Me.bytes, 0, length)
            Me.countValue = length
        End Sub

        ''' <summary>
        ''' Get the data from the Buffer.
        ''' </summary>
        ''' <returns> The data is only valid between 0 and getCount() - 1. </returns>
        Public Overridable Function [get]() As Byte()
            If bytes Is Nothing Then
                bytes = New Byte() {}
            End If
            Return bytes
        End Function

        ''' <summary>
        ''' Get the current count of the buffer.
        ''' </summary>
        Public Overridable ReadOnly Property Count As Integer
            Get
                Return countValue
            End Get
        End Property

        ''' <summary>
        ''' Get the current count of the buffer.
        ''' </summary>
        Public Function getCount() As Integer
            Return countValue
        End Function

        ''' <summary>
        ''' Get the capacity, which is the maximum count that could handled without
        ''' resizing the backing storage.
        ''' </summary>
        ''' <returns> The number of bytes </returns>
        Public Overridable Property Capacity As Integer
            Get
                Return Me.get().Length
            End Get
            Set(ByVal newCapacity As Integer)
                If newCapacity < 0 Then
                    Throw New ArgumentException("Invalid capacity argument " & newCapacity)
                End If
                If newCapacity = 0 Then
                    Me.bytes = Nothing
                    Me.countValue = 0
                    Return
                End If
                If newCapacity <> Capacity Then
                    Dim data(newCapacity - 1) As Byte
                    If newCapacity < countValue Then
                        countValue = newCapacity
                    End If
                    If countValue <> 0 Then
                        Array.Copy(Me.get(), 0, data, 0, countValue)
                    End If
                    bytes = data
                End If
            End Set
        End Property

        Public Function getCapacity() As Integer
            Return Capacity
        End Function

        Public Sub setCapacity(ByVal newCapacity As Integer)
            Capacity = newCapacity
        End Sub

        ''' <summary>
        ''' Reset the buffer to 0 size
        ''' </summary>
        Public Overridable Sub reset()
            Capacity = 0
        End Sub

        ''' <summary>
        ''' Change the capacity of the backing store to be the same as the current
        ''' count of buffer.
        ''' </summary>
        Public Overridable Sub truncate()
            Capacity = countValue
        End Sub

        ''' <summary>
        ''' Append specified bytes to the buffer.
        ''' </summary>
        ''' <param name="bytes"> byte array to be appended </param>
        ''' <param name="offset"> offset into byte array </param>
        ''' <param name="length"> length of data
        '''  </param>
        Public Overridable Sub append(ByVal bytes() As Byte, ByVal offset As Integer, ByVal length As Integer)
            Capacity = countValue + length
            Array.Copy(bytes, offset, Me.get(), countValue, length)
            countValue = countValue + length
        End Sub

        ''' <summary>
        ''' Append specified bytes to the buffer
        ''' </summary>
        ''' <param name="bytes"> byte array to be appended </param>
        Public Overridable Sub append(ByVal bytes() As Byte)
            append(bytes, 0, bytes.Length)
        End Sub

        ' inherit javadoc
        Public Overrides Function GetHashCode() As Integer
            Dim hash As Integer = 1
            Dim b() As Byte = Me.get()
            For i As Integer = 0 To countValue - 1
                ' Java bytes are signed: use the sign-extended value
                hash = (31 * hash) + If(b(i) > 127, CInt(b(i)) - 256, CInt(b(i)))
            Next i
            Return hash
        End Function

        ''' <summary>
        ''' Define the sort order of the Buffer.
        ''' </summary>
        ''' <param name="other"> The other buffer </param>
        ''' <returns> Positive if this is bigger than other, 0 if they are equal, and
        '''         negative if this is smaller than other. </returns>
        Public Function CompareTo(ByVal other As Object) As Integer Implements IComparable.CompareTo
            Dim right As Buffer = DirectCast(other, Buffer)
            Dim lb() As Byte = Me.get()
            Dim rb() As Byte = right.get()
            Dim i As Integer = 0
            Do While i < countValue AndAlso i < right.countValue
                Dim a As Integer = (lb(i) And &HFF)
                Dim b As Integer = (rb(i) And &HFF)
                If a <> b Then
                    Return a - b
                End If
                i += 1
            Loop
            Return countValue - right.countValue
        End Function

        ''' <summary>Java-style alias of <see cref="CompareTo"/>.</summary>
        Public Function compareTo(ByVal other As Buffer) As Integer
            Return CompareTo(DirectCast(other, Object))
        End Function

        ' inherit javadoc
        Public Overrides Function Equals(ByVal other As Object) As Boolean
            If TypeOf other Is Buffer AndAlso Me IsNot other Then
                Return CompareTo(other) = 0
            End If
            Return (Me Is other)
        End Function

        ' inheric javadoc
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder(2 * countValue)
            For idx As Integer = 0 To countValue - 1
                sb.Append(forDigit((bytes(idx) And &HF0) >> 4))
                sb.Append(forDigit(bytes(idx) And &HF))
            Next idx
            Return sb.ToString()
        End Function

        ''' <summary>Java <c>Character.forDigit(digit, 16)</c>: lower-case hex digit.</summary>
        Private Shared Function forDigit(ByVal digit As Integer) As Char
            If digit < 10 Then Return ChrW(AscW("0"c) + digit)
            Return ChrW(AscW("a"c) + digit - 10)
        End Function

        ''' <summary>
        ''' Convert the byte buffer to a string an specific character encoding
        ''' </summary>
        ''' <param name="charsetName"> Valid Java Character Set Name </param>
        Public Overloads Function toString(ByVal charsetName As String) As String
            Return Encoding.GetEncoding(charsetName).GetString(Me.get(), 0, Me.Count)
        End Function

        ' inherit javadoc
        Public Function Clone() As Object Implements ICloneable.Clone
            Dim result As Buffer = DirectCast(MyBase.MemberwiseClone(), Buffer)
            result.bytes = Nothing
            result.copy(Me.get(), 0, Me.Count)
            Return result
        End Function
    End Class

End Namespace
