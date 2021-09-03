Imports System.Collections.Generic
Imports val = lombok.val
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StatsStorageRouterProvider = org.deeplearning4j.core.storage.StatsStorageRouterProvider
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports Model = org.deeplearning4j.nn.api.Model
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports ComputationGraphUtil = org.deeplearning4j.nn.graph.util.ComputationGraphUtil
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports MultiLayerUpdater = org.deeplearning4j.nn.updater.MultiLayerUpdater
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports TrainingHook = org.deeplearning4j.spark.api.TrainingHook
Imports WorkerConfiguration = org.deeplearning4j.spark.api.WorkerConfiguration
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports NetBroadcastTuple = org.deeplearning4j.spark.api.worker.NetBroadcastTuple
Imports VanillaStatsStorageRouter = org.deeplearning4j.spark.impl.listeners.VanillaStatsStorageRouter
Imports ParameterAveragingTrainingWorkerStats = org.deeplearning4j.spark.impl.paramavg.stats.ParameterAveragingTrainingWorkerStats
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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


	Public Class ParameterAveragingTrainingWorker
		Inherits BaseTrainingWorker(Of ParameterAveragingTrainingResult)

		Private ReadOnly broadcast As Broadcast(Of NetBroadcastTuple)
		Private ReadOnly saveUpdater As Boolean
		Private trainingHooks As ICollection(Of TrainingHook)
		Private ReadOnly configuration As WorkerConfiguration
		Private stats As ParameterAveragingTrainingWorkerStats.ParameterAveragingTrainingWorkerStatsHelper = Nothing
		Private trainingListeners As ICollection(Of TrainingListener)
		Private listenerRouterProvider As StatsStorageRouterProvider

		Public Sub New(ByVal broadcast As Broadcast(Of NetBroadcastTuple), ByVal saveUpdater As Boolean, ByVal configuration As WorkerConfiguration, ByVal trainingHooks As ICollection(Of TrainingHook), ByVal listeners As ICollection(Of TrainingListener), ByVal routerProvider As StatsStorageRouterProvider)

			Me.broadcast = broadcast
			Me.saveUpdater = saveUpdater
			Me.configuration = configuration
			Me.trainingHooks = trainingHooks
			Me.trainingListeners = listeners
			Me.listenerRouterProvider = routerProvider
		End Sub

		''' <summary>
		''' Remove a training hook from the worker
		''' </summary>
		''' <param name="trainingHook"> the training hook to remove </param>
		Public Overrides Sub removeHook(ByVal trainingHook As TrainingHook)
			If trainingHooks Is Nothing Then
				Return
			End If
			trainingHooks.remove(trainingHook)
		End Sub

		''' <summary>
		''' Add a training hook to be used
		''' during training of the worker
		''' </summary>
		''' <param name="trainingHook"> the training hook to add </param>
		Public Overrides Sub addHook(ByVal trainingHook As TrainingHook)
			If trainingHooks Is Nothing Then
				trainingHooks = New List(Of TrainingHook)()
			End If
			trainingHooks.Add(trainingHook)
		End Sub

		Public Overrides ReadOnly Property InitialModel As MultiLayerNetwork
			Get
				If configuration.isCollectTrainingStats() Then
					stats = New ParameterAveragingTrainingWorkerStats.ParameterAveragingTrainingWorkerStatsHelper()
				End If
    
				If configuration.isCollectTrainingStats() Then
					stats.logBroadcastGetValueStart()
				End If
				Dim tuple As NetBroadcastTuple = broadcast.getValue()
				If configuration.isCollectTrainingStats() Then
					stats.logBroadcastGetValueEnd()
				End If
    
				'Don't want to have shared configuration object: each may update its iteration count (for LR schedule etc) individually
				Dim net As New MultiLayerNetwork(tuple.getConfiguration().clone())
				'Can't have shared parameter array across executors for parameter averaging, hence the 'true' for clone parameters array arg
				net.init(tuple.getParameters().unsafeDuplication(), False)
    
				If tuple.getUpdaterState() IsNot Nothing Then
					net.Updater = New MultiLayerUpdater(net, tuple.getUpdaterState().unsafeDuplication()) 'Can't have shared updater state
				End If
    
				Nd4j.Executioner.commit()
    
				configureListeners(net, tuple.getCounter().getAndIncrement())
    
				If configuration.isCollectTrainingStats() Then
					stats.logInitEnd()
				End If
    
				Return net
			End Get
		End Property

		Public Overrides ReadOnly Property InitialModelGraph As ComputationGraph
			Get
				If configuration.isCollectTrainingStats() Then
					stats = New ParameterAveragingTrainingWorkerStats.ParameterAveragingTrainingWorkerStatsHelper()
				End If
    
				If configuration.isCollectTrainingStats() Then
					stats.logBroadcastGetValueStart()
				End If
				Dim tuple As NetBroadcastTuple = broadcast.getValue()
				If configuration.isCollectTrainingStats() Then
					stats.logBroadcastGetValueEnd()
				End If
    
				'Don't want to have shared configuration object: each may update its iteration count (for LR schedule etc) individually
				Dim net As New ComputationGraph(tuple.getGraphConfiguration().clone())
				'Can't have shared parameter array across executors for parameter averaging, hence the 'true' for clone parameters array arg
				net.init(tuple.getParameters().unsafeDuplication(), False)
    
				If tuple.getUpdaterState() IsNot Nothing Then
					net.Updater = New ComputationGraphUpdater(net, tuple.getUpdaterState().unsafeDuplication()) 'Again: can't have shared updater state
				End If
    
				Nd4j.Executioner.commit()
    
				configureListeners(net, tuple.getCounter().getAndIncrement())
    
				If configuration.isCollectTrainingStats() Then
					stats.logInitEnd()
				End If
    
				Return net
			End Get
		End Property

		Private Sub configureListeners(ByVal m As Model, ByVal counter As Integer)
			If trainingListeners IsNot Nothing Then
				Dim list As IList(Of TrainingListener) = New List(Of TrainingListener)(trainingListeners.Count)
				For Each l As TrainingListener In trainingListeners
					If listenerRouterProvider IsNot Nothing AndAlso TypeOf l Is RoutingIterationListener Then
						Dim rl As RoutingIterationListener = DirectCast(l, RoutingIterationListener)
						rl.StorageRouter = listenerRouterProvider.Router
						Dim workerID As String = UIDProvider.JVMUID & "_" & counter
						rl.WorkerID = workerID
					End If
					list.Add(l) 'Don't need to clone listeners: not from broadcast, so deserialization handles
				Next l
				If TypeOf m Is MultiLayerNetwork Then
					DirectCast(m, MultiLayerNetwork).setListeners(list)
				Else
					DirectCast(m, ComputationGraph).setListeners(list)
				End If
			End If
		End Sub

		Public Overrides Function processMinibatch(ByVal dataSet As DataSet, ByVal network As MultiLayerNetwork, ByVal isLast As Boolean) As ParameterAveragingTrainingResult
			If configuration.isCollectTrainingStats() Then
				stats.logFitStart()
			End If

			If trainingHooks IsNot Nothing Then
				For Each trainingHook As TrainingHook In trainingHooks
					trainingHook.preUpdate(dataSet, network)
				Next trainingHook
			End If

			network.fit(dataSet)

			If trainingHooks IsNot Nothing Then
				For Each trainingHook As TrainingHook In trainingHooks
					trainingHook.postUpdate(dataSet, network)
				Next trainingHook
			End If


			If configuration.isCollectTrainingStats() Then
				stats.logFitEnd(dataSet.numExamples())
			End If

			Nd4j.Executioner.commit()

			If isLast Then
				Dim result As val = getFinalResult(network)

				' releasing Context here
	'            Nd4j.getMemoryManager().releaseCurrentContext();

				Return result
			End If

			' releasing Context here
	'        Nd4j.getMemoryManager().releaseCurrentContext();

			Return Nothing
		End Function

		Public Overrides Function processMinibatch(ByVal dataSet As DataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As ParameterAveragingTrainingResult
			Return processMinibatch(ComputationGraphUtil.toMultiDataSet(dataSet), graph, isLast)
		End Function

		Public Overrides Function processMinibatch(ByVal dataSet As MultiDataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As ParameterAveragingTrainingResult
			If configuration.isCollectTrainingStats() Then
				stats.logFitStart()
			End If
			'pre training hooks
			If trainingHooks IsNot Nothing Then
				For Each trainingHook As TrainingHook In trainingHooks
					trainingHook.preUpdate(dataSet, graph)
				Next trainingHook
			End If

			graph.fit(dataSet)

			'post training hooks
			If trainingHooks IsNot Nothing Then
				For Each trainingHook As TrainingHook In trainingHooks
					trainingHook.postUpdate(dataSet, graph)
				Next trainingHook
			End If
			If configuration.isCollectTrainingStats() Then
				stats.logFitEnd(dataSet.getFeatures(0).size(0))
			End If

			Nd4j.Executioner.commit()

			If isLast Then
				Dim result As val = getFinalResult(graph)

				' releasing Context here
	'            Nd4j.getMemoryManager().releaseCurrentContext();

				Return result
			End If

			' releasing Context here
	'        Nd4j.getMemoryManager().releaseCurrentContext();

			Return Nothing
		End Function


		Public Overrides Function processMinibatchWithStats(ByVal dataSet As DataSet, ByVal network As MultiLayerNetwork, ByVal isLast As Boolean) As Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)
			Dim result As ParameterAveragingTrainingResult = processMinibatch(dataSet, network, isLast)
			If result Is Nothing Then
				Return Nothing
			End If

			Dim statsToReturn As SparkTrainingStats = (If(stats IsNot Nothing, stats.build(), Nothing))
			Return New Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)(result, statsToReturn)
		End Function

		Public Overrides Function processMinibatchWithStats(ByVal dataSet As DataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)
			Return processMinibatchWithStats(ComputationGraphUtil.toMultiDataSet(dataSet), graph, isLast)
		End Function

		Public Overrides Function processMinibatchWithStats(ByVal dataSet As MultiDataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)
			Dim result As ParameterAveragingTrainingResult = processMinibatch(dataSet, graph, isLast)
			If result Is Nothing Then
				Return Nothing
			End If

			Dim statsToReturn As SparkTrainingStats = (If(stats IsNot Nothing, stats.build(), Nothing))
			Return New Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)(result, statsToReturn)
		End Function

		Public Overrides Function getFinalResult(ByVal network As MultiLayerNetwork) As ParameterAveragingTrainingResult
			Dim updaterState As INDArray = Nothing
			If saveUpdater Then
				Dim u As Updater = network.Updater
				If u IsNot Nothing Then
					updaterState = u.StateViewArray
				End If
			End If

			Nd4j.Executioner.commit()

			Dim storageMetaData As ICollection(Of StorageMetaData) = Nothing
			Dim listenerStaticInfo As ICollection(Of Persistable) = Nothing
			Dim listenerUpdates As ICollection(Of Persistable) = Nothing
			If listenerRouterProvider IsNot Nothing Then
				Dim r As StatsStorageRouter = listenerRouterProvider.Router
				If TypeOf r Is VanillaStatsStorageRouter Then 'TODO this is ugly... need to find a better solution
					Dim ssr As VanillaStatsStorageRouter = DirectCast(r, VanillaStatsStorageRouter)
					storageMetaData = ssr.getStorageMetaData()
					listenerStaticInfo = ssr.getStaticInfo()
					listenerUpdates = ssr.getUpdates()
				End If
			End If
			Return New ParameterAveragingTrainingResult(network.params(), updaterState, network.score(), storageMetaData, listenerStaticInfo, listenerUpdates)
		End Function

		Public Overrides Function getFinalResult(ByVal network As ComputationGraph) As ParameterAveragingTrainingResult
			Dim updaterState As INDArray = Nothing
			If saveUpdater Then
				Dim u As ComputationGraphUpdater = network.Updater
				If u IsNot Nothing Then
					updaterState = u.StateViewArray
				End If
			End If

			Nd4j.Executioner.commit()

			Dim storageMetaData As ICollection(Of StorageMetaData) = Nothing
			Dim listenerStaticInfo As ICollection(Of Persistable) = Nothing
			Dim listenerUpdates As ICollection(Of Persistable) = Nothing
			If listenerRouterProvider IsNot Nothing Then
				Dim r As StatsStorageRouter = listenerRouterProvider.Router
				If TypeOf r Is VanillaStatsStorageRouter Then 'TODO this is ugly... need to find a better solution
					Dim ssr As VanillaStatsStorageRouter = DirectCast(r, VanillaStatsStorageRouter)
					storageMetaData = ssr.getStorageMetaData()
					listenerStaticInfo = ssr.getStaticInfo()
					listenerUpdates = ssr.getUpdates()
				End If
			End If

			Return New ParameterAveragingTrainingResult(network.params(), updaterState, network.score(), storageMetaData, listenerStaticInfo, listenerUpdates)
		End Function

		Public Overrides ReadOnly Property FinalResultNoData As ParameterAveragingTrainingResult
			Get
				Return New ParameterAveragingTrainingResult(Nothing, Nothing, 0.0, Nothing, Nothing, Nothing)
			End Get
		End Property

		Public Overrides ReadOnly Property FinalResultNoDataWithStats As Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)
			Get
				Return New Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)(FinalResultNoData, Nothing)
			End Get
		End Property

		Public Overrides Function getFinalResultWithStats(ByVal network As MultiLayerNetwork) As Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)
			Dim result As ParameterAveragingTrainingResult = getFinalResult(network)
			If result Is Nothing Then
				Return Nothing
			End If

			Dim statsToReturn As SparkTrainingStats = (If(stats IsNot Nothing, stats.build(), Nothing))
			Return New Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)(result, statsToReturn)
		End Function

		Public Overrides Function getFinalResultWithStats(ByVal graph As ComputationGraph) As Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)
			Dim result As ParameterAveragingTrainingResult = getFinalResult(graph)
			If result Is Nothing Then
				Return Nothing
			End If

			Dim statsToReturn As SparkTrainingStats = (If(stats IsNot Nothing, stats.build(), Nothing))
			Return New Pair(Of ParameterAveragingTrainingResult, SparkTrainingStats)(result, statsToReturn)
		End Function

		Public Overrides ReadOnly Property DataConfiguration As WorkerConfiguration
			Get
				Return configuration
			End Get
		End Property

		Public Overrides ReadOnly Property InstanceId As Long
			Get
				'Not used for parameter averaging
				Return 0
			End Get
		End Property


	End Class

End Namespace