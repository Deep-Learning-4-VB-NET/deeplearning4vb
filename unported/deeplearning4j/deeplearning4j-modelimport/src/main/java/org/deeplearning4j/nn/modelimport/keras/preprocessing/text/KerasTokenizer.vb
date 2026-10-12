Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.deeplearning4j.nn.modelimport.keras.utils.KerasModelUtils.parseJsonString

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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessing.text


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class KerasTokenizer
	Public Class KerasTokenizer

		' TODO: might want to recreate "one_hot" util for tokenizer

		Private Shared ReadOnly DEFAULT_FILTER As String = "!""#$%&()*+,-./:;<=>?@[\]^_`{|}~" & vbTab & vbLf
		Private Const DEFAULT_SPLIT As String = " "

		Private numWords As Integer?
		Private filters As String
		Private lower As Boolean
		Private split As String
		Private charLevel As Boolean
		Private outOfVocabularyToken As String

		Private wordCounts As IDictionary(Of String, Integer) = New LinkedHashMap(Of String, Integer)()
		Private wordDocs As New Dictionary(Of String, Integer)()
		Private wordIndex As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
		Private indexWord As IDictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
		Private indexDocs As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
		Private documentCount As Integer?



		''' <summary>
		''' Create a Keras Tokenizer instance with full set of properties.
		''' </summary>
		''' <param name="numWords">             The maximum vocabulary size, can be null </param>
		''' <param name="filters">              Characters to filter </param>
		''' <param name="lower">                whether to lowercase input or not </param>
		''' <param name="split">                by which string to split words (usually single space) </param>
		''' <param name="charLevel">            whether to operate on character- or word-level </param>
		''' <param name="outOfVocabularyToken"> replace items outside the vocabulary by this token </param>
		Public Sub New(ByVal numWords As Integer?, ByVal filters As String, ByVal lower As Boolean, ByVal split As String, ByVal charLevel As Boolean, ByVal outOfVocabularyToken As String)

			Me.numWords = numWords
			Me.filters = filters
			Me.lower = lower
			Me.split = split
			Me.charLevel = charLevel
			Me.outOfVocabularyToken = outOfVocabularyToken
		End Sub


		''' <summary>
		''' Tokenizer constructor with only numWords specified
		''' </summary>
		''' <param name="numWords">             The maximum vocabulary size, can be null </param>
		Public Sub New(ByVal numWords As Integer?)
			Me.New(numWords, DEFAULT_FILTER, True, DEFAULT_SPLIT, False, Nothing)
		End Sub

		''' <summary>
		''' Default Keras tokenizer constructor
		''' </summary>
		Public Sub New()
			Me.New(Nothing, DEFAULT_FILTER, True, DEFAULT_SPLIT, False, Nothing)
		End Sub


		''' <summary>
		''' Import Keras Tokenizer from JSON file created with `tokenizer.to_json()` in Python.
		''' </summary>
		''' <param name="jsonFileName"> Full path of the JSON file to load </param>
		''' <returns> Keras Tokenizer instance loaded from JSON </returns>
		''' <exception cref="IOException"> I/O exception </exception>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid Keras configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static KerasTokenizer fromJson(String jsonFileName) throws IOException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function fromJson(ByVal jsonFileName As String) As KerasTokenizer
			Dim json As New String(Files.readAllBytes(Paths.get(jsonFileName)))
			Dim tokenizerBaseConfig As IDictionary(Of String, Object) = parseJsonString(json)
			Dim tokenizerConfig As IDictionary(Of String, Object)
			If tokenizerBaseConfig.ContainsKey("config") Then
				tokenizerConfig = DirectCast(tokenizerBaseConfig("config"), IDictionary(Of String, Object))
			Else
				Throw New InvalidKerasConfigurationException("No configuration found for Keras tokenizer")
			End If


			Dim numWords As Integer? = DirectCast(tokenizerConfig("num_words"), Integer?)
			Dim filters As String = DirectCast(tokenizerConfig("filters"), String)
			Dim lower As Boolean? = DirectCast(tokenizerConfig("lower"), Boolean?)
			Dim split As String = DirectCast(tokenizerConfig("split"), String)
			Dim charLevel As Boolean? = DirectCast(tokenizerConfig("char_level"), Boolean?)
			Dim oovToken As String = DirectCast(tokenizerConfig("oov_token"), String)
			Dim documentCount As Integer? = DirectCast(tokenizerConfig("document_count"), Integer?)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") Map<String, Integer> wordCounts = (Map) parseJsonString((String) tokenizerConfig.get("word_counts"));
			Dim wordCounts As IDictionary(Of String, Integer) = CType(parseJsonString(DirectCast(tokenizerConfig("word_counts"), String)), System.Collections.IDictionary)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") Map<String, Integer> wordDocs = (Map) parseJsonString((String) tokenizerConfig.get("word_docs"));
			Dim wordDocs As IDictionary(Of String, Integer) = CType(parseJsonString(DirectCast(tokenizerConfig("word_docs"), String)), System.Collections.IDictionary)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") Map<String, Integer> wordIndex = (Map) parseJsonString((String) tokenizerConfig.get("word_index"));
			Dim wordIndex As IDictionary(Of String, Integer) = CType(parseJsonString(DirectCast(tokenizerConfig("word_index"), String)), System.Collections.IDictionary)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") Map<Integer, String> indexWord = (Map) parseJsonString((String) tokenizerConfig.get("index_word"));
			Dim indexWord As IDictionary(Of Integer, String) = CType(parseJsonString(DirectCast(tokenizerConfig("index_word"), String)), System.Collections.IDictionary)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") Map<Integer, Integer> indexDocs = (Map) parseJsonString((String) tokenizerConfig.get("index_docs"));
			Dim indexDocs As IDictionary(Of Integer, Integer) = CType(parseJsonString(DirectCast(tokenizerConfig("index_docs"), String)), System.Collections.IDictionary)

			Dim tokenizer As New KerasTokenizer(numWords, filters, lower, split, charLevel, oovToken)
			tokenizer.setDocumentCount(documentCount)
			tokenizer.setWordCounts(wordCounts)
			tokenizer.setWordDocs(New Dictionary(Of )(wordDocs))
			tokenizer.setWordIndex(wordIndex)
			tokenizer.setIndexWord(indexWord)
			tokenizer.setIndexDocs(indexDocs)

			Return tokenizer
		End Function

		''' <summary>
		''' Turns a String text into a sequence of tokens.
		''' </summary>
		''' <param name="text">                 input text </param>
		''' <param name="filters">              characters to filter </param>
		''' <param name="lower">                whether to lowercase input or not </param>
		''' <param name="split">                by which string to split words (usually single space) </param>
		''' <returns> Sequence of tokens as String array </returns>
		Public Shared Function textToWordSequence(ByVal text As String, ByVal filters As String, ByVal lower As Boolean, ByVal split As String) As String()
			If lower Then
				text = text.ToLower()
			End If

			For Each filter As String In filters.Split("", True)
				text = text.Replace(filter, split)
			Next filter
			Dim sequences() As String = text.Split(split, True)
			Dim seqList As IList(Of String) = New List(Of String) From {sequences}
			seqList.RemoveAll(java.util.Arrays.asList("", Nothing))

			Return CType(seqList, List(Of String)).ToArray()
		End Function

		''' <summary>
		''' Fit this tokenizer on a corpus of texts.
		''' </summary>
		''' <param name="texts"> array of strings to fit tokenizer on. </param>
		Public Overridable Sub fitOnTexts(ByVal texts() As String)
			Dim sequence() As String
			For Each text As String In texts
				If documentCount Is Nothing Then
					documentCount = 1
				Else
					documentCount += 1
				End If
				If charLevel Then
					If lower Then
						text = text.ToLower()
					End If
					sequence = text.Split("", True)
				Else
					sequence = textToWordSequence(text, filters, lower, split)
				End If
				For Each word As String In sequence
					If wordCounts.ContainsKey(word) Then
						wordCounts(word) = wordCounts(word) + 1
					Else
						wordCounts(word) = 1
					End If
				Next word
				Dim sequenceSet As ISet(Of String) = New HashSet(Of String)(java.util.Arrays.asList(sequence))
				For Each word As String In sequenceSet
					If wordDocs.ContainsKey(word) Then
						wordDocs(word) = wordDocs(word) + 1
					Else
						wordDocs(word) = 1
					End If
				Next word
			Next text
			Dim sortedWordCounts As IDictionary(Of String, Integer) = reverseSortByValues(CType(wordCounts, Hashtable))

			Dim sortedVocabulary As New List(Of String)()
			If outOfVocabularyToken IsNot Nothing Then
				sortedVocabulary.Add(outOfVocabularyToken)
			End If
			For Each word As String In sortedWordCounts.Keys
				sortedVocabulary.Add(word)
			Next word

			For i As Integer = 0 To sortedVocabulary.Count - 1
				wordIndex(sortedVocabulary(i)) = i+1
			Next i

			For Each key As String In wordIndex.Keys
				indexWord(wordIndex(key)) = key
			Next key

			For Each key As String In wordDocs.Keys
				indexDocs(wordIndex(key)) = wordDocs(key)
			Next key
		End Sub

		''' <summary>
		''' Sort HashMap by values in reverse order
		''' </summary>
		''' <param name="map"> input HashMap </param>
		''' <returns> sorted HashMap </returns>
		Private Shared Function reverseSortByValues(ByVal map As Hashtable) As Hashtable
			Dim list As System.Collections.IList = New LinkedList(map.SetOfKeyValuePairs())
			list.Sort(New ComparatorAnonymousInnerClass())
			Dim sortedHashMap As Hashtable = New LinkedHashMap()
			Dim it As System.Collections.IEnumerator = list.GetEnumerator()
			Do While it.MoveNext()
				Dim entry As DictionaryEntry = CType(it.Current, DictionaryEntry)
				sortedHashMap(entry.Key) = entry.Value
			Loop
			Return sortedHashMap
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements System.Collections.IComparer

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public int compare(Object o1, Object o2)
			If True Then
				Return CType(DirectCast(o1, DictionaryEntry).Value, IComparable).CompareTo(DirectCast(o2, DictionaryEntry).Value)
			End If
		End Class

		''' <summary>
		''' Fit this tokenizer on a corpus of word indices
		''' </summary>
		''' <param name="sequences"> array of indices derived from a text. </param>
		public void fitOnSequences(Integer?()() sequences)
		If True Then
			documentCount += 1
			For Each sequence As System.Nullable()(Of Integer) In sequences
				Dim sequenceSet As ISet(Of Integer) = New HashSet(Of Integer)(java.util.Arrays.asList(sequence))
				For Each index As Integer? In sequenceSet
					indexDocs(index) = indexDocs(index) + 1
				Next index
			Next sequence
		End If

		''' <summary>
		''' Transforms a bunch of texts into their index representations.
		''' </summary>
		''' <param name="texts"> input texts </param>
		''' <returns> array of indices of the texts </returns>
		public Integer?()() textsToSequences(String() texts)
		If True Then
			Dim oovTokenIndex As Integer? = wordIndex(outOfVocabularyToken)
			Dim wordSequence() As String
			Dim sequences As New List(Of Integer())()
			For Each text As String In texts
				If charLevel Then
					If lower Then
						text = text.ToLower()
					End If
					wordSequence = text.Split("", True)
				Else
					wordSequence = textToWordSequence(text, filters, lower, split)
				End If
				Dim indexVector As New List(Of Integer)()
				For Each word As String In wordSequence
					If wordIndex.ContainsKey(word) Then
						Dim index As Integer = wordIndex(word)
						If numWords IsNot Nothing AndAlso index >= numWords Then
							If oovTokenIndex IsNot Nothing Then
								indexVector.Add(oovTokenIndex)
							End If
						Else
							indexVector.Add(index)
						End If
					ElseIf oovTokenIndex IsNot Nothing Then
						indexVector.Add(oovTokenIndex)
					End If
				Next word
				Dim indices() As Integer? = indexVector.ToArray()
				sequences.Add(indices)
			Next text
			Return sequences.ToArray()
		End If


		''' <summary>
		''' Turns index sequences back into texts
		''' </summary>
		''' <param name="sequences"> index sequences </param>
		''' <returns> text reconstructed from sequences </returns>
		public String() sequencesToTexts(Integer?()() sequences)
		If True Then
			Dim oovTokenIndex As Integer? = wordIndex(outOfVocabularyToken)
			Dim texts As New List(Of String)()
			For Each sequence As System.Nullable()(Of Integer) In sequences
				Dim wordVector As New List(Of String)()
				For Each index As Integer? In sequence
					If indexWord.ContainsKey(index) Then
						Dim word As String = indexWord(index)
						If numWords IsNot Nothing AndAlso index >= numWords Then
							If oovTokenIndex IsNot Nothing Then
								wordVector.Add(indexWord(oovTokenIndex))
							Else
								wordVector.Add(word)
							End If
						End If
					ElseIf oovTokenIndex IsNot Nothing Then
						wordVector.Add(indexWord(oovTokenIndex))
					End If
				Next index
				Dim builder As New StringBuilder()
				For Each word As String In wordVector
					builder.Append(word & split)
				Next word
				Dim text As String = builder.ToString()
				texts.Add(text)
			Next sequence
			Return texts.ToArray()
		End If


		''' <summary>
		''' Turns an array of texts into an ND4J matrix of shape
		''' (number of texts, number of words in vocabulary)
		''' </summary>
		''' <param name="texts"> input texts </param>
		''' <param name="mode"> TokenizerMode that controls how to vectorize data </param>
		''' <returns> resulting matrix representation </returns>
		public INDArray textsToMatrix(String() texts, TokenizerMode mode)
		If True Then
			Dim sequences()() As Integer? = textsToSequences(texts)
			Return sequencesToMatrix(sequences, mode)
		End If

		''' <summary>
		''' Turns an array of index sequences into an ND4J matrix of shape
		''' (number of texts, number of words in vocabulary)
		''' </summary>
		''' <param name="sequences"> input sequences </param>
		''' <param name="mode"> TokenizerMode that controls how to vectorize data </param>
		''' <returns> resulting matrix representatio </returns>
		public INDArray sequencesToMatrix(Integer?()() sequences, TokenizerMode mode)
		If True Then
			If numWords Is Nothing Then
				If wordIndex.Count > 0 Then
					numWords = wordIndex.Count
				Else
					Throw New System.ArgumentException("Either specify numWords argument" & "or fit Tokenizer on data first, i.e. by using fitOnTexts")
				End If
			End If
			If mode.Equals(TokenizerMode.TFIDF) AndAlso documentCount Is Nothing Then
				Throw New System.ArgumentException("To use TFIDF mode you need to" & "fit the Tokenizer instance with fitOnTexts first.")
			End If
			Dim x As INDArray = Nd4j.zeros(sequences.length, numWords)
			For i As Integer = 0 To sequences.length - 1
				Dim sequence() As Integer? = sequences(i)
				If sequence Is Nothing Then
					Continue For
				End If
				Dim counts As New Dictionary(Of Integer, Integer)()
				For Each j As Integer In sequence
					If j >= numWords Then
						Continue For
					End If
					If counts.ContainsKey(j) Then
						counts(j) = counts(j) + 1
					Else
						counts(j) = 1
					End If
				Next j
				For Each j As Integer In counts.Keys
					Dim count As Integer = counts(j)
					Select Case mode
						Case COUNT
							x.put(i, j, count)
						Case FREQ
							x.put(i, j, count \ sequence.Length)
						Case BINARY
							x.put(i, j, 1)
						Case TFIDF
							Dim tf As Double = 1.0 + Math.Log(count)
							Dim index As Integer = If(indexDocs.ContainsKey(j), indexDocs(j), 0)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
							Dim idf As Double = Math.Log(1 + documentCount.Value / (1.0 + index))
							x.put(i, j, tf * idf)
					End Select
				Next j
			Next i
			Return x
		End If

	End Class

End Namespace