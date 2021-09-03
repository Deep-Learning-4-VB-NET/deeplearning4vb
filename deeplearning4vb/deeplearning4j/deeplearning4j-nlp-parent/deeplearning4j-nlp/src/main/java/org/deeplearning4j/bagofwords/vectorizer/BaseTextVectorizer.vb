Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports org.deeplearning4j.models.sequencevectors.iterators
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.models.word2vec.wordstore.inmemory
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelsSource = org.deeplearning4j.text.documentiterator.LabelsSource
Imports org.deeplearning4j.text.invertedindex
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory

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


	<Serializable>
	Public MustInherit Class BaseTextVectorizer
		Implements TextVectorizer

		Public MustOverride Function vectorize() As org.nd4j.linalg.dataset.DataSet
		Public MustOverride ReadOnly Property Index As InvertedIndex(Of VocabWord) Implements TextVectorizer.getIndex
		Public MustOverride Function transform(ByVal tokens As IList(Of String)) As org.nd4j.linalg.api.ndarray.INDArray
		Public MustOverride Function transform(ByVal text As String) As org.nd4j.linalg.api.ndarray.INDArray Implements TextVectorizer.transform
		Public MustOverride Function vectorize(ByVal input As java.io.File, ByVal label As String) As org.nd4j.linalg.dataset.DataSet Implements TextVectorizer.vectorize
		Public MustOverride Function vectorize(ByVal text As String, ByVal label As String) As org.nd4j.linalg.dataset.DataSet Implements TextVectorizer.vectorize
		Public MustOverride Function vectorize(ByVal [is] As Stream, ByVal label As String) As org.nd4j.linalg.dataset.DataSet
		Public MustOverride ReadOnly Property VocabCache As VocabCache(Of VocabWord) Implements TextVectorizer.getVocabCache
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected transient org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory tokenizerFactory;
		<NonSerialized>
		Protected Friend tokenizerFactory As TokenizerFactory
		<NonSerialized>
		Protected Friend iterator As LabelAwareIterator
		Protected Friend minWordFrequency As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocabCache;
		Protected Friend vocabCache As VocabCache(Of VocabWord)
'JAVA TO VB CONVERTER NOTE: The field labelsSource was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend labelsSource_Conflict As LabelsSource
		Protected Friend stopWords As ICollection(Of String) = New List(Of String)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient org.deeplearning4j.text.invertedindex.InvertedIndex<org.deeplearning4j.models.word2vec.VocabWord> index;
		<NonSerialized>
		Protected Friend index As InvertedIndex(Of VocabWord)
		Protected Friend isParallel As Boolean = True

		Protected Friend Overridable ReadOnly Property LabelsSource As LabelsSource
			Get
				Return labelsSource_Conflict
			End Get
		End Property

		Public Overridable Sub buildVocab()
			If vocabCache Is Nothing Then
				vocabCache = (New AbstractCache.Builder(Of VocabWord)()).build()
			End If


			Dim transformer As SentenceTransformer = (New SentenceTransformer.Builder()).iterator(Me.iterator).tokenizerFactory(tokenizerFactory).build()

			Dim iterator As AbstractSequenceIterator(Of VocabWord) = (New AbstractSequenceIterator.Builder(Of VocabWord)(transformer)).build()

			Dim constructor As VocabConstructor(Of VocabWord) = (New VocabConstructor.Builder(Of VocabWord)()).addSource(iterator, minWordFrequency).setTargetVocabCache(vocabCache).setStopWords(stopWords).allowParallelTokenization(isParallel).build()

			constructor.buildJointVocabulary(False, True)
		End Sub

		Public Overridable Sub fit() Implements TextVectorizer.fit
			buildVocab()
		End Sub

		''' <summary>
		''' Returns the number of words encountered so far
		''' </summary>
		''' <returns> the number of words encountered so far </returns>
		Public Overridable Function numWordsEncountered() As Long Implements TextVectorizer.numWordsEncountered
			Return vocabCache.totalWordOccurrences()
		End Function
	End Class

End Namespace