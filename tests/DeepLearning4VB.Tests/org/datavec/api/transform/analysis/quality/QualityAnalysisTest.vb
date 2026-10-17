Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality.categorical
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality.[integer]
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality.longq
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality.real
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality.[string]
Imports DeepLearning4VB.org.datavec.api.transform.analysis.quality.time
Imports DeepLearning4VB.org.datavec.api.transform.quality.columns
Imports BinaryMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.BinaryMetaData
Imports BytesQualityAnalysisState = DeepLearning4VB.org.datavec.api.transform.analysis.quality.bytes.BytesQualityAnalysisState
Imports CategoricalMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports DataQualityAnalysisT = DeepLearning4VB.org.datavec.api.transform.quality.DataQualityAnalysis
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports QualityAnalysisStateT = DeepLearning4VB.org.datavec.api.transform.analysis.quality.QualityAnalysisState
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SerializationUtils = DeepLearning4VB.org.nd4vb.common.util.SerializationUtils
Imports StringMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.StringMetaData
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports TimeMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.TimeMetaData
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.quality

    ''' <summary>
    ''' Tests of the data-quality functions: the per-type add functions (valid / invalid / missing and the type specific
    ''' counts, with Java's parsing rules), merge functions, analysis states and the record-level add/combine functions.
    ''' </summary>
    <TestClass>
    Public Class QualityAnalysisTest

        ' ---------------- integer ----------------

        <TestMethod>
        Public Sub integerQualityAddFunctionCounts()
            Dim f As New IntegerQualityAddFunction(New IntegerMetaData("i", 0, 10))
            Dim q As New IntegerQuality()
            For Each w As WritableT In New WritableT() {New IntWritable(5), New IntWritable(11), New TextT("abc"), NullWritable.INSTANCE,
                                                       New TextT(""), New TextT("7"), New TextT(" 7"), New TextT("99999999999")}
                q = f.apply(q, w)
            Next w
            assertEquals(2L, q.getCountValid())     ' 5 and "7"
            assertEquals(4L, q.getCountInvalid())   ' 11 (out of range), "abc", " 7" (Java parseInt rejects spaces), int overflow
            assertEquals(2L, q.getCountMissing())   ' NullWritable and the empty text
            assertEquals(8L, q.getCountTotal())
            ' every value whose text is not an int (including "NullWritable" and "")
            assertEquals(5L, q.getCountNonInteger())
        End Sub

        <TestMethod>
        Public Sub integerQualityMergeAndState()
            Dim merged As IntegerQuality = (New IntegerQualityMergeFunction()).apply(New IntegerQuality(1, 2, 3, 6, 4), New IntegerQuality(10, 20, 30, 60, 40))
            assertEquals(New IntegerQuality(11, 22, 33, 66, 44), merged)

            Dim s1 As New IntegerQualityAnalysisState(New IntegerMetaData("i"))
            s1.add(New IntWritable(1)).add(New TextT("x"))
            Dim s2 As New IntegerQualityAnalysisState(New IntegerMetaData("i"))
            s2.add(NullWritable.INSTANCE)
            assertTrue(s1.merge(s2) Is s1)
            assertEquals(New IntegerQuality(1, 1, 1, 3, 2), s1.getIntegerQuality())
            assertTrue(s1.ColumnQuality Is s1.getIntegerQuality())
            ' the other state is unchanged
            assertEquals(New IntegerQuality(0, 0, 1, 1, 1), s2.getIntegerQuality())
        End Sub

        ' ---------------- long ----------------

        <TestMethod>
        Public Sub longQualityAddFunctionAndState()
            Dim f As New LongQualityAddFunction(New LongMetaData("l"))
            Dim q As New LongQuality()
            For Each w As WritableT In New WritableT() {New LongWritable(5), New TextT("12345678901"), New TextT("1.5"), NullWritable.INSTANCE}
                q = f.apply(q, w)
            Next w
            assertEquals(New LongQuality(2, 1, 1, 4, 2), q)

            Dim s As New LongQualityAnalysisState(New LongMetaData("l", 0L, 100L))
            s.add(New LongWritable(50)).add(New LongWritable(500)).add(New TextT(""))
            Dim other As New LongQualityAnalysisState(New LongMetaData("l", 0L, 100L))
            other.add(New LongWritable(-1))
            s.merge(other)
            assertEquals(New LongQuality(1, 2, 1, 4, 1), s.getLongQuality())
        End Sub

        ' ---------------- double ----------------

        <TestMethod>
        Public Sub realQualityAddFunctionCounts()
            Dim f As New RealQualityAddFunction(New DoubleMetaData("d", 0.0, 10.0))
            Dim q As New DoubleQuality()
            For Each w As WritableT In New WritableT() {New DoubleWritable(5.0), New DoubleWritable(Double.NaN), New DoubleWritable(Double.PositiveInfinity),
                                                       New DoubleWritable(11.0), New TextT("abc"), NullWritable.INSTANCE, New TextT("2.5")}
                q = f.apply(q, w)
            Next w
            ' As in Java, NaN passes the min/max checks of DoubleMetaData (all comparisons with NaN are false)
            assertEquals(3L, q.getCountValid())     ' 5.0, NaN, "2.5"
            assertEquals(3L, q.getCountInvalid())   ' Infinity (> max), 11.0, "abc"
            assertEquals(1L, q.getCountMissing())
            assertEquals(7L, q.getCountTotal())
            assertEquals(2L, q.getCountNonReal())   ' "abc", "NullWritable"
            assertEquals(1L, q.getCountNaN())
            assertEquals(1L, q.getCountInfinite())
        End Sub

        <TestMethod>
        Public Sub realQualityState()
            Dim s As New RealQualityAnalysisState(New DoubleMetaData("d"))
            s.add(New DoubleWritable(1.0)).add(New TextT("-Infinity")).add(New TextT(""))
            Dim other As New RealQualityAnalysisState(New DoubleMetaData("d"))
            other.add(New TextT("NaN"))
            s.merge(other)
            Dim q As DoubleQuality = s.getRealQuality()
            assertTrue(s.ColumnQuality Is q)
            assertEquals(New DoubleQuality(3, 0, 1, 4, 1, 1, 1), q)
        End Sub

        ' ---------------- string ----------------

        <TestMethod>
        Public Sub stringQualityAddFunctionUsesJavaCharacterClasses()
            Dim f As New StringQualityAddFunction(New StringMetaData("s", "[a-z]+", Nothing, Nothing))
            Dim q As New StringQuality()
            Dim values() As WritableT = {New TextT("abc"), New TextT("x"), New TextT("123"), New TextT("   "), New TextT(""), NullWritable.INSTANCE,
                                         New TextT(ChrW(&HE9)), New TextT(ChrW(&H663))}
            For Each w As WritableT In values
                q = f.apply(q, w)
            Next w
            assertEquals(2L, q.getCountValid())          ' "abc", "x"
            assertEquals(5L, q.getCountInvalid())        ' "123", "   ", "", e-acute, Arabic-indic three
            assertEquals(1L, q.getCountMissing())        ' NullWritable
            assertEquals(8L, q.getCountTotal())
            assertEquals(1L, q.getCountEmptyString())
            ' Java's pattern "[a-z A-Z]" has no quantifier: only one-letter strings are "alphabetic"
            assertEquals(1L, q.getCountAlphabetic())     ' "x"
            ' Java's \d and \w are ASCII only: the Arabic-indic digit and e-acute do not count
            assertEquals(1L, q.getCountNumerical())      ' "123"
            assertEquals(4L, q.getCountWordCharacter())  ' "abc", "x", "123", "NullWritable"
            assertEquals(1L, q.getCountWhitespace())     ' "   "
            assertEquals(8.0, CDbl(q.getHll().cardinality()), 1.0)
        End Sub

        <TestMethod>
        Public Sub stringQualityStateMergesTheDistinctCounts()
            Dim s1 As New StringQualityAnalysisState(New StringMetaData("s"))
            Dim s2 As New StringQualityAnalysisState(New StringMetaData("s"))
            For i As Integer = 0 To 29
                s1.add(New TextT("v" & i))
            Next i
            For i As Integer = 20 To 49
                s2.add(New TextT("v" & i))
            Next i
            s1.merge(s2)
            Dim q As StringQuality = s1.getStringQuality()
            assertEquals(60L, q.getCountTotal())
            assertEquals(60L, q.getCountValid())
            assertEquals(60L, q.getCountWordCharacter())
            assertEquals(50.0, CDbl(q.getHll().cardinality()), 3.0) ' 50 distinct values
            assertTrue(s1.ColumnQuality Is q)
        End Sub

        ' ---------------- categorical ----------------

        <TestMethod>
        Public Sub categoricalQualityAddFunctionAndState()
            Dim f As New CategoricalQualityAddFunction(New CategoricalMetaData("c", "a", "b"))
            Dim q As New CategoricalQuality()
            For Each w As WritableT In New WritableT() {New TextT("a"), New TextT("b"), New TextT("z"), New TextT(""), NullWritable.INSTANCE, New IntWritable(1)}
                q = f.apply(q, w)
            Next w
            assertEquals(New CategoricalQuality(2, 2, 2, 6), q)

            Dim s As New CategoricalQualityAnalysisState(New CategoricalMetaData("c", "a", "b"))
            s.add(New TextT("a"))
            Dim other As New CategoricalQualityAnalysisState(New CategoricalMetaData("c", "a", "b"))
            other.add(New TextT("c"))
            assertTrue(s.merge(other) Is s)
            assertEquals(New CategoricalQuality(1, 1, 0, 2), s.getCategoricalQuality())
            assertEquals(New CategoricalQuality(3, 4, 5, 12), (New CategoricalQualityMergeFunction()).apply(New CategoricalQuality(1, 2, 3, 6), New CategoricalQuality(2, 2, 2, 6)))
        End Sub

        ' ---------------- time ----------------

        <TestMethod>
        Public Sub timeQualityAddFunctionAndState()
            Dim meta As New TimeMetaData("t", DateTimeZone.UTC, 0L, 1000L)
            Dim f As New TimeQualityAddFunction(meta)
            Dim q As New TimeQuality()
            For Each w As WritableT In New WritableT() {New LongWritable(500), New LongWritable(2000), New TextT("700"), New TextT("abc"), NullWritable.INSTANCE, New TextT("")}
                q = f.apply(q, w)
            Next w
            assertEquals(2L, q.getCountValid())
            assertEquals(2L, q.getCountInvalid())
            assertEquals(2L, q.getCountMissing())
            assertEquals(6L, q.getCountTotal())

            Dim s As New TimeQualityAnalysisState(meta)
            s.add(New LongWritable(0)).add(New LongWritable(1000)).add(New LongWritable(1001))
            s.merge(New TimeQualityAnalysisState(meta).add(New LongWritable(-1)))
            Dim tq As TimeQuality = s.getTimeQuality()
            assertEquals(2L, tq.getCountValid()) ' the limits are inclusive
            assertEquals(2L, tq.getCountInvalid())
            assertEquals(4L, tq.getCountTotal())
            assertEquals(New TimeQuality(5, 7, 9, 21), (New TimeQualityMergeFunction()).apply(New TimeQuality(1, 2, 3, 6), New TimeQuality(4, 5, 6, 15)))
        End Sub

        ' ---------------- bytes ----------------

        <TestMethod>
        Public Sub bytesQualityStateAnalysesNothing()
            Dim s As New BytesQualityAnalysisState()
            assertTrue(s.add(New TextT("x")) Is s)
            assertTrue(s.merge(New BytesQualityAnalysisState()) Is s)
            assertEquals(New BytesQuality(0, 0, 0, 0), s.getBytesQuality())
            assertTrue(s.ColumnQuality Is s.getBytesQuality())
        End Sub

        ' ---------------- record level ----------------

        Private Shared Function schemaOfAllTypes() As SchemaT
            Return (New SchemaT.Builder()).addColumnString("s").addColumnInteger("i", 0, 100).addColumnLong("l").addColumnDouble("d").addColumnCategorical("c", "a", "b").addColumnTime("t", DateTimeZone.UTC).addColumn(New BinaryMetaData("b")).build()
        End Function

        Private Shared Function record(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub qualityAnalysisAddFunctionCreatesOneStatePerColumn()
            Dim f As New QualityAnalysisAddFunction(schemaOfAllTypes())
            Dim states As IList(Of QualityAnalysisStateT) = f.apply(Nothing, record(New TextT("x"), New IntWritable(5), New LongWritable(1), New DoubleWritable(0.5), New TextT("a"), New LongWritable(10), New TextT("b")))
            assertEquals(7, states.Count)
            assertTrue(TypeOf states(0) Is StringQualityAnalysisState)
            assertTrue(TypeOf states(1) Is IntegerQualityAnalysisState)
            assertTrue(TypeOf states(2) Is LongQualityAnalysisState)
            assertTrue(TypeOf states(3) Is RealQualityAnalysisState)
            assertTrue(TypeOf states(4) Is CategoricalQualityAnalysisState)
            assertTrue(TypeOf states(5) Is TimeQualityAnalysisState)
            assertTrue(TypeOf states(6) Is BytesQualityAnalysisState)

            Dim same As IList(Of QualityAnalysisStateT) = f.apply(states, record(NullWritable.INSTANCE, New IntWritable(500), New TextT("x"), New TextT("NaN"), New TextT("q"), New TextT(""), New TextT("c")))
            assertTrue(same Is states)
            assertEquals(1L, states(0).ColumnQuality.getCountMissing())
            assertEquals(1L, states(1).ColumnQuality.getCountInvalid()) ' 500 is above the maximum
            assertEquals(New LongQuality(1, 1, 0, 2, 1), states(2).ColumnQuality)
            assertEquals(2L, states(3).ColumnQuality.getCountValid())
            assertEquals(New CategoricalQuality(1, 1, 0, 2), states(4).ColumnQuality)
            assertEquals(New TimeQuality(1, 0, 1, 2), states(5).ColumnQuality)
            assertEquals(New BytesQuality(), states(6).ColumnQuality)
            For Each st As QualityAnalysisStateT In states.Take(6)
                assertEquals(2L, st.ColumnQuality.getCountTotal())
            Next st
        End Sub

        <TestMethod>
        Public Sub qualityAnalysisAddFunctionRejectsWrongRecordLengthAndUnsupportedTypes()
            Dim f As New QualityAnalysisAddFunction(schemaOfAllTypes())
            assertThrows(Of InvalidOperationException)(Sub() Call f.apply(Nothing, record(New TextT("x"))))

            ' As in Java, float columns have no quality state
            Dim floatSchema As SchemaT = (New SchemaT.Builder()).addColumnFloat("f").build()
            Dim floatFunction As New QualityAnalysisAddFunction(floatSchema)
            assertThrows(Of ArgumentException)(Sub() Call floatFunction.apply(Nothing, record(New DoubleWritable(1))))
        End Sub

        <TestMethod>
        Public Sub qualityAnalysisCombineFunctionMergesLists()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("i").addColumnCategorical("c", "a").build()
            Dim add As New QualityAnalysisAddFunction(schema)
            Dim part1 As IList(Of QualityAnalysisStateT) = add.apply(Nothing, record(New IntWritable(1), New TextT("a")))
            Dim part2 As IList(Of QualityAnalysisStateT) = add.apply(Nothing, record(New TextT("x"), New TextT("z")))
            add.apply(part2, record(NullWritable.INSTANCE, NullWritable.INSTANCE))

            Dim combine As New QualityAnalysisCombineFunction()
            assertTrue(combine.apply(Nothing, part2) Is part2)
            assertTrue(combine.apply(part1, Nothing) Is part1)

            Dim combined As IList(Of QualityAnalysisStateT) = combine.apply(part1, part2)
            assertEquals(2, combined.Count)
            assertEquals(New IntegerQuality(1, 1, 1, 3, 2), combined(0).ColumnQuality)
            assertEquals(New CategoricalQuality(1, 1, 1, 3), combined(1).ColumnQuality)

            assertThrows(Of InvalidOperationException)(Sub() Call combine.apply(part1, New List(Of QualityAnalysisStateT)()))

            ' the combined qualities feed the data quality report
            Dim report As New DataQualityAnalysisT(schema, combined.Select(Function(st) st.ColumnQuality).ToList())
            Dim lines() As String = report.ToString().Split(New Char() {ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
            assertEquals(3, lines.Length)
            assertTrue(lines(1).Contains("FAIL"))
            assertTrue(lines(1).Contains("IntegerQuality(countValid=1, countInvalid=1, countMissing=1, countTotal=3, countNonInteger=2)"))
        End Sub

        <TestMethod>
        Public Sub qualityStatesSurviveBinarySerializationAndKeepAdding()
            ' the states carry their add/merge functions and metadata, which are all <Serializable> like the Java ones
            Dim s As New StringQualityAnalysisState(New StringMetaData("s", "[a-z]+", Nothing, Nothing))
            s.add(New TextT("abc")).add(New TextT("123"))
            Dim copy As StringQualityAnalysisState = SerializationUtils.fromByteArray(Of StringQualityAnalysisState)(SerializationUtils.toByteArray(s))
            assertEquals(s.getStringQuality(), copy.getStringQuality())
            copy.add(New TextT("xyz"))
            Dim q As StringQuality = copy.getStringQuality()
            assertEquals(2L, q.getCountValid())
            assertEquals(1L, q.getCountInvalid())
            assertEquals(3L, q.getCountTotal())
            assertEquals(3.0, CDbl(q.getHll().cardinality()), 0.5)

            Dim f As New QualityAnalysisAddFunction(schemaOfAllTypes())
            Dim fCopy As QualityAnalysisAddFunction = SerializationUtils.fromByteArray(Of QualityAnalysisAddFunction)(SerializationUtils.toByteArray(f))
            Dim states As IList(Of QualityAnalysisStateT) = fCopy.apply(Nothing, record(New TextT("x"), New IntWritable(5), New LongWritable(1), New DoubleWritable(0.5), New TextT("a"), New LongWritable(10), New TextT("b")))
            assertEquals(1L, states(4).ColumnQuality.getCountValid())
        End Sub

    End Class

End Namespace
