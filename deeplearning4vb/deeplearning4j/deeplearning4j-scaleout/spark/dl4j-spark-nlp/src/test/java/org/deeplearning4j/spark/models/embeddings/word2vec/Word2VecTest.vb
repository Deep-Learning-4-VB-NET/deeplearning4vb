Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports DL4JResources = org.deeplearning4j.common.resources.DL4JResources
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports Tag = org.junit.jupiter.api.Tag
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.deeplearning4j.models.embeddings.inmemory
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports CommonPreprocessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor
Imports LowCasePreProcessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.LowCasePreProcessor
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports Downloader = org.nd4j.common.resources.Downloader
Imports StrumpfResolver = org.nd4j.common.resources.strumpf.StrumpfResolver
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.spark.models.embeddings.word2vec



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Slf4j @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) @Disabled("Permissions issues on CI") @Tag(TagNames.NEEDS_VERIFY) public class Word2VecTest
	Public Class Word2VecTest
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
'ORIGINAL LINE: @Test public void testConcepts(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConcepts(ByVal testDir As Path)
			' These are all default values for word2vec
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[8]").set("spark.driver.host", "localhost").setAppName("sparktest")

			' Set SparkContext
			Dim sc As New JavaSparkContext(sparkConf)

			' Path of data part-00000
			Dim dataPath As String = (New ClassPathResource("big/raw_sentences.txt")).File.getAbsolutePath()
			'        dataPath = "/ext/Temp/part-00000";
			'        String dataPath = new ClassPathResource("spark_word2vec_test.txt").getFile().getAbsolutePath();

			' Read in data
			Dim corpus As JavaRDD(Of String) = sc.textFile(dataPath)

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim word2Vec As Word2Vec = (New Word2Vec.Builder()).setNGrams(1).tokenizerFactory(t).seed(42L).negative(10).useAdaGrad(False).layerSize(150).windowSize(5).learningRate(0.025).minLearningRate(0.0001).iterations(1).batchSize(100).minWordFrequency(5).stopWords(Arrays.asList("three")).useUnknown(True).build()

			word2Vec.train(corpus)

			'word2Vec.setModelUtils(new FlatModelUtils());

			Console.WriteLine("UNK: " & word2Vec.getWordVectorMatrix("UNK"))

			Dim table As InMemoryLookupTable(Of VocabWord) = CType(word2Vec.lookupTable(), InMemoryLookupTable(Of VocabWord))

			Dim sim As Double = word2Vec.similarity("day", "night")
			Console.WriteLine("day/night similarity: " & sim)
	'        
	'        System.out.println("Hornjo: " + word2Vec.getWordVectorMatrix("hornjoserbsce"));
	'        System.out.println("carro: " + word2Vec.getWordVectorMatrix("carro"));
	'        
	'        Collection<String> portu = word2Vec.wordsNearest("carro", 10);
	'        printWords("carro", portu, word2Vec);
	'        
	'        portu = word2Vec.wordsNearest("davi", 10);
	'        printWords("davi", portu, word2Vec);
	'        
	'        System.out.println("---------------------------------------");
	'        

			Dim words As ICollection(Of String) = word2Vec.wordsNearest("day", 10)
			printWords("day", words, word2Vec)

			assertTrue(words.Contains("night"))
			assertTrue(words.Contains("week"))
			assertTrue(words.Contains("year"))

			sim = word2Vec.similarity("two", "four")
			Console.WriteLine("two/four similarity: " & sim)

			words = word2Vec.wordsNearest("two", 10)
			printWords("two", words, word2Vec)

			' three should be absent due to stopWords
			assertFalse(words.Contains("three"))

			assertTrue(words.Contains("five"))
			assertTrue(words.Contains("four"))

			sc.stop()


			' test serialization

			Dim tempFile As File = Files.createTempFile(testDir,"temp" & DateTimeHelper.CurrentUnixTimeMillis(),"tmp").toFile()

			Dim idx1 As Integer = word2Vec.vocab().wordFor("day").getIndex()

			Dim array1 As INDArray = word2Vec.getWordVectorMatrix("day").dup()

			Dim word1 As VocabWord = word2Vec.vocab().elementAtIndex(0)

			WordVectorSerializer.writeWordVectors(word2Vec.getLookupTable(), tempFile)

			Dim vectors As WordVectors = WordVectorSerializer.loadTxtVectors(tempFile)

			Dim word2 As VocabWord = CType(vectors.vocab(), VocabCache(Of VocabWord)).elementAtIndex(0)
			Dim wordIT As VocabWord = CType(vectors.vocab(), VocabCache(Of VocabWord)).wordFor("it")
			Dim idx2 As Integer = vectors.vocab().wordFor("day").Index

			Dim array2 As INDArray = vectors.getWordVectorMatrix("day").dup()

			Console.WriteLine("word 'i': " & word2)
			Console.WriteLine("word 'it': " & wordIT)

			assertEquals(idx1, idx2)
			assertEquals(word1, word2)
			assertEquals(array1, array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Test public void testSparkW2VonBiggerCorpus() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSparkW2VonBiggerCorpus()
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[8]").setAppName("sparktest").set("spark.driver.host", "localhost").set("spark.driver.maxResultSize", "4g").set("spark.driver.memory", "8g").set("spark.executor.memory", "8g")

			' Set SparkContext
			Dim sc As New JavaSparkContext(sparkConf)

			' Path of data part-00000
			'String dataPath = Resources.asFile("big/raw_sentences.txt").getAbsolutePath();
			'        String dataPath = "/ext/Temp/SampleRussianCorpus.txt";
			Dim dataPath As String = (New ClassPathResource("spark_word2vec_test.txt")).File.getAbsolutePath()

			' Read in data
			Dim corpus As JavaRDD(Of String) = sc.textFile(dataPath)

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New LowCasePreProcessor()

			Dim word2Vec As Word2Vec = (New Word2Vec.Builder()).setNGrams(1).tokenizerFactory(t).seed(42L).negative(3).useAdaGrad(False).layerSize(100).windowSize(5).learningRate(0.025).minLearningRate(0.0001).iterations(1).batchSize(100).minWordFrequency(5).useUnknown(True).build()

			word2Vec.train(corpus)


			sc.stop()

			WordVectorSerializer.writeWordVectors(word2Vec.getLookupTable(), "/ext/Temp/sparkRuModel.txt")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testPortugeseW2V() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testPortugeseW2V()
			Dim word2Vec As WordVectors = WordVectorSerializer.loadTxtVectors(New File("/ext/Temp/para.txt"))
			word2Vec.ModelUtils = New FlatModelUtils()

			Dim portu As ICollection(Of String) = word2Vec.wordsNearest("carro", 10)
			printWords("carro", portu, word2Vec)

			portu = word2Vec.wordsNearest("davi", 10)
			printWords("davi", portu, word2Vec)
		End Sub

		Private Shared Sub printWords(ByVal target As String, ByVal list As ICollection(Of String), ByVal vec As WordVectors)
			Console.WriteLine("Words close to [" & target & "]:")
			For Each word As String In list
				Dim sim As Double = vec.similarity(target, word)
				Console.Write("'" & word & "': [" & sim & "], ")
			Next word
			Console.Write(vbLf)
		End Sub
	End Class

End Namespace