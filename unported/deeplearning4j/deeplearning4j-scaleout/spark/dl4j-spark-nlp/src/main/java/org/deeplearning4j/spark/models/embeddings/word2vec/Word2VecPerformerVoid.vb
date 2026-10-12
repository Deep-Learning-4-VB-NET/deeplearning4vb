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
	Public Class Word2VecPerformerVoid
		Implements VoidFunction(Of Pair(Of IList(Of VocabWord), AtomicLong))


		Public Const NAME_SPACE As String = "org.deeplearning4j.scaleout.perform.models.word2vec"
		Public Shared ReadOnly VECTOR_LENGTH As String = NAME_SPACE & ".length"
		Public Shared ReadOnly ADAGRAD As String = NAME_SPACE & ".adagrad"
'JAVA TO VB CONVERTER NOTE: The field NEGATIVE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly NEGATIVE_Conflict As String = NAME_SPACE & ".negative"
		Public Shared ReadOnly NUM_WORDS As String = NAME_SPACE & ".numwords"
'JAVA TO VB CONVERTER NOTE: The field TABLE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly TABLE_Conflict As String = NAME_SPACE & ".table"
'JAVA TO VB CONVERTER NOTE: The field WINDOW was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly WINDOW_Conflict As String = NAME_SPACE & ".window"
'JAVA TO VB CONVERTER NOTE: The field ALPHA was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly ALPHA_Conflict As String = NAME_SPACE & ".alpha"
		Public Shared ReadOnly MIN_ALPHA As String = NAME_SPACE & ".minalpha"
'JAVA TO VB CONVERTER NOTE: The field ITERATIONS was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly ITERATIONS_Conflict As String = NAME_SPACE & ".iterations"

'JAVA TO VB CONVERTER NOTE: The field MAX_EXP was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared MAX_EXP_Conflict As Double = 6
'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private useAdaGrad_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field negative was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private negative_Conflict As Double = 5
'JAVA TO VB CONVERTER NOTE: The field numWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numWords_Conflict As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field table was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private table_Conflict As INDArray
'JAVA TO VB CONVERTER NOTE: The field window was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private window_Conflict As Integer = 5
'JAVA TO VB CONVERTER NOTE: The field nextRandom was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private nextRandom_Conflict As New AtomicLong(5)
'JAVA TO VB CONVERTER NOTE: The field alpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private alpha_Conflict As Double = 0.025
'JAVA TO VB CONVERTER NOTE: The field minAlpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private minAlpha_Conflict As Double = 1e-2
'JAVA TO VB CONVERTER NOTE: The field totalWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private totalWords_Conflict As Integer = 1
'JAVA TO VB CONVERTER NOTE: The field iterations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private iterations_Conflict As Integer = 5
		Private transient As static
