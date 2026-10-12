Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports org.deeplearning4j.models.embeddings.inmemory
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.embeddings.wordvectors
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports CountCumSum = org.deeplearning4j.spark.text.functions.CountCumSum
Imports TextPipeline = org.deeplearning4j.spark.text.functions.TextPipeline
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Environment = org.nd4j.linalg.heartbeat.reports.Environment
Imports [Event] = org.nd4j.linalg.heartbeat.reports.Event
Imports EnvironmentUtils = org.nd4j.linalg.heartbeat.utils.EnvironmentUtils
Imports org.nd4j.common.primitives
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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


	<Serializable>
	Public Class Word2Vec
		Inherits WordVectorsImpl(Of VocabWord)

		Private trainedSyn1 As INDArray
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(Word2Vec))
		Private MAX_EXP As Integer = 6
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private double[] expTable;
		Private expTable() As Double
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.embeddings.loader.VectorsConfiguration configuration;
		Protected Friend configuration As VectorsConfiguration

		' Input by user only via setters
		Private nGrams As Integer = 1
		Private tokenizer As String = "org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory"
		Private tokenPreprocessor As String = "org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor"
		Private removeStop As Boolean = False
		Private Shadows seed As Long = 42L
		Private Shadows useUnknown As Boolean = False

		' Constructor to take InMemoryLookupCache table from an already trained model
		Protected Friend Sub New(ByVal trainedSyn1 As INDArray)
			Me.trainedSyn1 = trainedSyn1
			Me.expTable = initExpTable()
		End Sub

		Protected Friend Sub New()
			Me.expTable = initExpTable()
		End Sub

		Protected Friend Overridable Function initExpTable() As Double()
			Dim expTable(99999) As Double
			For i As Integer = 0 To expTable.Length - 1
				Dim tmp As Double = FastMath.exp((i / CDbl(expTable.Length) * 2 - 1) * MAX_EXP)
				expTable(i) = tmp / (tmp + 1.0)
			Next i
			Return expTable
		End Function

		Public Overridable ReadOnly Property TokenizerVarMap As IDictionary(Of String, Object)
			Get
				Return New HashMapAnonymousInnerClass(Me)
			End Get
		End Property

		Private Class HashMapAnonymousInnerClass
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As Word2Vec

			Public Sub New(ByVal outerInstance As Word2Vec)
				Me.outerInstance = outerInstance

				Me.put("numWords", outerInstance.minWordFrequency)
				Me.put("nGrams", outerInstance.nGrams)
				Me.put("tokenizer", outerInstance.tokenizer)
				Me.put("tokenPreprocessor", outerInstance.tokenPreprocessor)
				Me.put("removeStop", outerInstance.removeStop)
				Me.put("stopWords", outerInstance.stopWords)
				Me.put("useUnk", outerInstance.useUnknown)
				Me.put("vectorsConfiguration", outerInstance.configuration)
			End Sub

		End Class

		Public Overridable ReadOnly Property Word2vecVarMap As IDictionary(Of String, Object)
			Get
				Return New HashMapAnonymousInnerClass2(Me)
			End Get
		End Property

		Private Class HashMapAnonymousInnerClass2
			Inherits Dictionary(Of String, Object)

			Private ReadOnly outerInstance As Word2Vec

			Public Sub New(ByVal outerInstance As Word2Vec)
				Me.outerInstance = outerInstance

				Me.put("vectorLength", outerInstance.layerSize_Conflict)
				Me.put("useAdaGrad", outerInstance.useAdeGrad)
				Me.put("negative", outerInstance.negative)
				Me.put("window", outerInstance.window)
				Me.put("alpha", outerInstance.learningRate.get())
				Me.put("minAlpha", outerInstance.minLearningRate)
				Me.put("iterations", outerInstance.numIterations)
				Me.put("seed", outerInstance.seed)
				Me.put("maxExp", outerInstance.MAX_EXP)
				Me.put("batchSize", outerInstance.batchSize)
			End Sub

		End Class

		''' <summary>
		'''  Training word2vec model on a given text corpus
		''' </summary>
		''' <param name="corpusRDD"> training corpus </param>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void train(org.apache.spark.api.java.JavaRDD<String> corpusRDD) throws Exception
		Public Overridable Sub train(ByVal corpusRDD As JavaRDD(Of String))
			log.info("Start training ...")

			If workers > 0 Then
				corpusRDD.repartition(workers)
			End If

			' SparkContext
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.api.java.JavaSparkContext sc = new org.apache.spark.api.java.JavaSparkContext(corpusRDD.context());
			Dim sc As New JavaSparkContext(corpusRDD.context())

			' Pre-defined variables
			Dim tokenizerVarMap As IDictionary(Of String, Object) = getTokenizerVarMap()
			Dim word2vecVarMap As IDictionary(Of String, Object) = getWord2vecVarMap()

			' Variables to fill in train
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.api.java.JavaRDD<java.util.concurrent.atomic.AtomicLong> sentenceWordsCountRDD;
			Dim sentenceWordsCountRDD As JavaRDD(Of AtomicLong)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.api.java.JavaRDD<java.util.List<org.deeplearning4j.models.word2vec.VocabWord>> vocabWordListRDD;
			Dim vocabWordListRDD As JavaRDD(Of IList(Of VocabWord))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.api.java.JavaPairRDD<java.util.List<org.deeplearning4j.models.word2vec.VocabWord>, Long> vocabWordListSentenceCumSumRDD;
			Dim vocabWordListSentenceCumSumRDD As JavaPairRDD(Of IList(Of VocabWord), Long)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache;
			Dim vocabCache As VocabCache(Of VocabWord)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.spark.api.java.JavaRDD<Long> sentenceCumSumCountRDD;
			Dim sentenceCumSumCountRDD As JavaRDD(Of Long)
			Dim maxRep As Integer = 1

			' Start Training //
			'////////////////////////////////////
			log.info("Tokenization and building VocabCache ...")
			' Processing every sentence and make a VocabCache which gets fed into a LookupCache
			Dim broadcastTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(tokenizerVarMap)
			Dim pipeline As New TextPipeline(corpusRDD, broadcastTokenizerVarMap)
			pipeline.buildVocabCache()
			pipeline.buildVocabWordListRDD()

			' Get total word count and put into word2vec variable map
			word2vecVarMap("totalWordCount") = pipeline.getTotalWordCount()

			' 2 RDDs: (vocab words list) and (sentence Count).Already cached
			sentenceWordsCountRDD = pipeline.getSentenceCountRDD()
			vocabWordListRDD = pipeline.getVocabWordListRDD()

			' Get vocabCache and broad-casted vocabCache
			Dim vocabCacheBroadcast As Broadcast(Of VocabCache(Of VocabWord)) = pipeline.getBroadCastVocabCache()
			vocabCache = vocabCacheBroadcast.getValue()

			log.info("Vocab size: {}", vocabCache.numWords())

			'////////////////////////////////////
			log.info("Building Huffman Tree ...")
			' Building Huffman Tree would update the code and point in each of the vocabWord in vocabCache
	'        
	'        We don't need to build tree here, since it was built earlier, at TextPipeline.buildVocabCache() call.
	'        
	'        Huffman huffman = new Huffman(vocabCache.vocabWords());
	'        huffman.build();
	'        huffman.applyIndexes(vocabCache);
	'        
			'////////////////////////////////////
			log.info("Calculating cumulative sum of sentence counts ...")
			sentenceCumSumCountRDD = (New CountCumSum(sentenceWordsCountRDD)).buildCumSum()

			'////////////////////////////////////
			log.info("Mapping to RDD(vocabWordList, cumulative sentence count) ...")
			vocabWordListSentenceCumSumRDD = vocabWordListRDD.zip(sentenceCumSumCountRDD).setName("vocabWordListSentenceCumSumRDD")

			'///////////////////////////////////
			log.info("Broadcasting word2vec variables to workers ...")
			Dim word2vecVarMapBroadcast As Broadcast(Of IDictionary(Of String, Object)) = sc.broadcast(word2vecVarMap)
			Dim expTableBroadcast As Broadcast(Of Double()) = sc.broadcast(expTable)



			'///////////////////////////////////
			log.info("Training word2vec sentences ...")
			Dim firstIterFunc As FlatMapFunction = New FirstIterationFunction(word2vecVarMapBroadcast, expTableBroadcast, vocabCacheBroadcast)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") org.apache.spark.api.java.JavaRDD<org.nd4j.common.primitives.Pair<org.deeplearning4j.models.word2vec.VocabWord, org.nd4j.linalg.api.ndarray.INDArray>> indexSyn0UpdateEntryRDD = vocabWordListSentenceCumSumRDD.mapPartitions(firstIterFunc).map(new MapToPairFunction());
			Dim indexSyn0UpdateEntryRDD As JavaRDD(Of Pair(Of VocabWord, INDArray)) = vocabWordListSentenceCumSumRDD.mapPartitions(firstIterFunc).map(New MapToPairFunction())

			' Get all the syn0 updates into a list in driver
			Dim syn0UpdateEntries As IList(Of Pair(Of VocabWord, INDArray)) = indexSyn0UpdateEntryRDD.collect()

			' Instantiate syn0
			Dim syn0 As INDArray = Nd4j.zeros(vocabCache.numWords(), layerSize_Conflict)

			' Updating syn0 first pass: just add vectors obtained from different nodes
			log.info("Averaging results...")
			Dim updates As IDictionary(Of VocabWord, AtomicInteger) = New Dictionary(Of VocabWord, AtomicInteger)()
			Dim updaters As IDictionary(Of Long, Long) = New Dictionary(Of Long, Long)()
			For Each syn0UpdateEntry As Pair(Of VocabWord, INDArray) In syn0UpdateEntries
				syn0.getRow(syn0UpdateEntry.First.Index).addi(syn0UpdateEntry.Second)

				' for proper averaging we need to divide resulting sums later, by the number of additions
				If updates.ContainsKey(syn0UpdateEntry.First) Then
					updates(syn0UpdateEntry.First).incrementAndGet()
				Else
					updates(syn0UpdateEntry.First) = New AtomicInteger(1)
				End If

				If Not updaters.ContainsKey(syn0UpdateEntry.First.getVocabId()) Then
					updaters(syn0UpdateEntry.First.getVocabId()) = syn0UpdateEntry.First.getAffinityId()
				End If
			Next syn0UpdateEntry

			' Updating syn0 second pass: average obtained vectors
			For Each entry As KeyValuePair(Of VocabWord, AtomicInteger) In updates.SetOfKeyValuePairs()
				If entry.Value.get() > 1 Then
					If entry.Value.get() > maxRep Then
						maxRep = entry.Value.get()
					End If
					syn0.getRow(entry.Key.getIndex()).divi(entry.Value.get())
				End If
			Next entry

			Dim totals As Long = 0

			log.info("Finished calculations...")


			vocab_Conflict = vocabCache
			Dim inMemoryLookupTable As New InMemoryLookupTable(Of VocabWord)()
			Dim env As Environment = EnvironmentUtils.buildEnvironment()
			env.setNumCores(maxRep)
			env.setAvailableMemory(totals)
			update(env, [Event].SPARK)
			inMemoryLookupTable.Vocab = vocabCache
			inMemoryLookupTable.VectorLength = layerSize_Conflict
			inMemoryLookupTable.setSyn0(syn0)
			lookupTable_Conflict = inMemoryLookupTable
			modelUtils_Conflict.init(lookupTable_Conflict)
		End Sub



		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field nGrams was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend nGrams_Conflict As Integer = 1
			Protected Friend numIterations As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field minWordFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minWordFrequency_Conflict As Integer = 1
			Protected Friend numEpochs As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field learningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend learningRate_Conflict As Double = 0.025
