Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports ThreadingMode = io.aeron.driver.ThreadingMode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CloseHelper = org.agrona.CloseHelper
Imports BusySpinIdleStrategy = org.agrona.concurrent.BusySpinIdleStrategy
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ParameterServerClient = org.nd4j.parameterserver.client.ParameterServerClient
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

Namespace org.nd4j.parameterserver.background


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class RemoteParameterServerClientTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class RemoteParameterServerClientTests
		Inherits BaseND4JTest

		Private parameterLength As Integer = 1000
		Private ctx As Aeron.Context
		Private mediaDriver As MediaDriver
		Private masterStatus As New AtomicInteger(0)
		Private slaveStatus As New AtomicInteger(0)
		Private aeron As Aeron

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub before()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.driver.MediaDriver.Context ctx = new io.aeron.driver.MediaDriver.Context().threadingMode(io.aeron.driver.ThreadingMode.DEDICATED).dirDeleteOnStart(true).termBufferSparseFile(false).conductorIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).receiverIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).senderIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy());
			Dim ctx As MediaDriver.Context = (New MediaDriver.Context()).threadingMode(ThreadingMode.DEDICATED).dirDeleteOnStart(True).termBufferSparseFile(False).conductorIdleStrategy(New BusySpinIdleStrategy()).receiverIdleStrategy(New BusySpinIdleStrategy()).senderIdleStrategy(New BusySpinIdleStrategy())

			mediaDriver = MediaDriver.launchEmbedded(ctx)
			aeron = Aeron.connect(Context)

			Dim t As New Thread(Sub()
			Try
				masterStatus.set(BackgroundDaemonStarter.startMaster(parameterLength, mediaDriver.aeronDirectoryName()))
			Catch e As Exception
				log.error("",e)
			End Try
			End Sub)

			t.Start()
			log.info("Started master")
			Dim t2 As New Thread(Sub()
			Try
				slaveStatus.set(BackgroundDaemonStarter.startSlave(parameterLength, mediaDriver.aeronDirectoryName()))
			Catch e As Exception
				log.error("",e)
			End Try
			End Sub)
			t2.Start()
			log.info("Started slave")
			'wait on the http servers
			Thread.Sleep(30000)

		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub after()
			CloseHelper.close(mediaDriver)
			CloseHelper.close(aeron)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(60000L) @Disabled public void remoteTests() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub remoteTests()
			If masterStatus.get() <> 0 OrElse slaveStatus.get() <> 0 Then
				Throw New System.InvalidOperationException("Master or slave failed to start. Exiting")
			End If

			Dim client As ParameterServerClient = ParameterServerClient.builder().aeron(aeron).ndarrayRetrieveUrl(BackgroundDaemonStarter.masterResponderUrl()).ndarraySendUrl(BackgroundDaemonStarter.slaveConnectionUrl()).subscriberHost("localhost").masterStatusHost("localhost").masterStatusPort(9200).subscriberPort(40125).subscriberStream(12).build()

			assertEquals("localhost:40125:12", client.connectionUrl())
			Do While Not client.masterStarted()
				Thread.Sleep(1000)
				log.info("Waiting on master starting.")
			Loop

			'flow 1:
			''' <summary>
			''' Client (40125:12): sends array to listener on slave(40126:10)
			''' which publishes to master (40123:11)
			''' which adds the array for parameter averaging.
			''' In this case totalN should be 1.
			''' </summary>
			log.info("Pushing ndarray")
			client.pushNDArray(Nd4j.ones(parameterLength))
			Do While client.arraysSentToResponder() < 1
				Thread.Sleep(1000)
				log.info("Waiting on ndarray responder to receive array")
			Loop

			log.info("Pushed ndarray")
			Dim arr As INDArray = client.Array
			assertEquals(Nd4j.ones(1000), arr)

	'        
	'        StopWatch stopWatch = new StopWatch();
	'        long nanoTimeTotal = 0;
	'        int n = 1000;
	'        for(int i = 0; i < n; i++) {
	'            stopWatch.start();
	'            client.getArray();
	'            stopWatch.stop();
	'            nanoTimeTotal += stopWatch.getNanoTime();
	'            stopWatch.reset();
	'        }
	'        
	'        System.out.println(nanoTimeTotal / n);
	'        



		End Sub



		Private ReadOnly Property Context As Aeron.Context
			Get
				If ctx Is Nothing Then
					ctx = (New Aeron.Context()).driverTimeoutMs(Long.MaxValue).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriver.aeronDirectoryName()).keepAliveIntervalNs(1000).errorHandler(Function(e) log.error(e.ToString(), e))
				End If
				Return ctx
			End Get
		End Property

	End Class

End Namespace