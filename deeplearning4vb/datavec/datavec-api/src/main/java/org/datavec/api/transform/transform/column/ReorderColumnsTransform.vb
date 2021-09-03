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
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema", "outputOrder"}) @Data public class ReorderColumnsTransform implements org.datavec.api.transform.Transform, org.datavec.api.transform.ColumnOp
	<Serializable>
	Public Class ReorderColumnsTransform
		Implements Transform, ColumnOp

		Private ReadOnly newOrder As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema
		Private outputOrder() As Integer 'Mapping from in to out. so output[i] = input.get(outputOrder[i])

		''' <param name="newOrder"> A partial or complete order of the columns in the output </param>
		Public Sub New(ParamArray ByVal newOrder() As String)
			Me.New(Arrays.asList(newOrder))
		End Sub

		''' <param name="newOrder"> A partial or complete order of the columns in the output </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReorderColumnsTransform(@JsonProperty("newOrder") java.util.List<String> newOrder)
		Public Sub New(ByVal newOrder As IList(Of String))
			Me.newOrder = newOrder
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			For Each s As String In newOrder
				If Not inputSchema.hasColumn(s) Then
					Throw New System.InvalidOperationException("Input schema does not contain column with name """ & s & """")
				End If
			Next s
			If inputSchema.numColumns() < newOrder.Count Then
				Throw New System.ArgumentException("Schema has " & inputSchema.numColumns() & " column but newOrder has " & newOrder.Count & " columns")
			End If

			Dim origNames As IList(Of String) = inputSchema.getColumnNames()
			Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
			Dim outMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()

			Dim taken(origNames.Count - 1) As Boolean
			For Each s As String In newOrder
				Dim idx As Integer = inputSchema.getIndexOfColumn(s)
				outMeta.Add(origMeta(idx))
				taken(idx) = True
			Next s

			For i As Integer = 0 To taken.Length - 1
				If taken(i) Then
					Continue For
				End If
				outMeta.Add(origMeta(i))
			Next i

			Return inputSchema.newSchema(outMeta)
		End Function

		Public Overridable Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				For Each s As String In newOrder
					If Not inputSchema.hasColumn(s) Then
						Throw New System.InvalidOperationException("Input schema does not contain column with name """ & s & """")
					End If
				Next s
				If inputSchema.numColumns() < newOrder.Count Then
					Throw New System.ArgumentException("Schema has " & inputSchema.numColumns() & " columns but newOrder has " & newOrder.Count & " columns")
				End If
    
				Dim origNames As IList(Of String) = inputSchema.getColumnNames()
				outputOrder = New Integer(origNames.Count - 1){}
    
				Dim taken(origNames.Count - 1) As Boolean
				Dim j As Integer = 0
				For Each s As String In newOrder
					Dim idx As Integer = inputSchema.getIndexOfColumn(s)
					taken(idx) = True
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: outputOrder[j++] = idx;
					outputOrder(j) = idx
						j += 1
				Next s
    
				For i As Integer = 0 To taken.Length - 1
					If taken(i) Then
						Continue For
					End If
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: outputOrder[j++] = i;
					outputOrder(j) = i
						j += 1
				Next i
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Dim [out] As IList(Of Writable) = New List(Of Writable)()
			For Each i As Integer In outputOrder
				[out].Add(writables(i))
			Next i
			Return [out]
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
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
		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")

		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Unable to map. Please treat this as a special operation. This should be handled by your implementation.")
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim o2 As ReorderColumnsTransform = DirectCast(o, ReorderColumnsTransform)

'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (!newOrder.equals(o2.newOrder))
			If Not newOrder.SequenceEqual(o2.newOrder) Then
				Return False
			End If
			Return outputOrder.SequenceEqual(o2.outputOrder)

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = newOrder.GetHashCode()
			result = 31 * result + Arrays.hashCode(outputOrder)
			Return result
		End Function

		Public Overrides Function ToString() As String
			Return "ReorderColumnsTransform(newOrder=" & newOrder & ")"

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
			Return CType(newOrder, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
			Return InputSchema.getColumnNames().toArray(New String((InputSchema.getColumnNames().size()) - 1){})
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