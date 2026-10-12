Imports System
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace

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
	''' Double buffer implementation of data buffer
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class DoubleBuffer
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

		Public Sub New(ByVal buffer As ByteBuffer, ByVal dataType As DataType, ByVal length As Long, ByVal offset As Long)
			MyBase.New(buffer, dataType, length, offset)
		End Sub

		Public Sub New(ByVal length As Long)
			MyBase.New(length)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean)
			MyBase.New(length, initialize)
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

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(length, initialize, workspace)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal workspace As MemoryWorkspace)
			Me.New(data, True, workspace)
		End Sub

		Public Sub New(ByVal floats() As Double, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(floats, copy, workspace)
		End Sub

		Public Sub New(ByVal data() As Double)
			MyBase.New(data)
		End Sub

		Public Sub New(ByVal data() As Integer)
			Me.New(data, True)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copyOnOps As Boolean)
			MyBase.New(data, copyOnOps)
		End Sub

		Public Sub New(ByVal data() As Integer, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal data() As Single)
			Me.New(data, True)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copyOnOps As Boolean)
			MyBase.New(data, copyOnOps)
		End Sub

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal doubles() As Double, ByVal copy As Boolean)
			MyBase.New(doubles, copy)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long)
			MyBase.New(data, copy, offset)
		End Sub

		Public Sub New(ByVal data() As Double, ByVal copy As Boolean, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, copy, offset, workspace)
		End Sub

		Public Overrides Function getFloat(ByVal i As Long) As Single
			Return CSng(getDouble(i))
		End Function

		Public Overrides Function getNumber(ByVal i As Long) As Number
			Return getDouble(i)
		End Function

		Public Overrides Function create(ByVal data() As Double) As DataBuffer
			Return New DoubleBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Single) As DataBuffer
			Return New DoubleBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Integer) As DataBuffer
			Return New DoubleBuffer(data)
		End Function

		Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
			Return New DoubleBuffer(length)
		End Function


		Public Overrides Sub flush()
		End Sub
		Protected Friend Overrides Sub initTypeAndSize()
			elementSize_Conflict = 8
			type = DataType.DOUBLE
		End Sub



	End Class

End Namespace