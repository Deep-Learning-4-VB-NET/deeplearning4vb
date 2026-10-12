Imports Random = org.nd4j.linalg.api.rng.Random

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
Namespace org.deeplearning4j.rl4j.mdp.robotlake

	Public Class RobotLakeMap
		Private Const SAFE_ICE_PROBABILITY As Double = 0.8

		Private ReadOnly rnd As Random

		Private ReadOnly lake()() As Char
		Public ReadOnly size As Integer

		Public Sub New(ByVal size As Integer, ByVal rnd As Random)
			Me.size = size
			Me.rnd = rnd
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: lake = new Char[size][size]
			lake = RectangularArrays.RectangularCharArray(size, size)
		End Sub

		Public Overridable Sub generateLake(ByVal playerY As Integer, ByVal playerX As Integer, ByVal goalY As Integer, ByVal goalX As Integer)
			For y As Integer = 0 To size - 1
				For x As Integer = 0 To size - 1
					lake(y)(x) = If(rnd.nextDouble() <= SAFE_ICE_PROBABILITY, RobotLake.ICE, RobotLake.HOLE)
				Next x
			Next y

			lake(goalY)(goalX) = RobotLake.GOAL
			lake(playerY)(playerX) = RobotLake.ICE
		End Sub

		Public Overridable Function getLocation(ByVal y As Integer, ByVal x As Integer) As Char
			Return lake(y)(x)
		End Function
	End Class

End Namespace