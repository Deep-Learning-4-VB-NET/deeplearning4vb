Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports StringReduceOp = org.datavec.api.transform.StringReduceOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports StringMetaData = org.datavec.api.transform.metadata.StringMetaData
Imports ColumnReduction = org.datavec.api.transform.reduce.ColumnReduction
Imports Schema = org.datavec.api.transform.schema.Schema
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

Namespace org.datavec.api.transform.stringreduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"schema", "keyColumnsSet"}) @EqualsAndHashCode(exclude = {"schema", "keyColumnsSet"}) public class StringReducer implements IStringReducer
	<Serializable>
	Public Class StringReducer
		Implements IStringReducer

		Private schema As Schema
		Private ReadOnly inputColumns As IList(Of String)
		Private ReadOnly inputColumnsSet As ISet(Of String)
		Private outputColumnName As String
		Private ReadOnly stringReduceOp As StringReduceOp
		Private customReductions As IDictionary(Of String, ColumnReduction)

		Private Sub New(ByVal builder As Builder)
			Me.New(builder.inputColumns_Conflict, builder.defaultOp, builder.customReductions, builder.outputColumnName_Conflict)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StringReducer(@JsonProperty("inputColumns") List<String> inputColumns, @JsonProperty("op") org.datavec.api.transform.StringReduceOp stringReduceOp, @JsonProperty("customReductions") Map<String, org.datavec.api.transform.reduce.ColumnReduction> customReductions, @JsonProperty("outputColumnName") String outputColumnName)
		Public Sub New(ByVal inputColumns As IList(Of String), ByVal stringReduceOp As StringReduceOp, ByVal customReductions As IDictionary(Of String, ColumnReduction), ByVal outputColumnName As String)
			Me.inputColumns = inputColumns
			Me.inputColumnsSet = (If(inputColumns Is Nothing, Nothing, New HashSet(Of )(inputColumns)))
			Me.stringReduceOp = stringReduceOp
			Me.customReductions = customReductions
			Me.outputColumnName = outputColumnName
		End Sub

		Public Overridable Property InputSchema Implements IStringReducer.setInputSchema As Schema
			Set(ByVal schema As Schema)
				Me.schema = schema
			End Set
			Get
				Return schema
			End Get
		End Property


		Public Overridable ReadOnly Property InputColumns As IList(Of String)
			Get
				Return inputColumns
			End Get
		End Property

		''' <summary>
		''' Get the output schema, given the input schema
		''' </summary>
		Public Overridable Function transform(ByVal schema As Schema) As Schema Implements IStringReducer.transform
			Dim nCols As Integer = schema.numColumns()
			Dim meta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(nCols)
			CType(newMeta, List(Of ColumnMetaData)).AddRange(meta)
			newMeta.Add(New StringMetaData(outputColumnName))
			Return schema.newSchema(newMeta)
		End Function

		Private Shared Function getMetaForColumn(ByVal op As StringReduceOp, ByVal name As String, ByVal inMeta As ColumnMetaData) As ColumnMetaData
			inMeta = inMeta.clone()
			Select Case op
				Case StringReduceOp.PREPEND
					inMeta.Name = "prepend(" & name & ")"
					Return inMeta
				Case StringReduceOp.APPEND
					inMeta.Name = "append(" & name & ")"
					Return inMeta
				Case StringReduceOp.REPLACE
					inMeta.Name = "replace(" & name & ")"
					Return inMeta
				Case StringReduceOp.MERGE
					inMeta.Name = "merge(" & name & ")"
					Return inMeta
				Case Else
					Throw New System.NotSupportedException("Unknown or not implemented op: " & op)
			End Select
		End Function

		Public Overridable Function reduce(ByVal examplesList As IList(Of IList(Of Writable))) As IList(Of Writable)
			'Go through each writable, and reduce according to whatever strategy is specified

			If schema Is Nothing Then
				Throw New System.InvalidOperationException("Error: Schema has not been set")
			End If


			Dim [out] As IList(Of Writable) = New List(Of Writable)(examplesList.Count)
			For i As Integer = 0 To examplesList.Count - 1
				[out].Add(reduceStringOrCategoricalColumn(stringReduceOp, examplesList(i)))
			Next i

			Return [out]
		End Function



		Public Shared Function reduceStringOrCategoricalColumn(ByVal op As StringReduceOp, ByVal values As IList(Of Writable)) As Writable
			Select Case op
				Case StringReduceOp.MERGE, APPEND
					Dim stringBuilder As New StringBuilder()
					For Each w As Writable In values
						stringBuilder.Append(w.ToString())
					Next w
					Return New Text(stringBuilder.ToString())
				Case StringReduceOp.REPLACE
					If values.Count > 2 Then
						Throw New System.ArgumentException("Unable to run replace on columns > 2")
					End If
					Return New Text(values(1).ToString())
				Case StringReduceOp.PREPEND
					Dim reverse As IList(Of Writable) = New List(Of Writable)(values)
					reverse.Reverse()
					Dim stringBuilder2 As New StringBuilder()
					For Each w As Writable In reverse
						stringBuilder2.Append(w.ToString())
					Next w

					Return New Text(stringBuilder2.ToString())
				Case Else
					Throw New System.NotSupportedException("Cannot execute op """ & op & """ on String/Categorical column " & "(can only perform Count, CountUnique, TakeFirst and TakeLast ops on categorical columns)")
			End Select
		End Function



		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder("StringReducer(")

			sb.Append("defaultOp=").Append(stringReduceOp)

			If customReductions IsNot Nothing Then
				sb.Append(",customReductions=").Append(customReductions)
			End If


			sb.Append(")")
			Return sb.ToString()
		End Function


		Public Class Builder

			Friend defaultOp As StringReduceOp
			Friend opMap As IDictionary(Of String, StringReduceOp) = New Dictionary(Of String, StringReduceOp)()
			Friend customReductions As IDictionary(Of String, ColumnReduction) = New Dictionary(Of String, ColumnReduction)()
			Friend ignoreInvalidInColumns As ISet(Of String) = New HashSet(Of String)()
'JAVA TO VB CONVERTER NOTE: The field outputColumnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend outputColumnName_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field inputColumns was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inputColumns_Conflict As IList(Of String)



'JAVA TO VB CONVERTER NOTE: The parameter inputColumns was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function inputColumns(ByVal inputColumns_Conflict As IList(Of String)) As Builder
				Me.inputColumns_Conflict = inputColumns_Conflict
				Return Me
			End Function

			''' <summary>
			''' Create a StringReducer builder, and set the default column reduction operation.
			''' For any columns that aren't specified explicitly, they will use the default reduction operation.
			''' If a column does have a reduction operation explicitly specified, then it will override
			''' the default specified here.
			''' </summary>
			''' <param name="defaultOp"> Default reduction operation to perform </param>
			Public Sub New(ByVal defaultOp As StringReduceOp)
				Me.defaultOp = defaultOp
			End Sub

