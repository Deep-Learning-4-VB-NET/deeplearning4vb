Imports System.Collections.Generic
Imports [Function] = org.nd4j.shade.guava.base.Function
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
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

Namespace org.deeplearning4j.text.invertedindex


	Public Interface InvertedIndex(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)


		''' <summary>
		''' Iterate over batches </summary>
		''' <returns> the batch size </returns>
		Function batchIter(ByVal batchSize As Integer) As IEnumerator(Of IList(Of IList(Of T)))

		''' <summary>
		''' Iterate over documents
		''' @return
		''' </summary>
		Function docs() As IEnumerator(Of IList(Of T))

		''' <summary>
		''' Unlock the index
		''' </summary>
		Sub unlock()

		''' <summary>
		''' Cleanup any resources used
		''' </summary>
		Sub cleanup()

		''' <summary>
		''' Sampling for creating mini batches </summary>
		''' <returns> the sampling for mini batches </returns>
		Function sample() As Double

		''' <summary>
		''' Iterates over mini batches </summary>
		''' <returns> the mini batches created by this vectorizer </returns>
		Function miniBatches() As IEnumerator(Of IList(Of T))

		''' <summary>
		''' Returns a list of words for a document </summary>
		''' <param name="index">
		''' @return </param>
		Function document(ByVal index As Integer) As IList(Of T)

		''' <summary>
		''' Returns a list of words for a document
		''' and the associated label </summary>
		''' <param name="index">
		''' @return </param>
		Function documentWithLabel(ByVal index As Integer) As Pair(Of IList(Of T), String)

		''' <summary>
		''' Returns a list of words associated with the document
		''' and the associated labels </summary>
		''' <param name="index">
		''' @return </param>
		Function documentWithLabels(ByVal index As Integer) As Pair(Of IList(Of T), ICollection(Of String))

		''' <summary>
		''' Returns the list of documents a vocab word is in </summary>
		''' <param name="vocabWord"> the vocab word to get documents for </param>
		''' <returns> the documents for a vocab word </returns>
		Function documents(ByVal vocabWord As T) As Integer()

		''' <summary>
		''' Returns the number of documents
		''' @return
		''' </summary>
		Function numDocuments() As Integer

		''' <summary>
		''' Returns a list of all documents </summary>
		''' <returns> the list of all documents </returns>
		Function allDocs() As Integer()



		''' <summary>
		''' Add word to a document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="word"> the word to add </param>
		Sub addWordToDoc(ByVal doc As Integer, ByVal word As T)


		''' <summary>
		''' Adds words to the given document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="words"> the words to add </param>
		Sub addWordsToDoc(ByVal doc As Integer, ByVal words As IList(Of T))



		''' <summary>
		''' Add word to a document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="word"> the word to add </param>
		Sub addLabelForDoc(ByVal doc As Integer, ByVal word As T)


		''' <summary>
		''' Adds words to the given document </summary>
		''' <param name="doc"> the document to add to
		'''  </param>
		Sub addLabelForDoc(ByVal doc As Integer, ByVal label As String)



		''' <summary>
		''' Adds words to the given document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="words"> the words to add </param>
		''' <param name="label"> the label for the document </param>
		Sub addWordsToDoc(ByVal doc As Integer, ByVal words As IList(Of T), ByVal label As String)


		''' <summary>
		''' Adds words to the given document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="words"> the words to add </param>
		''' <param name="label"> the label for the document </param>
		Sub addWordsToDoc(ByVal doc As Integer, ByVal words As IList(Of T), ByVal label As T)



		''' <summary>
		''' Add word to a document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="word"> the word to add </param>
		Sub addLabelsForDoc(ByVal doc As Integer, ByVal word As IList(Of T))


		''' <summary>
		''' Adds words to the given document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="label"> the labels to add
		'''  </param>
		Sub addLabelsForDoc(ByVal doc As Integer, ByVal label As ICollection(Of String))



		''' <summary>
		''' Adds words to the given document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="words"> the words to add </param>
		''' <param name="label"> the label for the document </param>
		Sub addWordsToDoc(ByVal doc As Integer, ByVal words As IList(Of T), ByVal label As ICollection(Of String))


		''' <summary>
		''' Adds words to the given document </summary>
		''' <param name="doc"> the document to add to </param>
		''' <param name="words"> the words to add </param>
		''' <param name="label"> the label for the document </param>
		Sub addWordsToDocVocabWord(ByVal doc As Integer, ByVal words As IList(Of T), ByVal label As ICollection(Of T))



		''' <summary>
		''' Finishes saving data
		''' </summary>
		Sub finish()

		''' <summary>
		''' Total number of words in the index </summary>
		''' <returns> the total number of words in the index </returns>
		Function totalWords() As Long

		''' <summary>
		''' For word vectors, this is the batch size for which to train on </summary>
		''' <returns> the batch size for which to train on </returns>
		Function batchSize() As Integer

		''' <summary>
		''' Iterate over each document with a label </summary>
		''' <param name="func"> the function to apply </param>
		''' <param name="exec"> executor service for execution </param>
		Sub eachDocWithLabels(ByVal func As [Function](Of Pair(Of IList(Of T), ICollection(Of String)), Void), ByVal exec As Executor)


		''' <summary>
		''' Iterate over each document with a label </summary>
		''' <param name="func"> the function to apply </param>
		''' <param name="exec"> executor service for execution </param>
		Sub eachDocWithLabel(ByVal func As [Function](Of Pair(Of IList(Of T), String), Void), ByVal exec As Executor)

		''' <summary>
		''' Iterate over each document </summary>
		''' <param name="func"> the function to apply </param>
		''' <param name="exec"> executor service for execution </param>
		Sub eachDoc(ByVal func As [Function](Of IList(Of T), Void), ByVal exec As Executor)
	End Interface

End Namespace