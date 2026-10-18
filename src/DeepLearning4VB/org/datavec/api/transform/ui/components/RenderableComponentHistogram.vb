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
    ''' A histogram: bins given by lower and upper bound with a value (usually a count) each, rendered as an inline SVG
    ''' column chart with a collapsible data table. The margins are the space in SVG units around a fixed 480 x 200
    ''' plot area (the title is an HTML caption above the chart). Hovering a bin shows its range and value.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    Public Class RenderableComponentHistogram
        Inherits RenderableComponent

        Public Const COMPONENT_TYPE As String = "histogram"

        Private Const PLOT_WIDTH As Double = 480
        Private Const PLOT_HEIGHT As Double = 200

        Private title As String
        Private lowerBounds As IList(Of Double) = New List(Of Double)()
        Private upperBounds As IList(Of Double) = New List(Of Double)()
        Private yValues As IList(Of Double) = New List(Of Double)()
        Private marginTop As Integer
        Private marginBottom As Integer
        Private marginLeft As Integer
        Private marginRight As Integer

        ''' <summary>
        ''' Report-internal rendering hint (not part of the Java data): the bounds are epoch milliseconds, so the axis and
        ''' the tooltips show UTC times. Not included in equality.
        ''' </summary>
        Friend timeAxis As Boolean

        Public Sub New(ByVal builder As Builder)
            MyBase.New(COMPONENT_TYPE)
            Me.title = builder.titleValue
            Me.lowerBounds = New List(Of Double)(builder.lowerBoundsValue)
            Me.upperBounds = New List(Of Double)(builder.upperBoundsValue)
            Me.yValues = New List(Of Double)(builder.yValuesValue)
            Me.marginTop = builder.marginTopValue
            Me.marginBottom = builder.marginBottomValue
            Me.marginLeft = builder.marginLeftValue
            Me.marginRight = builder.marginRightValue
        End Sub

        Public Overrides Function toHtml() As String
            Dim top As Double = Math.Max(0, marginTop)
            Dim left As Double = Math.Max(0, marginLeft)
            Dim width As Double = left + PLOT_WIDTH + Math.Max(0, marginRight)
            Dim height As Double = top + PLOT_HEIGHT + Math.Max(0, marginBottom)
            Dim plotRight As Double = left + PLOT_WIDTH
            Dim plotBottom As Double = top + PLOT_HEIGHT
            Dim n As Integer = Math.Min(If(lowerBounds Is Nothing, 0, lowerBounds.Count), Math.Min(If(upperBounds Is Nothing, 0, upperBounds.Count), If(yValues Is Nothing, 0, yValues.Count)))

            Dim xLow As Double = Double.PositiveInfinity
            Dim xHigh As Double = Double.NegativeInfinity
            Dim yMax As Double = 0
            Dim integralValues As Boolean = True
            For i As Integer = 0 To n - 1
                If isFinite(lowerBounds(i)) Then xLow = Math.Min(xLow, lowerBounds(i))
                If isFinite(upperBounds(i)) Then xHigh = Math.Max(xHigh, upperBounds(i))
                If isFinite(yValues(i)) Then
                    yMax = Math.Max(yMax, yValues(i))
                    If yValues(i) <> Math.Truncate(yValues(i)) Then integralValues = False
                End If
            Next
            Dim hasData As Boolean = n > 0 AndAlso xLow <= xHigh

            Dim sb As New StringBuilder()
            sb.Append("<figure class=""dv-figure dv-histogram"">")
            If Not String.IsNullOrEmpty(title) Then sb.Append("<figcaption>").Append(HtmlEncode(title)).Append("</figcaption>")
            sb.Append("<svg class=""dv-chart"" viewBox=""0 0 ").Append(SvgNum(width)).Append(" "c).Append(SvgNum(height)).Append(""" width=""").Append(SvgNum(width)).
                Append(""" height=""").Append(SvgNum(height)).Append(""" role=""img"" aria-label=""").
                Append(HtmlEncode("Histogram" & If(String.IsNullOrEmpty(title), "", ": " & title) & " (" & n & " bins)")).Append(""">")

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
            If yMax <= 0 Then yMax = 1
            Dim yStep As Double = NiceStep(yMax / 4)
            If integralValues Then yStep = Math.Max(1, yStep)
            Dim yTop As Double = Math.Ceiling(yMax / yStep - 1.0E-9) * yStep
            Dim x0 As Double = xLow
            Dim xSpan As Double = xHigh - xLow
            Dim scaleX As Func(Of Double, Double) = Function(v) left + (v - x0) / xSpan * PLOT_WIDTH
            Dim boundText As Func(Of Double, String)
            Dim tickText As Func(Of Double, String) = Nothing
            If timeAxis Then
                boundText = Function(v) FormatEpochMillis(v)
                tickText = Function(v) FormatTimeTick(v, xSpan)
            Else
                boundText = Function(v) FormatValue(v)
            End If
            Dim scaleY As Func(Of Double, Double) = Function(v) plotBottom - Math.Max(0, Math.Min(v, yTop)) / yTop * PLOT_HEIGHT

            AppendYGrid(sb, TicksBetween(0, yTop, yStep), scaleY, left, plotRight)

            sb.Append("<g class=""dv-bars"">")
            For i As Integer = 0 To n - 1
                Dim lower As Double = lowerBounds(i)
                Dim upper As Double = upperBounds(i)
                If Not isFinite(lower) OrElse Not isFinite(upper) Then Continue For
                Dim xa As Double = scaleX(Math.Min(lower, upper))
                Dim xb As Double = scaleX(Math.Max(lower, upper))
                Dim gap As Double = If(xb - xa >= 6, 1, 0)
                sb.Append("<g class=""dv-bin""><title>").Append(HtmlEncode(boundText(lower) & " to " & boundText(upper) & ": " & FormatValue(yValues(i)))).Append("</title>")
                sb.Append("<rect class=""dv-hit"" fill=""transparent"" x=""").Append(SvgNum(xa)).Append(""" y=""").Append(SvgNum(top)).Append(""" width=""").
                    Append(SvgNum(Math.Max(1, xb - xa))).Append(""" height=""").Append(SvgNum(PLOT_HEIGHT)).Append("""/>")
                Dim y As Double = yValues(i)
                If isFinite(y) AndAlso y > 0 Then
                    sb.Append("<path class=""dv-bar"" fill=""").Append(SeriesColorsLight(0)).Append(""" d=""").
                        Append(barPath(xa + gap, xb - gap, scaleY(y), plotBottom)).Append("""/>")
                End If
                sb.Append("</g>")
            Next
            sb.Append("</g>")

            AppendBaseline(sb, left, plotRight, plotBottom)
            AppendXLabels(sb, HorizontalTicks(xLow, xHigh, PLOT_WIDTH, tickText), scaleX, plotBottom, tickText)
            sb.Append("</svg>")

            ' The table view: every value is reachable without hovering.
            Dim rows As New List(Of String())()
            For i As Integer = 0 To n - 1
                rows.Add(New String() {boundText(lowerBounds(i)), boundText(upperBounds(i)), FormatValue(yValues(i))})
            Next
            Dim data As RenderableComponentTable = (New RenderableComponentTable.Builder()).header("From", "To", "Value").table(rows.ToArray()).build()
            sb.Append("<details class=""dv-data""><summary>Histogram data</summary>").Append(data.renderHtml("dv-compact")).Append("</details>")
            sb.Append("</figure>")
            Return sb.ToString()
        End Function

        ''' <summary>A column from the baseline up to <paramref name="yTop"/>, square at the baseline with 4px rounded top corners.</summary>
        Private Shared Function barPath(ByVal x0 As Double, ByVal x1 As Double, ByVal yTop As Double, ByVal yBase As Double) As String
            If x1 - x0 < 1 Then x1 = x0 + 1
            Dim r As Double = Math.Min(4, Math.Min((x1 - x0) / 2, yBase - yTop))
            Dim sb As New StringBuilder()
            sb.Append("M"c).Append(SvgNum(x0)).Append(","c).Append(SvgNum(yBase))
            sb.Append("V"c).Append(SvgNum(yTop + r))
            sb.Append("Q"c).Append(SvgNum(x0)).Append(","c).Append(SvgNum(yTop)).Append(" "c).Append(SvgNum(x0 + r)).Append(","c).Append(SvgNum(yTop))
            sb.Append("H"c).Append(SvgNum(x1 - r))
            sb.Append("Q"c).Append(SvgNum(x1)).Append(","c).Append(SvgNum(yTop)).Append(" "c).Append(SvgNum(x1)).Append(","c).Append(SvgNum(yTop + r))
            sb.Append("V"c).Append(SvgNum(yBase)).Append("Z"c)
            Return sb.ToString()
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

        Public Function getLowerBounds() As IList(Of Double)
            Return lowerBounds
        End Function

        Public Sub setLowerBounds(ByVal lowerBounds As IList(Of Double))
            Me.lowerBounds = lowerBounds
        End Sub

        Public Function getUpperBounds() As IList(Of Double)
            Return upperBounds
        End Function

        Public Sub setUpperBounds(ByVal upperBounds As IList(Of Double))
            Me.upperBounds = upperBounds
        End Sub

        Public Function getYValues() As IList(Of Double)
            Return yValues
        End Function

        Public Sub setYValues(ByVal yValues As IList(Of Double))
            Me.yValues = yValues
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

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As RenderableComponentHistogram = DirectCast(obj, RenderableComponentHistogram)
            Return String.Equals(title, other.title) AndAlso ListEquals(lowerBounds, other.lowerBounds) AndAlso ListEquals(upperBounds, other.upperBounds) AndAlso
                   ListEquals(yValues, other.yValues) AndAlso marginTop = other.marginTop AndAlso marginBottom = other.marginBottom AndAlso
                   marginLeft = other.marginLeft AndAlso marginRight = other.marginRight
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), title, ListHash(lowerBounds), ListHash(upperBounds), ListHash(yValues), marginTop, marginBottom, marginLeft, marginRight)
        End Function

        Public Overrides Function ToString() As String
            Return "RenderableComponentHistogram(title=" & Objects.toString(title) & ", lowerBounds=" & ListToString(lowerBounds) & ", upperBounds=" & ListToString(upperBounds) &
                   ", yValues=" & ListToString(yValues) & ", marginTop=" & marginTop & ", marginBottom=" & marginBottom & ", marginLeft=" & marginLeft &
                   ", marginRight=" & marginRight & ")"
        End Function

        ''' <summary>Fluent builder of a <see cref="RenderableComponentHistogram"/>; margins default to top 60, bottom 60, left 60, right 20.</summary>
        Public Class Builder

            Friend titleValue As String
            Friend lowerBoundsValue As IList(Of Double) = New List(Of Double)()
            Friend upperBoundsValue As IList(Of Double) = New List(Of Double)()
            Friend yValuesValue As IList(Of Double) = New List(Of Double)()
            Friend marginTopValue As Integer = 60
            Friend marginBottomValue As Integer = 60
            Friend marginLeftValue As Integer = 60
            Friend marginRightValue As Integer = 20

            Public Overridable Function title(ByVal value As String) As Builder
                Me.titleValue = value
                Return Me
            End Function

            ''' <summary>Adds a bin from <paramref name="lower"/> to <paramref name="upper"/> with the value <paramref name="yValue"/>.</summary>
            Public Overridable Function addBin(ByVal lower As Double, ByVal upper As Double, ByVal yValue As Double) As Builder
                lowerBoundsValue.Add(lower)
                upperBoundsValue.Add(upper)
                yValuesValue.Add(yValue)
                Return Me
            End Function

            Public Overridable Function margins(ByVal top As Integer, ByVal bottom As Integer, ByVal left As Integer, ByVal right As Integer) As Builder
                Me.marginTopValue = top
                Me.marginBottomValue = bottom
                Me.marginLeftValue = left
                Me.marginRightValue = right
                Return Me
            End Function

            Public Overridable Function build() As RenderableComponentHistogram
                Return New RenderableComponentHistogram(Me)
            End Function
        End Class
    End Class

End Namespace
