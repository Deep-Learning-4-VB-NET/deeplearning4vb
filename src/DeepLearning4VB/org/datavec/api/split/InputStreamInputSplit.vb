Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DeepLearning4VB.org.datavec.api.util.files

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
    ''' An input split that wraps a single input stream. The Java class also had a constructor taking a
    ''' <c>java.io.File</c>; build the URI with <see cref="URIUtil.fileToURI"/> instead.
    ''' </summary>
    Public Class InputStreamInputSplit
        Implements InputSplit

        Private inputStreamValue As Stream
        Private ReadOnly location() As Uri

        ''' <summary>Instantiate with the given file as a uri</summary>
        ''' <param name="is"> the input stream to use </param>
        ''' <param name="path"> the path to use (a URI string, see <see cref="URIUtil.createURI"/>) </param>
        Public Sub New(ByVal [is] As Stream, ByVal path As String)
            Me.New([is], URIUtil.createURI(path))
        End Sub

        ''' <summary>Instantiate with the given file as a uri</summary>
        ''' <param name="is"> the input stream to use </param>
        ''' <param name="path"> the path to use </param>
        Public Sub New(ByVal [is] As Stream, ByVal path As Uri)
            Me.inputStreamValue = [is]
            Me.location = New Uri() {path}
        End Sub

        Public Sub New(ByVal [is] As Stream)
            Me.inputStreamValue = [is]
            Me.location = New Uri() {}
        End Sub

        Public Overridable Function canWriteToLocation(ByVal location As Uri) As Boolean Implements InputSplit.canWriteToLocation
            Return False
        End Function

        Public Overridable Function addNewLocation() As String Implements InputSplit.addNewLocation
            Return Nothing
        End Function

        Public Overridable Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
            Return Nothing
        End Function

        Public Overridable Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
        End Sub

        Public Overridable Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
            Throw New NotSupportedException()
        End Function

        Public Overridable Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
            Return inputStreamValue
        End Function

        Public Overridable Function length() As Long Implements InputSplit.length
            Throw New NotSupportedException()
        End Function

        Public Overridable Function locations() As Uri() Implements InputSplit.locations
            Return location
        End Function

        Public Overridable Function locationsIterator() As IEnumerator(Of Uri) Implements InputSplit.locationsIterator
            Return JavaCollections.singletonList(location(0)).GetEnumerator()
        End Function

        Public Overridable Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
            If location.Length >= 1 Then
                Return JavaCollections.singletonList(URIUtil.getPath(location(0))).GetEnumerator()
            End If
            Return Arrays.asList("").GetEnumerator()
        End Function

        Public Overridable Sub reset() Implements InputSplit.reset
            If Not resetSupported() Then
                Throw New NotSupportedException("Reset not supported from streams")
            End If
            Try
                inputStreamValue = openInputStreamFor(URIUtil.getPath(location(0)))
            Catch e As Exception
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Sub

        Public Overridable Function resetSupported() As Boolean Implements InputSplit.resetSupported
            Return location IsNot Nothing AndAlso location.Length > 0
        End Function

        Public Function getIs() As Stream
            Return inputStreamValue
        End Function

        Public Sub setIs(ByVal [is] As Stream)
            inputStreamValue = [is]
        End Sub

        ''' <summary>The wrapped input stream (<c>getIs()</c>/<c>setIs()</c> in Java).</summary>
        Public Property InputStream As Stream
            Get
                Return inputStreamValue
            End Get
            Set(ByVal value As Stream)
                inputStreamValue = value
            End Set
        End Property

    End Class

End Namespace