'JAVA TO VB CONVERTER NOTE: The field minLearningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minLearningRate_Conflict As Double = 0.001
'JAVA TO VB CONVERTER NOTE: The field windowSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend windowSize_Conflict As Integer = 5
'JAVA TO VB CONVERTER NOTE: The field negative was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend negative_Conflict As Double = 0
'JAVA TO VB CONVERTER NOTE: The field sampling was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend sampling_Conflict As Double = 1e-5
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seed_Conflict As Long = 42L
'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend useAdaGrad_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field tokenizerFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend tokenizerFactory_Conflict As TokenizerFactory = New DefaultTokenizerFactory()
			Protected Friend configuration As New VectorsConfiguration()
'JAVA TO VB CONVERTER NOTE: The field layerSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend layerSize_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field stopWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend stopWords_Conflict As IList(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field batchSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend batchSize_Conflict As Integer = 100
			Protected Friend useUnk As Boolean = False
			Friend tokenizer As String = ""
'JAVA TO VB CONVERTER NOTE: The field tokenPreprocessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend tokenPreprocessor_Conflict As String = ""
'JAVA TO VB CONVERTER NOTE: The field workers was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend workers_Conflict As Integer = 0

			''' <summary>
			''' Creates Builder instance with default parameters set.
			''' </summary>
			Public Sub New()
				Me.New(New VectorsConfiguration())
			End Sub

			''' <summary>
			''' Uses VectorsConfiguration bean to initialize Word2Vec model parameters
			''' </summary>
			''' <param name="configuration"> </param>
			Public Sub New(ByVal configuration As VectorsConfiguration)
				Me.configuration = configuration
				Me.numIterations = configuration.getIterations()
				Me.numEpochs = configuration.getEpochs()
				Me.minLearningRate_Conflict = configuration.getMinLearningRate()
				Me.learningRate_Conflict = configuration.getLearningRate()
				Me.sampling_Conflict = configuration.getSampling()
				Me.negative_Conflict = configuration.getNegative()
				Me.minWordFrequency_Conflict = configuration.getMinWordFrequency()
				Me.seed_Conflict = configuration.getSeed()
				'            this.stopWords = configuration.get

				'  TODO: investigate this
				'this.hugeModelExpected = configuration.isHugeModelExpected();

				Me.batchSize_Conflict = configuration.getBatchSize()
				Me.layerSize_Conflict = configuration.getLayersSize()

				'  this.learningRateDecayWords = configuration.getLearningRateDecayWords();
				Me.useAdaGrad_Conflict = configuration.isUseAdaGrad()
				Me.windowSize_Conflict = configuration.getWindow()

				If configuration.getStopList() IsNot Nothing Then
					CType(Me.stopWords_Conflict, List(Of String)).AddRange(configuration.getStopList())
				End If
			End Sub

			''' <summary>
			''' Specifies window size
			''' </summary>
			''' <param name="windowSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter windowSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function windowSize(ByVal windowSize_Conflict As Integer) As Builder
				Me.windowSize_Conflict = windowSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specifies negative sampling </summary>
			''' <param name="negative">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter negative was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function negative(ByVal negative_Conflict As Integer) As Builder
				Me.negative_Conflict = negative_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specifies subsamplng value
			''' </summary>
			''' <param name="sampling">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter sampling was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sampling(ByVal sampling_Conflict As Double) As Builder
				Me.sampling_Conflict = sampling_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method specifies initial learning rate for model
			''' </summary>
			''' <param name="lr">
			''' @return </param>
			Public Overridable Function learningRate(ByVal lr As Double) As Builder
				Me.learningRate_Conflict = lr
				Return Me
			End Function

			''' <summary>
			''' This method specifies bottom threshold for learning rate decay
			''' </summary>
			''' <param name="mlr">
			''' @return </param>
			Public Overridable Function minLearningRate(ByVal mlr As Double) As Builder
				Me.minLearningRate_Conflict = mlr
				Return Me
			End Function

			''' <summary>
			''' This method specifies number of iterations over batch on each node
			''' </summary>
			''' <param name="numIterations">
			''' @return </param>
			Public Overridable Function iterations(ByVal numIterations As Integer) As Builder
				Me.numIterations = numIterations
				Return Me
			End Function

			''' <summary>
			''' This method specifies number of epochs done over whole corpus
			''' 
			''' PLEASE NOTE: NOT IMPLEMENTED
			''' </summary>
			''' <param name="numEpochs">
			''' @return </param>
			Public Overridable Function epochs(ByVal numEpochs As Integer) As Builder
				' TODO: implement epochs imitation for spark w2v
				Me.numEpochs = numEpochs
				Return Me
			End Function

			''' <summary>
			''' This method specifies minimum word frequency threshold. All words below this threshold will be ignored.
			''' </summary>
			''' <param name="minWordFrequency">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter minWordFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minWordFrequency(ByVal minWordFrequency_Conflict As Integer) As Builder
				Me.minWordFrequency_Conflict = minWordFrequency_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method specifies, if adaptive gradients should be used during model training
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function useAdaGrad(ByVal reallyUse As Boolean) As Builder
				Me.useAdaGrad_Conflict = reallyUse
				Return Me
			End Function

			''' <summary>
			''' Specifies random seed to be used during weights initialization;
			''' </summary>
			''' <param name="seed">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter seed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function seed(ByVal seed_Conflict As Long) As Builder
				Me.seed_Conflict = seed_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specifies TokenizerFactory to be used for tokenization
			''' 
			''' PLEASE NOTE: You can't use anonymous implementation here
			''' </summary>
			''' <param name="factory">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder tokenizerFactory(@NonNull TokenizerFactory factory)
			Public Overridable Function tokenizerFactory(ByVal factory As TokenizerFactory) As Builder
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.tokenizer = factory.GetType().FullName

				If factory.getTokenPreProcessor() IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					Me.tokenPreprocessor_Conflict = factory.getTokenPreProcessor().GetType().FullName
				Else
					Me.tokenPreprocessor_Conflict = ""
				End If

				Return Me
			End Function

			''' <summary>
			''' Specifies TokenizerFactory class to be used for tokenization
			''' 
			''' </summary>
			''' <param name="tokenizer"> class name for tokenizerFactory
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder tokenizerFactory(@NonNull String tokenizer)
			Public Overridable Function tokenizerFactory(ByVal tokenizer As String) As Builder
				Me.tokenizer = tokenizer
				Return Me
			End Function

			''' <summary>
			''' Specifies TokenPreProcessor class to be used during tokenization
			''' 
			''' </summary>
			''' <param name="tokenPreprocessor"> class name for tokenPreProcessor
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder tokenPreprocessor(@NonNull String tokenPreprocessor)
'JAVA TO VB CONVERTER NOTE: The parameter tokenPreprocessor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tokenPreprocessor(ByVal tokenPreprocessor_Conflict As String) As Builder
				Me.tokenPreprocessor_Conflict = tokenPreprocessor_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify number of workers for training process.
			''' This value will be used to repartition RDD.
			''' 
			''' PLEASE NOTE: Recommended value is number of vCPU available within your spark cluster.
			''' </summary>
			''' <param name="workers">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter workers was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workers(ByVal workers_Conflict As Integer) As Builder
				Me.workers_Conflict = workers_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specifies output vector's dimensions
			''' </summary>
			''' <param name="layerSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter layerSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layerSize(ByVal layerSize_Conflict As Integer) As Builder
				Me.layerSize_Conflict = layerSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specifies N of n-Grams :)
			''' </summary>
			''' <param name="nGrams">
			''' @return </param>
			Public Overridable Function setNGrams(ByVal nGrams As Integer) As Builder
				Me.nGrams_Conflict = nGrams
				Return Me
			End Function

			''' <summary>
			''' This method defines list of stop-words, that are to be ignored during vocab building and training
			''' </summary>
			''' <param name="stopWords">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder stopWords(@NonNull List<String> stopWords)
