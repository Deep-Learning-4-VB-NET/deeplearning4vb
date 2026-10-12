Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ActivationLayer = org.deeplearning4j.nn.conf.layers.ActivationLayer
Imports AutoEncoder = org.deeplearning4j.nn.conf.layers.AutoEncoder
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports ActivationELU = org.nd4j.linalg.activations.impl.ActivationELU
Imports ActivationRationalTanh = org.nd4j.linalg.activations.impl.ActivationRationalTanh
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
Namespace org.deeplearning4j.nn.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Activation Layer Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class ActivationLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ActivationLayerTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Input Types") void testInputTypes()
		Friend Overridable Sub testInputTypes()
			Dim l As ActivationLayer = (New ActivationLayer.Builder()).activation(Activation.RELU).build()
			Dim in1 As InputType = InputType.feedForward(20)
			Dim in2 As InputType = InputType.convolutional(28, 28, 1)
			assertEquals(in1, l.getOutputType(0, in1))
			assertEquals(in2, l.getOutputType(0, in2))
			assertNull(l.getPreProcessorForInputType(in1))
			assertNull(l.getPreProcessorForInputType(in2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Dense Activation Layer") void testDenseActivationLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testDenseActivationLayer()
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			' Run without separate activation layer
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28 * 1).nOut(10).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.fit([next])
			' Run with separate activation layer
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28 * 1).nOut(10).activation(Activation.IDENTITY).weightInit(WeightInit.XAVIER).build()).layer(1, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim network2 As New MultiLayerNetwork(conf2)
			network2.init()
			network2.fit([next])
			' check parameters
			assertEquals(network.getLayer(0).getParam("W"), network2.getLayer(0).getParam("W"))
			assertEquals(network.getLayer(1).getParam("W"), network2.getLayer(2).getParam("W"))
			assertEquals(network.getLayer(0).getParam("b"), network2.getLayer(0).getParam("b"))
			assertEquals(network.getLayer(1).getParam("b"), network2.getLayer(2).getParam("b"))
			' check activations
			network.init()
			network.Input = [next].Features
			Dim activations As IList(Of INDArray) = network.feedForward(True)
			network2.init()
			network2.Input = [next].Features
			Dim activations2 As IList(Of INDArray) = network2.feedForward(True)
			assertEquals(activations(1).reshape(activations2(2).shape()), activations2(2))
			assertEquals(activations(2), activations2(3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Auto Encoder Activation Layer") void testAutoEncoderActivationLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testAutoEncoderActivationLayer()
			Dim minibatch As Integer = 3
			Dim nIn As Integer = 5
			Dim layerSize As Integer = 5
			Dim nOut As Integer = 3
			Dim [next] As INDArray = Nd4j.rand(New Integer() { minibatch, nIn })
			Dim labels As INDArray = Nd4j.zeros(minibatch, nOut)
			For i As Integer = 0 To minibatch - 1
				labels.putScalar(i, i Mod nOut, 1.0)
			Next i
			' Run without separate activation layer
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New AutoEncoder.Builder()).nIn(nIn).nOut(layerSize).corruptionLevel(0.0).activation(Activation.SIGMOID).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.RECONSTRUCTION_CROSSENTROPY)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			' Labels are necessary for this test: layer activation function affect pretraining results, otherwise
			network.fit([next], labels)
			' Run with separate activation layer
			Nd4j.Random.setSeed(12345)
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New AutoEncoder.Builder()).nIn(nIn).nOut(layerSize).corruptionLevel(0.0).activation(Activation.IDENTITY).build()).layer(1, (New ActivationLayer.Builder()).activation(Activation.SIGMOID).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.RECONSTRUCTION_CROSSENTROPY)).activation(Activation.SOFTMAX).nIn(layerSize).nOut(nOut).build()).build()
			Dim network2 As New MultiLayerNetwork(conf2)
			network2.init()
			network2.fit([next], labels)
			' check parameters
			assertEquals(network.getLayer(0).getParam("W"), network2.getLayer(0).getParam("W"))
			assertEquals(network.getLayer(1).getParam("W"), network2.getLayer(2).getParam("W"))
			assertEquals(network.getLayer(0).getParam("b"), network2.getLayer(0).getParam("b"))
			assertEquals(network.getLayer(1).getParam("b"), network2.getLayer(2).getParam("b"))
			' check activations
			network.init()
			network.Input = [next]
			Dim activations As IList(Of INDArray) = network.feedForward(True)
			network2.init()
			network2.Input = [next]
			Dim activations2 As IList(Of INDArray) = network2.feedForward(True)
			assertEquals(activations(1).reshape(activations2(2).shape()), activations2(2))
			assertEquals(activations(2), activations2(3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test CNN Activation Layer") void testCNNActivationLayer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testCNNActivationLayer()
			Dim iter As DataSetIterator = New MnistDataSetIterator(2, 2)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next] As DataSet = iter.next()
			' Run without separate activation layer
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).nIn(1).nOut(20).activation(Activation.RELU).weightInit(WeightInit.XAVIER).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			network.fit([next])
			' Run with separate activation layer
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).list().layer(0, (New ConvolutionLayer.Builder(4, 4)).stride(2, 2).nIn(1).nOut(20).activation(Activation.IDENTITY).weightInit(WeightInit.XAVIER).build()).layer(1, (New ActivationLayer.Builder()).activation(Activation.RELU).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutionalFlat(28, 28, 1)).build()
			Dim network2 As New MultiLayerNetwork(conf2)
			network2.init()
			network2.fit([next])
			' check parameters
			assertEquals(network.getLayer(0).getParam("W"), network2.getLayer(0).getParam("W"))
			assertEquals(network.getLayer(1).getParam("W"), network2.getLayer(2).getParam("W"))
			assertEquals(network.getLayer(0).getParam("b"), network2.getLayer(0).getParam("b"))
			' check activations
			network.init()
			network.Input = [next].Features
			Dim activations As IList(Of INDArray) = network.feedForward(True)
			network2.init()
			network2.Input = [next].Features
			Dim activations2 As IList(Of INDArray) = network2.feedForward(True)
			assertEquals(activations(1).reshape(activations2(2).shape()), activations2(2))
			assertEquals(activations(2), activations2(3))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Activation Inheritance") void testActivationInheritance()
		Friend Overridable Sub testActivationInheritance()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).weightInit(WeightInit.XAVIER).activation(Activation.RATIONALTANH).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(New ActivationLayer()).layer((New ActivationLayer.Builder()).build()).layer((New ActivationLayer.Builder()).activation(Activation.ELU).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim network As New MultiLayerNetwork(conf)
			network.init()
			assertNotNull(CType(network.getLayer(1).conf().getLayer(), ActivationLayer).getActivationFn())
			assertTrue(TypeOf (CType(network.getLayer(0).conf().getLayer(), DenseLayer)).getActivationFn() Is ActivationRationalTanh)
			assertTrue(TypeOf (CType(network.getLayer(1).conf().getLayer(), ActivationLayer)).getActivationFn() Is ActivationRationalTanh)
			assertTrue(TypeOf (CType(network.getLayer(2).conf().getLayer(), ActivationLayer)).getActivationFn() Is ActivationRationalTanh)
			assertTrue(TypeOf (CType(network.getLayer(3).conf().getLayer(), ActivationLayer)).getActivationFn() Is ActivationELU)
			assertTrue(TypeOf (CType(network.getLayer(4).conf().getLayer(), OutputLayer)).getActivationFn() Is ActivationSoftmax)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Activation Inheritance CG") void testActivationInheritanceCG()
		Friend Overridable Sub testActivationInheritanceCG()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).seed(123).weightInit(WeightInit.XAVIER).activation(Activation.RATIONALTANH).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", New ActivationLayer(), "0").addLayer("2", (New ActivationLayer.Builder()).build(), "1").addLayer("3", (New ActivationLayer.Builder()).activation(Activation.ELU).build(), "2").addLayer("4", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "3").setOutputs("4").build()
			Dim network As New ComputationGraph(conf)
			network.init()
			assertNotNull(CType(network.getLayer("1").conf().getLayer(), ActivationLayer).getActivationFn())
			assertTrue(TypeOf (CType(network.getLayer("0").conf().getLayer(), DenseLayer)).getActivationFn() Is ActivationRationalTanh)
			assertTrue(TypeOf (CType(network.getLayer("1").conf().getLayer(), ActivationLayer)).getActivationFn() Is ActivationRationalTanh)
			assertTrue(TypeOf (CType(network.getLayer("2").conf().getLayer(), ActivationLayer)).getActivationFn() Is ActivationRationalTanh)
			assertTrue(TypeOf (CType(network.getLayer("3").conf().getLayer(), ActivationLayer)).getActivationFn() Is ActivationELU)
			assertTrue(TypeOf (CType(network.getLayer("4").conf().getLayer(), OutputLayer)).getActivationFn() Is ActivationSoftmax)
		End Sub
	End Class

End Namespace