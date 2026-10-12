Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.saver
Imports DataSetLossCalculator = org.deeplearning4j.earlystopping.scorecalc.DataSetLossCalculator
Imports MaxEpochsTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxEpochsTerminationCondition
Imports MaxScoreIterationTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxScoreIterationTerminationCondition
Imports MaxTimeIterationTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxTimeIterationTerminationCondition
Imports org.deeplearning4j.earlystopping.trainer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.parallelism


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TestParallelEarlyStopping extends org.deeplearning4j.BaseDL4JTest
	Public Class TestParallelEarlyStopping
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingEveryNEpoch()
		Public Overridable Sub testEarlyStoppingEveryNEpoch()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd()).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(50, 600)
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).evaluateEveryNEpochs(2).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingParallelTrainer(Of MultiLayerNetwork)(esConf, net, irisIter, Nothing, 2, 6, 1)

			Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()
			Console.WriteLine(result)

			assertEquals(5, result.getTotalEpochs())
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBadTuning()
		Public Overridable Sub testBadTuning()
			'Test poor tuning (high LR): should terminate on MaxScoreIterationTerminationCondition

			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(1.0)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(10, 150)
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5000)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES), New MaxScoreIterationTerminationCondition(10)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingParallelTrainer(Of MultiLayerNetwork)(esConf, net, irisIter, Nothing, 2, 2, 1)
			Dim result As EarlyStoppingResult = trainer.fit()

			assertTrue(result.getTotalEpochs() < 5)
			assertEquals(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New MaxScoreIterationTerminationCondition(10)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())

			assertTrue(result.getBestModelEpoch() <= 0)
			assertNotNull(result.getBestModel())
		End Sub
	End Class

End Namespace