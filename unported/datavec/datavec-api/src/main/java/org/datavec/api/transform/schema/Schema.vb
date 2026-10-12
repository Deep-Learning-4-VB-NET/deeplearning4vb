Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports System.Linq
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.metadata
Imports JsonMappers = org.datavec.api.transform.serde.JsonMappers
Imports org.datavec.api.writable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports org.nd4j.shade.jackson.annotation
Imports JsonFactory = org.nd4j.shade.jackson.core.JsonFactory
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
Imports InvalidTypeIdException = org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException
Imports YAMLFactory = org.nd4j.shade.jackson.dataformat.yaml.YAMLFactory
Imports JodaModule = org.nd4j.shade.jackson.datatype.joda.JodaModule

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

Namespace org.datavec.api.transform.schema


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"columnNames", "columnNamesIndex"}) @EqualsAndHashCode @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @Data public class Schema implements java.io.Serializable
	<Serializable>
	Public Class Schema

		Private columnNames As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonProperty("columns") private List<ColumnMetaData> columnMetaData;
		Private columnMetaData As IList(Of ColumnMetaData)
		Private columnNamesIndex As IDictionary(Of String, Integer) 'For efficient lookup

		Private Sub New()
			'No-arg constructor for Jackson
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			Me.columnMetaData = builder.columnMetaData
			Me.columnNames = New List(Of String)()
			For Each meta As ColumnMetaData In Me.columnMetaData
				Me.columnNames.Add(meta.Name)
			Next meta
			columnNamesIndex = New Dictionary(Of String, Integer)()
			For i As Integer = 0 To columnNames.Count - 1
				columnNamesIndex(columnNames(i)) = i
			Next i
		End Sub

		''' <summary>
		''' Create a schema based on the
		''' given metadata </summary>
		''' <param name="columnMetaData"> the metadata to create the
		'''                       schema from </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Schema(@JsonProperty("columns") List<ColumnMetaData> columnMetaData)
		Public Sub New(ByVal columnMetaData As IList(Of ColumnMetaData))
			If columnMetaData Is Nothing OrElse columnMetaData.Count = 0 Then
				Throw New System.ArgumentException("Column meta data must be non-empty")
			End If
			Me.columnMetaData = columnMetaData
			Me.columnNames = New List(Of String)()
			For Each meta As ColumnMetaData In Me.columnMetaData
				Me.columnNames.Add(meta.Name)
			Next meta
			Me.columnNamesIndex = New Dictionary(Of String, Integer)()
			For i As Integer = 0 To columnNames.Count - 1
				columnNamesIndex(columnNames(i)) = i
			Next i
		End Sub


		''' <summary>
		''' Returns true if the given schema
		''' has the same types at each index </summary>
		''' <param name="schema"> the schema to compare the types to </param>
		''' <returns> true if the schema has the same types
		''' at every index as this one,false otherwise </returns>
'JAVA TO VB CONVERTER NOTE: The parameter schema was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function sameTypes(ByVal schema_Conflict As Schema) As Boolean
			If schema_Conflict.numColumns() <> numColumns() Then
				Return False
			End If
			Dim i As Integer = 0
			Do While i < schema_Conflict.numColumns()
				If [getType](i) <> schema_Conflict.getType(i) Then
					Return False
				End If
				i += 1
			Loop

			Return True
		End Function

		''' <summary>
		''' Compute the difference in <seealso cref="ColumnMetaData"/>
		''' between this schema and the passed in schema.
		''' This is useful during the <seealso cref="org.datavec.api.transform.TransformProcess"/>
		''' to identify what a process will do to a given <seealso cref="Schema"/>.
		''' </summary>
		''' <param name="schema"> the schema to compute the difference for </param>
		''' <returns> the metadata that is different (in order)
		''' between this schema and the other schema </returns>
