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

Imports System.Collections.Generic
Imports System.Text
Imports DeepLearning4VB.org.nd4vb.common.function
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.misc

    ''' <summary>
    ''' Writes a record as one line of delimited text: the values' <c>ToString()</c> joined with the delimiter. If a
    ''' quote string is set, values that contain the delimiter are quoted (and quotes inside them doubled).
    ''' </summary>
    Public Class WritablesToStringFunction
        Implements [Function](Of IList(Of WritableT), String)

        Private ReadOnly delim As String
        Private ReadOnly quote As String

        ''' <param name="delim">the delimiter between values (for example ",")</param>
        Public Sub New(ByVal delim As String)
            Me.New(delim, Nothing)
        End Sub

        ''' <param name="delim">the delimiter between values (for example ",")</param>
        ''' <param name="quote">the quote for values containing the delimiter, or Nothing for no quoting</param>
        Public Sub New(ByVal delim As String, ByVal quote As String)
            Me.delim = delim
            Me.quote = quote
        End Sub

        Public Overridable Function apply(ByVal c As IList(Of WritableT)) As String Implements [Function](Of IList(Of WritableT), String).apply
            Dim sb As New StringBuilder()
            append(c, sb, delim, quote)
            Return sb.ToString()
        End Function

        ''' <summary>Appends the delimited values of <paramref name="c"/> to <paramref name="sb"/>.</summary>
        Public Shared Sub append(ByVal c As IList(Of WritableT), ByVal sb As StringBuilder, ByVal delim As String, ByVal quote As String)
            Dim first As Boolean = True
            For Each w As WritableT In c
                If Not first Then
                    sb.Append(delim)
                End If
                Dim s As String = w.ToString()
                Dim needQuotes As Boolean = s.Contains(delim)
                If needQuotes AndAlso quote IsNot Nothing Then
                    sb.Append(quote)
                    s = s.Replace(quote, quote & quote)
                End If
                sb.Append(s)
                If needQuotes AndAlso quote IsNot Nothing Then
                    sb.Append(quote)
                End If
                first = False
            Next w
        End Sub

    End Class

End Namespace