'JAVA TO VB CONVERTER NOTE: The parameter stopWords was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function stopWords(ByVal stopWords_Conflict As IList(Of String)) As Builder
				For Each word As String In stopWords_Conflict
					If Not Me.stopWords_Conflict.Contains(word) Then
						Me.stopWords_Conflict.Add(word)
					End If
				Next word
				Return Me
			End Function

			''' <summary>
			''' Specifies the size of mini-batch, used in single iteration during training
			''' </summary>
			''' <param name="batchSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter batchSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function batchSize(ByVal batchSize_Conflict As Integer) As Builder
				Me.batchSize_Conflict = batchSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specifies, if UNK word should be used instead of words that are absent in vocab
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function useUnknown(ByVal reallyUse As Boolean) As Builder
				Me.useUnk = reallyUse
				Return Me
			End Function

			Public Overridable Function build() As Word2Vec
				Dim ret As New Word2Vec()

				Me.configuration.setLearningRate(Me.learningRate_Conflict)
				Me.configuration.setLayersSize(layerSize_Conflict)
				Me.configuration.setWindow(windowSize_Conflict)
				Me.configuration.setMinWordFrequency(minWordFrequency_Conflict)
				Me.configuration.setIterations(numIterations)
				Me.configuration.setSeed(seed_Conflict)
				Me.configuration.setMinLearningRate(minLearningRate_Conflict)
				Me.configuration.setSampling(Me.sampling_Conflict)
				Me.configuration.setUseAdaGrad(useAdaGrad_Conflict)
				Me.configuration.setNegative(negative_Conflict)
				Me.configuration.setEpochs(Me.numEpochs)
				Me.configuration.setBatchSize(Me.batchSize_Conflict)
				Me.configuration.setStopList(Me.stopWords_Conflict)

				ret.workers = Me.workers_Conflict
				ret.nGrams = Me.nGrams_Conflict

				ret.configuration = Me.configuration

				ret.numEpochs = Me.numEpochs
				ret.numIterations = Me.numIterations
				ret.minWordFrequency = Me.minWordFrequency_Conflict
				ret.learningRate.set(Me.learningRate_Conflict)
				ret.minLearningRate = Me.minLearningRate_Conflict
				ret.sampling = Me.sampling_Conflict
				ret.negative = Me.negative_Conflict
				ret.layerSize_Conflict = Me.layerSize_Conflict
				ret.window = Me.windowSize_Conflict
				ret.useAdeGrad = Me.useAdaGrad_Conflict
				ret.stopWords = Me.stopWords_Conflict
				ret.batchSize = Me.batchSize_Conflict
				ret.useUnknown = Me.useUnk

				ret.tokenizer = Me.tokenizer
				ret.tokenPreprocessor = Me.tokenPreprocessor_Conflict

				Return ret
			End Function
		End Class
	End Class

End Namespace