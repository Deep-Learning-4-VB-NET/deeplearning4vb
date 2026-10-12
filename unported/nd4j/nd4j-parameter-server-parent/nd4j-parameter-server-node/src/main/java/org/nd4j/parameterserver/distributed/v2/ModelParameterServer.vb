Imports System
Imports System.Collections.Generic
Imports Flowable = io.reactivex.Flowable
Imports Disposable = io.reactivex.disposables.Disposable
Imports Consumer = io.reactivex.functions.Consumer
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports org.nd4j.common.primitives
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports PropagationMode = org.nd4j.parameterserver.distributed.v2.enums.PropagationMode
Imports GradientsUpdateMessage = org.nd4j.parameterserver.distributed.v2.messages.impl.GradientsUpdateMessage
Imports HandshakeResponse = org.nd4j.parameterserver.distributed.v2.messages.pairs.handshake.HandshakeResponse
Imports ModelParametersMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersMessage
Imports ModelParametersRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersRequest
Imports UpdaterParametersMessage = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersMessage
Imports UpdaterParametersRequest = org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersRequest
Imports RestartCallback = org.nd4j.parameterserver.distributed.v2.transport.RestartCallback
Imports Transport = org.nd4j.parameterserver.distributed.v2.transport.Transport
Imports UpdaterParametersProvider = org.nd4j.parameterserver.distributed.v2.transport.UpdaterParametersProvider
Imports UpdatesHandler = org.nd4j.parameterserver.distributed.v2.transport.UpdatesHandler
Imports StaticPortSupplier = org.nd4j.parameterserver.distributed.v2.transport.impl.StaticPortSupplier
Imports org.nd4j.parameterserver.distributed.v2.util
Imports UpdaterParametersHolder = org.nd4j.parameterserver.distributed.v2.util.UpdaterParametersHolder
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

