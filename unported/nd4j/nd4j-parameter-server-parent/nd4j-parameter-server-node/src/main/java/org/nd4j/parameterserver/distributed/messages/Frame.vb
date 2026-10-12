Imports System
Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
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
'ORIGINAL LINE: @Slf4j @Deprecated public class Frame<T extends TrainingMessage> implements java.io.Serializable, Iterable<T>, VoidMessage
	<Obsolete, Serializable>
	Public Class Frame(Of T As TrainingMessage)
		Implements IEnumerable(Of T), VoidMessage

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) @Setter(lombok.AccessLevel.@PROTECTED) protected java.util.List<T> list = new java.util.ArrayList<T>();
		Protected Friend list As IList(Of T) = New List(Of T)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected long originatorId;
'JAVA TO VB CONVERTER NOTE: The field originatorId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend originatorId_Conflict As Long
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected short targetId;
		Protected Friend targetId As Short
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected long taskId;
		Protected Friend taskId As Long


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
'ORIGINAL LINE: @Getter @Setter(lombok.AccessLevel.@PRIVATE) protected transient int retransmitCount = 0;
		<NonSerialized>
		Protected Friend retransmitCount As Integer = 0

		Protected Friend Sub New()

		End Sub

		Public Sub New(ByVal taskId As Long)
			Me.taskId = taskId
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Frame(@NonNull T message)
		Public Sub New(ByVal message As T)
			Me.New()
			list.Add(message)
		End Sub

		Public Overridable WriteOnly Property OriginatorId Implements VoidMessage.setOriginatorId As Long
			Set(ByVal id As Long)
				Me.originatorId_Conflict = id
				If list IsNot Nothing Then
					list.ForEach(Sub(msg)
					msg.setOriginatorId(Me.OriginatorId)
					End Sub)
				End If
			End Set
		End Property

		''' <summary>
		''' This method adds single TrainingMessage to this Frame
		''' 
		''' PLEASE NOTE: This method is synchronized </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void stackMessage(@NonNull T message)
		Public Overridable Sub stackMessage(ByVal message As T)
			SyncLock Me
				stackMessageUnlocked(message)
			End SyncLock
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private void stackMessageUnlocked(@NonNull T message)
		Private Sub stackMessageUnlocked(ByVal message As T)
			If message.JoinSupported Then
				Dim index As Integer = list.IndexOf(message)
				If index >= 0 Then
					list(index).joinMessage(message)
				Else
					message.FrameId = Me.TaskId
					list.Add(message)
				End If
			Else
				message.FrameId = Me.TaskId
				list.Add(message)
			End If
		End Sub

		''' <summary>
		''' This method adds multiple messages to this frame
		''' 
		''' PLEASE NOTE: This method is synchronized </summary>
		''' <param name="messages"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void stackMessages(@NonNull Collection<T> messages)
		Public Overridable Sub stackMessages(ByVal messages As ICollection(Of T))
			SyncLock Me
				For Each message As T In messages
					stackMessageUnlocked(message)
				Next message
			End SyncLock
		End Sub

		''' <summary>
		''' This method adds multiple messages to this frame
		''' 
		''' PLEASE NOTE: This method is synchronized </summary>
		''' <param name="messages"> </param>
		Public Overridable Sub stackMessages(ParamArray ByVal messages() As T)
			SyncLock Me
				For Each message As T In messages
					If message IsNot Nothing Then
						stackMessageUnlocked(message)
					End If
				Next message
			End SyncLock
		End Sub

		Public Overridable ReadOnly Property Messages As ICollection(Of T)
			Get
				Return list
			End Get
		End Property

		Public Overridable Function size() As Integer
			Return list.Count
		End Function

		Public Overridable Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerator(Of T).GetEnumerator
			Return list.GetEnumerator()
		End Function

		Public Overridable ReadOnly Property MessageType As Integer Implements VoidMessage.getMessageType
			Get
				Return 3
			End Get
		End Property

		Public Overridable Function asBytes() As SByte() Implements VoidMessage.asBytes
			Return SerializationUtils.toByteArray(Me)
		End Function

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
			Me.originatorId_Conflict = message.originatorId
		End Sub

		Public Overridable Sub processMessage() Implements VoidMessage.processMessage
			'        log.info("Processing frame {} of {} messages... Originator: {}", this.getTaskId(), list.size(), originatorId);

			' we register all messages first
			'      if(list == null || trainer == null)
			'          return;
			If trainer IsNot Nothing AndAlso transport IsNot Nothing Then
				list.ForEach(Sub(message)
				trainer.addCompletionHook(OriginatorId, TaskId, message.getTaskId())
				End Sub)
			End If

			'list.parallelStream().forEach((message) -> {
			For Each message As TrainingMessage In list
				If trainer IsNot Nothing AndAlso transport IsNot Nothing Then
					message.attachContext(voidConfiguration, trainer, clipboard, transport, storage, role, shardIndex)
				End If

				' if there's more then 1 round should be applied
				Dim i As Integer = 0
				Do While i < message.Counter
					'log.info("Firing message {}; originator: {}; frameId: {}; taskId: {}", message.getClass().getSimpleName(), message.getOriginatorId(), message.getFrameId(), message.getTaskId());
					message.processMessage()
					i += 1
				Loop
			Next message
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
				Return True
			End Get
		End Property

		Public Overridable Sub incrementRetransmitCount() Implements VoidMessage.incrementRetransmitCount
			retransmitCount += 1
		End Sub
	End Class

End Namespace