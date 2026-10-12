Imports System
Imports val = lombok.val
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports BaseDataBuffer = org.nd4j.linalg.api.buffer.BaseDataBuffer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocUtil = org.nd4j.linalg.api.buffer.util.AllocUtil
Imports Deallocatable = org.nd4j.linalg.api.memory.Deallocatable
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpaqueDataBuffer = org.nd4j.nativeblas.OpaqueDataBuffer
import static org.nd4j.linalg.api.buffer.DataType.INT8

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

Namespace org.nd4j.linalg.cpu.nativecpu.buffer


	<Serializable>
	Public MustInherit Class BaseCpuDataBuffer
		Inherits BaseDataBuffer
		Implements Deallocatable

		<NonSerialized>
		Protected Friend ptrDataBuffer As OpaqueDataBuffer

		<NonSerialized>
		Private ReadOnly instanceId As Long = Nd4j.DeallocatorService.nextValue()

		Protected Friend Sub New()

		End Sub


		Public Overridable ReadOnly Property UniqueId As String Implements Deallocatable.getUniqueId
			Get
				Return "BCDB_" & instanceId
			End Get
		End Property

		Public Overridable Function deallocator() As Deallocator
			Return New CpuDeallocator(Me)
		End Function

		Public Overridable ReadOnly Property OpaqueDataBuffer As OpaqueDataBuffer
			Get
				If released Then
					Throw New System.InvalidOperationException("You can't use DataBuffer once it was released")
				End If
    
				Return ptrDataBuffer
			End Get
		End Property

		Public Overridable Function targetDevice() As Integer Implements Deallocatable.targetDevice
			' TODO: once we add NUMA support this might change. Or might not.
			Return 0
		End Function


		''' 
		''' <param name="length"> </param>
		''' <param name="elementSize"> </param>
		Public Sub New(ByVal length As Long, ByVal elementSize As Integer)
			If length < 1 Then
				Throw New System.ArgumentException("Length must be >= 1")
			End If
			initTypeAndSize()
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			Me.length_Conflict = length
			Me.underlyingLength_Conflict = length
			Me.elementSize_Conflict = CSByte(elementSize)

			If dataType() <> DataType.UTF8 Then
				ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(length, dataType(), False)
			End If

			If dataType() = DataType.DOUBLE Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asDoublePointer()

				indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
			ElseIf dataType() = DataType.FLOAT Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asFloatPointer()

				Indexer = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
			ElseIf dataType() = DataType.INT32 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asIntPointer()

				Indexer = IntIndexer.create(CType(pointer_Conflict, IntPointer))
			ElseIf dataType() = DataType.LONG Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asLongPointer()

				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))
			ElseIf dataType() = DataType.SHORT Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()

				Indexer = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf dataType() = DataType.BYTE Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asBytePointer()

				Indexer = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
			ElseIf dataType() = DataType.UBYTE Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asBytePointer()

				Indexer = UByteIndexer.create(CType(pointer_Conflict, BytePointer))
			ElseIf dataType() = DataType.UTF8 Then
				ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(length, INT8, False)
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asBytePointer()

				Indexer = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
			ElseIf dataType() = DataType.FLOAT16 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()
				Indexer = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf dataType() = DataType.BFLOAT16 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()
				Indexer = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf dataType() = DataType.BOOL Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asBoolPointer()
				Indexer = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
			ElseIf dataType() = DataType.UINT16 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()
				Indexer = UShortIndexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf dataType() = DataType.UINT32 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asIntPointer()
				Indexer = UIntIndexer.create(CType(pointer_Conflict, IntPointer))
			ElseIf dataType() = DataType.UINT64 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asLongPointer()
				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))
			End If

			Nd4j.DeallocatorService.pickObject(Me)
		End Sub

		''' 
		''' <param name="length"> </param>
		''' <param name="elementSize"> </param>
		Public Sub New(ByVal length As Integer, ByVal elementSize As Integer, ByVal offset As Long)
			Me.New(length, elementSize)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = length - offset
			Me.underlyingLength_Conflict = length
		End Sub


		Protected Friend Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
			MyBase.New(underlyingBuffer, length, offset)

			' for vew we need "externally managed" pointer and deallocator registration
			ptrDataBuffer = DirectCast(underlyingBuffer, BaseCpuDataBuffer).ptrDataBuffer.createView(length * underlyingBuffer.ElementSize, offset * underlyingBuffer.ElementSize)
			Nd4j.DeallocatorService.pickObject(Me)


			' update pointer now
			actualizePointerAndIndexer()
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

			Dim ptr As val = ptrDataBuffer.primaryBuffer()

			If offset > 0 Then
				temp = New PagedPointer(temp.address() + offset * ElementSize)
			End If

			Pointer.memcpy(ptr, temp, length * Nd4j.sizeOfDataType(dtype))
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

		Public Overrides Sub pointerIndexerByCurrentType(ByVal currentType As DataType)

			type = currentType

			If ptrDataBuffer Is Nothing Then
				ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(length(), type, False)
				Nd4j.DeallocatorService.pickObject(Me)
			End If

			actualizePointerAndIndexer()
		End Sub

		''' <summary>
		''' Instantiate a buffer with the given length
		''' </summary>
		''' <param name="length"> the length of the buffer </param>
		Protected Friend Sub New(ByVal length As Long)
			Me.New(length, True)
		End Sub

		Protected Friend Sub New(ByVal length As Long, ByVal initialize As Boolean)
			If length < 0 Then
				Throw New System.ArgumentException("Length must be >= 0")
			End If
			initTypeAndSize()
			Me.length_Conflict = length
			Me.underlyingLength_Conflict = length
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			If length < 0 Then
				Throw New System.ArgumentException("Unable to create a buffer of length <= 0")
			End If

			If dataType() <> DataType.UTF8 Then
				ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(length, dataType(), False)
			End If

			If dataType() = DataType.DOUBLE Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asDoublePointer()

				indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.FLOAT Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asFloatPointer()

				Indexer = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))

				If initialize Then
					fillPointerWithZero()
				End If

			ElseIf dataType() = DataType.HALF Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()

				Indexer = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.BFLOAT16 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()

				Indexer = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.INT Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asIntPointer()

				Indexer = IntIndexer.create(CType(pointer_Conflict, IntPointer))
				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.LONG Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asLongPointer()

				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.BYTE Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asBytePointer()

				Indexer = ByteIndexer.create(CType(pointer_Conflict, BytePointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.SHORT Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()

				Indexer = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.UBYTE Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asBytePointer()

				Indexer = UByteIndexer.create(CType(pointer_Conflict, BytePointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.UINT16 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asShortPointer()

				Indexer = UShortIndexer.create(CType(pointer_Conflict, ShortPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.UINT32 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asIntPointer()

				Indexer = UIntIndexer.create(CType(pointer_Conflict, IntPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.UINT64 Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asLongPointer()

				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.BOOL Then
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), length)).asBoolPointer()

				Indexer = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))

				If initialize Then
					fillPointerWithZero()
				End If
			ElseIf dataType() = DataType.UTF8 Then
				' we are allocating buffer as INT8 intentionally
				ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(Me.length(), INT8, False)
				pointer_Conflict = (New PagedPointer(ptrDataBuffer.primaryBuffer(), Me.length())).asBytePointer()

				Indexer = ByteIndexer.create(CType(pointer_Conflict, BytePointer))

				If initialize Then
					fillPointerWithZero()
				End If
			End If

			Nd4j.DeallocatorService.pickObject(Me)
		End Sub

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

		Public Overrides Function addressPointer() As Pointer
			' we're fetching actual pointer right from C++
			Dim tempPtr As val = New PagedPointer(ptrDataBuffer.primaryBuffer())

			Select Case Me.type.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return tempPtr.asDoublePointer()
				Case DataType.InnerEnum.FLOAT
					Return tempPtr.asFloatPointer()
				Case DataType.InnerEnum.UINT16, [SHORT], BFLOAT16, HALF
					Return tempPtr.asShortPointer()
				Case DataType.InnerEnum.UINT32, INT
					Return tempPtr.asIntPointer()
				Case DataType.InnerEnum.UBYTE, [BYTE]
					Return tempPtr.asBytePointer()
				Case DataType.InnerEnum.UINT64, [LONG]
					Return tempPtr.asLongPointer()
				Case DataType.InnerEnum.BOOL
					Return tempPtr.asBoolPointer()
				Case Else
					Return tempPtr.asBytePointer()
			End Select
		End Function

		Protected Friend Sub New(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			If length < 1 Then
				Throw New System.ArgumentException("Length must be >= 1")
			End If
			initTypeAndSize()
			Me.length_Conflict = length
			Me.underlyingLength_Conflict = length
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext



			If length < 0 Then
				Throw New System.ArgumentException("Unable to create a buffer of length <= 0")
			End If

			If dataType() = DataType.DOUBLE Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asDoublePointer() 'new DoublePointer(length());
				indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))

			ElseIf dataType() = DataType.FLOAT Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asFloatPointer() 'new FloatPointer(length());
				Indexer = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))

			ElseIf dataType() = DataType.HALF Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asShortPointer() 'new FloatPointer(length());
				Indexer = HalfIndexer.create(CType(pointer_Conflict, ShortPointer))

			ElseIf dataType() = DataType.BFLOAT16 Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asShortPointer() 'new FloatPointer(length());
				Indexer = Bfloat16Indexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf dataType() = DataType.INT Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asIntPointer() 'new IntPointer(length());
				Indexer = IntIndexer.create(CType(pointer_Conflict, IntPointer))

			ElseIf dataType() = DataType.UINT32 Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asIntPointer() 'new IntPointer(length());
				Indexer = UIntIndexer.create(CType(pointer_Conflict, IntPointer))

			ElseIf dataType() = DataType.UINT64 Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				' FIXME: need unsigned indexer here
				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asLongPointer() 'new IntPointer(length());
				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))

			ElseIf dataType() = DataType.LONG Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asLongPointer() 'new LongPointer(length());
				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))
			ElseIf dataType() = DataType.BYTE Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asBytePointer() 'new LongPointer(length());
				Indexer = ByteIndexer.create(CType(pointer_Conflict, BytePointer))
			ElseIf dataType() = DataType.UBYTE Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asBytePointer() 'new LongPointer(length());
				Indexer = UByteIndexer.create(CType(pointer_Conflict, BytePointer))
			ElseIf dataType() = DataType.UINT16 Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asShortPointer() 'new IntPointer(length());
				Indexer = UShortIndexer.create(CType(pointer_Conflict, ShortPointer))

			ElseIf dataType() = DataType.SHORT Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asShortPointer() 'new LongPointer(length());
				Indexer = ShortIndexer.create(CType(pointer_Conflict, ShortPointer))
			ElseIf dataType() = DataType.BOOL Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asBoolPointer() 'new LongPointer(length());
				Indexer = BooleanIndexer.create(CType(pointer_Conflict, BooleanPointer))
			ElseIf dataType() = DataType.UTF8 Then
				attached_Conflict = True
				parentWorkspace_Conflict = workspace

				pointer_Conflict = workspace.alloc(length * ElementSize, dataType(), initialize).asLongPointer() 'new LongPointer(length());
				Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))
			End If

			' storing pointer into native DataBuffer
			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(length, dataType(), Me.pointer_Conflict, Nothing)

			' adding deallocator reference
			Nd4j.DeallocatorService.pickObject(Me)

			workspaceGenerationId = workspace.GenerationId
		End Sub

		Public Sub New(ByVal pointer As Pointer, ByVal indexer As Indexer, ByVal length As Long)
			MyBase.New(pointer, indexer, length)

			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(length, dataType(), Me.pointer_Conflict, Nothing)
			Nd4j.DeallocatorService.pickObject(Me)
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="copy"> </param>
		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long)
			Me.New(data, copy)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length

		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			Me.New(data, copy, workspace)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="copy"> </param>
		Public Sub New(ByVal data() As Single, ByVal copy As Boolean)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			initTypeAndSize()

			pointer_Conflict = New FloatPointer(data)

			' creating & registering native DataBuffer
			ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(data.Length, DataType.FLOAT, False)
			ptrDataBuffer.setPrimaryBuffer(pointer_Conflict, data.Length)
			Nd4j.DeallocatorService.pickObject(Me)

			Indexer = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
			'wrappedBuffer = pointer.asByteBuffer();

			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
			attached_Conflict = True
			parentWorkspace_Conflict = workspace

			initTypeAndSize()

			'log.info("Allocating FloatPointer from array of {} elements", data.length);

			pointer_Conflict = workspace.alloc(data.Length * ElementSize, dataType(), False).asFloatPointer().put(data)

			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(length_Conflict, dataType(), Me.pointer_Conflict, Nothing)
			Nd4j.DeallocatorService.pickObject(Me)

			workspaceGenerationId = workspace.GenerationId
			Indexer = FloatIndexer.create(CType(pointer_Conflict, FloatPointer))
			'wrappedBuffer = pointer.asByteBuffer();
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
			attached_Conflict = True
			parentWorkspace_Conflict = workspace

			initTypeAndSize()

			'log.info("Allocating FloatPointer from array of {} elements", data.length);

			pointer_Conflict = workspace.alloc(data.Length * ElementSize, dataType(), False).asDoublePointer().put(data)

			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(length_Conflict, dataType(), Me.pointer_Conflict, Nothing)
			Nd4j.DeallocatorService.pickObject(Me)

			workspaceGenerationId = workspace.GenerationId
			indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))
			'wrappedBuffer = pointer.asByteBuffer();
		End Sub


		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
			attached_Conflict = True
			parentWorkspace_Conflict = workspace

			initTypeAndSize()

			'log.info("Allocating FloatPointer from array of {} elements", data.length);

			pointer_Conflict = workspace.alloc(data.Length * ElementSize, dataType(), False).asIntPointer().put(data)

			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(length_Conflict, dataType(), Me.pointer_Conflict, Nothing)
			Nd4j.DeallocatorService.pickObject(Me)

			workspaceGenerationId = workspace.GenerationId
			indexer_Conflict = IntIndexer.create(CType(pointer_Conflict, IntPointer))
			'wrappedBuffer = pointer.asByteBuffer();
		End Sub

		Public Sub New(ByVal data() As Long, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
			attached_Conflict = True
			parentWorkspace_Conflict = workspace

			initTypeAndSize()

			'log.info("Allocating FloatPointer from array of {} elements", data.length);

			pointer_Conflict = workspace.alloc(data.Length * ElementSize, dataType(), False).asLongPointer().put(data)

			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(length_Conflict, dataType(), Me.pointer_Conflict, Nothing)
			Nd4j.DeallocatorService.pickObject(Me)

			workspaceGenerationId = workspace.GenerationId
			indexer_Conflict = LongIndexer.create(CType(pointer_Conflict, LongPointer))
			'wrappedBuffer = pointer.asByteBuffer();
		End Sub


		''' 
		''' <param name="data"> </param>
		''' <param name="copy"> </param>
		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long)
			Me.New(data, copy)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.underlyingLength_Conflict = data.Length
			Me.length_Conflict = underlyingLength_Conflict - offset
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			Me.New(data, copy, workspace)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.underlyingLength_Conflict = data.Length
			Me.length_Conflict = underlyingLength_Conflict - offset
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="copy"> </param>
		Public Sub New(ByVal data() As Double, ByVal copy As Boolean)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			initTypeAndSize()

			pointer_Conflict = New DoublePointer(data)
			indexer_Conflict = DoubleIndexer.create(CType(pointer_Conflict, DoublePointer))

			' creating & registering native DataBuffer
			ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(data.Length, DataType.DOUBLE, False)
			ptrDataBuffer.setPrimaryBuffer(pointer_Conflict, data.Length)
			Nd4j.DeallocatorService.pickObject(Me)

			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
		End Sub


		''' 
		''' <param name="data"> </param>
		''' <param name="copy"> </param>
		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal offset As Long)
			Me.New(data, copy)
			Me.offset_Conflict = offset
			Me.originalOffset_Conflict = offset
			Me.length_Conflict = data.Length - offset
			Me.underlyingLength_Conflict = data.Length
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="copy"> </param>
		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			initTypeAndSize()

			pointer_Conflict = New IntPointer(data)
			Indexer = IntIndexer.create(CType(pointer_Conflict, IntPointer))

			' creating & registering native DataBuffer
			ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(data.Length, DataType.INT32, False)
			ptrDataBuffer.setPrimaryBuffer(pointer_Conflict, data.Length)
			Nd4j.DeallocatorService.pickObject(Me)

			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
		End Sub

		''' 
		''' <param name="data"> </param>
		''' <param name="copy"> </param>
		Public Sub New(ByVal data() As Long, ByVal copy As Boolean)
			allocationMode_Conflict = AllocUtil.AllocationModeFromContext
			initTypeAndSize()

			pointer_Conflict = New LongPointer(data)
			Indexer = LongIndexer.create(CType(pointer_Conflict, LongPointer))

			' creating & registering native DataBuffer
			ptrDataBuffer = OpaqueDataBuffer.allocateDataBuffer(data.Length, DataType.INT64, False)
			ptrDataBuffer.setPrimaryBuffer(pointer_Conflict, data.Length)
			Nd4j.DeallocatorService.pickObject(Me)

			length_Conflict = data.Length
			underlyingLength_Conflict = data.Length
		End Sub


		''' 
		''' <param name="data"> </param>
		Public Sub New(ByVal data() As Double)
			Me.New(data, True)
		End Sub

		''' 
		''' <param name="data"> </param>
		Public Sub New(ByVal data() As Integer)
			Me.New(data, True)
		End Sub

		''' 
		''' <param name="data"> </param>
		Public Sub New(ByVal data() As Single)
			Me.New(data, True)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal workspace As MemoryWorkspace)
			Me.New(data, True, workspace)
		End Sub

		Protected Friend Overrides Sub release()
			ptrDataBuffer.closeBuffer()
			MyBase.release()
		End Sub

		''' <summary>
		''' Reallocate the native memory of the buffer </summary>
		''' <param name="length"> the new length of the buffer </param>
		''' <returns> this databuffer
		'''  </returns>
		Public Overrides Function reallocate(ByVal length As Long) As DataBuffer
			Dim oldPointer As val = ptrDataBuffer.primaryBuffer()

			If Attached Then
				Dim capacity As val = length * ElementSize
				Dim nPtr As val = ParentWorkspace.alloc(capacity, dataType(), False)
				Me.ptrDataBuffer.setPrimaryBuffer(nPtr, length)

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

				Pointer.memcpy(pointer_Conflict, oldPointer, Me.length() * ElementSize)
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

		Public Overrides Sub syncToPrimary()
			ptrDataBuffer.syncToPrimary()
		End Sub

		Public Overrides Sub syncToSpecial()
			ptrDataBuffer.syncToSpecial()
		End Sub
	End Class

End Namespace