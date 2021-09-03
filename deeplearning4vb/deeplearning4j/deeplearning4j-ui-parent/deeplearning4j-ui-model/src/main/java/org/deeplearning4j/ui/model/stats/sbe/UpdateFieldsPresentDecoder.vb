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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.UpdateFieldsPresentDecoder"}) @SuppressWarnings("all") public class UpdateFieldsPresentDecoder
	Public Class UpdateFieldsPresentDecoder
		Public Const ENCODED_LENGTH As Integer = 4
		Private buffer As DirectBuffer
		Private offset As Integer

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public UpdateFieldsPresentDecoder wrap(final org.agrona.DirectBuffer buffer, final int offset)
		Public Overridable Function wrap(ByVal buffer As DirectBuffer, ByVal offset As Integer) As UpdateFieldsPresentDecoder
			Me.buffer = buffer
			Me.offset = offset

			Return Me
		End Function

		Public Overridable Function encodedLength() As Integer
			Return ENCODED_LENGTH
		End Function

		Public Overridable Function score() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 0))
		End Function

		Public Overridable Function memoryUse() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 1))
		End Function

		Public Overridable Function performance() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 2))
		End Function

		Public Overridable Function garbageCollection() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 3))
		End Function

		Public Overridable Function histogramParameters() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 4))
		End Function

		Public Overridable Function histogramGradients() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 5))
		End Function

		Public Overridable Function histogramUpdates() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 6))
		End Function

		Public Overridable Function histogramActivations() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 7))
		End Function

		Public Overridable Function meanParameters() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 8))
		End Function

		Public Overridable Function meanGradients() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 9))
		End Function

		Public Overridable Function meanUpdates() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 10))
		End Function

		Public Overridable Function meanActivations() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 11))
		End Function

		Public Overridable Function stdevParameters() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 12))
		End Function

		Public Overridable Function stdevGradients() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 13))
		End Function

		Public Overridable Function stdevUpdates() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 14))
		End Function

		Public Overridable Function stdevActivations() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 15))
		End Function

		Public Overridable Function meanMagnitudeParameters() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 16))
		End Function

		Public Overridable Function meanMagnitudeGradients() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 17))
		End Function

		Public Overridable Function meanMagnitudeUpdates() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 18))
		End Function

		Public Overridable Function meanMagnitudeActivations() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 19))
		End Function

		Public Overridable Function learningRatesPresent() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 20))
		End Function

		Public Overridable Function dataSetMetaDataPresent() As Boolean
			Return 0 <> (buffer.getInt(offset, java.nio.ByteOrder.LITTLE_ENDIAN) And (1 << 21))
		End Function

		Public Overrides Function ToString() As String
			Return appendTo(New StringBuilder(100)).ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public StringBuilder appendTo(final StringBuilder builder)
		Public Overridable Function appendTo(ByVal builder As StringBuilder) As StringBuilder
			builder.Append("{"c)
			Dim atLeastOne As Boolean = False
			If score() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("score")
				atLeastOne = True
			End If
			If memoryUse() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("memoryUse")
				atLeastOne = True
			End If
			If performance() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("performance")
				atLeastOne = True
			End If
			If garbageCollection() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("garbageCollection")
				atLeastOne = True
			End If
			If histogramParameters() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("histogramParameters")
				atLeastOne = True
			End If
			If histogramGradients() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("histogramGradients")
				atLeastOne = True
			End If
			If histogramUpdates() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("histogramUpdates")
				atLeastOne = True
			End If
			If histogramActivations() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("histogramActivations")
				atLeastOne = True
			End If
			If meanParameters() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanParameters")
				atLeastOne = True
			End If
			If meanGradients() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanGradients")
				atLeastOne = True
			End If
			If meanUpdates() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanUpdates")
				atLeastOne = True
			End If
			If meanActivations() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanActivations")
				atLeastOne = True
			End If
			If stdevParameters() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("stdevParameters")
				atLeastOne = True
			End If
			If stdevGradients() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("stdevGradients")
				atLeastOne = True
			End If
			If stdevUpdates() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("stdevUpdates")
				atLeastOne = True
			End If
			If stdevActivations() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("stdevActivations")
				atLeastOne = True
			End If
			If meanMagnitudeParameters() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanMagnitudeParameters")
				atLeastOne = True
			End If
			If meanMagnitudeGradients() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanMagnitudeGradients")
				atLeastOne = True
			End If
			If meanMagnitudeUpdates() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanMagnitudeUpdates")
				atLeastOne = True
			End If
			If meanMagnitudeActivations() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("meanMagnitudeActivations")
				atLeastOne = True
			End If
			If learningRatesPresent() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("learningRatesPresent")
				atLeastOne = True
			End If
			If dataSetMetaDataPresent() Then
				If atLeastOne Then
					builder.Append(","c)
				End If
				builder.Append("dataSetMetaDataPresent")
				atLeastOne = True
			End If
			builder.Append("}"c)

			Return builder
		End Function
	End Class

End Namespace