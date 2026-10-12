Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports Aeron = io.aeron.Aeron
Imports FragmentAssembler = io.aeron.FragmentAssembler
Imports Publication = io.aeron.Publication
Imports Subscription = io.aeron.Subscription
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports Header = io.aeron.logbuffer.Header
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports CloseHelper = org.agrona.CloseHelper
Imports DirectBuffer = org.agrona.DirectBuffer
Imports IdleStrategy = org.agrona.concurrent.IdleStrategy
Imports SleepingIdleStrategy = org.agrona.concurrent.SleepingIdleStrategy
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports org.nd4j.parameterserver.distributed.messages
Imports MeaningfulMessage = org.nd4j.parameterserver.distributed.messages.MeaningfulMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.messages.VoidMessage

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

Namespace org.nd4j.parameterserver.distributed.transport


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public abstract class BaseTransport implements Transport
	<Obsolete>
	Public MustInherit Class BaseTransport
		Implements Transport

		Public MustOverride ReadOnly Property TargetIndex As Short Implements Transport.getTargetIndex
		Public MustOverride ReadOnly Property ShardIndex As Short Implements Transport.getShardIndex
		Protected Friend voidConfiguration As VoidConfiguration
		Protected Friend nodeRole As NodeRole

		Protected Friend aeron As Aeron
		Protected Friend context As Aeron.Context

		Protected Friend unicastChannelUri As String

