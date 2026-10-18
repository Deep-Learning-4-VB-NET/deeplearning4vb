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
    ''' An exponential schedule: <c>value(i) = initialValue * gamma^i</c>, where i is the iteration or epoch
    ''' (depending on the schedule type).
    ''' </summary>
    <Serializable>
    Public Class ExponentialSchedule
        Implements ISchedule

        Private ReadOnly scheduleType As ScheduleType
        Private ReadOnly initialValue As Double
        Private ReadOnly gamma As Double

        <JsonConstructor>
        Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialValue As Double, ByVal gamma As Double)
            Me.scheduleType = scheduleType
            Me.initialValue = initialValue
            Me.gamma = gamma
        End Sub

        Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
            Dim i As Integer = If(scheduleType = ScheduleType.ITERATION, iteration, epoch)
            Return initialValue * Math.Pow(gamma, i)
        End Function

        Public Overridable Function clone() As ISchedule Implements ISchedule.clone
            Return New ExponentialSchedule(scheduleType, initialValue, gamma)
        End Function

        Public Overridable Function getScheduleType() As ScheduleType
            Return scheduleType
        End Function

        Public Overridable Function getInitialValue() As Double
            Return initialValue
        End Function

        Public Overridable Function getGamma() As Double
            Return gamma
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As ExponentialSchedule = TryCast(obj, ExponentialSchedule)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return scheduleType.Equals(o.scheduleType) AndAlso initialValue.Equals(o.initialValue) AndAlso gamma.Equals(o.gamma)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + scheduleType.GetHashCode()
            h = h * 59 + initialValue.GetHashCode()
            h = h * 59 + gamma.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "ExponentialSchedule(" & "scheduleType=" & scheduleType.ToString() & ", initialValue=" & Arrays.JavaDoubleToString(initialValue) & ", gamma=" & Arrays.JavaDoubleToString(gamma) & ")"
        End Function

    End Class

End Namespace
