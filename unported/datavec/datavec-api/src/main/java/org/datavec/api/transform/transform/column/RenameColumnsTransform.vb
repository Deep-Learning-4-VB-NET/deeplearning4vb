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
'ORIGINAL LINE: @JsonIgnoreProperties({"inputSchema"}) @Data public class RenameColumnsTransform implements org.datavec.api.transform.Transform, org.datavec.api.transform.ColumnOp
	<Serializable>
	Public Class RenameColumnsTransform
		Implements Transform, ColumnOp

		Private ReadOnly oldNames As IList(Of String)
		Private ReadOnly newNames As IList(Of String)
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		Public Sub New(ByVal oldName As String, ByVal newName As String)
			Me.New(Collections.singletonList(oldName), Collections.singletonList(newName))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RenameColumnsTransform(@JsonProperty("oldNames") java.util.List<String> oldNames, @JsonProperty("newNames") java.util.List<String> newNames)
		Public Sub New(ByVal oldNames As IList(Of String), ByVal newNames As IList(Of String))
			If oldNames.Count <> newNames.Count Then
				Throw New System.ArgumentException("Invalid input: old/new names lists differ in length")
			End If
			Me.oldNames = oldNames
			Me.newNames = newNames
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim o2 As RenameColumnsTransform = DirectCast(o, RenameColumnsTransform)

'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: if (!oldNames.equals(o2.oldNames))
			If Not oldNames.SequenceEqual(o2.oldNames) Then
				Return False
			End If
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: return newNames.equals(o2.newNames);
			Return newNames.SequenceEqual(o2.newNames)

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = oldNames.GetHashCode()
			result = 31 * result + newNames.GetHashCode()
			Return result
		End Function

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			'Validate that all 'original' names exist:
			For i As Integer = 0 To oldNames.Count - 1
				Dim s As String = oldNames(i)
				If Not inputSchema.hasColumn(s) Then
					Throw New System.InvalidOperationException("Cannot rename from """ & s & """ to """ & newNames(i) & """: original column name """ & s & """ does not exist. All columns for input schema: " & inputSchema.getColumnNames())
				End If
			Next i

			Dim inputNames As IList(Of String) = inputSchema.getColumnNames()

			Dim outputMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)()
			For Each s As String In inputNames
				Dim idx As Integer = oldNames.IndexOf(s)
				If idx >= 0 Then
					'Switch the old and new names
					Dim meta As ColumnMetaData = inputSchema.getMetaData(s).clone()
					meta.Name = newNames(idx)
					outputMeta.Add(meta)
				Else
					outputMeta.Add(inputSchema.getMetaData(s))
				End If
			Next s

			Return inputSchema.newSchema(outputMeta)
		End Function

		Public Overridable Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			'No op
			Return writables
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			'No op
			Return sequence
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

		Public Overrides Function ToString() As String
			Return "RenameColumnsTransform(oldNames=" & oldNames & ",newNames=" & newNames & ")"
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
			Return CType(newNames, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
			Return CType(oldNames, List(Of String)).ToArray()
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