'JAVA TO VB CONVERTER NOTE: The field ip was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend ip_Conflict As String
'JAVA TO VB CONVERTER NOTE: The field port was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend port_Conflict As Integer = 0

		' TODO: move this to singleton holder
		Protected Friend driver As MediaDriver

		Protected Friend publicationForShards As Publication
		Protected Friend publicationForClients As Publication

		Protected Friend subscriptionForShards As Subscription
		Protected Friend subscriptionForClients As Subscription

		Protected Friend messageHandlerForShards As FragmentAssembler
		Protected Friend messageHandlerForClients As FragmentAssembler

		Protected Friend messages As New LinkedBlockingQueue(Of VoidMessage)()

		Protected Friend completed As IDictionary(Of Long, MeaningfulMessage) = New ConcurrentDictionary(Of Long, MeaningfulMessage)()

		Protected Friend runner As New AtomicBoolean(True)

		' service threads where poll will happen
		Protected Friend threadA As Thread
		Protected Friend threadB As Thread

		Protected Friend clipboard As Clipboard

		Protected Friend frameCount As New AtomicLong(0)

		' TODO: make this configurable?
		Protected Friend idler As IdleStrategy = New SleepingIdleStrategy(1000)
		Protected Friend feedbackIdler As IdleStrategy = New SleepingIdleStrategy(100000)

		Protected Friend threadingModel As ThreadingModel = ThreadingModel.DEDICATED_THREADS

		Protected Friend originatorId As Long

		' TODO: make this auto-configurable
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected short targetIndex = 0;
		Protected Friend targetIndex As Short = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected short shardIndex = 0;
		Protected Friend shardIndex As Short = 0

		Public Overridable ReadOnly Property OwnOriginatorId As Long Implements Transport.getOwnOriginatorId
			Get
				Return originatorId
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.parameterserver.distributed.messages.MeaningfulMessage sendMessageAndGetResponse(@NonNull VoidMessage message)
		Public Overridable Function sendMessageAndGetResponse(ByVal message As VoidMessage) As MeaningfulMessage
			Dim startTime As Long = DateTimeHelper.CurrentUnixTimeMillis()

			Dim taskId As Long = message.getTaskId()
			sendCommandToShard(message)
			Dim cnt As New AtomicLong(0)

			'        log.info("Sent message to shard: {}, taskId: {}, originalId: {}", message.getClass().getSimpleName(), message.getTaskId(), taskId);

			Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()

			Dim msg As MeaningfulMessage
			msg = completed(taskId)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((msg = completed.get(taskId)) == null)
			Do While msg Is Nothing
				Try
					'Thread.sleep(voidConfiguration.getResponseTimeframe());
					feedbackIdler.idle()

					If DateTimeHelper.CurrentUnixTimeMillis() - currentTime > voidConfiguration.getResponseTimeout() Then
						log.info("Resending request for taskId [{}]", taskId)
						message.incrementRetransmitCount()

						' TODO: make retransmit threshold configurable
						If message.getRetransmitCount() > 20 Then
							Throw New Exception("Giving up on message delivery...")
						End If

						Return sendMessageAndGetResponse(message)
					End If
				Catch e As Exception
					Throw New Exception(e)
				End Try
					msg = completed(taskId)
			Loop

			completed.Remove(taskId)

			Dim endTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim timeSpent As Long = endTime - startTime

			If TypeOf message Is Frame AndAlso frameCount.incrementAndGet() Mod 1000 = 0 Then
				log.info("Frame of {} messages [{}] processed in {} ms", CType(message, Frame).size(), message.getTaskId(), timeSpent)
			End If


			Return msg
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setIpAndPort(@NonNull String ip, int port)
		Public Overridable Sub setIpAndPort(ByVal ip As String, ByVal port As Integer) Implements Transport.setIpAndPort
			Me.ip_Conflict = ip
			Me.port_Conflict = port
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void sendMessage(@NonNull VoidMessage message)
		Public Overridable Sub sendMessage(ByVal message As VoidMessage)
			Select Case message.getMessageType()
				' messages 0..9 inclusive are reserved for Client->Shard commands
				Case 0, 1, 2, 3, 4, 5, 6, 7, 8, 9
					' TODO: check, if current role is Shard itself, in this case we want to modify command queue directly, to reduce network load
					' this command is possible to issue from any node role
					'log.info("Sending message to Shard: {}; Messages size: {}", message.getClass().getSimpleName(), messages.size());
					'while (messages.size() > 100) {
					'    log.info("sI_{} [{}] going to sleep..", shardIndex, nodeRole );
					'    idler.idle();
					'}

					If message.isBlockingMessage() Then
						' we issue blocking message, but we don't care about response
						sendMessageAndGetResponse(message)
					Else
						sendCommandToShard(message)
					End If
				' messages 10..19 inclusive are reserved for Shard->Clients commands
				Case 10, 11, 12, 13, 19
					' this command is possible to issue only from Shard
					'log.info("Sending feedback message to Client: {}", message.getClass().getSimpleName());
					sendFeedbackToClient(message)
				' messages 20..29 inclusive are reserved for Shard->Shard commands
				Case 20, 21, 22, 28
					'log.info("Sending message to ALL Shards: {}", message.getClass().getSimpleName());
					sendCoordinationCommand(message)
				Case Else
					Throw New Exception("Unknown messageType passed for delivery")
			End Select
		End Sub

		''' <summary>
		''' This message handler is responsible for receiving messages on Shard side
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="offset"> </param>
		''' <param name="length"> </param>
		''' <param name="header"> </param>
		Protected Friend Overridable Sub shardMessageHandler(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal length As Integer, ByVal header As Header)
			''' <summary>
			''' All incoming messages here are supposed to be unicast messages.
			''' </summary>
			' TODO: implement fragmentation handler here PROBABLY. Or forbid messages > MTU?
			'log.info("shardMessageHandler message request incoming...");
			Dim data(length - 1) As SByte
			buffer.getBytes(offset, data)

			Dim message As VoidMessage = VoidMessage.fromBytes(data)
			If message.MessageType = 7 Then
				' if that's vector request message - it's special case, we don't send it to other shards yet
				'log.info("Shortcut for vector request");
				messages.add(message)
			Else
				' and send it away to other Shards
				publicationForShards.offer(buffer, offset, length)
			End If
		End Sub

		''' <summary>
		''' This message handler is responsible for receiving coordination messages on Shard side
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="offset"> </param>
		''' <param name="length"> </param>
		''' <param name="header"> </param>
		Protected Friend Overridable Sub internalMessageHandler(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal length As Integer, ByVal header As Header)
			''' <summary>
			''' All incoming internal messages are either op commands, or aggregation messages that are tied to commands
			''' </summary>
			Dim data(length - 1) As SByte
			buffer.getBytes(offset, data)

			Dim message As VoidMessage = VoidMessage.fromBytes(data)

			messages.add(message)

			'    log.info("internalMessageHandler message request incoming: {}", message.getClass().getSimpleName());
		End Sub

		''' <summary>
		''' This message handler is responsible for receiving messages on Client side </summary>
		''' <param name="buffer"> </param>
		''' <param name="offset"> </param>
		''' <param name="length"> </param>
		''' <param name="header"> </param>
		Protected Friend Overridable Sub clientMessageHandler(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal length As Integer, ByVal header As Header)
			''' <summary>
			'''  All incoming messages here are supposed to be "just messages", only unicast communication
			'''  All of them should implement MeaningfulMessage interface
			''' </summary>
			' TODO: to be implemented
			'  log.info("clientMessageHandler message request incoming");

			Dim data(length - 1) As SByte
			buffer.getBytes(offset, data)

			Dim message As MeaningfulMessage = DirectCast(VoidMessage.fromBytes(data), MeaningfulMessage)
			completed(message.TaskId) = message
		End Sub


		''' <param name="message"> </param>
		Public Overridable Sub sendMessageToAllShards(ByVal message As VoidMessage) Implements Transport.sendMessageToAllShards
			'        if (nodeRole != NodeRole.SHARD)
			'            throw new RuntimeException("This method shouldn't be called only from Shard context");

			'log.info("Sending message to All shards");

			message.TargetId = (Short) -1
			'publicationForShards.offer(message.asUnsafeBuffer());
			sendCoordinationCommand(message)
		End Sub

		''' <summary>
		''' This method does initialization of Transport instance
		''' </summary>
		''' <param name="voidConfiguration"> </param>
		''' <param name="clipboard"> </param>
		''' <param name="role"> </param>
		''' <param name="localIp"> </param>
		''' <param name="localPort"> </param>
		''' <param name="shardIndex"> </param>
		Public Overridable Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal clipboard As Clipboard, ByVal role As NodeRole, ByVal localIp As String, ByVal localPort As Integer, ByVal shardIndex As Short) Implements Transport.init
			'Runtime.getRuntime().addShutdownHook(new Thread(() -> shutdownSilent()));
		End Sub

		''' <summary>
		''' This method starts transport mechanisms.
		''' 
		''' PLEASE NOTE: init() method should be called prior to launch() call
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void launch(@NonNull ThreadingModel threading)
		Public Overridable Sub launch(ByVal threading As ThreadingModel) Implements Transport.launch
			Me.threadingModel = threading

			Select Case threading
				Case org.nd4j.parameterserver.distributed.transport.Transport.ThreadingModel.SINGLE_THREAD

					log.warn("SINGLE_THREAD model is used, performance will be significantly reduced")

					' single thread for all queues. shouldn't be used in real world
					threadA = New Thread(Sub()
					Do While runner.get()
						If subscriptionForShards IsNot Nothing Then
							subscriptionForShards.poll(messageHandlerForShards, 512)
						End If
						idler.idle(subscriptionForClients.poll(messageHandlerForClients, 512))
					Loop
					End Sub)

					threadA.setDaemon(True)
					threadA.Start()
				Case org.nd4j.parameterserver.distributed.transport.Transport.ThreadingModel.DEDICATED_THREADS
					' we start separate thread for each handler

					''' <summary>
					''' We definitely might use less conditional code here, BUT i'll keep it as is,
					''' only because we want code to be obvious for people
					''' </summary>
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicBoolean localRunner = new java.util.concurrent.atomic.AtomicBoolean(false);
					Dim localRunner As New AtomicBoolean(False)
					Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()

					If nodeRole = NodeRole.NONE Then
						Throw New ND4JIllegalStateException("No role is set for current node!")
					ElseIf nodeRole = NodeRole.SHARD OrElse nodeRole = NodeRole.BACKUP OrElse nodeRole = NodeRole.MASTER Then
						' // Shard or Backup uses two subscriptions

						' setting up thread for shard->client communication listener
						If messageHandlerForShards IsNot Nothing Then
							threadB = New Thread(Sub()
							Nd4j.AffinityManager.unsafeSetDevice(deviceId)
							Do While runner.get()
								idler.idle(subscriptionForShards.poll(messageHandlerForShards, 512))
							Loop
							End Sub)
							threadB.setDaemon(True)
							threadB.setName("VoidParamServer subscription threadB [" & nodeRole & "]")
						End If

						' setting up thread for inter-shard communication listener
						threadA = New Thread(Sub()
						localRunner.set(True)
						Nd4j.AffinityManager.unsafeSetDevice(deviceId)
						Do While runner.get()
							idler.idle(subscriptionForClients.poll(messageHandlerForClients, 512))
						Loop
						End Sub)

						If threadB IsNot Nothing Then

							threadB.setDaemon(True)
							threadB.setName("VoidParamServer subscription threadB [" & nodeRole & "]")
							threadB.Start()
						End If
					Else
						' setting up thread for shard->client communication listener
						threadA = New Thread(Sub()
						localRunner.set(True)
						Do While runner.get()
							idler.idle(subscriptionForClients.poll(messageHandlerForClients, 512))
						Loop
						End Sub)
					End If

					' all roles have threadA anyway
					'Nd4j.getAffinityManager().attachThreadToDevice(threadA,                                Nd4j.getAffinityManager().getDeviceForCurrentThread());
					threadA.setDaemon(True)
					threadA.setName("VoidParamServer subscription threadA [" & nodeRole & "]")
					threadA.Start()

					Do While Not localRunner.get()
						Try
							Thread.Sleep(50)
						Catch e As Exception
						End Try
					Loop
				Case org.nd4j.parameterserver.distributed.transport.Transport.ThreadingModel.SAME_THREAD
					' no additional threads at all, we do poll within takeMessage loop
					log.warn("SAME_THREAD model is used, performance will be dramatically reduced")
				Case Else
					Throw New System.InvalidOperationException("Unknown thread model: [" & threading.ToString() & "]")
			End Select
		End Sub


		Protected Friend Overridable Sub shutdownSilent()
			log.info("Shutting down Aeron infrastructure...")
			CloseHelper.quietClose(publicationForClients)
			CloseHelper.quietClose(publicationForShards)
			CloseHelper.quietClose(subscriptionForShards)
			CloseHelper.quietClose(subscriptionForClients)
			CloseHelper.quietClose(aeron)
			CloseHelper.quietClose(driver)
		End Sub

		''' <summary>
		''' This method stops transport system.
		''' </summary>
		Public Overridable Sub shutdown() Implements Transport.shutdown
			' Since Aeron's poll isn't blocking, all we need is just special flag
			runner.set(False)
			Try
				threadA.Join()

				If threadB IsNot Nothing Then
					threadB.Join()
				End If
			Catch e As Exception
				'
			End Try
			CloseHelper.quietClose(driver)
			Try
				Thread.Sleep(500)
			Catch e As Exception

			End Try
		End Sub

		''' <summary>
		''' This method saves incoming message to the Queue, for later dispatch from higher-level code, like actual TrainingFunction or VoidParameterServer itself
		''' </summary>
		''' <param name="message"> </param>
		Public Overridable Sub receiveMessage(ByVal message As VoidMessage) Implements Transport.receiveMessage
			Try
				log.info("Message received, saving...")
				messages.put(message)
			Catch e As Exception
				' do nothing
			End Try
		End Sub

		''' <summary>
		''' This method takes 1 message from "incoming messages" queue, blocking if queue is empty
		''' 
		''' @return
		''' </summary>
		Public Overridable Function takeMessage() As VoidMessage Implements Transport.takeMessage
			If threadingModel <> ThreadingModel.SAME_THREAD Then
				Try
					Return messages.take()
				Catch e As InterruptedException
					' probably we don't want to do anything here
					Return Nothing
				Catch e As Exception
					Throw New Exception(e)
				End Try
			Else
				''' <summary>
				''' PLEASE NOTE: This branch is suitable for debugging only, should never be used in wild life
				''' </summary>
				' we do inplace poll
				If subscriptionForShards IsNot Nothing Then
					subscriptionForShards.poll(messageHandlerForShards, 512)
				End If

				subscriptionForClients.poll(messageHandlerForClients, 512)

				Return messages.poll()
			End If
		End Function

		''' <summary>
		''' This method puts message into processing queue
		''' </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void putMessage(@NonNull VoidMessage message)
		Public Overridable Sub putMessage(ByVal message As VoidMessage)
			messages.add(message)
		End Sub

		''' <summary>
		''' This method peeks 1 message from "incoming messages" queue, returning null if queue is empty
		''' 
		''' PLEASE NOTE: This method is suitable for debug purposes only
		''' 
		''' @return
		''' </summary>
		Public Overridable Function peekMessage() As VoidMessage Implements Transport.peekMessage
			Return messages.peek()
		End Function

		''' <summary>
		''' This command is possible to issue only from Client
		''' </summary>
		''' <param name="message"> </param>
		Protected Friend Overridable Sub sendCommandToShard(ByVal message As VoidMessage)
			SyncLock Me
				' if this node is shard - we just step over TCP/IP infrastructure
				' TODO: we want LocalTransport to be used in such cases
				If nodeRole = NodeRole.SHARD Then
					message.TargetId = shardIndex
					messages.add(message)
					Return
				End If
        
        
				'log.info("Sending CS: {}", message.getClass().getCanonicalName());
        
				message.TargetId = targetIndex
				Dim buffer As DirectBuffer = message.asUnsafeBuffer()
        
				Dim result As Long = publicationForShards.offer(buffer)
        
				If result < 0 Then
					Dim i As Integer = 0
					Do While i < 5 AndAlso result < 0
						Try
							' TODO: make this configurable
							Thread.Sleep(1000)
						Catch e As Exception
						End Try
						result = publicationForShards.offer(buffer)
						i += 1
					Loop
				End If
        
				' TODO: handle retransmit & backpressure separately
        
				If result < 0 Then
					Throw New Exception("Unable to send message over the wire. Error code: " & result)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' This command is possible to issue only from Shard
		''' </summary>
		''' <param name="message"> </param>
		Protected Friend MustOverride Sub sendCoordinationCommand(ByVal message As VoidMessage)

		''' <summary>
		''' This command is possible to issue only from Shard </summary>
		''' <param name="message"> </param>
		Protected Friend MustOverride Sub sendFeedbackToClient(ByVal message As VoidMessage)

		Public Overridable Sub addClient(ByVal ip As String, ByVal port As Integer) Implements Transport.addClient
			'
		End Sub

		Public Overridable ReadOnly Property Ip As String Implements Transport.getIp
			Get
				Return ip_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property Port As Integer Implements Transport.getPort
			Get
				Return port_Conflict
			End Get
		End Property

		Public Overridable Function numberOfKnownClients() As Integer Implements Transport.numberOfKnownClients
			Return 0
		End Function

		Public Overridable Function numberOfKnownShards() As Integer Implements Transport.numberOfKnownShards
			Return 0
		End Function

		Public Overridable Sub addShard(ByVal ip As String, ByVal port As Integer) Implements Transport.addShard
			' no-op
		End Sub

		Public Overridable Sub sendMessageToAllClients(ByVal message As VoidMessage, ParamArray ByVal exclusions() As Long?) Implements Transport.sendMessageToAllClients
			' no-op
		End Sub
	End Class

End Namespace