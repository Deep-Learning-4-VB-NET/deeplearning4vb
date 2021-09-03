Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DM<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.deeplearning4j.models.embeddings.learning.SequenceLearningAlgorithm<T>
	Public Class DM(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements SequenceLearningAlgorithm(Of T)

		Private vocabCache As VocabCache(Of T)
		Private lookupTable As WeightLookupTable(Of T)
		Private configuration As VectorsConfiguration

		Protected Friend Shared MAX_EXP As Double = 6

		Protected Friend window As Integer
		Protected Friend useAdaGrad As Boolean
		Protected Friend negative As Double
		Protected Friend sampling As Double

		Protected Friend expTable() As Double

		Protected Friend syn0, syn1, syn1Neg, table As INDArray

		Private cbow As New CBOW(Of T)()

		Public Overridable ReadOnly Property ElementsLearningAlgorithm As ElementsLearningAlgorithm(Of T)
			Get
				Return cbow
			End Get
		End Property

		Public Overridable ReadOnly Property CodeName As String Implements SequenceLearningAlgorithm(Of T).getCodeName
			Get
				Return "PV-DM"
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void configure(@NonNull VocabCache<T> vocabCache, @NonNull WeightLookupTable<T> lookupTable, @NonNull VectorsConfiguration configuration)
		Public Overridable Sub configure(ByVal vocabCache As VocabCache(Of T), ByVal lookupTable As WeightLookupTable(Of T), ByVal configuration As VectorsConfiguration)
			Me.vocabCache = vocabCache
			Me.lookupTable = lookupTable
			Me.configuration = configuration

			cbow.configure(vocabCache, lookupTable, configuration)

			Me.window = configuration.getWindow()
			Me.useAdaGrad = configuration.isUseAdaGrad()
			Me.negative = configuration.getNegative()
			Me.sampling = configuration.getSampling()

			Me.syn0 = CType(lookupTable, InMemoryLookupTable(Of T)).getSyn0()
			Me.syn1 = CType(lookupTable, InMemoryLookupTable(Of T)).getSyn1()
			Me.syn1Neg = CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg
			Me.expTable = CType(lookupTable, InMemoryLookupTable(Of T)).ExpTable
			Me.table = CType(lookupTable, InMemoryLookupTable(Of T)).Table
		End Sub

		Public Overridable Sub pretrain(ByVal iterator As SequenceIterator(Of T)) Implements SequenceLearningAlgorithm(Of T).pretrain
			' no-op
		End Sub

		Public Overridable Function learnSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal learningRate As Double, ByVal batchSequences As BatchSequences(Of T)) As Double Implements SequenceLearningAlgorithm(Of T).learnSequence
			Dim seq As Sequence(Of T) = cbow.applySubsampling(sequence, nextRandom)

			If sequence.SequenceLabel Is Nothing Then
				Return 0
			End If

			Dim labels As IList(Of T) = New List(Of T)()
			CType(labels, List(Of T)).AddRange(sequence.getSequenceLabels())

			If seq.Empty OrElse labels.Count = 0 Then
				Return 0
			End If


			Dim i As Integer = 0
			Do While i < seq.size()
				nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
				dm(i, seq, CInt(Math.Truncate(nextRandom.get())) Mod window, nextRandom, learningRate, labels, False, Nothing, batchSequences)
				i += 1
			Loop

			Return 0
		End Function

		Public Overridable Sub dm(ByVal i As Integer, ByVal sequence As Sequence(Of T), ByVal b As Integer, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal labels As IList(Of T), ByVal isInference As Boolean, ByVal inferenceVector As INDArray, ByVal batchSequences As BatchSequences(Of T))
			Dim [end] As Integer = window * 2 + 1 - b

			Dim currentWord As T = sequence.getElementByIndex(i)

			Dim intsList As IList(Of Integer) = New List(Of Integer)()
			Dim statusesList As IList(Of Boolean) = New List(Of Boolean)()
			For a As Integer = b To [end] - 1
				If a <> window Then
					Dim c As Integer = i - window + a
					If c >= 0 AndAlso c < sequence.size() Then
						Dim lastWord As T = sequence.getElementByIndex(c)

						intsList.Add(lastWord.getIndex())
						statusesList.Add(lastWord.isLocked())
					End If
				End If
			Next a

			' appending labels indexes
			If labels IsNot Nothing Then
				For Each label As T In labels
					intsList.Add(label.getIndex())
				Next label
			End If

			Dim windowWords(intsList.Count - 1) As Integer
			Dim statuses(intsList.Count - 1) As Boolean
			For x As Integer = 0 To windowWords.Length - 1
				windowWords(x) = intsList(x)
				statuses(x) = False
			Next x

			Dim batchSize As Integer = configuration.getBatchSize()
			If batchSize = 1 OrElse isInference Then
				' pass for underlying
				cbow.iterateSample(currentWord, windowWords, statuses, nextRandom, alpha, isInference,If(labels Is Nothing, 0, labels.Count), configuration.isTrainElementsVectors(), inferenceVector)
			Else
				batchSequences.put(currentWord, windowWords, statuses, nextRandom.get(), alpha,If(labels Is Nothing, 0, labels.Count))
			End If

			If cbow.getBatch() IsNot Nothing AndAlso cbow.getBatch().Count >= configuration.getBatchSize() Then
				Nd4j.Executioner.exec(cbow.getBatch())
				cbow.getBatch().Clear()
			End If
		End Sub

		Public Overridable ReadOnly Property EarlyTerminationHit As Boolean Implements SequenceLearningAlgorithm(Of T).isEarlyTerminationHit
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' This method does training on previously unseen paragraph, and returns inferred vector
		''' </summary>
		''' <param name="sequence"> </param>
		''' <param name="nr"> </param>
		''' <param name="learningRate">
		''' @return </param>
		Public Overridable Function inferSequence(ByVal sequence As Sequence(Of T), ByVal nr As Long, ByVal learningRate As Double, ByVal minLearningRate As Double, ByVal iterations As Integer) As INDArray Implements SequenceLearningAlgorithm(Of T).inferSequence
			Dim nextRandom As New AtomicLong(nr)

			' we probably don't want subsampling here
			' Sequence<T> seq = cbow.applySubsampling(sequence, nextRandom);
			' if (sequence.getSequenceLabel() == null) throw new IllegalStateException("Label is NULL");

			If sequence.Empty Then
				Return Nothing
			End If

			Dim random As Random = Nd4j.RandomFactory.getNewRandomInstance(configuration.getSeed() * sequence.GetHashCode(), lookupTable.layerSize() + 1)
			Dim ret As INDArray = Nd4j.rand(New Integer() {1, lookupTable.layerSize()}, random).subi(0.5).divi(lookupTable.layerSize())

			log.info("Inf before: {}", ret)

			For iter As Integer = 0 To iterations - 1
				Dim i As Integer = 0
				Do While i < sequence.size()
					nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
					dm(i, sequence, CInt(Math.Truncate(nextRandom.get())) Mod window, nextRandom, learningRate, Nothing, True, ret, Nothing)
					i += 1
				Loop
				learningRate = ((learningRate - minLearningRate) / (iterations - iter)) + minLearningRate
			Next iter

			finish()

			Return ret
		End Function


		Public Overridable Sub finish() Implements SequenceLearningAlgorithm(Of T).finish
			If cbow IsNot Nothing AndAlso cbow.getBatch() IsNot Nothing AndAlso cbow.getBatch().Count > 0 Then
				Nd4j.Executioner.exec(cbow.getBatch())
				cbow.getBatch().Clear()
			End If
		End Sub
	End Class

End Namespace