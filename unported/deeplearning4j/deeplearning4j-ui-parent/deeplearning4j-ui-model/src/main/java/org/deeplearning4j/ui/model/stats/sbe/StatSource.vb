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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"StatSource"}) public enum StatSource
	Public NotInheritable Class StatSource
		Public Shared ReadOnly Parameters As New StatSource("Parameters", InnerEnum.Parameters, CShort(0))
		Public Shared ReadOnly Updates As New StatSource("Updates", InnerEnum.Updates, CShort(1))
		Public Shared ReadOnly Activations As New StatSource("Activations", InnerEnum.Activations, CShort(2))
		Public Shared ReadOnly NULL_VAL As New StatSource("NULL_VAL", InnerEnum.NULL_VAL, CShort(255))

		Private Shared ReadOnly valueList As New List(Of StatSource)()

		Shared Sub New()
			valueList.Add(Parameters)
			valueList.Add(Updates)
			valueList.Add(Activations)
			valueList.Add(NULL_VAL)
		End Sub

		Public Enum InnerEnum
			Parameters
			Updates
			Activations
			NULL_VAL
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private ReadOnly value As Short

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: StatSource(final short value)
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
'ORIGINAL LINE: public static StatSource get(final short value)
		Public Shared Function get(ByVal value As Short) As StatSource
			Select Case value
				Case 0
					Return Parameters
				Case 1
					Return Updates
				Case 2
					Return Activations
			End Select

			If CShort(255) = value Then
				Return NULL_VAL
			End If

			Throw New System.ArgumentException("Unknown value: " & value)
		End Function

		Public Shared Function values() As StatSource()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As StatSource, ByVal two As StatSource) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As StatSource, ByVal two As StatSource) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As StatSource
			For Each enumInstance As StatSource In StatSource.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace