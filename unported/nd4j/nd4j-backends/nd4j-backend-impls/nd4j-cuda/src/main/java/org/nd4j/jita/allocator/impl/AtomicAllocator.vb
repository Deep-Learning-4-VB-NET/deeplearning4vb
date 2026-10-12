Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports Aggressiveness = org.nd4j.jita.allocator.enums.Aggressiveness
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports Ring = org.nd4j.jita.allocator.time.Ring
Imports LockedRing = org.nd4j.jita.allocator.time.rings.LockedRing
Imports Configuration = org.nd4j.jita.conf.Configuration
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports ConstantProtector = org.nd4j.jita.constant.ConstantProtector
Imports FlowController = org.nd4j.jita.flow.FlowController
Imports MemoryHandler = org.nd4j.jita.handler.MemoryHandler
Imports CudaZeroHandler = org.nd4j.jita.handler.impl.CudaZeroHandler
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ConstantHandler = org.nd4j.linalg.cache.ConstantHandler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BaseCudaDataBuffer = org.nd4j.linalg.jcublas.buffer.BaseCudaDataBuffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
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

Namespace org.nd4j.jita.allocator.impl


	''' <summary>
	''' Just-in-Time Allocator for CUDA
	''' 
	''' This method is a basement for pre-allocated memory management for cuda.
	''' Basically that's sophisticated garbage collector for both zero-copy memory, and multiple device memory.
	''' 
	''' There's multiple possible data movement directions, but general path is:
	''' host memory (issued on JVM side) ->
	'''          zero-copy pinned memory (which is allocated for everything out there) ->
	'''                  device memory (where data gets moved from zero-copy, if used actively enough)
	''' 
	''' And the backward movement, if memory isn't used anymore (like if originating INDArray was trashed by JVM GC), or it's not popular enough to hold in device memory
	''' 
	''' Mechanism is as lock-free, as possible. This achieved using three-state memory state signalling: Tick/Tack/Toe.
	''' Tick: memory chunk (or its part) is accessed on device
	''' Tack: memory chink (or its part) device access session was finished
	''' Toe: memory chunk is locked for some reason. Possible reasons:
	'''              Memory synchronization is ongoing, host->gpu or gpu->host
	'''              Memory relocation is ongoing, zero->gpu, or gpu->zero, or gpu->host
	'''              Memory removal is ongoing.
	''' 
	''' So, basically memory being used for internal calculations, not interfered with manual changes (aka putRow etc), are always available without locks
	''' 
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class AtomicAllocator
		Implements Allocator

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New AtomicAllocator()

'JAVA TO VB CONVERTER NOTE: The field configuration was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private configuration_Conflict As Configuration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private transient org.nd4j.jita.handler.MemoryHandler memoryHandler;
'JAVA TO VB CONVERTER NOTE: The field memoryHandler was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private memoryHandler_Conflict As MemoryHandler
		Private allocationsCounter As New AtomicLong(0)

		Private objectsTracker As New AtomicLong(0)

		' we have single tracking point for allocation points, since we're not going to cycle through it any time soon
'JAVA TO VB CONVERTER NOTE: The field allocationsMap was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private allocationsMap_Conflict As IDictionary(Of Long, AllocationPoint) = New ConcurrentDictionary(Of Long, AllocationPoint)()

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(AtomicAllocator))

	'    
	'        locks for internal resources
	'     
		Private globalLock As New ReentrantReadWriteLock()
		Private externalsLock As New ReentrantReadWriteLock()

	'    
	'        here we have handles for garbage collector threads
	'        ThreadId, GarbageCollector
	'     
		'private Map<Integer, UnifiedGarbageCollectorThread> collectorsUnified = new ConcurrentHashMap<>();

		Private ReadOnly shouldStop As New AtomicBoolean(False)

		Private ReadOnly wasInitialised As New AtomicBoolean(False)

		Private ReadOnly deviceLong As Ring = New LockedRing(30)
		Private ReadOnly deviceShort As Ring = New LockedRing(30)

		Private ReadOnly zeroLong As Ring = New LockedRing(30)
		Private ReadOnly zeroShort As Ring = New LockedRing(30)

		Private constantHandler As ConstantHandler = Nd4j.ConstantHandler
		Private useTracker As New AtomicLong(DateTimeHelper.CurrentUnixTimeMillis())

		Public Shared ReadOnly Property Instance As AtomicAllocator
			Get
				If INSTANCE_Conflict Is Nothing Then
					Throw New Exception("AtomicAllocator is NULL")
				End If
				Return INSTANCE_Conflict
			End Get
		End Property

		Protected Friend Shared protector As ConstantProtector

		Private Sub New()
			Me.configuration_Conflict = CudaEnvironment.Instance.Configuration
			applyConfiguration()

			Me.memoryHandler_Conflict = New CudaZeroHandler()

			Me.memoryHandler_Conflict.init(configuration_Conflict, Me)

	'        initDeviceCollectors();
	'        initHostCollectors();
			Me.protector = ConstantProtector.Instance

		End Sub

		Protected Friend Overridable Function allocationsMap() As IDictionary(Of Long, AllocationPoint)
			Return allocationsMap_Conflict
		End Function

		Public Overridable Sub applyConfiguration()
			'log.info("Applying CUDA configuration...");

			CudaEnvironment.Instance.notifyConfigurationApplied()

			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(configuration_Conflict.isDebug())
			'configuration.enableDebug(configuration.isDebug());

			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(configuration_Conflict.isVerbose())
			'configuration.setVerbose(configuration.isVerbose());

			NativeOpsHolder.Instance.getDeviceNativeOps().enableP2P(configuration_Conflict.isCrossDeviceAccessAllowed())
			'configuration.allowCrossDeviceAccess(configuration.isCrossDeviceAccessAllowed());

			NativeOpsHolder.Instance.getDeviceNativeOps().setGridLimit(configuration_Conflict.getMaximumGridSize())
			'configuration.setMaximumGridSize(configuration.getMaximumGridSize());

			NativeOpsHolder.Instance.getDeviceNativeOps().setOmpNumThreads(configuration_Conflict.getMaximumBlockSize())
			' configuration.setMaximumBlockSize(configuration.getMaximumBlockSize());

			NativeOpsHolder.Instance.getDeviceNativeOps().setOmpMinThreads(configuration_Conflict.getMinimumBlockSize())
			' configuration.setMinimumBlockSize(configuration.getMinimumBlockSize());
		End Sub

		''' <summary>
		''' This method executes preconfigured number of host memory garbage collectors
		''' </summary>
	'    protected void initHostCollectors() {
	'        for (int i = 0; i < configuration.getNumberOfGcThreads(); i++) {
	'            ReferenceQueue<BaseDataBuffer> queue = new ReferenceQueue<>();
	'
	'            UnifiedGarbageCollectorThread uThread = new UnifiedGarbageCollectorThread(i, queue);
	'
	'            // all GC threads should be attached to default device
	'            Nd4j.getAffinityManager().attachThreadToDevice(uThread, getDeviceId());
	'
	'            queueMap.put(i, queue);
	'
	'            uThread.start();
	'
	'            collectorsUnified.put(i, uThread);
	'            *
	'            ZeroGarbageCollectorThread zThread = new ZeroGarbageCollectorThread((long) i, shouldStop);
	'            zThread.start();
	'            
	'            collectorsZero.put((long) i, zThread);
	'            *
	'        }
	'    }

		''' <summary>
		''' This method executes garbage collectors for each special device (i.e. CUDA GPUs) present in system
		''' </summary>
		Protected Friend Overridable Sub initDeviceCollectors()
	'        
	'        for (Integer deviceId : this.memoryHandler.getAvailableDevices()) {
	'        
	'            DeviceGarbageCollectorThread dThread = new DeviceGarbageCollectorThread(deviceId, shouldStop);
	'            dThread.start();
	'            collectorsDevice.put(deviceId, dThread);
	'        }
	'        
		End Sub

		''' <summary>
		''' This method returns CudaContext for current thread
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DeviceContext As CudaContext Implements Allocator.getDeviceContext
			Get
				' FIXME: proper lock avoidance required here
				Return memoryHandler_Conflict.DeviceContext
			End Get
		End Property

		''' <summary>
		''' This method specifies Mover implementation to be used internally </summary>
		''' <param name="memoryHandler"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setMemoryHandler(@NonNull MemoryHandler memoryHandler)
		Public Overridable WriteOnly Property MemoryHandler As MemoryHandler
			Set(ByVal memoryHandler As MemoryHandler)
				globalLock.writeLock().lock()
    
				Me.memoryHandler_Conflict = memoryHandler
				Me.memoryHandler_Conflict.init(configuration_Conflict, Me)
    
				globalLock.writeLock().unlock()
			End Set
		End Property

		''' <summary>
		''' Consume and apply configuration passed in as argument
		''' 
		''' PLEASE NOTE: This method should only be used BEFORE any calculations were started.
		''' </summary>
		''' <param name="configuration"> configuration bean to be applied </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void applyConfiguration(@NonNull Configuration configuration)
		Public Overridable Sub applyConfiguration(ByVal configuration As Configuration)
			If Not wasInitialised.get() Then
				globalLock.writeLock().lock()

				Me.configuration_Conflict = configuration

				globalLock.writeLock().unlock()
			End If
		End Sub


		''' <summary>
		''' Returns current Allocator configuration
		''' </summary>
		''' <returns> current configuration </returns>
		Public Overridable ReadOnly Property Configuration As Configuration Implements Allocator.getConfiguration
			Get
				Try
					globalLock.readLock().lock()
					Return configuration_Conflict
				Finally
					globalLock.readLock().unlock()
				End Try
			End Get
		End Property


		''' <summary>
		''' This method returns actual device pointer valid for current object
		''' </summary>
		''' <param name="buffer"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.bytedeco.javacpp.Pointer getPointer(@NonNull DataBuffer buffer, org.nd4j.linalg.jcublas.context.CudaContext context)
		Public Overridable Function getPointer(ByVal buffer As DataBuffer, ByVal context As CudaContext) As Pointer
			Return memoryHandler_Conflict.getDevicePointer(buffer, context)
		End Function

		Public Overridable Function getPointer(ByVal buffer As DataBuffer) As Pointer
			Return memoryHandler_Conflict.getDevicePointer(buffer, DeviceContext)
		End Function

		''' <summary>
		''' This method returns actual device pointer valid for specified shape of current object
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="shape"> </param>
		''' <param name="isView"> </param>
		<Obsolete>
		Public Overridable Function getPointer(ByVal buffer As DataBuffer, ByVal shape As AllocationShape, ByVal isView As Boolean, ByVal context As CudaContext) As Pointer
			Return memoryHandler_Conflict.getDevicePointer(buffer, context)
		End Function

		''' <summary>
		''' This method returns actual device pointer valid for specified INDArray
		''' </summary>
		''' <param name="array"> </param>
		Public Overridable Function getPointer(ByVal array As INDArray, ByVal context As CudaContext) As Pointer Implements Allocator.getPointer
			'    DataBuffer buffer = array.data().originalDataBuffer() == null ? array.data() : array.data().originalDataBuffer();
			If array.Empty OrElse array.S Then
				Throw New System.NotSupportedException("Pew-pew")
			End If

			Return memoryHandler_Conflict.getDevicePointer(array.data(), context)
		End Function

		''' <summary>
		''' This method returns actual host pointer valid for current object
		''' </summary>
		''' <param name="array"> </param>
		Public Overridable Function getHostPointer(ByVal array As INDArray) As Pointer Implements Allocator.getHostPointer
			If array.Empty Then
				Return Nothing
			End If

			synchronizeHostData(array)
			Return memoryHandler_Conflict.getHostPointer(array.data())
		End Function

		''' <summary>
		''' This method returns actual host pointer valid for current object
		''' </summary>
		''' <param name="buffer"> </param>
		Public Overridable Function getHostPointer(ByVal buffer As DataBuffer) As Pointer Implements Allocator.getHostPointer
			Return memoryHandler_Conflict.getHostPointer(buffer)
		End Function


		''' <summary>
		''' This method should be called to make sure that data on host side is actualized
		''' </summary>
		''' <param name="array"> </param>
		Public Overridable Sub synchronizeHostData(ByVal array As INDArray) Implements Allocator.synchronizeHostData
			If array.Empty OrElse array.S Then
				Return
			End If

			Dim buffer As val = If(array.data().originalDataBuffer() Is Nothing, array.data(), array.data().originalDataBuffer())
			synchronizeHostData(buffer)
		End Sub

		''' <summary>
		''' This method should be called to make sure that data on host side is actualized
		''' </summary>
		''' <param name="buffer"> </param>
		Public Overridable Sub synchronizeHostData(ByVal buffer As DataBuffer) Implements Allocator.synchronizeHostData
			' we actually need synchronization only in device-dependant environment. no-op otherwise. managed by native code
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSyncToPrimary(DirectCast(buffer, BaseCudaDataBuffer).OpaqueDataBuffer)
		End Sub


		''' <summary>
		''' This method returns CUDA deviceId for specified buffer
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Public Overridable Function getDeviceId(ByVal array As INDArray) As Integer?
			Return getAllocationPoint(array).DeviceId
		End Function


		''' <summary>
		''' This method releases memory allocated for this allocation point </summary>
		''' <param name="point"> </param>
		Public Overridable Sub freeMemory(ByVal point As AllocationPoint)
			If point.getAllocationStatus() = AllocationStatus.DEVICE Then
				Me.MemoryHandler.getMemoryProvider().free(point)

				If point.HostPointer IsNot Nothing Then
					point.setAllocationStatus(AllocationStatus.HOST)
					Me.MemoryHandler.getMemoryProvider().free(point)
					Me.MemoryHandler.forget(point, AllocationStatus.DEVICE)
				End If
			Else
				' call it only once
				If point.HostPointer IsNot Nothing Then
					Me.MemoryHandler.getMemoryProvider().free(point)
					Me.MemoryHandler.forget(point, AllocationStatus.HOST)
				End If
			End If

			allocationsMap_Conflict.Remove(point.getObjectId())
		End Sub

		''' <summary>
		''' This method allocates required chunk of memory
		''' </summary>
		''' <param name="requiredMemory"> </param>
		Public Overridable Function allocateMemory(ByVal buffer As DataBuffer, ByVal requiredMemory As AllocationShape, ByVal initialize As Boolean) As AllocationPoint
			' by default we allocate on initial location
			Dim point As AllocationPoint = Nothing

			If configuration_Conflict.getMemoryModel() = Configuration.MemoryModel.IMMEDIATE Then
				point = allocateMemory(buffer, requiredMemory, memoryHandler_Conflict.InitialLocation, initialize)
			ElseIf configuration_Conflict.getMemoryModel() = Configuration.MemoryModel.DELAYED Then
				' for DELAYED memory model we allocate only host memory, regardless of firstMemory configuration value
				point = allocateMemory(buffer, requiredMemory, AllocationStatus.HOST, initialize)
			End If

			Return point
		End Function


		Public Overridable Function pickExternalBuffer(ByVal buffer As DataBuffer) As AllocationPoint
			''' <summary>
			''' AllocationPoint point = new AllocationPoint();
			''' Long allocId = objectsTracker.getAndIncrement();
			''' point.setObjectId(allocId);
			''' point.setConstant(true);
			''' point.setDeviceId(Nd4j.getAffinityManager().getDeviceForCurrentThread());
			''' 
			''' allocationsMap.put(allocId, point);
			''' 
			''' point.tickDeviceWrite();
			''' point.tickHostRead();
			''' 
			''' return point;
			''' </summary>

			Throw New System.NotSupportedException("Pew-pew")
		End Function

		''' <summary>
		''' This method allocates required chunk of memory in specific location
		''' <para>
		''' PLEASE NOTE: Do not use this method, unless you're 100% sure what you're doing
		''' 
		''' </para>
		''' </summary>
		''' <param name="requiredMemory"> </param>
		''' <param name="location"> </param>
		Public Overridable Function allocateMemory(ByVal buffer As DataBuffer, ByVal requiredMemory As AllocationShape, ByVal location As AllocationStatus, ByVal initialize As Boolean) As AllocationPoint
			Throw New System.NotSupportedException("Pew-pew")
		End Function


		''' <summary>
		''' This method returns AllocationPoint POJO for specified tracking ID </summary>
		''' <param name="objectId">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected AllocationPoint getAllocationPoint(@NonNull Long objectId)
		Protected Friend Overridable Function getAllocationPoint(ByVal objectId As Long) As AllocationPoint
			Return allocationsMap(objectId)
		End Function

		''' <summary>
		''' This method frees native system memory referenced by specified tracking id/AllocationPoint
		''' </summary>
		''' <param name="bucketId"> </param>
		''' <param name="objectId"> </param>
		''' <param name="point"> </param>
		''' <param name="copyback"> </param>
		Protected Friend Overridable Sub purgeZeroObject(ByVal bucketId As Long?, ByVal objectId As Long?, ByVal point As AllocationPoint, ByVal copyback As Boolean)
			allocationsMap_Conflict.Remove(objectId)

			memoryHandler_Conflict.purgeZeroObject(bucketId, objectId, point, copyback)

			'getFlowController().getEventsProvider().storeEvent(point.getLastWriteEvent());
			'getFlowController().getEventsProvider().storeEvent(point.getLastReadEvent());
		End Sub

		''' <summary>
		''' This method frees native device memory referenced by specified tracking id/AllocationPoint </summary>
		''' <param name="threadId"> </param>
		''' <param name="deviceId"> </param>
		''' <param name="objectId"> </param>
		''' <param name="point"> </param>
		''' <param name="copyback"> </param>
		Protected Friend Overridable Sub purgeDeviceObject(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal objectId As Long?, ByVal point As AllocationPoint, ByVal copyback As Boolean)
			memoryHandler_Conflict.purgeDeviceObject(threadId, deviceId, objectId, point, copyback)

			' since we can't allow java object without native memory, we explicitly specify that memory is handled using HOST memory only, after device memory is released
			'point.setAllocationStatus(AllocationStatus.HOST);

			'memoryHandler.purgeZeroObject(point.getBucketId(), point.getObjectId(), point, copyback);
		End Sub

		''' <summary>
		''' This method seeks for unused zero-copy memory allocations
		''' </summary>
		''' <param name="bucketId"> Id of the bucket, serving allocations </param>
		''' <returns> size of memory that was deallocated </returns>
		Protected Friend Overridable Function seekUnusedZero(ByVal bucketId As Long?, ByVal aggressiveness As Aggressiveness) As Long
			SyncLock Me
				Dim freeSpace As New AtomicLong(0)
        
				Dim totalElements As Integer = CInt(memoryHandler_Conflict.getAllocatedHostObjects(bucketId))
        
				' these 2 variables will contain jvm-wise memory access frequencies
				Dim shortAverage As Single = zeroShort.Average
				Dim longAverage As Single = zeroLong.Average
        
				' threshold is calculated based on agressiveness specified via configuration
				Dim shortThreshold As Single = shortAverage / (System.Enum.GetValues(GetType(Aggressiveness)).length - aggressiveness.ordinal())
				Dim longThreshold As Single = longAverage / (System.Enum.GetValues(GetType(Aggressiveness)).length - aggressiveness.ordinal())
        
				' simple counter for dereferenced objects
				Dim elementsDropped As New AtomicInteger(0)
				Dim elementsSurvived As New AtomicInteger(0)
        
				For Each [object] As Long? In memoryHandler_Conflict.getHostTrackingPoints(bucketId)
					Dim point As AllocationPoint = getAllocationPoint([object])
        
					' point can be null, if memory was promoted to device and was deleted there
					If point Is Nothing Then
						Continue For
					End If
        
					If point.getAllocationStatus() = AllocationStatus.HOST Then
						'point.getAccessState().isToeAvailable()
						'point.getAccessState().requestToe();
        
		'                
		'                    Check if memory points to non-existant buffer, using externals.
		'                    If externals don't have specified buffer - delete reference.
		'                 
						If point.Buffer Is Nothing Then
							purgeZeroObject(bucketId, [object], point, False)
							'freeSpace.addAndGet(AllocationUtils.getRequiredMemory(point.getShape()));
							Throw New System.NotSupportedException("Pew-pew")
        
							'elementsDropped.incrementAndGet();
							'continue;
						Else
							elementsSurvived.incrementAndGet()
						End If
        
						'point.getAccessState().releaseToe();
					Else
						'  log.warn("SKIPPING :(");
					End If
				Next [object]
        
        
        
				'log.debug("Short average: ["+shortAverage+"], Long average: [" + longAverage + "]");
				'log.debug("Aggressiveness: ["+ aggressiveness+"]; Short threshold: ["+shortThreshold+"]; Long threshold: [" + longThreshold + "]");
				log.debug("Zero {} elements checked: [{}], deleted: {}, survived: {}", bucketId, totalElements, elementsDropped.get(), elementsSurvived.get())
        
				Return freeSpace.get()
			End SyncLock
		End Function

		''' <summary>
		''' This method seeks for unused device memory allocations, for specified thread and device
		''' </summary>
		''' <param name="threadId"> Id of the thread, retrieved via Thread.currentThread().getId() </param>
		''' <param name="deviceId"> Id of the device </param>
		''' <returns> size of memory that was deallocated </returns>
		Protected Friend Overridable Function seekUnusedDevice(ByVal threadId As Long?, ByVal deviceId As Integer?, ByVal aggressiveness As Aggressiveness) As Long
			Dim freeSpace As New AtomicLong(0)


			'  int initialSize = allocations.size();

			' these 2 variables will contain jvm-wise memory access frequencies
			Dim shortAverage As Single = deviceShort.Average
			Dim longAverage As Single = deviceLong.Average

			' threshold is calculated based on agressiveness specified via configuration
			Dim shortThreshold As Single = shortAverage / (System.Enum.GetValues(GetType(Aggressiveness)).length - aggressiveness.ordinal())
			Dim longThreshold As Single = longAverage / (System.Enum.GetValues(GetType(Aggressiveness)).length - aggressiveness.ordinal())

			Dim elementsDropped As New AtomicInteger(0)
			Dim elementsMoved As New AtomicInteger(0)
			Dim elementsSurvived As New AtomicInteger(0)

			For Each [object] As Long? In memoryHandler_Conflict.getDeviceTrackingPoints(deviceId)
				Dim point As AllocationPoint = getAllocationPoint([object])

				'            if (point.getAccessState().isToeAvailable()) {
				'                point.getAccessState().requestToe();

	'            
	'                Check if memory points to non-existant buffer, using externals.
	'                If externals don't have specified buffer - delete reference.
	'             
				If point.Buffer Is Nothing Then
					If point.getAllocationStatus() = AllocationStatus.DEVICE Then
						' we deallocate device memory
						purgeDeviceObject(threadId, deviceId, [object], point, False)
						'freeSpace.addAndGet(AllocationUtils.getRequiredMemory(point.getShape()));

						' and we deallocate host memory, since object is dereferenced
						'purgeZeroObject(point.getBucketId(), object, point, false);

						'elementsDropped.incrementAndGet();
						'continue;
						Throw New System.NotSupportedException("Pew-pew")
					End If
				Else
					elementsSurvived.incrementAndGet()
				End If

	'            
	'                Check, if memory can be removed from allocation.
	'                To check it, we just compare average rates for few tens of latest calls
	'             
	'            
	'                long millisecondsTTL = configuration.getMinimumTTLMilliseconds();
	'                if (point.getRealDeviceAccessTime() < System.currentTimeMillis() - millisecondsTTL) {
	'                    // we could remove device allocation ONLY if it's older then minimum TTL
	'                    if (point.getTimerLong().getFrequencyOfEvents() < longThreshold && point.getTimerShort().getFrequencyOfEvents() < shortThreshold) {
	'                        //log.info("Removing object: " + object);
	'            
	'                        purgeDeviceObject(threadId, deviceId, object, point, true);
	'            
	'                        freeSpace.addAndGet(AllocationUtils.getRequiredMemory(point.getShape()));
	'            
	'                        elementsMoved.incrementAndGet();
	'            
	'                        //purgeDeviceObject(threadId, deviceId, object, point, true);
	'                    }
	'                }
	'            
				'  point.getAccessState().releaseToe();
				'}
			Next [object]

			log.debug("Thread/Device [" & threadId & "/" & deviceId & "] elements purged: [" & elementsDropped.get() & "]; Relocated: [" & elementsMoved.get() & "]; Survivors: [" & elementsSurvived.get() & "]")

			Return freeSpace.get()
		End Function

	'    private class UnifiedGarbageCollectorThread extends Thread implements Runnable {
	'        private final ReferenceQueue<BaseDataBuffer> queue;
	'        private int threadId;
	'        private int deviceId;
	'        private AtomicLong stopper = new AtomicLong(System.currentTimeMillis());
	'
	'        public UnifiedGarbageCollectorThread(Integer threadId, @NonNull ReferenceQueue<BaseDataBuffer> queue) {
	'            this.queue = queue;
	'            this.setDaemon(true);
	'            this.setName("UniGC thread " + threadId);
	'            this.threadId = threadId;
	'        }
	'
	'        @Override
	'        public void run() {
	'            while (true) {
	'                try {
	'                    GarbageBufferReference reference = threadId == 0 ? (GarbageBufferReference) queue.poll() : (GarbageBufferReference) queue.remove();
	'                    if (reference != null) {
	'                        AllocationPoint point = reference.getPoint();
	'
	'                        // skipping any allocation that is coming from workspace
	'                        if (point.isAttached()) {
	'                            // TODO: remove allocation point as well?
	'                            if (!allocationsMap.containsKey(point.getObjectId()))
	'                                throw new RuntimeException();
	'
	'                            getFlowController().waitTillReleased(point);
	'
	'                            getFlowController().getEventsProvider().storeEvent(point.getLastWriteEvent());
	'                            getFlowController().getEventsProvider().storeEvent(point.getLastReadEvent());
	'
	'                            allocationsMap.remove(point.getObjectId());
	'
	'                            continue;
	'                        }
	'
	'                        if (threadId == 0)
	'                            stopper.set(System.currentTimeMillis());
	'
	'                        //log.info("Purging {} bytes...", AllocationUtils.getRequiredMemory(point.getShape()));
	'
	'                        if (point.getAllocationStatus() == AllocationStatus.HOST) {
	'                            purgeZeroObject(point.getBucketId(), point.getObjectId(), point, false);
	'                        } else if (point.getAllocationStatus() == AllocationStatus.DEVICE) {
	'                            purgeDeviceObject(0L, point.getDeviceId(), point.getObjectId(), point, false);
	'
	'                            // and we deallocate host memory, since object is dereferenced
	'                            purgeZeroObject(point.getBucketId(), point.getObjectId(), point, false);
	'                        }
	'
	'                    } else {
	'                        try {
	'                            if (threadId == 0) {
	'                                // we don't call for System.gc if last memory allocation was more then 3 seconds ago
	'                                if (Nd4j.getMemoryManager().isPeriodicGcActive()) {
	'                                    long ct = System.currentTimeMillis();
	'                                    if (useTracker.get() > ct - 3000 && ct > Nd4j.getMemoryManager().getLastGcTime() + Nd4j.getMemoryManager().getAutoGcWindow()) {
	'                                        Nd4j.getMemoryManager().invokeGc();
	'                                    } else {
	'                                        LockSupport.parkNanos(50000L);
	'                                    }
	'                                } else {
	'                                    LockSupport.parkNanos(50000L);
	'                                }
	'                            }
	'                        } catch (Exception e) {
	'
	'                        }
	'                    }
	'                } catch (InterruptedException e) {
	'                    // do nothing
	'                }
	'            }
	'        }
	'    }

		''' <summary>
		''' This class implements garbage collector for memory allocated on host system.
		''' 
		'''  There's only 1 possible reason of deallocation event: object that reference some memory chunk was removed by JVM gc.
		''' </summary>
		Private Class ZeroGarbageCollectorThread
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As AtomicAllocator


			Friend ReadOnly bucketId As Long?
			Friend ReadOnly terminate As AtomicBoolean

			Public Sub New(ByVal outerInstance As AtomicAllocator, ByVal bucketId As Long?, ByVal terminate As AtomicBoolean)
				Me.outerInstance = outerInstance
				Me.bucketId = bucketId
				Me.terminate = terminate

				Me.setName("zero gc thread " & bucketId)
				Me.setDaemon(True)
			End Sub

			Public Overrides Sub run()
				log.debug("Starting zero GC for thread: " & bucketId)
				Dim lastCheck As Long = DateTimeHelper.CurrentUnixTimeMillis()
				Do While Not terminate.get()

	'                
	'                    Check for zero-copy garbage
	'                 
					'   log.info("ZeroGC started...");
	'                
	'                    We want allocations to take in account multiple things:
	'                    1. average access rates for last X objects
	'                    2. total number of currently allocated objects
	'                    3. total allocated memory size
	'                    4. desired aggressiveness
	'                
					Try
						Thread.Sleep(Math.Max(outerInstance.configuration_Conflict.getMinimumTTLMilliseconds(), 10000))
						'if (bucketId == 0)
							'System.gc();
					Catch e As Exception
						' we can have interruption here, to force gc
					End Try

					Dim aggressiveness As Aggressiveness = outerInstance.configuration_Conflict.getHostDeallocAggressiveness()

					' if we have too much objects, or total allocated memory has met 75% of max allocation - use urgent mode
					If (outerInstance.memoryHandler_Conflict.getAllocatedHostObjects(bucketId) > 500000 OrElse outerInstance.memoryHandler_Conflict.AllocatedHostMemory > (outerInstance.configuration_Conflict.getMaximumZeroAllocation() * 0.75)) AndAlso aggressiveness.ordinal() < Aggressiveness.URGENT.ordinal() Then
						aggressiveness = Aggressiveness.URGENT
					End If

					If outerInstance.memoryHandler_Conflict.AllocatedHostMemory > (outerInstance.configuration_Conflict.getMaximumZeroAllocation() * 0.85) Then
						aggressiveness = Aggressiveness.IMMEDIATE
					End If

					If outerInstance.memoryHandler_Conflict.AllocatedHostMemory < (outerInstance.configuration_Conflict.getMaximumZeroAllocation() * 0.25) AndAlso (outerInstance.memoryHandler_Conflict.getAllocatedHostObjects(bucketId) < 5000) AndAlso lastCheck > DateTimeHelper.CurrentUnixTimeMillis() - 30000 Then
 ' i don't want deallocation to be fired on lower thresholds. just no sense locking stuff
						  'log.debug("Skipping zero GC round: ["+zeroUseCounter.get()+"/" +zeroAllocations.get(threadId).size() + "]");
					Else
						outerInstance.seekUnusedZero(bucketId, aggressiveness)
						lastCheck = DateTimeHelper.CurrentUnixTimeMillis()
					End If
				Loop
			End Sub
		End Class

		''' <summary>
		''' This class implements garbage collection for memory regions allocated on devices.
		''' For each device 1 thread is launched.
		''' 
		''' There's 2 basic reasons for deallocation:
		'''  1. Memory isn't used anymore. I.e. INDArray object referencing specific memory chunk was removed by JVM gc.
		'''  2. Memory wasn't used for quite some time.
		''' </summary>
		Private Class DeviceGarbageCollectorThread
			Inherits Thread
			Implements ThreadStart

			Private ReadOnly outerInstance As AtomicAllocator


			Friend ReadOnly deviceId As Integer?
			Friend ReadOnly terminate As AtomicBoolean

			Public Sub New(ByVal outerInstance As AtomicAllocator, ByVal deviceId As Integer?, ByVal terminate As AtomicBoolean)
				Me.outerInstance = outerInstance
				Me.deviceId = deviceId
				Me.terminate = terminate
				Me.setName("device gc thread [" & deviceId & "]")
				Me.setDaemon(True)
			End Sub

			Public Overrides Sub run()
				log.info("Starting device GC for device: " & deviceId)
				Dim lastCheck As Long = DateTimeHelper.CurrentUnixTimeMillis()
				Do While Not terminate.get()
	'                
	'                    Check for device garbage
	'                 

					Try
						Thread.Sleep(Math.Max(outerInstance.configuration_Conflict.getMinimumTTLMilliseconds(), 5000))
					Catch e As Exception
						' we can have interruption here, to force gc

					End Try

					'log.info("DeviceGC started...");
					Dim aggressiveness As Aggressiveness = outerInstance.configuration_Conflict.getGpuDeallocAggressiveness()

					' if we have too much objects, or total allocated memory has met 75% of max allocation - use urgent mode
					If (outerInstance.memoryHandler_Conflict.getAllocatedDeviceObjects(deviceId) > 100000 OrElse outerInstance.memoryHandler_Conflict.getAllocatedDeviceMemory(deviceId) > (outerInstance.configuration_Conflict.getMaximumDeviceAllocation() * 0.75)) AndAlso aggressiveness.ordinal() < Aggressiveness.URGENT.ordinal() Then
						aggressiveness = Aggressiveness.URGENT
					End If

					If outerInstance.memoryHandler_Conflict.getAllocatedDeviceMemory(deviceId) > (outerInstance.configuration_Conflict.getMaximumDeviceAllocation() * 0.85) Then
						aggressiveness = Aggressiveness.IMMEDIATE
					End If

					If outerInstance.memoryHandler_Conflict.getAllocatedDeviceMemory(deviceId) < (outerInstance.configuration_Conflict.getMaximumDeviceAllocation() * 0.25) AndAlso (outerInstance.memoryHandler_Conflict.getAllocatedDeviceObjects(deviceId) < 500) AndAlso lastCheck > DateTimeHelper.CurrentUnixTimeMillis() - 30000 Then
						' i don't want deallocation to be fired on lower thresholds. just no sense locking stuff
					Else
						outerInstance.seekUnusedDevice(0L, Me.deviceId, aggressiveness)
						lastCheck = DateTimeHelper.CurrentUnixTimeMillis()
					End If


				Loop
			End Sub
		End Class


		''' <summary>
		''' This method returns the number of tracked zero-copy allocations
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property TotalAllocatedHostMemory As Long
			Get
				Return 0L ' memoryHandler.getAllocationStatistics().row(AllocationStatus.HOST).get(0);
			End Get
		End Property

		''' <summary>
		''' This method returns the number of all tracked memory chunks
		''' 
		''' @return
		''' </summary>
		Protected Friend Overridable ReadOnly Property TotalTrackingPoints As Integer
			Get
				Return allocationsMap_Conflict.Count
			End Get
		End Property

		''' <summary>
		''' This method returns total amount of memory allocated on specified device
		''' </summary>
		''' <param name="deviceId">
		''' @return </param>
		Public Overridable Function getTotalAllocatedDeviceMemory(ByVal deviceId As Integer?) As Long
			Return 0L '; memoryHandler.getAllocationStatistics().row(AllocationStatus.DEVICE).get(deviceId);
		End Function

		''' <summary>
		''' This method implements asynchronous memcpy, if that's available on current hardware
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcPointer"> </param>
		''' <param name="length"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub memcpyAsync(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long) Implements Allocator.memcpyAsync
			'        if (dstBuffer.isConstant()) {
			'            this.memoryHandler.memcpySpecial(dstBuffer, srcPointer, length, dstOffset);
			'        } else
			Me.memoryHandler_Conflict.memcpyAsync(dstBuffer, srcPointer, length, dstOffset)
		End Sub

		Public Overridable Sub memcpySpecial(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long) Implements Allocator.memcpySpecial
			Me.memoryHandler_Conflict.memcpySpecial(dstBuffer, srcPointer, length, dstOffset)
		End Sub

		Public Overridable Sub memcpyDevice(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long, ByVal context As CudaContext) Implements Allocator.memcpyDevice
			Me.memoryHandler_Conflict.memcpyDevice(dstBuffer, srcPointer, length, dstOffset, context)
		End Sub

		''' <summary>
		''' This method implements blocking memcpy
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcPointer"> </param>
		''' <param name="length"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub memcpyBlocking(ByVal dstBuffer As DataBuffer, ByVal srcPointer As Pointer, ByVal length As Long, ByVal dstOffset As Long) Implements Allocator.memcpyBlocking
			Me.memoryHandler_Conflict.memcpyBlocking(dstBuffer, srcPointer, length, dstOffset)
		End Sub

		''' <summary>
		''' This method implements blocking memcpy
		''' </summary>
		''' <param name="dstBuffer"> </param>
		''' <param name="srcBuffer"> </param>
		Public Overridable Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer) Implements Allocator.memcpy
			Me.memoryHandler_Conflict.memcpy(dstBuffer, srcBuffer)
		End Sub

		Public Overridable Sub tickHostWrite(ByVal buffer As DataBuffer) Implements Allocator.tickHostWrite
			getAllocationPoint(buffer).tickHostWrite()
		End Sub

		Public Overridable Sub tickHostWrite(ByVal array As INDArray) Implements Allocator.tickHostWrite
			getAllocationPoint(array.data()).tickHostWrite()
		End Sub

		Public Overridable Sub tickDeviceWrite(ByVal array As INDArray) Implements Allocator.tickDeviceWrite
			getAllocationPoint(array.data()).tickDeviceWrite()
		End Sub

		Public Overridable Function getAllocationPoint(ByVal array As INDArray) As AllocationPoint
			Return getAllocationPoint(array.data())
		End Function

		Public Overridable Function getAllocationPoint(ByVal buffer As DataBuffer) As AllocationPoint
			Return DirectCast(buffer, BaseCudaDataBuffer).getAllocationPoint()
		End Function

		''' <summary>
		''' This method returns deviceId for current thread
		''' All values >= 0 are considered valid device IDs, all values < 0 are considered stubs.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DeviceId As Integer? Implements Allocator.getDeviceId
			Get
				Return memoryHandler_Conflict.getDeviceId()
			End Get
		End Property

		''' <summary>
		''' Returns <seealso cref="getDeviceId()"/> wrapped as a <seealso cref="Pointer"/>. </summary>
		Public Overridable ReadOnly Property DeviceIdPointer As Pointer Implements Allocator.getDeviceIdPointer
			Get
				Return New CudaPointer(getDeviceId())
			End Get
		End Property

		Public Overridable Sub registerAction(ByVal context As CudaContext, ByVal result As INDArray, ParamArray ByVal operands() As INDArray) Implements Allocator.registerAction
			memoryHandler_Conflict.registerAction(context, result, operands)
		End Sub

		Public Overridable ReadOnly Property FlowController As FlowController Implements Allocator.getFlowController
			Get
				Return memoryHandler_Conflict.FlowController
			End Get
		End Property

		Public Overridable Function getConstantBuffer(ByVal array() As Integer) As DataBuffer Implements Allocator.getConstantBuffer
			Return Nd4j.ConstantHandler.getConstantBuffer(array, DataType.INT)
		End Function

		Public Overridable Function getConstantBuffer(ByVal array() As Single) As DataBuffer Implements Allocator.getConstantBuffer
			Return Nd4j.ConstantHandler.getConstantBuffer(array, DataType.FLOAT)
		End Function

		Public Overridable Function getConstantBuffer(ByVal array() As Double) As DataBuffer Implements Allocator.getConstantBuffer
			Return Nd4j.ConstantHandler.getConstantBuffer(array, DataType.DOUBLE)
		End Function

		Public Overridable Function moveToConstant(ByVal dataBuffer As DataBuffer) As DataBuffer Implements Allocator.moveToConstant
			Nd4j.ConstantHandler.moveToConstantSpace(dataBuffer)
			Return dataBuffer
		End Function
	End Class

End Namespace