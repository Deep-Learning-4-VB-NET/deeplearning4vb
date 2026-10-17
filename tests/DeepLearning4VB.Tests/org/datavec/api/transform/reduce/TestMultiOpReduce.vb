Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports DeepLearning4VB.org.datavec.api.transform.reduce
Imports DeepLearning4VB.org.nd4vb.common.function
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnOp = DeepLearning4VB.org.datavec.api.transform.ColumnOp
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NumberFormatException = DeepLearning4VB.JavaCompat.NumberFormatException
Imports OperationT = DeepLearning4VB.org.datavec.api.transform.Operation(Of DeepLearning4VB.org.datavec.api.transform.schema.Schema, DeepLearning4VB.org.datavec.api.transform.schema.Schema)
Imports ReduceOp = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports StringColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.StringColumnCondition
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports UnsafeWritableInjector = DeepLearning4VB.org.datavec.api.writable.UnsafeWritableInjector
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce

    ''' <summary>Port of the DataVec <c>TestMultiOpReduce</c> JUnit class.</summary>
    <TestClass>
    Public Class TestMultiOpReduce

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub testMultiOpReducerDouble()

            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(row(New Text("someKey"), New DoubleWritable(0)))
            inputs.Add(row(New Text("someKey"), New DoubleWritable(1)))
            inputs.Add(row(New Text("someKey"), New DoubleWritable(2)))
            inputs.Add(row(New Text("someKey"), New DoubleWritable(2)))

            Dim exp As IDictionary(Of ReduceOp, Double) = New Global.DeepLearning4VB.JavaCompat.LinkedHashMap(Of ReduceOp, Double)()
            exp(ReduceOp.Min) = 0.0
            exp(ReduceOp.Max) = 2.0
            exp(ReduceOp.Range) = 2.0
            exp(ReduceOp.Sum) = 5.0
            exp(ReduceOp.Mean) = 1.25
            exp(ReduceOp.Stdev) = 0.957427108
            exp(ReduceOp.Count) = 4.0
            exp(ReduceOp.CountUnique) = 3.0
            exp(ReduceOp.TakeFirst) = 0.0
            exp(ReduceOp.TakeLast) = 2.0

            For Each op As ReduceOp In exp.Keys

                Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnDouble("column").build()

                Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()

                reducer.InputSchema = schema
                Dim accumulator As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

                For i As Integer = 0 To inputs.Count - 1
                    accumulator.accept(inputs(i))
                Next i
                Dim [out] As IList(Of WritableT) = accumulator.get()

                assertEquals(2, [out].Count)

                assertEquals([out](0), New Text("someKey"))

                Dim msg As String = op.ToString()
                assertEquals(exp(op), [out](1).toDouble(), 0.00001, msg)
            Next op
        End Sub

        <TestMethod>
        Public Sub testReducerInteger()

            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(row(New Text("someKey"), New IntWritable(0)))
            inputs.Add(row(New Text("someKey"), New IntWritable(1)))
            inputs.Add(row(New Text("someKey"), New IntWritable(2)))
            inputs.Add(row(New Text("someKey"), New IntWritable(2)))

            Dim exp As IDictionary(Of ReduceOp, Double) = New Global.DeepLearning4VB.JavaCompat.LinkedHashMap(Of ReduceOp, Double)()
            exp(ReduceOp.Min) = 0.0
            exp(ReduceOp.Max) = 2.0
            exp(ReduceOp.Range) = 2.0
            exp(ReduceOp.Sum) = 5.0
            exp(ReduceOp.Mean) = 1.25
            exp(ReduceOp.Stdev) = 0.957427108
            exp(ReduceOp.Count) = 4.0
            exp(ReduceOp.CountUnique) = 3.0
            exp(ReduceOp.TakeFirst) = 0.0
            exp(ReduceOp.TakeLast) = 2.0

            For Each op As ReduceOp In exp.Keys

                Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("column").build()

                Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()

                reducer.InputSchema = schema
                Dim accumulator As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

                For i As Integer = 0 To inputs.Count - 1
                    accumulator.accept(inputs(i))
                Next i
                Dim [out] As IList(Of WritableT) = accumulator.get()

                assertEquals(2, [out].Count)

                assertEquals([out](0), New Text("someKey"))

                Dim msg As String = op.ToString()
                assertEquals(exp(op), [out](1).toDouble(), 0.00001, msg)
            Next op
        End Sub

        <TestMethod>
        Public Sub testReduceString()
            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(row(New Text("someKey"), New Text("1")))
            inputs.Add(row(New Text("someKey"), New Text("2")))
            inputs.Add(row(New Text("someKey"), New Text("3")))
            inputs.Add(row(New Text("someKey"), New Text("4")))

            Dim exp As IDictionary(Of ReduceOp, String) = New Global.DeepLearning4VB.JavaCompat.LinkedHashMap(Of ReduceOp, String)()
            exp(ReduceOp.Append) = "1234"
            exp(ReduceOp.Prepend) = "4321"

            For Each op As ReduceOp In exp.Keys

                Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnsString("column").build()

                Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()

                reducer.InputSchema = schema
                Dim accumulator As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

                For i As Integer = 0 To inputs.Count - 1
                    accumulator.accept(inputs(i))
                Next i
                Dim [out] As IList(Of WritableT) = accumulator.get()

                assertEquals(2, [out].Count)

                assertEquals([out](0), New Text("someKey"))

                Dim msg As String = op.ToString()
                assertEquals(exp(op), [out](1).ToString(), msg)
            Next op
        End Sub

        <TestMethod>
        Public Sub testReduceIntegerIgnoreInvalidValues()

            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(row(New Text("someKey"), New Text("0")))
            inputs.Add(row(New Text("someKey"), New Text("1")))
            inputs.Add(row(New Text("someKey"), New IntWritable(2)))
            inputs.Add(row(New Text("someKey"), New Text("ignore me")))
            inputs.Add(row(New Text("someKey"), New Text("also ignore me")))
            inputs.Add(row(New Text("someKey"), New Text("2")))

            Dim exp As IDictionary(Of ReduceOp, Double) = New Global.DeepLearning4VB.JavaCompat.LinkedHashMap(Of ReduceOp, Double)()
            exp(ReduceOp.Min) = 0.0
            exp(ReduceOp.Max) = 2.0
            exp(ReduceOp.Range) = 2.0
            exp(ReduceOp.Sum) = 5.0
            exp(ReduceOp.Mean) = 1.25
            exp(ReduceOp.Stdev) = 0.957427108
            exp(ReduceOp.Count) = 4.0
            exp(ReduceOp.CountUnique) = 3.0
            exp(ReduceOp.TakeFirst) = 0.0
            exp(ReduceOp.TakeLast) = 2.0

            For Each op As ReduceOp In exp.Keys
                Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("column").build()

                Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").setIgnoreInvalid("column").build()

                reducer.InputSchema = schema

                Dim accumulator As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

                For i As Integer = 0 To inputs.Count - 1
                    accumulator.accept(inputs(i))
                Next i
                Dim [out] As IList(Of WritableT) = accumulator.get()

                assertEquals(2, [out].Count)

                assertEquals([out](0), New Text("someKey"))

                Dim msg As String = op.ToString()
                assertEquals(exp(op), [out](1).toDouble(), 0.00001, msg)
            Next op

            For Each op As ReduceOp In New ReduceOp() {ReduceOp.Min, ReduceOp.Max, ReduceOp.Range, ReduceOp.Sum, ReduceOp.Mean, ReduceOp.Stdev}
                'Try the same thing WITHOUT setIgnoreInvalid -> expect exception

                Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("column").build()

                Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()
                reducer.InputSchema = schema
                Dim accu As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

                Try
                    For Each i As IList(Of WritableT) In inputs
                        accu.accept(i)
                    Next i
                    fail("No exception thrown for invalid input: op=" & op.ToString())
                Catch e As NumberFormatException
                    'ok
                End Try
            Next op
        End Sub

        <TestMethod>
        Public Sub testCustomReductions()

            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(row(New Text("someKey"), New IntWritable(1), New Text("zero"), New DoubleWritable(0)))
            inputs.Add(row(New Text("someKey"), New IntWritable(2), New Text("one"), New DoubleWritable(1)))
            inputs.Add(row(New Text("someKey"), New IntWritable(3), New Text("two"), New DoubleWritable(2)))
            inputs.Add(row(New Text("someKey"), New IntWritable(4), New Text("three"), New DoubleWritable(3)))

            Dim expected As IList(Of WritableT) = row(New Text("someKey"), New IntWritable(10), New Text("one"), New DoubleWritable(1))

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("intCol").addColumnString("textCol").addColumnString("doubleCol").build()

            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").customReduction("textCol", New CustomReduceTakeSecond()).customReduction("doubleCol", New CustomReduceTakeSecond()).build()

            reducer.InputSchema = schema

            Dim accumulator As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

            For i As Integer = 0 To inputs.Count - 1
                accumulator.accept(inputs(i))
            Next i
            Dim [out] As IList(Of WritableT) = accumulator.get()

            assertEquals(4, [out].Count)
            assertEquals(expected, [out])

            'Check schema:
            Dim expNames() As String = {"key", "sum(intCol)", "myCustomReduce(textCol)", "myCustomReduce(doubleCol)"}
            Dim expTypes() As ColumnType = {ColumnType.String, ColumnType.Integer, ColumnType.String, ColumnType.String}
            Dim outSchema As SchemaT = reducer.transform(schema)

            assertEquals(4, outSchema.numColumns())
            For i As Integer = 0 To 3
                assertEquals(expNames(i), outSchema.getName(i))
                assertEquals(expTypes(i), outSchema.getType(i))
            Next i
        End Sub

        <TestMethod>
        Public Sub testCustomReductionsWithCondition()

            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(row(New Text("someKey"), New IntWritable(1), New Text("zero"), New DoubleWritable(0)))
            inputs.Add(row(New Text("someKey"), New IntWritable(2), New Text("one"), New DoubleWritable(1)))
            inputs.Add(row(New Text("someKey"), New IntWritable(3), New Text("two"), New DoubleWritable(2)))
            inputs.Add(row(New Text("someKey"), New IntWritable(4), New Text("three"), New DoubleWritable(3)))

            Dim expected As IList(Of WritableT) = row(New Text("someKey"), New IntWritable(10), New IntWritable(3), New DoubleWritable(1))

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("intCol").addColumnString("textCol").addColumnString("doubleCol").build()

            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").conditionalReduction("textCol", "condTextCol", ReduceOp.Count, New StringColumnCondition("textCol", ConditionOp.NotEqual, "three")).customReduction("doubleCol", New CustomReduceTakeSecond()).build()

            reducer.InputSchema = schema

            Dim accumulator As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

            For i As Integer = 0 To inputs.Count - 1
                accumulator.accept(inputs(i))
            Next i
            Dim [out] As IList(Of WritableT) = accumulator.get()

            assertEquals(4, [out].Count)
            assertEquals(expected, [out])

            'Check schema:
            Dim expNames() As String = {"key", "sum(intCol)", "condTextCol", "myCustomReduce(doubleCol)"}
            Dim expTypes() As ColumnType = {ColumnType.String, ColumnType.Integer, ColumnType.Long, ColumnType.String}
            Dim outSchema As SchemaT = reducer.transform(schema)

            assertEquals(4, outSchema.numColumns())
            For i As Integer = 0 To 3
                assertEquals(expNames(i), outSchema.getName(i))
                assertEquals(expTypes(i), outSchema.getType(i))
            Next i
        End Sub

        ''' <summary>A custom reduction that takes the second value of the column.</summary>
        <Serializable>
        Private Class CustomReduceTakeSecond
            Implements AggregableColumnReduction

            Public Overridable Function reduceOp() As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) Implements AggregableColumnReduction.reduceOp
                'For testing: let's take the second value
                Return New AggregableMultiOp(Of WritableT)(JavaCollections.singletonList(Of IAggregableReduceOp(Of WritableT, WritableT))(New AggregableSecond(Of WritableT)()))
            End Function

            Public Overridable Function getColumnsOutputName(ByVal columnInputName As String) As IList(Of String) Implements AggregableColumnReduction.getColumnsOutputName
                Return JavaCollections.singletonList("myCustomReduce(" & columnInputName & ")")
            End Function

            Public Overridable Function getColumnOutputMetaData(ByVal newColumnName As IList(Of String), ByVal columnInputMeta As ColumnMetaData) As IList(Of ColumnMetaData) Implements AggregableColumnReduction.getColumnOutputMetaData
                Dim thiscolumnMeta As ColumnMetaData = New StringMetaData(newColumnName(0))
                Return JavaCollections.singletonList(thiscolumnMeta)
            End Function

            ''' <summary>Keeps the second element seen.</summary>
            <Serializable>
            Public Class AggregableSecond(Of T)
                Implements IAggregableReduceOp(Of T, WritableT)

                Friend firstMet As T = Nothing
                Friend elem As T = Nothing

                Public Function getFirstMet() As T
                    Return firstMet
                End Function

                Public Function getElem() As T
                    Return elem
                End Function

                Public Overridable Sub accept(ByVal element As T) Implements Consumer(Of T).accept
                    If firstMet Is Nothing Then
                        firstMet = element
                    Else
                        If elem Is Nothing Then
                            elem = element
                        End If
                    End If
                End Sub

                Public Overridable Sub combine(ByVal accu As IAggregableReduceOp(Of T, WritableT)) Implements IAggregableReduceOp(Of T, WritableT).combine
                    If TypeOf accu Is AggregableSecond(Of T) AndAlso elem Is Nothing Then
                        Dim accumulator As AggregableSecond(Of T) = DirectCast(accu, AggregableSecond(Of T))
                        If firstMet Is Nothing Then ' this accumulator is empty, import accu
                            Dim otherFirst As T = accumulator.getFirstMet()
                            Dim otherElement As T = accumulator.getElem()
                            If otherFirst IsNot Nothing Then
                                firstMet = otherFirst
                            End If
                            If otherElement IsNot Nothing Then
                                elem = otherElement
                            End If
                        Else ' we have the first element, they may have the rest
                            Dim otherFirst As T = accumulator.getFirstMet()
                            If otherFirst IsNot Nothing Then
                                elem = otherFirst
                            End If
                        End If
                    End If
                End Sub

                Public Overridable Function [get]() As WritableT Implements Supplier(Of WritableT).get
                    Return UnsafeWritableInjector.inject(elem)
                End Function
            End Class

            ''' <summary>
            ''' Get the output schema for this transformation, given an input schema
            ''' </summary>
            Public Overridable Function transform(ByVal inputSchema As SchemaT) As SchemaT Implements OperationT.transform
                Return Nothing
            End Function

            ''' <summary>
            ''' Set the input schema.
            ''' </summary>
            Public Overridable Property InputSchema As SchemaT Implements ColumnOp.InputSchema
                Get
                    Return Nothing
                End Get
                Set(ByVal value As SchemaT)
                End Set
            End Property

            Public Overridable Function outputColumnName() As String Implements ColumnOp.outputColumnName
                Return Nothing
            End Function

            Public Overridable Function outputColumnNames() As String() Implements ColumnOp.outputColumnNames
                Return New String() {}
            End Function

            Public Overridable Function columnNames() As String() Implements ColumnOp.columnNames
                Return New String() {}
            End Function

            Public Overridable Function columnName() As String Implements ColumnOp.columnName
                Return Nothing
            End Function
        End Class

        <TestMethod>
        Public Sub testConditionalReduction()

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("intCol").addColumnString("filterCol").addColumnString("textCol").build()

            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(row(New Text("someKey"), New IntWritable(1), New Text("a"), New Text("zero")))
            inputs.Add(row(New Text("someKey"), New IntWritable(2), New Text("b"), New Text("one")))
            inputs.Add(row(New Text("someKey"), New IntWritable(3), New Text("a"), New Text("two")))
            inputs.Add(row(New Text("someKey"), New IntWritable(4), New Text("b"), New Text("three")))
            inputs.Add(row(New Text("someKey"), New IntWritable(5), New Text("a"), New Text("three")))
            inputs.Add(row(New Text("someKey"), New IntWritable(6), New Text("b"), New Text("three")))

            Dim condition As ConditionT = New StringColumnCondition("filterCol", ConditionOp.Equal, "a")

            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Stdev)).keyColumns("key").conditionalReduction("intCol", "sumOfAs", ReduceOp.Sum, condition).countUniqueColumns("filterCol", "textCol").build()

            reducer.InputSchema = schema

            Dim accumulator As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()

            For i As Integer = 0 To inputs.Count - 1
                accumulator.accept(inputs(i))
            Next i
            Dim [out] As IList(Of WritableT) = accumulator.get()
            Dim expected As IList(Of WritableT) = row(New Text("someKey"), New IntWritable(1 + 3 + 5), New LongWritable(2), New LongWritable(4))

            assertEquals(4, [out].Count)
            assertEquals(expected, [out])

            Dim outSchema As SchemaT = reducer.transform(schema)
            assertEquals(4, outSchema.numColumns())
            assertEquals(New List(Of String) From {"key", "sumOfAs", "countunique(filterCol)", "countunique(textCol)"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.String, ColumnType.Integer, ColumnType.Long, ColumnType.Long}, outSchema.getColumnTypes())
        End Sub

    End Class

End Namespace
