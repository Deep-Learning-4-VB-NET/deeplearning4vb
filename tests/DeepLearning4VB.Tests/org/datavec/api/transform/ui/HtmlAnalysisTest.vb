Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BytesAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.BytesAnalysis
Imports CategoricalAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.CategoricalAnalysis
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DoubleAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.DoubleAnalysis
Imports HtmlAnalysisT = DeepLearning4VB.org.datavec.api.transform.ui.HtmlAnalysis
Imports HtmlReportSupportT = DeepLearning4VB.org.datavec.api.transform.ui.HtmlReportSupport
Imports IntegerAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.IntegerAnalysis
Imports LongAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.LongAnalysis
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceDataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports SequenceLengthAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports StringAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.StringAnalysis
Imports TimeAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.TimeAnalysis

Namespace org.datavec.api.transform.ui

    ''' <summary>
    ''' Tests of the HTML analysis report: content per column type, HTML encoding, empty analyses, file output,
    ''' sequence analyses and culture-invariant number formatting.
    ''' </summary>
    <TestClass>
    Public Class HtmlAnalysisTest

        ''' <summary>An analysis of five columns: integer and string (with histograms), double, categorical and time.</summary>
        Friend Shared Function smallAnalysis() As DataAnalysisT
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("age").addColumnDouble("score").addColumnCategorical("colour", "red", "green", "blue").
                addColumnString("comment").addColumnTime("timestamp", DateTimeZone.UTC).build()
            Dim colours As New Dictionary(Of String, Long) From {{"green", 3L}, {"red", 6L}, {"blue", 1L}}
            Dim list As New List(Of ColumnAnalysisT) From {
                (New IntegerAnalysisT.Builder()).min(1).max(1234).mean(617.5).sampleStdev(12.25).sampleVariance(150.0625).countZero(0).countNegative(0).countPositive(1500).
                    countMinValue(2).countMaxValue(3).countTotal(1500).histogramBuckets(New Double() {0, 500, 1000, 1500}).histogramBucketCounts(New Long() {700, 500, 300}).build(),
                (New DoubleAnalysisT.Builder()).min(-2.5).max(3.75).countNaN(7).mean(0.125).sampleStdev(1.5).sampleVariance(2.25).countNegative(400).countPositive(593).
                    countTotal(1000).build(),
                New CategoricalAnalysisT(colours),
                (New StringAnalysisT.Builder()).minLength(2).maxLength(9).meanLength(4.5).sampleStdevLength(1.25).sampleVarianceLength(1.5625).countTotal(42).
                    histogramBuckets(New Double() {2, 5, 9}).histogramBucketCounts(New Long() {30, 12}).build(),
                (New TimeAnalysisT.Builder()).min(1451606400000L).max(1451606400000L + 60000L).mean(1451606430000.0).countTotal(60).build()}
            Return New DataAnalysisT(schema, list)
        End Function

        <TestMethod>
        Public Sub reportShowsEveryColumnWithItsStatisticsAndHistograms()
            Dim html As String = HtmlAnalysisT.createHtmlAnalysisString(smallAnalysis())
            assertSelfContained(html)
            assertTrue(html.Contains("<title>Data Analysis</title>"))
            assertTrue(html.Contains("5 columns"))

            For Each name As String In New String() {"age", "score", "colour", "comment", "timestamp"}
                assertTrue(html.Contains("<h3>" & name & "</h3>"), name)
            Next
            For Each type As String In New String() {"Integer", "Double", "Categorical", "String", "Time"}
                assertTrue(html.Contains("<span class=""dv-badge"">" & type & "</span>"), type)
            Next

            'Integer column
            assertStat(html, "Count", "1,500")
            assertStat(html, "Min", "1")
            assertStat(html, "Max", "1,234")
            assertStat(html, "Mean", "617.5")
            assertStat(html, "Sample std dev", "12.25")
            assertStat(html, "Values equal to max", "3")
            'Double column, with its invalid (NaN) count
            assertStat(html, "Min", "-2.5")
            assertStat(html, "Max", "3.75")
            assertStat(html, "Mean", "0.125")
            assertStat(html, "NaN values", "7")
            'Categorical column: counts and top categories, most frequent first
            assertStat(html, "Count", "10")
            assertStat(html, "Distinct categories", "3")
            assertStat(html, "Most frequent", "red (60.0%)")
            assertTrue(html.Contains("<caption>Top categories</caption>"))
            assertTrue(html.Contains("<tr><td>red</td><td class=""dv-num"">6</td><td class=""dv-num"">60.0%</td></tr>"))
            assertTrue(html.IndexOf("<td>red</td>", StringComparison.Ordinal) < html.IndexOf("<td>green</td>", StringComparison.Ordinal))
            assertTrue(html.IndexOf("<td>green</td>", StringComparison.Ordinal) < html.IndexOf("<td>blue</td>", StringComparison.Ordinal))
            'String column
            assertStat(html, "Min length", "2")
            assertStat(html, "Max length", "9")
            assertStat(html, "Mean length", "4.5")
            'Time column, printed in UTC
            assertStat(html, "Earliest", "2016-01-01 00:00:00.000 UTC")
            assertStat(html, "Latest", "2016-01-01 00:01:00.000 UTC")
            assertStat(html, "Mean", "2016-01-01 00:00:30.000 UTC")

            'Histograms for the integer and the string column only
            assertEquals(2, countOccurrences(html, "<figure class=""dv-figure dv-histogram"">"))
            assertTrue(html.Contains("<figcaption>Histogram of values</figcaption>"))
            assertTrue(html.Contains("<figcaption>Histogram of string lengths</figcaption>"))
            assertEquals(5, countOccurrences(html, "<path class=""dv-bar"""))
            assertTrue(html.Contains("<title>0 to 500: 700</title>"))
            assertTrue(html.Contains("<title>5 to 9: 12</title>"))

            'The summary table lists every column; the raw analysis text is kept in the details
            assertTrue(html.Contains("<th scope=""col"">Column Name</th>"))
            assertTrue(html.Contains("min 1, max 1,234, mean 617.5"))
            assertTrue(html.Contains("IntegerAnalysis(min=1, max=1234, mean=617.5"))
        End Sub

        <TestMethod>
        Public Sub columnNamesAndCategoriesAreHtmlEncoded()
            Dim evilName As String = "<script>alert('x')</script>"
            Dim evilCategory As String = "<img src=x onerror=""alert(1)"">&"
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnCategorical(evilName, evilCategory, "ok").addColumnLong("x<y").build()
            Dim list As New List(Of ColumnAnalysisT) From {
                New CategoricalAnalysisT(New Dictionary(Of String, Long) From {{evilCategory, 5L}, {"ok", 1L}}),
                (New LongAnalysisT.Builder()).min(-5).max(5).countTotal(3).build()}
            Dim html As String = HtmlAnalysisT.createHtmlAnalysisString(New DataAnalysisT(schema, list))

            assertSelfContained(html)
            assertFalse(html.Contains("<script"))
            assertFalse(html.Contains("<img"))
            assertFalse(html.Contains("x<y"))
            assertTrue(html.Contains("<h3>&lt;script&gt;alert(&#39;x&#39;)&lt;/script&gt;</h3>"))
            assertTrue(html.Contains("<td>&lt;img src=x onerror=&quot;alert(1)&quot;&gt;&amp;</td>"))
            assertTrue(html.Contains("<h3>x&lt;y</h3>"))
        End Sub

        <TestMethod>
        Public Sub emptyAnalysisGivesAPageWithoutColumns()
            Dim html As String = HtmlAnalysisT.createHtmlAnalysisString(New DataAnalysisT((New SchemaT.Builder()).build(), New List(Of ColumnAnalysisT)()))
            assertSelfContained(html)
            assertTrue(html.Contains("This analysis has no columns."))
            assertTrue(html.Contains("0 columns"))
            assertFalse(html.Contains("<svg"))
            assertFalse(html.Contains("<table"))
            assertThrows(Of ArgumentNullException)(Sub() Call HtmlAnalysisT.createHtmlAnalysisString(Nothing))
        End Sub

        <TestMethod>
        Public Sub missingAnalysesAndUnusableHistogramsAreTolerated()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("a").addColumnInteger("b").addColumnCategorical("c", "x").build()
            Dim list As New List(Of ColumnAnalysisT) From {
                Nothing,
                (New IntegerAnalysisT.Builder()).min(0).max(1).countTotal(2).histogramBuckets(New Double() {0, 1}).histogramBucketCounts(New Long() {1, 1}).build(),
                New CategoricalAnalysisT(New Dictionary(Of String, Long)())}
            Dim html As String = HtmlAnalysisT.createHtmlAnalysisString(New DataAnalysisT(schema, list))
            assertTrue(html.Contains("No analysis is available for this column."))
            'Two counts need three bucket edges: the histogram is left out instead of failing
            assertFalse(html.Contains("dv-histogram"))
            assertStat(html, "Distinct categories", "0")
        End Sub

        <TestMethod>
        Public Sub bytesAnalysisShowsItsNullCount()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnInteger("placeholder").build()
            Dim list As New List(Of ColumnAnalysisT) From {New BytesAnalysisT(10, 2, 1, 0, 2048)}
            Dim html As String = HtmlAnalysisT.createHtmlAnalysisString(New DataAnalysisT(schema, list))
            assertStat(html, "Null values", "2")
            assertStat(html, "Zero-length values", "1")
            assertStat(html, "Max bytes", "2,048")
        End Sub

        <TestMethod>
        Public Sub sequenceAnalysisAddsTheSequenceLengths()
            Dim da As DataAnalysisT = smallAnalysis()
            Dim sla As SequenceLengthAnalysisT = SequenceLengthAnalysisT.builder().totalNumSequences(100).minSeqLength(1).maxSeqLength(50).countZeroLength(0).
                countOneLength(10).meanLength(20.0).histogramBuckets(New Double() {0, 10, 20, 30, 40, 50}).histogramBucketCounts(New Long() {20, 30, 25, 15, 10}).build()
            Dim html As String = HtmlAnalysisT.createHtmlAnalysisString(New SequenceDataAnalysisT(da.getSchema(), da.getColumnAnalysis(), sla))
            assertSelfContained(html)
            assertTrue(html.Contains("<title>Sequence Data Analysis</title>"))
            assertTrue(html.Contains("100 sequences"))
            assertTrue(html.Contains("<h3>Sequence Lengths</h3>"))
            assertTrue(html.Contains("<figcaption>Histogram of sequence lengths</figcaption>"))
            assertStat(html, "Sequences", "100")
            assertStat(html, "One-step sequences", "10")
            assertStat(html, "Mean length", "20")
            'The sequence lengths come first, before the first column
            assertTrue(html.IndexOf("<h3>Sequence Lengths</h3>", StringComparison.Ordinal) < html.IndexOf("<h3>age</h3>", StringComparison.Ordinal))
            assertEquals(3, countOccurrences(html, "<figure class=""dv-figure dv-histogram"">"))
        End Sub

        <TestMethod>
        Public Sub reportIsWrittenAsUtf8WithoutBom()
            Dim dir As String = newTempDir()
            Try
                Dim path As String = System.IO.Path.Combine(dir, "nested", "deeper", "analysis.html")
                HtmlAnalysisT.createHtmlAnalysisFile(smallAnalysis(), path)
                assertTrue(System.IO.File.Exists(path))
                Dim bytes As Byte() = System.IO.File.ReadAllBytes(path)
                assertEquals(CByte(AscW("<"c)), bytes(0))
                Dim html As String = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8)
                assertTrue(html.Contains("<h3>colour</h3>"))
                assertTrue(html.EndsWith("</html>" & vbLf, StringComparison.Ordinal))
                assertThrows(Of ArgumentException)(Sub() HtmlAnalysisT.createHtmlAnalysisFile(smallAnalysis(), ""))
            Finally
                System.IO.Directory.Delete(dir, True)
            End Try
            assertFalse(System.IO.Directory.Exists(dir))
        End Sub

        <TestMethod>
        Public Sub numbersDoNotDependOnTheCurrentCulture()
            Dim previous As CultureInfo = Thread.CurrentThread.CurrentCulture
            Try
                Thread.CurrentThread.CurrentCulture = New CultureInfo("de-DE")
                Dim html As String = HtmlAnalysisT.createHtmlAnalysisString(smallAnalysis())
                assertStat(html, "Max", "1,234")
                assertStat(html, "Mean", "617.5")
                assertFalse(html.Contains("617,5"))
                assertFalse(html.Contains("1.234<"))
            Finally
                Thread.CurrentThread.CurrentCulture = previous
            End Try
        End Sub

        <TestMethod>
        Public Sub formattingHelpers()
            assertEquals("0", HtmlReportSupportT.FormatValue(0.0))
            assertEquals("0", HtmlReportSupportT.FormatValue(-0.0))
            assertEquals("1,234", HtmlReportSupportT.FormatValue(1234.0))
            assertEquals("-9,999,999,999", HtmlReportSupportT.FormatValue(-9999999999.0))
            assertEquals("1,234.57", HtmlReportSupportT.FormatValue(1234.5678))
            assertEquals("0.333333", HtmlReportSupportT.FormatValue(1.0 / 3))
            assertEquals("0.3", HtmlReportSupportT.FormatValue(0.1 + 0.2))
            assertEquals("0.00012345", HtmlReportSupportT.FormatValue(0.00012345))
            assertEquals("1.5E+20", HtmlReportSupportT.FormatValue(1.5E+20))
            assertEquals("2.5E-9", HtmlReportSupportT.FormatValue(2.5E-9))
            assertEquals("NaN", HtmlReportSupportT.FormatValue(Double.NaN))
            assertEquals("-Infinity", HtmlReportSupportT.FormatValue(Double.NegativeInfinity))
            assertEquals("1,234,567", HtmlReportSupportT.FormatCount(1234567L))
            assertEquals("12.5%", HtmlReportSupportT.FormatShare(0.125))
            assertEquals("2.5M", HtmlReportSupportT.FormatTick(2500000.0))
            assertEquals("100M", HtmlReportSupportT.FormatTick(100000000.0))
            assertEquals("999,999", HtmlReportSupportT.FormatTick(999999.0))
            assertEquals("2016-01-01 00:00:00.000 UTC", HtmlReportSupportT.FormatEpochMillis(1451606400000L))
            assertEquals("1970-01-01 00:00:00.001 UTC", HtmlReportSupportT.FormatEpochMillis(0.6))
        End Sub

        <TestMethod>
        Public Sub htmlEncodeEscapesMarkupQuotesAndControlCharacters()
            assertEquals("", HtmlReportSupportT.HtmlEncode(Nothing))
            assertEquals("&lt;a href=&quot;x&quot; title=&#39;y&#39;&gt;&amp;&lt;/a&gt;", HtmlReportSupportT.HtmlEncode("<a href=""x"" title='y'>&</a>"))
            assertEquals("a" & ChrW(&HFFFD) & "b" & vbTab & "c" & vbLf, HtmlReportSupportT.HtmlEncode("a" & ChrW(0) & "b" & vbTab & "c" & vbLf))
            assertEquals("caf" & ChrW(&HE9), HtmlReportSupportT.HtmlEncode("caf" & ChrW(&HE9)))
        End Sub

        <TestMethod>
        Public Sub axisTicksAreNiceNumbers()
            assertEquals(1.0, HtmlReportSupportT.NiceStep(0.7), 0.0)
            assertEquals(2.0, HtmlReportSupportT.NiceStep(1.3), 0.0)
            assertEquals(25.0, HtmlReportSupportT.NiceStep(22.0), 0.0)
            assertEquals(500.0, HtmlReportSupportT.NiceStep(301.0), 0.0)
            assertEquals(New List(Of Double) From {-3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0}, HtmlReportSupportT.TicksBetween(-3, 3, 1))
            assertEquals(New List(Of Double) From {0.0, 0.25, 0.5}, HtmlReportSupportT.TicksBetween(-0.1, 0.6, 0.25))
            Dim ticks As IList(Of Double) = HtmlReportSupportT.HorizontalTicks(0, 1.0E+9, 480)
            assertTrue(ticks.Count >= 2 AndAlso ticks.Count <= 9)
        End Sub

    End Class

End Namespace
