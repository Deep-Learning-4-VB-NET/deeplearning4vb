Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports HtmlSequencePlottingT = DeepLearning4VB.org.datavec.api.transform.ui.HtmlSequencePlotting
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.ui

    ''' <summary>Tests of the HTML sequence plots: one line chart per plottable column, gaps, encoding and file output.</summary>
    <TestClass>
    Public Class HtmlSequencePlottingTest

        Private Shared Function twoColumnSchema() As SchemaT
            Return (New SequenceSchemaT.Builder()).addColumnDouble("temperature").addColumnInteger("count").build()
        End Function

        Private Shared Function steps(ByVal ParamArray rows()() As WritableT) As IList(Of IList(Of WritableT))
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each row As WritableT() In rows
                sequence.Add(New List(Of WritableT)(row))
            Next
            Return sequence
        End Function

        Private Shared Function twoColumnSequence() As IList(Of IList(Of WritableT))
            Return steps(New WritableT() {New DoubleWritable(1.5), New IntWritable(3)},
                         New WritableT() {New DoubleWritable(2.0), New IntWritable(5)},
                         New WritableT() {New DoubleWritable(-0.5), New IntWritable(4)},
                         New WritableT() {New DoubleWritable(0.25), New IntWritable(8)},
                         New WritableT() {New DoubleWritable(3.0), New IntWritable(1)})
        End Function

        <TestMethod>
        Public Sub twoColumnSequenceGivesOneLineChartPerColumn()
            Dim html As String = HtmlSequencePlottingT.createHtmlSequencePlots("Sensor 7", twoColumnSchema(), twoColumnSequence())
            assertSelfContained(html)
            assertTrue(html.Contains("<title>Sensor 7</title>"))
            assertTrue(html.Contains("<h1>Sensor 7</h1>"))
            assertTrue(html.Contains("5 time steps"))
            assertTrue(html.Contains("2 columns"))

            'Schema table: index, name and type of each column, two columns per row as in Java
            assertTrue(html.Contains("<tr><td class=""dv-num"">0</td><td>temperature</td><td>Double</td><td class=""dv-num"">1</td><td>count</td><td>Integer</td></tr>"))

            assertEquals(2, countOccurrences(html, "<figure class=""dv-figure dv-linechart"">"))
            assertTrue(html.Contains("<figcaption>Column: &quot;temperature&quot; - Column Type: Double</figcaption>"))
            assertTrue(html.Contains("<figcaption>Column: &quot;count&quot; - Column Type: Integer</figcaption>"))
            assertEquals(2, countOccurrences(html, "<polyline class=""dv-line"""))
            'Few points: every point is marked and has a tooltip
            assertEquals(10, countOccurrences(html, "<circle class=""dv-dot"""))
            assertTrue(html.Contains("<title>temperature: x = 0, y = 1.5</title>"))
            assertTrue(html.Contains("<title>count: x = 3, y = 8</title>"))
            'A single series needs no legend
            assertFalse(html.Contains("<ul class=""dv-legend"">"))
        End Sub

        <TestMethod>
        Public Sub categoricalColumnsArePlottedByStateIndexAndOthersAreListed()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnCategorical("state", "low", "high").addColumnString("note").build()
            Dim sequence As IList(Of IList(Of WritableT)) = steps(New WritableT() {New TextT("high"), New TextT("a")},
                                                                  New WritableT() {New TextT("low"), New TextT("b")},
                                                                  New WritableT() {New TextT("high"), New TextT("c")})
            Dim html As String = HtmlSequencePlottingT.createHtmlSequencePlots("States", schema, sequence)
            assertEquals(1, countOccurrences(html, "<figure class=""dv-figure dv-linechart"">"))
            assertTrue(html.Contains("Column: &quot;state&quot; - Column Type: Categorical - (0=low, 1=high)"))
            assertTrue(html.Contains("<title>state: x = 1, y = 0</title>"))
            assertTrue(html.Contains("<title>state: x = 2, y = 1</title>"))
            assertTrue(html.Contains("Not plotted: note (String)"))
        End Sub

        <TestMethod>
        Public Sub missingValuesLeaveAGapInTheLine()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnDouble("v").build()
            Dim sequence As IList(Of IList(Of WritableT)) = steps(New WritableT() {New DoubleWritable(1)}, New WritableT() {New DoubleWritable(2)},
                                                                  New WritableT() {NullWritable.INSTANCE}, New WritableT() {New TextT("not a number")},
                                                                  New WritableT() {New DoubleWritable(4)}, New WritableT() {New DoubleWritable(5)})
            Dim html As String = HtmlSequencePlottingT.createHtmlSequencePlots("Gaps", schema, sequence)
            assertEquals(2, countOccurrences(html, "<polyline class=""dv-line"""))
            assertEquals(4, countOccurrences(html, "<g class=""dv-point"">"))
        End Sub

        <TestMethod>
        Public Sub titleAndNamesAreHtmlEncoded()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnDouble("<b>bold</b>").build()
            Dim sequence As IList(Of IList(Of WritableT)) = steps(New WritableT() {New DoubleWritable(1)}, New WritableT() {New DoubleWritable(2)})
            Dim html As String = HtmlSequencePlottingT.createHtmlSequencePlots("<script>alert(1)</script>", schema, sequence)
            assertSelfContained(html)
            assertFalse(html.Contains("<script"))
            assertFalse(html.Contains("<b>"))
            assertTrue(html.Contains("<title>&lt;script&gt;alert(1)&lt;/script&gt;</title>"))
            assertTrue(html.Contains("<td>&lt;b&gt;bold&lt;/b&gt;</td>"))
            assertTrue(html.Contains("<title>&lt;b&gt;bold&lt;/b&gt;: x = 1, y = 2</title>"))
        End Sub

        <TestMethod>
        Public Sub emptySequenceGivesEmptyCharts()
            Dim html As String = HtmlSequencePlottingT.createHtmlSequencePlots("Empty", twoColumnSchema(), New List(Of IList(Of WritableT))())
            assertSelfContained(html)
            assertTrue(html.Contains("0 time steps"))
            assertEquals(2, countOccurrences(html, ">No data</text>"))
            assertFalse(html.Contains("<polyline"))
            assertThrows(Of ArgumentNullException)(Sub() Call HtmlSequencePlottingT.createHtmlSequencePlots("x", Nothing, New List(Of IList(Of WritableT))()))
        End Sub

        <TestMethod>
        Public Sub plotsAreWrittenToAFile()
            Dim dir As String = newTempDir()
            Try
                Dim path As String = System.IO.Path.Combine(dir, "plots", "sequence.html")
                HtmlSequencePlottingT.createHtmlSequencePlotFile("Sensor 7", twoColumnSchema(), twoColumnSequence(), path)
                Dim bytes As Byte() = System.IO.File.ReadAllBytes(path)
                assertEquals(CByte(AscW("<"c)), bytes(0))
                Dim html As String = System.IO.File.ReadAllText(path)
                assertEquals(2, countOccurrences(html, "<figure class=""dv-figure dv-linechart"">"))
            Finally
                System.IO.Directory.Delete(dir, True)
            End Try
        End Sub

    End Class

End Namespace
