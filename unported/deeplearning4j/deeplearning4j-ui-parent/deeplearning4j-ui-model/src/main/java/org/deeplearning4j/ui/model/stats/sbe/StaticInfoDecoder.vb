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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.StaticInfoDecoder"}) @SuppressWarnings("all") public class StaticInfoDecoder
	Public Class StaticInfoDecoder
		Public Const BLOCK_LENGTH As Integer = 40
		Public Const TEMPLATE_ID As Integer = 1
		Public Const SCHEMA_ID As Integer = 1
		Public Const SCHEMA_VERSION As Integer = 0

		Private ReadOnly parentMessage As StaticInfoDecoder = Me
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
'ORIGINAL LINE: public StaticInfoDecoder wrap(final org.agrona.DirectBuffer buffer, final int offset, final int actingBlockLength, final int actingVersion)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal actingBlockLength As Integer, ByVal actingVersion As Integer) As StaticInfoDecoder
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

		Public Shared Function timeId() As Integer
			Return 1
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String timeMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function timeMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function timeNullValue() As Long
			Return -9223372036854775808L
		End Function

		Public Shared Function timeMinValue() As Long
			Return -9223372036854775807L
		End Function

		Public Shared Function timeMaxValue() As Long
			Return 9223372036854775807L
		End Function

		Public Overridable Function time() As Long
			Return buffer.getLong(offset_Conflict + 0, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


		Public Shared Function fieldsPresentId() As Integer
			Return 2
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String fieldsPresentMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function fieldsPresentMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

'JAVA TO VB CONVERTER NOTE: The field fieldsPresent was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly fieldsPresent_Conflict As New InitFieldsPresentDecoder()

		Public Overridable Function fieldsPresent() As InitFieldsPresentDecoder
			fieldsPresent_Conflict.wrap(buffer, offset_Conflict + 8)
			Return fieldsPresent_Conflict
		End Function

		Public Shared Function hwJvmProcessorsId() As Integer
			Return 3
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String hwJvmProcessorsMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function hwJvmProcessorsMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function hwJvmProcessorsNullValue() As Integer
			Return 65535
		End Function

		Public Shared Function hwJvmProcessorsMinValue() As Integer
			Return 0
		End Function

		Public Shared Function hwJvmProcessorsMaxValue() As Integer
			Return 65534
		End Function

		Public Overridable Function hwJvmProcessors() As Integer
			Return (buffer.getShort(offset_Conflict + 9, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF)
		End Function


		Public Shared Function hwNumDevicesId() As Integer
			Return 4
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String hwNumDevicesMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function hwNumDevicesMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function hwNumDevicesNullValue() As Short
			Return CShort(255)
		End Function

		Public Shared Function hwNumDevicesMinValue() As Short
			Return CShort(0)
		End Function

		Public Shared Function hwNumDevicesMaxValue() As Short
			Return CShort(254)
		End Function

		Public Overridable Function hwNumDevices() As Short
			Return (CShort(buffer.getByte(offset_Conflict + 11) And &HFF))
		End Function


		Public Shared Function hwJvmMaxMemoryId() As Integer
			Return 5
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String hwJvmMaxMemoryMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function hwJvmMaxMemoryMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function hwJvmMaxMemoryNullValue() As Long
			Return -9223372036854775808L
		End Function

		Public Shared Function hwJvmMaxMemoryMinValue() As Long
			Return -9223372036854775807L
		End Function

		Public Shared Function hwJvmMaxMemoryMaxValue() As Long
			Return 9223372036854775807L
		End Function

		Public Overridable Function hwJvmMaxMemory() As Long
			Return buffer.getLong(offset_Conflict + 12, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


		Public Shared Function hwOffheapMaxMemoryId() As Integer
			Return 6
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String hwOffheapMaxMemoryMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function hwOffheapMaxMemoryMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function hwOffheapMaxMemoryNullValue() As Long
			Return -9223372036854775808L
		End Function

		Public Shared Function hwOffheapMaxMemoryMinValue() As Long
			Return -9223372036854775807L
		End Function

		Public Shared Function hwOffheapMaxMemoryMaxValue() As Long
			Return 9223372036854775807L
		End Function

		Public Overridable Function hwOffheapMaxMemory() As Long
			Return buffer.getLong(offset_Conflict + 20, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


		Public Shared Function modelNumLayersId() As Integer
			Return 7
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String modelNumLayersMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function modelNumLayersMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function modelNumLayersNullValue() As Integer
			Return -2147483648
		End Function

		Public Shared Function modelNumLayersMinValue() As Integer
			Return -2147483647
		End Function

		Public Shared Function modelNumLayersMaxValue() As Integer
			Return 2147483647
		End Function

		Public Overridable Function modelNumLayers() As Integer
			Return buffer.getInt(offset_Conflict + 28, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


		Public Shared Function modelNumParamsId() As Integer
			Return 8
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String modelNumParamsMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function modelNumParamsMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function modelNumParamsNullValue() As Long
			Return -9223372036854775808L
		End Function

		Public Shared Function modelNumParamsMinValue() As Long
			Return -9223372036854775807L
		End Function

		Public Shared Function modelNumParamsMaxValue() As Long
			Return 9223372036854775807L
		End Function

		Public Overridable Function modelNumParams() As Long
			Return buffer.getLong(offset_Conflict + 32, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


'JAVA TO VB CONVERTER NOTE: The field hwDeviceInfoGroup was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly hwDeviceInfoGroup_Conflict As New HwDeviceInfoGroupDecoder()

		Public Shared Function hwDeviceInfoGroupDecoderId() As Long
			Return 9
		End Function

		Public Overridable Function hwDeviceInfoGroup() As HwDeviceInfoGroupDecoder
			hwDeviceInfoGroup_Conflict.wrap(parentMessage, buffer)
			Return hwDeviceInfoGroup_Conflict
		End Function

		Public Class HwDeviceInfoGroupDecoder
			Implements IEnumerable(Of HwDeviceInfoGroupDecoder), IEnumerator(Of HwDeviceInfoGroupDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As StaticInfoDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StaticInfoDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As StaticInfoDecoder, ByVal buffer As DirectBuffer)
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
				Return 8
			End Function

			Public Overridable Function actingBlockLength() As Integer
				Return blockLength
			End Function

			Public Overridable Function count() As Integer
				Return count_Conflict
			End Function

			Public Overridable Function GetEnumerator() As IEnumerator(Of HwDeviceInfoGroupDecoder) Implements IEnumerator(Of HwDeviceInfoGroupDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As HwDeviceInfoGroupDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function deviceMemoryMaxId() As Integer
				Return 10
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String deviceMemoryMaxMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function deviceMemoryMaxMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function deviceMemoryMaxNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function deviceMemoryMaxMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function deviceMemoryMaxMaxValue() As Long
				Return 9223372036854775807L
			End Function

			Public Overridable Function deviceMemoryMax() As Long
				Return buffer.getLong(offset + 0, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Shared Function deviceDescriptionId() As Integer
				Return 50
			End Function

			Public Shared Function deviceDescriptionCharacterEncoding() As String
				Return "UTF-8"
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String deviceDescriptionMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function deviceDescriptionMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function deviceDescriptionHeaderLength() As Integer
				Return 4
			End Function

			Public Overridable Function deviceDescriptionLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
				Dim limit As Integer = parentMessage.limit()
				Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getDeviceDescription(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
			Public Overridable Function getDeviceDescription(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getDeviceDescription(final byte[] dst, final int dstOffset, final int length)
			Public Overridable Function getDeviceDescription(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

			Public Overridable Function deviceDescription() As String
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
				builder.Append("("c)
				'Token{signal=BEGIN_FIELD, name='deviceMemoryMax', description='null', id=10, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("deviceMemoryMax=")
				builder.Append(deviceMemoryMax())
				builder.Append("|"c)
				'Token{signal=BEGIN_VAR_DATA, name='deviceDescription', description='null', id=50, version=0, encodedLength=0, offset=8, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("deviceDescription=")
				builder.Append(deviceDescription())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field swEnvironmentInfo was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly swEnvironmentInfo_Conflict As New SwEnvironmentInfoDecoder()

		Public Shared Function swEnvironmentInfoDecoderId() As Long
			Return 12
		End Function

		Public Overridable Function swEnvironmentInfo() As SwEnvironmentInfoDecoder
			swEnvironmentInfo_Conflict.wrap(parentMessage, buffer)
			Return swEnvironmentInfo_Conflict
		End Function

		Public Class SwEnvironmentInfoDecoder
			Implements IEnumerable(Of SwEnvironmentInfoDecoder), IEnumerator(Of SwEnvironmentInfoDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As StaticInfoDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StaticInfoDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As StaticInfoDecoder, ByVal buffer As DirectBuffer)
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
				Return 0
			End Function

			Public Overridable Function actingBlockLength() As Integer
				Return blockLength
			End Function

			Public Overridable Function count() As Integer
				Return count_Conflict
			End Function

			Public Overridable Function GetEnumerator() As IEnumerator(Of SwEnvironmentInfoDecoder) Implements IEnumerator(Of SwEnvironmentInfoDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As SwEnvironmentInfoDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function envKeyId() As Integer
				Return 51
			End Function

			Public Shared Function envKeyCharacterEncoding() As String
				Return "UTF-8"
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String envKeyMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function envKeyMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function envKeyHeaderLength() As Integer
				Return 4
			End Function

			Public Overridable Function envKeyLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
				Dim limit As Integer = parentMessage.limit()
				Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getEnvKey(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
			Public Overridable Function getEnvKey(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getEnvKey(final byte[] dst, final int dstOffset, final int length)
			Public Overridable Function getEnvKey(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

			Public Overridable Function envKey() As String
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

			Public Shared Function envValueId() As Integer
				Return 52
			End Function

			Public Shared Function envValueCharacterEncoding() As String
				Return "UTF-8"
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String envValueMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function envValueMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function envValueHeaderLength() As Integer
				Return 4
			End Function

			Public Overridable Function envValueLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
				Dim limit As Integer = parentMessage.limit()
				Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getEnvValue(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
			Public Overridable Function getEnvValue(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getEnvValue(final byte[] dst, final int dstOffset, final int length)
			Public Overridable Function getEnvValue(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

			Public Overridable Function envValue() As String
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
				builder.Append("("c)
				'Token{signal=BEGIN_VAR_DATA, name='envKey', description='null', id=51, version=0, encodedLength=0, offset=0, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("envKey=")
				builder.Append(envKey())
				builder.Append("|"c)
				'Token{signal=BEGIN_VAR_DATA, name='envValue', description='null', id=52, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("envValue=")
				builder.Append(envValue())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field modelParamNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly modelParamNames_Conflict As New ModelParamNamesDecoder()

		Public Shared Function modelParamNamesDecoderId() As Long
			Return 11
		End Function

		Public Overridable Function modelParamNames() As ModelParamNamesDecoder
			modelParamNames_Conflict.wrap(parentMessage, buffer)
			Return modelParamNames_Conflict
		End Function

		Public Class ModelParamNamesDecoder
			Implements IEnumerable(Of ModelParamNamesDecoder), IEnumerator(Of ModelParamNamesDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As StaticInfoDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StaticInfoDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As StaticInfoDecoder, ByVal buffer As DirectBuffer)
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
				Return 0
			End Function

			Public Overridable Function actingBlockLength() As Integer
				Return blockLength
			End Function

			Public Overridable Function count() As Integer
				Return count_Conflict
			End Function

			Public Overridable Function GetEnumerator() As IEnumerator(Of ModelParamNamesDecoder) Implements IEnumerator(Of ModelParamNamesDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As ModelParamNamesDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function modelParamNamesId() As Integer
				Return 53
			End Function

			Public Shared Function modelParamNamesCharacterEncoding() As String
				Return "UTF-8"
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String modelParamNamesMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function modelParamNamesMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function modelParamNamesHeaderLength() As Integer
				Return 4
			End Function

			Public Overridable Function modelParamNamesLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
				Dim limit As Integer = parentMessage.limit()
				Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getModelParamNames(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
			Public Overridable Function getModelParamNames(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getModelParamNames(final byte[] dst, final int dstOffset, final int length)
			Public Overridable Function getModelParamNames(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

			Public Overridable Function modelParamNames() As String
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
				builder.Append("("c)
				'Token{signal=BEGIN_VAR_DATA, name='modelParamNames', description='null', id=53, version=0, encodedLength=0, offset=0, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("modelParamNames=")
				builder.Append(modelParamNames())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

		Public Shared Function sessionIDId() As Integer
			Return 100
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
			Return 101
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
			Return 102
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

		Public Shared Function swArchId() As Integer
			Return 201
		End Function

		Public Shared Function swArchCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swArchMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swArchMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swArchHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swArchLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwArch(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwArch(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwArch(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwArch(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swArch() As String
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

		Public Shared Function swOsNameId() As Integer
			Return 202
		End Function

		Public Shared Function swOsNameCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swOsNameMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swOsNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swOsNameHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swOsNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwOsName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwOsName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwOsName(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwOsName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swOsName() As String
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

		Public Shared Function swJvmNameId() As Integer
			Return 203
		End Function

		Public Shared Function swJvmNameCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swJvmNameMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swJvmNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swJvmNameHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swJvmNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwJvmName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwJvmName(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swJvmName() As String
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

		Public Shared Function swJvmVersionId() As Integer
			Return 204
		End Function

		Public Shared Function swJvmVersionCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swJvmVersionMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swJvmVersionMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swJvmVersionHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swJvmVersionLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwJvmVersion(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmVersion(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwJvmVersion(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmVersion(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swJvmVersion() As String
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

		Public Shared Function swJvmSpecVersionId() As Integer
			Return 205
		End Function

		Public Shared Function swJvmSpecVersionCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swJvmSpecVersionMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swJvmSpecVersionMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swJvmSpecVersionHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swJvmSpecVersionLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwJvmSpecVersion(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmSpecVersion(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwJvmSpecVersion(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmSpecVersion(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swJvmSpecVersion() As String
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

		Public Shared Function swNd4jBackendClassId() As Integer
			Return 206
		End Function

		Public Shared Function swNd4jBackendClassCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swNd4jBackendClassMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swNd4jBackendClassMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swNd4jBackendClassHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swNd4jBackendClassLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwNd4jBackendClass(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwNd4jBackendClass(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwNd4jBackendClass(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwNd4jBackendClass(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swNd4jBackendClass() As String
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

		Public Shared Function swNd4jDataTypeNameId() As Integer
			Return 207
		End Function

		Public Shared Function swNd4jDataTypeNameCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swNd4jDataTypeNameMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swNd4jDataTypeNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swNd4jDataTypeNameHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swNd4jDataTypeNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwNd4jDataTypeName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwNd4jDataTypeName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwNd4jDataTypeName(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwNd4jDataTypeName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swNd4jDataTypeName() As String
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

		Public Shared Function swHostNameId() As Integer
			Return 208
		End Function

		Public Shared Function swHostNameCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swHostNameMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swHostNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swHostNameHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swHostNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwHostName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwHostName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwHostName(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwHostName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swHostName() As String
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

		Public Shared Function swJvmUIDId() As Integer
			Return 209
		End Function

		Public Shared Function swJvmUIDCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String swJvmUIDMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function swJvmUIDMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function swJvmUIDHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function swJvmUIDLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getSwJvmUID(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmUID(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getSwJvmUID(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getSwJvmUID(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function swJvmUID() As String
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

		Public Shared Function hwHardwareUIDId() As Integer
			Return 300
		End Function

		Public Shared Function hwHardwareUIDCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String hwHardwareUIDMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function hwHardwareUIDMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function hwHardwareUIDHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function hwHardwareUIDLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getHwHardwareUID(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getHwHardwareUID(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getHwHardwareUID(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getHwHardwareUID(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function hwHardwareUID() As String
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

		Public Shared Function modelConfigClassNameId() As Integer
			Return 400
		End Function

		Public Shared Function modelConfigClassNameCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String modelConfigClassNameMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function modelConfigClassNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function modelConfigClassNameHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function modelConfigClassNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getModelConfigClassName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getModelConfigClassName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getModelConfigClassName(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getModelConfigClassName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function modelConfigClassName() As String
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

		Public Shared Function modelConfigJsonId() As Integer
			Return 401
		End Function

		Public Shared Function modelConfigJsonCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String modelConfigJsonMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function modelConfigJsonMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function modelConfigJsonHeaderLength() As Integer
			Return 4
		End Function

		Public Overridable Function modelConfigJsonLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getModelConfigJson(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getModelConfigJson(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getModelConfigJson(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getModelConfigJson(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function modelConfigJson() As String
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
			builder.Append("[StaticInfo](sbeTemplateId=")
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
			'Token{signal=BEGIN_FIELD, name='time', description='null', id=1, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("time=")
			builder.Append(time())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='fieldsPresent', description='null', id=2, version=0, encodedLength=0, offset=8, componentTokenCount=7, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=BEGIN_SET, name='InitFieldsPresent', description='null', id=-1, version=0, encodedLength=1, offset=8, componentTokenCount=5, encoding=Encoding{presence=REQUIRED, primitiveType=UINT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='FieldsPresent'}}
			builder.Append("fieldsPresent=")
			builder.Append(fieldsPresent())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='hwJvmProcessors', description='null', id=3, version=0, encodedLength=0, offset=9, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='uint16', description='null', id=-1, version=0, encodedLength=2, offset=9, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=UINT16, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("hwJvmProcessors=")
			builder.Append(hwJvmProcessors())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='hwNumDevices', description='null', id=4, version=0, encodedLength=0, offset=11, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='uint8', description='null', id=-1, version=0, encodedLength=1, offset=11, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=UINT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("hwNumDevices=")
			builder.Append(hwNumDevices())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='hwJvmMaxMemory', description='null', id=5, version=0, encodedLength=0, offset=12, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=12, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("hwJvmMaxMemory=")
			builder.Append(hwJvmMaxMemory())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='hwOffheapMaxMemory', description='null', id=6, version=0, encodedLength=0, offset=20, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=20, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("hwOffheapMaxMemory=")
			builder.Append(hwOffheapMaxMemory())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='modelNumLayers', description='null', id=7, version=0, encodedLength=0, offset=28, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int32', description='null', id=-1, version=0, encodedLength=4, offset=28, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("modelNumLayers=")
			builder.Append(modelNumLayers())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='modelNumParams', description='null', id=8, version=0, encodedLength=0, offset=32, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=32, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("modelNumParams=")
			builder.Append(modelNumParams())
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='hwDeviceInfoGroup', description='null', id=9, version=0, encodedLength=8, offset=40, componentTokenCount=15, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("hwDeviceInfoGroup=[")
			Dim hwDeviceInfoGroup As HwDeviceInfoGroupDecoder = Me.hwDeviceInfoGroup()
			If hwDeviceInfoGroup.count() > 0 Then
				Do While hwDeviceInfoGroup.MoveNext()
					hwDeviceInfoGroup.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='swEnvironmentInfo', description='null', id=12, version=0, encodedLength=0, offset=-1, componentTokenCount=18, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("swEnvironmentInfo=[")
			Dim swEnvironmentInfo As SwEnvironmentInfoDecoder = Me.swEnvironmentInfo()
			If swEnvironmentInfo.count() > 0 Then
				Do While swEnvironmentInfo.MoveNext()
					swEnvironmentInfo.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='modelParamNames', description='null', id=11, version=0, encodedLength=0, offset=-1, componentTokenCount=12, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("modelParamNames=[")
			Dim modelParamNames As ModelParamNamesDecoder = Me.modelParamNames()
			If modelParamNames.count() > 0 Then
				Do While modelParamNames.MoveNext()
					modelParamNames.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='sessionID', description='null', id=100, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("sessionID=")
			builder.Append(sessionID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='typeID', description='null', id=101, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("typeID=")
			builder.Append(typeID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='workerID', description='null', id=102, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("workerID=")
			builder.Append(workerID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swArch', description='null', id=201, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swArch=")
			builder.Append(swArch())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swOsName', description='null', id=202, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swOsName=")
			builder.Append(swOsName())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swJvmName', description='null', id=203, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swJvmName=")
			builder.Append(swJvmName())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swJvmVersion', description='null', id=204, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swJvmVersion=")
			builder.Append(swJvmVersion())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swJvmSpecVersion', description='null', id=205, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swJvmSpecVersion=")
			builder.Append(swJvmSpecVersion())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swNd4jBackendClass', description='null', id=206, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swNd4jBackendClass=")
			builder.Append(swNd4jBackendClass())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swNd4jDataTypeName', description='null', id=207, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swNd4jDataTypeName=")
			builder.Append(swNd4jDataTypeName())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swHostName', description='null', id=208, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swHostName=")
			builder.Append(swHostName())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='swJvmUID', description='null', id=209, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("swJvmUID=")
			builder.Append(swJvmUID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='hwHardwareUID', description='null', id=300, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("hwHardwareUID=")
			builder.Append(hwHardwareUID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='modelConfigClassName', description='null', id=400, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("modelConfigClassName=")
			builder.Append(modelConfigClassName())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='modelConfigJson', description='null', id=401, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("modelConfigJson=")
			builder.Append(modelConfigJson())

			limit(originalLimit)

			Return builder
		End Function
	End Class

End Namespace