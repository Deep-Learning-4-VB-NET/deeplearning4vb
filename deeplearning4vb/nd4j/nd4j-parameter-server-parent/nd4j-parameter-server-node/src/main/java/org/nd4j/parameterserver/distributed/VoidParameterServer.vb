Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports org.nd4j.common.primitives
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports ExecutionMode = org.nd4j.parameterserver.distributed.enums.ExecutionMode
Imports NodeRole = org.nd4j.parameterserver.distributed.enums.NodeRole
Imports org.nd4j.parameterserver.distributed.logic
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports BasicSequenceProvider = org.nd4j.parameterserver.distributed.logic.sequence.BasicSequenceProvider
Imports WordVectorStorage = org.nd4j.parameterserver.distributed.logic.storage.WordVectorStorage
Imports org.nd4j.parameterserver.distributed.messages
Imports org.nd4j.parameterserver.distributed.messages.requests
Imports org.nd4j.parameterserver.distributed.training
Imports SkipGramTrainer = org.nd4j.parameterserver.distributed.training.impl.SkipGramTrainer
Imports RoutedTransport = org.nd4j.parameterserver.distributed.transport.RoutedTransport
Imports Transport = org.nd4j.parameterserver.distributed.transport.Transport
Imports NetworkOrganizer = org.nd4j.parameterserver.distributed.util.NetworkOrganizer

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

Namespace org.nd4j.parameterserver.distributed


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class VoidParameterServer
	<Obsolete>
	Public Class VoidParameterServer
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New VoidParameterServer()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected volatile org.nd4j.parameterserver.distributed.enums.NodeRole nodeRole;
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Protected Friend nodeRole As NodeRole

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile org.nd4j.parameterserver.distributed.conf.VoidConfiguration voidConfiguration;
		Protected Friend voidConfiguration As VoidConfiguration


		Protected Friend initLocker As New AtomicBoolean(False)
		Protected Friend initFinished As New AtomicBoolean(False)
		Protected Friend shutdownLocker As New AtomicBoolean(False)
		Protected Friend shutdownFinished As New AtomicBoolean(False)

'JAVA TO VB CONVERTER NOTE: The field transport was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend transport_Conflict As Transport

		<NonSerialized>
		Protected Friend manualMode As New AtomicBoolean(False)
		<NonSerialized>
		Protected Friend runner As New AtomicBoolean(False)

		<NonSerialized>
		Protected Friend processingThreads() As Thread
		<NonSerialized>
		Protected Friend processingRunnables() As ThreadStart

		' FIXME: we want trainer to be configurable here
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: protected transient org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends TrainingMessage> trainer;
		<NonSerialized>
		Protected Friend trainer As TrainingDriver(Of TrainingMessage)

