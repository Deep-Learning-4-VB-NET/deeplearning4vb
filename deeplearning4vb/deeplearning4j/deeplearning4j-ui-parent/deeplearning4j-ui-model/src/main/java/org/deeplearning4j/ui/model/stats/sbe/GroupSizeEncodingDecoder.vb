Imports System.Text
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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.GroupSizeEncodingDecoder"}) @SuppressWarnings("all") public class GroupSizeEncodingDecoder
	Public Class GroupSizeEncodingDecoder
		Public Const ENCODED_LENGTH As Integer = 4
		Private buffer As DirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public GroupSizeEncodingDecoder wrap(final org.agrona.DirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer) As GroupSizeEncodingDecoder
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


		Public Shared Function numInGroupNullValue() As Integer
			Return 65535
		End Function

		Public Shared Function numInGroupMinValue() As Integer
			Return 0
		End Function

		Public Shared Function numInGroupMaxValue() As Integer
			Return 65534
		End Function

		Public Overridable Function numInGroup() As Integer
			Return (buffer.getShort(offset + 2, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF)
		End Function

		Public Overrides Function ToString() As String
			Return appendTo(New StringBuilder(100)).ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
		Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
			builder.Append("("c)
			'Token{signal=ENCODING, name='blockLength', description='Extra metadata bytes', id=-1, version=0, encodedLength=2, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=UINT16, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='UTF-8', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("blockLength=")
			builder.Append(blockLength())
			builder.Append("|"c)
			'Token{signal=ENCODING, name='numInGroup', description='Extra metadata bytes', id=-1, version=0, encodedLength=2, offset=2, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=UINT16, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='UTF-8', epoch='null', timeUnit=null, semanticType='null'}}
			builder.Append("numInGroup=")
			builder.Append(numInGroup())
			builder.Append(")"c)

			Return builder
		End Function
	End Class

End Namespace