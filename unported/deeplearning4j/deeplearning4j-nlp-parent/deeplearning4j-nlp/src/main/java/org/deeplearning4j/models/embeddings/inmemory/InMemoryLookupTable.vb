Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports AtomicDouble = org.nd4j.shade.guava.util.concurrent.AtomicDouble
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports org.deeplearning4j.models.embeddings
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports org.deeplearning4j.models.word2vec.wordstore
Imports UiConnectionInfo = org.deeplearning4j.core.ui.UiConnectionInfo
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AdaGrad = org.nd4j.linalg.learning.legacy.AdaGrad
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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

Namespace org.deeplearning4j.models.embeddings.inmemory



	''' <summary>
	''' Default word lookup table
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class InMemoryLookupTable(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements WeightLookupTable(Of T)

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(InMemoryLookupTable))

'JAVA TO VB CONVERTER NOTE: The field syn0 was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field syn1 was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend syn0_Conflict, syn1_Conflict As INDArray
'JAVA TO VB CONVERTER NOTE: The field vectorLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vectorLength_Conflict As Integer
		<NonSerialized>
		Protected Friend rng As Random = Nd4j.Random
'JAVA TO VB CONVERTER NOTE: The field lr was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend lr_Conflict As New AtomicDouble(25e-3)
'JAVA TO VB CONVERTER NOTE: The field expTable was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend expTable_Conflict() As Double
		Protected Friend Shared MAX_EXP As Double = 6
		Protected Friend seed As Long = 123
		'negative sampling table
