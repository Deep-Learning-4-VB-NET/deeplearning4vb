Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports var = lombok.var
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports OpaqueLaunchContext = org.nd4j.nativeblas.OpaqueLaunchContext
Imports HashBasedTable = org.nd4j.shade.guava.collect.HashBasedTable
Imports Table = org.nd4j.shade.guava.collect.Table
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports RandomUtils = org.apache.commons.lang3.RandomUtils
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports DeviceAllocationsTracker = org.nd4j.jita.allocator.concurrency.DeviceAllocationsTracker
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports CudaConstants = org.nd4j.jita.allocator.enums.CudaConstants
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports PointersPair = org.nd4j.jita.allocator.pointers.PointersPair
Imports cublasHandle_t = org.nd4j.jita.allocator.pointers.cuda.cublasHandle_t
Imports cudaStream_t = org.nd4j.jita.allocator.pointers.cuda.cudaStream_t
Imports cusolverDnHandle_t = org.nd4j.jita.allocator.pointers.cuda.cusolverDnHandle_t
Imports Configuration = org.nd4j.jita.conf.Configuration
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports FlowController = org.nd4j.jita.flow.FlowController
Imports GridFlowController = org.nd4j.jita.flow.impl.GridFlowController
Imports MemoryHandler = org.nd4j.jita.handler.MemoryHandler
Imports MemoryProvider = org.nd4j.jita.memory.MemoryProvider
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BaseCudaDataBuffer = org.nd4j.linalg.jcublas.buffer.BaseCudaDataBuffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
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

Namespace org.nd4j.jita.handler.impl


	''' <summary>
	''' This Mover implementation uses following techs:
	''' 1. Unified Memory Architecture
	''' 2. Zero-Copy Pinned Memory (if available)
	''' 3. Pageable memory (if zero-copy pinned memory isn't supported by device)
	''' 
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class CudaZeroHandler
		Implements MemoryHandler

		Private Shared configuration As Configuration = CudaEnvironment.Instance.Configuration

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(CudaZeroHandler))

		' simple counter to track allocated host-memory
		Protected Friend ReadOnly zeroUseCounter As New AtomicLong(0)

		' another simple counter, to track allocated device memory on per-thread per-device basis
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: protected volatile org.nd4j.jita.allocator.concurrency.DeviceAllocationsTracker deviceMemoryTracker;
		Protected Friend deviceMemoryTracker As DeviceAllocationsTracker

		' tracker for thread->device affinity
		Protected Friend devicesAffinity As IDictionary(Of Long, Integer) = New ConcurrentDictionary(Of Long, Integer)()

		Private deviceLock As New ReentrantReadWriteLock()

		Private devPtr As New AtomicInteger(0)

		Private ReadOnly wasInitialised As New AtomicBoolean(False)

