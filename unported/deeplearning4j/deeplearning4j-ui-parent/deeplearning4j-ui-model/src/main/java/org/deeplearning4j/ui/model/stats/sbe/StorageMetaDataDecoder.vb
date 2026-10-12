Imports System
Imports System.Collections.Generic
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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.StorageMetaDataDecoder"}) @SuppressWarnings("all") public class StorageMetaDataDecoder
	Public Class StorageMetaDataDecoder
		Public Const BLOCK_LENGTH As Integer = 8
		Public Const TEMPLATE_ID As Integer = 3
		Public Const SCHEMA_ID As Integer = 1
		Public Const SCHEMA_VERSION As Integer = 0

		Private ReadOnly parentMessage As StorageMetaDataDecoder = Me
		Private buffer As DirectBuffer
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
'ORIGINAL LINE: public StorageMetaDataDecoder wrap(final org.agrona.DirectBuffer buffer, final int offset, final int actingBlockLength, final int actingVersion)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal actingBlockLength As Integer, ByVal actingVersion As Integer) As StorageMetaDataDecoder
			Me.buffer = buffer
			Me.offset_Conflict = offset
			Me.actingBlockLength = actingBlockLength
			Me.actingVersion = actingVersion
			limit(offset + actingBlockLength)

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

		Public Shared Function timeStampId() As Integer
			Return 1
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String timeStampMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function timeStampMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function timeStampNullValue() As Long
			Return -9223372036854775808L
		End Function

		Public Shared Function timeStampMinValue() As Long
			Return -9223372036854775807L
		End Function

		Public Shared Function timeStampMaxValue() As Long
			Return 9223372036854775807L
		End Function

		Public Overridable Function timeStamp() As Long
			Return buffer.getLong(offset_Conflict + 0, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


'JAVA TO VB CONVERTER NOTE: The field extraMetaDataBytes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly extraMetaDataBytes_Conflict As New ExtraMetaDataBytesDecoder()

		Public Shared Function extraMetaDataBytesDecoderId() As Long
			Return 2
		End Function

		Public Overridable Function extraMetaDataBytes() As ExtraMetaDataBytesDecoder
			extraMetaDataBytes_Conflict.wrap(parentMessage, buffer)
			Return extraMetaDataBytes_Conflict
		End Function

		Public Class ExtraMetaDataBytesDecoder
			Implements IEnumerable(Of ExtraMetaDataBytesDecoder), IEnumerator(Of ExtraMetaDataBytesDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As StorageMetaDataDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StorageMetaDataDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As StorageMetaDataDecoder, ByVal buffer As DirectBuffer)
				Me.parentMessage = parentMessage
				Me.buffer = buffer
				dimensions.wrap(buffer, parentMessage.limit())
				blockLength = dimensions.blockLength()
				count_Conflict = dimensions.numInGroup()
				index = -1
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 1
			End Function

			Public Overridable Function actingBlockLength() As Integer
				Return blockLength
			End Function

			Public Overridable Function count() As Integer
				Return count_Conflict
			End Function

			Public Overridable Function GetEnumerator() As IEnumerator(Of ExtraMetaDataBytesDecoder) Implements IEnumerator(Of ExtraMetaDataBytesDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As ExtraMetaDataBytesDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function bytesId() As Integer
				Return 3
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String bytesMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function bytesMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function bytesNullValue() As SByte
				Return (SByte) -128
			End Function

			Public Shared Function bytesMinValue() As SByte
				Return (SByte) -127
			End Function

			Public Shared Function bytesMaxValue() As SByte
				Return CSByte(127)
			End Function

			Public Overridable Function bytes() As SByte
				Return buffer.getByte(offset + 0)
			End Function


			Public Overrides Function ToString() As String
				Return appendTo(New StringBuilder(100)).ToString()
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
			Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
				builder.Append("("c)
				'Token{signal=BEGIN_FIELD, name='bytes', description='null', id=3, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int8', description='null', id=-1, version=0, encodedLength=1, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("bytes=")
				builder.Append(bytes())
				builder.Append(")"c)
				Return builder
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

		Public Overridable Function sessionIDLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSessionID(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSessionID(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSessionID(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSessionID(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

		Public Overridable Function sessionID() As String
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			parentMessage.limit(limit + headerLength + dataLength)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] tmp = new byte[dataLength];
			Dim tmp(dataLength - 1) As SByte
			buffer.getBytes(limit + headerLength, tmp, 0, dataLength)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String value;
			Dim value As String
			Try
				value = StringHelper.NewString(tmp, "UTF-8")
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

			Return value
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

		Public Overridable Function typeIDLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getTypeID(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getTypeID(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getTypeID(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getTypeID(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

		Public Overridable Function typeID() As String
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			parentMessage.limit(limit + headerLength + dataLength)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] tmp = new byte[dataLength];
			Dim tmp(dataLength - 1) As SByte
			buffer.getBytes(limit + headerLength, tmp, 0, dataLength)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String value;
			Dim value As String
			Try
				value = StringHelper.NewString(tmp, "UTF-8")
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

			Return value
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

		Public Overridable Function workerIDLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getWorkerID(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getWorkerID(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getWorkerID(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getWorkerID(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

		Public Overridable Function workerID() As String
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			parentMessage.limit(limit + headerLength + dataLength)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] tmp = new byte[dataLength];
			Dim tmp(dataLength - 1) As SByte
			buffer.getBytes(limit + headerLength, tmp, 0, dataLength)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String value;
			Dim value As String
			Try
				value = StringHelper.NewString(tmp, "UTF-8")
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

			Return value
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

		Public Overridable Function initTypeClassLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getInitTypeClass(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getInitTypeClass(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getInitTypeClass(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getInitTypeClass(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

		Public Overridable Function initTypeClass() As String
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			parentMessage.limit(limit + headerLength + dataLength)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] tmp = new byte[dataLength];
			Dim tmp(dataLength - 1) As SByte
			buffer.getBytes(limit + headerLength, tmp, 0, dataLength)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String value;
			Dim value As String
			Try
				value = StringHelper.NewString(tmp, "UTF-8")
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

			Return value
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

		Public Overridable Function updateTypeClassLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getUpdateTypeClass(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getUpdateTypeClass(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getUpdateTypeClass(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getUpdateTypeClass(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int bytesCopied = Math.min(length, dataLength);
			Dim bytesCopied As Integer = Math.Min(length, dataLength)
			parentMessage.limit(limit + headerLength + dataLength)
			buffer.getBytes(limit + headerLength, dst, dstOffset, bytesCopied)

			Return bytesCopied
		End Function

		Public Overridable Function updateTypeClass() As String
			Const headerLength As Integer = 4
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int dataLength = (int)(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) & &HFFFF_FFFFL);
			Dim dataLength As Integer = CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			parentMessage.limit(limit + headerLength + dataLength)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] tmp = new byte[dataLength];
			Dim tmp(dataLength - 1) As SByte
			buffer.getBytes(limit + headerLength, tmp, 0, dataLength)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String value;
			Dim value As String
			Try
				value = StringHelper.NewString(tmp, "UTF-8")
'JAVA TO VB CONVERTER WARNING: 'final' catch parameters are not available in VB:
'ORIGINAL LINE: catch (final java.io.UnsupportedEncodingException ex)
			Catch ex As java.io.UnsupportedEncodingException
				Throw New Exception(ex)
			End Try

			Return value
		End Function

		Public Overrides Function ToString() As String
			Return appendTo(New StringBuilder(100)).ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
		Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int originalLimit = limit();
			Dim originalLimit As Integer = limit()
			limit(offset_Conflict + actingBlockLength)
			builder.Append("[StorageMetaData](sbeTemplateId=")
			builder.Append(TEMPLATE_ID)
			builder.Append("|sbeSchemaId=")
			builder.Append(SCHEMA_ID)
			builder.Append("|sbeSchemaVersion=")
			If actingVersion <> SCHEMA_VERSION Then
				builder.Append(actingVersion)
				builder.Append("/"c)
			End If
			builder.Append(SCHEMA_VERSION)
			builder.Append("|sbeBlockLength=")
			If actingBlockLength <> BLOCK_LENGTH Then
				builder.Append(actingBlockLength)
				builder.Append("/"c)
			End If
			builder.Append(BLOCK_LENGTH)
			builder.Append("):")
			'Token{signal=BEGIN_FIELD, name='timeStamp', description='null', id=1, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("timeStamp=")
			builder.Append(timeStamp())
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='extraMetaDataBytes', description='Extra metadata bytes', id=2, version=0, encodedLength=1, offset=8, componentTokenCount=9, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("extraMetaDataBytes=[")
			Dim extraMetaDataBytes As ExtraMetaDataBytesDecoder = Me.extraMetaDataBytes()
			If extraMetaDataBytes.count() > 0 Then
				Do While extraMetaDataBytes.MoveNext()
					extraMetaDataBytes.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='sessionID', description='null', id=4, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("sessionID=")
			builder.Append(sessionID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='typeID', description='null', id=5, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("typeID=")
			builder.Append(typeID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='workerID', description='null', id=6, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("workerID=")
			builder.Append(workerID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='initTypeClass', description='null', id=7, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("initTypeClass=")
			builder.Append(initTypeClass())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='updateTypeClass', description='null', id=8, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("updateTypeClass=")
			builder.Append(updateTypeClass())

			limit(originalLimit)

			Return builder
		End Function
	End Class

End Namespace