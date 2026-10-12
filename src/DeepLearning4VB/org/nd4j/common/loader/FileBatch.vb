Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text

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

Namespace org.nd4j.common.loader

    ''' <summary>
    ''' A batch of files held in memory as byte arrays together with their original URIs.
    ''' A batch can be saved to and loaded from a zip file. File contents are <c>Byte()</c>
    ''' (the Java original used <c>byte[]</c>) and file locations are path strings.
    ''' </summary>
    <Serializable>
    Public Class FileBatch
        ''' <summary>Name of the entry in the zip file that lists the original paths/URIs.</summary>
        Public Const ORIGINAL_PATHS_FILENAME As String = "originalUris.txt"

        Private ReadOnly fileBytesList As IList(Of Byte())
        Private ReadOnly originalUriList As IList(Of String)

        Public Sub New(ByVal fileBytes As IList(Of Byte()), ByVal originalUris As IList(Of String))
            If fileBytes Is Nothing Then Throw New ArgumentNullException(NameOf(fileBytes))
            If originalUris Is Nothing Then Throw New ArgumentNullException(NameOf(originalUris))
            fileBytesList = fileBytes
            originalUriList = originalUris
        End Sub

        Public ReadOnly Property FileBytes As IList(Of Byte())
            Get
                Return fileBytesList
            End Get
        End Property

        Public ReadOnly Property OriginalUris As IList(Of String)
            Get
                Return originalUriList
            End Get
        End Property

        Public Function getFileBytes() As IList(Of Byte())
            Return fileBytesList
        End Function

        Public Function getOriginalUris() As IList(Of String)
            Return originalUriList
        End Function

        ''' <summary>Reads a FileBatch previously saved with <see cref="writeAsZip(String)"/>.</summary>
        Public Shared Function readFromZip(ByVal file As String) As FileBatch
            Using fis As New FileStream(file, FileMode.Open, FileAccess.Read)
                Return readFromZip(fis)
            End Using
        End Function

        ''' <summary>Reads a FileBatch previously saved with <see cref="writeAsZip(Stream)"/>. The stream is closed.</summary>
        Public Shared Function readFromZip(ByVal [is] As Stream) As FileBatch
            Dim originalUris As String = Nothing
            Dim bytesMap As New Dictionary(Of Integer, Byte())()
            Using zip As New ZipArchive([is], ZipArchiveMode.Read, False)
                For Each entry As ZipArchiveEntry In zip.Entries
                    Dim name As String = entry.FullName
                    Dim bytes As Byte()
                    Using es As Stream = entry.Open()
                        Using ms As New MemoryStream()
                            es.CopyTo(ms)
                            bytes = ms.ToArray()
                        End Using
                    End Using
                    If name.Equals(ORIGINAL_PATHS_FILENAME) Then
                        originalUris = Encoding.UTF8.GetString(bytes)
                    Else
                        Dim idxSplit As Integer = name.IndexOf("_", StringComparison.Ordinal)
                        Dim idxSplit2 As Integer = name.IndexOf(".", StringComparison.Ordinal)
                        Dim fileIdx As Integer = Integer.Parse(name.Substring(idxSplit + 1, idxSplit2 - (idxSplit + 1)))
                        bytesMap(fileIdx) = bytes
                    End If
                Next entry
            End Using

            If originalUris Is Nothing Then
                Throw New IOException("Zip file does not contain the " & ORIGINAL_PATHS_FILENAME & " entry; not a FileBatch")
            End If

            Dim list As IList(Of Byte()) = New List(Of Byte())(bytesMap.Count)
            For i As Integer = 0 To bytesMap.Count - 1
                If Not bytesMap.ContainsKey(i) Then
                    Throw New IOException("Zip file is missing the entry for file index " & i)
                End If
                list.Add(bytesMap(i))
            Next i

            Dim origPaths As IList(Of String) = New List(Of String)(originalUris.Split(ControlChars.Lf, True))
            Return New FileBatch(list, origPaths)
        End Function

        ''' <summary>Creates a FileBatch from the specified files.</summary>
        Public Shared Function forFiles(ByVal ParamArray files() As String) As FileBatch
            Return forFiles(DirectCast(New List(Of String)(files), IList(Of String)))
        End Function

        ''' <summary>Creates a FileBatch from the specified files.</summary>
        Public Shared Function forFiles(ByVal files As IList(Of String)) As FileBatch
            Dim origPaths As IList(Of String) = New List(Of String)(files.Count)
            Dim bytes As IList(Of Byte()) = New List(Of Byte())(files.Count)
            For Each f As String In files
                bytes.Add(File.ReadAllBytes(f))
                origPaths.Add(New Uri(Path.GetFullPath(f)).AbsoluteUri)
            Next f
            Return New FileBatch(bytes, origPaths)
        End Function

        ''' <summary>Writes the FileBatch to the specified file in zip format.</summary>
        Public Overridable Sub writeAsZip(ByVal f As String)
            Using fs As New FileStream(f, FileMode.Create, FileAccess.Write)
                writeAsZip(fs)
            End Using
        End Sub

        ''' <summary>Writes the FileBatch to the output stream in zip format. The stream is closed.</summary>
        Public Overridable Sub writeAsZip(ByVal os As Stream)
            Using zip As New ZipArchive(os, ZipArchiveMode.Create, False)
                ' Original paths as a text file
                Dim pathsEntry As ZipArchiveEntry = zip.CreateEntry(ORIGINAL_PATHS_FILENAME)
                Using es As Stream = pathsEntry.Open()
                    Dim joined As Byte() = Encoding.UTF8.GetBytes(String.Join(ControlChars.Lf, originalUriList))
                    es.Write(joined, 0, joined.Length)
                End Using

                For i As Integer = 0 To fileBytesList.Count - 1
                    Dim ext As String = GetExtension(originalUriList(i))
                    If ext Is Nothing OrElse ext.Length = 0 Then
                        ext = "bin"
                    End If
                    Dim name As String = "file_" & i & "." & ext
                    Dim entry As ZipArchiveEntry = zip.CreateEntry(name)
                    Using es As Stream = entry.Open()
                        Dim data As Byte() = fileBytesList(i)
                        es.Write(data, 0, data.Length)
                    End Using
                Next i
            End Using
        End Sub

        ''' <summary>Extension of a path or URI without the leading dot, or "" if none (FilenameUtils.getExtension).</summary>
        Private Shared Function GetExtension(ByVal uriOrPath As String) As String
            If uriOrPath Is Nothing Then Return ""
            Dim lastSep As Integer = Math.Max(uriOrPath.LastIndexOf("/"c), uriOrPath.LastIndexOf("\"c))
            Dim lastDot As Integer = uriOrPath.LastIndexOf("."c)
            If lastDot < 0 OrElse lastDot < lastSep Then Return ""
            Return uriOrPath.Substring(lastDot + 1)
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Dim other = TryCast(o, FileBatch)
            If other Is Nothing Then Return False
            If fileBytesList.Count <> other.fileBytesList.Count OrElse originalUriList.Count <> other.originalUriList.Count Then Return False
            For i As Integer = 0 To fileBytesList.Count - 1
                If Not Arrays.equals(fileBytesList(i), other.fileBytesList(i)) Then Return False
            Next i
            For i As Integer = 0 To originalUriList.Count - 1
                If Not String.Equals(originalUriList(i), other.originalUriList(i)) Then Return False
            Next i
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 17
            For Each b In fileBytesList
                h = Arrays.WrapToInt32(31L * h + Arrays.hashCode(b))
            Next b
            For Each u In originalUriList
                h = Arrays.WrapToInt32(31L * h + Objects.hashCode(u))
            Next u
            Return h
        End Function
    End Class

End Namespace
