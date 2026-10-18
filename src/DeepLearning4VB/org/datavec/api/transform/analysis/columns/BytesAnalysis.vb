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
Imports System.Text.Json.Serialization
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>Analysis of a bytes column (only the number of values is filled in by the analysis).</summary>
    ' Java: @AllArgsConstructor @Data @NoArgsConstructor
    <Serializable>
    Public Class BytesAnalysis
        Implements ColumnAnalysis

        ''' <summary>Java field <c>countTotal</c> (renamed: VB names are case-insensitive and <see cref="CountTotal"/> exists).</summary>
        <JsonPropertyName("countTotal")>
        Private countTotalValue As Long
        Private countNull As Long
        Private countZeroLength As Long
        Private minNumBytes As Integer
        Private maxNumBytes As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal countTotal As Long, ByVal countNull As Long, ByVal countZeroLength As Long, ByVal minNumBytes As Integer, ByVal maxNumBytes As Integer)
            Me.countTotalValue = countTotal
            Me.countNull = countNull
            Me.countZeroLength = countZeroLength
            Me.minNumBytes = minNumBytes
            Me.maxNumBytes = maxNumBytes
        End Sub

        Public Sub New(ByVal builder As Builder)
            Me.New(builder.countTotalValue, builder.countNullValue, builder.countZeroLengthValue, builder.minNumBytesValue, builder.maxNumBytesValue)
        End Sub

        Public Overrides Function ToString() As String
            Return "BytesAnalysis()"
        End Function

        Public Overridable ReadOnly Property CountTotal As Long Implements ColumnAnalysis.CountTotal
            Get
                Return countTotalValue
            End Get
        End Property

        Public Overridable ReadOnly Property ColumnType As ColumnTypeEnum Implements ColumnAnalysis.ColumnType
            Get
                Return ColumnTypeEnum.Bytes
            End Get
        End Property

        Public Function getCountTotal() As Long Implements ColumnAnalysis.getCountTotal
            Return CountTotal
        End Function

        Public Function getColumnType() As ColumnTypeEnum Implements ColumnAnalysis.getColumnType
            Return ColumnType
        End Function

        ' ---- Lombok @Data accessors ----

        Public Sub setCountTotal(ByVal countTotal As Long)
            Me.countTotalValue = countTotal
        End Sub

        Public Function getCountNull() As Long
            Return countNull
        End Function

        Public Sub setCountNull(ByVal countNull As Long)
            Me.countNull = countNull
        End Sub

        Public Function getCountZeroLength() As Long
            Return countZeroLength
        End Function

        Public Sub setCountZeroLength(ByVal countZeroLength As Long)
            Me.countZeroLength = countZeroLength
        End Sub

        Public Function getMinNumBytes() As Integer
            Return minNumBytes
        End Function

        Public Sub setMinNumBytes(ByVal minNumBytes As Integer)
            Me.minNumBytes = minNumBytes
        End Sub

        Public Function getMaxNumBytes() As Integer
            Return maxNumBytes
        End Function

        Public Sub setMaxNumBytes(ByVal maxNumBytes As Integer)
            Me.maxNumBytes = maxNumBytes
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As BytesAnalysis = TryCast(obj, BytesAnalysis)
            If other Is Nothing Then Return False
            Return countTotalValue = other.countTotalValue AndAlso countNull = other.countNull AndAlso countZeroLength = other.countZeroLength AndAlso
                   minNumBytes = other.minNumBytes AndAlso maxNumBytes = other.maxNumBytes
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(countTotalValue, countNull, countZeroLength, minNumBytes, maxNumBytes)
        End Function

        Public Class Builder
            Friend countTotalValue As Long
            Friend countNullValue As Long
            Friend countZeroLengthValue As Long
            Friend minNumBytesValue As Integer
            Friend maxNumBytesValue As Integer

            Public Overridable Function countTotal(ByVal value As Long) As Builder
                Me.countTotalValue = value
                Return Me
            End Function

            Public Overridable Function countNull(ByVal value As Long) As Builder
                Me.countNullValue = value
                Return Me
            End Function

            Public Overridable Function countZeroLength(ByVal value As Long) As Builder
                Me.countZeroLengthValue = value
                Return Me
            End Function

            Public Overridable Function minNumBytes(ByVal value As Integer) As Builder
                Me.minNumBytesValue = value
                Return Me
            End Function

            Public Overridable Function maxNumBytes(ByVal value As Integer) As Builder
                Me.maxNumBytesValue = value
                Return Me
            End Function

            Public Overridable Function build() As BytesAnalysis
                Return New BytesAnalysis(Me)
            End Function
        End Class

    End Class

End Namespace
