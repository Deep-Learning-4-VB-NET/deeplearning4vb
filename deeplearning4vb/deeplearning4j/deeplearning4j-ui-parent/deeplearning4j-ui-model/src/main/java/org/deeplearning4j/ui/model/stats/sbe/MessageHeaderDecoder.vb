Imports DirectBuffer = org.agrona.DirectBuffer

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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.MessageHeaderDecoder"}) @SuppressWarnings("all") public class MessageHeaderDecoder
	Public Class MessageHeaderDecoder
		Public Const ENCODED_LENGTH As Integer = 8
		Private buffer As DirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public MessageHeaderDecoder wrap(final org.agrona.DirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer) As MessageHeaderDecoder
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

		Public Overridable Function blockLength() As Integer
			Return (buffer.getShort(offset + 0, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF)
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

		Public Overridable Function templateId() As Integer
			Return (buffer.getShort(offset + 2, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF)
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

		Public Overridable Function schemaId() As Integer
			Return (buffer.getShort(offset + 4, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF)
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

		Public Overridable Function version() As Integer
			Return (buffer.getShort(offset + 6, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF)
		End Function

	End Class

End Namespace