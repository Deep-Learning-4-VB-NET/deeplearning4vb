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
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.Json.Serialization
Imports DeepLearning4VB.org.datavec.api.transform.serde
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnReduction = DeepLearning4VB.org.datavec.api.transform.reduce.ColumnReduction
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.stringreduce

    ''' <summary>
    ''' A <see cref="IStringReducer"/> that reduces the values of each record into one string with a
    ''' <see cref="StringReduceOp"/> (MERGE and APPEND concatenate, PREPEND concatenates in reverse order, REPLACE takes
    ''' the second value). The output schema is the input schema plus one string column named by
    ''' <see cref="Builder.outputColumnName"/>. Built with <c>New StringReducer.Builder(StringReduceOp.APPEND).build()</c>.
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"schema", "keyColumnsSet"})</c>) compares the input columns,
    ''' the op, the custom reductions and the output column name.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"schema", "keyColumnsSet"})
    <JsonIgnoreProperties("schema", "keyColumnsSet")>
    <Serializable>
    Public Class StringReducer
        Implements IStringReducer

        Private schemaField As SchemaT
        Private ReadOnly inputColumnsField As IList(Of String)
        Private ReadOnly inputColumnsSet As ISet(Of String)
        Private outputColumnName As String
        Private ReadOnly stringReduceOpField As StringReduceOp
        Private customReductions As IDictionary(Of String, ColumnReduction)

        Private Sub New(ByVal builder As Builder)
            Me.New(builder.inputColumnsField, builder.defaultOp, builder.customReductions, builder.outputColumnNameField)
        End Sub

        ''' <param name="inputColumns">     the input columns (may be Nothing) </param>
        ''' <param name="stringReduceOp">   the reduction (JSON property "op") </param>
        ''' <param name="customReductions"> custom reductions per column </param>
        ''' <param name="outputColumnName"> name of the output column </param>
        <JsonConstructor>
        Public Sub New(ByVal inputColumns As IList(Of String), <JsonProperty("op")> ByVal stringReduceOp As StringReduceOp, ByVal customReductions As IDictionary(Of String, ColumnReduction), ByVal outputColumnName As String)
            Me.inputColumnsField = inputColumns
            Me.inputColumnsSet = If(inputColumns Is Nothing, Nothing, New HashSet(Of String)(inputColumns))
            Me.stringReduceOpField = stringReduceOp
            Me.customReductions = customReductions
            Me.outputColumnName = outputColumnName
        End Sub

        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements IStringReducer.InputSchema
            Get
                Return schemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.schemaField = value
            End Set
        End Property

        Public Overridable ReadOnly Property InputColumns As IList(Of String) Implements IStringReducer.InputColumns
            Get
                Return inputColumnsField
            End Get
        End Property

        ''' <summary>Java <c>getInputColumns()</c> (Lombok).</summary>
        Public Function getInputColumns() As IList(Of String)
            Return inputColumnsField
        End Function

        ''' <summary>Java <c>getSchema()</c> (Lombok): the input schema.</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        ''' <summary>Java <c>getInputColumnsSet()</c> (Lombok).</summary>
        Public Function getInputColumnsSet() As ISet(Of String)
            Return inputColumnsSet
        End Function

        ''' <summary>Java <c>getOutputColumnName()</c> (Lombok).</summary>
        Public Function getOutputColumnName() As String
            Return outputColumnName
        End Function

        ''' <summary>Java <c>setOutputColumnName()</c> (Lombok).</summary>
        Public Sub setOutputColumnName(ByVal outputColumnName As String)
            Me.outputColumnName = outputColumnName
        End Sub

        ''' <summary>Java <c>getStringReduceOp()</c> (Lombok).</summary>
        Public Function getStringReduceOp() As StringReduceOp
            Return stringReduceOpField
        End Function

        ''' <summary>Java <c>getCustomReductions()</c> (Lombok).</summary>
        Public Function getCustomReductions() As IDictionary(Of String, ColumnReduction)
            Return customReductions
        End Function

        ''' <summary>Java <c>setCustomReductions()</c> (Lombok).</summary>
        Public Sub setCustomReductions(ByVal customReductions As IDictionary(Of String, ColumnReduction))
            Me.customReductions = customReductions
        End Sub

        ''' <summary>
        ''' Get the output schema, given the input schema: the input columns plus a string column named by the
        ''' output column name.
        ''' </summary>
        Public Overridable Function transform(ByVal schema As SchemaT) As SchemaT Implements IStringReducer.transform
            Dim nCols As Integer = schema.numColumns()
            Dim meta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
            Dim newMeta As New List(Of ColumnMetaData)(nCols)
            newMeta.AddRange(meta)
            newMeta.Add(New StringMetaData(outputColumnName))
            Return schema.newSchema(newMeta)
        End Function

        ''' <summary>
        ''' Reduces each record of the list into one value with the op (the schema must have been set).
        ''' </summary>
        ''' <exception cref="InvalidOperationException">if the schema has not been set</exception>
        Public Overridable Function reduce(ByVal examplesList As IList(Of IList(Of WritableT))) As IList(Of WritableT) Implements IStringReducer.reduce
            'Go through each writable, and reduce according to whatever strategy is specified

            If schemaField Is Nothing Then
                Throw New InvalidOperationException("Error: Schema has not been set")
            End If

            Dim [out] As IList(Of WritableT) = New List(Of WritableT)(examplesList.Count)
            For i As Integer = 0 To examplesList.Count - 1
                [out].Add(reduceStringOrCategoricalColumn(stringReduceOpField, examplesList(i)))
            Next i

            Return [out]
        End Function

        ''' <summary>
        ''' Reduces the values with the op: MERGE and APPEND concatenate the values, PREPEND concatenates them in reverse
        ''' order and REPLACE returns the second value (at most two values are allowed).
        ''' </summary>
        Public Shared Function reduceStringOrCategoricalColumn(ByVal op As StringReduceOp, ByVal values As IList(Of WritableT)) As WritableT
            Select Case op
                Case StringReduceOp.MERGE, StringReduceOp.APPEND
                    Dim stringBuilder As New StringBuilder()
                    For Each w As WritableT In values
                        stringBuilder.Append(w.ToString())
                    Next w
                    Return New TextT(stringBuilder.ToString())
                Case StringReduceOp.REPLACE
                    If values.Count > 2 Then
                        Throw New ArgumentException("Unable to run replace on columns > 2")
                    End If
                    Return New TextT(values(1).ToString())
                Case StringReduceOp.PREPEND
                    Dim reversed As New List(Of WritableT)(values)
                    reversed.Reverse()
                    Dim stringBuilder2 As New StringBuilder()
                    For Each w As WritableT In reversed
                        stringBuilder2.Append(w.ToString())
                    Next w

                    Return New TextT(stringBuilder2.ToString())
                Case Else
                    Throw New NotSupportedException("Cannot execute op """ & op.ToString() & """ on String/Categorical column " & "(can only perform Count, CountUnique, TakeFirst and TakeLast ops on categorical columns)")
            End Select
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder("StringReducer(")

            sb.Append("defaultOp=").Append(stringReduceOpField.ToString())

            If customReductions IsNot Nothing Then
                sb.Append(",customReductions=").Append(mapToString(customReductions))
            End If

            sb.Append(")")
            Return sb.ToString()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As StringReducer = DirectCast(obj, StringReducer)
            Return listEquals(inputColumnsField, other.inputColumnsField) AndAlso String.Equals(outputColumnName, other.outputColumnName) AndAlso
                stringReduceOpField = other.stringReduceOpField AndAlso mapEquals(customReductions, other.customReductions)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            If inputColumnsField IsNot Nothing Then
                For Each s As String In inputColumnsField
                    h = 31 * h + Objects.hashCode(s)
                Next s
            End If
            h = h * 59 + Objects.hashCode(outputColumnName)
            h = h * 59 + CInt(stringReduceOpField)
            If customReductions IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, ColumnReduction) In customReductions
                    h += Objects.hashCode(kv.Key) Xor Objects.hashCode(kv.Value)
                Next kv
            End If
            Return h
        End Function

        ' ---- Java collection semantics; candidates for hoisting ----

        ''' <summary>Java <c>List.equals</c> for string lists (null-safe).</summary>
        Private Shared Function listEquals(ByVal a As IList(Of String), ByVal b As IList(Of String)) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
            If a.Count <> b.Count Then Return False
            For i As Integer = 0 To a.Count - 1
                If Not String.Equals(a(i), b(i)) Then Return False
            Next i
            Return True
        End Function

        ''' <summary>Java <c>Map.equals</c> (null-safe).</summary>
        Private Shared Function mapEquals(ByVal a As IDictionary(Of String, ColumnReduction), ByVal b As IDictionary(Of String, ColumnReduction)) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
            If a.Count <> b.Count Then Return False
            For Each kv As KeyValuePair(Of String, ColumnReduction) In a
                Dim otherValue As ColumnReduction = Nothing
                If Not b.TryGetValue(kv.Key, otherValue) OrElse Not Object.Equals(kv.Value, otherValue) Then Return False
            Next kv
            Return True
        End Function

        ''' <summary>Java <c>Map.toString</c>: <c>{k=v, ...}</c>.</summary>
        Private Shared Function mapToString(ByVal map As IDictionary(Of String, ColumnReduction)) As String
            Dim sb As New StringBuilder("{")
            Dim first As Boolean = True
            For Each kv As KeyValuePair(Of String, ColumnReduction) In map
                If Not first Then sb.Append(", ")
                first = False
                sb.Append(kv.Key).Append("=").Append(Objects.toString(kv.Value))
            Next kv
            Return sb.Append("}").ToString()
        End Function

        ''' <summary>Fluent builder for a <see cref="StringReducer"/>.</summary>
        Public Class Builder

            Friend defaultOp As StringReduceOp
            Friend opMap As IDictionary(Of String, StringReduceOp) = New Dictionary(Of String, StringReduceOp)()
            Friend customReductions As IDictionary(Of String, ColumnReduction) = New Dictionary(Of String, ColumnReduction)()
            Friend ignoreInvalidInColumns As ISet(Of String) = New HashSet(Of String)()
            Friend outputColumnNameField As String
            Friend inputColumnsField As IList(Of String)

            ''' <summary>
            ''' Create a StringReducer builder, and set the default column reduction operation.
            ''' For any columns that aren't specified explicitly, they will use the default reduction operation.
            ''' If a column does have a reduction operation explicitly specified, then it will override
            ''' the default specified here.
            ''' </summary>
            ''' <param name="defaultOp"> Default reduction operation to perform </param>
            Public Sub New(ByVal defaultOp As StringReduceOp)
                Me.defaultOp = defaultOp
            End Sub

            ''' <summary>Sets the input columns.</summary>
            Public Overridable Function inputColumns(ByVal columns As IList(Of String)) As Builder
                Me.inputColumnsField = columns
                Return Me
            End Function

            ''' <summary>Sets the name of the output column.</summary>
            Public Overridable Function outputColumnName(ByVal name As String) As Builder
                Me.outputColumnNameField = name
                Return Me
            End Function

            Friend Function add(ByVal op As StringReduceOp, ByVal cols() As String) As Builder
                For Each s As String In cols
                    opMap(s) = op
                Next s
                Return Me
            End Function

            ''' <summary>
            ''' Reduce the specified columns by appending their values
            ''' </summary>
            Public Overridable Function appendColumns(ByVal ParamArray columns() As String) As Builder
                Return add(StringReduceOp.APPEND, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by prepending their values
            ''' </summary>
            Public Overridable Function prependColumns(ByVal ParamArray columns() As String) As Builder
                Return add(StringReduceOp.PREPEND, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by merging their values
            ''' </summary>
            Public Overridable Function mergeColumns(ByVal ParamArray columns() As String) As Builder
                Return add(StringReduceOp.MERGE, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by replacing their values
            ''' </summary>
            Public Overridable Function replaceColumn(ByVal ParamArray columns() As String) As Builder
                Return add(StringReduceOp.REPLACE, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified column using a custom column reduction functionality.
            ''' </summary>
            ''' <param name="column">          Column to execute the custom reduction functionality on </param>
            ''' <param name="columnReduction"> Column reduction to execute on that column </param>
            Public Overridable Function customReduction(ByVal column As String, ByVal columnReduction As ColumnReduction) As Builder
                customReductions(column) = columnReduction
                Return Me
            End Function

            ''' <summary>
            ''' When doing the reduction: set the specified columns to ignore any invalid values.
            ''' Invalid: defined as being not valid according to the ColumnMetaData: <c>ColumnMetaData.isValid(Writable)</c>.
            ''' For numerical columns, this typically means being unable to parse the Writable. For example, Writable.toLong() failing for a Long column.
            ''' If the column has any restrictions (min/max values, regex for Strings etc) these will also be taken into account.
            ''' </summary>
            ''' <param name="columns"> Columns to set 'ignore invalid' for </param>
            Public Overridable Function setIgnoreInvalid(ByVal ParamArray columns() As String) As Builder
                For Each c As String In columns
                    ignoreInvalidInColumns.Add(c)
                Next c
                Return Me
            End Function

            Public Overridable Function build() As StringReducer
                Return New StringReducer(Me)
            End Function
        End Class

    End Class

End Namespace
