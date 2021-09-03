Imports System.Collections.Generic

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

Namespace org.deeplearning4j.nn.conf

	Public NotInheritable Class CNN2DFormat Implements DataFormat
		Public Shared ReadOnly NCHW As New CNN2DFormat("NCHW", InnerEnum.NCHW)
		Public Shared ReadOnly NHWC As New CNN2DFormat("NHWC", InnerEnum.NHWC)

		Private Shared ReadOnly valueList As New List(Of CNN2DFormat)()

		Shared Sub New()
			valueList.Add(NCHW)
			valueList.Add(NHWC)
		End Sub

		Public Enum InnerEnum
			NCHW
			NHWC
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
			nameValue = name
			ordinalValue = nextOrdinal
			nextOrdinal += 1
			innerEnumValue = thisInnerEnumValue
		End Sub

		''' <summary>
		''' Returns a string that explains the dimensions:<br>
		''' NCHW -> returns "[minibatch, channels, height, width]"<br>
		''' NHWC -> returns "[minibatch, height, width, channels]"
		''' </summary>
		Public Function dimensionNames() As String
			Select Case Me
				Case NCHW
					Return "[minibatch, channels, height, width]"
				Case NHWC
					Return "[minibatch, height, width, channels]"
				Case Else
					Throw New System.InvalidOperationException("Unknown enum: " & Me) 'Should never happen
			End Select
		End Function

		Public Shared Function values() As CNN2DFormat()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As CNN2DFormat, ByVal two As CNN2DFormat) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As CNN2DFormat, ByVal two As CNN2DFormat) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As CNN2DFormat
			For Each enumInstance As CNN2DFormat In CNN2DFormat.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace