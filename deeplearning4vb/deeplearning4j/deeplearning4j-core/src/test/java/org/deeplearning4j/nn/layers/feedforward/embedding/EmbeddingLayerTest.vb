Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports EmbeddingLayer = org.deeplearning4j.nn.conf.layers.EmbeddingLayer
Imports EmbeddingSequenceLayer = org.deeplearning4j.nn.conf.layers.EmbeddingSequenceLayer
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToFeedForwardPreProcessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports EmbeddingInitializer = org.deeplearning4j.nn.weights.embeddings.EmbeddingInitializer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.nn.layers.feedforward.embedding

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Embedding Layer Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class EmbeddingLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class EmbeddingLayerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Layer Config") void testEmbeddingLayerConfig()
		Friend Overridable Sub testEmbeddingLayerConfig()
			For Each hasBias As Boolean In New Boolean() { True, False }
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer(0, (New EmbeddingLayer.Builder()).hasBias(hasBias).nIn(10).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nIn(5).nOut(4).activation(Activation.SOFTMAX).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim l0 As Layer = net.getLayer(0)
				assertEquals(GetType(org.deeplearning4j.nn.layers.feedforward.embedding.EmbeddingLayer), l0.GetType())
				assertEquals(10, CType(l0.conf().getLayer(), FeedForwardLayer).getNIn())
				assertEquals(5, CType(l0.conf().getLayer(), FeedForwardLayer).getNOut())
				Dim weights As INDArray = l0.getParam(DefaultParamInitializer.WEIGHT_KEY)
				Dim bias As INDArray = l0.getParam(DefaultParamInitializer.BIAS_KEY)
				assertArrayEquals(New Long() { 10, 5 }, weights.shape())
				If hasBias Then
					assertArrayEquals(New Long() { 1, 5 }, bias.shape())
				End If
			Next hasBias
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Sequence Layer Config") void testEmbeddingSequenceLayerConfig()
		Friend Overridable Sub testEmbeddingSequenceLayerConfig()
			Dim inputLength As Integer = 6
			Dim nIn As Integer = 10
			Dim embeddingDim As Integer = 5
			Dim nout As Integer = 4
			For Each hasBias As Boolean In New Boolean() { True, False }
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer((New EmbeddingSequenceLayer.Builder()).hasBias(hasBias).inputLength(inputLength).nIn(nIn).nOut(embeddingDim).build()).layer((New RnnOutputLayer.Builder()).nIn(embeddingDim).nOut(nout).activation(Activation.SOFTMAX).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim l0 As Layer = net.getLayer(0)
				assertEquals(GetType(org.deeplearning4j.nn.layers.feedforward.embedding.EmbeddingSequenceLayer), l0.GetType())
				assertEquals(10, CType(l0.conf().getLayer(), FeedForwardLayer).getNIn())
				assertEquals(5, CType(l0.conf().getLayer(), FeedForwardLayer).getNOut())
				Dim weights As INDArray = l0.getParam(DefaultParamInitializer.WEIGHT_KEY)
				Dim bias As INDArray = l0.getParam(DefaultParamInitializer.BIAS_KEY)
				assertArrayEquals(New Long() { 10, 5 }, weights.shape())
				If hasBias Then
					assertArrayEquals(New Long() { 1, 5 }, bias.shape())
				End If
			Next hasBias
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Longer Sequences Forward Pass") void testEmbeddingLongerSequencesForwardPass()
		Friend Overridable Sub testEmbeddingLongerSequencesForwardPass()
			Dim nClassesIn As Integer = 10
			Dim inputLength As Integer = 6
			Dim embeddingDim As Integer = 5
			Dim nOut As Integer = 4
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer((New EmbeddingSequenceLayer.Builder()).inputLength(inputLength).hasBias(True).nIn(nClassesIn).nOut(embeddingDim).build()).layer((New RnnOutputLayer.Builder()).nIn(embeddingDim).nOut(nOut).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim batchSize As Integer = 3
			Dim inEmbedding As INDArray = Nd4j.create(batchSize, inputLength)
			Dim r As New Random(12345)
			For i As Integer = 0 To batchSize - 1
				Dim classIdx As Integer = r.Next(nClassesIn)
				inEmbedding.putScalar(i, classIdx)
			Next i
			Dim output As INDArray = net.output(inEmbedding)
			assertArrayEquals(New Long() { batchSize, nOut, inputLength }, output.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Single Sequence Forward Pass") void testEmbeddingSingleSequenceForwardPass()
		Friend Overridable Sub testEmbeddingSingleSequenceForwardPass()
			Dim nClassesIn As Integer = 10
			Dim embeddingDim As Integer = 5
			Dim nOut As Integer = 4
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer((New EmbeddingSequenceLayer.Builder()).inputLength(1).hasBias(True).nIn(nClassesIn).nOut(embeddingDim).build()).layer((New RnnOutputLayer.Builder()).nIn(embeddingDim).nOut(nOut).activation(Activation.SOFTMAX).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer(0, (New DenseLayer.Builder()).nIn(nClassesIn).nOut(5).activation(Activation.IDENTITY).build()).layer(1, (New OutputLayer.Builder()).nIn(5).nOut(4).activation(Activation.SOFTMAX).build()).inputPreProcessor(0, New RnnToFeedForwardPreProcessor()).build()
			Dim net As New MultiLayerNetwork(conf)
			Dim net2 As New MultiLayerNetwork(conf2)
			net.init()
			net2.init()
			net2.Params = net.params().dup()
			Dim batchSize As Integer = 3
			Dim inEmbedding As INDArray = Nd4j.create(batchSize, 1)
			Dim inOneHot As INDArray = Nd4j.create(batchSize, nClassesIn, 1)
			Dim r As New Random(12345)
			For i As Integer = 0 To batchSize - 1
				Dim classIdx As Integer = r.Next(nClassesIn)
				inEmbedding.putScalar(i, classIdx)
				inOneHot.putScalar(New Integer() { i, classIdx, 0 }, 1.0)
			Next i
			Dim activationsDense As IList(Of INDArray) = net2.feedForward(inOneHot, False)
			Dim activationEmbedding As IList(Of INDArray) = net.feedForward(inEmbedding, False)
			Dim actD1 As INDArray = activationsDense(1)
			Dim actE1 As INDArray = activationEmbedding(1).reshape(batchSize, embeddingDim)
			assertEquals(actD1, actE1)
			Dim actD2 As INDArray = activationsDense(2)
			Dim actE2 As INDArray = activationEmbedding(2).reshape(batchSize, nOut)
			assertEquals(actD2, actE2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Forward Pass") void testEmbeddingForwardPass()
		Friend Overridable Sub testEmbeddingForwardPass()
			' With the same parameters, embedding layer should have same activations as the equivalent one-hot representation
			' input with a DenseLayer
			Dim nClassesIn As Integer = 10
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer(0, (New EmbeddingLayer.Builder()).hasBias(True).nIn(nClassesIn).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nIn(5).nOut(4).activation(Activation.SOFTMAX).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer(0, (New DenseLayer.Builder()).nIn(nClassesIn).nOut(5).activation(Activation.IDENTITY).build()).layer(1, (New OutputLayer.Builder()).nIn(5).nOut(4).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			Dim net2 As New MultiLayerNetwork(conf2)
			net.init()
			net2.init()
			net2.Params = net.params().dup()
			Dim batchSize As Integer = 3
			Dim inEmbedding As INDArray = Nd4j.create(batchSize, 1)
			Dim inOneHot As INDArray = Nd4j.create(batchSize, nClassesIn)
			Dim r As New Random(12345)
			For i As Integer = 0 To batchSize - 1
				Dim classIdx As Integer = r.Next(nClassesIn)
				inEmbedding.putScalar(i, classIdx)
				inOneHot.putScalar(New Integer() { i, classIdx }, 1.0)
			Next i
			Dim activationsEmbedding As IList(Of INDArray) = net.feedForward(inEmbedding, False)
			Dim activationsDense As IList(Of INDArray) = net2.feedForward(inOneHot, False)
			For i As Integer = 1 To 2
				Dim actE As INDArray = activationsEmbedding(i)
				Dim actD As INDArray = activationsDense(i)
				assertEquals(actE, actD)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Backward Pass") void testEmbeddingBackwardPass()
		Friend Overridable Sub testEmbeddingBackwardPass()
			' With the same parameters, embedding layer should have same activations as the equivalent one-hot representation
			' input with a DenseLayer
			Dim nClassesIn As Integer = 10
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer(0, (New EmbeddingLayer.Builder()).hasBias(True).nIn(nClassesIn).nOut(5).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(5).nOut(4).activation(Activation.SOFTMAX).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer((New DenseLayer.Builder()).nIn(nClassesIn).nOut(5).activation(Activation.IDENTITY).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(5).nOut(4).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			Dim net2 As New MultiLayerNetwork(conf2)
			net.init()
			net2.init()
			net2.Params = net.params().dup()
			Dim batchSize As Integer = 3
			Dim inEmbedding As INDArray = Nd4j.create(batchSize, 1)
			Dim inOneHot As INDArray = Nd4j.create(batchSize, nClassesIn)
			Dim outLabels As INDArray = Nd4j.create(batchSize, 4)
			Dim r As New Random(12345)
			For i As Integer = 0 To batchSize - 1
				Dim classIdx As Integer = r.Next(nClassesIn)
				inEmbedding.putScalar(i, classIdx)
				inOneHot.putScalar(New Integer() { i, classIdx }, 1.0)
				Dim labelIdx As Integer = r.Next(4)
				outLabels.putScalar(New Integer() { i, labelIdx }, 1.0)
			Next i
			net.Input = inEmbedding
			net2.Input = inOneHot
			net.Labels = outLabels
			net2.Labels = outLabels
			net.computeGradientAndScore()
			net2.computeGradientAndScore()
			assertEquals(net2.score(), net.score(), 1e-6)
			Dim gradient As IDictionary(Of String, INDArray) = net.gradient().gradientForVariable()
			Dim gradient2 As IDictionary(Of String, INDArray) = net2.gradient().gradientForVariable()
			assertEquals(gradient.Count, gradient2.Count)
			For Each s As String In gradient.Keys
				assertEquals(gradient2(s), gradient(s))
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Sequence Backward Pass") void testEmbeddingSequenceBackwardPass()
		Friend Overridable Sub testEmbeddingSequenceBackwardPass()
			Dim nClassesIn As Integer = 10
			Dim embeddingDim As Integer = 5
			Dim nOut As Integer = 4
			Dim inputLength As Integer = 1
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer((New EmbeddingSequenceLayer.Builder()).inputLength(inputLength).hasBias(True).nIn(nClassesIn).nOut(embeddingDim).build()).layer((New RnnOutputLayer.Builder()).nIn(embeddingDim).nOut(nOut).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(nClassesIn, inputLength, RNNFormat.NCW)).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).list().layer((New DenseLayer.Builder()).nIn(nClassesIn).nOut(embeddingDim).activation(Activation.IDENTITY).build()).layer((New RnnOutputLayer.Builder()).nIn(embeddingDim).nOut(nOut).activation(Activation.SOFTMAX).build()).setInputType(InputType.recurrent(nClassesIn, inputLength, RNNFormat.NCW)).build()
			Dim net As New MultiLayerNetwork(conf)
			Dim net2 As New MultiLayerNetwork(conf2)
			net.init()
			net2.init()
			net2.Params = net.params().dup()
			Dim batchSize As Integer = 3
			Dim inEmbedding As INDArray = Nd4j.create(batchSize, 1)
			Dim inOneHot As INDArray = Nd4j.create(batchSize, nClassesIn, 1)
			Dim outLabels As INDArray = Nd4j.create(batchSize, 4, 1)
			Dim r As New Random(1337)
			For i As Integer = 0 To batchSize - 1
				Dim classIdx As Integer = r.Next(nClassesIn)
				inEmbedding.putScalar(i, classIdx)
				inOneHot.putScalar(New Integer() { i, classIdx, 0 }, 1.0)
				Dim labelIdx As Integer = r.Next(4)
				outLabels.putScalar(New Integer() { i, labelIdx, 0 }, 1.0)
			Next i
			net.Input = inEmbedding
			net2.Input = inOneHot
			net.Labels = outLabels
			net2.Labels = outLabels
			net.computeGradientAndScore()
			net2.computeGradientAndScore()
			' System.out.println(net.score() + "\t" + net2.score());
			assertEquals(net2.score(), net.score(), 1e-6)
			Dim gradient As IDictionary(Of String, INDArray) = net.gradient().gradientForVariable()
			Dim gradient2 As IDictionary(Of String, INDArray) = net2.gradient().gradientForVariable()
			assertEquals(gradient.Count, gradient2.Count)
			For Each s As String In gradient.Keys
				assertEquals(gradient2(s), gradient(s))
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Layer RNN") void testEmbeddingLayerRNN()
		Friend Overridable Sub testEmbeddingLayerRNN()
			Dim nClassesIn As Integer = 10
			Dim batchSize As Integer = 3
			Dim timeSeriesLength As Integer = 8
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).dataType(DataType.DOUBLE).list().layer(0, (New EmbeddingLayer.Builder()).hasBias(True).nIn(nClassesIn).nOut(5).build()).layer(1, (New LSTM.Builder()).nIn(5).nOut(7).activation(Activation.SOFTSIGN).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(7).nOut(4).activation(Activation.SOFTMAX).build()).inputPreProcessor(0, New RnnToFeedForwardPreProcessor()).inputPreProcessor(1, New FeedForwardToRnnPreProcessor()).setInputType(InputType.recurrent(nClassesIn, timeSeriesLength, RNNFormat.NCW)).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).dataType(DataType.DOUBLE).list().layer(0, (New DenseLayer.Builder()).nIn(nClassesIn).nOut(5).activation(Activation.IDENTITY).build()).layer(1, (New LSTM.Builder()).nIn(5).nOut(7).activation(Activation.SOFTSIGN).build()).layer(2, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(7).nOut(4).activation(Activation.SOFTMAX).build()).inputPreProcessor(0, New RnnToFeedForwardPreProcessor()).inputPreProcessor(1, New FeedForwardToRnnPreProcessor()).setInputType(InputType.recurrent(nClassesIn, timeSeriesLength, RNNFormat.NCW)).build()
			Dim net As New MultiLayerNetwork(conf)
			Dim net2 As New MultiLayerNetwork(conf2)
			net.init()
			net2.init()
			net2.Params = net.params().dup()

			Dim inEmbedding As INDArray = Nd4j.create(batchSize, 1, timeSeriesLength)
			Dim inOneHot As INDArray = Nd4j.create(batchSize, nClassesIn, timeSeriesLength)
			Dim outLabels As INDArray = Nd4j.create(batchSize, 4, timeSeriesLength)
			Dim r As New Random(12345)
			For i As Integer = 0 To batchSize - 1
				For j As Integer = 0 To timeSeriesLength - 1
					Dim classIdx As Integer = r.Next(nClassesIn)
					inEmbedding.putScalar(New Integer() { i, 0, j }, classIdx)
					inOneHot.putScalar(New Integer() { i, classIdx, j }, 1.0)
					Dim labelIdx As Integer = r.Next(4)
					outLabels.putScalar(New Integer() { i, labelIdx, j }, 1.0)
				Next j
			Next i
			net.Input = inEmbedding
			net2.Input = inOneHot
			net.Labels = outLabels
			net2.Labels = outLabels
			net.computeGradientAndScore()
			net2.computeGradientAndScore()
			' System.out.println(net.score() + "\t" + net2.score());
			assertEquals(net2.score(), net.score(), 1e-5)
			Dim gradient As IDictionary(Of String, INDArray) = net.gradient().gradientForVariable()
			Dim gradient2 As IDictionary(Of String, INDArray) = net2.gradient().gradientForVariable()
			assertEquals(gradient.Count, gradient2.Count)
			For Each s As String In gradient.Keys
				assertEquals(gradient2(s), gradient(s))
			Next s
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Layer With Masking") void testEmbeddingLayerWithMasking()
		Friend Overridable Sub testEmbeddingLayerWithMasking()
			' Idea: have masking on the input with an embedding and dense layers on input
			' Ensure that the parameter gradients for the inputs don't depend on the inputs when inputs are masked
			Dim miniBatchSizes() As Integer = { 1, 2, 5 }
			Dim nIn As Integer = 2
			Dim r As New Random(12345)
			Dim numInputClasses As Integer = 10
			Dim timeSeriesLength As Integer = 5
			For Each maskDtype As DataType In New DataType() { DataType.FLOAT, DataType.DOUBLE, DataType.INT }
				For Each nExamples As Integer In miniBatchSizes
					Nd4j.Random.setSeed(12345)
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).list().layer(0, (New EmbeddingLayer.Builder()).hasBias(True).activation(Activation.TANH).nIn(numInputClasses).nOut(5).build()).layer(1, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(5).nOut(4).build()).layer(2, (New LSTM.Builder()).activation(Activation.TANH).nIn(4).nOut(3).build()).layer(3, (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(3).nOut(4).build()).inputPreProcessor(0, New RnnToFeedForwardPreProcessor()).inputPreProcessor(2, New FeedForwardToRnnPreProcessor()).setInputType(InputType.recurrent(numInputClasses, timeSeriesLength, RNNFormat.NCW)).build()
					Dim net As New MultiLayerNetwork(conf)
					net.init()
					Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).list().layer(0, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(numInputClasses).nOut(5).build()).layer(1, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(5).nOut(4).build()).layer(2, (New LSTM.Builder()).activation(Activation.TANH).nIn(4).nOut(3).build()).layer(3, (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(3).nOut(4).build()).inputPreProcessor(0, New RnnToFeedForwardPreProcessor()).inputPreProcessor(2, New FeedForwardToRnnPreProcessor()).setInputType(InputType.recurrent(numInputClasses, timeSeriesLength, RNNFormat.NCW)).build()
					Dim net2 As New MultiLayerNetwork(conf2)
					net2.init()
					net2.Params = net.params().dup()
					Dim inEmbedding As INDArray = Nd4j.zeros(nExamples, 1, timeSeriesLength)
					Dim inDense As INDArray = Nd4j.zeros(nExamples, numInputClasses, timeSeriesLength)
					Dim labels As INDArray = Nd4j.zeros(nExamples, 4, timeSeriesLength)
					For i As Integer = 0 To nExamples - 1
						For j As Integer = 0 To timeSeriesLength - 1
							Dim inIdx As Integer = r.Next(numInputClasses)
							inEmbedding.putScalar(New Integer() { i, 0, j }, inIdx)
							inDense.putScalar(New Integer() { i, inIdx, j }, 1.0)
							Dim outIdx As Integer = r.Next(4)
							labels.putScalar(New Integer() { i, outIdx, j }, 1.0)
						Next j
					Next i
					Dim inputMask As INDArray = Nd4j.zeros(maskDtype, nExamples, timeSeriesLength)
					For i As Integer = 0 To nExamples - 1
						For j As Integer = 0 To timeSeriesLength - 1
							inputMask.putScalar(New Integer() { i, j }, (If(r.nextBoolean(), 1.0, 0.0)))
						Next j
					Next i
					net.setLayerMaskArrays(inputMask, Nothing)
					net2.setLayerMaskArrays(inputMask, Nothing)
					Dim actEmbedding As IList(Of INDArray) = net.feedForward(inEmbedding, False)
					Dim actDense As IList(Of INDArray) = net2.feedForward(inDense, False)
					For i As Integer = 1 To actEmbedding.Count - 1
						assertEquals(actDense(i), actEmbedding(i))
					Next i
					net.Labels = labels
					net2.Labels = labels
					net.computeGradientAndScore()
					net2.computeGradientAndScore()
					' System.out.println(net.score() + "\t" + net2.score());
					assertEquals(net2.score(), net.score(), 1e-5)
					Dim gradients As IDictionary(Of String, INDArray) = net.gradient().gradientForVariable()
					Dim gradients2 As IDictionary(Of String, INDArray) = net2.gradient().gradientForVariable()
					assertEquals(gradients.Keys, gradients2.Keys)
					For Each s As String In gradients.Keys
						assertEquals(gradients2(s), gradients(s))
					Next s
				Next nExamples
			Next maskDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test W 2 V Inits") void testW2VInits()
		Friend Overridable Sub testW2VInits()
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			For i As Integer = 0 To 1
				Dim vectors As INDArray = Nd4j.linspace(1, 15, 15, DataType.FLOAT).reshape(ChrW(5), 3)
				Dim el As EmbeddingLayer
				If i = 0 Then
					el = (New EmbeddingLayer.Builder()).weightInit(vectors).build()
				Else
					el = (New EmbeddingLayer.Builder()).weightInit(New WordVectorsMockup()).build()
				End If
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(el).layer((New DenseLayer.Builder()).activation(Activation.TANH).nIn(3).nOut(3).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(3).nOut(4).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
				Dim w As INDArray = net.getParam("0_W")
				assertEquals(vectors, w)
				TestUtils.testModelSerialization(net)
				' Test same thing for embedding sequence layer:
				Dim esl As EmbeddingSequenceLayer
				If i = 0 Then
					esl = (New EmbeddingSequenceLayer.Builder()).weightInit(vectors).build()
				Else
					esl = (New EmbeddingSequenceLayer.Builder()).weightInit(New WordVectorsMockup()).build()
				End If
				conf = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(esl).layer(New GlobalPoolingLayer()).layer((New DenseLayer.Builder()).activation(Activation.TANH).nIn(3).nOut(3).build()).layer((New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(3).nOut(4).build()).build()
				net = New MultiLayerNetwork(conf)
				net.init()
				w = net.getParam("0_W")
				assertEquals(vectors, w)
				TestUtils.testModelSerialization(net)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Sequence Layer With Masking") void testEmbeddingSequenceLayerWithMasking()
		Friend Overridable Sub testEmbeddingSequenceLayerWithMasking()
			' Idea: have masking on the input with an embedding and dense layers on input
			' Ensure that the parameter gradients for the inputs don't depend on the inputs when inputs are masked
			Dim miniBatchSizes() As Integer = { 1, 3 }
			Dim nIn As Integer = 2
			Dim r As New Random(12345)
			Dim numInputClasses As Integer = 10
			Dim timeSeriesLength As Integer = 5
			For Each maskDtype As DataType In New DataType() { DataType.FLOAT, DataType.DOUBLE, DataType.INT }
				For Each inLabelDtype As DataType In New DataType() { DataType.FLOAT, DataType.DOUBLE, DataType.INT }
					For Each inputRank As Integer In New Integer() { 2, 3 }
						For Each nExamples As Integer In miniBatchSizes
							Nd4j.Random.setSeed(12345)
							Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).list().layer(0, (New EmbeddingSequenceLayer.Builder()).hasBias(True).activation(Activation.TANH).nIn(numInputClasses).nOut(5).build()).layer(1, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(5).nOut(4).build()).layer(2, (New LSTM.Builder()).activation(Activation.TANH).nIn(4).nOut(3).build()).layer(3, (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(3).nOut(4).build()).setInputType(InputType.recurrent(numInputClasses, timeSeriesLength, RNNFormat.NCW)).build()
							Dim net As New MultiLayerNetwork(conf)
							net.init()
							Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).seed(12345).list().layer(0, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(numInputClasses).nOut(5).build()).layer(1, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(5).nOut(4).build()).layer(2, (New LSTM.Builder()).activation(Activation.TANH).nIn(4).nOut(3).dataFormat(RNNFormat.NCW).build()).layer(3, (New RnnOutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(3).nOut(4).build()).setInputType(InputType.recurrent(numInputClasses, 1, RNNFormat.NCW)).build()
							Dim net2 As New MultiLayerNetwork(conf2)
							net2.init()
							net2.Params = net.params().dup()
							Dim inEmbedding As INDArray = Nd4j.zeros(inLabelDtype,If(inputRank = 2, New Long() { nExamples, timeSeriesLength }, New Long()){ nExamples, 1, timeSeriesLength })
							Dim inDense As INDArray = Nd4j.zeros(inLabelDtype, nExamples, numInputClasses, timeSeriesLength)
							Dim labels As INDArray = Nd4j.zeros(inLabelDtype, nExamples, 4, timeSeriesLength)
							For i As Integer = 0 To nExamples - 1
								For j As Integer = 0 To timeSeriesLength - 1
									Dim inIdx As Integer = r.Next(numInputClasses)
									inEmbedding.putScalar(If(inputRank = 2, New Integer() { i, j }, New Integer()){ i, 0, j }, inIdx)
									inDense.putScalar(New Integer() { i, inIdx, j }, 1.0)
									Dim outIdx As Integer = r.Next(4)
									labels.putScalar(New Integer() { i, outIdx, j }, 1.0)
								Next j
							Next i
							Dim inputMask As INDArray = Nd4j.zeros(maskDtype, nExamples, timeSeriesLength)
							For i As Integer = 0 To nExamples - 1
								For j As Integer = 0 To timeSeriesLength - 1
									inputMask.putScalar(New Integer() { i, j }, (If(r.nextBoolean(), 1.0, 0.0)))
								Next j
							Next i
							net.setLayerMaskArrays(inputMask, Nothing)
							net2.setLayerMaskArrays(inputMask, Nothing)
							Dim actEmbedding As IList(Of INDArray) = net.feedForward(inEmbedding, False)
							Dim actDense As IList(Of INDArray) = net2.feedForward(inDense, False)
							For i As Integer = 2 To actEmbedding.Count - 1
								' Start from layer 2: EmbeddingSequence is 3d, first dense is 2d (before reshape)
								assertEquals(actDense(i), actEmbedding(i))
							Next i
							net.Labels = labels
							net2.Labels = labels
							net.computeGradientAndScore()
							net2.computeGradientAndScore()
							assertEquals(net2.score(), net.score(), 1e-5)
							Dim gradients As IDictionary(Of String, INDArray) = net.gradient().gradientForVariable()
							Dim gradients2 As IDictionary(Of String, INDArray) = net2.gradient().gradientForVariable()
							assertEquals(gradients.Keys, gradients2.Keys)
							For Each s As String In gradients.Keys
								assertEquals(gradients2(s), gradients(s))
							Next s
						Next nExamples
					Next inputRank
				Next inLabelDtype
			Next maskDtype
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @DisplayName("Word Vectors Mockup") private static class WordVectorsMockup implements org.deeplearning4j.nn.weights.embeddings.EmbeddingInitializer
		<Serializable>
		Private Class WordVectorsMockup
			Implements EmbeddingInitializer

			Public Overridable Sub loadWeightsInto(ByVal array As INDArray) Implements EmbeddingInitializer.loadWeightsInto
				Dim vectors As INDArray = Nd4j.linspace(1, 15, 15, DataType.FLOAT).reshape(ChrW(5), 3)
				array.assign(vectors)
			End Sub

			Public Overridable Function vocabSize() As Long Implements EmbeddingInitializer.vocabSize
				Return 5
			End Function

			Public Overridable Function vectorSize() As Integer Implements EmbeddingInitializer.vectorSize
				Return 3
			End Function

			Public Overridable Function jsonSerializable() As Boolean Implements EmbeddingInitializer.jsonSerializable
				Return True
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Default Activation") void testEmbeddingDefaultActivation()
		Friend Overridable Sub testEmbeddingDefaultActivation()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New EmbeddingLayer.Builder()).nIn(10).nOut(10).build()).layer((New EmbeddingSequenceLayer.Builder()).nIn(10).nOut(10).build()).build()
			Dim l As EmbeddingLayer = CType(conf.getConf(0).getLayer(), EmbeddingLayer)
			assertEquals(New ActivationIdentity(), l.getActivationFn())
			Dim l2 As EmbeddingSequenceLayer = CType(conf.getConf(1).getLayer(), EmbeddingSequenceLayer)
			assertEquals(New ActivationIdentity(), l2.getActivationFn())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Embedding Weight Init") void testEmbeddingWeightInit()
		Friend Overridable Sub testEmbeddingWeightInit()
			' https://github.com/eclipse/deeplearning4j/issues/8663
			' The embedding layer weight initialization should be independent of the vocabulary size (nIn setting)
			For Each wi As WeightInit In New WeightInit() { WeightInit.XAVIER, WeightInit.RELU, WeightInit.XAVIER_UNIFORM, WeightInit.LECUN_NORMAL }
				For Each seq As Boolean In New Boolean() { False, True }
					Nd4j.Random.setSeed(12345)
					Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(If(seq, (New EmbeddingSequenceLayer.Builder()).weightInit(wi).nIn(100).nOut(100).build(), (New EmbeddingLayer.Builder()).weightInit(wi).nIn(100).nOut(100).build())).build()
					Dim net As New MultiLayerNetwork(conf)
					net.init()
					Nd4j.Random.setSeed(12345)
					Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(If(seq, (New EmbeddingSequenceLayer.Builder()).weightInit(wi).nIn(100).nOut(100).build(), (New EmbeddingLayer.Builder()).weightInit(wi).nIn(100).nOut(100).build())).build()
					Dim net2 As New MultiLayerNetwork(conf2)
					net2.init()
					Nd4j.Random.setSeed(12345)
					Dim conf3 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(If(seq, (New EmbeddingSequenceLayer.Builder()).weightInit(wi).nIn(100000).nOut(100).build(), (New EmbeddingLayer.Builder()).weightInit(wi).nIn(100000).nOut(100).build())).build()
					Dim net3 As New MultiLayerNetwork(conf3)
					net3.init()
					Dim p1 As INDArray = net.params()
					Dim p2 As INDArray = net2.params()
					Dim p3 As INDArray = net3.params()
					Dim eq As Boolean = p1.equalsWithEps(p2, 1e-4)
					Dim str As String = (If(seq, "EmbeddingSequenceLayer", "EmbeddingLayer")) & " - " & wi
					assertTrue(eq,str & " p1/p2 params not equal")
					Dim m1 As Double = p1.meanNumber().doubleValue()
					Dim s1 As Double = p1.stdNumber().doubleValue()
					Dim m3 As Double = p3.meanNumber().doubleValue()
					Dim s3 As Double = p3.stdNumber().doubleValue()
					assertEquals(m1, m3, 0.1,str)
					assertEquals(s1, s3, 0.1,str)
					Dim re As Double = relErr(s1, s3)
					assertTrue(re < 0.05,str & " - " & re)
				Next seq
			Next wi
		End Sub

		Public Shared Function relErr(ByVal d1 As Double, ByVal d2 As Double) As Double
			If d1 = 0.0 AndAlso d2 = 0.0 Then
				Return 0.0
			End If
			Return Math.Abs(d1 - d2) / (Math.Abs(d1) + Math.Abs(d2))
		End Function
	End Class

End Namespace