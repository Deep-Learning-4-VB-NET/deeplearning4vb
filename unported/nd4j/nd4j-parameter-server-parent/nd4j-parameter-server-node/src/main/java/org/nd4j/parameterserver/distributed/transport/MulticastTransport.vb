Imports System
Imports Aeron = io.aeron.Aeron
Imports FragmentAssembler = io.aeron.FragmentAssembler
Imports MediaDriver = io.aeron.driver.MediaDriver
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
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
'ORIGINAL LINE: @Slf4j @Deprecated public class MulticastTransport extends BaseTransport
	<Obsolete>
	Public Class MulticastTransport
		Inherits BaseTransport

		Protected Friend multicastChannelUri As String

		Public Sub New()
			' no-op
			log.info("Initializing MulticastTransport")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Clipboard clipboard, @NonNull NodeRole role, @NonNull String localIp, int localPort, short shardIndex)
		Public Overridable Overloads Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal clipboard As Clipboard, ByVal role As NodeRole, ByVal localIp As String, ByVal localPort As Integer, ByVal shardIndex As Short)
			If voidConfiguration.getTtl() < 1 Then
				Throw New ND4JIllegalStateException("For MulticastTransport you should have TTL >= 1, it won't work otherwise")
			End If

			If voidConfiguration.getMulticastNetwork() Is Nothing OrElse voidConfiguration.getMulticastNetwork().isEmpty() Then
				Throw New ND4JIllegalStateException("For MulticastTransport you should provide IP from multicast network available/allowed in your environment, i.e.: 224.0.1.1")
			End If

			'shutdown hook
			MyBase.init(voidConfiguration, clipboard, role, localIp, localPort, shardIndex)

			Me.voidConfiguration = voidConfiguration
			Me.nodeRole = role
			Me.clipboard = clipboard

			context = New Aeron.Context()

			driver = MediaDriver.launchEmbedded()

			context.aeronDirectoryName(driver.aeronDirectoryName())

			aeron = Aeron.connect(context)



			Me.shardIndex = shardIndex



			multicastChannelUri = "aeron:udp?endpoint=" & voidConfiguration.getMulticastNetwork() & ":" & voidConfiguration.getMulticastPort()
			If voidConfiguration.getMulticastInterface() IsNot Nothing AndAlso Not voidConfiguration.getMulticastInterface().isEmpty() Then
				multicastChannelUri = multicastChannelUri & "|interface=" & voidConfiguration.getMulticastInterface()
			End If

			multicastChannelUri = multicastChannelUri & "|ttl=" & voidConfiguration.getTtl()

			If voidConfiguration.getNumberOfShards() < 0 Then
				voidConfiguration.setNumberOfShards(voidConfiguration.getShardAddresses().size())
			End If

			Select Case nodeRole
				Case NodeRole.BACKUP, SHARD
	'                
	'                    In case of Shard, unicast address for communication is known in advance
	'                 
					If ip_Conflict Is Nothing Then
						ip_Conflict = localIp
						port_Conflict = voidConfiguration.getUnicastControllerPort()
					End If


					unicastChannelUri = "aeron:udp?endpoint=" & ip_Conflict & ":" & port_Conflict
					log.info("Shard unicast URI: {}/{}", unicastChannelUri, voidConfiguration.getStreamId())

					' this channel will be used to receive batches from Clients
					subscriptionForShards = aeron.addSubscription(unicastChannelUri, voidConfiguration.getStreamId())

					' this channel will be used to send completion reports back to Clients
					publicationForClients = aeron.addPublication(multicastChannelUri, voidConfiguration.getStreamId() + 1)

					' this channel will be used for communication with other Shards
					publicationForShards = aeron.addPublication(multicastChannelUri, voidConfiguration.getStreamId() + 2)

					' this channel will be used to receive messages from other Shards
					subscriptionForClients = aeron.addSubscription(multicastChannelUri, voidConfiguration.getStreamId() + 2)

					messageHandlerForShards = New FragmentAssembler(Sub(buffer, offset, length, header) shardMessageHandler(buffer, offset, length, header))

					messageHandlerForClients = New FragmentAssembler((Sub(buffer, offset, length, header) internalMessageHandler(buffer, offset, length, header)))



				Case NodeRole.CLIENT
					ip_Conflict = localIp

	'                
	'                    In case of Client, unicast will be one of shards, picked up with random
	'                 
					' FIXME: we don't want that

					Dim rts As String = voidConfiguration.getShardAddresses().get(0) 'ArrayUtil.getRandomElement(configuration.getShardAddresses());
					Dim split() As String = rts.Split(":", True)
					If split.Length = 1 Then
						ip_Conflict = rts
						port_Conflict = voidConfiguration.getUnicastControllerPort()
					Else
						ip_Conflict = split(0)
						port_Conflict = Convert.ToInt32(split(1))
					End If


					unicastChannelUri = "aeron:udp?endpoint=" & ip_Conflict & ":" & port_Conflict
					'unicastChannelUri = "aeron:udp?endpoint=" + ip  + ":" + (configuration.getUnicastPort()) ;

					log.info("Client unicast URI: {}/{}", unicastChannelUri, voidConfiguration.getStreamId())

	'                
	'                 this channel will be used to send batches to Shards, it's 1:1 channel to one of the Shards
	'                
					publicationForShards = aeron.addPublication(unicastChannelUri, voidConfiguration.getStreamId())

					' this channel will be used to receive completion reports from Shards
					subscriptionForClients = aeron.addSubscription(multicastChannelUri, voidConfiguration.getStreamId() + 1)

					messageHandlerForClients = New FragmentAssembler(Sub(buffer, offset, length, header) clientMessageHandler(buffer, offset, length, header))
				Case Else
					log.warn("Unknown role passed: {}", nodeRole)
					Throw New Exception()
			End Select



			' if that's local spark run - we don't need this
			If voidConfiguration.getNumberOfShards() = 1 AndAlso nodeRole = NodeRole.SHARD Then
				shutdownSilent()
			End If
		End Sub

		''' <summary>
		''' This command is possible to issue only from Shard
		''' </summary>
		''' <param name="message"> </param>
		Protected Friend Overrides Sub sendCoordinationCommand(ByVal message As VoidMessage)
			SyncLock Me
				If nodeRole = NodeRole.SHARD AndAlso voidConfiguration.getNumberOfShards() = 1 Then
					message.TargetId = (Short) -1
					messages.add(message)
					Return
				End If
        
				'log.info("Sending CC: {}", message.getClass().getCanonicalName());
        
				message.TargetId = (Short) -1
				publicationForShards.offer(message.asUnsafeBuffer())
			End SyncLock
		End Sub

		''' <summary>
		''' This command is possible to issue only from Shard
		''' </summary>
		''' <param name="message"> </param>
		Protected Friend Overrides Sub sendFeedbackToClient(ByVal message As VoidMessage)
			SyncLock Me
				If nodeRole = NodeRole.SHARD AndAlso voidConfiguration.getNumberOfShards() = 1 AndAlso TypeOf message Is MeaningfulMessage Then
					message.TargetId = (Short) -1
					completed(message.TaskId) = DirectCast(message, MeaningfulMessage)
					Return
				End If
        
				'log.info("Sending FC: {}", message.getClass().getCanonicalName());
        
				message.TargetId = (Short) -1
				publicationForClients.offer(message.asUnsafeBuffer())
			End SyncLock
		End Sub
	End Class

End Namespace