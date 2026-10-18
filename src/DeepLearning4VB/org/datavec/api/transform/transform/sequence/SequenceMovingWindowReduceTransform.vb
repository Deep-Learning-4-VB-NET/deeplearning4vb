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
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports AggregableReductionUtils = DeepLearning4VB.org.datavec.api.transform.reduce.AggregableReductionUtils
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.sequence

    ''' <summary>
    ''' SequenceMovingWindowReduceTransform: Adds a new column, where the value is derived by:<br/>
    ''' (a) using a window of the last N values in a single column,<br/>
    ''' (b) Apply a reduction op on the window to calculate a new value<br/>
    ''' for example, this transformer can be used to implement a simple moving average of the last N values,
    ''' or determine the minimum or maximum values in the last N time steps.
    ''' <para>
    ''' For example, for a simple moving average, length 20: <c>New SequenceMovingWindowReduceTransform("myCol", 20, ReduceOp.Mean)</c>
    ''' </para>
    ''' <para>Equality (Lombok <c>@Data</c>) compares the class and all fields, including the input schema.</para>
    ''' </summary>
    <Serializable>
    <JsonIgnoreProperties("inputSchema")>
    <JsonIncludeNonNull>
    Public Class SequenceMovingWindowReduceTransform
        Implements TransformT

        ''' <summary>
        ''' Enumeration to specify how each cases are handled: For example, for a look back period of 20, how should the
        ''' first 19 output values be calculated?<br/>
        ''' Default: Perform your former reduction as normal, with as many values are available<br/>
        ''' SpecifiedValue: use the given/specified value instead of the actual output value. For example, you could assign
        ''' values of 0 or NullWritable to positions 0 through 18 of the output.
        ''' </summary>
        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Enum EdgeCaseHandling
            [Default]
            SpecifiedValue
        End Enum

        Private ReadOnly columnNameField As String
        Private ReadOnly newColumnName As String
        Private ReadOnly lookback As Integer
        Private ReadOnly op As ReduceOp
        Private ReadOnly edgeCaseHandlingField As EdgeCaseHandling
        Private ReadOnly edgeCaseValue As WritableT
        Private inputSchemaField As SchemaT

        ''' <param name="columnName"> Column name to perform windowing on </param>
        ''' <param name="lookback">   Look back period for windowing </param>
        ''' <param name="op">         Reduction operation to perform on each window </param>
        Public Sub New(ByVal columnName As String, ByVal lookback As Integer, ByVal op As ReduceOp)
            Me.New(columnName, defaultOutputColumnName(columnName, lookback, op), lookback, op, EdgeCaseHandling.Default, Nothing)
        End Sub

        ''' <param name="columnName">       Column name to perform windowing on </param>
        ''' <param name="newColumnName">    Name of the new output column (with results) </param>
        ''' <param name="lookback">         Look back period for windowing </param>
        ''' <param name="op">               Reduction operation to perform on each window </param>
        ''' <param name="edgeCaseHandling"> How the 1st steps should be handled (positions in sequence with indices less then the look-back period) </param>
        ''' <param name="edgeCaseValue">    Used only with EdgeCaseHandling.SpecifiedValue, maybe null otherwise </param>
        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal newColumnName As String, ByVal lookback As Integer, ByVal op As ReduceOp, ByVal edgeCaseHandling As EdgeCaseHandling, ByVal edgeCaseValue As WritableT)
            Me.columnNameField = columnName
            Me.newColumnName = newColumnName
            Me.lookback = lookback
            Me.op = op
            Me.edgeCaseHandlingField = edgeCaseHandling
            Me.edgeCaseValue = edgeCaseValue
        End Sub

        ''' <summary>The default name of the output column: <c>op(lookback,originalName)</c>, for example <c>mean(20,price)</c>.</summary>
        Public Shared Function defaultOutputColumnName(ByVal originalName As String, ByVal lookback As Integer, ByVal op As ReduceOp) As String
            Return op.ToString().ToLowerInvariant() & "(" & lookback & "," & originalName & ")"
        End Function

        ''' <summary>
        ''' The output schema: the input columns plus the new column (same type as the input for Min, Max, Range,
        ''' TakeFirst and TakeLast; Double for Prod, Sum, Mean and Stdev; Integer for Count and CountUnique).
        ''' </summary>
        ''' <exception cref="NotSupportedException">for other reduction ops</exception>
        Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements Operation(Of SchemaT, SchemaT).transform
            Dim colIdx As Integer = inputSchema.getIndexOfColumn(columnNameField)

            'Approach here: The reducer gives us a schema for one time step -> simply convert this to a sequence schema...
            Dim oldMeta As IList(Of ColumnMetaData) = inputSchema.getColumnMetaData()
            Dim meta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(oldMeta)

            Dim m As ColumnMetaData
            Select Case op
                Case ReduceOp.Min, ReduceOp.Max, ReduceOp.Range, ReduceOp.TakeFirst, ReduceOp.TakeLast
                    'Same type as input
                    m = oldMeta(colIdx)
                    m = m.clone()
                    m.Name = newColumnName
                Case ReduceOp.Prod, ReduceOp.Sum, ReduceOp.Mean, ReduceOp.Stdev
                    'Double type
                    m = New DoubleMetaData(newColumnName)
                Case ReduceOp.Count, ReduceOp.CountUnique
                    'Integer type
                    m = New IntegerMetaData(newColumnName)
                Case Else
                    Throw New NotSupportedException("Unknown op type: " & op.ToString())
            End Select
            meta.Add(m)

            Return New SequenceSchemaT(meta)
        End Function

        Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
            Set(ByVal value As SchemaT)
                Me.inputSchemaField = value
            End Set
            Get
                Return inputSchemaField
            End Get
        End Property

        ''' <exception cref="NotSupportedException">always: the transform applies to sequences only</exception>
        Public Overridable Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT) Implements TransformT.map
            Throw New NotSupportedException("SequenceMovingWindowReduceTransform can only be applied on sequences")
        End Function

        Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements TransformT.mapSequence
            Dim colIdx As Integer = inputSchemaField.getIndexOfColumn(columnNameField)
            Dim columnType As ColumnType = inputSchemaField.getType(colIdx)
            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(sequence.Count)
            Dim window As New LinkedList(Of WritableT)()
            For i As Integer = 0 To sequence.Count - 1
                Dim current As WritableT = sequence(i)(colIdx)
                window.AddLast(current)
                If window.Count > lookback Then
                    window.RemoveFirst()
                End If
                Dim reduced As WritableT
                If window.Count < lookback AndAlso edgeCaseHandlingField = EdgeCaseHandling.SpecifiedValue Then
                    reduced = edgeCaseValue
                Else
                    Dim reductionOp As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = AggregableReductionUtils.reduceColumn(JavaCollections.singletonList(op), columnType, False, Nothing)
                    For Each w As WritableT In window
                        reductionOp.accept(w)
                    Next w
                    reduced = reductionOp.get()(0)
                End If
                Dim outThisStep As New List(Of WritableT)(sequence(i).Count + 1)
                outThisStep.AddRange(sequence(i))
                outThisStep.Add(reduced)
                [out].Add(outThisStep)
            Next i

            Return [out]
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overridable Function map(ByVal input As Object) As Object Implements TransformT.map
            Throw New NotSupportedException("SequenceMovingWindowReduceTransform can only be applied to sequences")
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements TransformT.mapSequence
            Throw New NotSupportedException("not yet implemented")
        End Function

        Public Overrides Function ToString() As String
            Return "SequenceMovingWindowReduceTransform(columnName=""" & columnNameField & """,newColumnName=""" & newColumnName & """,lookback=" & lookback & ",op=" & op.ToString() & ",edgeCaseHandling=" & edgeCaseHandlingField.ToString() & (If(edgeCaseHandlingField = EdgeCaseHandling.SpecifiedValue, ",edgeCaseValue=" & javaToString(edgeCaseValue), "")) & ")"
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
        ''' this op is meant to run on (as in Java: all the columns of the input schema)
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

        ' ---- Lombok @Data accessors ----

        Public Function getColumnName() As String
            Return columnNameField
        End Function

        Public Function getNewColumnName() As String
            Return newColumnName
        End Function

        Public Function getLookback() As Integer
            Return lookback
        End Function

        Public Function getOp() As ReduceOp
            Return op
        End Function

        Public Function getEdgeCaseHandling() As EdgeCaseHandling
            Return edgeCaseHandlingField
        End Function

        Public Function getEdgeCaseValue() As WritableT
            Return edgeCaseValue
        End Function

        Public Function getInputSchema() As SchemaT
            Return inputSchemaField
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim o As SequenceMovingWindowReduceTransform = DirectCast(obj, SequenceMovingWindowReduceTransform)
            Return String.Equals(columnNameField, o.columnNameField) AndAlso String.Equals(newColumnName, o.newColumnName) AndAlso
                lookback = o.lookback AndAlso op = o.op AndAlso edgeCaseHandlingField = o.edgeCaseHandlingField AndAlso
                Object.Equals(edgeCaseValue, o.edgeCaseValue) AndAlso Object.Equals(inputSchemaField, o.inputSchemaField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(columnNameField, newColumnName, lookback, op, edgeCaseHandlingField, edgeCaseValue, inputSchemaField)
        End Function

    End Class

End Namespace