'JAVA TO VB CONVERTER NOTE: The field lastChecked was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastChecked_Conflict As Integer = 0
'JAVA TO VB CONVERTER NOTE: The field wordCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private wordCount_Conflict As Broadcast(Of AtomicLong)
'JAVA TO VB CONVERTER NOTE: The field weights was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private weights_Conflict As InMemoryLookupTable
'JAVA TO VB CONVERTER NOTE: The field expTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private expTable_Conflict(999) As Double
'JAVA TO VB CONVERTER NOTE: The field vectorLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private vectorLength_Conflict As Integer


		Public Sub New(ByVal sc As SparkConf, ByVal wordCount As Broadcast(Of AtomicLong), ByVal weights As InMemoryLookupTable)
			Me.weights_Conflict = weights
			Me.wordCount_Conflict = wordCount
			setup(sc)
		End Sub

		Public Overridable Sub setup(ByVal conf As SparkConf)
			useAdaGrad_Conflict = conf.getBoolean(ADAGRAD, False)
			negative_Conflict = conf.getDouble(NEGATIVE_Conflict, 5)
			numWords_Conflict = conf.getInt(NUM_WORDS, 1)
			window_Conflict = conf.getInt(WINDOW_Conflict, 5)
			alpha_Conflict = conf.getDouble(ALPHA_Conflict, 0.025f)
			minAlpha_Conflict = conf.getDouble(MIN_ALPHA, 1e-2f)
			totalWords_Conflict = conf.getInt(NUM_WORDS, 1)
			iterations_Conflict = conf.getInt(ITERATIONS_Conflict, 5)
			vectorLength_Conflict = conf.getInt(VECTOR_LENGTH, 100)

			initExpTable()

			If negative_Conflict > 0 AndAlso conf.contains(TABLE_Conflict) Then
				Dim bis As New MemoryStream(conf.get(TABLE_Conflict).getBytes())
				Dim dis As New DataInputStream(bis)
				table_Conflict = Nd4j.read(dis)
			End If
		End Sub


		Public Overridable Property VectorLength As Integer
			Get
				Return vectorLength_Conflict
			End Get
			Set(ByVal vectorLength As Integer)
				Me.vectorLength_Conflict = vectorLength
			End Set
		End Property


		Public Overridable Property ExpTable As Double()
			Get
				Return expTable_Conflict
			End Get
			Set(ByVal expTable() As Double)
				Me.expTable_Conflict = expTable
			End Set
		End Property


		Public Overridable Property Weights As InMemoryLookupTable
			Get
				Return weights_Conflict
			End Get
			Set(ByVal weights As InMemoryLookupTable)
				Me.weights_Conflict = weights
			End Set
		End Property


		Public Overridable Property WordCount As Broadcast(Of AtomicLong)
			Get
				Return wordCount_Conflict
			End Get
			Set(ByVal wordCount As Broadcast(Of AtomicLong))
				Me.wordCount_Conflict = wordCount
			End Set
		End Property


		Public Overridable Property LastChecked As Integer
			Get
				Return lastChecked_Conflict
			End Get
			Set(ByVal lastChecked As Integer)
				Me.lastChecked_Conflict = lastChecked
			End Set
		End Property


		Public Shared ReadOnly Property Log As Logger
			Get
				Return log
			End Get
		End Property

		Public Overridable Property Iterations As Integer
			Get
				Return iterations_Conflict
			End Get
			Set(ByVal iterations As Integer)
				Me.iterations_Conflict = iterations
			End Set
		End Property


		Public Overridable Property TotalWords As Integer
			Get
				Return totalWords_Conflict
			End Get
			Set(ByVal totalWords As Integer)
				Me.totalWords_Conflict = totalWords
			End Set
		End Property


		Public Overridable Property MinAlpha As Double
			Get
				Return minAlpha_Conflict
			End Get
			Set(ByVal minAlpha As Double)
				Me.minAlpha_Conflict = minAlpha
			End Set
		End Property


		Public Overridable Property Alpha As Double
			Get
				Return alpha_Conflict
			End Get
			Set(ByVal alpha As Double)
				Me.alpha_Conflict = alpha
			End Set
		End Property


		Public Overridable Property NextRandom As AtomicLong
			Get
				Return nextRandom_Conflict
			End Get
			Set(ByVal nextRandom As AtomicLong)
				Me.nextRandom_Conflict = nextRandom
			End Set
		End Property


		Public Overridable Property Window As Integer
			Get
				Return window_Conflict
			End Get
			Set(ByVal window As Integer)
				Me.window_Conflict = window
			End Set
		End Property


		Public Overridable Property Table As INDArray
			Get
				Return table_Conflict
			End Get
			Set(ByVal table As INDArray)
				Me.table_Conflict = table
			End Set
		End Property


		Public Overridable Property NumWords As Integer
			Get
				Return numWords_Conflict
			End Get
			Set(ByVal numWords As Integer)
				Me.numWords_Conflict = numWords
			End Set
		End Property


		Public Overridable Property Negative As Double
			Get
				Return negative_Conflict
			End Get
			Set(ByVal negative As Double)
				Me.negative_Conflict = negative
			End Set
		End Property


		Public Overridable Property UseAdaGrad As Boolean
			Get
				Return useAdaGrad_Conflict
			End Get
			Set(ByVal useAdaGrad As Boolean)
				Me.useAdaGrad_Conflict = useAdaGrad
			End Set
		End Property


		Public Shared Property MAX_EXP As Double
			Get
				Return MAX_EXP_Conflict
			End Get
			Set(ByVal MAX_EXP As Double)
				Word2VecPerformerVoid.MAX_EXP_Conflict = MAX_EXP
			End Set
		End Property


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
						nextRandom_Conflict.set(nextRandom_Conflict.get() * 25214903917L + 11)
						skipGram(i, sentence, CInt(Math.Truncate(nextRandom_Conflict.get())) Mod window_Conflict, alpha)
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
				Dim [end] As Integer = window_Conflict * 2 + 1 - b
				For a As Integer = b To [end] - 1
					If a <> window_Conflict Then
						Dim c As Integer = i - window_Conflict + a
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
			Dim l1 As INDArray = weights_Conflict.vector(w2.Word)


			'error for current word and context
			Dim neu1e As INDArray = Nd4j.create(vectorLength_Conflict)

			Dim i As Integer = 0
			Do While i < w1.getCodeLength()
				Dim code As Integer = w1.getCodes()(i)
				Dim point As Integer = w1.getPoints()(i)

				Dim syn1 As INDArray = weights_Conflict.getSyn1().slice(point)

				Dim dot As Double = Nd4j.BlasWrapper.dot(l1, syn1)

				If dot >= -MAX_EXP_Conflict AndAlso dot < MAX_EXP_Conflict Then

					Dim idx As Integer = CInt(Math.Truncate((dot + MAX_EXP_Conflict) * (CDbl(expTable_Conflict.Length) / MAX_EXP_Conflict / 2.0)))
					If idx >= expTable_Conflict.Length Then
						i += 1
						Continue Do
					End If

					'score
					Dim f As Double = expTable_Conflict(idx)
					'gradient
					Dim g As Double = (1 - code - f) * (If(useAdaGrad_Conflict, w1.getGradient(i, alpha, Me.alpha_Conflict), alpha))


					If neu1e.data().dataType() = DataType.DOUBLE Then
						Nd4j.BlasWrapper.axpy(g, syn1, neu1e)
						Nd4j.BlasWrapper.axpy(g, l1, syn1)
					Else
						Nd4j.BlasWrapper.axpy(CSng(g), syn1, neu1e)
						Nd4j.BlasWrapper.axpy(CSng(g), l1, syn1)
					End If
				End If


				i += 1
			Loop


			'negative sampling
			If negative_Conflict > 0 Then
				Dim target As Integer = w1.Index
				Dim label As Integer
				Dim syn1Neg As INDArray = weights_Conflict.getSyn1Neg().slice(target)

				Dim d As Integer = 0
				Do While d < negative_Conflict + 1
					If d = 0 Then

						label = 1
					Else
						nextRandom_Conflict.set(nextRandom_Conflict.get() * 25214903917L + 11)
						target = table_Conflict.getInt(CInt(nextRandom_Conflict.get() >> 16) Mod CInt(table_Conflict.length()))
						If target = 0 Then
							target = CInt(Math.Truncate(nextRandom_Conflict.get())) Mod (numWords_Conflict - 1) + 1
						End If
						If target = w1.Index Then
							d += 1
							Continue Do
						End If
						label = 0
					End If

					Dim f As Double = Nd4j.BlasWrapper.dot(l1, syn1Neg)
					Dim g As Double
					If f > MAX_EXP_Conflict Then
						g = If(useAdaGrad_Conflict, w1.getGradient(target, (label - 1), Me.alpha_Conflict), (label - 1) * alpha)
					ElseIf f < -MAX_EXP_Conflict Then
						g = label * (If(useAdaGrad_Conflict, w1.getGradient(target, alpha, Me.alpha_Conflict), alpha))
					Else
						g = If(useAdaGrad_Conflict, w1.getGradient(target, label - expTable_Conflict(CInt(Math.Truncate((f + MAX_EXP_Conflict) * (expTable_Conflict.Length / MAX_EXP_Conflict / 2)))), Me.alpha_Conflict), (label - expTable_Conflict(CInt(Math.Truncate((f + MAX_EXP_Conflict) * (expTable_Conflict.Length / MAX_EXP_Conflict / 2))))) * alpha)
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
			For i As Integer = 0 To expTable_Conflict.Length - 1
				Dim tmp As Double = FastMath.exp((i / CDbl(expTable_Conflict.Length) * 2 - 1) * MAX_EXP_Conflict)
				expTable_Conflict(i) = tmp / (tmp + 1.0)
			Next i
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(org.nd4j.common.primitives.Pair<java.util.List<org.deeplearning4j.models.word2vec.VocabWord>, java.util.concurrent.atomic.AtomicLong> pair) throws Exception
		Public Overrides Sub [call](ByVal pair As Pair(Of IList(Of VocabWord), AtomicLong))
			Dim numWordsSoFar As Double = wordCount_Conflict.getValue().doubleValue()

			Dim sentence As IList(Of VocabWord) = pair.First
			Dim alpha2 As Double = Math.Max(minAlpha_Conflict, alpha_Conflict * (1 - (1.0 * numWordsSoFar / CDbl(totalWords_Conflict))))
			Dim totalNewWords As Integer = 0
			trainSentence(sentence, alpha2)
			totalNewWords += sentence.Count



			Dim newWords As Double = totalNewWords + numWordsSoFar
			Dim diff As Double = Math.Abs(newWords - lastChecked_Conflict)
			If diff >= 10000 Then
				lastChecked_Conflict = CInt(Math.Truncate(newWords))
				log.info("Words so far " & newWords & " out of " & totalWords_Conflict)
			End If

			pair.Second.getAndAdd(CLng(totalNewWords))
		End Sub


	End Class

End Namespace