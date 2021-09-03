Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports ThreadingMode = io.aeron.driver.ThreadingMode
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CloseHelper = org.agrona.CloseHelper
Imports BusySpinIdleStrategy = org.agrona.concurrent.BusySpinIdleStrategy
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports org.nd4j.aeron.ipc
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.aeron.ipc.response


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NotThreadSafe @Disabled("Tests are too flaky") @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class AeronNDArrayResponseTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class AeronNDArrayResponseTest
		Inherits BaseND4JTest

		Private mediaDriver As MediaDriver

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			If IntegrationTests Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.driver.MediaDriver.Context ctx = new io.aeron.driver.MediaDriver.Context().threadingMode(io.aeron.driver.ThreadingMode.@SHARED).dirDeleteOnShutdown(true).dirDeleteOnStart(true).termBufferSparseFile(false).conductorIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).receiverIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).senderIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy());
				Dim ctx As MediaDriver.Context = (New MediaDriver.Context()).threadingMode(ThreadingMode.SHARED).dirDeleteOnShutdown(True).dirDeleteOnStart(True).termBufferSparseFile(False).conductorIdleStrategy(New BusySpinIdleStrategy()).receiverIdleStrategy(New BusySpinIdleStrategy()).senderIdleStrategy(New BusySpinIdleStrategy())
				mediaDriver = MediaDriver.launchEmbedded(ctx)
				Console.WriteLine("Using media driver directory " & mediaDriver.aeronDirectoryName())
				Console.WriteLine("Launched media driver")
			End If
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testResponse() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testResponse()
			skipUnlessIntegrationTests() 'Long-running test - don't run as part of unit tests by default

			Dim streamId As Integer = 10
			Dim responderStreamId As Integer = 11
			Dim host As String = "127.0.0.1"
			Dim ctx As Aeron.Context = (New Aeron.Context()).driverTimeoutMs(100000).availableImageHandler(AddressOf AeronUtil.printAvailableImage).unavailableImageHandler(AddressOf AeronUtil.printUnavailableImage).aeronDirectoryName(mediaDriver.aeronDirectoryName()).keepAliveIntervalNs(100000).errorHandler(Function(e) log.error(e.ToString(), e))

			Dim baseSubscriberPort As Integer = 40123 + (New Random()).Next(1000)

			Dim aeron As Aeron = Aeron.connect(ctx)
			Dim responder As AeronNDArrayResponder = AeronNDArrayResponder.startSubscriber(aeron, host, baseSubscriberPort + 1, New NDArrayHolderAnonymousInnerClass(Me)

										   , responderStreamId)

			Dim count As New AtomicInteger(0)
			Dim running As New AtomicBoolean(True)
			Dim subscriber As AeronNDArraySubscriber = AeronNDArraySubscriber.startSubscriber(aeron, host, baseSubscriberPort, New NDArrayCallbackAnonymousInnerClass(Me, count)
						   , streamId, running)

			Dim expectedResponses As Integer = 10
			Dim publisher As HostPortPublisher = HostPortPublisher.builder().aeron(aeron).uriToSend(host & String.Format(":{0:D}:", baseSubscriberPort) + streamId).channel(AeronUtil.aeronChannel(host, baseSubscriberPort + 1)).streamId(responderStreamId).build()



			For i As Integer = 0 To expectedResponses - 1
				publisher.send()
			Next i


			Thread.Sleep(60000)



			assertEquals(expectedResponses, count.get())

			Console.WriteLine("After")

			CloseHelper.close(responder)
			CloseHelper.close(subscriber)
			CloseHelper.close(publisher)
			CloseHelper.close(aeron)

		End Sub

		Private Class NDArrayHolderAnonymousInnerClass
			Implements NDArrayHolder

			Private ReadOnly outerInstance As AeronNDArrayResponseTest

			Public Sub New(ByVal outerInstance As AeronNDArrayResponseTest)
				Me.outerInstance = outerInstance
			End Sub

									''' <summary>
									''' Set the ndarray
									''' </summary>
									''' <param name="arr"> the ndarray for this holder
									'''            to use </param>
			Public WriteOnly Property Array Implements NDArrayHolder.setArray As INDArray
				Set(ByVal arr As INDArray)
    
				End Set
			End Property

			''' <summary>
			''' The number of updates
			''' that have been sent to this older.
			''' 
			''' @return
			''' </summary>
			Public Function totalUpdates() As Integer Implements NDArrayHolder.totalUpdates
				Return 1
			End Function

			''' <summary>
			''' Retrieve an ndarray
			''' 
			''' @return
			''' </summary>
			Public Function get() As INDArray Implements NDArrayHolder.get
				Return Nd4j.scalar(1.0)
			End Function

			''' <summary>
			''' Retrieve a partial view of the ndarray.
			''' This method uses tensor along dimension internally
			''' Note this will call dup()
			''' </summary>
			''' <param name="idx">        the index of the tad to get </param>
			''' <param name="dimensions"> the dimensions to use </param>
			''' <returns> the tensor along dimension based on the index and dimensions
			''' from the master array. </returns>
			Public Function getTad(ByVal idx As Integer, ParamArray ByVal dimensions() As Integer) As INDArray Implements NDArrayHolder.getTad
				Return Nd4j.scalar(1.0)
			End Function
		End Class

		Private Class NDArrayCallbackAnonymousInnerClass
			Implements NDArrayCallback

			Private ReadOnly outerInstance As AeronNDArrayResponseTest

			Private count As AtomicInteger

			Public Sub New(ByVal outerInstance As AeronNDArrayResponseTest, ByVal count As AtomicInteger)
				Me.outerInstance = outerInstance
				Me.count = count
			End Sub

									''' <summary>
									''' A listener for ndarray message
									''' </summary>
									''' <param name="message"> the message for the callback </param>
			Public Sub onNDArrayMessage(ByVal message As NDArrayMessage) Implements NDArrayCallback.onNDArrayMessage
				count.incrementAndGet()

			End Sub

			Public Sub onNDArrayPartial(ByVal arr As INDArray, ByVal idx As Long, ParamArray ByVal dimensions() As Integer) Implements NDArrayCallback.onNDArrayPartial
				count.incrementAndGet()
			End Sub

			Public Sub onNDArray(ByVal arr As INDArray) Implements NDArrayCallback.onNDArray
				count.incrementAndGet()
			End Sub
		End Class



	End Class

End Namespace