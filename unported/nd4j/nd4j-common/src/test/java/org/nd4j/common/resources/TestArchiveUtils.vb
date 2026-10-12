Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
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


	Public Class TestArchiveUtils

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUnzipFileTo(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUnzipFileTo(ByVal testDir As Path)
			'random txt file
			Dim dir As File = testDir.toFile()
			Dim content As String = "test file content"
			Dim path As String = "myDir/myTestFile.txt"
			Dim testFile As New File(dir, path)
			testFile.getParentFile().mkdir()
			FileUtils.writeStringToFile(testFile, content, StandardCharsets.UTF_8)

			'zip it as test.zip
			Dim zipFile As New File(testFile.getParentFile(),"test.zip")
			Dim fos As New FileStream(zipFile, FileMode.Create, FileAccess.Write)
			Dim zipOut As New ZipOutputStream(fos)
			Dim fis As New FileStream(testFile, FileMode.Open, FileAccess.Read)
			Dim zipEntry As New ZipEntry(testFile.getName())
			zipOut.putNextEntry(zipEntry)
			Dim bytes(1023) As SByte
			Dim length As Integer
			length = fis.Read(bytes, 0, bytes.Length)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((length = fis.read(bytes)) >= 0)
			Do While length >= 0
				zipOut.write(bytes, 0, length)
					length = fis.Read(bytes, 0, bytes.Length)
			Loop
			zipOut.close()
			fis.Close()
			fos.Close()

			'now unzip to a directory that doesn't previously exist
			Dim unzipDir As New File(testFile.getParentFile(),"unzipTo")
			ArchiveUtils.unzipFileTo(zipFile.getAbsolutePath(),unzipDir.getAbsolutePath())
		End Sub
	End Class

End Namespace