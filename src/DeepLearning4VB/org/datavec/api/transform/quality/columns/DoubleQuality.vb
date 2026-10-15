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

    ''' <summary>Quality report of a double column: adds the counts of non-real, NaN and infinite values.</summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class DoubleQuality
        Inherits ColumnQuality

        Private ReadOnly countNonReal As Long
        Private ReadOnly countNaN As Long
        Private ReadOnly countInfinite As Long

        Public Sub New()
            Me.New(0, 0, 0, 0, 0, 0, 0)
        End Sub

        Public Sub New(ByVal countValid As Long, ByVal countInvalid As Long, ByVal countMissing As Long, ByVal countTotal As Long, ByVal countNonReal As Long, ByVal countNaN As Long, ByVal countInfinite As Long)
            MyBase.New(countValid, countInvalid, countMissing, countTotal)
            Me.countNonReal = countNonReal
            Me.countNaN = countNaN
            Me.countInfinite = countInfinite
        End Sub

        Public Function getCountNonReal() As Long
            Return countNonReal
        End Function

        Public Function getCountNaN() As Long
            Return countNaN
        End Function

        Public Function getCountInfinite() As Long
            Return countInfinite
        End Function

        Public Overridable Function add(ByVal other As DoubleQuality) As DoubleQuality
            Return New DoubleQuality(countValid + other.countValid, countInvalid + other.countInvalid, countMissing + other.countMissing, countTotal + other.countTotal, countNonReal + other.countNonReal, countNaN + other.countNaN, countInfinite + other.countInfinite)
        End Function

        Public Overrides Function ToString() As String
            Return "DoubleQuality(" & MyBase.ToString() & ", countNonReal=" & countNonReal & ", countNaN=" & countNaN & ", countInfinite=" & countInfinite & ")"
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As DoubleQuality = DirectCast(obj, DoubleQuality)
            Return countNonReal = other.countNonReal AndAlso countNaN = other.countNaN AndAlso countInfinite = other.countInfinite
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), countNonReal, countNaN, countInfinite)
        End Function

    End Class

End Namespace
