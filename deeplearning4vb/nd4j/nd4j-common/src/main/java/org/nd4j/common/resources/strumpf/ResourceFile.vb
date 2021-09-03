Imports System
Imports System.Collections.Generic
Imports System.IO
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports Files = org.nd4j.shade.guava.io.Files
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DigestUtils = org.apache.commons.codec.digest.DigestUtils
Imports GzipCompressorInputStream = org.apache.commons.compress.compressors.gzip.GzipCompressorInputStream
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature

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

Namespace org.nd4j.common.resources.strumpf


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data @JsonIgnoreProperties("filePath") @Slf4j public class ResourceFile
	Public Class ResourceFile
		''' <summary>
		''' Default value for resource downloading connection timeout - see <seealso cref="ND4JSystemProperties.RESOURCES_CONNECTION_TIMEOUT"/>
		''' </summary>
		Public Const DEFAULT_CONNECTION_TIMEOUT As Integer = 60000 'Timeout for connections to be established
		''' <summary>
		''' Default value for resource downloading read timeout - see <seealso cref="ND4JSystemProperties.RESOURCES_READ_TIMEOUT"/>
		''' </summary>
		Public Const DEFAULT_READ_TIMEOUT As Integer = 60000 'Timeout for amount of time between connection established and data is available
		Protected Friend Const PATH_KEY As String = "full_remote_path"
		Protected Friend Const HASH As String = "_hash"
		Protected Friend Const COMPRESSED_HASH As String = "_compressed_hash"

		Protected Friend Const MAX_DOWNLOAD_ATTEMPTS As Integer = 3

		Public Shared ReadOnly MAPPER As ObjectMapper = newMapper()

		'Note: Field naming to match Strumpf JSON format
		Protected Friend current_version As Integer
		Protected Friend v1 As IDictionary(Of String, String)

		'Not in JSON:
		Protected Friend filePath As String

		Public Shared Function fromFile(ByVal path As String) As ResourceFile
			Return fromFile(New File(path))
		End Function

		Public Shared Function fromFile(ByVal file As File) As ResourceFile
			Dim s As String
			Try
				s = FileUtils.readFileToString(file, StandardCharsets.UTF_8)
				Dim rf As ResourceFile = MAPPER.readValue(s, GetType(ResourceFile))
				rf.setFilePath(file.getPath())
				Return rf
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function relativePath() As String
			Dim hashKey As String = Nothing
			For Each key As String In v1.Keys
				If key.EndsWith(HASH, StringComparison.Ordinal) AndAlso Not key.EndsWith(COMPRESSED_HASH, StringComparison.Ordinal) Then
					hashKey = key
					Exit For
				End If
			Next key
			If hashKey Is Nothing Then
				Throw New System.InvalidOperationException("Could not find <filename>_hash in resource reference file: " & filePath)
			End If

