Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.metadata
Imports DeepLearning4VB.org.datavec.api.writable
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone

Namespace org.datavec.api.transform.metadata

    ''' <summary>
    ''' Every <see cref="ColumnMetaData"/> implementation: validity of writables and of plain objects, the
    ''' column type, the accessors, <c>clone</c>, equality and the Lombok-style <c>ToString</c> layouts.
    ''' </summary>
    <TestClass>
    Public Class ColumnMetaDataTest

        ' ---- IntegerMetaData ----

        <TestMethod>
        Public Sub testIntegerMetaDataValidity()
            Dim meta As New IntegerMetaData("intCol")
            assertEquals("intCol", meta.getName())
            assertEquals("intCol", meta.Name)
            assertEquals(ColumnType.[Integer], meta.ColumnType)
            assertEquals(ColumnType.[Integer], meta.getColumnType())
            assertNull(meta.getMinAllowedValue())
            assertNull(meta.getMaxAllowedValue())
            'values are validated through their text form (Integer.parseInt)
            assertTrue(meta.isValid(New IntWritable(5)))
            assertTrue(meta.isValid(New IntWritable(Integer.MinValue)))
            assertTrue(meta.isValid(New LongWritable(Integer.MaxValue)))
            assertTrue(meta.isValid(New Text("-17")))
            assertTrue(meta.isValid(New Text("+3")))
            assertFalse(meta.isValid(New Text("1.0")))
            assertFalse(meta.isValid(New Text("abc")))
            assertFalse(meta.isValid(New Text("")))
            assertFalse(meta.isValid(New Text(" 1")))
            assertFalse(meta.isValid(New Text("NaN")))
            assertFalse(meta.isValid(New DoubleWritable(1.0))) '"1.0"
            assertFalse(meta.isValid(New LongWritable(1L + Integer.MaxValue)))
            assertFalse(meta.isValid(NullWritable.INSTANCE))
            assertFalse(meta.isValid(New BooleanWritable(True)))
        End Sub

        <TestMethod>
        Public Sub testIntegerMetaDataBoundsAndObjects()
            Dim meta As New IntegerMetaData("i", 0, 10)
            assertEquals(0, meta.getMinAllowedValue().Value)
            assertEquals(10, meta.getMaxAllowedValue().Value)
            'bounds are inclusive
            assertTrue(meta.isValid(New IntWritable(0)))
            assertTrue(meta.isValid(New IntWritable(10)))
            assertFalse(meta.isValid(New IntWritable(-1)))
            assertFalse(meta.isValid(New IntWritable(11)))
            Dim minOnly As New IntegerMetaData("i", 5, Nothing)
            assertTrue(minOnly.isValid(New IntWritable(Integer.MaxValue)))
            assertFalse(minOnly.isValid(New IntWritable(4)))
            Dim maxOnly As New IntegerMetaData("i", Nothing, 5)
            assertTrue(maxOnly.isValid(New IntWritable(Integer.MinValue)))
            assertFalse(maxOnly.isValid(New IntWritable(6)))
            'plain objects are validated through their string form as well
            assertTrue(meta.isValid(CObj(3)))
            assertTrue(meta.isValid("3"))
            assertTrue(meta.isValid(CObj(10L)))
            assertFalse(meta.isValid(CObj(11)))
            assertFalse(meta.isValid(CObj(2.5)))
            assertFalse(meta.isValid("x"))
        End Sub

        <TestMethod>
        Public Sub testIntegerMetaDataEqualityCloneAndToString()
            Dim a As New IntegerMetaData("i", 0, 10)
            Dim b As New IntegerMetaData("i", 0, 10)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertTrue(a.Equals(a))
            assertNotEquals(a, New IntegerMetaData("i", 0, 11))
            assertNotEquals(a, New IntegerMetaData("i", 1, 10))
            assertNotEquals(a, New IntegerMetaData("j", 0, 10))
            assertNotEquals(a, New IntegerMetaData("i"))
            assertFalse(a.Equals(Nothing))
            assertFalse(a.Equals("i"))
            'a clone is a distinct but equal object
            Dim c As ColumnMetaData = a.clone()
            assertTrue(TypeOf c Is IntegerMetaData)
            assertFalse(c Is a)
            assertEquals(a, c)
            assertEquals(0, DirectCast(c, IntegerMetaData).getMinAllowedValue().Value)
            'the name can be changed and takes part in equality
            c.setName("k")
            assertEquals("k", c.getName())
            assertNotEquals(a, c)
            'Lombok-style toString, with the fields that are set
            assertEquals("IntegerMetaData(name=""i"",minAllowed=0,maxAllowed=10)", a.ToString())
            assertEquals("IntegerMetaData(name=""i"",)", New IntegerMetaData("i").ToString())
            assertEquals("IntegerMetaData(name=""i"",minAllowed=-5)", New IntegerMetaData("i", -5, Nothing).ToString())
            assertEquals("IntegerMetaData(name=""i"",maxAllowed=7)", New IntegerMetaData("i", Nothing, 7).ToString())
        End Sub

        ' ---- LongMetaData ----

        <TestMethod>
        Public Sub testLongMetaData()
            Dim meta As New LongMetaData("l", -100L, Nothing)
            assertEquals(ColumnType.[Long], meta.getColumnType())
            assertEquals(-100L, meta.getMinAllowedValue().Value)
            assertNull(meta.getMaxAllowedValue())
            'int and long writables are read directly, everything else is parsed (Long.parseLong)
            assertTrue(meta.isValid(New LongWritable(-100)))
            assertFalse(meta.isValid(New LongWritable(-101)))
            assertTrue(meta.isValid(New IntWritable(5)))
            assertTrue(meta.isValid(New LongWritable(Long.MaxValue)))
            assertTrue(meta.isValid(New Text("123456789012")))
            assertFalse(meta.isValid(New Text("1.5")))
            assertFalse(meta.isValid(New Text("abc")))
            assertFalse(meta.isValid(New DoubleWritable(2.0))) '"2.0"
            assertFalse(meta.isValid(New FloatWritable(2.0F)))
            assertFalse(meta.isValid(NullWritable.INSTANCE))
            Dim bounded As New LongMetaData("l", 1L, 3L)
            assertTrue(bounded.isValid(New LongWritable(1)))
            assertTrue(bounded.isValid(New LongWritable(3)))
            assertFalse(bounded.isValid(New LongWritable(4)))
            assertFalse(bounded.isValid(New IntWritable(0)))
            'objects
            assertTrue(meta.isValid(CObj(5L)))
            assertTrue(meta.isValid("7"))
            assertFalse(meta.isValid(CObj(-200L)))
            assertFalse(meta.isValid("x"))
            assertFalse(meta.isValid(CObj(1.5)))
            'equality, clone and toString
            assertEquals(meta, New LongMetaData("l", -100L, Nothing))
            assertEquals(meta.GetHashCode(), New LongMetaData("l", -100L, Nothing).GetHashCode())
            assertNotEquals(meta, New LongMetaData("l"))
            assertNotEquals(meta, New LongMetaData("m", -100L, Nothing))
            Dim c As ColumnMetaData = meta.clone()
            assertTrue(TypeOf c Is LongMetaData)
            assertFalse(c Is meta)
            assertEquals(meta, c)
            assertEquals("LongMetaData(name=""l"",minAllowed=-100)", meta.ToString())
            assertEquals("LongMetaData(name=""l"",)", New LongMetaData("l").ToString())
            assertEquals("LongMetaData(name=""l"",minAllowed=1,maxAllowed=3)", bounded.ToString())
            assertEquals("LongMetaData(name=""l"",maxAllowed=3)", New LongMetaData("l", Nothing, 3L).ToString())
        End Sub

        ' ---- DoubleMetaData ----

        <TestMethod>
        Public Sub testDoubleMetaDataValidity()
            Dim meta As New DoubleMetaData("d", -1.0, 1.0)
            assertEquals(ColumnType.[Double], meta.getColumnType())
            assertEquals(-1.0, meta.getMinAllowedValue().Value, 0.0)
            assertEquals(1.0, meta.getMaxAllowedValue().Value, 0.0)
            assertFalse(meta.isAllowNaN())
            assertFalse(meta.isAllowInfinite())
            assertTrue(meta.isValid(New DoubleWritable(0.5)))
            assertTrue(meta.isValid(New DoubleWritable(1.0)))
            assertTrue(meta.isValid(New DoubleWritable(-1.0)))
            assertFalse(meta.isValid(New DoubleWritable(1.01)))
            assertFalse(meta.isValid(New DoubleWritable(-1.5)))
            assertTrue(meta.isValid(New IntWritable(0)))
            assertTrue(meta.isValid(New FloatWritable(0.25F)))
            assertTrue(meta.isValid(New Text("0.25")))
            assertTrue(meta.isValid(New Text("1e-1")))
            'a writable that cannot be converted to a double is invalid
            assertFalse(meta.isValid(New Text("abc")))
            assertFalse(meta.isValid(NullWritable.INSTANCE))
            'objects are parsed like Double.parseDouble(input.toString())
            assertTrue(meta.isValid("0.5"))
            assertTrue(meta.isValid(CObj(0.5)))
            assertTrue(meta.isValid(CObj(1)))
            assertFalse(meta.isValid(CObj(2.0)))
            assertFalse(meta.isValid("x"))
            assertFalse(meta.isValid(Nothing))
            'no restriction at all
            Dim free As New DoubleMetaData("d")
            assertNull(free.getMinAllowedValue())
            assertNull(free.getMaxAllowedValue())
            assertTrue(free.isValid(New DoubleWritable(Double.MaxValue)))
            assertTrue(free.isValid(New DoubleWritable(-Double.MaxValue)))
        End Sub

        <TestMethod>
        Public Sub testDoubleMetaDataNaNAndInfinity()
            'Java semantics: allowNaN/allowInfinite grant validity before the bounds are checked; without them a NaN
            'or infinite value is only rejected by a bound it violates (NaN never violates a bound)
            Dim strict As New DoubleMetaData("d")
            assertTrue(strict.isValid(New DoubleWritable(Double.NaN)))
            assertTrue(strict.isValid(New DoubleWritable(Double.PositiveInfinity)))
            assertTrue(strict.isValid(New DoubleWritable(Double.NegativeInfinity)))
            Dim bounded As New DoubleMetaData("d", 0.0, 100.0)
            assertFalse(bounded.isValid(New DoubleWritable(Double.PositiveInfinity)))
            assertFalse(bounded.isValid(New DoubleWritable(Double.NegativeInfinity)))
            assertTrue(bounded.isValid(New DoubleWritable(Double.NaN)))
            Dim lenient As New DoubleMetaData("d", 0.0, 100.0, True, True)
            assertTrue(lenient.isAllowNaN())
            assertTrue(lenient.isAllowInfinite())
            assertTrue(lenient.isValid(New DoubleWritable(Double.PositiveInfinity)))
            assertTrue(lenient.isValid(New DoubleWritable(Double.NegativeInfinity)))
            assertTrue(lenient.isValid(New DoubleWritable(Double.NaN)))
            assertFalse(lenient.isValid(New DoubleWritable(101.0)))
            Dim nanOnly As New DoubleMetaData("d", 0.0, 100.0, True, False)
            assertTrue(nanOnly.isValid(New DoubleWritable(Double.NaN)))
            assertFalse(nanOnly.isValid(New DoubleWritable(Double.NegativeInfinity)))
            Dim infOnly As New DoubleMetaData("d", 0.0, 100.0, False, True)
            assertTrue(infOnly.isValid(New DoubleWritable(Double.NegativeInfinity)))
            'the object form parses "NaN"/"Infinity" like Double.parseDouble
            assertTrue(lenient.isValid("NaN"))
            assertTrue(lenient.isValid("Infinity"))
            assertFalse(bounded.isValid("-Infinity"))
        End Sub

        <TestMethod>
        Public Sub testDoubleMetaDataEqualityCloneAndToString()
            Dim a As New DoubleMetaData("d", -1.0, 1.0, True, False)
            assertEquals(a, New DoubleMetaData("d", -1.0, 1.0, True, False))
            assertEquals(a.GetHashCode(), New DoubleMetaData("d", -1.0, 1.0, True, False).GetHashCode())
            assertNotEquals(a, New DoubleMetaData("d", -1.0, 1.0, False, False))
            assertNotEquals(a, New DoubleMetaData("d", -1.0, 1.0, True, True))
            assertNotEquals(a, New DoubleMetaData("d", -1.0, 2.0, True, False))
            assertNotEquals(a, New DoubleMetaData("e", -1.0, 1.0, True, False))
            assertNotEquals(a, New DoubleMetaData("d"))
            Dim c As ColumnMetaData = a.clone()
            assertTrue(TypeOf c Is DoubleMetaData)
            assertFalse(c Is a)
            assertEquals(a, c)
            'the two-argument constructor disallows NaN and infinite values
            assertEquals(New DoubleMetaData("d", -1.0, 1.0, False, False), New DoubleMetaData("d", -1.0, 1.0))
            assertEquals("DoubleMetaData(name=""d"",minAllowed=-1.0,maxAllowed=1.0,allowNaN=true,allowInfinite=false)", a.ToString())
            assertEquals("DoubleMetaData(name=""d"",allowNaN=false,allowInfinite=false)", New DoubleMetaData("d").ToString())
            assertEquals("DoubleMetaData(name=""d"",maxAllowed=100.0,allowNaN=true,allowInfinite=false)", New DoubleMetaData("d", Nothing, 100.0, True, False).ToString())
            assertEquals("DoubleMetaData(name=""d"",minAllowed=0.5,allowNaN=false,allowInfinite=true)", New DoubleMetaData("d", 0.5, Nothing, False, True).ToString())
        End Sub

        ' ---- FloatMetaData ----

        <TestMethod>
        Public Sub testFloatMetaData()
            Dim meta As New FloatMetaData("f", -1.0F, 1.0F)
            assertEquals(ColumnType.Float, meta.getColumnType())
            assertEquals(-1.0F, meta.getMinAllowedValue().Value, 0.0F)
            assertEquals(1.0F, meta.getMaxAllowedValue().Value, 0.0F)
            assertFalse(meta.isAllowNaN())
            assertFalse(meta.isAllowInfinite())
            assertTrue(meta.isValid(New FloatWritable(0.5F)))
            assertTrue(meta.isValid(New FloatWritable(1.0F)))
            assertFalse(meta.isValid(New FloatWritable(1.5F)))
            assertFalse(meta.isValid(New FloatWritable(-1.5F)))
            assertTrue(meta.isValid(New DoubleWritable(0.25)))
            assertTrue(meta.isValid(New IntWritable(1)))
            assertTrue(meta.isValid(New Text("0.25")))
            assertFalse(meta.isValid(New Text("abc")))
            assertFalse(meta.isValid(NullWritable.INSTANCE))
            'NaN / infinity work like the double metadata
            assertTrue(New FloatMetaData("f").isValid(New FloatWritable(Single.NaN)))
            assertFalse(meta.isValid(New FloatWritable(Single.PositiveInfinity)))
            assertTrue(meta.isValid(New FloatWritable(Single.NaN)))
            assertTrue(New FloatMetaData("f", -1.0F, 1.0F, False, True).isValid(New FloatWritable(Single.NegativeInfinity)))
            'Java casts the object to Float, so only boxed singles can be valid
            assertTrue(meta.isValid(CObj(0.5F)))
            assertFalse(meta.isValid(CObj(2.0F)))
            assertFalse(meta.isValid(CObj(0.5)))
            assertFalse(meta.isValid("0.5"))
            assertFalse(meta.isValid(CObj(1)))
            assertFalse(meta.isValid(Nothing))
            'equality, clone and toString
            assertEquals(meta, New FloatMetaData("f", -1.0F, 1.0F, False, False))
            assertEquals(meta.GetHashCode(), New FloatMetaData("f", -1.0F, 1.0F).GetHashCode())
            assertNotEquals(meta, New FloatMetaData("f", -1.0F, 1.0F, True, False))
            assertNotEquals(meta, New FloatMetaData("f"))
            assertNotEquals(meta, New FloatMetaData("g", -1.0F, 1.0F))
            Dim c As ColumnMetaData = meta.clone()
            assertTrue(TypeOf c Is FloatMetaData)
            assertFalse(c Is meta)
            assertEquals(meta, c)
            assertEquals("FloatMetaData(name=""f"",minAllowed=-1.0,maxAllowed=1.0,allowNaN=false,allowInfinite=false)", meta.ToString())
            assertEquals("FloatMetaData(name=""f"",allowNaN=false,allowInfinite=false)", New FloatMetaData("f").ToString())
            assertEquals("FloatMetaData(name=""f"",maxAllowed=0.5,allowNaN=true,allowInfinite=true)", New FloatMetaData("f", Nothing, 0.5F, True, True).ToString())
        End Sub

        ' ---- StringMetaData ----

        <TestMethod>
        Public Sub testStringMetaDataValidity()
            Dim free As New StringMetaData("s")
            assertEquals(ColumnType.[String], free.getColumnType())
            assertNull(free.getRegex())
            assertNull(free.getMinLength())
            assertNull(free.getMaxLength())
            assertTrue(free.isValid(New Text("")))
            assertTrue(free.isValid(New Text("anything at all")))
            assertTrue(free.isValid(New IntWritable(3)))
            assertTrue(free.isValid(NullWritable.INSTANCE))
            'length restrictions are inclusive, the regex must match the whole string
            Dim meta As New StringMetaData("s", "[a-z]+", 2, 4)
            assertEquals("[a-z]+", meta.getRegex())
            assertEquals(2, meta.getMinLength().Value)
            assertEquals(4, meta.getMaxLength().Value)
            assertTrue(meta.isValid(New Text("ab")))
            assertTrue(meta.isValid(New Text("abcd")))
            assertFalse(meta.isValid(New Text("a")))
            assertFalse(meta.isValid(New Text("abcde")))
            assertFalse(meta.isValid(New Text("AB")))
            assertFalse(meta.isValid(New Text("ab1")))
            assertFalse(meta.isValid(New Text("")))
            Dim regexOnly As New StringMetaData("s", "\d+", Nothing, Nothing)
            assertTrue(regexOnly.isValid(New Text("123")))
            assertTrue(regexOnly.isValid(New IntWritable(42)))
            assertFalse(regexOnly.isValid(New Text("12a")))
            assertFalse(regexOnly.isValid(New Text("x123x")))
            assertFalse(regexOnly.isValid(New Text("")))
            Dim lengthOnly As New StringMetaData("s", Nothing, Nothing, 3)
            assertTrue(lengthOnly.isValid(New Text("abc")))
            assertFalse(lengthOnly.isValid(New Text("abcd")))
            'objects are validated through their string form
            assertTrue(meta.isValid("abc"))
            assertFalse(meta.isValid("abcdef"))
            assertTrue(regexOnly.isValid(CObj(12)))
            assertFalse(regexOnly.isValid(CObj(1.5)))
        End Sub

        <TestMethod>
        Public Sub testStringMetaDataEqualityCloneAndToString()
            Dim a As New StringMetaData("s", "[a-z]+", 2, 4)
            assertEquals(a, New StringMetaData("s", "[a-z]+", 2, 4))
            assertEquals(a.GetHashCode(), New StringMetaData("s", "[a-z]+", 2, 4).GetHashCode())
            assertNotEquals(a, New StringMetaData("s", "[a-z]*", 2, 4))
            assertNotEquals(a, New StringMetaData("s", "[a-z]+", 1, 4))
            assertNotEquals(a, New StringMetaData("s", "[a-z]+", 2, Nothing))
            assertNotEquals(a, New StringMetaData("t", "[a-z]+", 2, 4))
            assertNotEquals(a, New StringMetaData("s"))
            Dim c As ColumnMetaData = a.clone()
            assertTrue(TypeOf c Is StringMetaData)
            assertFalse(c Is a)
            assertEquals(a, c)
            'the parameterless constructor (for Jackson) has no name and no restrictions
            Dim empty As New StringMetaData()
            assertNull(empty.getName())
            assertNull(empty.getRegex())
            assertTrue(empty.isValid(New Text("x")))
            assertEquals("StringMetaData(name=""s"",minLengthAllowed=2,maxLengthAllowed=4,regex=[a-z]+)", a.ToString())
            assertEquals("StringMetaData(name=""s"",)", New StringMetaData("s").ToString())
            assertEquals("StringMetaData(name=""s"",regex=\d+)", New StringMetaData("s", "\d+", Nothing, Nothing).ToString())
            assertEquals("StringMetaData(name=""s"",maxLengthAllowed=3)", New StringMetaData("s", Nothing, Nothing, 3).ToString())
            assertEquals("StringMetaData(name=""s"",minLengthAllowed=1,regex=x)", New StringMetaData("s", "x", 1, Nothing).ToString())
        End Sub

        ' ---- CategoricalMetaData ----

        <TestMethod>
        Public Sub testCategoricalMetaData()
            Dim meta As New CategoricalMetaData("c", "alpha", "beta")
            assertEquals(ColumnType.Categorical, meta.getColumnType())
            assertEquals(Arrays.asList("alpha", "beta"), meta.getStateNames())
            assertEquals(Arrays.asList("alpha", "beta"), meta.StateNames)
            assertEquals(New HashSet(Of String)(New String() {"alpha", "beta"}), meta.getStateNamesSet())
            assertTrue(meta.isValid(New Text("alpha")))
            assertTrue(meta.isValid(New Text("beta")))
            assertFalse(meta.isValid(New Text("gamma")))
            assertFalse(meta.isValid(New Text("Alpha")))
            assertFalse(meta.isValid(New Text("")))
            assertTrue(meta.isValid("beta"))
            assertFalse(meta.isValid("delta"))
            'any writable whose text is a state name is valid
            Dim numeric As New CategoricalMetaData("n", Arrays.asList("1", "2"))
            assertTrue(numeric.isValid(New IntWritable(1)))
            assertTrue(numeric.isValid(New LongWritable(2)))
            assertFalse(numeric.isValid(New IntWritable(3)))
            assertTrue(numeric.isValid(CObj(2)))
            'equality compares the name and the states in order
            assertEquals(meta, New CategoricalMetaData("c", Arrays.asList("alpha", "beta")))
            assertEquals(meta.GetHashCode(), New CategoricalMetaData("c", "alpha", "beta").GetHashCode())
            assertNotEquals(meta, New CategoricalMetaData("c", "beta", "alpha"))
            assertNotEquals(meta, New CategoricalMetaData("c", "alpha"))
            assertNotEquals(meta, New CategoricalMetaData("d", "alpha", "beta"))
            Dim c As ColumnMetaData = meta.clone()
            assertTrue(TypeOf c Is CategoricalMetaData)
            assertFalse(c Is meta)
            assertEquals(meta, c)
            Dim text As String = meta.ToString()
            assertTrue(text.StartsWith("CategoricalMetaData(name=""c"",stateNames=[", StringComparison.Ordinal), text)
            assertTrue(text.EndsWith("])", StringComparison.Ordinal), text)
            assertTrue(text.Contains("""alpha"""), text)
            assertTrue(text.Contains("""beta"""), text)
            assertTrue(text.Contains(""","""), text)
            assertEquals("CategoricalMetaData(name=""one"",stateNames=[""x""])", New CategoricalMetaData("one", "x").ToString())
        End Sub

        ' ---- TimeMetaData ----

        <TestMethod>
        Public Sub testTimeMetaData()
            Dim meta As New TimeMetaData("t")
            assertEquals(ColumnType.Time, meta.getColumnType())
            assertEquals(DateTimeZone.UTC, meta.getTimeZone())
            assertEquals("UTC", meta.TimeZone.getID())
            assertNull(meta.getMinValidTime())
            assertNull(meta.getMaxValidTime())
            'long writables are read directly, everything else is parsed as a long
            assertTrue(meta.isValid(New LongWritable(0)))
            assertTrue(meta.isValid(New LongWritable(-1)))
            assertTrue(meta.isValid(New IntWritable(5)))
            assertTrue(meta.isValid(New Text("1451606400000")))
            assertFalse(meta.isValid(New Text("abc")))
            assertFalse(meta.isValid(New Text("1.0")))
            assertFalse(meta.isValid(New DoubleWritable(1.0)))
            assertFalse(meta.isValid(NullWritable.INSTANCE))
            assertTrue(meta.isValid(CObj(5L)))
            assertTrue(meta.isValid("12"))
            assertFalse(meta.isValid("x"))
            assertFalse(meta.isValid(CObj(1.5)))
            'restrictions in epoch milliseconds (inclusive)
            Dim bounded As New TimeMetaData("t", DateTimeZone.UTC, 10L, 20L)
            assertEquals(10L, bounded.getMinValidTime().Value)
            assertEquals(20L, bounded.getMaxValidTime().Value)
            assertTrue(bounded.isValid(New LongWritable(10)))
            assertTrue(bounded.isValid(New LongWritable(20)))
            assertFalse(bounded.isValid(New LongWritable(9)))
            assertFalse(bounded.isValid(New LongWritable(21)))
            assertFalse(bounded.isValid(CObj(21L)))
            assertTrue(bounded.isValid("15"))
            'java.util.TimeZone constructors map to the Joda zone
            assertEquals(meta, New TimeMetaData("t", TimeZoneInfo.Utc))
            assertEquals(meta, New TimeMetaData("t", DateTimeZone.UTC))
            assertEquals(bounded, New TimeMetaData("t", TimeZoneInfo.Utc, 10L, 20L))
            assertEquals(bounded.GetHashCode(), New TimeMetaData("t", TimeZoneInfo.Utc, 10L, 20L).GetHashCode())
            'the zone and the bounds take part in equality
            assertNotEquals(meta, New TimeMetaData("t", DateTimeZone.forOffsetHours(1)))
            assertNotEquals(meta, bounded)
            assertNotEquals(bounded, New TimeMetaData("t", DateTimeZone.UTC, 10L, 21L))
            assertNotEquals(meta, New TimeMetaData("u"))
            Dim c As ColumnMetaData = bounded.clone()
            assertTrue(TypeOf c Is TimeMetaData)
            assertFalse(c Is bounded)
            assertEquals(bounded, c)
            assertEquals("TimeMetaData(name=""t"",timeZone=UTC)", meta.ToString())
            'Java quirk: no separator between the zone and the first bound
            assertEquals("TimeMetaData(name=""t"",timeZone=UTCminValidTime=10,maxValidTime=20)", bounded.ToString())
            assertEquals("TimeMetaData(name=""t"",timeZone=UTCmaxValidTime=20)", New TimeMetaData("t", DateTimeZone.UTC, Nothing, 20L).ToString())
            assertEquals("TimeMetaData(name=""t"",timeZone=UTCminValidTime=10)", New TimeMetaData("t", DateTimeZone.UTC, 10L, Nothing).ToString())
        End Sub

        ' ---- BooleanMetaData and BinaryMetaData ----

        <TestMethod>
        Public Sub testBooleanAndBinaryMetaData()
            Dim b As New BooleanMetaData("b")
            assertEquals(ColumnType.[Boolean], b.getColumnType())
            assertEquals("b", b.getName())
            'Boolean.parseBoolean never fails, so every value is valid
            assertTrue(b.isValid(New BooleanWritable(True)))
            assertTrue(b.isValid(New BooleanWritable(False)))
            assertTrue(b.isValid(New Text("abc")))
            assertTrue(b.isValid(New IntWritable(7)))
            assertTrue(b.isValid(NullWritable.INSTANCE))
            assertTrue(b.isValid("x"))
            assertTrue(b.isValid(Nothing))
            assertEquals(b, New BooleanMetaData("b"))
            assertEquals(b.GetHashCode(), New BooleanMetaData("b").GetHashCode())
            assertNotEquals(b, New BooleanMetaData("c"))
            Dim bc As ColumnMetaData = b.clone()
            assertTrue(TypeOf bc Is BooleanMetaData)
            assertFalse(bc Is b)
            assertEquals(b, bc)
            assertEquals("BooleanMetaData(name=""b"",)", b.ToString())

            Dim bin As New BinaryMetaData("raw")
            assertEquals(ColumnType.Bytes, bin.getColumnType())
            assertTrue(bin.isValid(New Text("anything")))
            assertTrue(bin.isValid(New BytesWritable(New Byte() {1, 2, 3})))
            assertTrue(bin.isValid(NullWritable.INSTANCE))
            assertTrue(bin.isValid(Nothing))
            assertEquals(bin, New BinaryMetaData("raw"))
            assertNotEquals(bin, New BinaryMetaData("other"))
            Dim binc As ColumnMetaData = bin.clone()
            assertTrue(TypeOf binc Is BinaryMetaData)
            assertEquals(bin, binc)
            assertEquals("BinaryMetaData(name=""raw"",)", bin.ToString())
            'different metadata classes are never equal, even with the same name
            assertNotEquals(b, New BinaryMetaData("b"))
        End Sub

        ' ---- BaseColumnMetaData ----

        <TestMethod>
        Public Sub testBaseMetaDataNameAndCrossTypeEquality()
            Dim i As New IntegerMetaData("a")
            Dim l As New LongMetaData("a")
            Dim d As New DoubleMetaData("a")
            Dim f As New FloatMetaData("a")
            Dim s As New StringMetaData("a")
            Dim t As New TimeMetaData("a")
            Dim all() As ColumnMetaData = {i, l, d, f, s, t, New BooleanMetaData("a"), New BinaryMetaData("a"), New CategoricalMetaData("a", "x")}
            For x As Integer = 0 To all.Length - 1
                For y As Integer = 0 To all.Length - 1
                    If x = y Then
                        assertEquals(all(x), all(y))
                    Else
                        assertNotEquals(all(x), all(y))
                    End If
                Next y
                assertEquals("a", all(x).getName())
                assertEquals(all(x).getColumnType(), all(x).ColumnType)
            Next x
            'the column types are all distinct
            Dim types As New HashSet(Of ColumnType)()
            For Each m As ColumnMetaData In all
                types.Add(m.getColumnType())
            Next m
            assertEquals(9, types.Count)
            'Name is a read/write property mirrored by getName/setName
            i.Name = "renamed"
            assertEquals("renamed", i.getName())
            i.setName("again")
            assertEquals("again", i.Name)
            'a writable-typed null is handled by the parsing implementations
            Dim nullWritable As WritableT = Nothing
            assertThrows(Of NullReferenceException)(Sub() i.isValid(nullWritable))
        End Sub

    End Class

End Namespace
