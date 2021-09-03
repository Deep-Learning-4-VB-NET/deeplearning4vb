Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaRDDLike = org.apache.spark.api.java.JavaRDDLike
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports StorageLevel = org.apache.spark.storage.StorageLevel
Imports BroadcastHadoopConfigHolder = org.datavec.spark.util.BroadcastHadoopConfigHolder
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports SerializedDataSetLoader = org.deeplearning4j.core.loader.impl.SerializedDataSetLoader
Imports SerializedMultiDataSetLoader = org.deeplearning4j.core.loader.impl.SerializedMultiDataSetLoader
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports DL4JEnvironmentVars = org.deeplearning4j.common.config.DL4JEnvironmentVars
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports ResidualClippingPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.residual.ResidualClippingPostProcessor
Imports AdaptiveThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.AdaptiveThresholdAlgorithm
Imports org.deeplearning4j.spark.api
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports NetBroadcastTuple = org.deeplearning4j.spark.api.worker.NetBroadcastTuple
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports org.deeplearning4j.spark.impl.paramavg
Imports ParameterAveragingTrainingMasterStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingMasterStats
Imports DefaultRepartitioner = org.deeplearning4j.spark.impl.repartitioner.DefaultRepartitioner
Imports SharedTrainingAccumulationFunction = org.deeplearning4j.spark.parameterserver.accumulation.SharedTrainingAccumulationFunction
Imports SharedTrainingAccumulationTuple = org.deeplearning4j.spark.parameterserver.accumulation.SharedTrainingAccumulationTuple
Imports SharedTrainingAggregateFunction = org.deeplearning4j.spark.parameterserver.accumulation.SharedTrainingAggregateFunction
Imports SharedTrainingConfiguration = org.deeplearning4j.spark.parameterserver.conf.SharedTrainingConfiguration
Imports org.deeplearning4j.spark.parameterserver.functions
Imports org.deeplearning4j.spark.parameterserver.functions
Imports org.deeplearning4j.spark.parameterserver.functions
Imports org.deeplearning4j.spark.parameterserver.functions
Imports SilentTrainingDriver = org.deeplearning4j.spark.parameterserver.networking.v1.SilentTrainingDriver
Imports UpdatesConsumer = org.deeplearning4j.spark.parameterserver.networking.v2.UpdatesConsumer
Imports SparkUtils = org.deeplearning4j.spark.util.SparkUtils
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports ExecutionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports TransportType = org.nd4j.parameterserver.distributed.enums.TransportType
Imports NetworkOrganizer = org.nd4j.parameterserver.distributed.util.NetworkOrganizer
Imports ModelParameterServer = org.nd4j.parameterserver.distributed.v2.ModelParameterServer
Imports Transport = org.nd4j.parameterserver.distributed.v2.transport.Transport
Imports AeronUdpTransport = org.nd4j.parameterserver.distributed.v2.transport.impl.AeronUdpTransport
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.spark.parameterserver.training


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Data public class SharedTrainingMaster extends org.deeplearning4j.spark.impl.paramavg.BaseTrainingMaster<SharedTrainingResult, SharedTrainingWorker> implements TrainingMaster<SharedTrainingResult, SharedTrainingWorker>
	Public Class SharedTrainingMaster
		Inherits BaseTrainingMaster(Of SharedTrainingResult, SharedTrainingWorker)
		Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker)

		'Static counter/id fields used to determine which training master last set up the singleton param servers, etc
		Protected Friend Shared ReadOnly INSTANCE_COUNTER As New AtomicInteger()
		Protected Friend Shared ReadOnly LAST_TRAINING_INSTANCE As New AtomicInteger(-1)

		Protected Friend trainingHooks As IList(Of TrainingHook)
		Protected Friend voidConfiguration As VoidConfiguration

		Protected Friend numWorkers As Integer?
		Protected Friend numWorkersPerNode As Integer?
		Protected Friend workerPrefetchBatches As Integer
		Protected Friend Shadows rddTrainingApproach As RDDTrainingApproach
		Protected Friend Shadows storageLevel As StorageLevel
		Protected Friend repartitioner As Repartitioner

