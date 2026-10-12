Imports System
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.space
Imports Box = org.deeplearning4j.rl4j.space.Box
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports org.deeplearning4j.rl4j.space

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


	Public Class CartpoleNative
		Implements MDP(Of Box, Integer, DiscreteSpace)

		Public Enum KinematicsIntegrators
			Euler
			SemiImplicitEuler

		End Enum
		Private Const NUM_ACTIONS As Integer = 2
		Private Const ACTION_LEFT As Integer = 0
		Private Const ACTION_RIGHT As Integer = 1
		Private Const OBSERVATION_NUM_FEATURES As Integer = 4

		Private Const gravity As Double = 9.8
		Private Const massCart As Double = 1.0
		Private Const massPole As Double = 0.1
		Private Shared ReadOnly totalMass As Double = massPole + massCart
		Private Const length As Double = 0.5 ' actually half the pole's length
		Private Shared ReadOnly polemassLength As Double = massPole * length
		Private Const forceMag As Double = 10.0
		Private Const tau As Double = 0.02 ' seconds between state updates

		' Angle at which to fail the episode
		Private Shared ReadOnly thetaThresholdRadians As Double = 12.0 * 2.0 * Math.PI / 360.0
		Private Const xThreshold As Double = 2.4

		Private ReadOnly rnd As Random

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private KinematicsIntegrators kinematicsIntegrator = KinematicsIntegrators.Euler;
		Private kinematicsIntegrator As KinematicsIntegrators = KinematicsIntegrators.Euler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean done = false;
		Private done As Boolean = False

		Private x As Double
		Private xDot As Double
		Private theta As Double
		Private thetaDot As Double
		Private stepsBeyondDone As Integer?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.DiscreteSpace actionSpace = new org.deeplearning4j.rl4j.space.DiscreteSpace(NUM_ACTIONS);
		Private actionSpace As New DiscreteSpace(NUM_ACTIONS)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.deeplearning4j.rl4j.space.ObservationSpace<org.deeplearning4j.rl4j.space.Box> observationSpace = new org.deeplearning4j.rl4j.space.ArrayObservationSpace(new int[] { OBSERVATION_NUM_FEATURES });
		Private observationSpace As ObservationSpace(Of Box) = New ArrayObservationSpace(New Integer() { OBSERVATION_NUM_FEATURES })

		Public Sub New()
			rnd = New Random()
		End Sub

		Public Sub New(ByVal seed As Integer)
			rnd = New Random(seed)
		End Sub

		Public Overridable Function reset() As Box

			x = 0.1 * rnd.NextDouble() - 0.05
			xDot = 0.1 * rnd.NextDouble() - 0.05
			theta = 0.1 * rnd.NextDouble() - 0.05
			thetaDot = 0.1 * rnd.NextDouble() - 0.05
			stepsBeyondDone = Nothing
			done = False

			Return New Box(x, xDot, theta, thetaDot)
		End Function

		Public Overridable Sub close() Implements MDP(Of Box, Integer, DiscreteSpace).close

		End Sub

		Public Overridable Function [step](ByVal action As Integer?) As StepReply(Of Box)
			Dim force As Double = If(action = ACTION_RIGHT, forceMag, -forceMag)
			Dim cosTheta As Double = Math.Cos(theta)
			Dim sinTheta As Double = Math.Sin(theta)
			Dim temp As Double = (force + polemassLength * thetaDot * thetaDot * sinTheta) / totalMass
			Dim thetaAcc As Double = (gravity * sinTheta - cosTheta* temp) / (length * (4.0/3.0 - massPole * cosTheta * cosTheta / totalMass))
			Dim xAcc As Double = temp - polemassLength * thetaAcc * cosTheta / totalMass

			Select Case kinematicsIntegrator
				Case org.deeplearning4j.rl4j.mdp.CartpoleNative.KinematicsIntegrators.Euler
					x += tau * xDot
					xDot += tau * xAcc
					theta += tau * thetaDot
					thetaDot += tau * thetaAcc

				Case org.deeplearning4j.rl4j.mdp.CartpoleNative.KinematicsIntegrators.SemiImplicitEuler
					xDot += tau * xAcc
					x += tau * xDot
					thetaDot += tau * thetaAcc
					theta += tau * thetaDot
			End Select

			done = done Or x < -xThreshold OrElse x > xThreshold OrElse theta < -thetaThresholdRadians OrElse theta > thetaThresholdRadians

			Dim reward As Double
			If Not done Then
				reward = 1.0
			ElseIf stepsBeyondDone Is Nothing Then
				stepsBeyondDone = 0
				reward = 1.0
			Else
				stepsBeyondDone += 1
				reward = 0
			End If

			Return New StepReply(Of Box)(New Box(x, xDot, theta, thetaDot), reward, done, Nothing)
		End Function

		Public Overridable Function newInstance() As MDP(Of Box, Integer, DiscreteSpace) Implements MDP(Of Box, Integer, DiscreteSpace).newInstance
			Return New CartpoleNative()
		End Function

	End Class

End Namespace