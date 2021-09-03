Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports Model = org.deeplearning4j.nn.api.Model
Imports StepFunction = org.deeplearning4j.optimize.api.StepFunction
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports ResidualClippingPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.residual.ResidualClippingPostProcessor
Imports AdaptiveThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.AdaptiveThresholdAlgorithm
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports org.nd4j.linalg.api.memory.enums
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ThresholdCompression = org.nd4j.linalg.compression.ThresholdCompression
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports AtomicThrowable = org.nd4j.linalg.util.AtomicThrowable

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

Namespace org.deeplearning4j.optimize.solvers.accumulation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class EncodedGradientsAccumulator implements GradientsAccumulator, Registerable
	<Serializable>
	Public Class EncodedGradientsAccumulator
		Implements GradientsAccumulator, Registerable

		Public Shared ReadOnly DEFAULT_INITIAL_MEMORY As Long = 100 * 1024 * 1024L
		Protected Friend accumulator As New ThreadLocal(Of INDArray)()

		Protected Friend parties As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected MessageHandler handler;
		Protected Friend handler As MessageHandler
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected java.util.List<java.util.concurrent.BlockingQueue<org.nd4j.linalg.api.ndarray.INDArray>> messages = new java.util.ArrayList<>();
		Protected Friend messages As IList(Of BlockingQueue(Of INDArray)) = New List(Of BlockingQueue(Of INDArray))()
		Protected Friend workspaces As IList(Of MemoryWorkspace) = New List(Of MemoryWorkspace)()
		Protected Friend locks As IList(Of ReentrantLock) = New List(Of ReentrantLock)()

		Protected Friend workersCounter As New AtomicInteger(0)
		Protected Friend index As New ThreadLocal(Of Integer)()
		Protected Friend initialMemory As Long = 100 * 1024 * 1024L
		Protected Friend queueSize As Integer = 5
		Protected Friend boundary As Integer? = Integer.MaxValue
		Protected Friend encodingDebugMode As Boolean

'JAVA TO VB CONVERTER NOTE: The field externalSource was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend externalSource_Conflict As IndexedTail

		Protected Friend isFirst As New AtomicBoolean(False)
		Protected Friend isDone As New AtomicBoolean(True)

		Protected Friend barrier As New AtomicInteger(0)
		Protected Friend secondary As New AtomicInteger(0)
		Protected Friend registered As New AtomicBoolean(False)
		Protected Friend bypassMode As New AtomicBoolean(False)
		Protected Friend ReadOnly currentConsumers As New AtomicInteger(0)

		Protected Friend ReadOnly throwable As New AtomicThrowable()

		Protected Friend isDebug As Boolean = False
		Protected Friend ReadOnly relocatable As Boolean

		Protected Friend updatesApplied As New ThreadLocal(Of AtomicLong)()

		Protected Friend externalUpdatesAvailable As New AtomicBoolean(False)

		Protected Friend appliedConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().minSize(5 * 1024 * 1024L).overallocationLimit(0.3).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.BLOCK_LEFT).build()

		Public Sub New(ByVal parties As Integer, ByVal threshold As Double)
			Me.New(parties, New AdaptiveThresholdAlgorithm(threshold), New ResidualClippingPostProcessor(5, 5), False)
		End Sub

		Public Sub New(ByVal parties As Integer, ByVal thresholdAlgorithm As ThresholdAlgorithm, ByVal residualPostProcessor As ResidualPostProcessor, ByVal encodingDebugMode As Boolean)
			Me.New(parties, New EncodingHandler(thresholdAlgorithm, residualPostProcessor, Integer.MaxValue, encodingDebugMode), DEFAULT_INITIAL_MEMORY, 10, Integer.MaxValue, encodingDebugMode)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EncodedGradientsAccumulator(int parties, @NonNull MessageHandler handler, long initialMemory, int queueSize, System.Nullable<Integer> boundary, boolean encodingDebugMode)
		Public Sub New(ByVal parties As Integer, ByVal handler As MessageHandler, ByVal initialMemory As Long, ByVal queueSize As Integer, ByVal boundary As Integer?, ByVal encodingDebugMode As Boolean)
			Me.parties = parties
			Me.handler = handler
			Me.initialMemory = initialMemory
			Me.queueSize = queueSize
			Me.boundary = boundary
			Me.encodingDebugMode = encodingDebugMode

			' maybe not the best idea in the world, but we'll use cyclic workspace of 25MB to receive updates
			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(initialMemory).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.STRICT).policySpill(SpillPolicy.FAIL).policyLearning(LearningPolicy.NONE).build()


			' we want to know, if we'll have to relocate data if accessed from different threads/devices
			relocatable = Nd4j.AffinityManager.NumberOfDevices > 1 AndAlso Not Nd4j.AffinityManager.CrossDeviceAccessSupported

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices

			' we are going to take single-device systems as edge case: cpu & small models at single-gpu systems.
			If parties > numDevices AndAlso numDevices <> 1 Then
				Throw New ND4JIllegalStateException("Number of parties [" & parties & "] should be less or equal to number of devices [" & numDevices & "]")
			End If

			' pre-create Queues for local workers
			Dim curDev As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()

			For i As Integer = 0 To parties - 1
				messages.Add(New LinkedBlockingQueue(Of INDArray)(queueSize))

				' we don't want device index to step out of boundaries here
				Dim cDevice As Integer = If(numDevices > 1, i Mod numDevices, 0)

				Nd4j.AffinityManager.unsafeSetDevice(cDevice)
				Dim ws As MemoryWorkspace = Nd4j.WorkspaceManager.createNewWorkspace(configuration, "CGA-" & i, cDevice)
				'ws.enableDebug(true);
				workspaces.Add(ws)

				locks.Add(New ReentrantLock())
			Next i
			Nd4j.AffinityManager.unsafeSetDevice(curDev)

			handler.initialize(Me)
		End Sub

		''' <summary>
		''' This method returns optimal bufferSize for a given model
		''' 
		''' We know, that updates are guaranteed to have MAX size of params / 16. So, here we go.
		''' I.e. for model with 100m params, that's 400m of floats (or 800m of doubles)
		''' The worst case for us is bitmap encoding, that takes 2 bits to encode each gradient value
		''' 
		''' so, for float in worst case we'll have (100m / 16) int elements. So, our buffer size will be 6.25m * queueSize * 4 bytes per int
		''' </summary>
		''' <param name="paramsLength"> </param>
		''' <param name="numWorkers"> </param>
		''' <param name="queueSize">
		''' @return </param>
		Public Shared Function getOptimalBufferSize(ByVal paramsLength As Long, ByVal numWorkers As Integer, ByVal queueSize As Integer) As Long
			' we add 64kb just for future proof volatility
			Dim bufferSize As val = ((paramsLength \ 16) + 65536) * numWorkers * queueSize * 4
			Return bufferSize
		End Function


		Public Shared Function getOptimalBufferSize(ByVal model As Model, ByVal numWorkers As Integer, ByVal queueSize As Integer) As Long
			Return getOptimalBufferSize(model.params().length(), numWorkers, queueSize)
		End Function

		Public Overridable Sub fallbackToSingleConsumerMode(ByVal reallyFallback As Boolean) Implements Registerable.fallbackToSingleConsumerMode
			If externalSource_Conflict IsNot Nothing AndAlso TypeOf externalSource_Conflict Is Registerable Then
				DirectCast(externalSource_Conflict, Registerable).fallbackToSingleConsumerMode(reallyFallback)
			End If

			bypassMode.set(reallyFallback)
		End Sub

		Public Overridable Sub registerConsumers(ByVal numConsumers As Integer) Implements Registerable.registerConsumers
			' we don't want double spending here
			If registered.get() Then
				If isDebug Then
					log.info("Master thread locks at RC")
				End If

				Do While registered.get()
					ThreadUtils.uncheckedSleep(1)
					If throwable.Triggered Then
						Throw New Exception(throwable.get())
					End If
				Loop

				If isDebug Then
					log.info("Master thread unlocks at RC")
				End If
			End If

			' we're passing number of consumers for current session to externalSource, if applicable
			If externalSource_Conflict IsNot Nothing AndAlso TypeOf externalSource_Conflict Is Registerable Then
				'externalUpdatesAvailable.set(!externalSource.isEmpty());

				DirectCast(externalSource_Conflict, Registerable).registerConsumers(numConsumers)
			End If

			currentConsumers.set(numConsumers)
			registered.set(True)
		End Sub

		Public Overridable Property ExternalSource As IndexedTail Implements GradientsAccumulator.getExternalSource
			Get
				Return externalSource_Conflict
			End Get
			Set(ByVal source As IndexedTail)
				Me.externalSource_Conflict = source
			End Set
		End Property

		Public Overridable Sub markExternalUpdates(ByVal updatesAvailable As Boolean) Implements GradientsAccumulator.markExternalUpdates
			externalUpdatesAvailable.set(updatesAvailable)
		End Sub

		Protected Friend Overridable Sub synchronize(ByVal consumers As Integer)
			synchronize(consumers, False)
		End Sub

		Protected Friend Overridable Sub synchronize(ByVal consumers As Integer, ByVal finalLock As Boolean)
			If consumers = 1 OrElse bypassMode.get() Then
				If finalLock Then
					registered.set(False)
				End If

				Return
			End If

			If isDebug Then
				log.info("thread {} locking at CGA: {}", Thread.CurrentThread.getId(), currentConsumers.get())
			End If

			' any first thread entering this block - will reset this field to false
			isDone.compareAndSet(True, False)

			' last thread will set isDone to true
			If barrier.incrementAndGet() = consumers Then
				secondary.set(0)
				barrier.set(0)
				isFirst.set(False)
				isDone.set(True)
			Else
				' just wait, till last thread will set isDone to true
				Do While Not isDone.get()
					ThreadUtils.uncheckedSleep(1)
					If throwable.Triggered Then
						Throw New Exception(throwable.get())
					End If
				Loop
			End If

			' second lock here needed only to ensure we won't get overrun over isDone flag
			If secondary.incrementAndGet() = consumers Then
				If finalLock Then
					registered.set(False)
				End If

				isFirst.set(True)
			Else
				Do While Not isFirst.get()
					ThreadUtils.uncheckedSleep(1)
					If throwable.Triggered Then
						Throw New Exception(throwable.get())
					End If
				Loop
			End If

			If isDebug Then
				log.info("thread {} unlocking at CGA: {}", Thread.CurrentThread.getId(), currentConsumers.get())
			End If

		End Sub

		''' <summary>
		''' This method applies accumulated updates via given StepFunction
		''' </summary>
		''' <param name="function"> </param>
		''' <param name="params"> </param>
		Public Overridable Sub applyUpdate(ByVal [function] As StepFunction, ByVal params As INDArray, ByVal updates As INDArray, ByVal isFinalStep As Boolean) Implements GradientsAccumulator.applyUpdate
			If updatesApplied.get() Is Nothing Then
				updatesApplied.set(New AtomicLong(0))
			End If
			Try
				' nullify given updates first
				Nd4j.MemoryManager.memset(updates)
				'updates.assign(0.0);

				Dim cnt As Integer = 0
				Do While Not messages(index.get()).isEmpty()
					Dim compressed As INDArray = messages(index.get()).poll()

					Dim encoding As Integer = compressed.data().getInt(3)
					If encoding = ThresholdCompression.FLEXIBLE_ENCODING Then
						Nd4j.Executioner.thresholdDecode(compressed, updates)
					ElseIf encoding = ThresholdCompression.BITMAP_ENCODING Then
						Nd4j.Executioner.bitmapDecode(compressed, updates)
					Else
						Throw New DL4JInvalidConfigException("Unknown compression header received: " & encoding)
					End If

					cnt += 1
				Loop

				If cnt > 0 AndAlso isDebug Then
					log.info("Local updates to be applied: {}", cnt)
				End If

				If externalSource_Conflict IsNot Nothing Then
					Dim ent As Integer = 0
					If externalSource_Conflict.hasAnything() Then
						externalSource_Conflict.drainTo(updates)

						cnt += 1
						ent += 1
					End If

					If isDebug Then
						log.info("thread {} finished at Externals", Thread.CurrentThread.getId())
					End If

					If ent > 0 AndAlso isDebug Then
						log.info("External updates to be applied: {}", ent)
					End If
				End If

				If isFinalStep Then
					synchronize(currentConsumers.get(), isFinalStep)
				End If

				' TODO: average updates probably?

				If cnt > 0 Then
					[function].step(params, updates)
					updatesApplied.get().addAndGet(cnt)
					If isDebug Then
						log.info("Total updates applied so far for thread [{}]: [{}]", Thread.CurrentThread.getName(), updatesApplied.get())
					End If
				End If
			Catch e As Exception
				throwable.IfFirst = e
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method applies accumulated updates via given StepFunction
		''' </summary>
		''' <param name="function"> </param>
		''' <param name="params"> </param>
		''' <param name="alpha"> </param>
		Public Overridable Sub applyUpdate(ByVal [function] As StepFunction, ByVal params As INDArray, ByVal updates As INDArray, ByVal alpha As Double) Implements GradientsAccumulator.applyUpdate
			Try
				' nullify given updates first
				Nd4j.MemoryManager.memset(updates)
				'updates.assign(0.0);

				Dim cnt As Integer = 0
				Do While Not messages(index.get()).isEmpty()
					Dim compressed As INDArray = messages(index.get()).poll()

					Dim encoding As Integer = compressed.data().getInt(3)
					If encoding = ThresholdCompression.FLEXIBLE_ENCODING Then
						Nd4j.Executioner.thresholdDecode(compressed, updates)
					ElseIf encoding = ThresholdCompression.BITMAP_ENCODING Then
						Nd4j.Executioner.bitmapDecode(compressed, updates)
					Else
						Throw New DL4JInvalidConfigException("Unknown compression header received: " & encoding)
					End If

					cnt += 1
				Loop

				If cnt > 0 AndAlso isDebug Then
					log.info("Local updates to be applied: {}", cnt)
				End If

				If externalSource_Conflict IsNot Nothing Then
					Dim ent As Integer = 0
					If externalSource_Conflict.hasAnything() Then
						externalSource_Conflict.drainTo(updates)

						cnt += 1
						ent += 1
					End If

					If ent > 0 AndAlso isDebug Then
						log.info("External updates to be applied: {}", ent)
					End If
				End If

				synchronize(currentConsumers.get(), True)

				' TODO: average updates? might have sense

				If cnt > 0 Then
					[function].step(params, updates, alpha)
				End If
			Catch e As Exception
				throwable.IfFirst = e
				Throw New Exception(e)
			End Try
		End Sub


		''' <summary>
		''' This method does initialization of given worker wrt Thread-Device Affinity
		''' </summary>
		Public Overridable Sub touch() Implements GradientsAccumulator.touch
			If index.get() Is Nothing Then
				' set index
				Dim numDevces As Integer = Nd4j.AffinityManager.NumberOfDevices

	'            
	'                if we have > 1 computational device, we assign workers to workspaces "as is", as provided via AffinityManager
	'             
				If numDevces > 1 AndAlso parties > 1 Then
					Dim localIndex As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()

					index.set(localIndex)
				Else
					' if we have only 1 device (like cpu system, or single gpu), just attach consumer via flat index
					index.set(workersCounter.getAndIncrement())
				End If
			End If
		End Sub

		''' <summary>
		''' This method accepts updates suitable for StepFunction, and accumulates/propagates it across all workers
		''' </summary>
		''' <param name="array"> </param>
		Public Overridable Sub storeUpdate(ByVal array As INDArray, ByVal iterationNumber As Integer, ByVal epochNumber As Integer) Implements GradientsAccumulator.storeUpdate
			Try
				If accumulator.get() Is Nothing Then
					' we don't want accumulator to be attached to workspaces
					Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						accumulator.set(Nd4j.create(array.shape(), array.ordering()))
					End Using
				End If

				' accumulate gradients updates in residental array
				accumulator.get().addi(array)

				If isDebug Then
					log.info("thread {} locking at Register", Thread.CurrentThread.getId())
				End If

				' block until ParallelWrapper sends us message about number of threads in this cycle
				If Not bypassMode.get() Then
					Do While Not registered.get()
						ThreadUtils.uncheckedSleep(1)
						If throwable.Triggered Then
							Throw New Exception(throwable.get())
						End If
					Loop
				End If

				If isDebug Then
					log.info("thread {} unlocking at Register", Thread.CurrentThread.getId())
				End If

				' propagate changes & modify accumulator
				handler.broadcastUpdates(accumulator.get(), iterationNumber, epochNumber)

				' we're blocking here, untill all done broadcasting updates
				synchronize(currentConsumers.get())
			Catch e As Exception
				throwable.IfFirst = e
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method accepts updates suitable for StepFunction and puts them to the queue, which is used in backpropagation loop
		''' <para>
		''' PLEASE NOTE: array is expected to be ready for use and match params dimensionality
		''' 
		''' </para>
		''' </summary>
		''' <param name="array"> </param>
		Public Overridable Sub receiveUpdate(ByVal array As INDArray) Implements GradientsAccumulator.receiveUpdate
			Try
				' we're replicating COMPRESSED MESSAGES, decompression will be thread-local
				For i As Integer = 0 To parties - 1
					' we don't want to have same workspace to be accessible by 2 different threads for now
	'                
	'                    With synchronized external data, it's impossible to deadlock here.
	'                    Each worker is guaranteed to have at least NUM_WORKERS slots in buffer.
	'                    So we use this lock just to ensure thread-safety of corresponding workspaces
	'                
					locks(i).lock()

					Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaces(i).notifyScopeEntered()
						' we might just scope out of workspace here, instead of throwing error out
						If array.data().length() > (initialMemory \ queueSize) \ Nd4j.sizeOfDataType(array.data().dataType()) Then
							Throw New ND4JIllegalStateException("Not enough memory to handle update: [" & array.data().length() * Nd4j.sizeOfDataType(array.data().dataType()) & " bytes required]. Please increase memory amount for GradientsAccumulator")
						End If

						Dim compressed As INDArray = array.unsafeDuplication()
						Try
							messages(i).put(compressed)
						Catch e As InterruptedException
							Thread.CurrentThread.Interrupt()
							log.warn("Something bad at index_{}", i)
							Throw New Exception(e)
						End Try
					End Using

					locks(i).unlock()
				Next i
			Catch e As Exception
				throwable.IfFirst = e
				Throw New Exception(e)
			End Try
		End Sub

		''' <summary>
		''' This method resets all accumulated updates (if any)
		''' </summary>
		Public Overridable Sub reset() Implements GradientsAccumulator.reset
			' just replace accumulator, gc will do the rest
			accumulator = New ThreadLocal(Of INDArray)()

			' resetting this counter too
			workersCounter.set(0)

			' reset indexes too
			index = New ThreadLocal(Of Integer)()

			' throw away message queues
			For i As Integer = 0 To parties - 1
				messages(i).clear()
			Next i
		End Sub

		Public Overridable Function hasAnything() As Boolean Implements GradientsAccumulator.hasAnything
			Return externalSource_Conflict IsNot Nothing AndAlso externalSource_Conflict.hasAnything() 'externalUpdatesAvailable.get();
		End Function

		Public Class Builder
			Protected Friend parties As Integer
'JAVA TO VB CONVERTER NOTE: The field thresholdAlgorithm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend thresholdAlgorithm_Conflict As ThresholdAlgorithm
'JAVA TO VB CONVERTER NOTE: The field residualPostProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend residualPostProcessor_Conflict As ResidualPostProcessor
			Protected Friend initialMemory As Long = DEFAULT_INITIAL_MEMORY
			Protected Friend queueSize As Integer = 5
			Protected Friend handler As MessageHandler
			Protected Friend boundary As Integer = Integer.MaxValue
'JAVA TO VB CONVERTER NOTE: The field encodingDebugMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend encodingDebugMode_Conflict As Boolean

			''' <summary>
			''' This </summary>
			''' <param name="parties"> </param>
			Public Sub New(ByVal parties As Integer)
				If parties < 1 Then
					Throw New DL4JInvalidConfigException("Number of parties for GradientsAccumulation should be positive value")
				End If

				Me.parties = parties
			End Sub

			''' <summary>
			''' This method allows to specify MessageHandler instance
			''' 
			''' Default value: EncodingHandler </summary>
			''' <param name="handler">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder messageHandler(@NonNull MessageHandler handler)
			Public Overridable Function messageHandler(ByVal handler As MessageHandler) As Builder
				Me.handler = handler
				Return Me
			End Function

			''' <summary>
			''' This method allows to set the ThresholdAlgorithm to be used for determining the threshold
			''' @return
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter thresholdAlgorithm was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function thresholdAlgorithm(ByVal thresholdAlgorithm_Conflict As ThresholdAlgorithm) As Builder
				Me.thresholdAlgorithm_Conflict = thresholdAlgorithm_Conflict
				Return Me
			End Function

			''' <summary>
			''' Set the residual post processor
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter residualPostProcessor was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function residualPostProcessor(ByVal residualPostProcessor_Conflict As ResidualPostProcessor) As Builder
				Me.residualPostProcessor_Conflict = residualPostProcessor_Conflict
				Return Me
			End Function

			''' <summary>
			''' This method enables optional limit for max number of updates per message
			''' 
			''' Default value: Integer.MAX_VALUE (no limit) </summary>
			''' <param name="boundary"> positive value in range 0..1
			''' @return </param>
			Public Overridable Function updatesBoundary(ByVal boundary As Integer) As Builder
				If boundary <= 0 Then
					Throw New DL4JInvalidConfigException("Boundary should have positive value")
				End If

				Me.boundary = boundary
				Return Me
			End Function


			''' <summary>
			''' This method allows to define buffer memory parameters for this GradientsAccumulator
			''' 
			''' Default values: 100MB initialMemory, 5 queueSize </summary>
			''' <param name="initialMemory"> </param>
			''' <param name="queueSize">
			''' @return </param>
			Public Overridable Function memoryParameters(ByVal initialMemory As Long, ByVal queueSize As Integer) As Builder
				Me.initialMemory = initialMemory
				Me.queueSize = queueSize
				Return Me
			End Function

			Public Overridable Function encodingDebugMode(ByVal enable As Boolean) As Builder
				Me.encodingDebugMode_Conflict = enable
				Return Me
			End Function

			Public Overridable Function build() As EncodedGradientsAccumulator
				If handler Is Nothing Then
					Preconditions.checkNotNull(thresholdAlgorithm_Conflict, "Both threshold algorithm and handler are null - one or the other must be set")
					handler = New EncodingHandler(thresholdAlgorithm_Conflict, residualPostProcessor_Conflict, boundary, encodingDebugMode_Conflict)
				End If

				Dim accumulator As New EncodedGradientsAccumulator(parties, handler, initialMemory, queueSize, boundary, encodingDebugMode_Conflict)

				Return accumulator
			End Function
		End Class
	End Class

End Namespace