Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.util
Imports DeepLearning4VB.org.nd4vb.linalg.schedule

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.schedule

    ''' <summary>
    ''' Ported from the ND4J JUnit test of the same name. Java's <c>testJson</c> round trip (Jackson) is replaced by an
    ''' equality/clone/serialization round trip, because the schedules carry no JSON type information yet.
    ''' </summary>
    <TestClass>
    Public Class TestSchedules

        Private Shared Function allSchedules() As ISchedule()
            Return New ISchedule() {
                New ExponentialSchedule(ScheduleType.ITERATION, 1.0, 0.5),
                New InverseSchedule(ScheduleType.ITERATION, 1.0, 0.5, 2),
                (New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 1.0).add(10, 0.5).build(),
                New PolySchedule(ScheduleType.ITERATION, 1.0, 2, 100),
                New SigmoidSchedule(ScheduleType.ITERATION, 1.0, 0.5, 10),
                New StepSchedule(ScheduleType.ITERATION, 1.0, 0.9, 100),
                New CycleSchedule(ScheduleType.ITERATION, 1.5, 100),
                New FixedSchedule(0.25),
                New RampSchedule(New FixedSchedule(1.0), 4)
            }
        End Function

        <TestMethod>
        Public Sub testCloneEqualsAndSerialization()
            For Each s As ISchedule In allSchedules()
                Dim c As ISchedule = s.clone()
                Assert.AreNotSame(s, c)
                assertEquals(s, c)
                assertEquals(s.GetHashCode(), c.GetHashCode())
                assertEquals(s.ToString(), c.ToString())

                Dim bytes As Byte() = SerializationUtils.toByteArray(s)
                Dim back As ISchedule = SerializationUtils.deserialize(Of ISchedule)(bytes)
                assertEquals(s, back)
                For i As Integer = 0 To 120 Step 7
                    assertEquals(s.valueAt(i, i \ 10), back.valueAt(i, i \ 10), 0.0)
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub testEqualityIsByValue()
            assertEquals(New ExponentialSchedule(ScheduleType.EPOCH, 1.0, 0.5), New ExponentialSchedule(ScheduleType.EPOCH, 1.0, 0.5))
            assertFalse(New ExponentialSchedule(ScheduleType.EPOCH, 1.0, 0.5).Equals(New ExponentialSchedule(ScheduleType.ITERATION, 1.0, 0.5)))
            assertFalse(New ExponentialSchedule(ScheduleType.EPOCH, 1.0, 0.5).Equals(New ExponentialSchedule(ScheduleType.EPOCH, 1.0, 0.6)))
            ' same fields, different class
            assertFalse(New InverseSchedule(ScheduleType.EPOCH, 1.0, 0.5, 2).Equals(New PolySchedule(ScheduleType.EPOCH, 1.0, 0.5, 2)))
            assertEquals((New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 1.0).add(3, 2.0).build(),
                         (New MapSchedule.Builder(ScheduleType.ITERATION)).add(3, 2.0).add(0, 1.0).build())
            assertFalse((New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 1.0).add(3, 2.0).build().Equals(
                         (New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 1.0).add(3, 2.5).build()))
        End Sub

        <TestMethod>
        Public Sub testToStringLikeLombok()
            assertEquals("ExponentialSchedule(scheduleType=ITERATION, initialValue=1.0, gamma=0.5)", New ExponentialSchedule(ScheduleType.ITERATION, 1.0, 0.5).ToString())
            assertEquals("StepSchedule(scheduleType=EPOCH, initialValue=0.1, decayRate=0.9, step=100.0)", New StepSchedule(ScheduleType.EPOCH, 0.1, 0.9, 100).ToString())
            assertEquals("FixedSchedule(value=1.0E-4)", New FixedSchedule(0.0001).ToString())
            Dim m As MapSchedule = (New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 1.0).add(10, 0.5).build()
            assertTrue(m.ToString().StartsWith("MapSchedule(scheduleType=ITERATION, values={0=1.0, 10=0.5}, allKeysSorted=[0, 10]"), m.ToString())
        End Sub

        <TestMethod>
        Public Sub testGetters()
            Dim c As New CycleSchedule(ScheduleType.ITERATION, 1.5, 100)
            assertEquals(0.15, c.getInitialLearningRate(), 1e-12)
            assertEquals(1.5, c.getMaxLearningRate(), 1e-12)
            assertEquals(100, c.getCycleLength())
            assertEquals(10, c.getAnnealingLength())
            assertEquals(45, c.getStepSize())
            assertEquals(0.03, c.getIncrement(), 1e-12)
            assertEquals(0.1, c.getAnnealingDecay(), 1e-12)
            c.setAnnealingDecay(0.5)
            assertEquals(0.5, c.getAnnealingDecay(), 1e-12)

            Dim s As New StepSchedule(ScheduleType.EPOCH, 0.1, 0.9, 100)
            assertEquals(ScheduleType.EPOCH, s.getScheduleType())
            assertEquals(100.0, s.getStep(), 0.0)
            assertEquals(0.9, s.getDecayRate(), 0.0)
        End Sub

        <TestMethod>
        Public Sub testScheduleValues()
            Dim lr As Double = 0.8
            Dim decay As Double = 0.9
            Dim power As Double = 2
            Dim gamma As Double = 0.5
            Dim [step] As Integer = 20

            For Each st As ScheduleType In [Enum].GetValues(GetType(ScheduleType))
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

                        assertEquals(e, now, 1e-6, s.ToString() & ", " & st.ToString())
                    Next i
                Next s
            Next st
        End Sub

        <TestMethod>
        Public Sub testPolyScheduleIsZeroAfterMaxIter()
            Dim s As New PolySchedule(ScheduleType.ITERATION, 1.0, 2, 10)
            assertEquals(0.0, s.valueAt(10, 0), 0.0)
            assertEquals(0.0, s.valueAt(11, 0), 0.0)
        End Sub

        <TestMethod>
        Public Sub testMapSchedule()
            Dim schedule As ISchedule = (New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 0.5).add(5, 0.1).build()

            For i As Integer = 0 To 9
                If i < 5 Then
                    assertEquals(0.5, schedule.valueAt(i, 0), 1e-6)
                Else
                    assertEquals(0.1, schedule.valueAt(i, 0), 1e-6)
                End If
            Next i

            ' epoch-based, with a gap between keys
            Dim byEpoch As ISchedule = (New MapSchedule.Builder(ScheduleType.EPOCH)).add(0, 3.0).add(2, 2.0).add(7, 1.0).build()
            assertEquals(3.0, byEpoch.valueAt(100, 1), 0.0)
            assertEquals(2.0, byEpoch.valueAt(0, 6), 0.0)
            assertEquals(1.0, byEpoch.valueAt(0, 70), 0.0)
        End Sub

        <TestMethod>
        Public Sub testMapScheduleRequiresInitialValue()
            Dim values As New Dictionary(Of Integer, Double) From {{1, 0.5}}
            Dim ex = assertThrows(Of ArgumentException)(Sub()
                                                              Dim unused As New MapSchedule(ScheduleType.ITERATION, values)
                                                          End Sub)
            assertEquals("Invalid set of values: must contain initial value (position 0)", ex.Message)
        End Sub

        <TestMethod>
        Public Sub testCycleSchedule()
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

            ' the cycle repeats, and EPOCH uses the epoch as the position
            Dim byEpoch As ISchedule = New CycleSchedule(ScheduleType.EPOCH, 1.5, 100)
            assertEquals(1.5, byEpoch.valueAt(0, 45), 1e-6)
            assertEquals(1.5, byEpoch.valueAt(0, 145), 1e-6)
        End Sub

        <TestMethod>
        Public Sub testRampSchedule()
            Dim s As ISchedule = New RampSchedule(New FixedSchedule(2.0), 4)
            assertEquals(0.5, s.valueAt(0, 0), 1e-12)
            assertEquals(1.0, s.valueAt(1, 0), 1e-12)
            assertEquals(1.5, s.valueAt(2, 0), 1e-12)
            assertEquals(2.0, s.valueAt(3, 0), 1e-12)
            assertEquals(2.0, s.valueAt(50, 0), 1e-12)
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
