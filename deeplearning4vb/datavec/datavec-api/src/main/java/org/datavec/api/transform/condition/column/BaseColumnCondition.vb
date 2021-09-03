Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SequenceConditionMode = org.datavec.api.transform.condition.SequenceConditionMode
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

Namespace org.datavec.api.transform.condition.column


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonIgnoreProperties({"columnIdx", "schema", "sequenceMode"}) @EqualsAndHashCode(exclude = {"columnIdx", "schema", "sequenceMode"}) @Data public abstract class BaseColumnCondition implements ColumnCondition
	<Serializable>
	Public MustInherit Class BaseColumnCondition
		Implements ColumnCondition

		Public MustOverride Function condition(ByVal input As Object) As Boolean
		Public MustOverride Function columnCondition(ByVal writable As Writable) As Boolean Implements ColumnCondition.columnCondition

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ReadOnly columnName_Conflict As String
		Protected Friend columnIdx As Integer = -1
		Protected Friend schema As Schema
		Protected Friend sequenceMode As SequenceConditionMode

		Protected Friend Sub New(ByVal columnName As String, ByVal sequenceConditionMode As SequenceConditionMode)
			Me.columnName_Conflict = columnName
			Me.sequenceMode = sequenceConditionMode
		End Sub

		Public Overridable Property InputSchema Implements ColumnCondition.setInputSchema As Schema
			Set(ByVal schema As Schema)
				columnIdx = schema.getColumnNames().IndexOf(columnName_Conflict)
				If columnIdx < 0 Then
					Throw New System.InvalidOperationException("Invalid state: column """ & columnName_Conflict & """ not present in input schema")
				End If
				Me.schema = schema
			End Set
			Get
				Return schema
			End Get
		End Property


		''' <summary>
		''' Get the output schema for this transformation, given an input schema
		''' </summary>
		''' <param name="inputSchema"> </param>
		Public Overridable Function transform(ByVal inputSchema As Schema) As Schema Implements ColumnCondition.transform
			Return inputSchema
		End Function



		Public Overridable Function condition(ByVal list As IList(Of Writable)) As Boolean Implements ColumnCondition.condition
			Return columnCondition(list(columnIdx))
		End Function

		Public Overridable Function conditionSequence(ByVal list As IList(Of IList(Of Writable))) As Boolean Implements ColumnCondition.conditionSequence
			Select Case sequenceMode
				Case SequenceConditionMode.And
					For Each l As IList(Of Writable) In list
						If Not condition(l) Then
							Return False
						End If
					Next l
					Return True
				Case SequenceConditionMode.Or
					For Each l As IList(Of Writable) In list
						If condition(l) Then
							Return True
						End If
					Next l
					Return False
				Case SequenceConditionMode.NoSequenceMode
					Throw New System.InvalidOperationException("Column condition " & ToString() & " does not support sequence execution")
				Case Else
					Throw New Exception("Unknown/not implemented sequence mode: " & sequenceMode)
			End Select
		End Function

		Public Overridable Function conditionSequence(ByVal list As Object) As Boolean Implements ColumnCondition.conditionSequence
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.List<?> objects = (java.util.List<?>) list;
			Dim objects As IList(Of Object) = DirectCast(list, IList(Of Object))
			Select Case sequenceMode
				Case SequenceConditionMode.And
					For Each l As Object In objects
						If Not condition(l) Then
							Return False
						End If
					Next l
					Return True
				Case SequenceConditionMode.Or
					For Each l As Object In objects
						If condition(l) Then
							Return True
						End If
					Next l
					Return False
				Case SequenceConditionMode.NoSequenceMode
					Throw New System.InvalidOperationException("Column condition " & ToString() & " does not support sequence execution")
				Case Else
					Throw New Exception("Unknown/not implemented sequence mode: " & sequenceMode)
			End Select
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String Implements ColumnCondition.outputColumnName
			Return columnName()
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String() Implements ColumnCondition.outputColumnNames
			Return columnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements ColumnCondition.columnNames
			Return New String() {columnName_Conflict}
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String Implements ColumnCondition.columnName
			Return columnNames()(0)
		End Function

		Public MustOverride Overrides Function ToString() As String
	End Class

End Namespace