'JAVA TO VB CONVERTER NOTE: The field table was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field syn1Neg was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend table_Conflict, syn1Neg_Conflict As INDArray
'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend useAdaGrad_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field negative was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend negative_Conflict As Double = 0
'JAVA TO VB CONVERTER NOTE: The field useHS was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend useHS_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field vocab was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vocab_Conflict As VocabCache(Of T)
'JAVA TO VB CONVERTER NOTE: The field codes was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend codes_Conflict As IDictionary(Of Integer, INDArray) = New ConcurrentDictionary(Of Integer, INDArray)()



		Protected Friend adaGrad As AdaGrad

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected System.Nullable<Long> tableId;
		Protected Friend tableId As Long?

		Public Sub New()
		End Sub

		Public Sub New(ByVal vocab As VocabCache(Of T), ByVal vectorLength As Integer, ByVal useAdaGrad As Boolean, ByVal lr As Double, ByVal gen As Random, ByVal negative As Double, ByVal useHS As Boolean)
			Me.New(vocab, vectorLength, useAdaGrad, lr, gen, negative)
			Me.useHS_Conflict = useHS
		End Sub

		Public Sub New(ByVal vocab As VocabCache(Of T), ByVal vectorLength As Integer, ByVal useAdaGrad As Boolean, ByVal lr As Double, ByVal gen As Random, ByVal negative As Double)
			Me.vocab_Conflict = vocab
			Me.vectorLength_Conflict = vectorLength
			Me.useAdaGrad_Conflict = useAdaGrad
			Me.lr_Conflict.set(lr)
			Me.rng = gen
			Me.negative_Conflict = negative
			initExpTable()

			If useAdaGrad Then
				initAdaGrad()
			End If
		End Sub

		Protected Friend Overridable Sub initAdaGrad()
			Dim shape() As Long = {vocab_Conflict.numWords() + 1, vectorLength_Conflict}
			Dim length As Integer = ArrayUtil.prod(shape)
			adaGrad = New AdaGrad(shape, lr_Conflict.get())
			adaGrad.setStateViewArray(Nd4j.zeros(shape).reshape(ChrW(1), length), shape, Nd4j.order(), True)

		End Sub

		Public Overridable Property ExpTable As Double()
			Get
    
				Return expTable_Conflict
			End Get
			Set(ByVal expTable() As Double)
				Me.expTable_Conflict = expTable
			End Set
		End Property


		Public Overridable Function getGradient(ByVal column As Integer, ByVal gradient As Double) As Double Implements WeightLookupTable(Of T).getGradient
			If adaGrad Is Nothing Then
				initAdaGrad()
			End If

			Return adaGrad.getGradient(gradient, column, syn0_Conflict.shape())
		End Function

		Public Overridable Function layerSize() As Integer Implements WeightLookupTable(Of T).layerSize
			Return vectorLength_Conflict
		End Function

		Public Overridable Sub resetWeights(ByVal reset As Boolean) Implements WeightLookupTable(Of T).resetWeights
			If Me.rng Is Nothing Then
				Me.rng = Nd4j.Random
			End If

			Me.rng.Seed = seed

			If syn0_Conflict Is Nothing OrElse reset Then
				syn0_Conflict = Nd4j.rand(New Integer() {vocab_Conflict.numWords(), vectorLength_Conflict}, rng).subi(0.5).divi(vectorLength_Conflict)
			End If

			If (syn1_Conflict Is Nothing OrElse reset) AndAlso useHS_Conflict Then
				log.info("Initializing syn1...")
				syn1_Conflict = syn0_Conflict.like()
			End If

			initNegative()
		End Sub



		''' <param name="codeIndex"> </param>
		''' <param name="code"> </param>
		Public Overridable Sub putCode(ByVal codeIndex As Integer, ByVal code As INDArray) Implements WeightLookupTable(Of T).putCode
			codes(codeIndex) = code
		End Sub

		''' <summary>
		''' Loads the co-occurrences for the given codes
		''' </summary>
		''' <param name="codes"> the codes to load </param>
		''' <returns> an ndarray of code.length by layerSize </returns>
		Public Overridable Function loadCodes(ByVal codes() As Integer) As INDArray Implements WeightLookupTable(Of T).loadCodes
			Return syn1_Conflict.getRows(codes)
		End Function


		Public Overridable Sub initNegative()
			SyncLock Me
				If negative_Conflict > 0 AndAlso syn1Neg_Conflict Is Nothing Then
					syn1Neg_Conflict = Nd4j.zeros(syn0_Conflict.shape())
					makeTable(Math.Max(expTable_Conflict.Length, 100000), 0.75)
				End If
			End SyncLock
		End Sub


		Protected Friend Overridable Sub initExpTable()
			expTable_Conflict = New Double(99999){}
			For i As Integer = 0 To expTable_Conflict.Length - 1
				Dim tmp As Double = FastMath.exp((i / CDbl(expTable_Conflict.Length) * 2 - 1) * MAX_EXP)
				expTable_Conflict(i) = tmp / (tmp + 1.0)
			Next i
		End Sub



		''' <summary>
		''' Iterate on the given 2 vocab words
		''' </summary>
		''' <param name="w1"> the first word to iterate on </param>
		''' <param name="w2"> the second word to iterate on </param>
		''' <param name="nextRandom"> next random for sampling </param>
		<Obsolete>
		Public Overridable Sub iterateSample(ByVal w1 As T, ByVal w2 As T, ByVal nextRandom As AtomicLong, ByVal alpha As Double) Implements WeightLookupTable(Of T).iterateSample
			If w2 Is Nothing OrElse w2.getIndex() < 0 OrElse w1.getIndex() = w2.getIndex() OrElse w1.getLabel().Equals("STOP") OrElse w2.getLabel().Equals("STOP") OrElse w1.getLabel().Equals("UNK") OrElse w2.getLabel().Equals("UNK") Then
				Return
			End If
			'current word vector
			Dim l1 As INDArray = Me.syn0_Conflict.slice(w2.getIndex())


			'error for current word and context
			Dim neu1e As INDArray = Nd4j.create(vectorLength_Conflict)


			Dim i As Integer = 0
			Do While i < w1.getCodeLength()
				Dim code As Integer = w1.getCodes().get(i)
				Dim point As Integer = w1.getPoints().get(i)
				If point >= syn0_Conflict.rows() OrElse point < 0 Then
					Throw New System.InvalidOperationException("Illegal point " & point)
				End If
				'other word vector

				Dim syn1 As INDArray = Me.syn1_Conflict.slice(point)


				Dim dot As Double = Nd4j.BlasWrapper.dot(l1, syn1)

				If dot < -MAX_EXP OrElse dot >= MAX_EXP Then
					i += 1
					Continue Do
				End If


				Dim idx As Integer = CInt(Math.Truncate((dot + MAX_EXP) * (CDbl(expTable_Conflict.Length) / MAX_EXP / 2.0)))
				If idx >= expTable_Conflict.Length Then
					i += 1
					Continue Do
				End If

				'score
				Dim f As Double = expTable_Conflict(idx)
				'gradient
				Dim g As Double = If(useAdaGrad_Conflict, w1.getGradient(i, (1 - code - f), lr_Conflict.get()), (1 - code - f) * alpha)

				Nd4j.BlasWrapper.level1().axpy(syn1.length(), g, syn1, neu1e)
				Nd4j.BlasWrapper.level1().axpy(syn1.length(), g, l1, syn1)

				i += 1
			Loop


			Dim target As Integer = w1.getIndex()
			Dim label As Integer
			'negative sampling
			If negative_Conflict > 0 Then
				Dim d As Integer = 0
				Do While d < negative_Conflict + 1
					If d = 0 Then
						label = 1
					Else
						nextRandom.set(nextRandom.get() * 25214903917L + 11)

						Dim idx As Integer = CInt(Math.Abs(CInt(nextRandom.get() >> 16) Mod table_Conflict.length()))

						target = table_Conflict.getInt(idx)
						If target <= 0 Then
							target = CInt(Math.Truncate(nextRandom.get())) Mod (vocab_Conflict.numWords() - 1) + 1
						End If

						If target = w1.getIndex() Then
							d += 1
							Continue Do
						End If
						label = 0
					End If


					If target >= syn1Neg_Conflict.rows() OrElse target < 0 Then
						d += 1
						Continue Do
					End If

					Dim f As Double = Nd4j.BlasWrapper.dot(l1, syn1Neg_Conflict.slice(target))
					Dim g As Double
					If f > MAX_EXP Then
						g = If(useAdaGrad_Conflict, w1.getGradient(target, (label - 1), alpha), (label - 1) * alpha)
					ElseIf f < -MAX_EXP Then
						g = label * (If(useAdaGrad_Conflict, w1.getGradient(target, alpha, alpha), alpha))
					Else
						g = If(useAdaGrad_Conflict, w1.getGradient(target, label - expTable_Conflict(CInt(Math.Truncate((f + MAX_EXP) * (expTable_Conflict.Length / MAX_EXP / 2)))), alpha), (label - expTable_Conflict(CInt(Math.Truncate((f + MAX_EXP) * (expTable_Conflict.Length / MAX_EXP / 2))))) * alpha)
					End If
					If syn0_Conflict.data().dataType() = DataType.DOUBLE Then
						Nd4j.BlasWrapper.axpy(g, syn1Neg_Conflict.slice(target), neu1e)
					Else
						Nd4j.BlasWrapper.axpy(CSng(g), syn1Neg_Conflict.slice(target), neu1e)
					End If

					If syn0_Conflict.data().dataType() = DataType.DOUBLE Then
						Nd4j.BlasWrapper.axpy(g, l1, syn1Neg_Conflict.slice(target))
					Else
						Nd4j.BlasWrapper.axpy(CSng(g), l1, syn1Neg_Conflict.slice(target))
					End If
					d += 1
				Loop
			End If

			If syn0_Conflict.data().dataType() = DataType.DOUBLE Then
				Nd4j.BlasWrapper.axpy(1.0, neu1e, l1)

			Else
				Nd4j.BlasWrapper.axpy(1.0f, neu1e, l1)
			End If

		End Sub

		Public Overridable Property UseAdaGrad As Boolean
			Get
				Return useAdaGrad_Conflict
			End Get
			Set(ByVal useAdaGrad As Boolean)
				Me.useAdaGrad_Conflict = useAdaGrad
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

		Public Overridable WriteOnly Property UseHS As Boolean
			Set(ByVal useHS As Boolean)
				Me.useHS_Conflict = useHS
			End Set
		End Property


		''' <summary>
		''' Iterate on the given 2 vocab words
		''' </summary>
		''' <param name="w1"> the first word to iterate on </param>
		''' <param name="w2"> the second word to iterate on </param>
		<Obsolete>
		Public Overridable Sub iterate(ByVal w1 As T, ByVal w2 As T) Implements WeightLookupTable(Of T).iterate

		End Sub


		''' <summary>
		''' Reset the weights of the cache
		''' </summary>
		Public Overridable Sub resetWeights() Implements WeightLookupTable(Of T).resetWeights
			resetWeights(True)
		End Sub


		Protected Friend Overridable Sub makeTable(ByVal tableSize As Integer, ByVal power As Double)
			Dim vocabSize As Integer = syn0_Conflict.rows()
			table_Conflict = Nd4j.create(tableSize)
			Dim trainWordsPow As Double = 0.0
			For Each word As String In vocab_Conflict.words()
				trainWordsPow += Math.Pow(vocab_Conflict.wordFrequency(word), power)
			Next word

			Dim wordIdx As Integer = 0
			Dim word As String = vocab_Conflict.wordAtIndex(wordIdx)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim d1 As Double = Math.Pow(vocab_Conflict.wordFrequency(word), power) / trainWordsPow
			For i As Integer = 0 To tableSize - 1
				table_Conflict.putScalar(i, wordIdx)
				Dim mul As Double = i * 1.0 / CDbl(tableSize)
				If mul > d1 Then
					If wordIdx < vocabSize - 1 Then
						wordIdx += 1
					End If
					word = vocab_Conflict.wordAtIndex(wordIdx)
					Dim wordAtIndex As String = vocab_Conflict.wordAtIndex(wordIdx)
					If word Is Nothing Then
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					d1 += Math.Pow(vocab_Conflict.wordFrequency(wordAtIndex), power) / trainWordsPow
				End If
			Next i
		End Sub

		''' <summary>
		''' Inserts a word vector
		''' </summary>
		''' <param name="word">   the word to insert </param>
		''' <param name="vector"> the vector to insert </param>
		Public Overridable Sub putVector(ByVal word As String, ByVal vector As INDArray) Implements WeightLookupTable(Of T).putVector
			If word Is Nothing Then
				Throw New System.ArgumentException("No null words allowed")
			End If
			If vector Is Nothing Then
				Throw New System.ArgumentException("No null vectors allowed")
			End If
			Dim idx As Integer = vocab_Conflict.indexOf(word)
			syn0_Conflict.slice(idx).assign(vector)

		End Sub

		Public Overridable Property Table As INDArray
			Get
				Return table_Conflict
			End Get
			Set(ByVal table As INDArray)
				Me.table_Conflict = table
			End Set
		End Property


		Public Overridable Property Syn1Neg As INDArray
			Get
				Return syn1Neg_Conflict
			End Get
			Set(ByVal syn1Neg As INDArray)
				Me.syn1Neg_Conflict = syn1Neg
			End Set
		End Property


		''' <param name="word">
		''' @return </param>
		Public Overridable Function vector(ByVal word As String) As INDArray Implements WeightLookupTable(Of T).vector
			If word Is Nothing Then
				Return Nothing
			End If
			Dim idx As Integer = vocab_Conflict.indexOf(word)
			If idx < 0 Then
				idx = vocab_Conflict.indexOf(Word2Vec.DEFAULT_UNK)
				If idx < 0 Then
					Return Nothing
				End If
			End If
			Return syn0_Conflict.getRow(idx, True)
		End Function

		Public Overridable WriteOnly Property LearningRate(ByVal lr As Double) Implements WeightLookupTable.setLearningRate As Double
			Set(ByVal lr As Double)
				Me.lr_Conflict.set(lr)
			End Set
		End Property

		Public Overridable Function vectors() As IEnumerator(Of INDArray)
			Return New WeightIterator(Me)
		End Function

		Public Overridable ReadOnly Property Weights As INDArray Implements WeightLookupTable(Of T).getWeights
			Get
				Return syn0_Conflict
			End Get
		End Property


		Protected Friend Class WeightIterator
			Implements IEnumerator(Of INDArray)

			Private ReadOnly outerInstance As InMemoryLookupTable(Of T)

			Public Sub New(ByVal outerInstance As InMemoryLookupTable(Of T))
				Me.outerInstance = outerInstance
			End Sub

			Protected Friend currIndex As Integer = 0

			Public Overrides Function hasNext() As Boolean
				Return currIndex < outerInstance.syn0_Conflict.rows()
			End Function

			Public Overrides Function [next]() As INDArray
				Dim ret As INDArray = outerInstance.syn0_Conflict.slice(currIndex)
				currIndex += 1
				Return ret
			End Function

			Public Overrides Sub remove()
				Throw New System.NotSupportedException()
			End Sub
		End Class

		Public Overridable Property Syn0 As INDArray
			Get
				Return syn0_Conflict
			End Get
			Set(ByVal syn0 As INDArray)
				Preconditions.checkArgument(Not syn0.isEmpty(), "syn0 can't be empty")
				Preconditions.checkArgument(syn0.rank() = 2, "syn0 must have rank 2")
    
				Me.syn0_Conflict = syn0
				Me.vectorLength_Conflict = syn0.columns()
			End Set
		End Property


		Public Overridable Property Syn1 As INDArray
			Get
				Return syn1_Conflict
			End Get
			Set(ByVal syn1 As INDArray)
				Me.syn1_Conflict = syn1
			End Set
		End Property


		Public Overridable Function getVocabCache() As VocabCache(Of T) Implements WeightLookupTable(Of T).getVocabCache
			Return vocab_Conflict
		End Function

		Public Overridable WriteOnly Property VectorLength As Integer
			Set(ByVal vectorLength As Integer)
				Me.vectorLength_Conflict = vectorLength
			End Set
		End Property

		''' <summary>
		''' This method is deprecated, since all logic was pulled out from this class and is not used anymore.
		''' However this method will be around for a while, due to backward compatibility issues. </summary>
		''' <returns> initial learning rate </returns>
		<Obsolete>
		Public Overridable Property Lr As AtomicDouble
			Get
				Return lr_Conflict
			End Get
			Set(ByVal lr As AtomicDouble)
				Me.lr_Conflict = lr
			End Set
		End Property


		Public Overridable Property Vocab As VocabCache
			Get
				Return vocab_Conflict
			End Get
			Set(ByVal vocab As VocabCache)
				Me.vocab_Conflict = vocab
			End Set
		End Property


		Public Overridable Property Codes As IDictionary(Of Integer, INDArray)
			Get
				Return codes_Conflict
			End Get
			Set(ByVal codes As IDictionary(Of Integer, INDArray))
				Me.codes_Conflict = codes
			End Set
		End Property


		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER NOTE: The field vectorLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend vectorLength_Conflict As Integer = 100
