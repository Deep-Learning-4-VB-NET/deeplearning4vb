Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports BertMaskedLMMasker = org.deeplearning4j.iterator.bert.BertMaskedLMMasker
Imports BertSequenceMasker = org.deeplearning4j.iterator.bert.BertSequenceMasker
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports BertWordPieceTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.BertWordPieceTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.iterator


	<Serializable>
	Public Class BertIterator
		Implements MultiDataSetIterator

		Public Enum Task
			UNSUPERVISED
			SEQ_CLASSIFICATION

		End Enum
		Public Enum LengthHandling
			FIXED_LENGTH
			ANY_LENGTH
			CLIP_ONLY

		End Enum
		Public Enum FeatureArrays
			INDICES_MASK
			INDICES_MASK_SEGMENTID

		End Enum
		Public Enum UnsupervisedLabelFormat
			RANK2_IDX
			RANK3_NCL
			RANK3_LNC

		End Enum
		Protected Friend task As Task
		Protected Friend tokenizerFactory As TokenizerFactory
		Protected Friend maxTokens As Integer = -1
		Protected Friend minibatchSize As Integer = 32
		Protected Friend padMinibatches As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor preProcessor;
		Protected Friend preProcessor As MultiDataSetPreProcessor
		Protected Friend sentenceProvider As LabeledSentenceProvider = Nothing
		Protected Friend sentencePairProvider As LabeledPairSentenceProvider = Nothing
		Protected Friend lengthHandling As LengthHandling
		Protected Friend featureArrays As FeatureArrays
		Protected Friend vocabMap As IDictionary(Of String, Integer) 'TODO maybe use Eclipse ObjectIntHashMap or similar for fewer objects?
		Protected Friend masker As BertSequenceMasker = Nothing
		Protected Friend unsupervisedLabelFormat As UnsupervisedLabelFormat = Nothing
		Protected Friend maskToken As String
		Protected Friend prependToken As String
		Protected Friend appendToken As String


		Protected Friend vocabKeysAsList As IList(Of String)

		Protected Friend Sub New(ByVal b As Builder)
			Me.task = b.task_Conflict
			Me.tokenizerFactory = b.tokenizerFactory
			Me.maxTokens = b.maxTokens
			Me.minibatchSize = b.minibatchSize_Conflict
			Me.padMinibatches = b.padMinibatches_Conflict
			Me.preProcessor = b.preProcessor_Conflict
			Me.sentenceProvider = b.sentenceProvider_Conflict
			Me.sentencePairProvider = b.sentencePairProvider_Conflict
			Me.lengthHandling = b.lengthHandling_Conflict
			Me.featureArrays = b.featureArrays_Conflict
			Me.vocabMap = b.vocabMap_Conflict
			Me.masker = b.masker_Conflict
			Me.unsupervisedLabelFormat = b.unsupervisedLabelFormat_Conflict
			Me.maskToken = b.maskToken_Conflict
			Me.prependToken = b.prependToken_Conflict
			Me.appendToken = b.appendToken_Conflict
		End Sub

		Public Overrides Function hasNext() As Boolean
			If sentenceProvider IsNot Nothing Then
				Return sentenceProvider.hasNext()
			End If
			Return sentencePairProvider.hasNext()
		End Function

		Public Overrides Function [next]() As MultiDataSet
			Return [next](minibatchSize)
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Preconditions.checkState(hasNext(), "No next element available")
			Dim tokensAndLabelList As IList(Of Pair(Of IList(Of String), String))
			Dim mbSize As Integer = 0
			Dim outLength As Integer
			Dim segIdOnesFrom() As Long = Nothing
			If sentenceProvider IsNot Nothing Then
				Dim list As IList(Of Pair(Of String, String)) = New List(Of Pair(Of String, String))(num)
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (sentenceProvider.hasNext() && mbSize++ < num)
				Do While sentenceProvider.hasNext() AndAlso mbSize++ < num
					list.Add(sentenceProvider.nextSentence())
				Loop
				Dim sentenceListProcessed As SentenceListProcessed = tokenizeMiniBatch(list)
				tokensAndLabelList = sentenceListProcessed.getTokensAndLabelList()
				outLength = sentenceListProcessed.getMaxL()
			ElseIf sentencePairProvider IsNot Nothing Then
				Dim listPairs As IList(Of Triple(Of String, String, String)) = New List(Of Triple(Of String, String, String))(num)
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (sentencePairProvider.hasNext() && mbSize++ < num)
				Do While sentencePairProvider.hasNext() AndAlso mbSize++ < num
					listPairs.Add(sentencePairProvider.nextSentencePair())
				Loop
				Dim sentencePairListProcessed As SentencePairListProcessed = tokenizePairsMiniBatch(listPairs)
				tokensAndLabelList = sentencePairListProcessed.getTokensAndLabelList()
				outLength = sentencePairListProcessed.MaxL
				segIdOnesFrom = sentencePairListProcessed.getSegIdOnesFrom()
			Else
				'TODO - other types of iterators...
				Throw New System.NotSupportedException("Labelled sentence provider is null and no other iterator types have yet been implemented")
			End If

			Dim featuresAndMaskArraysPair As Pair(Of INDArray(), INDArray()) = convertMiniBatchFeatures(tokensAndLabelList, outLength, segIdOnesFrom)
			Dim featureArray() As INDArray = featuresAndMaskArraysPair.First
			Dim featureMaskArray() As INDArray = featuresAndMaskArraysPair.Second


			Dim labelsAndMaskArraysPair As Pair(Of INDArray(), INDArray()) = convertMiniBatchLabels(tokensAndLabelList, featureArray, outLength)
			Dim labelArray() As INDArray = labelsAndMaskArraysPair.First
			Dim labelMaskArray() As INDArray = labelsAndMaskArraysPair.Second

			Dim mds As New org.nd4j.linalg.dataset.MultiDataSet(featureArray, labelArray, featureMaskArray, labelMaskArray)
			If preProcessor IsNot Nothing Then
				preProcessor.preProcess(mds)
			End If

			Return mds
		End Function


		''' <summary>
		''' For use during inference. Will convert a given list of sentences to features and feature masks as appropriate.
		''' </summary>
		''' <param name="listOnlySentences"> </param>
		''' <returns> Pair of INDArrays[], first element is feature arrays and the second is the masks array </returns>
		Public Overridable Function featurizeSentences(ByVal listOnlySentences As IList(Of String)) As Pair(Of INDArray(), INDArray())

			Dim sentencesWithNullLabel As IList(Of Pair(Of String, String)) = addDummyLabel(listOnlySentences)
			Dim sentenceListProcessed As SentenceListProcessed = tokenizeMiniBatch(sentencesWithNullLabel)
			Dim tokensAndLabelList As IList(Of Pair(Of IList(Of String), String)) = sentenceListProcessed.getTokensAndLabelList()
			Dim outLength As Integer = sentenceListProcessed.getMaxL()

			If preProcessor IsNot Nothing Then
				Dim featureFeatureMasks As Pair(Of INDArray(), INDArray()) = convertMiniBatchFeatures(tokensAndLabelList, outLength, Nothing)
				Dim dummyMDS As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(featureFeatureMasks.First, Nothing, featureFeatureMasks.Second, Nothing)
				preProcessor.preProcess(dummyMDS)
				Return New Pair(Of INDArray(), INDArray())(dummyMDS.Features, dummyMDS.FeaturesMaskArrays)
			End If
			Return convertMiniBatchFeatures(tokensAndLabelList, outLength, Nothing)
		End Function

		''' <summary>
		''' For use during inference. Will convert a given pair of a list of sentences to features and feature masks as appropriate.
		''' </summary>
		''' <param name="listOnlySentencePairs"> </param>
		''' <returns> Pair of INDArrays[], first element is feature arrays and the second is the masks array </returns>
		Public Overridable Function featurizeSentencePairs(ByVal listOnlySentencePairs As IList(Of Pair(Of String, String))) As Pair(Of INDArray(), INDArray())
			Preconditions.checkState(sentencePairProvider IsNot Nothing, "The featurizeSentencePairs method is meant for inference with sentence pairs. Use only when the sentence pair provider is set (i.e not null).")

			Dim sentencePairsWithNullLabel As IList(Of Triple(Of String, String, String)) = addDummyLabelForPairs(listOnlySentencePairs)
			Dim sentencePairListProcessed As SentencePairListProcessed = tokenizePairsMiniBatch(sentencePairsWithNullLabel)
			Dim tokensAndLabelList As IList(Of Pair(Of IList(Of String), String)) = sentencePairListProcessed.getTokensAndLabelList()
			Dim outLength As Integer = sentencePairListProcessed.MaxL
			Dim segIdOnesFrom() As Long = sentencePairListProcessed.getSegIdOnesFrom()
			If preProcessor IsNot Nothing Then
				Dim featuresAndMaskArraysPair As Pair(Of INDArray(), INDArray()) = convertMiniBatchFeatures(tokensAndLabelList, outLength, segIdOnesFrom)
				Dim dummyMDS As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(featuresAndMaskArraysPair.First, Nothing, featuresAndMaskArraysPair.Second, Nothing)
				preProcessor.preProcess(dummyMDS)
				Return New Pair(Of INDArray(), INDArray())(dummyMDS.Features, dummyMDS.FeaturesMaskArrays)
			End If
			Return convertMiniBatchFeatures(tokensAndLabelList, outLength, segIdOnesFrom)
		End Function

		Private Function convertMiniBatchFeatures(ByVal tokensAndLabelList As IList(Of Pair(Of IList(Of String), String)), ByVal outLength As Integer, ByVal segIdOnesFrom() As Long) As Pair(Of INDArray(), INDArray())
			Dim mbPadded As Integer = If(padMinibatches, minibatchSize, tokensAndLabelList.Count)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim outIdxs[][] As Integer = new Integer[mbPadded][outLength]
			Dim outIdxs()() As Integer = RectangularArrays.RectangularIntegerArray(mbPadded, outLength)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim outMask[][] As Integer = new Integer[mbPadded][outLength]
			Dim outMask()() As Integer = RectangularArrays.RectangularIntegerArray(mbPadded, outLength)
			Dim outSegmentId()() As Integer = Nothing
			If featureArrays = FeatureArrays.INDICES_MASK_SEGMENTID Then
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: outSegmentId = new Integer[mbPadded][outLength]
				outSegmentId = RectangularArrays.RectangularIntegerArray(mbPadded, outLength)
			End If
			For i As Integer = 0 To tokensAndLabelList.Count - 1
				Dim p As Pair(Of IList(Of String), String) = tokensAndLabelList(i)
				Dim t As IList(Of String) = p.First
				Dim j As Integer = 0
				Do While j < outLength AndAlso j < t.Count
					Preconditions.checkState(vocabMap.ContainsKey(t(j)), "Unknown token encountered: token ""%s"" is not in vocabulary", t(j))
					Dim idx As Integer = vocabMap(t(j))
					outIdxs(i)(j) = idx
					outMask(i)(j) = 1
					If segIdOnesFrom IsNot Nothing AndAlso j >= segIdOnesFrom(i) Then
						outSegmentId(i)(j) = 1
					End If
					j += 1
				Loop
			Next i

			'Create actual arrays. Indices, mask, and optional segment ID
			Dim outIdxsArr As INDArray = Nd4j.createFromArray(outIdxs)
			Dim outMaskArr As INDArray = Nd4j.createFromArray(outMask)
			Dim outSegmentIdArr As INDArray
			Dim f() As INDArray
			Dim fm() As INDArray
			If featureArrays = FeatureArrays.INDICES_MASK_SEGMENTID Then
				outSegmentIdArr = Nd4j.createFromArray(outSegmentId)
				f = New INDArray(){outIdxsArr, outSegmentIdArr}
				fm = New INDArray(){outMaskArr, Nothing}
			Else
				f = New INDArray(){outIdxsArr}
				fm = New INDArray(){outMaskArr}
			End If
			Return New Pair(Of INDArray(), INDArray())(f, fm)
		End Function

		Private Function tokenizeMiniBatch(ByVal list As IList(Of Pair(Of String, String))) As SentenceListProcessed
			'Get and tokenize the sentences for this minibatch
			Dim sentenceListProcessed As New SentenceListProcessed(list.Count)
			Dim longestSeq As Integer = -1
			For Each p As Pair(Of String, String) In list
				Dim tokens As IList(Of String) = tokenizeSentence(p.First)
				sentenceListProcessed.addProcessedToList(New Pair(Of IList(Of String), String)(tokens, p.Second))
				longestSeq = Math.Max(longestSeq, tokens.Count)
			Next p
			'Determine output array length...
			Dim outLength As Integer
			Select Case lengthHandling
				Case org.deeplearning4j.iterator.BertIterator.LengthHandling.FIXED_LENGTH
					outLength = maxTokens
				Case org.deeplearning4j.iterator.BertIterator.LengthHandling.ANY_LENGTH
					outLength = longestSeq
				Case org.deeplearning4j.iterator.BertIterator.LengthHandling.CLIP_ONLY
					outLength = Math.Min(maxTokens, longestSeq)
				Case Else
					Throw New Exception("Not implemented length handling mode: " & lengthHandling)
			End Select
			sentenceListProcessed.setMaxL(outLength)
			Return sentenceListProcessed
		End Function

		Private Function tokenizePairsMiniBatch(ByVal listPairs As IList(Of Triple(Of String, String, String))) As SentencePairListProcessed
			Dim sentencePairListProcessed As New SentencePairListProcessed(listPairs.Count)
			For Each t As Triple(Of String, String, String) In listPairs
				Dim tokensL As IList(Of String) = tokenizeSentence(t.getFirst(), True)
				Dim tokensR As IList(Of String) = tokenizeSentence(t.getSecond(), True)
				Dim tokens As IList(Of String) = New List(Of String)(maxTokens)
				Dim maxLength As Integer = maxTokens
				If prependToken IsNot Nothing Then
					maxLength -= 1
				End If
				If appendToken IsNot Nothing Then
					maxLength -= 2
				End If
				If tokensL.Count + tokensR.Count > maxLength Then
					Dim shortOnL As Boolean = tokensL.Count < tokensR.Count
					Dim shortSize As Integer = Math.Min(tokensL.Count, tokensR.Count)
					If shortSize > maxLength \ 2 Then
						'both lists need to be sliced
						tokensL.RemoveRange(maxLength \ 2, tokensL.Count) 'if maxsize/2 is odd pop extra on L side to match implementation in TF
						tokensR.RemoveRange(maxLength - maxLength \ 2, tokensR.Count)
					Else
						'slice longer list
						If shortOnL Then
							'longer on R - slice R
							tokensR.RemoveRange(maxLength - tokensL.Count, tokensR.Count)
						Else
							'longer on L - slice L
							tokensL.RemoveRange(maxLength - tokensR.Count, tokensL.Count)
						End If
					End If
				End If
				If prependToken IsNot Nothing Then
					tokens.Add(prependToken)
				End If
				CType(tokens, List(Of String)).AddRange(tokensL)
				If appendToken IsNot Nothing Then
					tokens.Add(appendToken)
				End If
				Dim segIdOnesFrom As Integer = tokens.Count
				CType(tokens, List(Of String)).AddRange(tokensR)
				If appendToken IsNot Nothing Then
					tokens.Add(appendToken)
				End If
				sentencePairListProcessed.addProcessedToList(segIdOnesFrom, New Pair(Of IList(Of String), String)(tokens, t.getThird()))
			Next t
			sentencePairListProcessed.MaxL = maxTokens
			Return sentencePairListProcessed
		End Function

		Private Function convertMiniBatchLabels(ByVal tokenizedSentences As IList(Of Pair(Of IList(Of String), String)), ByVal featureArray() As INDArray, ByVal outLength As Integer) As Pair(Of INDArray(), INDArray())
			Dim l(0) As INDArray
			Dim lm() As INDArray
			Dim mbSize As Integer = tokenizedSentences.Count
			Dim mbPadded As Integer = If(padMinibatches, minibatchSize, tokenizedSentences.Count)
			If task = Task.SEQ_CLASSIFICATION Then
				'Sequence classification task: output is 2d, one-hot, shape [minibatch, numClasses]
				Dim numClasses As Integer
				Dim classLabels(mbPadded - 1) As Integer
				If sentenceProvider IsNot Nothing Then
					numClasses = sentenceProvider.numLabelClasses()
					Dim labels As IList(Of String) = sentenceProvider.allLabels()
					For i As Integer = 0 To mbSize - 1
						Dim lbl As String = tokenizedSentences(i).getRight()
						classLabels(i) = labels.IndexOf(lbl)
						Preconditions.checkState(classLabels(i) >= 0, "Provided label ""%s"" for sentence does not exist in set of classes/categories", lbl)
					Next i
				ElseIf sentencePairProvider IsNot Nothing Then
					numClasses = sentencePairProvider.numLabelClasses()
					Dim labels As IList(Of String) = sentencePairProvider.allLabels()
					For i As Integer = 0 To mbSize - 1
						Dim lbl As String = tokenizedSentences(i).getRight()
						classLabels(i) = labels.IndexOf(lbl)
						Preconditions.checkState(classLabels(i) >= 0, "Provided label ""%s"" for sentence does not exist in set of classes/categories", lbl)
					Next i
				Else
					Throw New Exception()
				End If
				l(0) = Nd4j.create(DataType.FLOAT, mbPadded, numClasses)
				For i As Integer = 0 To mbSize - 1
					l(0).putScalar(i, classLabels(i), 1.0)
				Next i
				lm = Nothing
				If padMinibatches AndAlso mbSize <> mbPadded Then
					Dim a As INDArray = Nd4j.zeros(DataType.FLOAT, mbPadded, 1)
					lm = New INDArray(){a}
					a.get(NDArrayIndex.interval(0, mbSize), NDArrayIndex.all()).assign(1)
				End If
			ElseIf task = Task.UNSUPERVISED Then
				'Unsupervised, masked language model task
				'Output is either 2d, or 3d depending on settings
				If vocabKeysAsList Is Nothing Then
					Dim arr(vocabMap.Count - 1) As String
					For Each e As KeyValuePair(Of String, Integer) In vocabMap.SetOfKeyValuePairs()
						arr(e.Value) = e.Key
					Next e
					vocabKeysAsList = New List(Of String) From {arr}
				End If


				Dim vocabSize As Integer = vocabMap.Count
				Dim labelArr As INDArray
				Dim lMask As INDArray = Nd4j.zeros(DataType.INT, mbPadded, outLength)
				If unsupervisedLabelFormat = UnsupervisedLabelFormat.RANK2_IDX Then
					labelArr = Nd4j.create(DataType.INT, mbPadded, outLength)
				ElseIf unsupervisedLabelFormat = UnsupervisedLabelFormat.RANK3_NCL Then
					labelArr = Nd4j.create(DataType.FLOAT, mbPadded, vocabSize, outLength)
				ElseIf unsupervisedLabelFormat = UnsupervisedLabelFormat.RANK3_LNC Then
					labelArr = Nd4j.create(DataType.FLOAT, outLength, mbPadded, vocabSize)
				Else
					Throw New System.InvalidOperationException("Unknown unsupervised label format: " & unsupervisedLabelFormat)
				End If

				For i As Integer = 0 To mbSize - 1
					Dim tokens As IList(Of String) = tokenizedSentences(i).getFirst()
					Dim p As Pair(Of IList(Of String), Boolean()) = masker.maskSequence(tokens, maskToken, vocabKeysAsList)
					Dim maskedTokens As IList(Of String) = p.First
					Dim predictionTarget() As Boolean = p.Second
					Dim seqLen As Integer = Math.Min(predictionTarget.Length, outLength)
					For j As Integer = 0 To seqLen - 1
						If predictionTarget(j) Then
							Dim oldToken As String = tokenizedSentences(i).getFirst().get(j) 'This is target
							Dim targetTokenIdx As Integer = vocabMap(oldToken)
							If unsupervisedLabelFormat = UnsupervisedLabelFormat.RANK2_IDX Then
								labelArr.putScalar(i, j, targetTokenIdx)
							ElseIf unsupervisedLabelFormat = UnsupervisedLabelFormat.RANK3_NCL Then
								labelArr.putScalar(i, j, targetTokenIdx, 1.0)
							ElseIf unsupervisedLabelFormat = UnsupervisedLabelFormat.RANK3_LNC Then
								labelArr.putScalar(j, i, targetTokenIdx, 1.0)
							End If

							lMask.putScalar(i, j, 1.0)

							'Also update previously created feature label indexes:
							Dim newToken As String = maskedTokens(j)
							Dim newTokenIdx As Integer = vocabMap(newToken)
							'first element of features is outIdxsArr
							featureArray(0).putScalar(i, j, newTokenIdx)
						End If
					Next j
				Next i
				l(0) = labelArr
				lm = New INDArray(0){}
				lm(0) = lMask
			Else
				Throw New System.InvalidOperationException("Task not yet implemented: " & task)
			End If
			Return New Pair(Of INDArray(), INDArray())(l, lm)
		End Function

		Private Function tokenizeSentence(ByVal sentence As String) As IList(Of String)
			Return tokenizeSentence(sentence, False)
		End Function

		Private Function tokenizeSentence(ByVal sentence As String, ByVal ignorePrependAppend As Boolean) As IList(Of String)
			Dim t As Tokenizer = tokenizerFactory.create(sentence)

			Dim tokens As IList(Of String) = New List(Of String)()
			If prependToken IsNot Nothing AndAlso Not ignorePrependAppend Then
				tokens.Add(prependToken)
			End If

			Do While t.hasMoreTokens()
				Dim token As String = t.nextToken()
				tokens.Add(token)
			Loop
			If appendToken IsNot Nothing AndAlso Not ignorePrependAppend Then
				tokens.Add(appendToken)
			End If
			Return tokens
		End Function


		Private Function addDummyLabel(ByVal listOnlySentences As IList(Of String)) As IList(Of Pair(Of String, String))
			Dim list As IList(Of Pair(Of String, String)) = New List(Of Pair(Of String, String))(listOnlySentences.Count)
			For Each s As String In listOnlySentences
				list.Add(New Pair(Of String, String)(s, Nothing))
			Next s
			Return list
		End Function

		Private Function addDummyLabelForPairs(ByVal listOnlySentencePairs As IList(Of Pair(Of String, String))) As IList(Of Triple(Of String, String, String))
			Dim list As IList(Of Triple(Of String, String, String)) = New List(Of Triple(Of String, String, String))(listOnlySentencePairs.Count)
			For Each p As Pair(Of String, String) In listOnlySentencePairs
				list.Add(New Triple(Of String, String, String)(p.First, p.Second, Nothing))
			Next p
			Return list
		End Function

		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			If sentenceProvider IsNot Nothing Then
				sentenceProvider.reset()
			End If
		End Sub

		Public Shared Function builder() As Builder
			Return New Builder()
		End Function

		Public Class Builder

