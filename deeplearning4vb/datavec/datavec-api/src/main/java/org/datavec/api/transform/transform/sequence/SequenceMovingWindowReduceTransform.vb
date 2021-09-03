Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = org.datavec.api.transform.metadata.DoubleMetaData
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports org.datavec.api.transform.ops
Imports AggregableReductionUtils = org.datavec.api.transform.reduce.AggregableReductionUtils
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports Writable = org.datavec.api.writable.Writable
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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

Namespace org.datavec.api.transform.transform.sequence


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonIgnoreProperties({"inputSchema"}) @Data public class SequenceMovingWindowReduceTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class SequenceMovingWindowReduceTransform
		Implements Transform

		''' <summary>
		''' Enumeration to specify how each cases are handled: For example, for a look back period of 20, how should the
		''' first 19 output values be calculated?<br>
		''' Default: Perform your former reduction as normal, with as many values are available<br>
		''' SpecifiedValue: use the given/specified value instead of the actual output value. For example, you could assign
		''' values of 0 or NullWritable to positions 0 through 18 of the output.
		''' </summary>
		Public Enum EdgeCaseHandling
			[Default]
			SpecifiedValue
		End Enum

'JAVA TO VB CONVERTER NOTE: The field columnName was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly columnName_Conflict As String
		Private ReadOnly newColumnName As String
		Private ReadOnly lookback As Integer
		Private ReadOnly op As ReduceOp
		Private ReadOnly edgeCaseHandling As EdgeCaseHandling
		Private ReadOnly edgeCaseValue As Writable
'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

		''' 
		''' <param name="columnName"> Column name to perform windowing on </param>
		''' <param name="lookback">   Look back period for windowing </param>
		''' <param name="op">         Reduction operation to perform on each window </param>
		Public Sub New(ByVal columnName As String, ByVal lookback As Integer, ByVal op As ReduceOp)
			Me.New(columnName, defaultOutputColumnName(columnName, lookback, op), lookback, op, EdgeCaseHandling.Default, Nothing)
		End Sub

		''' <param name="columnName">       Column name to perform windowing on </param>
		''' <param name="newColumnName">    Name of the new output column (with results) </param>
		''' <param name="lookback">         Look back period for windowing </param>
		''' <param name="op">               Reduction operation to perform on each window </param>
		''' <param name="edgeCaseHandling"> How the 1st steps should be handled (positions in sequence with indices less then the look-back period) </param>
		''' <param name="edgeCaseValue">    Used only with EdgeCaseHandling.SpecifiedValue, maybe null otherwise </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SequenceMovingWindowReduceTransform(@JsonProperty("columnName") String columnName, @JsonProperty("newColumnName") String newColumnName, @JsonProperty("lookback") int lookback, @JsonProperty("op") org.datavec.api.transform.ReduceOp op, @JsonProperty("edgeCaseHandling") EdgeCaseHandling edgeCaseHandling, @JsonProperty("edgeCaseValue") org.datavec.api.writable.Writable edgeCaseValue)
		Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal lookback As Integer, ByVal op As ReduceOp, ByVal edgeCaseHandling As EdgeCaseHandling, ByVal edgeCaseValue As Writable)
			Me.columnName_Conflict = columnName
			Me.newColumnName = newColumnName
			Me.lookback = lookback
			Me.op = op
			Me.edgeCaseHandling = edgeCaseHandling
			Me.edgeCaseValue = edgeCaseValue
		End Sub

		Public Shared Function defaultOutputColumnName(ByVal originalName As String, ByVal lookback As Integer, ByVal op As ReduceOp) As String
			Return op.ToString().ToLower() & "(" & lookback & "," & originalName & ")"
		End Function

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Dim colIdx As Integer = inputSchema.getIndexOfColumn(columnName_Conflict)

			'Approach here: The reducer gives us a schema for one time step -> simply convert this to a sequence schema...
			Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
			Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta)

			Dim m As ColumnMetaData
			Select Case op
				Case ReduceOp.Min, Max, Range, TakeFirst, TakeLast
					'Same type as input
					m = oldMeta(colIdx)
					m = m.clone()
					m.Name = newColumnName
				Case ReduceOp.Prod, Sum, Mean, Stdev
					'Double type
					m = New DoubleMetaData(newColumnName)
				Case ReduceOp.Count, CountUnique
					'Integer type
					m = New IntegerMetaData(newColumnName)
				Case Else
					Throw New System.NotSupportedException("Unknown op type: " & op)
			End Select
			meta.Add(m)

			Return New SequenceSchema(meta)
		End Function

		Public Overridable Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				Me.inputSchema_Conflict = inputSchema
			End Set
			Get
				Return inputSchema_Conflict
			End Get
		End Property


		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Throw New System.NotSupportedException("SequenceMovingWindowReduceTransform can only be applied on sequences")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim colIdx As Integer = inputSchema_Conflict.getIndexOfColumn(columnName_Conflict)
			Dim columnType As ColumnType = inputSchema_Conflict.getType(colIdx)
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(sequence.Count)
			Dim window As New LinkedList(Of Writable)()
			For i As Integer = 0 To sequence.Count - 1
				Dim current As Writable = sequence(i)(colIdx)
				window.AddLast(current)
				If window.Count > lookback Then
					window.RemoveFirst()
				End If
				Dim reduced As Writable
				If window.Count < lookback AndAlso edgeCaseHandling = EdgeCaseHandling.SpecifiedValue Then
					reduced = edgeCaseValue
				Else
					Dim reductionOp As IAggregableReduceOp(Of Writable, IList(Of Writable)) = AggregableReductionUtils.reduceColumn(Collections.singletonList(op), columnType, False, Nothing)
					For Each w As Writable In window
						reductionOp.accept(w)
					Next w
					reduced = reductionOp.get().get(0)
				End If
				Dim outThisStep As New List(Of Writable)(sequence(i).Count + 1)
				outThisStep.AddRange(sequence(i))
				outThisStep.Add(reduced)
				[out].Add(outThisStep)
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
			Throw New System.NotSupportedException("SequenceMovingWindowReduceTransform can only be applied to sequences")
		End Function

		''' <summary>
		''' Transform a sequence
		''' </summary>
		''' <param name="sequence"> </param>
		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Public Overrides Function ToString() As String
			Return "SequenceMovingWindowReduceTransform(columnName=""" & columnName_Conflict & """,newColumnName=""" & newColumnName & """,lookback=" & lookback & ",op=" & op & ",edgeCaseHandling=" & edgeCaseHandling + (If(edgeCaseHandling = EdgeCaseHandling.SpecifiedValue, ",edgeCaseValue=" & edgeCaseValue, "")) & ")"
		End Function

		''' <summary>
		''' The output column name
		''' after the operation has been applied
		''' </summary>
		''' <returns> the output column name </returns>
		Public Overridable Function outputColumnName() As String
			Return outputColumnNames()(0)
		End Function

		''' <summary>
		''' The output column names
		''' This will often be the same as the input
		''' </summary>
		''' <returns> the output column names </returns>
		Public Overridable Function outputColumnNames() As String()
			Return columnNames()
		End Function

		''' <summary>
		''' Returns column names
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnNames() As String()
			Return InputSchema.getColumnNames().toArray(New String(InputSchema.numColumns() - 1){})
		End Function

		''' <summary>
		''' Returns a singular column name
		''' this op is meant to run on
		''' 
		''' @return
		''' </summary>
		Public Overridable Function columnName() As String
			Return columnNames()(0)
		End Function

	End Class

End Namespace