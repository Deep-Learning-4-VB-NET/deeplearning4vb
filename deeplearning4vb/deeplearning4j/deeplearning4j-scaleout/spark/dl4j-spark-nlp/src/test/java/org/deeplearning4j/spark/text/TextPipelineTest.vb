Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports Huffman = org.deeplearning4j.models.word2vec.Huffman
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports FirstIterationFunction = org.deeplearning4j.spark.models.embeddings.word2vec.FirstIterationFunction
Imports MapToPairFunction = org.deeplearning4j.spark.models.embeddings.word2vec.MapToPairFunction
Imports Word2Vec = org.deeplearning4j.spark.models.embeddings.word2vec.Word2Vec
Imports CountCumSum = org.deeplearning4j.spark.text.functions.CountCumSum
Imports TextPipeline = org.deeplearning4j.spark.text.functions.TextPipeline
Imports StopWords = org.deeplearning4j.text.stopwords.StopWords
Imports org.junit.jupiter.api
Imports Downloader = org.nd4j.common.resources.Downloader
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports Tuple2 = scala.Tuple2
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

Namespace org.deeplearning4j.spark.text


	''' <summary>
	''' @author Jeffrey Tang
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TextPipelineTest extends BaseSparkTest
	<Serializable>
	Public Class TextPipelineTest
		Inherits BaseSparkTest

		Private sentenceList As IList(Of String)
		Private conf As SparkConf
		Private word2vec As Word2Vec
		Private word2vecNoStop As Word2Vec

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(TextPipeline))

		Public Overridable Function getCorpusRDD(ByVal sc As JavaSparkContext) As JavaRDD(Of String)
			Return sc.parallelize(sentenceList, 2)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll @SneakyThrows public static void beforeAll()
		Public Shared Sub beforeAll()
			If Platform.isWindows() Then
				Dim hadoopHome As New File(System.getProperty("java.io.tmpdir"),"hadoop-tmp")
				Dim binDir As New File(hadoopHome,"bin")
				If Not binDir.exists() Then
					binDir.mkdirs()
				End If
				Dim outputFile As New File(binDir,"winutils.exe")
				If Not outputFile.exists() Then
					log.info("Fixing spark for windows")
					Downloader.download("winutils.exe", URI.create("https://github.com/cdarlint/winutils/blob/master/hadoop-2.6.5/bin/winutils.exe?raw=true").toURL(), outputFile,"db24b404d2331a1bec7443336a5171f1",3)
				End If

				System.setProperty("hadoop.home.dir", hadoopHome.getAbsolutePath())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overrides Sub before()
			conf = (New SparkConf()).setMaster("local[4]").setAppName("sparktest").set("spark.driver.host", "localhost")

			' All the avaliable options. These are default values
			word2vec = (New Word2Vec.Builder()).minWordFrequency(1).setNGrams(1).tokenizerFactory("org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory").tokenPreprocessor("org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor").stopWords(StopWords.getStopWords()).seed(42L).negative(0).useAdaGrad(False).layerSize(100).windowSize(5).learningRate(0.025).minLearningRate(0.0001).iterations(1).build()

			word2vecNoStop = (New Word2Vec.Builder()).minWordFrequency(1).setNGrams(1).tokenizerFactory("org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory").tokenPreprocessor("org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor").seed(42L).negative(0).useAdaGrad(False).layerSize(100).windowSize(5).learningRate(0.025).minLearningRate(0.0001).iterations(1).build()

			sentenceList = New List(Of String) From {"This is a strange strange world.", "Flowers are red."}
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTokenizer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testTokenizer()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			Dim tokenizedRDD As JavaRDD(Of IList(Of String)) = pipeline.tokenize()

			assertEquals(2, tokenizedRDD.count())

			assertEquals(java.util.Arrays.asList("this", "is", "a", "strange", "strange", "world"), tokenizedRDD.first())

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWordFreqAccIdentifyStopWords() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWordFreqAccIdentifyStopWords()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			Dim tokenizedRDD As JavaRDD(Of IList(Of String)) = pipeline.tokenize()
			Dim sentenceWordsCountRDD As JavaRDD(Of Pair(Of IList(Of String), AtomicLong)) = pipeline.updateAndReturnAccumulatorVal(tokenizedRDD)

			Dim wordFreqCounter As Counter(Of String) = pipeline.getWordFreqAcc().value()
			assertEquals(wordFreqCounter.getCount("STOP"), 4, 0)
			assertEquals(wordFreqCounter.getCount("strange"), 2, 0)
			assertEquals(wordFreqCounter.getCount("flowers"), 1, 0)
			assertEquals(wordFreqCounter.getCount("world"), 1, 0)
			assertEquals(wordFreqCounter.getCount("red"), 1, 0)

			Dim ret As IList(Of Pair(Of IList(Of String), AtomicLong)) = sentenceWordsCountRDD.collect()
			assertEquals(ret(0).getFirst(), java.util.Arrays.asList("this", "is", "a", "strange", "strange", "world"))
			assertEquals(ret(1).getFirst(), java.util.Arrays.asList("flowers", "are", "red"))
			assertEquals(ret(0).getSecond().get(), 6)
			assertEquals(ret(1).getSecond().get(), 3)


			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWordFreqAccNotIdentifyingStopWords() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWordFreqAccNotIdentifyingStopWords()

			Dim sc As JavaSparkContext = Context
			'  word2vec.setRemoveStop(false);
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vecNoStop.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			Dim tokenizedRDD As JavaRDD(Of IList(Of String)) = pipeline.tokenize()
			pipeline.updateAndReturnAccumulatorVal(tokenizedRDD)

			Dim wordFreqCounter As Counter(Of String) = pipeline.getWordFreqAcc().value()
			assertEquals(wordFreqCounter.getCount("is"), 1, 0)
			assertEquals(wordFreqCounter.getCount("this"), 1, 0)
			assertEquals(wordFreqCounter.getCount("are"), 1, 0)
			assertEquals(wordFreqCounter.getCount("a"), 1, 0)
			assertEquals(wordFreqCounter.getCount("strange"), 2, 0)
			assertEquals(wordFreqCounter.getCount("flowers"), 1, 0)
			assertEquals(wordFreqCounter.getCount("world"), 1, 0)
			assertEquals(wordFreqCounter.getCount("red"), 1, 0)

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWordFreqAccIdentifyingStopWords() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWordFreqAccIdentifyingStopWords()

			Dim sc As JavaSparkContext = Context
			'  word2vec.setRemoveStop(false);
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			Dim tokenizedRDD As JavaRDD(Of IList(Of String)) = pipeline.tokenize()
			pipeline.updateAndReturnAccumulatorVal(tokenizedRDD)

			Dim wordFreqCounter As Counter(Of String) = pipeline.getWordFreqAcc().value()
			assertEquals(wordFreqCounter.getCount("is"), 0, 0)
			assertEquals(wordFreqCounter.getCount("this"), 0, 0)
			assertEquals(wordFreqCounter.getCount("are"), 0, 0)
			assertEquals(wordFreqCounter.getCount("a"), 0, 0)
			assertEquals(wordFreqCounter.getCount("STOP"), 4, 0)
			assertEquals(wordFreqCounter.getCount("strange"), 2, 0)
			assertEquals(wordFreqCounter.getCount("flowers"), 1, 0)
			assertEquals(wordFreqCounter.getCount("world"), 1, 0)
			assertEquals(wordFreqCounter.getCount("red"), 1, 0)

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFilterMinWordAddVocab() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFilterMinWordAddVocab()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			Dim tokenizedRDD As JavaRDD(Of IList(Of String)) = pipeline.tokenize()
			pipeline.updateAndReturnAccumulatorVal(tokenizedRDD)
			Dim wordFreqCounter As Counter(Of String) = pipeline.getWordFreqAcc().value()

			pipeline.filterMinWordAddVocab(wordFreqCounter)
			Dim vocabCache As VocabCache(Of VocabWord) = pipeline.getVocabCache()

			assertTrue(vocabCache IsNot Nothing)

			Dim redVocab As VocabWord = vocabCache.tokenFor("red")
			Dim flowerVocab As VocabWord = vocabCache.tokenFor("flowers")
			Dim worldVocab As VocabWord = vocabCache.tokenFor("world")
			Dim strangeVocab As VocabWord = vocabCache.tokenFor("strange")


			assertEquals(redVocab.Word, "red")
			assertEquals(redVocab.getElementFrequency(), 1, 0)

			assertEquals(flowerVocab.Word, "flowers")
			assertEquals(flowerVocab.getElementFrequency(), 1, 0)

			assertEquals(worldVocab.Word, "world")
			assertEquals(worldVocab.getElementFrequency(), 1, 0)

			assertEquals(strangeVocab.Word, "strange")
			assertEquals(strangeVocab.getElementFrequency(), 2, 0)

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBuildVocabCache() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBuildVocabCache()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			Dim vocabCache As VocabCache(Of VocabWord) = pipeline.getVocabCache()

			assertTrue(vocabCache IsNot Nothing)

			log.info("VocabWords: " & vocabCache.words())
			assertEquals(5, vocabCache.numWords())


			Dim redVocab As VocabWord = vocabCache.tokenFor("red")
			Dim flowerVocab As VocabWord = vocabCache.tokenFor("flowers")
			Dim worldVocab As VocabWord = vocabCache.tokenFor("world")
			Dim strangeVocab As VocabWord = vocabCache.tokenFor("strange")

			log.info("Red word: " & redVocab)
			log.info("Flower word: " & flowerVocab)
			log.info("World word: " & worldVocab)
			log.info("Strange word: " & strangeVocab)

			assertEquals(redVocab.Word, "red")
			assertEquals(redVocab.getElementFrequency(), 1, 0)

			assertEquals(flowerVocab.Word, "flowers")
			assertEquals(flowerVocab.getElementFrequency(), 1, 0)

			assertEquals(worldVocab.Word, "world")
			assertEquals(worldVocab.getElementFrequency(), 1, 0)

			assertEquals(strangeVocab.Word, "strange")
			assertEquals(strangeVocab.getElementFrequency(), 2, 0)

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBuildVocabWordListRDD() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBuildVocabWordListRDD()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			pipeline.buildVocabWordListRDD()
			Dim sentenceCountRDD As JavaRDD(Of AtomicLong) = pipeline.getSentenceCountRDD()
			Dim vocabWordListRDD As JavaRDD(Of IList(Of VocabWord)) = pipeline.getVocabWordListRDD()
			Dim vocabWordList As IList(Of IList(Of VocabWord)) = vocabWordListRDD.collect()
			Dim firstSentenceVocabList As IList(Of VocabWord) = vocabWordList(0)
			Dim secondSentenceVocabList As IList(Of VocabWord) = vocabWordList(1)

			Console.WriteLine(java.util.Arrays.deepToString(firstSentenceVocabList.ToArray()))

			Dim firstSentenceTokenList As IList(Of String) = New List(Of String)()
			Dim secondSentenceTokenList As IList(Of String) = New List(Of String)()
			For Each v As VocabWord In firstSentenceVocabList
				If v IsNot Nothing Then
					firstSentenceTokenList.Add(v.Word)
				End If
			Next v
			For Each v As VocabWord In secondSentenceVocabList
				If v IsNot Nothing Then
					secondSentenceTokenList.Add(v.Word)
				End If
			Next v

			assertEquals(pipeline.getTotalWordCount(), 9, 0)
			assertEquals(sentenceCountRDD.collect().get(0).get(), 6)
			assertEquals(sentenceCountRDD.collect().get(1).get(), 3)
			assertTrue(firstSentenceTokenList.ContainsAll(java.util.Arrays.asList("strange", "strange", "world")))
			assertTrue(secondSentenceTokenList.ContainsAll(java.util.Arrays.asList("flowers", "red")))

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testHuffman() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testHuffman()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()

			Dim vocabCache As VocabCache(Of VocabWord) = pipeline.getVocabCache()

			Dim huffman As New Huffman(vocabCache.vocabWords())
			huffman.build()
			huffman.applyIndexes(vocabCache)

			Dim vocabWords As ICollection(Of VocabWord) = vocabCache.vocabWords()
			Console.WriteLine("Huffman Test:")
			For Each vocabWord As VocabWord In vocabWords
				Console.WriteLine("Word: " & vocabWord)
				Console.WriteLine(vocabWord.getCodes())
				Console.WriteLine(vocabWord.getPoints())
			Next vocabWord

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCountCumSum() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCountCumSum()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			pipeline.buildVocabWordListRDD()
			Dim sentenceCountRDD As JavaRDD(Of AtomicLong) = pipeline.getSentenceCountRDD()

			Dim countCumSum As New CountCumSum(sentenceCountRDD)
			Dim sentenceCountCumSumRDD As JavaRDD(Of Long) = countCumSum.buildCumSum()
			Dim sentenceCountCumSumList As IList(Of Long) = sentenceCountCumSumRDD.collect()
			assertTrue(sentenceCountCumSumList(0) = 6L)
			assertTrue(sentenceCountCumSumList(1) = 9L)

			sc.stop()
		End Sub

		''' <summary>
		''' This test checked generations retrieved using stopWords
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testZipFunction1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testZipFunction1()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			'  word2vec.setRemoveStop(false);
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			pipeline.buildVocabWordListRDD()
			Dim sentenceCountRDD As JavaRDD(Of AtomicLong) = pipeline.getSentenceCountRDD()
			Dim vocabWordListRDD As JavaRDD(Of IList(Of VocabWord)) = pipeline.getVocabWordListRDD()

			Dim countCumSum As New CountCumSum(sentenceCountRDD)
			Dim sentenceCountCumSumRDD As JavaRDD(Of Long) = countCumSum.buildCumSum()

			Dim vocabWordListSentenceCumSumRDD As JavaPairRDD(Of IList(Of VocabWord), Long) = vocabWordListRDD.zip(sentenceCountCumSumRDD)
			Dim lst As IList(Of Tuple2(Of IList(Of VocabWord), Long)) = vocabWordListSentenceCumSumRDD.collect()

			Dim vocabWordsList1 As IList(Of VocabWord) = lst(0)._1()
			Dim cumSumSize1 As Long? = lst(0)._2()
			assertEquals(3, vocabWordsList1.Count)
			assertEquals(vocabWordsList1(0).getWord(), "strange")
			assertEquals(vocabWordsList1(1).getWord(), "strange")
			assertEquals(vocabWordsList1(2).getWord(), "world")
			assertEquals(cumSumSize1, 6L, 0)

			Dim vocabWordsList2 As IList(Of VocabWord) = lst(1)._1()
			Dim cumSumSize2 As Long? = lst(1)._2()
			assertEquals(2, vocabWordsList2.Count)
			assertEquals(vocabWordsList2(0).getWord(), "flowers")
			assertEquals(vocabWordsList2(1).getWord(), "red")
			assertEquals(cumSumSize2, 9L, 0)

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testZipFunction2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testZipFunction2()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			'  word2vec.setRemoveStop(false);
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vecNoStop.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			pipeline.buildVocabWordListRDD()
			Dim sentenceCountRDD As JavaRDD(Of AtomicLong) = pipeline.getSentenceCountRDD()
			Dim vocabWordListRDD As JavaRDD(Of IList(Of VocabWord)) = pipeline.getVocabWordListRDD()

			Dim countCumSum As New CountCumSum(sentenceCountRDD)
			Dim sentenceCountCumSumRDD As JavaRDD(Of Long) = countCumSum.buildCumSum()

			Dim vocabWordListSentenceCumSumRDD As JavaPairRDD(Of IList(Of VocabWord), Long) = vocabWordListRDD.zip(sentenceCountCumSumRDD)
			Dim lst As IList(Of Tuple2(Of IList(Of VocabWord), Long)) = vocabWordListSentenceCumSumRDD.collect()

			Dim vocabWordsList1 As IList(Of VocabWord) = lst(0)._1()
			Dim cumSumSize1 As Long? = lst(0)._2()
			assertEquals(6, vocabWordsList1.Count)
			assertEquals(vocabWordsList1(0).getWord(), "this")
			assertEquals(vocabWordsList1(1).getWord(), "is")
			assertEquals(vocabWordsList1(2).getWord(), "a")
			assertEquals(vocabWordsList1(3).getWord(), "strange")
			assertEquals(vocabWordsList1(4).getWord(), "strange")
			assertEquals(vocabWordsList1(5).getWord(), "world")
			assertEquals(cumSumSize1, 6L, 0)

			Dim vocabWordsList2 As IList(Of VocabWord) = lst(1)._1()
			Dim cumSumSize2 As Long? = lst(1)._2()
			assertEquals(vocabWordsList2.Count, 3)
			assertEquals(vocabWordsList2(0).getWord(), "flowers")
			assertEquals(vocabWordsList2(1).getWord(), "are")
			assertEquals(vocabWordsList2(2).getWord(), "red")
			assertEquals(cumSumSize2, 9L, 0)

			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFirstIteration() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFirstIteration()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			' word2vec.setRemoveStop(false);
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			pipeline.buildVocabWordListRDD()
			Dim vocabCache As VocabCache(Of VocabWord) = pipeline.getVocabCache()
	'                Huffman huffman = new Huffman(vocabCache.vocabWords());
	'        huffman.build();
	'        huffman.applyIndexes(vocabCache);
	'        
			Dim token As VocabWord = vocabCache.tokenFor("strange")
			Dim word As VocabWord = vocabCache.wordFor("strange")
			log.info("Strange token: " & token)
			log.info("Strange word: " & word)

			' Get total word count and put into word2vec variable map
			Dim word2vecVarMap As IDictionary(Of String, Object) = word2vec.getWord2vecVarMap()
			word2vecVarMap("totalWordCount") = pipeline.getTotalWordCount()
			Dim expTable() As Double = word2vec.getExpTable()

			Dim sentenceCountRDD As JavaRDD(Of AtomicLong) = pipeline.getSentenceCountRDD()
			Dim vocabWordListRDD As JavaRDD(Of IList(Of VocabWord)) = pipeline.getVocabWordListRDD()

			Dim countCumSum As New CountCumSum(sentenceCountRDD)
			Dim sentenceCountCumSumRDD As JavaRDD(Of Long) = countCumSum.buildCumSum()

			Dim vocabWordListSentenceCumSumRDD As JavaPairRDD(Of IList(Of VocabWord), Long) = vocabWordListRDD.zip(sentenceCountCumSumRDD)

			Dim word2vecVarMapBroadcast As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vecVarMap)
			Dim expTableBroadcast As Broadcast(Of Double()) = sc.broadcast(expTable)

			Dim iterator As IEnumerator(Of Tuple2(Of IList(Of VocabWord), Long)) = vocabWordListSentenceCumSumRDD.collect().GetEnumerator()

			Dim firstIterationFunction As New FirstIterationFunction(word2vecVarMapBroadcast, expTableBroadcast, pipeline.getBroadCastVocabCache())

			Dim ret As IEnumerator(Of KeyValuePair(Of VocabWord, INDArray)) = firstIterationFunction.call(iterator)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			assertTrue(ret.hasNext())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSyn0AfterFirstIteration() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSyn0AfterFirstIteration()
			Dim sc As JavaSparkContext = Context
			Dim corpusRDD As JavaRDD(Of String) = getCorpusRDD(sc)
			'  word2vec.setRemoveStop(false);
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vec.getTokenizerVarMap())

			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			pipeline.buildVocabWordListRDD()
			Dim vocabCache As VocabCache(Of VocabWord) = pipeline.getVocabCache()
			Dim huffman As New Huffman(vocabCache.vocabWords())
			huffman.build()

			' Get total word count and put into word2vec variable map
			Dim word2vecVarMap As IDictionary(Of String, Object) = word2vec.getWord2vecVarMap()
			word2vecVarMap("totalWordCount") = pipeline.getTotalWordCount()
			Dim expTable() As Double = word2vec.getExpTable()

			Dim sentenceCountRDD As JavaRDD(Of AtomicLong) = pipeline.getSentenceCountRDD()
			Dim vocabWordListRDD As JavaRDD(Of IList(Of VocabWord)) = pipeline.getVocabWordListRDD()

			Dim countCumSum As New CountCumSum(sentenceCountRDD)
			Dim sentenceCountCumSumRDD As JavaRDD(Of Long) = countCumSum.buildCumSum()

			Dim vocabWordListSentenceCumSumRDD As JavaPairRDD(Of IList(Of VocabWord), Long) = vocabWordListRDD.zip(sentenceCountCumSumRDD)

			Dim word2vecVarMapBroadcast As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vecVarMap)
			Dim expTableBroadcast As Broadcast(Of Double()) = sc.broadcast(expTable)

			Dim firstIterationFunction As New FirstIterationFunction(word2vecVarMapBroadcast, expTableBroadcast, pipeline.getBroadCastVocabCache())
			Dim pointSyn0Vec As JavaRDD(Of Pair(Of VocabWord, INDArray)) = vocabWordListSentenceCumSumRDD.mapPartitions(firstIterationFunction).map(New MapToPairFunction())
		End Sub

	End Class


End Namespace