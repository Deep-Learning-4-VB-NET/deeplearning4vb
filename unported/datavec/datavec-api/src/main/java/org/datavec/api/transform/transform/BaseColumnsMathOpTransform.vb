Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports MathOp = org.datavec.api.transform.MathOp
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports DoubleMathOpTransform = org.datavec.api.transform.transform.doubletransform.DoubleMathOpTransform
Imports IntegerMathOpTransform = org.datavec.api.transform.transform.integer.IntegerMathOpTransform
Imports LongMathOpTransform = org.datavec.api.transform.transform.longtransform.LongMathOpTransform
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude

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
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonIgnoreProperties({"columnIdxs", "inputSchema"}) @EqualsAndHashCode(exclude = {"columnIdxs", "inputSchema"}) @Data public abstract class BaseColumnsMathOpTransform implements org.datavec.api.transform.Transform, org.datavec.api.transform.ColumnOp
	<Serializable>
	Public MustInherit Class BaseColumnsMathOpTransform
		Implements Transform, ColumnOp

		Public MustOverride Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
		Public MustOverride Function map(ByVal input As Object) As Object Implements Transform.map

		Protected Friend ReadOnly newColumnName As String
		Protected Friend ReadOnly mathOp As MathOp
		Protected Friend ReadOnly columns() As String
		Private columnIdxs() As Integer
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		Public Sub New(ByVal newColumnName As String, ByVal mathOp As MathOp, ParamArray ByVal columns() As String)
			If columns Is Nothing OrElse columns.Length = 0 Then
				Throw New System.ArgumentException("Invalid input: cannot have null/0 columns")
			End If
			Me.newColumnName = newColumnName
			Me.mathOp = mathOp
			Me.columns = columns

			Select Case mathOp
				Case MathOp.Add
					If columns.Length < 2 Then
						Throw New System.ArgumentException("Need 2 or more columns for Add op. Got: " & Arrays.toString(columns))
					End If
				Case MathOp.Subtract
					If columns.Length <> 2 Then
						Throw New System.ArgumentException("Need exactly 2 columns for Subtract op. Got: " & Arrays.toString(columns))
					End If
				Case MathOp.Multiply
					If columns.Length < 2 Then
						Throw New System.ArgumentException("Need 2 or more columns for Multiply op. Got: " & Arrays.toString(columns))
					End If
				Case MathOp.Divide
					If columns.Length <> 2 Then
						Throw New System.ArgumentException("Need exactly 2 columns for Divide op. Got: " & Arrays.toString(columns))
					End If
				Case MathOp.Modulus
					If columns.Length <> 2 Then
						Throw New System.ArgumentException("Need exactly 2 columns for Modulus op. Got: " & Arrays.toString(columns))
					End If
				Case MathOp.ReverseSubtract, ReverseDivide, ScalarMin, ScalarMax
					Throw New System.ArgumentException("Invalid MathOp: cannot use " & mathOp & " with ...ColumnsMathOpTransform")
				Case Else
					Throw New Exception("Unknown MathOp: " & mathOp)
			End Select
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			For Each name As String In columns
				If Not inputSchema.hasColumn(name) Then
					Throw New System.InvalidOperationException("Input schema does not have column with name """ & name & """")
				End If
			Next name

			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())

			newMeta.Add(derivedColumnMetaData(newColumnName, inputSchema))

			Return inputSchema.newSchema(newMeta)
		End Function

		Public Overridable Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				columnIdxs = New Integer(columns.Length - 1){}
				Dim i As Integer = 0
				For Each name As String In columns
					If Not inputSchema.hasColumn(name) Then
						Throw New System.InvalidOperationException("Input schema does not have column with name """ & name & """")
					End If
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: columnIdxs[i++] = inputSchema.getIndexOfColumn(name);
					columnIdxs(i) = inputSchema.getIndexOfColumn(name)
						i += 1
				Next name
    
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			If inputSchema_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Input schema has not been set")
			End If
			Dim [out] As IList(Of Writable) = New List(Of Writable)(writables)

			Dim temp(columns.Length - 1) As Writable
			For i As Integer = 0 To columnIdxs.Length - 1
				temp(i) = [out](columnIdxs(i))
			Next i

			[out].Add(doOp(temp))
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
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
			Return newColumnName
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
			Return New String() {newColumnName}
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
			Return columns
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

		Protected Friend MustOverride Function derivedColumnMetaData(ByVal newColumnName As String, ByVal inputSchema As Schema) As ColumnMetaData

		Protected Friend MustOverride Function doOp(ParamArray ByVal input() As Writable) As Writable

		Public MustOverride Overrides Function ToString() As String
	End Class

End Namespace