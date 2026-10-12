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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.InitFieldsPresentEncoder"}) @SuppressWarnings("all") public class InitFieldsPresentEncoder
	Public Class InitFieldsPresentEncoder
		Public Const ENCODED_LENGTH As Integer = 1
		Private buffer As MutableDirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public InitFieldsPresentEncoder wrap(final org.agrona.MutableDirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As MutableDirectBuffer, ByVal offset As Integer) As InitFieldsPresentEncoder
			Me.buffer = buffer
			Me.offset = offset

			Return Me
		End Function

		Public Overridable Function encodedLength() As Integer
			Return ENCODED_LENGTH
		End Function

		Public Overridable Function clear() As InitFieldsPresentEncoder
			buffer.putByte(offset, CSByte(CShort(0)))
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public InitFieldsPresentEncoder softwareInfo(final boolean value)
		Public Overridable Function softwareInfo(ByVal value As Boolean) As InitFieldsPresentEncoder
			Dim bits As SByte = buffer.getByte(offset)
			bits = CSByte(If(value, bits Or (1 << 0), bits And Not (1 << 0)))
			buffer.putByte(offset, bits)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public InitFieldsPresentEncoder hardwareInfo(final boolean value)
		Public Overridable Function hardwareInfo(ByVal value As Boolean) As InitFieldsPresentEncoder
			Dim bits As SByte = buffer.getByte(offset)
			bits = CSByte(If(value, bits Or (1 << 1), bits And Not (1 << 1)))
			buffer.putByte(offset, bits)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public InitFieldsPresentEncoder modelInfo(final boolean value)
		Public Overridable Function modelInfo(ByVal value As Boolean) As InitFieldsPresentEncoder
			Dim bits As SByte = buffer.getByte(offset)
			bits = CSByte(If(value, bits Or (1 << 2), bits And Not (1 << 2)))
			buffer.putByte(offset, bits)
			Return Me
		End Function
	End Class

End Namespace