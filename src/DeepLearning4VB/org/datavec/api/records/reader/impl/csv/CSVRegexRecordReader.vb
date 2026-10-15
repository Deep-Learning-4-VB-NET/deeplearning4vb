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
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.JavaCompat
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.records.reader.impl.csv

    ''' <summary>
    ''' A CSV reader that additionally splits selected columns with a regular expression (one capture
    ''' group per output value). Patterns are <see cref="Regex"/>es anchored like Java's <c>Matcher.matches()</c>.
    ''' </summary>
    <Serializable>
    Public Class CSVRegexRecordReader
        Inherits CSVRecordReader

        Protected Friend regexs() As String = Nothing
        Protected Friend patterns() As System.Text.RegularExpressions.Regex = Nothing
        Protected Friend Shadows delimiter As String
        Protected Friend Shadows quote As String

        ''' <summary>
        ''' Skip lines, use delimiter, strip quotes, and parse each column with a regex </summary>
        ''' <param name="skipNumLines"> the number of lines to skip </param>
        ''' <param name="delimiter"> the delimiter </param>
        ''' <param name="quote"> the quote to strip </param>
        ''' <param name="regexs"> the regexs to parse columns with </param>
        Public Sub New(ByVal skipNumLines As Integer, ByVal delimiter As String, ByVal quote As String, ByVal regexs() As String)
            MyBase.New(skipNumLines)
            Me.delimiter = delimiter
            Me.quote = quote
            Me.regexs = regexs
            If regexs IsNot Nothing Then
                patterns = New System.Text.RegularExpressions.Regex(regexs.Length - 1) {}
                For i As Integer = 0 To regexs.Length - 1
                    If regexs(i) IsNot Nothing Then
                        patterns(i) = New System.Text.RegularExpressions.Regex("\A(?:" & regexs(i) & ")\z")
                    End If
                Next i
            End If
        End Sub

        Protected Friend Overrides Function parseLine(ByVal line As String) As IList(Of writable.Writable)
            Dim split() As String = StringHelper.Split(line, delimiter, True)
            Dim ret As IList(Of writable.Writable) = New List(Of writable.Writable)()
            For i As Integer = 0 To split.Length - 1
                Dim s As String = split(i)
                If quote IsNot Nothing AndAlso s.StartsWith(quote, StringComparison.Ordinal) AndAlso s.EndsWith(quote, StringComparison.Ordinal) Then
                    Dim n As Integer = quote.Length
                    s = s.Substring(n, (s.Length - n) - n).Replace(quote & quote, quote)
                End If
                If regexs IsNot Nothing AndAlso regexs(i) IsNot Nothing Then
                    Dim m As Match = patterns(i).Match(s)
                    If m.Success Then
                        Dim j As Integer = 1
                        Do While j < m.Groups.Count 'Note: Matcher.group(0) is the entire sequence; we only care about groups 1 onward
                            ret.Add(New Text(m.Groups(j).Value))
                            j += 1
                        Loop
                    Else
                        Throw New InvalidOperationException("Invalid line: value does not match regex (regex=""" & regexs(i) & """; value=""" & s & """")
                    End If
                Else
                    ret.Add(New Text(s))
                End If
            Next i
            Return ret
        End Function

    End Class

End Namespace
