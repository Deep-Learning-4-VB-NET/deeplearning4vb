Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.inmemory
Imports org.deeplearning4j.models.embeddings.learning
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Aggregate = org.nd4j.linalg.api.ops.aggregates.Aggregate
Imports SkipGramRound = org.nd4j.linalg.api.ops.impl.nlp.SkipGramRound
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DeviceLocalNDArray = org.nd4j.linalg.util.DeviceLocalNDArray
import static org.datavec.api.transform.ColumnType.NDArray

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

Namespace org.deeplearning4j.models.embeddings.learning.impl.elements


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SkipGram<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.deeplearning4j.models.embeddings.learning.ElementsLearningAlgorithm<T>
	Public Class SkipGram(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements ElementsLearningAlgorithm(Of T)

		Protected Friend vocabCache As VocabCache(Of T)
		Protected Friend lookupTable As WeightLookupTable(Of T)
		Protected Friend configuration As VectorsConfiguration

		Protected Friend window As Integer
		Protected Friend useAdaGrad As Boolean
		Protected Friend negative As Double
		Protected Friend sampling As Double
		Protected Friend variableWindows() As Integer
		Protected Friend vectorLength As Integer
'JAVA TO VB CONVERTER NOTE: The field workers was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend workers_Conflict As Integer = Runtime.getRuntime().availableProcessors()

		Public Overridable Property Workers As Integer
			Get
				Return workers_Conflict
			End Get
			Set(ByVal workers As Integer)
				Me.workers_Conflict = workers
			End Set
		End Property


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.nd4j.linalg.util.DeviceLocalNDArray syn0, syn1, syn1Neg, table, expTable;
		Protected Friend syn0, syn1, syn1Neg, table, expTable As DeviceLocalNDArray

		Protected Friend batches As New ThreadLocal(Of IList(Of Aggregate))()

		'private BatchSequences<T> batchSequences;

		''' <summary>
		''' Dummy construction is required for reflection
		''' </summary>
		Public Sub New()

		End Sub

		Public Overridable ReadOnly Property Batch As IList(Of Aggregate)
			Get
				Return batches.get()
			End Get
		End Property

		''' <summary>
		''' Returns implementation code name
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property CodeName As String Implements ElementsLearningAlgorithm(Of T).getCodeName
			Get
				Return "SkipGram"
			End Get
		End Property

		''' <summary>
		''' SkipGram initialization over given vocabulary and WeightLookupTable
		''' </summary>
		''' <param name="vocabCache"> </param>
		''' <param name="lookupTable"> </param>
		''' <param name="configuration"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void configure(@NonNull VocabCache<T> vocabCache, @NonNull WeightLookupTable<T> lookupTable, @NonNull VectorsConfiguration configuration)
		Public Overridable Sub configure(ByVal vocabCache As VocabCache(Of T), ByVal lookupTable As WeightLookupTable(Of T), ByVal configuration As VectorsConfiguration)
			Me.vocabCache = vocabCache
			Me.lookupTable = lookupTable
			Me.configuration = configuration

			If configuration.getNegative() > 0 Then
				If CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg Is Nothing Then
					log.info("Initializing syn1Neg...")
					CType(lookupTable, InMemoryLookupTable(Of T)).UseHS = configuration.isUseHierarchicSoftmax()
					CType(lookupTable, InMemoryLookupTable(Of T)).Negative = configuration.getNegative()
					CType(lookupTable, InMemoryLookupTable(Of T)).resetWeights(False)
				End If
			End If

			Me.syn0 = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).getSyn0())
			Me.syn1 = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).getSyn1())
			Me.syn1Neg = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg)
			Me.expTable = New DeviceLocalNDArray(Nd4j.create(CType(lookupTable, InMemoryLookupTable(Of T)).ExpTable, New Long(){CType(lookupTable, InMemoryLookupTable(Of T)).ExpTable.Length}, syn0.get().dataType()))
			Me.table = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Table)



			Me.window = configuration.getWindow()
			Me.useAdaGrad = configuration.isUseAdaGrad()
			Me.negative = configuration.getNegative()
			Me.sampling = configuration.getSampling()
			Me.variableWindows = configuration.getVariableWindows()

			Me.vectorLength = configuration.getLayersSize()
		End Sub

		''' <summary>
		''' SkipGram doesn't involves any pretraining
		''' </summary>
		''' <param name="iterator"> </param>
		Public Overridable Sub pretrain(ByVal iterator As SequenceIterator(Of T)) Implements ElementsLearningAlgorithm(Of T).pretrain
			' no-op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.deeplearning4j.models.sequencevectors.sequence.Sequence<T> applySubsampling(@NonNull Sequence<T> sequence, @NonNull AtomicLong nextRandom)
		Public Overridable Function applySubsampling(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong) As Sequence(Of T)
			Dim result As New Sequence(Of T)()

			' subsampling implementation, if subsampling threshold met, just continue to next element
			If sampling > 0 Then
				result.setSequenceId(sequence.getSequenceId())
				If sequence.getSequenceLabels() IsNot Nothing Then
					result.SequenceLabels = sequence.getSequenceLabels()
				End If
				If sequence.getSequenceLabel() IsNot Nothing Then
					result.SequenceLabel = sequence.getSequenceLabel()
				End If

				For Each element As T In sequence.getElements()
					Dim numWords As Double = vocabCache.totalWordOccurrences()
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					Dim ran As Double = (Math.Sqrt(element.getElementFrequency() / (sampling * numWords)) + 1) * (sampling * numWords) / element.getElementFrequency()

					nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))

					If ran < (nextRandom.get() And &HFFFF) / CDbl(65536) Then
						Continue For
					End If
					result.addElement(element)
				Next element
				Return result
			Else
				Return sequence
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public double learnSequence(@NonNull Sequence<T> sequence, @NonNull AtomicLong nextRandom, double learningRate, BatchSequences<T> batchSequences)
		Public Overridable Function learnSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal learningRate As Double, ByVal batchSequences As BatchSequences(Of T)) As Double
			Dim tempSequence As Sequence(Of T) = sequence
			If sampling > 0 Then
				tempSequence = applySubsampling(sequence, nextRandom)
			End If

			Dim score As Double = 0.0

			Dim currentWindow As Integer = window

			If variableWindows IsNot Nothing AndAlso variableWindows.Length <> 0 Then
				currentWindow = variableWindows(RandomUtils.nextInt(0, variableWindows.Length))
			End If

			Dim i As Integer = 0
			Do While i < tempSequence.getElements().size()
				nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
				score = skipGram(i, tempSequence.getElements(), CInt(Math.Truncate(nextRandom.get())) Mod currentWindow, nextRandom, learningRate, currentWindow, batchSequences)
				i += 1
			Loop
	'        int batchSize = configuration.getBatchSize();
	'        if (batchSize > 1 && batchSequences != null && batchSequences.size() >= batchSize) {
	'            int rest = batchSequences.size() % batchSize;
	'            int chunks = ((batchSequences.size() >= batchSize) ? batchSequences.size() / batchSize : 0) + ((rest > 0)? 1 : 0);
	'            for (int j = 0; j < chunks; ++j) {
	'                score = iterateSample(batchSequences.get(j));
	'            }
	'            batchSequences.clear();
	'        }

			If batches IsNot Nothing AndAlso batches.get() IsNot Nothing AndAlso batches.get().size() >= configuration.getBatchSize() Then
				Nd4j.Executioner.exec(batches.get())
				batches.get().clear()
			End If

			Return score
		End Function
		''' <summary>
		''' Learns sequence using SkipGram algorithm
		''' </summary>
		''' <param name="sequence"> </param>
		''' <param name="nextRandom"> </param>
		''' <param name="learningRate"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public double learnSequence(@NonNull Sequence<T> sequence, @NonNull AtomicLong nextRandom, double learningRate)
		Public Overridable Function learnSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal learningRate As Double) As Double
			Dim tempSequence As Sequence(Of T) = sequence
			If sampling > 0 Then
				tempSequence = applySubsampling(sequence, nextRandom)
			End If

			Dim score As Double = 0.0

			Dim currentWindow As Integer = window

			If variableWindows IsNot Nothing AndAlso variableWindows.Length <> 0 Then
				currentWindow = variableWindows(RandomUtils.nextInt(0, variableWindows.Length))
			End If
			'batchSequences = new BatchSequences<>(configuration.getBatchSize());
			Dim i As Integer = 0
			Do While i < tempSequence.getElements().size()
				nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
				score = skipGram(i, tempSequence.getElements(), CInt(Math.Truncate(nextRandom.get())) Mod currentWindow, nextRandom, learningRate, currentWindow)
				i += 1
			Loop
	'        int batchSize = configuration.getBatchSize();
	'        if (batchSize > 1 && batchSequences != null) {
	'            int rest = batchSequences.size() % batchSize;
	'            int chunks = ((batchSequences.size() >= batchSize) ? batchSequences.size() / batchSize : 0) + ((rest > 0)? 1 : 0);
	'            for (int j = 0; j < chunks; ++j) {
	'                score = iterateSample(batchSequences.get(j));
	'            }
	'            batchSequences.clear();
	'        }

			If batches IsNot Nothing AndAlso batches.get() IsNot Nothing AndAlso batches.get().size() >= configuration.getBatchSize() Then
				Nd4j.Executioner.exec(batches.get())
				batches.get().clear()
			End If

			Return score
		End Function

		Public Overridable Sub finish() Implements ElementsLearningAlgorithm(Of T).finish
			If batches IsNot Nothing AndAlso batches.get() IsNot Nothing AndAlso Not batches.get().isEmpty() Then
				Nd4j.Executioner.exec(batches.get())
				batches.get().clear()
			End If
		End Sub

		''' <summary>
		''' SkipGram has no reasons for early termination ever.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property EarlyTerminationHit As Boolean Implements ElementsLearningAlgorithm(Of T).isEarlyTerminationHit
			Get
				Return False
			End Get
		End Property

		Private Function skipGram(ByVal i As Integer, ByVal sentence As IList(Of T), ByVal b As Integer, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal currentWindow As Integer) As Double
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final T word = sentence.get(i);
			Dim word As T = sentence(i)
			If word Is Nothing OrElse sentence.Count = 0 Then
				Return 0.0
			End If

			Dim score As Double = 0.0
			Dim batchSize As Integer = configuration.getBatchSize()

			Dim [end] As Integer = currentWindow * 2 + 1 - b
			For a As Integer = b To [end] - 1
				If a <> currentWindow Then
					Dim c As Integer = i - currentWindow + a
					If c >= 0 AndAlso c < sentence.Count Then
						Dim lastWord As T = sentence(c)
						score = iterateSample(word, lastWord, nextRandom, alpha, False, Nothing)
					End If
				End If
			Next a

			Return score
		End Function

		Private Function skipGram(ByVal i As Integer, ByVal sentence As IList(Of T), ByVal b As Integer, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal currentWindow As Integer, ByVal batchSequences As BatchSequences(Of T)) As Double
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final T word = sentence.get(i);
			Dim word As T = sentence(i)
			If word Is Nothing OrElse sentence.Count = 0 OrElse word.isLocked() Then
				Return 0.0
			End If

			Dim score As Double = 0.0
			Dim batchSize As Integer = configuration.getBatchSize()

			Dim [end] As Integer = currentWindow * 2 + 1 - b
			For a As Integer = b To [end] - 1
				If a <> currentWindow Then
					Dim c As Integer = i - currentWindow + a
					If c >= 0 AndAlso c < sentence.Count Then
						Dim lastWord As T = sentence(c)
						nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
						If batchSize <= 1 Then
							score = iterateSample(word, lastWord, nextRandom, alpha, False, Nothing)
						Else
							batchSequences.put(word, lastWord, nextRandom.get(), alpha)
						End If
					End If
				End If
			Next a

			Return score
		End Function

		Public Overridable Function iterateSample(ByVal w1 As T, ByVal lastWord As T, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal isInference As Boolean, ByVal inferenceVector As INDArray) As Double
			If w1 Is Nothing OrElse lastWord Is Nothing OrElse (lastWord.getIndex() < 0 AndAlso Not isInference) OrElse w1.getIndex() = lastWord.getIndex() OrElse w1.getLabel().Equals("STOP") OrElse lastWord.getLabel().Equals("STOP") OrElse w1.getLabel().Equals("UNK") OrElse lastWord.getLabel().Equals("UNK") Then
				Return 0.0
			End If


			Dim score As Double = 0.0

			Dim idxSyn1() As Integer = Nothing
			Dim codes() As SByte = Nothing
			If configuration.isUseHierarchicSoftmax() Then
				idxSyn1 = New Integer(w1.getCodeLength() - 1){}
				codes = New SByte(w1.getCodeLength() - 1){}
				Dim i As Integer = 0
				Do While i < w1.getCodeLength()
					Dim code As Integer = w1.getCodes().get(i)
					Dim point As Integer = w1.getPoints().get(i)
					If point >= vocabCache.numWords() OrElse point < 0 Then
						i += 1
						Continue Do
					End If

					codes(i) = CSByte(code)
					idxSyn1(i) = point
					i += 1
				Loop
			Else
				idxSyn1 = New Integer(){}
				codes = New SByte(){}
			End If


			Dim target As Integer = w1.getIndex()
			'negative sampling
			If negative > 0 Then
				If syn1Neg Is Nothing Then
					CType(lookupTable, InMemoryLookupTable(Of T)).initNegative()
					syn1Neg = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg)
				End If
			End If

			If batches.get() Is Nothing Then
				batches.set(New List(Of Aggregate)())
			End If

			'log.info("VocabWords: {}; lastWordIndex: {}; syn1neg: {}", vocabCache.numWords(), lastWord.getIndex(), syn1Neg.get().rows());

	'        AggregateSkipGram sg = new AggregateSkipGram(syn0.get(), syn1.get(), syn1Neg.get(), expTable.get(), table.get(),
	'                        lastWord.getIndex(), idxSyn1, codes, (int) negative, target, vectorLength, alpha,
	'                        nextRandom.get(), vocabCache.numWords(), inferenceVector);
	'        if (!isInference) {
	'            batches.get().add(sg);
	'            if (batches.get().size() > 4096) {
	'                Nd4j.getExecutioner().exec(batches.get());
	'                batches.get().clear();
	'            }
	'        } else {
	'            Nd4j.getExecutioner().exec(sg);
	'        }

			nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))

			Dim sg As SkipGramRound = Nothing
			Dim useHS As Boolean = configuration.isUseHierarchicSoftmax()
			Dim useNegative As Boolean = configuration.getNegative() > 0

			Dim intCodes(codes.Length - 1) As Integer
			For i As Integer = 0 To codes.Length - 1
				intCodes(i) = codes(i)
			Next i

			If useHS AndAlso useNegative Then
				sg = New SkipGramRound(Nd4j.scalar(lastWord.getIndex()), Nd4j.scalar(target), syn0.get(), syn1.get(), syn1Neg.get(), expTable.get(), table.get(), CInt(Math.Truncate(negative)), Nd4j.create(idxSyn1), Nd4j.create(intCodes), Nd4j.scalar(alpha), Nd4j.scalar(nextRandom.get()),If(inferenceVector IsNot Nothing, inferenceVector, Nd4j.empty(syn0.get().dataType())), configuration.isPreciseMode(), workers_Conflict)
			ElseIf useHS Then
				sg = New SkipGramRound(lastWord.getIndex(), syn0.get(), syn1.get(), expTable.get(), idxSyn1, codes, alpha, nextRandom.get(),If(inferenceVector IsNot Nothing, inferenceVector, Nd4j.empty(syn0.get().dataType())))
			ElseIf useNegative Then
				sg = New SkipGramRound(lastWord.getIndex(), target, syn0.get(), syn1Neg.get(), expTable.get(), table.get(), CInt(Math.Truncate(negative)), alpha, nextRandom.get(),If(inferenceVector IsNot Nothing, inferenceVector, Nd4j.empty(syn0.get().dataType())))
			End If

			Nd4j.Executioner.exec(sg)


			Return score
		End Function

		Public Overridable Function iterateSample(ByVal items As IList(Of BatchItem(Of T))) As Double

			Dim useHS As Boolean = configuration.isUseHierarchicSoftmax()
			Dim useNegative As Boolean = configuration.getNegative() > 0

			Dim score As Double = 0.0
			Dim isInference As Boolean = False

			Dim targets(items.Count - 1) As Integer
			Dim starters(items.Count - 1) As Integer
			Dim alphas(items.Count - 1) As Double
			Dim randomValues(items.Count - 1) As Long

			Dim maxCols As Integer = 1
			For i As Integer = 0 To items.Count - 1
				Dim curr As Integer = items(i).getWord().getCodeLength()
				If curr > maxCols Then
					maxCols = curr
				End If
			Next i
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim codes[][] As SByte = new SByte[items.Count][maxCols]
			Dim codes()() As SByte = RectangularArrays.RectangularSByteArray(items.Count, maxCols)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim indices[][] As Integer = new Integer[items.Count][maxCols]
			Dim indices()() As Integer = RectangularArrays.RectangularIntegerArray(items.Count, maxCols)

			For cnt As Integer = 0 To items.Count - 1
				Dim w1 As T = items(cnt).getWord()
				Dim lastWord As T = items(cnt).getLastWord()
				randomValues(cnt) = items(cnt).getRandomValue()
				Dim alpha As Double = items(cnt).getAlpha()

				If w1 Is Nothing OrElse lastWord Is Nothing OrElse (lastWord.getIndex() < 0 AndAlso Not isInference) OrElse w1.getIndex() = lastWord.getIndex() OrElse w1.getLabel().Equals("STOP") OrElse lastWord.getLabel().Equals("STOP") OrElse w1.getLabel().Equals("UNK") OrElse lastWord.getLabel().Equals("UNK") Then
					Continue For
				End If

				Dim target As Integer = lastWord.getIndex()
				Dim ngStarter As Integer = w1.getIndex()

				targets(cnt) = target
				starters(cnt) = ngStarter
				alphas(cnt) = alpha

				Dim idxSyn1() As Integer = Nothing
				Dim interimCodes() As SByte = Nothing
				If useHS Then
					idxSyn1 = New Integer(w1.getCodeLength() - 1){}
					interimCodes = New SByte(w1.getCodeLength() - 1){}
					Dim i As Integer = 0
					Do While i < w1.getCodeLength()
						Dim code As Integer = w1.getCodes().get(i)
						Dim point As Integer = w1.getPoints().get(i)
						If point >= vocabCache.numWords() OrElse point < 0 Then
							i += 1
							Continue Do
						End If

						interimCodes(i) = CSByte(code)
						idxSyn1(i) = point
						i += 1
					Loop
					For i As Integer = 0 To maxCols - 1
						If i < w1.getCodeLength() Then
							codes(cnt)(i) = interimCodes(i)
						Else
							codes(cnt)(i) = -1
						End If
					Next i
					For i As Integer = 0 To maxCols - 1
						If i < w1.getCodeLength() Then
							indices(cnt)(i) = idxSyn1(i)
						Else
							indices(cnt)(i) = -1
						End If
					Next i

				Else
					idxSyn1 = New Integer(){}
					interimCodes = New SByte(){}
					codes = New SByte()(){}
					indices = New Integer()(){}
				End If

				'negative sampling
				If negative > 0 Then
					If syn1Neg Is Nothing Then
						CType(lookupTable, InMemoryLookupTable(Of T)).initNegative()
						syn1Neg = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg)
					End If
				End If
			Next cnt
			Dim targetArray As INDArray = Nd4j.createFromArray(targets)
			Dim ngStarterArray As INDArray = Nd4j.createFromArray(starters)
			Dim alphasArray As INDArray = Nd4j.createFromArray(alphas)
			Dim randomValuesArray As INDArray = Nd4j.createFromArray(randomValues)
			Dim indicesArray As INDArray = Nd4j.createFromArray(indices)
			Dim codesArray As INDArray = Nd4j.createFromArray(codes)

			Dim sg As val = New SkipGramRound(targetArray,If(negative > 0, ngStarterArray, Nd4j.empty(DataType.INT)), syn0.get(),If(useHS, syn1.get(), Nd4j.empty(syn0.get().dataType())),If(negative > 0, syn1Neg.get(), Nd4j.empty(syn0.get().dataType())), expTable.get(),If(negative > 0, table.get(), Nd4j.empty(syn0.get().dataType())), CInt(Math.Truncate(negative)),If(useHS, indicesArray, Nd4j.empty(DataType.INT)),If(useHS, codesArray, Nd4j.empty(DataType.BYTE)), alphasArray, randomValuesArray, Nd4j.empty(syn0.get().dataType()), configuration.isPreciseMode(), workers_Conflict)

			Nd4j.Executioner.exec(sg)

			Return score
		End Function
	End Class
End Namespace