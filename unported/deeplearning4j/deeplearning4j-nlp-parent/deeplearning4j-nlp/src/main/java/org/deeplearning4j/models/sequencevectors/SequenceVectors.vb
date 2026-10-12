Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports AtomicDouble = org.nd4j.shade.guava.util.concurrent.AtomicDouble
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.learning
Imports org.deeplearning4j.models.embeddings.learning
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.learning.impl.sequence
Imports org.deeplearning4j.models.embeddings.learning.impl.sequence
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports org.deeplearning4j.models.embeddings.reader
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports org.deeplearning4j.models.embeddings.wordvectors
Imports ListenerEvent = org.deeplearning4j.models.sequencevectors.enums.ListenerEvent
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.models.sequencevectors


	<Serializable>
	Public Class SequenceVectors(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Inherits WordVectorsImpl(Of T)
		Implements WordVectors

		Private Const serialVersionUID As Long = 78249242142L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient org.deeplearning4j.models.sequencevectors.interfaces.SequenceIterator<T> iterator;
		<NonSerialized>
		Protected Friend iterator As SequenceIterator(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected transient org.deeplearning4j.models.embeddings.learning.ElementsLearningAlgorithm<T> elementsLearningAlgorithm;
		<NonSerialized>
		Protected Friend elementsLearningAlgorithm As ElementsLearningAlgorithm(Of T)
		<NonSerialized>
		Protected Friend sequenceLearningAlgorithm As SequenceLearningAlgorithm(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.embeddings.loader.VectorsConfiguration configuration = new org.deeplearning4j.models.embeddings.loader.VectorsConfiguration();
		Protected Friend configuration As New VectorsConfiguration()

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(SequenceVectors))

		<NonSerialized>
		Protected Friend existingModel As WordVectors
		<NonSerialized>
		Protected Friend intersectModel As WordVectors
		<NonSerialized>
		Protected Friend unknownElement As T
		<NonSerialized>
		Protected Friend scoreElements As New AtomicDouble(0.0)
		<NonSerialized>
		Protected Friend scoreSequences As New AtomicDouble(0.0)
		<NonSerialized>
		Protected Friend configured As Boolean = False
		<NonSerialized>
		Protected Friend lockFactor As Boolean = False

		Protected Friend enableScavenger As Boolean = False
		Protected Friend vocabLimit As Integer = 0

		Private batchSequences As BatchSequences(Of T)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected transient @Set<org.deeplearning4j.models.sequencevectors.interfaces.VectorsListener<T>> eventListeners;
		<NonSerialized>
		Protected Friend eventListeners As ISet(Of VectorsListener(Of T))

		Public Overrides Property UNK As String Implements WordVectors.getUNK
			Get
				Return configuration.getUNK()
			End Get
			Set(ByVal UNK As String)
				configuration.setUNK(UNK)
				MyBase.UNK = UNK
			End Set
		End Property


		Public Overridable ReadOnly Property ElementsScore As Double
			Get
				Return scoreElements.get()
			End Get
		End Property

		Public Overridable ReadOnly Property SequencesScore As Double
			Get
				Return scoreSequences.get()
			End Get
		End Property


		Public Overrides Function getWordVectorMatrix(ByVal word As String) As INDArray Implements WordVectors.getWordVectorMatrix
			If configuration.isUseUnknown() AndAlso Not hasWord(word) Then
				Return MyBase.getWordVectorMatrix(UNK)
			Else
				Return MyBase.getWordVectorMatrix(word)
			End If
		End Function

		''' <summary>
		''' Builds vocabulary from provided SequenceIterator instance
		''' </summary>
		Public Overridable Sub buildVocab()


			Dim constructor As val = (New VocabConstructor.Builder(Of T)()).addSource(iterator, minWordFrequency).setTargetVocabCache(vocab_Conflict).fetchLabels(trainSequenceVectors).setStopWords(stopWords).enableScavenger(enableScavenger).setEntriesLimit(vocabLimit).allowParallelTokenization(configuration.isAllowParallelTokenization()).setUnk(If(useUnknown AndAlso unknownElement IsNot Nothing, unknownElement, Nothing)).build()

			If existingModel IsNot Nothing AndAlso TypeOf lookupTable_Conflict Is InMemoryLookupTable AndAlso TypeOf existingModel.lookupTable() Is InMemoryLookupTable Then
				log.info("Merging existing vocabulary into the current one...")
	'            
	'                if we have existing model defined, we're forced to fetch labels only.
	'                the rest of vocabulary & weights should be transferred from existing model
	'             

				constructor.buildMergedVocabulary(existingModel, True)

	'            
	'                Now we have vocab transferred, and we should transfer syn0 values into lookup table
	'             
				CType(lookupTable_Conflict, InMemoryLookupTable(Of VocabWord)).consume(CType(existingModel.lookupTable(), InMemoryLookupTable(Of VocabWord)))
			Else
				log.info("Starting vocabulary building...")
				' if we don't have existing model defined, we just build vocabulary


				constructor.buildJointVocabulary(False, True)

	'            
	'            if (useUnknown && unknownElement != null && !vocab.containsWord(unknownElement.getLabel())) {
	'                log.info("Adding UNK element...");
	'                unknownElement.setSpecial(true);
	'                unknownElement.markAsLabel(false);
	'                unknownElement.setIndex(vocab.numWords());
	'                vocab.addToken(unknownElement);
	'            }
	'            


				' check for malformed inputs. if numWords/numSentences ratio is huge, then user is passing something weird
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				If vocab_Conflict.numWords() / constructor.getNumberOfSequences() > 1000 Then
					log.warn("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!")
					log.warn("!                                                                                       !")
					log.warn("! Your input looks malformed: number of sentences is too low, model accuracy may suffer !")
					log.warn("!                                                                                       !")
					log.warn("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!")
				End If
			End If


		End Sub


		Protected Friend Overridable Sub initLearners()
			SyncLock Me
				If Not configured Then
					log.info("Building learning algorithms:")
					If trainElementsVectors AndAlso elementsLearningAlgorithm IsNot Nothing AndAlso Not trainSequenceVectors Then
						log.info("          building ElementsLearningAlgorithm: [" & elementsLearningAlgorithm.CodeName & "]")
						elementsLearningAlgorithm.configure(vocab_Conflict, lookupTable_Conflict, configuration)
						elementsLearningAlgorithm.pretrain(iterator)
					End If
					If trainSequenceVectors AndAlso sequenceLearningAlgorithm IsNot Nothing Then
						log.info("          building SequenceLearningAlgorithm: [" & sequenceLearningAlgorithm.CodeName & "]")
						sequenceLearningAlgorithm.configure(vocab_Conflict, lookupTable_Conflict, configuration)
						sequenceLearningAlgorithm.pretrain(Me.iterator)
        
						' we'll use the ELA compatible with selected SLA
						If trainElementsVectors Then
							elementsLearningAlgorithm = sequenceLearningAlgorithm.getElementsLearningAlgorithm()
							log.info("          building ElementsLearningAlgorithm: [" & elementsLearningAlgorithm.CodeName & "]")
						End If
					End If
					configured = True
				End If
			End SyncLock
		End Sub

		Private Sub initIntersectVectors()
			If intersectModel IsNot Nothing AndAlso intersectModel.vocab().numWords() > 0 Then
				Dim indexes As IList(Of Integer) = New List(Of Integer)()
				Dim i As Integer = 0
				Do While i < intersectModel.vocab().numWords()
					Dim externalWord As String = intersectModel.vocab().wordAtIndex(i)
					Dim index As Integer = Me.vocab_Conflict.indexOf(externalWord)
					If index >= 0 Then
						Me.vocab_Conflict.wordFor(externalWord).setLocked(lockFactor)
						indexes.Add(index)
					End If
					i += 1
				Loop

				If indexes.Count > 0 Then
					Dim intersectIndexes() As Integer = Ints.toArray(indexes)

					Nd4j.scatterUpdate(org.nd4j.linalg.api.ops.impl.scatter.ScatterUpdate.UpdateOp.ASSIGN, CType(lookupTable_Conflict, InMemoryLookupTable(Of VocabWord)).getSyn0(), Nd4j.createFromArray(intersectIndexes), CType(intersectModel.lookupTable(), InMemoryLookupTable(Of VocabWord)).getSyn0(), 1)
				End If
			End If
		End Sub

		''' <summary>
		''' Starts training over
		''' </summary>
		Public Overridable Sub fit()
			Dim props As val = Nd4j.Executioner.EnvironmentInformation
			If props.getProperty("backend").Equals("CUDA") Then
				If Nd4j.AffinityManager.NumberOfDevices > 1 Then
					Throw New System.InvalidOperationException("Multi-GPU word2vec/doc2vec isn't available atm")
				End If
				'if (!NativeOpsHolder.getInstance().getDeviceNativeOps().isP2PAvailable())
				'throw new IllegalStateException("Running Word2Vec on multi-gpu system requires P2P support between GPUs, which looks to be unavailable on your system.");
			End If

			Nd4j.Random.setSeed(configuration.getSeed())

			Dim timeSpent As New AtomicLong(0)
			If Not trainElementsVectors AndAlso Not trainSequenceVectors Then
				Throw New System.InvalidOperationException("You should define at least one training goal 'trainElementsRepresentation' or 'trainSequenceRepresentation'")
			End If
			If iterator Is Nothing Then
				Throw New System.InvalidOperationException("You can't fit() data without SequenceIterator defined")
			End If

			If resetModel OrElse (lookupTable_Conflict IsNot Nothing AndAlso vocab_Conflict IsNot Nothing AndAlso vocab_Conflict.numWords() = 0) Then
				' build vocabulary from scratches
				buildVocab()
			End If

			WordVectorSerializer.printOutProjectedMemoryUse(vocab_Conflict.numWords(), configuration.getLayersSize(),If(configuration.isUseHierarchicSoftmax() AndAlso configuration.getNegative() > 0, 3, 2))

			If vocab_Conflict Is Nothing OrElse lookupTable_Conflict Is Nothing OrElse vocab_Conflict.numWords() = 0 Then
				Throw New System.InvalidOperationException("You can't fit() model with empty Vocabulary or WeightLookupTable")
			End If

			' if model vocab and lookupTable is built externally we basically should check that lookupTable was properly initialized
			If Not resetModel OrElse existingModel IsNot Nothing Then
				lookupTable_Conflict.resetWeights(False)
			Else
				' otherwise we reset weights, independent of actual current state of lookup table
				lookupTable_Conflict.resetWeights(True)

				' if preciseWeights used, we roll over data once again
				If configuration.isPreciseWeightInit() Then
					log.info("Using precise weights init...")
					iterator.reset()

					Do While iterator.hasMoreSequences()
						Dim sequence As val = iterator.nextSequence()

						' initializing elements, only once
						For Each element As T In sequence.getElements()
							Dim realElement As T = vocab_Conflict.tokenFor(element.getLabel())

							If realElement IsNot Nothing AndAlso Not realElement.isInit() Then
								Dim rng As val = Nd4j.RandomFactory.getNewRandomInstance(configuration.getSeed() * realElement.GetHashCode(), configuration.getLayersSize() + 1)

								Dim randArray As val = Nd4j.rand(New Integer() {1, configuration.getLayersSize()}, rng).subi(0.5).divi(configuration.getLayersSize())

								lookupTable_Conflict.Weights.getRow(realElement.getIndex(), True).assign(randArray)
								realElement.setInit(True)
							End If
						Next element

						' initializing labels, only once
						For Each label As T In sequence.getSequenceLabels()
							Dim realElement As T = vocab_Conflict.tokenFor(label.getLabel())

							If realElement IsNot Nothing AndAlso Not realElement.isInit() Then
								Dim rng As Random = Nd4j.RandomFactory.getNewRandomInstance(configuration.getSeed() * realElement.GetHashCode(), configuration.getLayersSize() + 1)
								Dim randArray As INDArray = Nd4j.rand(New Integer() {1, configuration.getLayersSize()}, rng).subi(0.5).divi(configuration.getLayersSize())

								lookupTable_Conflict.Weights.getRow(realElement.getIndex(), True).assign(randArray)
								realElement.setInit(True)
							End If
						Next label
					Loop

					Me.iterator.reset()
				End If
			End If

			initLearners()
			initIntersectVectors()

			log.info("Starting learning process...")
			timeSpent.set(DateTimeHelper.CurrentUnixTimeMillis())
			If Me.stopWords Is Nothing Then
				Me.stopWords = New List(Of String)()
			End If

			Dim wordsCounter As val = New AtomicLong(0)
			Dim currentEpoch As Integer = 1
			Do While currentEpoch <= numEpochs
				Dim linesCounter As val = New AtomicLong(0)


				Dim sequencer As val = New AsyncSequencer(Me, Me.iterator, Me.stopWords)
				sequencer.start()

				Dim timer As val = New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis())
				Dim thread As val = New VectorCalculationsThread(Me, 0, currentEpoch, wordsCounter, vocab_Conflict.totalWordOccurrences(), linesCounter, sequencer, timer, numEpochs)
				thread.start()

				Try
					sequencer.join()
				Catch e As Exception
					Throw New Exception(e)
				End Try

				Try
				   thread.join()
				Catch e As Exception
					Throw New Exception(e)
				End Try

				' TODO: fix this to non-exclusive termination
				If trainElementsVectors AndAlso elementsLearningAlgorithm IsNot Nothing AndAlso (Not trainSequenceVectors OrElse sequenceLearningAlgorithm Is Nothing) AndAlso elementsLearningAlgorithm.EarlyTerminationHit Then
					Exit Do
				End If

				If trainSequenceVectors AndAlso sequenceLearningAlgorithm IsNot Nothing AndAlso (Not trainElementsVectors OrElse elementsLearningAlgorithm Is Nothing) AndAlso sequenceLearningAlgorithm.EarlyTerminationHit Then
					Exit Do
				End If
				log.info("Epoch [" & currentEpoch & "] finished; Elements processed so far: [" & wordsCounter.get() & "];  Sequences processed: [" & linesCounter.get() & "]")

				If eventListeners IsNot Nothing AndAlso eventListeners.Count > 0 Then
					For Each listener As VectorsListener In eventListeners
						If listener.validateEvent(ListenerEvent.EPOCH, currentEpoch) Then
							listener.processEvent(ListenerEvent.EPOCH, Me, currentEpoch)
						End If
					Next listener
				End If
				currentEpoch += 1
			Loop

			log.info("Time spent on training: {} ms", DateTimeHelper.CurrentUnixTimeMillis() - timeSpent.get())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void trainSequence(@NonNull Sequence<T> sequence, java.util.concurrent.atomic.AtomicLong nextRandom, double alpha)
		Protected Friend Overridable Sub trainSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal alpha As Double)

			If sequence.getElements().isEmpty() Then
				Return
			End If

	'        
	'            we do NOT train elements separately if sequnceLearningAlgorithm isn't CBOW
	'            we skip that, because PV-DM includes CBOW
	'          
			If trainElementsVectors AndAlso Not (trainSequenceVectors AndAlso TypeOf sequenceLearningAlgorithm Is DM) Then
				' call for ElementsLearningAlgorithm
				nextRandom.set(nextRandom.get() * 25214903917L + 11)
				If Not elementsLearningAlgorithm.EarlyTerminationHit Then
						scoreElements.set(elementsLearningAlgorithm.learnSequence(sequence, nextRandom, alpha, batchSequences))
				Else
					scoreElements.set(elementsLearningAlgorithm.learnSequence(sequence, nextRandom, alpha))
				End If
			End If

			If trainSequenceVectors Then
				' call for SequenceLearningAlgorithm
				nextRandom.set(nextRandom.get() * 25214903917L + 11)
				If Not sequenceLearningAlgorithm.EarlyTerminationHit Then
					scoreSequences.set(sequenceLearningAlgorithm.learnSequence(sequence, nextRandom, alpha, batchSequences))
				End If
			End If
		End Sub


		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER NOTE: The field vocabCache was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend vocabCache_Conflict As VocabCache(Of T)
'JAVA TO VB CONVERTER NOTE: The field lookupTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend lookupTable_Conflict As WeightLookupTable(Of T)
			Protected Friend iterator As SequenceIterator(Of T)
'JAVA TO VB CONVERTER NOTE: The field modelUtils was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend modelUtils_Conflict As ModelUtils(Of T) = New BasicModelUtils(Of T)()

			Protected Friend existingVectors As WordVectors
			Protected Friend intersectVectors As SequenceVectors(Of T)
			Protected Friend lockFactor As Boolean = False

'JAVA TO VB CONVERTER NOTE: The field sampling was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend sampling_Conflict As Double = 0
			Protected Friend negative As Double = 0
'JAVA TO VB CONVERTER NOTE: The field learningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend learningRate_Conflict As Double = 0.025
'JAVA TO VB CONVERTER NOTE: The field minLearningRate was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minLearningRate_Conflict As Double = 0.0001
'JAVA TO VB CONVERTER NOTE: The field minWordFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minWordFrequency_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field iterations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend iterations_Conflict As Integer = 1
			Protected Friend numEpochs As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field layerSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend layerSize_Conflict As Integer = 100
			Protected Friend window As Integer = 5
			Protected Friend hugeModelExpected As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field batchSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend batchSize_Conflict As Integer = 512
			Protected Friend learningRateDecayWords As Integer
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seed_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend useAdaGrad_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field resetModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend resetModel_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field workers was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend workers_Conflict As Integer = Runtime.getRuntime().availableProcessors()
'JAVA TO VB CONVERTER NOTE: The field useUnknown was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend useUnknown_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field useHierarchicSoftmax was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend useHierarchicSoftmax_Conflict As Boolean = True
			Protected Friend variableWindows() As Integer

			Protected Friend trainSequenceVectors As Boolean = False
			Protected Friend trainElementsVectors As Boolean = True

			Protected Friend preciseWeightInit As Boolean = False

