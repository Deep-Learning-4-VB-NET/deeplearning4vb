Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Component = org.deeplearning4j.ui.api.Component
Imports LengthUnit = org.deeplearning4j.ui.api.LengthUnit
Imports Style = org.deeplearning4j.ui.api.Style
Imports org.deeplearning4j.ui.components.chart
Imports StyleChart = org.deeplearning4j.ui.components.chart.style.StyleChart
Imports ComponentDiv = org.deeplearning4j.ui.components.component.ComponentDiv
Imports StyleDiv = org.deeplearning4j.ui.components.component.style.StyleDiv
Imports DecoratorAccordion = org.deeplearning4j.ui.components.decorator.DecoratorAccordion
Imports StyleAccordion = org.deeplearning4j.ui.components.decorator.style.StyleAccordion
Imports ComponentTable = org.deeplearning4j.ui.components.table.ComponentTable
Imports StyleTable = org.deeplearning4j.ui.components.table.style.StyleTable
Imports ComponentText = org.deeplearning4j.ui.components.text.ComponentText
Imports StyleText = org.deeplearning4j.ui.components.text.style.StyleText
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestRendering extends org.deeplearning4j.BaseDL4JTest
	Public Class TestRendering
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test public void test() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub test()

			Dim list As IList(Of Component) = New List(Of Component)()

			'Common style for all of the charts
			Dim s As StyleChart = (New StyleChart.Builder()).width(640, LengthUnit.Px).height(480, LengthUnit.Px).margin(LengthUnit.Px, 100, 40, 40, 20).strokeWidth(2).pointSize(4).seriesColors(Color.GREEN, Color.MAGENTA).titleStyle((New StyleText.Builder()).font("courier").fontSize(16).underline(True).color(Color.GRAY).build()).build()

			'Line chart with vertical grid
			Dim c1 As Component = (New ChartLine.Builder("Line Chart!", s)).addSeries("series0", New Double() {0, 1, 2, 3}, New Double() {0, 2, 1, 4}).addSeries("series1", New Double() {0, 1, 2, 3}, New Double() {0, 1, 0.5, 2.5}).setGridWidth(1.0, Nothing).build()
			list.Add(c1)

			'Scatter chart
			Dim c2 As Component = (New ChartScatter.Builder("Scatter!", s)).addSeries("series0", New Double() {0, 1, 2, 3}, New Double() {0, 2, 1, 4}).showLegend(True).setGridWidth(0, 0).build()
			list.Add(c2)

			'Histogram with variable sized bins
			Dim c3 As Component = (New ChartHistogram.Builder("Histogram!", s)).addBin(-1, -0.5, 0.2).addBin(-0.5, 0, 0.5).addBin(0, 1, 2.5).addBin(1, 2, 0.5).build()
			list.Add(c3)

			'Stacked area chart
			Dim c4 As Component = (New ChartStackedArea.Builder("Area Chart!", s)).setXValues(New Double() {0, 1, 2, 3, 4, 5}).addSeries("series0", New Double() {0, 1, 0, 2, 0, 1}).addSeries("series1", New Double() {2, 1, 2, 0.5, 2, 1}).build()
			list.Add(c4)

			'Table
			Dim ts As StyleTable = (New StyleTable.Builder()).backgroundColor(Color.LIGHT_GRAY).headerColor(Color.ORANGE).borderWidth(1).columnWidths(LengthUnit.Percent, 20, 40, 40).width(500, LengthUnit.Px).height(200, LengthUnit.Px).build()

			Dim c5 As Component = (New ComponentTable.Builder(ts)).header("H1", "H2", "H3").content(New String()() {
				New String() {"row0col0", "row0col1", "row0col2"},
				New String() {"row1col0", "row1col1", "row1col2"}
			}).build()
			list.Add(c5)

			'Accordion decorator, with the same chart
			Dim ac As StyleAccordion = (New StyleAccordion.Builder()).height(480, LengthUnit.Px).width(640, LengthUnit.Px).build()

			Dim c6 As Component = (New DecoratorAccordion.Builder(ac)).title("Accordion - Collapsed By Default!").setDefaultCollapsed(True).addComponents(c5).build()
			list.Add(c6)

			'Text with styling
			Dim c7 As Component = (New ComponentText.Builder("Here's some blue text in a green div!", (New StyleText.Builder()).font("courier").fontSize(30).underline(True).color(Color.BLUE).build())).build()

			'Div, with a chart inside
			Dim divStyle As Style = (New StyleDiv.Builder()).width(30, LengthUnit.Percent).height(200, LengthUnit.Px).backgroundColor(Color.GREEN).floatValue(StyleDiv.FloatValue.right).build()
			Dim c8 As Component = New ComponentDiv(divStyle, c7, New ComponentText("(Also: it's float right, 30% width, 200 px high )", Nothing))
			list.Add(c8)


			'Timeline chart:
			Dim entries As IList(Of ChartTimeline.TimelineEntry) = New List(Of ChartTimeline.TimelineEntry)()
			For i As Integer = 0 To 9
				entries.Add(New ChartTimeline.TimelineEntry("e0-" & i, 10 * i, 10 * i + 5, Color.BLUE))
			Next i
			Dim entries2 As IList(Of ChartTimeline.TimelineEntry) = New List(Of ChartTimeline.TimelineEntry)()
			For i As Integer = 0 To 9
				entries2.Add(New ChartTimeline.TimelineEntry("e1-" & i, CInt(Math.Truncate(5 * i + 0.2 * i * i)), CInt(Math.Truncate(5 * i + 0.2 * i * i)) + 3, Color.ORANGE))
			Next i
			Dim entries3 As IList(Of ChartTimeline.TimelineEntry) = New List(Of ChartTimeline.TimelineEntry)()
			For i As Integer = 0 To 9
				entries3.Add(New ChartTimeline.TimelineEntry("e2-" & i, CInt(Math.Truncate(2 * i + 0.6 * i * i + 3)), CInt(Math.Truncate(2 * i + 0.6 * i * i + 3)) + 2 * i + 1))
			Next i
			Dim c() As Color = {Color.CYAN, Color.YELLOW, Color.GREEN, Color.PINK}
			Dim entries4 As IList(Of ChartTimeline.TimelineEntry) = New List(Of ChartTimeline.TimelineEntry)()
			Dim r As New Random(12345)
			For i As Integer = 0 To 9
				entries4.Add(New ChartTimeline.TimelineEntry("e3-" & i, CInt(Math.Truncate(2 * i + 0.6 * i * i + 3)), CInt(Math.Truncate(2 * i + 0.6 * i * i + 3)) + i + 1, c(r.Next(c.Length))))
			Next i
			Dim c9 As Component = (New ChartTimeline.Builder("Title", s)).addLane("Lane 0", entries).addLane("Lane 1", entries2).addLane("Lane 2", entries3).addLane("Lane 3", entries4).build()
			list.Add(c9)



			'Generate HTML
			Dim sb As New StringBuilder()
			sb.Append("<!DOCTYPE html>" & vbLf & "<html lang=""en"">" & vbLf & "<head>" & vbLf & "    <meta charset=""UTF-8"">" & vbLf & "    <title>Title</title>" & vbLf & "</head>" & vbLf & "<body>" & vbLf & vbLf & "    <div id=""maindiv"">" & vbLf & vbLf & "    </div>" & vbLf & vbLf & vbLf & "    <script src=""//code.jquery.com/jquery-1.10.2.js""></script>" & vbLf & "    <script src=""https://code.jquery.com/ui/1.11.4/jquery-ui.min.js""></script>" & vbLf & "    <link rel=""stylesheet"" href=""//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css"">" & vbLf & "    <script src=""https://cdnjs.cloudflare.com/ajax/libs/d3/3.5.5/d3.min.js""></script>" & vbLf & "    <script src=""src/main/resources/assets/dl4j-ui.js""></script>" & vbLf & vbLf & "    <script>" & vbLf)

			Dim om As New ObjectMapper()
			For i As Integer = 0 To list.Count - 1
				Dim component As Component = list(i)
				sb.Append("        ").Append("var str").Append(i).Append(" = '").Append(om.writeValueAsString(component).replaceAll("'", "\\'")).Append("';" & vbLf)

				sb.Append("        ").Append("var obj").Append(i).Append(" = Component.getComponent(str").Append(i).Append(");" & vbLf)
				sb.Append("        ").Append("obj").Append(i).Append(".render($('#maindiv'));" & vbLf)
				sb.Append(vbLf & vbLf)
			Next i


			sb.Append("    </script>" & vbLf & vbLf & "</body>" & vbLf & "</html>")

			FileUtils.writeStringToFile(New File("TestRendering.html"), sb.ToString())
		End Sub

	End Class

End Namespace