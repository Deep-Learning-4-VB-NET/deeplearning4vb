Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseTransform = org.datavec.api.transform.transform.BaseTransform
Imports IntWritable = org.datavec.api.writable.IntWritable
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

Namespace org.datavec.api.transform.transform.integer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(exclude = {"columnIdx"}, callSuper = false) @JsonIgnoreProperties({"inputSchema", "columnIdx", "stateNames", "statesMap"}) public class IntegerToOneHotTransform extends org.datavec.api.transform.transform.BaseTransform
	<Serializable>
	Public Class IntegerToOneHotTransform
		Inherits BaseTransform

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private columnName_Conflict As String
		Private minValue As Integer
		Private maxValue As Integer
		Private columnIdx As Integer = -1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public IntegerToOneHotTransform(@JsonProperty("columnName") String columnName, @JsonProperty("minValue") int minValue, @JsonProperty("maxValue") int maxValue)
		Public Sub New(ByVal columnName As String, ByVal minValue As Integer, ByVal maxValue As Integer)
			Me.columnName_Conflict = columnName
			Me.minValue = minValue
			Me.maxValue = maxValue
		End Sub

		Public Overrides WriteOnly Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				MyBase.InputSchema = inputSchema
    
				columnIdx = inputSchema.getIndexOfColumn(columnName_Conflict)
				Dim meta As ColumnMetaData = inputSchema.getMetaData(columnName_Conflict)
				If Not (TypeOf meta Is IntegerMetaData) Then
					Throw New System.InvalidOperationException("Cannot convert column """ & columnName_Conflict & """ from integer to one-hot: column is not integer (is: " & meta.ColumnType & ")")
				End If
			End Set
		End Property

		Public Overrides Function ToString() As String
			Return "CategoricalToOneHotTransform(columnName=""" & columnName_Conflict & """)"

		End Function

		Public Overrides Function transform(ByVal schema As Schema) As Schema
			Dim origNames As IList(Of String) = schema.getColumnNames()
			Dim origMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()

			Dim i As Integer = 0
			Dim namesIter As IEnumerator(Of String) = origNames.GetEnumerator()
			Dim typesIter As IEnumerator(Of ColumnMetaData) = origMeta.GetEnumerator()

			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(schema.numColumns())

			Do While namesIter.MoveNext()
				Dim s As String = namesIter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim t As ColumnMetaData = typesIter.next()

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == columnIdx)
				If i = columnIdx Then
						i += 1
					'Convert this to one-hot:
					For x As Integer = minValue To maxValue
						Dim newName As String = s & "[" & x & "]"
						newMeta.Add(New IntegerMetaData(newName, 0, 1))
					Next x
				Else
						i += 1
					newMeta.Add(t)
				End If
			Loop

			Return schema.newSchema(newMeta)
		End Function

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & ToString())
			End If
			Dim idx As Integer = getColumnIdx()

			Dim n As Integer = maxValue - minValue + 1
			Dim [out] As IList(Of Writable) = New List(Of Writable)(writables.Count + n)

			Dim i As Integer = 0
			For Each w As Writable In writables

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == idx)
				If i = idx Then
						i += 1
					Dim currValue As Integer = w.toInt()
					If currValue < minValue OrElse currValue > maxValue Then
						Throw New System.InvalidOperationException("Invalid value: integer value (" & currValue & ") is outside of " & "valid range: must be between " & minValue & " and " & maxValue & " inclusive")
					End If

					For j As Integer = minValue To maxValue
						If j = currValue Then
							[out].Add(New IntWritable(1))
						Else
							[out].Add(New IntWritable(0))
						End If
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
			Dim currValue As Integer = DirectCast(input, Number).intValue()
			If currValue < minValue OrElse currValue > maxValue Then
				Throw New System.InvalidOperationException("Invalid value: integer value (" & currValue & ") is outside of " & "valid range: must be between " & minValue & " and " & maxValue & " inclusive")
			End If

			Dim oneHot As IList(Of Integer) = New List(Of Integer)()
			For j As Integer = minValue To maxValue
				If j = currValue Then
					oneHot.Add(1)
				Else
					oneHot.Add(0)
				End If
			Next j
			Return oneHot
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> values = (java.util.List<?>) sequence;
			Dim values As IList(Of Object) = DirectCast(sequence, IList(Of Object))
			Dim ret As IList(Of IList(Of Integer)) = New List(Of IList(Of Integer))()
			For Each obj As Object In values
				ret.Add(DirectCast(map(obj), IList(Of Integer)))
			Next obj
			Return ret
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overrides Function outputColumnName() As String
			Throw New System.NotSupportedException("Output column name will be more than 1")
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overrides Function outputColumnNames() As String()
			Dim l As IList(Of String) = transform(inputSchema_Conflict).getColumnNames()
			Return CType(l, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnNames() As String()
			Return New String() {columnName_Conflict}
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