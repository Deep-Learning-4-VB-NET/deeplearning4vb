Imports System.Runtime.InteropServices

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

Namespace vizdoom

	Public Class DoomGame
		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for VB:
'			System.loadLibrary("vizdoom")
		End Sub

		Public internalPtr As Long = 0
		Public Sub New()
			Me.DoomGameNative()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function doomTics2Ms(ByVal tics As Double, ByVal ticrate As Integer) As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function ms2DoomTics(ByVal ms As Double, ByVal ticrate As Integer) As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function doomTics2Sec(ByVal tics As Double, ByVal ticrate As Integer) As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function sec2DoomTics(ByVal sec As Double, ByVal ticrate As Integer) As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function doomFixedToDouble(ByVal doomFixed As Double) As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isBinaryButton(ByVal button As Button) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isDeltaButton(ByVal button As Button) As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Private Sub DoomGameNative()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function loadConfig(ByVal file As String) As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function init() As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub close()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub newEpisode()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub newEpisode(ByVal file As String)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub replayEpisode(ByVal file As String)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub replayEpisode(ByVal file As String, ByVal player As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isRunning() As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isMultiplayerGame() As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setAction(ByVal actions() As Double)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub advanceAction()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub advanceAction(ByVal tics As Integer)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub advanceAction(ByVal tics As Integer, ByVal stateUpdate As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function makeAction(ByVal actions() As Double) As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function makeAction(ByVal actions() As Double, ByVal tics As Integer) As Double
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getState() As GameState
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getLastAction() As Double()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isNewEpisode() As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isEpisodeFinished() As Boolean
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isPlayerDead() As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub respawnPlayer()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getAvailableButtons() As Button()
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setAvailableButtons(ByVal buttons() As Button)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub addAvailableButton(ByVal button As Button)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub addAvailableButton(ByVal button As Button, ByVal maxValue As Double)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub clearAvailableButtons()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getAvailableButtonsSize() As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setButtonMaxValue(ByVal button As Button, ByVal maxValue As Double)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getButtonMaxValue(ByVal button As Button) As Double
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getAvailableGameVariables() As GameVariable()
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setAvailableGameVariables(ByVal gameVariables() As GameVariable)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub addAvailableGameVariable(ByVal var As GameVariable)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub clearAvailableGameVariables()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getAvailableGameVariablesSize() As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub addGameArgs(ByVal arg As String)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub clearGameArgs()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub sendGameCommand(ByVal cmd As String)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Private Function getModeNative() As Integer
		End Function

		Public Overridable ReadOnly Property Mode As Mode
			Get
				Return System.Enum.GetValues(GetType(Mode))(Me.getModeNative())
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setMode(ByVal mode As Mode)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getTicrate() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setTicrate(ByVal ticrate As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getGameVariable(ByVal var As GameVariable) As Double
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getLivingReward() As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setLivingReward(ByVal livingReward As Double)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getDeathPenalty() As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setDeathPenalty(ByVal deathPenalty As Double)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getLastReward() As Double
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getTotalReward() As Double
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setViZDoomPath(ByVal path As String)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setDoomGamePath(ByVal path As String)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setDoomScenarioPath(ByVal path As String)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setDoomMap(ByVal map As String)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setDoomSkill(ByVal skill As Integer)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setDoomConfigPath(ByVal path As String)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getSeed() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setSeed(ByVal seed As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getEpisodeStartTime() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setEpisodeStartTime(ByVal tics As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getEpisodeTimeout() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setEpisodeTimeout(ByVal tics As Integer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getEpisodeTime() As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isDepthBufferEnabled() As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setDepthBufferEnabled(ByVal depthBuffer As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isLabelsBufferEnabled() As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setLabelsBufferEnabled(ByVal labelsBuffer As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function isAutomapBufferEnabled() As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setAutomapBufferEnabled(ByVal automapBuffer As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setAutomapMode(ByVal mode As AutomapMode)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setAutomapRotate(ByVal rotate As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setAutomapRenderTextures(ByVal textures As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setScreenResolution(ByVal resolution As ScreenResolution)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setScreenFormat(ByVal format As ScreenFormat)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderHud(ByVal hud As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderMinimalHud(ByVal minimalHud As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderWeapon(ByVal weapon As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderCrosshair(ByVal crosshair As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderDecals(ByVal decals As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderParticles(ByVal particles As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderEffectsSprites(ByVal sprites As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderMessages(ByVal messages As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderCorpses(ByVal corpses As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderScreenFlashes(ByVal flashes As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setRenderAllFrames(ByVal allFrames As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setWindowVisible(ByVal visibility As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setConsoleEnabled(ByVal console As Boolean)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Sub setSoundEnabled(ByVal sound As Boolean)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getScreenWidth() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getScreenHeight() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getScreenChannels() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getScreenPitch() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Public Function getScreenSize() As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")>
		Private Function getScreenFormatNative() As Integer
		End Function

		Public Overridable ReadOnly Property ScreenFormat As ScreenFormat
			Get
				Return System.Enum.GetValues(GetType(ScreenFormat))(Me.getScreenFormatNative())
			End Get
		End Property

	End Class

End Namespace