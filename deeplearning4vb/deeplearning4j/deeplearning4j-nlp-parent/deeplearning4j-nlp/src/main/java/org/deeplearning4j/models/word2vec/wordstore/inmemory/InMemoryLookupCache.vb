Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports Util = org.deeplearning4j.text.movingwindow.Util
Imports org.nd4j.common.primitives
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports Index = org.nd4j.common.util.Index

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


	<Obsolete, Serializable>
	Public Class InMemoryLookupCache
		Implements VocabCache(Of VocabWord)

		Private wordIndex As New Index()
'JAVA TO VB CONVERTER NOTE: The field wordFrequencies was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public wordFrequencies_Conflict As Counter(Of String) = Util.parallelCounter()
		Public docFrequencies As Counter(Of String) = Util.parallelCounter()
'JAVA TO VB CONVERTER NOTE: The field vocabs was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public vocabs_Conflict As IDictionary(Of String, VocabWord) = New ConcurrentDictionary(Of String, VocabWord)()
'JAVA TO VB CONVERTER NOTE: The field tokens was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public tokens_Conflict As IDictionary(Of String, VocabWord) = New ConcurrentDictionary(Of String, VocabWord)()
'JAVA TO VB CONVERTER NOTE: The field totalWordOccurrences was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly totalWordOccurrences_Conflict As New AtomicLong(0)
		Private numDocs As Integer = 0

		Public Overridable Property WordFrequencies As Counter(Of String)
			Set(ByVal cnt As Counter(Of String))
				SyncLock Me
					Me.wordFrequencies_Conflict = cnt
				End SyncLock
			End Set
			Get
				SyncLock Me
					Return Me.wordFrequencies_Conflict
				End SyncLock
			End Get
		End Property

		Public Overridable Property Vocabs As IDictionary(Of String, VocabWord)
			Get
				SyncLock Me
					Return Me.vocabs_Conflict
				End SyncLock
			End Get
			Set(ByVal vocabs As IDictionary(Of String, VocabWord))
				SyncLock Me
					Me.vocabs_Conflict = vocabs
				End SyncLock
			End Set
		End Property



		Public Overridable Property Tokens As IDictionary(Of String, VocabWord)
			Set(ByVal tokens As IDictionary(Of String, VocabWord))
				SyncLock Me
					Me.tokens_Conflict = tokens
				End SyncLock
			End Set
			Get
				SyncLock Me
					Return Me.tokens_Conflict
				End SyncLock
			End Get
		End Property


		Public Sub New()
			'  this(false);
		End Sub

		<Obsolete>
		Public Sub New(ByVal addUnk As Boolean)
	'        if(addUnk) {
	'            T word = (T) new SequenceElement(); //VocabWord(1.0, Word2Vec.UNK);
	'            word.setIndex(0);
	'            addToken(word);
	'            addWordToIndex(0, Word2Vec.UNK);
	'            putVocabWord(Word2Vec.UNK);
	'        }
	'        
		End Sub

		''' <summary>
		''' Returns all of the words in the vocab
		''' 
		''' @returns all the words in the vocab
		''' </summary>
		Public Overridable Function words() As ICollection(Of String) Implements VocabCache(Of VocabWord).words
			SyncLock Me
				Return vocabs_Conflict.Keys
			End SyncLock
		End Function


		''' <summary>
		''' Increment the count for the given word
		''' </summary>
		''' <param name="word"> the word to increment the count for </param>
		Public Overridable Sub incrementWordCount(ByVal word As String) Implements VocabCache(Of VocabWord).incrementWordCount
			SyncLock Me
				incrementWordCount(word, 1)
			End SyncLock
		End Sub

		''' <summary>
		''' Increment the count for the given word by
		''' the amount increment
		''' </summary>
		''' <param name="word">      the word to increment the count for </param>
		''' <param name="increment"> the amount to increment by </param>
		Public Overridable Sub incrementWordCount(ByVal word As String, ByVal increment As Integer) Implements VocabCache(Of VocabWord).incrementWordCount
			SyncLock Me
				If word Is Nothing OrElse word.Length = 0 Then
					Throw New System.ArgumentException("Word can't be empty or null")
				End If
				wordFrequencies_Conflict.incrementCount(word, increment)
        
				If hasToken(word) Then
					Dim token As VocabWord = tokenFor(word)
					token.increaseElementFrequency(increment)
				End If
				totalWordOccurrences_Conflict.set(totalWordOccurrences_Conflict.get() + increment)
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the number of times the word has occurred
		''' </summary>
		''' <param name="word"> the word to retrieve the occurrence frequency for </param>
		''' <returns> 0 if hasn't occurred or the number of times
		''' the word occurs </returns>
		Public Overridable Function wordFrequency(ByVal word As String) As Integer Implements VocabCache(Of VocabWord).wordFrequency
			SyncLock Me
				Return CInt(Math.Truncate(wordFrequencies_Conflict.getCount(word)))
			End SyncLock
		End Function

		''' <summary>
		''' Returns true if the cache contains the given word
		''' </summary>
		''' <param name="word"> the word to check for
		''' @return </param>
		Public Overridable Function containsWord(ByVal word As String) As Boolean Implements VocabCache(Of VocabWord).containsWord
			SyncLock Me
				Return vocabs_Conflict.ContainsKey(word)
			End SyncLock
		End Function

		''' <summary>
		''' Returns the word contained at the given index or null
		''' </summary>
		''' <param name="index"> the index of the word to get </param>
		''' <returns> the word at the given index </returns>
		Public Overridable Function wordAtIndex(ByVal index As Integer) As String Implements VocabCache(Of VocabWord).wordAtIndex
			SyncLock Me
				Return CStr(wordIndex.get(index))
			End SyncLock
		End Function

		Public Overridable Function elementAtIndex(ByVal index As Integer) As VocabWord
			Return wordFor(wordAtIndex(index))
		End Function

		''' <summary>
		''' Returns the index of a given word
		''' </summary>
		''' <param name="word"> the index of a given word </param>
		''' <returns> the index of a given word or -1
		''' if not found </returns>
		Public Overridable Function indexOf(ByVal word As String) As Integer Implements VocabCache(Of VocabWord).indexOf
			SyncLock Me
				If containsWord(word) Then
					Return wordFor(word).Index
				End If
				Return -1
			End SyncLock
		End Function


		''' <summary>
		''' Returns all of the vocab word nodes
		''' 
		''' @return
		''' </summary>
		Public Overridable Function vocabWords() As ICollection(Of VocabWord) Implements VocabCache(Of VocabWord).vocabWords
			SyncLock Me
				Return vocabs_Conflict.Values
			End SyncLock
		End Function

		''' <summary>
		''' The total number of word occurrences
		''' </summary>
		''' <returns> the total number of word occurrences </returns>
		Public Overridable Function totalWordOccurrences() As Long Implements VocabCache(Of VocabWord).totalWordOccurrences
			SyncLock Me
				Return totalWordOccurrences_Conflict.get()
			End SyncLock
		End Function



		''' <param name="word">
		''' @return </param>
		Public Overridable Function wordFor(ByVal word As String) As VocabWord
			SyncLock Me
				If word Is Nothing Then
					Return Nothing
				End If
				Dim ret As VocabWord = vocabs(word)
				Return ret
			End SyncLock
		End Function

		Public Overridable Function wordFor(ByVal id As Long) As VocabWord
			Throw New System.NotSupportedException()
		End Function

		''' <param name="index"> </param>
		''' <param name="word"> </param>
		Public Overridable Sub addWordToIndex(ByVal index As Integer, ByVal word As String) Implements VocabCache(Of VocabWord).addWordToIndex
			SyncLock Me
				If word Is Nothing OrElse word.Length = 0 Then
					Throw New System.ArgumentException("Word can't be empty or null")
				End If
        
        
        
				If Not tokens_Conflict.ContainsKey(word) Then
					Dim token As New VocabWord(1.0, word)
					tokens(word) = token
					wordFrequencies_Conflict.incrementCount(word, CSng(1.0))
				End If
        
		'        
		'            If we're speaking about adding any word to index directly, it means it's going to be vocab word, not token
		'         
				If Not vocabs_Conflict.ContainsKey(word) Then
					Dim vw As VocabWord = tokenFor(word)
					vw.Index = index
					vocabs(word) = vw
					vw.Index = index
				End If
        
				If Not wordFrequencies_Conflict.containsElement(word) Then
					wordFrequencies_Conflict.incrementCount(word, 1)
				End If
        
				wordIndex.add(word, index)
        
			End SyncLock
		End Sub

		Public Overridable Sub addWordToIndex(ByVal index As Integer, ByVal elementId As Long) Implements VocabCache(Of VocabWord).addWordToIndex
			Throw New System.NotSupportedException()
		End Sub

		''' <param name="word"> </param>
		<Obsolete>
		Public Overridable Sub putVocabWord(ByVal word As String) Implements VocabCache(Of VocabWord).putVocabWord
			SyncLock Me
				If word Is Nothing OrElse word.Length = 0 Then
					Throw New System.ArgumentException("Word can't be empty or null")
				End If
				' STOP and UNK are not added as tokens
				If word.Equals("STOP") OrElse word.Equals("UNK") Then
					Return
				End If
				Dim token As VocabWord = tokenFor(word)
				If token Is Nothing Then
					Throw New System.InvalidOperationException("Word " & word & " not found as token in vocab")
				End If
				Dim ind As Integer = token.Index
				addWordToIndex(ind, word)
				If Not hasToken(word) Then
					Throw New System.InvalidOperationException("Unable to add token " & word & " when not already a token")
				End If
				vocabs(word) = token
				wordIndex.add(word, token.Index)
			End SyncLock
		End Sub


		''' <summary>
		''' Returns the number of words in the cache
		''' </summary>
		''' <returns> the number of words in the cache </returns>
		Public Overridable Function numWords() As Integer Implements VocabCache(Of VocabWord).numWords
			SyncLock Me
				Return vocabs_Conflict.Count
			End SyncLock
		End Function

		Public Overridable Function docAppearedIn(ByVal word As String) As Integer Implements VocabCache(Of VocabWord).docAppearedIn
			SyncLock Me
				Return CInt(Math.Truncate(docFrequencies.getCount(word)))
			End SyncLock
		End Function

		Public Overridable Sub incrementDocCount(ByVal word As String, ByVal howMuch As Long) Implements VocabCache(Of VocabWord).incrementDocCount
			SyncLock Me
				docFrequencies.incrementCount(word, howMuch)
			End SyncLock
		End Sub

		Public Overridable Sub setCountForDoc(ByVal word As String, ByVal count As Long) Implements VocabCache(Of VocabWord).setCountForDoc
			SyncLock Me
				docFrequencies.setCount(word, count)
			End SyncLock
		End Sub

		Public Overridable Function totalNumberOfDocs() As Long Implements VocabCache(Of VocabWord).totalNumberOfDocs
			SyncLock Me
				Return numDocs
			End SyncLock
		End Function

		Public Overridable Sub incrementTotalDocCount() Implements VocabCache(Of VocabWord).incrementTotalDocCount
			SyncLock Me
				numDocs += 1
			End SyncLock
		End Sub

		Public Overridable Sub incrementTotalDocCount(ByVal by As Long) Implements VocabCache(Of VocabWord).incrementTotalDocCount
			SyncLock Me
				numDocs += by
			End SyncLock
		End Sub

		Public Overridable Function tokens() As ICollection(Of VocabWord) Implements VocabCache(Of VocabWord).tokens
			SyncLock Me
				Return tokens_Conflict.Values
			End SyncLock
		End Function

		Public Overridable Function addToken(ByVal word As VocabWord) As Boolean
			SyncLock Me
				If Nothing Is tokens_Conflict.put(word.Label, word) Then
					Return True
				End If
				Return False
			End SyncLock
		End Function

		Public Overridable Function tokenFor(ByVal word As String) As VocabWord
			SyncLock Me
				Return tokens(word)
			End SyncLock
		End Function

		Public Overridable Function tokenFor(ByVal id As Long) As VocabWord
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function hasToken(ByVal token As String) As Boolean Implements VocabCache(Of VocabWord).hasToken
			SyncLock Me
				Return tokenFor(token) IsNot Nothing
			End SyncLock
		End Function

		Public Overridable Sub importVocabulary(ByVal vocabCache As VocabCache(Of VocabWord))
			For Each word As VocabWord In vocabCache.vocabWords()
				If vocabs_Conflict.ContainsKey(word.Label) Then
					wordFrequencies_Conflict.incrementCount(word.Label, CSng(word.getElementFrequency()))
				Else
					tokens(word.Label) = word
					vocabs(word.Label) = word
					wordFrequencies_Conflict.incrementCount(word.Label, CSng(word.getElementFrequency()))
				End If
				totalWordOccurrences_Conflict.addAndGet(CLng(Math.Truncate(word.getElementFrequency())))
			Next word
		End Sub

		Public Overridable Sub updateWordsOccurrences() Implements VocabCache(Of VocabWord).updateWordsOccurrences
			totalWordOccurrences_Conflict.set(0)
			For Each word As VocabWord In vocabWords()
				totalWordOccurrences_Conflict.addAndGet(CLng(Math.Truncate(word.getElementFrequency())))
			Next word
		End Sub

		Public Overridable Sub removeElement(ByVal label As String) Implements VocabCache(Of VocabWord).removeElement
			vocabs_Conflict.Remove(label)
			tokens_Conflict.Remove(label)
		End Sub

		Public Overridable Sub removeElement(ByVal element As VocabWord)
			removeElement(element.Label)
		End Sub


		Public Overridable Sub saveVocab() Implements VocabCache(Of VocabWord).saveVocab
			SyncLock Me
				SerializationUtils.saveObject(Me, New File("ser"))
			End SyncLock
		End Sub

		Public Overridable Function vocabExists() As Boolean Implements VocabCache(Of VocabWord).vocabExists
			SyncLock Me
				Return Directory.Exists("ser") OrElse File.Exists("ser")
			End SyncLock
		End Function


		''' <summary>
		''' Load a look up cache from an input stream
		''' delimited by \n </summary>
		''' <param name="from"> the input stream to read from </param>
		''' <returns> the in memory lookup cache </returns>
		Public Shared Function load(ByVal from As Stream) As InMemoryLookupCache
	'        
	'        Reader inputStream = new InputStreamReader(from);
	'        LineIterator iter = IOUtils.lineIterator(inputStream);
	'        String line;
	'        InMemoryLookupCache ret = new InMemoryLookupCache();
	'        int count = 0;
	'        while((iter.hasNext())) {
	'            line = iter.nextLine();
	'            if(line.isEmpty())
	'                continue;
	'            ret.incrementWordCount(line);
	'            VocabWord word = new VocabWord(1.0,line);
	'            word.setIndex(count);
	'            ret.addToken((SequenceElement) word);
	'            ret.addWordToIndex(count,line);
	'            ret.putVocabWord(line);
	'            count++;
	'        
	'        }
	'        
	'        return ret; 
			Return Nothing
		End Function

		Public Overridable Sub loadVocab() Implements VocabCache(Of VocabWord).loadVocab
			SyncLock Me
				Dim cache As InMemoryLookupCache = SerializationUtils.readObject(New File("ser"))
				Me.vocabs_Conflict = cache.vocabs_Conflict
				Me.wordFrequencies_Conflict = cache.wordFrequencies_Conflict
				Me.wordIndex = cache.wordIndex
				Me.tokens_Conflict = cache.tokens_Conflict
        
        
			End SyncLock
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As InMemoryLookupCache = DirectCast(o, InMemoryLookupCache)

			If numDocs <> that.numDocs Then
				Return False
			End If
			If If(wordIndex IsNot Nothing, Not wordIndex.Equals(that.wordIndex), that.wordIndex IsNot Nothing) Then
				Return False
			End If
			If If(wordFrequencies_Conflict IsNot Nothing, Not wordFrequencies_Conflict.Equals(that.wordFrequencies_Conflict), that.wordFrequencies_Conflict IsNot Nothing) Then
				Return False
			End If
			If If(docFrequencies IsNot Nothing, Not docFrequencies.Equals(that.docFrequencies), that.docFrequencies IsNot Nothing) Then
				Return False
			End If
			If vocabWords().Equals(that.vocabWords()) Then
				Return True
			End If

			Return True

		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = If(wordIndex IsNot Nothing, wordIndex.GetHashCode(), 0)
			result = 31 * result + (If(wordFrequencies_Conflict IsNot Nothing, wordFrequencies_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(docFrequencies IsNot Nothing, docFrequencies.GetHashCode(), 0))
			result = 31 * result + (If(vocabs_Conflict IsNot Nothing, vocabs_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(tokens_Conflict IsNot Nothing, tokens_Conflict.GetHashCode(), 0))
			result = 31 * result + (If(totalWordOccurrences_Conflict IsNot Nothing, totalWordOccurrences_Conflict.GetHashCode(), 0))
			result = 31 * result + numDocs
			Return result
		End Function

		Public Overrides Function ToString() As String
			Return "InMemoryLookupCache{" & "totalWordOccurrences=" & totalWordOccurrences_Conflict & ", numDocs=" & numDocs & "}"c
		End Function
	End Class

End Namespace