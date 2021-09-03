Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports TrainingHook = org.deeplearning4j.spark.api.TrainingHook
Imports org.deeplearning4j.spark.api
Imports WorkerConfiguration = org.deeplearning4j.spark.api.WorkerConfiguration
Imports SparkTrainingStats = org.deeplearning4j.spark.api.stats.SparkTrainingStats
Imports NetBroadcastTuple = org.deeplearning4j.spark.api.worker.NetBroadcastTuple
Imports org.deeplearning4j.spark.impl.paramavg
Imports SharedTrainingConfiguration = org.deeplearning4j.spark.parameterserver.conf.SharedTrainingConfiguration
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

Namespace org.deeplearning4j.spark.parameterserver.training


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter public class SharedTrainingWorker extends org.deeplearning4j.spark.impl.paramavg.BaseTrainingWorker<SharedTrainingResult> implements org.deeplearning4j.spark.api.TrainingWorker<SharedTrainingResult>
	Public Class SharedTrainingWorker
		Inherits BaseTrainingWorker(Of SharedTrainingResult)
		Implements TrainingWorker(Of SharedTrainingResult)

		Private ReadOnly instanceId As Long
		Private ReadOnly broadcastModel As Broadcast(Of NetBroadcastTuple)
		Private ReadOnly broadcastConfiguration As Broadcast(Of SharedTrainingConfiguration)
		Private ReadOnly listeners As IList(Of TrainingListener)
		Private ReadOnly router As StatsStorageRouter
		Private ReadOnly workerTogglePeriodicGC As Boolean?
		Private ReadOnly workerPeriodicGCFrequency As Integer?

		Public Sub New(ByVal instanceId As Long, ByVal broadcastModel As Broadcast(Of NetBroadcastTuple), ByVal broadcastConfiguration As Broadcast(Of SharedTrainingConfiguration), ByVal listeners As IList(Of TrainingListener), ByVal router As StatsStorageRouter, ByVal workerTogglePeriodicGC As Boolean?, ByVal workerPeriodicGCFrequency As Integer?)
			Me.instanceId = instanceId
			' our initial model is stored here.
			Me.broadcastModel = broadcastModel
			Me.broadcastConfiguration = broadcastConfiguration
			Me.listeners = listeners
			Me.router = router
			Me.workerTogglePeriodicGC = workerTogglePeriodicGC
			Me.workerPeriodicGCFrequency = workerPeriodicGCFrequency
		End Sub

		Public Overrides Sub removeHook(ByVal trainingHook As TrainingHook)
			Throw New System.NotSupportedException()
		End Sub

		Public Overrides Sub addHook(ByVal trainingHook As TrainingHook)
			Throw New System.NotSupportedException()
		End Sub

		Public Overrides ReadOnly Property InitialModel As MultiLayerNetwork Implements TrainingWorker(Of SharedTrainingResult).getInitialModel
			Get
				If workerTogglePeriodicGC IsNot Nothing Then
					Nd4j.MemoryManager.togglePeriodicGc(workerTogglePeriodicGC)
				End If
				If workerPeriodicGCFrequency IsNot Nothing Then
					Nd4j.MemoryManager.AutoGcWindow = workerPeriodicGCFrequency
				End If
    
				' This method will be called ONLY once, in master thread
				'Before getting NetBroadcastTuple, to ensure it always gets mapped to device 0
				Nd4j.AffinityManager.unsafeSetDevice(0)
    
				Dim tuple As NetBroadcastTuple = broadcastModel.getValue()
				If tuple.getConfiguration() IsNot Nothing Then
					Dim conf As MultiLayerConfiguration = tuple.getConfiguration()
					Dim network As New MultiLayerNetwork(conf)
					network.init()
    
					If tuple.getParameters() IsNot Nothing Then
						network.Params = tuple.getParameters()
					End If
    
					' we can assign properly, without
					If tuple.getUpdaterState() IsNot Nothing Then
						network.Updater.StateViewArray.assign(tuple.getUpdaterState())
					End If
    
					Return network
				Else
					Return Nothing
				End If
			End Get
		End Property

		Public Overrides ReadOnly Property InitialModelGraph As ComputationGraph Implements TrainingWorker(Of SharedTrainingResult).getInitialModelGraph
			Get
				'Before getting NetBroadcastTuple, to ensure it always gets mapped to device 0
				Nd4j.AffinityManager.unsafeSetDevice(0)
				Dim tuple As NetBroadcastTuple = broadcastModel.getValue()
				If tuple.getGraphConfiguration() IsNot Nothing Then
					Dim conf As ComputationGraphConfiguration = tuple.getGraphConfiguration()
					Dim network As New ComputationGraph(conf)
					network.init()
    
					If tuple.getParameters() IsNot Nothing Then
						network.Params = tuple.getParameters()
					End If
    
					If tuple.getUpdaterState() IsNot Nothing Then
						network.Updater.getUpdaterStateViewArray().assign(tuple.getUpdaterState())
					End If
    
					Return network
				Else
					Return Nothing
				End If
			End Get
		End Property

		Public Overrides Function processMinibatch(ByVal dataSet As DataSet, ByVal network As MultiLayerNetwork, ByVal isLast As Boolean) As SharedTrainingResult
	'        
	'            We're not really going to use this method for training.
	'            Partitions will be mapped to ParallelWorker threads dynamically, wrt thread/device affinity.
	'            So plan is simple: we're going to use individual partitions to feed main worker
	'         
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function processMinibatch(ByVal dataSet As DataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As SharedTrainingResult
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function processMinibatch(ByVal dataSet As MultiDataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As SharedTrainingResult
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function processMinibatchWithStats(ByVal dataSet As DataSet, ByVal network As MultiLayerNetwork, ByVal isLast As Boolean) As Pair(Of SharedTrainingResult, SparkTrainingStats) Implements TrainingWorker(Of SharedTrainingResult).processMinibatchWithStats
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function processMinibatchWithStats(ByVal dataSet As DataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As Pair(Of SharedTrainingResult, SparkTrainingStats) Implements TrainingWorker(Of SharedTrainingResult).processMinibatchWithStats
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function processMinibatchWithStats(ByVal dataSet As MultiDataSet, ByVal graph As ComputationGraph, ByVal isLast As Boolean) As Pair(Of SharedTrainingResult, SparkTrainingStats) Implements TrainingWorker(Of SharedTrainingResult).processMinibatchWithStats
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function getFinalResult(ByVal network As MultiLayerNetwork) As SharedTrainingResult
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function getFinalResult(ByVal network As ComputationGraph) As SharedTrainingResult
			Throw New System.NotSupportedException()
		End Function

		Public Overrides ReadOnly Property FinalResultNoData As SharedTrainingResult
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		Public Overrides Function getFinalResultNoDataWithStats() As Pair(Of SharedTrainingResult, SparkTrainingStats) Implements TrainingWorker(Of SharedTrainingResult).getFinalResultNoDataWithStats
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function getFinalResultWithStats(ByVal network As MultiLayerNetwork) As Pair(Of SharedTrainingResult, SparkTrainingStats) Implements TrainingWorker(Of SharedTrainingResult).getFinalResultWithStats
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function getFinalResultWithStats(ByVal graph As ComputationGraph) As Pair(Of SharedTrainingResult, SparkTrainingStats) Implements TrainingWorker(Of SharedTrainingResult).getFinalResultWithStats
			Throw New System.NotSupportedException()
		End Function

		Public Overrides ReadOnly Property DataConfiguration As WorkerConfiguration
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property
	End Class

End Namespace