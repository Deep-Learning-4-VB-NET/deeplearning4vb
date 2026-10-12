Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports org.deeplearning4j.rl4j.environment
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

Namespace org.deeplearning4j.rl4j.mdp


	Public Class CartpoleEnvironment
		Implements Environment(Of Integer)

		Private Const NUM_ACTIONS As Integer = 2
		Private Const ACTION_LEFT As Integer = 0
		Private Const ACTION_RIGHT As Integer = 1

		Private ReadOnly schema As Schema(Of Integer)

		Public Enum KinematicsIntegrators
			Euler
			SemiImplicitEuler

		End Enum
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
'ORIGINAL LINE: @Getter private boolean episodeFinished = false;
		Private episodeFinished As Boolean = False

		Private x As Double
		Private xDot As Double
		Private theta As Double
		Private thetaDot As Double
		Private stepsBeyondDone As Integer?

		Public Sub New()
			Me.New(Nd4j.Random)
		End Sub

		Public Sub New(ByVal rnd As Random)
			Me.rnd = rnd
			Me.schema = New Schema(Of Integer)(New IntegerActionSchema(NUM_ACTIONS, ACTION_LEFT, rnd))
		End Sub

		Public Overridable Function getSchema() As Schema(Of Integer) Implements Environment(Of Integer).getSchema
			Return schema
		End Function

		Public Overridable Function reset() As IDictionary(Of String, Object) Implements Environment(Of Integer).reset

			x = 0.1 * rnd.nextDouble() - 0.05
			xDot = 0.1 * rnd.nextDouble() - 0.05
			theta = 0.1 * rnd.nextDouble() - 0.05
			thetaDot = 0.1 * rnd.nextDouble() - 0.05
			stepsBeyondDone = Nothing
			episodeFinished = False

			Return New HashMapAnonymousInnerClass(Me)
		End Function

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As CartpoleEnvironment

			Public Sub New(ByVal outerInstance As CartpoleEnvironment)
				Me.outerInstance = outerInstance

				Me.put("data", New Double(){outerInstance.x, outerInstance.xDot, outerInstance.theta, outerInstance.thetaDot})
			End Sub

		End Class

		Public Overridable Function [step](ByVal action As Integer?) As StepResult
			Dim force As Double = If(action = ACTION_RIGHT, forceMag, -forceMag)
			Dim cosTheta As Double = Math.Cos(theta)
			Dim sinTheta As Double = Math.Sin(theta)
			Dim temp As Double = (force + polemassLength * thetaDot * thetaDot * sinTheta) / totalMass
			Dim thetaAcc As Double = (gravity * sinTheta - cosTheta* temp) / (length * (4.0/3.0 - massPole * cosTheta * cosTheta / totalMass))
			Dim xAcc As Double = temp - polemassLength * thetaAcc * cosTheta / totalMass

			Select Case kinematicsIntegrator
				Case org.deeplearning4j.rl4j.mdp.CartpoleEnvironment.KinematicsIntegrators.Euler
					x += tau * xDot
					xDot += tau * xAcc
					theta += tau * thetaDot
					thetaDot += tau * thetaAcc

				Case org.deeplearning4j.rl4j.mdp.CartpoleEnvironment.KinematicsIntegrators.SemiImplicitEuler
					xDot += tau * xAcc
					x += tau * xDot
					thetaDot += tau * thetaAcc
					theta += tau * thetaDot
			End Select

			episodeFinished = episodeFinished Or x < -xThreshold OrElse x > xThreshold OrElse theta < -thetaThresholdRadians OrElse theta > thetaThresholdRadians

			Dim reward As Double
			If Not episodeFinished Then
				reward = 1.0
			ElseIf stepsBeyondDone Is Nothing Then
				stepsBeyondDone = 0
				reward = 1.0
			Else
				stepsBeyondDone += 1
				reward = 0
			End If

			Dim channelsData As IDictionary(Of String, Object) = New HashMapAnonymousInnerClass2(Me)
			Return New StepResult(channelsData, reward, episodeFinished)
		End Function

		Private Class HashMapAnonymousInnerClass2
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As CartpoleEnvironment

			Public Sub New(ByVal outerInstance As CartpoleEnvironment)
				Me.outerInstance = outerInstance

				Me.put("data", New Double(){outerInstance.x, outerInstance.xDot, outerInstance.theta, outerInstance.thetaDot})
			End Sub

		End Class

		Public Overridable Sub close() Implements Environment(Of Integer).close
			' Do nothing
		End Sub
	End Class
End Namespace