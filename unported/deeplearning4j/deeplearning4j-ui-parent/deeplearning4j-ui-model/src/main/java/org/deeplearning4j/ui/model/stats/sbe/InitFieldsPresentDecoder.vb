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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.InitFieldsPresentDecoder"}) @SuppressWarnings("all") public class InitFieldsPresentDecoder
	Public Class InitFieldsPresentDecoder
		Public Const ENCODED_LENGTH As Integer = 1
		Private buffer As DirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public InitFieldsPresentDecoder wrap(final org.agrona.DirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer) As InitFieldsPresentDecoder
			Me.buffer = buffer
			Me.offset = offset

			Return Me
		End Function

		Public Overridable Function encodedLength() As Integer
			Return ENCODED_LENGTH
		End Function

		Public Overridable Function softwareInfo() As Boolean
			Return 0 <> (buffer.getByte(offset) And (1 << 0))
		End Function

		Public Overridable Function hardwareInfo() As Boolean
			Return 0 <> (buffer.getByte(offset) And (1 << 1))
		End Function

		Public Overridable Function modelInfo() As Boolean
			Return 0 <> (buffer.getByte(offset) And (1 << 2))
		End Function

		Public Overrides Function ToString() As String
			Return appendTo(New StringBuilder(100)).ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
		Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
			builder.Append("{"c)
			Dim atLeastOne As Boolean = False
			If softwareInfo() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("softwareInfo")
				atLeastOne = True
			End If
			If hardwareInfo() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("hardwareInfo")
				atLeastOne = True
			End If
			If modelInfo() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("modelInfo")
				atLeastOne = True
			End If
			builder.Append("}"c)

			Return builder
		End Function
	End Class

End Namespace