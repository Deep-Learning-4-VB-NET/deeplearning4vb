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
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports TrivialColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.TrivialColumnCondition
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce

    ''' <summary>
    ''' A <see cref="IAssociativeReducer"/> that reduces each column of a group of records with one or more
    ''' <see cref="ReduceOp"/>s (a default op plus per-column ops), custom column reductions
    ''' (<see cref="AggregableColumnReduction"/>) or conditional reductions (only the records that satisfy a
    ''' condition are reduced). Key columns keep their (first) value. Built with
    ''' <c>New Reducer.Builder(ReduceOp.Sum).keyColumns("key").meanColumns("x").build()</c>.
    ''' Equality (Lombok <c>@EqualsAndHashCode(exclude = {"schema", "keyColumnsSet"})</c>) compares the key columns,
    ''' the ops and the reductions.
    ''' </summary>
    ' Java: @JsonIgnoreProperties({"schema", "keyColumnsSet"})
    <Serializable>
    Public Class Reducer
        Implements IAssociativeReducer

        Private schemaField As SchemaT
        Private ReadOnly keyColumnsField As IList(Of String)
        Private ReadOnly keyColumnsSet As ISet(Of String)
        Private ReadOnly defaultOp As ReduceOp
        Private ReadOnly opMap As IDictionary(Of String, IList(Of ReduceOp))
        Private conditionalReductions As IDictionary(Of String, ConditionalReduction)
        Private customReductions As IDictionary(Of String, AggregableColumnReduction)

        Private ignoreInvalidInColumns As ISet(Of String)

        Private Sub New(ByVal builder As Builder)
            Me.New(If(builder.keyColumnsField Is Nothing, Nothing, New List(Of String)(builder.keyColumnsField)), builder.defaultOp, builder.opMap, builder.customReductions, builder.conditionalReductions, builder.ignoreInvalidInColumns)
        End Sub

        <JsonConstructor>
        Public Sub New(ByVal keyColumns As IList(Of String), ByVal defaultOp As ReduceOp, ByVal opMap As IDictionary(Of String, IList(Of ReduceOp)), ByVal customReductions As IDictionary(Of String, AggregableColumnReduction), ByVal conditionalReductions As IDictionary(Of String, ConditionalReduction), ByVal ignoreInvalidInColumns As ISet(Of String))
            Me.keyColumnsField = keyColumns
            Me.keyColumnsSet = If(keyColumns Is Nothing, Nothing, New HashSet(Of String)(keyColumns))
            Me.defaultOp = defaultOp
            Me.opMap = opMap
            Me.customReductions = customReductions
            Me.conditionalReductions = conditionalReductions
            Me.ignoreInvalidInColumns = ignoreInvalidInColumns
        End Sub

        ''' <summary>
        ''' The input schema. Setting it also sets the input schema of the conditions of the conditional reductions.
        ''' </summary>
        <JsonIgnore>
        Public Overridable Property InputSchema As SchemaT Implements IAssociativeReducer.InputSchema
            Get
                Return schemaField
            End Get
            Set(ByVal value As SchemaT)
                Me.schemaField = value
                'Conditions (if any) also need the input schema:
                If conditionalReductions IsNot Nothing Then
                    For Each cr As ConditionalReduction In conditionalReductions.Values
                        cr.getCondition().InputSchema = value
                    Next cr
                End If
            End Set
        End Property

        ''' <summary>The key columns (Nothing when there are none).</summary>
        Public Overridable ReadOnly Property KeyColumns As IList(Of String) Implements IAssociativeReducer.KeyColumns
            Get
                Return keyColumnsField
            End Get
        End Property

        ''' <summary>Java <c>getKeyColumns()</c> (Lombok).</summary>
        Public Function getKeyColumns() As IList(Of String)
            Return keyColumnsField
        End Function

        ''' <summary>Java <c>getSchema()</c> (Lombok): the input schema.</summary>
        Public Function getSchema() As SchemaT
            Return schemaField
        End Function

        ''' <summary>Java <c>getKeyColumnsSet()</c> (Lombok).</summary>
        Public Function getKeyColumnsSet() As ISet(Of String)
            Return keyColumnsSet
        End Function

        ''' <summary>Java <c>getDefaultOp()</c> (Lombok).</summary>
        Public Function getDefaultOp() As ReduceOp
            Return defaultOp
        End Function

        ''' <summary>Java <c>getOpMap()</c> (Lombok).</summary>
        Public Function getOpMap() As IDictionary(Of String, IList(Of ReduceOp))
            Return opMap
        End Function

        ''' <summary>Java <c>getConditionalReductions()</c> (Lombok).</summary>
        Public Function getConditionalReductions() As IDictionary(Of String, ConditionalReduction)
            Return conditionalReductions
        End Function

        ''' <summary>Java <c>setConditionalReductions()</c> (Lombok).</summary>
        Public Sub setConditionalReductions(ByVal conditionalReductions As IDictionary(Of String, ConditionalReduction))
            Me.conditionalReductions = conditionalReductions
        End Sub

        ''' <summary>Java <c>getCustomReductions()</c> (Lombok).</summary>
        Public Function getCustomReductions() As IDictionary(Of String, AggregableColumnReduction)
            Return customReductions
        End Function

        ''' <summary>Java <c>setCustomReductions()</c> (Lombok).</summary>
        Public Sub setCustomReductions(ByVal customReductions As IDictionary(Of String, AggregableColumnReduction))
            Me.customReductions = customReductions
        End Sub

        ''' <summary>Java <c>getIgnoreInvalidInColumns()</c> (Lombok).</summary>
        Public Function getIgnoreInvalidInColumns() As ISet(Of String)
            Return ignoreInvalidInColumns
        End Function

        ''' <summary>Java <c>setIgnoreInvalidInColumns()</c> (Lombok).</summary>
        Public Sub setIgnoreInvalidInColumns(ByVal ignoreInvalidInColumns As ISet(Of String))
            Me.ignoreInvalidInColumns = ignoreInvalidInColumns
        End Sub

        ''' <summary>
        ''' Get the output schema, given the input schema
        ''' </summary>
        Public Overridable Function transform(ByVal schema As SchemaT) As SchemaT Implements IAssociativeReducer.transform
            Dim nCols As Integer = schema.numColumns()
            Dim colNames As IList(Of String) = schema.getColumnNames()
            Dim meta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
            Dim newMeta As New List(Of ColumnMetaData)(nCols)

            For i As Integer = 0 To nCols - 1
                Dim name As String = colNames(i)
                Dim inMeta As ColumnMetaData = meta(i)

                If keyColumnsSet IsNot Nothing AndAlso keyColumnsSet.Contains(name) Then
                    'No change to key columns
                    newMeta.Add(inMeta)
                    Continue For
                End If

                'First: check for a custom reduction on this column
                If customReductions IsNot Nothing AndAlso customReductions.ContainsKey(name) Then
                    Dim reduction As AggregableColumnReduction = customReductions(name)

                    Dim outName As IList(Of String) = reduction.getColumnsOutputName(name)
                    Dim outMeta As IList(Of ColumnMetaData) = reduction.getColumnOutputMetaData(outName, inMeta)
                    newMeta.AddRange(outMeta)
                    Continue For
                End If

                'Second: check for conditional reductions on this column:
                If conditionalReductions IsNot Nothing AndAlso conditionalReductions.ContainsKey(name) Then
                    Dim reduction As ConditionalReduction = conditionalReductions(name)

                    Dim outNames As IList(Of String) = reduction.getOutputNames()
                    Dim reductions As IList(Of ReduceOp) = reduction.getReductions()
                    For j As Integer = 0 To reductions.Count - 1
                        Dim red As ReduceOp = reductions(j)
                        Dim outName As String = outNames(j)
                        Dim m As ColumnMetaData = getMetaForColumn(red, name, inMeta)
                        m.Name = outName
                        newMeta.Add(m)
                    Next j
                    Continue For
                End If

                'Otherwise: get the specified (built-in) reduction op
                'If no reduction op is specified for that column: use the default
                Dim lop As IList(Of ReduceOp)
                If opMap IsNot Nothing AndAlso opMap.ContainsKey(name) Then
                    lop = opMap(name)
                Else
                    lop = JavaCollections.singletonList(defaultOp)
                End If
                If lop IsNot Nothing Then
                    For Each op As ReduceOp In lop
                        newMeta.Add(getMetaForColumn(op, name, inMeta))
                    Next op
                End If
            Next i

            Return schema.newSchema(newMeta)
        End Function

        Private Shared Function getOutNameForColumn(ByVal op As ReduceOp, ByVal name As String) As String
            Return op.ToString().ToLowerInvariant() & "(" & name & ")"
        End Function

        Private Shared Function getMetaForColumn(ByVal op As ReduceOp, ByVal name As String, ByVal inMeta As ColumnMetaData) As ColumnMetaData
            inMeta = inMeta.clone()
            Select Case op
                ' type-preserving operations
                Case ReduceOp.Min, ReduceOp.Max, ReduceOp.Range, ReduceOp.TakeFirst, ReduceOp.TakeLast
                    inMeta.Name = getOutNameForColumn(op, name)
                    Return inMeta
                Case ReduceOp.Prod, ReduceOp.Sum
                    Dim outName As String = getOutNameForColumn(op, name)
                    'Issue with prod/sum: the input meta data restrictions probably won't hold. But the data _type_ should essentially remain the same
                    Dim outMeta As ColumnMetaData
                    If TypeOf inMeta Is IntegerMetaData Then
                        outMeta = New IntegerMetaData(outName)
                    ElseIf TypeOf inMeta Is LongMetaData Then
                        outMeta = New LongMetaData(outName)
                    ElseIf TypeOf inMeta Is FloatMetaData Then
                        outMeta = New FloatMetaData(outName)
                    ElseIf TypeOf inMeta Is DoubleMetaData Then
                        outMeta = New DoubleMetaData(outName)
                    Else
                        'Sum/Prod doesn't really make sense to sum other column types anyway...
                        outMeta = inMeta
                    End If
                    outMeta.Name = outName
                    Return outMeta
                Case ReduceOp.Mean, ReduceOp.Stdev, ReduceOp.Variance, ReduceOp.PopulationVariance, ReduceOp.UncorrectedStdDev
                    Return New DoubleMetaData(getOutNameForColumn(op, name))
                Case ReduceOp.Append, ReduceOp.Prepend
                    Return New StringMetaData(getOutNameForColumn(op, name))
                Case ReduceOp.Count, ReduceOp.CountUnique
                    'Always Long
                    Return New LongMetaData(getOutNameForColumn(op, name), 0L, Nothing)
                Case Else
                    Throw New NotSupportedException("Unknown or not implemented op: " & op.ToString())
            End Select
        End Function

        ''' <summary>
        ''' The accumulator that reduces records of the input schema (which must have been set) into one record.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">if the input schema has not been set</exception>
        Public Overridable Function aggregableReducer() As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) Implements IAssociativeReducer.aggregableReducer
            'Go through each writable, and reduce according to whatever strategy is specified

            If schemaField Is Nothing Then
                Throw New InvalidOperationException("Error: Schema has not been set")
            End If

            Dim nCols As Integer = schemaField.numColumns()
            Dim colNames As IList(Of String) = schemaField.getColumnNames()

            Dim ops As IList(Of IAggregableReduceOp(Of WritableT, IList(Of WritableT))) = New List(Of IAggregableReduceOp(Of WritableT, IList(Of WritableT)))(nCols)
            Dim conditionalActive As Boolean = (conditionalReductions IsNot Nothing AndAlso conditionalReductions.Count > 0)
            Dim conditions As IList(Of ConditionT) = New List(Of ConditionT)(nCols)

            For i As Integer = 0 To nCols - 1
                Dim colName As String = colNames(i)
                If keyColumnsSet IsNot Nothing AndAlso keyColumnsSet.Contains(colName) Then
                    Dim first As IAggregableReduceOp(Of WritableT, WritableT) = New AggregatorImpls.AggregableFirst(Of WritableT)()
                    ops.Add(New AggregableMultiOp(Of WritableT)(JavaCollections.singletonList(first)))
                    If conditionalActive Then
                        conditions.Add(New TrivialColumnCondition(colName))
                    End If
                    Continue For
                End If

                ' is this a *custom* reduction column?
                If customReductions IsNot Nothing AndAlso customReductions.ContainsKey(colName) Then
                    Dim reduction As AggregableColumnReduction = customReductions(colName)
                    ops.Add(reduction.reduceOp())
                    If conditionalActive Then
                        conditions.Add(New TrivialColumnCondition(colName))
                    End If
                    Continue For
                End If

                ' are we adding global *conditional* reduction column?
                ' Only practical difference with conditional reductions is we filter the input on an all-fields condition first
                If conditionalActive Then
                    If conditionalReductions.ContainsKey(colName) Then
                        conditions.Add(conditionalReductions(colName).getCondition())
                    Else
                        conditions.Add(New TrivialColumnCondition(colName))
                    End If
                End If

                'What type of column is this?
                Dim type As ColumnType = schemaField.getType(i)

                'What ops are we performing on this column?
                Dim conditionalOp As Boolean = conditionalActive AndAlso conditionalReductions.ContainsKey(colName)
                Dim lop As IList(Of ReduceOp) = Nothing
                If conditionalOp Then
                    lop = conditionalReductions(colName).getReductions()
                ElseIf opMap IsNot Nothing Then
                    opMap.TryGetValue(colName, lop)
                End If
                If lop Is Nothing OrElse lop.Count = 0 Then
                    lop = JavaCollections.singletonList(defaultOp)
                End If

                'Execute the reduction, store the result
                Dim ignoreInvalid As Boolean = ignoreInvalidInColumns IsNot Nothing AndAlso ignoreInvalidInColumns.Contains(colName)
                ops.Add(AggregableReductionUtils.reduceColumn(lop, type, ignoreInvalid, schemaField.getMetaData(i)))
            Next i

            If conditionalActive Then
                Return New DispatchWithConditionOp(Of WritableT)(ops, conditions)
            Else
                Return New DispatchOp(Of WritableT, WritableT)(ops)
            End If
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder("Reducer(")
            If keyColumnsField IsNot Nothing Then
                sb.Append("keyColumns=").Append(javaString(keyColumnsField)).Append(",")
            End If
            sb.Append("defaultOp=").Append(defaultOp.ToString())
            If opMap IsNot Nothing Then
                sb.Append(",opMap=").Append(javaString(opMap))
            End If
            If customReductions IsNot Nothing Then
                sb.Append(",customReductions=").Append(javaString(customReductions))
            End If
            If conditionalReductions IsNot Nothing Then
                sb.Append(",conditionalReductions=").Append(javaString(conditionalReductions))
            End If
            If ignoreInvalidInColumns IsNot Nothing Then
                sb.Append(",ignoreInvalidInColumns=").Append(javaString(ignoreInvalidInColumns))
            End If
            sb.Append(")")
            Return sb.ToString()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Dim other As Reducer = DirectCast(obj, Reducer)
            Return listEquals(keyColumnsField, other.keyColumnsField) AndAlso defaultOp = other.defaultOp AndAlso
                mapEquals(Of IList(Of ReduceOp))(opMap, other.opMap, Function(x, y) listEquals(x, y)) AndAlso
                mapEquals(Of ConditionalReduction)(conditionalReductions, other.conditionalReductions, Function(x, y) Object.Equals(x, y)) AndAlso
                mapEquals(Of AggregableColumnReduction)(customReductions, other.customReductions, Function(x, y) Object.Equals(x, y)) AndAlso
                setEquals(ignoreInvalidInColumns, other.ignoreInvalidInColumns)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + listHash(keyColumnsField)
            h = h * 59 + CInt(defaultOp)
            h = h * 59 + mapHash(Of IList(Of ReduceOp))(opMap, Function(x) listHash(x))
            h = h * 59 + mapHash(Of ConditionalReduction)(conditionalReductions, Function(x) Objects.hashCode(x))
            h = h * 59 + mapHash(Of AggregableColumnReduction)(customReductions, Function(x) Objects.hashCode(x))
            h = h * 59 + setHash(ignoreInvalidInColumns)
            Return h
        End Function

        ' ---- Java collection semantics (List/Map/Set equals, hashCode and toString); candidates for hoisting ----

        ''' <summary>Java <c>List.equals</c>: same size and pairwise equal elements (null-safe).</summary>
        Friend Shared Function listEquals(Of T)(ByVal a As IList(Of T), ByVal b As IList(Of T)) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
            If a.Count <> b.Count Then Return False
            For i As Integer = 0 To a.Count - 1
                If Not Object.Equals(a(i), b(i)) Then Return False
            Next i
            Return True
        End Function

        ''' <summary>Java <c>List.hashCode</c> (0 for Nothing).</summary>
        Friend Shared Function listHash(Of T)(ByVal a As IList(Of T)) As Integer
            If a Is Nothing Then Return 0
            Dim h As Integer = 1
            For Each x As T In a
                h = 31 * h + If(x Is Nothing, 0, x.GetHashCode())
            Next x
            Return h
        End Function

        ''' <summary>Java <c>Map.equals</c> with a value comparison (null-safe).</summary>
        Private Shared Function mapEquals(Of V)(ByVal a As IDictionary(Of String, V), ByVal b As IDictionary(Of String, V), ByVal valueEquals As Func(Of V, V, Boolean)) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
            If a.Count <> b.Count Then Return False
            For Each kv As KeyValuePair(Of String, V) In a
                Dim otherValue As V = Nothing
                If Not b.TryGetValue(kv.Key, otherValue) Then Return False
                If Not valueEquals(kv.Value, otherValue) Then Return False
            Next kv
            Return True
        End Function

        ''' <summary>Java <c>Map.hashCode</c> (sum of key hash xor value hash) with a value hash function.</summary>
        Private Shared Function mapHash(Of V)(ByVal a As IDictionary(Of String, V), ByVal valueHash As Func(Of V, Integer)) As Integer
            If a Is Nothing Then Return 0
            Dim h As Integer = 0
            For Each kv As KeyValuePair(Of String, V) In a
                h += Objects.hashCode(kv.Key) Xor valueHash(kv.Value)
            Next kv
            Return h
        End Function

        ''' <summary>Java <c>Set.equals</c> (null-safe).</summary>
        Private Shared Function setEquals(ByVal a As ISet(Of String), ByVal b As ISet(Of String)) As Boolean
            If a Is Nothing OrElse b Is Nothing Then Return a Is Nothing AndAlso b Is Nothing
            Return a.Count = b.Count AndAlso a.SetEquals(b)
        End Function

        ''' <summary>Java <c>Set.hashCode</c> (sum of the element hashes).</summary>
        Private Shared Function setHash(ByVal a As ISet(Of String)) As Integer
            If a Is Nothing Then Return 0
            Dim h As Integer = 0
            For Each s As String In a
                h += Objects.hashCode(s)
            Next s
            Return h
        End Function

        ''' <summary>Java <c>String.valueOf</c> for collections: <c>[a, b]</c> for lists and sets, <c>{k=v}</c> for maps.</summary>
        Friend Shared Function javaString(ByVal o As Object) As String
            If o Is Nothing Then Return "null"
            If TypeOf o Is String Then Return DirectCast(o, String)
            Dim map As IDictionary = TryCast(o, IDictionary)
            If map IsNot Nothing Then
                Dim sb As New StringBuilder("{")
                Dim first As Boolean = True
                For Each entry As DictionaryEntry In map
                    If Not first Then sb.Append(", ")
                    first = False
                    sb.Append(javaString(entry.Key)).Append("=").Append(javaString(entry.Value))
                Next entry
                Return sb.Append("}").ToString()
            End If
            Dim items As IEnumerable = TryCast(o, IEnumerable)
            If items IsNot Nothing Then
                Dim sb As New StringBuilder("[")
                Dim first As Boolean = True
                For Each item As Object In items
                    If Not first Then sb.Append(", ")
                    first = False
                    sb.Append(javaString(item))
                Next item
                Return sb.Append("]").ToString()
            End If
            Return Objects.toString(o)
        End Function

        ''' <summary>Fluent builder for a <see cref="Reducer"/>.</summary>
        Public Class Builder

            Friend defaultOp As ReduceOp
            Friend opMap As IDictionary(Of String, IList(Of ReduceOp)) = New Dictionary(Of String, IList(Of ReduceOp))()
            Friend customReductions As IDictionary(Of String, AggregableColumnReduction) = New Dictionary(Of String, AggregableColumnReduction)()
            Friend conditionalReductions As IDictionary(Of String, ConditionalReduction) = New Dictionary(Of String, ConditionalReduction)()
            Friend ignoreInvalidInColumns As ISet(Of String) = New HashSet(Of String)()
            Friend keyColumnsField() As String

            ''' <summary>
            ''' Create a Reducer builder, and set the default column reduction operation.
            ''' For any columns that aren't specified explicitly, they will use the default reduction operation.
            ''' If a column does have a reduction operation explicitly specified, then it will override
            ''' the default specified here.
            ''' </summary>
            ''' <param name="defaultOp"> Default reduction operation to perform </param>
            Public Sub New(ByVal defaultOp As ReduceOp)
                Me.defaultOp = defaultOp
            End Sub

            ''' <summary>
            ''' Specify the key columns. The idea here is to be able to create a (potentially compound) key
            ''' out of multiple columns, using the toString representation of the values in these columns
            ''' </summary>
            ''' <param name="columns"> Columns that will make up the key </param>
            Public Overridable Function keyColumns(ByVal ParamArray columns() As String) As Builder
                Me.keyColumnsField = columns
                Return Me
            End Function

            Friend Function add(ByVal op As ReduceOp, ByVal cols() As String) As Builder
                For Each s As String In cols
                    Dim ops As New List(Of ReduceOp)()
                    If opMap.ContainsKey(s) Then
                        ops.AddRange(opMap(s))
                    End If
                    ops.Add(op)
                    opMap(s) = ops
                Next s
                Return Me
            End Function

            Friend Function addAll(ByVal ops As IList(Of ReduceOp), ByVal cols() As String) As Builder
                For Each s As String In cols
                    Dim theseOps As New List(Of ReduceOp)()
                    If opMap.ContainsKey(s) Then
                        theseOps.AddRange(opMap(s))
                    End If
                    theseOps.AddRange(ops)
                    opMap(s) = theseOps
                Next s
                Return Me
            End Function

            ''' <summary>
            ''' Reduce the specified columns with several ops each (the Java method name, typo included).
            ''' </summary>
            Public Overridable Function multipleOpColmumns(ByVal ops As IList(Of ReduceOp), ByVal ParamArray columns() As String) As Builder
                Return addAll(ops, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the minimum value
            ''' </summary>
            Public Overridable Function minColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Min, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the maximum value
            ''' </summary>
            Public Overridable Function maxColumn(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Max, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the sum of values
            ''' </summary>
            Public Overridable Function sumColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Sum, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the product of values
            ''' </summary>
            Public Overridable Function prodColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Prod, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the mean of the values
            ''' </summary>
            Public Overridable Function meanColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Mean, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the standard deviation of the values
            ''' </summary>
            Public Overridable Function stdevColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Stdev, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the uncorrected standard deviation of the values.
            ''' (The Java method added <c>ReduceOp.Stdev</c>, contradicting its documentation; this one adds
            ''' <c>ReduceOp.UncorrectedStdDev</c>.)
            ''' </summary>
            Public Overridable Function uncorrectedStdevColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.UncorrectedStdDev, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the variance of the values
            ''' </summary>
            Public Overridable Function variance(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Variance, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the population variance of the values
            ''' </summary>
            Public Overridable Function populationVariance(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.PopulationVariance, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by counting the number of values
            ''' </summary>
            Public Overridable Function countColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Count, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the range (max-min) of the values
            ''' </summary>
            Public Overridable Function rangeColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Range, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by counting the number of unique values
            ''' </summary>
            Public Overridable Function countUniqueColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.CountUnique, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the first value
            ''' </summary>
            Public Overridable Function takeFirstColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.TakeFirst, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the last value
            ''' </summary>
            Public Overridable Function takeLastColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.TakeLast, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the concatenation of all content
            ''' Beware, the output will be huge!
            ''' </summary>
            Public Overridable Function appendColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Append, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified columns by taking the concatenation of all content in the reverse order
            ''' Beware, the output will be huge!
            ''' </summary>
            Public Overridable Function prependColumns(ByVal ParamArray columns() As String) As Builder
                Return add(ReduceOp.Prepend, columns)
            End Function

            ''' <summary>
            ''' Reduce the specified column using a custom column reduction functionality.
            ''' </summary>
            ''' <param name="column">          Column to execute the custom reduction functionality on </param>
            ''' <param name="columnReduction"> Column reduction to execute on that column </param>
            Public Overridable Function customReduction(ByVal column As String, ByVal columnReduction As AggregableColumnReduction) As Builder
                customReductions(column) = columnReduction
                Return Me
            End Function

            ''' <summary>
            ''' Conditional reduction: apply the reduces on a specified column, where the reduction occurs *only* on those
            ''' examples where the condition returns true. Examples where the condition does not apply (returns false) are
            ''' ignored/excluded.
            ''' </summary>
            ''' <param name="column">      Name of the column to execute the conditional reduction on </param>
            ''' <param name="outputNames"> Names of the columns, after the reduction has been executed </param>
            ''' <param name="reductions">  Reductions to execute </param>
            ''' <param name="condition">   Condition to use in the reductions </param>
            ''' <exception cref="ArgumentException">if the number of names differs from the number of reductions</exception>
            Public Overridable Function conditionalReduction(ByVal column As String, ByVal outputNames As IList(Of String), ByVal reductions As IList(Of ReduceOp), ByVal condition As ConditionT) As Builder
                Preconditions.checkArgument(outputNames.Count = reductions.Count, "Conditional reductions should provide names for every column")
                Me.conditionalReductions(column) = New ConditionalReduction(column, outputNames, reductions, condition)
                Return Me
            End Function

            ''' <summary>
            ''' Conditional reduction: apply the reduces on a specified column, where the reduction occurs *only* on those
            ''' examples where the condition returns true. Examples where the condition does not apply (returns false) are
            ''' ignored/excluded.
            ''' </summary>
            ''' <param name="column">     Name of the column to execute the conditional reduction on </param>
            ''' <param name="outputName"> Name of the column, after the reduction has been executed </param>
            ''' <param name="reduction">  Reduction to execute </param>
            ''' <param name="condition">  Condition to use in the reductions </param>
            Public Overridable Function conditionalReduction(ByVal column As String, ByVal outputName As String, ByVal reduction As ReduceOp, ByVal condition As ConditionT) As Builder
                Me.conditionalReductions(column) = New ConditionalReduction(column, JavaCollections.singletonList(outputName), JavaCollections.singletonList(reduction), condition)
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

            Public Overridable Function build() As Reducer
                Return New Reducer(Me)
            End Function
        End Class

        ''' <summary>
        ''' A conditional reduction of one column: the reductions only see the records that satisfy the condition.
        ''' Equality (Lombok <c>@Data</c>) compares all four fields.
        ''' </summary>
        <Serializable>
        Public Class ConditionalReduction
            Private ReadOnly columnNameField As String
            Private ReadOnly outputNamesField As IList(Of String)
            Private ReadOnly reductionsField As IList(Of ReduceOp)
            Private ReadOnly conditionField As ConditionT

            Public Sub New(ByVal columnName As String, ByVal outputNames As IList(Of String), ByVal reductions As IList(Of ReduceOp), ByVal condition As ConditionT)
                Me.columnNameField = columnName
                Me.outputNamesField = outputNames
                Me.reductionsField = reductions
                Me.conditionField = condition
            End Sub

            Public Function getColumnName() As String
                Return columnNameField
            End Function

            Public Function getOutputNames() As IList(Of String)
                Return outputNamesField
            End Function

            Public Function getReductions() As IList(Of ReduceOp)
                Return reductionsField
            End Function

            Public Function getCondition() As ConditionT
                Return conditionField
            End Function

            Public Overrides Function Equals(ByVal obj As Object) As Boolean
                If obj Is Me Then Return True
                If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
                Dim other As ConditionalReduction = DirectCast(obj, ConditionalReduction)
                Return String.Equals(columnNameField, other.columnNameField) AndAlso listEquals(outputNamesField, other.outputNamesField) AndAlso
                    listEquals(reductionsField, other.reductionsField) AndAlso Object.Equals(conditionField, other.conditionField)
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return Objects.hash(columnNameField, listHash(outputNamesField), listHash(reductionsField), conditionField)
            End Function

            Public Overrides Function ToString() As String
                Return "Reducer.ConditionalReduction(columnName=" & columnNameField & ", outputNames=" & javaString(outputNamesField) &
                    ", reductions=" & javaString(reductionsField) & ", condition=" & Objects.toString(conditionField) & ")"
            End Function
        End Class

    End Class

End Namespace
