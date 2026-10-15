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

Imports System

Namespace org.datavec.api.transform.quality.columns

    ''' <summary>Quality report of an integer column: adds the count of non-integer values.</summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class IntegerQuality
        Inherits ColumnQuality

        Private ReadOnly countNonInteger As Long

        Public Sub New()
            Me.New(0, 0, 0, 0, 0)
        End Sub

        Public Sub New(ByVal countValid As Long, ByVal countInvalid As Long, ByVal countMissing As Long, ByVal countTotal As Long, ByVal countNonInteger As Long)
            MyBase.New(countValid, countInvalid, countMissing, countTotal)
            Me.countNonInteger = countNonInteger
        End Sub

        Public Function getCountNonInteger() As Long
            Return countNonInteger
        End Function

        Public Overridable Function add(ByVal other As IntegerQuality) As IntegerQuality
            Return New IntegerQuality(countValid + other.countValid, countInvalid + other.countInvalid, countMissing + other.countMissing, countTotal + other.countTotal, countNonInteger + other.countNonInteger)
        End Function

        Public Overrides Function ToString() As String
            Return "IntegerQuality(" & MyBase.ToString() & ", countNonInteger=" & countNonInteger & ")"
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Return countNonInteger = DirectCast(obj, IntegerQuality).countNonInteger
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), countNonInteger)
        End Function

    End Class

End Namespace
