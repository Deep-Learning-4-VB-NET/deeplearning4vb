Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports DataSetCallback = org.nd4j.linalg.dataset.callbacks.DataSetCallback
Imports DefaultCallback = org.nd4j.linalg.dataset.callbacks.DefaultCallback
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.dataset


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class AsyncDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class AsyncDataSetIterator
		Implements DataSetIterator

		Protected Friend backedIterator As DataSetIterator

		Protected Friend terminator As New DataSet()
		Protected Friend nextElement As DataSet = Nothing
		Protected Friend buffer As BlockingQueue(Of DataSet)
		Protected Friend thread As AsyncPrefetchThread
		Protected Friend shouldWork As New AtomicBoolean(True)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile RuntimeException throwable = null;
		Protected Friend throwable As Exception = Nothing
		Protected Friend useWorkspace As Boolean = True
		Protected Friend prefetchSize As Integer
		Protected Friend workspaceId As String
		Protected Friend deviceId As Integer?
		Protected Friend hasDepleted As New AtomicBoolean(False)

		Protected Friend callback As DataSetCallback

		Protected Friend Sub New()
			'
		End Sub

		''' <summary>
		''' Create an Async iterator with the default queue size of 8 </summary>
		''' <param name="baseIterator"> Underlying iterator to wrap and fetch asynchronously from </param>
		Public Sub New(ByVal baseIterator As DataSetIterator)
			Me.New(baseIterator, 8)
		End Sub

		''' <summary>
		''' Create an Async iterator with the default queue size of 8 </summary>
		''' <param name="iterator"> Underlying iterator to wrap and fetch asynchronously from </param>
		''' <param name="queue">    Queue size - number of iterators to </param>
		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet))
			Me.New(iterator, queueSize, queue, True)
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize))
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize), useWorkspace)
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean, ByVal deviceId As Integer?)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize), useWorkspace, New DefaultCallback(), deviceId)
		End Sub

		Public Sub New(ByVal baseIterator As DataSetIterator, ByVal queueSize As Integer, ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback)
			Me.New(baseIterator, queueSize, New LinkedBlockingQueue(Of DataSet)(queueSize), useWorkspace, callback)
		End Sub

		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet), ByVal useWorkspace As Boolean)
			Me.New(iterator, queueSize, queue, useWorkspace, New DefaultCallback())
		End Sub

		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback)
			Me.New(iterator, queueSize, queue, useWorkspace, callback, Nd4j.AffinityManager.getDeviceForCurrentThread())
		End Sub

		Public Sub New(ByVal iterator As DataSetIterator, ByVal queueSize As Integer, ByVal queue As BlockingQueue(Of DataSet), ByVal useWorkspace As Boolean, ByVal callback As DataSetCallback, ByVal deviceId As Integer?)
			If queueSize < 2 Then
				queueSize = 2
			End If

			Me.deviceId = deviceId
			Me.callback = callback
			Me.useWorkspace = useWorkspace
			Me.buffer = queue
			Me.prefetchSize = queueSize
			Me.backedIterator = iterator
			Me.workspaceId = "ADSI_ITER-" & System.Guid.randomUUID().ToString()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If iterator.resetSupported() AndAlso Not iterator.hasNext() Then
				Me.backedIterator.reset()
			End If

			Me.thread = New AsyncPrefetchThread(Me, buffer, iterator, terminator, Nothing, deviceId)

			thread.setDaemon(True)
			thread.Start()
		End Sub

		''' <summary>
		''' Like the standard next method but allows a
		''' customizable number of examples returned
		''' </summary>
		''' <param name="num"> the number of examples </param>
		''' <returns> the next data applyTransformToDestination </returns>
		Public Overridable Function [next](ByVal num As Integer) As DataSet
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' Input columns for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return backedIterator.inputColumns()
		End Function

		''' <summary>
		''' The number of labels for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return backedIterator.totalOutcomes()
		End Function

		''' <summary>
		''' Is resetting supported by this DataSetIterator? Many DataSetIterators do support resetting,
		''' but some don't
		''' </summary>
		''' <returns> true if reset method is supported; false otherwise </returns>
		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return backedIterator.resetSupported()
		End Function

		''' <summary>
		''' Does this DataSetIterator support asynchronous prefetching of multiple DataSet objects?
		''' Most DataSetIterators do, but in some cases it may not make sense to wrap this iterator in an
		''' iterator that does asynchronous prefetching. For example, it would not make sense to use asynchronous
		''' prefetching for the following types of iterators:
		''' (a) Iterators that store their full contents in memory already
		''' (b) Iterators that re-use features/labels arrays (as future next() calls will overwrite past contents)
		''' (c) Iterators that already implement some level of asynchronous prefetching
		''' (d) Iterators that may return different data depending on when the next() method is called
		''' </summary>
		''' <returns> true if asynchronous prefetching from this iterator is OK; false if asynchronous prefetching should not
		''' be used with this iterator </returns>
		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return False
		End Function

		Protected Friend Overridable Sub externalCall()
			' for spark
		End Sub

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Public Overridable Sub reset() Implements DataSetIterator.reset
			buffer.clear()

			If thread IsNot Nothing Then
				thread.Interrupt()
			End If
			Try
				' Shutdown() should be a synchronous operation since the iterator is reset after shutdown() is
				' called in AsyncLabelAwareIterator.reset().
				If thread IsNot Nothing Then
					thread.Join()
				End If
			Catch e As InterruptedException
				Thread.CurrentThread.Interrupt()
				Throw New Exception(e)
			End Try
			Me.thread.shutdown()
			buffer.clear()


			backedIterator.reset()
			shouldWork.set(True)
			Me.thread = New AsyncPrefetchThread(Me, buffer, backedIterator, terminator, Nothing, deviceId)

			thread.setDaemon(True)
			thread.Start()
			hasDepleted.set(False)

			nextElement = Nothing
		End Sub

		''' <summary>
		''' This method will terminate background thread AND will destroy attached workspace (if any)
		''' 
		''' PLEASE NOTE: After shutdown() call, this instance can't be used anymore
		''' </summary>
		Public Overridable Sub shutdown()
			buffer.clear()

			If thread IsNot Nothing Then
				thread.Interrupt()
			End If
			Try
				' Shutdown() should be a synchronous operation since the iterator is reset after shutdown() is
				' called in AsyncLabelAwareIterator.reset().
				If thread IsNot Nothing Then
					thread.Join()
				End If
			Catch e As InterruptedException
				Thread.CurrentThread.Interrupt()
				Throw New Exception(e)
			End Try
			Me.thread.shutdown()
			buffer.clear()
		End Sub

		''' <summary>
		''' Batch size
		''' 
		''' @return
		''' </summary>
		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return backedIterator.batch()
		End Function

		''' <summary>
		''' Set a pre processor
		''' </summary>
		''' <param name="preProcessor"> a pre processor to set </param>
		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				backedIterator.PreProcessor = preProcessor
			End Set
			Get
				Return backedIterator.PreProcessor
			End Get
		End Property


		''' <summary>
		''' Get dataset iterator record reader labels
		''' </summary>
		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return backedIterator.getLabels()
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
		Public Overrides Function hasNext() As Boolean
			If throwable IsNot Nothing Then
				Throw throwable
			End If

			Try
				If hasDepleted.get() Then
					Return False
				End If

				If nextElement IsNot Nothing AndAlso nextElement IsNot terminator Then
					Return True
				ElseIf nextElement Is terminator Then
					Return False
				End If


				nextElement = buffer.take()

				If nextElement Is terminator Then
					hasDepleted.set(True)
					Return False
				End If

				Return True
			Catch e As Exception
				log.error("Premature end of loop!")
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Returns the next element in the iteration.
		''' </summary>
		''' <returns> the next element in the iteration </returns>
		Public Overrides Function [next]() As DataSet
			If throwable IsNot Nothing Then
				Throw throwable
			End If

			If hasDepleted.get() Then
				Return Nothing
			End If

			Dim temp As DataSet = nextElement
			nextElement = Nothing
			Return temp
		End Function

		''' <summary>
		''' Removes from the underlying collection the last element returned
		''' by this iterator (optional operation).  This method can be called
		''' only once per call to <seealso cref="next"/>.  The behavior of an iterator
		''' is unspecified if the underlying collection is modified while the
		''' iteration is in progress in any way other than by calling this
		''' method.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the {@code remove}
		'''                                       operation is not supported by this iterator </exception>
		''' <exception cref="IllegalStateException">         if the {@code next} method has not
		'''                                       yet been called, or the {@code remove} method has already
		'''                                       been called after the last call to the {@code next}
		'''                                       method
		''' @implSpec The default implementation throws an instance of
		''' <seealso cref="System.NotSupportedException"/> and performs no other action. </exception>
		Public Overrides Sub remove()

		End Sub

		Protected Friend Class AsyncPrefetchThread
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As AsyncDataSetIterator

			Friend queue As BlockingQueue(Of DataSet)
			Friend iterator As DataSetIterator
			Friend terminator As DataSet
			Friend isShutdown As Boolean = False ' locked around `this`
			Friend configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().minSize(10 * 1024L * 1024L).overallocationLimit(outerInstance.prefetchSize + 2).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyLearning(LearningPolicy.FIRST_LOOP).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.REALLOCATE).build()

			Friend workspace As MemoryWorkspace
			Friend ReadOnly deviceId As Integer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected AsyncPrefetchThread(@NonNull BlockingQueue<DataSet> queue, @NonNull DataSetIterator iterator, @NonNull DataSet terminator, org.nd4j.linalg.api.memory.MemoryWorkspace workspace, int deviceId)
			Protected Friend Sub New(ByVal outerInstance As AsyncDataSetIterator, ByVal queue As BlockingQueue(Of DataSet), ByVal iterator As DataSetIterator, ByVal terminator As DataSet, ByVal workspace As MemoryWorkspace, ByVal deviceId As Integer)
				Me.outerInstance = outerInstance
				Me.queue = queue
				Me.iterator = iterator
				Me.terminator = terminator
				Me.deviceId = deviceId

				Me.setDaemon(True)
				Me.setName("ADSI prefetch thread")
			End Sub

			Public Overrides Sub run()
				Nd4j.AffinityManager.unsafeSetDevice(deviceId)
				outerInstance.externalCall()
				Try
					If outerInstance.useWorkspace Then
						workspace = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(configuration, outerInstance.workspaceId)
					End If

					Do While iterator.MoveNext() AndAlso outerInstance.shouldWork.get()
						Dim smth As DataSet = Nothing

						If outerInstance.useWorkspace Then
							Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspace.notifyScopeEntered()
								smth = iterator.Current

								If outerInstance.callback IsNot Nothing Then
									outerInstance.callback.call(smth)
								End If
							End Using
						Else
							smth = iterator.Current

							If outerInstance.callback IsNot Nothing Then
								outerInstance.callback.call(smth)
							End If
						End If

						' we want to ensure underlying iterator finished dataset creation
						Nd4j.Executioner.commit()

						If smth IsNot Nothing Then
							queue.put(smth)
						End If

					Loop
					queue.put(terminator)
				Catch e As InterruptedException
					Thread.CurrentThread.Interrupt()
					' do nothing
					outerInstance.shouldWork.set(False)
				Catch e As Exception
					outerInstance.throwable = e
					Throw New Exception(e)
				Catch e As Exception
					outerInstance.throwable = New Exception(e)
					Throw New Exception(e)
				Finally
					'log.info("Trying destroy...");
					'if (useWorkspace)
					'Nd4j.getWorkspaceManager().getWorkspaceForCurrentThread(workspaceId).destroyWorkspace();
					SyncLock Me
						isShutdown = True
						Monitor.PulseAll(Me)
					End SyncLock
				End Try
			End Sub

			Public Overridable Sub shutdown()
				SyncLock Me
					Do While Not isShutdown
						Try
							Monitor.Wait(Me)
						Catch e As InterruptedException
							Thread.CurrentThread.Interrupt()
							Throw New Exception(e)
						End Try
					Loop
				End SyncLock

				If workspace IsNot Nothing Then
					log.debug("Manually destroying ADSI workspace")
					workspace.destroyWorkspace(True)
				End If
			End Sub
		End Class
	End Class

End Namespace