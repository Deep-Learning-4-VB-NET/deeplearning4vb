Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports org.deeplearning4j.models.embeddings
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports Huffman = org.deeplearning4j.models.word2vec.Huffman
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports org.deeplearning4j.text.invertedindex
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports PriorityScheduler = org.threadly.concurrent.PriorityScheduler

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

Namespace org.deeplearning4j.models.word2vec.wordstore


	Public Class VocabConstructor(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Private sources As IList(Of VocabSource(Of T)) = New List(Of VocabSource(Of T))()
		Private cache As VocabCache(Of T)
		Private stopWords As ICollection(Of String)
		Private useAdaGrad As Boolean = False
		Private fetchLabels As Boolean = False
		Private limit As Integer
		Private seqCount As New AtomicLong(0)
		Private index As InvertedIndex(Of T)
		Private enableScavenger As Boolean = False
		Private unk As T
		Private allowParallelBuilder As Boolean = True
		Private lockf As Boolean = False

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(VocabConstructor))

		Private Sub New()

		End Sub

		''' <summary>
		''' Placeholder for future implementation
		''' @return
		''' </summary>
		Protected Friend Overridable Function buildExtendedLookupTable() As WeightLookupTable(Of T)
			Return Nothing
		End Function

		''' <summary>
		''' Placeholder for future implementation
		''' @return
		''' </summary>
		Protected Friend Overridable Function buildExtendedVocabulary() As VocabCache(Of T)
			Return Nothing
		End Function

		''' <summary>
		''' This method transfers existing WordVectors model into current one
		''' </summary>
		''' <param name="wordVectors">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public VocabCache<T> buildMergedVocabulary(@NonNull WordVectors wordVectors, boolean fetchLabels)
		Public Overridable Function buildMergedVocabulary(ByVal wordVectors As WordVectors, ByVal fetchLabels As Boolean) As VocabCache(Of T)
			Return buildMergedVocabulary(CType(wordVectors.vocab(), VocabCache(Of T)), fetchLabels)
		End Function


		''' <summary>
		''' This method returns total number of sequences passed through VocabConstructor
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property NumberOfSequences As Long
			Get
				Return seqCount.get()
			End Get
		End Property


		''' <summary>
		''' This method transfers existing vocabulary into current one
		''' 
		''' Please note: this method expects source vocabulary has Huffman tree indexes applied
		''' </summary>
		''' <param name="vocabCache">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocabCache<T> buildMergedVocabulary(@NonNull VocabCache<T> vocabCache, boolean fetchLabels)
		Public Overridable Function buildMergedVocabulary(ByVal vocabCache As VocabCache(Of T), ByVal fetchLabels As Boolean) As VocabCache(Of T)
			If cache Is Nothing Then
				cache = (New AbstractCache.Builder(Of T)()).build()
			End If
			Dim t As Integer = 0
			Do While t < vocabCache.numWords()
				Dim label As String = vocabCache.wordAtIndex(t)
				If label Is Nothing Then
					t += 1
					Continue Do
				End If
				Dim element As T = vocabCache.wordFor(label)

				' skip this element if it's a label, and user don't want labels to be merged
				If Not fetchLabels AndAlso element.isLabel() Then
					t += 1
					Continue Do
				End If

				'element.setIndex(t);
				cache.addToken(element)
				cache.addWordToIndex(element.getIndex(), element.getLabel())

				' backward compatibility code
				cache.putVocabWord(element.getLabel())
				t += 1
			Loop

			If cache.numWords() = 0 Then
				Throw New System.InvalidOperationException("Source VocabCache has no indexes available, transfer is impossible")
			End If

	'        
	'            Now, when we have transferred vocab, we should roll over iterator, and  gather labels, if any
	'         
			log.info("Vocab size before labels: " & cache.numWords())

			If fetchLabels Then
				For Each source As VocabSource(Of T) In sources
					Dim iterator As SequenceIterator(Of T) = source.getIterator()
					iterator.reset()

					Do While iterator.hasMoreSequences()
						Dim sequence As Sequence(Of T) = iterator.nextSequence()
						seqCount.incrementAndGet()

						If sequence.getSequenceLabels() IsNot Nothing Then
							For Each label As T In sequence.getSequenceLabels()
								If Not cache.containsWord(label.getLabel()) Then
									label.markAsLabel(True)
									label.setSpecial(True)

									label.setIndex(cache.numWords())

									cache.addToken(label)
									cache.addWordToIndex(label.getIndex(), label.getLabel())

									' backward compatibility code
									cache.putVocabWord(label.getLabel())

									'  log.info("Adding label ["+label.getLabel()+"]: " + cache.wordFor(label.getLabel()));
								End If ' else log.info("Label ["+label.getLabel()+"] already exists: " + cache.wordFor(label.getLabel()));
							Next label
						End If
					Loop
				Next source
			End If

			log.info("Vocab size after labels: " & cache.numWords())

			Return cache
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocabCache<T> transferVocabulary(@NonNull VocabCache<T> vocabCache, boolean buildHuffman)
		Public Overridable Function transferVocabulary(ByVal vocabCache As VocabCache(Of T), ByVal buildHuffman As Boolean) As VocabCache(Of T)
			Dim result As val = If(cache IsNot Nothing, cache, (New AbstractCache.Builder(Of T)()).build())

			For Each v As val In vocabCache.tokens()
					result.addToken(v)
					' optionally transferring indices
					If v.getIndex() >= 0 Then
						result.addWordToIndex(v.getIndex(), v.getLabel())
					Else
						result.addWordToIndex(result.numWords(), v.getLabel())
					End If
			Next v

			If buildHuffman Then
				Dim huffman As val = New Huffman(result.vocabWords())
				huffman.build()
				huffman.applyIndexes(result)
			End If

			Return result
		End Function

		Public Overridable Sub processDocument(ByVal targetVocab As AbstractCache(Of T), ByVal document As Sequence(Of T), ByVal finalCounter As AtomicLong, ByVal loopCounter As AtomicLong)
			Try
				Dim seqMap As IDictionary(Of String, AtomicLong) = New Dictionary(Of String, AtomicLong)()
				'  log.info("Sequence length: ["+ document.getElements().size()+"]");

				If fetchLabels AndAlso document.getSequenceLabels() IsNot Nothing Then
					For Each labelWord As T In document.getSequenceLabels()
						If Not targetVocab.hasToken(labelWord.getLabel()) Then
							labelWord.setSpecial(True)
							labelWord.markAsLabel(True)
							labelWord.setElementFrequency(1)

							targetVocab.addToken(labelWord)
						End If
					Next labelWord
				End If

				Dim tokens As IList(Of String) = document.asLabels()
				For Each token As String In tokens
					If stopWords IsNot Nothing AndAlso stopWords.Contains(token) Then
						Continue For
					End If
					If token Is Nothing OrElse token.Length = 0 Then
						Continue For
					End If

					If Not targetVocab.containsWord(token) Then
						Dim element As T = document.getElementByLabel(token)
						element.setElementFrequency(1)
						element.setSequencesCount(1)
						targetVocab.addToken(element)
						'                    elementsCounter.incrementAndGet();
						loopCounter.incrementAndGet()

						' if there's no such element in tempHolder, it's safe to set seqCount to 1
						seqMap(token) = New AtomicLong(0)
					Else
						targetVocab.incrementWordCount(token)

						' if element exists in tempHolder, we should update it seqCount, but only once per sequence
						If Not seqMap.ContainsKey(token) Then
							seqMap(token) = New AtomicLong(1)
							Dim element As T = targetVocab.wordFor(token)
							element.incrementSequencesCount()
						End If

						If index IsNot Nothing Then
							If document.SequenceLabel IsNot Nothing Then
								index.addWordsToDoc(index.numDocuments(), document.getElements(), document.SequenceLabel)
							Else
								index.addWordsToDoc(index.numDocuments(), document.getElements())
							End If
						End If
					End If
				Next token
			Catch e As Exception
				Throw New Exception(e)
			Finally
				finalCounter.incrementAndGet()
			End Try
		End Sub
		''' <summary>
		''' This method scans all sources passed through builder, and returns all words as vocab.
		''' If TargetVocabCache was set during instance creation, it'll be filled too.
		''' 
		''' 
		''' @return
		''' </summary>
		Public Overridable Function buildJointVocabulary(ByVal resetCounters As Boolean, ByVal buildHuffmanTree As Boolean) As VocabCache(Of T)
			Dim lastTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim lastSequences As Long = 0
			Dim lastElements As Long = 0
			Dim startTime As Long = lastTime
			Dim parsedCount As New AtomicLong(0)
			If resetCounters AndAlso buildHuffmanTree Then
				Throw New System.InvalidOperationException("You can't reset counters and build Huffman tree at the same time!")
			End If

			If cache Is Nothing Then
				cache = (New AbstractCache.Builder(Of T)()).build()
			End If

			log.debug("Target vocab size before building: [" & cache.numWords() & "]")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong loopCounter = new java.util.concurrent.atomic.AtomicLong(0);
			Dim loopCounter As New AtomicLong(0)

			Dim topHolder As AbstractCache(Of T) = (New AbstractCache.Builder(Of T)()).minElementFrequency(0).build()

			Dim cnt As Integer = 0
			Dim numProc As Integer = Runtime.getRuntime().availableProcessors()
			Dim numThreads As Integer = Math.Max(numProc \ 2, 2)
			Dim executorService As New PriorityScheduler(numThreads)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong execCounter = new java.util.concurrent.atomic.AtomicLong(0);
			Dim execCounter As New AtomicLong(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicLong finCounter = new java.util.concurrent.atomic.AtomicLong(0);
			Dim finCounter As New AtomicLong(0)

			For Each source As VocabSource(Of T) In sources
				Dim iterator As SequenceIterator(Of T) = source.getIterator()
				iterator.reset()

				log.debug("Trying source iterator: [" & cnt & "]")
				log.debug("Target vocab size before building: [" & cache.numWords() & "]")
				cnt += 1

				Dim tempHolder As AbstractCache(Of T) = (New AbstractCache.Builder(Of T)()).build()

				Dim sequences As Integer = 0
				Do While iterator.hasMoreSequences()
					Dim document As Sequence(Of T) = iterator.nextSequence()

					seqCount.incrementAndGet()
					parsedCount.addAndGet(document.size())
					tempHolder.incrementTotalDocCount()
					execCounter.incrementAndGet()

					If allowParallelBuilder Then
						executorService.execute(New VocabRunnable(Me, tempHolder, document, finCounter, loopCounter))
						' as we see in profiler, this lock isn't really happen too often
						' we don't want too much left in tail

						Do While execCounter.get() - finCounter.get() > numProc
							ThreadUtils.uncheckedSleep(1)
						Loop
					Else
						processDocument(tempHolder, document, finCounter, loopCounter)
					End If

					sequences += 1
					If seqCount.get() Mod 100000 = 0 Then
						Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
						Dim currentSequences As Long = seqCount.get()
						Dim currentElements As Long = parsedCount.get()

						Dim seconds As Double = (currentTime - lastTime) / CDbl(1000)

						'                    Collections.sort(timesHasNext);
						'                    Collections.sort(timesNext);

						Dim seqPerSec As Double = (currentSequences - lastSequences) / seconds
						Dim elPerSec As Double = (currentElements - lastElements) / seconds
						'                    log.info("Document time: {} us; hasNext time: {} us", timesNext.get(timesNext.size() / 2), timesHasNext.get(timesHasNext.size() / 2));
						log.info("Sequences checked: [{}]; Current vocabulary size: [{}]; Sequences/sec: {}; Words/sec: {};", seqCount.get(), tempHolder.numWords(), String.Format("{0:F2}", seqPerSec), String.Format("{0:F2}", elPerSec))
						lastTime = currentTime
						lastElements = currentElements
						lastSequences = currentSequences

						'                    timesHasNext.clear();
						'                    timesNext.clear();
					End If

					''' <summary>
					''' Firing scavenger loop
					''' </summary>
					If enableScavenger AndAlso loopCounter.get() >= 2000000 AndAlso tempHolder.numWords() > 10000000 Then
						log.info("Starting scavenger...")
						Do While execCounter.get() <> finCounter.get()
							ThreadUtils.uncheckedSleep(1)
						Loop

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						filterVocab(tempHolder, Math.Max(1, source.getMinWordFrequency() / 2))
						loopCounter.set(0)
					End If

					'                timesNext.add((time2 - time1) / 1000L);
					'                timesHasNext.add((time1 - time3) / 1000L);

					'                time3 = System.nanoTime();
				Loop

				' block untill all threads are finished
				log.debug("Waiting till all processes stop...")
				Do While execCounter.get() <> finCounter.get()
					ThreadUtils.uncheckedSleep(1)
				Loop


				' apply minWordFrequency set for this source
				log.debug("Vocab size before truncation: [" & tempHolder.numWords() & "],  NumWords: [" & tempHolder.totalWordOccurrences() & "], sequences parsed: [" & seqCount.get() & "], counter: [" & parsedCount.get() & "]")
				If source.getMinWordFrequency() > 0 Then
					filterVocab(tempHolder, source.getMinWordFrequency())
				End If

				log.debug("Vocab size after truncation: [" & tempHolder.numWords() & "],  NumWords: [" & tempHolder.totalWordOccurrences() & "], sequences parsed: [" & seqCount.get() & "], counter: [" & parsedCount.get() & "]")
				' at this moment we're ready to transfer
				topHolder.importVocabulary(tempHolder)
			Next source

			' at this moment, we have vocabulary full of words, and we have to reset counters before transfer everything back to VocabCache

			'topHolder.resetWordCounters();


			System.GC.Collect()
			cache.importVocabulary(topHolder)

			' adding UNK word
			If unk IsNot Nothing Then
				log.info("Adding UNK element to vocab...")
				unk.setSpecial(True)
				cache.addToken(unk)
			End If

			If resetCounters Then
				For Each element As T In cache.vocabWords()
					element.setElementFrequency(0)
				Next element
				cache.updateWordsOccurrences()
			End If

			If buildHuffmanTree Then

				If limit > 0 Then
					' we want to sort labels before truncating them, so we'll keep most important words
					Dim words As val = New List(Of T)(cache.vocabWords())
					Collections.sort(words)

					' now rolling through them
					For Each element As val In words
						If element.getIndex() > limit AndAlso Not element.isSpecial() AndAlso Not element.isLabel() Then
							cache.removeElement(element.getLabel())
						End If
					Next element
				End If
				' and now we're building Huffman tree
				Dim huffman As val = New Huffman(cache.vocabWords())
				huffman.build()
				huffman.applyIndexes(cache)
			End If

			executorService.shutdown()

			System.GC.Collect()

			Dim endSequences As Long = seqCount.get()
			Dim endTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim seconds As Double = (endTime - startTime) / CDbl(1000)
			Dim seqPerSec As Double = endSequences / seconds
			log.info("Sequences checked: [{}], Current vocabulary size: [{}]; Sequences/sec: [{}];", seqCount.get(), cache.numWords(), String.Format("{0:F2}", seqPerSec))
			Return cache
		End Function

		Protected Friend Overridable Sub filterVocab(ByVal cache As AbstractCache(Of T), ByVal minWordFrequency As Integer)
			Dim numWords As Integer = cache.numWords()
			Dim labelsToRemove As New LinkedBlockingQueue(Of String)()
			For Each element As T In cache.vocabWords()
				If element.getElementFrequency() < minWordFrequency AndAlso Not element.isSpecial() AndAlso Not element.isLabel() Then
					labelsToRemove.add(element.getLabel())
				End If
			Next element

			For Each label As String In labelsToRemove
				cache.removeElement(label)
			Next label

			log.debug("Scavenger: Words before: {}; Words after: {};", numWords, cache.numWords())
		End Sub

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
			Friend sources As IList(Of VocabSource(Of T)) = New List(Of VocabSource(Of T))()
			Friend cache As VocabCache(Of T)
