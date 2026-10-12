Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Tag(TagNames.JACKSON_SERDE) @Tag(TagNames.JAVA_ONLY) public class TestSchedules extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestSchedules
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJson() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testJson()

			Dim om As New ObjectMapper()
			om.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			om.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			om.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			om.enable(SerializationFeature.INDENT_OUTPUT)

			Dim schedules() As ISchedule = {
				New ExponentialSchedule(ScheduleType.ITERATION, 1.0, 0.5),
				New InverseSchedule(ScheduleType.ITERATION, 1.0, 0.5, 2),
				(New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 1.0).add(10,0.5).build(),
				New PolySchedule(ScheduleType.ITERATION, 1.0, 2, 100),
				New SigmoidSchedule(ScheduleType.ITERATION, 1.0, 0.5, 10),
				New StepSchedule(ScheduleType.ITERATION, 1.0, 0.9, 100),
				New CycleSchedule(ScheduleType.ITERATION, 1.5, 100)
			}


			For Each s As ISchedule In schedules
				Dim json As String = om.writeValueAsString(s)
				Dim fromJson As ISchedule = om.readValue(json, GetType(ISchedule))
				assertEquals(s, fromJson)
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testScheduleValues(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testScheduleValues(ByVal backend As Nd4jBackend)

			Dim lr As Double = 0.8
			Dim decay As Double = 0.9
			Dim power As Double = 2
			Dim gamma As Double = 0.5
			Dim [step] As Integer = 20

			For Each st As ScheduleType In System.Enum.GetValues(GetType(ScheduleType))

				Dim schedules() As ISchedule = {
					New ExponentialSchedule(st, lr, gamma),
					New InverseSchedule(st, lr, gamma, power),
					New PolySchedule(st, lr, power, [step]),
					New SigmoidSchedule(st, lr, gamma, [step]),
					New StepSchedule(st, lr, decay, [step])
				}

				For Each s As ISchedule In schedules

					For i As Integer = 0 To 8
						Dim epoch As Integer = i \ 3
						Dim x As Integer
						If st = ScheduleType.ITERATION Then
							x = i
						Else
							x = epoch
						End If

						Dim now As Double = s.valueAt(i, epoch)
						Dim e As Double
						If TypeOf s Is ExponentialSchedule Then
							e = calcExponentialDecay(lr, gamma, x)
						ElseIf TypeOf s Is InverseSchedule Then
							e = calcInverseDecay(lr, gamma, x, power)
						ElseIf TypeOf s Is PolySchedule Then
							e = calcPolyDecay(lr, x, power, [step])
						ElseIf TypeOf s Is SigmoidSchedule Then
							e = calcSigmoidDecay(lr, gamma, x, [step])
						ElseIf TypeOf s Is StepSchedule Then
							e = calcStepDecay(lr, decay, x, [step])
						Else
							Throw New Exception()
						End If

						assertEquals(e, now, 1e-6,s.ToString() & ", " & st)
					Next i
				Next s
			Next st
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMapSchedule(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMapSchedule(ByVal backend As Nd4jBackend)

			Dim schedule As ISchedule = (New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 0.5).add(5, 0.1).build()

			For i As Integer = 0 To 9
				If i < 5 Then
					assertEquals(0.5, schedule.valueAt(i, 0), 1e-6)
				Else
					assertEquals(0.1, schedule.valueAt(i, 0), 1e-6)
				End If
			Next i
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCycleSchedule(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCycleSchedule(ByVal backend As Nd4jBackend)
			Dim schedule As ISchedule = New CycleSchedule(ScheduleType.ITERATION, 1.5, 100)
			assertEquals(0.15, schedule.valueAt(0, 0), 1e-6)
			assertEquals(1.5, schedule.valueAt(45, 0), 1e-6)
			assertEquals(0.15, schedule.valueAt(90, 0), 1e-6)
			assertEquals(0.015, schedule.valueAt(91, 0), 1e-6)

			schedule = New CycleSchedule(ScheduleType.ITERATION, 0.95, 0.85, 100, 10, 1)
			assertEquals(0.95, schedule.valueAt(0, 0), 1e-6)
			assertEquals(0.85, schedule.valueAt(45, 0), 1e-6)
			assertEquals(0.95, schedule.valueAt(90, 0), 1e-6)
			assertEquals(0.95, schedule.valueAt(91, 0), 1e-6)
		End Sub

		Private Shared Function calcExponentialDecay(ByVal lr As Double, ByVal decayRate As Double, ByVal iteration As Double) As Double
			Return lr * Math.Pow(decayRate, iteration)
		End Function

		Private Shared Function calcInverseDecay(ByVal lr As Double, ByVal decayRate As Double, ByVal iteration As Double, ByVal power As Double) As Double
			Return lr / Math.Pow((1 + decayRate * iteration), power)
		End Function

		Private Shared Function calcStepDecay(ByVal lr As Double, ByVal decayRate As Double, ByVal iteration As Double, ByVal steps As Double) As Double
			Return lr * Math.Pow(decayRate, Math.Floor(iteration / steps))
		End Function

		Private Shared Function calcPolyDecay(ByVal lr As Double, ByVal iteration As Double, ByVal power As Double, ByVal maxIterations As Double) As Double
			Return lr * Math.Pow(1 + iteration / maxIterations, power)
		End Function

		Private Shared Function calcSigmoidDecay(ByVal lr As Double, ByVal decayRate As Double, ByVal iteration As Double, ByVal steps As Double) As Double
			Return lr / (1 + Math.Exp(-decayRate * (iteration - steps)))
		End Function

	End Class

End Namespace