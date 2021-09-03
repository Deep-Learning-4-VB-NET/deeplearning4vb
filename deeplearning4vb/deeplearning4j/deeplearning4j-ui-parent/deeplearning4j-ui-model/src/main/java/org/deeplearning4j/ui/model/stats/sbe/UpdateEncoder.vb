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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.UpdateEncoder"}) @SuppressWarnings("all") public class UpdateEncoder
	Public Class UpdateEncoder
		Public Const BLOCK_LENGTH As Integer = 32
		Public Const TEMPLATE_ID As Integer = 2
		Public Const SCHEMA_ID As Integer = 1
		Public Const SCHEMA_VERSION As Integer = 0

		Private ReadOnly parentMessage As UpdateEncoder = Me
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
'ORIGINAL LINE: public UpdateEncoder wrap(final org.agrona.MutableDirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As MutableDirectBuffer, ByVal offset As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder time(final long value)
		Public Overridable Function time(ByVal value As Long) As UpdateEncoder
			buffer.putLong(offset_Conflict + 0, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Public Shared Function deltaTimeNullValue() As Integer
			Return -2147483648
		End Function

		Public Shared Function deltaTimeMinValue() As Integer
			Return -2147483647
		End Function

		Public Shared Function deltaTimeMaxValue() As Integer
			Return 2147483647
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateEncoder deltaTime(final int value)
		Public Overridable Function deltaTime(ByVal value As Integer) As UpdateEncoder
			buffer.putInt(offset_Conflict + 8, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Public Shared Function iterationCountNullValue() As Integer
			Return -2147483648
		End Function

		Public Shared Function iterationCountMinValue() As Integer
			Return -2147483647
		End Function

		Public Shared Function iterationCountMaxValue() As Integer
			Return 2147483647
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateEncoder iterationCount(final int value)
		Public Overridable Function iterationCount(ByVal value As Integer) As UpdateEncoder
			buffer.putInt(offset_Conflict + 12, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


'JAVA TO VB CONVERTER NOTE: The field fieldsPresent was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly fieldsPresent_Conflict As New UpdateFieldsPresentEncoder()

		Public Overridable Function fieldsPresent() As UpdateFieldsPresentEncoder
			fieldsPresent_Conflict.wrap(buffer, offset_Conflict + 16)
			Return fieldsPresent_Conflict
		End Function

		Public Shared Function statsCollectionDurationNullValue() As Integer
			Return -2147483648
		End Function

		Public Shared Function statsCollectionDurationMinValue() As Integer
			Return -2147483647
		End Function

		Public Shared Function statsCollectionDurationMaxValue() As Integer
			Return 2147483647
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateEncoder statsCollectionDuration(final int value)
		Public Overridable Function statsCollectionDuration(ByVal value As Integer) As UpdateEncoder
			buffer.putInt(offset_Conflict + 20, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Public Shared Function scoreNullValue() As Double
			Return Double.NaN
		End Function

		Public Shared Function scoreMinValue() As Double
			Return 4.9E-324R
		End Function

		Public Shared Function scoreMaxValue() As Double
			Return 1.7976931348623157E308R
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateEncoder score(final double value)
		Public Overridable Function score(ByVal value As Double) As UpdateEncoder
			buffer.putDouble(offset_Conflict + 24, value, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Private ReadOnly memoryUse As New MemoryUseEncoder()

		Public Shared Function memoryUseId() As Long
			Return 100
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MemoryUseEncoder memoryUseCount(final int count)
		Public Overridable Function memoryUseCount(ByVal count As Integer) As MemoryUseEncoder
			memoryUse.wrap(parentMessage, buffer, count)
			Return memoryUse
		End Function

		Public Class MemoryUseEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As UpdateEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
				If count < 0 OrElse count > 65534 Then
					Throw New System.ArgumentException("count outside allowed range: count=" & count)
				End If

				Me.parentMessage = parentMessage
				Me.buffer = buffer
				actingVersion = SCHEMA_VERSION
				dimensions.wrap(buffer, parentMessage.limit())
				dimensions.blockLength(CInt(9))
				dimensions.numInGroup(CInt(count))
				index = -1
				Me.count = count
				blockLength = 9
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 9
			End Function

			Public Overridable Function [next]() As MemoryUseEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MemoryUseEncoder memoryType(final MemoryType value)
			Public Overridable Function memoryType(ByVal value As MemoryType) As MemoryUseEncoder
				buffer.putByte(offset + 0, CSByte(Math.Truncate(value.value())))
				Return Me
			End Function

			Public Shared Function memoryBytesNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function memoryBytesMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function memoryBytesMaxValue() As Long
				Return 9223372036854775807L
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MemoryUseEncoder memoryBytes(final long value)
			Public Overridable Function memoryBytes(ByVal value As Long) As MemoryUseEncoder
				buffer.putLong(offset + 1, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function

		End Class

		Private ReadOnly performance As New PerformanceEncoder()

		Public Shared Function performanceId() As Long
			Return 200
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerformanceEncoder performanceCount(final int count)
		Public Overridable Function performanceCount(ByVal count As Integer) As PerformanceEncoder
			performance.wrap(parentMessage, buffer, count)
			Return performance
		End Function

		Public Class PerformanceEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As UpdateEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
				If count < 0 OrElse count > 65534 Then
					Throw New System.ArgumentException("count outside allowed range: count=" & count)
				End If

				Me.parentMessage = parentMessage
				Me.buffer = buffer
				actingVersion = SCHEMA_VERSION
				dimensions.wrap(buffer, parentMessage.limit())
				dimensions.blockLength(CInt(32))
				dimensions.numInGroup(CInt(count))
				index = -1
				Me.count = count
				blockLength = 32
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 32
			End Function

			Public Overridable Function [next]() As PerformanceEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function totalRuntimeMsNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function totalRuntimeMsMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function totalRuntimeMsMaxValue() As Long
				Return 9223372036854775807L
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerformanceEncoder totalRuntimeMs(final long value)
			Public Overridable Function totalRuntimeMs(ByVal value As Long) As PerformanceEncoder
				buffer.putLong(offset + 0, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function


			Public Shared Function totalExamplesNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function totalExamplesMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function totalExamplesMaxValue() As Long
				Return 9223372036854775807L
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerformanceEncoder totalExamples(final long value)
			Public Overridable Function totalExamples(ByVal value As Long) As PerformanceEncoder
				buffer.putLong(offset + 8, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function


			Public Shared Function totalMinibatchesNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function totalMinibatchesMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function totalMinibatchesMaxValue() As Long
				Return 9223372036854775807L
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerformanceEncoder totalMinibatches(final long value)
			Public Overridable Function totalMinibatches(ByVal value As Long) As PerformanceEncoder
				buffer.putLong(offset + 16, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function


			Public Shared Function examplesPerSecondNullValue() As Single
				Return Float.NaN
			End Function

			Public Shared Function examplesPerSecondMinValue() As Single
				Return 1.401298464324817E-45f
			End Function

			Public Shared Function examplesPerSecondMaxValue() As Single
				Return 3.4028234663852886E38f
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerformanceEncoder examplesPerSecond(final float value)
			Public Overridable Function examplesPerSecond(ByVal value As Single) As PerformanceEncoder
				buffer.putFloat(offset + 24, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function


			Public Shared Function minibatchesPerSecondNullValue() As Single
				Return Float.NaN
			End Function

			Public Shared Function minibatchesPerSecondMinValue() As Single
				Return 1.401298464324817E-45f
			End Function

			Public Shared Function minibatchesPerSecondMaxValue() As Single
				Return 3.4028234663852886E38f
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerformanceEncoder minibatchesPerSecond(final float value)
			Public Overridable Function minibatchesPerSecond(ByVal value As Single) As PerformanceEncoder
				buffer.putFloat(offset + 28, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function

		End Class

		Private ReadOnly gcStats As New GcStatsEncoder()

		Public Shared Function gcStatsId() As Long
			Return 300
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public GcStatsEncoder gcStatsCount(final int count)
		Public Overridable Function gcStatsCount(ByVal count As Integer) As GcStatsEncoder
			gcStats.wrap(parentMessage, buffer, count)
			Return gcStats
		End Function

		Public Class GcStatsEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As UpdateEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
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

			Public Overridable Function [next]() As GcStatsEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function deltaGCCountNullValue() As Integer
				Return -2147483648
			End Function

			Public Shared Function deltaGCCountMinValue() As Integer
				Return -2147483647
			End Function

			Public Shared Function deltaGCCountMaxValue() As Integer
				Return 2147483647
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public GcStatsEncoder deltaGCCount(final int value)
			Public Overridable Function deltaGCCount(ByVal value As Integer) As GcStatsEncoder
				buffer.putInt(offset + 0, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function


			Public Shared Function deltaGCTimeMsNullValue() As Integer
				Return -2147483648
			End Function

			Public Shared Function deltaGCTimeMsMinValue() As Integer
				Return -2147483647
			End Function

			Public Shared Function deltaGCTimeMsMaxValue() As Integer
				Return 2147483647
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public GcStatsEncoder deltaGCTimeMs(final int value)
			Public Overridable Function deltaGCTimeMs(ByVal value As Integer) As GcStatsEncoder
				buffer.putInt(offset + 4, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function


			Public Shared Function gcNameId() As Integer
				Return 1000
			End Function

			Public Shared Function gcNameCharacterEncoding() As String
				Return "UTF-8"
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String gcNameMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function gcNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function gcNameHeaderLength() As Integer
				Return 4
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public GcStatsEncoder putGcName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
			Public Overridable Function putGcName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As GcStatsEncoder
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
'ORIGINAL LINE: public GcStatsEncoder putGcName(final byte[] src, final int srcOffset, final int length)
			Public Overridable Function putGcName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As GcStatsEncoder
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
'ORIGINAL LINE: public GcStatsEncoder gcName(final String value)
			Public Overridable Function gcName(ByVal value As String) As GcStatsEncoder
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

		Private ReadOnly paramNames As New ParamNamesEncoder()

		Public Shared Function paramNamesId() As Long
			Return 350
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public ParamNamesEncoder paramNamesCount(final int count)
		Public Overridable Function paramNamesCount(ByVal count As Integer) As ParamNamesEncoder
			paramNames.wrap(parentMessage, buffer, count)
			Return paramNames
		End Function

		Public Class ParamNamesEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As UpdateEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
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

			Public Overridable Function [next]() As ParamNamesEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function paramNameId() As Integer
				Return 1100
			End Function

			Public Shared Function paramNameCharacterEncoding() As String
				Return "UTF-8"
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String paramNameMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function paramNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function paramNameHeaderLength() As Integer
				Return 4
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public ParamNamesEncoder putParamName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
			Public Overridable Function putParamName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As ParamNamesEncoder
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
'ORIGINAL LINE: public ParamNamesEncoder putParamName(final byte[] src, final int srcOffset, final int length)
			Public Overridable Function putParamName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As ParamNamesEncoder
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
'ORIGINAL LINE: public ParamNamesEncoder paramName(final String value)
			Public Overridable Function paramName(ByVal value As String) As ParamNamesEncoder
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

		Private ReadOnly layerNames As New LayerNamesEncoder()

		Public Shared Function layerNamesId() As Long
			Return 351
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public LayerNamesEncoder layerNamesCount(final int count)
		Public Overridable Function layerNamesCount(ByVal count As Integer) As LayerNamesEncoder
			layerNames.wrap(parentMessage, buffer, count)
			Return layerNames
		End Function

		Public Class LayerNamesEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As UpdateEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
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

			Public Overridable Function [next]() As LayerNamesEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function layerNameId() As Integer
				Return 1101
			End Function

			Public Shared Function layerNameCharacterEncoding() As String
				Return "UTF-8"
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String layerNameMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function layerNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function layerNameHeaderLength() As Integer
				Return 4
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public LayerNamesEncoder putLayerName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
			Public Overridable Function putLayerName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As LayerNamesEncoder
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
'ORIGINAL LINE: public LayerNamesEncoder putLayerName(final byte[] src, final int srcOffset, final int length)
			Public Overridable Function putLayerName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As LayerNamesEncoder
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
'ORIGINAL LINE: public LayerNamesEncoder layerName(final String value)
			Public Overridable Function layerName(ByVal value As String) As LayerNamesEncoder
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

		Private ReadOnly perParameterStats As New PerParameterStatsEncoder()

		Public Shared Function perParameterStatsId() As Long
			Return 400
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerParameterStatsEncoder perParameterStatsCount(final int count)
		Public Overridable Function perParameterStatsCount(ByVal count As Integer) As PerParameterStatsEncoder
			perParameterStats.wrap(parentMessage, buffer, count)
			Return perParameterStats
		End Function

		Public Class PerParameterStatsEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As UpdateEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
				If count < 0 OrElse count > 65534 Then
					Throw New System.ArgumentException("count outside allowed range: count=" & count)
				End If

				Me.parentMessage = parentMessage
				Me.buffer = buffer
				actingVersion = SCHEMA_VERSION
				dimensions.wrap(buffer, parentMessage.limit())
				dimensions.blockLength(CInt(4))
				dimensions.numInGroup(CInt(count))
				index = -1
				Me.count = count
				blockLength = 4
				parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
			End Sub

			Public Shared Function sbeHeaderSize() As Integer
				Return HEADER_SIZE
			End Function

			Public Shared Function sbeBlockLength() As Integer
				Return 4
			End Function

			Public Overridable Function [next]() As PerParameterStatsEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function learningRateNullValue() As Single
				Return Float.NaN
			End Function

			Public Shared Function learningRateMinValue() As Single
				Return 1.401298464324817E-45f
			End Function

			Public Shared Function learningRateMaxValue() As Single
				Return 3.4028234663852886E38f
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public PerParameterStatsEncoder learningRate(final float value)
			Public Overridable Function learningRate(ByVal value As Single) As PerParameterStatsEncoder
				buffer.putFloat(offset + 0, value, java.nio.ByteOrder.LITTLE_ENDIAN)
				Return Me
			End Function


			Friend ReadOnly summaryStat As New SummaryStatEncoder()

			Public Shared Function summaryStatId() As Long
				Return 402
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public SummaryStatEncoder summaryStatCount(final int count)
			Public Overridable Function summaryStatCount(ByVal count As Integer) As SummaryStatEncoder
				summaryStat.wrap(parentMessage, buffer, count)
				Return summaryStat
			End Function

			Public Class SummaryStatEncoder
				Friend Const HEADER_SIZE As Integer = 4
				Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
				Friend parentMessage As UpdateEncoder
				Friend buffer As MutableDirectBuffer
				Friend blockLength As Integer
				Friend actingVersion As Integer
				Friend count As Integer
				Friend index As Integer
				Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
				Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
					If count < 0 OrElse count > 65534 Then
						Throw New System.ArgumentException("count outside allowed range: count=" & count)
					End If

					Me.parentMessage = parentMessage
					Me.buffer = buffer
					actingVersion = SCHEMA_VERSION
					dimensions.wrap(buffer, parentMessage.limit())
					dimensions.blockLength(CInt(10))
					dimensions.numInGroup(CInt(count))
					index = -1
					Me.count = count
					blockLength = 10
					parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
				End Sub

				Public Shared Function sbeHeaderSize() As Integer
					Return HEADER_SIZE
				End Function

				Public Shared Function sbeBlockLength() As Integer
					Return 10
				End Function

				Public Overridable Function [next]() As SummaryStatEncoder
					If index + 1 >= count Then
						Throw New java.util.NoSuchElementException()
					End If

					offset = parentMessage.limit()
					parentMessage.limit(offset + blockLength)
					index += 1

					Return Me
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public SummaryStatEncoder statType(final StatsType value)
				Public Overridable Function statType(ByVal value As StatsType) As SummaryStatEncoder
					buffer.putByte(offset + 0, CSByte(Math.Truncate(value.value())))
					Return Me
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public SummaryStatEncoder summaryType(final SummaryType value)
				Public Overridable Function summaryType(ByVal value As SummaryType) As SummaryStatEncoder
					buffer.putByte(offset + 1, CSByte(Math.Truncate(value.value())))
					Return Me
				End Function

				Public Shared Function valueNullValue() As Double
					Return Double.NaN
				End Function

				Public Shared Function valueMinValue() As Double
					Return 4.9E-324R
				End Function

				Public Shared Function valueMaxValue() As Double
					Return 1.7976931348623157E308R
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public SummaryStatEncoder value(final double value)
'JAVA TO VB CONVERTER NOTE: The parameter value was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
				Public Overridable Function value(ByVal value_Conflict As Double) As SummaryStatEncoder
					buffer.putDouble(offset + 2, value_Conflict, java.nio.ByteOrder.LITTLE_ENDIAN)
					Return Me
				End Function

			End Class

			Friend ReadOnly histograms As New HistogramsEncoder()

			Public Shared Function histogramsId() As Long
				Return 406
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HistogramsEncoder histogramsCount(final int count)
			Public Overridable Function histogramsCount(ByVal count As Integer) As HistogramsEncoder
				histograms.wrap(parentMessage, buffer, count)
				Return histograms
			End Function

			Public Class HistogramsEncoder
				Friend Const HEADER_SIZE As Integer = 4
				Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
				Friend parentMessage As UpdateEncoder
				Friend buffer As MutableDirectBuffer
				Friend blockLength As Integer
				Friend actingVersion As Integer
				Friend count As Integer
				Friend index As Integer
				Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
				Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
					If count < 0 OrElse count > 65534 Then
						Throw New System.ArgumentException("count outside allowed range: count=" & count)
					End If

					Me.parentMessage = parentMessage
					Me.buffer = buffer
					actingVersion = SCHEMA_VERSION
					dimensions.wrap(buffer, parentMessage.limit())
					dimensions.blockLength(CInt(21))
					dimensions.numInGroup(CInt(count))
					index = -1
					Me.count = count
					blockLength = 21
					parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
				End Sub

				Public Shared Function sbeHeaderSize() As Integer
					Return HEADER_SIZE
				End Function

				Public Shared Function sbeBlockLength() As Integer
					Return 21
				End Function

				Public Overridable Function [next]() As HistogramsEncoder
					If index + 1 >= count Then
						Throw New java.util.NoSuchElementException()
					End If

					offset = parentMessage.limit()
					parentMessage.limit(offset + blockLength)
					index += 1

					Return Me
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HistogramsEncoder statType(final StatsType value)
				Public Overridable Function statType(ByVal value As StatsType) As HistogramsEncoder
					buffer.putByte(offset + 0, CSByte(Math.Truncate(value.value())))
					Return Me
				End Function

				Public Shared Function minValueNullValue() As Double
					Return Double.NaN
				End Function

				Public Shared Function minValueMinValue() As Double
					Return 4.9E-324R
				End Function

				Public Shared Function minValueMaxValue() As Double
					Return 1.7976931348623157E308R
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HistogramsEncoder minValue(final double value)
				Public Overridable Function minValue(ByVal value As Double) As HistogramsEncoder
					buffer.putDouble(offset + 1, value, java.nio.ByteOrder.LITTLE_ENDIAN)
					Return Me
				End Function


				Public Shared Function maxValueNullValue() As Double
					Return Double.NaN
				End Function

				Public Shared Function maxValueMinValue() As Double
					Return 4.9E-324R
				End Function

				Public Shared Function maxValueMaxValue() As Double
					Return 1.7976931348623157E308R
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HistogramsEncoder maxValue(final double value)
				Public Overridable Function maxValue(ByVal value As Double) As HistogramsEncoder
					buffer.putDouble(offset + 9, value, java.nio.ByteOrder.LITTLE_ENDIAN)
					Return Me
				End Function


				Public Shared Function nBinsNullValue() As Integer
					Return -2147483648
				End Function

				Public Shared Function nBinsMinValue() As Integer
					Return -2147483647
				End Function

				Public Shared Function nBinsMaxValue() As Integer
					Return 2147483647
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HistogramsEncoder nBins(final int value)
				Public Overridable Function nBins(ByVal value As Integer) As HistogramsEncoder
					buffer.putInt(offset + 17, value, java.nio.ByteOrder.LITTLE_ENDIAN)
					Return Me
				End Function


				Friend ReadOnly histogramCounts As New HistogramCountsEncoder()

				Public Shared Function histogramCountsId() As Long
					Return 411
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HistogramCountsEncoder histogramCountsCount(final int count)
				Public Overridable Function histogramCountsCount(ByVal count As Integer) As HistogramCountsEncoder
					histogramCounts.wrap(parentMessage, buffer, count)
					Return histogramCounts
				End Function

				Public Class HistogramCountsEncoder
					Friend Const HEADER_SIZE As Integer = 4
					Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
					Friend parentMessage As UpdateEncoder
					Friend buffer As MutableDirectBuffer
					Friend blockLength As Integer
					Friend actingVersion As Integer
					Friend count As Integer
					Friend index As Integer
					Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
					Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
						If count < 0 OrElse count > 65534 Then
							Throw New System.ArgumentException("count outside allowed range: count=" & count)
						End If

						Me.parentMessage = parentMessage
						Me.buffer = buffer
						actingVersion = SCHEMA_VERSION
						dimensions.wrap(buffer, parentMessage.limit())
						dimensions.blockLength(CInt(4))
						dimensions.numInGroup(CInt(count))
						index = -1
						Me.count = count
						blockLength = 4
						parentMessage.limit(parentMessage.limit() + HEADER_SIZE)
					End Sub

					Public Shared Function sbeHeaderSize() As Integer
						Return HEADER_SIZE
					End Function

					Public Shared Function sbeBlockLength() As Integer
						Return 4
					End Function

					Public Overridable Function [next]() As HistogramCountsEncoder
						If index + 1 >= count Then
							Throw New java.util.NoSuchElementException()
						End If

						offset = parentMessage.limit()
						parentMessage.limit(offset + blockLength)
						index += 1

						Return Me
					End Function

					Public Shared Function binCountNullValue() As Long
						Return 4294967294L
					End Function

					Public Shared Function binCountMinValue() As Long
						Return 0L
					End Function

					Public Shared Function binCountMaxValue() As Long
						Return 4294967293L
					End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public HistogramCountsEncoder binCount(final long value)
					Public Overridable Function binCount(ByVal value As Long) As HistogramCountsEncoder
						buffer.putInt(offset + 0, CInt(value), java.nio.ByteOrder.LITTLE_ENDIAN)
						Return Me
					End Function

				End Class
			End Class
		End Class

		Private ReadOnly dataSetMetaDataBytes As New DataSetMetaDataBytesEncoder()

		Public Shared Function dataSetMetaDataBytesId() As Long
			Return 500
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public DataSetMetaDataBytesEncoder dataSetMetaDataBytesCount(final int count)
		Public Overridable Function dataSetMetaDataBytesCount(ByVal count As Integer) As DataSetMetaDataBytesEncoder
			dataSetMetaDataBytes.wrap(parentMessage, buffer, count)
			Return dataSetMetaDataBytes
		End Function

		Public Class DataSetMetaDataBytesEncoder
			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
			Friend parentMessage As UpdateEncoder
			Friend buffer As MutableDirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
			Friend count As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
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

			Public Overridable Function [next]() As DataSetMetaDataBytesEncoder
				If index + 1 >= count Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Friend ReadOnly metaDataBytes As New MetaDataBytesEncoder()

			Public Shared Function metaDataBytesId() As Long
				Return 501
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MetaDataBytesEncoder metaDataBytesCount(final int count)
			Public Overridable Function metaDataBytesCount(ByVal count As Integer) As MetaDataBytesEncoder
				metaDataBytes.wrap(parentMessage, buffer, count)
				Return metaDataBytes
			End Function

			Public Class MetaDataBytesEncoder
				Friend Const HEADER_SIZE As Integer = 4
				Friend ReadOnly dimensions As New GroupSizeEncodingEncoder()
				Friend parentMessage As UpdateEncoder
				Friend buffer As MutableDirectBuffer
				Friend blockLength As Integer
				Friend actingVersion As Integer
				Friend count As Integer
				Friend index As Integer
				Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateEncoder parentMessage, final org.agrona.MutableDirectBuffer buffer, final int count)
				Public Overridable Sub wrap(ByVal parentMessage As UpdateEncoder, ByVal buffer As MutableDirectBuffer, ByVal count As Integer)
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

				Public Overridable Function [next]() As MetaDataBytesEncoder
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
'ORIGINAL LINE: public MetaDataBytesEncoder bytes(final byte value)
				Public Overridable Function bytes(ByVal value As SByte) As MetaDataBytesEncoder
					buffer.putByte(offset + 0, value)
					Return Me
				End Function

			End Class
		End Class

		Public Shared Function sessionIDId() As Integer
			Return 1200
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
'ORIGINAL LINE: public UpdateEncoder putSessionID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putSessionID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder putSessionID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putSessionID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder sessionID(final String value)
		Public Overridable Function sessionID(ByVal value As String) As UpdateEncoder
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
			Return 1201
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
'ORIGINAL LINE: public UpdateEncoder putTypeID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putTypeID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder putTypeID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putTypeID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder typeID(final String value)
		Public Overridable Function typeID(ByVal value As String) As UpdateEncoder
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
			Return 1202
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
'ORIGINAL LINE: public UpdateEncoder putWorkerID(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putWorkerID(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder putWorkerID(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putWorkerID(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder workerID(final String value)
		Public Overridable Function workerID(ByVal value As String) As UpdateEncoder
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

		Public Shared Function dataSetMetaDataClassNameId() As Integer
			Return 1300
		End Function

		Public Shared Function dataSetMetaDataClassNameCharacterEncoding() As String
			Return "UTF-8"
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String dataSetMetaDataClassNameMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function dataSetMetaDataClassNameMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function dataSetMetaDataClassNameHeaderLength() As Integer
			Return 4
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateEncoder putDataSetMetaDataClassName(final org.agrona.DirectBuffer src, final int srcOffset, final int length)
		Public Overridable Function putDataSetMetaDataClassName(ByVal src As DirectBuffer, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder putDataSetMetaDataClassName(final byte[] src, final int srcOffset, final int length)
		Public Overridable Function putDataSetMetaDataClassName(ByVal src() As SByte, ByVal srcOffset As Integer, ByVal length As Integer) As UpdateEncoder
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
'ORIGINAL LINE: public UpdateEncoder dataSetMetaDataClassName(final String value)
		Public Overridable Function dataSetMetaDataClassName(ByVal value As String) As UpdateEncoder
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
			Dim writer As New UpdateDecoder()
			writer.wrap(buffer, offset_Conflict, BLOCK_LENGTH, SCHEMA_VERSION)

			Return writer.appendTo(builder)
		End Function
	End Class

End Namespace