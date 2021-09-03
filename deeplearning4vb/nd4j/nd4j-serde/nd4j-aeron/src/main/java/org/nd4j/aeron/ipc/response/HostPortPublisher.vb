Imports System
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports Publication = io.aeron.Publication
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports CloseHelper = org.agrona.CloseHelper
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports AeronNDArrayPublisher = org.nd4j.aeron.ipc.AeronNDArrayPublisher
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

Namespace org.nd4j.aeron.ipc.response


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder public class HostPortPublisher implements AutoCloseable
	Public Class HostPortPublisher
		Implements AutoCloseable

		Private uriToSend As String
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
		Private publicationTimeout As Integer

		Private Sub init()
			publicationTimeout = If(publicationTimeout = 0, 100, publicationTimeout)
			channel = If(channel Is Nothing, "aeron:udp?endpoint=localhost:40123", channel)
			streamId = If(streamId = 0, 10, streamId)
				If ctx Is Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ctx = ctx = new io.aeron.Aeron.Context();
					ctx = New Aeron.Context()
				Else
					ctx = ctx
				End If
			init_Conflict = True
			log.info("Channel publisher" & channel & " and stream " & streamId)
		End Sub


		Public Overridable Sub send()
			If Not init_Conflict Then
				init()
			End If

			' Create an Aeron instance with client-provided context configuration and connect to the
			' media driver, and create a Publication.  The Aeron and Publication classes implement
			' AutoCloseable, and will automatically clean up resources when this try block is finished.
			If aeron Is Nothing Then
				aeron = Aeron.connect(ctx)
			End If

			Do While publication Is Nothing
				Try
					publication = aeron.addPublication(channel, streamId)
					log.info("Publication created on channel " & channel)
				Catch e As Exception
					log.warn("Trying to connect again on channel " & channel)
				End Try
			Loop


			Dim buffer As New UnsafeBuffer(uriToSend.GetBytes())
			' Try to publish the buffer. 'offer' is a non-blocking call.
			' If it returns less than 0, the message was not sent, and the offer should be retried.
			Dim result As Long
			log.info("Begin publish " & channel & " and stream " & streamId)
			Dim timesFailed As Integer = 0
			result = publication.offer(buffer, 0, buffer.capacity())
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((result = publication.offer(buffer, 0, buffer.capacity())) < 0L)
			Do While result < 0L
				If result = Publication.BACK_PRESSURED AndAlso timesFailed Mod 1000 = 0 Then
					log.info("Offer failed due to back pressure " & channel & " and stream " & streamId)

				ElseIf result = Publication.NOT_CONNECTED AndAlso timesFailed Mod 1000 = 0 Then
					log.info("Offer failed because publisher is not connected to subscriber " & channel & " and stream " & streamId)

				ElseIf result = Publication.ADMIN_ACTION AndAlso timesFailed Mod 1000 = 0 Then
					log.info("Offer failed because of an administration action in the system " & channel & " and stream " & streamId)

				ElseIf result = Publication.CLOSED AndAlso timesFailed Mod 1000 = 0 Then
					log.info("Offer failed publication is closed " & channel & " and stream " & streamId)

				ElseIf timesFailed Mod 1000 = 0 Then
					log.info("Offer failed due to unknown reason on channel " & channel & " and stream " & streamId)
				End If
				Try
					Thread.Sleep(publicationTimeout)
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
				End Try
				timesFailed += 1

					result = publication.offer(buffer, 0, buffer.capacity())
			Loop


			log.info("Done sending uri " & uriToSend)
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