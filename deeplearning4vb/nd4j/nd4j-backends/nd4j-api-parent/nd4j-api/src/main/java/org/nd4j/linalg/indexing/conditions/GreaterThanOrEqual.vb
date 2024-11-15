﻿'
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

	Public Class GreaterThanOrEqual
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
		''' 
		''' @return
		''' </summary>
		Public Overrides Function condtionNum() As Integer
			Return 5
		End Function

		Public Overrides Function apply(ByVal input As Number) As Boolean?
			Return input.floatValue() >= value_Conflict.floatValue()
		End Function
	End Class

End Namespace