Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports InMemoryLookupCache = org.deeplearning4j.models.word2vec.wordstore.inmemory.InMemoryLookupCache
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.models.word2vec.wordstore


	<Serializable>
	Public Class VocabularyHolder
		Private ReadOnly vocabulary As IDictionary(Of String, VocabularyWord) = New ConcurrentDictionary(Of String, VocabularyWord)()

		' idxMap marked as transient, since there's no real reason to save this data on serialization
		<NonSerialized>
		Private idxMap As IDictionary(Of Integer, VocabularyWord) = New ConcurrentDictionary(Of Integer, VocabularyWord)()
		Private minWordFrequency As Integer = 0
		Private hugeModelExpected As Boolean = False
		Private retentionDelay As Integer = 3

		Private vocabCache As VocabCache

		' this variable defines how often scavenger will be activated
		Private scavengerThreshold As Integer = 2000000

		Private totalWordOccurrences As Long = 0

		' for scavenger mechanics we need to know the actual number of words being added
		<NonSerialized>
		Private hiddenWordsCounter As New AtomicLong(0)

		Private totalWordCount As New AtomicInteger(0)

		Private logger As Logger = LoggerFactory.getLogger(GetType(VocabularyHolder))

		Private Const MAX_CODE_LENGTH As Integer = 40

		''' <summary>
		''' Default constructor
		''' </summary>
		Protected Friend Sub New()

		End Sub

		''' <summary>
		''' Builds VocabularyHolder from VocabCache.
		''' 
		''' Basically we just ignore tokens, and transfer VocabularyWords, supposing that it's already truncated by minWordFrequency.
		''' 
		''' Huffman tree data is ignored and recalculated, due to suspectable flaw in dl4j huffman impl, and it's excessive memory usage.
		''' 
		''' This code is required for compatibility between dl4j w2v implementation, and standalone w2v </summary>
		''' <param name="cache"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected VocabularyHolder(@NonNull VocabCache<? extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> cache, boolean markAsSpecial)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Protected Friend Sub New(ByVal cache As VocabCache(Of SequenceElement), ByVal markAsSpecial As Boolean)
			Me.vocabCache = cache
			For Each word As SequenceElement In cache.tokens()
				Dim vw As New VocabularyWord(word.Label)
				vw.setCount(CInt(Math.Truncate(word.getElementFrequency())))

				' since we're importing this word from external VocabCache, we'll assume that this word is SPECIAL, and should NOT be affected by minWordFrequency
				vw.setSpecial(markAsSpecial)

				' please note: we don't transfer huffman data, since proper way is  to recalculate it after new words being added
				If word.getPoints() IsNot Nothing AndAlso word.getPoints().Count > 0 Then
					vw.setHuffmanNode(buildNode(word.getCodes(), word.getPoints(), word.getCodeLength(), word.Index))
				End If

				vocabulary(vw.getWord()) = vw
			Next word

			' there's no sense building huffman tree just for UNK word
			If numWords() > 1 Then
				updateHuffmanCodes()
			End If
			logger.info("Init from VocabCache is complete. " & numWords() & " word(s) were transferred.")
		End Sub

		Public Shared Function buildNode(ByVal codes As IList(Of SByte), ByVal points As IList(Of Integer), ByVal codeLen As Integer, ByVal index As Integer) As HuffmanNode
			Return New HuffmanNode(listToArray(codes), listToArray(points, MAX_CODE_LENGTH), index, CSByte(codeLen))
		End Function


		Public Overridable Sub transferBackToVocabCache()
			transferBackToVocabCache(Me.vocabCache, True)
		End Sub

		Public Overridable Sub transferBackToVocabCache(ByVal cache As VocabCache)
			transferBackToVocabCache(cache, True)
		End Sub

		''' <summary>
		''' This method is required for compatibility purposes.
		'''  It just transfers vocabulary from VocabHolder into VocabCache
		''' </summary>
		''' <param name="cache"> </param>
		Public Overridable Sub transferBackToVocabCache(ByVal cache As VocabCache, ByVal emptyHolder As Boolean)
			If Not (TypeOf cache Is InMemoryLookupCache) Then
				Throw New System.InvalidOperationException("Sorry, only InMemoryLookupCache use implemented.")
			End If

			' make sure that huffman codes are updated before transfer
			Dim words As IList(Of VocabularyWord) = Me.words() 'updateHuffmanCodes();

			For Each word As VocabularyWord In words
				If word.getWord().isEmpty() Then
					Continue For
				End If
				Dim vocabWord As New VocabWord(1, word.getWord())

				' if we're transferring full model, it CAN contain HistoricalGradient for AdaptiveGradient feature
				If word.getHistoricalGradient() IsNot Nothing Then
					Dim gradient As INDArray = Nd4j.create(word.getHistoricalGradient())
					vocabWord.HistoricalGradient = gradient
				End If

				' put VocabWord into both Tokens and Vocabs maps
				CType(cache, InMemoryLookupCache).getVocabs()(word.getWord()) = vocabWord
				CType(cache, InMemoryLookupCache).getTokens()(word.getWord()) = vocabWord


				' update Huffman tree information
				If word.getHuffmanNode() IsNot Nothing Then
					vocabWord.Index = word.getHuffmanNode().getIdx()
					vocabWord.setCodeLength(word.getHuffmanNode().getLength())
					vocabWord.setPoints(arrayToList(word.getHuffmanNode().getPoint(), word.getHuffmanNode().getLength()))
					vocabWord.Codes = arrayToList(word.getHuffmanNode().getCode(), word.getHuffmanNode().getLength())

					' put word into index
					cache.addWordToIndex(word.getHuffmanNode().getIdx(), word.getWord())
				End If

				'update vocabWord counter. substract 1, since its the base value for any token
				' >1 hack is required since VocabCache impl imples 1 as base word count, not 0
				If word.getCount() > 1 Then
					cache.incrementWordCount(word.getWord(), word.getCount() - 1)
				End If
			Next word

			' at this moment its pretty safe to nullify all vocabs.
			If emptyHolder Then
				idxMap.Clear()
				vocabulary.Clear()
			End If
		End Sub

		''' <summary>
		''' This method is needed ONLY for unit tests and should NOT be available in public scope.
		''' 
		''' It sets the vocab size ratio, at wich dynamic scavenger will be activated </summary>
		''' <param name="threshold"> </param>
		Protected Friend Overridable WriteOnly Property ScavengerActivationThreshold As Integer
			Set(ByVal threshold As Integer)
				Me.scavengerThreshold = threshold
			End Set
		End Property


		''' <summary>
		'''  This method is used only for VocabCache compatibility purposes </summary>
		''' <param name="array"> </param>
		''' <param name="codeLen">
		''' @return </param>
		Public Shared Function arrayToList(ByVal array() As SByte, ByVal codeLen As Integer) As IList(Of SByte)
			Dim result As IList(Of SByte) = New List(Of SByte)()
			For x As Integer = 0 To codeLen - 1
				result.Add(array(x))
			Next x
			Return result
		End Function

		Public Shared Function listToArray(ByVal code As IList(Of SByte)) As SByte()
			Dim array(MAX_CODE_LENGTH - 1) As SByte
			For x As Integer = 0 To code.Count - 1
				array(x) = code(x)
			Next x
			Return array
		End Function

		Public Shared Function listToArray(ByVal points As IList(Of Integer), ByVal codeLen As Integer) As Integer()
			Dim array(points.Count - 1) As Integer
			For x As Integer = 0 To points.Count - 1
				array(x) = points(x).intValue()
			Next x
			Return array
		End Function

		''' <summary>
		'''  This method is used only for VocabCache compatibility purposes </summary>
		''' <param name="array"> </param>
		''' <param name="codeLen">
		''' @return </param>
		Public Shared Function arrayToList(ByVal array() As Integer, ByVal codeLen As Integer) As IList(Of Integer)
			Dim result As IList(Of Integer) = New List(Of Integer)()
			For x As Integer = 0 To codeLen - 1
				result.Add(array(x))
			Next x
			Return result
		End Function

		Public Overridable ReadOnly Property Vocabulary As ICollection(Of VocabularyWord)
			Get
				Return vocabulary.Values
			End Get
		End Property

		Public Overridable Function getVocabularyWordByString(ByVal word As String) As VocabularyWord
			Return vocabulary(word)
		End Function

		Public Overridable Function getVocabularyWordByIdx(ByVal id As Integer?) As VocabularyWord
			Return idxMap(id)
		End Function

		''' <summary>
		''' Checks vocabulary for the word existence
		''' </summary>
		''' <param name="word"> to be looked for </param>
		''' <returns> TRUE of contains, FALSE otherwise </returns>
		Public Overridable Function containsWord(ByVal word As String) As Boolean
			Return vocabulary.ContainsKey(word)
		End Function

		''' <summary>
		''' Increments by one number of occurrences of the word in corpus
		''' </summary>
		''' <param name="word"> whose counter is to be incremented </param>
		Public Overridable Sub incrementWordCounter(ByVal word As String)
			If vocabulary.ContainsKey(word) Then
				vocabulary(word).incrementCount()
			End If
			' there's no need to throw such exception here. just do nothing if word is not found
			'else throw new IllegalStateException("No such word found");
		End Sub

		''' <summary>
		''' Adds new word to vocabulary
		''' </summary>
		''' <param name="word"> to be added </param>
		' TODO: investigate, if it's worth to make this internally synchronized and virtually thread-safe
		Public Overridable Sub addWord(ByVal word As String)
			If Not vocabulary.ContainsKey(word) Then
				Dim vw As New VocabularyWord(word)

	'            
	'                TODO: this should be done in different way, since this implementation causes minWordFrequency ultimate ignoral if markAsSpecial set to TRUE
	'            
	'                Probably the best way to solve it, is remove markAsSpecial option here, and let this issue be regulated with minWordFrequency
	'              
				' vw.setSpecial(markAsSpecial);

				' initialize frequencyShift only if hugeModelExpected. It's useless otherwise :)
				If hugeModelExpected Then
					vw.setFrequencyShift(New SByte(retentionDelay - 1){})
				End If

				vocabulary(word) = vw



				If hugeModelExpected AndAlso minWordFrequency > 1 AndAlso hiddenWordsCounter.incrementAndGet() Mod scavengerThreshold = 0 Then
					activateScavenger()
				End If

				Return
			End If
		End Sub

		Public Overridable Sub addWord(ByVal word As VocabularyWord)
			vocabulary(word.getWord()) = word
		End Sub

		Public Overridable Sub consumeVocabulary(ByVal holder As VocabularyHolder)
			For Each word As VocabularyWord In holder.getVocabulary()
				If Not Me.containsWord(word.getWord()) Then
					Me.addWord(word)
				Else
					holder.incrementWordCounter(word.getWord())
				End If
			Next word
		End Sub

		''' <summary>
		''' This method removes low-frequency words based on their frequency change between activations.
		''' I.e. if word has appeared only once, and it's retained the same frequency over consequence activations, we can assume it can be removed freely
		''' </summary>
		Protected Friend Overridable Sub activateScavenger()
			SyncLock Me
				Dim initialSize As Integer = vocabulary.Count
				Dim words As IList(Of VocabularyWord) = New List(Of VocabularyWord)(vocabulary.Values)
				For Each word As VocabularyWord In words
					' scavenging could be applied only to non-special tokens that are below minWordFrequency
					If word.isSpecial() OrElse word.getCount() >= minWordFrequency OrElse word.getFrequencyShift() Is Nothing Then
						word.setFrequencyShift(Nothing)
						Continue For
					End If
        
					' save current word counter to byte array at specified position
					word.getFrequencyShift()(word.getRetentionStep()) = CSByte(Math.Truncate(word.getCount()))
        
		'            
		'                    we suppose that we're hunting only low-freq words that already passed few activations
		'                    so, we assume word personal threshold as 20% of minWordFrequency, but not less then 1.
		'            
		'                    so, if after few scavenging cycles wordCount is still <= activation - just remove word.
		'                    otherwise nullify word.frequencyShift to avoid further checks
		'              
					Dim activation As Integer = Math.Max(minWordFrequency \ 5, 2)
					logger.debug("Current state> Activation: [" & activation & "], retention info: " & java.util.Arrays.toString(word.getFrequencyShift()))
					If word.getCount() <= activation AndAlso word.getFrequencyShift()(Me.retentionDelay - 1) > 0 Then
        
						' if final word count at latest retention point is the same as at the beginning - just remove word
						If word.getFrequencyShift()(Me.retentionDelay - 1) <= activation AndAlso word.getFrequencyShift()(Me.retentionDelay - 1) = word.getFrequencyShift()(0) Then
							vocabulary.Remove(word.getWord())
						End If
					End If
        
					' shift retention history to the left
					If word.getRetentionStep() < retentionDelay - 1 Then
						word.incrementRetentionStep()
					Else
						For x As Integer = 1 To retentionDelay - 1
							word.getFrequencyShift()(x - 1) = word.getFrequencyShift()(x)
						Next x
					End If
				Next word
				logger.info("Scavenger was activated. Vocab size before: [" & initialSize & "],  after: [" & vocabulary.Count & "]")
			End SyncLock
		End Sub

		''' <summary>
		''' This methods reset counters for all words in vocabulary
		''' </summary>
		Public Overridable Sub resetWordCounters()
			For Each word As VocabularyWord In getVocabulary()
				word.setHuffmanNode(Nothing)
				word.setFrequencyShift(Nothing)
				word.setCount(0)
			Next word
		End Sub

		''' 
		''' <returns> number of words in vocabulary </returns>
		Public Overridable Function numWords() As Integer
			Return vocabulary.Count
		End Function

		''' <summary>
		''' The same as truncateVocabulary(this.minWordFrequency)
		''' </summary>
		Public Overridable Sub truncateVocabulary()
			truncateVocabulary(minWordFrequency)
		End Sub

		''' <summary>
		''' All words with frequency below threshold wii be removed
		''' </summary>
		''' <param name="threshold"> exclusive threshold for removal </param>
		Public Overridable Sub truncateVocabulary(ByVal threshold As Integer)
			logger.debug("Truncating vocabulary to minWordFrequency: [" & threshold & "]")
			Dim keyset As ISet(Of String) = vocabulary.Keys
			For Each word As String In keyset
				Dim vw As VocabularyWord = vocabulary(word)

				' please note: we're not applying threshold to SPECIAL words
				If Not vw.isSpecial() AndAlso vw.getCount() < threshold Then
					vocabulary.Remove(word)
					If vw.getHuffmanNode() IsNot Nothing Then
						idxMap.Remove(vw.getHuffmanNode().getIdx())
					End If
				End If
			Next word
		End Sub

		''' <summary>
		''' build binary tree ordered by counter.
		''' 
		''' Based on original w2v by google
		''' </summary>
		Public Overridable Function updateHuffmanCodes() As IList(Of VocabularyWord)
			Dim min1i As Integer
			Dim min2i As Integer
			Dim b As Integer
			Dim i As Integer
			' get vocabulary as sorted list
			Dim vocab As IList(Of VocabularyWord) = Me.words()
			Dim count(vocab.Count * 2) As Integer
			Dim parent_node(vocab.Count * 2) As Integer
			Dim binary(vocab.Count * 2) As SByte

			' at this point vocab is sorted, with descending order
			For a As Integer = 0 To vocab.Count - 1
				count(a) = vocab(a).getCount()
			Next a
			For a As Integer = vocab.Count To (vocab.Count * 2) - 1
				count(a) = Integer.MaxValue
			Next a
			Dim pos1 As Integer = vocab.Count - 1
			Dim pos2 As Integer = vocab.Count
			For a As Integer = 0 To vocab.Count - 1
				' First, find two smallest nodes 'min1, min2'
				If pos1 >= 0 Then
					If count(pos1) < count(pos2) Then
						min1i = pos1
						pos1 -= 1
					Else
						min1i = pos2
						pos2 += 1
					End If
				Else
					min1i = pos2
					pos2 += 1
				End If
				If pos1 >= 0 Then
					If count(pos1) < count(pos2) Then
						min2i = pos1
						pos1 -= 1
					Else
						min2i = pos2
						pos2 += 1
					End If
				Else
					min2i = pos2
					pos2 += 1
				End If
				count(vocab.Count + a) = count(min1i) + count(min2i)
				parent_node(min1i) = vocab.Count + a
				parent_node(min2i) = vocab.Count + a
				binary(min2i) = 1
			Next a

			' Now assign binary code to each vocabulary word
			Dim code(MAX_CODE_LENGTH - 1) As SByte
			Dim point(MAX_CODE_LENGTH - 1) As Integer

			Dim a As Integer = 0
			Do While a < vocab.Count
				b = a
				i = 0
				Dim lcode(MAX_CODE_LENGTH - 1) As SByte
				Dim lpoint(MAX_CODE_LENGTH - 1) As Integer
				Do
					code(i) = binary(b)
					point(i) = b
					i += 1
					b = parent_node(b)
					If b = vocab.Count * 2 - 2 Then
						Exit Do
					End If
				Loop

				lpoint(0) = vocab.Count - 2
				For b = 0 To i - 1
					lcode(i - b - 1) = code(b)
					lpoint(i - b) = point(b) - vocab.Count
				Next b

				vocab(a).setHuffmanNode(New HuffmanNode(lcode, lpoint, a, CSByte(i)))
				a += 1
			Loop

			idxMap.Clear()
			For Each word As VocabularyWord In vocab
				idxMap(word.getHuffmanNode().getIdx()) = word
			Next word

			Return vocab
		End Function

		''' <summary>
		''' This method returns index of word in sorted list.
		''' </summary>
		''' <param name="word">
		''' @return </param>
		Public Overridable Function indexOf(ByVal word As String) As Integer
			If vocabulary.ContainsKey(word) Then
				Return vocabulary(word).getHuffmanNode().getIdx()
			Else
				Return -1
			End If
		End Function


		''' <summary>
		''' Returns sorted list of words in vocabulary.
		''' Sort is DESCENDING.
		''' </summary>
		''' <returns> list of VocabularyWord </returns>
		Public Overridable Function words() As IList(Of VocabularyWord)
			Dim vocab As IList(Of VocabularyWord) = New List(Of VocabularyWord)(vocabulary.Values)
			vocab.Sort(New ComparatorAnonymousInnerClass(Me))

			Return vocab
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of VocabularyWord)

			Private ReadOnly outerInstance As VocabularyHolder

			Public Sub New(ByVal outerInstance As VocabularyHolder)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As VocabularyWord, ByVal o2 As VocabularyWord) As Integer Implements IComparer(Of VocabularyWord).Compare
				Return Integer.compare(o2.getCount(), o1.getCount())
			End Function
		End Class

		Public Overridable Function totalWordsBeyondLimit() As Long
			If totalWordOccurrences = 0 Then
				For Each word As VocabularyWord In vocabulary.Values
					totalWordOccurrences += word.getCount()
				Next word
				Return totalWordOccurrences
			Else
				Return totalWordOccurrences
			End If
		End Function

		Public Class Builder
			Friend cache As VocabCache = Nothing
