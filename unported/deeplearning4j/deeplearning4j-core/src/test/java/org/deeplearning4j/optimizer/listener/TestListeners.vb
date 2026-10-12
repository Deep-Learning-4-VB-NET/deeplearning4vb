Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports AutoEncoder = org.deeplearning4j.nn.conf.layers.AutoEncoder
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ComposableIterationListener = org.deeplearning4j.optimize.listeners.ComposableIterationListener
Imports PerformanceListener = org.deeplearning4j.optimize.listeners.PerformanceListener
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports TimeIterationListener = org.deeplearning4j.optimize.listeners.TimeIterationListener
Imports CheckpointListener = org.deeplearning4j.optimize.listeners.CheckpointListener
Imports BaseOptimizer = org.deeplearning4j.optimize.solvers.BaseOptimizer
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.optimizer.listener


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TestListeners extends org.deeplearning4j.BaseDL4JTest
	Public Class TestListeners
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSettingListenersUnsupervised()
		Public Overridable Sub testSettingListenersUnsupervised()
			'Pretrain layers should get copies of the listeners, in addition to the

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New AutoEncoder.Builder()).nIn(10).nOut(10).build()).layer(1, (New VariationalAutoencoder.Builder()).nIn(10).nOut(10).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			net.setListeners(New ScoreIterationListener(), New TestRoutingListener())

			For Each l As Layer In net.Layers
				Dim layerListeners As ICollection(Of TrainingListener) = l.getListeners()
				assertEquals(2, layerListeners.Count,l.GetType().ToString())
				Dim lArr() As TrainingListener = layerListeners.ToArray()
				assertTrue(TypeOf lArr(0) Is ScoreIterationListener)
				assertTrue(TypeOf lArr(1) Is TestRoutingListener)
			Next l

			Dim netListeners As ICollection(Of TrainingListener) = net.getListeners()
			assertEquals(2, netListeners.Count)
			Dim lArr() As TrainingListener = netListeners.ToArray()
			assertTrue(TypeOf lArr(0) Is ScoreIterationListener)
			assertTrue(TypeOf lArr(1) Is TestRoutingListener)


			Dim gConf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New AutoEncoder.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New VariationalAutoencoder.Builder()).nIn(10).nOut(10).build(), "0").setOutputs("1").build()
			Dim cg As New ComputationGraph(gConf)
			cg.init()

			cg.setListeners(New ScoreIterationListener(), New TestRoutingListener())

			For Each l As Layer In cg.Layers
				Dim layerListeners As ICollection(Of TrainingListener) = l.getListeners()
				assertEquals(2, layerListeners.Count)
				lArr = layerListeners.ToArray()
				assertTrue(TypeOf lArr(0) Is ScoreIterationListener)
				assertTrue(TypeOf lArr(1) Is TestRoutingListener)
			Next l

			netListeners = cg.getListeners()
			assertEquals(2, netListeners.Count)
			lArr = netListeners.ToArray()
			assertTrue(TypeOf lArr(0) Is ScoreIterationListener)
			assertTrue(TypeOf lArr(1) Is TestRoutingListener)
		End Sub

		<Serializable>
		Private Class TestRoutingListener
			Inherits BaseTrainingListener
			Implements RoutingIterationListener

			Public Overridable Property StorageRouter Implements RoutingIterationListener.setStorageRouter As StatsStorageRouter
				Set(ByVal router As StatsStorageRouter)
				End Set
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Property WorkerID Implements RoutingIterationListener.setWorkerID As String
				Set(ByVal workerID As String)
				End Set
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Property SessionID Implements RoutingIterationListener.setSessionID As String
				Set(ByVal sessionID As String)
				End Set
				Get
					Return Nothing
				End Get
			End Property

			Public Overridable Function clone() As RoutingIterationListener
				Return Nothing
			End Function

			Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			End Sub
		End Class





