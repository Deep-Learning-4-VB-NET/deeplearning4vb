﻿Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports DocumentIterator = org.deeplearning4j.text.documentiterator.DocumentIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelAwareIteratorWrapper = org.deeplearning4j.text.documentiterator.LabelAwareIteratorWrapper
Imports LabelsSource = org.deeplearning4j.text.documentiterator.LabelsSource
Imports DocumentIteratorConverter = org.deeplearning4j.text.documentiterator.interoperability.DocumentIteratorConverter
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports SentenceIteratorConverter = org.deeplearning4j.text.sentenceiterator.interoperability.SentenceIteratorConverter
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports MathUtils = org.nd4j.common.util.MathUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil

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

Namespace org.deeplearning4j.bagofwords.vectorizer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TfidfVectorizer extends BaseTextVectorizer
	<Serializable>
	Public Class TfidfVectorizer
		Inherits BaseTextVectorizer

		Public Overrides Function vectorize(ByVal [is] As Stream, ByVal label As String) As DataSet
			Try
				Dim reader As New StreamReader([is], Encoding.UTF8)
				Dim line As String = ""
				Dim builder As New StringBuilder()
				line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
				Do While line IsNot Nothing
					builder.Append(line)
						line = reader.ReadLine()
				Loop
				Return vectorize(builder.ToString(), label)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Vectorizes the passed in text treating it as one document
		''' </summary>
		''' <param name="text">  the text to vectorize </param>
		''' <param name="label"> the label of the text </param>
		''' <returns> a dataset with a transform of weights(relative to impl; could be word counts or tfidf scores) </returns>
		Public Overrides Function vectorize(ByVal text As String, ByVal label As String) As DataSet
			Dim input As INDArray = transform(text)
			Dim labelMatrix As INDArray = FeatureUtil.toOutcomeVector(labelsSource_Conflict.indexOf(label), labelsSource_Conflict.size())

			Return New DataSet(input, labelMatrix)
		End Function

		''' <param name="input"> the text to vectorize </param>
		''' <param name="label"> the label of the text </param>
		''' <returns> <seealso cref="DataSet"/> with a applyTransformToDestination of
		''' weights(relative to impl; could be word counts or tfidf scores) </returns>
		Public Overrides Function vectorize(ByVal input As File, ByVal label As String) As DataSet
			Try
				Dim [string] As String = FileUtils.readFileToString(input)
				Return vectorize([string], label)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Transforms the matrix
		''' </summary>
		''' <param name="text"> text to transform </param>
		''' <returns> <seealso cref="INDArray"/> </returns>
		Public Overrides Function transform(ByVal text As String) As INDArray
			Dim tokenizer As Tokenizer = tokenizerFactory.create(text)
			Dim tokens As IList(Of String) = tokenizer.getTokens()

			' build document words count
			Return transform(tokens)
		End Function


		Public Overridable Overloads Function transform(ByVal tokens As IList(Of String)) As INDArray
			Dim ret As INDArray = Nd4j.create(1, vocabCache.numWords())

			Dim counts As IDictionary(Of String, AtomicLong) = New Dictionary(Of String, AtomicLong)()
			For Each token As String In tokens
				If Not counts.ContainsKey(token) Then
					counts(token) = New AtomicLong(0)
				End If

				counts(token).incrementAndGet()
			Next token

			For i As Integer = 0 To tokens.Count - 1
				Dim idx As Integer = vocabCache.indexOf(tokens(i))
				If idx >= 0 Then
					Dim tf_idf As Double = tfidfWord(tokens(i), counts(tokens(i)).longValue(), tokens.Count)
					'log.info("TF-IDF for word: {} -> {} / {} => {}", tokens.get(i), counts.get(tokens.get(i)).longValue(), tokens.size(), tf_idf);
					ret.putScalar(idx, tf_idf)
				End If
			Next i
			Return ret
		End Function

		Public Overridable Function tfidfWord(ByVal word As String, ByVal wordCount As Long, ByVal documentLength As Long) As Double
			'log.info("word: {}; TF: {}; IDF: {}", word, tfForWord(wordCount, documentLength), idfForWord(word));
			Return MathUtils.tfidf(tfForWord(wordCount, documentLength), idfForWord(word))
		End Function

		Private Function tfForWord(ByVal wordCount As Long, ByVal documentLength As Long) As Double
			Return CDbl(wordCount) / CDbl(documentLength)
		End Function

		Private Function idfForWord(ByVal word As String) As Double
			Return MathUtils.idf(vocabCache.totalNumberOfDocs(), vocabCache.docAppearedIn(word))
		End Function


		''' <summary>
		''' Vectorizes the input source in to a dataset
		''' </summary>
		''' <returns> Adam Gibson </returns>
		Public Overrides Function vectorize() As DataSet
			Return Nothing
		End Function

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field tokenizerFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend tokenizerFactory_Conflict As TokenizerFactory
'JAVA TO VB CONVERTER NOTE: The field iterator was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend iterator_Conflict As LabelAwareIterator
'JAVA TO VB CONVERTER NOTE: The field minWordFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minWordFrequency_Conflict As Integer
			Protected Friend vocabCache As VocabCache(Of VocabWord)
			Protected Friend labelsSource As New LabelsSource()
'JAVA TO VB CONVERTER NOTE: The field stopWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend stopWords_Conflict As ICollection(Of String) = New List(Of String)()
			Protected Friend isParallel As Boolean = True

			Public Sub New()
			End Sub

			Public Overridable Function allowParallelTokenization(ByVal reallyAllow As Boolean) As Builder
				Me.isParallel = reallyAllow
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setTokenizerFactory(@NonNull TokenizerFactory tokenizerFactory)
			Public Overridable Function setTokenizerFactory(ByVal tokenizerFactory As TokenizerFactory) As Builder
				Me.tokenizerFactory_Conflict = tokenizerFactory
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setIterator(@NonNull LabelAwareIterator iterator)
			Public Overridable Function setIterator(ByVal iterator As LabelAwareIterator) As Builder
				Me.iterator_Conflict = New LabelAwareIteratorWrapper(iterator, labelsSource)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setIterator(@NonNull DocumentIterator iterator)
			Public Overridable Function setIterator(ByVal iterator As DocumentIterator) As Builder
				Me.iterator_Conflict = New DocumentIteratorConverter(iterator, labelsSource)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setIterator(@NonNull SentenceIterator iterator)
			Public Overridable Function setIterator(ByVal iterator As SentenceIterator) As Builder
				Me.iterator_Conflict = New SentenceIteratorConverter(iterator, labelsSource)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setVocab(@NonNull VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocab)
			Public Overridable Function setVocab(ByVal vocab As VocabCache(Of VocabWord)) As Builder
				Me.vocabCache = vocab
				Return Me
			End Function

			Public Overridable Function setMinWordFrequency(ByVal minWordFrequency As Integer) As Builder
				Me.minWordFrequency_Conflict = minWordFrequency
				Return Me
			End Function

			Public Overridable Function setStopWords(ByVal stopWords As ICollection(Of String)) As Builder
				Me.stopWords_Conflict = stopWords
				Return Me
			End Function

			Public Overridable Function build() As TfidfVectorizer
				Dim vectorizer As New TfidfVectorizer()

				vectorizer.tokenizerFactory = Me.tokenizerFactory_Conflict
				vectorizer.iterator = Me.iterator_Conflict
				vectorizer.minWordFrequency = Me.minWordFrequency_Conflict
				vectorizer.labelsSource_Conflict = Me.labelsSource
				vectorizer.isParallel = Me.isParallel

				If Me.vocabCache Is Nothing Then
					Me.vocabCache = (New AbstractCache.Builder(Of VocabWord)()).build()
				End If

				vectorizer.vocabCache = Me.vocabCache
				vectorizer.stopWords = Me.stopWords_Conflict

				Return vectorizer
			End Function

		End Class
	End Class

End Namespace