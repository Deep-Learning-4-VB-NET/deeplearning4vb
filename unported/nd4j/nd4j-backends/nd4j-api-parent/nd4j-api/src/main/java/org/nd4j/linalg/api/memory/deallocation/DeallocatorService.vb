Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Deallocatable = org.nd4j.linalg.api.memory.Deallocatable
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

Namespace org.nd4j.linalg.api.memory.deallocation



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DeallocatorService
	Public Class DeallocatorService
		Private deallocatorThreads() As Thread
		Private queues() As ReferenceQueue(Of Deallocatable)
		Private referenceMap As IDictionary(Of String, DeallocatableReference) = New ConcurrentDictionary(Of String, DeallocatableReference)()
		Private deviceMap As IList(Of IList(Of ReferenceQueue(Of Deallocatable))) = New List(Of IList(Of ReferenceQueue(Of Deallocatable)))()

		<NonSerialized>
		Private ReadOnly counter As New AtomicLong(0)

		Public Sub New()
			' we need to have at least 2 threads, but for CUDA we'd need at least numDevices threads, due to thread->device affinity
			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices
			Dim numThreads As Integer = Math.Max(2, numDevices * 2)

			For e As Integer = 0 To numDevices - 1
				deviceMap.Add(New List(Of ReferenceQueue(Of Deallocatable))())
			Next e

			deallocatorThreads = New Thread(numThreads - 1){}
			queues = New ReferenceQueue(numThreads - 1){}
			For e As Integer = 0 To numThreads - 1
				log.trace("Starting deallocator thread {}", e + 1)
				queues(e) = New ReferenceQueue(Of Deallocatable)()

				Dim deviceId As Integer = e Mod numDevices
				' attaching queue to its own thread
				deallocatorThreads(e) = New DeallocatorServiceThread(Me, queues(e), e, deviceId)
				deallocatorThreads(e).setName("DeallocatorServiceThread_" & e)
				deallocatorThreads(e).setDaemon(True)

				deviceMap(deviceId).Add(queues(e))

				deallocatorThreads(e).Start()
			Next e
		End Sub

		Public Overridable Function nextValue() As Long
			Return counter.incrementAndGet()
		End Function

		''' <summary>
		''' This method adds Deallocatable object instance to tracking system
		''' </summary>
		''' <param name="deallocatable"> object to track </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void pickObject(@NonNull Deallocatable deallocatable)
		Public Overridable Sub pickObject(ByVal deallocatable As Deallocatable)
			Dim desiredDevice As val = deallocatable.targetDevice()
			Dim map As val = deviceMap(desiredDevice)
			Dim reference As val = New DeallocatableReference(deallocatable, map.get(RandomUtils.nextInt(0, map.size())))
			referenceMap(deallocatable.getUniqueId()) = reference
		End Sub


		Private Class DeallocatorServiceThread
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As DeallocatorService

			Friend ReadOnly queue As ReferenceQueue(Of Deallocatable)
			Friend ReadOnly threadIdx As Integer
			Public Const DeallocatorThreadNamePrefix As String = "DeallocatorServiceThread thread "
			Friend ReadOnly deviceId As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private DeallocatorServiceThread(@NonNull ReferenceQueue<org.nd4j.linalg.api.memory.Deallocatable> queue, int threadIdx, int deviceId)
			Friend Sub New(ByVal outerInstance As DeallocatorService, ByVal queue As ReferenceQueue(Of Deallocatable), ByVal threadIdx As Integer, ByVal deviceId As Integer)
				Me.outerInstance = outerInstance
				Me.queue = queue
				Me.threadIdx = threadIdx
				Me.setName(DeallocatorThreadNamePrefix & threadIdx)
				Me.deviceId = deviceId
				setContextClassLoader(Nothing)
			End Sub

			Public Overrides Sub run()
				Nd4j.AffinityManager.unsafeSetDevice(deviceId)
				Dim canRun As Boolean = True
				Dim cnt As Long = 0
				Do While canRun
					' if periodicGc is enabled, only first thread will call for it
					If Nd4j.MemoryManager.PeriodicGcActive AndAlso threadIdx = 0 AndAlso Nd4j.MemoryManager.AutoGcWindow > 0 Then
						Dim reference As val = CType(queue.poll(), DeallocatableReference)
						If reference Is Nothing Then
							Dim timeout As val = Nd4j.MemoryManager.AutoGcWindow
							Try
								Thread.Sleep(Nd4j.MemoryManager.AutoGcWindow)
								Nd4j.MemoryManager.invokeGc()
							Catch e As InterruptedException
								canRun = False
							End Try
						Else
							' invoking deallocator
							reference.getDeallocator().deallocate()
							outerInstance.referenceMap.Remove(reference.getId())
						End If
					Else
						Try
							Dim reference As val = CType(queue.remove(), DeallocatableReference)
							If reference Is Nothing Then
								Continue Do
							End If

							' invoking deallocator
							reference.getDeallocator().deallocate()
							outerInstance.referenceMap.Remove(reference.getId())
						Catch e As InterruptedException
							canRun = False
						Catch e As Exception
							Throw New Exception(e)
						End Try
					End If
				Loop
			End Sub
		End Class
	End Class

End Namespace