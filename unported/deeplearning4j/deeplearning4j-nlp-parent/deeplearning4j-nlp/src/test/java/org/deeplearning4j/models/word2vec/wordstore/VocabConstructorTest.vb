Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.iterators
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports FileLabelAwareIterator = org.deeplearning4j.text.documentiterator.FileLabelAwareIterator
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports CommonPreprocessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.deeplearning4j.models.word2vec.wordstore



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class VocabConstructorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class VocabConstructorTest
		Inherits BaseDL4JTest



		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(VocabConstructorTest))

		Friend t As TokenizerFactory = New DefaultTokenizerFactory()




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			t.TokenPreProcessor = New CommonPreprocessor()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVocab() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVocab()
			Dim inputFile As File = Resources.asFile("big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(inputFile)

			Dim set As ISet(Of String) = New HashSet(Of String)()
			Dim lines As Integer = 0
			Dim cnt As Integer = 0
			Do While iter.hasNext()
				Dim tok As Tokenizer = t.create(iter.nextSentence())
				For Each token As String In tok.getTokens()
					If token Is Nothing OrElse token.Length = 0 OrElse token.Trim().Length = 0 Then
						Continue For
					End If
					cnt += 1

					If Not set.Contains(token) Then
						set.Add(token)
					End If
				Next token

				lines += 1
			Loop

			log.info("Total number of tokens: [" & cnt & "], lines: [" & lines & "], set size: [" & set.Count & "]")
			log.info("Set:" & vbLf & set)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBuildJointVocabulary1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBuildJointVocabulary1()
			Dim inputFile As File = Resources.asFile("big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(inputFile)

			Dim cache As VocabCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iter).tokenizerFactory(t).build()


	'        
	'            And we pack that transformer into AbstractSequenceIterator
	'         
			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim constructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 0).useAdaGrad(False).setTargetVocabCache(cache).build()

			constructor.buildJointVocabulary(True, False)


			assertEquals(244, cache.numWords())

			assertEquals(0, cache.totalWordOccurrences())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBuildJointVocabulary2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBuildJointVocabulary2()
			Dim inputFile As File = Resources.asFile("big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(inputFile)

			Dim cache As VocabCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iter).tokenizerFactory(t).build()


			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim constructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 5).useAdaGrad(False).setTargetVocabCache(cache).build()

			constructor.buildJointVocabulary(False, True)

			'        assertFalse(cache.hasToken("including"));

			assertEquals(242, cache.numWords())


			assertEquals("i", cache.wordAtIndex(1))
			assertEquals("it", cache.wordAtIndex(0))

			assertEquals(634303, cache.totalWordOccurrences())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCounter1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCounter1()
			Dim vocabCache As VocabCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<org.deeplearning4j.models.word2vec.VocabWord> words = new ArrayList<>();
			Dim words As IList(Of VocabWord) = New List(Of VocabWord)()

			words.Add(New VocabWord(1, "word"))
			words.Add(New VocabWord(2, "test"))
			words.Add(New VocabWord(1, "here"))

			Dim iterable As IEnumerable(Of Sequence(Of VocabWord)) = New IterableAnonymousInnerClass(Me, words)


			Dim sequenceIterator As SequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(iterable)).build()

			Dim constructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 0).useAdaGrad(False).setTargetVocabCache(vocabCache).build()

			constructor.buildJointVocabulary(False, True)

			assertEquals(3, vocabCache.numWords())

			assertEquals(1, vocabCache.wordFrequency("test"))
		End Sub

		Private Class IterableAnonymousInnerClass
			Implements IEnumerable(Of Sequence(Of VocabWord))

			Private ReadOnly outerInstance As VocabConstructorTest

			Private words As IList(Of VocabWord)

			Public Sub New(ByVal outerInstance As VocabConstructorTest, ByVal words As IList(Of VocabWord))
				Me.outerInstance = outerInstance
				Me.words = words
			End Sub

			Public Function GetEnumerator() As IEnumerator(Of Sequence(Of VocabWord)) Implements IEnumerator(Of Sequence(Of VocabWord)).GetEnumerator

				Return New IteratorAnonymousInnerClass(Me)
			End Function

			Private Class IteratorAnonymousInnerClass
				Implements IEnumerator(Of Sequence(Of VocabWord))

				Private ReadOnly outerInstance As IterableAnonymousInnerClass

				Public Sub New(ByVal outerInstance As IterableAnonymousInnerClass)
					Me.outerInstance = outerInstance
					switcher = New AtomicBoolean(True)
				End Sub

				Private switcher As AtomicBoolean

				Public Function hasNext() As Boolean
					Return switcher.getAndSet(False)
				End Function

				Public Function [next]() As Sequence(Of VocabWord)
					Dim sequence As New Sequence(Of VocabWord)(outerInstance.words)
					Return sequence
				End Function

				Public Sub remove()
					Throw New System.NotSupportedException()
				End Sub
			End Class
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCounter2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCounter2()
			Dim vocabCache As VocabCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<org.deeplearning4j.models.word2vec.VocabWord> words = new ArrayList<>();
			Dim words As IList(Of VocabWord) = New List(Of VocabWord)()

			words.Add(New VocabWord(1, "word"))
			words.Add(New VocabWord(0, "test"))
			words.Add(New VocabWord(1, "here"))

			Dim iterable As IEnumerable(Of Sequence(Of VocabWord)) = New IterableAnonymousInnerClass2(Me, words)


			Dim sequenceIterator As SequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(iterable)).build()

			Dim constructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 0).useAdaGrad(False).setTargetVocabCache(vocabCache).build()

			constructor.buildJointVocabulary(False, True)

			assertEquals(3, vocabCache.numWords())

			assertEquals(1, vocabCache.wordFrequency("test"))
		End Sub

		Private Class IterableAnonymousInnerClass2
			Implements IEnumerable(Of Sequence(Of VocabWord))

			Private ReadOnly outerInstance As VocabConstructorTest

			Private words As IList(Of VocabWord)

			Public Sub New(ByVal outerInstance As VocabConstructorTest, ByVal words As IList(Of VocabWord))
				Me.outerInstance = outerInstance
				Me.words = words
			End Sub

			Public Function GetEnumerator() As IEnumerator(Of Sequence(Of VocabWord)) Implements IEnumerator(Of Sequence(Of VocabWord)).GetEnumerator

				Return New IteratorAnonymousInnerClass2(Me)
			End Function

			Private Class IteratorAnonymousInnerClass2
				Implements IEnumerator(Of Sequence(Of VocabWord))

				Private ReadOnly outerInstance As IterableAnonymousInnerClass2

				Public Sub New(ByVal outerInstance As IterableAnonymousInnerClass2)
					Me.outerInstance = outerInstance
					switcher = New AtomicBoolean(True)
				End Sub

				Private switcher As AtomicBoolean

				Public Function hasNext() As Boolean
					Return switcher.getAndSet(False)
				End Function

				Public Function [next]() As Sequence(Of VocabWord)
					Dim sequence As New Sequence(Of VocabWord)(outerInstance.words)
					Return sequence
				End Function

				Public Sub remove()
					Throw New System.NotSupportedException()
				End Sub
			End Class
		End Class

		''' <summary>
		''' Here we test basic vocab transfer, done WITHOUT labels </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMergedVocab1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMergedVocab1()
			Dim cacheSource As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim cacheTarget As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim resource As File = Resources.asFile("big/raw_sentences.txt")

			Dim underlyingIterator As New BasicLineIterator(resource)


			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(underlyingIterator).tokenizerFactory(t).build()

			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim vocabConstructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 1).setTargetVocabCache(cacheSource).build()

			vocabConstructor.buildJointVocabulary(False, True)

			Dim sourceSize As Integer = cacheSource.numWords()
			log.info("Source Vocab size: " & sourceSize)


			Dim vocabTransfer As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 1).setTargetVocabCache(cacheTarget).build()

			vocabTransfer.buildMergedVocabulary(cacheSource, False)

			assertEquals(sourceSize, cacheTarget.numWords())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMergedVocabWithLabels1(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMergedVocabWithLabels1(ByVal testDir As Path)
			Dim cacheSource As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim cacheTarget As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim resource As File = Resources.asFile("big/raw_sentences.txt")

			Dim underlyingIterator As New BasicLineIterator(resource)


			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(underlyingIterator).tokenizerFactory(t).build()

			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim vocabConstructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 1).setTargetVocabCache(cacheSource).build()

			vocabConstructor.buildJointVocabulary(False, True)

			Dim sourceSize As Integer = cacheSource.numWords()
			log.info("Source Vocab size: " & sourceSize)

			Dim dir As val = testDir.toFile()
			Call (New ClassPathResource("/paravec/labeled/")).copyDirectory(dir)


			Dim labelAwareIterator As FileLabelAwareIterator = (New FileLabelAwareIterator.Builder()).addSourceFolder(dir).build()

			transformer = (New SentenceTransformer.Builder()).iterator(labelAwareIterator).tokenizerFactory(t).build()

			sequenceIterator = (New AbstractSequenceIterator.Builder(Of )(transformer)).build()

			Dim vocabTransfer As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 1).setTargetVocabCache(cacheTarget).build()

			vocabTransfer.buildMergedVocabulary(cacheSource, True)

			' those +3 go for 3 additional entries in target VocabCache: labels
			assertEquals(sourceSize + 3, cacheTarget.numWords())

			' now we check index equality for transferred elements
			assertEquals(cacheSource.wordAtIndex(17), cacheTarget.wordAtIndex(17))
			assertEquals(cacheSource.wordAtIndex(45), cacheTarget.wordAtIndex(45))
			assertEquals(cacheSource.wordAtIndex(89), cacheTarget.wordAtIndex(89))

			' we check that newly added labels have indexes beyond the VocabCache index space
			' please note, we need >= since the indexes are zero-based, and sourceSize is not
			assertTrue(cacheTarget.indexOf("Zfinance") > sourceSize - 1)
			assertTrue(cacheTarget.indexOf("Zscience") > sourceSize - 1)
			assertTrue(cacheTarget.indexOf("Zhealth") > sourceSize - 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransfer_1()
		Public Overridable Sub testTransfer_1()
			Dim vocab As val = New AbstractCache(Of VocabWord)()

			vocab.addToken(New VocabWord(1.0,"alpha"))
			vocab.addWordToIndex(0, "alpha")

			vocab.addToken(New VocabWord(2.0,"beta"))
			vocab.addWordToIndex(5, "beta")

			vocab.addToken(New VocabWord(3.0,"gamma"))
			vocab.addWordToIndex(10, "gamma")

			Dim constructor As val = (New VocabConstructor.Builder(Of VocabWord)()).build()


			Dim result As val = constructor.transferVocabulary(vocab, True)

			assertEquals(3, result.numWords())

			assertEquals("gamma", result.wordAtIndex(0))
			assertEquals("beta", result.wordAtIndex(1))
			assertEquals("alpha", result.wordAtIndex(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransfer_2()
		Public Overridable Sub testTransfer_2()
			Dim vocab As val = New AbstractCache(Of VocabWord)()

			vocab.addToken(New VocabWord(1.0,"alpha"))
			vocab.addWordToIndex(0, "alpha")

			vocab.addToken(New VocabWord(2.0,"beta"))
			vocab.addWordToIndex(5, "beta")

			vocab.addToken(New VocabWord(3.0,"gamma"))
			vocab.addWordToIndex(10, "gamma")

			Dim constructor As val = (New VocabConstructor.Builder(Of VocabWord)()).build()


			Dim result As val = constructor.transferVocabulary(vocab, False)

			assertEquals(3, result.numWords())

			assertEquals("gamma", result.wordAtIndex(10))
			assertEquals("beta", result.wordAtIndex(5))
			assertEquals("alpha", result.wordAtIndex(0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransfer_3()
		Public Overridable Sub testTransfer_3()
			Dim vocab As val = New AbstractCache(Of VocabWord)()

			vocab.addToken(New VocabWord(1.0,"alpha"))
			vocab.addWordToIndex(0, "alpha")

			vocab.addToken(New VocabWord(2.0,"beta"))
			vocab.addWordToIndex(5, "beta")

			vocab.addToken(New VocabWord(3.0,"gamma"))
			vocab.addWordToIndex(10, "gamma")

			Dim vocabIntersect As val = New AbstractCache(Of VocabWord)()

			vocabIntersect.addToken(New VocabWord(4.0,"alpha"))
			vocabIntersect.addWordToIndex(0, "alpha")

			vocab.addToken(New VocabWord(2.0,"delta"))
			vocab.addWordToIndex(15, "delta")


			Dim constructor As val = (New VocabConstructor.Builder(Of VocabWord)()).setTargetVocabCache(vocab).setLockFactor(False).build()

			Dim result As val = constructor.transferVocabulary(vocabIntersect, True)

			assertEquals(4, result.numWords())

			assertEquals("alpha", result.wordAtIndex(0))
			assertEquals(5.0, result.wordFrequency("alpha"), 1e-5)

			assertEquals("beta", result.wordAtIndex(5))
			assertEquals("gamma", result.wordAtIndex(10))
			assertEquals("delta", result.wordAtIndex(15))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(5000) public void testParallelTokenizationDisabled_Completes() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelTokenizationDisabled_Completes()
			Dim inputFile As File = Resources.asFile("big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(inputFile)

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iter).tokenizerFactory(t).build()

			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim constructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 5).allowParallelTokenization(False).build()

			constructor.buildJointVocabulary(False, True)
		End Sub
	End Class

End Namespace