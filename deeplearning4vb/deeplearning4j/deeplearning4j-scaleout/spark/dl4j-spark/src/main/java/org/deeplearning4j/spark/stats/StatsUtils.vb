Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports Component = org.deeplearning4j.ui.api.Component
Imports LengthUnit = org.deeplearning4j.ui.api.LengthUnit
Imports ChartHistogram = org.deeplearning4j.ui.components.chart.ChartHistogram
Imports ChartLine = org.deeplearning4j.ui.components.chart.ChartLine
Imports ChartTimeline = org.deeplearning4j.ui.components.chart.ChartTimeline
Imports StyleChart = org.deeplearning4j.ui.components.chart.style.StyleChart
Imports ComponentDiv = org.deeplearning4j.ui.components.component.ComponentDiv
Imports StyleDiv = org.deeplearning4j.ui.components.component.style.StyleDiv
Imports ComponentText = org.deeplearning4j.ui.components.text.ComponentText
Imports StyleText = org.deeplearning4j.ui.components.text.style.StyleText
Imports StaticPageUtil = org.deeplearning4j.ui.standalone.StaticPageUtil
Imports Tuple3 = scala.Tuple3

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

Namespace org.deeplearning4j.spark.stats


	Public Class StatsUtils

		Public Const DEFAULT_MAX_TIMELINE_SIZE_MS As Long = 20 * 60 * 1000 '20 minutes

		Private Sub New()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStats(java.util.List<EventStats> list, String outputDirectory, String filename, String delimiter, org.apache.spark.SparkContext sc) throws java.io.IOException
		Public Shared Sub exportStats(ByVal list As IList(Of EventStats), ByVal outputDirectory As String, ByVal filename As String, ByVal delimiter As String, ByVal sc As SparkContext)
			Dim path As String = FilenameUtils.concat(outputDirectory, filename)
			exportStats(list, path, delimiter, sc)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStats(java.util.List<EventStats> list, String outputPath, String delimiter, org.apache.spark.SparkContext sc) throws java.io.IOException
		Public Shared Sub exportStats(ByVal list As IList(Of EventStats), ByVal outputPath As String, ByVal delimiter As String, ByVal sc As SparkContext)
			Dim sb As New StringBuilder()
			Dim first As Boolean = True
			For Each e As EventStats In list
				If first Then
					sb.Append(e.getStringHeader(delimiter)).Append(vbLf)
				End If
				sb.Append(e.asString(delimiter)).Append(vbLf)
				first = False
			Next e
			SparkUtils.writeStringToFile(outputPath, sb.ToString(), sc)
		End Sub

		Public Shared Function getDurationAsString(ByVal list As IList(Of EventStats), ByVal delim As String) As String
			Dim sb As New StringBuilder()
			Dim num As Integer = list.Count
			Dim count As Integer = 0
			For Each e As EventStats In list
				sb.Append(e.DurationMs)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (count++ < num - 1)
				If count < num - 1 Then
						count += 1
					sb.Append(delim)
					Else
						count += 1
					End If
			Next e
			Return sb.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStatsAsHtml(org.deeplearning4j.spark.api.stats.SparkTrainingStats sparkTrainingStats, String path, org.apache.spark.api.java.JavaSparkContext sc) throws Exception
		Public Shared Sub exportStatsAsHtml(ByVal sparkTrainingStats As SparkTrainingStats, ByVal path As String, ByVal sc As JavaSparkContext)
			exportStatsAsHtml(sparkTrainingStats, path, sc.sc())
		End Sub

		''' <summary>
		''' Generate and export a HTML representation (including charts, etc) of the Spark training statistics<br>
		''' Note: exporting is done via Spark, so the path here can be a local file, HDFS, etc.
		''' </summary>
		''' <param name="sparkTrainingStats"> Stats to generate HTML page for </param>
		''' <param name="path">               Path to export. May be local or HDFS </param>
		''' <param name="sc">                 Spark context </param>
		''' <exception cref="Exception"> IO errors or error generating HTML file </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStatsAsHtml(org.deeplearning4j.spark.api.stats.SparkTrainingStats sparkTrainingStats, String path, org.apache.spark.SparkContext sc) throws Exception
		Public Shared Sub exportStatsAsHtml(ByVal sparkTrainingStats As SparkTrainingStats, ByVal path As String, ByVal sc As SparkContext)
			exportStatsAsHtml(sparkTrainingStats, DEFAULT_MAX_TIMELINE_SIZE_MS, path, sc)
		End Sub

		''' <summary>
		''' Generate and export a HTML representation (including charts, etc) of the Spark training statistics<br>
		''' Note: exporting is done via Spark, so the path here can be a local file, HDFS, etc.
		''' </summary>
		''' <param name="sparkTrainingStats"> Stats to generate HTML page for </param>
		''' <param name="path">               Path to export. May be local or HDFS </param>
		''' <param name="maxTimelineSizeMs">  maximum amount of activity to show in a single timeline plot (multiple plots will be used if training exceeds this amount of time) </param>
		''' <param name="sc">                 Spark context </param>
		''' <exception cref="Exception"> IO errors or error generating HTML file </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStatsAsHtml(org.deeplearning4j.spark.api.stats.SparkTrainingStats sparkTrainingStats, long maxTimelineSizeMs, String path, org.apache.spark.SparkContext sc) throws Exception
		Public Shared Sub exportStatsAsHtml(ByVal sparkTrainingStats As SparkTrainingStats, ByVal maxTimelineSizeMs As Long, ByVal path As String, ByVal sc As SparkContext)
			Dim fileSystem As FileSystem = FileSystem.get(sc.hadoopConfiguration())
			Using bos As New java.io.BufferedOutputStream(fileSystem.create(New org.apache.hadoop.fs.Path(path)))
				exportStatsAsHTML(sparkTrainingStats, maxTimelineSizeMs, bos)
			End Using
		End Sub

		''' <summary>
		''' Generate and export a HTML representation (including charts, etc) of the Spark training statistics<br>
		''' This overload is for writing to an output stream
		''' </summary>
		''' <param name="sparkTrainingStats"> Stats to generate HTML page for </param>
		''' <exception cref="Exception"> IO errors or error generating HTML file </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStatsAsHTML(org.deeplearning4j.spark.api.stats.SparkTrainingStats sparkTrainingStats, java.io.OutputStream outputStream) throws Exception
		Public Shared Sub exportStatsAsHTML(ByVal sparkTrainingStats As SparkTrainingStats, ByVal outputStream As Stream)
			exportStatsAsHTML(sparkTrainingStats, DEFAULT_MAX_TIMELINE_SIZE_MS, outputStream)
		End Sub

		''' <summary>
		''' Generate and export a HTML representation (including charts, etc) of the Spark training statistics<br>
		''' This overload is for writing to an output stream
		''' </summary>
		''' <param name="sparkTrainingStats"> Stats to generate HTML page for </param>
		''' <param name="maxTimelineSizeMs">  maximum amount of activity to show in a single timeline plot (multiple plots will be used if training exceeds this amount of time) </param>
		''' <exception cref="Exception"> IO errors or error generating HTML file </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void exportStatsAsHTML(org.deeplearning4j.spark.api.stats.SparkTrainingStats sparkTrainingStats, long maxTimelineSizeMs, java.io.OutputStream outputStream) throws Exception
		Public Shared Sub exportStatsAsHTML(ByVal sparkTrainingStats As SparkTrainingStats, ByVal maxTimelineSizeMs As Long, ByVal outputStream As Stream)
			Dim keySet As ISet(Of String) = sparkTrainingStats.getKeySet()

			Dim components As IList(Of Component) = New List(Of Component)()

			Dim styleChart As StyleChart = (New StyleChart.Builder()).backgroundColor(Color.WHITE).width(700, LengthUnit.Px).height(400, LengthUnit.Px).build()

			Dim styleText As StyleText = (New StyleText.Builder()).color(Color.BLACK).fontSize(20).build()
			Dim headerText As Component = New ComponentText("Deeplearning4j - Spark Training Analysis", styleText)
			Dim header As Component = New ComponentDiv((New StyleDiv.Builder()).height(40, LengthUnit.Px).width(100, LengthUnit.Percent).build(), headerText)
			components.Add(header)

			Dim keySetInclude As ISet(Of String) = New HashSet(Of String)()
			For Each s As String In keySet
				If sparkTrainingStats.defaultIncludeInPlots(s) Then
					keySetInclude.Add(s)
				End If
			Next s

			Collections.addAll(components, getTrainingStatsTimelineChart(sparkTrainingStats, keySetInclude, maxTimelineSizeMs))

			For Each s As String In keySet
				Dim list As IList(Of EventStats) = New List(Of EventStats)(sparkTrainingStats.getValue(s))
				list.Sort(New StartTimeComparator())

				Dim x(list.Count - 1) As Double
				Dim duration(list.Count - 1) As Double
				Dim minDur As Double = Double.MaxValue
				Dim maxDur As Double = -Double.MaxValue
				For i As Integer = 0 To duration.Length - 1
					x(i) = i
					duration(i) = list(i).getDurationMs()
					minDur = Math.Min(minDur, duration(i))
					maxDur = Math.Max(maxDur, duration(i))
				Next i

				Dim line As Component = (New ChartLine.Builder(s, styleChart)).addSeries("Duration", x, duration).setYMin(If(minDur = maxDur, minDur - 1, Nothing)).setYMax(If(minDur = maxDur, minDur + 1, Nothing)).build()

				'Also build a histogram...
				Dim hist As Component = Nothing
				If minDur <> maxDur AndAlso list.Count > 0 Then
					hist = getHistogram(duration, 20, s, styleChart)
				End If

				Dim temp() As Component
				If hist IsNot Nothing Then
					temp = New Component() {line, hist}
				Else
					temp = New Component() {line}
				End If

				components.Add(New ComponentDiv((New StyleDiv.Builder()).width(100, LengthUnit.Percent).build(), temp))


				'TODO this is really ugly
				If list.Count > 0 AndAlso (TypeOf list(0) Is ExampleCountEventStats OrElse TypeOf list(0) Is PartitionCountEventStats) Then
					Dim exCount As Boolean = TypeOf list(0) Is ExampleCountEventStats

					Dim y(list.Count - 1) As Double
					Dim miny As Double = Double.MaxValue
					Dim maxy As Double = -Double.MaxValue
					For i As Integer = 0 To y.Length - 1
						y(i) = (If(exCount, DirectCast(list(i), ExampleCountEventStats).getTotalExampleCount(), DirectCast(list(i), PartitionCountEventStats).getNumPartitions()))
						miny = Math.Min(miny, y(i))
						maxy = Math.Max(maxy, y(i))
					Next i

					Dim title As String = s & " / " & (If(exCount, "Number of Examples", "Number of Partitions"))
					Dim line2 As Component = (New ChartLine.Builder(title, styleChart)).addSeries((If(exCount, "Examples", "Partitions")), x, y).setYMin(If(miny = maxy, miny - 1, Nothing)).setYMax(If(miny = maxy, miny + 1, Nothing)).build()


					'Also build a histogram...
					Dim hist2 As Component = Nothing
					If miny <> maxy Then
						hist2 = getHistogram(y, 20, title, styleChart)
					End If

					Dim temp2() As Component
					If hist2 IsNot Nothing Then
						temp2 = New Component() {line2, hist2}
					Else
						temp2 = New Component() {line2}
					End If

					components.Add(New ComponentDiv((New StyleDiv.Builder()).width(100, LengthUnit.Percent).build(), temp2))
				End If
			Next s

			Dim html As String = StaticPageUtil.renderHTML(components)
			outputStream.WriteByte(html.GetBytes(Encoding.UTF8))
		End Sub


		Public Class StartTimeComparator
			Implements IComparer(Of EventStats)

			Public Overridable Function Compare(ByVal o1 As EventStats, ByVal o2 As EventStats) As Integer Implements IComparer(Of EventStats).Compare
				Return Long.compare(o1.StartTime, o2.StartTime)
			End Function
		End Class


		Private Shared Function getTrainingStatsTimelineChart(ByVal stats As SparkTrainingStats, ByVal includeSet As ISet(Of String), ByVal maxDurationMs As Long) As Component()
			Dim uniqueTuples As ISet(Of Tuple3(Of String, String, Long)) = New HashSet(Of Tuple3(Of String, String, Long))()
			Dim machineIDs As ISet(Of String) = New HashSet(Of String)()
			Dim jvmIDs As ISet(Of String) = New HashSet(Of String)()

			Dim machineShortNames As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			Dim jvmShortNames As IDictionary(Of String, String) = New Dictionary(Of String, String)()

			Dim earliestStart As Long = Long.MaxValue
			Dim latestEnd As Long = Long.MinValue
			For Each s As String In includeSet
				Dim list As IList(Of EventStats) = stats.getValue(s)
				For Each e As EventStats In list
					machineIDs.Add(e.MachineID)
					jvmIDs.Add(e.JvmID)
					uniqueTuples.Add(New Tuple3(Of String, String, Long)(e.MachineID, e.JvmID, e.ThreadID))
					earliestStart = Math.Min(earliestStart, e.StartTime)
					latestEnd = Math.Max(latestEnd, e.StartTime + e.DurationMs)
				Next e
			Next s
			Dim count As Integer = 0
			For Each s As String In machineIDs
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: machineShortNames.put(s, "PC " + count++);
				machineShortNames(s) = "PC " & count
					count += 1
			Next s
			count = 0
			For Each s As String In jvmIDs
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: jvmShortNames.put(s, "JVM " + count++);
				jvmShortNames(s) = "JVM " & count
					count += 1
			Next s

			Dim nLanes As Integer = uniqueTuples.Count
			Dim outputOrder As IList(Of Tuple3(Of String, String, Long)) = New List(Of Tuple3(Of String, String, Long))(uniqueTuples)
			outputOrder.Sort(New TupleComparator())

			Dim colors() As Color = getColors(includeSet.Count)
			Dim colorMap As IDictionary(Of String, Color) = New Dictionary(Of String, Color)()
			count = 0
			For Each s As String In includeSet
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: colorMap.put(s, colors[count++]);
				colorMap(s) = colors(count)
					count += 1
			Next s

			'Create key for charts:
			Dim tempList As IList(Of Component) = New List(Of Component)()
			For Each s As String In includeSet
				Dim key As String = stats.getShortNameForKey(s) & " - " & s

				tempList.Add(New ComponentDiv((New StyleDiv.Builder()).backgroundColor(colorMap(s)).width(33.3, LengthUnit.Percent).height(25, LengthUnit.Px).floatValue(StyleDiv.FloatValue.left).build(), New ComponentText(key, (New StyleText.Builder()).fontSize(11).build())))
			Next s
			Dim key As Component = New ComponentDiv((New StyleDiv.Builder()).width(100, LengthUnit.Percent).build(), tempList)

			'How many charts?
			Dim nCharts As Integer = CInt((latestEnd - earliestStart) \ maxDurationMs)
			If nCharts < 1 Then
				nCharts = 1
			End If
			Dim chartStartTimes(nCharts - 1) As Long
			Dim chartEndTimes(nCharts - 1) As Long
			For i As Integer = 0 To nCharts - 1
				chartStartTimes(i) = earliestStart + i * maxDurationMs
				chartEndTimes(i) = earliestStart + (i + 1) * maxDurationMs
			Next i


			Dim entriesByLane As IList(Of IList(Of IList(Of ChartTimeline.TimelineEntry))) = New List(Of IList(Of IList(Of ChartTimeline.TimelineEntry)))()
			For c As Integer = 0 To nCharts - 1
				entriesByLane.Add(New List(Of IList(Of ChartTimeline.TimelineEntry))())
				For i As Integer = 0 To nLanes - 1
					entriesByLane(c).Add(New List(Of ChartTimeline.TimelineEntry)())
				Next i
			Next c

			For Each s As String In includeSet

				Dim list As IList(Of EventStats) = stats.getValue(s)
				For Each e As EventStats In list
					If e.DurationMs = 0 Then
						Continue For
					End If

					Dim start As Long = e.StartTime
					Dim [end] As Long = start + e.DurationMs

					Dim chartIdx As Integer = -1
					For j As Integer = 0 To nCharts - 1
						If start >= chartStartTimes(j) AndAlso start < chartEndTimes(j) Then
							chartIdx = j
						End If
					Next j
					If chartIdx = -1 Then
						chartIdx = nCharts - 1
					End If


					Dim tuple As New Tuple3(Of String, String, Long)(e.MachineID, e.JvmID, e.ThreadID)

					Dim idx As Integer = outputOrder.IndexOf(tuple)
					Dim c As Color = colorMap(s)
					'                ChartTimeline.TimelineEntry entry = new ChartTimeline.TimelineEntry(null, start, end, c);
					Dim entry As New ChartTimeline.TimelineEntry(stats.getShortNameForKey(s), start, [end], c)
					entriesByLane(chartIdx)(idx).Add(entry)
				Next e
			Next s

			'Sort each lane by start time:
			For i As Integer = 0 To nCharts - 1
				For Each l As IList(Of ChartTimeline.TimelineEntry) In entriesByLane(i)
					l.Sort(New ComparatorAnonymousInnerClass())
				Next l
			Next i

			Dim sc As StyleChart = (New StyleChart.Builder()).width(1280, LengthUnit.Px).height(35 * nLanes + (60 + 20 + 25), LengthUnit.Px).margin(LengthUnit.Px, 60, 20, 200, 10).build()

			Dim list As IList(Of Component) = New List(Of Component)(nCharts)
			For j As Integer = 0 To nCharts - 1
				Dim b As New ChartTimeline.Builder("Timeline: Training Activities", sc)
				Dim i As Integer = 0
				For Each l As IList(Of ChartTimeline.TimelineEntry) In entriesByLane(j)
					Dim t3 As Tuple3(Of String, String, Long) = outputOrder(i)
					Dim name As String = machineShortNames(t3._1()) & ", " & jvmShortNames(t3._2()) & ", Thread " & t3._3()
					b.addLane(name, l)
					i += 1
				Next l
				list.Add(b.build())
			Next j

			list.Add(key)

			Return CType(list, List(Of Component)).ToArray()
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of ChartTimeline.TimelineEntry)

			Public Function Compare(ByVal o1 As ChartTimeline.TimelineEntry, ByVal o2 As ChartTimeline.TimelineEntry) As Integer Implements IComparer(Of ChartTimeline.TimelineEntry).Compare
				Return Long.compare(o1.getStartTimeMs(), o2.getStartTimeMs())
			End Function
		End Class

		Private Class TupleComparator
			Implements IComparer(Of Tuple3(Of String, String, Long))

			Public Overridable Function Compare(ByVal o1 As Tuple3(Of String, String, Long), ByVal o2 As Tuple3(Of String, String, Long)) As Integer Implements IComparer(Of Tuple3(Of String, String, Long)).Compare
				If o1._1().Equals(o2._1()) Then
					'Equal machine IDs, so sort on JVM ids
					If o1._2().Equals(o2._2()) Then
						'Equal machine AND JVM IDs, so sort on thread ID
						Return Long.compare(o1._3(), o2._3())
					Else
						Return o1._2().compareTo(o2._2())
					End If
				Else
					Return o1._1().compareTo(o2._1())
				End If
			End Function
		End Class

		Private Shared Function getColors(ByVal nColors As Integer) As Color()
			Dim c(nColors - 1) As Color
			Dim [step] As Double
			If nColors <= 1 Then
				[step] = 1.0
			Else
				[step] = 1.0 / (nColors + 1)
			End If
			For i As Integer = 0 To nColors - 1
				'            c[i] = Color.getHSBColor((float) step * i, 0.4f, 0.75f);   //step hue; fixed saturation + variance to (hopefully) ensure readability of labels
				If i Mod 2 = 0 Then
					c(i) = Color.getHSBColor(CSng([step]) * i, 0.4f, 0.75f) 'step hue; fixed saturation + variance to (hopefully) ensure readability of labels
				Else
					c(i) = Color.getHSBColor(CSng([step]) * i, 1.0f, 1.0f) 'step hue; fixed saturation + variance to (hopefully) ensure readability of labels
				End If
			Next i
			Return c
		End Function

		Private Shared Function getHistogram(ByVal data() As Double, ByVal nBins As Integer, ByVal title As String, ByVal styleChart As StyleChart) As Component
			Dim min As Double = Double.MaxValue
			Dim max As Double = -Double.MaxValue
			For Each d As Double In data
				min = Math.Min(min, d)
				max = Math.Max(max, d)
			Next d

			If min = max Then
				Return Nothing
			End If
			Dim bins(nBins) As Double
			Dim counts(nBins - 1) As Integer
			Dim [step] As Double = (max - min) / nBins
			For i As Integer = 0 To bins.Length - 1
				bins(i) = min + i * [step]
			Next i

			For Each d As Double In data
				For i As Integer = 0 To bins.Length - 2
					If d >= bins(i) AndAlso d < bins(i + 1) Then
						counts(i) += 1
						Exit For
					End If
				Next i
				If d = bins(bins.Length - 1) Then
					counts(counts.Length - 1) += 1
				End If
			Next d

			Dim b As New ChartHistogram.Builder(title, styleChart)
			For i As Integer = 0 To bins.Length - 2
				b.addBin(bins(i), bins(i + 1), counts(i))
			Next i

			Return b.build()
		End Function
	End Class

End Namespace