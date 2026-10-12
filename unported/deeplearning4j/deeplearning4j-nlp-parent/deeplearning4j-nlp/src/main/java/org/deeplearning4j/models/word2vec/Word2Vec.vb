Imports System
Imports System.Collections.Generic
Imports JsonObject = com.google.gson.JsonObject
Imports JsonParser = com.google.gson.JsonParser
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.learning
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.embeddings.reader
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports org.deeplearning4j.models.sequencevectors
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.iterators
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports DocumentIterator = org.deeplearning4j.text.documentiterator.DocumentIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports StreamLineIterator = org.deeplearning4j.text.sentenceiterator.StreamLineIterator
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
Imports CollectionType = org.nd4j.shade.jackson.databind.type.CollectionType

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

Namespace org.deeplearning4j.models.word2vec


	<Serializable>
	Public Class Word2Vec
		Inherits SequenceVectors(Of VocabWord)

		Private Const serialVersionUID As Long = 78249242142L

		<NonSerialized>
		Protected Friend sentenceIter As SentenceIterator
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory tokenizerFactory;
'JAVA TO VB CONVERTER NOTE: The field tokenizerFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend tokenizerFactory_Conflict As TokenizerFactory

		''' <summary>
		''' This method defines TokenizerFactory instance to be using during model building
		''' </summary>
		''' <param name="tokenizerFactory"> TokenizerFactory instance </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setTokenizerFactory(@NonNull TokenizerFactory tokenizerFactory)
		Public Overridable WriteOnly Property TokenizerFactory As TokenizerFactory
			Set(ByVal tokenizerFactory As TokenizerFactory)
				Me.tokenizerFactory_Conflict = tokenizerFactory
    
				If sentenceIter IsNot Nothing Then
					Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(sentenceIter).tokenizerFactory(Me.tokenizerFactory_Conflict).build()
					Me.iterator = (New AbstractSequenceIterator.Builder(Of )(transformer)).build()
				End If
			End Set
		End Property

		''' <summary>
		''' This method defines SentenceIterator instance, that will be used as training corpus source
		''' </summary>
		''' <param name="iterator"> SentenceIterator instance </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setSentenceIterator(@NonNull SentenceIterator iterator)
		Public Overridable WriteOnly Property SentenceIterator As SentenceIterator
			Set(ByVal iterator As SentenceIterator)
				'if (tokenizerFactory == null) throw new IllegalStateException("Please call setTokenizerFactory() prior to setSentenceIter() call.");
    
				If tokenizerFactory_Conflict IsNot Nothing Then
					Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(iterator).tokenizerFactory(tokenizerFactory_Conflict).allowMultithreading(configuration Is Nothing OrElse configuration.isAllowParallelTokenization()).build()
					Me.iterator = (New AbstractSequenceIterator.Builder(Of )(transformer)).build()
				Else
					log.error("Please call setTokenizerFactory() prior to setSentenceIter() call.")
				End If
			End Set
		End Property

		''' <summary>
		''' This method defines SequenceIterator instance, that will be used as training corpus source.
		''' Main difference with other iterators here: it allows you to pass already tokenized Sequence<VocabWord> for training
		''' </summary>
		''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setSequenceIterator(@NonNull SequenceIterator<VocabWord> iterator)
		Public Overridable WriteOnly Property SequenceIterator As SequenceIterator(Of VocabWord)
			Set(ByVal iterator As SequenceIterator(Of VocabWord))
				Me.iterator = iterator
			End Set
		End Property

