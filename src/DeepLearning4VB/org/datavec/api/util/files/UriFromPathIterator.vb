Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text.RegularExpressions

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
    ''' Wraps an enumerator of path strings and yields them as URIs: strings with a scheme are parsed
    ''' as URIs, anything else is treated as a file path (for backward compatibility).
    ''' </summary>
    Public Class UriFromPathIterator
        Implements IEnumerator(Of Uri)

        Friend Shared ReadOnly schemaPattern As New Regex("^.*?:/.*$", RegexOptions.Compiled)

        Private ReadOnly paths As IEnumerator(Of String)
        Private currentItem As Uri

        Public Sub New(ByVal paths As IEnumerator(Of String))
            Me.paths = paths
        End Sub

        Public ReadOnly Property Current As Uri Implements IEnumerator(Of Uri).Current
            Get
                Return currentItem
            End Get
        End Property

        Private ReadOnly Property CurrentObject As Object Implements IEnumerator.Current
            Get
                Return currentItem
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            If Not paths.MoveNext() Then
                currentItem = Nothing
                Return False
            End If
            currentItem = toUri(paths.Current)
            Return True
        End Function

        Private Shared Function toUri(ByVal s As String) As Uri
            If schemaPattern.IsMatch(s) Then
                Return New Uri(s)
            Else
                'No scheme - assume file for backward compatibility
                Return URIUtil.fileToURI(s)
            End If
        End Function

        Public Sub Reset() Implements IEnumerator.Reset
            paths.Reset()
            currentItem = Nothing
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            paths.Dispose()
        End Sub

    End Class

End Namespace
