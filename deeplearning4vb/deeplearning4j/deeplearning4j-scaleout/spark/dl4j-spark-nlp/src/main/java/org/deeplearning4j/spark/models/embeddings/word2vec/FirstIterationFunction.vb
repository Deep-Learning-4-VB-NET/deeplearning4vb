Imports System
Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports Tuple2 = scala.Tuple2

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

Namespace org.deeplearning4j.spark.models.embeddings.word2vec


	Public Class FirstIterationFunction
		Implements FlatMapFunction(Of IEnumerator(Of Tuple2(Of IList(Of VocabWord), Long)), Entry(Of VocabWord, INDArray))

		Private ithIteration As Integer = 1
		Private vectorLength As Integer
		Private useAdaGrad As Boolean
		Private batchSize As Integer = 0
		Private negative As Double
		Private window As Integer
		Private alpha As Double
		Private minAlpha As Double
		Private totalWordCount As Long
		Private seed As Long
		Private maxExp As Integer
		Private expTable() As Double
		Private iterations As Integer
		Private indexSyn0VecMap As IDictionary(Of VocabWord, INDArray)
		Private pointSyn1VecMap As IDictionary(Of Integer, INDArray)
		Private nextRandom As New AtomicLong(5)

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.word2vec.VocabWord> vocab;
		Private vocab As VocabCache(Of VocabWord)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile NegativeHolder negativeHolder;
		Private negativeHolder As NegativeHolder
		Private cid As New AtomicLong(0)
		Private aff As New AtomicLong(0)



		Public Sub New(ByVal word2vecVarMapBroadcast As Broadcast(Of IDictionary(Of String, Object)), ByVal expTableBroadcast As Broadcast(Of Double()), ByVal vocabCacheBroadcast As Broadcast(Of VocabCache(Of VocabWord)))

			Dim word2vecVarMap As IDictionary(Of String, Object) = word2vecVarMapBroadcast.getValue()
			Me.expTable = expTableBroadcast.getValue()
			Me.vectorLength = DirectCast(word2vecVarMap("vectorLength"), Integer)
			Me.useAdaGrad = DirectCast(word2vecVarMap("useAdaGrad"), Boolean)
			Me.negative = DirectCast(word2vecVarMap("negative"), Double)
			Me.window = DirectCast(word2vecVarMap("window"), Integer)
			Me.alpha = DirectCast(word2vecVarMap("alpha"), Double)
			Me.minAlpha = DirectCast(word2vecVarMap("minAlpha"), Double)
			Me.totalWordCount = DirectCast(word2vecVarMap("totalWordCount"), Long)
			Me.seed = DirectCast(word2vecVarMap("seed"), Long)
			Me.maxExp = DirectCast(word2vecVarMap("maxExp"), Integer)
			Me.iterations = DirectCast(word2vecVarMap("iterations"), Integer)
			Me.batchSize = DirectCast(word2vecVarMap("batchSize"), Integer)
			Me.indexSyn0VecMap = New Dictionary(Of VocabWord, INDArray)()
			Me.pointSyn1VecMap = New Dictionary(Of Integer, INDArray)()
			Me.vocab = vocabCacheBroadcast.getValue()

			If Me.vocab Is Nothing Then
				Throw New Exception("VocabCache is null")
			End If

			If negative > 0 Then
				negativeHolder = NegativeHolder.Instance
				negativeHolder.initHolder(vocab, expTable, Me.vectorLength)
			End If
		End Sub



		Public Overrides Function [call](ByVal pairIter As IEnumerator(Of Tuple2(Of IList(Of VocabWord), Long))) As IEnumerator(Of KeyValuePair(Of VocabWord, INDArray))
			Do While pairIter.MoveNext()
				Dim batch As IList(Of Pair(Of IList(Of VocabWord), Long)) = New List(Of Pair(Of IList(Of VocabWord), Long))()
				Do While pairIter.MoveNext() AndAlso batch.Count < batchSize
					Dim pair As Tuple2(Of IList(Of VocabWord), Long) = pairIter.Current
					Dim vocabWordsList As IList(Of VocabWord) = pair._1()
					Dim sentenceCumSumCount As Long? = pair._2()
					batch.Add(Pair.of(vocabWordsList, sentenceCumSumCount))
				Loop

				For i As Integer = 0 To iterations - 1
					'System.out.println("Training sentence: " + vocabWordsList);
					For Each pair As Pair(Of IList(Of VocabWord), Long) In batch
						Dim vocabWordsList As IList(Of VocabWord) = pair.getKey()
						Dim sentenceCumSumCount As Long? = pair.getValue()
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
						Dim currentSentenceAlpha As Double = Math.Max(minAlpha, alpha - (alpha - minAlpha) * (sentenceCumSumCount.Value / CDbl(totalWordCount)))
						trainSentence(vocabWordsList, currentSentenceAlpha)
					Next pair
				Next i
			Loop
			Return indexSyn0VecMap.SetOfKeyValuePairs().GetEnumerator()
		End Function


		Public Overridable Sub trainSentence(ByVal vocabWordsList As IList(Of VocabWord), ByVal currentSentenceAlpha As Double)

			If vocabWordsList IsNot Nothing AndAlso vocabWordsList.Count > 0 Then
				Dim ithWordInSentence As Integer = 0
				Do While ithWordInSentence < vocabWordsList.Count
					' Random value ranging from 0 to window size
					nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))
					Dim b As Integer = CInt(CLng(Math.Truncate(Me.nextRandom.get()))) Mod window
					Dim currentWord As VocabWord = vocabWordsList(ithWordInSentence)
					If currentWord IsNot Nothing Then
						skipGram(ithWordInSentence, vocabWordsList, b, currentSentenceAlpha)
					End If
					ithWordInSentence += 1
				Loop
			End If
		End Sub

		Public Overridable Sub skipGram(ByVal ithWordInSentence As Integer, ByVal vocabWordsList As IList(Of VocabWord), ByVal b As Integer, ByVal currentSentenceAlpha As Double)

			Dim currentWord As VocabWord = vocabWordsList(ithWordInSentence)
			If currentWord IsNot Nothing AndAlso vocabWordsList.Count > 0 Then
				Dim [end] As Integer = window * 2 + 1 - b
				For a As Integer = b To [end] - 1
					If a <> window Then
						Dim c As Integer = ithWordInSentence - window + a
						If c >= 0 AndAlso c < vocabWordsList.Count Then
							Dim lastWord As VocabWord = vocabWordsList(c)
							iterateSample(currentWord, lastWord, currentSentenceAlpha)
						End If
					End If
				Next a
			End If
		End Sub

		Public Overridable Sub iterateSample(ByVal w1 As VocabWord, ByVal w2 As VocabWord, ByVal currentSentenceAlpha As Double)


			If w1 Is Nothing OrElse w2 Is Nothing OrElse w2.Index < 0 OrElse w2.Index = w1.Index Then
				Return
			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int currentWordIndex = w2.getIndex();
			Dim currentWordIndex As Integer = w2.Index

			' error for current word and context
			Dim neu1e As INDArray = Nd4j.create(vectorLength)

			' First iteration Syn0 is random numbers
			Dim l1 As INDArray = Nothing
			If indexSyn0VecMap.ContainsKey(vocab.elementAtIndex(currentWordIndex)) Then
				l1 = indexSyn0VecMap(vocab.elementAtIndex(currentWordIndex))
			Else
				l1 = getRandomSyn0Vec(vectorLength, CLng(currentWordIndex))
			End If

			'
			Dim i As Integer = 0
			Do While i < w1.getCodeLength()
				Dim code As Integer = w1.getCodes()(i)
				Dim point As Integer = w1.getPoints()(i)
				If point < 0 Then
					Throw New System.InvalidOperationException("Illegal point " & point)
				End If
				' Point to
				Dim syn1 As INDArray
				If pointSyn1VecMap.ContainsKey(point) Then
					syn1 = pointSyn1VecMap(point)
				Else
					syn1 = Nd4j.zeros(1, vectorLength) ' 1 row of vector length of zeros
					pointSyn1VecMap(point) = syn1
				End If

				' Dot product of Syn0 and Syn1 vecs
				Dim dot As Double = Nd4j.BlasWrapper.level1().dot(vectorLength, 1.0, l1, syn1)

				If dot < -maxExp OrElse dot >= maxExp Then
					i += 1
					Continue Do
				End If

				Dim idx As Integer = CInt(Math.Truncate((dot + maxExp) * (CDbl(expTable.Length) / maxExp / 2.0)))

				If idx >= expTable.Length Then
					i += 1
					Continue Do
				End If

				'score
				Dim f As Double = expTable(idx)
				'gradient
				Dim g As Double = (1 - code - f) * (If(useAdaGrad, w1.getGradient(i, currentSentenceAlpha, currentSentenceAlpha), currentSentenceAlpha))


				Nd4j.BlasWrapper.level1().axpy(vectorLength, g, syn1, neu1e)
				Nd4j.BlasWrapper.level1().axpy(vectorLength, g, l1, syn1)
				i += 1
			Loop

			Dim target As Integer = w1.Index
			Dim label As Integer
			'negative sampling
			If negative > 0 Then
				Dim d As Integer = 0
				Do While d < negative + 1
					If d = 0 Then
						label = 1
					Else
						nextRandom.set(Math.Abs(nextRandom.get() * 25214903917L + 11))

						Dim idx As Integer = Math.Abs(CInt(nextRandom.get() >> 16) Mod CInt(negativeHolder.getTable().length()))

						target = negativeHolder.getTable().getInt(idx)
						If target <= 0 Then
							target = CInt(Math.Truncate(nextRandom.get())) Mod (vocab.numWords() - 1) + 1
						End If

						If target = w1.Index Then
							d += 1
							Continue Do
						End If
						label = 0
					End If

					If target >= negativeHolder.getSyn1Neg().rows() OrElse target < 0 Then
						d += 1
						Continue Do
					End If

					Dim f As Double = Nd4j.BlasWrapper.dot(l1, negativeHolder.getSyn1Neg().slice(target))
					Dim g As Double
					If f > maxExp Then
						g = If(useAdaGrad, w1.getGradient(target, (label - 1), alpha), (label - 1) * alpha)
					ElseIf f < -maxExp Then
						g = label * (If(useAdaGrad, w1.getGradient(target, alpha, alpha), alpha))
					Else
						Dim idx As Integer = CInt(Math.Truncate((f + maxExp) * (expTable.Length \ maxExp \ 2)))
						If idx >= expTable.Length Then
							d += 1
							Continue Do
						End If

						g = If(useAdaGrad, w1.getGradient(target, label - expTable(idx), alpha), (label - expTable(idx)) * alpha)
					End If

					Nd4j.BlasWrapper.level1().axpy(vectorLength, g, negativeHolder.getSyn1Neg().slice(target), neu1e)

					Nd4j.BlasWrapper.level1().axpy(vectorLength, g, l1, negativeHolder.getSyn1Neg().slice(target))
					d += 1
				Loop
			End If


			' Updated the Syn0 vector based on gradient. Syn0 is not random anymore.
			Nd4j.BlasWrapper.level1().axpy(vectorLength, 1.0f, neu1e, l1)

			Dim word As VocabWord = vocab.elementAtIndex(currentWordIndex)
			indexSyn0VecMap(word) = l1
		End Sub

		Private Function getRandomSyn0Vec(ByVal vectorLength As Integer, ByVal lseed As Long) As INDArray
	'        
	'            we use wordIndex as part of seed here, to guarantee that during word syn0 initialization on dwo distinct nodes, initial weights will be the same for the same word
	'         
			Return Nd4j.rand(New Integer() {1, vectorLength}, lseed * seed).subi(0.5).divi(vectorLength)
		End Function
	End Class


End Namespace