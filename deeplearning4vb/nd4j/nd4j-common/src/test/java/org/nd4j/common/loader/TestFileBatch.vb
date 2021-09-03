Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports FileBatch = org.nd4j.common.loader.FileBatch
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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


	Public Class TestFileBatch



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFileBatch(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFileBatch(ByVal testDir As Path)
			Dim baseDir As File = testDir.toFile()

			Dim fileList As IList(Of File) = New List(Of File)()
			For i As Integer = 0 To 9
				Dim s As String = "File contents - file " & i
				Dim f As New File(baseDir, "origFile" & i & ".txt")
				FileUtils.writeStringToFile(f, s, StandardCharsets.UTF_8)
				fileList.Add(f)
			Next i

			Dim fb As FileBatch = FileBatch.forFiles(fileList)

			assertEquals(10, fb.getFileBytes().size())
			assertEquals(10, fb.getOriginalUris().size())
			For i As Integer = 0 To 9
				Dim expBytes() As SByte = ("File contents - file " & i).GetBytes(Encoding.UTF8)
				Dim actBytes() As SByte = fb.getFileBytes().get(i)
				assertArrayEquals(expBytes, actBytes)

				Dim expPath As String = fileList(i).toURI().ToString()
				Dim actPath As String = fb.getOriginalUris().get(i)
				assertEquals(expPath, actPath)
			Next i

			'Save and load:
			Dim baos As New MemoryStream()
			fb.writeAsZip(baos)
			Dim asBytes() As SByte = baos.toByteArray()

			Dim fb2 As FileBatch
			Using bais As New MemoryStream(asBytes)
				fb2 = FileBatch.readFromZip(bais)
			End Using

			assertEquals(fb.getOriginalUris(), fb2.getOriginalUris())
			assertEquals(10, fb2.getFileBytes().size())
			For i As Integer = 0 To 9
				assertArrayEquals(fb.getFileBytes().get(i), fb2.getFileBytes().get(i))
			Next i

			'Check that it is indeed a valid zip file:
			Dim f As File = Files.createTempFile(testDir,"testfile","zip").toFile()
			fb.writeAsZip(f)

			Dim zf As New ZipFile(f)
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Iterator<? extends java.util.zip.ZipEntry> e = zf.entries();
			Dim e As IEnumerator(Of ZipEntry) = zf.entries()
			Dim count As Integer = 0
			Dim names As ISet(Of String) = New HashSet(Of String)()
			Do While e.MoveNext()
				Dim entry As ZipEntry = e.Current
				names.Add(entry.getName())
			Loop

			zf.close()
			assertEquals(11, names.Count) '10 files, 1 "original file names" file
			assertTrue(names.Contains(FileBatch.ORIGINAL_PATHS_FILENAME))
			For i As Integer = 0 To 9
				Dim n As String = "file_" & i & ".txt"
				assertTrue(names.Contains(n),n)
			Next i
		End Sub

	End Class

End Namespace