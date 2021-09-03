Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
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

Namespace org.deeplearning4j.text.documentiterator




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class FileDocumentIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class FileDocumentIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

		''' <summary>
		''' Checks actual number of documents retrieved by DocumentIterator </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNextDocument() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNextDocument()
			Dim reuters5250 As New ClassPathResource("/reuters/5250")
			Dim f As File = reuters5250.File

			Dim iter As DocumentIterator = New FileDocumentIterator(f.getAbsolutePath())

			log.info(f.getAbsolutePath())

			Dim cnt As Integer = 0
			Do While iter.hasNext()
				Dim stream As Stream = iter.nextDocument()
				stream.Close()
				cnt += 1
			Loop

			assertEquals(24, cnt)
		End Sub


		''' <summary>
		''' Checks actual number of documents retrieved by DocumentIterator after being RESET </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDocumentReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDocumentReset()
			Dim reuters5250 As New ClassPathResource("/reuters/5250")
			Dim f As File = reuters5250.File

			Dim iter As DocumentIterator = New FileDocumentIterator(f.getAbsolutePath())

			Dim cnt As Integer = 0
			Do While iter.hasNext()
				Dim stream As Stream = iter.nextDocument()
				stream.Close()
				cnt += 1
			Loop

			iter.reset()

			Do While iter.hasNext()
				Dim stream As Stream = iter.nextDocument()
				stream.Close()
				cnt += 1
			Loop

			assertEquals(48, cnt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(5000) public void testEmptyDocument(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEmptyDocument(ByVal testDir As Path)
			Dim f As File = Files.createTempFile(testDir,"newfile","bin").toFile()
			assertTrue(f.exists())
			assertEquals(0, f.length())

			Try
				Dim iter As DocumentIterator = New FileDocumentIterator(f.getAbsolutePath())
			Catch t As Exception
				Dim msg As String = t.getMessage()
				assertTrue(msg.Contains("empty"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(5000) public void testEmptyDocument2(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEmptyDocument2(ByVal testDir As Path)
			Dim dir As File = testDir.toFile()
			Dim f1 As New File(dir, "1.txt")
			FileUtils.writeStringToFile(f1, "line 1" & vbLf & "line2", StandardCharsets.UTF_8)
			Dim f2 As New File(dir, "2.txt")
			f2.createNewFile()
			Dim f3 As New File(dir, "3.txt")
			FileUtils.writeStringToFile(f3, "line 3" & vbLf & "line4", StandardCharsets.UTF_8)

			Dim iter As DocumentIterator = New FileDocumentIterator(dir)
			Dim count As Integer = 0
			Dim lines As ISet(Of String) = New HashSet(Of String)()
			Do While iter.hasNext()
				Dim [next] As String = IOUtils.readLines(iter.nextDocument(), StandardCharsets.UTF_8).get(0)
				lines.Add([next])
			Loop

			assertEquals(4, lines.Count)
		End Sub

	End Class

End Namespace