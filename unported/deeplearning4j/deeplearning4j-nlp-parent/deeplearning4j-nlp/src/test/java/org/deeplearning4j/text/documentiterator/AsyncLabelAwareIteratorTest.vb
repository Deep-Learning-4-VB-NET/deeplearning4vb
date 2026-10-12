Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class AsyncLabelAwareIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class AsyncLabelAwareIteratorTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(30000) public void nextDocument() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub nextDocument()
			Dim sentence As SentenceIterator = New BasicLineIterator(Resources.asFile("big/raw_sentences.txt"))
			Dim backed As BasicLabelAwareIterator = (New BasicLabelAwareIterator.Builder(sentence)).build()

			Dim cnt As Integer = 0
			Do While backed.hasNextDocument()
				backed.nextDocument()
				cnt += 1
			Loop
			assertEquals(97162, cnt)

			backed.reset()

			Dim iterator As New AsyncLabelAwareIterator(backed, 64)
			cnt = 0
			Do While iterator.MoveNext()
				iterator.Current
				cnt += 1

				If cnt = 10 Then
					iterator.reset()
				End If
			Loop
			assertEquals(97172, cnt)
		End Sub

	End Class

End Namespace