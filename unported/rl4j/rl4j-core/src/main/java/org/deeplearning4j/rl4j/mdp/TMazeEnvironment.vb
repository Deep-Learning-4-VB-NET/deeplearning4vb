Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.environment
Imports IntegerActionSchema = org.deeplearning4j.rl4j.environment.IntegerActionSchema
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
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

Namespace org.deeplearning4j.rl4j.mdp


	Public Class TMazeEnvironment
		Implements Environment(Of Integer)

		Private Const BAD_MOVE_REWARD As Double = -0.1
		Private Const GOAL_REWARD As Double = 4.0
		Private Const TRAP_REWARD As Double = -4.0
		Private Const BRANCH_REWARD As Double = 1.0

		Private Const NUM_ACTIONS As Integer = 4
		Private Const ACTION_LEFT As Integer = 0
		Private Const ACTION_RIGHT As Integer = 1
		Private Const ACTION_UP As Integer = 2
		Private Const ACTION_DOWN As Integer = 3

		Private ReadOnly lengthOfMaze As Integer
		Private ReadOnly rnd As Random

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.environment.Schema<Integer> schema;
		Private ReadOnly schema As Schema(Of Integer)

		Private currentLocation As Integer
		Private hasNavigatedToBranch As Boolean

'JAVA TO VB CONVERTER NOTE: The field hasNavigatedToSolution was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasNavigatedToSolution_Conflict As Boolean
		Public Overridable Function hasNavigatedToSolution() As Boolean
			Return hasNavigatedToSolution_Conflict
		End Function

		Private isSolutionUp As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter boolean episodeFinished;
		Friend episodeFinished As Boolean

		Public Sub New(ByVal lengthOfMaze As Integer, ByVal rnd As Random)
			Me.lengthOfMaze = lengthOfMaze
			Me.rnd = rnd

			Me.schema = New Schema(Of Integer)(New IntegerActionSchema(NUM_ACTIONS, ACTION_RIGHT, rnd))
		End Sub

		Public Overridable Function reset() As IDictionary(Of String, Object) Implements Environment(Of Integer).reset
			episodeFinished = False
			currentLocation = 0
			hasNavigatedToBranch = False

			isSolutionUp = rnd.nextBoolean()

			Return New HashMapAnonymousInnerClass(Me)
		End Function

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TMazeEnvironment

			Public Sub New(ByVal outerInstance As TMazeEnvironment)
				Me.outerInstance = outerInstance

				Me.put("data", New Double() {1.0, 0.0, 0.0, If(outerInstance.isSolutionUp, 1.0, 0.0), If(outerInstance.isSolutionUp, 0.0, 1.0)})
			End Sub

		End Class

		Public Overridable Function [step](ByVal action As Integer?) As StepResult
			Dim isAtJunction As Boolean = currentLocation = lengthOfMaze - 1
			Dim reward As Double = 0.0

			If Not episodeFinished Then
				Select Case action
					Case ACTION_LEFT
						reward = BAD_MOVE_REWARD
						If currentLocation > 0 Then
							currentLocation -= 1
						End If

					Case ACTION_RIGHT
						If isAtJunction Then
							reward = BAD_MOVE_REWARD
						Else
							currentLocation += 1
						End If

					Case ACTION_UP
						If Not isAtJunction Then
							reward = BAD_MOVE_REWARD
						Else
							reward = If(isSolutionUp, GOAL_REWARD, TRAP_REWARD)
							hasNavigatedToSolution_Conflict = isSolutionUp
							episodeFinished = True
						End If

					Case ACTION_DOWN
						If Not isAtJunction Then
							reward = BAD_MOVE_REWARD
						Else
							reward = If(Not isSolutionUp, GOAL_REWARD, TRAP_REWARD)
							hasNavigatedToSolution_Conflict = Not isSolutionUp
							episodeFinished = True
						End If
				End Select
			End If

			Dim isAtJunctionAfterMove As Boolean = currentLocation = lengthOfMaze - 1
			If Not hasNavigatedToBranch AndAlso isAtJunctionAfterMove Then
				reward += BRANCH_REWARD
				hasNavigatedToBranch = True
			End If
			Dim channelData() As Double = If(isAtJunctionAfterMove, New Double() { 0.0, 0.0, 1.0, -1.0, -1.0 }, New Double()){ 0.0, 1.0, 0.0, -1.0, -1.0 }

			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass2(Me, channelData)
			Return New StepResult(channelsData, reward, episodeFinished)
		End Function

		Private Class HashMapAnonymousInnerClass2
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As TMazeEnvironment

			private Double() channelData

			public HashMapAnonymousInnerClass2(TMazeEnvironment outerInstance, Double() channelData)
			If True Then
				Me.outerInstance = outerInstance
				Me.channelData = channelData

				Me.put("data", channelData)
			End If

		End Class


		public void close()
		If True Then
			' Do nothing
		End If
	End Class
End Namespace