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

Namespace org.nd4vb.common.io

    ''' <summary>Default implementations of the <see cref="Resource"/> members.</summary>
    Public MustInherit Class AbstractResource
        Implements Resource

        Public MustOverride ReadOnly Property InputStream As Stream Implements InputStreamSource.InputStream
        Public MustOverride ReadOnly Property Description As String Implements Resource.Description

        Protected Sub New()
        End Sub

        Public Function getInputStream() As Stream
            Return InputStream
        End Function

        Public Function getDescription() As String
            Return Description
        End Function

        ''' <summary>Exists if the file exists, or else if a stream can be opened.</summary>
        Public Overridable Function exists() As Boolean Implements Resource.exists
            Try
                Return System.IO.File.Exists(Me.File) OrElse Directory.Exists(Me.File)
            Catch ex As IOException
                Try
                    Using s As Stream = Me.InputStream
                        Return True
                    End Using
                Catch
                    Return False
                End Try
            End Try
        End Function

        Public Overridable ReadOnly Property Readable As Boolean Implements Resource.Readable
            Get
                Return True
            End Get
        End Property

        Public Overridable ReadOnly Property Open As Boolean Implements Resource.Open
            Get
                Return False
            End Get
        End Property

        Public Overridable ReadOnly Property URI As Uri Implements Resource.URI
            Get
                Throw New FileNotFoundException(Me.Description & " cannot be resolved to a URI")
            End Get
        End Property

        Public Function getURI() As Uri
            Return URI
        End Function

        Public Overridable ReadOnly Property File As String Implements Resource.File
            Get
                Throw New FileNotFoundException(Me.Description & " cannot be resolved to absolute file path")
            End Get
        End Property

        Public Function getFile() As String
            Return File
        End Function

        ''' <summary>Reads the stream to the end to measure it (subclasses backed by files override this).</summary>
        Public Overridable Function contentLength() As Long Implements Resource.contentLength
            Using s As Stream = Me.InputStream
                If s Is Nothing Then
                    Throw New InvalidOperationException("resource input stream must not be null")
                End If
                Dim size As Long = 0L
                Dim buf(254) As Byte
                Dim read As Integer = s.Read(buf, 0, buf.Length)
                Do While read > 0
                    size += read
                    read = s.Read(buf, 0, buf.Length)
                Loop
                Return size
            End Using
        End Function

        Public Overridable Function lastModified() As Long Implements Resource.lastModified
            Dim path As String = Me.FileForLastModifiedCheck
            If Not System.IO.File.Exists(path) Then
                Throw New FileNotFoundException(Me.Description & " cannot be resolved in the file system for resolving its last-modified timestamp")
            End If
            Return CLng((System.IO.File.GetLastWriteTimeUtc(path) - New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds)
        End Function

        Protected Overridable ReadOnly Property FileForLastModifiedCheck As String
            Get
                Return Me.File
            End Get
        End Property

        Public Overridable Function createRelative(ByVal relativePath As String) As Resource Implements Resource.createRelative
            Throw New FileNotFoundException("Cannot create a relative resource for " & Me.Description)
        End Function

        Public Overridable ReadOnly Property Filename As String Implements Resource.Filename
            Get
                Return Nothing
            End Get
        End Property

        Public Function getFilename() As String
            Return Filename
        End Function

        Public Overrides Function ToString() As String
            Return Me.Description
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other = TryCast(obj, Resource)
            Return other IsNot Nothing AndAlso String.Equals(other.Description, Me.Description, StringComparison.Ordinal)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me.Description.GetHashCode()
        End Function
    End Class

End Namespace
