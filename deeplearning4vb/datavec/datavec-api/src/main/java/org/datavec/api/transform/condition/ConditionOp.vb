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

Namespace org.datavec.api.transform.condition


	Public NotInheritable Class ConditionOp
		Public Shared ReadOnly LessThan As New ConditionOp("LessThan", InnerEnum.LessThan)
		Public Shared ReadOnly LessOrEqual As New ConditionOp("LessOrEqual", InnerEnum.LessOrEqual)
		Public Shared ReadOnly GreaterThan As New ConditionOp("GreaterThan", InnerEnum.GreaterThan)
		Public Shared ReadOnly GreaterOrEqual As New ConditionOp("GreaterOrEqual", InnerEnum.GreaterOrEqual)
		Public Shared ReadOnly Equal As New ConditionOp("Equal", InnerEnum.Equal)
		Public Shared ReadOnly NotEqual As New ConditionOp("NotEqual", InnerEnum.NotEqual)
		Public Shared ReadOnly InSet As New ConditionOp("InSet", InnerEnum.InSet)
		Public Shared ReadOnly NotInSet As New ConditionOp("NotInSet", InnerEnum.NotInSet)

		Private Shared ReadOnly valueList As New List(Of ConditionOp)()

		Shared Sub New()
			valueList.Add(LessThan)
			valueList.Add(LessOrEqual)
			valueList.Add(GreaterThan)
			valueList.Add(GreaterOrEqual)
			valueList.Add(Equal)
			valueList.Add(NotEqual)
			valueList.Add(InSet)
			valueList.Add(NotInSet)
		End Sub

		Public Enum InnerEnum
			LessThan
			LessOrEqual
			GreaterThan
			GreaterOrEqual
			Equal
			NotEqual
			InSet
			NotInSet
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


		Public Function apply(ByVal x As Double, ByVal value As Double, ByVal set As ISet(Of Double)) As Boolean
			Select Case Me
				Case LessThan
					Return x < value
				Case LessOrEqual
					Return x <= value
				Case GreaterThan
					Return x > value
				Case GreaterOrEqual
					Return x >= value
				Case Equal
					Return x = value
				Case NotEqual
					Return x <> value
				Case InSet
					Return set.Contains(x)
				Case NotInSet
					Return Not set.Contains(x)
				Case Else
					Throw New Exception("Unknown or not implemented op: " & Me)
			End Select
		End Function


		Public Function apply(ByVal x As Single, ByVal value As Single, ByVal set As ISet(Of Single)) As Boolean
			Select Case Me
				Case LessThan
					Return x < value
				Case LessOrEqual
					Return x <= value
				Case GreaterThan
					Return x > value
				Case GreaterOrEqual
					Return x >= value
				Case Equal
					Return x = value
				Case NotEqual
					Return x <> value
				Case InSet
					Return set.Contains(x)
				Case NotInSet
					Return Not set.Contains(x)
				Case Else
					Throw New Exception("Unknown or not implemented op: " & Me)
			End Select
		End Function

		Public Function apply(ByVal x As Integer, ByVal value As Integer, ByVal set As ISet(Of Integer)) As Boolean
			Select Case Me
				Case LessThan
					Return x < value
				Case LessOrEqual
					Return x <= value
				Case GreaterThan
					Return x > value
				Case GreaterOrEqual
					Return x >= value
				Case Equal
					Return x = value
				Case NotEqual
					Return x <> value
				Case InSet
					Return set.Contains(x)
				Case NotInSet
					Return Not set.Contains(x)
				Case Else
					Throw New Exception("Unknown or not implemented op: " & Me)
			End Select
		End Function

		Public Function apply(ByVal x As Long, ByVal value As Long, ByVal set As ISet(Of Long)) As Boolean
			Select Case Me
				Case LessThan
					Return x < value
				Case LessOrEqual
					Return x <= value
				Case GreaterThan
					Return x > value
				Case GreaterOrEqual
					Return x >= value
				Case Equal
					Return x = value
				Case NotEqual
					Return x <> value
				Case InSet
					Return set.Contains(x)
				Case NotInSet
					Return Not set.Contains(x)
				Case Else
					Throw New Exception("Unknown or not implemented op: " & Me)
			End Select
		End Function


		Public Function apply(ByVal x As String, ByVal value As String, ByVal set As ISet(Of String)) As Boolean
			Select Case Me
				Case Equal
					Return value.Equals(x)
				Case NotEqual
					Return Not value.Equals(x)
				Case InSet
					Return set.Contains(x)
				Case NotInSet
					Return Not set.Contains(x)
				Case LessThan, LessOrEqual, GreaterThan, GreaterOrEqual
					Throw New System.NotSupportedException("Cannot use ConditionOp """ & Me & """ on Strings")
				Case Else
					Throw New Exception("Unknown or not implemented op: " & Me)
			End Select
		End Function




		Public Shared Function values() As ConditionOp()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As ConditionOp, ByVal two As ConditionOp) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As ConditionOp, ByVal two As ConditionOp) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As ConditionOp
			For Each enumInstance As ConditionOp In ConditionOp.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace