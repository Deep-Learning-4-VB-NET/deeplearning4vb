Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports IntMath = org.nd4j.shade.guava.math.IntMath
Imports Aeron = io.aeron.Aeron
Imports FragmentAssembler = io.aeron.FragmentAssembler
Imports Publication = io.aeron.Publication
Imports Subscription = io.aeron.Subscription
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports Header = io.aeron.logbuffer.Header
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DirectBuffer = org.agrona.DirectBuffer
Imports SleepingIdleStrategy = org.agrona.concurrent.SleepingIdleStrategy
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports HashUtil = org.nd4j.linalg.util.HashUtil
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports PropagationMode = org.nd4j.parameterserver.distributed.v2.enums.PropagationMode
Imports TransmissionStatus = org.nd4j.parameterserver.distributed.v2.enums.TransmissionStatus
Imports INDArrayMessage = org.nd4j.parameterserver.distributed.v2.messages.INDArrayMessage
Imports RequestMessage = org.nd4j.parameterserver.distributed.v2.messages.RequestMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage
Imports org.nd4j.parameterserver.distributed.v2.transport
Imports MeshOrganizer = org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer
Imports MessageSplitter = org.nd4j.parameterserver.distributed.v2.util.MessageSplitter
import static System.setProperty

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
'ORIGINAL LINE: @Slf4j public class AeronUdpTransport extends BaseTransport implements AutoCloseable
	Public Class AeronUdpTransport
		Inherits BaseTransport
		Implements AutoCloseable

		' this is for tests only
		Protected Friend interceptors As IDictionary(Of String, MessageCallable) = New Dictionary(Of String, MessageCallable)()
		Protected Friend precursors As IDictionary(Of String, MessageCallable) = New Dictionary(Of String, MessageCallable)()

		' this map holds outgoing connections, basically
		Protected Friend remoteConnections As IDictionary(Of String, RemoteConnection) = New ConcurrentDictionary(Of String, RemoteConnection)()

		Protected Friend ReadOnly SENDER_THREADS As Integer = 2
		Protected Friend ReadOnly MESSAGE_THREADS As Integer = 2
		Protected Friend ReadOnly SUBSCRIPTION_THREADS As Integer = 1

		Protected Friend aeron As Aeron
		Protected Friend context As Aeron.Context

		Protected Friend ownSubscription As Subscription
		Protected Friend messageHandler As FragmentAssembler
		Protected Friend subscriptionThread As Thread

		' TODO: move this to singleton holder
		Protected Friend driver As MediaDriver

		Private Shared ReadOnly DEFAULT_TERM_BUFFER_PROP As Long = IntMath.pow(2,25) '32MB

		' this is intermediate buffer for incoming messages
		Protected Friend Shadows messageQueue As BlockingQueue(Of VoidMessage) = New LinkedTransferQueue(Of VoidMessage)()

		' this is intermediate buffer for messages enqueued for propagation
		Protected Friend propagationQueue As BlockingQueue(Of INDArrayMessage) = New LinkedBlockingQueue(Of INDArrayMessage)(32)

		' this lock is used for aeron publications
		Protected Friend aeronLock As New ReentrantLock()

		Protected Friend ReadOnly shutdownFlag As New AtomicBoolean(False)

		Protected Friend ReadOnly connectedFlag As New AtomicBoolean(False)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AeronUdpTransport(@NonNull String ownIp, @NonNull String rootIp, @NonNull VoidConfiguration configuration)
		Public Sub New(ByVal ownIp As String, ByVal rootIp As String, ByVal configuration As VoidConfiguration)
			Me.New(ownIp, configuration.getPortSupplier().getPort(), rootIp, configuration.getUnicastControllerPort(), configuration)
		End Sub

		''' <summary>
		''' This constructor creates root transport instance </summary>
		''' <param name="rootIp"> </param>
		''' <param name="rootPort"> </param>
		''' <param name="configuration"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AeronUdpTransport(@NonNull String rootIp, int rootPort, @NonNull VoidConfiguration configuration)
		Public Sub New(ByVal rootIp As String, ByVal rootPort As Integer, ByVal configuration As VoidConfiguration)
			Me.New(rootIp, rootPort, rootIp, rootPort, configuration)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AeronUdpTransport(@NonNull String ownIp, int ownPort, @NonNull String rootIp, int rootPort, @NonNull VoidConfiguration configuration)
		Public Sub New(ByVal ownIp As String, ByVal ownPort As Integer, ByVal rootIp As String, ByVal rootPort As Integer, ByVal configuration As VoidConfiguration)
			MyBase.New("aeron:udp?endpoint=" & ownIp & ":" & ownPort, "aeron:udp?endpoint=" & rootIp & ":" & rootPort, configuration)

			Preconditions.checkArgument(ownPort > 0 AndAlso ownPort < 65536, "Own UDP port should be positive value in range of 1 and 65536")
			Preconditions.checkArgument(rootPort > 0 AndAlso rootPort < 65536, "Master node UDP port should be positive value in range of 1 and 65536")

			setProperty("aeron.client.liveness.timeout", "30000000000")

			' setting this property to try to increase maxmessage length, not sure if it still works though
			'Term buffer length: must be power of 2 and in range 64kB to 1GB: https://github.com/real-logic/aeron/wiki/Configuration-Options
			Dim p As String = System.getProperty(ND4JSystemProperties.AERON_TERM_BUFFER_PROP)
			If p Is Nothing Then
				System.setProperty(ND4JSystemProperties.AERON_TERM_BUFFER_PROP, DEFAULT_TERM_BUFFER_PROP.ToString())
			End If

			splitter = MessageSplitter.Instance

			context = (New Aeron.Context()).driverTimeoutMs(30000).keepAliveIntervalNs(100000000)
			AeronUtil.setDaemonizedThreadFactories(context)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final io.aeron.driver.MediaDriver.Context mediaDriverCtx = new io.aeron.driver.MediaDriver.Context();
			Dim mediaDriverCtx As New MediaDriver.Context()
			AeronUtil.setDaemonizedThreadFactories(mediaDriverCtx)

			driver = MediaDriver.launchEmbedded(mediaDriverCtx)
			context.aeronDirectoryName(driver.aeronDirectoryName())
			aeron = Aeron.connect(context)

			Runtime.getRuntime().addShutdownHook(New Thread(Sub()
			Me.shutdown()
			End Sub))
		End Sub

		' this executor service han
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected ExecutorService messagesExecutorService = Executors.newFixedThreadPool(SENDER_THREADS + MESSAGE_THREADS + SUBSCRIPTION_THREADS, new ThreadFactory()
		Protected Friend messagesExecutorService As ExecutorService = Executors.newFixedThreadPool(SENDER_THREADS + MESSAGE_THREADS + SUBSCRIPTION_THREADS, New ThreadFactoryAnonymousInnerClass())

		Private Class ThreadFactoryAnonymousInnerClass
			Inherits ThreadFactory

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Thread newThread(@NonNull final Runnable r)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
			Public Overrides Function newThread(ByVal r As ThreadStart) As Thread
				Dim t As val = New Thread(Sub()
				'TODO implement support for multi-GPU masters
				Nd4j.AffinityManager.unsafeSetDevice(0) 'Associate thread with device 0 (no-op for CPU)
				r.run()
				End Sub)

				t.setDaemon(True)
				t.setName("MessagesExecutorService thread")
				Return t
			End Function
		End Class


		Protected Friend Overridable Sub createSubscription()
			' create subscription
			ownSubscription = aeron.addSubscription(Me.id(), voidConfiguration.getStreamId())

			' create thread that polls messages from subscription
			messageHandler = New FragmentAssembler(Sub(buffer, offset, length, header) jointMessageHandler(buffer, offset, length, header))

			' starting thread(s) that will be fetching messages from network
			For e As Integer = 0 To SUBSCRIPTION_THREADS - 1
				messagesExecutorService.execute(Sub()
				Dim idler As val = New SleepingIdleStrategy(1000)
				Do
					idler.idle(ownSubscription.poll(messageHandler, 1024))
				Loop
				End Sub)
			Next e

			' starting thread(s) that will be actually executing message
			For e As Integer = 0 To MESSAGE_THREADS - 1
				messagesExecutorService.execute(Sub()
				Do
					Try
						' basically fetching messages from queue one by one, and processing them
						Dim msg As val = messageQueue.take()
						processMessage(msg)
					Catch e As InterruptedException
						' just terminate loop
						Exit Do
					End Try
				Loop
				End Sub)
			Next e


			For e As Integer = 0 To SENDER_THREADS - 1
				messagesExecutorService.execute(Sub()
				Do
					Try
						Dim msg As val = propagationQueue.take()
						redirectedPropagateArrayMessage(msg)
					Catch e As InterruptedException
						Exit Do
					Catch e As IOException
						log.error("Exception: {}", e)
						Throw New Exception(e)
					End Try
				Loop
				End Sub)
			Next e
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override protected void propagateArrayMessage(org.nd4j.parameterserver.distributed.v2.messages.INDArrayMessage message, org.nd4j.parameterserver.distributed.v2.enums.PropagationMode mode) throws java.io.IOException
		Protected Friend Overrides Sub propagateArrayMessage(ByVal message As INDArrayMessage, ByVal mode As PropagationMode)
			Try
				propagationQueue.put(message)
			Catch e As InterruptedException
				' just swallow this
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected void redirectedPropagateArrayMessage(org.nd4j.parameterserver.distributed.v2.messages.INDArrayMessage message) throws java.io.IOException
		Protected Friend Overridable Sub redirectedPropagateArrayMessage(ByVal message As INDArrayMessage)
			MyBase.propagateArrayMessage(message, PropagationMode.BOTH_WAYS)
		End Sub

		''' <summary>
		''' This method converts aeron buffer into VoidMessage and puts into temp queue for further processing
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="offset"> </param>
		''' <param name="length"> </param>
		''' <param name="header"> </param>
		Protected Friend Overridable Sub jointMessageHandler(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal length As Integer, ByVal header As Header)
			Dim data(length - 1) As SByte
			buffer.getBytes(offset, data)

			' deserialize message
			Dim message As val = VoidMessage.fromBytes(data)

			' we're checking if this is known connection or not, and add it if not
			If Not remoteConnections.ContainsKey(message.getOriginatorId()) Then
				addConnection(message.getOriginatorId())
			End If

			log.debug("Got [{}] message from [{}]", message.GetType().Name, message.getOriginatorId())

			' we're just putting deserialized message into the buffer
			Try
				messageQueue.put(message)
			Catch e As InterruptedException
				' :(
				Throw New Exception(e)
			End Try
		End Sub

		Public Overrides Sub onRemap(ByVal id As String)
			Try
				aeronLock.lock()

				log.info("Trying to disconnect failed node: [{}]", id)

				If remoteConnections.ContainsKey(id) Then
					Dim v As val = remoteConnections(id)
					Try
						v.getPublication().close()
					Catch e As Exception
						' no-op
					End Try

					remoteConnections.Remove(id)
				End If

				log.info("Trying to add failed node back again: [{}]", id)
				addConnection(id)
			Finally
				aeronLock.unlock()
			End Try
		End Sub

		Public Overrides Sub ensureConnection(ByVal id As String)
			' we just directly call addConnection
			addConnection(id)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void addConnection(@NonNull String ipAndPort)
		Protected Friend Overridable Sub addConnection(ByVal ipAndPort As String)
			Try
				aeronLock.lock()

				If remoteConnections.ContainsKey(ipAndPort) Then
					Return
				End If

				log.info("Adding UDP connection: [{}]", ipAndPort)

				Dim v As val = aeron.addPublication(ipAndPort, voidConfiguration.getStreamId())

				Dim cnt As Integer = 0
				Do While Not v.isConnected()
					Try
						Thread.Sleep(100)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if (cnt ++ > 100)
						If cnt > 100 Then
								cnt += 1
							Throw New ND4JIllegalStateException("Can't establish connection afet 10 seconds. Terminating...")
							Else
								cnt += 1
							End If
					Catch e As InterruptedException
						'
					End Try
				Loop

				Dim hash As val = HashUtil.getLongHash(ipAndPort)

				Dim rc As val = RemoteConnection.builder().ip(ipAndPort).port(0).longHash(hash).publication(v).build()

				remoteConnections(ipAndPort) = rc
			Finally
				aeronLock.unlock()
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()
			shutdown()
		End Sub

		Public Overrides Sub launch()
			SyncLock Me
				If Not masterMode Then
					' we set up aeron  connection to master first
					addConnection(Me.rootId_Conflict)
        
					' add own subscription
					createSubscription()
				End If
        
				MyBase.launch()
			End SyncLock
		End Sub

		Public Overrides Sub launchAsMaster()
			SyncLock Me
				' connection goes first, we're just creating subscription
				createSubscription()
        
				MyBase.launchAsMaster()
			End SyncLock
		End Sub

		Public Overrides Function id() As String
			Return id
		End Function

		Public Overrides ReadOnly Property Connected As Boolean
			Get
				If connectedFlag.get() OrElse masterMode Then
					Return True
				End If
    
				' node supposed to be connected if rootNode is connected and downstreams + upstream + downstreams are connected
				If Not remoteConnections.ContainsKey(rootId_Conflict) Then
					Return False
				End If
    
				SyncLock mesh
					Dim u As val = mesh.get().getUpstreamForNode(Me.id()).getId()
					If Not remoteConnections.ContainsKey(u) Then
						Return False
					End If
    
					For Each n As val In mesh.get().getDownstreamsForNode(Me.id())
						If Not remoteConnections.ContainsKey(n.getId()) Then
							Return False
						End If
					Next n
				End SyncLock
    
				connectedFlag.set(True)
				Return True
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void sendMessage(@NonNull VoidMessage message, @NonNull String id)
		Public Overridable Overloads Sub sendMessage(ByVal message As VoidMessage, ByVal id As String)
			If message.getOriginatorId() Is Nothing Then
				message.setOriginatorId(Me.id())
			End If

			' TODO: get rid of UUID!!!11
			If TypeOf message Is RequestMessage Then
				If DirectCast(message, RequestMessage).RequestId Is Nothing Then
					DirectCast(message, RequestMessage).RequestId = System.Guid.randomUUID().ToString()
				End If
			End If

			' let's not send messages to ourselves
			If message.getOriginatorId().Equals(id) Then
				Me.processMessage(message)
				Return
			End If

			If TypeOf message Is INDArrayMessage Then
				Try
					Dim splits As val = splitter.split(message, voidConfiguration.getMaxChunkSize())

					For Each m As val In splits
						sendMessage(m, id)
					Next m
				Catch e As IOException
					Throw New Exception(e)
				End Try

				Return
			End If

			' serialize out of locks
			Dim b As val = message.asUnsafeBuffer()

			' blocking until all connections are up
			If Not id.Equals(rootId_Conflict) Then
				Do While Not Connected
					LockSupport.parkNanos(10000000)
				Loop
			End If

			Dim conn As val = remoteConnections(id)
			If conn Is Nothing Then
				Throw New ND4JIllegalStateException("Unknown target ID specified: [" & id & "]")
			End If

			' serialize & send message right away
			Dim status As TransmissionStatus = TransmissionStatus.UNKNOWN
			Do While status <> TransmissionStatus.OK
				SyncLock conn.locker
					status = TransmissionStatus.fromLong(conn.getPublication().offer(b))
				End SyncLock

				' if response != OK we must do something with response
				Select Case status.innerEnumValue
					Case TransmissionStatus.InnerEnum.MAX_POSITION_EXCEEDED
							log.warn("MaxPosition hit: [{}]", id)
							Try
								' in case of backpressure we're just sleeping for a while, and message out again
								Thread.Sleep(voidConfiguration.getRetransmitTimeout())
							Catch e As InterruptedException
								'
							End Try
					Case TransmissionStatus.InnerEnum.CLOSED
						' TODO: here we should properly handle reconnection
						log.warn(" Connection was closed: [{}]", id)
						Return
					Case TransmissionStatus.InnerEnum.ADMIN_ACTION
							log.info("ADMIN_ACTION: [{}]", id)
							Try
								Thread.Sleep(voidConfiguration.getRetransmitTimeout())
							Catch e As InterruptedException
								'
							End Try
					Case TransmissionStatus.InnerEnum.NOT_CONNECTED
								log.info("NOT_CONNECTED: [{}]", id)
								addConnection(id)
								Try
									' in case of backpressure we're just sleeping for a while, and message out again
									Thread.Sleep(voidConfiguration.getRetransmitTimeout())
								Catch e As InterruptedException
									'
								End Try
					Case TransmissionStatus.InnerEnum.BACK_PRESSURED
						log.info("BACK_PRESSURED: [{}]", id)
						Try
							' in case of backpressure we're just sleeping for a while, and message out again
							Thread.Sleep(voidConfiguration.getRetransmitTimeout())
						Catch e As InterruptedException
							'
						End Try
				End Select
			Loop
		End Sub


		Protected Friend Overridable Sub shutdownSilent()
			' closing own connection
			ownSubscription.close()

			' and all known publications
			For Each rc As val In remoteConnections.Values
				rc.getPublication().close()
			Next rc

			' shutting down executor
			messagesExecutorService.shutdown()

			' closing aeron stuff
			aeron.close()
			context.close()
			driver.close()
		End Sub

		Public Overrides Sub shutdown()
			If shutdownFlag.compareAndSet(False, True) Then
				shutdownSilent()

				MyBase.shutdown()
			End If
		End Sub

		Public Overrides Sub onMeshUpdate(ByVal mesh As MeshOrganizer)
			mesh.flatNodes().forEach(Sub(n) addConnection(n.getId()))

			MyBase.onMeshUpdate(mesh)
		End Sub

		''' <summary>
		''' This method add interceptor for incoming messages. If interceptor is defined for given message class - runnable will be executed instead of processMessage() </summary>
		''' <param name="cls"> </param>
		''' <param name="callable"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.parameterserver.distributed.v2.messages.VoidMessage> void addInterceptor(@NonNull @Class<T> cls, @NonNull MessageCallable<T> callable)
		Public Overridable Sub addInterceptor(Of T As VoidMessage)(ByVal cls As Type(Of T), ByVal callable As MessageCallable(Of T))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			interceptors(cls.FullName) = callable
		End Sub

		''' <summary>
		''' This method add precursor for incoming messages. If precursor is defined for given message class - runnable will be executed before processMessage() </summary>
		''' <param name="cls"> </param>
		''' <param name="callable"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.parameterserver.distributed.v2.messages.VoidMessage> void addPrecursor(@NonNull @Class<T> cls, @NonNull MessageCallable<T> callable)
		Public Overridable Sub addPrecursor(Of T As VoidMessage)(ByVal cls As Type(Of T), ByVal callable As MessageCallable(Of T))
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			precursors(cls.FullName) = callable
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void processMessage(@NonNull VoidMessage message)
		Public Overridable Overloads Sub processMessage(ByVal message As VoidMessage)
			' fast super call if there's no callbacks where defined
			If interceptors.Count = 0 AndAlso precursors.Count = 0 Then
				MyBase.processMessage(message)
				Return
			End If

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			Dim name As val = message.GetType().FullName
			Dim callable As val = interceptors(name)

			If callable IsNot Nothing Then
				callable.apply(message)
			Else
				Dim precursor As val = precursors(name)
				If precursor IsNot Nothing Then
					precursor.apply(message)
				End If

				MyBase.processMessage(message)
			End If
		End Sub

		''' <summary>
		''' This method returns Mesh stored in this Transport instance
		''' PLEASE NOTE: This method is suited for tests
		''' @return
		''' </summary>
		Protected Friend Overridable ReadOnly Property Mesh As MeshOrganizer
			Get
				SyncLock mesh
					Return mesh.get()
				End SyncLock
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder public static class RemoteConnection
		Public Class RemoteConnection
			Friend ip As String
			Friend port As Integer
			Friend publication As Publication
			Friend ReadOnly locker As New Object()
			Friend ReadOnly activated As New AtomicBoolean(False)
			Protected Friend longHash As Long
		End Class

	End Class

End Namespace