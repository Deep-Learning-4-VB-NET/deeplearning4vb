Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports UnitNormConstraint = org.deeplearning4j.nn.conf.constraint.UnitNormConstraint
Imports ConstantDistribution = org.deeplearning4j.nn.conf.distribution.ConstantDistribution
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports AttentionVertex = org.deeplearning4j.nn.conf.graph.AttentionVertex
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports FrozenLayer = org.deeplearning4j.nn.conf.layers.misc.FrozenLayer
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports DropConnect = org.deeplearning4j.nn.conf.weightnoise.DropConnect
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
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
Namespace org.deeplearning4j.nn.transferlearning

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Transfer Learning Comp Graph Test") class TransferLearningCompGraphTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class TransferLearningCompGraphTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Simple Fine Tune") void simpleFineTune()
		Friend Overridable Sub simpleFineTune()
			Dim rng As Long = 12345L
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 3))
			' original conf
			Dim confToChange As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(rng).optimizationAlgo(OptimizationAlgorithm.LBFGS).updater(New Nesterovs(0.01, 0.99)).graphBuilder().addInputs("layer0In").setInputTypes(InputType.feedForward(4)).addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "layer0In").addLayer("layer1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer0").setOutputs("layer1").build()
			' conf with learning parameters changed
			Dim expectedConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(rng).updater(New RmsProp(0.2)).graphBuilder().addInputs("layer0In").setInputTypes(InputType.feedForward(4)).addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "layer0In").addLayer("layer1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer0").setOutputs("layer1").build()
			Dim expectedModel As New ComputationGraph(expectedConf)
			expectedModel.init()
			Dim modelToFineTune As New ComputationGraph(expectedConf)
			modelToFineTune.init()
			modelToFineTune.Params = expectedModel.params()
			' model after applying changes with transfer learning
			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToFineTune)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).seed(rng).updater(New RmsProp(0.2)).build()).build()
			' Check json
			assertEquals(expectedConf.toJson(), modelNow.Configuration.toJson())
			' Check params after fit
			modelNow.fit(randomData)
			expectedModel.fit(randomData)
			assertEquals(modelNow.score(), expectedModel.score(), 1e-8)
			assertEquals(modelNow.params(), expectedModel.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Nout Changes") void testNoutChanges()
		Friend Overridable Sub testNoutChanges()
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 2))
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY)
'JAVA TO VB CONVERTER NOTE: The variable fineTuneConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim fineTuneConfiguration_Conflict As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY).build()
			Dim modelToFineTune As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "layer0In").addLayer("layer1", (New DenseLayer.Builder()).nIn(3).nOut(2).build(), "layer0").addLayer("layer2", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "layer1").addLayer("layer3", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer2").setOutputs("layer3").build())
			modelToFineTune.init()
			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToFineTune)).fineTuneConfiguration(fineTuneConfiguration_Conflict).nOutReplace("layer3", 2, WeightInit.XAVIER).nOutReplace("layer0", 3, New NormalDistribution(1, 1e-1), WeightInit.XAVIER).build()
			Dim bl0 As BaseLayer = (CType(modelNow.getLayer("layer0").conf().getLayer(), BaseLayer))
			Dim bl1 As BaseLayer = (CType(modelNow.getLayer("layer1").conf().getLayer(), BaseLayer))
			Dim bl3 As BaseLayer = (CType(modelNow.getLayer("layer3").conf().getLayer(), BaseLayer))
			assertEquals(bl0.getWeightInitFn(), New WeightInitDistribution(New NormalDistribution(1, 1e-1)))
			assertEquals(bl1.getWeightInitFn(), New WeightInitXavier())
			assertEquals(bl1.getWeightInitFn(), New WeightInitXavier())
			Dim modelExpectedArch As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "layer0In").addLayer("layer1", (New DenseLayer.Builder()).nIn(3).nOut(2).build(), "layer0").addLayer("layer2", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "layer1").addLayer("layer3", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(2).build(), "layer2").setOutputs("layer3").build())
			modelExpectedArch.init()
			' modelNow should have the same architecture as modelExpectedArch
			assertArrayEquals(modelExpectedArch.params().shape(), modelNow.params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer0").params().shape(), modelNow.getLayer("layer0").params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer1").params().shape(), modelNow.getLayer("layer1").params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer2").params().shape(), modelNow.getLayer("layer2").params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer3").params().shape(), modelNow.getLayer("layer3").params().shape())
			modelNow.Params = modelExpectedArch.params()
			' fit should give the same results
			modelExpectedArch.fit(randomData)
			modelNow.fit(randomData)
			assertEquals(modelExpectedArch.score(), modelNow.score(), 1e-8)
			assertEquals(modelExpectedArch.params(), modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Remove And Add") void testRemoveAndAdd()
		Friend Overridable Sub testRemoveAndAdd()
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 3))
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY)
'JAVA TO VB CONVERTER NOTE: The variable fineTuneConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim fineTuneConfiguration_Conflict As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY).build()
			Dim modelToFineTune As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "layer0In").addLayer("layer1", (New DenseLayer.Builder()).nIn(5).nOut(2).build(), "layer0").addLayer("layer2", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "layer1").addLayer("layer3", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer2").setOutputs("layer3").build())
			modelToFineTune.init()
			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToFineTune)).fineTuneConfiguration(fineTuneConfiguration_Conflict).nOutReplace("layer0", 7, WeightInit.XAVIER, WeightInit.XAVIER).nOutReplace("layer2", 5, WeightInit.XAVIER).removeVertexKeepConnections("layer3").addLayer("layer3", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(5).nOut(3).activation(Activation.SOFTMAX).build(), "layer2").build()
			Dim modelExpectedArch As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(7).build(), "layer0In").addLayer("layer1", (New DenseLayer.Builder()).nIn(7).nOut(2).build(), "layer0").addLayer("layer2", (New DenseLayer.Builder()).nIn(2).nOut(5).build(), "layer1").addLayer("layer3", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(5).nOut(3).build(), "layer2").setOutputs("layer3").build())
			modelExpectedArch.init()
			' modelNow should have the same architecture as modelExpectedArch
			assertArrayEquals(modelExpectedArch.params().shape(), modelNow.params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer0").params().shape(), modelNow.getLayer("layer0").params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer1").params().shape(), modelNow.getLayer("layer1").params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer2").params().shape(), modelNow.getLayer("layer2").params().shape())
			assertArrayEquals(modelExpectedArch.getLayer("layer3").params().shape(), modelNow.getLayer("layer3").params().shape())
			modelNow.Params = modelExpectedArch.params()
			' fit should give the same results
			modelExpectedArch.fit(randomData)
			modelNow.fit(randomData)
			assertEquals(modelExpectedArch.score(), modelNow.score(), 1e-8)
			assertEquals(modelExpectedArch.params(), modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test All With CNN") void testAllWithCNN()
		Friend Overridable Sub testAllWithCNN()
			Dim randomData As New DataSet(Nd4j.rand(10, 28 * 28 * 3).reshape(10, 3, 28, 28), Nd4j.rand(10, 10))
			Dim modelToFineTune As New ComputationGraph((New NeuralNetConfiguration.Builder()).seed(123).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).graphBuilder().addInputs("layer0In").setInputTypes(InputType.convolutionalFlat(28, 28, 3)).addLayer("layer0", (New ConvolutionLayer.Builder(5, 5)).nIn(3).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build(), "layer0In").addLayer("layer1", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build(), "layer0").addLayer("layer2", (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build(), "layer1").addLayer("layer3", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build(), "layer2").addLayer("layer4", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build(), "layer3").addLayer("layer5", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(250).build(), "layer4").addLayer("layer6", (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(100).activation(Activation.SOFTMAX).build(), "layer5").setOutputs("layer6").build())
			modelToFineTune.init()
			' this will override the learning configuration set in the model
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(456).updater(New Sgd(0.001))
'JAVA TO VB CONVERTER NOTE: The variable fineTuneConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim fineTuneConfiguration_Conflict As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).seed(456).updater(New Sgd(0.001)).build()
			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToFineTune)).fineTuneConfiguration(fineTuneConfiguration_Conflict).setFeatureExtractor("layer1").nOutReplace("layer4", 600, WeightInit.XAVIER).removeVertexAndConnections("layer5").removeVertexAndConnections("layer6").setInputs("layer0In").setInputTypes(InputType.convolutionalFlat(28, 28, 3)).addLayer("layer5", (New DenseLayer.Builder()).activation(Activation.RELU).nIn(600).nOut(300).build(), "layer4").addLayer("layer6", (New DenseLayer.Builder()).activation(Activation.RELU).nIn(300).nOut(150).build(), "layer5").addLayer("layer7", (New DenseLayer.Builder()).activation(Activation.RELU).nIn(150).nOut(50).build(), "layer6").addLayer("layer8", (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).activation(Activation.SOFTMAX).nIn(50).nOut(10).build(), "layer7").setOutputs("layer8").build()
			Dim modelExpectedArch As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").setInputTypes(InputType.convolutionalFlat(28, 28, 3)).addLayer("layer0", New FrozenLayer((New ConvolutionLayer.Builder(5, 5)).nIn(3).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()), "layer0In").addLayer("layer1", New FrozenLayer((New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()), "layer0").addLayer("layer2", (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build(), "layer1").addLayer("layer3", (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build(), "layer2").addLayer("layer4", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(600).build(), "layer3").addLayer("layer5", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(300).build(), "layer4").addLayer("layer6", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(150).build(), "layer5").addLayer("layer7", (New DenseLayer.Builder()).activation(Activation.RELU).nOut(50).build(), "layer6").addLayer("layer8", (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(10).activation(Activation.SOFTMAX).build(), "layer7").setOutputs("layer8").build())
			modelExpectedArch.init()
			modelExpectedArch.getVertex("layer0").setLayerAsFrozen()
			modelExpectedArch.getVertex("layer1").setLayerAsFrozen()
			assertEquals(modelExpectedArch.Configuration.toJson(), modelNow.Configuration.toJson())
			modelNow.Params = modelExpectedArch.params()
			Dim i As Integer = 0
			Do While i < 5
				modelExpectedArch.fit(randomData)
				modelNow.fit(randomData)
				i += 1
			Loop
			assertEquals(modelExpectedArch.params(), modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Transfer Global Pool") void testTransferGlobalPool()
		Friend Overridable Sub testTransferGlobalPool()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Adam(0.1)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("blstm1", (New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).activation(Activation.TANH).build(), "in").addLayer("pool", (New GlobalPoolingLayer.Builder()).build(), "blstm1").addLayer("dense", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "pool").addLayer("out", (New OutputLayer.Builder()).nIn(10).nOut(10).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build(), "dense").setOutputs("out").build()
			Dim g As New ComputationGraph(conf)
			g.init()
'JAVA TO VB CONVERTER NOTE: The variable fineTuneConfiguration was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim fineTuneConfiguration_Conflict As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).seed(12345).updater(New Sgd(0.01)).build()
			Dim graph As ComputationGraph = (New TransferLearning.GraphBuilder(g)).fineTuneConfiguration(fineTuneConfiguration_Conflict).removeVertexKeepConnections("out").setFeatureExtractor("dense").addLayer("out", (New OutputLayer.Builder()).updater(New Adam(0.1)).weightInit(WeightInit.XAVIER).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).nIn(10).nOut(5).build(), "dense").build()
			Dim confExpected As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Sgd(0.01)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("blstm1", New FrozenLayer((New GravesBidirectionalLSTM.Builder()).nIn(10).nOut(10).activation(Activation.TANH).build()), "in").addLayer("pool", New FrozenLayer((New GlobalPoolingLayer.Builder()).build()), "blstm1").addLayer("dense", New FrozenLayer((New DenseLayer.Builder()).nIn(10).nOut(10).build()), "pool").addLayer("out", (New OutputLayer.Builder()).nIn(10).nOut(5).activation(Activation.SOFTMAX).updater(New Adam(0.1)).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "dense").setOutputs("out").build()
			Dim modelExpected As New ComputationGraph(confExpected)
			modelExpected.init()
			' assertEquals(confExpected, graph.getConfiguration());
			assertEquals(confExpected.toJson(), graph.Configuration.toJson())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Object Overrides") void testObjectOverrides()
		Friend Overridable Sub testObjectOverrides()
			' https://github.com/eclipse/deeplearning4j/issues/4368
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(0.5).weightNoise(New DropConnect(0.5)).l2(0.5).constrainWeights(New UnitNormConstraint()).graphBuilder().addInputs("in").addLayer("layer", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").setOutputs("layer").build()
			Dim orig As New ComputationGraph(conf)
			orig.init()
			Dim ftc As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).dropOut(0).weightNoise(Nothing).constraints(Nothing).l2(0.0).build()
			Dim transfer As ComputationGraph = (New TransferLearning.GraphBuilder(orig)).fineTuneConfiguration(ftc).build()
			Dim l As DenseLayer = CType(transfer.getLayer(0).conf().getLayer(), DenseLayer)
			assertNull(l.getIDropout())
			assertNull(l.getWeightNoise())
			assertNull(l.getConstraints())
			assertNull(TestUtils.getL2Reg(l))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Transfer Learning Subsequent") void testTransferLearningSubsequent()
		Friend Overridable Sub testTransferLearningSubsequent()
			Dim inputName As String = "in"
			Dim outputName As String = "out"
			Const firstConv As String = "firstConv"
			Const secondConv As String = "secondConv"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.create(6, 6, 6, 6);
			Dim input As INDArray = Nd4j.create(6, 6, 6, 6)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph graph = new org.deeplearning4j.nn.graph.ComputationGraph(new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().weightInit(new org.deeplearning4j.nn.conf.distribution.ConstantDistribution(666)).graphBuilder().addInputs(inputName).setOutputs(outputName).setInputTypes(org.deeplearning4j.nn.conf.inputs.InputType.inferInputTypes(input)).addLayer(firstConv, new Convolution2D.Builder(3, 3).nOut(10).build(), inputName).addLayer(secondConv, new Convolution2D.Builder(1, 1).nOut(3).build(), firstConv).addLayer(outputName, new OutputLayer.Builder().nOut(2).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).build(), secondConv).build());
			Dim graph As New ComputationGraph((New NeuralNetConfiguration.Builder()).weightInit(New ConstantDistribution(666)).graphBuilder().addInputs(inputName).setOutputs(outputName).setInputTypes(InputType.inferInputTypes(input)).addLayer(firstConv, (New Convolution2D.Builder(3, 3)).nOut(10).build(), inputName).addLayer(secondConv, (New Convolution2D.Builder(1, 1)).nOut(3).build(), firstConv).addLayer(outputName, (New OutputLayer.Builder()).nOut(2).lossFunction(LossFunctions.LossFunction.MSE).build(), secondConv).build())
			graph.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph newGraph = new TransferLearning.GraphBuilder(graph).nOutReplace(firstConv, 7, new org.deeplearning4j.nn.conf.distribution.ConstantDistribution(333)).nOutReplace(secondConv, 3, new org.deeplearning4j.nn.conf.distribution.ConstantDistribution(111)).removeVertexAndConnections(outputName).addLayer(outputName, new OutputLayer.Builder().nIn(48).nOut(2).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).build(), new org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor(4, 4, 3), secondConv).setOutputs(outputName).build();
			Dim newGraph As ComputationGraph = (New TransferLearning.GraphBuilder(graph)).nOutReplace(firstConv, 7, New ConstantDistribution(333)).nOutReplace(secondConv, 3, New ConstantDistribution(111)).removeVertexAndConnections(outputName).addLayer(outputName, (New OutputLayer.Builder()).nIn(48).nOut(2).lossFunction(LossFunctions.LossFunction.MSE).build(), New CnnToFeedForwardPreProcessor(4, 4, 3), secondConv).setOutputs(outputName).build()
			newGraph.init()
			assertEquals(7, newGraph.layerInputSize(secondConv), "Incorrect # inputs")
			newGraph.outputSingle(input)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Change N Out N In") void testChangeNOutNIn()
		Friend Overridable Sub testChangeNOutNIn()
			Const inputName As String = "input"
			Const changeNoutName As String = "changeNout"
			Const poolName As String = "pool"
			Const afterPoolName As String = "afterPool"
			Const outputName As String = "output"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.create(new long[] { 1, 2, 4, 4 });
			Dim input As INDArray = Nd4j.create(New Long() { 1, 2, 4, 4 })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph graph = new org.deeplearning4j.nn.graph.ComputationGraph(new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().graphBuilder().addInputs(inputName).setOutputs(outputName).setInputTypes(org.deeplearning4j.nn.conf.inputs.InputType.inferInputTypes(input)).addLayer(changeNoutName, new Convolution2D.Builder(1, 1).nOut(10).build(), inputName).addLayer(poolName, new SubsamplingLayer.Builder(1, 1).build(), changeNoutName).addLayer(afterPoolName, new Convolution2D.Builder(1, 1).nOut(7).build(), poolName).addLayer(outputName, new OutputLayer.Builder().activation(org.nd4j.linalg.activations.Activation.SOFTMAX).nOut(2).build(), afterPoolName).build());
			Dim graph As New ComputationGraph((New NeuralNetConfiguration.Builder()).graphBuilder().addInputs(inputName).setOutputs(outputName).setInputTypes(InputType.inferInputTypes(input)).addLayer(changeNoutName, (New Convolution2D.Builder(1, 1)).nOut(10).build(), inputName).addLayer(poolName, (New SubsamplingLayer.Builder(1, 1)).build(), changeNoutName).addLayer(afterPoolName, (New Convolution2D.Builder(1, 1)).nOut(7).build(), poolName).addLayer(outputName, (New OutputLayer.Builder()).activation(Activation.SOFTMAX).nOut(2).build(), afterPoolName).build())
			graph.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph newGraph = new TransferLearning.GraphBuilder(graph).nOutReplace(changeNoutName, 5, org.deeplearning4j.nn.weights.WeightInit.XAVIER).nInReplace(afterPoolName, 5, org.deeplearning4j.nn.weights.WeightInit.XAVIER).build();
			Dim newGraph As ComputationGraph = (New TransferLearning.GraphBuilder(graph)).nOutReplace(changeNoutName, 5, WeightInit.XAVIER).nInReplace(afterPoolName, 5, WeightInit.XAVIER).build()
			newGraph.init()
			assertEquals(5, newGraph.layerSize(changeNoutName), "Incorrect number of outputs!")
			assertEquals(5, newGraph.layerInputSize(afterPoolName), "Incorrect number of inputs!")
			newGraph.output(input)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Transfer Learning Same Diff Layers Graph") void testTransferLearningSameDiffLayersGraph()
		Friend Overridable Sub testTransferLearningSameDiffLayersGraph()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("l0", (New LSTM.Builder()).nIn(5).nOut(5).build(), "in").layer("l1", (New RecurrentAttentionLayer.Builder()).nHeads(1).headSize(5).nIn(5).nOut(5).build(), "l0").layer("out", (New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build(), "l1").setOutputs("out").build()
			Dim cg As New ComputationGraph(conf)
			cg.init()
			Dim arr As INDArray = Nd4j.rand(DataType.FLOAT, 2, 5, 10)
			Dim [out] As INDArray = cg.output(arr)(0)
			Dim cg2 As ComputationGraph = (New TransferLearning.GraphBuilder(cg)).removeVertexAndConnections("out").fineTuneConfiguration(FineTuneConfiguration.builder().updater(New Adam(0.01)).build()).removeVertexAndConnections("out").addLayer("newOut", (New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build(), "l1").setOutputs("newOut").build()
			cg2.output(arr)
			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)(cg.paramTable())
			m("newOut_W") = m.Remove("out_W")
			m("newOut_b") = m.Remove("out_b")
			cg2.ParamTable = m
			Dim p1 As IDictionary(Of String, INDArray) = cg.paramTable()
			Dim p2 As IDictionary(Of String, INDArray) = cg2.paramTable()
			For Each s As String In p1.Keys
				Dim i1 As INDArray = p1(s)
				Dim i2 As INDArray = p2(s.replaceAll("out", "newOut"))
				assertEquals(i1, i2,s)
			Next s
			Dim out2 As INDArray = cg2.outputSingle(arr)
			assertEquals([out], out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Transfer Learning Same Diff Layers Graph Vertex") void testTransferLearningSameDiffLayersGraphVertex()
		Friend Overridable Sub testTransferLearningSameDiffLayersGraphVertex()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("l0", (New LSTM.Builder()).nIn(5).nOut(5).build(), "in").addVertex("l1", (New AttentionVertex.Builder()).nHeads(1).headSize(5).nInKeys(5).nInQueries(5).nInValues(5).nOut(5).build(), "l0", "l0", "l0").layer("out", (New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build(), "l1").setOutputs("out").build()
			Dim cg As New ComputationGraph(conf)
			cg.init()
			Dim arr As INDArray = Nd4j.rand(DataType.FLOAT, 2, 5, 10)
			Dim [out] As INDArray = cg.output(arr)(0)
			Dim cg2 As ComputationGraph = (New TransferLearning.GraphBuilder(cg)).removeVertexAndConnections("out").fineTuneConfiguration(FineTuneConfiguration.builder().updater(New Adam(0.01)).build()).removeVertexAndConnections("out").addLayer("newOut", (New RnnOutputLayer.Builder()).nIn(5).nOut(5).activation(Activation.SOFTMAX).build(), "l1").setOutputs("newOut").build()
			cg2.output(arr)
			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)(cg.paramTable())
			m("newOut_W") = m.Remove("out_W")
			m("newOut_b") = m.Remove("out_b")
			cg2.ParamTable = m
			Dim p1 As IDictionary(Of String, INDArray) = cg.paramTable()
			Dim p2 As IDictionary(Of String, INDArray) = cg2.paramTable()
			For Each s As String In p1.Keys
				Dim i1 As INDArray = p1(s)
				Dim i2 As INDArray = p2(s.replaceAll("out", "newOut"))
				assertEquals(i1, i2,s)
			Next s
			Dim out2 As INDArray = cg2.outputSingle(arr)
			assertEquals([out], out2)
		End Sub
	End Class

End Namespace