Imports System
Imports BinaryComparable = org.datavec.api.io.BinaryComparable
Imports org.datavec.api.io
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

Namespace org.datavec.api.writable





	<Serializable>
	Public Class Text
		Inherits BinaryComparable
		Implements WritableComparable(Of BinaryComparable)

		Private Shared ENCODER_FACTORY As ThreadLocal(Of CharsetEncoder) = New ThreadLocalAnonymousInnerClass()

		Private Class ThreadLocalAnonymousInnerClass
			Inherits ThreadLocal(Of CharsetEncoder)

			Protected Friend Function initialValue() As CharsetEncoder
				Return StandardCharsets.UTF_8.newEncoder().onMalformedInput(CodingErrorAction.REPORT).onUnmappableCharacter(CodingErrorAction.REPORT)
			End Function
		End Class

		Private Shared DECODER_FACTORY As ThreadLocal(Of CharsetDecoder) = New ThreadLocalAnonymousInnerClass2()

		Private Class ThreadLocalAnonymousInnerClass2
			Inherits ThreadLocal(Of CharsetDecoder)

			Protected Friend Function initialValue() As CharsetDecoder
				Return StandardCharsets.UTF_8.newDecoder().onMalformedInput(CodingErrorAction.REPORT).onUnmappableCharacter(CodingErrorAction.REPORT)
			End Function
		End Class

		Private Shared ReadOnly EMPTY_BYTES(-1) As SByte

