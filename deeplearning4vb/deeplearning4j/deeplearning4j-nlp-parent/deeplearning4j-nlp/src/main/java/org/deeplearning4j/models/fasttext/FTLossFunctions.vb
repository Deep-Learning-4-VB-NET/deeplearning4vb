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

	Public NotInheritable Class FTLossFunctions
		Public Shared ReadOnly HS As New FTLossFunctions("HS", InnerEnum.HS, "hs")
		Public Shared ReadOnly NS As New FTLossFunctions("NS", InnerEnum.NS, "ns")
		Public Shared ReadOnly SOFTMAX As New FTLossFunctions("SOFTMAX", InnerEnum.SOFTMAX, "softmax")

		Private Shared ReadOnly valueList As New List(Of FTLossFunctions)()

		Shared Sub New()
			valueList.Add(HS)
			valueList.Add(NS)
			valueList.Add(SOFTMAX)
		End Sub

		Public Enum InnerEnum
			HS
			NS
			SOFTMAX
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

		Public Shared Function values() As FTLossFunctions()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Shared Operator =(ByVal one As FTLossFunctions, ByVal two As FTLossFunctions) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As FTLossFunctions, ByVal two As FTLossFunctions) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As FTLossFunctions
			For Each enumInstance As FTLossFunctions In FTLossFunctions.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace