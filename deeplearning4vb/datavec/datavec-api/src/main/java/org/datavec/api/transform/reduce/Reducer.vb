Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Preconditions = com.clearspring.analytics.util.Preconditions
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports Condition = org.datavec.api.transform.condition.Condition
Imports TrivialColumnCondition = org.datavec.api.transform.condition.column.TrivialColumnCondition
Imports org.datavec.api.transform.metadata
Imports org.datavec.api.transform.ops
Imports Schema = org.datavec.api.transform.schema.Schema
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

Namespace org.datavec.api.transform.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"schema", "keyColumnsSet"}) @EqualsAndHashCode(exclude = {"schema", "keyColumnsSet"}) public class Reducer implements IAssociativeReducer
	<Serializable>
	Public Class Reducer
		Implements IAssociativeReducer

		Private schema As Schema
		Private ReadOnly keyColumns As IList(Of String)
		Private ReadOnly keyColumnsSet As ISet(Of String)
		Private ReadOnly defaultOp As ReduceOp
		Private ReadOnly opMap As IDictionary(Of String, IList(Of ReduceOp))
		Private conditionalReductions As IDictionary(Of String, ConditionalReduction)
		Private customReductions As IDictionary(Of String, AggregableColumnReduction)

		Private ignoreInvalidInColumns As ISet(Of String)

		Private Sub New(ByVal builder As Builder)
			Me.New((If(builder.keyColumns_Conflict Is Nothing, Nothing, java.util.Arrays.asList(builder.keyColumns_Conflict))), builder.defaultOp, builder.opMap, builder.customReductions, builder.conditionalReductions, builder.ignoreInvalidInColumns)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Reducer(@JsonProperty("keyColumns") List<String> keyColumns, @JsonProperty("defaultOp") org.datavec.api.transform.ReduceOp defaultOp, @JsonProperty("opMap") Map<String, List<org.datavec.api.transform.ReduceOp>> opMap, @JsonProperty("customReductions") Map<String, AggregableColumnReduction> customReductions, @JsonProperty("conditionalReductions") Map<String, ConditionalReduction> conditionalReductions, @JsonProperty("ignoreInvalidInColumns") @Set<String> ignoreInvalidInColumns)
		Public Sub New(ByVal keyColumns As IList(Of String), ByVal defaultOp As ReduceOp, ByVal opMap As IDictionary(Of String, IList(Of ReduceOp)), ByVal customReductions As IDictionary(Of String, AggregableColumnReduction), ByVal conditionalReductions As IDictionary(Of String, ConditionalReduction), ByVal ignoreInvalidInColumns As ISet(Of String))
			Me.keyColumns = keyColumns
			Me.keyColumnsSet = (If(keyColumns Is Nothing, Nothing, New HashSet(Of )(keyColumns)))
			Me.defaultOp = defaultOp
			Me.opMap = opMap
			Me.customReductions = customReductions
			Me.conditionalReductions = conditionalReductions
			Me.ignoreInvalidInColumns = ignoreInvalidInColumns
		End Sub

		Public Overridable Property InputSchema Implements IAssociativeReducer.setInputSchema As Schema
			Set(ByVal schema As Schema)
				Me.schema = schema
				'Conditions (if any) also need the input schema:
				For Each cr As ConditionalReduction In conditionalReductions.Values
					cr.getCondition().setInputSchema(schema)
				Next cr
			End Set
			Get
				Return schema
			End Get
		End Property


		Public Overridable ReadOnly Property KeyColumns As IList(Of String)
			Get
				Return keyColumns
			End Get
		End Property

		''' <summary>
		''' Get the output schema, given the input schema
		''' </summary>
		Public Overridable Function transform(ByVal schema As Schema) As Schema Implements IAssociativeReducer.transform
			Dim nCols As Integer = schema.numColumns()
			Dim colNames As IList(Of String) = schema.getColumnNames()
			Dim meta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(nCols)

			For i As Integer = 0 To nCols - 1
				Dim name As String = colNames(i)
				Dim inMeta As ColumnMetaData = meta(i)

				If keyColumnsSet IsNot Nothing AndAlso keyColumnsSet.Contains(name) Then
					'No change to key columns
					newMeta.Add(inMeta)
					Continue For
				End If

				'First: check for a custom reduction on this column
				If customReductions IsNot Nothing AndAlso customReductions.ContainsKey(name) Then
					Dim reduction As AggregableColumnReduction = customReductions(name)

					Dim outName As IList(Of String) = reduction.getColumnsOutputName(name)
					Dim outMeta As IList(Of ColumnMetaData) = reduction.getColumnOutputMetaData(outName, inMeta)
					CType(newMeta, List(Of ColumnMetaData)).AddRange(outMeta)
					Continue For
				End If

				'Second: check for conditional reductions on this column:
				If conditionalReductions IsNot Nothing AndAlso conditionalReductions.ContainsKey(name) Then
					Dim reduction As ConditionalReduction = conditionalReductions(name)

					Dim outNames As IList(Of String) = reduction.getOutputNames()
					Dim reductions As IList(Of ReduceOp) = reduction.getReductions()
					Dim j As Integer = 0
					Do While j < reduction.getReductions().size()
						Dim red As ReduceOp = reductions(j)
						Dim outName As String = outNames(j)
						Dim m As ColumnMetaData = getMetaForColumn(red, name, inMeta)
						m.Name = outName
						newMeta.Add(m)
						j += 1
					Loop
					Continue For
				End If


				'Otherwise: get the specified (built-in) reduction op
				'If no reduction op is specified for that column: use the default
				Dim lop As IList(Of ReduceOp) = If(opMap.ContainsKey(name), opMap(name), Collections.singletonList(defaultOp))
				If lop IsNot Nothing Then
					For Each op As ReduceOp In lop
						newMeta.Add(getMetaForColumn(op, name, inMeta))
					Next op
				End If
			Next i

			Return schema.newSchema(newMeta)
		End Function

		Private Shared Function getOutNameForColumn(ByVal op As ReduceOp, ByVal name As String) As String
			Return op.ToString().ToLower() & "(" & name & ")"
		End Function

		Private Shared Function getMetaForColumn(ByVal op As ReduceOp, ByVal name As String, ByVal inMeta As ColumnMetaData) As ColumnMetaData
			inMeta = inMeta.clone()
			Select Case op
				' type-preserving operations
				Case ReduceOp.Min, Max, Range, TakeFirst, TakeLast
					inMeta.Name = getOutNameForColumn(op, name)
					Return inMeta
				Case ReduceOp.Prod, Sum
					Dim outName As String = getOutNameForColumn(op, name)
					'Issue with prod/sum: the input meta data restrictions probably won't hold. But the data _type_ should essentially remain the same
					Dim outMeta As ColumnMetaData
					If TypeOf inMeta Is IntegerMetaData Then
						outMeta = New IntegerMetaData(outName)
					ElseIf TypeOf inMeta Is LongMetaData Then
						outMeta = New LongMetaData(outName)
					ElseIf TypeOf inMeta Is FloatMetaData Then
						outMeta = New FloatMetaData(outName)
					ElseIf TypeOf inMeta Is DoubleMetaData Then
						outMeta = New DoubleMetaData(outName)
					Else 'Sum/Prod doesn't really make sense to sum other column types anyway...
						outMeta = inMeta
					End If
					outMeta.Name = outName
					Return outMeta
				Case ReduceOp.Mean, Stdev, Variance, PopulationVariance, UncorrectedStdDev
					Return New DoubleMetaData(getOutNameForColumn(op, name))
				Case ReduceOp.Append, Prepend
					Return New StringMetaData(getOutNameForColumn(op, name))
				Case ReduceOp.Count, CountUnique 'Always Long
					Return New LongMetaData(getOutNameForColumn(op, name), 0L, Nothing)
				Case Else
					Throw New System.NotSupportedException("Unknown or not implemented op: " & op)
			End Select
		End Function

		Public Overridable Function aggregableReducer() As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable))
			'Go through each writable, and reduce according to whatever strategy is specified

			If schema Is Nothing Then
				Throw New System.InvalidOperationException("Error: Schema has not been set")
			End If

			Dim nCols As Integer = schema.numColumns()
			Dim colNames As IList(Of String) = schema.getColumnNames()

			Dim ops As IList(Of IAggregableReduceOp(Of Writable, IList(Of Writable))) = New List(Of IAggregableReduceOp(Of Writable, IList(Of Writable)))(nCols)
			Dim conditionalActive As Boolean = (conditionalReductions IsNot Nothing AndAlso conditionalReductions.Count > 0)
			Dim conditions As IList(Of Condition) = New List(Of Condition)(nCols)

			For i As Integer = 0 To nCols - 1
				Dim colName As String = colNames(i)
				If keyColumnsSet IsNot Nothing AndAlso keyColumnsSet.Contains(colName) Then
					Dim first As IAggregableReduceOp(Of Writable, Writable) = New AggregatorImpls.AggregableFirst(Of Writable, Writable)()
					ops.Add(New AggregableMultiOp(Of Writable, IList(Of Writable))(Collections.singletonList(first)))
					If conditionalActive Then
						conditions.Add(New TrivialColumnCondition(colName))
					End If
					Continue For
				End If


				' is this a *custom* reduction column?
				If customReductions IsNot Nothing AndAlso customReductions.ContainsKey(colName) Then
					Dim reduction As AggregableColumnReduction = customReductions(colName)
					ops.Add(reduction.reduceOp())
					If conditionalActive Then
						conditions.Add(New TrivialColumnCondition(colName))
					End If
					Continue For
				End If

				' are we adding global *conditional* reduction column?
				' Only practical difference with conditional reductions is we filter the input on an all-fields condition first
				If conditionalActive Then
					If conditionalReductions.ContainsKey(colName) Then
						conditions.Add(conditionalReductions(colName).getCondition())
					Else
						conditions.Add(New TrivialColumnCondition(colName))
					End If
				End If

				'What type of column is this?
				Dim type As ColumnType = schema.getType(i)

				'What ops are we performing on this column?
				Dim conditionalOp As Boolean = conditionalActive AndAlso conditionalReductions.ContainsKey(colName)
				Dim lop As IList(Of ReduceOp) = (If(conditionalOp, conditionalReductions(colName).getReductions(), opMap(colName)))
				If lop Is Nothing OrElse lop.Count = 0 Then
					lop = Collections.singletonList(defaultOp)
				End If

				'Execute the reduction, store the result
				ops.Add(AggregableReductionUtils.reduceColumn(lop, type, ignoreInvalidInColumns.Contains(colName), schema.getMetaData(i)))
			Next i

			If conditionalActive Then
				Return New DispatchWithConditionOp(Of IList(Of Writable), IList(Of Writable))(ops, conditions)
			Else
				Return New DispatchOp(Of IList(Of Writable), IList(Of Writable))(ops)
			End If
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder("Reducer(")
			If keyColumns IsNot Nothing Then
				sb.Append("keyColumns=").Append(keyColumns).Append(",")
			End If
			sb.Append("defaultOp=").Append(defaultOp)
			If opMap IsNot Nothing Then
				sb.Append(",opMap=").Append(opMap)
			End If
			If customReductions IsNot Nothing Then
				sb.Append(",customReductions=").Append(customReductions)
			End If
			If conditionalReductions IsNot Nothing Then
				sb.Append(",conditionalReductions=").Append(conditionalReductions)
			End If
			If ignoreInvalidInColumns IsNot Nothing Then
				sb.Append(",ignoreInvalidInColumns=").Append(ignoreInvalidInColumns)
			End If
			sb.Append(")")
			Return sb.ToString()
		End Function


		Public Class Builder

			Friend defaultOp As ReduceOp
			Friend opMap As IDictionary(Of String, IList(Of ReduceOp)) = New Dictionary(Of String, IList(Of ReduceOp))()
			Friend customReductions As IDictionary(Of String, AggregableColumnReduction) = New Dictionary(Of String, AggregableColumnReduction)()
			Friend conditionalReductions As IDictionary(Of String, ConditionalReduction) = New Dictionary(Of String, ConditionalReduction)()
			Friend ignoreInvalidInColumns As ISet(Of String) = New HashSet(Of String)()
'JAVA TO VB CONVERTER NOTE: The field keyColumns was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend keyColumns_Conflict() As String


			''' <summary>
			''' Create a Reducer builder, and set the default column reduction operation.
			''' For any columns that aren't specified explicitly, they will use the default reduction operation.
			''' If a column does have a reduction operation explicitly specified, then it will override
			''' the default specified here.
			''' </summary>
			''' <param name="defaultOp"> Default reduction operation to perform </param>
			Public Sub New(ByVal defaultOp As ReduceOp)
				Me.defaultOp = defaultOp
			End Sub

			''' <summary>
			''' Specify the key columns. The idea here is to be able to create a (potentially compound) key
			''' out of multiple columns, using the toString representation of the values in these columns
			''' </summary>
			''' <param name="keyColumns"> Columns that will make up the key
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter keyColumns was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function keyColumns(ParamArray ByVal keyColumns_Conflict() As String) As Builder
				Me.keyColumns_Conflict = keyColumns_Conflict
				Return Me
			End Function

			Friend Overridable Function add(ByVal op As ReduceOp, ByVal cols() As String) As Builder
				For Each s As String In cols
					Dim ops As IList(Of ReduceOp) = New List(Of ReduceOp)()
					If opMap.ContainsKey(s) Then
						CType(ops, List(Of ReduceOp)).AddRange(opMap(s))
					End If
					ops.Add(op)
					opMap(s) = ops
				Next s
				Return Me
			End Function

			Friend Overridable Function addAll(ByVal ops As IList(Of ReduceOp), ByVal cols() As String) As Builder
				For Each s As String In cols
					Dim theseOps As IList(Of ReduceOp) = New List(Of ReduceOp)()
					If opMap.ContainsKey(s) Then
						CType(theseOps, List(Of ReduceOp)).AddRange(opMap(s))
					End If
					CType(theseOps, List(Of ReduceOp)).AddRange(ops)
					opMap(s) = theseOps
				Next s
				Return Me
			End Function

			Public Overridable Function multipleOpColmumns(ByVal ops As IList(Of ReduceOp), ParamArray ByVal columns() As String) As Builder
				Return addAll(ops, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the minimum value
			''' </summary>
			Public Overridable Function minColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Min, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the maximum value
			''' </summary>
			Public Overridable Function maxColumn(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Max, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the sum of values
			''' </summary>
			Public Overridable Function sumColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Sum, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the product of values
			''' </summary>
			Public Overridable Function prodColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Prod, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the mean of the values
			''' </summary>
			Public Overridable Function meanColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Mean, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the standard deviation of the values
			''' </summary>
			Public Overridable Function stdevColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Stdev, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the uncorrected standard deviation of the values
			''' </summary>
			Public Overridable Function uncorrectedStdevColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Stdev, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the variance of the values
			''' </summary>
			Public Overridable Function variance(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Variance, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the population variance of the values
			''' </summary>
			Public Overridable Function populationVariance(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.PopulationVariance, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by counting the number of values
			''' </summary>
			Public Overridable Function countColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Count, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the range (max-min) of the values
			''' </summary>
			Public Overridable Function rangeColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Range, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by counting the number of unique values
			''' </summary>
			Public Overridable Function countUniqueColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.CountUnique, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the first value
			''' </summary>
			Public Overridable Function takeFirstColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.TakeFirst, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the last value
			''' </summary>
			Public Overridable Function takeLastColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.TakeLast, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the concatenation of all content
			''' Beware, the output will be huge!
			''' </summary>
			Public Overridable Function appendColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Append, columns)
			End Function

			''' <summary>
			''' Reduce the specified columns by taking the concatenation of all content in the reverse order
			''' Beware, the output will be huge!
			''' </summary>
			Public Overridable Function prependColumns(ParamArray ByVal columns() As String) As Builder
				Return add(ReduceOp.Prepend, columns)
			End Function

			''' <summary>
			''' Reduce the specified column using a custom column reduction functionality.
			''' </summary>
			''' <param name="column">          Column to execute the custom reduction functionality on </param>
			''' <param name="columnReduction"> Column reduction to execute on that column </param>
			Public Overridable Function customReduction(ByVal column As String, ByVal columnReduction As AggregableColumnReduction) As Builder
				customReductions(column) = columnReduction
				Return Me
			End Function

			''' <summary>
			''' Conditional reduction: apply the reduces on a specified column, where the reduction occurs *only* on those
			''' examples where the condition returns true. Examples where the condition does not apply (returns false) are
			''' ignored/excluded.
			''' </summary>
			''' <param name="column">     Name of the column to execute the conditional reduction on </param>
			''' <param name="outputName"> Name of the column, after the reduction has been executed </param>
			''' <param name="reductions">  Reductions to execute </param>
			''' <param name="condition">  Condition to use in the reductions </param>
			Public Overridable Function conditionalReduction(ByVal column As String, ByVal outputNames As IList(Of String), ByVal reductions As IList(Of ReduceOp), ByVal condition As Condition) As Builder
				Preconditions.checkArgument(outputNames.Count = reductions.Count, "Conditional reductions should provide names for every column")
				Me.conditionalReductions(column) = New ConditionalReduction(column, outputNames, reductions, condition)
				Return Me
			End Function

			''' <summary>
			''' Conditional reduction: apply the reduces on a specified column, where the reduction occurs *only* on those
			''' examples where the condition returns true. Examples where the condition does not apply (returns false) are
			''' ignored/excluded.
			''' </summary>
			''' <param name="column">     Name of the column to execute the conditional reduction on </param>
			''' <param name="outputName"> Name of the column, after the reduction has been executed </param>
			''' <param name="reductions">  Reductions to execute </param>
			''' <param name="condition">  Condition to use in the reductions </param>
			Public Overridable Function conditionalReduction(ByVal column As String, ByVal outputName As String, ByVal reduction As ReduceOp, ByVal condition As Condition) As Builder
				Me.conditionalReductions(column) = New ConditionalReduction(column, Collections.singletonList(outputName), Collections.singletonList(reduction), condition)
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

			Public Overridable Function build() As Reducer
				Return New Reducer(Me)
			End Function
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class ConditionalReduction implements java.io.Serializable
		<Serializable>
		Public Class ConditionalReduction
			Friend ReadOnly columnName As String
			Friend ReadOnly outputNames As IList(Of String)
			Friend ReadOnly reductions As IList(Of ReduceOp)
			Friend ReadOnly condition As Condition
		End Class

	End Class

End Namespace