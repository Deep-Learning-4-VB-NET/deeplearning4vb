Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports Resources = org.nd4j.common.resources.Resources
Imports StrumpfResolver = org.nd4j.common.resources.strumpf.StrumpfResolver
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

Namespace org.nd4j.common.resources


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled public class TestStrumpf
	Public Class TestStrumpf
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testResolvingActual() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testResolvingActual()
			Dim f As File = Resources.asFile("data/irisSvmLight.txt")
			assertTrue(f.exists())

			'System.out.println(f.getAbsolutePath());
			Dim count As Integer = 0
			Using r As java.io.Reader = New StreamReader(f)
				Dim iter As LineIterator = IOUtils.lineIterator(r)
				Do While iter.hasNext()
					Dim line As String = iter.next()
					'System.out.println("LINE " + i + ": " + line);
					count += 1
				Loop
			End Using

			assertEquals(12, count) 'Iris normally has 150 examples; this is subset with 12
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testResolveLocal(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testResolveLocal(ByVal testDir As Path)

			Dim dir As File = testDir.toFile()

			Dim content As String = "test file content"
			Dim path As String = "myDir/myTestFile.txt"
			Dim testFile As New File(dir, path)
			testFile.getParentFile().mkdir()
			FileUtils.writeStringToFile(testFile, content, StandardCharsets.UTF_8)

			System.setProperty(ND4JSystemProperties.RESOURCES_LOCAL_DIRS, dir.getAbsolutePath())

			Try
				Dim r As New StrumpfResolver()
				assertTrue(r.exists(path))
				Dim f As File = r.asFile(path)
				assertTrue(f.exists())
				assertEquals(testFile.getAbsolutePath(), f.getAbsolutePath())
				Dim s As String = FileUtils.readFileToString(f, StandardCharsets.UTF_8)
				assertEquals(content, s)
			Finally
				System.setProperty(ND4JSystemProperties.RESOURCES_LOCAL_DIRS, "")
			End Try
		End Sub

	End Class

End Namespace