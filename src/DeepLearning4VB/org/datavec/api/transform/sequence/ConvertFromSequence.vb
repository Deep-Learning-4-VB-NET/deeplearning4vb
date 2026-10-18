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

Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Splits each sequence into its individual time steps, which become (non-sequence) records. This class only
    ''' describes the conversion (output schema); the executor does the flattening. Equality (Lombok
    ''' <c>@EqualsAndHashCode(exclude = {"inputSchema"})</c>) has no fields to compare: all instances are equal.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"})
    <JsonIgnoreProperties("inputSchema")>
    Public Class ConvertFromSequence

        Private inputSchemaField As SequenceSchemaT

        Public Sub New()
        End Sub

        ''' <summary>The input sequence schema (Java <c>getInputSchema</c>/<c>setInputSchema</c>, Lombok).</summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SequenceSchemaT
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SequenceSchemaT)
                inputSchemaField = value
            End Set
        End Property

        ''' <summary>Java <c>getInputSchema()</c> (Lombok).</summary>
        Public Function getInputSchema() As SequenceSchemaT
            Return inputSchemaField
        End Function

        ''' <summary>Java <c>setInputSchema()</c> (Lombok).</summary>
        Public Sub setInputSchema(ByVal inputSchema As SequenceSchemaT)
            inputSchemaField = inputSchema
        End Sub

        ''' <summary>A (non-sequence) schema with the columns of the sequence schema.</summary>
        Public Overridable Function transform(ByVal schema As SequenceSchemaT) As SchemaT
            Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(schema.getColumnMetaData())

            Return New SchemaT(meta)
        End Function

        Public Overrides Function ToString() As String
            Return "ConvertFromSequence()"
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return obj IsNot Nothing AndAlso obj.GetType() Is Me.GetType()
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 1
        End Function

    End Class

End Namespace
