Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution
Imports GaussianDistribution = org.deeplearning4j.nn.conf.distribution.GaussianDistribution
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports DropConnect = org.deeplearning4j.nn.conf.weightnoise.DropConnect
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitDistribution = org.deeplearning4j.nn.weights.WeightInitDistribution
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports MapSchedule = org.nd4j.linalg.schedule.MapSchedule
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNull
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
import static org.junit.jupiter.api.Assertions.assertThrows

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
Namespace org.deeplearning4j.nn.conf.layers

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Layer Config Validation Test") @NativeTag @Tag(TagNames.DL4J_OLD_API) class LayerConfigValidationTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class LayerConfigValidationTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Drop Connect") void testDropConnect()
		Friend Overridable Sub testDropConnect()
			' Warning thrown only since some layers may not have l1 or l2
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).weightNoise(New DropConnect(0.5)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test L 1 L 2 Not Set") void testL1L2NotSet()
		Friend Overridable Sub testL1L2NotSet()
			' Warning thrown only since some layers may not have l1 or l2
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.3)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @DisplayName("Test Reg Not Set L 1 Global") void testRegNotSetL1Global()
		Friend Overridable Sub testRegNotSetL1Global()
			assertThrows(GetType(System.InvalidOperationException), Sub()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.3)).l1(0.5).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			End Sub)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Weight Init Dist Not Set") void testWeightInitDistNotSet()
		Friend Overridable Sub testWeightInitDistNotSet()
			' Warning thrown only since global dist can be set with a different weight init locally
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.3)).dist(New GaussianDistribution(1e-3, 2)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Nesterovs Not Set Global") void testNesterovsNotSetGlobal()
		Friend Overridable Sub testNesterovsNotSetGlobal()
			' Warnings only thrown
			Dim testMomentumAfter As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
			testMomentumAfter(0) = 0.1
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Nesterovs(1.0, New MapSchedule(ScheduleType.ITERATION, testMomentumAfter))).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Comp Graph Null Layer") void testCompGraphNullLayer()
		Friend Overridable Sub testCompGraphNullLayer()
			Dim gb As ComputationGraphConfiguration.GraphBuilder = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.01)).seed(42).miniBatch(False).l1(0.2).l2(0.2).updater(Updater.RMSPROP).graphBuilder().addInputs("in").addLayer("L" & 1, (New GravesLSTM.Builder()).nIn(20).updater(Updater.RMSPROP).nOut(10).weightInit(WeightInit.XAVIER).dropOut(0.4).l1(0.3).activation(Activation.SIGMOID).build(), "in").addLayer("output", (New RnnOutputLayer.Builder()).nIn(20).nOut(10).activation(Activation.SOFTMAX).weightInit(WeightInit.RELU_UNIFORM).build(), "L" & 1).setOutputs("output")
			Dim conf As ComputationGraphConfiguration = gb.build()
			Dim cg As New ComputationGraph(conf)
			cg.init()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Predefined Config Values") void testPredefinedConfigValues()
		Friend Overridable Sub testPredefinedConfigValues()
			Dim expectedMomentum As Double = 0.9
			Dim expectedAdamMeanDecay As Double = 0.9
			Dim expectedAdamVarDecay As Double = 0.999
			Dim expectedRmsDecay As Double = 0.95
			Dim expectedDist As Distribution = New NormalDistribution(0, 1)
			Dim expectedL1 As Double = 0.0
			Dim expectedL2 As Double = 0.0
			' Nesterovs Updater
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Nesterovs(0.9)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).l2(0.5).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).updater(New Nesterovs(0.3, 0.4)).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim layerConf As BaseLayer = CType(net.getLayer(0).conf().getLayer(), BaseLayer)
			assertEquals(expectedMomentum, CType(layerConf.getIUpdater(), Nesterovs).getMomentum(), 1e-3)
			assertNull(TestUtils.getL1Reg(layerConf.getRegularization()))
			assertEquals(0.5, TestUtils.getL2(layerConf), 1e-3)
			Dim layerConf1 As BaseLayer = CType(net.getLayer(1).conf().getLayer(), BaseLayer)
			assertEquals(0.4, CType(layerConf1.getIUpdater(), Nesterovs).getMomentum(), 1e-3)
			' Adam Updater
			conf = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.3)).weightInit(New WeightInitDistribution(expectedDist)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).l2(0.5).l1(0.3).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			layerConf = CType(net.getLayer(0).conf().getLayer(), BaseLayer)
			assertEquals(0.3, TestUtils.getL1(layerConf), 1e-3)
			assertEquals(0.5, TestUtils.getL2(layerConf), 1e-3)
			layerConf1 = CType(net.getLayer(1).conf().getLayer(), BaseLayer)
			assertEquals(expectedAdamMeanDecay, CType(layerConf1.getIUpdater(), Adam).getBeta1(), 1e-3)
			assertEquals(expectedAdamVarDecay, CType(layerConf1.getIUpdater(), Adam).getBeta2(), 1e-3)
			assertEquals(New WeightInitDistribution(expectedDist), layerConf1.getWeightInitFn())
			assertNull(TestUtils.getL1Reg(layerConf1.getRegularization()))
			assertNull(TestUtils.getL2Reg(layerConf1.getRegularization()))
			' RMSProp Updater
			conf = (New NeuralNetConfiguration.Builder()).updater(New RmsProp(0.3)).list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(2).build()).layer(1, (New DenseLayer.Builder()).nIn(2).nOut(2).updater(New RmsProp(0.3, 0.4, RmsProp.DEFAULT_RMSPROP_EPSILON)).build()).build()
			net = New MultiLayerNetwork(conf)
			net.init()
			layerConf = CType(net.getLayer(0).conf().getLayer(), BaseLayer)
			assertEquals(expectedRmsDecay, CType(layerConf.getIUpdater(), RmsProp).getRmsDecay(), 1e-3)
			assertNull(TestUtils.getL1Reg(layerConf.getRegularization()))
			assertNull(TestUtils.getL2Reg(layerConf.getRegularization()))
			layerConf1 = CType(net.getLayer(1).conf().getLayer(), BaseLayer)
			assertEquals(0.4, CType(layerConf1.getIUpdater(), RmsProp).getRmsDecay(), 1e-3)
		End Sub
	End Class

End Namespace