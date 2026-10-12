Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
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
import static org.junit.jupiter.api.Assertions.assertNotEquals
import static org.junit.jupiter.api.Assertions.assertNotNull
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
'ORIGINAL LINE: @Slf4j @DisplayName("Frozen Layer With Backprop Test") @NativeTag @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) class FrozenLayerWithBackpropTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class FrozenLayerWithBackpropTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Frozen With Backprop Layer Instantiation") void testFrozenWithBackpropLayerInstantiation()
		Friend Overridable Sub testFrozenWithBackpropLayerInstantiation()
			' We need to be able to instantitate frozen layers from JSON etc, and have them be the same as if
			' they were initialized via the builder
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(1, (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
			Dim conf2 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).list().layer(0, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build())).layer(1, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build())).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build()).build()
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
			Dim conf1 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build(), "0").addLayer("2", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "1").setOutputs("2").build()
			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).graphBuilder().addInputs("in").addLayer("0", New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()), "in").addLayer("1", New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(10).nOut(10).activation(Activation.TANH).weightInit(WeightInit.XAVIER).build()), "0").addLayer("2", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(10).build(), "1").setOutputs("2").build()
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Multi Layer Network Frozen Layer Params After Backprop") void testMultiLayerNetworkFrozenLayerParamsAfterBackprop()
		Friend Overridable Sub testMultiLayerNetworkFrozenLayerParamsAfterBackprop()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(100, 4), Nd4j.rand(100, 1))
			Dim conf1 As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).updater(New Sgd(2)).list().layer((New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(3).nOut(4).build())).layer(New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(4).nOut(2).build())).layer(New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.TANH).nIn(2).nOut(1).build())).build()
			Dim network As New MultiLayerNetwork(conf1)
			network.init()
			Dim unfrozenLayerParams As INDArray = network.getLayer(0).params().dup()
			Dim frozenLayerParams1 As INDArray = network.getLayer(1).params().dup()
			Dim frozenLayerParams2 As INDArray = network.getLayer(2).params().dup()
			Dim frozenOutputLayerParams As INDArray = network.getLayer(3).params().dup()
			For i As Integer = 0 To 99
				network.fit(randomData)
			Next i
			assertNotEquals(unfrozenLayerParams, network.getLayer(0).params())
			assertEquals(frozenLayerParams1, network.getLayer(1).params())
			assertEquals(frozenLayerParams2, network.getLayer(2).params())
			assertEquals(frozenOutputLayerParams, network.getLayer(3).params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Computation Graph Frozen Layer Params After Backprop") void testComputationGraphFrozenLayerParamsAfterBackprop()
		Friend Overridable Sub testComputationGraphFrozenLayerParamsAfterBackprop()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(100, 4), Nd4j.rand(100, 1))
			Dim frozenBranchName As String = "B1-"
			Dim unfrozenBranchName As String = "B2-"
			Dim initialLayer As String = "initial"
			Dim frozenBranchUnfrozenLayer0 As String = frozenBranchName & "0"
			Dim frozenBranchFrozenLayer1 As String = frozenBranchName & "1"
			Dim frozenBranchFrozenLayer2 As String = frozenBranchName & "2"
			Dim frozenBranchOutput As String = frozenBranchName & "Output"
			Dim unfrozenLayer0 As String = unfrozenBranchName & "0"
			Dim unfrozenLayer1 As String = unfrozenBranchName & "1"
			Dim unfrozenBranch2 As String = unfrozenBranchName & "Output"
			Dim computationGraphConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(2.0)).seed(12345).graphBuilder().addInputs("input").addLayer(initialLayer, (New DenseLayer.Builder()).nIn(4).nOut(4).build(), "input").addLayer(frozenBranchUnfrozenLayer0, (New DenseLayer.Builder()).nIn(4).nOut(3).build(), initialLayer).addLayer(frozenBranchFrozenLayer1, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(3).nOut(4).build()), frozenBranchUnfrozenLayer0).addLayer(frozenBranchFrozenLayer2, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(4).nOut(2).build()), frozenBranchFrozenLayer1).addLayer(unfrozenLayer0, (New DenseLayer.Builder()).nIn(4).nOut(4).build(), initialLayer).addLayer(unfrozenLayer1, (New DenseLayer.Builder()).nIn(4).nOut(2).build(), unfrozenLayer0).addLayer(unfrozenBranch2, (New DenseLayer.Builder()).nIn(2).nOut(1).build(), unfrozenLayer1).addVertex("merge", New MergeVertex(), frozenBranchFrozenLayer2, unfrozenBranch2).addLayer(frozenBranchOutput, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.TANH).nIn(3).nOut(1).build()), "merge").setOutputs(frozenBranchOutput).build()
			Dim computationGraph As New ComputationGraph(computationGraphConf)
			computationGraph.init()
			Dim unfrozenLayerParams As INDArray = computationGraph.getLayer(frozenBranchUnfrozenLayer0).params().dup()
			Dim frozenLayerParams1 As INDArray = computationGraph.getLayer(frozenBranchFrozenLayer1).params().dup()
			Dim frozenLayerParams2 As INDArray = computationGraph.getLayer(frozenBranchFrozenLayer2).params().dup()
			Dim frozenOutputLayerParams As INDArray = computationGraph.getLayer(frozenBranchOutput).params().dup()
			For i As Integer = 0 To 99
				computationGraph.fit(randomData)
			Next i
			assertNotEquals(unfrozenLayerParams, computationGraph.getLayer(frozenBranchUnfrozenLayer0).params())
			assertEquals(frozenLayerParams1, computationGraph.getLayer(frozenBranchFrozenLayer1).params())
			assertEquals(frozenLayerParams2, computationGraph.getLayer(frozenBranchFrozenLayer2).params())
			assertEquals(frozenOutputLayerParams, computationGraph.getLayer(frozenBranchOutput).params())
		End Sub

		''' <summary>
		''' Frozen layer should have same results as a layer with Sgd updater with learning rate set to 0
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Frozen Layer Vs Sgd") void testFrozenLayerVsSgd()
		Friend Overridable Sub testFrozenLayerVsSgd()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(100, 4), Nd4j.rand(100, 1))
			Dim confSgd As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).updater(New Sgd(2)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New DenseLayer.Builder()).updater(New Sgd(0.0)).biasUpdater(New Sgd(0.0)).nIn(3).nOut(4).build()).layer(2, (New DenseLayer.Builder()).updater(New Sgd(0.0)).biasUpdater(New Sgd(0.0)).nIn(4).nOut(2).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).updater(New Sgd(0.0)).biasUpdater(New Sgd(0.0)).activation(Activation.TANH).nIn(2).nOut(1).build()).build()
			Dim confFrozen As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).weightInit(WeightInit.XAVIER).updater(New Sgd(2)).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(3).nOut(4).build())).layer(2, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(4).nOut(2).build())).layer(3, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.TANH).nIn(2).nOut(1).build())).build()
			Dim frozenNetwork As New MultiLayerNetwork(confFrozen)
			frozenNetwork.init()
			Dim unfrozenLayerParams As INDArray = frozenNetwork.getLayer(0).params().dup()
			Dim frozenLayerParams1 As INDArray = frozenNetwork.getLayer(1).params().dup()
			Dim frozenLayerParams2 As INDArray = frozenNetwork.getLayer(2).params().dup()
			Dim frozenOutputLayerParams As INDArray = frozenNetwork.getLayer(3).params().dup()
			Dim sgdNetwork As New MultiLayerNetwork(confSgd)
			sgdNetwork.init()
			Dim unfrozenSgdLayerParams As INDArray = sgdNetwork.getLayer(0).params().dup()
			Dim frozenSgdLayerParams1 As INDArray = sgdNetwork.getLayer(1).params().dup()
			Dim frozenSgdLayerParams2 As INDArray = sgdNetwork.getLayer(2).params().dup()
			Dim frozenSgdOutputLayerParams As INDArray = sgdNetwork.getLayer(3).params().dup()
			For i As Integer = 0 To 99
				frozenNetwork.fit(randomData)
			Next i
			For i As Integer = 0 To 99
				sgdNetwork.fit(randomData)
			Next i
			assertEquals(frozenNetwork.getLayer(0).params(), sgdNetwork.getLayer(0).params())
			assertEquals(frozenNetwork.getLayer(1).params(), sgdNetwork.getLayer(1).params())
			assertEquals(frozenNetwork.getLayer(2).params(), sgdNetwork.getLayer(2).params())
			assertEquals(frozenNetwork.getLayer(3).params(), sgdNetwork.getLayer(3).params())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Computation Graph Vs Sgd") void testComputationGraphVsSgd()
		Friend Overridable Sub testComputationGraphVsSgd()
			Nd4j.Random.setSeed(12345)
			Dim randomData As New DataSet(Nd4j.rand(100, 4), Nd4j.rand(100, 1))
			Dim frozenBranchName As String = "B1-"
			Dim unfrozenBranchName As String = "B2-"
			Dim initialLayer As String = "initial"
			Dim frozenBranchUnfrozenLayer0 As String = frozenBranchName & "0"
			Dim frozenBranchFrozenLayer1 As String = frozenBranchName & "1"
			Dim frozenBranchFrozenLayer2 As String = frozenBranchName & "2"
			Dim frozenBranchOutput As String = frozenBranchName & "Output"
			Dim unfrozenLayer0 As String = unfrozenBranchName & "0"
			Dim unfrozenLayer1 As String = unfrozenBranchName & "1"
			Dim unfrozenBranch2 As String = unfrozenBranchName & "Output"
			Dim computationGraphConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(2.0)).seed(12345).graphBuilder().addInputs("input").addLayer(initialLayer, (New DenseLayer.Builder()).nIn(4).nOut(4).build(), "input").addLayer(frozenBranchUnfrozenLayer0, (New DenseLayer.Builder()).nIn(4).nOut(3).build(), initialLayer).addLayer(frozenBranchFrozenLayer1, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(3).nOut(4).build()), frozenBranchUnfrozenLayer0).addLayer(frozenBranchFrozenLayer2, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New DenseLayer.Builder()).nIn(4).nOut(2).build()), frozenBranchFrozenLayer1).addLayer(unfrozenLayer0, (New DenseLayer.Builder()).nIn(4).nOut(4).build(), initialLayer).addLayer(unfrozenLayer1, (New DenseLayer.Builder()).nIn(4).nOut(2).build(), unfrozenLayer0).addLayer(unfrozenBranch2, (New DenseLayer.Builder()).nIn(2).nOut(1).build(), unfrozenLayer1).addVertex("merge", New MergeVertex(), frozenBranchFrozenLayer2, unfrozenBranch2).addLayer(frozenBranchOutput, New org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).activation(Activation.TANH).nIn(3).nOut(1).build()), "merge").setOutputs(frozenBranchOutput).build()
			Dim computationSgdGraphConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(2.0)).seed(12345).graphBuilder().addInputs("input").addLayer(initialLayer, (New DenseLayer.Builder()).nIn(4).nOut(4).build(), "input").addLayer(frozenBranchUnfrozenLayer0, (New DenseLayer.Builder()).nIn(4).nOut(3).build(), initialLayer).addLayer(frozenBranchFrozenLayer1, (New DenseLayer.Builder()).updater(New Sgd(0.0)).biasUpdater(New Sgd(0.0)).nIn(3).nOut(4).build(), frozenBranchUnfrozenLayer0).addLayer(frozenBranchFrozenLayer2, (New DenseLayer.Builder()).updater(New Sgd(0.0)).biasUpdater(New Sgd(0.0)).nIn(4).nOut(2).build(), frozenBranchFrozenLayer1).addLayer(unfrozenLayer0, (New DenseLayer.Builder()).nIn(4).nOut(4).build(), initialLayer).addLayer(unfrozenLayer1, (New DenseLayer.Builder()).nIn(4).nOut(2).build(), unfrozenLayer0).addLayer(unfrozenBranch2, (New DenseLayer.Builder()).nIn(2).nOut(1).build(), unfrozenLayer1).addVertex("merge", New MergeVertex(), frozenBranchFrozenLayer2, unfrozenBranch2).addLayer(frozenBranchOutput, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).updater(New Sgd(0.0)).biasUpdater(New Sgd(0.0)).activation(Activation.TANH).nIn(3).nOut(1).build(), "merge").setOutputs(frozenBranchOutput).build()
			Dim frozenComputationGraph As New ComputationGraph(computationGraphConf)
			frozenComputationGraph.init()
			Dim unfrozenLayerParams As INDArray = frozenComputationGraph.getLayer(frozenBranchUnfrozenLayer0).params().dup()
			Dim frozenLayerParams1 As INDArray = frozenComputationGraph.getLayer(frozenBranchFrozenLayer1).params().dup()
			Dim frozenLayerParams2 As INDArray = frozenComputationGraph.getLayer(frozenBranchFrozenLayer2).params().dup()
			Dim frozenOutputLayerParams As INDArray = frozenComputationGraph.getLayer(frozenBranchOutput).params().dup()
			Dim sgdComputationGraph As New ComputationGraph(computationSgdGraphConf)
			sgdComputationGraph.init()
			Dim unfrozenSgdLayerParams As INDArray = sgdComputationGraph.getLayer(frozenBranchUnfrozenLayer0).params().dup()
			Dim frozenSgdLayerParams1 As INDArray = sgdComputationGraph.getLayer(frozenBranchFrozenLayer1).params().dup()
			Dim frozenSgdLayerParams2 As INDArray = sgdComputationGraph.getLayer(frozenBranchFrozenLayer2).params().dup()
			Dim frozenSgdOutputLayerParams As INDArray = sgdComputationGraph.getLayer(frozenBranchOutput).params().dup()
			For i As Integer = 0 To 99
				frozenComputationGraph.fit(randomData)
			Next i
			For i As Integer = 0 To 99
				sgdComputationGraph.fit(randomData)
			Next i
			assertEquals(frozenComputationGraph.getLayer(frozenBranchUnfrozenLayer0).params(), sgdComputationGraph.getLayer(frozenBranchUnfrozenLayer0).params())
			assertEquals(frozenComputationGraph.getLayer(frozenBranchFrozenLayer1).params(), sgdComputationGraph.getLayer(frozenBranchFrozenLayer1).params())
			assertEquals(frozenComputationGraph.getLayer(frozenBranchFrozenLayer2).params(), sgdComputationGraph.getLayer(frozenBranchFrozenLayer2).params())
			assertEquals(frozenComputationGraph.getLayer(frozenBranchOutput).params(), sgdComputationGraph.getLayer(frozenBranchOutput).params())
		End Sub
	End Class

End Namespace