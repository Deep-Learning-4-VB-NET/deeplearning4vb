Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.transforms.categorical
Imports DeepLearning4VB.org.datavec.api.writable
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.categorical

    ''' <summary>
    ''' Focused tests of the categorical transforms: the object/sequence paths, invalid input, equality and
    ''' <c>ToString</c>, and the Java <c>testFirstDigitTransform</c> data applied without a TransformProcess.
    ''' </summary>
    <TestClass>
    Public Class CategoricalTransformsTest

        Private Shared Function catSchema() As SchemaT
            Return TransformTestSupport.getSchema(ColumnType.Categorical, "zero", "one", "two")
        End Function

        <TestMethod>
        Public Sub testCategoricalToIntegerObjectSequenceAndErrors()
            Dim t As New CategoricalToIntegerTransform("column")
            t.InputSchema = catSchema()

            assertEquals(2, t.map(CObj("two")))
            assertNull(t.mapSequence(CObj(objList("zero"))))
            assertEquals(seqOf(row(New IntWritable(1)), row(New IntWritable(0))), t.mapSequence(seqOf(row(New Text("one")), row(New Text("zero")))))

            assertThrows(Of InvalidOperationException)(Sub() t.map(row(New Text("three"))))
            assertThrows(Of InvalidOperationException)(Sub() t.map(CObj("three")))
            assertThrows(Of InvalidOperationException)(Sub() t.map(row(New Text("zero"), New Text("one"))))

            Dim t2 As New CategoricalToIntegerTransform("column")
            Dim intSchema As SchemaT = TransformTestSupport.getSchema(ColumnType.Integer)
            assertThrows(Of InvalidOperationException)(Sub() t2.InputSchema = intSchema)

            assertArrayEquals(New String() {"column"}, t.columnNames())
            assertArrayEquals(New String() {"column"}, t.outputColumnNames())
            assertEquals("column", t.outputColumnName())
            assertEquals(0, t.getColumnIdx())
        End Sub

        <TestMethod>
        Public Sub testCategoricalToIntegerEqualityAndToString()
            Dim t As New CategoricalToIntegerTransform("column")
            assertEquals(New CategoricalToIntegerTransform("column"), t)
            assertEquals(New CategoricalToIntegerTransform("column").GetHashCode(), t.GetHashCode())
            assertNotEquals(New CategoricalToIntegerTransform("other"), t)

            t.InputSchema = catSchema()
            assertEquals("CategoricalToIntegerTransform(columnName=column, columnIdx=0, stateNames=[zero, one, two], statesMap={zero=0, one=1, two=2})", t.ToString())
        End Sub

        <TestMethod>
        Public Sub testCategoricalToOneHotObjectSequenceAndErrors()
            Dim t As New CategoricalToOneHotTransform("column")
            t.InputSchema = catSchema()

            assertEquals(Arrays.asList(0, 1, 0), t.map(CObj("one")))
            Dim seq As Object = t.mapSequence(CObj(objList("zero", "two")))
            assertEquals(New List(Of IList(Of Integer)) From {Arrays.asList(1, 0, 0), Arrays.asList(0, 0, 1)}, seq)

            assertThrows(Of InvalidOperationException)(Sub() t.map(CObj("three")))
            assertThrows(Of InvalidOperationException)(Sub() t.map(row(New Text("three"))))
            assertThrows(Of NotSupportedException)(Sub() t.outputColumnName())
            assertArrayEquals(New String() {"zero", "one", "two"}, t.outputColumnNames())
            assertArrayEquals(New String() {"column"}, t.columnNames())

            assertEquals("CategoricalToOneHotTransform(columnName=""column"")", t.ToString())
            assertEquals(New CategoricalToOneHotTransform("column"), t)
            assertNotEquals(New CategoricalToOneHotTransform("x"), t)
            assertNotEquals(New CategoricalToIntegerTransform("column"), t)
        End Sub

        ''' <summary>The data of the Java testFirstDigitTransform, applied without a TransformProcess.</summary>
        <TestMethod>
        Public Sub testFirstDigitTransform()
            Dim s As SchemaT = (New SchemaT.Builder()).addColumnString("data").addColumnDouble("double").addColumnString("stringNumber").build()

            Dim t1 As New FirstDigitTransform("double", "fdDouble", FirstDigitTransform.Mode.EXCEPTION_ON_INVALID)
            t1.InputSchema = s
            Dim s1 As SchemaT = t1.transform(s)
            Dim t2 As New FirstDigitTransform("stringNumber", "stringNumber", FirstDigitTransform.Mode.INCLUDE_OTHER_CATEGORY)
            t2.InputSchema = s1
            Dim s2 As SchemaT = t2.transform(s1)

            assertEquals(Arrays.asList("data", "double", "fdDouble", "stringNumber"), s2.getColumnNames())
            assertEquals(Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Categorical, ColumnType.Categorical), s2.getColumnTypes())
            assertEquals(10, DirectCast(s2.getMetaData(2), CategoricalMetaData).getStateNames().Count)
            assertEquals(11, DirectCast(s2.getMetaData(3), CategoricalMetaData).getStateNames().Count)
            assertEquals(FirstDigitTransform.OTHER_CATEGORY, DirectCast(s2.getMetaData(3), CategoricalMetaData).getStateNames()(10))

            Dim [in] As IList(Of IList(Of WritableT)) = seqOf(
                row(New Text("a"), New DoubleWritable(3.14159), New Text("8e-4")),
                row(New Text("b"), New DoubleWritable(2.71828), New Text("7e2")),
                row(New Text("c"), New DoubleWritable(1.61803), New Text("6e8")),
                row(New Text("c"), New DoubleWritable(-2), New Text("non numerical")))

            Dim expected As IList(Of IList(Of WritableT)) = seqOf(
                row(New Text("a"), New DoubleWritable(3.14159), New Text("3"), New Text("8")),
                row(New Text("b"), New DoubleWritable(2.71828), New Text("2"), New Text("7")),
                row(New Text("c"), New DoubleWritable(1.61803), New Text("1"), New Text("6")),
                row(New Text("c"), New DoubleWritable(-2), New Text("2"), New Text("Other")))

            Dim [out] As New List(Of IList(Of WritableT))()
            For Each r As IList(Of WritableT) In [in]
                [out].Add(t2.map(t1.map(r)))
            Next r
            assertEquals(expected, [out])

            'Invalid values with Mode.EXCEPTION_ON_INVALID; "Other" for an empty string with INCLUDE_OTHER_CATEGORY
            assertThrows(Of InvalidOperationException)(Sub() t1.map(row(New Text("x"), New Text("abc"), New Text("1"))))
            assertThrows(Of InvalidOperationException)(Sub() t1.map(row(New Text("x"), New Text(""), New Text("1"))))
            assertEquals(row(New Text("x"), New DoubleWritable(1), New Text("1"), New Text("Other")), t2.map(row(New Text("x"), New DoubleWritable(1), New Text("1"), New Text(""))))

            'Output column must not exist already (unless in place); input column must exist
            Dim t3 As New FirstDigitTransform("double", "data", FirstDigitTransform.Mode.EXCEPTION_ON_INVALID)
            assertThrows(Of InvalidOperationException)(Sub() t3.transform(s))
            Dim t4 As New FirstDigitTransform("missing", "x", FirstDigitTransform.Mode.EXCEPTION_ON_INVALID)
            assertThrows(Of InvalidOperationException)(Sub() t4.transform(s))

            assertThrows(Of NotSupportedException)(Sub() t1.map(CObj(1.5)))
            assertThrows(Of NotSupportedException)(Sub() t1.mapSequence(CObj(objList(1.5))))

            assertEquals("FirstDigitTransform(input=""double"",output=""fdDouble"",mode=EXCEPTION_ON_INVALID)", t1.ToString())
            assertEquals(New FirstDigitTransform("double", "fdDouble", FirstDigitTransform.Mode.EXCEPTION_ON_INVALID), t1)
            assertNotEquals(New FirstDigitTransform("double", "fdDouble", FirstDigitTransform.Mode.INCLUDE_OTHER_CATEGORY), t1)
            assertArrayEquals(New String() {"double"}, t1.columnNames())
            assertArrayEquals(New String() {"fdDouble"}, t1.outputColumnNames())
        End Sub

        <TestMethod>
        Public Sub testIntegerToCategoricalObjectSequenceAndEquality()
            Dim t As New IntegerToCategoricalTransform("column", Arrays.asList("zero", "one", "two"))
            t.InputSchema = TransformTestSupport.getSchema(ColumnType.Integer)

            assertEquals(New Text("one"), t.map(CObj(1)))
            assertEquals(New Text("two"), t.map(CObj("2")))
            assertEquals(Arrays.asList(Of Object)(New Text("zero"), New Text("two")), t.mapSequence(CObj(objList(0, 2))))

            assertEquals("IntegerToCategoricalTransform(map=[0=""zero"",1=""one"",2=""two""])", t.ToString())

            Dim map As IDictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
            map(2) = "two"
            map(0) = "zero"
            map(1) = "one"
            Dim t2 As New IntegerToCategoricalTransform("column", map)
            assertEquals(t2, t)
            assertEquals(t2.GetHashCode(), t.GetHashCode())
            assertNotEquals(New IntegerToCategoricalTransform("other", Arrays.asList("zero", "one", "two")), t)
            assertNotEquals(New IntegerToCategoricalTransform("column", Arrays.asList("zero", "one")), t)
        End Sub

        <TestMethod>
        Public Sub testPivotTransformDefaultValuesAndObjects()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnCategorical("key", "a", "b").addColumnInteger("value").build()

            Dim t As New PivotTransform("key", "value")
            assertNull(t.getDefaultValue())
            assertEquals("PivotTransform(keyColumn=key, valueColumn=value, defaultValue=null)", t.ToString())
            assertEquals(New PivotTransform("key", "value"), t)

            t.InputSchema = schema
            Dim [out] As SchemaT = t.transform(schema)
            assertEquals(Arrays.asList("key[a]", "key[b]"), [out].getColumnNames())
            assertEquals(Arrays.asList(ColumnType.Integer, ColumnType.Integer), [out].getColumnTypes())
            'Default value inferred from the value column type
            assertEquals(New IntWritable(0), t.getDefaultValue())

            assertEquals(row(New IntWritable(0), New IntWritable(5)), t.map(row(New Text("b"), New IntWritable(5))))
            assertEquals(Arrays.asList(Of WritableT)(New IntWritable(7), New IntWritable(0)), t.map(CObj(row(New Text("a"), New IntWritable(7)))))
            assertEquals(Arrays.asList(Of Object)(row(New IntWritable(0), New IntWritable(1))), t.mapSequence(CObj(objList(row(New Text("b"), New IntWritable(1))))))
            assertThrows(Of Exception)(Sub() t.map(row(New Text("c"), New IntWritable(5))))

            assertArrayEquals(New String() {"a", "b"}, t.outputColumnNames())
            assertArrayEquals(New String() {"key", "value"}, t.columnNames())
            assertThrows(Of NotSupportedException)(Sub() t.columnName())
            assertThrows(Of NotSupportedException)(Sub() t.outputColumnName())

            'Explicit default value
            Dim t2 As New PivotTransform("key", "value", NullWritable.INSTANCE)
            t2.InputSchema = schema
            t2.transform(schema)
            assertEquals(row(NullWritable.INSTANCE, New IntWritable(5)), t2.map(row(New Text("b"), New IntWritable(5))))

            'Missing key/value column
            Dim t3 As New PivotTransform("key", "missing")
            assertThrows(Of NotSupportedException)(Sub() t3.transform(schema))
        End Sub

        <TestMethod>
        Public Sub testStringToCategoricalTransformEquality()
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New StringToCategoricalTransform("column", New List(Of String)())
                                               End Sub)

            Dim t As New StringToCategoricalTransform("column", "a", "b")
            assertEquals(New StringToCategoricalTransform("column", Arrays.asList("a", "b")), t)
            assertNotEquals(New StringToCategoricalTransform("column", "a", "c"), t)
            assertNotEquals(New StringToCategoricalTransform("other", "a", "b"), t)
            assertEquals("StringToCategoricalTransform(stateNames=[a, b])", t.ToString())

            Dim input As Object = objList("a", "b")
            assertTrue(t.mapSequence(input) Is input)
            assertEquals("a", t.map(CObj("a")))
        End Sub

    End Class

End Namespace
