Imports System
Imports Data = lombok.Data
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.nd4j.linalg.schedule

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class CycleSchedule implements ISchedule
	<Serializable>
	Public Class CycleSchedule
		Implements ISchedule

		Private ReadOnly scheduleType As ScheduleType
		Private ReadOnly initialLearningRate As Double
		Private ReadOnly maxLearningRate As Double
		Private ReadOnly cycleLength As Integer
		Private ReadOnly annealingLength As Integer
		Private ReadOnly stepSize As Integer
		Private ReadOnly increment As Double
		Private annealingDecay As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CycleSchedule(@JsonProperty("scheduleType") ScheduleType scheduleType, @JsonProperty("initialLearningRate") double initialLearningRate, @JsonProperty("maxLearningRate") double maxLearningRate, @JsonProperty("cycleLength") int cycleLength, @JsonProperty("annealingLength") int annealingLength, @JsonProperty("annealingDecay") double annealingDecay)
		Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialLearningRate As Double, ByVal maxLearningRate As Double, ByVal cycleLength As Integer, ByVal annealingLength As Integer, ByVal annealingDecay As Double)
			Me.scheduleType = scheduleType
			Me.initialLearningRate = initialLearningRate
			Me.maxLearningRate = maxLearningRate
			Me.cycleLength = cycleLength
			Me.annealingDecay = annealingDecay
			Me.annealingLength = annealingLength

			stepSize = ((cycleLength - annealingLength) \ 2)
			increment = (maxLearningRate - initialLearningRate) / stepSize
		End Sub

		Public Sub New(ByVal scheduleType As ScheduleType, ByVal maxLearningRate As Double, ByVal cycleLength As Integer)
			Me.New(scheduleType, maxLearningRate * 0.1, maxLearningRate, cycleLength, CInt(CLng(Math.Round(cycleLength * 0.1, MidpointRounding.AwayFromZero))), 0.1)
		End Sub


		Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
			Dim learningRate As Double
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int positionInCycle = (scheduleType == ScheduleType.EPOCH ? epoch : iteration) % cycleLength;
			Dim positionInCycle As Integer = (If(scheduleType = ScheduleType.EPOCH, epoch, iteration)) Mod cycleLength

			If positionInCycle < stepSize Then
				learningRate = initialLearningRate + increment * positionInCycle
			ElseIf positionInCycle < 2*stepSize Then
				learningRate = maxLearningRate - increment * (positionInCycle - stepSize)
			Else
				learningRate = initialLearningRate * Math.Pow(annealingDecay, annealingLength - (cycleLength - positionInCycle))
			End If

			Return learningRate
		End Function

		Public Overridable Function clone() As ISchedule Implements ISchedule.clone
			Return New CycleSchedule(scheduleType, initialLearningRate, maxLearningRate, cycleLength, annealingLength, annealingDecay)
		End Function
	End Class

End Namespace