'JAVA TO VB CONVERTER NOTE: The field task was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend task_Conflict As Task
			Protected Friend tokenizerFactory As TokenizerFactory
'JAVA TO VB CONVERTER NOTE: The field lengthHandling was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend lengthHandling_Conflict As LengthHandling = LengthHandling.FIXED_LENGTH
			Protected Friend maxTokens As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field minibatchSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend minibatchSize_Conflict As Integer = 32
'JAVA TO VB CONVERTER NOTE: The field padMinibatches was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend padMinibatches_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend preProcessor_Conflict As MultiDataSetPreProcessor
'JAVA TO VB CONVERTER NOTE: The field sentenceProvider was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend sentenceProvider_Conflict As LabeledSentenceProvider = Nothing
'JAVA TO VB CONVERTER NOTE: The field sentencePairProvider was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend sentencePairProvider_Conflict As LabeledPairSentenceProvider = Nothing
'JAVA TO VB CONVERTER NOTE: The field featureArrays was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend featureArrays_Conflict As FeatureArrays = FeatureArrays.INDICES_MASK_SEGMENTID
'JAVA TO VB CONVERTER NOTE: The field vocabMap was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend vocabMap_Conflict As IDictionary(Of String, Integer) 'TODO maybe use Eclipse ObjectIntHashMap for fewer objects?
'JAVA TO VB CONVERTER NOTE: The field masker was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend masker_Conflict As BertSequenceMasker = New BertMaskedLMMasker()
'JAVA TO VB CONVERTER NOTE: The field unsupervisedLabelFormat was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend unsupervisedLabelFormat_Conflict As UnsupervisedLabelFormat
'JAVA TO VB CONVERTER NOTE: The field maskToken was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend maskToken_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field prependToken was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend prependToken_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field appendToken was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend appendToken_Conflict As String

			''' <summary>
			''' Specify the <seealso cref="Task"/> the iterator should be set up for. See <seealso cref="BertIterator"/> for more details.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter task was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function task(ByVal task_Conflict As Task) As Builder
				Me.task_Conflict = task_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify the TokenizerFactory to use.
			''' For BERT, typically <seealso cref="org.deeplearning4j.text.tokenization.tokenizerfactory.BertWordPieceTokenizerFactory"/>
			''' is used
			''' </summary>
			Public Overridable Function tokenizer(ByVal tokenizerFactory As TokenizerFactory) As Builder
				Me.tokenizerFactory = tokenizerFactory
				Return Me
			End Function

			''' <summary>
			''' Specifies how the sequence length of the output data should be handled. See <seealso cref="BertIterator"/> for more details.
			''' </summary>
			''' <param name="lengthHandling"> Length handling </param>
			''' <param name="maxLength">      Not used if LengthHandling is set to <seealso cref="LengthHandling.ANY_LENGTH"/>
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder lengthHandling(@NonNull LengthHandling lengthHandling, int maxLength)
'JAVA TO VB CONVERTER NOTE: The parameter lengthHandling was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function lengthHandling(ByVal lengthHandling_Conflict As LengthHandling, ByVal maxLength As Integer) As Builder
				Me.lengthHandling_Conflict = lengthHandling_Conflict
				Me.maxTokens = maxLength
				Return Me
			End Function

			''' <summary>
			''' Minibatch size to use (number of examples to train on for each iteration)
			''' See also: <seealso cref="padMinibatches"/>
			''' </summary>
			''' <param name="minibatchSize"> Minibatch size </param>
'JAVA TO VB CONVERTER NOTE: The parameter minibatchSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minibatchSize(ByVal minibatchSize_Conflict As Integer) As Builder
				Me.minibatchSize_Conflict = minibatchSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Default: false (disabled)<br>
			''' If the dataset is not an exact multiple of the minibatch size, should we pad the smaller final minibatch?<br>
			''' For example, if we have 100 examples total, and 32 minibatch size, the following number of examples will be returned
			''' for subsequent calls of next() in the one epoch:<br>
			''' padMinibatches = false (default): 32, 32, 32, 4.<br>
			''' padMinibatches = true: 32, 32, 32, 32 (note: the last minibatch will have 4 real examples, and 28 masked out padding examples).<br>
			''' Both options should result in exactly the same model. However, some BERT implementations may require exactly an
			''' exact number of examples in all minibatches to function.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter padMinibatches was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function padMinibatches(ByVal padMinibatches_Conflict As Boolean) As Builder
				Me.padMinibatches_Conflict = padMinibatches_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the preprocessor to be used on the MultiDataSets before returning them. Default: none (null)
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter preProcessor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function preProcessor(ByVal preProcessor_Conflict As MultiDataSetPreProcessor) As Builder
				Me.preProcessor_Conflict = preProcessor_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify the source of the data for classification.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter sentenceProvider was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sentenceProvider(ByVal sentenceProvider_Conflict As LabeledSentenceProvider) As Builder
				Me.sentenceProvider_Conflict = sentenceProvider_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify the source of the data for classification on sentence pairs.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter sentencePairProvider was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sentencePairProvider(ByVal sentencePairProvider_Conflict As LabeledPairSentenceProvider) As Builder
				Me.sentencePairProvider_Conflict = sentencePairProvider_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify what arrays should be returned. See <seealso cref="BertIterator"/> for more details.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter featureArrays was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function featureArrays(ByVal featureArrays_Conflict As FeatureArrays) As Builder
				Me.featureArrays_Conflict = featureArrays_Conflict
				Return Me
			End Function

			''' <summary>
			''' Provide the vocabulary as a map. Keys are the words in the vocabulary, and values are the indices of those
			''' words. For indices, they should be in range 0 to vocabMap.size()-1 inclusive.<br>
			''' If using <seealso cref="BertWordPieceTokenizerFactory"/>,
			''' this can be obtained using <seealso cref="BertWordPieceTokenizerFactory.getVocab()"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter vocabMap was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function vocabMap(ByVal vocabMap_Conflict As IDictionary(Of String, Integer)) As Builder
				Me.vocabMap_Conflict = vocabMap_Conflict
				Return Me
			End Function

			''' <summary>
			''' Used only for unsupervised training (i.e., when task is set to <seealso cref="Task.UNSUPERVISED"/> for learning a
			''' masked language model. This can be used to customize how the masking is performed.<br>
			''' Default: <seealso cref="BertMaskedLMMasker"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter masker was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function masker(ByVal masker_Conflict As BertSequenceMasker) As Builder
				Me.masker_Conflict = masker_Conflict
				Return Me
			End Function

			''' <summary>
			''' Used only for unsupervised training (i.e., when task is set to <seealso cref="Task.UNSUPERVISED"/> for learning a
			''' masked language model. Used to specify the format that the labels should be returned in.
			''' See <seealso cref="BertIterator"/> for more details.
			''' </summary>
			Public Overridable Function unsupervisedLabelFormat(ByVal labelFormat As UnsupervisedLabelFormat) As Builder
				Me.unsupervisedLabelFormat_Conflict = labelFormat
				Return Me
			End Function

			''' <summary>
			''' Used only for unsupervised training (i.e., when task is set to <seealso cref="Task.UNSUPERVISED"/> for learning a
			''' masked language model. This specifies the token (such as "[MASK]") that should be used when a value is masked out.
			''' Note that this is passed to the <seealso cref="BertSequenceMasker"/> defined by <seealso cref="masker(BertSequenceMasker)"/> hence
			''' the exact behaviour will depend on what masker is used.<br>
			''' Note that this must be in the vocabulary map set in <seealso cref="vocabMap"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter maskToken was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maskToken(ByVal maskToken_Conflict As String) As Builder
				Me.maskToken_Conflict = maskToken_Conflict
				Return Me
			End Function

			''' <summary>
			''' Prepend the specified token to the sequences, when doing supervised training.<br>
			''' i.e., any token sequences will have this added at the start.<br>
			''' Some BERT/Transformer models may need this - for example sequences starting with a "[CLS]" token.<br>
			''' No token is prepended by default.
			''' </summary>
			''' <param name="prependToken"> The token to start each sequence with (null: no token will be prepended) </param>
'JAVA TO VB CONVERTER NOTE: The parameter prependToken was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function prependToken(ByVal prependToken_Conflict As String) As Builder
				Me.prependToken_Conflict = prependToken_Conflict
				Return Me
			End Function

			''' <summary>
			''' Append the specified token to the sequences, when doing training on sentence pairs.<br>
			''' Generally "[SEP]" is used
			''' No token in appended by default.
			''' </summary>
			''' <param name="appendToken"> Token at end of each sentence for pairs of sentences (null: no token will be appended)
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter appendToken was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function appendToken(ByVal appendToken_Conflict As String) As Builder
				Me.appendToken_Conflict = appendToken_Conflict
				Return Me
			End Function

			Public Overridable Function build() As BertIterator
				Preconditions.checkState(task_Conflict <> Nothing, "No task has been set. Use .task(BertIterator.Task.X) to set the task to be performed")
				Preconditions.checkState(tokenizerFactory IsNot Nothing, "No tokenizer factory has been set. A tokenizer factory (such as BertWordPieceTokenizerFactory) is required")
				Preconditions.checkState(vocabMap_Conflict IsNot Nothing, "Cannot create iterator: No vocabMap has been set. Use Builder.vocabMap(Map<String,Integer>) to set")
				Preconditions.checkState(task_Conflict <> Task.UNSUPERVISED OrElse masker_Conflict IsNot Nothing, "If task is UNSUPERVISED training, a masker must be set via masker(BertSequenceMasker) method")
				Preconditions.checkState(task_Conflict <> Task.UNSUPERVISED OrElse unsupervisedLabelFormat_Conflict <> Nothing, "If task is UNSUPERVISED training, a label format must be set via masker(BertSequenceMasker) method")
				Preconditions.checkState(task_Conflict <> Task.UNSUPERVISED OrElse maskToken_Conflict IsNot Nothing, "If task is UNSUPERVISED training, the mask token in the vocab (such as ""[MASK]"" must be specified")
				If sentencePairProvider_Conflict IsNot Nothing Then
					Preconditions.checkState(task_Conflict = Task.SEQ_CLASSIFICATION, "Currently only supervised sequence classification is set up with sentence pairs. "".task(BertIterator.Task.SEQ_CLASSIFICATION)"" is required with a sentence pair provider")
					Preconditions.checkState(featureArrays_Conflict = FeatureArrays.INDICES_MASK_SEGMENTID, "Currently only supervised sequence classification is set up with sentence pairs. "".featureArrays(FeatureArrays.INDICES_MASK_SEGMENTID)"" is required with a sentence pair provider")
					Preconditions.checkState(lengthHandling_Conflict = LengthHandling.FIXED_LENGTH, "Currently only fixed length is supported for sentence pairs. "".lengthHandling(BertIterator.LengthHandling.FIXED_LENGTH, maxLength)"" is required with a sentence pair provider")
					Preconditions.checkState(sentencePairProvider_Conflict IsNot Nothing, "Provide either a sentence provider or a sentence pair provider. Both cannot be non null")
				End If
				If appendToken_Conflict IsNot Nothing Then
					Preconditions.checkState(sentencePairProvider_Conflict IsNot Nothing, "Tokens are only appended with sentence pairs. Sentence pair provider is not set. Set sentence pair provider.")
				End If
				Return New BertIterator(Me)
			End Function
		End Class

		Private Class SentencePairListProcessed
			Friend listLength As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private long[] segIdOnesFrom;
			Friend segIdOnesFrom() As Long
			Friend cursor As Integer = 0
			Friend sentenceListProcessed As SentenceListProcessed

			Friend Sub New(ByVal listLength As Integer)
				Me.listLength = listLength
				segIdOnesFrom = New Long(listLength - 1){}
				sentenceListProcessed = New SentenceListProcessed(listLength)
			End Sub

			Friend Overridable Sub addProcessedToList(ByVal segIdIdx As Long, ByVal tokenizedSentencePairAndLabel As Pair(Of IList(Of String), String))
				segIdOnesFrom(cursor) = segIdIdx
				sentenceListProcessed.addProcessedToList(tokenizedSentencePairAndLabel)
				cursor += 1
			End Sub

			Friend Overridable Property MaxL As Integer
				Set(ByVal maxL As Integer)
					sentenceListProcessed.setMaxL(maxL)
				End Set
				Get
					Return sentenceListProcessed.getMaxL()
				End Get
			End Property


			Friend Overridable ReadOnly Property TokensAndLabelList As IList(Of Pair(Of IList(Of String), String))
				Get
					Return sentenceListProcessed.getTokensAndLabelList()
				End Get
			End Property
		End Class

		Private Class SentenceListProcessed
			Friend listLength As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private int maxL;
			Friend maxL As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.List<org.nd4j.common.primitives.Pair<java.util.List<String>, String>> tokensAndLabelList;
			Friend tokensAndLabelList As IList(Of Pair(Of IList(Of String), String))

			Friend Sub New(ByVal listLength As Integer)
				Me.listLength = listLength
				tokensAndLabelList = New List(Of Pair(Of IList(Of String), String))(listLength)
			End Sub

			Friend Overridable Sub addProcessedToList(ByVal tokenizedSentenceAndLabel As Pair(Of IList(Of String), String))
				tokensAndLabelList.Add(tokenizedSentenceAndLabel)
			End Sub
		End Class
	End Class

End Namespace