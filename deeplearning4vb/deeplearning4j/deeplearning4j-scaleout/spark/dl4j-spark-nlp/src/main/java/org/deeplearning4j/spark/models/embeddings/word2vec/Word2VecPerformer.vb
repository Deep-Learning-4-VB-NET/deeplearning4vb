Imports System
Imports System.Collections.Generic
Imports System.IO
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports SparkConf = org.apache.spark.SparkConf
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports org.deeplearning4j.models.embeddings.inmemory
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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


	<Obsolete>
	Public Class Word2VecPerformer
		Implements VoidFunction(Of Pair(Of IList(Of VocabWord), AtomicLong))

		Private Shared MAX_EXP As Double = 6
		Private useAdaGrad As Boolean = False
		Private negative As Double = 5
		Private numWords As Integer = 1
		Private table As INDArray
		Private window As Integer = 5
		Private nextRandom As New AtomicLong(5)
		Private alpha As Double = 0.025
		Private minAlpha As Double = 1e-2
		Private totalWords As Integer = 1
		Private transient As static
		Private lastChecked As Integer = 0
		Private wordCount As Broadcast(Of AtomicLong)
		Private weights As InMemoryLookupTable
		Private expTable(999) As Double
		Private vectorLength As Integer


		Public Sub New(ByVal sc As SparkConf, ByVal wordCount As Broadcast(Of AtomicLong), ByVal weights As InMemoryLookupTable)
			Me.weights = weights
			Me.wordCount = wordCount
			setup(sc)
		End Sub

		Public Overridable Sub setup(ByVal conf As SparkConf)
			useAdaGrad = conf.getBoolean(Word2VecVariables.ADAGRAD, False)
			negative = conf.getDouble(Word2VecVariables.NEGATIVE, 5)
			numWords = conf.getInt(Word2VecVariables.NUM_WORDS, 1)
			window = conf.getInt(Word2VecVariables.WINDOW, 5)
			alpha = conf.getDouble(Word2VecVariables.ALPHA, 0.025f)
			minAlpha = conf.getDouble(Word2VecVariables.MIN_ALPHA, 1e-2f)
			totalWords = conf.getInt(Word2VecVariables.NUM_WORDS, 1)
			vectorLength = conf.getInt(Word2VecVariables.VECTOR_LENGTH, 100)
			initExpTable()

			If negative > 0 AndAlso conf.contains(Word2VecVariables.TABLE) Then
				Dim bis As New MemoryStream(conf.get(Word2VecVariables.TABLE).getBytes())
				Dim dis As New DataInputStream(bis)
				table = Nd4j.read(dis)
			End If

		End Sub



		''' <summary>
		''' Train on a list of vocab words </summary>
		''' <param name="sentence"> the list of vocab words to train on </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void trainSentence(final java.util.List<org.deeplearning4j.models.word2vec.VocabWord> sentence, double alpha)
		Public Overridable Sub trainSentence(ByVal sentence As IList(Of VocabWord), ByVal alpha As Double)
			If sentence IsNot Nothing AndAlso sentence.Count > 0 Then
				Dim i As Integer = 0
				Do While i < sentence.Count
					If Not sentence(i).getWord().EndsWith("STOP") Then
						nextRandom.set(nextRandom.get() * 25214903917L + 11)
						skipGram(i, sentence, CInt(Math.Truncate(nextRandom.get())) Mod window, alpha)
					End If
					i += 1
				Loop
			End If

		End Sub


		''' <summary>
		''' Train via skip gram </summary>
		''' <param name="i"> </param>
		''' <param name="sentence"> </param>
		Public Overridable Sub skipGram(ByVal i As Integer, ByVal sentence As IList(Of VocabWord), ByVal b As Integer, ByVal alpha As Double)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.models.word2vec.VocabWord word = sentence.get(i);
			Dim word As VocabWord = sentence(i)
			If word IsNot Nothing AndAlso sentence.Count > 0 Then
				Dim [end] As Integer = window * 2 + 1 - b
				For a As Integer = b To [end] - 1
					If a <> window Then
						Dim c As Integer = i - window + a
						If c >= 0 AndAlso c < sentence.Count Then
							Dim lastWord As VocabWord = sentence(c)
							iterateSample(word, lastWord, alpha)
						End If
					End If
				Next a
			End If
		End Sub



		''' <summary>
		''' Iterate on the given 2 vocab words
		''' </summary>
		''' <param name="w1"> the first word to iterate on </param>
		''' <param name="w2"> the second word to iterate on </param>
		Public Overridable Sub iterateSample(ByVal w1 As VocabWord, ByVal w2 As VocabWord, ByVal alpha As Double)
			If w2 Is Nothing OrElse w2.Index < 0 Then
				Return
			End If

			'current word vector
			Dim l1 As INDArray = weights.vector(w2.Word)


			'error for current word and context
			Dim neu1e As INDArray = Nd4j.create(vectorLength)

			Dim i As Integer = 0
			Do While i < w1.getCodeLength()
				Dim code As Integer = w1.getCodes()(i)
				Dim point As Integer = w1.getPoints()(i)

				Dim syn1 As INDArray = weights.getSyn1().slice(point)

				Dim dot As Double = Nd4j.BlasWrapper.dot(l1, syn1)

				If dot >= -MAX_EXP AndAlso dot < MAX_EXP Then

					Dim idx As Integer = CInt(Math.Truncate((dot + MAX_EXP) * (CDbl(expTable.Length) / MAX_EXP / 2.0)))
					If idx >= expTable.Length Then
						i += 1
						Continue Do
					End If

					'score
					Dim f As Double = expTable(idx)
					'gradient
					Dim g As Double = (1 - code - f) * (If(useAdaGrad, w1.getGradient(i, alpha, Me.alpha), alpha))

					Nd4j.BlasWrapper.level1().axpy(l1.length(), g, syn1, neu1e)
					Nd4j.BlasWrapper.level1().axpy(l1.length(), g, l1, syn1)
				End If


				i += 1
			Loop


			'negative sampling
			If negative > 0 Then
				Dim target As Integer = w1.Index
				Dim label As Integer
				Dim syn1Neg As INDArray = weights.getSyn1Neg().slice(target)

				Dim d As Integer = 0
				Do While d < negative + 1
					If d = 0 Then

						label = 1
					Else
						nextRandom.set(nextRandom.get() * 25214903917L + 11)

						target = table.getInt(CInt(nextRandom.get() >> 16) Mod CInt(table.length()))
						If target = 0 Then
							target = CInt(Math.Truncate(nextRandom.get())) Mod (numWords - 1) + 1
						End If
						If target = w1.Index Then
							d += 1
							Continue Do
						End If
						label = 0
					End If

					Dim f As Double = Nd4j.BlasWrapper.dot(l1, syn1Neg)
					Dim g As Double
					If f > MAX_EXP Then
						g = If(useAdaGrad, w1.getGradient(target, (label - 1), Me.alpha), (label - 1) * alpha)
					ElseIf f < -MAX_EXP Then
						g = label * (If(useAdaGrad, w1.getGradient(target, alpha, Me.alpha), alpha))
					Else
						g = If(useAdaGrad, w1.getGradient(target, label - expTable(CInt(Math.Truncate((f + MAX_EXP) * (expTable.Length / MAX_EXP / 2)))), Me.alpha), (label - expTable(CInt(Math.Truncate((f + MAX_EXP) * (expTable.Length / MAX_EXP / 2))))) * alpha)
					End If
					If syn1Neg.data().dataType() = DataType.DOUBLE Then
						Nd4j.BlasWrapper.axpy(g, neu1e, l1)
					Else
						Nd4j.BlasWrapper.axpy(CSng(g), neu1e, l1)
					End If

					If syn1Neg.data().dataType() = DataType.DOUBLE Then
						Nd4j.BlasWrapper.axpy(g, syn1Neg, l1)
					Else
						Nd4j.BlasWrapper.axpy(CSng(g), syn1Neg, l1)
					End If
					d += 1
				Loop
			End If

			If neu1e.data().dataType() = DataType.DOUBLE Then
				Nd4j.BlasWrapper.axpy(1.0, neu1e, l1)

			Else
				Nd4j.BlasWrapper.axpy(1.0f, neu1e, l1)
			End If

		End Sub

		Private Sub initExpTable()
			For i As Integer = 0 To expTable.Length - 1
				Dim tmp As Double = FastMath.exp((i / CDbl(expTable.Length) * 2 - 1) * MAX_EXP)
				expTable(i) = tmp / (tmp + 1.0)
			Next i
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(org.nd4j.common.primitives.Pair<java.util.List<org.deeplearning4j.models.word2vec.VocabWord>, java.util.concurrent.atomic.AtomicLong> pair) throws Exception
		Public Overrides Sub [call](ByVal pair As Pair(Of IList(Of VocabWord), AtomicLong))
			Dim numWordsSoFar As Double = wordCount.getValue().doubleValue()

			Dim sentence As IList(Of VocabWord) = pair.First
			Dim alpha2 As Double = Math.Max(minAlpha, alpha * (1 - (1.0 * numWordsSoFar / CDbl(totalWords))))
			Dim totalNewWords As Integer = 0
			trainSentence(sentence, alpha2)
			totalNewWords += sentence.Count



			Dim newWords As Double = totalNewWords + numWordsSoFar
			Dim diff As Double = Math.Abs(newWords - lastChecked)
			If diff >= 10000 Then
				lastChecked = CInt(Math.Truncate(newWords))
				log.info("Words so far " & newWords & " out of " & totalWords)
			End If

			pair.Second.getAndAdd(CLng(totalNewWords))
		End Sub


	End Class

End Namespace