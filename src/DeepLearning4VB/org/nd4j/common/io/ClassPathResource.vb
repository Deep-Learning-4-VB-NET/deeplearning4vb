Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions
Imports ND4JClassLoading = DeepLearning4VB.org.nd4j.common.config.ND4JClassLoading

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

Namespace org.nd4j.common.io

    ''' <summary>
    ''' A resource looked up "on the class path". .NET has no class path, so a path is resolved in this order:
    ''' <list type="number">
    ''' <item>a file or directory under one of the <see cref="SearchDirectories"/> (by default the application
    ''' base directory, the current directory, and their <c>resources</c> sub-directories);</item>
    ''' <item>an embedded manifest resource of the given assembly, or of any assembly known to
    ''' <see cref="ND4JClassLoading"/>, whose logical name equals the path (with '/' or '.' separators)
    ''' or ends with it.</item>
    ''' </list>
    ''' Embedded resources have no file; <see cref="File"/> extracts them to a temporary file, as the Java
    ''' version did for resources packed in a jar.
    ''' </summary>
    Public Class ClassPathResource
        Inherits AbstractResource

        Private Shared ReadOnly searchDirs As New List(Of String)()
        Private Shared ReadOnly searchDirsLock As New Object()

        Private ReadOnly pathValue As String
        Private ReadOnly assembly As Assembly
        Private ReadOnly clazz As Type

        Shared Sub New()
            resetSearchDirectories()
        End Sub

        ''' <summary>Directories searched for a file matching the resource path, in order. Add your own to resolve test resources.</summary>
        Public Shared ReadOnly Property SearchDirectories As IList(Of String)
            Get
                SyncLock searchDirsLock
                    Return New List(Of String)(searchDirs)
                End SyncLock
            End Get
        End Property

        Public Shared Sub addSearchDirectory(ByVal directory As String)
            If String.IsNullOrEmpty(directory) Then Throw New ArgumentNullException(NameOf(directory))
            SyncLock searchDirsLock
                If Not searchDirs.Contains(directory) Then searchDirs.Insert(0, directory)
            End SyncLock
        End Sub

        Public Shared Sub removeSearchDirectory(ByVal directory As String)
            SyncLock searchDirsLock
                searchDirs.Remove(directory)
            End SyncLock
        End Sub

        Public Shared Sub resetSearchDirectories()
            SyncLock searchDirsLock
                searchDirs.Clear()
                Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
                Dim cwd As String = Directory.GetCurrentDirectory()
                For Each d As String In New String() {baseDir, System.IO.Path.Combine(baseDir, "resources"), cwd, System.IO.Path.Combine(cwd, "resources")}
                    If Not searchDirs.Contains(d) Then searchDirs.Add(d)
                Next d
            End SyncLock
        End Sub

        Public Sub New(ByVal path As String)
            Me.New(path, DirectCast(Nothing, Assembly))
        End Sub

        Public Sub New(ByVal path As String, ByVal assembly As Assembly)
            If path Is Nothing Then Throw New ArgumentNullException(NameOf(path), "Path must not be null")
            pathValue = CleanPath(path)
            Me.assembly = assembly
        End Sub

        Public Sub New(ByVal path As String, ByVal clazz As Type)
            If path Is Nothing Then Throw New ArgumentNullException(NameOf(path), "Path must not be null")
            pathValue = CleanPath(path)
            Me.clazz = clazz
            Me.assembly = If(clazz Is Nothing, Nothing, clazz.Assembly)
        End Sub

        ''' <summary>Normalises separators to '/', removes a leading '/', and resolves "." and ".." segments.</summary>
        Friend Shared Function CleanPath(ByVal path As String) As String
            Dim p As String = path.Replace("\"c, "/"c)
            While p.StartsWith("/", StringComparison.Ordinal)
                p = p.Substring(1)
            End While
            Dim parts As New List(Of String)()
            For Each segment As String In p.Split("/"c)
                If segment = "." OrElse segment.Length = 0 Then
                    Continue For
                ElseIf segment = ".." Then
                    If parts.Count > 0 Then parts.RemoveAt(parts.Count - 1)
                Else
                    parts.Add(segment)
                End If
            Next segment
            Dim result As String = String.Join("/", parts)
            If p.EndsWith("/", StringComparison.Ordinal) AndAlso result.Length > 0 Then result &= "/"
            Return result
        End Function

        Public ReadOnly Property Path As String
            Get
                Return pathValue
            End Get
        End Property

        Public Function getPath() As String
            Return pathValue
        End Function

        ' ---- resolution ----

        ''' <summary>The file-system path the resource resolves to, or Nothing if no search directory contains it.</summary>
        Private Function ResolveFileSystem() As String
            Dim relative As String = pathValue.TrimEnd("/"c).Replace("/"c, System.IO.Path.DirectorySeparatorChar)
            If relative.Length = 0 Then Return Nothing
            For Each dir As String In SearchDirectories
                Try
                    Dim candidate As String = System.IO.Path.Combine(dir, relative)
                    If System.IO.File.Exists(candidate) OrElse Directory.Exists(candidate) Then
                        Return System.IO.Path.GetFullPath(candidate)
                    End If
                Catch ex As Exception When TypeOf ex Is ArgumentException OrElse TypeOf ex Is NotSupportedException OrElse TypeOf ex Is PathTooLongException
                    ' skip malformed search directories
                End Try
            Next dir
            Return Nothing
        End Function

        Private Function CandidateAssemblies() As IEnumerable(Of Assembly)
            If assembly IsNot Nothing Then
                Return New Assembly() {assembly}
            End If
            Return ND4JClassLoading.Nd4jAssemblies.Invoke()
        End Function

        ''' <summary>Finds an embedded resource matching the path; returns the assembly and manifest name, or Nothing.</summary>
        Private Function ResolveEmbedded() As KeyValuePair(Of Assembly, String)?
            Dim slashName As String = pathValue.TrimEnd("/"c)
            If slashName.Length = 0 Then Return Nothing
            Dim dotName As String = slashName.Replace("/"c, "."c)
            For Each asm As Assembly In CandidateAssemblies()
                Dim names() As String
                Try
                    names = asm.GetManifestResourceNames()
                Catch
                    Continue For
                End Try
                ' exact match first
                For Each n As String In names
                    If String.Equals(n, slashName, StringComparison.OrdinalIgnoreCase) OrElse String.Equals(n, dotName, StringComparison.OrdinalIgnoreCase) Then
                        Return New KeyValuePair(Of Assembly, String)(asm, n)
                    End If
                Next n
                ' then "<anything>.<path>" / "<anything>/<path>"
                For Each n As String In names
                    If n.EndsWith("." & dotName, StringComparison.OrdinalIgnoreCase) OrElse n.EndsWith("/" & slashName, StringComparison.OrdinalIgnoreCase) Then
                        Return New KeyValuePair(Of Assembly, String)(asm, n)
                    End If
                Next n
            Next asm
            Return Nothing
        End Function

        ''' <summary>Embedded resources whose name starts with the path, for directory extraction.</summary>
        Private Function ResolveEmbeddedChildren() As IList(Of Tuple(Of Assembly, String, String))
            Dim result As New List(Of Tuple(Of Assembly, String, String))()
            Dim slashPrefix As String = pathValue.TrimEnd("/"c) & "/"
            Dim dotPrefix As String = pathValue.TrimEnd("/"c).Replace("/"c, "."c) & "."
            For Each asm As Assembly In CandidateAssemblies()
                Dim names() As String
                Try
                    names = asm.GetManifestResourceNames()
                Catch
                    Continue For
                End Try
                For Each n As String In names
                    Dim idx As Integer = n.IndexOf(slashPrefix, StringComparison.OrdinalIgnoreCase)
                    If idx >= 0 AndAlso (idx = 0 OrElse n(idx - 1) = "/"c OrElse n(idx - 1) = "."c) Then
                        result.Add(Tuple.Create(asm, n, n.Substring(idx + slashPrefix.Length)))
                        Continue For
                    End If
                    idx = n.IndexOf(dotPrefix, StringComparison.OrdinalIgnoreCase)
                    If idx >= 0 AndAlso (idx = 0 OrElse n(idx - 1) = "."c) Then
                        result.Add(Tuple.Create(asm, n, n.Substring(idx + dotPrefix.Length)))
                    End If
                Next n
            Next asm
            Return result
        End Function

        ' ---- Resource implementation ----

        Public Overrides Function exists() As Boolean
            Return ResolveFileSystem() IsNot Nothing OrElse ResolveEmbedded().HasValue OrElse ResolveEmbeddedChildren().Count > 0
        End Function

        ''' <summary>True if the resource resolves to a directory (on disk, or as a prefix of embedded resources).</summary>
        Public Function isDirectory() As Boolean
            Dim fs As String = ResolveFileSystem()
            If fs IsNot Nothing Then Return Directory.Exists(fs)
            Return Not ResolveEmbedded().HasValue AndAlso ResolveEmbeddedChildren().Count > 0
        End Function

        Public Overrides ReadOnly Property InputStream As Stream
            Get
                Dim fs As String = ResolveFileSystem()
                If fs IsNot Nothing Then
                    If Directory.Exists(fs) Then
                        Throw New IOException(Description & " is a directory and cannot be opened as a stream")
                    End If
                    Return New BufferedStream(New FileStream(fs, FileMode.Open, FileAccess.Read, FileShare.Read))
                End If
                Dim embedded = ResolveEmbedded()
                If embedded.HasValue Then
                    Dim s As Stream = embedded.Value.Key.GetManifestResourceStream(embedded.Value.Value)
                    If s IsNot Nothing Then Return s
                End If
                Throw New FileNotFoundException(pathValue & " cannot be opened because it does not exist")
            End Get
        End Property

        ''' <summary>
        ''' The file the resource resolves to. An embedded resource is extracted to a temporary file
        ''' (the Java original did the same for resources inside jars).
        ''' </summary>
        Public Overrides ReadOnly Property File As String
            Get
                Dim fs As String = ResolveFileSystem()
                If fs IsNot Nothing Then Return fs
                If ResolveEmbedded().HasValue Then Return TempFileFromArchive
                Throw New FileNotFoundException(Description & " cannot be resolved to absolute file path because it does not exist")
            End Get
        End Property

        ''' <summary>Copies the resource content to a new temporary file and returns its path.</summary>
        Public Overridable ReadOnly Property TempFileFromArchive As String
            Get
                Return getTempFileFromArchive(Nothing)
            End Get
        End Property

        ''' <summary>
        ''' Copies the resource content to a file. With a root directory the original file name is kept
        ''' inside that directory; otherwise a uniquely named temporary file is created.
        ''' </summary>
        Public Overridable Function getTempFileFromArchive(ByVal rootDirectory As String) As String
            Dim name As String = Filename
            Dim tmpFile As String
            If rootDirectory IsNot Nothing Then
                Directory.CreateDirectory(rootDirectory)
                tmpFile = System.IO.Path.Combine(rootDirectory, name)
            Else
                Dim dir As String = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "dl4vb-" & Guid.NewGuid().ToString("N"))
                Directory.CreateDirectory(dir)
                tmpFile = System.IO.Path.Combine(dir, name)
            End If
            Using input As Stream = InputStream
                Using output As New FileStream(tmpFile, FileMode.Create, FileAccess.Write)
                    input.CopyTo(output)
                End Using
            End Using
            Return tmpFile
        End Function

        ''' <summary>Copies the directory this resource points to (on disk or embedded) into an existing destination directory.</summary>
        Public Overridable Sub copyDirectory(ByVal destination As String)
            Preconditions.checkState(destination IsNot Nothing AndAlso Directory.Exists(destination), "Destination directory must exist and be a directory: %s", destination)
            Dim fs As String = ResolveFileSystem()
            If fs IsNot Nothing Then
                Preconditions.checkState(Directory.Exists(fs), "Source must be a directory: %s", fs)
                CopyDirectoryRecursive(fs, destination)
                Return
            End If
            Dim children = ResolveEmbeddedChildren()
            If children.Count = 0 Then
                Throw New InvalidOperationException("Resource '" & pathValue & "' cannot be found.")
            End If
            For Each child In children
                Dim relative As String = child.Item3
                ' manifest names of embedded files use '.' between folders only when no logical name was given; keep as-is otherwise
                Dim target As String = System.IO.Path.Combine(destination, relative.Replace("/"c, System.IO.Path.DirectorySeparatorChar))
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target))
                Using input As Stream = child.Item1.GetManifestResourceStream(child.Item2)
                    Using output As New FileStream(target, FileMode.Create, FileAccess.Write)
                        input.CopyTo(output)
                    End Using
                End Using
            Next child
        End Sub

        Friend Shared Sub CopyDirectoryRecursive(ByVal source As String, ByVal destination As String)
            Directory.CreateDirectory(destination)
            For Each f As String In Directory.GetFiles(source)
                System.IO.File.Copy(f, System.IO.Path.Combine(destination, System.IO.Path.GetFileName(f)), True)
            Next f
            For Each d As String In Directory.GetDirectories(source)
                CopyDirectoryRecursive(d, System.IO.Path.Combine(destination, System.IO.Path.GetFileName(d)))
            Next d
        End Sub

        Public Overrides ReadOnly Property URI As Uri
            Get
                Dim fs As String = ResolveFileSystem()
                If fs IsNot Nothing Then Return New Uri(fs)
                Dim embedded = ResolveEmbedded()
                If embedded.HasValue Then
                    Return New Uri("resource://" & embedded.Value.Key.GetName().Name & "/" & embedded.Value.Value)
                End If
                Throw New FileNotFoundException(Description & " cannot be resolved to URL because it does not exist")
            End Get
        End Property

        Public Overrides Function contentLength() As Long
            Dim fs As String = ResolveFileSystem()
            If fs IsNot Nothing AndAlso System.IO.File.Exists(fs) Then
                Return New FileInfo(fs).Length
            End If
            Return MyBase.contentLength()
        End Function

        Public Overrides Function lastModified() As Long
            Dim fs As String = ResolveFileSystem()
            If fs IsNot Nothing Then
                Return CLng((System.IO.File.GetLastWriteTimeUtc(fs) - New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds)
            End If
            Return 0L
        End Function

        Public Overrides Function createRelative(ByVal relativePath As String) As Resource
            Dim basePath As String = pathValue
            Dim lastSlash As Integer = basePath.LastIndexOf("/"c)
            Dim combined As String = If(lastSlash < 0, relativePath, basePath.Substring(0, lastSlash + 1) & relativePath)
            If clazz IsNot Nothing Then Return New ClassPathResource(combined, clazz)
            Return New ClassPathResource(combined, assembly)
        End Function

        Public Overrides ReadOnly Property Filename As String
            Get
                Dim p As String = pathValue.TrimEnd("/"c)
                Dim lastSlash As Integer = p.LastIndexOf("/"c)
                Return If(lastSlash < 0, p, p.Substring(lastSlash + 1))
            End Get
        End Property

        Public Overrides ReadOnly Property Description As String
            Get
                Return "class path resource [" & pathValue & "]"
            End Get
        End Property

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other = TryCast(obj, ClassPathResource)
            If other Is Nothing Then Return False
            Return pathValue.Equals(other.pathValue) AndAlso Object.Equals(assembly, other.assembly) AndAlso Object.Equals(clazz, other.clazz)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return pathValue.GetHashCode()
        End Function
    End Class

End Namespace
