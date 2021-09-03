Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.deeplearning4j.models.sequencevectors.iterators
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports FileLabelAwareIterator = org.deeplearning4j.text.documentiterator.FileLabelAwareIterator
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports CommonPreprocessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
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

Namespace org.deeplearning4j.models.embeddings.inmemory



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class InMemoryLookupTableTest extends org.deeplearning4j.BaseDL4JTest
	Public Class InMemoryLookupTableTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) public void testConsumeOnEqualVocabs() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConsumeOnEqualVocabs()
			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim cacheSource As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()


			Dim resource As File = Resources.asFile("big/raw_sentences.txt")

			Dim underlyingIterator As New BasicLineIterator(resource)


			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(underlyingIterator).tokenizerFactory(t).build()

			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim vocabConstructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 1).setTargetVocabCache(cacheSource).build()

			vocabConstructor.buildJointVocabulary(False, True)

			assertEquals(244, cacheSource.numWords())

			Dim mem1 As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).vectorLength(100).cache(cacheSource).seed(17).build()

			mem1.resetWeights(True)

			Dim mem2 As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).vectorLength(100).cache(cacheSource).seed(15).build()

			mem2.resetWeights(True)

			assertNotEquals(mem1.vector("day"), mem2.vector("day"))

			mem2.consume(mem1)

			assertEquals(mem1.vector("day"), mem2.vector("day"))

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) @Disabled("d file hash does not match expected hash: https://dl4jtest.blob.core.windows.net/resources/big/raw_sentences.txt.gzx.v1 ") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testConsumeOnNonEqualVocabs(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConsumeOnNonEqualVocabs(ByVal testDir As Path)
			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim cacheSource As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()


			Dim resource As File = Resources.asFile("big/raw_sentences.txt")

			Dim underlyingIterator As New BasicLineIterator(resource)


			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(underlyingIterator).tokenizerFactory(t).build()

			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim vocabConstructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 1).setTargetVocabCache(cacheSource).build()

			vocabConstructor.buildJointVocabulary(False, True)

			assertEquals(244, cacheSource.numWords())

			Dim mem1 As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).vectorLength(100).cache(cacheSource).build()

			mem1.resetWeights(True)



			Dim cacheTarget As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()


			Dim dir As val = testDir.toFile()
			Call (New ClassPathResource("/paravec/labeled/")).copyDirectory(dir)

			Dim labelAwareIterator As FileLabelAwareIterator = (New FileLabelAwareIterator.Builder()).addSourceFolder(dir).build()

			transformer = (New SentenceTransformer.Builder()).iterator(labelAwareIterator).tokenizerFactory(t).build()

			sequenceIterator = (New AbstractSequenceIterator.Builder(Of )(transformer)).build()

			Dim vocabTransfer As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 1).setTargetVocabCache(cacheTarget).build()

			vocabTransfer.buildMergedVocabulary(cacheSource, True)

			' those +3 go for 3 additional entries in target VocabCache: labels
			assertEquals(cacheSource.numWords() + 3, cacheTarget.numWords())


			Dim mem2 As InMemoryLookupTable(Of VocabWord) = CType((New InMemoryLookupTable.Builder(Of VocabWord)()).vectorLength(100).cache(cacheTarget).seed(18).build(), InMemoryLookupTable(Of VocabWord))

			mem2.resetWeights(True)

			assertNotEquals(mem1.vector("day"), mem2.vector("day"))

			mem2.consume(mem1)

			assertEquals(mem1.vector("day"), mem2.vector("day"))

			assertTrue(mem1.syn0_Conflict.rows() < mem2.syn0_Conflict.rows())

			assertEquals(mem1.syn0_Conflict.rows() + 3, mem2.syn0_Conflict.rows())
		End Sub
	End Class

End Namespace