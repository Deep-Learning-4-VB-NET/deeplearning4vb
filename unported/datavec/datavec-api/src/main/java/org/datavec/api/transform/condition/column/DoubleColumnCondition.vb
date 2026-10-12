Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports SequenceConditionMode = org.datavec.api.transform.condition.SequenceConditionMode
Imports Writable = org.datavec.api.writable.Writable
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

Namespace org.datavec.api.transform.condition.column


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class DoubleColumnCondition extends BaseColumnCondition
	<Serializable>
	Public Class DoubleColumnCondition
		Inherits BaseColumnCondition

		Private ReadOnly op As ConditionOp
		Private ReadOnly value As Double?
		Private ReadOnly set As ISet(Of Double)

		''' <summary>
		''' Constructor for operations such as less than, equal to, greater than, etc.
		''' Uses default sequence condition mode, <seealso cref="BaseColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE"/>
		''' </summary>
		''' <param name="columnName"> Column to check for the condition </param>
		''' <param name="op">         Operation (<, >=, !=, etc) </param>
		''' <param name="value">      Value to use in the condition </param>
		Public Sub New(ByVal columnName As String, ByVal op As ConditionOp, ByVal value As Double)
			Me.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE, op, value)
		End Sub

		''' <summary>
		''' Constructor for operations such as less than, equal to, greater than, etc.
		''' </summary>
		''' <param name="column">                Column to check for the condition </param>
		''' <param name="sequenceConditionMode"> Mode for handling sequence data </param>
		''' <param name="op">                    Operation (<, >=, !=, etc) </param>
		''' <param name="value">                 Value to use in the condition </param>
		Public Sub New(ByVal column As String, ByVal sequenceConditionMode As SequenceConditionMode, ByVal op As ConditionOp, ByVal value As Double)
			MyBase.New(column, sequenceConditionMode)
			If op = ConditionOp.InSet OrElse op = ConditionOp.NotInSet Then
				Throw New System.ArgumentException("Invalid condition op: cannot use this constructor with InSet or NotInSet ops")
			End If
			Me.op = op
			Me.value = value
			Me.set = Nothing
		End Sub

		''' <summary>
		''' Constructor for operations: ConditionOp.InSet, ConditionOp.NotInSet.
		''' Uses default sequence condition mode, <seealso cref="BaseColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE"/>
		''' </summary>
		''' <param name="column"> Column to check for the condition </param>
		''' <param name="op">     Operation. Must be either ConditionOp.InSet, ConditionOp.NotInSet </param>
		''' <param name="set">    Set to use in the condition </param>
		Public Sub New(ByVal column As String, ByVal op As ConditionOp, ByVal set As ISet(Of Double))
			Me.New(column, DEFAULT_SEQUENCE_CONDITION_MODE, op, set)
		End Sub

		''' <summary>
		''' Constructor for operations: ConditionOp.InSet, ConditionOp.NotInSet
		''' </summary>
		''' <param name="column">                Column to check for the condition </param>
		''' <param name="sequenceConditionMode"> Mode for handling sequence data </param>
		''' <param name="op">                    Operation. Must be either ConditionOp.InSet, ConditionOp.NotInSet </param>
		''' <param name="set">                   Set to use in the condition </param>
		Public Sub New(ByVal column As String, ByVal sequenceConditionMode As SequenceConditionMode, ByVal op As ConditionOp, ByVal set As ISet(Of Double))
			MyBase.New(column, sequenceConditionMode)
			If op <> ConditionOp.InSet AndAlso op <> ConditionOp.NotInSet Then
				Throw New System.ArgumentException("Invalid condition op: can ONLY use this constructor with InSet or NotInSet ops")
			End If
			Me.op = op
			Me.value = Nothing
			Me.set = set
		End Sub

		'Private constructor for Jackson deserialization only
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private DoubleColumnCondition(@JsonProperty("columnName") String columnName, @JsonProperty("op") org.datavec.api.transform.condition.ConditionOp op, @JsonProperty("value") double value, @JsonProperty("set") java.util.@Set<Double> set)
		Private Sub New(ByVal columnName As String, ByVal op As ConditionOp, ByVal value As Double, ByVal set As ISet(Of Double))
			MyBase.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE)
			Me.op = op
			Me.value = (If(set Is Nothing, value, Nothing))
			Me.set = set
		End Sub


		Public Overrides Function columnCondition(ByVal writable As Writable) As Boolean
			Return op.apply(writable.toDouble(), (If(value Is Nothing, Double.NaN, value)), set)
		End Function

		Public Overrides Function ToString() As String
			Return "DoubleColumnCondition(columnName=""" & columnName_Conflict & """," & op & "," & (If(op = ConditionOp.NotInSet OrElse op = ConditionOp.InSet, set, value)) & ")"
		End Function

		''' <summary>
		''' Condition on arbitrary input
		''' </summary>
		''' <param name="input"> the input to return
		'''              the condition for </param>
		''' <returns> true if the condition is met
		''' false otherwise </returns>
		Public Overrides Function condition(ByVal input As Object) As Boolean
			Dim d As Number = DirectCast(input, Number)
			Return op.apply(d.doubleValue(), (If(value Is Nothing, Double.NaN, value)), set)
		End Function

	End Class

End Namespace