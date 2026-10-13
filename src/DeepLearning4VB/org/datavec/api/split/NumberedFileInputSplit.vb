Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.org.datavec.api.util.files
Imports DeepLearning4VB.org.slf4j

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

Namespace org.datavec.api.split

    ''' <summary>
    ''' An input split over a numbered sequence of files, <c>prefix%d.suffix</c> for the indices
    ''' between two bounds (inclusive). The format is applied with <see cref="JavaString.format"/>.
    ''' </summary>
    Public Class NumberedFileInputSplit
        Implements InputSplit

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(NumberedFileInputSplit))

        Private ReadOnly baseString As String
        Private ReadOnly minIdx As Integer
        Private ReadOnly maxIdx As Integer

        Private Shared ReadOnly p As New Regex("%(0\d)?d", RegexOptions.Compiled)
        Private Shared ReadOnly hasScheme As New Regex("^.{2,}:/.*$", RegexOptions.Compiled)

        ''' <param name="baseString"> String that defines file format. Must contain "%d", which will be replaced with
        '''                   the index of the file, possibly zero-padded to x digits if the pattern is in the form %0xd. </param>
        ''' <param name="minIdxInclusive"> Minimum index/number (starting number in sequence of files, inclusive) </param>
        ''' <param name="maxIdxInclusive"> Maximum index/number (last number in sequence of files, inclusive) </param>
        Public Sub New(ByVal baseString As String, ByVal minIdxInclusive As Integer, ByVal maxIdxInclusive As Integer)
            If baseString Is Nothing OrElse Not p.IsMatch(baseString) Then
                Throw New ArgumentException("Base String must match this regular expression: " & p.ToString())
            End If
            Me.baseString = baseString
            Me.minIdx = minIdxInclusive
            Me.maxIdx = maxIdxInclusive
        End Sub

        Public Overridable Function canWriteToLocation(ByVal location As Uri) As Boolean Implements InputSplit.canWriteToLocation
            Return location.IsAbsoluteUri
        End Function

        Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
            Return Nothing
        End Function

        Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
            Return Nothing
        End Function

        Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
            'no-op (locations() is dynamic)
        End Sub

        Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
            Return locations() Is Nothing OrElse locations().Length < 1 OrElse (locations().Length = 1 AndAlso Not locations()(0).IsAbsoluteUri)
        End Function

        Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
            If locations().Length = 1 AndAlso Not locations()(0).IsAbsoluteUri Then
                Dim parentDir As String = URIUtil.toFilePath(locations()(0))
                Dim writeFile As String = Path.Combine(parentDir, "write-file")
                Try
                    If Not File.Exists(writeFile) Then
                        Using fs As New FileStream(writeFile, FileMode.CreateNew, FileAccess.Write)
                        End Using
                    End If
                Catch e As IOException
                    log.error("", e)
                End Try
            End If
        End Sub

        Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
            Dim path As String = If(location.StartsWith("file:", StringComparison.Ordinal), URIUtil.toFilePath(New Uri(location)), location)
            Return New FileStream(path, FileMode.Create, FileAccess.Write)
        End Function

        Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
            Return New FileStream(location, FileMode.Open, FileAccess.Read)
        End Function

        Public Overridable Function length() As Long Implements InputSplit.length
            Return maxIdx - minIdx + 1
        End Function

        Public Overridable Function locations() As Uri() Implements InputSplit.locations
            Dim uris(CInt(length()) - 1) As Uri
            Dim x As Integer = 0
            If hasScheme.IsMatch(baseString) Then
                'URI (has scheme)
                For i As Integer = minIdx To maxIdx
                    uris(x) = New Uri(JavaString.format(baseString, i))
                    x += 1
                Next i
            Else
                'File, no scheme
                For i As Integer = minIdx To maxIdx
                    uris(x) = URIUtil.fileToURI(JavaString.format(baseString, i))
                    x += 1
                Next i
            End If
            Return uris
        End Function

        Public Overridable Function locationsIterator() As IEnumerator(Of Uri) Implements InputSplit.locationsIterator
            Return New UriFromPathIterator(locationsPathIterator())
        End Function

        Public Overridable Iterator Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
            For i As Integer = minIdx To maxIdx
                Yield JavaString.format(baseString, i)
            Next i
        End Function

        Public Overridable Sub reset() Implements InputSplit.reset
            'No op
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
            Return True
        End Function

    End Class

End Namespace
