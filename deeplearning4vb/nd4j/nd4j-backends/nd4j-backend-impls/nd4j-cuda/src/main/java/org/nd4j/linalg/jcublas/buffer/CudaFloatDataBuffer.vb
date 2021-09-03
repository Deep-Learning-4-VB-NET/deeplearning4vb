Imports System
Imports System.IO
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports Slf4j = lombok.extern.slf4j.Slf4j

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
	''' Cuda float buffer
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudaFloatDataBuffer extends BaseCudaDataBuffer
	<Serializable>
	Public Class CudaFloatDataBuffer
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
			MyBase.New(length, 4)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean)
			MyBase.New(length, 4, initialize)
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(length, 4, initialize, workspace)
		End Sub


		Public Sub New(ByVal length As Long, ByVal elementSize As Integer)
			MyBase.New(length, elementSize)
		End Sub

		Public Sub New(ByVal length As Long, ByVal elementSize As Integer, ByVal offset As Long)
			MyBase.New(length, elementSize, offset)
		End Sub

		''' <summary>
		''' Initialize the opType of this buffer
		''' </summary>
		Protected Friend Overrides Sub initTypeAndSize()
			elementSize_Conflict = 4
			type = DataType.FLOAT
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

		Public Sub New(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(data, copy, workspace)
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

		Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
			Return New CudaFloatDataBuffer(length)
		End Function


		Public Overrides Function getDoublesAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Double()
			Return ArrayUtil.toDoubles(getFloatsAt(offset, inc, length))
		End Function


		Public Overrides WriteOnly Property Data As Integer()
			Set(ByVal data() As Integer)
				setData(ArrayUtil.toFloats(data))
			End Set
		End Property



		Public Overrides WriteOnly Property Data As Double()
			Set(ByVal data() As Double)
				setData(ArrayUtil.toFloats(data))
			End Set
		End Property

		Public Overrides Function asBytes() As SByte()
			Dim data() As Single = asFloat()
			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			For i As Integer = 0 To data.Length - 1
				Try
					dos.writeFloat(data(i))
				Catch e As IOException
					log.error("",e)
				End Try
			Next i
			Return bos.toByteArray()
		End Function

		Public Overrides Function dataType() As DataType
			Return type
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
			Return New CudaFloatDataBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Single) As DataBuffer
			Return New CudaFloatDataBuffer(data)
		End Function

		Public Overrides Function create(ByVal data() As Integer) As DataBuffer
			Return New CudaFloatDataBuffer(data)
		End Function

		Public Overrides Sub flush()

		End Sub



	End Class

End Namespace