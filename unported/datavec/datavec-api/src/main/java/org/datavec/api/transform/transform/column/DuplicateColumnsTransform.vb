Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
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

Namespace org.datavec.api.transform.transform.column


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"columnsToDuplicateSet", "columnIndexesToDuplicateSet", "inputSchema"}) @Data public class DuplicateColumnsTransform implements org.datavec.api.transform.Transform, org.datavec.api.transform.ColumnOp
	<Serializable>
	Public Class DuplicateColumnsTransform
		Implements Transform, ColumnOp

		Private ReadOnly columnsToDuplicate As IList(Of String)
		Private ReadOnly newColumnNames As IList(Of String)
		Private ReadOnly columnsToDuplicateSet As ISet(Of String)
		Private ReadOnly columnIndexesToDuplicateSet As ISet(Of Integer)
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		''' <param name="columnsToDuplicate"> List of columns to duplicate </param>
		''' <param name="newColumnNames">     List of names for the new (duplicate) columns </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DuplicateColumnsTransform(@JsonProperty("columnsToDuplicate") java.util.List<String> columnsToDuplicate, @JsonProperty("newColumnNames") java.util.List<String> newColumnNames)
		Public Sub New(ByVal columnsToDuplicate As IList(Of String), ByVal newColumnNames As IList(Of String))
			If columnsToDuplicate Is Nothing OrElse newColumnNames Is Nothing Then
				Throw New System.ArgumentException("Columns/names cannot be null")
			End If
			If columnsToDuplicate.Count <> newColumnNames.Count Then
				Throw New System.ArgumentException("Invalid input: columns to duplicate and the new names must have equal lengths")
			End If
			Me.columnsToDuplicate = columnsToDuplicate
			Me.newColumnNames = newColumnNames
			Me.columnsToDuplicateSet = New HashSet(Of String)(columnsToDuplicate)
			Me.columnIndexesToDuplicateSet = New HashSet(Of Integer)()
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count + newColumnNames.Count)

			Dim oldNames As IList(Of String) = inputSchema.getColumnNames()

			Dim dupCount As Integer = 0
			For i As Integer = 0 To oldMeta.Count - 1
				Dim current As String = oldNames(i)
				newMeta.Add(oldMeta(i))

				If columnsToDuplicateSet.Contains(current) Then
					'Duplicate the current columnName, and place it after...
					Dim dupName As String = newColumnNames(dupCount)
					Dim m As ColumnMetaData = oldMeta(i).clone()
					m.Name = dupName
					newMeta.Add(m)
					dupCount += 1
				End If
			Next i

			Return inputSchema.newSchema(newMeta)
		End Function

		Public Overridable Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				columnIndexesToDuplicateSet.Clear()
    
				Dim schemaColumnNames As IList(Of String) = inputSchema.getColumnNames()
				For Each s As String In columnsToDuplicate
					Dim idx As Integer = schemaColumnNames.IndexOf(s)
					If idx = -1 Then
						Throw New System.InvalidOperationException("Invalid state: column to duplicate """ & s & """ does not appear " & "in input schema")
					End If
					columnIndexesToDuplicateSet.Add(idx)
				Next s
    
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & ToString())
			End If
			Dim [out] As IList(Of Writable) = New List(Of Writable)(writables.Count + columnsToDuplicate.Count)
			Dim i As Integer = 0
			For Each w As Writable In writables
				[out].Add(w)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (columnIndexesToDuplicateSet.contains(i++))
				If columnIndexesToDuplicateSet.Contains(i) Then
						i += 1
					[out].Add(w) 'TODO safter to copy here...
					Else
						i += 1
					End If
			Next w
			Return [out]
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(sequence.Count)
			For Each l As IList(Of Writable) In sequence
				[out].Add(map(l))
			Next l
			Return [out]
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Return input
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Return sequence
		End Function

		Public Overrides Function ToString() As String
			Return "DuplicateColumnsTransform(toDuplicate=" & columnsToDuplicate & ",newNames=" & newColumnNames & ")"
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim o2 As DuplicateColumnsTransform = DirectCast(o, DuplicateColumnsTransform)

'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (!columnsToDuplicate.equals(o2.columnsToDuplicate))
			If Not columnsToDuplicate.SequenceEqual(o2.columnsToDuplicate) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: return newColumnNames.equals(o2.newColumnNames);
			Return newColumnNames.SequenceEqual(o2.newColumnNames)

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = columnsToDuplicate.GetHashCode()
			result = 31 * result + newColumnNames.GetHashCode()
			Return result
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
			Return outputColumnNames()(0)
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
			Return CType(newColumnNames, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
			Return CType(columnsToDuplicate, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String Implements ColumnOp.columnName
			Return columnNames()(0)
		End Function
	End Class

End Namespace