'JAVA TO VB CONVERTER NOTE: The field shardIndex was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend shardIndex_Conflict As Short

		Protected Friend clipboard As New Clipboard()

		Protected Friend storage As Storage = New WordVectorStorage()

		Protected Friend frames As IDictionary(Of String, Frame(Of TrainingMessage)) = New ConcurrentDictionary(Of String, Frame(Of TrainingMessage))()

		Protected Friend Shared ReadOnly numThreads As Integer = Runtime.getRuntime().availableProcessors() * 2
		Protected Friend executor As ThreadPoolExecutor = CType(Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors() * 2), ThreadPoolExecutor)


		'//////////////////// SeqVec part

		Protected Friend Shared MAX_EXP As Double = 6

		'//////////////////// end of SeqVec part


		Protected Friend Sub New()
			nodeRole = NodeRole.NONE
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected VoidParameterServer(@NonNull NodeRole nodeRole)
		Protected Friend Sub New(ByVal nodeRole As NodeRole)
			Me.nodeRole = nodeRole
		End Sub

		Protected Friend Sub New(ByVal manualMode As Boolean)
			Me.New()
			Me.manualMode.set(manualMode)
		End Sub

		Public Shared ReadOnly Property Instance As VoidParameterServer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setTrainingDriver(@NonNull TrainingDriver<? extends TrainingMessage> trainer)
		Public Overridable WriteOnly Property TrainingDriver(Of T1 As TrainingMessage) As TrainingDriver(Of T1)
			Set(ByVal trainer As TrainingDriver(Of T1))
				Me.trainer = trainer
			End Set
		End Property

		''' <summary>
		''' This method returns shardIndex value.
		''' If current node is Shard - it reutrns it's value
		''' If current node is client - it returns Shard index of paired Shard
		''' @return
		''' </summary>
		Public Overridable Property ShardIndex As Short
			Get
				Return shardIndex_Conflict
			End Get
			Set(ByVal idx As Short)
				shardIndex_Conflict = idx
			End Set
		End Property

		Protected Friend Overridable Sub setIpPortForShard(ByVal ip As String, ByVal port As Integer)
			transport_Conflict.setIpAndPort(ip, port)
		End Sub


		Protected Friend Overridable ReadOnly Property Transport As Transport
			Get
				Return transport_Conflict
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property Syn0 As INDArray
			Get
				Return storage.getArray(WordVectorStorage.SYN_0)
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property Syn1 As INDArray
			Get
				Return storage.getArray(WordVectorStorage.SYN_1)
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property Syn1Neg As INDArray
			Get
				Return storage.getArray(WordVectorStorage.SYN_1_NEGATIVE)
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property ExpTable As INDArray
			Get
				Return storage.getArray(WordVectorStorage.EXP_TABLE)
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property NegTable As INDArray
			Get
				Return storage.getArray(WordVectorStorage.NEGATIVE_TABLE)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void init(@NonNull VoidConfiguration voidConfiguration)
		Protected Friend Overridable Sub init(ByVal voidConfiguration As VoidConfiguration)
			init(voidConfiguration, New RoutedTransport(), New SkipGramTrainer())
		End Sub

		''' <summary>
		''' This method returns True if initialization was started AND was finished, false otherwise
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Init As Boolean
			Get
				Return initFinished.get()
			End Get
		End Property

		''' <summary>
		''' This method starts ParameterServer instance
		''' 
		''' PLEASE NOTE: This method is blocking for first caller only
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Transport transport, org.nd4j.parameterserver.distributed.training.TrainingDriver<? extends TrainingMessage> trainer)
		Public Overridable Sub init(Of T1 As TrainingMessage)(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport, ByVal trainer As TrainingDriver(Of T1))
			''' <summary>
			''' Basic plan here:
			'''      start publishers/listeners/subscribers
			'''      determine role of the current instance:
			'''          Shard
			'''          Backup
			'''          Client
			'''      shutdown unwanted aeron helpers (according to role)
			'''      wait for incoming task queries (according to role
			''' 
			''' </summary>
			If initFinished.get() Then
				Return
			End If

			SyncLock Me
				If initLocker.compareAndSet(False, True) Then
					Me.trainer = trainer
					Me.voidConfiguration = voidConfiguration

					Me.transport_Conflict = transport

					' first we need to check, if our current IP matches designated shards or backup
					If nodeRole = NodeRole.NONE AndAlso (voidConfiguration.getForcedRole() Is Nothing OrElse voidConfiguration.getForcedRole() = NodeRole.NONE) Then
						Dim pair As Pair(Of NodeRole, String) = Nothing
						If voidConfiguration.getShardAddresses().size() = 1 AndAlso voidConfiguration.getShardAddresses().get(0).contains("127.0.0.1") Then
							pair = Pair.create(NodeRole.SHARD, voidConfiguration.getShardAddresses().get(0))
						Else
							pair = getRole(voidConfiguration, getLocalAddresses())
						End If
						nodeRole = pair.First

						Dim ipAndPort As String = pair.Second
						Dim ip As String = "127.0.0.1"
						Dim port As Integer = 0
						' if we're Shard and have port enforced
						If ipAndPort.Contains(":") Then
							Dim split() As String = ipAndPort.Split(":", True)
							ip = split(0)
							port = Convert.ToInt32(split(1))
						Else
							ip = ipAndPort
							port = voidConfiguration.getUnicastControllerPort()
						End If

						' if we're Shard here, we should define shardIndex
						If nodeRole = NodeRole.SHARD AndAlso voidConfiguration.getShardAddresses().size() > 1 Then
							Dim cnt As Short = 0
							For Each shard As String In voidConfiguration.getShardAddresses()
								Dim lIp As String = Nothing
								If shard.Contains(":") Then
									Dim split() As String = ipAndPort.Split(":", True)
									lIp = split(0)
								Else
									lIp = shard
								End If

								If lIp.Equals(ip) Then
									shardIndex_Conflict = cnt
								End If
								cnt += 1
							Next shard
						End If

						Me.transport_Conflict.init(voidConfiguration, clipboard, nodeRole, ip, port, shardIndex_Conflict)

					Else
						If nodeRole = NodeRole.NONE Then
							nodeRole = voidConfiguration.getForcedRole()
						End If

						' if we're using forced roles here, we'll assume that controllerAddress belongs to this box
						Dim localIp As String = If(voidConfiguration.getExecutionMode() = ExecutionMode.MANAGED, voidConfiguration.getControllerAddress(), "127.0.0.1")

						Me.transport_Conflict.init(voidConfiguration, clipboard, nodeRole, localIp, voidConfiguration.getUnicastControllerPort(), shardIndex_Conflict)
					End If


					' TODO: we need real ip only if this is a shard *FOR NOW*, but later we'll need it for client as well

					' we launch message processing if we're not in debug mode
					If Not manualMode.get() Then
						processingThreads = New Thread(numThreads - 1){}
						processingRunnables = New ThreadStart(numThreads - 1){}

						Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()

						For x As Integer = 0 To numThreads - 1
							processingThreads(x) = New Thread(Sub()
							runner.set(True)
							Nd4j.AffinityManager.unsafeSetDevice(deviceId)
							Do While runner.get()
								Try
									handleMessage(transport.takeMessage())
								Catch e As ND4JIllegalStateException
									Throw New Exception(e)
								Catch e As Exception
									Throw New Exception(e)
								End Try
							Loop
							End Sub)

							processingThreads(x).setDaemon(True)
							processingThreads(x).setName("VoidParameterServer messages handling thread")
							processingThreads(x).Start()
						Next x
					End If


					log.info("Launching transport...")
					transport.launch(Transport.ThreadingModel.DEDICATED_THREADS)
					trainer.init(Me.voidConfiguration, Me.transport_Conflict, storage, clipboard)

					initFinished.set(True)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' This method is available for debug purposes only
		''' </summary>
		''' <param name="mode"> </param>
		Protected Friend Overridable Function toggleManualMode(ByVal mode As Boolean) As VoidParameterServer
			manualMode.set(mode)
			Return Me
		End Function

		''' <summary>
		''' This method checks for designated role, according to local IP addresses and configuration passed into method
		''' </summary>
		''' <param name="voidConfiguration"> </param>
		''' <param name="localIPs">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected org.nd4j.common.primitives.Pair<org.nd4j.parameterserver.distributed.enums.NodeRole, String> getRole(@NonNull VoidConfiguration voidConfiguration, @NonNull Collection<String> localIPs)
		Protected Friend Overridable Function getRole(ByVal voidConfiguration As VoidConfiguration, ByVal localIPs As ICollection(Of String)) As Pair(Of NodeRole, String)
			Dim result As NodeRole = NodeRole.CLIENT

			For Each ip As String In voidConfiguration.getShardAddresses()
				Dim cleansed As String = ip.replaceAll(":.*", "")
				If localIPs.Contains(cleansed) Then
					Return Pair.create(NodeRole.SHARD, ip)
				End If
			Next ip

			If voidConfiguration.getBackupAddresses() IsNot Nothing Then
				For Each ip As String In voidConfiguration.getBackupAddresses()
					Dim cleansed As String = ip.replaceAll(":.*", "")
					If localIPs.Contains(cleansed) Then
						Return Pair.create(NodeRole.BACKUP, ip)
					End If
				Next ip
			End If


			Dim sparkIp As String = Nothing


			If sparkIp Is Nothing AndAlso voidConfiguration.getNetworkMask() IsNot Nothing Then
				Dim organizer As New NetworkOrganizer(voidConfiguration.getNetworkMask())
				sparkIp = organizer.MatchingAddress
			End If

			' last resort here...
			If sparkIp Is Nothing Then
				sparkIp = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.DL4J_VOID_IP)
			End If


			log.info("Got [{}] as sparkIp", sparkIp)
			If sparkIp Is Nothing Then
				Throw New ND4JIllegalStateException("Can't get IP address for UDP communcation")
			End If

			' local IP from pair is used for shard only, so we don't care
			Return Pair.create(result, sparkIp & ":" & voidConfiguration.getUnicastControllerPort())
		End Function

		''' <summary>
		''' This method initiates shutdown sequence for this instance.
		''' 
		''' PLEASE NOTE: This method is blocking for first caller only
		''' </summary>
		Public Overridable Sub shutdown()
			''' <summary>
			''' Probably we don't need this method in practice
			''' </summary>
			If initLocker.get() AndAlso shutdownLocker.compareAndSet(False, True) Then
				' do shutdown
				log.info("Shutting down transport...")

				' we just sending out ShutdownRequestMessage
				'transport.sendMessage(new ShutdownRequestMessage());
				transport_Conflict.shutdown()

				executor.shutdown()
				initFinished.set(False)
				initLocker.set(False)
				shutdownLocker.set(False)
			End If
		End Sub

		''' <summary>
		''' This method returns set of local IP addresses available in system.
		''' 
		''' PLEASE NOTE: loopback, disabled interfaces, IPv6 addresses are ignored here.
		''' 
		''' @return
		''' </summary>
		Public Shared ReadOnly Property LocalAddresses As ISet(Of String)
			Get
				Try
					Dim interfaces As IList(Of NetworkInterface) = Collections.list(NetworkInterface.getNetworkInterfaces())
    
					Dim result As ISet(Of String) = New HashSet(Of String)()
    
					For Each networkInterface As NetworkInterface In interfaces
						If networkInterface.isLoopback() OrElse Not networkInterface.isUp() Then
							Continue For
						End If
    
						For Each address As InterfaceAddress In networkInterface.getInterfaceAddresses()
							Dim addr As String = address.getAddress().getHostAddress()
    
							If addr Is Nothing OrElse addr.Length = 0 OrElse addr.Contains(":") Then
								Continue For
							End If
    
							result.Add(addr)
						Next address
					Next networkInterface
    
					Return result
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End Get
		End Property


		' TODO: remove @NonNull check here
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected void handleMessage(@NonNull VoidMessage message)
		Protected Friend Overridable Sub handleMessage(ByVal message As VoidMessage)
			If message Is Nothing Then
				'            log.info("sI_{} got null message", getShardIndex());
				Return
			End If

			If message.TargetId >= 0 AndAlso message.TargetId <> shardIndex_Conflict Then
				log.warn("sI_{}: Skipping message: [{}]; TargetIdx: [{}]", shardIndex_Conflict, message.GetType().Name, message.TargetId)
				Return
			End If

			'      log.info("sI_{}: Processing message: [{}]", shardIndex, message.getClass().getSimpleName());

			message.attachContext(voidConfiguration, trainer, clipboard, transport_Conflict, storage, nodeRole, shardIndex_Conflict)
			message.processMessage()
		End Sub

		''' <summary>
		''' This method handles Shards initialization
		''' 
		''' PLEASE NOTE: This method is blocking
		''' </summary>
		' TODO: right now we support only columnar splits over tables
		Public Overridable Sub initializeSeqVec(ByVal vectorLength As Integer, ByVal numWords As Integer, ByVal seed As Long, ByVal columnsPerShard As Integer, ByVal useHs As Boolean, ByVal useNegSampling As Boolean)
			Dim [dim] As New InitializationRequestMessage(vectorLength, numWords, seed, useHs, useNegSampling, columnsPerShard)
			transport_Conflict.sendMessage([dim])
		End Sub

		''' <summary>
		''' This method dispatches TrainingMessage to ParameterServer network
		''' 
		''' PLEASE NOTE: This method is synchronized and *periodically* becomes blocking by design </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void execDistributed(@NonNull TrainingMessage message)
		Public Overridable Sub execDistributed(ByVal message As TrainingMessage)
			SyncLock Me
				''' <summary>
				''' Basically we should batch messages coming from different TrainingFunctions on spark executor side here.
				''' So we pack them into batches, and send over the wire to selected Shard
				''' </summary>
				Dim currentFrame As Frame
				currentFrame = frames(message.getClass().Name)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if ((currentFrame = frames.get(message.getClass().getSimpleName())) == null)
				If currentFrame Is Nothing Then
					currentFrame = New Frame(Of )(BasicSequenceProvider.Instance.getNextValue())
					frames(message.GetType().Name) = currentFrame
				End If
        
				currentFrame.stackMessage(message)
        
				' TODO: make this threshold variable
				If currentFrame.size() >= 128 Then
					transport_Conflict.sendMessage(currentFrame)
					currentFrame = New Frame(Of )(BasicSequenceProvider.Instance.getNextValue())
					frames(message.GetType().Name) = currentFrame
				End If
        
				'transport.sendMessage(message);
			End SyncLock
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void execDistributedImmediately(@NonNull TrainingMessage message)
		Public Overridable Sub execDistributedImmediately(ByVal message As TrainingMessage)
			transport_Conflict.sendMessageToAllShards(message)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void execDistributed(@NonNull Frame<? extends TrainingMessage> messages)
		Public Overridable Sub execDistributed(Of T1 As TrainingMessage)(ByVal messages As Frame(Of T1))
			transport_Conflict.sendMessage(messages)
		End Sub


		Public Overridable Function getVector(ByVal rowIdx As Integer) As INDArray
			Return getVector(WordVectorStorage.SYN_0, rowIdx)
		End Function

		''' <summary>
		''' This method returns INDArray matching requested storageId value
		''' 
		''' PLEASE NOTE: This method IS blocking
		''' </summary>
		''' <param name="rowIdx">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray getVector(@NonNull Integer key, int rowIdx)
		Public Overridable Function getVector(ByVal key As Integer, ByVal rowIdx As Integer) As INDArray
			''' <summary>
			''' we create VoidMessage, send it, and block until it gets responded
			''' </summary>

			Dim message As New VectorRequestMessage(key, rowIdx)

			Dim response As MeaningfulMessage = transport_Conflict.sendMessageAndGetResponse(message)

			Return response.Payload
		End Function

		''' <summary>
		''' This method sends given message to all Shards
		''' </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void sendMessageToAllShards(@NonNull VoidMessage message)
		Public Overridable Sub sendMessageToAllShards(ByVal message As VoidMessage)
			SyncLock Me
				transport_Conflict.sendMessageToAllShards(message)
			End SyncLock
		End Sub

		''' <summary>
		''' This method sends given message to all Clients
		''' </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void sendMessageToAllClients(@NonNull VoidMessage message)
		Public Overridable Sub sendMessageToAllClients(ByVal message As VoidMessage)
			Me.sendMessageToAllClients(message, Nothing)
		End Sub

		''' <summary>
		''' This method sends given message to all Clients, excluding
		''' </summary>
		''' <param name="message"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized void sendMessageToAllClients(@NonNull VoidMessage message, System.Nullable<Long>... exclusions)
		Public Overridable Sub sendMessageToAllClients(ByVal message As VoidMessage, ParamArray ByVal exclusions() As Long?)
			SyncLock Me
				transport_Conflict.sendMessageToAllClients(message)
			End SyncLock
		End Sub
	End Class

End Namespace