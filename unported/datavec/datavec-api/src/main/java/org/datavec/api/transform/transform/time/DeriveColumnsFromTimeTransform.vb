Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports StringMetaData = org.datavec.api.transform.metadata.StringMetaData
Imports TimeMetaData = org.datavec.api.transform.metadata.TimeMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports DateTimeFieldTypeDeserializer = org.datavec.api.util.jackson.DateTimeFieldTypeDeserializer
Imports DateTimeFieldTypeSerializer = org.datavec.api.util.jackson.DateTimeFieldTypeSerializer
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports DateTime = org.joda.time.DateTime
Imports DateTimeFieldType = org.joda.time.DateTimeFieldType
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports DateTimeFormat = org.joda.time.format.DateTimeFormat
Imports DateTimeFormatter = org.joda.time.format.DateTimeFormatter
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize

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

Namespace org.datavec.api.transform.transform.time


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "insertAfterIdx", "deriveFromIdx"}) @EqualsAndHashCode(exclude = {"inputSchema", "insertAfterIdx", "deriveFromIdx"}) @Data public class DeriveColumnsFromTimeTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class DeriveColumnsFromTimeTransform
		Implements Transform

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly columnName_Conflict As String
		Private ReadOnly insertAfter As String
		Private inputTimeZone As DateTimeZone
		Private ReadOnly derivedColumns As IList(Of DerivedColumn)
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema
		Private insertAfterIdx As Integer = -1
		Private deriveFromIdx As Integer = -1


		Private Sub New(ByVal builder As Builder)
			Me.derivedColumns = builder.derivedColumns
			Me.columnName_Conflict = builder.columnName
			Me.insertAfter = builder.insertAfter_Conflict
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeriveColumnsFromTimeTransform(@JsonProperty("columnName") String columnName, @JsonProperty("insertAfter") String insertAfter, @JsonProperty("inputTimeZone") org.joda.time.DateTimeZone inputTimeZone, @JsonProperty("derivedColumns") java.util.List<DerivedColumn> derivedColumns)
		Public Sub New(ByVal columnName As String, ByVal insertAfter As String, ByVal inputTimeZone As DateTimeZone, ByVal derivedColumns As IList(Of DerivedColumn))
			Me.columnName_Conflict = columnName
			Me.insertAfter = insertAfter
			Me.inputTimeZone = inputTimeZone
			Me.derivedColumns = derivedColumns
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count + derivedColumns.Count)

			Dim oldNames As IList(Of String) = inputSchema.getColumnNames()

			For i As Integer = 0 To oldMeta.Count - 1
				Dim current As String = oldNames(i)
				newMeta.Add(oldMeta(i))

				If insertAfter.Equals(current) Then
					'Insert the derived columns here
					For Each d As DerivedColumn In derivedColumns
						Select Case d.columnType.innerEnumValue
							Case ColumnType.InnerEnum.String
								newMeta.Add(New StringMetaData(d.columnName))
							Case Integer?
								newMeta.Add(New IntegerMetaData(d.columnName)) 'TODO: ranges... if it's a day, we know it must be 1 to 31, etc...
							Case Else
								Throw New System.InvalidOperationException("Unexpected column type: " & d.columnType)
						End Select
					Next d
				End If
			Next i

			Return inputSchema.newSchema(newMeta)
		End Function

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				insertAfterIdx = inputSchema.getColumnNames().IndexOf(insertAfter)
				If insertAfterIdx = -1 Then
					Throw New System.InvalidOperationException("Invalid schema/insert after column: input schema does not contain column """ & insertAfter & """")
				End If
    
				deriveFromIdx = inputSchema.getColumnNames().IndexOf(columnName_Conflict)
				If deriveFromIdx = -1 Then
					Throw New System.InvalidOperationException("Invalid source column: input schema does not contain column """ & columnName_Conflict & """")
				End If
    
				Me.inputSchema_Conflict = inputSchema
    
				If Not (TypeOf inputSchema.getMetaData(columnName_Conflict) Is TimeMetaData) Then
					Throw New System.InvalidOperationException("Invalid state: input column """ & columnName_Conflict & """ is not a time column. Is: " & inputSchema.getMetaData(columnName_Conflict))
				End If
				Dim meta As TimeMetaData = DirectCast(inputSchema.getMetaData(columnName_Conflict), TimeMetaData)
				inputTimeZone = meta.getTimeZone()
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & ToString())
			End If

			Dim i As Integer = 0
			Dim source As Writable = writables(deriveFromIdx)
			Dim list As IList(Of Writable) = New List(Of Writable)(writables.Count + derivedColumns.Count)
			For Each w As Writable In writables
				list.Add(w)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == insertAfterIdx)
				If i = insertAfterIdx Then
						i += 1
					For Each d As DerivedColumn In derivedColumns
						Select Case d.columnType.innerEnumValue
							Case ColumnType.InnerEnum.String
								list.Add(New Text(d.dateTimeFormatter.print(source.toLong())))
							Case Integer?
								Dim dt As New DateTime(source.toLong(), inputTimeZone)
								list.Add(New IntWritable(dt.get(d.fieldType)))
							Case Else
								Throw New System.InvalidOperationException("Unexpected column type: " & d.columnType)
						End Select
					Next d
					Else
						i += 1
					End If
			Next w
			Return list
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(sequence.Count)
			For Each [step] As IList(Of Writable) In sequence
				[out].Add(map([step]))
			Next [step]
			Return [out]
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Dim ret As IList(Of Object) = New List(Of Object)()
			Dim l As Long? = DirectCast(input, Long?)
			For Each d As DerivedColumn In derivedColumns
				Select Case d.columnType.innerEnumValue
					Case ColumnType.InnerEnum.String
						ret.Add(d.dateTimeFormatter.print(l))
					Case Integer?
						Dim dt As New DateTime(l, inputTimeZone)
						ret.Add(dt.get(d.fieldType))
					Case Else
						Throw New System.InvalidOperationException("Unexpected column type: " & d.columnType)
				End Select
			Next d

			Return ret
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Dim longs As IList(Of Long) = DirectCast(sequence, IList(Of Long))
			Dim ret As IList(Of IList(Of Object)) = New List(Of IList(Of Object))()
			For Each l As Long? In longs
				ret.Add(CType(map(l), IList(Of Object)))
			Next l
			Return ret
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder()
			sb.Append("DeriveColumnsFromTimeTransform(timeColumn=""").Append(columnName_Conflict).Append(""",insertAfter=""").Append(insertAfter).Append(""",derivedColumns=(")

			Dim first As Boolean = True
			For Each d As DerivedColumn In derivedColumns
				If Not first Then
					sb.Append(",")
				End If
				sb.Append(d)
				first = False
			Next d

			sb.Append("))")

			Return sb.ToString()
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String
			Return outputColumnNames()(0)
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String()
			Dim ret(derivedColumns.Count - 1) As String
			For i As Integer = 0 To ret.Length - 1
				ret(i) = derivedColumns(i).columnName
			Next i
			Return ret
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String()
			Return New String() {columnName()}
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String
			Return columnName_Conflict
		End Function

		Public Class Builder

			Friend ReadOnly columnName As String
'JAVA TO VB CONVERTER NOTE: The field insertAfter was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend insertAfter_Conflict As String
			Friend ReadOnly derivedColumns As IList(Of DerivedColumn) = New List(Of DerivedColumn)()


			''' <param name="timeColumnName"> The name of the time column from which to derive the new values </param>
			Public Sub New(ByVal timeColumnName As String)
				Me.columnName = timeColumnName
				Me.insertAfter_Conflict = timeColumnName
			End Sub

			''' <summary>
			''' Where should the new columns be inserted?
			''' By default, they will be inserted after the source column
			''' </summary>
			''' <param name="columnName"> Name of the column to insert the derived columns after </param>
			Public Overridable Function insertAfter(ByVal columnName As String) As Builder
				Me.insertAfter_Conflict = columnName
				Return Me
			End Function

			''' <summary>
			''' Add a String column (for example, human readable format), derived from the time
			''' </summary>
			''' <param name="columnName"> Name of the new/derived column </param>
			''' <param name="format">     Joda time format, as per <a href="http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html">http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html</a> </param>
			''' <param name="timeZone">   Timezone to use for formatting </param>
			Public Overridable Function addStringDerivedColumn(ByVal columnName As String, ByVal format As String, ByVal timeZone As DateTimeZone) As Builder
				derivedColumns.Add(New DerivedColumn(columnName, ColumnType.String, format, timeZone, Nothing))
				Return Me
			End Function

			''' <summary>
			''' Add an integer derived column - for example, the hour of day, etc. Uses timezone from the time column metadata
			''' </summary>
			''' <param name="columnName"> Name of the column </param>
			''' <param name="type">       Type of field (for example, DateTimeFieldType.hourOfDay() etc) </param>
			Public Overridable Function addIntegerDerivedColumn(ByVal columnName As String, ByVal type As DateTimeFieldType) As Builder
				derivedColumns.Add(New DerivedColumn(columnName, ColumnType.Integer, Nothing, Nothing, type))
				Return Me
			End Function

			''' <summary>
			''' Create the transform instance
			''' </summary>
			Public Overridable Function build() As DeriveColumnsFromTimeTransform
				Return New DeriveColumnsFromTimeTransform(Me)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @EqualsAndHashCode(exclude = "dateTimeFormatter") @Data @JsonIgnoreProperties({"dateTimeFormatter"}) public static class DerivedColumn implements java.io.Serializable
		<Serializable>
		Public Class DerivedColumn
			Friend ReadOnly columnName As String
			Friend ReadOnly columnType As ColumnType
			Friend ReadOnly format As String
			Friend ReadOnly dateTimeZone As DateTimeZone
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.datavec.api.util.jackson.DateTimeFieldTypeSerializer.class) @JsonDeserialize(using = org.datavec.api.util.jackson.DateTimeFieldTypeDeserializer.class) private final org.joda.time.DateTimeFieldType fieldType;
			Friend ReadOnly fieldType As DateTimeFieldType
			<NonSerialized>
			Friend dateTimeFormatter As DateTimeFormatter

			'        public DerivedColumn(String columnName, ColumnType columnType, String format, DateTimeZone dateTimeZone, DateTimeFieldType fieldType) {
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DerivedColumn(@JsonProperty("columnName") String columnName, @JsonProperty("columnType") org.datavec.api.transform.ColumnType columnType, @JsonProperty("format") String format, @JsonProperty("dateTimeZone") org.joda.time.DateTimeZone dateTimeZone, @JsonProperty("fieldType") org.joda.time.DateTimeFieldType fieldType)
			Public Sub New(ByVal columnName As String, ByVal columnType As ColumnType, ByVal format As String, ByVal dateTimeZone As DateTimeZone, ByVal fieldType As DateTimeFieldType)
				Me.columnName = columnName
				Me.columnType = columnType
				Me.format = format
				Me.dateTimeZone = dateTimeZone
				Me.fieldType = fieldType
				If format IsNot Nothing Then
					dateTimeFormatter = DateTimeFormat.forPattern(Me.format).withZone(dateTimeZone)
				End If
			End Sub

			Public Overrides Function ToString() As String
				Return "(name=" & columnName & ",type=" & columnType & ",derived=" & (If(format IsNot Nothing, format, fieldType)) & ")"
			End Function

			'Custom serialization methods, because Joda Time doesn't allow DateTimeFormatter objects to be serialized :(
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
			Friend Overridable Sub writeObject(ByVal [out] As ObjectOutputStream)
				[out].defaultWriteObject()
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws IOException, ClassNotFoundException
			Friend Overridable Sub readObject(ByVal [in] As ObjectInputStream)
				[in].defaultReadObject()
				If format IsNot Nothing Then
					dateTimeFormatter = DateTimeFormat.forPattern(format).withZone(dateTimeZone)
				End If
			End Sub
		End Class
	End Class

End Namespace