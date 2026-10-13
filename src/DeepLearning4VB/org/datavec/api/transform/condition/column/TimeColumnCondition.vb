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

Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.JavaCompat

Namespace org.datavec.api.transform.condition.column

    ''' <summary>Condition on a time column (epoch milliseconds): a comparison with a value, or in / not in a set of values.</summary>
    <Serializable>
    Public Class TimeColumnCondition
        Inherits BaseColumnCondition

        Private ReadOnly op As ConditionOp
        Private ReadOnly value As Long?
        Private ReadOnly [set] As ISet(Of Long)

        ''' <summary>
        ''' Constructor for operations such as less than, equal to, greater than, etc.
        ''' Uses default sequence condition mode, <seealso cref="BaseColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE"/>
        ''' </summary>
        ''' <param name="columnName"> Column to check for the condition </param>
        ''' <param name="op">         Operation (&lt;, &gt;=, !=, etc) </param>
        ''' <param name="value">      Time value (in epoch millisecond format) to use in the condition </param>
        Public Sub New(ByVal columnName As String, ByVal op As ConditionOp, ByVal value As Long)
            Me.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE, op, value)
        End Sub

        ''' <summary>
        ''' Constructor for operations such as less than, equal to, greater than, etc.
        ''' </summary>
        ''' <param name="column">                Column to check for the condition </param>
        ''' <param name="sequenceConditionMode"> Mode for handling sequence data </param>
        ''' <param name="op">                    Operation (&lt;, &gt;=, !=, etc) </param>
        ''' <param name="value">                 Time value (in epoch millisecond format) to use in the condition </param>
        Public Sub New(ByVal column As String, ByVal sequenceConditionMode As SequenceConditionMode, ByVal op As ConditionOp, ByVal value As Long)
            MyBase.New(column, sequenceConditionMode)
            If op = ConditionOp.InSet OrElse op = ConditionOp.NotInSet Then
                Throw New ArgumentException("Invalid condition op: cannot use this constructor with InSet or NotInSet ops")
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
        Public Sub New(ByVal column As String, ByVal op As ConditionOp, ByVal [set] As ISet(Of Long))
            Me.New(column, DEFAULT_SEQUENCE_CONDITION_MODE, op, [set])
        End Sub

        ''' <summary>
        ''' Constructor for operations: ConditionOp.InSet, ConditionOp.NotInSet
        ''' </summary>
        ''' <param name="column">                Column to check for the condition </param>
        ''' <param name="sequenceConditionMode"> Mode for handling sequence data </param>
        ''' <param name="op">                    Operation. Must be either ConditionOp.InSet, ConditionOp.NotInSet </param>
        ''' <param name="set">                   Set to use in the condition </param>
        Public Sub New(ByVal column As String, ByVal sequenceConditionMode As SequenceConditionMode, ByVal op As ConditionOp, ByVal [set] As ISet(Of Long))
            MyBase.New(column, sequenceConditionMode)
            If op <> ConditionOp.InSet AndAlso op <> ConditionOp.NotInSet Then
                Throw New ArgumentException("Invalid condition op: can ONLY use this constructor with InSet or NotInSet ops")
            End If
            Me.op = op
            Me.value = Nothing
            Me.set = [set]
        End Sub

        'Private constructor for Jackson deserialization only
        <JsonConstructor>
        Private Sub New(ByVal columnName As String, ByVal op As ConditionOp, ByVal value As Long, ByVal [set] As ISet(Of Long))
            MyBase.New(columnName, DEFAULT_SEQUENCE_CONDITION_MODE)
            Me.op = op
            Me.value = If([set] Is Nothing, CType(value, Long?), Nothing)
            Me.set = [set]
        End Sub

        Public Function getOp() As ConditionOp
            Return op
        End Function

        Public Function getValue() As Long?
            Return value
        End Function

        Public Function getSet() As ISet(Of Long)
            Return [set]
        End Function

        Public Overrides Function columnCondition(ByVal writable As writable.Writable) As Boolean
            Return op.apply(writable.toLong(), value.GetValueOrDefault(), [set])
        End Function

        Public Overrides Function ToString() As String
            Return "TimeColumnCondition(columnName=""" & columnNameField & """," & op.ToString() & "," & (If(op = ConditionOp.NotInSet OrElse op = ConditionOp.InSet, setToString(), valueToString())) & ")"
        End Function

        Private Function setToString() As String
            If [set] Is Nothing Then Return "null"
            Return "[" & String.Join(", ", [set]) & "]"
        End Function

        Private Function valueToString() As String
            Return If(value.HasValue, value.Value.ToString(), "null")
        End Function

        ''' <summary>
        ''' Condition on arbitrary input
        ''' </summary>
        ''' <param name="input"> the input to return
        '''              the condition for </param>
        ''' <returns> true if the condition is met
        ''' false otherwise </returns>
        Public Overrides Function condition(ByVal input As Object) As Boolean
            Return op.apply(JavaNumbers.longValue(input), value.GetValueOrDefault(), [set])
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other = DirectCast(obj, TimeColumnCondition)
            Return op = other.op AndAlso Nullable.Equals(value, other.value) AndAlso setsEqual([set], other.set)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 31 * Objects.hash(columnNameField, op, value) + setHash([set])
        End Function
    End Class

End Namespace
