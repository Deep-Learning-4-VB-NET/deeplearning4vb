Imports System.Collections.Generic
Imports Button = Global.vizdoom.Button

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

Namespace org.deeplearning4j.rl4j.mdp.vizdoom


	Public Class Basic
		Inherits VizDoom

		Public Sub New(ByVal render As Boolean)
			MyBase.New(render)
		End Sub

		Public Overrides ReadOnly Property Configuration As Configuration
			Get
    
				Dim buttons As IList(Of Button) = New List(Of Button) From {Button.ATTACK, Button.MOVE_LEFT, Button.MOVE_RIGHT}
    
				Return New Configuration("basic", -0.01, 1, 0, 700, 0, buttons)
			End Get
		End Property

		Public Overrides Function newInstance() As Basic
			Return New Basic(isRender())
		End Function
	End Class

End Namespace