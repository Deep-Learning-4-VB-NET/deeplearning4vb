Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
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

Namespace org.datavec.api.transform.transform.categorical


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"inputSchema", "columnIdx", "stateNames", "statesMap"}) public class CategoricalToIntegerTransform extends org.datavec.api.transform.transform.BaseTransform
	<Serializable>
	Public Class CategoricalToIntegerTransform
		Inherits BaseTransform

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private columnName_Conflict As String
		Private columnIdx As Integer = -1
		Private stateNames As IList(Of String)
		Private statesMap As IDictionary(Of String, Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CategoricalToIntegerTransform(@JsonProperty("columnName") String columnName)
		Public Sub New(ByVal columnName As String)
			Me.columnName_Conflict = columnName
		End Sub

		Public Overrides WriteOnly Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				MyBase.InputSchema = inputSchema
    
				columnIdx = inputSchema.getIndexOfColumn(columnName_Conflict)
				Dim meta As ColumnMetaData = inputSchema.getMetaData(columnName_Conflict)
				If Not (TypeOf meta Is CategoricalMetaData) Then
					Throw New System.InvalidOperationException("Cannot convert column """ & columnName_Conflict & """ from categorical to one-hot: column is not categorical (is: " & meta.ColumnType & ")")
				End If
				Me.stateNames = DirectCast(meta, CategoricalMetaData).getStateNames()
    
				Me.statesMap = New Dictionary(Of String, Integer)(stateNames.Count)
				For i As Integer = 0 To stateNames.Count - 1
					Me.statesMap(stateNames(i)) = i
				Next i
			End Set
		End Property

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
					'Convert this to integer
					Dim nClasses As Integer = stateNames.Count
					newMeta.Add(New IntegerMetaData(t.Name, 0, nClasses - 1))
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

			Dim n As Integer = stateNames.Count
			Dim [out] As IList(Of Writable) = New List(Of Writable)(writables.Count + n)

			Dim i As Integer = 0
			For Each w As Writable In writables
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == idx)
				If i = idx Then
						i += 1
					'Do conversion
					Dim str As String = w.ToString()
					Dim classIdx As Integer? = statesMap(str)
					If classIdx Is Nothing Then
						Throw New System.InvalidOperationException("Cannot convert categorical value to integer value: input value (""" & str & """) is not in the list of known categories (state names/categories: " & stateNames & ")")
					End If
					[out].Add(New IntWritable(classIdx))
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
			Dim value As String = input.ToString()
			'Do conversion
			Dim classIdx As Integer? = statesMap(value)
			If classIdx Is Nothing Then
				Throw New System.InvalidOperationException("Cannot convert categorical value to integer value: input value (""" & value & """) is not in the list of known categories (state names/categories: " & stateNames & ")")
			End If
			Return classIdx
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
			Return Nothing
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then
				Return True
			End If
			If Not (TypeOf o Is CategoricalToIntegerTransform) Then
				Return False
			End If

			Dim o2 As CategoricalToIntegerTransform = DirectCast(o, CategoricalToIntegerTransform)

			If columnName_Conflict Is Nothing Then
				Return o2.columnName_Conflict Is Nothing
			Else
				Return columnName_Conflict.Equals(o2.columnName_Conflict)
			End If
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return columnName_Conflict.GetHashCode()
		End Function

		Protected Friend Overridable Function canEqual(ByVal other As Object) As Boolean
			Return TypeOf other Is CategoricalToIntegerTransform
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overrides Function outputColumnName() As String
			Return columnName_Conflict
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overrides Function outputColumnNames() As String()
			Return New String() {columnName()}
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