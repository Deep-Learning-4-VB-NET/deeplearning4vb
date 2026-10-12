Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.indexing.conditions

	Public Class ConditionBuilder

		Private soFar As Condition


'JAVA TO VB CONVERTER NOTE: The parameter conditions was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function [or](ParamArray ByVal conditions_Conflict() As Condition) As ConditionBuilder
			If soFar Is Nothing Then
				soFar = New [Or](conditions_Conflict)
			Else
				soFar = New [Or](ArrayUtil.combine(conditions_Conflict, New Condition() {soFar}))
			End If
			Return Me
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter conditions was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function [and](ParamArray ByVal conditions_Conflict() As Condition) As ConditionBuilder
			If soFar Is Nothing Then
				soFar = New [And](conditions_Conflict)
			Else
				soFar = New [And](ArrayUtil.combine(conditions_Conflict, New Condition() {soFar}))
			End If
			Return Me
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter conditions was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function eq(ParamArray ByVal conditions_Conflict() As Condition) As ConditionBuilder
			If soFar Is Nothing Then
				soFar = New ConditionEquals(conditions_Conflict)
			Else
				soFar = New ConditionEquals(ArrayUtil.combine(conditions_Conflict, New Condition() {soFar}))
			End If
			Return Me
		End Function

		Public Overridable Function [not]() As ConditionBuilder
			If soFar Is Nothing Then
				Throw New System.InvalidOperationException("No condition to take the opposite of")
			End If
			soFar = New [Not](soFar)
			Return Me
		End Function

		Public Overridable Function build() As Condition
			Return soFar
		End Function


	End Class

End Namespace