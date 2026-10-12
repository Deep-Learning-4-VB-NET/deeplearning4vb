Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports ThreadingMode = io.aeron.driver.ThreadingMode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BusySpinIdleStrategy = org.agrona.concurrent.BusySpinIdleStrategy
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ParameterServerListener = org.nd4j.parameterserver.ParameterServerListener
Imports ParameterServerSubscriber = org.nd4j.parameterserver.ParameterServerSubscriber
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
'ORIGINAL LINE: @Slf4j @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class ParameterServerClientPartialTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class ParameterServerClientPartialTest
		Inherits BaseND4JTest

		Private Shared mediaDriver As MediaDriver
		Private Shared ctx As Aeron.Context
		Private Shared masterNode, slaveNode As ParameterServerSubscriber
		Private shape() As Integer = {2, 2}
		Private Shared aeron As Aeron

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void beforeClass() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub beforeClass()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.driver.MediaDriver.Context ctx = new io.aeron.driver.MediaDriver.Context().threadingMode(io.aeron.driver.ThreadingMode.@SHARED).dirDeleteOnStart(true).termBufferSparseFile(false).conductorIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).receiverIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).senderIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy());
			Dim ctx As MediaDriver.Context = (New MediaDriver.Context()).threadingMode(ThreadingMode.SHARED).dirDeleteOnStart(True).termBufferSparseFile(False).conductorIdleStrategy(New BusySpinIdleStrategy()).receiverIdleStrategy(New BusySpinIdleStrategy()).senderIdleStrategy(New BusySpinIdleStrategy())

			mediaDriver = MediaDriver.launchEmbedded(ctx)
			aeron = Aeron.connect(Context)
			masterNode = New ParameterServerSubscriber(mediaDriver)
			masterNode.setAeron(aeron)
			Dim masterPort As Integer = 40223 + (New Random()).Next(13000)
			Dim masterStatusPort As Integer = masterPort - 2000
			masterNode.run(New String() {"-m", "true", "-p", masterPort.ToString(), "-h", "localhost", "-id", "11", "-md", mediaDriver.aeronDirectoryName(), "-sp", masterStatusPort.ToString(), "-s", "2,2", "-u", 1.ToString() })

			assertTrue(masterNode.isMaster())
			assertEquals(masterPort, masterNode.getPort())
			assertEquals("localhost", masterNode.getHost())
			assertEquals(11, masterNode.getStreamId())
			assertEquals(12, masterNode.getResponder().getStreamId())
			assertEquals(masterNode.MasterArray, Nd4j.create(New Integer() {2, 2}))

			slaveNode = New ParameterServerSubscriber(mediaDriver)
			slaveNode.setAeron(aeron)
			Dim slavePort As Integer = masterPort + 100
			Dim slaveStatusPort As Integer = slavePort - 2000
			slaveNode.run(New String() {"-p", slavePort.ToString(), "-h", "localhost", "-id", "10", "-pm", masterNode.getSubscriber().connectionUrl(), "-md", mediaDriver.aeronDirectoryName(), "-sp", slaveStatusPort.ToString(), "-u", 1.ToString() })

			assertFalse(slaveNode.isMaster())
			assertEquals(slavePort, slaveNode.getPort())
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
'ORIGINAL LINE: @Test() @Timeout(60000L) @Disabled("AB 2019/06/01 - Intermittent failures - see issue 7657") public void testServer() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testServer()
			Dim client As ParameterServerClient = ParameterServerClient.builder().aeron(aeron).ndarrayRetrieveUrl(masterNode.getResponder().connectionUrl()).ndarraySendUrl(slaveNode.getSubscriber().connectionUrl()).subscriberHost("localhost").subscriberPort(40325).subscriberStream(12).build()
			assertEquals("localhost:40325:12", client.connectionUrl())
			'flow 1:
			''' <summary>
			''' Client (40125:12): sends array to listener on slave(40126:10)
			''' which publishes to master (40123:11)
			''' which adds the array for parameter averaging.
			''' In this case totalN should be 1.
			''' </summary>
			client.pushNDArrayMessage(NDArrayMessage.of(Nd4j.ones(2), New Integer() {0}, 0))
			log.info("Pushed ndarray")
			Thread.Sleep(30000)
			Dim listener As ParameterServerListener = CType(masterNode.getCallback(), ParameterServerListener)
			assertEquals(1, listener.getUpdater().numUpdates())
			Dim assertion As INDArray = Nd4j.create(New Integer() {2, 2})
			assertion.getColumn(0).addi(1.0)
			assertEquals(assertion, listener.getUpdater().ndArrayHolder().get())
			Dim arr As INDArray = client.Array
			assertEquals(assertion, arr)
		End Sub



		Private Shared ReadOnly Property Context As Aeron.Context
			Get
				If ctx Is Nothing Then
					ctx = (New Aeron.Context()).driverTimeoutMs(Long.MaxValue).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriver.aeronDirectoryName()).keepAliveIntervalNs(10000).errorHandler(Function(e) log.error(e.ToString(), e))
				End If
				Return ctx
			End Get
		End Property


	End Class

End Namespace