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
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.filter

    ''' <summary>Removes examples (and sequences containing a step) whose number of columns differs from the schema's.</summary>
    ' Java: @Data @AllArgsConstructor
    <Serializable>
    Public Class InvalidNumColumns
        Implements Filter

        Private inputSchemaField As SchemaT

        Public Sub New()
        End Sub

        Public Sub New(ByVal inputSchema As SchemaT)
            Me.inputSchemaField = inputSchema
        End Sub

        ''' <param name="writables"> Example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeExample(ByVal writables As Object) As Boolean Implements Filter.removeExample
            Return False
        End Function

        ''' <param name="sequence"> sequence example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeSequence(ByVal sequence As Object) As Boolean Implements Filter.removeSequence
            Return False
        End Function

        ''' <param name="writables"> Example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeExample(ByVal writables As IList(Of WritableT)) As Boolean Implements Filter.removeExample
            Return writables.Count <> inputSchemaField.numColumns()
        End Function

        ''' <param name="sequence"> sequence example </param>
        ''' <returns> true if example should be removed, false to keep </returns>
        Public Overridable Function removeSequence(ByVal sequence As IList(Of IList(Of WritableT))) As Boolean Implements Filter.removeSequence
            For Each record As IList(Of WritableT) In sequence
                If record.Count <> inputSchemaField.numColumns() Then
                    Return True
                End If
            Next record
            Return False
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
                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
            Return inputSchemaField.getColumnNames()(0)
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
            Return inputSchemaField.getColumnNames().ToArray()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
            Return inputSchemaField.getColumnNames().ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overridable Function columnName() As String Implements ColumnOp.columnName
            Return columnNames()(0)
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As InvalidNumColumns = TryCast(obj, InvalidNumColumns)
            Return other IsNot Nothing AndAlso Object.Equals(inputSchemaField, other.inputSchemaField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return If(inputSchemaField Is Nothing, 0, inputSchemaField.GetHashCode())
        End Function

        Public Overrides Function ToString() As String
            Return "InvalidNumColumns(inputSchema=" & If(inputSchemaField Is Nothing, "null", inputSchemaField.ToString()) & ")"
        End Function

    End Class

End Namespace
