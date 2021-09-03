Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Model = org.deeplearning4j.nn.api.Model
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener

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
'ORIGINAL LINE: @Slf4j public class TimeIterationListener extends org.deeplearning4j.optimize.api.BaseTrainingListener implements java.io.Serializable
	<Serializable>
	Public Class TimeIterationListener
		Inherits BaseTrainingListener

		Private ReadOnly frequency As Integer
		Private ReadOnly start As Long
		Private ReadOnly iterationCount As Integer
		Private ReadOnly iterationCounter As New AtomicLong(0)

		''' <summary>
		''' Constructor </summary>
		''' <param name="iterationCount"> The global number of iteration for training (all epochs) </param>
		Public Sub New(ByVal iterationCount As Integer)
			Me.New(1, iterationCount)
		End Sub

		''' <summary>
		''' Constructor </summary>
		''' <param name="frequency"> frequency with which to print the remaining time </param>
		''' <param name="iterationCount"> The global number of iteration for training (all epochs) </param>
		Public Sub New(ByVal frequency As Integer, ByVal iterationCount As Integer)
			Me.frequency = frequency
			Me.iterationCount = iterationCount
			Me.start = DateTimeHelper.CurrentUnixTimeMillis()
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			Dim currentIteration As Long = iterationCounter.incrementAndGet()
			If iteration Mod frequency = 0 Then
				Dim elapsed As Long = DateTimeHelper.CurrentUnixTimeMillis() - start
				Dim remaining As Long = (iterationCount - currentIteration) * elapsed \ currentIteration
				Dim minutes As Long = remaining \ (1000 * 60)
				Dim [date] As New DateTime(start + elapsed + remaining)
				log.info("Remaining time : " & minutes & "mn - End expected : " & [date].ToString())
			End If
		End Sub

	End Class

End Namespace