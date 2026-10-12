Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports StringUtils = org.apache.commons.lang3.StringUtils

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class FileBatch implements Serializable
	<Serializable>
	Public Class FileBatch
		''' <summary>
		''' Name of the file in the zip file that contains the original paths/filenames
		''' </summary>
		Public Const ORIGINAL_PATHS_FILENAME As String = "originalUris.txt"

		Private ReadOnly fileBytes As IList(Of SByte())
		Private ReadOnly originalUris As IList(Of String)

		''' <summary>
		''' Read a FileBatch from the specified file. This method assumes the FileBatch was previously saved to
		''' zip format using <seealso cref="writeAsZip(File)"/> or <seealso cref="writeAsZip(OutputStream)"/>
		''' </summary>
		''' <param name="file"> File to read from </param>
		''' <returns> The loaded FileBatch </returns>
		''' <exception cref="IOException"> If an error occurs during reading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static FileBatch readFromZip(File file) throws IOException
		Public Shared Function readFromZip(ByVal file As File) As FileBatch
			Using fis As New FileStream(file, FileMode.Open, FileAccess.Read)
				Return readFromZip(fis)
			End Using
		End Function

		''' <summary>
		''' Read a FileBatch from the specified input stream. This method assumes the FileBatch was previously saved to
		''' zip format using <seealso cref="writeAsZip(File)"/> or <seealso cref="writeAsZip(OutputStream)"/>
		''' </summary>
		''' <param name="is"> Input stream to read from </param>
		''' <returns> The loaded FileBatch </returns>
		''' <exception cref="IOException"> If an error occurs during reading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static FileBatch readFromZip(InputStream is) throws IOException
		Public Shared Function readFromZip(ByVal [is] As Stream) As FileBatch
			Dim originalUris As String = Nothing
			Dim bytesMap As IDictionary(Of Integer, SByte()) = New Dictionary(Of Integer, SByte())()
			Using zis As New java.util.zip.ZipInputStream(New BufferedInputStream([is]))
				Dim ze As ZipEntry
				ze = zis.getNextEntry()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((ze = zis.getNextEntry()) != null)
				Do While ze IsNot Nothing
					Dim name As String = ze.getName()
					Dim bytes() As SByte = IOUtils.toByteArray(zis)
					If name.Equals(ORIGINAL_PATHS_FILENAME) Then
						originalUris = StringHelper.NewString(bytes, 0, bytes.Length, StandardCharsets.UTF_8)
					Else
						Dim idxSplit As Integer = name.IndexOf("_", StringComparison.Ordinal)
						Dim idxSplit2 As Integer = name.IndexOf(".", StringComparison.Ordinal)
						Dim fileIdx As Integer = Integer.Parse(name.Substring(idxSplit + 1, idxSplit2 - (idxSplit + 1)))
						bytesMap(fileIdx) = bytes
					End If
						ze = zis.getNextEntry()
				Loop
			End Using

			Dim list As IList(Of SByte()) = New List(Of SByte())(bytesMap.Count)
			For i As Integer = 0 To bytesMap.Count - 1
				list.Add(bytesMap(i))
			Next i

			Dim origPaths As IList(Of String) = New List(Of String) From {originalUris.Split(vbLf, True)}
			Return New FileBatch(list, origPaths)
		End Function

		''' <summary>
		''' Create a FileBatch from the specified files
		''' </summary>
		''' <param name="files"> Files to create the FileBatch from </param>
		''' <returns> The created FileBatch </returns>
		''' <exception cref="IOException"> If an error occurs during reading of the file content </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static FileBatch forFiles(File... files) throws IOException
		Public Shared Function forFiles(ParamArray ByVal files() As File) As FileBatch
			Return forFiles(java.util.Arrays.asList(files))
		End Function

		''' <summary>
		''' Create a FileBatch from the specified files
		''' </summary>
		''' <param name="files"> Files to create the FileBatch from </param>
		''' <returns> The created FileBatch </returns>
		''' <exception cref="IOException"> If an error occurs during reading of the file content </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static FileBatch forFiles(List<File> files) throws IOException
		Public Shared Function forFiles(ByVal files As IList(Of File)) As FileBatch
			Dim origPaths As IList(Of String) = New List(Of String)(files.Count)
			Dim bytes As IList(Of SByte()) = New List(Of SByte())(files.Count)
			For Each f As File In files
				bytes.Add(FileUtils.readFileToByteArray(f))
				origPaths.Add(f.toURI().ToString())
			Next f
			Return New FileBatch(bytes, origPaths)
		End Function

		''' <summary>
		''' Write the FileBatch to the specified File, in zip file format
		''' </summary>
		''' <param name="f"> File to write to </param>
		''' <exception cref="IOException"> If an error occurs during writing </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void writeAsZip(File f) throws IOException
		Public Overridable Sub writeAsZip(ByVal f As File)
			writeAsZip(New FileStream(f, FileMode.Create, FileAccess.Write))
		End Sub

		''' <param name="os"> Write the FileBatch to the specified output stream, in zip file format </param>
		''' <exception cref="IOException"> If an error occurs during writing </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void writeAsZip(OutputStream os) throws IOException
		Public Overridable Sub writeAsZip(ByVal os As Stream)
			Using zos As New java.util.zip.ZipOutputStream(New BufferedOutputStream(os))

				'Write original paths as a text file:
				Dim ze As New ZipEntry(ORIGINAL_PATHS_FILENAME)
				Dim originalUrisJoined As String = StringUtils.join(originalUris, vbLf) 'Java String.join is Java 8
				zos.putNextEntry(ze)
				zos.write(originalUrisJoined.GetBytes(Encoding.UTF8))

				For i As Integer = 0 To fileBytes.Count - 1
					Dim ext As String = FilenameUtils.getExtension(originalUris(i))
					If ext Is Nothing OrElse ext.Length = 0 Then
						ext = "bin"
					End If
					Dim name As String = "file_" & i & "." & ext
					ze = New ZipEntry(name)
					zos.putNextEntry(ze)
					zos.write(fileBytes(i))
				Next i
			End Using
		End Sub
	End Class

End Namespace