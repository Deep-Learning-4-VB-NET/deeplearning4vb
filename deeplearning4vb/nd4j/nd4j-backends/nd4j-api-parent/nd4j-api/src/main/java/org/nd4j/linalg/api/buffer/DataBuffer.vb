Imports System.Collections.Generic
Imports System.IO
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
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

Namespace org.nd4j.linalg.api.buffer


	Public Interface DataBuffer
		Inherits AutoCloseable

		Friend Enum TypeEx

		End Enum

		ReadOnly Property GenerationId As Long


		''' <summary>
		''' Mainly used for backward compatability.
		''' Note that DIRECT and HEAP modes have been deprecated asd should not be used.
		''' </summary>
		Friend Enum AllocationMode

			<System.Obsolete>
			DIRECT
			<System.Obsolete>
			HEAP
			<System.Obsolete>
			JAVACPP
			<System.Obsolete>
			LONG_SHAPE ' long shapes will be used instead of int

			MIXED_DATA_TYPES ' latest generation of INDArrays support multiple data types, with information stored within shapeInfo "offset" field.
		End Enum

		''' <summary>
		''' Returns an underlying pointer if one exists
		''' @return
		''' </summary>
		Function pointer() As Pointer


		''' <summary>
		''' Returns the address of the pointer wrapped in a Pointer </summary>
		''' <returns> the address of the pointer wrapped in a Pointer </returns>
		Function addressPointer() As Pointer

		''' <summary>
		''' Returns the indexer for the buffer
		''' @return
		''' </summary>
		Function indexer() As Indexer


		''' <summary>
		''' Returns the address of the pointer </summary>
		''' <returns> the address of the pointer </returns>
		Function address() As Long

		''' <summary>
		''' Returns the address of platform-specific pointer:
		''' - for native backend that'll be host pointer
		''' - for cuda backend that'll be device pointer
		''' @return
		''' </summary>
		Function platformAddress() As Long

		''' <summary>
		''' Returns true if the underlying data source
		''' is the same for both buffers (referential equals) </summary>
		''' <param name="buffer"> whether the buffer is the same underlying data or not </param>
		''' <returns> true if both data buffers have the same
		''' underlying data SOURCE </returns>
		Function sameUnderlyingData(ByVal buffer As DataBuffer) As Boolean

		Sub read(ByVal s As DataInputStream, ByVal allocationMode As AllocationMode, ByVal length As Long, ByVal dataType As DataType)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void write(DataOutputStream out) throws IOException;
		Sub write(ByVal [out] As DataOutputStream)

		''' <summary>
		''' Returns the backing array
		''' of this buffer (if there is one) </summary>
		''' <returns> the backing array of this buffer </returns>
		Function array() As Object

		''' <summary>
		''' Returns a view of this as an
		''' nio byte buffer </summary>
		''' <returns> a view of this as an nio int buffer </returns>
		Function asNioInt() As java.nio.IntBuffer

		Function asNioLong() As java.nio.LongBuffer

		''' <summary>
		''' Returns a view of this as an
		''' nio byte buffer </summary>
		''' <returns> a view of this as an nio double buffer </returns>
		Function asNioDouble() As java.nio.DoubleBuffer

		''' <summary>
		''' Returns a view of this as an
		''' nio byte buffer </summary>
		''' <returns> a view of this as an nio float buffer </returns>
		Function asNioFloat() As java.nio.FloatBuffer

		''' <summary>
		''' Returns a view of this as an
		''' nio byte buffer </summary>
		''' <returns> a view of this as an nio byte buffer </returns>
		Function asNio() As ByteBuffer

		''' <summary>
		''' Whether the buffer is dirty:
		''' aka has been updated </summary>
		''' <returns> true if the buffer has been
		''' updated, false otherwise </returns>
		Function dirty() As Boolean


		''' <summary>
		''' Underlying buffer:
		''' This is meant for a data buffer
		''' to be a view of another data buffer
		''' @return
		''' </summary>
		Function underlyingDataBuffer() As DataBuffer


		''' <summary>
		'''  Original DataBuffer.
		'''  In case if we have a view derived from another view, derived from some other view, original DataBuffer will point to the originating DataBuffer, where all views come from.
		''' </summary>
		Function originalDataBuffer() As DataBuffer

		''' <summary>
		''' Copies from
		''' the given buffer
		''' at the specified stride
		''' for up to n elements </summary>
		''' <param name="buf"> the data buffer to copy from </param>
		''' <param name="n"> the number of elements to copy </param>
		''' <param name="stride"> the stride to copy at </param>
		''' <param name="yStride"> </param>
		''' <param name="offset"> </param>
		''' <param name="yOffset"> </param>
		Sub copyAtStride(ByVal buf As DataBuffer, ByVal n As Long, ByVal stride As Long, ByVal yStride As Long, ByVal offset As Long, ByVal yOffset As Long)

		''' <summary>
		''' Allocation mode for buffers </summary>
		''' <returns> the allocation mode for the buffer </returns>
		Function allocationMode() As AllocationMode

		''' <summary>
		''' Mark this buffer as persistent
		''' </summary>
		Sub persist()

		''' <summary>
		''' Whether the buffer should be persistent.
		''' This is mainly for the
		''' aggressive garbage collection strategy. </summary>
		''' <returns> whether the buffer should be persistent or not (default false) </returns>
		ReadOnly Property Persist As Boolean

		''' <summary>
		''' Un persist the buffer
		''' </summary>
		Sub unPersist()

		''' <summary>
		''' The number of bytes for each individual element
		''' </summary>
		''' <returns> the number of bytes for each individual element </returns>
		ReadOnly Property ElementSize As Integer

		''' <summary>
		''' Remove the referenced id if it exists
		''' </summary>
		''' <param name="id"> the id to remove </param>
		Sub removeReferencing(ByVal id As String)

		''' <summary>
		''' The referencers pointing to this buffer
		''' </summary>
		''' <returns> the references pointing to this buffer </returns>
		Function references() As ICollection(Of String)

		''' <summary>
		''' Add a referencing element to this buffer
		''' </summary>
		''' <param name="id"> the id to reference </param>
		Sub addReferencing(ByVal id As String)

		''' <summary>
		''' Assign the given elements to the given indices
		''' </summary>
		''' <param name="indices">    the indices to assign </param>
		''' <param name="data">       the data to assign </param>
		''' <param name="contiguous"> whether the indices are contiguous or not </param>
		''' <param name="inc">        the number to increment by when assigning </param>
		Sub assign(ByVal indices() As Long, ByVal data() As Single, ByVal contiguous As Boolean, ByVal inc As Long)

		''' <summary>
		''' Assign the given elements to the given indices
		''' </summary>
		''' <param name="indices">    the indices to assign </param>
		''' <param name="data">       the data to assign </param>
		''' <param name="contiguous"> whether the data is contiguous or not </param>
		''' <param name="inc">        the number to increment by when assigning </param>
		Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean, ByVal inc As Long)


		''' <summary>
		''' Assign the given elements to the given indices
		''' </summary>
		''' <param name="indices">    the indices to assign </param>
		''' <param name="data">       the data to assign </param>
		''' <param name="contiguous"> whether the indices are contiguous or not </param>
		Sub assign(ByVal indices() As Long, ByVal data() As Single, ByVal contiguous As Boolean)

		''' <summary>
		''' Assign the given elements to the given indices
		''' </summary>
		''' <param name="indices">    the indices to assign </param>
		''' <param name="data">       the data to assign </param>
		''' <param name="contiguous"> whether the data is contiguous or not </param>
		Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean)

		''' <summary>
		''' Get the doubles at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getDoublesAt(ByVal offset As Long, ByVal length As Integer) As Double()


		''' <summary>
		''' Get the doubles at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getFloatsAt(ByVal offset As Long, ByVal length As Integer) As Single()

		''' <summary>
		''' Get the ints at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getIntsAt(ByVal offset As Long, ByVal length As Integer) As Integer()

		''' <summary>
		''' Get the longs at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getLongsAt(ByVal offset As Long, ByVal length As Integer) As Long()

		''' <summary>
		''' Get the doubles at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="inc">    the increment to use </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getDoublesAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Double()


		''' <summary>
		''' Get the doubles at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="inc">    the increment to use </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getFloatsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Single()

		''' <summary>
		''' Get the ints at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="inc">    the increment to use </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getIntsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Integer()


		''' <summary>
		''' Get the long at a particular offset
		''' </summary>
		''' <param name="offset"> the offset to start </param>
		''' <param name="inc">    the increment to use </param>
		''' <param name="length"> the length of the array </param>
		''' <returns> the doubles at the specified offset and length </returns>
		Function getLongsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Long()


		''' <summary>
		''' Assign the given value to the buffer
		''' </summary>
		''' <param name="value"> the value to assign </param>
		Sub assign(ByVal value As Number)

		''' <summary>
		''' Assign the given value to the buffer
		''' starting at offset
		''' </summary>
		''' <param name="value">  assign the value to set </param>
		''' <param name="offset"> the offset to start at </param>
		Sub assign(ByVal value As Number, ByVal offset As Long)

		''' <summary>
		''' Set the data for this buffer
		''' </summary>
		''' <param name="data"> the data for this buffer </param>
		WriteOnly Property Data As Integer()

		''' <summary>
		''' Set the data for this buffer
		''' </summary>
		''' <param name="data"> the data for this buffer </param>
		WriteOnly Property Data As Long()


		''' <summary>
		''' Set the data for this buffer
		''' </summary>
		''' <param name="data"> the data for this buffer </param>
		WriteOnly Property Data As Single()

		''' <summary>
		''' Set the data for this buffer
		''' </summary>
		''' <param name="data"> the data for this buffer </param>
		WriteOnly Property Data As Double()
		WriteOnly Property Data As Short()
		WriteOnly Property Data As SByte()
		WriteOnly Property Data As Boolean()

		''' <summary>
		''' Raw byte array storage
		''' </summary>
		''' <returns> the data represented as a raw byte array </returns>
		Function asBytes() As SByte()

		''' <summary>
		''' The data opType of the buffer
		''' </summary>
		''' <returns> the data opType of the buffer </returns>
		Function dataType() As DataType

		''' <summary>
		''' Return the buffer as a float array
		''' Relative to the datatype, this will either be a copy
		''' or a reference. The reference is preferred for
		''' faster access of data and no copying
		''' </summary>
		''' <returns> the buffer as a float </returns>
		Function asFloat() As Single()

		''' <summary>
		''' Return the buffer as a double array
		''' Relative to the datatype, this will either be a copy
		''' or a reference. The reference is preferred for
		''' faster access of data and no copying
		''' </summary>
		''' <returns> the buffer as a float </returns>
		Function asDouble() As Double()

		''' <summary>
		''' Return the buffer as an int  array
		''' Relative to the datatype, this will either be a copy
		''' or a reference. The reference is preferred for
		''' faster access of data and no copying
		''' </summary>
		''' <returns> the buffer as a int </returns>
		Function asInt() As Integer()

		''' <summary>
		''' Return the buffer as an long  array
		''' Relative to the datatype, this will either be a copy
		''' or a reference. The reference is preferred for
		''' faster access of data and no copying
		''' </summary>
		''' <returns> the buffer as a long </returns>
		Function asLong() As Long()


		''' <summary>
		''' Get element i in the buffer as a double
		''' </summary>
		''' <param name="i"> the element to getFloat </param>
		''' <returns> the element at this index </returns>
		Function getDouble(ByVal i As Long) As Double

		''' <summary>
		''' Get element i in the buffer as long value </summary>
		''' <param name="i">
		''' @return </param>
		Function getLong(ByVal i As Long) As Long

		''' <summary>
		''' Get element i in the buffer as a double
		''' </summary>
		''' <param name="i"> the element to getFloat </param>
		''' <returns> the element at this index </returns>
		Function getFloat(ByVal i As Long) As Single

		''' <summary>
		''' Get element i in the buffer as a double
		''' </summary>
		''' <param name="i"> the element to getFloat </param>
		''' <returns> the element at this index </returns>
		Function getNumber(ByVal i As Long) As Number


		''' <summary>
		''' Assign an element in the buffer to the specified index
		''' </summary>
		''' <param name="i">       the index </param>
		''' <param name="element"> the element to assign </param>
		Sub put(ByVal i As Long, ByVal element As Single)

		''' <summary>
		''' Assign an element in the buffer to the specified index
		''' </summary>
		''' <param name="i">       the index </param>
		''' <param name="element"> the element to assign </param>
		Sub put(ByVal i As Long, ByVal element As Double)

		''' <summary>
		''' Assign an element in the buffer to the specified index
		''' </summary>
		''' <param name="i">       the index </param>
		''' <param name="element"> the element to assign </param>
		Sub put(ByVal i As Long, ByVal element As Integer)

		Sub put(ByVal i As Long, ByVal element As Long)

		Sub put(ByVal i As Long, ByVal element As Boolean)


		''' <summary>
		''' Returns the length of the buffer
		''' </summary>
		''' <returns> the length of the buffer </returns>
		Function length() As Long

		''' <summary>
		''' Returns the length of the buffer
		''' </summary>
		''' <returns> the length of the buffer </returns>
		Function underlyingLength() As Long

		''' <summary>
		''' Returns the offset of the buffer
		''' </summary>
		''' <returns> the offset of the buffer </returns>
		Function offset() As Long

		''' <summary>
		''' Returns the offset of the buffer relative to originalDataBuffer
		''' 
		''' @return
		''' </summary>
		Function originalOffset() As Long

		''' <summary>
		''' Get the int at the specified index
		''' </summary>
		''' <param name="ix"> the int at the specified index </param>
		''' <returns> the int at the specified index </returns>
		Function getInt(ByVal ix As Long) As Integer

		''' <summary>
		''' Return a copy of this buffer
		''' </summary>
		''' <returns> a copy of this buffer </returns>
		Function dup() As DataBuffer

		''' <summary>
		''' Flush the data buffer
		''' </summary>
		Sub flush()


		''' <summary>
		''' Assign the contents of this buffer
		''' to this buffer
		''' </summary>
		''' <param name="data"> the data to assign </param>
		Sub assign(ByVal data As DataBuffer)


		''' <summary>
		''' Assign the given buffers to this buffer
		''' based on the given offsets and strides.
		''' Note that the offsets and strides must be of equal
		''' length to the number of buffers </summary>
		'''  <param name="offsets"> the offsets to use </param>
		''' <param name="strides"> the strides to use </param>
		''' <param name="n">       the number of elements to operate on </param>
		''' <param name="buffers"> the buffers to assign data from </param>
		Sub assign(ByVal offsets() As Long, ByVal strides() As Long, ByVal n As Long, ParamArray ByVal buffers() As DataBuffer)

		''' <summary>
		''' Assign the given data buffers to this buffer
		''' </summary>
		''' <param name="buffers"> the buffers to assign </param>
		Sub assign(ParamArray ByVal buffers() As DataBuffer)

		''' <summary>
		''' Assign the given buffers to this buffer
		''' based on the given offsets and strides.
		''' Note that the offsets and strides must be of equal
		''' length to the number of buffers
		''' </summary>
		''' <param name="offsets"> the offsets to use </param>
		''' <param name="strides"> the strides to use </param>
		''' <param name="buffers"> the buffers to assign data from </param>
		Sub assign(ByVal offsets() As Long, ByVal strides() As Long, ParamArray ByVal buffers() As DataBuffer)


		''' <summary>
		''' release all resources for this buffer
		''' </summary>
		Sub destroy()

		''' <summary>
		''' Write this buffer to the output stream </summary>
		''' <param name="dos"> the output stream to write </param>
		Sub write(ByVal dos As Stream)

		''' <summary>
		''' Write this buffer to the input stream. </summary>
		''' <param name="is"> the inpus tream to write to </param>
		Sub read(ByVal [is] As Stream, ByVal allocationMode As AllocationMode, ByVal length As Long, ByVal dataType As DataType)

		''' <summary>
		''' This method returns whether this DataBuffer is constant, or not.
		''' Constant buffer means that it modified only during creation time, and then it stays the same for all lifecycle. I.e. used in shape info databuffers.
		''' 
		''' @return
		''' </summary>
		Property Constant As Boolean


		''' <summary>
		''' This method returns True, if this DataBuffer is attached to some workspace. False otherwise
		''' 
		''' @return
		''' </summary>
		ReadOnly Property Attached As Boolean

		''' <summary>
		''' This method checks, if given attached INDArray is still in scope of its parent Workspace
		''' 
		''' PLEASE NOTE: if this INDArray isn't attached to any Workspace, this method will return true
		''' @return
		''' </summary>
		ReadOnly Property InScope As Boolean

		''' <summary>
		''' This method returns Workspace this DataBuffer is attached to
		''' @return
		''' </summary>
		ReadOnly Property ParentWorkspace As MemoryWorkspace

		''' <summary>
		''' Reallocate the native memory of the buffer </summary>
		''' <param name="length"> the new length of the buffer </param>
		''' <returns> this databuffer
		'''  </returns>
		Function reallocate(ByVal length As Long) As DataBuffer

		''' <returns> the capacity of the databuffer
		'''  </returns>
		Function capacity() As Long

		''' <summary>
		''' This method checks, if this DataBuffer instalce can use close() method </summary>
		''' <returns> true if DataBuffer can be released, false otherwise </returns>
		Function closeable() As Boolean

		''' <summary>
		''' This method releases exclusive off-heap resources uses by this DataBuffer instance.
		''' If DataBuffer relies on shared resources, exception will be thrown instead
		''' 
		''' PLEASE NOTE: This method is NOT safe by any means
		''' </summary>
		Sub close()

		''' <summary>
		''' This method checks if array or its buffer was closed before </summary>
		''' <returns> true if was closed, false otherwise </returns>
		Function wasClosed() As Boolean
	End Interface

End Namespace