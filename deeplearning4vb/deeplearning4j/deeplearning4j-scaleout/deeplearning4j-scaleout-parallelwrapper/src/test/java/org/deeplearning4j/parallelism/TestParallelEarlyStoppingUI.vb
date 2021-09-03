Imports System
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.saver
Imports DataSetLossCalculator = org.deeplearning4j.earlystopping.scorecalc.DataSetLossCalculator
Imports MaxEpochsTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxEpochsTerminationCondition
Imports org.deeplearning4j.earlystopping.trainer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports UIServer = org.deeplearning4j.ui.api.UIServer
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TestParallelEarlyStoppingUI extends org.deeplearning4j.BaseDL4JTest
	Public Class TestParallelEarlyStoppingUI
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testParallelStatsListenerCompatibility() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParallelStatsListenerCompatibility()
			Dim uiServer As UIServer = UIServer.getInstance()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd()).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer(1, (New OutputLayer.Builder()).nIn(3).nOut(3).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)

			' it's important that the UI can report results from parallel training
			' there's potential for StatsListener to fail if certain properties aren't set in the model
			Dim statsStorage As StatsStorage = New InMemoryStatsStorage()
			net.setListeners(New StatsListener(statsStorage))
			uiServer.attach(statsStorage)

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(50, 500)
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(500)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).evaluateEveryNEpochs(2).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingParallelTrainer(Of MultiLayerNetwork)(esConf, net, irisIter, Nothing, 3, 6, 2)

			Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()
			Console.WriteLine(result)

			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
		End Sub
	End Class

End Namespace