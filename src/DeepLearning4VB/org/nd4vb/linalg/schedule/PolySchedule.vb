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
    ''' A polynomial schedule: <c>value(i) = initialValue * (1 + i/maxIter)^power</c> for <c>i &lt; maxIter</c>, and 0
    ''' afterwards, where i is the iteration or epoch (depending on the schedule type). The formula is the Java one.
    ''' </summary>
    <Serializable>
    Public Class PolySchedule
        Implements ISchedule

        Private ReadOnly scheduleType As ScheduleType
        Private ReadOnly initialValue As Double
        Private ReadOnly power As Double
        Private ReadOnly maxIter As Integer

        <JsonConstructor>
        Public Sub New(ByVal scheduleType As ScheduleType, ByVal initialValue As Double, ByVal power As Double, ByVal maxIter As Integer)
            Me.scheduleType = scheduleType
            Me.initialValue = initialValue
            Me.power = power
            Me.maxIter = maxIter
        End Sub

        Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
            Dim i As Integer = If(scheduleType = ScheduleType.ITERATION, iteration, epoch)
            If i >= maxIter Then
                Return 0
            End If
            Return initialValue * Math.Pow(1 + i / CDbl(maxIter), power)
        End Function

        ''' <summary>A copy of this schedule (Java declares the covariant return type <c>PolySchedule</c>).</summary>
        Public Overridable Function clone() As ISchedule Implements ISchedule.clone
            Return New PolySchedule(scheduleType, initialValue, power, maxIter)
        End Function

        Public Overridable Function getScheduleType() As ScheduleType
            Return scheduleType
        End Function

        Public Overridable Function getInitialValue() As Double
            Return initialValue
        End Function

        Public Overridable Function getPower() As Double
            Return power
        End Function

        Public Overridable Function getMaxIter() As Integer
            Return maxIter
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As PolySchedule = TryCast(obj, PolySchedule)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return scheduleType.Equals(o.scheduleType) AndAlso initialValue.Equals(o.initialValue) AndAlso power.Equals(o.power) AndAlso maxIter.Equals(o.maxIter)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + scheduleType.GetHashCode()
            h = h * 59 + initialValue.GetHashCode()
            h = h * 59 + power.GetHashCode()
            h = h * 59 + maxIter.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "PolySchedule(" & "scheduleType=" & scheduleType.ToString() & ", initialValue=" & Arrays.JavaDoubleToString(initialValue) & ", power=" & Arrays.JavaDoubleToString(power) & ", maxIter=" & maxIter.ToString() & ")"
        End Function

    End Class

End Namespace
