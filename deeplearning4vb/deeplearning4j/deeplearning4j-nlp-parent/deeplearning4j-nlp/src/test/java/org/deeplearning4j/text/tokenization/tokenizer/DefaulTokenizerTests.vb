Imports System
Imports System.IO
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class DefaulTokenizerTests extends org.deeplearning4j.BaseDL4JTest
	Public Class DefaulTokenizerTests
		Inherits BaseDL4JTest

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DefaulTokenizerTests))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDefaultTokenizer1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDefaultTokenizer1()
			Dim toTokenize As String = "Mary had a little lamb."
			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))
			Dim position As Integer = 1
			Do While tokenizer2.hasMoreTokens()
				Dim tok1 As String = tokenizer.nextToken()
				Dim tok2 As String = tokenizer2.nextToken()
				log.info("Position: [" & position & "], token1: '" & tok1 & "', token 2: '" & tok2 & "'")
				position += 1
				assertEquals(tok1, tok2)
			Loop


			Dim resource As New ClassPathResource("reuters/5250")
			Dim str As String = FileUtils.readFileToString(resource.File)
			Dim stringCount As Integer = t.create(str).countTokens()
			Dim stringCount2 As Integer = t.create(resource.InputStream).countTokens()
			assertTrue(Math.Abs(stringCount - stringCount2) < 2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDefaultTokenizer2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDefaultTokenizer2()
			Dim toTokenize As String = "Mary had a little lamb."
			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))
			tokenizer2.countTokens()
			Do While tokenizer.hasMoreTokens()
				Dim tok1 As String = tokenizer.nextToken()
				Dim tok2 As String = tokenizer2.nextToken()
				assertEquals(tok1, tok2)
			Loop


			Console.WriteLine("-----------------------------------------------")

			Dim resource As New ClassPathResource("reuters/5250")
			Dim str As String = FileUtils.readFileToString(resource.File)
			Dim stringCount As Integer = t.create(str).countTokens()
			Dim stringCount2 As Integer = t.create(resource.InputStream).countTokens()

			log.info("String tok: [" & stringCount & "], Stream tok: [" & stringCount2 & "], Difference: " & Math.Abs(stringCount - stringCount2))

			assertTrue(Math.Abs(stringCount - stringCount2) < 2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDefaultTokenizer3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDefaultTokenizer3()
			Dim toTokenize As String = "Mary had a little lamb."
			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))
			Dim position As Integer = 1
			Do While tokenizer2.hasMoreTokens()
				Dim tok1 As String = tokenizer.nextToken()
				Dim tok2 As String = tokenizer2.nextToken()
				log.info("Position: [" & position & "], token1: '" & tok1 & "', token 2: '" & tok2 & "'")
				position += 1
				assertEquals(tok1, tok2)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDefaultStreamTokenizer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDefaultStreamTokenizer()
			Dim toTokenize As String = "Mary had a little lamb."
			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))

			assertEquals(5, tokenizer2.countTokens())

			Dim cnt As Integer = 0
			Do While tokenizer2.hasMoreTokens()
				Dim tok1 As String = tokenizer2.nextToken()
				log.info(tok1)
				cnt += 1
			Loop

			assertEquals(5, cnt)
		End Sub


	End Class

End Namespace