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
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.comparator

    ''' <summary>
    ''' Sequence comparator: compares the time steps on the numerical value of one column, ascending or descending.
    ''' Supported column types: Integer, Long, Double and Time (as in Java, Float columns are rejected). Doubles are
    ''' compared like Java's <c>Double.compare</c> (NaN is greater than everything, -0.0 is less than 0.0).
    ''' Equality (Lombok <c>@EqualsAndHashCode(callSuper = true, exclude = {"columnType"})</c>) compares the column
    ''' name and the sort order.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"columnType", "schema", "columnIdx"})
    <Serializable>
    Public Class NumericalColumnComparator
        Inherits BaseColumnComparator

        Private columnTypeField As ColumnType
        Private ascending As Boolean

        Public Sub New(ByVal columnName As String)
            Me.New(columnName, True)
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal ascending As Boolean)
            MyBase.New(columnName)
            Me.ascending = ascending
        End Sub

        ''' <summary>Java <c>getColumnType()</c> (Lombok): the type of the column, known once the schema is set.</summary>
        Public Function getColumnType() As ColumnType
            Return columnTypeField
        End Function

        ''' <summary>Java <c>isAscending()</c> (Lombok).</summary>
        Public Function isAscending() As Boolean
            Return ascending
        End Function

        ''' <summary>Java <c>setAscending()</c> (Lombok).</summary>
        Public Sub setAscending(ByVal ascending As Boolean)
            Me.ascending = ascending
        End Sub

        ''' <summary>Sets the sequence schema; the column must be an Integer, Long, Double or Time column.</summary>
        ''' <exception cref="InvalidOperationException">for any other column type</exception>
        Public Overrides WriteOnly Property Schema As SchemaT
            Set(ByVal value As SchemaT)
                MyBase.Schema = value
                Me.columnTypeField = value.getType(Me.columnIdx)
                Select Case columnTypeField
                    Case ColumnType.Integer, ColumnType.Long, ColumnType.Double, ColumnType.Time
                        'All ok. Time column uses LongWritables too...
                    Case Else
                        Throw New InvalidOperationException("Cannot apply numerical column comparator on column of type " & columnTypeField.ToString())
                End Select
            End Set
        End Property

        Protected Friend Overrides Function compare(ByVal w1 As WritableT, ByVal w2 As WritableT) As Integer
            Dim result As Integer
            Select Case columnTypeField
                Case ColumnType.Integer
                    result = w1.toInt().CompareTo(w2.toInt())
                Case ColumnType.Time, ColumnType.Long
                    result = w1.toLong().CompareTo(w2.toLong())
                Case ColumnType.Double
                    result = javaDoubleCompare(w1.toDouble(), w2.toDouble())
                Case Else
                    'Should never happen...
                    Throw New Exception("Cannot apply numerical column comparator on column of type " & columnTypeField.ToString())
            End Select

            If ascending Then
                Return result
            End If
            Return -result
        End Function

        ''' <summary>Java <c>Double.compare</c>: NaN is greater than everything (and equal to itself), -0.0 is less than 0.0.</summary>
        Private Shared Function javaDoubleCompare(ByVal d1 As Double, ByVal d2 As Double) As Integer
            If d1 < d2 Then Return -1
            If d1 > d2 Then Return 1
            Dim bits1 As Long = If(Double.IsNaN(d1), &H7FF8000000000000L, BitConverter.DoubleToInt64Bits(d1))
            Dim bits2 As Long = If(Double.IsNaN(d2), &H7FF8000000000000L, BitConverter.DoubleToInt64Bits(d2))
            If bits1 = bits2 Then Return 0
            Return If(bits1 < bits2, -1, 1)
        End Function

        Public Overrides Function ToString() As String
            Return "NumericalColumnComparator(columnName=""" & columnNameField & """,ascending=" & If(ascending, "true", "false") & ")"
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Return ascending = DirectCast(obj, NumericalColumnComparator).ascending
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return MyBase.GetHashCode() * 59 + If(ascending, 79, 97)
        End Function

    End Class

End Namespace
