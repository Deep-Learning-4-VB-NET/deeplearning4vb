Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports UnitNormConstraint = org.deeplearning4j.nn.conf.constraint.UnitNormConstraint
Imports ConstantDistribution = org.deeplearning4j.nn.conf.distribution.ConstantDistribution
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports org.deeplearning4j.nn.conf.layers
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports FeedForwardToRnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.FeedForwardToRnnPreProcessor
Imports RnnToCnnPreProcessor = org.deeplearning4j.nn.conf.preprocessor.RnnToCnnPreProcessor
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports DropConnect = org.deeplearning4j.nn.conf.weightnoise.DropConnect
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports WeightInitRelu = org.deeplearning4j.nn.weights.WeightInitRelu
Imports WeightInitXavier = org.deeplearning4j.nn.weights.WeightInitXavier
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.linalg.learning.config
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
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
'ORIGINAL LINE: @Slf4j @DisplayName("Transfer Learning MLN Test") class TransferLearningMLNTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class TransferLearningMLNTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Simple Fine Tune") void simpleFineTune()
		Friend Overridable Sub simpleFineTune()
			Dim rng As Long = 12345L
			Nd4j.Random.Seed = rng
			Dim randomData As New DataSet(Nd4j.rand(DataType.FLOAT, 10, 4), TestUtils.randomOneHot(DataType.FLOAT, 10, 3))
			' original conf
			Dim confToChange As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(rng).optimizationAlgo(OptimizationAlgorithm.LBFGS).updater(New Nesterovs(0.01, 0.99))
			Dim modelToFineTune As New MultiLayerNetwork(confToChange.list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			modelToFineTune.init()
			' model after applying changes with transfer learning
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).seed(rng).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New RmsProp(0.5)).l2(0.4).build()).build()
			For Each l As org.deeplearning4j.nn.api.Layer In modelNow.Layers
				Dim bl As BaseLayer = (CType(l.conf().getLayer(), BaseLayer))
				assertEquals(New RmsProp(0.5), bl.getIUpdater())
			Next l
			Dim confSet As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(rng).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New RmsProp(0.5)).l2(0.4)
			Dim expectedModel As New MultiLayerNetwork(confSet.list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			expectedModel.init()
			expectedModel.Params = modelToFineTune.params().dup()
			assertEquals(expectedModel.params(), modelNow.params())
			' Check json
			Dim expectedConf As MultiLayerConfiguration = expectedModel.LayerWiseConfigurations
			assertEquals(expectedConf.toJson(), modelNow.LayerWiseConfigurations.toJson())
			' Check params after fit
			modelNow.fit(randomData)
			expectedModel.fit(randomData)
			assertEquals(modelNow.score(), expectedModel.score(), 1e-6)
			Dim pExp As INDArray = expectedModel.params()
			Dim pNow As INDArray = modelNow.params()
			assertEquals(pExp, pNow)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Nout Changes") void testNoutChanges()
		Friend Overridable Sub testNoutChanges()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(DataType.FLOAT, 10, 4), TestUtils.randomOneHot(DataType.FLOAT, 10, 2))
			Dim equivalentConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1))
			Dim overallConf As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).build()
			Dim modelToFineTune As New MultiLayerNetwork(equivalentConf.list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(5).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).build()).layer(2, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			modelToFineTune.init()
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).fineTuneConfiguration(overallConf).nOutReplace(3, 2, WeightInit.XAVIER, WeightInit.XAVIER).nOutReplace(0, 3, WeightInit.XAVIER, New NormalDistribution(1, 1e-1)).build()
			Dim modelExpectedArch As New MultiLayerNetwork(equivalentConf.list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).build()).layer(2, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(2).build()).build())
			modelExpectedArch.init()
			' Will fail - expected because of dist and weight init changes
			' assertEquals(modelExpectedArch.getLayerWiseConfigurations().toJson(), modelNow.getLayerWiseConfigurations().toJson());
			Dim bl0 As BaseLayer = (CType(modelNow.LayerWiseConfigurations.getConf(0).getLayer(), BaseLayer))
			Dim bl1 As BaseLayer = (CType(modelNow.LayerWiseConfigurations.getConf(1).getLayer(), BaseLayer))
			Dim bl3 As BaseLayer = (CType(modelNow.LayerWiseConfigurations.getConf(3).getLayer(), BaseLayer))
			assertEquals(bl0.getWeightInitFn().GetType(), GetType(WeightInitXavier))
			Try
				assertEquals(JsonMappers.Mapper.writeValueAsString(bl1.getWeightInitFn()), JsonMappers.Mapper.writeValueAsString(New WeightInitDistribution(New NormalDistribution(1, 1e-1))))
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
			assertEquals(bl3.getWeightInitFn(), New WeightInitXavier())
			' modelNow should have the same architecture as modelExpectedArch
			assertArrayEquals(modelExpectedArch.params().shape(), modelNow.params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(0).params().shape(), modelNow.getLayer(0).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(1).params().shape(), modelNow.getLayer(1).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(2).params().shape(), modelNow.getLayer(2).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(3).params().shape(), modelNow.getLayer(3).params().shape())
			modelNow.Params = modelExpectedArch.params()
			' fit should give the same results
			modelExpectedArch.fit(randomData)
			modelNow.fit(randomData)
			assertEquals(modelExpectedArch.score(), modelNow.score(), 0.000001)
			assertEquals(modelExpectedArch.params(), modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Remove And Add") void testRemoveAndAdd()
		Friend Overridable Sub testRemoveAndAdd()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(DataType.FLOAT, 10, 4), TestUtils.randomOneHot(DataType.FLOAT, 10, 3))
			Dim equivalentConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1))
			Dim overallConf As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).build()
			Dim modelToFineTune As New MultiLayerNetwork(equivalentConf.list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(5).build()).layer(1, (New DenseLayer.Builder()).nIn(5).nOut(2).build()).layer(2, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			modelToFineTune.init()
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).fineTuneConfiguration(overallConf).nOutReplace(0, 7, WeightInit.XAVIER, WeightInit.XAVIER).nOutReplace(2, 5, WeightInit.XAVIER).removeOutputLayer().addLayer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(5).nOut(3).updater(New Sgd(0.5)).activation(Activation.SOFTMAX).build()).build()
			Dim modelExpectedArch As New MultiLayerNetwork(equivalentConf.list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(7).build()).layer(1, (New DenseLayer.Builder()).nIn(7).nOut(2).build()).layer(2, (New DenseLayer.Builder()).nIn(2).nOut(5).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).updater(New Sgd(0.5)).nIn(5).nOut(3).build()).build())
			modelExpectedArch.init()
			' modelNow should have the same architecture as modelExpectedArch
			assertArrayEquals(modelExpectedArch.params().shape(), modelNow.params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(0).params().shape(), modelNow.getLayer(0).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(1).params().shape(), modelNow.getLayer(1).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(2).params().shape(), modelNow.getLayer(2).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(3).params().shape(), modelNow.getLayer(3).params().shape())
			modelNow.Params = modelExpectedArch.params()
			' fit should give the same results
			modelExpectedArch.fit(randomData)
			modelNow.fit(randomData)
			Dim scoreExpected As Double = modelExpectedArch.score()
			Dim scoreActual As Double = modelNow.score()
			assertEquals(scoreExpected, scoreActual, 1e-4)
			assertEquals(modelExpectedArch.params(), modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Remove And Processing") void testRemoveAndProcessing()
		Friend Overridable Sub testRemoveAndProcessing()
			Dim V_WIDTH As Integer = 130
			Dim V_HEIGHT As Integer = 130
			Dim V_NFRAMES As Integer = 150
			Dim confForArchitecture As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l2(0.001).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New AdaGrad(0.4)).list().layer(0, (New ConvolutionLayer.Builder(10, 10)).nIn(3).nOut(30).stride(4, 4).activation(Activation.RELU).weightInit(WeightInit.RELU).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3, 3).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(3, 3)).nIn(30).nOut(10).stride(2, 2).activation(Activation.RELU).weightInit(WeightInit.RELU).build()).layer(3, (New DenseLayer.Builder()).activation(Activation.RELU).nIn(490).nOut(50).weightInit(WeightInit.RELU).updater(New AdaGrad(0.5)).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).layer(4, (New GravesLSTM.Builder()).activation(Activation.SOFTSIGN).nIn(50).nOut(50).weightInit(WeightInit.XAVIER).updater(New AdaGrad(0.6)).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).layer(5, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(50).nOut(4).weightInit(WeightInit.XAVIER).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).inputPreProcessor(0, New RnnToCnnPreProcessor(V_HEIGHT, V_WIDTH, 3)).inputPreProcessor(3, New CnnToFeedForwardPreProcessor(7, 7, 10)).inputPreProcessor(4, New FeedForwardToRnnPreProcessor()).backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(V_NFRAMES \ 5).tBPTTBackwardLength(V_NFRAMES \ 5).build()
			Dim modelExpectedArch As New MultiLayerNetwork(confForArchitecture)
			modelExpectedArch.init()
			Dim modelToTweak As New MultiLayerNetwork((New NeuralNetConfiguration.Builder()).seed(12345).updater(New RmsProp(0.1)).list().layer(0, (New ConvolutionLayer.Builder(10, 10)).nIn(3).nOut(30).stride(4, 4).activation(Activation.RELU).weightInit(WeightInit.RELU).updater(New AdaGrad(0.1)).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(5, 5).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(6, 6)).nIn(30).nOut(10).stride(2, 2).activation(Activation.RELU).weightInit(WeightInit.RELU).build()).layer(3, (New DenseLayer.Builder()).activation(Activation.RELU).nIn(250).nOut(50).weightInit(WeightInit.RELU).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).updater(New RmsProp(0.01)).build()).layer(4, (New GravesLSTM.Builder()).activation(Activation.SOFTSIGN).nIn(50).nOut(25).weightInit(WeightInit.XAVIER).build()).layer(5, (New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(25).nOut(4).weightInit(WeightInit.XAVIER).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).inputPreProcessor(0, New RnnToCnnPreProcessor(V_HEIGHT, V_WIDTH, 3)).inputPreProcessor(3, New CnnToFeedForwardPreProcessor(5, 5, 10)).inputPreProcessor(4, New FeedForwardToRnnPreProcessor()).backpropType(BackpropType.TruncatedBPTT).tBPTTForwardLength(V_NFRAMES \ 5).tBPTTBackwardLength(V_NFRAMES \ 5).build())
			modelToTweak.init()
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToTweak)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).seed(12345).l2(0.001).updater(New AdaGrad(0.4)).weightInit(WeightInit.RELU).build()).removeLayersFromOutput(5).addLayer((New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(3, 3).stride(2, 2).build()).addLayer((New ConvolutionLayer.Builder(3, 3)).nIn(30).nOut(10).stride(2, 2).activation(Activation.RELU).weightInit(WeightInit.RELU).build()).addLayer((New DenseLayer.Builder()).activation(Activation.RELU).nIn(490).nOut(50).weightInit(WeightInit.RELU).updater(New AdaGrad(0.5)).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).addLayer((New GravesLSTM.Builder()).activation(Activation.SOFTSIGN).nIn(50).nOut(50).weightInit(WeightInit.XAVIER).updater(New AdaGrad(0.6)).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).addLayer((New RnnOutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(50).nOut(4).weightInit(WeightInit.XAVIER).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(10).build()).setInputPreProcessor(3, New CnnToFeedForwardPreProcessor(7, 7, 10)).setInputPreProcessor(4, New FeedForwardToRnnPreProcessor()).build()
			' modelNow should have the same architecture as modelExpectedArch
			assertEquals(modelExpectedArch.LayerWiseConfigurations.getConf(0).toJson(), modelNow.LayerWiseConfigurations.getConf(0).toJson())
			' some learning related info the subsampling layer will not be overwritten
			' assertTrue(modelExpectedArch.getLayerWiseConfigurations().getConf(1).toJson().equals(modelNow.getLayerWiseConfigurations().getConf(1).toJson()));
			assertEquals(modelExpectedArch.LayerWiseConfigurations.getConf(2).toJson(), modelNow.LayerWiseConfigurations.getConf(2).toJson())
			assertEquals(modelExpectedArch.LayerWiseConfigurations.getConf(3).toJson(), modelNow.LayerWiseConfigurations.getConf(3).toJson())
			assertEquals(modelExpectedArch.LayerWiseConfigurations.getConf(4).toJson(), modelNow.LayerWiseConfigurations.getConf(4).toJson())
			assertEquals(modelExpectedArch.LayerWiseConfigurations.getConf(5).toJson(), modelNow.LayerWiseConfigurations.getConf(5).toJson())
			assertArrayEquals(modelExpectedArch.params().shape(), modelNow.params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(0).params().shape(), modelNow.getLayer(0).params().shape())
			' subsampling has no params
			' assertArrayEquals(modelExpectedArch.getLayer(1).params().shape(), modelNow.getLayer(1).params().shape());
			assertArrayEquals(modelExpectedArch.getLayer(2).params().shape(), modelNow.getLayer(2).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(3).params().shape(), modelNow.getLayer(3).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(4).params().shape(), modelNow.getLayer(4).params().shape())
			assertArrayEquals(modelExpectedArch.getLayer(5).params().shape(), modelNow.getLayer(5).params().shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test All With CNN") void testAllWithCNN()
		Friend Overridable Sub testAllWithCNN()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(DataType.FLOAT, 10, 28 * 28 * 3).reshape(10, 3, 28, 28), TestUtils.randomOneHot(DataType.FLOAT, 10, 10))
			Dim modelToFineTune As New MultiLayerNetwork((New NeuralNetConfiguration.Builder()).seed(123).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(3).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build()).layer(3, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build()).layer(5, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(250).build()).layer(6, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(100).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 3)).build())
			modelToFineTune.init()
			' 10x20x12x12
			Dim asFrozenFeatures As INDArray = modelToFineTune.feedForwardToLayer(2, randomData.Features, False)(2)
			Dim equivalentConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.2)).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
			Dim overallConf As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.2)).build()
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).fineTuneConfiguration(overallConf).setFeatureExtractor(1).nOutReplace(4, 600, WeightInit.XAVIER).removeLayersFromOutput(2).addLayer((New DenseLayer.Builder()).activation(Activation.RELU).nIn(600).nOut(300).build()).addLayer((New DenseLayer.Builder()).activation(Activation.RELU).nIn(300).nOut(150).build()).addLayer((New DenseLayer.Builder()).activation(Activation.RELU).nIn(150).nOut(50).build()).addLayer((New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).activation(Activation.SOFTMAX).nIn(50).nOut(10).build()).build()
			Dim notFrozen As New MultiLayerNetwork(equivalentConf.list().layer(0, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(SubsamplingLayer.PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(600).build()).layer(3, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(300).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(150).build()).layer(5, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(50).build()).layer(6, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(10).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(12, 12, 20)).build())
			notFrozen.init()
			assertArrayEquals(modelToFineTune.getLayer(0).params().shape(), modelNow.getLayer(0).params().shape())
			' subsampling has no params
			' assertArrayEquals(modelExpectedArch.getLayer(1).params().shape(), modelNow.getLayer(1).params().shape());
			assertArrayEquals(notFrozen.getLayer(0).params().shape(), modelNow.getLayer(2).params().shape())
			modelNow.getLayer(2).Params = notFrozen.getLayer(0).params()
			' subsampling has no params
			' assertArrayEquals(notFrozen.getLayer(1).params().shape(), modelNow.getLayer(3).params().shape());
			assertArrayEquals(notFrozen.getLayer(2).params().shape(), modelNow.getLayer(4).params().shape())
			modelNow.getLayer(4).Params = notFrozen.getLayer(2).params()
			assertArrayEquals(notFrozen.getLayer(3).params().shape(), modelNow.getLayer(5).params().shape())
			modelNow.getLayer(5).Params = notFrozen.getLayer(3).params()
			assertArrayEquals(notFrozen.getLayer(4).params().shape(), modelNow.getLayer(6).params().shape())
			modelNow.getLayer(6).Params = notFrozen.getLayer(4).params()
			assertArrayEquals(notFrozen.getLayer(5).params().shape(), modelNow.getLayer(7).params().shape())
			modelNow.getLayer(7).Params = notFrozen.getLayer(5).params()
			assertArrayEquals(notFrozen.getLayer(6).params().shape(), modelNow.getLayer(8).params().shape())
			modelNow.getLayer(8).Params = notFrozen.getLayer(6).params()
			Dim i As Integer = 0
			Do While i < 3
				notFrozen.fit(New DataSet(asFrozenFeatures, randomData.Labels))
				modelNow.fit(randomData)
				i += 1
			Loop
			Dim expectedParams As INDArray = Nd4j.hstack(modelToFineTune.getLayer(0).params(), notFrozen.params())
			assertEquals(expectedParams, modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Fine Tune Override") void testFineTuneOverride()
		Friend Overridable Sub testFineTuneOverride()
			' Check that fine-tune overrides are selective - i.e., if I only specify a new LR, only the LR should be modified
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-4)).activation(Activation.TANH).weightInit(WeightInit.RELU).l1(0.1).l2(0.2).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(5).build()).layer(1, (New OutputLayer.Builder()).nIn(5).nOut(4).activation(Activation.HARDSIGMOID).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim net2 As MultiLayerNetwork = (New TransferLearning.Builder(net)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).updater(New Adam(2e-2)).backpropType(BackpropType.TruncatedBPTT).build()).build()
			' Check original net isn't modified:
			Dim l0 As BaseLayer = CType(net.getLayer(0).conf().getLayer(), BaseLayer)
			assertEquals(New Adam(1e-4), l0.getIUpdater())
			assertEquals(Activation.TANH.getActivationFunction(), l0.getActivationFn())
			assertEquals(New WeightInitRelu(), l0.getWeightInitFn())
			assertEquals(0.1, TestUtils.getL1(l0), 1e-6)
			Dim l1 As BaseLayer = CType(net.getLayer(1).conf().getLayer(), BaseLayer)
			assertEquals(New Adam(1e-4), l1.getIUpdater())
			assertEquals(Activation.HARDSIGMOID.getActivationFunction(), l1.getActivationFn())
			assertEquals(New WeightInitRelu(), l1.getWeightInitFn())
			assertEquals(0.2, TestUtils.getL2(l1), 1e-6)
			assertEquals(BackpropType.Standard, conf.getBackpropType())
			' Check new net has only the appropriate things modified (i.e., LR)
			l0 = CType(net2.getLayer(0).conf().getLayer(), BaseLayer)
			assertEquals(New Adam(2e-2), l0.getIUpdater())
			assertEquals(Activation.TANH.getActivationFunction(), l0.getActivationFn())
			assertEquals(New WeightInitRelu(), l0.getWeightInitFn())
			assertEquals(0.1, TestUtils.getL1(l0), 1e-6)
			l1 = CType(net2.getLayer(1).conf().getLayer(), BaseLayer)
			assertEquals(New Adam(2e-2), l1.getIUpdater())
			assertEquals(Activation.HARDSIGMOID.getActivationFunction(), l1.getActivationFn())
			assertEquals(New WeightInitRelu(), l1.getWeightInitFn())
			assertEquals(0.2, TestUtils.getL2(l1), 1e-6)
			assertEquals(BackpropType.TruncatedBPTT, net2.LayerWiseConfigurations.getBackpropType())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test All With CNN New") void testAllWithCNNNew()
		Friend Overridable Sub testAllWithCNNNew()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(DataType.FLOAT, 10, 28 * 28 * 3).reshape(10, 3, 28, 28), TestUtils.randomOneHot(10, 10))
			Dim modelToFineTune As New MultiLayerNetwork((New NeuralNetConfiguration.Builder()).seed(123).weightInit(WeightInit.XAVIER).updater(New Nesterovs(0.01, 0.9)).list().layer(0, (New ConvolutionLayer.Builder(5, 5)).nIn(3).stride(1, 1).nOut(20).activation(Activation.IDENTITY).build()).layer(1, (New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(2, (New ConvolutionLayer.Builder(5, 5)).stride(1, 1).nOut(50).activation(Activation.IDENTITY).build()).layer(3, (New SubsamplingLayer.Builder(PoolingType.MAX)).kernelSize(2, 2).stride(2, 2).build()).layer(4, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(500).build()).layer(5, (New DenseLayer.Builder()).activation(Activation.RELU).nOut(250).build()).layer(6, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nOut(100).activation(Activation.SOFTMAX).build()).setInputType(InputType.convolutionalFlat(28, 28, 3)).build())
			modelToFineTune.init()
			' 10x20x12x12
			Dim asFrozenFeatures As INDArray = modelToFineTune.feedForwardToLayer(2, randomData.Features, False)(2)
			Dim equivalentConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.2))
			Dim overallConf As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.2)).build()
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).fineTuneConfiguration(overallConf).setFeatureExtractor(1).removeLayersFromOutput(5).addLayer((New DenseLayer.Builder()).activation(Activation.RELU).nIn(12 * 12 * 20).nOut(300).build()).addLayer((New DenseLayer.Builder()).activation(Activation.RELU).nIn(300).nOut(150).build()).addLayer((New DenseLayer.Builder()).activation(Activation.RELU).nIn(150).nOut(50).build()).addLayer((New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).activation(Activation.SOFTMAX).nIn(50).nOut(10).build()).setInputPreProcessor(2, New CnnToFeedForwardPreProcessor(12, 12, 20)).build()
			Dim notFrozen As New MultiLayerNetwork(equivalentConf.list().layer(0, (New DenseLayer.Builder()).activation(Activation.RELU).nIn(12 * 12 * 20).nOut(300).build()).layer(1, (New DenseLayer.Builder()).activation(Activation.RELU).nIn(300).nOut(150).build()).layer(2, (New DenseLayer.Builder()).activation(Activation.RELU).nIn(150).nOut(50).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).nIn(50).nOut(10).activation(Activation.SOFTMAX).build()).inputPreProcessor(0, New CnnToFeedForwardPreProcessor(12, 12, 20)).build())
			notFrozen.init()
			assertArrayEquals(modelToFineTune.getLayer(0).params().shape(), modelNow.getLayer(0).params().shape())
			' subsampling has no params
			' assertArrayEquals(modelExpectedArch.getLayer(1).params().shape(), modelNow.getLayer(1).params().shape());
			assertArrayEquals(notFrozen.getLayer(0).params().shape(), modelNow.getLayer(2).params().shape())
			modelNow.getLayer(2).Params = notFrozen.getLayer(0).params()
			assertArrayEquals(notFrozen.getLayer(1).params().shape(), modelNow.getLayer(3).params().shape())
			modelNow.getLayer(3).Params = notFrozen.getLayer(1).params()
			assertArrayEquals(notFrozen.getLayer(2).params().shape(), modelNow.getLayer(4).params().shape())
			modelNow.getLayer(4).Params = notFrozen.getLayer(2).params()
			assertArrayEquals(notFrozen.getLayer(3).params().shape(), modelNow.getLayer(5).params().shape())
			modelNow.getLayer(5).Params = notFrozen.getLayer(3).params()
			Dim i As Integer = 0
			Do While i < 3
				notFrozen.fit(New DataSet(asFrozenFeatures, randomData.Labels))
				modelNow.fit(randomData)
				i += 1
			Loop
			Dim expectedParams As INDArray = Nd4j.hstack(modelToFineTune.getLayer(0).params(), notFrozen.params())
			assertEquals(expectedParams, modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Object Overrides") void testObjectOverrides()
		Friend Overridable Sub testObjectOverrides()
			' https://github.com/eclipse/deeplearning4j/issues/4368
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(0.5).weightNoise(New DropConnect(0.5)).l2(0.5).constrainWeights(New UnitNormConstraint()).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).build()
			Dim orig As New MultiLayerNetwork(conf)
			orig.init()
			Dim ftc As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).dropOut(0).weightNoise(Nothing).constraints(Nothing).l2(0.0).build()
			Dim transfer As MultiLayerNetwork = (New TransferLearning.Builder(orig)).fineTuneConfiguration(ftc).build()
			Dim l As DenseLayer = CType(transfer.getLayer(0).conf().getLayer(), DenseLayer)
			assertNull(l.getIDropout())
			assertNull(l.getWeightNoise())
			assertNull(l.getConstraints())
			assertNull(TestUtils.getL2Reg(l))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Transfer Learning Subsequent") void testTransferLearningSubsequent()
		Friend Overridable Sub testTransferLearningSubsequent()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray input = org.nd4j.linalg.factory.Nd4j.create(6, 6, 6, 6);
			Dim input As INDArray = Nd4j.create(6, 6, 6, 6)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork net = new org.deeplearning4j.nn.multilayer.MultiLayerNetwork(new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().weightInit(new org.deeplearning4j.nn.conf.distribution.ConstantDistribution(666)).list().setInputType(org.deeplearning4j.nn.conf.inputs.InputType.inferInputTypes(input)[0]).layer(new Convolution2D.Builder(3, 3).nOut(10).build()).layer(new Convolution2D.Builder(1, 1).nOut(3).build()).layer(new OutputLayer.Builder().nOut(2).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).build()).build());
			Dim net As New MultiLayerNetwork((New NeuralNetConfiguration.Builder()).weightInit(New ConstantDistribution(666)).list().setInputType(InputType.inferInputTypes(input)(0)).layer((New Convolution2D.Builder(3, 3)).nOut(10).build()).layer((New Convolution2D.Builder(1, 1)).nOut(3).build()).layer((New OutputLayer.Builder()).nOut(2).lossFunction(LossFunctions.LossFunction.MSE).build()).build())
			net.init()
			Dim newGraph As MultiLayerNetwork = (New TransferLearning.Builder(net)).fineTuneConfiguration((New FineTuneConfiguration.Builder()).build()).nOutReplace(0, 7, New ConstantDistribution(333)).nOutReplace(1, 3, New ConstantDistribution(111)).removeLayersFromOutput(1).addLayer((New OutputLayer.Builder()).nIn(48).nOut(2).lossFunction(LossFunctions.LossFunction.MSE).build()).setInputPreProcessor(2, New CnnToFeedForwardPreProcessor(4, 4, 3)).build()
			newGraph.init()
			assertEquals(7, newGraph.layerInputSize(1), "Incorrect # inputs")
			newGraph.output(input)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Change N Out N In") void testChangeNOutNIn()
		Friend Overridable Sub testChangeNOutNIn()
			Dim input As INDArray = Nd4j.create(New Long() { 1, 2, 4, 4 })
			Dim net As New MultiLayerNetwork((New NeuralNetConfiguration.Builder()).list().setInputType(InputType.inferInputTypes(input)(0)).layer((New Convolution2D.Builder(1, 1)).nOut(10).build()).layer((New SubsamplingLayer.Builder(1, 1)).build()).layer((New Convolution2D.Builder(1, 1)).nOut(7).build()).layer((New OutputLayer.Builder()).activation(Activation.SOFTMAX).nOut(2).build()).build())
			net.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork newNet = new TransferLearning.Builder(net).nOutReplace(0, 5, org.deeplearning4j.nn.weights.WeightInit.XAVIER).nInReplace(2, 5, org.deeplearning4j.nn.weights.WeightInit.XAVIER).build();
			Dim newNet As MultiLayerNetwork = (New TransferLearning.Builder(net)).nOutReplace(0, 5, WeightInit.XAVIER).nInReplace(2, 5, WeightInit.XAVIER).build()
			newNet.init()
			assertEquals(5, newNet.layerSize(0), "Incorrect number of outputs!")
			assertEquals(5, newNet.layerInputSize(2), "Incorrect number of inputs!")
			newNet.output(input)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Transfer Learning Same Diff Layers") void testTransferLearningSameDiffLayers()
		Friend Overridable Sub testTransferLearningSameDiffLayers()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dataType(DataType.DOUBLE).activation(Activation.TANH).updater(New Adam(0.01)).weightInit(WeightInit.XAVIER).list().layer((New LSTM.Builder()).nOut(8).build()).layer((New SelfAttentionLayer.Builder()).nOut(4).nHeads(2).projectInput(True).build()).layer((New GlobalPoolingLayer.Builder()).poolingType(PoolingType.MAX).build()).layer((New OutputLayer.Builder()).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).setInputType(InputType.recurrent(4)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim [in] As INDArray = Nd4j.rand(DataType.FLOAT, 3, 4, 5)
			Dim [out] As INDArray = net.output([in])
			Dim net2 As MultiLayerNetwork = (New TransferLearning.Builder(net)).fineTuneConfiguration(FineTuneConfiguration.builder().updater(New Adam(0.01)).build()).removeLayersFromOutput(1).addLayer((New OutputLayer.Builder()).nIn(4).nOut(2).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			net2.setParam("3_W", net.getParam("3_W"))
			net2.setParam("3_b", net.getParam("3_b"))
			Dim p1 As IDictionary(Of String, INDArray) = net.paramTable()
			Dim p2 As IDictionary(Of String, INDArray) = net2.paramTable()
			For Each s As String In p1.Keys
				Dim i1 As INDArray = p1(s)
				Dim i2 As INDArray = p2(s)
				assertEquals(i1, i2,s)
			Next s
			Dim out2 As INDArray = net2.output([in])
			assertEquals([out], out2)
		End Sub
	End Class

End Namespace