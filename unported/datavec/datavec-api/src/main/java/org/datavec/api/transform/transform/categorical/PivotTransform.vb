Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseTransform = org.datavec.api.transform.transform.BaseTransform
Imports org.datavec.api.writable

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
'ORIGINAL LINE: @Data public class PivotTransform extends org.datavec.api.transform.transform.BaseTransform
	<Serializable>
	Public Class PivotTransform
		Inherits BaseTransform

		Private ReadOnly keyColumn As String
		Private ReadOnly valueColumn As String
		Private defaultValue As Writable

		''' 
		''' <param name="keyColumnName">   Key column to expand </param>
		''' <param name="valueColumnName"> Name of the column that contains the value </param>
		Public Sub New(ByVal keyColumnName As String, ByVal valueColumnName As String)
			Me.New(keyColumnName, valueColumnName, Nothing)
		End Sub

		''' 
		''' <param name="keyColumnName">   Key column to expand </param>
		''' <param name="valueColumnName"> Name of the column that contains the value </param>
		''' <param name="defaultValue">    The default value to use in expanded columns. </param>
		Public Sub New(ByVal keyColumnName As String, ByVal valueColumnName As String, ByVal defaultValue As Writable)
			Me.keyColumn = keyColumnName
			Me.valueColumn = valueColumnName
			Me.defaultValue = defaultValue
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			If Not inputSchema.hasColumn(keyColumn) OrElse Not inputSchema.hasColumn(valueColumn) Then
				Throw New System.NotSupportedException("Key or value column not found: " & keyColumn & ", " & valueColumn & " in " & inputSchema.getColumnNames())
			End If

			Dim origNames As IList(Of String) = inputSchema.getColumnNames()
			Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()

			Dim i As Integer = 0
			Dim namesIter As IEnumerator(Of String) = origNames.GetEnumerator()
			Dim typesIter As IEnumerator(Of ColumnMetaData) = origMeta.GetEnumerator()

			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.numColumns())

			Dim idxKey As Integer = inputSchema.getIndexOfColumn(keyColumn)
			Dim idxValue As Integer = inputSchema.getIndexOfColumn(valueColumn)

			Dim valueMeta As ColumnMetaData = inputSchema.getMetaData(idxValue)

			Do While namesIter.MoveNext()
				Dim s As String = namesIter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim t As ColumnMetaData = typesIter.next()

				If i = idxKey Then
					'Convert this to a set of separate columns
					Dim stateNames As IList(Of String) = DirectCast(inputSchema.getMetaData(idxKey), CategoricalMetaData).getStateNames()
					For Each stateName As String In stateNames
						Dim newName As String = s & "[" & stateName & "]"

						Dim newValueMeta As ColumnMetaData = valueMeta.clone()
						newValueMeta.Name = newName

						newMeta.Add(newValueMeta)
					Next stateName
				ElseIf i = idxValue Then
					i += 1
					Continue Do 'Skip column
				Else
					newMeta.Add(t)
				End If
				i += 1
			Loop

			'Infer the default value if necessary
			If defaultValue Is Nothing Then
				Select Case valueMeta.ColumnType
					Case String
						defaultValue = New Text("")
					Case Integer?
						defaultValue = New IntWritable(0)
					Case Long?
						defaultValue = New LongWritable(0)
					Case Double?
						defaultValue = New DoubleWritable(0.0)
					Case Single?
						defaultValue = New FloatWritable(0.0f)
					Case Categorical
						defaultValue = New NullWritable()
					Case Time
						defaultValue = New LongWritable(0)
					Case Bytes
						Throw New System.NotSupportedException("Cannot infer default value for bytes")
					Case Boolean?
						defaultValue = New Text("false")
					Case Else
						Throw New System.NotSupportedException("Cannot infer default value for " & valueMeta.ColumnType)
				End Select
			End If

			Return inputSchema.newSchema(newMeta)
		End Function


		Public Overrides Function outputColumnName() As String
			Throw New System.NotSupportedException("Output column name will be more than 1")
		End Function

		Public Overrides Function outputColumnNames() As String()
			Dim l As IList(Of String) = DirectCast(inputSchema_Conflict.getMetaData(keyColumn), CategoricalMetaData).getStateNames()
			Return CType(l, List(Of String)).ToArray()
		End Function

		Public Overrides Function columnNames() As String()
			Return New String() {keyColumn, valueColumn}
		End Function

		Public Overrides Function columnName() As String
			Throw New System.NotSupportedException("Multiple input columns")
		End Function

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & ToString())
			End If

			Dim idxKey As Integer = inputSchema_Conflict.getIndexOfColumn(keyColumn)
			Dim idxValue As Integer = inputSchema_Conflict.getIndexOfColumn(valueColumn)
			Dim stateNames As IList(Of String) = DirectCast(inputSchema_Conflict.getMetaData(idxKey), CategoricalMetaData).getStateNames()

			Dim i As Integer = 0
			Dim [out] As IList(Of Writable) = New List(Of Writable)()
			For Each w As Writable In writables

				If i = idxKey Then
					'Do conversion
					Dim str As String = w.ToString()
					Dim stateIdx As Integer = stateNames.IndexOf(str)

					If stateIdx < 0 Then
						Throw New Exception("Unknown state (index not found): " & str)
					End If
					For j As Integer = 0 To stateNames.Count - 1
						If j = stateIdx Then
							[out].Add(writables(idxValue))
						Else
							[out].Add(defaultValue)
						End If
					Next j
				ElseIf i = idxValue Then
					i += 1
					Continue For
				Else
					'No change to this column
					[out].Add(w)
				End If
				i += 1
			Next w
			Return [out]
		End Function

		Public Overrides Function map(ByVal input As Object) As Object
			Dim l As IList(Of Writable) = DirectCast(input, IList(Of Writable))
			Dim k As Writable = l(0)
			Dim v As Writable = l(1)

			Dim idxKey As Integer = inputSchema_Conflict.getIndexOfColumn(keyColumn)
			Dim stateNames As IList(Of String) = DirectCast(inputSchema_Conflict.getMetaData(idxKey), CategoricalMetaData).getStateNames()
			Dim n As Integer = stateNames.Count

			Dim position As Integer = stateNames.IndexOf(k.ToString())

			Dim [out] As IList(Of Writable) = New List(Of Writable)()
			For j As Integer = 0 To n - 1
				If j = position Then
					[out].Add(v)
				Else
					[out].Add(defaultValue)
				End If
			Next j
			Return [out]
		End Function

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
	End Class

End Namespace