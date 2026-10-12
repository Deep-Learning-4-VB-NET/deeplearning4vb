Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ParameterServerListener = org.nd4j.parameterserver.ParameterServerListener
Imports ParameterServerSubscriber = org.nd4j.parameterserver.ParameterServerSubscriber
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

Namespace org.nd4j.parameterserver.client

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class ParameterServerClientTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class ParameterServerClientTest
		Inherits BaseND4JTest

		Private Shared mediaDriver As MediaDriver
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(ParameterServerClientTest))
		Private Shared aeron As Aeron
		Private Shared masterNode, slaveNode As ParameterServerSubscriber
		Private Shared parameterLength As Integer = 1000

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void beforeClass() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub beforeClass()
			mediaDriver = MediaDriver.launchEmbedded(AeronUtil.getMediaDriverContext(parameterLength))
			System.setProperty("play.server.dir", "/tmp")
			aeron = Aeron.connect(Context)
			masterNode = New ParameterServerSubscriber(mediaDriver)
			masterNode.setAeron(aeron)
			Dim masterPort As Integer = 40323 + (New Random()).Next(3000)
			masterNode.run(New String() {"-m", "true", "-s", "1," & parameterLength.ToString(), "-p", masterPort.ToString(), "-h", "localhost", "-id", "11", "-md", mediaDriver.aeronDirectoryName(), "-sp", "33000", "-u", 1.ToString()})

			assertTrue(masterNode.isMaster())
			assertEquals(masterPort, masterNode.getPort())
			assertEquals("localhost", masterNode.getHost())
			assertEquals(11, masterNode.getStreamId())
			assertEquals(12, masterNode.getResponder().getStreamId())

			slaveNode = New ParameterServerSubscriber(mediaDriver)
			slaveNode.setAeron(aeron)
			slaveNode.run(New String() {"-p", (masterPort + 100).ToString(), "-h", "localhost", "-id", "10", "-pm", masterNode.getSubscriber().connectionUrl(), "-md", mediaDriver.aeronDirectoryName(), "-sp", "31000", "-u", 1.ToString()})

			assertFalse(slaveNode.isMaster())
			assertEquals(masterPort + 100, slaveNode.getPort())
			assertEquals("localhost", slaveNode.getHost())
			assertEquals(10, slaveNode.getStreamId())

			Dim tries As Integer = 10
			Do While Not masterNode.subscriberLaunched() AndAlso Not slaveNode.subscriberLaunched() AndAlso tries < 10
				Thread.Sleep(10000)
				tries += 1
			Loop

			If Not masterNode.subscriberLaunched() AndAlso Not slaveNode.subscriberLaunched() Then
				Throw New System.InvalidOperationException("Failed to start master and slave node")
			End If

			log.info("Using media driver directory " & mediaDriver.aeronDirectoryName())
			log.info("Launched media driver")
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) @Disabled("AB 2019/05/31 - Intermittent failures on CI - see issue 7657") public void testServer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testServer()
			Dim subscriberPort As Integer = 40625 + (New Random()).Next(100)
			Dim client As ParameterServerClient = ParameterServerClient.builder().aeron(aeron).ndarrayRetrieveUrl(masterNode.getResponder().connectionUrl()).ndarraySendUrl(slaveNode.getSubscriber().connectionUrl()).subscriberHost("localhost").subscriberPort(subscriberPort).subscriberStream(12).build()
			assertEquals(String.Format("localhost:{0:D}:12", subscriberPort), client.connectionUrl())
			'flow 1:
			''' <summary>
			''' Client (40125:12): sends array to listener on slave(40126:10)
			''' which publishes to master (40123:11)
			''' which adds the array for parameter averaging.
			''' In this case totalN should be 1.
			''' </summary>
			client.pushNDArray(Nd4j.ones(1, parameterLength))
			log.info("Pushed ndarray")
			Thread.Sleep(30000)
			Dim listener As ParameterServerListener = CType(masterNode.getCallback(), ParameterServerListener)
			assertEquals(1, listener.getUpdater().numUpdates())
			assertEquals(Nd4j.ones(1, parameterLength), listener.getUpdater().ndArrayHolder().get())
			Dim arr As INDArray = client.Array
			assertEquals(Nd4j.ones(1, 1000), arr)
		End Sub



		Private Shared ReadOnly Property Context As Aeron.Context
			Get
				Return (New Aeron.Context()).driverTimeoutMs(Long.MaxValue).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriver.aeronDirectoryName()).keepAliveIntervalNs(100000).errorHandler(Function(e) log.error(e.ToString(), e))
			End Get
		End Property


	End Class

End Namespace