'JAVA TO VB CONVERTER NOTE: The field mapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared mapper_Conflict As ObjectMapper = Nothing
		Private Shared ReadOnly lock As New Object()

		Private Shared Function mapper() As ObjectMapper
			If mapper_Conflict Is Nothing Then
				SyncLock lock
					If mapper_Conflict Is Nothing Then
						mapper_Conflict = New ObjectMapper()
						mapper_Conflict.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
						mapper_Conflict.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
						Return mapper_Conflict
					End If
				End SyncLock
			End If
			Return mapper_Conflict
		End Function

		Private Const CLASS_FIELD As String = "@class"
		Private Const VOCAB_LIST_FIELD As String = "VocabCache"

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String toJson() throws org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overridable Function toJson() As String

			Dim retVal As New JsonObject()
			Dim mapper As ObjectMapper = Word2Vec.mapper()

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			retVal.addProperty(CLASS_FIELD, mapper.writeValueAsString(Me.GetType().FullName))

			If TypeOf Me.vocab_Conflict Is AbstractCache Then
				retVal.addProperty(VOCAB_LIST_FIELD, CType(Me.vocab_Conflict, AbstractCache(Of VocabWord)).toJson())
			End If

			Return retVal.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Word2Vec fromJson(String jsonString) throws java.io.IOException
		Public Shared Function fromJson(ByVal jsonString As String) As Word2Vec

			Dim ret As New Word2Vec()

			Dim parser As New JsonParser()
			Dim json As JsonObject = parser.parse(jsonString).getAsJsonObject()

			Dim cache As VocabCache = AbstractCache.fromJson(json.get(VOCAB_LIST_FIELD).getAsString())

			ret.Vocab = cache
			Return ret
		End Function

		Public Class Builder
			Inherits SequenceVectors.Builder(Of VocabWord)

			Protected Friend sentenceIterator As SentenceIterator
			Protected Friend labelAwareIterator As LabelAwareIterator
