Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Configuration = org.nd4j.jita.conf.Configuration
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.jita.allocator.concurrency


	''' 
	''' 
	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class DeviceAllocationsTracker
		Private configuration As Configuration

		Private ReadOnly globalLock As New ReentrantReadWriteLock()

		Private ReadOnly deviceLocks As IDictionary(Of Integer, ReentrantReadWriteLock) = New ConcurrentDictionary(Of Integer, ReentrantReadWriteLock)()

		Private ReadOnly memoryTackled As IDictionary(Of Integer, AtomicLong) = New ConcurrentDictionary(Of Integer, AtomicLong)()

		Private ReadOnly reservedSpace As IDictionary(Of Integer, AtomicLong) = New ConcurrentDictionary(Of Integer, AtomicLong)()

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(DeviceAllocationsTracker))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DeviceAllocationsTracker(@NonNull Configuration configuration)
		Public Sub New(ByVal configuration As Configuration)
			Me.configuration = configuration

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices

			For device As Integer = 0 To numDevices - 1
				deviceLocks(device) = New ReentrantReadWriteLock()
			Next device
		End Sub

		Protected Friend Overridable Sub ensureThreadRegistered(ByVal threadId As Long?, ByVal deviceId As Integer?)
			globalLock.readLock().lock()

			'  boolean contains = allocationTable.contains(deviceId, threadId);

			globalLock.readLock().unlock()

			If Not memoryTackled.ContainsKey(deviceId) Then
				globalLock.writeLock().lock()

				'contains = allocationTable.contains(deviceId, threadId);
				'if (!contains) {
				'allocationTable.put(deviceId, threadId, new AtomicLong(0));

				If Not memoryTackled.ContainsKey(deviceId) Then
					memoryTackled(deviceId) = New AtomicLong(0)
				End If

				If Not reservedSpace.ContainsKey(deviceId) Then
					reservedSpace(deviceId) = New AtomicLong(0)
				End If
				'}
				globalLock.writeLock().unlock()
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public long addToAllocation(@NonNull Long threadId, System.Nullable<Integer> deviceId, long memorySize)
		Public Overridable Function addToAllocation(ByVal threadId As Long, ByVal deviceId As Integer?, ByVal memorySize As Long) As Long
			ensureThreadRegistered(threadId, deviceId)
			Try
				deviceLocks(deviceId).readLock().lock()

				Dim res As Long = memoryTackled(deviceId).addAndGet(memorySize)

				subFromReservedSpace(deviceId, memorySize)

				Return res 'allocationTable.get(deviceId, threadId).addAndGet(memorySize);
			Finally
				deviceLocks(deviceId).readLock().unlock()
			End Try
		End Function

		Public Overridable Function subFromAllocation(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal memorySize As Long) As Long
			ensureThreadRegistered(threadId, deviceId)

			Try
				deviceLocks(deviceId).writeLock().lock()

				Dim val2 As AtomicLong = memoryTackled(deviceId)
				'long before = val2.get();
				val2.addAndGet(memorySize * -1)

				'long after = memoryTackled.get(deviceId).get();

				'log.info("Memory reduction on device [{}], memory size: [{}], before: [{}], after [{}]", deviceId, memorySize, before, after);

				'            AtomicLong val = allocationTable.get(deviceId, threadId);

				'            val.addAndGet(memorySize * -1);

				Return val2.get()
			Finally
				deviceLocks(deviceId).writeLock().unlock()
			End Try
		End Function

		''' <summary>
		''' This method "reserves" memory within allocator
		''' </summary>
		''' <param name="threadId"> </param>
		''' <param name="deviceId"> </param>
		''' <param name="memorySize">
		''' @return </param>
		Public Overridable Function reserveAllocationIfPossible(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal memorySize As Long) As Boolean
			ensureThreadRegistered(threadId, deviceId)
			Try
				deviceLocks(deviceId).writeLock().lock()
	'            
	'            if (getAllocatedSize(deviceId) + memorySize + getReservedSpace(deviceId)> environment.getDeviceInformation(deviceId).getTotalMemory() * configuration.getMaxDeviceMemoryUsed()) {
	'                return false;
	'            } else {
	'                addToReservedSpace(deviceId, memorySize);
	'                return true;
	'            }
	'            
				addToReservedSpace(deviceId, memorySize)
				Return True
			Finally
				deviceLocks(deviceId).writeLock().unlock()
			End Try
		End Function

		Public Overridable Function getAllocatedSize(ByVal threadId As Long?, ByVal deviceId As Integer?) As Long
			ensureThreadRegistered(threadId, deviceId)

			Try
				deviceLocks(deviceId).readLock().lock()

				Return getAllocatedSize(deviceId) '/ allocationTable.get(deviceId, threadId).get();
			Finally
				deviceLocks(deviceId).readLock().unlock()
			End Try
		End Function


		Public Overridable Function getAllocatedSize(ByVal deviceId As Integer?) As Long
			If Not memoryTackled.ContainsKey(deviceId) Then
				Return 0L
			End If
			Try
				deviceLocks(deviceId).readLock().lock()
				Return memoryTackled(deviceId).get()
			Finally
				deviceLocks(deviceId).readLock().unlock()
			End Try
		End Function

		Public Overridable Function getReservedSpace(ByVal deviceId As Integer?) As Long
			Return reservedSpace(deviceId).get()
		End Function

		Protected Friend Overridable Sub addToReservedSpace(ByVal deviceId As Integer?, ByVal memorySize As Long)
			ensureThreadRegistered(Thread.CurrentThread.getId(), deviceId)

			reservedSpace(deviceId).addAndGet(memorySize)
		End Sub

		Protected Friend Overridable Sub subFromReservedSpace(ByVal deviceId As Integer?, ByVal memorySize As Long)
			ensureThreadRegistered(Thread.CurrentThread.getId(), deviceId)

			reservedSpace(deviceId).addAndGet(memorySize * -1)
		End Sub
	End Class

End Namespace