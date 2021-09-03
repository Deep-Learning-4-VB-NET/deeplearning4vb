Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports IntMath = org.nd4j.shade.guava.math.IntMath
Imports Aeron = io.aeron.Aeron
Imports FragmentAssembler = io.aeron.FragmentAssembler
Imports Publication = io.aeron.Publication
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports Header = io.aeron.logbuffer.Header
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports CloseHelper = org.agrona.CloseHelper
Imports DirectBuffer = org.agrona.DirectBuffer
Imports AeronUtil = org.nd4j.aeron.ipc.AeronUtil
Imports ND4JSystemProperties = org.nd4j.common.config.ND4JSystemProperties
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports HashUtil = org.nd4j.linalg.util.HashUtil
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports ClientRouter = org.nd4j.parameterserver.distributed.logic.ClientRouter
Imports RetransmissionHandler = org.nd4j.parameterserver.distributed.logic.RetransmissionHandler
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports InterleavedRouter = org.nd4j.parameterserver.distributed.logic.routing.InterleavedRouter
Imports org.nd4j.parameterserver.distributed.messages
Imports IntroductionRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.IntroductionRequestMessage
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

Namespace org.nd4j.parameterserver.distributed.transport


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class RoutedTransport extends BaseTransport
	<Obsolete>
	Public Class RoutedTransport
		Inherits BaseTransport

		Private Shared ReadOnly DEFAULT_TERM_BUFFER_PROP As Long = IntMath.pow(2,25) '32MB

		Protected Friend shards As IList(Of RemoteConnection) = New List(Of RemoteConnection)()
		Protected Friend clients As IDictionary(Of Long, RemoteConnection) = New ConcurrentDictionary(Of Long, RemoteConnection)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected org.nd4j.parameterserver.distributed.logic.ClientRouter router;
		Protected Friend router As ClientRouter

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Clipboard clipboard, @NonNull NodeRole role, @NonNull String localIp, int localPort, short shardIndex)
		Public Overridable Overloads Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal clipboard As Clipboard, ByVal role As NodeRole, ByVal localIp As String, ByVal localPort As Integer, ByVal shardIndex As Short)
			Me.nodeRole = role
			Me.clipboard = clipboard
			Me.voidConfiguration = voidConfiguration
			Me.shardIndex = shardIndex
			Me.messages = New LinkedBlockingQueue(Of org.nd4j.parameterserver.distributed.messages.VoidMessage)()
			'shutdown hook
			MyBase.init(voidConfiguration, clipboard, role, localIp, localPort, shardIndex)
			setProperty("aeron.client.liveness.timeout", "30000000000")

			' setting this property to try to increase maxmessage length, not sure if it still works though
			'Term buffer length: must be power of 2 and in range 64kB to 1GB: https://github.com/real-logic/aeron/wiki/Configuration-Options
			Dim p As String = System.getProperty(ND4JSystemProperties.AERON_TERM_BUFFER_PROP)
			If p Is Nothing Then
				System.setProperty(ND4JSystemProperties.AERON_TERM_BUFFER_PROP, DEFAULT_TERM_BUFFER_PROP.ToString())
			End If


			context = (New Aeron.Context()).driverTimeoutMs(30000).keepAliveIntervalNs(100000000)
			AeronUtil.setDaemonizedThreadFactories(context)

			Dim ctx As New MediaDriver.Context()
			AeronUtil.setDaemonizedThreadFactories(ctx)
			driver = MediaDriver.launchEmbedded(ctx)
			context.aeronDirectoryName(driver.aeronDirectoryName())
			aeron = Aeron.connect(context)



			If router Is Nothing Then
				router = New InterleavedRouter()
			End If


	'        
	'            Regardless of current role, we raise subscription for incoming messages channel
	'         
			' we skip IPs assign process if they were defined externally
			If port_Conflict = 0 Then
				ip_Conflict = localIp
				port_Conflict = localPort
			End If
			unicastChannelUri = "aeron:udp?endpoint=" & ip_Conflict & ":" & port_Conflict
			subscriptionForClients = aeron.addSubscription(unicastChannelUri, voidConfiguration.getStreamId())
			'clean shut down
			Runtime.getRuntime().addShutdownHook(New Thread(Sub()
			CloseHelper.quietClose(aeron)
			CloseHelper.quietClose(driver)
			CloseHelper.quietClose(subscriptionForClients)
			End Sub))


			messageHandlerForClients = New FragmentAssembler(Function(buffer, offset, length, header) jointMessageHandler(buffer, offset, length, header))

	'        
	'            Now, regardless of current role,
	'             we set up publication channel to each shard
	'         
			Dim shardChannelUri As String = Nothing
			Dim remoteIp As String = Nothing
			Dim remotePort As Integer = 0
			For Each ip As String In voidConfiguration.getShardAddresses()
				If ip.Contains(":") Then
					shardChannelUri = "aeron:udp?endpoint=" & ip
					Dim split() As String = ip.Split(":", True)
					remoteIp = split(0)
					remotePort = Convert.ToInt32(split(1))
				Else
					shardChannelUri = "aeron:udp?endpoint=" & ip & ":" & voidConfiguration.getUnicastControllerPort()
					remoteIp = ip
					remotePort = voidConfiguration.getUnicastControllerPort()
				End If

				Dim publication As Publication = aeron.addPublication(shardChannelUri, voidConfiguration.getStreamId())

				Dim connection As RemoteConnection = RemoteConnection.builder().ip(remoteIp).port(remotePort).publication(publication).locker(New Object()).build()

				shards.Add(connection)
			Next ip

			If nodeRole = NodeRole.SHARD Then
				log.info("Initialized as [{}]; ShardIndex: [{}]; Own endpoint: [{}]", nodeRole, shardIndex, unicastChannelUri)
			Else
				log.info("Initialized as [{}]; Own endpoint: [{}]", nodeRole, unicastChannelUri)
			End If

			Select Case nodeRole
				Case NodeRole.MASTER, BACKUP

