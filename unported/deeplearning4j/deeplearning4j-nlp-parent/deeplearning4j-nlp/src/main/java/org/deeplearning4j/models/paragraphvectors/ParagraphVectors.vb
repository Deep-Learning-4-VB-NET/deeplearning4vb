Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports JsonObject = com.google.gson.JsonObject
Imports JsonParser = com.google.gson.JsonParser
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.learning
Imports org.deeplearning4j.models.embeddings.learning
Imports org.deeplearning4j.models.embeddings.learning.impl.sequence
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.embeddings.reader
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.iterators
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports org.deeplearning4j.text.documentiterator
Imports DocumentIteratorConverter = org.deeplearning4j.text.documentiterator.interoperability.DocumentIteratorConverter
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports SentenceIteratorConverter = org.deeplearning4j.text.sentenceiterator.interoperability.SentenceIteratorConverter
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
Imports PriorityScheduler = org.threadly.concurrent.PriorityScheduler
Imports TaskPriority = org.threadly.concurrent.TaskPriority

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


	<Serializable>
	Public Class ParagraphVectors
		Inherits Word2Vec

		Private Const serialVersionUID As Long = 78249242142L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected LabelsSource labelsSource;
		Protected Friend labelsSource As LabelsSource
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected transient LabelAwareIterator labelAwareIterator;
		<NonSerialized>
		Protected Friend labelAwareIterator As LabelAwareIterator
		Protected Friend labelsMatrix As INDArray
		Protected Friend labelsList As IList(Of VocabWord) = New List(Of VocabWord)()
		Protected Friend normalizedLabels As Boolean = False

		<NonSerialized>
		Protected Friend ReadOnly inferenceLocker As New Object()
		<NonSerialized>
		Protected Friend inferenceExecutor As PriorityScheduler
		<NonSerialized>
		Protected Friend countSubmitted As AtomicLong
		<NonSerialized>
		Protected Friend countFinished As AtomicLong

		Protected Friend Sub New()
			MyBase.New()
		End Sub

		Protected Friend Overridable Sub initInference()
			SyncLock Me
				If countSubmitted Is Nothing OrElse countFinished Is Nothing OrElse inferenceExecutor Is Nothing Then
					inferenceExecutor = New PriorityScheduler(Math.Max(Runtime.getRuntime().availableProcessors() - 2, 2), TaskPriority.High, 1000, New ThreadFactoryAnonymousInnerClass(Me))
					countSubmitted = New AtomicLong(0)
					countFinished = New AtomicLong(0)
				End If
			End SyncLock
		End Sub

		Private Class ThreadFactoryAnonymousInnerClass
			Inherits ThreadFactory

			Private ReadOnly outerInstance As ParagraphVectors

			Public Sub New(ByVal outerInstance As ParagraphVectors)
				Me.outerInstance = outerInstance
			End Sub

			Public Function newThread(ByVal r As ThreadStart) As Thread
				Dim t As Thread = Executors.defaultThreadFactory().newThread(r)
				t.setName("ParagraphVectors inference thread")
				t.setDaemon(True)
				Return t
			End Function
		End Class

		''' <summary>
		''' This method takes raw text, applies tokenizer, and returns most probable label
		''' </summary>
		''' <param name="rawText">
		''' @return </param>
		<Obsolete>
		Public Overridable Function predict(ByVal rawText As String) As String
			If tokenizerFactory_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("TokenizerFactory should be defined, prior to predict() call")
			End If

			Dim tokens As IList(Of String) = tokenizerFactory_Conflict.create(rawText).getTokens()
			Dim document As IList(Of VocabWord) = New List(Of VocabWord)()
			For Each token As String In tokens
				If vocab.containsWord(token) Then
					document.Add(vocab.wordFor(token))
				End If
			Next token

			Return predict(document)
		End Function

		''' <summary>
		''' This method defines SequenceIterator instance, that will be used as training corpus source.
		''' Main difference with other iterators here: it allows you to pass already tokenized Sequence<VocabWord> for training
		''' </summary>
		''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setSequenceIterator(@NonNull SequenceIterator<org.deeplearning4j.models.word2vec.VocabWord> iterator)
		Public Overridable Overloads WriteOnly Property SequenceIterator As SequenceIterator(Of VocabWord)
			Set(ByVal iterator As SequenceIterator(Of VocabWord))
				Me.iterator = iterator
			End Set
		End Property

		''' <summary>
		''' This method predicts label of the document.
		''' Computes a similarity wrt the mean of the
		''' representation of words in the document </summary>
		''' <param name="document"> the document </param>
		''' <returns> the word distances for each label </returns>
		Public Overridable Function predict(ByVal document As LabelledDocument) As String
			If document.getReferencedContent() IsNot Nothing Then
				Return predict(document.getReferencedContent())
			Else
				Return predict(document.getContent())
			End If
		End Function

		Public Overridable Sub extractLabels()
			Dim vocabWordCollection As ICollection(Of VocabWord) = vocab.vocabWords()
			Dim vocabWordList As IList(Of VocabWord) = New List(Of VocabWord)()
			Dim stringList As IList(Of String) = New List(Of String)()
			Dim indexArray() As Integer

			'INDArray pulledArray;
			'Check if word has label and build a list out of the collection
			For Each vWord As VocabWord In vocabWordCollection
				If vWord.Label Then
					vocabWordList.Add(vWord)
					stringList.Add(vWord.Label)
				End If
			Next vWord
			'Build array of indexes in the order of the vocablist
			indexArray = New Integer(vocabWordList.Count - 1){}
			Dim i As Integer = 0
			For Each vWord As VocabWord In vocabWordList
				indexArray(i) = vWord.Index
				i += 1
			Next vWord
			'pull the label rows and create new matrix
			If i > 0 Then
				labelsMatrix = Nd4j.pullRows(lookupTable.getWeights(), 1, indexArray)
				Me.labelsList = vocabWordList

				Me.labelsSource = New LabelsSource(stringList)
			End If
		End Sub

		''' <summary>
		''' This method calculates inferred vector for given text
		''' </summary>
		''' <param name="text">
		''' @return </param>
		Public Overridable Function inferVector(ByVal text As String, ByVal learningRate As Double, ByVal minLearningRate As Double, ByVal iterations As Integer) As INDArray
			If tokenizerFactory_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("TokenizerFactory should be defined, prior to predict() call")
			End If

			If Me.vocab Is Nothing OrElse Me.vocab.numWords() = 0 Then
				reassignExistingModel()
			End If

			Dim tokens As IList(Of String) = tokenizerFactory_Conflict.create(text).getTokens()
			Dim document As IList(Of VocabWord) = New List(Of VocabWord)()
			For Each token As String In tokens
				If vocab.containsWord(token) Then
					document.Add(vocab.wordFor(token))
				End If
			Next token

			If document.Count = 0 Then
				Throw New ND4JIllegalStateException("Text passed for inference has no matches in model vocabulary.")
			End If

			Return inferVector(document, learningRate, minLearningRate, iterations)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") protected synchronized void reassignExistingModel()
		Protected Friend Overridable Sub reassignExistingModel()
			SyncLock Me
				If (Me.vocab Is Nothing OrElse Me.vocab.numWords() = 0) AndAlso existingModel IsNot Nothing Then
					Me.vocab = existingModel.vocab()
					Me.lookupTable = existingModel.lookupTable()
				End If
        
			End SyncLock
		End Sub

		''' <summary>
		''' This method calculates inferred vector for given document
		''' </summary>
		''' <param name="document">
		''' @return </param>
		Public Overridable Function inferVector(ByVal document As LabelledDocument, ByVal learningRate As Double, ByVal minLearningRate As Double, ByVal iterations As Integer) As INDArray
			If document.getReferencedContent() IsNot Nothing AndAlso Not document.getReferencedContent().isEmpty() Then
				Return inferVector(document.getReferencedContent(), learningRate, minLearningRate, iterations)
			Else
				Return inferVector(document.getContent(), learningRate, minLearningRate, iterations)
			End If
		End Function

		''' <summary>
		''' This method calculates inferred vector for given document
		''' </summary>
		''' <param name="document">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray inferVector(@NonNull List<org.deeplearning4j.models.word2vec.VocabWord> document, double learningRate, double minLearningRate, int iterations)
		Public Overridable Function inferVector(ByVal document As IList(Of VocabWord), ByVal learningRate As Double, ByVal minLearningRate As Double, ByVal iterations As Integer) As INDArray

			If Me.vocab Is Nothing OrElse Me.vocab.numWords() = 0 Then
				reassignExistingModel()
			End If

			Dim learner As SequenceLearningAlgorithm(Of VocabWord) = sequenceLearningAlgorithm

			If learner Is Nothing Then
				SyncLock Me
					If sequenceLearningAlgorithm Is Nothing Then
						log.info("Creating new PV-DM learner...")
						learner = New DM(Of VocabWord)()
						learner.configure(vocab, lookupTable, configuration)
						sequenceLearningAlgorithm = learner
					Else
						learner = sequenceLearningAlgorithm
					End If
				End SyncLock
			End If

			learner = sequenceLearningAlgorithm



			If document.Count = 0 Then
				Throw New ND4JIllegalStateException("Impossible to apply inference to empty list of words")
			End If


			Dim sequence As New Sequence(Of VocabWord)()
			sequence.addElements(document)
			sequence.SequenceLabel = New VocabWord(1.0, ((New Random()).Next()).ToString())

			initLearners()

			Dim inf As INDArray = learner.inferSequence(sequence, seed, learningRate, minLearningRate, iterations)

			Return inf
		End Function

		''' <summary>
		''' This method calculates inferred vector for given text, with default parameters for learning rate and iterations
		''' </summary>
		''' <param name="text">
		''' @return </param>
		Public Overridable Function inferVector(ByVal text As String) As INDArray
			Return inferVector(text, Me.learningRate.get(), Me.minLearningRate, Me.numEpochs * Me.numIterations)
		End Function

		''' <summary>
		''' This method calculates inferred vector for given document, with default parameters for learning rate and iterations
		''' </summary>
		''' <param name="document">
		''' @return </param>
		Public Overridable Function inferVector(ByVal document As LabelledDocument) As INDArray
			Return inferVector(document, Me.learningRate.get(), Me.minLearningRate, Me.numEpochs * Me.numIterations)
		End Function

		''' <summary>
		''' This method calculates inferred vector for given list of words, with default parameters for learning rate and iterations
		''' </summary>
		''' <param name="document">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray inferVector(@NonNull List<org.deeplearning4j.models.word2vec.VocabWord> document)
		Public Overridable Function inferVector(ByVal document As IList(Of VocabWord)) As INDArray
			Return inferVector(document, Me.learningRate.get(), Me.minLearningRate, Me.numEpochs * Me.numIterations)
		End Function

		''' <summary>
		''' This method implements batched inference, based on Java Future parallelism model.
		''' 
		''' PLEASE NOTE: In order to use this method, LabelledDocument being passed in should have Id field defined.
		''' </summary>
		''' <param name="document">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Future<org.nd4j.common.primitives.Pair<String, org.nd4j.linalg.api.ndarray.INDArray>> inferVectorBatched(@NonNull LabelledDocument document)
		Public Overridable Function inferVectorBatched(ByVal document As LabelledDocument) As Future(Of Pair(Of String, INDArray))
			If countSubmitted Is Nothing Then
				initInference()
			End If

			If Me.vocab Is Nothing OrElse Me.vocab.numWords() = 0 Then
				reassignExistingModel()
			End If

			' we block execution until queued amount of documents gets below acceptable level, to avoid memory exhaust
			Do While countSubmitted.get() - countFinished.get() > 1024
				ThreadUtils.uncheckedSleep(50)
			Loop

			Dim callable As New InferenceCallable(Me, vocab, tokenizerFactory_Conflict, document)
			Dim future As Future(Of Pair(Of String, INDArray)) = inferenceExecutor.submit(callable)
			countSubmitted.incrementAndGet()

			Return future
		End Function

		''' <summary>
		''' This method implements batched inference, based on Java Future parallelism model.
		''' 
		''' PLEASE NOTE: This method will return you Future&lt;INDArray&gt;, so tracking relation between document and INDArray will be your responsibility
		''' </summary>
		''' <param name="document">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Future<org.nd4j.linalg.api.ndarray.INDArray> inferVectorBatched(@NonNull String document)
		Public Overridable Function inferVectorBatched(ByVal document As String) As Future(Of INDArray)
			If countSubmitted Is Nothing Then
				initInference()
			End If

			If Me.vocab Is Nothing OrElse Me.vocab.numWords() = 0 Then
				reassignExistingModel()
			End If

			' we block execution until queued amount of documents gets below acceptable level, to avoid memory exhaust
			Do While countSubmitted.get() - countFinished.get() > 1024
				ThreadUtils.uncheckedSleep(50)
			Loop

			Dim callable As New BlindInferenceCallable(Me, vocab, tokenizerFactory_Conflict, document)
			Dim future As Future(Of INDArray) = inferenceExecutor.submit(callable)
			countSubmitted.incrementAndGet()

			Return future
		End Function

		''' <summary>
		''' This method does inference on a given List&lt;String&gt; </summary>
		''' <param name="documents"> </param>
		''' <returns> INDArrays in the same order as input texts </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public List<org.nd4j.linalg.api.ndarray.INDArray> inferVectorBatched(@NonNull List<String> documents)
		Public Overridable Function inferVectorBatched(ByVal documents As IList(Of String)) As IList(Of INDArray)
			If countSubmitted Is Nothing Then
				initInference()
			End If

			If Me.vocab Is Nothing OrElse Me.vocab.numWords() = 0 Then
				reassignExistingModel()
			End If

			Dim futuresList As IList(Of Future(Of INDArray)) = New List(Of Future(Of INDArray))()
			Dim results As IList(Of INDArray) = New List(Of INDArray)()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong flag = new java.util.concurrent.atomic.AtomicLong(0);
			Dim flag As New AtomicLong(0)

			For i As Integer = 0 To documents.Count - 1
				Dim callable As New BlindInferenceCallable(Me, vocab, tokenizerFactory_Conflict, documents(i), flag)

				futuresList.Add(inferenceExecutor.submit(callable))
			Next i

			For i As Integer = 0 To documents.Count - 1
				Dim future As Future(Of INDArray) = futuresList(i)
				Try
					results.Add(future.get())
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					Throw New Exception(e)
				Catch e As ExecutionException
					Throw New Exception(e)
				End Try
			Next i

			Return results
		End Function

		''' <summary>
		''' This method predicts label of the document.
		''' Computes a similarity wrt the mean of the
		''' representation of words in the document </summary>
		''' <param name="document"> the document </param>
		''' <returns> the word distances for each label </returns>
		Public Overridable Function predict(ByVal document As IList(Of VocabWord)) As String
	'        
	'            This code was transferred from original ParagraphVectors DL4j implementation, and yet to be tested
	'         
			If document.Count = 0 Then
				Throw New System.InvalidOperationException("Document has no words inside")
			End If

	'        
	'        INDArray arr = Nd4j.create(document.size(), this.layerSize);
	'        for (int i = 0; i < document.size(); i++) {
	'            arr.putRow(i, getWordVectorMatrix(document.get(i).getWord()));
	'        }

			Dim docMean As INDArray = inferVector(document) 'arr.mean(0);
			Dim distances As New Counter(Of String)()

			For Each s As String In labelsSource.getLabels()
				Dim otherVec As INDArray = getWordVectorMatrix(s)
				Dim sim As Double = Transforms.cosineSim(docMean, otherVec)
				distances.incrementCount(s, CSng(sim))
			Next s

			Return distances.argMax()
		End Function

		''' <summary>
		''' Predict several labels based on the document.
		''' Computes a similarity wrt the mean of the
		''' representation of words in the document </summary>
		''' <param name="document"> raw text of the document </param>
		''' <returns> possible labels in descending order </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Collection<String> predictSeveral(@NonNull LabelledDocument document, int limit)
		Public Overridable Function predictSeveral(ByVal document As LabelledDocument, ByVal limit As Integer) As ICollection(Of String)
			If document.getReferencedContent() IsNot Nothing Then
				Return predictSeveral(document.getReferencedContent(), limit)
			Else
				Return predictSeveral(document.getContent(), limit)
			End If
		End Function

		''' <summary>
		''' Predict several labels based on the document.
		''' Computes a similarity wrt the mean of the
		''' representation of words in the document </summary>
		''' <param name="rawText"> raw text of the document </param>
		''' <returns> possible labels in descending order </returns>
		Public Overridable Function predictSeveral(ByVal rawText As String, ByVal limit As Integer) As ICollection(Of String)
			If tokenizerFactory_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("TokenizerFactory should be defined, prior to predict() call")
			End If

			Dim tokens As IList(Of String) = tokenizerFactory_Conflict.create(rawText).getTokens()
			Dim document As IList(Of VocabWord) = New List(Of VocabWord)()
			For Each token As String In tokens
				If vocab.containsWord(token) Then
					document.Add(vocab.wordFor(token))
				End If
			Next token

			Return predictSeveral(document, limit)
		End Function

		''' <summary>
		''' Predict several labels based on the document.
		''' Computes a similarity wrt the mean of the
		''' representation of words in the document </summary>
		''' <param name="document"> the document </param>
		''' <returns> possible labels in descending order </returns>
		Public Overridable Function predictSeveral(ByVal document As IList(Of VocabWord), ByVal limit As Integer) As ICollection(Of String)
	'        
	'            This code was transferred from original ParagraphVectors DL4j implementation, and yet to be tested
	'         
			If document.Count = 0 Then
				Throw New System.InvalidOperationException("Document has no words inside")
			End If
	'
	'        INDArray arr = Nd4j.create(document.size(), this.layerSize);
	'        for (int i = 0; i < document.size(); i++) {
	'            arr.putRow(i, getWordVectorMatrix(document.get(i).getWord()));
	'        }
	'
			Dim docMean As INDArray = inferVector(document) 'arr.mean(0);
			Dim distances As New Counter(Of String)()

			For Each s As String In labelsSource.getLabels()
				Dim otherVec As INDArray = getWordVectorMatrix(s)
				Dim sim As Double = Transforms.cosineSim(docMean, otherVec)
				log.debug("Similarity inside: [" & s & "] -> " & sim)
				distances.incrementCount(s, CSng(sim))
			Next s

			Dim keys As val = distances.keySetSorted()
			Return keys.subList(0, Math.Min(limit, keys.size()))
		End Function

		''' <summary>
		''' This method returns top N labels nearest to specified document
		''' </summary>
		''' <param name="document"> </param>
		''' <param name="topN">
		''' @return </param>
		Public Overridable Function nearestLabels(ByVal document As LabelledDocument, ByVal topN As Integer) As ICollection(Of String)
			If document.getReferencedContent() IsNot Nothing Then
				Return nearestLabels(document.getReferencedContent(), topN)
			Else
				Return nearestLabels(document.getContent(), topN)
			End If
		End Function

		''' <summary>
		''' This method returns top N labels nearest to specified text
		''' </summary>
		''' <param name="rawText"> </param>
		''' <param name="topN">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Collection<String> nearestLabels(@NonNull String rawText, int topN)
		Public Overridable Function nearestLabels(ByVal rawText As String, ByVal topN As Integer) As ICollection(Of String)
			Dim tokens As IList(Of String) = tokenizerFactory_Conflict.create(rawText).getTokens()
			Dim document As IList(Of VocabWord) = New List(Of VocabWord)()
			For Each token As String In tokens
				If vocab.containsWord(token) Then
					document.Add(vocab.wordFor(token))
				End If
			Next token

			' we're returning empty collection for empty document
			If document.Count = 0 Then
				log.info("Document passed to nearestLabels() has no matches in model vocabulary")
				Return New List(Of String)()
			End If

			Return nearestLabels(document, topN)
		End Function

		''' <summary>
		''' This method returns top N labels nearest to specified set of vocab words
		''' </summary>
		''' <param name="document"> </param>
		''' <param name="topN">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Collection<String> nearestLabels(@NonNull Collection<org.deeplearning4j.models.word2vec.VocabWord> document, int topN)
		Public Overridable Function nearestLabels(ByVal document As ICollection(Of VocabWord), ByVal topN As Integer) As ICollection(Of String)
			If document.Count = 0 Then
				Throw New ND4JIllegalStateException("Impossible to get nearestLabels for empty list of words")
			End If

			Dim vector As INDArray = inferVector(New List(Of VocabWord)(document))
			Return nearestLabels(vector, topN)
		End Function

		''' <summary>
		''' This method returns top N labels nearest to specified features vector
		''' </summary>
		''' <param name="labelVector"> </param>
		''' <param name="topN">
		''' @return </param>
		Public Overridable Function nearestLabels(ByVal labelVector As INDArray, ByVal topN As Integer) As ICollection(Of String)
			If labelsMatrix Is Nothing OrElse labelsList Is Nothing OrElse labelsList.Count = 0 Then
				extractLabels()
			End If

			Dim result As IList(Of BasicModelUtils.WordSimilarity) = New List(Of BasicModelUtils.WordSimilarity)()

			' if list still empty - return empty collection
			If labelsMatrix Is Nothing OrElse labelsList Is Nothing OrElse labelsList.Count = 0 Then
				log.warn("Labels list is empty!")
				Return New List(Of String)()
			End If

			If Not normalizedLabels Then
				SyncLock Me
					If Not normalizedLabels Then
						labelsMatrix.diviColumnVector(labelsMatrix.norm1(1))
						normalizedLabels = True
					End If
				End SyncLock
			End If

			Dim similarity As INDArray = Transforms.unitVec(labelVector).mmul(labelsMatrix.transpose())
			Dim highToLowSimList As IList(Of Double) = getTopN(similarity, topN + 20)

			For i As Integer = 0 To highToLowSimList.Count - 1
				Dim word As String = labelsList(highToLowSimList(i).intValue()).getLabel()
				If word IsNot Nothing AndAlso Not word.Equals("UNK") AndAlso Not word.Equals("STOP") Then
					Dim otherVec As INDArray = lookupTable.vector(word)
					Dim sim As Double = Transforms.cosineSim(labelVector, otherVec)

					result.Add(New BasicModelUtils.WordSimilarity(word, sim))
				End If
			Next i

			result.Sort(New BasicModelUtils.SimilarityComparator())

			Return BasicModelUtils.getLabels(result, topN)
		End Function

		''' <summary>
		''' Get top N elements
		''' </summary>
		''' <param name="vec"> the vec to extract the top elements from </param>
		''' <param name="N"> the number of elements to extract </param>
		''' <returns> the indices and the sorted top N elements </returns>
		Private Function getTopN(ByVal vec As INDArray, ByVal N As Integer) As IList(Of Double)
			Dim comparator As New BasicModelUtils.ArrayComparator()
			Dim queue As New PriorityQueue(Of Double())(vec.rows(), comparator)

			For j As Integer = 0 To vec.length() - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final System.Nullable<Double>[] pair = new System.Nullable<Double>[] {vec.getDouble(j), (double) j};
				Dim pair() As Double? = {vec.getDouble(j), CDbl(j)}
				If queue.size() < N Then
					queue.add(pair)
				Else
					Dim head() As Double? = queue.peek()
					If comparator.Compare(pair, head) > 0 Then
						queue.poll()
						queue.add(pair)
					End If
				End If
			Next j

			Dim lowToHighSimLst As IList(Of Double) = New List(Of Double)()

			Do While Not queue.isEmpty()
				Dim ind As Double = queue.poll()(1)
				lowToHighSimLst.Add(ind)
			Loop
			Return Lists.reverse(lowToHighSimLst)
		End Function

		''' <summary>
		''' This method returns similarity of the document to specific label, based on mean value
		''' </summary>
		''' <param name="rawText"> </param>
		''' <param name="label">
		''' @return </param>
		<Obsolete>
		Public Overridable Function similarityToLabel(ByVal rawText As String, ByVal label As String) As Double
			If tokenizerFactory_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("TokenizerFactory should be defined, prior to predict() call")
			End If

			Dim tokens As IList(Of String) = tokenizerFactory_Conflict.create(rawText).getTokens()
			Dim document As IList(Of VocabWord) = New List(Of VocabWord)()
			For Each token As String In tokens
				If vocab.containsWord(token) Then
					document.Add(vocab.wordFor(token))
				End If
			Next token
			Return similarityToLabel(document, label)
		End Function

		Public Overrides Sub fit()
			MyBase.fit()

			extractLabels()
		End Sub

		''' <summary>
		''' This method returns similarity of the document to specific label, based on mean value
		''' </summary>
		''' <param name="document"> </param>
		''' <param name="label">
		''' @return </param>
		Public Overridable Function similarityToLabel(ByVal document As LabelledDocument, ByVal label As String) As Double
			If document.getReferencedContent() IsNot Nothing Then
				Return similarityToLabel(document.getReferencedContent(), label)
			Else
				Return similarityToLabel(document.getContent(), label)
			End If
		End Function

		''' <summary>
		''' This method returns similarity of the document to specific label, based on mean value
		''' </summary>
		''' <param name="document"> </param>
		''' <param name="label">
		''' @return </param>
		Public Overridable Function similarityToLabel(ByVal document As IList(Of VocabWord), ByVal label As String) As Double
			If document.Count = 0 Then
				Throw New System.InvalidOperationException("Document has no words inside")
			End If

	'        
	'        INDArray arr = Nd4j.create(document.size(), this.layerSize);
	'        for (int i = 0; i < document.size(); i++) {
	'            arr.putRow(i, getWordVectorMatrix(document.get(i).getWord()));
	'        }

			Dim docMean As INDArray = inferVector(document) 'arr.mean(0);

			Dim otherVec As INDArray = getWordVectorMatrix(label)
			Dim sim As Double = Transforms.cosineSim(docMean, otherVec)
			Return sim
		End Function

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
		Public Overrides Function toJson() As String

			Dim retVal As New JsonObject()
			Dim mapper As ObjectMapper = ParagraphVectors.mapper()

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			retVal.addProperty(CLASS_FIELD, mapper.writeValueAsString(Me.GetType().FullName))

			If TypeOf Me.vocab Is AbstractCache Then
				retVal.addProperty(VOCAB_LIST_FIELD, CType(Me.vocab, AbstractCache(Of VocabWord)).toJson())
			End If

			Return retVal.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static ParagraphVectors fromJson(String jsonString) throws java.io.IOException
		Public Shared Function fromJson(ByVal jsonString As String) As ParagraphVectors

			Dim ret As New ParagraphVectors()

			Dim parser As New JsonParser()
			Dim json As JsonObject = parser.parse(jsonString).getAsJsonObject()

			Dim cache As VocabCache = AbstractCache.fromJson(json.get(VOCAB_LIST_FIELD).getAsString())

			ret.Vocab = cache
			Return ret
		End Function

		Public Class Builder
			Inherits Word2Vec.Builder

			Protected Friend Shadows labelAwareIterator As LabelAwareIterator
'JAVA TO VB CONVERTER NOTE: The field labelsSource was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend labelsSource_Conflict As LabelsSource
			Protected Friend docIter As DocumentIterator



			''' <summary>
			''' This method allows you to use pre-built WordVectors model (e.g. Word2Vec) for ParagraphVectors.
			''' Existing model will be transferred into new model before training starts.
			''' 
			''' PLEASE NOTE: Non-normalized model is recommended to use here.
			''' </summary>
			''' <param name="vec"> existing WordVectors model
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public Builder useExistingWordVectors(@NonNull WordVectors vec)
			Public Overrides Function useExistingWordVectors(ByVal vec As WordVectors) As Builder
				If CType(vec.lookupTable(), InMemoryLookupTable(Of VocabWord)).getSyn1() Is Nothing AndAlso CType(vec.lookupTable(), InMemoryLookupTable(Of VocabWord)).Syn1Neg Is Nothing Then
					Throw New ND4JIllegalStateException("Model being passed as existing has no syn1/syn1Neg available")
				End If

				Me.existingVectors = vec
				Return Me
			End Function

			''' <summary>
			''' This method defines, if words representations should be build together with documents representations.
			''' </summary>
			''' <param name="trainElements">
			''' @return </param>
			Public Overridable Function trainWordVectors(ByVal trainElements As Boolean) As Builder
				Me.trainElementsRepresentation(trainElements)
				Return Me
			End Function

			''' <summary>
			''' This method attaches pre-defined labels source to ParagraphVectors
			''' </summary>
			''' <param name="source">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder labelsSource(@NonNull LabelsSource source)
			Public Overridable Function labelsSource(ByVal source As LabelsSource) As Builder
				Me.labelsSource_Conflict = source
				Return Me
			End Function

			''' <summary>
			''' This method builds new LabelSource instance from labels.
			''' 
			''' PLEASE NOTE: Order synchro between labels and input documents delegated to end-user.
			''' PLEASE NOTE: Due to order issues it's recommended to use label aware iterators instead.
			''' </summary>
			''' <param name="labels">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated public Builder labels(@NonNull List<String> labels)
