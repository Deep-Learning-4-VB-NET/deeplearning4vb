Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports SparkSkipGram = org.deeplearning4j.spark.models.sequencevectors.learning.elements.SparkSkipGram
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports org.junit.jupiter.api
Imports Downloader = org.nd4j.common.resources.Downloader
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
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

Namespace org.deeplearning4j.spark.models.word2vec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Slf4j public class SparkWord2VecTest extends org.deeplearning4j.BaseDL4JTest
	Public Class SparkWord2VecTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000L
			End Get
		End Property

		Private Shared sentences As IList(Of String)
		Private sc As JavaSparkContext


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
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			If sentences Is Nothing Then
				sentences = New List(Of String)()

				sentences.Add("one two thee four")
				sentences.Add("some once again")
				sentences.Add("one another sentence")
			End If

			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[8]").set("spark.driver.host", "localhost").setAppName("SeqVecTests")
			sc = New JavaSparkContext(sparkConf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub tearDown()
			sc.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/21 - Failing - Issue #7657") public void testStringsTokenization1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStringsTokenization1()
			Dim rddSentences As JavaRDD(Of String) = sc.parallelize(sentences)

			Dim word2Vec As New SparkWord2Vec()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			word2Vec.getConfiguration().setTokenizerFactory(GetType(DefaultTokenizerFactory).FullName)
			word2Vec.getConfiguration().setElementsLearningAlgorithm("org.deeplearning4j.spark.models.sequencevectors.learning.elements.SparkSkipGram")
			word2Vec.setExporter(New SparkModelExporterAnonymousInnerClass(Me))


			word2Vec.fitSentences(rddSentences)

			Dim vocabCache As VocabCache(Of ShallowSequenceElement) = word2Vec.getShallowVocabCache()

			assertNotEquals(Nothing, vocabCache)

			assertEquals(9, vocabCache.numWords())
			assertEquals(2.0, vocabCache.wordFor(SequenceElement.getLongHash("one")).getElementFrequency(), 1e-5)
			assertEquals(1.0, vocabCache.wordFor(SequenceElement.getLongHash("two")).getElementFrequency(), 1e-5)
		End Sub

		Private Class SparkModelExporterAnonymousInnerClass
			Implements SparkModelExporter(Of VocabWord)

			Private ReadOnly outerInstance As SparkWord2VecTest

			Public Sub New(ByVal outerInstance As SparkWord2VecTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub export(ByVal rdd As JavaRDD(Of ExportContainer(Of VocabWord))) Implements SparkModelExporter(Of VocabWord).export
				rdd.foreach(New TestFn())
			End Sub
		End Class

		<Serializable>
		Public Class TestFn
			Implements VoidFunction(Of ExportContainer(Of VocabWord))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(org.deeplearning4j.spark.models.sequencevectors.export.ExportContainer<org.deeplearning4j.models.word2vec.VocabWord> v) throws Exception
			Public Overrides Sub [call](ByVal v As ExportContainer(Of VocabWord))
				assertNotNull(v.getElement())
				assertNotNull(v.getArray())
	'            System.out.println(v.getElement() + " - " + v.getArray());
			End Sub
		End Class
	End Class

End Namespace