Imports System.IO
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.hamcrest.MatcherAssert.assertThat
import static org.hamcrest.core.AnyOf.anyOf
import static org.hamcrest.core.IsEqual.equalTo
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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
Namespace org.datavec.api.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Class Path Resource Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class ClassPathResourceTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class ClassPathResourceTest
		Inherits BaseND4JTest

		' File sizes are reported slightly different on Linux vs. Windows
		Private isWindows As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub setUp()
			Dim osname As String = System.getProperty("os.name")
			If osname IsNot Nothing AndAlso osname.ToLower().Contains("win") Then
				isWindows = True
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get File 1") void testGetFile1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetFile1()
			Dim intFile As File = (New ClassPathResource("datavec-api/iris.dat")).File
			assertTrue(intFile.exists())
			If isWindows Then
				assertThat(intFile.length(), anyOf(equalTo(2700L), equalTo(2850L)))
			Else
				assertEquals(2700, intFile.length())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get File Slash 1") void testGetFileSlash1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetFileSlash1()
			Dim intFile As File = (New ClassPathResource("datavec-api/iris.dat")).File
			assertTrue(intFile.exists())
			If isWindows Then
				assertThat(intFile.length(), anyOf(equalTo(2700L), equalTo(2850L)))
			Else
				assertEquals(2700, intFile.length())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Get File With Space 1") void testGetFileWithSpace1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testGetFileWithSpace1()
			Dim intFile As File = (New ClassPathResource("datavec-api/csvsequence test.txt")).File
			assertTrue(intFile.exists())
			If isWindows Then
				assertThat(intFile.length(), anyOf(equalTo(60L), equalTo(64L)))
			Else
				assertEquals(60, intFile.length())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Stream") void testInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInputStream()
			Dim resource As New ClassPathResource("datavec-api/csvsequence_1.txt")
			Dim intFile As File = resource.File
			If isWindows Then
				assertThat(intFile.length(), anyOf(equalTo(60L), equalTo(64L)))
			Else
				assertEquals(60, intFile.length())
			End If
			Dim stream As Stream = resource.InputStream
			Dim reader As New StreamReader(stream)
			Dim line As String = ""
			Dim cnt As Integer = 0
			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				cnt += 1
					line = reader.ReadLine()
			Loop
			assertEquals(5, cnt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Stream Slash") void testInputStreamSlash() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInputStreamSlash()
			Dim resource As New ClassPathResource("datavec-api/csvsequence_1.txt")
			Dim intFile As File = resource.File
			If isWindows Then
				assertThat(intFile.length(), anyOf(equalTo(60L), equalTo(64L)))
			Else
				assertEquals(60, intFile.length())
			End If
			Dim stream As Stream = resource.InputStream
			Dim reader As New StreamReader(stream)
			Dim line As String = ""
			Dim cnt As Integer = 0
			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				cnt += 1
					line = reader.ReadLine()
			Loop
			assertEquals(5, cnt)
		End Sub
	End Class

End Namespace