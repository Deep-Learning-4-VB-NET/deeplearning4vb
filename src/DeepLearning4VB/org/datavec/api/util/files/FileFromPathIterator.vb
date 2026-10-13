Imports System
Imports System.Collections
Imports System.Collections.Generic

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
    ''' Wraps an enumerator of <c>file:</c> URI strings and yields the local file path of each
    ''' (the Java original yielded <c>java.io.File</c>; files are path strings in this port).
    ''' </summary>
    Public Class FileFromPathIterator
        Implements IEnumerator(Of String)

        Private ReadOnly paths As IEnumerator(Of String)
        Private currentItem As String

        Public Sub New(ByVal paths As IEnumerator(Of String))
            Me.paths = paths
        End Sub

        Public ReadOnly Property Current As String Implements IEnumerator(Of String).Current
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
            currentItem = URIUtil.toFilePath(URIUtil.createURI(paths.Current))
            Return True
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
