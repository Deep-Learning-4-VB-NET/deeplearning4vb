Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports NonNull = lombok.NonNull
Imports LabelAwareConverter = org.deeplearning4j.iterator.provider.LabelAwareConverter
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports LabelAwareDocumentIterator = org.deeplearning4j.text.documentiterator.LabelAwareDocumentIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports DocumentIteratorConverter = org.deeplearning4j.text.documentiterator.interoperability.DocumentIteratorConverter
Imports SentenceIteratorConverter = org.deeplearning4j.text.sentenceiterator.interoperability.SentenceIteratorConverter
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class CnnSentenceDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class CnnSentenceDataSetIterator
		Implements DataSetIterator

		Public Enum UnknownWordHandling
			RemoveWord
			UseUnknownVector
		End Enum

		''' <summary>
		''' Format of features:<br>
		''' CNN1D: For use with 1d convolution layers: Shape [minibatch, vectorSize, sentenceLength]<br>
		''' CNN2D: For use with 2d convolution layers: Shape [minibatch, 1, vectorSize, sentenceLength] or [minibatch, 1, sentenceLength, vectorSize],
		''' depending on the setting for 'sentencesAlongHeight' configuration.
		''' </summary>
		Public Enum Format
			RNN
			CNN1D
			CNN2D
		End Enum

		Private Const UNKNOWN_WORD_SENTINEL As String = "UNKNOWN_WORD_SENTINEL"

		Private format As Format
		Private sentenceProvider As LabeledSentenceProvider
		Private wordVectors As WordVectors
		Private tokenizerFactory As TokenizerFactory
		Private unknownWordHandling As UnknownWordHandling
		Private useNormalizedWordVectors As Boolean
		Private minibatchSize As Integer
		Private maxSentenceLength As Integer
		Private sentencesAlongHeight As Boolean
		Private dataSetPreProcessor As DataSetPreProcessor

		Private wordVectorSize As Integer
		Private numClasses As Integer
		Private labelClassMap As IDictionary(Of String, Integer)
		Private unknown As INDArray

		Private cursor As Integer = 0

		Private preLoadedTokens As Pair(Of IList(Of String), String)

		Protected Friend Sub New(ByVal builder As Builder)
			Me.format = builder.format
			Me.sentenceProvider = builder.sentenceProvider_Conflict
			Me.wordVectors = builder.wordVectors_Conflict
			Me.tokenizerFactory = builder.tokenizerFactory_Conflict
			Me.unknownWordHandling = builder.unknownWordHandling_Conflict
			Me.useNormalizedWordVectors = builder.useNormalizedWordVectors_Conflict
			Me.minibatchSize = builder.minibatchSize_Conflict
			Me.maxSentenceLength = builder.maxSentenceLength_Conflict
			Me.sentencesAlongHeight = builder.sentencesAlongHeight_Conflict
			Me.dataSetPreProcessor = builder.dataSetPreProcessor_Conflict


			Me.numClasses = Me.sentenceProvider.numLabelClasses()
			Me.labelClassMap = New Dictionary(Of String, Integer)()
			Dim count As Integer = 0
			'First: sort the labels to ensure the same label assignment order (say train vs. test)
			Dim sortedLabels As IList(Of String) = New List(Of String)(Me.sentenceProvider.allLabels())
			sortedLabels.Sort()

			Me.wordVectorSize = wordVectors.getWordVector(wordVectors.vocab().wordAtIndex(0)).Length

			For Each s As String In sortedLabels
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: this.labelClassMap.put(s, count++);
				Me.labelClassMap(s) = count
					count += 1
			Next s
			If unknownWordHandling = UnknownWordHandling.UseUnknownVector Then
				If useNormalizedWordVectors Then
					unknown = wordVectors.getWordVectorMatrixNormalized(wordVectors.UNK)
				Else
					unknown = wordVectors.getWordVectorMatrix(wordVectors.UNK)
				End If

				If unknown Is Nothing Then
					unknown = wordVectors.getWordVectorMatrix(wordVectors.vocab().wordAtIndex(0)).like()
				End If
			End If
		End Sub

		''' <summary>
		''' Generally used post training time to load a single sentence for predictions
		''' </summary>
		Public Overridable Function loadSingleSentence(ByVal sentence As String) As INDArray
			Dim tokens As IList(Of String) = tokenizeSentence(sentence)
			If tokens.Count = 0 Then
				Throw New System.InvalidOperationException("No tokens available for input sentence - empty string or no words in vocabulary with RemoveWord unknown handling? Sentence = """ & sentence & """")
			End If
			If format = Format.CNN1D OrElse format = Format.RNN Then
				Dim featuresShape() As Integer = {1, wordVectorSize, Math.Min(maxSentenceLength, tokens.Count)}
				Dim features As INDArray = Nd4j.create(featuresShape, (If(format = Format.CNN1D, "c"c, "f"c)))
				Dim indices(2) As INDArrayIndex
				indices(0) = NDArrayIndex.point(0)
				Dim i As Integer = 0
				Do While i < featuresShape(2)
					Dim vector As INDArray = getVector(tokens(i))
					indices(1) = NDArrayIndex.all()
					indices(2) = NDArrayIndex.point(i)
					features.put(indices, vector)
					i += 1
				Loop
				Return features
			Else
				Dim featuresShape() As Integer = {1, 1, 0, 0}
				If sentencesAlongHeight Then
					featuresShape(2) = Math.Min(maxSentenceLength, tokens.Count)
					featuresShape(3) = wordVectorSize
				Else
					featuresShape(2) = wordVectorSize
					featuresShape(3) = Math.Min(maxSentenceLength, tokens.Count)
				End If

				Dim features As INDArray = Nd4j.create(featuresShape)
				Dim length As Integer = (If(sentencesAlongHeight, featuresShape(2), featuresShape(3)))
				Dim indices(3) As INDArrayIndex
				indices(0) = NDArrayIndex.point(0)
				indices(1) = NDArrayIndex.point(0)
				For i As Integer = 0 To length - 1
					Dim vector As INDArray = getVector(tokens(i))

					If sentencesAlongHeight Then
						indices(2) = NDArrayIndex.point(i)
						indices(3) = NDArrayIndex.all()
					Else
						indices(2) = NDArrayIndex.all()
						indices(3) = NDArrayIndex.point(i)
					End If

					features.put(indices, vector)
				Next i

				Return features
			End If
		End Function

		Private Function getVector(ByVal word As String) As INDArray
			Dim vector As INDArray
			If unknownWordHandling = UnknownWordHandling.UseUnknownVector AndAlso String.ReferenceEquals(word, UNKNOWN_WORD_SENTINEL) Then 'Yes, this *should* be using == for the sentinel String here
				vector = unknown
			Else
				If useNormalizedWordVectors Then
					vector = wordVectors.getWordVectorMatrixNormalized(word)
				Else
					vector = wordVectors.getWordVectorMatrix(word)
				End If
			End If
			Return vector
		End Function

		Private Function tokenizeSentence(ByVal sentence As String) As IList(Of String)
			Dim t As Tokenizer = tokenizerFactory.create(sentence)

			Dim tokens As IList(Of String) = New List(Of String)()
			Do While t.hasMoreTokens()
				Dim token As String = t.nextToken()
				If Not wordVectors.outOfVocabularySupported() AndAlso Not wordVectors.hasWord(token) Then
					Select Case unknownWordHandling
						Case org.deeplearning4j.iterator.CnnSentenceDataSetIterator.UnknownWordHandling.RemoveWord
							Continue Do
						Case org.deeplearning4j.iterator.CnnSentenceDataSetIterator.UnknownWordHandling.UseUnknownVector
							token = UNKNOWN_WORD_SENTINEL
					End Select
				End If
				tokens.Add(token)
			Loop
			Return tokens
		End Function

		Public Overridable ReadOnly Property LabelClassMap As IDictionary(Of String, Integer)
			Get
				Return New Dictionary(Of String, Integer)(labelClassMap)
			End Get
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String)
			Get
				'We don't want to just return the list from the LabelledSentenceProvider, as we sorted them earlier to do the
				' String -> Integer mapping
				Dim str(labelClassMap.Count - 1) As String
				For Each e As KeyValuePair(Of String, Integer) In labelClassMap.SetOfKeyValuePairs()
					str(e.Value) = e.Key
				Next e
				Return New List(Of String) From {str}
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			If sentenceProvider Is Nothing Then
				Throw New System.NotSupportedException("Cannot do next/hasNext without a sentence provider")
			End If

			Do While preLoadedTokens Is Nothing AndAlso sentenceProvider.hasNext()
				'Pre-load tokens. Because we filter out empty strings, or sentences with no valid words
				'we need to pre-load some tokens. Otherwise, sentenceProvider could have 1 (invalid) sentence
				'next, hasNext() would return true, but next(int) wouldn't be able to return anything
				preLoadTokens()
			Loop

			Return preLoadedTokens IsNot Nothing
		End Function

		Private Sub preLoadTokens()
			If preLoadedTokens IsNot Nothing Then
				Return
			End If
			Dim p As Pair(Of String, String) = sentenceProvider.nextSentence()
			Dim tokens As IList(Of String) = tokenizeSentence(p.First)
			If tokens.Count > 0 Then
				preLoadedTokens = New Pair(Of IList(Of String), String)(tokens, p.Second)
			End If
		End Sub

		Public Overrides Function [next]() As DataSet
			Return [next](minibatchSize)
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			If sentenceProvider Is Nothing Then
				Throw New System.NotSupportedException("Cannot do next/hasNext without a sentence provider")
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If


			Dim tokenizedSentences As IList(Of Pair(Of IList(Of String), String)) = New List(Of Pair(Of IList(Of String), String))(num)
			Dim maxLength As Integer = -1
			Dim minLength As Integer = Integer.MaxValue 'Track to we know if we can skip mask creation for "all same length" case
			If preLoadedTokens IsNot Nothing Then
				tokenizedSentences.Add(preLoadedTokens)
				maxLength = Math.Max(maxLength, preLoadedTokens.First.Count)
				minLength = Math.Min(minLength, preLoadedTokens.First.Count)
				preLoadedTokens = Nothing
			End If
			Dim i As Integer = tokenizedSentences.Count
			Do While i < num AndAlso sentenceProvider.hasNext()
				Dim p As Pair(Of String, String) = sentenceProvider.nextSentence()
				Dim tokens As IList(Of String) = tokenizeSentence(p.First)

				If tokens.Count > 0 Then
					'Handle edge case: no tokens from sentence
					maxLength = Math.Max(maxLength, tokens.Count)
					minLength = Math.Min(minLength, tokens.Count)
					tokenizedSentences.Add(New Pair(Of IList(Of String), String)(tokens, p.Second))
				Else
					'Skip the current iterator
					i -= 1
				End If
				i += 1
			Loop

			If maxSentenceLength > 0 AndAlso maxLength > maxSentenceLength Then
				maxLength = maxSentenceLength
			End If

			Dim currMinibatchSize As Integer = tokenizedSentences.Count
			Dim labels As INDArray = Nd4j.create(currMinibatchSize, numClasses)
			For i As Integer = 0 To tokenizedSentences.Count - 1
				Dim labelStr As String = tokenizedSentences(i).getSecond()
				If Not labelClassMap.ContainsKey(labelStr) Then
					Throw New System.InvalidOperationException("Got label """ & labelStr & """ that is not present in list of LabeledSentenceProvider labels")
				End If

				Dim labelIdx As Integer = labelClassMap(labelStr)

				labels.putScalar(i, labelIdx, 1.0)
			Next i

			Dim features As INDArray
			Dim featuresMask As INDArray = Nothing
			If format = Format.CNN1D OrElse format = Format.RNN Then
				Dim featuresShape() As Integer = {currMinibatchSize, wordVectorSize, maxLength}
				features = Nd4j.create(featuresShape, (If(format = Format.CNN1D, "c"c, "f"c)))

				Dim idxs(2) As INDArrayIndex
				idxs(1) = NDArrayIndex.all()
				For i As Integer = 0 To currMinibatchSize - 1
					idxs(0) = NDArrayIndex.point(i)
					Dim currSentence As IList(Of String) = tokenizedSentences(i).getFirst()
					Dim j As Integer = 0
					Do While j < currSentence.Count AndAlso j < maxSentenceLength
						idxs(2) = NDArrayIndex.point(j)
						Dim vector As INDArray = getVector(currSentence(j))
						features.put(idxs, vector)
						j += 1
					Loop
				Next i

				If minLength <> maxLength Then
					featuresMask = Nd4j.create(currMinibatchSize, maxLength)
					For i As Integer = 0 To currMinibatchSize - 1
						Dim sentenceLength As Integer = tokenizedSentences(i).getFirst().size()
						If sentenceLength >= maxLength Then
							featuresMask.getRow(i).assign(1.0)
						Else
							featuresMask.get(NDArrayIndex.point(i), NDArrayIndex.interval(0, sentenceLength)).assign(1.0)
						End If
					Next i
				End If

			Else
				Dim featuresShape(3) As Integer
				featuresShape(0) = currMinibatchSize
				featuresShape(1) = 1
				If sentencesAlongHeight Then
					featuresShape(2) = maxLength
					featuresShape(3) = wordVectorSize
				Else
					featuresShape(2) = wordVectorSize
					featuresShape(3) = maxLength
				End If

				features = Nd4j.create(featuresShape)
				Dim indices(3) As INDArrayIndex
				indices(1) = NDArrayIndex.point(0)
				For i As Integer = 0 To currMinibatchSize - 1
					indices(0) = NDArrayIndex.point(i)
					Dim currSentence As IList(Of String) = tokenizedSentences(i).getFirst()
					Dim j As Integer = 0
					Do While j < currSentence.Count AndAlso j < maxSentenceLength
						Dim vector As INDArray = getVector(currSentence(j))

						If sentencesAlongHeight Then
							indices(2) = NDArrayIndex.point(j)
							indices(3) = NDArrayIndex.all()
						Else
							indices(2) = NDArrayIndex.all()
							indices(3) = NDArrayIndex.point(j)
						End If

						features.put(indices, vector)
						j += 1
					Loop
				Next i

				If minLength <> maxLength Then
					If sentencesAlongHeight Then
						featuresMask = Nd4j.create(currMinibatchSize, 1, maxLength, 1)
						For i As Integer = 0 To currMinibatchSize - 1
							Dim sentenceLength As Integer = tokenizedSentences(i).getFirst().size()
							If sentenceLength >= maxLength Then
								featuresMask.slice(i).assign(1.0)
							Else
								featuresMask.get(NDArrayIndex.point(i), NDArrayIndex.point(0), NDArrayIndex.interval(0, sentenceLength), NDArrayIndex.point(0)).assign(1.0)
							End If
						Next i
					Else
						featuresMask = Nd4j.create(currMinibatchSize, 1, 1, maxLength)
						For i As Integer = 0 To currMinibatchSize - 1
							Dim sentenceLength As Integer = tokenizedSentences(i).getFirst().size()
							If sentenceLength >= maxLength Then
								featuresMask.slice(i).assign(1.0)
							Else
								featuresMask.get(NDArrayIndex.point(i), NDArrayIndex.point(0), NDArrayIndex.point(0), NDArrayIndex.interval(0, sentenceLength)).assign(1.0)
							End If
						Next i
					End If
				End If
			End If

			Dim ds As New DataSet(features, labels, featuresMask, Nothing)

			If dataSetPreProcessor IsNot Nothing Then
				dataSetPreProcessor.preProcess(ds)
			End If

			cursor += ds.numExamples()
			Return ds
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return wordVectorSize
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return numClasses
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			cursor = 0
			sentenceProvider.reset()
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return minibatchSize
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.dataSetPreProcessor = preProcessor
			End Set
			Get
				Return dataSetPreProcessor
			End Get
		End Property


		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Class Builder

			Friend format As Format
'JAVA TO VB CONVERTER NOTE: The field sentenceProvider was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend sentenceProvider_Conflict As LabeledSentenceProvider = Nothing
'JAVA TO VB CONVERTER NOTE: The field wordVectors was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend wordVectors_Conflict As WordVectors
'JAVA TO VB CONVERTER NOTE: The field tokenizerFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend tokenizerFactory_Conflict As TokenizerFactory = New DefaultTokenizerFactory()
'JAVA TO VB CONVERTER NOTE: The field unknownWordHandling was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend unknownWordHandling_Conflict As UnknownWordHandling = UnknownWordHandling.RemoveWord
'JAVA TO VB CONVERTER NOTE: The field useNormalizedWordVectors was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend useNormalizedWordVectors_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field maxSentenceLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend maxSentenceLength_Conflict As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field minibatchSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minibatchSize_Conflict As Integer = 32
'JAVA TO VB CONVERTER NOTE: The field sentencesAlongHeight was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend sentencesAlongHeight_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field dataSetPreProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend dataSetPreProcessor_Conflict As DataSetPreProcessor

			''' @deprecated Due to old default, that will be changed in the future. Use <seealso cref="Builder(Format)"/> to specify
			''' the <seealso cref="Format"/> of the activations 
			<Obsolete("Due to old default, that will be changed in the future. Use <seealso cref=""Builder(Format)""/> to specify")>
			Public Sub New()
				'Default for backward compatibility
				Me.New(Format.CNN2D)
			End Sub

			''' <param name="format"> The format to use for the features - i.e., for 1D or 2D CNNs </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull Format format)
			Public Sub New(ByVal format As Format)
				Me.format = format
			End Sub

			''' <summary>
			''' Specify how the (labelled) sentences / documents should be provided
			''' </summary>
			Public Overridable Function sentenceProvider(ByVal labeledSentenceProvider As LabeledSentenceProvider) As Builder
				Me.sentenceProvider_Conflict = labeledSentenceProvider
				Return Me
			End Function

			''' <summary>
			''' Specify how the (labelled) sentences / documents should be provided
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder sentenceProvider(org.deeplearning4j.text.documentiterator.LabelAwareIterator iterator, @NonNull List<String> labels)
			Public Overridable Function sentenceProvider(ByVal iterator As LabelAwareIterator, ByVal labels As IList(Of String)) As Builder
				Dim converter As New LabelAwareConverter(iterator, labels)
				Return sentenceProvider(converter)
			End Function

			''' <summary>
			''' Specify how the (labelled) sentences / documents should be provided
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder sentenceProvider(org.deeplearning4j.text.documentiterator.LabelAwareDocumentIterator iterator, @NonNull List<String> labels)
			Public Overridable Function sentenceProvider(ByVal iterator As LabelAwareDocumentIterator, ByVal labels As IList(Of String)) As Builder
				Dim converter As New DocumentIteratorConverter(iterator)
				Return sentenceProvider(converter, labels)
			End Function

			''' <summary>
			''' Specify how the (labelled) sentences / documents should be provided
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder sentenceProvider(org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator iterator, @NonNull List<String> labels)
			Public Overridable Function sentenceProvider(ByVal iterator As LabelAwareSentenceIterator, ByVal labels As IList(Of String)) As Builder
				Dim converter As New SentenceIteratorConverter(iterator)
				Return sentenceProvider(converter, labels)
			End Function


			''' <summary>
			''' Provide the WordVectors instance that should be used for training
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter wordVectors was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function wordVectors(ByVal wordVectors_Conflict As WordVectors) As Builder
				Me.wordVectors_Conflict = wordVectors_Conflict
				Return Me
			End Function

			''' <summary>
			''' The <seealso cref="TokenizerFactory"/> that should be used. Defaults to <seealso cref="DefaultTokenizerFactory"/>
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter tokenizerFactory was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tokenizerFactory(ByVal tokenizerFactory_Conflict As TokenizerFactory) As Builder
				Me.tokenizerFactory_Conflict = tokenizerFactory_Conflict
				Return Me
			End Function

			''' <summary>
			''' Specify how unknown words (those that don't have a word vector in the provided WordVectors instance) should be
			''' handled. Default: remove/ignore unknown words.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter unknownWordHandling was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function unknownWordHandling(ByVal unknownWordHandling_Conflict As UnknownWordHandling) As Builder
				Me.unknownWordHandling_Conflict = unknownWordHandling_Conflict
				Return Me
			End Function

			''' <summary>
			''' Minibatch size to use for the DataSetIterator
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter minibatchSize was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minibatchSize(ByVal minibatchSize_Conflict As Integer) As Builder
				Me.minibatchSize_Conflict = minibatchSize_Conflict
				Return Me
			End Function

			''' <summary>
			''' Whether normalized word vectors should be used. Default: true
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter useNormalizedWordVectors was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function useNormalizedWordVectors(ByVal useNormalizedWordVectors_Conflict As Boolean) As Builder
				Me.useNormalizedWordVectors_Conflict = useNormalizedWordVectors_Conflict
				Return Me
			End Function

			''' <summary>
			''' Maximum sentence/document length. If sentences exceed this, they will be truncated to this length by
			''' taking the first 'maxSentenceLength' known words.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter maxSentenceLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maxSentenceLength(ByVal maxSentenceLength_Conflict As Integer) As Builder
				Me.maxSentenceLength_Conflict = maxSentenceLength_Conflict
				Return Me
			End Function

			''' <summary>
			''' If true (default): output features data with shape [minibatchSize, 1, maxSentenceLength, wordVectorSize]<br>
			''' If false: output features with shape [minibatchSize, 1, wordVectorSize, maxSentenceLength]
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter sentencesAlongHeight was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function sentencesAlongHeight(ByVal sentencesAlongHeight_Conflict As Boolean) As Builder
				Me.sentencesAlongHeight_Conflict = sentencesAlongHeight_Conflict
				Return Me
			End Function

			''' <summary>
			''' Optional DataSetPreProcessor
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter dataSetPreProcessor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function dataSetPreProcessor(ByVal dataSetPreProcessor_Conflict As DataSetPreProcessor) As Builder
				Me.dataSetPreProcessor_Conflict = dataSetPreProcessor_Conflict
				Return Me
			End Function

			Public Overridable Function build() As CnnSentenceDataSetIterator
				If wordVectors_Conflict Is Nothing Then
					Throw New System.InvalidOperationException("Cannot build CnnSentenceDataSetIterator without a WordVectors instance")
				End If

				Return New CnnSentenceDataSetIterator(Me)
			End Function

		End Class
	End Class

End Namespace