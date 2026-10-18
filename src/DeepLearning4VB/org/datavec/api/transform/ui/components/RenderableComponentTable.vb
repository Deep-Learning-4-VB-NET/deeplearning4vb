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
Imports System.Text

Namespace org.datavec.api.transform.ui.components

    ''' <summary>
    ''' A table of strings with an optional title and header row, rendered as an HTML <c>&lt;table&gt;</c>.
    ''' <para>Styling: the cell padding, border width and column widths are honoured as given; when no padding or
    ''' border is set the report stylesheet applies its own. A background or header colour replaces the themed one
    ''' (and with a hex colour the text switches to black or white for contrast); colours that are not plain colour
    ''' values are ignored. Cells that hold a formatted number are right-aligned.</para>
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    Public Class RenderableComponentTable
        Inherits RenderableComponent

        Public Const COMPONENT_TYPE As String = "simpletable"

        Private title As String
        Private header() As String
        Private table()() As String
        Private padLeftPx As Integer = 0
        Private padRightPx As Integer = 0
        Private padTopPx As Integer = 0
        Private padBottomPx As Integer = 0
        Private border As Integer = 0
        Private colWidthsPercent() As Double = Nothing
        Private backgroundColor As String
        Private headerColor As String

        Public Sub New()
            MyBase.New(COMPONENT_TYPE)
            'No arg constructor for Jackson
        End Sub

        Public Sub New(ByVal builder As Builder)
            MyBase.New(COMPONENT_TYPE)
            Me.title = builder.titleValue
            Me.header = builder.headerValue
            Me.table = builder.tableValue
            Me.padLeftPx = builder.padLeftPxValue
            Me.padRightPx = builder.padRightPxValue
            Me.padTopPx = builder.padTopPxValue
            Me.padBottomPx = builder.padBottomPxValue
            Me.border = builder.borderValue
            Me.colWidthsPercent = builder.colWidthsPercentValue
            Me.backgroundColor = builder.backgroundColorValue
            Me.headerColor = builder.headerColorValue
        End Sub

        Public Sub New(ByVal header() As String, ByVal table()() As String)
            Me.New(Nothing, header, table)
        End Sub

        Public Sub New(ByVal title As String, ByVal header() As String, ByVal table()() As String)
            MyBase.New(COMPONENT_TYPE)
            Me.title = title
            Me.header = header
            Me.table = table
        End Sub

        Public Overrides Function toHtml() As String
            Return renderHtml(Nothing)
        End Function

        ''' <summary>Renders the table; <paramref name="cssClass"/> (trusted, report-internal) adds classes to the table element.</summary>
        Friend Function renderHtml(ByVal cssClass As String) As String
            Dim sb As New StringBuilder()
            Dim cellStyle As New StringBuilder()
            If padLeftPx > 0 OrElse padRightPx > 0 OrElse padTopPx > 0 OrElse padBottomPx > 0 Then
                cellStyle.Append("padding:").Append(Math.Max(0, padTopPx)).Append("px ").Append(Math.Max(0, padRightPx)).Append("px ").
                    Append(Math.Max(0, padBottomPx)).Append("px ").Append(Math.Max(0, padLeftPx)).Append("px;")
            End If
            If border > 0 Then
                cellStyle.Append("border:").Append(border).Append("px solid var(--dv-axis,").Append(AxisFallback).Append(");")
            End If

            Dim tableStyle As New StringBuilder()
            If IsSafeCssColor(backgroundColor) Then
                tableStyle.Append("background-color:").Append(backgroundColor).Append(";"c)
                Dim ink As String = ContrastingInk(backgroundColor)
                If ink IsNot Nothing Then tableStyle.Append("color:").Append(ink).Append(";"c)
            End If
            If border > 0 Then
                tableStyle.Append("border:").Append(border).Append("px solid var(--dv-axis,").Append(AxisFallback).Append(");")
            End If

            Dim headerStyle As New StringBuilder(cellStyle.ToString())
            If IsSafeCssColor(headerColor) Then
                headerStyle.Append("background-color:").Append(headerColor).Append(";"c)
                Dim ink As String = ContrastingInk(headerColor)
                If ink IsNot Nothing Then headerStyle.Append("color:").Append(ink).Append(";"c)
            End If

            sb.Append("<div class=""dv-table-wrap""><table class=""dv-table")
            If Not String.IsNullOrEmpty(cssClass) Then sb.Append(" "c).Append(cssClass)
            sb.Append(""""c)
            appendStyle(sb, tableStyle.ToString())
            sb.Append(">"c)
            If Not String.IsNullOrEmpty(title) Then
                sb.Append("<caption>").Append(HtmlEncode(title)).Append("</caption>")
            End If
            If colWidthsPercent IsNot Nothing AndAlso colWidthsPercent.Length > 0 Then
                sb.Append("<colgroup>")
                For Each w As Double In colWidthsPercent
                    If Double.IsNaN(w) OrElse Double.IsInfinity(w) OrElse w < 0 Then
                        sb.Append("<col>")
                    Else
                        sb.Append("<col style=""width:").Append(SvgNum(w)).Append("%"">")
                    End If
                Next
                sb.Append("</colgroup>")
            End If
            If header IsNot Nothing AndAlso header.Length > 0 Then
                sb.Append("<thead><tr>")
                For Each h As String In header
                    sb.Append("<th scope=""col""")
                    If LooksNumeric(h) Then sb.Append(" class=""dv-num""")
                    appendStyle(sb, headerStyle.ToString())
                    sb.Append(">"c).Append(HtmlEncode(h)).Append("</th>")
                Next
                sb.Append("</tr></thead>")
            End If
            sb.Append("<tbody>")
            If table IsNot Nothing Then
                For Each row As String() In table
                    sb.Append("<tr>")
                    If row IsNot Nothing Then
                        For Each cell As String In row
                            sb.Append("<td")
                            If LooksNumeric(cell) Then sb.Append(" class=""dv-num""")
                            appendStyle(sb, cellStyle.ToString())
                            sb.Append(">"c).Append(HtmlEncode(cell)).Append("</td>")
                        Next
                    End If
                    sb.Append("</tr>")
                Next
            End If
            sb.Append("</tbody></table></div>")
            Return sb.ToString()
        End Function

        Private Shared Sub appendStyle(ByVal sb As StringBuilder, ByVal style As String)
            If style.Length > 0 Then sb.Append(" style=""").Append(HtmlEncode(style)).Append(""""c)
        End Sub

        ' ---- Lombok @Data accessors ----

        Public Function getTitle() As String
            Return title
        End Function

        Public Sub setTitle(ByVal title As String)
            Me.title = title
        End Sub

        Public Function getHeader() As String()
            Return header
        End Function

        Public Sub setHeader(ByVal header() As String)
            Me.header = header
        End Sub

        Public Function getTable() As String()()
            Return table
        End Function

        Public Sub setTable(ByVal table()() As String)
            Me.table = table
        End Sub

        Public Function getPadLeftPx() As Integer
            Return padLeftPx
        End Function

        Public Sub setPadLeftPx(ByVal padLeftPx As Integer)
            Me.padLeftPx = padLeftPx
        End Sub

        Public Function getPadRightPx() As Integer
            Return padRightPx
        End Function

        Public Sub setPadRightPx(ByVal padRightPx As Integer)
            Me.padRightPx = padRightPx
        End Sub

        Public Function getPadTopPx() As Integer
            Return padTopPx
        End Function

        Public Sub setPadTopPx(ByVal padTopPx As Integer)
            Me.padTopPx = padTopPx
        End Sub

        Public Function getPadBottomPx() As Integer
            Return padBottomPx
        End Function

        Public Sub setPadBottomPx(ByVal padBottomPx As Integer)
            Me.padBottomPx = padBottomPx
        End Sub

        Public Function getBorder() As Integer
            Return border
        End Function

        Public Sub setBorder(ByVal border As Integer)
            Me.border = border
        End Sub

        Public Function getColWidthsPercent() As Double()
            Return colWidthsPercent
        End Function

        Public Sub setColWidthsPercent(ByVal colWidthsPercent() As Double)
            Me.colWidthsPercent = colWidthsPercent
        End Sub

        Public Function getBackgroundColor() As String
            Return backgroundColor
        End Function

        Public Sub setBackgroundColor(ByVal backgroundColor As String)
            Me.backgroundColor = backgroundColor
        End Sub

        Public Function getHeaderColor() As String
            Return headerColor
        End Function

        Public Sub setHeaderColor(ByVal headerColor As String)
            Me.headerColor = headerColor
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim other As RenderableComponentTable = DirectCast(obj, RenderableComponentTable)
            Return String.Equals(title, other.title) AndAlso Arrays.equals(header, other.header) AndAlso ArrayListEquals(table, other.table) AndAlso
                   padLeftPx = other.padLeftPx AndAlso padRightPx = other.padRightPx AndAlso padTopPx = other.padTopPx AndAlso
                   padBottomPx = other.padBottomPx AndAlso border = other.border AndAlso Arrays.equals(colWidthsPercent, other.colWidthsPercent) AndAlso
                   String.Equals(backgroundColor, other.backgroundColor) AndAlso String.Equals(headerColor, other.headerColor)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(MyBase.GetHashCode(), title, Arrays.hashCode(header), ArrayListHash(table), padLeftPx, padRightPx, padTopPx, padBottomPx,
                                border, Arrays.hashCode(colWidthsPercent), backgroundColor, headerColor)
        End Function

        Public Overrides Function ToString() As String
            Return "RenderableComponentTable(title=" & Objects.toString(title) & ", header=" & Arrays.toString(header) & ", table=" & ArrayListToString(table) &
                   ", padLeftPx=" & padLeftPx & ", padRightPx=" & padRightPx & ", padTopPx=" & padTopPx & ", padBottomPx=" & padBottomPx &
                   ", border=" & border & ", colWidthsPercent=" & Arrays.toString(colWidthsPercent) & ", backgroundColor=" & Objects.toString(backgroundColor) &
                   ", headerColor=" & Objects.toString(headerColor) & ")"
        End Function

        ''' <summary>Fluent builder of a <see cref="RenderableComponentTable"/>.</summary>
        Public Class Builder

            Friend titleValue As String
            Friend headerValue() As String
            Friend tableValue()() As String
            Friend padLeftPxValue As Integer = 0
            Friend padRightPxValue As Integer = 0
            Friend padTopPxValue As Integer = 0
            Friend padBottomPxValue As Integer = 0
            Friend borderValue As Integer = 0
            Friend colWidthsPercentValue() As Double
            Friend backgroundColorValue As String
            Friend headerColorValue As String

            Public Overridable Function title(ByVal value As String) As Builder
                Me.titleValue = value
                Return Me
            End Function

            Public Overridable Function header(ParamArray ByVal value() As String) As Builder
                Me.headerValue = value
                Return Me
            End Function

            Public Overridable Function table(ByVal value()() As String) As Builder
                Me.tableValue = value
                Return Me
            End Function

            ''' <summary>Border width in pixels of the table and its cells (0, the default, leaves the borders to the stylesheet).</summary>
            Public Overridable Function border(ByVal value As Integer) As Builder
                Me.borderValue = value
                Return Me
            End Function

            Public Overridable Function padLeftPx(ByVal value As Integer) As Builder
                Me.padLeftPxValue = value
                Return Me
            End Function

            Public Overridable Function padRightPx(ByVal value As Integer) As Builder
                Me.padRightPxValue = value
                Return Me
            End Function

            Public Overridable Function padTopPx(ByVal value As Integer) As Builder
                Me.padTopPxValue = value
                Return Me
            End Function

            Public Overridable Function padBottomPx(ByVal value As Integer) As Builder
                Me.padBottomPxValue = value
                Return Me
            End Function

            ''' <summary>The same padding on all four sides of every cell.</summary>
            Public Overridable Function paddingPx(ByVal value As Integer) As Builder
                padLeftPx(value)
                padRightPx(value)
                padTopPx(value)
                padBottomPx(value)
                Return Me
            End Function

            Public Overridable Function paddingPx(ByVal left As Integer, ByVal right As Integer, ByVal top As Integer, ByVal bottom As Integer) As Builder
                padLeftPx(left)
                padRightPx(right)
                padTopPx(top)
                padBottomPx(bottom)
                Return Me
            End Function

            Public Overridable Function colWidthsPercent(ParamArray ByVal value() As Double) As Builder
                Me.colWidthsPercentValue = value
                Return Me
            End Function

            ''' <summary>Background colour of the table, e.g. <c>#FFFFFF</c>; replaces the light/dark themed surface.</summary>
            Public Overridable Function backgroundColor(ByVal value As String) As Builder
                Me.backgroundColorValue = value
                Return Me
            End Function

            ''' <summary>Background colour of the header row, e.g. <c>#CCCCCC</c>; replaces the themed header colour.</summary>
            Public Overridable Function headerColor(ByVal value As String) As Builder
                Me.headerColorValue = value
                Return Me
            End Function

            Public Overridable Function build() As RenderableComponentTable
                Return New RenderableComponentTable(Me)
            End Function

        End Class

    End Class

End Namespace
