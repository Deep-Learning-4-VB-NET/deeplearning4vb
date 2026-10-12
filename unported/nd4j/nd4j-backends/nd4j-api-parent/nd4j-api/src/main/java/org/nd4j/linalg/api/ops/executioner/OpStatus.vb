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

Namespace org.nd4j.linalg.api.ops.executioner

	Public NotInheritable Class OpStatus
		Public Shared ReadOnly ND4J_STATUS_OK As New OpStatus("ND4J_STATUS_OK", InnerEnum.ND4J_STATUS_OK)
		Public Shared ReadOnly ND4J_STATUS_BAD_INPUT As New OpStatus("ND4J_STATUS_BAD_INPUT", InnerEnum.ND4J_STATUS_BAD_INPUT)
		Public Shared ReadOnly ND4J_STATUS_BAD_SHAPE As New OpStatus("ND4J_STATUS_BAD_SHAPE", InnerEnum.ND4J_STATUS_BAD_SHAPE)
		Public Shared ReadOnly ND4J_STATUS_BAD_RANK As New OpStatus("ND4J_STATUS_BAD_RANK", InnerEnum.ND4J_STATUS_BAD_RANK)
		Public Shared ReadOnly ND4J_STATUS_BAD_PARAMS As New OpStatus("ND4J_STATUS_BAD_PARAMS", InnerEnum.ND4J_STATUS_BAD_PARAMS)
		Public Shared ReadOnly ND4J_STATUS_BAD_OUTPUT As New OpStatus("ND4J_STATUS_BAD_OUTPUT", InnerEnum.ND4J_STATUS_BAD_OUTPUT)
		Public Shared ReadOnly ND4J_STATUS_BAD_RNG As New OpStatus("ND4J_STATUS_BAD_RNG", InnerEnum.ND4J_STATUS_BAD_RNG)
		Public Shared ReadOnly ND4J_STATUS_BAD_EPSILON As New OpStatus("ND4J_STATUS_BAD_EPSILON", InnerEnum.ND4J_STATUS_BAD_EPSILON)
		Public Shared ReadOnly ND4J_STATUS_BAD_GRADIENTS As New OpStatus("ND4J_STATUS_BAD_GRADIENTS", InnerEnum.ND4J_STATUS_BAD_GRADIENTS)
		Public Shared ReadOnly ND4J_STATUS_BAD_BIAS As New OpStatus("ND4J_STATUS_BAD_BIAS", InnerEnum.ND4J_STATUS_BAD_BIAS)

		Public Shared ReadOnly ND4J_STATUS_BAD_GRAPH As New OpStatus("ND4J_STATUS_BAD_GRAPH", InnerEnum.ND4J_STATUS_BAD_GRAPH)
		Public Shared ReadOnly ND4J_STATUS_BAD_LENGTH As New OpStatus("ND4J_STATUS_BAD_LENGTH", InnerEnum.ND4J_STATUS_BAD_LENGTH)
		Public Shared ReadOnly ND4J_STATUS_BAD_DIMENSIONS As New OpStatus("ND4J_STATUS_BAD_DIMENSIONS", InnerEnum.ND4J_STATUS_BAD_DIMENSIONS)
		Public Shared ReadOnly ND4J_STATUS_BAD_ORDER As New OpStatus("ND4J_STATUS_BAD_ORDER", InnerEnum.ND4J_STATUS_BAD_ORDER)
		Public Shared ReadOnly ND4J_STATUS_BAD_ARGUMENTS As New OpStatus("ND4J_STATUS_BAD_ARGUMENTS", InnerEnum.ND4J_STATUS_BAD_ARGUMENTS)
		Public Shared ReadOnly ND4J_STATUS_VALIDATION As New OpStatus("ND4J_STATUS_VALIDATION", InnerEnum.ND4J_STATUS_VALIDATION)

		Private Shared ReadOnly valueList As New List(Of OpStatus)()

		Shared Sub New()
			valueList.Add(ND4J_STATUS_OK)
			valueList.Add(ND4J_STATUS_BAD_INPUT)
			valueList.Add(ND4J_STATUS_BAD_SHAPE)
			valueList.Add(ND4J_STATUS_BAD_RANK)
			valueList.Add(ND4J_STATUS_BAD_PARAMS)
			valueList.Add(ND4J_STATUS_BAD_OUTPUT)
			valueList.Add(ND4J_STATUS_BAD_RNG)
			valueList.Add(ND4J_STATUS_BAD_EPSILON)
			valueList.Add(ND4J_STATUS_BAD_GRADIENTS)
			valueList.Add(ND4J_STATUS_BAD_BIAS)
			valueList.Add(ND4J_STATUS_BAD_GRAPH)
			valueList.Add(ND4J_STATUS_BAD_LENGTH)
			valueList.Add(ND4J_STATUS_BAD_DIMENSIONS)
			valueList.Add(ND4J_STATUS_BAD_ORDER)
			valueList.Add(ND4J_STATUS_BAD_ARGUMENTS)
			valueList.Add(ND4J_STATUS_VALIDATION)
		End Sub

		Public Enum InnerEnum
			ND4J_STATUS_OK
			ND4J_STATUS_BAD_INPUT
			ND4J_STATUS_BAD_SHAPE
			ND4J_STATUS_BAD_RANK
			ND4J_STATUS_BAD_PARAMS
			ND4J_STATUS_BAD_OUTPUT
			ND4J_STATUS_BAD_RNG
			ND4J_STATUS_BAD_EPSILON
			ND4J_STATUS_BAD_GRADIENTS
			ND4J_STATUS_BAD_BIAS
			ND4J_STATUS_BAD_GRAPH
			ND4J_STATUS_BAD_LENGTH
			ND4J_STATUS_BAD_DIMENSIONS
			ND4J_STATUS_BAD_ORDER
			ND4J_STATUS_BAD_ARGUMENTS
			ND4J_STATUS_VALIDATION
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

		Public Shared Function byNumber(ByVal val As Integer) As OpStatus
			Select Case val
				Case 0
					Return ND4J_STATUS_OK
				Case 1
					Return ND4J_STATUS_BAD_INPUT
				Case 2
					Return ND4J_STATUS_BAD_SHAPE
				Case 3
					Return ND4J_STATUS_BAD_RANK
				Case 4
					Return ND4J_STATUS_BAD_PARAMS
				Case 5
					Return ND4J_STATUS_BAD_OUTPUT
				Case 6
					Return ND4J_STATUS_BAD_RNG
				Case 7
					Return ND4J_STATUS_BAD_EPSILON
				Case 8
					Return ND4J_STATUS_BAD_GRADIENTS
				Case 9
					Return ND4J_STATUS_BAD_BIAS
				Case 20
					Return ND4J_STATUS_VALIDATION
				Case 30
					Return ND4J_STATUS_BAD_GRAPH
				Case 31
					Return ND4J_STATUS_BAD_LENGTH
				Case 32
					Return ND4J_STATUS_BAD_DIMENSIONS
				Case 33
					Return ND4J_STATUS_BAD_ORDER
				Case 34
					Return ND4J_STATUS_BAD_ARGUMENTS
				Case Else
					Throw New ND4JIllegalStateException("Unknown status given: " & val)
			End Select
		End Function

		Public Shared Function values() As OpStatus()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As OpStatus, ByVal two As OpStatus) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As OpStatus, ByVal two As OpStatus) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As OpStatus
			For Each enumInstance As OpStatus In OpStatus.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace