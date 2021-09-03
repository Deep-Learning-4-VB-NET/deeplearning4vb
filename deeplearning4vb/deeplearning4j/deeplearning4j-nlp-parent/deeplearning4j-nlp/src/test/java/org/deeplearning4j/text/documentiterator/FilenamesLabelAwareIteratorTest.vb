Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.NEEDS_VERIFY) @Disabled("Permissions issues on CI") public class FilenamesLabelAwareIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class FilenamesLabelAwareIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNextDocument(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNextDocument(ByVal testDir As Path)
			Dim tempDir As val = testDir.toFile()
			Resources.copyDirectory("/big/", tempDir)

			Dim iterator As FilenamesLabelAwareIterator = (New FilenamesLabelAwareIterator.Builder()).addSourceFolder(tempDir).useAbsolutePathAsLabel(False).build()

			Dim labels As IList(Of String) = New List(Of String)()

			Dim doc1 As LabelledDocument = iterator.nextDocument()
			labels.Add(doc1.Label)

			Dim doc2 As LabelledDocument = iterator.nextDocument()
			labels.Add(doc2.Label)

			Dim doc3 As LabelledDocument = iterator.nextDocument()
			labels.Add(doc3.Label)

			Dim doc4 As LabelledDocument = iterator.nextDocument()
			labels.Add(doc4.Label)

			Dim doc5 As LabelledDocument = iterator.nextDocument()
			labels.Add(doc5.Label)

			Dim doc6 As LabelledDocument = iterator.nextDocument()
			labels.Add(doc6.Label)

			assertFalse(iterator.hasNextDocument())

			Console.WriteLine("Labels: " & labels)

			assertTrue(labels.Contains("coc.txt"))
			assertTrue(labels.Contains("occurrences.txt"))
			assertTrue(labels.Contains("raw_sentences.txt"))
			assertTrue(labels.Contains("tokens.txt"))
			assertTrue(labels.Contains("raw_sentences_2.txt"))
			assertTrue(labels.Contains("rnj.txt"))

		End Sub
	End Class

End Namespace