'JAVA TO VB CONVERTER NOTE: The local variable relativePath was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim relativePath_Conflict As String = hashKey.Substring(0, hashKey.Length - 5) '-5 to remove "_hash" suffix
			Return relativePath_Conflict.replaceAll("\\", "/")
		End Function

		Public Overridable Function localFileExistsAndValid(ByVal cacheRootDir As File) As Boolean

			Dim file As File = getLocalFile(cacheRootDir)
			If Not file.exists() Then
				Return False
			End If

			'File exists... but is it valid?
			Dim sha256Property As String = relativePath() & HASH
			Dim expSha256 As String = v1(sha256Property)

			Preconditions.checkState(expSha256 IsNot Nothing, "Expected JSON property %s was not found in resource reference file %s", sha256Property, filePath)

			Dim actualSha256 As String = sha256(file)
			If Not expSha256.Equals(actualSha256) Then
				Return False
			End If
			Return True
		End Function

		''' <summary>
		''' Get the local file - or where it *would* be if it has been downloaded. If it does not exist, it will not be downloaded here
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable Function getLocalFile(ByVal cacheRootDir As File) As File
			Dim relativePath As String = Me.relativePath()

			'For resolving local files with different versions, we want paths like:
			' ".../dir/filename.txt__v1/filename.txt"
			' ".../dir/filename.txt__v2/filename.txt"
			'This is to support multiple versions of files simultaneously... for example, different projects needing different
			' versions, or supporting old versions of resource files etc

			Dim lastSlash As Integer = Math.Max(relativePath.LastIndexOf("/"c), relativePath.LastIndexOf("\"c))
			Dim filename As String
			If lastSlash < 0 Then
				filename = relativePath
			Else
				filename = relativePath.Substring(lastSlash + 1)
			End If

			Dim parentDir As New File(cacheRootDir, relativePath & "__v" & current_version)
			Dim file As New File(parentDir, filename)
			Return file
		End Function

		''' <summary>
		''' Get the local file - downloading and caching if required
		''' 
		''' @return
		''' </summary>
		Public Overridable Function localFile(ByVal cacheRootDir As File) As File
			If localFileExistsAndValid(cacheRootDir) Then
				Return getLocalFile(cacheRootDir)
			End If

			'Need to download and extract...
			Dim remotePath As String = v1(PATH_KEY)
			Preconditions.checkState(remotePath IsNot Nothing, "No remote path was found in resource reference file %s", filePath)
			Dim f As File = getLocalFile(cacheRootDir)

			Dim tempDir As File = Files.createTempDir()
			Dim tempFile As New File(tempDir, FilenameUtils.getName(remotePath))

			Dim sha256PropertyCompressed As String = relativePath() & COMPRESSED_HASH

			Dim sha256Compressed As String = v1(sha256PropertyCompressed)
			Preconditions.checkState(sha256Compressed IsNot Nothing, "Expected JSON property %s was not found in resource reference file %s", sha256PropertyCompressed, filePath)

			Dim sha256Property As String = relativePath() & HASH
			Dim sha256Uncompressed As String = v1(sha256Property)

			Dim connTimeoutStr As String = System.getProperty(ND4JSystemProperties.RESOURCES_CONNECTION_TIMEOUT)
			Dim readTimeoutStr As String = System.getProperty(ND4JSystemProperties.RESOURCES_READ_TIMEOUT)
			Dim validCTimeout As Boolean = connTimeoutStr IsNot Nothing AndAlso connTimeoutStr.matches("\d+")
			Dim validRTimeout As Boolean = readTimeoutStr IsNot Nothing AndAlso readTimeoutStr.matches("\d+")

			Dim connectTimeout As Integer = If(validCTimeout, Integer.Parse(connTimeoutStr), DEFAULT_CONNECTION_TIMEOUT)
			Dim readTimeout As Integer = If(validRTimeout, Integer.Parse(readTimeoutStr), DEFAULT_READ_TIMEOUT)

			Try
				Dim correctHash As Boolean = False
				For tryCount As Integer = 0 To MAX_DOWNLOAD_ATTEMPTS - 1
					Try
						If tempFile.exists() Then
							tempFile.delete()
						End If
						log.info("Downloading remote resource {} to {}", remotePath, tempFile)
						FileUtils.copyURLToFile(New URL(remotePath), tempFile, connectTimeout, readTimeout)
						'Now: check if downloaded archive hash is OK
'JAVA TO VB CONVERTER NOTE: The variable hash was renamed since Visual Basic does not handle local variables named the same as class members well:
						Dim hash_Conflict As String = sha256(tempFile)
						correctHash = sha256Compressed.Equals(hash_Conflict)
						If Not correctHash Then
							log.warn("Download of file {} failed: expected hash {} vs. actual hash {}", remotePath, sha256Compressed, hash_Conflict)
							Continue For
						End If
						log.info("Downloaded {} to temporary file {}", remotePath, tempFile)
						Exit For
					Catch t As Exception
						If tryCount = MAX_DOWNLOAD_ATTEMPTS - 1 Then
							Throw New Exception("Error downloading test resource: " & remotePath, t)
						End If
						log.warn("Error downloading test resource, retrying... {}", remotePath, t)
					End Try
				Next tryCount

				If Not correctHash Then
					Throw New Exception("Could not successfully download with correct hash file after " & MAX_DOWNLOAD_ATTEMPTS & " attempts: " & remotePath)
				End If

				'Now, extract:
				f.getParentFile().mkdirs()
				Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using os As System.IO.Stream_Output = new BufferedOutputStream(new System.IO.FileStream(f, System.IO.FileMode.Create, System.IO.FileAccess.Write)), is As System.IO.Stream_Input = new BufferedInputStream(new org.apache.commons.compress.compressors.gzip.GzipCompressorInputStream(new System.IO.FileStream(tempFile, System.IO.FileMode.Open, System.IO.FileAccess.Read)))
						New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write)), [is] As Stream = New BufferedInputStream(New GzipCompressorInputStream(New FileStream(tempFile, FileMode.Open, FileAccess.Read)))
							Using os As Stream = New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write)), [is] As Stream
						IOUtils.copy([is], os)
						End Using
				Catch e As IOException
					Throw New Exception("Error extracting resource file", e)
				End Try
				log.info("Extracted {} to {}", tempFile, f)

				'Check extracted file hash:
				Dim extractedHash As String = sha256(f)
				If Not extractedHash.Equals(sha256Uncompressed) Then
					Throw New Exception("Extracted file hash does not match expected hash: " & remotePath & " -> " & f.getAbsolutePath() & " - expected has " & sha256Uncompressed & ", actual hash " & extractedHash)
				End If

			Finally
				tempFile.delete()
			End Try

			Return f
		End Function

		Public Shared Function sha256(ByVal f As File) As String
			Try
					Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
					Return DigestUtils.sha256Hex([is])
					End Using
			Catch e As IOException
				Throw New Exception("Error when hashing file: " & f.getPath(), e)
			End Try
		End Function


		Public Shared Function newMapper() As ObjectMapper
			Dim ret As New ObjectMapper()
			ret.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			ret.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			ret.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			ret.enable(SerializationFeature.INDENT_OUTPUT)
			Return ret
		End Function
	End Class

End Namespace