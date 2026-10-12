Imports System
Imports System.Collections.Generic
Imports Configuration = freemarker.template.Configuration
Imports Template = freemarker.template.Template
Imports TemplateExceptionHandler = freemarker.template.TemplateExceptionHandler
Imports Version = freemarker.template.Version
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports SequenceDataAnalysis = org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports org.datavec.api.transform.analysis.columns
Imports SequenceLengthAnalysis = org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports Schema = org.datavec.api.transform.schema.Schema
Imports RenderableComponentHistogram = org.datavec.api.transform.ui.components.RenderableComponentHistogram
Imports RenderableComponentTable = org.datavec.api.transform.ui.components.RenderableComponentTable
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports DateTimeFormat = org.joda.time.format.DateTimeFormat
Imports DateTimeFormatter = org.joda.time.format.DateTimeFormatter
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature

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

Namespace org.datavec.api.transform.ui


	Public Class HtmlAnalysis

		Private Sub New()

		End Sub

		''' <summary>
		''' Render a data analysis object as a HTML file. This will produce a summary table, along charts for
		''' numerical columns. The contents of the HTML file are returned as a String, which should be written
		''' to a .html file.
		''' </summary>
		''' <param name="analysis"> Data analysis object to render </param>
		''' <seealso cref= #createHtmlAnalysisFile(DataAnalysis, File) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String createHtmlAnalysisString(org.datavec.api.transform.analysis.DataAnalysis analysis) throws Exception
		Public Shared Function createHtmlAnalysisString(ByVal analysis As DataAnalysis) As String
			Dim cfg As New Configuration(New Version(2, 3, 23))

			' Where do we load the templates from:
			cfg.setClassForTemplateLoading(GetType(HtmlAnalysis), "/templates/")

			' Some other recommended settings:
			cfg.setIncompatibleImprovements(New Version(2, 3, 23))
			cfg.setDefaultEncoding("UTF-8")
			cfg.setLocale(Locale.US)
			cfg.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER)


			Dim input As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()

			Dim ret As New ObjectMapper()
			ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			ret.enable(SerializationFeature.INDENT_OUTPUT)

			Dim caList As IList(Of ColumnAnalysis) = analysis.getColumnAnalysis()
			Dim schema As Schema = analysis.getSchema()

			Dim sda As SequenceDataAnalysis = Nothing
			Dim hasSLA As Boolean = False
			If TypeOf analysis Is SequenceDataAnalysis Then
				sda = DirectCast(analysis, SequenceDataAnalysis)
				hasSLA = sda.getSequenceLengthAnalysis() IsNot Nothing
			End If


			Dim n As Integer = caList.Count
			If hasSLA Then
				n += 1
			End If
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim table[][] As String = new String[n][3]
			Dim table()() As String = RectangularArrays.RectangularStringArray(n, 3)

			Dim divs As IList(Of DivObject) = New List(Of DivObject)()
			Dim histogramDivNames As IList(Of String) = New List(Of String)()

			'Render sequence length analysis, if required:
			If hasSLA Then
				Dim seqLength As SequenceLengthAnalysis = sda.getSequenceLengthAnalysis()
				Dim name As String = "Sequence Lengths"

				table(0)(0) = name
				table(0)(1) = "(Seq Length)"
				table(0)(2) = seqLength.ToString().replaceAll(",", ", ") 'Hacky work-around to improve display in HTML table
				table(0)(2) = table(0)(2).replaceAll(" -> ", " : ") 'Quantiles rendering

				Dim buckets() As Double = seqLength.getHistogramBuckets()
				Dim counts() As Long = seqLength.getHistogramBucketCounts()


				If buckets IsNot Nothing Then
					Dim histBuilder As New RenderableComponentHistogram.Builder()
					For j As Integer = 0 To counts.Length - 1
						histBuilder.addBin(buckets(j), buckets(j + 1), counts(j))
					Next j
					histBuilder.margins(60, 60, 90, 20)

					Dim hist As RenderableComponentHistogram = histBuilder.title(name).build()

					Dim divName As String = "histdiv_" & name.replaceAll("\W", "")
					divs.Add(New DivObject(divName, ret.writeValueAsString(hist)))
					histogramDivNames.Add(divName)
				End If
			End If

			For i As Integer = 0 To caList.Count - 1
				Dim ca As ColumnAnalysis = caList(i)
				Dim name As String = schema.getName(i) 'namesList.get(i);
				Dim type As ColumnType = schema.getType(i)

				Dim idx As Integer = i + (If(sda IsNot Nothing AndAlso sda.getSequenceLengthAnalysis() IsNot Nothing, 1, 0))
				table(idx)(0) = name
				table(idx)(1) = type.ToString()
				table(idx)(2) = ca.ToString().replaceAll(",", ", ") 'Hacky work-around to improve display in HTML table
				table(idx)(2) = table(idx)(2).replaceAll(" -> ", " : ") 'Quantiles rendering
				Dim buckets() As Double
				Dim counts() As Long

				Select Case type.innerEnumValue
					Case ColumnType.InnerEnum.String
						Dim sa As StringAnalysis = DirectCast(ca, StringAnalysis)
						buckets = sa.getHistogramBuckets()
						counts = sa.getHistogramBucketCounts()
					Case Integer?
						Dim ia As IntegerAnalysis = DirectCast(ca, IntegerAnalysis)
						buckets = ia.getHistogramBuckets()
						counts = ia.getHistogramBucketCounts()
					Case Long?
						Dim la As LongAnalysis = DirectCast(ca, LongAnalysis)
						buckets = la.getHistogramBuckets()
						counts = la.getHistogramBucketCounts()
					Case Double?
						Dim da As DoubleAnalysis = DirectCast(ca, DoubleAnalysis)
						buckets = da.getHistogramBuckets()
						counts = da.getHistogramBucketCounts()
					Case ColumnType.InnerEnum.NDArray
						Dim na As NDArrayAnalysis = DirectCast(ca, NDArrayAnalysis)
						buckets = na.getHistogramBuckets()
						counts = na.getHistogramBucketCounts()
					Case ColumnType.InnerEnum.Categorical, Time, Bytes
						buckets = Nothing
						counts = Nothing
					Case Else
						Throw New Exception("Invalid/unknown column type: " & type)
				End Select

				If buckets IsNot Nothing Then
					Dim histBuilder As New RenderableComponentHistogram.Builder()

					For j As Integer = 0 To counts.Length - 1
						histBuilder.addBin(buckets(j), buckets(j + 1), counts(j))
					Next j

					histBuilder.margins(60, 60, 90, 20)

					Dim hist As RenderableComponentHistogram = histBuilder.title(name).build()

					Dim divName As String = "histdiv_" & name.replaceAll("\W", "")
					divs.Add(New DivObject(divName, ret.writeValueAsString(hist)))
					histogramDivNames.Add(divName)
				End If
			Next i

			'Create the summary table
			Dim rct As RenderableComponentTable = (New RenderableComponentTable.Builder()).table(table).header("Column Name", "Column Type", "Column Analysis").backgroundColor("#FFFFFF").headerColor("#CCCCCC").colWidthsPercent(20, 10, 70).border(1).padLeftPx(4).padRightPx(4).build()

			divs.Add(New DivObject("tablesource", ret.writeValueAsString(rct)))

			input("divs") = divs
			input("histogramIDs") = histogramDivNames

			'Current date/time, UTC
			Dim formatter As DateTimeFormatter = DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss zzz").withZone(DateTimeZone.UTC)
			Dim currTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim dateTime As String = formatter.print(currTime)
			input("datetime") = dateTime

			Dim template As Template = cfg.getTemplate("analysis.ftl")

			'Process template to String
			Dim stringWriter As Writer = New StringWriter()
			template.process(input, stringWriter)

			Return stringWriter.ToString()
		End Function

		''' <summary>
		''' Render a data analysis object as a HTML file. This will produce a summary table, along charts for
		''' numerical columns
		''' </summary>
		''' <param name="dataAnalysis"> Data analysis object to render </param>
		''' <param name="output">       Output file (should have extension .html) </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void createHtmlAnalysisFile(org.datavec.api.transform.analysis.DataAnalysis dataAnalysis, java.io.File output) throws Exception
'JAVA TO VB CONVERTER NOTE: The parameter dataAnalysis was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Sub createHtmlAnalysisFile(ByVal dataAnalysis_Conflict As DataAnalysis, ByVal output As File)

			Dim str As String = createHtmlAnalysisString(dataAnalysis_Conflict)

			FileUtils.writeStringToFile(output, str, StandardCharsets.UTF_8)
		End Sub

	End Class

End Namespace