'JAVA TO VB CONVERTER NOTE: The parameter labels was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete>
			Public Overridable Function labels(ByVal labels_Conflict As IList(Of String)) As Builder
				Me.labelsSource_Conflict = New LabelsSource(labels_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method used to feed LabelAwareDocumentIterator, that contains training corpus, into ParagraphVectors
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterate(@NonNull LabelAwareDocumentIterator iterator)
			Public Overridable Overloads Function iterate(ByVal iterator As LabelAwareDocumentIterator) As Builder
				Me.docIter = iterator
				Return Me
			End Function

			''' <summary>
			''' This method used to feed LabelAwareSentenceIterator, that contains training corpus, into ParagraphVectors
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterate(@NonNull LabelAwareSentenceIterator iterator)
			Public Overridable Overloads Function iterate(ByVal iterator As LabelAwareSentenceIterator) As Builder
				Me.sentenceIterator = iterator
				Return Me
			End Function

			''' <summary>
			''' This method used to feed LabelAwareIterator, that contains training corpus, into ParagraphVectors
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
			''' This method used to feed DocumentIterator, that contains training corpus, into ParagraphVectors
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder iterate(@NonNull DocumentIterator iterator)
			Public Overrides Function iterate(ByVal iterator As DocumentIterator) As Builder
				Me.docIter = iterator
				Return Me
			End Function

			''' <summary>
			''' This method used to feed SentenceIterator, that contains training corpus, into ParagraphVectors
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder iterate(@NonNull SentenceIterator iterator)
			Public Overrides Function iterate(ByVal iterator As SentenceIterator) As Builder
				Me.sentenceIterator = iterator
				Return Me
			End Function

			''' <summary>
			''' Sets ModelUtils that gonna be used as provider for utility methods: similarity(), wordsNearest(), accuracy(), etc
			''' </summary>
			''' <param name="modelUtils"> model utils to be used
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder modelUtils(@NonNull ModelUtils<org.deeplearning4j.models.word2vec.VocabWord> modelUtils)
'JAVA TO VB CONVERTER NOTE: The parameter modelUtils was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function modelUtils(ByVal modelUtils_Conflict As ModelUtils(Of VocabWord)) As Builder
				MyBase.modelUtils(modelUtils_Conflict)
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
			''' This method allows you to specify SequenceElement that will be used as UNK element, if UNK is used
			''' </summary>
			''' <param name="element">
			''' @return </param>
			Public Overrides Function unknownElement(ByVal element As VocabWord) As Builder
				MyBase.unknownElement(element)
				Return Me
			End Function

			''' <summary>
			''' This method enables/disables parallel tokenization.
			''' 
			''' Default value: TRUE </summary>
			''' <param name="allow">
			''' @return </param>
			Public Overrides Function allowParallelTokenization(ByVal allow As Boolean) As Builder
				MyBase.allowParallelTokenization(allow)
				Return Me
			End Function

			''' <summary>
			''' This method allows you to specify, if UNK word should be used internally
			''' </summary>
			''' <param name="reallyUse">
			''' @return </param>
			Public Overrides Function useUnknown(ByVal reallyUse As Boolean) As Builder
				MyBase.useUnknown(reallyUse)
				If Me.unknownElement Is Nothing Then
					Me.unknownElement(New VocabWord(1.0, ParagraphVectors.DEFAULT_UNK))
				End If
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

			Public Overrides Function build() As ParagraphVectors
				presetTables()

				Dim ret As New ParagraphVectors()

				If Me.existingVectors IsNot Nothing Then
					trainWordVectors(False)
					trainElementsRepresentation(False)
					Me.elementsLearningAlgorithm = Nothing

					'    this.lookupTable = this.existingVectors.lookupTable();
					'    this.vocabCache = this.existingVectors.vocab();
				End If

				If Me.labelsSource_Conflict Is Nothing Then
					Me.labelsSource_Conflict = New LabelsSource()
				End If
				If docIter IsNot Nothing Then
	'                
	'                        we're going to work with DocumentIterator.
	'                        First, we have to assume that user can provide LabelAwareIterator. In this case we'll use them, as provided source, and collec labels provided there
	'                        Otherwise we'll go for own labels via LabelsSource
	'                

					If TypeOf docIter Is LabelAwareDocumentIterator Then
						Me.labelAwareIterator = New DocumentIteratorConverter(DirectCast(docIter, LabelAwareDocumentIterator), labelsSource_Conflict)
					Else
						Me.labelAwareIterator = New DocumentIteratorConverter(docIter, labelsSource_Conflict)
					End If
				ElseIf sentenceIterator IsNot Nothing Then
					' we have SentenceIterator. Mechanics will be the same, as above
					If TypeOf sentenceIterator Is LabelAwareSentenceIterator Then
						Me.labelAwareIterator = New SentenceIteratorConverter(DirectCast(sentenceIterator, LabelAwareSentenceIterator), labelsSource_Conflict)
					Else
						Me.labelAwareIterator = New SentenceIteratorConverter(sentenceIterator, labelsSource_Conflict)
					End If
				ElseIf labelAwareIterator IsNot Nothing Then
					' if we have LabelAwareIterator defined, we have to be sure that LabelsSource is propagated properly
					Me.labelsSource_Conflict = labelAwareIterator.LabelsSource
				Else
					' we have nothing, probably that's restored model building. ignore iterator for now.
					' probably there's few reasons to move iterator initialization code into ParagraphVectors methods. Like protected setLabelAwareIterator method.
				End If

				If labelAwareIterator IsNot Nothing Then
					Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(labelAwareIterator).tokenizerFactory(tokenizerFactory_Conflict).allowMultithreading(allowParallelTokenization_Conflict).build()
					Me.iterator = (New AbstractSequenceIterator.Builder(Of )(transformer)).build()
				End If

				ret.numEpochs = Me.numEpochs
				ret.numIterations = Me.iterations
				ret.vocab_Conflict = Me.vocabCache
				ret.minWordFrequency = Me.minWordFrequency
				ret.learningRate.set(Me.learningRate)
				ret.minLearningRate = Me.minLearningRate
				ret.sampling = Me.sampling
				ret.negative = Me.negative
				ret.layerSize_Conflict = Me.layerSize
				ret.batchSize = Me.batchSize
				ret.learningRateDecayWords = Me.learningRateDecayWords
				ret.window = Me.window
				ret.resetModel = Me.resetModel
				ret.useAdeGrad = Me.useAdaGrad
				ret.stopWords = Me.stopWords
				ret.workers = Me.workers
				ret.useUnknown = Me.useUnknown
				ret.unknownElement = Me.unknownElement
				ret.seed = Me.seed
				ret.enableScavenger = Me.enableScavenger
				ret.vocabLimit = Me.vocabLimit

				ret.trainElementsVectors = Me.trainElementsVectors
				ret.trainSequenceVectors = Me.trainSequenceVectors

				ret.elementsLearningAlgorithm = Me.elementsLearningAlgorithm
				ret.sequenceLearningAlgorithm = Me.sequenceLearningAlgorithm

				ret.tokenizerFactory_Conflict = Me.tokenizerFactory_Conflict

				ret.existingModel = Me.existingVectors

				ret.lookupTable_Conflict = Me.lookupTable
				ret.modelUtils_Conflict = Me.modelUtils
				ret.eventListeners = Me.vectorsListeners

				Me.configuration.setLearningRate(Me.learningRate)
				Me.configuration.setLayersSize(layerSize)
				Me.configuration.setHugeModelExpected(hugeModelExpected)
				Me.configuration.setWindow(window)
				Me.configuration.setMinWordFrequency(minWordFrequency)
				Me.configuration.setIterations(iterations)
				Me.configuration.setSeed(seed)
				Me.configuration.setBatchSize(batchSize)
				Me.configuration.setLearningRateDecayWords(learningRateDecayWords)
				Me.configuration.setMinLearningRate(minLearningRate)
				Me.configuration.setSampling(Me.sampling)
				Me.configuration.setUseAdaGrad(useAdaGrad)
				Me.configuration.setNegative(negative)
				Me.configuration.setEpochs(Me.numEpochs)
				Me.configuration.setStopList(Me.stopWords)
				Me.configuration.setUseHierarchicSoftmax(Me.useHierarchicSoftmax)
				Me.configuration.setTrainElementsVectors(Me.trainElementsVectors)
				Me.configuration.setPreciseWeightInit(Me.preciseWeightInit)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.configuration.setSequenceLearningAlgorithm(Me.sequenceLearningAlgorithm.GetType().FullName)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.configuration.setModelUtils(Me.modelUtils.GetType().FullName)
				Me.configuration.setAllowParallelTokenization(Me.allowParallelTokenization_Conflict)

				If tokenizerFactory_Conflict IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					Me.configuration.setTokenizerFactory(tokenizerFactory_Conflict.GetType().FullName)
					If tokenizerFactory_Conflict.TokenPreProcessor IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
						Me.configuration.setTokenPreProcessor(tokenizerFactory_Conflict.TokenPreProcessor.GetType().FullName)
					End If
				End If

				ret.configuration = Me.configuration


				' hardcoded to TRUE, since it's ParagraphVectors wrapper
				ret.trainElementsVectors = Me.trainElementsVectors
				ret.trainSequenceVectors = True
				ret.labelsSource = Me.labelsSource_Conflict
				ret.labelAwareIterator = Me.labelAwareIterator
				ret.iterator = Me.iterator

				Return ret
			End Function

			Public Sub New()
				MyBase.New()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VectorsConfiguration configuration)
			Public Sub New(ByVal configuration As VectorsConfiguration)
				MyBase.New(configuration)
			End Sub


			''' <summary>
			''' This method defines TokenizerFactory to be used for strings tokenization during training
			''' PLEASE NOTE: If external VocabCache is used, the same TokenizerFactory should be used to keep derived tokens equal.
			''' </summary>
			''' <param name="tokenizerFactory">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder tokenizerFactory(@NonNull TokenizerFactory tokenizerFactory)
'JAVA TO VB CONVERTER NOTE: The parameter tokenizerFactory was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overrides Function tokenizerFactory(ByVal tokenizerFactory_Conflict As TokenizerFactory) As Builder
				MyBase.tokenizerFactory(tokenizerFactory_Conflict)
				Return Me
			End Function

			''' <summary>
			''' This method used to feed SequenceIterator, that contains training corpus, into ParagraphVectors
			''' </summary>
			''' <param name="iterator">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder iterate(@NonNull SequenceIterator<org.deeplearning4j.models.word2vec.VocabWord> iterator)
			Public Overrides Function iterate(ByVal iterator As SequenceIterator(Of VocabWord)) As Builder
				MyBase.iterate(iterator)
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
			''' This method sets VectorsListeners for this SequenceVectors model
			''' </summary>
			''' <param name="vectorsListeners">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder setVectorsListeners(@NonNull Collection<org.deeplearning4j.models.sequencevectors.interfaces.VectorsListener<org.deeplearning4j.models.word2vec.VocabWord>> vectorsListeners)
			Public Overrides Function setVectorsListeners(ByVal vectorsListeners As ICollection(Of VectorsListener(Of VocabWord))) As Builder
				MyBase.VectorsListeners = vectorsListeners
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
			''' This method allows to define external VocabCache to be used
			''' </summary>
			''' <param name="vocabCache">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder vocabCache(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache)
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
'ORIGINAL LINE: @Override public Builder lookupTable(@NonNull WeightLookupTable<org.deeplearning4j.models.word2vec.VocabWord> lookupTable)
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
			''' This method defines stop words that should be ignored during training </summary>
			''' <param name="stopList">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder stopWords(@NonNull List<String> stopList)
			Public Overrides Function stopWords(ByVal stopList As IList(Of String)) As Builder
				MyBase.stopWords(stopList)
				Return Me
			End Function

			''' <summary>
			''' This method defines, if words representation should be build together with documents representations.
			''' </summary>
			''' <param name="trainElements">
			''' @return </param>
			Public Overrides Function trainElementsRepresentation(ByVal trainElements As Boolean) As Builder
				Me.trainElementsVectors = trainElements
				Return Me
			End Function

			''' <summary>
			''' This method is hardcoded to TRUE, since that's whole point of ParagraphVectors
			''' </summary>
			''' <param name="trainSequences">
			''' @return </param>
			Public Overrides Function trainSequencesRepresentation(ByVal trainSequences As Boolean) As Builder
				Me.trainSequenceVectors = trainSequences
				Return Me
			End Function

			''' <summary>
			''' This method defines stop words that should be ignored during training
			''' </summary>
			''' <param name="stopList">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Builder stopWords(@NonNull Collection<org.deeplearning4j.models.word2vec.VocabWord> stopList)
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
			''' This method defines maximum number of concurrent threads available for training
			''' </summary>
			''' <param name="numWorkers">
			''' @return </param>
			Public Overrides Function workers(ByVal numWorkers As Integer) As Builder
				MyBase.workers(numWorkers)
				Return Me
			End Function

			Public Overrides Function sequenceLearningAlgorithm(ByVal algorithm As SequenceLearningAlgorithm(Of VocabWord)) As Builder
				MyBase.sequenceLearningAlgorithm(algorithm)
				Return Me
			End Function

			Public Overrides Function sequenceLearningAlgorithm(ByVal algorithm As String) As Builder
				MyBase.sequenceLearningAlgorithm(algorithm)
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

			''' <summary>
			''' This method has no effect for ParagraphVectors
			''' </summary>
			''' <param name="windows">
			''' @return </param>
			Public Overrides Function useVariableWindow(ParamArray ByVal windows() As Integer) As Builder
				' no-op
				Return Me
			End Function

			Public Overrides Function elementsLearningAlgorithm(ByVal algorithm As ElementsLearningAlgorithm(Of VocabWord)) As Builder
				MyBase.elementsLearningAlgorithm(algorithm)
				Return Me
			End Function

			Public Overrides Function elementsLearningAlgorithm(ByVal algorithm As String) As Builder
				MyBase.elementsLearningAlgorithm(algorithm)
				Return Me
			End Function

			Public Overrides Function usePreciseWeightInit(ByVal reallyUse As Boolean) As Builder
				MyBase.usePreciseWeightInit(reallyUse)
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
		End Class


		Public Class InferenceCallable
			Implements Callable(Of Pair(Of String, INDArray))

			Private ReadOnly outerInstance As ParagraphVectors

			Friend ReadOnly tokenizerFactory As TokenizerFactory
			Friend ReadOnly vocab As VocabCache(Of VocabWord)
			Friend ReadOnly document As LabelledDocument
			Friend flag As AtomicLong

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InferenceCallable(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache, @NonNull TokenizerFactory tokenizerFactory, @NonNull LabelledDocument document)
			Public Sub New(ByVal outerInstance As ParagraphVectors, ByVal vocabCache As VocabCache(Of VocabWord), ByVal tokenizerFactory As TokenizerFactory, ByVal document As LabelledDocument)
				Me.outerInstance = outerInstance
				Me.tokenizerFactory = tokenizerFactory
				Me.vocab = vocabCache
				Me.document = document
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public InferenceCallable(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache, @NonNull TokenizerFactory tokenizerFactory, @NonNull LabelledDocument document, @NonNull AtomicLong flag)
			Public Sub New(ByVal outerInstance As ParagraphVectors, ByVal vocabCache As VocabCache(Of VocabWord), ByVal tokenizerFactory As TokenizerFactory, ByVal document As LabelledDocument, ByVal flag As AtomicLong)
				Me.New(outerInstance, vocabCache, tokenizerFactory, document)
				Me.outerInstance = outerInstance
				Me.flag = flag
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.common.primitives.Pair<String, org.nd4j.linalg.api.ndarray.INDArray> call() throws Exception
			Public Overrides Function [call]() As Pair(Of String, INDArray)

				' first part of this callable will be actually run in parallel
				Dim tokens As IList(Of String) = tokenizerFactory.create(document.getContent()).getTokens()
				Dim documentAsWords As IList(Of VocabWord) = New List(Of VocabWord)()
				For Each token As String In tokens
					If vocab.containsWord(token) Then
						documentAsWords.Add(vocab.wordFor(token))
					End If
				Next token

				If documentAsWords.Count = 0 Then
					Throw New ND4JIllegalStateException("Text passed for inference has no matches in model vocabulary.")
				End If

				' inference will be single-threaded in java, and parallel in native
				Dim result As Pair(Of String, INDArray) = Pair.makePair(document.getId(), outerInstance.inferVector(documentAsWords))


				outerInstance.countFinished.incrementAndGet()

				If flag IsNot Nothing Then
					flag.incrementAndGet()
				End If

				Return result
			End Function
		End Class

		Public Class BlindInferenceCallable
			Implements Callable(Of INDArray)

			Private ReadOnly outerInstance As ParagraphVectors

			Friend ReadOnly tokenizerFactory As TokenizerFactory
			Friend ReadOnly vocab As VocabCache(Of VocabWord)
			Friend ReadOnly document As String
			Friend flag As AtomicLong

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BlindInferenceCallable(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache, @NonNull TokenizerFactory tokenizerFactory, @NonNull String document)
			Public Sub New(ByVal outerInstance As ParagraphVectors, ByVal vocabCache As VocabCache(Of VocabWord), ByVal tokenizerFactory As TokenizerFactory, ByVal document As String)
				Me.outerInstance = outerInstance
				Me.tokenizerFactory = tokenizerFactory
				Me.vocab = vocabCache
				Me.document = document
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BlindInferenceCallable(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache, @NonNull TokenizerFactory tokenizerFactory, @NonNull String document, @NonNull AtomicLong flag)
			Public Sub New(ByVal outerInstance As ParagraphVectors, ByVal vocabCache As VocabCache(Of VocabWord), ByVal tokenizerFactory As TokenizerFactory, ByVal document As String, ByVal flag As AtomicLong)
				Me.New(outerInstance, vocabCache, tokenizerFactory, document)
				Me.outerInstance = outerInstance
				Me.flag = flag
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray call() throws Exception
			Public Overrides Function [call]() As INDArray

				' first part of this callable will be actually run in parallel
				Dim tokens As IList(Of String) = tokenizerFactory.create(document).getTokens()
				Dim documentAsWords As IList(Of VocabWord) = New List(Of VocabWord)()
				For Each token As String In tokens
					If vocab.containsWord(token) Then
						documentAsWords.Add(vocab.wordFor(token))
					End If
				Next token

				If documentAsWords.Count = 0 Then
					Throw New ND4JIllegalStateException("Text passed for inference has no matches in model vocabulary.")
				End If


				' inference will be single-threaded in java, and parallel in native
				Dim result As INDArray = outerInstance.inferVector(documentAsWords)

				outerInstance.countFinished.incrementAndGet()

				If flag IsNot Nothing Then
					flag.incrementAndGet()
				End If

				Return result
			End Function
		End Class
	End Class

End Namespace