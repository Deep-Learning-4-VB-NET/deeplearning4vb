Imports System
Imports NonNull = lombok.NonNull
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
	Public Class LocalTransport
		Implements Transport

		Public Overridable Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal clipboard As Clipboard, ByVal role As NodeRole, ByVal localIp As String, ByVal localPort As Integer, ByVal shardIndex As Short) Implements Transport.init

		End Sub

		''' <summary>
		''' This method accepts message for delivery, routing is applied according on message opType
		''' </summary>
		''' <param name="message"> </param>
		Public Overridable Sub sendMessage(ByVal message As VoidMessage) Implements Transport.sendMessage

		End Sub

		Public Overridable Function numberOfKnownClients() As Integer Implements Transport.numberOfKnownClients
			Return 0
		End Function

		Public Overridable Function numberOfKnownShards() As Integer Implements Transport.numberOfKnownShards
			Return 0
		End Function

		''' <param name="message"> </param>
		Public Overridable Sub sendMessageToAllShards(ByVal message As VoidMessage) Implements Transport.sendMessageToAllShards

		End Sub

		''' <summary>
		''' This method accepts message from network
		''' </summary>
		''' <param name="message"> </param>
		Public Overridable Sub receiveMessage(ByVal message As VoidMessage) Implements Transport.receiveMessage

		End Sub

		''' <summary>
		''' This method takes 1 message from "incoming messages" queue, blocking if queue is empty
		''' 
		''' @return
		''' </summary>
		Public Overridable Function takeMessage() As VoidMessage Implements Transport.takeMessage
			Return Nothing
		End Function

		''' <summary>
		''' This method puts message into processing queue
		''' </summary>
		''' <param name="message"> </param>
		Public Overridable Sub putMessage(ByVal message As VoidMessage) Implements Transport.putMessage

		End Sub

		''' <summary>
		''' This method peeks 1 message from "incoming messages" queue, returning null if queue is empty
		''' <para>
		''' PLEASE NOTE: This method is suitable for debug purposes only
		''' 
		''' @return
		''' </para>
		''' </summary>
		Public Overridable Function peekMessage() As VoidMessage Implements Transport.peekMessage
			Return Nothing
		End Function

		''' <summary>
		''' This method starts transport mechanisms.
		''' <para>
		''' PLEASE NOTE: init() method should be called prior to launch() call
		''' 
		''' </para>
		''' </summary>
		''' <param name="threading"> </param>
		Public Overridable Sub launch(ByVal threading As ThreadingModel) Implements Transport.launch

		End Sub

		''' <summary>
		''' This method stops transport system.
		''' </summary>
		Public Overridable Sub shutdown() Implements Transport.shutdown

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.parameterserver.distributed.messages.MeaningfulMessage sendMessageAndGetResponse(@NonNull VoidMessage message)
		Public Overridable Function sendMessageAndGetResponse(ByVal message As VoidMessage) As MeaningfulMessage
			Throw New System.NotSupportedException()
		End Function

		Public Overridable ReadOnly Property ShardIndex As Short Implements Transport.getShardIndex
			Get
				Return 0
			End Get
		End Property

		Public Overridable ReadOnly Property TargetIndex As Short Implements Transport.getTargetIndex
			Get
				Return 0
			End Get
		End Property

		Public Overridable Sub setIpAndPort(ByVal ip As String, ByVal port As Integer) Implements Transport.setIpAndPort

		End Sub

		Public Overridable Sub addClient(ByVal ip As String, ByVal port As Integer) Implements Transport.addClient
			'
		End Sub

		Public Overridable ReadOnly Property Ip As String Implements Transport.getIp
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property Port As Integer Implements Transport.getPort
			Get
				Return 0
			End Get
		End Property

		Public Overridable Sub addShard(ByVal ip As String, ByVal port As Integer) Implements Transport.addShard
			' no-op
		End Sub

		Public Overridable Sub sendMessageToAllClients(ByVal message As VoidMessage, ParamArray ByVal exclusions() As Long?) Implements Transport.sendMessageToAllClients
			' no-op
		End Sub

		Public Overridable ReadOnly Property OwnOriginatorId As Long Implements Transport.getOwnOriginatorId
			Get
				Return 0
			End Get
		End Property
	End Class

End Namespace