'JAVA TO VB CONVERTER NOTE: The field collectTrainingStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shadows collectTrainingStats_Conflict As Boolean
		Protected Friend rddDataSetNumExamples As Integer
		Protected Friend debugLongerIterations As Long = 0L
		Protected Friend logMinibatchesPerWorker As Boolean = False
		Protected Friend encodingDebugMode As Boolean = False

		Protected Friend thresholdAlgorithm As ThresholdAlgorithm
		Protected Friend residualPostProcessor As ResidualPostProcessor

		Protected Friend Shadows repartition As Repartition
		Protected Friend Shadows repartitionStrategy As RepartitionStrategy

		Protected Friend Shadows stats As ParameterAveragingTrainingMasterStats.ParameterAveragingTrainingMasterStatsHelper

		Protected Friend Shadows rng As Random

		Protected Friend isFirstRun As AtomicBoolean

		' better ignore
		<NonSerialized>
		Protected Friend ReadOnly instanceId As Integer
		<NonSerialized>
		Protected Friend broadcastModel As Broadcast(Of NetBroadcastTuple)
		<NonSerialized>
		Protected Friend broadcastConfiguration As Broadcast(Of SharedTrainingConfiguration)
		<NonSerialized>
		Protected Friend transport As Transport
		<NonSerialized>
		Protected Friend trainingDriver As SilentTrainingDriver

		<NonSerialized>
		Protected Friend updatesConsumer As UpdatesConsumer

		Protected Friend setupDone As Boolean

		Protected Friend Sub New()
			' just a stub for ser/de
			instanceId = INSTANCE_COUNTER.getAndIncrement()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SharedTrainingMaster(@NonNull VoidConfiguration voidConfiguration, System.Nullable<Integer> numWorkers, RDDTrainingApproach rddTrainingApproach, org.apache.spark.storage.StorageLevel storageLevel, boolean collectTrainingStats, RepartitionStrategy repartitionStrategy, Repartition repartition, org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm thresholdAlgorithm, org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor residualPostProcessor, int rddDataSetNumExamples, int batchSizePerWorker, long debugLongerIterations, int numWorkersPerNode, int workerPrefetchBatches, Repartitioner repartitioner, System.Nullable<Boolean> workerTogglePeriodicGC, System.Nullable<Integer> workerPeriodicGCFrequency, boolean encodingDebugMode)
		Public Sub New(ByVal voidConfiguration As VoidConfiguration, ByVal numWorkers As Integer?, ByVal rddTrainingApproach As RDDTrainingApproach, ByVal storageLevel As StorageLevel, ByVal collectTrainingStats As Boolean, ByVal repartitionStrategy As RepartitionStrategy, ByVal repartition As Repartition, ByVal thresholdAlgorithm As ThresholdAlgorithm, ByVal residualPostProcessor As ResidualPostProcessor, ByVal rddDataSetNumExamples As Integer, ByVal batchSizePerWorker As Integer, ByVal debugLongerIterations As Long, ByVal numWorkersPerNode As Integer, ByVal workerPrefetchBatches As Integer, ByVal repartitioner As Repartitioner, ByVal workerTogglePeriodicGC As Boolean?, ByVal workerPeriodicGCFrequency As Integer?, ByVal encodingDebugMode As Boolean)
			Me.voidConfiguration = voidConfiguration
			Me.numWorkers = numWorkers
			Me.thresholdAlgorithm = thresholdAlgorithm
			Me.residualPostProcessor = residualPostProcessor
			Me.rddTrainingApproach = rddTrainingApproach
			Me.repartitionStrategy = repartitionStrategy
			Me.repartition = repartition
			Me.storageLevel = storageLevel
			Me.collectTrainingStats_Conflict = collectTrainingStats
			Me.isFirstRun = New AtomicBoolean(False)
			Me.batchSizePerWorker = batchSizePerWorker
			Me.rddDataSetNumExamples = rddDataSetNumExamples
			Me.debugLongerIterations = debugLongerIterations
			Me.numWorkersPerNode = numWorkersPerNode
			Me.workerPrefetchBatches = workerPrefetchBatches
			Me.repartitioner = repartitioner
			Me.workerTogglePeriodicGC = workerTogglePeriodicGC
			Me.workerPeriodicGCFrequency = workerPeriodicGCFrequency
			Me.encodingDebugMode = encodingDebugMode


			If collectTrainingStats Then
				stats = New ParameterAveragingTrainingMasterStats.ParameterAveragingTrainingMasterStatsHelper()
			End If


			Dim jvmuid As String = UIDProvider.JVMUID
			Me.trainingMasterUID = DateTimeHelper.CurrentUnixTimeMillis() & "_" & (If(jvmuid.Length <= 8, jvmuid, jvmuid.Substring(0, 8)))
			instanceId = INSTANCE_COUNTER.getAndIncrement()
		End Sub

		Public Overrides Sub removeHook(ByVal trainingHook As TrainingHook) Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).removeHook
			If trainingHooks IsNot Nothing Then
				trainingHooks.Remove(trainingHook)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void addHook(@NonNull TrainingHook trainingHook)
		Public Overridable Overloads Sub addHook(ByVal trainingHook As TrainingHook) Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).addHook
			If trainingHooks Is Nothing Then
				trainingHooks = New List(Of TrainingHook)()
			End If

			trainingHooks.Add(trainingHook)
		End Sub

		Public Overrides Function toJson() As String Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).toJson
			Dim om As ObjectMapper = JsonMapper

			Try
				Return om.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception("Error producing JSON representation for ParameterAveragingTrainingMaster", e)
			End Try
		End Function

		Public Overrides Function toYaml() As String Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).toYaml
			Dim om As ObjectMapper = YamlMapper

			Try
				Return om.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception("Error producing YAML representation for ParameterAveragingTrainingMaster", e)
			End Try
		End Function

		''' <summary>
		''' Create a SharedTrainingMaster instance by deserializing a JSON string that has been serialized with
		''' <seealso cref="toJson()"/>
		''' </summary>
		''' <param name="jsonStr"> SharedTrainingMaster configuration serialized as JSON </param>
		Public Shared Function fromJson(ByVal jsonStr As String) As SharedTrainingMaster
			Dim om As ObjectMapper = JsonMapper
			Try
				Return om.readValue(jsonStr, GetType(SharedTrainingMaster))
			Catch e As IOException
				Throw New Exception("Could not parse JSON", e)
			End Try
		End Function

		''' <summary>
		''' Create a SharedTrainingMaster instance by deserializing a YAML string that has been serialized with
		''' <seealso cref="toYaml()"/>
		''' </summary>
		''' <param name="yamlStr"> SharedTrainingMaster configuration serialized as YAML </param>
		Public Shared Function fromYaml(ByVal yamlStr As String) As SharedTrainingMaster
			Dim om As ObjectMapper = YamlMapper
			Try
				Return om.readValue(yamlStr, GetType(SharedTrainingMaster))
			Catch e As IOException
				Throw New Exception("Could not parse YAML", e)
			End Try
		End Function

		Public Overrides Function getWorkerInstance(ByVal network As SparkDl4jMultiLayer) As SharedTrainingWorker
	'        
	'            Here we're going create our worker, which will be passed into corresponding FlatMapFunction
	'         
			Dim tuple As New NetBroadcastTuple(network.Network.LayerWiseConfigurations, network.Network.params(), network.Network.Updater.StateViewArray)

			voidConfiguration.setUnicastControllerPort(voidConfiguration.getPortSupplier().getPort())

			Dim configuration As SharedTrainingConfiguration = SharedTrainingConfiguration.builder().thresholdAlgorithm(thresholdAlgorithm).residualPostProcessor(residualPostProcessor).voidConfiguration(voidConfiguration).debugLongerIterations(debugLongerIterations).numberOfWorkersPerNode(numWorkersPerNode).encodingDebugMode(encodingDebugMode).build()

			If collectTrainingStats_Conflict Then
				stats.logBroadcastStart()
			End If

			If broadcastModel Is Nothing Then
				broadcastModel = network.SparkContext.broadcast(tuple)
			End If

			If broadcastConfiguration Is Nothing Then
				broadcastConfiguration = network.SparkContext.broadcast(configuration)
			End If

			If collectTrainingStats_Conflict Then
				stats.logBroadcastEnd()
			End If

			Dim worker As New SharedTrainingWorker(instanceId, broadcastModel, broadcastConfiguration, listeners, statsStorage, workerTogglePeriodicGC, workerPeriodicGCFrequency)

			Return worker
		End Function

		Public Overrides Function getWorkerInstance(ByVal graph As SparkComputationGraph) As SharedTrainingWorker
			Dim tuple As New NetBroadcastTuple(graph.Network.Configuration, graph.Network.params(), graph.Network.Updater.StateViewArray)

			Dim configuration As SharedTrainingConfiguration = SharedTrainingConfiguration.builder().thresholdAlgorithm(thresholdAlgorithm).residualPostProcessor(residualPostProcessor).voidConfiguration(voidConfiguration).debugLongerIterations(debugLongerIterations).numberOfWorkersPerNode(numWorkersPerNode).prefetchSize(workerPrefetchBatches).encodingDebugMode(encodingDebugMode).build()

			If collectTrainingStats_Conflict Then
				stats.logBroadcastStart()
			End If

			If broadcastModel Is Nothing Then
				broadcastModel = graph.SparkContext.broadcast(tuple)
			End If

			If broadcastConfiguration Is Nothing Then
				broadcastConfiguration = graph.SparkContext.broadcast(configuration)
			End If

			If collectTrainingStats_Conflict Then
				stats.logBroadcastEnd()
			End If

			Dim worker As New SharedTrainingWorker(instanceId, broadcastModel, broadcastConfiguration, listeners, statsStorage, workerTogglePeriodicGC, workerPeriodicGCFrequency)

			Return worker
		End Function

		Protected Friend Overridable Function numObjectsEachWorker(ByVal numExamplesEachRddObject As Integer) As Integer
			Return batchSizePerWorker \ numExamplesEachRddObject
		End Function

		Protected Friend Overridable Function getTotalDataSetObjectCount(Of T, Repr As JavaRDDLike(Of T, Repr))(ByVal trainingData As JavaRDDLike(Of T, Repr)) As Long
			If collectTrainingStats_Conflict Then
				stats.logCountStart()
			End If

			Dim totalDataSetObjectCount As Long = trainingData.count()

			If collectTrainingStats_Conflict Then
				stats.logCountEnd()
			End If

			Return totalDataSetObjectCount
		End Function

		Protected Friend Overridable Sub executeTrainingDirect(ByVal network As SparkDl4jMultiLayer, ByVal trainingData As JavaRDD(Of DataSet))
			If collectTrainingStats_Conflict Then
				stats.logFitStart()
			End If

			'For "vanilla" parameter averaging training, we need to split the full data set into batches of size N, such that we can process the specified
			' number of minibatches between averagings
			'But to do that, wee need to know: (a) the number of examples, and (b) the number of workers
			If storageLevel IsNot Nothing Then
				trainingData.persist(storageLevel)
			End If

			Dim totalDataSetObjectCount As Long = getTotalDataSetObjectCount(trainingData)

			' since this is real distributed training, we don't need to split data
			doIteration(network, trainingData, 1, 1)

			If collectTrainingStats_Conflict Then
				stats.logFitEnd(CInt(totalDataSetObjectCount))
			End If
		End Sub

		Protected Friend Overridable Sub executeTrainingDirectMDS(ByVal network As SparkComputationGraph, ByVal trainingData As JavaRDD(Of MultiDataSet))
			If collectTrainingStats_Conflict Then
				stats.logFitStart()
			End If

			'For "vanilla" parameter averaging training, we need to split the full data set into batches of size N, such that we can process the specified
			' number of minibatches between averagings
			'But to do that, wee need to know: (a) the number of examples, and (b) the number of workers
			If storageLevel IsNot Nothing Then
				trainingData.persist(storageLevel)
			End If

			Dim totalDataSetObjectCount As Long = getTotalDataSetObjectCount(trainingData)

			' since this is real distributed training, we don't need to split data
			doIterationMDS(network, trainingData, 1, 1)

			If collectTrainingStats_Conflict Then
				stats.logFitEnd(CInt(totalDataSetObjectCount))
			End If
		End Sub

		Protected Friend Overridable Sub executeTrainingDirect(ByVal network As SparkComputationGraph, ByVal trainingData As JavaRDD(Of DataSet))
			If collectTrainingStats_Conflict Then
				stats.logFitStart()
			End If

			'For "vanilla" parameter averaging training, we need to split the full data set into batches of size N, such that we can process the specified
			' number of minibatches between averagings
			'But to do that, wee need to know: (a) the number of examples, and (b) the number of workers
			If storageLevel IsNot Nothing Then
				trainingData.persist(storageLevel)
			End If

			Dim totalDataSetObjectCount As Long = getTotalDataSetObjectCount(trainingData)

			' since this is real distributed training, we don't need to split data
			doIteration(network, trainingData, 1, 1)

			If collectTrainingStats_Conflict Then
				stats.logFitEnd(CInt(totalDataSetObjectCount))
			End If
		End Sub


		Public Overridable Overloads Sub executeTrainingPaths(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal trainingDataPaths As JavaRDD(Of String), ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader) Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).executeTrainingPaths
			prepareNetworkAndStuff(network, graph)
			executeTrainingPathsHelper(network, graph, trainingDataPaths, dsLoader, mdsLoader, rddDataSetNumExamples)
		End Sub

		Protected Friend Overridable Sub executeTrainingPathsHelper(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal trainingDataPaths As JavaRDD(Of String), ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader, ByVal dataSetObjectsNumExamples As Integer)

			If numWorkers Is Nothing Then
				If network IsNot Nothing Then
					numWorkers = network.SparkContext.defaultParallelism()
				Else
					numWorkers = graph.SparkContext.defaultParallelism()
				End If
			End If

			If collectTrainingStats_Conflict Then
				stats.logFitStart()
			End If

			If storageLevelStreams IsNot Nothing Then
				trainingDataPaths.persist(storageLevelStreams)
			End If

			Dim totalDataSetObjectCount As Long = getTotalDataSetObjectCount(trainingDataPaths)

			doIterationPaths(network, graph, trainingDataPaths, 1, 1, dsLoader, mdsLoader, dataSetObjectsNumExamples)

			If collectTrainingStats_Conflict Then
				stats.logFitEnd(CInt(totalDataSetObjectCount))
			End If
		End Sub

		Protected Friend Overridable Sub prepareNetworkAndStuff(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph)
			If network Is Nothing AndAlso graph Is Nothing Then
				Throw New System.InvalidOperationException("Both MLN & CG are undefined")
			End If

			'Get the port for communicating with the master/driver - and add it to the configuration for use from each machine
			'Note that each machine will allocate their own port for inbound communications according to what the PortSupplier
			'returns on each worker machine.
			voidConfiguration.setUnicastControllerPort(voidConfiguration.getPortSupplier().getPort())

			' if streamId has default value - generate random one
			If voidConfiguration.getStreamId() < 1 Then
				voidConfiguration.StreamId = RandomUtils.nextInt(119, Integer.MaxValue - 1)
			End If

			' first of all, we're instantiating ParameterServer shard here\
			If numWorkers Is Nothing Then
				numWorkers = If(network IsNot Nothing, network.SparkContext.defaultParallelism(), graph.SparkContext.defaultParallelism())
			End If

			' set current box as controller, if field is unset - switch to next step
			If voidConfiguration.getControllerAddress() Is Nothing Then
				Try
					Dim e As val = Environment.GetEnvironmentVariable("SPARK_PUBLIC_DNS")
					log.info("Trying {SPARK_PUBLIC_DNS}: [{}]", e)
					If e IsNot Nothing Then
						Dim sparkIp As String = InetAddress.getByName(e).getHostAddress()
						voidConfiguration.setControllerAddress(sparkIp)
					End If
				Catch e As UnknownHostException
				End Try
			End If

			' next step - is to get ip address that matches specific network mask
			If voidConfiguration.getControllerAddress() Is Nothing AndAlso voidConfiguration.NetworkMask IsNot Nothing Then
				Dim organizer As New NetworkOrganizer(voidConfiguration.NetworkMask)
				Dim s As val = organizer.MatchingAddress
				log.info("Trying auto-detected address: [{}]", s)

				voidConfiguration.setControllerAddress(s)
			End If

			If voidConfiguration.getControllerAddress() Is Nothing Then
				Dim envVar As String = Environment.GetEnvironmentVariable(DL4JEnvironmentVars.DL4J_VOID_IP)
				If envVar IsNot Nothing AndAlso envVar.Length > 0 Then
					voidConfiguration.setControllerAddress(envVar)
				End If
			End If

			If voidConfiguration.getControllerAddress() Is Nothing Then
				Throw New DL4JInvalidConfigException("Can't get Spark Master local address. Please specify it manually using VoidConfiguration.setControllerAddress(String) method or VoidConfiguration.setNetworkMask(String) method")
			End If

			' we're forcing proper defaults
			log.info("Setting controller address to {}:{}", voidConfiguration.getControllerAddress(), voidConfiguration.getUnicastControllerPort())
			voidConfiguration.setShardAddresses(voidConfiguration.getControllerAddress())
			voidConfiguration.setNumberOfShards(1)

			If network IsNot Nothing Then
				network.Network.init()
			Else
				graph.Network.init()
			End If

			' this instance will be SilentWorker - it'll accept and apply messages, but won't contribute to training. And we init it only once
			If isFirstRun.compareAndSet(False, True) OrElse LAST_TRAINING_INSTANCE.get() <> instanceId Then
				If LAST_TRAINING_INSTANCE.get() >= 0 AndAlso LAST_TRAINING_INSTANCE.get() <> instanceId Then
					log.debug("Detected changed training instance - setting up new parameter server - old instance {}, new instance {}", LAST_TRAINING_INSTANCE, instanceId)

					ModelParameterServer.Instance.shutdown()
					Try 'TODO is this required?
						Thread.Sleep(3000)
					Catch e As Exception
						Throw New Exception(e)
					End Try
				End If

				Dim transport As val = If(voidConfiguration.getTransportType() = TransportType.ROUTED_UDP, New AeronUdpTransport(voidConfiguration.getControllerAddress(), voidConfiguration.getUnicastControllerPort(), voidConfiguration), Nothing)

				If transport Is Nothing Then
					Throw New DL4JInvalidConfigException("No Transport implementation was defined for this training session!")
				End If

				Dim params As val = If(network IsNot Nothing, network.Network.params(), graph.Network.params())

				updatesConsumer = UpdatesConsumer.builder().params(params).updates(Nd4j.create(params.shape(), params.ordering())).stepFunction(If(network IsNot Nothing, network.Network.Optimizer.StepFunction, graph.Network.Optimizer.StepFunction)).build()

				' apply configuration
				ModelParameterServer.Instance.configure(voidConfiguration, transport, True)

				' and attach our consumer
				ModelParameterServer.Instance.addUpdatesSubscriber(updatesConsumer)


				' and start actual server
				If Not ModelParameterServer.Instance.Initialized Then
					ModelParameterServer.Instance.launch()
				End If

				LAST_TRAINING_INSTANCE.set(instanceId)
			End If

			setupDone = True
		End Sub

		Protected Friend Overridable Sub finalizeTraining()
	'        
	'            Here we basically want to do few things:
	'            1) update statistics, if any
	'            2) finalize updates of silent worker
	'            3) pull back gradients, maybe?
	'         

			' applying non-applied updates, if any :)
			If trainingDriver IsNot Nothing Then
				trainingDriver.finishTraining(0L, 0L)
			End If

			' the same, but v2 impl
			If updatesConsumer IsNot Nothing Then
				updatesConsumer.flush()
			End If
		End Sub

		Public Overridable Overloads Sub executeTraining(ByVal network As SparkDl4jMultiLayer, ByVal trainingData As JavaRDD(Of DataSet)) Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).executeTraining
	'        
	'            This method (and other similar methods) is basically one of our entry points, here we'll spawn our training process:
	'            1) broadcast everything needed: initial model params, updaters state, conf. Useful for uptraining
	'            2) shuffle, if needed
	'            3) repartition, if needed
	'            4) EXECUTE SILENT WORKER
	'            5) invoke training function via mapPartitions
	'            6) wait till finished
	'            7) do something with final model, i.e. export it somewhere :)
	'         

			prepareNetworkAndStuff(network, Nothing)

			' at this moment we have coordinator server up (master works as coordinator)
			If rddTrainingApproach = RDDTrainingApproach.Direct Then
				executeTrainingDirect(network, trainingData)
			ElseIf rddTrainingApproach = RDDTrainingApproach.Export Then
				'Export data if required (or, use cached export)
				Dim paths As JavaRDD(Of String) = exportIfRequired(network.SparkContext, trainingData)
				executeTrainingPathsHelper(network, Nothing, paths, New SerializedDataSetLoader(), Nothing, batchSizePerWorker)
			Else
				Throw New DL4JInvalidConfigException("Unknown RDDtrainingApproach [" & rddTrainingApproach & "] was specified!")
			End If
		End Sub

		Public Overridable Overloads Sub executeTraining(ByVal graph As SparkComputationGraph, ByVal trainingData As JavaRDD(Of DataSet)) Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).executeTraining
			prepareNetworkAndStuff(Nothing, graph)

			' at this moment we have coordinator server up (master works as coordinator)
			If rddTrainingApproach = RDDTrainingApproach.Direct Then
				executeTrainingDirect(graph, trainingData)
			ElseIf rddTrainingApproach = RDDTrainingApproach.Export Then
				'Export data if required (or, use cached export)
				Dim paths As JavaRDD(Of String) = exportIfRequired(graph.SparkContext, trainingData)
				executeTrainingPathsHelper(Nothing, graph, paths, New SerializedDataSetLoader(), Nothing, batchSizePerWorker)
			Else
				Throw New DL4JInvalidConfigException("Unknown RDDtrainingApproach [" & rddTrainingApproach & "] was specified!")
			End If
		End Sub

		Public Overridable Overloads Sub executeTrainingMDS(ByVal graph As SparkComputationGraph, ByVal trainingData As JavaRDD(Of MultiDataSet)) Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).executeTrainingMDS
			prepareNetworkAndStuff(Nothing, graph)

			' at this moment we have coordinator server up (master works as coordinator)
			If rddTrainingApproach = RDDTrainingApproach.Direct Then
				executeTrainingDirectMDS(graph, trainingData)
			ElseIf rddTrainingApproach = RDDTrainingApproach.Export Then
				'Export data if required (or, use cached export)
				Dim paths As JavaRDD(Of String) = exportIfRequiredMDS(graph.SparkContext, trainingData)
				executeTrainingPathsHelper(Nothing, graph, paths, Nothing, New SerializedMultiDataSetLoader(), batchSizePerWorker)
			Else
				Throw New DL4JInvalidConfigException("Unknown RDDtrainingApproach [" & rddTrainingApproach & "] was specified!")
			End If
		End Sub

		Public Overrides WriteOnly Property CollectTrainingStats(ByVal collectTrainingStats As Boolean) Implements TrainingMaster.setCollectTrainingStats As Boolean
			Set(ByVal collectTrainingStats As Boolean)
				Me.collectTrainingStats_Conflict = collectTrainingStats
			End Set
		End Property

		Public Overrides ReadOnly Property IsCollectTrainingStats As Boolean Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).getIsCollectTrainingStats
			Get
				Return collectTrainingStats_Conflict
			End Get
		End Property

		Public Overrides ReadOnly Property TrainingStats As SparkTrainingStats Implements TrainingMaster(Of SharedTrainingResult, SharedTrainingWorker).getTrainingStats
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Overloads WriteOnly Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				setListeners(Nothing, listeners)
			End Set
		End Property

		Public Overridable Overloads Sub setListeners(ByVal router As StatsStorageRouter, ByVal listeners As ICollection(Of TrainingListener))
			Me.statsStorage = router
			Me.listeners = (If(listeners Is Nothing, Nothing, New List(Of )(listeners)))
		End Sub


		Protected Friend Overridable Sub processResults(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal results As JavaRDD(Of SharedTrainingResult))
			Preconditions.checkState(network IsNot Nothing OrElse graph IsNot Nothing, "Both MLN & CG are null")
			Preconditions.checkState(setupDone, "Setup was not completed before trying to process results")



			If collectTrainingStats_Conflict Then
				stats.logAggregateStartTime()
			End If

			Dim finalResult As SharedTrainingAccumulationTuple = results.treeAggregate(Nothing, New SharedTrainingAggregateFunction(), New SharedTrainingAccumulationFunction(), 4)
			Dim aggregatedStats As SparkTrainingStats = finalResult.getSparkTrainingStats()
			If collectTrainingStats_Conflict Then
				stats.logAggregationEndTime()
			End If

			'finalizeTraining has to be *after* training has completed, otherwise the RDD (via tree aggregate)
			finalizeTraining()


			If collectTrainingStats_Conflict Then
				stats.logProcessParamsUpdaterStart()
			End If

			If finalResult.getUpdaterStateArray() IsNot Nothing Then

				If finalResult.getAggregationsCount() > 1 Then
					finalResult.getUpdaterStateArray().divi(finalResult.getAggregationsCount())
				End If

				If network IsNot Nothing Then
					If network.Network.Updater IsNot Nothing AndAlso network.Network.Updater.StateViewArray IsNot Nothing Then
						network.Network.Updater.StateViewArray.assign(finalResult.getUpdaterStateArray())
					End If
				Else
					If graph.Network.Updater IsNot Nothing AndAlso graph.Network.Updater.StateViewArray IsNot Nothing Then
						graph.Network.Updater.StateViewArray.assign(finalResult.getUpdaterStateArray())
					End If
				End If
			End If


'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim score As Double = finalResult.getScoreSum() / Math.Max(1, finalResult.getAggregationsCount())

			If network IsNot Nothing Then
				network.Network.Score = score
			Else
				graph.Network.Score = score
			End If

			If collectTrainingStats_Conflict Then
				stats.logProcessParamsUpdaterEnd()
			End If


			If collectTrainingStats_Conflict Then
				stats.logProcessParamsUpdaterEnd()
				stats.addWorkerStats(aggregatedStats)
			End If

			If statsStorage IsNot Nothing Then
				Dim meta As ICollection(Of StorageMetaData) = finalResult.getListenerMetaData()
				If meta IsNot Nothing AndAlso meta.Count > 0 Then
					statsStorage.putStorageMetaData(meta)
				End If

				Dim staticInfo As ICollection(Of Persistable) = finalResult.getListenerStaticInfo()
				If staticInfo IsNot Nothing AndAlso staticInfo.Count > 0 Then
					statsStorage.putStaticInfo(staticInfo)
				End If

				Dim updates As ICollection(Of Persistable) = finalResult.getListenerUpdates()
				If updates IsNot Nothing AndAlso updates.Count > 0 Then
					statsStorage.putUpdate(updates)
				End If
			End If

			If logMinibatchesPerWorker Then
				If finalResult.getMinibatchesPerExecutor() IsNot Nothing Then
					Dim l As IList(Of String) = New List(Of String)(finalResult.getMinibatchesPerExecutor().keySet())
					l.Sort()
					Dim linkedMap As IDictionary(Of String, Integer) = New LinkedHashMap(Of String, Integer)()
					For Each s As String In l
						linkedMap(s) = finalResult.getMinibatchesPerExecutor().get(s)
					Next s
					log.info("Number of minibatches processed per JVM/executor: {}", linkedMap)
				End If
			End If

			If finalResult.getThresholdAlgorithmReducer() IsNot Nothing Then
				'Store the final threshold algorithm after aggregation
				'Some threshold algorithms contain state/history, used to adapt the threshold algorithm
				'The idea is we want to keep this history/state for next epoch, rather than simply throwing it away
				' and starting the threshold adaption process from scratch on each epoch
				Dim ta As ThresholdAlgorithm = finalResult.getThresholdAlgorithmReducer().getFinalResult()
				Me.thresholdAlgorithm = ta
			End If

			Nd4j.Executioner.commit()
		End Sub

		Protected Friend Overridable Sub doIteration(ByVal network As SparkDl4jMultiLayer, ByVal split As JavaRDD(Of DataSet), ByVal splitNum As Integer, ByVal numSplits As Integer)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, thresholdAlgorithm={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, thresholdAlgorithm, numWorkers)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsStart()
			End If

			Dim splitData As JavaRDD(Of DataSet) = split

			If collectTrainingStats_Conflict Then
				stats.logRepartitionStart()
			End If

			If repartitioner IsNot Nothing Then
				log.info("Repartitioning training data using repartitioner: {}", repartitioner)
				Dim minPerWorker As Integer = Math.Max(1, batchSizePerWorker\rddDataSetNumExamples)
				splitData = repartitioner.repartition(splitData, minPerWorker, numWorkers)
			Else
				log.info("Repartitioning training data using SparkUtils repartitioner")
				splitData = SparkUtils.repartitionEqually(splitData, repartition, numWorkers)
			End If
			Dim nPartitions As Integer = splitData.partitions().size()

			If collectTrainingStats_Conflict AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If


			Dim [function] As FlatMapFunction(Of IEnumerator(Of DataSet), SharedTrainingResult) = New SharedFlatMapDataSet(Of IEnumerator(Of DataSet), SharedTrainingResult)(getWorkerInstance(network))

			Dim result As JavaRDD(Of SharedTrainingResult) = splitData.mapPartitions([function])

			processResults(network, Nothing, result)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub

		Protected Friend Overridable Sub doIterationMDS(ByVal network As SparkComputationGraph, ByVal split As JavaRDD(Of MultiDataSet), ByVal splitNum As Integer, ByVal numSplits As Integer)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, thresholdAlgorithm={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, thresholdAlgorithm, numWorkers)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsStart()
			End If

			Dim splitData As JavaRDD(Of MultiDataSet) = split

			If collectTrainingStats_Conflict Then
				stats.logRepartitionStart()
			End If

			If repartitioner IsNot Nothing Then
				log.info("Repartitioning training data using repartitioner: {}", repartitioner)
				Dim minPerWorker As Integer = Math.Max(1, batchSizePerWorker\rddDataSetNumExamples)
				splitData = repartitioner.repartition(splitData, minPerWorker, numWorkers)
			Else
				log.info("Repartitioning training data using SparkUtils repartitioner")
				splitData = SparkUtils.repartitionEqually(splitData, repartition, numWorkers)
			End If
			Dim nPartitions As Integer = splitData.partitions().size()

			If collectTrainingStats_Conflict AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If


			Dim [function] As FlatMapFunction(Of IEnumerator(Of MultiDataSet), SharedTrainingResult) = New SharedFlatMapMultiDataSet(Of IEnumerator(Of MultiDataSet), SharedTrainingResult)(getWorkerInstance(network))

			Dim result As JavaRDD(Of SharedTrainingResult) = splitData.mapPartitions([function])

			processResults(Nothing, network, result)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub

		Protected Friend Overridable Sub doIteration(ByVal network As SparkComputationGraph, ByVal data As JavaRDD(Of DataSet), ByVal splitNum As Integer, ByVal numSplits As Integer)
			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, thresholdAlgorithm={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, thresholdAlgorithm, numWorkers)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsStart()
			End If

			If collectTrainingStats_Conflict Then
				stats.logRepartitionStart()
			End If

			If repartitioner IsNot Nothing Then
				log.info("Repartitioning training data using repartitioner: {}", repartitioner)
				Dim minPerWorker As Integer = Math.Max(1, batchSizePerWorker\rddDataSetNumExamples)
				data = repartitioner.repartition(data, minPerWorker, numWorkers)
			Else
				log.info("Repartitioning training data using SparkUtils repartitioner")
				data = SparkUtils.repartitionEqually(data, repartition, numWorkers)
			End If
			Dim nPartitions As Integer = data.partitions().size()

			If collectTrainingStats_Conflict AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If


			Dim [function] As FlatMapFunction(Of IEnumerator(Of DataSet), SharedTrainingResult) = New SharedFlatMapDataSet(Of IEnumerator(Of DataSet), SharedTrainingResult)(getWorkerInstance(network))

			Dim result As JavaRDD(Of SharedTrainingResult) = data.mapPartitions([function])

			processResults(Nothing, network, result)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub

		Protected Friend Overridable Sub doIterationPaths(ByVal network As SparkDl4jMultiLayer, ByVal graph As SparkComputationGraph, ByVal data As JavaRDD(Of String), ByVal splitNum As Integer, ByVal numSplits As Integer, ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader, ByVal dataSetObjectNumExamples As Integer)
			If network Is Nothing AndAlso graph Is Nothing Then
				Throw New DL4JInvalidConfigException("Both MLN & CompGraph are NULL")
			End If

			log.info("Starting training of split {} of {}. workerMiniBatchSize={}, thresholdAlgorithm={}, Configured for {} workers", splitNum, numSplits, batchSizePerWorker, thresholdAlgorithm, numWorkers)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsStart()
			End If

			If collectTrainingStats_Conflict Then
				stats.logRepartitionStart()
			End If

			If repartitioner IsNot Nothing Then
				log.info("Repartitioning training data using repartitioner: {}", repartitioner)
				Dim minPerWorker As Integer = Math.Max(1, batchSizePerWorker\dataSetObjectNumExamples)
				data = repartitioner.repartition(data, minPerWorker, numWorkers)
			Else
				log.info("Repartitioning training data using SparkUtils repartitioner")
				data = SparkUtils.repartitionEqually(data, repartition, numWorkers)
			End If

			Dim nPartitions As Integer = data.partitions().size()
			If collectTrainingStats_Conflict AndAlso repartition <> Repartition.Never Then
				stats.logRepartitionEnd()
			End If

			Dim sc As JavaSparkContext = (If(network IsNot Nothing, network.SparkContext, graph.SparkContext))
			Dim [function] As FlatMapFunction(Of IEnumerator(Of String), SharedTrainingResult)
			If dsLoader IsNot Nothing Then
				[function] = New SharedFlatMapPaths(Of IEnumerator(Of String), SharedTrainingResult)(If(network IsNot Nothing, getWorkerInstance(network), getWorkerInstance(graph)), dsLoader, BroadcastHadoopConfigHolder.get(sc))
			Else
				[function] = New SharedFlatMapPathsMDS(Of IEnumerator(Of String), SharedTrainingResult)(If(network IsNot Nothing, getWorkerInstance(network), getWorkerInstance(graph)), mdsLoader, BroadcastHadoopConfigHolder.get(sc))
			End If


			Dim result As JavaRDD(Of SharedTrainingResult) = data.mapPartitions([function])

			processResults(network, graph, result)

			If collectTrainingStats_Conflict Then
				stats.logMapPartitionsEnd(nPartitions)
			End If
		End Sub


		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field thresholdAlgorithm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend thresholdAlgorithm_Conflict As ThresholdAlgorithm = New AdaptiveThresholdAlgorithm()
