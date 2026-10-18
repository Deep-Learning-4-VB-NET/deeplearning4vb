Imports System
Imports System.Text.Json.Serialization

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

Namespace org.nd4vb.linalg.schedule

    ''' <summary>
    ''' The "1cycle" schedule: the value rises linearly from <c>initialLearningRate</c> to <c>maxLearningRate</c> in
    ''' the first half of the cycle (minus the annealing part), falls back linearly in the second half and then decays
    ''' by <c>annealingDecay</c> per step during the last <c>annealingLength</c> steps. The position in the cycle is
    ''' taken from the iteration or the epoch, depending on the schedule type.
    ''' </summary>
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

        <JsonConstructor>
        Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialLearningRate As Double, ByVal maxLearningRate As Double, ByVal cycleLength As Integer, ByVal annealingLength As Integer, ByVal annealingDecay As Double)
            Me.scheduleType = scheduleType
            Me.initialLearningRate = initialLearningRate
            Me.maxLearningRate = maxLearningRate
            Me.cycleLength = cycleLength
            Me.annealingDecay = annealingDecay
            Me.annealingLength = annealingLength

            stepSize = (cycleLength - annealingLength) \ 2
            increment = (maxLearningRate - initialLearningRate) / stepSize
        End Sub

        ''' <summary>
        ''' A cycle from <c>maxLearningRate * 0.1</c> to <c>maxLearningRate</c>, with the last 10% of the cycle (Java
        ''' <c>Math.round</c>) annealing by a factor of 0.1 per step.
        ''' </summary>
        Public Sub New(ByVal scheduleType As ScheduleType, ByVal maxLearningRate As Double, ByVal cycleLength As Integer)
            Me.New(scheduleType, maxLearningRate * 0.1, maxLearningRate, cycleLength, CInt(Math.Floor(cycleLength * 0.1 + 0.5)), 0.1)
        End Sub

        Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
            Dim learningRate As Double
            Dim positionInCycle As Integer = If(scheduleType = ScheduleType.EPOCH, epoch, iteration) Mod cycleLength

            If positionInCycle < stepSize Then
                learningRate = initialLearningRate + increment * positionInCycle
            ElseIf positionInCycle < 2 * stepSize Then
                learningRate = maxLearningRate - increment * (positionInCycle - stepSize)
            Else
                learningRate = initialLearningRate * Math.Pow(annealingDecay, annealingLength - (cycleLength - positionInCycle))
            End If

            Return learningRate
        End Function

        Public Overridable Function clone() As ISchedule Implements ISchedule.clone
            Return New CycleSchedule(scheduleType, initialLearningRate, maxLearningRate, cycleLength, annealingLength, annealingDecay)
        End Function

        Public Overridable Function getScheduleType() As ScheduleType
            Return scheduleType
        End Function

        Public Overridable Function getInitialLearningRate() As Double
            Return initialLearningRate
        End Function

        Public Overridable Function getMaxLearningRate() As Double
            Return maxLearningRate
        End Function

        Public Overridable Function getCycleLength() As Integer
            Return cycleLength
        End Function

        Public Overridable Function getAnnealingLength() As Integer
            Return annealingLength
        End Function

        Public Overridable Function getStepSize() As Integer
            Return stepSize
        End Function

        Public Overridable Function getIncrement() As Double
            Return increment
        End Function

        Public Overridable Function getAnnealingDecay() As Double
            Return annealingDecay
        End Function

        Public Overridable Sub setAnnealingDecay(ByVal annealingDecay As Double)
            Me.annealingDecay = annealingDecay
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As CycleSchedule = TryCast(obj, CycleSchedule)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return scheduleType.Equals(o.scheduleType) AndAlso initialLearningRate.Equals(o.initialLearningRate) AndAlso maxLearningRate.Equals(o.maxLearningRate) AndAlso cycleLength.Equals(o.cycleLength) AndAlso annealingLength.Equals(o.annealingLength) AndAlso stepSize.Equals(o.stepSize) AndAlso increment.Equals(o.increment) AndAlso annealingDecay.Equals(o.annealingDecay)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + scheduleType.GetHashCode()
            h = h * 59 + initialLearningRate.GetHashCode()
            h = h * 59 + maxLearningRate.GetHashCode()
            h = h * 59 + cycleLength.GetHashCode()
            h = h * 59 + annealingLength.GetHashCode()
            h = h * 59 + stepSize.GetHashCode()
            h = h * 59 + increment.GetHashCode()
            h = h * 59 + annealingDecay.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "CycleSchedule(" & "scheduleType=" & scheduleType.ToString() & ", initialLearningRate=" & Arrays.JavaDoubleToString(initialLearningRate) & ", maxLearningRate=" & Arrays.JavaDoubleToString(maxLearningRate) & ", cycleLength=" & cycleLength.ToString() & ", annealingLength=" & annealingLength.ToString() & ", stepSize=" & stepSize.ToString() & ", increment=" & Arrays.JavaDoubleToString(increment) & ", annealingDecay=" & Arrays.JavaDoubleToString(annealingDecay) & ")"
        End Function

    End Class

End Namespace
