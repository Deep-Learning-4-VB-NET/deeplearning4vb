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
'ORIGINAL LINE: @javax.annotation.Generated(value = {"org.deeplearning4j.ui.stats.sbe.SummaryType"}) public enum SummaryType
	Public NotInheritable Class SummaryType
		Public Shared ReadOnly Mean As New SummaryType("Mean", InnerEnum.Mean, CShort(0))
		Public Shared ReadOnly Stdev As New SummaryType("Stdev", InnerEnum.Stdev, CShort(1))
		Public Shared ReadOnly MeanMagnitude As New SummaryType("MeanMagnitude", InnerEnum.MeanMagnitude, CShort(2))
		Public Shared ReadOnly NULL_VAL As New SummaryType("NULL_VAL", InnerEnum.NULL_VAL, CShort(255))

		Private Shared ReadOnly valueList As New List(Of SummaryType)()

		Shared Sub New()
			valueList.Add(Mean)
			valueList.Add(Stdev)
			valueList.Add(MeanMagnitude)
			valueList.Add(NULL_VAL)
		End Sub

		Public Enum InnerEnum
			Mean
			Stdev
			MeanMagnitude
			NULL_VAL
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private ReadOnly value As Short

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: SummaryType(final short value)
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
'ORIGINAL LINE: public static SummaryType get(final short value)
		Public Shared Function get(ByVal value As Short) As SummaryType
			Select Case value
				Case 0
					Return Mean
				Case 1
					Return Stdev
				Case 2
					Return MeanMagnitude
			End Select

			If CShort(255) = value Then
				Return NULL_VAL
			End If

			Throw New System.ArgumentException("Unknown value: " & value)
		End Function

		Public Shared Function values() As SummaryType()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As SummaryType, ByVal two As SummaryType) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As SummaryType, ByVal two As SummaryType) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As SummaryType
			For Each enumInstance As SummaryType In SummaryType.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace