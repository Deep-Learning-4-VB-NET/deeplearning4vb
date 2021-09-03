Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports Flowable = io.reactivex.Flowable
Imports Consumer = io.reactivex.functions.Consumer
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports org.nd4j.common.primitives
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports MeshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode
Imports VoidChunk = org.nd4j.parameterserver.distributed.v2.chunks.VoidChunk
Imports PropagationMode = org.nd4j.parameterserver.distributed.v2.enums.PropagationMode
Imports org.nd4j.parameterserver.distributed.v2.messages
Imports org.nd4j.parameterserver.distributed.v2.messages.history
Imports MeshUpdateMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.MeshUpdateMessage
Imports HandshakeRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.handshake.HandshakeRequest
Imports HandshakeResponse = org.nd4j.parameterserver.distributed.v2.messages.pairs.handshake.HandshakeResponse
Imports PingMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.ping.PingMessage
Imports PongMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.ping.PongMessage
Imports RestartCallback = org.nd4j.parameterserver.distributed.v2.transport.RestartCallback
Imports Transport = org.nd4j.parameterserver.distributed.v2.transport.Transport
Imports MeshOrganizer = org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer
Imports MessageSplitter = org.nd4j.parameterserver.distributed.v2.util.MessageSplitter
Imports Publisher = org.reactivestreams.Publisher
Imports Subscriber = org.reactivestreams.Subscriber

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

Namespace org.nd4j.parameterserver.distributed.v2.transport.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseTransport implements org.nd4j.parameterserver.distributed.v2.transport.Transport
	Public MustInherit Class BaseTransport
		Implements Transport

		Public MustOverride Sub sendMessage(ByVal message As org.nd4j.parameterserver.distributed.v2.messages.VoidMessage, ByVal id As String) Implements Transport.sendMessage
		Public MustOverride Function id() As String Implements Transport.id
		' this stream is for delivering messages from this host to other hosts in the network
		Protected Friend ReadOnly outgoingFlow As New MessageFlow(Of VoidMessage)()

		' this stream is for receiving INDArray messages from the network
		Protected Friend ReadOnly incomingFlow As New MessageFlow(Of INDArrayMessage)()

		' here we're storing reference to mesh
		Protected Friend ReadOnly mesh As New Atomic(Of MeshOrganizer)()

		' this is Id of this Transport instance
		Protected Friend id As String

		' id of the root node is used for initial communication
'JAVA TO VB CONVERTER NOTE: The field rootId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend rootId_Conflict As String

		Protected Friend masterMode As Boolean = False

		' this is simple storage for replies
		Protected Friend ReadOnly replies As IDictionary(Of String, ResponseMessage) = New ConcurrentDictionary(Of String, ResponseMessage)()

		' dedicated callback for restart messages
