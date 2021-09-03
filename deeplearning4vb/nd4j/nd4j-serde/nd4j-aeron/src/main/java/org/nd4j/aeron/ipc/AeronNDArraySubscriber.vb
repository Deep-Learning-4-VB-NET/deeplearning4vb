﻿Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports FragmentAssembler = io.aeron.FragmentAssembler
Imports Subscription = io.aeron.Subscription
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports CloseHelper = org.agrona.CloseHelper
Imports SigInt = org.agrona.concurrent.SigInt
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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
'ORIGINAL LINE: @Data @Builder public class AeronNDArraySubscriber implements AutoCloseable
	Public Class AeronNDArraySubscriber
		Implements AutoCloseable

		' The channel (an endpoint identifier) to receive messages from
		Private channel As String
		' A unique identifier for a stream within a channel. Stream ID 0 is reserved
		' for internal use and should not be used by applications.
		Private streamId As Integer = -1
		' Maximum number of message fragments to receive during a single 'poll' operation
		Private fragmentLimitCount As Integer
		' Create a context, needed for client connection to media driver
		' A separate media driver process need to run prior to running this application
		Private ctx As Aeron.Context
		Private running As New AtomicBoolean(True)
'JAVA TO VB CONVERTER NOTE: The field init was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly init_Conflict As New AtomicBoolean(False)
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(AeronNDArraySubscriber))
		Private ndArrayCallback As NDArrayCallback
		Private aeron As Aeron
		Private subscription As Subscription
