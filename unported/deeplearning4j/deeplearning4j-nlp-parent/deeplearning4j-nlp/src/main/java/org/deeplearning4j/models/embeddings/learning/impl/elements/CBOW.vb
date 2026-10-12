Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
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
Imports CbowRound = org.nd4j.linalg.api.ops.impl.nlp.CbowRound
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DeviceLocalNDArray = org.nd4j.linalg.util.DeviceLocalNDArray
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

Namespace org.deeplearning4j.models.embeddings.learning.impl.elements


	Public Class CBOW(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements ElementsLearningAlgorithm(Of T)

		Private vocabCache As VocabCache(Of T)
		Private lookupTable As WeightLookupTable(Of T)
		Private configuration As VectorsConfiguration

		Private Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(CBOW))

		Protected Friend Shared MAX_EXP As Double = 6

		Protected Friend window As Integer
		Protected Friend useAdaGrad As Boolean
		Protected Friend negative As Double
		Protected Friend sampling As Double
		Protected Friend variableWindows() As Integer
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
'ORIGINAL LINE: @Getter @Setter protected org.nd4j.linalg.util.DeviceLocalNDArray syn0, syn1, syn1Neg, expTable, table;
		Protected Friend syn0, syn1, syn1Neg, expTable, table As DeviceLocalNDArray

		Protected Friend batches As New ThreadLocal(Of IList(Of Aggregate))()

		Public Overridable ReadOnly Property Batch As IList(Of Aggregate)
			Get
				Return batches.get()
			End Get
		End Property

		Public Overridable ReadOnly Property CodeName As String Implements ElementsLearningAlgorithm(Of T).getCodeName
			Get
				Return "CBOW"
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void configure(@NonNull VocabCache<T> vocabCache, @NonNull WeightLookupTable<T> lookupTable, @NonNull VectorsConfiguration configuration)
		Public Overridable Sub configure(ByVal vocabCache As VocabCache(Of T), ByVal lookupTable As WeightLookupTable(Of T), ByVal configuration As VectorsConfiguration)
			Me.vocabCache = vocabCache
			Me.lookupTable = lookupTable
			Me.configuration = configuration

			Me.window = configuration.getWindow()
			Me.useAdaGrad = configuration.isUseAdaGrad()
			Me.negative = configuration.getNegative()
			Me.sampling = configuration.getSampling()

			If configuration.getNegative() > 0 Then
				If CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg Is Nothing Then
					logger.info("Initializing syn1Neg...")
					CType(lookupTable, InMemoryLookupTable(Of T)).UseHS = configuration.isUseHierarchicSoftmax()
					CType(lookupTable, InMemoryLookupTable(Of T)).Negative = configuration.getNegative()
					CType(lookupTable, InMemoryLookupTable(Of T)).resetWeights(False)
				End If
			End If


			Me.syn0 = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).getSyn0())
			Me.syn1 = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).getSyn1())
			Me.syn1Neg = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg)
			'this.expTable = new DeviceLocalNDArray(Nd4j.create(((InMemoryLookupTable<T>) lookupTable).getExpTable()));
			Me.expTable = New DeviceLocalNDArray(Nd4j.create(CType(lookupTable, InMemoryLookupTable(Of T)).ExpTable, New Long(){CType(lookupTable, InMemoryLookupTable(Of T)).ExpTable.Length}, syn0.get().dataType()))
			Me.table = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Table)
			Me.variableWindows = configuration.getVariableWindows()
		End Sub

		''' <summary>
		''' CBOW doesn't involve any pretraining
		''' </summary>
		''' <param name="iterator"> </param>
		Public Overridable Sub pretrain(ByVal iterator As SequenceIterator(Of T)) Implements ElementsLearningAlgorithm(Of T).pretrain
			' no-op
		End Sub

		Public Overridable Sub finish() Implements ElementsLearningAlgorithm(Of T).finish
			If batches IsNot Nothing AndAlso batches.get() IsNot Nothing AndAlso Not batches.get().isEmpty() Then
				Nd4j.Executioner.exec(batches.get())
				batches.get().clear()
			End If
		End Sub

		Public Overridable Function learnSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal learningRate As Double, ByVal batchSequences As BatchSequences(Of T)) As Double
			Dim tempSequence As Sequence(Of T) = sequence

			If sampling > 0 Then
				tempSequence = applySubsampling(sequence, nextRandom)
			End If

			Dim currentWindow As Integer = window

			If variableWindows IsNot Nothing AndAlso variableWindows.Length <> 0 Then
				currentWindow = variableWindows(RandomUtils.nextInt(0, variableWindows.Length))
			End If

			Dim i As Integer = 0
			Do While i < tempSequence.getElements().size()
				nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
				cbow(i, tempSequence.getElements(), CInt(Math.Truncate(nextRandom.get())) Mod currentWindow, nextRandom, learningRate, currentWindow, batchSequences)
				i += 1
			Loop

			Return 0
		End Function

		Public Overridable Function learnSequence(ByVal sequence As Sequence(Of T), ByVal nextRandom As AtomicLong, ByVal learningRate As Double) As Double Implements ElementsLearningAlgorithm(Of T).learnSequence
			Dim tempSequence As Sequence(Of T) = sequence
			If sampling > 0 Then
				tempSequence = applySubsampling(sequence, nextRandom)
			End If

			Dim currentWindow As Integer = window

			If variableWindows IsNot Nothing AndAlso variableWindows.Length <> 0 Then
				currentWindow = variableWindows(RandomUtils.nextInt(0, variableWindows.Length))
			End If

			Dim i As Integer = 0
			Do While i < tempSequence.getElements().size()
				nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
				cbow(i, tempSequence.getElements(), CInt(Math.Truncate(nextRandom.get())) Mod currentWindow, nextRandom, learningRate, currentWindow, Nothing)
				i += 1
			Loop

			Return 0
		End Function

		Public Overridable ReadOnly Property EarlyTerminationHit As Boolean Implements ElementsLearningAlgorithm(Of T).isEarlyTerminationHit
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub iterateSample(ByVal currentWord As T, ByVal windowWords() As Integer, ByVal wordStatuses() As Boolean, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal isInference As Boolean, ByVal numLabels As Integer, ByVal trainWords As Boolean, ByVal inferenceVector As INDArray)
			Dim idxSyn1() As Integer = Nothing
			Dim codes() As SByte = Nothing

			If configuration.isUseHierarchicSoftmax() Then
				idxSyn1 = New Integer(currentWord.getCodeLength() - 1){}
				codes = New SByte(currentWord.getCodeLength() - 1){}
				Dim p As Integer = 0
				Do While p < currentWord.getCodeLength()
					If currentWord.getPoints().get(p) < 0 Then
						p += 1
						Continue Do
					End If

					codes(p) = currentWord.getCodes().get(p)
					idxSyn1(p) = currentWord.getPoints().get(p)
					p += 1
				Loop
			Else
				idxSyn1 = New Integer(){}
				codes = New SByte(){}
			End If


			If negative > 0 Then
				If syn1Neg Is Nothing Then
					CType(lookupTable, InMemoryLookupTable(Of T)).initNegative()
					syn1Neg = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg)
				End If
			End If

			If batches.get() Is Nothing Then
				batches.set(New List(Of Aggregate)())
			End If

	'        AggregateCBOW(syn0.get(), syn1.get(), syn1Neg.get(), expTable.get(), table.get(),
	'                currentWord.getIndex(), windowWords, idxSyn1, codes, (int) negative, currentWord.getIndex(),
	'                lookupTable.layerSize(), alpha, nextRandom.get(), vocabCache.numWords(), numLabels, trainWords,
	'                inferenceVector);

			Dim useHS As Boolean = configuration.isUseHierarchicSoftmax()
			Dim useNegative As Boolean = configuration.getNegative() > 0

			Dim inputStatuses(windowWords.Length - 1) As Integer
			For i As Integer = 0 To windowWords.Length - 1
				If i < wordStatuses.Length Then
					inputStatuses(i) = If(wordStatuses(i), 1, 0)
				Else
					inputStatuses(i) = -1
				End If
			Next i
			Dim wordsStatuses As INDArray = Nd4j.createFromArray(inputStatuses)

			Dim cbow As CbowRound = Nothing

			If useHS AndAlso useNegative Then
				cbow = New CbowRound(Nd4j.scalar(currentWord.getIndex()), Nd4j.createFromArray(windowWords), wordsStatuses, Nd4j.scalar(currentWord.getIndex()), syn0.get(), syn1.get(), syn1Neg.get(), expTable.get(), table.get(), Nd4j.createFromArray(idxSyn1), Nd4j.createFromArray(codes), CInt(Math.Truncate(negative)), Nd4j.scalar(alpha), Nd4j.scalar(nextRandom.get()),If(inferenceVector IsNot Nothing, inferenceVector, Nd4j.empty(syn0.get().dataType())), Nd4j.empty(DataType.INT), trainWords, workers_Conflict)
			ElseIf useHS Then
				cbow = New CbowRound(currentWord.getIndex(), windowWords, wordsStatuses.toIntVector(), syn0.get(), syn1.get(), expTable.get(), idxSyn1, codes, alpha, nextRandom.get(),If(inferenceVector IsNot Nothing, inferenceVector, Nd4j.empty(syn0.get().dataType())), 0)
			ElseIf useNegative Then
				cbow = New CbowRound(currentWord.getIndex(), windowWords, wordsStatuses.toIntVector(), currentWord.getIndex(), syn0.get(), syn1Neg.get(), expTable.get(), table.get(), CInt(Math.Truncate(negative)), alpha, nextRandom.get(),If(inferenceVector IsNot Nothing, inferenceVector, Nd4j.empty(syn0.get().dataType())), 0)
			End If

			nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
			Nd4j.Executioner.exec(cbow)

	'        if (!isInference) {
	'            batches.get().add(cbow);
	'            if (batches.get().size() > 4096) {
	'                Nd4j.getExecutioner().exec(batches.get());
	'                batches.get().clear();
	'            }
	'        } else
	'            Nd4j.getExecutioner().exec(cbow);

		End Sub

		Public Overridable Sub iterateSample(ByVal items As IList(Of BatchItem(Of T)))

			Dim useHS As Boolean = configuration.isUseHierarchicSoftmax()
			Dim useNegative As Boolean = configuration.getNegative() > 0

			Dim idxSyn1() As Integer = Nothing
			Dim codes() As SByte = Nothing

			Dim maxCols As Integer = 1
			For i As Integer = 0 To items.Count - 1
				Dim curr As Integer = items(i).getWord().getCodeLength()
				If curr > maxCols Then
					maxCols = curr
				End If
			Next i

