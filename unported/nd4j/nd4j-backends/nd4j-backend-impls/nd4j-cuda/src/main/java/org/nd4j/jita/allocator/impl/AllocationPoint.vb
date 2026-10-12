Imports System
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports GarbageBufferReference = org.nd4j.jita.allocator.garbage.GarbageBufferReference
Imports cudaEvent_t = org.nd4j.jita.allocator.pointers.cuda.cudaEvent_t
Imports TimeProvider = org.nd4j.jita.allocator.time.TimeProvider
Imports OperativeProvider = org.nd4j.jita.allocator.time.providers.OperativeProvider
Imports BaseDataBuffer = org.nd4j.linalg.api.buffer.BaseDataBuffer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports OpaqueDataBuffer = org.nd4j.nativeblas.OpaqueDataBuffer
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
	''' This class describes top-level allocation unit.
	''' Every buffer passed into CUDA wii have allocation point entry, describing allocation state.
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	' DO NOT EVER MAKE THIS CLASS SERIALIZABLE.
	Public Class AllocationPoint
		Private Shared log As Logger = LoggerFactory.getLogger(GetType(AllocationPoint))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.nativeblas.OpaqueDataBuffer ptrDataBuffer;
		Private ptrDataBuffer As OpaqueDataBuffer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private System.Nullable<Long> objectId;
		Private objectId As Long?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private System.Nullable<Long> bucketId;
		Private bucketId As Long?

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private boolean isAttached = false;
		Private isAttached As Boolean = False

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private volatile boolean released = false;
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private released As Boolean = False

		' thread safety is guaranteed by allocLock
'JAVA TO VB CONVERTER NOTE: The field allocationStatus was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private allocationStatus_Conflict As AllocationStatus = AllocationStatus.UNDEFINED

		<NonSerialized>
		Private timeProvider As TimeProvider = New OperativeProvider()

		' corresponding access times in TimeProvider quants
		Private accessHostRead As Long = 0L
		Private accessDeviceRead As Long = 0L

		Private accessHostWrite As Long = 0L
		Private accessDeviceWrite As Long = 0L

		Protected Friend Shared ReadOnly nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
	'
	'    @Getter
	'    @Setter
	'    protected volatile cudaEvent_t writeLane;
	'
	'    @Getter
	'    protected Queue<cudaEvent_t> readLane = new ConcurrentLinkedQueue<>();
	'
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private boolean constant;
		Private constant As Boolean

	'    
	'     device, where memory was/will be allocated.
	'    Valid integer >= 0 is deviceId, null for undefined
	'    
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile int deviceId;
'JAVA TO VB CONVERTER NOTE: The field deviceId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private deviceId_Conflict As Integer

		Private bytes As Long

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AllocationPoint(@NonNull OpaqueDataBuffer opaqueDataBuffer, long bytes)
		Public Sub New(ByVal opaqueDataBuffer As OpaqueDataBuffer, ByVal bytes As Long)
			ptrDataBuffer = opaqueDataBuffer
			Me.bytes = bytes
			objectId = Nd4j.DeallocatorService.nextValue()
		End Sub

		Public Overridable Sub setPointers(ByVal primary As Pointer, ByVal special As Pointer, ByVal numberOfElements As Long)
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSetPrimaryBuffer(ptrDataBuffer, primary, numberOfElements)
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSetSpecialBuffer(ptrDataBuffer, special, numberOfElements)
		End Sub

		Public Overridable Property DeviceId As Integer
			Get
				Return ptrDataBuffer.deviceId()
			End Get
			Set(ByVal deviceId As Integer)
				NativeOpsHolder.Instance.getDeviceNativeOps().dbSetDeviceId(ptrDataBuffer, deviceId)
			End Set
		End Property


