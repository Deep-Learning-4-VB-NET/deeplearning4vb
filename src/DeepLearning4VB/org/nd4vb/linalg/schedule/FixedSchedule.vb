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
    ''' A fixed schedule: the value is the same for every iteration and epoch.
    ''' </summary>
    <Serializable>
    Public Class FixedSchedule
        Implements ISchedule

        Private ReadOnly value As Double

        <JsonConstructor>
        Public Sub New(ByVal value As Double)
            Me.value = value
        End Sub

        Public Overridable Function valueAt(ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements ISchedule.valueAt
            Return value
        End Function

        Public Overridable Function clone() As ISchedule Implements ISchedule.clone
            Return New FixedSchedule(value)
        End Function

        Public Overridable Function getValue() As Double
            Return value
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As FixedSchedule = TryCast(obj, FixedSchedule)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return value.Equals(o.value)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + value.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "FixedSchedule(" & "value=" & Arrays.JavaDoubleToString(value) & ")"
        End Function

    End Class

End Namespace
