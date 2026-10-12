Imports System
Imports lombok
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports org.nd4j.parameterserver.distributed.training
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport

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

Namespace org.nd4j.parameterserver.distributed.messages

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Data @Deprecated public abstract class BaseVoidMessage implements VoidMessage
	<Obsolete, Serializable>
	Public MustInherit Class BaseVoidMessage
		Implements VoidMessage

		Public MustOverride ReadOnly Property RetransmitCount As Integer Implements VoidMessage.getRetransmitCount
		Public MustOverride Sub processMessage() Implements VoidMessage.processMessage
		Public MustOverride Function fromBytes(ByVal array() As SByte) As T
		Public MustOverride Property OriginatorId As Long
		Public MustOverride ReadOnly Property TaskId As Long Implements VoidMessage.getTaskId
		Public MustOverride Property TargetId As Short Implements VoidMessage.getTargetId
'JAVA TO VB CONVERTER NOTE: The field messageType was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend messageType_Conflict As Integer = -1
		Protected Friend originatorId As Long = 0L
		Protected Friend taskId As Long
		Protected Friend targetId As Short

		' these fields are used only for op invocation
		<NonSerialized>
		Protected Friend voidConfiguration As VoidConfiguration
		<NonSerialized>
		Protected Friend clipboard As Clipboard
		<NonSerialized>
		Protected Friend transport As Transport
		<NonSerialized>
		Protected Friend storage As Storage
		<NonSerialized>
		Protected Friend role As NodeRole
		<NonSerialized>
		Protected Friend shardIndex As Short
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected transient org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends TrainingMessage> trainer;
		<NonSerialized>
		Protected Friend trainer As TrainingDriver(Of TrainingMessage)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(AccessLevel.@PRIVATE) protected transient int retransmitCount = 0;
		<NonSerialized>
		Protected Friend retransmitCount As Integer = 0

		Protected Friend Sub New(ByVal messageType As Integer)
			Me.messageType_Conflict = messageType
		End Sub

		Public Overridable Function asBytes() As SByte() Implements VoidMessage.asBytes
			Return SerializationUtils.toByteArray(Me)
		End Function

		Public Overridable ReadOnly Property MessageType As Integer Implements VoidMessage.getMessageType
			Get
				Return messageType_Conflict
			End Get
		End Property


		Public Overridable Function asUnsafeBuffer() As UnsafeBuffer Implements VoidMessage.asUnsafeBuffer
			Return New UnsafeBuffer(asBytes())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void attachContext(@NonNull VoidConfiguration voidConfiguration, @NonNull TrainingDriver<? extends TrainingMessage> trainer, @NonNull Clipboard clipboard, @NonNull Transport transport, @NonNull Storage storage, @NonNull NodeRole role, short shardIndex)
		Public Overridable Sub attachContext(Of T1 As TrainingMessage)(ByVal voidConfiguration As VoidConfiguration, ByVal trainer As TrainingDriver(Of T1), ByVal clipboard As Clipboard, ByVal transport As Transport, ByVal storage As Storage, ByVal role As NodeRole, ByVal shardIndex As Short)
			Me.voidConfiguration = voidConfiguration
			Me.clipboard = clipboard
			Me.transport = transport
			Me.storage = storage
			Me.role = role
			Me.shardIndex = shardIndex
			Me.trainer = trainer
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void extractContext(@NonNull BaseVoidMessage message)
		Public Overridable Sub extractContext(ByVal message As BaseVoidMessage) Implements VoidMessage.extractContext
			Me.voidConfiguration = message.voidConfiguration
			Me.clipboard = message.clipboard
			Me.transport = message.transport
			Me.storage = message.storage
			Me.role = message.role
			Me.shardIndex = message.shardIndex
			Me.trainer = message.trainer
			Me.originatorId = message.originatorId
		End Sub

		Public Overridable ReadOnly Property JoinSupported As Boolean Implements VoidMessage.isJoinSupported
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub joinMessage(ByVal message As VoidMessage) Implements VoidMessage.joinMessage
			' no-op
		End Sub

		Public Overridable ReadOnly Property BlockingMessage As Boolean Implements VoidMessage.isBlockingMessage
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub incrementRetransmitCount() Implements VoidMessage.incrementRetransmitCount
			retransmitCount += 1
		End Sub
	End Class

End Namespace