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

	Public Class [Or]
		Implements Condition

'JAVA TO VB CONVERTER NOTE: The variable conditions was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private conditions_Conflict() As Condition

'JAVA TO VB CONVERTER NOTE: The parameter conditions was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Sub New(ParamArray ByVal conditions_Conflict() As Condition)
			Me.conditions_Conflict = conditions_Conflict
		End Sub


		''' <summary>
		''' Returns condition ID for native side
		''' 
		''' @return
		''' </summary>
		Public Overridable Function condtionNum() As Integer Implements Condition.condtionNum
			Return -1
		End Function

		Public Overridable ReadOnly Property Value As Double Implements Condition.getValue
			Get
				Return -1
			End Get
		End Property

		Public Overridable Function apply(ByVal input As Number) As Boolean?
			Dim ret As Boolean = conditions_Conflict(0).apply(input)
			For i As Integer = 1 To conditions_Conflict.Length - 1
				ret = ret OrElse conditions_Conflict(i).apply(input)
			Next i
			Return ret
		End Function

		Public Overridable Function epsThreshold() As Double Implements Condition.epsThreshold
			Return 0
		End Function
	End Class

End Namespace