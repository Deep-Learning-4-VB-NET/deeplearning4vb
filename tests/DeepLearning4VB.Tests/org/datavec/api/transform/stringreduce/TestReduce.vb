Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.stringreduce
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnReduction = DeepLearning4VB.org.datavec.api.transform.reduce.ColumnReduction
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports StringReduceOp = DeepLearning4VB.org.datavec.api.transform.StringReduceOp
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.stringreduce

    ''' <summary>Port of the DataVec <c>stringreduce.TestReduce</c> JUnit class, plus tests of the rest of <see cref="StringReducer"/>.</summary>
    <TestClass>
    Public Class TestReduce

        <TestMethod>
        Public Sub testReducerDouble()

            Dim inputs As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            inputs.Add(New List(Of WritableT) From {New Text("1"), New Text("2")})
            inputs.Add(New List(Of WritableT) From {New Text("1"), New Text("2")})
            inputs.Add(New List(Of WritableT) From {New Text("1"), New Text("2")})

            Dim exp As IDictionary(Of StringReduceOp, String) = New DeepLearning4VB.JavaCompat.LinkedHashMap(Of StringReduceOp, String)()
            exp(StringReduceOp.MERGE) = "12"
            exp(StringReduceOp.APPEND) = "12"
            exp(StringReduceOp.PREPEND) = "21"
            exp(StringReduceOp.REPLACE) = "2"

            For Each op As StringReduceOp In exp.Keys

                Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnString("column").build()

                Dim reducer As StringReducer = (New StringReducer.Builder(op)).build()

                reducer.InputSchema = schema

                Dim [out] As IList(Of WritableT) = reducer.reduce(inputs)

                assertEquals(3, [out].Count)
                assertEquals(exp(op), [out](0).ToString())
            Next op
        End Sub

        <TestMethod>
        Public Sub testReduceWithoutSchemaThrows()
            Dim reducer As StringReducer = (New StringReducer.Builder(StringReduceOp.APPEND)).build()
            assertThrows(Of InvalidOperationException)(Sub() reducer.reduce(New List(Of IList(Of WritableT))()))
        End Sub

        <TestMethod>
        Public Sub testReduceStringOrCategoricalColumn()
            Dim values As IList(Of WritableT) = New List(Of WritableT) From {New Text("a"), New IntWritable(1), New Text("c")}
            assertEquals("a1c", StringReducer.reduceStringOrCategoricalColumn(StringReduceOp.APPEND, values).ToString())
            assertEquals("a1c", StringReducer.reduceStringOrCategoricalColumn(StringReduceOp.MERGE, values).ToString())
            assertEquals("c1a", StringReducer.reduceStringOrCategoricalColumn(StringReduceOp.PREPEND, values).ToString())
            'REPLACE takes the second of (at most) two values
            assertThrows(Of ArgumentException)(Sub() StringReducer.reduceStringOrCategoricalColumn(StringReduceOp.REPLACE, values))
            assertEquals("b", StringReducer.reduceStringOrCategoricalColumn(StringReduceOp.REPLACE, New List(Of WritableT) From {New Text("a"), New Text("b")}).ToString())
            'The input list is not modified by PREPEND
            assertEquals("a", values(0).ToString())
        End Sub

        <TestMethod>
        Public Sub testTransformSchema()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("first").addColumnInteger("second").build()
            Dim reducer As StringReducer = (New StringReducer.Builder(StringReduceOp.APPEND)).inputColumns(New List(Of String) From {"first", "second"}).outputColumnName("joined").build()

            Dim outSchema As SchemaT = reducer.transform(schema)
            assertEquals(New List(Of String) From {"first", "second", "joined"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.String, ColumnType.Integer, ColumnType.String}, outSchema.getColumnTypes())
            assertEquals(New List(Of String) From {"first", "second"}, reducer.InputColumns)
            assertEquals("joined", reducer.getOutputColumnName())
            assertEquals(StringReduceOp.APPEND, reducer.getStringReduceOp())

            'A sequence schema stays a sequence schema
            Dim seqSchema As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("first").build()
            assertTrue(TypeOf reducer.transform(seqSchema) Is SequenceSchemaT)
        End Sub

        <TestMethod>
        Public Sub testEqualsAndToString()
            Dim a As StringReducer = (New StringReducer.Builder(StringReduceOp.APPEND)).inputColumns(New List(Of String) From {"x"}).outputColumnName("out").build()
            Dim b As StringReducer = (New StringReducer.Builder(StringReduceOp.APPEND)).inputColumns(New List(Of String) From {"x"}).outputColumnName("out").build()
            Dim c As StringReducer = (New StringReducer.Builder(StringReduceOp.PREPEND)).inputColumns(New List(Of String) From {"x"}).outputColumnName("out").build()
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertFalse(a.Equals(c))

            'The schema is not part of the equality
            a.InputSchema = (New SchemaT.Builder()).addColumnString("x").build()
            assertEquals(a, b)

            assertEquals("StringReducer(defaultOp=APPEND,customReductions={})", a.ToString())
        End Sub

    End Class

End Namespace
