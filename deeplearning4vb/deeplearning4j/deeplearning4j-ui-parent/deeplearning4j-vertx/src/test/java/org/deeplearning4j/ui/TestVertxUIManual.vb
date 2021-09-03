Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports HttpResponseStatus = io.netty.handler.codec.http.HttpResponseStatus
Imports Future = io.vertx.core.Future
Imports Promise = io.vertx.core.Promise
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports UIServer = org.deeplearning4j.ui.api.UIServer
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports InMemoryStatsStorage = org.deeplearning4j.ui.model.storage.InMemoryStatsStorage
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
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestVertxUIManual extends org.deeplearning4j.BaseDL4JTest
	Public Class TestVertxUIManual
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(TestVertxUIManual).FullName)


		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 3600_000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testUI() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUI()
			Dim uiServer As VertxUIServer = CType(UIServer.getInstance(), VertxUIServer)
			assertEquals(9000, uiServer.Port)

			Thread.Sleep(3000_000)
			uiServer.stop()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testUISequentialSessions() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUISequentialSessions()
			Dim uiServer As UIServer = UIServer.getInstance()
			Dim ss As StatsStorage = Nothing
			For session As Integer = 0 To 2

				If ss IsNot Nothing Then
					uiServer.detach(ss)
				End If
				ss = New InMemoryStatsStorage()
				uiServer.attach(ss)

				Dim numInputs As Integer = 4
				Dim outputNum As Integer = 3
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).activation(Activation.TANH).weightInit(WeightInit.XAVIER).updater(New Sgd(0.03)).l2(1e-4).list().layer(0, (New DenseLayer.Builder()).nIn(numInputs).nOut(3).build()).layer(1, (New DenseLayer.Builder()).nIn(3).nOut(3).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.NEGATIVELOGLIKELIHOOD)).activation(Activation.SOFTMAX).nIn(3).nOut(outputNum).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()
				net.setListeners(New StatsListener(ss), New ScoreIterationListener(1))

				Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

				For i As Integer = 0 To 99
					net.fit(iter)
				Next i
				Thread.Sleep(5_000)
			Next session
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testUIServerStop() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIServerStop()
			Dim uiServer As UIServer = UIServer.getInstance(True, Nothing)
			assertTrue(uiServer.MultiSession)
			assertFalse(uiServer.Stopped)

			Dim sleepMilliseconds As Long = 30_000
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
'ORIGINAL LINE: @Test @Disabled public void testUIServerStopAsync() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIServerStopAsync()
			Dim uiServer As UIServer = UIServer.getInstance(True, Nothing)
			assertTrue(uiServer.MultiSession)
			assertFalse(uiServer.Stopped)

			Dim sleepMilliseconds As Long = 30_000
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
'ORIGINAL LINE: @Test @Disabled public void testUIAutoAttachDetach() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIAutoAttachDetach()
			Dim detachTimeoutMillis As Long = 15_000
			Dim statsProvider As New AutoDetachingStatsStorageProvider(detachTimeoutMillis)
			Dim uIServer As UIServer = UIServer.getInstance(True, statsProvider)
			statsProvider.UIServer = uIServer
			Dim ss As InMemoryStatsStorage = Nothing
			For session As Integer = 0 To 2
				Dim layerSize As Integer = session + 4

				ss = New InMemoryStatsStorage()
				Dim sessionId As String = Convert.ToString(session)
				statsProvider.put(sessionId, ss)
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(4).nOut(layerSize).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(layerSize).nOut(3).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim statsListener As New StatsListener(ss, 1)
				statsListener.SessionID = sessionId
				net.setListeners(statsListener, New ScoreIterationListener(1))
				uIServer.attach(ss)

				Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

				For i As Integer = 0 To 19
					net.fit(iter)
				Next i

				assertTrue(uIServer.isAttached(ss))
				uIServer.detach(ss)
				assertFalse(uIServer.isAttached(ss))

	'            
	'             * Visiting /train/:sessionId to auto-attach StatsStorage
	'             
				Dim sessionUrl As String = trainingSessionUrl(uIServer.Address, sessionId)
				Dim conn As HttpURLConnection = CType((New URL(sessionUrl)).openConnection(), HttpURLConnection)
				conn.connect()

				assertEquals(HttpResponseStatus.OK.code(), conn.getResponseCode())
				assertTrue(uIServer.isAttached(ss))
			Next session

			Thread.Sleep(detachTimeoutMillis + 60_000)
			assertFalse(uIServer.isAttached(ss))
		End Sub


		''' <summary>
		''' Get URL-encoded URL for training session on given server address </summary>
		''' <param name="serverAddress"> server address </param>
		''' <param name="sessionId"> session ID </param>
		''' <returns> URL </returns>
		''' <exception cref="UnsupportedEncodingException"> if the used encoding is not supported </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static String trainingSessionUrl(String serverAddress, String sessionId) throws java.io.UnsupportedEncodingException
		Private Shared Function trainingSessionUrl(ByVal serverAddress As String, ByVal sessionId As String) As String
			Return String.Format("{0}/train/{1}", serverAddress, URLEncoder.encode(sessionId, "UTF-8"))
		End Function

		''' <summary>
		''' StatsStorage provider with automatic detaching of StatsStorage after a timeout
		''' @author Tamas Fenyvesi
		''' </summary>
		Private Class AutoDetachingStatsStorageProvider
			Implements [Function](Of String, StatsStorage)

			Friend storageForSession As New Dictionary(Of String, InMemoryStatsStorage)()
'JAVA TO VB CONVERTER NOTE: The field uIServer was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend uIServer_Conflict As UIServer
			Friend autoDetachTimeoutMillis As Long

			Public Sub New(ByVal autoDetachTimeoutMillis As Long)
				Me.autoDetachTimeoutMillis = autoDetachTimeoutMillis
			End Sub

			Public Overridable Sub put(ByVal sessionId As String, ByVal statsStorage As InMemoryStatsStorage)
				storageForSession(sessionId) = statsStorage
			End Sub

			Public Overridable WriteOnly Property UIServer As UIServer
				Set(ByVal uIServer As UIServer)
					Me.uIServer_Conflict = uIServer
				End Set
			End Property

			Public Overridable Function apply(ByVal sessionId As String) As StatsStorage
				Dim statsStorage As StatsStorage = storageForSession(sessionId)

				If statsStorage IsNot Nothing Then
					Call (New Thread(Sub()
					Try
						log.info("Waiting to detach StatsStorage (session ID: {})" & " after {} ms ", sessionId, autoDetachTimeoutMillis)
						Thread.Sleep(autoDetachTimeoutMillis)
					Catch e As InterruptedException
						Console.WriteLine(e.ToString())
						Console.Write(e.StackTrace)
					Finally
						log.info("Auto-detaching StatsStorage (session ID: {}) after {} ms.", sessionId, autoDetachTimeoutMillis)
						uIServer_Conflict.detach(statsStorage)
						log.info(" To re-attach StatsStorage of training session, visit {}/train/{}", uIServer_Conflict.Address, sessionId)
					End Try
					End Sub)).Start()
				End If

				Return statsStorage
			End Function
		End Class

	End Class

End Namespace