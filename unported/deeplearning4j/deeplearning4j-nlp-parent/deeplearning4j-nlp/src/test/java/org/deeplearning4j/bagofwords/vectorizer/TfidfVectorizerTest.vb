Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Timeout = org.junit.jupiter.api.Timeout
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument
Imports LabelsSource = org.deeplearning4j.text.documentiterator.LabelsSource
Imports SimpleLabelAwareIterator = org.deeplearning4j.text.documentiterator.SimpleLabelAwareIterator
Imports CollectionSentenceIterator = org.deeplearning4j.text.sentenceiterator.CollectionSentenceIterator
Imports LabelAwareFileSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareFileSentenceIterator
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator
Imports DefaultTokenizer = org.deeplearning4j.text.tokenization.tokenizer.DefaultTokenizer
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
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

Namespace org.deeplearning4j.bagofwords.vectorizer



	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class TfidfVectorizerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class TfidfVectorizerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testTfIdfVectorizer(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTfIdfVectorizer(ByVal testDir As Path)
			Dim rootDir As val = testDir.toFile()
			Dim resource As New ClassPathResource("tripledir/")
			resource.copyDirectory(rootDir)

			assertTrue(rootDir.isDirectory())

			Dim iter As LabelAwareSentenceIterator = New LabelAwareFileSentenceIterator(rootDir)
			Dim tokenizerFactory As TokenizerFactory = New DefaultTokenizerFactory()

			Dim vectorizer As TfidfVectorizer = (New TfidfVectorizer.Builder()).setMinWordFrequency(1).setStopWords(New List(Of String)()).setTokenizerFactory(tokenizerFactory).setIterator(iter).allowParallelTokenization(False).build()

			vectorizer.fit()
			Dim word As VocabWord = vectorizer.getVocabCache().wordFor("file.")
			assertNotNull(word)
			assertEquals(word, vectorizer.getVocabCache().tokenFor("file."))
			assertEquals(3, vectorizer.getVocabCache().totalNumberOfDocs())

			assertEquals(3, word.SequencesCount)
			assertEquals(3, word.getElementFrequency(), 0.1)

			Dim word1 As VocabWord = vectorizer.getVocabCache().wordFor("1")

			assertEquals(1, word1.SequencesCount)
			assertEquals(1, word1.getElementFrequency(), 0.1)

			log.info("Labels used: " & vectorizer.LabelsSource.getLabels())
			assertEquals(3, vectorizer.LabelsSource.getNumberOfLabelsUsed())

			assertEquals(3, vectorizer.getVocabCache().totalNumberOfDocs())

			assertEquals(11, vectorizer.numWordsEncountered())

			Dim vector As INDArray = vectorizer.transform("This is 3 file.")
			log.info("TF-IDF vector: " & Arrays.toString(vector.data().asDouble()))

			Dim vocabCache As VocabCache(Of VocabWord) = vectorizer.getVocabCache()

			assertEquals(.04402, vector.getDouble(vocabCache.tokenFor("This").Index), 0.001)
			assertEquals(.04402, vector.getDouble(vocabCache.tokenFor("is").Index), 0.001)
			assertEquals(0.119, vector.getDouble(vocabCache.tokenFor("3").Index), 0.001)
			assertEquals(0, vector.getDouble(vocabCache.tokenFor("file.").Index), 0.001)



			Dim dataSet As DataSet = vectorizer.vectorize("This is 3 file.", "label3")
			'assertEquals(0.0, dataSet.getLabels().getDouble(0), 0.1);
			'assertEquals(0.0, dataSet.getLabels().getDouble(1), 0.1);
			'assertEquals(1.0, dataSet.getLabels().getDouble(2), 0.1);
			Dim cnt As Integer = 0
			For i As Integer = 0 To 2
				If dataSet.Labels.getDouble(i) > 0.1 Then
					cnt += 1
				End If
			Next i

			assertEquals(1, cnt)



			Dim tempFile As File = Files.createTempFile(testDir,"somefile","bin").toFile()
			tempFile.delete()

			SerializationUtils.saveObject(vectorizer, tempFile)

			Dim vectorizer2 As TfidfVectorizer = SerializationUtils.readObject(tempFile)
			vectorizer2.setTokenizerFactory(tokenizerFactory)

			dataSet = vectorizer2.vectorize("This is 3 file.", "label2")
			assertEquals(vector, dataSet.Features)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void testTfIdfVectorizerFromLabelAwareIterator() throws Exception
		Public Overridable Sub testTfIdfVectorizerFromLabelAwareIterator()
			Dim doc1 As New LabelledDocument()
			doc1.addLabel("dog")
			doc1.setContent("it barks like a dog")

			Dim doc2 As New LabelledDocument()
			doc2.addLabel("cat")
			doc2.setContent("it meows like a cat")

			Dim docs As IList(Of LabelledDocument) = New List(Of LabelledDocument)(2)
			docs.Add(doc1)
			docs.Add(doc2)

			Dim iterator As LabelAwareIterator = New SimpleLabelAwareIterator(docs)
			Dim tokenizerFactory As TokenizerFactory = New DefaultTokenizerFactory()

			Dim vectorizer As TfidfVectorizer = (New TfidfVectorizer.Builder()).setMinWordFrequency(1).setStopWords(New List(Of String)()).setTokenizerFactory(tokenizerFactory).setIterator(iterator).allowParallelTokenization(False).build()

			vectorizer.fit()

			Dim dataset As DataSet = vectorizer.vectorize("it meows like a cat", "cat")
			assertNotNull(dataset)

			Dim source As LabelsSource = vectorizer.LabelsSource
			assertEquals(2, source.NumberOfLabelsUsed)
			Dim labels As IList(Of String) = source.getLabels()
			assertEquals("dog", labels(0))
			assertEquals("cat", labels(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(10000L) public void testParallelFlag1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelFlag1()
			Dim vectorizer As val = (New TfidfVectorizer.Builder()).allowParallelTokenization(False).build()

			assertFalse(vectorizer.isParallel)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000L) public void testParallelFlag2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelFlag2()
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim collection As val = New List(Of String)()
			collection.add("First string")
			collection.add("Second string")
			collection.add("Third string")
			collection.add("")
			collection.add("Fifth string")
			Dim vectorizer As val = (New TfidfVectorizer.Builder()).allowParallelTokenization(False).setIterator(New CollectionSentenceIterator(collection)).setTokenizerFactory(New ExplodingTokenizerFactory(Me, 8, -1)).build()
			vectorizer.buildVocab()
			log.info("Fitting vectorizer...")
			vectorizer.fit()
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(20000L) public void testParallelFlag3() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelFlag3()
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim collection As val = New List(Of String)()
			collection.add("First string")
			collection.add("Second string")
			collection.add("Third string")
			collection.add("")
			collection.add("Fifth string")
			collection.add("Long long long string")
			collection.add("Sixth string")
			Dim vectorizer As val = (New TfidfVectorizer.Builder()).allowParallelTokenization(False).setIterator(New CollectionSentenceIterator(collection)).setTokenizerFactory(New ExplodingTokenizerFactory(Me, -1, 4)).build()
			vectorizer.buildVocab()
			log.info("Fitting vectorizer...")
			vectorizer.fit()
			End Sub)

		End Sub


		Protected Friend Class ExplodingTokenizerFactory
			Inherits DefaultTokenizerFactory

			Private ReadOnly outerInstance As TfidfVectorizerTest

			Protected Friend triggerSentence As Integer
			Protected Friend triggerWord As Integer
			Protected Friend cnt As New AtomicLong(0)

			Protected Friend Sub New(ByVal outerInstance As TfidfVectorizerTest, ByVal triggerSentence As Integer, ByVal triggerWord As Integer)
				Me.outerInstance = outerInstance
				Me.triggerSentence = triggerSentence
				Me.triggerWord = triggerWord
			End Sub

			Public Overrides Function create(ByVal toTokenize As String) As Tokenizer

				If triggerSentence >= 0 AndAlso cnt.incrementAndGet() >= triggerSentence Then
					Throw New ND4JIllegalStateException("TokenizerFactory exploded")
				End If


				Dim tkn As val = New ExplodingTokenizer(outerInstance, toTokenize, triggerWord)

				Return tkn
			End Function
		End Class

		Protected Friend Class ExplodingTokenizer
			Inherits DefaultTokenizer

			Private ReadOnly outerInstance As TfidfVectorizerTest

			Protected Friend triggerWord As Integer

			Public Sub New(ByVal outerInstance As TfidfVectorizerTest, ByVal [string] As String, ByVal triggerWord As Integer)
				MyBase.New([string])
				Me.outerInstance = outerInstance

				Me.triggerWord = triggerWord
				If Me.triggerWord >= 0 Then
					If Me.countTokens() >= triggerWord Then
						Throw New ND4JIllegalStateException("Tokenizer exploded")
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace