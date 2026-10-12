Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports RDDTrainingApproach = org.deeplearning4j.spark.api.RDDTrainingApproach
Imports org.deeplearning4j.spark.api
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Downloader = org.nd4j.common.resources.Downloader
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
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

Namespace org.deeplearning4j.spark.impl.paramavg


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Slf4j public class TestCompareParameterAveragingSparkVsSingleMachine
	Public Class TestCompareParameterAveragingSparkVsSingleMachine
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			'CudaEnvironment.getInstance().getConfiguration().allowMultiGPU(false);
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SneakyThrows @BeforeEach void before()
		Friend Overridable Sub before()
			If Platform.isWindows() Then
				Dim hadoopHome As New File(System.getProperty("java.io.tmpdir"),"hadoop-tmp")
				Dim binDir As New File(hadoopHome,"bin")
				If Not binDir.exists() Then
					binDir.mkdirs()
				End If
				Dim outputFile As New File(binDir,"winutils.exe")
				If Not outputFile.exists() Then
					log.info("Fixing spark for windows")
					Downloader.download("winutils.exe", URI.create("https://github.com/cdarlint/winutils/blob/master/hadoop-2.6.5/bin/winutils.exe?raw=true").toURL(), outputFile,"db24b404d2331a1bec7443336a5171f1",3)
				End If

				System.setProperty("hadoop.home.dir", hadoopHome.getAbsolutePath())
			End If
		End Sub


		Private Shared Function getConf(ByVal seed As Integer, ByVal updater As IUpdater) As MultiLayerConfiguration
			Nd4j.Random.setSeed(seed)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).updater(updater).seed(seed).list().layer(0, (New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build()).build()
			Return conf
		End Function

		Private Shared Function getConfCNN(ByVal seed As Integer, ByVal updater As IUpdater) As MultiLayerConfiguration
			Nd4j.Random.setSeed(seed)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).updater(updater).seed(seed).list().layer(0, (New ConvolutionLayer.Builder()).nOut(3).kernelSize(2, 2).stride(1, 1).padding(0, 0).activation(Activation.TANH).build()).layer(1, (New ConvolutionLayer.Builder()).nOut(3).kernelSize(2, 2).stride(1, 1).padding(0, 0).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nOut(10).build()).setInputType(InputType.convolutional(10, 10, 3)).build()
			Return conf
		End Function

		Private Shared Function getGraphConf(ByVal seed As Integer, ByVal updater As IUpdater) As ComputationGraphConfiguration
			Nd4j.Random.setSeed(seed)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).updater(updater).seed(seed).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(10).nOut(10).build(), "0").setOutputs("1").build()
			Return conf
		End Function

		Private Shared Function getGraphConfCNN(ByVal seed As Integer, ByVal updater As IUpdater) As ComputationGraphConfiguration
			Nd4j.Random.setSeed(seed)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).updater(updater).seed(seed).graphBuilder().addInputs("in").addLayer("0", (New ConvolutionLayer.Builder()).nOut(3).kernelSize(2, 2).stride(1, 1).padding(0, 0).activation(Activation.TANH).build(), "in").addLayer("1", (New ConvolutionLayer.Builder()).nOut(3).kernelSize(2, 2).stride(1, 1).padding(0, 0).activation(Activation.TANH).build(), "0").addLayer("2", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nOut(10).build(), "1").setOutputs("2").setInputTypes(InputType.convolutional(10, 10, 3)).build()
			Return conf
		End Function

		Private Shared Function getTrainingMaster(ByVal avgFreq As Integer, ByVal miniBatchSize As Integer) As TrainingMaster
			Return getTrainingMaster(avgFreq, miniBatchSize, True)
		End Function

		Private Shared Function getTrainingMaster(ByVal avgFreq As Integer, ByVal miniBatchSize As Integer, ByVal saveUpdater As Boolean) As TrainingMaster
			Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(1)).averagingFrequency(avgFreq).batchSizePerWorker(miniBatchSize).saveUpdater(saveUpdater).aggregationDepth(2).workerPrefetchNumBatches(0).build()
			Return tm
		End Function

		Private Shared Function getContext(ByVal nWorkers As Integer) As JavaSparkContext
			Dim sparkConf As New SparkConf()
			sparkConf.setMaster("local[" & nWorkers & "]")
			sparkConf.setAppName("Test")
			sparkConf.set("spark.driver.host", "localhost")

			Dim sc As New JavaSparkContext(sparkConf)
			Return sc
		End Function

		Private Function getOneDataSetAsIndividalExamples(ByVal totalExamples As Integer, ByVal seed As Integer) As IList(Of DataSet)
			Nd4j.Random.setSeed(seed)
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To totalExamples - 1
				Dim f As INDArray = Nd4j.rand(1, 10)
				Dim l As INDArray = Nd4j.rand(1, 10)
				Dim ds As New DataSet(f, l)
				list.Add(ds)
			Next i
			Return list
		End Function

		Private Function getOneDataSetAsIndividalExamplesCNN(ByVal totalExamples As Integer, ByVal seed As Integer) As IList(Of DataSet)
			Nd4j.Random.setSeed(seed)
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To totalExamples - 1
				Dim f As INDArray = Nd4j.rand(New Integer() {1, 3, 10, 10})
				Dim l As INDArray = Nd4j.rand(1, 10)
				Dim ds As New DataSet(f, l)
				list.Add(ds)
			Next i
			Return list
		End Function

		Private Function getOneDataSet(ByVal totalExamples As Integer, ByVal seed As Integer) As DataSet
			Return DataSet.merge(getOneDataSetAsIndividalExamples(totalExamples, seed))
		End Function

		Private Function getOneDataSetCNN(ByVal totalExamples As Integer, ByVal seed As Integer) As DataSet
			Return DataSet.merge(getOneDataSetAsIndividalExamplesCNN(totalExamples, seed))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOneExecutor()
		Public Overridable Sub testOneExecutor()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			'Idea: single worker/executor on Spark should give identical results to a single machine

			Dim miniBatchSize As Integer = 10
			Dim nWorkers As Integer = 1

			For Each saveUpdater As Boolean In New Boolean() {True, False}
				Dim sc As JavaSparkContext = getContext(nWorkers)

				Try
					'Do training locally, for 3 minibatches
					Dim seeds() As Integer = {1, 2, 3}

					Dim net As New MultiLayerNetwork(getConf(12345, New RmsProp(0.5)))
					net.init()
					Dim initialParams As INDArray = net.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim ds As DataSet = getOneDataSet(miniBatchSize, seeds(i))
						If Not saveUpdater Then
							net.Updater = Nothing
						End If
						net.fit(ds)
					Next i
					Dim finalParams As INDArray = net.params().dup()

					'Do training on Spark with one executor, for 3 separate minibatches
					Dim tm As TrainingMaster = getTrainingMaster(1, miniBatchSize, saveUpdater)
					Dim sparkNet As New SparkDl4jMultiLayer(sc, getConf(12345, New RmsProp(0.5)), tm)
					sparkNet.CollectTrainingStats = True
					Dim initialSparkParams As INDArray = sparkNet.Network.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim list As IList(Of DataSet) = getOneDataSetAsIndividalExamples(miniBatchSize, seeds(i))
						Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

						sparkNet.fit(rdd)
					Next i

					Dim finalSparkParams As INDArray = sparkNet.Network.params().dup()

					assertEquals(initialParams, initialSparkParams)
					assertNotEquals(initialParams, finalParams)
					assertEquals(finalParams, finalSparkParams)
				Finally
					sc.stop()
				End Try
			Next saveUpdater
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testOneExecutorGraph()
		Public Overridable Sub testOneExecutorGraph()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			'Idea: single worker/executor on Spark should give identical results to a single machine

			Dim miniBatchSize As Integer = 10
			Dim nWorkers As Integer = 1

			For Each saveUpdater As Boolean In New Boolean() {True, False}
				Dim sc As JavaSparkContext = getContext(nWorkers)

				Try
					'Do training locally, for 3 minibatches
					Dim seeds() As Integer = {1, 2, 3}

					Dim net As New ComputationGraph(getGraphConf(12345, New RmsProp(0.5)))
					net.init()
					Dim initialParams As INDArray = net.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim ds As DataSet = getOneDataSet(miniBatchSize, seeds(i))
						If Not saveUpdater Then
							net.Updater = Nothing
						End If
						net.fit(ds)
					Next i
					Dim finalParams As INDArray = net.params().dup()

					'Do training on Spark with one executor, for 3 separate minibatches
					Dim tm As TrainingMaster = getTrainingMaster(1, miniBatchSize, saveUpdater)
					Dim sparkNet As New SparkComputationGraph(sc, getGraphConf(12345, New RmsProp(0.5)), tm)
					sparkNet.CollectTrainingStats = True
					Dim initialSparkParams As INDArray = sparkNet.Network.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim list As IList(Of DataSet) = getOneDataSetAsIndividalExamples(miniBatchSize, seeds(i))
						Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

						sparkNet.fit(rdd)
					Next i

					Dim finalSparkParams As INDArray = sparkNet.Network.params().dup()

					assertEquals(initialParams, initialSparkParams)
					assertNotEquals(initialParams, finalParams)
					assertEquals(finalParams, finalSparkParams)
				Finally
					sc.stop()
				End Try
			Next saveUpdater
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAverageEveryStep()
		Public Overridable Sub testAverageEveryStep()
			'Idea: averaging every step with SGD (SGD updater + optimizer) is mathematically identical to doing the learning
			' on a single machine for synchronous distributed training
			'BUT: This is *ONLY* the case if all workers get an identical number of examples. This won't be the case if
			' we use RDD.randomSplit (which is what occurs if we use .fit(JavaRDD<DataSet> on a data set that needs splitting),
			' which might give a number of examples that isn't divisible by number of workers (like 39 examples on 4 executors)
			'This is also ONLY the case using SGD updater

			Dim miniBatchSizePerWorker As Integer = 10
			Dim nWorkers As Integer = 4


			For Each saveUpdater As Boolean In New Boolean() {True, False}
				Dim sc As JavaSparkContext = getContext(nWorkers)

				Try
					'Do training locally, for 3 minibatches
					Dim seeds() As Integer = {1, 2, 3}

					'                CudaGridExecutioner executioner = (CudaGridExecutioner) Nd4j.getExecutioner();

					Dim net As New MultiLayerNetwork(getConf(12345, New Sgd(0.5)))
					net.init()
					Dim initialParams As INDArray = net.params().dup()
					'              executioner.addToWatchdog(initialParams, "initialParams");


					For i As Integer = 0 To seeds.Length - 1
						Dim ds As DataSet = getOneDataSet(miniBatchSizePerWorker * nWorkers, seeds(i))
						If Not saveUpdater Then
							net.Updater = Nothing
						End If
						net.fit(ds)
					Next i
					Dim finalParams As INDArray = net.params().dup()

					'Do training on Spark with one executor, for 3 separate minibatches
					'                TrainingMaster tm = getTrainingMaster(1, miniBatchSizePerWorker, saveUpdater);
					Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(1)).averagingFrequency(1).batchSizePerWorker(miniBatchSizePerWorker).saveUpdater(saveUpdater).workerPrefetchNumBatches(0).rddTrainingApproach(RDDTrainingApproach.Export).build()
					Dim sparkNet As New SparkDl4jMultiLayer(sc, getConf(12345, New Sgd(0.5)), tm)
					sparkNet.CollectTrainingStats = True
					Dim initialSparkParams As INDArray = sparkNet.Network.params().dup()

					'            executioner.addToWatchdog(initialSparkParams, "initialSparkParams");

					For i As Integer = 0 To seeds.Length - 1
						Dim list As IList(Of DataSet) = getOneDataSetAsIndividalExamples(miniBatchSizePerWorker * nWorkers, seeds(i))
						Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

						sparkNet.fit(rdd)
					Next i

	'                System.out.println(sparkNet.getSparkTrainingStats().statsAsString());
					sparkNet.SparkTrainingStats.statsAsString()

					Dim finalSparkParams As INDArray = sparkNet.Network.params().dup()

	'                System.out.println("Initial (Local) params:       " + Arrays.toString(initialParams.data().asFloat()));
	'                System.out.println("Initial (Spark) params:       "
	'                                + Arrays.toString(initialSparkParams.data().asFloat()));
	'                System.out.println("Final (Local) params: " + Arrays.toString(finalParams.data().asFloat()));
	'                System.out.println("Final (Spark) params: " + Arrays.toString(finalSparkParams.data().asFloat()));
					assertEquals(initialParams, initialSparkParams)
					assertNotEquals(initialParams, finalParams)
					assertEquals(finalParams, finalSparkParams)

					Dim sparkScore As Double = sparkNet.Score
					assertTrue(sparkScore > 0.0)

					assertEquals(net.score(), sparkScore, 1e-3)
				Finally
					sc.stop()
				End Try
			Next saveUpdater
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAverageEveryStepCNN()
		Public Overridable Sub testAverageEveryStepCNN()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			'Idea: averaging every step with SGD (SGD updater + optimizer) is mathematically identical to doing the learning
			' on a single machine for synchronous distributed training
			'BUT: This is *ONLY* the case if all workers get an identical number of examples. This won't be the case if
			' we use RDD.randomSplit (which is what occurs if we use .fit(JavaRDD<DataSet> on a data set that needs splitting),
			' which might give a number of examples that isn't divisible by number of workers (like 39 examples on 4 executors)
			'This is also ONLY the case using SGD updater

			Dim miniBatchSizePerWorker As Integer = 10
			Dim nWorkers As Integer = 4


			For Each saveUpdater As Boolean In New Boolean() {True, False}
				Dim sc As JavaSparkContext = getContext(nWorkers)

				Try
					'Do training locally, for 3 minibatches
					Dim seeds() As Integer = {1, 2, 3}

					Dim net As New MultiLayerNetwork(getConfCNN(12345, New Sgd(0.5)))
					net.init()
					Dim initialParams As INDArray = net.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim ds As DataSet = getOneDataSetCNN(miniBatchSizePerWorker * nWorkers, seeds(i))
						If Not saveUpdater Then
							net.Updater = Nothing
						End If
						net.fit(ds)
					Next i
					Dim finalParams As INDArray = net.params().dup()

					'Do training on Spark with one executor, for 3 separate minibatches
					Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(1)).averagingFrequency(1).batchSizePerWorker(miniBatchSizePerWorker).saveUpdater(saveUpdater).workerPrefetchNumBatches(0).rddTrainingApproach(RDDTrainingApproach.Export).build()
					Dim sparkNet As New SparkDl4jMultiLayer(sc, getConfCNN(12345, New Sgd(0.5)), tm)
					sparkNet.CollectTrainingStats = True
					Dim initialSparkParams As INDArray = sparkNet.Network.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim list As IList(Of DataSet) = getOneDataSetAsIndividalExamplesCNN(miniBatchSizePerWorker * nWorkers, seeds(i))
						Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

						sparkNet.fit(rdd)
					Next i

	'                System.out.println(sparkNet.getSparkTrainingStats().statsAsString());
					sparkNet.SparkTrainingStats.statsAsString()

					Dim finalSparkParams As INDArray = sparkNet.Network.params().dup()

	'                System.out.println("Initial (Local) params:       " + Arrays.toString(initialParams.data().asFloat()));
	'                System.out.println("Initial (Spark) params:       "
	'                                + Arrays.toString(initialSparkParams.data().asFloat()));
	'                System.out.println("Final (Local) params: " + Arrays.toString(finalParams.data().asFloat()));
	'                System.out.println("Final (Spark) params: " + Arrays.toString(finalSparkParams.data().asFloat()));
					assertArrayEquals(initialParams.data().asFloat(), initialSparkParams.data().asFloat(), 1e-8f)
					assertArrayEquals(finalParams.data().asFloat(), finalSparkParams.data().asFloat(), 1e-6f)

					Dim sparkScore As Double = sparkNet.Score
					assertTrue(sparkScore > 0.0)

					assertEquals(net.score(), sparkScore, 1e-3)
				Finally
					sc.stop()
				End Try
			Next saveUpdater
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAverageEveryStepGraph()
		Public Overridable Sub testAverageEveryStepGraph()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			'Idea: averaging every step with SGD (SGD updater + optimizer) is mathematically identical to doing the learning
			' on a single machine for synchronous distributed training
			'BUT: This is *ONLY* the case if all workers get an identical number of examples. This won't be the case if
			' we use RDD.randomSplit (which is what occurs if we use .fit(JavaRDD<DataSet> on a data set that needs splitting),
			' which might give a number of examples that isn't divisible by number of workers (like 39 examples on 4 executors)
			'This is also ONLY the case using SGD updater

			Dim miniBatchSizePerWorker As Integer = 10
			Dim nWorkers As Integer = 4


			For Each saveUpdater As Boolean In New Boolean() {True, False}
				Dim sc As JavaSparkContext = getContext(nWorkers)

				Try
					'Do training locally, for 3 minibatches
					Dim seeds() As Integer = {1, 2, 3}

					'                CudaGridExecutioner executioner = (CudaGridExecutioner) Nd4j.getExecutioner();

					Dim net As New ComputationGraph(getGraphConf(12345, New Sgd(0.5)))
					net.init()
					Dim initialParams As INDArray = net.params().dup()
					'                executioner.addToWatchdog(initialParams, "initialParams");

					For i As Integer = 0 To seeds.Length - 1
						Dim ds As DataSet = getOneDataSet(miniBatchSizePerWorker * nWorkers, seeds(i))
						If Not saveUpdater Then
							net.Updater = Nothing
						End If
						net.fit(ds)
					Next i
					Dim finalParams As INDArray = net.params().dup()
					'                executioner.addToWatchdog(finalParams, "finalParams");

					'Do training on Spark with one executor, for 3 separate minibatches
					Dim tm As TrainingMaster = getTrainingMaster(1, miniBatchSizePerWorker, saveUpdater)
					Dim sparkNet As New SparkComputationGraph(sc, getGraphConf(12345, New Sgd(0.5)), tm)
					sparkNet.CollectTrainingStats = True
					Dim initialSparkParams As INDArray = sparkNet.Network.params().dup()

					'                executioner.addToWatchdog(initialSparkParams, "initialSparkParams");

					For i As Integer = 0 To seeds.Length - 1
						Dim list As IList(Of DataSet) = getOneDataSetAsIndividalExamples(miniBatchSizePerWorker * nWorkers, seeds(i))
						Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

						sparkNet.fit(rdd)
					Next i

	'                System.out.println(sparkNet.getSparkTrainingStats().statsAsString());
					sparkNet.SparkTrainingStats.statsAsString()

					Dim finalSparkParams As INDArray = sparkNet.Network.params().dup()
					'                executioner.addToWatchdog(finalSparkParams, "finalSparkParams");

					Dim fp() As Single = finalParams.data().asFloat()
					Dim fps() As Single = finalSparkParams.data().asFloat()
	'                System.out.println("Initial (Local) params:       " + Arrays.toString(initialParams.data().asFloat()));
	'                System.out.println("Initial (Spark) params:       "
	'                                + Arrays.toString(initialSparkParams.data().asFloat()));
	'                System.out.println("Final (Local) params: " + Arrays.toString(fp));
	'                System.out.println("Final (Spark) params: " + Arrays.toString(fps));

					assertEquals(initialParams, initialSparkParams)
					assertNotEquals(initialParams, finalParams)
					assertArrayEquals(fp, fps, 1e-5f)

					Dim sparkScore As Double = sparkNet.Score
					assertTrue(sparkScore > 0.0)

					assertEquals(net.score(), sparkScore, 1e-3)
				Finally
					sc.stop()
				End Try
			Next saveUpdater
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAverageEveryStepGraphCNN()
		Public Overridable Sub testAverageEveryStepGraphCNN()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			'Idea: averaging every step with SGD (SGD updater + optimizer) is mathematically identical to doing the learning
			' on a single machine for synchronous distributed training
			'BUT: This is *ONLY* the case if all workers get an identical number of examples. This won't be the case if
			' we use RDD.randomSplit (which is what occurs if we use .fit(JavaRDD<DataSet> on a data set that needs splitting),
			' which might give a number of examples that isn't divisible by number of workers (like 39 examples on 4 executors)
			'This is also ONLY the case using SGD updater

			Dim miniBatchSizePerWorker As Integer = 10
			Dim nWorkers As Integer = 4


			For Each saveUpdater As Boolean In New Boolean() {True, False}
				Dim sc As JavaSparkContext = getContext(nWorkers)

				Try
					'Do training locally, for 3 minibatches
					Dim seeds() As Integer = {1, 2, 3}

					Dim net As New ComputationGraph(getGraphConfCNN(12345, New Sgd(0.5)))
					net.init()
					Dim initialParams As INDArray = net.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim ds As DataSet = getOneDataSetCNN(miniBatchSizePerWorker * nWorkers, seeds(i))
						If Not saveUpdater Then
							net.Updater = Nothing
						End If
						net.fit(ds)
					Next i
					Dim finalParams As INDArray = net.params().dup()

					'Do training on Spark with one executor, for 3 separate minibatches
					Dim tm As TrainingMaster = getTrainingMaster(1, miniBatchSizePerWorker, saveUpdater)
					Dim sparkNet As New SparkComputationGraph(sc, getGraphConfCNN(12345, New Sgd(0.5)), tm)
					sparkNet.CollectTrainingStats = True
					Dim initialSparkParams As INDArray = sparkNet.Network.params().dup()

					For i As Integer = 0 To seeds.Length - 1
						Dim list As IList(Of DataSet) = getOneDataSetAsIndividalExamplesCNN(miniBatchSizePerWorker * nWorkers, seeds(i))
						Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

						sparkNet.fit(rdd)
					Next i

	'                System.out.println(sparkNet.getSparkTrainingStats().statsAsString());
					sparkNet.SparkTrainingStats.statsAsString()

					Dim finalSparkParams As INDArray = sparkNet.Network.params().dup()

	'                System.out.println("Initial (Local) params:  " + Arrays.toString(initialParams.data().asFloat()));
	'                System.out.println("Initial (Spark) params:  " + Arrays.toString(initialSparkParams.data().asFloat()));
	'                System.out.println("Final (Local) params:    " + Arrays.toString(finalParams.data().asFloat()));
	'                System.out.println("Final (Spark) params:    " + Arrays.toString(finalSparkParams.data().asFloat()));
					assertArrayEquals(initialParams.data().asFloat(), initialSparkParams.data().asFloat(), 1e-8f)
					assertArrayEquals(finalParams.data().asFloat(), finalSparkParams.data().asFloat(), 1e-6f)

					Dim sparkScore As Double = sparkNet.Score
					assertTrue(sparkScore > 0.0)

					assertEquals(net.score(), sparkScore, 1e-3)
				Finally
					sc.stop()
				End Try
			Next saveUpdater
		End Sub
	End Class

End Namespace