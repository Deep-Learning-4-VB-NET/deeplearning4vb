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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class IntegerColumnCondition extends BaseColumnCondition
	<Serializable>
	Public Class IntegerColumnCondition
		Inherits BaseColumnCondition

		Private ReadOnly op As ConditionOp
		Private ReadOnly value As Integer?
		Private ReadOnly set As ISet(Of Integer)

		''' <summary>
		''' Constructor for operations such as less than, equal to, greater than, etc.
		''' Uses default sequence condition mode, <seealso cref="BaseColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE"/>
		''' </summary>
		''' <param name="columnName"> Column to check for the condition </param>
		''' <param name="op">         Operation (<, >=, !=, etc) </param>
		''' <param name="value">      Value to use in the condition </param>
		Public Sub New(ByVal columnName As String, ByVal op As ConditionOp, ByVal value As Integer)
			Me.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE, op, value)
		End Sub

		''' <summary>
		''' Constructor for operations such as less than, equal to, greater than, etc.
		''' </summary>
		''' <param name="column">                Column to check for the condition </param>
		''' <param name="sequenceConditionMode"> Mode for handling sequence data </param>
		''' <param name="op">                    Operation (<, >=, !=, etc) </param>
		''' <param name="value">                 Value to use in the condition </param>
		Public Sub New(ByVal column As String, ByVal sequenceConditionMode As SequenceConditionMode, ByVal op As ConditionOp, ByVal value As Integer)
			MyBase.New(column, sequenceConditionMode)
			If op = ConditionOp.InSet OrElse op = ConditionOp.NotInSet Then
				Throw New System.ArgumentException("Invalid condition op: cannot use this constructor with InSet or NotInSet ops")
			End If
			Me.op = op
			Me.value = value
			Me.set = Nothing
		End Sub

		''' <summary>
		''' Constructor for operations: ConditionOp.InSet, ConditionOp.NotInSet
		''' Uses default sequence condition mode, <seealso cref="BaseColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE"/>
		''' </summary>
		''' <param name="column"> Column to check for the condition </param>
		''' <param name="op">     Operation. Must be either ConditionOp.InSet, ConditionOp.NotInSet </param>
		''' <param name="set">    Set to use in the condition </param>
		Public Sub New(ByVal column As String, ByVal op As ConditionOp, ByVal set As ISet(Of Integer))
			Me.New(column, DEFAULT_SEQUENCE_CONDITION_MODE, op, set)
		End Sub

		''' <summary>
		''' Constructor for operations: ConditionOp.InSet, ConditionOp.NotInSet
		''' </summary>
		''' <param name="column">                Column to check for the condition </param>
		''' <param name="sequenceConditionMode"> Mode for handling sequence data </param>
		''' <param name="op">                    Operation. Must be either ConditionOp.InSet, ConditionOp.NotInSet </param>
		''' <param name="set">                   Set to use in the condition </param>
		Public Sub New(ByVal column As String, ByVal sequenceConditionMode As SequenceConditionMode, ByVal op As ConditionOp, ByVal set As ISet(Of Integer))
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
'ORIGINAL LINE: private IntegerColumnCondition(@JsonProperty("columnName") String columnName, @JsonProperty("op") org.datavec.api.transform.condition.ConditionOp op, @JsonProperty("value") System.Nullable<Integer> value, @JsonProperty("set") java.util.@Set<Integer> set)
		Private Sub New(ByVal columnName As String, ByVal op As ConditionOp, ByVal value As Integer?, ByVal set As ISet(Of Integer))
			MyBase.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE)
			Me.op = op
			Me.value = (If(set Is Nothing, value, Nothing))
			Me.set = set
		End Sub


		Public Overrides Function columnCondition(ByVal writable As Writable) As Boolean
			Select Case op.innerEnumValue
				Case ConditionOp.InnerEnum.LessThan
					Return writable.toInt() < value
				Case ConditionOp.InnerEnum.LessOrEqual
					Return writable.toInt() <= value
				Case ConditionOp.InnerEnum.GreaterThan
					Return writable.toInt() > value
				Case ConditionOp.InnerEnum.GreaterOrEqual
					Return writable.toInt() >= value
				Case ConditionOp.InnerEnum.Equal
					Return writable.toInt() = value
				Case ConditionOp.InnerEnum.NotEqual
					Return writable.toInt() <> value
				Case ConditionOp.InnerEnum.InSet
					Return set.Contains(writable.toInt())
				Case ConditionOp.InnerEnum.NotInSet
					Return Not set.Contains(writable.toInt())
				Case Else
					Throw New Exception("Unknown or not implemented op: " & op)
			End Select
		End Function

		Public Overrides Function ToString() As String
			Return "IntegerColumnCondition(columnName=""" & columnName_Conflict & """," & op & "," & (If(op = ConditionOp.NotInSet OrElse op = ConditionOp.InSet, set, value)) & ")"
		End Function

		''' <summary>
		''' Condition on arbitrary input
		''' </summary>
		''' <param name="input"> the input to return
		'''              the condition for </param>
		''' <returns> true if the condition is met
		''' false otherwise </returns>
		Public Overrides Function condition(ByVal input As Object) As Boolean
			Dim n As Number = DirectCast(input, Number)
			Select Case op.innerEnumValue
				Case ConditionOp.InnerEnum.LessThan
					Return n.intValue() < value
				Case ConditionOp.InnerEnum.LessOrEqual
					Return n.intValue() <= value
				Case ConditionOp.InnerEnum.GreaterThan
					Return n.intValue() > value
				Case ConditionOp.InnerEnum.GreaterOrEqual
					Return n.intValue() >= value
				Case ConditionOp.InnerEnum.Equal
					Return n.intValue() = value
				Case ConditionOp.InnerEnum.NotEqual
					Return n.intValue() <> value
				Case ConditionOp.InnerEnum.InSet
					Return set.Contains(n.intValue())
				Case ConditionOp.InnerEnum.NotInSet
					Return Not set.Contains(n.intValue())
				Case Else
					Throw New Exception("Unknown or not implemented op: " & op)
			End Select
		End Function
	End Class

End Namespace