'JAVA TO VB CONVERTER NOTE: The field useAdaGrad was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend useAdaGrad_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field lr was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend lr_Conflict As Double = 0.025
'JAVA TO VB CONVERTER NOTE: The field gen was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend gen_Conflict As Random = Nd4j.Random
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend seed_Conflict As Long = 123
'JAVA TO VB CONVERTER NOTE: The field negative was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend negative_Conflict As Double = 0
			Protected Friend vocabCache As VocabCache(Of T)
			Protected Friend useHS As Boolean = True



			Public Overridable Function useHierarchicSoftmax(ByVal reallyUse As Boolean) As Builder(Of T)
				Me.useHS = reallyUse
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder<T> cache(@NonNull VocabCache<T> vocab)
			Public Overridable Function cache(ByVal vocab As VocabCache(Of T)) As Builder(Of T)
				Me.vocabCache = vocab
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter negative was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function negative(ByVal negative_Conflict As Double) As Builder(Of T)
				Me.negative_Conflict = negative_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter vectorLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function vectorLength(ByVal vectorLength_Conflict As Integer) As Builder(Of T)
				Me.vectorLength_Conflict = vectorLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter useAdaGrad was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function useAdaGrad(ByVal useAdaGrad_Conflict As Boolean) As Builder(Of T)
				Me.useAdaGrad_Conflict = useAdaGrad_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method is deprecated, since all logic was pulled out from this class </summary>
			''' <param name="lr">
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter lr was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete>
			Public Overridable Function lr(ByVal lr_Conflict As Double) As Builder(Of T)
				Me.lr_Conflict = lr_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter gen was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function gen(ByVal gen_Conflict As Random) As Builder(Of T)
				Me.gen_Conflict = gen_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter seed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function seed(ByVal seed_Conflict As Long) As Builder(Of T)
				Me.seed_Conflict = seed_Conflict
				Return Me
			End Function



			Public Overridable Function build() As InMemoryLookupTable(Of T)
				If vocabCache Is Nothing Then
					Throw New System.InvalidOperationException("Vocab cache must be specified")
				End If

				Dim table As New InMemoryLookupTable(Of T)(vocabCache, vectorLength_Conflict, useAdaGrad_Conflict, lr_Conflict, gen_Conflict, negative_Conflict, useHS)
				table.seed = seed_Conflict

				Return table
			End Function
		End Class

		Public Overrides Function ToString() As String
			Return "InMemoryLookupTable{" & "syn0=" & syn0_Conflict & ", syn1=" & syn1_Conflict & ", vectorLength=" & vectorLength_Conflict & ", rng=" & rng & ", lr=" & lr_Conflict & ", expTable=" & java.util.Arrays.toString(expTable_Conflict) & ", seed=" & seed & ", table=" & table_Conflict & ", syn1Neg=" & syn1Neg_Conflict & ", useAdaGrad=" & useAdaGrad_Conflict & ", negative=" & negative_Conflict & ", vocab=" & vocab_Conflict & ", codes=" & codes_Conflict & "}"c
		End Function

		''' <summary>
		''' This method consumes weights of a given InMemoryLookupTable
		''' 
		''' PLEASE NOTE: this method explicitly resets current weights
		''' </summary>
		''' <param name="srcTable"> </param>
		Public Overridable Sub consume(ByVal srcTable As InMemoryLookupTable(Of T))
			If srcTable.vectorLength_Conflict <> Me.vectorLength_Conflict Then
				Throw New System.InvalidOperationException("You can't consume lookupTable with different vector lengths")
			End If

			If srcTable.syn0_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Source lookupTable Syn0 is NULL")
			End If

			Me.resetWeights(True)

			Dim cntHs As New AtomicInteger(0)
			Dim cntNg As New AtomicInteger(0)

			If srcTable.syn0_Conflict.rows() > Me.syn0_Conflict.rows() Then
				Throw New System.InvalidOperationException("You can't consume lookupTable with built for larger vocabulary without updating your vocabulary first")
			End If

			Dim x As Integer = 0
			Do While x < srcTable.syn0_Conflict.rows()
				Me.syn0_Conflict.putRow(x, srcTable.syn0_Conflict.getRow(x))

				If Me.syn1_Conflict IsNot Nothing AndAlso srcTable.syn1_Conflict IsNot Nothing Then
					Me.syn1_Conflict.putRow(x, srcTable.syn1_Conflict.getRow(x))
				ElseIf cntHs.incrementAndGet() = 1 Then
					log.info("Skipping syn1 merge")
				End If

				If Me.syn1Neg_Conflict IsNot Nothing AndAlso srcTable.syn1Neg_Conflict IsNot Nothing Then
					Me.syn1Neg_Conflict.putRow(x, srcTable.syn1Neg_Conflict.getRow(x))
				ElseIf cntNg.incrementAndGet() = 1 Then
					log.info("Skipping syn1Neg merge")
				End If

				If cntHs.get() > 0 AndAlso cntNg.get() > 0 Then
					Throw New ND4JIllegalStateException("srcTable has no syn1/syn1neg")
				End If
				x += 1
			Loop
		End Sub
	End Class

End Namespace