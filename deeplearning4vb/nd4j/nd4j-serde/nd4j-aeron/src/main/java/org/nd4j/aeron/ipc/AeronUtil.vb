Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports Image = io.aeron.Image
Imports Subscription = io.aeron.Subscription
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports ThreadingMode = io.aeron.driver.ThreadingMode
Imports FragmentHandler = io.aeron.logbuffer.FragmentHandler
Imports HeaderFlyweight = io.aeron.protocol.HeaderFlyweight
Imports BitUtil = org.agrona.BitUtil
Imports LangUtil = org.agrona.LangUtil
Imports BusySpinIdleStrategy = org.agrona.concurrent.BusySpinIdleStrategy
Imports IdleStrategy = org.agrona.concurrent.IdleStrategy

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


	Public Class AeronUtil

		''' <summary>
		''' Get a media driver context
		''' for sending ndarrays
		''' based on a given length
		''' where length is the length (number of elements)
		''' in the ndarrays hat are being sent </summary>
		''' <param name="length"> the length to based the ipc length </param>
		''' <returns> the media driver context based on the given length </returns>
		Public Shared Function getMediaDriverContext(ByVal length As Integer) As MediaDriver.Context
			'length of array * sizeof(float)
			Dim ipcLength As Integer = length * 16
			'padding for NDArrayMessage
			ipcLength += 64
			'must be a power of 2
			ipcLength *= 2
			'ipc length must be positive power of 2
			Do While Not BitUtil.isPowerOfTwo(ipcLength)
				ipcLength += 2
			Loop
			' System.setProperty("aeron.term.buffer.size",String.valueOf(ipcLength));
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.driver.MediaDriver.Context ctx = new io.aeron.driver.MediaDriver.Context().threadingMode(io.aeron.driver.ThreadingMode.@SHARED).dirDeleteOnStart(true).dirDeleteOnShutdown(true).conductorIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).receiverIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy()).senderIdleStrategy(new org.agrona.concurrent.BusySpinIdleStrategy());
			Dim ctx As MediaDriver.Context = (New MediaDriver.Context()).threadingMode(ThreadingMode.SHARED).dirDeleteOnStart(True).dirDeleteOnShutdown(True).conductorIdleStrategy(New BusySpinIdleStrategy()).receiverIdleStrategy(New BusySpinIdleStrategy()).senderIdleStrategy(New BusySpinIdleStrategy())
			Return ctx
		End Function


		''' <summary>
		''' Aeron channel generation </summary>
		''' <param name="host"> the host </param>
		''' <param name="port"> the port </param>
		''' <returns> the aeron channel via udp </returns>
		Public Shared Function aeronChannel(ByVal host As String, ByVal port As Integer) As String
			Return String.Format("aeron:udp?endpoint={0}:{1:D}", host, port)
		End Function

		''' <summary>
		''' Return a reusable, parametrized
		''' event loop that calls a
		''' default idler
		''' when no messages are received
		''' </summary>
		''' <param name="fragmentHandler"> to be called back for each message. </param>
		''' <param name="limit">           passed to <seealso cref="Subscription.poll(FragmentHandler, Integer)"/> </param>
		''' <param name="running">         indication for loop </param>
		''' <returns> loop function </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static java.util.function.Consumer<io.aeron.Subscription> subscriberLoop(final io.aeron.logbuffer.FragmentHandler fragmentHandler, final int limit, final java.util.concurrent.atomic.AtomicBoolean running, final java.util.concurrent.atomic.AtomicBoolean launched)
		Public Shared Function subscriberLoop(ByVal fragmentHandler As FragmentHandler, ByVal limit As Integer, ByVal running As AtomicBoolean, ByVal launched As AtomicBoolean) As System.Action(Of Subscription)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.agrona.concurrent.IdleStrategy idleStrategy = new org.agrona.concurrent.BusySpinIdleStrategy();
			Dim idleStrategy As IdleStrategy = New BusySpinIdleStrategy()
			Return subscriberLoop(fragmentHandler, limit, running, idleStrategy, launched)
		End Function

		''' <summary>
		''' Return a reusable, parameterized event
		''' loop that calls and idler
		''' when no messages are received
		''' </summary>
		''' <param name="fragmentHandler"> to be called back for each message. </param>
		''' <param name="limit">           passed to <seealso cref="Subscription.poll(FragmentHandler, Integer)"/> </param>
		''' <param name="running">         indication for loop </param>
		''' <param name="idleStrategy">    to use for loop </param>
		''' <returns> loop function </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static java.util.function.Consumer<io.aeron.Subscription> subscriberLoop(final io.aeron.logbuffer.FragmentHandler fragmentHandler, final int limit, final java.util.concurrent.atomic.AtomicBoolean running, final org.agrona.concurrent.IdleStrategy idleStrategy, final java.util.concurrent.atomic.AtomicBoolean launched)
		Public Shared Function subscriberLoop(ByVal fragmentHandler As FragmentHandler, ByVal limit As Integer, ByVal running As AtomicBoolean, ByVal idleStrategy As IdleStrategy, ByVal launched As AtomicBoolean) As System.Action(Of Subscription)
			Return Sub(subscription)
			Try
				Do While running.get()
					idleStrategy.idle(subscription.poll(fragmentHandler, limit))
					launched.set(True)
				Loop
			Catch ex As Exception
				LangUtil.rethrowUnchecked(ex)
			End Try
			End Sub
		End Function

		''' <summary>
		''' Return a reusable, parameterized <seealso cref="FragmentHandler"/> that prints to stdout
		''' </summary>
		''' <param name="streamId"> to show when printing </param>
		''' <returns> subscription data handler function that prints the message contents </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static io.aeron.logbuffer.FragmentHandler printStringMessage(final int streamId)
		Public Shared Function printStringMessage(ByVal streamId As Integer) As FragmentHandler
			Return Sub(buffer, offset, length, header)
			Dim data(length - 1) As SByte
			buffer.getBytes(offset, data)

			Console.WriteLine(String.Format("Message to stream {0:D} from session {1:D} ({2:D}@{3:D}) <<{4}>>", streamId, header.sessionId(), length, offset, StringHelper.NewString(data)))
			End Sub
		End Function


		''' <summary>
		''' Generic error handler that just prints message to stdout.
		''' </summary>
		''' <param name="channel">   for the error </param>
		''' <param name="streamId">  for the error </param>
		''' <param name="sessionId"> for the error, if source </param>
		''' <param name="message">   indicating what the error was </param>
		''' <param name="cause">     of the error </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static void printError(final String channel, final int streamId, final int sessionId, final String message, final io.aeron.protocol.HeaderFlyweight cause)
		Public Shared Sub printError(ByVal channel As String, ByVal streamId As Integer, ByVal sessionId As Integer, ByVal message As String, ByVal cause As HeaderFlyweight)
			Console.WriteLine(message)
		End Sub

		''' <summary>
		''' Print the rates to stdout
		''' </summary>
		''' <param name="messagesPerSec"> being reported </param>
		''' <param name="bytesPerSec">    being reported </param>
		''' <param name="totalMessages">  being reported </param>
		''' <param name="totalBytes">     being reported </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static void printRate(final double messagesPerSec, final double bytesPerSec, final long totalMessages, final long totalBytes)
		Public Shared Sub printRate(ByVal messagesPerSec As Double, ByVal bytesPerSec As Double, ByVal totalMessages As Long, ByVal totalBytes As Long)
			Console.WriteLine(String.Format("{0:g02} msgs/sec, {1:g02} bytes/sec, totals {2:D} messages {3:D} MB", messagesPerSec, bytesPerSec, totalMessages, totalBytes \ (1024 * 1024)))
		End Sub

		''' <summary>
		''' Print the information for an available image to stdout.
		''' </summary>
		''' <param name="image"> that has been created </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static void printAvailableImage(final io.aeron.Image image)
		Public Shared Sub printAvailableImage(ByVal image As Image)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.Subscription subscription = image.subscription();
			Dim subscription As Subscription = image.subscription()
			Console.WriteLine(String.Format("Available image on {0} streamId={1:D} sessionId={2:D} from {3}", subscription.channel(), subscription.streamId(), image.sessionId(), image.sourceIdentity()))
		End Sub

		''' <summary>
		''' Print the information for an unavailable image to stdout.
		''' </summary>
		''' <param name="image"> that has gone inactive </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static void printUnavailableImage(final io.aeron.Image image)
		Public Shared Sub printUnavailableImage(ByVal image As Image)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.Subscription subscription = image.subscription();
			Dim subscription As Subscription = image.subscription()
			Console.WriteLine(String.Format("Unavailable image on {0} streamId={1:D} sessionId={2:D}", subscription.channel(), subscription.streamId(), image.sessionId()))
		End Sub

		Private Shared ReadOnly conductorCount As New AtomicInteger()
		Private Shared ReadOnly receiverCount As New AtomicInteger()
		Private Shared ReadOnly senderCount As New AtomicInteger()
		Private Shared ReadOnly sharedNetworkCount As New AtomicInteger()
		Private Shared ReadOnly sharedThreadCount As New AtomicInteger()

		''' <summary>
		''' Set all Aeron thread factories to create daemon threads (to stop aeron threads from keeping JVM alive
		''' if all other threads have exited) </summary>
		''' <param name="mediaDriverCtx"> Media driver context to configure </param>
		Public Shared WriteOnly Property DaemonizedThreadFactories As MediaDriver.Context
			Set(ByVal mediaDriverCtx As MediaDriver.Context)
    
				'Set thread factories so we can make the Aeron threads daemon threads (some are not by default)
				mediaDriverCtx.conductorThreadFactory(Function(r)
				Dim t As New Thread(r)
				t.setDaemon(True)
				t.setName("aeron-conductor-thread-" & conductorCount.getAndIncrement())
				Return t
				End Function)
    
				mediaDriverCtx.receiverThreadFactory(Function(r)
				Dim t As New Thread(r)
				t.setDaemon(True)
				t.setName("aeron-receiver-thread-" & receiverCount.getAndIncrement())
				Return t
				End Function)
    
    
				mediaDriverCtx.senderThreadFactory(Function(r)
				Dim t As New Thread(r)
				t.setDaemon(True)
				t.setName("aeron-sender-thread-" & senderCount.getAndIncrement())
				Return t
				End Function)
    
    
				mediaDriverCtx.sharedNetworkThreadFactory(Function(r)
				Dim t As New Thread(r)
				t.setDaemon(True)
				t.setName("aeron-shared-network-thread-" & sharedNetworkCount.getAndIncrement())
				Return t
				End Function)
    
				mediaDriverCtx.sharedThreadFactory(Function(r)
				Dim t As New Thread(r)
				t.setDaemon(True)
				t.setName("aeron-shared-thread-" & sharedThreadCount.getAndIncrement())
				Return t
				End Function)
			End Set
		End Property

		Public Shared WriteOnly Property DaemonizedThreadFactories As Aeron.Context
			Set(ByVal aeronCtx As Aeron.Context)
				aeronCtx.threadFactory(Function(r)
				Dim t As New Thread(r)
				t.setDaemon(True)
				Return t
				End Function)
			End Set
		End Property
	End Class

End Namespace