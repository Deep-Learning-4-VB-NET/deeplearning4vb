Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

	Public Class EqualsCondition
		Inherits BaseCondition

		''' <summary>
		''' Special constructor for pairwise boolean operations.
		''' </summary>
		Public Sub New()
			MyBase.New(0.0)
		End Sub

		Public Sub New(ByVal value As Number)
			MyBase.New(value)
		End Sub

		Public Overrides WriteOnly Property Value As Number
			Set(ByVal value As Number)
				'no op where we can pass values in
			End Set
		End Property

		''' <summary>
		''' Returns condition ID for native side
		''' Condition number is affected by:
		''' https://github.com/eclipse/deeplearning4j/blob/0ba0f933a95d2dceeff3651bc540d03b5f3b1631/libnd4j/include/ops/ops.h#L2253
		''' 
		''' @return
		''' </summary>
		Public Overrides Function condtionNum() As Integer
			Return 0
		End Function

		Public Overrides Function apply(ByVal input As Number) As Boolean?
			If Nd4j.dataType() = DataType.DOUBLE Then
				Return input.doubleValue() = value_Conflict.doubleValue()
			Else
				Return input.floatValue() = value_Conflict.floatValue()
			End If
		End Function
	End Class

End Namespace