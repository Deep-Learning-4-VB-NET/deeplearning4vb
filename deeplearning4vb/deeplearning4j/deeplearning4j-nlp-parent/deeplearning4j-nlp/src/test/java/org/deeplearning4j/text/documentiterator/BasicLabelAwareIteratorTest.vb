Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class BasicLabelAwareIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class BasicLabelAwareIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHasNextDocument1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasNextDocument1()
			Dim inputFile As File = Resources.asFile("big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(inputFile.getAbsolutePath())

			Dim iterator As BasicLabelAwareIterator = (New BasicLabelAwareIterator.Builder(iter)).setLabelTemplate("DOCZ_").build()

			Dim cnt As Integer = 0
			Do While iterator.hasNextDocument()
				iterator.nextDocument()
				cnt += 1
			Loop

			assertEquals(97162, cnt)

			Dim generator As LabelsSource = iterator.LabelsSource

			assertEquals(97162, generator.getLabels().Count)
			assertEquals("DOCZ_0", generator.getLabels()(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHasNextDocument2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasNextDocument2()

			Dim inputFile As File = Resources.asFile("big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(inputFile.getAbsolutePath())

			Dim iterator As BasicLabelAwareIterator = (New BasicLabelAwareIterator.Builder(iter)).setLabelTemplate("DOCZ_").build()

			Dim cnt As Integer = 0
			Do While iterator.hasNextDocument()
				iterator.nextDocument()
				cnt += 1
			Loop

			assertEquals(97162, cnt)

			iterator.reset()

			cnt = 0
			Do While iterator.hasNextDocument()
				iterator.nextDocument()
				cnt += 1
			Loop

			assertEquals(97162, cnt)

			Dim generator As LabelsSource = iterator.LabelsSource

			' this is important moment. Iterator after reset should not increase number of labels attained
			assertEquals(97162, generator.getLabels().Count)
			assertEquals("DOCZ_0", generator.getLabels()(0))
		End Sub
	End Class

End Namespace