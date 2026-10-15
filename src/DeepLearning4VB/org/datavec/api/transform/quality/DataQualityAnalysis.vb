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
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ColumnQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.ColumnQuality
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema

Namespace org.datavec.api.transform.quality

    ''' <summary>
    ''' A report outlining number of invalid and missing features
    ''' </summary>
    ' Java: @AllArgsConstructor @Data
    <Serializable>
    Public Class DataQualityAnalysis

        Private schemaField As SchemaT
        Private columnQualityList As IList(Of ColumnQuality)

        Public Sub New()
        End Sub

        Public Sub New(ByVal schema As SchemaT, ByVal columnQualityList As IList(Of ColumnQuality))
            Me.schemaField = schema
            Me.columnQualityList = columnQualityList
        End Sub

        ''' <summary>Java <c>getSchema()</c> (Lombok).</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        ''' <summary>Java <c>setSchema()</c> (Lombok).</summary>
        Public Sub setSchema(ByVal schema As SchemaT)
            schemaField = schema
        End Sub

        ''' <summary>Java <c>getColumnQualityList()</c> (Lombok).</summary>
        Public Function getColumnQualityList() As IList(Of ColumnQuality)
            Return columnQualityList
        End Function

        ''' <summary>Java <c>setColumnQualityList()</c> (Lombok).</summary>
        Public Sub setColumnQualityList(ByVal list As IList(Of ColumnQuality))
            columnQualityList = list
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            Dim nCol As Integer = schemaField.numColumns()

            Dim maxNameLength As Integer = 0
            For Each s As String In schemaField.getColumnNames()
                maxNameLength = Math.Max(maxNameLength, s.Length)
            Next s

            'Header:
            sb.Append(JavaString.format("%-6s", "idx")).Append(JavaString.format("%-" & (maxNameLength + 8) & "s", "name")).Append(JavaString.format("%-15s", "type")).Append(JavaString.format("%-10s", "quality")).Append("details").Append(ControlChars.Lf)

            For i As Integer = 0 To nCol - 1
                Dim colName As String = schemaField.getName(i)
                Dim type As ColumnType = schemaField.getType(i)
                Dim columnQuality As ColumnQuality = columnQualityList(i)
                Dim pass As Boolean = columnQuality.getCountInvalid() = 0L AndAlso columnQuality.getCountMissing() = 0L
                Dim paddedName As String = JavaString.format("%-" & (maxNameLength + 8) & "s", """" & colName & """")
                sb.Append(JavaString.format("%-6d", i)).Append(paddedName).Append(JavaString.format("%-15s", type.ToString())).Append(JavaString.format("%-10s", If(pass, "ok", "FAIL"))).Append(columnQuality.ToString()).Append(ControlChars.Lf)
            Next i

            Return sb.ToString()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As DataQualityAnalysis = TryCast(obj, DataQualityAnalysis)
            If other Is Nothing Then Return False
            If Not Object.Equals(schemaField, other.schemaField) Then Return False
            If columnQualityList Is Nothing OrElse other.columnQualityList Is Nothing Then Return columnQualityList Is other.columnQualityList
            Return columnQualityList.SequenceEqual(other.columnQualityList)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = If(schemaField Is Nothing, 0, schemaField.GetHashCode())
            If columnQualityList IsNot Nothing Then
                For Each q As ColumnQuality In columnQualityList
                    h = h * 31 + If(q Is Nothing, 0, q.GetHashCode())
                Next
            End If
            Return h
        End Function

    End Class

End Namespace
