Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Component = org.deeplearning4j.ui.api.Component
Imports LengthUnit = org.deeplearning4j.ui.api.LengthUnit
Imports ChartHistogram = org.deeplearning4j.ui.components.chart.ChartHistogram
Imports ChartLine = org.deeplearning4j.ui.components.chart.ChartLine
Imports StyleChart = org.deeplearning4j.ui.components.chart.style.StyleChart
Imports ComponentDiv = org.deeplearning4j.ui.components.component.ComponentDiv
Imports StyleDiv = org.deeplearning4j.ui.components.component.style.StyleDiv
Imports ComponentTable = org.deeplearning4j.ui.components.table.ComponentTable
Imports StyleTable = org.deeplearning4j.ui.components.table.style.StyleTable
Imports ComponentText = org.deeplearning4j.ui.components.text.ComponentText
Imports StyleText = org.deeplearning4j.ui.components.text.style.StyleText
Imports StaticPageUtil = org.deeplearning4j.ui.standalone.StaticPageUtil
Imports EvaluationCalibration = org.nd4j.evaluation.classification.EvaluationCalibration
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports Histogram = org.nd4j.evaluation.curves.Histogram
Imports PrecisionRecallCurve = org.nd4j.evaluation.curves.PrecisionRecallCurve
Imports ReliabilityDiagram = org.nd4j.evaluation.curves.ReliabilityDiagram
Imports RocCurve = org.nd4j.evaluation.curves.RocCurve

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

