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
Imports System.IO
Imports System.Text
Imports DeepLearning4VB.org.slf4j

Namespace org.datavec.api.transform.schema

    ''' <summary>
    ''' Infers a <see cref="Schema"/> from the header line and the first data line of a CSV file: a column is a
    ''' double, an integer, a long or otherwise the default type. The opencsv <c>CSVParser</c> of the Java
    ''' original is replaced by a private single-line parser (delimiter, quote and escape characters).
    ''' </summary>
    Public Class InferredSchema

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(InferredSchema))

        Protected Friend schemaBuilder As Schema.Builder
        Protected Friend pathToCsv As String
        Protected Friend defaultType As DataType
        Protected Friend quote As String

        Private ReadOnly delimiterChar As Char = ","c
        Private ReadOnly quoteChar As Char = """"c
        Private ReadOnly escapeChar As Char = "\"c

        Public Sub New(ByVal pathToCsv As String)
            Me.pathToCsv = pathToCsv
            Me.defaultType = DataType.STRING
        End Sub

        Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType)
            Me.pathToCsv = pathToCsv
            Me.defaultType = defaultType
        End Sub

        Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType, ByVal delimiter As Char)
            Me.pathToCsv = pathToCsv
            Me.defaultType = defaultType
            Me.delimiterChar = delimiter
        End Sub

        Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType, ByVal delimiter As Char, ByVal quote As Char)
            Me.pathToCsv = pathToCsv
            Me.defaultType = defaultType
            Me.delimiterChar = delimiter
            Me.quoteChar = quote
        End Sub

        Public Sub New(ByVal pathToCsv As String, ByVal defaultType As DataType, ByVal delimiter As Char, ByVal quote As Char, ByVal escape As Char)
            Me.pathToCsv = pathToCsv
            Me.defaultType = defaultType
            Me.delimiterChar = delimiter
            Me.quoteChar = quote
            Me.escapeChar = escape
        End Sub

        Public Overridable Function build() As Schema
            Dim headersAndRows As IList(Of String) = Nothing
            Me.schemaBuilder = New Schema.Builder()

            Try
                headersAndRows = New List(Of String)(File.ReadAllLines(pathToCsv))
            Catch e As IOException
                log.error("An error occurred while parsing sample CSV for schema", e)
            End Try
            Dim headers As IList(Of String) = parseLine(headersAndRows(0))
            Dim samples As IList(Of String) = parseLine(headersAndRows(1))

            If headers.Count <> samples.Count Then
                Throw New InvalidOperationException("CSV headers length does not match number of sample columns. " & "Please check that your CSV is valid, or check the delimiter used to parse the CSV.")
            End If

            For i As Integer = 0 To headers.Count - 1
                inferAndAddType(schemaBuilder, headers(i), samples(i))
            Next i
            Return schemaBuilder.build()
        End Function

        Private Function inferAndAddType(ByVal builder As Schema.Builder, ByVal header As String, ByVal sample As String) As Schema.Builder
            If isParsableAsDouble(sample) Then
                addOn(builder, header, DataType.DOUBLE)
            ElseIf isParsableAsInteger(sample) Then
                addOn(builder, header, DataType.INTEGER)
            ElseIf isParsableAsLong(sample) Then
                addOn(builder, header, DataType.LONG)
            Else
                addOn(builder, header, defaultType)
            End If

            Return schemaBuilder
        End Function

        Private Shared Function addOn(ByVal builder As Schema.Builder, ByVal columnName As String, ByVal columnType As DataType) As Schema.Builder
            Select Case columnType
                Case DataType.DOUBLE
                    Return builder.addColumnDouble(columnName, Nothing, Nothing, False, False) 'no nans/inf
                Case DataType.INTEGER
                    Return builder.addColumnInteger(columnName)
                Case DataType.LONG
                    Return builder.addColumnLong(columnName)
                Case DataType.STRING
                    Return builder.addColumnString(columnName)
                Case Else
                    Throw New ArgumentException("Schema inputs have to be string, integer or double")
            End Select
        End Function

        Private Function parseLine(ByVal line As String) As IList(Of String)
            Dim split() As String = parseCsvLine(line)
            Dim ret As New List(Of String)()
            For Each field As String In split
                Dim s As String = field
                If Me.quote IsNot Nothing AndAlso s.StartsWith(Me.quote, StringComparison.Ordinal) AndAlso s.EndsWith(Me.quote, StringComparison.Ordinal) Then
                    Dim n As Integer = Me.quote.Length
                    s = s.Substring(n, (s.Length - n) - n).Replace(Me.quote & Me.quote, Me.quote)
                End If
                ret.Add(s)
            Next field

            Return ret
        End Function

        ''' <summary>
        ''' A single-line CSV split in the style of opencsv's <c>CSVParser.parseLine</c>: fields are separated by the
        ''' delimiter, a quoted field may contain delimiters, a doubled quote inside a quoted field is a literal
        ''' quote, and the escape character makes the following quote/escape character literal.
        ''' </summary>
        Private Function parseCsvLine(ByVal line As String) As String()
            Dim fields As New List(Of String)()
            Dim sb As New StringBuilder()
            Dim inQuotes As Boolean = False
            Dim i As Integer = 0
            Do While i < line.Length
                Dim c As Char = line(i)
                If c = escapeChar AndAlso i + 1 < line.Length AndAlso (line(i + 1) = quoteChar OrElse line(i + 1) = escapeChar) Then
                    sb.Append(line(i + 1))
                    i += 2
                    Continue Do
                End If
                If c = quoteChar Then
                    If inQuotes AndAlso i + 1 < line.Length AndAlso line(i + 1) = quoteChar Then
                        sb.Append(quoteChar)
                        i += 2
                        Continue Do
                    End If
                    inQuotes = Not inQuotes
                ElseIf c = delimiterChar AndAlso Not inQuotes Then
                    fields.Add(sb.ToString())
                    sb.Length = 0
                Else
                    sb.Append(c)
                End If
                i += 1
            Loop
            If inQuotes Then
                Throw New IOException("Un-terminated quoted field at end of CSV line")
            End If
            fields.Add(sb.ToString())
            Return fields.ToArray()
        End Function

        Private Shared Function isParsableAsLong(ByVal s As String) As Boolean
            Dim v As Long
            Return JavaNumbers.tryParseLong(s, v)
        End Function

        Private Shared Function isParsableAsInteger(ByVal s As String) As Boolean
            Dim v As Integer
            Return JavaNumbers.tryParseInt(s, v)
        End Function

        Private Shared Function isParsableAsDouble(ByVal s As String) As Boolean
            Dim v As Double
            Return JavaNumbers.tryParseDouble(s, v)
        End Function

        ''' <summary>The column types the inference can produce (Java: a private enum; public here so the constructors can take it).</summary>
        Public Enum DataType
            [STRING]
            [INTEGER]
            [DOUBLE]
            [LONG]
        End Enum
    End Class
End Namespace
