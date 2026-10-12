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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.UpdateDecoder"}) @SuppressWarnings("all") public class UpdateDecoder
	Public Class UpdateDecoder
		Public Const BLOCK_LENGTH As Integer = 32
		Public Const TEMPLATE_ID As Integer = 2
		Public Const SCHEMA_ID As Integer = 1
		Public Const SCHEMA_VERSION As Integer = 0

		Private ReadOnly parentMessage As UpdateDecoder = Me
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
'ORIGINAL LINE: public UpdateDecoder wrap(final org.agrona.DirectBuffer buffer, final int offset, final int actingBlockLength, final int actingVersion)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal actingBlockLength As Integer, ByVal actingVersion As Integer) As UpdateDecoder
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


		Public Shared Function deltaTimeId() As Integer
			Return 2
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String deltaTimeMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function deltaTimeMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function deltaTimeNullValue() As Integer
			Return -2147483648
		End Function

		Public Shared Function deltaTimeMinValue() As Integer
			Return -2147483647
		End Function

		Public Shared Function deltaTimeMaxValue() As Integer
			Return 2147483647
		End Function

		Public Overridable Function deltaTime() As Integer
			Return buffer.getInt(offset_Conflict + 8, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


		Public Shared Function iterationCountId() As Integer
			Return 3
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String iterationCountMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function iterationCountMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function iterationCountNullValue() As Integer
			Return -2147483648
		End Function

		Public Shared Function iterationCountMinValue() As Integer
			Return -2147483647
		End Function

		Public Shared Function iterationCountMaxValue() As Integer
			Return 2147483647
		End Function

		Public Overridable Function iterationCount() As Integer
			Return buffer.getInt(offset_Conflict + 12, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


		Public Shared Function fieldsPresentId() As Integer
			Return 4
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
		Private ReadOnly fieldsPresent_Conflict As New UpdateFieldsPresentDecoder()

		Public Overridable Function fieldsPresent() As UpdateFieldsPresentDecoder
			fieldsPresent_Conflict.wrap(buffer, offset_Conflict + 16)
			Return fieldsPresent_Conflict
		End Function

		Public Shared Function statsCollectionDurationId() As Integer
			Return 5
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String statsCollectionDurationMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function statsCollectionDurationMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function statsCollectionDurationNullValue() As Integer
			Return -2147483648
		End Function

		Public Shared Function statsCollectionDurationMinValue() As Integer
			Return -2147483647
		End Function

		Public Shared Function statsCollectionDurationMaxValue() As Integer
			Return 2147483647
		End Function

		Public Overridable Function statsCollectionDuration() As Integer
			Return buffer.getInt(offset_Conflict + 20, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


		Public Shared Function scoreId() As Integer
			Return 6
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String scoreMetaAttribute(final MetaAttribute metaAttribute)
		Public Shared Function scoreMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

		Public Shared Function scoreNullValue() As Double
			Return Double.NaN
		End Function

		Public Shared Function scoreMinValue() As Double
			Return 4.9E-324R
		End Function

		Public Shared Function scoreMaxValue() As Double
			Return 1.7976931348623157E308R
		End Function

		Public Overridable Function score() As Double
			Return buffer.getDouble(offset_Conflict + 24, java.nio.ByteOrder.LITTLE_ENDIAN)
		End Function


'JAVA TO VB CONVERTER NOTE: The field memoryUse was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly memoryUse_Conflict As New MemoryUseDecoder()

		Public Shared Function memoryUseDecoderId() As Long
			Return 100
		End Function

		Public Overridable Function memoryUse() As MemoryUseDecoder
			memoryUse_Conflict.wrap(parentMessage, buffer)
			Return memoryUse_Conflict
		End Function

		Public Class MemoryUseDecoder
			Implements IEnumerable(Of MemoryUseDecoder), IEnumerator(Of MemoryUseDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As UpdateDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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
				Return 9
			End Function

			Public Overridable Function actingBlockLength() As Integer
				Return blockLength
			End Function

			Public Overridable Function count() As Integer
				Return count_Conflict
			End Function

			Public Overridable Function GetEnumerator() As IEnumerator(Of MemoryUseDecoder) Implements IEnumerator(Of MemoryUseDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As MemoryUseDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function memoryTypeId() As Integer
				Return 101
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String memoryTypeMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function memoryTypeMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Overridable Function memoryType() As MemoryType
				Return MemoryType.get((CShort(buffer.getByte(offset + 0) And &HFF)))
			End Function


			Public Shared Function memoryBytesId() As Integer
				Return 102
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String memoryBytesMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function memoryBytesMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function memoryBytesNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function memoryBytesMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function memoryBytesMaxValue() As Long
				Return 9223372036854775807L
			End Function

			Public Overridable Function memoryBytes() As Long
				Return buffer.getLong(offset + 1, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Overrides Function ToString() As String
				Return appendTo(New StringBuilder(100)).ToString()
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
			Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
				builder.Append("("c)
				'Token{signal=BEGIN_FIELD, name='memoryType', description='null', id=101, version=0, encodedLength=0, offset=0, componentTokenCount=10, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=BEGIN_ENUM, name='MemoryType', description='null', id=-1, version=0, encodedLength=1, offset=0, componentTokenCount=8, encoding=Encoding{presence=REQUIRED, primitiveType=UINT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
				builder.Append("memoryType=")
				builder.Append(memoryType())
				builder.Append("|"c)
				'Token{signal=BEGIN_FIELD, name='memoryBytes', description='null', id=102, version=0, encodedLength=0, offset=1, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=1, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("memoryBytes=")
				builder.Append(memoryBytes())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field performance was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly performance_Conflict As New PerformanceDecoder()

		Public Shared Function performanceDecoderId() As Long
			Return 200
		End Function

		Public Overridable Function performance() As PerformanceDecoder
			performance_Conflict.wrap(parentMessage, buffer)
			Return performance_Conflict
		End Function

		Public Class PerformanceDecoder
			Implements IEnumerable(Of PerformanceDecoder), IEnumerator(Of PerformanceDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As UpdateDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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
				Return 32
			End Function

			Public Overridable Function actingBlockLength() As Integer
				Return blockLength
			End Function

			Public Overridable Function count() As Integer
				Return count_Conflict
			End Function

			Public Overridable Function GetEnumerator() As IEnumerator(Of PerformanceDecoder) Implements IEnumerator(Of PerformanceDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As PerformanceDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function totalRuntimeMsId() As Integer
				Return 201
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String totalRuntimeMsMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function totalRuntimeMsMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function totalRuntimeMsNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function totalRuntimeMsMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function totalRuntimeMsMaxValue() As Long
				Return 9223372036854775807L
			End Function

			Public Overridable Function totalRuntimeMs() As Long
				Return buffer.getLong(offset + 0, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Shared Function totalExamplesId() As Integer
				Return 202
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String totalExamplesMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function totalExamplesMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function totalExamplesNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function totalExamplesMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function totalExamplesMaxValue() As Long
				Return 9223372036854775807L
			End Function

			Public Overridable Function totalExamples() As Long
				Return buffer.getLong(offset + 8, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Shared Function totalMinibatchesId() As Integer
				Return 203
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String totalMinibatchesMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function totalMinibatchesMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function totalMinibatchesNullValue() As Long
				Return -9223372036854775808L
			End Function

			Public Shared Function totalMinibatchesMinValue() As Long
				Return -9223372036854775807L
			End Function

			Public Shared Function totalMinibatchesMaxValue() As Long
				Return 9223372036854775807L
			End Function

			Public Overridable Function totalMinibatches() As Long
				Return buffer.getLong(offset + 16, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Shared Function examplesPerSecondId() As Integer
				Return 204
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String examplesPerSecondMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function examplesPerSecondMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function examplesPerSecondNullValue() As Single
				Return Float.NaN
			End Function

			Public Shared Function examplesPerSecondMinValue() As Single
				Return 1.401298464324817E-45f
			End Function

			Public Shared Function examplesPerSecondMaxValue() As Single
				Return 3.4028234663852886E38f
			End Function

			Public Overridable Function examplesPerSecond() As Single
				Return buffer.getFloat(offset + 24, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Shared Function minibatchesPerSecondId() As Integer
				Return 205
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String minibatchesPerSecondMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function minibatchesPerSecondMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function minibatchesPerSecondNullValue() As Single
				Return Float.NaN
			End Function

			Public Shared Function minibatchesPerSecondMinValue() As Single
				Return 1.401298464324817E-45f
			End Function

			Public Shared Function minibatchesPerSecondMaxValue() As Single
				Return 3.4028234663852886E38f
			End Function

			Public Overridable Function minibatchesPerSecond() As Single
				Return buffer.getFloat(offset + 28, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Overrides Function ToString() As String
				Return appendTo(New StringBuilder(100)).ToString()
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
			Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
				builder.Append("("c)
				'Token{signal=BEGIN_FIELD, name='totalRuntimeMs', description='null', id=201, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("totalRuntimeMs=")
				builder.Append(totalRuntimeMs())
				builder.Append("|"c)
				'Token{signal=BEGIN_FIELD, name='totalExamples', description='null', id=202, version=0, encodedLength=0, offset=8, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=8, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("totalExamples=")
				builder.Append(totalExamples())
				builder.Append("|"c)
				'Token{signal=BEGIN_FIELD, name='totalMinibatches', description='null', id=203, version=0, encodedLength=0, offset=16, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int64', description='null', id=-1, version=0, encodedLength=8, offset=16, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT64, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("totalMinibatches=")
				builder.Append(totalMinibatches())
				builder.Append("|"c)
				'Token{signal=BEGIN_FIELD, name='examplesPerSecond', description='null', id=204, version=0, encodedLength=0, offset=24, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='float', description='null', id=-1, version=0, encodedLength=4, offset=24, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=FLOAT, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("examplesPerSecond=")
				builder.Append(examplesPerSecond())
				builder.Append("|"c)
				'Token{signal=BEGIN_FIELD, name='minibatchesPerSecond', description='null', id=205, version=0, encodedLength=0, offset=28, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='float', description='null', id=-1, version=0, encodedLength=4, offset=28, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=FLOAT, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("minibatchesPerSecond=")
				builder.Append(minibatchesPerSecond())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field gcStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly gcStats_Conflict As New GcStatsDecoder()

		Public Shared Function gcStatsDecoderId() As Long
			Return 300
		End Function

		Public Overridable Function gcStats() As GcStatsDecoder
			gcStats_Conflict.wrap(parentMessage, buffer)
			Return gcStats_Conflict
		End Function

		Public Class GcStatsDecoder
			Implements IEnumerable(Of GcStatsDecoder), IEnumerator(Of GcStatsDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As UpdateDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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

			Public Overridable Function GetEnumerator() As IEnumerator(Of GcStatsDecoder) Implements IEnumerator(Of GcStatsDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As GcStatsDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function deltaGCCountId() As Integer
				Return 301
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String deltaGCCountMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function deltaGCCountMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function deltaGCCountNullValue() As Integer
				Return -2147483648
			End Function

			Public Shared Function deltaGCCountMinValue() As Integer
				Return -2147483647
			End Function

			Public Shared Function deltaGCCountMaxValue() As Integer
				Return 2147483647
			End Function

			Public Overridable Function deltaGCCount() As Integer
				Return buffer.getInt(offset + 0, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


			Public Shared Function deltaGCTimeMsId() As Integer
				Return 302
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String deltaGCTimeMsMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function deltaGCTimeMsMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function deltaGCTimeMsNullValue() As Integer
				Return -2147483648
			End Function

			Public Shared Function deltaGCTimeMsMinValue() As Integer
				Return -2147483647
			End Function

			Public Shared Function deltaGCTimeMsMaxValue() As Integer
				Return 2147483647
			End Function

			Public Overridable Function deltaGCTimeMs() As Integer
				Return buffer.getInt(offset + 4, java.nio.ByteOrder.LITTLE_ENDIAN)
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

			Public Overridable Function gcNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
				Dim limit As Integer = parentMessage.limit()
				Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getGcName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
			Public Overridable Function getGcName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getGcName(final byte[] dst, final int dstOffset, final int length)
			Public Overridable Function getGcName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

			Public Overridable Function gcName() As String
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
				'Token{signal=BEGIN_FIELD, name='deltaGCCount', description='null', id=301, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int32', description='null', id=-1, version=0, encodedLength=4, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("deltaGCCount=")
				builder.Append(deltaGCCount())
				builder.Append("|"c)
				'Token{signal=BEGIN_FIELD, name='deltaGCTimeMs', description='null', id=302, version=0, encodedLength=0, offset=4, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='int32', description='null', id=-1, version=0, encodedLength=4, offset=4, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("deltaGCTimeMs=")
				builder.Append(deltaGCTimeMs())
				builder.Append("|"c)
				'Token{signal=BEGIN_VAR_DATA, name='gcName', description='null', id=1000, version=0, encodedLength=0, offset=8, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("gcName=")
				builder.Append(gcName())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field paramNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly paramNames_Conflict As New ParamNamesDecoder()

		Public Shared Function paramNamesDecoderId() As Long
			Return 350
		End Function

		Public Overridable Function paramNames() As ParamNamesDecoder
			paramNames_Conflict.wrap(parentMessage, buffer)
			Return paramNames_Conflict
		End Function

		Public Class ParamNamesDecoder
			Implements IEnumerable(Of ParamNamesDecoder), IEnumerator(Of ParamNamesDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As UpdateDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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

			Public Overridable Function GetEnumerator() As IEnumerator(Of ParamNamesDecoder) Implements IEnumerator(Of ParamNamesDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As ParamNamesDecoder
				If index + 1 >= count_Conflict Then
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

			Public Overridable Function paramNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
				Dim limit As Integer = parentMessage.limit()
				Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getParamName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
			Public Overridable Function getParamName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getParamName(final byte[] dst, final int dstOffset, final int length)
			Public Overridable Function getParamName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

			Public Overridable Function paramName() As String
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
				'Token{signal=BEGIN_VAR_DATA, name='paramName', description='null', id=1100, version=0, encodedLength=0, offset=0, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("paramName=")
				builder.Append(paramName())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field layerNames was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly layerNames_Conflict As New LayerNamesDecoder()

		Public Shared Function layerNamesDecoderId() As Long
			Return 351
		End Function

		Public Overridable Function layerNames() As LayerNamesDecoder
			layerNames_Conflict.wrap(parentMessage, buffer)
			Return layerNames_Conflict
		End Function

		Public Class LayerNamesDecoder
			Implements IEnumerable(Of LayerNamesDecoder), IEnumerator(Of LayerNamesDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As UpdateDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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

			Public Overridable Function GetEnumerator() As IEnumerator(Of LayerNamesDecoder) Implements IEnumerator(Of LayerNamesDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As LayerNamesDecoder
				If index + 1 >= count_Conflict Then
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

			Public Overridable Function layerNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
				Dim limit As Integer = parentMessage.limit()
				Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getLayerName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
			Public Overridable Function getLayerName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getLayerName(final byte[] dst, final int dstOffset, final int length)
			Public Overridable Function getLayerName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

			Public Overridable Function layerName() As String
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
				'Token{signal=BEGIN_VAR_DATA, name='layerName', description='null', id=1101, version=0, encodedLength=0, offset=0, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("layerName=")
				builder.Append(layerName())
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field perParameterStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly perParameterStats_Conflict As New PerParameterStatsDecoder()

		Public Shared Function perParameterStatsDecoderId() As Long
			Return 400
		End Function

		Public Overridable Function perParameterStats() As PerParameterStatsDecoder
			perParameterStats_Conflict.wrap(parentMessage, buffer)
			Return perParameterStats_Conflict
		End Function

		Public Class PerParameterStatsDecoder
			Implements IEnumerable(Of PerParameterStatsDecoder), IEnumerator(Of PerParameterStatsDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As UpdateDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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
				Return 4
			End Function

			Public Overridable Function actingBlockLength() As Integer
				Return blockLength
			End Function

			Public Overridable Function count() As Integer
				Return count_Conflict
			End Function

			Public Overridable Function GetEnumerator() As IEnumerator(Of PerParameterStatsDecoder) Implements IEnumerator(Of PerParameterStatsDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As PerParameterStatsDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

			Public Shared Function learningRateId() As Integer
				Return 401
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String learningRateMetaAttribute(final MetaAttribute metaAttribute)
			Public Shared Function learningRateMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

			Public Shared Function learningRateNullValue() As Single
				Return Float.NaN
			End Function

			Public Shared Function learningRateMinValue() As Single
				Return 1.401298464324817E-45f
			End Function

			Public Shared Function learningRateMaxValue() As Single
				Return 3.4028234663852886E38f
			End Function

			Public Overridable Function learningRate() As Single
				Return buffer.getFloat(offset + 0, java.nio.ByteOrder.LITTLE_ENDIAN)
			End Function


'JAVA TO VB CONVERTER NOTE: The field summaryStat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend ReadOnly summaryStat_Conflict As New SummaryStatDecoder()

			Public Shared Function summaryStatDecoderId() As Long
				Return 402
			End Function

			Public Overridable Function summaryStat() As SummaryStatDecoder
				summaryStat_Conflict.wrap(parentMessage, buffer)
				Return summaryStat_Conflict
			End Function

			Public Class SummaryStatDecoder
				Implements IEnumerable(Of SummaryStatDecoder), IEnumerator(Of SummaryStatDecoder)

				Friend Const HEADER_SIZE As Integer = 4
				Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
				Friend parentMessage As UpdateDecoder
				Friend buffer As DirectBuffer
				Friend blockLength As Integer
				Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
				Friend count_Conflict As Integer
				Friend index As Integer
				Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
				Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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
					Return 10
				End Function

				Public Overridable Function actingBlockLength() As Integer
					Return blockLength
				End Function

				Public Overridable Function count() As Integer
					Return count_Conflict
				End Function

				Public Overridable Function GetEnumerator() As IEnumerator(Of SummaryStatDecoder) Implements IEnumerator(Of SummaryStatDecoder).GetEnumerator
					Return Me
				End Function

				Public Overridable Sub remove()
					Throw New System.NotSupportedException()
				End Sub

				Public Overridable Function hasNext() As Boolean
					Return (index + 1) < count_Conflict
				End Function

				Public Overridable Function [next]() As SummaryStatDecoder
					If index + 1 >= count_Conflict Then
						Throw New java.util.NoSuchElementException()
					End If

					offset = parentMessage.limit()
					parentMessage.limit(offset + blockLength)
					index += 1

					Return Me
				End Function

				Public Shared Function statTypeId() As Integer
					Return 403
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String statTypeMetaAttribute(final MetaAttribute metaAttribute)
				Public Shared Function statTypeMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

				Public Overridable Function statType() As StatsType
					Return StatsType.get((CShort(buffer.getByte(offset + 0) And &HFF)))
				End Function


				Public Shared Function summaryTypeId() As Integer
					Return 404
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String summaryTypeMetaAttribute(final MetaAttribute metaAttribute)
				Public Shared Function summaryTypeMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

				Public Overridable Function summaryType() As SummaryType
					Return SummaryType.get((CShort(buffer.getByte(offset + 1) And &HFF)))
				End Function


				Public Shared Function valueId() As Integer
					Return 405
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String valueMetaAttribute(final MetaAttribute metaAttribute)
				Public Shared Function valueMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

				Public Shared Function valueNullValue() As Double
					Return Double.NaN
				End Function

				Public Shared Function valueMinValue() As Double
					Return 4.9E-324R
				End Function

				Public Shared Function valueMaxValue() As Double
					Return 1.7976931348623157E308R
				End Function

				Public Overridable Function value() As Double
					Return buffer.getDouble(offset + 2, java.nio.ByteOrder.LITTLE_ENDIAN)
				End Function


				Public Overrides Function ToString() As String
					Return appendTo(New StringBuilder(100)).ToString()
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
				Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
					builder.Append("("c)
					'Token{signal=BEGIN_FIELD, name='statType', description='null', id=403, version=0, encodedLength=0, offset=0, componentTokenCount=8, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=BEGIN_ENUM, name='StatsType', description='null', id=-1, version=0, encodedLength=1, offset=0, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=UINT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
					builder.Append("statType=")
					builder.Append(statType())
					builder.Append("|"c)
					'Token{signal=BEGIN_FIELD, name='summaryType', description='null', id=404, version=0, encodedLength=0, offset=1, componentTokenCount=7, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=BEGIN_ENUM, name='SummaryType', description='null', id=-1, version=0, encodedLength=1, offset=1, componentTokenCount=5, encoding=Encoding{presence=REQUIRED, primitiveType=UINT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
					builder.Append("summaryType=")
					builder.Append(summaryType())
					builder.Append("|"c)
					'Token{signal=BEGIN_FIELD, name='value', description='null', id=405, version=0, encodedLength=0, offset=2, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=ENCODING, name='double', description='null', id=-1, version=0, encodedLength=8, offset=2, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=DOUBLE, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					builder.Append("value=")
					builder.Append(value())
					builder.Append(")"c)
					Return builder
				End Function
			End Class

'JAVA TO VB CONVERTER NOTE: The field histograms was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend ReadOnly histograms_Conflict As New HistogramsDecoder()

			Public Shared Function histogramsDecoderId() As Long
				Return 406
			End Function

			Public Overridable Function histograms() As HistogramsDecoder
				histograms_Conflict.wrap(parentMessage, buffer)
				Return histograms_Conflict
			End Function

			Public Class HistogramsDecoder
				Implements IEnumerable(Of HistogramsDecoder), IEnumerator(Of HistogramsDecoder)

				Friend Const HEADER_SIZE As Integer = 4
				Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
				Friend parentMessage As UpdateDecoder
				Friend buffer As DirectBuffer
				Friend blockLength As Integer
				Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
				Friend count_Conflict As Integer
				Friend index As Integer
				Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
				Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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
					Return 21
				End Function

				Public Overridable Function actingBlockLength() As Integer
					Return blockLength
				End Function

				Public Overridable Function count() As Integer
					Return count_Conflict
				End Function

				Public Overridable Function GetEnumerator() As IEnumerator(Of HistogramsDecoder) Implements IEnumerator(Of HistogramsDecoder).GetEnumerator
					Return Me
				End Function

				Public Overridable Sub remove()
					Throw New System.NotSupportedException()
				End Sub

				Public Overridable Function hasNext() As Boolean
					Return (index + 1) < count_Conflict
				End Function

				Public Overridable Function [next]() As HistogramsDecoder
					If index + 1 >= count_Conflict Then
						Throw New java.util.NoSuchElementException()
					End If

					offset = parentMessage.limit()
					parentMessage.limit(offset + blockLength)
					index += 1

					Return Me
				End Function

				Public Shared Function statTypeId() As Integer
					Return 407
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String statTypeMetaAttribute(final MetaAttribute metaAttribute)
				Public Shared Function statTypeMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

				Public Overridable Function statType() As StatsType
					Return StatsType.get((CShort(buffer.getByte(offset + 0) And &HFF)))
				End Function


				Public Shared Function minValueId() As Integer
					Return 408
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String minValueMetaAttribute(final MetaAttribute metaAttribute)
				Public Shared Function minValueMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

				Public Shared Function minValueNullValue() As Double
					Return Double.NaN
				End Function

				Public Shared Function minValueMinValue() As Double
					Return 4.9E-324R
				End Function

				Public Shared Function minValueMaxValue() As Double
					Return 1.7976931348623157E308R
				End Function

				Public Overridable Function minValue() As Double
					Return buffer.getDouble(offset + 1, java.nio.ByteOrder.LITTLE_ENDIAN)
				End Function


				Public Shared Function maxValueId() As Integer
					Return 409
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String maxValueMetaAttribute(final MetaAttribute metaAttribute)
				Public Shared Function maxValueMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

				Public Shared Function maxValueNullValue() As Double
					Return Double.NaN
				End Function

				Public Shared Function maxValueMinValue() As Double
					Return 4.9E-324R
				End Function

				Public Shared Function maxValueMaxValue() As Double
					Return 1.7976931348623157E308R
				End Function

				Public Overridable Function maxValue() As Double
					Return buffer.getDouble(offset + 9, java.nio.ByteOrder.LITTLE_ENDIAN)
				End Function


				Public Shared Function nBinsId() As Integer
					Return 410
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String nBinsMetaAttribute(final MetaAttribute metaAttribute)
				Public Shared Function nBinsMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

				Public Shared Function nBinsNullValue() As Integer
					Return -2147483648
				End Function

				Public Shared Function nBinsMinValue() As Integer
					Return -2147483647
				End Function

				Public Shared Function nBinsMaxValue() As Integer
					Return 2147483647
				End Function

				Public Overridable Function nBins() As Integer
					Return buffer.getInt(offset + 17, java.nio.ByteOrder.LITTLE_ENDIAN)
				End Function


'JAVA TO VB CONVERTER NOTE: The field histogramCounts was renamed since Visual Basic does not allow fields to have the same name as other class members:
				Friend ReadOnly histogramCounts_Conflict As New HistogramCountsDecoder()

				Public Shared Function histogramCountsDecoderId() As Long
					Return 411
				End Function

				Public Overridable Function histogramCounts() As HistogramCountsDecoder
					histogramCounts_Conflict.wrap(parentMessage, buffer)
					Return histogramCounts_Conflict
				End Function

				Public Class HistogramCountsDecoder
					Implements IEnumerable(Of HistogramCountsDecoder), IEnumerator(Of HistogramCountsDecoder)

					Friend Const HEADER_SIZE As Integer = 4
					Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
					Friend parentMessage As UpdateDecoder
					Friend buffer As DirectBuffer
					Friend blockLength As Integer
					Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
					Friend count_Conflict As Integer
					Friend index As Integer
					Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
					Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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
						Return 4
					End Function

					Public Overridable Function actingBlockLength() As Integer
						Return blockLength
					End Function

					Public Overridable Function count() As Integer
						Return count_Conflict
					End Function

					Public Overridable Function GetEnumerator() As IEnumerator(Of HistogramCountsDecoder) Implements IEnumerator(Of HistogramCountsDecoder).GetEnumerator
						Return Me
					End Function

					Public Overridable Sub remove()
						Throw New System.NotSupportedException()
					End Sub

					Public Overridable Function hasNext() As Boolean
						Return (index + 1) < count_Conflict
					End Function

					Public Overridable Function [next]() As HistogramCountsDecoder
						If index + 1 >= count_Conflict Then
							Throw New java.util.NoSuchElementException()
						End If

						offset = parentMessage.limit()
						parentMessage.limit(offset + blockLength)
						index += 1

						Return Me
					End Function

					Public Shared Function binCountId() As Integer
						Return 412
					End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String binCountMetaAttribute(final MetaAttribute metaAttribute)
					Public Shared Function binCountMetaAttribute(ByVal metaAttribute As MetaAttribute) As String
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

					Public Shared Function binCountNullValue() As Long
						Return 4294967294L
					End Function

					Public Shared Function binCountMinValue() As Long
						Return 0L
					End Function

					Public Shared Function binCountMaxValue() As Long
						Return 4294967293L
					End Function

					Public Overridable Function binCount() As Long
						Return (buffer.getInt(offset + 0, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
					End Function


					Public Overrides Function ToString() As String
						Return appendTo(New StringBuilder(100)).ToString()
					End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
					Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
						builder.Append("("c)
						'Token{signal=BEGIN_FIELD, name='binCount', description='null', id=412, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
						'Token{signal=ENCODING, name='uint32', description='null', id=-1, version=0, encodedLength=4, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=UINT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
						builder.Append("binCount=")
						builder.Append(binCount())
						builder.Append(")"c)
						Return builder
					End Function
				End Class

				Public Overrides Function ToString() As String
					Return appendTo(New StringBuilder(100)).ToString()
				End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
				Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
					builder.Append("("c)
					'Token{signal=BEGIN_FIELD, name='statType', description='null', id=407, version=0, encodedLength=0, offset=0, componentTokenCount=8, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=BEGIN_ENUM, name='StatsType', description='null', id=-1, version=0, encodedLength=1, offset=0, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=UINT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
					builder.Append("statType=")
					builder.Append(statType())
					builder.Append("|"c)
					'Token{signal=BEGIN_FIELD, name='minValue', description='null', id=408, version=0, encodedLength=0, offset=1, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=ENCODING, name='double', description='null', id=-1, version=0, encodedLength=8, offset=1, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=DOUBLE, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					builder.Append("minValue=")
					builder.Append(minValue())
					builder.Append("|"c)
					'Token{signal=BEGIN_FIELD, name='maxValue', description='null', id=409, version=0, encodedLength=0, offset=9, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=ENCODING, name='double', description='null', id=-1, version=0, encodedLength=8, offset=9, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=DOUBLE, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					builder.Append("maxValue=")
					builder.Append(maxValue())
					builder.Append("|"c)
					'Token{signal=BEGIN_FIELD, name='nBins', description='null', id=410, version=0, encodedLength=0, offset=17, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=ENCODING, name='int32', description='null', id=-1, version=0, encodedLength=4, offset=17, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					builder.Append("nBins=")
					builder.Append(nBins())
					builder.Append("|"c)
					'Token{signal=BEGIN_GROUP, name='histogramCounts', description='null', id=411, version=0, encodedLength=4, offset=21, componentTokenCount=9, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
					builder.Append("histogramCounts=[")
					Dim histogramCounts As HistogramCountsDecoder = Me.histogramCounts()
					If histogramCounts.count() > 0 Then
						Do While histogramCounts.MoveNext()
							histogramCounts.Current.appendTo(builder)
							builder.Append(","c)
						Loop
						builder.Length = builder.Length - 1
					End If
					builder.Append("]"c)
					builder.Append(")"c)
					Return builder
				End Function
			End Class

			Public Overrides Function ToString() As String
				Return appendTo(New StringBuilder(100)).ToString()
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
			Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
				builder.Append("("c)
				'Token{signal=BEGIN_FIELD, name='learningRate', description='null', id=401, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				'Token{signal=ENCODING, name='float', description='null', id=-1, version=0, encodedLength=4, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=FLOAT, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
				builder.Append("learningRate=")
				builder.Append(learningRate())
				builder.Append("|"c)
				'Token{signal=BEGIN_GROUP, name='summaryStat', description='null', id=402, version=0, encodedLength=10, offset=4, componentTokenCount=24, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
				builder.Append("summaryStat=[")
				Dim summaryStat As SummaryStatDecoder = Me.summaryStat()
				If summaryStat.count() > 0 Then
					Do While summaryStat.MoveNext()
						summaryStat.Current.appendTo(builder)
						builder.Append(","c)
					Loop
					builder.Length = builder.Length - 1
				End If
				builder.Append("]"c)
				builder.Append("|"c)
				'Token{signal=BEGIN_GROUP, name='histograms', description='null', id=406, version=0, encodedLength=21, offset=-1, componentTokenCount=32, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
				builder.Append("histograms=[")
				Dim histograms As HistogramsDecoder = Me.histograms()
				If histograms.count() > 0 Then
					Do While histograms.MoveNext()
						histograms.Current.appendTo(builder)
						builder.Append(","c)
					Loop
					builder.Length = builder.Length - 1
				End If
				builder.Append("]"c)
				builder.Append(")"c)
				Return builder
			End Function
		End Class

'JAVA TO VB CONVERTER NOTE: The field dataSetMetaDataBytes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly dataSetMetaDataBytes_Conflict As New DataSetMetaDataBytesDecoder()

		Public Shared Function dataSetMetaDataBytesDecoderId() As Long
			Return 500
		End Function

		Public Overridable Function dataSetMetaDataBytes() As DataSetMetaDataBytesDecoder
			dataSetMetaDataBytes_Conflict.wrap(parentMessage, buffer)
			Return dataSetMetaDataBytes_Conflict
		End Function

		Public Class DataSetMetaDataBytesDecoder
			Implements IEnumerable(Of DataSetMetaDataBytesDecoder), IEnumerator(Of DataSetMetaDataBytesDecoder)

			Friend Const HEADER_SIZE As Integer = 4
			Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
			Friend parentMessage As UpdateDecoder
			Friend buffer As DirectBuffer
			Friend blockLength As Integer
			Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend count_Conflict As Integer
			Friend index As Integer
			Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
			Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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

			Public Overridable Function GetEnumerator() As IEnumerator(Of DataSetMetaDataBytesDecoder) Implements IEnumerator(Of DataSetMetaDataBytesDecoder).GetEnumerator
				Return Me
			End Function

			Public Overridable Sub remove()
				Throw New System.NotSupportedException()
			End Sub

			Public Overridable Function hasNext() As Boolean
				Return (index + 1) < count_Conflict
			End Function

			Public Overridable Function [next]() As DataSetMetaDataBytesDecoder
				If index + 1 >= count_Conflict Then
					Throw New java.util.NoSuchElementException()
				End If

				offset = parentMessage.limit()
				parentMessage.limit(offset + blockLength)
				index += 1

				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The field metaDataBytes was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend ReadOnly metaDataBytes_Conflict As New MetaDataBytesDecoder()

			Public Shared Function metaDataBytesDecoderId() As Long
				Return 501
			End Function

			Public Overridable Function metaDataBytes() As MetaDataBytesDecoder
				metaDataBytes_Conflict.wrap(parentMessage, buffer)
				Return metaDataBytes_Conflict
			End Function

			Public Class MetaDataBytesDecoder
				Implements IEnumerable(Of MetaDataBytesDecoder), IEnumerator(Of MetaDataBytesDecoder)

				Friend Const HEADER_SIZE As Integer = 4
				Friend ReadOnly dimensions As New GroupSizeEncodingDecoder()
				Friend parentMessage As UpdateDecoder
				Friend buffer As DirectBuffer
				Friend blockLength As Integer
				Friend actingVersion As Integer
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
				Friend count_Conflict As Integer
				Friend index As Integer
				Friend offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void wrap(final UpdateDecoder parentMessage, final org.agrona.DirectBuffer buffer)
				Public Overridable Sub wrap(ByVal parentMessage As UpdateDecoder, ByVal buffer As DirectBuffer)
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

				Public Overridable Function GetEnumerator() As IEnumerator(Of MetaDataBytesDecoder) Implements IEnumerator(Of MetaDataBytesDecoder).GetEnumerator
					Return Me
				End Function

				Public Overridable Sub remove()
					Throw New System.NotSupportedException()
				End Sub

				Public Overridable Function hasNext() As Boolean
					Return (index + 1) < count_Conflict
				End Function

				Public Overridable Function [next]() As MetaDataBytesDecoder
					If index + 1 >= count_Conflict Then
						Throw New java.util.NoSuchElementException()
					End If

					offset = parentMessage.limit()
					parentMessage.limit(offset + blockLength)
					index += 1

					Return Me
				End Function

				Public Shared Function bytesId() As Integer
					Return 502
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
					'Token{signal=BEGIN_FIELD, name='bytes', description='null', id=502, version=0, encodedLength=0, offset=0, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					'Token{signal=ENCODING, name='int8', description='null', id=-1, version=0, encodedLength=1, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
					builder.Append("bytes=")
					builder.Append(bytes())
					builder.Append(")"c)
					Return builder
				End Function
			End Class

			Public Overrides Function ToString() As String
				Return appendTo(New StringBuilder(100)).ToString()
			End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
			Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
				builder.Append("("c)
				'Token{signal=BEGIN_GROUP, name='metaDataBytes', description='null', id=501, version=0, encodedLength=1, offset=0, componentTokenCount=9, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
				builder.Append("metaDataBytes=[")
				Dim metaDataBytes As MetaDataBytesDecoder = Me.metaDataBytes()
				If metaDataBytes.count() > 0 Then
					Do While metaDataBytes.MoveNext()
						metaDataBytes.Current.appendTo(builder)
						builder.Append(","c)
					Loop
					builder.Length = builder.Length - 1
				End If
				builder.Append("]"c)
				builder.Append(")"c)
				Return builder
			End Function
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

		Public Overridable Function dataSetMetaDataClassNameLength() As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int limit = parentMessage.limit();
			Dim limit As Integer = parentMessage.limit()
			Return CInt(buffer.getInt(limit, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int getDataSetMetaDataClassName(final org.agrona.MutableDirectBuffer dst, final int dstOffset, final int length)
		Public Overridable Function getDataSetMetaDataClassName(ByVal dst As MutableDirectBuffer, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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
'ORIGINAL LINE: public int getDataSetMetaDataClassName(final byte[] dst, final int dstOffset, final int length)
		Public Overridable Function getDataSetMetaDataClassName(ByVal dst() As SByte, ByVal dstOffset As Integer, ByVal length As Integer) As Integer
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

		Public Overridable Function dataSetMetaDataClassName() As String
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
			builder.Append("[Update](sbeTemplateId=")
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
			'Token{signal=BEGIN_FIELD, name='deltaTime', description='null', id=2, version=0, encodedLength=0, offset=8, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int32', description='null', id=-1, version=0, encodedLength=4, offset=8, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("deltaTime=")
			builder.Append(deltaTime())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='iterationCount', description='null', id=3, version=0, encodedLength=0, offset=12, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int32', description='null', id=-1, version=0, encodedLength=4, offset=12, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("iterationCount=")
			builder.Append(iterationCount())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='fieldsPresent', description='null', id=4, version=0, encodedLength=0, offset=16, componentTokenCount=26, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=BEGIN_SET, name='UpdateFieldsPresent', description='null', id=-1, version=0, encodedLength=4, offset=16, componentTokenCount=24, encoding=Encoding{presence=REQUIRED, primitiveType=UINT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='UpdateFieldsPresent'}}
			builder.Append("fieldsPresent=")
			builder.Append(fieldsPresent())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='statsCollectionDuration', description='null', id=5, version=0, encodedLength=0, offset=20, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='int32', description='null', id=-1, version=0, encodedLength=4, offset=20, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=INT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("statsCollectionDuration=")
			builder.Append(statsCollectionDuration())
			builder.Append("|"c)
			'Token{signal=BEGIN_FIELD, name='score', description='null', id=6, version=0, encodedLength=0, offset=24, componentTokenCount=3, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			'Token{signal=ENCODING, name='double', description='null', id=-1, version=0, encodedLength=8, offset=24, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=DOUBLE, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("score=")
			builder.Append(score())
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='memoryUse', description='null', id=100, version=0, encodedLength=9, offset=32, componentTokenCount=19, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("memoryUse=[")
			Dim memoryUse As MemoryUseDecoder = Me.memoryUse()
			If memoryUse.count() > 0 Then
				Do While memoryUse.MoveNext()
					memoryUse.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='performance', description='null', id=200, version=0, encodedLength=32, offset=-1, componentTokenCount=21, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("performance=[")
			Dim performance As PerformanceDecoder = Me.performance()
			If performance.count() > 0 Then
				Do While performance.MoveNext()
					performance.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='gcStats', description='null', id=300, version=0, encodedLength=8, offset=-1, componentTokenCount=18, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("gcStats=[")
			Dim gcStats As GcStatsDecoder = Me.gcStats()
			If gcStats.count() > 0 Then
				Do While gcStats.MoveNext()
					gcStats.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='paramNames', description='null', id=350, version=0, encodedLength=0, offset=-1, componentTokenCount=12, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("paramNames=[")
			Dim paramNames As ParamNamesDecoder = Me.paramNames()
			If paramNames.count() > 0 Then
				Do While paramNames.MoveNext()
					paramNames.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='layerNames', description='null', id=351, version=0, encodedLength=0, offset=-1, componentTokenCount=12, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("layerNames=[")
			Dim layerNames As LayerNamesDecoder = Me.layerNames()
			If layerNames.count() > 0 Then
				Do While layerNames.MoveNext()
					layerNames.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='perParameterStats', description='null', id=400, version=0, encodedLength=4, offset=-1, componentTokenCount=65, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("perParameterStats=[")
			Dim perParameterStats As PerParameterStatsDecoder = Me.perParameterStats()
			If perParameterStats.count() > 0 Then
				Do While perParameterStats.MoveNext()
					perParameterStats.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_GROUP, name='dataSetMetaDataBytes', description='null', id=500, version=0, encodedLength=0, offset=-1, componentTokenCount=15, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("dataSetMetaDataBytes=[")
			Dim dataSetMetaDataBytes As DataSetMetaDataBytesDecoder = Me.dataSetMetaDataBytes()
			If dataSetMetaDataBytes.count() > 0 Then
				Do While dataSetMetaDataBytes.MoveNext()
					dataSetMetaDataBytes.Current.appendTo(builder)
					builder.Append(","c)
				Loop
				builder.Length = builder.Length - 1
			End If
			builder.Append("]"c)
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='sessionID', description='null', id=1200, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("sessionID=")
			builder.Append(sessionID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='typeID', description='null', id=1201, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("typeID=")
			builder.Append(typeID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='workerID', description='null', id=1202, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("workerID=")
			builder.Append(workerID())
			builder.Append("|"c)
			'Token{signal=BEGIN_VAR_DATA, name='dataSetMetaDataClassName', description='null', id=1300, version=0, encodedLength=0, offset=-1, componentTokenCount=6, encoding=Encoding{presence=REQUIRED, primitiveType=null, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='null', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("dataSetMetaDataClassName=")
			builder.Append(dataSetMetaDataClassName())

			limit(originalLimit)

			Return builder
		End Function
	End Class

End Namespace