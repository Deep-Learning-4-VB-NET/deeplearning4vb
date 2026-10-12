Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.common.util

    ''' <summary>
    ''' Extraction and listing of zip, jar, tar, tar.gz/tgz and gz archives. Zip handling uses
    ''' System.IO.Compression; tar handling is a small built-in reader (the Java code used commons-compress).
    ''' </summary>
    Public Class ArchiveUtils
        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ArchiveUtils))

        Protected Sub New()
        End Sub

        Public Shared Sub unzipFileTo(ByVal file As String, ByVal dest As String)
            unzipFileTo(file, dest, True)
        End Sub

        ''' <summary>
        ''' Extracts the archive into <paramref name="dest"/> (created if needed) and, like the Java
        ''' original, deletes the archive afterwards. The format is inferred from the file extension.
        ''' </summary>
        Public Shared Sub unzipFileTo(ByVal file As String, ByVal dest As String, ByVal logFiles As Boolean)
            If Not System.IO.File.Exists(file) Then
                Throw New ArgumentException("Archive doesnt exist")
            End If
            Directory.CreateDirectory(dest)
            Dim destRoot As String = Path.GetFullPath(dest).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) & Path.DirectorySeparatorChar

            If file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) OrElse file.EndsWith(".jar", StringComparison.OrdinalIgnoreCase) Then
                Using zip As ZipArchive = ZipFile.OpenRead(file)
                    For Each entry As ZipArchiveEntry In zip.Entries
                        Dim newFile As String = SafeTarget(destRoot, entry.FullName)
                        If entry.FullName.EndsWith("/", StringComparison.Ordinal) OrElse entry.FullName.EndsWith("\", StringComparison.Ordinal) Then
                            Directory.CreateDirectory(newFile)
                            Continue For
                        End If
                        Directory.CreateDirectory(Path.GetDirectoryName(newFile))
                        Using input As Stream = entry.Open()
                            Using output As New FileStream(newFile, FileMode.Create, FileAccess.Write)
                                input.CopyTo(output)
                            End Using
                        End Using
                        If logFiles Then
                            log.info("File extracted: {}", newFile)
                        End If
                    Next entry
                End Using
            ElseIf file.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase) OrElse file.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase) OrElse file.EndsWith(".tar", StringComparison.OrdinalIgnoreCase) Then
                Using raw As New FileStream(file, FileMode.Open, FileAccess.Read)
                    Using tarStream As Stream = If(file.EndsWith(".tar", StringComparison.OrdinalIgnoreCase), DirectCast(raw, Stream), New GZipStream(raw, CompressionMode.Decompress))
                        Dim reader As New TarReader(tarStream)
                        Dim entry As TarEntry = reader.NextEntry()
                        Do While entry IsNot Nothing
                            If logFiles Then
                                log.info("Extracting: {}", entry.Name)
                            End If
                            Dim target As String = SafeTarget(destRoot, entry.Name)
                            If entry.IsDirectory Then
                                Directory.CreateDirectory(target)
                            Else
                                Directory.CreateDirectory(Path.GetDirectoryName(target))
                                Using output As New FileStream(target, FileMode.Create, FileAccess.Write)
                                    reader.CopyEntryTo(output)
                                End Using
                            End If
                            entry = reader.NextEntry()
                        Loop
                    End Using
                End Using
            ElseIf file.EndsWith(".gz", StringComparison.OrdinalIgnoreCase) Then
                Dim extracted As String = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(file)), Path.GetFileName(file).Replace(".gz", ""))
                If System.IO.File.Exists(extracted) Then
                    System.IO.File.Delete(extracted)
                End If
                Using input As New GZipStream(New FileStream(file, FileMode.Open, FileAccess.Read), CompressionMode.Decompress)
                    Using output As New FileStream(extracted, FileMode.Create, FileAccess.Write)
                        input.CopyTo(output)
                    End Using
                End Using
            Else
                Throw New InvalidOperationException("Unable to infer file type (compression format) from source file name: " & file)
            End If
            System.IO.File.Delete(file)
        End Sub

        ''' <summary>Resolves an archive entry name under the destination and rejects paths that escape it (zip slip).</summary>
        Private Shared Function SafeTarget(ByVal destRoot As String, ByVal entryName As String) As String
            Dim relative As String = entryName.Replace("/"c, Path.DirectorySeparatorChar).Replace("\"c, Path.DirectorySeparatorChar)
            Dim full As String = Path.GetFullPath(Path.Combine(destRoot, relative))
            If Not full.StartsWith(destRoot, StringComparison.OrdinalIgnoreCase) Then
                log.debug("Attempt to unzip entry is outside of the target dir")
                Throw New IOException("Entry is outside of the target dir: " & entryName)
            End If
            Return full
        End Function

        Public Shared Function tarListFiles(ByVal tarFile As String) As IList(Of String)
            Preconditions.checkState(Not tarFile.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase), ".tar.gz files should not use this method - use tarGzListFiles instead")
            Return tarGzListFiles(tarFile, False)
        End Function

        Public Shared Function tarGzListFiles(ByVal tarGzFile As String) As IList(Of String)
            Return tarGzListFiles(tarGzFile, True)
        End Function

        Protected Friend Shared Function tarGzListFiles(ByVal file As String, ByVal isTarGz As Boolean) As IList(Of String)
            Dim result As IList(Of String) = New List(Of String)()
            Using raw As New FileStream(file, FileMode.Open, FileAccess.Read)
                Using tarStream As Stream = If(isTarGz, New GZipStream(raw, CompressionMode.Decompress), DirectCast(raw, Stream))
                    Dim reader As New TarReader(tarStream)
                    Dim entry As TarEntry = reader.NextEntry()
                    Do While entry IsNot Nothing
                        result.Add(entry.Name)
                        entry = reader.NextEntry()
                    Loop
                End Using
            End Using
            Return result
        End Function

        Public Shared Function zipListFiles(ByVal zipFile As String) As IList(Of String)
            Dim result As IList(Of String) = New List(Of String)()
            Using zf As ZipArchive = System.IO.Compression.ZipFile.OpenRead(zipFile)
                For Each ze As ZipArchiveEntry In zf.Entries
                    result.Add(ze.FullName)
                Next ze
            End Using
            Return result
        End Function

        Public Shared Sub zipExtractSingleFile(ByVal zipFile As String, ByVal destination As String, ByVal pathInZip As String)
            Using zf As ZipArchive = System.IO.Compression.ZipFile.OpenRead(zipFile)
                Dim entry As ZipArchiveEntry = zf.GetEntry(pathInZip)
                If entry Is Nothing Then
                    Throw New FileNotFoundException("Entry " & pathInZip & " not found in " & zipFile)
                End If
                Using input As Stream = entry.Open()
                    Using output As New FileStream(destination, FileMode.Create, FileAccess.Write)
                        input.CopyTo(output)
                    End Using
                End Using
            End Using
        End Sub

        Public Shared Sub tarGzExtractSingleFile(ByVal tarGz As String, ByVal destination As String, ByVal pathInTarGz As String)
            Dim extracted As Boolean = False
            Using raw As New FileStream(tarGz, FileMode.Open, FileAccess.Read)
                Using tarStream As New GZipStream(raw, CompressionMode.Decompress)
                    Dim reader As New TarReader(tarStream)
                    Dim entry As TarEntry = reader.NextEntry()
                    Do While entry IsNot Nothing
                        If pathInTarGz.Equals(entry.Name) Then
                            Using output As New FileStream(destination, FileMode.Create, FileAccess.Write)
                                reader.CopyEntryTo(output)
                            End Using
                            extracted = True
                        End If
                        entry = reader.NextEntry()
                    Loop
                End Using
            End Using
            Preconditions.checkState(extracted, "No file was extracted. File not found? %s", pathInTarGz)
        End Sub

        ''' <summary>A tar archive entry (ustar/GNU/pax long names supported; links and devices are skipped).</summary>
        Public Class TarEntry
            Public Property Name As String
            Public Property Size As Long
            Public Property IsDirectory As Boolean
            Public Property Mode As Integer
            Public Property ModifiedTime As DateTime
        End Class

        ''' <summary>Minimal forward-only tar reader.</summary>
        Public Class TarReader
            Private ReadOnly stream As Stream
            Private ReadOnly header(511) As Byte
            Private remaining As Long
            Private padding As Long

            Public Sub New(ByVal stream As Stream)
                Me.stream = stream
            End Sub

            ''' <summary>Advances to the next file or directory entry; Nothing at the end of the archive.</summary>
            Public Function NextEntry() As TarEntry
                SkipRemaining()
                Dim longName As String = Nothing
                Do
                    If Not ReadFully(header, 512) Then Return Nothing
                    If IsZeroBlock(header) Then Return Nothing
                    Dim typeFlag As Char = ChrW(header(156))
                    Dim size As Long = ParseOctal(header, 124, 12)
                    Dim name As String = ReadString(header, 0, 100)
                    Dim prefix As String = If(ReadString(header, 257, 6).StartsWith("ustar", StringComparison.Ordinal), ReadString(header, 345, 155), "")
                    If prefix.Length > 0 Then name = prefix & "/" & name
                    If typeFlag = "L"c Then
                        ' GNU long name: the data block holds the real name of the next entry
                        Dim data(CInt(size) - 1) As Byte
                        ReadFully(data, data.Length)
                        SkipBytes(Pad(size))
                        longName = Encoding.UTF8.GetString(data).TrimEnd(ChrW(0))
                        Continue Do
                    ElseIf typeFlag = "x"c OrElse typeFlag = "g"c Then
                        ' pax extended header: look for a path= record, then continue to the real entry
                        Dim data(CInt(size) - 1) As Byte
                        ReadFully(data, data.Length)
                        SkipBytes(Pad(size))
                        For Each line As String In Encoding.UTF8.GetString(data).Split(ControlChars.Lf)
                            Dim sp As Integer = line.IndexOf(" "c)
                            If sp > 0 AndAlso line.Substring(sp + 1).StartsWith("path=", StringComparison.Ordinal) Then
                                longName = line.Substring(sp + 1 + "path=".Length)
                            End If
                        Next line
                        Continue Do
                    End If
                    If longName IsNot Nothing Then
                        name = longName
                        longName = Nothing
                    End If
                    Dim isDir As Boolean = typeFlag = "5"c OrElse name.EndsWith("/", StringComparison.Ordinal)
                    If isDir Then size = 0
                    remaining = size
                    padding = Pad(size)
                    If typeFlag <> "0"c AndAlso typeFlag <> ChrW(0) AndAlso typeFlag <> "5"c AndAlso typeFlag <> "7"c Then
                        ' links, devices, etc.: skip
                        SkipRemaining()
                        Continue Do
                    End If
                    Dim entry As New TarEntry()
                    entry.Name = name
                    entry.Size = size
                    entry.IsDirectory = isDir
                    entry.Mode = CInt(ParseOctal(header, 100, 8))
                    entry.ModifiedTime = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ParseOctal(header, 136, 12))
                    Return entry
                Loop
            End Function

            ''' <summary>Copies the current entry's content to the output stream.</summary>
            Public Sub CopyEntryTo(ByVal output As Stream)
                Dim buffer(8191) As Byte
                While remaining > 0
                    Dim n As Integer = stream.Read(buffer, 0, CInt(Math.Min(buffer.Length, remaining)))
                    If n <= 0 Then Throw New EndOfStreamException("Unexpected end of tar archive")
                    output.Write(buffer, 0, n)
                    remaining -= n
                End While
                SkipBytes(padding)
                padding = 0
            End Sub

            Private Sub SkipRemaining()
                SkipBytes(remaining + padding)
                remaining = 0
                padding = 0
            End Sub

            Private Sub SkipBytes(ByVal count As Long)
                Dim buffer(8191) As Byte
                While count > 0
                    Dim n As Integer = stream.Read(buffer, 0, CInt(Math.Min(buffer.Length, count)))
                    If n <= 0 Then Return
                    count -= n
                End While
            End Sub

            Private Function ReadFully(ByVal buffer() As Byte, ByVal count As Integer) As Boolean
                Dim offset As Integer = 0
                While offset < count
                    Dim n As Integer = stream.Read(buffer, offset, count - offset)
                    If n <= 0 Then Return offset = 0 AndAlso False
                    offset += n
                End While
                Return True
            End Function

            Private Shared Function Pad(ByVal size As Long) As Long
                Dim remainder As Long = size Mod 512
                Return If(remainder = 0, 0, 512 - remainder)
            End Function

            Private Shared Function IsZeroBlock(ByVal block() As Byte) As Boolean
                For Each b As Byte In block
                    If b <> 0 Then Return False
                Next b
                Return True
            End Function

            Private Shared Function ReadString(ByVal block() As Byte, ByVal offset As Integer, ByVal length As Integer) As String
                Dim [end] As Integer = offset
                While [end] < offset + length AndAlso block([end]) <> 0
                    [end] += 1
                End While
                Return Encoding.UTF8.GetString(block, offset, [end] - offset)
            End Function

            Private Shared Function ParseOctal(ByVal block() As Byte, ByVal offset As Integer, ByVal length As Integer) As Long
                ' GNU base-256 encoding for large values
                If (block(offset) And &H80) <> 0 Then
                    Dim v As Long = 0
                    For i As Integer = offset + 1 To offset + length - 1
                        v = (v << 8) Or block(i)
                    Next i
                    Return v
                End If
                Dim s As String = ReadString(block, offset, length).Trim(" "c, ChrW(0))
                If s.Length = 0 Then Return 0
                Dim result As Long = 0
                For Each c As Char In s
                    If c < "0"c OrElse c > "7"c Then Exit For
                    result = result * 8 + (AscW(c) - AscW("0"c))
                Next c
                Return result
            End Function
        End Class
    End Class

End Namespace
