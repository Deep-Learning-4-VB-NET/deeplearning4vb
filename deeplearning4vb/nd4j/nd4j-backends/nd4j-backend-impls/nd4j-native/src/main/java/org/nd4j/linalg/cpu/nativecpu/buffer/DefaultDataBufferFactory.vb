Imports System
Imports NonNull = lombok.NonNull
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DoubleIndexer = org.bytedeco.javacpp.indexer.DoubleIndexer
Imports FloatIndexer = org.bytedeco.javacpp.indexer.FloatIndexer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports IntIndexer = org.bytedeco.javacpp.indexer.IntIndexer
Imports org.nd4j.linalg.api.buffer
Imports DataBufferFactory = org.nd4j.linalg.api.buffer.factory.DataBufferFactory
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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


	Public Class DefaultDataBufferFactory
		Implements DataBufferFactory

'JAVA TO VB CONVERTER NOTE: The field allocationMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend allocationMode_Conflict As DataBuffer.AllocationMode


		Public Overridable WriteOnly Property AllocationMode As DataBuffer.AllocationMode
			Set(ByVal allocationMode As DataBuffer.AllocationMode)
				Me.allocationMode_Conflict = allocationMode
			End Set
		End Property

		Public Overridable Function allocationMode() As DataBuffer.AllocationMode
			If allocationMode_Conflict = Nothing Then
				Dim otherAlloc As String = System.getProperty("alloc")
				If otherAlloc.Equals("heap") Then
					AllocationMode = DataBuffer.AllocationMode.HEAP
				ElseIf otherAlloc.Equals("direct") Then
					AllocationMode = DataBuffer.AllocationMode.DIRECT
				ElseIf otherAlloc.Equals("javacpp") Then
					AllocationMode = DataBuffer.AllocationMode.JAVACPP
				End If
			End If
			Return allocationMode_Conflict
		End Function

		Public Overridable Function create(ByVal underlyingBuffer As DataBuffer, ByVal offset As Long, ByVal length As Long) As DataBuffer
			If underlyingBuffer.dataType() = DataType.DOUBLE Then
				Return New DoubleBuffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.FLOAT Then
				Return New FloatBuffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.INT Then
				Return New IntBuffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.LONG Then
				Return New LongBuffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.BOOL Then
				Return New BoolBuffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.SHORT Then
				Return New Int16Buffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.BYTE Then
				Return New Int8Buffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.UBYTE Then
				Return New UInt8Buffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.UINT16 Then
				Return New UInt16Buffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.UINT32 Then
				Return New UInt32Buffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.UINT64 Then
				Return New UInt64Buffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.BFLOAT16 Then
				Return New BFloat16Buffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.HALF Then
				Return New HalfBuffer(underlyingBuffer, length, offset)
			ElseIf underlyingBuffer.dataType() = DataType.UTF8 Then
				Return New Utf8Buffer(underlyingBuffer, length, offset)
			End If
			Return Nothing
		End Function


		Public Overridable Function createDouble(ByVal offset As Long, ByVal length As Integer) As DataBuffer
			Return New DoubleBuffer(length, 8, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal length As Integer) As DataBuffer
			Return New FloatBuffer(length, 4, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal length As Integer) As DataBuffer
			Return New IntBuffer(length, 4, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Integer) As DataBuffer
			Return createDouble(offset, data, True)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Integer) As DataBuffer
			Dim ret As New FloatBuffer(ArrayUtil.toFloats(data), True, offset)
			Return ret
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Integer) As DataBuffer
			Return New IntBuffer(data, True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double) As DataBuffer
			Return New DoubleBuffer(data, True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New DoubleBuffer(data, True, offset, workspace)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As SByte, ByVal length As Integer) As DataBuffer
			Return createDouble(offset, ArrayUtil.toDoubleArray(data), True)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As SByte, ByVal length As Integer) As DataBuffer
			Return createFloat(offset, ArrayUtil.toFloatArray(data), True)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Double) As DataBuffer
			Return New FloatBuffer(ArrayUtil.toFloats(data), True, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Double) As DataBuffer
			Return New IntBuffer(ArrayUtil.toInts(data), True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Single) As DataBuffer
			Return New DoubleBuffer(ArrayUtil.toDoubles(data), True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single) As DataBuffer
			Return New FloatBuffer(data, True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New FloatBuffer(data, True, offset, workspace)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Single) As DataBuffer
			Return New IntBuffer(ArrayUtil.toInts(data), True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Return New DoubleBuffer(ArrayUtil.toDoubles(data), True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Return New FloatBuffer(ArrayUtil.toFloats(data), copy, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Return New IntBuffer(data, copy, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Return New DoubleBuffer(data, copy, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Return New FloatBuffer(ArrayUtil.toFloats(data), copy, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Return New IntBuffer(ArrayUtil.toInts(data), copy, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Return New DoubleBuffer(ArrayUtil.toDoubles(data), copy, offset)
		End Function



		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Return New FloatBuffer(data, copy, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Return New IntBuffer(ArrayUtil.toInts(data), copy, offset)
		End Function


		Public Overridable Function createDouble(ByVal length As Long) As DataBuffer
			Return New DoubleBuffer(length)
		End Function

		Public Overridable Function createDouble(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New DoubleBuffer(length, initialize)
		End Function

		Public Overridable Function createFloat(ByVal length As Long) As DataBuffer
			Return New FloatBuffer(length)
		End Function

		Public Overridable Function createFloat(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New FloatBuffer(length, initialize)
		End Function

		Public Overridable Function createFloat(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New FloatBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function create(ByVal underlyingBuffer As ByteBuffer, ByVal dataType As DataType, ByVal length As Long, ByVal offset As Long) As DataBuffer
			Select Case dataType.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Return New DoubleBuffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Return New FloatBuffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Return New HalfBuffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16
					Return New BFloat16Buffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					Return New LongBuffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Return New IntBuffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Return New Int16Buffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					Return New UInt8Buffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16
					Return New UInt16Buffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32
					Return New UInt32Buffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64
					Return New UInt64Buffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Return New Int8Buffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Return New BoolBuffer(underlyingBuffer, dataType, length, offset)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UTF8
					Return New Utf8Buffer(underlyingBuffer, dataType, length, offset)
				Case Else
					Throw New System.InvalidOperationException("Unknown datatype used: [" & dataType & "]")
			End Select
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public DataBuffer create(@NonNull DataType dataType, long length, boolean initialize)
		Public Overridable Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Select Case dataType.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Return New DoubleBuffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Return New FloatBuffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Return New HalfBuffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16
					Return New BFloat16Buffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					Return New LongBuffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Return New IntBuffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Return New Int16Buffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					Return New UInt8Buffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16
					Return New UInt16Buffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32
					Return New UInt32Buffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64
					Return New UInt64Buffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Return New Int8Buffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Return New BoolBuffer(length, initialize)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UTF8
					Return New Utf8Buffer(length, True)
				Case Else
					Throw New System.InvalidOperationException("Unknown datatype used: [" & dataType & "]")
			End Select
		End Function

		Public Overridable Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Select Case dataType.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Return New DoubleBuffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Return New FloatBuffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16
					Return New BFloat16Buffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Return New HalfBuffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					Return New LongBuffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Return New IntBuffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Return New Int16Buffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					Return New UInt8Buffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16
					Return New UInt16Buffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32
					Return New UInt32Buffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64
					Return New UInt64Buffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Return New Int8Buffer(length, initialize, workspace)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Return New BoolBuffer(length, initialize, workspace)
				Case Else
					Throw New System.InvalidOperationException("Unknown datatype used: [" & dataType & "]")
			End Select
		End Function

		Public Overridable Function createInt(ByVal length As Long) As DataBuffer
			Return New IntBuffer(length)
		End Function

		Public Overridable Function createBFloat16(ByVal length As Long) As DataBuffer
			Return New BFloat16Buffer(length)
		End Function

		Public Overridable Function createUInt(ByVal length As Long) As DataBuffer
			Return New UInt32Buffer(length)
		End Function

		Public Overridable Function createUShort(ByVal length As Long) As DataBuffer
			Return New UInt16Buffer(length)
		End Function

		Public Overridable Function createUByte(ByVal length As Long) As DataBuffer
			Return New UInt8Buffer(length)
		End Function

		Public Overridable Function createULong(ByVal length As Long) As DataBuffer
			Return New UInt64Buffer(length)
		End Function

		Public Overridable Function createBool(ByVal length As Long) As DataBuffer
			Return New BoolBuffer(length)
		End Function

		Public Overridable Function createShort(ByVal length As Long) As DataBuffer
			Return New Int16Buffer(length)
		End Function

		Public Overridable Function createByte(ByVal length As Long) As DataBuffer
			Return New Int8Buffer(length)
		End Function

		Public Overridable Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New BFloat16Buffer(length, initialize)
		End Function

		Public Overridable Function createUInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New UInt32Buffer(length, initialize)
		End Function

		Public Overridable Function createUShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New UInt16Buffer(length, initialize)
		End Function

		Public Overridable Function createUByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New UInt8Buffer(length, initialize)
		End Function

		Public Overridable Function createULong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New UInt64Buffer(length, initialize)
		End Function

		Public Overridable Function createBool(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New BoolBuffer(length, initialize)
		End Function

		Public Overridable Function createShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New Int16Buffer(length, initialize)
		End Function

		Public Overridable Function createByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New Int8Buffer(length, initialize)
		End Function

		Public Overridable Function createInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New IntBuffer(length, initialize)
		End Function

		Public Overridable Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New BFloat16Buffer(length, initialize, workspace)
		End Function

		Public Overridable Function createUInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New UInt32Buffer(length, initialize, workspace)
		End Function

		Public Overridable Function createUShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New UInt16Buffer(length, initialize, workspace)
		End Function

		Public Overridable Function createUByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New UInt8Buffer(length, initialize, workspace)
		End Function

		Public Overridable Function createULong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New UInt64Buffer(length, initialize, workspace)
		End Function

		Public Overridable Function createBool(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New BoolBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New Int16Buffer(length, initialize, workspace)
		End Function

		Public Overridable Function createByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New Int8Buffer(length, initialize, workspace)
		End Function


		Public Overridable Function createInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New IntBuffer(length, initialize, workspace)
		End Function

		''' <summary>
		''' This method will create new DataBuffer of the same dataType & same length
		''' </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Overridable Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean) As DataBuffer
			Return create(buffer.dataType(), buffer.length(), init)
		End Function

		''' <summary>
		''' This method will create new DataBuffer of the same dataType & same length
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="workspace">
		''' @return </param>
		Public Overridable Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return create(buffer.dataType(), buffer.length(), init, workspace)
		End Function

		Public Overridable Function createDouble(ByVal data() As Integer) As DataBuffer
			Return createDouble(data, True)
		End Function

		Public Overridable Function createFloat(ByVal data() As Integer) As DataBuffer
			Return createFloat(data, True)
		End Function

		Public Overridable Function createInt(ByVal data() As Integer) As DataBuffer
			Return createInt(data, True)
		End Function

		Public Overridable Function createInt(ByVal data() As Integer, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return createInt(data, True, workspace)
		End Function

		Public Overridable Function createInt(ByVal data() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New IntBuffer(data, copy, workspace)
		End Function

		Public Overridable Function createDouble(ByVal data() As Double) As DataBuffer
			Return createDouble(data, True)
		End Function

		Public Overridable Function createFloat(ByVal data() As Double) As DataBuffer
			Return createFloat(data, True)
		End Function

		Public Overridable Function createInt(ByVal data() As Double) As DataBuffer
			Return createInt(data, True)
		End Function

		Public Overridable Function createDouble(ByVal data() As Single) As DataBuffer
			Return createDouble(data, True)
		End Function

		Public Overridable Function createFloat(ByVal data() As Single) As DataBuffer
			Return createFloat(data, True)
		End Function

		Public Overridable Function createFloat(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return createFloat(data, True, workspace)
		End Function

		Public Overridable Function createInt(ByVal data() As Single) As DataBuffer
			Return createInt(data, True)
		End Function

		Public Overridable Function createDouble(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Return New DoubleBuffer(ArrayUtil.toDoubles(data), copy)
		End Function

		Public Overridable Function createFloat(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Return New FloatBuffer(ArrayUtil.toFloats(data), copy)
		End Function

		Public Overridable Function createInt(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Return New IntBuffer(data, copy)
		End Function

		Public Overridable Function createLong(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Return New LongBuffer(ArrayUtil.toLongArray(data), copy)
		End Function

		Public Overridable Function createDouble(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer
			Return New DoubleBuffer(ArrayUtil.toDouble(data), copy)
		End Function

		Public Overridable Function createFloat(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer
			Return New FloatBuffer(ArrayUtil.toFloats(data), copy)
		End Function

		Public Overridable Function createInt(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer
			Return New IntBuffer(ArrayUtil.toInts(data), copy)
		End Function

		Public Overridable Function createLong(ByVal data() As Long) As DataBuffer
			Return createLong(data, True)
		End Function

		Public Overridable Function createLong(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer
			Return New LongBuffer(data, copy)
		End Function

		Public Overridable Function createLong(ByVal data() As Long, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New LongBuffer(data, True, workspace)
		End Function

		Public Overridable Function createLong(ByVal length As Long) As DataBuffer
			Return New LongBuffer(length)
		End Function

		Public Overridable Function createLong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New LongBuffer(length, initialize)
		End Function

		Public Overridable Function createLong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New LongBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createDouble(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Return New DoubleBuffer(data, copy)
		End Function

		Public Overridable Function createDouble(ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return createDouble(data, True, workspace)
		End Function

		Public Overridable Function createDouble(ByVal data() As Double, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New DoubleBuffer(data, copy, workspace)
		End Function

		Public Overridable Function createDouble(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New DoubleBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createFloat(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Return New FloatBuffer(ArrayUtil.toFloats(data), copy)
		End Function

		Public Overridable Function createInt(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Return New IntBuffer(ArrayUtil.toInts(data), copy)
		End Function

		Public Overridable Function createDouble(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Return New DoubleBuffer(data, copy)
		End Function

		Public Overridable Function createFloat(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Return New FloatBuffer(data, copy)
		End Function

		Public Overridable Function createFloat(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New FloatBuffer(data, copy, workspace)
		End Function

		Public Overridable Function createInt(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Return New IntBuffer(ArrayUtil.toInts(data), copy)
		End Function

		''' <summary>
		''' Create a data buffer based on the
		''' given pointer, data buffer opType,
		''' and length of the buffer
		''' </summary>
		''' <param name="pointer"> the pointer to use </param>
		''' <param name="type">    the opType of buffer </param>
		''' <param name="length">  the length of the buffer </param>
		''' <param name="indexer"> the indexer for the pointer </param>
		''' <returns> the data buffer
		''' backed by this pointer with the given
		''' opType and length. </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public DataBuffer create(org.bytedeco.javacpp.Pointer pointer, DataType type, long length, @NonNull Indexer indexer)
		Public Overridable Function create(ByVal pointer As Pointer, ByVal type As DataType, ByVal length As Long, ByVal indexer As Indexer) As DataBuffer
			Select Case type.innerEnumValue
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BOOL
					Return New BoolBuffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BYTE
					Return New Int8Buffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UBYTE
					Return New UInt8Buffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT16
					Return New UInt16Buffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT32
					Return New UInt32Buffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.UINT64
					Return New UInt64Buffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.SHORT
					Return New Int16Buffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.INT
					Return New IntBuffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.LONG
					Return New LongBuffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.HALF
					Return New HalfBuffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.BFLOAT16
					Return New BFloat16Buffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.FLOAT
					Return New FloatBuffer(pointer, indexer, length)
				Case org.nd4j.linalg.api.buffer.DataType.InnerEnum.DOUBLE
					Return New DoubleBuffer(pointer, indexer, length)
			End Select
			Throw New System.ArgumentException("Invalid opType " & type)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public DataBuffer create(org.bytedeco.javacpp.Pointer pointer, org.bytedeco.javacpp.Pointer specialPointer, DataType type, long length, @NonNull Indexer indexer)
		Public Overridable Function create(ByVal pointer As Pointer, ByVal specialPointer As Pointer, ByVal type As DataType, ByVal length As Long, ByVal indexer As Indexer) As DataBuffer
			Return create(pointer, type, length, indexer)
		End Function

		''' <param name="doublePointer"> </param>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function create(ByVal doublePointer As DoublePointer, ByVal length As Long) As DataBuffer
			doublePointer.capacity(length)
			doublePointer.limit(length)
			doublePointer.position(0)
			Return New DoubleBuffer(doublePointer, DoubleIndexer.create(doublePointer), length)
		End Function

		''' <param name="intPointer"> </param>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function create(ByVal intPointer As IntPointer, ByVal length As Long) As DataBuffer
			intPointer.capacity(length)
			intPointer.limit(length)
			intPointer.position(0)
			Return New IntBuffer(intPointer, IntIndexer.create(intPointer), length)
		End Function

		''' <param name="floatPointer"> </param>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function create(ByVal floatPointer As FloatPointer, ByVal length As Long) As DataBuffer
			floatPointer.capacity(length)
			floatPointer.limit(length)
			floatPointer.position(0)
			Return New FloatBuffer(floatPointer, FloatIndexer.create(floatPointer), length)
		End Function


		Public Overridable Function createHalf(ByVal length As Long) As DataBuffer
			Return New HalfBuffer(length)
		End Function

		Public Overridable Function createHalf(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New HalfBuffer(length, initialize)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Double) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Integer) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As SByte, ByVal copy As Boolean) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Single) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Double) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Integer) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function


		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="length"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal length As Integer) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		Public Overridable Function createHalf(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		Public Overridable Function createHalf(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		Public Overridable Function createHalf(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Throw New System.NotSupportedException("FP16 isn't supported for CPU yet")
		End Function

		Public Overridable Function intBufferClass() As Type Implements DataBufferFactory.intBufferClass
			Return GetType(IntBuffer)
		End Function

		Public Overridable Function longBufferClass() As Type Implements DataBufferFactory.longBufferClass
			Return GetType(LongBuffer)
		End Function

		Public Overridable Function halfBufferClass() As Type Implements DataBufferFactory.halfBufferClass
			Return Nothing 'Not yet supported
		End Function

		Public Overridable Function floatBufferClass() As Type Implements DataBufferFactory.floatBufferClass
			Return GetType(FloatBuffer)
		End Function

		Public Overridable Function doubleBufferClass() As Type Implements DataBufferFactory.doubleBufferClass
			Return GetType(DoubleBuffer)
		End Function

		Public Overridable Function createUtf8Buffer(ByVal data() As SByte, ByVal product As Long) As DataBuffer
			Return New Utf8Buffer(data, product)
		End Function
	End Class

End Namespace