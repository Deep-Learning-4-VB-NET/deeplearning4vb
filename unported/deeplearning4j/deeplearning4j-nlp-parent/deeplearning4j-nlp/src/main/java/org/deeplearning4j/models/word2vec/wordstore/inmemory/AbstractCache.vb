Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports JsonArray = com.google.gson.JsonArray
Imports JsonObject = com.google.gson.JsonObject
Imports JsonParser = com.google.gson.JsonParser
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports org.nd4j.shade.jackson.databind
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

Namespace org.deeplearning4j.models.word2vec.wordstore.inmemory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY, getterVisibility = JsonAutoDetect.Visibility.NONE, setterVisibility = JsonAutoDetect.Visibility.NONE) public class AbstractCache<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.deeplearning4j.models.word2vec.wordstore.VocabCache<T>
	Public Class AbstractCache(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements VocabCache(Of T)

		Private Const CLASS_FIELD As String = "@class"
		Private Const VOCAB_LIST_FIELD As String = "VocabList"
		Private Const VOCAB_ITEM_FIELD As String = "VocabItem"
		Private Const DOC_CNT_FIELD As String = "DocumentsCounter"
		Private Const MINW_FREQ_FIELD As String = "MinWordsFrequency"
		Private Const HUGE_MODEL_FIELD As String = "HugeModelExpected"
		Private Const STOP_WORDS_FIELD As String = "StopWords"
		Private Const SCAVENGER_FIELD As String = "ScavengerThreshold"
		Private Const RETENTION_FIELD As String = "RetentionDelay"
		Private Const TOTAL_WORD_FIELD As String = "TotalWordCount"

		Private ReadOnly vocabulary As ConcurrentMap(Of Long, T) = New ConcurrentDictionary(Of Long, T)()

		Private ReadOnly extendedVocabulary As IDictionary(Of String, T) = New ConcurrentDictionary(Of String, T)()

		Private ReadOnly idxMap As IDictionary(Of Integer, T) = New ConcurrentDictionary(Of Integer, T)()

		Private ReadOnly documentsCounter As New AtomicLong(0)

		Private minWordFrequency As Integer = 0
		Private hugeModelExpected As Boolean = False

		' we're using <String>for compatibility & failproof reasons: it's easier to store unique labels then abstract objects of unknown size
		' TODO: wtf this one is doing here?
		Private stopWords As IList(Of String) = New List(Of String)() ' stop words

		' this variable defines how often scavenger will be activated
		Private scavengerThreshold As Integer = 3000000 ' ser
		Private retentionDelay As Integer = 3 ' ser

		' for scavenger mechanics we need to know the actual number of words being added
		<NonSerialized>
		Private hiddenWordsCounter As New AtomicLong(0)

		Private ReadOnly totalWordCount As New AtomicLong(0) ' ser

		Private Const MAX_CODE_LENGTH As Integer = 40

		''' <summary>
		''' Deserialize vocabulary from specified path
		''' </summary>
		Public Overridable Sub loadVocab() Implements VocabCache(Of T).loadVocab
			' TODO: this method should be static and accept path
		End Sub

		''' <summary>
		''' Returns true, if number of elements in vocabulary > 0, false otherwise
		''' 
		''' @return
		''' </summary>
		Public Overridable Function vocabExists() As Boolean Implements VocabCache(Of T).vocabExists
			Return Not vocabulary.isEmpty()
		End Function

		''' <summary>
		''' Serialize vocabulary to specified path
		''' 
		''' </summary>
		Public Overridable Sub saveVocab() Implements VocabCache(Of T).saveVocab
			' TODO: this method should be static and accept path
		End Sub

		''' <summary>
		''' Returns collection of labels available in this vocabulary
		''' 
		''' @return
		''' </summary>
		Public Overridable Function words() As ICollection(Of String)
			Return Collections.unmodifiableCollection(extendedVocabulary.Keys)
		End Function

		''' <summary>
		''' Increment frequency for specified label by 1
		''' </summary>
		''' <param name="word"> the word to increment the count for </param>
		Public Overridable Sub incrementWordCount(ByVal word As String) Implements VocabCache(Of T).incrementWordCount
			incrementWordCount(word, 1)
		End Sub


		''' <summary>
		''' Increment frequency for specified label by specified value
		''' </summary>
		''' <param name="word"> the word to increment the count for </param>
		''' <param name="increment"> the amount to increment by </param>
		Public Overridable Sub incrementWordCount(ByVal word As String, ByVal increment As Integer) Implements VocabCache(Of T).incrementWordCount
			Dim element As T = extendedVocabulary(word)
			If element IsNot Nothing Then
				element.increaseElementFrequency(increment)
				totalWordCount.addAndGet(increment)
			End If
		End Sub

		''' <summary>
		''' Returns the SequenceElement's frequency over training corpus
		''' </summary>
		''' <param name="word"> the word to retrieve the occurrence frequency for
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public int wordFrequency(@NonNull String word)
		Public Overridable Function wordFrequency(ByVal word As String) As Integer Implements VocabCache(Of T).wordFrequency
			' TODO: proper wordFrequency impl should return long, instead of int
			Dim element As T = extendedVocabulary(word)
			If element IsNot Nothing Then
				Return CInt(Math.Truncate(element.getElementFrequency()))
			End If
			Return 0
		End Function

		''' <summary>
		''' Checks, if specified label exists in vocabulary
		''' </summary>
		''' <param name="word"> the word to check for
		''' @return </param>
		Public Overridable Function containsWord(ByVal word As String) As Boolean Implements VocabCache(Of T).containsWord
			Return extendedVocabulary.ContainsKey(word)
		End Function

		''' <summary>
		''' Checks, if specified element exists in vocabulary
		''' </summary>
		''' <param name="element">
		''' @return </param>
		Public Overridable Function containsElement(ByVal element As T) As Boolean
			' FIXME: lolwtf
			Return vocabulary.values().contains(element)
		End Function

		''' <summary>
		''' Returns the label of the element at specified Huffman index
		''' </summary>
		''' <param name="index"> the index of the word to get
		''' @return </param>
		Public Overridable Function wordAtIndex(ByVal index As Integer) As String Implements VocabCache(Of T).wordAtIndex
			Dim element As T = idxMap(index)
			If element IsNot Nothing Then
				Return element.getLabel()
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns SequenceElement at specified index
		''' </summary>
		''' <param name="index">
		''' @return </param>
		Public Overridable Function elementAtIndex(ByVal index As Integer) As T Implements VocabCache(Of T).elementAtIndex
			Return idxMap(index)
		End Function

		''' <summary>
		''' Returns Huffman index for specified label
		''' </summary>
		''' <param name="label"> the label to get index for </param>
		''' <returns> >=0 if label exists, -1 if Huffman tree wasn't built yet, -2 if specified label wasn't found </returns>
		Public Overridable Function indexOf(ByVal label As String) As Integer Implements VocabCache(Of T).indexOf
			Dim token As T = tokenFor(label)
			If token IsNot Nothing Then
				Return token.getIndex()
			Else
				Return -2
			End If
		End Function

		''' <summary>
		''' Returns collection of SequenceElements stored in this vocabulary
		''' 
		''' @return
		''' </summary>
		Public Overridable Function vocabWords() As ICollection(Of T)
			Return vocabulary.values()
		End Function

		''' <summary>
		''' Returns total number of elements observed
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalWordOccurrences() As Long Implements VocabCache(Of T).totalWordOccurrences
			Return totalWordCount.get()
		End Function

		Public Overridable WriteOnly Property TotalWordOccurences As Long
			Set(ByVal value As Long)
				totalWordCount.set(value)
			End Set
		End Property

		''' <summary>
		''' Returns SequenceElement for specified label
		''' </summary>
		''' <param name="label"> to fetch element for
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public T wordFor(@NonNull String label)
		Public Overridable Function wordFor(ByVal label As String) As T Implements VocabCache(Of T).wordFor
			Return extendedVocabulary(label)
		End Function

		Public Overridable Function wordFor(ByVal id As Long) As T Implements VocabCache(Of T).wordFor
			Return vocabulary.get(id)
		End Function

		''' <summary>
		''' This method allows to insert specified label to specified Huffman tree position.
		''' CAUTION: Never use this, unless you 100% sure what are you doing.
		''' </summary>
		''' <param name="index"> </param>
		''' <param name="label"> </param>
		Public Overridable Sub addWordToIndex(ByVal index As Integer, ByVal label As String) Implements VocabCache(Of T).addWordToIndex
			If index >= 0 Then
				Dim token As T = tokenFor(label)
				If token IsNot Nothing Then
					idxMap(index) = token
					token.setIndex(index)
				End If
			End If
		End Sub

		Public Overridable Sub addWordToIndex(ByVal index As Integer, ByVal elementId As Long) Implements VocabCache(Of T).addWordToIndex
			If index >= 0 Then
				idxMap(index) = tokenFor(elementId)
			End If
		End Sub

		<Obsolete>
		Public Overridable Sub putVocabWord(ByVal word As String) Implements VocabCache(Of T).putVocabWord
			If Not containsWord(word) Then
				Throw New System.InvalidOperationException("Specified label is not present in vocabulary")
			End If
		End Sub

		''' <summary>
		''' Returns number of elements in this vocabulary
		''' 
		''' @return
		''' </summary>
		Public Overridable Function numWords() As Integer Implements VocabCache(Of T).numWords
			Return vocabulary.size()
		End Function

		''' <summary>
		''' Returns number of documents (if applicable) the label was observed in.
		''' </summary>
		''' <param name="word"> the number of documents the word appeared in
		''' @return </param>
		Public Overridable Function docAppearedIn(ByVal word As String) As Integer Implements VocabCache(Of T).docAppearedIn
			Dim element As T = extendedVocabulary(word)
			If element IsNot Nothing Then
				Return CInt(Math.Truncate(element.getSequencesCount()))
			Else
				Return -1
			End If
		End Function

		''' <summary>
		''' Increment number of documents the label was observed in
		''' 
		''' Please note: this method is NOT thread-safe
		''' </summary>
		''' <param name="word"> the word to increment by </param>
		''' <param name="howMuch"> </param>
		Public Overridable Sub incrementDocCount(ByVal word As String, ByVal howMuch As Long) Implements VocabCache(Of T).incrementDocCount
			Dim element As T = extendedVocabulary(word)
			If element IsNot Nothing Then
				element.incrementSequencesCount()
			End If
		End Sub

		''' <summary>
		''' Set exact number of observed documents that contain specified word
		''' 
		''' Please note: this method is NOT thread-safe
		''' </summary>
		''' <param name="word"> the word to set the count for </param>
		''' <param name="count"> the count of the word </param>
		Public Overridable Sub setCountForDoc(ByVal word As String, ByVal count As Long) Implements VocabCache(Of T).setCountForDoc
			Dim element As T = extendedVocabulary(word)
			If element IsNot Nothing Then
				element.setSequencesCount(count)
			End If
		End Sub

		''' <summary>
		''' Returns total number of documents observed (if applicable)
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalNumberOfDocs() As Long Implements VocabCache(Of T).totalNumberOfDocs
			Return documentsCounter.intValue()
		End Function

		''' <summary>
		''' Increment total number of documents observed by 1
		''' </summary>
		Public Overridable Sub incrementTotalDocCount() Implements VocabCache(Of T).incrementTotalDocCount
			documentsCounter.incrementAndGet()
		End Sub

		''' <summary>
		''' Increment total number of documents observed by specified value
		''' </summary>
		Public Overridable Sub incrementTotalDocCount(ByVal by As Long) Implements VocabCache(Of T).incrementTotalDocCount
			documentsCounter.addAndGet(by)
		End Sub

		''' <summary>
		''' This method allows to set total number of documents </summary>
		''' <param name="by"> </param>
		Public Overridable WriteOnly Property TotalDocCount As Long
			Set(ByVal by As Long)
    
				documentsCounter.set(by)
			End Set
		End Property


		''' <summary>
		''' Returns collection of SequenceElements from this vocabulary. The same as vocabWords() method
		''' </summary>
		''' <returns> collection of SequenceElements </returns>
		Public Overridable Function tokens() As ICollection(Of T)
			Return vocabWords()
		End Function

		''' <summary>
		''' This method adds specified SequenceElement to vocabulary
		''' </summary>
		''' <param name="element"> the word to add </param>
		Public Overridable Function addToken(ByVal element As T) As Boolean Implements VocabCache(Of T).addToken
			Dim ret As Boolean = False
			Dim oldElement As T = vocabulary.putIfAbsent(element.getStorageId(), element)
			If oldElement Is Nothing Then
				'putIfAbsent added our element
				If element.getLabel() IsNot Nothing Then
					extendedVocabulary(element.getLabel()) = element
				End If
				oldElement = element
				ret = True
			Else
				oldElement.incrementSequencesCount(element.getSequencesCount())
				oldElement.increaseElementFrequency(CInt(Math.Truncate(element.getElementFrequency())))
			End If
			totalWordCount.addAndGet(CLng(Math.Truncate(oldElement.getElementFrequency())))
			Return ret
		End Function

		Public Overridable Sub addToken(ByVal element As T, ByVal lockf As Boolean)
			Dim oldElement As T = vocabulary.putIfAbsent(element.getStorageId(), element)
			If oldElement Is Nothing Then
				'putIfAbsent added our element
				If element.getLabel() IsNot Nothing Then
					extendedVocabulary(element.getLabel()) = element
				End If
				oldElement = element
			Else
				oldElement.incrementSequencesCount(element.getSequencesCount())
				oldElement.increaseElementFrequency(CInt(Math.Truncate(element.getElementFrequency())))
			End If
			totalWordCount.addAndGet(CLng(Math.Truncate(oldElement.getElementFrequency())))
		End Sub

		''' <summary>
		''' Returns SequenceElement for specified label. The same as wordFor() method.
		''' </summary>
		''' <param name="label"> the label to get the token for
		''' @return </param>
		Public Overridable Function tokenFor(ByVal label As String) As T Implements VocabCache(Of T).tokenFor
			Return wordFor(label)
		End Function

		Public Overridable Function tokenFor(ByVal id As Long) As T Implements VocabCache(Of T).tokenFor
			Return vocabulary.get(id)
		End Function

		''' <summary>
		''' Checks, if specified label already exists in vocabulary. The same as containsWord() method.
		''' </summary>
		''' <param name="label"> the token to test
		''' @return </param>
		Public Overridable Function hasToken(ByVal label As String) As Boolean Implements VocabCache(Of T).hasToken
			Return containsWord(label)
		End Function


		''' <summary>
		''' This method imports all elements from VocabCache passed as argument
		''' If element already exists,
		''' </summary>
		''' <param name="vocabCache"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void importVocabulary(@NonNull VocabCache<T> vocabCache)
		Public Overridable Sub importVocabulary(ByVal vocabCache As VocabCache(Of T)) Implements VocabCache(Of T).importVocabulary
			Dim added As New AtomicBoolean(False)
			For Each element As T In vocabCache.vocabWords()
				If Me.addToken(element) Then
					added.set(True)
				End If
			Next element
			'logger.info("Current state: {}; Adding value: {}", this.documentsCounter.get(), vocabCache.totalNumberOfDocs());
			If added.get() Then
				Me.documentsCounter.addAndGet(vocabCache.totalNumberOfDocs())
			End If
		End Sub

		Public Overridable Sub updateWordsOccurrences() Implements VocabCache(Of T).updateWordsOccurrences
			totalWordCount.set(0)
			For Each element As T In vocabulary.values()
				Dim value As Long = CLng(Math.Truncate(element.getElementFrequency()))

				If value > 0 Then
					totalWordCount.addAndGet(value)
				End If
			Next element
			log.info("Updated counter: [" & totalWordCount.get() & "]")
		End Sub

		Public Overridable Sub removeElement(ByVal label As String) Implements VocabCache(Of T).removeElement
			Dim element As SequenceElement = extendedVocabulary(label)
			If element IsNot Nothing Then
				totalWordCount.getAndAdd(CLng(Math.Truncate(element.getElementFrequency())) * -1)
				idxMap.Remove(element.Index)
				extendedVocabulary.Remove(label)
				vocabulary.remove(element.getStorageId())
			Else
				Throw New System.InvalidOperationException("Can't get label: '" & label & "'")
			End If
		End Sub

		Public Overridable Sub removeElement(ByVal element As T) Implements VocabCache(Of T).removeElement
			removeElement(element.getLabel())
		End Sub

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

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public String toJson() throws org.nd4j.shade.jackson.core.JsonProcessingException
		Public Overridable Function toJson() As String

			Dim retVal As New JsonObject()
			Dim mapper As ObjectMapper = AbstractCache.mapper()
			Dim iter As IEnumerator(Of T) = vocabulary.values().GetEnumerator()
			Dim clazz As Type = Nothing
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If iter.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				clazz = iter.next().GetType()
			Else
				Return retVal.getAsString()
			End If

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			retVal.addProperty(CLASS_FIELD, mapper.writeValueAsString(Me.GetType().FullName))

			Dim jsonValues As New JsonArray()
			For Each value As T In vocabulary.values()
				Dim item As New JsonObject()
				item.addProperty(CLASS_FIELD, mapper.writeValueAsString(clazz))
				item.addProperty(VOCAB_ITEM_FIELD, mapper.writeValueAsString(value))
				jsonValues.add(item)
			Next value
			retVal.add(VOCAB_LIST_FIELD, jsonValues)

			retVal.addProperty(DOC_CNT_FIELD, mapper.writeValueAsString(documentsCounter.longValue()))
			retVal.addProperty(MINW_FREQ_FIELD, mapper.writeValueAsString(minWordFrequency))
			retVal.addProperty(HUGE_MODEL_FIELD, mapper.writeValueAsString(hugeModelExpected))

			retVal.addProperty(STOP_WORDS_FIELD, mapper.writeValueAsString(stopWords))

			retVal.addProperty(SCAVENGER_FIELD, mapper.writeValueAsString(scavengerThreshold))
			retVal.addProperty(RETENTION_FIELD, mapper.writeValueAsString(retentionDelay))
			retVal.addProperty(TOTAL_WORD_FIELD, mapper.writeValueAsString(totalWordCount.longValue()))

			Return retVal.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> AbstractCache<T> fromJson(String jsonString) throws java.io.IOException
		Public Shared Function fromJson(Of T As SequenceElement)(ByVal jsonString As String) As AbstractCache(Of T)
			Dim retVal As AbstractCache(Of T) = (New AbstractCache.Builder(Of T)()).build()

			Dim parser As New JsonParser()
			Dim json As JsonObject = parser.parse(jsonString).getAsJsonObject()

			Dim mapper As ObjectMapper = AbstractCache.mapper()

			Dim wordsCollectionType As CollectionType = mapper.getTypeFactory().constructCollectionType(GetType(System.Collections.IList), GetType(VocabWord))

			Dim items As IList(Of T) = New List(Of T)()
			Dim jsonArray As JsonArray = json.get(VOCAB_LIST_FIELD).getAsJsonArray()
			For i As Integer = 0 To jsonArray.size() - 1
				Dim item As VocabWord = mapper.readValue(jsonArray.get(i).getAsJsonObject().get(VOCAB_ITEM_FIELD).getAsString(), GetType(VocabWord))
				items.Add(CType(item, T))
			Next i

			Dim vocabulary As ConcurrentMap(Of Long, T) = New ConcurrentDictionary(Of Long, T)()
			Dim extendedVocabulary As IDictionary(Of String, T) = New ConcurrentDictionary(Of String, T)()
			Dim idxMap As IDictionary(Of Integer, T) = New ConcurrentDictionary(Of Integer, T)()

			For Each item As T In items
				vocabulary.put(item.getStorageId(), item)
				extendedVocabulary(item.getLabel()) = item
				idxMap(item.getIndex()) = item
			Next item
			Dim stopWords As IList(Of String) = mapper.readValue(json.get(STOP_WORDS_FIELD).getAsString(), GetType(System.Collections.IList))

			Dim documentsCounter As Long? = json.get(DOC_CNT_FIELD).getAsLong()
			Dim minWordsFrequency As Integer? = json.get(MINW_FREQ_FIELD).getAsInt()
			Dim hugeModelExpected As Boolean? = json.get(HUGE_MODEL_FIELD).getAsBoolean()
			Dim scavengerThreshold As Integer? = json.get(SCAVENGER_FIELD).getAsInt()
			Dim retentionDelay As Integer? = json.get(RETENTION_FIELD).getAsInt()
			Dim totalWordCount As Long? = json.get(TOTAL_WORD_FIELD).getAsLong()

			retVal.vocabulary.putAll(vocabulary)
			retVal.extendedVocabulary.PutAll(extendedVocabulary)
			retVal.idxMap.PutAll(idxMap)
			CType(retVal.stopWords, List(Of String)).AddRange(stopWords)
			retVal.documentsCounter.set(documentsCounter)
			retVal.minWordFrequency = minWordsFrequency
			retVal.hugeModelExpected = hugeModelExpected
			retVal.scavengerThreshold = scavengerThreshold
			retVal.retentionDelay = retentionDelay
			retVal.totalWordCount.set(totalWordCount)
			Return retVal
		End Function

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER NOTE: The field scavengerThreshold was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend scavengerThreshold_Conflict As Integer = 3000000
			Protected Friend retentionDelay As Integer = 3
'JAVA TO VB CONVERTER NOTE: The field minElementFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minElementFrequency_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field hugeModelExpected was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend hugeModelExpected_Conflict As Boolean = False


			Public Overridable Function hugeModelExpected(ByVal reallyExpected As Boolean) As Builder(Of T)
				Me.hugeModelExpected_Conflict = reallyExpected
				Return Me
			End Function

			Public Overridable Function scavengerThreshold(ByVal threshold As Integer) As Builder(Of T)
				Me.scavengerThreshold_Conflict = threshold
				Return Me
			End Function

			Public Overridable Function scavengerRetentionDelay(ByVal delay As Integer) As Builder(Of T)
				Me.retentionDelay = delay
				Return Me
			End Function

			Public Overridable Function minElementFrequency(ByVal minFrequency As Integer) As Builder(Of T)
				Me.minElementFrequency_Conflict = minFrequency
				Return Me
			End Function

			Public Overridable Function build() As AbstractCache(Of T)
				Dim cache As New AbstractCache(Of T)()
				cache.minWordFrequency = Me.minElementFrequency_Conflict
				cache.scavengerThreshold = Me.scavengerThreshold_Conflict
				cache.retentionDelay = Me.retentionDelay

				Return cache
			End Function

		End Class
	End Class

End Namespace