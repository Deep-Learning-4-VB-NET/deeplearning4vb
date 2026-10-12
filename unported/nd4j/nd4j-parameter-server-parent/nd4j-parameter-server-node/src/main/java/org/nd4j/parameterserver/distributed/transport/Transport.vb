Imports System
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

	<Obsolete>
	Public Interface Transport
		Friend Enum ThreadingModel
			SAME_THREAD ' DO NOT USE IT IN REAL ENVIRONMENT!!!11oneoneeleven
			SINGLE_THREAD
			DEDICATED_THREADS
		End Enum

		Sub setIpAndPort(ByVal ip As String, ByVal port As Integer)

		ReadOnly Property Ip As String

		ReadOnly Property Port As Integer

		ReadOnly Property ShardIndex As Short


		ReadOnly Property TargetIndex As Short


		Sub addClient(ByVal ip As String, ByVal port As Integer)


		Sub addShard(ByVal ip As String, ByVal port As Integer)

		''' <summary>
		''' This method does initialization of Transport instance
		''' </summary>
		''' <param name="voidConfiguration"> </param>
		''' <param name="role"> </param>
		''' <param name="localIp"> </param>
		Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal clipboard As Clipboard, ByVal role As NodeRole, ByVal localIp As String, ByVal localPort As Integer, ByVal shardIndex As Short)


		''' <summary>
		''' This method accepts message for delivery, routing is applied according on message opType
		''' </summary>
		''' <param name="message"> </param>
		Sub sendMessage(ByVal message As VoidMessage)

		''' <summary>
		''' This method accepts message for delivery, and blocks until response delivered
		''' 
		''' @return
		''' </summary>
		Function sendMessageAndGetResponse(ByVal message As VoidMessage) As MeaningfulMessage

		''' 
		''' <param name="message"> </param>
		Sub sendMessageToAllShards(ByVal message As VoidMessage)

		''' 
		''' <param name="message"> </param>
		Sub sendMessageToAllClients(ByVal message As VoidMessage, ParamArray ByVal exclusions() As Long?)

		''' <summary>
		''' This method accepts message from network
		''' </summary>
		''' <param name="message"> </param>
		Sub receiveMessage(ByVal message As VoidMessage)

		''' <summary>
		''' This method takes 1 message from "incoming messages" queue, blocking if queue is empty
		''' 
		''' @return
		''' </summary>
		Function takeMessage() As VoidMessage

		''' <summary>
		''' This method puts message into processing queue
		''' </summary>
		''' <param name="message"> </param>
		Sub putMessage(ByVal message As VoidMessage)

		''' <summary>
		''' This method peeks 1 message from "incoming messages" queue, returning null if queue is empty
		''' 
		''' PLEASE NOTE: This method is suitable for debug purposes only
		''' 
		''' @return
		''' </summary>
		Function peekMessage() As VoidMessage

		''' <summary>
		''' This method starts transport mechanisms.
		''' 
		''' PLEASE NOTE: init() method should be called prior to launch() call
		''' </summary>
		Sub launch(ByVal threading As ThreadingModel)

		''' <summary>
		''' This method stops transport system.
		''' </summary>
		Sub shutdown()

		''' <summary>
		''' This method returns number of known Clients
		''' @return
		''' </summary>
		Function numberOfKnownClients() As Integer

		''' <summary>
		''' This method returns number of known Shards
		''' @return
		''' </summary>
		Function numberOfKnownShards() As Integer

		''' <summary>
		''' This method returns ID of this Transport instance
		''' @return
		''' </summary>
		ReadOnly Property OwnOriginatorId As Long
	End Interface

End Namespace