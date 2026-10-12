Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports InMemoryLookupCache = org.deeplearning4j.models.word2vec.wordstore.inmemory.InMemoryLookupCache
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
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

Namespace org.deeplearning4j.models.word2vec.wordstore

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class VocabularyHolderTest extends org.deeplearning4j.BaseDL4JTest
	Public Class VocabularyHolderTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransferBackToVocabCache() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTransferBackToVocabCache()
			Dim holder As New VocabularyHolder()
			holder.addWord("test")
			holder.addWord("tests")
			holder.addWord("testz")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("testz")

			Dim cache As New InMemoryLookupCache(False)
			holder.updateHuffmanCodes()
			holder.transferBackToVocabCache(cache)

			' checking word frequency transfer
			assertEquals(3, cache.numWords())
			assertEquals(1, cache.wordFrequency("test"))
			assertEquals(2, cache.wordFrequency("testz"))
			assertEquals(3, cache.wordFrequency("tests"))


			' checking Huffman tree transfer
			assertEquals("tests", cache.wordAtIndex(0))
			assertEquals("testz", cache.wordAtIndex(1))
			assertEquals("test", cache.wordAtIndex(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConstructor() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConstructor()
			Dim cache As New InMemoryLookupCache(True)
			Dim holder As New VocabularyHolder(cache, False)

			' no more UNK token here
			assertEquals(0, holder.numWords())
		End Sub

		''' <summary>
		''' In this test we make sure SPECIAL words are not affected by truncation in extending vocab </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSpecial1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSpecial1()
			Dim holder As VocabularyHolder = (New VocabularyHolder.Builder()).minWordFrequency(1).build()

			holder.addWord("test")
			holder.addWord("tests")

			holder.truncateVocabulary()

			assertEquals(2, holder.numWords())

			Dim cache As VocabCache = New InMemoryLookupCache()
			holder.transferBackToVocabCache(cache)

			Dim holder2 As VocabularyHolder = (New VocabularyHolder.Builder()).externalCache(cache).minWordFrequency(10).build()

			holder2.addWord("testz")
			assertEquals(3, holder2.numWords())

			holder2.truncateVocabulary()
			assertEquals(2, holder2.numWords())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScavenger1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testScavenger1()
			Dim holder As VocabularyHolder = (New VocabularyHolder.Builder()).minWordFrequency(5).hugeModelExpected(True).scavengerActivationThreshold(1000000).scavengerRetentionDelay(3).build()

			holder.addWord("test")
			holder.addWord("tests")

			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")

			holder.activateScavenger()
			assertEquals(2, holder.numWords())
			holder.activateScavenger()
			assertEquals(2, holder.numWords())

			' after third activation, word "test" should be removed
			holder.activateScavenger()
			assertEquals(1, holder.numWords())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScavenger2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testScavenger2()
			Dim holder As VocabularyHolder = (New VocabularyHolder.Builder()).minWordFrequency(5).hugeModelExpected(True).scavengerActivationThreshold(1000000).scavengerRetentionDelay(3).build()

			holder.addWord("test")
			holder.incrementWordCounter("test")

			holder.addWord("tests")

			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")

			holder.activateScavenger()
			assertEquals(2, holder.numWords())
			holder.activateScavenger()
			assertEquals(2, holder.numWords())

			' after third activation, word "test" should be removed
			holder.activateScavenger()
			assertEquals(1, holder.numWords())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScavenger3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testScavenger3()
			Dim holder As VocabularyHolder = (New VocabularyHolder.Builder()).minWordFrequency(5).hugeModelExpected(True).scavengerActivationThreshold(1000000).scavengerRetentionDelay(3).build()

			holder.addWord("test")

			holder.activateScavenger()
			assertEquals(1, holder.numWords())

			holder.incrementWordCounter("test")
			holder.addWord("tests")

			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")


			holder.activateScavenger()
			assertEquals(2, holder.numWords())

			' after third activation, word "test" should NOT be removed, since at point 0 we have freq == 1, and 2 in the following tests
			holder.activateScavenger()
			assertEquals(2, holder.numWords())

			' here we should have all retention points shifted, and word "test" should be removed
			holder.activateScavenger()
			assertEquals(1, holder.numWords())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScavenger4() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testScavenger4()
			Dim holder As VocabularyHolder = (New VocabularyHolder.Builder()).minWordFrequency(5).hugeModelExpected(True).scavengerActivationThreshold(1000000).scavengerRetentionDelay(3).build()

			holder.addWord("test")

			holder.activateScavenger()
			assertEquals(1, holder.numWords())

			holder.incrementWordCounter("test")

			holder.addWord("tests")

			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")
			holder.incrementWordCounter("tests")


			holder.activateScavenger()
			assertEquals(2, holder.numWords())

			' after third activation, word "test" should NOT be removed, since at point 0 we have freq == 1, and 2 in the following tests
			holder.activateScavenger()
			assertEquals(2, holder.numWords())

			holder.incrementWordCounter("test")

			' here we should have all retention points shifted, and word "test" should NOT be removed, since now it's above the scavenger threshold
			holder.activateScavenger()
			assertEquals(2, holder.numWords())
		End Sub
	End Class

End Namespace