'JAVA TO VB CONVERTER NOTE: The field residualPostProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend residualPostProcessor_Conflict As ResidualPostProcessor = New ResidualClippingPostProcessor(5.0, 5)
			Protected Friend rddDataSetNumExamples As Integer = 1
			<Obsolete>
			Protected Friend repartition As Repartition = Repartition.Always
'JAVA TO VB CONVERTER NOTE: The field repartitionStrategy was renamed since Visual Basic does not allow fields to have the same name as other class members:
			<Obsolete>
			Protected Friend repartitionStrategy_Conflict As RepartitionStrategy = RepartitionStrategy.Balanced
'JAVA TO VB CONVERTER NOTE: The field storageLevel was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend storageLevel_Conflict As StorageLevel = StorageLevel.MEMORY_ONLY_SER()
			Protected Friend voidConfiguration As VoidConfiguration
'JAVA TO VB CONVERTER NOTE: The field rddTrainingApproach was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend rddTrainingApproach_Conflict As RDDTrainingApproach = RDDTrainingApproach.Export
'JAVA TO VB CONVERTER NOTE: The field rngSeed was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend rngSeed_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field exportDirectory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend exportDirectory_Conflict As String = Nothing
			Protected Friend numWorkers As Integer?
'JAVA TO VB CONVERTER NOTE: The field collectTrainingStats was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend collectTrainingStats_Conflict As Boolean
'JAVA TO VB CONVERTER NOTE: The field transport was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend transport_Conflict As Transport
			Protected Friend batchSize As Integer
'JAVA TO VB CONVERTER NOTE: The field debugLongerIterations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend debugLongerIterations_Conflict As Long = 0L
			Protected Friend numWorkersPerNode As Integer = -1
'JAVA TO VB CONVERTER NOTE: The field workerPrefetchNumBatches was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend workerPrefetchNumBatches_Conflict As Integer = 2
'JAVA TO VB CONVERTER NOTE: The field repartitioner was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend repartitioner_Conflict As Repartitioner = New DefaultRepartitioner()
'JAVA TO VB CONVERTER NOTE: The field workerTogglePeriodicGC was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend workerTogglePeriodicGC_Conflict As Boolean? = New Boolean?(True)
'JAVA TO VB CONVERTER NOTE: The field workerPeriodicGCFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend workerPeriodicGCFrequency_Conflict As Integer? = New Integer?(5000)
'JAVA TO VB CONVERTER NOTE: The field encodingDebugMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend encodingDebugMode_Conflict As Boolean = False

			''' <summary>
			''' Create a SharedTrainingMaster with defaults other than the RDD number of examples </summary>
			''' <param name="rddDataSetNumExamples"> When fitting from an {@code RDD<DataSet>} how many examples are in each dataset? </param>
			Public Sub New(ByVal rddDataSetNumExamples As Integer)
				Me.New(New AdaptiveThresholdAlgorithm(), rddDataSetNumExamples)
			End Sub

			''' <summary>
			''' Create a SharedTrainingMaster with defaults other than the RDD number of examples </summary>
			''' <param name="voidConfiguration">     Configuration bean for the SharedTrainingMaster parameter server </param>
			''' <param name="rddDataSetNumExamples"> When fitting from an {@code RDD<DataSet>} how many examples are in each dataset? </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VoidConfiguration voidConfiguration, int rddDataSetNumExamples)
			Public Sub New(ByVal voidConfiguration As VoidConfiguration, ByVal rddDataSetNumExamples As Integer)
				Me.New(voidConfiguration, New AdaptiveThresholdAlgorithm(), rddDataSetNumExamples)
			End Sub

			''' <summary>
			''' Create a SharedTrainingMaster with defaults other than the RDD number of examples </summary>
			''' <param name="thresholdAlgorithm">    Threshold algorithm for the sparse update encoding </param>
			''' <param name="rddDataSetNumExamples"> When fitting from an {@code RDD<DataSet>} how many examples are in each dataset? </param>
			Public Sub New(ByVal thresholdAlgorithm As ThresholdAlgorithm, ByVal rddDataSetNumExamples As Integer)
				Me.New(VoidConfiguration.builder().executionMode(ExecutionMode.MANAGED).forcedRole(NodeRole.SHARD).controllerAddress(Environment.GetEnvironmentVariable("SPARK_PUBLIC_DNS")).build(), thresholdAlgorithm, rddDataSetNumExamples)
			End Sub

			''' <param name="voidConfiguration">     Configuration bean for the SharedTrainingMaster parameter server </param>
			''' <param name="numWorkers">            No longer used/required </param>
			''' <param name="threshold">             Encoding threshold </param>
			''' <param name="rddDataSetNumExamples"> When fitting from an {@code RDD<DataSet>} how many examples are in each dataset? </param>
			''' @deprecated This constructor is deprecated - use <seealso cref="Builder(VoidConfiguration, Integer)"/> or <seealso cref="Builder(VoidConfiguration, ThresholdAlgorithm, Integer)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("This constructor is deprecated - use <seealso cref=""Builder(VoidConfiguration, Integer)""/> or <seealso cref=""Builder(VoidConfiguration, ThresholdAlgorithm, Integer)""/>") public Builder(@NonNull VoidConfiguration voidConfiguration, System.Nullable<Integer> numWorkers, double threshold, int rddDataSetNumExamples)
			<Obsolete("This constructor is deprecated - use <seealso cref=""Builder(VoidConfiguration, Integer)""/> or <seealso cref=""Builder(VoidConfiguration, ThresholdAlgorithm, Integer)""/>")>
			Public Sub New(ByVal voidConfiguration As VoidConfiguration, ByVal numWorkers As Integer?, ByVal threshold As Double, ByVal rddDataSetNumExamples As Integer)
				Me.New(voidConfiguration, New AdaptiveThresholdAlgorithm(threshold), rddDataSetNumExamples)
			End Sub

			''' <param name="voidConfiguration">     Configuration bean for the SharedTrainingMaster parameter server </param>
			''' <param name="thresholdAlgorithm">    Update sharing threshold algorithm </param>
			''' <param name="rddDataSetNumExamples"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VoidConfiguration voidConfiguration, org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm thresholdAlgorithm, int rddDataSetNumExamples)
			Public Sub New(ByVal voidConfiguration As VoidConfiguration, ByVal thresholdAlgorithm As ThresholdAlgorithm, ByVal rddDataSetNumExamples As Integer)
				Me.thresholdAlgorithm_Conflict = thresholdAlgorithm
				Me.voidConfiguration = voidConfiguration
				Me.rddDataSetNumExamples = rddDataSetNumExamples

				' we're enforcing managed mode in all cases here
				Me.voidConfiguration.ExecutionMode = ExecutionMode.MANAGED
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull VoidConfiguration voidConfiguration, System.Nullable<Integer> numWorkers, org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm thresholdAlgorithm, int rddDataSetNumExamples)
			Public Sub New(ByVal voidConfiguration As VoidConfiguration, ByVal numWorkers As Integer?, ByVal thresholdAlgorithm As ThresholdAlgorithm, ByVal rddDataSetNumExamples As Integer)
				Me.thresholdAlgorithm_Conflict = thresholdAlgorithm
				Me.voidConfiguration = voidConfiguration
				Me.rddDataSetNumExamples = rddDataSetNumExamples
				Me.numWorkers = numWorkers

				' we're enforcing managed mode in all cases here
				Me.voidConfiguration.ExecutionMode = ExecutionMode.MANAGED
			End Sub

			''' <summary>
			''' Enable/disable collection of training statistics </summary>
			''' <param name="enable"> Enable
			''' @return </param>
			Public Overridable Function collectTrainingStats(ByVal enable As Boolean) As Builder
				Me.collectTrainingStats_Conflict = enable
				Return Me
			End Function

			''' <summary>
			''' This parameter defines when repartition is applied (if applied). </summary>
			''' <param name="repartition"> Repartition setting </param>
			''' @deprecated Use <seealso cref="repartitioner(Repartitioner)"/> 
			<Obsolete("Use <seealso cref=""repartitioner(Repartitioner)""/>")>
			Public Overridable Function repartitionData(ByVal repartition As Repartition) As Builder
				Me.repartition = repartition
				Return Me
			End Function

			''' <summary>
			''' Used in conjunction with <seealso cref="repartitionData(Repartition)"/> (which defines <i>when</i> repartitioning should be
			''' conducted), repartitionStrategy defines <i>how</i> the repartitioning should be done. See <seealso cref="RepartitionStrategy"/>
			''' for details
			''' </summary>
			''' <param name="repartitionStrategy"> Repartitioning strategy to use </param>
			''' @deprecated Use <seealso cref="repartitioner(Repartitioner)"/> 
