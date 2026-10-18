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
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports CategoricalMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema

Namespace org.datavec.api.transform.analysis

    ''' <summary>
    ''' The analysis of a data set: the schema and one <see cref="ColumnAnalysisT"/> per column, in column order.
    ''' </summary>
    ' Java: @AllArgsConstructor @Data @JsonTypeInfo(use = JsonTypeInfo.Id.CLASS, include = JsonTypeInfo.As.PROPERTY, property = "@class")
    <JsonClassType>
    <Serializable>
    Public Class DataAnalysis
        Private Const COL_NAME As String = "columnName"
        Private Const COL_IDX As String = "columnIndex"
        Private Const COL_TYPE As String = "columnType"
        Private Const CATEGORICAL_STATE_NAMES As String = "stateNames"
        Private Const ANALYSIS As String = "analysis"
        Private Const DATA_ANALYSIS As String = "DataAnalysis"

        <JsonPropertyName("schema")>
        Private schemaValue As SchemaT
        <JsonPropertyName("columnAnalysis")>
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

        ''' <summary>
        ''' Convert this analysis to JSON, in the format of Java DataVec (see <see cref="JsonMappers"/>). The t-digests
        ''' of the numerical columns are written in the System.Text.Json form of the t-digest stand-in (Java wrote a Java
        ''' serialization of its MergingDigest in base64). YAML is not supported: the Java <c>toYaml</c>/<c>fromYaml</c>
        ''' methods are not ported.
        ''' </summary>
        Public Overridable Function toJson() As String
            Return JsonMappers.writeValueAsString(Me)
        End Function

        ''' <summary>
        ''' Deserialize a JSON DataAnalysis String that was previously serialized with <see cref="toJson"/> or by Java
        ''' DataVec, including the legacy format (<c>{"DataAnalysis" : [ {"columnName" : ..., "analysis" : ...} ]}</c>).
        ''' A t-digest written by Java (Java object serialization) cannot be read: it is dropped with a warning.
        ''' </summary>
        ''' <exception cref="JsonException">if the JSON is not a valid data analysis</exception>
        Public Shared Function fromJson(ByVal json As String) As DataAnalysis
            Dim root As JsonObject = TryCast(JsonNode.Parse(json), JsonObject)
            If root IsNot Nothing AndAlso root.ContainsKey(DATA_ANALYSIS) AndAlso Not root.ContainsKey("@class") Then
                'Legacy format
                Return fromLegacyJson(root)
            End If
            Return JsonMappers.readValue(Of DataAnalysis)(json)
        End Function

        ''' <summary>Reads the legacy format (Java <c>fromMapper</c>).</summary>
        Private Shared Function fromLegacyJson(ByVal node As JsonObject) As DataAnalysis
            Dim meta As IList(Of ColumnMetaDataT) = New List(Of ColumnMetaDataT)()
            Dim analysisList As IList(Of ColumnAnalysisT) = New List(Of ColumnAnalysisT)()
            Dim arrayNode As JsonArray = TryCast(node(DATA_ANALYSIS), JsonArray)
            If arrayNode Is Nothing Then Throw New JsonException("Invalid legacy DataAnalysis JSON: """ & DATA_ANALYSIS & """ is not an array")
            Dim options As JsonSerializerOptions = JsonMappers.Mapper
            For i As Integer = 0 To arrayNode.Count - 1
                Dim analysisNode As JsonObject = DirectCast(arrayNode(i), JsonObject)
                Dim name As String = analysisNode(COL_NAME).GetValue(Of String)()
                Dim type As ColumnTypeEnum = ColumnTypeValueOf(analysisNode(COL_TYPE).GetValue(Of String)())

                Dim daNode As JsonNode = analysisNode(ANALYSIS)
                If daNode IsNot Nothing Then daNode = JsonNode.Parse(daNode.ToJsonString())
                Dim dataAnalysis As ColumnAnalysisT = DirectCast(JsonMappers.fromNode(daNode, GetType(ColumnAnalysisT), options), ColumnAnalysisT)

                If type = ColumnTypeEnum.Categorical Then
                    Dim an As JsonArray = DirectCast(analysisNode(CATEGORICAL_STATE_NAMES), JsonArray)
                    Dim stateNames As IList(Of String) = New List(Of String)(an.Count)
                    For Each n As JsonNode In an
                        stateNames.Add(n.GetValue(Of String)())
                    Next n
                    meta.Add(New CategoricalMetaDataT(name, stateNames))
                Else
                    meta.Add(type.newColumnMetaData(name))
                End If

                analysisList.Add(dataAnalysis)
            Next i

            Dim schema As New SchemaT(meta)
            Return New DataAnalysis(schema, analysisList)
        End Function

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
