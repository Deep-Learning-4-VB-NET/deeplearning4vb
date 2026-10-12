Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports org.bytedeco.javacpp.indexer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataBufferFactory = org.nd4j.linalg.api.buffer.factory.DataBufferFactory
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports org.nd4j.linalg.jcublas.buffer
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

Namespace org.nd4j.linalg.jcublas.buffer.factory


	''' <summary>
	''' Creates cuda buffers
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudaDataBufferFactory implements org.nd4j.linalg.api.buffer.factory.DataBufferFactory
	Public Class CudaDataBufferFactory
		Implements DataBufferFactory

'JAVA TO VB CONVERTER NOTE: The field allocationMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend allocationMode_Conflict As DataBuffer.AllocationMode

		Public Overridable WriteOnly Property AllocationMode Implements DataBufferFactory.setAllocationMode As DataBuffer.AllocationMode
			Set(ByVal allocationMode As DataBuffer.AllocationMode)
				Me.allocationMode_Conflict = allocationMode
			End Set
		End Property

		Public Overridable Function allocationMode() As DataBuffer.AllocationMode Implements DataBufferFactory.allocationMode
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

		Public Overridable Function create(ByVal underlyingBuffer As ByteBuffer, ByVal dataType As DataType, ByVal length As Long, ByVal offset As Long) As DataBuffer Implements DataBufferFactory.create
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return New CudaDoubleDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.FLOAT
					Return New CudaFloatDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.HALF
					Return New CudaHalfDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.BFLOAT16
					Return New CudaBfloat16DataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.LONG
					Return New CudaLongDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.INT
					Return New CudaIntDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.SHORT
					Return New CudaShortDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.UBYTE
					Return New CudaUByteDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.UINT16
					Return New CudaUInt16DataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.UINT32
					Return New CudaUInt32DataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.UINT64
					Return New CudaUInt64DataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.BYTE
					Return New CudaByteDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.BOOL
					Return New CudaBoolDataBuffer(underlyingBuffer, dataType, length, offset)
				Case DataType.InnerEnum.UTF8
					Return New CudaUtf8Buffer(underlyingBuffer, dataType, length, offset)
				Case Else
					Throw New System.InvalidOperationException("Unknown datatype used: [" & dataType & "]")
			End Select
		End Function

		Public Overridable Function create(ByVal underlyingBuffer As DataBuffer, ByVal offset As Long, ByVal length As Long) As DataBuffer Implements DataBufferFactory.create
			Select Case underlyingBuffer.dataType()
				Case [DOUBLE]
					Return New CudaDoubleDataBuffer(underlyingBuffer, length, offset)
				Case FLOAT
					Return New CudaFloatDataBuffer(underlyingBuffer, length, offset)
				Case HALF
					Return New CudaHalfDataBuffer(underlyingBuffer, length, offset)
				Case BFLOAT16
					Return New CudaBfloat16DataBuffer(underlyingBuffer, length, offset)
				Case UINT64
					Return New CudaUInt64DataBuffer(underlyingBuffer, length, offset)
				Case [LONG]
					Return New CudaLongDataBuffer(underlyingBuffer, length, offset)
				Case UINT32
					Return New CudaUInt32DataBuffer(underlyingBuffer, length, offset)
				Case INT
					Return New CudaIntDataBuffer(underlyingBuffer, length, offset)
				Case UINT16
					Return New CudaUInt16DataBuffer(underlyingBuffer, length, offset)
				Case [SHORT]
					Return New CudaShortDataBuffer(underlyingBuffer, length, offset)
				Case UBYTE
					Return New CudaUByteDataBuffer(underlyingBuffer, length, offset)
				Case [BYTE]
					Return New CudaByteDataBuffer(underlyingBuffer, length, offset)
				Case BOOL
					Return New CudaBoolDataBuffer(underlyingBuffer, length, offset)
				Case UTF8
					Return New CudaUtf8Buffer(underlyingBuffer, length, offset)
				Case Else
					Throw New ND4JIllegalStateException("Unknown data buffer type: " & underlyingBuffer.dataType().ToString())
			End Select
		End Function

		''' <summary>
		''' This method will create new DataBuffer of the same dataType & same length
		''' </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Overridable Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean) As DataBuffer Implements DataBufferFactory.createSame
			Select Case buffer.dataType()
				Case INT
					Return createInt(buffer.length(), init)
				Case FLOAT
					Return createFloat(buffer.length(), init)
				Case [DOUBLE]
					Return createDouble(buffer.length(), init)
				Case BFLOAT16
					Return createBfloat16(buffer.length(), init)
				Case HALF
					Return createHalf(buffer.length(), init)
				Case Else
					Throw New System.NotSupportedException("Unknown dataType: " & buffer.dataType())
			End Select
		End Function

		''' <summary>
		''' This method will create new DataBuffer of the same dataType & same length
		''' </summary>
		''' <param name="buffer"> </param>
		''' <param name="workspace">
		''' @return </param>
		Public Overridable Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createSame
			Select Case buffer.dataType()
				Case INT
					Return createInt(buffer.length(), init, workspace)
				Case FLOAT
					Return createFloat(buffer.length(), init, workspace)
				Case [DOUBLE]
					Return createDouble(buffer.length(), init, workspace)
				Case BFLOAT16
					Return createBfloat16(buffer.length(), init, workspace)
				Case HALF
					Return createHalf(buffer.length(), init, workspace)
				Case Else
					Throw New System.NotSupportedException("Unknown dataType: " & buffer.dataType())
			End Select
		End Function

		Public Overridable Function createFloat(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
			Return createFloat(data, True, workspace)
		End Function

		Public Overridable Function createFloat(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, copy, workspace)
		End Function

		Public Overridable Function createInt(ByVal data() As Integer, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, workspace)
		End Function

		Public Overridable Function createInt(ByVal data() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, copy, workspace)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(length, 8, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(length, 4, length)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(length, 4, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, True, offset, workspace)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As SByte, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(ArrayUtil.toDoubleArray(data), True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As SByte, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(ArrayUtil.toDoubleArray(data), True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, True, offset, workspace)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data, copy, offset)
		End Function

		Public Overridable Function createDouble(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(length)
		End Function

		Public Overridable Function createDouble(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(length, initialize)
		End Function

		Public Overridable Function createFloat(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(length)
		End Function

		Public Overridable Function createFloat(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(length, initialize)
		End Function

		Public Overridable Function createFloat(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.create
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.UINT16
					Return New CudaUInt16DataBuffer(length, initialize)
				Case DataType.InnerEnum.UINT32
					Return New CudaUInt32DataBuffer(length, initialize)
				Case DataType.InnerEnum.UINT64
					Return New CudaUInt64DataBuffer(length, initialize)
				Case DataType.InnerEnum.LONG
					Return New CudaLongDataBuffer(length, initialize)
				Case DataType.InnerEnum.INT
					Return New CudaIntDataBuffer(length, initialize)
				Case DataType.InnerEnum.SHORT
					Return New CudaShortDataBuffer(length, initialize)
				Case DataType.InnerEnum.UBYTE
					Return New CudaUByteDataBuffer(length, initialize)
				Case DataType.InnerEnum.BYTE
					Return New CudaByteDataBuffer(length, initialize)
				Case DataType.InnerEnum.DOUBLE
					Return New CudaDoubleDataBuffer(length, initialize)
				Case DataType.InnerEnum.FLOAT
					Return New CudaFloatDataBuffer(length, initialize)
				Case DataType.InnerEnum.BFLOAT16
					Return New CudaBfloat16DataBuffer(length, initialize)
				Case DataType.InnerEnum.HALF
					Return New CudaHalfDataBuffer(length, initialize)
				Case DataType.InnerEnum.BOOL
					Return New CudaBoolDataBuffer(length, initialize)
				Case DataType.InnerEnum.UTF8
					Return New CudaUtf8Buffer(length, True)
				Case Else
					Throw New System.NotSupportedException("Unknown data type: [" & dataType & "]")
			End Select
		End Function

		Public Overridable Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.create
			If workspace Is Nothing Then
				Return create(dataType, length, initialize)
			End If

			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.UINT16
					Return New CudaUInt16DataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.UINT32
					Return New CudaUInt32DataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.UINT64
					Return New CudaUInt64DataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.LONG
					Return New CudaLongDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.INT
					Return New CudaIntDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.SHORT
					Return New CudaShortDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.UBYTE
					Return New CudaUByteDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.BYTE
					Return New CudaByteDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.DOUBLE
					Return New CudaDoubleDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.FLOAT
					Return New CudaFloatDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.HALF
					Return New CudaHalfDataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.BFLOAT16
					Return New CudaBfloat16DataBuffer(length, initialize, workspace)
				Case DataType.InnerEnum.BOOL
					Return New CudaBoolDataBuffer(length, initialize, workspace)
				Case Else
					Throw New System.NotSupportedException("Unknown data type: [" & dataType & "]")
			End Select
		End Function

		Public Overridable Function createInt(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(length)
		End Function

		Public Overridable Function createBFloat16(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createBFloat16
			Return New CudaBfloat16DataBuffer(length)
		End Function

		Public Overridable Function createUInt(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createUInt
			Return New CudaUInt32DataBuffer(length)
		End Function

		Public Overridable Function createUShort(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createUShort
			Return New CudaUInt16DataBuffer(length)
		End Function

		Public Overridable Function createUByte(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createUByte
			Return New CudaUByteDataBuffer(length)
		End Function

		Public Overridable Function createULong(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createULong
			Return New CudaUInt64DataBuffer(length)
		End Function

		Public Overridable Function createBool(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createBool
			Return New CudaBoolDataBuffer(length)
		End Function

		Public Overridable Function createShort(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createShort
			Return New CudaShortDataBuffer(length)
		End Function

		Public Overridable Function createByte(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createByte
			Return New CudaByteDataBuffer(length)
		End Function

		Public Overridable Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createBFloat16
			Return New CudaBfloat16DataBuffer(length, initialize)
		End Function

		Public Overridable Function createUInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createUInt
			Return New CudaUInt32DataBuffer(length, initialize)
		End Function

		Public Overridable Function createUShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createUShort
			Return New CudaUInt16DataBuffer(length, initialize)
		End Function

		Public Overridable Function createUByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createUByte
			Return New CudaUByteDataBuffer(length, initialize)
		End Function

		Public Overridable Function createULong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createULong
			Return New CudaUInt64DataBuffer(length, initialize)
		End Function

		Public Overridable Function createBool(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createBool
			Return New CudaBoolDataBuffer(length, initialize)
		End Function

		Public Overridable Function createShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createShort
			Return New CudaShortDataBuffer(length, initialize)
		End Function

		Public Overridable Function createByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createByte
			Return New CudaByteDataBuffer(length, initialize)
		End Function

		Public Overridable Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createBFloat16
			Return New CudaBfloat16DataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createUInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createUInt
			Return New CudaUInt32DataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createUShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createUShort
			Return New CudaUInt16DataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createUByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createUByte
			Return New CudaUByteDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createULong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createULong
			Return New CudaUInt64DataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createBool(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createBool
			Return New CudaBoolDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createShort
			Return New CudaShortDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createByte
			Return New CudaByteDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(length, initialize)
		End Function

		Public Overridable Function createInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createDouble(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(ArrayUtil.toDoubles(data))
		End Function

		Public Overridable Function createFloat(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(ArrayUtil.toFloats(data))
		End Function

		Public Overridable Function createInt(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data)
		End Function

		Public Overridable Function createDouble(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data)
		End Function

		Public Overridable Function createFloat(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(ArrayUtil.toFloats(data))
		End Function

		Public Overridable Function createInt(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(ArrayUtil.toInts(data))
		End Function

		Public Overridable Function createDouble(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(ArrayUtil.toDoubles(data))
		End Function

		Public Overridable Function createFloat(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data)
		End Function

		Public Overridable Function createInt(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(ArrayUtil.toInts(data))
		End Function

		Public Overridable Function createDouble(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(ArrayUtil.toDouble(data))
		End Function

		Public Overridable Function createFloat(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(ArrayUtil.toFloats(data))
		End Function

		Public Overridable Function createInt(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data)
		End Function

		Public Overridable Function createLong(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createLong
			Return New CudaLongDataBuffer(data)
		End Function

		Public Overridable Function createDouble(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data)
		End Function

		Public Overridable Function createFloat(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(ArrayUtil.toFloats(data))
		End Function

		Public Overridable Function createInt(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(ArrayUtil.toInts(data))
		End Function

		Public Overridable Function createDouble(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(ArrayUtil.toDoubles(data))
		End Function

		Public Overridable Function createFloat(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(data)
		End Function

		Public Overridable Function createInt(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(ArrayUtil.toInts(data))
		End Function

		Public Overridable Function createDouble(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(ArrayUtil.toDoubles(data))
		End Function

		Public Overridable Function createFloat(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
			Return New CudaFloatDataBuffer(ArrayUtil.toFloats(data))
		End Function

		Public Overridable Function createInt(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
			Return New CudaIntDataBuffer(data)
		End Function

		''' <summary>
		''' Create a data buffer based on the
		''' given pointer, data buffer opType,
		''' and length of the buffer
		''' </summary>
		''' <param name="pointer"> the pointer to use </param>
		''' <param name="type">    the opType of buffer </param>
		''' <param name="length">  the length of the buffer </param>
		''' <param name="indexer"> </param>
		''' <returns> the data buffer
		''' backed by this pointer with the given
		''' opType and length. </returns>
		Public Overridable Function create(ByVal pointer As Pointer, ByVal type As DataType, ByVal length As Long, ByVal indexer As Indexer) As DataBuffer
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.UINT64
					Return New CudaUInt64DataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.LONG
					Return New CudaLongDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.UINT32
					Return New CudaUInt32DataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.INT
					Return New CudaIntDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.UINT16
					Return New CudaUInt16DataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.SHORT
					Return New CudaShortDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.UBYTE
					Return New CudaUByteDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.BYTE
					Return New CudaByteDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.DOUBLE
					Return New CudaDoubleDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.FLOAT
					Return New CudaFloatDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.HALF
					Return New CudaHalfDataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.BFLOAT16
					Return New CudaBfloat16DataBuffer(pointer, indexer, length)
				Case DataType.InnerEnum.BOOL
					Return New CudaBoolDataBuffer(pointer, indexer, length)
			End Select

			Throw New System.ArgumentException("Illegal dtype " & type)
		End Function

		Public Overridable Function create(ByVal pointer As Pointer, ByVal specialPointer As Pointer, ByVal type As DataType, ByVal length As Long, ByVal indexer As Indexer) As DataBuffer
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.UINT64
					Return New CudaUInt64DataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.LONG
					Return New CudaLongDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.UINT32
					Return New CudaUInt32DataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.INT
					Return New CudaIntDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.UINT16
					Return New CudaUInt16DataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.SHORT
					Return New CudaShortDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.UBYTE
					Return New CudaUByteDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.BYTE
					Return New CudaByteDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.DOUBLE
					Return New CudaDoubleDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.FLOAT
					Return New CudaFloatDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.HALF
					Return New CudaHalfDataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.BFLOAT16
					Return New CudaBfloat16DataBuffer(pointer, specialPointer, indexer, length)
				Case DataType.InnerEnum.BOOL
					Return New CudaBoolDataBuffer(pointer, specialPointer, indexer, length)
			End Select

			Throw New System.ArgumentException("Illegal dtype " & type)
		End Function

		''' <param name="doublePointer"> </param>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function create(ByVal doublePointer As DoublePointer, ByVal length As Long) As DataBuffer Implements DataBufferFactory.create
			Return New CudaDoubleDataBuffer(doublePointer,DoubleIndexer.create(doublePointer),length)
		End Function

		''' <param name="intPointer"> </param>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function create(ByVal intPointer As IntPointer, ByVal length As Long) As DataBuffer Implements DataBufferFactory.create
			Return New CudaIntDataBuffer(intPointer, IntIndexer.create(intPointer),length)
		End Function

		''' <param name="floatPointer"> </param>
		''' <param name="length">
		''' @return </param>
		Public Overridable Function create(ByVal floatPointer As FloatPointer, ByVal length As Long) As DataBuffer Implements DataBufferFactory.create
			Return New CudaFloatDataBuffer(floatPointer, FloatIndexer.create(floatPointer),length)
		End Function


		Public Overridable Function createHalf(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(length)
		End Function

		Public Overridable Function createHalf(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(length, initialize)
		End Function

		Public Overridable Function createBfloat16(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
			Return New CudaBfloat16DataBuffer(length, initialize)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, copy)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, copy)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, copy, offset)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, copy, offset)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, copy, offset)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, True, offset)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, True, offset)
		End Function

		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, True, offset, workspace)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, True, offset)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="data">   the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As SByte, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(ArrayUtil.toFloatArray(data), copy, offset)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <param name="copy"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, copy)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data)
		End Function

		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="data"> the data to create the buffer from </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data)
		End Function


		''' <summary>
		''' Creates a half-precision data buffer
		''' </summary>
		''' <param name="offset"> </param>
		''' <param name="length"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createHalf(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(length)
		End Function

		Public Overridable Function createDouble(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(length, initialize, workspace)
		End Function

		''' <summary>
		''' Creates a double data buffer
		''' </summary>
		''' <param name="data">      the data to create the buffer from </param>
		''' <param name="workspace"> </param>
		''' <returns> the new buffer </returns>
		Public Overridable Function createDouble(ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
			Return createDouble(data, True, workspace)
		End Function

		''' <summary>
		''' Creates a double data buffer
		''' </summary>
		''' <param name="data">      the data to create the buffer from </param>
		''' <param name="copy"> </param> </param>
		''' <param name="workspace"> <returns> the new buffer </returns>
		Public Overridable Function createDouble(ByVal data() As Double, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
			Return New CudaDoubleDataBuffer(data, copy, workspace)
		End Function

		Public Overridable Function createHalf(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createBfloat16(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
			Return New CudaBfloat16DataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createHalf(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
			Return createHalf(data, True, workspace)
		End Function

		Public Overridable Function createHalf(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
			Return New CudaHalfDataBuffer(data, copy, workspace)
		End Function


		Public Overridable Function intBufferClass() As Type Implements DataBufferFactory.intBufferClass
			Return GetType(CudaIntDataBuffer)
		End Function

		Public Overridable Function longBufferClass() As Type Implements DataBufferFactory.longBufferClass
			Return GetType(CudaLongDataBuffer)
		End Function

		Public Overridable Function halfBufferClass() As Type Implements DataBufferFactory.halfBufferClass
			Return GetType(CudaHalfDataBuffer) 'Not yet supported
		End Function

		Public Overridable Function floatBufferClass() As Type Implements DataBufferFactory.floatBufferClass
			Return GetType(CudaFloatDataBuffer)
		End Function

		Public Overridable Function doubleBufferClass() As Type Implements DataBufferFactory.doubleBufferClass
			Return GetType(CudaDoubleDataBuffer)
		End Function



		Public Overridable Function createLong(ByVal data() As Long) As DataBuffer Implements DataBufferFactory.createLong
			Return createLong(data, True)
		End Function

		Public Overridable Function createLong(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createLong
			Return New CudaLongDataBuffer(data, copy)
		End Function

		Public Overridable Function createLong(ByVal data() As Long, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createLong
			Return New CudaLongDataBuffer(data, workspace)
		End Function

		Public Overridable Function createLong(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createLong
			Return createLong(length, True)
		End Function

		Public Overridable Function createLong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createLong
			Return New CudaLongDataBuffer(length, initialize)
		End Function

		Public Overridable Function createLong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createLong
			Return New CudaLongDataBuffer(length, initialize, workspace)
		End Function

		Public Overridable Function createUtf8Buffer(ByVal data() As SByte, ByVal product As Long) As DataBuffer Implements DataBufferFactory.createUtf8Buffer
			Return New CudaUtf8Buffer(data, product)
		End Function
	End Class

End Namespace