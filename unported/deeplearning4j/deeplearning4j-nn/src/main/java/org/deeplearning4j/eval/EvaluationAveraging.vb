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

Namespace org.deeplearning4j.eval

	<Obsolete>
	Public NotInheritable Class EvaluationAveraging
		Public Shared ReadOnly Macro As New EvaluationAveraging("Macro", InnerEnum.Macro)
		Public Shared ReadOnly Micro As New EvaluationAveraging("Micro", InnerEnum.Micro)

		Private Shared ReadOnly valueList As New List(Of EvaluationAveraging)()

		Shared Sub New()
			valueList.Add(Macro)
			valueList.Add(Micro)
		End Sub

		Public Enum InnerEnum
			Macro
			Micro
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

		Public Function toNd4j() As org.nd4j.evaluation.EvaluationAveraging
			Select Case Me
				Case Macro
					Return org.nd4j.evaluation.EvaluationAveraging.Macro
				Case Micro
					Return org.nd4j.evaluation.EvaluationAveraging.Micro
			End Select
			Throw New System.NotSupportedException("Unknown: " & Me)
		End Function

		Public Shared Function values() As EvaluationAveraging()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As EvaluationAveraging, ByVal two As EvaluationAveraging) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As EvaluationAveraging, ByVal two As EvaluationAveraging) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As EvaluationAveraging
			For Each enumInstance As EvaluationAveraging In EvaluationAveraging.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace