Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace

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

Namespace org.deeplearning4j.malmo

	Public MustInherit Class MalmoActionSpace
		Inherits DiscreteSpace

		''' <summary>
		''' Array of action strings that will be sent to Malmo
		''' </summary>
		Protected Friend actions() As String

		''' <summary>
		''' Protected constructor </summary>
		''' <param name="size"> number of discrete actions in this space </param>
		Protected Friend Sub New(ByVal size As Integer)
			MyBase.New(size)
		End Sub

		Public Overrides Function encode(ByVal action As Integer?) As Object
			Return actions(action)
		End Function

		Public Overrides Function noOp() As Integer?
			Return -1
		End Function
	End Class

End Namespace