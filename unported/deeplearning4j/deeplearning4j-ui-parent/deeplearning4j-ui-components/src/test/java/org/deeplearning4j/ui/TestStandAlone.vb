Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports LengthUnit = org.deeplearning4j.ui.api.LengthUnit
Imports ChartHistogram = org.deeplearning4j.ui.components.chart.ChartHistogram
Imports ChartLine = org.deeplearning4j.ui.components.chart.ChartLine
Imports StyleChart = org.deeplearning4j.ui.components.chart.style.StyleChart
Imports ComponentTable = org.deeplearning4j.ui.components.table.ComponentTable
Imports StyleTable = org.deeplearning4j.ui.components.table.style.StyleTable
Imports StaticPageUtil = org.deeplearning4j.ui.standalone.StaticPageUtil
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestStandAlone extends org.deeplearning4j.BaseDL4JTest
	Public Class TestStandAlone
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStandAlone() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStandAlone()


			Dim ct As ComponentTable = (New ComponentTable.Builder((New StyleTable.Builder()).backgroundColor(Color.LIGHT_GRAY).columnWidths(LengthUnit.Px, 100, 100).build())).content(New String()() {
				New String() {"First", "Second"},
				New String() {"More", "More2"}
			}).build()

			Dim cl As ChartLine = (New ChartLine.Builder("Title", (New StyleChart.Builder()).axisStrokeWidth(1.0).seriesColors(Color.BLACK, Color.ORANGE).width(640, LengthUnit.Px).height(480, LengthUnit.Px).build())).addSeries("First Series", New Double() {0, 1, 2, 3, 4, 5}, New Double() {10, 20, 30, 40, 50, 60}).addSeries("Second", New Double() {0, 0.5, 1, 1.5, 2}, New Double() {5, 10, 15, 10, 5}).build()

			Dim ch As ChartHistogram = (New ChartHistogram.Builder("Histogram", (New StyleChart.Builder()).axisStrokeWidth(1.0).seriesColors(Color.MAGENTA).width(640, LengthUnit.Px).height(480, LengthUnit.Px).build())).addBin(0, 1, 1).addBin(1, 2, 2).addBin(2, 3, 1).build()

			Dim s As String = StaticPageUtil.renderHTML(ct, cl, ch)
		End Sub

	End Class

End Namespace