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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.UpdateFieldsPresentEncoder"}) @SuppressWarnings("all") public class UpdateFieldsPresentEncoder
	Public Class UpdateFieldsPresentEncoder
		Public Const ENCODED_LENGTH As Integer = 4
		Private buffer As MutableDirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder wrap(final org.agrona.MutableDirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As MutableDirectBuffer, ByVal offset As Integer) As UpdateFieldsPresentEncoder
			Me.buffer = buffer
			Me.offset = offset

			Return Me
		End Function

		Public Overridable Function encodedLength() As Integer
			Return ENCODED_LENGTH
		End Function

		Public Overridable Function clear() As UpdateFieldsPresentEncoder
			buffer.putInt(offset, CInt(0L), java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder score(final boolean value)
		Public Overridable Function score(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 0), bits And Not (1 << 0))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder memoryUse(final boolean value)
		Public Overridable Function memoryUse(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 1), bits And Not (1 << 1))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder performance(final boolean value)
		Public Overridable Function performance(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 2), bits And Not (1 << 2))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder garbageCollection(final boolean value)
		Public Overridable Function garbageCollection(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 3), bits And Not (1 << 3))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder histogramParameters(final boolean value)
		Public Overridable Function histogramParameters(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 4), bits And Not (1 << 4))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder histogramGradients(final boolean value)
		Public Overridable Function histogramGradients(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 5), bits And Not (1 << 5))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder histogramUpdates(final boolean value)
		Public Overridable Function histogramUpdates(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 6), bits And Not (1 << 6))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder histogramActivations(final boolean value)
		Public Overridable Function histogramActivations(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 7), bits And Not (1 << 7))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanParameters(final boolean value)
		Public Overridable Function meanParameters(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 8), bits And Not (1 << 8))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanGradients(final boolean value)
		Public Overridable Function meanGradients(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 9), bits And Not (1 << 9))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanUpdates(final boolean value)
		Public Overridable Function meanUpdates(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 10), bits And Not (1 << 10))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanActivations(final boolean value)
		Public Overridable Function meanActivations(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 11), bits And Not (1 << 11))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder stdevParameters(final boolean value)
		Public Overridable Function stdevParameters(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 12), bits And Not (1 << 12))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder stdevGradients(final boolean value)
		Public Overridable Function stdevGradients(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 13), bits And Not (1 << 13))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder stdevUpdates(final boolean value)
		Public Overridable Function stdevUpdates(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 14), bits And Not (1 << 14))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder stdevActivations(final boolean value)
		Public Overridable Function stdevActivations(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 15), bits And Not (1 << 15))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanMagnitudeParameters(final boolean value)
		Public Overridable Function meanMagnitudeParameters(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 16), bits And Not (1 << 16))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanMagnitudeGradients(final boolean value)
		Public Overridable Function meanMagnitudeGradients(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 17), bits And Not (1 << 17))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanMagnitudeUpdates(final boolean value)
		Public Overridable Function meanMagnitudeUpdates(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 18), bits And Not (1 << 18))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder meanMagnitudeActivations(final boolean value)
		Public Overridable Function meanMagnitudeActivations(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 19), bits And Not (1 << 19))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder learningRatesPresent(final boolean value)
		Public Overridable Function learningRatesPresent(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 20), bits And Not (1 << 20))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentEncoder dataSetMetaDataPresent(final boolean value)
		Public Overridable Function dataSetMetaDataPresent(ByVal value As Boolean) As UpdateFieldsPresentEncoder
			Dim bits As Integer = buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN)
			bits = If(value, bits Or (1 << 21), bits And Not (1 << 21))
			buffer.putInt(offset, bits, java.nio.ByteOrder.LITTLE_ENDIAN)
			Return Me
		End Function
	End Class

End Namespace