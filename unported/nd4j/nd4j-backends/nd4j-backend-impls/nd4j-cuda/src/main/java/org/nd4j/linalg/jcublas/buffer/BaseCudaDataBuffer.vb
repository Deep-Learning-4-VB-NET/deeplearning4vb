Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports CudaConstants = org.nd4j.jita.allocator.enums.CudaConstants
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaDeallocator = org.nd4j.jita.allocator.impl.CudaDeallocator
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports BaseDataBuffer = org.nd4j.linalg.api.buffer.BaseDataBuffer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports Deallocatable = org.nd4j.linalg.api.memory.Deallocatable
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports MirroringPolicy = org.nd4j.linalg.api.memory.enums.MirroringPolicy
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports LongUtils = org.nd4j.linalg.util.LongUtils
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

Namespace org.nd4j.linalg.jcublas.buffer


	''' <summary>
	''' Base class for a data buffer
	''' 
	''' CUDA implementation for DataBuffer always uses JavaCPP
	''' as allocationMode, and device access is masked by
	''' appropriate allocator mover implementation.
	''' 
	''' Memory allocation/deallocation is strictly handled by allocator,
	''' since JavaCPP alloc/dealloc has nothing to do with CUDA.
	''' But besides that, host pointers obtained from CUDA are 100%
	''' compatible with CPU
	''' 
	''' @author Adam Gibson
	''' @author raver119@gmail.com
	''' </summary>
	<Serializable>
	Public MustInherit Class BaseCudaDataBuffer
		Inherits BaseDataBuffer
		Implements JCudaBuffer, Deallocatable

		Protected Friend ptrDataBuffer As OpaqueDataBuffer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient volatile org.nd4j.jita.allocator.impl.AllocationPoint allocationPoint;
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized>
		Protected Friend allocationPoint As AllocationPoint

		Private Shared allocator As AtomicAllocator = AtomicAllocator.Instance

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(BaseCudaDataBuffer))

		Protected Friend globalType As DataType = DataTypeUtil.DtypeFromContext

		Public Sub New()

		End Sub

		Public Overridable ReadOnly Property OpaqueDataBuffer As OpaqueDataBuffer
			Get
				If released Then
					Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
				End If
    
				Return ptrDataBuffer
			End Get
		End Property


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseCudaDataBuffer(@NonNull Pointer pointer, @NonNull Pointer specialPointer, @NonNull Indexer indexer, long length)
		Public Sub New(ByVal pointer As Pointer, ByVal specialPointer As Pointer, ByVal indexer As Indexer, ByVal length As Long)
			Me.allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES

			Me.indexer_Conflict = indexer

			Me.offset_Conflict = 0
			Me.originalOffset_Conflict = 0
			Me.underlyingLength_Conflict = length
			Me.length_Conflict = length

			initTypeAndSize()

			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(length, Me.type, pointer, specialPointer)
			Me.allocationPoint = New AllocationPoint(ptrDataBuffer, Me.type.width() * length)

			Nd4j.DeallocatorService.pickObject(Me)
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If
		End Sub

		''' <summary>
		''' Meant for creating another view of a buffer
		''' </summary>
		''' <param name="pointer"> the underlying buffer to create a view from </param>
		''' <param name="indexer"> the indexer for the pointer </param>
		''' <param name="length">  the length of the view </param>
		Public Sub New(ByVal pointer As Pointer, ByVal indexer As Indexer, ByVal length As Long)
			MyBase.New(pointer, indexer, length)

			' allocating interop buffer
			Me.ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(length, type, False)

			' passing existing pointer to native holder
			Me.ptrDataBuffer.setPrimaryBuffer(pointer, length)

			'cuda specific bits
			Me.allocationPoint = New AllocationPoint(ptrDataBuffer, length * elementSize_Conflict)
			Nd4j.DeallocatorService.pickObject(Me)

			' now we're getting context and copying our stuff to device
			Dim context As val = AtomicAllocator.Instance.DeviceContext

			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(allocationPoint.DevicePointer, pointer, length * ElementSize, CudaConstants.cudaMemcpyHostToDevice, context.getSpecialStream())

			PerformanceTracker.Instance.helperRegisterTransaction(allocationPoint.DeviceId, perfD / 2, allocationPoint.NumberOfBytes, MemcpyDirection.HOST_TO_DEVICE)
			context.getSpecialStream().synchronize()
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean)
			'super(data, copy);
			Me.New(data, copy, 0)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			'super(data, copy);
			Me.New(data, copy, 0, workspace)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long)
			Me.New(data.Length, 4, False)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
			set(data, Me.length_Conflict, offset, offset)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			Me.New(data.Length, 8, False, workspace)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
			set(data, Me.length_Conflict, offset, offset)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			Me.New(data.Length, 4,False, workspace)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
			set(data, Me.length_Conflict, offset, offset)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean)
			Me.New(data, copy, 0)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long)
			Me.New(data.Length, 8, False)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
			set(data, Me.length_Conflict, offset, offset)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean)
			Me.New(data, copy, 0)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			Me.New(data, copy, 0, workspace)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal offset As Long)
			Me.New(data.Length, 4, False)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
			set(data, Me.length_Conflict, offset, offset)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			Me.New(data.Length, 4, False, workspace)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
			set(data, Me.length_Conflict, offset, offset)
		End Sub

		Protected Friend Overridable Sub initPointers(ByVal length As Long, ByVal dtype As DataType, ByVal initialize As Boolean)
			initPointers(length, Nd4j.sizeOfDataType(dtype), initialize)
		End Sub

		Public Overridable Sub lazyAllocateHostPointer()
			If length() = 0 Then
				Return
			End If

			' java side might be unaware of native-side buffer allocation
			If Me.indexer_Conflict Is Nothing OrElse Me.pointer_Conflict Is Nothing OrElse Me.pointer_Conflict.address() = 0 Then
				initHostPointerAndIndexer()
			ElseIf allocationPoint.HostPointer IsNot Nothing AndAlso allocationPoint.HostPointer.address() <> Me.pointer_Conflict.address() Then
				initHostPointerAndIndexer()
			End If
		End Sub

		Protected Friend Sub New(ByVal buffer As ByteBuffer, ByVal dtype As DataType, ByVal length As Long, ByVal offset As Long)
			Me.New(length, Nd4j.sizeOfDataType(dtype))

			Dim temp As Pointer = Nothing

			Select Case dataType()
				Case [DOUBLE]
					temp = New DoublePointer(buffer.asDoubleBuffer())
				Case FLOAT
					temp = New FloatPointer(buffer.asFloatBuffer())
				Case HALF
					temp = New ShortPointer(buffer.asShortBuffer())
				Case [LONG]
					temp = New LongPointer(buffer.asLongBuffer())
				Case INT
					temp = New IntPointer(buffer.asIntBuffer())
				Case [SHORT]
					temp = New ShortPointer(buffer.asShortBuffer())
				Case UBYTE, [BYTE] 'Fall through
					temp = New BytePointer(buffer)
				Case BOOL
					temp = New BooleanPointer(Me.length())
				Case UTF8
					temp = New BytePointer(Me.length())
				Case BFLOAT16
					temp = New ShortPointer(Me.length())
				Case UINT16
					temp = New ShortPointer(Me.length())
				Case UINT32
					temp = New IntPointer(Me.length())
				Case UINT64
					temp = New LongPointer(Me.length())
			End Select

			' copy data to device
			Dim stream As val = AtomicAllocator.Instance.DeviceContext.getSpecialStream()
			Dim ptr As val = ptrDataBuffer.specialBuffer()

			If offset > 0 Then
				temp = New PagedPointer(temp.address() + offset * ElementSize)
			End If

			NativeOpsHolder.Instance.getDeviceNativeOps().memcpyAsync(ptr, temp, length * Nd4j.sizeOfDataType(dtype), CudaConstants.cudaMemcpyHostToDevice, stream)
			stream.synchronize()

			' mark device buffer as updated
			allocationPoint.tickDeviceWrite()
		End Sub

		Protected Friend Overridable Sub initHostPointerAndIndexer()
			If length() = 0 Then
				Return
			End If

			If allocationPoint.HostPointer Is Nothing Then
				Dim location As val = allocationPoint.getAllocationStatus()
				If parentWorkspace_Conflict Is Nothing Then
					' let cpp allocate primary buffer
					NativeOpsHolder.Instance.getDeviceNativeOps().dbAllocatePrimaryBuffer(ptrDataBuffer)
				Else
					'log.info("ws alloc step");
					Dim ptr As val = parentWorkspace_Conflict.alloc(Me.length_Conflict * Me.elementSize_Conflict, MemoryKind.HOST, Me.dataType(), False)
					ptrDataBuffer.setPrimaryBuffer(ptr, Me.length_Conflict)
				End If
				Me.allocationPoint.setAllocationStatus(location)
				Me.allocationPoint.tickDeviceWrite()
			End If

			Dim hostPointer As val = allocationPoint.HostPointer

			Debug.Assert(hostPointer IsNot Nothing)

			Select Case dataType()
				Case [DOUBLE]
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asDoublePointer()
					indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
				Case FLOAT
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asFloatPointer()
					indexer_Conflict = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
				Case UINT32
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asIntPointer()
					indexer_Conflict = UIntIndexer.create(CType(pointer_Conflict, IntPointer))
				Case INT
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asIntPointer()
					indexer_Conflict = IntIndexer.create(CType(pointer_Conflict, IntPointer))
				Case BFLOAT16
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asShortPointer()
					indexer_Conflict = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))
				Case HALF
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asShortPointer()
					indexer_Conflict = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))
				Case UINT64, [LONG] 'Fall through
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asLongPointer()
					indexer_Conflict = LongIndexer.create(CType(pointer_Conflict, LongPointer))
				Case UINT16
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asShortPointer()
					indexer_Conflict = UShortIndexer.create(CType(pointer_Conflict, ShortPointer))
				Case [SHORT]
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asShortPointer()
					indexer_Conflict = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
				Case UBYTE
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asBytePointer()
					indexer_Conflict = UByteIndexer.create(CType(pointer_Conflict, BytePointer))
				Case [BYTE]
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asBytePointer()
					indexer_Conflict = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
				Case BOOL
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asBooleanPointer()
					indexer_Conflict = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
				Case UTF8
					Me.pointer_Conflict = (New CudaPointer(hostPointer, length_Conflict, 0)).asBytePointer()
					indexer_Conflict = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
				Case Else
					Throw New System.NotSupportedException()
			End Select
		End Sub

		Protected Friend Overridable Sub initPointers(ByVal length As Long, ByVal elementSize As Integer, ByVal initialize As Boolean)
			Me.allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES
			Me.length_Conflict = length
			Me.elementSize_Conflict = CSByte(elementSize)

			Me.offset_Conflict = 0
			Me.originalOffset_Conflict = 0

			' we allocate native DataBuffer AND it will contain our device pointer
			ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(length, type, False)
			Me.allocationPoint = New AllocationPoint(ptrDataBuffer, length * type.width())

			If initialize Then
				Dim ctx As val = AtomicAllocator.Instance.DeviceContext
				Dim devicePtr As val = allocationPoint.DevicePointer
				NativeOpsHolder.Instance.getDeviceNativeOps().memsetAsync(devicePtr, 0, length * elementSize, 0, ctx.getSpecialStream())
				ctx.getSpecialStream().synchronize()
			End If

			' let deallocator pick up this object
			Nd4j.DeallocatorService.pickObject(Me)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal initialize As Boolean)
			initTypeAndSize()
			initPointers(length, elementSize, initialize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseCudaDataBuffer(long length, int elementSize, boolean initialize, @NonNull MemoryWorkspace workspace)
		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			Me.allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES
			initTypeAndSize()

			Me.attached_Conflict = True
			Me.parentWorkspace_Conflict = workspace

			Me.length_Conflict = length

			Me.offset_Conflict = 0
			Me.originalOffset_Conflict = 0

			If workspace.getWorkspaceConfiguration().getPolicyMirroring() = MirroringPolicy.FULL Then
				Dim devicePtr As val = workspace.alloc(length * elementSize, MemoryKind.DEVICE, type, initialize)

				' allocate from workspace, and pass it  to native DataBuffer
				ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(Me.length_Conflict, type, Nothing, devicePtr)

				If initialize Then
					Dim ctx As val = AtomicAllocator.Instance.DeviceContext
					NativeOpsHolder.Instance.getDeviceNativeOps().memsetAsync(devicePtr, 0, length * elementSize, 0, ctx.getSpecialStream())
					ctx.getSpecialStream().synchronize()
				End If
			Else
				' we can register this pointer as device, because it's pinned memory
				Dim devicePtr As val = workspace.alloc(length * elementSize, MemoryKind.HOST, type, initialize)
				ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(Me.length_Conflict, type, Nothing, devicePtr)

				If initialize Then
					Dim ctx As val = AtomicAllocator.Instance.DeviceContext
					NativeOpsHolder.Instance.getDeviceNativeOps().memsetAsync(devicePtr, 0, length * elementSize, 0, ctx.getSpecialStream())
					ctx.getSpecialStream().synchronize()
				End If
			End If

			Me.allocationPoint = New AllocationPoint(ptrDataBuffer, elementSize * length)

			' registering for deallocation
			Nd4j.DeallocatorService.pickObject(Me)

			workspaceGenerationId = workspace.getGenerationId()
			Me.attached_Conflict = True
			Me.parentWorkspace_Conflict = workspace
		End Sub

		Protected Friend Overrides WriteOnly Property Indexer As Indexer
			Set(ByVal indexer As Indexer)
				'TODO: to be abstracted
				Me.indexer_Conflict = indexer
			End Set
		End Property

		''' <summary>
		''' Base constructor. It's used within all constructors internally
		''' </summary>
		''' <param name="length">      the length of the buffer </param>
		''' <param name="elementSize"> the size of each element </param>
		Public Sub New(ByVal length As Long, ByVal elementSize As Integer)
			Me.New(length, elementSize, True)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal workspace As MemoryWorkspace)
			Me.New(length, elementSize, True, workspace)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal offset As Long)
			Me.New(length, elementSize)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseCudaDataBuffer(@NonNull DataBuffer underlyingBuffer, long length, long offset)
		Public Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
			If underlyingBuffer.wasClosed() Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			'this(length, underlyingBuffer.getElementSize(), offset);
			Me.allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES
			initTypeAndSize()
			Me.wrappedDataBuffer = underlyingBuffer
			Me.originalBuffer = If(underlyingBuffer.originalDataBuffer() Is Nothing, underlyingBuffer, underlyingBuffer.originalDataBuffer())
			Me.length_Conflict = length
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.elementSize_Conflict = CSByte(Math.Truncate(underlyingBuffer.getElementSize()))

			' in case of view creation, we initialize underlying buffer regardless of anything
			CType(underlyingBuffer, BaseCudaDataBuffer).lazyAllocateHostPointer()

			' we're creating view of the native DataBuffer
			ptrDataBuffer = CType(underlyingBuffer, BaseCudaDataBuffer).ptrDataBuffer.createView(length * underlyingBuffer.getElementSize(), offset * underlyingBuffer.getElementSize())
			Me.allocationPoint = New AllocationPoint(ptrDataBuffer, length)
			Dim hostPointer As val = allocationPoint.HostPointer

			Nd4j.DeallocatorService.pickObject(Me)

			Select Case underlyingBuffer.dataType()
				Case [DOUBLE]
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asDoublePointer()
					indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
				Case FLOAT
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asFloatPointer()
					indexer_Conflict = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
				Case UINT32
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asIntPointer()
					indexer_Conflict = UIntIndexer.create(CType(pointer_Conflict, IntPointer))
				Case INT
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asIntPointer()
					indexer_Conflict = IntIndexer.create(CType(pointer_Conflict, IntPointer))
				Case BFLOAT16
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asShortPointer()
					indexer_Conflict = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))
				Case HALF
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asShortPointer()
					indexer_Conflict = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))
				Case UINT64, [LONG] 'Fall through
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asLongPointer()
					indexer_Conflict = LongIndexer.create(CType(pointer_Conflict, LongPointer))
				Case UINT16
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asShortPointer()
					indexer_Conflict = UShortIndexer.create(CType(pointer_Conflict, ShortPointer))
				Case [SHORT]
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asShortPointer()
					indexer_Conflict = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
				Case BOOL
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asBooleanPointer()
					indexer_Conflict = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
				Case [BYTE]
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asBytePointer()
					indexer_Conflict = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
				Case UBYTE
					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asBytePointer()
					indexer_Conflict = UByteIndexer.create(CType(pointer_Conflict, BytePointer))
				Case UTF8
					Preconditions.checkArgument(offset = 0, "String array can't be a view")

					Me.pointer_Conflict = (New CudaPointer(hostPointer, originalBuffer.length())).asBytePointer()
					indexer_Conflict = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
				Case Else
					Throw New System.NotSupportedException()
			End Select
		End Sub

		Public Sub New(ByVal length As Long)
			Me.New(length, Nd4j.sizeOfDataType(Nd4j.dataType()))
		End Sub

		Public Sub New(ByVal data() As Single)
			'super(data);
			Me.New(data.Length, Nd4j.sizeOfDataType(DataType.FLOAT), False)
			set(data, data.Length, 0, 0)
		End Sub

		Public Sub New(ByVal data() As Integer)
			'super(data);
			Me.New(data.Length, Nd4j.sizeOfDataType(DataType.INT), False)
			set(data, data.Length, 0, 0)
		End Sub

		Public Sub New(ByVal data() As Long)
			'super(data);
			Me.New(data.Length, Nd4j.sizeOfDataType(DataType.LONG), False)
			set(data, data.Length, 0, 0)
		End Sub

		Public Sub New(ByVal data() As Long, ByVal copy As Boolean)
			'super(data);
			Me.New(data.Length, Nd4j.sizeOfDataType(DataType.LONG), False)

			If copy Then
				set(data, data.Length, 0, 0)
			End If
		End Sub

		Public Sub New(ByVal data() As Double)
			' super(data);
			Me.New(data.Length, Nd4j.sizeOfDataType(DataType.DOUBLE), False)
			set(data, data.Length, 0, 0)
		End Sub


		''' <summary>
		''' This method always returns host pointer
		''' 
		''' @return
		''' </summary>
		Public Overrides Function address() As Long
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Return allocationPoint.HostPointer.address()
		End Function

		Public Overrides Function platformAddress() As Long
			Return allocationPoint.DevicePointer.address()
		End Function

		Public Overrides Function pointer() As Pointer
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			' FIXME: very bad thing,
			lazyAllocateHostPointer()

			Return MyBase.pointer()
		End Function


		''' 
		''' <summary>
		''' PLEASE NOTE: length, srcOffset, dstOffset are considered numbers of elements, not byte offsets
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="length"> </param>
		''' <param name="srcOffset"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub set(ByVal data() As Integer, ByVal length As Long, ByVal srcOffset As Long, ByVal dstOffset As Long)
			' TODO: make sure getPointer returns proper pointer

			Select Case dataType()
				Case BOOL
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [BYTE]
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case UBYTE
						For e As Integer = 0 To data.Length - 1
							put(e, data(e))
						Next e
				Case [SHORT]
						Dim pointer As val = New ShortPointer(ArrayUtil.toShorts(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case INT
						Dim pointer As val = New IntPointer(data)
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [LONG]
						Dim pointer As val = New LongPointer(LongUtils.toLongs(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case HALF
						Dim pointer As val = New ShortPointer(ArrayUtil.toHalfs(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case FLOAT
						Dim pointer As val = New FloatPointer(ArrayUtil.toFloats(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [DOUBLE]
						Dim pointer As val = New DoublePointer(ArrayUtil.toDouble(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select
		End Sub

		Public Overridable Sub set(ByVal data() As Long, ByVal length As Long, ByVal srcOffset As Long, ByVal dstOffset As Long)
			' TODO: make sure getPointer returns proper pointer

			Select Case dataType()
				Case BOOL
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [BYTE]
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case UBYTE
					data = ArrayUtil.cutBelowZero(data)
						For e As Integer = 0 To data.Length - 1
							put(e, data(e))
						Next e
				Case UINT16
					data = ArrayUtil.cutBelowZero(data)
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case [SHORT]
						Dim pointer As val = New ShortPointer(ArrayUtil.toShorts(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case UINT32
					data = ArrayUtil.cutBelowZero(data)
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case INT
						Dim pointer As val = New IntPointer(ArrayUtil.toInts(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case UINT64
					data = ArrayUtil.cutBelowZero(data)
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case [LONG]
						Dim pointer As val = New LongPointer(data)
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case BFLOAT16
					Dim pointer As val = New ShortPointer(ArrayUtil.toBfloats(data))
					Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

					allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

					' we're keeping pointer reference for JVM
					pointer.address()
				Case HALF
						Dim pointer As val = New ShortPointer(ArrayUtil.toHalfs(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case FLOAT
						Dim pointer As val = New FloatPointer(ArrayUtil.toFloats(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [DOUBLE]
						Dim pointer As val = New DoublePointer(ArrayUtil.toDouble(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)
						' we're keeping pointer reference for JVM
						pointer.address()
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select

		End Sub

		''' 
		''' <summary>
		''' PLEASE NOTE: length, srcOffset, dstOffset are considered numbers of elements, not byte offsets
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="length"> </param>
		''' <param name="srcOffset"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub set(ByVal data() As Single, ByVal length As Long, ByVal srcOffset As Long, ByVal dstOffset As Long)
			Select Case dataType()
				Case BOOL
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [BYTE]
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case UBYTE
						For e As Integer = 0 To data.Length - 1
							put(e, data(e))
						Next e
				Case [SHORT]
						Dim pointer As val = New ShortPointer(ArrayUtil.toShorts(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case INT
						Dim pointer As val = New IntPointer(ArrayUtil.toInts(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [LONG]
						Dim pointer As val = New LongPointer(ArrayUtil.toLongArray(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case HALF
						Dim pointer As val = New ShortPointer(ArrayUtil.toHalfs(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case FLOAT
						Dim pointer As val = New FloatPointer(data)
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [DOUBLE]
						Dim pointer As New DoublePointer(ArrayUtil.toDoubles(data))
						Dim srcPtr As Pointer = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select
		End Sub

		''' 
		''' <summary>
		''' PLEASE NOTE: length, srcOffset, dstOffset are considered numbers of elements, not byte offsets
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="length"> </param>
		''' <param name="srcOffset"> </param>
		''' <param name="dstOffset"> </param>
		Public Overridable Sub set(ByVal data() As Double, ByVal length As Long, ByVal srcOffset As Long, ByVal dstOffset As Long)
			Select Case dataType()
				Case BOOL
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [BYTE]
						Dim pointer As val = New BytePointer(ArrayUtil.toBytes(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case UBYTE
						For e As Integer = 0 To data.Length - 1
							put(e, data(e))
						Next e
				Case [SHORT]
						Dim pointer As val = New ShortPointer(ArrayUtil.toShorts(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case INT
						Dim pointer As val = New IntPointer(ArrayUtil.toInts(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [LONG]
						Dim pointer As val = New LongPointer(ArrayUtil.toLongs(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case HALF
						Dim pointer As val = New ShortPointer(ArrayUtil.toHalfs(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case FLOAT
						Dim pointer As val = New FloatPointer(ArrayUtil.toFloats(data))
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case [DOUBLE]
						Dim pointer As val = New DoublePointer(data)
						Dim srcPtr As val = New CudaPointer(pointer.address() + (srcOffset * elementSize_Conflict))

						allocator.memcpyAsync(Me, srcPtr, length * elementSize_Conflict, dstOffset * elementSize_Conflict)

						' we're keeping pointer reference for JVM
						pointer.address()
				Case Else
					Throw New System.NotSupportedException("Unsupported data type: " & dataType())
			End Select
		End Sub

		Public Overrides WriteOnly Property Data As Integer()
			Set(ByVal data() As Integer)
				If data.Length = 0 Then
					Return
				End If
    
				set(data, data.Length, 0, 0)
			End Set
		End Property

		Public Overrides WriteOnly Property Data As Long()
			Set(ByVal data() As Long)
				If data.Length = 0 Then
					Return
				End If
    
				set(data, data.Length, 0, 0)
			End Set
		End Property

		Public Overrides WriteOnly Property Data As Single()
			Set(ByVal data() As Single)
				If data.Length = 0 Then
					Return
				End If
    
				set(data, data.Length, 0, 0)
			End Set
		End Property

		Public Overrides WriteOnly Property Data As Double()
			Set(ByVal data() As Double)
				If data.Length = 0 Then
					Return
				End If
    
				set(data, data.Length, 0, 0)
			End Set
		End Property

		Protected Friend Overrides Sub setNioBuffer()
			Throw New System.NotSupportedException("setNioBuffer() is not supported for CUDA backend")
		End Sub

		Public Overrides Sub copyAtStride(ByVal buf As DataBuffer, ByVal n As Long, ByVal stride As Long, ByVal yStride As Long, ByVal offset As Long, ByVal yOffset As Long)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			allocator.synchronizeHostData(buf)
			MyBase.copyAtStride(buf, n, stride, yStride, offset, yOffset)
		End Sub

		Public Overrides Function allocationMode() As AllocationMode
			Return allocationMode_Conflict
		End Function

		Public Overridable ReadOnly Property HostBuffer As ByteBuffer
			Get
				Return pointer_Conflict.asByteBuffer()
			End Get
		End Property

		Public Overridable ReadOnly Property HostPointer As Pointer
			Get
				Return AtomicAllocator.Instance.getHostPointer(Me)
			End Get
		End Property

		Public Overridable Function getHostPointer(ByVal offset As Long) As Pointer
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Sub removeReferencing(ByVal id As String)
			'referencing.remove(id);
		End Sub

		Public Overrides Function references() As ICollection(Of String)
			'return referencing;
			Return Nothing
		End Function

		Public Overrides ReadOnly Property ElementSize As Integer
			Get
				Return elementSize_Conflict
			End Get
		End Property


		Public Overrides Sub addReferencing(ByVal id As String)
			'referencing.add(id);
		End Sub


		<Obsolete>
		Public Overridable Function getHostPointer(ByVal arr As INDArray, ByVal stride As Integer, ByVal offset As Long, ByVal length As Integer) As Pointer
			Throw New System.NotSupportedException("This method is deprecated")
		End Function

		<Obsolete>
		Public Overridable Sub set(ByVal pointer As Pointer)
			Throw New System.NotSupportedException("set(Pointer) is not supported")
		End Sub

		Public Overrides Sub put(ByVal i As Long, ByVal element As Single)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			allocator.tickHostWrite(Me)
			MyBase.put(i, element)
		End Sub

		Public Overrides Sub put(ByVal i As Long, ByVal element As Boolean)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			allocator.tickHostWrite(Me)
			MyBase.put(i, element)
		End Sub

		Public Overrides Sub put(ByVal i As Long, ByVal element As Double)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			allocator.tickHostWrite(Me)
			MyBase.put(i, element)
		End Sub

		Public Overrides Sub put(ByVal i As Long, ByVal element As Integer)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			allocator.tickHostWrite(Me)
			MyBase.put(i, element)
		End Sub

		Public Overrides Sub put(ByVal i As Long, ByVal element As Long)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			allocator.tickHostWrite(Me)
			MyBase.put(i, element)
		End Sub

		Public Overrides Function addressPointer() As Pointer
			If released Then
				Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
			End If

			Return AtomicAllocator.Instance.getHostPointer(Me)
		End Function

		''' <summary>
		''' Set an individual element
		''' </summary>
		''' <param name="index"> the index of the element </param>
		''' <param name="from">  the element to get data from </param>
		<Obsolete>
		Protected Friend Overridable Sub set(ByVal index As Long, ByVal length As Long, ByVal from As Pointer, ByVal inc As Long)


			Dim offset As Long = ElementSize * index
			If offset >= Me.length() * ElementSize Then
				Throw New System.ArgumentException("Illegal offset " & offset & " with index of " & index & " and length " & Me.length())
			End If

			' TODO: fix this
			Throw New System.NotSupportedException("Deprecated set() call")
		End Sub

		''' <summary>
		''' Set an individual element
		''' </summary>
		''' <param name="index"> the index of the element </param>
		''' <param name="from">  the element to get data from </param>
		<Obsolete>
		Protected Friend Overridable Sub set(ByVal index As Long, ByVal length As Long, ByVal from As Pointer)
			set(index, length, from, 1)
		End Sub

		Public Overrides Sub assign(ByVal data As DataBuffer)
	'        JCudaBuffer buf = (JCudaBuffer) data;
	'        set(0, buf.getHostPointer());
	'        
	'        
	'        memcpyAsync(
	'                new Pointer(allocator.getPointer(this).address()),
	'                new Pointer(allocator.getPointer(data).address()),
	'                data.length()
	'        );
			allocator.memcpy(Me, data)
		End Sub

		Public Overrides Sub assign(ByVal indices() As Long, ByVal data() As Single, ByVal contiguous As Boolean, ByVal inc As Long)
			If indices.Length <> data.Length Then
				Throw New System.ArgumentException("Indices and data length must be the same")
			End If
			If indices.Length > length() Then
				Throw New System.ArgumentException("More elements than space to assign. This buffer is of length " & length() & " where the indices are of length " & data.Length)
			End If

			' TODO: eventually consider memcpy here
			For i As Integer = 0 To indices.Length - 1
				put(indices(i), data(i))
			Next i
		End Sub

		Public Overrides Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean, ByVal inc As Long)

			If indices.Length <> data.Length Then
				Throw New System.ArgumentException("Indices and data length must be the same")
			End If
			If indices.Length > length() Then
				Throw New System.ArgumentException("More elements than space to assign. This buffer is of length " & length() & " where the indices are of length " & data.Length)
			End If

			' TODO: eventually consider memcpy here
			For i As Integer = 0 To indices.Length - 1
				put(indices(i), data(i))
			Next i
		End Sub


		''' <summary>
		''' Set an individual element
		''' </summary>
		''' <param name="index"> the index of the element </param>
		''' <param name="from">  the element to get data from </param>
		<Obsolete>
		Protected Friend Overridable Sub set(ByVal index As Long, ByVal from As Pointer)
			set(index, 1, from)
		End Sub

		Public Overrides Sub flush()
			'
		End Sub


		Public Overrides Sub destroy()
		End Sub
		Protected Friend Overrides Function getDoubleUnsynced(ByVal index As Long) As Double
			Return MyBase.getDouble(index)
		End Function

		Protected Friend Overrides Function getFloatUnsynced(ByVal index As Long) As Single
			Return MyBase.getFloat(index)
		End Function

		Protected Friend Overrides Function getLongUnsynced(ByVal index As Long) As Long
			Return MyBase.getLong(index)
		End Function

		Protected Friend Overrides Function getIntUnsynced(ByVal index As Long) As Integer
			Return MyBase.getInt(index)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(DataOutputStream out) throws IOException
		Public Overrides Sub write(ByVal [out] As DataOutputStream)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			MyBase.write([out])
		End Sub

		Public Overrides Sub write(ByVal dos As Stream)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			MyBase.write(dos)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream stream) throws IOException
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			stream.defaultWriteObject()
			write(stream)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws IOException, ClassNotFoundException
		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
			doReadObject(stream)
		End Sub

		Public Overrides Function ToString() As String
			lazyAllocateHostPointer()
			AtomicAllocator.Instance.synchronizeHostData(Me)
			Return MyBase.ToString()
		End Function

		Public Overrides Function sameUnderlyingData(ByVal buffer As DataBuffer) As Boolean
			Return ptrDataBuffer.address() = DirectCast(buffer, BaseCudaDataBuffer).ptrDataBuffer.address()
		End Function

		''' <summary>
		''' PLEASE NOTE: this method implies STRICT equality only.
		''' I.e: this == object
		''' </summary>
		''' <param name="o">
		''' @return </param>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Nothing Then
				Return False
			End If
			If Me Is o Then
				Return True
			End If

			Return False
		End Function

		Public Overrides Sub read(ByVal [is] As Stream, ByVal allocationMode As AllocationMode, ByVal length As Long, ByVal dataType As DataType)
			If allocationPoint Is Nothing Then
				initPointers(length, dataType, False)
			End If
			MyBase.read([is], allocationMode, length, dataType)
			Me.allocationPoint.tickHostWrite()
		End Sub

		Public Overrides Sub pointerIndexerByCurrentType(ByVal currentType As DataType)
			'
	'        
	'        switch (currentType) {
	'            case LONG:
	'                pointer = new LongPointer(length());
	'                setIndexer(LongIndexer.create((LongPointer) pointer));
	'                type = DataType.LONG;
	'                break;
	'            case INT:
	'                pointer = new IntPointer(length());
	'                setIndexer(IntIndexer.create((IntPointer) pointer));
	'                type = DataType.INT;
	'                break;
	'            case DOUBLE:
	'                pointer = new DoublePointer(length());
	'                indexer = DoubleIndexer.create((DoublePointer) pointer);
	'                break;
	'            case FLOAT:
	'                pointer = new FloatPointer(length());
	'                setIndexer(FloatIndexer.create((FloatPointer) pointer));
	'                break;
	'            case HALF:
	'                pointer = new ShortPointer(length());
	'                setIndexer(HalfIndexer.create((ShortPointer) pointer));
	'                break;
	'            case COMPRESSED:
	'                break;
	'            default:
	'                throw new UnsupportedOperationException();
	'        }
	'        
		End Sub

		'@Override
		Public Overridable Overloads Sub read(ByVal s As DataInputStream)
			Try
				Dim savedMode As val = System.Enum.Parse(GetType(AllocationMode), s.readUTF())
				allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES

				Dim locLength As Long = 0

				If savedMode.ordinal() < 3 Then
					locLength = s.readInt()
				Else
					locLength = s.readLong()
				End If

				Dim reallocate As Boolean = locLength <> length_Conflict OrElse indexer_Conflict Is Nothing
				length_Conflict = locLength

				Dim t As val = DataType.valueOf(s.readUTF())
				'                  log.info("Restoring buffer ["+t+"] of length ["+ length+"]");
				If globalType = Nothing AndAlso Nd4j.dataType() <> Nothing Then
					globalType = Nd4j.dataType()
				End If

				If t = DataType.COMPRESSED Then
					type = t
					Return
				End If

				Me.elementSize_Conflict = CSByte(Nd4j.sizeOfDataType(t))
				Me.allocationPoint = AtomicAllocator.Instance.allocateMemory(Me, New AllocationShape(length_Conflict, elementSize_Conflict, t), False)

				Me.type = t

				Nd4j.DeallocatorService.pickObject(Me)

				Select Case type.innerEnumValue
					Case DataType.InnerEnum.DOUBLE
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asDoublePointer()
							indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
					Case DataType.InnerEnum.FLOAT
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asFloatPointer()
							indexer_Conflict = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
					Case DataType.InnerEnum.HALF
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asShortPointer()
							indexer_Conflict = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))
					Case DataType.InnerEnum.LONG
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asLongPointer()
							indexer_Conflict = LongIndexer.create(CType(pointer_Conflict, LongPointer))
					Case DataType.InnerEnum.INT
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asIntPointer()
							indexer_Conflict = IntIndexer.create(CType(pointer_Conflict, IntPointer))
					Case DataType.InnerEnum.SHORT
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asShortPointer()
							indexer_Conflict = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
					Case DataType.InnerEnum.UBYTE
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asBytePointer()
							indexer_Conflict = UByteIndexer.create(CType(pointer_Conflict, BytePointer))
					Case DataType.InnerEnum.BYTE
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asBytePointer()
							indexer_Conflict = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
					Case DataType.InnerEnum.BOOL
							Me.pointer_Conflict = (New CudaPointer(allocationPoint.HostPointer, length_Conflict)).asBooleanPointer()
							indexer_Conflict = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
					Case Else
						Throw New System.NotSupportedException("Unsupported data type: " & type)
				End Select

				readContent(s, t, t)
				allocationPoint.tickHostWrite()

			Catch e As Exception
				Throw New Exception(e)
			End Try


			' we call sync to copyback data to host
			AtomicAllocator.Instance.FlowController.synchronizeToDevice(allocationPoint)
			'allocator.synchronizeHostData(this);
		End Sub

		Public Overrides Function asBytes() As SByte()
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asBytes()
		End Function

		Public Overrides Function asDouble() As Double()
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asDouble()
		End Function

		Public Overrides Function asFloat() As Single()
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asFloat()
		End Function

		Public Overrides Function asInt() As Integer()
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asInt()
		End Function

		Public Overrides Function asLong() As Long()
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asLong()
		End Function

		Public Overrides Function asNio() As ByteBuffer
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asNio()
		End Function

		Public Overrides Function asNioDouble() As DoubleBuffer
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asNioDouble()
		End Function

		Public Overrides Function asNioFloat() As FloatBuffer
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asNioFloat()
		End Function

		Public Overrides Function asNioInt() As IntBuffer
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.asNioInt()
		End Function

		Public Overrides Function dup() As DataBuffer
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Dim buffer As DataBuffer = create(Me.length_Conflict)
			allocator.memcpyBlocking(buffer, New CudaPointer(allocator.getHostPointer(Me).address()), Me.length_Conflict * elementSize_Conflict, 0)
			Return buffer
		End Function

		Public Overrides Function getNumber(ByVal i As Long) As Number
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.getNumber(i)
		End Function

		Public Overrides Function getDouble(ByVal i As Long) As Double
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.getDouble(i)
		End Function

		Public Overrides Function getLong(ByVal i As Long) As Long
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.getLong(i)
		End Function


		Public Overrides Function getFloat(ByVal i As Long) As Single
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.getFloat(i)
		End Function

		Public Overrides Function getInt(ByVal ix As Long) As Integer
			lazyAllocateHostPointer()
			allocator.synchronizeHostData(Me)
			Return MyBase.getInt(ix)
		End Function

		Public Overridable Sub actualizePointerAndIndexer()
			Dim cptr As val = ptrDataBuffer.primaryBuffer()

			' skip update if pointers are equal
			If cptr IsNot Nothing AndAlso pointer_Conflict IsNot Nothing AndAlso cptr.address() = pointer_Conflict.address() Then
				Return
			End If

			Dim t As val = dataType()
			If t = DataType.BOOL Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asBoolPointer()
				Indexer = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
			ElseIf t = DataType.UBYTE Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asBytePointer()
				Indexer = UByteIndexer.create(CType(pointer_Conflict, BytePointer))
			ElseIf t = DataType.BYTE Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asBytePointer()
				Indexer = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
			ElseIf t = DataType.UINT16 Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asShortPointer()
				Indexer = UShortIndexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf t = DataType.SHORT Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asShortPointer()
				Indexer = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf t = DataType.UINT32 Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asIntPointer()
				Indexer = UIntIndexer.create(CType(pointer_Conflict, IntPointer))
			ElseIf t = DataType.INT Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asIntPointer()
				Indexer = IntIndexer.create(CType(pointer_Conflict, IntPointer))
			ElseIf t = DataType.UINT64 Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asLongPointer()
				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))
			ElseIf t = DataType.LONG Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asLongPointer()
				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))
			ElseIf t = DataType.BFLOAT16 Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asShortPointer()
				Indexer = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf t = DataType.HALF Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asShortPointer()
				Indexer = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf t = DataType.FLOAT Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asFloatPointer()
				Indexer = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
			ElseIf t = DataType.DOUBLE Then
				pointer_Conflict = (New PagedPointer(cptr, length_Conflict)).asDoublePointer()
				Indexer = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
			ElseIf t = DataType.UTF8 Then
				pointer_Conflict = (New PagedPointer(cptr, length())).asBytePointer()
				Indexer = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
			Else
				Throw New System.ArgumentException("Unknown datatype: " & dataType())
			End If
		End Sub

		Public Overrides Function reallocate(ByVal length As Long) As DataBuffer
			Dim oldHostPointer As val = Me.ptrDataBuffer.primaryBuffer()
			Dim oldDevicePointer As val = Me.ptrDataBuffer.specialBuffer()

			If Attached Then
				Dim capacity As val = length * ElementSize

				If oldDevicePointer IsNot Nothing AndAlso oldDevicePointer.address() <> 0 Then
					Dim nPtr As val = ParentWorkspace.alloc(capacity, MemoryKind.DEVICE, dataType(), False)
					NativeOpsHolder.Instance.getDeviceNativeOps().memcpySync(nPtr, oldDevicePointer, length * ElementSize, 3, Nothing)
					Me.ptrDataBuffer.setPrimaryBuffer(nPtr, length)

					allocationPoint.tickDeviceRead()
				End If

				If oldHostPointer IsNot Nothing AndAlso oldHostPointer.address() <> 0 Then
					Dim nPtr As val = ParentWorkspace.alloc(capacity, MemoryKind.HOST, dataType(), False)
					Pointer.memcpy(nPtr, oldHostPointer, Me.length() * ElementSize)
					Me.ptrDataBuffer.setPrimaryBuffer(nPtr, length)

					allocationPoint.tickHostRead()

					Select Case dataType()
						Case BOOL
							pointer_Conflict = nPtr.asBoolPointer()
							indexer_Conflict = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
						Case UTF8, [BYTE], UBYTE
							pointer_Conflict = nPtr.asBytePointer()
							indexer_Conflict = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
						Case UINT16, [SHORT]
							pointer_Conflict = nPtr.asShortPointer()
							indexer_Conflict = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
						Case UINT32
							pointer_Conflict = nPtr.asIntPointer()
							indexer_Conflict = UIntIndexer.create(CType(pointer_Conflict, IntPointer))
						Case INT
							pointer_Conflict = nPtr.asIntPointer()
							indexer_Conflict = IntIndexer.create(CType(pointer_Conflict, IntPointer))
						Case [DOUBLE]
							pointer_Conflict = nPtr.asDoublePointer()
							indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
						Case FLOAT
							pointer_Conflict = nPtr.asFloatPointer()
							indexer_Conflict = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
						Case HALF
							pointer_Conflict = nPtr.asShortPointer()
							indexer_Conflict = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))
						Case BFLOAT16
							pointer_Conflict = nPtr.asShortPointer()
							indexer_Conflict = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))
						Case UINT64, [LONG]
							pointer_Conflict = nPtr.asLongPointer()
							indexer_Conflict = LongIndexer.create(CType(pointer_Conflict, LongPointer))
					End Select
				End If


				workspaceGenerationId = ParentWorkspace.GenerationId
			Else
				Me.ptrDataBuffer.expand(length)
				Dim nPtr As val = New PagedPointer(Me.ptrDataBuffer.primaryBuffer(), length)

				Select Case dataType()
					Case BOOL
						pointer_Conflict = nPtr.asBoolPointer()
						indexer_Conflict = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
					Case UTF8, [BYTE], UBYTE
						pointer_Conflict = nPtr.asBytePointer()
						indexer_Conflict = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
					Case UINT16, [SHORT]
						pointer_Conflict = nPtr.asShortPointer()
						indexer_Conflict = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
					Case UINT32
						pointer_Conflict = nPtr.asIntPointer()
						indexer_Conflict = UIntIndexer.create(CType(pointer_Conflict, IntPointer))
					Case INT
						pointer_Conflict = nPtr.asIntPointer()
						indexer_Conflict = IntIndexer.create(CType(pointer_Conflict, IntPointer))
					Case [DOUBLE]
						pointer_Conflict = nPtr.asDoublePointer()
						indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
					Case FLOAT
						pointer_Conflict = nPtr.asFloatPointer()
						indexer_Conflict = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
					Case HALF
						pointer_Conflict = nPtr.asShortPointer()
						indexer_Conflict = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))
					Case BFLOAT16
						pointer_Conflict = nPtr.asShortPointer()
						indexer_Conflict = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))
					Case UINT64, [LONG]
						pointer_Conflict = nPtr.asLongPointer()
						indexer_Conflict = LongIndexer.create(CType(pointer_Conflict, LongPointer))
				End Select
			End If

			Me.underlyingLength_Conflict = length
			Me.length_Conflict = length
			Return Me
		End Function

		Public Overrides Function capacity() As Long
			If allocationPoint.HostPointer IsNot Nothing Then
				Return pointer_Conflict.capacity()
			Else
				Return length_Conflict
			End If
		End Function

		Protected Friend Overrides Sub release()
			If Not released Then
				ptrDataBuffer.closeBuffer()
				allocationPoint.setReleased(True)
			End If

			MyBase.release()
		End Sub

	'    
	'    protected short fromFloat( float fval ) {
	'        int fbits = Float.floatToIntBits( fval );
	'        int sign = fbits >>> 16 & 0x8000;          // sign only
	'        int val = ( fbits & 0x7fffffff ) + 0x1000; // rounded value
	'    
	'        if( val >= 0x47800000 )               // might be or become NaN/Inf
	'        {                                     // avoid Inf due to rounding
	'            if( ( fbits & 0x7fffffff ) >= 0x47800000 )
	'            {                                 // is or must become NaN/Inf
	'                if( val < 0x7f800000 )        // was value but too large
	'                    return (short) (sign | 0x7c00);     // make it +/-Inf
	'                return (short) (sign | 0x7c00 |        // remains +/-Inf or NaN
	'                        ( fbits & 0x007fffff ) >>> 13); // keep NaN (and Inf) bits
	'            }
	'            return (short) (sign | 0x7bff);             // unrounded not quite Inf
	'        }
	'        if( val >= 0x38800000 )               // remains normalized value
	'            return (short) (sign | val - 0x38000000 >>> 13); // exp - 127 + 15
	'        if( val < 0x33000000 )                // too small for subnormal
	'            return (short) sign;                      // becomes +/-0
	'        val = ( fbits & 0x7fffffff ) >>> 23;  // tmp exp for subnormal calc
	'        return (short) (sign | ( ( fbits & 0x7fffff | 0x800000 ) // add subnormal bit
	'                + ( 0x800000 >>> val - 102 )     // round depending on cut off
	'                >>> 126 - val ));   // div by 2^(1-(exp-127+15)) and >> 13 | exp=0
	'    }
	'    

		Public Overridable ReadOnly Property UniqueId As String Implements Deallocatable.getUniqueId
			Get
				Return "BCDB_" & allocationPoint.getObjectId()
			End Get
		End Property

		''' <summary>
		''' This method returns deallocator associated with this instance
		''' @return
		''' </summary>
		Public Overridable Function deallocator() As Deallocator
			Return New CudaDeallocator(Me)
		End Function

		Public Overridable Function targetDevice() As Integer Implements Deallocatable.targetDevice
			Return AtomicAllocator.Instance.getAllocationPoint(Me).getDeviceId()
		End Function

		Public Overrides Sub syncToPrimary()
			ptrDataBuffer.syncToPrimary()
		End Sub

		Public Overrides Sub syncToSpecial()
			ptrDataBuffer.syncToSpecial()
		End Sub
	End Class

End Namespace