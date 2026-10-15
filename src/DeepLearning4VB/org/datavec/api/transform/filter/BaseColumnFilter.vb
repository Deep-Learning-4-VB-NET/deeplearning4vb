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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.filter

    ''' <summary>
    ''' Base class for filters that look at a single column: resolves the column index from the input schema
    ''' and applies <see cref="removeExample(WritableT)"/> to that column of each example (a sequence is removed
    ''' when any of its steps is). The remaining <see cref="Filter"/>/<see cref="ColumnOp"/> members are left
    ''' abstract, as in Java.
    ''' </summary>
    <Serializable>
    Public MustInherit Class BaseColumnFilter
        Implements Filter

        Protected Friend schemaField As SchemaT
        Protected Friend ReadOnly column As String
        Protected Friend columnIdx As Integer

        Protected Friend Sub New(ByVal column As String)
            Me.column = column
        End Sub

        Public Overridable Overloads Function removeExample(ByVal writables As IList(Of WritableT)) As Boolean Implements Filter.removeExample
            Return removeExample(writables(columnIdx))
        End Function

        Public Overridable Overloads Function removeSequence(ByVal sequence As IList(Of IList(Of WritableT))) As Boolean Implements Filter.removeSequence
            For Each c As IList(Of WritableT) In sequence
                If removeExample(c) Then
                    Return True
                End If
            Next c
            Return False
        End Function

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                Me.schemaField = value
                Me.columnIdx = value.getIndexOfColumn(column)
            End Set
            Get
                Return schemaField
            End Get
        End Property

        ''' <summary>
        ''' Should the example or sequence be removed, based on the values from the specified column? </summary>
        Public MustOverride Overloads Function removeExample(ByVal writable As WritableT) As Boolean

        Public MustOverride Overloads Function removeExample(ByVal example As Object) As Boolean Implements Filter.removeExample

        Public MustOverride Overloads Function removeSequence(ByVal sequence As Object) As Boolean Implements Filter.removeSequence

        Public MustOverride Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform

        Public MustOverride Function outputColumnName() As String Implements ColumnOp.outputColumnName

        Public MustOverride Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames

        Public MustOverride Function columnNames() As String() Implements ColumnOp.columnNames

        Public MustOverride Function columnName() As String Implements ColumnOp.columnName

    End Class

End Namespace
