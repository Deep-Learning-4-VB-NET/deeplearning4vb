Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports Writable = org.datavec.api.writable.Writable
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

Namespace org.datavec.api.transform.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"inputSchema", "columnNumber"}) @NoArgsConstructor public abstract class BaseColumnTransform extends BaseTransform implements org.datavec.api.transform.ColumnOp
	<Serializable>
	Public MustInherit Class BaseColumnTransform
		Inherits BaseTransform
		Implements ColumnOp

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend columnName_Conflict As String
		Protected Friend columnNumber As Integer = -1
		Private Const serialVersionUID As Long = 0L

		Public Sub New(ByVal columnName As String)
			Me.columnName_Conflict = columnName
		End Sub

		Public Overrides WriteOnly Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
				columnNumber = inputSchema.getIndexOfColumn(columnName_Conflict)
			End Set
		End Property

		Public Overrides Function transform(ByVal schema As Schema) As Schema
			If columnNumber = -1 Then
				Throw New System.InvalidOperationException("columnNumber == -1 -> setInputSchema not called?")
			End If
			Dim oldMeta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta.Count)

			Dim typesIter As IEnumerator(Of ColumnMetaData) = oldMeta.GetEnumerator()

			Dim i As Integer = 0
			Do While typesIter.MoveNext()
				Dim t As ColumnMetaData = typesIter.Current
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == columnNumber)
				If i = columnNumber Then
						i += 1
					newMeta.Add(getNewColumnMetaData(t.Name, t))
				Else
						i += 1
					newMeta.Add(t)
				End If
			Loop

			Return schema.newSchema(newMeta)
		End Function

		Public MustOverride Function getNewColumnMetaData(ByVal newName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			If writables.Count <> inputSchema_Conflict.numColumns() Then
				Throw New System.InvalidOperationException("Cannot execute transform: input writables list length (" & writables.Count & ") does not " & "match expected number of elements (schema: " & inputSchema_Conflict.numColumns() & "). Transform = " & ToString())
			End If
			Dim n As Integer = writables.Count
			Dim [out] As IList(Of Writable) = New List(Of Writable)(n)

			Dim i As Integer = 0
			For Each w As Writable In writables
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (i++ == columnNumber)
				If i = columnNumber Then
						i += 1
					Dim newW As Writable = map(w)
					[out].Add(newW)
				Else
						i += 1
					[out].Add(w)
				End If
			Next w

			Return [out]
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
			Return New String() {columnName_Conflict}
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnNames() As String() Implements ColumnOp.columnNames
			Return New String() {columnName_Conflict}
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

		Public MustOverride Function map(ByVal columnWritable As Writable) As Writable

		Public MustOverride Overrides Function ToString() As String

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim o2 As BaseColumnTransform = DirectCast(o, BaseColumnTransform)

			Return columnName_Conflict.Equals(o2.columnName_Conflict)

		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overrides Function mapSequence(ByVal sequence As Object) As Object
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> list = (java.util.List<?>) sequence;
			Dim list As IList(Of Object) = DirectCast(sequence, IList(Of Object))
			Dim ret As IList(Of Object) = New List(Of Object)()
			For Each o As Object In list
				ret.Add(map(o))
			Next o
			Return ret
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return columnName_Conflict.GetHashCode()
		End Function
	End Class

End Namespace