Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class FileLabelAwareIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class FileLabelAwareIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExtractLabelFromPath1(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testExtractLabelFromPath1(ByVal testDir As Path)
			Dim dir As val = testDir.resolve("new-folder").toFile()
			dir.mkdirs()
			Dim resource As val = New ClassPathResource("/labeled/")
			resource.copyDirectory(dir)

			Dim iterator As val = (New FileLabelAwareIterator.Builder()).addSourceFolder(dir).build()

			Dim cnt As Integer = 0
			Do While iterator.hasNextDocument()
				Dim document As val = iterator.nextDocument()
				assertNotEquals(Nothing, document)
				assertNotEquals(Nothing, document.getContent())
				assertNotEquals(Nothing, document.getLabel())
				cnt += 1
			Loop

			assertEquals(3, cnt)


			assertEquals(3, iterator.getLabelsSource().getNumberOfLabelsUsed())

			assertTrue(iterator.getLabelsSource().getLabels().contains("positive"))
			assertTrue(iterator.getLabelsSource().getLabels().contains("negative"))
			assertTrue(iterator.getLabelsSource().getLabels().contains("neutral"))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExtractLabelFromPath2(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testExtractLabelFromPath2(ByVal testDir As Path)
			testDir = testDir.resolve("new-folder")
			testDir.toFile().mkdirs()
			Dim dir0 As val = New File(testDir.toFile(),"dir-0")
			Dim dir1 As val = New File(testDir.toFile(),"dir-1")
			dir0.mkdirs()
			dir1.mkdirs()
			Dim resource As val = New ClassPathResource("/labeled/")
			Dim resource2 As val = New ClassPathResource("/rootdir/")
			resource.copyDirectory(dir0)
			resource2.copyDirectory(dir1)

			Dim iterator As FileLabelAwareIterator = (New FileLabelAwareIterator.Builder()).addSourceFolder(dir0).addSourceFolder(dir1).build()

			Dim cnt As Integer = 0
			Do While iterator.hasNextDocument()
				Dim document As LabelledDocument = iterator.nextDocument()
				assertNotEquals(Nothing, document)
				assertNotEquals(Nothing, document.getContent())
				assertNotEquals(Nothing, document.Label)
				cnt += 1
			Loop

			assertEquals(5, cnt)


			assertEquals(5, iterator.LabelsSource.NumberOfLabelsUsed)

			assertTrue(iterator.LabelsSource.getLabels().Contains("positive"))
			assertTrue(iterator.LabelsSource.getLabels().Contains("negative"))
			assertTrue(iterator.LabelsSource.getLabels().Contains("neutral"))
			assertTrue(iterator.LabelsSource.getLabels().Contains("label1"))
			assertTrue(iterator.LabelsSource.getLabels().Contains("label2"))
		End Sub
	End Class

End Namespace