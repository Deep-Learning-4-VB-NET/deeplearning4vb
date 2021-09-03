Imports org.bytedeco.javacpp
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

Namespace org.nd4j.jita.allocator.pointers

	''' <summary>
	''' This class is simple logic-less holder for pointers derived from CUDA.
	''' 
	''' PLEASE NOTE:
	''' 1. All pointers are blind, and do NOT care about length/capacity/offsets/strides whatever
	''' 2. They are really blind. Even data opType is unknown.
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class CudaPointer
		Inherits Pointer

		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(CudaPointer))


		Public Sub New(ByVal pointer As Pointer)
			Me.address = pointer.address()
			Me.capacity = pointer.capacity()
			Me.limit = pointer.limit()
			Me.position = pointer.position()
		End Sub

		Public Sub New(ByVal pointer As Pointer, ByVal capacity As Long)
			Me.address = pointer.address()
			Me.capacity = capacity
			Me.limit = capacity
			Me.position = 0

			'   logger.info("Creating pointer: ["+this.address+"],  capacity: ["+this.capacity+"]");
		End Sub

		Public Sub New(ByVal pointer As Pointer, ByVal capacity As Long, ByVal byteOffset As Long)
			Me.address = pointer.address() + byteOffset
			Me.capacity = capacity
			Me.limit = capacity
			Me.position = 0
		End Sub

		Public Sub New(ByVal address As Long)
			Me.address = address
		End Sub

		Public Sub New(ByVal address As Long, ByVal capacity As Long)
			Me.address = address
			Me.capacity = capacity
			Me.limit = capacity
			Me.position = 0
		End Sub

		Public Overridable Function asNativePointer() As Pointer
			Return New Pointer(Me)
		End Function

		Public Overridable Function asFloatPointer() As FloatPointer
			Return New FloatPointer(Me)
		End Function

		Public Overridable Function asLongPointer() As LongPointer
			Return New LongPointer(Me)
		End Function

		Public Overridable Function asDoublePointer() As DoublePointer
			Return New DoublePointer(Me)
		End Function

		Public Overridable Function asIntPointer() As IntPointer
			Return New IntPointer(Me)
		End Function

		Public Overridable Function asShortPointer() As ShortPointer
			Return New ShortPointer(Me)
		End Function

		Public Overridable Function asBytePointer() As BytePointer
			Return New BytePointer(Me)
		End Function

		Public Overridable Function asBooleanPointer() As BooleanPointer
			Return New BooleanPointer(Me)
		End Function

		Public Overridable ReadOnly Property NativePointer As Long
			Get
				Return address()
			End Get
		End Property

		''' <summary>
		''' Returns 1 for Pointer or BytePointer else {@code Loader.sizeof(getClass())} or -1 on error.
		''' </summary>
		Public Overrides Function sizeof() As Integer
			Return 4
		End Function
	End Class

End Namespace