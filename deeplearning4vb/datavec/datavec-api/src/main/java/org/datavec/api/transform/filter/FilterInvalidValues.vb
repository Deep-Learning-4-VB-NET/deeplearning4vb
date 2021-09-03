Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ToString = lombok.ToString
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports org.datavec.api.writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties

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

Namespace org.datavec.api.transform.filter


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(exclude = {"schema", "columnIdxs"}) @JsonIgnoreProperties({"schema", "columnIdxs"}) @Data @ToString(exclude = {"schema", "columnIdxs"}) public class FilterInvalidValues implements Filter
	<Serializable>
	Public Class FilterInvalidValues
		Implements Filter

		Private schema As Schema
		Private ReadOnly filterAnyInvalid As Boolean
		Private ReadOnly columnsToFilterIfInvalid() As String
		Private columnIdxs() As Integer

		''' <summary>
		''' Filter examples that have invalid values in ANY columns. </summary>
		Public Sub New()
			filterAnyInvalid = True
			columnsToFilterIfInvalid = Nothing
		End Sub

		''' <param name="columnsToFilterIfInvalid"> Columns to check for invalid values </param>
		Public Sub New(ParamArray ByVal columnsToFilterIfInvalid() As String)
			If columnsToFilterIfInvalid Is Nothing OrElse columnsToFilterIfInvalid.Length = 0 Then
				Throw New System.ArgumentException("Cannot filter 0/null columns: columns to filter on must be specified")
			End If
			Me.columnsToFilterIfInvalid = columnsToFilterIfInvalid
			filterAnyInvalid = False
		End Sub

		''' <summary>
		''' Get the output schema for this transformation, given an input schema
		''' </summary>
		''' <param name="inputSchema"> </param>
		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Return inputSchema
		End Function

		Public Overridable Property InputSchema Implements Filter.setInputSchema As Schema
			Set(ByVal schema As Schema)
				Me.schema = schema
				If Not filterAnyInvalid Then
					Me.columnIdxs = New Integer(columnsToFilterIfInvalid.Length - 1){}
					For i As Integer = 0 To columnsToFilterIfInvalid.Length - 1
						Me.columnIdxs(i) = schema.getIndexOfColumn(columnsToFilterIfInvalid(i))
					Next i
				End If
			End Set
			Get
				Return schema
			End Get
		End Property


		''' <param name="writables"> Example </param>
		''' <returns> true if example should be removed, false to keep </returns>
		Public Overridable Function removeExample(ByVal writables As Object) As Boolean Implements Filter.removeExample
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> row = (java.util.List<?>) writables;
			Dim row As IList(Of Object) = DirectCast(writables, IList(Of Object))
			If Not filterAnyInvalid Then
				'Filter only on specific columns
				For Each i As Integer In columnIdxs
					If filterColumn(row, i) Then
						Return True 'Remove if not valid
					End If

				Next i
			Else
				'Filter on ALL columns
				Dim nCols As Integer = schema.numColumns()
				For i As Integer = 0 To nCols - 1
					If filterColumn(row, i) Then
						Return True
					End If
				Next i
			End If
			Return False
		End Function

		Private Function filterColumn(Of T1)(ByVal row As IList(Of T1), ByVal i As Integer) As Boolean
			Dim meta As ColumnMetaData = schema.getMetaData(i)
			If TypeOf row(i) Is Single? Then
				If Not meta.isValid(New FloatWritable(CType(row(i), Single?))) Then
					Return True
				End If
			ElseIf TypeOf row(i) Is Double? Then
				If Not meta.isValid(New DoubleWritable(CType(row(i), Double?))) Then
					Return True
				End If
			ElseIf TypeOf row(i) Is String Then
				If Not meta.isValid(New Text(CStr(row(i)).ToString())) Then
					Return True
				End If
			ElseIf TypeOf row(i) Is Integer? Then
				If Not meta.isValid(New IntWritable(CType(row(i), Integer?))) Then
					Return True
				End If

			ElseIf TypeOf row(i) Is Long? Then
				If Not meta.isValid(New LongWritable(CType(row(i), Long?))) Then
					Return True
				End If
			ElseIf TypeOf row(i) Is Boolean? Then
				If Not meta.isValid(New BooleanWritable(CType(row(i), Boolean?))) Then
					Return True
				End If
			End If
			Return False
		End Function

		''' <param name="sequence"> sequence example </param>
		''' <returns> true if example should be removed, false to keep </returns>
		Public Overridable Function removeSequence(ByVal sequence As Object) As Boolean Implements Filter.removeSequence
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> seq = (java.util.List<?>) sequence;
			Dim seq As IList(Of Object) = DirectCast(sequence, IList(Of Object))
			'If _any_ of the values are invalid, remove the entire sequence
			For Each c As Object In seq
				If removeExample(c) Then
					Return True
				End If
			Next c
			Return False
		End Function

		Public Overridable Function removeExample(ByVal writables As IList(Of Writable)) As Boolean Implements Filter.removeExample
			If writables.Count <> schema.numColumns() Then
				Return True
			End If

			If Not filterAnyInvalid Then
				'Filter only on specific columns
				For Each i As Integer In columnIdxs
					Dim meta As ColumnMetaData = schema.getMetaData(i)
					If Not meta.isValid(writables(i)) Then
						Return True 'Remove if not valid
					End If
				Next i
			Else
				'Filter on ALL columns
				Dim nCols As Integer = schema.numColumns()
				For i As Integer = 0 To nCols - 1
					Dim meta As ColumnMetaData = schema.getMetaData(i)
					If Not meta.isValid(writables(i)) Then
						Return True 'Remove if not valid
					End If
				Next i
			End If
			Return False
		End Function

		Public Overridable Function removeSequence(ByVal sequence As IList(Of IList(Of Writable))) As Boolean Implements Filter.removeSequence
			'If _any_ of the values are invalid, remove the entire sequence
			For Each c As IList(Of Writable) In sequence
				If removeExample(c) Then
					Return True
				End If
			Next c
			Return False
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
			Return columnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String()
			Return CType(schema.getColumnNames(), List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String
			Return columnNames()(0)
		End Function
	End Class

End Namespace