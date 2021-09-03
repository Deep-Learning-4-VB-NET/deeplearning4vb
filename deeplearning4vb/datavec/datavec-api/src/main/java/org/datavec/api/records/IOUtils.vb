Imports System
Imports System.Text
Imports Microsoft.VisualBasic
Imports WritableComparator = org.datavec.api.io.WritableComparator
Imports WritableUtils = org.datavec.api.io.WritableUtils

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

Namespace org.datavec.api.records


	Public Class IOUtils

		''' <summary>
		''' Cannot create a new instance of IOUtils </summary>
		Private Sub New()
		End Sub

		Public Shared ReadOnly hexchars() As Char = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c}

		''' 
		''' <param name="s">
		''' @return </param>
		Friend Shared Function toXMLString(ByVal s As String) As String
			Dim sb As New StringBuilder()
			For idx As Integer = 0 To s.Length - 1
				Dim ch As Char = s.Chars(idx)
				If ch = "<"c Then
					sb.Append("&lt;")
				ElseIf ch = "&"c Then
					sb.Append("&amp;")
				ElseIf ch = "%"c Then
					sb.Append("%0025")
				ElseIf AscW(ch) < &H20 OrElse (AscW(ch) > &HD7FF AndAlso AscW(ch) < &HE000) OrElse (AscW(ch) > &HFFFD) Then
					sb.Append("%")
					sb.Append(hexchars((AscW(ch) And &HF000) >> 12))
					sb.Append(hexchars((AscW(ch) And &HF00) >> 8))
					sb.Append(hexchars((AscW(ch) And &HF0) >> 4))
					sb.Append(hexchars((AscW(ch) And &HF)))
				Else
					sb.Append(ch)
				End If
			Next idx
			Return sb.ToString()
		End Function

		Private Shared Function h2c(ByVal ch As Char) As Integer
			If ch >= "0"c AndAlso ch <= "9"c Then
				Return AscW(ch) - AscW("0"c)
			ElseIf ch >= "A"c AndAlso ch <= "F"c Then
				Return AscW(ch) - AscW("A"c) + 10
			ElseIf ch >= "a"c AndAlso ch <= "f"c Then
				Return AscW(ch) - AscW("a"c) + 10
			End If
			Return 0
		End Function

		''' 
		''' <param name="s">
		''' @return </param>
		Friend Shared Function fromXMLString(ByVal s As String) As String
			Dim sb As New StringBuilder()
			Dim idx As Integer = 0
			Do While idx < s.Length
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: char ch = s.charAt(idx++);
				Dim ch As Char = s.Chars(idx)
					idx += 1
				If ch = "%"c Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int ch1 = h2c(s.charAt(idx++)) << 12;
					Dim ch1 As Integer = h2c(s.Chars(idx)) << 12
						idx += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int ch2 = h2c(s.charAt(idx++)) << 8;
					Dim ch2 As Integer = h2c(s.Chars(idx)) << 8
						idx += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int ch3 = h2c(s.charAt(idx++)) << 4;
					Dim ch3 As Integer = h2c(s.Chars(idx)) << 4
						idx += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int ch4 = h2c(s.charAt(idx++));
					Dim ch4 As Integer = h2c(s.Chars(idx))
						idx += 1
					Dim res As Char = ChrW(ch1 Or ch2 Or ch3 Or ch4)
					sb.Append(res)
				Else
					sb.Append(ch)
				End If
			Loop
			Return sb.ToString()
		End Function

		''' 
		''' <param name="s">
		''' @return </param>
		Friend Shared Function toCSVString(ByVal s As String) As String
			Dim sb As New StringBuilder(s.Length + 1)
			sb.Append("'"c)
			Dim len As Integer = s.Length
			For i As Integer = 0 To len - 1
				Dim c As Char = s.Chars(i)
				Select Case c
					Case ControlChars.NullChar
						sb.Append("%00")
					Case ControlChars.Lf
						sb.Append("%0A")
					Case ControlChars.Cr
						sb.Append("%0D")
					Case ","c
						sb.Append("%2C")
					Case "}"c
						sb.Append("%7D")
					Case "%"c
						sb.Append("%25")
					Case Else
						sb.Append(c)
				End Select
			Next i
			Return sb.ToString()
		End Function

		''' 
		''' <param name="s"> </param>
		''' <exception cref="java.io.IOException">
		''' @return </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static String fromCSVString(String s) throws java.io.IOException
		Friend Shared Function fromCSVString(ByVal s As String) As String
			If s.Chars(0) <> "'"c Then
				Throw New IOException("Error deserializing string.")
			End If
			Dim len As Integer = s.Length
			Dim sb As New StringBuilder(len - 1)
			For i As Integer = 1 To len - 1
				Dim c As Char = s.Chars(i)
				If c = "%"c Then
					Dim ch1 As Char = s.Chars(i + 1)
					Dim ch2 As Char = s.Chars(i + 2)
					i += 2
					If ch1 = "0"c AndAlso ch2 = "0"c Then
						sb.Append(ControlChars.NullChar)
					ElseIf ch1 = "0"c AndAlso ch2 = "A"c Then
						sb.Append(ControlChars.Lf)
					ElseIf ch1 = "0"c AndAlso ch2 = "D"c Then
						sb.Append(ControlChars.Cr)
					ElseIf ch1 = "2"c AndAlso ch2 = "C"c Then
						sb.Append(","c)
					ElseIf ch1 = "7"c AndAlso ch2 = "D"c Then
						sb.Append("}"c)
					ElseIf ch1 = "2"c AndAlso ch2 = "5"c Then
						sb.Append("%"c)
					Else
						Throw New IOException("Error deserializing string.")
					End If
				Else
					sb.Append(c)
				End If
			Next i
			Return sb.ToString()
		End Function

		''' 
		''' <param name="s">
		''' @return </param>
		Friend Shared Function toXMLBuffer(ByVal s As Buffer) As String
			Return s.ToString()
		End Function

		''' 
		''' <param name="s"> </param>
		''' <exception cref="java.io.IOException">
		''' @return </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static Buffer fromXMLBuffer(String s) throws java.io.IOException
		Friend Shared Function fromXMLBuffer(ByVal s As String) As Buffer
			If s.Length = 0 Then
				Return New Buffer()
			End If
			Dim blen As Integer = s.Length \ 2
			Dim barr(blen - 1) As SByte
			For idx As Integer = 0 To blen - 1
				Dim c1 As Char = s.Chars(2 * idx)
				Dim c2 As Char = s.Chars(2 * idx + 1)
				barr(idx) = CSByte(Convert.ToInt32("" & AscW(c1) + c2, 16))
			Next idx
			Return New Buffer(barr)
		End Function

		''' 
		''' <param name="buf">
		''' @return </param>
		Friend Shared Function toCSVBuffer(ByVal buf As Buffer) As String
			Dim sb As New StringBuilder("#")
			sb.Append(buf.ToString())
			Return sb.ToString()
		End Function

		''' <summary>
		''' Converts a CSV-serialized representation of buffer to a new
		''' Buffer </summary>
		''' <param name="s"> CSV-serialized representation of buffer </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> Deserialized Buffer </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static Buffer fromCSVBuffer(String s) throws java.io.IOException
		Friend Shared Function fromCSVBuffer(ByVal s As String) As Buffer
			If s.Chars(0) <> "#"c Then
				Throw New IOException("Error deserializing buffer.")
			End If
			If s.Length = 1 Then
				Return New Buffer()
			End If
			Dim blen As Integer = (s.Length - 1) \ 2
			Dim barr(blen - 1) As SByte
			For idx As Integer = 0 To blen - 1
				Dim c1 As Char = s.Chars(2 * idx + 1)
				Dim c2 As Char = s.Chars(2 * idx + 2)
				barr(idx) = CSByte(Convert.ToInt32("" & AscW(c1) + c2, 16))
			Next idx
			Return New Buffer(barr)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static int utf8LenForCodePoint(final int cpt) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Private Shared Function utf8LenForCodePoint(ByVal cpt As Integer) As Integer
			If cpt >= 0 AndAlso cpt <= &H7F Then
				Return 1
			End If
			If cpt >= &H80 AndAlso cpt <= &H7FF Then
				Return 2
			End If
			If (cpt >= &H800 AndAlso cpt < &HD800) OrElse (cpt > &HDFFF AndAlso cpt <= &HFFFD) Then
				Return 3
			End If
			If cpt >= &H10000 AndAlso cpt <= &H10FFFF Then
				Return 4
			End If
			Throw New IOException("Illegal Unicode Codepoint " & cpt.ToString("x") & " in string.")
		End Function

		Private Shared ReadOnly B10 As Integer = Convert.ToInt32("10000000", 2)
		Private Shared ReadOnly B110 As Integer = Convert.ToInt32("11000000", 2)
		Private Shared ReadOnly B1110 As Integer = Convert.ToInt32("11100000", 2)
		Private Shared ReadOnly B11110 As Integer = Convert.ToInt32("11110000", 2)
		Private Shared ReadOnly B11 As Integer = Convert.ToInt32("11000000", 2)
		Private Shared ReadOnly B111 As Integer = Convert.ToInt32("11100000", 2)
		Private Shared ReadOnly B1111 As Integer = Convert.ToInt32("11110000", 2)
		Private Shared ReadOnly B11111 As Integer = Convert.ToInt32("11111000", 2)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static int writeUtf8(int cpt, final byte[] bytes, final int offset) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Private Shared Function writeUtf8(ByVal cpt As Integer, ByVal bytes() As SByte, ByVal offset As Integer) As Integer
			If cpt >= 0 AndAlso cpt <= &H7F Then
				bytes(offset) = CSByte(cpt)
				Return 1
			End If
			If cpt >= &H80 AndAlso cpt <= &H7FF Then
				bytes(offset + 1) = CSByte(B10 Or (cpt And &H3F))
				cpt = cpt >> 6
				bytes(offset) = CSByte(B110 Or (cpt And &H1F))
				Return 2
			End If
			If (cpt >= &H800 AndAlso cpt < &HD800) OrElse (cpt > &HDFFF AndAlso cpt <= &HFFFD) Then
				bytes(offset + 2) = CSByte(B10 Or (cpt And &H3F))
				cpt = cpt >> 6
				bytes(offset + 1) = CSByte(B10 Or (cpt And &H3F))
				cpt = cpt >> 6
				bytes(offset) = CSByte(B1110 Or (cpt And &HF))
				Return 3
			End If
			If cpt >= &H10000 AndAlso cpt <= &H10FFFF Then
				bytes(offset + 3) = CSByte(B10 Or (cpt And &H3F))
				cpt = cpt >> 6
				bytes(offset + 2) = CSByte(B10 Or (cpt And &H3F))
				cpt = cpt >> 6
				bytes(offset + 1) = CSByte(B10 Or (cpt And &H3F))
				cpt = cpt >> 6
				bytes(offset) = CSByte(B11110 Or (cpt And &H7))
				Return 4
			End If
			Throw New IOException("Illegal Unicode Codepoint " & cpt.ToString("x") & " in string.")
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static void toBinaryString(final java.io.DataOutput out, final String str) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Friend Shared Sub toBinaryString(ByVal [out] As DataOutput, ByVal str As String)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int strlen = str.length();
			Dim strlen As Integer = str.Length
			Dim bytes((strlen * 4) - 1) As SByte ' Codepoints expand to 4 bytes max
			Dim utf8Len As Integer = 0
			Dim idx As Integer = 0
			Do While idx < strlen
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int cpt = str.codePointAt(idx);
				Dim cpt As Integer = Char.ConvertToUtf32(str, idx)
				idx += If(Character.isSupplementaryCodePoint(cpt), 2, 1)
				utf8Len += writeUtf8(cpt, bytes, utf8Len)
			Loop
			writeVInt([out], utf8Len)
			[out].write(bytes, 0, utf8Len)
		End Sub

		Friend Shared Function isValidCodePoint(ByVal cpt As Integer) As Boolean
			Return Not ((cpt > &H10FFFF) OrElse (cpt >= &HD800 AndAlso cpt <= &HDFFF) OrElse (cpt >= &HFFFE AndAlso cpt <= &HFFFF))
		End Function

		Private Shared Function utf8ToCodePoint(ByVal b1 As Integer, ByVal b2 As Integer, ByVal b3 As Integer, ByVal b4 As Integer) As Integer
			Dim cpt As Integer
			cpt = (((b1 And Not B11111) << 18) Or ((b2 And Not B11) << 12) Or ((b3 And Not B11) << 6) Or (b4 And Not B11))
			Return cpt
		End Function

		Private Shared Function utf8ToCodePoint(ByVal b1 As Integer, ByVal b2 As Integer, ByVal b3 As Integer) As Integer
			Dim cpt As Integer = 0
			cpt = (((b1 And Not B1111) << 12) Or ((b2 And Not B11) << 6) Or (b3 And Not B11))
			Return cpt
		End Function

		Private Shared Function utf8ToCodePoint(ByVal b1 As Integer, ByVal b2 As Integer) As Integer
			Dim cpt As Integer = 0
			cpt = (((b1 And Not B111) << 6) Or (b2 And Not B11))
			Return cpt
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void checkB10(int b) throws java.io.IOException
		Private Shared Sub checkB10(ByVal b As Integer)
			If (b And B11) <> B10 Then
				Throw New IOException("Invalid UTF-8 representation.")
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: static String fromBinaryString(final java.io.DataInput din) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Friend Shared Function fromBinaryString(ByVal din As DataInput) As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int utf8Len = readVInt(din);
			Dim utf8Len As Integer = readVInt(din)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] bytes = new byte[utf8Len];
			Dim bytes(utf8Len - 1) As SByte
			din.readFully(bytes)
			Dim len As Integer = 0
			' For the most commmon case, i.e. ascii, numChars = utf8Len
			Dim sb As New StringBuilder(utf8Len)
			Do While len < utf8Len
				Dim cpt As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int b1 = bytes[len++] & &HFF;
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
				Dim b1 As Integer = bytes(len) And &HFF
					len += 1
				If b1 <= &H7F Then
					cpt = b1
				ElseIf (b1 And B11111) = B11110 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int b2 = bytes[len++] & &HFF;
					Dim b2 As Integer = bytes(len) And &HFF
						len += 1
					checkB10(b2)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int b3 = bytes[len++] & &HFF;
					Dim b3 As Integer = bytes(len) And &HFF
						len += 1
					checkB10(b3)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int b4 = bytes[len++] & &HFF;
					Dim b4 As Integer = bytes(len) And &HFF
						len += 1
					checkB10(b4)
					cpt = utf8ToCodePoint(b1, b2, b3, b4)
				ElseIf (b1 And B1111) = B1110 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int b2 = bytes[len++] & &HFF;
					Dim b2 As Integer = bytes(len) And &HFF
						len += 1
					checkB10(b2)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int b3 = bytes[len++] & &HFF;
					Dim b3 As Integer = bytes(len) And &HFF
						len += 1
					checkB10(b3)
					cpt = utf8ToCodePoint(b1, b2, b3)
				ElseIf (b1 And B111) = B110 Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: int b2 = bytes[len++] & &HFF;
					Dim b2 As Integer = bytes(len) And &HFF
						len += 1
					checkB10(b2)
					cpt = utf8ToCodePoint(b1, b2)
				Else
					Throw New IOException("Invalid UTF-8 byte " & b1.ToString("x") & " at offset " & (len - 1) & " in length of " & utf8Len)
				End If
				If Not isValidCodePoint(cpt) Then
					Throw New IOException("Illegal Unicode Codepoint " & cpt.ToString("x") & " in stream.")
				End If
				sb.appendCodePoint(cpt)
			Loop
			Return sb.ToString()
		End Function

		''' <summary>
		''' Parse a float from a byte array. </summary>
		Public Shared Function readFloat(ByVal bytes() As SByte, ByVal start As Integer) As Single
			Return WritableComparator.readFloat(bytes, start)
		End Function

		''' <summary>
		''' Parse a double from a byte array. </summary>
		Public Shared Function readDouble(ByVal bytes() As SByte, ByVal start As Integer) As Double
			Return WritableComparator.readDouble(bytes, start)
		End Function

		''' <summary>
		''' Reads a zero-compressed encoded long from a byte array and returns it. </summary>
		''' <param name="bytes"> byte array with decode long </param>
		''' <param name="start"> starting index </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized long </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static long readVLong(byte[] bytes, int start) throws java.io.IOException
		Public Shared Function readVLong(ByVal bytes() As SByte, ByVal start As Integer) As Long
			Return WritableComparator.readVLong(bytes, start)
		End Function

		''' <summary>
		''' Reads a zero-compressed encoded integer from a byte array and returns it. </summary>
		''' <param name="bytes"> byte array with the encoded integer </param>
		''' <param name="start"> start index </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized integer </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int readVInt(byte[] bytes, int start) throws java.io.IOException
		Public Shared Function readVInt(ByVal bytes() As SByte, ByVal start As Integer) As Integer
			Return WritableComparator.readVInt(bytes, start)
		End Function

		''' <summary>
		''' Reads a zero-compressed encoded long from a stream and return it. </summary>
		''' <param name="in"> input stream </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized long </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static long readVLong(java.io.DataInput in) throws java.io.IOException
		Public Shared Function readVLong(ByVal [in] As DataInput) As Long
			Return WritableUtils.readVLong([in])
		End Function

		''' <summary>
		''' Reads a zero-compressed encoded integer from a stream and returns it. </summary>
		''' <param name="in"> input stream </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized integer </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int readVInt(java.io.DataInput in) throws java.io.IOException
		Public Shared Function readVInt(ByVal [in] As DataInput) As Integer
			Return WritableUtils.readVInt([in])
		End Function

		''' <summary>
		''' Get the encoded length if an integer is stored in a variable-length format </summary>
		''' <returns> the encoded length </returns>
		Public Shared Function getVIntSize(ByVal i As Long) As Integer
			Return WritableUtils.getVIntSize(i)
		End Function

		''' <summary>
		''' Serializes a long to a binary stream with zero-compressed encoding.
		''' For -112 <= i <= 127, only one byte is used with the actual value.
		''' For other values of i, the first byte value indicates whether the
		''' long is positive or negative, and the number of bytes that follow.
		''' If the first byte value v is between -113 and -120, the following long
		''' is positive, with number of bytes that follow are -(v+112).
		''' If the first byte value v is between -121 and -128, the following long
		''' is negative, with number of bytes that follow are -(v+120). Bytes are
		''' stored in the high-non-zero-byte-first order.
		''' </summary>
		''' <param name="stream"> Binary output stream </param>
		''' <param name="i"> Long to be serialized </param>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeVLong(java.io.DataOutput stream, long i) throws java.io.IOException
		Public Shared Sub writeVLong(ByVal stream As DataOutput, ByVal i As Long)
			WritableUtils.writeVLong(stream, i)
		End Sub

		''' <summary>
		''' Serializes an int to a binary stream with zero-compressed encoding.
		''' </summary>
		''' <param name="stream"> Binary output stream </param>
		''' <param name="i"> int to be serialized </param>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeVInt(java.io.DataOutput stream, int i) throws java.io.IOException
		Public Shared Sub writeVInt(ByVal stream As DataOutput, ByVal i As Integer)
			WritableUtils.writeVInt(stream, i)
		End Sub

		''' <summary>
		''' Lexicographic order of binary data. </summary>
		Public Shared Function compareBytes(ByVal b1() As SByte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As SByte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
			Return WritableComparator.compareBytes(b1, s1, l1, b2, s2, l2)
		End Function
	End Class

End Namespace