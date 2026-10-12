Imports System
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
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
	''' Cuda double  buffer
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class CudaDoubleDataBuffer
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
			MyBase.New(length, 8)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean)
			MyBase.New(length, 8, initialize)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(length, 8, initialize, workspace)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer)
			MyBase.New(length, elementSize)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal offset As Long)
			MyBase.New(length, elementSize, offset)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, copy,0, workspace)
		End Sub

		''' <summary>
		''' Initialize the opType of this buffer
		''' </summary>
		Protected Friend Overrides Sub initTypeAndSize()
			type = DataType.DOUBLE
			elementSize_Conflict = 8
		End Sub

		Public Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
			MyBase.New(underlyingBuffer, length, offset)
		End Sub


		''' <summary>
		''' Instantiate based on the given data
		''' </summary>
		''' <param name="data"> the data to instantiate with </param>
		Public Sub New(ByVal data() As Double)
			Me.New(data.Length)
			setData(data)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, copy, offset, workspace)
		End Sub

		Public Sub New(ByVal data() As Single)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long)
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

		Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
			Return New CudaDoubleDataBuffer(length)
		End Function

		Public Overrides WriteOnly Property Data As Integer()
			Set(ByVal data() As Integer)
				setData(ArrayUtil.toDoubles(data))
			End Set
		End Property

		Public Overrides WriteOnly Property Data As Single()
			Set(ByVal data() As Single)
				setData(ArrayUtil.toDoubles(data))
			End Set
		End Property



		Public Overrides Function create(ByVal data() As Double) As DataBuffer
			Return New CudaDoubleDataBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Single) As DataBuffer
			Return New CudaDoubleDataBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Integer) As DataBuffer
			Return New CudaDoubleDataBuffer(data)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream stream) throws java.io.IOException
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)
			stream.defaultWriteObject()

			If HostPointer Is Nothing Then
				stream.writeInt(0)
			Else
				Dim arr() As Double = Me.asDouble()

				stream.writeInt(arr.Length)
				For i As Integer = 0 To arr.Length - 1
					stream.writeDouble(arr(i))
				Next i
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws java.io.IOException, ClassNotFoundException
		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)
			stream.defaultReadObject()

			Dim n As Integer = stream.readInt()
			Dim arr(n - 1) As Double

			For i As Integer = 0 To n - 1
				arr(i) = stream.readDouble()
			Next i

			Me.length_Conflict = n
			Me.elementSize_Conflict = 8

			Me.allocationPoint = AtomicAllocator.Instance.allocateMemory(Me, New AllocationShape(length_Conflict, elementSize_Conflict, DataType.DOUBLE), False)

			setData(arr)
		End Sub

	End Class

End Namespace