Imports System
Imports NonNull = lombok.NonNull
Imports LongPointer = org.bytedeco.javacpp.LongPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports LongIndexer = org.bytedeco.javacpp.indexer.LongIndexer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpaqueDataBuffer = org.nd4j.nativeblas.OpaqueDataBuffer

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



	''' <summary>
	''' Int buffer
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class LongBuffer
		Inherits BaseCpuDataBuffer

		''' <summary>
		''' Meant for creating another view of a buffer
		''' </summary>
		''' <param name="pointer"> the underlying buffer to create a view from </param>
		''' <param name="indexer"> the indexer for the pointer </param>
		''' <param name="length">  the length of the view </param>
		Public Sub New(ByVal pointer As Pointer, ByVal indexer As Indexer, ByVal length As Long)
			MyBase.New(pointer, indexer, length)
		End Sub

		Public Sub New(ByVal length As Long)
			MyBase.New(length)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean)
			MyBase.New(length, initialize)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(length, initialize, workspace)
		End Sub

		Public Sub New(ByVal buffer As ByteBuffer, ByVal dataType As DataType, ByVal length As Long, ByVal offset As Long)
			MyBase.New(buffer, dataType, length, offset)
		End Sub

		Public Sub New(ByVal ints() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(ints, copy, workspace)
		End Sub


		Public Sub New(ByVal data() As Double, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Long, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		Public Sub New(ByVal data() As Long, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, copy, workspace)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal length As Integer, ByVal elementSize As Integer)
			MyBase.New(length, elementSize)
		End Sub

		Public Sub New(ByVal length As Integer, ByVal elementSize As Integer, ByVal offset As Long)
			MyBase.New(length, elementSize, offset)
		End Sub

		Public Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
			MyBase.New(underlyingBuffer, length, offset)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LongBuffer(@NonNull Pointer hostPointer, long numberOfElements)
		Public Sub New(ByVal hostPointer As Pointer, ByVal numberOfElements As Long)
			Me.allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES
			Me.offset_Conflict = 0
			Me.originalOffset_Conflict = 0
			Me.underlyingLength_Conflict = numberOfElements
			Me.length_Conflict = numberOfElements
			initTypeAndSize()

			Me.pointer_Conflict = (New PagedPointer(hostPointer, numberOfElements)).asLongPointer()
			indexer_Conflict = LongIndexer.create(CType(Me.pointer_Conflict, LongPointer))

			' we still want this buffer to have native representation

			ptrDataBuffer = OpaqueDataBuffer.externalizedDataBuffer(numberOfElements, DataType.INT64, Me.pointer_Conflict, Nothing)

			Nd4j.DeallocatorService.pickObject(Me)
		End Sub

		Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
			Return New LongBuffer(length)
		End Function

		Public Sub New(ByVal data() As Integer)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data() As Double)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data() As Single)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data() As Long)
			MyBase.New(data, True)
		End Sub

		Public Overrides Function create(ByVal data() As Double) As DataBuffer
			Return New LongBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Single) As DataBuffer
			Return New LongBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Integer) As DataBuffer
			Return New LongBuffer(data)
		End Function

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean)
			MyBase.New(data, copy)
		End Sub

		''' <summary>
		''' Initialize the opType of this buffer
		''' </summary>
		Protected Friend Overrides Sub initTypeAndSize()
			elementSize_Conflict = 8
			type = DataType.LONG
		End Sub


	End Class

End Namespace