'JAVA TO VB CONVERTER NOTE: The field flowController was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly flowController_Conflict As FlowController

		Private ReadOnly INITIAL_LOCATION As AllocationStatus

		Private ReadOnly cublasHandles As IList(Of cublasHandle_t) = New List(Of cublasHandle_t)()

		Private ReadOnly affinityManager As AffinityManager = Nd4j.AffinityManager

		<NonSerialized>
		Private ReadOnly tlContext As New ThreadLocal(Of CudaContext)()

	'    
	'    table for Thread, Device, Object allocations of device memory. Objects should be used to grab Allocation point from allocationsMap
	'    
		' TODO: proper thread-safe implementation would be nice to have here :(
		' FIXME: CopyOnWriteArrayList is BAD here. Really BAD. B A D.
		' Table thread safety is guaranteed by reentrant read/write locks :(
		'private Table<Long, Integer, ConcurrentHashMap<Long, Long>> deviceAllocations = HashBasedTable.create();
		'private final Map<Integer, ConcurrentHashMap<Long, Long>> deviceAllocations = new ConcurrentHashMap<>();
		Private ReadOnly deviceAllocations As IList(Of ConcurrentDictionary(Of Long, Long)) = New List(Of ConcurrentDictionary(Of Long, Long))()

	'    
	'        map for Thread, Object allocations in zero memory.
	'    
		' CopyOnWriteArrayList performance to be investigated in this use case
		' Map thread safety is guaranteed by exclusive writeLock in getDeviceId() method, because we can't use putIfAbsent on j7
		' FIXME: at j7 -> j8 transition, this one could be changed to ConcurrentHashMap
		Private ReadOnly zeroAllocations As IDictionary(Of Long, ConcurrentDictionary(Of Long, Long)) = New ConcurrentDictionary(Of Long, ConcurrentDictionary(Of Long, Long))()


		Private zeroCounter As New AtomicLong(0)

		Protected Friend nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()

		Public Sub New()

			configuration.setInitialized()

			Me.INITIAL_LOCATION = configuration.getFirstMemory()

			Select Case configuration.getExecutionModel()
				Case SEQUENTIAL
					Me.flowController_Conflict = New GridFlowController()
				Case Else
					Throw New Exception("Unknown ExecutionModel: [" & configuration.getExecutionModel() & "]")
			End Select

			Dim numDevices As Integer = NativeOpsHolder.Instance.getDeviceNativeOps().getAvailableDevices()
			For i As Integer = 0 To numDevices - 1
				deviceAllocations.Add(New ConcurrentDictionary(Of Long, Long)())
				cublasHandles.Add(Nothing)
			Next i

			If NativeOpsHolder.Instance.getDeviceNativeOps().getDeviceMajor(0) < 3 Then
				Throw New ND4JIllegalStateException("CUDA backend requires compute capatibility of 3.0 and above to run.")
			End If
		End Sub

		''' <summary>
		''' This method gets called from Allocator, during Allocator/MemoryHandler initialization
		''' </summary>
		''' <param name="configuration"> </param>
		''' <param name="allocator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void init(@NonNull Configuration configuration, @NonNull Allocator allocator)
		Public Overridable Sub init(ByVal configuration As Configuration, ByVal allocator As Allocator)
			Me.configuration = configuration

			Me.deviceMemoryTracker = New DeviceAllocationsTracker(Me.configuration)
			Me.flowController_Conflict.init(allocator)
		End Sub

		Private Sub pickupHostAllocation(ByVal point As AllocationPoint)
			Dim numBuckets As Integer = configuration.getNumberOfGcThreads()
			Dim bucketId As Long = RandomUtils.nextInt(0, numBuckets)

			Dim reqMemory As Long = point.NumberOfBytes

			zeroUseCounter.addAndGet(reqMemory)

			point.setBucketId(bucketId)

			If Not zeroAllocations.ContainsKey(bucketId) Then
				log.debug("Creating bucketID: " & bucketId)
				SyncLock Me
					If Not zeroAllocations.ContainsKey(bucketId) Then
						zeroAllocations(bucketId) = New ConcurrentDictionary(Of Long, Long)()
					End If
				End SyncLock
			End If

			zeroAllocations(bucketId)(point.getObjectId()) = point.getObjectId()
		End Sub


		''' <summary>
		''' Allocate specified memory chunk on specified device/host
		''' </summary>
		''' <param name="targetMode"> valid arguments are DEVICE, ZERO </param>
		''' <param name="shape">
		''' @return </param>
		Public Overridable Function alloc(ByVal targetMode As AllocationStatus, ByVal point As AllocationPoint, ByVal shape As AllocationShape, ByVal initialize As Boolean) As PointersPair Implements MemoryHandler.alloc

				Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' This method checks if specified device has free memory
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="requiredMemory">
		''' @return </param>
		Public Overridable Function pingDeviceForFreeMemory(ByVal deviceId As Integer?, ByVal requiredMemory As Long) As Boolean Implements MemoryHandler.pingDeviceForFreeMemory
			Return True
		End Function

		''' <summary>
		''' Copies specific chunk of memory from one storage to another
		''' 
		''' Possible directions:  HOST -> DEVICE, DEVICE -> HOST
		''' </summary>
		''' <param name="currentStatus"> </param>
		''' <param name="targetStatus"> </param>
		''' <param name="point"> </param>
		Public Overridable Sub relocate(ByVal currentStatus As AllocationStatus, ByVal targetStatus As AllocationStatus, ByVal point As AllocationPoint, ByVal shape As AllocationShape, ByVal context As CudaContext) Implements MemoryHandler.relocate

		End Sub

		''' <summary>
		''' Copies memory from device to host, if needed.
		''' Device copy is preserved as is.
		''' </summary>
		''' <param name="point"> </param>
		<Obsolete>
		Public Overridable Sub copyback(ByVal point As AllocationPoint, ByVal shape As AllocationShape) Implements MemoryHandler.copyback
	'        
	'            Technically that's just a case for relocate, with source as point.getAllocationStatus() and target HOST
	'         
			'   log.info("copyback() called on shape: " + point.getShape());
			'  relocate(point.getAllocationStatus(), AllocationStatus.HOST, point, shape);
			Throw New System.NotSupportedException("Deprecated call")
		End Sub

		''' <summary>
		''' Copies memory from host buffer to device.
		''' Host copy is preserved as is.
		''' </summary>
		''' <param name="point"> </param>
		<Obsolete>
		Public Overridable Sub copyforward(ByVal point As AllocationPoint, ByVal shape As AllocationShape) Implements MemoryHandler.copyforward
			Throw New System.NotSupportedException("Deprecated call")
		End Sub

		''' <summary>
		''' Copies memory from device to zero-copy memory
		''' </summary>
		''' <param name="point"> </param>
		''' <param name="shape"> </param>
		<Obsolete>
		Public Overridable Sub fallback(ByVal point As AllocationPoint, ByVal shape As AllocationShape) Implements MemoryHandler.fallback
			Throw New System.InvalidOperationException("Can't fallback from [" & point.getAllocationStatus() & "]")
		End Sub

		''' <summary>
		''' This method frees memory chunk specified by pointer and location
		''' </summary>
		''' <param name="point"> Pointer </param>
		Public Overridable Sub free(ByVal point As AllocationPoint, ByVal target As AllocationStatus) Implements MemoryHandler.free

		End Sub

		''' <summary>
		''' This method returns initial allocation location. So, it can be HOST, or DEVICE if environment allows that.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property InitialLocation As AllocationStatus Implements MemoryHandler.getInitialLocation
			Get
				Return INITIAL_LOCATION
			End Get
		End Property

		''' <summary>
		''' This method initializes specific device for current thread
		''' </summary>
		''' <param name="threadId"> </param>
		''' <param name="deviceId"> </param>
		Public Overridable Sub initializeDevice(ByVal threadId As Long?, ByVal deviceId As Integer?) Implements MemoryHandler.initializeDevice
	'        
	'        JCuda.cudaSetDevice(deviceId);
	'        
	'        CudaContext context = new CudaContext();
	'        context.initHandle();
	'        context.initOldStream();
	'        //        context.initStream();
	'        context.associateHandle();
	'        
	'        contextPool.put(threadId, context);
	'        
		End Sub

		''' <summary>
		''' Asynchronous version of memcpy
		''' 
		''' PLEASE NOTE: This is device-dependent method, if it's not supported in your environment, blocking call will be used instead.
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcPointer"> </param>
		''' <param name="length"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub memcpyAsync(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long) Implements MemoryHandler.memcpyAsync
			If length < 1 Then
				Return
			End If

			Preconditions.checkArgument(length <= (dstBuffer.length() * Nd4j.sizeOfDataType(dstBuffer.dataType())), "Length requested is bigger than target DataBuffer length")

			Dim point As val = DirectCast(dstBuffer, BaseCudaDataBuffer).getAllocationPoint()
			Dim tContext As CudaContext = Nothing

			If dstBuffer.Constant Then
				Dim dstPointer As Pointer = New CudaPointer(point.getHostPointer().address() + dstOffset, 0L)
				Dim srcPointerJ As Pointer = New CudaPointer(srcPointer, length)

				Dim profD As val = PerformanceTracker.Instance.helperStartTransaction()
				Pointer.memcpy(dstPointer, srcPointerJ, length)
				PerformanceTracker.Instance.helperRegisterTransaction(point.getDeviceId(), profD, point.getNumberOfBytes(), MemcpyDirection.HOST_TO_HOST)

				point.tickHostRead()
			Else
				' if we're copying something into host memory, but we're on device - we need to provide exact copy to device as well
				Dim rDP As Pointer = New CudaPointer(point.getDevicePointer().address() + dstOffset)

				If tContext Is Nothing Then
					tContext = flowController_Conflict.prepareAction(point)
				End If

				Dim prof As var = PerformanceTracker.Instance.helperStartTransaction()

				flowController_Conflict.commitTransfer(tContext.getSpecialStream())

				If nativeOps.memcpyAsync(rDP, srcPointer, length, CudaConstants.cudaMemcpyHostToDevice, tContext.getSpecialStream()) = 0 Then
					Throw New System.InvalidOperationException("MemcpyAsync H2D failed: [" & srcPointer.address() & "] -> [" & rDP.address() & "]")
				End If

				flowController_Conflict.commitTransfer(tContext.getSpecialStream())

				PerformanceTracker.Instance.helperRegisterTransaction(point.getDeviceId(), prof, point.getNumberOfBytes(), MemcpyDirection.HOST_TO_DEVICE)

				flowController_Conflict.registerAction(tContext, point)
				point.tickDeviceWrite()

				' we optionally copy to host memory
				If point.getHostPointer() IsNot Nothing Then
					Dim dP As Pointer = New CudaPointer((point.getHostPointer().address()) + dstOffset)

					Dim context As CudaContext = flowController_Conflict.prepareAction(point)
					tContext = context

					prof = PerformanceTracker.Instance.helperStartTransaction()

					If nativeOps.memcpyAsync(dP, srcPointer, length, CudaConstants.cudaMemcpyHostToHost, context.getSpecialStream()) = 0 Then
						Throw New System.InvalidOperationException("MemcpyAsync H2H failed: [" & srcPointer.address() & "] -> [" & dP.address() & "]")
					End If

					flowController_Conflict.commitTransfer(tContext.getSpecialStream())

					PerformanceTracker.Instance.helperRegisterTransaction(point.getDeviceId(), prof, point.getNumberOfBytes(), MemcpyDirection.HOST_TO_HOST)

					If point.getAllocationStatus() = AllocationStatus.HOST Then
						flowController_Conflict.registerAction(context, point)
					End If

					point.tickHostRead()
				End If
			End If
		End Sub

		Public Overridable Sub memcpyDevice(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long, ByVal context As CudaContext) Implements MemoryHandler.memcpyDevice
			Dim point As AllocationPoint = DirectCast(dstBuffer, BaseCudaDataBuffer).getAllocationPoint()

			Dim dP As Pointer = New CudaPointer((point.DevicePointer.address()) + dstOffset)

			If nativeOps.memcpyAsync(dP, srcPointer, length, CudaConstants.cudaMemcpyDeviceToDevice, context.getOldStream()) = 0 Then
				Throw New ND4JIllegalStateException("memcpyAsync failed")
			End If

			point.tickDeviceWrite()
		End Sub

		''' <summary>
		''' Special memcpy version, addressing shapeInfoDataBuffer copies
		''' 
		''' PLEASE NOTE: Blocking H->H, Async H->D
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcPointer"> </param>
		''' <param name="length"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub memcpySpecial(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long) Implements MemoryHandler.memcpySpecial
			Dim context As CudaContext = CudaContext
			Dim point As AllocationPoint = DirectCast(dstBuffer, BaseCudaDataBuffer).getAllocationPoint()

			Dim dP As Pointer = New CudaPointer((point.HostPointer.address()) + dstOffset)

			Dim profH As val = PerformanceTracker.Instance.helperStartTransaction()

			If nativeOps.memcpyAsync(dP, srcPointer, length, CudaConstants.cudaMemcpyHostToHost, context.getOldStream()) = 0 Then
				Throw New ND4JIllegalStateException("memcpyAsync failed")
			End If

			PerformanceTracker.Instance.helperRegisterTransaction(point.DeviceId, profH, point.NumberOfBytes,MemcpyDirection.HOST_TO_HOST)

			If point.getAllocationStatus() = AllocationStatus.DEVICE Then
				Dim rDP As Pointer = New CudaPointer(point.DevicePointer.address() + dstOffset)

				Dim profD As val = PerformanceTracker.Instance.helperStartTransaction()

				If nativeOps.memcpyAsync(rDP, dP, length, CudaConstants.cudaMemcpyHostToDevice, context.getOldStream()) = 0 Then
					Throw New ND4JIllegalStateException("memcpyAsync failed")
				End If

				context.syncOldStream()

				PerformanceTracker.Instance.helperRegisterTransaction(point.DeviceId, profD, point.NumberOfBytes,MemcpyDirection.HOST_TO_DEVICE)
			End If

			context.syncOldStream()


			point.tickDeviceWrite()
		End Sub



		''' <summary>
		'''  Synchronous version of memcpy.
		''' 
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcPointer"> </param>
		''' <param name="length"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub memcpyBlocking(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long) Implements MemoryHandler.memcpyBlocking
			' internally it's just memcpyAsync + sync
			Dim context As CudaContext = CudaContext
			memcpyAsync(dstBuffer, srcPointer, length, dstOffset)
			context.syncOldStream()
		End Sub

		''' <summary>
		'''  Synchronous version of memcpy.
		''' 
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcBuffer"> </param>
		Public Overridable Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer) Implements MemoryHandler.memcpy
			'log.info("Buffer MemCpy called");
			'log.info("Memcpy buffer: {} bytes ", dstBuffer.length() * dstBuffer.getElementSize());
			Dim context As CudaContext = CudaContext
			Dim dstPoint As val = DirectCast(dstBuffer, BaseCudaDataBuffer).getAllocationPoint()
			Dim srcPoint As val = DirectCast(srcBuffer, BaseCudaDataBuffer).getAllocationPoint()

			Dim dP As Pointer = Nothing 'new CudaPointer(dstPoint.getPointers().getHostPointer().address());
			Dim sP As Pointer = Nothing
			Dim direction As MemcpyDirection = Nothing

			Dim profDH As val = PerformanceTracker.Instance.helperStartTransaction()



			Nd4j.Executioner.push()

			If srcPoint.isActualOnDeviceSide() Then
				sP = AtomicAllocator.Instance.getPointer(srcBuffer, context)
				dP = AtomicAllocator.Instance.getPointer(dstBuffer, context)

				If nativeOps.memcpyAsync(dP, sP, srcBuffer.length() * srcBuffer.ElementSize, CudaConstants.cudaMemcpyDeviceToDevice, context.getOldStream()) = 0 Then
					Throw New ND4JIllegalStateException("memcpyAsync failed")
				End If

				dstPoint.tickDeviceWrite()
				direction = MemcpyDirection.DEVICE_TO_DEVICE
			Else
				sP = AtomicAllocator.Instance.getHostPointer(srcBuffer)
				dP = AtomicAllocator.Instance.getPointer(dstBuffer, context)

				If nativeOps.memcpyAsync(dP, sP, srcBuffer.length() * srcBuffer.ElementSize, CudaConstants.cudaMemcpyHostToDevice, context.getOldStream()) = 0 Then
					Throw New ND4JIllegalStateException("memcpyAsync failed")
				End If

				direction = MemcpyDirection.HOST_TO_DEVICE
			End If

			dstPoint.tickDeviceWrite()

			' it has to be blocking call
			context.syncOldStream()

			PerformanceTracker.Instance.helperRegisterTransaction(srcPoint.getDeviceId(), profDH / 2, dstPoint.getNumberOfBytes(), direction)
	'        PerformanceTracker.getInstance().helperRegisterTransaction(dstPoint.getDeviceId(), profDH / 2, dstPoint.getNumberOfBytes(), MemcpyDirection.HOST_TO_DEVICE);
		End Sub

		''' <summary>
		''' PLEASE NOTE: Specific implementation, on systems without special devices can return HostPointer here
		''' </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Overridable Function getDevicePointer(ByVal buffer As DataBuffer, ByVal context As CudaContext) As Pointer Implements MemoryHandler.getDevicePointer
			' TODO: It would be awesome to get rid of typecasting here
			Dim dstPoint As AllocationPoint = DirectCast(buffer, BaseCudaDataBuffer).getAllocationPoint()

			' if that's device state, we probably might want to update device memory state
			If dstPoint.getAllocationStatus() = AllocationStatus.DEVICE Then
				If Not dstPoint.ActualOnDeviceSide Then
					'relocate(AllocationStatus.HOST, AllocationStatus.DEVICE, dstPoint, dstPoint.getShape(), context);
					Throw New System.NotSupportedException("Pew-pew")
				End If
			End If

			If dstPoint.DevicePointer Is Nothing Then
				Return Nothing
			End If


			' return pointer. length is specified for constructor compatibility purposes. Offset is accounted at C++ side
			Dim p As val = New CudaPointer(dstPoint.DevicePointer, buffer.length(), 0)

			If OpProfiler.Instance.getConfig().isCheckLocality() Then
				 NativeOpsHolder.Instance.getDeviceNativeOps().tryPointer(context.getOldStream(), p, 1)
			End If

			Select Case buffer.dataType()
				Case [DOUBLE]
					Return p.asDoublePointer()
				Case FLOAT
					Return p.asFloatPointer()
				Case UINT32, INT
					Return p.asIntPointer()
				Case [SHORT], UINT16, HALF, BFLOAT16
					Return p.asShortPointer()
				Case UINT64, [LONG]
					Return p.asLongPointer()
				Case UTF8, UBYTE, [BYTE]
					Return p.asBytePointer()
				Case BOOL
					Return p.asBooleanPointer()
				Case Else
					Return p
			End Select
		End Function

		''' <summary>
		''' PLEASE NOTE: This method always returns pointer within OS memory space
		''' </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Overridable Function getHostPointer(ByVal buffer As DataBuffer) As Pointer Implements MemoryHandler.getHostPointer
			Dim dstPoint As AllocationPoint = DirectCast(buffer, BaseCudaDataBuffer).getAllocationPoint()

			' return pointer with offset if needed. length is specified for constructor compatibility purposes
			If dstPoint.HostPointer Is Nothing Then
				Return Nothing
			End If

			synchronizeThreadDevice(Thread.CurrentThread.getId(), dstPoint.DeviceId, dstPoint)

			Dim p As New CudaPointer(dstPoint.HostPointer, buffer.length(), 0)

			Select Case buffer.dataType()
				Case [DOUBLE]
					Return p.asDoublePointer()
				Case FLOAT
					Return p.asFloatPointer()
				Case UINT32, INT
					Return p.asIntPointer()
				Case [SHORT], UINT16, BFLOAT16, HALF
					Return p.asShortPointer()
				Case UINT64, [LONG]
					Return p.asLongPointer()
				Case Else
					Return p
			End Select
		End Function

		Public Overridable Sub relocateObject(ByVal buffer As DataBuffer) Implements MemoryHandler.relocateObject
			SyncLock Me
				Dim dstPoint As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(buffer)
        
				If 1 > 0 Then
					Throw New System.NotSupportedException("Pew-pew")
				End If
        
				' we don't relocate non-DEVICE buffers (i.e HOST or CONSTANT)
				If dstPoint.getAllocationStatus() <> AllocationStatus.DEVICE Then
					Return
				End If
        
				Dim deviceId As Integer = getDeviceId()
        
        
				If dstPoint.DeviceId >= 0 AndAlso dstPoint.DeviceId = deviceId Then
					Return
				End If
        
				Dim okDevice As val = dstPoint.ActualOnDeviceSide
				Dim okHost As val = dstPoint.ActualOnHostSide
        
				Dim odPtr As val = dstPoint.DevicePointer
				Dim ohPtr As val = dstPoint.HostPointer
        
				' FIXME: cross-thread access, might cause problems
				If dstPoint.HostPointer IsNot Nothing AndAlso Not dstPoint.ActualOnHostSide Then
					AtomicAllocator.Instance.synchronizeHostData(buffer)
				End If
        
				If dstPoint.HostPointer IsNot Nothing AndAlso Not dstPoint.ActualOnHostSide Then
					Throw New Exception("Buffer synchronization failed")
				End If
        
				If buffer.Attached OrElse dstPoint.isAttached() Then
					' if this buffer is Attached, we just relocate to new workspace
        
					Dim workspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
        
					If workspace Is Nothing Then
						' if we're out of workspace, we should mark our buffer as detached, so gc will pick it up eventually
						' host part is optional
						If dstPoint.HostPointer IsNot Nothing Then
							'val pairH = alloc(AllocationStatus.HOST, dstPoint, dstPoint.getShape(), false);
							'dstPoint.getPointers().setHostPointer(pairH.getHostPointer());
						End If
        
						'val pairD = alloc(AllocationStatus.DEVICE, dstPoint, dstPoint.getShape(), false);
						'dstPoint.getPointers().setDevicePointer(pairD.getDevicePointer());
        
						'//log.info("New host pointer: {}; Old host pointer: {}", dstPoint.getHostPointer().address(), ohPtr.address());
        
						Dim context As CudaContext = CudaContext
        
						Dim profD As val = PerformanceTracker.Instance.helperStartTransaction()
        
						If okDevice Then
							If nativeOps.memcpyAsync(dstPoint.DevicePointer, odPtr, buffer.length() * buffer.ElementSize, CudaConstants.cudaMemcpyDeviceToDevice, context.getSpecialStream()) = 0 Then
								Throw New ND4JIllegalStateException("memcpyAsync failed")
							End If
        
							context.syncSpecialStream()
							PerformanceTracker.Instance.helperRegisterTransaction(dstPoint.DeviceId, profD / 2, dstPoint.NumberOfBytes, MemcpyDirection.DEVICE_TO_DEVICE)
						Else
							If nativeOps.memcpyAsync(dstPoint.DevicePointer, ohPtr, buffer.length() * buffer.ElementSize, CudaConstants.cudaMemcpyHostToDevice, context.getSpecialStream()) = 0 Then
								Throw New ND4JIllegalStateException("memcpyAsync failed")
							End If
        
							context.syncSpecialStream()
							PerformanceTracker.Instance.helperRegisterTransaction(dstPoint.DeviceId, profD / 2, dstPoint.NumberOfBytes, MemcpyDirection.HOST_TO_DEVICE)
						End If
						' marking it as detached
						dstPoint.setAttached(False)
        
						' marking it as proper on device
						dstPoint.tickDeviceWrite()
					Else
						' this call will automagically take care of workspaces, so it'll be either
						'log.info("Relocating to deviceId [{}], workspace [{}]...", deviceId, workspace.getId());
						Dim nBuffer As BaseCudaDataBuffer = DirectCast(Nd4j.createBuffer(buffer.length()), BaseCudaDataBuffer)
        
						Nd4j.MemoryManager.memcpy(nBuffer, buffer)
        
						'dstPoint.getPointers().setDevicePointer(nBuffer.getAllocationPoint().getDevicePointer());
        
						If dstPoint.HostPointer IsNot Nothing Then
						  '  dstPoint.getPointers().setHostPointer(nBuffer.getAllocationPoint().getHostPointer());
						End If
        
						dstPoint.DeviceId = deviceId
        
						dstPoint.tickDeviceRead()
						dstPoint.tickHostRead()
					End If
        
        
					Return
				End If
        
				If buffer.Constant Then
					' we can't relocate or modify buffers
					Throw New Exception("Can't relocateObject() for constant buffer")
				Else
					'                log.info("Free relocateObject: deviceId: {}, pointer: {}", deviceId, dstPoint.getPointers().getDevicePointer().address());
					Dim context As val = CudaContext
					If dstPoint.HostPointer Is Nothing Then
						DirectCast(buffer, BaseCudaDataBuffer).lazyAllocateHostPointer()
        
						If nativeOps.memcpyAsync(dstPoint.HostPointer, dstPoint.DevicePointer, buffer.length() * buffer.ElementSize, 2, context.getSpecialStream()) = 0 Then
							Throw New ND4JIllegalStateException("memcpyAsync failed")
						End If
        
						context.syncSpecialStream()
					End If
        
					'deviceMemoryTracker.subFromAllocation(Thread.currentThread().getId(), dstPoint.getDeviceId(), AllocationUtils.getRequiredMemory(dstPoint.getShape()));
        
					' we replace original device pointer with new one
					'alloc(AllocationStatus.DEVICE, dstPoint, dstPoint.getShape(), false);
        
					Dim profD As val = PerformanceTracker.Instance.helperStartTransaction()
        
					If nativeOps.memcpyAsync(dstPoint.DevicePointer, dstPoint.HostPointer, buffer.length() * buffer.ElementSize, 1, context.getSpecialStream()) = 0 Then
						Throw New ND4JIllegalStateException("memcpyAsync failed")
					End If
        
					context.syncSpecialStream()
        
					PerformanceTracker.Instance.helperRegisterTransaction(dstPoint.DeviceId, profD, dstPoint.NumberOfBytes, MemcpyDirection.HOST_TO_DEVICE)
        
					dstPoint.tickDeviceRead()
					dstPoint.tickHostRead()
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' This method moves specific object from zero-copy memory to device memory
		''' 
		''' PLEASE NOTE:  DO NOT EVER USE THIS METHOD MANUALLY, UNLESS YOU 100% HAVE TO
		''' 
		''' @return
		''' </summary>
		Public Overridable Function promoteObject(ByVal buffer As DataBuffer) As Boolean Implements MemoryHandler.promoteObject
			Dim dstPoint As AllocationPoint = AtomicAllocator.Instance.getAllocationPoint(buffer)

			If 1 > 0 Then
				Throw New System.NotSupportedException("Pew-pew")
			End If

			If dstPoint.getAllocationStatus() <> AllocationStatus.HOST Then
				Return False
			End If

			If configuration.getMemoryModel() = Configuration.MemoryModel.DELAYED AndAlso dstPoint.getAllocationStatus() = AllocationStatus.HOST Then


				' if we have constant buffer (aka shapeInfo or other constant stuff)
				If buffer.Constant Then
					Nd4j.ConstantHandler.moveToConstantSpace(buffer)
				Else

					Dim pair As PointersPair = Nothing 'memoryProvider.malloc(dstPoint.getShape(), dstPoint, AllocationStatus.DEVICE);

					If pair IsNot Nothing Then
						Dim deviceId As Integer? = getDeviceId()
						'               log.info("Promoting object to device: [{}]", deviceId);

						'dstPoint.setDevicePointer(pair.getDevicePointer());
						dstPoint.setAllocationStatus(AllocationStatus.DEVICE)

						deviceAllocations(deviceId)(dstPoint.getObjectId()) = dstPoint.getObjectId()

						zeroAllocations(dstPoint.getBucketId()).Remove(dstPoint.getObjectId())
						'deviceMemoryTracker.addToAllocation(Thread.currentThread().getId(), deviceId, AllocationUtils.getRequiredMemory(dstPoint.getShape()));


						dstPoint.tickHostWrite()
					Else
						Throw New Exception("PewPew")
					End If

				End If
			End If

			Return True
		End Function

		''' <summary>
		''' This method returns total amount of memory allocated within system
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AllocationStatistics As Table(Of AllocationStatus, Integer, Long) Implements MemoryHandler.getAllocationStatistics
			Get
				Dim table As Table(Of AllocationStatus, Integer, Long) = HashBasedTable.create()
				table.put(AllocationStatus.HOST, 0, zeroUseCounter.get())
				For Each deviceId As Integer? In configuration.getAvailableDevices()
					table.put(AllocationStatus.DEVICE, deviceId, getAllocatedDeviceMemory(deviceId))
				Next deviceId
				Return table
			End Get
		End Property

		''' <summary>
		''' This method returns total amount of memory allocated at specified device
		''' </summary>
		''' <param name="device">
		''' @return </param>
		Public Overridable Function getAllocatedDeviceMemory(ByVal device As Integer?) As Long Implements MemoryHandler.getAllocatedDeviceMemory
			Return deviceMemoryTracker.getAllocatedSize(device)
		End Function

		''' <summary>
		''' This method returns total amount of host memory allocated within this MemoryHandler
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AllocatedHostMemory As Long Implements MemoryHandler.getAllocatedHostMemory
			Get
				Return zeroUseCounter.get()
			End Get
		End Property

		''' <summary>
		''' This method returns total number of object allocated on specified device
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getAllocatedDeviceObjects(ByVal deviceId As Integer?) As Long Implements MemoryHandler.getAllocatedDeviceObjects
			Return deviceAllocations(deviceId).Count
		End Function

		''' <summary>
		''' This method returns number of allocated objects within specific bucket
		''' </summary>
		''' <param name="bucketId">
		''' @return </param>
		Public Overridable Function getAllocatedHostObjects(ByVal bucketId As Long?) As Long Implements MemoryHandler.getAllocatedHostObjects
			If zeroAllocations.ContainsKey(bucketId) Then
				Return zeroAllocations(bucketId).Count
			Else
				Return 0L
			End If
		End Function

		''' <summary>
		''' This method returns total number of allocated objects in host memory
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AllocatedHostObjects As Long Implements MemoryHandler.getAllocatedHostObjects
			Get
				Dim counter As New AtomicLong(0)
				For Each threadId As Long? In zeroAllocations.Keys
					counter.addAndGet(zeroAllocations(threadId).Count)
				Next threadId
				Return counter.get()
			End Get
		End Property

		''' <summary>
		''' This method returns set of allocation tracking IDs for specific device
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getDeviceTrackingPoints(ByVal deviceId As Integer?) As ISet(Of Long)
			Return deviceAllocations(deviceId).Keys
		End Function

		''' <summary>
		''' This method returns sets of allocation tracking IDs for specific bucket
		''' </summary>
		''' <param name="bucketId">
		''' @return </param>
		Public Overridable Function getHostTrackingPoints(ByVal bucketId As Long?) As ISet(Of Long)
			If Not zeroAllocations.ContainsKey(bucketId) Then
				Return New HashSet(Of Long)()
			End If
			Return zeroAllocations(bucketId).Keys
		End Function


		''' <summary>
		''' This method explicitly removes object from device memory.
		''' </summary>
		''' <param name="threadId"> </param>
		''' <param name="objectId"> </param>
		''' <param name="copyback">  if TRUE, corresponding memory block on JVM side will be updated, if FALSE - memory will be just discarded </param>
		Public Overridable Sub purgeDeviceObject(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal objectId As Long?, ByVal point As AllocationPoint, ByVal copyback As Boolean) Implements MemoryHandler.purgeDeviceObject
			If point.getAllocationStatus() <> AllocationStatus.DEVICE Then
				Return
			End If

			flowController_Conflict.waitTillReleased(point)

			free(point, AllocationStatus.DEVICE)

			If Not deviceAllocations(deviceId).ContainsKey(objectId) Then
				Throw New System.InvalidOperationException("Can't happen ever")
			End If

			forget(point, AllocationStatus.DEVICE)

			If deviceAllocations(deviceId).ContainsKey(objectId) Then
				Throw New System.InvalidOperationException("Can't happen ever")
			End If

			'deviceMemoryTracker.subFromAllocation(threadId, deviceId, AllocationUtils.getRequiredMemory(point.getShape()));

			point.setAllocationStatus(AllocationStatus.HOST)

			'environment.trackAllocatedMemory(deviceId, AllocationUtils.getRequiredMemory(point.getShape()));
		End Sub

		''' <summary>
		''' This method explicitly removes object from zero-copy memory.
		''' </summary>
		''' <param name="bucketId"> </param>
		''' <param name="objectId"> </param>
		''' <param name="copyback">  if TRUE, corresponding memory block on JVM side will be updated, if FALSE - memory will be just discarded </param>
		Public Overridable Sub purgeZeroObject(ByVal bucketId As Long?, ByVal objectId As Long?, ByVal point As AllocationPoint, ByVal copyback As Boolean) Implements MemoryHandler.purgeZeroObject
			If 1 > 0 Then
				Throw New System.NotSupportedException("Pew-pew")
			End If

			forget(point, AllocationStatus.HOST)

			flowController_Conflict.waitTillReleased(point)

			' we call for caseless deallocation here
			If point.HostPointer IsNot Nothing Then
				free(point, AllocationStatus.HOST)

				'long reqMem = AllocationUtils.getRequiredMemory(point.getShape()) * -1;
				'zeroUseCounter.addAndGet(reqMem);
			End If

			point.setAllocationStatus(AllocationStatus.DEALLOCATED)
		End Sub

		Public Overridable Sub forget(ByVal point As AllocationPoint, ByVal location As AllocationStatus) Implements MemoryHandler.forget
			If location = AllocationStatus.DEVICE Then
				deviceAllocations(point.DeviceId).Remove(point.getObjectId())
			ElseIf location = AllocationStatus.HOST Then
				If point.HostPointer IsNot Nothing Then
					zeroAllocations(point.getBucketId()).Remove(point.getObjectId())
				End If
			End If
		End Sub


		''' <summary>
		''' This method returns CUDA deviceId for current thread
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DeviceId As Integer? Implements MemoryHandler.getDeviceId
			Get
				Dim deviceId As Integer = Nd4j.AffinityManager.getDeviceForCurrentThread()
    
				Return deviceId
			End Get
		End Property

		''' <summary>
		''' Returns <seealso cref="getDeviceId()"/> wrapped as a <seealso cref="Pointer"/>. </summary>
		Public Overridable ReadOnly Property DeviceIdPointer As Pointer Implements MemoryHandler.getDeviceIdPointer
			Get
				Return New CudaPointer(getDeviceId())
			End Get
		End Property

		''' <summary>
		''' This method returns set of available devices
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AvailableDevices As ISet(Of Integer)
			Get
				Return New HashSet(Of Integer)(configuration.getAvailableDevices())
			End Get
		End Property

		''' <summary>
		''' This method returns ExternalContext wrapper (if applicable)
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DeviceContext As CudaContext Implements MemoryHandler.getDeviceContext
			Get
				Return CudaContext
			End Get
		End Property

		'
		Private ReadOnly lock As New ReentrantReadWriteLock()

		Protected Friend Overridable Function getCudaCublasHandle(ByVal lc As OpaqueLaunchContext) As cublasHandle_t
			Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Try
				lock.writeLock().lock()

				If cublasHandles(deviceId) Is Nothing Then
					cublasHandles.Remove(deviceId)
					cublasHandles.Insert(deviceId, New cublasHandle_t(nativeOps.lcBlasHandle(lc)))
				End If

				Return cublasHandles(deviceId)
			Finally
				lock.writeLock().unlock()
			End Try
		End Function

		''' <summary>
		''' This method returns CudaContext for current thread. If context doesn't exist - it gets created first.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property CudaContext As CudaContext Implements MemoryHandler.getCudaContext
			Get
				Dim ctx As var = tlContext.get()
				If ctx Is Nothing Then
					Dim lc As val = nativeOps.defaultLaunchContext()
    
					ctx = CudaContext.builder().bufferScalar(nativeOps.lcScalarPointer(lc)).bufferReduction(nativeOps.lcReductionPointer(lc)).bufferAllocation(nativeOps.lcAllocationPointer(lc)).bufferSpecial(nativeOps.lcScalarPointer(lc)).oldStream(New cudaStream_t(nativeOps.lcExecutionStream(lc))).specialStream(New cudaStream_t(nativeOps.lcCopyStream(lc))).cublasHandle(getCudaCublasHandle(lc)).solverHandle(New cusolverDnHandle_t(nativeOps.lcSolverHandle(lc))).build()
    
					tlContext.set(ctx)
					Return ctx
				Else
					Return ctx
				End If
			End Get
		End Property

		Public Overridable Sub resetCachedContext() Implements MemoryHandler.resetCachedContext
			tlContext.remove()
		End Sub

		''' <summary>
		''' This method returns if this MemoryHandler instance is device-dependant (i.e. CUDA)
		''' </summary>
		''' <returns> TRUE if dependant, FALSE otherwise </returns>
		Public Overridable ReadOnly Property DeviceDependant As Boolean Implements MemoryHandler.isDeviceDependant
			Get
				' this is always TRUE for current implementation
				Return True
			End Get
		End Property

		''' <summary>
		''' This method causes memory synchronization on host side.
		'''  Viable only for Device-dependant MemoryHandlers
		''' </summary>
		''' <param name="threadId"> </param>
		''' <param name="deviceId"> </param>
		''' <param name="point"> </param>
		Public Overridable Sub synchronizeThreadDevice(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal point As AllocationPoint) Implements MemoryHandler.synchronizeThreadDevice
			' we synchronize only if this AllocationPoint was used within device context, so for multiple consequent syncs only first one will be issued
			flowController_Conflict.synchronizeToHost(point)
		End Sub

		Public Overridable Sub registerAction(ByVal context As CudaContext, ByVal result As INDArray, ParamArray ByVal operands() As INDArray) Implements MemoryHandler.registerAction
			flowController_Conflict.registerAction(context, result, operands)
		End Sub

		Public Overridable ReadOnly Property FlowController As FlowController Implements MemoryHandler.getFlowController
			Get
				Return flowController_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property MemoryProvider As MemoryProvider Implements MemoryHandler.getMemoryProvider
			Get
				Return Nothing
			End Get
		End Property
	End Class

End Namespace