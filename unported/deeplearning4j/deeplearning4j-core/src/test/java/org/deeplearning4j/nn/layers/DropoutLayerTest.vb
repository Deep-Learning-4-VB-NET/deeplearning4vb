Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Dropout = org.deeplearning4j.nn.conf.dropout.Dropout
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports DropoutLayer = org.deeplearning4j.nn.conf.layers.DropoutLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNull
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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Dropout Layer Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class DropoutLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class DropoutLayerTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Types") void testInputTypes()
		Friend Overridable Sub testInputTypes()
			Dim config As DropoutLayer = (New DropoutLayer.Builder(0.5)).build()
			Dim in1 As InputType = InputType.feedForward(20)
			Dim in2 As InputType = InputType.convolutional(28, 28, 1)
			assertEquals(in1, config.getOutputType(0, in1))
			assertEquals(in2, config.getOutputType(0, in2))
			assertNull(config.getPreProcessorForInputType(in1))
			assertNull(config.getPreProcessorForInputType(in2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dropout Layer Without Training") @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) void testDropoutLayerWithoutTraining() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testDropoutLayerWithoutTraining()
			Dim confIntegrated As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(3648).list().layer(0, (New ConvolutionLayer.Builder(1, 1)).stride(1, 1).nIn(1).nOut(1).dropOut(0.25).activation(Activation.IDENTITY).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).weightInit(WeightInit.XAVIER).dropOut(0.25).nOut(4).build()).setInputType(InputType.convolutionalFlat(2, 2, 1)).build()
			Dim netIntegrated As New MultiLayerNetwork(confIntegrated)
			netIntegrated.init()
			netIntegrated.getLayer(0).setParam("W", Nd4j.eye(1))
			netIntegrated.getLayer(0).setParam("b", Nd4j.zeros(1, 1))
			netIntegrated.getLayer(1).setParam("W", Nd4j.eye(4))
			netIntegrated.getLayer(1).setParam("b", Nd4j.zeros(4, 1))
			Dim confSeparate As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(3648).list().layer(0, (New DropoutLayer.Builder(0.25)).build()).layer(1, (New ConvolutionLayer.Builder(1, 1)).stride(1, 1).nIn(1).nOut(1).activation(Activation.IDENTITY).weightInit(WeightInit.XAVIER).build()).layer(2, (New DropoutLayer.Builder(0.25)).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nOut(4).build()).setInputType(InputType.convolutionalFlat(2, 2, 1)).build()
			Dim netSeparate As New MultiLayerNetwork(confSeparate)
			netSeparate.init()
			netSeparate.getLayer(1).setParam("W", Nd4j.eye(1))
			netSeparate.getLayer(1).setParam("b", Nd4j.zeros(1, 1))
			netSeparate.getLayer(3).setParam("W", Nd4j.eye(4))
			netSeparate.getLayer(3).setParam("b", Nd4j.zeros(4, 1))
			' Disable input modification for this test:
			For Each l As Layer In netIntegrated.Layers
				l.allowInputModification(False)
			Next l
			For Each l As Layer In netSeparate.Layers
				l.allowInputModification(False)
			Next l
			Dim [in] As INDArray = Nd4j.arange(1, 5).reshape(ChrW(1), 4)
			Nd4j.Random.setSeed(12345)
			Dim actTrainIntegrated As IList(Of INDArray) = netIntegrated.feedForward([in].dup(), True)
			Nd4j.Random.setSeed(12345)
			Dim actTrainSeparate As IList(Of INDArray) = netSeparate.feedForward([in].dup(), True)
			Nd4j.Random.setSeed(12345)
			Dim actTestIntegrated As IList(Of INDArray) = netIntegrated.feedForward([in].dup(), False)
			Nd4j.Random.setSeed(12345)
			Dim actTestSeparate As IList(Of INDArray) = netSeparate.feedForward([in].dup(), False)
			' Check masks:
			Dim maskIntegrated As INDArray = CType(netIntegrated.getLayer(0).conf().getLayer().getIDropout(), Dropout).getMask()
			Dim maskSeparate As INDArray = CType(netSeparate.getLayer(0).conf().getLayer().getIDropout(), Dropout).getMask()
			assertEquals(maskIntegrated, maskSeparate)
			assertEquals(actTrainIntegrated(1), actTrainSeparate(2))
			assertEquals(actTrainIntegrated(2), actTrainSeparate(4))
			assertEquals(actTestIntegrated(1), actTestSeparate(2))
			assertEquals(actTestIntegrated(2), actTestSeparate(4))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dropout Layer With Dense Mnist") void testDropoutLayerWithDenseMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testDropoutLayerWithDenseMnist()
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			' Run without separate activation layer
			Dim confIntegrated As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28 * 1).nOut(10).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).dropOut(0.25).nIn(10).nOut(10).build()).build()
			Dim netIntegrated As New MultiLayerNetwork(confIntegrated)
			netIntegrated.init()
			netIntegrated.fit([next])
			' Run with separate activation layer
			Dim confSeparate As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28 * 1).nOut(10).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New DropoutLayer.Builder(0.25)).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim netSeparate As New MultiLayerNetwork(confSeparate)
			netSeparate.init()
			netSeparate.fit([next])
			' Disable input modification for this test:
			For Each l As Layer In netIntegrated.Layers
				l.allowInputModification(False)
			Next l
			For Each l As Layer In netSeparate.Layers
				l.allowInputModification(False)
			Next l
			' check parameters
			assertEquals(netIntegrated.getLayer(0).getParam("W"), netSeparate.getLayer(0).getParam("W"))
			assertEquals(netIntegrated.getLayer(0).getParam("b"), netSeparate.getLayer(0).getParam("b"))
			assertEquals(netIntegrated.getLayer(1).getParam("W"), netSeparate.getLayer(2).getParam("W"))
			assertEquals(netIntegrated.getLayer(1).getParam("b"), netSeparate.getLayer(2).getParam("b"))
			' check activations
			netIntegrated.Input = [next].Features
			netSeparate.Input = [next].Features
			Nd4j.Random.setSeed(12345)
			Dim actTrainIntegrated As IList(Of INDArray) = netIntegrated.feedForward(True)
			Nd4j.Random.setSeed(12345)
			Dim actTrainSeparate As IList(Of INDArray) = netSeparate.feedForward(True)
			assertEquals(actTrainIntegrated(1), actTrainSeparate(1))
			assertEquals(actTrainIntegrated(2), actTrainSeparate(3))
			Nd4j.Random.setSeed(12345)
			Dim actTestIntegrated As IList(Of INDArray) = netIntegrated.feedForward(False)
			Nd4j.Random.setSeed(12345)
			Dim actTestSeparate As IList(Of INDArray) = netSeparate.feedForward(False)
			assertEquals(actTestIntegrated(1), actTrainSeparate(1))
			assertEquals(actTestIntegrated(2), actTestSeparate(3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dropout Layer With Conv Mnist") void testDropoutLayerWithConvMnist() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testDropoutLayerWithConvMnist()
			' Set to double datatype - MKL-DNN not used for CPU (otherwise different strides due to Dl4J impl permutes)
			Nd4j.setDefaultDataTypes(DataType.DOUBLE, DataType.DOUBLE)
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			' Run without separate activation layer
			Nd4j.Random.setSeed(12345)
			Dim confIntegrated As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).nIn(1).nOut(20).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).dropOut(0.5).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			' Run with separate activation layer
			Nd4j.Random.setSeed(12345)
			' Manually configure preprocessors
			' This is necessary, otherwise CnnToFeedForwardPreprocessor will be in different locatinos
			' i.e., dropout on 4d activations in latter, and dropout on 2d activations in former
			Dim preProcessorMap As IDictionary(Of Integer, InputPreProcessor) = New Dictionary(Of Integer, InputPreProcessor)()
			preProcessorMap(1) = New CnnToFeedForwardPreProcessor(13, 13, 20)
			Dim confSeparate As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).list().layer(0, (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).nIn(1).nOut(20).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(1, (New DropoutLayer.Builder(0.5)).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nOut(10).build()).inputPreProcessors(preProcessorMap).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Nd4j.Random.setSeed(12345)
			Dim netIntegrated As New MultiLayerNetwork(confIntegrated)
			netIntegrated.init()
			Nd4j.Random.setSeed(12345)
			Dim netSeparate As New MultiLayerNetwork(confSeparate)
			netSeparate.init()
			assertEquals(netIntegrated.params(), netSeparate.params())
			Nd4j.Random.setSeed(12345)
			netIntegrated.fit([next])
			Nd4j.Random.setSeed(12345)
			netSeparate.fit([next])
			assertEquals(netIntegrated.params(), netSeparate.params())
			' check parameters
			assertEquals(netIntegrated.getLayer(0).getParam("W"), netSeparate.getLayer(0).getParam("W"))
			assertEquals(netIntegrated.getLayer(0).getParam("b"), netSeparate.getLayer(0).getParam("b"))
			assertEquals(netIntegrated.getLayer(1).getParam("W"), netSeparate.getLayer(2).getParam("W"))
			assertEquals(netIntegrated.getLayer(1).getParam("b"), netSeparate.getLayer(2).getParam("b"))
			' check activations
			netIntegrated.Input = [next].Features.dup()
			netSeparate.Input = [next].Features.dup()
			Nd4j.Random.setSeed(12345)
			Dim actTrainIntegrated As IList(Of INDArray) = netIntegrated.feedForward(True)
			Nd4j.Random.setSeed(12345)
			Dim actTrainSeparate As IList(Of INDArray) = netSeparate.feedForward(True)
			assertEquals(actTrainIntegrated(1), actTrainSeparate(1))
			assertEquals(actTrainIntegrated(2), actTrainSeparate(3))
			netIntegrated.Input = [next].Features.dup()
			netSeparate.Input = [next].Features.dup()
			Nd4j.Random.setSeed(12345)
			Dim actTestIntegrated As IList(Of INDArray) = netIntegrated.feedForward(False)
			Nd4j.Random.setSeed(12345)
			Dim actTestSeparate As IList(Of INDArray) = netSeparate.feedForward(False)
			assertEquals(actTestIntegrated(1), actTestSeparate(1))
			assertEquals(actTestIntegrated(2), actTestSeparate(3))
		End Sub
	End Class

End Namespace