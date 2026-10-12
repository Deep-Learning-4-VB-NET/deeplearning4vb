Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.models.embeddings
Imports org.deeplearning4j.models.embeddings.reader
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports org.deeplearning4j.models.word2vec.wordstore
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.linalg.compression
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.deeplearning4j.models.word2vec


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StaticWord2Vec implements org.deeplearning4j.models.embeddings.wordvectors.WordVectors
	<Serializable>
	Public Class StaticWord2Vec
		Implements WordVectors

		Private cacheWrtDevice As IList(Of IDictionary(Of Integer, INDArray)) = New List(Of IDictionary(Of Integer, INDArray))()
		Private storage As AbstractStorage(Of Integer)
		Private cachePerDevice As Long = 0L
		Private vocabCache As VocabCache(Of VocabWord)
'JAVA TO VB CONVERTER NOTE: The field unk was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private unk_Conflict As String = Nothing

		Private Sub New()

		End Sub

		Public Overridable Property UNK As String Implements WordVectors.getUNK
			Get
				Return unk_Conflict
			End Get
			Set(ByVal newUNK As String)
				Me.unk_Conflict = newUNK
			End Set
		End Property


		''' <summary>
		''' Init method validates configuration defined using
		''' </summary>
		Protected Friend Overridable Sub init()
			If storage.size() <> vocabCache.numWords() Then
				Throw New Exception("Number of words in Vocab isn't matching number of stored Vectors. vocab: [" & vocabCache.numWords() & "]; storage: [" & storage.size() & "]")
			End If

			' initializing device cache
			Dim i As Integer = 0
			Do While i < Nd4j.AffinityManager.NumberOfDevices
				cacheWrtDevice.Add(New ConcurrentDictionary(Of Integer, INDArray)())
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Returns true if the model has this word in the vocab
		''' </summary>
		''' <param name="word"> the word to test for </param>
		''' <returns> true if the model has the word in the vocab </returns>
		Public Overridable Function hasWord(ByVal word As String) As Boolean Implements WordVectors.hasWord
			Return vocabCache.containsWord(word)
		End Function

		Public Overridable Function wordsNearest(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String) Implements WordVectors.wordsNearest
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		Public Overridable Function wordsNearestSum(ByVal words As INDArray, ByVal top As Integer) As ICollection(Of String) Implements WordVectors.wordsNearestSum
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		''' <summary>
		''' Get the top n words most similar to the given word
		''' PLEASE NOTE: This method is not available in this implementation.
		''' </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="n">    the n to get </param>
		''' <returns> the top n words </returns>
		Public Overridable Function wordsNearestSum(ByVal word As String, ByVal n As Integer) As ICollection(Of String) Implements WordVectors.wordsNearestSum
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		''' <summary>
		''' Words nearest based on positive and negative words
		''' PLEASE NOTE: This method is not available in this implementation.
		''' </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top">      the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearestSum(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String) Implements WordVectors.wordsNearestSum
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		''' <summary>
		''' Accuracy based on questions which are a space separated list of strings
		''' where the first word is the query word, the next 2 words are negative,
		''' and the last word is the predicted word to be nearest
		''' PLEASE NOTE: This method is not available in this implementation.
		''' </summary>
		''' <param name="questions"> the questions to ask </param>
		''' <returns> the accuracy based on these questions </returns>
		Public Overridable Function accuracy(ByVal questions As IList(Of String)) As IDictionary(Of String, Double) Implements WordVectors.accuracy
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		Public Overridable Function indexOf(ByVal word As String) As Integer Implements WordVectors.indexOf
			Return vocabCache.indexOf(word)
		End Function

		''' <summary>
		''' Find all words with a similar characters
		''' in the vocab
		''' PLEASE NOTE: This method is not available in this implementation.
		''' </summary>
		''' <param name="word">     the word to compare </param>
		''' <param name="accuracy"> the accuracy: 0 to 1 </param>
		''' <returns> the list of words that are similar in the vocab </returns>
		Public Overridable Function similarWordsInVocabTo(ByVal word As String, ByVal accuracy As Double) As IList(Of String) Implements WordVectors.similarWordsInVocabTo
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		''' <summary>
		''' Get the word vector for a given matrix
		''' </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the ndarray for this word </returns>
		Public Overridable Function getWordVector(ByVal word As String) As Double() Implements WordVectors.getWordVector
			Return getWordVectorMatrix(word).data().asDouble()
		End Function

		''' <summary>
		''' Returns the word vector divided by the norm2 of the array
		''' </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the looked up matrix </returns>
		Public Overridable Function getWordVectorMatrixNormalized(ByVal word As String) As INDArray Implements WordVectors.getWordVectorMatrixNormalized
			Return Transforms.unitVec(getWordVectorMatrix(word))
		End Function

		''' <summary>
		''' Get the word vector for a given matrix
		''' </summary>
		''' <param name="word"> the word to get the matrix for </param>
		''' <returns> the ndarray for this word </returns>
		Public Overridable Function getWordVectorMatrix(ByVal word As String) As INDArray Implements WordVectors.getWordVectorMatrix
			' TODO: add variable UNK here
			Dim idx As Integer = 0
			If hasWord(word) Then
				idx = vocabCache.indexOf(word)
			ElseIf UNK IsNot Nothing Then
				idx = vocabCache.indexOf(UNK)
			Else
				Return Nothing
			End If

			Dim deviceId As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Dim array As INDArray = Nothing

			If cachePerDevice > 0 AndAlso cacheWrtDevice(deviceId).ContainsKey(idx) Then
				Return cacheWrtDevice(Nd4j.AffinityManager.getDeviceForCurrentThread())(idx)
			End If

			array = storage.get(idx)

			If cachePerDevice > 0 Then
				' TODO: add cache here
				Dim arrayBytes As Long = array.length() * array.data().ElementSize
				If (arrayBytes * cacheWrtDevice(deviceId).Count) + arrayBytes < cachePerDevice Then
					cacheWrtDevice(deviceId)(idx) = array
				End If
			End If

			Return array
		End Function

		''' <summary>
		''' This method returns 2D array, where each row represents corresponding word/label
		''' </summary>
		''' <param name="labels">
		''' @return </param>
		Public Overridable Function getWordVectors(ByVal labels As ICollection(Of String)) As INDArray Implements WordVectors.getWordVectors
			Dim words As IList(Of INDArray) = New List(Of INDArray)()
			For Each label As String In labels
				If hasWord(label) OrElse UNK IsNot Nothing Then
					words.Add(getWordVectorMatrix(label))
				End If
			Next label

			Return Nd4j.vstack(words)
		End Function

		''' <summary>
		''' This method returns mean vector, built from words/labels passed in
		''' </summary>
		''' <param name="labels">
		''' @return </param>
		Public Overridable Function getWordVectorsMean(ByVal labels As ICollection(Of String)) As INDArray Implements WordVectors.getWordVectorsMean
			Dim matrix As INDArray = getWordVectors(labels)

			' TODO: check this (1)
			Return matrix.mean(1)
		End Function

		''' <summary>
		''' Words nearest based on positive and negative words
		''' PLEASE NOTE: This method is not available in this implementation.
		''' </summary>
		''' <param name="positive"> the positive words </param>
		''' <param name="negative"> the negative words </param>
		''' <param name="top">      the top n words </param>
		''' <returns> the words nearest the mean of the words </returns>
		Public Overridable Function wordsNearest(ByVal positive As ICollection(Of String), ByVal negative As ICollection(Of String), ByVal top As Integer) As ICollection(Of String) Implements WordVectors.wordsNearest
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		''' <summary>
		''' Get the top n words most similar to the given word
		''' PLEASE NOTE: This method is not available in this implementation.
		''' </summary>
		''' <param name="word"> the word to compare </param>
		''' <param name="n">    the n to get </param>
		''' <returns> the top n words </returns>
		Public Overridable Function wordsNearest(ByVal word As String, ByVal n As Integer) As ICollection(Of String) Implements WordVectors.wordsNearest
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		''' <summary>
		''' Returns the similarity of 2 words
		''' </summary>
		''' <param name="label1">  the first word </param>
		''' <param name="label2"> the second word </param>
		''' <returns> a normalized similarity (cosine similarity) </returns>
		Public Overridable Function similarity(ByVal label1 As String, ByVal label2 As String) As Double Implements WordVectors.similarity
			If label1 Is Nothing OrElse label2 Is Nothing Then
				log.debug("LABELS: " & label1 & ": " & (If(label1 Is Nothing, "null", "exists")) & ";" & label2 & " vec2:" & (If(label2 Is Nothing, "null", "exists")))
				Return Double.NaN
			End If

			Dim vec1 As INDArray = getWordVectorMatrix(label1).dup()
			Dim vec2 As INDArray = getWordVectorMatrix(label2).dup()

			If vec1 Is Nothing OrElse vec2 Is Nothing Then
				log.debug(label1 & ": " & (If(vec1 Is Nothing, "null", "exists")) & ";" & label2 & " vec2:" & (If(vec2 Is Nothing, "null", "exists")))
				Return Double.NaN
			End If

			If label1.Equals(label2) Then
				Return 1.0
			End If

			vec1 = Transforms.unitVec(vec1)
			vec2 = Transforms.unitVec(vec2)

			Return Transforms.cosineSim(vec1, vec2)
		End Function

		''' <summary>
		''' Vocab for the vectors
		''' 
		''' @return
		''' </summary>
		Public Overridable Function vocab() As VocabCache Implements WordVectors.vocab
			Return vocabCache
		End Function

		''' <summary>
		''' Lookup table for the vectors
		''' PLEASE NOTE: This method is not available in this implementation.
		''' 
		''' @return
		''' </summary>
		Public Overridable Function lookupTable() As WeightLookupTable Implements WordVectors.lookupTable
			Throw New System.NotSupportedException("Method isn't implemented. Please use usual Word2Vec implementation")
		End Function

		''' <summary>
		''' Specifies ModelUtils to be used to access model
		''' PLEASE NOTE: This method has no effect in this implementation.
		''' </summary>
		''' <param name="utils"> </param>
		Public Overridable WriteOnly Property ModelUtils Implements WordVectors.setModelUtils As ModelUtils
			Set(ByVal utils As ModelUtils)
				' no-op
			End Set
		End Property

		Public Overridable Sub loadWeightsInto(ByVal array As INDArray)
			Dim n As Integer = CInt(vocabSize())
			Dim zero As INDArray = Nothing
			For i As Integer = 0 To n - 1
				Dim arr As INDArray = storage.get(i)
				If arr Is Nothing Then 'TODO is this even possible?
					If zero Is Nothing Then
						zero = Nd4j.create(array.dataType(), 1, array.size(1))
					End If
					arr = zero
				End If
				array.putRow(i, arr)
			Next i
		End Sub

		Public Overridable Function vocabSize() As Long
			Return storage.size()
		End Function

		Public Overridable Function vectorSize() As Integer
			Dim arr As INDArray = storage.get(0)
			If arr IsNot Nothing Then
				Return CInt(arr.length())
			End If

			Dim vs As Integer = CInt(vocabSize())
			For i As Integer = 1 To vs - 1
				arr = storage.get(0)
				If arr IsNot Nothing Then
					Return CInt(arr.length())
				End If
			Next i
			Throw New System.NotSupportedException("No vectors found")
		End Function

		Public Overridable Function jsonSerializable() As Boolean
			Return False
		End Function

		Public Overridable Function outOfVocabularySupported() As Boolean Implements WordVectors.outOfVocabularySupported
			Return False
		End Function

		Public Class Builder

			Friend storage As AbstractStorage(Of Integer)
'JAVA TO VB CONVERTER NOTE: The field cachePerDevice was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend cachePerDevice_Conflict As Long = 0L
			Friend vocabCache As VocabCache(Of VocabWord)

			''' 
			''' <param name="storage"> AbstractStorage implementation, key has to be Integer, index of vocabWords </param>
			''' <param name="vocabCache"> VocabCache implementation, which will be used to lookup word indexes </param>
			Public Sub New(ByVal storage As AbstractStorage(Of Integer), ByVal vocabCache As VocabCache(Of VocabWord))
				Me.storage = storage
				Me.vocabCache = vocabCache
			End Sub


			''' <summary>
			''' This method lets you to define if decompressed values will be cached, to avoid excessive decompressions.
			''' If bytes == 0 - no cache will be used.
			''' </summary>
			''' <param name="bytes">
			''' @return </param>
			Public Overridable Function setCachePerDevice(ByVal bytes As Long) As Builder
				Me.cachePerDevice_Conflict = bytes
				Return Me
			End Function


			''' <summary>
			''' This method returns Static Word2Vec implementation, which is suitable for tasks like neural nets feeding.
			''' 
			''' @return
			''' </summary>
			Public Overridable Function build() As StaticWord2Vec
'JAVA TO VB CONVERTER NOTE: The variable word2Vec was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim word2Vec_Conflict As New StaticWord2Vec()
				word2Vec_Conflict.cachePerDevice = Me.cachePerDevice_Conflict
				word2Vec_Conflict.storage = Me.storage
				word2Vec_Conflict.vocabCache = Me.vocabCache

				word2Vec_Conflict.init()

				Return word2Vec_Conflict
			End Function
		End Class
	End Class

End Namespace