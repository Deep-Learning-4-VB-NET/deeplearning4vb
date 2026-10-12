Imports System
Imports System.Collections.Generic
Imports JsonObject = com.google.gson.JsonObject
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ExtVocabWord = org.deeplearning4j.models.sequencevectors.serialization.ExtVocabWord
Imports Huffman = org.deeplearning4j.models.word2vec.Huffman
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
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

Namespace org.deeplearning4j.models.word2vec.wordstore.inmemory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class AbstractCacheTest extends org.deeplearning4j.BaseDL4JTest
	Public Class AbstractCacheTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNumWords() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNumWords()
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			cache.addToken(New VocabWord(1.0, "word"))
			cache.addToken(New VocabWord(1.0, "test"))

			assertEquals(2, cache.numWords())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHuffman() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHuffman()
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			cache.addToken(New VocabWord(1.0, "word"))
			cache.addToken(New VocabWord(2.0, "test"))
			cache.addToken(New VocabWord(3.0, "tester"))

			assertEquals(3, cache.numWords())

			Dim huffman As New Huffman(cache.tokens())
			huffman.build()
			huffman.applyIndexes(cache)

			assertEquals("tester", cache.wordAtIndex(0))
			assertEquals("test", cache.wordAtIndex(1))
			assertEquals("word", cache.wordAtIndex(2))

			Dim word As VocabWord = cache.tokenFor("tester")
			assertEquals(0, word.Index)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWordsOccurencies() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWordsOccurencies()
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			cache.addToken(New VocabWord(1.0, "word"))
			cache.addToken(New VocabWord(2.0, "test"))
			cache.addToken(New VocabWord(3.0, "tester"))

			assertEquals(3, cache.numWords())
			assertEquals(6, cache.totalWordOccurrences())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRemoval() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRemoval()
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			cache.addToken(New VocabWord(1.0, "word"))
			cache.addToken(New VocabWord(2.0, "test"))
			cache.addToken(New VocabWord(3.0, "tester"))

			assertEquals(3, cache.numWords())
			assertEquals(6, cache.totalWordOccurrences())

			cache.removeElement("tester")
			assertEquals(2, cache.numWords())
			assertEquals(3, cache.totalWordOccurrences())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabels() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLabels()
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			cache.addToken(New VocabWord(1.0, "word"))
			cache.addToken(New VocabWord(2.0, "test"))
			cache.addToken(New VocabWord(3.0, "tester"))

			Dim collection As ICollection(Of String) = cache.words()
			assertEquals(3, collection.Count)

			assertTrue(collection.Contains("word"))
			assertTrue(collection.Contains("test"))
			assertTrue(collection.Contains("tester"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerialization()
		Public Overridable Sub testSerialization()
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim words As val = New VocabWord(2){}
			words(0) = New VocabWord(1.0, "word")
			words(1) = New VocabWord(2.0, "test")
			words(2) = New VocabWord(3.0, "tester")

			For i As Integer = 0 To words.length - 1
				cache.addToken(words(i))
				cache.addWordToIndex(i, words(i).getLabel())
			Next i

			Dim json As String = Nothing
			Dim unserialized As AbstractCache(Of VocabWord) = Nothing
			Try
				json = cache.toJson()
				log.info("{}", json.ToString())

				unserialized = AbstractCache.fromJson(json)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
			assertEquals(cache.totalWordOccurrences(),unserialized.totalWordOccurrences())
			assertEquals(cache.totalNumberOfDocs(), unserialized.totalNumberOfDocs())

			For i As Integer = 0 To words.length - 1
				Dim cached As val = cache.wordAtIndex(i)
				Dim restored As val = unserialized.wordAtIndex(i)
				assertNotNull(cached)
				assertEquals(cached, restored)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUserClassSerialization()
		Public Overridable Sub testUserClassSerialization()
			Dim cache As AbstractCache(Of ExtVocabWord) = (New AbstractCache.Builder(Of ExtVocabWord)()).build()

			Dim words(2) As ExtVocabWord
			words(0) = New ExtVocabWord("some", 1100, 1.0, "word")
			words(1) = New ExtVocabWord("none", 23214, 2.0, "test")
			words(2) = New ExtVocabWord("wwew", 13223, 3.0, "tester")

			For i As Integer = 0 To 2
				cache.addToken(words(i))
				cache.addWordToIndex(i, words(i).Label)
			Next i

			Dim json As String = Nothing
			Dim unserialized As AbstractCache(Of VocabWord) = Nothing
			Try
				json = cache.toJson()
				unserialized = AbstractCache.fromJson(json)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
			assertEquals(cache.totalWordOccurrences(),unserialized.totalWordOccurrences())
			assertEquals(cache.totalNumberOfDocs(), unserialized.totalNumberOfDocs())
			For i As Integer = 0 To 2
				Dim t As val = cache.wordAtIndex(i)
				assertNotNull(t)
				assertTrue(unserialized.containsWord(t))
				assertEquals(cache.wordAtIndex(i), unserialized.wordAtIndex(i))
			Next i
		End Sub

	End Class

End Namespace