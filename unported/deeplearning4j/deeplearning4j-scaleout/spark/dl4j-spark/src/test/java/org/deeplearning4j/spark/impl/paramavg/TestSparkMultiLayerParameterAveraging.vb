Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Platform = com.sun.jna.Platform
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports LocatedFileStatus = org.apache.hadoop.fs.LocatedFileStatus
Imports RemoteIterator = org.apache.hadoop.fs.RemoteIterator
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Vectors = org.apache.spark.mllib.linalg.Vectors
Imports LabeledPoint = org.apache.spark.mllib.regression.LabeledPoint
Imports MLUtils = org.apache.spark.mllib.util.MLUtils
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports GaussianReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.GaussianReconstructionDistribution
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports Repartition = org.deeplearning4j.spark.api.Repartition
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports EventStats = org.deeplearning4j.spark.stats.EventStats
Imports ExampleCountEventStats = org.deeplearning4j.spark.stats.ExampleCountEventStats
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Tuple2 = scala.Tuple2
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
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestSparkMultiLayerParameterAveraging extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestSparkMultiLayerParameterAveraging
		Inherits BaseSparkTest

		Public Class TestFn
			Implements [Function](Of LabeledPoint, LabeledPoint)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.apache.spark.mllib.regression.LabeledPoint call(org.apache.spark.mllib.regression.LabeledPoint v1) throws Exception
			Public Overrides Function [call](ByVal v1 As LabeledPoint) As LabeledPoint
				Return New LabeledPoint(v1.label(), Vectors.dense(v1.features().toArray()))
			End Function
		End Class




		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000L
			End Get
		End Property

		Public Overrides ReadOnly Property DefaultFPDataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFromSvmLightBackprop() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFromSvmLightBackprop()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim data As JavaRDD(Of LabeledPoint) = MLUtils.loadLibSVMFile(sc.sc(), (New ClassPathResource("svmLight/iris_svmLight_0.txt")).TempFileFromArchive.getAbsolutePath()).toJavaRDD().map(New TestFn())

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim d As DataSet = (New IrisDataSetIterator(150, 150)).next()
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(100).weightInit(WeightInit.XAVIER).activation(Activation.RELU).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(100).nOut(3).activation(Activation.SOFTMAX).weightInit(WeightInit.XAVIER).build()).build()



			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Console.WriteLine("Initializing network")

			Dim master As New SparkDl4jMultiLayer(sc, conf, New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 5, 1, 0))

			Dim network2 As MultiLayerNetwork = master.fitLabeledPoint(data)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFromSvmLight() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFromSvmLight()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim data As JavaRDD(Of LabeledPoint) = MLUtils.loadLibSVMFile(sc.sc(), (New ClassPathResource("svmLight/iris_svmLight_0.txt")).TempFileFromArchive.getAbsolutePath()).toJavaRDD().map(New TestFn())

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).updater(New Adam(1e-6)).weightInit(WeightInit.XAVIER).list().layer((New BatchNormalization.Builder()).nIn(4).nOut(4).build()).layer((New DenseLayer.Builder()).nIn(4).nOut(32).activation(Activation.RELU).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(32).nOut(3).activation(Activation.SOFTMAX).build()).build()



			Dim network As New MultiLayerNetwork(conf)
			network.init()
			Console.WriteLine("Initializing network")
			Dim master As New SparkDl4jMultiLayer(sc, BasicConf, New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 5, 1, 0))

			master.fitLabeledPoint(data)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRunIteration()
		Public Overridable Sub testRunIteration()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim dataSet As DataSet = (New IrisDataSetIterator(5, 5)).next()
			Dim list As IList(Of DataSet) = dataSet.asList()
			Dim data As JavaRDD(Of DataSet) = sc.parallelize(list)

			Dim sparkNetCopy As New SparkDl4jMultiLayer(sc, BasicConf, New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 5, 1, 0))
			Dim networkCopy As MultiLayerNetwork = sparkNetCopy.fit(data)

			Dim expectedParams As INDArray = networkCopy.params()

			Dim sparkNet As SparkDl4jMultiLayer = BasicNetwork
			Dim network As MultiLayerNetwork = sparkNet.fit(data)
			Dim actualParams As INDArray = network.params()

			assertEquals(expectedParams.size(1), actualParams.size(1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUpdaters()
		Public Overridable Sub testUpdaters()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim sparkNet As SparkDl4jMultiLayer = BasicNetwork
			Dim netCopy As MultiLayerNetwork = sparkNet.Network.clone()

			netCopy.fit(data)
			Dim expectedUpdater As IUpdater = CType(netCopy.conf().getLayer(), BaseLayer).getIUpdater()
			Dim expectedLR As Double = CType(CType(netCopy.conf().getLayer(), BaseLayer).getIUpdater(), Nesterovs).getLearningRate()
			Dim expectedMomentum As Double = CType(CType(netCopy.conf().getLayer(), BaseLayer).getIUpdater(), Nesterovs).getMomentum()

			Dim actualUpdater As IUpdater = CType(sparkNet.Network.conf().getLayer(), BaseLayer).getIUpdater()
			sparkNet.fit(sparkData)
			Dim actualLR As Double = CType(CType(sparkNet.Network.conf().getLayer(), BaseLayer).getIUpdater(), Nesterovs).getLearningRate()
			Dim actualMomentum As Double = CType(CType(sparkNet.Network.conf().getLayer(), BaseLayer).getIUpdater(), Nesterovs).getMomentum()

			assertEquals(expectedUpdater, actualUpdater)
			assertEquals(expectedLR, actualLR, 0.01)
			assertEquals(expectedMomentum, actualMomentum, 0.01)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEvaluation()
		Public Overridable Sub testEvaluation()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim sparkNet As SparkDl4jMultiLayer = BasicNetwork
			Dim netCopy As MultiLayerNetwork = sparkNet.Network.clone()

			Dim evalExpected As New Evaluation()
			Dim outLocal As INDArray = netCopy.output(input, Layer.TrainingMode.TEST)
			evalExpected.eval(labels, outLocal)

			Dim evalActual As Evaluation = sparkNet.evaluate(sparkData)

			assertEquals(evalExpected.accuracy(), evalActual.accuracy(), 1e-3)
			assertEquals(evalExpected.f1(), evalActual.f1(), 1e-3)
			assertEquals(evalExpected.NumRowCounter, evalActual.NumRowCounter, 1e-3)
			assertMapEquals(evalExpected.falseNegatives(), evalActual.falseNegatives())
			assertMapEquals(evalExpected.falsePositives(), evalActual.falsePositives())
			assertMapEquals(evalExpected.trueNegatives(), evalActual.trueNegatives())
			assertMapEquals(evalExpected.truePositives(), evalActual.truePositives())
			assertEquals(evalExpected.precision(), evalActual.precision(), 1e-3)
			assertEquals(evalExpected.recall(), evalActual.recall(), 1e-3)
			assertEquals(evalExpected.getConfusionMatrix(), evalActual.getConfusionMatrix())
		End Sub

		Private Shared Sub assertMapEquals(ByVal first As IDictionary(Of Integer, Integer), ByVal second As IDictionary(Of Integer, Integer))
			assertEquals(first.Keys, second.Keys)
			For Each i As Integer? In first.Keys
				assertEquals(first(i), second(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSmallAmountOfData()
		Public Overridable Sub testSmallAmountOfData()
			'Idea: Test spark training where some executors don't get any data
			'in this case: by having fewer examples (2 DataSets) than executors (local[*])
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(3).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(3).nOut(nOut).activation(Activation.SOFTMAX).build()).build()

			Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0))

			Nd4j.Random.setSeed(12345)
			Dim d1 As New DataSet(Nd4j.rand(1, nIn), Nd4j.rand(1, nOut))
			Dim d2 As New DataSet(Nd4j.rand(1, nIn), Nd4j.rand(1, nOut))

			Dim rddData As JavaRDD(Of DataSet) = sc.parallelize(java.util.Arrays.asList(d1, d2))

			sparkNet.fit(rddData)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDistributedScoring()
		Public Overridable Sub testDistributedScoring()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).l1(0.1).l2(0.1).seed(123).updater(New Nesterovs(0.1, 0.9)).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(3).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(nOut).activation(Activation.SOFTMAX).build()).build()

			Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0))
			Dim netCopy As MultiLayerNetwork = sparkNet.Network.clone()

			Dim nRows As Integer = 100

			Dim features As INDArray = Nd4j.rand(nRows, nIn)
			Dim labels As INDArray = Nd4j.zeros(nRows, nOut)
			Dim r As New Random(12345)
			For i As Integer = 0 To nRows - 1
				labels.putScalar(New Integer() {i, r.Next(nOut)}, 1.0)
			Next i

			Dim localScoresWithReg As INDArray = netCopy.scoreExamples(New DataSet(features, labels), True)
			Dim localScoresNoReg As INDArray = netCopy.scoreExamples(New DataSet(features, labels), False)

			Dim dataWithKeys As IList(Of Tuple2(Of String, DataSet)) = New List(Of Tuple2(Of String, DataSet))()
			For i As Integer = 0 To nRows - 1
				Dim ds As New DataSet(features.getRow(i,True).dup(), labels.getRow(i,True).dup())
				dataWithKeys.Add(New Tuple2(Of String, DataSet)(i.ToString(), ds))
			Next i
			Dim dataWithKeysRdd As JavaPairRDD(Of String, DataSet) = sc.parallelizePairs(dataWithKeys)

			Dim sparkScoresWithReg As JavaPairRDD(Of String, Double) = sparkNet.scoreExamples(dataWithKeysRdd, True, 4)
			Dim sparkScoresNoReg As JavaPairRDD(Of String, Double) = sparkNet.scoreExamples(dataWithKeysRdd, False, 4)

			Dim sparkScoresWithRegMap As IDictionary(Of String, Double) = sparkScoresWithReg.collectAsMap()
			Dim sparkScoresNoRegMap As IDictionary(Of String, Double) = sparkScoresNoReg.collectAsMap()

			For i As Integer = 0 To nRows - 1
				Dim scoreRegExp As Double = localScoresWithReg.getDouble(i)
				Dim scoreRegAct As Double = sparkScoresWithRegMap(i.ToString())
				assertEquals(scoreRegExp, scoreRegAct, 1e-5)

				Dim scoreNoRegExp As Double = localScoresNoReg.getDouble(i)
				Dim scoreNoRegAct As Double = sparkScoresNoRegMap(i.ToString())
				assertEquals(scoreNoRegExp, scoreNoRegAct, 1e-5)

				'            System.out.println(scoreRegExp + "\t" + scoreRegAct + "\t" + scoreNoRegExp + "\t" + scoreNoRegAct);
			Next i

			Dim dataNoKeys As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To nRows - 1
				dataNoKeys.Add(New DataSet(features.getRow(i,True).dup(), labels.getRow(i,True).dup()))
			Next i
			Dim dataNoKeysRdd As JavaRDD(Of DataSet) = sc.parallelize(dataNoKeys)

			Dim scoresWithReg As IList(Of Double) = New List(Of Double)(sparkNet.scoreExamples(dataNoKeysRdd, True, 4).collect())
			Dim scoresNoReg As IList(Of Double) = New List(Of Double)(sparkNet.scoreExamples(dataNoKeysRdd, False, 4).collect())
			scoresWithReg.Sort()
			scoresNoReg.Sort()
			Dim localScoresWithRegDouble() As Double = localScoresWithReg.data().asDouble()
			Dim localScoresNoRegDouble() As Double = localScoresNoReg.data().asDouble()
			Array.Sort(localScoresWithRegDouble)
			Array.Sort(localScoresNoRegDouble)

			For i As Integer = 0 To localScoresWithRegDouble.Length - 1
				assertEquals(localScoresWithRegDouble(i), scoresWithReg(i), 1e-5)
				assertEquals(localScoresNoRegDouble(i), scoresNoReg(i), 1e-5)

				'System.out.println(localScoresWithRegDouble[i] + "\t" + scoresWithReg.get(i) + "\t" + localScoresNoRegDouble[i] + "\t" + scoresNoReg.get(i));
			Next i
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testParameterAveragingMultipleExamplesPerDataSet() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testParameterAveragingMultipleExamplesPerDataSet()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim dataSetObjSize As Integer = 5
			Dim batchSizePerExecutor As Integer = 25
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			Dim iter As DataSetIterator = New MnistDataSetIterator(dataSetObjSize, 1000, False)
			Do While iter.MoveNext()
				list.Add(iter.Current)
			Loop

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28).nOut(50).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(50).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(1).aggregationDepth(1).repartionData(Repartition.Always).build())
			sparkNet.CollectTrainingStats = True

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

			sparkNet.fit(rdd)

			Dim stats As SparkTrainingStats = sparkNet.SparkTrainingStats

			Dim mapPartitionStats As IList(Of EventStats) = stats.getValue("ParameterAveragingMasterMapPartitionsTimesMs")
			Dim numSplits As Integer = list.Count * dataSetObjSize \ (numExecutors() * batchSizePerExecutor) 'For an averaging frequency of 1
			assertEquals(numSplits, mapPartitionStats.Count)


			Dim workerFitStats As IList(Of EventStats) = stats.getValue("ParameterAveragingWorkerFitTimesMs")
			For Each e As EventStats In workerFitStats
				Dim eces As ExampleCountEventStats = DirectCast(e, ExampleCountEventStats)
	'            System.out.println(eces.getTotalExampleCount());
			Next e

			For Each e As EventStats In workerFitStats
				Dim eces As ExampleCountEventStats = DirectCast(e, ExampleCountEventStats)
				assertEquals(batchSizePerExecutor, eces.getTotalExampleCount())
			Next e
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Permissions issues on CI") public void testFitViaStringPaths(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFitViaStringPaths(ByVal testDir As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim tempDir As Path = (New File(testDir.toFile(),"DL4J-testFitViaStringPaths")).toPath()
			Dim tempDirF As File = tempDir.toFile()
			tempDirF.deleteOnExit()

			Dim dataSetObjSize As Integer = 5
			Dim batchSizePerExecutor As Integer = 25
			Dim iter As DataSetIterator = New MnistDataSetIterator(dataSetObjSize, 1000, False)
			Dim i As Integer = 0
			Do While iter.MoveNext()
				Dim nextFile As New File(tempDirF, i & ".bin")
				Dim ds As DataSet = iter.Current
				ds.save(nextFile)
				i += 1
			Loop

			Console.WriteLine("Saved to: " & tempDirF.getAbsolutePath())



			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28).nOut(50).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(50).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).workerPrefetchNumBatches(5).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(1).repartionData(Repartition.Always).build())
			sparkNet.CollectTrainingStats = True


			'List files:
			Dim config As New Configuration()
			Dim hdfs As FileSystem = FileSystem.get(tempDir.toUri(), config)
			Dim fileIter As RemoteIterator(Of LocatedFileStatus) = hdfs.listFiles(New org.apache.hadoop.fs.Path(tempDir.ToString()), False)

			Dim paths As IList(Of String) = New List(Of String)()
			Do While fileIter.hasNext()
				Dim path As String = fileIter.next().getPath().ToString()
				paths.Add(path)
			Loop

			Dim paramsBefore As INDArray = sparkNet.Network.params().dup()
			Dim pathRdd As JavaRDD(Of String) = sc.parallelize(paths)
			sparkNet.fitPaths(pathRdd)

			Dim paramsAfter As INDArray = sparkNet.Network.params().dup()
			assertNotEquals(paramsBefore, paramsAfter)

			Dim stats As SparkTrainingStats = sparkNet.SparkTrainingStats
	'        System.out.println(stats.statsAsString());
			stats.statsAsString()

			sparkNet.TrainingMaster.deleteTempFiles(sc)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Permissions issues on CI") public void testFitViaStringPathsSize1(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFitViaStringPathsSize1(ByVal testDir As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim tempDir As Path = (New File(testDir.toFile(),"DL4J-testFitViaStringPathsSize1")).toPath()
			Dim tempDirF As File = tempDir.toFile()
			tempDirF.deleteOnExit()

			Dim dataSetObjSize As Integer = 1
			Dim batchSizePerExecutor As Integer = 4
			Dim numSplits As Integer = 3
			Dim averagingFrequency As Integer = 3
			Dim totalExamples As Integer = numExecutors() * batchSizePerExecutor * numSplits * averagingFrequency
			Dim iter As DataSetIterator = New MnistDataSetIterator(dataSetObjSize, totalExamples, False)
			Dim i As Integer = 0
			Do While iter.MoveNext()
				Dim nextFile As New File(tempDirF, i & ".bin")
				Dim ds As DataSet = iter.Current
				ds.save(nextFile)
				i += 1
			Loop

	'        System.out.println("Saved to: " + tempDirF.getAbsolutePath());



			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28).nOut(50).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(50).nOut(10).activation(Activation.SOFTMAX).build()).build()

			Dim sparkNet As New SparkDl4jMultiLayer(sc, conf, (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).workerPrefetchNumBatches(5).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(averagingFrequency).repartionData(Repartition.Always).build())
			sparkNet.CollectTrainingStats = True


			'List files:
			Dim config As New Configuration()
			Dim hdfs As FileSystem = FileSystem.get(tempDir.toUri(), config)
			Dim fileIter As RemoteIterator(Of LocatedFileStatus) = hdfs.listFiles(New org.apache.hadoop.fs.Path(tempDir.ToString()), False)

			Dim paths As IList(Of String) = New List(Of String)()
			Do While fileIter.hasNext()
				Dim path As String = fileIter.next().getPath().ToString()
				paths.Add(path)
			Loop

			Dim paramsBefore As INDArray = sparkNet.Network.params().dup()
			Dim pathRdd As JavaRDD(Of String) = sc.parallelize(paths)
			sparkNet.fitPaths(pathRdd)

			Dim paramsAfter As INDArray = sparkNet.Network.params().dup()
			assertNotEquals(paramsBefore, paramsAfter)

			Thread.Sleep(200)
			Dim stats As SparkTrainingStats = sparkNet.SparkTrainingStats

			'Expect
	'        System.out.println(stats.statsAsString());
			stats.statsAsString()
			assertEquals(numSplits, stats.getValue("ParameterAveragingMasterRepartitionTimesMs").Count)

			Dim list As IList(Of EventStats) = stats.getValue("ParameterAveragingWorkerFitTimesMs")
			assertEquals(numSplits * numExecutors() * averagingFrequency, list.Count)
			For Each es As EventStats In list
				Dim e As ExampleCountEventStats = DirectCast(es, ExampleCountEventStats)
				assertTrue(batchSizePerExecutor * averagingFrequency >= e.getTotalExampleCount())
			Next es


			sparkNet.TrainingMaster.deleteTempFiles(sc)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Permissions issues on CI") public void testFitViaStringPathsCompGraph(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testFitViaStringPathsCompGraph(ByVal testDir As Path)
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim tempDir As Path = (New File(testDir.toFile(),"DL4J-testFitViaStringPathsCG")).toPath()
			Dim tempDir2 As Path = (New File(testDir.toFile(),"DL4J-testFitViaStringPathsCG-MDS")).toPath()
			Dim tempDirF As File = tempDir.toFile()
			Dim tempDirF2 As File = tempDir2.toFile()
			tempDirF.deleteOnExit()
			tempDirF2.deleteOnExit()

			Dim dataSetObjSize As Integer = 4
			Dim batchSizePerExecutor As Integer = 8
			Dim iter As DataSetIterator = New MnistDataSetIterator(dataSetObjSize, 128, False)
			Dim i As Integer = 0
			Do While iter.MoveNext()
				Dim nextFile As New File(tempDirF, i & ".bin")
				Dim nextFile2 As New File(tempDirF2, i & ".bin")
				Dim ds As DataSet = iter.Current
				Dim mds As New MultiDataSet(ds.Features, ds.Labels)
				ds.save(nextFile)
				mds.save(nextFile2)
				i += 1
			Loop

	'        System.out.println("Saved to: " + tempDirF.getAbsolutePath());
	'        System.out.println("Saved to: " + tempDirF2.getAbsolutePath());



			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(28 * 28).nOut(50).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(50).nOut(10).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Dim sparkNet As New SparkComputationGraph(sc, conf, (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).workerPrefetchNumBatches(5).workerPrefetchNumBatches(0).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(1).repartionData(Repartition.Always).build())
			sparkNet.CollectTrainingStats = True


			'List files:
			Dim config As New Configuration()
			Dim hdfs As FileSystem = FileSystem.get(tempDir.toUri(), config)
			Dim fileIter As RemoteIterator(Of LocatedFileStatus) = hdfs.listFiles(New org.apache.hadoop.fs.Path(tempDir.ToString()), False)

			Dim paths As IList(Of String) = New List(Of String)()
			Do While fileIter.hasNext()
				Dim path As String = fileIter.next().getPath().ToString()
				paths.Add(path)
			Loop

			Dim paramsBefore As INDArray = sparkNet.Network.params().dup()
			Dim pathRdd As JavaRDD(Of String) = sc.parallelize(paths)
			sparkNet.fitPaths(pathRdd)

			Dim paramsAfter As INDArray = sparkNet.Network.params().dup()
			assertNotEquals(paramsBefore, paramsAfter)

			Dim stats As SparkTrainingStats = sparkNet.SparkTrainingStats
	'        System.out.println(stats.statsAsString());
			stats.statsAsString()

			'Same thing, buf for MultiDataSet objects:
			config = New Configuration()
			hdfs = FileSystem.get(tempDir2.toUri(), config)
			fileIter = hdfs.listFiles(New org.apache.hadoop.fs.Path(tempDir2.ToString()), False)

			paths = New List(Of String)()
			Do While fileIter.hasNext()
				Dim path As String = fileIter.next().getPath().ToString()
				paths.Add(path)
			Loop

			paramsBefore = sparkNet.Network.params().dup()
			pathRdd = sc.parallelize(paths)
			sparkNet.fitPathsMultiDataSet(pathRdd)

			paramsAfter = sparkNet.Network.params().dup()
			assertNotEquals(paramsBefore, paramsAfter)

			stats = sparkNet.SparkTrainingStats
	'        System.out.println(stats.statsAsString());
			stats.statsAsString()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("AB 2019/05/23 - Failing on CI only - passing locally. Possible precision or threading issue") public void testSeedRepeatability() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSeedRepeatability()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(4).nOut(3).activation(Activation.SOFTMAX).build()).build()

			Nd4j.Random.setSeed(12345)
			Dim n1 As New MultiLayerNetwork(conf)
			n1.init()

			Nd4j.Random.setSeed(12345)
			Dim n2 As New MultiLayerNetwork(conf)
			n2.init()

			Nd4j.Random.setSeed(12345)
			Dim n3 As New MultiLayerNetwork(conf)
			n3.init()

			Dim sparkNet1 As New SparkDl4jMultiLayer(sc, n1, (New ParameterAveragingTrainingMaster.Builder(1)).workerPrefetchNumBatches(5).batchSizePerWorker(5).averagingFrequency(1).repartionData(Repartition.Always).rngSeed(12345).build())

			Thread.Sleep(100) 'Training master IDs are only unique if they are created at least 1 ms apart...

			Dim sparkNet2 As New SparkDl4jMultiLayer(sc, n2, (New ParameterAveragingTrainingMaster.Builder(1)).workerPrefetchNumBatches(5).batchSizePerWorker(5).averagingFrequency(1).repartionData(Repartition.Always).rngSeed(12345).build())

			Thread.Sleep(100)

			Dim sparkNet3 As New SparkDl4jMultiLayer(sc, n3, (New ParameterAveragingTrainingMaster.Builder(1)).workerPrefetchNumBatches(5).batchSizePerWorker(5).averagingFrequency(1).repartionData(Repartition.Always).rngSeed(98765).build())

			Dim data As IList(Of DataSet) = New List(Of DataSet)()
			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 150)
			Do While iter.MoveNext()
				data.Add(iter.Current)
			Loop

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(data)


			sparkNet1.fit(rdd)
			sparkNet2.fit(rdd)
			sparkNet3.fit(rdd)


			Dim p1 As INDArray = sparkNet1.Network.params()
			Dim p2 As INDArray = sparkNet2.Network.params()
			Dim p3 As INDArray = sparkNet3.Network.params()

			sparkNet1.TrainingMaster.deleteTempFiles(sc)
			sparkNet2.TrainingMaster.deleteTempFiles(sc)
			sparkNet3.TrainingMaster.deleteTempFiles(sc)

			Dim eq1 As Boolean = p1.equalsWithEps(p2, 0.01)
			Dim eq2 As Boolean = p1.equalsWithEps(p3, 0.01)
			assertTrue(eq1, "Model 1 and 2 params should be equal")
			assertFalse(eq2, "Model 1 and 3 params shoud be different")
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIterationCounts() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIterationCounts()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim dataSetObjSize As Integer = 5
			Dim batchSizePerExecutor As Integer = 25
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			Dim minibatchesPerWorkerPerEpoch As Integer = 10
			Dim iter As DataSetIterator = New MnistDataSetIterator(dataSetObjSize, batchSizePerExecutor * numExecutors() * minibatchesPerWorkerPerEpoch, False)
			Do While iter.MoveNext()
				list.Add(iter.Current)
			Loop

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).nIn(28 * 28).nOut(50).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(50).nOut(10).activation(Activation.SOFTMAX).build()).build()

			For Each avgFreq As Integer In New Integer() {1, 5, 10}
	'            System.out.println("--- Avg freq " + avgFreq + " ---");
				Dim sparkNet As New SparkDl4jMultiLayer(sc, conf.clone(), (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(avgFreq).repartionData(Repartition.Always).build())

				sparkNet.setListeners(New ScoreIterationListener(5))



				Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

				assertEquals(0, sparkNet.Network.LayerWiseConfigurations.getIterationCount())
				sparkNet.fit(rdd)
				assertEquals(minibatchesPerWorkerPerEpoch, sparkNet.Network.LayerWiseConfigurations.getIterationCount())
				sparkNet.fit(rdd)
				assertEquals(2 * minibatchesPerWorkerPerEpoch, sparkNet.Network.LayerWiseConfigurations.getIterationCount())

				sparkNet.TrainingMaster.deleteTempFiles(sc)
			Next avgFreq
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIterationCountsGraph() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIterationCountsGraph()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim dataSetObjSize As Integer = 5
			Dim batchSizePerExecutor As Integer = 25
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			Dim minibatchesPerWorkerPerEpoch As Integer = 10
			Dim iter As DataSetIterator = New MnistDataSetIterator(dataSetObjSize, batchSizePerExecutor * numExecutors() * minibatchesPerWorkerPerEpoch, False)
			Do While iter.MoveNext()
				list.Add(iter.Current)
			Loop

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New RmsProp()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(28 * 28).nOut(50).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(50).nOut(10).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			For Each avgFreq As Integer In New Integer() {1, 5, 10}
	'            System.out.println("--- Avg freq " + avgFreq + " ---");
				Dim sparkNet As New SparkComputationGraph(sc, conf.clone(), (New ParameterAveragingTrainingMaster.Builder(numExecutors(), dataSetObjSize)).batchSizePerWorker(batchSizePerExecutor).averagingFrequency(avgFreq).repartionData(Repartition.Always).build())

				sparkNet.setListeners(New ScoreIterationListener(5))

				Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(list)

				assertEquals(0, sparkNet.Network.Configuration.getIterationCount())
				sparkNet.fit(rdd)
				assertEquals(minibatchesPerWorkerPerEpoch, sparkNet.Network.Configuration.getIterationCount())
				sparkNet.fit(rdd)
				assertEquals(2 * minibatchesPerWorkerPerEpoch, sparkNet.Network.Configuration.getIterationCount())

				sparkNet.TrainingMaster.deleteTempFiles(sc)
			Next avgFreq
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testVaePretrainSimple()
		Public Overridable Sub testVaePretrainSimple()
			'Simple sanity check on pretraining
			Dim nIn As Integer = 8

			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New RmsProp()).weightInit(WeightInit.XAVIER).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(8).nOut(10).encoderLayerSizes(12).decoderLayerSizes(13).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.IDENTITY)).build()).build()

			'Do training on Spark with one executor, for 3 separate minibatches
			Dim rddDataSetNumExamples As Integer = 10
			Dim totalAveragings As Integer = 5
			Dim averagingFrequency As Integer = 3
			Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(rddDataSetNumExamples)).averagingFrequency(averagingFrequency).batchSizePerWorker(rddDataSetNumExamples).saveUpdater(True).workerPrefetchNumBatches(0).build()
			Nd4j.Random.setSeed(12345)
			Dim sparkNet As New SparkDl4jMultiLayer(sc, conf.clone(), tm)

			Dim trainData As IList(Of DataSet) = New List(Of DataSet)()
			Dim nDataSets As Integer = numExecutors() * totalAveragings * averagingFrequency
			For i As Integer = 0 To nDataSets - 1
				trainData.Add(New DataSet(Nd4j.rand(rddDataSetNumExamples, nIn), Nothing))
			Next i

			Dim data As JavaRDD(Of DataSet) = sc.parallelize(trainData)

			sparkNet.fit(data)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testVaePretrainSimpleCG()
		Public Overridable Sub testVaePretrainSimpleCG()
			'Simple sanity check on pretraining
			Dim nIn As Integer = 8

			Nd4j.Random.setSeed(12345)
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New RmsProp()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New VariationalAutoencoder.Builder()).nIn(8).nOut(10).encoderLayerSizes(12).decoderLayerSizes(13).reconstructionDistribution(New GaussianReconstructionDistribution(Activation.IDENTITY)).build(), "in").setOutputs("0").build()

			'Do training on Spark with one executor, for 3 separate minibatches
			Dim rddDataSetNumExamples As Integer = 10
			Dim totalAveragings As Integer = 5
			Dim averagingFrequency As Integer = 3
			Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(rddDataSetNumExamples)).averagingFrequency(averagingFrequency).batchSizePerWorker(rddDataSetNumExamples).saveUpdater(True).workerPrefetchNumBatches(0).build()
			Nd4j.Random.setSeed(12345)
			Dim sparkNet As New SparkComputationGraph(sc, conf.clone(), tm)

			Dim trainData As IList(Of DataSet) = New List(Of DataSet)()
			Dim nDataSets As Integer = numExecutors() * totalAveragings * averagingFrequency
			For i As Integer = 0 To nDataSets - 1
				trainData.Add(New DataSet(Nd4j.rand(rddDataSetNumExamples, nIn), Nothing))
			Next i

			Dim data As JavaRDD(Of DataSet) = sc.parallelize(trainData)

			sparkNet.fit(data)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testROC()
		Public Overridable Sub testROC()

			Dim nArrays As Integer = 100
			Dim minibatch As Integer = 64
			Dim steps As Integer = 20
			Dim nIn As Integer = 5
			Dim nOut As Integer = 2
			Dim layerSize As Integer = 10

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(layerSize).build()).layer(1, (New OutputLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()


			Nd4j.Random.setSeed(12345)
			Dim r As New Random(12345)

			Dim local As New ROC(steps)
			Dim dsList As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To nArrays - 1
				Dim features As INDArray = Nd4j.rand(minibatch, nIn)

				Dim p As INDArray = net.output(features)

				Dim l As INDArray = Nd4j.zeros(minibatch, 2)
				For j As Integer = 0 To minibatch - 1
					l.putScalar(j, r.Next(2), 1.0)
				Next j

				local.eval(l, p)

				dsList.Add(New DataSet(features, l))
			Next i


			Dim sparkNet As New SparkDl4jMultiLayer(sc, net, Nothing)
			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(dsList)

			Dim sparkROC As ROC = sparkNet.evaluateROC(rdd, steps, 32)

			assertEquals(sparkROC.calculateAUC(), sparkROC.calculateAUC(), 1e-6)

			assertEquals(local.RocCurve, sparkROC.RocCurve)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testROCMultiClass()
		Public Overridable Sub testROCMultiClass()

			Dim nArrays As Integer = 100
			Dim minibatch As Integer = 64
			Dim steps As Integer = 20
			Dim nIn As Integer = 5
			Dim nOut As Integer = 3
			Dim layerSize As Integer = 10

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(layerSize).build()).layer(1, (New OutputLayer.Builder()).nIn(layerSize).nOut(nOut).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()


			Nd4j.Random.setSeed(12345)
			Dim r As New Random(12345)

			Dim local As New ROCMultiClass(steps)
			Dim dsList As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To nArrays - 1
				Dim features As INDArray = Nd4j.rand(minibatch, nIn)

				Dim p As INDArray = net.output(features)

				Dim l As INDArray = Nd4j.zeros(minibatch, nOut)
				For j As Integer = 0 To minibatch - 1
					l.putScalar(j, r.Next(nOut), 1.0)
				Next j

				local.eval(l, p)

				dsList.Add(New DataSet(features, l))
			Next i


			Dim sparkNet As New SparkDl4jMultiLayer(sc, net, Nothing)
			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(dsList)

			Dim sparkROC As ROCMultiClass = sparkNet.evaluateROCMultiClass(rdd, steps, 32)

			For i As Integer = 0 To nOut - 1
				assertEquals(sparkROC.calculateAUC(i), sparkROC.calculateAUC(i), 1e-6)

				assertEquals(local.getRocCurve(i), sparkROC.getRocCurve(i))
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(120000) public void testEpochCounter() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEpochCounter()
			If Platform.isWindows() Then
				'Spark tests don't run on windows
				Return
			End If
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).build()).build()

			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("out", (New OutputLayer.Builder()).nIn(4).nOut(3).build(), "in").setOutputs("out").build()

			Dim iter As DataSetIterator = New IrisDataSetIterator(1, 50)

			Dim l As IList(Of DataSet) = New List(Of DataSet)()
			Do While iter.MoveNext()
				l.Add(iter.Current)
			Loop

			Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(l)


			Dim rddDataSetNumExamples As Integer = 1
			Dim averagingFrequency As Integer = 2
			Dim batch As Integer = 2
			Dim tm As ParameterAveragingTrainingMaster = (New ParameterAveragingTrainingMaster.Builder(rddDataSetNumExamples)).averagingFrequency(averagingFrequency).batchSizePerWorker(batch).saveUpdater(True).workerPrefetchNumBatches(0).build()
			Nd4j.Random.setSeed(12345)


			Dim sn1 As New SparkDl4jMultiLayer(sc, conf.clone(), tm)
			Dim sn2 As New SparkComputationGraph(sc, conf2.clone(), tm)


			For i As Integer = 0 To 2
				assertEquals(i, sn1.Network.LayerWiseConfigurations.EpochCount)
				assertEquals(i, sn2.Network.Configuration.getEpochCount())
				sn1.fit(rdd)
				sn2.fit(rdd)
				assertEquals(i+1, sn1.Network.LayerWiseConfigurations.EpochCount)
				assertEquals(i+1, sn2.Network.Configuration.getEpochCount())
			Next i
		End Sub
	End Class

End Namespace