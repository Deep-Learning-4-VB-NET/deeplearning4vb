Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaDoubleRDD = org.apache.spark.api.java.JavaDoubleRDD
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports RDD = org.apache.spark.rdd.RDD
Imports BroadcastHadoopConfigHolder = org.datavec.spark.util.BroadcastHadoopConfigHolder
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports SerializedDataSetLoader = org.deeplearning4j.core.loader.impl.SerializedDataSetLoader
Imports SerializedMultiDataSetLoader = org.deeplearning4j.core.loader.impl.SerializedMultiDataSetLoader
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.deeplearning4j.spark.api
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports SparkListenable = org.deeplearning4j.spark.impl.SparkListenable
Imports LongDoubleReduceFunction = org.deeplearning4j.spark.impl.common.reduce.LongDoubleReduceFunction
Imports DataSetToMultiDataSetFn = org.deeplearning4j.spark.impl.graph.dataset.DataSetToMultiDataSetFn
Imports org.deeplearning4j.spark.impl.graph.dataset
Imports org.deeplearning4j.spark.impl.graph.evaluation
Imports IEvaluateMDSPathsFlatMapFunction = org.deeplearning4j.spark.impl.graph.evaluation.IEvaluateMDSPathsFlatMapFunction
Imports org.deeplearning4j.spark.impl.graph.scoring
Imports org.deeplearning4j.spark.impl.multilayer.evaluation
Imports org.deeplearning4j.spark.impl.multilayer.evaluation
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Heartbeat = org.nd4j.linalg.heartbeat.Heartbeat
Imports Environment = org.nd4j.linalg.heartbeat.reports.Environment
Imports [Event] = org.nd4j.linalg.heartbeat.reports.Event
Imports Task = org.nd4j.linalg.heartbeat.reports.Task
Imports EnvironmentUtils = org.nd4j.linalg.heartbeat.utils.EnvironmentUtils
Imports Tuple2 = scala.Tuple2

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
'ORIGINAL LINE: @Slf4j public class SparkComputationGraph extends org.deeplearning4j.spark.impl.SparkListenable
	Public Class SparkComputationGraph
		Inherits SparkListenable

		Public Const DEFAULT_ROC_THRESHOLD_STEPS As Integer = 32
		Public Const DEFAULT_EVAL_SCORE_BATCH_SIZE As Integer = 64
		Public Const DEFAULT_EVAL_WORKERS As Integer = 4
		<NonSerialized>
		Private sc As JavaSparkContext
		Private conf As ComputationGraphConfiguration
'JAVA TO VB CONVERTER NOTE: The field network was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private network_Conflict As ComputationGraph
		Private lastScore As Double
