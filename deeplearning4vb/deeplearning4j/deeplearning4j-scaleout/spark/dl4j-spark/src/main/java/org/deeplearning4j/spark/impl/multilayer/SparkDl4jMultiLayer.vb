Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaDoubleRDD = org.apache.spark.api.java.JavaDoubleRDD
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports Matrix = org.apache.spark.mllib.linalg.Matrix
Imports Vector = org.apache.spark.mllib.linalg.Vector
Imports LabeledPoint = org.apache.spark.mllib.regression.LabeledPoint
Imports RDD = org.apache.spark.rdd.RDD
Imports BroadcastHadoopConfigHolder = org.datavec.spark.util.BroadcastHadoopConfigHolder
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports SerializedDataSetLoader = org.deeplearning4j.core.loader.impl.SerializedDataSetLoader
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports org.deeplearning4j.spark.api
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports RemoteFileSourceFactory = org.deeplearning4j.spark.data.loader.RemoteFileSourceFactory
Imports SparkListenable = org.deeplearning4j.spark.impl.SparkListenable
Imports LoadDataSetFunction = org.deeplearning4j.spark.impl.common.LoadDataSetFunction
Imports IntDoubleReduceFunction = org.deeplearning4j.spark.impl.common.reduce.IntDoubleReduceFunction
Imports IEvaluateMDSPathsFlatMapFunction = org.deeplearning4j.spark.impl.graph.evaluation.IEvaluateMDSPathsFlatMapFunction
Imports org.deeplearning4j.spark.impl.multilayer.evaluation
Imports org.deeplearning4j.spark.impl.multilayer.evaluation
Imports org.deeplearning4j.spark.impl.multilayer.evaluation
Imports org.deeplearning4j.spark.impl.multilayer.scoring
Imports MLLibUtil = org.deeplearning4j.spark.util.MLLibUtil
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
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
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

Namespace org.deeplearning4j.spark.impl.multilayer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SparkDl4jMultiLayer extends org.deeplearning4j.spark.impl.SparkListenable
	Public Class SparkDl4jMultiLayer
		Inherits SparkListenable

		Public Const DEFAULT_EVAL_SCORE_BATCH_SIZE As Integer = 64
		Public Const DEFAULT_ROC_THRESHOLD_STEPS As Integer = 32
		Public Const DEFAULT_EVAL_WORKERS As Integer = 4
		<NonSerialized>
		Private sc As JavaSparkContext
		Private conf As MultiLayerConfiguration
'JAVA TO VB CONVERTER NOTE: The field network was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private network_Conflict As MultiLayerNetwork
		Private lastScore As Double
