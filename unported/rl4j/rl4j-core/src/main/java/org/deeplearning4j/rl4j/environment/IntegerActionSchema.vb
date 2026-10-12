Imports Getter = lombok.Getter
Imports Random = org.nd4j.linalg.api.rng.Random
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
Namespace org.deeplearning4j.rl4j.environment

	' Work in progress
	Public Class IntegerActionSchema
		Implements IActionSchema(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final int actionSpaceSize;
		Private ReadOnly actionSpaceSize As Integer

		Private ReadOnly noOpAction As Integer
		Private ReadOnly rnd As Random

		Public Sub New(ByVal numActions As Integer, ByVal noOpAction As Integer)
			Me.New(numActions, noOpAction, Nd4j.Random)
		End Sub

		Public Sub New(ByVal numActions As Integer, ByVal noOpAction As Integer, ByVal rnd As Random)
			Me.actionSpaceSize = numActions
			Me.noOpAction = noOpAction
			Me.rnd = rnd
		End Sub

		Public Overridable ReadOnly Property NoOp As Integer?
			Get
				Return noOpAction
			End Get
		End Property

		Public Overridable ReadOnly Property RandomAction As Integer?
			Get
				Return rnd.nextInt(actionSpaceSize)
			End Get
		End Property
	End Class

End Namespace