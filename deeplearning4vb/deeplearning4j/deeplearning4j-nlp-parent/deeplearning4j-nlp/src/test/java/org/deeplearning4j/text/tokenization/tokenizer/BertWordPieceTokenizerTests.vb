Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BertWordPiecePreProcessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.BertWordPiecePreProcessor
Imports BertWordPieceTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.BertWordPieceTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resources = org.nd4j.common.resources.Resources
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

Namespace org.deeplearning4j.text.tokenization.tokenizer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Tag(TagNames.FILE_IO) @NativeTag public class BertWordPieceTokenizerTests extends org.deeplearning4j.BaseDL4JTest
	Public Class BertWordPieceTokenizerTests
		Inherits BaseDL4JTest

		Private pathToVocab As File = Resources.asFile("other/vocab.txt")
		Private c As Charset = StandardCharsets.UTF_8

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public BertWordPieceTokenizerTests() throws java.io.IOException
		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer1()
			Dim toTokenize As String = "I saw a girl with a telescope."
			Dim t As TokenizerFactory = New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)
			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))
			Dim position As Integer = 1
			Do While tokenizer2.hasMoreTokens()
				Dim tok1 As String = tokenizer.nextToken()
				Dim tok2 As String = tokenizer2.nextToken()
				log.info("Position: [" & position & "], token1: '" & tok1 & "', token 2: '" & tok2 & "'")
				position += 1
				assertEquals(tok1, tok2)

				Dim s2 As String = BertWordPiecePreProcessor.reconstructFromTokens(tokenizer.getTokens())
				assertEquals(toTokenize, s2)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer2()
			Dim t As TokenizerFactory = New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)

			Dim resource As New ClassPathResource("reuters/5250")
			Dim str As String = FileUtils.readFileToString(resource.File)
			Dim stringCount As Integer = t.create(str).countTokens()
			Dim stringCount2 As Integer = t.create(resource.InputStream).countTokens()
			assertTrue(Math.Abs(stringCount - stringCount2) < 2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer3()
			Dim toTokenize As String = "Donaudampfschifffahrtskapitänsmützeninnenfuttersaum"
			Dim t As TokenizerFactory = New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)
			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<String> expected = java.util.Arrays.asList("Donau", "##dam", "##pf", "##schiff", "##fahrt", "##skap", "##itä", "##ns", "##m", "##ützen", "##innen", "##fu", "##tter", "##sa", "##um");
			Dim expected As IList(Of String) = New List(Of String) From {"Donau", "##dam", "##pf", "##schiff", "##fahrt", "##skap", "##itä", "##ns", "##m", "##ützen", "##innen", "##fu", "##tter", "##sa", "##um"}
			assertEquals(expected, tokenizer.getTokens())
			assertEquals(expected, tokenizer2.getTokens())

			Dim s2 As String = BertWordPiecePreProcessor.reconstructFromTokens(tokenizer.getTokens())
			assertEquals(toTokenize, s2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer4()
			Dim toTokenize As String = "I saw a girl with a telescope."
			Dim t As TokenizerFactory = New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)
			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<String> expected = java.util.Arrays.asList("I", "saw", "a", "girl", "with", "a", "tele", "##scope", ".");
			Dim expected As IList(Of String) = New List(Of String) From {"I", "saw", "a", "girl", "with", "a", "tele", "##scope", "."}
			assertEquals(expected, tokenizer.getTokens())
			assertEquals(expected, tokenizer2.getTokens())

			Dim s2 As String = BertWordPiecePreProcessor.reconstructFromTokens(tokenizer.getTokens())
			assertEquals(toTokenize, s2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/24 - Disabled until dev branch merged - see issue #7657") public void testBertWordPieceTokenizer5() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer5()
			' Longest Token in Vocab is 22 chars long, so make sure splits on the edge are properly handled
			Dim toTokenize As String = "Donaudampfschifffahrts Kapitänsmützeninnenfuttersaum"
			Dim t As TokenizerFactory = New BertWordPieceTokenizerFactory(pathToVocab, False, False, c)
			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<String> expected = java.util.Arrays.asList("Donau", "##dam", "##pf", "##schiff", "##fahrt", "##s", "Kapitän", "##sm", "##ützen", "##innen", "##fu", "##tter", "##sa", "##um");
			Dim expected As IList(Of String) = New List(Of String) From {"Donau", "##dam", "##pf", "##schiff", "##fahrt", "##s", "Kapitän", "##sm", "##ützen", "##innen", "##fu", "##tter", "##sa", "##um"}
			assertEquals(expected, tokenizer.getTokens())
			assertEquals(expected, tokenizer2.getTokens())

			Dim s2 As String = BertWordPiecePreProcessor.reconstructFromTokens(tokenizer.getTokens())
			assertEquals(toTokenize, s2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer6() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer6()
			Dim toTokenize As String = "I sAw A gIrL wItH a tElEsCoPe."
			Dim t As New BertWordPieceTokenizerFactory(pathToVocab, True, True, c)

			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<String> expected = java.util.Arrays.asList("i", "saw", "a", "girl", "with", "a", "tele", "##scope", ".");
			Dim expected As IList(Of String) = New List(Of String) From {"i", "saw", "a", "girl", "with", "a", "tele", "##scope", "."}
			assertEquals(expected, tokenizer.getTokens())
			assertEquals(expected, tokenizer2.getTokens())

			Dim s2 As String = BertWordPiecePreProcessor.reconstructFromTokens(tokenizer.getTokens())
			assertEquals(toTokenize.ToLower(), s2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer7() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer7()
			Dim toTokenize As String = "I saw a girl with a telescope."
			Dim t As New BertWordPieceTokenizerFactory(pathToVocab, True, True, c)

			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<String> expected = java.util.Arrays.asList("i", "saw", "a", "girl", "with", "a", "tele", "##scope", ".");
			Dim expected As IList(Of String) = New List(Of String) From {"i", "saw", "a", "girl", "with", "a", "tele", "##scope", "."}
			assertEquals(expected, tokenizer.getTokens())
			assertEquals(expected, tokenizer2.getTokens())

			Dim s2 As String = BertWordPiecePreProcessor.reconstructFromTokens(tokenizer.getTokens())
			assertEquals(toTokenize.ToLower(), s2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer8() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer8()
			'Insert some invalid characters...
			Dim toTokenize As String = "I saw a girl " & ChrW(8) & " with a tele" & ChrW(7) & "scope."
			Dim t As New BertWordPieceTokenizerFactory(pathToVocab, True, True, c)

			Dim tokenizer As Tokenizer = t.create(toTokenize)
			Dim tokenizer2 As Tokenizer = t.create(New MemoryStream(toTokenize.GetBytes()))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<String> expected = java.util.Arrays.asList("i", "saw", "a", "girl", "with", "a", "tele", "##scope", ".");
			Dim expected As IList(Of String) = New List(Of String) From {"i", "saw", "a", "girl", "with", "a", "tele", "##scope", "."}
			assertEquals(expected, tokenizer.getTokens())
			assertEquals(expected, tokenizer2.getTokens())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBertWordPieceTokenizer9() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer9()
			'Insert some invalid characters - without the preprocessing. This should fail

			For Each toTokenize As String In New String(){ "I saw a girl with a tele" & ChrW(7) & "scope.", "I saw a girl " & ChrW(8) & " with a tele" & ChrW(7) & "scope.", ChrW(&H23A0).ToString() & "I saw a girl with a telescope.", "I saw a girl with a telescope." & ChrW(&H23A0).ToString() }
				Dim t As New BertWordPieceTokenizerFactory(pathToVocab, True, True, c)
				t.setPreTokenizePreProcessor(Nothing)

				Try
					t.create(toTokenize)
					fail("Expected exception: " & toTokenize)
				Catch e As System.InvalidOperationException
					Dim m As String = e.Message
					assertNotNull(m)
					m = m.ToLower()
					assertTrue(m.Contains("invalid") AndAlso m.Contains("token") AndAlso m.Contains("preprocessor"), m)
				End Try

				Try
					t.create(New MemoryStream(toTokenize.GetBytes()))
					fail("Expected exception: " & toTokenize)
				Catch e As System.InvalidOperationException
					Dim m As String = e.Message
					assertNotNull(m)
					m = m.ToLower()
					assertTrue(m.Contains("invalid") AndAlso m.Contains("token") AndAlso m.Contains("preprocessor"), m)
				End Try
			Next toTokenize
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) public void testBertWordPieceTokenizer10() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertWordPieceTokenizer10()
			Dim f As File = Resources.asFile("deeplearning4j-nlp/bert/uncased_L-12_H-768_A-12/vocab.txt")
			Dim t As New BertWordPieceTokenizerFactory(f, True, True, StandardCharsets.UTF_8)

			Dim s As String = "This is a sentence with Multiple Cases For Words. It should be coverted to Lower Case here."

			Dim tokenizer As Tokenizer = t.create(s)
			Dim list As IList(Of String) = tokenizer.getTokens()
			Console.WriteLine(list)

			Dim s2 As String = BertWordPiecePreProcessor.reconstructFromTokens(list)
			Dim exp As String = s.ToLower()
			assertEquals(exp, s2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTokenizerHandlesLargeContiguousWhitespace() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTokenizerHandlesLargeContiguousWhitespace()
			Dim sb As New StringBuilder()
			sb.Append("apple.")
			For i As Integer = 0 To 9999
				sb.Append(" ")
			Next i
			sb.Append(".pen. .pineapple")

			Dim f As File = Resources.asFile("deeplearning4j-nlp/bert/uncased_L-12_H-768_A-12/vocab.txt")
			Dim t As New BertWordPieceTokenizerFactory(f, True, True, StandardCharsets.UTF_8)

			Dim tokenizer As Tokenizer = t.create(sb.ToString())
			Dim list As IList(Of String) = tokenizer.getTokens()
			Console.WriteLine(list)

			assertEquals(8, list.Count)
		End Sub
	End Class

End Namespace