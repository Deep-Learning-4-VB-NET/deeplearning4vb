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

Namespace org.deeplearning4j.models.fasttext

	Public NotInheritable Class FTModels
		Public Shared ReadOnly CBOW As New FTModels("CBOW", InnerEnum.CBOW, "cbow")
		Public Shared ReadOnly SG As New FTModels("SG", InnerEnum.SG, "sg")
		Public Shared ReadOnly SUP As New FTModels("SUP", InnerEnum.SUP, "sup")

		Private Shared ReadOnly valueList As New List(Of FTModels)()

		Shared Sub New()
			valueList.Add(CBOW)
			valueList.Add(SG)
			valueList.Add(SUP)
		End Sub

		Public Enum InnerEnum
			CBOW
			SG
			SUP
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private ReadOnly name As String

		Friend Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum, ByVal name As String)
			Me.name = name

			nameValue = name
			ordinalValue = nextOrdinal
			nextOrdinal += 1
			innerEnumValue = thisInnerEnumValue
		End Sub

		Public Overrides Function ToString() As String
			Return Me.name
		End Function

		Public Shared Function values() As FTModels()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Shared Operator =(ByVal one As FTModels, ByVal two As FTModels) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As FTModels, ByVal two As FTModels) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As FTModels
			For Each enumInstance As FTModels In FTModels.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace