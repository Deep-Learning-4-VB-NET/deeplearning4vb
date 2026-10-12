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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.StaticInfoEncoder"}) @SuppressWarnings("all") public class StaticInfoEncoder
	Public Class StaticInfoEncoder
		Public Const BLOCK_LENGTH As Integer = 40
		Public Const TEMPLATE_ID As Integer = 1
		Public Const SCHEMA_ID As Integer = 1
		Public Const SCHEMA_VERSION As Integer = 0

		Private ReadOnly parentMessage As StaticInfoEncoder = Me
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
'ORIGINAL LINE: public StaticInfoEncoder wrap(final org.agrona.MutableDirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As MutableDirectBuffer, ByVal offset As Integer) As StaticInfoEncoder
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

		Public Shared Function timeNullValue() As Long
			Return -9223372036854775808L
		End Function

		Public Shared Function timeMinValue() As Long
			Return -9223372036854775807L
		End Function

		Public Shared Function timeMaxValue() As Long
			Return 9223372036854775807L
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder time(final long value)
		Public Overridable Function time(ByVal value As Long) As StaticInfoEncoder
			buffer.putLong(offset_Conflict + 0, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


'JAVA TO VB CONVERTER NOTE: The field fieldsPresent was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly fieldsPresent_Conflict As New InitFieldsPresentEncoder()

		Public Overridable Function fieldsPresent() As InitFieldsPresentEncoder
			fieldsPresent_Conflict.wrap(buffer, offset_Conflict + 8)
			Return fieldsPresent_Conflict
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder hwJvmProcessors(final int value)
		Public Overridable Function hwJvmProcessors(ByVal value As Integer) As StaticInfoEncoder
			buffer.putShort(offset_Conflict + 9, CShort(value), java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder hwNumDevices(final short value)
		Public Overridable Function hwNumDevices(ByVal value As Short) As StaticInfoEncoder
			buffer.putByte(offset_Conflict + 11, CSByte(value))
			Return Me
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder hwJvmMaxMemory(final long value)
		Public Overridable Function hwJvmMaxMemory(ByVal value As Long) As StaticInfoEncoder
			buffer.putLong(offset_Conflict + 12, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder hwOffheapMaxMemory(final long value)
		Public Overridable Function hwOffheapMaxMemory(ByVal value As Long) As StaticInfoEncoder
			buffer.putLong(offset_Conflict + 20, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder modelNumLayers(final int value)
		Public Overridable Function modelNumLayers(ByVal value As Integer) As StaticInfoEncoder
			buffer.putInt(offset_Conflict + 28, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder modelNumParams(final long value)
		Public Overridable Function modelNumParams(ByVal value As Long) As StaticInfoEncoder
			buffer.putLong(offset_Conflict + 32, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Private ReadOnly hwDeviceInfoGroup As New HwDeviceInfoGroupEncoder()

		Public Shared Function hwDeviceInfoGroupId() As Long
			Return 9
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HwDeviceInfoGroupEncoder hwDeviceInfoGroupCount(final int count)
		Public Overridable Function hwDeviceInfoGroupCount(ByVal count As Integer) As HwDeviceInfoGroupEncoder
			hwDeviceInfoGroup.wrap(parentMessage, buffer, count)
			Return hwDeviceInfoGroup
		End Function

		Public Class HwDeviceInfoGroupEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As StaticInfoEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StaticInfoEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As StaticInfoEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
				If count < 0 OrElse count > 65534 Then
					Throw New System.ArgumentException("count outside allowed range: count=" & count)
				End If

				Me.parentMessage = parentMessage
				Me.buffer = buffer
				actingVersion = SCHEMA_VERSION
				dimensions.wrap(buffer, parentMessage.limit())
				dimensions.blockLength(CInt(8))
				dimensions.numInGroup(CInt(count))
				index = -1
				Me.count = count
				blockLength = 8
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 8
			End Function

			Public Overridable Function [next]() As HwDeviceInfoGroupEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HwDeviceInfoGroupEncoder deviceMemoryMax(final long value)
			Public Overridable Function deviceMemoryMax(ByVal value As Long) As HwDeviceInfoGroupEncoder
				buffer.putLong(offset + 0, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HwDeviceInfoGroupEncoder putDeviceDescription(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
			Public Overridable Function putDeviceDescription(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As HwDeviceInfoGroupEncoder
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
'ORIGINAL LINE: public HwDeviceInfoGroupEncoder putDeviceDescription(final byte[] src, final int srcOffset, final int length)
			Public Overridable Function putDeviceDescription(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As HwDeviceInfoGroupEncoder
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
'ORIGINAL LINE: public HwDeviceInfoGroupEncoder deviceDescription(final String value)
			Public Overridable Function deviceDescription(ByVal value As String) As HwDeviceInfoGroupEncoder
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
		End Class

		Private ReadOnly swEnvironmentInfo As New SwEnvironmentInfoEncoder()

		Public Shared Function swEnvironmentInfoId() As Long
			Return 12
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public SwEnvironmentInfoEncoder swEnvironmentInfoCount(final int count)
		Public Overridable Function swEnvironmentInfoCount(ByVal count As Integer) As SwEnvironmentInfoEncoder
			swEnvironmentInfo.wrap(parentMessage, buffer, count)
			Return swEnvironmentInfo
		End Function

		Public Class SwEnvironmentInfoEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As StaticInfoEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StaticInfoEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As StaticInfoEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
				If count < 0 OrElse count > 65534 Then
					Throw New System.ArgumentException("count outside allowed range: count=" & count)
				End If

				Me.parentMessage = parentMessage
				Me.buffer = buffer
				actingVersion = SCHEMA_VERSION
				dimensions.wrap(buffer, parentMessage.limit())
				dimensions.blockLength(CInt(0))
				dimensions.numInGroup(CInt(count))
				index = -1
				Me.count = count
				blockLength = 0
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 0
			End Function

			Public Overridable Function [next]() As SwEnvironmentInfoEncoder
				If index + 1 >= count Then
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public SwEnvironmentInfoEncoder putEnvKey(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
			Public Overridable Function putEnvKey(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As SwEnvironmentInfoEncoder
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
'ORIGINAL LINE: public SwEnvironmentInfoEncoder putEnvKey(final byte[] src, final int srcOffset, final int length)
			Public Overridable Function putEnvKey(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As SwEnvironmentInfoEncoder
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
'ORIGINAL LINE: public SwEnvironmentInfoEncoder envKey(final String value)
			Public Overridable Function envKey(ByVal value As String) As SwEnvironmentInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public SwEnvironmentInfoEncoder putEnvValue(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
			Public Overridable Function putEnvValue(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As SwEnvironmentInfoEncoder
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
'ORIGINAL LINE: public SwEnvironmentInfoEncoder putEnvValue(final byte[] src, final int srcOffset, final int length)
			Public Overridable Function putEnvValue(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As SwEnvironmentInfoEncoder
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
'ORIGINAL LINE: public SwEnvironmentInfoEncoder envValue(final String value)
			Public Overridable Function envValue(ByVal value As String) As SwEnvironmentInfoEncoder
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
		End Class

		Private ReadOnly modelParamNames As New ModelParamNamesEncoder()

		Public Shared Function modelParamNamesId() As Long
			Return 11
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public ModelParamNamesEncoder modelParamNamesCount(final int count)
		Public Overridable Function modelParamNamesCount(ByVal count As Integer) As ModelParamNamesEncoder
			modelParamNames.wrap(parentMessage, buffer, count)
			Return modelParamNames
		End Function

		Public Class ModelParamNamesEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As StaticInfoEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final StaticInfoEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As StaticInfoEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
				If count < 0 OrElse count > 65534 Then
					Throw New System.ArgumentException("count outside allowed range: count=" & count)
				End If

				Me.parentMessage = parentMessage
				Me.buffer = buffer
				actingVersion = SCHEMA_VERSION
				dimensions.wrap(buffer, parentMessage.limit())
				dimensions.blockLength(CInt(0))
				dimensions.numInGroup(CInt(count))
				index = -1
				Me.count = count
				blockLength = 0
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 0
			End Function

			Public Overridable Function [next]() As ModelParamNamesEncoder
				If index + 1 >= count Then
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public ModelParamNamesEncoder putModelParamNames(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
			Public Overridable Function putModelParamNames(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As ModelParamNamesEncoder
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
'ORIGINAL LINE: public ModelParamNamesEncoder putModelParamNames(final byte[] src, final int srcOffset, final int length)
			Public Overridable Function putModelParamNames(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As ModelParamNamesEncoder
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
'ORIGINAL LINE: public ModelParamNamesEncoder modelParamNames(final String value)
			Public Overridable Function modelParamNames(ByVal value As String) As ModelParamNamesEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSessionID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSessionID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSessionID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSessionID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder sessionID(final String value)
		Public Overridable Function sessionID(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putTypeID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putTypeID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putTypeID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putTypeID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder typeID(final String value)
		Public Overridable Function typeID(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putWorkerID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putWorkerID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putWorkerID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putWorkerID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder workerID(final String value)
		Public Overridable Function workerID(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwArch(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwArch(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwArch(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwArch(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swArch(final String value)
		Public Overridable Function swArch(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwOsName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwOsName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwOsName(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwOsName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swOsName(final String value)
		Public Overridable Function swOsName(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmName(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swJvmName(final String value)
		Public Overridable Function swJvmName(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmVersion(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmVersion(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmVersion(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmVersion(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swJvmVersion(final String value)
		Public Overridable Function swJvmVersion(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmSpecVersion(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmSpecVersion(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmSpecVersion(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmSpecVersion(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swJvmSpecVersion(final String value)
		Public Overridable Function swJvmSpecVersion(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwNd4jBackendClass(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwNd4jBackendClass(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwNd4jBackendClass(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwNd4jBackendClass(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swNd4jBackendClass(final String value)
		Public Overridable Function swNd4jBackendClass(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwNd4jDataTypeName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwNd4jDataTypeName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwNd4jDataTypeName(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwNd4jDataTypeName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swNd4jDataTypeName(final String value)
		Public Overridable Function swNd4jDataTypeName(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwHostName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwHostName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwHostName(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwHostName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swHostName(final String value)
		Public Overridable Function swHostName(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmUID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmUID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putSwJvmUID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSwJvmUID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder swJvmUID(final String value)
		Public Overridable Function swJvmUID(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putHwHardwareUID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putHwHardwareUID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putHwHardwareUID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putHwHardwareUID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder hwHardwareUID(final String value)
		Public Overridable Function hwHardwareUID(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putModelConfigClassName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putModelConfigClassName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putModelConfigClassName(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putModelConfigClassName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder modelConfigClassName(final String value)
		Public Overridable Function modelConfigClassName(ByVal value As String) As StaticInfoEncoder
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

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StaticInfoEncoder putModelConfigJson(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putModelConfigJson(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder putModelConfigJson(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putModelConfigJson(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As StaticInfoEncoder
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
'ORIGINAL LINE: public StaticInfoEncoder modelConfigJson(final String value)
		Public Overridable Function modelConfigJson(ByVal value As String) As StaticInfoEncoder
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
			Dim writer As New StaticInfoDecoder()
			writer.wrap(buffer, offset_Conflict, BLOCK_LENGTH, SCHEMA_VERSION)

			Return writer.appendTo(builder)
		End Function
	End Class

End Namespace