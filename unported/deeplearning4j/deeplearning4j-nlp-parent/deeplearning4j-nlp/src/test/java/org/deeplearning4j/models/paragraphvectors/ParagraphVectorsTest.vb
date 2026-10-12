Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports IOUtils = org.apache.commons.io.IOUtils
Imports LineIterator = org.apache.commons.io.LineIterator
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports BasicTransformerIterator = org.deeplearning4j.models.sequencevectors.transformers.impl.iterables.BasicTransformerIterator
Imports org.deeplearning4j.text.sentenceiterator
Imports Tag = org.junit.jupiter.api.Tag
Imports Timeout = org.junit.jupiter.api.Timeout
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.learning.impl.sequence
Imports org.deeplearning4j.models.embeddings.learning.impl.sequence
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports InMemoryLookupCache = org.deeplearning4j.models.word2vec.wordstore.inmemory.InMemoryLookupCache
Imports FileLabelAwareIterator = org.deeplearning4j.text.documentiterator.FileLabelAwareIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument
Imports LabelsSource = org.deeplearning4j.text.documentiterator.LabelsSource
Imports SentenceIteratorConverter = org.deeplearning4j.text.sentenceiterator.interoperability.SentenceIteratorConverter
Imports CommonPreprocessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CollectionUtils = org.nd4j.common.io.CollectionUtils
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports Resources = org.nd4j.common.resources.Resources
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

