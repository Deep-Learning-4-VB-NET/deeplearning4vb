Imports System.Collections.Generic
Imports System.IO
Imports Vectorizer = org.deeplearning4j.core.datasets.vectorizer.Vectorizer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.text.invertedindex
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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


	Public Interface TextVectorizer
		Inherits Vectorizer


		''' <summary>
		''' Sampling for building mini batches </summary>
		''' <returns> the sampling </returns>
		'double sample();

		''' <summary>
		''' For word vectors, this is the batch size for how to partition documents
		''' in to workloads </summary>
		''' <returns> the batchsize for partitioning documents in to workloads </returns>
		'int batchSize();

		''' <summary>
		''' The vocab sorted in descending order </summary>
		''' <returns> the vocab sorted in descending order </returns>
		ReadOnly Property VocabCache As VocabCache(Of VocabWord)


		''' <summary>
		''' Text coming from an input stream considered as one document </summary>
		''' <param name="is"> the input stream to read from </param>
		''' <param name="label"> the label to assign </param>
		''' <returns> a dataset with a applyTransformToDestination of weights(relative to impl; could be word counts or tfidf scores) </returns>
		Function vectorize(ByVal [is] As Stream, ByVal label As String) As DataSet

		''' <summary>
		''' Vectorizes the passed in text treating it as one document </summary>
		''' <param name="text"> the text to vectorize </param>
		''' <param name="label"> the label of the text </param>
		''' <returns> a dataset with a transform of weights(relative to impl; could be word counts or tfidf scores) </returns>
		Function vectorize(ByVal text As String, ByVal label As String) As DataSet

		''' <summary>
		''' Train the model
		''' </summary>
		Sub fit()

		''' 
		''' <param name="input"> the text to vectorize </param>
		''' <param name="label"> the label of the text </param>
		''' <returns> <seealso cref="DataSet"/> with a applyTransformToDestination of
		'''          weights(relative to impl; could be word counts or tfidf scores) </returns>
		Function vectorize(ByVal input As File, ByVal label As String) As DataSet


		''' <summary>
		''' Transforms the matrix </summary>
		''' <param name="text"> text to transform </param>
		''' <returns> <seealso cref="INDArray"/> </returns>
		Function transform(ByVal text As String) As INDArray

		''' <summary>
		''' Transforms the matrix </summary>
		''' <param name="tokens">
		''' @return </param>
		Function transform(ByVal tokens As IList(Of String)) As INDArray

		''' <summary>
		''' Returns the number of words encountered so far </summary>
		''' <returns> the number of words encountered so far </returns>
		Function numWordsEncountered() As Long

		''' <summary>
		''' Inverted index </summary>
		''' <returns> the inverted index for this vectorizer </returns>
		ReadOnly Property Index As InvertedIndex(Of VocabWord)
	End Interface

End Namespace