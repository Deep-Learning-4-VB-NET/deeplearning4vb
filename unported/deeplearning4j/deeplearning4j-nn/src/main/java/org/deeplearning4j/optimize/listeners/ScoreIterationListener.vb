Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
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

Namespace org.deeplearning4j.optimize.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ScoreIterationListener extends org.deeplearning4j.optimize.api.BaseTrainingListener implements java.io.Serializable
	<Serializable>
	Public Class ScoreIterationListener
		Inherits BaseTrainingListener

		Private printIterations As Integer = 10

		''' <param name="printIterations">    frequency with which to print scores (i.e., every printIterations parameter updates) </param>
		Public Sub New(ByVal printIterations As Integer)
			Me.printIterations = printIterations
		End Sub

		''' <summary>
		''' Default constructor printing every 10 iterations </summary>
		Public Sub New()
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			If printIterations <= 0 Then
				printIterations = 1
			End If
			If iteration Mod printIterations = 0 Then
				Dim score As Double = model.score()
				log.info("Score at iteration {} is {}", iteration, score)
			End If
		End Sub

		Public Overrides Function ToString() As String
			Return "ScoreIterationListener(" & printIterations & ")"
		End Function
	End Class

End Namespace