Namespace org.deeplearning4j.models.paragraphvectors




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class ParagraphVectorsTest extends org.deeplearning4j.BaseDL4JTest
	Public Class ParagraphVectorsTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return If(IntegrationTests, 600_000, 240_000)
			End Get
		End Property


		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

		Public Overrides ReadOnly Property DefaultFPDataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property



		''' <summary>
		''' This test checks, how vocab is built using SentenceIterator provided, without labels.
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Timeout(2400000) @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParagraphVectorsVocabBuilding1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParagraphVectorsVocabBuilding1()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(file) 'UimaSentenceIterator.createWithPath(file.getAbsolutePath());

			Dim numberOfLines As Integer = 0
			Do While iter.hasNext()
				iter.nextSentence()
				numberOfLines += 1
			Loop

			iter.reset()

			Dim cache As New InMemoryLookupCache(False)

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			' LabelsSource source = new LabelsSource("DOC_");

			Dim vec As ParagraphVectors = (New ParagraphVectors.Builder()).minWordFrequency(1).iterations(5).layerSize(100).windowSize(5).iterate(iter).vocabCache(cache).tokenizerFactory(t).build()

			vec.buildVocab()

			Dim source As LabelsSource = vec.getLabelsSource()


			'VocabCache cache = vec.getVocab();
			log.info("Number of lines in corpus: " & numberOfLines)
			assertEquals(numberOfLines, source.getLabels().Count)
			assertEquals(97162, source.getLabels().Count)

			assertNotEquals(Nothing, cache)
			assertEquals(97406, cache.numWords())

			' proper number of words for minWordsFrequency = 1 is 244
			assertEquals(244, cache.numWords() - source.getLabels().Count)
		End Sub

		''' <summary>
		''' This test doesn't really cares about actual results. We only care about equality between live model & restored models
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Timeout(3000000) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParagraphVectorsModelling1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParagraphVectorsModelling1(ByVal backend As Nd4jBackend)
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(file)

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim source As New LabelsSource("DOC_")

			Dim vec As ParagraphVectors = (New ParagraphVectors.Builder()).minWordFrequency(1).iterations(5).seed(119).epochs(1).layerSize(150).learningRate(0.025).labelsSource(source).windowSize(5).sequenceLearningAlgorithm(New DM(Of VocabWord)()).iterate(iter).trainWordVectors(True).usePreciseWeightInit(True).batchSize(8192).tokenizerFactory(t).workers(4).sampling(0).build()

			vec.fit()

			Dim cache As VocabCache(Of VocabWord) = vec.getVocab()

			Dim fullFile As File = File.createTempFile("paravec", "tests")
			fullFile.deleteOnExit()

			Dim originalSyn1_17 As INDArray = CType(vec.getLookupTable(), InMemoryLookupTable).getSyn1().getRow(17, True).dup()

			WordVectorSerializer.writeParagraphVectors(vec, fullFile)

			Dim cnt1 As Integer = cache.wordFrequency("day")
			Dim cnt2 As Integer = cache.wordFrequency("me")

			assertNotEquals(1, cnt1)
			assertNotEquals(1, cnt2)
			assertNotEquals(cnt1, cnt2)

			assertEquals(97406, cache.numWords())

			assertTrue(vec.hasWord("DOC_16392"))
			assertTrue(vec.hasWord("DOC_3720"))

			Dim result As IList(Of String) = New List(Of String)(vec.nearestLabels(vec.getWordVectorMatrix("DOC_16392"), 10))
			Console.WriteLine("nearest labels: " & result)
			For Each label As String In result
				Console.WriteLine(label & "/DOC_16392: " & vec.similarity(label, "DOC_16392"))
			Next label
			assertTrue(result.Contains("DOC_16392"))
			'assertTrue(result.contains("DOC_21383"));



	'        
	'            We have few lines that contain pretty close words invloved.
	'            These sentences should be pretty close to each other in vector space
	'         
			' line 3721: This is my way .
			' line 6348: This is my case .
			' line 9836: This is my house .
			' line 12493: This is my world .
			' line 16393: This is my work .

			' this is special sentence, that has nothing common with previous sentences
			' line 9853: We now have one .

			Dim similarityD As Double = vec.similarity("day", "night")
			log.info("day/night similarity: " & similarityD)

			If similarityD < 0.0 Then
				log.info("Day: " & java.util.Arrays.toString(vec.getWordVectorMatrix("day").dup().data().asDouble()))
				log.info("Night: " & java.util.Arrays.toString(vec.getWordVectorMatrix("night").dup().data().asDouble()))
			End If


			Dim labelsOriginal As IList(Of String) = vec.labelsSource.getLabels()

			Dim similarityW As Double = vec.similarity("way", "work")
			log.info("way/work similarity: " & similarityW)

			Dim similarityH As Double = vec.similarity("house", "world")
			log.info("house/world similarity: " & similarityH)

			Dim similarityC As Double = vec.similarity("case", "way")
			log.info("case/way similarity: " & similarityC)

			Dim similarity1 As Double = vec.similarity("DOC_9835", "DOC_12492")
			log.info("9835/12492 similarity: " & similarity1)
			'        assertTrue(similarity1 > 0.7d);

			Dim similarity2 As Double = vec.similarity("DOC_3720", "DOC_16392")
			log.info("3720/16392 similarity: " & similarity2)
			'        assertTrue(similarity2 > 0.7d);

			Dim similarity3 As Double = vec.similarity("DOC_6347", "DOC_3720")
			log.info("6347/3720 similarity: " & similarity3)
			'        assertTrue(similarity2 > 0.7d);

			' likelihood in this case should be significantly lower
			Dim similarityX As Double = vec.similarity("DOC_3720", "DOC_9852")
			log.info("3720/9852 similarity: " & similarityX)
			assertTrue(similarityX < 0.5R)

			Dim tempFile As File = File.createTempFile("paravec", "ser")
			tempFile.deleteOnExit()

			Dim day As INDArray = vec.getWordVectorMatrix("day").dup()

	'        
	'            Testing txt serialization
	'         
			Dim tempFile2 As File = File.createTempFile("paravec", "ser")
			tempFile2.deleteOnExit()

			WordVectorSerializer.writeWordVectors(vec, tempFile2)

			Dim vec3 As ParagraphVectors = WordVectorSerializer.readParagraphVectorsFromText(tempFile2)

			Dim day3 As INDArray = vec3.getWordVectorMatrix("day").dup()

			Dim labelsRestored As IList(Of String) = vec3.labelsSource.getLabels()

			assertEquals(day, day3)

			assertEquals(labelsOriginal.Count, labelsRestored.Count)

	'        
	'         Testing binary serialization
	'        
			SerializationUtils.saveObject(vec, tempFile)


			Dim vec2 As ParagraphVectors = SerializationUtils.readObject(tempFile)
			Dim day2 As INDArray = vec2.getWordVectorMatrix("day").dup()

			Dim labelsBinary As IList(Of String) = vec2.labelsSource.getLabels()

			assertEquals(day, day2)

			tempFile.delete()


			assertEquals(labelsOriginal.Count, labelsBinary.Count)

			Dim original As INDArray = vec.getWordVectorMatrix("DOC_16392").dup()
			Dim originalPreserved As INDArray = original.dup()
			Dim inferredA1 As INDArray = vec.inferVector("This is my work .")
			Dim inferredB1 As INDArray = vec.inferVector("This is my work .")

			Dim cosAO1 As Double = Transforms.cosineSim(inferredA1.dup(), original.dup())
			Dim cosAB1 As Double = Transforms.cosineSim(inferredA1.dup(), inferredB1.dup())

			log.info("Cos O/A: {}", cosAO1)
			log.info("Cos A/B: {}", cosAB1)
			log.info("Inferred: {}", inferredA1)
			'        assertTrue(cosAO1 > 0.45);
			assertTrue(cosAB1 > 0.95)

			'assertArrayEquals(inferredA.data().asDouble(), inferredB.data().asDouble(), 0.01);

			Dim restoredVectors As ParagraphVectors = WordVectorSerializer.readParagraphVectors(fullFile)
			restoredVectors.TokenizerFactory = t

			Dim restoredSyn1_17 As INDArray = CType(restoredVectors.getLookupTable(), InMemoryLookupTable).getSyn1().getRow(17, True).dup()

			assertEquals(originalSyn1_17, restoredSyn1_17)

			Dim originalRestored As INDArray = vec.getWordVectorMatrix("DOC_16392").dup()

			assertEquals(originalPreserved, originalRestored)

			Dim inferredA2 As INDArray = restoredVectors.inferVector("This is my work .")
			Dim inferredB2 As INDArray = restoredVectors.inferVector("This is my work .")
			Dim inferredC2 As INDArray = restoredVectors.inferVector("world way case .")

			Dim cosAO2 As Double = Transforms.cosineSim(inferredA2.dup(), original.dup())
			Dim cosAB2 As Double = Transforms.cosineSim(inferredA2.dup(), inferredB2.dup())
			Dim cosAAX As Double = Transforms.cosineSim(inferredA1.dup(), inferredA2.dup())
			Dim cosAC2 As Double = Transforms.cosineSim(inferredC2.dup(), inferredA2.dup())

			log.info("Cos A2/B2: {}", cosAB2)
			log.info("Cos A1/A2: {}", cosAAX)
			log.info("Cos O/A2: {}", cosAO2)
			log.info("Cos C2/A2: {}", cosAC2)

			log.info("Vector: {}", java.util.Arrays.toString(inferredA1.data().asFloat()))

			log.info("cosAO2: {}", cosAO2)

			'  assertTrue(cosAO2 > 0.45);
			assertTrue(cosAB2 > 0.95)
			assertTrue(cosAAX > 0.95)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParagraphVectorsDM() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParagraphVectorsDM()
			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(file)

			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim source As New LabelsSource("DOC_")

			Dim vec As ParagraphVectors = (New ParagraphVectors.Builder()).minWordFrequency(1).iterations(2).seed(119).epochs(1).layerSize(100).learningRate(0.025).labelsSource(source).windowSize(5).iterate(iter).trainWordVectors(True).vocabCache(cache).tokenizerFactory(t).negativeSample(0).useHierarchicSoftmax(True).sampling(0).workers(1).usePreciseWeightInit(True).sequenceLearningAlgorithm(New DM(Of VocabWord)()).build()

			vec.fit()


			Dim cnt1 As Integer = cache.wordFrequency("day")
			Dim cnt2 As Integer = cache.wordFrequency("me")

			assertNotEquals(1, cnt1)
			assertNotEquals(1, cnt2)
			assertNotEquals(cnt1, cnt2)

			Dim simDN As Double = vec.similarity("day", "night")
			log.info("day/night similariry: {}", simDN)

			Dim similarity1 As Double = vec.similarity("DOC_9835", "DOC_12492")
			log.info("9835/12492 similarity: " & similarity1)
			'        assertTrue(similarity1 > 0.2d);

			Dim similarity2 As Double = vec.similarity("DOC_3720", "DOC_16392")
			log.info("3720/16392 similarity: " & similarity2)
			'      assertTrue(similarity2 > 0.2d);

			Dim similarity3 As Double = vec.similarity("DOC_6347", "DOC_3720")
			log.info("6347/3720 similarity: " & similarity3)
			'        assertTrue(similarity3 > 0.6d);

			Dim similarityX As Double = vec.similarity("DOC_3720", "DOC_9852")
			log.info("3720/9852 similarity: " & similarityX)
			If IntegrationTests Then
				assertTrue(similarityX < 0.5R)
			End If


			' testing DM inference now

			Dim original As INDArray = vec.getWordVectorMatrix("DOC_16392").dup()
			Dim inferredA1 As INDArray = vec.inferVector("This is my work")
			Dim inferredB1 As INDArray = vec.inferVector("This is my work .")

			Dim cosAO1 As Double = Transforms.cosineSim(inferredA1.dup(), original.dup())
			Dim cosAB1 As Double = Transforms.cosineSim(inferredA1.dup(), inferredB1.dup())

			log.info("Cos O/A: {}", cosAO1)
			log.info("Cos A/B: {}", cosAB1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Timeout(300000) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParagraphVectorsDBOW() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParagraphVectorsDBOW()
			skipUnlessIntegrationTests()

			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(file)

			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim source As New LabelsSource("DOC_")

			Dim vec As ParagraphVectors = (New ParagraphVectors.Builder()).minWordFrequency(1).iterations(5).seed(119).epochs(1).layerSize(100).learningRate(0.025).labelsSource(source).windowSize(5).iterate(iter).trainWordVectors(True).vocabCache(cache).tokenizerFactory(t).negativeSample(0).allowParallelTokenization(True).useHierarchicSoftmax(True).sampling(0).workers(4).usePreciseWeightInit(True).sequenceLearningAlgorithm(New DBOW(Of VocabWord)()).build()

			vec.fit()

			assertFalse(CType(vec.getLookupTable(), InMemoryLookupTable(Of VocabWord)).getSyn0().Attached)
			assertFalse(CType(vec.getLookupTable(), InMemoryLookupTable(Of VocabWord)).getSyn1().Attached)

			Dim cnt1 As Integer = cache.wordFrequency("day")
			Dim cnt2 As Integer = cache.wordFrequency("me")

			assertNotEquals(1, cnt1)
			assertNotEquals(1, cnt2)
			assertNotEquals(cnt1, cnt2)

			Dim simDN As Double = vec.similarity("day", "night")
			log.info("day/night similariry: {}", simDN)

			Dim similarity1 As Double = vec.similarity("DOC_9835", "DOC_12492")
			log.info("9835/12492 similarity: " & similarity1)
			'        assertTrue(similarity1 > 0.2d);

			Dim similarity2 As Double = vec.similarity("DOC_3720", "DOC_16392")
			log.info("3720/16392 similarity: " & similarity2)
			'      assertTrue(similarity2 > 0.2d);

			Dim similarity3 As Double = vec.similarity("DOC_6347", "DOC_3720")
			log.info("6347/3720 similarity: " & similarity3)
			'        assertTrue(similarity3 > 0.6d);

			Dim similarityX As Double = vec.similarity("DOC_3720", "DOC_9852")
			log.info("3720/9852 similarity: " & similarityX)
			assertTrue(similarityX < 0.5R)


			' testing DM inference now

			Dim original As INDArray = vec.getWordVectorMatrix("DOC_16392").dup()
			Dim inferredA1 As INDArray = vec.inferVector("This is my work")
			Dim inferredB1 As INDArray = vec.inferVector("This is my work .")
			Dim inferredC1 As INDArray = vec.inferVector("This is my day")
			Dim inferredD1 As INDArray = vec.inferVector("This is my night")

			log.info("A: {}", java.util.Arrays.toString(inferredA1.data().asFloat()))
			log.info("C: {}", java.util.Arrays.toString(inferredC1.data().asFloat()))

			assertNotEquals(inferredA1, inferredC1)

			Dim cosAO1 As Double = Transforms.cosineSim(inferredA1.dup(), original.dup())
			Dim cosAB1 As Double = Transforms.cosineSim(inferredA1.dup(), inferredB1.dup())
			Dim cosAC1 As Double = Transforms.cosineSim(inferredA1.dup(), inferredC1.dup())
			Dim cosCD1 As Double = Transforms.cosineSim(inferredD1.dup(), inferredC1.dup())

			log.info("Cos O/A: {}", cosAO1)
			log.info("Cos A/B: {}", cosAB1)
			log.info("Cos A/C: {}", cosAC1)
			log.info("Cos C/D: {}", cosCD1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParagraphVectorsWithWordVectorsModelling1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParagraphVectorsWithWordVectorsModelling1()
			Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
			If Not IntegrationTests AndAlso "CUDA".Equals(backend, StringComparison.OrdinalIgnoreCase) Then
				skipUnlessIntegrationTests() 'Skip CUDA except for integration tests due to very slow test speed
			End If

			Dim file As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(file)

			'        InMemoryLookupCache cache = new InMemoryLookupCache(false);
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim source As New LabelsSource("DOC_")

			Dim vec As ParagraphVectors = (New ParagraphVectors.Builder()).minWordFrequency(1).iterations(3).epochs(1).layerSize(100).learningRate(0.025).labelsSource(source).windowSize(5).iterate(iter).trainWordVectors(True).vocabCache(cache).tokenizerFactory(t).sampling(0).build()

			vec.fit()


			Dim cnt1 As Integer = cache.wordFrequency("day")
			Dim cnt2 As Integer = cache.wordFrequency("me")

			assertNotEquals(1, cnt1)
			assertNotEquals(1, cnt2)
			assertNotEquals(cnt1, cnt2)

	'        
	'            We have few lines that contain pretty close words invloved.
	'            These sentences should be pretty close to each other in vector space
	'         
			' line 3721: This is my way .
			' line 6348: This is my case .
			' line 9836: This is my house .
			' line 12493: This is my world .
			' line 16393: This is my work .

			' this is special sentence, that has nothing common with previous sentences
			' line 9853: We now have one .

			assertTrue(vec.hasWord("DOC_3720"))

			Dim similarityD As Double = vec.similarity("day", "night")
			log.info("day/night similarity: " & similarityD)

			Dim similarityW As Double = vec.similarity("way", "work")
			log.info("way/work similarity: " & similarityW)

			Dim similarityH As Double = vec.similarity("house", "world")
			log.info("house/world similarity: " & similarityH)

			Dim similarityC As Double = vec.similarity("case", "way")
			log.info("case/way similarity: " & similarityC)

			Dim similarity1 As Double = vec.similarity("DOC_9835", "DOC_12492")
			log.info("9835/12492 similarity: " & similarity1)
			'        assertTrue(similarity1 > 0.7d);

			Dim similarity2 As Double = vec.similarity("DOC_3720", "DOC_16392")
			log.info("3720/16392 similarity: " & similarity2)
			'        assertTrue(similarity2 > 0.7d);

			Dim similarity3 As Double = vec.similarity("DOC_6347", "DOC_3720")
			log.info("6347/3720 similarity: " & similarity3)
			'        assertTrue(similarity2 > 0.7d);

			' likelihood in this case should be significantly lower
			' however, since corpus is small, and weight initialization is random-based, sometimes this test CAN fail
			Dim similarityX As Double = vec.similarity("DOC_3720", "DOC_9852")
			log.info("3720/9852 similarity: " & similarityX)
			assertTrue(similarityX < 0.5R)


			Dim sim119 As Double = vec.similarityToLabel("This is my case .", "DOC_6347")
			Dim sim120 As Double = vec.similarityToLabel("This is my case .", "DOC_3720")
			log.info("1/2: " & sim119 & "/" & sim120)
			'assertEquals(similarity3, sim119, 0.001);
		End Sub


		''' <summary>
		''' This test is not indicative.
		''' there's no need in this test within travis, use it manually only for problems detection
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParagraphVectorsReducedLabels1(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParagraphVectorsReducedLabels1(ByVal testDir As Path)
			Dim tempDir As val = testDir.toFile()
			Dim resource As New ClassPathResource("/labeled")
			resource.copyDirectory(tempDir)

			Dim iter As LabelAwareIterator = (New FileLabelAwareIterator.Builder()).addSourceFolder(tempDir).build()

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()

			''' <summary>
			''' Please note: text corpus is REALLY small, and some kind of "results" could be received with HIGH epochs number, like 30.
			''' But there's no reason to keep at that high
			''' </summary>

			Dim vec As ParagraphVectors = (New ParagraphVectors.Builder()).minWordFrequency(1).epochs(3).layerSize(100).stopWords(New List(Of String)()).windowSize(5).iterate(iter).tokenizerFactory(t).build()

			vec.fit()

			'WordVectorSerializer.writeWordVectors(vec, "vectors.txt");

			Dim w1 As INDArray = vec.lookupTable().vector("I")
			Dim w2 As INDArray = vec.lookupTable().vector("am")
			Dim w3 As INDArray = vec.lookupTable().vector("sad.")

			Dim words As INDArray = Nd4j.create(3, vec.lookupTable().layerSize())

			words.putRow(0, w1)
			words.putRow(1, w2)
			words.putRow(2, w3)


			Dim mean As INDArray = If(words.Matrix, words.mean(0), words)

			log.info("Mean" & java.util.Arrays.toString(mean.dup().data().asDouble()))
			log.info("Array" & java.util.Arrays.toString(vec.lookupTable().vector("negative").dup().data().asDouble()))

			Dim simN As Double = Transforms.cosineSim(mean, vec.lookupTable().vector("negative"))
			log.info("Similarity negative: " & simN)


			Dim simP As Double = Transforms.cosineSim(mean, vec.lookupTable().vector("neutral"))
			log.info("Similarity neutral: " & simP)

			Dim simV As Double = Transforms.cosineSim(mean, vec.lookupTable().vector("positive"))
			log.info("Similarity positive: " & simV)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParallelIterator() throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelIterator()
			Dim factory As TokenizerFactory = New DefaultTokenizerFactory()
			Dim iterator As SentenceIterator = New BasicLineIterator(Resources.asFile("big/raw_sentences.txt"))

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iterator).allowMultithreading(True).tokenizerFactory(factory).build()

			Dim iter As BasicTransformerIterator = CType(transformer.GetEnumerator(), BasicTransformerIterator)
			For i As Integer = 0 To 99
				Dim cnt As Integer = 0
				Dim counter As Long = 0
				Dim sequence As Sequence(Of VocabWord) = Nothing
				Do While iter.MoveNext()
					sequence = iter.Current
					counter += sequence.size()
					cnt += 1
				Loop
				iter.reset()
				assertEquals(757172, counter)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testIterator(@TempDir Path testDir) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIterator(ByVal testDir As Path)
			Dim folder_labeled As val = New File(testDir.toFile(),"labeled")
			Dim folder_unlabeled As val = New File(testDir.toFile(),"unlabeled")
			assertTrue(folder_labeled.mkdirs())
			assertTrue(folder_labeled.mkdirs())
			Call (New ClassPathResource("/paravec/labeled/")).copyDirectory(folder_labeled)
			Call (New ClassPathResource("/paravec/unlabeled/")).copyDirectory(folder_unlabeled)


			Dim labelAwareIterator As FileLabelAwareIterator = (New FileLabelAwareIterator.Builder()).addSourceFolder(folder_labeled).build()

			Dim resource_sentences As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iter As SentenceIterator = New BasicLineIterator(resource_sentences)

			Dim i As Integer = 0
			Do While i < 10
				Dim j As Integer = 0
				Dim labels As Integer = 0
				Dim words As Integer = 0
				Do While labelAwareIterator.hasNextDocument()
					j += 1
					Dim document As LabelledDocument = labelAwareIterator.nextDocument()
					labels += document.getLabels().size()
					Dim lst As IList(Of VocabWord) = document.getReferencedContent()
					If Not CollectionUtils.isEmpty(lst) Then
						words += lst.Count
					End If
				Loop
				labelAwareIterator.reset()
				'System.out.println(words + " " + labels + " " + j);
				assertEquals(0, words)
				assertEquals(30, labels)
				assertEquals(30, j)
				j = 0
				Do While iter.hasNext()
					j += 1
					iter.nextSentence()
				Loop
				assertEquals(97162, j)
				iter.reset()
				i += 1
			Loop

		End Sub

	'    
	'        In this test we'll build w2v model, and will use it's vocab and weights for ParagraphVectors.
	'        there's no need in this test within travis, use it manually only for problems detection
	'    
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testParagraphVectorsOverExistingWordVectorsModel(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParagraphVectorsOverExistingWordVectorsModel(ByVal testDir As Path)
			Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
			If Not IntegrationTests AndAlso "CUDA".Equals(backend, StringComparison.OrdinalIgnoreCase) Then
				skipUnlessIntegrationTests() 'Skip CUDA except for integration tests due to very slow test speed
			End If

			' we build w2v from multiple sources, to cover everything
			Dim resource_sentences As File = Resources.asFile("/big/raw_sentences.txt")

			Dim folder_mixed As val = testDir.toFile()
			Dim resource_mixed As New ClassPathResource("paravec/")
			resource_mixed.copyDirectory(folder_mixed)

			Dim iter As SentenceIterator = (New AggregatingSentenceIterator.Builder()).addSentenceIterator(New BasicLineIterator(resource_sentences)).addSentenceIterator(New FileSentenceIterator(folder_mixed)).build()

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim wordVectors As Word2Vec = (New Word2Vec.Builder()).seed(119).minWordFrequency(1).batchSize(250).iterations(1).epochs(1).learningRate(0.025).layerSize(150).minLearningRate(0.001).elementsLearningAlgorithm(New SkipGram(Of VocabWord)()).useHierarchicSoftmax(True).windowSize(5).allowParallelTokenization(True).workers(1).iterate(iter).tokenizerFactory(t).build()

			wordVectors.fit()

			Dim day_A As VocabWord = wordVectors.getVocab().tokenFor("day")

			Dim vector_day1 As INDArray = wordVectors.getWordVectorMatrix("day").dup()

			' At this moment we have ready w2v model. It's time to use it for ParagraphVectors

			Dim folder_labeled As val = New File(testDir.toFile(),"labeled")
			Dim folder_unlabeled As val = New File(testDir.toFile(),"unlabeled")
			Call (New ClassPathResource("/paravec/labeled/")).copyDirectory(folder_labeled)
			Call (New ClassPathResource("/paravec/unlabeled/")).copyDirectory(folder_unlabeled)


			Dim labelAwareIterator As FileLabelAwareIterator = (New FileLabelAwareIterator.Builder()).addSourceFolder(folder_labeled).build()


			' documents from this iterator will be used for classification
			Dim unlabeledIterator As FileLabelAwareIterator = (New FileLabelAwareIterator.Builder()).addSourceFolder(folder_unlabeled).build()


			' we're building classifier now, with pre-built w2v model passed in
'JAVA TO VB CONVERTER NOTE: The variable paragraphVectors was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim paragraphVectors_Conflict As ParagraphVectors = (New ParagraphVectors.Builder()).seed(119).iterate(labelAwareIterator).learningRate(0.025).minLearningRate(0.001).iterations(10).epochs(1).layerSize(150).tokenizerFactory(t).sequenceLearningAlgorithm(New DBOW(Of VocabWord)()).useHierarchicSoftmax(True).allowParallelTokenization(True).workers(1).trainWordVectors(False).useExistingWordVectors(wordVectors).build()

			paragraphVectors_Conflict.fit()

			Dim day_B As VocabWord = paragraphVectors_Conflict.getVocab().tokenFor("day")

			assertEquals(day_A.Index, day_B.Index)

	'        
	'        double similarityD = wordVectors.similarity("day", "night");
	'        log.info("day/night similarity: " + similarityD);
	'        assertTrue(similarityD > 0.5d);
	'        

			Dim vector_day2 As INDArray = paragraphVectors_Conflict.getWordVectorMatrix("day").dup()
			Dim crossDay As Double = arraysSimilarity(vector_day1, vector_day2)

			log.info("Day1: " & vector_day1)
			log.info("Day2: " & vector_day2)
			log.info("Cross-Day similarity: " & crossDay)
			log.info("Cross-Day similiarity 2: " & Transforms.cosineSim(Transforms.unitVec(vector_day1), Transforms.unitVec(vector_day2)))

			assertTrue(crossDay > 0.9R)

			''' 
			''' <summary>
			''' Here we're checking cross-vocabulary equality
			''' 
			''' </summary>
	'        
	'        Random rnd = new Random();
	'        VocabCache<VocabWord> cacheP = paragraphVectors.getVocab();
	'        VocabCache<VocabWord> cacheW = wordVectors.getVocab();
	'        for (int x = 0; x < 1000; x++) {
	'            int idx = rnd.nextInt(cacheW.numWords());
	'        
	'            String wordW = cacheW.wordAtIndex(idx);
	'            String wordP = cacheP.wordAtIndex(idx);
	'        
	'            assertEquals(wordW, wordP);
	'        
	'            INDArray arrayW = wordVectors.getWordVectorMatrix(wordW);
	'            INDArray arrayP = paragraphVectors.getWordVectorMatrix(wordP);
	'        
	'            double simWP = Transforms.cosineSim(arrayW, arrayP);
	'            assertTrue(simWP >= 0.9);
	'        }
	'        

			log.info("Zfinance: " & paragraphVectors_Conflict.getWordVectorMatrix("Zfinance"))
			log.info("Zhealth: " & paragraphVectors_Conflict.getWordVectorMatrix("Zhealth"))
			log.info("Zscience: " & paragraphVectors_Conflict.getWordVectorMatrix("Zscience"))

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(unlabeledIterator.hasNext())
			Dim document As LabelledDocument = unlabeledIterator.nextDocument()

			log.info("Results for document '" & document.Label & "'")

			Dim results As IList(Of String) = New List(Of String)(paragraphVectors_Conflict.predictSeveral(document, 3))
			For Each result As String In results
				Dim sim As Double = paragraphVectors_Conflict.similarityToLabel(document, result)
				log.info("Similarity to [" & result & "] is [" & sim & "]")
			Next result

			Dim topPrediction As String = paragraphVectors_Conflict.predict(document)
			assertEquals("Z" & document.Label, topPrediction)
		End Sub

	'    
	'        Left as reference implementation, before stuff was changed in w2v
	'     
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated private double arraysSimilarity(@NonNull INDArray array1, @NonNull INDArray array2)
		<Obsolete>
		Private Function arraysSimilarity(ByVal array1 As INDArray, ByVal array2 As INDArray) As Double
			If array1.Equals(array2) Then
				Return 1.0
			End If

			Dim vector As INDArray = Transforms.unitVec(array1)
			Dim vector2 As INDArray = Transforms.unitVec(array2)

			If vector Is Nothing OrElse vector2 Is Nothing Then
				Return -1
			End If

			Return Transforms.cosineSim(vector, vector2)

		End Function

		''' <summary>
		''' Special test to check d2v inference against pre-trained gensim model and
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testGensimEquality() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGensimEquality()

			Dim expA As INDArray = Nd4j.create(New Double() {-0.02461922, -0.00801059, -0.01821643, 0.0167951, 0.02240154, -0.00414107, -0.0022868, 0.00278438, -0.00651088, -0.02066556, -0.01045411, -0.02853066, 0.00153375, 0.02707097, -0.00754221, -0.02795872, -0.00275301, -0.01455731, -0.00981289, 0.01557207, -0.005259, 0.00355505, 0.01503531, -0.02185878, 0.0339283, -0.05049067, 0.02849454, -0.01242505, 0.00438659, -0.03037345, 0.01866657, -0.00740161, -0.01850279, 0.00851284, -0.01774663, -0.01976997, -0.03317627, 0.00372983, 0.01313218, -0.00041131, 0.00089357, -0.0156924, 0.01278253, -0.01596088, -0.01415407, -0.01795845, 0.00558284, -0.00529536, -0.03508032, 0.00725479, -0.01910841, -0.0008098, 0.00614283, -0.00926585, 0.01761538, -0.00272953, -0.01483113, 0.02062481, -0.03134528, 0.03416841, -0.0156226, -0.01418961, -0.00817538, 0.01848741, 0.00444605, 0.01090323, 0.00746163, -0.02490317, 0.00835013, 0.01091823, -0.0177979, 0.0207753, -0.00854185, 0.04269911, 0.02786852, 0.00179449, 0.00303065, -0.00127148, -0.01589409, -0.01110292, 0.01736244, -0.01177608, 0.00110929, 0.01790557, -0.01800732, 0.00903072, 0.00210271, 0.0103053, -0.01508116, 0.00336775, 0.00319031, -0.00982859, 0.02409827, -0.0079536, 0.01347831, -0.02555985, 0.00282605, 0.00350526, -0.00471707, -0.00592073, -0.01009063, -0.02396305, 0.02643895, -0.05487461, -0.01710705, -0.0082839, 0.01322765, 0.00098093, 0.01707118, 0.00290805, 0.03256396, 0.00277155, 0.00350602, 0.0096487, -0.0062662, 0.0331796, -0.01758772, 0.0295204, 0.00295053, -0.00670782, 0.02172252, 0.00172433, 0.0122977, -0.02401575, 0.01179839, -0.01646545, -0.0242724, 0.01318037, -0.00745518, -0.00400624, -0.01735787, 0.01627645, 0.04445697, -0.0189355, 0.01315041, 0.0131585, 0.01770667, -0.00114554, 0.00581599, 0.00745188, -0.01318868, -0.00801476, -0.00884938, 0.00084786, 0.02578231, -0.01312729, -0.02047793, 0.00485749, -0.00342519, -0.00744475, 0.01180929, 0.02871456, 0.01483848, -0.00696516, 0.02003011, -0.01721076, -0.0124568, -0.0114492, -0.00970469, 0.01971609, 0.01599673, -0.01426137, 0.00808409, -0.01431519, 0.01187332, 0.00144421, -0.00459554, 0.00384032, 0.00866845, 0.00265177, -0.01003456, 0.0289338, 0.00353483, -0.01664903, -0.03050662, 0.01305057, -0.0084294, -0.01615093, -0.00897918, 0.00768479, 0.02155688, 0.01594496, 0.00034328, -0.00557031, -0.00256555, 0.03939554, 0.00274235, 0.001288, 0.02933025, 0.0070212, -0.00573742, 0.00883708, 0.00829396, -0.01100356, -0.02653269, -0.01023274, 0.03079773, -0.00765917, 0.00949703, 0.01212146, -0.01362515, -0.0076843, -0.00290596, -0.01707907, 0.02899382, -0.00089925, 0.01510732, 0.02378234, -0.00947305, 0.0010998, -0.00558241, 0.00057873, 0.01098226, -0.02019168, -0.013942, -0.01639287, -0.00675588, -0.00400709, -0.02914054, -0.00433462, 0.01551765, -0.03552055, 0.01681101, -0.00629782, -0.01698086, 0.01891401, 0.03597684, 0.00888052, -0.01587857, 0.00935822, 0.00931327, -0.0128156, 0.05170929, -0.01811879, 0.02096679, 0.00897546, 0.00132624, -0.01796336, 0.01888563, -0.01142226, -0.00805926, 0.00049782, -0.02151541, 0.00747257, 0.023373, -0.00198183, 0.02968843, 0.00443042, -0.00328569, -0.04200815, 0.01306543, -0.01608924, -0.01604842, 0.03137267, 0.0266054, 0.00172526, -0.01205696, 0.00047532, 0.00321026, 0.00671424, 0.01710422, -0.01129941, 0.00268044, -0.01065434, -0.01107133, 0.00036135, -0.02991677, 0.02351665, -0.00343891, -0.01736755, -0.00100577, -0.00312481, -0.01083809, 0.00387084, 0.01136449, 0.01675043, -0.01978249, -0.00765182, 0.02746241, -0.01082247, -0.01587164, 0.01104732, -0.00878782, -0.00497555, -0.00186257, -0.02281011, 0.00141792, 0.00432851, -0.01290263, -0.00387155, 0.00802639, -0.00761913, 0.01508144, 0.02226428, 0.0107248, 0.01003709, 0.01587571, 0.00083492, -0.01632052, -0.00435973})
			Dim expB As INDArray = Nd4j.create(New Double() {-0.02465764, 0.00756337, -0.0268607, 0.01588023, 0.01580242, -0.00150542, 0.00116652, 0.0021577, -0.00754891, -0.02441176, -0.01271976, -0.02015191, 0.00220599, 0.03722657, -0.01629612, -0.02779619, -0.01157856, -0.01937938, -0.00744667, 0.01990043, -0.00505888, 0.00573646, 0.00385467, -0.0282531, 0.03484593, -0.05528606, 0.02428633, -0.01510474, 0.00153177, -0.03637344, 0.01747423, -0.00090738, -0.02199888, 0.01410434, -0.01710641, -0.01446697, -0.04225266, 0.00262217, 0.00871943, 0.00471594, 0.0101348, -0.01991908, 0.00874325, -0.00606416, -0.01035323, -0.01376545, 0.00451507, -0.01220307, -0.04361237, 0.00026028, -0.02401881, 0.00580314, 0.00238946, -0.01325974, 0.01879044, -0.00335623, -0.01631887, 0.02222102, -0.02998703, 0.03190075, -0.01675236, -0.01799807, -0.01314015, 0.01950069, 0.0011723, 0.01013178, 0.01093296, -0.034143, 0.00420227, 0.01449351, -0.00629987, 0.01652851, -0.01286825, 0.03314656, 0.03485073, 0.01120341, 0.01298241, 0.0019494, -0.02420256, -0.0063762, 0.01527091, -0.00732881, 0.0060427, 0.019327, -0.02068196, 0.00876712, 0.00292274, 0.01312969, -0.01529114, 0.0021757, -0.00565621, -0.01093122, 0.02758765, -0.01342688, 0.01606117, -0.02666447, 0.00541112, 0.00375426, -0.00761796, 0.00136015, -0.01169962, -0.03012749, 0.03012953, -0.05491332, -0.01137303, -0.01392103, 0.01370098, -0.00794501, 0.0248435, 0.00319645, 0.04261713, -0.00364211, 0.00780485, 0.01182583, -0.00647098, 0.03291231, -0.02515565, 0.03480943, 0.00119836, -0.00490694, 0.02615346, -0.00152456, 0.00196142, -0.02326461, 0.00603225, -0.02414703, -0.02540966, 0.0072112, -0.01090273, -0.00505061, -0.02196866, 0.00515245, 0.04981546, -0.02237269, -0.00189305, 0.0169786, 0.01782372, -0.00430022, 0.00551226, 0.00293861, -0.01337168, -0.00302476, -0.01869966, 0.00270757, 0.03199976, -0.01614617, -0.02716484, 0.01560035, -0.01312686, -0.01604082, 0.01347521, 0.03229654, 0.00707219, -0.00588392, 0.02444809, -0.01068742, -0.0190814, -0.00556385, -0.00462766, 0.01283929, 0.02001247, -0.00837629, -0.00041943, -0.02298774, 0.00874839, 0.00434907, -0.00963332, 0.00476905, 0.00793049, -0.00212557, -0.01839353, 0.03345517, 0.00838255, -0.0157447, -0.0376134, 0.01059611, -0.02323246, -0.01326356, -0.01116734, 0.00598869, 0.0211626, 0.01872963, -0.0038276, -0.01208279, -0.00989125, 0.04147648, 0.00181867, -0.00369355, 0.02312465, 0.0048396, 0.00564515, 0.01317832, -0.0057621, -0.01882041, -0.02869064, -0.00670661, 0.02585443, -0.01108428, 0.01411031, 0.01204507, -0.01244726, -0.00962342, -0.00205239, -0.01653971, 0.02871559, -0.00772978, 0.0214524, 0.02035478, -0.01324312, 0.00169302, -0.00064739, 0.00531795, 0.01059279, -0.02455794, -0.00002782, -0.0068906, -0.0160858, -0.0031842, -0.02295724, 0.01481094, 0.01769004, -0.02925742, 0.02050495, -0.00029003, -0.02815636, 0.02467367, 0.03419458, 0.00654938, -0.01847546, 0.00999932, 0.00059222, -0.01722176, 0.05172159, -0.01548486, 0.01746444, 0.007871, 0.0078471, -0.02414417, 0.01898077, -0.01470176, -0.00299465, 0.00368212, -0.02474656, 0.01317451, 0.03706085, -0.00032923, 0.02655881, 0.0013586, -0.0120303, -0.05030316, 0.0222294, -0.0070967, -0.02150935, 0.03254268, 0.01369857, 0.00246183, -0.02253576, -0.00551247, 0.00787363, 0.01215617, 0.02439827, -0.01104699, -0.00774596, -0.01898127, -0.01407653, 0.00195514, -0.03466602, 0.01560903, -0.01239944, -0.02474852, 0.00155114, 0.00089324, -0.01725949, -0.00011816, 0.00742845, 0.01247074, -0.02467943, -0.00679623, 0.01988366, -0.00626181, -0.02396477, 0.01052101, -0.01123178, -0.00386291, -0.00349261, -0.02714747, -0.00563315, 0.00228767, -0.01303677, -0.01971108, 0.00014759, -0.00346399, 0.02220698, 0.01979946, -0.00526076, 0.00647453, 0.01428513, 0.00223467, -0.01690172, -0.0081715})

			Dim configuration As New VectorsConfiguration()

			configuration.setIterations(5)
			configuration.setLearningRate(0.01)
			configuration.setUseHierarchicSoftmax(True)
			configuration.setNegative(0)

			Dim w2v As Word2Vec = WordVectorSerializer.readWord2VecFromText(New File("/home/raver119/Downloads/gensim_models_for_dl4j/word"), New File("/home/raver119/Downloads/gensim_models_for_dl4j/hs"), New File("/home/raver119/Downloads/gensim_models_for_dl4j/hs_code"), New File("/home/raver119/Downloads/gensim_models_for_dl4j/hs_mapping"), configuration)

			Dim tokenizerFactory As TokenizerFactory = New DefaultTokenizerFactory()
			tokenizerFactory.TokenPreProcessor = New CommonPreprocessor()


			assertNotEquals(Nothing, w2v.getLookupTable())
			assertNotEquals(Nothing, w2v.getVocab())

			Dim d2v As ParagraphVectors = (New ParagraphVectors.Builder(configuration)).useExistingWordVectors(w2v).sequenceLearningAlgorithm(New DM(Of VocabWord)()).tokenizerFactory(tokenizerFactory).resetModel(False).build()


			assertNotEquals(Nothing, d2v.getLookupTable())
			assertNotEquals(Nothing, d2v.getVocab())

			assertTrue(d2v.getVocab() = w2v.getVocab())
			assertTrue(d2v.getLookupTable() = w2v.getLookupTable())

			Dim textA As String = "Donald Trump referred to President Obama as “your president” during the first presidential debate on Monday, much to many people’s chagrin on social media. Trump, made the reference after saying that the greatest threat facing the world is nuclear weapons. He then turned to Hillary Clinton and said, “Not global warming like you think and your President thinks,” referring to Obama."

			Dim textB As String = "The comment followed Trump doubling down on his false claims about the so-called birther conspiracy theory about Obama. People following the debate were immediately angered that Trump implied Obama is not his president."

			Dim textC As String = "practice of trust owned Trump for example indeed and conspiracy between provoke"

			Dim arrayA As INDArray = d2v.inferVector(textA)
			Dim arrayB As INDArray = d2v.inferVector(textB)
			Dim arrayC As INDArray = d2v.inferVector(textC)

			assertNotEquals(Nothing, arrayA)
			assertNotEquals(Nothing, arrayB)

			Transforms.unitVec(arrayA)
			Transforms.unitVec(arrayB)

			Transforms.unitVec(expA)
			Transforms.unitVec(expB)

			Dim simX As Double = Transforms.cosineSim(arrayA, arrayB)
			Dim simC As Double = Transforms.cosineSim(arrayA, arrayC)
			Dim simB As Double = Transforms.cosineSim(arrayB, expB)

			log.info("SimilarityX: {}", simX)
			log.info("SimilarityC: {}", simC)
			log.info("SimilarityB: {}", simB)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testDirectInference(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDirectInference(ByVal testDir As Path)
			Dim isIntegration As Boolean = IntegrationTests
			Dim resource As File = Resources.asFile("/big/raw_sentences.txt")
			Dim sentencesIter As SentenceIterator = getIterator(isIntegration, resource)

			Dim resource_mixed As New ClassPathResource("paravec/")
			Dim local_resource_mixed As File = testDir.toFile()
			resource_mixed.copyDirectory(local_resource_mixed)
			Dim iter As SentenceIterator = (New AggregatingSentenceIterator.Builder()).addSentenceIterator(sentencesIter).addSentenceIterator(New FileSentenceIterator(local_resource_mixed)).build()

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim wordVectors As Word2Vec = (New Word2Vec.Builder()).minWordFrequency(1).batchSize(250).iterations(1).epochs(1).learningRate(0.025).layerSize(150).minLearningRate(0.001).elementsLearningAlgorithm(New SkipGram(Of VocabWord)()).useHierarchicSoftmax(True).windowSize(5).iterate(iter).tokenizerFactory(t).build()

			wordVectors.fit()

			Dim pv As ParagraphVectors = (New ParagraphVectors.Builder()).tokenizerFactory(t).iterations(10).useHierarchicSoftmax(True).trainWordVectors(True).useExistingWordVectors(wordVectors).negativeSample(0).sequenceLearningAlgorithm(New DM(Of VocabWord)()).build()

			Dim vec1 As INDArray = pv.inferVector("This text is pretty awesome")
			Dim vec2 As INDArray = pv.inferVector("Fantastic process of crazy things happening inside just for history purposes")

			log.info("vec1/vec2: {}", Transforms.cosineSim(vec1, vec2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testGoogleModelForInference() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testGoogleModelForInference()
			Dim googleVectors As WordVectors = WordVectorSerializer.readWord2VecModel(New File("/ext/GoogleNews-vectors-negative300.bin.gz"))

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim pv As ParagraphVectors = (New ParagraphVectors.Builder()).tokenizerFactory(t).iterations(10).useHierarchicSoftmax(False).trainWordVectors(False).iterations(10).useExistingWordVectors(googleVectors).negativeSample(10).sequenceLearningAlgorithm(New DM(Of VocabWord)()).build()

			Dim vec1 As INDArray = pv.inferVector("This text is pretty awesome")
			Dim vec2 As INDArray = pv.inferVector("Fantastic process of crazy things happening inside just for history purposes")

			log.info("vec1/vec2: {}", Transforms.cosineSim(vec1, vec2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testHash()
		Public Overridable Sub testHash()
			Dim w1 As New VocabWord(1.0, "D1")
			Dim w2 As New VocabWord(1.0, "Bo")



			log.info("W1 > Short hash: {}; Long hash: {}", w1.Label.GetHashCode(), w1.getStorageId())
			log.info("W2 > Short hash: {}; Long hash: {}", w2.Label.GetHashCode(), w2.getStorageId())

			assertNotEquals(w1.getStorageId(), w2.getStorageId())
		End Sub


		''' <summary>
		''' This is very long test, to track memory consumption over time
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Disabled("Takes too long for CI") @Tag(org.nd4j.common.tests.tags.TagNames.NEEDS_VERIFY) public void testsParallelFit1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testsParallelFit1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final File file = org.nd4j.common.resources.Resources.asFile("big/raw_sentences.txt");
			Dim file As File = Resources.asFile("big/raw_sentences.txt")

			For i As Integer = 0 To 999
				Dim threads As IList(Of Thread) = New List(Of Thread)()
				For t As Integer = 0 To 2
					threads.Add(New Thread(Sub()
					Try
						Dim t1 As TokenizerFactory = New DefaultTokenizerFactory()
						Dim source As New LabelsSource("DOC_")
						Dim sic As New SentenceIteratorConverter(New BasicLineIterator(file), source)
						Dim vec As ParagraphVectors = (New ParagraphVectors.Builder()).seed(42).minWordFrequency(1).iterations(1).epochs(5).layerSize(100).learningRate(0.05).windowSize(5).trainWordVectors(True).allowParallelTokenization(False).tokenizerFactory(t1).workers(1).iterate(sic).build()
						vec.fit()
					Catch e As Exception
						Throw New Exception(e)
					End Try
					End Sub))
				Next t

				For Each t As Thread In threads
					t.Start()
				Next t

				For Each t As Thread In threads
					t.Join()
				Next t
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testJSONSerialization()
		Public Overridable Sub testJSONSerialization()
'JAVA TO VB CONVERTER NOTE: The variable paragraphVectors was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim paragraphVectors_Conflict As ParagraphVectors = (New ParagraphVectors.Builder()).build()
			Dim cache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim words As val = New VocabWord(2){}
			words(0) = New VocabWord(1.0, "word")
			words(1) = New VocabWord(2.0, "test")
			words(2) = New VocabWord(3.0, "tester")

			For i As Integer = 0 To words.length - 1
				cache.addToken(words(i))
				cache.addWordToIndex(i, words(i).getLabel())
			Next i
			paragraphVectors_Conflict.Vocab = cache

			Dim json As String = Nothing
			Dim unserialized As Word2Vec = Nothing
			Try
				json = paragraphVectors_Conflict.toJson()
				log.info("{}", json.ToString())

				unserialized = ParagraphVectors.fromJson(json)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try

			assertEquals(cache.totalWordOccurrences(), DirectCast(unserialized, ParagraphVectors).getVocab().totalWordOccurrences())
			assertEquals(cache.totalNumberOfDocs(), DirectCast(unserialized, ParagraphVectors).getVocab().totalNumberOfDocs())

			For i As Integer = 0 To words.length - 1
				Dim cached As val = cache.wordAtIndex(i)
				Dim restored As val = DirectCast(unserialized, ParagraphVectors).getVocab().wordAtIndex(i)
				assertNotNull(cached)
				assertEquals(cached, restored)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testDoubleFit() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDoubleFit()
			Dim isIntegration As Boolean = IntegrationTests
			Dim resource As File = Resources.asFile("/big/raw_sentences.txt")
			Dim iter As SentenceIterator = getIterator(isIntegration, resource)


			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim source As New LabelsSource("DOC_")

			Dim builder As val = New ParagraphVectors.Builder()
			Dim vec As ParagraphVectors = builder.minWordFrequency(1).iterations(5).seed(119).epochs(1).layerSize(150).learningRate(0.025).labelsSource(source).windowSize(5).sequenceLearningAlgorithm(New DM(Of VocabWord)()).iterate(iter).trainWordVectors(True).usePreciseWeightInit(True).batchSize(8192).allowParallelTokenization(False).tokenizerFactory(t).workers(1).sampling(0).build()

			vec.fit()
			Dim num1 As Long = vec.vocab().totalNumberOfDocs()

			vec.fit()
			Console.WriteLine(vec.vocab().totalNumberOfDocs())
			Dim num2 As Long = vec.vocab().totalNumberOfDocs()

			assertEquals(num1, num2)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static SentenceIterator getIterator(boolean isIntegration, File file) throws IOException
		Public Shared Function getIterator(ByVal isIntegration As Boolean, ByVal file As File) As SentenceIterator
			Return getIterator(isIntegration, file, 500)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static SentenceIterator getIterator(boolean isIntegration, File file, int linesForUnitTest) throws IOException
		Public Shared Function getIterator(ByVal isIntegration As Boolean, ByVal file As File, ByVal linesForUnitTest As Integer) As SentenceIterator
			If isIntegration Then
				Return New BasicLineIterator(file)
			Else
				Dim lines As IList(Of String) = New List(Of String)()
				Using [is] As Stream = New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
					Dim lineIter As LineIterator = IOUtils.lineIterator([is], StandardCharsets.UTF_8)
					Try
						Dim i As Integer=0
						Do While i<linesForUnitTest AndAlso lineIter.hasNext()
							lines.Add(lineIter.next())
							i += 1
						Loop
					Finally
						lineIter.close()
					End Try
				End Using

				Return New CollectionSentenceIterator(lines)
			End If
		End Function
	End Class



End Namespace