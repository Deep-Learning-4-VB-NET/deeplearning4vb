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
    ''' A sigmoid schedule: <c>value(i) = initialValue / (1 + exp(-gamma * (i - stepSize)))</c>, where i is the
    ''' iteration or epoch (depending on the schedule type).
    ''' </summary>
    <Serializable>
    Public Class SigmoidSchedule
        Implements ISchedule

        Private ReadOnly scheduleType As ScheduleType
        Private ReadOnly initialValue As Double
        Private ReadOnly gamma As Double
        Private ReadOnly stepSize As Integer

        <JsonConstructor>
        Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialValue As Double, ByVal gamma As Double, ByVal stepSize As Integer)
            Me.scheduleType = scheduleType
            Me.initialValue = initialValue
            Me.gamma = gamma
            Me.stepSize = stepSize
        End Sub

        Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
            Dim i As Integer = If(scheduleType = ScheduleType.ITERATION, iteration, epoch)
            Return initialValue / (1.0 + Math.Exp(-gamma * (i - stepSize)))
        End Function

        Public Overridable Function clone() As ISchedule Implements ISchedule.clone
            Return New SigmoidSchedule(scheduleType, initialValue, gamma, stepSize)
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

        Public Overridable Function getStepSize() As Integer
            Return stepSize
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As SigmoidSchedule = TryCast(obj, SigmoidSchedule)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return scheduleType.Equals(o.scheduleType) AndAlso initialValue.Equals(o.initialValue) AndAlso gamma.Equals(o.gamma) AndAlso stepSize.Equals(o.stepSize)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + scheduleType.GetHashCode()
            h = h * 59 + initialValue.GetHashCode()
            h = h * 59 + gamma.GetHashCode()
            h = h * 59 + stepSize.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "SigmoidSchedule(" & "scheduleType=" & scheduleType.ToString() & ", initialValue=" & Arrays.JavaDoubleToString(initialValue) & ", gamma=" & Arrays.JavaDoubleToString(gamma) & ", stepSize=" & stepSize.ToString() & ")"
        End Function

    End Class

End Namespace