'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim inputCodes[][] As SByte = new SByte[items.Count][maxCols]
			Dim inputCodes()() As SByte = RectangularArrays.RectangularSByteArray(items.Count, maxCols)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim inputIndices[][] As Integer = new Integer[items.Count][maxCols]
			Dim inputIndices()() As Integer = RectangularArrays.RectangularIntegerArray(items.Count, maxCols)
			Dim numLabels(items.Count - 1) As Integer
			Dim hasNumLabels As Boolean = False

			Dim maxWinWordsCols As Integer = -1
			For i As Integer = 0 To items.Count - 1
				Dim curr As Integer = items(i).getWindowWords().length
				If curr > maxWinWordsCols Then
					maxWinWordsCols = curr
				End If
			Next i
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim inputWindowWords[][] As Integer = new Integer[items.Count][maxWinWordsCols]
			Dim inputWindowWords()() As Integer = RectangularArrays.RectangularIntegerArray(items.Count, maxWinWordsCols)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim inputWordsStatuses[][] As Integer = new Integer[items.Count][maxWinWordsCols]
			Dim inputWordsStatuses()() As Integer = RectangularArrays.RectangularIntegerArray(items.Count, maxWinWordsCols)

			Dim randoms(items.Count - 1) As Long
			Dim alphas(items.Count - 1) As Double
			Dim currentWordIndexes(items.Count - 1) As Integer

			For cnt As Integer = 0 To items.Count - 1

				Dim currentWord As T = items(cnt).getWord()
				currentWordIndexes(cnt) = currentWord.getIndex()

				Dim windowWords() As Integer = items(cnt).getWindowWords().clone()
				Dim windowStatuses() As Boolean = items(cnt).getWordStatuses().clone()

				For i As Integer = 0 To maxWinWordsCols - 1
					If i < windowWords.Length Then
						inputWindowWords(cnt)(i) = windowWords(i)
						inputWordsStatuses(cnt)(i) = If(windowStatuses(i), 1, 0)
					Else
						inputWindowWords(cnt)(i) = -1
						inputWordsStatuses(cnt)(i) = -1
					End If
				Next i

				Dim randomValue As Long = items(cnt).getRandomValue()
				Dim alpha As Double = items(cnt).getAlpha()
				alphas(cnt) = alpha

				randoms(cnt) = randomValue
				numLabels(cnt) = items(cnt).getNumLabel()
				If numLabels(cnt) > 0 Then
					hasNumLabels = True
				End If

				If useHS Then
					idxSyn1 = New Integer(currentWord.getCodeLength() - 1){}
					codes = New SByte(currentWord.getCodeLength() - 1){}
					Dim p As Integer = 0
					Do While p < currentWord.getCodeLength()
						If currentWord.getPoints().get(p) < 0 Then
							p += 1
							Continue Do
						End If

						codes(p) = currentWord.getCodes().get(p)
						idxSyn1(p) = currentWord.getPoints().get(p)
						p += 1
					Loop
					For i As Integer = 0 To maxCols - 1
						If i < currentWord.getCodeLength() Then
							inputCodes(cnt)(i) = codes(i)
						Else
							inputCodes(cnt)(i) = -1
						End If
					Next i
					For i As Integer = 0 To maxCols - 1
						If i < currentWord.getCodeLength() Then
							inputIndices(cnt)(i) = idxSyn1(i)
						Else
							inputIndices(cnt)(i) = -1
						End If
					Next i
				Else
					idxSyn1 = New Integer(){}
					codes = New SByte(){}

					inputIndices = New Integer()(){}
					inputCodes = New SByte()(){}
				End If


				If negative > 0 Then
					If syn1Neg Is Nothing Then
						CType(lookupTable, InMemoryLookupTable(Of T)).initNegative()
						syn1Neg = New DeviceLocalNDArray(CType(lookupTable, InMemoryLookupTable(Of T)).Syn1Neg)
					End If
				End If

				If batches.get() Is Nothing Then
					batches.set(New List(Of Aggregate)())
				End If

				'nextRandom.set(Math.abs(nextRandom.get() * 25214903917L + 11));
			Next cnt

			Dim currentWordIndexesArray As INDArray = Nd4j.createFromArray(currentWordIndexes)
			Dim alphasArray As INDArray = Nd4j.createFromArray(alphas)
			Dim windowWordsArray As INDArray = Nd4j.createFromArray(inputWindowWords)
			Dim wordsStatusesArray As INDArray = Nd4j.createFromArray(inputWordsStatuses)
			Dim codesArray As INDArray = Nd4j.createFromArray(inputCodes)
			Dim indicesArray As INDArray = Nd4j.createFromArray(inputIndices)
			Dim numLabelsArray As INDArray = Nd4j.createFromArray(numLabels)

			Dim cbow As New CbowRound(currentWordIndexesArray, windowWordsArray, wordsStatusesArray, currentWordIndexesArray, syn0.get(),If(useHS, syn1.get(), Nd4j.empty(syn0.get().dataType())),If(negative > 0, syn1Neg.get(), Nd4j.empty(syn0.get().dataType())), expTable.get(),If(negative > 0, table.get(), Nd4j.empty(syn0.get().dataType())),If(useHS, indicesArray, Nd4j.empty(DataType.INT)),If(useHS, codesArray, Nd4j.empty(DataType.BYTE)), CInt(Math.Truncate(negative)), alphasArray, Nd4j.createFromArray(randoms), Nd4j.empty(syn0.get().dataType()),If(hasNumLabels, numLabelsArray, Nd4j.empty(DataType.INT)), configuration.isTrainElementsVectors(), workers_Conflict)

			Nd4j.Executioner.exec(cbow)

	'        if (!isInference) {
	'            batches.get().add(cbow);
	'            if (batches.get().size() > 4096) {
	'                Nd4j.getExecutioner().exec(batches.get());
	'                batches.get().clear();
	'            }
	'        } else
	'            Nd4j.getExecutioner().exec(cbow);

		End Sub

		Public Overridable Sub cbow(ByVal i As Integer, ByVal sentence As IList(Of T), ByVal b As Integer, ByVal nextRandom As AtomicLong, ByVal alpha As Double, ByVal currentWindow As Integer, ByVal batchSequences As BatchSequences(Of T))
			Dim batchSize As Integer = configuration.getBatchSize()

			Dim [end] As Integer = window * 2 + 1 - b

			Dim currentWord As T = sentence(i)

			Dim intsList As IList(Of Integer) = New List(Of Integer)()
			Dim statusesList As IList(Of Boolean) = New List(Of Boolean)()
			For a As Integer = b To [end] - 1
				If a <> currentWindow Then
					Dim c As Integer = i - currentWindow + a
					If c >= 0 AndAlso c < sentence.Count Then
						Dim lastWord As T = sentence(c)

						intsList.Add(lastWord.getIndex())
						statusesList.Add(lastWord.isLocked())
					End If
				End If
			Next a

			Dim windowWords(intsList.Count - 1) As Integer
			Dim statuses(intsList.Count - 1) As Boolean
			For x As Integer = 0 To windowWords.Length - 1
				windowWords(x) = intsList(x)
				statuses(x) = statusesList(x)
			Next x

			' we don't allow inference from main loop here
			If batchSize <= 1 Then
				iterateSample(currentWord, windowWords, statuses, nextRandom, alpha, False, 0, True, Nothing)
			Else
				batchSequences.put(currentWord, windowWords, statuses, nextRandom.get(), alpha)
			End If

			If batches IsNot Nothing AndAlso batches.get() IsNot Nothing AndAlso batches.get().size() >= configuration.getBatchSize() Then
				Nd4j.Executioner.exec(batches.get())
				batches.get().clear()
			End If
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
	End Class

End Namespace