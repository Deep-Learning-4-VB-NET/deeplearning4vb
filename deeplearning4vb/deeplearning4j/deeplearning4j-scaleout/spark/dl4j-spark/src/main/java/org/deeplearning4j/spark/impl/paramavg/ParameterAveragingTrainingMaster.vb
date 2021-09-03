Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaRDDLike = org.apache.spark.api.java.JavaRDDLike
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports BroadcastHadoopConfigHolder = org.datavec.spark.util.BroadcastHadoopConfigHolder
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports SerializedDataSetLoader = org.deeplearning4j.core.loader.impl.SerializedDataSetLoader
Imports SerializedMultiDataSetLoader = org.deeplearning4j.core.loader.impl.SerializedMultiDataSetLoader
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StatsStorageRouterProvider = org.deeplearning4j.core.storage.StatsStorageRouterProvider
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports org.deeplearning4j.spark.api
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports org.deeplearning4j.spark.api.worker
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports DataSetToMultiDataSetFn = org.deeplearning4j.spark.impl.graph.dataset.DataSetToMultiDataSetFn
Imports VanillaStatsStorageRouterProvider = org.deeplearning4j.spark.impl.listeners.VanillaStatsStorageRouterProvider
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingAggregationTuple = org.deeplearning4j.spark.impl.paramavg.aggregator.ParameterAveragingAggregationTuple
Imports ParameterAveragingElementAddFunction = org.deeplearning4j.spark.impl.paramavg.aggregator.ParameterAveragingElementAddFunction
Imports ParameterAveragingElementCombineFunction = org.deeplearning4j.spark.impl.paramavg.aggregator.ParameterAveragingElementCombineFunction
Imports ParameterAveragingTrainingMasterStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingMasterStats
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
import static org.nd4j.shade.guava.base.Preconditions.checkArgument

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
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"stats", "listeners", "iterationCount", "rng", "lastExportedRDDId", "lastRDDExportPath", "trainingMasterUID"}) @EqualsAndHashCode(exclude = {"stats", "listeners", "iterationCount", "rng", "lastExportedRDDId", "lastRDDExportPath", "trainingMasterUID"}) @Slf4j public class ParameterAveragingTrainingMaster extends BaseTrainingMaster<ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker> implements TrainingMaster<ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker>
	Public Class ParameterAveragingTrainingMaster
		Inherits BaseTrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker)
		Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker)

		Protected Friend Const COALESCE_THRESHOLD As Integer = 3


		Protected Friend saveUpdater As Boolean
		Protected Friend numWorkers As Integer?
		Protected Friend rddDataSetNumExamples As Integer

		Protected Friend averagingFrequency As Integer
		Protected Friend aggregationDepth As Integer
		Protected Friend prefetchNumBatches As Integer
		Protected Friend iterationCount As Integer = 0

		Protected Friend trainingHookList As ICollection(Of TrainingHook)

		Protected Friend Sub New()
			' no-arg constructor for Jackson

			Dim jvmuid As String = UIDProvider.JVMUID
			Me.trainingMasterUID = DateTimeHelper.CurrentUnixTimeMillis() & "_" & (If(jvmuid.Length <= 8, jvmuid, jvmuid.Substring(0, 8)))
			Me.rng = New Random()
		End Sub

		Protected Friend Sub New(ByVal builder As Builder)
			Me.saveUpdater = builder.saveUpdater_Conflict
			Me.numWorkers = builder.numWorkers
			Me.rddDataSetNumExamples = builder.rddDataSetNumExamples
			Me.batchSizePerWorker = builder.batchSizePerWorker_Conflict
			Me.averagingFrequency = builder.averagingFrequency_Conflict
			Me.aggregationDepth = builder.aggregationDepth_Conflict
			Me.prefetchNumBatches = builder.prefetchNumBatches
			Me.repartition = builder.repartition
			Me.repartitionStrategy = builder.repartitionStrategy_Conflict
			Me.storageLevel = builder.storageLevel_Conflict
			Me.storageLevelStreams = builder.storageLevelStreams_Conflict
			Me.rddTrainingApproach = builder.rddTrainingApproach_Conflict
			Me.exportDirectory = builder.exportDirectory_Conflict
			Me.trainingHookList = builder.trainingHooks_Conflict
			Me.collectTrainingStats = builder.collectTrainingStats_Conflict
			If collectTrainingStats Then
				stats = New ParameterAveragingTrainingMasterStats.ParameterAveragingTrainingMasterStatsHelper()
			End If


			If builder.rngSeed_Conflict Is Nothing Then
				Me.rng = New Random()
			Else
				Me.rng = New Random(builder.rngSeed_Conflict)
			End If

			Dim jvmuid As String = UIDProvider.JVMUID
			Me.trainingMasterUID = DateTimeHelper.CurrentUnixTimeMillis() & "_" & (If(jvmuid.Length <= 8, jvmuid, jvmuid.Substring(0, 8)))
		End Sub

		Public Sub New(ByVal saveUpdater As Boolean, ByVal numWorkers As Integer?, ByVal rddDataSetNumExamples As Integer, ByVal batchSizePerWorker As Integer, ByVal averagingFrequency As Integer, ByVal prefetchNumBatches As Integer)
			Me.New(saveUpdater, numWorkers, rddDataSetNumExamples, batchSizePerWorker, averagingFrequency, 2, prefetchNumBatches, Repartition.Always, RepartitionStrategy.Balanced, False)
		End Sub

		''' <param name="saveUpdater">           If true: save (and average) the updater state when doing parameter averaging </param>
		''' <param name="numWorkers">            Number of workers (executors * threads per executor) for the cluster </param>
		''' <param name="rddDataSetNumExamples"> Number of examples in each DataSet object in the {@code RDD<DataSet>} </param>
		''' <param name="batchSizePerWorker">    Number of examples to use per worker per fit </param>
		''' <param name="averagingFrequency">    Frequency (in number of minibatches) with which to average parameters </param>
		''' <param name="aggregationDepth">      Number of aggregation levels used in parameter aggregation </param>
		''' <param name="prefetchNumBatches">    Number of batches to asynchronously prefetch (0: disable) </param>
		''' <param name="repartition">           Set if/when repartitioning should be conducted for the training data </param>
		''' <param name="repartitionStrategy">   Repartitioning strategy to use. See <seealso cref="RepartitionStrategy"/> </param>
		''' <param name="collectTrainingStats">  If true: collect training statistics for debugging/optimization purposes </param>
		Public Sub New(ByVal saveUpdater As Boolean, ByVal numWorkers As Integer?, ByVal rddDataSetNumExamples As Integer, ByVal batchSizePerWorker As Integer, ByVal averagingFrequency As Integer, ByVal aggregationDepth As Integer, ByVal prefetchNumBatches As Integer, ByVal repartition As Repartition, ByVal repartitionStrategy As RepartitionStrategy, ByVal collectTrainingStats As Boolean)
			Me.New(saveUpdater, numWorkers, rddDataSetNumExamples, batchSizePerWorker, averagingFrequency, aggregationDepth, prefetchNumBatches, repartition, repartitionStrategy, StorageLevel.MEMORY_ONLY_SER(), collectTrainingStats)
		End Sub

		Public Sub New(ByVal saveUpdater As Boolean, ByVal numWorkers As Integer?, ByVal rddDataSetNumExamples As Integer, ByVal batchSizePerWorker As Integer, ByVal averagingFrequency As Integer, ByVal aggregationDepth As Integer, ByVal prefetchNumBatches As Integer, ByVal repartition As Repartition, ByVal repartitionStrategy As RepartitionStrategy, ByVal storageLevel As StorageLevel, ByVal collectTrainingStats As Boolean)
			checkArgument(numWorkers > 0, "Invalid number of workers: " & numWorkers & " (must be >= 1)")
			checkArgument(rddDataSetNumExamples > 0, "Invalid rdd data set size: " & rddDataSetNumExamples & " (must be >= 1)")
			checkArgument(averagingFrequency > 0, "Invalid input: averaging frequency must be >= 1")
			checkArgument(aggregationDepth > 0, "Invalid input: tree aggregation channels must be >= 1")

			Me.saveUpdater = saveUpdater
			Me.numWorkers = numWorkers
			Me.rddDataSetNumExamples = rddDataSetNumExamples
			Me.batchSizePerWorker = batchSizePerWorker
			Me.averagingFrequency = averagingFrequency
			Me.aggregationDepth = aggregationDepth
			Me.prefetchNumBatches = prefetchNumBatches
			Me.collectTrainingStats = collectTrainingStats
			Me.repartition = repartition
			Me.repartitionStrategy = repartitionStrategy
			Me.storageLevel = storageLevel
			If collectTrainingStats Then
				stats = New ParameterAveragingTrainingMasterStats.ParameterAveragingTrainingMasterStatsHelper()
			End If

			Dim jvmuid As String = UIDProvider.JVMUID
			Me.trainingMasterUID = DateTimeHelper.CurrentUnixTimeMillis() & "_" & (If(jvmuid.Length <= 8, jvmuid, jvmuid.Substring(0, 8)))
			Me.rng = New Random()
		End Sub



		''' <summary>
		''' Remove a training hook from the worker
		''' </summary>
		''' <param name="trainingHook"> the training hook to remove </param>
		Public Overrides Sub removeHook(ByVal trainingHook As TrainingHook) Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).removeHook
			If trainingHookList Is Nothing Then
				Return
			End If
			trainingHookList.remove(trainingHook)
		End Sub

		''' <summary>
		''' Add a hook for the master for pre and post training
		''' </summary>
		''' <param name="trainingHook"> the training hook to add </param>
		Public Overrides Sub addHook(ByVal trainingHook As TrainingHook) Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).addHook
			If trainingHookList Is Nothing Then
				trainingHookList = New List(Of TrainingHook)()
			End If
			trainingHookList.Add(trainingHook)
		End Sub

		Public Overrides Function toJson() As String Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).toJson
			Dim om As ObjectMapper = JsonMapper

			Try
				Return om.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception("Error producing JSON representation for ParameterAveragingTrainingMaster", e)
			End Try
		End Function

		Public Overrides Function toYaml() As String Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).toYaml
			Dim om As ObjectMapper = YamlMapper

			Try
				Return om.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception("Error producing YAML representation for ParameterAveragingTrainingMaster", e)
			End Try
		End Function

		''' <summary>
		''' Create a ParameterAveragingTrainingMaster instance by deserializing a JSON string that has been serialized with
		''' <seealso cref="toJson()"/>
		''' </summary>
		''' <param name="jsonStr"> ParameterAveragingTrainingMaster configuration serialized as JSON </param>
		Public Shared Function fromJson(ByVal jsonStr As String) As ParameterAveragingTrainingMaster
			Dim om As ObjectMapper = JsonMapper
			Try
				Return om.readValue(jsonStr, GetType(ParameterAveragingTrainingMaster))
			Catch e As IOException
				Throw New Exception("Could not parse JSON", e)
			End Try
		End Function

		''' <summary>
		''' Create a ParameterAveragingTrainingMaster instance by deserializing a YAML string that has been serialized with
		''' <seealso cref="toYaml()"/>
		''' </summary>
		''' <param name="yamlStr"> ParameterAveragingTrainingMaster configuration serialized as YAML </param>
		Public Shared Function fromYaml(ByVal yamlStr As String) As ParameterAveragingTrainingMaster
			Dim om As ObjectMapper = YamlMapper
			Try
				Return om.readValue(yamlStr, GetType(ParameterAveragingTrainingMaster))
			Catch e As IOException
				Throw New Exception("Could not parse YAML", e)
			End Try
		End Function


		Public Overrides Function getWorkerInstance(ByVal network As SparkDl4jMultiLayer) As ParameterAveragingTrainingWorker
			Dim tuple As New NetBroadcastTuple(network.Network.LayerWiseConfigurations, network.Network.params(), network.Network.Updater.StateViewArray)

			If collectTrainingStats Then
				stats.logBroadcastStart()
			End If
			Dim broadcast As Broadcast(Of NetBroadcastTuple) = network.SparkContext.broadcast(tuple)
			If collectTrainingStats Then
				stats.logBroadcastEnd()
			End If

			Dim configuration As New WorkerConfiguration(False, rddDataSetNumExamples, batchSizePerWorker, averagingFrequency, prefetchNumBatches, collectTrainingStats)
			Return New ParameterAveragingTrainingWorker(broadcast, saveUpdater, configuration, trainingHookList, listeners, RouterProvider)
		End Function

		Public Overrides Function getWorkerInstance(ByVal graph As SparkComputationGraph) As ParameterAveragingTrainingWorker
			Dim tuple As New NetBroadcastTuple(graph.Network.Configuration, graph.Network.params(), graph.Network.Updater.StateViewArray)

			If collectTrainingStats Then
				stats.logBroadcastStart()
			End If
			Dim broadcast As Broadcast(Of NetBroadcastTuple) = graph.SparkContext.broadcast(tuple)
			If collectTrainingStats Then
				stats.logBroadcastEnd()
			End If

			Dim configuration As New WorkerConfiguration(True, rddDataSetNumExamples, batchSizePerWorker, averagingFrequency, prefetchNumBatches, collectTrainingStats)
			Return New ParameterAveragingTrainingWorker(broadcast, saveUpdater, configuration, trainingHookList, listeners, RouterProvider)
		End Function

		Protected Friend Overridable Function numObjectsEachWorker(ByVal numExamplesEachRddObject As Integer) As Integer
			Return batchSizePerWorker * averagingFrequency \ numExamplesEachRddObject
		End Function

		Protected Friend Overridable Function getNumDataSetObjectsPerSplit(ByVal numExamplesEachRddObject As Integer) As Integer
			Dim dataSetObjectsPerSplit As Integer
			If numExamplesEachRddObject = 1 Then
				dataSetObjectsPerSplit = numWorkers.Value * batchSizePerWorker * averagingFrequency
			Else
				Dim numDataSetObjsReqEachWorker As Integer = numObjectsEachWorker(numExamplesEachRddObject)
				If numDataSetObjsReqEachWorker < 1 Then
					'In this case: more examples in a DataSet object than we actually require
					'For example, 100 examples in DataSet, with batchSizePerWorker=50 and averagingFrequency=1
					numDataSetObjsReqEachWorker = 1
				End If

				dataSetObjectsPerSplit = numDataSetObjsReqEachWorker * numWorkers.Value
			End If
			Return dataSetObjectsPerSplit
		End Function

		Public Overridable Overloads Sub executeTraining(ByVal network As SparkDl4jMultiLayer, ByVal trainingData As JavaRDD(Of DataSet)) Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).executeTraining
			If numWorkers Is Nothing Then
				numWorkers = network.SparkContext.defaultParallelism()
			End If

			If rddTrainingApproach = RDDTrainingApproach.Direct Then
				executeTrainingDirect(network, trainingData)
			Else
				'Export data if required (or, use cached export)
				Dim paths As JavaRDD(Of String) = exportIfRequired(network.SparkContext, trainingData)
				executeTrainingPathsHelper(network, Nothing, paths, New SerializedDataSetLoader(), Nothing, batchSizePerWorker) 'Originally (pre-export): had rddDataSetNumExamples per DataSet. Now we have batchSizePerWorker per exported DataSet
			End If
		End Sub

		Protected Friend Overridable Function getTotalDataSetObjectCount(Of T, Repr As JavaRDDLike(Of T, Repr))(ByVal trainingData As JavaRDDLike(Of T, Repr)) As Long
			If collectTrainingStats Then
				stats.logCountStart()
			End If
			Dim totalDataSetObjectCount As Long = trainingData.count()
			If collectTrainingStats Then
				stats.logCountEnd()
			End If
			Return totalDataSetObjectCount
		End Function

		Protected Friend Overridable Function getSplitRDDs(Of T, Repr)(ByVal trainingData As JavaPairRDD(Of T, Repr), ByVal totalDataSetObjectCount As Integer) As JavaPairRDD(Of T, Repr)()
			Dim dataSetObjectsPerSplit As Integer = getNumDataSetObjectsPerSplit(rddDataSetNumExamples)

			If collectTrainingStats Then
				stats.logSplitStart()
			End If
			Dim splits() As JavaPairRDD(Of T, Repr) = SparkUtils.balancedRandomSplit(totalDataSetObjectCount, dataSetObjectsPerSplit, trainingData, rng.nextLong())
			If collectTrainingStats Then
				stats.logSplitEnd()
			End If
			Return splits
		End Function

		Protected Friend Overridable Function getSplitRDDs(Of T)(ByVal trainingData As JavaRDD(Of T), ByVal totalDataSetObjectCount As Integer, ByVal examplesPerDataSetObject As Integer) As JavaRDD(Of T)()
			Dim dataSetObjectsPerSplit As Integer = getNumDataSetObjectsPerSplit(examplesPerDataSetObject)

			If collectTrainingStats Then
				stats.logSplitStart()
			End If
			Dim splits() As JavaRDD(Of T) = SparkUtils.balancedRandomSplit(totalDataSetObjectCount, dataSetObjectsPerSplit, trainingData, rng.nextLong())
			If collectTrainingStats Then
				stats.logSplitEnd()
			End If
			Return splits
		End Function

		Protected Friend Overridable Sub executeTrainingDirect(ByVal network As SparkDl4jMultiLayer, ByVal trainingData As JavaRDD(Of DataSet))
			If collectTrainingStats Then
				stats.logFitStart()
			End If
			'For "vanilla" parameter averaging training, we need to split the full data set into batches of size N, such that we can process the specified
			' number of minibatches between averagings
			'But to do that, wee need to know: (a) the number of examples, and (b) the number of workers
			If storageLevel IsNot Nothing Then
				trainingData.persist(storageLevel)
			End If

			Dim totalDataSetObjectCount As Long = getTotalDataSetObjectCount(trainingData)
			Dim splits() As JavaRDD(Of DataSet) = getSplitRDDs(trainingData, CInt(totalDataSetObjectCount), rddDataSetNumExamples)

			Dim splitNum As Integer = 1
			For Each split As JavaRDD(Of DataSet) In splits
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: doIteration(network, split, splitNum++, splits.length);
				doIteration(network, split, splitNum, splits.Length)
					splitNum += 1
			Next split

			If collectTrainingStats Then
				stats.logFitEnd(CInt(totalDataSetObjectCount))
			End If
		End Sub

		Public Overridable Overloads Sub executeTrainingPaths(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal trainingDataPaths As JavaRDD(Of String), ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader) Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).executeTrainingPaths
			executeTrainingPathsHelper(network, graph, trainingDataPaths, dsLoader, mdsLoader, rddDataSetNumExamples)
		End Sub

		Protected Friend Overridable Sub executeTrainingPathsHelper(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal trainingDataPaths As JavaRDD(Of String), ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader, ByVal dataSetObjectsNumExamples As Integer)
			If numWorkers Is Nothing Then
				numWorkers = network.SparkContext.defaultParallelism()
			End If

			If collectTrainingStats Then
				stats.logFitStart()
			End If
			If storageLevelStreams IsNot Nothing Then
				trainingDataPaths.persist(storageLevelStreams)
			End If

			Dim totalDataSetObjectCount As Long = getTotalDataSetObjectCount(trainingDataPaths)
			Dim splits() As JavaRDD(Of String) = getSplitRDDs(trainingDataPaths, CInt(totalDataSetObjectCount), dataSetObjectsNumExamples)

			Dim splitNum As Integer = 1
			For Each split As JavaRDD(Of String) In splits
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: doIterationPaths(network, graph, split, splitNum++, splits.length, dataSetObjectsNumExamples, dsLoader, mdsLoader);
				doIterationPaths(network, graph, split, splitNum, splits.Length, dataSetObjectsNumExamples, dsLoader, mdsLoader)
					splitNum += 1
			Next split

			If collectTrainingStats Then
				stats.logFitEnd(CInt(totalDataSetObjectCount))
			End If
		End Sub

		Public Overridable Overloads Sub executeTraining(ByVal graph As SparkComputationGraph, ByVal trainingData As JavaRDD(Of DataSet)) Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).executeTraining
			If numWorkers Is Nothing Then
				numWorkers = graph.SparkContext.defaultParallelism()
			End If

			Dim mdsTrainingData As JavaRDD(Of MultiDataSet) = trainingData.map(New DataSetToMultiDataSetFn())

			executeTrainingMDS(graph, mdsTrainingData)
		End Sub

		Public Overridable Overloads Sub executeTrainingMDS(ByVal graph As SparkComputationGraph, ByVal trainingData As JavaRDD(Of MultiDataSet)) Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).executeTrainingMDS
			If numWorkers Is Nothing Then
				numWorkers = graph.SparkContext.defaultParallelism()
			End If

			If rddTrainingApproach = RDDTrainingApproach.Direct Then
				executeTrainingDirect(graph, trainingData)
			Else
				'Export data if required (or, use cached export)
				Dim paths As JavaRDD(Of String) = exportIfRequiredMDS(graph.SparkContext, trainingData)
				executeTrainingPathsHelper(Nothing, graph, paths, Nothing, New SerializedMultiDataSetLoader(), batchSizePerWorker)
			End If
		End Sub

		Protected Friend Overridable Sub executeTrainingDirect(ByVal graph As SparkComputationGraph, ByVal trainingData As JavaRDD(Of MultiDataSet))
			If collectTrainingStats Then
				stats.logFitStart()
			End If
			'For "vanilla" parameter averaging training, we need to split the full data set into batches of size N, such that we can process the specified
			' number of minibatches between averaging
			'But to do that, we need to know: (a) the number of examples, and (b) the number of workers
			If storageLevel IsNot Nothing Then
				trainingData.persist(storageLevel)
			End If

			Dim totalDataSetObjectCount As Long = getTotalDataSetObjectCount(trainingData)

			Dim splits() As JavaRDD(Of MultiDataSet) = getSplitRDDs(trainingData, CInt(totalDataSetObjectCount), rddDataSetNumExamples)

			Dim splitNum As Integer = 1
			For Each split As JavaRDD(Of MultiDataSet) In splits
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: doIteration(graph, split, splitNum++, splits.length);
				doIteration(graph, split, splitNum, splits.Length)
					splitNum += 1
			Next split

			If collectTrainingStats Then
				stats.logFitEnd(CInt(totalDataSetObjectCount))
			End If
		End Sub

		Public Overrides WriteOnly Property CollectTrainingStats(ByVal collectTrainingStats As Boolean) Implements TrainingMaster.setCollectTrainingStats As Boolean
			Set(ByVal collectTrainingStats As Boolean)
				Me.collectTrainingStats = collectTrainingStats
				If collectTrainingStats Then
					If Me.stats Is Nothing Then
						Me.stats = New ParameterAveragingTrainingMasterStats.ParameterAveragingTrainingMasterStatsHelper()
					End If
				Else
					Me.stats = Nothing
				End If
			End Set
		End Property

		Public Overrides ReadOnly Property IsCollectTrainingStats As Boolean Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).getIsCollectTrainingStats
			Get
				Return collectTrainingStats
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingStats As SparkTrainingStats Implements TrainingMaster(Of ParameterAveragingTrainingResult, ParameterAveragingTrainingWorker).getTrainingStats
			Get
				If stats IsNot Nothing Then
					Return stats.build()
				End If
				Return Nothing
			End Get
		End Property

		Public Overridable Overloads WriteOnly Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				setListeners(Nothing, listeners)
			End Set
		End Property

		Public Overridable Overloads Sub setListeners(ByVal statsStorage As StatsStorageRouter, ByVal listeners As ICollection(Of TrainingListener))
			Me.statsStorage = statsStorage
			Me.listeners = If(listeners Is Nothing, Nothing, New List(Of )(listeners))
		End Sub



		Protected Friend Overridable Sub doIteration(ByVal network As SparkDl4jMultiLayer, ByVal split As JavaRDD(Of DataSet), ByVal splitNum As Integer, ByVal numSplits As Integer)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, averagingFreq={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, averagingFrequency, numWorkers)
			If collectTrainingStats Then
				stats.logMapPartitionsStart()
			End If

			Dim splitData As JavaRDD(Of DataSet) = split
			If collectTrainingStats Then
				stats.logRepartitionStart()
			End If
			splitData = SparkUtils.repartition(splitData, repartition, repartitionStrategy, numObjectsEachWorker(rddDataSetNumExamples), numWorkers)
			Dim nPartitions As Integer = splitData.partitions().size()
			If collectTrainingStats AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If


			Dim [function] As FlatMapFunction(Of IEnumerator(Of DataSet), ParameterAveragingTrainingResult) = New ExecuteWorkerFlatMap(Of IEnumerator(Of DataSet), ParameterAveragingTrainingResult)(getWorkerInstance(network))
			Dim result As JavaRDD(Of ParameterAveragingTrainingResult) = splitData.mapPartitions([function])
			processResults(network, Nothing, result, splitNum, numSplits)

			If collectTrainingStats Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub

		<Obsolete>
		Protected Friend Overridable Sub doIterationPDS(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal split As JavaRDD(Of PortableDataStream), ByVal splitNum As Integer, ByVal numSplits As Integer)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, averagingFreq={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, averagingFrequency, numWorkers)
			If collectTrainingStats Then
				stats.logMapPartitionsStart()
			End If

			Dim splitData As JavaRDD(Of PortableDataStream) = split
			If collectTrainingStats Then
				stats.logRepartitionStart()
			End If
			splitData = SparkUtils.repartition(splitData, repartition, repartitionStrategy, numObjectsEachWorker(rddDataSetNumExamples), numWorkers)
			Dim nPartitions As Integer = splitData.partitions().size()
			If collectTrainingStats AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If

			Dim [function] As FlatMapFunction(Of IEnumerator(Of PortableDataStream), ParameterAveragingTrainingResult)
			If network IsNot Nothing Then
				[function] = New ExecuteWorkerPDSFlatMap(Of IEnumerator(Of PortableDataStream), ParameterAveragingTrainingResult)(getWorkerInstance(network))
			Else
				[function] = New ExecuteWorkerPDSFlatMap(Of IEnumerator(Of PortableDataStream), ParameterAveragingTrainingResult)(getWorkerInstance(graph))
			End If

			Dim result As JavaRDD(Of ParameterAveragingTrainingResult) = splitData.mapPartitions([function])
			processResults(network, graph, result, splitNum, numSplits)

			If collectTrainingStats Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub

		Protected Friend Overridable Sub doIterationPaths(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal split As JavaRDD(Of String), ByVal splitNum As Integer, ByVal numSplits As Integer, ByVal dataSetObjectNumExamples As Integer, ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, averagingFreq={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, averagingFrequency, numWorkers)
			If collectTrainingStats Then
				stats.logMapPartitionsStart()
			End If

			Dim splitData As JavaRDD(Of String) = split
			If collectTrainingStats Then
				stats.logRepartitionStart()
			End If
			splitData = SparkUtils.repartition(splitData, repartition, repartitionStrategy, numObjectsEachWorker(dataSetObjectNumExamples), numWorkers)
			Dim nPartitions As Integer = splitData.partitions().size()
			If collectTrainingStats AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If

			Dim sc As JavaSparkContext = (If(network IsNot Nothing, network.SparkContext, graph.SparkContext))
			Dim [function] As FlatMapFunction(Of IEnumerator(Of String), ParameterAveragingTrainingResult)
			If network IsNot Nothing Then
				If dsLoader IsNot Nothing Then
					[function] = New ExecuteWorkerPathFlatMap(Of IEnumerator(Of String), ParameterAveragingTrainingResult)(getWorkerInstance(network), dsLoader, BroadcastHadoopConfigHolder.get(sc))
				Else
					[function] = New ExecuteWorkerPathMDSFlatMap(Of IEnumerator(Of String), ParameterAveragingTrainingResult)(getWorkerInstance(network), mdsLoader, BroadcastHadoopConfigHolder.get(sc))
				End If
			Else
				If dsLoader IsNot Nothing Then
					[function] = New ExecuteWorkerPathFlatMap(Of IEnumerator(Of String), ParameterAveragingTrainingResult)(getWorkerInstance(graph), dsLoader, BroadcastHadoopConfigHolder.get(sc))
				Else
					[function] = New ExecuteWorkerPathMDSFlatMap(Of IEnumerator(Of String), ParameterAveragingTrainingResult)(getWorkerInstance(graph), mdsLoader, BroadcastHadoopConfigHolder.get(sc))
				End If
			End If

			Dim result As JavaRDD(Of ParameterAveragingTrainingResult) = splitData.mapPartitions([function])
			processResults(network, graph, result, splitNum, numSplits)

			If collectTrainingStats Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub

		Protected Friend Overridable Sub doIteration(ByVal graph As SparkComputationGraph, ByVal split As JavaRDD(Of MultiDataSet), ByVal splitNum As Integer, ByVal numSplits As Integer)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, averagingFreq={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, averagingFrequency, numWorkers)
			If collectTrainingStats Then
				stats.logMapPartitionsStart()
			End If

			Dim splitData As JavaRDD(Of MultiDataSet) = split

			splitData = SparkUtils.repartition(splitData, repartition, repartitionStrategy, numObjectsEachWorker(rddDataSetNumExamples), numWorkers)
			Dim nPartitions As Integer = split.partitions().size()

			Dim [function] As FlatMapFunction(Of IEnumerator(Of MultiDataSet), ParameterAveragingTrainingResult) = New ExecuteWorkerMultiDataSetFlatMap(Of IEnumerator(Of MultiDataSet), ParameterAveragingTrainingResult)(getWorkerInstance(graph))
			Dim result As JavaRDD(Of ParameterAveragingTrainingResult) = splitData.mapPartitions([function])
			processResults(Nothing, graph, result, splitNum, numSplits)

			If collectTrainingStats Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub

		Protected Friend Overridable Sub doIterationPDS_MDS(ByVal graph As SparkComputationGraph, ByVal split As JavaRDD(Of PortableDataStream), ByVal splitNum As Integer, ByVal numSplits As Integer)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, averagingFreq={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, averagingFrequency, numWorkers)
			If collectTrainingStats Then
				stats.logMapPartitionsStart()
			End If

			Dim splitData As JavaRDD(Of PortableDataStream) = split
			If collectTrainingStats Then
				stats.logRepartitionStart()
			End If
			splitData = SparkUtils.repartition(splitData, repartition, repartitionStrategy, numObjectsEachWorker(rddDataSetNumExamples), numWorkers)
			Dim nPartitions As Integer = splitData.partitions().size()
			If collectTrainingStats AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If

			Dim [function] As FlatMapFunction(Of IEnumerator(Of PortableDataStream), ParameterAveragingTrainingResult) = New ExecuteWorkerPDSMDSFlatMap(Of IEnumerator(Of PortableDataStream), ParameterAveragingTrainingResult)(getWorkerInstance(graph))

			Dim result As JavaRDD(Of ParameterAveragingTrainingResult) = splitData.mapPartitions([function])
			processResults(Nothing, graph, result, splitNum, numSplits)

			If collectTrainingStats Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub


		Protected Friend Overridable Sub processResults(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal results As JavaRDD(Of ParameterAveragingTrainingResult), ByVal splitNum As Integer, ByVal totalSplits As Integer)
			'Need to do parameter averaging, and where necessary also do averaging of the updaters
			'Let's do all of this in ONE step, such that we don't have extra synchronization costs

			If collectTrainingStats Then
				stats.logAggregateStartTime()
			End If
			Dim tuple As ParameterAveragingAggregationTuple = results.treeAggregate(Nothing, New ParameterAveragingElementAddFunction(), New ParameterAveragingElementCombineFunction(), Me.aggregationDepth)
			Dim params As INDArray = tuple.getParametersSum()
			Dim aggCount As Integer = tuple.getAggregationsCount()
			Dim aggregatedStats As SparkTrainingStats = tuple.getSparkTrainingStats()
			If collectTrainingStats Then
				stats.logAggregationEndTime()
			End If


			If collectTrainingStats Then
				stats.logProcessParamsUpdaterStart()
			End If
			If params IsNot Nothing Then
				params.divi(aggCount)
				Dim updaterState As INDArray = tuple.getUpdaterStateSum()
				If updaterState IsNot Nothing Then
					updaterState.divi(aggCount) 'May be null if all SGD updaters, for example
				End If

				If network IsNot Nothing Then
					Dim net As MultiLayerNetwork = network.Network
					net.Parameters = params
					If updaterState IsNot Nothing Then
						net.Updater.setStateViewArray(Nothing, updaterState, False)
					End If

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					network.Score = tuple.getScoreSum() / tuple.getAggregationsCount()
				Else
					Dim g As ComputationGraph = graph.Network
					g.Params = params
					If updaterState IsNot Nothing Then
						g.Updater.StateViewArray = updaterState
					End If

'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					graph.Score = tuple.getScoreSum() / tuple.getAggregationsCount()
				End If
			Else
				log.info("Skipping imbalanced split with no data for all executors")
			End If



			If collectTrainingStats Then
				stats.logProcessParamsUpdaterEnd()
				stats.addWorkerStats(aggregatedStats)
			End If

			If statsStorage IsNot Nothing Then
				Dim meta As ICollection(Of StorageMetaData) = tuple.getListenerMetaData()
				If meta IsNot Nothing AndAlso meta.Count > 0 Then
					statsStorage.putStorageMetaData(meta)
				End If

				Dim staticInfo As ICollection(Of Persistable) = tuple.getListenerStaticInfo()
				If staticInfo IsNot Nothing AndAlso staticInfo.Count > 0 Then
					statsStorage.putStaticInfo(staticInfo)
				End If

				Dim updates As ICollection(Of Persistable) = tuple.getListenerUpdates()
				If updates IsNot Nothing AndAlso updates.Count > 0 Then
					statsStorage.putUpdate(updates)
				End If
			End If

			Nd4j.Executioner.commit()

			log.info("Completed training of split {} of {}", splitNum, totalSplits)

			If params IsNot Nothing Then
				'Params may be null for edge case (empty RDD)
				If network IsNot Nothing Then
					Dim conf As MultiLayerConfiguration = network.Network.LayerWiseConfigurations
					Dim numUpdates As Integer = averagingFrequency
					conf.setIterationCount(conf.getIterationCount() + numUpdates)
				Else
					Dim conf As ComputationGraphConfiguration = graph.Network.Configuration
					Dim numUpdates As Integer = averagingFrequency
					conf.setIterationCount(conf.getIterationCount() + numUpdates)
				End If
			End If
		End Sub



		Protected Friend Overridable ReadOnly Property RouterProvider As StatsStorageRouterProvider
			Get
				If statsStorage Is Nothing Then
					Return Nothing 'Not needed
				End If
				Return New VanillaStatsStorageRouterProvider()
			End Get
		End Property


		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field saveUpdater was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend saveUpdater_Conflict As Boolean
			Protected Friend numWorkers As Integer?
			Protected Friend rddDataSetNumExamples As Integer
'JAVA TO VB CONVERTER NOTE: The field batchSizePerWorker was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend batchSizePerWorker_Conflict As Integer = 16
'JAVA TO VB CONVERTER NOTE: The field averagingFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend averagingFrequency_Conflict As Integer = 5
'JAVA TO VB CONVERTER NOTE: The field aggregationDepth was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend aggregationDepth_Conflict As Integer = 2
			Protected Friend prefetchNumBatches As Integer = 0
			Protected Friend repartition As Repartition = Repartition.Always
'JAVA TO VB CONVERTER NOTE: The field repartitionStrategy was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend repartitionStrategy_Conflict As RepartitionStrategy = RepartitionStrategy.Balanced
'JAVA TO VB CONVERTER NOTE: The field storageLevel was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend storageLevel_Conflict As StorageLevel = StorageLevel.MEMORY_ONLY_SER()
'JAVA TO VB CONVERTER NOTE: The field storageLevelStreams was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend storageLevelStreams_Conflict As StorageLevel = StorageLevel.MEMORY_ONLY()
'JAVA TO VB CONVERTER NOTE: The field rddTrainingApproach was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend rddTrainingApproach_Conflict As RDDTrainingApproach = RDDTrainingApproach.Export
'JAVA TO VB CONVERTER NOTE: The field exportDirectory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend exportDirectory_Conflict As String = Nothing
'JAVA TO VB CONVERTER NOTE: The field rngSeed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend rngSeed_Conflict As Long?
'JAVA TO VB CONVERTER NOTE: The field trainingHooks was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend trainingHooks_Conflict As ICollection(Of TrainingHook)
'JAVA TO VB CONVERTER NOTE: The field collectTrainingStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend collectTrainingStats_Conflict As Boolean = False


			''' <summary>
			''' Adds training hooks to the master.
			''' The training master will setup the workers
			''' with the desired hooks for training.
			''' This can allow for tings like parameter servers
			''' and async updates as well as collecting statistics.
			''' </summary>
			''' <param name="trainingHooks"> the training hooks to ad
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter trainingHooks was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function trainingHooks(ByVal trainingHooks_Conflict As ICollection(Of TrainingHook)) As Builder
				Me.trainingHooks_Conflict = trainingHooks_Conflict
				Return Me
			End Function

			''' <summary>
			''' Adds training hooks to the master.
			''' The training master will setup the workers
			''' with the desired hooks for training.
			''' This can allow for tings like parameter servers
			''' and async updates as well as collecting statistics. </summary>
			''' <param name="hooks"> the training hooks to ad
			''' @return </param>
			Public Overridable Function trainingHooks(ParamArray ByVal hooks() As TrainingHook) As Builder
				Me.trainingHooks_Conflict = java.util.Arrays.asList(hooks)
				Return Me
			End Function

			''' <summary>
			''' Same as <seealso cref="Builder(Integer, Integer)"/> but automatically set number of workers based on JavaSparkContext.defaultParallelism()
			''' </summary>
			''' <param name="rddDataSetNumExamples"> Number of examples in each DataSet object in the {@code RDD<DataSet>} </param>
			Public Sub New(ByVal rddDataSetNumExamples As Integer)
				Me.New(Nothing, rddDataSetNumExamples)
			End Sub

			''' <summary>
			''' Create a builder, where the following number of workers (Spark executors * number of threads per executor) are
			''' being used.<br>
			''' Note: this should match the configuration of the cluster.<br>
			''' <para>
			''' It is also necessary to specify how many examples are in each DataSet that appears in the {@code RDD<DataSet>}
			''' or {@code JavaRDD<DataSet>} used for training.<br>
			''' Two most common cases here:<br>
			''' (a) Preprocessed data pipelines will often load binary DataSet objects with N > 1 examples in each; in this case,
			''' rddDataSetNumExamples should be set to N <br>
			''' (b) "In line" data pipelines (for example, CSV String -> record reader -> DataSet just before training) will
			''' typically have exactly 1 example in each DataSet object. In this case, rddDataSetNumExamples should be set to 1
			''' 
			''' </para>
			''' </summary>
			''' <param name="numWorkers">            Number of Spark execution threads in the cluster. May be null. If null: number of workers will
			'''                              be obtained from JavaSparkContext.defaultParallelism(), which should provide the number of cores
			'''                              in the cluster. </param>
			''' <param name="rddDataSetNumExamples"> Number of examples in each DataSet object in the {@code RDD<DataSet>} </param>
			Public Sub New(ByVal numWorkers As Integer?, ByVal rddDataSetNumExamples As Integer)
				checkArgument(numWorkers Is Nothing OrElse numWorkers > 0, "Invalid number of workers: " & numWorkers & " (must be >= 1)")
				checkArgument(rddDataSetNumExamples > 0, "Invalid rdd data set size: " & rddDataSetNumExamples & " (must be >= 1)")
				Me.numWorkers = numWorkers
				Me.rddDataSetNumExamples = rddDataSetNumExamples
			End Sub

			''' <summary>
			''' Batch size (in number of examples) per worker, for each fit(DataSet) call.
			''' </summary>
			''' <param name="batchSizePerWorker"> Size of each minibatch to use for each worker
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter batchSizePerWorker was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function batchSizePerWorker(ByVal batchSizePerWorker_Conflict As Integer) As Builder
				Me.batchSizePerWorker_Conflict = batchSizePerWorker_Conflict
				Return Me
			End Function

			''' <summary>
			''' Frequency with which to average worker parameters.<br>
			''' <b>Note</b>: Too high or too low can be bad for different reasons.<br>
			''' - Too low (such as 1) can result in a lot of network traffic<br>
			''' - Too high (>> 20 or so) can result in accuracy issues or problems with network convergence
			''' </summary>
			''' <param name="averagingFrequency"> Frequency (in number of minibatches of size 'batchSizePerWorker') to average parameters </param>
'JAVA TO VB CONVERTER NOTE: The parameter averagingFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function averagingFrequency(ByVal averagingFrequency_Conflict As Integer) As Builder
				checkArgument(averagingFrequency_Conflict > 0, "Invalid input: averaging frequency must be >= 1")
				Me.averagingFrequency_Conflict = averagingFrequency_Conflict
				Return Me
			End Function

			''' <summary>
			''' The number of levels in the aggregation tree for parameter synchronization. (default: 2)
			''' <b>Note</b>: For large models trained with many partitions, increasing this number
			''' will reduce the load on the driver and help prevent it from becoming a bottleneck.<br>
			''' </summary>
			''' <param name="aggregationDepth"> RDD tree aggregation channels when averaging parameter updates. </param>
'JAVA TO VB CONVERTER NOTE: The parameter aggregationDepth was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function aggregationDepth(ByVal aggregationDepth_Conflict As Integer) As Builder
				checkArgument(aggregationDepth_Conflict > 0, "Invalid input: tree aggregation channels must be >= 1")
				Me.aggregationDepth_Conflict = aggregationDepth_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the number of minibatches to asynchronously prefetch in the worker.
			''' <para>
			''' Default: 0 (no prefetching)
			''' 
			''' </para>
			''' </summary>
			''' <param name="prefetchNumBatches"> Number of minibatches (DataSets of size batchSizePerWorker) to fetch </param>
			Public Overridable Function workerPrefetchNumBatches(ByVal prefetchNumBatches As Integer) As Builder
				Me.prefetchNumBatches = prefetchNumBatches
				Return Me
			End Function

			''' <summary>
			''' Set whether the updater (i.e., historical state for momentum, adagrad, etc should be saved).
			''' <b>NOTE</b>: This can <b>double</b> (or more) the amount of network traffic in each direction, but might
			''' improve network training performance (and can be more stable for certain updaters such as adagrad).<br>
			''' <para>
			''' This is <b>enabled</b> by default.
			''' 
			''' </para>
			''' </summary>
			''' <param name="saveUpdater"> If true: retain the updater state (default). If false, don't retain (updaters will be
			'''                    reinitalized in each worker after averaging). </param>
'JAVA TO VB CONVERTER NOTE: The parameter saveUpdater was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function saveUpdater(ByVal saveUpdater_Conflict As Boolean) As Builder
				Me.saveUpdater_Conflict = saveUpdater_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set if/when repartitioning should be conducted for the training data.<br>
			''' Default value: always repartition (if required to guarantee correct number of partitions and correct number
			''' of examples in each partition).
			''' </summary>
			''' <param name="repartition"> Setting for repartitioning </param>
			Public Overridable Function repartionData(ByVal repartition As Repartition) As Builder
				Me.repartition = repartition
				Return Me
			End Function

			''' <summary>
			''' Used in conjunction with <seealso cref="repartionData(Repartition)"/> (which defines <i>when</i> repartitioning should be
			''' conducted), repartitionStrategy defines <i>how</i> the repartitioning should be done. See <seealso cref="RepartitionStrategy"/>
			''' for details
			''' </summary>
			''' <param name="repartitionStrategy"> Repartitioning strategy to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter repartitionStrategy was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function repartitionStrategy(ByVal repartitionStrategy_Conflict As RepartitionStrategy) As Builder
				Me.repartitionStrategy_Conflict = repartitionStrategy_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the storage level for {@code RDD<DataSet>}s.<br>
			''' Default: StorageLevel.MEMORY_ONLY_SER() - i.e., store in memory, in serialized form<br>
			''' To use no RDD persistence, use {@code null}<br>
			''' <para>
			''' <b>Note</b>: Spark's StorageLevel.MEMORY_ONLY() and StorageLevel.MEMORY_AND_DISK() can be problematic when
			''' it comes to off-heap data (which DL4J/ND4J uses extensively). Spark does not account for off-heap memory
			''' when deciding if/when to drop blocks to ensure enough free memory; consequently, for DataSet RDDs that are
			''' larger than the total amount of (off-heap) memory, this can lead to OOM issues. Put another way: Spark counts
			''' the on-heap size of DataSet and INDArray objects only (which is negligible) resulting in a significant
			''' underestimate of the true DataSet object sizes. More DataSets are thus kept in memory than we can really afford.
			''' 
			''' </para>
			''' </summary>
			''' <param name="storageLevel"> Storage level to use for DataSet RDDs </param>
'JAVA TO VB CONVERTER NOTE: The parameter storageLevel was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function storageLevel(ByVal storageLevel_Conflict As StorageLevel) As Builder
				Me.storageLevel_Conflict = storageLevel_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the storage level RDDs used when fitting data from Streams: either PortableDataStreams (sc.binaryFiles via
			''' <seealso cref="SparkDl4jMultiLayer.fit(String)"/> and <seealso cref="SparkComputationGraph.fit(String)"/>) or String paths
			''' (via <seealso cref="SparkDl4jMultiLayer.fitPaths(JavaRDD)"/>, <seealso cref="SparkComputationGraph.fitPaths(JavaRDD)"/> and
			''' <seealso cref="SparkComputationGraph.fitPathsMultiDataSet(JavaRDD)"/>).<br>
			''' <para>
			''' Default storage level is StorageLevel.MEMORY_ONLY() which should be appropriate in most cases.
			''' 
			''' </para>
			''' </summary>
			''' <param name="storageLevelStreams"> Storage level to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter storageLevelStreams was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function storageLevelStreams(ByVal storageLevelStreams_Conflict As StorageLevel) As Builder
				Me.storageLevelStreams_Conflict = storageLevelStreams_Conflict
				Return Me
			End Function

			''' <summary>
			''' The approach to use when training on a {@code RDD<DataSet>} or {@code RDD<MultiDataSet>}.
			''' Default: <seealso cref="RDDTrainingApproach.Export"/>, which exports data to a temporary directory first
			''' </summary>
			''' <param name="rddTrainingApproach"> Training approach to use when training from a {@code RDD<DataSet>} or {@code RDD<MultiDataSet>} </param>
'JAVA TO VB CONVERTER NOTE: The parameter rddTrainingApproach was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function rddTrainingApproach(ByVal rddTrainingApproach_Conflict As RDDTrainingApproach) As Builder
				Me.rddTrainingApproach_Conflict = rddTrainingApproach_Conflict
				Return Me
			End Function

			''' <summary>
			''' When <seealso cref="rddTrainingApproach(RDDTrainingApproach)"/> is set to <seealso cref="RDDTrainingApproach.Export"/> (as it is by default)
			''' the data is exported to a temporary directory first.
			''' <para>
			''' Default: null. -> use {hadoop.tmp.dir}/dl4j/. In this case, data is exported to {hadoop.tmp.dir}/dl4j/SOME_UNIQUE_ID/<br>
			''' If you specify a directory, the directory {exportDirectory}/SOME_UNIQUE_ID/ will be used instead.
			''' 
			''' </para>
			''' </summary>
			''' <param name="exportDirectory"> Base directory to export data </param>
'JAVA TO VB CONVERTER NOTE: The parameter exportDirectory was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function exportDirectory(ByVal exportDirectory_Conflict As String) As Builder
				Me.exportDirectory_Conflict = exportDirectory_Conflict
				Return Me
			End Function

			''' <summary>
			''' Random number generator seed, used mainly for enforcing repeatable splitting on RDDs
			''' Default: no seed set (i.e., random seed)
			''' </summary>
			''' <param name="rngSeed"> RNG seed
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter rngSeed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function rngSeed(ByVal rngSeed_Conflict As Long) As Builder
				Me.rngSeed_Conflict = rngSeed_Conflict
				Return Me
			End Function

			''' <summary>
			''' Whether training stats collection should be enabled (disabled by default). </summary>
			''' <seealso cref= ParameterAveragingTrainingMaster#setCollectTrainingStats(boolean) </seealso>
			''' <seealso cref= org.deeplearning4j.spark.stats.StatsUtils#exportStatsAsHTML(SparkTrainingStats, OutputStream) </seealso>
			''' <param name="collectTrainingStats"> </param>
'JAVA TO VB CONVERTER NOTE: The parameter collectTrainingStats was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function collectTrainingStats(ByVal collectTrainingStats_Conflict As Boolean) As Builder
				Me.collectTrainingStats_Conflict = collectTrainingStats_Conflict
				Return Me
			End Function

			Public Overridable Function build() As ParameterAveragingTrainingMaster
				Return New ParameterAveragingTrainingMaster(Me)
			End Function
		End Class
	End Class

End Namespace