Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports MultipleEpochsIterator = org.deeplearning4j.datasets.iterator.MultipleEpochsIterator
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports org.deeplearning4j.datasets.iterator.impl
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports SingletonDataSetIterator = org.deeplearning4j.datasets.iterator.impl.SingletonDataSetIterator
Imports org.deeplearning4j.earlystopping.listener
Imports org.deeplearning4j.earlystopping.saver
Imports LocalFileModelSaver = org.deeplearning4j.earlystopping.saver.LocalFileModelSaver
Imports org.deeplearning4j.earlystopping.scorecalc
Imports org.deeplearning4j.earlystopping.termination
Imports EarlyStoppingTrainer = org.deeplearning4j.earlystopping.trainer.EarlyStoppingTrainer
Imports org.deeplearning4j.earlystopping.trainer
Imports Model = org.deeplearning4j.nn.api.Model
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.layers
Imports BernoulliReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.BernoulliReconstructionDistribution
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports BaseOptimizer = org.deeplearning4j.optimize.solvers.BaseOptimizer
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROCBinary = org.nd4j.evaluation.classification.ROCBinary
Imports Metric = org.nd4j.evaluation.regression.RegressionEvaluation.Metric
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Sin = org.nd4j.linalg.api.ops.impl.transforms.strict.Sin
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.FILE_IO) @Tag(TagNames.NDARRAY_ETL) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) @Tag(TagNames.LARGE_RESOURCES) @Tag(TagNames.LONG_TEST) public class TestEarlyStopping extends org.deeplearning4j.BaseDL4JTest
	Public Class TestEarlyStopping
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.DOUBLE
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingIris()
		Public Overridable Sub testEarlyStoppingIris()

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)

			For i As Integer = 0 To 5
				Nd4j.Random.setSeed(12345)

				Dim sc As ScoreCalculator
				Dim min As Boolean
				Select Case i
					Case 0
						sc = New DataSetLossCalculator(irisIter, True)
						min = True
					Case 1
						sc = New ClassificationScoreCalculator(Evaluation.Metric.ACCURACY, irisIter)
						min = False
					Case 2
						sc = New ClassificationScoreCalculator(Evaluation.Metric.F1, irisIter)
						min = False
					Case 3
						sc = New RegressionScoreCalculator(Metric.MSE, irisIter)
						min = True
					Case 4
						sc = New ROCScoreCalculator(ROCScoreCalculator.ROCType.MULTICLASS, ROCScoreCalculator.Metric.AUC, irisIter)
						min = False
					Case 5
						sc = New ROCScoreCalculator(ROCScoreCalculator.ROCType.MULTICLASS, ROCScoreCalculator.Metric.AUPRC, irisIter)
						min = False
					Case 6
						sc = New ROCScoreCalculator(ROCScoreCalculator.ROCType.BINARY, ROCScoreCalculator.Metric.AUC, irisIter)
						min = False
					Case Else
						Throw New Exception()
				End Select

				Dim msg As String = i & " - " & sc.GetType().Name
				log.info("Starting test - {}", msg)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New Sgd(0.5)).weightInit(WeightInit.XAVIER).list().layer((New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()
	'            net.setListeners(new ScoreIterationListener(1));

				Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
				Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(sc).modelSaver(saver).build()

				Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingTrainer(esConf, net, irisIter)

				Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()
				Console.WriteLine(result)

				assertEquals(5, result.getTotalEpochs())
				assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
				Dim scoreVsIter As IDictionary(Of Integer, Double) = result.getScoreVsEpoch()
				assertEquals(5, scoreVsIter.Count)
				Dim expDetails As String = esConf.getEpochTerminationConditions().get(0).ToString()
				assertEquals(expDetails, result.getTerminationDetails())

				Dim [out] As MultiLayerNetwork = result.BestModel
				assertNotNull([out])



				'Validate that it is in fact the best model:
				Dim bestEpoch As Integer = -1
				Dim bestScore As Double = (If(min, Double.MaxValue, -Double.MaxValue))
				For j As Integer = 0 To 4
					Dim s As Double = scoreVsIter(j)
					If (min AndAlso s < bestScore) OrElse (Not min AndAlso s > bestScore) Then
						bestScore = s
						bestEpoch = j
					End If
				Next j
				assertEquals(bestEpoch, [out].EpochCount,msg)
				assertEquals(bestScore, result.getBestModelScore(), 1e-5,msg)

				'Check that best score actually matches (returned model vs. manually calculated score)
				Dim bestNetwork As MultiLayerNetwork = result.BestModel
				irisIter.reset()
				Dim score As Double
				Select Case i
					Case 0
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						score = bestNetwork.score(irisIter.next())
					Case 1
						score = bestNetwork.evaluate(irisIter).accuracy()
					Case 2
						score = bestNetwork.evaluate(irisIter).f1()
					Case 3
						score = bestNetwork.evaluateRegression(irisIter).averageMeanSquaredError()
					Case 4
						score = bestNetwork.evaluateROCMultiClass(irisIter).calculateAverageAUC()
					Case 5
						score = bestNetwork.evaluateROCMultiClass(irisIter).calculateAverageAUCPR()
					Case 6
						score = bestNetwork.doEvaluation(irisIter, New ROCBinary())(0).calculateAverageAuc()
					Case Else
						Throw New Exception()
				End Select
				assertEquals(result.getBestModelScore(), score, 1e-2,msg)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingEveryNEpoch()
		Public Overridable Sub testEarlyStoppingEveryNEpoch()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.01)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).evaluateEveryNEpochs(2).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingTrainer(esConf, net, irisIter)

			Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()
			Console.WriteLine(result)

			assertEquals(5, result.getTotalEpochs())
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingIrisMultiEpoch()
		Public Overridable Sub testEarlyStoppingIrisMultiEpoch()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.001)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim mIter As New MultipleEpochsIterator(10, irisIter)

			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingTrainer(esConf, net, mIter)

			Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()
			Console.WriteLine(result)

			assertEquals(5, result.getTotalEpochs())
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
			Dim scoreVsIter As IDictionary(Of Integer, Double) = result.getScoreVsEpoch()
			assertEquals(5, scoreVsIter.Count)
			Dim expDetails As String = esConf.getEpochTerminationConditions().get(0).ToString()
			assertEquals(expDetails, result.getTerminationDetails())

			Dim [out] As MultiLayerNetwork = result.BestModel
			assertNotNull([out])

			'Check that best score actually matches (returned model vs. manually calculated score)
			Dim bestNetwork As MultiLayerNetwork = result.BestModel
			irisIter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim score As Double = bestNetwork.score(irisIter.next(), False)
			assertEquals(result.getBestModelScore(), score, 1e-2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBadTuning()
		Public Overridable Sub testBadTuning()
			'Test poor tuning (high LR): should terminate on MaxScoreIterationTerminationCondition

			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(5.0)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5000)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES), New MaxScoreIterationTerminationCondition(10)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingTrainer(esConf, net, irisIter)
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
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(1e-6)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)

			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(10000)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(3, TimeUnit.SECONDS), New MaxScoreIterationTerminationCondition(50)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingTrainer(esConf, net, irisIter)
			Dim startTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim result As EarlyStoppingResult = trainer.fit()
			Dim endTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim durationSeconds As Integer = CInt(endTime - startTime) \ 1000

			assertTrue(durationSeconds >= 3)
			assertTrue(durationSeconds <= 12)

			assertEquals(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New MaxTimeIterationTerminationCondition(3, TimeUnit.SECONDS)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNoImprovementNEpochsTermination()
		Public Overridable Sub testNoImprovementNEpochsTermination()
			'Idea: terminate training if score (test set loss) does not improve for 5 consecutive epochs
			'Simulate this by setting LR = 0.0

			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.0)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)

			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(100), New ScoreImprovementEpochTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES), New MaxScoreIterationTerminationCondition(50)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingTrainer(esConf, net, irisIter)
			Dim result As EarlyStoppingResult = trainer.fit()

			'Expect no score change due to 0 LR -> terminate after 6 total epochs
			assertEquals(6, result.getTotalEpochs())
			assertEquals(0, result.getBestModelEpoch())
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New ScoreImprovementEpochTerminationCondition(5)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMinImprovementNEpochsTermination()
		Public Overridable Sub testMinImprovementNEpochsTermination()
			'Idea: terminate training if score (test set loss) does not improve more than minImprovement for 5 consecutive epochs
			'Simulate this by setting LR = 0.0
			Dim rng As New Random(123)
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Nesterovs(0.0,0.9)).list().layer(0, (New DenseLayer.Builder()).nIn(1).nOut(20).weightInit(WeightInit.XAVIER).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).weightInit(WeightInit.XAVIER).activation(Activation.IDENTITY).weightInit(WeightInit.XAVIER).nIn(20).nOut(1).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))
			Dim nSamples As Integer = 100
			'Generate the training data
			Dim x As INDArray = Nd4j.linspace(-10, 10, nSamples).reshape(ChrW(nSamples), 1)
			Dim y As INDArray = Nd4j.Executioner.exec(New Sin(x.dup()))
			Dim allData As New DataSet(x, y)

			Dim list As IList(Of DataSet) = allData.asList()
			Collections.shuffle(list, rng)
			Dim training As DataSetIterator = New ListDataSetIterator(list, nSamples)

			Dim minImprovement As Double = 0.0009
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(1000), New ScoreImprovementEpochTerminationCondition(5, minImprovement)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(3, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculator(training, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingTrainer(esConf, net, training)
			Dim result As EarlyStoppingResult = trainer.fit()

			assertEquals(6, result.getTotalEpochs())
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New ScoreImprovementEpochTerminationCondition(5, minImprovement)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingGetBestModel()
		Public Overridable Sub testEarlyStoppingGetBestModel()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.001)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim mIter As New MultipleEpochsIterator(10, irisIter)

			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingTrainer(esConf, net, mIter)

			Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()
			Console.WriteLine(result)

			Dim mln As MultiLayerNetwork = result.BestModel

			assertEquals(net.getnLayers(), mln.getnLayers())
			assertEquals(net.conf().getOptimizationAlgo(), mln.conf().getOptimizationAlgo())
			Dim bl As BaseLayer = CType(net.conf().getLayer(), BaseLayer)
			assertEquals(bl.getActivationFn().ToString(), CType(mln.conf().getLayer(), BaseLayer).getActivationFn().ToString())
			assertEquals(bl.getIUpdater(), CType(mln.conf().getLayer(), BaseLayer).getIUpdater())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListeners()
		Public Overridable Sub testListeners()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.001)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.setListeners(New ScoreIterationListener(1))

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim listener As New LoggingEarlyStoppingListener()

			Dim trainer As IEarlyStoppingTrainer = New EarlyStoppingTrainer(esConf, net, irisIter, listener)

			trainer.fit()

			assertEquals(1, listener.onStartCallCount)
			assertEquals(5, listener.onEpochCallCount)
			assertEquals(1, listener.onCompletionCallCount)
		End Sub

		Private Class LoggingEarlyStoppingListener
			Implements EarlyStoppingListener(Of MultiLayerNetwork)

			Friend Shared log As Logger = LoggerFactory.getLogger(GetType(LoggingEarlyStoppingListener))
			Friend onStartCallCount As Integer = 0
			Friend onEpochCallCount As Integer = 0
			Friend onCompletionCallCount As Integer = 0

			Public Overridable Sub onStart(ByVal esConfig As EarlyStoppingConfiguration, ByVal net As MultiLayerNetwork)
				log.info("EarlyStopping: onStart called")
				onStartCallCount += 1
			End Sub

			Public Overridable Sub onEpoch(ByVal epochNum As Integer, ByVal score As Double, ByVal esConfig As EarlyStoppingConfiguration, ByVal net As MultiLayerNetwork)
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

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(784).nOut(32).build()).layer((New OutputLayer.Builder()).nIn(32).nOut(784).activation(Activation.SIGMOID).lossFunction(LossFunctions.LossFunction.MSE).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
				Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New RegressionScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()

				assertNotNull(result.BestModel)
				assertTrue(result.getBestModelScore() > 0.0)
			Next metric
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAEScoreFunctionSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAEScoreFunctionSimple()

			For Each metric As Metric In New Metric(){Metric.MSE, Metric.MAE}
				log.info("Metric: " & metric)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New AutoEncoder.Builder()).nIn(784).nOut(32).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
				Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New AutoencoderScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.pretrain()

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

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New VariationalAutoencoder.Builder()).nIn(784).nOut(32).encoderLayerSizes(64).decoderLayerSizes(64).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
				Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New VAEReconErrorScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.pretrain()

				assertNotNull(result.BestModel)
				assertTrue(result.getBestModelScore() > 0.0)
			Next metric
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testVAEScoreFunctionReconstructionProbSimple() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testVAEScoreFunctionReconstructionProbSimple()

			For Each logProb As Boolean In New Boolean(){False, True}

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New VariationalAutoencoder.Builder()).nIn(784).nOut(32).encoderLayerSizes(64).decoderLayerSizes(64).reconstructionDistribution(New BernoulliReconstructionDistribution(Activation.SIGMOID)).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(New DataSet(ds.Features, ds.Features))
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
				Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New VAEReconProbScoreCalculator(iter, 20, logProb)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.pretrain()

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

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(784).nOut(32).build()).layer((New OutputLayer.Builder()).nIn(32).nOut(10).activation(Activation.SOFTMAX).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim iter As DataSetIterator = New MnistDataSetIterator(32, False, 12345)

				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				For i As Integer = 0 To 9
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim ds As DataSet = iter.next()
					l.Add(ds)
				Next i

				iter = New ExistingDataSetIterator(l)

				Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
				Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New ClassificationScoreCalculator(metric, iter)).modelSaver(saver).build()

				Dim trainer As New EarlyStoppingTrainer(esConf, net, iter)
				Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()

				assertNotNull(result.BestModel)
			Next metric
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingListeners()
		Public Overridable Sub testEarlyStoppingListeners()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.001)).weightInit(WeightInit.XAVIER).list().layer(0, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)

			Dim tl As New TestListener()
			net.setListeners(tl)

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(50, 150)
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New InMemoryModelSaver(Of MultiLayerNetwork)()
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(1, TimeUnit.MINUTES)).scoreCalculator(New DataSetLossCalculator(irisIter, True)).modelSaver(saver).build()

			Dim trainer As IEarlyStoppingTrainer(Of MultiLayerNetwork) = New EarlyStoppingTrainer(esConf, net, irisIter)

			Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = trainer.fit()

			assertEquals(5, tl.countEpochStart)
			assertEquals(5, tl.countEpochEnd)
			assertEquals(5 * 150\50, tl.iterCount)

			assertEquals(4, tl.maxEpochStart)
			assertEquals(4, tl.maxEpochEnd)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class TestListener extends org.deeplearning4j.optimize.api.BaseTrainingListener
		Public Class TestListener
			Inherits BaseTrainingListener

			Friend countEpochStart As Integer = 0
			Friend countEpochEnd As Integer = 0
			Friend iterCount As Integer = 0
			Friend maxEpochStart As Integer = -1
			Friend maxEpochEnd As Integer = -1

			Public Overrides Sub onEpochStart(ByVal model As Model)
				countEpochStart += 1
				maxEpochStart = Math.Max(maxEpochStart, BaseOptimizer.getEpochCount(model))
			End Sub

			Public Overrides Sub onEpochEnd(ByVal model As Model)
				countEpochEnd += 1
				maxEpochEnd = Math.Max(maxEpochEnd, BaseOptimizer.getEpochCount(model))
			End Sub

			Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
				iterCount += 1
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingMaximizeScore(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEarlyStoppingMaximizeScore(ByVal testDir As Path)
			Nd4j.Random.setSeed(12345)

			Dim outputs As Integer = 2

			Dim ds As New DataSet(Nd4j.rand(New Integer(){3, 10, 50}), TestUtils.randomOneHotTimeSeries(3, outputs, 50, 12345))
			Dim train As DataSetIterator = New ExistingDataSetIterator(java.util.Arrays.asList(ds, ds, ds, ds, ds, ds, ds, ds, ds, ds))
			Dim test As DataSetIterator = New SingletonDataSetIterator(ds)


			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).weightInit(WeightInit.XAVIER).updater(New Adam(0.1)).activation(Activation.ELU).l2(1e-5).gradientNormalization(GradientNormalization.ClipElementWiseAbsoluteValue).gradientNormalizationThreshold(1.0).list().layer(0, (New LSTM.Builder()).nIn(10).nOut(10).activation(Activation.TANH).gateActivationFunction(Activation.SIGMOID).dropOut(0.5).build()).layer(1, (New RnnOutputLayer.Builder()).nIn(10).nOut(outputs).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

			Dim f As File = testDir.resolve("test-dir").toFile()
			Dim saver As EarlyStoppingModelSaver(Of MultiLayerNetwork) = New LocalFileModelSaver(f.getAbsolutePath())
			Dim esConf As EarlyStoppingConfiguration(Of MultiLayerNetwork) = (New EarlyStoppingConfiguration.Builder(Of MultiLayerNetwork)()).epochTerminationConditions(New MaxEpochsTerminationCondition(10), New ScoreImprovementEpochTerminationCondition(1)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(10, TimeUnit.MINUTES)).scoreCalculator(New ClassificationScoreCalculator(Evaluation.Metric.F1, test)).modelSaver(saver).saveLastModel(True).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim t As New EarlyStoppingTrainer(esConf, net, train)
			Dim result As EarlyStoppingResult(Of MultiLayerNetwork) = t.fit()

			Dim map As IDictionary(Of Integer, Double) = result.getScoreVsEpoch()
			For i As Integer = 1 To map.Count - 1
				If i = map.Count - 1 Then
					assertTrue(map(i) <+ map(i-1))
				Else
					assertTrue(map(i) > map(i-1))
				End If
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConditionJson() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConditionJson()

			Dim etc() As EpochTerminationCondition = {
				New BestScoreEpochTerminationCondition(0.5),
				New MaxEpochsTerminationCondition(10),
				New ScoreImprovementEpochTerminationCondition(3, 0.5)
			}

			For Each e As EpochTerminationCondition In etc
				Dim s As String = NeuralNetConfiguration.mapper().writeValueAsString(e)
				Dim c As EpochTerminationCondition = NeuralNetConfiguration.mapper().readValue(s, GetType(EpochTerminationCondition))
				assertEquals(e, c)
			Next e

			Dim itc() As IterationTerminationCondition = {
				New InvalidScoreIterationTerminationCondition(),
				New MaxScoreIterationTerminationCondition(10.0),
				New MaxTimeIterationTerminationCondition(10, TimeUnit.MINUTES)
			}

			For Each i As IterationTerminationCondition In itc
				Dim s As String = NeuralNetConfiguration.mapper().writeValueAsString(i)
				Dim c As IterationTerminationCondition = NeuralNetConfiguration.mapper().readValue(s, GetType(IterationTerminationCondition))
				assertEquals(i, c)
			Next i
		End Sub
	End Class

End Namespace