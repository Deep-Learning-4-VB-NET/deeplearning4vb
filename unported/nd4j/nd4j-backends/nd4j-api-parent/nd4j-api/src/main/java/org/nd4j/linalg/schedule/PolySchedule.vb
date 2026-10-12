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
'ORIGINAL LINE: @Data public class PolySchedule implements ISchedule
	<Serializable>
	Public Class PolySchedule
		Implements ISchedule

		Private ReadOnly scheduleType As ScheduleType
		Private ReadOnly initialValue As Double
		Private ReadOnly power As Double
		Private ReadOnly maxIter As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PolySchedule(@JsonProperty("scheduleType") ScheduleType scheduleType, @JsonProperty("initialValue") double initialValue, @JsonProperty("power") double power, @JsonProperty("maxIter") int maxIter)
		Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialValue As Double, ByVal power As Double, ByVal maxIter As Integer)
			Me.scheduleType = scheduleType
			Me.initialValue = initialValue
			Me.power = power
			Me.maxIter = maxIter
		End Sub

		Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
			Dim i As Integer = (If(scheduleType = ScheduleType.ITERATION, iteration, epoch))

			If i >= maxIter Then
				Return 0
			End If

			Return initialValue * Math.Pow(1 + i / CDbl(maxIter), power)
		End Function

		Public Overridable Function clone() As PolySchedule
			Return New PolySchedule(scheduleType, initialValue, power, maxIter)
		End Function

	End Class

End Namespace