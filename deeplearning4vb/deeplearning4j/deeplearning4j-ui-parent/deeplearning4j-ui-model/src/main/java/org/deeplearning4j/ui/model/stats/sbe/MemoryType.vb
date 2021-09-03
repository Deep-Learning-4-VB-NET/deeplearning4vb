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

Namespace org.deeplearning4j.ui.model.stats.sbe

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.MemoryType"}) public enum MemoryType
	Public NotInheritable Class MemoryType
		Public Shared ReadOnly JvmCurrent As New MemoryType("JvmCurrent", InnerEnum.JvmCurrent, CShort(0))
		Public Shared ReadOnly JvmMax As New MemoryType("JvmMax", InnerEnum.JvmMax, CShort(1))
		Public Shared ReadOnly OffHeapCurrent As New MemoryType("OffHeapCurrent", InnerEnum.OffHeapCurrent, CShort(2))
		Public Shared ReadOnly OffHeapMax As New MemoryType("OffHeapMax", InnerEnum.OffHeapMax, CShort(3))
		Public Shared ReadOnly DeviceCurrent As New MemoryType("DeviceCurrent", InnerEnum.DeviceCurrent, CShort(4))
		Public Shared ReadOnly DeviceMax As New MemoryType("DeviceMax", InnerEnum.DeviceMax, CShort(5))
		Public Shared ReadOnly NULL_VAL As New MemoryType("NULL_VAL", InnerEnum.NULL_VAL, CShort(255))

		Private Shared ReadOnly valueList As New List(Of MemoryType)()

		Shared Sub New()
			valueList.Add(JvmCurrent)
			valueList.Add(JvmMax)
			valueList.Add(OffHeapCurrent)
			valueList.Add(OffHeapMax)
			valueList.Add(DeviceCurrent)
			valueList.Add(DeviceMax)
			valueList.Add(NULL_VAL)
		End Sub

		Public Enum InnerEnum
			JvmCurrent
			JvmMax
			OffHeapCurrent
			OffHeapMax
			DeviceCurrent
			DeviceMax
			NULL_VAL
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private ReadOnly value As Short

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: MemoryType(final short value)
		Friend Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum, ByVal value As Short)
			Me.value = value

			nameValue = name
			ordinalValue = nextOrdinal
			nextOrdinal += 1
			innerEnumValue = thisInnerEnumValue
		End Sub

		Public Function value() As Short
			Return value
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static MemoryType get(final short value)
		Public Shared Function get(ByVal value As Short) As MemoryType
			Select Case value
				Case 0
					Return JvmCurrent
				Case 1
					Return JvmMax
				Case 2
					Return OffHeapCurrent
				Case 3
					Return OffHeapMax
				Case 4
					Return DeviceCurrent
				Case 5
					Return DeviceMax
			End Select

			If CShort(255) = value Then
				Return NULL_VAL
			End If

			Throw New System.ArgumentException("Unknown value: " & value)
		End Function

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