'JAVA TO VB CONVERTER NOTE: The parameter repartitionStrategy was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete("Use <seealso cref=""repartitioner(Repartitioner)""/>")>
			Public Overridable Function repartitionStrategy(ByVal repartitionStrategy_Conflict As RepartitionStrategy) As Builder
				Me.repartitionStrategy_Conflict = repartitionStrategy_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the storage level for {@code RDD<DataSet>}s.<br>
			''' Default: StorageLevel.MEMORY_ONLY_SER() - i.e., store in memory, in serialized form<br>
			''' To use no RDD persistence, use {@code null}<br>
			''' Note that this only has effect when {@code RDDTrainingApproach.Direct} is used (which is not the default),
			''' and when fitting from an {@code RDD<DataSet>}.
			''' <para>
			''' <b>Note</b>: Spark's StorageLevel.MEMORY_ONLY() and StorageLevel.MEMORY_AND_DISK() can be problematic when
			''' it comes to off-heap data (which DL4J/ND4J uses extensively). Spark does not account for off-heap memory
			''' when deciding if/when to drop blocks to ensure enough free memory; consequently, for DataSet RDDs that are
			''' larger than the total amount of (off-heap) memory, this can lead to OOM issues. Put another way: Spark counts
			''' the on-heap size of DataSet and INDArray objects only (which is negligible) resulting in a significant
			''' underestimate of the true DataSet object sizes. More DataSets are thus kept in memory than we can really afford.<br>
			''' <br>
			''' Note also that fitting directly from an {@code RDD<DataSet>} is discouraged - it is better to export your
			''' prepared data once and call (for example} {@code SparkDl4jMultiLayer.fit(String savedDataDirectory)}.
			''' See DL4J's Spark website documentation for details.<br>
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
			''' The approach to use when training on a {@code RDD<DataSet>} or {@code RDD<MultiDataSet>}.
			''' Default: <seealso cref="RDDTrainingApproach.Export"/>, which exports data to a temporary directory first.<br>
			''' The default cluster temporary directory is used, though can be configured using <seealso cref="exportDirectory(String)"/>
			''' Note also that fitting directly from an {@code RDD<DataSet>} is discouraged - it is better to export your
			''' prepared data once and call (for example} {@code SparkDl4jMultiLayer.fit(String savedDataDirectory)}.
			''' See DL4J's Spark website documentation for details.<br>
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
			''' Random number generator seed, used mainly for enforcing repeatable splitting/repartitioning on RDDs
			''' Default: no seed set (i.e., random seed)
			''' </summary>
			''' <param name="rngSeed"> RNG seed </param>
'JAVA TO VB CONVERTER NOTE: The parameter rngSeed was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function rngSeed(ByVal rngSeed_Conflict As Long) As Builder
				Me.rngSeed_Conflict = rngSeed_Conflict
				Return Me
			End Function

			''' @deprecated Use <seealso cref="thresholdAlgorithm(ThresholdAlgorithm)"/> with (for example) <seealso cref="AdaptiveThresholdAlgorithm"/> 
