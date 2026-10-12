Imports System.IO
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Resources = org.nd4j.common.resources.Resources
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

Namespace org.deeplearning4j.text.sentenceiterator



	Public Class BasicLineIteratorTest
		Inherits BaseDL4JTest



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled(".opentest4j.AssertionFailedError: expected: <97162> but was: <16889> Line 66") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testHasMoreLinesFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasMoreLinesFile()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iterator As New BasicLineIterator(file)

			Dim cnt As Integer = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(97162, cnt)

			iterator.reset()

			cnt = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(97162, cnt)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHasMoreLinesStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasMoreLinesStream()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iterator As New BasicLineIterator(New FileStream(file, FileMode.Open, FileAccess.Read))

			Dim cnt As Integer = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(97162, cnt)

			iterator.reset()

			cnt = 0
			Do While iterator.hasNext()
				Dim line As String = iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(97162, cnt)
		End Sub
	End Class

End Namespace