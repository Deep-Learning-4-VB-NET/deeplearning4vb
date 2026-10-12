Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports FineTuneConfiguration = org.deeplearning4j.nn.transferlearning.FineTuneConfiguration
Imports TransferLearning = org.deeplearning4j.nn.transferlearning.TransferLearning
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
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
'ORIGINAL LINE: @Slf4j @DisplayName("Frozen Layer Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class FrozenLayerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class FrozenLayerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Frozen") void testFrozen()
		Friend Overridable Sub testFrozen()
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 3))
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY)
			Dim finetune As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).updater(New Sgd(0.1)).build()
			Dim modelToFineTune As New MultiLayerNetwork(overallConf.clone().list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).build()).layer(2, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(3, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			modelToFineTune.init()
			Dim ff As IList(Of INDArray) = modelToFineTune.feedForwardToLayer(2, randomData.Features, False)
			Dim asFrozenFeatures As INDArray = ff(2)
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).fineTuneConfiguration(finetune).setFeatureExtractor(1).build()
			Dim paramsLastTwoLayers As INDArray = Nd4j.hstack(modelToFineTune.getLayer(2).params(), modelToFineTune.getLayer(3).params())
			Dim notFrozen As New MultiLayerNetwork(overallConf.clone().list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build(), paramsLastTwoLayers)
			' assertEquals(modelNow.getLayer(2).conf(), notFrozen.getLayer(0).conf());  //Equal, other than names
			' assertEquals(modelNow.getLayer(3).conf(), notFrozen.getLayer(1).conf());  //Equal, other than names
			' Check: forward pass
			Dim outNow As INDArray = modelNow.output(randomData.Features)
			Dim outNotFrozen As INDArray = notFrozen.output(asFrozenFeatures)
			assertEquals(outNow, outNotFrozen)
			For i As Integer = 0 To 4
				notFrozen.fit(New DataSet(asFrozenFeatures, randomData.Labels))
				modelNow.fit(randomData)
			Next i
			Dim expected As INDArray = Nd4j.hstack(modelToFineTune.getLayer(0).params(), modelToFineTune.getLayer(1).params(), notFrozen.params())
			Dim act As INDArray = modelNow.params()
			assertEquals(expected, act)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Clone MLN Frozen") void cloneMLNFrozen()
		Friend Overridable Sub cloneMLNFrozen()
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 3))
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY)
			Dim modelToFineTune As New MultiLayerNetwork(overallConf.list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).build()).layer(2, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(3, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			modelToFineTune.init()
			Dim asFrozenFeatures As INDArray = modelToFineTune.feedForwardToLayer(2, randomData.Features, False)(2)
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).setFeatureExtractor(1).build()
			Dim clonedModel As MultiLayerNetwork = modelNow.clone()
			' Check json
			assertEquals(modelNow.LayerWiseConfigurations.toJson(), clonedModel.LayerWiseConfigurations.toJson())
			' Check params
			assertEquals(modelNow.params(), clonedModel.params())
			Dim notFrozen As New MultiLayerNetwork(overallConf.list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build(), Nd4j.hstack(modelToFineTune.getLayer(2).params(), modelToFineTune.getLayer(3).params()))
			Dim i As Integer = 0
			Do While i < 5
				notFrozen.fit(New DataSet(asFrozenFeatures, randomData.Labels))
				modelNow.fit(randomData)
				clonedModel.fit(randomData)
				i += 1
			Loop
			Dim expectedParams As INDArray = Nd4j.hstack(modelToFineTune.getLayer(0).params(), modelToFineTune.getLayer(1).params(), notFrozen.params())
			assertEquals(expectedParams, modelNow.params())
			assertEquals(expectedParams, clonedModel.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Frozen Comp Graph") void testFrozenCompGraph()
		Friend Overridable Sub testFrozenCompGraph()
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 3))
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY)
			Dim modelToFineTune As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "layer0In").addLayer("layer1", (New DenseLayer.Builder()).nIn(3).nOut(2).build(), "layer0").addLayer("layer2", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "layer1").addLayer("layer3", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer2").setOutputs("layer3").build())
			modelToFineTune.init()
			Dim asFrozenFeatures As INDArray = modelToFineTune.feedForward(randomData.Features, False)("layer1")
			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToFineTune)).setFeatureExtractor("layer1").build()
			Dim notFrozen As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "layer0In").addLayer("layer1", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer0").setOutputs("layer1").build())
			notFrozen.init()
			notFrozen.Params = Nd4j.hstack(modelToFineTune.getLayer("layer2").params(), modelToFineTune.getLayer("layer3").params())
			Dim i As Integer = 0
			Do While i < 5
				notFrozen.fit(New DataSet(asFrozenFeatures, randomData.Labels))
				modelNow.fit(randomData)
				i += 1
			Loop
			assertEquals(Nd4j.hstack(modelToFineTune.getLayer("layer0").params(), modelToFineTune.getLayer("layer1").params(), notFrozen.params()), modelNow.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Clone Comp Graph Frozen") void cloneCompGraphFrozen()
		Friend Overridable Sub cloneCompGraphFrozen()
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 3))
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).activation(Activation.IDENTITY)
			Dim modelToFineTune As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "layer0In").addLayer("layer1", (New DenseLayer.Builder()).nIn(3).nOut(2).build(), "layer0").addLayer("layer2", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "layer1").addLayer("layer3", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer2").setOutputs("layer3").build())
			modelToFineTune.init()
			Dim asFrozenFeatures As INDArray = modelToFineTune.feedForward(randomData.Features, False)("layer1")
			Dim modelNow As ComputationGraph = (New TransferLearning.GraphBuilder(modelToFineTune)).setFeatureExtractor("layer1").build()
			Dim clonedModel As ComputationGraph = modelNow.clone()
			' Check json
			assertEquals(clonedModel.Configuration.toJson(), modelNow.Configuration.toJson())
			' Check params
			assertEquals(modelNow.params(), clonedModel.params())
			Dim notFrozen As New ComputationGraph(overallConf.graphBuilder().addInputs("layer0In").addLayer("layer0", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "layer0In").addLayer("layer1", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build(), "layer0").setOutputs("layer1").build())
			notFrozen.init()
			notFrozen.Params = Nd4j.hstack(modelToFineTune.getLayer("layer2").params(), modelToFineTune.getLayer("layer3").params())
			Dim i As Integer = 0
			Do While i < 5
				notFrozen.fit(New DataSet(asFrozenFeatures, randomData.Labels))
				modelNow.fit(randomData)
				clonedModel.fit(randomData)
				i += 1
			Loop
			Dim expectedParams As INDArray = Nd4j.hstack(modelToFineTune.getLayer("layer0").params(), modelToFineTune.getLayer("layer1").params(), notFrozen.params())
			assertEquals(expectedParams, modelNow.params())
			assertEquals(expectedParams, clonedModel.params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Frozen Layer Instantiation") void testFrozenLayerInstantiation()
		Friend Overridable Sub testFrozenLayerInstantiation()
			' We need to be able to instantitate frozen layers from JSON etc, and have them be the same as if
			' they were initialized via the builder
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(1, (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(2, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayer((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build())).layer(1, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayer((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build())).layer(2, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim net1 As New MultiLayerNetwork(conf1)
			net1.init()
			Dim net2 As New MultiLayerNetwork(conf2)
			net2.init()
			assertEquals(net1.params(), net2.params())
			Dim json As String = conf2.toJson()
			Dim fromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(json)
			assertEquals(conf2, fromJson)
			Dim net3 As New MultiLayerNetwork(fromJson)
			net3.init()
			Dim input As INDArray = Nd4j.rand(10, 10)
			Dim out2 As INDArray = net2.output(input)
			Dim out3 As INDArray = net3.output(input)
			assertEquals(out2, out3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Frozen Layer Instantiation Comp Graph") void testFrozenLayerInstantiationCompGraph()
		Friend Overridable Sub testFrozenLayerInstantiationCompGraph()
			' We need to be able to instantitate frozen layers from JSON etc, and have them be the same as if
			' they were initialized via the builder
			Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build(), "0").addLayer("2", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "1").setOutputs("2").build()
			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New org.deeplearning4j.nn.conf.layers.misc.FrozenLayer.Builder()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).build(), "in").addLayer("1", (New org.deeplearning4j.nn.conf.layers.misc.FrozenLayer.Builder()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).build(), "0").addLayer("2", (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "1").setOutputs("2").build()
			Dim net1 As New ComputationGraph(conf1)
			net1.init()
			Dim net2 As New ComputationGraph(conf2)
			net2.init()
			assertEquals(net1.params(), net2.params())
			Dim json As String = conf2.toJson()
			Dim fromJson As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(json)
			assertEquals(conf2, fromJson)
			Dim net3 As New ComputationGraph(fromJson)
			net3.init()
			Dim input As INDArray = Nd4j.rand(10, 10)
			Dim out2 As INDArray = net2.outputSingle(input)
			Dim out3 As INDArray = net3.outputSingle(input)
			assertEquals(out2, out3)
		End Sub
	End Class

End Namespace