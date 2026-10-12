Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports val = lombok.val
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports RecordReaderMultiDataSetIterator = org.deeplearning4j.datasets.datavec.RecordReaderMultiDataSetIterator
Imports IteratorMultiDataSetIterator = org.deeplearning4j.datasets.iterator.IteratorMultiDataSetIterator
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports org.deeplearning4j.datasets.iterator.impl
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Updater = org.deeplearning4j.nn.conf.Updater
Imports ElementWiseVertex = org.deeplearning4j.nn.conf.graph.ElementWiseVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports BaseSparkTest = org.deeplearning4j.spark.BaseSparkTest
Imports RDDTrainingApproach = org.deeplearning4j.spark.api.RDDTrainingApproach
Imports Repartition = org.deeplearning4j.spark.api.Repartition
Imports org.deeplearning4j.spark.api
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
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

Namespace org.deeplearning4j.spark.impl.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("AB 2019/05/24 - Rarely getting stuck on CI - see issue #7657") @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestSparkComputationGraph extends org.deeplearning4j.spark.BaseSparkTest
	<Serializable>
	Public Class TestSparkComputationGraph
		Inherits BaseSparkTest

		Public Shared ReadOnly Property BasicNetIris2Class As ComputationGraph
			Get
    
				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("l0", (New DenseLayer.Builder()).nIn(4).nOut(10).build(), "in").addLayer("l1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(10).nOut(2).build(), "l0").setOutputs("l1").build()
    
				Dim cg As New ComputationGraph(conf)
				cg.init()
    
				Return cg
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasic()

			Dim sc As JavaSparkContext = Me.sc

			Dim rr As RecordReader = New CSVRecordReader(0, ","c)
			rr.initialize(New org.datavec.api.Split.FileSplit((New ClassPathResource("iris.txt")).TempFileFromArchive))
			Dim iter As MultiDataSetIterator = (New RecordReaderMultiDataSetIterator.Builder(1)).addReader("iris", rr).addInput("iris", 0, 3).addOutputOneHot("iris", 4, 3).build()

			Dim list As IList(Of MultiDataSet) = New List(Of MultiDataSet)(150)
			Do While iter.MoveNext()
				list.Add(iter.Current)
			Loop

			Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Sgd(0.1)).graphBuilder().addInputs("in").addLayer("dense", (New DenseLayer.Builder()).nIn(4).nOut(2).build(), "in").addLayer("out", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(2).nOut(3).build(), "dense").setOutputs("out").build()

			Dim cg As New ComputationGraph(config)
			cg.init()

			Dim tm As TrainingMaster = New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0)

			Dim scg As New SparkComputationGraph(sc, cg, tm)
			scg.setListeners(Collections.singleton(DirectCast(New ScoreIterationListener(5), TrainingListener)))

			Dim rdd As JavaRDD(Of MultiDataSet) = sc.parallelize(list)
			scg.fitMultiDataSet(rdd)

			'Try: fitting using DataSet
			Dim iris As DataSetIterator = New IrisDataSetIterator(1, 150)
			Dim list2 As IList(Of DataSet) = New List(Of DataSet)()
			Do While iris.MoveNext()
				list2.Add(iris.Current)
			Loop
			Dim rddDS As JavaRDD(Of DataSet) = sc.parallelize(list2)

			scg.fit(rddDS)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDistributedScoring()
		Public Overridable Sub testDistributedScoring()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).l1(0.1).l2(0.1).seed(123).updater(New Nesterovs(0.1, 0.9)).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(nIn).nOut(3).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(nOut).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Dim tm As TrainingMaster = New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0)

			Dim sparkNet As New SparkComputationGraph(sc, conf, tm)
			Dim netCopy As ComputationGraph = sparkNet.Network.clone()

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

				'            System.out.println(localScoresWithRegDouble[i] + "\t" + scoresWithReg.get(i) + "\t" + localScoresNoRegDouble[i] + "\t" + scoresNoReg.get(i));
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("AB 2019/05/23 - Failing on CI only - passing locally. Possible precision or threading issue") public void testSeedRepeatability() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSeedRepeatability()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(Updater.RMSPROP).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).weightInit(WeightInit.XAVIER).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(4).nOut(3).activation(Activation.SOFTMAX).build(), "0").setOutputs("1").build()

			Nd4j.Random.setSeed(12345)
			Dim n1 As New ComputationGraph(conf.clone())
			n1.init()

			Nd4j.Random.setSeed(12345)
			Dim n2 As New ComputationGraph(conf.clone())
			n2.init()

			Nd4j.Random.setSeed(12345)
			Dim n3 As New ComputationGraph(conf.clone())
			n3.init()

			Dim sparkNet1 As New SparkComputationGraph(sc, n1, (New ParameterAveragingTrainingMaster.Builder(1)).workerPrefetchNumBatches(5).batchSizePerWorker(5).averagingFrequency(1).repartionData(Repartition.Always).rngSeed(12345).build())

			Thread.Sleep(100) 'Training master IDs are only unique if they are created at least 1 ms apart...

			Dim sparkNet2 As New SparkComputationGraph(sc, n2, (New ParameterAveragingTrainingMaster.Builder(1)).workerPrefetchNumBatches(5).batchSizePerWorker(5).averagingFrequency(1).repartionData(Repartition.Always).rngSeed(12345).build())

			Thread.Sleep(100)

			Dim sparkNet3 As New SparkComputationGraph(sc, n3, (New ParameterAveragingTrainingMaster.Builder(1)).workerPrefetchNumBatches(5).batchSizePerWorker(5).averagingFrequency(1).repartionData(Repartition.Always).rngSeed(98765).build())

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
'ORIGINAL LINE: @Test() @Timeout(60000L) public void testEvaluationAndRoc()
		Public Overridable Sub testEvaluationAndRoc()
			For Each evalWorkers As Integer In New Integer(){1, 4, 8}
				Dim iter As DataSetIterator = New IrisDataSetIterator(5, 150)

				'Make a 2-class version of iris:
				Dim l As IList(Of DataSet) = New List(Of DataSet)()
				iter.reset()
				Do While iter.MoveNext()
					Dim ds As DataSet = iter.Current
					Dim newL As INDArray = Nd4j.create(ds.Labels.size(0), 2)
					newL.putColumn(0, ds.Labels.getColumn(0))
					newL.putColumn(1, ds.Labels.getColumn(1))
					newL.getColumn(1).addi(ds.Labels.getColumn(2))
					ds.Labels = newL
					l.Add(ds)
				Loop

				iter = New ListDataSetIterator(Of )(l)

				Dim cg As ComputationGraph = BasicNetIris2Class

				Dim e As Evaluation = cg.evaluate(iter)
				Dim roc As ROC = cg.evaluateROC(iter, 32)


				Dim scg As New SparkComputationGraph(sc, cg, Nothing)
				scg.DefaultEvaluationWorkers = evalWorkers


				Dim rdd As JavaRDD(Of DataSet) = sc.parallelize(l)
				rdd = rdd.repartition(20)

				Dim e2 As Evaluation = scg.evaluate(rdd)
				Dim roc2 As ROC = scg.evaluateROC(rdd)


				assertEquals(e2.accuracy(), e.accuracy(), 1e-3)
				assertEquals(e2.f1(), e.f1(), 1e-3)
				assertEquals(e2.NumRowCounter, e.NumRowCounter, 1e-3)
				assertEquals(e2.falseNegatives(), e.falseNegatives())
				assertEquals(e2.falsePositives(), e.falsePositives())
				assertEquals(e2.trueNegatives(), e.trueNegatives())
				assertEquals(e2.truePositives(), e.truePositives())
				assertEquals(e2.precision(), e.precision(), 1e-3)
				assertEquals(e2.recall(), e.recall(), 1e-3)
				assertEquals(e2.getConfusionMatrix(), e.getConfusionMatrix())

				assertEquals(roc.calculateAUC(), roc2.calculateAUC(), 1e-5)
				assertEquals(roc.calculateAUCPR(), roc2.calculateAUCPR(), 1e-5)
			Next evalWorkers
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEvaluationAndRocMDS()
		Public Overridable Sub testEvaluationAndRocMDS()
			For Each evalWorkers As Integer In New Integer(){1, 4, 8}

				Dim iter As DataSetIterator = New IrisDataSetIterator(5, 150)

				'Make a 2-class version of iris:
				Dim l As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
				iter.reset()
				Do While iter.MoveNext()
					Dim ds As DataSet = iter.Current
					Dim newL As INDArray = Nd4j.create(ds.Labels.size(0), 2)
					newL.putColumn(0, ds.Labels.getColumn(0))
					newL.putColumn(1, ds.Labels.getColumn(1))
					newL.getColumn(1).addi(ds.Labels.getColumn(2))

					Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(ds.Features, newL)
					l.Add(mds)
				Loop

				Dim mdsIter As MultiDataSetIterator = New IteratorMultiDataSetIterator(l.GetEnumerator(), 5)

				Dim cg As ComputationGraph = BasicNetIris2Class

				Dim es() As IEvaluation = cg.doEvaluation(mdsIter, New Evaluation(), New ROC(32))
				Dim e As Evaluation = CType(es(0), Evaluation)
				Dim roc As ROC = CType(es(1), ROC)


				Dim scg As New SparkComputationGraph(sc, cg, Nothing)
				scg.DefaultEvaluationWorkers = evalWorkers

				Dim rdd As JavaRDD(Of MultiDataSet) = sc.parallelize(l)
				rdd = rdd.repartition(20)

				Dim es2() As IEvaluation = scg.doEvaluationMDS(rdd, 5, New Evaluation(), New ROC(32))
				Dim e2 As Evaluation = CType(es2(0), Evaluation)
				Dim roc2 As ROC = CType(es2(1), ROC)


				assertEquals(e2.accuracy(), e.accuracy(), 1e-3)
				assertEquals(e2.f1(), e.f1(), 1e-3)
				assertEquals(e2.NumRowCounter, e.NumRowCounter, 1e-3)
				assertEquals(e2.falseNegatives(), e.falseNegatives())
				assertEquals(e2.falsePositives(), e.falsePositives())
				assertEquals(e2.trueNegatives(), e.trueNegatives())
				assertEquals(e2.truePositives(), e.truePositives())
				assertEquals(e2.precision(), e.precision(), 1e-3)
				assertEquals(e2.recall(), e.recall(), 1e-3)
				assertEquals(e2.getConfusionMatrix(), e.getConfusionMatrix())

				assertEquals(roc.calculateAUC(), roc2.calculateAUC(), 1e-5)
				assertEquals(roc.calculateAUCPR(), roc2.calculateAUCPR(), 1e-5)
			Next evalWorkers
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIssue7068() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testIssue7068()

			Dim batchSize As val = 5
			Dim featSize As val = 10
			Dim labelSize As val = 2
			Dim random As val = New Random(0)

			Dim l As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
			For i As Integer = 0 To 9
				Dim mds As New org.nd4j.linalg.dataset.MultiDataSet(New INDArray(){Nd4j.rand(batchSize, featSize).castTo(DataType.DOUBLE), Nd4j.rand(batchSize, featSize).castTo(DataType.DOUBLE)}, New INDArray(){Nd4j.rand(batchSize, labelSize).castTo(DataType.DOUBLE)})
				l.Add(mds)
			Next i
			Dim rdd As JavaRDD(Of MultiDataSet) = sc.parallelize(l)

			' simple model
			Dim modelConf As val = (New NeuralNetConfiguration.Builder()).updater(New Adam(0.01)).weightInit(WeightInit.XAVIER_UNIFORM).biasInit(0).graphBuilder().addInputs("input1", "input2").addVertex("avg",New ElementWiseVertex(ElementWiseVertex.Op.Average),"input1","input2").addLayer("dense",(New DenseLayer.Builder()).dropOut(0.9).nIn(featSize).nOut(featSize / 2).build(),"avg").addLayer("output",(New OutputLayer.Builder()).nIn(featSize / 2).nOut(2).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).hasBias(False).build(),"dense").setOutputs("output").build()

			Dim model As val = New ComputationGraph(modelConf)
			model.init()

			Dim trainingMaster As val = (New ParameterAveragingTrainingMaster.Builder(batchSize)).rddTrainingApproach(RDDTrainingApproach.Direct).build()
			Dim sparkModel As val = New SparkComputationGraph(sc, model, trainingMaster)

			For i As Integer = 0 To 2
				sparkModel.fitMultiDataSet(rdd)
			Next i
		End Sub
	End Class

End Namespace