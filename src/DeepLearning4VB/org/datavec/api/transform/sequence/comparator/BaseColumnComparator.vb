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
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.comparator

    ''' <summary>
    ''' Base class of the sequence comparators that compare the time steps on the value of one column. Setting the
    ''' <see cref="Schema"/> resolves the column index (setting <see cref="InputSchema"/> only stores the schema, as in
    ''' Java). Equality (Lombok <c>@EqualsAndHashCode(exclude = {"schema", "columnIdx"})</c>) compares the class and
    ''' the column name.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"schema", "columnIdx"})
    <Serializable>
    Public MustInherit Class BaseColumnComparator
        Implements SequenceComparatorT, ColumnOp

        Protected Friend schemaField As SchemaT
        Protected Friend ReadOnly columnNameField As String
        Protected Friend columnIdx As Integer = -1

        Protected Friend Sub New(ByVal columnName As String)
            Me.columnNameField = columnName
        End Sub

        ''' <summary>Sets the sequence schema and resolves the index of the column (Java <c>setSchema</c>).</summary>
        ''' <exception cref="KeyNotFoundException">if the schema has no such column</exception>
        <JsonIgnore>
        Public Overridable WriteOnly Property Schema As SchemaT Implements SequenceComparatorT.Schema
            Set(ByVal value As SchemaT)
                Me.schemaField = value
                Me.columnIdx = value.getIndexOfColumn(columnNameField)
            End Set
        End Property

        ''' <summary>Java <c>getSchema()</c> (Lombok).</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        ''' <summary>Java <c>getColumnName()</c> (Lombok).</summary>
        Public Function getColumnName() As String
            Return columnNameField
        End Function

        ''' <summary>Java <c>getColumnIdx()</c> (Lombok).</summary>
        Public Function getColumnIdx() As Integer
            Return columnIdx
        End Function

        ''' <summary>Java <c>setColumnIdx()</c> (Lombok).</summary>
        Public Sub setColumnIdx(ByVal columnIdx As Integer)
            Me.columnIdx = columnIdx
        End Sub

        ''' <summary>
        ''' Get the output schema for this transformation, given an input schema
        ''' </summary>
        ''' <param name="inputSchema"> </param>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Return inputSchema
        End Function

        ''' <summary>
        ''' The input schema (only stored; use <see cref="Schema"/> to resolve the column index).
        ''' </summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return schemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.schemaField = value
            End Set
        End Property

        ''' <summary>Compares two time steps on the value of the column.</summary>
        Public Overridable Function Compare(ByVal o1 As IList(Of WritableT), ByVal o2 As IList(Of WritableT)) As Integer Implements IComparer(Of IList(Of WritableT)).Compare
            Return compare(o1(columnIdx), o2(columnIdx))
        End Function

        ''' <summary>Compares two values of the column.</summary>
        Protected Friend MustOverride Function compare(ByVal w1 As WritableT, ByVal w2 As WritableT) As Integer

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
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New String() {columnNameField}
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Return String.Equals(columnNameField, DirectCast(obj, BaseColumnComparator).columnNameField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 59 + Objects.hashCode(columnNameField)
        End Function

    End Class

End Namespace
