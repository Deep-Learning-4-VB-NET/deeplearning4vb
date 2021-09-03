Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports LongPointer = org.bytedeco.javacpp.LongPointer
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
	''' UTF-8 buffer
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class Utf8Buffer
		Inherits BaseCpuDataBuffer

		Protected Friend references As ICollection(Of Pointer) = New List(Of Pointer)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected long numWords = 0;
		Protected Friend numWords As Long = 0

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
			''' <summary>
			''' Special case: we're creating empty buffer for length strings, each of 0 chars
			''' </summary>
			MyBase.New((length + 1) * 8, True)
			numWords = length
		End Sub

		Public Sub New(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
			''' <summary>
			''' Special case: we're creating empty buffer for length strings, each of 0 chars
			''' </summary>

			MyBase.New((length + 1) * 8, True, workspace)
			numWords = length
		End Sub

		Public Sub New(ByVal buffer As ByteBuffer, ByVal dataType As DataType, ByVal length As Long, ByVal offset As Long)
			MyBase.New(buffer, dataType, length, offset)
		End Sub

		Public Sub New(ByVal ints() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace)
			MyBase.New(ints, copy, workspace)
		End Sub

		Public Sub New(ByVal data() As SByte, ByVal numWords As Long)
			MyBase.New(data.Length, False)

			Dim bp As val = CType(pointer_Conflict, BytePointer)
			bp.put(data)
			Me.numWords = numWords
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
			Me.numWords = length
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Utf8Buffer(@NonNull Collection<String> strings)
		Public Sub New(ByVal strings As ICollection(Of String))
			MyBase.New(Utf8Buffer.stringBufferRequiredLength(strings), False)

			' at this point we should have fully allocated buffer, time to fill length
			Dim headerLength As val = (strings.size() + 1) * 8
			Dim headerPointer As val = New LongPointer(Pointer)
			Dim dataPointer As val = New BytePointer(Pointer)
			Me.pointer_Conflict.retainReference()
			numWords = strings.size()

			Dim cnt As Long = 0
			Dim currentLength As Long = 0
			For Each s As val In strings
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: headerPointer.put(cnt++, currentLength);
				headerPointer.put(cnt, currentLength)
					cnt += 1
				Dim length As val = s.length()
				Dim chars As val = s.toCharArray()

				' putting down chars
				For e As Integer = 0 To length - 1
					Dim b As val = CSByte(Math.Truncate(chars(e)))
					Dim idx As val = headerLength + currentLength + e
					dataPointer.put(idx, b)
				Next e

				currentLength += length
			Next s
			headerPointer.put(cnt, currentLength)
		End Sub


		Private ReadOnly Property Pointer As Pointer
			Get
				SyncLock Me
					Return Me.pointer_Conflict
				End SyncLock
			End Get
		End Property

		Public Overridable Function getString(ByVal index As Long) As String
			SyncLock Me
				If index > numWords Then
					Throw New System.ArgumentException("Requested index [" & index & "] is above actual number of words stored: [" & numWords & "]")
				End If
        
				Dim headerPointer As val = New LongPointer(Pointer)
				Dim dataPointer As val = CType(Pointer, BytePointer)
        
				Dim start As val = headerPointer.get(index)
				Dim [end] As val = headerPointer.get(index + 1)
        
				If [end] - start > Integer.MaxValue Then
					Throw New System.InvalidOperationException("Array is too long for Java")
				End If
        
				Dim dataLength As val = CInt([end] - start)
				Dim bytes As val = New SByte(dataLength - 1){}
        
				Dim headerLength As val = (numWords + 1) * 8
        
				For e As Integer = 0 To dataLength - 1
					Dim idx As val = headerLength + start + e
					bytes(e) = dataPointer.get(idx)
				Next e
        
				Try
					Return New String(bytes, "UTF-8")
				Catch e As UnsupportedEncodingException
					Throw New Exception(e)
				End Try
			End SyncLock
		End Function

		Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
			Return New Utf8Buffer(length)
		End Function

		Public Overrides Function create(ByVal data() As Double) As DataBuffer
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function create(ByVal data() As Single) As DataBuffer
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function create(ByVal data() As Integer) As DataBuffer
			Throw New System.NotSupportedException()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static long stringBufferRequiredLength(@NonNull Collection<String> strings)
		Private Shared Function stringBufferRequiredLength(ByVal strings As ICollection(Of String)) As Long
			' header size first
			Dim size As Long = (strings.size() + 1) * 8

			For Each s As val In strings
				size += s.length()
			Next s

			Return size
		End Function

		Public Overridable Overloads Sub put(ByVal index As Long, ByVal pointer As Pointer)
			Throw New System.NotSupportedException()
			'references.add(pointer);
			'((LongIndexer) indexer).put(index, pointer.address());
		End Sub

		''' <summary>
		''' Initialize the opType of this buffer
		''' </summary>
		Protected Friend Overrides Sub initTypeAndSize()
			elementSize_Conflict = 1
			type = DataType.UTF8
		End Sub


	End Class

End Namespace