'JAVA TO VB CONVERTER NOTE: The field minWordFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minWordFrequency_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field hugeModelExpected was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hugeModelExpected_Conflict As Boolean = False
			Friend scavengerThreshold As Integer = 2000000
			Friend retentionDelay As Integer = 3

			Public Sub New()

			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder externalCache(@NonNull VocabCache cache)
			Public Overridable Function externalCache(ByVal cache As VocabCache) As Builder
				Me.cache = cache
				Return Me
			End Function

			Public Overridable Function minWordFrequency(ByVal threshold As Integer) As Builder
				Me.minWordFrequency_Conflict = threshold
				Return Me
			End Function

			''' <summary>
			''' With this argument set to true, you'll have your vocab scanned for low-freq words periodically.
			''' 
			''' Please note: this is incompatible with SPECIAL mechanics.
			''' </summary>
			''' <param name="reallyExpected">
			''' @return </param>
			Public Overridable Function hugeModelExpected(ByVal reallyExpected As Boolean) As Builder
				Me.hugeModelExpected_Conflict = reallyExpected
				Return Me
			End Function

			''' <summary>
			'''  Activation threshold defines, how ofter scavenger will be executed, to throw away low-frequency keywords.
			'''  Good values to start mostly depends on your workstation. Something like 1000000 looks pretty nice to start with.
			'''  Too low values can lead to undesired removal of words from vocab.
			''' 
			'''  Please note: this is incompatible with SPECIAL mechanics.
			''' </summary>
			''' <param name="threshold">
			''' @return </param>
			Public Overridable Function scavengerActivationThreshold(ByVal threshold As Integer) As Builder
				Me.scavengerThreshold = threshold
				Return Me
			End Function

			''' <summary>
			''' Retention delay defines, how long low-freq word will be kept in vocab, during building.
			''' Good values to start with: 3,4,5. Not too high, and not too low.
			''' 
			''' Please note: this is incompatible with SPECIAL mechanics.
			''' </summary>
			''' <param name="delay">
			''' @return </param>
			Public Overridable Function scavengerRetentionDelay(ByVal delay As Integer) As Builder
				If delay < 2 Then
					Throw New System.InvalidOperationException("Delay < 2 doesn't really makes sense")
				End If
				Me.retentionDelay = delay
				Return Me
			End Function

			Public Overridable Function build() As VocabularyHolder
				Dim holder As VocabularyHolder = Nothing
				If cache IsNot Nothing Then
					holder = New VocabularyHolder(cache, True)
				Else
					holder = New VocabularyHolder()
				End If
				holder.minWordFrequency = Me.minWordFrequency_Conflict
				holder.hugeModelExpected = Me.hugeModelExpected_Conflict
				holder.scavengerThreshold = Me.scavengerThreshold
				holder.retentionDelay = Me.retentionDelay

				Return holder
			End Function
		End Class
	End Class

End Namespace