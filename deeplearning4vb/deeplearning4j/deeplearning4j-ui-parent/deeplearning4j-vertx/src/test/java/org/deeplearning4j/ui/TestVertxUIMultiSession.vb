Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports HttpResponseStatus = io.netty.handler.codec.http.HttpResponseStatus
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
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
Imports Adam = org.nd4j.linalg.learning.config.Adam
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


	''' <summary>
	''' @author Tamas Fenyvesi
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestVertxUIMultiSession extends org.deeplearning4j.BaseDL4JTest
	 Public Class TestVertxUIMultiSession
		 Inherits BaseDL4JTest

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(TestVertxUIMultiSession).FullName)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub setUp()
			UIServer.stopInstance()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIMultiSessionParallelTraining() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIMultiSessionParallelTraining()
			Dim uIServer As UIServer = UIServer.getInstance(True, Nothing)
			Dim statStorageForThread As New Dictionary(Of Thread, StatsStorage)()
			Dim sessionIdForThread As New Dictionary(Of Thread, String)()

			Dim parallelTrainingCount As Integer = 10
			For session As Integer = 0 To parallelTrainingCount - 1

				Dim ss As StatsStorage = New InMemoryStatsStorage()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int sid = session;
				Dim sid As Integer = session
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String sessionId = System.Convert.ToString(sid);
				Dim sessionId As String = Convert.ToString(sid)

				Dim training As New Thread(Sub()
				Dim layerSize As Integer = sid + 4
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Adam(1e-2)).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(4).nOut(layerSize).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(layerSize).nOut(3).build()).build()
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
				End Sub)

				training.Start()
				statStorageForThread(training) = ss
				sessionIdForThread(training) = sessionId
			Next session

			For Each thread As Thread In statStorageForThread.Keys
				Dim ss As StatsStorage = statStorageForThread(thread)
				Dim sessionId As String = sessionIdForThread(thread)
				Try
					thread.Join()
	'                
	'                 * Visiting /train/:sessionId to check if training session is available on it's URL
	'                 
					Dim sessionUrl As String = trainingSessionUrl(uIServer.Address, sessionId)
					Dim conn As HttpURLConnection = CType((New URL(sessionUrl)).openConnection(), HttpURLConnection)
					conn.connect()

					assertEquals(HttpResponseStatus.OK.code(), conn.getResponseCode())
					assertTrue(uIServer.isAttached(ss))
				Catch e As IOException
					log.error("",e)
					fail(e.Message)
				Finally
					uIServer.detach(ss)
					assertFalse(uIServer.isAttached(ss))
				End Try
			Next thread
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testUIAutoAttach() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testUIAutoAttach()
			Dim statsStorageForSession As New Dictionary(Of String, StatsStorage)()

			Dim statsStorageProvider As [Function](Of String, StatsStorage) = AddressOf statsStorageForSession.get
			Dim uIServer As UIServer = UIServer.getInstance(True, statsStorageProvider)

			For session As Integer = 0 To 2
				Dim layerSize As Integer = session + 4

				Dim ss As New InMemoryStatsStorage()
				Dim sessionId As String = Convert.ToString(session)
				statsStorageForSession(sessionId) = ss
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

				assertTrue(uIServer.isAttached(statsStorageForSession(sessionId)))
				uIServer.detach(ss)
				assertFalse(uIServer.isAttached(statsStorageForSession(sessionId)))

	'            
	'             * Visiting /train/:sessionId to auto-attach StatsStorage
	'             
				Dim sessionUrl As String = trainingSessionUrl(uIServer.Address, sessionId)
				Dim conn As HttpURLConnection = CType((New URL(sessionUrl)).openConnection(), HttpURLConnection)
				conn.connect()

				assertEquals(HttpResponseStatus.OK.code(), conn.getResponseCode())
				assertTrue(uIServer.isAttached(statsStorageForSession(sessionId)))
			Next session
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testUIServerGetInstanceMultipleCalls1()
		Public Overridable Sub testUIServerGetInstanceMultipleCalls1()
		   assertThrows(GetType(DL4JException),Sub()
		   Dim uiServer As UIServer = UIServer.getInstance()
		   assertFalse(uiServer.MultiSession)
		   UIServer.getInstance(True, Nothing)
		   End Sub)



		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() public void testUIServerGetInstanceMultipleCalls2()
		Public Overridable Sub testUIServerGetInstanceMultipleCalls2()
			assertThrows(GetType(DL4JException),Sub()
			Dim uiServer As UIServer = UIServer.getInstance(True, Nothing)
			assertTrue(uiServer.MultiSession)
			UIServer.getInstance(False, Nothing)
			End Sub)

		End Sub

		''' <summary>
		''' Get URL for training session on given server address </summary>
		''' <param name="serverAddress"> server address </param>
		''' <param name="sessionId"> session ID (will be URL-encoded) </param>
		''' <returns> URL </returns>
		''' <exception cref="UnsupportedEncodingException"> if the used encoding is not supported </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static String trainingSessionUrl(String serverAddress, String sessionId) throws java.io.UnsupportedEncodingException
		Private Shared Function trainingSessionUrl(ByVal serverAddress As String, ByVal sessionId As String) As String
			Return String.Format("{0}/train/{1}", serverAddress, URLEncoder.encode(sessionId, "UTF-8"))
		End Function
	 End Class

End Namespace