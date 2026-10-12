Imports System.Collections.Generic
Imports System.Text
Imports Configuration = freemarker.template.Configuration
Imports Template = freemarker.template.Template
Imports TemplateExceptionHandler = freemarker.template.TemplateExceptionHandler
Imports Version = freemarker.template.Version
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports RenderableComponentLineChart = org.datavec.api.transform.ui.components.RenderableComponentLineChart
Imports RenderableComponentTable = org.datavec.api.transform.ui.components.RenderableComponentTable
Imports Writable = org.datavec.api.writable.Writable
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


	Public Class HtmlSequencePlotting

		Private Sub New()

		End Sub

		''' <summary>
		''' Create a HTML file with plots for the given sequence.
		''' </summary>
		''' <param name="title">    Title of the page </param>
		''' <param name="schema">   Schema for the data </param>
		''' <param name="sequence"> Sequence to plot </param>
		''' <returns> HTML file as a string </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String createHtmlSequencePlots(String title, org.datavec.api.transform.schema.Schema schema, List<List<org.datavec.api.writable.Writable>> sequence) throws Exception
		Public Shared Function createHtmlSequencePlots(ByVal title As String, ByVal schema As Schema, ByVal sequence As IList(Of IList(Of Writable))) As String
			Dim cfg As New Configuration(New Version(2, 3, 23))

			' Where do we load the templates from:
			cfg.setClassForTemplateLoading(GetType(HtmlSequencePlotting), "/templates/")

			' Some other recommended settings:
			cfg.setIncompatibleImprovements(New Version(2, 3, 23))
			cfg.setDefaultEncoding("UTF-8")
			cfg.setLocale(Locale.US)
			cfg.setTemplateExceptionHandler(TemplateExceptionHandler.RETHROW_HANDLER)


			Dim input As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			input("pagetitle") = title

			Dim ret As New ObjectMapper()
			ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			ret.enable(SerializationFeature.INDENT_OUTPUT)

			Dim divs As IList(Of DivObject) = New List(Of DivObject)()
			Dim divNames As IList(Of String) = New List(Of String)()

			'First: create table for schema
			Dim n As Integer = schema.numColumns()
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim table[][] As String = new String[n \ 2 + n % 2][6] 'Number, name, type; 2 columns
			Dim table()() As String = RectangularArrays.RectangularStringArray(n \ 2 + n Mod 2, 6) 'Number, name, type; 2 columns

			Dim meta As IList(Of ColumnMetaData) = schema.getColumnMetaData()
			For i As Integer = 0 To meta.Count - 1
				Dim o As Integer = i Mod 2
				table(i \ 2)(o * 3) = i.ToString()
				table(i \ 2)(o * 3 + 1) = meta(i).getName()
				table(i \ 2)(o * 3 + 2) = meta(i).getColumnType().ToString()
			Next i

			For i As Integer = 0 To table.Length - 1
				Dim j As Integer = 0
				Do While j < table(i).Length
					If table(i)(j) Is Nothing Then
						table(i)(j) = ""
					End If
					j += 1
				Loop
			Next i


			Dim rct As RenderableComponentTable = (New RenderableComponentTable.Builder()).table(table).header("#", "Name", "Type", "#", "Name", "Type").backgroundColor("#FFFFFF").headerColor("#CCCCCC").colWidthsPercent(8, 30, 12, 8, 30, 12).border(1).padLeftPx(4).padRightPx(4).build()
			divs.Add(New DivObject("tablesource", ret.writeValueAsString(rct)))

			'Create the plots
			Dim x(sequence.Count - 1) As Double
			For i As Integer = 0 To x.Length - 1
				x(i) = i
			Next i

			For i As Integer = 0 To n - 1
				Dim lineData() As Double
				Select Case meta(i).getColumnType()
					Case Integer?, System.Nullable<Long>, System.Nullable<Double>, System.Nullable<Single>, Time
						lineData = New Double(sequence.Count - 1){}
						For j As Integer = 0 To lineData.Length - 1
							lineData(j) = sequence(j)(i).toDouble()
						Next j
					Case Categorical
						'This is a quick-and-dirty way to plot categorical variables as a line chart
						Dim stateNames As IList(Of String) = DirectCast(meta(i), CategoricalMetaData).getStateNames()
						lineData = New Double(sequence.Count - 1){}
						For j As Integer = 0 To lineData.Length - 1
							Dim state As String = sequence(j)(i).ToString()
							Dim idx As Integer = stateNames.IndexOf(state)
							lineData(j) = idx
						Next j
					Case Else
						'Skip
						Continue For
				End Select

				Dim name As String = meta(i).getName()

				Dim chartTitle As String = "Column: """ & name & """ - Column Type: " & meta(i).getColumnType()
				If meta(i).getColumnType() = ColumnType.Categorical Then
					Dim stateNames As IList(Of String) = DirectCast(meta(i), CategoricalMetaData).getStateNames()
					Dim sb As New StringBuilder(chartTitle)
					sb.Append(" - (")
					For j As Integer = 0 To stateNames.Count - 1
						If j > 0 Then
							sb.Append(", ")
						End If
						sb.Append(j).Append("=").Append(stateNames(j))
					Next j
					sb.Append(")")
					chartTitle = sb.ToString()
				End If

				Dim lc As RenderableComponentLineChart = (New RenderableComponentLineChart.Builder()).title(chartTitle).addSeries(name, x, lineData).build()

				Dim divname As String = "plot_" & i

				divs.Add(New DivObject(divname, ret.writeValueAsString(lc)))
				divNames.Add(divname)
			Next i

			input("divs") = divs
			input("divnames") = divNames

			'Current date/time, UTC
			Dim formatter As DateTimeFormatter = DateTimeFormat.forPattern("YYYY-MM-dd HH:mm:ss zzz").withZone(DateTimeZone.UTC)
			Dim currTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim dateTime As String = formatter.print(currTime)
			input("datetime") = dateTime

			Dim template As Template = cfg.getTemplate("sequenceplot.ftl")

			'Process template to String
			Dim stringWriter As Writer = New StringWriter()
			template.process(input, stringWriter)

			Return stringWriter.ToString()
		End Function

		''' <summary>
		''' Create a HTML file with plots for the given sequence and write it to a file.
		''' </summary>
		''' <param name="title">    Title of the page </param>
		''' <param name="schema">   Schema for the data </param>
		''' <param name="sequence"> Sequence to plot </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void createHtmlSequencePlotFile(String title, org.datavec.api.transform.schema.Schema schema, List<List<org.datavec.api.writable.Writable>> sequence, java.io.File output) throws Exception
		Public Shared Sub createHtmlSequencePlotFile(ByVal title As String, ByVal schema As Schema, ByVal sequence As IList(Of IList(Of Writable)), ByVal output As File)
			Dim s As String = createHtmlSequencePlots(title, schema, sequence)
			FileUtils.writeStringToFile(output, s, StandardCharsets.UTF_8)
		End Sub
	End Class

End Namespace