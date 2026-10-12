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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.VarDataUTF8Decoder"}) @SuppressWarnings("all") public class VarDataUTF8Decoder
	Public Class VarDataUTF8Decoder
		Public Const ENCODED_LENGTH As Integer = -1
		Private buffer As DirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public VarDataUTF8Decoder wrap(final org.agrona.DirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer) As VarDataUTF8Decoder
			Me.buffer = buffer
			Me.offset = offset

			Return Me
		End Function

		Public Overridable Function encodedLength() As Integer
			Return ENCODED_LENGTH
		End Function

		Public Shared Function lengthNullValue() As Long
			Return 4294967294L
		End Function

		Public Shared Function lengthMinValue() As Long
			Return 0L
		End Function

		Public Shared Function lengthMaxValue() As Long
			Return 1073741824L
		End Function

		Public Overridable Function length() As Long
			Return (buffer.getInt(offset + 0, java.nio.ByteOrder.LITTLE_ENDIAN) And &HFFFF_FFFFL)
		End Function


		Public Shared Function varDataNullValue() As Short
			Return CShort(255)
		End Function

		Public Shared Function varDataMinValue() As Short
			Return CShort(0)
		End Function

		Public Shared Function varDataMaxValue() As Short
			Return CShort(254)
		End Function

		Public Overrides Function ToString() As String
			Return appendTo(New StringBuilder(100)).ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
		Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
			builder.Append("("c)
			'Token{signal=ENCODING, name='length', description='null', id=-1, version=0, encodedLength=4, offset=0, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=UINT32, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=1073741824, nullValue=null, constValue=null, characterEncoding='UTF-8', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append("length=")
			builder.Append(length())
			builder.Append("|"c)
			'Token{signal=ENCODING, name='varData', description='null', id=-1, version=0, encodedLength=-1, offset=4, componentTokenCount=1, encoding=Encoding{presence=REQUIRED, primitiveType=UINT8, byteOrder=LITTLE_ENDIAN, minValue=null, maxValue=null, nullValue=null, constValue=null, characterEncoding='UTF-8', epoch='unix', timeUnit=nanosecond, semanticType='null'}}
			builder.Append(")"c)

			Return builder
		End Function
	End Class

End Namespace