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
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.filter

    ''' <summary>A filter that removes every example (or sequence) for which the wrapped <see cref="ConditionT"/> holds.</summary>
    ' Java: @EqualsAndHashCode @Data; JSON property "condition".
    <Serializable>
    Public Class ConditionFilter
        Implements Filter

        Private ReadOnly cond As ConditionT

        Public Sub New(ByVal condition As ConditionT)
            If condition Is Nothing Then Throw New ArgumentNullException("condition")
            Me.cond = condition
        End Sub

        ''' <summary>Java <c>getCondition()</c> (Lombok).</summary>
        Public Function getCondition() As ConditionT
            Return cond
        End Function

        ''' <param name="writables"> Example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeExample(ByVal writables As Object) As Boolean Implements Filter.removeExample
            Return cond.condition(writables)
        End Function

        ''' <param name="sequence"> sequence example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeSequence(ByVal sequence As Object) As Boolean Implements Filter.removeSequence
            Return cond.condition(sequence)
        End Function

        Public Overridable Function removeExample(ByVal writables As IList(Of WritableT)) As Boolean Implements Filter.removeExample
            Return cond.condition(writables)
        End Function

        Public Overridable Function removeSequence(ByVal sequence As IList(Of IList(Of WritableT))) As Boolean Implements Filter.removeSequence
            Return cond.conditionSequence(sequence)
        End Function

        ''' <summary>
        ''' Get the output schema for this transformation, given an input schema
        ''' </summary>
        ''' <param name="inputSchema"> </param>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Return inputSchema
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                cond.InputSchema = value
            End Set
            Get
                Return cond.InputSchema
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return "ConditionFilter(" & cond.ToString() & ")"
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return cond.outputColumnName()
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return cond.outputColumnNames()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return cond.columnNames()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return cond.columnName()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As ConditionFilter = TryCast(obj, ConditionFilter)
            Return other IsNot Nothing AndAlso cond.Equals(other.cond)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 59 + cond.GetHashCode()
        End Function

    End Class

End Namespace
