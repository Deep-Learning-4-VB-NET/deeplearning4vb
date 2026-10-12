Imports System.Collections.Generic
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.reader
Imports org.deeplearning4j.models.word2vec.wordstore
Imports EmbeddingInitializer = org.deeplearning4j.nn.weights.embeddings.EmbeddingInitializer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.models.embeddings.wordvectors


	Public Interface WordVectors
		Inherits EmbeddingInitializer

		Property UNK As String


		''' <summary>
		''' Returns true if the model has this word in the vocab </summary>
		''' <param name="word"> the word to test for </param>
		''' <returns> true if the model has the word in the vocab </returns>
		Function hasWord(ByVal word As String) As Boolean

		Function wordsNearest(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)

		Function wordsNearestSum(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)

		''' <summary>
		''' Get the top n words most similar to the given word </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="n"> the n to get </param>
		''' <returns> the top n words </returns>
		Function wordsNearestSum(ByVal word As String, ByVal n As Integer) As ICollection(Of String)


		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Function wordsNearestSum(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)

		''' <summary>
		''' Accuracy based on questions which are a space separated list of strings
		''' where the first word is the query word, the next 2 words are negative,
		''' and the last word is the predicted word to be nearest </summary>
		''' <param name="questions"> the questions to ask </param>
		''' <returns> the accuracy based on these questions </returns>
		Function accuracy(ByVal questions As IList(Of String)) As IDictionary(Of String, Double)

		Function indexOf(ByVal word As String) As Integer

		''' <summary>
		''' Find all words with a similar characters
		''' in the vocab </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="accuracy"> the accuracy: 0 to 1 </param>
		''' <returns> the list of words that are similar in the vocab </returns>
		Function similarWordsInVocabTo(ByVal word As String, ByVal accuracy As Double) As IList(Of String)

		''' <summary>
		''' Get the word vector for a given matrix </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the ndarray for this word </returns>
		Function getWordVector(ByVal word As String) As Double()

		''' <summary>
		''' Returns the word vector divided by the norm2 of the array </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the looked up matrix </returns>
		Function getWordVectorMatrixNormalized(ByVal word As String) As INDArray

		''' <summary>
		''' Get the word vector for a given matrix </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the ndarray for this word </returns>
		Function getWordVectorMatrix(ByVal word As String) As INDArray


		''' <summary>
		''' This method returns 2D array, where each row represents corresponding word/label
		''' </summary>
		''' <param name="labels">
		''' @return </param>
		Function getWordVectors(ByVal labels As ICollection(Of String)) As INDArray

		''' <summary>
		''' This method returns mean vector, built from words/labels passed in
		''' </summary>
		''' <param name="labels">
		''' @return </param>
		Function getWordVectorsMean(ByVal labels As ICollection(Of String)) As INDArray

		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Function wordsNearest(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)


		''' <summary>
		''' Get the top n words most similar to the given word </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="n"> the n to get </param>
		''' <returns> the top n words </returns>
		Function wordsNearest(ByVal word As String, ByVal n As Integer) As ICollection(Of String)



		''' <summary>
		''' Returns the similarity of 2 words </summary>
		''' <param name="word"> the first word </param>
		''' <param name="word2"> the second word </param>
		''' <returns> a normalized similarity (cosine similarity) </returns>
		Function similarity(ByVal word As String, ByVal word2 As String) As Double

		''' <summary>
		''' Vocab for the vectors
		''' @return
		''' </summary>
		Function vocab() As VocabCache

		''' <summary>
		''' Lookup table for the vectors
		''' @return
		''' </summary>
		Function lookupTable() As WeightLookupTable

		''' <summary>
		''' Specifies ModelUtils to be used to access model </summary>
		''' <param name="utils"> </param>
		WriteOnly Property ModelUtils As ModelUtils

	   ''' <summary>
	   ''' Does implementation vectorize words absent in vocabulary </summary>
	   ''' <returns> boolean </returns>
		Function outOfVocabularySupported() As Boolean

	End Interface

End Namespace