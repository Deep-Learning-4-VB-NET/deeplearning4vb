Imports System.Collections.Generic
Imports System.Threading
Imports Future = io.vertx.core.Future
Imports Promise = io.vertx.core.Promise
Imports Vertx = io.vertx.core.Vertx
Imports IOUtils = org.apache.commons.io.IOUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports GaussianReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.GaussianReconstructionDistribution
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports JsonMappers = org.deeplearning4j.nn.conf.serde.JsonMappers
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports UIServer = org.deeplearning4j.ui.api.UIServer
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports org.nd4j.common.function
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
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

Namespace org.deeplearning4j.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestVertxUI extends org.deeplearning4j.BaseDL4JTest
	Public Class TestVertxUI
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(TestVertxUI).FullName)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			UIServer.stopInstance()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUI() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUI()
			Dim uiServer As VertxUIServer = CType(UIServer.getInstance(), VertxUIServer)
			assertEquals(9000, uiServer.Port)
			uiServer.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUI_VAE() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUI_VAE()
			'Variational autoencoder - for unsupervised layerwise pretraining

			Dim ss As StatsStorage = New InMemoryStatsStorage()

			Dim uiServer As UIServer = UIServer.getInstance()
			uiServer.attach(ss)

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(1e-5)).list().layer(0, (New VariationalAutoencoder.Builder()).nIn(4).nOut(3).encoderLayerSizes(10, 11).decoderLayerSizes(12, 13).weightInit(WeightInit.XAVIER).pzxActivationFunction(Activation.IDENTITY).reconstructionDistribution(New GaussianReconstructionDistribution()).activation(Activation.LEAKYRELU).build()).layer(1, (New VariationalAutoencoder.Builder()).nIn(3).nOut(3).encoderLayerSizes(7).decoderLayerSizes(8).weightInit(WeightInit.XAVIER).pzxActivationFunction(Activation.IDENTITY).reconstructionDistribution(New GaussianReconstructionDistribution()).activation(Activation.LEAKYRELU).build()).layer(2, (New OutputLayer.Builder()).nIn(3).nOut(3).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.setListeners(New StatsListener(ss), New ScoreIterationListener(1))

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

			For i As Integer = 0 To 49
				net.fit(iter)
				Thread.Sleep(100)
			Next i

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIMultipleSessions() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIMultipleSessions()

			For session As Integer = 0 To 2

				Dim ss As StatsStorage = New InMemoryStatsStorage()

				Dim uiServer As UIServer = UIServer.getInstance()
				uiServer.attach(ss)

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(4).nOut(4).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(4).nOut(3).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				net.setListeners(New StatsListener(ss, 1), New ScoreIterationListener(1))

				Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

				For i As Integer = 0 To 19
					net.fit(iter)
					Thread.Sleep(100)
				Next i
			Next session
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUICompGraph()
		Public Overridable Sub testUICompGraph()

			Dim ss As StatsStorage = New InMemoryStatsStorage()

			Dim uiServer As UIServer = UIServer.getInstance()
			uiServer.attach(ss)

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("L0", (New DenseLayer.Builder()).activation(Activation.TANH).nIn(4).nOut(4).build(), "in").addLayer("L1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(4).nOut(3).build(), "L0").setOutputs("L1").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			net.setListeners(New StatsListener(ss), New ScoreIterationListener(1))

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

			For i As Integer = 0 To 99
				net.fit(iter)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAutoAttach() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAutoAttach()

			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("L0", (New DenseLayer.Builder()).activation(Activation.TANH).nIn(4).nOut(4).build(), "in").addLayer("L1", (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(4).nOut(3).build(), "L0").setOutputs("L1").build()

			Dim net As New ComputationGraph(conf)
			net.init()

			Dim ss1 As StatsStorage = New InMemoryStatsStorage()

			net.setListeners(New StatsListener(ss1, 1, "ss1"))

			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

			For i As Integer = 0 To 4
				net.fit(iter)
			Next i

			Dim ss2 As StatsStorage = New InMemoryStatsStorage()
			net.setListeners(New StatsListener(ss2, 1, "ss2"))

			For i As Integer = 0 To 3
				net.fit(iter)
			Next i

			Dim ui As UIServer = UIServer.getInstance(True, Nothing)
			Try
				DirectCast(ui, VertxUIServer).autoAttachStatsStorageBySessionId(New FunctionAnonymousInnerClass(Me))

				Dim json1 As String = IOUtils.toString(New URL("http://localhost:9000/train/ss1/overview/data"), StandardCharsets.UTF_8)

				Dim json2 As String = IOUtils.toString(New URL("http://localhost:9000/train/ss2/overview/data"), StandardCharsets.UTF_8)

				assertNotEquals(json1, json2)

				Dim m1 As IDictionary(Of String, Object) = JsonMappers.Mapper.readValue(json1, GetType(System.Collections.IDictionary))
				Dim m2 As IDictionary(Of String, Object) = JsonMappers.Mapper.readValue(json2, GetType(System.Collections.IDictionary))

				Dim s1 As IList(Of Object) = DirectCast(m1("scores"), IList(Of Object))
				Dim s2 As IList(Of Object) = DirectCast(m2("scores"), IList(Of Object))
				assertEquals(5, s1.Count)
				assertEquals(4, s2.Count)
			Finally
				ui.stop()
			End Try
		End Sub

		Private Class FunctionAnonymousInnerClass
			Implements [Function](Of String, StatsStorage)

			Private ReadOnly outerInstance As TestVertxUI

			Public Sub New(ByVal outerInstance As TestVertxUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Function apply(ByVal s As String) As StatsStorage
				If "ss1".Equals(s) Then
					Return ss1
				ElseIf "ss2".Equals(s) Then
					Return ss2
				End If
				Return Nothing
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIAttachDetach() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIAttachDetach()
			Dim ss As StatsStorage = New InMemoryStatsStorage()

			Dim uiServer As UIServer = UIServer.getInstance()
			uiServer.attach(ss)
			assertFalse(uiServer.getStatsStorageInstances().Count = 0)
			uiServer.detach(ss)
			assertTrue(uiServer.getStatsStorageInstances().Count = 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIServerStop() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIServerStop()
			Dim uiServer As UIServer = UIServer.getInstance(True, Nothing)
			assertTrue(uiServer.MultiSession)
			assertFalse(uiServer.Stopped)

			Dim sleepMilliseconds As Long = 1_000
			log.info("Waiting {} ms before stopping.", sleepMilliseconds)
			Thread.Sleep(sleepMilliseconds)
			uiServer.stop()
			assertTrue(uiServer.Stopped)

			log.info("UI server is stopped. Waiting {} ms before starting new UI server.", sleepMilliseconds)
			Thread.Sleep(sleepMilliseconds)
			uiServer = UIServer.getInstance(False, Nothing)
			assertFalse(uiServer.MultiSession)
			assertFalse(uiServer.Stopped)

			log.info("Waiting {} ms before stopping.", sleepMilliseconds)
			Thread.Sleep(sleepMilliseconds)
			uiServer.stop()
			assertTrue(uiServer.Stopped)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIServerStopAsync() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIServerStopAsync()
			Dim uiServer As UIServer = UIServer.getInstance(True, Nothing)
			assertTrue(uiServer.MultiSession)
			assertFalse(uiServer.Stopped)

			Dim sleepMilliseconds As Long = 1_000
			log.info("Waiting {} ms before stopping.", sleepMilliseconds)
			Thread.Sleep(sleepMilliseconds)

			Dim latch As New System.Threading.CountdownEvent(1)
			Dim promise As Promise(Of Void) = Promise.promise()
			promise.future().compose(Function(success) Future.future(Function(prom) latch.Signal()), Function(failure) Future.future(Function(prom) latch.Signal()))

			uiServer.stopAsync(promise)
			latch.await()
			assertTrue(uiServer.Stopped)

			log.info("UI server is stopped. Waiting {} ms before starting new UI server.", sleepMilliseconds)
			Thread.Sleep(sleepMilliseconds)
			uiServer = UIServer.getInstance(False, Nothing)
			assertFalse(uiServer.MultiSession)

			log.info("Waiting {} ms before stopping.", sleepMilliseconds)
			Thread.Sleep(sleepMilliseconds)
			uiServer.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testUIStartPortAlreadyBound() throws InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIStartPortAlreadyBound()
			assertThrows(GetType(DL4JException),Sub()
			Dim latch As New System.Threading.CountdownEvent(1)
			Dim port As Integer = VertxUIServer.DEFAULT_UI_PORT
			Dim vertx As Vertx = Vertx.vertx()
			vertx.createHttpServer().requestHandler(Sub([event])
			End Sub).listen(port, Function(result) latch.Signal())
			latch.await()
			Try
				UIServer.getInstance()
			Finally
				vertx.close()
			End Try
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIStartAsync() throws InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIStartAsync()
			Dim latch As New System.Threading.CountdownEvent(1)
			Dim promise As Promise(Of String) = Promise.promise()
			promise.future().compose(Function(success) Future.future(Function(prom) latch.Signal()), Function(failure) Future.future(Function(prom) latch.Signal()))
			Dim port As Integer = VertxUIServer.DEFAULT_UI_PORT
			VertxUIServer.getInstance(port, False, Nothing, promise)
			latch.await()
			If promise.future().succeeded() Then
				Dim deploymentId As String = promise.future().result()
				log.debug("UI server deployed, deployment ID = {}", deploymentId)
			Else
				log.debug("UI server failed to deploy.", promise.future().cause())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIShutdownHook() throws InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIShutdownHook()
			Dim uIServer As UIServer = UIServer.getInstance()
			Dim shutdownHook As Thread = UIServer.getShutdownHook()
			shutdownHook.Start()
			shutdownHook.Join()
	'        
	'         * running the shutdown hook thread before the Runtime is terminated
	'         * enables us to check if the UI server has been shut down or not
	'         
			assertTrue(uIServer.Stopped)
			log.info("Deeplearning4j UI server stopped in shutdown hook.")
		End Sub
	End Class

End Namespace