'JAVA TO VB CONVERTER NOTE: The parameter updatesThreshold was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			<Obsolete("Use <seealso cref=""thresholdAlgorithm(ThresholdAlgorithm)""/> with (for example) <seealso cref=""AdaptiveThresholdAlgorithm""/>")>
			Public Overridable Function updatesThreshold(ByVal updatesThreshold_Conflict As Double) As Builder
				Return thresholdAlgorithm(New AdaptiveThresholdAlgorithm(updatesThreshold_Conflict))
			End Function

			''' <summary>
			''' Algorithm to use to determine the threshold for updates encoding. Lower values might improve convergence, but
			''' increase amount of network communication<br>
			''' Values that are too low may also impact network convergence. If convergence problems are observed, try increasing
			''' or decreasing this by a factor of 10 - say 1e-4 and 1e-2.<br>
			''' For technical details, see the paper <a href="https://s3-us-west-2.amazonaws.com/amazon.jobs-public-documents/strom_interspeech2015.pdf">
			''' Scalable Distributed DNN Training Using Commodity GPU Cloud Computing</a><br>
			''' See also <seealso cref="ThresholdAlgorithm"/><br><br>
			''' Default: <seealso cref="AdaptiveThresholdAlgorithm"/> with default parameters </summary>
			''' <param name="thresholdAlgorithm"> Threshold algorithm to use to determine encoding threshold </param>
