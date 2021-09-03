Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ColumnAnalysis = org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports JsonMappers = org.datavec.api.transform.serde.JsonMappers
Imports JsonSerializer = org.datavec.api.transform.serde.JsonSerializer
Imports YamlSerializer = org.datavec.api.transform.serde.YamlSerializer
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports JsonNode = org.nd4j.shade.jackson.databind.JsonNode
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports InvalidTypeIdException = org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException
Imports ArrayNode = org.nd4j.shade.jackson.databind.node.ArrayNode

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

Namespace org.datavec.api.transform.analysis


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public class DataAnalysis implements java.io.Serializable
	<Serializable>
	Public Class DataAnalysis
		Private Const COL_NAME As String = "columnName"
		Private Const COL_IDX As String = "columnIndex"
		Private Const COL_TYPE As String = "columnType"
		Private Const CATEGORICAL_STATE_NAMES As String = "stateNames"
		Private Const ANALYSIS As String = "analysis"
		Private Const DATA_ANALYSIS As String = "DataAnalysis"

		Private schema As Schema
		Private columnAnalysis As IList(Of ColumnAnalysis)

		Protected Friend Sub New()
			'No arg for JSON
		End Sub

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			Dim nCol As Integer = schema.numColumns()

			Dim maxNameLength As Integer = 0
			For Each s As String In schema.getColumnNames()
				maxNameLength = Math.Max(maxNameLength, s.Length)
			Next s

			'Header:
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			sb.Append(String.Format("{0,-6}", "idx")).Append(String.Format("%-" & (maxNameLength + 8) & "s", "name")).Append(String.Format("{0,-15}", "type")).Append("analysis").Append(vbLf)

			For i As Integer = 0 To nCol - 1
				Dim colName As String = schema.getName(i)
				Dim type As ColumnType = schema.getType(i)
