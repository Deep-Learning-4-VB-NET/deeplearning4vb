Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BaseCudaDataBuffer = org.nd4j.linalg.jcublas.buffer.BaseCudaDataBuffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports BasicMemoryManager = org.nd4j.linalg.api.memory.BasicMemoryManager
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.nd4j.jita.memory


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudaMemoryManager extends org.nd4j.linalg.api.memory.BasicMemoryManager
	Public Class CudaMemoryManager
		Inherits BasicMemoryManager

		Public Overrides Function allocate(ByVal bytes As Long, ByVal kind As MemoryKind, ByVal initialize As Boolean) As Pointer
			Dim allocator As val = AtomicAllocator.Instance

			'log.info("Allocating {} bytes in {} memory...", bytes, kind);

			If kind = MemoryKind.HOST Then
				Dim ptr As val = NativeOpsHolder.Instance.getDeviceNativeOps().mallocHost(bytes, 0)

				If ptr Is Nothing Then
					Throw New Exception("Failed to allocate " & bytes & " bytes from HOST memory")
				End If

				If initialize Then
					Pointer.memset(ptr, 0, bytes)
				End If

				Return ptr 'allocator.getMemoryHandler().alloc(AllocationStatus.HOST, null, null, initialize).getHostPointer();
			ElseIf kind = MemoryKind.DEVICE Then
				Dim ptr As val = NativeOpsHolder.Instance.getDeviceNativeOps().mallocDevice(bytes, 0, 0)
				log.trace("Allocating {} bytes for device_{}", bytes, Nd4j.AffinityManager.getDeviceForCurrentThread())

				Dim ec As val = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorCode()
				If ec <> 0 Then
					Dim em As val = NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorMessage()
					Throw New Exception(em & "; Bytes: [" & bytes & "]; Error code [" & ec & "]; DEVICE [" & Nd4j.AffinityManager.getDeviceForCurrentThread().Value & "]")
				End If

				If ptr Is Nothing Then
					Throw New Exception("Failed to allocate " & bytes & " bytes from DEVICE [" & Nd4j.AffinityManager.getDeviceForCurrentThread() & "] memory")
				End If

				If initialize Then
					Dim context As val = AtomicAllocator.Instance.DeviceContext

					Dim i As Integer = NativeOpsHolder.Instance.getDeviceNativeOps().memsetAsync(ptr, 0, bytes, 0, context.getSpecialStream())
					If i = 0 Then
						Throw New ND4JIllegalStateException("memset failed on device_" & Nd4j.AffinityManager.getDeviceForCurrentThread())
					End If

					context.getSpecialStream().synchronize()
				End If


				Return ptr 'allocator.getMemoryHandler().alloc(AllocationStatus.HOST, null, null, initialize).getDevicePointer();
			Else
				Throw New Exception("Unknown MemoryKind requested: " & kind)
			End If
		End Function

		''' <summary>
		''' This method detaches off-heap memory from passed INDArray instances, and optionally stores them in cache for future reuse
		''' PLEASE NOTE: Cache options depend on specific implementations
		''' </summary>
		''' <param name="arrays"> </param>
		Public Overrides Sub collect(ParamArray ByVal arrays() As INDArray)
			' we basically want to free memory, without touching INDArray itself.
			' so we don't care when gc is going to release object: memory is already cached

			Nd4j.Executioner.commit()

			Dim cnt As Integer = -1
			Dim allocator As AtomicAllocator = AtomicAllocator.Instance
			For Each array As INDArray In arrays
				cnt += 1
				' we don't collect views, since they don't have their own memory
				If array Is Nothing OrElse array.View Then
					Continue For
				End If

				Dim point As AllocationPoint = allocator.getAllocationPoint(array)

				If point.getAllocationStatus() = AllocationStatus.HOST Then
					allocator.MemoryHandler.free(point, AllocationStatus.HOST)
				ElseIf point.getAllocationStatus() = AllocationStatus.DEVICE Then
					allocator.MemoryHandler.free(point, AllocationStatus.DEVICE)
					allocator.MemoryHandler.free(point, AllocationStatus.HOST)
				ElseIf point.getAllocationStatus() = AllocationStatus.DEALLOCATED Then
					' do nothing
				Else
					Throw New Exception("Unknown AllocationStatus: " & point.getAllocationStatus() & " for argument: " & cnt)
				End If

				point.setAllocationStatus(AllocationStatus.DEALLOCATED)
			Next array
		End Sub

		''' <summary>
		''' This method purges all cached memory chunks
		''' PLEASE NOTE: This method SHOULD NOT EVER BE USED without being 146% clear of all consequences.
		''' </summary>
		Public Overrides Sub purgeCaches()
			SyncLock Me
				' reset device cache offset
				'        Nd4j.getConstantHandler().purgeConstants();
        
				' reset TADs
				'        ((CudaGridExecutioner) Nd4j.getExecutioner()).getTadManager().purgeBuffers();
        
				' purge shapes
				'        Nd4j.getShapeInfoProvider().purgeCache();
        
				' purge memory cache
				'AtomicAllocator.getInstance().getMemoryHandler().getMemoryProvider().purgeCache();
        
			End SyncLock
		End Sub

		Protected Friend Overridable Sub allocateHostPointers(ParamArray ByVal dataBuffers() As DataBuffer)
			For Each v As val In dataBuffers
				If v IsNot Nothing AndAlso TypeOf v Is BaseCudaDataBuffer Then
					CType(v, BaseCudaDataBuffer).lazyAllocateHostPointer()
				End If
			Next v
		End Sub

		''' <summary>
		''' This method provides basic memcpy functionality with respect to target environment
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcBuffer"> </param>
		Public Overrides Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer)
			Dim context As val = AtomicAllocator.Instance.DeviceContext


			If TypeOf dstBuffer Is CompressedDataBuffer AndAlso Not (TypeOf srcBuffer Is CompressedDataBuffer) Then
				' destination is compressed, source isn't
				Dim srcPoint As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(srcBuffer)

				allocateHostPointers(dstBuffer, srcBuffer)

				Dim size As Long = srcBuffer.ElementSize * srcBuffer.length()
				If Not srcPoint.ActualOnHostSide Then
					' copying device -> host

					AtomicAllocator.Instance.synchronizeHostData(srcBuffer)

					' Pointer src = AtomicAllocator.getInstance().getPointer(srcBuffer, context);

					' NativeOpsHolder.getInstance().getDeviceNativeOps().memcpyAsync(dstBuffer.addressPointer(), src, size, 2, context.getSpecialStream());
					' context.syncSpecialStream();

				End If ' else {
				  ' copying host -> host
				Dim src As val = AtomicAllocator.Instance.getHostPointer(srcBuffer)

				Pointer.memcpy(dstBuffer.addressPointer(), src, size)
				' }

			ElseIf Not (TypeOf dstBuffer Is CompressedDataBuffer) AndAlso TypeOf srcBuffer Is CompressedDataBuffer Then
				allocateHostPointers(dstBuffer, srcBuffer)

				' destination is NOT compressed, source is compressed
				Dim dstPoint As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(dstBuffer)
				Dim size As Long = srcBuffer.ElementSize * srcBuffer.length()

				Pointer.memcpy(dstBuffer.addressPointer(), srcBuffer.addressPointer(), size)
				dstPoint.tickHostWrite()

			ElseIf TypeOf dstBuffer Is CompressedDataBuffer AndAlso TypeOf srcBuffer Is CompressedDataBuffer Then
				' both buffers are compressed, just fire memcpy

				allocateHostPointers(dstBuffer, srcBuffer)

				Pointer.memcpy(dstBuffer.addressPointer(), srcBuffer.addressPointer(), srcBuffer.length() * srcBuffer.ElementSize)
			Else
				' both buffers are NOT compressed
				AtomicAllocator.Instance.memcpy(dstBuffer, srcBuffer)
			End If
		End Sub

		''' <summary>
		''' This method releases previously allocated memory chunk
		''' </summary>
		''' <param name="pointer"> </param>
		''' <param name="kind">
		''' @return </param>
		Public Overrides Sub release(ByVal pointer As Pointer, ByVal kind As MemoryKind)
			If kind = MemoryKind.DEVICE Then
				NativeOpsHolder.Instance.getDeviceNativeOps().freeDevice(pointer, 0)
				pointer.setNull()
			ElseIf kind = MemoryKind.HOST Then
				NativeOpsHolder.Instance.getDeviceNativeOps().freeHost(pointer)
				pointer.setNull()
			End If
		End Sub

		Public Overrides WriteOnly Property AutoGcWindow As Integer
			Set(ByVal windowMillis As Integer)
				MyBase.AutoGcWindow = windowMillis
				CudaEnvironment.Instance.Configuration.NoGcWindowMs = windowMillis
			End Set
		End Property

		Public Overrides Sub memset(ByVal array As INDArray)
			If array.View Then
				array.assign(0.0)

				' we don't want any mGRID activations here
				Nd4j.Executioner.commit()
				Return
			End If

			' we want to be sure we have no trails left in mGRID
			Nd4j.Executioner.push()

			Dim point As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(array)

			If point.getAllocationStatus() = AllocationStatus.DEVICE Then
				Dim context As CudaContext = AtomicAllocator.Instance.DeviceContext
				NativeOpsHolder.Instance.getDeviceNativeOps().memsetAsync(AtomicAllocator.Instance.getPointer(array, context),0, array.data().length() * Nd4j.sizeOfDataType(array.data().dataType()),0, context.getOldStream())

				' we also memset host pointer
				Pointer.memset(AtomicAllocator.Instance.getHostPointer(array), 0, array.data().length() * Nd4j.sizeOfDataType(array.data().dataType()))

				' better be safe then sorry
				context.getOldStream().synchronize()
				point.tickDeviceWrite()
				point.tickHostRead()
			ElseIf point.getAllocationStatus() = AllocationStatus.HOST Then
				Nd4j.Executioner.commit()

				' just casual memset
				Pointer.memset(AtomicAllocator.Instance.getHostPointer(array), 0, array.data().length() * Nd4j.sizeOfDataType(array.data().dataType()))
				point.tickHostWrite()
			End If
		End Sub

		Public Overrides ReadOnly Property BandwidthUse As IDictionary(Of Integer, Long)
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function allocatedMemory(ByVal deviceId As Integer?) As Long
			Return AllocationsTracker.Instance.bytesOnDevice(AllocationKind.GENERAL, deviceId) + AllocationsTracker.Instance.bytesOnDevice(AllocationKind.WORKSPACE, deviceId)
		End Function

		Public Overrides Sub releaseCurrentContext()
			Throw New System.NotSupportedException("Not implemented yet")
		End Sub
	End Class

End Namespace