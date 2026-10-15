Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.condition
Imports DeepLearning4VB.org.datavec.api.transform.condition.column
Imports DeepLearning4VB.org.datavec.api.writable
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports NumberFormatException = DeepLearning4VB.JavaCompat.NumberFormatException
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.condition

    ''' <summary>
    ''' The single-column conditions beyond the ported JUnit cases: every <see cref="ConditionOp"/>, the
    ''' constructor validation, the accessors, the <c>condition(Object)</c> overloads, the sequence modes of
    ''' <see cref="BaseColumnCondition"/>, schema handling, equality and <c>ToString</c>.
    ''' </summary>
    <TestClass>
    Public Class ColumnConditionsTest

        Private Shared Function row(ByVal w As WritableT) As IList(Of WritableT)
            Return JavaCollections.singletonList(Of WritableT)(w)
        End Function

        Private Shared Function withSchema(Of T As ConditionT)(ByVal c As T, ByVal schema As SchemaT) As T
            c.InputSchema = schema
            Return c
        End Function

        <TestMethod>
        Public Sub testIntegerConditionOpsAndAccessors()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])
            Dim ops() As ConditionOp = {ConditionOp.LessThan, ConditionOp.LessOrEqual, ConditionOp.GreaterThan, ConditionOp.GreaterOrEqual, ConditionOp.Equal, ConditionOp.NotEqual}
            'value 5 against -1, 5 and 9 for each comparison
            Dim expected(,) As Boolean = {{True, False, False}, {True, True, False}, {False, False, True}, {False, True, True}, {False, True, False}, {True, False, True}}
            Dim inputs() As Integer = {-1, 5, 9}
            For o As Integer = 0 To ops.Length - 1
                Dim c As IntegerColumnCondition = withSchema(New IntegerColumnCondition("column", ops(o), 5), schema)
                assertEquals(ops(o), c.getOp())
                assertEquals(5, c.getValue().Value)
                assertNull(c.getSet())
                For i As Integer = 0 To inputs.Length - 1
                    assertEquals(expected(o, i), c.condition(row(New IntWritable(inputs(i)))), ops(o).ToString() & " " & inputs(i))
                    assertEquals(expected(o, i), c.columnCondition(New IntWritable(inputs(i))))
                    assertEquals(expected(o, i), c.condition(CObj(inputs(i))))
                Next i
            Next o
            'the default sequence mode is Or
            Dim c1 As New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
            assertEquals(SequenceConditionMode.[Or], c1.getSequenceMode())
            assertEquals(SequenceConditionMode.[Or], BaseColumnCondition.DEFAULT_SEQUENCE_CONDITION_MODE)
            assertEquals(SequenceConditionMode.[And], New IntegerColumnCondition("column", SequenceConditionMode.[And], ConditionOp.LessThan, 0).getSequenceMode())
            'set constructors
            Dim [set] As ISet(Of Integer) = New HashSet(Of Integer)() From {0, 3}
            Dim inSet As IntegerColumnCondition = withSchema(New IntegerColumnCondition("column", ConditionOp.NotInSet, [set]), schema)
            assertEquals(ConditionOp.NotInSet, inSet.getOp())
            assertNull(inSet.getValue())
            assertTrue(inSet.getSet() Is [set])
            assertTrue(inSet.condition(row(New IntWritable(1))))
            assertFalse(inSet.condition(row(New IntWritable(3))))
            assertFalse(inSet.condition(CObj(0)))
            assertEquals(SequenceConditionMode.[And], New IntegerColumnCondition("column", SequenceConditionMode.[And], ConditionOp.InSet, [set]).getSequenceMode())
            'constructor validation: value constructors reject the set ops and vice versa
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New IntegerColumnCondition("column", ConditionOp.InSet, 0)
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.NotInSet, 0)
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New IntegerColumnCondition("column", ConditionOp.Equal, [set])
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.LessThan, [set])
                                               End Sub)
            'toString
            assertEquals("IntegerColumnCondition(columnName=""column"",LessThan,0)", c1.ToString())
            assertEquals("IntegerColumnCondition(columnName=""column"",NotInSet,[0, 3])", inSet.ToString())
            'non-writable inputs are read as Java numbers
            assertTrue(c1.condition(CObj(-1L)))
            assertFalse(c1.condition(CObj(-0.5))) 'Java intValue() truncates -0.5 to 0, and 0 < 0 is false
            assertTrue(c1.condition(CObj(-1.5)))
            assertFalse(c1.condition(CObj(0.5)))
            assertThrows(Of InvalidCastException)(Sub() c1.condition(CObj("-1")))
        End Sub

        <TestMethod>
        Public Sub testLongAndTimeConditionOpsAndAccessors()
            Dim longSchema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Long])
            Dim lc As LongColumnCondition = withSchema(New LongColumnCondition("column", ConditionOp.GreaterThan, 10L), longSchema)
            assertEquals(ConditionOp.GreaterThan, lc.getOp())
            assertEquals(10L, lc.getValue().Value)
            assertNull(lc.getSet())
            assertTrue(lc.condition(row(New LongWritable(11))))
            assertFalse(lc.condition(row(New LongWritable(10))))
            assertTrue(lc.condition(row(New IntWritable(100))))
            assertTrue(lc.condition(row(New Text("12"))))
            assertTrue(lc.condition(CObj(11)))
            assertFalse(lc.condition(CObj(9.9)))
            assertTrue(withSchema(New LongColumnCondition("column", ConditionOp.LessOrEqual, Long.MinValue), longSchema).condition(row(New LongWritable(Long.MinValue))))
            assertTrue(withSchema(New LongColumnCondition("column", ConditionOp.GreaterOrEqual, 3L), longSchema).condition(row(New LongWritable(3))))
            assertTrue(withSchema(New LongColumnCondition("column", ConditionOp.Equal, 3L), longSchema).condition(row(New LongWritable(3))))
            assertFalse(withSchema(New LongColumnCondition("column", ConditionOp.LessThan, 3L), longSchema).condition(row(New LongWritable(3))))
            Dim lset As ISet(Of Long) = New HashSet(Of Long)() From {1L, 2L}
            Dim lin As LongColumnCondition = withSchema(New LongColumnCondition("column", SequenceConditionMode.[And], ConditionOp.InSet, lset), longSchema)
            assertTrue(lin.getSet() Is lset)
            assertNull(lin.getValue())
            assertEquals(SequenceConditionMode.[And], lin.getSequenceMode())
            assertTrue(lin.condition(row(New LongWritable(2))))
            assertFalse(lin.condition(row(New LongWritable(3))))
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New LongColumnCondition("column", ConditionOp.InSet, 1L)
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New LongColumnCondition("column", ConditionOp.GreaterThan, lset)
                                               End Sub)
            assertEquals("LongColumnCondition(columnName=""column"",GreaterThan,10)", lc.ToString())
            assertEquals("LongColumnCondition(columnName=""column"",InSet,[1, 2])", lin.ToString())

            Dim timeSchema As SchemaT = TransformTestSupport.getSchema(ColumnType.Time)
            Dim tc As TimeColumnCondition = withSchema(New TimeColumnCondition("column", ConditionOp.GreaterOrEqual, 1451606400000L), timeSchema)
            assertEquals(ConditionOp.GreaterOrEqual, tc.getOp())
            assertEquals(1451606400000L, tc.getValue().Value)
            assertNull(tc.getSet())
            assertEquals(SequenceConditionMode.[Or], tc.getSequenceMode())
            assertTrue(tc.condition(row(New LongWritable(1451606400000L))))
            assertFalse(tc.condition(row(New LongWritable(1451606399999L))))
            assertTrue(tc.condition(CObj(1451606400001L)))
            Dim tset As ISet(Of Long) = New HashSet(Of Long)() From {1451606400000L}
            Dim tin As TimeColumnCondition = withSchema(New TimeColumnCondition("column", SequenceConditionMode.[And], ConditionOp.NotInSet, tset), timeSchema)
            assertTrue(tin.getSet() Is tset)
            assertNull(tin.getValue())
            assertEquals(SequenceConditionMode.[And], tin.getSequenceMode())
            assertFalse(tin.condition(row(New LongWritable(1451606400000L))))
            assertTrue(tin.condition(row(New LongWritable(0))))
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New TimeColumnCondition("column", ConditionOp.NotInSet, 1L)
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New TimeColumnCondition("column", ConditionOp.Equal, tset)
                                               End Sub)
            assertEquals("TimeColumnCondition(columnName=""column"",GreaterOrEqual,1451606400000)", tc.ToString())
            assertEquals("TimeColumnCondition(columnName=""column"",NotInSet,[1451606400000])", tin.ToString())
        End Sub

        <TestMethod>
        Public Sub testDoubleAndFloatConditionOpsAndAccessors()
            Dim dSchema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Double])
            Dim dc As DoubleColumnCondition = withSchema(New DoubleColumnCondition("column", ConditionOp.LessThan, 0.5), dSchema)
            assertEquals(ConditionOp.LessThan, dc.getOp())
            assertEquals(0.5, dc.getValue().Value, 0.0)
            assertNull(dc.getSet())
            assertTrue(dc.condition(row(New DoubleWritable(0.25))))
            assertFalse(dc.condition(row(New DoubleWritable(0.5))))
            assertTrue(dc.condition(row(New IntWritable(0))))
            assertTrue(dc.condition(row(New Text("-1e3"))))
            assertFalse(dc.condition(row(New DoubleWritable(Double.NaN))))
            assertTrue(dc.condition(CObj(0.1F)))
            assertFalse(dc.condition(CObj(1)))
            'NaN is never equal, not even to itself
            assertFalse(withSchema(New DoubleColumnCondition("column", ConditionOp.Equal, Double.NaN), dSchema).condition(row(New DoubleWritable(Double.NaN))))
            assertTrue(withSchema(New DoubleColumnCondition("column", ConditionOp.NotEqual, Double.NaN), dSchema).condition(row(New DoubleWritable(Double.NaN))))
            assertTrue(withSchema(New DoubleColumnCondition("column", ConditionOp.GreaterThan, 1.0), dSchema).condition(row(New DoubleWritable(Double.PositiveInfinity))))
            Dim dset As ISet(Of Double) = New HashSet(Of Double)() From {0.5, 1.5}
            Dim din As DoubleColumnCondition = withSchema(New DoubleColumnCondition("column", SequenceConditionMode.[And], ConditionOp.InSet, dset), dSchema)
            assertTrue(din.getSet() Is dset)
            assertNull(din.getValue())
            assertTrue(din.condition(row(New DoubleWritable(1.5))))
            assertFalse(din.condition(row(New DoubleWritable(1.0))))
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New DoubleColumnCondition("column", ConditionOp.InSet, 1.0)
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New DoubleColumnCondition("column", ConditionOp.LessThan, dset)
                                               End Sub)
            assertEquals("DoubleColumnCondition(columnName=""column"",LessThan,0.5)", dc.ToString())
            assertEquals("DoubleColumnCondition(columnName=""column"",InSet,[0.5, 1.5])", din.ToString())
            assertEquals("DoubleColumnCondition(columnName=""column"",GreaterOrEqual,0.0)", New DoubleColumnCondition("column", ConditionOp.GreaterOrEqual, 0).ToString())

            Dim fSchema As SchemaT = TransformTestSupport.getSchema(ColumnType.Float)
            Dim fc As FloatColumnCondition = withSchema(New FloatColumnCondition("column", ConditionOp.GreaterOrEqual, 0.5F), fSchema)
            assertEquals(ConditionOp.GreaterOrEqual, fc.getOp())
            assertEquals(0.5F, fc.getValue().Value, 0.0F)
            assertNull(fc.getSet())
            assertTrue(fc.condition(row(New FloatWritable(0.5F))))
            assertFalse(fc.condition(row(New FloatWritable(0.49F))))
            assertTrue(fc.condition(row(New DoubleWritable(2.0))))
            assertTrue(fc.condition(row(New Text("1"))))
            assertTrue(fc.condition(CObj(0.5)))
            assertFalse(fc.condition(CObj(0)))
            Dim fset As ISet(Of Single) = New HashSet(Of Single)() From {0.5F}
            Dim fin As FloatColumnCondition = withSchema(New FloatColumnCondition("column", ConditionOp.NotInSet, fset), fSchema)
            assertTrue(fin.getSet() Is fset)
            assertNull(fin.getValue())
            assertFalse(fin.condition(row(New FloatWritable(0.5F))))
            assertTrue(fin.condition(row(New FloatWritable(0.25F))))
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New FloatColumnCondition("column", ConditionOp.NotInSet, 1.0F)
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New FloatColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.Equal, fset)
                                               End Sub)
            assertEquals("FloatColumnCondition(columnName=""column"",GreaterOrEqual,0.5)", fc.ToString())
            assertEquals("FloatColumnCondition(columnName=""column"",NotInSet,[0.5])", fin.ToString())
        End Sub

        <TestMethod>
        Public Sub testStringAndCategoricalConditionOps()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[String])
            Dim eq As StringColumnCondition = withSchema(New StringColumnCondition("column", ConditionOp.Equal, "value"), schema)
            assertEquals(ConditionOp.Equal, eq.getOp())
            assertEquals("value", eq.getValue())
            assertNull(eq.getSet())
            assertTrue(eq.condition(row(New Text("value"))))
            assertFalse(eq.condition(row(New Text("Value"))))
            assertTrue(eq.condition(CObj("value")))
            assertFalse(eq.condition(CObj(1)))
            Dim ne As StringColumnCondition = withSchema(New StringColumnCondition("column", SequenceConditionMode.[And], ConditionOp.NotEqual, "5"), schema)
            assertEquals(SequenceConditionMode.[And], ne.getSequenceMode())
            assertTrue(ne.condition(row(New Text("6"))))
            assertFalse(ne.condition(row(New IntWritable(5)))) 'compared through the text form
            assertFalse(ne.condition(CObj(5)))
            Dim sset As ISet(Of String) = New HashSet(Of String)() From {"a", "b"}
            Dim notIn As StringColumnCondition = withSchema(New StringColumnCondition("column", ConditionOp.NotInSet, sset), schema)
            assertTrue(notIn.getSet() Is sset)
            assertNull(notIn.getValue())
            assertTrue(notIn.condition(row(New Text("c"))))
            assertFalse(notIn.condition(row(New Text("a"))))
            'only Equal/NotEqual with a value, only InSet/NotInSet with a set
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New StringColumnCondition("column", ConditionOp.LessThan, "x")
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New StringColumnCondition("column", ConditionOp.InSet, "x")
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New StringColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.Equal, sset)
                                               End Sub)
            assertEquals("StringColumnCondition(columnName=""column"",Equal,value)", eq.ToString())
            Dim text As String = notIn.ToString()
            assertTrue(text.StartsWith("StringColumnCondition(columnName=""column"",NotInSet,[", StringComparison.Ordinal), text)
            assertTrue(text.Contains("a") AndAlso text.Contains("b") AndAlso text.EndsWith("])", StringComparison.Ordinal), text)

            Dim catSchema As SchemaT = TransformTestSupport.getSchema(ColumnType.Categorical, "alpha", "beta", "gamma")
            Dim ceq As CategoricalColumnCondition = withSchema(New CategoricalColumnCondition("column", ConditionOp.NotEqual, "alpha"), catSchema)
            assertEquals(ConditionOp.NotEqual, ceq.getOp())
            assertEquals("alpha", ceq.getValue())
            assertNull(ceq.getSet())
            assertFalse(ceq.condition(row(New Text("alpha"))))
            assertTrue(ceq.condition(row(New Text("beta"))))
            assertTrue(ceq.condition(CObj("gamma")))
            Dim cset As ISet(Of String) = New HashSet(Of String)() From {"beta", "gamma"}
            Dim cin As CategoricalColumnCondition = withSchema(New CategoricalColumnCondition("column", SequenceConditionMode.[And], ConditionOp.InSet, cset), catSchema)
            assertTrue(cin.getSet() Is cset)
            assertNull(cin.getValue())
            assertEquals(SequenceConditionMode.[And], cin.getSequenceMode())
            assertTrue(cin.condition(row(New Text("gamma"))))
            assertFalse(cin.condition(row(New Text("alpha"))))
            assertTrue(cin.condition(CObj("beta")))
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New CategoricalColumnCondition("column", ConditionOp.GreaterThan, "x")
                                               End Sub)
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New CategoricalColumnCondition("column", ConditionOp.NotEqual, cset)
                                               End Sub)
            assertEquals("CategoricalColumnCondition(columnName=""column"",NotEqual,alpha)", ceq.ToString())
            Dim ctext As String = cin.ToString()
            assertTrue(ctext.StartsWith("CategoricalColumnCondition(columnName=""column"",InSet,[", StringComparison.Ordinal), ctext)
            assertTrue(ctext.Contains("beta") AndAlso ctext.Contains("gamma") AndAlso ctext.EndsWith("])", StringComparison.Ordinal), ctext)
        End Sub

        <TestMethod>
        Public Sub testBooleanColumnCondition()
            Dim schema As SchemaT = New SchemaT.Builder().addColumnBoolean("column").build()
            Dim c As BooleanColumnCondition = withSchema(New BooleanColumnCondition("column", SequenceConditionMode.[Or]), schema)
            assertTrue(c.condition(row(New BooleanWritable(True))))
            assertFalse(c.condition(row(New BooleanWritable(False))))
            assertTrue(c.columnCondition(New BooleanWritable(True)))
            assertTrue(c.condition(CObj(True)))
            assertFalse(c.condition(CObj(False)))
            'the Java toString is the class name
            assertEquals(GetType(BooleanColumnCondition).ToString(), c.ToString())
            'only boolean writables are accepted (Java: ClassCastException)
            assertThrows(Of InvalidCastException)(Sub() c.condition(row(New IntWritable(1))))
            assertEquals(c, New BooleanColumnCondition("column", SequenceConditionMode.[And]))
            assertNotEquals(c, New BooleanColumnCondition("other", SequenceConditionMode.[Or]))
        End Sub

        <TestMethod>
        Public Sub testNaNAndInfiniteColumnConditions()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Double])
            Dim nan As NaNColumnCondition = withSchema(New NaNColumnCondition("column"), schema)
            assertEquals(SequenceConditionMode.[Or], nan.getSequenceMode())
            assertTrue(nan.condition(row(New DoubleWritable(Double.NaN))))
            assertTrue(nan.condition(row(New FloatWritable(Single.NaN))))
            assertTrue(nan.condition(row(New Text("NaN"))))
            assertFalse(nan.condition(row(New DoubleWritable(1.0))))
            assertFalse(nan.condition(row(New DoubleWritable(Double.PositiveInfinity))))
            assertFalse(nan.condition(row(New IntWritable(0))))
            assertTrue(nan.condition(CObj(Double.NaN)))
            assertFalse(nan.condition(CObj(1)))
            assertThrows(Of NumberFormatException)(Sub() nan.condition(row(New Text("x"))))
            assertEquals("NaNColumnCondition()", nan.ToString())
            assertEquals(SequenceConditionMode.[And], New NaNColumnCondition("column", SequenceConditionMode.[And]).getSequenceMode())
            assertEquals(nan, New NaNColumnCondition("column"))

            Dim inf As InfiniteColumnCondition = withSchema(New InfiniteColumnCondition("column"), schema)
            assertEquals(SequenceConditionMode.[Or], inf.getSequenceMode())
            assertTrue(inf.condition(row(New DoubleWritable(Double.PositiveInfinity))))
            assertTrue(inf.condition(row(New DoubleWritable(Double.NegativeInfinity))))
            assertTrue(inf.condition(row(New FloatWritable(Single.NegativeInfinity))))
            assertTrue(inf.condition(row(New Text("Infinity"))))
            assertFalse(inf.condition(row(New DoubleWritable(Double.NaN))))
            assertFalse(inf.condition(row(New DoubleWritable(Double.MaxValue))))
            assertTrue(inf.condition(CObj(Double.NegativeInfinity)))
            assertFalse(inf.condition(CObj(1.0)))
            assertEquals("InfiniteColumnCondition()", inf.ToString())
            assertEquals(SequenceConditionMode.NoSequenceMode, New InfiniteColumnCondition("column", SequenceConditionMode.NoSequenceMode).getSequenceMode())
            assertNotEquals(nan, inf)
        End Sub

        <TestMethod>
        Public Sub testNullWritableAndTrivialConditions()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])
            Dim nullCond As NullWritableColumnCondition = withSchema(New NullWritableColumnCondition("column"), schema)
            assertTrue(nullCond.columnCondition(NullWritable.INSTANCE))
            assertFalse(nullCond.columnCondition(New Text("NullWritable")))
            assertTrue(nullCond.condition(CObj(Nothing)))
            assertFalse(nullCond.condition(CObj(0)))
            assertFalse(nullCond.condition(CObj(NullWritable.INSTANCE))) 'the object form only tests for null
            assertEquals("NullWritableColumnCondition()", nullCond.ToString())
            assertEquals(SequenceConditionMode.[Or], nullCond.getSequenceMode())
            assertEquals(nullCond, New NullWritableColumnCondition("column"))

            'the trivial condition is always satisfied, even without a schema
            Dim trivial As New TrivialColumnCondition("column")
            assertTrue(trivial.columnCondition(NullWritable.INSTANCE))
            assertTrue(trivial.columnCondition(New IntWritable(-1)))
            assertTrue(trivial.condition(row(New IntWritable(-1))))
            assertTrue(trivial.condition(New List(Of WritableT)()))
            assertTrue(trivial.condition(CObj(Nothing)))
            assertTrue(trivial.condition(CObj("x")))
            assertEquals("Trivial(column)", trivial.ToString())
            assertEquals("column", trivial.getColumnName())
            assertEquals(-1, trivial.getColumnIdx())
            assertNull(trivial.getSchema())
            trivial.InputSchema = schema
            assertEquals(0, trivial.getColumnIdx())
            assertEquals(trivial, New TrivialColumnCondition("column"))
            assertNotEquals(trivial, New TrivialColumnCondition("other"))
            assertNotEquals(trivial, nullCond)
        End Sub

        <TestMethod>
        Public Sub testInvalidValueConditionOnObjects()
            Dim schema As SchemaT = New SchemaT.Builder().addColumnInteger("i", 0, 10).addColumnString("s", "[a-z]+", 1, 3).build()
            Dim onInt As InvalidValueColumnCondition = withSchema(New InvalidValueColumnCondition("i"), schema)
            assertEquals(1, onInt.getColumnIdx() + 1)
            assertEquals(SequenceConditionMode.[Or], onInt.getSequenceMode())
            'the object overload wraps the value in the matching writable
            assertFalse(onInt.condition(CObj(5)))
            assertTrue(onInt.condition(CObj(11)))
            assertFalse(onInt.condition("5"))
            assertTrue(onInt.condition("abc"))
            assertTrue(onInt.condition(CObj(5.5)))
            assertTrue(onInt.condition(CObj(99L)))
            assertTrue(onInt.condition(CObj(True)))
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() onInt.condition(CObj(1.5F)))
            assertTrue(ex.Message.StartsWith("Illegal type", StringComparison.Ordinal), ex.Message)
            'the second column: the condition applies to its own column of the row
            Dim onString As InvalidValueColumnCondition = withSchema(New InvalidValueColumnCondition("s"), schema)
            assertEquals(1, onString.getColumnIdx())
            assertFalse(onString.condition(Arrays.asList(Of WritableT)(New IntWritable(99), New Text("abc"))))
            assertTrue(onString.condition(Arrays.asList(Of WritableT)(New IntWritable(1), New Text("abcd"))))
            assertTrue(onString.condition(Arrays.asList(Of WritableT)(New IntWritable(1), New Text("ABC"))))
            assertFalse(onString.condition("ab"))
            assertTrue(onString.condition(CObj(5)))
            assertEquals("InvalidValueColumnCondition(columnName=""s"")", onString.ToString())
            assertEquals(onString, New InvalidValueColumnCondition("s"))
            assertNotEquals(onString, onInt)
        End Sub

        <TestMethod>
        Public Sub testSequenceConditionModes()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])
            Dim allNegative As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(New IList(Of WritableT)() {row(New IntWritable(-1)), row(New IntWritable(-2))})
            Dim someNegative As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(New IList(Of WritableT)() {row(New IntWritable(1)), row(New IntWritable(-2))})
            Dim nonNegative As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(New IList(Of WritableT)() {row(New IntWritable(1)), row(New IntWritable(2))})
            Dim emptySeq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()

            Dim orCond As IntegerColumnCondition = withSchema(New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.LessThan, 0), schema)
            assertTrue(orCond.conditionSequence(allNegative))
            assertTrue(orCond.conditionSequence(someNegative))
            assertFalse(orCond.conditionSequence(nonNegative))
            assertFalse(orCond.conditionSequence(emptySeq))

            Dim andCond As IntegerColumnCondition = withSchema(New IntegerColumnCondition("column", SequenceConditionMode.[And], ConditionOp.LessThan, 0), schema)
            assertTrue(andCond.conditionSequence(allNegative))
            assertFalse(andCond.conditionSequence(someNegative))
            assertFalse(andCond.conditionSequence(nonNegative))
            assertTrue(andCond.conditionSequence(emptySeq))

            Dim noMode As IntegerColumnCondition = withSchema(New IntegerColumnCondition("column", SequenceConditionMode.NoSequenceMode, ConditionOp.LessThan, 0), schema)
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() noMode.conditionSequence(allNegative))
            assertTrue(ex.Message.Contains("does not support sequence execution"), ex.Message)
            assertTrue(noMode.condition(row(New IntWritable(-1)))) 'single examples still work

            'the mode can be changed afterwards
            noMode.setSequenceMode(SequenceConditionMode.[Or])
            assertEquals(SequenceConditionMode.[Or], noMode.getSequenceMode())
            assertTrue(noMode.conditionSequence(someNegative))
            noMode.setSequenceMode(SequenceConditionMode.[And])
            assertFalse(noMode.conditionSequence(someNegative))

            'the object form of a sequence is a list of plain values
            Dim objAll As Object = New List(Of Object)(New Object() {-1, -2L})
            Dim objSome As Object = New List(Of Object)(New Object() {1, -2.5})
            assertTrue(orCond.conditionSequence(objAll))
            assertTrue(orCond.conditionSequence(objSome))
            assertFalse(orCond.conditionSequence(CObj(New List(Of Object)(New Object() {1, 2}))))
            assertTrue(andCond.conditionSequence(objAll))
            assertFalse(andCond.conditionSequence(objSome))
            assertTrue(andCond.conditionSequence(CObj(New List(Of Object)())))
            Dim noMode2 As New IntegerColumnCondition("column", SequenceConditionMode.NoSequenceMode, ConditionOp.LessThan, 0)
            assertThrows(Of InvalidOperationException)(Sub() noMode2.conditionSequence(objAll))
            assertThrows(Of InvalidCastException)(Sub() orCond.conditionSequence(CObj("not a list")))
            'the sequence mode does not take part in equality
            assertEquals(orCond, andCond)
            assertEquals(orCond.GetHashCode(), andCond.GetHashCode())
        End Sub

        <TestMethod>
        Public Sub testColumnConditionSchemaHandling()
            Dim schema As SchemaT = New SchemaT.Builder().addColumnString("first").addColumnInteger("second").addColumnDouble("third").build()
            Dim c As New IntegerColumnCondition("second", ConditionOp.GreaterThan, 0)
            'before the schema is set
            assertEquals("second", c.getColumnName())
            assertEquals("second", c.columnName())
            assertArrayEquals(New String() {"second"}, c.columnNames())
            assertEquals("second", c.outputColumnName())
            assertArrayEquals(New String() {"second"}, c.outputColumnNames())
            assertEquals(-1, c.getColumnIdx())
            assertNull(c.getSchema())
            assertNull(c.InputSchema)
            'a condition without a schema cannot index the row
            assertThrows(Of ArgumentOutOfRangeException)(Sub() c.condition(Arrays.asList(Of WritableT)(New Text("x"), New IntWritable(1), New DoubleWritable(0))))
            'setting the schema resolves the column index
            c.InputSchema = schema
            assertEquals(1, c.getColumnIdx())
            assertTrue(c.getSchema() Is schema)
            assertTrue(c.InputSchema Is schema)
            assertTrue(c.transform(schema) Is schema)
            assertTrue(c.condition(Arrays.asList(Of WritableT)(New Text("x"), New IntWritable(1), New DoubleWritable(-5))))
            assertFalse(c.condition(Arrays.asList(Of WritableT)(New Text("x"), New IntWritable(0), New DoubleWritable(5))))
            'the condition only looks at its own column
            assertTrue(c.condition(Arrays.asList(Of WritableT)(NullWritable.INSTANCE, New IntWritable(3), NullWritable.INSTANCE)))
            'a schema without the column is rejected and leaves the previous one in place
            Dim other As SchemaT = New SchemaT.Builder().addColumnInteger("unrelated").build()
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() c.InputSchema = other)
            assertTrue(ex.Message.Contains("""second"""), ex.Message)
            assertTrue(c.getSchema() Is schema)
            'the first matching column name is used
            Dim reordered As SchemaT = New SchemaT.Builder().addColumnInteger("second").addColumnString("first").build()
            c.InputSchema = reordered
            assertEquals(0, c.getColumnIdx())
            assertTrue(c.condition(Arrays.asList(Of WritableT)(New IntWritable(1), New Text("x"))))
        End Sub

        <TestMethod>
        Public Sub testColumnConditionEquality()
            Dim a As New IntegerColumnCondition("c", ConditionOp.LessThan, 0)
            Dim b As New IntegerColumnCondition("c", SequenceConditionMode.[And], ConditionOp.LessThan, 0)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertTrue(a.Equals(a))
            assertFalse(a.Equals(Nothing))
            assertFalse(a.Equals("x"))
            assertNotEquals(a, New IntegerColumnCondition("c", ConditionOp.LessOrEqual, 0))
            assertNotEquals(a, New IntegerColumnCondition("c", ConditionOp.LessThan, 1))
            assertNotEquals(a, New IntegerColumnCondition("d", ConditionOp.LessThan, 0))
            'a different condition class on the same column is a different condition
            assertNotEquals(a, New LongColumnCondition("c", ConditionOp.LessThan, 0L))
            assertNotEquals(a, New DoubleColumnCondition("c", ConditionOp.LessThan, 0.0))
            'the schema does not take part in equality
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])
            Dim withS As New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
            withS.InputSchema = schema
            assertEquals(New IntegerColumnCondition("column", ConditionOp.LessThan, 0), withS)
            'sets are compared by content
            Dim s1 As ISet(Of Integer) = New HashSet(Of Integer)() From {1, 2}
            Dim s2 As ISet(Of Integer) = New HashSet(Of Integer)() From {2, 1}
            Dim inA As New IntegerColumnCondition("c", ConditionOp.InSet, s1)
            Dim inB As New IntegerColumnCondition("c", ConditionOp.InSet, s2)
            assertEquals(inA, inB)
            assertEquals(inA.GetHashCode(), inB.GetHashCode())
            assertNotEquals(inA, New IntegerColumnCondition("c", ConditionOp.NotInSet, s1))
            assertNotEquals(inA, New IntegerColumnCondition("c", ConditionOp.InSet, New HashSet(Of Integer)() From {1}))
            assertNotEquals(inA, a)
            'the same for the other typed conditions
            assertEquals(New DoubleColumnCondition("c", ConditionOp.InSet, New HashSet(Of Double)() From {1.0, 2.0}), New DoubleColumnCondition("c", ConditionOp.InSet, New HashSet(Of Double)() From {2.0, 1.0}))
            assertEquals(New StringColumnCondition("c", ConditionOp.InSet, New HashSet(Of String)() From {"a"}), New StringColumnCondition("c", ConditionOp.InSet, New HashSet(Of String)() From {"a"}))
            assertNotEquals(New StringColumnCondition("c", ConditionOp.Equal, "a"), New StringColumnCondition("c", ConditionOp.Equal, "b"))
            assertEquals(New CategoricalColumnCondition("c", ConditionOp.Equal, "a"), New CategoricalColumnCondition("c", SequenceConditionMode.[And], ConditionOp.Equal, "a"))
            assertNotEquals(New CategoricalColumnCondition("c", ConditionOp.Equal, "a"), New StringColumnCondition("c", ConditionOp.Equal, "a"))
            assertEquals(New FloatColumnCondition("c", ConditionOp.Equal, 1.0F), New FloatColumnCondition("c", ConditionOp.Equal, 1.0F))
            assertNotEquals(New FloatColumnCondition("c", ConditionOp.Equal, 1.0F), New FloatColumnCondition("c", ConditionOp.Equal, 2.0F))
            assertEquals(New TimeColumnCondition("c", ConditionOp.Equal, 1L), New TimeColumnCondition("c", ConditionOp.Equal, 1L))
            assertNotEquals(New TimeColumnCondition("c", ConditionOp.Equal, 1L), New LongColumnCondition("c", ConditionOp.Equal, 1L))
        End Sub

    End Class

End Namespace
