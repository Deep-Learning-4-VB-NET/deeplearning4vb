Imports System.Collections.Generic
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
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestComponentSerialization extends org.deeplearning4j.BaseDL4JTest
	Public Class TestComponentSerialization
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerialization() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerialization()

			'Common style for all of the charts
			Dim s As StyleChart = (New StyleChart.Builder()).width(640, LengthUnit.Px).height(480, LengthUnit.Px).margin(LengthUnit.Px, 100, 40, 40, 20).strokeWidth(2).pointSize(4).seriesColors(Color.GREEN, Color.MAGENTA).titleStyle((New StyleText.Builder()).font("courier").fontSize(16).underline(True).color(Color.GRAY).build()).build()
			assertSerializable(s)


			'Line chart with vertical grid
			Dim c1 As Component = (New ChartLine.Builder("Line Chart!", s)).addSeries("series0", New Double() {0, 1, 2, 3}, New Double() {0, 2, 1, 4}).addSeries("series1", New Double() {0, 1, 2, 3}, New Double() {0, 1, 0.5, 2.5}).setGridWidth(1.0, Nothing).build()
			assertSerializable(c1)

			'Scatter chart
			Dim c2 As Component = (New ChartScatter.Builder("Scatter!", s)).addSeries("series0", New Double() {0, 1, 2, 3}, New Double() {0, 2, 1, 4}).showLegend(True).setGridWidth(0, 0).build()
			assertSerializable(c2)

			'Histogram with variable sized bins
			Dim c3 As Component = (New ChartHistogram.Builder("Histogram!", s)).addBin(-1, -0.5, 0.2).addBin(-0.5, 0, 0.5).addBin(0, 1, 2.5).addBin(1, 2, 0.5).build()
			assertSerializable(c3)

			'Stacked area chart
			Dim c4 As Component = (New ChartStackedArea.Builder("Area Chart!", s)).setXValues(New Double() {0, 1, 2, 3, 4, 5}).addSeries("series0", New Double() {0, 1, 0, 2, 0, 1}).addSeries("series1", New Double() {2, 1, 2, 0.5, 2, 1}).build()
			assertSerializable(c4)

			'Table
			Dim ts As StyleTable = (New StyleTable.Builder()).backgroundColor(Color.LIGHT_GRAY).headerColor(Color.ORANGE).borderWidth(1).columnWidths(LengthUnit.Percent, 20, 40, 40).width(500, LengthUnit.Px).height(200, LengthUnit.Px).build()
			assertSerializable(ts)

			Dim c5 As Component = (New ComponentTable.Builder(ts)).header("H1", "H2", "H3").content(New String()() {
				New String() {"row0col0", "row0col1", "row0col2"},
				New String() {"row1col0", "row1col1", "row1col2"}
			}).build()
			assertSerializable(c5)

			'Accordion decorator, with the same chart
			Dim ac As StyleAccordion = (New StyleAccordion.Builder()).height(480, LengthUnit.Px).width(640, LengthUnit.Px).build()
			assertSerializable(ac)

			Dim c6 As Component = (New DecoratorAccordion.Builder(ac)).title("Accordion - Collapsed By Default!").setDefaultCollapsed(True).addComponents(c5).build()
			assertSerializable(c6)

			'Text with styling
			Dim c7 As Component = (New ComponentText.Builder("Here's some blue text in a green div!", (New StyleText.Builder()).font("courier").fontSize(30).underline(True).color(Color.BLUE).build())).build()
			assertSerializable(c7)

			'Div, with a chart inside
			Dim divStyle As Style = (New StyleDiv.Builder()).width(30, LengthUnit.Percent).height(200, LengthUnit.Px).backgroundColor(Color.GREEN).floatValue(StyleDiv.FloatValue.right).build()
			assertSerializable(divStyle)
			Dim c8 As Component = New ComponentDiv(divStyle, c7, New ComponentText("(Also: it's float right, 30% width, 200 px high )", Nothing))
			assertSerializable(c8)


			'Timeline chart:
			Dim entries As IList(Of ChartTimeline.TimelineEntry) = New List(Of ChartTimeline.TimelineEntry)()
			For i As Integer = 0 To 9
				entries.Add(New ChartTimeline.TimelineEntry(i.ToString(), 10 * i, 10 * i + 5))
			Next i
			Dim c9 As Component = (New ChartTimeline.Builder("Title", s)).addLane("Lane0", entries).build()
			assertSerializable(c9)
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void assertSerializable(org.deeplearning4j.ui.api.Component component) throws Exception
		Private Shared Sub assertSerializable(ByVal component As Component)

			Dim om As New ObjectMapper()

			Dim json As String = om.writeValueAsString(component)

			Dim fromJson As Component = om.readValue(json, GetType(Component))

			assertEquals(component.ToString(), fromJson.ToString()) 'Yes, this is a bit hacky, but lombok equal method doesn't seem to work properly for List<double[]> etc
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void assertSerializable(org.deeplearning4j.ui.api.Style style) throws Exception
		Private Shared Sub assertSerializable(ByVal style As Style)
			Dim om As New ObjectMapper()

			Dim json As String = om.writeValueAsString(style)

			Dim fromJson As Style = om.readValue(json, GetType(Style))

			assertEquals(style.ToString(), fromJson.ToString()) 'Yes, this is a bit hacky, but lombok equal method doesn't seem to work properly for List<double[]> etc
		End Sub

	End Class

End Namespace