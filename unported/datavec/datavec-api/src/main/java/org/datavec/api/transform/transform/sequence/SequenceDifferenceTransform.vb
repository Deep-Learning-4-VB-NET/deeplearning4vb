Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports Transform = org.datavec.api.transform.Transform
Imports org.datavec.api.transform.metadata
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports org.datavec.api.writable
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

Namespace org.datavec.api.transform.transform.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonIgnoreProperties({"inputSchema", "columnType"}) @Data public class SequenceDifferenceTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class SequenceDifferenceTransform
		Implements Transform


		Public Enum FirstStepMode
			[Default]
			SpecifiedValue
		End Enum

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly columnName_Conflict As String
		Private ReadOnly newColumnName As String
		Private ReadOnly lookback As Integer
		Private ReadOnly firstStepMode As FirstStepMode
		Private ReadOnly specifiedValueWritable As Writable

'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema
		Private columnType As ColumnType

		''' <summary>
		''' Create a SequenceDifferenceTransform with default lookback of 1, and using FirstStepMode.Default.
		''' Output column name is the same as the input column name.
		''' </summary>
		''' <param name="columnName"> Name of the column to perform the operation on. </param>
		Public Sub New(ByVal columnName As String)
			Me.New(columnName, columnName, 1, FirstStepMode.Default, Nothing)
		End Sub

		''' <summary>
		''' Create a SequenceDifferenceTransform with default lookback of 1, and using FirstStepMode.Default,
		''' where the output column name is specified
		''' </summary>
		''' <param name="columnName">    Name of the column to perform the operation on. </param>
		''' <param name="newColumnName"> New name for the column. May be same as the origina lcolumn name </param>
		''' <param name="lookback">      Lookback period, in number of time steps. Must be > 0 </param>
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal lookback As Integer)
			Me.New(columnName, newColumnName, lookback, FirstStepMode.Default, Nothing)
		End Sub

		''' <summary>
		''' Create a SequenceDifferenceTransform with default lookback of 1, and using FirstStepMode.Default,
		''' where the output column name is specified
		''' </summary>
		''' <param name="columnName">             Name of the column to perform the operation on. </param>
		''' <param name="newColumnName">          New name for the column. May be same as the origina lcolumn name </param>
		''' <param name="lookback">               Lookback period, in number of time steps. Must be > 0 </param>
		''' <param name="firstStepMode">          see <seealso cref="FirstStepMode"/> </param>
		''' <param name="specifiedValueWritable"> Must be null if using FirstStepMode.Default, or non-null if using FirstStepMode.SpecifiedValue </param>
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal lookback As Integer, ByVal firstStepMode As FirstStepMode, ByVal specifiedValueWritable As Writable)
			If firstStepMode <> FirstStepMode.SpecifiedValue AndAlso specifiedValueWritable IsNot Nothing Then
				Throw New System.ArgumentException("Specified value writable provided (" & specifiedValueWritable & ") but " & "firstStepMode != FirstStepMode.SpecifiedValue")
			End If
			If firstStepMode = FirstStepMode.SpecifiedValue AndAlso specifiedValueWritable Is Nothing Then
				Throw New System.ArgumentException("Specified value writable is null but firstStepMode != FirstStepMode.SpecifiedValue")
			End If
			If lookback <= 0 Then
				Throw New System.ArgumentException("Lookback period must be > 0. Got: lookback period = " & lookback)
			End If

			Me.columnName_Conflict = columnName
			Me.newColumnName = newColumnName
			Me.lookback = lookback
			Me.firstStepMode = firstStepMode
			Me.specifiedValueWritable = specifiedValueWritable
		End Sub

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String Implements org.datavec.api.transform.ColumnOp.outputColumnName
			Return columnName_Conflict
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String() Implements org.datavec.api.transform.ColumnOp.outputColumnNames
			Return New String() {columnName()}
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String() Implements org.datavec.api.transform.ColumnOp.columnNames
			Return New String() {columnName()}
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String Implements org.datavec.api.transform.ColumnOp.columnName
			Return columnName_Conflict
		End Function


		Public Overridable Function transform(ByVal inputSchema As Schema) As Schema
			If Not inputSchema.hasColumn(columnName_Conflict) Then
				Throw New System.InvalidOperationException("Invalid input schema: does not have column with name """ & columnName_Conflict & """" & vbLf & ". All schema names: " & inputSchema.getColumnNames())
			End If
			If Not (TypeOf inputSchema Is SequenceSchema) Then
				Throw New System.InvalidOperationException("Invalid input schema: expected a SequenceSchema, got " & inputSchema.GetType())
			End If

			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.numColumns())
			For Each m As ColumnMetaData In inputSchema.getColumnMetaData()
				If columnName_Conflict.Equals(m.Name) Then
					Select Case m.ColumnType
						Case Integer?
							newMeta.Add(New IntegerMetaData(newColumnName))
						Case Long?
							newMeta.Add(New LongMetaData(newColumnName))
						Case Double?
							newMeta.Add(New DoubleMetaData(newColumnName))
						Case Single?
							newMeta.Add(New FloatMetaData(newColumnName))
						Case Time
							newMeta.Add(New LongMetaData(newColumnName)) 'not Time - time column isn't used for duration...
						Case Else
							Throw New System.InvalidOperationException("Cannot perform sequence difference on column of type " & m.ColumnType)
					End Select
				Else
					newMeta.Add(m)
				End If
			Next m

			Return inputSchema.newSchema(newMeta)
		End Function

		Public Overridable Property InputSchema Implements org.datavec.api.transform.ColumnOp.setInputSchema As Schema
			Set(ByVal inputSchema As Schema)
				If Not inputSchema.hasColumn(columnName_Conflict) Then
					Throw New System.InvalidOperationException("Invalid input schema: does not have column with name """ & columnName_Conflict & """" & vbLf & ". All schema names: " & inputSchema.getColumnNames())
				End If
    
				Me.columnType = inputSchema.getMetaData(columnName_Conflict).ColumnType
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Throw New System.NotSupportedException("Only sequence operations are supported for SequenceDifferenceTransform." & " Attempting to apply SequenceDifferenceTransform on non-sequence data?")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim columnIdx As Integer = inputSchema_Conflict.getIndexOfColumn(columnName_Conflict)

			Dim numSteps As Integer = sequence.Count
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For i As Integer = 0 To numSteps - 1
				Dim timeStep As IList(Of Writable) = sequence(i)
				Dim newTimeStep As IList(Of Writable) = New List(Of Writable)(timeStep.Count)
				For j As Integer = 0 To timeStep.Count - 1
					If j = columnIdx Then
						If j < lookback AndAlso firstStepMode = FirstStepMode.SpecifiedValue Then
							newTimeStep.Add(specifiedValueWritable)
						Else
							Dim current As Writable = timeStep(j)
							Dim past As Writable = sequence(Math.Max(0, i - lookback))(j)
							Select Case columnType.innerEnumValue
								Case Integer?
									newTimeStep.Add(New IntWritable(current.toInt() - past.toInt()))
								Case Double?
									newTimeStep.Add(New DoubleWritable(current.toDouble() - past.toDouble()))
								Case Single?
									newTimeStep.Add(New FloatWritable(current.toFloat() - past.toFloat()))
								Case Long?, Time
									newTimeStep.Add(New LongWritable(current.toLong() - past.toLong()))
								Case Else
									Throw New System.InvalidOperationException("Cannot perform sequence difference on column of type " & columnType)
							End Select
						End If
					Else
						newTimeStep.Add(timeStep(j))
					End If
				Next j
				[out].Add(newTimeStep)
			Next i

			Return [out]
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("Only sequence operations are supported for SequenceDifferenceTransform." & " Attempting to apply SequenceDifferenceTransform on non-sequence data?")
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Only sequence operations are supported for SequenceDifferenceTransform." & " Attempting to apply SequenceDifferenceTransform on non-sequence data?")
		End Function
	End Class

End Namespace