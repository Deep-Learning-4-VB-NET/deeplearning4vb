Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
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


	Public Class AggregatingSentenceIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) @Disabled("Needs verification, could be permissions issues: g.opentest4j.AssertionFailedError: expected: <388648> but was: <262782> at line 60") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testHasNext() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHasNext()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iterator As New BasicLineIterator(file)
			Dim iterator2 As New BasicLineIterator(file)

			Dim aggr As AggregatingSentenceIterator = (New AggregatingSentenceIterator.Builder()).addSentenceIterator(iterator).addSentenceIterator(iterator2).build()

			Dim cnt As Integer = 0
			Do While aggr.hasNext()
				Dim line As String = aggr.nextSentence()
				cnt += 1
			Loop

			assertEquals((97162 * 2), cnt)

			aggr.reset()

			Do While aggr.hasNext()
				Dim line As String = aggr.nextSentence()
				cnt += 1
			Loop

			assertEquals((97162 * 4), cnt)
		End Sub
	End Class

End Namespace