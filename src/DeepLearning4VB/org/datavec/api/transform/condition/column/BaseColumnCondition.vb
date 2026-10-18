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
Imports DeepLearning4VB.org.datavec.api.transform.serde

Namespace org.datavec.api.transform.condition.column

    ''' <summary>
    ''' Base class of the single-column conditions: resolves the column index from the input schema and applies
    ''' <see cref="columnCondition"/> to that column of each record (and to every / any record of a sequence,
    ''' according to the <see cref="SequenceConditionMode"/>). Equality (Lombok <c>@EqualsAndHashCode</c>
    ''' excluding columnIdx, schema and sequenceMode) compares the class and the column name; subclasses add their
    ''' own fields.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"columnIdx", "schema", "sequenceMode"})
    <JsonIgnoreProperties("columnIdx", "schema", "sequenceMode")>
    <Serializable>
    Public MustInherit Class BaseColumnCondition
        Implements ColumnCondition

        ''' <summary>The Java <c>ColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE</c> interface constant.</summary>
        Public Shared ReadOnly DEFAULT_SEQUENCE_CONDITION_MODE As SequenceConditionMode = SequenceConditionMode.Or

        Protected Friend ReadOnly columnNameField As String
        Protected Friend columnIdx As Integer = -1
        Protected Friend schemaField As schema.Schema
        Protected Friend sequenceMode As SequenceConditionMode

        Protected Friend Sub New(ByVal columnName As String, ByVal sequenceConditionMode As SequenceConditionMode)
            Me.columnNameField = columnName
            Me.sequenceMode = sequenceConditionMode
        End Sub

        <JsonIgnore>
        Public Overridable Property InputSchema As schema.Schema Implements ColumnOp.InputSchema
            Set(ByVal value As schema.Schema)
                columnIdx = value.getColumnNames().IndexOf(columnNameField)
                If columnIdx < 0 Then
                    Throw New InvalidOperationException("Invalid state: column """ & columnNameField & """ not present in input schema")
                End If
                Me.schemaField = value
            End Set
            Get
                Return schemaField
            End Get
        End Property

        ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
        Public Function getColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>Java <c>getColumnIdx()</c> (Lombok).</summary>
        Public Function getColumnIdx() As Integer
            Return columnIdx
        End Function

        ''' <summary>Java <c>getSchema()</c> (Lombok).</summary>
        Public Function getSchema() As schema.Schema
            Return schemaField
        End Function

        ''' <summary>Java <c>getSequenceMode()</c> (Lombok).</summary>
        Public Function getSequenceMode() As SequenceConditionMode
            Return sequenceMode
        End Function

        ''' <summary>Java <c>setSequenceMode()</c> (Lombok).</summary>
        Public Sub setSequenceMode(ByVal mode As SequenceConditionMode)
            sequenceMode = mode
        End Sub

        ''' <summary>
        ''' Get the output schema for this transformation, given an input schema
        ''' </summary>
        ''' <param name="inputSchema"> </param>
        Public Overridable Function transform(ByVal inputSchema As schema.Schema) As schema.Schema Implements Operation(Of schema.Schema, schema.Schema).transform
            Return inputSchema
        End Function

        Public Overridable Overloads Function condition(ByVal list As IList(Of writable.Writable)) As Boolean Implements Condition.condition
            Return columnCondition(list(columnIdx))
        End Function

        Public MustOverride Overloads Function condition(ByVal input As Object) As Boolean Implements Condition.condition

        Public Overridable Function conditionSequence(ByVal list As IList(Of IList(Of writable.Writable))) As Boolean Implements Condition.conditionSequence
            Select Case sequenceMode
                Case SequenceConditionMode.And
                    For Each l As IList(Of writable.Writable) In list
                        If Not condition(l) Then
                            Return False
                        End If
                    Next l
                    Return True
                Case SequenceConditionMode.Or
                    For Each l As IList(Of writable.Writable) In list
                        If condition(l) Then
                            Return True
                        End If
                    Next l
                    Return False
                Case SequenceConditionMode.NoSequenceMode
                    Throw New InvalidOperationException("Column condition " & ToString() & " does not support sequence execution")
                Case Else
                    Throw New Exception("Unknown/not implemented sequence mode: " & sequenceMode.ToString())
            End Select
        End Function

        Public Overridable Function conditionSequence(ByVal list As Object) As Boolean Implements Condition.conditionSequence
            ' Java: List<?> objects = (List<?>) list
            Dim objects As System.Collections.IList = DirectCast(list, System.Collections.IList)
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
                    Throw New InvalidOperationException("Column condition " & ToString() & " does not support sequence execution")
                Case Else
                    Throw New Exception("Unknown/not implemented sequence mode: " & sequenceMode.ToString())
            End Select
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return columnName()
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
            Return New String() {columnNameField}
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

        Public MustOverride Function columnCondition(ByVal writable As writable.Writable) As Boolean Implements ColumnCondition.columnCondition

        Public MustOverride Overrides Function ToString() As String

        ''' <summary>Equal when the other object is of the same class and has the same column name (subclasses add their fields).</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Return String.Equals(columnNameField, DirectCast(obj, BaseColumnCondition).columnNameField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(columnNameField)
        End Function

        ''' <summary>Null-safe set equality (Java <c>Set.equals</c>).</summary>
        Friend Shared Function setsEqual(Of T)(ByVal a As ISet(Of T), ByVal b As ISet(Of T)) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
            Return a.Count = b.Count AndAlso a.SetEquals(b)
        End Function

        ''' <summary>Null-safe, order independent set hash (Java <c>Set.hashCode</c>).</summary>
        Friend Shared Function setHash(Of T)(ByVal a As ISet(Of T)) As Integer
            If a Is Nothing Then Return 0
            Dim h As Integer = 0
            For Each item As T In a
                h += Objects.hashCode(item)
            Next item
            Return h
        End Function

    End Class

End Namespace
