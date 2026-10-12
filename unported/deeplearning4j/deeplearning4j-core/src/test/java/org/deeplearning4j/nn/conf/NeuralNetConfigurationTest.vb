Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports DefaultStepFunction = org.deeplearning4j.nn.conf.stepfunctions.DefaultStepFunction
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports org.deeplearning4j.nn.weights
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports StochasticGradientDescent = org.deeplearning4j.optimize.solvers.StochasticGradientDescent
Imports NegativeDefaultStepFunction = org.deeplearning4j.optimize.stepfunctions.NegativeDefaultStepFunction
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LeakyReLU = org.nd4j.linalg.api.ops.impl.scalar.LeakyReLU
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotSame
import static org.junit.jupiter.api.Assertions.assertTrue
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
Namespace org.deeplearning4j.nn.conf

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Neural Net Configuration Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class NeuralNetConfigurationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class NeuralNetConfigurationTest
		Inherits BaseDL4JTest

		Friend ReadOnly trainingSet As DataSet = createData()

		Public Overridable Function createData() As DataSet
			Dim numFeatures As Integer = 40
			' have to be at least two or else output layer gradient is a scalar and cause exception
			Dim input As INDArray = Nd4j.create(2, numFeatures)
			Dim labels As INDArray = Nd4j.create(2, 2)
			Dim row0 As INDArray = Nd4j.create(1, numFeatures)
			row0.assign(0.1)
			input.putRow(0, row0)
			' set the 4th column
			labels.put(0, 1, 1)
			Dim row1 As INDArray = Nd4j.create(1, numFeatures)
			row1.assign(0.2)
			input.putRow(1, row1)
			' set the 2nd column
			labels.put(1, 0, 1)
			Return New DataSet(input, labels)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Json") void testJson()
		Friend Overridable Sub testJson()
			Dim conf As NeuralNetConfiguration = getConfig(1, 1, New WeightInitXavier(), True)
			Dim json As String = conf.toJson()
			Dim read As NeuralNetConfiguration = NeuralNetConfiguration.fromJson(json)
			assertEquals(conf, read)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Yaml") void testYaml()
		Friend Overridable Sub testYaml()
			Dim conf As NeuralNetConfiguration = getConfig(1, 1, New WeightInitXavier(), True)
			Dim json As String = conf.toYaml()
			Dim read As NeuralNetConfiguration = NeuralNetConfiguration.fromYaml(json)
			assertEquals(conf, read)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Clone") void testClone()
		Friend Overridable Sub testClone()
			Dim conf As NeuralNetConfiguration = getConfig(1, 1, New WeightInitUniform(), True)
			Dim bl As BaseLayer = CType(conf.getLayer(), BaseLayer)
			conf.setStepFunction(New DefaultStepFunction())
			Dim conf2 As NeuralNetConfiguration = conf.clone()
			assertEquals(conf, conf2)
			assertNotSame(conf, conf2)
			assertNotSame(conf.getLayer(), conf2.getLayer())
			assertNotSame(conf.getStepFunction(), conf2.getStepFunction())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test RNG") void testRNG()
		Friend Overridable Sub testRNG()
			Dim layer As DenseLayer = (New DenseLayer.Builder()).nIn(trainingSet.numInputs()).nOut(trainingSet.numOutcomes()).weightInit(WeightInit.UNIFORM).activation(Activation.TANH).build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).layer(layer).build()
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Dim model As Layer = conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
			Dim modelWeights As INDArray = model.getParam(DefaultParamInitializer.WEIGHT_KEY)
			Dim layer2 As DenseLayer = (New DenseLayer.Builder()).nIn(trainingSet.numInputs()).nOut(trainingSet.numOutcomes()).weightInit(WeightInit.UNIFORM).activation(Activation.TANH).build()
			Dim conf2 As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).layer(layer2).build()
			Dim numParams2 As Long = conf2.getLayer().initializer().numParams(conf)
			Dim params2 As INDArray = Nd4j.create(1, numParams)
			Dim model2 As Layer = conf2.getLayer().instantiate(conf2, Nothing, 0, params2, True, params.dataType())
			Dim modelWeights2 As INDArray = model2.getParam(DefaultParamInitializer.WEIGHT_KEY)
			assertEquals(modelWeights, modelWeights2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Seed Size") void testSetSeedSize()
		Friend Overridable Sub testSetSeedSize()
			Nd4j.Random.setSeed(123)
			Dim model As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitXavier(), True)
			Dim modelWeights As INDArray = model.getParam(DefaultParamInitializer.WEIGHT_KEY)
			Nd4j.Random.setSeed(123)
			Dim model2 As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitXavier(), True)
			Dim modelWeights2 As INDArray = model2.getParam(DefaultParamInitializer.WEIGHT_KEY)
			assertEquals(modelWeights, modelWeights2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Seed Normalized") void testSetSeedNormalized()
		Friend Overridable Sub testSetSeedNormalized()
			Nd4j.Random.setSeed(123)
			Dim model As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitXavier(), True)
			Dim modelWeights As INDArray = model.getParam(DefaultParamInitializer.WEIGHT_KEY)
			Nd4j.Random.setSeed(123)
			Dim model2 As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitXavier(), True)
			Dim modelWeights2 As INDArray = model2.getParam(DefaultParamInitializer.WEIGHT_KEY)
			assertEquals(modelWeights, modelWeights2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Seed Xavier") void testSetSeedXavier()
		Friend Overridable Sub testSetSeedXavier()
			Nd4j.Random.setSeed(123)
			Dim model As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitUniform(), True)
			Dim modelWeights As INDArray = model.getParam(DefaultParamInitializer.WEIGHT_KEY)
			Nd4j.Random.setSeed(123)
			Dim model2 As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitUniform(), True)
			Dim modelWeights2 As INDArray = model2.getParam(DefaultParamInitializer.WEIGHT_KEY)
			assertEquals(modelWeights, modelWeights2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Set Seed Distribution") void testSetSeedDistribution()
		Friend Overridable Sub testSetSeedDistribution()
			Nd4j.Random.setSeed(123)
			Dim model As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitDistribution(New NormalDistribution(1, 1)), True)
			Dim modelWeights As INDArray = model.getParam(DefaultParamInitializer.WEIGHT_KEY)
			Nd4j.Random.setSeed(123)
			Dim model2 As Layer = getLayer(trainingSet.numInputs(), trainingSet.numOutcomes(), New WeightInitDistribution(New NormalDistribution(1, 1)), True)
			Dim modelWeights2 As INDArray = model2.getParam(DefaultParamInitializer.WEIGHT_KEY)
			assertEquals(modelWeights, modelWeights2)
		End Sub

		Private Shared Function getConfig(ByVal nIn As Integer, ByVal nOut As Integer, ByVal weightInit As IWeightInit, ByVal pretrain As Boolean) As NeuralNetConfiguration
			Dim layer As DenseLayer = (New DenseLayer.Builder()).nIn(nIn).nOut(nOut).weightInit(weightInit).activation(Activation.TANH).build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.CONJUGATE_GRADIENT).layer(layer).build()
			Return conf
		End Function

		Private Shared Function getLayer(ByVal nIn As Integer, ByVal nOut As Integer, ByVal weightInit As IWeightInit, ByVal preTrain As Boolean) As Layer
			Dim conf As NeuralNetConfiguration = getConfig(nIn, nOut, weightInit, preTrain)
			Dim numParams As Long = conf.getLayer().initializer().numParams(conf)
			Dim params As INDArray = Nd4j.create(1, numParams)
			Return conf.getLayer().instantiate(conf, Nothing, 0, params, True, params.dataType())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Learning Rate By Param") void testLearningRateByParam()
		Friend Overridable Sub testLearningRateByParam()
			Dim lr As Double = 0.01
			Dim biasLr As Double = 0.02
			Dim nIns() As Integer = { 4, 3, 3 }
			Dim nOuts() As Integer = { 3, 3, 3 }
			Dim oldScore As Integer = 1
			Dim newScore As Integer = 1
			Dim iteration As Integer = 3
			Dim gradientW As INDArray = Nd4j.ones(nIns(0), nOuts(0))
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.3)).list().layer(0, (New DenseLayer.Builder()).nIn(nIns(0)).nOut(nOuts(0)).updater(New Sgd(lr)).biasUpdater(New Sgd(biasLr)).build()).layer(1, (New BatchNormalization.Builder()).nIn(nIns(1)).nOut(nOuts(1)).updater(New Sgd(0.7)).build()).layer(2, (New OutputLayer.Builder()).nIn(nIns(2)).nOut(nOuts(2)).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim opt As ConvexOptimizer = New StochasticGradientDescent(net.DefaultConfiguration, New NegativeDefaultStepFunction(), Nothing, net)
			assertEquals(lr, CType(net.getLayer(0).conf().getLayer().getUpdaterByParam("W"), Sgd).getLearningRate(), 1e-4)
			assertEquals(biasLr, CType(net.getLayer(0).conf().getLayer().getUpdaterByParam("b"), Sgd).getLearningRate(), 1e-4)
			assertEquals(0.7, CType(net.getLayer(1).conf().getLayer().getUpdaterByParam("gamma"), Sgd).getLearningRate(), 1e-4)
			' From global LR
			assertEquals(0.3, CType(net.getLayer(2).conf().getLayer().getUpdaterByParam("W"), Sgd).getLearningRate(), 1e-4)
			' From global LR
			assertEquals(0.3, CType(net.getLayer(2).conf().getLayer().getUpdaterByParam("W"), Sgd).getLearningRate(), 1e-4)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Leakyrelu Alpha") void testLeakyreluAlpha()
		Friend Overridable Sub testLeakyreluAlpha()
			' FIXME: Make more generic to use neuralnetconfs
			Dim sizeX As Integer = 4
			Dim scaleX As Integer = 10
			Console.WriteLine("Here is a leaky vector..")
			Dim leakyVector As INDArray = Nd4j.linspace(-1, 1, sizeX, Nd4j.dataType())
			leakyVector = leakyVector.mul(scaleX)
			Console.WriteLine(leakyVector)
			Dim myAlpha As Double = 0.5
			Console.WriteLine("======================")
			Console.WriteLine("Exec and Return: Leaky Relu transformation with alpha = 0.5 ..")
			Console.WriteLine("======================")
			Dim outDef As INDArray = Nd4j.Executioner.exec(New LeakyReLU(leakyVector.dup(), myAlpha))
			Console.WriteLine(outDef)
			Dim confActivation As String = "leakyrelu"
			Dim confExtra() As Object = { myAlpha }
			Dim outMine As INDArray = Nd4j.Executioner.exec(New LeakyReLU(leakyVector.dup(), myAlpha))
			Console.WriteLine("======================")
			Console.WriteLine("Exec and Return: Leaky Relu transformation with a value via getOpFactory")
			Console.WriteLine("======================")
			Console.WriteLine(outMine)
			' Test equality for ndarray elementwise
			' assertArrayEquals(..)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test L 1 L 2 By Param") void testL1L2ByParam()
		Friend Overridable Sub testL1L2ByParam()
			Dim l1 As Double = 0.01
			Dim l2 As Double = 0.07
			Dim nIns() As Integer = { 4, 3, 3 }
			Dim nOuts() As Integer = { 3, 3, 3 }
			Dim oldScore As Integer = 1
			Dim newScore As Integer = 1
			Dim iteration As Integer = 3
			Dim gradientW As INDArray = Nd4j.ones(nIns(0), nOuts(0))
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l1(l1).l2(l2).list().layer(0, (New DenseLayer.Builder()).nIn(nIns(0)).nOut(nOuts(0)).build()).layer(1, (New BatchNormalization.Builder()).nIn(nIns(1)).nOut(nOuts(1)).l2(0.5).build()).layer(2, (New OutputLayer.Builder()).nIn(nIns(2)).nOut(nOuts(2)).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim opt As ConvexOptimizer = New StochasticGradientDescent(net.DefaultConfiguration, New NegativeDefaultStepFunction(), Nothing, net)
			assertEquals(l1, TestUtils.getL1(net.getLayer(0).conf().getLayer().getRegularizationByParam("W")), 1e-4)
			Dim r As IList(Of Regularization) = net.getLayer(0).conf().getLayer().getRegularizationByParam("b")
			assertEquals(0, r.Count)
			r = net.getLayer(1).conf().getLayer().getRegularizationByParam("beta")
			assertTrue(r Is Nothing OrElse r.Count = 0)
			r = net.getLayer(1).conf().getLayer().getRegularizationByParam("gamma")
			assertTrue(r Is Nothing OrElse r.Count = 0)
			r = net.getLayer(1).conf().getLayer().getRegularizationByParam("mean")
			assertTrue(r Is Nothing OrElse r.Count = 0)
			r = net.getLayer(1).conf().getLayer().getRegularizationByParam("var")
			assertTrue(r Is Nothing OrElse r.Count = 0)
			assertEquals(l2, TestUtils.getL2(net.getLayer(2).conf().getLayer().getRegularizationByParam("W")), 1e-4)
			r = net.getLayer(2).conf().getLayer().getRegularizationByParam("b")
			assertTrue(r Is Nothing OrElse r.Count = 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Layer Pretrain Config") void testLayerPretrainConfig()
		Friend Overridable Sub testLayerPretrainConfig()
			Dim pretrain As Boolean = True
			Dim layer As VariationalAutoencoder = (New VariationalAutoencoder.Builder()).nIn(10).nOut(5).updater(New Sgd(1e-1)).lossFunction(LossFunctions.LossFunction.KL_DIVERGENCE).build()
			Dim conf As NeuralNetConfiguration = (New NeuralNetConfiguration.Builder()).seed(42).layer(layer).build()
		End Sub
	End Class

End Namespace