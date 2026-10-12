Imports System
Imports System.Collections.Generic
Imports AtomicDouble = org.nd4j.shade.guava.util.concurrent.AtomicDouble
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports ArrayUtils = org.apache.commons.lang.ArrayUtils
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.reader
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Heartbeat = org.nd4j.linalg.heartbeat.Heartbeat
Imports Environment = org.nd4j.linalg.heartbeat.reports.Environment
Imports [Event] = org.nd4j.linalg.heartbeat.reports.Event
Imports Task = org.nd4j.linalg.heartbeat.reports.Task
Imports EnvironmentUtils = org.nd4j.linalg.heartbeat.utils.EnvironmentUtils

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

Namespace org.deeplearning4j.models.embeddings.wordvectors


	<Serializable>
	Public Class WordVectorsImpl(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements WordVectors

		Private Const serialVersionUID As Long = 78249242142L

		'number of times the word must occur in the vocab to appear in the calculations, otherwise treat as unknown
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected int minWordFrequency = 5;
		Protected Friend minWordFrequency As Integer = 5
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.embeddings.WeightLookupTable<T> lookupTable;
'JAVA TO VB CONVERTER NOTE: The field lookupTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend lookupTable_Conflict As WeightLookupTable(Of T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.word2vec.wordstore.VocabCache<T> vocab;
'JAVA TO VB CONVERTER NOTE: The field vocab was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vocab_Conflict As VocabCache(Of T)
'JAVA TO VB CONVERTER NOTE: The field layerSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend layerSize_Conflict As Integer = 100
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient org.deeplearning4j.models.embeddings.reader.ModelUtils<T> modelUtils = new org.deeplearning4j.models.embeddings.reader.impl.BasicModelUtils<>();
'JAVA TO VB CONVERTER NOTE: The field modelUtils was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend modelUtils_Conflict As ModelUtils(Of T) = New BasicModelUtils(Of T)()
		Private initDone As Boolean = False

		Protected Friend numIterations As Integer = 1
		Protected Friend numEpochs As Integer = 1
		Protected Friend negative As Double = 0
		Protected Friend sampling As Double = 0
		Protected Friend learningRate As New AtomicDouble(0.025)
		Protected Friend minLearningRate As Double = 0.01
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected int window = 5;
		Protected Friend window As Integer = 5
		Protected Friend batchSize As Integer
		Protected Friend learningRateDecayWords As Integer
		Protected Friend resetModel As Boolean
		Protected Friend useAdeGrad As Boolean
		Protected Friend workers As Integer = 1 'Runtime.getRuntime().availableProcessors();
		Protected Friend trainSequenceVectors As Boolean = False
		Protected Friend trainElementsVectors As Boolean = True
		Protected Friend seed As Long
		Protected Friend useUnknown As Boolean = False
		Protected Friend variableWindows() As Integer


		''' <summary>
		''' This method returns word vector size
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property LayerSize As Integer
			Get
				If lookupTable_Conflict IsNot Nothing AndAlso lookupTable_Conflict.Weights IsNot Nothing Then
					Return lookupTable_Conflict.Weights.columns()
				Else
					Return layerSize_Conflict
				End If
			End Get
		End Property

		Public Const DEFAULT_UNK As String = "UNK"
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private String UNK = DEFAULT_UNK;
		Private UNK As String = DEFAULT_UNK

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected Collection<String> stopWords = new ArrayList<>();
		Protected Friend stopWords As ICollection(Of String) = New List(Of String)() 'StopWords.getStopWords();

		''' <summary>
		''' Returns true if the model has this word in the vocab </summary>
		''' <param name="word"> the word to test for </param>
		''' <returns> true if the model has the word in the vocab </returns>
		Public Overridable Function hasWord(ByVal word As String) As Boolean Implements WordVectors.hasWord
			Return vocab().indexOf(word) >= 0
		End Function

		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearestSum(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)
			Return modelUtils_Conflict.wordsNearestSum(positive, negative, top)
		End Function

		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' * <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearestSum(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)
			Return modelUtils_Conflict.wordsNearestSum(words, top)
		End Function

		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' * <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearest(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)
			Return modelUtils_Conflict.wordsNearest(words, top)
		End Function

		''' <summary>
		''' Get the top n words most similar to the given word </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="n"> the n to get </param>
		''' <returns> the top n words </returns>
		Public Overridable Function wordsNearestSum(ByVal word As String, ByVal n As Integer) As ICollection(Of String)
			Return modelUtils_Conflict.wordsNearestSum(word, n)
		End Function


		''' <summary>
		''' Accuracy based on questions which are a space separated list of strings
		''' where the first word is the query word, the next 2 words are negative,
		''' and the last word is the predicted word to be nearest </summary>
		''' <param name="questions"> the questions to ask </param>
		''' <returns> the accuracy based on these questions </returns>
		Public Overridable Function accuracy(ByVal questions As IList(Of String)) As IDictionary(Of String, Double)
			Return modelUtils_Conflict.accuracy(questions)
		End Function

		Public Overridable Function indexOf(ByVal word As String) As Integer Implements WordVectors.indexOf
			Return vocab().indexOf(word)
		End Function


		''' <summary>
		''' Find all words with a similar characters
		''' in the vocab </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="accuracy"> the accuracy: 0 to 1 </param>
		''' <returns> the list of words that are similar in the vocab </returns>
		Public Overridable Function similarWordsInVocabTo(ByVal word As String, ByVal accuracy As Double) As IList(Of String)
			Return Me.modelUtils_Conflict.similarWordsInVocabTo(word, accuracy)
		End Function

		''' <summary>
		''' Get the word vector for a given matrix </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the ndarray for this word </returns>
		Public Overridable Function getWordVector(ByVal word As String) As Double() Implements WordVectors.getWordVector
			Dim r As INDArray = getWordVectorMatrix(word)
			If r Is Nothing Then
				Return Nothing
			End If
			Return r.dup().data().asDouble()
		End Function

		''' <summary>
		''' Returns the word vector divided by the norm2 of the array </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the looked up matrix </returns>
		Public Overridable Function getWordVectorMatrixNormalized(ByVal word As String) As INDArray Implements WordVectors.getWordVectorMatrixNormalized
			Dim r As INDArray = getWordVectorMatrix(word)
			If r Is Nothing Then
				Return Nothing
			End If

			Return r.div(Nd4j.BlasWrapper.nrm2(r))
		End Function

		Public Overridable Function getWordVectorMatrix(ByVal word As String) As INDArray Implements WordVectors.getWordVectorMatrix
			Return lookupTable().vector(word)
		End Function


		''' <summary>
		''' Words nearest based on positive and negative words
		''' </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearest(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)
			Return modelUtils_Conflict.wordsNearest(positive, negative, top)
		End Function

		''' <summary>
		''' This method returns 2D array, where each row represents corresponding label
		''' </summary>
		''' <param name="labels">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray getWordVectors(@NonNull Collection<String> labels)
		Public Overridable Function getWordVectors(ByVal labels As ICollection(Of String)) As INDArray
			Dim indexes(labels.Count - 1) As Integer
			Dim cnt As Integer = 0
			Dim useIndexUnknown As Boolean = useUnknown AndAlso vocab_Conflict.containsWord(UNK)

			For Each label As String In labels
				If vocab_Conflict.containsWord(label) Then
					indexes(cnt) = vocab_Conflict.indexOf(label)
				Else
					indexes(cnt) = If(useIndexUnknown, vocab_Conflict.indexOf(UNK), -1)
				End If
				cnt += 1
			Next label

			Do While ArrayUtils.contains(indexes, -1)
				indexes = ArrayUtils.removeElement(indexes, -1)
			Loop
			If indexes.Length = 0 Then
					Return Nd4j.empty(CType(lookupTable_Conflict, InMemoryLookupTable).getSyn0().dataType())
			End If

			Dim result As INDArray = Nd4j.pullRows(lookupTable_Conflict.Weights, 1, indexes)
			Return result
		End Function

		''' <summary>
		''' This method returns mean vector, built from words/labels passed in
		''' </summary>
		''' <param name="labels">
		''' @return </param>
		Public Overridable Function getWordVectorsMean(ByVal labels As ICollection(Of String)) As INDArray
			Dim array As INDArray = getWordVectors(labels)
			Return array.mean(0)
		End Function

		''' <summary>
		''' Get the top n words most similar to the given word </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="n"> the n to get </param>
		''' <returns> the top n words </returns>
		Public Overridable Function wordsNearest(ByVal word As String, ByVal n As Integer) As ICollection(Of String)
			Return modelUtils_Conflict.wordsNearest(word, n)
		End Function


		''' <summary>
		''' Returns similarity of two elements, provided by ModelUtils
		''' </summary>
		''' <param name="word"> the first word </param>
		''' <param name="word2"> the second word </param>
		''' <returns> a normalized similarity (cosine similarity) </returns>
		Public Overridable Function similarity(ByVal word As String, ByVal word2 As String) As Double Implements WordVectors.similarity
			Return modelUtils_Conflict.similarity(word, word2)
		End Function

		Public Overridable Function vocab() As VocabCache(Of T)
			Return vocab_Conflict
		End Function

		Public Overridable Function lookupTable() As WeightLookupTable Implements WordVectors.lookupTable
			Return lookupTable_Conflict
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void setModelUtils(@NonNull ModelUtils modelUtils)
		Public Overridable WriteOnly Property ModelUtils As ModelUtils
			Set(ByVal modelUtils As ModelUtils)
				If lookupTable_Conflict IsNot Nothing Then
					modelUtils.init(lookupTable_Conflict)
					Me.modelUtils_Conflict = modelUtils
					'0.25, -0.03, -0.47, 0.10, -0.25, 0.28, 0.37,
				End If
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setLookupTable(@NonNull WeightLookupTable lookupTable)
		Public Overridable WriteOnly Property LookupTable As WeightLookupTable
			Set(ByVal lookupTable As WeightLookupTable)
				Me.lookupTable_Conflict = lookupTable
				If modelUtils_Conflict Is Nothing Then
					Me.modelUtils_Conflict = New BasicModelUtils(Of T)()
				End If
    
				Me.modelUtils_Conflict.init(lookupTable)
			End Set
		End Property

		Public Overridable WriteOnly Property Vocab As VocabCache
			Set(ByVal vocab As VocabCache)
				Me.vocab_Conflict = vocab
			End Set
		End Property

		Protected Friend Overridable Sub update()
			update(EnvironmentUtils.buildEnvironment(), [Event].STANDALONE)
		End Sub

		Protected Friend Overridable Sub update(ByVal env As Environment, ByVal [event] As [Event])
			If Not initDone Then
				initDone = True

				Dim heartbeat As Heartbeat = Heartbeat.Instance
				Dim task As New Task()
				task.setNumFeatures(layerSize_Conflict)
				If vocab_Conflict IsNot Nothing Then
					task.setNumSamples(vocab_Conflict.numWords())
				End If
				task.setNetworkType(Task.NetworkType.DenseNetwork)
				task.setArchitectureType(Task.ArchitectureType.WORDVECTORS)

				heartbeat.reportEvent([event], env, task)
			End If
		End Sub

		Public Overridable Sub loadWeightsInto(ByVal array As INDArray)
			array.assign(lookupTable_Conflict.Weights)
		End Sub

		Public Overridable Function vocabSize() As Long
			Return lookupTable_Conflict.Weights.size(0)
		End Function

		Public Overridable Function vectorSize() As Integer
			Return lookupTable_Conflict.layerSize()
		End Function

		Public Overridable Function jsonSerializable() As Boolean
			Return False
		End Function

		Public Overridable Function outOfVocabularySupported() As Boolean Implements WordVectors.outOfVocabularySupported
			Return False
		End Function
	End Class

End Namespace