'JAVA TO VB CONVERTER NOTE: The field restartCallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend restartCallback_Conflict As RestartCallback

		' collection of callbacks for connection with ParameterServer implementation
		Protected Friend consumers As IDictionary(Of String, Consumer) = New Dictionary(Of String, Consumer)()

		' just configuration bean
		Protected Friend ReadOnly voidConfiguration As VoidConfiguration

		Protected Friend ReadOnly meshBuildMode As MeshBuildMode = MeshBuildMode.MESH

		' exactly what name says
		Protected Friend ReadOnly numerOfNodes As New AtomicInteger(0)

		' this queue handles all incoming messages
		Protected Friend ReadOnly messageQueue As TransferQueue(Of VoidMessage) = New LinkedTransferQueue(Of VoidMessage)()

		' MessageSplitter instance that'll be used in this transport
		Protected Friend splitter As MessageSplitter

		' we're keeping Ids of last 2k INDArrayMessages, just to avoid double spending/retransmission
		Protected Friend historyHolder As MessagesHistoryHolder(Of String) = New HashHistoryHolder(Of String)(2048)

		' this flag is used to track status of handshake procedure at node side
		Protected Friend handshakeFlag As New AtomicBoolean(False)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected final ThreadPoolExecutor executorService = (ThreadPoolExecutor) Executors.newFixedThreadPool(Math.max(2, Runtime.getRuntime().availableProcessors()), new ThreadFactory()
		Protected Friend ReadOnly executorService As ThreadPoolExecutor = CType(Executors.newFixedThreadPool(Math.Max(2, Runtime.getRuntime().availableProcessors()), New ThreadFactoryAnonymousInnerClass()), ThreadPoolExecutor)

		Private Class ThreadFactoryAnonymousInnerClass
			Inherits ThreadFactory

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Thread newThread(@NonNull final Runnable r)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
			Public Overrides Function newThread(ByVal r As ThreadStart) As Thread
				Dim t As val = New Thread(Sub()
				Nd4j.AffinityManager.unsafeSetDevice(0)
				r.run()
				End Sub)

				t.setDaemon(True)
				Return t
			End Function
		End Class



		Protected Friend Sub New()
			Me.New(System.Guid.randomUUID().ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseTransport(@NonNull String rootId)
		Protected Friend Sub New(ByVal rootId As String)
			Me.New(rootId, VoidConfiguration.builder().build())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseTransport(@NonNull String rootId, @NonNull VoidConfiguration voidConfiguration)
		Protected Friend Sub New(ByVal rootId As String, ByVal voidConfiguration As VoidConfiguration)
			Me.mesh.set(New MeshOrganizer(voidConfiguration.getMeshBuildMode()))
			Me.rootId_Conflict = rootId
			Me.voidConfiguration = voidConfiguration
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseTransport(@NonNull String ownId, @NonNull String rootId, @NonNull VoidConfiguration voidConfiguration)
		Protected Friend Sub New(ByVal ownId As String, ByVal rootId As String, ByVal voidConfiguration As VoidConfiguration)
			Me.mesh.set(New MeshOrganizer(voidConfiguration.getMeshBuildMode()))
			Me.id = ownId
			Me.rootId_Conflict = rootId
			Me.voidConfiguration = voidConfiguration

			masterMode = ownId.Equals(rootId, StringComparison.OrdinalIgnoreCase)
			If masterMode Then
				Me.mesh.get().getRootNode().setId(rootId)
			End If
		End Sub

		Public Overridable Function outgoingConsumer() As Consumer(Of VoidMessage) Implements Transport.outgoingConsumer
			Return outgoingFlow
		End Function

		Public Overridable Function incomingPublisher() As Publisher(Of INDArrayMessage) Implements Transport.incomingPublisher
			Return incomingFlow
		End Function

		Public Overridable ReadOnly Property UpstreamId As String Implements Transport.getUpstreamId
			Get
				If mesh.get().getRootNode().getId().Equals(Me.id()) Then
					Return Me.id()
				End If
    
				Return mesh.get().getNodeById(Me.id()).getUpstreamNode().getId()
			End Get
		End Property

		Public Overridable Sub launch() Implements Transport.launch
			SyncLock Me
				' master mode assumes heartbeat thread, so we'll need one more thread to run there
				Dim lim As Integer = If(masterMode, 1, 0)
				' we're launching threads for messages processing
				Dim e As Integer = 0
				Do While e< executorService.getMaximumPoolSize() - lim
					executorService.submit(Sub()
					Do
						Try
							Dim message As val = messageQueue.take()
							If message IsNot Nothing Then
								internalProcessMessage(message)
							End If
						Catch e As InterruptedException
							Exit Do
						Catch e As Exception
							log.error("Exception: {}", e)
						End Try
					Loop
					End Sub)
					e += 1
				Loop
        
        
				' this flow gets converted to VoidChunks and sent to upstream and downstreams
				Dim d As val = Flowable.fromPublisher(outgoingFlow).subscribe(Sub(voidMessage)
				If mesh.get() Is Nothing Then
					log.warn("Mesh wasn't received yet!")
					Return
				End If
				voidMessage.setOriginatorId(id)
				propagateMessage(voidMessage, PropagationMode.BOTH_WAYS)
				End Sub)
        
				' now we're going for Handshake with master
				If Not masterMode Then
					Try
						sendMessageBlocking(New HandshakeRequest(), rootId_Conflict)
					Catch e As Exception
						Throw New ND4JIllegalStateException("Can't proceed with handshake from [" & Me.id() & "] to [" & rootId_Conflict & "]", e)
					End Try
				End If
			End SyncLock
		End Sub

		Public Overridable Sub launchAsMaster() Implements Transport.launchAsMaster
			SyncLock Me
				If mesh.get() Is Nothing Then
					mesh.set(New MeshOrganizer(meshBuildMode))
				End If
        
				masterMode = True
				mesh.get().getRootNode().setId(Me.id())
        
				' launching heartbeat thread, that will monitor offline nodes
				executorService.submit(New HeartbeatThread(120000, Me, mesh))
        
				Me.launch()
			End SyncLock
		End Sub

		Public Overridable Sub shutdown() Implements Transport.shutdown
			SyncLock Me
				' shuttng down
				executorService.shutdown()
			End SyncLock
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected void propagateArrayMessage(INDArrayMessage message, org.nd4j.parameterserver.distributed.v2.enums.PropagationMode mode) throws java.io.IOException
		Protected Friend Overridable Sub propagateArrayMessage(ByVal message As INDArrayMessage, ByVal mode As PropagationMode)
			Dim node As val = mesh.get().getNodeById(id)

			Dim root As val = mesh.get().getRootNode()
			Dim upstream As val = node.getUpstreamNode()
			Dim downstreams As val = node.getDownstreamNodes()

			' TODO: make chunk size configurable
			Dim chunks As val = splitter.split(message, voidConfiguration.getMaxChunkSize())
			' send chunks to the upstream
			If Not node.isRootNode() AndAlso (PropagationMode.BOTH_WAYS = mode OrElse PropagationMode.ONLY_UP = mode) Then
				chunks.forEach(Sub(c) sendMessage(c, upstream.getId()))
			End If

			' and send chunks to all downstreams
			If PropagationMode.BOTH_WAYS = mode OrElse PropagationMode.ONLY_DOWN = mode Then
				downstreams.ForEach(Sub(n)
				chunks.forEach(Sub(c) sendMessage(c, n.getId()))
				End Sub)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void propagateMessage(@NonNull VoidMessage voidMessage, org.nd4j.parameterserver.distributed.v2.enums.PropagationMode mode) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub propagateMessage(ByVal voidMessage As VoidMessage, ByVal mode As PropagationMode)
			Dim node As val = mesh.get().getNodeById(id)

			'if (voidMessage.getOriginatorId() != null && id != null && voidMessage.getOriginatorId().equals(id))
			 '   return;

			' it's possible situation to have master as regular node. i.e. spark localhost mode
			If mesh.get().totalNodes() = 1 Then
				internalProcessMessage(voidMessage)
				Return
			End If

			Dim root As val = mesh.get().getRootNode()
			Dim upstream As val = node.getUpstreamNode()
			Dim downstreams As val = node.getDownstreamNodes()

			' setting on first one
			'if (voidMessage.getOriginatorId() == null)
				'voidMessage.setOriginatorId(this.id());

			If TypeOf voidMessage Is BroadcastableMessage Then
				DirectCast(voidMessage, BroadcastableMessage).RelayId = id
			End If

			' if this is INDArrayMessage we'll split it into chunks
			If TypeOf voidMessage Is INDArrayMessage Then
				propagateArrayMessage(DirectCast(voidMessage, INDArrayMessage), mode)
			Else
				' send message to the upstream
				If Not node.isRootNode() AndAlso (PropagationMode.BOTH_WAYS = mode OrElse PropagationMode.ONLY_UP = mode) Then
					sendMessage(voidMessage, upstream.getId())
				End If

				' and send message for all downstreams
				If PropagationMode.BOTH_WAYS = mode OrElse PropagationMode.ONLY_DOWN = mode Then
					downstreams.forEach(Sub(n) sendMessage(voidMessage, n.getId()))
				End If
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void propagateBroadcastableMessage(@NonNull BroadcastableMessage voidMessage, org.nd4j.parameterserver.distributed.v2.enums.PropagationMode mode)
		Protected Friend Overridable Sub propagateBroadcastableMessage(ByVal voidMessage As BroadcastableMessage, ByVal mode As PropagationMode)
			' we never broadcast MeshUpdates, master will send everyone copy anyway
			If TypeOf voidMessage Is MeshUpdateMessage Then
			   Return
			End If

			' if this message is already a known one - just skip it
			If historyHolder.storeIfUnknownMessageId(voidMessage.MessageId) Then
				Return
			End If

			Dim node As val = mesh.get().getNodeById(id)

			If voidMessage.OriginatorId IsNot Nothing AndAlso id IsNot Nothing AndAlso voidMessage.OriginatorId.Equals(id) Then
				Return
			End If

			Dim root As val = mesh.get().getRootNode()
			Dim upstream As val = node.getUpstreamNode()
			Dim downstreams As val = node.getDownstreamNodes()

			Dim ownId As val = id()
			Dim upstreamId As val = If(node.isRootNode(), Nothing, upstream.getId())
			Dim originatorId As val = voidMessage.OriginatorId
			Dim relayId As val = voidMessage.RelayId
			voidMessage.RelayId = id()

			' we never propagate upstream if we're on root node
			' we never send to the latest node
			' we never send to the original node
			If Not node.isRootNode() AndAlso (PropagationMode.BOTH_WAYS = mode OrElse PropagationMode.ONLY_UP = mode) AndAlso Not isLoopedNode(upstream, originatorId, relayId) Then
				If Not isLoopedNode(upstream, originatorId, relayId) Then
					sendMessage(voidMessage, upstreamId)
				End If
			End If

			' now we're sending message down
			If PropagationMode.BOTH_WAYS = mode OrElse PropagationMode.ONLY_DOWN = mode Then
				For Each n As val In downstreams
					If Not isLoopedNode(n, originatorId, relayId) Then
						sendMessage(voidMessage, n.getId())
					End If
				Next n
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected boolean isLoopedNode(@NonNull MeshOrganizer.Node node, @NonNull String originatorId, @NonNull String relayId)
		Protected Friend Overridable Function isLoopedNode(ByVal node As MeshOrganizer.Node, ByVal originatorId As String, ByVal relayId As String) As Boolean
			Return node.getId().Equals(originatorId) OrElse node.getId().Equals(relayId)
		End Function

		''' <summary>
		''' This method puts INDArray to the flow read by parameter server </summary>
		''' <param name="message"> </param>
		Private Sub forwardToParameterServer(ByVal message As INDArrayMessage)
			Try
				incomingFlow.accept(message)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		Protected Friend Overridable Sub internalProcessMessage(ByVal message As VoidMessage)
			Dim m As val = TypeOf message Is INDArrayMessage
			''' <summary>
			''' TODO: we need better isolation here
			''' </summary>
			If TypeOf message Is PingMessage Then

				Dim msg As val = New PongMessage()
				msg.setRequestId(DirectCast(message, PingMessage).RequestId)
				sendMessage(msg, message.OriginatorId)
				Return
			End If
			If TypeOf message Is PongMessage Then

				' do nothing
			ElseIf TypeOf message Is VoidChunk Then
				' we merge chunks to get full INDArrayMessage
				Dim opt As [Optional](Of INDArrayMessage) = splitter.merge(DirectCast(message, VoidChunk), voidConfiguration.getChunksBufferSize())

				' if this chunk was the last message, we'll forward it to parameter server for actual use
				If opt.Present Then
					Me.internalProcessMessage(opt.get())
				End If
			ElseIf TypeOf message Is INDArrayMessage Then
				' just forward message, but ONLY if it's not a Response message, since it's probably processed separately
				If Not (TypeOf message Is ResponseMessage) Then
					' we're not applying the same message twice
					If Not historyHolder.isKnownMessageId(message.MessageId) Then
						forwardToParameterServer(DirectCast(message, INDArrayMessage))
					End If
				Else
					' in this case we store message to the map, to be fetched later
					Dim reply As val = DirectCast(message, ResponseMessage)
					If Not replies.ContainsKey(reply.getRequestId()) : replies.Add(reply.getRequestId(), reply)
				End If
			ElseIf TypeOf message Is HandshakeRequest Then
				SyncLock mesh
					If Not mesh.get().isKnownNode(Me.id()) Then
						mesh.get().getRootNode().setId(Me.id)
					End If
				End SyncLock

				' our response
				Dim response As val = HandshakeResponse.builder().build()

				SyncLock mesh
					If mesh.get().isKnownNode(message.OriginatorId) Then
						log.warn("Got request from known node [{}]. Remapping.", message.OriginatorId)

						' notifying transport implementation about node reconnect
						onRemap(message.OriginatorId)

						mesh.get().remapNodeAndDownstreams(message.OriginatorId)
						' we say that this model has restarted
						response.setRestart(True)
					Else
						' first we add new node to the mesh
						mesh.get().addNode(message.OriginatorId)
						numerOfNodes.incrementAndGet()
					End If

					response.setMesh(mesh.get().clone())
				End SyncLock

				response.setRequestId(DirectCast(message, HandshakeRequest).RequestId)
				sendMessage(response, message.OriginatorId)

				' update all other nodes with new mesh
				' this message is called only from  spark driver context probably
				Try
					propagateMessageDirect(New MeshUpdateMessage(mesh.get()))
				Catch e As Exception
					log.error("Wasn't able to propagate message from [{}]", id())
					log.error("MeshUpdateMessage propagation failed:", e)
					Throw New Exception(e)
				End Try
			ElseIf TypeOf message Is HandshakeResponse Then
				Dim response As val = DirectCast(message, HandshakeResponse)
				Dim newMesh As val = response.getMesh()

				mesh.cas(Nothing, response.getMesh())

				SyncLock mesh
					Dim v1 As val = mesh.get().getVersion()
					Dim v2 As val = newMesh.getVersion()

					'log.info("Starting update A on [{}]; version: [{}/{}]; size: [{}]", this.id(), v1, v2, newMesh.totalNodes());
					' we update only if new mesh is older that existing one
					If v1 < v2 Then
						mesh.set(newMesh)
					End If
				End SyncLock

				' optionally calling out callback, which will happen approximately 100% of time
				If response.isRestart() Then
					log.info("Processing restart response...")
					If restartCallback_Conflict IsNot Nothing Then
						restartCallback_Conflict.call(response)
					Else
						log.warn("Got restart message from master, but there's no defined RestartCallback")
					End If
				End If

				' at last step we're updating handshake flag, so we're aware of finished handshake process
				handshakeFlag.set(True)

				' in any way we're putting this message back to replies
				Dim reply As val = DirectCast(message, ResponseMessage)
				If Not replies.ContainsKey(reply.getRequestId()) : replies.Add(reply.getRequestId(), reply)

				' this is default handler for message pairs
			ElseIf TypeOf message Is ResponseMessage Then
				' in this case we store message to the map, to be fetched later
				Dim reply As val = DirectCast(message, ResponseMessage)
				If Not replies.ContainsKey(reply.getRequestId()) : replies.Add(reply.getRequestId(), reply)

			ElseIf TypeOf message Is MeshUpdateMessage Then
				Dim newMesh As val = DirectCast(message, MeshUpdateMessage).getMesh()

				mesh.cas(Nothing, newMesh)

				SyncLock mesh
					Dim v1 As val = mesh.get().getVersion()
					Dim v2 As val = newMesh.getVersion()

					'log.info("Starting update B on [{}]; version: [{}/{}]; size: [{}]", this.id(), v1, v2, newMesh.totalNodes());
					' we update only if new mesh is older that existing one
					If v1 < v2 Then
						mesh.set(newMesh)
					End If
				End SyncLock

				' should be out of locked block
				onMeshUpdate(newMesh)
			Else
				If TypeOf message Is RequestMessage Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					Dim name As val = message.GetType().FullName
					Dim consumer As val = consumers(name)
					If consumer Is Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
						Throw New ND4JIllegalStateException("Not supported RequestMessage received: [" & message.GetType().FullName & "]")
					End If
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
					Throw New ND4JIllegalStateException("Unknown message received: [" & message.GetType().FullName & "]")
				End If
			End If


			If TypeOf message Is BroadcastableMessage Then
				' here we should propagate message down
				Try
					' we propagate message ONLY if we've already received Mesh from master
					If numerOfNodes.get() > 0 Then
						propagateBroadcastableMessage(DirectCast(message, BroadcastableMessage), PropagationMode.BOTH_WAYS)
					Else
						log.info("Skipping broadcast due to absence of nodes in mesh")
					End If
				Catch e As Exception
					log.error("Wasn't able to propagate message [{}] from [{}]", message.GetType().Name, message.OriginatorId)
					log.error("BroadcastableMessage propagation exception:", e)
					Throw New Exception(e)
				End Try
			End If

			' Request messages might be sent back to ParameterServer, which will take care of processing
			If TypeOf message Is RequestMessage Then
				' looks for callback for a given message type
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Dim consumer As val = consumers(message.GetType().FullName)
				If consumer IsNot Nothing Then
					Try
						consumer.accept(message)
					Catch e As Exception
						Throw New Exception(e)
					End Try
				End If
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void propagateMessageDirect(@NonNull BroadcastableMessage message)
		Public Overridable Sub propagateMessageDirect(ByVal message As BroadcastableMessage)
			SyncLock mesh
				Dim nodes As val = mesh.get().flatNodes()
				nodes.ForEach(Sub(n)
				If Not n.isRootNode() Then
					sendMessage(message, n.getId())
				End If
				End Sub)
			End SyncLock
		End Sub

		Public Overridable Sub processMessage(ByVal message As VoidMessage)
			Try
				messageQueue.transfer(message)
			Catch e As InterruptedException
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public String getRandomDownstreamFrom(@NonNull String id, String exclude)
		Public Overridable Function getRandomDownstreamFrom(ByVal id As String, ByVal exclude As String) As String Implements Transport.getRandomDownstreamFrom
			Dim nodes As val = mesh.get().getDownstreamsForNode(id)
			If nodes.isEmpty() Then
				Return Nothing
			End If

			' fetching ids of all the nodes
			Dim ids As val = New List(Of String)(nodes.Select(Function(node)
			Return node.getId()
			End Function).ToList())
			If exclude IsNot Nothing Then
				ids.remove(exclude)
			End If

			If ids.size() > 1 Then
				Collections.shuffle(ids)
			End If

			Return ids.get(0)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public <T extends ResponseMessage> T sendMessageBlocking(@NonNull RequestMessage message, @NonNull String id) throws InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function sendMessageBlocking(Of T As ResponseMessage)(ByVal message As RequestMessage, ByVal id As String) As T
			If message.RequestId Is Nothing Then
				message.RequestId = System.Guid.randomUUID().ToString()
			End If

			' we send message to the node first
			sendMessage(message, id)

			' and then we just block until we get response
			Dim r As ResponseMessage = Nothing
			r = replies(message.RequestId)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((r = replies.get(message.getRequestId())) == null)
			Do While r Is Nothing
				Thread.Sleep(10)
					r = replies(message.RequestId)
			Loop

			' remove response from holder
			replies.Remove(message.RequestId)

			'and return reply back
			Return DirectCast(r, T)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public <T extends ResponseMessage> T sendMessageBlocking(@NonNull RequestMessage message, @NonNull String id, long timeWait, @NonNull TimeUnit timeUnit) throws InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function sendMessageBlocking(Of T As ResponseMessage)(ByVal message As RequestMessage, ByVal id As String, ByVal timeWait As Long, ByVal timeUnit As TimeUnit) As T
			If message.RequestId Is Nothing Then
				message.RequestId = System.Guid.randomUUID().ToString()
			End If

			' we send message to the node first
			sendMessage(message, id)

			Dim sleepMs As val = TimeUnit.MILLISECONDS.convert(timeWait, timeUnit)
			Dim startTime As val = DateTimeHelper.CurrentUnixTimeMillis()

			' and then we just block until we get response
			Dim r As ResponseMessage = Nothing
			r = replies(message.RequestId)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((r = replies.get(message.getRequestId())) == null)
			Do While r Is Nothing
				Dim currTime As val = DateTimeHelper.CurrentUnixTimeMillis()

				If currTime - startTime > sleepMs Then
					Exit Do
				End If

				LockSupport.parkNanos(5000)
					r = replies(message.RequestId)
			Loop



			' remove response from holder
			replies.Remove(message.RequestId)

			'and return reply back
			Return DirectCast(r, T)
		End Function

		Public Overridable WriteOnly Property RestartCallback As RestartCallback
			Set(ByVal callback As RestartCallback)
				Me.restartCallback_Conflict = callback
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public <T extends RequestMessage> void addRequestConsumer(@NonNull @Class<T> cls, io.reactivex.functions.Consumer<T> consumer)
		Public Overridable Sub addRequestConsumer(Of T As RequestMessage)(ByVal cls As Type(Of T), ByVal consumer As Consumer(Of T)) Implements Transport.addRequestConsumer
			If consumer Is Nothing Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				consumers.Remove(cls.FullName)
			Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				consumers(cls.FullName) = consumer
			End If
		End Sub

		Public Overridable Sub onMeshUpdate(ByVal mesh As MeshOrganizer) Implements Transport.onMeshUpdate
			' FIXME: (int) is bad here
			numerOfNodes.set(CInt(mesh.totalNodes()))
		End Sub

		''' <summary>
		''' Generic Publisher/Consumer implementation for interconnect </summary>
		''' @param <T> </param>
		Public Class MessageFlow(Of T)
			Implements Consumer(Of T), Publisher(Of T)

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: private List<org.reactivestreams.Subscriber<? super T>> subscribers = new CopyOnWriteArrayList<>();
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
			Friend subscribers As IList(Of Subscriber(Of Object)) = New CopyOnWriteArrayList(Of Subscriber(Of Object))()

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(T voidMessage) throws Exception
			Public Overrides Sub accept(ByVal voidMessage As T)
				' just propagate messages further away
				subscribers.ForEach(Function(s) s.onNext(voidMessage))
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: @Override public void subscribe(org.reactivestreams.Subscriber<? super T> subscriber)
			Public Overrides Sub subscribe(Of T1)(ByVal subscriber As Subscriber(Of T1))
				' we're just maintaining list of
				subscribers.Add(subscriber)
			End Sub
		End Class


		Protected Friend Class HeartbeatThread
			Inherits Thread
			Implements ThreadStart

			Protected Friend ReadOnly delay As Long
			Protected Friend ReadOnly mesh As Atomic(Of MeshOrganizer)
			Protected Friend ReadOnly transport As Transport

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected HeartbeatThread(long delayMilliseconds, @NonNull Transport transport, @NonNull Atomic<org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer> mesh)
			Protected Friend Sub New(ByVal delayMilliseconds As Long, ByVal transport As Transport, ByVal mesh As Atomic(Of MeshOrganizer))
				Me.delay = delayMilliseconds
				Me.mesh = mesh
				Me.transport = transport
			End Sub

			Public Overrides Sub run()
				Try
					Do
						Thread.Sleep(delay)
						Dim remapped As val = New AtomicBoolean(False)

						Dim nodes As val = mesh.get().flatNodes()
						For Each n As val In nodes
							' we're skipping own node
							If transport.id().Equals(n.getId()) Then
								Continue For
							End If

							Dim m As PongMessage = transport.sendMessageBlocking(New PingMessage(), n.getId(), 100, TimeUnit.MILLISECONDS)

							' if we're not getting response in reasonable time - we're considering this node as failed
							If m Is Nothing Then
								mesh.get().remapNode(n)
								mesh.get().markNodeOffline(n)
								remapped.set(True)
							End If
						Next n

						If remapped.get() Then
							Try
								transport.propagateMessage(New MeshUpdateMessage(mesh.get()), PropagationMode.ONLY_DOWN)
							Catch e As IOException
								' hm.
							End Try
						End If
					Loop
				Catch e As InterruptedException
					'
				End Try
			End Sub
		End Class

		Public Overridable Sub onRemap(ByVal id As String) Implements Transport.onRemap
			'
		End Sub

		Public Overridable ReadOnly Property RootId As String Implements Transport.getRootId
			Get
				Return rootId_Conflict
			End Get
		End Property

		Public Overridable Function totalNumberOfNodes() As Integer Implements Transport.totalNumberOfNodes
			Return numerOfNodes.get()
		End Function

		Public Overridable ReadOnly Property Connected As Boolean Implements Transport.isConnected
			Get
				Return True
			End Get
		End Property

		Public Overridable ReadOnly Property Introduced As Boolean Implements Transport.isIntroduced
			Get
				If masterMode Then
					Return True
				End If
    
				Return handshakeFlag.get()
			End Get
		End Property

		Public Overridable Sub ensureConnection(ByVal id As String) Implements Transport.ensureConnection
			' no-op for local transports
		End Sub
	End Class

End Namespace