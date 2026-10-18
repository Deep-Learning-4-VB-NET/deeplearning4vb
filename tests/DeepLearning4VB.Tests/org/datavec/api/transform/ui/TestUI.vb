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
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports HtmlAnalysisT = DeepLearning4VB.org.datavec.api.transform.ui.HtmlAnalysis
Imports HtmlSequencePlottingT = DeepLearning4VB.org.datavec.api.transform.ui.HtmlSequencePlotting
Imports IntegerAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.IntegerAnalysis
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceDataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports SequenceLengthAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports StringAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.StringAnalysis
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text
Imports TimeAnalysis = DeepLearning4VB.org.datavec.api.transform.analysis.columns.TimeAnalysis
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.ui

    ''' <summary>
    ''' Port of the DataVec <c>TestUI</c>: writes the HTML analysis report of a data analysis and of a sequence data
    ''' analysis, and the HTML plots of a sequence. Files go to a per-test temporary directory (JUnit's <c>@TempDir</c>)
    ''' that is deleted afterwards.
    ''' </summary>
    <TestClass>
    Public Class TestUI

        Private testDir As String

        <TestInitialize>
        Public Sub setUp()
            testDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "dl4vb-testui-" & Guid.NewGuid().ToString("N"))
            System.IO.Directory.CreateDirectory(testDir)
        End Sub

        <TestCleanup>
        Public Sub tearDown()
            If testDir IsNot Nothing AndAlso System.IO.Directory.Exists(testDir) Then System.IO.Directory.Delete(testDir, True)
        End Sub

        <TestMethod>
        Public Sub testUI()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("StringColumn").addColumnInteger("IntColumn").addColumnInteger("IntColumn2").addColumnInteger("IntColumn3").addColumnTime("TimeColumn", DateTimeZone.UTC).build()

            Dim list As IList(Of ColumnAnalysisT) = New List(Of ColumnAnalysisT)()
            list.Add((New StringAnalysis.Builder()).countTotal(10).maxLength(7).countTotal(999999999L).minLength(99999999).maxLength(99999999).meanLength(9999999999.0).sampleStdevLength(99999999.0).sampleVarianceLength(0.99999999999).histogramBuckets(New Double() {0, 1, 2, 3, 4, 5}).histogramBucketCounts(New Long() {50, 30, 10, 12, 3}).build())

            list.Add((New IntegerAnalysis.Builder()).countTotal(10).countMaxValue(1).countMinValue(4).min(0).max(30).countTotal(999999999).countMaxValue(99999999).countMinValue(999999999).min(-999999999).max(9999999).min(99999999).max(99999999).mean(9999999999.0).sampleStdev(99999999.0).sampleVariance(0.99999999999).histogramBuckets(New Double() {-3, -2, -1, 0, 1, 2, 3}).histogramBucketCounts(New Long() {100_000_000, 20_000_000, 30_000_000, 40_000_000, 50_000_000, 60_000_000}).build())

            list.Add((New IntegerAnalysis.Builder()).countTotal(10).countMaxValue(1).countMinValue(4).min(0).max(30).histogramBuckets(New Double() {-3, -2, -1, 0, 1, 2, 3}).histogramBucketCounts(New Long() {15, 20, 35, 40, 55, 60}).build())

            Dim t As TDigest = TDigest.createDigest(100)
            For i As Integer = 0 To 99
                t.add(i)
            Next i
            list.Add((New IntegerAnalysis.Builder()).countTotal(10).countMaxValue(1).countMinValue(4).min(0).max(30).histogramBuckets(New Double() {-3, -2, -1, 0, 1, 2, 3}).histogramBucketCounts(New Long() {10, 2, 3, 4, 5, 6}).digest(t).build())

            list.Add((New TimeAnalysis.Builder()).min(1451606400000L).max(1451606400000L + 60000L).build())


            Dim da As New DataAnalysisT(schema, list)

            Dim outPath As String = System.IO.Path.Combine(testDir, "datavec_transform_UITest.html")
            HtmlAnalysisT.createHtmlAnalysisFile(da, outPath)
            Dim html As String = System.IO.File.ReadAllText(outPath)
            For Each name As String In schema.getColumnNames()
                assertTrue(html.Contains("<h3>" & name & "</h3>"), name)
            Next
            'The four string/integer histograms; the time analysis has none
            assertEquals(4, UiTestSupport.countOccurrences(html, "<figure class=""dv-figure dv-histogram"">"))
            'The t-digest quantiles of IntColumn3
            assertTrue(html.Contains("Quantile 0.5 (median)"))
            assertTrue(html.Contains("2016-01-01 00:00:00.000 UTC"))


            'Test JSON (Java: assertEquals(da, da2)). The analysis classes compare by reference in this port,
            'so the round trip is checked through its JSON text and the schema.
            Dim json As String = da.toJson()
            Dim da2 As DataAnalysisT = DataAnalysisT.fromJson(json)
            assertEquals(json, da2.toJson())
            assertEquals(da.getSchema(), da2.getSchema())
            assertEquals(da.getColumnAnalysis().Count, da2.getColumnAnalysis().Count)



            'Test sequence analysis:
            Dim sla As SequenceLengthAnalysisT = SequenceLengthAnalysisT.builder().totalNumSequences(100).minSeqLength(1).maxSeqLength(50).countZeroLength(0).countOneLength(10).meanLength(20.0).histogramBuckets(New Double() {0.0, 1.0, 2.0, 3.0, 4.0, 5.0}).histogramBucketCounts(New Long() {1, 2, 3, 4, 5}).build()
            Dim sda As New SequenceDataAnalysisT(da.getSchema(), da.getColumnAnalysis(), sla)


            'HTML:
            outPath = System.IO.Path.Combine(testDir, "datavec_transform_UITest_seq.html")
            HtmlAnalysisT.createHtmlAnalysisFile(sda, outPath)
            html = System.IO.File.ReadAllText(outPath)
            assertTrue(html.Contains("<h3>Sequence Lengths</h3>"))
            assertEquals(5, UiTestSupport.countOccurrences(html, "<figure class=""dv-figure dv-histogram"">"))


            'JSON (Java: assertEquals(sda, sda2)), checked as above
            Dim json2 As String = sda.toJson()
            Dim sda2 As SequenceDataAnalysisT = SequenceDataAnalysisT.fromJson(json2)
            assertEquals(json2, sda2.toJson())
            assertEquals(sda.getSchema(), sda2.getSchema())
        End Sub


        ''' <summary>Java marked this test @Disabled (it wrote to java.io.tmpdir); here it writes to the test's temporary directory.</summary>
        <TestMethod>
        Public Sub testSequencePlot()

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnDouble("sinx").addColumnCategorical("cat", "s0", "s1", "s2").addColumnString("stringcol").build()

            Dim nSteps As Integer = 100
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))(nSteps)
            For i As Integer = 0 To nSteps - 1
                Dim c As String = "s" & (i Mod 3)
                sequence.Add(New List(Of WritableT) From {New DoubleWritable(Math.Sin(i / 10.0)), New TextT(c), New TextT(i.ToString())})
            Next i

            Dim outPath As String = System.IO.Path.Combine(testDir, "datavec_seqplot_test.html")
            HtmlSequencePlottingT.createHtmlSequencePlotFile("Title!", schema, sequence, outPath)

            Dim html As String = System.IO.File.ReadAllText(outPath)
            assertTrue(html.Contains("<title>Title!</title>"))
            'sinx and cat are plotted, stringcol is not
            assertEquals(2, UiTestSupport.countOccurrences(html, "<figure class=""dv-figure dv-linechart"">"))
            assertTrue(html.Contains("Column: &quot;cat&quot; - Column Type: Categorical - (0=s0, 1=s1, 2=s2)"))
            assertTrue(html.Contains("Not plotted: stringcol (String)"))
        End Sub
    End Class

End Namespace
