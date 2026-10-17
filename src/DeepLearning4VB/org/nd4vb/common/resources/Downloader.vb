Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Threading
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory
Imports ArchiveUtils = DeepLearning4VB.org.nd4vb.common.util.ArchiveUtils

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

Namespace org.nd4vb.common.resources

    ''' <summary>
    ''' Downloads files (with MD5 verification and retries) and optionally extracts archives.
    ''' Concurrent downloads into the same directory are serialised with a lock file, as in Java.
    ''' Files are path strings; URLs are <see cref="Uri"/>s (<c>file://</c> URIs are copied locally).
    ''' </summary>
    Public NotInheritable Class Downloader
        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(Downloader))
        Public Const DEFAULT_CONNECTION_TIMEOUT As Integer = 60000
        Public Const DEFAULT_READ_TIMEOUT As Integer = 60000

        Shared Sub New()
#If NETFRAMEWORK Then
            ' .NET Framework defaults to old TLS versions; current download hosts need TLS 1.2.
            ' (.NET 10 uses the operating system's TLS defaults, and ServicePointManager is obsolete there.)
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol Or SecurityProtocolType.Tls12
#End If
        End Sub

        Private Sub New()
        End Sub

        Public Shared Sub download(ByVal name As String, ByVal url As Uri, ByVal f As String, ByVal targetMD5 As String, ByVal maxTries As Integer)
            download(name, url, f, targetMD5, maxTries, DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)
        End Sub

        Public Shared Sub download(ByVal name As String, ByVal url As Uri, ByVal f As String, ByVal targetMD5 As String, ByVal maxTries As Integer, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
            download(name, url, f, targetMD5, maxTries, 0, connectionTimeout, readTimeout)
        End Sub

        Private Shared Sub download(ByVal name As String, ByVal url As Uri, ByVal f As String, ByVal targetMD5 As String, ByVal maxTries As Integer, ByVal attempt As Integer, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
            ' Retries run as a loop inside one lock: the Java code recursed into doOrWait while still
            ' holding the lock, which can never be re-acquired.
            doOrWait(Path.GetDirectoryName(Path.GetFullPath(f)),
                     Sub()
                         Dim tries As Integer = attempt
                         Do
                             If File.Exists(f) AndAlso checkMD5OfFile(targetMD5, f) Then
                                 Return
                             End If
                             If tries >= maxTries Then
                                 Throw New IOException("Could not download " & name & " from " & url.ToString() & ControlChars.Lf & " properly despite trying " & maxTries & " times, check your connection.")
                             End If
                             copyURLToFile(url, f, connectionTimeout, readTimeout)
                             If checkMD5OfFile(targetMD5, f) Then
                                 Return
                             End If
                             File.Delete(f)
                             tries += 1
                         Loop
                     End Sub)
        End Sub

        Public Shared Sub downloadAndExtract(ByVal name As String, ByVal url As Uri, ByVal f As String, ByVal extractToDir As String, ByVal targetMD5 As String, ByVal maxTries As Integer)
            downloadAndExtract(name, url, f, extractToDir, targetMD5, maxTries, DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)
        End Sub

        Public Shared Sub downloadAndExtract(ByVal name As String, ByVal url As Uri, ByVal f As String, ByVal extractToDir As String, ByVal targetMD5 As String, ByVal maxTries As Integer, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
            downloadAndExtract(0, maxTries, name, url, f, extractToDir, targetMD5, connectionTimeout, readTimeout)
        End Sub

        Private Shared Sub downloadAndExtract(ByVal attempt As Integer, ByVal maxTries As Integer, ByVal name As String, ByVal url As Uri, ByVal f As String, ByVal extractToDir As String, ByVal targetMD5 As String, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
            doOrWait(Path.GetDirectoryName(Path.GetFullPath(f)),
                     Sub()
                         Dim tries As Integer = attempt
                         Do
                             Dim isCorrectFile As Boolean = File.Exists(f) AndAlso checkMD5OfFile(targetMD5, f)
                             If tries >= maxTries Then
                                 If isCorrectFile Then Return
                                 Throw New IOException("Could not download and extract " & name & " from " & url.AbsolutePath & ControlChars.Lf & " properly despite trying " & maxTries & " times, check your connection. File info:" & ControlChars.Lf & "Target MD5: " & targetMD5 & ControlChars.Lf & "Hash matches: " & If(File.Exists(f), checkMD5OfFile(targetMD5, f).ToString().ToLowerInvariant(), "false") & ControlChars.Lf & "Is valid file: " & File.Exists(f).ToString().ToLowerInvariant())
                             End If
                             If Not isCorrectFile Then
                                 copyURLToFile(url, f, connectionTimeout, readTimeout)
                                 If Not checkMD5OfFile(targetMD5, f) Then
                                     File.Delete(f)
                                     tries += 1
                                     Continue Do
                                 End If
                             End If
                             Try
                                 ArchiveUtils.unzipFileTo(Path.GetFullPath(f), Path.GetFullPath(extractToDir), False)
                                 Return
                             Catch t As Exception
                                 log.warn("Error extracting {} files from file {} - retrying...", name, Path.GetFullPath(f), t)
                                 If File.Exists(f) Then File.Delete(f)
                                 tries += 1
                             End Try
                         Loop
                     End Sub)
        End Sub

        ''' <summary>Compares the MD5 of the file (lowercase hex) with the expected digest.</summary>
        Public Shared Function checkMD5OfFile(ByVal targetMD5 As String, ByVal file As String) As Boolean
            Dim trueMd5 As String = md5Hex(file)
            Return String.Equals(targetMD5, trueMd5, StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>Lowercase hexadecimal MD5 digest of the file content.</summary>
        Public Shared Function md5Hex(ByVal file As String) As String
            Using md5 As MD5 = MD5.Create()
                Using fs As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Return BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "").ToLowerInvariant()
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Copies the content at the URL to the file (commons-io <c>FileUtils.copyURLToFile</c>).
        ''' <c>file:</c> URIs are copied directly; everything else is fetched with <see cref="HttpClient"/>.
        ''' </summary>
        Public Shared Sub copyURLToFile(ByVal url As Uri, ByVal destination As String, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
            If url Is Nothing Then Throw New ArgumentNullException(NameOf(url))
            Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(destination))
            If Not String.IsNullOrEmpty(dir) Then Directory.CreateDirectory(dir)
            If url.IsFile Then
                File.Copy(url.LocalPath, destination, True)
                Return
            End If
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromMilliseconds(Math.Max(1000, connectionTimeout + readTimeout))
                Using response As HttpResponseMessage = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult()
                    If Not response.IsSuccessStatusCode Then
                        Throw New IOException("Server returned HTTP response code: " & CInt(response.StatusCode) & " for URL: " & url.ToString())
                    End If
                    Using input As Stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult()
                        Using output As New FileStream(destination, FileMode.Create, FileAccess.Write)
                            input.CopyTo(output)
                        End Using
                    End Using
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Runs the block while holding an exclusive lock on <c>inProgress.lock</c> in the directory. If
        ''' another process holds the lock, waits for it and skips the block (the other process did the work).
        ''' </summary>
        Private Shared Sub doOrWait(ByVal flagDir As String, ByVal block As Action)
            If Not Directory.Exists(flagDir) Then
                Throw New IOException("Target directory " & flagDir & " must exist!")
            End If
            Dim lockFile As String = Path.Combine(flagDir, "inProgress.lock")
            Dim waitForFinish As Boolean = False
            Do
                Dim handle As FileStream = Nothing
                Try
                    handle = New FileStream(lockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
                Catch ex As IOException
                    ' file is locked, someone else is already doing the work we want to do; wait until it is finished
                    log.debug("Waiting to acquire download lock in dir {}", flagDir)
                    waitForFinish = True
                    Thread.Sleep(100)
                    Continue Do
                End Try
                Try
                    If Not waitForFinish Then
                        block()
                    End If
                Finally
                    handle.Dispose()
                    Try
                        File.Delete(lockFile)
                    Catch
                        ' another waiter may already have deleted it
                    End Try
                End Try
                Return
            Loop
        End Sub

    End Class

End Namespace
