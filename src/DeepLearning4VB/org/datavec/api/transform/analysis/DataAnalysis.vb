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
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema

Namespace org.datavec.api.transform.analysis

    ''' <summary>
    ''' The analysis of a data set: the schema and one <see cref="ColumnAnalysisT"/> per column, in column order.
    ''' </summary>
    ' Java: @AllArgsConstructor @Data @JsonTypeInfo(use = JsonTypeInfo.Id.CLASS, include = JsonTypeInfo.As.PROPERTY, property = "@class")
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    <Serializable>
    Public Class DataAnalysis
        Private Const COL_NAME As String = "columnName"
        Private Const COL_IDX As String = "columnIndex"
        Private Const COL_TYPE As String = "columnType"
        Private Const CATEGORICAL_STATE_NAMES As String = "stateNames"
        Private Const ANALYSIS As String = "analysis"
        Private Const DATA_ANALYSIS As String = "DataAnalysis"

        Private schemaValue As SchemaT
        Private columnAnalysisList As IList(Of ColumnAnalysisT)

        Protected Friend Sub New()
            'No arg for JSON
        End Sub

        Public Sub New(ByVal schema As SchemaT, ByVal columnAnalysis As IList(Of ColumnAnalysisT))
            Me.schemaValue = schema
            Me.columnAnalysisList = columnAnalysis
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            Dim nCol As Integer = schemaValue.numColumns()

            Dim maxNameLength As Integer = 0
            For Each s As String In schemaValue.getColumnNames()
                maxNameLength = Math.Max(maxNameLength, s.Length)
            Next s

            'Header:
            sb.Append(JavaString.format("%-6s", "idx")).Append(JavaString.format("%-" & (maxNameLength + 8) & "s", "name")).Append(JavaString.format("%-15s", "type")).Append("analysis").Append(ControlChars.Lf)

            For i As Integer = 0 To nCol - 1
                Dim colName As String = schemaValue.getName(i)
                Dim type As ColumnTypeEnum = schemaValue.getType(i)
                Dim columnAnalysis As ColumnAnalysisT = columnAnalysisList(i)
                Dim paddedName As String = JavaString.format("%-" & (maxNameLength + 8) & "s", """" & colName & """")
                sb.Append(JavaString.format("%-6d", i)).Append(paddedName).Append(JavaString.format("%-15s", type.ToString())).Append(Objects.toString(columnAnalysis)).Append(ControlChars.Lf)
            Next i

            Return sb.ToString()
        End Function

        ''' <summary>The analysis of the named column.</summary>
        Public Overridable Function getColumnAnalysis(ByVal column As String) As ColumnAnalysisT
            Return columnAnalysisList(schemaValue.getIndexOfColumn(column))
        End Function

        ' TODO(serde): toJson/fromJson come with the serde wave
        ' (toJson, toYaml, fromJson, fromYaml and the legacy-format reader fromMapper use the Jackson-based serde
        ' package, which is ported in wave D).

        ' ---- Lombok @Data accessors ----

        Public Function getSchema() As SchemaT
            Return schemaValue
        End Function

        Public Sub setSchema(ByVal schema As SchemaT)
            schemaValue = schema
        End Sub

        ''' <summary>The analyses of all columns, in column order.</summary>
        Public Function getColumnAnalysis() As IList(Of ColumnAnalysisT)
            Return columnAnalysisList
        End Function

        Public Sub setColumnAnalysis(ByVal columnAnalysis As IList(Of ColumnAnalysisT))
            columnAnalysisList = columnAnalysis
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As DataAnalysis = TryCast(obj, DataAnalysis)
            If other Is Nothing OrElse obj.GetType() IsNot CObj(Me).GetType() Then Return False
            If Not Object.Equals(schemaValue, other.schemaValue) Then Return False
            If columnAnalysisList Is Nothing OrElse other.columnAnalysisList Is Nothing Then Return columnAnalysisList Is other.columnAnalysisList
            Return columnAnalysisList.SequenceEqual(other.columnAnalysisList)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = If(schemaValue Is Nothing, 0, schemaValue.GetHashCode())
            If columnAnalysisList IsNot Nothing Then
                For Each a As ColumnAnalysisT In columnAnalysisList
                    h = h * 31 + If(a Is Nothing, 0, a.GetHashCode())
                Next
            End If
            Return h
        End Function

    End Class

End Namespace
