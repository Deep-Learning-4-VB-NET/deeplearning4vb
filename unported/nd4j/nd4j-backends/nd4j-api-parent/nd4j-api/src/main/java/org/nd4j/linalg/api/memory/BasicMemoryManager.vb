Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DummyWorkspace = org.nd4j.linalg.api.memory.abstracts.DummyWorkspace

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

Namespace org.nd4j.linalg.api.memory


	Public MustInherit Class BasicMemoryManager
		Implements MemoryManager

		Public MustOverride Function allocatedMemory(ByVal deviceId As Integer?) As Long Implements MemoryManager.allocatedMemory
		Public MustOverride ReadOnly Property BandwidthUse As IDictionary(Of Integer, Long) Implements MemoryManager.getBandwidthUse
		Public MustOverride Sub memset(ByVal array As INDArray) Implements MemoryManager.memset
		Public MustOverride Sub release(ByVal pointer As Pointer, ByVal kind As enums.MemoryKind)
		Protected Friend frequency As New AtomicInteger(0)
		Protected Friend freqCounter As New AtomicLong(0)

'JAVA TO VB CONVERTER NOTE: The field lastGcTime was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend lastGcTime_Conflict As New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis())

		Protected Friend periodicEnabled As New AtomicBoolean(False)

'JAVA TO VB CONVERTER NOTE: The field averageLoopTime was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend averageLoopTime_Conflict As New AtomicInteger(0)

		Protected Friend noGcWindow As New AtomicInteger(100)

		Protected Friend averagingEnabled As New AtomicBoolean(False)

		Protected Friend Const intervalTail As Integer = 100

		Protected Friend intervals As LinkedList(Of Integer) = New ConcurrentLinkedQueue(Of Integer)()

		Private workspace As New ThreadLocal(Of MemoryWorkspace)()

		Private tempWorkspace As New ThreadLocal(Of MemoryWorkspace)()

		''' <summary>
		''' This method returns
		''' PLEASE NOTE: Cache options
		''' depend on specific implementations
		''' </summary>
		''' <param name="bytes"> </param>
		''' <param name="kind"> </param>
		''' <param name="initialize"> </param>
		Public Overridable Function allocate(ByVal bytes As Long, ByVal kind As MemoryKind, ByVal initialize As Boolean) As Pointer Implements MemoryManager.allocate
			Throw New System.NotSupportedException("This method isn't available for this backend")
		End Function

		''' <summary>
		''' This method detaches off-heap memory from passed INDArray instances, and optionally stores them in cache for future reuse
		''' PLEASE NOTE: Cache options depend on specific implementations
		''' </summary>
		''' <param name="arrays"> </param>
		Public Overridable Sub collect(ParamArray ByVal arrays() As INDArray) Implements MemoryManager.collect
			Throw New System.NotSupportedException("This method isn't implemented yet")
		End Sub

		Public Overridable Sub toggleAveraging(ByVal enabled As Boolean) Implements MemoryManager.toggleAveraging
			averagingEnabled.set(enabled)
		End Sub

		''' <summary>
		''' This method purges all cached memory chunks
		''' 
		''' </summary>
		Public Overridable Sub purgeCaches() Implements MemoryManager.purgeCaches
			'No op for CPU (no cache)
		End Sub

		Public Overridable Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer) Implements MemoryManager.memcpy
			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			Pointer.memcpy(dstBuffer.addressPointer(), srcBuffer.addressPointer(), srcBuffer.length() * srcBuffer.ElementSize)

			PerformanceTracker.Instance.helperRegisterTransaction(0, perfD, srcBuffer.length() * srcBuffer.ElementSize, MemcpyDirection.HOST_TO_HOST)
		End Sub

		Public Overridable Sub notifyScopeEntered() Implements MemoryManager.notifyScopeEntered
			' TODO: to be implemented
		End Sub

		Public Overridable Sub notifyScopeLeft() Implements MemoryManager.notifyScopeLeft
			' TODO: to be implemented
		End Sub

		Public Overridable Sub invokeGcOccasionally() Implements MemoryManager.invokeGcOccasionally
			Dim currentTime As Long = DateTimeHelper.CurrentUnixTimeMillis()

			If averagingEnabled.get() Then
				intervals.AddLast(CInt(Math.Truncate(currentTime - lastGcTime_Conflict.get())))
			End If

			' not sure if we want to conform autoGcWindow here...
			If frequency.get() > 0 Then
				If freqCounter.incrementAndGet() Mod frequency.get() = 0 AndAlso currentTime > LastGcTime + AutoGcWindow Then
					System.GC.Collect()
					lastGcTime_Conflict.set(DateTimeHelper.CurrentUnixTimeMillis())
				End If
			End If

			If averagingEnabled.get() Then
				If intervals.Count > intervalTail Then
					intervals.RemoveFirst()
				End If
			End If
		End Sub

		Public Overridable Sub invokeGc() Implements MemoryManager.invokeGc
			System.GC.Collect()
			lastGcTime_Conflict.set(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		Public Overridable ReadOnly Property PeriodicGcActive As Boolean Implements MemoryManager.isPeriodicGcActive
			Get
				Return periodicEnabled.get()
			End Get
		End Property

		Public Overridable Property OccasionalGcFrequency Implements MemoryManager.setOccasionalGcFrequency As Integer
			Set(ByVal frequency As Integer)
				Me.frequency.set(frequency)
			End Set
			Get
				Return frequency.get()
			End Get
		End Property

		Public Overridable Property AutoGcWindow Implements MemoryManager.setAutoGcWindow As Integer
			Set(ByVal windowMillis As Integer)
				noGcWindow.set(windowMillis)
			End Set
			Get
				Return noGcWindow.get()
			End Get
		End Property



		Public Overridable ReadOnly Property LastGcTime As Long Implements MemoryManager.getLastGcTime
			Get
				Return lastGcTime_Conflict.get()
			End Get
		End Property

		Public Overridable Sub togglePeriodicGc(ByVal enabled As Boolean) Implements MemoryManager.togglePeriodicGc
			periodicEnabled.set(enabled)
		End Sub

		Public Overridable ReadOnly Property AverageLoopTime As Integer Implements MemoryManager.getAverageLoopTime
			Get
				If averagingEnabled.get() Then
					Dim cnt As Integer = 0
					For Each value As Integer? In intervals
						cnt += value
					Next value
					cnt \= intervals.Count
					Return cnt
				Else
					Return 0
				End If
    
			End Get
		End Property

		Public Overridable Property CurrentWorkspace As MemoryWorkspace Implements MemoryManager.getCurrentWorkspace
			Get
				Return workspace.get()
			End Get
			Set(ByVal workspace As MemoryWorkspace)
				Me.workspace.set(workspace)
			End Set
		End Property



		Public Overridable Function scopeOutOfWorkspaces() As MemoryWorkspace Implements MemoryManager.scopeOutOfWorkspaces
			Dim workspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			If workspace Is Nothing Then
				Return New DummyWorkspace()
			Else
				'Nd4j.getMemoryManager().setCurrentWorkspace(null);
				Return (New DummyWorkspace()).notifyScopeEntered() 'workspace.tagOutOfScopeUse();
			End If
		End Function

		Public Overridable Sub releaseCurrentContext() Implements MemoryManager.releaseCurrentContext
			' no-op
		End Sub
	End Class

End Namespace