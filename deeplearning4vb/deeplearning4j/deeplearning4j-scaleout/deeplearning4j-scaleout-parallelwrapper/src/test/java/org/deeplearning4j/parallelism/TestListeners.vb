Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.parallelism


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class TestListeners extends org.deeplearning4j.BaseDL4JTest
	Public Class TestListeners
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListeners()
		Public Overridable Sub testListeners()
			TestListener.clearCounts()

			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer(0, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(10).nOut(10).activation(Activation.TANH).build())

			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()

			testListenersForModel(model, Collections.singletonList(New TestListener()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenersGraph()
		Public Overridable Sub testListenersGraph()
			TestListener.clearCounts()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(10).nOut(10).activation(Activation.TANH).build(), "in").setOutputs("0").build()

			Dim model As New ComputationGraph(conf)
			model.init()

			testListenersForModel(model, Collections.singletonList(New TestListener()))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenersViaModel()
		Public Overridable Sub testListenersViaModel()
			TestListener.clearCounts()

			Dim builder As MultiLayerConfiguration.Builder = (New NeuralNetConfiguration.Builder()).list().layer(0, (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(10).nOut(10).activation(Activation.TANH).build())

			Dim conf As MultiLayerConfiguration = builder.build()
			Dim model As New MultiLayerNetwork(conf)
			model.init()

			Dim ss As StatsStorage = New InMemoryStatsStorage()
			model.setListeners(New TestListener(), New StatsListener(ss))

			testListenersForModel(model, Nothing)

			assertEquals(1, ss.listSessionIDs().Count)
			assertEquals(2, ss.listWorkerIDsForSession(ss.listSessionIDs()(0)).Count)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenersViaModelGraph()
		Public Overridable Sub testListenersViaModelGraph()
			TestListener.clearCounts()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(10).nOut(10).activation(Activation.TANH).build(), "in").setOutputs("0").build()

			Dim model As New ComputationGraph(conf)
			model.init()

			Dim ss As StatsStorage = New InMemoryStatsStorage()
			model.setListeners(New TestListener(), New StatsListener(ss))

			testListenersForModel(model, Nothing)

			assertEquals(1, ss.listSessionIDs().Count)
			assertEquals(2, ss.listWorkerIDsForSession(ss.listSessionIDs()(0)).Count)
		End Sub

		Private Shared Sub testListenersForModel(ByVal model As Model, ByVal listeners As IList(Of TrainingListener))

			Dim nWorkers As Integer = 2
			Dim wrapper As ParallelWrapper = (New ParallelWrapper.Builder(model)).workers(nWorkers).averagingFrequency(1).reportScoreAfterAveraging(True).build()

			If listeners IsNot Nothing Then
				wrapper.setListeners(listeners)
			End If

			Dim data As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To nWorkers - 1
				data.Add(New DataSet(Nd4j.rand(1, 10), Nd4j.rand(1, 10)))
			Next i

			Dim iter As DataSetIterator = New ExistingDataSetIterator(data)

			TestListener.clearCounts()
			wrapper.fit(iter)

			assertEquals(2, TestListener.workerIDs.Count)
			assertEquals(1, TestListener.sessionIDs.Count)
			assertEquals(2, TestListener.forwardPassCount.get())
			assertEquals(2, TestListener.backwardPassCount.get())
		End Sub


		<Serializable>
		Private Class TestListener
			Inherits BaseTrainingListener
			Implements RoutingIterationListener

			Friend Shared ReadOnly forwardPassCount As New AtomicInteger()
			Friend Shared ReadOnly backwardPassCount As New AtomicInteger()
			Friend Shared ReadOnly instanceCount As New AtomicInteger()
			Friend Shared ReadOnly workerIDs As ISet(Of String) = Collections.newSetFromMap(New ConcurrentDictionary(Of String, Boolean)())
			Friend Shared ReadOnly sessionIDs As ISet(Of String) = Collections.newSetFromMap(New ConcurrentDictionary(Of String, Boolean)())

			Public Shared Sub clearCounts()
				forwardPassCount.set(0)
				backwardPassCount.set(0)
				instanceCount.set(0)
				workerIDs.Clear()
				sessionIDs.Clear()
			End Sub

			Public Sub New()
				instanceCount.incrementAndGet()
			End Sub

			Public Overrides Sub onEpochStart(ByVal model As Model)
			End Sub
			Public Overrides Sub onEpochEnd(ByVal model As Model)
			End Sub
			Public Overridable Overloads Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray))
				forwardPassCount.incrementAndGet()
			End Sub

			Public Overridable Overloads Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray))
				forwardPassCount.incrementAndGet()
			End Sub

			Public Overrides Sub onGradientCalculation(ByVal model As Model)
			End Sub
			Public Overrides Sub onBackwardPass(ByVal model As Model)
				backwardPassCount.getAndIncrement()
			End Sub

			Public Overridable Property StorageRouter Implements RoutingIterationListener.setStorageRouter As StatsStorageRouter
				Set(ByVal router As StatsStorageRouter)
				End Set
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Property WorkerID Implements RoutingIterationListener.setWorkerID As String
				Set(ByVal workerID As String)
					workerIDs.Add(workerID)
				End Set
				Get
					Return Nothing
				End Get
			End Property


			Public Overridable Property SessionID Implements RoutingIterationListener.setSessionID As String
				Set(ByVal sessionID As String)
					sessionIDs.Add(sessionID)
				End Set
				Get
					Return "session_id"
				End Get
			End Property


			Public Overridable Function clone() As RoutingIterationListener
				Return New TestListener()
			End Function

			Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			End Sub
		End Class

	End Class

End Namespace