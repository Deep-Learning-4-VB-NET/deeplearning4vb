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
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports CategoricalMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.CategoricalMetaData
Imports ColumnMetaDataT = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports NumberFormatExceptionT = DeepLearning4VB.JavaCompat.NumberFormatException
Imports RenderableComponentLineChart = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponentLineChart
Imports RenderableComponentTable = DeepLearning4VB.org.datavec.api.transform.ui.components.RenderableComponentTable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.ui

    ''' <summary>
    ''' Renders one sequence as a self-contained HTML page: a table of the schema, then one line chart per column over
    ''' the time steps. Numerical and time columns are plotted by value; categorical columns by the index of the state
    ''' (the chart title lists the index of each state, as in Java); other columns (string, bytes, NDArray) are listed
    ''' as not plotted. Missing (null) and unparseable values leave a gap in the line.
    ''' <para>Java rendered a FreeMarker template (<c>templates/sequenceplot.ftl</c>) with JavaScript charts from the
    ''' DL4J UI; this port builds the page itself (inline CSS for light and dark, inline SVG, no scripts, no external
    ''' resources). The title and all names and state values are HTML-encoded.</para>
    ''' </summary>
    Public Class HtmlSequencePlotting

        Private Shared ReadOnly Separator As String = " " & ChrW(&HB7) & " "

        Private Sub New()

        End Sub

        ''' <summary>
        ''' Create a HTML file with plots for the given sequence.
        ''' </summary>
        ''' <param name="title">    Title of the page </param>
        ''' <param name="schema">   Schema for the data </param>
        ''' <param name="sequence"> Sequence to plot: one list of values (in schema column order) per time step </param>
        ''' <returns> HTML file as a string </returns>
        Public Shared Function createHtmlSequencePlots(ByVal title As String, ByVal schema As SchemaT, ByVal sequence As IList(Of IList(Of WritableT))) As String
            If schema Is Nothing Then Throw New ArgumentNullException(NameOf(schema))
            If sequence Is Nothing Then Throw New ArgumentNullException(NameOf(sequence))
            Dim lf As String = ControlChars.Lf
            Dim body As New StringBuilder()

            'First: create table for schema
            Dim n As Integer = schema.numColumns()
            Dim table()() As String = RectangularArrays.RectangularStringArray(n \ 2 + n Mod 2, 6) 'Number, name, type; 2 columns

            Dim meta As IList(Of ColumnMetaDataT) = schema.getColumnMetaData()
            For i As Integer = 0 To meta.Count - 1
                Dim o As Integer = i Mod 2
                table(i \ 2)(o * 3) = i.ToString(CultureInfo.InvariantCulture)
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

            Dim rct As RenderableComponentTable = (New RenderableComponentTable.Builder()).table(table).header("#", "Name", "Type", "#", "Name", "Type").
                colWidthsPercent(6, 30, 14, 6, 30, 14).build()
            body.Append("<section class=""dv-section"" id=""schema""><h2>Schema</h2>").Append(rct.renderHtml("dv-schema")).Append("</section>").Append(lf)

            'Create the plots
            Dim x(sequence.Count - 1) As Double
            For i As Integer = 0 To x.Length - 1
                x(i) = i
            Next i

            'The plots share the time-step axis: the same left margin keeps their plot areas aligned (wider for time labels)
            Dim leftMargin As Integer = If(meta.Any(Function(m) m.getColumnType() = ColumnTypeEnum.Time), 80, 56)
            Dim divs As IList(Of DivObject) = New List(Of DivObject)()
            Dim notPlotted As New List(Of String)()
            For i As Integer = 0 To n - 1
                Dim lineData() As Double
                Dim type As ColumnTypeEnum = meta(i).getColumnType()
                Select Case type
                    Case ColumnTypeEnum.Integer, ColumnTypeEnum.Long, ColumnTypeEnum.Double, ColumnTypeEnum.Float, ColumnTypeEnum.Time
                        lineData = New Double(sequence.Count - 1) {}
                        For j As Integer = 0 To lineData.Length - 1
                            lineData(j) = valueAt(sequence(j), i)
                        Next j
                    Case ColumnTypeEnum.Categorical
                        'This is a quick-and-dirty way to plot categorical variables as a line chart
                        Dim stateNames As IList(Of String) = categoricalStates(meta(i))
                        lineData = New Double(sequence.Count - 1) {}
                        For j As Integer = 0 To lineData.Length - 1
                            Dim w As WritableT = writableAt(sequence(j), i)
                            Dim idx As Integer = If(w Is Nothing, -1, stateNames.IndexOf(w.ToString()))
                            lineData(j) = If(w Is Nothing, Double.NaN, idx)
                        Next j
                    Case Else
                        'Skip
                        notPlotted.Add(meta(i).getName() & " (" & type.ToString() & ")")
                        Continue For
                End Select

                Dim name As String = meta(i).getName()

                Dim chartTitle As String = "Column: """ & name & """ - Column Type: " & type.ToString()
                If type = ColumnTypeEnum.Categorical Then
                    Dim stateNames As IList(Of String) = categoricalStates(meta(i))
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

                'The title names the single series, so the chart needs no legend
                Dim lc As RenderableComponentLineChart = (New RenderableComponentLineChart.Builder()).title(chartTitle).addSeries(name, x, lineData).
                    margins(12, 32, leftMargin, 16).legend(False).build()
                lc.timeValues = (type = ColumnTypeEnum.Time)

                Dim divname As String = "plot_" & i.ToString(CultureInfo.InvariantCulture)
                divs.Add(New DivObject(divname, lc.toHtml()))
            Next i

            body.Append("<section class=""dv-section"" id=""plots""><h2>Plots</h2>").Append(lf)
            If divs.Count = 0 Then body.Append("<p class=""dv-note"">No column of this sequence can be plotted.</p>").Append(lf)
            For Each div As DivObject In divs
                body.Append("<article class=""dv-card"" id=""").Append(HtmlEncode(div.getId())).Append(""">").Append(div.getContent()).Append("</article>").Append(lf)
            Next
            If notPlotted.Count > 0 Then
                body.Append("<p class=""dv-note"">Not plotted: ").Append(HtmlEncode(String.Join(", ", notPlotted))).Append("</p>").Append(lf)
            End If
            body.Append("</section>").Append(lf)

            Dim metaLine As String = FormatCount(sequence.Count) & If(sequence.Count = 1, " time step", " time steps") & Separator &
                                     FormatCount(n) & If(n = 1, " column", " columns") & Separator & "generated " & FormatGeneratedTime()
            Return BuildPage(If(title, ""), HtmlEncode(metaLine), body.ToString())
        End Function

        ''' <summary>
        ''' Create a HTML file with plots for the given sequence and write it to a file (UTF-8 without byte order mark;
        ''' missing parent directories are created).
        ''' </summary>
        ''' <param name="title">    Title of the page </param>
        ''' <param name="schema">   Schema for the data </param>
        ''' <param name="sequence"> Sequence to plot </param>
        ''' <param name="output">   Output file path (should have extension .html) </param>
        Public Shared Sub createHtmlSequencePlotFile(ByVal title As String, ByVal schema As SchemaT, ByVal sequence As IList(Of IList(Of WritableT)), ByVal output As String)
            Dim s As String = createHtmlSequencePlots(title, schema, sequence)
            WriteReport(output, s)
        End Sub

        Private Shared Function categoricalStates(ByVal meta As ColumnMetaDataT) As IList(Of String)
            Dim cm As CategoricalMetaDataT = TryCast(meta, CategoricalMetaDataT)
            If cm Is Nothing OrElse cm.getStateNames() Is Nothing Then Return New List(Of String)()
            Return cm.getStateNames()
        End Function

        Private Shared Function writableAt(ByVal timeStep As IList(Of WritableT), ByVal column As Integer) As WritableT
            If timeStep Is Nothing OrElse column >= timeStep.Count Then Return Nothing
            Return timeStep(column)
        End Function

        ''' <summary>The value of a numerical column at one time step; NaN (a gap in the line) when it is missing or not a number.</summary>
        Private Shared Function valueAt(ByVal timeStep As IList(Of WritableT), ByVal column As Integer) As Double
            Dim w As WritableT = writableAt(timeStep, column)
            If w Is Nothing Then Return Double.NaN
            Try
                Return w.toDouble()
            Catch e As NumberFormatExceptionT
                Return Double.NaN
            Catch e As NotSupportedException
                'NullWritable
                Return Double.NaN
            End Try
        End Function

    End Class

End Namespace