Namespace org.nd4j.parameterserver.distributed.v2


	''' 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public final class ModelParameterServer
	Public NotInheritable Class ModelParameterServer
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend Shared ReadOnly INSTANCE_Conflict As New ModelParameterServer()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.parameterserver.distributed.v2.transport.Transport transport;
		Private transport As Transport

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ndarray.INDArray masterModelParams;
		Private masterModelParams As INDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ndarray.INDArray masterUpdaterParams;
		Private masterUpdaterParams As INDArray

		Private updaterParametersProvider As UpdaterParametersProvider

		' queue is used only if there's no subscribers defined
		Private ReadOnly updatesQueue As BlockingQueue(Of INDArray) = New LinkedBlockingQueue(Of INDArray)(4096)

		' subsribers that are connected to actual model
		Protected Friend ReadOnly updatesSubscribers As IList(Of UpdatesHandler) = New CopyOnWriteArrayList(Of UpdatesHandler)()
		Protected Friend ReadOnly modelParamsSubsribers As IList(Of Subscriber(Of INDArray)) = New CopyOnWriteArrayList(Of Subscriber(Of INDArray))()
		Protected Friend ReadOnly updaterParamsSubscribers As IList(Of Subscriber(Of INDArray)) = New CopyOnWriteArrayList(Of Subscriber(Of INDArray))()

		Private masterMode As Boolean

		Protected Friend configuration As VoidConfiguration

		' this flag is true once mps is launched
		Private ReadOnly launchLock As New AtomicBoolean(False)
		Private ReadOnly stopLock As New AtomicBoolean(False)

		' this queue is used as temporary storage for updates received during restart event.
		Protected Friend updatesBacklog As BlockingQueue(Of INDArray) = New LinkedBlockingQueue(Of INDArray)()

		' these two fields only used at master node, to store latest updater copy
		Protected Friend ReadOnly updaterParameters As New Atomic(Of UpdaterParametersHolder)()
		Protected Friend ReadOnly updaterParamsLock As New ReentrantReadWriteLock()
		Protected Friend ReadOnly gotFinalState As New AtomicBoolean(False)

		Private disposable As Disposable


		Private iterationNumber As New AtomicInteger(0)
		Private epochNumber As New AtomicInteger(0)

		Protected Friend Sub New()
			'
		End Sub

		Public Shared ReadOnly Property Instance As ModelParameterServer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' This constructor is for tests only
		''' </summary>
		''' <param name="transport"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected ModelParameterServer(@NonNull Transport transport)
		Protected Friend Sub New(ByVal transport As Transport)
			Me.New(transport, False)
		End Sub

		''' <summary>
		''' This constructor is for tests only
		''' </summary>
		''' <param name="transport"> </param>
		''' <param name="isMasterNode"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected ModelParameterServer(@NonNull Transport transport, boolean isMasterNode)
		Protected Friend Sub New(ByVal transport As Transport, ByVal isMasterNode As Boolean)
			Me.New(VoidConfiguration.builder().portSupplier(New StaticPortSupplier(40123)).streamId(119).build(), transport, isMasterNode)
		End Sub

		''' <summary>
		''' This constructor creates new ModelParameterServer instance
		''' </summary>
		''' <param name="configuration"> VoidConfiguration bean </param>
		''' <param name="transport"> Transport instance to be used for communications </param>
		''' <param name="isMasterNode"> set to true if this parameter server instance will be a master node, false otherwise </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ModelParameterServer(@NonNull VoidConfiguration configuration, @NonNull Transport transport, boolean isMasterNode)
		Public Sub New(ByVal configuration As VoidConfiguration, ByVal transport As Transport, ByVal isMasterNode As Boolean)
			Me.New()
			configure(configuration, transport, isMasterNode)
		End Sub

		''' <summary>
		''' This method stores provided entities for MPS internal use
		''' </summary>
		''' <param name="configuration"> </param>
		''' <param name="transport"> </param>
		''' <param name="isMasterNode"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void configure(@NonNull VoidConfiguration configuration, @NonNull Transport transport, boolean isMasterNode)
		Public Sub configure(ByVal configuration As VoidConfiguration, ByVal transport As Transport, ByVal isMasterNode As Boolean)
			Me.transport = transport
			Me.masterMode = isMasterNode
			Me.configuration = configuration
		End Sub

		''' <summary>
		''' This method stores provided entities for MPS internal use
		''' </summary>
		''' <param name="configuration"> </param>
		''' <param name="transport"> </param>
		''' <param name="isMasterNode"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void configure(@NonNull VoidConfiguration configuration, @NonNull Transport transport, @NonNull UpdaterParametersProvider updaterProvider)
		Public Sub configure(ByVal configuration As VoidConfiguration, ByVal transport As Transport, ByVal updaterProvider As UpdaterParametersProvider)
			Me.transport = transport
			Me.masterMode = False
			Me.configuration = configuration
			Me.updaterParametersProvider = updaterProvider
		End Sub

		''' <summary>
		''' This method adds subcriber that will be called upon gradients update receival </summary>
		''' <param name="s"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addUpdatesSubscriber(@NonNull UpdatesHandler s)
		Public Sub addUpdatesSubscriber(ByVal s As UpdatesHandler)
			updatesSubscribers.Add(s)
		End Sub

		''' <summary>
		''' This method adds subcriber that will be called upon model params receival </summary>
		''' <param name="s"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addModelParamsSubscriber(@NonNull Subscriber<org.nd4j.linalg.api.ndarray.INDArray> s)
		Public Sub addModelParamsSubscriber(ByVal s As Subscriber(Of INDArray))
			modelParamsSubsribers.Add(s)
		End Sub

		''' <summary>
		''' This method adds subcriber that will be called upon updater params receival </summary>
		''' <param name="s"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addUpdaterParamsSubscriber(@NonNull Subscriber<org.nd4j.linalg.api.ndarray.INDArray> s)
		Public Sub addUpdaterParamsSubscriber(ByVal s As Subscriber(Of INDArray))
			updaterParamsSubscribers.Add(s)
		End Sub

		''' <summary>
		''' This method checks if ModelParameterServer was initialized
		''' </summary>
		''' <returns> true if already initalized, false otherwise </returns>
		Public ReadOnly Property Initialized As Boolean
			Get
				Return launchLock.get()
			End Get
		End Property

		''' <summary>
		''' This method returns pair of integers: iteration number and epoch number
		''' @return
		''' </summary>
		Public ReadOnly Property StartPosition As Pair(Of Integer, Integer)
			Get
				Return Pair.makePair(iterationNumber.get(), epochNumber.get())
			End Get
		End Property

		''' <summary>
		''' This method starts parameter server
		''' </summary>
		Public Sub launch()
			SyncLock Me
				log.info("ModelParameterServer starting")
				If launchLock.get() Then
					Return
				End If
        
				configuration.setUnicastControllerPort(configuration.getPortSupplier().getPort())
        
				transport.RestartCallback = New RestartCallbackAnonymousInnerClass(Me)
        
				' listener for model params requests
				transport.addRequestConsumer(GetType(ModelParametersRequest), New ConsumerAnonymousInnerClass(Me))
        
				If masterMode Then
					' on master node when updater params come - we're just storing them
					addUpdaterParamsSubscriber(New AbstractSubscriberAnonymousInnerClass(Me))
        
					' listener for updater params requests
					transport.addRequestConsumer(GetType(UpdaterParametersRequest), New ConsumerAnonymousInnerClass2(Me))
				Else
					' in case of regular
					transport.addRequestConsumer(GetType(UpdaterParametersRequest), New ConsumerAnonymousInnerClass3(Me))
				End If
        
				' this flow will be providing INDArray messages
				disposable = Flowable.fromPublisher(transport.incomingPublisher()).subscribe(Sub(message)
				If TypeOf message Is GradientsUpdateMessage Then
					Dim gum As val = CType(message, GradientsUpdateMessage)
					If iterationNumber.get() < gum.getIteration() Then
						iterationNumber.set(gum.getIteration())
					End If
					If epochNumber.get() < gum.getEpoch() Then
						epochNumber.set(gum.getEpoch())
					End If
					If updatesSubscribers.Count = 0 Then
						updatesQueue.add(message.getPayload())
					Else
						updatesSubscribers.ForEach(Function(s) s.onNext(message.getPayload()))
					End If
				Else
					Throw New System.NotSupportedException("Unknown message received: [" & message.GetType().FullName & "]")
				End If
				End Sub)
        
				' we start transport only once we're ready
				If Me.masterMode Then
					transport.launchAsMaster()
				Else
					transport.launch()
				End If
        
				' instance can be stopped now
				stopLock.set(False)
        
				launchLock.set(True)
			End SyncLock
		End Sub

		Private Class RestartCallbackAnonymousInnerClass
			Implements RestartCallback

			Private ReadOnly outerInstance As ModelParameterServer

			Public Sub New(ByVal outerInstance As ModelParameterServer)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub [call](ByVal response As HandshakeResponse) Implements RestartCallback.call
				' upon restart command we'll request current parameters from the current upstream (without any propagation
				Try
					log.info("Restart callback started...")
					Dim msg As val = New ModelParametersRequest()
					Dim rootId As val = outerInstance.transport.RootId
					Dim modelParams As ModelParametersMessage = outerInstance.transport.sendMessageBlocking(msg, rootId)
					Dim mParams As val = modelParams.Payload
					outerInstance.modelParamsSubsribers.ForEach(Function(s) s.onNext(mParams))

					' updating starting points
					outerInstance.iterationNumber.set(modelParams.getIterationNumber())
					outerInstance.epochNumber.set(modelParams.getEpochNumber())

					' updater parameters are optional, it's possible to have models without updater parameters (i.e. SGD)
					Dim updaterParams As UpdaterParametersMessage = outerInstance.transport.sendMessageBlocking(New UpdaterParametersRequest(), rootId)
					Dim uParams As val = updaterParams.Payload
					If uParams IsNot Nothing Then
						outerInstance.updaterParamsSubscribers.ForEach(Function(s) s.onNext(uParams))
						log.debug("Updater parameters propagated...")
					End If
				Catch e As Exception
					log.error("RestartCallback processing exception: {}", e)
					Throw New Exception(e)
				End Try
			End Sub
		End Class

		Private Class ConsumerAnonymousInnerClass
			Inherits Consumer(Of ModelParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServer

			Public Sub New(ByVal outerInstance As ModelParameterServer)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(org.nd4j.parameterserver.distributed.v2.messages.pairs.params.ModelParametersRequest modelParametersRequest) throws Exception
			Public Overrides Sub accept(ByVal modelParametersRequest As ModelParametersRequest)
				' send model parameters somewhere
				Dim msg As val = New ModelParametersMessage(System.Guid.randomUUID().ToString(), outerInstance.updatesSubscribers(0).getParametersArray())
				msg.setRequestId(modelParametersRequest.RequestId)
				msg.setIterationNumber(outerInstance.iterationNumber.get())
				msg.setEpochNumber(outerInstance.epochNumber.get())
				outerInstance.transport.sendMessage(msg, modelParametersRequest.OriginatorId)
			End Sub
		End Class

		Private Class AbstractSubscriberAnonymousInnerClass
			Inherits AbstractSubscriber(Of INDArray)

			Private ReadOnly outerInstance As ModelParameterServer

			Public Sub New(ByVal outerInstance As ModelParameterServer)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub onNext(ByVal array As INDArray)
				' we're keeping first final updater params state
				If outerInstance.gotFinalState.get() Then
					Return
				End If
				Try
					outerInstance.updaterParamsLock.writeLock().lock()

					' just store new array
					outerInstance.updaterParameters.get().setParameters(array)
					outerInstance.updaterParameters.get().setTimeReceived(DateTimeHelper.CurrentUnixTimeMillis())
				Finally
					outerInstance.updaterParamsLock.writeLock().unlock()
				End Try
			End Sub
		End Class

		Private Class ConsumerAnonymousInnerClass2
			Inherits Consumer(Of UpdaterParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServer

			Public Sub New(ByVal outerInstance As ModelParameterServer)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersRequest updaterParametersRequest) throws Exception
			Public Overrides Sub accept(ByVal updaterParametersRequest As UpdaterParametersRequest)
				' master mode physically can't have own updater parameters, so we're acting as proxy here

				' we're not requesting updater params if
				If Not outerInstance.gotFinalState.get() Then
					Dim tId As val = outerInstance.transport.getRandomDownstreamFrom(outerInstance.transport.RootId, updaterParametersRequest.OriginatorId)
					log.debug("Sending UpdaterParameters request to [{}]", tId)

					' trying to get updaters from root downstreams, excluding original message sender
					Dim updaterParams As UpdaterParametersMessage = outerInstance.transport.sendMessageBlocking(New UpdaterParametersRequest(), tId)
					Dim uParams As val = updaterParams.Payload

					Try
						outerInstance.updaterParamsLock.writeLock().lock()

						If outerInstance.updaterParameters.get() Is Nothing Then
							outerInstance.updaterParameters.set(New UpdaterParametersHolder(uParams, DateTimeHelper.CurrentUnixTimeMillis(), False))
						Else
							outerInstance.updaterParameters.get().setParameters(uParams)
						End If

					Finally
						outerInstance.updaterParamsLock.writeLock().unlock()
					End Try
				End If

				Try
					outerInstance.updaterParamsLock.readLock().lock()

					' send updater parameters somewhere
					log.debug("Trying to send back Updater parameters...")
					Dim msg As val = New UpdaterParametersMessage(System.Guid.randomUUID().ToString(), outerInstance.updaterParameters.get().getParameters())
					msg.setRequestId(updaterParametersRequest.RequestId)
					outerInstance.transport.sendMessage(msg, updaterParametersRequest.OriginatorId)
				Finally
					outerInstance.updaterParamsLock.readLock().unlock()
				End Try
			End Sub
		End Class

		Private Class ConsumerAnonymousInnerClass3
			Inherits Consumer(Of UpdaterParametersRequest)

			Private ReadOnly outerInstance As ModelParameterServer

			Public Sub New(ByVal outerInstance As ModelParameterServer)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void accept(org.nd4j.parameterserver.distributed.v2.messages.pairs.params.UpdaterParametersRequest updaterParametersRequest) throws Exception
			Public Overrides Sub accept(ByVal updaterParametersRequest As UpdaterParametersRequest)
				' master mode physically can't have updater parameters
				log.debug("Trying to send back Updater parameters...")
				If outerInstance.updaterParametersProvider Is Nothing Then
					log.warn("UpdaterParametersProvider wasn't set!")
					Dim msg As val = New UpdaterParametersMessage(System.Guid.randomUUID().ToString(), Nothing)
					msg.setRequestId(updaterParametersRequest.RequestId)
					outerInstance.transport.sendMessage(msg, updaterParametersRequest.OriginatorId)
				Else
					' send updater parameters back
					Dim msg As val = New UpdaterParametersMessage(System.Guid.randomUUID().ToString(), outerInstance.updaterParametersProvider.UpdaterParameters)
					msg.setRequestId(updaterParametersRequest.RequestId)
					outerInstance.transport.sendMessage(msg, updaterParametersRequest.OriginatorId)
				End If
			End Sub
		End Class

		''' <summary>
		''' This method stops parameter server
		''' </summary>
		Public Sub shutdown()
			SyncLock Me
				If stopLock.get() Then
					Return
				End If
        
        
				' shutting down underlying transport
				transport.shutdown()
        
				' disposing INDArray flow
				disposable.dispose()
        
				updaterParamsSubscribers.Clear()
				modelParamsSubsribers.Clear()
				updatesSubscribers.Clear()
				updatesQueue.clear()
        
				' state that we're done
				launchLock.set(False)
        
				stopLock.set(True)
			End SyncLock
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void sendUpdate(@NonNull INDArray array, int iteration, int epoch)
		Public Sub sendUpdate(ByVal array As INDArray, ByVal iteration As Integer, ByVal epoch As Integer)
			Try
				'transport.outgoingConsumer().accept(new GradientsUpdateMessage(java.util.UUID.randomUUID().toString(), array));
				Dim msg As val = New GradientsUpdateMessage(System.Guid.randomUUID().ToString(), array)
				msg.setOriginatorId(transport.id())
				msg.setIteration(iteration)
				msg.setEpoch(epoch)
				transport.propagateMessage(msg, PropagationMode.BOTH_WAYS)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method sends gradient updates to the cluster
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void sendUpdate(@NonNull INDArray array)
		Public Sub sendUpdate(ByVal array As INDArray)
			sendUpdate(array, 0, 0)
		End Sub

		''' <summary>
		''' This method returns updates received from network
		''' @return
		''' </summary>
		Public ReadOnly Property Updates As ICollection(Of INDArray)
			Get
				' just drain stuff from the queue
				Dim list As val = New List(Of INDArray)()
				updatesQueue.drainTo(list)
				Return list
			End Get
		End Property
	End Class

End Namespace