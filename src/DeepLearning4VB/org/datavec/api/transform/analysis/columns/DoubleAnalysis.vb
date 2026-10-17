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
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>
    ''' Analysis of a double (or float) column: the numerical statistics plus the minimum, maximum and the count of
    ''' NaN values (which, as in Java, is not part of <see cref="ToString"/>).
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor
    <Serializable>
    Public Class DoubleAnalysis
        Inherits NumericalColumnAnalysis

        Private min As Double
        Private max As Double
        Private countNaN As Long

        Public Sub New()
        End Sub

        Private Sub New(ByVal builder As Builder)
            MyBase.New(builder)
            Me.min = builder.minValue
            Me.max = builder.maxValue
            Me.countNaN = builder.countNaNValue
        End Sub

        Public Overrides Function ToString() As String
            Return "DoubleAnalysis(min=" & Arrays.JavaDoubleToString(min) & ",max=" & Arrays.JavaDoubleToString(max) & "," & MyBase.ToString() & ")"
        End Function

        Public Overrides ReadOnly Property MinDouble As Double
            Get
                Return min
            End Get
        End Property

        Public Overrides ReadOnly Property MaxDouble As Double
            Get
                Return max
            End Get
        End Property

        Public Overrides ReadOnly Property ColumnType As ColumnTypeEnum
            Get
                Return ColumnTypeEnum.Double
            End Get
        End Property

        ' ---- Lombok @Data accessors ----

        Public Function getMin() As Double
            Return min
        End Function

        Public Sub setMin(ByVal min As Double)
            Me.min = min
        End Sub

        Public Function getMax() As Double
            Return max
        End Function

        Public Sub setMax(ByVal max As Double)
            Me.max = max
        End Sub

        Public Function getCountNaN() As Long
            Return countNaN
        End Function

        Public Sub setCountNaN(ByVal countNaN As Long)
            Me.countNaN = countNaN
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As DoubleAnalysis = DirectCast(obj, DoubleAnalysis)
            Return min.Equals(other.min) AndAlso max.Equals(other.max) AndAlso countNaN = other.countNaN
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), min, max, countNaN)
        End Function

        Public Class Builder
            Inherits NumericalColumnAnalysis.Builder(Of Builder)

            Friend minValue As Double
            Friend maxValue As Double
            Friend countNaNValue As Long

            Public Overridable Function min(ByVal value As Double) As Builder
                Me.minValue = value
                Return Me
            End Function

            Public Overridable Function max(ByVal value As Double) As Builder
                Me.maxValue = value
                Return Me
            End Function

            Public Overridable Function countNaN(ByVal value As Long) As Builder
                Me.countNaNValue = value
                Return Me
            End Function

            Public Overridable Function build() As DoubleAnalysis
                Return New DoubleAnalysis(Me)
            End Function
        End Class
    End Class

End Namespace
