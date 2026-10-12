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

	Public Class [Not]
		Implements Condition

		Private opposite As Condition


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

		Public Sub New(ByVal condition As Condition)
			Me.opposite = condition
		End Sub

		Public Overridable Function apply(ByVal input As Number) As Boolean?
			Return Not opposite.apply(input)
		End Function

		Public Overridable Function epsThreshold() As Double Implements Condition.epsThreshold
			Return 0
		End Function
	End Class

End Namespace