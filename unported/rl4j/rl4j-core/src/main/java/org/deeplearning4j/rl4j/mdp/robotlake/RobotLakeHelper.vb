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

	Public Class RobotLakeHelper
		Private Shared ReadOnly SAFE_PATH_TO_LOCATION_EXISTS As SByte = CSByte(1)
		Private Shared ReadOnly DANGEROUS_LOCATION As SByte = CSByte(-1)
		Private Shared ReadOnly UNVISITED_LOCATION As SByte = CSByte(0)

		Public Shared Function isGoalAtLocation(ByVal lake As RobotLakeMap, ByVal y As Integer, ByVal x As Integer) As Boolean
			Return lake.getLocation(y, x) = RobotLake.GOAL
		End Function

		Public Shared Function pathExistsToGoal(ByVal lake As RobotLakeMap, ByVal startY As Integer, ByVal startX As Integer) As Boolean
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim path[][] As SByte = new SByte[lake.size][lake.size]
			Dim path()() As SByte = RectangularArrays.RectangularSByteArray(lake.size, lake.size)

			For y As Integer = 0 To lake.size - 1
				For x As Integer = 0 To lake.size - 1
					If Not isLocationSafe(lake, y, x) Then
						path(y)(x) = DANGEROUS_LOCATION
					End If
				Next x
			Next y

			path(startY)(startX) = 1
			Dim previousNumberOfLocations As Integer = 0
			Do
				Dim numberOfLocations As Integer = 0

				For y As Integer = 0 To lake.size - 1
					For x As Integer = 0 To lake.size - 1
						If path(y)(x) = SAFE_PATH_TO_LOCATION_EXISTS Then
							numberOfLocations += 1
							Dim hasFoundValidPath As Boolean = updatePathSafetyAtLocation(lake, path, y - 1, x) OrElse updatePathSafetyAtLocation(lake, path, y, x - 1) OrElse updatePathSafetyAtLocation(lake, path, y + 1, x) OrElse updatePathSafetyAtLocation(lake, path, y, x + 1)

							If hasFoundValidPath Then
								Return True
							End If
						End If
					Next x
				Next y

				If previousNumberOfLocations = numberOfLocations Then
					Return False
				End If
				previousNumberOfLocations = numberOfLocations
			Loop
		End Function

		' returns true if goal has been reached
		Private Shared Function updatePathSafetyAtLocation(ByVal lake As RobotLakeMap, ByVal path()() As SByte, ByVal y As Integer, ByVal x As Integer) As Boolean
			If y < 0 OrElse y >= path.Length OrElse x < 0 OrElse x >= path.Length OrElse path(y)(x) <> UNVISITED_LOCATION Then
				Return False
			End If

			If isGoalAtLocation(lake, y, x) Then
				Return True
			End If

			path(y)(x) = If(isLocationSafe(lake, y, x), SAFE_PATH_TO_LOCATION_EXISTS, DANGEROUS_LOCATION)

			Return False
		End Function

		Public Shared Function isLocationSafe(ByVal lake As RobotLakeMap, ByVal y As Integer, ByVal x As Integer) As Boolean
			Dim contentOfLocation As Char = lake.getLocation(y, x)
			Return contentOfLocation = RobotLake.ICE OrElse contentOfLocation = RobotLake.GOAL
		End Function

	End Class
End Namespace