'JAVA TO VB CONVERTER NOTE: The field enqueued was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private enqueued_Conflict As New AtomicBoolean(False)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.jita.allocator.pointers.cuda.cudaEvent_t lastWriteEvent;
		Private lastWriteEvent As cudaEvent_t

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.jita.allocator.pointers.cuda.cudaEvent_t lastReadEvent;
		Private lastReadEvent As cudaEvent_t

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile org.nd4j.linalg.jcublas.context.CudaContext currentContext;
'JAVA TO VB CONVERTER NOTE: The field currentContext was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private currentContext_Conflict As CudaContext

		Public Overridable ReadOnly Property Enqueued As Boolean
			Get
				Return enqueued_Conflict.get()
			End Get
		End Property

		Public Overridable Sub markEnqueued(ByVal reallyEnqueued As Boolean)
			enqueued_Conflict.set(reallyEnqueued)
		End Sub

		Public Overridable Property CurrentContext As CudaContext
			Get
				SyncLock Me
					Return currentContext_Conflict
				End SyncLock
			End Get
			Set(ByVal context As CudaContext)
				SyncLock Me
					Me.currentContext_Conflict = context
				End SyncLock
			End Set
		End Property


		Public Overridable ReadOnly Property NumberOfBytes As Long
			Get
				Return bytes
			End Get
		End Property

	'    
	'    public void addReadLane(cudaEvent_t event) {
	'        readLane.add(event);
	'    }
	'    

		''' <summary>
		''' This method stores WeakReference to original BaseCudaDataBuffer
		''' </summary>
		''' <param name="buffer"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void attachBuffer(@NonNull BaseDataBuffer buffer)
		Public Overridable Sub attachBuffer(ByVal buffer As BaseDataBuffer)
			'originalDataBufferReference = new WeakReference<BaseDataBuffer>(buffer);
		End Sub

		Public Overridable Sub attachReference(ByVal reference As GarbageBufferReference)
			'garbageBufferReference = reference;
		End Sub

		''' <summary>
		''' This method returns previously stored BaseCudaDataBuffer instance
		''' 
		''' PLEASE NOTE: Return value CAN be null
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Buffer As DataBuffer
			Get
				'if (originalDataBufferReference != null) {
				'    return originalDataBufferReference.get();
				'} else
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' This method returns current AllocationStatus for this point
		''' @return
		''' </summary>
		Public Overridable Property AllocationStatus As AllocationStatus
			Get
				Return allocationStatus_Conflict
			End Get
			Set(ByVal status As AllocationStatus)
				allocationStatus_Conflict = status
			End Set
		End Property


		''' <summary>
		''' This method returns CUDA pointer object for this allocation.
		''' It can be either device pointer or pinned memory pointer, or null.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DevicePointer As Pointer
			Get
				Return NativeOpsHolder.Instance.getDeviceNativeOps().dbSpecialBuffer(ptrDataBuffer)
			End Get
		End Property

		''' <summary>
		''' This method returns CUDA pointer object for this allocation.
		''' It can be either device pointer or pinned memory pointer, or null.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property HostPointer As Pointer
			Get
				Return NativeOpsHolder.Instance.getDeviceNativeOps().dbPrimaryBuffer(ptrDataBuffer)
			End Get
		End Property


		Public Overridable Sub tickDeviceRead()
			SyncLock Me
				NativeOpsHolder.Instance.getDeviceNativeOps().dbTickDeviceRead(ptrDataBuffer)
			End SyncLock
		End Sub

		''' <summary>
		''' Returns time, in milliseconds, when this point was accessed on device side
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DeviceAccessTime As Long
			Get
				SyncLock Me
					Return accessDeviceRead
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns time when point was written on device last time
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DeviceWriteTime As Long
			Get
				SyncLock Me
					Return accessDeviceWrite
				End SyncLock
			End Get
		End Property

		Public Overridable Sub tickHostRead()
			SyncLock Me
				NativeOpsHolder.Instance.getDeviceNativeOps().dbTickHostRead(ptrDataBuffer)
			End SyncLock
		End Sub

		''' <summary>
		''' This method sets time when this point was changed on device
		''' 
		''' </summary>
		Public Overridable Sub tickDeviceWrite()
			SyncLock Me
				NativeOpsHolder.Instance.getDeviceNativeOps().dbTickDeviceWrite(ptrDataBuffer)
			End SyncLock
		End Sub

		''' <summary>
		''' This method sets time when this point was changed on host
		''' </summary>
		Public Overridable Sub tickHostWrite()
			SyncLock Me
				NativeOpsHolder.Instance.getDeviceNativeOps().dbTickHostWrite(ptrDataBuffer)
			End SyncLock
		End Sub

		''' <summary>
		''' This method returns, if host side has actual copy of data
		''' </summary>
		''' <returns> true, if data is actual, false otherwise </returns>
		Public Overridable ReadOnly Property ActualOnHostSide As Boolean
			Get
				SyncLock Me
					Dim s As val = NativeOpsHolder.Instance.getDeviceNativeOps().dbLocality(ptrDataBuffer)
					Return s <= 0
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' This method returns, if device side has actual copy of data
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ActualOnDeviceSide As Boolean
			Get
				SyncLock Me
					Dim s As val = NativeOpsHolder.Instance.getDeviceNativeOps().dbLocality(ptrDataBuffer)
					Return s >= 0
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' This method sets device access time equal to host write time
		''' </summary>
		Public Overridable Sub tickDeviceToHost()
			SyncLock Me
				accessDeviceRead = (accessHostRead)
			End SyncLock
		End Sub

		Public Overrides Function ToString() As String
			Return "AllocationPoint{" & "deviceId=" & deviceId_Conflict & ", objectId=" & objectId & "}"
		End Function
	End Class

End Namespace