'JAVA TO VB CONVERTER NOTE: The parameter thresholdAlgorithm was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function thresholdAlgorithm(ByVal thresholdAlgorithm_Conflict As ThresholdAlgorithm) As Builder
				Me.thresholdAlgorithm_Conflict = thresholdAlgorithm_Conflict
				Return Me
			End Function

			''' <summary>
			''' Residual post processor. See <seealso cref="ResidualPostProcessor"/> for details.
			''' 
			''' Default: {@code new ResidualClippingPostProcessor(5.0, 5)} - i.e., a <seealso cref="ResidualClippingPostProcessor"/>
			''' that clips the residual to +/- 5x current threshold, every 5 iterations.
			''' </summary>
			''' <param name="residualPostProcessor"> Residual post processor to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter residualPostProcessor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function residualPostProcessor(ByVal residualPostProcessor_Conflict As ResidualPostProcessor) As Builder
				Me.residualPostProcessor_Conflict = residualPostProcessor_Conflict
				Return Me
			End Function

			''' <summary>
			''' Minibatch size to use when training workers. In principle, the source data (i.e., {@code RDD<DataSet>} etc)
			''' can have a different number of examples in each {@code DataSet} than we want to use when training.
			''' i.e., we can split or combine DataSets if required.
			''' </summary>
			''' <param name="batchSize"> Minibatch size to use when fitting each worker </param>
			Public Overridable Function batchSizePerWorker(ByVal batchSize As Integer) As Builder
				Me.batchSize = batchSize
				Return Me
			End Function

			''' <summary>
			''' This method allows to configure number of network training threads per cluster node.<br>
			''' Default value: -1, which defines automated number of workers selection, based on hardware present in system
			''' (i.e., number of GPUs, if training on a GPU enabled system).
			''' <br>
			''' When training on GPUs, you should use 1 worker per GPU (which is the default). For CPUs, 1 worker per
			''' node is usually preferred, though multi-CPU (i.e., multiple physical CPUs) or CPUs with large core counts
			''' may have better throughput (i.e., more examples per second) when increasing the number of workers,
			''' at the expense of more memory consumed. Note that if you increase the number of workers on a CPU system,
			''' you should set the number of OpenMP threads using the {@code OMP_NUM_THREADS} property - see
			''' <seealso cref="ND4JEnvironmentVars.OMP_NUM_THREADS"/> for more details.
			''' For example, a machine with 32 physical cores could use 4 workers with {@code OMP_NUM_THREADS=8}
			''' </summary>
			''' <param name="numWorkers"> Number of workers on each node. </param>
			Public Overridable Function workersPerNode(ByVal numWorkers As Integer) As Builder
				If numWorkers < 1 Then
					numWorkers = -1
				End If

				Me.numWorkersPerNode = numWorkers
				Return Me
			End Function

			''' <summary>
			''' This method allows you to artificially extend iteration time using Thread.sleep() for a given time.
			''' 
			''' PLEASE NOTE: Never use that option in production environment. It's suited for debugging purposes only.
			''' </summary>
			''' <param name="timeMs">
			''' @return </param>
			<Obsolete>
			Public Overridable Function debugLongerIterations(ByVal timeMs As Long) As Builder
				If timeMs < 0 Then
					timeMs = 0L
				End If
				Me.debugLongerIterations_Conflict = timeMs
				Return Me
			End Function

			''' <summary>
			''' Optional method: Transport implementation to be used as TransportType.CUSTOM for VoidParameterAveraging method<br>
			''' Generally not used by users
			''' </summary>
			''' <param name="transport"> Transport to use
			''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter transport was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function transport(ByVal transport_Conflict As Transport) As Builder
				Me.transport_Conflict = transport_Conflict
				Return Me
			End Function

			''' <summary>
			''' Number of minibatches to asynchronously prefetch on each worker when training. Default: 2, which is usually suitable
			''' in most cases. Increasing this might help in some cases of ETL (data loading) bottlenecks, at the expense
			''' of greater memory consumption </summary>
			''' <param name="prefetchNumBatches"> Number of batches to prefetch </param>
			Public Overridable Function workerPrefetchNumBatches(ByVal prefetchNumBatches As Integer) As Builder
				Me.workerPrefetchNumBatches_Conflict = prefetchNumBatches
				Return Me
			End Function

			''' <summary>
			''' Repartitioner to use to repartition data before fitting.<br>
			''' DL4J performs a MapPartitions operation for training, hence how the data is partitioned can matter a lot for
			''' performance - too few partitions (or very imbalanced partitions can result in poor cluster utilization, due to
			''' some workers being idle. A larger number of smaller partitions can help to avoid so-called "end-of-epoch"
			''' effects where training can only complete once the last/slowest worker finishes it's partition.<br>
			''' Default repartitioner is <seealso cref="DefaultRepartitioner"/>, which repartitions equally up to a maximum of 5000
			''' partitions, and is usually suitable for most purposes. In the worst case, the "end of epoch" effect
			''' when using the partitioner should be limited to a maximum of the amount of time required to process a single partition.
			''' </summary>
			''' <param name="repartitioner"> Repartitioner to use </param>
'JAVA TO VB CONVERTER NOTE: The parameter repartitioner was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function repartitioner(ByVal repartitioner_Conflict As Repartitioner) As Builder
				Me.repartitioner_Conflict = repartitioner_Conflict
				Return Me
			End Function

			''' <summary>
			''' Used to disable the periodic garbage collection calls on the workers.<br>
			''' Equivalent to {@code Nd4j.getMemoryManager().togglePeriodicGc(workerTogglePeriodicGC);}<br>
			''' Pass false to disable periodic GC on the workers or true (equivalent to the default, or not setting it) to keep it enabled.
			''' </summary>
			''' <param name="workerTogglePeriodicGC"> Worker periodic garbage collection setting </param>
'JAVA TO VB CONVERTER NOTE: The parameter workerTogglePeriodicGC was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workerTogglePeriodicGC(ByVal workerTogglePeriodicGC_Conflict As Boolean) As Builder
				Me.workerTogglePeriodicGC_Conflict = workerTogglePeriodicGC_Conflict
				Return Me
			End Function

			''' <summary>
			''' Used to set the periodic garbage collection frequency on the workers.<br>
			''' Equivalent to calling {@code Nd4j.getMemoryManager().setAutoGcWindow(workerPeriodicGCFrequency);} on each worker<br>
			''' Does not have any effect if <seealso cref="workerTogglePeriodicGC(Boolean)"/> is set to false
			''' </summary>
			''' <param name="workerPeriodicGCFrequency"> The periodic GC frequency to use on the workers </param>
'JAVA TO VB CONVERTER NOTE: The parameter workerPeriodicGCFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function workerPeriodicGCFrequency(ByVal workerPeriodicGCFrequency_Conflict As Integer) As Builder
				Me.workerPeriodicGCFrequency_Conflict = workerPeriodicGCFrequency_Conflict
				Return Me
			End Function

			''' <summary>
			''' Enable debug mode for threshold encoding. When enabled, various statistics for the threshold and the residual
			''' will be calculated and logged on each worker (at info log level).<br>
			''' This information can be used to check if the encoding threshold is too big (for example, virtually all updates
			''' are much smaller than the threshold) or too big (majority of updates are much larger than the threshold).<br>
			''' encodingDebugMode is disabled by default.<br>
			''' <b>IMPORTANT</b>: enabling this has a performance overhead, and should not be enabled unless the debug information is actually required.<br>
			''' </summary>
			''' <param name="enabled"> True to enable </param>
			Public Overridable Function encodingDebugMode(ByVal enabled As Boolean) As Builder
				Me.encodingDebugMode_Conflict = enabled
				Return Me
			End Function

			Public Overridable Function build() As SharedTrainingMaster
				Dim master As New SharedTrainingMaster(voidConfiguration, numWorkers, rddTrainingApproach_Conflict, storageLevel_Conflict, collectTrainingStats_Conflict, repartitionStrategy_Conflict, repartition, thresholdAlgorithm_Conflict, residualPostProcessor_Conflict, rddDataSetNumExamples, batchSize, debugLongerIterations_Conflict, numWorkersPerNode, workerPrefetchNumBatches_Conflict, repartitioner_Conflict, workerTogglePeriodicGC_Conflict, workerPeriodicGCFrequency_Conflict, encodingDebugMode_Conflict)
				If transport_Conflict IsNot Nothing Then
					master.transport = Me.transport_Conflict
				End If

				Return master
			End Function
		End Class
	End Class

End Namespace