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

Namespace org.datavec.api.transform.ui

    ''' <summary>
    ''' Shared pieces of the self-contained HTML reports (<see cref="HtmlAnalysis"/>, <see cref="HtmlSequencePlotting"/>
    ''' and the <c>RenderableComponent</c> classes): HTML encoding, culture-invariant number formatting, axis ticks for
    ''' the inline SVG charts, the page shell with its light/dark stylesheet, and the equality helpers of the components.
    ''' <para>The Java reports were FreeMarker templates plus JavaScript from the DL4J UI assets, neither of which is part
    ''' of this port; everything here is plain HTML, CSS and SVG with no scripts and no external resources.</para>
    ''' </summary>
    Friend Module HtmlReportSupport

        Private ReadOnly Invariant As CultureInfo = CultureInfo.InvariantCulture

        ''' <summary>Categorical series colours (light surface), in their fixed order.</summary>
        Friend ReadOnly SeriesColorsLight() As String = {"#2a78d6", "#eb6834", "#1baf7a", "#eda100", "#e87ba4", "#008300", "#4a3aa7", "#e34948"}

        ''' <summary>The same eight hues stepped for the dark surface.</summary>
        Friend ReadOnly SeriesColorsDark() As String = {"#3987e5", "#d95926", "#199e70", "#c98500", "#d55181", "#008300", "#9085e9", "#e66767"}

        ''' <summary>Fallback colours written as SVG presentation attributes; the page stylesheet overrides them.</summary>
        Friend Const InkSecondaryFallback As String = "#52514e"
        Friend Const GridFallback As String = "#e1e0d9"
        Friend Const AxisFallback As String = "#c3c2b7"

        ''' <summary>Approximate width of one character of an 11px axis label, used to keep tick labels apart.</summary>
        Friend Const TickCharWidth As Double = 6.4

        Private Const MinEpochMillis As Long = -62135596800000L
        Private Const MaxEpochMillis As Long = 253402300799999L

        Private ReadOnly NumericCell As New System.Text.RegularExpressions.Regex(
            "^[-+]?(\d{1,3}(,\d{3})+|\d+)(\.\d+)?([eE][-+]?\d+)?%?$", System.Text.RegularExpressions.RegexOptions.CultureInvariant)

        Private ReadOnly SafeColor As New System.Text.RegularExpressions.Regex(
            "^(#[0-9A-Fa-f]{3,8}|[A-Za-z]{3,30}|(rgb|rgba|hsl|hsla)\([0-9.,%\s/]+\))$", System.Text.RegularExpressions.RegexOptions.CultureInvariant)

        ' ---- encoding and formatting ----

        ''' <summary>
        ''' Encodes text for HTML element content and quoted attribute values: <c>&amp; &lt; &gt; " '</c> become entities
        ''' and control characters other than tab, line feed and carriage return become U+FFFD. <c>Nothing</c> gives "".
        ''' </summary>
        Public Function HtmlEncode(ByVal value As String) As String
            If String.IsNullOrEmpty(value) Then Return ""
            Dim sb As New StringBuilder(value.Length + 16)
            For Each c As Char In value
                Select Case c
                    Case "&"c
                        sb.Append("&amp;")
                    Case "<"c
                        sb.Append("&lt;")
                    Case ">"c
                        sb.Append("&gt;")
                    Case """"c
                        sb.Append("&quot;")
                    Case "'"c
                        sb.Append("&#39;")
                    Case ControlChars.Tab, ControlChars.Lf, ControlChars.Cr
                        sb.Append(c)
                    Case Else
                        If AscW(c) < 32 OrElse AscW(c) = 127 Then
                            sb.Append(ChrW(&HFFFD))
                        Else
                            sb.Append(c)
                        End If
                End Select
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Formats a number for display, independent of the current culture: whole numbers with thousands separators
        ''' (<c>1,234</c>), other values with six significant digits (<c>0.333333</c>, <c>1,234.57</c>), very large or
        ''' very small magnitudes in scientific notation (<c>1.5E+20</c>), and <c>NaN</c>, <c>Infinity</c>,
        ''' <c>-Infinity</c> as Java prints them.
        ''' </summary>
        Public Function FormatValue(ByVal value As Double) As String
            If Double.IsNaN(value) Then Return "NaN"
            If Double.IsPositiveInfinity(value) Then Return "Infinity"
            If Double.IsNegativeInfinity(value) Then Return "-Infinity"
            If value = 0 Then Return "0"
            Dim magnitude As Double = Math.Abs(value)
            If magnitude >= 1.0E+15 OrElse magnitude < 1.0E-6 Then Return value.ToString("0.#####E+0", Invariant)
            If value = Math.Truncate(value) Then Return value.ToString("#,##0", Invariant)
            Dim decimals As Integer = Math.Max(0, Math.Min(12, 5 - CInt(Math.Floor(Math.Log10(magnitude)))))
            Dim s As String = If(decimals = 0, value.ToString("#,##0", Invariant), value.ToString("#,##0." & New String("#"c, decimals), Invariant))
            Return If(s = "-0", "0", s)
        End Function

        ''' <summary>Formats a count with thousands separators (<c>1,234,567</c>), independent of the current culture.</summary>
        Public Function FormatCount(ByVal value As Long) As String
            Return value.ToString("#,##0", Invariant)
        End Function

        ''' <summary>Formats a fraction (0..1) as a percentage with one decimal, e.g. <c>12.5%</c>.</summary>
        Public Function FormatShare(ByVal fraction As Double) As String
            If Double.IsNaN(fraction) OrElse Double.IsInfinity(fraction) Then Return "-"
            Return (fraction * 100).ToString("0.0", Invariant) & "%"
        End Function

        ''' <summary>Formats epoch milliseconds as a UTC time, e.g. <c>2016-01-01 00:00:00.000 UTC</c>.</summary>
        Public Function FormatEpochMillis(ByVal millis As Long) As String
            If millis < MinEpochMillis OrElse millis > MaxEpochMillis Then Return FormatCount(millis) & " ms"
            Return DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", Invariant) & " UTC"
        End Function

        ''' <summary>Formats fractional epoch milliseconds (a mean or a quantile) as a UTC time.</summary>
        Public Function FormatEpochMillis(ByVal millis As Double) As String
            If Double.IsNaN(millis) OrElse Double.IsInfinity(millis) OrElse millis < MinEpochMillis OrElse millis > MaxEpochMillis Then Return FormatValue(millis)
            Return FormatEpochMillis(CLng(Math.Round(millis)))
        End Function

        ''' <summary>The current UTC time as shown in the report headers (Java: <c>YYYY-MM-dd HH:mm:ss zzz</c> in UTC).</summary>
        Public Function FormatGeneratedTime() As String
            Return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", Invariant) & " UTC"
        End Function

        ''' <summary>
        ''' A short UTC time label for an axis of epoch milliseconds covering <paramref name="spanMillis"/>: the date for spans
        ''' of three days or more (<c>2024-03-10</c>), else month, day and minutes (<c>03-10 14:05</c>), and seconds with
        ''' milliseconds below one hour (<c>14:05:09.250</c>).
        ''' </summary>
        Public Function FormatTimeTick(ByVal millis As Double, ByVal spanMillis As Double) As String
            If Double.IsNaN(millis) OrElse Double.IsInfinity(millis) OrElse millis < MinEpochMillis OrElse millis > MaxEpochMillis Then Return FormatTick(millis)
            Dim t As DateTime = DateTimeOffset.FromUnixTimeMilliseconds(CLng(Math.Round(millis))).UtcDateTime
            If spanMillis >= 3 * 86400000.0 Then Return t.ToString("yyyy-MM-dd", Invariant)
            If spanMillis >= 3600000.0 Then Return t.ToString("MM-dd HH:mm", Invariant)
            Return t.ToString("HH:mm:ss.fff", Invariant)
        End Function

        ''' <summary>A duration given in milliseconds, in the largest fitting unit: <c>212.2 days</c>, <c>5.5 hours</c>, <c>250 ms</c>.</summary>
        Public Function FormatDuration(ByVal millis As Double) As String
            Dim magnitude As Double = Math.Abs(millis)
            If Double.IsNaN(millis) OrElse Double.IsInfinity(millis) Then Return FormatValue(millis)
            If magnitude >= 86400000.0 Then Return (millis / 86400000.0).ToString("#,##0.#", Invariant) & " days"
            If magnitude >= 3600000.0 Then Return (millis / 3600000.0).ToString("0.#", Invariant) & " hours"
            If magnitude >= 60000.0 Then Return (millis / 60000.0).ToString("0.#", Invariant) & " minutes"
            If magnitude >= 1000.0 Then Return (millis / 1000.0).ToString("0.###", Invariant) & " s"
            Return FormatValue(millis) & " ms"
        End Function

        ''' <summary>Formats an SVG coordinate with at most two decimals and an invariant decimal point.</summary>
        Public Function SvgNum(ByVal value As Double) As String
            Dim s As String = Math.Round(value, 2).ToString("0.##", Invariant)
            Return If(s = "-0", "0", s)
        End Function

        ''' <summary>Formats an axis tick: like <see cref="FormatValue"/>, but millions and above are compacted (<c>2.5M</c>, <c>1B</c>).</summary>
        Public Function FormatTick(ByVal value As Double) As String
            Dim magnitude As Double = Math.Abs(value)
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) OrElse magnitude < 1.0E+6 OrElse magnitude >= 1.0E+15 Then Return FormatValue(value)
            If magnitude >= 1.0E+12 Then Return (value / 1.0E+12).ToString("0.##", Invariant) & "T"
            If magnitude >= 1.0E+9 Then Return (value / 1.0E+9).ToString("0.##", Invariant) & "B"
            Return (value / 1.0E+6).ToString("0.##", Invariant) & "M"
        End Function

        ''' <summary>True when the text of a table cell is a formatted number (it is then right-aligned).</summary>
        Public Function LooksNumeric(ByVal text As String) As Boolean
            Return Not String.IsNullOrEmpty(text) AndAlso NumericCell.IsMatch(text)
        End Function

        ' ---- colours ----

        ''' <summary>
        ''' True for colour values that can be written into a style attribute without changing its meaning: hex colours,
        ''' colour names and numeric <c>rgb()/rgba()/hsl()/hsla()</c>. Anything else is ignored by the renderers.
        ''' </summary>
        Public Function IsSafeCssColor(ByVal color As String) As Boolean
            Return Not String.IsNullOrEmpty(color) AndAlso SafeColor.IsMatch(color)
        End Function

        ''' <summary>Near-black or white text for a hex background colour, whichever contrasts more; <c>Nothing</c> for other colour syntaxes.</summary>
        Public Function ContrastingInk(ByVal color As String) As String
            If String.IsNullOrEmpty(color) OrElse color(0) <> "#"c Then Return Nothing
            Dim hex As String = color.Substring(1)
            If hex.Length = 3 OrElse hex.Length = 4 Then
                hex = New String(New Char() {hex(0), hex(0), hex(1), hex(1), hex(2), hex(2)})
            ElseIf hex.Length = 6 OrElse hex.Length = 8 Then
                hex = hex.Substring(0, 6)
            Else
                Return Nothing
            End If
            Dim rgb(2) As Double
            For i As Integer = 0 To 2
                Dim c As Double = Convert.ToInt32(hex.Substring(i * 2, 2), 16) / 255.0
                rgb(i) = If(c <= 0.03928, c / 12.92, Math.Pow((c + 0.055) / 1.055, 2.4))
            Next
            Dim luminance As Double = 0.2126 * rgb(0) + 0.7152 * rgb(1) + 0.0722 * rgb(2)
            Return If(luminance > 0.179, "#0b0b0b", "#ffffff")
        End Function

        ''' <summary>The 1-based categorical colour slot of the series with the given 0-based index (slots repeat after 8).</summary>
        Public Function SeriesSlot(ByVal seriesIndex As Integer) As Integer
            Return (seriesIndex Mod SeriesColorsLight.Length) + 1
        End Function

        ' ---- axes ----

        ''' <summary>A "nice" tick step (1, 2, 2.5 or 5 times a power of ten) not smaller than <paramref name="roughStep"/>.</summary>
        Public Function NiceStep(ByVal roughStep As Double) As Double
            If Double.IsNaN(roughStep) OrElse Double.IsInfinity(roughStep) OrElse roughStep <= 0 Then Return 1
            Dim power As Double = Math.Pow(10, Math.Floor(Math.Log10(roughStep)))
            Dim fraction As Double = roughStep / power
            Dim nice As Double
            If fraction <= 1 Then
                nice = 1
            ElseIf fraction <= 2 Then
                nice = 2
            ElseIf fraction <= 2.5 Then
                nice = 2.5
            ElseIf fraction <= 5 Then
                nice = 5
            Else
                nice = 10
            End If
            Return nice * power
        End Function

        ''' <summary>The multiples of <paramref name="tickStep"/> between <paramref name="low"/> and <paramref name="high"/> (inclusive).</summary>
        Public Function TicksBetween(ByVal low As Double, ByVal high As Double, ByVal tickStep As Double) As IList(Of Double)
            Dim ticks As New List(Of Double)()
            If tickStep <= 0 OrElse high < low Then Return ticks
            Dim first As Long = CLng(Math.Ceiling(low / tickStep - 1.0E-9))
            Dim last As Long = CLng(Math.Floor(high / tickStep + 1.0E-9))
            If last - first > 1000 Then Return ticks
            For k As Long = first To last
                ticks.Add(k * tickStep)
            Next
            Return ticks
        End Function

        ''' <summary>
        ''' Ticks for a horizontal axis spanning <paramref name="low"/>..<paramref name="high"/> over
        ''' <paramref name="widthPx"/> pixels: as many as fit (at most eight) without the labels touching.
        ''' </summary>
        Public Function HorizontalTicks(ByVal low As Double, ByVal high As Double, ByVal widthPx As Double, Optional ByVal label As Func(Of Double, String) = Nothing) As IList(Of Double)
            Dim format As Func(Of Double, String) = If(label, New Func(Of Double, String)(AddressOf FormatTick))
            Dim ticks As IList(Of Double) = New List(Of Double)()
            For count As Integer = 8 To 2 Step -1
                ticks = TicksBetween(low, high, NiceStep((high - low) / count))
                Dim needed As Double = ticks.Sum(Function(t) format(t).Length * TickCharWidth + 12)
                If needed <= widthPx Then Return ticks
            Next
            Return ticks
        End Function

        ''' <summary>Appends horizontal gridlines and their right-aligned labels (<see cref="FormatTick"/> unless <paramref name="label"/> is given) for the given y ticks.</summary>
        Public Sub AppendYGrid(ByVal sb As StringBuilder, ByVal ticks As IList(Of Double), ByVal scaleY As Func(Of Double, Double), ByVal left As Double, ByVal right As Double, Optional ByVal label As Func(Of Double, String) = Nothing)
            Dim format As Func(Of Double, String) = If(label, New Func(Of Double, String)(AddressOf FormatTick))
            sb.Append("<g class=""dv-grid"" stroke=""").Append(GridFallback).Append(""" stroke-width=""1"">")
            For Each t As Double In ticks
                Dim y As String = SvgNum(scaleY(t))
                sb.Append("<line x1=""").Append(SvgNum(left)).Append(""" x2=""").Append(SvgNum(right)).Append(""" y1=""").Append(y).Append(""" y2=""").Append(y).Append("""/>")
            Next
            sb.Append("</g>")
            sb.Append("<g class=""dv-ticks"" fill=""").Append(InkSecondaryFallback).Append(""" font-size=""11"" text-anchor=""end"">")
            For Each t As Double In ticks
                sb.Append("<text x=""").Append(SvgNum(left - 8)).Append(""" y=""").Append(SvgNum(scaleY(t))).Append(""" dy=""0.32em"">").Append(HtmlEncode(format(t))).Append("</text>")
            Next
            sb.Append("</g>")
        End Sub

        ''' <summary>Appends the labels of a horizontal axis below the plot area (<see cref="FormatTick"/> unless <paramref name="label"/> is given).</summary>
        Public Sub AppendXLabels(ByVal sb As StringBuilder, ByVal ticks As IList(Of Double), ByVal scaleX As Func(Of Double, Double), ByVal baselineY As Double, Optional ByVal label As Func(Of Double, String) = Nothing)
            Dim format As Func(Of Double, String) = If(label, New Func(Of Double, String)(AddressOf FormatTick))
            sb.Append("<g class=""dv-ticks"" fill=""").Append(InkSecondaryFallback).Append(""" font-size=""11"" text-anchor=""middle"">")
            For Each t As Double In ticks
                sb.Append("<text x=""").Append(SvgNum(scaleX(t))).Append(""" y=""").Append(SvgNum(baselineY + 18)).Append(""">").Append(HtmlEncode(format(t))).Append("</text>")
            Next
            sb.Append("</g>")
        End Sub

        ''' <summary>Appends the baseline of the plot area.</summary>
        Public Sub AppendBaseline(ByVal sb As StringBuilder, ByVal left As Double, ByVal right As Double, ByVal y As Double)
            sb.Append("<line class=""dv-axis"" stroke=""").Append(AxisFallback).Append(""" stroke-width=""1"" x1=""").Append(SvgNum(left)).Append(""" x2=""").Append(SvgNum(right)).
                Append(""" y1=""").Append(SvgNum(y)).Append(""" y2=""").Append(SvgNum(y)).Append("""/>")
        End Sub

        ' ---- page ----

        ''' <summary>
        ''' Wraps report content in a complete HTML document with the inline stylesheet. <paramref name="title"/> is
        ''' encoded here; <paramref name="metaHtml"/> and <paramref name="bodyHtml"/> must already be HTML.
        ''' </summary>
        Public Function BuildPage(ByVal title As String, ByVal metaHtml As String, ByVal bodyHtml As String) As String
            Dim sb As New StringBuilder(bodyHtml.Length + 8192)
            sb.Append("<!DOCTYPE html>").Append(ControlChars.Lf)
            sb.Append("<html lang=""en"">").Append(ControlChars.Lf)
            sb.Append("<head>").Append(ControlChars.Lf)
            sb.Append("<meta charset=""utf-8"">").Append(ControlChars.Lf)
            sb.Append("<meta name=""viewport"" content=""width=device-width, initial-scale=1"">").Append(ControlChars.Lf)
            sb.Append("<meta name=""color-scheme"" content=""light dark"">").Append(ControlChars.Lf)
            sb.Append("<meta name=""generator"" content=""DeepLearning4VB DataVec"">").Append(ControlChars.Lf)
            sb.Append("<title>").Append(HtmlEncode(title)).Append("</title>").Append(ControlChars.Lf)
            sb.Append("<style>").Append(ControlChars.Lf).Append(StyleSheet()).Append("</style>").Append(ControlChars.Lf)
            sb.Append("</head>").Append(ControlChars.Lf)
            sb.Append("<body>").Append(ControlChars.Lf)
            sb.Append("<main class=""dv-page"">").Append(ControlChars.Lf)
            sb.Append("<header class=""dv-header"">").Append(ControlChars.Lf)
            sb.Append("<h1>").Append(HtmlEncode(title)).Append("</h1>").Append(ControlChars.Lf)
            sb.Append("<p class=""dv-meta"">").Append(metaHtml).Append("</p>").Append(ControlChars.Lf)
            sb.Append("</header>").Append(ControlChars.Lf)
            sb.Append(bodyHtml)
            sb.Append("<footer class=""dv-footer"">Generated by DeepLearning4VB DataVec.</footer>").Append(ControlChars.Lf)
            sb.Append("</main>").Append(ControlChars.Lf)
            sb.Append("</body>").Append(ControlChars.Lf)
            sb.Append("</html>").Append(ControlChars.Lf)
            Return sb.ToString()
        End Function

        ''' <summary>The report stylesheet: colour tokens for light and dark (OS setting or a <c>data-theme</c> attribute), layout, tables and charts.</summary>
        Private Function StyleSheet() As String
            Dim light As New StringBuilder()
            light.Append("color-scheme:light;--dv-page:#f9f9f7;--dv-surface:#fcfcfb;--dv-ink:#0b0b0b;--dv-ink-2:#52514e;--dv-muted:#6f6d68;")
            light.Append("--dv-grid:#e1e0d9;--dv-axis:#c3c2b7;--dv-ring:rgba(11,11,11,.10);--dv-head:#f1f0ec;--dv-hover:#1c5cab;--dv-wash:rgba(42,120,214,.08);")
            Dim dark As New StringBuilder()
            dark.Append("color-scheme:dark;--dv-page:#0d0d0d;--dv-surface:#1a1a19;--dv-ink:#ffffff;--dv-ink-2:#c3c2b7;--dv-muted:#9a988f;")
            dark.Append("--dv-grid:#2c2c2a;--dv-axis:#4a4a46;--dv-ring:rgba(255,255,255,.10);--dv-head:#242423;--dv-hover:#86b6ef;--dv-wash:rgba(57,135,229,.14);")
            For i As Integer = 0 To SeriesColorsLight.Length - 1
                light.Append("--dv-s").Append(i + 1).Append(":"c).Append(SeriesColorsLight(i)).Append(";"c)
                dark.Append("--dv-s").Append(i + 1).Append(":"c).Append(SeriesColorsDark(i)).Append(";"c)
            Next

            Dim css As New StringBuilder()
            Dim lf As String = ControlChars.Lf
            css.Append(":root{").Append(light).Append("}").Append(lf)
            css.Append("@media (prefers-color-scheme: dark){:root:not([data-theme=""light""]){").Append(dark).Append("}}").Append(lf)
            css.Append(":root[data-theme=""dark""]{").Append(dark).Append("}").Append(lf)
            css.Append("*{box-sizing:border-box}").Append(lf)
            css.Append("body{margin:0;background:var(--dv-page);color:var(--dv-ink);font:14px/1.5 system-ui,-apple-system,""Segoe UI"",Roboto,sans-serif}").Append(lf)
            css.Append(".dv-page{max-width:1120px;margin:0 auto;padding:24px 16px 48px}").Append(lf)
            css.Append(".dv-header h1{font-size:24px;line-height:1.25;font-weight:600;margin:0 0 4px;overflow-wrap:anywhere}").Append(lf)
            css.Append(".dv-meta{margin:0;color:var(--dv-ink-2)}").Append(lf)
            css.Append("h2{font-size:17px;font-weight:600;margin:32px 0 12px}").Append(lf)
            css.Append(".dv-card{background:var(--dv-surface);border:1px solid var(--dv-ring);border-radius:8px;padding:16px;margin:0 0 16px;min-width:0}").Append(lf)
            css.Append(".dv-card-head{display:flex;flex-wrap:wrap;align-items:baseline;gap:4px 10px;margin:0 0 12px}").Append(lf)
            css.Append(".dv-card-head h3{font-size:16px;font-weight:600;margin:0;overflow-wrap:anywhere}").Append(lf)
            css.Append(".dv-index{color:var(--dv-ink-2);font-variant-numeric:tabular-nums}").Append(lf)
            css.Append(".dv-badge{font-size:12px;line-height:18px;padding:0 8px;border-radius:999px;border:1px solid var(--dv-axis);color:var(--dv-ink-2)}").Append(lf)
            css.Append(".dv-card-body{display:flex;flex-wrap:wrap;gap:16px 32px;align-items:flex-start}").Append(lf)
            css.Append(".dv-card-stats{flex:1 1 260px;min-width:0;max-width:560px}").Append(lf)
            css.Append(".dv-card-chart{flex:2 1 380px;min-width:0}").Append(lf)
            css.Append(".dv-table-wrap{overflow-x:auto;max-width:100%}").Append(lf)
            css.Append(".dv-table{border-collapse:collapse;width:100%;font-variant-numeric:tabular-nums}").Append(lf)
            css.Append(".dv-table caption{text-align:left;font-weight:600;padding:0 0 6px}").Append(lf)
            css.Append(".dv-table th,.dv-table td{padding:5px 10px;text-align:left;vertical-align:top;border-bottom:1px solid var(--dv-grid);overflow-wrap:break-word}").Append(lf)
            css.Append(".dv-table thead th{background:var(--dv-head);font-weight:600;border-bottom-color:var(--dv-axis)}").Append(lf)
            css.Append(".dv-table .dv-num{text-align:right;white-space:nowrap}").Append(lf)
            css.Append(".dv-summary{min-width:600px}.dv-schema{min-width:560px}").Append(lf)
            css.Append(".dv-stats td:first-child{color:var(--dv-ink-2)}").Append(lf)
            css.Append(".dv-stats td:last-child{text-align:right}").Append(lf)
            css.Append(".dv-compact th,.dv-compact td{padding:3px 8px;font-size:13px}").Append(lf)
            css.Append(".dv-categories{margin-top:16px}").Append(lf)
            css.Append(".dv-figure{margin:0;min-width:0}").Append(lf)
            css.Append(".dv-figure figcaption{font-weight:600;margin:0 0 6px;overflow-wrap:anywhere}").Append(lf)
            css.Append(".dv-chart{display:block;width:100%;max-width:100%;height:auto;overflow:visible}").Append(lf)
            css.Append(".dv-chart .dv-ticks{fill:var(--dv-ink-2)}").Append(lf)
            css.Append(".dv-chart .dv-grid{stroke:var(--dv-grid)}").Append(lf)
            css.Append(".dv-chart .dv-axis{stroke:var(--dv-axis)}").Append(lf)
            css.Append(".dv-chart .dv-empty{fill:var(--dv-ink-2)}").Append(lf)
            css.Append(".dv-bar{fill:var(--dv-s1)}").Append(lf)
            css.Append(".dv-hit{fill:transparent}").Append(lf)
            css.Append(".dv-bin:hover .dv-bar{fill:var(--dv-hover)}").Append(lf)
            css.Append(".dv-bin:hover .dv-hit{fill:var(--dv-wash)}").Append(lf)
            For i As Integer = 1 To SeriesColorsLight.Length
                css.Append(".dv-c").Append(i).Append("{--dv-c:var(--dv-s").Append(i).Append(")}")
            Next
            css.Append(lf)
            css.Append(".dv-line{fill:none;stroke:var(--dv-c,var(--dv-s1));stroke-width:2;stroke-linejoin:round;stroke-linecap:round}").Append(lf)
            css.Append(".dv-dot{fill:var(--dv-c,var(--dv-s1));stroke:var(--dv-surface);stroke-width:2}").Append(lf)
            css.Append(".dv-point:hover .dv-hit{fill:var(--dv-wash)}").Append(lf)
            css.Append(".dv-legend{list-style:none;display:flex;flex-wrap:wrap;gap:4px 16px;margin:0 0 6px;padding:0;color:var(--dv-ink-2);font-size:13px}").Append(lf)
            css.Append(".dv-legend li{display:flex;align-items:center;gap:6px;overflow-wrap:anywhere}").Append(lf)
            css.Append(".dv-legend svg{flex:none}").Append(lf)
            css.Append("details{margin-top:12px}").Append(lf)
            css.Append("details summary{cursor:pointer;color:var(--dv-ink-2);font-size:13px}").Append(lf)
            css.Append("details .dv-table-wrap{margin-top:8px}").Append(lf)
            css.Append(".dv-raw pre{margin:8px 0 0;white-space:pre-wrap;overflow-wrap:anywhere;font:12px/1.5 ui-monospace,SFMono-Regular,Consolas,monospace;color:var(--dv-ink-2)}").Append(lf)
            css.Append(".dv-note{color:var(--dv-ink-2);margin:8px 0 0}").Append(lf)
            css.Append(".dv-footer{margin-top:32px;color:var(--dv-muted);font-size:12px}").Append(lf)
            css.Append("@media print{body{background:#fff}.dv-card{break-inside:avoid}}").Append(lf)
            Return css.ToString()
        End Function

        ''' <summary>
        ''' Writes a report as UTF-8 without a byte order mark, creating the parent directory if needed (as Apache
        ''' Commons IO's <c>FileUtils.writeStringToFile</c> did).
        ''' </summary>
        Public Sub WriteReport(ByVal path As String, ByVal content As String)
            If String.IsNullOrEmpty(path) Then Throw New ArgumentException("Output path must not be empty", NameOf(path))
            Dim directory As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))
            If Not String.IsNullOrEmpty(directory) Then System.IO.Directory.CreateDirectory(directory)
            System.IO.File.WriteAllText(path, content, New UTF8Encoding(False))
        End Sub

        ' ---- equality helpers of the components (Lombok @Data semantics) ----

        ''' <summary>Java <c>List.equals</c>: both <c>Nothing</c>, or the same elements in the same order.</summary>
        Public Function ListEquals(Of T)(ByVal a As IList(Of T), ByVal b As IList(Of T)) As Boolean
            If a Is b Then Return True
            If a Is Nothing OrElse b Is Nothing Then Return False
            Return a.SequenceEqual(b)
        End Function

        ''' <summary>Java <c>List.hashCode</c>.</summary>
        Public Function ListHash(Of T)(ByVal a As IList(Of T)) As Integer
            If a Is Nothing Then Return 0
            Return Arrays.hashCode(a.ToArray())
        End Function

        ''' <summary>Element-wise equality of two lists of arrays (array contents compared, not references).</summary>
        Public Function ArrayListEquals(Of T)(ByVal a As IList(Of T()), ByVal b As IList(Of T())) As Boolean
            If a Is b Then Return True
            If a Is Nothing OrElse b Is Nothing OrElse a.Count <> b.Count Then Return False
            For i As Integer = 0 To a.Count - 1
                If Not Arrays.equals(a(i), b(i)) Then Return False
            Next
            Return True
        End Function

        ''' <summary>Hash code of a list of arrays, consistent with <see cref="ArrayListEquals"/>.</summary>
        Public Function ArrayListHash(Of T)(ByVal a As IList(Of T())) As Integer
            If a Is Nothing Then Return 0
            Dim h As Integer = 1
            For Each element As T() In a
                h = Arrays.WrapToInt32(31L * h + Arrays.hashCode(element))
            Next
            Return h
        End Function

        ''' <summary>Renders a list of arrays Java-style, e.g. <c>[[0.0, 1.0], [2.0]]</c>.</summary>
        Public Function ArrayListToString(Of T)(ByVal a As IList(Of T())) As String
            If a Is Nothing Then Return "null"
            Return "[" & String.Join(", ", a.Select(Function(e) Arrays.toString(e))) & "]"
        End Function

        ''' <summary>Renders a list Java-style, e.g. <c>[0.0, 1.0]</c>.</summary>
        Public Function ListToString(Of T)(ByVal a As IList(Of T)) As String
            If a Is Nothing Then Return "null"
            Return Arrays.toString(a.ToArray())
        End Function

    End Module

End Namespace
