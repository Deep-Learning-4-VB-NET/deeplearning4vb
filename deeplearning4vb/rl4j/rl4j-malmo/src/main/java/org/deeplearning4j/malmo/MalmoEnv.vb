Imports System
Imports System.Threading
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.mdp
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports AgentHost = com.microsoft.msr.malmo.AgentHost
Imports ClientInfo = com.microsoft.msr.malmo.ClientInfo
Imports ClientPool = com.microsoft.msr.malmo.ClientPool
Imports MissionRecordSpec = com.microsoft.msr.malmo.MissionRecordSpec
Imports MissionSpec = com.microsoft.msr.malmo.MissionSpec
Imports TimestampedReward = com.microsoft.msr.malmo.TimestampedReward
Imports WorldState = com.microsoft.msr.malmo.WorldState
Imports Setter = lombok.Setter
Imports Getter = lombok.Getter
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.malmo


	Public Class MalmoEnv
		Implements MDP(Of MalmoBox, Integer, DiscreteSpace)

		' Malmo Java client library depends on native library
		Shared Sub New()
			Dim malmoHome As String = Environment.GetEnvironmentVariable("MALMO_HOME")

			If malmoHome Is Nothing Then
				Throw New Exception("MALMO_HOME must be set to your Malmo environement.")
			End If

			Try
				If Files.exists(Paths.get(malmoHome & "/Java_Examples/libMalmoJava.jnilib")) Then
					System.load(malmoHome & "/Java_Examples/libMalmoJava.jnilib")
				ElseIf Files.exists(Paths.get(malmoHome & "/Java_Examples/MalmoJava.dll")) Then
					System.load(malmoHome & "/Java_Examples/MalmoJava.dll")
				ElseIf Files.exists(Paths.get(malmoHome & "/Java_Examples/libMalmoJava.so")) Then
					System.load(malmoHome & "/Java_Examples/libMalmoJava.so")
				Else
					System.load("MalmoJava")
				End If
			Catch e As UnsatisfiedLinkError
				Throw New Exception("MALMO_HOME must be set to your Malmo environement. Could not load native library at '" & malmoHome & "/Java_Examples/'", e)
			End Try
		End Sub

		Private Const NUM_RETRIES As Integer = 10

		Private ReadOnly logger As Logger

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter MissionSpec mission = null;
		Friend mission As MissionSpec = Nothing

		Friend missionRecord As MissionRecordSpec = Nothing
		Friend clientPool As ClientPool = Nothing

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter MalmoObservationSpace observationSpace;
		Friend observationSpace As MalmoObservationSpace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter MalmoActionSpace actionSpace;
		Friend actionSpace As MalmoActionSpace

		Friend framePolicy As MalmoObservationPolicy

		Friend agent_host As AgentHost = Nothing
		Friend last_world_state As WorldState
		Friend last_observation As MalmoBox

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter MalmoResetHandler resetHandler = null;
		Friend resetHandler As MalmoResetHandler = Nothing

		''' <summary>
		''' Create a MalmoEnv using a XML-file mission description and minimal set of parameters.
		''' Equivalent to MalmoEnv( loadMissionXML( missionFileName ), missionRecord, actionSpace, observationSpace, framePolicy, clientPool ) </summary>
		''' <param name="missionFileName"> Name of XML file describing mission </param>
		''' <param name="actionSpace"> Malmo action space implementation </param>
		''' <param name="observationSpace"> Malmo observation space implementation </param>
		''' <param name="framePolicy"> Malmo frame policy implementation </param>
		''' <param name="clientPool"> Malmo client pool in host:port string form. If set to null, will default to a single Malmo client at 127.0.0.1:10000 </param>
		Public Sub New(ByVal missionFileName As String, ByVal actionSpace As MalmoActionSpace, ByVal observationSpace As MalmoObservationSpace, ByVal framePolicy As MalmoObservationPolicy, ParamArray ByVal clientPool() As String)
			Me.New(loadMissionXML(missionFileName), Nothing, actionSpace, observationSpace, framePolicy, Nothing)

			If clientPool Is Nothing OrElse clientPool.Length = 0 Then
				clientPool = New String() {"127.0.0.1:10000"}
			End If
			Me.clientPool = New ClientPool()
			For Each client As String In clientPool
				Dim parts() As String = client.Split(":", True)
				Me.clientPool.add(New ClientInfo(parts(0), Short.Parse(parts(1))))
			Next client
		End Sub

		''' <summary>
		''' Create a fully specified MalmoEnv </summary>
		''' <param name="mission"> Malmo mission specification. </param>
		''' <param name="missionRecord"> Malmo record specification. Ignored if set to NULL </param>
		''' <param name="actionSpace"> Malmo action space implementation </param>
		''' <param name="observationSpace"> Malmo observation space implementation </param>
		''' <param name="framePolicy"> Malmo frame policy implementation </param>
		''' <param name="clientPool"> Malmo client pool; cannot be null </param>
		Public Sub New(ByVal mission As MissionSpec, ByVal missionRecord As MissionRecordSpec, ByVal actionSpace As MalmoActionSpace, ByVal observationSpace As MalmoObservationSpace, ByVal framePolicy As MalmoObservationPolicy, ByVal clientPool As ClientPool)
			Me.mission = mission
			Me.missionRecord = If(missionRecord IsNot Nothing, missionRecord, New MissionRecordSpec())
			Me.actionSpace = actionSpace
			Me.observationSpace = observationSpace
			Me.framePolicy = framePolicy
			Me.clientPool = clientPool

			logger = LoggerFactory.getLogger(Me.GetType())
		End Sub

		''' <summary>
		''' Convenience method to load a Malmo mission specification from an XML-file </summary>
		''' <param name="filename"> name of XML file </param>
		''' <returns> Mission specification loaded from XML-file </returns>
		Public Shared Function loadMissionXML(ByVal filename As String) As MissionSpec
			Dim mission As MissionSpec = Nothing
			Try
				Dim xml As New String(Files.readAllBytes(Paths.get(filename)))
				mission = New MissionSpec(xml, True)
			Catch e As Exception
				'e.printStackTrace();
				Throw New Exception(e)
			End Try

			Return mission
		End Function

		Public Overridable Function reset() As MalmoBox
			close()

			If resetHandler IsNot Nothing Then
				resetHandler.onReset(Me)
			End If

			agent_host = New AgentHost()

			Dim i As Integer
			For i = NUM_RETRIES - 1 To 1 Step -1
				Try
					Thread.Sleep(100 + 500 * (NUM_RETRIES - i - 1))
					agent_host.startMission(mission, clientPool, missionRecord, 0, "rl4j_0")
				Catch e As Exception
					logger.warn("Error starting mission: " & e.Message & " Retrying " & i & " more times.")
					Continue For
				End Try
				Exit For
			Next i

			If i = 0 Then
				close()
				Throw New MalmoConnectionError("Unable to connect to client.")
			End If

			logger.info("Waiting for the mission to start")

			Do
				last_world_state = agent_host.getWorldState()
			Loop While Not last_world_state.getIsMissionRunning()

			'We need to make sure all the blocks are in place before we grab the observations
			Try
				Thread.Sleep(500)
			Catch e As InterruptedException
			End Try

			last_world_state = waitForObservations(False)

			last_observation = observationSpace.getObservation(last_world_state)

			Return last_observation
		End Function

		Private Function waitForObservations(ByVal andRewards As Boolean) As WorldState
			Dim world_state As WorldState
			Dim original_world_state As WorldState = last_world_state
			Dim missingData As Boolean

			Do
				Thread.yield()
				world_state = agent_host.peekWorldState()
				missingData = world_state.getObservations().isEmpty() OrElse world_state.getVideoFrames().isEmpty()
				missingData = If(andRewards, missingData OrElse Not framePolicy.isObservationConsistant(world_state, original_world_state), missingData)
			Loop While missingData AndAlso world_state.getIsMissionRunning()

			Return agent_host.getWorldState()
		End Function

		Public Overridable Sub close() Implements MDP(Of MalmoBox, Integer, DiscreteSpace).close
			If agent_host IsNot Nothing Then
				agent_host.delete()
			End If

			agent_host = Nothing
		End Sub

		Public Overridable Function [step](ByVal action As Integer?) As StepReply(Of MalmoBox)
			agent_host.sendCommand(DirectCast(actionSpace.encode(action), String))

			last_world_state = waitForObservations(True)
			last_observation = observationSpace.getObservation(last_world_state)

			If Done Then
				logger.info("Mission ended")
			End If

			Return New StepReply(Of MalmoBox)(last_observation, getRewards(last_world_state), Done, Nothing)
		End Function

		Private Function getRewards(ByVal world_state As WorldState) As Double
			Dim rval As Double = 0

			Dim i As Integer = 0
			Do While i < world_state.getRewards().size()
				Dim reward As TimestampedReward = world_state.getRewards().get(i)
				rval += reward.getValue()
				i += 1
			Loop

			Return rval
		End Function

		Public Overridable ReadOnly Property Done As Boolean Implements MDP(Of MalmoBox, Integer, DiscreteSpace).isDone
			Get
				Return Not last_world_state.getIsMissionRunning()
			End Get
		End Property

		Public Overridable Function newInstance() As MDP(Of MalmoBox, Integer, DiscreteSpace)
			Dim rval As New MalmoEnv(mission, missionRecord, actionSpace, observationSpace, framePolicy, clientPool)
			rval.setResetHandler(resetHandler)
			Return rval
		End Function
	End Class

End Namespace