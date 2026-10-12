Imports System
Imports System.Collections.Generic
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports org.apache.spark.sql
Imports DataTypes = org.apache.spark.sql.types.DataTypes
Imports Metadata = org.apache.spark.sql.types.Metadata
Imports StructField = org.apache.spark.sql.types.StructField
Imports StructType = org.apache.spark.sql.types.StructType
Imports org.nd4j.common.primitives
Imports Schema = org.datavec.api.transform.schema.Schema
Imports org.datavec.api.writable
Imports SequenceToRows = org.datavec.spark.transform.sparkfunction.SequenceToRows
Imports ToRecord = org.datavec.spark.transform.sparkfunction.ToRecord
Imports ToRow = org.datavec.spark.transform.sparkfunction.ToRow
Imports DataFrameToSequenceCreateCombiner = org.datavec.spark.transform.sparkfunction.sequence.DataFrameToSequenceCreateCombiner
Imports DataFrameToSequenceMergeCombiner = org.datavec.spark.transform.sparkfunction.sequence.DataFrameToSequenceMergeCombiner
Imports DataFrameToSequenceMergeValue = org.datavec.spark.transform.sparkfunction.sequence.DataFrameToSequenceMergeValue
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.apache.spark.sql.functions.avg
import static org.apache.spark.sql.functions.col

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

