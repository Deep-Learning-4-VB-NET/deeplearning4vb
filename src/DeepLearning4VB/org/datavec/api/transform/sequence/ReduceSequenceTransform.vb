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
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IAssociativeReducer = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Reduce a sequence to a sequence of length 1 (one time step) with an <see cref="IAssociativeReducer"/>.
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"inputSchema"})</c>) compares the reducer.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"})
    <Serializable>
    Public Class ReduceSequenceTransform
        Implements TransformT

        Private reducer As IAssociativeReducer
        Private inputSchemaField As SchemaT

        <JsonConstructor>
        Public Sub New(ByVal reducer As IAssociativeReducer)
            Me.reducer = reducer
        End Sub

        ''' <summary>Java <c>getReducer()</c> (Lombok).</summary>
        Public Function getReducer() As IAssociativeReducer
            Return reducer
        End Function

        ''' <summary>Java <c>setReducer()</c> (Lombok).</summary>
        Public Sub setReducer(ByVal reducer As IAssociativeReducer)
            Me.reducer = reducer
        End Sub

        ''' <summary>
        ''' The output schema: a sequence schema with the columns of the reducer's output schema.
        ''' </summary>
        ''' <exception cref="ArgumentException">if the input schema is not a sequence schema</exception>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            If inputSchema IsNot Nothing AndAlso Not (TypeOf inputSchema Is SequenceSchemaT) Then
                Throw New ArgumentException("Invalid input: input schema must be a SequenceSchema")
            End If

            'Approach here: The reducer gives us a schema for one time step -> simply convert this to a sequence schema...
            Dim oneStepSchema As SchemaT = reducer.transform(inputSchema)
            Dim meta As IList(Of ColumnMetaData) = oneStepSchema.getColumnMetaData()

            Return New SequenceSchemaT(meta)
        End Function

        ''' <summary>The input schema; setting it also sets the reducer's input schema.</summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
                reducer.InputSchema = value
            End Set
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("ReduceSequenceTransform can only be applied on sequences")
        End Function

        ''' <summary>Reduces the sequence to one time step.</summary>
        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim accu As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
            For Each l As IList(Of WritableT) In sequence
                accu.accept(l)
            Next l
            Return JavaCollections.singletonList(accu.get())
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("ReduceSequenceTransform can only be applied on sequences")
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Needs to be implemented")
        End Function

        Public Overrides Function ToString() As String
            Return "ReduceSequenceTransform(reducer=" & Objects.toString(reducer) & ")"
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
            Return New List(Of String)(InputSchema.getColumnNames()).ToArray()
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
            Return Object.Equals(reducer, DirectCast(obj, ReduceSequenceTransform).reducer)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 59 + Objects.hashCode(reducer)
        End Function

    End Class

End Namespace
