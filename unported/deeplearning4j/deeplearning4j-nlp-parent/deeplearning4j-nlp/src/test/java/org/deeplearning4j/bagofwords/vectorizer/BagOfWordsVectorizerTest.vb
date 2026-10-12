Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Tag = org.junit.jupiter.api.Tag
Imports Timeout = org.junit.jupiter.api.Timeout
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ArgMax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports LabelAwareFileSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareFileSentenceIterator
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Test = org.junit.jupiter.api.Test
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class BagOfWordsVectorizerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class BagOfWordsVectorizerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testBagOfWordsVectorizer(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBagOfWordsVectorizer(ByVal testDir As Path)
			Dim rootDir As val = testDir.toFile()
			Dim resource As New ClassPathResource("rootdir/")
			resource.copyDirectory(rootDir)

			Dim iter As LabelAwareSentenceIterator = New LabelAwareFileSentenceIterator(rootDir)
			Dim labels As IList(Of String) = New List(Of String) From {"label1", "label2"}
			Dim tokenizerFactory As TokenizerFactory = New DefaultTokenizerFactory()

			Dim vectorizer As BagOfWordsVectorizer = (New BagOfWordsVectorizer.Builder()).setMinWordFrequency(1).setStopWords(New List(Of String)()).setTokenizerFactory(tokenizerFactory).setIterator(iter).allowParallelTokenization(False).build()

			vectorizer.fit()
			Dim word As VocabWord = vectorizer.getVocabCache().wordFor("file.")
			assertNotNull(word)
			assertEquals(word, vectorizer.getVocabCache().tokenFor("file."))
			assertEquals(2, vectorizer.getVocabCache().totalNumberOfDocs())

			assertEquals(2, word.SequencesCount)
			assertEquals(2, word.getElementFrequency(), 0.1)

			Dim word1 As VocabWord = vectorizer.getVocabCache().wordFor("1")

			assertEquals(1, word1.SequencesCount)
			assertEquals(1, word1.getElementFrequency(), 0.1)

			log.info("Labels used: " & vectorizer.LabelsSource.getLabels())
			assertEquals(2, vectorizer.LabelsSource.getNumberOfLabelsUsed())

			'/////////////////
			Dim array As INDArray = vectorizer.transform("This is 2 file.")
			log.info("Transformed array: " & array)
			assertEquals(5, array.columns())


			Dim vocabCache As VocabCache(Of VocabWord) = vectorizer.getVocabCache()

			assertEquals(2, array.getDouble(vocabCache.tokenFor("This").Index), 0.1)
			assertEquals(2, array.getDouble(vocabCache.tokenFor("is").Index), 0.1)
			assertEquals(2, array.getDouble(vocabCache.tokenFor("file.").Index), 0.1)
			assertEquals(0, array.getDouble(vocabCache.tokenFor("1").Index), 0.1)
			assertEquals(1, array.getDouble(vocabCache.tokenFor("2").Index), 0.1)

			Dim dataSet As DataSet = vectorizer.vectorize("This is 2 file.", "label2")
			assertEquals(array, dataSet.Features)

			Dim labelz As INDArray = dataSet.Labels
			log.info("Labels array: " & labelz)

			Dim idx2 As Integer = Nd4j.Executioner.exec(New ArgMax(labelz))(0).getInt(0)
			'int idx2 = ((IndexAccumulation) Nd4j.getExecutioner().exec(new IMax(labelz))).getFinalResult().intValue();

			'        assertEquals(1.0, dataSet.getLabels().getDouble(0), 0.1);
			'        assertEquals(0.0, dataSet.getLabels().getDouble(1), 0.1);

			dataSet = vectorizer.vectorize("This is 1 file.", "label1")

			assertEquals(2, dataSet.Features.getDouble(vocabCache.tokenFor("This").Index), 0.1)
			assertEquals(2, dataSet.Features.getDouble(vocabCache.tokenFor("is").Index), 0.1)
			assertEquals(2, dataSet.Features.getDouble(vocabCache.tokenFor("file.").Index), 0.1)
			assertEquals(1, dataSet.Features.getDouble(vocabCache.tokenFor("1").Index), 0.1)
			assertEquals(0, dataSet.Features.getDouble(vocabCache.tokenFor("2").Index), 0.1)

			Dim idx1 As Integer = Nd4j.Executioner.exec(New ArgMax(dataSet.Labels))(0).getInt(0)
			'int idx1 = ((IndexAccumulation) Nd4j.getExecutioner().exec(new IMax(dataSet.getLabels()))).getFinalResult().intValue();

			'assertEquals(0.0, dataSet.getLabels().getDouble(0), 0.1);
			'assertEquals(1.0, dataSet.getLabels().getDouble(1), 0.1);

			assertNotEquals(idx2, idx1)

			' Serialization check
			Dim tempFile As File = createTempFile(testDir,"fdsf", "fdfsdf")
			tempFile.deleteOnExit()

			SerializationUtils.saveObject(vectorizer, tempFile)

			Dim vectorizer2 As BagOfWordsVectorizer = SerializationUtils.readObject(tempFile)
			vectorizer2.setTokenizerFactory(tokenizerFactory)

			dataSet = vectorizer2.vectorize("This is 2 file.", "label2")
			assertEquals(array, dataSet.Features)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.io.File createTempFile(java.nio.file.Path tempDir,String prefix, String suffix) throws java.io.IOException
		Private Function createTempFile(ByVal tempDir As Path, ByVal prefix As String, ByVal suffix As String) As File
			Dim newFile As File = Files.createTempFile(tempDir,prefix & "-" & System.nanoTime(),suffix).toFile()
			Return newFile
		End Function

	End Class

End Namespace