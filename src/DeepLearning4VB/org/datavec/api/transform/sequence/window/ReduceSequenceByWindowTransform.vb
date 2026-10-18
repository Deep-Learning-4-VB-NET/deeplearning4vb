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
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IAssociativeReducer = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.window

    ''' <summary>
    ''' Idea: do two things.
    ''' First, apply a window function to the sequence data.
    ''' Second: Reduce that window of data into a single value by using a Reduce function
    ''' The result is a sequence with one time step per window. Equality (Lombok
    ''' <c>@EqualsAndHashCode(exclude = {"inputSchema"})</c>) compares the reducer and the window function.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"inputSchema"})
    <JsonIgnoreProperties("inputSchema")>
    <Serializable>
    Public Class ReduceSequenceByWindowTransform
        Implements TransformT

        Private reducer As IAssociativeReducer
        Private windowFunction As WindowFunction
        Private inputSchemaField As SchemaT

        <JsonConstructor>
        Public Sub New(ByVal reducer As IAssociativeReducer, ByVal windowFunction As WindowFunction)
            Me.reducer = reducer
            Me.windowFunction = windowFunction
        End Sub

        ''' <summary>Java <c>getReducer()</c> (Lombok).</summary>
        Public Function getReducer() As IAssociativeReducer
            Return reducer
        End Function

        ''' <summary>Java <c>setReducer()</c> (Lombok).</summary>
        Public Sub setReducer(ByVal reducer As IAssociativeReducer)
            Me.reducer = reducer
        End Sub

        ''' <summary>Java <c>getWindowFunction()</c> (Lombok).</summary>
        Public Function getWindowFunction() As WindowFunction
            Return windowFunction
        End Function

        ''' <summary>Java <c>setWindowFunction()</c> (Lombok).</summary>
        Public Sub setWindowFunction(ByVal windowFunction As WindowFunction)
            Me.windowFunction = windowFunction
        End Sub

        ''' <summary>
        ''' The output schema: a sequence schema with the columns of the reducer's output for the window function's
        ''' output schema.
        ''' </summary>
        ''' <exception cref="ArgumentException">if the input schema is not a sequence schema</exception>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            If inputSchema IsNot Nothing AndAlso Not (TypeOf inputSchema Is SequenceSchemaT) Then
                Throw New ArgumentException("Invalid input: input schema must be a SequenceSchema")
            End If

            'Some window functions may make changes to the schema (adding window start/end times, for example)
            inputSchema = windowFunction.transform(inputSchema)

            'Approach here: The reducer gives us a schema for one time step -> simply convert this to a sequence schema...
            Dim oneStepSchema As SchemaT = reducer.transform(inputSchema)
            Dim meta As IList(Of ColumnMetaData) = oneStepSchema.getColumnMetaData()

            Return New SequenceSchemaT(meta)
        End Function

        ''' <summary>
        ''' The input schema; setting it also sets the window function's input schema and gives the reducer the window
        ''' function's output schema.
        ''' </summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Get
                Return inputSchemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
                Me.windowFunction.InputSchema = value
                reducer.InputSchema = windowFunction.transform(value)
            End Set
        End Property

        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("ReduceSequenceByWindownTransform can only be applied on sequences")
        End Function

        ''' <summary>Splits the sequence into windows and reduces each window to one time step.</summary>
        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence

            'List of windows, which are all small sequences...
            Dim sequenceAsWindows As IList(Of IList(Of IList(Of WritableT))) = windowFunction.applyToSequence(sequence)

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()

            For Each window As IList(Of IList(Of WritableT)) In sequenceAsWindows
                Dim accu As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
                For Each l As IList(Of WritableT) In window
                    accu.accept(l)
                Next l
                [out].Add(accu.get())
            Next window

            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("ReduceSequenceByWindownTransform can only be applied on sequences")
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("Needs to be implemented")
        End Function

        Public Overrides Function ToString() As String
            Return "ReduceSequenceByWindowTransform(reducer=" & Objects.toString(reducer) & ",windowFunction=" & Objects.toString(windowFunction) & ")"
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
            Dim other As ReduceSequenceByWindowTransform = DirectCast(obj, ReduceSequenceByWindowTransform)
            Return Object.Equals(reducer, other.reducer) AndAlso Object.Equals(windowFunction, other.windowFunction)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(reducer, windowFunction)
        End Function

    End Class

End Namespace
