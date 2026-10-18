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
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports BytesAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.BytesAnalysis
Imports CategoricalAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.CategoricalAnalysis
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DoubleAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.DoubleAnalysis
Imports IntegerAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.IntegerAnalysis
Imports LongAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.LongAnalysis
Imports NDArrayAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.NDArrayAnalysis
Imports NumericalColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.NumericalColumnAnalysis
Imports RenderableComponentHistogram = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponentHistogram
Imports RenderableComponentTable = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponentTable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceDataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports SequenceLengthAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports StringAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.StringAnalysis
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest
Imports TimeAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.TimeAnalysis

Namespace org.datavec.api.transform.ui

    ''' <summary>
    ''' Renders a <see cref="DataAnalysisT"/> as a self-contained HTML page: an overview table of all columns, then one
    ''' card per column with its type, the summary statistics of its analysis (count, minimum, maximum, mean, standard
    ''' deviation, NaN/null counts, quantiles, top categories), a histogram where the analysis has one, and the raw
    ''' analysis text. A <see cref="SequenceDataAnalysisT"/> gets an extra card for the sequence lengths.
    ''' <para>Java rendered a FreeMarker template (<c>templates/analysis.ftl</c>) with JavaScript charts from the DL4J
    ''' UI; this port builds the page itself: inline CSS (light and dark through <c>prefers-color-scheme</c>), inline
    ''' SVG charts, HTML tables, no scripts and no external resources. Numbers are formatted culture-invariantly and
    ''' every value that comes from the data (column names, category values) is HTML-encoded.</para>
    ''' </summary>
    Public Class HtmlAnalysis

        Private Const MAX_CATEGORIES_SHOWN As Integer = 10
        Private Shared ReadOnly Separator As String = " " & ChrW(&HB7) & " "
        Private Shared ReadOnly QUANTILES() As Double = {0.001, 0.01, 0.1, 0.5, 0.9, 0.99, 0.999}

        Private Sub New()

        End Sub

        ''' <summary>
        ''' Render a data analysis object as a HTML page. This produces a summary table and one section per column,
        ''' with a histogram for the columns whose analysis has one. The contents of the HTML file are returned as a
        ''' String, which should be written to a .html file.
        ''' </summary>
        ''' <param name="analysis"> Data analysis object to render </param>
        ''' <seealso cref="createHtmlAnalysisFile(DataAnalysisT, String)"/>
        Public Shared Function createHtmlAnalysisString(ByVal analysis As DataAnalysisT) As String
            If analysis Is Nothing Then Throw New ArgumentNullException(NameOf(analysis))
            Dim caList As IList(Of ColumnAnalysisT) = If(analysis.getColumnAnalysis(), New List(Of ColumnAnalysisT)())
            Dim schema As SchemaT = analysis.getSchema()

            Dim sla As SequenceLengthAnalysisT = Nothing
            Dim sda As SequenceDataAnalysisT = TryCast(analysis, SequenceDataAnalysisT)
            If sda IsNot Nothing Then sla = sda.getSequenceLengthAnalysis()

            Dim sections As New List(Of ColumnSection)()
            If sla IsNot Nothing Then sections.Add(describeSequenceLengths(sla))
            For i As Integer = 0 To caList.Count - 1
                sections.Add(describeColumn(i, schema, caList(i)))
            Next

            'One block per column, as Java's list of divs (the content is HTML here, not JSON)
            Dim divs As IList(Of DivObject) = New List(Of DivObject)()
            For Each section As ColumnSection In sections
                divs.Add(New DivObject(section.Id, renderCard(section)))
            Next

            Dim body As New StringBuilder()
            Dim lf As String = ControlChars.Lf
            If sections.Count = 0 Then
                body.Append("<p class=""dv-note"">This analysis has no columns.</p>").Append(lf)
            Else
                'Create the summary table
                Dim table As New List(Of String())()
                For Each section As ColumnSection In sections
                    table.Add(New String() {section.IndexText, section.Name, section.TypeName, section.Count, section.Overview})
                Next
                Dim rct As RenderableComponentTable = (New RenderableComponentTable.Builder()).table(table.ToArray()).
                    header("#", "Column Name", "Column Type", "Count", "Overview").colWidthsPercent(5, 22, 12, 11, 50).build()
                body.Append("<section class=""dv-section"" id=""summary""><h2>Summary</h2>").Append(rct.renderHtml("dv-summary")).Append("</section>").Append(lf)
                body.Append("<section class=""dv-section"" id=""columns""><h2>Columns</h2>").Append(lf)
                For Each div As DivObject In divs
                    body.Append("<article class=""dv-card"" id=""").Append(HtmlEncode(div.getId())).Append(""">").Append(div.getContent()).Append("</article>").Append(lf)
                Next
                body.Append("</section>").Append(lf)
            End If

            Dim meta As String = FormatCount(caList.Count) & If(caList.Count = 1, " column", " columns")
            If sla IsNot Nothing Then meta &= Separator & FormatCount(sla.getTotalNumSequences()) & If(sla.getTotalNumSequences() = 1, " sequence", " sequences")
            meta &= Separator & "generated " & FormatGeneratedTime()
            Dim title As String = If(sda IsNot Nothing, "Sequence Data Analysis", "Data Analysis")
            Return BuildPage(title, HtmlEncode(meta), body.ToString())
        End Function

        ''' <summary>
        ''' Render a data analysis object as a HTML file. This will produce a summary table, along with charts for
        ''' numerical columns. The file is written as UTF-8 (without byte order mark); missing parent directories are
        ''' created.
        ''' </summary>
        ''' <param name="dataAnalysis"> Data analysis object to render </param>
        ''' <param name="output">       Output file path (should have extension .html) </param>
        Public Shared Sub createHtmlAnalysisFile(ByVal dataAnalysis As DataAnalysisT, ByVal output As String)
            Dim str As String = createHtmlAnalysisString(dataAnalysis)
            WriteReport(output, str)
        End Sub

        ' ---- one card per column ----

        ''' <summary>What the report shows for one column (or for the sequence lengths).</summary>
        Private NotInheritable Class ColumnSection
            Public Id As String
            Public IndexText As String = ""
            Public Name As String
            Public TypeName As String
            Public Count As String = ""
            Public Overview As String = ""
            Public ReadOnly Stats As New List(Of String())()
            Public HistogramTitle As String
            Public Buckets() As Double
            Public BucketCounts() As Long
            Public Categories As String()()
            Public CategoriesNote As String
            Public TimeAxis As Boolean
            Public Raw As String
            Public Note As String
        End Class

        Private Shared Function renderCard(ByVal section As ColumnSection) As String
            Dim sb As New StringBuilder()
            sb.Append("<header class=""dv-card-head"">")
            If section.IndexText.Length > 0 Then sb.Append("<span class=""dv-index"">#").Append(HtmlEncode(section.IndexText)).Append("</span>")
            sb.Append("<h3>").Append(HtmlEncode(section.Name)).Append("</h3>")
            sb.Append("<span class=""dv-badge"">").Append(HtmlEncode(section.TypeName)).Append("</span>")
            sb.Append("</header>")

            sb.Append("<div class=""dv-card-body""><div class=""dv-card-stats"">")
            If section.Stats.Count > 0 Then
                Dim stats As RenderableComponentTable = (New RenderableComponentTable.Builder()).table(section.Stats.ToArray()).build()
                sb.Append(stats.renderHtml("dv-stats"))
            End If
            If section.Categories IsNot Nothing Then
                Dim categories As RenderableComponentTable = (New RenderableComponentTable.Builder()).title("Top categories").header("Category", "Count", "Share").
                    table(section.Categories).colWidthsPercent(60, 22, 18).build()
                sb.Append("<div class=""dv-categories"">").Append(categories.renderHtml("dv-compact")).Append("</div>")
                If section.CategoriesNote IsNot Nothing Then sb.Append("<p class=""dv-note"">").Append(HtmlEncode(section.CategoriesNote)).Append("</p>")
            End If
            If section.Note IsNot Nothing Then sb.Append("<p class=""dv-note"">").Append(HtmlEncode(section.Note)).Append("</p>")
            sb.Append("</div>")

            Dim hist As RenderableComponentHistogram = buildHistogram(section)
            If hist IsNot Nothing Then sb.Append("<div class=""dv-card-chart"">").Append(hist.toHtml()).Append("</div>")
            sb.Append("</div>")

            If Not String.IsNullOrEmpty(section.Raw) Then
                'Java showed this text in its summary table, with the same replacements for readability
                Dim raw As String = section.Raw.Replace(",", ", ").Replace(" -> ", " : ")
                sb.Append("<details class=""dv-raw""><summary>Analysis details</summary><pre>").Append(HtmlEncode(raw)).Append("</pre></details>")
            End If
            Return sb.ToString()
        End Function

        ''' <summary>The histogram of a section, or Nothing when its analysis has none (or the bucket arrays do not fit together).</summary>
        Private Shared Function buildHistogram(ByVal section As ColumnSection) As RenderableComponentHistogram
            Dim buckets() As Double = section.Buckets
            Dim counts() As Long = section.BucketCounts
            If buckets Is Nothing OrElse counts Is Nothing OrElse counts.Length = 0 OrElse buckets.Length < counts.Length + 1 Then Return Nothing
            Dim histBuilder As New RenderableComponentHistogram.Builder()
            For j As Integer = 0 To counts.Length - 1
                histBuilder.addBin(buckets(j), buckets(j + 1), counts(j))
            Next j
            histBuilder.margins(12, 32, 56, 16)
            Dim hist As RenderableComponentHistogram = histBuilder.title(section.HistogramTitle).build()
            hist.timeAxis = section.TimeAxis
            Return hist
        End Function

        Private Shared Sub addStat(ByVal section As ColumnSection, ByVal label As String, ByVal value As String)
            section.Stats.Add(New String() {label, value})
        End Sub

        Private Shared Function describeSequenceLengths(ByVal seqLength As SequenceLengthAnalysisT) As ColumnSection
            Dim section As New ColumnSection()
            section.Id = "sequence-lengths"
            section.Name = "Sequence Lengths"
            section.TypeName = "Sequence length"
            section.Count = FormatCount(seqLength.getTotalNumSequences())
            section.Overview = "length " & FormatCount(seqLength.getMinSeqLength()) & " to " & FormatCount(seqLength.getMaxSeqLength()) & ", mean " & FormatValue(seqLength.getMeanLength())
            addStat(section, "Sequences", FormatCount(seqLength.getTotalNumSequences()))
            addStat(section, "Min length", FormatCount(seqLength.getMinSeqLength()))
            addStat(section, "Max length", FormatCount(seqLength.getMaxSeqLength()))
            addStat(section, "Mean length", FormatValue(seqLength.getMeanLength()))
            addStat(section, "Zero-length sequences", FormatCount(seqLength.getCountZeroLength()))
            addStat(section, "One-step sequences", FormatCount(seqLength.getCountOneLength()))
            section.Buckets = seqLength.getHistogramBuckets()
            section.BucketCounts = seqLength.getHistogramBucketCounts()
            section.HistogramTitle = "Histogram of sequence lengths"
            section.Raw = seqLength.ToString()
            Return section
        End Function

        Private Shared Function describeColumn(ByVal index As Integer, ByVal schema As SchemaT, ByVal ca As ColumnAnalysisT) As ColumnSection
            Dim section As New ColumnSection()
            section.Id = "col-" & index.ToString(CultureInfo.InvariantCulture)
            section.IndexText = index.ToString(CultureInfo.InvariantCulture)
            Dim inSchema As Boolean = schema IsNot Nothing AndAlso index < schema.numColumns()
            section.Name = If(inSchema, schema.getName(index), "Column " & index.ToString(CultureInfo.InvariantCulture))
            If inSchema Then
                section.TypeName = schema.getType(index).ToString()
            ElseIf ca IsNot Nothing Then
                section.TypeName = ca.getColumnType().ToString()
            Else
                section.TypeName = "Unknown"
            End If

            If ca Is Nothing Then
                section.Overview = "No analysis"
                section.Note = "No analysis is available for this column."
                Return section
            End If

            If TypeOf ca Is CategoricalAnalysisT Then
                describeCategorical(section, DirectCast(ca, CategoricalAnalysisT))
                Return section
            End If

            section.Raw = ca.ToString()
            section.Count = FormatCount(ca.getCountTotal())
            If TypeOf ca Is TimeAnalysisT Then
                describeTime(section, DirectCast(ca, TimeAnalysisT))
            ElseIf TypeOf ca Is NumericalColumnAnalysisT Then
                describeNumerical(section, DirectCast(ca, NumericalColumnAnalysisT))
            ElseIf TypeOf ca Is StringAnalysisT Then
                describeString(section, DirectCast(ca, StringAnalysisT))
            ElseIf TypeOf ca Is BytesAnalysisT Then
                describeBytes(section, DirectCast(ca, BytesAnalysisT))
            ElseIf TypeOf ca Is NDArrayAnalysisT Then
                describeNDArray(section, DirectCast(ca, NDArrayAnalysisT))
            Else
                addStat(section, "Count", section.Count)
            End If
            Return section
        End Function

        Private Shared Sub describeNumerical(ByVal section As ColumnSection, ByVal na As NumericalColumnAnalysisT)
            Dim min, max As String
            If TypeOf na Is IntegerAnalysisT Then
                min = FormatCount(DirectCast(na, IntegerAnalysisT).getMin())
                max = FormatCount(DirectCast(na, IntegerAnalysisT).getMax())
            ElseIf TypeOf na Is LongAnalysisT Then
                min = FormatCount(DirectCast(na, LongAnalysisT).getMin())
                max = FormatCount(DirectCast(na, LongAnalysisT).getMax())
            Else
                min = FormatValue(na.MinDouble)
                max = FormatValue(na.MaxDouble)
            End If
            section.Overview = "min " & min & ", max " & max & ", mean " & FormatValue(na.getMean())
            addStat(section, "Count", section.Count)
            addStat(section, "Min", min)
            addStat(section, "Max", max)
            addStat(section, "Mean", FormatValue(na.getMean()))
            addStat(section, "Sample std dev", FormatValue(na.getSampleStdev()))
            addStat(section, "Sample variance", FormatValue(na.getSampleVariance()))
            If TypeOf na Is DoubleAnalysisT Then addStat(section, "NaN values", FormatCount(DirectCast(na, DoubleAnalysisT).getCountNaN()))
            addStat(section, "Zero values", FormatCount(na.getCountZero()))
            addStat(section, "Negative values", FormatCount(na.getCountNegative()))
            addStat(section, "Positive values", FormatCount(na.getCountPositive()))
            addStat(section, "Values equal to min", FormatCount(na.getCountMinValue()))
            addStat(section, "Values equal to max", FormatCount(na.getCountMaxValue()))
            addQuantiles(section, na.getDigest(), Function(v) FormatValue(v))
            section.Buckets = na.getHistogramBuckets()
            section.BucketCounts = na.getHistogramBucketCounts()
            section.HistogramTitle = "Histogram of values"
        End Sub

        Private Shared Sub describeTime(ByVal section As ColumnSection, ByVal ta As TimeAnalysisT)
            section.Overview = FormatEpochMillis(ta.getMin()) & " to " & FormatEpochMillis(ta.getMax())
            addStat(section, "Count", section.Count)
            addStat(section, "Earliest", FormatEpochMillis(ta.getMin()))
            addStat(section, "Latest", FormatEpochMillis(ta.getMax()))
            addStat(section, "Mean", FormatEpochMillis(ta.getMean()))
            addStat(section, "Sample std dev", FormatDuration(ta.getSampleStdev()))
            addStat(section, "Values equal to earliest", FormatCount(ta.getCountMinValue()))
            addStat(section, "Values equal to latest", FormatCount(ta.getCountMaxValue()))
            addQuantiles(section, ta.getDigest(), Function(v) FormatEpochMillis(v))
            section.Buckets = ta.getHistogramBuckets()
            section.BucketCounts = ta.getHistogramBucketCounts()
            section.HistogramTitle = "Histogram of times"
            section.TimeAxis = True
        End Sub

        Private Shared Sub addQuantiles(ByVal section As ColumnSection, ByVal digest As TDigest, ByVal format As Func(Of Double, String))
            If digest Is Nothing OrElse digest.size() = 0 Then Return
            For Each q As Double In QUANTILES
                Dim label As String = "Quantile " & q.ToString("0.###", CultureInfo.InvariantCulture) & If(q = 0.5, " (median)", "")
                addStat(section, label, format(digest.quantile(q)))
            Next
        End Sub

        Private Shared Sub describeString(ByVal section As ColumnSection, ByVal sa As StringAnalysisT)
            section.Overview = "length " & FormatCount(sa.getMinLength()) & " to " & FormatCount(sa.getMaxLength()) & ", mean " & FormatValue(sa.getMeanLength())
            addStat(section, "Count", section.Count)
            addStat(section, "Min length", FormatCount(sa.getMinLength()))
            addStat(section, "Max length", FormatCount(sa.getMaxLength()))
            addStat(section, "Mean length", FormatValue(sa.getMeanLength()))
            addStat(section, "Length std dev", FormatValue(sa.getSampleStdevLength()))
            addStat(section, "Length variance", FormatValue(sa.getSampleVarianceLength()))
            section.Buckets = sa.getHistogramBuckets()
            section.BucketCounts = sa.getHistogramBucketCounts()
            section.HistogramTitle = "Histogram of string lengths"
        End Sub

        Private Shared Sub describeCategorical(ByVal section As ColumnSection, ByVal ca As CategoricalAnalysisT)
            Dim counts As IDictionary(Of String, Long) = If(ca.getMapOfCounts(), New Dictionary(Of String, Long)())
            Dim total As Long = 0
            For Each c As Long In counts.Values
                total += c
            Next
            'Highest count first, as CategoricalAnalysis.ToString orders them (stable for ties)
            Dim ordered As IList(Of KeyValuePair(Of String, Long)) = counts.OrderByDescending(Function(kv) kv.Value).ToList()
            section.Count = FormatCount(total)
            addStat(section, "Count", section.Count)
            addStat(section, "Distinct categories", FormatCount(ordered.Count))
            If ordered.Count > 0 Then
                Dim topShare As String = FormatShare(If(total = 0, Double.NaN, ordered(0).Value / CDbl(total)))
                addStat(section, "Most frequent", ordered(0).Key & " (" & topShare & ")")
                section.Overview = FormatCount(ordered.Count) & If(ordered.Count = 1, " category", " categories") & ", most frequent: " & ordered(0).Key & " (" & topShare & ")"
                Dim shown As Integer = Math.Min(MAX_CATEGORIES_SHOWN, ordered.Count)
                Dim rows As String()() = New String(shown - 1)() {}
                For i As Integer = 0 To shown - 1
                    rows(i) = New String() {ordered(i).Key, FormatCount(ordered(i).Value), FormatShare(If(total = 0, Double.NaN, ordered(i).Value / CDbl(total)))}
                Next
                section.Categories = rows
                If ordered.Count > shown Then
                    Dim rest As Long = 0
                    For i As Integer = shown To ordered.Count - 1
                        rest += ordered(i).Value
                    Next
                    section.CategoriesNote = "and " & FormatCount(ordered.Count - shown) & " more " & If(ordered.Count - shown = 1, "category", "categories") &
                                             " with " & FormatCount(rest) & If(rest = 1, " value", " values") & " in total"
                End If
                section.Raw = ca.ToString()
            Else
                section.Overview = "no categories"
            End If
        End Sub

        Private Shared Sub describeBytes(ByVal section As ColumnSection, ByVal ba As BytesAnalysisT)
            section.Overview = FormatCount(ba.getMinNumBytes()) & " to " & FormatCount(ba.getMaxNumBytes()) & " bytes"
            addStat(section, "Count", section.Count)
            addStat(section, "Null values", FormatCount(ba.getCountNull()))
            addStat(section, "Zero-length values", FormatCount(ba.getCountZeroLength()))
            addStat(section, "Min bytes", FormatCount(ba.getMinNumBytes()))
            addStat(section, "Max bytes", FormatCount(ba.getMaxNumBytes()))
        End Sub

        Private Shared Sub describeNDArray(ByVal section As ColumnSection, ByVal na As NDArrayAnalysisT)
            section.Overview = "values " & FormatValue(na.getMinValue()) & " to " & FormatValue(na.getMaxValue())
            addStat(section, "Count", section.Count)
            addStat(section, "Null values", FormatCount(na.getCountNull()))
            addStat(section, "Min length", FormatCount(na.getMinLength()))
            addStat(section, "Max length", FormatCount(na.getMaxLength()))
            addStat(section, "Total array values", FormatCount(na.getTotalNDArrayValues()))
            addStat(section, "Min value", FormatValue(na.getMinValue()))
            addStat(section, "Max value", FormatValue(na.getMaxValue()))
            Dim byRank As IDictionary(Of Integer, Long) = na.getCountsByRank()
            If byRank IsNot Nothing AndAlso byRank.Count > 0 Then
                addStat(section, "Counts by rank", String.Join(", ", byRank.Keys.OrderBy(Function(k) k).Select(Function(k) "rank " & FormatCount(k) & ": " & FormatCount(byRank(k)))))
            End If
            section.Buckets = na.getHistogramBuckets()
            section.BucketCounts = na.getHistogramBucketCounts()
            section.HistogramTitle = "Histogram of array values"
        End Sub

    End Class

End Namespace
