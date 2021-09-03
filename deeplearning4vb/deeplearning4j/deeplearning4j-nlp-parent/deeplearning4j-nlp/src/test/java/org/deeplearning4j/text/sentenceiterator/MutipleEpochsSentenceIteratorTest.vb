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

	Public Class MutipleEpochsSentenceIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) @Disabled("Downloads need verification ile hash does not match expected hash: https://dl4jtest.blob.core.windows.net/resources/big/raw_sentences.txt.gzx.v1") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void hasNext() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub hasNext()
			Dim iterator As SentenceIterator = New MutipleEpochsSentenceIterator(New BasicLineIterator(Resources.asFile("big/raw_sentences.txt")), 100)

			Dim cnt As Integer = 0
			Do While iterator.hasNext()
				iterator.nextSentence()
				cnt += 1
			Loop

			assertEquals(9716200, cnt)
		End Sub

	End Class

End Namespace