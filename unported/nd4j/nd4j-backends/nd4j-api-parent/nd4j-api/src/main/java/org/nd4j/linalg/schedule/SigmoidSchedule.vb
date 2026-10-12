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
'ORIGINAL LINE: @Data public class SigmoidSchedule implements ISchedule
	<Serializable>
	Public Class SigmoidSchedule
		Implements ISchedule

		Private ReadOnly scheduleType As ScheduleType
		Private ReadOnly initialValue As Double
		Private ReadOnly gamma As Double
		Private ReadOnly stepSize As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SigmoidSchedule(@JsonProperty("scheduleType") ScheduleType scheduleType, @JsonProperty("initialValue") double initialValue, @JsonProperty("gamma") double gamma, @JsonProperty("stepSize") int stepSize)
		Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialValue As Double, ByVal gamma As Double, ByVal stepSize As Integer)
			Me.scheduleType = scheduleType
			Me.initialValue = initialValue
			Me.gamma = gamma
			Me.stepSize = stepSize
		End Sub


		Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
			Dim i As Integer = (If(scheduleType = ScheduleType.ITERATION, iteration, epoch))
			Return initialValue / (1.0 + Math.Exp(-gamma * (i - stepSize)))
		End Function

		Public Overridable Function clone() As ISchedule Implements ISchedule.clone
			Return New SigmoidSchedule(scheduleType, initialValue, gamma, stepSize)
		End Function
	End Class

End Namespace