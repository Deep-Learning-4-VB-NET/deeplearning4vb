Imports System
Imports System.Collections.Generic
Imports System.IO
Imports DeepLearning4VB.org.datavec.api.util.files
Imports DeepLearning4VB.org.nd4j.common.base
Imports DeepLearning4VB.org.nd4j.common.collection
Imports DeepLearning4VB.org.nd4j.common.util

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
    ''' An input split over a file or (recursively) over the files of a directory. The root is a
    ''' path string (the Java original used <c>java.io.File</c>).
    ''' </summary>
    Public Class FileSplit
        Inherits BaseInputSplit

        Protected Friend rootDirPath As String
        ' Use for Collections, pass in list of file type strings
        Protected Friend allowFormat() As String = Nothing
        Protected Friend recursive As Boolean = True
        ''' <summary>The random number generator (called <c>random</c> in Java, renamed so that it does not hide the <see cref="Random"/> type).</summary>
        Protected Friend rng As Random
        Protected Friend randomize As Boolean = False

        Protected Friend Sub New(ByVal rootDir As String, ByVal allowFormat() As String, ByVal recursive As Boolean, ByVal rng As Random, ByVal runMain As Boolean)
            Me.allowFormat = allowFormat
            Me.recursive = recursive
            Me.rootDirPath = rootDir
            If rng IsNot Nothing Then
                Me.rng = rng
                Me.randomize = True
            End If
            If runMain Then
                Me.initialize()
            End If
        End Sub

        Public Sub New(ByVal rootDir As String)
            Me.New(rootDir, Nothing, True, Nothing, True)
        End Sub

        Public Sub New(ByVal rootDir As String, ByVal rng As Random)
            Me.New(rootDir, Nothing, True, rng, True)
        End Sub

        Public Sub New(ByVal rootDir As String, ByVal allowFormat() As String)
            Me.New(rootDir, allowFormat, True, Nothing, True)
        End Sub

        Public Sub New(ByVal rootDir As String, ByVal allowFormat() As String, ByVal rng As Random)
            Me.New(rootDir, allowFormat, True, rng, True)
        End Sub

        Public Sub New(ByVal rootDir As String, ByVal allowFormat() As String, ByVal recursive As Boolean)
            Me.New(rootDir, allowFormat, recursive, Nothing, True)
        End Sub

        Protected Friend Overridable Sub initialize()
            If rootDirPath Is Nothing Then
                Throw New ArgumentException("File path must not be null")
            End If
            Dim exists As Boolean = File.Exists(rootDirPath) OrElse Directory.Exists(rootDirPath)
            If Path.IsPathRooted(rootDirPath) AndAlso Not exists Then
                Try
                    ' java.io.File.createNewFile(): create a new, empty file
                    Using fs As New FileStream(rootDirPath, FileMode.CreateNew, FileAccess.Write)
                    End Using
                    'ensure uri strings has the root file if it's not a directory
                    uriStrings = New List(Of String)()
                    uriStrings.Add(URIUtil.fileToURI(rootDirPath).AbsoluteUri)
                Catch e As IOException
                    Throw New InvalidOperationException(e.Message, e)
                End Try
            ElseIf Not exists Then
                ' When implementing wild card characters in the rootDir, remove this if exists,
                ' verify expanded paths exist and check for the edge case when expansion cannot be
                ' translated to existed locations
                Throw New ArgumentException("No such file or directory: " & Path.GetFullPath(rootDirPath))
            ElseIf Directory.Exists(rootDirPath) Then
                Dim list As IList(Of String) = listFiles(rootDirPath, allowFormat, recursive)
                uriStrings = New CompactHeapStringList()
                If randomize Then
                    iterationOrder = New Integer(list.Count - 1) {}
                    For i As Integer = 0 To iterationOrder.Length - 1
                        iterationOrder(i) = i
                    Next i
                    MathUtils.shuffleArray(iterationOrder, rng)
                End If
                For Each f As String In list
                    uriStrings.Add(URIUtil.fileToURI(f).AbsoluteUri)
                    splitLength += 1
                Next f
            Else
                ' Lists one file
                Dim toString As String = URIUtil.fileToURI(rootDirPath).AbsoluteUri 'URI.getPath(), getRawPath() etc don't have file:/ prefix necessary for conversion back to URI
                uriStrings = New List(Of String)(1)
                uriStrings.Add(toString)
                splitLength += New FileInfo(rootDirPath).Length
            End If
        End Sub

        Public Overrides Function addNewLocation() As String
            If Directory.Exists(rootDirPath) Then
                Return addNewLocation(URIUtil.fileToURI(Path.Combine(rootDirPath, Guid.NewGuid().ToString())).AbsoluteUri)
            Else
                'add a file in the same directory as the file with the same extension as the original file
                Dim absolute As String = Path.GetFullPath(rootDirPath)
                Dim extension As String = Path.GetExtension(absolute)
                If extension.StartsWith(".", StringComparison.Ordinal) Then extension = extension.Substring(1)
                Return addNewLocation(URIUtil.fileToURI(Path.Combine(Path.GetDirectoryName(absolute), Guid.NewGuid().ToString() & "." & extension)).AbsoluteUri)
            End If
        End Function

        Public Overrides Function addNewLocation(ByVal location As String) As String
            Dim f As String = URIUtil.toFilePath(URIUtil.createURI(location))
            Try
                ' java.io.File.createNewFile(): no-op when the file already exists
                If Not File.Exists(f) Then
                    Using fs As New FileStream(f, FileMode.CreateNew, FileAccess.Write)
                    End Using
                End If
            Catch e As IOException
                Throw New InvalidOperationException(e.Message, e)
            End Try
            uriStrings.Add(location)
            splitLength += 1
            Return location
        End Function

        Public Overrides Sub updateSplitLocations(ByVal reset As Boolean)
            If reset Then
                initialize()
            End If
        End Sub

        Public Overrides Function needsBootstrapForWrite() As Boolean
            Return locations() Is Nothing OrElse locations().Length < 1 OrElse (locations().Length = 1 AndAlso Not locations()(0).IsAbsoluteUri)
        End Function

        Public Overrides Sub bootStrapForWrite()
            If locations().Length = 1 AndAlso Not locations()(0).IsAbsoluteUri Then
                Dim parentDir As String = URIUtil.toFilePath(locations()(0))
                Dim writeFile As String = Path.Combine(parentDir, "write-file")
                Try
                    If Not File.Exists(writeFile) Then
                        Using fs As New FileStream(writeFile, FileMode.CreateNew, FileAccess.Write)
                        End Using
                    End If
                    'since locations are dynamically generated, allow
                    uriStrings.Add(URIUtil.fileToURI(writeFile).AbsoluteUri)
                Catch e As IOException
                    Throw New InvalidOperationException(e.Message, e)
                End Try
            End If
        End Sub

        Public Overrides Function openOutputStreamFor(ByVal location As String) As Stream
            Dim path As String = If(location.StartsWith("file:", StringComparison.Ordinal), URIUtil.toFilePath(New Uri(location)), location)
            Return New FileStream(path, FileMode.Create, FileAccess.Write)
        End Function

        Public Overrides Function openInputStreamFor(ByVal location As String) As Stream
            Dim path As String = If(location.StartsWith("file:", StringComparison.Ordinal), URIUtil.toFilePath(New Uri(location)), location)
            Return New FileStream(path, FileMode.Open, FileAccess.Read)
        End Function

        Public Overrides Function length() As Long
            Return splitLength
        End Function

        Public Overrides Sub reset()
            If randomize Then
                'Shuffle the iteration order
                MathUtils.shuffleArray(iterationOrder, rng)
            End If
        End Sub

        Public Overrides Function resetSupported() As Boolean
            Return True
        End Function

        Public Function getRootDir() As String
            Return rootDirPath
        End Function

        Public Overridable ReadOnly Property RootDir As String
            Get
                Return rootDirPath
            End Get
        End Property

        ''' <summary>
        ''' Lists the files under a directory, breadth first, keeping those whose name ends with one of the
        ''' allowed formats (all files when <paramref name="allowedFormats"/> is Nothing; the comparison is
        ''' case sensitive, like commons-io's <c>SuffixFileFilter</c>).
        ''' </summary>
        Private Function listFiles(ByVal dir As String, ByVal allowedFormats() As String, ByVal recursive As Boolean) As IList(Of String)
            Preconditions.checkState(Directory.Exists(dir), "Argument is not a directory: %s", dir)
            Dim queue As New Queue(Of String)()
            queue.Enqueue(dir)
            Dim [out] As IList(Of String) = New List(Of String)()
            Do While queue.Count > 0
                Dim entries() As String
                Try
                    entries = Directory.GetFileSystemEntries(queue.Dequeue())
                Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is UnauthorizedAccessException
                    entries = Nothing ' java.io.File.listFiles() returns null when the directory cannot be read
                End Try
                If entries IsNot Nothing Then
                    For Each f As String In entries
                        Dim isDir As Boolean = Directory.Exists(f)
                        If isDir AndAlso recursive Then
                            queue.Enqueue(f)
                        ElseIf Not isDir AndAlso acceptFile(Path.GetFileName(f), allowedFormats) Then
                            [out].Add(f)
                        End If
                    Next f
                End If
            Loop
            Return [out]
        End Function

        Private Shared Function acceptFile(ByVal name As String, ByVal allowedFormats() As String) As Boolean
            If allowedFormats Is Nothing Then Return True
            For Each suffix As String In allowedFormats
                If name.EndsWith(suffix, StringComparison.Ordinal) Then Return True
            Next suffix
            Return False
        End Function

    End Class

End Namespace
