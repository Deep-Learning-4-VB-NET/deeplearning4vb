Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.transform
Imports DeepLearning4VB.org.datavec.api.transform.condition
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

Namespace org.datavec.api.transform

    ''' <summary>
    ''' The top-level enums of the transform package (<see cref="ColumnType"/> and its Java instance methods,
    ''' <see cref="MathOp"/>, <see cref="ReduceOp"/>, <see cref="StringReduceOp"/>, <see cref="MathFunction"/>,
    ''' <see cref="Distance"/>, <see cref="ConditionOp"/>) and the Java number/string helpers behind them.
    ''' </summary>
    <TestClass>
    Public Class TransformEnumsTest

        <TestMethod>
        Public Sub testColumnTypeValuesAndOrdinals()
            'the Java declaration order (ordinals are used by the serde)
            Dim expected() As String = {"String", "Integer", "Long", "Double", "Float", "Categorical", "Time", "Bytes", "Boolean", "NDArray"}
            assertArrayEquals(expected, [Enum].GetNames(GetType(ColumnType)))
            Dim values() As ColumnType = ColumnTypeExtensions.ColumnTypeValues()
            assertEquals(10, values.Length)
            For i As Integer = 0 To values.Length - 1
                assertEquals(i, CInt(values(i)))
                assertEquals(expected(i), values(i).ToString())
            Next i
            assertEquals(ColumnType.Categorical, values(5))
        End Sub

        <TestMethod>
        Public Sub testColumnTypeValueOf()
            assertEquals(ColumnType.[Integer], ColumnTypeExtensions.ColumnTypeValueOf("Integer"))
            assertEquals(ColumnType.NDArray, ColumnTypeExtensions.ColumnTypeValueOf("NDArray"))
            'Java Enum.valueOf is case sensitive and rejects unknown names
            assertThrows(Of ArgumentException)(Sub() ColumnTypeExtensions.ColumnTypeValueOf("integer"))
            assertThrows(Of ArgumentException)(Sub() ColumnTypeExtensions.ColumnTypeValueOf("Nope"))
            assertThrows(Of ArgumentException)(Sub() ColumnTypeExtensions.ColumnTypeValueOf("3"))
        End Sub

        <TestMethod>
        Public Sub testColumnTypeNewColumnMetaData()
            Dim s As ColumnMetaData = ColumnType.[String].newColumnMetaData("s")
            assertTrue(TypeOf s Is StringMetaData)
            assertEquals("s", s.Name)
            assertEquals(ColumnType.[String], s.ColumnType)
            assertTrue(TypeOf ColumnType.[Integer].newColumnMetaData("i") Is IntegerMetaData)
            assertTrue(TypeOf ColumnType.[Long].newColumnMetaData("l") Is LongMetaData)
            assertTrue(TypeOf ColumnType.[Double].newColumnMetaData("d") Is DoubleMetaData)
            assertTrue(TypeOf ColumnType.Float.newColumnMetaData("f") Is FloatMetaData)
            Dim t As ColumnMetaData = ColumnType.Time.newColumnMetaData("t")
            assertTrue(TypeOf t Is TimeMetaData)
            assertEquals("UTC", DirectCast(t, TimeMetaData).getTimeZone().getID())
            'Java quirk: a Boolean column is created as a categorical column with the states "true" and "false"
            Dim b As ColumnMetaData = ColumnType.[Boolean].newColumnMetaData("b")
            assertTrue(TypeOf b Is CategoricalMetaData)
            assertEquals(Arrays.asList("true", "false"), DirectCast(b, CategoricalMetaData).getStateNames())
            assertEquals(ColumnType.Categorical, b.ColumnType)
            'no default metadata without state names, shape or for raw bytes
            assertThrows(Of NotSupportedException)(Sub() ColumnType.Categorical.newColumnMetaData("c"))
            assertThrows(Of NotSupportedException)(Sub() ColumnType.NDArray.newColumnMetaData("n"))
            assertThrows(Of NotSupportedException)(Sub() ColumnType.Bytes.newColumnMetaData("x"))
        End Sub

        <TestMethod>
        Public Sub testColumnTypeWritableTypes()
            assertEquals(WritableType.Text, ColumnType.[String].getWritableType())
            assertEquals(WritableType.Int, ColumnType.[Integer].getWritableType())
            assertEquals(WritableType.[Long], ColumnType.[Long].getWritableType())
            assertEquals(WritableType.[Double], ColumnType.[Double].getWritableType())
            assertEquals(WritableType.Float, ColumnType.Float.getWritableType())
            assertEquals(WritableType.Text, ColumnType.Categorical.getWritableType())
            assertEquals(WritableType.[Long], ColumnType.Time.getWritableType())
            assertEquals(WritableType.[Byte], ColumnType.Bytes.getWritableType())
            assertEquals(WritableType.[Boolean], ColumnType.[Boolean].getWritableType())
            assertEquals(WritableType.Image, ColumnType.NDArray.getWritableType())
            assertThrows(Of InvalidOperationException)(Sub() CType(99, ColumnType).getWritableType())
            assertThrows(Of NotSupportedException)(Sub() CType(99, ColumnType).newColumnMetaData("x"))
        End Sub

        <TestMethod>
        Public Sub testOperationEnums()
            assertArrayEquals(New String() {"Add", "Subtract", "Multiply", "Divide", "Modulus", "ReverseSubtract", "ReverseDivide", "ScalarMin", "ScalarMax"},
                              [Enum].GetNames(GetType(MathOp)))
            assertArrayEquals(New String() {"Prod", "Min", "Max", "Range", "Append", "Prepend", "Sum", "Mean", "Stdev", "UncorrectedStdDev",
                                            "Variance", "PopulationVariance", "Count", "CountUnique", "TakeFirst", "TakeLast"},
                              [Enum].GetNames(GetType(ReduceOp)))
            assertArrayEquals(New String() {"PREPEND", "APPEND", "MERGE", "REPLACE"}, [Enum].GetNames(GetType(StringReduceOp)))
            assertArrayEquals(New String() {"ABS", "ACOS", "ASIN", "ATAN", "CEIL", "COS", "COSH", "EXP", "FLOOR", "LOG", "LOG10", "SIGNUM", "SIN", "SINH", "SQRT", "TAN", "TANH"},
                              [Enum].GetNames(GetType(MathFunction)))
            assertArrayEquals(New String() {"COSINE", "EUCLIDEAN", "MANHATTAN"}, [Enum].GetNames(GetType(Distance)))
            assertArrayEquals(New String() {"LessThan", "LessOrEqual", "GreaterThan", "GreaterOrEqual", "Equal", "NotEqual", "InSet", "NotInSet"},
                              [Enum].GetNames(GetType(ConditionOp)))
            assertArrayEquals(New String() {"And", "Or", "NoSequenceMode"}, [Enum].GetNames(GetType(SequenceConditionMode)))
            'ordinals follow the declaration order
            assertEquals(0, CInt(MathOp.Add))
            assertEquals(8, CInt(MathOp.ScalarMax))
            assertEquals(15, CInt(ReduceOp.TakeLast))
            assertEquals(7, CInt(ConditionOp.NotInSet))
        End Sub

        <TestMethod>
        Public Sub testConditionOpApplyNumeric()
            Dim dset As ISet(Of Double) = New HashSet(Of Double)() From {1.0, 2.5}
            assertTrue(ConditionOp.LessThan.apply(1.0, 2.0, Nothing))
            assertFalse(ConditionOp.LessThan.apply(2.0, 2.0, Nothing))
            assertTrue(ConditionOp.LessOrEqual.apply(2.0, 2.0, Nothing))
            assertFalse(ConditionOp.LessOrEqual.apply(2.1, 2.0, Nothing))
            assertTrue(ConditionOp.GreaterThan.apply(3.0, 2.0, Nothing))
            assertFalse(ConditionOp.GreaterThan.apply(2.0, 2.0, Nothing))
            assertTrue(ConditionOp.GreaterOrEqual.apply(2.0, 2.0, Nothing))
            assertTrue(ConditionOp.Equal.apply(2.0, 2.0, Nothing))
            assertFalse(ConditionOp.Equal.apply(Double.NaN, Double.NaN, Nothing))
            assertTrue(ConditionOp.NotEqual.apply(2.0, 3.0, Nothing))
            assertTrue(ConditionOp.InSet.apply(2.5, 0.0, dset))
            assertFalse(ConditionOp.InSet.apply(2.0, 0.0, dset))
            assertTrue(ConditionOp.NotInSet.apply(2.0, 0.0, dset))
            assertFalse(ConditionOp.NotInSet.apply(1.0, 0.0, dset))

            Dim iset As ISet(Of Integer) = New HashSet(Of Integer)() From {1, 2}
            assertTrue(ConditionOp.LessThan.apply(1, 2, Nothing))
            assertTrue(ConditionOp.InSet.apply(2, 0, iset))
            assertFalse(ConditionOp.NotInSet.apply(2, 0, iset))
            assertTrue(ConditionOp.GreaterOrEqual.apply(Integer.MaxValue, Integer.MaxValue, Nothing))

            Dim lset As ISet(Of Long) = New HashSet(Of Long)() From {5L}
            assertTrue(ConditionOp.Equal.apply(5L, 5L, Nothing))
            assertTrue(ConditionOp.InSet.apply(5L, 0L, lset))
            assertTrue(ConditionOp.NotEqual.apply(Long.MaxValue, Long.MinValue, Nothing))

            Dim fset As ISet(Of Single) = New HashSet(Of Single)() From {0.5F}
            assertTrue(ConditionOp.LessOrEqual.apply(0.5F, 0.5F, Nothing))
            assertTrue(ConditionOp.InSet.apply(0.5F, 0.0F, fset))
            assertFalse(ConditionOp.InSet.apply(0.25F, 0.0F, fset))
        End Sub

        <TestMethod>
        Public Sub testConditionOpApplyString()
            Dim sset As ISet(Of String) = New HashSet(Of String)() From {"a", "b"}
            assertTrue(ConditionOp.Equal.apply("a", "a", Nothing))
            assertFalse(ConditionOp.Equal.apply("A", "a", Nothing))
            assertTrue(ConditionOp.NotEqual.apply("A", "a", Nothing))
            assertTrue(ConditionOp.InSet.apply("b", Nothing, sset))
            assertFalse(ConditionOp.InSet.apply("c", Nothing, sset))
            assertTrue(ConditionOp.NotInSet.apply("c", Nothing, sset))
            'ordering comparisons are not defined on strings
            For Each op As ConditionOp In New ConditionOp() {ConditionOp.LessThan, ConditionOp.LessOrEqual, ConditionOp.GreaterThan, ConditionOp.GreaterOrEqual}
                Dim ex As NotSupportedException = assertThrows(Of NotSupportedException)(Sub() Call op.apply("a", "b", Nothing))
                assertTrue(ex.Message.Contains(op.ToString()), ex.Message)
            Next op
        End Sub

        <TestMethod>
        Public Sub testConditionOpValueOf()
            assertEquals(ConditionOp.GreaterOrEqual, ConditionOpExtensions.ConditionOpValueOf("GreaterOrEqual"))
            assertEquals(8, ConditionOpExtensions.ConditionOpValues().Length)
            assertThrows(Of ArgumentException)(Sub() ConditionOpExtensions.ConditionOpValueOf("greaterOrEqual"))
            assertThrows(Of ArgumentException)(Sub() ConditionOpExtensions.ConditionOpValueOf(""))
        End Sub

        <TestMethod>
        Public Sub testJavaNumbersParsing()
            'Integer.parseInt: optional sign, decimal digits, no whitespace, no overflow
            Dim i As Integer
            assertTrue(JavaNumbers.tryParseInt("12", i))
            assertEquals(12, i)
            assertTrue(JavaNumbers.tryParseInt("+5", i))
            assertEquals(5, i)
            assertTrue(JavaNumbers.tryParseInt("-2147483648", i))
            assertFalse(JavaNumbers.tryParseInt(" 12", i))
            assertFalse(JavaNumbers.tryParseInt("1.0", i))
            assertFalse(JavaNumbers.tryParseInt("2147483648", i))
            assertFalse(JavaNumbers.tryParseInt("", i))
            assertFalse(JavaNumbers.tryParseInt(Nothing, i))
            assertEquals(7, JavaNumbers.parseInt("7"))
            assertThrows(Of NumberFormatException)(Sub() JavaNumbers.parseInt("x"))
            'Long.parseLong
            Dim l As Long
            assertTrue(JavaNumbers.tryParseLong("9223372036854775807", l))
            assertEquals(Long.MaxValue, l)
            assertFalse(JavaNumbers.tryParseLong("9223372036854775808", l))
            assertFalse(JavaNumbers.tryParseLong("1e3", l))
            assertEquals(-3L, JavaNumbers.parseLong("-3"))
            assertThrows(Of NumberFormatException)(Sub() JavaNumbers.parseLong("1.5"))
            'Double.parseDouble: surrounding whitespace, NaN/Infinity and the d/f suffixes are accepted
            Dim d As Double
            assertTrue(JavaNumbers.tryParseDouble(" 1.5 ", d))
            assertEquals(1.5, d, 0.0)
            assertTrue(JavaNumbers.tryParseDouble("1.5d", d))
            assertEquals(1.5, d, 0.0)
            assertTrue(JavaNumbers.tryParseDouble("2F", d))
            assertEquals(2.0, d, 0.0)
            assertTrue(JavaNumbers.tryParseDouble("1e3", d))
            assertEquals(1000.0, d, 0.0)
            assertTrue(JavaNumbers.tryParseDouble("NaN", d))
            assertTrue(Double.IsNaN(d))
            assertTrue(JavaNumbers.tryParseDouble("Infinity", d))
            assertTrue(Double.IsPositiveInfinity(d))
            assertTrue(JavaNumbers.tryParseDouble("+Infinity", d))
            assertTrue(JavaNumbers.tryParseDouble("-Infinity", d))
            assertTrue(Double.IsNegativeInfinity(d))
            assertFalse(JavaNumbers.tryParseDouble("", d))
            assertFalse(JavaNumbers.tryParseDouble("   ", d))
            assertFalse(JavaNumbers.tryParseDouble("abc", d))
            assertFalse(JavaNumbers.tryParseDouble("1,5", d))
            assertFalse(JavaNumbers.tryParseDouble(Nothing, d))
            assertEquals(0.25, JavaNumbers.parseDouble("0.25"), 0.0)
            assertThrows(Of NumberFormatException)(Sub() JavaNumbers.parseDouble("abc"))
            'Float.parseFloat goes through the double parser
            Dim f As Single
            assertTrue(JavaNumbers.tryParseFloat("0.5f", f))
            assertEquals(0.5F, f, 0.0F)
            assertFalse(JavaNumbers.tryParseFloat("x", f))
            assertEquals(2.5F, JavaNumbers.parseFloat("2.5"), 0.0F)
            assertThrows(Of NumberFormatException)(Sub() JavaNumbers.parseFloat("x"))
        End Sub

        <TestMethod>
        Public Sub testJavaNumbersConversions()
            assertTrue(JavaNumbers.isNumber(1))
            assertTrue(JavaNumbers.isNumber(1L))
            assertTrue(JavaNumbers.isNumber(1.5))
            assertTrue(JavaNumbers.isNumber(1.5F))
            assertTrue(JavaNumbers.isNumber(CShort(1)))
            assertTrue(JavaNumbers.isNumber(CSByte(1)))
            assertFalse(JavaNumbers.isNumber("1"))
            assertFalse(JavaNumbers.isNumber(Nothing))
            assertFalse(JavaNumbers.isNumber(True))
            'Number.doubleValue / floatValue
            assertEquals(3.0, JavaNumbers.doubleValue(3), 0.0)
            assertEquals(3.0, JavaNumbers.doubleValue(3L), 0.0)
            assertEquals(2.5, JavaNumbers.doubleValue(2.5F), 0.0)
            assertEquals(2.5F, JavaNumbers.floatValue(2.5), 0.0F)
            assertThrows(Of InvalidCastException)(Sub() JavaNumbers.doubleValue("3"))
            assertThrows(Of NullReferenceException)(Sub() JavaNumbers.doubleValue(Nothing))
            'intValue / longValue truncate towards zero, saturate on overflow of floating point values and wrap for longs
            assertEquals(3, JavaNumbers.intValue(3.9))
            assertEquals(-3, JavaNumbers.intValue(-3.9))
            assertEquals(0, JavaNumbers.intValue(Double.NaN))
            assertEquals(Integer.MaxValue, JavaNumbers.intValue(1.0E+20))
            assertEquals(Integer.MinValue, JavaNumbers.intValue(Double.NegativeInfinity))
            assertEquals(1, JavaNumbers.intValue(4294967297L))
            assertEquals(-1, JavaNumbers.intValue(4294967295L))
            assertEquals(7, JavaNumbers.intValue(CShort(7)))
            assertEquals(1L, JavaNumbers.longValue(1.5))
            assertEquals(-1L, JavaNumbers.longValue(-1.5F))
            assertEquals(0L, JavaNumbers.longValue(Double.NaN))
            assertEquals(Long.MaxValue, JavaNumbers.longValue(1.0E+30))
            assertEquals(Long.MinValue, JavaNumbers.longValue(-1.0E+30))
            assertEquals(7L, JavaNumbers.longValue(7))
            assertEquals(CDec(7), CDec(JavaNumbers.longValue(CDec(7.9))))
        End Sub

        <TestMethod>
        Public Sub testJavaStringsHelpers()
            'String.matches: the whole string must match
            assertTrue(JavaStrings.matches("abc", "abc"))
            assertTrue(JavaStrings.matches("abc", "a.c"))
            assertTrue(JavaStrings.matches("abc123", "[a-z]+\d+"))
            assertFalse(JavaStrings.matches("xabcx", "abc"))
            assertFalse(JavaStrings.matches("abc", "ab"))
            assertFalse(JavaStrings.matches("abc" & vbLf, "abc"))
            assertTrue(JavaStrings.matches("", ""))
            'StringBuilder.reverse
            assertEquals("cba", JavaStrings.reverse("abc"))
            assertEquals("", JavaStrings.reverse(""))
            assertEquals("a", JavaStrings.reverse("a"))
        End Sub

    End Class

End Namespace