'JAVA TO VB CONVERTER NOTE: The field launched was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private launched_Conflict As New AtomicBoolean(False)
		Private executors As Executor



		Private Sub init()
			ctx = If(ctx Is Nothing, New Aeron.Context(), ctx)
			channel = If(channel Is Nothing, "aeron:udp?endpoint=localhost:40123", channel)
			fragmentLimitCount = If(fragmentLimitCount = 0, 1000, fragmentLimitCount)
			streamId = If(streamId = 0, 10, streamId)
			running = If(running Is Nothing, New AtomicBoolean(True), running)
			If ndArrayCallback Is Nothing Then
				Throw New System.InvalidOperationException("NDArray callback must be specified in the builder.")
			End If
			init_Conflict.set(True)
			log.info("Channel subscriber " & channel & " and stream id " & streamId)
			launched_Conflict = New AtomicBoolean(False)
		End Sub


		''' <summary>
		''' Returns true if the subscriber
		''' is launched or not </summary>
		''' <returns> true if the subscriber is launched, false otherwise </returns>
		Public Overridable Function launched() As Boolean
			If launched_Conflict Is Nothing Then
				launched_Conflict = New AtomicBoolean(False)
			End If
			Return launched_Conflict.get()
		End Function

		''' <summary>
		''' Launch a background thread
		''' that subscribes to  the aeron context </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void launch() throws Exception
		Public Overridable Sub launch()
			If init_Conflict.get() Then
				Return
			End If
			' Register a SIGINT handler for graceful shutdown.
			If Not init_Conflict.get() Then
				init()
			End If

			log.info("Subscribing to " & channel & " on stream Id " & streamId)
			log.info("Using aeron directory " & ctx.aeronDirectoryName())

			' Register a SIGINT handler for graceful shutdown.
			SigInt.register(Sub() running.set(False))

			' Create an Aeron instance with client-provided context configuration, connect to the
			' media driver, and add a subscription for the given channel and stream using the supplied
			' dataHandler method, which will be called with new messages as they are received.
			' The Aeron and Subscription classes implement AutoCloseable, and will automatically
			' clean up resources when this try block is finished.
			'Note here that we are either creating 1 or 2 subscriptions.
			'The first one is a  normal 1 subscription listener.
			'The second one is when we want to send responses

			If channel Is Nothing Then
				Throw New System.InvalidOperationException("No channel for subscriber defined")
			End If
			If streamId <= 0 Then
				Throw New System.InvalidOperationException("No stream for subscriber defined")
			End If
			If aeron Is Nothing Then
				Throw New System.InvalidOperationException("No aeron instance defined")
			End If
			Dim started As Boolean = False
			Do While Not started
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: try (final io.aeron.Subscription subscription = aeron.addSubscription(channel, streamId))
				Try
						Using subscription As Subscription = aeron.addSubscription(channel, streamId)
						Me.subscription = subscription
						log.info("Beginning subscribe on channel " & channel & " and stream " & streamId)
						AeronUtil.subscriberLoop(New FragmentAssembler(New NDArrayFragmentHandler(ndArrayCallback)), fragmentLimitCount, running, launched_Conflict).accept(subscription)
						started = True
        
						End Using
				Catch e As Exception
					log.warn("Unable to connect...trying again on channel " & channel, e)
				End Try
			Loop

		End Sub

		''' <summary>
		''' Returns the connection uri in the form of:
		''' host:port:streamId
		''' @return
		''' </summary>
		Public Overridable Function connectionUrl() As String
			Dim split() As String = channel.Replace("aeron:udp?endpoint=", "").Split(":", True)
			Dim host As String = split(0)
			Dim port As Integer = Integer.Parse(split(1))
			Return AeronConnectionInformation.of(host, port, streamId).ToString()
		End Function



		''' <summary>
		''' Start a subscriber in another thread
		''' based on the given parameters </summary>
		''' <param name="aeron"> the aeron instance to use </param>
		''' <param name="host"> the host opName to bind to </param>
		''' <param name="port"> the port to bind to </param>
		''' <param name="callback"> the call back to use for the subscriber </param>
		''' <param name="streamId"> the stream id to subscribe to </param>
		''' <returns> the subscriber reference </returns>
		Public Shared Function startSubscriber(ByVal aeron As Aeron, ByVal host As String, ByVal port As Integer, ByVal callback As NDArrayCallback, ByVal streamId As Integer, ByVal running As AtomicBoolean) As AeronNDArraySubscriber

			Dim subscriber As AeronNDArraySubscriber = AeronNDArraySubscriber.builder().streamId(streamId).aeron(aeron).channel(AeronUtil.aeronChannel(host, port)).running(running).ndArrayCallback(callback).build()


			Dim t As New Thread(Sub()
			Try
				subscriber.launch()
			Catch e As Exception
				log.error("",e)
			End Try
			End Sub)

			t.Start()


			Return subscriber
		End Function

		''' <summary>
		''' Start a subscriber in another thread
		''' based on the given parameters </summary>
		''' <param name="context"> the context to use </param>
		''' <param name="host"> the host opName to bind to </param>
		''' <param name="port"> the port to bind to </param>
		''' <param name="callback"> the call back to use for the subscriber </param>
		''' <param name="streamId"> the stream id to subscribe to </param>
		''' <returns> the subscriber reference </returns>
		Public Shared Function startSubscriber(ByVal context As Aeron.Context, ByVal host As String, ByVal port As Integer, ByVal callback As NDArrayCallback, ByVal streamId As Integer, ByVal running As AtomicBoolean) As AeronNDArraySubscriber

			Dim subscriber As AeronNDArraySubscriber = AeronNDArraySubscriber.builder().streamId(streamId).ctx(context).channel(AeronUtil.aeronChannel(host, port)).running(running).ndArrayCallback(callback).build()


			Dim t As New Thread(Sub()
			Try
				subscriber.launch()
			Catch e As Exception
				log.error("",e)
			End Try
			End Sub)

			t.Start()


			Return subscriber
		End Function


		''' <summary>
		''' Closes this resource, relinquishing any underlying resources.
		''' This method is invoked automatically on objects managed by the
		''' {@code try}-with-resources statement.
		''' <para>
		''' </para>
		''' <para>While this interface method is declared to throw {@code
		''' Exception}, implementers are <em>strongly</em> encouraged to
		''' declare concrete implementations of the {@code close} method to
		''' throw more specific exceptions, or to throw no exception at all
		''' if the close operation cannot fail.
		''' </para>
		''' <para>
		''' </para>
		''' <para> Cases where the close operation may fail require careful
		''' attention by implementers. It is strongly advised to relinquish
		''' the underlying resources and to internally <em>mark</em> the
		''' resource as closed, prior to throwing the exception. The {@code
		''' close} method is unlikely to be invoked more than once and so
		''' this ensures that the resources are released in a timely manner.
		''' Furthermore it reduces problems that could arise when the resource
		''' wraps, or is wrapped, by another resource.
		''' </para>
		''' <para>
		''' </para>
		''' <para><em>Implementers of this interface are also strongly advised
		''' to not have the {@code close} method throw {@link
		''' InterruptedException}.</em>
		''' </para>
		''' <para>
		''' This exception interacts with a thread's interrupted status,
		''' and runtime misbehavior is likely to occur if an {@code
		''' InterruptedException} is {@link Throwable#addSuppressed
		''' suppressed}.
		''' </para>
		''' <para>
		''' More generally, if it would cause problems for an
		''' exception to be suppressed, the {@code AutoCloseable.close}
		''' method should not throw it.
		''' </para>
		''' <para>
		''' </para>
		''' <para>Note that unlike the <seealso cref="Closeable.close close"/>
		''' method of <seealso cref="System.IDisposable"/>, this {@code close} method
		''' is <em>not</em> required to be idempotent.  In other words,
		''' calling this {@code close} method more than once may have some
		''' visible side effect, unlike {@code Closeable.close} which is
		''' required to have no effect if called more than once.
		''' </para>
		''' <para>
		''' However, implementers of this interface are strongly encouraged
		''' to make their {@code close} methods idempotent.
		''' 
		''' </para>
		''' </summary>
		''' <exception cref="Exception"> if this resource cannot be closed </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()
			CloseHelper.quietClose(subscription)
		End Sub
	End Class



End Namespace