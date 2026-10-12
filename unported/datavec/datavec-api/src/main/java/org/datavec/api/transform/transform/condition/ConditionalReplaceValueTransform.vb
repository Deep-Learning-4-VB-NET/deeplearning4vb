Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnOp = org.datavec.api.transform.ColumnOp
Imports Transform = org.datavec.api.transform.Transform
Imports Condition = org.datavec.api.transform.condition.Condition
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

Namespace org.datavec.api.transform.transform.condition


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"columnToReplaceIdx"}) @EqualsAndHashCode(exclude = {"columnToReplaceIdx"}) @Data public class ConditionalReplaceValueTransform implements org.datavec.api.transform.Transform, org.datavec.api.transform.ColumnOp
	<Serializable>
	Public Class ConditionalReplaceValueTransform
		Implements Transform, ColumnOp

		Private ReadOnly columnToReplace As String
		Private ReadOnly newValue As Writable
		Private ReadOnly condition As Condition
		Private columnToReplaceIdx As Integer = -1

		''' <param name="columnToReplace"> Name of the column in which to replace the old value with 'newValue', if the condition holds </param>
		''' <param name="newValue">        New value to use </param>
		''' <param name="condition">       Condition </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ConditionalReplaceValueTransform(@JsonProperty("columnToReplace") String columnToReplace, @JsonProperty("newValue") org.datavec.api.writable.Writable newValue, @JsonProperty("condition") org.datavec.api.transform.condition.Condition condition)
		Public Sub New(ByVal columnToReplace As String, ByVal newValue As Writable, ByVal condition As Condition)
			Me.columnToReplace = columnToReplace
			Me.newValue = newValue
			Me.condition = condition
		End Sub

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			'Conditional replace should not change any of the metadata, under normal usage
			Return inputSchema
		End Function

		Public Overridable Property InputSchema Implements ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				columnToReplaceIdx = inputSchema.getColumnNames().IndexOf(columnToReplace)
				If columnToReplaceIdx < 0 Then
					Throw New System.InvalidOperationException("Column """ & columnToReplace & """ not found in input schema")
				End If
				condition.InputSchema = inputSchema
			End Set
			Get
				Return condition.InputSchema
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			If condition.condition(writables) Then
				'Condition holds -> set new value
				Dim newList As IList(Of Writable) = New List(Of Writable)(writables)
				newList(columnToReplaceIdx) = newValue
				Return newList
			Else
				'Condition does not hold -> no change
				Return writables
			End If
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
			If condition.condition(input) Then
				Return newValue
			End If
			Return input

		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> seq = (java.util.List<?>) sequence;
			Dim seq As IList(Of Object) = DirectCast(sequence, IList(Of Object))
			Dim [out] As IList(Of Object) = New List(Of Object)()
			For Each [step] As Object In seq
				[out].Add(map([step]))
			Next [step]
			Return [out]
		End Function

		Public Overrides Function ToString() As String
			Return "ConditionalReplaceValueTransform(replaceColumn=""" & columnToReplace & """,newValue=" & newValue & ",condition=" & condition & ")"
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
			Return columnToReplace
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
			Return columnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
			Return New String() {columnToReplace}
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String Implements ColumnOp.columnName
			Return columnToReplace
		End Function
	End Class

End Namespace