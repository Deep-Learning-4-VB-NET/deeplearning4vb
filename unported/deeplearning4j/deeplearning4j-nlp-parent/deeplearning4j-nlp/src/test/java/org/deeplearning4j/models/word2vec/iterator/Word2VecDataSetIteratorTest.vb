Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ParagraphVectorsTest = org.deeplearning4j.models.paragraphvectors.ParagraphVectorsTest
Imports org.deeplearning4j.models.embeddings.learning.impl.elements
Imports org.deeplearning4j.models.embeddings.reader.impl
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports Word2Vec = org.deeplearning4j.models.word2vec.Word2Vec
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports SentencePreProcessor = org.deeplearning4j.text.sentenceiterator.SentencePreProcessor
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator
Imports CommonPreprocessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Resources = org.nd4j.common.resources.Resources
import static org.junit.jupiter.api.Assertions.assertArrayEquals

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

Namespace org.deeplearning4j.models.word2vec.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class Word2VecDataSetIteratorTest extends org.deeplearning4j.BaseDL4JTest
	Public Class Word2VecDataSetIteratorTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 60000L
			End Get
		End Property

		''' <summary>
		''' Basically all we want from this test - being able to finish without exceptions.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIterator1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIterator1()

			Dim inputFile As File = Resources.asFile("big/raw_sentences.txt")
			Dim iter As SentenceIterator = ParagraphVectorsTest.getIterator(IntegrationTests, inputFile)
	'        SentenceIterator iter = new BasicLineIterator(inputFile.getAbsolutePath());

			Dim t As TokenizerFactory = New DefaultTokenizerFactory()
			t.TokenPreProcessor = New CommonPreprocessor()

			Dim vec As Word2Vec = (New Word2Vec.Builder()).minWordFrequency(10).iterations(1).learningRate(0.025).layerSize(150).seed(42).sampling(0).negativeSample(0).useHierarchicSoftmax(True).windowSize(5).modelUtils(New BasicModelUtils(Of VocabWord)()).useAdaGrad(False).iterate(iter).workers(8).tokenizerFactory(t).elementsLearningAlgorithm(New CBOW(Of VocabWord)()).build()

			vec.fit()

			Dim labels As IList(Of String) = New List(Of String)()
			labels.Add("positive")
			labels.Add("negative")

			Dim iterator As New Word2VecDataSetIterator(vec, getLASI(iter, labels), labels, 1)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim array As INDArray = iterator.next().getFeatures()
			Dim count As Integer = 0
			Do While iterator.MoveNext()
				Dim ds As DataSet = iterator.Current

				assertArrayEquals(array.shape(), ds.Features.shape())

'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: if(!isIntegrationTests() && count++ > 20)
				If Not IntegrationTests AndAlso count++ > 20 Then
					Exit Do 'raw_sentences.txt is 2.81 MB, takes quite some time to process. We'll only first 20 minibatches when doing unit tests
				End If
			Loop
		End Sub

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator getLASI(final org.deeplearning4j.text.sentenceiterator.SentenceIterator iterator, final java.util.List<String> labels)
		Protected Friend Overridable Function getLASI(ByVal iterator As SentenceIterator, ByVal labels As IList(Of String)) As LabelAwareSentenceIterator
			iterator.reset()

			Return New LabelAwareSentenceIteratorAnonymousInnerClass(Me)
		End Function

		Private Class LabelAwareSentenceIteratorAnonymousInnerClass
			Implements LabelAwareSentenceIterator

			Private ReadOnly outerInstance As Word2VecDataSetIteratorTest

			Public Sub New(ByVal outerInstance As Word2VecDataSetIteratorTest)
				Me.outerInstance = outerInstance
				cnt = New AtomicInteger(0)
			End Sub

			Private cnt As AtomicInteger

			Public Function currentLabel() As String Implements LabelAwareSentenceIterator.currentLabel
				Return labels.get(cnt.incrementAndGet() Mod labels.size())
			End Function

			Public Function currentLabels() As IList(Of String) Implements LabelAwareSentenceIterator.currentLabels
				Return Collections.singletonList(currentLabel())
			End Function

			Public Function nextSentence() As String
				Return iterator.nextSentence()
			End Function

			Public Function hasNext() As Boolean
				Return iterator.hasNext()
			End Function

			Public Sub reset()
				iterator.reset()
			End Sub

			Public Sub finish()
				iterator.finish()
			End Sub

			Public Property PreProcessor As SentencePreProcessor
				Get
					Return iterator.getPreProcessor()
				End Get
				Set(ByVal preProcessor As SentencePreProcessor)
					iterator.setPreProcessor(preProcessor)
				End Set
			End Property

		End Class
	End Class

End Namespace