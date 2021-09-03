Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Schema = org.datavec.api.transform.schema.Schema
Imports BaseTransform = org.datavec.api.transform.transform.BaseTransform
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.transform.parse


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class ParseDoubleTransform extends org.datavec.api.transform.transform.BaseTransform
	<Serializable>
	Public Class ParseDoubleTransform
		Inherits BaseTransform

		Public Overrides Function ToString() As String
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Return Me.GetType().FullName
		End Function

		''' <summary>
		''' Get the output schema for this transformation, given an input schema
		''' </summary>
		''' <param name="inputSchema"> </param>
		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Dim newSchema As New Schema.Builder()
			Dim i As Integer = 0
			Do While i < inputSchema.numColumns()
				If inputSchema.getType(i) = ColumnType.String Then
					newSchema.addColumnDouble(inputSchema.getMetaData(i).Name)
				Else
					newSchema.addColumn(inputSchema.getMetaData(i))
				End If

				i += 1
			Loop
			Return newSchema.build()
		End Function

		Public Overridable Overloads Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable)
			Dim transform As IList(Of Writable) = New List(Of Writable)()
			For Each w As Writable In writables
				If TypeOf w Is Text Then
					transform.Add(New DoubleWritable(w.toDouble()))
				Else
					transform.Add(w)
				End If
			Next w
			Return transform
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Return Double.Parse(input.ToString())
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

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overrides Function outputColumnName() As String
			Return InputSchema.getColumnNames().get(0)
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overrides Function outputColumnNames() As String()
			Return CType(inputSchema_Conflict.getColumnNames(), List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnNames() As String()
			Return CType(inputSchema_Conflict.getColumnNames(), List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overrides Function columnName() As String
			Return outputColumnName()
		End Function
	End Class

End Namespace