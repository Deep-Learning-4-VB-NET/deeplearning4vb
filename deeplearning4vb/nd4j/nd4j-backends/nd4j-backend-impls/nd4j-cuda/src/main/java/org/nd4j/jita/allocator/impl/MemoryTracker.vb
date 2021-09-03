Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

' ******************************************************************************
' *
' *
' * This program and the accompanying materials are made available under the
' * terms of the Apache License, Version 2.0 which is available at
' * https://www.apache.org/licenses/LICENSE-2.0.
' *
' *  See the NOTICE file distributed with this work for additional
' *  information regarding copyright ownership.
' * Unless required by applicable law or agreed to in writing, software
' * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' * License for the specific language governing permissions and limitations
' * under the License.
' *
' * SPDX-License-Identifier: Apache-2.0
' *****************************************************************************

Namespace org.nd4j.jita.allocator.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class MemoryTracker
	Public Class MemoryTracker

		Private allocatedPerDevice As IList(Of AtomicLong) = New List(Of AtomicLong)()
		Private cachedPerDevice As IList(Of AtomicLong) = New List(Of AtomicLong)()
		Private totalPerDevice As IList(Of AtomicLong) = New List(Of AtomicLong)()
		Private freePerDevice As IList(Of AtomicLong) = New List(Of AtomicLong)()
		Private workspacesPerDevice As IList(Of AtomicLong) = New List(Of AtomicLong)()
		Private cachedHost As New AtomicLong(0)
		Private allocatedHost As New AtomicLong(0)
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New MemoryTracker()

		Public Sub New()
			Dim i As Integer = 0
			Do While i < Nd4j.AffinityManager.NumberOfDevices
				allocatedPerDevice.Insert(i, New AtomicLong(0))
				cachedPerDevice.Insert(i, New AtomicLong(0))
				workspacesPerDevice.Insert(i, New AtomicLong(0))

				totalPerDevice.Insert(i, New AtomicLong(NativeOpsHolder.Instance.getDeviceNativeOps().getDeviceTotalMemory(i)))

				Dim f As val = New AtomicLong(NativeOpsHolder.Instance.getDeviceNativeOps().getDeviceFreeMemory(i))

				'log.debug("Free memory on device_{}: {}", i, f);
				freePerDevice.Insert(i, f)
				i += 1
			Loop
		End Sub

		Public Shared ReadOnly Property Instance As MemoryTracker
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overridable Function getAllocatedAmount(ByVal deviceId As Integer) As Long
			Return allocatedPerDevice(deviceId).get()
		End Function

		Public Overridable Function getCachedAmount(ByVal deviceId As Integer) As Long
			Return cachedPerDevice(deviceId).get()
		End Function

		''' <summary>
		''' This method returns number of bytes currently cached from host memory
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property CachedHostAmount As Long
			Get
				Return cachedHost.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of bytes currently allocated from host memory
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AllocatedHostAmount As Long
			Get
				Return allocatedHost.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of bytes allocated and cached in host ram
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ActiveHostAmount As Long
			Get
				Return AllocatedHostAmount + CachedHostAmount
			End Get
		End Property

		Public Overridable Sub incrementCachedHostAmount(ByVal numBytes As Long)
			cachedHost.addAndGet(numBytes)
		End Sub

		Public Overridable Sub incrementAllocatedHostAmount(ByVal numBytes As Long)
			allocatedHost.addAndGet(numBytes)
		End Sub

		Public Overridable Sub decrementCachedHostAmount(ByVal numBytes As Long)
			cachedHost.addAndGet(-numBytes)
		End Sub

		Public Overridable Sub decrementAllocatedHostAmount(ByVal numBytes As Long)
			allocatedHost.addAndGet(-numBytes)
		End Sub

		Public Overridable Function getWorkspaceAllocatedAmount(ByVal deviceId As Integer) As Long
			Return workspacesPerDevice(deviceId).get()
		End Function

		Public Overridable Function getTotalMemory(ByVal deviceId As Integer) As Long
			Return totalPerDevice(deviceId).get()
		End Function

		Public Overridable Function getFreeMemory(ByVal deviceId As Integer) As Long
			Return freePerDevice(deviceId).get()
		End Function

		''' <summary>
		''' This method returns approximate free memory on specified device </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getApproximateFreeMemory(ByVal deviceId As Integer) As Long
			Dim externalAllocations As val = getTotalMemory(deviceId) - getFreeMemory(deviceId)
			Dim active As val = getActiveMemory(deviceId)
			Dim free As val = getTotalMemory(deviceId) - (active + externalAllocations)
			Return free
		End Function

		''' <summary>
		''' This method returns precise amount of free memory on specified device </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getPreciseFreeMemory(ByVal deviceId As Integer) As Long
			' we refresh free memory on device
			Dim extFree As val = NativeOpsHolder.Instance.getDeviceNativeOps().getDeviceFreeMemory(deviceId)
			'freePerDevice.get(deviceId).set(extFree);

			Return extFree
		End Function

		''' <summary>
		''' This method returns delta between total memory and free memory </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getUsableMemory(ByVal deviceId As Integer) As Long
			Return getTotalMemory(deviceId) - getFreeMemory(deviceId)
		End Function

		''' <summary>
		''' This method returns total amount of device memory allocated on specified device
		''' 
		''' Includes: workspace memory, cached memory, regular memory </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getActiveMemory(ByVal deviceId As Integer) As Long
			Return getWorkspaceAllocatedAmount(deviceId) + getAllocatedAmount(deviceId) + getCachedAmount(deviceId)
		End Function

		''' <summary>
		''' This method returns amount of memory that relies on JVM GC
		''' 
		''' Includes: cached memory, regular allocated memory
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getManagedMemory(ByVal deviceId As Integer) As Long
			Return getAllocatedAmount(deviceId) + getCachedAmount(deviceId)
		End Function

		''' <summary>
		''' This method increments amount of regular allocated memory
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="memoryAdded"> </param>
		Public Overridable Sub incrementAllocatedAmount(ByVal deviceId As Integer, ByVal memoryAdded As Long)
			allocatedPerDevice(deviceId).getAndAdd(matchBlock(memoryAdded))
		End Sub

		''' <summary>
		''' This method increments amount of cached memory
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="memoryAdded"> </param>
		Public Overridable Sub incrementCachedAmount(ByVal deviceId As Integer, ByVal memoryAdded As Long)
			cachedPerDevice(deviceId).getAndAdd(matchBlock(memoryAdded))
		End Sub

		''' <summary>
		''' This method decrements amount of regular allocated memory
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="memorySubtracted"> </param>
		Public Overridable Sub decrementAllocatedAmount(ByVal deviceId As Integer, ByVal memorySubtracted As Long)
			allocatedPerDevice(deviceId).getAndAdd(-matchBlock(memorySubtracted))
		End Sub

		''' <summary>
		''' This method decrements amount of cached memory
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="memorySubtracted"> </param>
		Public Overridable Sub decrementCachedAmount(ByVal deviceId As Integer, ByVal memorySubtracted As Long)
			cachedPerDevice(deviceId).getAndAdd(-matchBlock(memorySubtracted))
		End Sub

		''' <summary>
		''' This method increments amount of memory allocated within workspaces
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="memoryAdded"> </param>
		Public Overridable Sub incrementWorkspaceAllocatedAmount(ByVal deviceId As Integer, ByVal memoryAdded As Long)
			workspacesPerDevice(deviceId).getAndAdd(matchBlock(memoryAdded))
		End Sub

		''' <summary>
		''' This method decrements amount of memory allocated within workspaces
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="memorySubtracted"> </param>
		Public Overridable Sub decrementWorkspaceAmount(ByVal deviceId As Integer, ByVal memorySubtracted As Long)
			workspacesPerDevice(deviceId).getAndAdd(-matchBlock(memorySubtracted))
		End Sub


		Private Sub setTotalPerDevice(ByVal device As Integer, ByVal memoryAvailable As Long)
			totalPerDevice.Insert(device, New AtomicLong(memoryAvailable))
		End Sub


		Private Function matchBlock(ByVal numBytes As Long) As Long
			'int align = 65536 * 2;
			'return numBytes + (align - (numBytes % align));
			Return numBytes
		End Function
	End Class

End Namespace