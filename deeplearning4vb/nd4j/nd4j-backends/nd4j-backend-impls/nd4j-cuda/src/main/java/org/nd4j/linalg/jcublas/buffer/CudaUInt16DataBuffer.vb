﻿Imports System
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.jcublas.buffer


	''' <summary>
	''' Cuda Short buffer
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	<Serializable>
	Public Class CudaUInt16DataBuffer
		Inherits BaseCudaDataBuffer

		''' <summary>
		''' Meant for creating another view of a buffer
		''' </summary>
		''' <param name="pointer"> the underlying buffer to create a view from </param>
		''' <param name="indexer"> the indexer for the pointer </param>
		''' <param name="length">  the length of the view </param>
		Public Sub New(ByVal pointer As Pointer, ByVal indexer As Indexer, ByVal length As Long)
			MyBase.New(pointer, indexer, length)
		End Sub

		Public Sub New(ByVal pointer As Pointer, ByVal specialPointer As Pointer, ByVal indexer As Indexer, ByVal length As Long)
			MyBase.New(pointer, specialPointer, indexer, length)
		End Sub

		Public Sub New(ByVal buffer As ByteBuffer, ByVal dataType As DataType, ByVal length As Long, ByVal offset As Long)
			MyBase.New(buffer, dataType, length, offset)
		End Sub

		''' <summary>
		''' Base constructor
		''' </summary>
		''' <param name="length"> the length of the buffer </param>
		Public Sub New(ByVal length As Long)
			MyBase.New(length, 2)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean)
			MyBase.New(length, 2, initialize)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer)
			MyBase.New(length, elementSize)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal offset As Long)
			MyBase.New(length, elementSize, offset)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(length, 2, initialize, workspace)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, copy,0, workspace)
		End Sub

		''' <summary>
		''' Initialize the opType of this buffer
		''' </summary>
		Protected Friend Overrides Sub initTypeAndSize()
			elementSize_Conflict = 2
			type = DataType.UINT16
		End Sub

		Public Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
			MyBase.New(underlyingBuffer, length, offset)
		End Sub

		Public Sub New(ByVal buffer() As Single)
			MyBase.New(buffer)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, copy, offset, workspace)
		End Sub

		Public Sub New(ByVal data() As Double)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal data() As Integer)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Overrides Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean, ByVal inc As Long)

			If indices.Length <> data.Length Then
				Throw New System.ArgumentException("Indices and data length must be the same")
			End If
			If indices.Length > length() Then
				Throw New System.ArgumentException("More elements than space to assign. This buffer is of length " & length() & " where the indices are of length " & data.Length)
			End If

			If contiguous Then
	'            long offset = indices[0];
	'            Pointer p = Pointer.to(data);
	'            set(offset, data.length, p, inc);
	'            
				Throw New System.NotSupportedException()
			Else
				Throw New System.NotSupportedException("Only contiguous supported")
			End If
		End Sub

		Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
			Return New CudaUInt16DataBuffer(length)
		End Function


		Public Overrides Function getFloatsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Single()
			Return MyBase.getFloatsAt(offset, inc, length)
		End Function

		Public Overrides Function getDoublesAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Double()
			Return ArrayUtil.toDoubles(getFloatsAt(offset, inc, length))
		End Function



		Public Overrides WriteOnly Property Data As Single()
			Set(ByVal data() As Single)
				setData(ArrayUtil.toShorts(data))
			End Set
		End Property

		Public Overrides WriteOnly Property Data As Integer()
			Set(ByVal data() As Integer)
				setData(ArrayUtil.toShorts(data))
			End Set
		End Property



		Public Overrides WriteOnly Property Data As Double()
			Set(ByVal data() As Double)
				setData(ArrayUtil.toFloats(data))
			End Set
		End Property

		Public Overrides Function dataType() As DataType
			Return DataType.UINT16
		End Function

		Public Overrides Function asFloat() As Single()
			Return MyBase.asFloat()
		End Function

		Public Overrides Function asDouble() As Double()
			Return ArrayUtil.toDoubles(asFloat())
		End Function

		Public Overrides Function asInt() As Integer()
			Return ArrayUtil.toInts(asFloat())
		End Function


		Public Overrides Function getDouble(ByVal i As Long) As Double
			Return MyBase.getFloat(i)
		End Function


		Public Overrides Function create(ByVal data() As Double) As DataBuffer
			Return New CudaUInt16DataBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Single) As DataBuffer
			Return New CudaUInt16DataBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Integer) As DataBuffer
			Return New CudaUInt16DataBuffer(data)
		End Function

		Public Overrides Sub flush()

		End Sub



	End Class

End Namespace