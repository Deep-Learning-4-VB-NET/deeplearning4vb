Imports System
Imports System.IO
Imports System.Text
Imports Configuration = org.datavec.api.conf.Configuration
Imports ReflectionUtils = org.datavec.api.util.ReflectionUtils
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.io


	Public NotInheritable Class WritableUtils



'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static byte[] readCompressedByteArray(DataInput in) throws IOException
		Public Shared Function readCompressedByteArray(ByVal [in] As DataInput) As SByte()
			Dim length As Integer = [in].readInt()
			If length = -1 Then
				Return Nothing
			End If
			Dim buffer(length - 1) As SByte
			[in].readFully(buffer) ' could/should use readFully(buffer,0,length)?
			Dim gzi As New GZIPInputStream(New MemoryStream(buffer, 0, buffer.Length))
			Dim outbuf(length - 1) As SByte
			Dim bos As New MemoryStream()
			Dim len As Integer
			len = gzi.read(outbuf, 0, outbuf.Length)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((len = gzi.read(outbuf, 0, outbuf.length)) != -1)
			Do While len <> -1
				bos.Write(outbuf, 0, len)
					len = gzi.read(outbuf, 0, outbuf.Length)
			Loop
			Dim decompressed() As SByte = bos.toByteArray()
			bos.Close()
			gzi.close()
			Return decompressed
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void skipCompressedByteArray(DataInput in) throws IOException
		Public Shared Sub skipCompressedByteArray(ByVal [in] As DataInput)
			Dim length As Integer = [in].readInt()
			If length <> -1 Then
				skipFully([in], length)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int writeCompressedByteArray(DataOutput out, byte[] bytes) throws IOException
		Public Shared Function writeCompressedByteArray(ByVal [out] As DataOutput, ByVal bytes() As SByte) As Integer
			If bytes IsNot Nothing Then
				Dim bos As New MemoryStream()
				Dim gzout As New GZIPOutputStream(bos)
				gzout.write(bytes, 0, bytes.Length)
				gzout.close()
				Dim buffer() As SByte = bos.toByteArray()
				Dim len As Integer = buffer.Length
				[out].writeInt(len)
				[out].write(buffer, 0, len)
				' debug only! Once we have confidence, can lose this. 
				Return (If(bytes.Length <> 0, (100 * buffer.Length) \ bytes.Length, 0))
			Else
				[out].writeInt(-1)
				Return -1
			End If
		End Function


		' Ugly utility, maybe someone else can do this better  
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readCompressedString(DataInput in) throws IOException
		Public Shared Function readCompressedString(ByVal [in] As DataInput) As String
			Dim bytes() As SByte = readCompressedByteArray([in])
			If bytes Is Nothing Then
				Return Nothing
			End If
			Return StringHelper.NewString(bytes, StandardCharsets.UTF_8)
		End Function


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int writeCompressedString(DataOutput out, String s) throws IOException
		Public Shared Function writeCompressedString(ByVal [out] As DataOutput, ByVal s As String) As Integer
			Return writeCompressedByteArray([out],If(s IsNot Nothing, s.GetBytes(Encoding.UTF8), Nothing))
		End Function

	'    
	'     *
	'     * Write a String as a Network Int n, followed by n Bytes
	'     * Alternative to 16 bit read/writeUTF.
	'     * Encoding standard is... ?
	'     *
	'     
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeString(DataOutput out, String s) throws IOException
		Public Shared Sub writeString(ByVal [out] As DataOutput, ByVal s As String)
			If s IsNot Nothing Then
				Dim buffer() As SByte = s.GetBytes(Encoding.UTF8)
				Dim len As Integer = buffer.Length
				[out].writeInt(len)
				[out].write(buffer, 0, len)
			Else
				[out].writeInt(-1)
			End If
		End Sub

	'    
	'     * Read a String as a Network Int n, followed by n Bytes
	'     * Alternative to 16 bit read/writeUTF.
	'     * Encoding standard is... ?
	'     *
	'     
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readString(DataInput in) throws IOException
		Public Shared Function readString(ByVal [in] As DataInput) As String
			Dim length As Integer = [in].readInt()
			If length = -1 Then
				Return Nothing
			End If
			Dim buffer(length - 1) As SByte
			[in].readFully(buffer) ' could/should use readFully(buffer,0,length)?
			Return StringHelper.NewString(buffer, StandardCharsets.UTF_8)
		End Function


	'    
	'     * Write a String array as a Nework Int N, followed by Int N Byte Array Strings.
	'     * Could be generalised using introspection.
	'     *
	'     
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeStringArray(DataOutput out, String[] s) throws IOException
		Public Shared Sub writeStringArray(ByVal [out] As DataOutput, ByVal s() As String)
			[out].writeInt(s.Length)
			For i As Integer = 0 To s.Length - 1
				writeString([out], s(i))
			Next i
		End Sub

	'    
	'     * Write a String array as a Nework Int N, followed by Int N Byte Array of
	'     * compressed Strings. Handles also null arrays and null values.
	'     * Could be generalised using introspection.
	'     *
	'     
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeCompressedStringArray(DataOutput out, String[] s) throws IOException
		Public Shared Sub writeCompressedStringArray(ByVal [out] As DataOutput, ByVal s() As String)
			If s Is Nothing Then
				[out].writeInt(-1)
				Return
			End If
			[out].writeInt(s.Length)
			For i As Integer = 0 To s.Length - 1
				writeCompressedString([out], s(i))
			Next i
		End Sub

	'    
	'     * Write a String array as a Nework Int N, followed by Int N Byte Array Strings.
	'     * Could be generalised using introspection. Actually this bit couldn't...
	'     *
	'     
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String[] readStringArray(DataInput in) throws IOException
		Public Shared Function readStringArray(ByVal [in] As DataInput) As String()
			Dim len As Integer = [in].readInt()
			If len = -1 Then
				Return Nothing
			End If
			Dim s(len - 1) As String
			For i As Integer = 0 To len - 1
				s(i) = readString([in])
			Next i
			Return s
		End Function


	'    
	'     * Write a String array as a Nework Int N, followed by Int N Byte Array Strings.
	'     * Could be generalised using introspection. Handles null arrays and null values.
	'     *
	'     
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String[] readCompressedStringArray(DataInput in) throws IOException
		Public Shared Function readCompressedStringArray(ByVal [in] As DataInput) As String()
			Dim len As Integer = [in].readInt()
			If len = -1 Then
				Return Nothing
			End If
			Dim s(len - 1) As String
			For i As Integer = 0 To len - 1
				s(i) = readCompressedString([in])
			Next i
			Return s
		End Function


	'    
	'     *
	'     * Test Utility Method Display Byte Array.
	'     *
	'     
		Public Shared Sub displayByteArray(ByVal record() As SByte)
			Dim i As Integer
			For i = 0 To record.Length - 2
				If i Mod 16 = 0 Then
					Console.WriteLine()
				End If
				Console.Write((record(i) >> 4 And &HF).ToString("x"))
				Console.Write((record(i) And &HF).ToString("x"))
				Console.Write(",")
			Next i
			Console.Write((record(i) >> 4 And &HF).ToString("x"))
			Console.Write((record(i) And &HF).ToString("x"))
			Console.WriteLine()
		End Sub

		''' <summary>
		''' Make a copy of a writable object using serialization to a buffer. </summary>
		''' <param name="orig"> The object to copy </param>
		''' <returns> The copied object </returns>
		Public Shared Function clone(Of T As Writable)(ByVal orig As T, ByVal conf As Configuration) As T
			Try
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") T newInst = org.datavec.api.util.ReflectionUtils.newInstance((@Class<T>) orig.getClass(), conf);
				Dim newInst As T = ReflectionUtils.newInstance(CType(orig.GetType(), Type(Of T)), conf)
				ReflectionUtils.copy(conf, orig, newInst)
				Return newInst
			Catch e As IOException
				Throw New Exception("Error writing/reading clone buffer", e)
			End Try
		End Function



		''' <summary>
		''' Serializes an integer to a binary stream with zero-compressed encoding.
		''' For -120 <= i <= 127, only one byte is used with the actual value.
		''' For other values of i, the first byte value indicates whether the
		''' integer is positive or negative, and the number of bytes that follow.
		''' If the first byte value v is between -121 and -124, the following integer
		''' is positive, with number of bytes that follow are -(v+120).
		''' If the first byte value v is between -125 and -128, the following integer
		''' is negative, with number of bytes that follow are -(v+124). Bytes are
		''' stored in the high-non-zero-byte-first order.
		''' </summary>
		''' <param name="stream"> Binary output stream </param>
		''' <param name="i"> Integer to be serialized </param>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeVInt(DataOutput stream, int i) throws IOException
		Public Shared Sub writeVInt(ByVal stream As DataOutput, ByVal i As Integer)
			writeVLong(stream, i)
		End Sub

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
'ORIGINAL LINE: public static void writeVLong(DataOutput stream, long i) throws IOException
		Public Shared Sub writeVLong(ByVal stream As DataOutput, ByVal i As Long)
			If i >= -112 AndAlso i <= 127 Then
				stream.writeByte(CSByte(i))
				Return
			End If

			Dim len As Integer = -112
			If i < 0 Then
				i = i Xor -1L ' take one's complement'
				len = -120
			End If

			Dim tmp As Long = i
			Do While tmp <> 0
				tmp = tmp >> 8
				len -= 1
			Loop

			stream.writeByte(CSByte(len))

			len = If(len < -120, -(len + 120), -(len + 112))

			Dim idx As Integer = len
			Do While idx <> 0
				Dim shiftbits As Integer = (idx - 1) * 8
				Dim mask As Long = &HFFL << shiftbits
				stream.writeByte(CSByte((i And mask) >> shiftbits))
				idx -= 1
			Loop
		End Sub


		''' <summary>
		''' Reads a zero-compressed encoded long from input stream and returns it. </summary>
		''' <param name="stream"> Binary input stream </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized long from stream. </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static long readVLong(DataInput stream) throws IOException
		Public Shared Function readVLong(ByVal stream As DataInput) As Long
			Dim firstByte As SByte = stream.readByte()
			Dim len As Integer = decodeVIntSize(firstByte)
			If len = 1 Then
				Return firstByte
			End If
			Dim i As Long = 0
			Dim idx As Integer = 0
			Do While idx < len - 1
				Dim b As SByte = stream.readByte()
				i = i << 8
				i = i Or (b And &HFF)
				idx += 1
			Loop
			Return (If(isNegativeVInt(firstByte), (i Xor -1L), i))
		End Function

		''' <summary>
		''' Reads a zero-compressed encoded integer from input stream and returns it. </summary>
		''' <param name="stream"> Binary input stream </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <returns> deserialized integer from stream. </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static int readVInt(DataInput stream) throws IOException
		Public Shared Function readVInt(ByVal stream As DataInput) As Integer
			Return CInt(readVLong(stream))
		End Function

		''' <summary>
		''' Given the first byte of a vint/vlong, determine the sign </summary>
		''' <param name="value"> the first byte </param>
		''' <returns> is the value negative </returns>
		Public Shared Function isNegativeVInt(ByVal value As SByte) As Boolean
			Return value < -120 OrElse (value >= -112 AndAlso value < 0)
		End Function

		''' <summary>
		''' Parse the first byte of a vint/vlong to determine the number of bytes </summary>
		''' <param name="value"> the first byte of the vint/vlong </param>
		''' <returns> the total number of bytes (1 to 9) </returns>
		Public Shared Function decodeVIntSize(ByVal value As SByte) As Integer
			If value >= -112 Then
				Return 1
			ElseIf value < -120 Then
				Return -119 - value
			End If
			Return -111 - value
		End Function

		''' <summary>
		''' Get the encoded length if an integer is stored in a variable-length format </summary>
		''' <returns> the encoded length </returns>
		Public Shared Function getVIntSize(ByVal i As Long) As Integer
			If i >= -112 AndAlso i <= 127 Then
				Return 1
			End If

			If i < 0 Then
				i = i Xor -1L ' take one's complement'
			End If
			' find the number of bytes with non-leading zeros
			Dim dataBits As Integer = (Len(New Long()) * 8) - Long.numberOfLeadingZeros(i)
			' find the number of data bytes + length byte
			Return (dataBits + 7) \ 8 + 1
		End Function

		''' <summary>
		''' Read an Enum value from DataInput, Enums are read and written
		''' using String values. </summary>
		''' @param <T> Enum type </param>
		''' <param name="in"> DataInput to read from </param>
		''' <param name="enumType"> Class type of Enum </param>
		''' <returns> Enum represented by String read from DataInput </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends @Enum<T>> T readEnum(DataInput in, @Class<T> enumType) throws IOException
		Public Shared Function readEnum(Of T As [Enum](Of T))(ByVal [in] As DataInput, ByVal enumType As Type(Of T)) As T
			Return T.valueOf(enumType, Text.readString([in]))
		End Function

		''' <summary>
		''' writes String value of enum to DataOutput. </summary>
		''' <param name="out"> Dataoutput stream </param>
		''' <param name="enumVal"> enum value </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeEnum(DataOutput out, @Enum<?> enumVal) throws IOException
		Public Shared Sub writeEnum(Of T1)(ByVal [out] As DataOutput, ByVal enumVal As [Enum](Of T1))
			Text.writeString([out], enumVal.name())
		End Sub

		''' <summary>
		''' Skip <i>len</i> number of bytes in input stream<i>in</i> </summary>
		''' <param name="in"> input stream </param>
		''' <param name="len"> number of bytes to skip </param>
		''' <exception cref="IOException"> when skipped less number of bytes </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void skipFully(DataInput in, int len) throws IOException
		Public Shared Sub skipFully(ByVal [in] As DataInput, ByVal len As Integer)
			Dim total As Integer = 0
			Dim cur As Integer = 0

			cur = [in].skipBytes(len - total)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((total < len) && ((cur = in.skipBytes(len - total)) > 0))
			Do While (total < len) AndAlso (cur > 0)
				total += cur
					cur = [in].skipBytes(len - total)
			Loop

			If total < len Then
				Throw New IOException("Not able to skip " & len & " bytes, possibly " & "due to end of input.")
			End If
		End Sub

		''' <summary>
		''' Convert writables to a byte array </summary>
		Public Shared Function toByteArray(ParamArray ByVal writables() As Writable) As SByte()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final DataOutputBuffer out = new DataOutputBuffer();
			Dim [out] As New DataOutputBuffer()
			Try
				For Each w As Writable In writables
					w.write([out])
				Next w
				[out].close()
			Catch e As IOException
				Throw New Exception("Fail to convert writables to a byte array", e)
			End Try
			Return [out].Data
		End Function
	End Class

End Namespace