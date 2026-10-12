Imports System
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DigestUtils = org.apache.commons.codec.digest.DigestUtils
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports ArchiveUtils = org.nd4j.common.util.ArchiveUtils

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

Namespace org.nd4j.common.resources


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Downloader
	Public Class Downloader
		''' <summary>
		''' Default connection timeout in milliseconds when using <seealso cref="FileUtils.copyURLToFile(URL, File, Integer, Integer)"/>
		''' </summary>
		Public Const DEFAULT_CONNECTION_TIMEOUT As Integer = 60000
		''' <summary>
		''' Default read timeout in milliseconds when using <seealso cref="FileUtils.copyURLToFile(URL, File, Integer, Integer)"/>
		''' </summary>
		Public Const DEFAULT_READ_TIMEOUT As Integer = 60000

		Private Sub New()
		End Sub

		''' <summary>
		''' As per <seealso cref="download(String, URL, File, String, Integer, Integer, Integer)"/> with the connection and read timeouts
		''' set to their default values - <seealso cref="DEFAULT_CONNECTION_TIMEOUT"/> and <seealso cref="DEFAULT_READ_TIMEOUT"/> respectively
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void download(String name, java.net.URL url, java.io.File f, String targetMD5, int maxTries) throws java.io.IOException
		Public Shared Sub download(ByVal name As String, ByVal url As URL, ByVal f As File, ByVal targetMD5 As String, ByVal maxTries As Integer)
			download(name, url, f, targetMD5, maxTries, DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)
		End Sub

		''' <summary>
		''' Download the specified URL to the specified file, and verify that the target MD5 matches
		''' </summary>
		''' <param name="name">              Name (mainly for providing useful exceptions) </param>
		''' <param name="url">               URL to download </param>
		''' <param name="f">                 Destination file </param>
		''' <param name="targetMD5">         Expected MD5 for file </param>
		''' <param name="maxTries">          Maximum number of download attempts before failing and throwing an exception </param>
		''' <param name="connectionTimeout"> connection timeout in milliseconds, as used by <seealso cref="org.apache.commons.io.FileUtils.copyURLToFile(URL, File, Integer, Integer)"/> </param>
		''' <param name="readTimeout">       read timeout in milliseconds, as used by <seealso cref="org.apache.commons.io.FileUtils.copyURLToFile(URL, File, Integer, Integer)"/> </param>
		''' <exception cref="IOException"> If an error occurs during downloading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void download(String name, java.net.URL url, java.io.File f, String targetMD5, int maxTries, int connectionTimeout, int readTimeout) throws java.io.IOException
		Public Shared Sub download(ByVal name As String, ByVal url As URL, ByVal f As File, ByVal targetMD5 As String, ByVal maxTries As Integer, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
			download(name, url, f, targetMD5, maxTries, 0, connectionTimeout, readTimeout)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void download(String name, java.net.URL url, java.io.File f, String targetMD5, int maxTries, int attempt, int connectionTimeout, int readTimeout) throws java.io.IOException
		Private Shared Sub download(ByVal name As String, ByVal url As URL, ByVal f As File, ByVal targetMD5 As String, ByVal maxTries As Integer, ByVal attempt As Integer, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
			doOrWait(f.getParentFile(), Sub()
			Dim isCorrectFile As Boolean = f.exists() AndAlso f.isFile() AndAlso checkMD5OfFile(targetMD5, f)
			If attempt < maxTries Then
				If Not isCorrectFile Then
					FileUtils.copyURLToFile(url, f, connectionTimeout, readTimeout)
					If Not checkMD5OfFile(targetMD5, f) Then
						f.delete()
						download(name, url, f, targetMD5, maxTries, attempt + 1, connectionTimeout, readTimeout)
					End If
				End If
			ElseIf Not isCorrectFile Then
				Throw New IOException("Could not download " & name & " from " & url & vbLf & " properly despite trying " & maxTries & " times, check your connection.")
			End If
			End Sub)

		End Sub

		''' <summary>
		''' As per <seealso cref="downloadAndExtract(String, URL, File, File, String, Integer, Integer, Integer)"/> with the connection and read timeouts
		'''      * set to their default values - <seealso cref="DEFAULT_CONNECTION_TIMEOUT"/> and <seealso cref="DEFAULT_READ_TIMEOUT"/> respectively
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void downloadAndExtract(String name, java.net.URL url, java.io.File f, java.io.File extractToDir, String targetMD5, int maxTries) throws java.io.IOException
		Public Shared Sub downloadAndExtract(ByVal name As String, ByVal url As URL, ByVal f As File, ByVal extractToDir As File, ByVal targetMD5 As String, ByVal maxTries As Integer)
			downloadAndExtract(name, url, f, extractToDir, targetMD5, maxTries, DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)
		End Sub

		''' <summary>
		''' Download the specified URL to the specified file, verify that the MD5 matches, and then extract it to the specified directory.<br>
		''' Note that the file must be an archive, with the correct file extension: .zip, .jar, .tar.gz, .tgz or .gz
		''' </summary>
		''' <param name="name">         Name (mainly for providing useful exceptions) </param>
		''' <param name="url">          URL to download </param>
		''' <param name="f">            Destination file </param>
		''' <param name="extractToDir"> Destination directory to extract all files </param>
		''' <param name="targetMD5">    Expected MD5 for file </param>
		''' <param name="maxTries">     Maximum number of download attempts before failing and throwing an exception </param>
		''' <param name="connectionTimeout"> connection timeout in milliseconds, as used by <seealso cref="org.apache.commons.io.FileUtils.copyURLToFile(URL, File, Integer, Integer)"/> </param>
		''' <param name="readTimeout">       read timeout in milliseconds, as used by <seealso cref="org.apache.commons.io.FileUtils.copyURLToFile(URL, File, Integer, Integer)"/> </param>
		''' <exception cref="IOException"> If an error occurs during downloading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void downloadAndExtract(String name, java.net.URL url, java.io.File f, java.io.File extractToDir, String targetMD5, int maxTries, int connectionTimeout, int readTimeout) throws java.io.IOException
		Public Shared Sub downloadAndExtract(ByVal name As String, ByVal url As URL, ByVal f As File, ByVal extractToDir As File, ByVal targetMD5 As String, ByVal maxTries As Integer, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
			downloadAndExtract(0, maxTries, name, url, f, extractToDir, targetMD5, connectionTimeout, readTimeout)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void downloadAndExtract(int attempt, int maxTries, String name, java.net.URL url, java.io.File f, java.io.File extractToDir, String targetMD5, int connectionTimeout, int readTimeout) throws java.io.IOException
		Private Shared Sub downloadAndExtract(ByVal attempt As Integer, ByVal maxTries As Integer, ByVal name As String, ByVal url As URL, ByVal f As File, ByVal extractToDir As File, ByVal targetMD5 As String, ByVal connectionTimeout As Integer, ByVal readTimeout As Integer)
			doOrWait(f.getParentFile(), Sub()
			Dim isCorrectFile As Boolean = f.exists() AndAlso f.isFile() AndAlso checkMD5OfFile(targetMD5, f)
			If attempt < maxTries Then
				If Not isCorrectFile Then
					FileUtils.copyURLToFile(url, f, connectionTimeout, readTimeout)
					If Not checkMD5OfFile(targetMD5, f) Then
						f.delete()
						downloadAndExtract(attempt + 1, maxTries, name, url, f, extractToDir, targetMD5, connectionTimeout, readTimeout)
					End If
				End If
				Try
					ArchiveUtils.unzipFileTo(f.getAbsolutePath(), extractToDir.getAbsolutePath(), False)
				Catch t As Exception
					log.warn("Error extracting {} files from file {} - retrying...", name, f.getAbsolutePath(), t)
					f.delete()
					downloadAndExtract(attempt + 1, maxTries, name, url, f, extractToDir, targetMD5, connectionTimeout, readTimeout)
				End Try
			ElseIf Not isCorrectFile Then
				Throw New IOException("Could not download and extract " & name & " from " & url.getPath() & vbLf & " properly despite trying " & maxTries & " times, check your connection. File info:" & vbLf & "Target MD5: " & targetMD5 & vbLf & "Hash matches: " & checkMD5OfFile(targetMD5, f) & vbLf & "Is valid file: " & f.isFile())
			End If
			End Sub)
		End Sub

		''' <summary>
		''' Check the MD5 of the specified file </summary>
		''' <param name="targetMD5"> Expected MD5 </param>
		''' <param name="file">      File to check </param>
		''' <returns>          True if MD5 matches, false otherwise </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static boolean checkMD5OfFile(String targetMD5, java.io.File file) throws java.io.IOException
		Public Shared Function checkMD5OfFile(ByVal targetMD5 As String, ByVal file As File) As Boolean
			Dim [in] As Stream = FileUtils.openInputStream(file)
			Dim trueMd5 As String = DigestUtils.md5Hex([in])
			IOUtils.closeQuietly([in])
			Return (targetMD5.Equals(trueMd5))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void doOrWait(java.io.File flagDir, IOCallable block) throws java.io.IOException
		Private Shared Sub doOrWait(ByVal flagDir As File, ByVal block As IOCallable)
			Dim waitForFinish As Boolean = False
			If flagDir.exists() Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File lockFile = flagDir.toPath().resolve("inProgress.lock").toFile();
				Dim lockFile As File = flagDir.toPath().resolve("inProgress.lock").toFile()
				Dim flag As New RandomAccessFile(lockFile, "rw")
				Do
					Try
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.nio.channels.FileChannel channel = flag.getChannel();
					Dim channel As FileChannel = flag.getChannel()
					Try
							Using lock As FileLock = channel.lock()
							If Not waitForFinish Then
								block()
							End If
							End Using
					Finally
						lockFile.delete()
					End Try
					Return
				Catch e As Exception When TypeOf e Is OverlappingFileLockException OrElse TypeOf e Is FileLockInterruptionException
					' file is locked, someone else is already doing the work we want to do.
					' just wait until it is finished, there should be no need to actually do anything
					' once we can acquire that lock
					Try
						log.debug("Waiting to acquire download lock in dir {}", flagDir.getPath())
						waitForFinish = True
						Thread.Sleep(100)
					Catch ignored As InterruptedException
						' noop, we retry to acquire that lock
					End Try
				End Try
				Loop
			Else
				Throw New IOException("Target directory " & flagDir.getPath() & " must exist!")
			End If
		End Sub

		Private Delegate Sub IOCallable()

	End Class

End Namespace