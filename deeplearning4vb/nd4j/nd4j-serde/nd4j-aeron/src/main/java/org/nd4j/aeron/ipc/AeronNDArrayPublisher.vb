Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports Publication = io.aeron.Publication
Imports DriverTimeoutException = io.aeron.exceptions.DriverTimeoutException
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports CloseHelper = org.agrona.CloseHelper
Imports DirectBuffer = org.agrona.DirectBuffer
Imports BusySpinIdleStrategy = org.agrona.concurrent.BusySpinIdleStrategy
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports NDArrayMessageChunk = org.nd4j.aeron.ipc.chunk.NDArrayMessageChunk
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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


	''' <summary>
	''' NDArray publisher
	''' for aeron
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder public class AeronNDArrayPublisher implements AutoCloseable
	Public Class AeronNDArrayPublisher
		Implements AutoCloseable

		' A unique identifier for a stream within a channel. Stream ID 0 is reserved
		' for internal use and should not be used by applications.
		Private streamId As Integer
		' The channel (an endpoint identifier) to send the message to
		Private channel As String
'JAVA TO VB CONVERTER NOTE: The field init was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private init_Conflict As Boolean = False
		Private ctx As Aeron.Context
		Private aeron As Aeron
		Private publication As Publication
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(AeronNDArrayPublisher))
		Public Const NUM_RETRIES As Integer = 100
		Private compress As Boolean = True
		Private Shared ReadOnly busySpinIdleStrategy As New BusySpinIdleStrategy()
		Private publishRetryTimeOut As Integer = 3000

		Private Sub init()
			channel = If(channel Is Nothing, "aeron:udp?endpoint=localhost:40123", channel)
			streamId = If(streamId = 0, 10, streamId)
			publishRetryTimeOut = If(publishRetryTimeOut = 0, 300000, publishRetryTimeOut)
				If ctx Is Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ctx = ctx = new io.aeron.Aeron.Context();
					ctx = New Aeron.Context()
				Else
					ctx = ctx
				End If
			init_Conflict = True
			log.info("Channel publisher" & channel & " and stream " & streamId & " with time out " & publishRetryTimeOut)
		End Sub

		''' <summary>
		''' Publish an ndarray
		''' to an aeron channel </summary>
		''' <param name="message"> </param>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void publish(NDArrayMessage message) throws Exception
		Public Overridable Sub publish(ByVal message As NDArrayMessage)
			If Not init_Conflict Then
				init()
			End If
			' Create a context, needed for client connection to media driver
			' A separate media driver process needs to be running prior to starting this application

			' Create an Aeron instance with client-provided context configuration and connect to the
			' media driver, and create a Publication.  The Aeron and Publication classes implement
			' AutoCloseable, and will automatically clean up resources when this try block is finished.
			Dim connected As Boolean = False
			If aeron Is Nothing Then
				Try
					Do While Not connected
						aeron = Aeron.connect(ctx)
						connected = True
					Loop
				Catch e As Exception
					log.warn("Reconnecting on publisher...failed to connect")
				End Try
			End If

			Dim connectionTries As Integer = 0
			Do While publication Is Nothing AndAlso connectionTries < NUM_RETRIES
				Try
					publication = aeron.addPublication(channel, streamId)
					log.info("Created publication on channel " & channel & " and stream " & streamId)
				Catch e As DriverTimeoutException
					Thread.Sleep(1000 * (connectionTries + 1))
					log.warn("Failed to connect due to driver time out on channel " & channel & " and stream " & streamId & "...retrying in " & connectionTries & " seconds")
					connectionTries += 1
				End Try
			Loop

			If Not connected AndAlso connectionTries >= 3 OrElse publication Is Nothing Then
				Throw New System.InvalidOperationException("Publisher unable to connect to channel " & channel & " and stream " & streamId)
			End If


			' Allocate enough buffer size to hold maximum message length
			' The UnsafeBuffer class is part of the Agrona library and is used for efficient buffer management
			log.info("Publishing to " & channel & " on stream Id " & streamId)
			'ensure default values are set
			Dim arr As INDArray = message.getArr()
			If isCompress() Then
				Do While Not message.getArr().isCompressed()
					Nd4j.Compressor.compressi(arr, "GZIP")
				Loop
			End If



			'array is large, need to segment
			If NDArrayMessage.byteBufferSizeForMessage(message) >= publication.maxMessageLength() Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim chunks() As NDArrayMessageChunk = NDArrayMessage.chunks(message, publication.maxMessageLength() / 128)
				For i As Integer = 0 To chunks.Length - 1
					Dim sendBuff As ByteBuffer = NDArrayMessageChunk.toBuffer(chunks(i))
					CType(sendBuff, Buffer).rewind()
					Dim buffer As DirectBuffer = New UnsafeBuffer(sendBuff)
					sendBuffer(buffer)
				Next i
			Else
				'send whole array
				Dim buffer As DirectBuffer = NDArrayMessage.toBuffer(message)
				sendBuffer(buffer)

			End If

		End Sub



'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void sendBuffer(org.agrona.DirectBuffer buffer) throws Exception
		Private Sub sendBuffer(ByVal buffer As DirectBuffer)
			' Try to publish the buffer. 'offer' is a non-blocking call.
			' If it returns less than 0, the message was not sent, and the offer should be retried.
			Dim result As Long
			Dim tries As Integer = 0
			result = publication.offer(buffer, 0, buffer.capacity())
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((result = publication.offer(buffer, 0, buffer.capacity())) < 0L && tries < 5)
			Do While result < 0L AndAlso tries < 5
				If result = Publication.BACK_PRESSURED Then
					log.info("Offer failed due to back pressure")
				ElseIf result = Publication.NOT_CONNECTED Then
					log.info("Offer failed because publisher is not connected to subscriber " & channel & " and stream " & streamId)
				ElseIf result = Publication.ADMIN_ACTION Then
					log.info("Offer failed because of an administration action in the system and channel" & channel & " and stream " & streamId)
				ElseIf result = Publication.CLOSED Then
					log.info("Offer failed publication is closed and channel" & channel & " and stream " & streamId)
				Else
					log.info(" Offer failed due to unknown reason and channel" & channel & " and stream " & streamId)
				End If



				Thread.Sleep(publishRetryTimeOut)
				tries += 1

					result = publication.offer(buffer, 0, buffer.capacity())
			Loop

			If tries >= 5 AndAlso result = 0 Then
				Throw New System.InvalidOperationException("Failed to send message")
			End If

		End Sub

		''' <summary>
		''' Publish an ndarray to an aeron channel </summary>
		''' <param name="arr"> </param>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void publish(org.nd4j.linalg.api.ndarray.INDArray arr) throws Exception
		Public Overridable Sub publish(ByVal arr As INDArray)
			publish(NDArrayMessage.wholeArrayUpdate(arr))
		End Sub


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
			If publication IsNot Nothing Then
				CloseHelper.quietClose(publication)
			End If

		End Sub
	End Class

End Namespace