'JAVA TO VB CONVERTER NOTE: The parameter schema was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function differences(ByVal schema_Conflict As Schema) As IList(Of ColumnMetaData)
			Dim ret As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
			Dim i As Integer = 0
			Do While i < schema_Conflict.numColumns()
				If Not columnMetaData.Contains(schema_Conflict.getMetaData(i)) Then
					ret.Add(schema_Conflict.getMetaData(i))
				End If
				i += 1
			Loop

			Return ret
		End Function

		''' <summary>
		''' Create a new schema based on the new metadata </summary>
		''' <param name="columnMetaData"> the new metadata to create the
		'''                       schema from </param>
		''' <returns> the new schema </returns>
		Public Overridable Function newSchema(ByVal columnMetaData As IList(Of ColumnMetaData)) As Schema
			Return New Schema(columnMetaData)
		End Function

		''' <summary>
		''' Returns the number of columns or fields
		''' for this schema </summary>
		''' <returns> the number of columns or fields for this schema </returns>
		Public Overridable Function numColumns() As Integer
			Return columnNames.Count
		End Function

		''' <summary>
		''' Returns the name of a
		''' given column at the specified index </summary>
		''' <param name="column"> the index of the column
		'''               to get the name for </param>
		''' <returns> the name of the column at the specified index </returns>
		Public Overridable Function getName(ByVal column As Integer) As String
			Return columnNames(column)
		End Function

		''' <summary>
		''' Returns the <seealso cref="ColumnType"/>
		''' for the column at the specified index </summary>
		''' <param name="column"> the index of the column to get the type for </param>
		''' <returns> the type of the column to at the specified inde </returns>
		Public Overridable Function [getType](ByVal column As Integer) As ColumnType
			If column < 0 OrElse column >= columnMetaData.Count Then
				Throw New System.ArgumentException("Invalid column number. " & column & "only " & columnMetaData.Count & "present.")
			End If
			Return columnMetaData(column).getColumnType()
		End Function

		''' <summary>
		''' Returns the <seealso cref="ColumnType"/>
		''' for the column at the specified index </summary>
		''' <param name="columnName"> the index of the column to get the type for </param>
		''' <returns> the type of the column to at the specified inde </returns>
		Public Overridable Function [getType](ByVal columnName As String) As ColumnType
			If Not hasColumn(columnName) Then
				Throw New System.ArgumentException("Column """ & columnName & """ does not exist in schema")
			End If
			Return getMetaData(columnName).ColumnType
		End Function

		''' <summary>
		''' Returns the <seealso cref="ColumnMetaData"/>
		''' at the specified column index </summary>
		''' <param name="column"> the index
		'''               to get the metadata for </param>
		''' <returns> the metadata at ths specified index </returns>
		Public Overridable Function getMetaData(ByVal column As Integer) As ColumnMetaData
			Return columnMetaData(column)
		End Function

		''' <summary>
		''' Retrieve the metadata for the given
		''' column name </summary>
		''' <param name="column"> the name of the column to get metadata for </param>
		''' <returns> the metadata for the given column name </returns>
		Public Overridable Function getMetaData(ByVal column As String) As ColumnMetaData
			Return getMetaData(getIndexOfColumn(column))
		End Function

		''' <summary>
		''' Return a copy of the list column names </summary>
		''' <returns> a copy of the list of column names
		''' for this schema </returns>
		Public Overridable ReadOnly Property ColumnNames As IList(Of String)
			Get
				Return New List(Of String)(columnNames)
			End Get
		End Property

		''' <summary>
		''' A copy of the list of <seealso cref="ColumnType"/>
		''' for this schema </summary>
		''' <returns> the list of column  types in order based
		''' on column index for this schema </returns>
		Public Overridable ReadOnly Property ColumnTypes As IList(Of ColumnType)
			Get
				Dim list As IList(Of ColumnType) = New List(Of ColumnType)(columnMetaData.Count)
				For Each md As ColumnMetaData In columnMetaData
					list.Add(md.ColumnType)
				Next md
				Return list
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the underlying
		''' schema <seealso cref="ColumnMetaData"/> </summary>
		''' <returns> the list of schema metadata </returns>
		Public Overridable ReadOnly Property ColumnMetaData As IList(Of ColumnMetaData)
			Get
				Return New List(Of ColumnMetaData)(columnMetaData)
			End Get
		End Property

		''' <summary>
		''' Returns the index for the given
		''' column name </summary>
		''' <param name="columnName"> the column name to get the
		'''                   index for </param>
		''' <returns> the index of the given column name
		''' for the schema </returns>
		Public Overridable Function getIndexOfColumn(ByVal columnName As String) As Integer
			Dim idx As Integer? = columnNamesIndex(columnName)
			If idx Is Nothing Then
				Throw New NoSuchElementException("Unknown column: """ & columnName & """")
			End If
			Return idx
		End Function

		''' <summary>
		''' Return the indices of the columns, given their namess
		''' </summary>
		''' <param name="columnNames"> Name of the columns to get indices for </param>
		''' <returns> Column indexes </returns>
		Public Overridable Function getIndexOfColumns(ByVal columnNames As ICollection(Of String)) As Integer()
			Return getIndexOfColumns(columnNames.ToArray())
		End Function

		''' <summary>
		''' Return the indices of the columns, given their namess
		''' </summary>
		''' <param name="columnNames"> Name of the columns to get indices for </param>
		''' <returns> Column indexes </returns>
		Public Overridable Function getIndexOfColumns(ParamArray ByVal columnNames() As String) As Integer()
			Dim [out](columnNames.Length - 1) As Integer
			For i As Integer = 0 To [out].Length - 1
				[out](i) = getIndexOfColumn(columnNames(i))
			Next i
			Return [out]
		End Function

		''' <summary>
		''' Determine if the schema has a column with the specified name
		''' </summary>
		''' <param name="columnName"> Name to see if the column exists </param>
		''' <returns> True if a column exists for that name, false otherwise </returns>
		Public Overridable Function hasColumn(ByVal columnName As String) As Boolean
			Dim idx As Integer? = columnNamesIndex(columnName)
			Return idx IsNot Nothing
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			Dim nCol As Integer = numColumns()

			Dim maxNameLength As Integer = 0
			For Each s As String In getColumnNames()
				maxNameLength = Math.Max(maxNameLength, s.Length)
			Next s

			'Header:
			sb.Append("Schema():" & vbLf)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			sb.Append(String.Format("{0,-6}", "idx")).Append(String.Format("%-" & (maxNameLength + 8) & "s", "name")).Append(String.Format("{0,-15}", "type")).Append("meta data").Append(vbLf)

			For i As Integer = 0 To nCol - 1
				Dim colName As String = getName(i)
				Dim type As ColumnType = [getType](i)
				Dim meta As ColumnMetaData = getMetaData(i)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Dim paddedName As String = String.Format("%-" & (maxNameLength + 8) & "s", """" & colName & """")
				sb.Append(String.Format("{0,-6:D}", i)).Append(paddedName).Append(String.Format("{0,-15}", type)).Append(meta).Append(vbLf)
			Next i

			Return sb.ToString()
		End Function

		''' <summary>
		''' Serialize this schema to json </summary>
		''' <returns> a json representation of this schema </returns>
		Public Overridable Function toJson() As String
			Return toJacksonString(New JsonFactory())
		End Function

		''' <summary>
		''' Serialize this schema to yaml </summary>
		''' <returns> the yaml representation of this schema </returns>
		Public Overridable Function toYaml() As String
			Return toJacksonString(New YAMLFactory())
		End Function

		Private Function toJacksonString(ByVal factory As JsonFactory) As String
			Dim om As New ObjectMapper(factory)
			om.registerModule(New JodaModule())
			om.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			om.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			om.enable(SerializationFeature.INDENT_OUTPUT)
			om.setVisibility(PropertyAccessor.ALL, JsonAutoDetect.Visibility.NONE)
			om.setVisibility(PropertyAccessor.FIELD, JsonAutoDetect.Visibility.ANY)
			Dim str As String
			Try
				str = om.writeValueAsString(Me)
			Catch e As Exception
				Throw New Exception(e)
			End Try

			Return str
		End Function

		''' <summary>
		''' Create a schema from a given json string </summary>
		''' <param name="json"> the json to create the schema from </param>
		''' <returns> the created schema based on the json </returns>
		Public Shared Function fromJson(ByVal json As String) As Schema
			Try
				Return JsonMappers.Mapper.readValue(json, GetType(Schema))
			Catch e As InvalidTypeIdException
				If e.Message.contains("@class") Then
					Try
						'JSON may be legacy (1.0.0-alpha or earlier), attempt to load it using old format
						Return JsonMappers.LegacyMapper.readValue(json, GetType(Schema))
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try
				End If
				Throw New Exception(e)
			Catch e As Exception
				'TODO better exceptions
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Create a schema from the given
		''' yaml string </summary>
		''' <param name="yaml"> the yaml to create the schema from </param>
		''' <returns> the created schema based on the yaml </returns>
		Public Shared Function fromYaml(ByVal yaml As String) As Schema
			Try
				Return JsonMappers.MapperYaml.readValue(yaml, GetType(Schema))
			Catch e As Exception
				'TODO better exceptions
				Throw New Exception(e)
			End Try
		End Function

		Public Class Builder
			Friend columnMetaData As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()

			''' <summary>
			''' Add a Float column with no restrictions on the allowable values, except for no NaN/infinite values allowed
			''' </summary>
			''' <param name="name"> Name of the column </param>
			Public Overridable Function addColumnFloat(ByVal name As String) As Builder
				Return addColumn(New FloatMetaData(name))
			End Function

			''' <summary>
			''' Add a Float column with the specified restrictions (and no NaN/Infinite values allowed)
			''' </summary>
			''' <param name="name">            Name of the column </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction
			''' @return </param>
			Public Overridable Function addColumnFloat(ByVal name As String, ByVal minAllowedValue As Single?, ByVal maxAllowedValue As Single?) As Builder
				Return addColumnFloat(name, minAllowedValue, maxAllowedValue, False, False)
			End Function

			''' <summary>
			''' Add a Float column with the specified restrictions
			''' </summary>
			''' <param name="name">            Name of the column </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			''' <param name="allowNaN">        If false: don't allow NaN values. If true: allow. </param>
			''' <param name="allowInfinite">   If false: don't allow infinite values. If true: allow </param>
			Public Overridable Function addColumnFloat(ByVal name As String, ByVal minAllowedValue As Single?, ByVal maxAllowedValue As Single?, ByVal allowNaN As Boolean, ByVal allowInfinite As Boolean) As Builder
				Return addColumn(New FloatMetaData(name, minAllowedValue, maxAllowedValue, allowNaN, allowInfinite))
			End Function

			''' <summary>
			''' Add multiple Float columns with no restrictions on the allowable values of the columns (other than no NaN/Infinite)
			''' </summary>
			''' <param name="columnNames"> Names of the columns to add </param>
			Public Overridable Function addColumnsFloat(ParamArray ByVal columnNames() As String) As Builder
				For Each s As String In columnNames
					addColumnFloat(s)
				Next s
				Return Me
			End Function

			''' <summary>
			''' A convenience method for adding multiple Float columns.
			''' For example, to add columns "myFloatCol_0", "myFloatCol_1", "myFloatCol_2", use
			''' {@code addColumnsFloat("myFloatCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			Public Overridable Function addColumnsFloat(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer) As Builder
				Return addColumnsFloat(pattern, minIdxInclusive, maxIdxInclusive, Nothing, Nothing, False, False)
			End Function

			''' <summary>
			''' A convenience method for adding multiple Float columns, with additional restrictions that apply to all columns
			''' For example, to add columns "myFloatCol_0", "myFloatCol_1", "myFloatCol_2", use
			''' {@code addColumnsFloat("myFloatCol_%d",0,2,null,null,false,false)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			''' <param name="allowNaN">        If false: don't allow NaN values. If true: allow. </param>
			''' <param name="allowInfinite">   If false: don't allow infinite values. If true: allow </param>
			Public Overridable Function addColumnsFloat(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer, ByVal minAllowedValue As Single?, ByVal maxAllowedValue As Single?, ByVal allowNaN As Boolean, ByVal allowInfinite As Boolean) As Builder
				For i As Integer = minIdxInclusive To maxIdxInclusive
					addColumnFloat(String.format(pattern, i), minAllowedValue, maxAllowedValue, allowNaN, allowInfinite)
				Next i
				Return Me
			End Function


			''' <summary>
			''' Add a Double column with no restrictions on the allowable values, except for no NaN/infinite values allowed
			''' </summary>
			''' <param name="name"> Name of the column </param>
			Public Overridable Function addColumnDouble(ByVal name As String) As Builder
				Return addColumn(New DoubleMetaData(name))
			End Function

			''' <summary>
			''' Add a Double column with the specified restrictions (and no NaN/Infinite values allowed)
			''' </summary>
			''' <param name="name">            Name of the column </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction
			''' @return </param>
			Public Overridable Function addColumnDouble(ByVal name As String, ByVal minAllowedValue As Double?, ByVal maxAllowedValue As Double?) As Builder
				Return addColumnDouble(name, minAllowedValue, maxAllowedValue, False, False)
			End Function

			''' <summary>
			''' Add a Double column with the specified restrictions
			''' </summary>
			''' <param name="name">            Name of the column </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			''' <param name="allowNaN">        If false: don't allow NaN values. If true: allow. </param>
			''' <param name="allowInfinite">   If false: don't allow infinite values. If true: allow </param>
			Public Overridable Function addColumnDouble(ByVal name As String, ByVal minAllowedValue As Double?, ByVal maxAllowedValue As Double?, ByVal allowNaN As Boolean, ByVal allowInfinite As Boolean) As Builder
				Return addColumn(New DoubleMetaData(name, minAllowedValue, maxAllowedValue, allowNaN, allowInfinite))
			End Function

			''' <summary>
			''' Add multiple Double columns with no restrictions on the allowable values of the columns (other than no NaN/Infinite)
			''' </summary>
			''' <param name="columnNames"> Names of the columns to add </param>
			Public Overridable Function addColumnsDouble(ParamArray ByVal columnNames() As String) As Builder
				For Each s As String In columnNames
					addColumnDouble(s)
				Next s
				Return Me
			End Function

			''' <summary>
			''' A convenience method for adding multiple Double columns.
			''' For example, to add columns "myDoubleCol_0", "myDoubleCol_1", "myDoubleCol_2", use
			''' {@code addColumnsDouble("myDoubleCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			Public Overridable Function addColumnsDouble(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer) As Builder
				Return addColumnsDouble(pattern, minIdxInclusive, maxIdxInclusive, Nothing, Nothing, False, False)
			End Function

			''' <summary>
			''' A convenience method for adding multiple Double columns, with additional restrictions that apply to all columns
			''' For example, to add columns "myDoubleCol_0", "myDoubleCol_1", "myDoubleCol_2", use
			''' {@code addColumnsDouble("myDoubleCol_%d",0,2,null,null,false,false)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			''' <param name="allowNaN">        If false: don't allow NaN values. If true: allow. </param>
			''' <param name="allowInfinite">   If false: don't allow infinite values. If true: allow </param>
			Public Overridable Function addColumnsDouble(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer, ByVal minAllowedValue As Double?, ByVal maxAllowedValue As Double?, ByVal allowNaN As Boolean, ByVal allowInfinite As Boolean) As Builder
				For i As Integer = minIdxInclusive To maxIdxInclusive
					addColumnDouble(String.format(pattern, i), minAllowedValue, maxAllowedValue, allowNaN, allowInfinite)
				Next i
				Return Me
			End Function

			''' <summary>
			''' Add an Integer column with no restrictions on the allowable values
			''' </summary>
			''' <param name="name"> Name of the column </param>
			Public Overridable Function addColumnInteger(ByVal name As String) As Builder
				Return addColumn(New IntegerMetaData(name))
			End Function

			''' <summary>
			''' Add an Integer column with the specified min/max allowable values
			''' </summary>
			''' <param name="name">            Name of the column </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			Public Overridable Function addColumnInteger(ByVal name As String, ByVal minAllowedValue As Integer?, ByVal maxAllowedValue As Integer?) As Builder
				Return addColumn(New IntegerMetaData(name, minAllowedValue, maxAllowedValue))
			End Function

			''' <summary>
			''' Add multiple Integer columns with no restrictions on the min/max allowable values
			''' </summary>
			''' <param name="names"> Names of the integer columns to add </param>
			Public Overridable Function addColumnsInteger(ParamArray ByVal names() As String) As Builder
				For Each s As String In names
					addColumnInteger(s)
				Next s
				Return Me
			End Function

			''' <summary>
			''' A convenience method for adding multiple Integer columns.
			''' For example, to add columns "myIntegerCol_0", "myIntegerCol_1", "myIntegerCol_2", use
			''' {@code addColumnsInteger("myIntegerCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			Public Overridable Function addColumnsInteger(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer) As Builder
				Return addColumnsInteger(pattern, minIdxInclusive, maxIdxInclusive, Nothing, Nothing)
			End Function

			''' <summary>
			''' A convenience method for adding multiple Integer columns.
			''' For example, to add columns "myIntegerCol_0", "myIntegerCol_1", "myIntegerCol_2", use
			''' {@code addColumnsInteger("myIntegerCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			Public Overridable Function addColumnsInteger(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer, ByVal minAllowedValue As Integer?, ByVal maxAllowedValue As Integer?) As Builder
				For i As Integer = minIdxInclusive To maxIdxInclusive
					addColumnInteger(String.format(pattern, i), minAllowedValue, maxAllowedValue)
				Next i
				Return Me
			End Function

			''' <summary>
			''' Add a Categorical column, with the specified state names
			''' </summary>
			''' <param name="name">       Name of the column </param>
			''' <param name="stateNames"> Names of the allowable states for this categorical column </param>
			Public Overridable Function addColumnCategorical(ByVal name As String, ParamArray ByVal stateNames() As String) As Builder
				Return addColumn(New CategoricalMetaData(name, stateNames))
			End Function

			''' <summary>
			''' Add a Categorical column, with the specified state names
			''' </summary>
			''' <param name="name">       Name of the column </param>
			''' <param name="stateNames"> Names of the allowable states for this categorical column </param>
			Public Overridable Function addColumnCategorical(ByVal name As String, ByVal stateNames As IList(Of String)) As Builder
				Return addColumn(New CategoricalMetaData(name, stateNames))
			End Function

			''' <summary>
			''' Add a Long column, with no restrictions on the min/max values
			''' </summary>
			''' <param name="name"> Name of the column </param>
			Public Overridable Function addColumnLong(ByVal name As String) As Builder
				Return addColumn(New LongMetaData(name))
			End Function

			''' <summary>
			''' Add a Long column with the specified min/max allowable values
			''' </summary>
			''' <param name="name">            Name of the column </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			Public Overridable Function addColumnLong(ByVal name As String, ByVal minAllowedValue As Long?, ByVal maxAllowedValue As Long?) As Builder
				Return addColumn(New LongMetaData(name, minAllowedValue, maxAllowedValue))
			End Function

			''' <summary>
			''' Add multiple Long columns, with no restrictions on the allowable values
			''' </summary>
			''' <param name="names"> Names of the Long columns to add </param>
			Public Overridable Function addColumnsLong(ParamArray ByVal names() As String) As Builder
				For Each s As String In names
					addColumnLong(s)
				Next s
				Return Me
			End Function

			''' <summary>
			''' A convenience method for adding multiple Long columns.
			''' For example, to add columns "myLongCol_0", "myLongCol_1", "myLongCol_2", use
			''' {@code addColumnsLong("myLongCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			Public Overridable Function addColumnsLong(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer) As Builder
				Return addColumnsLong(pattern, minIdxInclusive, maxIdxInclusive, Nothing, Nothing)
			End Function

			''' <summary>
			''' A convenience method for adding multiple Long columns.
			''' For example, to add columns "myLongCol_0", "myLongCol_1", "myLongCol_2", use
			''' {@code addColumnsLong("myLongCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			''' <param name="minAllowedValue"> Minimum allowed value (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedValue"> Maximum allowed value (inclusive). If null: no restriction </param>
			Public Overridable Function addColumnsLong(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer, ByVal minAllowedValue As Long?, ByVal maxAllowedValue As Long?) As Builder
				For i As Integer = minIdxInclusive To maxIdxInclusive
					addColumnLong(String.format(pattern, i), minAllowedValue, maxAllowedValue)
				Next i
				Return Me
			End Function


			''' <summary>
			''' Add a column
			''' </summary>
			''' <param name="metaData"> metadata for this column </param>
			Public Overridable Function addColumn(ByVal metaData As ColumnMetaData) As Builder
				columnMetaData.Add(metaData)
				Return Me
			End Function

			''' <summary>
			''' Add a String column with no restrictions on the allowable values.
			''' </summary>
			''' <param name="name"> Name of  the column </param>
			Public Overridable Function addColumnString(ByVal name As String) As Builder
				Return addColumn(New StringMetaData(name))
			End Function

			''' <summary>
			''' Add multiple String columns with no restrictions on the allowable values
			''' </summary>
			''' <param name="columnNames"> Names of the String columns to add </param>
			Public Overridable Function addColumnsString(ParamArray ByVal columnNames() As String) As Builder
				For Each s As String In columnNames
					addColumnString(s)
				Next s
				Return Me
			End Function

			''' <summary>
			''' Add a String column with the specified restrictions
			''' </summary>
			''' <param name="name">               Name of the column </param>
			''' <param name="regex">              Regex that the String must match in order to be considered valid. If null: no regex restriction </param>
			''' <param name="minAllowableLength"> Minimum allowable length for the String to be considered valid </param>
			''' <param name="maxAllowableLength"> Maximum allowable length for the String to be considered valid </param>
			Public Overridable Function addColumnString(ByVal name As String, ByVal regex As String, ByVal minAllowableLength As Integer?, ByVal maxAllowableLength As Integer?) As Builder
				Return addColumn(New StringMetaData(name, regex, minAllowableLength, maxAllowableLength))
			End Function

			''' <summary>
			''' A convenience method for adding multiple numbered String columns.
			''' For example, to add columns "myStringCol_0", "myStringCol_1", "myStringCol_2", use
			''' {@code addColumnsString("myStringCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">         Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive"> Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive"> Maximum column index to use (inclusive) </param>
			Public Overridable Function addColumnsString(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer) As Builder
				Return addColumnsString(pattern, minIdxInclusive, maxIdxInclusive, Nothing, Nothing, Nothing)
			End Function

			''' <summary>
			''' A convenience method for adding multiple numbered String columns.
			''' For example, to add columns "myStringCol_0", "myStringCol_1", "myStringCol_2", use
			''' {@code addColumnsString("myStringCol_%d",0,2)}
			''' </summary>
			''' <param name="pattern">          Pattern to use (via String.format). "%d" is replaced with column numbers </param>
			''' <param name="minIdxInclusive">  Minimum column index to use (inclusive) </param>
			''' <param name="maxIdxInclusive">  Maximum column index to use (inclusive) </param>
			''' <param name="regex">            Regex that the String must match in order to be considered valid. If null: no regex restriction </param>
			''' <param name="minAllowedLength"> Minimum allowed length of strings (inclusive). If null: no restriction </param>
			''' <param name="maxAllowedLength"> Maximum allowed length of strings (inclusive). If null: no restriction </param>
			Public Overridable Function addColumnsString(ByVal pattern As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer, ByVal regex As String, ByVal minAllowedLength As Integer?, ByVal maxAllowedLength As Integer?) As Builder
				For i As Integer = minIdxInclusive To maxIdxInclusive
					addColumnString(String.format(pattern, i), regex, minAllowedLength, maxAllowedLength)
				Next i
				Return Me
			End Function

			''' <summary>
			''' Add a Time column with no restrictions on the min/max allowable times
			''' <b>NOTE</b>: Time columns are represented by LONG (epoch millisecond) values. For time values in human-readable formats,
			''' use String columns + StringToTimeTransform
			''' </summary>
			''' <param name="columnName"> Name of the column </param>
			''' <param name="timeZone">   Time zone of the time column </param>
			Public Overridable Function addColumnTime(ByVal columnName As String, ByVal timeZone As TimeZone) As Builder
				Return addColumnTime(columnName, DateTimeZone.forTimeZone(timeZone))
			End Function

			''' <summary>
			''' Add a Time column with no restrictions on the min/max allowable times
			''' <b>NOTE</b>: Time columns are represented by LONG (epoch millisecond) values. For time values in human-readable formats,
			''' use String columns + StringToTimeTransform
			''' </summary>
			''' <param name="columnName"> Name of the column </param>
			''' <param name="timeZone">   Time zone of the time column </param>
			Public Overridable Function addColumnTime(ByVal columnName As String, ByVal timeZone As DateTimeZone) As Builder
				Return addColumnTime(columnName, timeZone, Nothing, Nothing)
			End Function

			''' <summary>
			''' Add a Time column with the specified restrictions
			''' <b>NOTE</b>: Time columns are represented by LONG (epoch millisecond) values. For time values in human-readable formats,
			''' use String columns + StringToTimeTransform
			''' </summary>
			''' <param name="columnName">    Name of the column </param>
			''' <param name="timeZone">      Time zone of the time column </param>
			''' <param name="minValidValue"> Minumum allowable time (in milliseconds). May be null. </param>
			''' <param name="maxValidValue"> Maximum allowable time (in milliseconds). May be null. </param>
			Public Overridable Function addColumnTime(ByVal columnName As String, ByVal timeZone As DateTimeZone, ByVal minValidValue As Long?, ByVal maxValidValue As Long?) As Builder
				addColumn(New TimeMetaData(columnName, timeZone, minValidValue, maxValidValue))
				Return Me
			End Function

			''' <summary>
			''' Add a NDArray column
			''' </summary>
			''' <param name="columnName"> Name of the column </param>
			''' <param name="shape">      shape of the NDArray column. Use -1 in entries to specify as "variable length" in that dimension </param>
			Public Overridable Function addColumnNDArray(ByVal columnName As String, ByVal shape() As Long) As Builder
				Return addColumn(New NDArrayMetaData(columnName, shape))
			End Function

			''' <summary>
			''' Add a boolean (binary true/false) column </summary>
			''' <param name="columnName"> Name of the new column </param>
			Public Overridable Function addColumnBoolean(ByVal columnName As String) As Builder
				Return addColumn(New BooleanMetaData(columnName))
			End Function

			''' <summary>
			''' Create the Schema
			''' </summary>
			Public Overridable Function build() As Schema
				Return New Schema(Me)
			End Function
		End Class

		''' <summary>
		''' Infers a schema based on the record.
		''' The column names are based on indexing. </summary>
		''' <param name="record"> the record to infer from </param>
		''' <returns> the infered schema </returns>
		Public Shared Function inferMultiple(ByVal record As IList(Of IList(Of Writable))) As Schema
			Return infer(record(0))
		End Function

		''' <summary>
		''' Infers a schema based on the record.
		''' The column names are based on indexing. </summary>
		''' <param name="record"> the record to infer from </param>
		''' <returns> the infered schema </returns>
		Public Shared Function infer(ByVal record As IList(Of Writable)) As Schema
			Dim builder As New Schema.Builder()
			For i As Integer = 0 To record.Count - 1
				If TypeOf record(i) Is DoubleWritable Then
					builder.addColumnDouble(i.ToString())
				ElseIf TypeOf record(i) Is IntWritable Then
					builder.addColumnInteger(i.ToString())
				ElseIf TypeOf record(i) Is LongWritable Then
					builder.addColumnLong(i.ToString())
				ElseIf TypeOf record(i) Is FloatWritable Then
					builder.addColumnFloat(i.ToString())
				ElseIf TypeOf record(i) Is Text Then
					builder.addColumnString(i.ToString())

				Else
					Throw New System.InvalidOperationException("Illegal writable for infering schema of type " & record(i).GetType().ToString() & " with record " & record)
				End If
			Next i

			Return builder.build()
		End Function

	End Class

End Namespace