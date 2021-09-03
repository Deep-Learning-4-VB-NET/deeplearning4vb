Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Data = lombok.Data
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseTransform = org.datavec.api.transform.transform.BaseTransform
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
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "columnsToRemoveIdx", "indicesToRemove"}) @Data public class RemoveColumnsTransform extends org.datavec.api.transform.transform.BaseTransform implements org.datavec.api.transform.ColumnOp
	<Serializable>
	Public Class RemoveColumnsTransform
		Inherits BaseTransform
		Implements ColumnOp

		Private columnsToRemoveIdx() As Integer
		Private columnsToRemove() As String
		Private indicesToRemove As ISet(Of Integer)
		Private leftOverColumns() As String

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RemoveColumnsTransform(@JsonProperty("columnsToRemove") String... columnsToRemove)
		Public Sub New(ParamArray ByVal columnsToRemove() As String)
			Me.columnsToRemove = columnsToRemove

		End Sub

		Public Overrides WriteOnly Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal schema As Schema)
				MyBase.InputSchema = schema
				'Validate that all 'columns to be removed exist
				For Each s As String In columnsToRemove
					If Not inputSchema_Conflict.hasColumn(s) Then
						Throw New System.InvalidOperationException("Cannot remove column """ & s & """: column does not exist. All " & "columns for input schema: " & inputSchema_Conflict.getColumnNames())
					End If
				Next s
    
    
				leftOverColumns = New String((schema.numColumns() - columnsToRemove.Length) - 1){}
    
				indicesToRemove = New HashSet(Of Integer)()
    
				Dim i As Integer = 0
				columnsToRemoveIdx = New Integer(columnsToRemove.Length - 1){}
				For Each s As String In columnsToRemove
					Dim idx As Integer = schema.getIndexOfColumn(s)
					If idx < 0 Then
						Throw New Exception("Column """ & s & """ not found")
					End If
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: columnsToRemoveIdx[i++] = idx;
					columnsToRemoveIdx(i) = idx
						i += 1
					indicesToRemove.Add(idx)
				Next s
    
    
				Dim leftOverColumnsIdx As Integer = 0
				Dim columnTest As IList(Of String) = New List(Of String) From {columnsToRemove}
				Dim origColumnNames As IList(Of String) = schema.getColumnNames()
				Dim remove As Integer = 0
				Do While remove < schema.numColumns()
					If Not columnTest.Contains(origColumnNames(remove)) Then
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: leftOverColumns[leftOverColumnsIdx++] = origColumnNames.get(remove);
						leftOverColumns(leftOverColumnsIdx) = origColumnNames(remove)
							leftOverColumnsIdx += 1
					End If
					remove += 1
				Loop
			End Set
		End Property

		Public Overrides Function transform(ByVal schema As Schema) As Schema
			Dim nToRemove As Integer = columnsToRemove.Length
			Dim newNumColumns As Integer = schema.numColumns() - nToRemove
			If newNumColumns <= 0 Then
				Throw New System.InvalidOperationException("Number of columns after executing operation is " & newNumColumns & " (is <= 0). " & "origColumns = " & schema.getColumnNames() & ", toRemove = " & java.util.Arrays.toString(columnsToRemove))
			End If

			Dim origNames As IList(Of String) = schema.getColumnNames()
			Dim origMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()

			Dim set As ISet(Of String) = New HashSet(Of String)()
			Collections.addAll(set, columnsToRemove)


			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(newNumColumns)

			Dim namesIter As IEnumerator(Of String) = origNames.GetEnumerator()
			Dim metaIter As IEnumerator(Of ColumnMetaData) = origMeta.GetEnumerator()

			Do While namesIter.MoveNext()
				Dim n As String = namesIter.Current
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim t As ColumnMetaData = metaIter.next()
				If Not set.Contains(n) Then
					newMeta.Add(t)
				End If
			Loop

			Return schema.newSchema(newMeta)
		End Function

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Dim list As IList(Of String) = New List(Of String)()
				For Each w As Writable In writables
					list.Add(w.ToString())
				Next w
				Dim toString As String = StringUtils.join(list, ",")
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & Me.ToString() & " and record " & toString)
			End If

			Dim outList As IList(Of Writable) = New List(Of Writable)(writables.Count - columnsToRemove.Length)

			Dim i As Integer = 0
			For Each w As Writable In writables
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (indicesToRemove.contains(i++))
				If indicesToRemove.Contains(i) Then
						i += 1
					Continue For
					Else
						i += 1
					End If
				outList.Add(w)
			Next w
			Return outList
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

		Public Overrides Function ToString() As String
			Return "RemoveColumnsTransform(" & java.util.Arrays.toString(columnsToRemove) & ")"
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim o2 As RemoveColumnsTransform = DirectCast(o, RemoveColumnsTransform)

			Return columnsToRemove.SequenceEqual(o2.columnsToRemove)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return java.util.Arrays.hashCode(columnsToRemove)
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overrides Function outputColumnName() As String Implements ColumnOp.outputColumnName
			Return outputColumnNames()(0)
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overrides Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
			Return leftOverColumns
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnNames() As String() Implements ColumnOp.columnNames
			Return CType(inputSchema_Conflict.getColumnNames(), List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnName() As String Implements ColumnOp.columnName
			Return columnNames()(0)
		End Function
	End Class

End Namespace