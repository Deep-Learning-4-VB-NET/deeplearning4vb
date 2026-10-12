Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Value = lombok.Value
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.space
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.space
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports vizdoom

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




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class VizDoom implements org.deeplearning4j.rl4j.mdp.MDP<VizDoom.GameScreen, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace>
	Public MustInherit Class VizDoom
		Implements MDP(Of VizDoom.GameScreen, Integer, DiscreteSpace)

		Public MustOverride Function [step](ByVal action As ACTION) As StepReply(Of OBSERVATION) Implements MDP(Of VizDoom.GameScreen, Integer, DiscreteSpace).step


		Public Const DOOM_ROOT As String = "vizdoom"

		Protected Friend game As DoomGame
		Protected Friend ReadOnly actions As IList(Of Double())
		Protected Friend ReadOnly discreteSpace As DiscreteSpace
		Protected Friend ReadOnly observationSpace As ObservationSpace(Of GameScreen)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final boolean render;
		Protected Friend ReadOnly render As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected double scaleFactor = 1;
		Protected Friend scaleFactor As Double = 1

		Public Sub New()
			Me.New(False)
		End Sub

		Public Sub New(ByVal render As Boolean)
			Me.render = render
			actions = New List(Of Double())()
			game = New DoomGame()
			setupGame()
			discreteSpace = New DiscreteSpace(Configuration.getButtons().size() + 1)
			observationSpace = New ArrayObservationSpace(Of GameScreen)(New Integer() {game.getScreenHeight(), game.getScreenWidth(), 3})
		End Sub


		Public Overridable Sub setupGame()

			Dim conf As Configuration = Configuration

			game.setViZDoomPath(DOOM_ROOT & "/vizdoom")
			game.setDoomGamePath(DOOM_ROOT & "/freedoom2.wad")
			game.setDoomScenarioPath(DOOM_ROOT & "/scenarios/" & conf.getScenario() & ".wad")

			game.setDoomMap("map01")

			game.setScreenFormat(ScreenFormat.RGB24)
			game.setScreenResolution(ScreenResolution.RES_800X600)
			' Sets other rendering options
			game.setRenderHud(False)
			game.setRenderCrosshair(False)
			game.setRenderWeapon(True)
			game.setRenderDecals(False)
			game.setRenderParticles(False)


			Dim gameVar() As GameVariable = {GameVariable.KILLCOUNT, GameVariable.ITEMCOUNT, GameVariable.SECRETCOUNT, GameVariable.FRAGCOUNT, GameVariable.HEALTH, GameVariable.ARMOR, GameVariable.DEAD, GameVariable.ON_GROUND, GameVariable.ATTACK_READY, GameVariable.ALTATTACK_READY, GameVariable.SELECTED_WEAPON, GameVariable.SELECTED_WEAPON_AMMO, GameVariable.AMMO1, GameVariable.AMMO2, GameVariable.AMMO3, GameVariable.AMMO4, GameVariable.AMMO5, GameVariable.AMMO6, GameVariable.AMMO7, GameVariable.AMMO8, GameVariable.AMMO9, GameVariable.AMMO0}
			' Adds game variables that will be included in state.

			For i As Integer = 0 To gameVar.Length - 1
				game.addAvailableGameVariable(gameVar(i))
			Next i


			' Causes episodes to finish after timeout tics
			game.setEpisodeTimeout(conf.getTimeout())

			game.setEpisodeStartTime(conf.getStartTime())

			game.setWindowVisible(render)
			game.setSoundEnabled(False)
			game.setMode(Mode.PLAYER)


			game.setLivingReward(conf.getLivingReward())

			' Adds buttons that will be allowed.
			Dim buttons As IList(Of Button) = conf.getButtons()
			Dim size As Integer = buttons.Count

			actions.Add(New Double(size){})
			For i As Integer = 0 To size - 1
				game.addAvailableButton(buttons(i))
				Dim action(size) As Double
				action(i) = 1
				actions.Add(action)
			Next i

			game.setDeathPenalty(conf.getDeathPenalty())
			game.setDoomSkill(conf.getDoomSkill())

			game.init()
		End Sub

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of VizDoom.GameScreen, Integer, DiscreteSpace).isDone
			Get
				Return game.isEpisodeFinished()
			End Get
		End Property


		Public Overridable Function reset() As GameScreen
			log.info("free Memory: " & Pointer.formatBytes(Pointer.availablePhysicalBytes()) & "/" & Pointer.formatBytes(Pointer.totalPhysicalBytes()))

			game.newEpisode()
			Return New GameScreen(observationSpace.Shape, game.getState().screenBuffer)
		End Function


		Public Overridable Sub close() Implements MDP(Of VizDoom.GameScreen, Integer, DiscreteSpace).close
			game.close()
		End Sub


		Public Overridable Function [step](ByVal action As Integer?) As StepReply(Of GameScreen)

			Dim r As Double = game.makeAction(actions(action)) * scaleFactor
			log.info(game.getEpisodeTime() & " " & r & " " & action.Value & " ")
			Return New StepReply(New GameScreen(observationSpace.Shape,If(game.isEpisodeFinished(), New SByte(game.getScreenSize() - 1){}, game.getState().screenBuffer)), r, game.isEpisodeFinished(), Nothing)

		End Function


		Public Overridable Function getObservationSpace() As ObservationSpace(Of GameScreen) Implements MDP(Of VizDoom.GameScreen, Integer, DiscreteSpace).getObservationSpace
			Return observationSpace
		End Function


		Public Overridable ReadOnly Property ActionSpace As DiscreteSpace
			Get
				Return discreteSpace
			End Get
		End Property

		Public MustOverride ReadOnly Property Configuration As Configuration

		Public MustOverride Function newInstance() As VizDoom

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value public static class Configuration
		Public Class Configuration
			Friend scenario As String
			Friend livingReward As Double
			Friend deathPenalty As Double
			Friend doomSkill As Integer
			Friend timeout As Integer
			Friend startTime As Integer
			Friend buttons As IList(Of Button)
		End Class

		Public Class GameScreen
			Implements Encodable

'JAVA TO VB CONVERTER NOTE: The field data was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend ReadOnly data_Conflict As INDArray
			Public Sub New(ByVal shape() As Integer, ByVal screen() As SByte)

				data_Conflict = Nd4j.create(screen, New Long() {shape(1), shape(2), 3}, DataType.UINT8).permute(2,0,1)
			End Sub

			Friend Sub New(ByVal toDup As INDArray)
				data_Conflict = toDup.dup()
			End Sub

			Public Overridable Function toArray() As Double() Implements Encodable.toArray
				Return data_Conflict.data().asDouble()
			End Function

			Public Overridable ReadOnly Property Skipped As Boolean Implements Encodable.isSkipped
				Get
					Return False
				End Get
			End Property

			Public Overridable ReadOnly Property Data As INDArray Implements Encodable.getData
				Get
					Return data_Conflict
				End Get
			End Property

			Public Overridable Function dup() As GameScreen
				Return New GameScreen(data_Conflict)
			End Function
		End Class

	End Class

End Namespace