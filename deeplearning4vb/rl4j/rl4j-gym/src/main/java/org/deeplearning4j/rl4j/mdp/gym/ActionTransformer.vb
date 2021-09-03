Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports HighLowDiscrete = org.deeplearning4j.rl4j.space.HighLowDiscrete

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

Namespace org.deeplearning4j.rl4j.mdp.gym


	Public Class ActionTransformer
		Inherits DiscreteSpace

		Private ReadOnly availableAction() As Integer
		Private ReadOnly hld As HighLowDiscrete

		Public Sub New(ByVal hld As HighLowDiscrete, ByVal availableAction() As Integer)
			MyBase.New(availableAction.Length)
			Me.hld = hld
			Me.availableAction = availableAction
		End Sub

		Public Overrides Function encode(ByVal a As Integer?) As Object
			Return hld.encode(availableAction(a))
		End Function
	End Class

End Namespace