Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping
Imports org.deeplearning4j.earlystopping.listener
Imports org.deeplearning4j.earlystopping.saver
Imports MaxEpochsTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxEpochsTerminationCondition
Imports MaxScoreIterationTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxScoreIterationTerminationCondition
Imports MaxTimeIterationTerminationCondition = org.deeplearning4j.earlystopping.termination.MaxTimeIterationTerminationCondition
Imports ScoreImprovementEpochTerminationCondition = org.deeplearning4j.earlystopping.termination.ScoreImprovementEpochTerminationCondition
Imports org.deeplearning4j.earlystopping.trainer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports org.deeplearning4j.spark.api
Imports SparkEarlyStoppingGraphTrainer = org.deeplearning4j.spark.earlystopping.SparkEarlyStoppingGraphTrainer
Imports SparkLossCalculatorComputationGraph = org.deeplearning4j.spark.earlystopping.SparkLossCalculatorComputationGraph
Imports DataSetToMultiDataSetFn = org.deeplearning4j.spark.impl.graph.dataset.DataSetToMultiDataSetFn
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.spark


	<Serializable>
	Public Class TestEarlyStoppingSparkCompGraph
		Inherits BaseSparkTest


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEarlyStoppingIris()
		Public Overridable Sub testEarlyStoppingIris()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(5))


			Dim irisData As JavaRDD(Of DataSet) = getIris()

			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(2, TimeUnit.MINUTES)).scoreCalculator(New SparkLossCalculatorComputationGraph(irisData.map(New DataSetToMultiDataSetFn()), True, sc.sc())).modelSaver(saver).build()

			Dim tm As TrainingMaster = New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0)

			Dim trainer As IEarlyStoppingTrainer(Of ComputationGraph) = New SparkEarlyStoppingGraphTrainer(Context.sc(), tm, esConf, net, irisData.map(New DataSetToMultiDataSetFn()))

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
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim score As Double = bestNetwork.score((New IrisDataSetIterator(150, 150)).next())
			Dim bestModelScore As Double = result.getBestModelScore()
			assertEquals(bestModelScore, score, 1e-3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBadTuning()
		Public Overridable Sub testBadTuning()
			'Test poor tuning (high LR): should terminate on MaxScoreIterationTerminationCondition
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(2.0)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(5))

			Dim irisData As JavaRDD(Of DataSet) = getIris()
			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5000)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(2, TimeUnit.MINUTES), New MaxScoreIterationTerminationCondition(7.5)).scoreCalculator(New SparkLossCalculatorComputationGraph(irisData.map(New DataSetToMultiDataSetFn()), True, sc.sc())).modelSaver(saver).build()

			Dim tm As TrainingMaster = New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0)

			Dim trainer As IEarlyStoppingTrainer(Of ComputationGraph) = New SparkEarlyStoppingGraphTrainer(Context.sc(), tm, esConf, net, irisData.map(New DataSetToMultiDataSetFn()))
			Dim result As EarlyStoppingResult = trainer.fit()

			assertTrue(result.getTotalEpochs() < 5)
			assertEquals(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New MaxScoreIterationTerminationCondition(7.5)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeTermination()
		Public Overridable Sub testTimeTermination()
			'test termination after max time
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(1e-6)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(5))

			Dim irisData As JavaRDD(Of DataSet) = getIris()

			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(10000)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(3, TimeUnit.SECONDS), New MaxScoreIterationTerminationCondition(7.5)).scoreCalculator(New SparkLossCalculatorComputationGraph(irisData.map(New DataSetToMultiDataSetFn()), True, sc.sc())).modelSaver(saver).build()

			Dim tm As TrainingMaster = New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0)

			Dim trainer As IEarlyStoppingTrainer(Of ComputationGraph) = New SparkEarlyStoppingGraphTrainer(Context.sc(), tm, esConf, net, irisData.map(New DataSetToMultiDataSetFn()))
			Dim startTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim result As EarlyStoppingResult = trainer.fit()
			Dim endTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim durationSeconds As Integer = CInt(endTime - startTime) \ 1000

			assertTrue(durationSeconds >= 3)
			assertTrue(durationSeconds <= 20)

			assertEquals(EarlyStoppingResult.TerminationReason.IterationTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New MaxTimeIterationTerminationCondition(3, TimeUnit.SECONDS)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNoImprovementNEpochsTermination()
		Public Overridable Sub testNoImprovementNEpochsTermination()
			'Idea: terminate training if score (test set loss) does not improve for 5 consecutive epochs
			'Simulate this by setting LR = 0.0
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.0)).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(5))

			Dim irisData As JavaRDD(Of DataSet) = getIris()

			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(100), New ScoreImprovementEpochTerminationCondition(5)).iterationTerminationConditions(New MaxScoreIterationTerminationCondition(7.5)).scoreCalculator(New SparkLossCalculatorComputationGraph(irisData.map(New DataSetToMultiDataSetFn()), True, sc.sc())).modelSaver(saver).build()

			Dim tm As TrainingMaster = New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0)

			Dim trainer As IEarlyStoppingTrainer(Of ComputationGraph) = New SparkEarlyStoppingGraphTrainer(Context.sc(), tm, esConf, net, irisData.map(New DataSetToMultiDataSetFn()))
			Dim result As EarlyStoppingResult = trainer.fit()

			'Expect no score change due to 0 LR -> terminate after 6 total epochs
			assertTrue(result.getTotalEpochs() < 12) 'Normally expect 6 epochs exactly; get a little more than that here due to rounding + order of operations
			assertEquals(EarlyStoppingResult.TerminationReason.EpochTerminationCondition, result.getTerminationReason())
			Dim expDetails As String = (New ScoreImprovementEpochTerminationCondition(5)).ToString()
			assertEquals(expDetails, result.getTerminationDetails())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListeners()
		Public Overridable Sub testListeners()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(4).nOut(3).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.setListeners(New ScoreIterationListener(5))


			Dim irisData As JavaRDD(Of DataSet) = getIris()

			Dim saver As EarlyStoppingModelSaver(Of ComputationGraph) = New InMemoryModelSaver(Of ComputationGraph)()
			Dim esConf As EarlyStoppingConfiguration(Of ComputationGraph) = (New EarlyStoppingConfiguration.Builder(Of ComputationGraph)()).epochTerminationConditions(New MaxEpochsTerminationCondition(5)).iterationTerminationConditions(New MaxTimeIterationTerminationCondition(2, TimeUnit.MINUTES)).scoreCalculator(New SparkLossCalculatorComputationGraph(irisData.map(New DataSetToMultiDataSetFn()), True, sc.sc())).modelSaver(saver).build()

			Dim listener As New LoggingEarlyStoppingListener()

			Dim tm As TrainingMaster = New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0)

			Dim trainer As IEarlyStoppingTrainer(Of ComputationGraph) = New SparkEarlyStoppingGraphTrainer(Context.sc(), tm, esConf, net, irisData.map(New DataSetToMultiDataSetFn()))
			trainer.Listener = listener

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
				log.info("EorlyStopping: onCompletion called (result: {})", esResult)
				onCompletionCallCount += 1
			End Sub
		End Class

		Private ReadOnly Property Iris As JavaRDD(Of DataSet)
			Get
    
				Dim sc As JavaSparkContext = Context
    
				Dim iter As New IrisDataSetIterator(1, 150)
				Dim list As IList(Of DataSet) = New List(Of DataSet)(150)
				Do While iter.MoveNext()
					list.Add(iter.Current)
				Loop
    
				Return sc.parallelize(list)
			End Get
		End Property
	End Class

End Namespace