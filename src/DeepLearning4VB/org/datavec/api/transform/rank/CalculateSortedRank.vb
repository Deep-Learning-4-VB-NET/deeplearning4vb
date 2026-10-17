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
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports WritableComparator = DeepLearning4VB.org.datavec.api.writable.comparator.WritableComparator

Namespace org.datavec.api.transform.rank

    ''' <summary>
    ''' CalculateSortedRank: calculates the rank of each example, after sorting the examples on the specified column
    ''' with the specified comparator. The rank (0 for the first example after sorting, 1 for the second and so on) is
    ''' added as a new (long) column at the end. This class only describes the operation (output schema, column names);
    ''' the sorting itself is done by the executor that runs the transform process.
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"inputSchema"})</c>) compares the column names, the
    ''' comparator and the sort order.
    ''' </summary>
    ' TODO(serde): polymorphic JSON attributes added in the serde wave
    ' (Java: @JsonIgnoreProperties({"inputSchema"}) @JsonInclude(NON_NULL)
    '  @JsonTypeInfo(use = CLASS, include = PROPERTY, property = "@class"))
    <Serializable>
    Public Class CalculateSortedRank
        Implements ColumnOp

        Private ReadOnly newColumnName As String
        Private ReadOnly sortOnColumn As String
        Private ReadOnly comparator As WritableComparator
        Private ReadOnly ascending As Boolean
        Private inputSchemaField As SchemaT

        ''' <param name="newColumnName">    Name of the new column (will contain the rank for each example) </param>
        ''' <param name="sortOnColumn">     Name of the column to sort on </param>
        ''' <param name="comparator">       Comparator used to sort examples </param>
        Public Sub New(ByVal newColumnName As String, ByVal sortOnColumn As String, ByVal comparator As WritableComparator)
            Me.New(newColumnName, sortOnColumn, comparator, True)
        End Sub

        ''' <param name="newColumnName">    Name of the new column (will contain the rank for each example) </param>
        ''' <param name="sortOnColumn">     Name of the column to sort on </param>
        ''' <param name="comparator">       Comparator used to sort examples </param>
        ''' <param name="ascending">        Whether examples should be ascending or descending, using the comparator </param>
        <JsonConstructor>
        Public Sub New(ByVal newColumnName As String, ByVal sortOnColumn As String, ByVal comparator As WritableComparator, ByVal ascending As Boolean)
            Me.newColumnName = newColumnName
            Me.sortOnColumn = sortOnColumn
            Me.comparator = comparator
            Me.ascending = ascending
        End Sub

        ''' <summary>Java <c>getNewColumnName()</c> (Lombok).</summary>
        Public Function getNewColumnName() As String
            Return newColumnName
        End Function

        ''' <summary>Java <c>getSortOnColumn()</c> (Lombok).</summary>
        Public Function getSortOnColumn() As String
            Return sortOnColumn
        End Function

        ''' <summary>Java <c>getComparator()</c> (Lombok).</summary>
        Public Function getComparator() As WritableComparator
            Return comparator
        End Function

        ''' <summary>Java <c>isAscending()</c> (Lombok).</summary>
        Public Function isAscending() As Boolean
            Return ascending
        End Function

        ''' <summary>
        ''' The input schema plus a long column (minimum 0) named <c>newColumnName</c>.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">for a sequence schema (not supported)</exception>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            If TypeOf inputSchema Is SequenceSchemaT Then
                Throw New InvalidOperationException("Calculating sorted rank on sequences: not yet supported")
            End If

            Dim origMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
            Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(origMeta)

            newMeta.Add(New LongMetaData(newColumnName, 0L, Nothing))

            Return inputSchema.newSchema(newMeta)
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
            End Set
        End Property

        ''' <summary>Java <c>getInputSchema()</c> (Lombok).</summary>
        Public Function getInputSchema() As SchemaT
            Return inputSchemaField
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return newColumnName
        End Function

        ''' <summary>
        ''' The output column names: the input column names plus the new column
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Dim names As IList(Of String) = inputSchemaField.getColumnNames()
            names.Add(newColumnName)
            Return New List(Of String)(names).ToArray()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return New List(Of String)(inputSchemaField.getColumnNames()).ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

        Public Overrides Function ToString() As String
            Return "CalculateSortedRank(newColumnName=""" & newColumnName & """, comparator=" & Objects.toString(comparator) & ")"
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As CalculateSortedRank = DirectCast(obj, CalculateSortedRank)
            Return String.Equals(newColumnName, other.newColumnName) AndAlso String.Equals(sortOnColumn, other.sortOnColumn) AndAlso
                Object.Equals(comparator, other.comparator) AndAlso ascending = other.ascending
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(newColumnName, sortOnColumn, comparator, ascending)
        End Function

    End Class

End Namespace