'JAVA TO VB CONVERTER NOTE: The field stopWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend stopWords_Conflict As ICollection(Of String) = New List(Of String)()

			Protected Friend configuration As New VectorsConfiguration()

'JAVA TO VB CONVERTER NOTE: The field unknownElement was renamed since Visual Basic does not allow fields to have the same name as other class members:
			<NonSerialized>
			Protected Friend unknownElement_Conflict As T
			Protected Friend UNK As String = configuration.getUNK()
			Protected Friend [STOP] As String = configuration.getSTOP()

'JAVA TO VB CONVERTER NOTE: The field enableScavenger was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend enableScavenger_Conflict As Boolean = False
			Protected Friend vocabLimit As Integer

			''' <summary>
			''' Experimental field. Switches on precise mode for batch operations.
			''' </summary>
			Protected Friend preciseMode As Boolean = False

			' defaults values for learning algorithms are set here
'JAVA TO VB CONVERTER NOTE: The field elementsLearningAlgorithm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend elementsLearningAlgorithm_Conflict As ElementsLearningAlgorithm(Of T) = New SkipGram(Of T)()
'JAVA TO VB CONVERTER NOTE: The field sequenceLearningAlgorithm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend sequenceLearningAlgorithm_Conflict As SequenceLearningAlgorithm(Of T) = New DBOW(Of T)()

'JAVA TO VB CONVERTER NOTE: The field vectorsListeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend vectorsListeners_Conflict As ISet(Of VectorsListener(Of T)) = New HashSet(Of VectorsListener(Of T))()

			Public Sub New()

			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VectorsConfiguration configuration)
			Public Sub New(ByVal configuration As VectorsConfiguration)
				Me.configuration = configuration
				Me.iterations_Conflict = configuration.getIterations()
				Me.numEpochs = configuration.getEpochs()
				Me.minLearningRate_Conflict = configuration.getMinLearningRate()
				Me.learningRate_Conflict = configuration.getLearningRate()
				Me.sampling_Conflict = configuration.getSampling()
				Me.negative = configuration.getNegative()
				Me.minWordFrequency_Conflict = configuration.getMinWordFrequency()
				Me.seed_Conflict = configuration.getSeed()
				Me.hugeModelExpected = configuration.isHugeModelExpected()
				Me.batchSize_Conflict = configuration.getBatchSize()
				Me.layerSize_Conflict = configuration.getLayersSize()
				Me.learningRateDecayWords = configuration.getLearningRateDecayWords()
				Me.useAdaGrad_Conflict = configuration.isUseAdaGrad()
				Me.window = configuration.getWindow()
				Me.UNK = configuration.getUNK()
				Me.STOP = configuration.getSTOP()
				Me.variableWindows = configuration.getVariableWindows()
				Me.useHierarchicSoftmax_Conflict = configuration.isUseHierarchicSoftmax()
				Me.preciseMode = configuration.isPreciseMode()

				Dim modelUtilsClassName As String = configuration.getModelUtils()
				If StringUtils.isNotEmpty(modelUtilsClassName) Then
					Try
						Me.modelUtils_Conflict = DL4JClassLoading.createNewInstance(modelUtilsClassName)
					Catch instantiationException As Exception
						log.error("Got '{}' trying to instantiate ModelUtils, falling back to BasicModelUtils instead", instantiationException.Message, instantiationException)
						Me.modelUtils_Conflict = New BasicModelUtils(Of T)()
					End Try
				End If

				If configuration.getElementsLearningAlgorithm() IsNot Nothing AndAlso Not configuration.getElementsLearningAlgorithm().isEmpty() Then
					Me.elementsLearningAlgorithm(configuration.getElementsLearningAlgorithm())
				End If

				If configuration.getSequenceLearningAlgorithm() IsNot Nothing AndAlso Not configuration.getSequenceLearningAlgorithm().isEmpty() Then
					Me.sequenceLearningAlgorithm(configuration.getSequenceLearningAlgorithm())
				End If

				If configuration.getStopList() IsNot Nothing Then
					Me.stopWords_Conflict.addAll(configuration.getStopList())
				End If
			End Sub

			''' <summary>
			''' This method allows you to use pre-built WordVectors model (e.g. SkipGram) for DBOW sequence learning.
			''' Existing model will be transferred into new model before training starts.
			''' 
			''' PLEASE NOTE: This model has no effect for elements learning algorithms. Only sequence learning is affected.
			''' PLEASE NOTE: Non-normalized model is recommended to use here.
			''' </summary>
			''' <param name="vec"> existing WordVectors model
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected Builder<T> useExistingWordVectors(@NonNull WordVectors vec)
			Protected Friend Overridable Function useExistingWordVectors(ByVal vec As WordVectors) As Builder(Of T)
				Me.existingVectors = vec
				Return Me
			End Function

			''' <summary>
			''' This method defines SequenceIterator to be used for model building </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> iterate(@NonNull SequenceIterator<T> iterator)
			Public Overridable Function iterate(ByVal iterator As SequenceIterator(Of T)) As Builder(Of T)
				Me.iterator = iterator
				Return Me
			End Function

			''' <summary>
			''' Sets specific LearningAlgorithm as Sequence Learning Algorithm
			''' </summary>
			''' <param name="algoName"> fully qualified class name
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> sequenceLearningAlgorithm(@NonNull String algoName)
			Public Overridable Function sequenceLearningAlgorithm(ByVal algoName As String) As Builder(Of T)
				Me.sequenceLearningAlgorithm_Conflict = DL4JClassLoading.createNewInstance(algoName)
				Return Me
			End Function

			''' <summary>
			''' Sets specific LearningAlgorithm as Sequence Learning Algorithm
			''' </summary>
			''' <param name="algorithm"> SequenceLearningAlgorithm implementation
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> sequenceLearningAlgorithm(@NonNull SequenceLearningAlgorithm<T> algorithm)
			Public Overridable Function sequenceLearningAlgorithm(ByVal algorithm As SequenceLearningAlgorithm(Of T)) As Builder(Of T)
				Me.sequenceLearningAlgorithm_Conflict = algorithm
				Return Me
			End Function

			''' <summary>
			''' * Sets specific LearningAlgorithm as Elements Learning Algorithm
			''' </summary>
			''' <param name="algoName"> fully qualified class name
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> elementsLearningAlgorithm(@NonNull String algoName)
			Public Overridable Function elementsLearningAlgorithm(ByVal algoName As String) As Builder(Of T)
				Me.elementsLearningAlgorithm_Conflict = DL4JClassLoading.createNewInstance(algoName)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.configuration.setElementsLearningAlgorithm(elementsLearningAlgorithm_Conflict.GetType().FullName)

				Return Me
			End Function

			''' <summary>
			''' * Sets specific LearningAlgorithm as Elements Learning Algorithm
			''' </summary>
			''' <param name="algorithm"> ElementsLearningAlgorithm implementation
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> elementsLearningAlgorithm(@NonNull ElementsLearningAlgorithm<T> algorithm)
			Public Overridable Function elementsLearningAlgorithm(ByVal algorithm As ElementsLearningAlgorithm(Of T)) As Builder(Of T)
				Me.elementsLearningAlgorithm_Conflict = algorithm
				Return Me
			End Function

			''' <summary>
			''' This method defines batchSize option, viable only if iterations > 1
			''' </summary>
			''' <param name="batchSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter batchSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function batchSize(ByVal batchSize_Conflict As Integer) As Builder(Of T)
				Me.batchSize_Conflict = batchSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines how much iterations should be done over batched sequences.
			''' </summary>
			''' <param name="iterations">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter iterations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function iterations(ByVal iterations_Conflict As Integer) As Builder(Of T)
				Me.iterations_Conflict = iterations_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines how much iterations should be done over whole training corpus during modelling </summary>
			''' <param name="numEpochs">
			''' @return </param>
			Public Overridable Function epochs(ByVal numEpochs As Integer) As Builder(Of T)
				Me.numEpochs = numEpochs
				Return Me
			End Function

			''' <summary>
			''' Sets number of worker threads to be used in calculations
			''' </summary>
			''' <param name="numWorkers">
			''' @return </param>
			Public Overridable Function workers(ByVal numWorkers As Integer) As Builder(Of T)
				Me.workers_Conflict = numWorkers
				Return Me
			End Function

			''' <summary>
			''' Enable/disable hierarchic softmax
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function useHierarchicSoftmax(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.useHierarchicSoftmax_Conflict = reallyUse
				Return Me
			End Function

			''' <summary>
			''' This method defines if Adaptive Gradients should be used in calculations
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			<Obsolete>
			Public Overridable Function useAdaGrad(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.useAdaGrad_Conflict = reallyUse
				Return Me
			End Function

			''' <summary>
			''' This method defines number of dimensions for outcome vectors.
			''' Please note: This option has effect only if lookupTable wasn't defined during building process.
			''' </summary>
			''' <param name="layerSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter layerSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layerSize(ByVal layerSize_Conflict As Integer) As Builder(Of T)
				Me.layerSize_Conflict = layerSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines initial learning rate.
			''' Default value is 0.025
			''' </summary>
			''' <param name="learningRate">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter learningRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function learningRate(ByVal learningRate_Conflict As Double) As Builder(Of T)
				Me.learningRate_Conflict = learningRate_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines minimal element frequency for elements found in the training corpus. All elements with frequency below this threshold will be removed before training.
			''' Please note: this method has effect only if vocabulary is built internally.
			''' </summary>
			''' <param name="minWordFrequency">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter minWordFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minWordFrequency(ByVal minWordFrequency_Conflict As Integer) As Builder(Of T)
				Me.minWordFrequency_Conflict = minWordFrequency_Conflict
				Return Me
			End Function


			''' <summary>
			''' This method sets vocabulary limit during construction.
			''' 
			''' Default value: 0. Means no limit
			''' </summary>
			''' <param name="limit">
			''' @return </param>
			Public Overridable Function limitVocabularySize(ByVal limit As Integer) As Builder
				If limit < 0 Then
					Throw New DL4JInvalidConfigException("Vocabulary limit should be non-negative number")
				End If

				Me.vocabLimit = limit
				Return Me
			End Function

			''' <summary>
			''' This method defines minimum learning rate after decay being applied.
			''' Default value is 0.01
			''' </summary>
			''' <param name="minLearningRate">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter minLearningRate was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minLearningRate(ByVal minLearningRate_Conflict As Double) As Builder(Of T)
				Me.minLearningRate_Conflict = minLearningRate_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines, should all model be reset before training. If set to true, vocabulary and WeightLookupTable will be reset before training, and will be built from scratches
			''' </summary>
			''' <param name="reallyReset">
			''' @return </param>
			Public Overridable Function resetModel(ByVal reallyReset As Boolean) As Builder(Of T)
				Me.resetModel_Conflict = reallyReset
				Return Me
			End Function

			''' <summary>
			''' You can pass externally built vocabCache object, containing vocabulary
			''' </summary>
			''' <param name="vocabCache">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> vocabCache(@NonNull VocabCache<T> vocabCache)
'JAVA TO VB CONVERTER NOTE: The parameter vocabCache was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function vocabCache(ByVal vocabCache_Conflict As VocabCache(Of T)) As Builder(Of T)
				Me.vocabCache_Conflict = vocabCache_Conflict
				Return Me
			End Function

			''' <summary>
			''' You can pass externally built WeightLookupTable, containing model weights and vocabulary.
			''' </summary>
			''' <param name="lookupTable">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> lookupTable(@NonNull WeightLookupTable<T> lookupTable)
'JAVA TO VB CONVERTER NOTE: The parameter lookupTable was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lookupTable(ByVal lookupTable_Conflict As WeightLookupTable(Of T)) As Builder(Of T)
				Me.lookupTable_Conflict = lookupTable_Conflict

				Me.layerSize(lookupTable_Conflict.layerSize())

				Return Me
			End Function

			''' <summary>
			''' This method defines sub-sampling threshold.
			''' </summary>
			''' <param name="sampling">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter sampling was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sampling(ByVal sampling_Conflict As Double) As Builder(Of T)
				Me.sampling_Conflict = sampling_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method defines negative sampling value for skip-gram algorithm.
			''' </summary>
			''' <param name="negative">
			''' @return </param>
			Public Overridable Function negativeSample(ByVal negative As Double) As Builder(Of T)
				Me.negative = negative
				Return Me
			End Function

			''' <summary>
			'''  You can provide collection of objects to be ignored, and excluded out of model
			'''  Please note: Object labels and hashCode will be used for filtering
			''' </summary>
			''' <param name="stopList">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> stopWords(@NonNull List<String> stopList)
			Public Overridable Function stopWords(ByVal stopList As IList(Of String)) As Builder(Of T)
				Me.stopWords_Conflict.addAll(stopList)
				Return Me
			End Function

			''' 
			''' <param name="trainElements">
			''' @return </param>
			Public Overridable Function trainElementsRepresentation(ByVal trainElements As Boolean) As Builder(Of T)
				Me.trainElementsVectors = trainElements
				Return Me
			End Function

			Public Overridable Function trainSequencesRepresentation(ByVal trainSequences As Boolean) As Builder(Of T)
				Me.trainSequenceVectors = trainSequences
				Return Me
			End Function

			''' <summary>
			''' You can provide collection of objects to be ignored, and excluded out of model
			''' Please note: Object labels and hashCode will be used for filtering
			''' </summary>
			''' <param name="stopList">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> stopWords(@NonNull Collection<T> stopList)
			Public Overridable Function stopWords(ByVal stopList As ICollection(Of T)) As Builder(Of T)
				For Each word As T In stopList
					Me.stopWords_Conflict.Add(word.getLabel())
				Next word
				Return Me
			End Function

			''' <summary>
			''' Sets window size for skip-Gram training
			''' </summary>
			''' <param name="windowSize">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter windowSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function windowSize(ByVal windowSize_Conflict As Integer) As Builder(Of T)
				Me.window = windowSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Sets seed for random numbers generator.
			''' Please note: this has effect only if vocabulary and WeightLookupTable is built internally
			''' </summary>
			''' <param name="randomSeed">
			''' @return </param>
			Public Overridable Function seed(ByVal randomSeed As Long) As Builder(Of T)
				' has no effect in original w2v actually
				Me.seed_Conflict = randomSeed
				Return Me
			End Function

			''' <summary>
			''' ModelUtils implementation, that will be used to access model.
			''' Methods like: similarity, wordsNearest, accuracy are provided by user-defined ModelUtils
			''' </summary>
			''' <param name="modelUtils"> model utils to be used
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> modelUtils(@NonNull ModelUtils<T> modelUtils)
'JAVA TO VB CONVERTER NOTE: The parameter modelUtils was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function modelUtils(ByVal modelUtils_Conflict As ModelUtils(Of T)) As Builder(Of T)
				Me.modelUtils_Conflict = modelUtils_Conflict
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.configuration.setModelUtils(modelUtils_Conflict.GetType().FullName)
				Return Me
			End Function

			''' <summary>
			''' This method allows you to specify, if UNK word should be used internally </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function useUnknown(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.useUnknown_Conflict = reallyUse
				Me.configuration.setUseUnknown(reallyUse)
				Return Me
			End Function

			''' <summary>
			''' This method allows you to specify SequenceElement that will be used as UNK element, if UNK is used </summary>
			''' <param name="element">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> unknownElement(@NonNull T element)
			Public Overridable Function unknownElement(ByVal element As T) As Builder(Of T)
				Me.unknownElement_Conflict = element
				Me.UNK = element.getLabel()
				Me.configuration.setUNK(Me.UNK)
				Return Me
			End Function

			''' <summary>
			''' This method allows to use variable window size. In this case, every batch gets processed using one of predefined window sizes
			''' </summary>
			''' <param name="windows">
			''' @return </param>
			Public Overridable Function useVariableWindow(ParamArray ByVal windows() As Integer) As Builder(Of T)
				If windows Is Nothing OrElse windows.Length = 0 Then
					Throw New System.InvalidOperationException("Variable windows can't be empty")
				End If

				variableWindows = windows

				Return Me
			End Function

			''' <summary>
			''' If set to true, initial weights for elements/sequences will be derived from elements themself.
			''' However, this implies additional cycle through input iterator.
			''' 
			''' Default value: FALSE
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overridable Function usePreciseWeightInit(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.preciseWeightInit = reallyUse
				Me.configuration.setPreciseWeightInit(reallyUse)
				Return Me
			End Function

			Public Overridable Function usePreciseMode(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.preciseMode = reallyUse
				Me.configuration.setPreciseMode(reallyUse)
				Return Me
			End Function

			''' <summary>
			''' This method creates new WeightLookupTable<T> and VocabCache<T> if there were none set
			''' </summary>
			Protected Friend Overridable Sub presetTables()
				If lookupTable_Conflict Is Nothing Then

					If vocabCache_Conflict Is Nothing Then
						vocabCache_Conflict = (New AbstractCache.Builder(Of T)()).hugeModelExpected(hugeModelExpected).scavengerRetentionDelay(Me.configuration.getScavengerRetentionDelay()).scavengerThreshold(Me.configuration.getScavengerActivationThreshold()).minElementFrequency(minWordFrequency_Conflict).build()
					End If

					lookupTable_Conflict = (New InMemoryLookupTable.Builder(Of T)()).useAdaGrad(Me.useAdaGrad_Conflict).cache(vocabCache_Conflict).negative(negative).useHierarchicSoftmax(useHierarchicSoftmax_Conflict).vectorLength(layerSize_Conflict).lr(learningRate_Conflict).seed(seed_Conflict).build()
				End If

				Dim elementsLearningAlgorithm As String = Me.configuration.getElementsLearningAlgorithm()
				If StringUtils.isNotEmpty(elementsLearningAlgorithm) Then
					Me.elementsLearningAlgorithm_Conflict = DL4JClassLoading.createNewInstance(elementsLearningAlgorithm)
				End If

				Dim sequenceLearningAlgorithm As String = Me.configuration.getSequenceLearningAlgorithm()
				If StringUtils.isNotEmpty(sequenceLearningAlgorithm) Then
					Me.sequenceLearningAlgorithm_Conflict = DL4JClassLoading.createNewInstance(sequenceLearningAlgorithm)
				End If

				If trainElementsVectors AndAlso Me.elementsLearningAlgorithm_Conflict Is Nothing Then
					' create default implementation of ElementsLearningAlgorithm
					Me.elementsLearningAlgorithm_Conflict = New SkipGram(Of T)()
				End If

				If trainSequenceVectors AndAlso Me.sequenceLearningAlgorithm_Conflict Is Nothing Then
					Me.sequenceLearningAlgorithm_Conflict = New DBOW(Of T)()
				End If

				Me.modelUtils_Conflict.init(lookupTable_Conflict)
			End Sub



			''' <summary>
			''' This method sets VectorsListeners for this SequenceVectors model
			''' </summary>
			''' <param name="listeners">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setVectorsListeners(@NonNull Collection<org.deeplearning4j.models.sequencevectors.interfaces.VectorsListener<T>> listeners)
			Public Overridable Function setVectorsListeners(ByVal listeners As ICollection(Of VectorsListener(Of T))) As Builder(Of T)
				vectorsListeners_Conflict.addAll(listeners)
				Return Me
			End Function

			''' <summary>
			''' This method ebables/disables periodical vocab truncation during construction
			''' 
			''' Default value: disabled
			''' </summary>
			''' <param name="reallyEnable">
			''' @return </param>
			Public Overridable Function enableScavenger(ByVal reallyEnable As Boolean) As Builder(Of T)
				Me.enableScavenger_Conflict = reallyEnable
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> intersectModel(@NonNull SequenceVectors<T> intersectVectors, boolean lockFactor)
			Public Overridable Function intersectModel(ByVal intersectVectors As SequenceVectors(Of T), ByVal lockFactor As Boolean) As Builder(Of T)
				Me.intersectVectors = intersectVectors
				Me.lockFactor = lockFactor
				Return Me
			End Function

			''' <summary>
			''' Build SequenceVectors instance with defined settings/options
			''' @return
			''' </summary>
			Public Overridable Function build() As SequenceVectors(Of T)
				presetTables()

				Dim vectors As New SequenceVectors(Of T)()

				If Me.existingVectors IsNot Nothing Then
					Me.trainElementsVectors = False
					Me.elementsLearningAlgorithm_Conflict = Nothing
				End If

				vectors.numEpochs = Me.numEpochs
				vectors.numIterations = Me.iterations_Conflict
				vectors.vocab_Conflict = Me.vocabCache_Conflict
				vectors.minWordFrequency = Me.minWordFrequency_Conflict
				vectors.learningRate.set(Me.learningRate_Conflict)
				vectors.minLearningRate = Me.minLearningRate_Conflict
				vectors.sampling = Me.sampling_Conflict
				vectors.negative = Me.negative
				vectors.layerSize_Conflict = Me.layerSize_Conflict
				vectors.batchSize = Me.batchSize_Conflict
				vectors.learningRateDecayWords = Me.learningRateDecayWords
				vectors.window = Me.window
				vectors.resetModel = Me.resetModel_Conflict
				vectors.useAdeGrad = Me.useAdaGrad_Conflict
				vectors.stopWords = Me.stopWords_Conflict
				vectors.workers = Me.workers_Conflict

				vectors.iterator = Me.iterator
				vectors.lookupTable_Conflict = Me.lookupTable_Conflict
				vectors.modelUtils_Conflict = Me.modelUtils_Conflict
				vectors.useUnknown = Me.useUnknown_Conflict
				vectors.unknownElement = Me.unknownElement_Conflict
				vectors.variableWindows = Me.variableWindows
				vectors.vocabLimit = Me.vocabLimit


				vectors.trainElementsVectors = Me.trainElementsVectors
				vectors.trainSequenceVectors = Me.trainSequenceVectors

				vectors.elementsLearningAlgorithm = Me.elementsLearningAlgorithm_Conflict
				vectors.sequenceLearningAlgorithm = Me.sequenceLearningAlgorithm_Conflict

				vectors.existingModel = Me.existingVectors
				vectors.intersectModel = Me.intersectVectors
				vectors.enableScavenger = Me.enableScavenger_Conflict
				vectors.lockFactor = Me.lockFactor

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

				vectors.configuration = Me.configuration

				Return vectors
			End Function
		End Class

		''' <summary>
		''' This class is used to fetch data from iterator in background thread, and convert it to List<VocabularyWord>
		''' 
		''' It becomes very usefull if text processing pipeline behind iterator is complex, and we're not loading data from simple text file with whitespaces as separator.
		''' Since this method allows you to hide preprocessing latency in background.
		''' 
		''' This mechanics will be change to PrefetchingSentenceIterator wrapper.
		''' </summary>
		Protected Friend Class AsyncSequencer
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As SequenceVectors(Of T)

			Friend ReadOnly iterator As SequenceIterator(Of T)
			Friend ReadOnly buffer As LinkedBlockingQueue(Of Sequence(Of T))
			'     private final AtomicLong linesCounter;
			Friend ReadOnly limitUpper As Integer
			Friend ReadOnly limitLower As Integer
			Friend isRunning As New AtomicBoolean(True)
			Friend nextRandom As AtomicLong
			Friend stopList As ICollection(Of String)

			Friend Const DEFAULT_BUFFER_SIZE As Integer = 512

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncSequencer(org.deeplearning4j.models.sequencevectors.interfaces.SequenceIterator<T> iterator, @NonNull Collection<String> stopList)
			Public Sub New(ByVal outerInstance As SequenceVectors(Of T), ByVal iterator As SequenceIterator(Of T), ByVal stopList As ICollection(Of String))
				Me.outerInstance = outerInstance
				Me.iterator = iterator
				'            this.linesCounter = linesCounter;
				Me.setName("AsyncSequencer thread")
				Me.nextRandom = New AtomicLong(outerInstance.workers + 1)
				Me.iterator.reset()
				Me.stopList = stopList
				Me.setDaemon(True)

				limitLower = outerInstance.workers * (If(outerInstance.batchSize < DEFAULT_BUFFER_SIZE, DEFAULT_BUFFER_SIZE, outerInstance.batchSize))
				limitUpper = limitLower * 2

				Me.buffer = New LinkedBlockingQueue(Of Sequence(Of T))(limitUpper)
			End Sub

			' Preserve order of input sequences to gurantee order of output tokens
			Public Overrides Sub run()
				isRunning.set(True)
				Do While Me.iterator.hasMoreSequences()

					' if buffered level is below limitLower, we're going to fetch limitUpper number of strings from fetcher
					If buffer.size() < limitLower Then
						outerInstance.update()
						Dim linesLoaded As New AtomicInteger(0)
						Do While linesLoaded.getAndIncrement() < limitUpper AndAlso Me.iterator.hasMoreSequences()
							Dim document As Sequence(Of T) = Me.iterator.nextSequence()

	'                        
	'                            We can't hope/assume that underlying iterator contains synchronized elements
	'                            That's why we're going to rebuild sequence from vocabulary
	'                          
							Dim newSequence As New Sequence(Of T)()

							If document.SequenceLabel IsNot Nothing Then
								Dim newLabel As T = outerInstance.vocab_Conflict.wordFor(document.SequenceLabel.getLabel())
								If newLabel IsNot Nothing Then
									newSequence.SequenceLabel = newLabel
								End If
							End If

							For Each element As T In document.getElements()
								If stopList.Contains(element.getLabel()) Then
									Continue For
								End If
								Dim realElement As T = outerInstance.vocab_Conflict.wordFor(element.getLabel())

								' please note: this serquence element CAN be absent in vocab, due to minFreq or stopWord or whatever else
								If realElement IsNot Nothing Then
									newSequence.addElement(realElement)
								ElseIf outerInstance.useUnknown AndAlso outerInstance.unknownElement IsNot Nothing Then
									newSequence.addElement(outerInstance.unknownElement)
								End If
							Next element

							' due to subsampling and null words, new sequence size CAN be 0, so there's no need to insert empty sequence into processing chain
							If newSequence.getElements().Count > 0 Then
								Try
									buffer.put(newSequence)
								Catch e As InterruptedException
									Thread.CurrentThread.Interrupt()
									Throw New Exception(e)
								End Try
							End If

							linesLoaded.incrementAndGet()
						Loop
					Else
						ThreadUtils.uncheckedSleep(50)
					End If
				Loop
				isRunning.set(False)
			End Sub

			Public Overridable Function hasMoreLines() As Boolean
				' statement order does matter here, since there's possible race condition
				Return Not buffer.isEmpty() OrElse isRunning.get()
			End Function

			Public Overridable Function nextSentence() As Sequence(Of T)
				Try
					Return buffer.poll(3L, TimeUnit.SECONDS)
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					Return Nothing
				End Try
			End Function
		End Class

		''' <summary>
		''' VectorCalculationsThreads are used for vector calculations, and work together with AsyncIteratorDigitizer.
		''' Basically, all they do is just transfer of digitized sentences into math layer.
		''' 
		''' Please note, they do not iterate the sentences over and over, each sentence processed only once.
		''' Training corpus iteration is implemented in fit() method.
		''' 
		''' </summary>
		Private Class VectorCalculationsThread
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As SequenceVectors(Of T)

			Friend ReadOnly threadId As Integer
			Friend ReadOnly epochNumber As Integer
			Friend ReadOnly wordsCounter As AtomicLong
			Friend ReadOnly totalWordsCount As Long
			Friend ReadOnly totalLines As AtomicLong

			Friend ReadOnly digitizer As AsyncSequencer
			Friend ReadOnly nextRandom As AtomicLong
			Friend ReadOnly timer As AtomicLong
			Friend ReadOnly startTime As Long
			Friend ReadOnly totalEpochs As Integer

	'        
	'                Long constructors suck, so this should be reduced to something reasonable later
	'         
			Public Sub New(ByVal outerInstance As SequenceVectors(Of T), ByVal threadId As Integer, ByVal epoch As Integer, ByVal wordsCounter As AtomicLong, ByVal totalWordsCount As Long, ByVal linesCounter As AtomicLong, ByVal digitizer As AsyncSequencer, ByVal timer As AtomicLong, ByVal totalEpochs As Integer)
				Me.outerInstance = outerInstance
				Me.threadId = threadId
				Me.totalEpochs = totalEpochs
				Me.epochNumber = epoch
				Me.wordsCounter = wordsCounter
				Me.totalWordsCount = totalWordsCount
				Me.totalLines = linesCounter
				Me.digitizer = digitizer
				Me.timer = timer
				Me.startTime = timer.get()
				Me.nextRandom = New AtomicLong(Me.threadId)
				Me.setName("VectorCalculationsThread " & Me.threadId)
			End Sub

			Public Overrides Sub run()
				' small workspace, just to handle
				Dim conf As val = WorkspaceConfiguration.builder().policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(3).initialSize(25L * 1024L * 1024L).build()
				Dim workspace_id As val = "sequence_vectors_training_" & System.Guid.randomUUID().ToString()

				Nd4j.AffinityManager.getDeviceForCurrentThread()
				Do While digitizer.hasMoreLines()
					Try
						' get current sentence as list of VocabularyWords
						Dim sequences As IList(Of Sequence(Of T)) = New List(Of Sequence(Of T))()
						For x As Integer = 0 To outerInstance.batchSize - 1
							If digitizer.hasMoreLines() Then
								Dim sequence As Sequence(Of T) = digitizer.nextSentence()
								If sequence IsNot Nothing Then
									sequences.Add(sequence)
								End If
							End If
						Next x
						Dim alpha As Double = 0.025

						If sequences.Count = 0 Then
							Continue Do
						End If

						' getting back number of iterations
						For i As Integer = 0 To outerInstance.numIterations - 1

							outerInstance.batchSequences = New BatchSequences(Of T)(outerInstance.configuration.getBatchSize())
							' we roll over sequences derived from digitizer, it's NOT window loop
							For x As Integer = 0 To sequences.Count - 1
								Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(conf, workspace_id)
									Dim sequence As Sequence(Of T) = sequences(x)

									'log.info("LR before: {}; wordsCounter: {}; totalWordsCount: {}", learningRate.get(), this.wordsCounter.get(), this.totalWordsCount);
									alpha = Math.Max(outerInstance.minLearningRate, outerInstance.learningRate.get() * (1 - (1.0 * Me.wordsCounter.get() / (CDbl(Me.totalWordsCount)) / (outerInstance.numIterations * totalEpochs))))

									outerInstance.trainSequence(sequence, nextRandom, alpha)

									' increment processed word count, please note: this affects learningRate decay
									totalLines.incrementAndGet()
									Me.wordsCounter.addAndGet(sequence.getElements().Count)

									If totalLines.get() Mod 100000 = 0 Then
										Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
										Dim timeSpent As Long = currentTime - timer.get()

										timer.set(currentTime)
										Dim totalTimeSpent As Long = currentTime - startTime

										Dim seqSec As Double = (100000.0 / (CDbl(timeSpent) / 1000.0))
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
										Dim wordsSecTotal As Double = Me.wordsCounter.get() / (CDbl(totalTimeSpent) / 1000.0)

										log.info("Epoch: [{}]; Words vectorized so far: [{}];  Lines vectorized so far: [{}]; Seq/sec: [{}]; Words/sec: [{}]; learningRate: [{}]", Me.epochNumber, Me.wordsCounter.get(), Me.totalLines.get(), String.Format("{0:F2}", seqSec), String.Format("{0:F2}", wordsSecTotal), alpha)
									End If
									If outerInstance.eventListeners IsNot Nothing AndAlso outerInstance.eventListeners.Count > 0 Then
										For Each listener As VectorsListener In outerInstance.eventListeners
											If listener.validateEvent(ListenerEvent.LINE, totalLines.get()) Then
												listener.processEvent(ListenerEvent.LINE, outerInstance, totalLines.get())
											End If
										Next listener
									End If
								End Using
							Next x

							If TypeOf outerInstance.elementsLearningAlgorithm Is SkipGram Then
								CType(outerInstance.elementsLearningAlgorithm, SkipGram).setWorkers(outerInstance.workers)
							ElseIf TypeOf outerInstance.elementsLearningAlgorithm Is CBOW Then
								CType(outerInstance.elementsLearningAlgorithm, CBOW).setWorkers(outerInstance.workers)
							End If

							Dim batchSize As Integer = outerInstance.configuration.getBatchSize()
							If batchSize > 1 AndAlso outerInstance.batchSequences IsNot Nothing Then
								Dim rest As Integer = outerInstance.batchSequences.size() Mod batchSize
								Dim chunks As Integer = (If(outerInstance.batchSequences.size() >= batchSize, outerInstance.batchSequences.size() \ batchSize, 0)) + (If(rest > 0, 1, 0))
								For j As Integer = 0 To chunks - 1
									If outerInstance.trainElementsVectors Then
										If TypeOf outerInstance.elementsLearningAlgorithm Is SkipGram Then
											CType(outerInstance.elementsLearningAlgorithm, SkipGram).iterateSample(outerInstance.batchSequences.get(j))
										ElseIf TypeOf outerInstance.elementsLearningAlgorithm Is CBOW Then
											CType(outerInstance.elementsLearningAlgorithm, CBOW).iterateSample(outerInstance.batchSequences.get(j))
										End If
									End If

									If outerInstance.trainSequenceVectors Then
										If TypeOf outerInstance.sequenceLearningAlgorithm Is DBOW Then
											CType(outerInstance.sequenceLearningAlgorithm.getElementsLearningAlgorithm(), SkipGram(Of T)).iterateSample(outerInstance.batchSequences.get(j))
										ElseIf TypeOf outerInstance.sequenceLearningAlgorithm Is DM Then
											CType(outerInstance.sequenceLearningAlgorithm.getElementsLearningAlgorithm(), CBOW(Of T)).iterateSample(outerInstance.batchSequences.get(j))
										End If
									End If
								Next j
								outerInstance.batchSequences.clear()
								outerInstance.batchSequences = Nothing
							End If

							If outerInstance.eventListeners IsNot Nothing AndAlso outerInstance.eventListeners.Count > 0 Then
								For Each listener As VectorsListener In outerInstance.eventListeners
									If listener.validateEvent(ListenerEvent.ITERATION, i) Then
										listener.processEvent(ListenerEvent.ITERATION, outerInstance, i)
									End If
								Next listener
							End If
						Next i


					Catch e As Exception
						Throw New Exception(e)
					End Try
				Loop

				If outerInstance.trainElementsVectors Then
					outerInstance.elementsLearningAlgorithm.finish()
				End If

				If outerInstance.trainSequenceVectors Then
					outerInstance.sequenceLearningAlgorithm.finish()
				End If
			End Sub
		End Class
	End Class

End Namespace