'JAVA TO VB CONVERTER NOTE: The field tokenizerFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend tokenizerFactory_Conflict As TokenizerFactory
'JAVA TO VB CONVERTER NOTE: The field allowParallelTokenization was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend allowParallelTokenization_Conflict As Boolean = True


			Public Sub New()

			End Sub

			''' <summary>
			''' This method has no effect for Word2Vec
			''' </summary>
			''' <param name="vec"> existing WordVectors model
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override protected Builder useExistingWordVectors(@NonNull WordVectors vec)
			Protected Friend Overrides Function useExistingWordVectors(ByVal vec As WordVectors) As Builder
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VectorsConfiguration configuration)
			Public Sub New(ByVal configuration As VectorsConfiguration)
				MyBase.New(configuration)
				Me.allowParallelTokenization_Conflict = configuration.isAllowParallelTokenization()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterate(@NonNull DocumentIterator iterator)
			Public Overridable Overloads Function iterate(ByVal iterator As DocumentIterator) As Builder
				Me.sentenceIterator = (New StreamLineIterator.Builder(iterator)).setFetchSize(100).build()
				Return Me
			End Function

			''' <summary>
			''' This method used to feed SentenceIterator, that contains training corpus, into ParagraphVectors
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterate(@NonNull SentenceIterator iterator)
			Public Overridable Overloads Function iterate(ByVal iterator As SentenceIterator) As Builder
				Me.sentenceIterator = iterator
				Return Me
			End Function

			''' <summary>
			''' This method defines TokenizerFactory to be used for strings tokenization during training
			''' PLEASE NOTE: If external VocabCache is used, the same TokenizerFactory should be used to keep derived tokens equal.
			''' </summary>
			''' <param name="tokenizerFactory">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder tokenizerFactory(@NonNull TokenizerFactory tokenizerFactory)
'JAVA TO VB CONVERTER NOTE: The parameter tokenizerFactory was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tokenizerFactory(ByVal tokenizerFactory_Conflict As TokenizerFactory) As Builder
				Me.tokenizerFactory_Conflict = tokenizerFactory_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method used to feed SequenceIterator, that contains training corpus, into ParagraphVectors
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder iterate(@NonNull SequenceIterator<VocabWord> iterator)
			Public Overrides Function iterate(ByVal iterator As SequenceIterator(Of VocabWord)) As Builder
				MyBase.iterate(iterator)
				Return Me
			End Function

			''' <summary>
			''' This method used to feed LabelAwareIterator, that is usually used
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterate(@NonNull LabelAwareIterator iterator)
			Public Overridable Overloads Function iterate(ByVal iterator As LabelAwareIterator) As Builder
				Me.labelAwareIterator = iterator
				Return Me
			End Function

			''' <summary>
			''' This method defines mini-batch size </summary>
			''' <param name="batchSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter batchSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function batchSize(ByVal batchSize_Conflict As Integer) As Builder
				MyBase.batchSize(batchSize_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines number of iterations done for each mini-batch during training </summary>
			''' <param name="iterations">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter iterations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function iterations(ByVal iterations_Conflict As Integer) As Builder
				MyBase.iterations(iterations_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines number of epochs (iterations over whole training corpus) for training </summary>
			''' <param name="numEpochs">
			''' @return </param>
			Public Overrides Function epochs(ByVal numEpochs As Integer) As Builder
				MyBase.epochs(numEpochs)
				Return Me
			End Function

			''' <summary>
			''' This method defines number of dimensions for output vectors </summary>
			''' <param name="layerSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter layerSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function layerSize(ByVal layerSize_Conflict As Integer) As Builder
				MyBase.layerSize(layerSize_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines initial learning rate for model training
			''' </summary>
			''' <param name="learningRate">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter learningRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function learningRate(ByVal learningRate_Conflict As Double) As Builder
				MyBase.learningRate(learningRate_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines minimal word frequency in training corpus. All words below this threshold will be removed prior model training
			''' </summary>
			''' <param name="minWordFrequency">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter minWordFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function minWordFrequency(ByVal minWordFrequency_Conflict As Integer) As Builder
				MyBase.minWordFrequency(minWordFrequency_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines minimal learning rate value for training
			''' </summary>
			''' <param name="minLearningRate">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter minLearningRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function minLearningRate(ByVal minLearningRate_Conflict As Double) As Builder
				MyBase.minLearningRate(minLearningRate_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines whether model should be totally wiped out prior building, or not
			''' </summary>
			''' <param name="reallyReset">
			''' @return </param>
			Public Overrides Function resetModel(ByVal reallyReset As Boolean) As Builder
				MyBase.resetModel(reallyReset)
				Return Me
			End Function

			''' <summary>
			''' This method sets vocabulary limit during construction.
			''' 
			''' Default value: 0. Means no limit
			''' </summary>
			''' <param name="limit">
			''' @return </param>
			Public Overrides Function limitVocabularySize(ByVal limit As Integer) As Builder
				MyBase.limitVocabularySize(limit)
				Return Me
			End Function

			''' <summary>
			''' This method allows to define external VocabCache to be used
			''' </summary>
			''' <param name="vocabCache">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder vocabCache(@NonNull VocabCache<VocabWord> vocabCache)
'JAVA TO VB CONVERTER NOTE: The parameter vocabCache was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function vocabCache(ByVal vocabCache_Conflict As VocabCache(Of VocabWord)) As Builder
				MyBase.vocabCache(vocabCache_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method allows to define external WeightLookupTable to be used
			''' </summary>
			''' <param name="lookupTable">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder lookupTable(@NonNull WeightLookupTable<VocabWord> lookupTable)
'JAVA TO VB CONVERTER NOTE: The parameter lookupTable was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function lookupTable(ByVal lookupTable_Conflict As WeightLookupTable(Of VocabWord)) As Builder
				MyBase.lookupTable(lookupTable_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines whether subsampling should be used or not
			''' </summary>
			''' <param name="sampling"> set > 0 to subsampling argument, or 0 to disable
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter sampling was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function sampling(ByVal sampling_Conflict As Double) As Builder
				MyBase.sampling(sampling_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines whether adaptive gradients should be used or not
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overrides Function useAdaGrad(ByVal reallyUse As Boolean) As Builder
				MyBase.useAdaGrad(reallyUse)
				Return Me
			End Function

			''' <summary>
			''' This method defines whether negative sampling should be used or not
			''' 
			''' PLEASE NOTE: If you're going to use negative sampling, you might want to disable HierarchicSoftmax, which is enabled by default
			''' 
			''' Default value: 0
			''' </summary>
			''' <param name="negative"> set > 0 as negative sampling argument, or 0 to disable
			''' @return </param>
			Public Overrides Function negativeSample(ByVal negative As Double) As Builder
				MyBase.negativeSample(negative)
				Return Me
			End Function

			''' <summary>
			''' This method defines stop words that should be ignored during training
			''' </summary>
			''' <param name="stopList">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder stopWords(@NonNull List<String> stopList)
			Public Overrides Function stopWords(ByVal stopList As IList(Of String)) As Builder
				MyBase.stopWords(stopList)
				Return Me
			End Function

			''' <summary>
			''' This method is hardcoded to TRUE, since that's whole point of Word2Vec
			''' </summary>
			''' <param name="trainElements">
			''' @return </param>
			Public Overrides Function trainElementsRepresentation(ByVal trainElements As Boolean) As Builder
				Throw New System.InvalidOperationException("You can't change this option for Word2Vec")
			End Function

			''' <summary>
			''' This method is hardcoded to FALSE, since that's whole point of Word2Vec
			''' </summary>
			''' <param name="trainSequences">
			''' @return </param>
			Public Overrides Function trainSequencesRepresentation(ByVal trainSequences As Boolean) As Builder
				Throw New System.InvalidOperationException("You can't change this option for Word2Vec")
			End Function

			''' <summary>
			''' This method defines stop words that should be ignored during training
			''' </summary>
			''' <param name="stopList">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder stopWords(@NonNull Collection<VocabWord> stopList)
			Public Overrides Function stopWords(ByVal stopList As ICollection(Of VocabWord)) As Builder
				MyBase.stopWords(stopList)
				Return Me
			End Function

			''' <summary>
			''' This method defines context window size
			''' </summary>
			''' <param name="windowSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter windowSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function windowSize(ByVal windowSize_Conflict As Integer) As Builder
				MyBase.windowSize(windowSize_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method defines random seed for random numbers generator </summary>
			''' <param name="randomSeed">
			''' @return </param>
			Public Overrides Function seed(ByVal randomSeed As Long) As Builder
				MyBase.seed(randomSeed)
				Return Me
			End Function

			''' <summary>
			''' This method defines maximum number of concurrent threads available for training
			''' </summary>
			''' <param name="numWorkers">
			''' @return </param>
			Public Overrides Function workers(ByVal numWorkers As Integer) As Builder
				MyBase.workers(numWorkers)
				Return Me
			End Function

			''' <summary>
			''' Sets ModelUtils that gonna be used as provider for utility methods: similarity(), wordsNearest(), accuracy(), etc
			''' </summary>
			''' <param name="modelUtils"> model utils to be used
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder modelUtils(@NonNull ModelUtils<VocabWord> modelUtils)
'JAVA TO VB CONVERTER NOTE: The parameter modelUtils was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function modelUtils(ByVal modelUtils_Conflict As ModelUtils(Of VocabWord)) As Builder
				MyBase.modelUtils(modelUtils_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method allows to use variable window size. In this case, every batch gets processed using one of predefined window sizes
			''' </summary>
			''' <param name="windows">
			''' @return </param>
			Public Overrides Function useVariableWindow(ParamArray ByVal windows() As Integer) As Builder
				MyBase.useVariableWindow(windows)
				Return Me
			End Function

			''' <summary>
			''' This method allows you to specify SequenceElement that will be used as UNK element, if UNK is used
			''' </summary>
			''' <param name="element">
			''' @return </param>
			Public Overrides Function unknownElement(ByVal element As VocabWord) As Builder
				MyBase.unknownElement(element)
				Return Me
			End Function

			''' <summary>
			''' This method allows you to specify, if UNK word should be used internally
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overrides Function useUnknown(ByVal reallyUse As Boolean) As Builder
				MyBase.useUnknown(reallyUse)
				If Me.unknownElement_Conflict Is Nothing Then
					Me.unknownElement(New VocabWord(1.0, Word2Vec.DEFAULT_UNK))
				End If
				Return Me
			End Function

			''' <summary>
			''' This method sets VectorsListeners for this SequenceVectors model
			''' </summary>
			''' <param name="vectorsListeners">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder setVectorsListeners(@NonNull Collection<org.deeplearning4j.models.sequencevectors.interfaces.VectorsListener<VocabWord>> vectorsListeners)
			Public Overrides Function setVectorsListeners(ByVal vectorsListeners As ICollection(Of VectorsListener(Of VocabWord))) As Builder
				MyBase.VectorsListeners = vectorsListeners
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder elementsLearningAlgorithm(@NonNull String algorithm)
			Public Overrides Function elementsLearningAlgorithm(ByVal algorithm As String) As Builder
				MyBase.elementsLearningAlgorithm(algorithm)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder elementsLearningAlgorithm(@NonNull ElementsLearningAlgorithm<VocabWord> algorithm)
			Public Overrides Function elementsLearningAlgorithm(ByVal algorithm As ElementsLearningAlgorithm(Of VocabWord)) As Builder
				MyBase.elementsLearningAlgorithm(algorithm)
				Return Me
			End Function

			''' <summary>
			''' This method enables/disables parallel tokenization.
			''' 
			''' Default value: TRUE </summary>
			''' <param name="allow">
			''' @return </param>
			Public Overridable Function allowParallelTokenization(ByVal allow As Boolean) As Builder
				Me.allowParallelTokenization_Conflict = allow
				Return Me
			End Function

			''' <summary>
			''' This method ebables/disables periodical vocab truncation during construction
			''' 
			''' Default value: disabled
			''' </summary>
			''' <param name="reallyEnable">
			''' @return </param>
			Public Overrides Function enableScavenger(ByVal reallyEnable As Boolean) As Builder
				MyBase.enableScavenger(reallyEnable)
				Return Me
			End Function

			''' <summary>
			''' This method enables/disables Hierarchic softmax
			''' 
			''' Default value: enabled
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overrides Function useHierarchicSoftmax(ByVal reallyUse As Boolean) As Builder
				MyBase.useHierarchicSoftmax(reallyUse)
				Return Me
			End Function

			Public Overrides Function usePreciseWeightInit(ByVal reallyUse As Boolean) As Builder
				MyBase.usePreciseWeightInit(reallyUse)
				Return Me
			End Function

			Public Overrides Function usePreciseMode(ByVal reallyUse As Boolean) As Builder
				MyBase.usePreciseMode(reallyUse)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder intersectModel(@NonNull SequenceVectors vectors, boolean isLocked)
			Public Overrides Function intersectModel(ByVal vectors As SequenceVectors, ByVal isLocked As Boolean) As Builder
				MyBase.intersectModel(vectors, isLocked)
				Return Me
			End Function

			Public Overrides Function build() As Word2Vec
				presetTables()

				Dim ret As New Word2Vec()

				If sentenceIterator IsNot Nothing Then
					If tokenizerFactory_Conflict Is Nothing Then
						tokenizerFactory_Conflict = New DefaultTokenizerFactory()
					End If

					Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(sentenceIterator).tokenizerFactory(tokenizerFactory_Conflict).allowMultithreading(allowParallelTokenization_Conflict).build()
					Me.iterator = (New AbstractSequenceIterator.Builder(Of )(transformer)).build()
				End If

				If Me.labelAwareIterator IsNot Nothing Then
					If tokenizerFactory_Conflict Is Nothing Then
						tokenizerFactory_Conflict = New DefaultTokenizerFactory()
					End If

					Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(labelAwareIterator).tokenizerFactory(tokenizerFactory_Conflict).allowMultithreading(allowParallelTokenization_Conflict).build()
					Me.iterator = (New AbstractSequenceIterator.Builder(Of )(transformer)).build()
				End If

				ret.numEpochs = Me.numEpochs
				ret.numIterations = Me.iterations_Conflict
				ret.vocab_Conflict = Me.vocabCache_Conflict
				ret.minWordFrequency = Me.minWordFrequency_Conflict
				ret.learningRate.set(Me.learningRate_Conflict)
				ret.minLearningRate = Me.minLearningRate_Conflict
				ret.sampling = Me.sampling_Conflict
				ret.negative = Me.negative
				ret.layerSize_Conflict = Me.layerSize_Conflict
				ret.batchSize = Me.batchSize_Conflict
				ret.learningRateDecayWords = Me.learningRateDecayWords
				ret.window = Me.window
				ret.resetModel = Me.resetModel_Conflict
				ret.useAdeGrad = Me.useAdaGrad_Conflict
				ret.stopWords = Me.stopWords_Conflict
				ret.workers = Me.workers_Conflict
				ret.useUnknown = Me.useUnknown_Conflict
				ret.unknownElement = Me.unknownElement_Conflict
				ret.variableWindows = Me.variableWindows
				ret.seed = Me.seed_Conflict
				ret.enableScavenger = Me.enableScavenger_Conflict
				ret.vocabLimit = Me.vocabLimit

				If ret.unknownElement Is Nothing Then
					ret.unknownElement = New VocabWord(1.0,SequenceVectors.DEFAULT_UNK)
				End If


				ret.iterator = Me.iterator
				ret.lookupTable_Conflict = Me.lookupTable_Conflict
				ret.tokenizerFactory_Conflict = Me.tokenizerFactory_Conflict
				ret.modelUtils_Conflict = Me.modelUtils_Conflict

				ret.elementsLearningAlgorithm = Me.elementsLearningAlgorithm_Conflict
				ret.sequenceLearningAlgorithm = Me.sequenceLearningAlgorithm_Conflict

				ret.intersectModel = Me.intersectVectors
				ret.lockFactor = Me.lockFactor

				Me.configuration.setLearningRate(Me.learningRate_Conflict)
				Me.configuration.setLayersSize(layerSize_Conflict)
				Me.configuration.setHugeModelExpected(hugeModelExpected)
				Me.configuration.setWindow(window)
				Me.configuration.setMinWordFrequency(minWordFrequency_Conflict)
				Me.configuration.setIterations(iterations_Conflict)
				Me.configuration.setSeed(seed_Conflict)
				Me.configuration.setBatchSize(batchSize_Conflict)
				Me.configuration.setLearningRateDecayWords(learningRateDecayWords)
				Me.configuration.setMinLearningRate(minLearningRate_Conflict)
				Me.configuration.setSampling(Me.sampling_Conflict)
				Me.configuration.setUseAdaGrad(useAdaGrad_Conflict)
				Me.configuration.setNegative(negative)
				Me.configuration.setEpochs(Me.numEpochs)
				Me.configuration.setStopList(Me.stopWords_Conflict)
				Me.configuration.setVariableWindows(variableWindows)
				Me.configuration.setUseHierarchicSoftmax(Me.useHierarchicSoftmax_Conflict)
				Me.configuration.setPreciseWeightInit(Me.preciseWeightInit)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.configuration.setModelUtils(Me.modelUtils_Conflict.GetType().FullName)
				Me.configuration.setAllowParallelTokenization(Me.allowParallelTokenization_Conflict)
				Me.configuration.setPreciseMode(Me.preciseMode)

				If tokenizerFactory_Conflict IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					Me.configuration.setTokenizerFactory(tokenizerFactory_Conflict.GetType().FullName)
					If tokenizerFactory_Conflict.TokenPreProcessor IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
						Me.configuration.setTokenPreProcessor(tokenizerFactory_Conflict.TokenPreProcessor.GetType().FullName)
					End If
				End If

				ret.configuration = Me.configuration

				' we hardcode
				ret.trainSequenceVectors = False
				ret.trainElementsVectors = True

				ret.eventListeners = Me.vectorsListeners_Conflict


				Return ret
			End Function
		End Class
	End Class

End Namespace