Namespace org.deeplearning4j.core.evaluation


	Public Class EvaluationTools

		Private Const ROC_TITLE As String = "ROC: TPR/Recall (y) vs. FPR (x)"
		Private Const PR_TITLE As String = "Precision (y) vs. Recall (x)"
		Private Const PR_THRESHOLD_TITLE As String = "Precision and Recall (y) vs. Classifier Threshold (x)"

		Private Const CHART_WIDTH_PX As Double = 600.0
		Private Const CHART_HEIGHT_PX As Double = 400.0

		Private Shared ReadOnly CHART_STYLE As StyleChart = (New StyleChart.Builder()).width(CHART_WIDTH_PX, LengthUnit.Px).height(CHART_HEIGHT_PX, LengthUnit.Px).margin(LengthUnit.Px, 60, 60, 75, 10).strokeWidth(2.0).seriesColors(Color.BLUE, Color.LIGHT_GRAY).build()

		Private Shared ReadOnly CHART_STYLE_PRECISION_RECALL As StyleChart = (New StyleChart.Builder()).width(CHART_WIDTH_PX, LengthUnit.Px).height(CHART_HEIGHT_PX, LengthUnit.Px).margin(LengthUnit.Px, 60, 60, 40, 10).strokeWidth(2.0).seriesColors(Color.BLUE, Color.GREEN).build()

		Private Shared ReadOnly TABLE_STYLE As StyleTable = (New StyleTable.Builder()).backgroundColor(Color.WHITE).headerColor(Color.LIGHT_GRAY).borderWidth(1).columnWidths(LengthUnit.Percent, 50, 50).width(400, LengthUnit.Px).height(200, LengthUnit.Px).build()

		Private Shared ReadOnly OUTER_DIV_STYLE As StyleDiv = (New StyleDiv.Builder()).width(2 * CHART_WIDTH_PX, LengthUnit.Px).height(CHART_HEIGHT_PX, LengthUnit.Px).build()

		Private Shared ReadOnly OUTER_DIV_STYLE_WIDTH_ONLY As StyleDiv = (New StyleDiv.Builder()).width(2 * CHART_WIDTH_PX, LengthUnit.Px).build()

		Private Shared ReadOnly INNER_DIV_STYLE As StyleDiv = (New StyleDiv.Builder()).width(CHART_WIDTH_PX, LengthUnit.Px).floatValue(StyleDiv.FloatValue.left).build()

		Private Shared ReadOnly PAD_DIV_STYLE As StyleDiv = (New StyleDiv.Builder()).width(CHART_WIDTH_PX, LengthUnit.Px).height(100, LengthUnit.Px).floatValue(StyleDiv.FloatValue.left).build()

		Private Shared ReadOnly PAD_DIV As New ComponentDiv(PAD_DIV_STYLE)

		Private Shared ReadOnly HEADER_TEXT_STYLE As StyleText = (New StyleText.Builder()).color(Color.BLACK).fontSize(16).underline(True).build()

		Private Shared ReadOnly HEADER_DIV_STYLE As StyleDiv = (New StyleDiv.Builder()).width(2 * CHART_WIDTH_PX - 150, LengthUnit.Px).height(30, LengthUnit.Px).backgroundColor(Color.LIGHT_GRAY).margin(LengthUnit.Px, 5, 5, 200, 10).floatValue(StyleDiv.FloatValue.left).build()

		Private Shared ReadOnly HEADER_DIV_STYLE_1400 As StyleDiv = (New StyleDiv.Builder()).width(1400 - 150, LengthUnit.Px).height(30, LengthUnit.Px).backgroundColor(Color.LIGHT_GRAY).margin(LengthUnit.Px, 5, 5, 200, 10).floatValue(StyleDiv.FloatValue.left).build()

		Private Shared ReadOnly HEADER_DIV_PAD_STYLE As StyleDiv = (New StyleDiv.Builder()).width(2 * CHART_WIDTH_PX, LengthUnit.Px).height(150, LengthUnit.Px).backgroundColor(Color.WHITE).build()

		Private Shared ReadOnly HEADER_DIV_TEXT_PAD_STYLE As StyleDiv = (New StyleDiv.Builder()).width(120, LengthUnit.Px).height(30, LengthUnit.Px).backgroundColor(Color.LIGHT_GRAY).floatValue(StyleDiv.FloatValue.left).build()

		Private Shared ReadOnly INFO_TABLE As ComponentTable = (New ComponentTable.Builder((New StyleTable.Builder()).backgroundColor(Color.WHITE).borderWidth(0).build())).content(New String()() {
			New String() {"Precision", "(true positives) / (true positives + false positives)"},
			New String() {"True Positive Rate (Recall)", "(true positives) / (data positives)"},
			New String() {"False Positive Rate", "(false positives) / (data negatives)"}
		}).build()

		Private Sub New()
		End Sub

		''' <summary>
		''' Given a <seealso cref="ROC"/> chart, export the ROC chart and precision vs. recall charts to a stand-alone HTML file </summary>
		''' <param name="roc">  ROC to export </param>
		''' <param name="file"> File to export to </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportRocChartsToHtmlFile(org.nd4j.evaluation.classification.ROC roc, java.io.File file) throws java.io.IOException
		Public Shared Sub exportRocChartsToHtmlFile(ByVal roc As ROC, ByVal file As File)
			Dim rocAsHtml As String = rocChartToHtml(roc)
			FileUtils.writeStringToFile(file, rocAsHtml)
		End Sub

		''' <summary>
		''' Given a <seealso cref="ROCMultiClass"/> chart, export the ROC chart and precision vs. recall charts to a stand-alone HTML file </summary>
		''' <param name="roc">  ROC to export </param>
		''' <param name="file"> File to export to </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportRocChartsToHtmlFile(org.nd4j.evaluation.classification.ROCMultiClass roc, java.io.File file) throws Exception
		Public Shared Sub exportRocChartsToHtmlFile(ByVal roc As ROCMultiClass, ByVal file As File)
			Dim rocAsHtml As String = rocChartToHtml(roc)
			FileUtils.writeStringToFile(file, rocAsHtml, StandardCharsets.UTF_8)
		End Sub

		''' <summary>
		''' Given a <seealso cref="ROC"/> instance, render the ROC chart and precision vs. recall charts to a stand-alone HTML file (returned as a String) </summary>
		''' <param name="roc">  ROC to render </param>
		Public Shared Function rocChartToHtml(ByVal roc As ROC) As String
			Dim rocCurve As RocCurve = roc.RocCurve

			Dim c As Component = getRocFromPoints(ROC_TITLE, rocCurve, roc.getCountActualPositive(), roc.getCountActualNegative(), roc.calculateAUC(), roc.calculateAUCPR())
			Dim c2 As Component = getPRCharts(PR_TITLE, PR_THRESHOLD_TITLE, roc.PrecisionRecallCurve)

			Return StaticPageUtil.renderHTML(c, c2)
		End Function

		''' <summary>
		''' Given a <seealso cref="ROCMultiClass"/> instance, render the ROC chart and precision vs. recall charts to a stand-alone HTML file (returned as a String) </summary>
		''' <param name="rocMultiClass">  ROC to render </param>
		Public Shared Function rocChartToHtml(ByVal rocMultiClass As ROCMultiClass) As String
			Return rocChartToHtml(rocMultiClass, Nothing)
		End Function

		''' <summary>
		''' Given a <seealso cref="ROCMultiClass"/> instance and (optionally) names for each class, render the ROC chart to a stand-alone
		''' HTML file (returned as a String) </summary>
		''' <param name="rocMultiClass">  ROC to render </param>
		''' <param name="classNames">     Names of the classes. May be null </param>
		Public Shared Function rocChartToHtml(ByVal rocMultiClass As ROCMultiClass, ByVal classNames As IList(Of String)) As String

			Dim n As Integer = rocMultiClass.NumClasses

			Dim components As IList(Of Component) = New List(Of Component)(n)
			For i As Integer = 0 To n - 1
				Dim roc As RocCurve = rocMultiClass.getRocCurve(i)
				Dim headerText As String = "Class " & i
				If classNames IsNot Nothing AndAlso classNames.Count > i Then
					headerText &= " (" & classNames(i) & ")"
				End If
				headerText &= " vs. All"

				Dim headerDivPad As Component = New ComponentDiv(HEADER_DIV_PAD_STYLE)
				components.Add(headerDivPad)

				Dim headerDivLeft As Component = New ComponentDiv(HEADER_DIV_TEXT_PAD_STYLE)
				Dim headerDiv As Component = New ComponentDiv(HEADER_DIV_STYLE, New ComponentText(headerText, HEADER_TEXT_STYLE))
				Dim c As Component = getRocFromPoints(ROC_TITLE, roc, rocMultiClass.getCountActualPositive(i), rocMultiClass.getCountActualNegative(i), rocMultiClass.calculateAUC(i), rocMultiClass.calculateAUCPR(i))
				Dim c2 As Component = getPRCharts(PR_TITLE, PR_THRESHOLD_TITLE, rocMultiClass.getPrecisionRecallCurve(i))
				components.Add(headerDivLeft)
				components.Add(headerDiv)
				components.Add(c)
				components.Add(c2)
			Next i

			Return StaticPageUtil.renderHTML(components)
		End Function

		''' <summary>
		''' Given a <seealso cref="EvaluationCalibration"/> instance, export the charts to a stand-alone HTML file </summary>
		''' <param name="ec">  EvaluationCalibration instance to export HTML charts for </param>
		''' <param name="file"> File to export to </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportevaluationCalibrationToHtmlFile(org.nd4j.evaluation.classification.EvaluationCalibration ec, java.io.File file) throws java.io.IOException
		Public Shared Sub exportevaluationCalibrationToHtmlFile(ByVal ec As EvaluationCalibration, ByVal file As File)
			Dim asHtml As String = evaluationCalibrationToHtml(ec)
			FileUtils.writeStringToFile(file, asHtml)
		End Sub

		Public Shared Function evaluationCalibrationToHtml(ByVal ec As EvaluationCalibration) As String

			Dim components As IList(Of Component) = New List(Of Component)()
			Dim nClasses As Integer = ec.numClasses()

			'Distribution of class labels + distribution of predicted classes
			Dim headerDiv As Component = New ComponentDiv(HEADER_DIV_STYLE_1400, New ComponentText("Labels and Network Prediction Class Distributions (X: Class Index. Y: Count)", HEADER_TEXT_STYLE))
			components.Add(headerDiv)
			Dim labelCounts() As Integer = ec.LabelCountsEachClass
			Dim predictedCounts() As Integer = ec.PredictionCountsEachClass
			Dim chbLabels As New ChartHistogram.Builder("Label Class Distribution", CHART_STYLE)
			Dim chbPredictions As New ChartHistogram.Builder("Predicted Class Distribution", CHART_STYLE)
			For i As Integer = 0 To nClasses - 1
				Dim lower As Double = i - 0.5
				Dim upper As Double = i + 0.5
				chbLabels.addBin(lower, upper, labelCounts(i))
				chbPredictions.addBin(lower, upper, predictedCounts(i))
			Next i

			Dim chL As ChartHistogram = chbLabels.build()
			Dim chP As ChartHistogram = chbPredictions.build()
			components.Add(New ComponentDiv(OUTER_DIV_STYLE_WIDTH_ONLY, chL, chP))

			'Reliability diagram, for each class
			headerDiv = New ComponentDiv(HEADER_DIV_STYLE_1400, New ComponentText("Reliability Diagrams (X: Mean Predicted Value. Y: Fraction Positives)", HEADER_TEXT_STYLE))
			components.Add(headerDiv)
			Dim sectionDiv As IList(Of Component) = New List(Of Component)()
			Dim zeroOne() As Double = {0.0, 1.0}
			For i As Integer = 0 To nClasses - 1
				Dim rd As ReliabilityDiagram = ec.getReliabilityDiagram(i)

				Dim x() As Double = rd.getMeanPredictedValueX()
				Dim y() As Double = rd.getFractionPositivesY()
				Dim title As String = rd.Title

				Dim cl As ChartLine = (New ChartLine.Builder(title, CHART_STYLE)).addSeries("Classifier", x, y).addSeries("Ideal Classifier", zeroOne, zeroOne).build()

				sectionDiv.Add(cl)
			Next i
			components.Add(New ComponentDiv(OUTER_DIV_STYLE_WIDTH_ONLY, sectionDiv))

			'Residual plots
			headerDiv = New ComponentDiv(HEADER_DIV_STYLE_1400, New ComponentText("Network Predictions - Residual Plots - |Label(i) - P(class(i))|", HEADER_TEXT_STYLE))
			components.Add(headerDiv)

			sectionDiv = New List(Of Component)()
			Dim resPlotAll As Histogram = ec.ResidualPlotAllClasses
			sectionDiv.Add(getHistogram(resPlotAll))
			For i As Integer = 0 To nClasses - 1
				Dim resPlotCurrent As Histogram = ec.getResidualPlot(i)
				sectionDiv.Add(getHistogram(resPlotCurrent))
			Next i
			components.Add(New ComponentDiv(OUTER_DIV_STYLE_WIDTH_ONLY, sectionDiv))


			'Histogram of probabilities, overall and for each class
			headerDiv = New ComponentDiv(HEADER_DIV_STYLE_1400, New ComponentText("Network Prediction Probabilities (X: P(class). Y: Count)", HEADER_TEXT_STYLE))
			components.Add(headerDiv)
			sectionDiv = New List(Of Component)()
			Dim allProbs As Histogram = ec.ProbabilityHistogramAllClasses
			sectionDiv.Add(getHistogram(allProbs))

			For i As Integer = 0 To nClasses - 1
				Dim classProbs As Histogram = ec.getProbabilityHistogram(i)
				sectionDiv.Add(getHistogram(classProbs))
			Next i
			components.Add(New ComponentDiv(OUTER_DIV_STYLE_WIDTH_ONLY, sectionDiv))

			Return StaticPageUtil.renderHTML(components)
		End Function

		Private Shared Function getRocFromPoints(ByVal title As String, ByVal roc As RocCurve, ByVal positiveCount As Long, ByVal negativeCount As Long, ByVal auc As Double, ByVal aucpr As Double) As Component
			Dim zeroOne() As Double = {0.0, 1.0}

			Dim chartLine As ChartLine = (New ChartLine.Builder(title, CHART_STYLE)).setXMin(0.0).setXMax(1.0).setYMin(0.0).setYMax(1.0).addSeries("ROC", roc.X, roc.Y).addSeries("", zeroOne, zeroOne).build()

			Dim ct As ComponentTable = (New ComponentTable.Builder(TABLE_STYLE)).header("Field", "Value").content(New String()() {
				New String() {"AUROC: Area under ROC:", String.Format("{0:F5}", auc)},
				New String() {"AUPRC: Area under P/R:", String.Format("{0:F5}", aucpr)},
				New String() {"Total Data Positive Count", positiveCount.ToString()},
				New String() {"Total Data Negative Count", negativeCount.ToString()}
			}).build()

			Dim divLeft As New ComponentDiv(INNER_DIV_STYLE, PAD_DIV, ct, PAD_DIV, INFO_TABLE)
			Dim divRight As New ComponentDiv(INNER_DIV_STYLE, chartLine)

			Return New ComponentDiv(OUTER_DIV_STYLE, divLeft, divRight)
		End Function

		Private Shared Function getPRCharts(ByVal precisionRecallTitle As String, ByVal prThresholdTitle As String, ByVal prCurve As PrecisionRecallCurve) As Component

			Dim divLeft As New ComponentDiv(INNER_DIV_STYLE, getPrecisionRecallCurve(precisionRecallTitle, prCurve))
			Dim divRight As New ComponentDiv(INNER_DIV_STYLE, getPrecisionRecallVsThreshold(prThresholdTitle, prCurve))

			Return New ComponentDiv(OUTER_DIV_STYLE, divLeft, divRight)
		End Function

		Private Shared Function getPrecisionRecallCurve(ByVal title As String, ByVal prCurve As PrecisionRecallCurve) As Component
			Dim recallX() As Double = prCurve.getRecall()
			Dim precisionY() As Double = prCurve.getPrecision()

			Return (New ChartLine.Builder(title, CHART_STYLE)).setXMin(0.0).setXMax(1.0).setYMin(0.0).setYMax(1.0).addSeries("P vs R", recallX, precisionY).build()
		End Function

		Private Shared Function getPrecisionRecallVsThreshold(ByVal title As String, ByVal prCurve As PrecisionRecallCurve) As Component

			Dim recallY() As Double = prCurve.getRecall()
			Dim precisionY() As Double = prCurve.getPrecision()
			Dim thresholdX() As Double = prCurve.getThreshold()

			Return (New ChartLine.Builder(title, CHART_STYLE_PRECISION_RECALL)).setXMin(0.0).setXMax(1.0).setYMin(0.0).setYMax(1.0).addSeries("Precision", thresholdX, precisionY).addSeries("Recall", thresholdX, recallY).showLegend(True).build()
		End Function

		Private Shared Function getHistogram(ByVal histogram As Histogram) As Component
			Dim chb As New ChartHistogram.Builder(histogram.Title, CHART_STYLE)
			Dim lower() As Double = histogram.BinLowerBounds
			Dim upper() As Double = histogram.BinUpperBounds
			Dim counts() As Integer = histogram.BinCounts
			For i As Integer = 0 To counts.Length - 1
				chb.addBin(lower(i), upper(i), counts(i))
			Next i

			Return chb.build()
		End Function
	End Class

End Namespace