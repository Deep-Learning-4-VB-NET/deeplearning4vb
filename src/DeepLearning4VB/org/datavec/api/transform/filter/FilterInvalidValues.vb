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
Imports System.Linq
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.writable
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.filter

    ''' <summary>
    ''' Removes examples (and sequences) that contain a value which is invalid according to the column
    ''' metadata of the input schema, either in any column or only in the named columns.
    ''' </summary>
    ' Java: @EqualsAndHashCode(exclude = {"schema", "columnIdxs"}) @JsonIgnoreProperties({"schema", "columnIdxs"}) @Data @ToString(exclude = {"schema", "columnIdxs"})
    <JsonIgnoreProperties("schema", "columnIdxs")>
    <Serializable>
    Public Class FilterInvalidValues
        Implements Filter

        Private schemaField As SchemaT
        Private ReadOnly filterAnyInvalid As Boolean
        Private ReadOnly columnsToFilterIfInvalid() As String
        Private columnIdxs() As Integer

        ''' <summary>
        ''' Filter examples that have invalid values in ANY columns. </summary>
        Public Sub New()
            filterAnyInvalid = True
            columnsToFilterIfInvalid = Nothing
        End Sub

        ''' <param name="columnsToFilterIfInvalid"> Columns to check for invalid values </param>
        Public Sub New(ByVal ParamArray columnsToFilterIfInvalid() As String)
            If columnsToFilterIfInvalid Is Nothing OrElse columnsToFilterIfInvalid.Length = 0 Then
                Throw New ArgumentException("Cannot filter 0/null columns: columns to filter on must be specified")
            End If
            Me.columnsToFilterIfInvalid = columnsToFilterIfInvalid
            filterAnyInvalid = False
        End Sub

        ''' <summary>Java <c>isFilterAnyInvalid()</c> (Lombok).</summary>
        Public Function isFilterAnyInvalid() As Boolean
            Return filterAnyInvalid
        End Function

        ''' <summary>Java <c>getColumnsToFilterIfInvalid()</c> (Lombok).</summary>
        Public Function getColumnsToFilterIfInvalid() As String()
            Return columnsToFilterIfInvalid
        End Function

        ''' <summary>Java <c>getSchema()</c> (Lombok).</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        ''' <summary>Java <c>getColumnIdxs()</c> (Lombok).</summary>
        Public Function getColumnIdxs() As Integer()
            Return columnIdxs
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
                Me.schemaField = value
                If Not filterAnyInvalid Then
                    Me.columnIdxs = New Integer(columnsToFilterIfInvalid.Length - 1) {}
                    For i As Integer = 0 To columnsToFilterIfInvalid.Length - 1
                        Me.columnIdxs(i) = value.getIndexOfColumn(columnsToFilterIfInvalid(i))
                    Next i
                End If
            End Set
            Get
                Return schemaField
            End Get
        End Property

        ''' <param name="writables"> Example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeExample(ByVal writables As Object) As Boolean Implements Filter.removeExample
            Dim row As System.Collections.IList = DirectCast(writables, System.Collections.IList)
            If Not filterAnyInvalid Then
                'Filter only on specific columns
                For Each i As Integer In columnIdxs
                    If filterColumn(row, i) Then
                        Return True 'Remove if not valid
                    End If
                Next i
            Else
                'Filter on ALL columns
                Dim nCols As Integer = schemaField.numColumns()
                For i As Integer = 0 To nCols - 1
                    If filterColumn(row, i) Then
                        Return True
                    End If
                Next i
            End If
            Return False
        End Function

        Private Function filterColumn(ByVal row As System.Collections.IList, ByVal i As Integer) As Boolean
            Dim meta As ColumnMetaData = schemaField.getMetaData(i)
            Dim v As Object = row(i)
            If TypeOf v Is Single Then
                If Not meta.isValid(New FloatWritable(CSng(v))) Then Return True
            ElseIf TypeOf v Is Double Then
                If Not meta.isValid(New DoubleWritable(CDbl(v))) Then Return True
            ElseIf TypeOf v Is String Then
                If Not meta.isValid(New Text(CStr(v))) Then Return True
            ElseIf TypeOf v Is Integer Then
                If Not meta.isValid(New IntWritable(CInt(v))) Then Return True
            ElseIf TypeOf v Is Long Then
                If Not meta.isValid(New LongWritable(CLng(v))) Then Return True
            ElseIf TypeOf v Is Boolean Then
                If Not meta.isValid(New BooleanWritable(CBool(v))) Then Return True
            End If
            Return False
        End Function

        ''' <param name="sequence"> sequence example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeSequence(ByVal sequence As Object) As Boolean Implements Filter.removeSequence
            Dim seq As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            'If _any_ of the values are invalid, remove the entire sequence
            For Each c As Object In seq
                If removeExample(c) Then
                    Return True
                End If
            Next c
            Return False
        End Function

        Public Overridable Function removeExample(ByVal writables As IList(Of WritableT)) As Boolean Implements Filter.removeExample
            If writables.Count <> schemaField.numColumns() Then
                Return True
            End If

            If Not filterAnyInvalid Then
                'Filter only on specific columns
                For Each i As Integer In columnIdxs
                    Dim meta As ColumnMetaData = schemaField.getMetaData(i)
                    If Not meta.isValid(writables(i)) Then
                        Return True 'Remove if not valid
                    End If
                Next i
            Else
                'Filter on ALL columns
                Dim nCols As Integer = schemaField.numColumns()
                For i As Integer = 0 To nCols - 1
                    Dim meta As ColumnMetaData = schemaField.getMetaData(i)
                    If Not meta.isValid(writables(i)) Then
                        Return True 'Remove if not valid
                    End If
                Next i
            End If
            Return False
        End Function

        Public Overridable Function removeSequence(ByVal sequence As IList(Of IList(Of WritableT))) As Boolean Implements Filter.removeSequence
            'If _any_ of the values are invalid, remove the entire sequence
            For Each c As IList(Of WritableT) In sequence
                If removeExample(c) Then
                    Return True
                End If
            Next c
            Return False
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return outputColumnNames()(0)
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
            Return schemaField.getColumnNames().ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As FilterInvalidValues = TryCast(obj, FilterInvalidValues)
            If other Is Nothing Then Return False
            If filterAnyInvalid <> other.filterAnyInvalid Then Return False
            If columnsToFilterIfInvalid Is Nothing OrElse other.columnsToFilterIfInvalid Is Nothing Then
                Return columnsToFilterIfInvalid Is other.columnsToFilterIfInvalid
            End If
            Return columnsToFilterIfInvalid.SequenceEqual(other.columnsToFilterIfInvalid)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = filterAnyInvalid.GetHashCode()
            If columnsToFilterIfInvalid IsNot Nothing Then
                For Each c As String In columnsToFilterIfInvalid
                    h = h * 31 + If(c Is Nothing, 0, c.GetHashCode())
                Next
            End If
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "FilterInvalidValues(filterAnyInvalid=" & filterAnyInvalid.ToString().ToLowerInvariant() & ", columnsToFilterIfInvalid=" &
                   If(columnsToFilterIfInvalid Is Nothing, "null", "[" & String.Join(", ", columnsToFilterIfInvalid) & "]") & ")"
        End Function

    End Class

End Namespace
