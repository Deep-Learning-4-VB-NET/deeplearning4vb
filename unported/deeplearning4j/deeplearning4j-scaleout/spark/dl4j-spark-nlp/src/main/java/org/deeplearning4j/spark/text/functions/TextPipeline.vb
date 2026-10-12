Imports System.Collections.Generic
Imports Accumulator = org.apache.spark.Accumulator
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports Huffman = org.deeplearning4j.models.word2vec.Huffman
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports WordFreqAccumulator = org.deeplearning4j.spark.text.accumulators.WordFreqAccumulator
Imports AtomicDouble = org.nd4j.common.primitives.AtomicDouble
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.spark.text.functions


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public class TextPipeline
	Public Class TextPipeline
		'params
		Private corpusRDD As JavaRDD(Of String)
		Private numWords As Integer
		Private nGrams As Integer
		Private tokenizer As String
		Private tokenizerPreprocessor As String
		Private stopWords As IList(Of String) = New List(Of String)()
		'Setup
		Private sc As JavaSparkContext
		Private wordFreqAcc As Accumulator(Of Counter(Of String))
		Private stopWordBroadCast As Broadcast(Of IList(Of String))
		' Return values
		Private sentenceWordsCountRDD As JavaRDD(Of Pair(Of IList(Of String), AtomicLong))
		Private vocabCache As VocabCache(Of VocabWord) = New AbstractCache(Of VocabWord)()
		Private vocabCacheBroadcast As Broadcast(Of VocabCache(Of VocabWord))
		Private vocabWordListRDD As JavaRDD(Of IList(Of VocabWord))
		Private sentenceCountRDD As JavaRDD(Of AtomicLong)
		Private totalWordCount As Long
		Private useUnk As Boolean
		Private configuration As VectorsConfiguration

		' Empty Constructor
		Public Sub New()
		End Sub

		' Constructor
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public TextPipeline(org.apache.spark.api.java.JavaRDD<String> corpusRDD, org.apache.spark.broadcast.Broadcast<java.util.Map<String, Object>> broadcasTokenizerVarMap) throws Exception
		Public Sub New(ByVal corpusRDD As JavaRDD(Of String), ByVal broadcasTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)))
			setRDDVarMap(corpusRDD, broadcasTokenizerVarMap)
			' Setup all Spark variables
			setup()
		End Sub

		Public Overridable Sub setRDDVarMap(ByVal corpusRDD As JavaRDD(Of String), ByVal broadcasTokenizerVarMap As Broadcast(Of IDictionary(Of String, Object)))
			Dim tokenizerVarMap As IDictionary(Of String, Object) = broadcasTokenizerVarMap.getValue()
			Me.corpusRDD = corpusRDD
			Me.numWords = DirectCast(tokenizerVarMap("numWords"), Integer)
			' TokenizerFunction Settings
			Me.nGrams = DirectCast(tokenizerVarMap("nGrams"), Integer)
			Me.tokenizer = DirectCast(tokenizerVarMap("tokenizer"), String)
			Me.tokenizerPreprocessor = DirectCast(tokenizerVarMap("tokenPreprocessor"), String)
			Me.useUnk = DirectCast(tokenizerVarMap("useUnk"), Boolean)
			Me.configuration = DirectCast(tokenizerVarMap("vectorsConfiguration"), VectorsConfiguration)
			' Remove Stop words
			' if ((boolean) tokenizerVarMap.get("removeStop")) {
			stopWords = DirectCast(tokenizerVarMap("stopWords"), IList(Of String))
			'    }
		End Sub

		Private Sub setup()
			' Set up accumulators and broadcast stopwords
			Me.sc = New JavaSparkContext(corpusRDD.context())
			Me.wordFreqAcc = sc.accumulator(New Counter(Of String)(), New WordFreqAccumulator())
			Me.stopWordBroadCast = sc.broadcast(stopWords)
		End Sub

		Public Overridable Function tokenize() As JavaRDD(Of IList(Of String))
			If corpusRDD Is Nothing Then
				Throw New System.InvalidOperationException("corpusRDD not assigned. Define TextPipeline with corpusRDD assigned.")
			End If
			Return corpusRDD.map(New TokenizerFunction(tokenizer, tokenizerPreprocessor, nGrams))
		End Function

		Public Overridable Function updateAndReturnAccumulatorVal(ByVal tokenizedRDD As JavaRDD(Of IList(Of String))) As JavaRDD(Of Pair(Of IList(Of String), AtomicLong))
			' Update the 2 accumulators
			Dim accumulatorClassFunction As New UpdateWordFreqAccumulatorFunction(stopWordBroadCast, wordFreqAcc)
			Dim sentenceWordsCountRDD As JavaRDD(Of Pair(Of IList(Of String), AtomicLong)) = tokenizedRDD.map(accumulatorClassFunction)

			' Loop through each element to update accumulator. Count does the same job (verified).
			sentenceWordsCountRDD.count()

			Return sentenceWordsCountRDD
		End Function

		Private Function filterMinWord(ByVal stringToken As String, ByVal tokenCount As Double) As String
			Return If(tokenCount < numWords, configuration.getUNK(), stringToken)
		End Function

		Private Sub addTokenToVocabCache(ByVal stringToken As String, ByVal tokenCount As Single?)
			' Making string token into actual token if not already an actual token (vocabWord)
			Dim actualToken As VocabWord
			If vocabCache.hasToken(stringToken) Then
				actualToken = vocabCache.tokenFor(stringToken)
				actualToken.increaseElementFrequency(tokenCount.Value)
			Else
				actualToken = New VocabWord(tokenCount, stringToken)
			End If

			' Set the index of the actual token (vocabWord)
			' Put vocabWord into vocabs in InMemoryVocabCache
			Dim vocabContainsWord As Boolean = vocabCache.containsWord(stringToken)
			If Not vocabContainsWord Then
				Dim idx As Integer = vocabCache.numWords()

				vocabCache.addToken(actualToken)
				actualToken.Index = idx
				vocabCache.putVocabWord(stringToken)
			End If
		End Sub

		Public Overridable Sub filterMinWordAddVocab(ByVal wordFreq As Counter(Of String))

			If wordFreq.Empty Then
				Throw New System.InvalidOperationException("IllegalStateException: wordFreqCounter has nothing. Check accumulator updating")
			End If

			For Each entry As KeyValuePair(Of String, AtomicDouble) In wordFreq.entrySet()
				Dim stringToken As String = entry.Key
				Dim tokenCount As Double? = entry.Value.doubleValue()

				' Turn words below min count to UNK
				stringToken = filterMinWord(stringToken, tokenCount)
				If Not useUnk AndAlso stringToken.Equals("UNK") Then
					' Turn tokens to vocab and add to vocab cache
				Else
					addTokenToVocabCache(stringToken, tokenCount.Value)
				End If
			Next entry
		End Sub

		Public Overridable Sub buildVocabCache()

			' Tokenize
			Dim tokenizedRDD As JavaRDD(Of IList(Of String)) = tokenize()

			' Update accumulator values and map to an RDD of sentence counts
			sentenceWordsCountRDD = updateAndReturnAccumulatorVal(tokenizedRDD).cache()

			' Get value from accumulator
			Dim wordFreqCounter As Counter(Of String) = wordFreqAcc.value()

			' Filter out low count words and add to vocab cache object and feed into LookupCache
			filterMinWordAddVocab(wordFreqCounter)

			' huffman tree should be built BEFORE vocab broadcast
			Dim huffman As New Huffman(vocabCache.vocabWords())
			huffman.build()
			huffman.applyIndexes(vocabCache)

			' At this point the vocab cache is built. Broadcast vocab cache
			vocabCacheBroadcast = sc.broadcast(vocabCache)

		End Sub

		Public Overridable Sub buildVocabWordListRDD()

			If sentenceWordsCountRDD Is Nothing Then
				Throw New System.InvalidOperationException("SentenceWordCountRDD must be defined first. Run buildLookupCache first.")
			End If

			vocabWordListRDD = sentenceWordsCountRDD.map(New WordsListToVocabWordsFunction(vocabCacheBroadcast)).setName("vocabWordListRDD").cache()
			sentenceCountRDD = sentenceWordsCountRDD.map(New GetSentenceCountFunction()).setName("sentenceCountRDD").cache()
			' Actions to fill vocabWordListRDD and sentenceCountRDD
			vocabWordListRDD.count()
			totalWordCount = sentenceCountRDD.reduce(New ReduceSentenceCount()).get()

			' Release sentenceWordsCountRDD from cache
			sentenceWordsCountRDD.unpersist()
		End Sub

		' Getters
		Public Overridable ReadOnly Property WordFreqAcc As Accumulator(Of Counter(Of String))
			Get
				If wordFreqAcc IsNot Nothing Then
					Return wordFreqAcc
				Else
					Throw New System.InvalidOperationException("IllegalStateException: wordFreqAcc not set at TextPipline.")
				End If
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.apache.spark.broadcast.Broadcast<org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord>> getBroadCastVocabCache() throws IllegalStateException
		Public Overridable ReadOnly Property BroadCastVocabCache As Broadcast(Of VocabCache(Of VocabWord))
			Get
				If vocabCache.numWords() > 0 Then
					Return vocabCacheBroadcast
				Else
					Throw New System.InvalidOperationException("IllegalStateException: VocabCache not set at TextPipline.")
				End If
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord> getVocabCache() throws IllegalStateException
		Public Overridable ReadOnly Property VocabCache As VocabCache(Of VocabWord)
			Get
				If vocabCache IsNot Nothing AndAlso vocabCache.numWords() > 0 Then
					Return vocabCache
				Else
					Throw New System.InvalidOperationException("IllegalStateException: VocabCache not set at TextPipline.")
				End If
			End Get
		End Property

		Public Overridable ReadOnly Property SentenceWordsCountRDD As JavaRDD(Of Pair(Of IList(Of String), AtomicLong))
			Get
				If sentenceWordsCountRDD IsNot Nothing Then
					Return sentenceWordsCountRDD
				Else
					Throw New System.InvalidOperationException("IllegalStateException: sentenceWordsCountRDD not set at TextPipline.")
				End If
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.apache.spark.api.java.JavaRDD<java.util.List<org.deeplearning4j.models.word2vec.VocabWord>> getVocabWordListRDD() throws IllegalStateException
		Public Overridable ReadOnly Property VocabWordListRDD As JavaRDD(Of IList(Of VocabWord))
			Get
				If vocabWordListRDD IsNot Nothing Then
					Return vocabWordListRDD
				Else
					Throw New System.InvalidOperationException("IllegalStateException: vocabWordListRDD not set at TextPipline.")
				End If
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.apache.spark.api.java.JavaRDD<java.util.concurrent.atomic.AtomicLong> getSentenceCountRDD() throws IllegalStateException
		Public Overridable ReadOnly Property SentenceCountRDD As JavaRDD(Of AtomicLong)
			Get
				If sentenceCountRDD IsNot Nothing Then
					Return sentenceCountRDD
				Else
					Throw New System.InvalidOperationException("IllegalStateException: sentenceCountRDD not set at TextPipline.")
				End If
			End Get
		End Property

		Public Overridable ReadOnly Property TotalWordCount As Long?
			Get
				If totalWordCount <> 0L Then
					Return totalWordCount
				Else
					Throw New System.InvalidOperationException("IllegalStateException: totalWordCount not set at TextPipline.")
				End If
			End Get
		End Property
	End Class

End Namespace