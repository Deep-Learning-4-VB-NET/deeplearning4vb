Imports System
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Value = lombok.Value
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ALEInterface = org.bytedeco.ale.ALEInterface
Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.mdp
Imports org.deeplearning4j.rl4j.space
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports Encodable = org.deeplearning4j.rl4j.space.Encodable
Imports org.deeplearning4j.rl4j.space
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.rl4j.mdp.ale

	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ALEMDP implements org.deeplearning4j.rl4j.mdp.MDP<ALEMDP.GameScreen, Integer, org.deeplearning4j.rl4j.space.DiscreteSpace>
	Public Class ALEMDP
		Implements MDP(Of ALEMDP.GameScreen, Integer, DiscreteSpace)

		Protected Friend ale As ALEInterface
		Protected Friend ReadOnly actions() As Integer
		Protected Friend ReadOnly discreteSpace As DiscreteSpace
		Protected Friend ReadOnly observationSpace As ObservationSpace(Of GameScreen)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final String romFile;
		Protected Friend ReadOnly romFile As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final boolean render;
		Protected Friend ReadOnly render As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final Configuration configuration;
		Protected Friend ReadOnly configuration As Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected double scaleFactor = 1;
		Protected Friend scaleFactor As Double = 1

		Private screenBuffer() As SByte

		Public Sub New(ByVal romFile As String)
			Me.New(romFile, False)
		End Sub

		Public Sub New(ByVal romFile As String, ByVal render As Boolean)
			Me.New(romFile, render, New Configuration(123, 0, 0, 0, True))
		End Sub

		Public Sub New(ByVal romFile As String, ByVal render As Boolean, ByVal configuration As Configuration)
			Me.romFile = romFile
			Me.configuration = configuration
			Me.render = render
			ale = New ALEInterface()
			setupGame()

			' Get the vector of minimal or legal actions
			Dim a As IntPointer = (If(getConfiguration().minimalActionSet, ale.getMinimalActionSet(), ale.getLegalActionSet()))
			actions = New Integer(CInt(Math.Truncate(a.limit())) - 1){}
			a.get(actions)

			Dim height As Integer = CInt(ale.getScreen().height())
			Dim width As Integer = CInt(CInt(ale.getScreen().width()))

			discreteSpace = New DiscreteSpace(actions.Length)
			Dim shape() As Integer = {3, height, width}
			observationSpace = New ArrayObservationSpace(Of GameScreen)(shape)
			screenBuffer = New SByte((shape(0) * shape(1) * shape(2)) - 1){}

		End Sub

		Public Overridable Sub setupGame()
			Dim conf As Configuration = getConfiguration()

			' Get & Set the desired settings
			ale.setInt("random_seed", conf.randomSeed)
			ale.setFloat("repeat_action_probability", conf.repeatActionProbability)

			ale.setBool("display_screen", render)
			ale.setBool("sound", render)

			' Causes episodes to finish after timeout tics
			ale.setInt("max_num_frames", conf.maxNumFrames)
			ale.setInt("max_num_frames_per_episode", conf.maxNumFramesPerEpisode)

			' Load the ROM file. (Also resets the system for new settings to
			' take effect.)
			ale.loadROM(romFile)
		End Sub

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of ALEMDP.GameScreen, Integer, DiscreteSpace).isDone
			Get
				Return ale.game_over()
			End Get
		End Property


		Public Overridable Function reset() As GameScreen
			ale.reset_game()
			ale.getScreenRGB(screenBuffer)
			Return New GameScreen(observationSpace.Shape, screenBuffer)
		End Function


		Public Overridable Sub close() Implements MDP(Of ALEMDP.GameScreen, Integer, DiscreteSpace).close
			ale.deallocate()
		End Sub

		Public Overridable Function [step](ByVal action As Integer?) As StepReply(Of GameScreen)
			Dim r As Double = ale.act(actions(action)) * scaleFactor
			log.info(ale.getEpisodeFrameNumber() & " " & r & " " & action.Value & " ")
			ale.getScreenRGB(screenBuffer)

			Return New StepReply(New GameScreen(observationSpace.Shape, screenBuffer), r, ale.game_over(), Nothing)
		End Function

		Public Overridable Function getObservationSpace() As ObservationSpace(Of GameScreen) Implements MDP(Of ALEMDP.GameScreen, Integer, DiscreteSpace).getObservationSpace
			Return observationSpace
		End Function

		Public Overridable ReadOnly Property ActionSpace As DiscreteSpace
			Get
				Return discreteSpace
			End Get
		End Property

		Public Overridable Function newInstance() As ALEMDP
			Return New ALEMDP(romFile, render, configuration)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value public static class Configuration
		Public Class Configuration
			Friend randomSeed As Integer
			Friend repeatActionProbability As Single
			Friend maxNumFrames As Integer
			Friend maxNumFramesPerEpisode As Integer
			Friend minimalActionSet As Boolean
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