Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.environment
Imports IntegerActionSchema = org.deeplearning4j.rl4j.environment.IntegerActionSchema
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
Imports org.deeplearning4j.rl4j.space
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports org.deeplearning4j.rl4j.space
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
Namespace org.deeplearning4j.rl4j.mdp.robotlake


	Public Class RobotLake
		Implements Environment(Of Integer)

		Private Const GOAL_REWARD As Double = 10.0
		Private Const STEPPED_ON_HOLE_REWARD As Double = -2.0
		Private Const MOVE_AWAY_FROM_GOAL_REWARD As Double = -0.1

		Public Const NUM_ACTIONS As Integer = 4
		Public Const ACTION_LEFT As Integer = 0
		Public Const ACTION_RIGHT As Integer = 1
		Public Const ACTION_UP As Integer = 2
		Public Const ACTION_DOWN As Integer = 3

		Public Const PLAYER As Char = "P"c
		Public Const GOAL As Char = "G"c
		Public Const HOLE As Char = "@"c
		Public Const ICE As Char = " "c

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.environment.Schema<Integer> schema;
		Private schema As Schema(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean episodeFinished = false;
		Private episodeFinished As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean goalReached = false;
		Private goalReached As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.DiscreteSpace actionSpace = new org.deeplearning4j.rl4j.space.DiscreteSpace(NUM_ACTIONS);
		Private actionSpace As New DiscreteSpace(NUM_ACTIONS)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.ObservationSpace<RobotLakeState> observationSpace = new org.deeplearning4j.rl4j.space.ArrayObservationSpace(new int[] { });
		Private observationSpace As ObservationSpace(Of RobotLakeState) = New ArrayObservationSpace(New Integer() {})

		Private state As RobotLakeState
		Private ReadOnly size As Integer

		Public Sub New(ByVal size As Integer)
			Me.New(size, False, Nd4j.Random)
		End Sub

		Public Sub New(ByVal size As Integer, ByVal areStartingPositionsRandom As Boolean, ByVal rnd As Random)
			state = New RobotLakeState(size, areStartingPositionsRandom, rnd)
			Me.size = size
			Me.schema = New Schema(Of Integer)(New IntegerActionSchema(NUM_ACTIONS, ACTION_LEFT, rnd))
		End Sub

		Public Overridable Function reset() As IDictionary(Of String, Object) Implements Environment(Of Integer).reset
			state.reset()
			episodeFinished = False
			goalReached = False

			Return getChannelsData()
		End Function

		Public Overridable Function [step](ByVal action As Integer?) As StepResult
			Dim reward As Double = 0.0

			Select Case action
				Case ACTION_LEFT
					state.moveRobotLeft()

				Case ACTION_RIGHT
					state.moveRobotRight()

				Case ACTION_UP
					state.moveRobotUp()

				Case ACTION_DOWN
					state.moveRobotDown()
			End Select

			If RobotLakeHelper.isGoalAtLocation(state.getLake(), state.getRobotY(), state.getRobotX()) Then
				episodeFinished = True
				goalReached = True
				reward = GOAL_REWARD
			ElseIf Not RobotLakeHelper.isLocationSafe(state.getLake(), state.getRobotY(), state.getRobotX()) Then
				episodeFinished = True
				reward = STEPPED_ON_HOLE_REWARD
			Else
				' Give a small negative reward for moving away from the goal (to speedup learning)
				Select Case action
					Case ACTION_LEFT
						reward = If(state.getGoalX() > 0, MOVE_AWAY_FROM_GOAL_REWARD, 0.0)

					Case ACTION_RIGHT
						reward = If(state.getGoalX() = 0, MOVE_AWAY_FROM_GOAL_REWARD, 0.0)

					Case ACTION_UP
						reward = If(state.getGoalY() > 0, MOVE_AWAY_FROM_GOAL_REWARD, 0.0)

					Case ACTION_DOWN
						reward = If(state.getGoalY() = 0, MOVE_AWAY_FROM_GOAL_REWARD, 0.0)
				End Select
			End If

			Return New StepResult(getChannelsData(), reward, episodeFinished)
		End Function


		Public Overridable Sub close() Implements Environment(Of Integer).close
			' Do nothing
		End Sub

		Private ReadOnly Property TrackerChannelData As Double()
			Get
				Return New Double() { state.getGoalY() - state.getRobotY(), state.getGoalX() - state.getRobotX() }
			End Get
		End Property

		Private ReadOnly Property RadarChannelData As Double()
			Get
				Return New Double() {If(state.getRobotY() = 0 OrElse RobotLakeHelper.isLocationSafe(state.getLake(), state.getRobotY() - 1, state.getRobotX()), 1.0, 0.0), If(state.getRobotX() = (size - 1) OrElse RobotLakeHelper.isLocationSafe(state.getLake(), state.getRobotY(), state.getRobotX() + 1), 1.0, 0.0), If(state.getRobotY() = (size - 1) OrElse RobotLakeHelper.isLocationSafe(state.getLake(), state.getRobotY() + 1, state.getRobotX()), 1.0, 0.0), If(state.getRobotX() = 0 OrElse RobotLakeHelper.isLocationSafe(state.getLake(), state.getRobotY(), state.getRobotX() - 1), 1.0, 0.0)}
			End Get
		End Property

		Private ReadOnly Property ChannelsData As IDictionary(Of String, Object)
			Get
				Return New HashMapAnonymousInnerClass(Me)
			End Get
		End Property

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As RobotLake

			Public Sub New(ByVal outerInstance As RobotLake)
				Me.outerInstance = outerInstance

				Me.put("tracker", outerInstance.TrackerChannelData)
				Me.put("radar", outerInstance.RadarChannelData)
			End Sub

		End Class
	End Class

End Namespace