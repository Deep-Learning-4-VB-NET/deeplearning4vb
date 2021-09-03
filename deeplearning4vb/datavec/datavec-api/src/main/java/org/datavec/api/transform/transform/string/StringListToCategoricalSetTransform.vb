Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseTransform = org.datavec.api.transform.transform.BaseTransform
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.transform.string


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "map", "columnIdx"}) @EqualsAndHashCode(callSuper = false, exclude = {"columnIdx"}) @Data public class StringListToCategoricalSetTransform extends org.datavec.api.transform.transform.BaseTransform
	<Serializable>
	Public Class StringListToCategoricalSetTransform
		Inherits BaseTransform

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly columnName_Conflict As String
		Private ReadOnly newColumnNames As IList(Of String)
		Private ReadOnly categoryTokens As IList(Of String)
		Private ReadOnly delimiter As String

'JAVA TO VB CONVERTER NOTE: The field map was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly map_Conflict As IDictionary(Of String, Integer)

		Private columnIdx As Integer = -1

		''' <param name="columnName">     The name of the column to convert </param>
		''' <param name="newColumnNames"> The names of the new columns to create </param>
		''' <param name="categoryTokens"> The possible tokens that may be present. Note this list must have the same length and order
		'''                       as the newColumnNames list </param>
		''' <param name="delimiter">      The delimiter for the Strings to convert </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringListToCategoricalSetTransform(@JsonProperty("columnName") String columnName, @JsonProperty("newColumnNames") List<String> newColumnNames, @JsonProperty("categoryTokens") List<String> categoryTokens, @JsonProperty("delimiter") String delimiter)
		Public Sub New(ByVal columnName As String, ByVal newColumnNames As IList(Of String), ByVal categoryTokens As IList(Of String), ByVal delimiter As String)
			If newColumnNames.Count <> categoryTokens.Count Then
				Throw New System.ArgumentException("Names/tokens sizes cannot differ")
			End If
			Me.columnName_Conflict = columnName
			Me.newColumnNames = newColumnNames
			Me.categoryTokens = categoryTokens
			Me.delimiter = delimiter

			map_Conflict = New Dictionary(Of String, Integer)()
			For i As Integer = 0 To categoryTokens.Count - 1
				map(categoryTokens(i)) = i
			Next i
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema

			Dim colIdx As Integer = inputSchema.getIndexOfColumn(columnName_Conflict)

			Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count + newColumnNames.Count - 1)
			Dim oldNames As IList(Of String) = inputSchema.getColumnNames()

			Dim typesIter As IEnumerator(Of ColumnMetaData) = oldMeta.GetEnumerator()
			Dim namesIter As IEnumerator(Of String) = oldNames.GetEnumerator()

			Dim i As Integer = 0
			Do While typesIter.MoveNext()
				Dim t As ColumnMetaData = typesIter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim name As String = namesIter.next()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == colIdx)
				If i = colIdx Then
						i += 1
					'Replace String column with a set of binary/categorical columns
					If t.ColumnType <> ColumnType.String Then
						Throw New System.InvalidOperationException("Cannot convert non-string type")
					End If

					For j As Integer = 0 To newColumnNames.Count - 1
						Dim meta As ColumnMetaData = New CategoricalMetaData(newColumnNames(j), "true", "false")
						newMeta.Add(meta)
					Next j
				Else
						i += 1
					newMeta.Add(t)
				End If
			Loop

			Return inputSchema.newSchema(newMeta)

		End Function

		Public Overrides WriteOnly Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
				Me.columnIdx = inputSchema.getIndexOfColumn(columnName_Conflict)
			End Set
		End Property

		Public Overrides Function ToString() As String
			Return "StringListToCategoricalSetTransform(columnName=" & columnName_Conflict & ",newColumnNames=" & newColumnNames & ",categoryTokens=" & categoryTokens & ",delimiter=""" & delimiter & """)"
		End Function

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & ToString())
			End If
			Dim n As Integer = writables.Count
			Dim [out] As IList(Of Writable) = New List(Of Writable)(n)

			Dim i As Integer = 0
			For Each w As Writable In writables
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == columnIdx)
				If i = columnIdx Then
						i += 1
					Dim str As String = w.ToString()
					Dim present(categoryTokens.Count - 1) As Boolean
					If str IsNot Nothing AndAlso str.Length > 0 Then
						Dim split() As String = str.Split(delimiter, True)
						For Each s As String In split
							Dim idx As Integer? = map(s)
							If idx Is Nothing Then
								Throw New System.InvalidOperationException("Encountered unknown String: """ & s & """")
							End If
							present(idx) = True
						Next s
					End If
					For j As Integer = 0 To present.Length - 1
						[out].Add(New Text(If(present(j), "true", "false")))
					Next j
				Else
						i += 1
					'No change to this column
					[out].Add(w)
				End If
			Next w

			Return [out]
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Return Nothing
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Return Nothing
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overrides Function outputColumnName() As String
			Throw New System.NotSupportedException("New column names is always more than 1 in length")
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overrides Function outputColumnNames() As String()
			Return CType(newColumnNames, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnNames() As String()
			Return New String() {columnName()}
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnName() As String
			Return columnName_Conflict
		End Function
	End Class

End Namespace