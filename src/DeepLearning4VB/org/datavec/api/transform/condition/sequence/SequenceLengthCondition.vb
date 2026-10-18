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
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.condition.sequence

    ''' <summary>
    ''' A condition on the length (number of steps) of a sequence: <c>op</c> compares the length with
    ''' <c>length</c>, or (for <c>InSet</c>/<c>NotInSet</c>) tests membership of <c>set</c>.
    ''' It cannot be applied to single (non-sequence) examples.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"}) @EqualsAndHashCode(exclude = {"inputSchema"}) @JsonInclude(NON_NULL) @Data
    <JsonIgnoreProperties("inputSchema")>
    <JsonIncludeNonNull>
    <Serializable>
    Public Class SequenceLengthCondition
        Implements ConditionT

        Private ReadOnly op As ConditionOp
        Private ReadOnly length As Integer?
        Private ReadOnly [set] As ISet(Of Integer)
        Private inputSchemaField As SchemaT

        Public Sub New(ByVal op As ConditionOp, ByVal length As Integer)
            Me.New(op, CType(length, Integer?), Nothing)
        End Sub

        Public Sub New(ByVal op As ConditionOp, ByVal [set] As ISet(Of Integer))
            Me.New(op, Nothing, [set])
        End Sub

        ''' <summary>Java's private JSON constructor (properties "op", "length", "set").</summary>
        Private Sub New(ByVal op As ConditionOp, ByVal length As Integer?, ByVal [set] As ISet(Of Integer))
            If [set] IsNot Nothing AndAlso op <> ConditionOp.InSet AndAlso op <> ConditionOp.NotInSet Then
                Throw New ArgumentException("Invalid condition op: can only use this constructor with InSet or NotInSet ops")
            End If
            Me.op = op
            Me.length = length
            Me.set = [set]
        End Sub

        ''' <summary>Java <c>getOp()</c> (Lombok).</summary>
        Public Function getOp() As ConditionOp
            Return op
        End Function

        ''' <summary>Java <c>getLength()</c> (Lombok).</summary>
        Public Function getLength() As Integer?
            Return length
        End Function

        ''' <summary>Java <c>getSet()</c> (Lombok).</summary>
        Public Function getSet() As ISet(Of Integer)
            Return [set]
        End Function

        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Return inputSchema 'No op
        End Function

        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return inputSchemaField.getColumnNames()(0)
        End Function

        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return inputSchemaField.getColumnNames().ToArray()
        End Function

        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return outputColumnNames()
        End Function

        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return outputColumnName()
        End Function

        Public Overridable Function condition(ByVal list As IList(Of WritableT)) As Boolean Implements ConditionT.condition
            Throw New NotSupportedException("Cannot apply SequenceLengthCondition on non-sequence data")
        End Function

        Public Overridable Function condition(ByVal input As Object) As Boolean Implements ConditionT.condition
            Throw New NotSupportedException("Cannot apply SequenceLengthCondition on non-sequence data")
        End Function

        Public Overridable Function conditionSequence(ByVal sequence As IList(Of IList(Of WritableT))) As Boolean Implements ConditionT.conditionSequence
            Return op.apply(sequence.Count, If(length.HasValue, length.Value, 0), [set])
        End Function

        Public Overridable Function conditionSequence(ByVal sequence As Object) As Boolean Implements ConditionT.conditionSequence
            Throw New NotSupportedException("Not yet implemented")
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As SequenceLengthCondition = TryCast(obj, SequenceLengthCondition)
            If other Is Nothing Then Return False
            If op <> other.op OrElse Not Nullable.Equals(length, other.length) Then Return False
            If [set] Is Nothing OrElse other.set Is Nothing Then Return [set] Is other.set
            Return [set].SetEquals(other.set)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = Objects.hash(op, length)
            If [set] IsNot Nothing Then
                For Each v As Integer In [set]
                    h = h Xor v.GetHashCode()
                Next
            End If
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "SequenceLengthCondition(op=" & op.ToString() & ", length=" & If(length.HasValue, length.Value.ToString(), "null") &
                   ", set=" & If([set] Is Nothing, "null", "[" & String.Join(", ", [set]) & "]") & ")"
        End Function

    End Class

End Namespace
