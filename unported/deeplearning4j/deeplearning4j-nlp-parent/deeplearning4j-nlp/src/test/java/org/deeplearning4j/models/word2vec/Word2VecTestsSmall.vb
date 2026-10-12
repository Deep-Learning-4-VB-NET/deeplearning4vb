Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ParagraphVectorsTest = org.deeplearning4j.models.paragraphvectors.ParagraphVectorsTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports EmbeddingLayer = org.deeplearning4j.nn.conf.layers.EmbeddingLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports FileLabelAwareIterator = org.deeplearning4j.text.documentiterator.FileLabelAwareIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports CommonPreprocessor = org.deeplearning4j.text.tokenization.tokenizer.preprocessor.CommonPreprocessor
Imports DefaultTokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.DefaultTokenizerFactory
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Tag = org.junit.jupiter.api.Tag
Imports Timeout = org.junit.jupiter.api.Timeout
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports WordVectorSerializer = org.deeplearning4j.models.embeddings.loader.WordVectorSerializer
Imports WordVectors = org.deeplearning4j.models.embeddings.wordvectors.WordVectors
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Resources = org.nd4j.common.resources.Resources
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @NativeTag public class Word2VecTestsSmall extends org.deeplearning4j.BaseDL4JTest
	Public Class Word2VecTestsSmall
		Inherits BaseDL4JTest

		Friend word2vec As WordVectors

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return If(IntegrationTests, 240000, 60000)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			word2vec = WordVectorSerializer.readWord2VecModel((New ClassPathResource("vec.bin")).File)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWordsNearest2VecTxt()
		Public Overridable Sub testWordsNearest2VecTxt()
			Dim word As String = "Adam"
			Dim expectedNeighbour As String = "is"
			Dim neighbours As Integer = 1

			Dim nearestWords As ICollection(Of String) = word2vec.wordsNearest(word, neighbours)
			Console.WriteLine(nearestWords)
			assertEquals(expectedNeighbour, nearestWords.GetEnumerator().next())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWordsNearest2NNeighbours()
		Public Overridable Sub testWordsNearest2NNeighbours()
			Dim word As String = "Adam"
			Dim neighbours As Integer = 2

			Dim nearestWords As ICollection(Of String) = word2vec.wordsNearest(word, neighbours)
			Console.WriteLine(nearestWords)
			assertEquals(neighbours, nearestWords.Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) public void testUnkSerialization_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUnkSerialization_1()
			Dim inputFile As val = Resources.asFile("big/raw_sentences.txt")
	'        val iter = new BasicLineIterator(inputFile);
			Dim iter As val = ParagraphVectorsTest.getIterator(IntegrationTests, inputFile)
			Dim t As val = New DefaultTokenizerFactory()
			t.setTokenPreProcessor(New CommonPreprocessor())

			Dim vec As val = (New Word2Vec.Builder()).minWordFrequency(1).epochs(1).layerSize(300).limitVocabularySize(1).windowSize(5).allowParallelTokenization(True).batchSize(512).learningRate(0.025).minLearningRate(0.0001).negativeSample(0.0).sampling(0.0).useAdaGrad(False).useHierarchicSoftmax(True).iterations(1).useUnknown(True).seed(42).iterate(iter).workers(4).tokenizerFactory(t).build()

			vec.fit()

			Dim tmpFile As val = File.createTempFile("temp","temp")
			tmpFile.deleteOnExit()

			WordVectorSerializer.writeWord2VecModel(vec, tmpFile) ' NullPointerException was thrown here
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLabelAwareIterator_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testLabelAwareIterator_1()
			Dim resource As val = New ClassPathResource("/labeled")
			Dim file As val = resource.getFile()

			Dim iter As val = DirectCast((New FileLabelAwareIterator.Builder()).addSourceFolder(file).build(), LabelAwareIterator)

			Dim t As val = New DefaultTokenizerFactory()

			Dim w2v As val = (New Word2Vec.Builder()).iterate(iter).tokenizerFactory(t).build()

			' we hope nothing is going to happen here
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPlot()
		Public Overridable Sub testPlot()
			'word2vec.lookupTable().plotVocab();
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) public void testW2VEmbeddingLayerInit() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testW2VEmbeddingLayerInit()
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)

			Dim inputFile As val = Resources.asFile("big/raw_sentences.txt")
			Dim iter As val = ParagraphVectorsTest.getIterator(IntegrationTests, inputFile)
	'        val iter = new BasicLineIterator(inputFile);
			Dim t As val = New DefaultTokenizerFactory()
			t.setTokenPreProcessor(New CommonPreprocessor())

			Dim vec As Word2Vec = (New Word2Vec.Builder()).minWordFrequency(1).epochs(1).layerSize(300).limitVocabularySize(1).windowSize(5).allowParallelTokenization(True).batchSize(512).learningRate(0.025).minLearningRate(0.0001).negativeSample(0.0).sampling(0.0).useAdaGrad(False).useHierarchicSoftmax(True).iterations(1).useUnknown(True).seed(42).iterate(iter).workers(4).tokenizerFactory(t).build()

			vec.fit()

			Dim w As INDArray = vec.lookupTable().getWeights()
			Console.WriteLine(w)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer((New EmbeddingLayer.Builder()).weightInit(vec).build()).layer((New DenseLayer.Builder()).activation(Activation.TANH).nIn(w.size(1)).nOut(3).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(3).nOut(4).build()).build()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork net = new org.deeplearning4j.nn.multilayer.MultiLayerNetwork(conf);
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim w0 As INDArray = net.getParam("0_W")
			assertEquals(w, w0)

			Dim baos As New MemoryStream()
			ModelSerializer.writeModel(net, baos, True)
			Dim bytes() As SByte = baos.toByteArray()

			Dim bais As New MemoryStream(bytes)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork restored = org.deeplearning4j.util.ModelSerializer.restoreMultiLayerNetwork(bais, true);
			Dim restored As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(bais, True)

			assertEquals(net.LayerWiseConfigurations, restored.LayerWiseConfigurations)
			assertTrue(net.params().equalsWithEps(restored.params(), 2e-3))
		End Sub
	End Class

End Namespace