'JAVA TO VB CONVERTER NOTE: The field defaultEvaluationWorkers was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private defaultEvaluationWorkers_Conflict As Integer = DEFAULT_EVAL_WORKERS

		''' <summary>
		''' Instantiate a multi layer spark instance
		''' with the given context and network.
		''' This is the prediction constructor
		''' </summary>
		''' <param name="sparkContext"> the spark context to use </param>
		''' <param name="network">      the network to use </param>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public SparkDl4jMultiLayer(org.apache.spark.SparkContext sparkContext, org.deeplearning4j.nn.multilayer.MultiLayerNetwork network, org.deeplearning4j.spark.api.TrainingMaster<?, ?> trainingMaster)
		Public Sub New(ByVal sparkContext As SparkContext, ByVal network As MultiLayerNetwork, ByVal trainingMaster As TrainingMaster(Of T1, T2))
			Me.New(New JavaSparkContext(sparkContext), network, trainingMaster)
		End Sub

		''' <summary>
		''' Training constructor. Instantiate with a configuration
		''' </summary>
		''' <param name="sparkContext"> the spark context to use </param>
		''' <param name="conf">         the configuration of the network </param>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public SparkDl4jMultiLayer(org.apache.spark.SparkContext sparkContext, org.deeplearning4j.nn.conf.MultiLayerConfiguration conf, org.deeplearning4j.spark.api.TrainingMaster<?, ?> trainingMaster)
		Public Sub New(ByVal sparkContext As SparkContext, ByVal conf As MultiLayerConfiguration, ByVal trainingMaster As TrainingMaster(Of T1, T2))
			Me.New(New JavaSparkContext(sparkContext), initNetwork(conf), trainingMaster)
		End Sub

		''' <summary>
		''' Training constructor. Instantiate with a configuration
		''' </summary>
		''' <param name="sc">   the spark context to use </param>
		''' <param name="conf"> the configuration of the network </param>
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public SparkDl4jMultiLayer(org.apache.spark.api.java.JavaSparkContext sc, org.deeplearning4j.nn.conf.MultiLayerConfiguration conf, org.deeplearning4j.spark.api.TrainingMaster<?, ?> trainingMaster)
		Public Sub New(ByVal sc As JavaSparkContext, ByVal conf As MultiLayerConfiguration, ByVal trainingMaster As TrainingMaster(Of T1, T2))
			Me.New(sc.sc(), conf, trainingMaster)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
'ORIGINAL LINE: public SparkDl4jMultiLayer(org.apache.spark.api.java.JavaSparkContext javaSparkContext, org.deeplearning4j.nn.multilayer.MultiLayerNetwork network, org.deeplearning4j.spark.api.TrainingMaster<?, ?> trainingMaster)
		Public Sub New(ByVal javaSparkContext As JavaSparkContext, ByVal network As MultiLayerNetwork, ByVal trainingMaster As TrainingMaster(Of T1, T2))
			sc = javaSparkContext
			Me.conf = network.LayerWiseConfigurations.clone()
			Me.network_Conflict = network
			If Not network.InitCalled Then
				network.init()
			End If
			Me.trainingMaster = trainingMaster

			'Check if kryo configuration is correct:
			SparkUtils.checkKryoConfiguration(javaSparkContext, log)
		End Sub

		Private Shared Function initNetwork(ByVal conf As MultiLayerConfiguration) As MultiLayerNetwork
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Return net
		End Function

		Public Overridable ReadOnly Property SparkContext As JavaSparkContext
			Get
				Return sc
			End Get
		End Property

		''' <returns> The MultiLayerNetwork underlying the SparkDl4jMultiLayer </returns>
		Public Overridable Property Network As MultiLayerNetwork
			Get
				Return network_Conflict
			End Get
			Set(ByVal network As MultiLayerNetwork)
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
		''' Set whether training statistics should be collected for debugging purposes. Statistics collection is disabled by default
		''' </summary>
		''' <param name="collectTrainingStats"> If true: collect training statistics. If false: don't collect. </param>
		Public Overridable WriteOnly Property CollectTrainingStats As Boolean
			Set(ByVal collectTrainingStats As Boolean)
				trainingMaster.setCollectTrainingStats(collectTrainingStats)
			End Set
		End Property

		''' <summary>
		''' Get the training statistics, after collection of stats has been enabled using <seealso cref="setCollectTrainingStats(Boolean)"/>
		''' </summary>
		''' <returns> Training statistics </returns>
		Public Overridable ReadOnly Property SparkTrainingStats As SparkTrainingStats
			Get
				Return trainingMaster.getTrainingStats()
			End Get
		End Property

		''' <summary>
		''' Predict the given feature matrix
		''' </summary>
		''' <param name="features"> the given feature matrix </param>
		''' <returns> the predictions </returns>
		Public Overridable Function predict(ByVal features As Matrix) As Matrix
			Return MLLibUtil.toMatrix(network_Conflict.output(MLLibUtil.toMatrix(features)))
		End Function


		''' <summary>
		''' Predict the given vector
		''' </summary>
		''' <param name="point"> the vector to predict </param>
		''' <returns> the predicted vector </returns>
		Public Overridable Function predict(ByVal point As Vector) As Vector
			Return MLLibUtil.toVector(network_Conflict.output(MLLibUtil.toVector(point)))
		End Function

		''' <summary>
		''' Fit the DataSet RDD. Equivalent to fit(trainingData.toJavaRDD())
		''' </summary>
		''' <param name="trainingData"> the training data RDD to fitDataSet </param>
		''' <returns> the MultiLayerNetwork after training </returns>
		Public Overridable Function fit(ByVal trainingData As RDD(Of DataSet)) As MultiLayerNetwork
			Return fit(trainingData.toJavaRDD())
		End Function

		''' <summary>
		''' Fit the DataSet RDD
		''' </summary>
		''' <param name="trainingData"> the training data RDD to fitDataSet </param>
		''' <returns> the MultiLayerNetwork after training </returns>
		Public Overridable Function fit(ByVal trainingData As JavaRDD(Of DataSet)) As MultiLayerNetwork
			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			trainingMaster.executeTraining(Me, trainingData)
			network_Conflict.incrementEpochCount()
			Return network_Conflict
		End Function

		''' <summary>
		''' Fit the SparkDl4jMultiLayer network using a directory of serialized DataSet objects
		''' The assumption here is that the directory contains a number of <seealso cref="DataSet"/> objects, each serialized using
		''' <seealso cref="DataSet.save(OutputStream)"/>
		''' </summary>
		''' <param name="path"> Path to the directory containing the serialized DataSet objcets </param>
		''' <returns> The MultiLayerNetwork after training </returns>
		Public Overridable Function fit(ByVal path As String) As MultiLayerNetwork
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
		Public Overridable Function fit(ByVal path As String, ByVal minPartitions As Integer) As MultiLayerNetwork
			Return fit(path)
		End Function

		''' <summary>
		''' Fit the network using a list of paths for serialized DataSet objects.
		''' </summary>
		''' <param name="paths">    List of paths </param>
		''' <returns> trained network </returns>
		Public Overridable Function fitPaths(ByVal paths As JavaRDD(Of String)) As MultiLayerNetwork
			Return fitPaths(paths, New SerializedDataSetLoader())
		End Function

		Public Overridable Function fitPaths(ByVal paths As JavaRDD(Of String), ByVal loader As DataSetLoader) As MultiLayerNetwork
			trainingMaster.executeTrainingPaths(Me, Nothing, paths, loader, Nothing)
			network_Conflict.incrementEpochCount()
			Return network_Conflict
		End Function

		''' <summary>
		''' Fit a MultiLayerNetwork using Spark MLLib LabeledPoint instances.
		''' This will convert the labeled points to the internal DL4J data format and train the model on that
		''' </summary>
		''' <param name="rdd"> the rdd to fitDataSet </param>
		''' <returns> the multi layer network that was fitDataSet </returns>
		Public Overridable Function fitLabeledPoint(ByVal rdd As JavaRDD(Of LabeledPoint)) As MultiLayerNetwork
			Dim nLayers As Integer = network_Conflict.LayerWiseConfigurations.getConfs().size()
			Dim ffl As FeedForwardLayer = CType(network_Conflict.LayerWiseConfigurations.getConf(nLayers - 1).getLayer(), FeedForwardLayer)
			Dim ds As JavaRDD(Of DataSet) = MLLibUtil.fromLabeledPoint(sc, rdd, ffl.getNOut())
			Return fit(ds)
		End Function

		''' <summary>
		''' Fits a MultiLayerNetwork using Spark MLLib LabeledPoint instances
		''' This will convert labeled points that have continuous labels used for regression to the internal
		''' DL4J data format and train the model on that </summary>
		''' <param name="rdd"> the javaRDD containing the labeled points </param>
		''' <returns> a MultiLayerNetwork </returns>
		Public Overridable Function fitContinuousLabeledPoint(ByVal rdd As JavaRDD(Of LabeledPoint)) As MultiLayerNetwork
			Return fit(MLLibUtil.fromContinuousLabeledPoint(sc, rdd))
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
		''' Overload of <seealso cref="calculateScore(JavaRDD, Boolean)"/> for {@code RDD<DataSet>} instead of {@code JavaRDD<DataSet>}
		''' </summary>
		Public Overridable Function calculateScore(ByVal data As RDD(Of DataSet), ByVal average As Boolean) As Double
			Return calculateScore(data.toJavaRDD(), average)
		End Function

		''' <summary>
		''' Calculate the score for all examples in the provided {@code JavaRDD<DataSet>}, either by summing
		''' or averaging over the entire data set. To calculate a score for each example individually, use <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/>
		''' or one of the similar methods. Uses default minibatch size in each worker, <seealso cref="SparkDl4jMultiLayer.DEFAULT_EVAL_SCORE_BATCH_SIZE"/>
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
			Dim rdd As JavaRDD(Of Tuple2(Of Integer, Double)) = data.mapPartitions(New ScoreFlatMapFunction(conf.toJson(), sc.broadcast(network_Conflict.params(False)), minibatchSize))

			'Reduce to a single tuple, with example count + sum of scores
			Dim countAndSumScores As Tuple2(Of Integer, Double) = rdd.reduce(New IntDoubleReduceFunction())
			If average Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return countAndSumScores._2() / countAndSumScores._1()
			Else
				Return countAndSumScores._2()
			End If
		End Function

		''' <summary>
		''' {@code RDD<DataSet>} overload of <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/>
		''' </summary>
		Public Overridable Function scoreExamples(ByVal data As RDD(Of DataSet), ByVal includeRegularizationTerms As Boolean) As JavaDoubleRDD
			Return scoreExamples(data.toJavaRDD(), includeRegularizationTerms)
		End Function

		''' <summary>
		''' Score the examples individually, using the default batch size <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/>. Unlike <seealso cref="calculateScore(JavaRDD, Boolean)"/>,
		''' this method returns a score for each example separately. If scoring is needed for specific examples use either
		''' <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/> or <seealso cref="scoreExamples(JavaPairRDD, Boolean, Integer)"/> which can have
		''' a key for each example.
		''' </summary>
		''' <param name="data">                       Data to score </param>
		''' <param name="includeRegularizationTerms"> If  true: include the l1/l2 regularization terms with the score (if any) </param>
		''' <returns> A JavaDoubleRDD containing the scores of each example </returns>
		''' <seealso cref= MultiLayerNetwork#scoreExamples(DataSet, boolean) </seealso>
		Public Overridable Function scoreExamples(ByVal data As JavaRDD(Of DataSet), ByVal includeRegularizationTerms As Boolean) As JavaDoubleRDD
			Return scoreExamples(data, includeRegularizationTerms, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' {@code RDD<DataSet>}
		''' overload of <seealso cref="scoreExamples(JavaRDD, Boolean, Integer)"/>
		''' </summary>
		Public Overridable Function scoreExamples(ByVal data As RDD(Of DataSet), ByVal includeRegularizationTerms As Boolean, ByVal batchSize As Integer) As JavaDoubleRDD
			Return scoreExamples(data.toJavaRDD(), includeRegularizationTerms, batchSize)
		End Function

		''' <summary>
		''' Score the examples individually, using a specified batch size. Unlike <seealso cref="calculateScore(JavaRDD, Boolean)"/>,
		''' this method returns a score for each example separately. If scoring is needed for specific examples use either
		''' <seealso cref="scoreExamples(JavaPairRDD, Boolean)"/> or <seealso cref="scoreExamples(JavaPairRDD, Boolean, Integer)"/> which can have
		''' a key for each example.
		''' </summary>
		''' <param name="data">                       Data to score </param>
		''' <param name="includeRegularizationTerms"> If  true: include the l1/l2 regularization terms with the score (if any) </param>
		''' <param name="batchSize">                  Batch size to use when doing scoring </param>
		''' <returns> A JavaDoubleRDD containing the scores of each example </returns>
		''' <seealso cref= MultiLayerNetwork#scoreExamples(DataSet, boolean) </seealso>
		Public Overridable Function scoreExamples(ByVal data As JavaRDD(Of DataSet), ByVal includeRegularizationTerms As Boolean, ByVal batchSize As Integer) As JavaDoubleRDD
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
		''' <param name="includeRegularizationTerms"> If  true: include the l1/l2 regularization terms with the score (if any) </param>
		''' @param <K>                        Key type </param>
		''' <returns> A {@code JavaPairRDD<K,Double>} containing the scores of each example </returns>
		''' <seealso cref= MultiLayerNetwork#scoreExamples(DataSet, boolean) </seealso>
		Public Overridable Function scoreExamples(Of K)(ByVal data As JavaPairRDD(Of K, DataSet), ByVal includeRegularizationTerms As Boolean) As JavaPairRDD(Of K, Double)
			Return scoreExamples(data, includeRegularizationTerms, DEFAULT_EVAL_SCORE_BATCH_SIZE)
		End Function

		''' <summary>
		''' Score the examples individually, using a specified batch size. Unlike <seealso cref="calculateScore(JavaRDD, Boolean)"/>,
		''' this method returns a score for each example separately<br>
		''' Note: The provided JavaPairRDD has a key that is associated with each example and returned score.<br>
		''' <b>Note:</b> The DataSet objects passed in must have exactly one example in them (otherwise: can't have a 1:1 association
		''' between keys and data sets to score)
		''' </summary>
		''' <param name="data">                       Data to score </param>
		''' <param name="includeRegularizationTerms"> If  true: include the l1/l2 regularization terms with the score (if any) </param>
		''' @param <K>                        Key type </param>
		''' <returns> A {@code JavaPairRDD<K,Double>} containing the scores of each example </returns>
		''' <seealso cref= MultiLayerNetwork#scoreExamples(DataSet, boolean) </seealso>
		Public Overridable Function scoreExamples(Of K)(ByVal data As JavaPairRDD(Of K, DataSet), ByVal includeRegularizationTerms As Boolean, ByVal batchSize As Integer) As JavaPairRDD(Of K, Double)
			Return data.mapPartitionsToPair(New ScoreExamplesWithKeyFunction(Of K)(sc.broadcast(network_Conflict.params()), sc.broadcast(conf.toJson()), includeRegularizationTerms, batchSize))
		End Function

		''' <summary>
		''' Feed-forward the specified data, with the given keys. i.e., get the network output/predictions for the specified data
		''' </summary>
		''' <param name="featuresData"> Features data to feed through the network </param>
		''' <param name="batchSize">    Batch size to use when doing feed forward operations </param>
		''' @param <K>          Type of data for key - may be anything </param>
		''' <returns> Network output given the input, by key </returns>
		Public Overridable Function feedForwardWithKey(Of K)(ByVal featuresData As JavaPairRDD(Of K, INDArray), ByVal batchSize As Integer) As JavaPairRDD(Of K, INDArray)
			Return feedForwardWithMaskAndKey(featuresData.mapToPair(New SingleToPairFunction(Of K)()), batchSize)
		End Function

		''' <summary>
		''' Feed-forward the specified data (and optionally mask array), with the given keys. i.e., get the network
		''' output/predictions for the specified data
		''' </summary>
		''' <param name="featuresDataAndMask"> Features data to feed through the network. The Tuple2 is of the network input (features),
		'''                            and optionally the feature mask arrays </param>
		''' <param name="batchSize">           Batch size to use when doing feed forward operations </param>
		''' @param <K>                 Type of data for key - may be anything </param>
		''' <returns> Network output given the input (and optionally mask), by key </returns>
		Public Overridable Function feedForwardWithMaskAndKey(Of K)(ByVal featuresDataAndMask As JavaPairRDD(Of K, Tuple2(Of INDArray, INDArray)), ByVal batchSize As Integer) As JavaPairRDD(Of K, INDArray)
			Return featuresDataAndMask.mapPartitionsToPair(New FeedForwardWithKeyFunction(Of K)(sc.broadcast(network_Conflict.params()), sc.broadcast(conf.toJson()), batchSize))
		End Function

		''' <summary>
		''' {@code RDD<DataSet>} overload of <seealso cref="evaluate(JavaRDD)"/>
		''' </summary>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal data As RDD(Of DataSet)) As T
			Return evaluate(data.toJavaRDD())
		End Function

		''' <summary>
		''' Evaluate on a directory containing a set of DataSet objects serialized with <seealso cref="DataSet.save(OutputStream)"/> </summary>
		''' <param name="path"> Path/URI to the directory containing the dataset objects </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal path As String) As T
			Return evaluate(path, New SerializedDataSetLoader())
		End Function

		''' <summary>
		''' Evaluate on a directory containing a set of DataSet objects to be loaded with a <seealso cref="DataSetLoader"/>.
		''' Uses default batch size of <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/> </summary>
		''' <param name="path"> Path/URI to the directory containing the datasets to load </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal path As String, ByVal loader As DataSetLoader) As T
			Return evaluate(path, DEFAULT_EVAL_SCORE_BATCH_SIZE, loader)
		End Function

		''' <summary>
		''' Evaluate on a directory containing a set of DataSet objects to be loaded with a <seealso cref="DataSetLoader"/>.
		''' Uses default batch size of <seealso cref="DEFAULT_EVAL_SCORE_BATCH_SIZE"/> </summary>
		''' <param name="path"> Path/URI to the directory containing the datasets to load </param>
		''' <returns> Evaluation </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal path As String, ByVal batchSize As Integer, ByVal loader As DataSetLoader) As T
			Dim paths As JavaRDD(Of String)
			Try
				paths = SparkUtils.listPaths(sc, path)
			Catch e As IOException
				Throw New Exception("Error listing paths in directory", e)
			End Try

			Dim rdd As JavaRDD(Of DataSet) = paths.map(New LoadDataSetFunction(loader, New RemoteFileSourceFactory(BroadcastHadoopConfigHolder.get(sc))))
			Return CType(doEvaluation(rdd, batchSize, New org.deeplearning4j.eval.Evaluation())(0), T)
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
			Dim nOut As Long = CType(network_Conflict.OutputLayer.conf().getLayer(), FeedForwardLayer).getNOut()
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

		Private Sub update(ByVal mr As Integer, ByVal mg As Long)
			Dim env As Environment = EnvironmentUtils.buildEnvironment()
			env.setNumCores(mr)
			env.setAvailableMemory(mg)
			Dim task As Task = ModelSerializer.taskByModel(network_Conflict)
			Heartbeat.Instance.reportEvent([Event].SPARK, env, task)
		End Sub

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
			Return doEvaluation(data, evalBatchSize, emptyEvaluation)(0)
		End Function

		''' <summary>
		''' Perform distributed evaluation of any type of <seealso cref="IEvaluation"/> - or multiple IEvaluation instances.
		''' Distributed equivalent of <seealso cref="MultiLayerNetwork.doEvaluation(DataSetIterator, IEvaluation[])"/>
		''' </summary>
		''' <param name="data">             Data to evaluate on </param>
		''' <param name="emptyEvaluations"> Empty evaluation instances. Starting point (serialized/duplicated, then merged) </param>
		''' <param name="evalBatchSize">    Evaluation batch size </param>
		''' @param <T>              Type of evaluation instance to return </param>
		''' <returns> IEvaluation instances </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.nd4j.evaluation.IEvaluation> T[] doEvaluation(org.apache.spark.api.java.JavaRDD<org.nd4j.linalg.dataset.DataSet> data, int evalBatchSize, T... emptyEvaluations)
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal data As JavaRDD(Of DataSet), ByVal evalBatchSize As Integer, ParamArray ByVal emptyEvaluations() As T) As T()
			Return doEvaluation(data, DefaultEvaluationWorkers, evalBatchSize, emptyEvaluations)
		End Function
		''' <summary>
		''' Perform distributed evaluation of any type of <seealso cref="IEvaluation"/> - or multiple IEvaluation instances.
		''' Distributed equivalent of <seealso cref="MultiLayerNetwork.doEvaluation(DataSetIterator, IEvaluation[])"/>
		''' </summary>
		''' <param name="data">             Data to evaluate on </param>
		''' <param name="emptyEvaluations"> Empty evaluation instances. Starting point (serialized/duplicated, then merged) </param>
		''' <param name="evalNumWorkers">   Number of workers (copies of the MultiLayerNetwork) model to use. Generally this should
		'''                         be smaller than the number of threads - 2 to 4 is often good enough. If using CUDA GPUs,
		'''                         this should ideally be set to the number of GPUs on each node (i.e., 1 for a single GPU node) </param>
		''' <param name="evalBatchSize">    Evaluation batch size </param>
		''' @param <T>              Type of evaluation instance to return </param>
		''' <returns> IEvaluation instances </returns>
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal data As JavaRDD(Of DataSet), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ParamArray ByVal emptyEvaluations() As T) As T()
			Dim evalFn As New IEvaluateFlatMapFunction(Of T)(False, sc.broadcast(conf.toJson()), SparkUtils.asByteArrayBroadcast(sc, network_Conflict.params()), evalNumWorkers, evalBatchSize, emptyEvaluations)
			Dim evaluations As JavaRDD(Of T()) = data.mapPartitions(evalFn)
			Return evaluations.treeAggregate(Nothing, New IEvaluateAggregateFunction(Of T)(), New IEvaluationReduceFunction(Of T)())
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
			Dim config As Configuration = sc.hadoopConfiguration()
			Dim evalFn As New IEvaluateMDSPathsFlatMapFunction(sc.broadcast(conf.toJson()), SparkUtils.asByteArrayBroadcast(sc, network_Conflict.params()), evalNumWorkers, evalBatchSize, loader, mdsLoader, BroadcastHadoopConfigHolder.get(sc), emptyEvaluations)
			Preconditions.checkArgument(evalNumWorkers > 0, "Invalid number of evaulation workers: require at least 1 - got %s", evalNumWorkers)
			Dim evaluations As JavaRDD(Of IEvaluation()) = data.mapPartitions(evalFn)
			Return evaluations.treeAggregate(Nothing, New IEvaluateAggregateFunction(Of )(), New IEvaluateAggregateFunction(Of )())
		End Function
	End Class

End Namespace