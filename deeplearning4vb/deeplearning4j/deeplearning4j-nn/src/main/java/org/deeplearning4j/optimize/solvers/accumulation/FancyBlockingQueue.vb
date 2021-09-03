Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ThreadUtils = org.nd4j.common.util.ThreadUtils

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
'ORIGINAL LINE: @Slf4j public class FancyBlockingQueue<E> implements java.util.concurrent.BlockingQueue<E>, Registerable
	Public Class FancyBlockingQueue(Of E)
		Implements BlockingQueue(Of E), Registerable

		Protected Friend backingQueue As BlockingQueue(Of E)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile int consumers;
		Protected Friend consumers As Integer

		Protected Friend currentStep As New ThreadLocal(Of AtomicLong)()
		Protected Friend ReadOnly [step] As New AtomicLong(0)
		Protected Friend ReadOnly state As New AtomicInteger(0)
		Protected Friend ReadOnly currentConsumers As New AtomicInteger(0)

		Protected Friend isFirst As New AtomicBoolean(False)
		Protected Friend isDone As New AtomicBoolean(True)

		Protected Friend barrier As New AtomicInteger(0)
		Protected Friend secondary As New AtomicInteger(0)

		Protected Friend numElementsReady As New AtomicInteger(0)
		Protected Friend numElementsDrained As New AtomicInteger(0)
		Protected Friend bypassMode As New AtomicBoolean(False)

		Protected Friend isDebug As Boolean = False
		Protected Friend lock As New ReentrantReadWriteLock()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FancyBlockingQueue(@NonNull BlockingQueue<E> queue)
		Public Sub New(ByVal queue As BlockingQueue(Of E))
			Me.New(queue, -1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FancyBlockingQueue(@NonNull BlockingQueue<E> queue, int consumers)
		Public Sub New(ByVal queue As BlockingQueue(Of E), ByVal consumers As Integer)
			Me.backingQueue = queue
			Me.consumers = consumers
			Me.currentConsumers.set(consumers)
		End Sub


		Public Overrides Function add(ByVal e As E) As Boolean
			Return backingQueue.add(e)
		End Function

		Public Overrides Function offer(ByVal e As E) As Boolean
			Return backingQueue.offer(e)
		End Function


		Public Overrides Function remove() As E
			Return backingQueue.remove()
		End Function

		Public Overridable Sub fallbackToSingleConsumerMode(ByVal reallyFallback As Boolean) Implements Registerable.fallbackToSingleConsumerMode
			bypassMode.set(reallyFallback)
		End Sub

		Public Overridable Sub registerConsumers(ByVal consumers As Integer) Implements Registerable.registerConsumers
			lock.writeLock().lock()

			Me.numElementsReady.set(backingQueue.size())
			Me.numElementsDrained.set(0)
			Me.consumers = consumers
			Me.currentConsumers.set(consumers)

			lock.writeLock().unlock()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void put(E e) throws InterruptedException
		Public Overrides Sub put(ByVal e As E)
			lock.readLock().lock()
			log.trace("Adding value to the buffer. Current size: [{}]", backingQueue.size())
			backingQueue.put(e)
			lock.readLock().unlock()
		End Sub

		Public Overrides ReadOnly Property Empty As Boolean
			Get
				If bypassMode.get() Then
					Return backingQueue.isEmpty()
				End If
    
    
				Dim res As Boolean = numElementsDrained.get() >= numElementsReady.get()
    
				If isDebug Then
					log.info("thread {} queries isEmpty: {}", Thread.CurrentThread.getId(), res)
				End If
    
    
				Return res
			End Get
		End Property

		Protected Friend Overridable Sub synchronize(ByVal consumers As Integer)
			If consumers = 1 OrElse bypassMode.get() Then
				Return
			End If

			If isDebug Then
				log.info("thread {} locking at FBQ", Thread.CurrentThread.getId())
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
				Loop
			End If

			' second lock here needed only to ensure we won't get overrun over isDone flag
			If secondary.incrementAndGet() = consumers Then
				isFirst.set(True)
			Else
				Do While Not isFirst.get()
					ThreadUtils.uncheckedSleep(1)
				Loop
			End If

			If isDebug Then
				log.info("thread {} unlocking at FBQ", Thread.CurrentThread.getId())
			End If

		End Sub

		Public Overrides Function poll() As E
			If bypassMode.get() Then
				Return backingQueue.poll()
			End If

			' if that's first step, set local step counter to -1
			If currentStep.get() Is Nothing Then
				currentStep.set(New AtomicLong(-1))
			End If

			' we block until everyone else step forward
			Do While [step].get() = currentStep.get().get()
				ThreadUtils.uncheckedSleep(1)
			Loop

			Dim [object] As E = peek()

			' we wait until all consumers peek() this object from queue
			synchronize(currentConsumers.get())

			currentStep.get().incrementAndGet()


			' last consumer shifts queue on step further
			If state.incrementAndGet() = currentConsumers.get() Then

				' we're removing current head of queue
				remove()

				numElementsDrained.incrementAndGet()

				' and moving step counter further
				state.set(0)
				[step].incrementAndGet()
			End If

			' we wait until all consumers know that queue is updated (for isEmpty())
			synchronize(currentConsumers.get())
			'log.info("Second lock passed");

			' now, every consumer in separate threads will get it's own copy of CURRENT head of the queue
			Return [object]
		End Function

		Public Overrides Function element() As E
			Return backingQueue.element()
		End Function

		Public Overrides Sub clear()
			backingQueue.clear()
			[step].set(0)
		End Sub

		Public Overrides Function size() As Integer
			Return backingQueue.size()
		End Function

		Public Overrides Function peek() As E
			Return backingQueue.peek()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public boolean offer(E e, long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
		Public Overrides Function offer(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean
			Return backingQueue.offer(e, timeout, unit)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public E take() throws InterruptedException
		Public Overrides Function take() As E
			Return Nothing
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public E poll(long timeout, java.util.concurrent.TimeUnit unit) throws InterruptedException
		Public Overrides Function poll(ByVal timeout As Long, ByVal unit As TimeUnit) As E
			Return backingQueue.poll(timeout, unit)
		End Function


		Public Overrides Function remainingCapacity() As Integer
			Return backingQueue.remainingCapacity()
		End Function

		Public Overrides Function remove(ByVal o As Object) As Boolean
			Return backingQueue.remove(o)
		End Function

		Public Overrides Function containsAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Return backingQueue.containsAll(c)
		End Function

		Public Overrides Function addAll(Of T1 As E)(ByVal c As ICollection(Of T1)) As Boolean
			Return backingQueue.addAll(c)
		End Function

		Public Overrides Function removeAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Return backingQueue.removeAll(c)
		End Function

		Public Overrides Function retainAll(Of T1)(ByVal c As ICollection(Of T1)) As Boolean
			Return backingQueue.retainAll(c)
		End Function

		Public Overrides Function contains(ByVal o As Object) As Boolean
			Return backingQueue.contains(o)
		End Function


		Public Overrides Function iterator() As IEnumerator(Of E)
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function toArray() As Object()
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function toArray(Of T)(ByVal a() As T) As T()
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: @Override public int drainTo(java.util.Collection<? super E> c)
		Public Overrides Function drainTo(Of T1)(ByVal c As ICollection(Of T1)) As Integer
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: @Override public int drainTo(java.util.Collection<? super E> c, int maxElements)
		Public Overrides Function drainTo(Of T1)(ByVal c As ICollection(Of T1), ByVal maxElements As Integer) As Integer
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace