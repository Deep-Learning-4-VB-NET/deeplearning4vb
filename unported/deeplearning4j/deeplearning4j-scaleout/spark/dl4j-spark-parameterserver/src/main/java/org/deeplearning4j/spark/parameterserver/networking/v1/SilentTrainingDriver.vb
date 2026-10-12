Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports org.deeplearning4j.optimize.solvers.accumulation
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports IndexedTail = org.deeplearning4j.optimize.solvers.accumulation.IndexedTail
Imports SilentUpdatesMessage = org.deeplearning4j.spark.parameterserver.networking.v1.messages.SilentUpdatesMessage
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ThresholdCompression = org.nd4j.linalg.compression.ThresholdCompression
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports Storage = org.nd4j.parameterserver.distributed.logic.Storage
Imports Clipboard = org.nd4j.parameterserver.distributed.logic.completion.Clipboard
Imports VoidAggregation = org.nd4j.parameterserver.distributed.messages.VoidAggregation
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

Namespace org.deeplearning4j.spark.parameterserver.networking.v1


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Deprecated public class SilentTrainingDriver implements org.nd4j.parameterserver.distributed.training.TrainingDriver<org.deeplearning4j.spark.parameterserver.networking.v1.messages.SilentUpdatesMessage>
	<Obsolete>
	Public Class SilentTrainingDriver
		Implements TrainingDriver(Of SilentUpdatesMessage)

		<NonSerialized>
		Protected Friend params As INDArray
		<NonSerialized>
		Protected Friend updates As INDArray
		<NonSerialized>
		Protected Friend stepFunction As StepFunction

		<NonSerialized>
		Protected Friend accumulator As GradientsAccumulator

		<NonSerialized>
		Protected Friend voidConfiguration As VoidConfiguration
		<NonSerialized>
		Protected Friend transport As Transport
		<NonSerialized>
		Protected Friend updatesCount As AtomicLong
		<NonSerialized>
		Protected Friend hasSomething As AtomicBoolean

'JAVA TO VB CONVERTER NOTE: The field bypassMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend bypassMode_Conflict As New AtomicBoolean(False)

		<NonSerialized>
		Protected Friend denseCounter As New AtomicLong(0)
		<NonSerialized>
		Protected Friend sparseCounter As New AtomicLong(0)

	'    
	'        We use this buffer to provide double buffering for incoming messages.
	'        So we store incoming messages right here, and apply them as time comes
	'     
'JAVA TO VB CONVERTER NOTE: The field updatesBuffer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend updatesBuffer_Conflict As IndexedTail

		' these 2 are not used here
		<NonSerialized>
		Protected Friend storage As Storage
		<NonSerialized>
		Protected Friend clipboard As Clipboard


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SilentTrainingDriver(@NonNull GradientsAccumulator accumulator)
		Public Sub New(ByVal accumulator As GradientsAccumulator)
			log.info("Creating TrainingDriver for worker...")
			Me.accumulator = accumulator
			Me.updatesCount = New AtomicLong(0)

			' TODO: make this configurable
			Me.updatesBuffer_Conflict = New IndexedTail(1)

			' FBQ will guarantee that all workers using given queue will be applying the same updates in the same order
			Me.accumulator.ExternalSource = updatesBuffer_Conflict
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SilentTrainingDriver(@NonNull INDArray params, @NonNull StepFunction stepFunction)
		Public Sub New(ByVal params As INDArray, ByVal stepFunction As StepFunction)
			log.info("Creating TrainingDriver for master...")
			log.info("Params at Master BEFORE: {}", params.meanNumber().doubleValue())
			Me.params = params
			Me.stepFunction = stepFunction
			Me.updatesCount = New AtomicLong(0)

			Me.hasSomething = New AtomicBoolean(False)

			' updates are always the same size as params
			Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Me.updates = Nd4j.create(params.shape(), params.ordering())
			End Using
		End Sub

		''' <summary>
		''' This method is viable only at Spark Workers, Master node will always have empty buffer here by design
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property UpdatesBuffer As IndexedTail
			Get
				Return updatesBuffer_Conflict
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull VoidConfiguration voidConfiguration, @NonNull Transport transport, org.nd4j.parameterserver.distributed.logic.Storage storage, org.nd4j.parameterserver.distributed.logic.completion.Clipboard clipboard)
		Public Overridable Sub init(ByVal voidConfiguration As VoidConfiguration, ByVal transport As Transport, ByVal storage As Storage, ByVal clipboard As Clipboard)
			Me.voidConfiguration = voidConfiguration
			Me.transport = transport
		End Sub

		Public Overridable Sub bypassMode(ByVal reallyBypass As Boolean)
			bypassMode_Conflict.set(reallyBypass)

			' if TrainingDriver is temporary disabled - remove existing messages from queue
			If reallyBypass Then
				'updatesBuffer.clear();
			End If
		End Sub

		Public Overridable Sub startTraining(ByVal message As SilentUpdatesMessage)
	'        
	'            this method will be invoked on master, and will do 2 things:
	'            1) silently update params via given StepFunction
	'            2) propagate this message to everyone
	'        
	'            on workers, it just enqueues updates into the FancyBlockingQueue
	'         
			' if accumulator is defined, we're working at Worker level, so it's not our problem what happens inside
			If accumulator IsNot Nothing Then
				If message.OriginatorId = transport.OwnOriginatorId Then
					'log.info("Skipping since originators match");
					Return
				End If

	'            
	'                we're just putting messages here. if thread gets blocked - messages won't be arriving,
	'                enforcing periodic messages retransmission from other nodes, so we should be all fine
	'              

				Try
					If Not bypassMode_Conflict.get() Then
						updatesBuffer_Conflict.put(message.getUpdates())
					End If
				Catch e As Exception
					Throw New Exception(e)
				End Try

				'accumulator.receiveUpdate(message.getUpdates());
			ElseIf params IsNot Nothing AndAlso stepFunction IsNot Nothing Then
				' master invokes everything, since that's Silent Worker approach: we want master to be always up-to-date
				SyncLock Me
					' threshold decoder is inplace & fast
					Dim encoding As Integer = message.getUpdates().data().getInt(3)
					If encoding = ThresholdCompression.FLEXIBLE_ENCODING Then
						Nd4j.Executioner.thresholdDecode(message.getUpdates(), updates)
						sparseCounter.incrementAndGet()
					ElseIf encoding = ThresholdCompression.BITMAP_ENCODING Then
						Nd4j.Executioner.bitmapDecode(message.getUpdates(), updates)
						denseCounter.incrementAndGet()
					Else
						Throw New DL4JInvalidConfigException("Unknown compression header received: " & encoding)
					End If

	'                
	'                if ((sparseCounter.get() + denseCounter.get()) % 100 == 0) {
	'                    log.info("Sparse/Dense ratio: {}", String.format("%.2f", (sparseCounter.get() +1) / (double) (denseCounter.get() + 1)));
	'                }
	'                


					' this simple flag shows that we have something not applied, will be used at finishTraining() method
					hasSomething.set(True)

					' we apply updates every X iterations, and we don't really need X to be small here
					If updatesCount.incrementAndGet() Mod Math.Max(transport.numberOfKnownClients(), 5) = 0 Then
						stepFunction.step(params, updates)

						' once accumulated updates are applied - reset storage, and wait for other messsages
						Nd4j.MemoryManager.memset(updates)
						hasSomething.set(False)
					End If
				End SyncLock

				' we should echo this message to everyone but this shard, but only if there's > 1 shard/client available
				If transport.numberOfKnownClients() > 1 Then
					'log.info("Resending message, skipping {}", message.getOriginatorId());
					transport.sendMessageToAllClients(message, message.OriginatorId, transport.OwnOriginatorId)
				End If ' else log.info("No known Clients so far");
			Else
				Throw New DL4JInvalidConfigException("Neither GradientsAccumulator or StepFunction is defined!")
			End If
		End Sub

		Public Overridable Sub pickTraining(ByVal message As SilentUpdatesMessage)
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Sub aggregationFinished(ByVal aggregation As VoidAggregation) Implements TrainingDriver(Of SilentUpdatesMessage).aggregationFinished
			Throw New System.NotSupportedException()
		End Sub

		''' <summary>
		''' This method is used on Master only, applies buffered updates to params
		''' </summary>
		''' <param name="originatorId"> </param>
		''' <param name="taskId"> </param>
		Public Overridable Sub finishTraining(ByVal originatorId As Long, ByVal taskId As Long) Implements TrainingDriver(Of SilentUpdatesMessage).finishTraining
			' on Master thread we'll be applying final gradients

			If params IsNot Nothing AndAlso stepFunction IsNot Nothing Then
				If hasSomething.get() Then
					stepFunction.step(params, updates)
					'Nd4j.getMemoryManager().memset(updates);
					updates.assign(0.0)
				End If
			End If

		End Sub

		Public Overridable Sub addCompletionHook(ByVal originatorId As Long, ByVal frameId As Long, ByVal messageId As Long) Implements TrainingDriver(Of SilentUpdatesMessage).addCompletionHook
			' no-op
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function targetMessageClass() As String Implements TrainingDriver(Of SilentUpdatesMessage).targetMessageClass
			Return GetType(SilentUpdatesMessage).Name
		End Function
	End Class

End Namespace