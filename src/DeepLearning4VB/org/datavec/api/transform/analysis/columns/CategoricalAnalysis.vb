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
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>
    ''' Analysis of a categorical column: the number of occurrences of each category. <see cref="ToString"/> lists the
    ''' categories from the most to the least frequent (a stable sort, so ties keep the map's iteration order).
    ''' </summary>
    ' Java: @AllArgsConstructor @Data @NoArgsConstructor
    <Serializable>
    Public Class CategoricalAnalysis
        Implements ColumnAnalysis

        Private mapOfCounts As IDictionary(Of String, Long)

        Public Sub New()
        End Sub

        Public Sub New(ByVal mapOfCounts As IDictionary(Of String, Long))
            Me.mapOfCounts = mapOfCounts
        End Sub

        Public Overrides Function ToString() As String
            'Returning the counts from highest to lowest here, which seems like a useful default
            Dim keys As IList(Of String) = mapOfCounts.Keys.OrderByDescending(Function(k) mapOfCounts(k)).ToList()

            Dim sb As New StringBuilder()
            sb.Append("CategoricalAnalysis(CategoryCounts={")
            Dim first As Boolean = True
            For Each s As String In keys
                If Not first Then
                    sb.Append(", ")
                End If
                first = False

                sb.Append(s).Append("=").Append(mapOfCounts(s).ToString(System.Globalization.CultureInfo.InvariantCulture))
            Next s
            sb.Append("})")

            Return sb.ToString()
        End Function

        ''' <summary>The sum of the category counts.</summary>
        Public Overridable ReadOnly Property CountTotal As Long Implements ColumnAnalysis.CountTotal
            Get
                Dim sum As Long = 0
                For Each l As Long In mapOfCounts.Values
                    sum += l
                Next l
                Return sum
            End Get
        End Property

        Public Overridable ReadOnly Property ColumnType As ColumnTypeEnum Implements ColumnAnalysis.ColumnType
            Get
                Return ColumnTypeEnum.Categorical
            End Get
        End Property

        Public Function getCountTotal() As Long Implements ColumnAnalysis.getCountTotal
            Return CountTotal
        End Function

        Public Function getColumnType() As ColumnTypeEnum Implements ColumnAnalysis.getColumnType
            Return ColumnType
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getMapOfCounts() As IDictionary(Of String, Long)
            Return mapOfCounts
        End Function

        Public Sub setMapOfCounts(ByVal mapOfCounts As IDictionary(Of String, Long))
            Me.mapOfCounts = mapOfCounts
        End Sub

        ''' <summary>Equal when the other analysis has the same categories with the same counts (Java <c>Map.equals</c>).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As CategoricalAnalysis = TryCast(obj, CategoricalAnalysis)
            If other Is Nothing Then Return False
            If mapOfCounts Is Nothing OrElse other.mapOfCounts Is Nothing Then Return mapOfCounts Is other.mapOfCounts
            If mapOfCounts.Count <> other.mapOfCounts.Count Then Return False
            For Each kv As KeyValuePair(Of String, Long) In mapOfCounts
                Dim v As Long
                If Not other.mapOfCounts.TryGetValue(kv.Key, v) OrElse v <> kv.Value Then Return False
            Next
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            If mapOfCounts IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, Long) In mapOfCounts
                    h += If(kv.Key Is Nothing, 0, kv.Key.GetHashCode()) Xor kv.Value.GetHashCode()
                Next
            End If
            Return h
        End Function
    End Class

End Namespace
