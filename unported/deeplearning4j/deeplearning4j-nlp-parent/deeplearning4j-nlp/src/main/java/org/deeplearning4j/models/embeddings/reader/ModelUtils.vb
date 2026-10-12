Imports System.Collections.Generic
Imports org.deeplearning4j.models.embeddings
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
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

Namespace org.deeplearning4j.models.embeddings.reader


	Public Interface ModelUtils(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)

		''' <summary>
		''' This method implementations should accept given lookup table, and use them in further calls to interface methods
		''' </summary>
		''' <param name="lookupTable"> </param>
		Sub init(ByVal lookupTable As WeightLookupTable(Of T))

		''' <summary>
		''' This method implementations should return distance between two given elements
		''' </summary>
		''' <param name="label1"> </param>
		''' <param name="label2">
		''' @return </param>
		Function similarity(ByVal label1 As String, ByVal label2 As String) As Double


		''' <summary>
		''' Accuracy based on questions which are a space separated list of strings
		''' where the first word is the query word, the next 2 words are negative,
		''' and the last word is the predicted word to be nearest </summary>
		''' <param name="questions"> the questions to ask </param>
		''' <returns> the accuracy based on these questions </returns>
		Function accuracy(ByVal questions As IList(Of String)) As IDictionary(Of String, Double)


		''' <summary>
		''' Find all words with a similar characters
		''' in the vocab </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="accuracy"> the accuracy: 0 to 1 </param>
		''' <returns> the list of words that are similar in the vocab </returns>
		Function similarWordsInVocabTo(ByVal word As String, ByVal accuracy As Double) As IList(Of String)


		''' <summary>
		''' This method implementations should return N nearest elements labels to given element's label
		''' </summary>
		''' <param name="label"> label to return nearest elements for </param>
		''' <param name="n"> number of nearest words to return
		''' @return </param>
		Function wordsNearest(ByVal label As String, ByVal n As Integer) As ICollection(Of String)

		''' <summary>
		''' Words nearest based on positive and negative words
		''' </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Function wordsNearest(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)


		''' <summary>
		''' Words nearest based on positive and negative words </summary>
		''' * <param name="top"> the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Function wordsNearest(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)


		Function wordsNearestSum(ByVal word As String, ByVal n As Integer) As ICollection(Of String)


		Function wordsNearestSum(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String)

		Function wordsNearestSum(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String)
	End Interface


End Namespace