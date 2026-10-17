Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Text.RegularExpressions
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports ND4VBSystemProperties = DeepLearning4VB.org.nd4vb.common.config.ND4VBSystemProperties
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports Downloader = DeepLearning4VB.org.nd4vb.common.resources.Downloader

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

Namespace org.nd4vb.common.resources.strumpf

    ''' <summary>
    ''' A Strumpf resource reference: a small JSON file (<c>name.resource_reference</c>) that records where
    ''' the real file can be downloaded from and the SHA-256 hashes of the compressed and extracted
    ''' content. <see cref="localFile"/> downloads, verifies and caches the real file on demand.
    ''' Property names match the Strumpf JSON format.
    ''' </summary>
    Public Class ResourceFile
        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ResourceFile))
        Public Const DEFAULT_CONNECTION_TIMEOUT As Integer = 60000 'Timeout for connections to be established
        Public Const DEFAULT_READ_TIMEOUT As Integer = 60000 'Timeout for amount of time between connection established and data is available
        Protected Friend Const PATH_KEY As String = "full_remote_path"
        Protected Friend Const HASH As String = "_hash"
        Protected Friend Const COMPRESSED_HASH As String = "_compressed_hash"

        Protected Friend Const MAX_DOWNLOAD_ATTEMPTS As Integer = 3

        Public Shared ReadOnly MAPPER As JsonSerializerOptions = newMapper()

        <JsonPropertyName("current_version")>
        Public Property current_version As Integer

        <JsonPropertyName("v1")>
        Public Property v1 As Dictionary(Of String, String)

        ''' <summary>Path of the reference file itself (not part of the JSON).</summary>
        <JsonIgnore>
        Public Property filePath As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal current_version As Integer, ByVal v1 As Dictionary(Of String, String), ByVal filePath As String)
            Me.current_version = current_version
            Me.v1 = v1
            Me.filePath = filePath
        End Sub

        Public Function getFilePath() As String
            Return filePath
        End Function

        Public Sub setFilePath(ByVal path As String)
            filePath = path
        End Sub

        ''' <summary>Reads a resource reference file.</summary>
        Public Shared Function fromFile(ByVal path As String) As ResourceFile
            Dim s As String
            Try
                s = File.ReadAllText(path, Encoding.UTF8)
            Catch e As IOException
                Throw New InvalidOperationException("Error reading resource reference file " & path, e)
            End Try
            Dim rf As ResourceFile = JsonSerializer.Deserialize(Of ResourceFile)(s, MAPPER)
            If rf Is Nothing Then
                Throw New InvalidOperationException("Resource reference file is empty: " & path)
            End If
            rf.filePath = path
            Return rf
        End Function

        ''' <summary>The path of the real file relative to the resource root (derived from the <c>&lt;path&gt;_hash</c> key).</summary>
        Public Overridable Function relativePath() As String
            Dim hashKey As String = Nothing
            If v1 IsNot Nothing Then
                For Each key As String In v1.Keys
                    If key.EndsWith(HASH, StringComparison.Ordinal) AndAlso Not key.EndsWith(COMPRESSED_HASH, StringComparison.Ordinal) Then
                        hashKey = key
                        Exit For
                    End If
                Next key
            End If
            If hashKey Is Nothing Then
                Throw New InvalidOperationException("Could not find <filename>_hash in resource reference file: " & filePath)
            End If
            Dim rel As String = hashKey.Substring(0, hashKey.Length - HASH.Length)
            Return rel.Replace("\"c, "/"c)
        End Function

        ''' <summary>True if the real file is present in the cache and its hash matches.</summary>
        Public Overridable Function localFileExistsAndValid(ByVal cacheRootDir As String) As Boolean
            Dim file As String = getLocalFile(cacheRootDir)
            If Not System.IO.File.Exists(file) Then
                Return False
            End If
            Dim sha256Property As String = relativePath() & HASH
            Dim expSha256 As String = v1.GetValueOrNull(sha256Property)
            Preconditions.checkState(expSha256 IsNot Nothing, "Expected JSON property %s was not found in resource reference file %s", sha256Property, filePath)
            Dim actualSha256 As String = sha256(file)
            Return String.Equals(expSha256, actualSha256, StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>The cache location of the real file: <c>&lt;cache&gt;/&lt;relativePath&gt;__v&lt;version&gt;/&lt;filename&gt;</c>.</summary>
        Protected Friend Overridable Function getLocalFile(ByVal cacheRootDir As String) As String
            Dim rel As String = relativePath()
            ' Versioned directories allow different versions of the same file to coexist in the cache.
            Dim lastSlash As Integer = Math.Max(rel.LastIndexOf("/"c), rel.LastIndexOf("\"c))
            Dim filename As String = If(lastSlash < 0, rel, rel.Substring(lastSlash + 1))
            Dim parentDir As String = Path.Combine(cacheRootDir, (rel & "__v" & current_version).Replace("/"c, Path.DirectorySeparatorChar))
            Return Path.Combine(parentDir, filename)
        End Function

        ''' <summary>Returns the real file, downloading, verifying and extracting it into the cache if necessary.</summary>
        Public Overridable Function localFile(ByVal cacheRootDir As String) As String
            If localFileExistsAndValid(cacheRootDir) Then
                Return getLocalFile(cacheRootDir)
            End If

            'Need to download and extract...
            Dim remotePath As String = v1.GetValueOrNull(PATH_KEY)
            Preconditions.checkState(remotePath IsNot Nothing, "No remote path was found in resource reference file %s", filePath)
            Dim f As String = getLocalFile(cacheRootDir)

            Dim tempDir As String = Path.Combine(Path.GetTempPath(), "strumpf-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tempDir)
            Dim tempFile As String = Path.Combine(tempDir, FileNameOf(remotePath))

            Dim sha256PropertyCompressed As String = relativePath() & COMPRESSED_HASH
            Dim sha256Compressed As String = v1.GetValueOrNull(sha256PropertyCompressed)
            Preconditions.checkState(sha256Compressed IsNot Nothing, "Expected JSON property %s was not found in resource reference file %s", sha256PropertyCompressed, filePath)

            Dim sha256Property As String = relativePath() & HASH
            Dim sha256Uncompressed As String = v1.GetValueOrNull(sha256Property)

            Dim connTimeoutStr As String = JavaSystem.getProperty(ND4VBSystemProperties.RESOURCES_CONNECTION_TIMEOUT)
            Dim readTimeoutStr As String = JavaSystem.getProperty(ND4VBSystemProperties.RESOURCES_READ_TIMEOUT)
            Dim validCTimeout As Boolean = connTimeoutStr IsNot Nothing AndAlso Regex.IsMatch(connTimeoutStr, "^\d+$")
            Dim validRTimeout As Boolean = readTimeoutStr IsNot Nothing AndAlso Regex.IsMatch(readTimeoutStr, "^\d+$")
            Dim connectTimeout As Integer = If(validCTimeout, Integer.Parse(connTimeoutStr), DEFAULT_CONNECTION_TIMEOUT)
            Dim readTimeout As Integer = If(validRTimeout, Integer.Parse(readTimeoutStr), DEFAULT_READ_TIMEOUT)

            Try
                Dim correctHash As Boolean = False
                For tryCount As Integer = 0 To MAX_DOWNLOAD_ATTEMPTS - 1
                    Try
                        If File.Exists(tempFile) Then
                            File.Delete(tempFile)
                        End If
                        log.info("Downloading remote resource {} to {}", remotePath, tempFile)
                        Downloader.copyURLToFile(New Uri(remotePath), tempFile, connectTimeout, readTimeout)
                        'Now: check if downloaded archive hash is OK
                        Dim actual As String = sha256(tempFile)
                        correctHash = String.Equals(sha256Compressed, actual, StringComparison.OrdinalIgnoreCase)
                        If Not correctHash Then
                            log.warn("Download of file {} failed: expected hash {} vs. actual hash {}", remotePath, sha256Compressed, actual)
                            Continue For
                        End If
                        log.info("Downloaded {} to temporary file {}", remotePath, tempFile)
                        Exit For
                    Catch t As Exception
                        If tryCount = MAX_DOWNLOAD_ATTEMPTS - 1 Then
                            Throw New InvalidOperationException("Error downloading test resource: " & remotePath, t)
                        End If
                        log.warn("Error downloading test resource, retrying... {}", remotePath, t)
                    End Try
                Next tryCount

                If Not correctHash Then
                    Throw New InvalidOperationException("Could not successfully download with correct hash file after " & MAX_DOWNLOAD_ATTEMPTS & " attempts: " & remotePath)
                End If

                'Now, extract (the remote file is gzip compressed):
                Directory.CreateDirectory(Path.GetDirectoryName(f))
                Try
                    Using input As New GZipStream(New FileStream(tempFile, FileMode.Open, FileAccess.Read), CompressionMode.Decompress)
                        Using output As New FileStream(f, FileMode.Create, FileAccess.Write)
                            input.CopyTo(output)
                        End Using
                    End Using
                Catch e As Exception
                    Throw New InvalidOperationException("Error extracting resource file", e)
                End Try
                log.info("Extracted {} to {}", tempFile, f)

                'Check extracted file hash:
                Dim extractedHash As String = sha256(f)
                If Not String.Equals(extractedHash, sha256Uncompressed, StringComparison.OrdinalIgnoreCase) Then
                    Throw New InvalidOperationException("Extracted file hash does not match expected hash: " & remotePath & " -> " & Path.GetFullPath(f) & " - expected has " & sha256Uncompressed & ", actual hash " & extractedHash)
                End If
            Finally
                Try
                    If Directory.Exists(tempDir) Then Directory.Delete(tempDir, True)
                Catch
                End Try
            End Try

            Return f
        End Function

        Private Shared Function FileNameOf(ByVal remotePath As String) As String
            Dim p As String = remotePath
            Dim q As Integer = p.IndexOf("?"c)
            If q >= 0 Then p = p.Substring(0, q)
            Dim lastSlash As Integer = p.LastIndexOf("/"c)
            Return If(lastSlash < 0, p, p.Substring(lastSlash + 1))
        End Function

        ''' <summary>Lowercase hexadecimal SHA-256 digest of the file content.</summary>
        Public Shared Function sha256(ByVal f As String) As String
            Try
                Using sha As System.Security.Cryptography.SHA256 = System.Security.Cryptography.SHA256.Create()
                    Using fs As New FileStream(f, FileMode.Open, FileAccess.Read, FileShare.Read)
                        Return BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "").ToLowerInvariant()
                    End Using
                End Using
            Catch e As IOException
                Throw New InvalidOperationException("Error when hashing file: " & f, e)
            End Try
        End Function

        ''' <summary>JSON options matching the Java mapper: unknown properties ignored, indented output, case-insensitive names.</summary>
        Public Shared Function newMapper() As JsonSerializerOptions
            Dim ret As New JsonSerializerOptions()
            ret.PropertyNameCaseInsensitive = True
            ret.WriteIndented = True
            ret.ReadCommentHandling = JsonCommentHandling.Skip
            ret.AllowTrailingCommas = True
            Return ret
        End Function

        Public Function toJson() As String
            Return JsonSerializer.Serialize(Me, MAPPER)
        End Function
    End Class

End Namespace
