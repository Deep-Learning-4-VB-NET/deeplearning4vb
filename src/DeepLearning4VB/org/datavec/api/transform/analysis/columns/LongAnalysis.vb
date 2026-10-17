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

    ''' <summary>Analysis of a long column: the numerical statistics plus the minimum and maximum.</summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data @NoArgsConstructor
    <Serializable>
    Public Class LongAnalysis
        Inherits NumericalColumnAnalysis

        Private min As Long
        Private max As Long

        Public Sub New()
        End Sub

        Private Sub New(ByVal builder As Builder)
            MyBase.New(builder)
            Me.min = builder.minValue
            Me.max = builder.maxValue
        End Sub

        Public Overrides Function ToString() As String
            Return "LongAnalysis(min=" & min & ",max=" & max & "," & MyBase.ToString() & ")"
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
                Return ColumnTypeEnum.Long
            End Get
        End Property

        ' ---- Lombok @Data accessors ----

        Public Function getMin() As Long
            Return min
        End Function

        Public Sub setMin(ByVal min As Long)
            Me.min = min
        End Sub

        Public Function getMax() As Long
            Return max
        End Function

        Public Sub setMax(ByVal max As Long)
            Me.max = max
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As LongAnalysis = DirectCast(obj, LongAnalysis)
            Return min = other.min AndAlso max = other.max
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), min, max)
        End Function

        Public Class Builder
            Inherits NumericalColumnAnalysis.Builder(Of Builder)

            Friend minValue As Long
            Friend maxValue As Long

            Public Overridable Function min(ByVal value As Long) As Builder
                Me.minValue = value
                Return Me
            End Function

            Public Overridable Function max(ByVal value As Long) As Builder
                Me.maxValue = value
                Return Me
            End Function

            Public Overridable Function build() As LongAnalysis
                Return New LongAnalysis(Me)
            End Function
        End Class
    End Class

End Namespace