'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case NodeRole.SHARD
	'                
	'                    For unicast transport we want to have interconnects between all shards first of all, because we know their IPs in advance.
	'                    But due to design requirements, clients have the same first step, so it's kinda shared for all states :)
	'                 

	'                
	'                    Next step is connections setup for backup nodes.
	'                    TODO: to be implemented
	'                 
					addClient(ip_Conflict, port_Conflict)
				Case NodeRole.CLIENT
	'                
	'                    For Clients on unicast transport, we either set up connection to single Shard, or to multiple shards
	'                    But since this code is shared - we don't do anything here
	'                 
				Case Else
					Throw New ND4JIllegalStateException("Unknown NodeRole being passed: " & nodeRole)
			End Select

			router.init(voidConfiguration, Me)
			Me.originatorId = HashUtil.getLongHash(Me.Ip & ":" & Me.Port)
		End Sub


		Public Overrides Sub sendMessageToAllClients(ByVal message As VoidMessage, ParamArray ByVal exclusions() As Long?)
			If nodeRole <> NodeRole.SHARD Then
				Throw New ND4JIllegalStateException("Only SHARD allowed to send messages to all Clients")
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.agrona.DirectBuffer buffer = message.asUnsafeBuffer();
			Dim buffer As DirectBuffer = message.asUnsafeBuffer()

			' no need to search for matches above number of then exclusions
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger cnt = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim cnt As New AtomicInteger(0)

			'final StringBuilder builder = new StringBuilder("Got message from: [").append(message.getOriginatorId()).append("]; Resend: {");

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: clients.values().parallelStream().filter(rc ->
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
			clients.Values.Where(Function(rc)
			If rc.getLongHash() = Me.originatorId OrElse rc.getLongHash() = 0 Then
				Return False
			End If
			If exclusions IsNot Nothing AndAlso cnt.get() < exclusions.Length Then
				For Each exclude As Long? In exclusions
					If exclude.Value = rc.getLongHash() Then
					cnt.incrementAndGet()
					Return False
					End If
				Next exclude
			End If
			Return True
			End Function).ForEach(Function(rc) { RetransmissionHandler.TransmissionStatus res; Long retr = 0; Boolean delivered = False; while (Not delivered) { synchronized(rc.locker) { res = RetransmissionHandler.getTransmissionStatus(rc.getPublication().offer(buffer)); } switch(res){case NOT_CONNECTED:{if (Not rc.getActivated().get()){retr
				retr += 1

								If retr > 20 Then
									Throw New ND4JIllegalStateException("Can't connect to Shard: [" & rc.getPublication().channel() & "]")
								End If

								Try
									'Thread.sleep(voidConfiguration.getRetransmitTimeout());
									LockSupport.parkNanos(voidConfiguration.getRetransmitTimeout() * 1000000)
								Catch e As Exception
									Throw New Exception(e)
								End Try
		End Sub
							Else
								Throw New ND4JIllegalStateException("Shards reassignment is to be implemented yet")
							End If
	End Class
							break
						Case ADMIN_ACTION, BACKPRESSURE
						If True Then
							Try
								'Thread.sleep(voidConfiguration.getRetransmitTimeout());
								LockSupport.parkNanos(voidConfiguration.getRetransmitTimeout() * 1000000)
							Catch e As Exception
								Throw New Exception(e)
							End Try
						End If
							break
						Case MESSAGE_SENT
							delivered = True
							rc.getActivated().set(True)
							break
					}
				}
			}
			)

			's   log.info("RESULT: {}", builder.toString());
		}

		''' <summary>
		''' This method implements Shard -> Shards comms
		''' </summary>
		''' <param name="message"> </param>
		protected void sendCoordinationCommand(VoidMessage message)
		If True Then

			'        log.info("Sending [{}] to all Shards...", message.getClass().getSimpleName());
			message.setOriginatorId(Me.originatorId)

			' if we're the only shard - we just put message into the queue
			If nodeRole = NodeRole.SHARD AndAlso voidConfiguration.getNumberOfShards() = 1 Then
				Try
					messages.put(message)
					Return
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.agrona.DirectBuffer buffer = message.asUnsafeBuffer();
			Dim buffer As DirectBuffer = message.asUnsafeBuffer()

			' TODO: check which approach is faster, lambda, direct roll through list, or queue approach
'JAVA TO VB CONVERTER TODO TASK: The following 'switch expression' was not converted by Java to VB Converter:
'			shards.parallelStream().forEach((rc) -> { org.nd4j.parameterserver.distributed.logic.RetransmissionHandler.TransmissionStatus res; long retr = 0; boolean delivered = False; long address = org.nd4j.linalg.util.HashUtil.getLongHash(rc.getIp() + ":" + rc.getPort()); if(originatorId == address) { try { messages.put(message); } catch(Exception e) { throw New RuntimeException(e); } Return; } while (!delivered) { synchronized(rc.locker) { res = org.nd4j.parameterserver.distributed.logic.RetransmissionHandler.getTransmissionStatus(rc.getPublication().offer(buffer)); } switch(res)
	'		{
	'					case NOT_CONNECTED:
	'					{
	'						if (!rc.getActivated().get())
	'						{
	'							retr++;
	'
	'							if (retr > 20)
	'								throw New ND4JIllegalStateException("Can't connect to Shard: [" + rc.getPublication().channel() + "]");
	'
	'							try
	'							{
	'								Thread.sleep(voidConfiguration.getRetransmitTimeout());
	'							}
	'							catch (Exception e)
	'							{
	'							}
	'						}
	'						else
	'						{
	'							throw New ND4JIllegalStateException("Shards reassignment is to be implemented yet");
	'						}
	'					}
	'						break;
	'					case ADMIN_ACTION:
	'					case BACKPRESSURE:
	'					{
	'						try
	'						{
	'							Thread.sleep(voidConfiguration.getRetransmitTimeout());
	'						}
	'						catch (Exception e)
	'						{
	'						}
	'					}
	'						break;
	'					case MESSAGE_SENT:
	'						delivered = True;
	'						rc.getActivated().set(True);
	'						break;
	'				}

					If Not delivered Then
						log.info("Attempting to resend message")
					End If
		End If
			}
			)
		}

		''' <summary>
		''' This method implements Shard -> Client comms
		''' </summary>
		''' <param name="message"> </param>
		protected void sendFeedbackToClient(VoidMessage message)
		If True Then
	'        
	'            PLEASE NOTE: In this case we don't change target. We just discard message if something goes wrong.
	'         
			' TODO: discard message if it's not sent for enough time?
			Dim targetAddress As Long = message.getOriginatorId()

			If targetAddress = originatorId Then
				completed.put(message.getTaskId(), DirectCast(message, MeaningfulMessage))
				Return
			End If

			Dim result As RetransmissionHandler.TransmissionStatus

			'log.info("sI_{} trying to send back {}/{}", shardIndex, targetAddress, message.getClass().getSimpleName());

			Dim connection As RemoteConnection = clients.get(targetAddress)
			Dim delivered As Boolean = False

			If connection Is Nothing Then
				log.info("Can't get client with address [{}]", targetAddress)
				log.info("Known clients: {}", clients.keySet())
				Throw New Exception()
			End If

			Do While Not delivered
				SyncLock connection.locker
					result = RetransmissionHandler.getTransmissionStatus(connection.getPublication().offer(message.asUnsafeBuffer()))
				End SyncLock

				Select Case result
					Case RetransmissionHandler.TransmissionStatus.ADMIN_ACTION, BACKPRESSURE
						Try
							Thread.Sleep(voidConfiguration.getRetransmitTimeout())
						Catch e As Exception
						End Try
					Case RetransmissionHandler.TransmissionStatus.NOT_CONNECTED
						' client dead? sleep and forget
						' TODO: we might want to delay this message & move it to separate queue?
						Try
							Thread.Sleep(voidConfiguration.getRetransmitTimeout())
						Catch e As Exception
						End Try
					' do not break here, we can't do too much here, if client is dead
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case RetransmissionHandler.TransmissionStatus.MESSAGE_SENT
						delivered = True
				End Select
			Loop
		End If

		public Integer numberOfKnownClients()
		If True Then
			Return clients.size()
		End If

		public Integer numberOfKnownShards()
		If True Then
			Return shards.size()
		End If

		protected void shutdownSilent()
		If True Then
			' closing shards
			shards.forEach(Sub(rc)
			rc.getPublication().close()
			End Sub)

			' closing clients connections
			clients.values().forEach(Sub(rc)
			rc.getPublication().close()
			End Sub)

			subscriptionForClients.close()

			aeron.close()
			context.close()
			driver.close()
		End If

		public void shutdown()
		If True Then
			runner.set(False)

			If threadB IsNot Nothing Then
				threadB.interrupt()
			End If

			If threadA IsNot Nothing Then
				threadA.interrupt()
			End If

			shutdownSilent()
		End If


		protected void sendCommandToShard(VoidMessage message)
		If True Then
			' fastpath for local Shard
			If nodeRole = NodeRole.SHARD AndAlso TypeOf message Is TrainingMessage Then
				router.setOriginator(message)
				message.setTargetId(getShardIndex())

				Try
					messages.put(message)
				Catch e As Exception
					Throw New Exception(e)
				End Try
				Return
			End If

			'log.info("sI_{} {}: message class: {}", shardIndex, nodeRole, message.getClass().getSimpleName());

			Dim result As RetransmissionHandler.TransmissionStatus

			Dim targetShard As Integer = router.assignTarget(message)

			'log.info("Sending message {} to shard {}", message.getClass().getSimpleName(), targetShard);
			Dim delivered As Boolean = False
			Dim connection As RemoteConnection = shards.get(targetShard)

			Do While Not delivered
				SyncLock connection.locker
					result = RetransmissionHandler.getTransmissionStatus(connection.getPublication().offer(message.asUnsafeBuffer()))
				End SyncLock

				Select Case result
					Case RetransmissionHandler.TransmissionStatus.BACKPRESSURE, ADMIN_ACTION
						' we just sleep, and retransmit again later
						Try
							Thread.Sleep(voidConfiguration.getRetransmitTimeout())
						Catch e As Exception
						End Try
					Case RetransmissionHandler.TransmissionStatus.NOT_CONNECTED
	'                    
	'                        two possible cases here:
	'                        1) We hadn't sent any messages to this Shard before
	'                        2) It was active before, and suddenly died
	'                     
						If Not connection.getActivated().get() Then
							' wasn't initialized before, just sleep and re-transmit
							Try
								Thread.Sleep(voidConfiguration.getRetransmitTimeout())
							Catch e As Exception
							End Try
						Else
							Throw New ND4JIllegalStateException("Shards reassignment is to be implemented yet")
						End If
					Case RetransmissionHandler.TransmissionStatus.MESSAGE_SENT
						delivered = True
						connection.getActivated().set(True)
				End Select
			Loop
		End If

		''' <summary>
		''' This message handler is responsible for receiving messages on any side of p2p network
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="offset"> </param>
		''' <param name="length"> </param>
		''' <param name="header"> </param>
		protected void jointMessageHandler(DirectBuffer buffer, Integer offset, Integer length, Header header)
		If True Then
			''' <summary>
			'''  All incoming messages here are supposed to be "just messages", only unicast communication
			'''  All of them should implement MeaningfulMessage interface
			''' </summary>

			Dim data(length - 1) As SByte
			buffer.getBytes(offset, data)

			Dim message As VoidMessage = VoidMessage.fromBytes(data)

			'        log.info("sI_{} received message: {}", shardIndex, message.getClass().getSimpleName());

			'if (messages.size() > 500)
			'    log.info("sI_{} got {} messages", shardIndex, messages.size());

			If TypeOf message Is MeaningfulMessage Then
				Dim msg As MeaningfulMessage = DirectCast(message, MeaningfulMessage)
				completed.put(message.TaskId, msg)
			ElseIf TypeOf message Is RequestMessage Then
				Try
					messages.put(DirectCast(message, RequestMessage))
				Catch e As InterruptedException
					' do nothing
				Catch e As Exception
					Throw New Exception(e)
				End Try
			ElseIf TypeOf message Is DistributedMessage Then
				Try
					messages.put(DirectCast(message, DistributedMessage))
				Catch e As InterruptedException
					' do nothing
				Catch e As Exception
					Throw New Exception(e)
				End Try
			ElseIf TypeOf message Is TrainingMessage Then
				Try
					messages.put(DirectCast(message, TrainingMessage))
				Catch e As InterruptedException
					' do nothing
				Catch e As Exception
					Throw New Exception(e)
				End Try
			ElseIf TypeOf message Is VoidAggregation Then
				Try
					messages.put(DirectCast(message, VoidAggregation))
				Catch e As InterruptedException
					' do nothing
				Catch e As Exception
					Throw New Exception(e)
				End Try
			ElseIf TypeOf message Is Frame Then
				Try
					messages.put(DirectCast(message, Frame))
				Catch e As InterruptedException
					' do nothing
				Catch e As Exception
					Throw New Exception(e)
				End Try
			Else
				log.info("Unknown message: {}", message.GetType().Name)
			End If
		End If

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void launch(@NonNull ThreadingModel threading)
		public void launch( ThreadingModel threading)
		If True Then
			MyBase.launch(threading)

			' send introductory message
			'        if (nodeRole == NodeRole.CLIENT) {
			'            shards.parallelStream().forEach((rc) -> {
			Dim irm As New IntroductionRequestMessage(getIp(), getPort())
			irm.TargetId = (Short) -1
			sendCoordinationCommand(irm)
			'            });
			'        }
		End If


		public synchronized void addShard(String ip, Integer port)
		If True Then
			Dim hash As Long? = HashUtil.getLongHash(ip & ":" & port)

			Dim connection As RemoteConnection = RemoteConnection.builder().ip(ip).port(port).publication(aeron.addPublication("aeron:udp?endpoint=" & ip & ":" & port, voidConfiguration.getStreamId())).longHash(hash).locker(New Object()).activated(New AtomicBoolean(False)).build()

			log.info("sI_{} {}: Adding SHARD: [{}] to {}:{}", shardIndex, nodeRole, hash, ip, port)
			shards.add(connection)
		End If

		public synchronized void addClient(String ip, Integer port)
		If True Then
			Dim hash As Long? = HashUtil.getLongHash(ip & ":" & port)
			If clients.containsKey(hash) Then
				Return
			End If

			Dim connection As RemoteConnection = RemoteConnection.builder().ip(ip).port(port).publication(aeron.addPublication("aeron:udp?endpoint=" & ip & ":" & port, voidConfiguration.getStreamId())).longHash(hash).locker(New Object()).activated(New AtomicBoolean(False)).build()


			log.info("sI_{} {}: Adding connection: [{}] to {}:{}", shardIndex, nodeRole, hash, ip, port)
			Me.clients.put(hash, connection)
			log.info("sI_{} {}: Known clients: {}", shardIndex, nodeRole, clients.keySet())
		End If


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder public static class RemoteConnection
		public static class RemoteConnection
		If True Then
			private String ip
			private Integer port
			private Publication publication
			private Object locker
			private AtomicBoolean activated
			protected Long longHash



			public static class RemoteConnectionBuilder
			If True Then
				private Object locker = New Object()
				private AtomicBoolean activated = New AtomicBoolean()
			End If
		End If

	}

End Namespace