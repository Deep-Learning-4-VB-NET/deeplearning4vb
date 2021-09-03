Imports System
Imports Getter = lombok.Getter
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

	Public Class RobotLakeState
		Private ReadOnly size As Integer
		Private ReadOnly areStartingPositionsRandom As Boolean
		Private ReadOnly rnd As Random

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final RobotLakeMap lake;
		Private ReadOnly lake As RobotLakeMap

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int robotY, robotX;
		Private robotY, robotX As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int goalY, goalX;
		Private goalY, goalX As Integer

		Public Sub New(ByVal size As Integer, ByVal areStartingPositionsRandom As Boolean, ByVal rnd As Random)
			Me.size = size
			Me.areStartingPositionsRandom = areStartingPositionsRandom
			Me.rnd = rnd
			lake = New RobotLakeMap(size, rnd)
		End Sub

		Public Overridable Sub reset()
			setRobotAndGoalLocations()
			generateValidPond()
		End Sub

		Private Sub generateValidPond()
			Dim attempts As Integer = 0
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while (attempts++ < 1000)
			Do While attempts < 1000
					attempts += 1
				lake.generateLake(robotY, robotX, goalY, goalX)
				If RobotLakeHelper.pathExistsToGoal(lake, robotY, robotX) Then
					Return
				End If
			Loop
				attempts += 1

			Throw New Exception("Failed to generate a valid pond after 1000 attempts")
		End Sub

		Public Overridable Sub moveRobotLeft()
			If robotX > 0 Then
				robotX -= 1
			End If
		End Sub

		Public Overridable Sub moveRobotRight()
			If robotX < size - 1 Then
				robotX += 1
			End If
		End Sub

		Public Overridable Sub moveRobotUp()
			If robotY > 0 Then
				robotY -= 1
			End If
		End Sub

		Public Overridable Sub moveRobotDown()
			If robotY < size - 1 Then
				robotY += 1
			End If
		End Sub

		Private Sub setRobotAndGoalLocations()
			If areStartingPositionsRandom Then
				If rnd.nextBoolean() Then
					' Robot on top side, goal on bottom side
					robotX = rnd.nextInt(size)
					robotY = 0
					goalX = rnd.nextInt(size)
					goalY = size - 1
				Else
					' Robot on left side, goal on right side
					robotX = 0
					robotY = rnd.nextInt(size)
					goalX = size - 1
					goalY = rnd.nextInt(size)
				End If
			Else
				robotX = 0
				robotY = 0
				goalX = size - 1
				goalY = size - 1
			End If
		End Sub
	End Class
End Namespace