'JAVA TO VB CONVERTER NOTE: The field bytes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private bytes_Conflict() As SByte
'JAVA TO VB CONVERTER NOTE: The field length was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private length_Conflict As Integer

		Public Sub New()
			bytes_Conflict = EMPTY_BYTES
		End Sub

		''' <summary>
		''' Construct from a string.
		''' </summary>
		Public Sub New(ByVal [string] As String)
			set([string])
		End Sub

		''' <summary>
		''' Construct from another text. </summary>
		Public Sub New(ByVal utf8 As Text)
			set(utf8)
		End Sub

		''' <summary>
		''' Construct from a byte array.
		''' </summary>
		Public Sub New(ByVal utf8() As SByte)
			set(utf8)
		End Sub

		''' <summary>
		''' Returns the raw bytes; however, only data up to <seealso cref="getLength()"/> is
		''' valid.
		''' </summary>
		Public Overrides ReadOnly Property Bytes As SByte()
			Get
				Return bytes_Conflict
			End Get
		End Property

		''' <summary>
		''' Returns the number of bytes in the byte array </summary>
		Public Overrides ReadOnly Property Length As Integer
			Get
				Return length_Conflict
			End Get
		End Property

		''' <summary>
		''' Returns the Unicode Scalar Value (32-bit integer value)
		''' for the character at <code>position</code>. Note that this
		''' method avoids using the converter or doing String instatiation </summary>
		''' <returns> the Unicode scalar value at position or -1
		'''          if the position is invalid or points to a
		'''          trailing byte </returns>
		Public Overridable Function charAt(ByVal position As Integer) As Integer
			If position > Me.length_Conflict Then
				Return -1 ' too long
			End If
			If position < 0 Then
				Return -1 ' duh.
			End If

			Dim bb As ByteBuffer = CType(ByteBuffer.wrap(bytes_Conflict).position(position), ByteBuffer)
			Return bytesToCodePoint(bb.slice())
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
			Try
				Dim src As ByteBuffer = ByteBuffer.wrap(Me.bytes_Conflict, 0, Me.length_Conflict)
				Dim tgt As ByteBuffer = encode(what)
				Dim b As SByte = tgt.get()
				src.position(start)

				Do While src.hasRemaining()
					If b = src.get() Then ' matching first byte
						src.mark() ' save position in loop
						tgt.mark() ' save position in target
						Dim found As Boolean = True
						Dim pos As Integer = src.position() - 1
						Do While tgt.hasRemaining()
							If Not src.hasRemaining() Then ' src expired first
								tgt.reset()
								src.reset()
								found = False
								Exit Do
							End If
							If Not (tgt.get() = src.get()) Then
								tgt.reset()
								src.reset()
								found = False
								Exit Do ' no match
							End If
						Loop
						If found Then
							Return pos
						End If
					End If
				Loop
				Return -1 ' not found
			Catch e As CharacterCodingException
				' can't get here
				Return -1
			End Try
		End Function

		''' <summary>
		''' Set to contain the contents of a string.
		''' </summary>
		Public Overridable Sub set(ByVal [string] As String)
			Try
				Dim bb As ByteBuffer = encode([string], True)
				bytes_Conflict = bb.array()
				length_Conflict = bb.limit()
			Catch e As CharacterCodingException
				Throw New Exception("Should not have happened " & e.ToString())
			End Try
		End Sub

		''' <summary>
		''' Set to a utf8 byte array
		''' </summary>
		Public Overridable Sub set(ByVal utf8() As SByte)
			set(utf8, 0, utf8.Length)
		End Sub

		''' <summary>
		''' copy a text. </summary>
		Public Overridable Sub set(ByVal other As Text)
			set(other.Bytes, 0, other.Length)
		End Sub

		''' <summary>
		''' Set the Text to range of bytes </summary>
		''' <param name="utf8"> the data to copy from </param>
		''' <param name="start"> the first position of the new string </param>
		''' <param name="len"> the number of bytes of the new string </param>
		Public Overridable Sub set(ByVal utf8() As SByte, ByVal start As Integer, ByVal len As Integer)
			setCapacity(len, False)
			Array.Copy(utf8, start, bytes_Conflict, 0, len)
			Me.length_Conflict = len
		End Sub

		''' <summary>
		''' Append a range of bytes to the end of the given text </summary>
		''' <param name="utf8"> the data to copy from </param>
		''' <param name="start"> the first position to append from utf8 </param>
		''' <param name="len"> the number of bytes to append </param>
		Public Overridable Sub append(ByVal utf8() As SByte, ByVal start As Integer, ByVal len As Integer)
			setCapacity(length_Conflict + len, True)
			Array.Copy(utf8, start, bytes_Conflict, length_Conflict, len)
			length_Conflict += len
		End Sub

		''' <summary>
		''' Clear the string to empty.
		''' </summary>
		Public Overridable Sub clear()
			length_Conflict = 0
		End Sub

	'    
	'     * Sets the capacity of this Text object to <em>at least</em>
	'     * <code>len</code> bytes. If the current buffer is longer,
	'     * then the capacity and existing content of the buffer are
	'     * unchanged. If <code>len</code> is larger
	'     * than the current capacity, the Text object's capacity is
	'     * increased to match.
	'     * @param len the number of bytes we need
	'     * @param keepData should the old data be kept
	'     
		Private Sub setCapacity(ByVal len As Integer, ByVal keepData As Boolean)
			If bytes_Conflict Is Nothing OrElse bytes_Conflict.Length < len Then
				Dim newBytes(len - 1) As SByte
				If bytes_Conflict IsNot Nothing AndAlso keepData Then
					Array.Copy(bytes_Conflict, 0, newBytes, 0, length_Conflict)
				End If
				bytes_Conflict = newBytes
			End If
		End Sub

		''' <summary>
		''' Convert text back to string </summary>
		''' <seealso cref= java.lang.Object#toString() </seealso>
		Public Overrides Function ToString() As String
			Try
				Return decode(bytes_Conflict, 0, length_Conflict)
			Catch e As CharacterCodingException
				Throw New Exception("Should not have happened " & e.ToString())
			End Try
		End Function

		''' <summary>
		''' deserialize
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void readFields(java.io.DataInput in) throws java.io.IOException
		Public Overridable Sub readFields(ByVal [in] As DataInput)
			Dim newLength As Integer = WritableUtils.readVInt([in])
			setCapacity(newLength, False)
			[in].readFully(bytes_Conflict, 0, newLength)
			length_Conflict = newLength
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void writeType(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub writeType(ByVal [out] As DataOutput)
			[out].writeShort(WritableType.Text.typeIdx())
		End Sub

		''' <summary>
		''' Skips over one Text in the input. </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void skip(java.io.DataInput in) throws java.io.IOException
		Public Shared Sub skip(ByVal [in] As DataInput)
			Dim length As Integer = WritableUtils.readVInt([in])
			WritableUtils.skipFully([in], length)
		End Sub

		''' <summary>
		''' serialize
		''' write this object to out
		''' length uses zero-compressed encoding </summary>
		''' <seealso cref= Writable#write(DataOutput) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void write(java.io.DataOutput out) throws java.io.IOException
		Public Overridable Sub write(ByVal [out] As DataOutput)
			WritableUtils.writeVInt([out], length_Conflict)
			[out].write(bytes_Conflict, 0, length_Conflict)
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

			Public Overrides Function compare(ByVal b1() As SByte, ByVal s1 As Integer, ByVal l1 As Integer, ByVal b2() As SByte, ByVal s2 As Integer, ByVal l2 As Integer) As Integer
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
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String decode(byte[] utf8) throws CharacterCodingException
		Public Shared Function decode(ByVal utf8() As SByte) As String
			Return decode(ByteBuffer.wrap(utf8), True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String decode(byte[] utf8, int start, int length) throws CharacterCodingException
		Public Shared Function decode(ByVal utf8() As SByte, ByVal start As Integer, ByVal length As Integer) As String
			Return decode(ByteBuffer.wrap(utf8, start, length), True)
		End Function

		''' <summary>
		''' Converts the provided byte array to a String using the
		''' UTF-8 encoding. If <code>replace</code> is true, then
		''' malformed input is replaced with the
		''' substitution character, which is U+FFFD. Otherwise the
		''' method throws a MalformedInputException.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String decode(byte[] utf8, int start, int length, boolean replace) throws CharacterCodingException
		Public Shared Function decode(ByVal utf8() As SByte, ByVal start As Integer, ByVal length As Integer, ByVal replace As Boolean) As String
			Return decode(ByteBuffer.wrap(utf8, start, length), replace)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static String decode(java.nio.ByteBuffer utf8, boolean replace) throws CharacterCodingException
		Private Shared Function decode(ByVal utf8 As ByteBuffer, ByVal replace As Boolean) As String
			Dim decoder As CharsetDecoder = DECODER_FACTORY.get()
			If replace Then
				decoder.onMalformedInput(java.nio.charset.CodingErrorAction.REPLACE)
				decoder.onUnmappableCharacter(CodingErrorAction.REPLACE)
			End If
			Dim str As String = decoder.decode(utf8).ToString()
			' set decoder back to its default value: REPORT
			If replace Then
				decoder.onMalformedInput(CodingErrorAction.REPORT)
				decoder.onUnmappableCharacter(CodingErrorAction.REPORT)
			End If
			Return str
		End Function

		''' <summary>
		''' Converts the provided String to bytes using the
		''' UTF-8 encoding. If the input is malformed,
		''' invalid chars are replaced by a default value. </summary>
		''' <returns> ByteBuffer: bytes stores at ByteBuffer.array()
		'''                     and length is ByteBuffer.limit() </returns>

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.nio.ByteBuffer encode(String string) throws CharacterCodingException
		Public Shared Function encode(ByVal [string] As String) As ByteBuffer
			Return encode([string], True)
		End Function

		''' <summary>
		''' Converts the provided String to bytes using the
		''' UTF-8 encoding. If <code>replace</code> is true, then
		''' malformed input is replaced with the
		''' substitution character, which is U+FFFD. Otherwise the
		''' method throws a MalformedInputException. </summary>
		''' <returns> ByteBuffer: bytes stores at ByteBuffer.array()
		'''                     and length is ByteBuffer.limit() </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.nio.ByteBuffer encode(String string, boolean replace) throws CharacterCodingException
		Public Shared Function encode(ByVal [string] As String, ByVal replace As Boolean) As ByteBuffer
			Dim encoder As CharsetEncoder = ENCODER_FACTORY.get()
			If replace Then
				encoder.onMalformedInput(CodingErrorAction.REPLACE)
				encoder.onUnmappableCharacter(CodingErrorAction.REPLACE)
			End If
			Dim bytes As ByteBuffer = encoder.encode(CharBuffer.wrap([string].ToCharArray()))
			If replace Then
				encoder.onMalformedInput(CodingErrorAction.REPORT)
				encoder.onUnmappableCharacter(CodingErrorAction.REPORT)
			End If
			Return bytes
		End Function

		''' <summary>
		''' Read a UTF8 encoded string from in
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readString(java.io.DataInput in) throws java.io.IOException
		Public Shared Function readString(ByVal [in] As DataInput) As String
			Dim length As Integer = WritableUtils.readVInt([in])
			Dim bytes(length - 1) As SByte
			[in].readFully(bytes, 0, length)
			Return decode(bytes)
		End Function

		''' <summary>
		''' Write a UTF8 encoded string to out
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int writeString(java.io.DataOutput out, String s) throws java.io.IOException
		Public Shared Function writeString(ByVal [out] As DataOutput, ByVal s As String) As Integer
			Dim bytes As ByteBuffer = encode(s)
			Dim length As Integer = bytes.limit()
			WritableUtils.writeVInt([out], length)
			[out].write(bytes.array(), 0, length)
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
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void validateUTF8(byte[] utf8) throws MalformedInputException
		Public Shared Sub validateUTF8(ByVal utf8() As SByte)
			validateUTF8(utf8, 0, utf8.Length)
		End Sub

		''' <summary>
		''' Check to see if a byte array is valid utf-8 </summary>
		''' <param name="utf8"> the array of bytes </param>
		''' <param name="start"> the offset of the first byte in the array </param>
		''' <param name="len"> the length of the byte sequence </param>
		''' <exception cref="MalformedInputException"> if the byte array contains invalid bytes </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void validateUTF8(byte[] utf8, int start, int len) throws MalformedInputException
		Public Shared Sub validateUTF8(ByVal utf8() As SByte, ByVal start As Integer, ByVal len As Integer)
			Dim count As Integer = start
			Dim leadByte As Integer = 0
			Dim length As Integer = 0
			Dim state As Integer = LEAD_BYTE
			Do While count < start + len
				Dim aByte As Integer = (CInt(utf8(count)) And &HFF)

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

					Case TRAIL_BYTE_1
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
						' falls through to regular trail-byte test!!
					Case TRAIL_BYTE
						If aByte < &H80 OrElse aByte > &HBF Then
							Throw New MalformedInputException(count)
						End If
						length -= 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (--length == 0)
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
		Friend Shared ReadOnly bytesFromUTF8() As Integer = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5}

		''' <summary>
		''' Returns the next code point at the current position in
		''' the buffer. The buffer's position will be incremented.
		''' Any mark set on this buffer will be changed by this method!
		''' </summary>
		Public Shared Function bytesToCodePoint(ByVal bytes As ByteBuffer) As Integer
			bytes.mark()
			Dim b As SByte = bytes.get()
			bytes.reset()
			Dim extraBytesToRead As Integer = bytesFromUTF8((b And &HFF))
			If extraBytesToRead < 0 Then
				Return -1 ' trailing byte!
			End If
			Dim ch As Integer = 0

			Select Case extraBytesToRead
				Case 5
					ch += (bytes.get() And &HFF)
					ch <<= 6 ' remember, illegal UTF-8
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case 4
					ch += (bytes.get() And &HFF)
					ch <<= 6 ' remember, illegal UTF-8
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case 3
					ch += (bytes.get() And &HFF)
					ch <<= 6
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case 2
					ch += (bytes.get() And &HFF)
					ch <<= 6
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case 1
					ch += (bytes.get() And &HFF)
					ch <<= 6
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case 0
					ch += (bytes.get() And &HFF)
			End Select
			ch -= offsetsFromUTF8(extraBytesToRead)

			Return ch
		End Function


		Friend Shared ReadOnly offsetsFromUTF8() As Integer = {&H0, &H3080, &HE2080, &H3C82080, &HFA082080L, &H82082080L}

		''' <summary>
		''' For the given string, returns the number of UTF-8 bytes
		''' required to encode the string. </summary>
		''' <param name="string"> text to encode </param>
		''' <returns> number of UTF-8 bytes required to encode </returns>
		Public Shared Function utf8Length(ByVal [string] As String) As Integer
			Dim iter As CharacterIterator = New StringCharacterIterator([string])
			Dim ch As Char = iter.first()
			Dim size As Integer = 0
			Do While ch <> CharacterIterator.DONE
				If (AscW(ch) >= &HD800) AndAlso (AscW(ch) < &HDC00) Then
					' surrogate pair?
					Dim trail As Char = iter.next()
					If (AscW(trail) > &HDBFF) AndAlso (AscW(trail) < &HE000) Then
						' valid pair
						size += 4
					Else
						' invalid pair
						size += 3
						iter.previous() ' rewind one
					End If
				ElseIf AscW(ch) < &H80 Then
					size += 1
				ElseIf AscW(ch) < &H800 Then
					size += 2
				Else
					' ch < 0x10000, that is, the largest char value
					size += 3
				End If
				ch = iter.next()
			Loop
			Return size
		End Function


		Public Overridable Function toDouble() As Double
			If ToString().StartsWith("0x", StringComparison.Ordinal) Then
				Return Long.decode(ToString())
			End If

			Return Double.Parse(ToString())
		End Function

		Public Overridable Function toFloat() As Single
			If ToString().StartsWith("0x", StringComparison.Ordinal) Then
				Return Integer.decode(ToString())
			End If
			Return Single.Parse(ToString())
		End Function

		Public Overridable Function toInt() As Integer
			If ToString().StartsWith("0x", StringComparison.Ordinal) Then
				Return Integer.decode(ToString())
			End If

			Return Integer.Parse(ToString())
		End Function

		Public Overridable Function toLong() As Long
			If ToString().StartsWith("0x", StringComparison.Ordinal) Then
				Return Long.decode(ToString())
			End If

			Return Long.Parse(ToString())
		End Function

		Public Overridable Function [getType]() As WritableType
			Return WritableType.Text
		End Function
	End Class

End Namespace