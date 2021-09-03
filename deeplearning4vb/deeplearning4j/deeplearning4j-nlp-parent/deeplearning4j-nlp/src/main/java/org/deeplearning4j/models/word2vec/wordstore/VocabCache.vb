Imports System
Imports System.Collections.Generic
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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




	Public Interface VocabCache(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)


		''' <summary>
		''' Load vocab
		''' </summary>
		Sub loadVocab()


		''' <summary>
		''' Vocab exists already
		''' @return
		''' </summary>
		Function vocabExists() As Boolean

		''' <summary>
		''' Saves the vocab: this allow for reuse of word frequencies	
		''' </summary>
		Sub saveVocab()


		''' <summary>
		''' Returns all of the words in the vocab
		''' @returns all the words in the vocab
		''' </summary>
		Function words() As ICollection(Of String)


		''' <summary>
		''' Increment the count for the given word </summary>
		''' <param name="word"> the word to increment the count for </param>
		Sub incrementWordCount(ByVal word As String)


		''' <summary>
		''' Increment the count for the given word by
		''' the amount increment </summary>
		''' <param name="word"> the word to increment the count for </param>
		''' <param name="increment"> the amount to increment by </param>
		Sub incrementWordCount(ByVal word As String, ByVal increment As Integer)

		''' <summary>
		''' Returns the number of times the word has occurred </summary>
		''' <param name="word"> the word to retrieve the occurrence frequency for </param>
		''' <returns> 0 if hasn't occurred or the number of times
		''' the word occurs </returns>
		Function wordFrequency(ByVal word As String) As Integer

		''' <summary>
		''' Returns true if the cache contains the given word </summary>
		''' <param name="word"> the word to check for
		''' @return </param>
		Function containsWord(ByVal word As String) As Boolean

		''' <summary>
		''' Returns the word contained at the given index or null </summary>
		''' <param name="index"> the index of the word to get </param>
		''' <returns> the word at the given index </returns>
		Function wordAtIndex(ByVal index As Integer) As String

		''' <summary>
		''' Returns SequenceElement at the given index or null
		''' </summary>
		''' <param name="index">
		''' @return </param>
		Function elementAtIndex(ByVal index As Integer) As T

		''' <summary>
		''' Returns the index of a given word </summary>
		''' <param name="word"> the index of a given word </param>
		''' <returns> the index of a given word or -1
		''' if not found </returns>
		Function indexOf(ByVal word As String) As Integer


		''' <summary>
		''' Returns all of the vocab word nodes
		''' @return
		''' </summary>
		Function vocabWords() As ICollection(Of T)


		''' <summary>
		''' The total number of word occurrences </summary>
		''' <returns> the total number of word occurrences </returns>
		Function totalWordOccurrences() As Long


		''' 
		''' <param name="word">
		''' @return </param>
		Function wordFor(ByVal word As String) As T


		Function wordFor(ByVal id As Long) As T

		''' 
		''' <param name="index"> </param>
		''' <param name="word"> </param>
		Sub addWordToIndex(ByVal index As Integer, ByVal word As String)


		Sub addWordToIndex(ByVal index As Integer, ByVal elementId As Long)

		''' <summary>
		''' Inserts the word as a vocab word
		''' (it gets the vocab word from the internal token store).
		''' Note that the index must be set on the token. </summary>
		''' <param name="word"> the word to add to the vocab </param>
		<Obsolete>
		Sub putVocabWord(ByVal word As String)

		''' <summary>
		''' Returns the number of words in the cache </summary>
		''' <returns> the number of words in the cache </returns>
		Function numWords() As Integer


		''' <summary>
		''' Count of documents a word appeared in </summary>
		''' <param name="word"> the number of documents the word appeared in
		''' @return </param>
		Function docAppearedIn(ByVal word As String) As Integer

		''' <summary>
		''' Increment the document count </summary>
		''' <param name="word"> the word to increment by </param>
		''' <param name="howMuch"> </param>
		Sub incrementDocCount(ByVal word As String, ByVal howMuch As Long)


		''' <summary>
		''' Set the count for the number of documents the word appears in </summary>
		''' <param name="word"> the word to set the count for </param>
		''' <param name="count"> the count of the word </param>
		Sub setCountForDoc(ByVal word As String, ByVal count As Long)

		''' <summary>
		''' Returns the total of number of documents encountered in the corpus </summary>
		''' <returns> the total number of docs in the corpus </returns>
		Function totalNumberOfDocs() As Long


		''' <summary>
		''' Increment the doc count
		''' </summary>
		Sub incrementTotalDocCount()

		''' <summary>
		''' Increment the doc count </summary>
		''' <param name="by"> the number to increment by </param>
		Sub incrementTotalDocCount(ByVal by As Long)

		''' <summary>
		''' All of the tokens in the cache, (not necessarily apart of the vocab) </summary>
		''' <returns> the tokens for this cache </returns>
		Function tokens() As ICollection(Of T)


		''' <summary>
		''' Adds a token
		''' to the cache </summary>
		''' <param name="element"> the word to add </param>
		''' <returns> true if token was added, false if updated </returns>
		Function addToken(ByVal element As T) As Boolean

		''' <summary>
		''' Returns the token (again not necessarily in the vocab)
		''' for this word </summary>
		''' <param name="word"> the word to get the token for </param>
		''' <returns> the vocab word for this token </returns>
		Function tokenFor(ByVal word As String) As T

		Function tokenFor(ByVal id As Long) As T

		''' <summary>
		''' Returns whether the cache
		''' contains this token or not </summary>
		''' <param name="token"> the token to tes </param>
		''' <returns> whether the token exists in
		''' the cache or not
		'''  </returns>
		Function hasToken(ByVal token As String) As Boolean


		''' <summary>
		''' imports vocabulary
		''' </summary>
		''' <param name="vocabCache"> </param>
		Sub importVocabulary(ByVal vocabCache As VocabCache(Of T))

		''' <summary>
		''' Updates counters
		''' </summary>
		Sub updateWordsOccurrences()

		''' <summary>
		''' Removes element with specified label from vocabulary
		''' Please note: Huffman index should be updated after element removal
		''' </summary>
		''' <param name="label"> label of the element to be removed </param>
		Sub removeElement(ByVal label As String)


		''' <summary>
		''' Removes specified element from vocabulary
		''' Please note: Huffman index should be updated after element removal
		''' </summary>
		''' <param name="element"> SequenceElement to be removed </param>
		Sub removeElement(ByVal element As T)

	End Interface

End Namespace