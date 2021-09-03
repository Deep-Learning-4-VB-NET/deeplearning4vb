Imports System.Collections.Generic
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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

Namespace org.nd4j.parameterserver.distributed.v2.enums

	Public NotInheritable Class TransmissionStatus
		Public Shared ReadOnly UNKNOWN As New TransmissionStatus("UNKNOWN", InnerEnum.UNKNOWN)
		Public Shared ReadOnly OK As New TransmissionStatus("OK", InnerEnum.OK)
		Public Shared ReadOnly NOT_CONNECTED As New TransmissionStatus("NOT_CONNECTED", InnerEnum.NOT_CONNECTED)
		Public Shared ReadOnly BACK_PRESSURED As New TransmissionStatus("BACK_PRESSURED", InnerEnum.BACK_PRESSURED)
		Public Shared ReadOnly ADMIN_ACTION As New TransmissionStatus("ADMIN_ACTION", InnerEnum.ADMIN_ACTION)
		Public Shared ReadOnly CLOSED As New TransmissionStatus("CLOSED", InnerEnum.CLOSED)
		Public Shared ReadOnly MAX_POSITION_EXCEEDED As New TransmissionStatus("MAX_POSITION_EXCEEDED", InnerEnum.MAX_POSITION_EXCEEDED)

		Private Shared ReadOnly valueList As New List(Of TransmissionStatus)()

		Shared Sub New()
			valueList.Add(UNKNOWN)
			valueList.Add(OK)
			valueList.Add(NOT_CONNECTED)
			valueList.Add(BACK_PRESSURED)
			valueList.Add(ADMIN_ACTION)
			valueList.Add(CLOSED)
			valueList.Add(MAX_POSITION_EXCEEDED)
		End Sub

		Public Enum InnerEnum
			UNKNOWN
			OK
			NOT_CONNECTED
			BACK_PRESSURED
			ADMIN_ACTION
			CLOSED
			MAX_POSITION_EXCEEDED
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


		Public Shared Function fromLong(ByVal value As Long) As TransmissionStatus
			If value = -1 Then
				Return NOT_CONNECTED
			ElseIf value = -2 Then
				Return BACK_PRESSURED
			ElseIf value = -3 Then
				Return ADMIN_ACTION
			ElseIf value = -4 Then
				Return CLOSED
			ElseIf value = -5 Then
				Return MAX_POSITION_EXCEEDED
			ElseIf value < 0 Then
				Throw New ND4JIllegalStateException("Unknown status returned: [" & value & "]")
			else
			If True Then
				Return OK
			End If
		End Function

		Public Shared Function values() As TransmissionStatus()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As TransmissionStatus, ByVal two As TransmissionStatus) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As TransmissionStatus, ByVal two As TransmissionStatus) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As TransmissionStatus
			For Each enumInstance As TransmissionStatus In TransmissionStatus.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace