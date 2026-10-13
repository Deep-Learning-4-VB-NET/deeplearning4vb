Imports System
Imports System.IO

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

Namespace org.datavec.api.util.files

    ''' <summary>
    ''' URI helpers. The Java class built <c>file:</c> URIs by hand from <c>java.io.File</c>; this port
    ''' takes path strings and uses <see cref="System.Uri"/>. It also holds the conversions the rest of
    ''' the DataVec port uses in place of <c>java.net.URI.toString()</c>, <c>URI.create</c>,
    ''' <c>URI.getPath()</c> and <c>new File(URI)</c>.
    ''' </summary>
    Public NotInheritable Class URIUtil

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Converts a file path to a <c>file:</c> URI (Java <c>File.toURI()</c>). Relative paths are resolved
        ''' against the current directory; no trailing slash is added for directories.
        ''' </summary>
        Public Shared Function fileToURI(ByVal f As String) As Uri
            If f Is Nothing Then Throw New ArgumentNullException(NameOf(f))
            Return New Uri(Path.GetFullPath(f))
        End Function

        ''' <summary>
        ''' The string form of a URI as Java's <c>URI.toString()</c> returned it: escaped (<c>AbsoluteUri</c>)
        ''' for absolute URIs, the original text for relative ones. <c>System.Uri.ToString()</c> unescapes,
        ''' so it is not used for strings that are parsed back into URIs.
        ''' </summary>
        Public Shared Function toUriString(ByVal uri As Uri) As String
            If uri Is Nothing Then Return Nothing
            Return If(uri.IsAbsoluteUri, uri.AbsoluteUri, uri.OriginalString)
        End Function

        ''' <summary>Parses a URI string that may be relative (Java <c>URI.create</c> / <c>new URI(String)</c>).</summary>
        Public Shared Function createURI(ByVal s As String) As Uri
            If s Is Nothing Then Throw New ArgumentNullException(NameOf(s))
            Return New Uri(s, UriKind.RelativeOrAbsolute)
        End Function

        ''' <summary>The decoded path component (Java <c>URI.getPath()</c>): <c>/C:/dir/file</c> for a Windows file URI.</summary>
        Public Shared Function getPath(ByVal uri As Uri) As String
            If uri Is Nothing Then Return Nothing
            If uri.IsAbsoluteUri Then
                Dim p As String = Uri.UnescapeDataString(uri.AbsolutePath)
                ' .NET Framework drops the leading slash of a Windows drive path (C:/dir); Java keeps it (/C:/dir)
                If uri.IsFile AndAlso p.Length >= 2 AndAlso Char.IsLetter(p(0)) AndAlso p(1) = ":"c Then p = "/" & p
                Return p
            End If
            Dim s As String = uri.OriginalString
            Dim cut As Integer = s.IndexOfAny(New Char() {"?"c, "#"c})
            If cut >= 0 Then s = s.Substring(0, cut)
            Return Uri.UnescapeDataString(s)
        End Function

        ''' <summary>The local file path of an absolute <c>file:</c> URI (Java <c>new File(URI)</c>).</summary>
        Public Shared Function toFilePath(ByVal uri As Uri) As String
            If uri Is Nothing Then Throw New ArgumentNullException(NameOf(uri))
            If Not uri.IsAbsoluteUri Then Throw New ArgumentException("URI is not absolute: " & uri.OriginalString)
            If Not uri.IsFile Then Throw New ArgumentException("URI scheme is not ""file"": " & uri.AbsoluteUri)
            Return uri.LocalPath
        End Function

    End Class

End Namespace
