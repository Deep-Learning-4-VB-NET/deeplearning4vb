Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports RequestMessage = org.nd4j.parameterserver.distributed.v2.messages.RequestMessage
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage
Imports org.nd4j.parameterserver.distributed.v2.transport
Imports Transport = org.nd4j.parameterserver.distributed.v2.transport.Transport
Imports MeshOrganizer = org.nd4j.parameterserver.distributed.v2.util.MeshOrganizer
Imports MessageSplitter = org.nd4j.parameterserver.distributed.v2.util.MessageSplitter

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
'ORIGINAL LINE: @Slf4j public class DummyTransport extends BaseTransport
	Public Class DummyTransport
		Inherits BaseTransport

		' this is for tests only
		Protected Friend interceptors As IDictionary(Of String, MessageCallable) = New Dictionary(Of String, MessageCallable)()
		Protected Friend precursors As IDictionary(Of String, MessageCallable) = New Dictionary(Of String, MessageCallable)()

		Protected Friend ReadOnly connector As Connector


		Public Sub New(ByVal id As String, ByVal connector As Connector)
			MyBase.New()
			Me.id = id
			Me.connector = connector

			Me.splitter = New MessageSplitter()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DummyTransport(String id, Connector connector, @NonNull String rootId)
		Public Sub New(ByVal id As String, ByVal connector As Connector, ByVal rootId As String)
			MyBase.New(rootId)
			Me.id = id
			Me.connector = connector

			Me.splitter = New MessageSplitter()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DummyTransport(String id, Connector connector, @NonNull String rootId, @NonNull VoidConfiguration configuration)
		Public Sub New(ByVal id As String, ByVal connector As Connector, ByVal rootId As String, ByVal configuration As VoidConfiguration)
			MyBase.New(rootId, configuration)
			Me.id = id
			Me.connector = connector

			Me.splitter = New MessageSplitter()
		End Sub

		Public Overrides Sub launch()
			MyBase.launch()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void sendMessage(@NonNull VoidMessage message, @NonNull String id)
		Public Overridable Overloads Sub sendMessage(ByVal message As VoidMessage, ByVal id As String)
			If message.getOriginatorId() Is Nothing Then
				message.setOriginatorId(Me.id())
			End If

			'if (message.getMessageId() == null)


			' TODO: get rid of UUID!!!11
			If TypeOf message Is RequestMessage Then
				If DirectCast(message, RequestMessage).RequestId Is Nothing Then
					DirectCast(message, RequestMessage).RequestId = System.Guid.randomUUID().ToString()
				End If
			End If

			connector.transferMessage(message, Me.id(), id)
		End Sub

		Public Overrides Function id() As String
			Return id
		End Function

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

				MyBase.internalProcessMessage(message)
			End If
		End Sub

		Protected Friend Overrides Sub internalProcessMessage(ByVal message As VoidMessage)
			processMessage(message)
		End Sub

		''' <summary>
		''' This class is written to mimic network connectivity locally
		''' </summary>
		Public Class Connector
			Friend transports As IDictionary(Of String, Transport) = New ConcurrentDictionary(Of String, Transport)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private ThreadPoolExecutor executorService = (ThreadPoolExecutor) Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors(), new ThreadFactory()
'JAVA TO VB CONVERTER NOTE: The field executorService was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend executorService_Conflict As ThreadPoolExecutor = CType(Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors(), New ThreadFactoryAnonymousInnerClass()), ThreadPoolExecutor)

			Private Class ThreadFactoryAnonymousInnerClass
				Inherits ThreadFactory

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public Thread newThread(@NonNull Runnable r)
				Public Overrides Function newThread(ByVal r As ThreadStart) As Thread
					Dim t As val = Executors.defaultThreadFactory().newThread(r)
					't.setDaemon(true);
					Return t
				End Function
			End Class

			Public Overridable Sub register(ParamArray ByVal transports() As Transport)
				For Each transport As val In transports
					If Not Me.transports.ContainsKey(transport.id()) : Me.transports.Add(transport.id(), transport)
				Next transport
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void blockUntilFinished() throws InterruptedException
			Public Overridable Sub blockUntilFinished()
				Dim timeStart As val = DateTimeHelper.CurrentUnixTimeMillis()
				Do While executorService_Conflict.getActiveCount() > 0 AndAlso executorService_Conflict.getQueue().size() > 0
					Thread.Sleep(500)
				Loop
				Dim timeStop As val = DateTimeHelper.CurrentUnixTimeMillis()

				If timeStop - timeStart < 700 Then
					Thread.Sleep(700)
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void transferMessage(@NonNull VoidMessage message, @NonNull String senderId, @NonNull String targetId)
			Public Overridable Sub transferMessage(ByVal message As VoidMessage, ByVal senderId As String, ByVal targetId As String)

				'if (message instanceof GradientsUpdateMessage)
				'    log.info("Trying to send message [{}] from [{}] to [{}]", message.getClass().getSimpleName(), senderId, targetId);

				Dim target As val = transports(targetId)
				If target Is Nothing Then
					Throw New ND4JIllegalStateException("Unknown target specified")
				End If

				target.processMessage(message)
			End Sub

			Public Overridable Function executorService() As ExecutorService
				Return executorService_Conflict
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void dropConnection(@NonNull String... ids)
			Public Overridable Sub dropConnection(ParamArray ByVal ids() As String)
				ids.Where(AddressOf Objects.nonNull).ForEach(AddressOf transports.remove)
			End Sub
		End Class

		''' <summary>
		''' This method returns Mesh stored in this Transport instance
		''' PLEASE NOTE: This method is suited for tests
		''' @return
		''' </summary>
		Public Overridable Property Mesh As MeshOrganizer
			Get
				SyncLock mesh
					Return mesh.get()
				End SyncLock
			End Get
			Set(ByVal mesh As MeshOrganizer)
				SyncLock Me.mesh
					Me.mesh.set(mesh)
				End SyncLock
			End Set
		End Property


		Public Overrides ReadOnly Property Connected As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

End Namespace