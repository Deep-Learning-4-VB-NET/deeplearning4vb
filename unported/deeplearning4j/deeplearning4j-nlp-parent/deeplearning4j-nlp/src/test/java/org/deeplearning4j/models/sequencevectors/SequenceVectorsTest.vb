Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Writable = org.datavec.api.writable.Writable
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports NoEdgeHandling = org.deeplearning4j.models.sequencevectors.graph.enums.NoEdgeHandling
Imports PopularityMode = org.deeplearning4j.models.sequencevectors.graph.enums.PopularityMode
Imports SpreadSpectrum = org.deeplearning4j.models.sequencevectors.graph.enums.SpreadSpectrum
Imports WalkDirection = org.deeplearning4j.models.sequencevectors.graph.enums.WalkDirection
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.primitives
Imports org.deeplearning4j.models.sequencevectors.graph.walkers
Imports org.deeplearning4j.models.sequencevectors.graph.walkers.impl
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.iterators
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.sequencevectors.serialization
Imports org.deeplearning4j.models.sequencevectors.transformers.impl
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports BasicLineIterator = org.deeplearning4j.text.sentenceiterator.BasicLineIterator
Imports CommonPreprocessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Heartbeat = org.nd4j.linalg.heartbeat.Heartbeat
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

Namespace org.deeplearning4j.models.sequencevectors


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) @Tag(TagNames.NEEDS_VERIFY) @Disabled("Permissions issues on CI") public class SequenceVectorsTest extends org.deeplearning4j.BaseDL4JTest
	Public Class SequenceVectorsTest
		Inherits BaseDL4JTest

		Protected Friend Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(SequenceVectorsTest))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAbstractW2VModel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAbstractW2VModel()
			Dim resource As New ClassPathResource("big/raw_sentences.txt")
			Dim file As File = resource.File

			logger.info("dtype: {}", Nd4j.dataType())

			Dim vocabCache As AbstractCache(Of VocabWord) = (New AbstractCache.Builder(Of VocabWord)()).build()

	'        
	'            First we build line iterator
	'         
			Dim underlyingIterator As New BasicLineIterator(file)


	'        
	'            Now we need the way to convert lines into Sequences of VocabWords.
	'            In this example that's SentenceTransformer
	'         
			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(underlyingIterator).tokenizerFactory(t).build()


	'        
	'            And we pack that transformer into AbstractSequenceIterator
	'         
			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()


	'        
	'            Now we should build vocabulary out of sequence iterator.
	'            We can skip this phase, and just set SequenceVectors.resetModel(TRUE), and vocabulary will be mastered internally
	'        
			Dim constructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(sequenceIterator, 5).setTargetVocabCache(vocabCache).build()

			constructor.buildJointVocabulary(False, True)

			assertEquals(242, vocabCache.numWords())

			assertEquals(634303, vocabCache.totalWordOccurrences())

			Dim wordz As VocabWord = vocabCache.wordFor("day")

			logger.info("Wordz: " & wordz)

	'        
	'            Time to build WeightLookupTable instance for our new model
	'        

			Dim lookupTable As WeightLookupTable(Of VocabWord) = (New InMemoryLookupTable.Builder(Of VocabWord)()).lr(0.025).vectorLength(150).useAdaGrad(False).cache(vocabCache).build()

	'        
	'            reset model is viable only if you're setting SequenceVectors.resetModel() to false
	'            if set to True - it will be called internally
	'        
			lookupTable.resetWeights(True)

	'        
	'            Now we can build SequenceVectors model, that suits our needs
	'         
			Dim vectors As SequenceVectors(Of VocabWord) = (New SequenceVectors.Builder(Of VocabWord)(New VectorsConfiguration())).minWordFrequency(5).lookupTable(lookupTable).iterate(sequenceIterator).vocabCache(vocabCache).batchSize(250).iterations(1).epochs(1).resetModel(False).trainElementsRepresentation(True).trainSequencesRepresentation(False).build()

	'        
	'            Now, after all options are set, we just call fit()
	'         
			logger.info("Starting training...")

			vectors.fit()

			logger.info("Model saved...")

	'        
	'            As soon as fit() exits, model considered built, and we can test it.
	'            Please note: all similarity context is handled via SequenceElement's labels, so if you're using SequenceVectors to build models for complex
	'            objects/relations please take care of Labels uniqueness and meaning for yourself.
	'         
			Dim sim As Double = vectors.similarity("day", "night")
			logger.info("Day/night similarity: " & sim)
			assertTrue(sim > 0.6R)

			Dim labels As ICollection(Of String) = vectors.wordsNearest("day", 10)
			logger.info("Nearest labels to 'day': " & labels)

			Dim factory As SequenceElementFactory(Of VocabWord) = New AbstractElementFactory(Of VocabWord)(GetType(VocabWord))
			WordVectorSerializer.writeSequenceVectors(vectors, factory, "seqvec.mod")

			Dim model As SequenceVectors(Of VocabWord) = WordVectorSerializer.readSequenceVectors(factory, New File("seqvec.mod"))
			sim = model.similarity("day", "night")
			logger.info("day/night similarity: " & sim)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInternalVocabConstruction() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testInternalVocabConstruction()
			Dim resource As New ClassPathResource("big/raw_sentences.txt")
			Dim file As File = resource.File

			Dim underlyingIterator As New BasicLineIterator(file)

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(underlyingIterator).tokenizerFactory(t).build()

			Dim sequenceIterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim vectors As SequenceVectors(Of VocabWord) = (New SequenceVectors.Builder(Of VocabWord)(New VectorsConfiguration())).minWordFrequency(5).iterate(sequenceIterator).batchSize(250).iterations(1).epochs(1).resetModel(False).trainElementsRepresentation(True).build()


			logger.info("Fitting model...")

			vectors.fit()

			logger.info("Model ready...")

			Dim sim As Double = vectors.similarity("day", "night")
			logger.info("Day/night similarity: " & sim)
			assertTrue(sim > 0.6R)

			Dim labels As ICollection(Of String) = vectors.wordsNearest("day", 10)
			logger.info("Nearest labels to 'day': " & labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testElementsLearningAlgo1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testElementsLearningAlgo1()
			Dim vectors As SequenceVectors(Of VocabWord) = (New SequenceVectors.Builder(Of VocabWord)(New VectorsConfiguration())).minWordFrequency(5).batchSize(250).iterations(1).elementsLearningAlgorithm("org.deeplearning4j.models.embeddings.learning.impl.elements.SkipGram").epochs(1).resetModel(False).trainElementsRepresentation(True).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceLearningAlgo1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSequenceLearningAlgo1()
			Dim vectors As SequenceVectors(Of VocabWord) = (New SequenceVectors.Builder(Of VocabWord)(New VectorsConfiguration())).minWordFrequency(5).batchSize(250).iterations(1).sequenceLearningAlgorithm("org.deeplearning4j.models.embeddings.learning.impl.sequence.DBOW").epochs(1).resetModel(False).trainElementsRepresentation(False).build()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDeepWalk() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeepWalk()
			Heartbeat.Instance.disableHeartbeat()

			Dim vocabCache As AbstractCache(Of Blogger) = (New AbstractCache.Builder(Of Blogger)()).build()

			Dim graph As Graph(Of Blogger, Double) = buildGraph()


			Dim walker As GraphWalker(Of Blogger) = (New PopularityWalker.Builder(Of Blogger)(graph)).setNoEdgeHandling(NoEdgeHandling.RESTART_ON_DISCONNECTED).setWalkLength(40).setWalkDirection(WalkDirection.FORWARD_UNIQUE).setRestartProbability(0.05).setPopularitySpread(10).setPopularityMode(PopularityMode.MAXIMUM).setSpreadSpectrum(SpreadSpectrum.PROPORTIONAL).build()

	'        
	'        GraphWalker<Blogger> walker = new RandomWalker.Builder<Blogger>(graph)
	'                .setNoEdgeHandling(NoEdgeHandling.RESTART_ON_DISCONNECTED)
	'                .setWalkLength(40)
	'                .setWalkDirection(WalkDirection.RANDOM)
	'                .setRestartProbability(0.05)
	'                .build();
	'        

			Dim graphTransformer As GraphTransformer(Of Blogger) = (New GraphTransformer.Builder(Of Blogger)(graph)).setGraphWalker(walker).shuffleOnReset(True).setVocabCache(vocabCache).build()

			Dim blogger As Blogger = graph.getVertex(0).getValue()
			assertEquals(119, blogger.getElementFrequency(), 0.001)

			logger.info("Blogger: " & blogger)


			Dim sequenceIterator As AbstractSequenceIterator(Of Blogger) = (New AbstractSequenceIterator.Builder(Of Blogger)(graphTransformer)).build()

			Dim lookupTable As WeightLookupTable(Of Blogger) = (New InMemoryLookupTable.Builder(Of Blogger)()).lr(0.025).vectorLength(150).useAdaGrad(False).cache(vocabCache).seed(42).build()


			lookupTable.resetWeights(True)

			Dim vectors As SequenceVectors(Of Blogger) = (New SequenceVectors.Builder(Of Blogger)(New VectorsConfiguration())).lookupTable(lookupTable).iterate(sequenceIterator).vocabCache(vocabCache).batchSize(1000).iterations(1).epochs(10).resetModel(False).trainElementsRepresentation(True).trainSequencesRepresentation(False).elementsLearningAlgorithm(New SkipGram(Of Blogger)()).learningRate(0.025).layerSize(150).sampling(0).negativeSample(0).windowSize(4).workers(6).seed(42).build()

			vectors.fit()

			vectors.ModelUtils = New FlatModelUtils()

			'     logger.info("12: " + Arrays.toString(vectors.getWordVector("12")));

			Dim sim As Double = vectors.similarity("12", "72")
			Dim list As ICollection(Of String) = vectors.wordsNearest("12", 20)
			logger.info("12->72: " & sim)
			printWords("12", list, vectors)

			assertTrue(sim > 0.10)
			assertFalse(Double.IsNaN(sim))
		End Sub


		Private Function getBloggersFromGraph(ByVal graph As Graph(Of Blogger, Double)) As IList(Of Blogger)
			Dim result As IList(Of Blogger) = New List(Of Blogger)()

			Dim bloggers As IList(Of Vertex(Of Blogger)) = graph.getVertices(0, graph.numVertices() - 1)
			For Each vertex As Vertex(Of Blogger) In bloggers
				result.Add(vertex.getValue())
			Next vertex

			Return result
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.deeplearning4j.models.sequencevectors.graph.primitives.Graph<Blogger, Double> buildGraph() throws IOException, InterruptedException
		Private Shared Function buildGraph() As Graph(Of Blogger, Double)
			Dim nodes As New File("/ext/Temp/BlogCatalog/nodes.csv")

			Dim reader As New CSVRecordReader(0, ","c)
			reader.initialize(New org.datavec.api.Split.FileSplit(nodes))

			Dim bloggers As IList(Of Blogger) = New List(Of Blogger)()
			Dim cnt As Integer = 0
			Do While reader.hasNext()
				Dim lines As IList(Of Writable) = New List(Of Writable)(reader.next())
				Dim blogger As New Blogger(lines(0).toInt())
				bloggers.Add(blogger)
				cnt += 1
			Loop

			reader.Dispose()

			Dim graph As New Graph(Of Blogger, Double)(bloggers, True)

			' load edges
			Dim edges As New File("/ext/Temp/BlogCatalog/edges.csv")

			reader = New CSVRecordReader(0, ","c)
			reader.initialize(New org.datavec.api.Split.FileSplit(edges))

			Do While reader.hasNext()
				Dim lines As IList(Of Writable) = New List(Of Writable)(reader.next())
				Dim from As Integer = lines(0).toInt()
				Dim [to] As Integer = lines(1).toInt()

				graph.addEdge(from - 1, [to] - 1, 1.0, False)
			Loop

			logger.info("Connected on 0: [" & graph.getConnectedVertices(0).Count & "]")
			logger.info("Connected on 1: [" & graph.getConnectedVertices(1).Count & "]")
			logger.info("Connected on 3: [" & graph.getConnectedVertices(3).Count & "]")
			assertEquals(119, graph.getConnectedVertices(0).Count)
			assertEquals(9, graph.getConnectedVertices(1).Count)
			assertEquals(6, graph.getConnectedVertices(3).Count)

			Return graph
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data private static class Blogger extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
		<Serializable>
		Private Class Blogger
			Inherits SequenceElement

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int id;
			Friend id As Integer

			Public Sub New()
				MyBase.New()
			End Sub

			Public Sub New(ByVal id As Integer)
				MyBase.New()
				Me.id = id
			End Sub

			''' <summary>
			''' This method should return string representation of this SequenceElement, so it can be used for
			''' 
			''' @return
			''' </summary>
			Public Overrides ReadOnly Property Label As String
				Get
					Return id.ToString()
				End Get
			End Property

			''' <summary>
			''' @return
			''' </summary>
			Public Overrides Function toJSON() As String
				Return Nothing
			End Function

			Public Overrides Function ToString() As String
				Return "VocabWord{" & "wordFrequency=" & Me.elementFrequency_Conflict & ", index=" & index_Conflict & ", codes=" & codes_Conflict & ", word='" & id.ToString() & "'"c & ", points=" & points_Conflict & ", codeLength=" & codeLength_Conflict & "}"c
			End Function
		End Class

		Private Shared Sub printWords(ByVal target As String, ByVal list As ICollection(Of String), ByVal vec As SequenceVectors)
			Console.WriteLine("Words close to [" & target & "]: ")
			For Each word As String In list
				Dim sim As Double = vec.similarity(target, word)
				Console.Write("'" & word & "': [" & sim & "], ")
			Next word
			Console.Write(vbLf)
		End Sub
	End Class

End Namespace