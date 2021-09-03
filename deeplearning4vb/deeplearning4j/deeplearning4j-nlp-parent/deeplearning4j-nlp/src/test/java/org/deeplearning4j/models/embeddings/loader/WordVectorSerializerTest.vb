Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports FastText = org.deeplearning4j.models.fasttext.FastText
Imports ParagraphVectors = org.deeplearning4j.models.paragraphvectors.ParagraphVectors
Imports org.deeplearning4j.models.sequencevectors
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertNotNull
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assertions.fail

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

Namespace org.deeplearning4j.models.embeddings.loader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class WordVectorSerializerTest extends org.deeplearning4j.BaseDL4JTest
	Public Class WordVectorSerializerTest
		Inherits BaseDL4JTest

		Private cache As AbstractCache(Of VocabWord)



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			cache = (New AbstractCache.Builder(Of VocabWord)()).build()

			Dim words As val = New VocabWord(2){}
			words(0) = New VocabWord(1.0, "word")
			words(1) = New VocabWord(2.0, "test")
			words(2) = New VocabWord(3.0, "tester")

			For i As Integer = 0 To words.length - 1
				cache.addToken(words(i))
				cache.addWordToIndex(i, words(i).getLabel())
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void sequenceVectorsCorrect_WhenDeserialized()
		Public Overridable Sub sequenceVectorsCorrect_WhenDeserialized()

			Dim syn0 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1Neg As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2)

			Dim lookupTable As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).useAdaGrad(False).cache(cache).build()

			lookupTable.setSyn0(syn0)
			lookupTable.setSyn1(syn1)
			lookupTable.Syn1Neg = syn1Neg

			Dim vectors As SequenceVectors(Of VocabWord) = (New SequenceVectors.Builder(Of VocabWord)(New VectorsConfiguration())).vocabCache(cache).lookupTable(lookupTable).build()
			Dim deser As SequenceVectors(Of VocabWord) = Nothing
			Try
				Dim baos As New MemoryStream()
				WordVectorSerializer.writeSequenceVectors(vectors, baos)
				Dim bytesResult() As SByte = baos.toByteArray()
				deser = WordVectorSerializer.readSequenceVectors(New MemoryStream(bytesResult), True)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try

			assertNotNull(vectors.getConfiguration())
			assertEquals(vectors.getConfiguration(), deser.getConfiguration())

			assertEquals(cache.totalWordOccurrences(),deser.vocab().totalWordOccurrences())
			assertEquals(cache.totalNumberOfDocs(), deser.vocab().totalNumberOfDocs())
			assertEquals(cache.numWords(), deser.vocab().numWords())

			Dim i As Integer = 0
			Do While i < cache.words().Count
				Dim cached As val = cache.wordAtIndex(i)
				Dim restored As val = deser.vocab().wordAtIndex(i)
				assertNotNull(cached)
				assertEquals(cached, restored)
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void W2V_Correct_WhenDeserialized()
		Public Overridable Sub W2V_Correct_WhenDeserialized()

			Dim syn0 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1Neg As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2)

			Dim lookupTable As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).useAdaGrad(False).cache(cache).build()

			lookupTable.setSyn0(syn0)
			lookupTable.setSyn1(syn1)
			lookupTable.Syn1Neg = syn1Neg

			Dim vectors As SequenceVectors(Of VocabWord) = (New SequenceVectors.Builder(Of VocabWord)(New VectorsConfiguration())).vocabCache(cache).lookupTable(lookupTable).layerSize(200).modelUtils(New BasicModelUtils(Of VocabWord)()).build()

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			Dim word2Vec As Word2Vec = (New Word2Vec.Builder(vectors.getConfiguration())).vocabCache(vectors.vocab()).lookupTable(lookupTable).modelUtils(New FlatModelUtils(Of VocabWord)()).limitVocabularySize(1000).elementsLearningAlgorithm(GetType(CBOW).FullName).allowParallelTokenization(True).usePreciseMode(True).batchSize(1024).windowSize(23).minWordFrequency(24).iterations(54).seed(45).learningRate(0.08).epochs(45).stopWords(Collections.singletonList("NOT")).sampling(44).workers(45).negativeSample(56).useAdaGrad(True).useHierarchicSoftmax(False).minLearningRate(0.002).resetModel(True).useUnknown(True).enableScavenger(True).usePreciseWeightInit(True).build()

			Dim deser As Word2Vec = Nothing
			Try
				Dim baos As New MemoryStream()
				WordVectorSerializer.writeWord2Vec(word2Vec, baos)
				Dim bytesResult() As SByte = baos.toByteArray()
				deser = WordVectorSerializer.readWord2Vec(New MemoryStream(bytesResult), True)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try

			assertNotNull(word2Vec.getConfiguration())
			assertEquals(word2Vec.getConfiguration(), deser.getConfiguration())

			assertEquals(cache.totalWordOccurrences(),deser.vocab().totalWordOccurrences())
			assertEquals(cache.totalNumberOfDocs(), deser.vocab().totalNumberOfDocs())
			assertEquals(cache.numWords(), deser.vocab().numWords())

			Dim i As Integer = 0
			Do While i < cache.words().Count
				Dim cached As val = cache.wordAtIndex(i)
				Dim restored As val = deser.vocab().wordAtIndex(i)
				assertNotNull(cached)
				assertEquals(cached, restored)
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void ParaVec_Correct_WhenDeserialized()
		Public Overridable Sub ParaVec_Correct_WhenDeserialized()

			Dim syn0 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1Neg As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2)

			Dim lookupTable As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).useAdaGrad(False).cache(cache).build()

			lookupTable.setSyn0(syn0)
			lookupTable.setSyn1(syn1)
			lookupTable.Syn1Neg = syn1Neg

			Dim paragraphVectors As ParagraphVectors = (New ParagraphVectors.Builder()).vocabCache(cache).lookupTable(lookupTable).build()

			Dim deser As Word2Vec = Nothing
			Try
				Dim baos As New MemoryStream()
				WordVectorSerializer.writeWord2Vec(paragraphVectors, baos)
				Dim bytesResult() As SByte = baos.toByteArray()
				deser = WordVectorSerializer.readWord2Vec(New MemoryStream(bytesResult), True)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try

			assertNotNull(paragraphVectors.getConfiguration())
			assertEquals(paragraphVectors.getConfiguration(), deser.getConfiguration())

			assertEquals(cache.totalWordOccurrences(),deser.vocab().totalWordOccurrences())
			assertEquals(cache.totalNumberOfDocs(), deser.vocab().totalNumberOfDocs())
			assertEquals(cache.numWords(), deser.vocab().numWords())

			Dim i As Integer = 0
			Do While i < cache.words().Count
				Dim cached As val = cache.wordAtIndex(i)
				Dim restored As val = deser.vocab().wordAtIndex(i)
				assertNotNull(cached)
				assertEquals(cached, restored)
				i += 1
			Loop

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void weightLookupTable_Correct_WhenDeserialized(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub weightLookupTable_Correct_WhenDeserialized(ByVal testDir As Path)

			Dim syn0 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1 As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2), syn1Neg As INDArray = Nd4j.rand(DataType.FLOAT, 10, 2)

			Dim lookupTable As InMemoryLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).useAdaGrad(False).cache(cache).build()

			lookupTable.setSyn0(syn0)
			lookupTable.setSyn1(syn1)
			lookupTable.Syn1Neg = syn1Neg

			Dim dir As File = testDir.toFile()
			Dim file As New File(dir, "lookupTable.txt")

			Dim deser As WeightLookupTable(Of VocabWord) = Nothing
			Try
				WordVectorSerializer.writeLookupTable(lookupTable, file)
				deser = WordVectorSerializer.readLookupTable(file)
			Catch e As Exception
				log.error("",e)
				fail()
			End Try
			assertEquals(lookupTable.Vocab.totalWordOccurrences(), CType(deser, InMemoryLookupTable(Of VocabWord)).Vocab.totalWordOccurrences())
			assertEquals(cache.totalNumberOfDocs(), CType(deser, InMemoryLookupTable(Of VocabWord)).Vocab.totalNumberOfDocs())
			assertEquals(cache.numWords(), CType(deser, InMemoryLookupTable(Of VocabWord)).Vocab.numWords())

			Dim i As Integer = 0
			Do While i < cache.words().Count
				Dim cached As val = cache.wordAtIndex(i)
				Dim restored As val = CType(deser, InMemoryLookupTable(Of VocabWord)).Vocab.wordAtIndex(i)
				assertNotNull(cached)
				assertEquals(cached, restored)
				i += 1
			Loop

			assertEquals(lookupTable.getSyn0().columns(), CType(deser, InMemoryLookupTable(Of VocabWord)).getSyn0().columns())
			assertEquals(lookupTable.getSyn0().rows(), CType(deser, InMemoryLookupTable(Of VocabWord)).getSyn0().rows())
			Dim c As Integer = 0
			Do While c < CType(deser, InMemoryLookupTable(Of VocabWord)).getSyn0().columns()
				Dim r As Integer = 0
				Do While r < CType(deser, InMemoryLookupTable(Of VocabWord)).getSyn0().rows()
					assertEquals(lookupTable.getSyn0().getDouble(r,c), CType(deser, InMemoryLookupTable(Of VocabWord)).getSyn0().getDouble(r,c), 1e-5)
					r += 1
				Loop
				c += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void FastText_Correct_WhenDeserialized(@TempDir Path testDir) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub FastText_Correct_WhenDeserialized(ByVal testDir As Path)

			Dim fastText As FastText = FastText.builder().cbow(True).build()

			Dim dir As File = testDir.toFile()
			WordVectorSerializer.writeWordVectors(fastText, New File(dir, "some.data"))

			Dim deser As FastText = Nothing
			Try
				deser = WordVectorSerializer.readWordVectors(New File(dir, "some.data"))
			Catch e As Exception
				log.error("",e)
				fail()
			End Try

			assertNotNull(deser)
			assertEquals(fastText.isCbow(), deser.isCbow())
			assertEquals(fastText.isModelLoaded(), deser.isModelLoaded())
			assertEquals(fastText.isAnalogies(), deser.isAnalogies())
			assertEquals(fastText.isNn(), deser.isNn())
			assertEquals(fastText.isPredict(), deser.isPredict())
			assertEquals(fastText.isPredict_prob(), deser.isPredict_prob())
			assertEquals(fastText.isQuantize(), deser.isQuantize())
			assertEquals(fastText.getInputFile(), deser.getInputFile())
			assertEquals(fastText.getOutputFile(), deser.getOutputFile())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIsHeader_withValidHeader()
		Public Overridable Sub testIsHeader_withValidHeader()

			' Given 
			Dim cache As New AbstractCache(Of VocabWord)()
			Dim line As String = "48 100"

			' When 
			Dim isHeader As Boolean = WordVectorSerializer.isHeader(line, cache)

			' Then 
			assertTrue(isHeader)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIsHeader_notHeader()
		Public Overridable Sub testIsHeader_notHeader()

			' Given 
			Dim cache As New AbstractCache(Of VocabWord)()
			Dim line As String = "your -0.0017603 0.0030831 0.00069072 0.0020581 -0.0050952 -2.2573e-05 -0.001141"

			' When 
			Dim isHeader As Boolean = WordVectorSerializer.isHeader(line, cache)

			' Then 
			assertFalse(isHeader)
		End Sub
	End Class

End Namespace