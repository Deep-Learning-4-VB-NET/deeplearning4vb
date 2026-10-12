Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArchiveEntry = org.apache.commons.compress.archivers.ArchiveEntry
Imports TarArchiveEntry = org.apache.commons.compress.archivers.tar.TarArchiveEntry
Imports TarArchiveInputStream = org.apache.commons.compress.archivers.tar.TarArchiveInputStream
Imports GzipCompressorInputStream = org.apache.commons.compress.compressors.gzip.GzipCompressorInputStream
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Preconditions = org.nd4j.common.base.Preconditions

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
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ArchiveUtils
	Public Class ArchiveUtils

		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Extracts all files from the archive to the specified destination.<br>
		''' Note: Logs the path of all extracted files by default. Use <seealso cref="unzipFileTo(String, String, Boolean)"/> if
		''' logging is not desired.<br>
		''' Can handle .zip, .jar, .tar.gz, .tgz, .tar, and .gz formats.
		''' Format is interpreted from the filename
		''' </summary>
		''' <param name="file"> the file to extract the files from </param>
		''' <param name="dest"> the destination directory. Will be created if it does not exist </param>
		''' <exception cref="IOException"> If an error occurs accessing the files or extracting </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void unzipFileTo(String file, String dest) throws IOException
		Public Shared Sub unzipFileTo(ByVal file As String, ByVal dest As String)
			unzipFileTo(file, dest, True)
		End Sub

		''' <summary>
		''' Extracts all files from the archive to the specified destination, optionally logging the extracted file path.<br>
		''' Can handle .zip, .jar, .tar.gz, .tgz, .tar, and .gz formats.
		''' Format is interpreted from the filename
		''' </summary>
		''' <param name="file">     the file to extract the files from </param>
		''' <param name="dest">     the destination directory. Will be created if it does not exist </param>
		''' <param name="logFiles"> If true: log the path of every extracted file; if false do not log </param>
		''' <exception cref="IOException"> If an error occurs accessing the files or extracting </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void unzipFileTo(String file, String dest, boolean logFiles) throws IOException
		Public Shared Sub unzipFileTo(ByVal file As String, ByVal dest As String, ByVal logFiles As Boolean)
			Dim target As New File(file)
			If Not target.exists() Then
				Throw New System.ArgumentException("Archive doesnt exist")
			End If
			If Not Directory.Exists(dest) OrElse File.Exists(dest) Then
				Directory.CreateDirectory(dest)
			End If
			Dim fin As New FileStream(target, FileMode.Open, FileAccess.Read)
			Dim BUFFER As Integer = 2048
			Dim data(BUFFER - 1) As SByte

			If file.EndsWith(".zip", StringComparison.Ordinal) OrElse file.EndsWith(".jar", StringComparison.Ordinal) Then
				Using zis As New java.util.zip.ZipInputStream(fin)
					'get the zipped file list entry
					Dim ze As ZipEntry = zis.getNextEntry()

					Do While ze IsNot Nothing
						Dim fileName As String = ze.getName()

						Dim canonicalDestinationDirPath As String = (New File(dest)).getCanonicalPath()
						Dim newFile As New File(dest & File.separator & fileName)
						Dim canonicalDestinationFile As String = newFile.getCanonicalPath()

						If Not canonicalDestinationFile.StartsWith(canonicalDestinationDirPath & File.separator, StringComparison.Ordinal) Then
							log.debug("Attempt to unzip entry is outside of the target dir")
							Throw New IOException("Entry is outside of the target dir: ")
						End If

						If ze.isDirectory() Then
							newFile.mkdirs()
							zis.closeEntry()
							ze = zis.getNextEntry()
							Continue Do
						End If

						Dim fos As New FileStream(newFile, FileMode.Create, FileAccess.Write)

						Dim len As Integer
						len = zis.read(data)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((len = zis.read(data)) > 0)
						Do While len > 0
							fos.Write(data, 0, len)
								len = zis.read(data)
						Loop

						fos.Close()
						ze = zis.getNextEntry()
						If logFiles Then
							log.info("File extracted: " & newFile.getAbsoluteFile())
						End If
					Loop

					zis.closeEntry()
				End Using
			ElseIf file.EndsWith(".tar.gz", StringComparison.Ordinal) OrElse file.EndsWith(".tgz", StringComparison.Ordinal) OrElse file.EndsWith(".tar", StringComparison.Ordinal) Then
				Dim [in] As New BufferedInputStream(fin)
				Dim tarIn As TarArchiveInputStream
				If file.EndsWith(".tar", StringComparison.Ordinal) Then
					'Not compressed
					tarIn = New TarArchiveInputStream([in])
				Else
					Dim gzIn As New GzipCompressorInputStream([in])
					 tarIn = New TarArchiveInputStream(gzIn)
				End If

				Dim entry As TarArchiveEntry
				' Read the tar entries using the getNextEntry method *
				entry = CType(tarIn.getNextEntry(), org.apache.commons.compress.archivers.tar.TarArchiveEntry)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((entry = (org.apache.commons.compress.archivers.tar.TarArchiveEntry) tarIn.getNextEntry()) != null)
				Do While entry IsNot Nothing
					If logFiles Then
						log.info("Extracting: " & entry.getName())
					End If
					' If the entry is a directory, create the directory. 

					If entry.isDirectory() Then
						Dim f As New File(dest & File.separator + entry.getName())
						f.mkdirs()
	'                
	'                 * If the entry is a file,write the decompressed file to the disk
	'                 * and close destination stream.
	'                 
					Else
						Dim count As Integer
						Using fos As New FileStream(dest & File.separator + entry.getName(), FileMode.Create, FileAccess.Write), destStream As New BufferedOutputStream(fos, BUFFER), 
							count = tarIn.read(data, 0, BUFFER)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((count = tarIn.read(data, 0, BUFFER)) != -1)
							Do While count <> -1
								destStream.write(data, 0, count)
									count = tarIn.read(data, 0, BUFFER)
							Loop

							destStream.flush()
							IOUtils.closeQuietly(destStream)
						End Using
					End If
						entry = CType(tarIn.getNextEntry(), TarArchiveEntry)
				Loop

				' Close the input stream
				tarIn.close()
			ElseIf file.EndsWith(".gz", StringComparison.Ordinal) Then
				Dim extracted As New File(target.getParent(), target.getName().replace(".gz", ""))
				If extracted.exists() Then
					extracted.delete()
				End If
				extracted.createNewFile()
				Using is2 As New java.util.zip.GZIPInputStream(fin), fos As Stream = org.apache.commons.io.FileUtils.openOutputStream(extracted)
					IOUtils.copyLarge(is2, fos)
					fos.Flush()
				End Using
			Else
				Throw New System.InvalidOperationException("Unable to infer file type (compression format) from source file name: " & file)
			End If
			target.delete()
		End Sub

		''' <summary>
		''' List all of the files and directories in the specified tar.gz file
		''' </summary>
		''' <param name="tarFile"> A .tar file </param>
		''' <returns> List of files and directories </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.List<String> tarListFiles(File tarFile) throws IOException
		Public Shared Function tarListFiles(ByVal tarFile As File) As IList(Of String)
			Preconditions.checkState(Not tarFile.getPath().EndsWith(".tar.gz"), ".tar.gz files should not use this method - use tarGzListFiles instead")
			Return tarGzListFiles(tarFile, False)
		End Function

		''' <summary>
		''' List all of the files and directories in the specified tar.gz file
		''' </summary>
		''' <param name="tarGzFile"> A tar.gz file </param>
		''' <returns> List of files and directories </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.List<String> tarGzListFiles(File tarGzFile) throws IOException
		Public Shared Function tarGzListFiles(ByVal tarGzFile As File) As IList(Of String)
			Return tarGzListFiles(tarGzFile, True)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static java.util.List<String> tarGzListFiles(File file, boolean isTarGz) throws IOException
		Protected Friend Shared Function tarGzListFiles(ByVal file As File, ByVal isTarGz As Boolean) As IList(Of String)
			Using tin As org.apache.commons.compress.archivers.tar.TarArchiveInputStream = If(isTarGz, New org.apache.commons.compress.archivers.tar.TarArchiveInputStream(New java.util.zip.GZIPInputStream(New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read)))), New org.apache.commons.compress.archivers.tar.TarArchiveInputStream(New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))))
				Dim entry As ArchiveEntry
				Dim [out] As IList(Of String) = New List(Of String)()
				entry = tin.getNextTarEntry()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((entry = tin.getNextTarEntry()) != null)
				Do While entry IsNot Nothing
					Dim name As String = entry.getName()
					[out].Add(name)
						entry = tin.getNextTarEntry()
				Loop
				Return [out]
			End Using
		End Function

		''' <summary>
		''' List all of the files and directories in the specified .zip file
		''' </summary>
		''' <param name="zipFile"> Zip file </param>
		''' <returns> List of files and directories </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static java.util.List<String> zipListFiles(File zipFile) throws IOException
		Public Shared Function zipListFiles(ByVal zipFile As File) As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			Using zf As New java.util.zip.ZipFile(zipFile)
				Dim entries As System.Collections.IEnumerator = zf.entries()
				Do While entries.MoveNext()
					Dim ze As ZipEntry = CType(entries.Current, ZipEntry)
					[out].Add(ze.getName())
				Loop
			End Using
			Return [out]
		End Function

		''' <summary>
		''' Extract a single file from a .zip file. Does not support directories
		''' </summary>
		''' <param name="zipFile">     Zip file to extract from </param>
		''' <param name="destination"> Destination file </param>
		''' <param name="pathInZip">   Path in the zip to extract </param>
		''' <exception cref="IOException"> If exception occurs while reading/writing </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void zipExtractSingleFile(File zipFile, File destination, String pathInZip) throws IOException
		Public Shared Sub zipExtractSingleFile(ByVal zipFile As File, ByVal destination As File, ByVal pathInZip As String)
			Using zf As New java.util.zip.ZipFile(zipFile), [is] As Stream = New BufferedInputStream(zf.getInputStream(zf.getEntry(pathInZip))), os As Stream = New BufferedOutputStream(New FileStream(destination, FileMode.Create, FileAccess.Write))
				IOUtils.copy([is], os)
			End Using
		End Sub

		''' <summary>
		''' Extract a single file from a tar.gz file. Does not support directories.
		''' NOTE: This should not be used for batch extraction of files, due to the need to iterate over the entries until the
		''' specified entry is found. Use <seealso cref="unzipFileTo(String, String)"/> for batch extraction instead
		''' </summary>
		''' <param name="tarGz">       A tar.gz file </param>
		''' <param name="destination"> The destination file to extract to </param>
		''' <param name="pathInTarGz"> The path in the tar.gz file to extract </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void tarGzExtractSingleFile(File tarGz, File destination, String pathInTarGz) throws IOException
		Public Shared Sub tarGzExtractSingleFile(ByVal tarGz As File, ByVal destination As File, ByVal pathInTarGz As String)
			Using tin As New org.apache.commons.compress.archivers.tar.TarArchiveInputStream(New java.util.zip.GZIPInputStream(New BufferedInputStream(New FileStream(tarGz, FileMode.Open, FileAccess.Read))))
				Dim entry As ArchiveEntry
				Dim extracted As Boolean = False
				entry = tin.getNextTarEntry()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((entry = tin.getNextTarEntry()) != null)
				Do While entry IsNot Nothing
					Dim name As String = entry.getName()
					If pathInTarGz.Equals(name) Then
						Using os As Stream = New BufferedOutputStream(New FileStream(destination, FileMode.Create, FileAccess.Write))
							IOUtils.copy(tin, os)
						End Using
						extracted = True
					End If
						entry = tin.getNextTarEntry()
				Loop
				Preconditions.checkState(extracted, "No file was extracted. File not found? %s", pathInTarGz)
			End Using
		End Sub
	End Class

End Namespace