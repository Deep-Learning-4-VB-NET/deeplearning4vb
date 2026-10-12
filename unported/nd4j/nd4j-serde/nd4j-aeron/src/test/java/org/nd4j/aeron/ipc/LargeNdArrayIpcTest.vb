Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CloseHelper = org.agrona.CloseHelper
Imports org.junit.jupiter.api
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertFalse

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

Namespace org.nd4j.aeron.ipc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NotThreadSafe @Disabled("Tests are too flaky") @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class LargeNdArrayIpcTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class LargeNdArrayIpcTest
		Inherits BaseND4JTest

		Private mediaDriver As MediaDriver
		Private ctx As Aeron.Context
		Private channel As String = "aeron:udp?endpoint=localhost:" & (40123 + (New Random()).Next(130))
		Private streamId As Integer = 10
		Private length As Integer = CInt(Math.Truncate(1e))7

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			If IntegrationTests Then
				'MediaDriver.loadPropertiesFile("aeron.properties");
				Dim ctx As MediaDriver.Context = AeronUtil.getMediaDriverContext(length)
				mediaDriver = MediaDriver.launchEmbedded(ctx)
				Console.WriteLine("Using media driver directory " & mediaDriver.aeronDirectoryName())
				Console.WriteLine("Launched media driver")
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			If IntegrationTests Then
				CloseHelper.quietClose(mediaDriver)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testMultiThreadedIpcBig() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiThreadedIpcBig()
			skipUnlessIntegrationTests() 'Long-running test - don't run as part of unit tests by default

			Dim length As Integer = CInt(Math.Truncate(1e))7
			Dim arr As INDArray = Nd4j.ones(length)
			Dim publisher As AeronNDArrayPublisher
			ctx = (New Aeron.Context()).driverTimeoutMs(1000000).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriver.aeronDirectoryName()).keepAliveIntervalNs(1000000).errorHandler(Function(err) err.printStackTrace())

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicBoolean running = new java.util.concurrent.atomic.AtomicBoolean(true);
			Dim running As New AtomicBoolean(True)
			Dim aeron As Aeron = Aeron.connect(ctx)
			Dim numSubscribers As Integer = 1
			Dim subscribers(numSubscribers - 1) As AeronNDArraySubscriber
			For i As Integer = 0 To numSubscribers - 1
				Dim subscriber As AeronNDArraySubscriber = AeronNDArraySubscriber.builder().streamId(streamId).ctx(Context).channel(channel).aeron(aeron).running(running).ndArrayCallback(New NDArrayCallbackAnonymousInnerClass(Me, running)).build()


				Dim t As New Thread(Sub()
				Try
					subscriber.launch()
				Catch e As Exception
					log.error("",e)
				End Try
				End Sub)

				t.Start()

				subscribers(i) = subscriber
			Next i

			Thread.Sleep(10000)

			publisher = AeronNDArrayPublisher.builder().publishRetryTimeOut(300000).streamId(streamId).channel(channel).aeron(aeron).build()


			Dim i As Integer = 0
			Do While i < 1 AndAlso running.get()
				log.info("About to send array.")
				publisher.publish(arr)
				log.info("Sent array")

				i += 1
			Loop

			Thread.Sleep(30000)



			For i As Integer = 0 To numSubscribers - 1
				CloseHelper.close(subscribers(i))
			Next i
			CloseHelper.close(aeron)
			CloseHelper.close(publisher)
			assertFalse(running.get())
		End Sub

		Private Class NDArrayCallbackAnonymousInnerClass
			Implements NDArrayCallback

			Private ReadOnly outerInstance As LargeNdArrayIpcTest

			Private running As AtomicBoolean

			Public Sub New(ByVal outerInstance As LargeNdArrayIpcTest, ByVal running As AtomicBoolean)
				Me.outerInstance = outerInstance
				Me.running = running
			End Sub

										''' <summary>
										''' A listener for ndarray message
										''' </summary>
										''' <param name="message"> the message for the callback </param>
			Public Sub onNDArrayMessage(ByVal message As NDArrayMessage) Implements NDArrayCallback.onNDArrayMessage
				running.set(False)
			End Sub

			Public Sub onNDArrayPartial(ByVal arr As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer) Implements NDArrayCallback.onNDArrayPartial

			End Sub

			Public Sub onNDArray(ByVal arr As INDArray) Implements NDArrayCallback.onNDArray
				running.set(False)
			End Sub
		End Class



		Private ReadOnly Property Context As Aeron.Context
			Get
				If ctx Is Nothing Then
					ctx = (New Aeron.Context()).driverTimeoutMs(1000000).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriver.aeronDirectoryName()).keepAliveIntervalNs(100000).errorHandler(Function(err) err.printStackTrace())
				End If
				Return ctx
			End Get
		End Property
	End Class

End Namespace