'JAVA TO VB CONVERTER NOTE: The field defaultEvaluationWorkers was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private defaultEvaluationWorkers_Conflict As Integer = DEFAULT_EVAL_WORKERS

		<NonSerialized>
		Private iterationsCount As New AtomicInteger(0)

		''' <summary>
		''' Instantiate a ComputationGraph instance with the given context, network and training master.
		''' </summary>
		''' <param name="sparkContext">   the spark context to use </param>
		''' <param name="network">        the network to use </param>
		''' <param name="trainingMaster"> Required for training. May be null if the SparkComputationGraph is only to be used
		'''                       for evaluation or inference </param>
		Public Sub New(ByVal sparkContext As SparkContext, ByVal network As ComputationGraph, ByVal trainingMaster As TrainingMaster)
			Me.New(New JavaSparkContext(sparkContext), network, trainingMaster)
		End Sub

		Public Sub New(ByVal javaSparkContext As JavaSparkContext, ByVal network As ComputationGraph, ByVal trainingMaster As TrainingMaster)
			sc = javaSparkContext
			Me.trainingMaster = trainingMaster
			Me.conf = network.Configuration.clone()
			Me.network_Conflict = network
			Me.network_Conflict.init()

			'Check if kryo configuration is correct:
			SparkUtils.checkKryoConfiguration(javaSparkContext, log)
		End Sub


		Public Sub New(ByVal sparkContext As SparkContext, ByVal conf As ComputationGraphConfiguration, ByVal trainingMaster As TrainingMaster)
			Me.New(New JavaSparkContext(sparkContext), conf, trainingMaster)
		End Sub

		Public Sub New(ByVal sparkContext As JavaSparkContext, ByVal conf As ComputationGraphConfiguration, ByVal trainingMaster As TrainingMaster)
			sc = sparkContext
			Me.trainingMaster = trainingMaster
			Me.conf = conf.clone()
			Me.network_Conflict = New ComputationGraph(conf)
			Me.network_Conflict.init()

			'Check if kryo configuration is correct:
			SparkUtils.checkKryoConfiguration(sparkContext, log)
		End Sub

		Public Overridable ReadOnly Property SparkContext As JavaSparkContext
			Get
				Return sc
			End Get
		End Property

		Public Overridable WriteOnly Property CollectTrainingStats As Boolean
			Set(ByVal collectTrainingStats As Boolean)
				trainingMaster.setCollectTrainingStats(collectTrainingStats)
			End Set
		End Property

		Public Overridable ReadOnly Property SparkTrainingStats As SparkTrainingStats
			Get
				Return trainingMaster.getTrainingStats()
			End Get
		End Property

		''' <returns> The trained ComputationGraph </returns>
		Public Overridable Property Network As ComputationGraph
			Get
				Return network_Conflict
			End Get
			Set(ByVal network As ComputationGraph)
				Me.network_Conflict = network
			End Set
		End Property

		''' <returns> The TrainingMaster for this network </returns>
		Public Overridable ReadOnly Property TrainingMaster As TrainingMaster
			Get
				Return trainingMaster
			End Get
		End Property


		''' <summary>
		''' Returns the currently set default number of evaluation workers/threads.
		''' Note that when the number of workers is provided explicitly in an evaluation method, the default value
		''' is not used.<br>
		''' In many cases, we may want this to be smaller than the number of Spark threads, to reduce memory requirements.
		''' For example, with 32 Spark threads and a large network, we don't want to spin up 32 instances of the network
		''' to perform evaluation. Better (for memory requirements, and reduced cache thrashing) to use say 4 workers.<br>
		''' If it is not set explicitly, <seealso cref="DEFAULT_EVAL_WORKERS"/> will be used
		''' </summary>
		''' <returns> Default number of evaluation workers (threads). </returns>
		Public Overridable Property DefaultEvaluationWorkers As Integer
			Get
				Return defaultEvaluationWorkers_Conflict
			End Get
			Set(ByVal workers As Integer)
				Preconditions.checkArgument(workers > 0, "Number of workers must be > 0: got %s", workers)
				Me.defaultEvaluationWorkers_Conflict = workers
			End Set
		End Property


		''' <summary>
		''' Fit the ComputationGraph with the given data set
		''' </summary>
		''' <param name="rdd"> Data to train on </param>
		''' <returns> Trained network </returns>
		Public Overridable Function fit(ByVal rdd As RDD(Of DataSet)) As ComputationGraph
			Return fit(rdd.toJavaRDD())
		End Function

		''' <summary>
		''' Fit the ComputationGraph with the given data set
		''' </summary>
		''' <param name="rdd"> Data to train on </param>
		''' <returns> Trained network </returns>
		Public Overridable Function fit(ByVal rdd As JavaRDD(Of DataSet)) As ComputationGraph
			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			trainingMaster.executeTraining(Me, rdd)
			network_Conflict.incrementEpochCount()
			Return network_Conflict
		End Function

		''' <summary>
		''' Fit the SparkComputationGraph network using a directory of serialized DataSet objects
		''' The assumption here is that the directory contains a number of <seealso cref="DataSet"/> objects, each serialized using
		''' <seealso cref="DataSet.save(OutputStream)"/>
		''' </summary>
		''' <param name="path"> Path to the directory containing the serialized DataSet objcets </param>
		''' <returns> The MultiLayerNetwork after training </returns>
		Public Overridable Function fit(ByVal path As String) As ComputationGraph
			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			Dim paths As JavaRDD(Of String)
			Try
				paths = SparkUtils.listPaths(sc, path)
			Catch e As IOException
				Throw New Exception("Error listing paths in directory", e)
			End Try

			Return fitPaths(paths)
		End Function

		''' @deprecated Use <seealso cref="fit(String)"/> 
		<Obsolete("Use <seealso cref=""fit(String)""/>")>
		Public Overridable Function fit(ByVal path As String, ByVal minPartitions As Integer) As ComputationGraph
			Return fit(path)
		End Function

		''' <summary>
		''' Fit the network using a list of paths for serialized DataSet objects.
		''' </summary>
		''' <param name="paths">    List of paths </param>
		''' <returns> trained network </returns>
		Public Overridable Function fitPaths(ByVal paths As JavaRDD(Of String)) As ComputationGraph
			Return fitPaths(paths, New SerializedDataSetLoader())
		End Function

		Public Overridable Function fitPaths(ByVal paths As JavaRDD(Of String), ByVal loader As DataSetLoader) As ComputationGraph
			trainingMaster.executeTrainingPaths(Nothing,Me, paths, loader, Nothing)
			network_Conflict.incrementEpochCount()
			Return network_Conflict
		End Function

		''' <summary>
		''' Fit the ComputationGraph with the given data set
		''' </summary>
		''' <param name="rdd"> Data to train on </param>
		''' <returns> Trained network </returns>
		Public Overridable Function fitMultiDataSet(ByVal rdd As RDD(Of MultiDataSet)) As ComputationGraph
			Return fitMultiDataSet(rdd.toJavaRDD())
		End Function

		''' <summary>
		''' Fit the ComputationGraph with the given data set
		''' </summary>
		''' <param name="rdd"> Data to train on </param>
		''' <returns> Trained network </returns>
		Public Overridable Function fitMultiDataSet(ByVal rdd As JavaRDD(Of MultiDataSet)) As ComputationGraph
			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			trainingMaster.executeTrainingMDS(Me, rdd)
			network_Conflict.incrementEpochCount()
			Return network_Conflict
		End Function

		''' <summary>
		''' Fit the SparkComputationGraph network using a directory of serialized MultiDataSet objects
		''' The assumption here is that the directory contains a number of serialized <seealso cref="MultiDataSet"/> objects
		''' </summary>
		''' <param name="path"> Path to the directory containing the serialized MultiDataSet objcets </param>
		''' <returns> The MultiLayerNetwork after training </returns>
		Public Overridable Function fitMultiDataSet(ByVal path As String) As ComputationGraph
			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			Dim paths As JavaRDD(Of String)
			Try
				paths = SparkUtils.listPaths(sc, path)
			Catch e As IOException
				Throw New Exception("Error listing paths in directory", e)
			End Try

			Return fitPathsMultiDataSet(paths)
		End Function

		''' <summary>
		''' Fit the network using a list of paths for serialized MultiDataSet objects.
		''' </summary>
		''' <param name="paths">    List of paths </param>
		''' <returns> trained network </returns>
		Public Overridable Function fitPathsMultiDataSet(ByVal paths As JavaRDD(Of String)) As ComputationGraph
			Return fitPaths(paths, New SerializedMultiDataSetLoader())
		End Function

		Public Overridable Function fitPaths(ByVal paths As JavaRDD(Of String), ByVal loader As MultiDataSetLoader) As ComputationGraph
			trainingMaster.executeTrainingPaths(Nothing, Me, paths, Nothing, loader)
			network_Conflict.incrementEpochCount()
			Return network_Conflict
		End Function

		''' @deprecated use <seealso cref="fitMultiDataSet(String)"/> 
		<Obsolete("use <seealso cref=""fitMultiDataSet(String)""/>")>
		Public Overridable Function fitMultiDataSet(ByVal path As String, ByVal minPartitions As Integer) As ComputationGraph
			Return fitMultiDataSet(path)
		End Function

		''' <summary>
		''' Gets the last (average) minibatch score from calling fit. This is the average score across all executors for the
		''' last minibatch executed in each worker
		''' </summary>
		Public Overridable Property Score As Double
			Get
				Return lastScore
			End Get
			Set(ByVal lastScore As Double)
				Me.lastScore = lastScore
			End Set
		End Property


		''' <summary>
		''' Calculate the score for all examples in the provided {@code JavaRDD<DataSet>}, either by summing
		''' or averaging over the entire data set. To calculate a score for each example individually, use <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/>
		''' or one of the similar methods. Uses default minibatch size in each worker, <seealso cref="SparkComputationGraph.DEFAULT_EVAL_SCORE_BATCH_SIZE"/>
		''' </summary>
		''' <param name="data">    Data to score </param>
		''' <param name="average"> Whether to sum the scores, or average them </param>
		Public Overridable Function calculateScore(ByVal data As JavaRDD(Of DataSet), ByVal average As Boolean) As Double
			Return calculateScore(data, average, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Calculate the score for all examples in the provided {@code JavaRDD<DataSet>}, either by summing
		''' or averaging over the entire data set. To calculate a score for each example individually, use <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/>
		''' or one of the similar methods
		''' </summary>
		''' <param name="data">          Data to score </param>
		''' <param name="average">       Whether to sum the scores, or average them </param>
		''' <param name="minibatchSize"> The number of examples to use in each minibatch when scoring. If more examples are in a partition than
		'''                      this, multiple scoring operations will be done (to avoid using too much memory by doing the whole partition
		'''                      in one go) </param>
		Public Overridable Function calculateScore(ByVal data As JavaRDD(Of DataSet), ByVal average As Boolean, ByVal minibatchSize As Integer) As Double
			Dim rdd As JavaRDD(Of Tuple2(Of Long, Double)) = data.mapPartitions(New ScoreFlatMapFunctionCGDataSet(conf.toJson(), sc.broadcast(network_Conflict.params()), minibatchSize))

			'Reduce to a single tuple, with example count + sum of scores
			Dim countAndSumScores As Tuple2(Of Long, Double) = rdd.reduce(New LongDoubleReduceFunction())
			If average Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return countAndSumScores._2() / countAndSumScores._1()
			Else
				Return countAndSumScores._2()
			End If
		End Function

		''' <summary>
		''' Calculate the score for all examples in the provided {@code JavaRDD<MultiDataSet>}, either by summing
		''' or averaging over the entire data set.
		''' Uses default minibatch size in each worker, <seealso cref="SparkComputationGraph.DEFAULT_EVAL_SCORE_BATCH_SIZE"/>
		''' </summary>
		''' <param name="data">    Data to score </param>
		''' <param name="average"> Whether to sum the scores, or average them </param>
		Public Overridable Function calculateScoreMultiDataSet(ByVal data As JavaRDD(Of MultiDataSet), ByVal average As Boolean) As Double
			Return calculateScoreMultiDataSet(data, average, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Calculate the score for all examples in the provided {@code JavaRDD<MultiDataSet>}, either by summing
		''' or averaging over the entire data set.
		'''      * </summary>
		''' <param name="data">          Data to score </param>
		''' <param name="average">       Whether to sum the scores, or average them </param>
		''' <param name="minibatchSize"> The number of examples to use in each minibatch when scoring. If more examples are in a partition than
		'''                      this, multiple scoring operations will be done (to avoid using too much memory by doing the whole partition
		'''                      in one go) </param>
		Public Overridable Function calculateScoreMultiDataSet(ByVal data As JavaRDD(Of MultiDataSet), ByVal average As Boolean, ByVal minibatchSize As Integer) As Double
			Dim rdd As JavaRDD(Of Tuple2(Of Long, Double)) = data.mapPartitions(New ScoreFlatMapFunctionCGMultiDataSet(conf.toJson(), sc.broadcast(network_Conflict.params()), minibatchSize))
			'Reduce to a single tuple, with example count + sum of scores
			Dim countAndSumScores As Tuple2(Of Long, Double) = rdd.reduce(New LongDoubleReduceFunction())
			If average Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return countAndSumScores._2() / countAndSumScores._1()
			Else
				Return countAndSumScores._2()
			End If
		End Function

		''' <summary>
		''' DataSet version of <seealso cref="scoreExamples(JavaRDD, Boolean)"/>
		''' </summary>
		Public Overridable Function scoreExamples(ByVal data As JavaRDD(Of DataSet), ByVal includeRegularizationTerms As Boolean) As JavaDoubleRDD
			Return scoreExamplesMultiDataSet(data.map(New DataSetToMultiDataSetFn()), includeRegularizationTerms)
		End Function

		''' <summary>
		''' DataSet version of <seealso cref="scoreExamples(JavaPairRDD, Boolean, Integer)"/>
		''' </summary>
		Public Overridable Function scoreExamples(ByVal data As JavaRDD(Of DataSet), ByVal includeRegularizationTerms As Boolean, ByVal batchSize As Integer) As JavaDoubleRDD
			Return scoreExamplesMultiDataSet(data.map(New DataSetToMultiDataSetFn()), includeRegularizationTerms, batchSize)
		End Function

		''' <summary>
		''' DataSet version of <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/>
		''' </summary>
		Public Overridable Function scoreExamples(Of K)(ByVal data As JavaPairRDD(Of K, DataSet), ByVal includeRegularizationTerms As Boolean) As JavaPairRDD(Of K, Double)
			Return scoreExamplesMultiDataSet(data.mapToPair(New PairDataSetToMultiDataSetFn(Of K)()), includeRegularizationTerms, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' DataSet version of <seealso cref="scoreExamples(JavaPairRDD, Boolean, Integer)"/>
		''' </summary>
		Public Overridable Function scoreExamples(Of K)(ByVal data As JavaPairRDD(Of K, DataSet), ByVal includeRegularizationTerms As Boolean, ByVal batchSize As Integer) As JavaPairRDD(Of K, Double)
			Return scoreExamplesMultiDataSet(data.mapToPair(New PairDataSetToMultiDataSetFn(Of K)()), includeRegularizationTerms, batchSize)
		End Function

		''' <summary>
		''' Score the examples individually, using the default batch size <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/>. Unlike <seealso cref="calculateScore(JavaRDD, Boolean)"/>,
		''' this method returns a score for each example separately. If scoring is needed for specific examples use either
		''' <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/> or <seealso cref="scoreExamples(JavaPairRDD, Boolean, Integer)"/> which can have
		''' a key for each example.
		''' </summary>
		''' <param name="data">                       Data to score </param>
		''' <param name="includeRegularizationTerms"> If true: include the l1/l2 regularization terms with the score (if any) </param>
		''' <returns> A JavaDoubleRDD containing the scores of each example </returns>
		''' <seealso cref= ComputationGraph#scoreExamples(MultiDataSet, boolean) </seealso>
		Public Overridable Function scoreExamplesMultiDataSet(ByVal data As JavaRDD(Of MultiDataSet), ByVal includeRegularizationTerms As Boolean) As JavaDoubleRDD
			Return scoreExamplesMultiDataSet(data, includeRegularizationTerms, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Score the examples individually, using a specified batch size. Unlike <seealso cref="calculateScore(JavaRDD, Boolean)"/>,
		''' this method returns a score for each example separately. If scoring is needed for specific examples use either
		''' <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/> or <seealso cref="scoreExamples(JavaPairRDD, Boolean, Integer)"/> which can have
		''' a key for each example.
		''' </summary>
		''' <param name="data">                       Data to score </param>
		''' <param name="includeRegularizationTerms"> If true: include the l1/l2 regularization terms with the score (if any) </param>
		''' <param name="batchSize">                  Batch size to use when doing scoring </param>
		''' <returns> A JavaDoubleRDD containing the scores of each example </returns>
		''' <seealso cref= ComputationGraph#scoreExamples(MultiDataSet, boolean) </seealso>
		Public Overridable Function scoreExamplesMultiDataSet(ByVal data As JavaRDD(Of MultiDataSet), ByVal includeRegularizationTerms As Boolean, ByVal batchSize As Integer) As JavaDoubleRDD
			Return data.mapPartitionsToDouble(New ScoreExamplesFunction(sc.broadcast(network_Conflict.params()), sc.broadcast(conf.toJson()), includeRegularizationTerms, batchSize))
		End Function

		''' <summary>
		''' Score the examples individually, using the default batch size <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/>. Unlike <seealso cref="calculateScore(JavaRDD, Boolean)"/>,
		''' this method returns a score for each example separately<br>
		''' Note: The provided JavaPairRDD has a key that is associated with each example and returned score.<br>
		''' <b>Note:</b> The DataSet objects passed in must have exactly one example in them (otherwise: can't have a 1:1 association
		''' between keys and data sets to score)
		''' </summary>
		''' <param name="data">                       Data to score </param>
		''' <param name="includeRegularizationTerms"> If true: include the l1/l2 regularization terms with the score (if any) </param>
		''' @param <K>                        Key type </param>
		''' <returns> A {@code JavaPairRDD<K,Double>} containing the scores of each example </returns>
		''' <seealso cref= MultiLayerNetwork#scoreExamples(DataSet, boolean) </seealso>
		Public Overridable Function scoreExamplesMultiDataSet(Of K)(ByVal data As JavaPairRDD(Of K, MultiDataSet), ByVal includeRegularizationTerms As Boolean) As JavaPairRDD(Of K, Double)
			Return scoreExamplesMultiDataSet(data, includeRegularizationTerms, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Feed-forward the specified data, with the given keys. i.e., get the network output/predictions for the specified data
		''' </summary>
		''' <param name="featuresData"> Features data to feed through the network </param>
		''' <param name="batchSize">    Batch size to use when doing feed forward operations </param>
		''' @param <K>          Type of data for key - may be anything </param>
		''' <returns>             Network output given the input, by key </returns>
		Public Overridable Function feedForwardWithKeySingle(Of K)(ByVal featuresData As JavaPairRDD(Of K, INDArray), ByVal batchSize As Integer) As JavaPairRDD(Of K, INDArray)
			If network_Conflict.NumInputArrays <> 1 OrElse network_Conflict.NumOutputArrays <> 1 Then
				Throw New System.InvalidOperationException("Cannot use this method with computation graphs with more than 1 input or output " & "( has: " & network_Conflict.NumInputArrays & " inputs, " & network_Conflict.NumOutputArrays & " outputs")
			End If
			Dim p As New PairToArrayPair(Of K)()
			Dim rdd As JavaPairRDD(Of K, INDArray()) = featuresData.mapToPair(p)
			Return feedForwardWithKey(rdd, batchSize).mapToPair(New ArrayPairToPair(Of K)())
		End Function

		''' <summary>
		''' Feed-forward the specified data, with the given keys. i.e., get the network output/predictions for the specified data
		''' </summary>
		''' <param name="featuresData"> Features data to feed through the network </param>
		''' <param name="batchSize">    Batch size to use when doing feed forward operations </param>
		''' @param <K>          Type of data for key - may be anything </param>
		''' <returns>             Network output given the input, by key </returns>
		Public Overridable Function feedForwardWithKey(Of K)(ByVal featuresData As JavaPairRDD(Of K, INDArray()), ByVal batchSize As Integer) As JavaPairRDD(Of K, INDArray())
			Return featuresData.mapPartitionsToPair(New GraphFeedForwardWithKeyFunction(Of K)(sc.broadcast(network_Conflict.params()), sc.broadcast(conf.toJson()), batchSize))
		End Function

		Private Sub update(ByVal mr As Integer, ByVal mg As Long)
			Dim env As Environment = EnvironmentUtils.buildEnvironment()
			env.setNumCores(mr)
			env.setAvailableMemory(mg)
			Dim task As Task = ModelSerializer.taskByModel(network_Conflict)
			Heartbeat.Instance.reportEvent([Event].SPARK, env, task)
		End Sub

		''' <summary>
		''' Score the examples individually, using a specified batch size. Unlike <seealso cref="calculateScore(JavaRDD, Boolean)"/>,
		''' this method returns a score for each example separately<br>
		''' Note: The provided JavaPairRDD has a key that is associated with each example and returned score.<br>
		''' <b>Note:</b> The DataSet objects passed in must have exactly one example in them (otherwise: can't have a 1:1 association
		''' between keys and data sets to score)
		''' </summary>
		''' <param name="data">                       Data to score </param>
		''' <param name="includeRegularizationTerms"> If true: include the l1/l2 regularization terms with the score (if any) </param>
		''' @param <K>                        Key type </param>
		''' <returns> A {@code JavaPairRDD<K,Double>} containing the scores of each example </returns>
		''' <seealso cref= MultiLayerNetwork#scoreExamples(DataSet, boolean) </seealso>
		Public Overridable Function scoreExamplesMultiDataSet(Of K)(ByVal data As JavaPairRDD(Of K, MultiDataSet), ByVal includeRegularizationTerms As Boolean, ByVal batchSize As Integer) As JavaPairRDD(Of K, Double)
			Return data.mapPartitionsToPair(New ScoreExamplesWithKeyFunction(Of K)(sc.broadcast(network_Conflict.params()), sc.broadcast(conf.toJson()), includeRegularizationTerms, batchSize))
		End Function

		''' <summary>
		''' Evaluate the single-output network on a directory containing a set of DataSet objects to be loaded with a <seealso cref="DataSetLoader"/>.
		''' Uses default batch size of <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/> </summary>
		''' <param name="path"> Path/URI to the directory containing the datasets to load </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function evaluate(ByVal path As String, ByVal loader As DataSetLoader) As Evaluation
			Dim data As JavaRDD(Of String)
			Try
				data = SparkUtils.listPaths(sc, path)
			Catch e As IOException
				Throw New Exception("Error listing files for evaluation of files at path: " & path, e)
			End Try
			Return CType(doEvaluation(data, DEFAULT_EVAL_WORKERS, DEFAULT_EVAL_SCORE_BATCH_SIZE, loader, DirectCast(Nothing, MultiDataSetLoader), New Evaluation())(0), Evaluation)
		End Function

		''' <summary>
		''' Evaluate the single-output network on a directory containing a set of MultiDataSet objects to be loaded with a <seealso cref="MultiDataSetLoader"/>.
		''' Uses default batch size of <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/> </summary>
		''' <param name="path"> Path/URI to the directory containing the datasets to load </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function evaluate(ByVal path As String, ByVal loader As MultiDataSetLoader) As Evaluation
			Dim data As JavaRDD(Of String)
			Try
				data = SparkUtils.listPaths(sc, path)
			Catch e As IOException
				Throw New Exception("Error listing files for evaluation of files at path: " & path, e)
			End Try
			Return CType(doEvaluation(data, DEFAULT_EVAL_WORKERS, DEFAULT_EVAL_SCORE_BATCH_SIZE, Nothing, loader, New Evaluation())(0), Evaluation)
		End Function

		''' <summary>
		''' {@code RDD<DataSet>} overload of <seealso cref="evaluate(JavaRDD)"/>
		''' </summary>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal data As RDD(Of DataSet)) As T
			Return evaluate(data.toJavaRDD())
		End Function

		''' <summary>
		''' Evaluate the network (classification performance) in a distributed manner on the provided data
		''' </summary>
		''' <param name="data"> Data to evaluate on </param>
		''' <returns> Evaluation object; results of evaluation on all examples in the data set </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal data As JavaRDD(Of DataSet)) As T
			Return evaluate(data, Nothing)
		End Function

		''' <summary>
		''' {@code RDD<DataSet>} overload of <seealso cref="evaluate(JavaRDD, List)"/>
		''' </summary>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal data As RDD(Of DataSet), ByVal labelsList As IList(Of String)) As T
			Return evaluate(data.toJavaRDD(), labelsList)
		End Function

		''' <summary>
		''' Evaluate the network (regression performance) in a distributed manner on the provided data
		''' </summary>
		''' <param name="data"> Data to evaluate </param>
		''' <returns>     <seealso cref="RegressionEvaluation"/> instance with regression performance </returns>
		Public Overridable Function evaluateRegression(Of T As RegressionEvaluation)(ByVal data As JavaRDD(Of DataSet)) As T
			Return evaluateRegression(data, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Evaluate the network (regression performance) in a distributed manner on the provided data
		''' </summary>
		''' <param name="data"> Data to evaluate </param>
		''' <param name="minibatchSize"> Minibatch size to use when doing performing evaluation </param>
		''' <returns>     <seealso cref="RegressionEvaluation"/> instance with regression performance </returns>
		Public Overridable Function evaluateRegression(Of T As RegressionEvaluation)(ByVal data As JavaRDD(Of DataSet), ByVal minibatchSize As Integer) As T
			Dim nOut As val = CType(network_Conflict.getOutputLayer(0).conf().getLayer(), FeedForwardLayer).getNOut()
			Return CType(doEvaluation(data, New org.deeplearning4j.eval.RegressionEvaluation(nOut), minibatchSize), T)
		End Function

		''' <summary>
		''' Evaluate the network (classification performance) in a distributed manner, using default batch size and a provided
		''' list of labels
		''' </summary>
		''' <param name="data">       Data to evaluate on </param>
		''' <param name="labelsList"> List of labels used for evaluation </param>
		''' <returns> Evaluation object; results of evaluation on all examples in the data set </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal data As JavaRDD(Of DataSet), ByVal labelsList As IList(Of String)) As T
			Return evaluate(data, labelsList, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Perform ROC analysis/evaluation on the given DataSet in a distributed manner, using the default number of
		''' threshold steps (<seealso cref="DEFAULT_ROC_THRESHOLD_STEPS"/>) and the default minibatch size (<seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/>)
		''' </summary>
		''' <param name="data">                    Test set data (to evaluate on) </param>
		''' <returns> ROC for the entire data set </returns>
		Public Overridable Function evaluateROC(Of T As ROC)(ByVal data As JavaRDD(Of DataSet)) As T
			Return evaluateROC(data, DEFAULT_ROC_THRESHOLD_STEPS, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Perform ROC analysis/evaluation on the given DataSet in a distributed manner
		''' </summary>
		''' <param name="data">                    Test set data (to evaluate on) </param>
		''' <param name="thresholdSteps">          Number of threshold steps for ROC - see <seealso cref="ROC"/> </param>
		''' <param name="evaluationMinibatchSize"> Minibatch size to use when performing ROC evaluation </param>
		''' <returns> ROC for the entire data set </returns>
		Public Overridable Function evaluateROC(Of T As ROC)(ByVal data As JavaRDD(Of DataSet), ByVal thresholdSteps As Integer, ByVal evaluationMinibatchSize As Integer) As T
			Return CType(doEvaluation(data, New org.deeplearning4j.eval.ROC(thresholdSteps), evaluationMinibatchSize), T)
		End Function

		''' <summary>
		''' Perform ROC analysis/evaluation (for the multi-class case, using <seealso cref="ROCMultiClass"/> on the given DataSet in a distributed manner
		''' </summary>
		''' <param name="data">                    Test set data (to evaluate on) </param>
		''' <returns> ROC for the entire data set </returns>
		Public Overridable Function evaluateROCMultiClass(Of T As ROCMultiClass)(ByVal data As JavaRDD(Of DataSet)) As T
			Return evaluateROCMultiClass(data, DEFAULT_ROC_THRESHOLD_STEPS, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Perform ROC analysis/evaluation (for the multi-class case, using <seealso cref="ROCMultiClass"/> on the given DataSet in a distributed manner
		''' </summary>
		''' <param name="data">                    Test set data (to evaluate on) </param>
		''' <param name="thresholdSteps">          Number of threshold steps for ROC - see <seealso cref="ROC"/> </param>
		''' <param name="evaluationMinibatchSize"> Minibatch size to use when performing ROC evaluation </param>
		''' <returns> ROCMultiClass for the entire data set </returns>
		Public Overridable Function evaluateROCMultiClass(Of T As ROCMultiClass)(ByVal data As JavaRDD(Of DataSet), ByVal thresholdSteps As Integer, ByVal evaluationMinibatchSize As Integer) As T
			Return CType(doEvaluation(data, New org.deeplearning4j.eval.ROCMultiClass(thresholdSteps), evaluationMinibatchSize), T)
		End Function



		''' <summary>
		''' Evaluate the network (classification performance) in a distributed manner, using specified batch size and a provided
		''' list of labels
		''' </summary>
		''' <param name="data">          Data to evaluate on </param>
		''' <param name="labelsList">    List of labels used for evaluation </param>
		''' <param name="evalBatchSize"> Batch size to use when conducting evaluations </param>
		''' <returns> Evaluation object; results of evaluation on all examples in the data set </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal data As JavaRDD(Of DataSet), ByVal labelsList As IList(Of String), ByVal evalBatchSize As Integer) As T
			Dim e As Evaluation = New org.deeplearning4j.eval.Evaluation()
			e = doEvaluation(data, e, evalBatchSize)
			If labelsList IsNot Nothing Then
				e.setLabelsList(labelsList)
			End If
			Return CType(e, T)
		End Function



		''' <summary>
		''' Evaluate the network (classification performance) in a distributed manner on the provided data
		''' </summary>
		Public Overridable Function evaluateMDS(Of T As Evaluation)(ByVal data As JavaRDD(Of MultiDataSet)) As T
			Return evaluateMDS(data, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Evaluate the network (classification performance) in a distributed manner on the provided data
		''' </summary>
		Public Overridable Function evaluateMDS(Of T As Evaluation)(ByVal data As JavaRDD(Of MultiDataSet), ByVal minibatchSize As Integer) As T
			Return CType(doEvaluationMDS(data, minibatchSize, New org.deeplearning4j.eval.Evaluation())(0), T)
		End Function

		''' <summary>
		''' Evaluate the network (regression performance) in a distributed manner on the provided data
		''' </summary>
		''' <param name="data"> Data to evaluate </param>
		''' <returns>     <seealso cref="RegressionEvaluation"/> instance with regression performance </returns>
		Public Overridable Function evaluateRegressionMDS(Of T As RegressionEvaluation)(ByVal data As JavaRDD(Of MultiDataSet)) As T
			Return evaluateRegressionMDS(data, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Evaluate the network (regression performance) in a distributed manner on the provided data
		''' </summary>
		''' <param name="data"> Data to evaluate </param>
		''' <param name="minibatchSize"> Minibatch size to use when doing performing evaluation </param>
		''' <returns>     <seealso cref="RegressionEvaluation"/> instance with regression performance </returns>
		Public Overridable Function evaluateRegressionMDS(Of T As RegressionEvaluation)(ByVal data As JavaRDD(Of MultiDataSet), ByVal minibatchSize As Integer) As T
			Return CType(doEvaluationMDS(data, minibatchSize, New org.deeplearning4j.eval.RegressionEvaluation())(0), T)
		End Function

		''' <summary>
		''' Perform ROC analysis/evaluation on the given DataSet in a distributed manner, using the default number of
		''' threshold steps (<seealso cref="DEFAULT_ROC_THRESHOLD_STEPS"/>) and the default minibatch size (<seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/>)
		''' </summary>
		''' <param name="data">                    Test set data (to evaluate on) </param>
		''' <returns> ROC for the entire data set </returns>
		Public Overridable Function evaluateROCMDS(ByVal data As JavaRDD(Of MultiDataSet)) As ROC
			Return evaluateROCMDS(data, DEFAULT_ROC_THRESHOLD_STEPS, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Perform ROC analysis/evaluation on the given DataSet in a distributed manner, using the specified number of
		''' steps and minibatch size
		''' </summary>
		''' <param name="data">                    Test set data (to evaluate on) </param>
		''' <param name="rocThresholdNumSteps">    See <seealso cref="ROC"/> for details </param>
		''' <param name="minibatchSize">           Minibatch size for evaluation </param>
		''' <returns> ROC for the entire data set </returns>
		Public Overridable Function evaluateROCMDS(Of T As ROC)(ByVal data As JavaRDD(Of MultiDataSet), ByVal rocThresholdNumSteps As Integer, ByVal minibatchSize As Integer) As T
			Return CType(doEvaluationMDS(data, minibatchSize, New org.deeplearning4j.eval.ROC(rocThresholdNumSteps))(0), T)
		End Function


		''' <summary>
		''' Perform distributed evaluation of any type of <seealso cref="IEvaluation"/>. For example, <seealso cref="Evaluation"/>, <seealso cref="RegressionEvaluation"/>,
		''' <seealso cref="ROC"/>, <seealso cref="ROCMultiClass"/> etc.
		''' </summary>
		''' <param name="data">            Data to evaluate on </param>
		''' <param name="emptyEvaluation"> Empty evaluation instance. This is the starting point (serialized/duplicated, then merged) </param>
		''' <param name="evalBatchSize">   Evaluation batch size </param>
		''' @param <T>             Type of evaluation instance to return </param>
		''' <returns>                IEvaluation instance </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.nd4j.evaluation.IEvaluation> T doEvaluation(org.apache.spark.api.java.JavaRDD<org.nd4j.linalg.dataset.DataSet> data, T emptyEvaluation, int evalBatchSize)
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal data As JavaRDD(Of DataSet), ByVal emptyEvaluation As T, ByVal evalBatchSize As Integer) As T
			Dim arr() As IEvaluation = {emptyEvaluation}
			Return CType(doEvaluation(data, evalBatchSize, arr)(0), T)
		End Function

		''' <summary>
		''' Perform distributed evaluation on a <i>single output</i> ComputationGraph form DataSet objects using Spark.
		''' Can be used to perform multiple evaluations on this single output (for example, <seealso cref="Evaluation"/> and
		''' <seealso cref="ROC"/>) at the same time.<br>
		''' Note that the default number of worker threads <seealso cref="getDefaultEvaluationWorkers()"/> will be used
		''' </summary>
		''' <param name="data">             Data to evaluatie </param>
		''' <param name="evalBatchSize">    Minibatch size for evaluation </param>
		''' <param name="emptyEvaluations"> Evaluations to perform </param>
		''' <returns>                 Evaluations </returns>
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal data As JavaRDD(Of DataSet), ByVal evalBatchSize As Integer, ParamArray ByVal emptyEvaluations() As T) As T()
			Return doEvaluation(data, DefaultEvaluationWorkers, evalBatchSize, emptyEvaluations)
		End Function

		''' <summary>
		''' Perform distributed evaluation on a <i>single output</i> ComputationGraph form DataSet objects using Spark.
		''' Can be used to perform multiple evaluations on this single output (for example, <seealso cref="Evaluation"/> and
		''' <seealso cref="ROC"/>) at the same time.<br>
		''' </summary>
		''' <param name="data">             Data to evaluatie </param>
		''' <param name="evalNumWorkers">   Number of worker threads (per machine) to use for evaluation. May want tis to be less than
		'''                         the number of Spark threads per machine/JVM to reduce memory requirements </param>
		''' <param name="evalBatchSize">    Minibatch size for evaluation </param>
		''' <param name="emptyEvaluations"> Evaluations to perform </param>
		''' <returns>                 Evaluations </returns>
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal data As JavaRDD(Of DataSet), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ParamArray ByVal emptyEvaluations() As T) As T()
			Dim evalFn As New IEvaluateFlatMapFunction(Of T)(True, sc.broadcast(conf.toJson()), SparkUtils.asByteArrayBroadcast(sc, network_Conflict.params()), evalNumWorkers, evalBatchSize, emptyEvaluations)
			Dim evaluations As JavaRDD(Of T()) = data.mapPartitions(evalFn)
			Return evaluations.treeAggregate(Nothing, New IEvaluateAggregateFunction(Of T)(), New IEvaluateAggregateFunction(Of T)())
		End Function

		''' <summary>
		''' Perform distributed evaluation on a <i>single output</i> ComputationGraph form MultiDataSet objects using Spark.
		''' Can be used to perform multiple evaluations on this single output (for example, <seealso cref="Evaluation"/> and
		''' <seealso cref="ROC"/>) at the same time.
		''' </summary>
		''' <param name="data">             Data to evaluatie </param>
		''' <param name="evalBatchSize">    Minibatch size for evaluation </param>
		''' <param name="emptyEvaluations"> Evaluations to perform </param>
		''' <returns>                 Evaluations </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.nd4j.evaluation.IEvaluation> T[] doEvaluationMDS(org.apache.spark.api.java.JavaRDD<org.nd4j.linalg.dataset.api.MultiDataSet> data, int evalBatchSize, T... emptyEvaluations)
		Public Overridable Function doEvaluationMDS(Of T As IEvaluation)(ByVal data As JavaRDD(Of MultiDataSet), ByVal evalBatchSize As Integer, ParamArray ByVal emptyEvaluations() As T) As T()
			Return doEvaluationMDS(data, DefaultEvaluationWorkers, evalBatchSize, emptyEvaluations)
		End Function

		Public Overridable Function doEvaluationMDS(Of T As IEvaluation)(ByVal data As JavaRDD(Of MultiDataSet), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ParamArray ByVal emptyEvaluations() As T) As T()
			Preconditions.checkArgument(evalNumWorkers > 0, "Invalid number of evaulation workers: require at least 1 - got %s", evalNumWorkers)
			Dim evalFn As New IEvaluateMDSFlatMapFunction(Of T)(sc.broadcast(conf.toJson()), SparkUtils.asByteArrayBroadcast(sc, network_Conflict.params()), evalNumWorkers, evalBatchSize, emptyEvaluations)
			Dim evaluations As JavaRDD(Of T()) = data.mapPartitions(evalFn)
			Return evaluations.treeAggregate(Nothing, New IEvaluateAggregateFunction(Of T)(), New IEvaluateAggregateFunction(Of T)())
		End Function

		''' <summary>
		''' Perform evaluation on serialized DataSet objects on disk, (potentially in any format), that are loaded using an <seealso cref="DataSetLoader"/>.<br>
		''' Uses the default number of workers (model replicas per JVM) of <seealso cref="DEFAULT_EVAL_WORKERS"/> with the default
		''' minibatch size of <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/> </summary>
		''' <param name="data">             List of paths to the data (that can be loaded as / converted to DataSets) </param>
		''' <param name="loader">           Used to load DataSets from their paths </param>
		''' <param name="emptyEvaluations"> Evaluations to perform </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function doEvaluation(ByVal data As JavaRDD(Of String), ByVal loader As DataSetLoader, ParamArray ByVal emptyEvaluations() As IEvaluation) As IEvaluation()
			Return doEvaluation(data, DEFAULT_EVAL_WORKERS, DEFAULT_EVAL_SCORE_BATCH_SIZE, loader, emptyEvaluations)
		End Function

		''' <summary>
		''' Perform evaluation on serialized DataSet objects on disk, (potentially in any format), that are loaded using an <seealso cref="DataSetLoader"/>. </summary>
		''' <param name="data">             List of paths to the data (that can be loaded as / converted to DataSets) </param>
		''' <param name="evalNumWorkers">   Number of workers to perform evaluation with. To reduce memory requirements and cache thrashing,
		'''                         it is common to set this to a lower value than the number of spark threads per JVM/executor </param>
		''' <param name="evalBatchSize">    Batch size to use when performing evaluation </param>
		''' <param name="loader">           Used to load DataSets from their paths </param>
		''' <param name="emptyEvaluations"> Evaluations to perform </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function doEvaluation(ByVal data As JavaRDD(Of String), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ByVal loader As DataSetLoader, ParamArray ByVal emptyEvaluations() As IEvaluation) As IEvaluation()
			Return doEvaluation(data, evalNumWorkers, evalBatchSize, loader, Nothing, emptyEvaluations)
		End Function

		''' <summary>
		''' Perform evaluation on serialized MultiDataSet objects on disk, (potentially in any format), that are loaded using an <seealso cref="MultiDataSetLoader"/>.<br>
		''' Uses the default number of workers (model replicas per JVM) of <seealso cref="DEFAULT_EVAL_WORKERS"/> with the default
		''' minibatch size of <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/> </summary>
		''' <param name="data">             List of paths to the data (that can be loaded as / converted to DataSets) </param>
		''' <param name="loader">           Used to load MultiDataSets from their paths </param>
		''' <param name="emptyEvaluations"> Evaluations to perform </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function doEvaluation(ByVal data As JavaRDD(Of String), ByVal loader As MultiDataSetLoader, ParamArray ByVal emptyEvaluations() As IEvaluation) As IEvaluation()
			Return doEvaluation(data, DEFAULT_EVAL_WORKERS, DEFAULT_EVAL_SCORE_BATCH_SIZE, Nothing, loader, emptyEvaluations)
		End Function

		''' <summary>
		''' Perform evaluation on serialized MultiDataSet objects on disk, (potentially in any format), that are loaded using an <seealso cref="MultiDataSetLoader"/> </summary>
		''' <param name="data">             List of paths to the data (that can be loaded as / converted to DataSets) </param>
		''' <param name="evalNumWorkers">   Number of workers to perform evaluation with. To reduce memory requirements and cache thrashing,
		'''                         it is common to set this to a lower value than the number of spark threads per JVM/executor </param>
		''' <param name="evalBatchSize">    Batch size to use when performing evaluation </param>
		''' <param name="loader">           Used to load MultiDataSets from their paths </param>
		''' <param name="emptyEvaluations"> Evaluations to perform </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function doEvaluation(ByVal data As JavaRDD(Of String), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ByVal loader As MultiDataSetLoader, ParamArray ByVal emptyEvaluations() As IEvaluation) As IEvaluation()
			Return doEvaluation(data, evalNumWorkers, evalBatchSize, Nothing, loader, emptyEvaluations)
		End Function

		Protected Friend Overridable Function doEvaluation(ByVal data As JavaRDD(Of String), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ByVal loader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader, ParamArray ByVal emptyEvaluations() As IEvaluation) As IEvaluation()
			Dim evalFn As New IEvaluateMDSPathsFlatMapFunction(sc.broadcast(conf.toJson()), SparkUtils.asByteArrayBroadcast(sc, network_Conflict.params()), evalNumWorkers, evalBatchSize, loader, mdsLoader, BroadcastHadoopConfigHolder.get(sc), emptyEvaluations)
			Preconditions.checkArgument(evalNumWorkers > 0, "Invalid number of evaulation workers: require at least 1 - got %s", evalNumWorkers)
			Dim evaluations As JavaRDD(Of IEvaluation()) = data.mapPartitions(evalFn)
			Return evaluations.treeAggregate(Nothing, New IEvaluateAggregateFunction(Of )(), New IEvaluateAggregateFunction(Of )())
		End Function
	End Class

End Namespace