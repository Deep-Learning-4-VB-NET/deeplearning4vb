Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

	Public MustInherit Class BaseCondition
		Implements Condition

		Public MustOverride Function apply(ByVal input As Number) As Boolean Implements Condition.apply
		Public MustOverride Function condtionNum() As Integer Implements Condition.condtionNum
'JAVA TO VB CONVERTER NOTE: The field value was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend value_Conflict As Number

		Public Sub New(ByVal value As Number)
			Me.value_Conflict = value
		End Sub

		Public Overridable Property Value Implements Condition.setValue As Number
			Set(ByVal value As Number)
				Me.value_Conflict = value
			End Set
			Get
				Return value_Conflict.doubleValue()
			End Get
		End Property

		Public Overridable Function epsThreshold() As Double Implements Condition.epsThreshold
			Return Nd4j.EPS_THRESHOLD
		End Function


	End Class

End Namespace