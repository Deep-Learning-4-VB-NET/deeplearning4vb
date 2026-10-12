Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports MergeVertex = org.deeplearning4j.nn.conf.graph.MergeVertex
Imports SubsetVertex = org.deeplearning4j.nn.conf.graph.SubsetVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
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
Namespace org.deeplearning4j.nn.transferlearning

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @DisplayName("Transfer Learning Helper Test") class TransferLearningHelperTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class TransferLearningHelperTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Tes Unfrozen Subset") void tesUnfrozenSubset()
		Friend Overridable Sub tesUnfrozenSubset()
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).seed(124).activation(Activation.IDENTITY).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1))
	'        
	'                             (inCentre)                        (inRight)
	'                                |                                |
	'                            denseCentre0                         |
	'                                |                                |
	'                 ,--------  denseCentre1                       denseRight0
	'                /               |                                |
	'        subsetLeft(0-3)    denseCentre2 ---- denseRight ----  mergeRight
	'              |                 |                                |
	'         denseLeft0        denseCentre3                        denseRight1
	'              |                 |                                |
	'          (outLeft)         (outCentre)                        (outRight)
	'        
	'         
			Dim conf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("inCentre", "inRight").addLayer("denseCentre0", (New DenseLayer.Builder()).nIn(10).nOut(9).build(), "inCentre").addLayer("denseCentre1", (New DenseLayer.Builder()).nIn(9).nOut(8).build(), "denseCentre0").addLayer("denseCentre2", (New DenseLayer.Builder()).nIn(8).nOut(7).build(), "denseCentre1").addLayer("denseCentre3", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("outCentre", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(7).nOut(4).build(), "denseCentre3").addVertex("subsetLeft", New SubsetVertex(0, 3), "denseCentre1").addLayer("denseLeft0", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "subsetLeft").addLayer("outLeft", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(6).build(), "denseLeft0").addLayer("denseRight", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseRight", "denseRight0").addLayer("denseRight1", (New DenseLayer.Builder()).nIn(10).nOut(5).build(), "mergeRight").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(5).build(), "denseRight1").setOutputs("outLeft", "outCentre", "outRight").build()
			Dim modelToTune As New ComputationGraph(conf)
			modelToTune.init()
			Dim helper As New TransferLearningHelper(modelToTune, "denseCentre2")
			Dim modelSubset As ComputationGraph = helper.unfrozenGraph()
			Dim expectedConf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("denseCentre1", "denseCentre2", "inRight").addLayer("denseCentre3", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("outCentre", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(7).nOut(4).build(), "denseCentre3").addVertex("subsetLeft", New SubsetVertex(0, 3), "denseCentre1").addLayer("denseLeft0", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "subsetLeft").addLayer("outLeft", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(6).build(), "denseLeft0").addLayer("denseRight", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseRight", "denseRight0").addLayer("denseRight1", (New DenseLayer.Builder()).nIn(10).nOut(5).build(), "mergeRight").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(5).build(), "denseRight1").setOutputs("outLeft", "outCentre", "outRight").build()
			Dim expectedModel As New ComputationGraph(expectedConf)
			expectedModel.init()
			assertEquals(expectedConf.toJson(), modelSubset.Configuration.toJson())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Fit Un Frozen") void testFitUnFrozen()
		Friend Overridable Sub testFitUnFrozen()
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.9)).seed(124).activation(Activation.IDENTITY).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT)
			Dim conf As ComputationGraphConfiguration = overallConf.graphBuilder().addInputs("inCentre", "inRight").addLayer("denseCentre0", (New DenseLayer.Builder()).nIn(10).nOut(9).build(), "inCentre").addLayer("denseCentre1", (New DenseLayer.Builder()).nIn(9).nOut(8).build(), "denseCentre0").addLayer("denseCentre2", (New DenseLayer.Builder()).nIn(8).nOut(7).build(), "denseCentre1").addLayer("denseCentre3", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("outCentre", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(7).nOut(4).build(), "denseCentre3").addVertex("subsetLeft", New SubsetVertex(0, 3), "denseCentre1").addLayer("denseLeft0", (New DenseLayer.Builder()).nIn(4).nOut(5).build(), "subsetLeft").addLayer("outLeft", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(6).build(), "denseLeft0").addLayer("denseRight", (New DenseLayer.Builder()).nIn(7).nOut(7).build(), "denseCentre2").addLayer("denseRight0", (New DenseLayer.Builder()).nIn(2).nOut(3).build(), "inRight").addVertex("mergeRight", New MergeVertex(), "denseRight", "denseRight0").addLayer("denseRight1", (New DenseLayer.Builder()).nIn(10).nOut(5).build(), "mergeRight").addLayer("outRight", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(5).nOut(5).build(), "denseRight1").setOutputs("outLeft", "outCentre", "outRight").build()
			Dim modelToTune As New ComputationGraph(conf)
			modelToTune.init()
			Dim inRight As INDArray = Nd4j.rand(10, 2)
			Dim inCentre As INDArray = Nd4j.rand(10, 10)
			Dim outLeft As INDArray = Nd4j.rand(10, 6)
			Dim outRight As INDArray = Nd4j.rand(10, 5)
			Dim outCentre As INDArray = Nd4j.rand(10, 4)
			Dim origData As New MultiDataSet(New INDArray() { inCentre, inRight }, New INDArray() { outLeft, outCentre, outRight })
			Dim modelIdentical As ComputationGraph = modelToTune.clone()
			modelIdentical.getVertex("denseCentre0").setLayerAsFrozen()
			modelIdentical.getVertex("denseCentre1").setLayerAsFrozen()
			modelIdentical.getVertex("denseCentre2").setLayerAsFrozen()
			Dim helper As New TransferLearningHelper(modelToTune, "denseCentre2")
			Dim featurizedDataSet As MultiDataSet = helper.featurize(origData)
			assertEquals(modelIdentical.getLayer("denseRight0").params(), modelToTune.getLayer("denseRight0").params())
			modelIdentical.fit(origData)
			helper.fitFeaturized(featurizedDataSet)
			assertEquals(modelIdentical.getLayer("denseCentre0").params(), modelToTune.getLayer("denseCentre0").params())
			assertEquals(modelIdentical.getLayer("denseCentre1").params(), modelToTune.getLayer("denseCentre1").params())
			assertEquals(modelIdentical.getLayer("denseCentre2").params(), modelToTune.getLayer("denseCentre2").params())
			assertEquals(modelIdentical.getLayer("denseCentre3").params(), modelToTune.getLayer("denseCentre3").params())
			assertEquals(modelIdentical.getLayer("outCentre").params(), modelToTune.getLayer("outCentre").params())
			assertEquals(modelIdentical.getLayer("denseRight").conf().toJson(), modelToTune.getLayer("denseRight").conf().toJson())
			assertEquals(modelIdentical.getLayer("denseRight").params(), modelToTune.getLayer("denseRight").params())
			assertEquals(modelIdentical.getLayer("denseRight0").conf().toJson(), modelToTune.getLayer("denseRight0").conf().toJson())
			' assertEquals(modelIdentical.getLayer("denseRight0").params(),modelToTune.getLayer("denseRight0").params());
			assertEquals(modelIdentical.getLayer("denseRight1").params(), modelToTune.getLayer("denseRight1").params())
			assertEquals(modelIdentical.getLayer("outRight").params(), modelToTune.getLayer("outRight").params())
			assertEquals(modelIdentical.getLayer("denseLeft0").params(), modelToTune.getLayer("denseLeft0").params())
			assertEquals(modelIdentical.getLayer("outLeft").params(), modelToTune.getLayer("outLeft").params())
			' log.info(modelIdentical.summary());
			' log.info(helper.unfrozenGraph().summary());
			modelIdentical.summary()
			helper.unfrozenGraph().summary()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test MLN") void testMLN()
		Friend Overridable Sub testMLN()
			Dim randomData As New DataSet(Nd4j.rand(10, 4), Nd4j.rand(10, 3))
			Dim overallConf As NeuralNetConfiguration.Builder = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).activation(Activation.IDENTITY)
			Dim modelToFineTune As New MultiLayerNetwork(overallConf.clone().list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(2).build()).layer(2, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(3, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build())
			modelToFineTune.init()
			Dim modelNow As MultiLayerNetwork = (New TransferLearning.Builder(modelToFineTune)).setFeatureExtractor(1).build()
			Dim ff As IList(Of INDArray) = modelToFineTune.feedForwardToLayer(2, randomData.Features, False)
			Dim asFrozenFeatures As INDArray = ff(2)
			Dim helper As New TransferLearningHelper(modelToFineTune, 1)
			Dim paramsLastTwoLayers As INDArray = Nd4j.hstack(modelToFineTune.getLayer(2).params(), modelToFineTune.getLayer(3).params())
			Dim notFrozen As New MultiLayerNetwork(overallConf.clone().list().layer(0, (New DenseLayer.Builder()).nIn(2).nOut(3).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(3).nOut(3).build()).build(), paramsLastTwoLayers)
			assertEquals(asFrozenFeatures, helper.featurize(randomData).Features)
			assertEquals(randomData.Labels, helper.featurize(randomData).Labels)
			For i As Integer = 0 To 4
				notFrozen.fit(New DataSet(asFrozenFeatures, randomData.Labels))
				helper.fitFeaturized(helper.featurize(randomData))
				modelNow.fit(randomData)
			Next i
			Dim expected As INDArray = Nd4j.hstack(modelToFineTune.getLayer(0).params(), modelToFineTune.getLayer(1).params(), notFrozen.params())
			Dim act As INDArray = modelNow.params()
			assertEquals(expected, act)
		End Sub
	End Class

End Namespace