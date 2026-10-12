Imports System
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

Namespace org.deeplearning4j.nn.conf.memory

	Public NotInheritable Class MemoryType
		Public Shared ReadOnly PARAMETERS As New MemoryType("PARAMETERS", InnerEnum.PARAMETERS)
		Public Shared ReadOnly PARAMATER_GRADIENTS As New MemoryType("PARAMATER_GRADIENTS", InnerEnum.PARAMATER_GRADIENTS)
		Public Shared ReadOnly ACTIVATIONS As New MemoryType("ACTIVATIONS", InnerEnum.ACTIVATIONS)
		Public Shared ReadOnly ACTIVATION_GRADIENTS As New MemoryType("ACTIVATION_GRADIENTS", InnerEnum.ACTIVATION_GRADIENTS)
		Public Shared ReadOnly UPDATER_STATE As New MemoryType("UPDATER_STATE", InnerEnum.UPDATER_STATE)
		Public Shared ReadOnly WORKING_MEMORY_FIXED As New MemoryType("WORKING_MEMORY_FIXED", InnerEnum.WORKING_MEMORY_FIXED)
		Public Shared ReadOnly WORKING_MEMORY_VARIABLE As New MemoryType("WORKING_MEMORY_VARIABLE", InnerEnum.WORKING_MEMORY_VARIABLE)
		Public Shared ReadOnly CACHED_MEMORY_FIXED As New MemoryType("CACHED_MEMORY_FIXED", InnerEnum.CACHED_MEMORY_FIXED)
		Public Shared ReadOnly CACHED_MEMORY_VARIABLE As New MemoryType("CACHED_MEMORY_VARIABLE", InnerEnum.CACHED_MEMORY_VARIABLE)

		Private Shared ReadOnly valueList As New List(Of MemoryType)()

		Shared Sub New()
			valueList.Add(PARAMETERS)
			valueList.Add(PARAMATER_GRADIENTS)
			valueList.Add(ACTIVATIONS)
			valueList.Add(ACTIVATION_GRADIENTS)
			valueList.Add(UPDATER_STATE)
			valueList.Add(WORKING_MEMORY_FIXED)
			valueList.Add(WORKING_MEMORY_VARIABLE)
			valueList.Add(CACHED_MEMORY_FIXED)
			valueList.Add(CACHED_MEMORY_VARIABLE)
		End Sub

		Public Enum InnerEnum
			PARAMETERS
			PARAMATER_GRADIENTS
			ACTIVATIONS
			ACTIVATION_GRADIENTS
			UPDATER_STATE
			WORKING_MEMORY_FIXED
			WORKING_MEMORY_VARIABLE
			CACHED_MEMORY_FIXED
			CACHED_MEMORY_VARIABLE
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

		''' <returns> True, if the memory type is used during inference. False if the memory type is used only during training. </returns>
		Public ReadOnly Property Inference As Boolean
			Get
				Select Case Me
					Case PARAMETERS, ACTIVATIONS, WORKING_MEMORY_FIXED, WORKING_MEMORY_VARIABLE
						Return True
					Case PARAMATER_GRADIENTS, ACTIVATION_GRADIENTS, UPDATER_STATE, CACHED_MEMORY_FIXED, CACHED_MEMORY_VARIABLE
						Return False
				End Select
				Throw New Exception("Unknown memory type: " & Me)
			End Get
		End Property

		Public Shared Function values() As MemoryType()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As MemoryType, ByVal two As MemoryType) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As MemoryType, ByVal two As MemoryType) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As MemoryType
			For Each enumInstance As MemoryType In MemoryType.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace