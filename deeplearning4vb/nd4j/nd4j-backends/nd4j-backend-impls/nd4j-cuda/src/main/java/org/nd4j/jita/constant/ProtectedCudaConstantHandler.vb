Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports Configuration = org.nd4j.jita.conf.Configuration
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports FlowController = org.nd4j.jita.flow.FlowController
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports ArrayDescriptor = org.nd4j.linalg.cache.ArrayDescriptor
Imports ConstantHandler = org.nd4j.linalg.cache.ConstantHandler
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.linalg.jcublas.buffer
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports Slf4j = lombok.extern.slf4j.Slf4j

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

Namespace org.nd4j.jita.constant


	''' <summary>
	''' Created by raver on 08.06.2016.
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ProtectedCudaConstantHandler implements org.nd4j.linalg.cache.ConstantHandler
	Public Class ProtectedCudaConstantHandler
		Implements ConstantHandler

		Private Shared ourInstance As New ProtectedCudaConstantHandler()

		Protected Friend constantOffsets As IDictionary(Of Integer, AtomicLong) = New Dictionary(Of Integer, AtomicLong)()
		Protected Friend deviceLocks As IDictionary(Of Integer, Semaphore) = New ConcurrentDictionary(Of Integer, Semaphore)()

		Protected Friend buffersCache As IDictionary(Of Integer, IDictionary(Of ArrayDescriptor, DataBuffer)) = New Dictionary(Of Integer, IDictionary(Of ArrayDescriptor, DataBuffer))()
		Protected Friend deviceAddresses As IDictionary(Of Integer, Pointer) = New Dictionary(Of Integer, Pointer)()
		Protected Friend bytes As New AtomicLong(0)
		Protected Friend flowController As FlowController

		Protected Friend Shared ReadOnly protector As ConstantProtector = ConstantProtector.Instance

		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(ProtectedCudaConstantHandler))

		Private Const MAX_CONSTANT_LENGTH As Integer = 49152
		Private Const MAX_BUFFER_LENGTH As Integer = 272

		Protected Friend lock As New Semaphore(1)
		Private resetHappened As Boolean = False


		Public Shared ReadOnly Property Instance As ProtectedCudaConstantHandler
			Get
				Return ourInstance
			End Get
		End Property

		Private Sub New()
		End Sub

		''' <summary>
		''' This method removes all cached constants
		''' </summary>
		Public Overridable Sub purgeConstants() Implements ConstantHandler.purgeConstants
			buffersCache = New Dictionary(Of Integer, IDictionary(Of ArrayDescriptor, DataBuffer))()

			protector.purgeProtector()

			resetHappened = True
			logger.info("Resetting Constants...")

			For Each device As Integer? In constantOffsets.Keys
				constantOffsets(device).set(0)
				buffersCache(device) = New ConcurrentDictionary(Of ArrayDescriptor, DataBuffer)()
			Next device
		End Sub

		''' <summary>
		''' Method suited for debug purposes only
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable Function amountOfEntries(ByVal deviceId As Integer) As Integer
			ensureMaps(deviceId)
			Return buffersCache(0).Count
		End Function

		''' <summary>
		''' This method moves specified dataBuffer to CUDA constant memory space.
		''' 
		''' PLEASE NOTE: CUDA constant memory is limited to 48KB per device.
		''' </summary>
		''' <param name="dataBuffer">
		''' @return </param>
		Public Overridable Function moveToConstantSpace(ByVal dataBuffer As DataBuffer) As Long Implements ConstantHandler.moveToConstantSpace
			SyncLock Me
				If 1 > 0 Then
					Throw New Exception("This code shouldn't be called, ever")
				End If
        
				' now, we move things to constant memory
				Dim deviceId As Integer? = AtomicAllocator.Instance.getDeviceId()
				ensureMaps(deviceId)
        
				Dim point As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(dataBuffer)
        
				Dim requiredMemoryBytes As Long = point.NumberOfBytes
				Dim originalBytes As val = requiredMemoryBytes
				requiredMemoryBytes += 8 - (requiredMemoryBytes Mod 8)
        
				Dim div As val = requiredMemoryBytes \ 4
				If div Mod 2 <> 0 Then
					requiredMemoryBytes += 4
				End If
        
				'logger.info("shape: " + point.getShape());
				' and release device memory :)
        
				AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, deviceId, requiredMemoryBytes)
        
				Dim currentOffset As Long = constantOffsets(deviceId).get()
				Dim context As val = AtomicAllocator.Instance.DeviceContext
				If currentOffset + requiredMemoryBytes >= MAX_CONSTANT_LENGTH OrElse requiredMemoryBytes > MAX_BUFFER_LENGTH Then
					If point.getAllocationStatus() = AllocationStatus.HOST AndAlso CudaEnvironment.Instance.Configuration.getMemoryModel() = Configuration.MemoryModel.DELAYED Then
						'AtomicAllocator.getInstance().getMemoryHandler().alloc(AllocationStatus.DEVICE, point, point.getShape(), false);
						Throw New System.NotSupportedException("Pew-pew")
					End If
        
					Dim profD As val = PerformanceTracker.Instance.helperStartTransaction()
        
					If NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(point.DevicePointer, point.HostPointer, originalBytes, 1, context.getSpecialStream()) = 0 Then
						Throw New ND4JIllegalStateException("memcpyAsync failed")
					End If
					flowController.commitTransfer(context.getSpecialStream())
        
					PerformanceTracker.Instance.helperRegisterTransaction(point.DeviceId, profD, point.NumberOfBytes, MemcpyDirection.HOST_TO_DEVICE)
        
					point.setConstant(True)
					point.tickDeviceWrite()
					point.tickHostRead()
					point.DeviceId = deviceId
        
					protector.persistDataBuffer(dataBuffer)
        
					Return 0
				End If
        
				Dim bytes As Long = requiredMemoryBytes
				currentOffset = constantOffsets(deviceId).getAndAdd(bytes)
        
				If currentOffset >= MAX_CONSTANT_LENGTH Then
					If point.getAllocationStatus() = AllocationStatus.HOST AndAlso CudaEnvironment.Instance.Configuration.getMemoryModel() = Configuration.MemoryModel.DELAYED Then
						'AtomicAllocator.getInstance().getMemoryHandler().alloc(AllocationStatus.DEVICE, point, point.getShape(), false);
						Throw New System.NotSupportedException("Pew-pew")
					End If
        
					Dim profD As val = PerformanceTracker.Instance.helperStartTransaction()
        
					If NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(point.DevicePointer, point.HostPointer, originalBytes, 1, context.getSpecialStream()) = 0 Then
						Throw New ND4JIllegalStateException("memcpyAsync failed")
					End If
					flowController.commitTransfer(context.getSpecialStream())
        
					PerformanceTracker.Instance.helperRegisterTransaction(point.DeviceId, profD, point.NumberOfBytes, MemcpyDirection.HOST_TO_DEVICE)
        
					point.setConstant(True)
					point.tickDeviceWrite()
					point.tickHostRead()
					point.DeviceId = deviceId
        
					protector.persistDataBuffer(dataBuffer)
        
					Return 0
				End If
        
        
        
				NativeOpsHolder.Instance.getDeviceNativeOps().memcpyConstantAsync(currentOffset, point.HostPointer, originalBytes, 1, context.getSpecialStream())
				flowController.commitTransfer(context.getSpecialStream())
        
				Dim cAddr As Long = deviceAddresses(deviceId).address() + currentOffset
        
				'if (resetHappened)
				'    logger.info("copying to constant: {}, bufferLength: {}, bufferDtype: {}, currentOffset: {}, currentAddres: {}", requiredMemoryBytes, dataBuffer.length(), dataBuffer.dataType(), currentOffset, cAddr);
        
				point.setAllocationStatus(AllocationStatus.CONSTANT)
				'point.setDevicePointer(new CudaPointer(cAddr));
				If 1 > 0 Then
					Throw New System.NotSupportedException("Pew-pew")
				End If
        
				point.setConstant(True)
				point.tickDeviceWrite()
				point.DeviceId = deviceId
				point.tickHostRead()
        
        
				protector.persistDataBuffer(dataBuffer)
        
				Return cAddr
			End SyncLock
		End Function

		''' <summary>
		''' PLEASE NOTE: This method implementation is hardware-dependant.
		''' PLEASE NOTE: This method does NOT allow concurrent use of any array
		''' </summary>
		''' <param name="dataBuffer">
		''' @return </param>
		Public Overridable Function relocateConstantSpace(ByVal dataBuffer As DataBuffer) As DataBuffer Implements ConstantHandler.relocateConstantSpace
			' we always assume that data is sync, and valid on host side
			Dim deviceId As Integer? = AtomicAllocator.Instance.getDeviceId()
			ensureMaps(deviceId)

			If TypeOf dataBuffer Is CudaIntDataBuffer Then
				Dim data() As Integer = dataBuffer.asInt()
				Return getConstantBuffer(data, DataType.INT)
			ElseIf TypeOf dataBuffer Is CudaFloatDataBuffer Then
				Dim data() As Single = dataBuffer.asFloat()
				Return getConstantBuffer(data, DataType.FLOAT)
			ElseIf TypeOf dataBuffer Is CudaDoubleDataBuffer Then
				Dim data() As Double = dataBuffer.asDouble()
				Return getConstantBuffer(data, DataType.DOUBLE)
			ElseIf TypeOf dataBuffer Is CudaHalfDataBuffer Then
				Dim data() As Single = dataBuffer.asFloat()
				Return getConstantBuffer(data, DataType.HALF)
			ElseIf TypeOf dataBuffer Is CudaLongDataBuffer Then
				Dim data() As Long = dataBuffer.asLong()
				Return getConstantBuffer(data, DataType.LONG)
			End If

			Throw New System.InvalidOperationException("Unknown CudaDataBuffer opType")
		End Function

		Private Sub ensureMaps(ByVal deviceId As Integer?)
			If Not buffersCache.ContainsKey(deviceId) Then
				If flowController Is Nothing Then
					flowController = AtomicAllocator.Instance.FlowController
				End If

				Try
					SyncLock Me
						If Not buffersCache.ContainsKey(deviceId) Then

							' TODO: this op call should be checked
							'nativeOps.setDevice(new CudaPointer(deviceId));

							buffersCache(deviceId) = New ConcurrentDictionary(Of ArrayDescriptor, DataBuffer)()
							constantOffsets(deviceId) = New AtomicLong(0)
							deviceLocks(deviceId) = New Semaphore(1)

							Dim cAddr As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().getConstantSpace()
							'                    logger.info("constant pointer: {}", cAddr.address() );

							deviceAddresses(deviceId) = cAddr
						End If
					End SyncLock
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End If
		End Sub

		''' <summary>
		''' This method returns DataBuffer with contant equal to input array.
		''' 
		''' PLEASE NOTE: This method assumes that you'll never ever change values within result DataBuffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Public Overridable Function getConstantBuffer(ByVal array() As Integer, ByVal type As DataType) As DataBuffer Implements ConstantHandler.getConstantBuffer
			Return Nd4j.Executioner.createConstantBuffer(array, type)
		End Function

		''' <summary>
		''' This method returns DataBuffer with contant equal to input array.
		''' 
		''' PLEASE NOTE: This method assumes that you'll never ever change values within result DataBuffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Public Overridable Function getConstantBuffer(ByVal array() As Single, ByVal type As DataType) As DataBuffer Implements ConstantHandler.getConstantBuffer
			Return Nd4j.Executioner.createConstantBuffer(array, type)
		End Function

		''' <summary>
		''' This method returns DataBuffer with contant equal to input array.
		''' 
		''' PLEASE NOTE: This method assumes that you'll never ever change values within result DataBuffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Public Overridable Function getConstantBuffer(ByVal array() As Double, ByVal type As DataType) As DataBuffer Implements ConstantHandler.getConstantBuffer
			Return Nd4j.Executioner.createConstantBuffer(array, type)
	'        
	'        ArrayDescriptor descriptor = new ArrayDescriptor(array, type);
	'
	'        Integer deviceId = AtomicAllocator.getInstance().getDeviceId();
	'
	'        ensureMaps(deviceId);
	'
	'        if (!buffersCache.get(deviceId).containsKey(descriptor)) {
	'            // we create new databuffer
	'            //logger.info("Creating new constant buffer...");
	'            DataBuffer buffer = Nd4j.createTypedBufferDetached(array, type);
	'
	'            if (constantOffsets.get(deviceId).get() + (array.length * Nd4j.sizeOfDataType()) < MAX_CONSTANT_LENGTH) {
	'                buffer.setConstant(true);
	'                // now we move data to constant memory, and keep happy
	'                moveToConstantSpace(buffer);
	'
	'                buffersCache.get(deviceId).put(descriptor, buffer);
	'
	'                bytes.addAndGet(array.length * Nd4j.sizeOfDataType());
	'            }
	'            return buffer;
	'        } //else logger.info("Reusing constant buffer...");
	'
	'        return buffersCache.get(deviceId).get(descriptor);
	'         
		End Function

		Public Overridable Function getConstantBuffer(ByVal array() As Long, ByVal type As DataType) As DataBuffer Implements ConstantHandler.getConstantBuffer
			Return Nd4j.Executioner.createConstantBuffer(array, type)
	'        
	'        //  logger.info("getConstantBuffer(int[]) called");
	'        ArrayDescriptor descriptor = new ArrayDescriptor(array, type);
	'
	'        Integer deviceId = AtomicAllocator.getInstance().getDeviceId();
	'
	'        ensureMaps(deviceId);
	'
	'        if (!buffersCache.get(deviceId).containsKey(descriptor)) {
	'            // we create new databuffer
	'            //logger.info("Creating new constant buffer...");
	'            DataBuffer buffer = Nd4j.createTypedBufferDetached(array, type);
	'
	'            if (constantOffsets.get(deviceId).get() + (array.length * 8) < MAX_CONSTANT_LENGTH) {
	'                buffer.setConstant(true);
	'                // now we move data to constant memory, and keep happy
	'                moveToConstantSpace(buffer);
	'
	'                buffersCache.get(deviceId).put(descriptor, buffer);
	'
	'                bytes.addAndGet(array.length * 8);
	'            }
	'            return buffer;
	'        } //else logger.info("Reusing constant buffer...");
	'
	'        return buffersCache.get(deviceId).get(descriptor);
	'         
		End Function

		Public Overridable Function getConstantBuffer(ByVal array() As Boolean, ByVal dataType As DataType) As DataBuffer Implements ConstantHandler.getConstantBuffer
			Return getConstantBuffer(ArrayUtil.toLongs(array), dataType)
		End Function

		Public Overridable ReadOnly Property CachedBytes As Long Implements ConstantHandler.getCachedBytes
			Get
				Return bytes.get()
			End Get
		End Property
	End Class

End Namespace