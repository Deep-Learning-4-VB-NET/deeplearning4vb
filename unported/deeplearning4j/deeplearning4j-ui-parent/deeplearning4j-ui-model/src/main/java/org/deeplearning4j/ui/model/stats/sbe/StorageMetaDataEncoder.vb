Imports System
Imports System.Text
Imports DirectBuffer = org.agrona.DirectBuffer
Imports MutableDirectBuffer = org.agrona.MutableDirectBuffer

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

Namespace org.deeplearning4j.ui.model.stats.sbe

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.StorageMetaDataEncoder"}) @SuppressWarnings("all") public class StorageMetaDataEncoder
	Public Class StorageMetaDataEncoder
		Public Const BLOCK_LENGTH As Integer = 8
		Public Const TEMPLATE_ID As Integer = 3
		Public Const SCHEMA_ID As Integer = 1
		Public Const SCHEMA_VERSION As Integer = 0

		Private ReadOnly parentMessage As StorageMetaDataEncoder = Me
		Private buffer As MutableDirectBuffer
'JAVA TO VB CONVERTER NOTE: The field offset was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend offset_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field limit was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend limit_Conflict As Integer
		Protected Friend actingBlockLength As Integer
		Protected Friend actingVersion As Integer

		Public Overridable Function sbeBlockLength() As Integer
			Return BLOCK_LENGTH
		End Function

		Public Overridable Function sbeTemplateId() As Integer
			Return TEMPLATE_ID
		End Function

		Public Overridable Function sbeSchemaId() As Integer
			Return SCHEMA_ID
		End Function

		Public Overridable Function sbeSchemaVersion() As Integer
			Return SCHEMA_VERSION
		End Function

		Public Overridable Function sbeSemanticType() As String
			Return ""
		End Function

		Public Overridable Function offset() As Integer
			Return offset_Conflict
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder wrap(final org.agrona.MutableDirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As MutableDirectBuffer, ByVal offset As Integer) As StorageMetaDataEncoder
			Me.buffer = buffer
			Me.offset_Conflict = offset
			limit(offset + BLOCK_LENGTH)

			Return Me
		End Function

		Public Overridable Function encodedLength() As Integer
			Return limit_Conflict - offset_Conflict
		End Function

		Public Overridable Function limit() As Integer
			Return limit_Conflict
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void limit(final int limit)
		Public Overridable Sub limit(ByVal limit As Integer)
			Me.limit_Conflict = limit
		End Sub

		Public Shared Function timeStampNullValue() As Long
			Return -9223372036854775808L
		End Function

		Public Shared Function timeStampMinValue() As Long
			Return -9223372036854775807L
		End Function

		Public Shared Function timeStampMaxValue() As Long
			Return 9223372036854775807L
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder timeStamp(final long value)
		Public Overridable Function timeStamp(ByVal value As Long) As StorageMetaDataEncoder
			buffer.putLong(offset_Conflict + 0, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Private ReadOnly extraMetaDataBytes As New ExtraMetaDataBytesEncoder()

		Public Shared Function extraMetaDataBytesId() As Long
			Return 2
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public ExtraMetaDataBytesEncoder extraMetaDataBytesCount(final int count)
		Public Overridable Function extraMetaDataBytesCount(ByVal count As Integer) As ExtraMetaDataBytesEncoder
			extraMetaDataBytes.wrap(parentMessage, buffer, count)
			Return extraMetaDataBytes
		End Function

		Public Class ExtraMetaDataBytesEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As StorageMetaDataEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StorageMetaDataEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As StorageMetaDataEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
				If count < 0 OrElse count > 65534 Then
					Throw New System.ArgumentException("count outside allowed range: count=" & count)
				End If

				Me.parentMessage = parentMessage
				Me.buffer = buffer
				actingVersion = SCHEMA_VERSION
				dimensions.wrap(buffer, parentMessage.limit())
				dimensions.blockLength(CInt(1))
				dimensions.numInGroup(CInt(count))
				index = -1
				Me.count = count
				blockLength = 1
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 1
			End Function

			Public Overridable Function [next]() As ExtraMetaDataBytesEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function bytesNullValue() As SByte
				Return (SByte) -128
			End Function

			Public Shared Function bytesMinValue() As SByte
				Return (SByte) -127
			End Function

			Public Shared Function bytesMaxValue() As SByte
				Return CSByte(127)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public ExtraMetaDataBytesEncoder bytes(final byte value)
			Public Overridable Function bytes(ByVal value As SByte) As ExtraMetaDataBytesEncoder
				buffer.putByte(offset + 0, value)
				Return Me
			End Function

		End Class

		Public Shared Function sessionIDId() As Integer
			Return 4
		End Function

		Public Shared Function sessionIDCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String sessionIDMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function sessionIDMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
			Select Case metaAttribute
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.EPOCH
					Return "unix"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.TIME_UNIT
					Return "nanosecond"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.SEMANTIC_TYPE
					Return ""
			End Select

			Return ""
		End Function

		Public Shared Function sessionIDHeaderLength() As Integer
			Return 4
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putSessionID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSessionID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putSessionID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSessionID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder sessionID(final String value)
		Public Overridable Function sessionID(ByVal value As String) As StorageMetaDataEncoder
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] bytes;
			Dim bytes() As SByte
			Try
				bytes = value.GetBytes(Encoding.UTF8)
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int length = bytes.length;
			Dim length As Integer = bytes.Length
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, bytes, 0, length)

			Return Me
		End Function

		Public Shared Function typeIDId() As Integer
			Return 5
		End Function

		Public Shared Function typeIDCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String typeIDMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function typeIDMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
			Select Case metaAttribute
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.EPOCH
					Return "unix"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.TIME_UNIT
					Return "nanosecond"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.SEMANTIC_TYPE
					Return ""
			End Select

			Return ""
		End Function

		Public Shared Function typeIDHeaderLength() As Integer
			Return 4
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putTypeID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putTypeID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putTypeID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putTypeID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder typeID(final String value)
		Public Overridable Function typeID(ByVal value As String) As StorageMetaDataEncoder
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] bytes;
			Dim bytes() As SByte
			Try
				bytes = value.GetBytes(Encoding.UTF8)
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int length = bytes.length;
			Dim length As Integer = bytes.Length
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, bytes, 0, length)

			Return Me
		End Function

		Public Shared Function workerIDId() As Integer
			Return 6
		End Function

		Public Shared Function workerIDCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String workerIDMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function workerIDMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
			Select Case metaAttribute
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.EPOCH
					Return "unix"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.TIME_UNIT
					Return "nanosecond"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.SEMANTIC_TYPE
					Return ""
			End Select

			Return ""
		End Function

		Public Shared Function workerIDHeaderLength() As Integer
			Return 4
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putWorkerID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putWorkerID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putWorkerID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putWorkerID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder workerID(final String value)
		Public Overridable Function workerID(ByVal value As String) As StorageMetaDataEncoder
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] bytes;
			Dim bytes() As SByte
			Try
				bytes = value.GetBytes(Encoding.UTF8)
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int length = bytes.length;
			Dim length As Integer = bytes.Length
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, bytes, 0, length)

			Return Me
		End Function

		Public Shared Function initTypeClassId() As Integer
			Return 7
		End Function

		Public Shared Function initTypeClassCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String initTypeClassMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function initTypeClassMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
			Select Case metaAttribute
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.EPOCH
					Return "unix"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.TIME_UNIT
					Return "nanosecond"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.SEMANTIC_TYPE
					Return ""
			End Select

			Return ""
		End Function

		Public Shared Function initTypeClassHeaderLength() As Integer
			Return 4
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putInitTypeClass(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putInitTypeClass(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putInitTypeClass(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putInitTypeClass(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder initTypeClass(final String value)
		Public Overridable Function initTypeClass(ByVal value As String) As StorageMetaDataEncoder
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] bytes;
			Dim bytes() As SByte
			Try
				bytes = value.GetBytes(Encoding.UTF8)
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int length = bytes.length;
			Dim length As Integer = bytes.Length
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, bytes, 0, length)

			Return Me
		End Function

		Public Shared Function updateTypeClassId() As Integer
			Return 8
		End Function

		Public Shared Function updateTypeClassCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String updateTypeClassMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function updateTypeClassMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
			Select Case metaAttribute
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.EPOCH
					Return "unix"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.TIME_UNIT
					Return "nanosecond"
				Case org.deeplearning4j.ui.model.stats.sbe.MetaAttribute.SEMANTIC_TYPE
					Return ""
			End Select

			Return ""
		End Function

		Public Shared Function updateTypeClassHeaderLength() As Integer
			Return 4
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putUpdateTypeClass(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putUpdateTypeClass(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder putUpdateTypeClass(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putUpdateTypeClass(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StorageMetaDataEncoder
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, src, srcOffset, length)

			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StorageMetaDataEncoder updateTypeClass(final String value)
		Public Overridable Function updateTypeClass(ByVal value As String) As StorageMetaDataEncoder
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] bytes;
			Dim bytes() As SByte
			Try
				bytes = value.GetBytes(Encoding.UTF8)
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int length = bytes.length;
			Dim length As Integer = bytes.Length
			If length > 1073741824 Then
				Throw New System.ArgumentException("length > max value for type: " & length)
			End If

			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			parentMessage.limit(limit + headerLength + length)
			buffer.putInt(limit, CInt(length), java.nio.ByteOrder.LITTLE_ENDIAN)
			buffer.putBytes(limit + headerLength, bytes, 0, length)

			Return Me
		End Function

		Public Overrides Function ToString() As String
			Return appendTo(New StringBuilder(100)).ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
		Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
			Dim writer As New StorageMetaDataDecoder()
			writer.wrap(buffer, offset_Conflict, BLOCK_LENGTH, SCHEMA_VERSION)

			Return writer.appendTo(builder)
		End Function
	End Class

End Namespace