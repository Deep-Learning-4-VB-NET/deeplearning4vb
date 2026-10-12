Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports LongMetaData = org.datavec.api.transform.metadata.LongMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports WritableComparator = org.datavec.api.writable.comparator.WritableComparator
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.datavec.api.transform.rank


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"inputSchema"}) @JsonIgnoreProperties({"inputSchema"}) @JsonInclude(JsonInclude.Include.NON_NULL) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public class CalculateSortedRank implements java.io.Serializable, org.datavec.api.transform.ColumnOp
	<Serializable>
	Public Class CalculateSortedRank
		Implements ColumnOp

		Private ReadOnly newColumnName As String
		Private ReadOnly sortOnColumn As String
		Private ReadOnly comparator As WritableComparator
		Private ReadOnly ascending As Boolean
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema


		''' 
		''' <param name="newColumnName">    Name of the new column (will contain the rank for each example) </param>
		''' <param name="sortOnColumn">     Name of the column to sort on </param>
		''' <param name="comparator">       Comparator used to sort examples </param>
		Public Sub New(ByVal newColumnName As String, ByVal sortOnColumn As String, ByVal comparator As WritableComparator)
			Me.New(newColumnName, sortOnColumn, comparator, True)
		End Sub

		''' 
		''' <param name="newColumnName">    Name of the new column (will contain the rank for each example) </param>
		''' <param name="sortOnColumn">     Name of the column to sort on </param>
		''' <param name="comparator">       Comparator used to sort examples </param>
		''' <param name="ascending">        Whether examples should be ascending or descending, using the comparator </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CalculateSortedRank(@JsonProperty("newColumnName") String newColumnName, @JsonProperty("sortOnColumn") String sortOnColumn, @JsonProperty("comparator") org.datavec.api.writable.comparator.WritableComparator comparator, @JsonProperty("ascending") boolean ascending)
		Public Sub New(ByVal newColumnName As String, ByVal sortOnColumn As String, ByVal comparator As WritableComparator, ByVal ascending As Boolean)
			Me.newColumnName = newColumnName
			Me.sortOnColumn = sortOnColumn
			Me.comparator = comparator
			Me.ascending = ascending
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			If TypeOf inputSchema Is SequenceSchema Then
				Throw New System.InvalidOperationException("Calculating sorted rank on sequences: not yet supported")
			End If

			Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(origMeta)

			newMeta.Add(New LongMetaData(newColumnName, 0L, Nothing))

			Return inputSchema.newSchema(newMeta)
		End Function

		Public Overridable Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal schema As Schema)
				Me.inputSchema_Conflict = schema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
			Return newColumnName
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
			Dim columnNames As IList(Of String) = inputSchema_Conflict.getColumnNames()
			columnNames.Add(newColumnName)
			Return CType(columnNames, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
			Return CType(inputSchema_Conflict.getColumnNames(), List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String Implements ColumnOp.columnName
			Return columnNames()(0)
		End Function

		Public Overrides Function ToString() As String
			Return "CalculateSortedRank(newColumnName=""" & newColumnName & """, comparator=" & comparator & ")"
		End Function
	End Class

End Namespace