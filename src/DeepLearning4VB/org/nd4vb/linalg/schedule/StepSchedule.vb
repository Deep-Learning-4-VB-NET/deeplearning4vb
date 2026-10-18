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
    ''' A step schedule: <c>value(i) = initialValue * decayRate ^ floor(i / step)</c>, where i is the iteration or
    ''' epoch (depending on the schedule type).
    ''' </summary>
    <Serializable>
    Public Class StepSchedule
        Implements ISchedule

        Private ReadOnly scheduleType As ScheduleType
        Private ReadOnly initialValue As Double
        Private ReadOnly decayRate As Double
        Private ReadOnly [step] As Double

        <JsonConstructor>
        Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialValue As Double, ByVal decayRate As Double, ByVal [step] As Double)
            Me.scheduleType = scheduleType
            Me.initialValue = initialValue
            Me.decayRate = decayRate
            Me.step = [step]
        End Sub

        Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
            Dim i As Integer = If(scheduleType = ScheduleType.ITERATION, iteration, epoch)
            Return initialValue * Math.Pow(decayRate, Math.Floor(i / [step]))
        End Function

        Public Overridable Function clone() As ISchedule Implements ISchedule.clone
            Return New StepSchedule(scheduleType, initialValue, decayRate, [step])
        End Function

        Public Overridable Function getScheduleType() As ScheduleType
            Return scheduleType
        End Function

        Public Overridable Function getInitialValue() As Double
            Return initialValue
        End Function

        Public Overridable Function getDecayRate() As Double
            Return decayRate
        End Function

        Public Overridable Function getStep() As Double
            Return [step]
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As StepSchedule = TryCast(obj, StepSchedule)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return scheduleType.Equals(o.scheduleType) AndAlso initialValue.Equals(o.initialValue) AndAlso decayRate.Equals(o.decayRate) AndAlso [step].Equals(o.[step])
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + scheduleType.GetHashCode()
            h = h * 59 + initialValue.GetHashCode()
            h = h * 59 + decayRate.GetHashCode()
            h = h * 59 + [step].GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "StepSchedule(" & "scheduleType=" & scheduleType.ToString() & ", initialValue=" & Arrays.JavaDoubleToString(initialValue) & ", decayRate=" & Arrays.JavaDoubleToString(decayRate) & ", step=" & Arrays.JavaDoubleToString([step]) & ")"
        End Function

    End Class

End Namespace
