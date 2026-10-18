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
Imports System.Linq
Imports System.Text

Namespace org.datavec.api.transform.ui.components

    ''' <summary>
    ''' A line chart of one or more series (x and y arrays of equal length), rendered as inline SVG. The margins are the
    ''' space in SVG units around a fixed 640 x 220 plot area (the title is an HTML caption above the chart). Non-finite
    ''' y values leave a gap in the line. A legend is shown for two or more series when <c>legend</c> is on; with
    ''' <c>removeAxisHorizontal</c> the x axis labels are omitted. Hovering a point shows its values (up to 500 points
    ''' per series); points are marked when a series has at most 30 of them.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    Public Class RenderableComponentLineChart
        Inherits RenderableComponent

        Public Const COMPONENT_TYPE As String = "linechart"

        Private Const PLOT_WIDTH As Double = 640
        Private Const PLOT_HEIGHT As Double = 220
        Private Const MAX_MARKED_POINTS As Integer = 30
        Private Const MAX_HOVER_POINTS As Integer = 500

        Private title As String
        Private x As IList(Of Double())
        Private y As IList(Of Double())
        Private seriesNames As IList(Of String)
        Private removeAxisHorizontal As Boolean
        Private marginTop As Integer
        Private marginBottom As Integer
        Private marginLeft As Integer
        Private marginRight As Integer
        Private legend As Boolean

        ''' <summary>
        ''' Report-internal rendering hint (not part of the Java data): the y values are epoch milliseconds, so the axis and
        ''' the tooltips show UTC times. Not included in equality.
        ''' </summary>
        Friend timeValues As Boolean

        Private Sub New(ByVal builder As Builder)
            MyBase.New(COMPONENT_TYPE)
            title = builder.titleValue
            x = New List(Of Double())(builder.xValue)
            y = New List(Of Double())(builder.yValue)
            seriesNames = New List(Of String)(builder.seriesNamesValue)
            Me.removeAxisHorizontal = builder.removeAxisHorizontalValue
            Me.marginTop = builder.marginTopValue
            Me.marginBottom = builder.marginBottomValue
            Me.marginLeft = builder.marginLeftValue
            Me.marginRight = builder.marginRightValue
            Me.legend = builder.legendValue
        End Sub

        Public Sub New()
            MyBase.New(COMPONENT_TYPE)
            'no-arg constructor for Jackson
        End Sub

        Public Overrides Function toHtml() As String
            Dim top As Double = Math.Max(0, marginTop)
            Dim left As Double = Math.Max(0, marginLeft)
            Dim width As Double = left + PLOT_WIDTH + Math.Max(0, marginRight)
            Dim height As Double = top + PLOT_HEIGHT + Math.Max(0, marginBottom)
            Dim plotRight As Double = left + PLOT_WIDTH
            Dim plotBottom As Double = top + PLOT_HEIGHT
            Dim seriesCount As Integer = If(x Is Nothing OrElse y Is Nothing, 0, Math.Min(x.Count, y.Count))

            Dim xLow As Double = Double.PositiveInfinity
            Dim xHigh As Double = Double.NegativeInfinity
            Dim yLow As Double = Double.PositiveInfinity
            Dim yHigh As Double = Double.NegativeInfinity
            Dim integralValues As Boolean = True
            For s As Integer = 0 To seriesCount - 1
                Dim xs As Double() = x(s)
                Dim ys As Double() = y(s)
                If xs Is Nothing OrElse ys Is Nothing Then Continue For
                For i As Integer = 0 To Math.Min(xs.Length, ys.Length) - 1
                    If isFinite(xs(i)) AndAlso isFinite(ys(i)) Then
                        xLow = Math.Min(xLow, xs(i))
                        xHigh = Math.Max(xHigh, xs(i))
                        yLow = Math.Min(yLow, ys(i))
                        yHigh = Math.Max(yHigh, ys(i))
                        If ys(i) <> Math.Truncate(ys(i)) Then integralValues = False
                    End If
                Next
            Next
            Dim hasData As Boolean = xLow <= xHigh

            Dim sb As New StringBuilder()
            sb.Append("<figure class=""dv-figure dv-linechart"">")
            If Not String.IsNullOrEmpty(title) Then sb.Append("<figcaption>").Append(HtmlEncode(title)).Append("</figcaption>")
            If legend AndAlso seriesCount >= 2 Then
                sb.Append("<ul class=""dv-legend"">")
                For s As Integer = 0 To seriesCount - 1
                    Dim slot As Integer = SeriesSlot(s)
                    sb.Append("<li><svg width=""18"" height=""8"" aria-hidden=""true""><line class=""dv-line dv-c").Append(slot).Append(""" stroke=""").Append(SeriesColorsLight(slot - 1)).
                        Append(""" stroke-width=""2""").Append(dashAttribute(s)).Append(" x1=""1"" x2=""17"" y1=""4"" y2=""4""/></svg>").Append(HtmlEncode(seriesName(s))).Append("</li>")
                Next
                sb.Append("</ul>")
            End If
            sb.Append("<svg class=""dv-chart"" viewBox=""0 0 ").Append(SvgNum(width)).Append(" "c).Append(SvgNum(height)).Append(""" width=""").Append(SvgNum(width)).
                Append(""" height=""").Append(SvgNum(height)).Append(""" role=""img"" aria-label=""").
                Append(HtmlEncode("Line chart" & If(String.IsNullOrEmpty(title), "", ": " & title))).Append(""">")

            If Not hasData Then
                AppendBaseline(sb, left, plotRight, plotBottom)
                sb.Append("<text class=""dv-empty"" fill=""").Append(InkSecondaryFallback).Append(""" font-size=""13"" text-anchor=""middle"" x=""").Append(SvgNum(left + PLOT_WIDTH / 2)).
                    Append(""" y=""").Append(SvgNum(top + PLOT_HEIGHT / 2)).Append(""">No data</text></svg></figure>")
                Return sb.ToString()
            End If

            If xHigh = xLow Then
                xLow -= 0.5
                xHigh += 0.5
            End If
            If yHigh = yLow Then
                yLow -= 1
                yHigh += 1
            End If
            Dim yStep As Double = NiceStep((yHigh - yLow) / 4)
            If integralValues Then yStep = Math.Max(1, yStep)
            yLow = Math.Floor(yLow / yStep + 1.0E-9) * yStep
            yHigh = Math.Ceiling(yHigh / yStep - 1.0E-9) * yStep
            Dim x0 As Double = xLow
            Dim xSpan As Double = xHigh - xLow
            Dim y0 As Double = yLow
            Dim ySpan As Double = yHigh - yLow
            Dim scaleX As Func(Of Double, Double) = Function(v) left + (v - x0) / xSpan * PLOT_WIDTH
            Dim scaleY As Func(Of Double, Double) = Function(v) plotBottom - (v - y0) / ySpan * PLOT_HEIGHT
            Dim yText As Func(Of Double, String)
            Dim yTickText As Func(Of Double, String) = Nothing
            If timeValues Then
                yText = Function(v) FormatEpochMillis(v)
                yTickText = Function(v) FormatTimeTick(v, ySpan)
            Else
                yText = Function(v) FormatValue(v)
            End If

            AppendYGrid(sb, TicksBetween(yLow, yHigh, yStep), scaleY, left, plotRight, yTickText)
            AppendBaseline(sb, left, plotRight, plotBottom)
            If Not removeAxisHorizontal Then AppendXLabels(sb, HorizontalTicks(xLow, xHigh, PLOT_WIDTH), scaleX, plotBottom)

            For s As Integer = 0 To seriesCount - 1
                Dim xs As Double() = x(s)
                Dim ys As Double() = y(s)
                If xs Is Nothing OrElse ys Is Nothing Then Continue For
                Dim count As Integer = Math.Min(xs.Length, ys.Length)
                Dim slot As Integer = SeriesSlot(s)
                Dim color As String = SeriesColorsLight(slot - 1)
                sb.Append("<g class=""dv-series dv-c").Append(slot).Append(""">")

                ' The line, split into segments at non-finite values
                Dim segment As New StringBuilder()
                Dim segmentPoints As Integer = 0
                For i As Integer = 0 To count
                    Dim finitePoint As Boolean = i < count AndAlso isFinite(xs(i)) AndAlso isFinite(ys(i))
                    If finitePoint Then
                        If segmentPoints > 0 Then segment.Append(" "c)
                        segment.Append(SvgNum(scaleX(xs(i)))).Append(","c).Append(SvgNum(scaleY(ys(i))))
                        segmentPoints += 1
                    ElseIf segmentPoints > 0 Then
                        If segmentPoints = 1 Then
                            ' A lone point between gaps: draw it as a dot so it is not lost
                            Dim xy As String() = segment.ToString().Split(","c)
                            sb.Append("<circle class=""dv-dot"" fill=""").Append(color).Append(""" r=""3"" cx=""").Append(xy(0)).Append(""" cy=""").Append(xy(1)).Append("""/>")
                        Else
                            sb.Append("<polyline class=""dv-line"" fill=""none"" stroke=""").Append(color).Append(""" stroke-width=""2"" stroke-linejoin=""round"" stroke-linecap=""round""").
                                Append(dashAttribute(s)).Append(" points=""").Append(segment).Append("""/>")
                        End If
                        segment.Clear()
                        segmentPoints = 0
                    End If
                Next

                If count <= MAX_HOVER_POINTS Then
                    Dim marked As Boolean = count <= MAX_MARKED_POINTS
                    For i As Integer = 0 To count - 1
                        If Not isFinite(xs(i)) OrElse Not isFinite(ys(i)) Then Continue For
                        Dim cx As String = SvgNum(scaleX(xs(i)))
                        Dim cy As String = SvgNum(scaleY(ys(i)))
                        sb.Append("<g class=""dv-point""><title>").Append(HtmlEncode(seriesName(s) & ": x = " & FormatValue(xs(i)) & ", y = " & yText(ys(i)))).Append("</title>")
                        sb.Append("<circle class=""dv-hit"" fill=""transparent"" r=""10"" cx=""").Append(cx).Append(""" cy=""").Append(cy).Append("""/>")
                        If marked Then
                            sb.Append("<circle class=""dv-dot"" fill=""").Append(color).Append(""" stroke=""#fcfcfb"" stroke-width=""2"" r=""4"" cx=""").Append(cx).Append(""" cy=""").Append(cy).Append("""/>")
                        End If
                        sb.Append("</g>")
                    Next
                End If
                sb.Append("</g>")
            Next
            sb.Append("</svg></figure>")
            Return sb.ToString()
        End Function

        Private Function seriesName(ByVal index As Integer) As String
            If seriesNames IsNot Nothing AndAlso index < seriesNames.Count AndAlso seriesNames(index) IsNot Nothing Then Return seriesNames(index)
            Return "Series " & (index + 1)
        End Function

        ''' <summary>Series past the eighth reuse the colours with a dashed line, so no two series look the same.</summary>
        Private Shared Function dashAttribute(ByVal seriesIndex As Integer) As String
            Return If(seriesIndex >= SeriesColorsLight.Length, " stroke-dasharray=""6 3""", "")
        End Function

        Private Shared Function isFinite(ByVal d As Double) As Boolean
            Return Not Double.IsNaN(d) AndAlso Not Double.IsInfinity(d)
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getTitle() As String
            Return title
        End Function

        Public Sub setTitle(ByVal title As String)
            Me.title = title
        End Sub

        Public Function getX() As IList(Of Double())
            Return x
        End Function

        Public Sub setX(ByVal x As IList(Of Double()))
            Me.x = x
        End Sub

        Public Function getY() As IList(Of Double())
            Return y
        End Function

        Public Sub setY(ByVal y As IList(Of Double()))
            Me.y = y
        End Sub

        Public Function getSeriesNames() As IList(Of String)
            Return seriesNames
        End Function

        Public Sub setSeriesNames(ByVal seriesNames As IList(Of String))
            Me.seriesNames = seriesNames
        End Sub

        Public Function isRemoveAxisHorizontal() As Boolean
            Return removeAxisHorizontal
        End Function

        Public Sub setRemoveAxisHorizontal(ByVal removeAxisHorizontal As Boolean)
            Me.removeAxisHorizontal = removeAxisHorizontal
        End Sub

        Public Function getMarginTop() As Integer
            Return marginTop
        End Function

        Public Sub setMarginTop(ByVal marginTop As Integer)
            Me.marginTop = marginTop
        End Sub

        Public Function getMarginBottom() As Integer
            Return marginBottom
        End Function

        Public Sub setMarginBottom(ByVal marginBottom As Integer)
            Me.marginBottom = marginBottom
        End Sub

        Public Function getMarginLeft() As Integer
            Return marginLeft
        End Function

        Public Sub setMarginLeft(ByVal marginLeft As Integer)
            Me.marginLeft = marginLeft
        End Sub

        Public Function getMarginRight() As Integer
            Return marginRight
        End Function

        Public Sub setMarginRight(ByVal marginRight As Integer)
            Me.marginRight = marginRight
        End Sub

        Public Function isLegend() As Boolean
            Return legend
        End Function

        Public Sub setLegend(ByVal legend As Boolean)
            Me.legend = legend
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As RenderableComponentLineChart = DirectCast(obj, RenderableComponentLineChart)
            Return String.Equals(title, other.title) AndAlso ArrayListEquals(x, other.x) AndAlso ArrayListEquals(y, other.y) AndAlso
                   ListEquals(seriesNames, other.seriesNames) AndAlso removeAxisHorizontal = other.removeAxisHorizontal AndAlso
                   marginTop = other.marginTop AndAlso marginBottom = other.marginBottom AndAlso marginLeft = other.marginLeft AndAlso
                   marginRight = other.marginRight AndAlso legend = other.legend
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), title, ArrayListHash(x), ArrayListHash(y), ListHash(seriesNames), removeAxisHorizontal,
                                marginTop, marginBottom, marginLeft, marginRight, legend)
        End Function

        Public Overrides Function ToString() As String
            Return "RenderableComponentLineChart(title=" & Objects.toString(title) & ", x=" & ArrayListToString(x) & ", y=" & ArrayListToString(y) &
                   ", seriesNames=" & ListToString(seriesNames) & ", removeAxisHorizontal=" & Objects.toString(removeAxisHorizontal) &
                   ", marginTop=" & marginTop & ", marginBottom=" & marginBottom & ", marginLeft=" & marginLeft & ", marginRight=" & marginRight &
                   ", legend=" & Objects.toString(legend) & ")"
        End Function

        ''' <summary>
        ''' Fluent builder of a <see cref="RenderableComponentLineChart"/>; margins default to top 60, bottom 60, left 60,
        ''' right 20, and the legend is on.
        ''' </summary>
        Public Class Builder

            Friend titleValue As String
            Friend xValue As IList(Of Double()) = New List(Of Double())()
            Friend yValue As IList(Of Double()) = New List(Of Double())()
            Friend seriesNamesValue As IList(Of String) = New List(Of String)()
            Friend removeAxisHorizontalValue As Boolean = False
            Friend legendValue As Boolean = True

            Friend marginTopValue As Integer = 60
            Friend marginBottomValue As Integer = 60
            Friend marginLeftValue As Integer = 60
            Friend marginRightValue As Integer = 20

            Public Overridable Function title(ByVal value As String) As Builder
                Me.titleValue = value
                Return Me
            End Function

            ''' <summary>Adds a series; <paramref name="xValues"/> and <paramref name="yValues"/> are paired by index.</summary>
            Public Overridable Function addSeries(ByVal seriesName As String, ByVal xValues() As Double, ByVal yValues() As Double) As Builder
                xValue.Add(xValues)
                yValue.Add(yValues)
                seriesNamesValue.Add(seriesName)
                Return Me
            End Function

            ''' <summary>When true the x axis labels are not drawn.</summary>
            Public Overridable Function setRemoveAxisHorizontal(ByVal removeAxisHorizontal As Boolean) As Builder
                Me.removeAxisHorizontalValue = removeAxisHorizontal
                Return Me
            End Function

            Public Overridable Function margins(ByVal top As Integer, ByVal bottom As Integer, ByVal left As Integer, ByVal right As Integer) As Builder
                Me.marginTopValue = top
                Me.marginBottomValue = bottom
                Me.marginLeftValue = left
                Me.marginRightValue = right
                Return Me
            End Function

            Public Overridable Function legend(ByVal value As Boolean) As Builder
                Me.legendValue = value
                Return Me
            End Function

            Public Overridable Function build() As RenderableComponentLineChart
                Return New RenderableComponentLineChart(Me)
            End Function

        End Class

    End Class

End Namespace
