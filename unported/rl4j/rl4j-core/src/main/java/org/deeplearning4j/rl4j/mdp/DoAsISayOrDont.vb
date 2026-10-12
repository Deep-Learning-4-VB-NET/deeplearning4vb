Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports org.deeplearning4j.rl4j.environment
Imports IntegerActionSchema = org.deeplearning4j.rl4j.environment.IntegerActionSchema
Imports org.deeplearning4j.rl4j.environment
Imports StepResult = org.deeplearning4j.rl4j.environment.StepResult
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


	Public Class DoAsISayOrDont
		Implements Environment(Of Integer)

		Private Const NUM_ACTIONS As Integer = 2

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.environment.Schema<Integer> schema;
		Private ReadOnly schema As Schema(Of Integer)
		Private ReadOnly rnd As Random

		Private isOpposite As Boolean
		Private nextAction As Integer

		Public Sub New(ByVal rnd As Random)
			Me.rnd = If(rnd IsNot Nothing, rnd, Nd4j.Random)
			Me.schema = New Schema(Of Integer)(New IntegerActionSchema(NUM_ACTIONS, 0, rnd))
		End Sub

		Public Overridable Function reset() As IDictionary(Of String, Object) Implements Environment(Of Integer).reset
			nextAction = If(rnd.nextBoolean(), 1, 0)
			isOpposite = rnd.nextBoolean()
			Return getChannelsData(True)
		End Function

		Public Overridable Function [step](ByVal action As Integer?) As StepResult

			Dim reward As Double
			If isOpposite Then
				reward = If(action <> nextAction, 1.0, -1.0)
			Else
				reward = If(action = nextAction, 1.0, -1.0)
			End If

			Dim shouldReverse As Boolean = rnd.nextBoolean()
			If shouldReverse Then
				isOpposite = Not isOpposite
			End If

			Return New StepResult(getChannelsData(shouldReverse), reward, False)
		End Function

		Public Overridable ReadOnly Property EpisodeFinished As Boolean Implements Environment(Of Integer).isEpisodeFinished
			Get
				Return False
			End Get
		End Property


		Public Overridable Sub close() Implements Environment(Of Integer).close

		End Sub

		Private Function getChannelsData(ByVal showIndicators As Boolean) As IDictionary(Of String, Object)
			Dim normalModeIndicator As Double = If(showIndicators, (If(isOpposite, 0.0, 1.0)), -1.0)
			Dim oppositeModeIndicator As Double = If(showIndicators, (If(isOpposite, 1.0, 0.0)), -1.0)

			Return New HashMapAnonymousInnerClass(Me, normalModeIndicator, oppositeModeIndicator)
		End Function

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As DoAsISayOrDont

			Private normalModeIndicator As Double
			Private oppositeModeIndicator As Double

			Public Sub New(ByVal outerInstance As DoAsISayOrDont, ByVal normalModeIndicator As Double, ByVal oppositeModeIndicator As Double)
				Me.outerInstance = outerInstance
				Me.normalModeIndicator = normalModeIndicator
				Me.oppositeModeIndicator = oppositeModeIndicator

				Me.put("data", New Double(){ CDbl(outerInstance.nextAction), (1.0 - outerInstance.nextAction), normalModeIndicator, oppositeModeIndicator})
			End Sub

		End Class
	End Class

End Namespace