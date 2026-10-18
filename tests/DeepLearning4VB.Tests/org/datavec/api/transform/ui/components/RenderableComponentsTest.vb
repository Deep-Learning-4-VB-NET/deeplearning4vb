Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DivObjectT = DeepLearning4VB.org.datavec.api.transform.ui.DivObject
Imports RenderableComponentHistogram = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponentHistogram
Imports RenderableComponentLineChart = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponentLineChart
Imports RenderableComponentT = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponent
Imports RenderableComponentTable = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponentTable

Namespace org.datavec.api.transform.ui.components

    ''' <summary>Tests of the renderable components: builders, Lombok-style accessors and equality, and their HTML/SVG.</summary>
    <TestClass>
    Public Class RenderableComponentsTest

        ' ---- histogram ----

        Private Shared Function histogram() As RenderableComponentHistogram
            Return (New RenderableComponentHistogram.Builder()).title("Lengths").addBin(0, 1, 5).addBin(1, 2, 12).addBin(2, 4, 0).margins(10, 20, 30, 40).build()
        End Function

        <TestMethod>
        Public Sub histogramBuilder()
            Dim h As RenderableComponentHistogram = histogram()
            assertEquals(RenderableComponentHistogram.COMPONENT_TYPE, h.getComponentType())
            assertEquals("histogram", h.getComponentType())
            assertEquals("Lengths", h.getTitle())
            assertEquals(New List(Of Double) From {0.0, 1.0, 2.0}, h.getLowerBounds())
            assertEquals(New List(Of Double) From {1.0, 2.0, 4.0}, h.getUpperBounds())
            assertEquals(New List(Of Double) From {5.0, 12.0, 0.0}, h.getYValues())
            assertEquals(10, h.getMarginTop())
            assertEquals(20, h.getMarginBottom())
            assertEquals(30, h.getMarginLeft())
            assertEquals(40, h.getMarginRight())

            Dim defaults As RenderableComponentHistogram = (New RenderableComponentHistogram.Builder()).build()
            assertNull(defaults.getTitle())
            assertEquals(0, defaults.getLowerBounds().Count)
            assertEquals(60, defaults.getMarginTop())
            assertEquals(60, defaults.getMarginBottom())
            assertEquals(60, defaults.getMarginLeft())
            assertEquals(20, defaults.getMarginRight())

            assertEquals(histogram(), h)
            assertEquals(histogram().GetHashCode(), h.GetHashCode())
            assertNotEquals(defaults, h)
            h.setTitle("Other")
            assertNotEquals(histogram(), h)
            assertTrue(histogram().ToString().StartsWith("RenderableComponentHistogram(title=Lengths, lowerBounds=[0.0, 1.0, 2.0]", StringComparison.Ordinal))
        End Sub

        <TestMethod>
        Public Sub histogramHtml()
            Dim html As String = histogram().toHtml()
            assertTrue(html.StartsWith("<figure class=""dv-figure dv-histogram""><figcaption>Lengths</figcaption><svg class=""dv-chart""", StringComparison.Ordinal))
            'plot area 480 x 200 plus the margins
            assertTrue(html.Contains("viewBox=""0 0 550 230"""))
            assertEquals(3, UiTestSupport.countOccurrences(html, "<g class=""dv-bin"">"))
            'the empty bin keeps its hover target but has no bar
            assertEquals(2, UiTestSupport.countOccurrences(html, "<path class=""dv-bar"""))
            assertTrue(html.Contains("<title>1 to 2: 12</title>"))
            assertTrue(html.Contains("<summary>Histogram data</summary>"))
            assertTrue(html.Contains("<tr><td class=""dv-num"">2</td><td class=""dv-num"">4</td><td class=""dv-num"">0</td></tr>"))
            assertTrue(html.EndsWith("</figure>", StringComparison.Ordinal))

            Dim empty As String = (New RenderableComponentHistogram.Builder()).title("<i>none</i>").build().toHtml()
            assertTrue(empty.Contains("<figcaption>&lt;i&gt;none&lt;/i&gt;</figcaption>"))
            assertTrue(empty.Contains(">No data</text>"))
        End Sub

        ' ---- line chart ----

        Private Shared Function lineChart() As RenderableComponentLineChart
            Return (New RenderableComponentLineChart.Builder()).title("Two series").addSeries("first", New Double() {0, 1, 2}, New Double() {1, 3, 2}).
                addSeries("second", New Double() {0, 1, 2}, New Double() {2, Double.NaN, 0.5}).build()
        End Function

        <TestMethod>
        Public Sub lineChartBuilder()
            Dim lc As RenderableComponentLineChart = lineChart()
            assertEquals("linechart", lc.getComponentType())
            assertEquals("Two series", lc.getTitle())
            assertEquals(New List(Of String) From {"first", "second"}, lc.getSeriesNames())
            assertEquals(2, lc.getX().Count)
            assertArrayEquals(New Double() {1, 3, 2}, lc.getY()(0), 0.0)
            assertFalse(lc.isRemoveAxisHorizontal())
            assertTrue(lc.isLegend())
            assertEquals(60, lc.getMarginTop())
            assertEquals(20, lc.getMarginRight())

            Dim custom As RenderableComponentLineChart = (New RenderableComponentLineChart.Builder()).setRemoveAxisHorizontal(True).margins(1, 2, 3, 4).legend(False).build()
            assertTrue(custom.isRemoveAxisHorizontal())
            assertFalse(custom.isLegend())
            assertEquals(1, custom.getMarginTop())
            assertEquals(2, custom.getMarginBottom())
            assertEquals(3, custom.getMarginLeft())
            assertEquals(4, custom.getMarginRight())

            assertEquals(lineChart(), lc)
            assertEquals(lineChart().GetHashCode(), lc.GetHashCode())
            assertNotEquals(custom, lc)
            Dim noArg As New RenderableComponentLineChart()
            assertEquals("linechart", noArg.getComponentType())
            assertNull(noArg.getX())
        End Sub

        <TestMethod>
        Public Sub lineChartHtml()
            Dim html As String = lineChart().toHtml()
            assertTrue(html.Contains("<figcaption>Two series</figcaption>"))
            'Two series: a legend with both names
            assertTrue(html.Contains("<ul class=""dv-legend"">"))
            assertTrue(html.Contains("</svg>first</li>"))
            assertTrue(html.Contains("</svg>second</li>"))
            'The NaN in the second series splits it: one polyline for the first series, two lone dots for the second
            assertEquals(1, UiTestSupport.countOccurrences(html, "<polyline class=""dv-line"""))
            assertTrue(html.Contains("<title>second: x = 2, y = 0.5</title>"))
            assertTrue(html.Contains("dv-c1"))
            assertTrue(html.Contains("dv-c2"))
            'x axis labels are drawn unless removed
            assertTrue(html.Contains("text-anchor=""middle"""))

            Dim noAxis As String = (New RenderableComponentLineChart.Builder()).addSeries("s", New Double() {0, 1}, New Double() {0, 1}).setRemoveAxisHorizontal(True).build().toHtml()
            assertFalse(noAxis.Contains("text-anchor=""middle"""))
            assertFalse(noAxis.Contains("dv-legend"))
            assertTrue(New RenderableComponentLineChart().toHtml().Contains(">No data</text>"))
        End Sub

        ' ---- table ----

        <TestMethod>
        Public Sub tableBuilder()
            Dim cells As String()() = {New String() {"a", "1"}, New String() {"b", "2"}}
            Dim t As RenderableComponentTable = (New RenderableComponentTable.Builder()).title("T").header("Name", "Value").table(cells).border(2).
                padLeftPx(1).padRightPx(2).padTopPx(3).padBottomPx(4).colWidthsPercent(70, 30).backgroundColor("#FFFFFF").headerColor("#000000").build()
            assertEquals("simpletable", t.getComponentType())
            assertEquals("T", t.getTitle())
            assertArrayEquals(New String() {"Name", "Value"}, t.getHeader())
            assertTrue(t.getTable() Is cells)
            assertEquals(2, t.getBorder())
            assertEquals(1, t.getPadLeftPx())
            assertEquals(2, t.getPadRightPx())
            assertEquals(3, t.getPadTopPx())
            assertEquals(4, t.getPadBottomPx())
            assertArrayEquals(New Double() {70, 30}, t.getColWidthsPercent(), 0.0)
            assertEquals("#FFFFFF", t.getBackgroundColor())
            assertEquals("#000000", t.getHeaderColor())

            Dim same As RenderableComponentTable = (New RenderableComponentTable.Builder()).paddingPx(5).build()
            assertEquals(5, same.getPadLeftPx())
            assertEquals(5, same.getPadBottomPx())
            Dim perSide As RenderableComponentTable = (New RenderableComponentTable.Builder()).paddingPx(6, 7, 8, 9).build()
            assertEquals(6, perSide.getPadLeftPx())
            assertEquals(7, perSide.getPadRightPx())
            assertEquals(8, perSide.getPadTopPx())
            assertEquals(9, perSide.getPadBottomPx())

            'Java's convenience constructors, and content-based equality of the cell arrays
            Dim a As New RenderableComponentTable(New String() {"h"}, New String()() {New String() {"x"}})
            Dim b As New RenderableComponentTable(Nothing, New String() {"h"}, New String()() {New String() {"x"}})
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertNotEquals(a, New RenderableComponentTable("title", New String() {"h"}, New String()() {New String() {"x"}}))
            assertNull(New RenderableComponentTable().getTable())
            assertTrue(a.ToString().Contains("header=[h], table=[[x]]"))
        End Sub

        <TestMethod>
        Public Sub tableHtml()
            Dim cells As String()() = {New String() {"<b>a</b>", "1,234.5"}, New String() {"b", Nothing}}
            Dim html As String = (New RenderableComponentTable.Builder()).title("Stats & more").header("Name", "Value").table(cells).border(2).
                paddingPx(1, 2, 3, 4).colWidthsPercent(70, 30).backgroundColor("#FFFFFF").headerColor("#000000").build().toHtml()
            assertTrue(html.StartsWith("<div class=""dv-table-wrap""><table class=""dv-table"" style=""background-color:#FFFFFF;color:#0b0b0b;border:2px solid var(--dv-axis,#c3c2b7);"">", StringComparison.Ordinal))
            assertTrue(html.Contains("<caption>Stats &amp; more</caption>"))
            assertTrue(html.Contains("<colgroup><col style=""width:70%""><col style=""width:30%""></colgroup>"))
            assertTrue(html.Contains("<th scope=""col"" style=""padding:3px 2px 4px 1px;border:2px solid var(--dv-axis,#c3c2b7);background-color:#000000;color:#ffffff;"">Name</th>"))
            assertTrue(html.Contains(">&lt;b&gt;a&lt;/b&gt;</td>"))
            assertTrue(html.Contains("<td class=""dv-num"" style=""padding:3px 2px 4px 1px;border:2px solid var(--dv-axis,#c3c2b7);"">1,234.5</td>"))
            'a missing cell is empty
            assertTrue(html.Contains("""></td></tr>"))
            assertTrue(html.EndsWith("</tbody></table></div>", StringComparison.Ordinal))

            'Values that are not plain colours are ignored rather than written into the style attribute
            Dim unsafe As String = (New RenderableComponentTable.Builder()).backgroundColor("red;position:fixed").headerColor("url(x)").header("h").build().toHtml()
            assertFalse(unsafe.Contains("position"))
            assertFalse(unsafe.Contains("url("))
            assertTrue(unsafe.StartsWith("<div class=""dv-table-wrap""><table class=""dv-table"">", StringComparison.Ordinal))
            Dim named As String = (New RenderableComponentTable.Builder()).backgroundColor("lightyellow").build().toHtml()
            assertTrue(named.Contains("style=""background-color:lightyellow;"""))
        End Sub

        ' ---- base class and DivObject ----

        <TestMethod>
        Public Sub componentsOfDifferentKindsAreNotEqual()
            Dim components As RenderableComponentT() = {histogram(), lineChart(), New RenderableComponentTable()}
            assertEquals("RenderableComponent(componentType=simpletable)", DirectCast(New TestComponent("simpletable"), RenderableComponentT).ToString())
            For i As Integer = 0 To components.Length - 1
                For j As Integer = 0 To components.Length - 1
                    If i <> j Then assertNotEquals(components(i), components(j))
                Next
                assertTrue(components(i).toHtml().Length > 0)
            Next
        End Sub

        <TestMethod>
        Public Sub divObjectAccessors()
            Dim d As New DivObjectT("plot_1", "<p>x</p>")
            assertEquals("plot_1", d.getId())
            assertEquals("plot_1", d.Id)
            assertEquals("<p>x</p>", d.getContent())
            assertEquals("<p>x</p>", d.Content)
            assertEquals(New DivObjectT("plot_1", "<p>x</p>"), d)
            assertEquals(New DivObjectT("plot_1", "<p>x</p>").GetHashCode(), d.GetHashCode())
            assertNotEquals(New DivObjectT("plot_2", "<p>x</p>"), d)
            assertEquals("DivObject(id=plot_1, content=<p>x</p>)", d.ToString())
        End Sub

        ''' <summary>A minimal subclass, to exercise the base class members.</summary>
        Private Class TestComponent
            Inherits RenderableComponentT

            Public Sub New(ByVal componentType As String)
                MyBase.New(componentType)
            End Sub

            Public Overrides Function toHtml() As String
                Return "<p></p>"
            End Function

            Public Overrides Function ToString() As String
                Return MyBase.ToString()
            End Function
        End Class

    End Class

End Namespace
