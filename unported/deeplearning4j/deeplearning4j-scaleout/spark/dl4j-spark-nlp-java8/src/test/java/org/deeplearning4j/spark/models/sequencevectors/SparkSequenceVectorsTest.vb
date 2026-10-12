Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports SparkWord2VecTest = org.deeplearning4j.spark.models.word2vec.SparkWord2VecTest
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports org.junit.jupiter.api
Imports org.nd4j.common.primitives
Imports Downloader = org.nd4j.common.resources.Downloader
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.deeplearning4j.spark.models.sequencevectors


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Slf4j public class SparkSequenceVectorsTest extends org.deeplearning4j.BaseDL4JTest
	Public Class SparkSequenceVectorsTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000L
			End Get
		End Property

		Protected Friend Shared sequencesCyclic As IList(Of Sequence(Of VocabWord))
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
			If sequencesCyclic Is Nothing Then
				sequencesCyclic = New List(Of Sequence(Of VocabWord))()

				' 10 sequences in total
				For с As Integer = 0 To 9

					Dim sequence As New Sequence(Of VocabWord)()

					For e As Integer = 0 To 9
						' we will have 9 equal elements, with total frequency of 10
						sequence.addElement(New VocabWord(1.0, "" & e, CLng(e)))
					Next e

					' and 1 element with frequency of 20
					sequence.addElement(New VocabWord(1.0, "0", 0L))
					sequencesCyclic.Add(sequence)
				Next с
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
'ORIGINAL LINE: @Test @Disabled("Timeout issue") public void testFrequenciesCount() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFrequenciesCount()

			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim sequences As JavaRDD(Of Sequence(Of VocabWord)) = sc.parallelize(sequencesCyclic)

			Dim seqVec As New SparkSequenceVectors(Of VocabWord)()

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			seqVec.getConfiguration().setTokenizerFactory(GetType(DefaultTokenizerFactory).FullName)
			seqVec.getConfiguration().setElementsLearningAlgorithm("org.deeplearning4j.spark.models.sequencevectors.learning.elements.SparkSkipGram")
			seqVec.setExporter(New SparkModelExporterAnonymousInnerClass(Me))

			seqVec.fitSequences(sequences)

			Dim counter As Counter(Of Long) = seqVec.getCounter()

			' element "0" should have frequency of 20
			assertEquals(20, counter.getCount(0L), 1e-5)

			' elements 1 - 9 should have frequencies of 10
			Dim e As Integer = 1
			Do While e < sequencesCyclic(0).getElements().size() - 1
				assertEquals(10, counter.getCount(sequencesCyclic(0).getElementByIndex(e).getStorageId()), 1e-5)
				e += 1
			Loop


			Dim shallowVocab As VocabCache(Of ShallowSequenceElement) = seqVec.getShallowVocabCache()

			assertEquals(10, shallowVocab.numWords())

			Dim zero As ShallowSequenceElement = shallowVocab.tokenFor(0L)
			Dim first As ShallowSequenceElement = shallowVocab.tokenFor(1L)

			assertNotEquals(Nothing, zero)
			assertEquals(20.0, zero.getElementFrequency(), 1e-5)
			assertEquals(0, zero.Index)

			assertEquals(10.0, first.getElementFrequency(), 1e-5)
		End Sub

		Private Class SparkModelExporterAnonymousInnerClass
			Implements SparkModelExporter(Of VocabWord)

			Private ReadOnly outerInstance As SparkSequenceVectorsTest

			Public Sub New(ByVal outerInstance As SparkSequenceVectorsTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub export(ByVal rdd As JavaRDD(Of ExportContainer(Of VocabWord))) Implements SparkModelExporter(Of VocabWord).export
				rdd.foreach(New SparkWord2VecTest.TestFn())
			End Sub
		End Class

	End Class

End Namespace