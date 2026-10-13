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
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.io
Imports BinaryComparable = DeepLearning4VB.org.datavec.api.io.BinaryComparable
Imports WritableComparator = DeepLearning4VB.org.datavec.api.io.WritableComparator
Imports WritableUtils = DeepLearning4VB.org.datavec.api.io.WritableUtils

Namespace org.datavec.api.writable

    ''' <summary>
    ''' A writable UTF-8 string, stored as a byte array that is reused when the value changes.
    ''' Byte arrays are <c>Byte()</c>; where the Java API returned a <c>ByteBuffer</c>
    ''' (<see cref="encode(String)"/>) a trimmed array is returned instead.
    ''' </summary>
    <Serializable>
    Public Class Text
        Inherits BinaryComparable
        Implements WritableComparable(Of BinaryComparable)

        ''' <summary>Thrown by <see cref="validateUTF8(Byte())"/> (Java <c>java.nio.charset.MalformedInputException</c>).</summary>
        Public Class MalformedInputException
            Inherits IOException

            Private ReadOnly inputLengthField As Integer

            Public Sub New(ByVal inputLength As Integer)
                MyBase.New("Input length = " & inputLength)
                inputLengthField = inputLength
            End Sub

            ''' <summary>The index of the offending byte.</summary>
            Public ReadOnly Property InputLength As Integer
                Get
                    Return inputLengthField
                End Get
            End Property

            Public Function getInputLength() As Integer
                Return inputLengthField
            End Function
        End Class

        ' Encoders matching Java's: REPLACE substitutes '?' on encoding and U+FFFD on decoding; REPORT throws.
        Private Shared ReadOnly ReplacingEncoding As Encoding = CreateReplacingEncoding()
        Private Shared ReadOnly ReportingEncoding As Encoding = New UTF8Encoding(False, True)

        Private Shared Function CreateReplacingEncoding() As Encoding
            Dim enc As Encoding = DirectCast(New UTF8Encoding(False, False).Clone(), Encoding)
            enc.EncoderFallback = New EncoderReplacementFallback("?")
            enc.DecoderFallback = New DecoderReplacementFallback(ChrW(&HFFFD))
            Return enc
        End Function

        Private Shared ReadOnly EMPTY_BYTES() As Byte = New Byte(-1) {}

        Private bytesField() As Byte
        Private lengthField As Integer

        Public Sub New()
            bytesField = EMPTY_BYTES
        End Sub

        ''' <summary>
        ''' Construct from a string.
        ''' </summary>
        Public Sub New(ByVal [string] As String)
            Me.New()
            [set]([string])
        End Sub

        ''' <summary>
        ''' Construct from another text. </summary>
        Public Sub New(ByVal utf8 As Text)
            Me.New()
            [set](utf8)
        End Sub

        ''' <summary>
        ''' Construct from a byte array.
        ''' </summary>
        Public Sub New(ByVal utf8() As Byte)
            Me.New()
            [set](utf8)
        End Sub

        ''' <summary>
        ''' Returns the raw bytes; however, only data up to <see cref="Length"/> is
        ''' valid.
        ''' </summary>
        <JsonIgnore>
        Public Overrides ReadOnly Property Bytes As Byte()
            Get
                Return bytesField
            End Get
        End Property

        ''' <summary>
        ''' Returns the number of bytes in the byte array </summary>
        <JsonIgnore>
        Public Overrides ReadOnly Property Length As Integer
            Get
                Return lengthField
            End Get
        End Property

        ''' <summary>JSON view of the valid bytes (Java serialized the <c>bytes</c> field).</summary>
        <JsonPropertyName("bytes")>
        Public Property JsonBytes As Byte()
            Get
                Return Arrays.CopyOf(bytesField, lengthField)
            End Get
            Set(ByVal value As Byte())
                If value Is Nothing Then
                    clear()
                Else
                    [set](value)
                End If
            End Set
        End Property

        ''' <summary>JSON view of the valid length (Java serialized the <c>length</c> field).</summary>
        <JsonPropertyName("length")>
        Public Property JsonLength As Integer
            Get
                Return lengthField
            End Get
            Set(ByVal value As Integer)
                lengthField = Math.Max(0, Math.Min(value, bytesField.Length))
            End Set
        End Property

        ''' <summary>
        ''' Returns the Unicode Scalar Value (32-bit integer value)
        ''' for the character at <code>position</code>. Note that this
        ''' method avoids using the converter or doing String instatiation </summary>
        ''' <returns> the Unicode scalar value at position or -1
        '''          if the position is invalid or points to a
        '''          trailing byte </returns>
        Public Overridable Function charAt(ByVal position As Integer) As Integer
            If position >= Me.lengthField Then
                Return -1 ' too long
            End If
            If position < 0 Then
                Return -1 ' duh.
            End If
            Dim pos As Integer = position
            Return bytesToCodePoint(bytesField, pos, lengthField)
        End Function

        Public Overridable Function find(ByVal what As String) As Integer
            Return find(what, 0)
        End Function

        ''' <summary>
        ''' Finds any occurence of <code>what</code> in the backing
        ''' buffer, starting as position <code>start</code>. The starting
        ''' position is measured in bytes and the return value is in
        ''' terms of byte position in the buffer. The backing buffer is
        ''' not converted to a string for this operation. </summary>
        ''' <returns> byte position of the first occurence of the search
        '''         string in the UTF-8 buffer or -1 if not found </returns>
        Public Overridable Function find(ByVal what As String, ByVal start As Integer) As Integer
            Dim tgt() As Byte = encode(what)
            If tgt.Length = 0 Then Return -1
            Dim b As Byte = tgt(0)
            Dim pos As Integer = start
            Do While pos < Me.lengthField
                If b = bytesField(pos) Then ' matching first byte
                    Dim found As Boolean = True
                    For k As Integer = 1 To tgt.Length - 1
                        If pos + k >= Me.lengthField OrElse tgt(k) <> bytesField(pos + k) Then
                            found = False
                            Exit For
                        End If
                    Next k
                    If found Then
                        Return pos
                    End If
                End If
                pos += 1
            Loop
            Return -1 ' not found
        End Function

        ''' <summary>
        ''' Set to contain the contents of a string.
        ''' </summary>
        Public Overridable Sub [set](ByVal [string] As String)
            Dim bb() As Byte = encode([string], True)
            bytesField = bb
            lengthField = bb.Length
        End Sub

        ''' <summary>
        ''' Set to a utf8 byte array
        ''' </summary>
        Public Overridable Sub [set](ByVal utf8() As Byte)
            [set](utf8, 0, utf8.Length)
        End Sub

        ''' <summary>
        ''' copy a text. </summary>
        Public Overridable Sub [set](ByVal other As Text)
            [set](other.Bytes, 0, other.Length)
        End Sub

        ''' <summary>
        ''' Set the Text to range of bytes </summary>
        ''' <param name="utf8"> the data to copy from </param>
        ''' <param name="start"> the first position of the new string </param>
        ''' <param name="len"> the number of bytes of the new string </param>
        Public Overridable Sub [set](ByVal utf8() As Byte, ByVal start As Integer, ByVal len As Integer)
            setCapacity(len, False)
            Array.Copy(utf8, start, bytesField, 0, len)
            Me.lengthField = len
        End Sub

        ''' <summary>
        ''' Append a range of bytes to the end of the given text </summary>
        ''' <param name="utf8"> the data to copy from </param>
        ''' <param name="start"> the first position to append from utf8 </param>
        ''' <param name="len"> the number of bytes to append </param>
        Public Overridable Sub append(ByVal utf8() As Byte, ByVal start As Integer, ByVal len As Integer)
            setCapacity(lengthField + len, True)
            Array.Copy(utf8, start, bytesField, lengthField, len)
            lengthField += len
        End Sub

        ''' <summary>
        ''' Clear the string to empty.
        ''' </summary>
        Public Overridable Sub clear()
            lengthField = 0
        End Sub

        ''' <summary>
        ''' Sets the capacity of this Text object to <em>at least</em>
        ''' <code>len</code> bytes. If the current buffer is longer,
        ''' then the capacity and existing content of the buffer are
        ''' unchanged. If <code>len</code> is larger
        ''' than the current capacity, the Text object's capacity is
        ''' increased to match.
        ''' </summary>
        ''' <param name="len"> the number of bytes we need </param>
        ''' <param name="keepData"> should the old data be kept </param>
        Private Sub setCapacity(ByVal len As Integer, ByVal keepData As Boolean)
            If bytesField Is Nothing OrElse bytesField.Length < len Then
                Dim newBytes(len - 1) As Byte
                If bytesField IsNot Nothing AndAlso keepData Then
                    Array.Copy(bytesField, 0, newBytes, 0, lengthField)
                End If
                bytesField = newBytes
            End If
        End Sub

        ''' <summary>
        ''' Convert text back to string </summary>
        Public Overrides Function ToString() As String
            Return decode(bytesField, 0, lengthField)
        End Function

        ''' <summary>
        ''' deserialize
        ''' </summary>
        Public Overridable Sub readFields(ByVal [in] As DataInput) Implements Writable.readFields
            Dim newLength As Integer = WritableUtils.readVInt([in])
            setCapacity(newLength, False)
            [in].readFully(bytesField, 0, newLength)
            lengthField = newLength
        End Sub

        Public Overridable Sub writeType(ByVal [out] As DataOutput) Implements Writable.writeType
            [out].writeShort(WritableType.Text.typeIdx())
        End Sub

        ''' <summary>
        ''' Skips over one Text in the input. </summary>
        Public Shared Sub skip(ByVal [in] As DataInput)
            Dim length As Integer = WritableUtils.readVInt([in])
            WritableUtils.skipFully([in], length)
        End Sub

        ''' <summary>
        ''' serialize
        ''' write this object to out
        ''' length uses zero-compressed encoding </summary>
        Public Overridable Sub write(ByVal [out] As DataOutput) Implements Writable.write
            WritableUtils.writeVInt([out], lengthField)
            [out].write(bytesField, 0, lengthField)
        End Sub

        ''' <summary>
        ''' Returns true iff <code>o</code> is a Text with the same contents. </summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Return TypeOf o Is Text AndAlso MyBase.Equals(o)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return MyBase.GetHashCode()
        End Function

        ''' <summary>
        ''' A WritableComparator optimized for Text keys. </summary>
        Public Class Comparator
            Inherits WritableComparator

            Public Sub New()
                MyBase.New(GetType(Text))
            End Sub

            Public Overrides Function compare(ByVal b1() As Byte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As Byte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
                Dim n1 As Integer = WritableUtils.decodeVIntSize(b1(s1))
                Dim n2 As Integer = WritableUtils.decodeVIntSize(b2(s2))
                Return compareBytes(b1, s1 + n1, l1 - n1, b2, s2 + n2, l2 - n2)
            End Function
        End Class

        Shared Sub New()
            ' register this comparator
            WritableComparator.define(GetType(Text), New Comparator())
        End Sub

        '/ STATIC UTILITIES FROM HERE DOWN

        ''' <summary>
        ''' Converts the provided byte array to a String using the
        ''' UTF-8 encoding. If the input is malformed,
        ''' replace by a default value.
        ''' </summary>
        Public Shared Function decode(ByVal utf8() As Byte) As String
            Return decode(utf8, 0, utf8.Length, True)
        End Function

        Public Shared Function decode(ByVal utf8() As Byte, ByVal start As Integer, ByVal length As Integer) As String
            Return decode(utf8, start, length, True)
        End Function

        ''' <summary>
        ''' Converts the provided byte array to a String using the
        ''' UTF-8 encoding. If <code>replace</code> is true, then
        ''' malformed input is replaced with the
        ''' substitution character, which is U+FFFD. Otherwise the
        ''' method throws a <see cref="DecoderFallbackException"/>.
        ''' </summary>
        Public Shared Function decode(ByVal utf8() As Byte, ByVal start As Integer, ByVal length As Integer, ByVal replace As Boolean) As String
            Return If(replace, ReplacingEncoding, ReportingEncoding).GetString(utf8, start, length)
        End Function

        ''' <summary>
        ''' Converts the provided String to bytes using the
        ''' UTF-8 encoding. If the input is malformed,
        ''' invalid chars are replaced by a default value. </summary>
        ''' <returns> the UTF-8 bytes (Java returned a ByteBuffer whose limit was the length) </returns>
        Public Shared Function encode(ByVal [string] As String) As Byte()
            Return encode([string], True)
        End Function

        ''' <summary>
        ''' Converts the provided String to bytes using the
        ''' UTF-8 encoding. If <code>replace</code> is true, then
        ''' malformed input (an unpaired surrogate) is replaced with '?'. Otherwise the
        ''' method throws an <see cref="EncoderFallbackException"/>. </summary>
        ''' <returns> the UTF-8 bytes (Java returned a ByteBuffer whose limit was the length) </returns>
        Public Shared Function encode(ByVal [string] As String, ByVal replace As Boolean) As Byte()
            Return If(replace, ReplacingEncoding, ReportingEncoding).GetBytes([string])
        End Function

        ''' <summary>
        ''' Read a UTF8 encoded string from in
        ''' </summary>
        Public Shared Function readString(ByVal [in] As DataInput) As String
            Dim length As Integer = WritableUtils.readVInt([in])
            Dim bytes(length - 1) As Byte
            [in].readFully(bytes, 0, length)
            Return decode(bytes)
        End Function

        ''' <summary>
        ''' Write a UTF8 encoded string to out
        ''' </summary>
        Public Shared Function writeString(ByVal [out] As DataOutput, ByVal s As String) As Integer
            Dim bytes() As Byte = encode(s)
            Dim length As Integer = bytes.Length
            WritableUtils.writeVInt([out], length)
            [out].write(bytes, 0, length)
            Return length
        End Function

        '//// states for validateUTF8

        Private Const LEAD_BYTE As Integer = 0

        Private Const TRAIL_BYTE_1 As Integer = 1

        Private Const TRAIL_BYTE As Integer = 2

        ''' <summary>
        ''' Check if a byte array contains valid utf-8 </summary>
        ''' <param name="utf8"> byte array </param>
        ''' <exception cref="MalformedInputException"> if the byte array contains invalid utf-8 </exception>
        Public Shared Sub validateUTF8(ByVal utf8() As Byte)
            validateUTF8(utf8, 0, utf8.Length)
        End Sub

        ''' <summary>
        ''' Check to see if a byte array is valid utf-8 </summary>
        ''' <param name="utf8"> the array of bytes </param>
        ''' <param name="start"> the offset of the first byte in the array </param>
        ''' <param name="len"> the length of the byte sequence </param>
        ''' <exception cref="MalformedInputException"> if the byte array contains invalid bytes </exception>
        Public Shared Sub validateUTF8(ByVal utf8() As Byte, ByVal start As Integer, ByVal len As Integer)
            Dim count As Integer = start
            Dim leadByte As Integer = 0
            Dim length As Integer = 0
            Dim state As Integer = LEAD_BYTE
            Do While count < start + len
                Dim aByte As Integer = utf8(count)

                Select Case state
                    Case LEAD_BYTE
                        leadByte = aByte
                        length = bytesFromUTF8(aByte)

                        Select Case length
                            Case 0 ' check for ASCII
                                If leadByte > &H7F Then
                                    Throw New MalformedInputException(count)
                                End If
                            Case 1
                                If leadByte < &HC2 OrElse leadByte > &HDF Then
                                    Throw New MalformedInputException(count)
                                End If
                                state = TRAIL_BYTE_1
                            Case 2
                                If leadByte < &HE0 OrElse leadByte > &HEF Then
                                    Throw New MalformedInputException(count)
                                End If
                                state = TRAIL_BYTE_1
                            Case 3
                                If leadByte < &HF0 OrElse leadByte > &HF4 Then
                                    Throw New MalformedInputException(count)
                                End If
                                state = TRAIL_BYTE_1
                            Case Else
                                ' too long! Longest valid UTF-8 is 4 bytes (lead + three)
                                ' or if < 0 we got a trail byte in the lead byte position
                                Throw New MalformedInputException(count)
                        End Select ' switch (length)

                    Case TRAIL_BYTE_1, TRAIL_BYTE
                        If state = TRAIL_BYTE_1 Then
                            If leadByte = &HF0 AndAlso aByte < &H90 Then
                                Throw New MalformedInputException(count)
                            End If
                            If leadByte = &HF4 AndAlso aByte > &H8F Then
                                Throw New MalformedInputException(count)
                            End If
                            If leadByte = &HE0 AndAlso aByte < &HA0 Then
                                Throw New MalformedInputException(count)
                            End If
                            If leadByte = &HED AndAlso aByte > &H9F Then
                                Throw New MalformedInputException(count)
                            End If
                        End If
                        ' the Java code falls through to the regular trail-byte test
                        If aByte < &H80 OrElse aByte > &HBF Then
                            Throw New MalformedInputException(count)
                        End If
                        length -= 1
                        If length = 0 Then
                            state = LEAD_BYTE
                        Else
                            state = TRAIL_BYTE
                        End If
                End Select ' switch (state)
                count += 1
            Loop
        End Sub

        ''' <summary>
        ''' Magic numbers for UTF-8. These are the number of bytes
        ''' that <em>follow</em> a given lead byte. Trailing bytes
        ''' have the value -1. The values 4 and 5 are presented in
        ''' this table, even though valid UTF-8 cannot include the
        ''' five and six byte sequences.
        ''' </summary>
        Friend Shared ReadOnly bytesFromUTF8() As Integer = BuildBytesFromUTF8()

        Private Shared Function BuildBytesFromUTF8() As Integer()
            Dim table(255) As Integer
            For i As Integer = 0 To 127
                table(i) = 0
            Next i
            For i As Integer = 128 To 191
                table(i) = -1
            Next i
            For i As Integer = 192 To 223
                table(i) = 1
            Next i
            For i As Integer = 224 To 239
                table(i) = 2
            Next i
            For i As Integer = 240 To 247
                table(i) = 3
            Next i
            For i As Integer = 248 To 251
                table(i) = 4
            Next i
            For i As Integer = 252 To 255
                table(i) = 5
            Next i
            Return table
        End Function

        ''' <summary>
        ''' Returns the next code point at the given position in the buffer and advances
        ''' <paramref name="position"/> past it (the Java version took a <c>ByteBuffer</c>).
        ''' </summary>
        Public Shared Function bytesToCodePoint(ByVal bytes() As Byte, ByRef position As Integer) As Integer
            Return bytesToCodePoint(bytes, position, bytes.Length)
        End Function

        Private Shared Function bytesToCodePoint(ByVal bytes() As Byte, ByRef position As Integer, ByVal limit As Integer) As Integer
            If position >= limit Then Throw New EndOfStreamException("Buffer underflow")
            Dim b As Integer = bytes(position)
            Dim extraBytesToRead As Integer = bytesFromUTF8(b)
            If extraBytesToRead < 0 Then
                Return -1 ' trailing byte!
            End If
            Dim ch As Integer = 0

            ' the Java switch falls through from case 5 down to case 0
            For remaining As Integer = extraBytesToRead To 1 Step -1
                If position >= limit Then Throw New EndOfStreamException("Buffer underflow")
                ch += bytes(position)
                position += 1
                ch <<= 6 ' remember, illegal UTF-8 for 4 and 5
            Next remaining
            If position >= limit Then Throw New EndOfStreamException("Buffer underflow")
            ch += bytes(position)
            position += 1
            ch -= offsetsFromUTF8(extraBytesToRead)

            Return ch
        End Function

        Friend Shared ReadOnly offsetsFromUTF8() As Integer = {&H0, &H3080, &HE2080, &H3C82080, &HFA082080, &H82082080}

        ''' <summary>
        ''' For the given string, returns the number of UTF-8 bytes
        ''' required to encode the string. </summary>
        ''' <param name="string"> text to encode </param>
        ''' <returns> number of UTF-8 bytes required to encode </returns>
        Public Shared Function utf8Length(ByVal [string] As String) As Integer
            Dim size As Integer = 0
            Dim i As Integer = 0
            Do While i < [string].Length
                Dim ch As Integer = AscW([string](i))
                If (ch >= &HD800) AndAlso (ch < &HDC00) Then
                    ' surrogate pair?
                    Dim trail As Integer = If(i + 1 < [string].Length, AscW([string](i + 1)), -1)
                    If (trail > &HDBFF) AndAlso (trail < &HE000) Then
                        ' valid pair
                        size += 4
                        i += 1
                    Else
                        ' invalid pair
                        size += 3
                    End If
                ElseIf ch < &H80 Then
                    size += 1
                ElseIf ch < &H800 Then
                    size += 2
                Else
                    ' ch < 0x10000, that is, the largest char value
                    size += 3
                End If
                i += 1
            Loop
            Return size
        End Function

        ''' <summary>Parses like Java <c>Double.parseDouble</c>; "0x" prefixed text is decoded as a hex long.</summary>
        Public Overridable Function toDouble() As Double Implements Writable.toDouble
            Dim s As String = ToString()
            If s.StartsWith("0x", StringComparison.Ordinal) Then
                Return DecodeHex(s)
            End If
            Try
                Return Double.Parse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture)
            Catch e As FormatException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            End Try
        End Function

        Public Overridable Function toFloat() As Single Implements Writable.toFloat
            Dim s As String = ToString()
            If s.StartsWith("0x", StringComparison.Ordinal) Then
                Return CInt(DecodeHex(s))
            End If
            Try
                Return Single.Parse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture)
            Catch e As FormatException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            End Try
        End Function

        Public Overridable Function toInt() As Integer Implements Writable.toInt
            Dim s As String = ToString()
            If s.StartsWith("0x", StringComparison.Ordinal) Then
                Return CInt(DecodeHex(s))
            End If
            Try
                Return Integer.Parse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
            Catch e As FormatException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            Catch e As OverflowException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            End Try
        End Function

        Public Overridable Function toLong() As Long Implements Writable.toLong
            Dim s As String = ToString()
            If s.StartsWith("0x", StringComparison.Ordinal) Then
                Return DecodeHex(s)
            End If
            Try
                Return Long.Parse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
            Catch e As FormatException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            Catch e As OverflowException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            End Try
        End Function

        ''' <summary>Java <c>Long.decode</c> for a "0x" prefixed string.</summary>
        Private Shared Function DecodeHex(ByVal s As String) As Long
            Try
                Return Long.Parse(s.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture)
            Catch e As FormatException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            Catch e As OverflowException
                Throw New NumberFormatException("For input string: """ & s & """", e)
            End Try
        End Function

        Public Overridable Shadows Function [getType]() As WritableType Implements Writable.getType
            Return WritableType.Text
        End Function
    End Class

End Namespace