'JAVA TO VB CONVERTER NOTE: The variable analysis was renamed since Visual Basic does not handle local variables named the same as class members well:
				Dim analysis_Conflict As ColumnAnalysis = columnAnalysis(i)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Dim paddedName As String = String.Format("%-" & (maxNameLength + 8) & "s", """" & colName & """")
				sb.Append(String.Format("{0,-6:D}", i)).Append(paddedName).Append(String.Format("{0,-15}", type)).Append(analysis_Conflict).Append(vbLf)
			Next i

			Return sb.ToString()
		End Function

		Public Overridable Function getColumnAnalysis(ByVal column As String) As ColumnAnalysis
			Return columnAnalysis(schema.getIndexOfColumn(column))
		End Function

		''' <summary>
		''' Convert the DataAnalysis object to JSON format
		''' </summary>
		Public Overridable Function toJson() As String
			Try
				Return (New JsonSerializer()).ObjectMapper.writeValueAsString(Me)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Convert the DataAnalysis object to YAML format
		''' </summary>
		Public Overridable Function toYaml() As String
			Try
				Return (New YamlSerializer()).ObjectMapper.writeValueAsString(Me)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Deserialize a JSON DataAnalysis String that was previously serialized with <seealso cref="toJson()"/>
		''' </summary>
		Public Shared Function fromJson(ByVal json As String) As DataAnalysis
			Try
				Return (New JsonSerializer()).ObjectMapper.readValue(json, GetType(DataAnalysis))
			Catch e As InvalidTypeIdException
				If e.Message.contains("@class") Then
					Try
						'JSON may be legacy (1.0.0-alpha or earlier), attempt to load it using old format
						Return JsonMappers.LegacyMapper.readValue(json, GetType(DataAnalysis))
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try
				End If
				Throw New Exception(e)
			Catch e As Exception
				'Legacy format
				Dim om As ObjectMapper = (New JsonSerializer()).ObjectMapper
				Return fromMapper(om, json)
			End Try
		End Function

		''' <summary>
		''' Deserialize a YAML DataAnalysis String that was previously serialized with <seealso cref="toYaml()"/>
		''' </summary>
		Public Shared Function fromYaml(ByVal yaml As String) As DataAnalysis
			Try
				Return (New YamlSerializer()).ObjectMapper.readValue(yaml, GetType(DataAnalysis))
			Catch e As Exception
				'Legacy format
				Dim om As ObjectMapper = (New YamlSerializer()).ObjectMapper
				Return fromMapper(om, yaml)
			End Try
		End Function

		Private Shared Function fromMapper(ByVal om As ObjectMapper, ByVal json As String) As DataAnalysis

			Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
'JAVA TO VB CONVERTER NOTE: The variable analysis was renamed since Visual Basic does not handle local variables named the same as class members well:
			Dim analysis_Conflict As IList(Of ColumnAnalysis) = New List(Of ColumnAnalysis)()
			Try
				Dim node As JsonNode = om.readTree(json)
				Dim fieldNames As IEnumerator(Of String) = node.fieldNames()
				Dim hasDataAnalysis As Boolean = False
				Do While fieldNames.MoveNext()
					If "DataAnalysis".Equals(fieldNames.Current) Then
						hasDataAnalysis = True
						Exit Do
					End If
				Loop
				If Not hasDataAnalysis Then
					Throw New Exception()
				End If

				Dim arrayNode As ArrayNode = CType(node.get("DataAnalysis"), ArrayNode)
				For i As Integer = 0 To arrayNode.size() - 1
					Dim analysisNode As JsonNode = arrayNode.get(i)
					Dim name As String = analysisNode.get(COL_NAME).asText()
					Dim idx As Integer = analysisNode.get(COL_IDX).asInt()
					Dim type As ColumnType = ColumnType.valueOf(analysisNode.get(COL_TYPE).asText())

					Dim daNode As JsonNode = analysisNode.get(ANALYSIS)
'JAVA TO VB CONVERTER NOTE: The variable dataAnalysis was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
					Dim dataAnalysis_Conflict As ColumnAnalysis = om.treeToValue(daNode, GetType(ColumnAnalysis))

					If type = ColumnType.Categorical Then
						Dim an As ArrayNode = CType(analysisNode.get(CATEGORICAL_STATE_NAMES), ArrayNode)
						Dim stateNames As IList(Of String) = New List(Of String)(an.size())
						Dim iter As IEnumerator(Of JsonNode) = an.elements()
						Do While iter.MoveNext()
							stateNames.Add(iter.Current.asText())
						Loop
						meta.Add(New CategoricalMetaData(name, stateNames))
					Else
						meta.Add(type.newColumnMetaData(name))
					End If

					analysis_Conflict.Add(dataAnalysis_Conflict)
				Next i
			Catch e As Exception
				Throw New Exception(e)
			End Try

			Dim schema As New Schema(meta)
			Return New DataAnalysis(schema, analysis_Conflict)
		End Function

		<Obsolete>
		Private ReadOnly Property JsonRepresentation As IDictionary(Of String, IList(Of IDictionary(Of String, Object)))
			Get
				Dim jsonRepresentation As IDictionary(Of String, IList(Of IDictionary(Of String, Object))) = New LinkedHashMap(Of String, IList(Of IDictionary(Of String, Object)))()
				Dim list As IList(Of IDictionary(Of String, Object)) = New List(Of IDictionary(Of String, Object))()
				jsonRepresentation("DataAnalysis") = list
    
				For Each colName As String In schema.getColumnNames()
					Dim current As IDictionary(Of String, Object) = New LinkedHashMap(Of String, Object)()
					Dim idx As Integer = schema.getIndexOfColumn(colName)
					current(COL_NAME) = colName
					current(COL_IDX) = idx
					Dim columnType As ColumnType = schema.getMetaData(colName).ColumnType
					current(COL_TYPE) = columnType
					If columnType = ColumnType.Categorical Then
						current(CATEGORICAL_STATE_NAMES) = DirectCast(schema.getMetaData(colName), CategoricalMetaData).getStateNames()
					End If
					current(ANALYSIS) = Collections.singletonMap(columnAnalysis(idx).GetType().Name, columnAnalysis(idx))
    
					list.Add(current)
				Next colName
    
				Return jsonRepresentation
			End Get
		End Property

		Private Function toJson(ByVal jsonRepresentation As IDictionary(Of String, IList(Of IDictionary(Of String, Object)))) As String
			Dim om As ObjectMapper = (New JsonSerializer()).ObjectMapper
			Try
				Return om.writeValueAsString(jsonRepresentation)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Private Function toYaml(ByVal jsonRepresentation As IDictionary(Of String, IList(Of IDictionary(Of String, Object)))) As String
			Dim om As ObjectMapper = (New YamlSerializer()).ObjectMapper
			Try
				Return om.writeValueAsString(jsonRepresentation)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace