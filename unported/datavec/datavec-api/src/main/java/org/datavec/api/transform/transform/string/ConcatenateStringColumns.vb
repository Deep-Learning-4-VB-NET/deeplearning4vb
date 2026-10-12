Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Linq
Imports Data = lombok.Data
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports ColumnType = org.datavec.api.transform.ColumnType
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
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema"}) @Data public class ConcatenateStringColumns extends org.datavec.api.transform.transform.BaseTransform implements org.datavec.api.transform.ColumnOp
	<Serializable>
	Public Class ConcatenateStringColumns
		Inherits BaseTransform
		Implements ColumnOp

		Private ReadOnly newColumnName As String
		Private ReadOnly delimiter As String
		Private ReadOnly columnsToConcatenate As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shadows inputSchema_Conflict As Schema

		''' <param name="columnsToConcatenate"> A partial or complete order of the columns in the output </param>
		Public Sub New(ByVal newColumnName As String, ByVal delimiter As String, ParamArray ByVal columnsToConcatenate() As String)
			Me.New(newColumnName, delimiter, Arrays.asList(columnsToConcatenate))
		End Sub

		''' <param name="columnsToConcatenate"> A partial or complete order of the columns in the output </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConcatenateStringColumns(@JsonProperty("newColumnName") String newColumnName, @JsonProperty("delimiter") String delimiter, @JsonProperty("columnsToConcatenate") java.util.List<String> columnsToConcatenate)
		Public Sub New(ByVal newColumnName As String, ByVal delimiter As String, ByVal columnsToConcatenate As IList(Of String))
			Me.newColumnName = newColumnName
			Me.delimiter = delimiter
			Me.columnsToConcatenate = columnsToConcatenate
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			For Each s As String In columnsToConcatenate
				If Not inputSchema.hasColumn(s) Then
					Throw New System.InvalidOperationException("Input schema does not contain column with name """ & s & """")
				End If
			Next s

			Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())

			Dim newColMeta As ColumnMetaData = ColumnType.String.newColumnMetaData(newColumnName)
			outMeta.Add(newColMeta)
			Return inputSchema.newSchema(outMeta)
		End Function

		Public Overrides Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				For Each s As String In columnsToConcatenate
					If Not inputSchema.hasColumn(s) Then
						Throw New System.InvalidOperationException("Input schema does not contain column with name """ & s & """")
					End If
				Next s
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			Dim newColumnText As New StringBuilder()
			Dim [out] As IList(Of Writable) = New List(Of Writable)(writables)
			Dim i As Integer = 0
			For Each columnName As String In columnsToConcatenate
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ > 0)
				If i > 0 Then
						i += 1
					newColumnText.Append(delimiter)
					Else
						i += 1
					End If
				Dim columnIdx As Integer = inputSchema_Conflict.getIndexOfColumn(columnName)
				newColumnText.Append(writables(columnIdx))
			Next columnName
			[out].Add(New Text(newColumnText.ToString()))
			Return [out]
		End Function

		Public Overridable Overloads Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable))
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
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
		Public Overrides Function map(ByVal input As Object) As Object
			Throw New System.NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")

		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Throw New System.NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim o2 As ConcatenateStringColumns = DirectCast(o, ConcatenateStringColumns)
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: return delimiter.equals(o2.delimiter) && columnsToConcatenate.equals(o2.columnsToConcatenate);
			Return delimiter.Equals(o2.delimiter) AndAlso columnsToConcatenate.SequenceEqual(o2.columnsToConcatenate)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = delimiter.GetHashCode()
			result = 31 * result + columnsToConcatenate.GetHashCode()
			Return result
		End Function

		Public Overrides Function ToString() As String
			Return "ConcatenateStringColumns(delimiters=" & delimiter & " columnsToConcatenate=" & columnsToConcatenate & ")"

		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overrides Function outputColumnName() As String Implements ColumnOp.outputColumnName
			Return newColumnName
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overrides Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
			Return New String() {newColumnName}
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnNames() As String() Implements ColumnOp.columnNames
			Return CType(columnsToConcatenate, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnName() As String Implements ColumnOp.columnName
			Return columnsToConcatenate(0)
		End Function
	End Class

End Namespace