'JAVA TO VB CONVERTER NOTE: The parameter outputColumnName was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function outputColumnName(ByVal outputColumnName_Conflict As String) As Builder
				Me.outputColumnName_Conflict = outputColumnName_Conflict
				Return Me
			End Function


			Friend Overridable Function add(ByVal op As StringReduceOp, ByVal cols() As String) As Builder
				For Each s As String In cols
					opMap(s) = op
				Next s
				Return Me
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the minimum value
			''' </summary>
			Public Overridable Function appendColumns(ParamArray ByVal columns() As String) As Builder
				Return add(StringReduceOp.APPEND, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the maximum value
			''' </summary>
			Public Overridable Function prependColumns(ParamArray ByVal columns() As String) As Builder
				Return add(StringReduceOp.PREPEND, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the sum of values
			''' </summary>
			Public Overridable Function mergeColumns(ParamArray ByVal columns() As String) As Builder
				Return add(StringReduceOp.MERGE, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the mean of the values
			''' </summary>
			Public Overridable Function replaceColumn(ParamArray ByVal columns() As String) As Builder
				Return add(StringReduceOp.REPLACE, columns)
			End Function

			''' <summary>
			''' Reduce the specified column using a custom column reduction functionality.
			''' </summary>
			''' <param name="column">          Column to execute the custom reduction functionality on </param>
			''' <param name="columnReduction"> Column reduction to execute on that column </param>
			Public Overridable Function customReduction(ByVal column As String, ByVal columnReduction As ColumnReduction) As Builder
				customReductions(column) = columnReduction
				Return Me
			End Function

			''' <summary>
			''' When doing the reduction: set the specified columns to ignore any invalid values.
			''' Invalid: defined as being not valid according to the ColumnMetaData: <seealso cref="ColumnMetaData.isValid(Writable)"/>.
			''' For numerical columns, this typically means being unable to parse the Writable. For example, Writable.toLong() failing for a Long column.
			''' If the column has any restrictions (min/max values, regex for Strings etc) these will also be taken into account.
			''' </summary>
			''' <param name="columns"> Columns to set 'ignore invalid' for </param>
			Public Overridable Function setIgnoreInvalid(ParamArray ByVal columns() As String) As Builder
				Collections.addAll(ignoreInvalidInColumns, columns)
				Return Me
			End Function

			Public Overridable Function build() As StringReducer
				Return New StringReducer(Me)
			End Function
		End Class


	End Class

End Namespace