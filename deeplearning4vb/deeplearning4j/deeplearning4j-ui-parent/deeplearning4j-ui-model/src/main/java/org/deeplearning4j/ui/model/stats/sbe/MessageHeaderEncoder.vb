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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.MessageHeaderEncoder"}) @SuppressWarnings("all") public class MessageHeaderEncoder
	Public Class MessageHeaderEncoder
		Public Const ENCODED_LENGTH As Integer = 8
		Private buffer As MutableDirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MessageHeaderEncoder wrap(final org.agrona.MutableDirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As MutableDirectBuffer, ByVal offset As Integer) As MessageHeaderEncoder
			Me.buffer = buffer
			Me.offset = offset

			Return Me
		End Function

		Public Overridable Function encodedLength() As Integer
			Return ENCODED_LENGTH
		End Function

		Public Shared Function blockLengthNullValue() As Integer
			Return 65535
		End Function

		Public Shared Function blockLengthMinValue() As Integer
			Return 0
		End Function

		Public Shared Function blockLengthMaxValue() As Integer
			Return 65534
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MessageHeaderEncoder blockLength(final int value)
		Public Overridable Function blockLength(ByVal value As Integer) As MessageHeaderEncoder
			buffer.putShort(offset + 0, CShort(value), java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Public Shared Function templateIdNullValue() As Integer
			Return 65535
		End Function

		Public Shared Function templateIdMinValue() As Integer
			Return 0
		End Function

		Public Shared Function templateIdMaxValue() As Integer
			Return 65534
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MessageHeaderEncoder templateId(final int value)
		Public Overridable Function templateId(ByVal value As Integer) As MessageHeaderEncoder
			buffer.putShort(offset + 2, CShort(value), java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Public Shared Function schemaIdNullValue() As Integer
			Return 65535
		End Function

		Public Shared Function schemaIdMinValue() As Integer
			Return 0
		End Function

		Public Shared Function schemaIdMaxValue() As Integer
			Return 65534
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MessageHeaderEncoder schemaId(final int value)
		Public Overridable Function schemaId(ByVal value As Integer) As MessageHeaderEncoder
			buffer.putShort(offset + 4, CShort(value), java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function


		Public Shared Function versionNullValue() As Integer
			Return 65535
		End Function

		Public Shared Function versionMinValue() As Integer
			Return 0
		End Function

		Public Shared Function versionMaxValue() As Integer
			Return 65534
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MessageHeaderEncoder version(final int value)
		Public Overridable Function version(ByVal value As Integer) As MessageHeaderEncoder
			buffer.putShort(offset + 6, CShort(value), java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

	End Class

End Namespace