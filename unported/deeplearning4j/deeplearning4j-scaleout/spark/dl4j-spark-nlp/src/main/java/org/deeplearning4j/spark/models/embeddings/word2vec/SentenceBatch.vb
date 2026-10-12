Imports System
Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports org.deeplearning4j.models.embeddings.inmemory
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.spark.models.embeddings.word2vec


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	<Obsolete>
	Public Class SentenceBatch
		Implements [Function](Of Word2VecFuncCall, Word2VecChange)

		Private nextRandom As New AtomicLong(5)
		'    private static Logger log = LoggerFactory.getLogger(SentenceBatch.class);


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Word2VecChange call(Word2VecFuncCall sentence) throws Exception
		Public Overrides Function [call](ByVal sentence As Word2VecFuncCall) As Word2VecChange
			Dim param As Word2VecParam = sentence.getParam().getValue()
			Dim changed As IList(Of Triple(Of Integer, Integer, Integer)) = New List(Of Triple(Of Integer, Integer, Integer))()
			Dim alpha As Double = Math.Max(param.MinAlpha, param.Alpha * (1 - (1.0 * sentence.getWordsSeen().Value / CDbl(param.TotalWords))))

			trainSentence(param, sentence.getSentence(), alpha, changed)
			Return New Word2VecChange(changed, param)
		End Function


		''' <summary>
		''' Train on a list of vocab words </summary>
		''' <param name="sentence"> the list of vocab words to train on </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void trainSentence(Word2VecParam param, final java.util.List<org.deeplearning4j.models.word2vec.VocabWord> sentence, double alpha, java.util.List<org.nd4j.common.primitives.Triple<Integer, Integer, Integer>> changed)
		Public Overridable Sub trainSentence(ByVal param As Word2VecParam, ByVal sentence As IList(Of VocabWord), ByVal alpha As Double, ByVal changed As IList(Of Triple(Of Integer, Integer, Integer)))
			If sentence IsNot Nothing AndAlso sentence.Count > 0 Then
				Dim i As Integer = 0
				Do While i < sentence.Count
					Dim vocabWord As VocabWord = sentence(i)
					If vocabWord IsNot Nothing AndAlso vocabWord.Word.EndsWith("STOP", StringComparison.Ordinal) Then
						nextRandom.set(nextRandom.get() * 25214903917L + 11)
						skipGram(param, i, sentence, CInt(Math.Truncate(nextRandom.get())) Mod param.Window, alpha, changed)
					End If
					i += 1
				Loop
			End If
		End Sub


		''' <summary>
		''' Train via skip gram </summary>
		''' <param name="i"> the current word </param>
		''' <param name="sentence"> the sentence to train on </param>
		''' <param name="b"> </param>
		''' <param name="alpha"> the learning rate </param>
		Public Overridable Sub skipGram(ByVal param As Word2VecParam, ByVal i As Integer, ByVal sentence As IList(Of VocabWord), ByVal b As Integer, ByVal alpha As Double, ByVal changed As IList(Of Triple(Of Integer, Integer, Integer)))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.models.word2vec.VocabWord word = sentence.get(i);
			Dim word As VocabWord = sentence(i)
			Dim window As Integer = param.Window
			If word IsNot Nothing AndAlso sentence.Count > 0 Then
				Dim [end] As Integer = window * 2 + 1 - b
				For a As Integer = b To [end] - 1
					If a <> window Then
						Dim c As Integer = i - window + a
						If c >= 0 AndAlso c < sentence.Count Then
							Dim lastWord As VocabWord = sentence(c)
							iterateSample(param, word, lastWord, alpha, changed)
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
		Public Overridable Sub iterateSample(ByVal param As Word2VecParam, ByVal w1 As VocabWord, ByVal w2 As VocabWord, ByVal alpha As Double, ByVal changed As IList(Of Triple(Of Integer, Integer, Integer)))
			If w2 Is Nothing OrElse w2.Index < 0 OrElse w1.Index = w2.Index OrElse w1.Word.Equals("STOP") OrElse w2.Word.Equals("STOP") OrElse w1.Word.Equals("UNK") OrElse w2.Word.Equals("UNK") Then
				Return
			End If
			Dim vectorLength As Integer = param.VectorLength
			Dim weights As InMemoryLookupTable = param.Weights
			Dim useAdaGrad As Boolean = param.UseAdaGrad
			Dim negative As Double = param.Negative
			Dim table As INDArray = param.Table
			Dim expTable() As Double = param.getExpTable().getValue()
			Dim MAX_EXP As Double = 6
			Dim numWords As Integer = param.NumWords
			'current word vector
			Dim l1 As INDArray = weights.vector(w2.Word)


			'error for current word and context
			Dim neu1e As INDArray = Nd4j.create(vectorLength)

			Dim i As Integer = 0
			Do While i < w1.getCodeLength()
				Dim code As Integer = w1.getCodes()(i)
				Dim point As Integer = w1.getPoints()(i)

				Dim syn1 As INDArray = weights.getSyn1().slice(point)

				Dim dot As Double = Nd4j.BlasWrapper.level1().dot(syn1.length(), 1.0, l1, syn1)

				If dot < -MAX_EXP OrElse dot >= MAX_EXP Then
					i += 1
					Continue Do
				End If

				Dim idx As Integer = CInt(Math.Truncate((dot + MAX_EXP) * (CDbl(expTable.Length) / MAX_EXP / 2.0)))

				'score
				Dim f As Double = expTable(idx)
				'gradient
				Dim g As Double = (1 - code - f) * (If(useAdaGrad, w1.getGradient(i, alpha, alpha), alpha))


				Nd4j.BlasWrapper.level1().axpy(syn1.length(), g, syn1, neu1e)
				Nd4j.BlasWrapper.level1().axpy(syn1.length(), g, l1, syn1)


				changed.Add(New Triple(Of Integer, Integer, Integer)(point, w1.Index, -1))

				i += 1
			Loop


			changed.Add(New Triple(Of Integer, Integer, Integer)(w1.Index, w2.Index, -1))
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
						g = If(useAdaGrad, w1.getGradient(target, (label - 1), alpha), (label - 1) * alpha)
					ElseIf f < -MAX_EXP Then
						g = label * (If(useAdaGrad, w1.getGradient(target, alpha, alpha), alpha))
					Else
						g = If(useAdaGrad, w1.getGradient(target, label - expTable(CInt(Math.Truncate((f + MAX_EXP) * (expTable.Length / MAX_EXP / 2)))), alpha), (label - expTable(CInt(Math.Truncate((f + MAX_EXP) * (expTable.Length / MAX_EXP / 2))))) * alpha)
					End If
					Nd4j.BlasWrapper.level1().axpy(l1.length(), g, neu1e, l1)

					Nd4j.BlasWrapper.level1().axpy(l1.length(), g, syn1Neg, l1)

					changed.Add(New Triple(Of Integer, Integer, Integer)(-1, -1, label))

					d += 1
				Loop
			End If


			Nd4j.BlasWrapper.level1().axpy(l1.length(), 1.0f, neu1e, l1)


		End Sub

	End Class

End Namespace