'JAVA TO VB CONVERTER NOTE: The field stopWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend stopWords_Conflict As ICollection(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend useAdaGrad_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field fetchLabels was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend fetchLabels_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field index was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend index_Conflict As InvertedIndex(Of T)
			Friend limit As Integer
'JAVA TO VB CONVERTER NOTE: The field enableScavenger was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend enableScavenger_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field unk was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend unk_Conflict As T
			Friend allowParallelBuilder As Boolean = True
			Friend lockf As Boolean = False

			Public Sub New()

			End Sub

			''' <summary>
			''' This method sets the limit to resulting vocabulary size.
			''' 
			''' PLEASE NOTE:  This method is applicable only if huffman tree is built.
			''' </summary>
			''' <param name="limit">
			''' @return </param>
			Public Overridable Function setEntriesLimit(ByVal limit As Integer) As Builder(Of T)
				Me.limit = limit
				Return Me
			End Function


			Public Overridable Function allowParallelTokenization(ByVal reallyAllow As Boolean) As Builder(Of T)
				Me.allowParallelBuilder = reallyAllow
				Return Me
			End Function

			''' <summary>
			''' Defines, if adaptive gradients should be created during vocabulary mastering
			''' </summary>
			''' <param name="useAdaGrad">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter useAdaGrad was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Protected Friend Overridable Function useAdaGrad(ByVal useAdaGrad_Conflict As Boolean) As Builder(Of T)
				Me.useAdaGrad_Conflict = useAdaGrad_Conflict
				Return Me
			End Function

			''' <summary>
			''' After temporary internal vocabulary is built, it will be transferred to target VocabCache you pass here
			''' </summary>
			''' <param name="cache"> target VocabCache
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setTargetVocabCache(@NonNull VocabCache<T> cache)
			Public Overridable Function setTargetVocabCache(ByVal cache As VocabCache(Of T)) As Builder(Of T)
				Me.cache = cache
				Return Me
			End Function

			''' <summary>
			''' Adds SequenceIterator for vocabulary construction.
			''' Please note, you can add as many sources, as you wish.
			''' </summary>
			''' <param name="iterator"> SequenceIterator to build vocabulary from </param>
			''' <param name="minElementFrequency"> elements with frequency below this value will be removed from vocabulary
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> addSource(@NonNull SequenceIterator<T> iterator, int minElementFrequency)
			Public Overridable Function addSource(ByVal iterator As SequenceIterator(Of T), ByVal minElementFrequency As Integer) As Builder(Of T)
				sources.Add(New VocabSource(Of T)(iterator, minElementFrequency))
				Return Me
			End Function

	'        
	'        public Builder<T> addSource(LabelAwareIterator iterator, int minWordFrequency) {
	'            sources.add(new VocabSource(iterator, minWordFrequency));
	'            return this;
	'        }
	'        
	'        public Builder<T> addSource(SentenceIterator iterator, int minWordFrequency) {
	'            sources.add(new VocabSource(new SentenceIteratorConverter(iterator), minWordFrequency));
	'            return this;
	'        }
	'        
	'        
	'        public Builder setTokenizerFactory(@NonNull TokenizerFactory factory) {
	'            this.tokenizerFactory = factory;
	'            return this;
	'        }
	'        
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> setStopWords(@NonNull Collection<String> stopWords)
			Public Overridable Function setStopWords(ByVal stopWords As ICollection(Of String)) As Builder(Of T)
				Me.stopWords_Conflict = stopWords
				Return Me
			End Function

			''' <summary>
			''' Sets, if labels should be fetched, during vocab building
			''' </summary>
			''' <param name="reallyFetch">
			''' @return </param>
			Public Overridable Function fetchLabels(ByVal reallyFetch As Boolean) As Builder(Of T)
				Me.fetchLabels_Conflict = reallyFetch
				Return Me
			End Function

			Public Overridable Function setIndex(ByVal index As InvertedIndex(Of T)) As Builder(Of T)
				Me.index_Conflict = index
				Return Me
			End Function

			Public Overridable Function enableScavenger(ByVal reallyEnable As Boolean) As Builder(Of T)
				Me.enableScavenger_Conflict = reallyEnable
				Return Me
			End Function

			Public Overridable Function setUnk(ByVal unk As T) As Builder(Of T)
				Me.unk_Conflict = unk
				Return Me
			End Function

			Public Overridable Function build() As VocabConstructor(Of T)
				Dim constructor As New VocabConstructor(Of T)()
				constructor.sources = Me.sources
				constructor.cache = Me.cache
				constructor.stopWords = Me.stopWords_Conflict
				constructor.useAdaGrad = Me.useAdaGrad_Conflict
				constructor.fetchLabels = Me.fetchLabels_Conflict
				constructor.limit = Me.limit
				constructor.index = Me.index_Conflict
				constructor.enableScavenger = Me.enableScavenger_Conflict
				constructor.unk = Me.unk_Conflict
				constructor.allowParallelBuilder = Me.allowParallelBuilder
				constructor.lockf = Me.lockf

				Return constructor
			End Function

			Public Overridable Function setLockFactor(ByVal lockf As Boolean) As Builder(Of T)
				Me.lockf = lockf
				Return Me
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data private static class VocabSource<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement>
		Private Class VocabSource(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private org.deeplearning4j.models.sequencevectors.interfaces.SequenceIterator<T> iterator;
			Friend iterator As SequenceIterator(Of T)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private int minWordFrequency;
			Friend minWordFrequency As Integer
		End Class


		Protected Friend Class VocabRunnable
			Implements ThreadStart

			Private ReadOnly outerInstance As VocabConstructor(Of T)

			Friend ReadOnly finalCounter As AtomicLong
			Friend ReadOnly document As Sequence(Of T)
			Friend ReadOnly targetVocab As AbstractCache(Of T)
			Friend ReadOnly loopCounter As AtomicLong
			Friend done As New AtomicBoolean(False)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VocabRunnable(@NonNull AbstractCache<T> targetVocab, @NonNull Sequence<T> sequence, @NonNull AtomicLong finalCounter, @NonNull AtomicLong loopCounter)
			Public Sub New(ByVal outerInstance As VocabConstructor(Of T), ByVal targetVocab As AbstractCache(Of T), ByVal sequence As Sequence(Of T), ByVal finalCounter As AtomicLong, ByVal loopCounter As AtomicLong)
				Me.outerInstance = outerInstance
				Me.finalCounter = finalCounter
				Me.document = sequence
				Me.targetVocab = targetVocab
				Me.loopCounter = loopCounter
			End Sub

			Public Overrides Sub run()
				 Try
					 outerInstance.processDocument(targetVocab, document, finalCounter, loopCounter)
				 Catch e As Exception
					 Throw New Exception(e)
				 Finally
					done.set(True)
				 End Try
			End Sub
		End Class
	End Class

End Namespace