Namespace org.datavec.spark.transform



	Public Class DataFrames

		Public Const SEQUENCE_UUID_COLUMN As String = "__SEQ_UUID"
		Public Const SEQUENCE_INDEX_COLUMN As String = "__SEQ_IDX"

		Private Sub New()
		End Sub

		''' <summary>
		''' Standard deviation for a column
		''' </summary>
		''' <param name="dataFrame">  the dataframe to
		'''                   get the column from </param>
		''' <param name="columnName"> the name of the column to get the standard
		'''                   deviation for </param>
		''' <returns> the column that represents the standard deviation </returns>
		Public Shared Function std(ByVal dataFrame As Dataset(Of Row), ByVal columnName As String) As Column
			Return functions.sqrt(var(dataFrame, columnName))
		End Function


		''' <summary>
		''' Standard deviation for a column
		''' </summary>
		''' <param name="dataFrame">  the dataframe to
		'''                   get the column from </param>
		''' <param name="columnName"> the name of the column to get the standard
		'''                   deviation for </param>
		''' <returns> the column that represents the standard deviation </returns>
		Public Shared Function var(ByVal dataFrame As Dataset(Of Row), ByVal columnName As String) As Column
			Return dataFrame.groupBy(columnName).agg(functions.variance(columnName)).col(columnName)
		End Function

		''' <summary>
		''' MIn for a column
		''' </summary>
		''' <param name="dataFrame">  the dataframe to
		'''                   get the column from </param>
		''' <param name="columnName"> the name of the column to get the min for </param>
		''' <returns> the column that represents the min </returns>
		Public Shared Function min(ByVal dataFrame As Dataset(Of Row), ByVal columnName As String) As Column
			Return dataFrame.groupBy(columnName).agg(functions.min(columnName)).col(columnName)
		End Function

		''' <summary>
		''' Max for a column
		''' </summary>
		''' <param name="dataFrame">  the dataframe to
		'''                   get the column from </param>
		''' <param name="columnName"> the name of the column
		'''                   to get the max for </param>
		''' <returns> the column that represents the max </returns>
		Public Shared Function max(ByVal dataFrame As Dataset(Of Row), ByVal columnName As String) As Column
			Return dataFrame.groupBy(columnName).agg(functions.max(columnName)).col(columnName)
		End Function

		''' <summary>
		''' Mean for a column
		''' </summary>
		''' <param name="dataFrame">  the dataframe to
		'''                   get the column fron </param>
		''' <param name="columnName"> the name of the column to get the mean for </param>
		''' <returns> the column that represents the mean </returns>
		Public Shared Function mean(ByVal dataFrame As Dataset(Of Row), ByVal columnName As String) As Column
			Return dataFrame.groupBy(columnName).agg(avg(columnName)).col(columnName)
		End Function

		''' <summary>
		''' Convert a datavec schema to a
		''' struct type in spark
		''' </summary>
		''' <param name="schema"> the schema to convert </param>
		''' <returns> the datavec struct type </returns>
		Public Shared Function fromSchema(ByVal schema As Schema) As StructType
			Dim structFields(schema.numColumns() - 1) As StructField
			For i As Integer = 0 To structFields.Length - 1
				Select Case schema.getColumnTypes()(i)
					Case Double?
						structFields(i) = New StructField(schema.getName(i), DataTypes.DoubleType, False, Metadata.empty())
					Case Integer?
						structFields(i) = New StructField(schema.getName(i), DataTypes.IntegerType, False, Metadata.empty())
					Case Long?
						structFields(i) = New StructField(schema.getName(i), DataTypes.LongType, False, Metadata.empty())
					Case Single?
						structFields(i) = New StructField(schema.getName(i), DataTypes.FloatType, False, Metadata.empty())
					Case Else
						Throw New System.InvalidOperationException("This api should not be used with strings , binary data or ndarrays. This is only for columnar data")
				End Select
			Next i
			Return New StructType(structFields)
		End Function

		''' <summary>
		''' Convert the DataVec sequence schema to a StructType for Spark, for example for use in
		''' <seealso cref="toDataFrameSequence(Schema, JavaRDD)"/>}
		''' <b>Note</b>: as per <seealso cref="toDataFrameSequence(Schema, JavaRDD)"/>}, the StructType has two additional columns added to it:<br>
		''' - Column 0: Sequence UUID (name: <seealso cref="SEQUENCE_UUID_COLUMN"/>) - a UUID for the original sequence<br>
		''' - Column 1: Sequence index (name: <seealso cref="SEQUENCE_INDEX_COLUMN"/> - an index (integer, starting at 0) for the position
		''' of this record in the original time series.<br>
		''' These two columns are required if the data is to be converted back into a sequence at a later point, for example
		''' using <seealso cref="toRecordsSequence(Dataset<Row>)"/>
		''' </summary>
		''' <param name="schema"> Schema to convert </param>
		''' <returns> StructType for the schema </returns>
		Public Shared Function fromSchemaSequence(ByVal schema As Schema) As StructType
			Dim structFields(schema.numColumns() + 1) As StructField

			structFields(0) = New StructField(SEQUENCE_UUID_COLUMN, DataTypes.StringType, False, Metadata.empty())
			structFields(1) = New StructField(SEQUENCE_INDEX_COLUMN, DataTypes.IntegerType, False, Metadata.empty())

			Dim i As Integer = 0
			Do While i < schema.numColumns()
				Select Case schema.getColumnTypes()(i)
					Case Double?
						structFields(i + 2) = New StructField(schema.getName(i), DataTypes.DoubleType, False, Metadata.empty())
					Case Integer?
						structFields(i + 2) = New StructField(schema.getName(i), DataTypes.IntegerType, False, Metadata.empty())
					Case Long?
						structFields(i + 2) = New StructField(schema.getName(i), DataTypes.LongType, False, Metadata.empty())
					Case Single?
						structFields(i + 2) = New StructField(schema.getName(i), DataTypes.FloatType, False, Metadata.empty())
					Case Else
						Throw New System.InvalidOperationException("This api should not be used with strings , binary data or ndarrays. This is only for columnar data")
				End Select
				i += 1
			Loop
			Return New StructType(structFields)
		End Function


		''' <summary>
		''' Create a datavec schema
		''' from a struct type
		''' </summary>
		''' <param name="structType"> the struct type to create the schema from </param>
		''' <returns> the created schema </returns>
		Public Shared Function fromStructType(ByVal structType As StructType) As Schema
			Dim builder As New Schema.Builder()
			Dim fields() As StructField = structType.fields()
			Dim fieldNames() As String = structType.fieldNames()
			For i As Integer = 0 To fields.Length - 1
				Dim name As String = fields(i).dataType().typeName().ToLower()
				Select Case name
					Case "double"
						builder.addColumnDouble(fieldNames(i))
					Case "float"
						builder.addColumnFloat(fieldNames(i))
					Case "long"
						builder.addColumnLong(fieldNames(i))
					Case "int", "integer"
						builder.addColumnInteger(fieldNames(i))
					Case "string"
						builder.addColumnString(fieldNames(i))
					Case Else
						Throw New Exception("Unknown type: " & name)
				End Select
			Next i

			Return builder.build()
		End Function


		''' <summary>
		''' Create a compatible schema
		''' and rdd for datavec
		''' </summary>
		''' <param name="dataFrame"> the dataframe to convert </param>
		''' <returns> the converted schema and rdd of writables </returns>
		Public Shared Function toRecords(ByVal dataFrame As Dataset(Of Row)) As Pair(Of Schema, JavaRDD(Of IList(Of Writable)))
			Dim schema As Schema = fromStructType(dataFrame.schema())
			Return New Pair(Of Schema, JavaRDD(Of IList(Of Writable)))(schema, dataFrame.javaRDD().map(New ToRecord(schema)))
		End Function

		''' <summary>
		''' Convert the given DataFrame to a sequence<br>
		''' <b>Note</b>: It is assumed here that the DataFrame has been created by <seealso cref="toDataFrameSequence(Schema, JavaRDD)"/>.
		''' In particular:<br>
		''' - the first column is a UUID for the original sequence the row is from<br>
		''' - the second column is a time step index: where the row appeared in the original sequence<br>
		''' <para>
		''' Typical use: Normalization via the <seealso cref="Normalization"/> static methods
		''' 
		''' </para>
		''' </summary>
		''' <param name="dataFrame"> Data frame to convert </param>
		''' <returns> Data in sequence (i.e., {@code List<List<Writable>>} form </returns>
		Public Shared Function toRecordsSequence(ByVal dataFrame As Dataset(Of Row)) As Pair(Of Schema, JavaRDD(Of IList(Of IList(Of Writable))))

			'Need to convert from flattened to sequence data...
			'First: Group by the Sequence UUID (first column)
			Dim grouped As JavaPairRDD(Of String, IEnumerable(Of Row)) = dataFrame.javaRDD().groupBy(New FunctionAnonymousInnerClass())


			Dim schema As Schema = fromStructType(dataFrame.schema())

			'Group by sequence UUID, and sort each row within the sequences using the time step index
			Dim createCombiner As [Function](Of IEnumerable(Of Row), IList(Of IList(Of Writable))) = New DataFrameToSequenceCreateCombiner(schema) 'Function to create the initial combiner
			Dim mergeValue As Function2(Of IList(Of IList(Of Writable)), IEnumerable(Of Row), IList(Of IList(Of Writable))) = New DataFrameToSequenceMergeValue(schema) 'Function to add a row
			Dim mergeCombiners As Function2(Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)), IList(Of IList(Of Writable))) = New DataFrameToSequenceMergeCombiner() 'Function to merge existing sequence writables

			Dim sequences As JavaRDD(Of IList(Of IList(Of Writable))) = grouped.combineByKey(createCombiner, mergeValue, mergeCombiners).values()

			'We no longer want/need the sequence UUID and sequence time step columns - extract those out
			Dim [out] As JavaRDD(Of IList(Of IList(Of Writable))) = sequences.map(New FunctionAnonymousInnerClass2())

			Return New Pair(Of Schema, JavaRDD(Of IList(Of IList(Of Writable))))(schema, [out])
		End Function

		Private Class FunctionAnonymousInnerClass
			Inherits [Function](Of Row, String)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public String call(Row row) throws Exception
			Public Overrides Function [call](ByVal row As Row) As String
				Return row.getString(0)
			End Function
		End Class

		Private Class FunctionAnonymousInnerClass2
			Inherits [Function](Of IList(Of IList(Of Writable)), IList(Of IList(Of Writable)))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<Writable>> call(java.util.List<java.util.List<Writable>> v1) throws Exception
			Public Overrides Function [call](ByVal v1 As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))
				Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(v1.Count)
				For Each l As IList(Of Writable) In v1
					Dim subset As IList(Of Writable) = New List(Of Writable)()
					For i As Integer = 2 To l.Count - 1
						subset.Add(l(i))
					Next i
					[out].Add(subset)
				Next l
				Return [out]
			End Function
		End Class

		''' <summary>
		''' Creates a data frame from a collection of writables
		''' rdd given a schema
		''' </summary>
		''' <param name="schema"> the schema to use </param>
		''' <param name="data">   the data to convert </param>
		''' <returns> the dataframe object </returns>
		Public Shared Function toDataFrame(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of Writable))) As Dataset(Of Row)
			Dim sc As New JavaSparkContext(data.context())
			Dim sqlContext As New SQLContext(sc)
			Dim rows As JavaRDD(Of Row) = data.map(New ToRow(schema))
			Return sqlContext.createDataFrame(rows, fromSchema(schema))
		End Function


		''' <summary>
		''' Convert the given sequence data set to a DataFrame.<br>
		''' <b>Note</b>: The resulting DataFrame has two additional columns added to it:<br>
		''' - Column 0: Sequence UUID (name: <seealso cref="SEQUENCE_UUID_COLUMN"/>) - a UUID for the original sequence<br>
		''' - Column 1: Sequence index (name: <seealso cref="SEQUENCE_INDEX_COLUMN"/> - an index (integer, starting at 0) for the position
		''' of this record in the original time series.<br>
		''' These two columns are required if the data is to be converted back into a sequence at a later point, for example
		''' using <seealso cref="toRecordsSequence(Dataset<Row>)"/>
		''' </summary>
		''' <param name="schema"> Schema for the data </param>
		''' <param name="data">   Sequence data to convert to a DataFrame </param>
		''' <returns> The dataframe object </returns>
		Public Shared Function toDataFrameSequence(ByVal schema As Schema, ByVal data As JavaRDD(Of IList(Of IList(Of Writable)))) As Dataset(Of Row)
			Dim sc As New JavaSparkContext(data.context())

			Dim sqlContext As New SQLContext(sc)
			Dim rows As JavaRDD(Of Row) = data.flatMap(New SequenceToRows(schema))
			Return sqlContext.createDataFrame(rows, fromSchemaSequence(schema))
		End Function

		''' <summary>
		''' Convert a given Row to a list of writables, given the specified Schema
		''' </summary>
		''' <param name="schema"> Schema for the data </param>
		''' <param name="row">    Row of data </param>
		Public Shared Function rowToWritables(ByVal schema As Schema, ByVal row As Row) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)()
			For i As Integer = 0 To row.size() - 1
				Select Case schema.getType(i)
					Case Double?
						ret.Add(New DoubleWritable(row.getDouble(i)))
					Case Single?
						ret.Add(New FloatWritable(row.getFloat(i)))
					Case Integer?
						ret.Add(New IntWritable(row.getInt(i)))
					Case Long?
						ret.Add(New LongWritable(row.getLong(i)))
					Case String
						ret.Add(New Text(row.getString(i)))
					Case Else
						Throw New System.InvalidOperationException("Illegal type")
				End Select
			Next i
			Return ret
		End Function

		''' <summary>
		''' Convert a string array into a list </summary>
		''' <param name="input"> the input to create the list from </param>
		''' <returns> the created array </returns>
		Public Shared Function toList(ByVal input() As String) As IList(Of String)
			Dim ret As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To input.Length - 1
				ret.Add(input(i))
			Next i
			Return ret
		End Function


		''' <summary>
		''' Convert a string list into a array </summary>
		''' <param name="list"> the input to create the array from </param>
		''' <returns> the created list </returns>
		Public Shared Function toArray(ByVal list As IList(Of String)) As String()
			Dim ret(list.Count - 1) As String
			For i As Integer = 0 To ret.Length - 1
				ret(i) = list(i)
			Next i
			Return ret
		End Function

		''' <summary>
		''' Convert a list of rows to a matrix </summary>
		''' <param name="rows"> the list of rows to convert </param>
		''' <returns> the converted matrix </returns>
		Public Shared Function toMatrix(ByVal rows As IList(Of Row)) As INDArray
			Dim ret As INDArray = Nd4j.create(rows.Count, rows(0).size())
			Dim i As Integer = 0
			Do While i < ret.rows()
				Dim j As Integer = 0
				Do While j < ret.columns()
					ret.putScalar(i, j, rows(i).getDouble(j))
					j += 1
				Loop
				i += 1
			Loop
			Return ret
		End Function


		''' <summary>
		''' Convert a list of string names
		''' to columns </summary>
		''' <param name="columns"> the columns to convert </param>
		''' <returns> the resulting column list </returns>
		Public Shared Function toColumn(ByVal columns As IList(Of String)) As IList(Of Column)
			Dim ret As IList(Of Column) = New List(Of Column)()
			For Each s As String In columns
				ret.Add(col(s))
			Next s
			Return ret
		End Function

		''' <summary>
		''' Convert an array of strings
		''' to column names </summary>
		''' <param name="columns"> the columns to convert </param>
		''' <returns> the converted columns </returns>
		Public Shared Function toColumns(ParamArray ByVal columns() As String) As Column()
			Dim ret(columns.Length - 1) As Column
			For i As Integer = 0 To columns.Length - 1
				ret(i) = col(columns(i))
			Next i
			Return ret
		End Function

	End Class

End Namespace