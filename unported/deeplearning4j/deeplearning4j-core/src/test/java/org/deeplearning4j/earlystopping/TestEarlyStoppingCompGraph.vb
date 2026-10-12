Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports org.deeplearning4j.earlystopping.listener
Imports org.deeplearning4j.earlystopping.saver
Imports org.deeplearning4j.earlystopping.scorecalc
Imports MaxEpochsTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxEpochsTerminationCondition
Imports MaxScoreIterationTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxScoreIterationTerminationCondition
Imports MaxTimeIterationTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxTimeIterationTerminationCondition
Imports ScoreImprovementEpochTerminationCondition = org.deeplearning4j.earlystopping.termination.ScoreImprovementEpochTerminationCondition
Imports EarlyStoppingGraphTrainer = org.deeplearning4j.earlystopping.trainer.EarlyStoppingGraphTrainer
Imports org.deeplearning4j.earlystopping.trainer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports AutoEncoder = org.deeplearning4j.nn.conf.layers.AutoEncoder
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports BernoulliReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.BernoulliReconstructionDistribution
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports Metric = org.nd4j.evaluation.regression.RegressionEvaluation.Metric
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.deeplearning4j.earlystopping


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class TestEarlyStoppingCompGraph extends org.deeplearning4j.BaseDL4JTest
	Public Class TestEarlyStoppingCompGraph
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.DOUBLE
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingIris()
		Public Overridable Sub testEarlyStoppingIris()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.001)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculatorCG(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of ComputationGraph) = New EarlyStoppingGraphTrainer(esConf, net, irisIter)

			Dim result As EarlyStoppingResult(Of ComputationGraph) = trainer.fit()
			Console.WriteLine(result)

			assertEquals(5, result.getTotalEpochs())
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
			Dim scoreVsIter As IDictionary(Of Integer, Double) = result.getScoreVsEpoch()
			assertEquals(5, scoreVsIter.Count)
			Dim expDetails As String = esConf.getEpochTerminationConditions().get(0).ToString()
			assertEquals(expDetails, result.getTerminationDetails())

			Dim [out] As ComputationGraph = result.BestModel
			assertNotNull([out])

			'Check that best score actually matches (returned model vs. manually calculated score)
			Dim bestNetwork As ComputationGraph = result.BestModel
			irisIter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim score As Double = bestNetwork.score(irisIter.next())
			assertEquals(result.getBestModelScore(), score, 1e-2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBadTuning()
		Public Overridable Sub testBadTuning()
			'Test poor tuning (high LR): should terminate on MaxScoreIterationTerminationCondition

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(5.0)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5000)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES), New MaxScoreIterationTerminationCondition(10)).scoreCalculator(New DataSetLossCalculatorCG(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingGraphTrainer(esConf, net, irisIter)
			Dim result As EarlyStoppingResult = trainer.fit()

			assertTrue(result.getTotalEpochs() < 5)
			assertEquals(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New MaxScoreIterationTerminationCondition(10)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())

			assertEquals(0, result.getBestModelEpoch())
			assertNotNull(result.getBestModel())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeTermination()
		Public Overridable Sub testTimeTermination()
			'test termination after max time

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(1e-6)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)

			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(10000)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(5, TimeUnit.SECONDS), New MaxScoreIterationTerminationCondition(50)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingGraphTrainer(esConf, net, irisIter)
			Dim startTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim result As EarlyStoppingResult = trainer.fit()
			Dim endTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim durationSeconds As Integer = CInt(endTime - startTime) \ 1000

			assertTrue(durationSeconds >= 3)
			assertTrue(durationSeconds <= 20)

			assertEquals(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New MaxTimeIterationTerminationCondition(5, TimeUnit.SECONDS)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNoImprovementNEpochsTermination()
		Public Overridable Sub testNoImprovementNEpochsTermination()
			'Idea: terminate training if score (test set loss) does not improve for 5 consecutive epochs
			'Simulate this by setting LR = 0.0

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.0)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)

			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(100), New ScoreImprovementEpochTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES), New MaxScoreIterationTerminationCondition(50)).scoreCalculator(New DataSetLossCalculatorCG(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingGraphTrainer(esConf, net, irisIter)
			Dim result As EarlyStoppingResult = trainer.fit()

			'Expect no score change due to 0 LR -> terminate after 6 total epochs
			assertEquals(6, result.getTotalEpochs())
			assertEquals(0, result.getBestModelEpoch())
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New ScoreImprovementEpochTerminationCondition(5)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListeners()
		Public Overridable Sub testListeners()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.001)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculatorCG(irisIter, True)).modelSaver(saver).build()

			Dim listener As New LoggingEarlyStoppingListener()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingGraphTrainer(esConf, net, irisIter, listener)

			trainer.fit()

			assertEquals(1, listener.onStartCallCount)
			assertEquals(5, listener.onEpochCallCount)
			assertEquals(1, listener.onCompletionCallCount)
		End Sub

		Private Class LoggingEarlyStoppingListener
			Implements EarlyStoppingListener(Of ComputationGraph)

			Friend Shared log As Logger = LoggerFactory.getLogger(GetType(LoggingEarlyStoppingListener))
			Friend onStartCallCount As Integer = 0
			Friend onEpochCallCount As Integer = 0
			Friend onCompletionCallCount As Integer = 0

			Public Overridable Sub onStart(ByVal esConfig As EarlyStoppingConfiguration, ByVal net As ComputationGraph)
				log.info("EarlyStopping: onStart called")
				onStartCallCount += 1
			End Sub

			Public Overridable Sub onEpoch(ByVal epochNum As Integer, ByVal score As Double, ByVal esConfig As EarlyStoppingConfiguration, ByVal net As ComputationGraph)
				log.info("EarlyStopping: onEpoch called (epochNum={}, score={}}", epochNum, score)
				onEpochCallCount += 1
			End Sub

			Public Overridable Sub onCompletion(ByVal esResult As EarlyStoppingResult)
				log.info("EarlyStopping: onCompletion called (result: {})", esResult)
				onCompletionCallCount += 1
			End Sub
		End Class




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRegressionScoreFunctionSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRegressionScoreFunctionSimple()

			For Each metric As Metric In New Metric(){Metric.MSE, Metric.MAE}
				log.info("Metric: " & metric)

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New DenseLayer.Builder()).nIn(784).nOut(32).build(), "in").layer("1", (New OutputLayer.Builder()).nIn(32).nOut(784).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.MSE).build(), "0").setOutputs("1").build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
				Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New RegressionScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingGraphTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of ComputationGraph) = trainer.fit()

				assertNotNull(result.BestModel)
				assertTrue(result.getBestModelScore() > 0.0)
			Next metric
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAEScoreFunctionSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAEScoreFunctionSimple()
			Dim dt As DataType = Nd4j.defaultFloatingPointType()

			For Each metric As Metric In New Metric(){Metric.MSE, Metric.MAE}
				log.info("Metric: " & metric)

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New AutoEncoder.Builder()).nIn(784).nOut(32).build(), "in").setOutputs("0").build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
				Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New AutoencoderScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingGraphTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of ComputationGraph) = trainer.pretrain()

				assertNotNull(result.BestModel)
				assertTrue(result.getBestModelScore() > 0.0)
			Next metric
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVAEScoreFunctionSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVAEScoreFunctionSimple()

			For Each metric As Metric In New Metric(){Metric.MSE, Metric.MAE}
				log.info("Metric: " & metric)

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New VariationalAutoencoder.Builder()).nIn(784).nOut(32).encoderLayerSizes(64).decoderLayerSizes(64).build(), "in").setOutputs("0").build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
				Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New VAEReconErrorScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingGraphTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of ComputationGraph) = trainer.pretrain()

				assertNotNull(result.BestModel)
				assertTrue(result.getBestModelScore() > 0.0)
			Next metric
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVAEScoreFunctionReconstructionProbSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVAEScoreFunctionReconstructionProbSimple()

			For Each logProb As Boolean In New Boolean(){False, True}

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-5)).graphBuilder().addInputs("in").layer("0", (New VariationalAutoencoder.Builder()).nIn(784).nOut(32).encoderLayerSizes(64).decoderLayerSizes(64).reconstructionDistribution(New BernoulliReconstructionDistribution(Activation.SIGMOID)).build(), "in").setOutputs("0").build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
				Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New VAEReconProbScoreCalculator(iter, 20, logProb)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingGraphTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of ComputationGraph) = trainer.pretrain()

				assertNotNull(result.BestModel)
				assertTrue(result.getBestModelScore() > 0.0)
			Next logProb
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClassificationScoreFunctionSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testClassificationScoreFunctionSimple()

			For Each metric As Evaluation.Metric In Evaluation.Metric.values()
				log.info("Metric: " & metric)

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").layer("0", (New DenseLayer.Builder()).nIn(784).nOut(32).build(), "in").layer("1", (New OutputLayer.Builder()).nIn(32).nOut(10).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

				Dim net As New ComputationGraph(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(ds)
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
				Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New ClassificationScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingGraphTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of ComputationGraph) = trainer.fit()

				assertNotNull(result.BestModel)
			Next metric
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingListenersCG()
		Public Overridable Sub testEarlyStoppingListenersCG()
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.001)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").layer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)

			Dim tl As New TestEarlyStopping.TestListener()
			net.setListeners(tl)

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(50, 150)
			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of ComputationGraph) = New EarlyStoppingGraphTrainer(esConf, net, irisIter)

			trainer.fit()

			assertEquals(5, tl.getCountEpochStart())
			assertEquals(5, tl.getCountEpochEnd())
			assertEquals(5 * 150\50, tl.getIterCount())

			assertEquals(4, tl.getMaxEpochStart())
			assertEquals(4, tl.getMaxEpochEnd())
		End Sub
	End Class

End Namespace