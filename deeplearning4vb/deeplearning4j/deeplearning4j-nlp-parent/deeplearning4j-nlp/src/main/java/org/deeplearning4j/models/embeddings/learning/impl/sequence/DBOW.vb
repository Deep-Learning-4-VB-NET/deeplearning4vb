Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.learning
Imports org.deeplearning4j.models.embeddings.learning
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
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

Namespace org.deeplearning4j.models.embeddings.learning.impl.sequence


	Public Class DBOW(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements SequenceLearningAlgorithm(Of T)

		Protected Friend vocabCache As VocabCache(Of T)
		Protected Friend lookupTable As WeightLookupTable(Of T)
		Protected Friend configuration As VectorsConfiguration


		Protected Friend window As Integer
		Protected Friend useAdaGrad As Boolean
		Protected Friend negative As Double

		Protected Friend skipGram As New SkipGram(Of T)()

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(DBOW))

		Public Overridable ReadOnly Property ElementsLearningAlgorithm As ElementsLearningAlgorithm(Of T)
			Get
				Return skipGram
			End Get
		End Property

		Public Sub New()

		End Sub

		Public Overridable ReadOnly Property CodeName As String Implements SequenceLearningAlgorithm(Of T).getCodeName
			Get
				Return "PV-DBOW"
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void configure(@NonNull VocabCache<T> vocabCache, @NonNull WeightLookupTable<T> lookupTable, @NonNull VectorsConfiguration configuration)
		Public Overridable Sub configure(ByVal vocabCache As VocabCache(Of T), ByVal lookupTable As WeightLookupTable(Of T), ByVal configuration As VectorsConfiguration)
			Me.vocabCache = vocabCache
			Me.lookupTable = lookupTable

			Me.window = configuration.getWindow()
			Me.useAdaGrad = configuration.isUseAdaGrad()
			Me.negative = configuration.getNegative()
			Me.configuration = configuration

			skipGram.configure(vocabCache, lookupTable, configuration)
		End Sub

		''' <summary>
		''' DBOW doesn't involves any pretraining
		''' </summary>
		''' <param name="iterator"> </param>
		Public Overridable Sub pretrain(ByVal iterator As SequenceIterator(Of T)) Implements SequenceLearningAlgorithm(Of T).pretrain

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public double learnSequence(@NonNull Sequence<T> sequence, @NonNull AtomicLong nextRandom, double learningRate, org.deeplearning4j.models.embeddings.learning.impl.elements.BatchSequences<T> batchSequences)
		Public Overridable Function learnSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal learningRate As Double, ByVal batchSequences As BatchSequences(Of T)) As Double

			' we just pass data to dbow, and loop over sequence there
			dbow(0, sequence, CInt(Math.Truncate(nextRandom.get())) Mod window, nextRandom, learningRate, False, Nothing, batchSequences)


			Return 0
		End Function

		''' <summary>
		''' DBOW has no reasons for early termination
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property EarlyTerminationHit As Boolean Implements SequenceLearningAlgorithm(Of T).isEarlyTerminationHit
			Get
				Return False
			End Get
		End Property

		Protected Friend Overridable Sub dbow(ByVal i As Integer, ByVal sequence As Sequence(Of T), ByVal b As Integer, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal isInference As Boolean, ByVal inferenceVector As INDArray, ByVal batchSequences As BatchSequences(Of T))

			'final T word = sequence.getElements().get(i);
			Dim sentence As IList(Of T) = skipGram.applySubsampling(sequence, nextRandom).getElements()


			If sequence.SequenceLabel Is Nothing Then
				Return
			End If

			Dim labels As IList(Of T) = New List(Of T)()
			CType(labels, List(Of T)).AddRange(sequence.getSequenceLabels())

			If sentence.Count = 0 OrElse labels.Count = 0 Then
				Return
			End If

			Dim batchSize As Integer = configuration.getBatchSize()

			For Each lastWord As T In labels
				For Each word As T In sentence
					If word Is Nothing Then
						Continue For
					End If

					If batchSize = 1 OrElse batchSequences Is Nothing OrElse isInference Then
						skipGram.iterateSample(word, lastWord, nextRandom, alpha, isInference, inferenceVector)
					Else
						batchSequences.put(word, lastWord, nextRandom.get(), alpha)
					End If
				Next word
			Next lastWord

			If skipGram IsNot Nothing AndAlso skipGram.getBatch() IsNot Nothing AndAlso skipGram.getBatch() IsNot Nothing AndAlso skipGram.getBatch().Count >= configuration.getBatchSize() Then
				Nd4j.Executioner.exec(skipGram.getBatch())
				skipGram.getBatch().Clear()
			End If
		End Sub

		''' <summary>
		''' This method does training on previously unseen paragraph, and returns inferred vector
		''' </summary>
		''' <param name="sequence"> </param>
		''' <param name="nextRandom"> </param>
		''' <param name="learningRate">
		''' @return </param>
		Public Overridable Function inferSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As Long, ByVal learningRate As Double, ByVal minLearningRate As Double, ByVal iterations As Integer) As INDArray Implements SequenceLearningAlgorithm(Of T).inferSequence
			Dim nr As New AtomicLong(nextRandom)

			' we probably don't want subsampling here
			' Sequence<T> seq = cbow.applySubsampling(sequence, nextRandom);
			' if (sequence.getSequenceLabel() == null) throw new IllegalStateException("Label is NULL");

			If sequence.Empty Then
				Return Nothing
			End If


			Dim random As Random = Nd4j.RandomFactory.getNewRandomInstance(configuration.getSeed() * sequence.GetHashCode(), lookupTable.layerSize() + 1)
			Dim ret As INDArray = Nd4j.rand(New Integer() {1, lookupTable.layerSize()}, random).subi(0.5).divi(lookupTable.layerSize())

			For iter As Integer = 0 To iterations - 1
				nr.set(Math.Abs(nr.get() * 25214903917L + 11))
				dbow(0, sequence, CInt(Math.Truncate(nr.get())) Mod window, nr, learningRate, True, ret, Nothing)

				learningRate = ((learningRate - minLearningRate) / (iterations - iter)) + minLearningRate
			Next iter

			finish()

			Return ret
		End Function

		Public Overridable Sub finish() Implements SequenceLearningAlgorithm(Of T).finish
			If skipGram IsNot Nothing AndAlso skipGram.getBatch() IsNot Nothing AndAlso skipGram.getBatch().Count > 0 Then
				Nd4j.Executioner.exec(skipGram.getBatch())
				skipGram.getBatch().Clear()
			End If
		End Sub
	End Class

End Namespace