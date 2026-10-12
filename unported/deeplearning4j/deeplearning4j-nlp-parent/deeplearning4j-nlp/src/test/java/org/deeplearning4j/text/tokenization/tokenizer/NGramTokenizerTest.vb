Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports NGramTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.NGramTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.text.tokenization.tokenizer



	''' <summary>
	''' @author sonali
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class NGramTokenizerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class NGramTokenizerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNGramTokenizer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNGramTokenizer()
			Dim toTokenize As String = "Mary had a little lamb."
			Dim factory As TokenizerFactory = New NGramTokenizerFactory(New DefaultTokenizerFactory(), 1, 2)
			Dim tokenizer As Tokenizer = factory.create(toTokenize)
			Dim tokenizer2 As Tokenizer = factory.create(toTokenize)
			Do While tokenizer.hasMoreTokens()
				assertEquals(tokenizer.nextToken(), tokenizer2.nextToken())
			Loop

			Dim stringCount As Integer = factory.create(toTokenize).countTokens()
			Dim tokens As IList(Of String) = factory.create(toTokenize).getTokens()
			assertEquals(9, stringCount)

			assertTrue(tokens.Contains("Mary"))
			assertTrue(tokens.Contains("had"))
			assertTrue(tokens.Contains("a"))
			assertTrue(tokens.Contains("little"))
			assertTrue(tokens.Contains("lamb."))
			assertTrue(tokens.Contains("Mary had"))
			assertTrue(tokens.Contains("had a"))
			assertTrue(tokens.Contains("a little"))
			assertTrue(tokens.Contains("little lamb."))

			factory = New NGramTokenizerFactory(New DefaultTokenizerFactory(), 2, 2)
			tokens = factory.create(toTokenize).getTokens()
			assertEquals(4, tokens.Count)

			assertTrue(tokens.Contains("Mary had"))
			assertTrue(tokens.Contains("had a"))
			assertTrue(tokens.Contains("a little"))
			assertTrue(tokens.Contains("little lamb."))
		End Sub
	End Class

End Namespace