'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenerSerialization(@TempDir Path tempDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testListenerSerialization(ByVal tempDir As Path)
			'Note: not all listeners are (or should be) serializable. But some should be - for Spark etc

			Dim listeners As IList(Of TrainingListener) = New List(Of TrainingListener)()
			listeners.Add(New ScoreIterationListener())
			listeners.Add(New PerformanceListener(1, True, True))
			listeners.Add(New TimeIterationListener(10000))
			listeners.Add(New ComposableIterationListener(New ScoreIterationListener(), New PerformanceListener(1, True, True)))
			listeners.Add((New CheckpointListener.Builder(tempDir.toFile())).keepAll().saveEveryNIterations(3).build()) 'Doesn't usually need to be serialized, but no reason it can't be...


			Dim iter As DataSetIterator = New IrisDataSetIterator(10, 150)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.setListeners(listeners)

			net.fit(iter)

			Dim listeners2 As IList(Of TrainingListener) = New List(Of TrainingListener)()
			For Each il As TrainingListener In listeners
				log.info("------------------")
				log.info("Testing listener: {}", il)
				Dim baos As New MemoryStream()
				Dim oos As New ObjectOutputStream(baos)
				oos.writeObject(il)
				Dim bytes() As SByte = baos.toByteArray()

				Dim ois As New ObjectInputStream(New MemoryStream(bytes))
				Dim il2 As TrainingListener = DirectCast(ois.readObject(), TrainingListener)

				listeners2.Add(il2)
			Next il

			net.setListeners(listeners2)
			net.fit(iter)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenerCalls()
		Public Overridable Sub testListenerCalls()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim tl As New TestListener()
			net.setListeners(tl)

			Dim irisIter As DataSetIterator = New IrisDataSetIterator(50, 150)

			net.fit(irisIter, 2)

			Dim exp As IList(Of Triple(Of [Call], Integer, Integer)) = New List(Of Triple(Of [Call], Integer, Integer))()
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].EPOCH_START, 0, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, 0, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_BWD, 0, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_GRAD, 0, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ITER_DONE, 0, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, 1, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_BWD, 1, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_GRAD, 1, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ITER_DONE, 1, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, 2, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_BWD, 2, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_GRAD, 2, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ITER_DONE, 2, 0))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].EPOCH_END, 3, 0)) 'Post updating iter count, pre update epoch count

			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].EPOCH_START, 3, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, 3, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_BWD, 3, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_GRAD, 3, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ITER_DONE, 3, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, 4, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_BWD, 4, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_GRAD, 4, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ITER_DONE, 4, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, 5, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_BWD, 5, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_GRAD, 5, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].ITER_DONE, 5, 1))
			exp.Add(New Triple(Of [Call], Integer, Integer)([Call].EPOCH_END, 6, 1))


			assertEquals(exp, tl.getCalls())


			tl = New TestListener()

			Dim cg As ComputationGraph = net.toComputationGraph()
			cg.setListeners(tl)

			cg.fit(irisIter, 2)

			assertEquals(exp, tl.getCalls())
		End Sub

		Private Enum [Call]
			ITER_DONE
			EPOCH_START
			EPOCH_END
			ON_FWD
			ON_GRAD
			ON_BWD
		End Enum

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data private static class TestListener implements org.deeplearning4j.optimize.api.TrainingListener
		Private Class TestListener
			Implements TrainingListener

			Friend calls As IList(Of Triple(Of [Call], Integer, Integer)) = New List(Of Triple(Of [Call], Integer, Integer))()


			Public Overridable Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer) Implements TrainingListener.iterationDone
				calls.Add(New Triple(Of [Call], Integer, Integer)([Call].ITER_DONE, iteration, epoch))
			End Sub

			Public Overridable Sub onEpochStart(ByVal model As Model) Implements TrainingListener.onEpochStart
				calls.Add(New Triple(Of [Call], Integer, Integer)([Call].EPOCH_START, BaseOptimizer.getIterationCount(model), BaseOptimizer.getEpochCount(model)))
			End Sub

			Public Overridable Sub onEpochEnd(ByVal model As Model) Implements TrainingListener.onEpochEnd
				calls.Add(New Triple(Of [Call], Integer, Integer)([Call].EPOCH_END, BaseOptimizer.getIterationCount(model), BaseOptimizer.getEpochCount(model)))
			End Sub

			Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IList(Of INDArray)) Implements TrainingListener.onForwardPass
				calls.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, BaseOptimizer.getIterationCount(model), BaseOptimizer.getEpochCount(model)))
			End Sub

			Public Overridable Sub onForwardPass(ByVal model As Model, ByVal activations As IDictionary(Of String, INDArray)) Implements TrainingListener.onForwardPass
				calls.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_FWD, BaseOptimizer.getIterationCount(model), BaseOptimizer.getEpochCount(model)))
			End Sub

			Public Overridable Sub onGradientCalculation(ByVal model As Model) Implements TrainingListener.onGradientCalculation
				calls.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_GRAD, BaseOptimizer.getIterationCount(model), BaseOptimizer.getEpochCount(model)))
			End Sub

			Public Overridable Sub onBackwardPass(ByVal model As Model) Implements TrainingListener.onBackwardPass
				calls.Add(New Triple(Of [Call], Integer, Integer)([Call].ON_BWD, BaseOptimizer.getIterationCount(model), BaseOptimizer.getEpochCount(model)))
			End Sub
		End Class
	End Class

End Namespace