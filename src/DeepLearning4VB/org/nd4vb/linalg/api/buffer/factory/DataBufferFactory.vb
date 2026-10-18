Imports System
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace

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

Namespace org.nd4vb.linalg.api.buffer.factory


    ''' <summary>
    ''' Creates data buffers for the backend (Java <c>DataBufferFactory</c>).
    ''' <para>
    ''' Differences from Java: the overloads that wrapped JavaCPP pointers (<c>create(Pointer, DataType, long,
    ''' Indexer)</c>, <c>create(Pointer, Pointer, DataType, long, Indexer)</c>, <c>create(DoublePointer, long)</c>,
    ''' <c>create(IntPointer, long)</c>, <c>create(FloatPointer, long)</c>) are deleted as native-only;
    ''' <c>create(ByteBuffer, ...)</c> takes the raw bytes as <c>Byte()</c>; Java <c>byte[]</c> parameters are
    ''' <c>Byte()</c>; the setter <c>setAllocationMode</c> is a method (a property would clash with
    ''' <see cref="allocationMode"/>).
    ''' </para>
    ''' </summary>
    Public Interface DataBufferFactory

        ''' <summary>
        ''' Setter for the allocation mode </summary>
        ''' <param name="allocationMode"> </param>
        Sub setAllocationMode(ByVal allocationMode As DataBuffer.AllocationMode)

        ''' <summary>
        ''' Allocation mode for the data buffer
        ''' @return
        ''' </summary>
        Function allocationMode() As DataBuffer.AllocationMode

        ''' <summary>
        ''' Create a databuffer wrapping another one
        ''' this allows you to create a view of a buffer
        ''' with a different offset and length
        ''' backed by the same storage </summary>
        ''' <param name="underlyingBuffer"> the underlying buffer to get the storage from </param>
        ''' <param name="offset"> the offset to view the data as </param>
        ''' <param name="length"> the length of the buffer </param>
        ''' <returns> the databuffer as a view </returns>
        Function create(ByVal underlyingBuffer As DataBuffer, ByVal offset As Long, ByVal length As Long) As DataBuffer

        ''' <summary>
        ''' Creates a DataBuffer from java.nio.ByteBuffer </summary>
        ''' <param name="underlyingBuffer"> </param>
        ''' <param name="offset"> </param>
        ''' <param name="length">
        ''' @return </param>
        Function create(ByVal underlyingBuffer As Byte(), ByVal type As DataType, ByVal length As Long, ByVal offset As Long) As DataBuffer

        ''' <summary>
        ''' Create a double data buffer
        ''' </summary>
        ''' <returns> the new data buffer </returns>
        Function createDouble(ByVal offset As Long, ByVal length As Integer) As DataBuffer

        ''' <summary>
        ''' This method will create new DataBuffer of the same dataType & same length </summary>
        ''' <param name="buffer">
        ''' @return </param>
        Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean) As DataBuffer

        ''' <summary>
        ''' This method will create new DataBuffer of the same dataType & same length </summary>
        ''' <param name="buffer">
        ''' @return </param>
        Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Create a float data buffer
        ''' </summary>
        ''' <param name="length"> the length of the buffer </param>
        ''' <returns> the new data buffer </returns>
        Function createFloat(ByVal offset As Long, ByVal length As Integer) As DataBuffer

        ''' <summary>
        ''' Create an int data buffer
        ''' </summary>
        ''' <param name="length"> the length of the data buffer </param>
        ''' <returns> the create data buffer </returns>
        Function createInt(ByVal offset As Long, ByVal length As Integer) As DataBuffer


        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal offset As Long, ByVal data() As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal offset As Long, ByVal data() As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal offset As Long, ByVal data() As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal offset As Long, ByVal data() As Double) As DataBuffer

        Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer


        ''' <summary>
        ''' Create a double buffer </summary>
        ''' <param name="data"> </param>
        ''' <param name="length">
        ''' @return </param>
        Function createDouble(ByVal offset As Long, ByVal data() As Byte, ByVal length As Integer) As DataBuffer

        ''' <summary>
        ''' Create a double buffer </summary>
        ''' <param name="data"> </param>
        ''' <param name="length">
        ''' @return </param>
        Function createFloat(ByVal offset As Long, ByVal data() As Byte, ByVal length As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal offset As Long, ByVal data() As Double) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal offset As Long, ByVal data() As Double) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal offset As Long, ByVal data() As Single) As DataBuffer

        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal offset As Long, ByVal data() As Single) As DataBuffer

        Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal offset As Long, ByVal data() As Single) As DataBuffer


        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Create a double data buffer
        ''' </summary>
        ''' <returns> the new data buffer </returns>
        Function createDouble(ByVal length As Long) As DataBuffer

        ''' <summary>
        ''' Create a double data buffer, with optional initialization
        ''' </summary>
        ''' <param name="initialize"> If true: initialize the buffer. If false: don't initialize.
        ''' </param>
        ''' <returns> the new data buffer </returns>
        Function createDouble(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer

        Function createDouble(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Create a float data buffer
        ''' </summary>
        ''' <param name="length"> the length of the buffer </param>
        ''' <returns> the new data buffer </returns>
        Function createFloat(ByVal length As Long) As DataBuffer

        ''' <summary>
        ''' Create a float data buffer, with optional initialization
        ''' </summary>
        ''' <param name="length"> the length of the buffer </param>
        ''' <param name="initialize"> If true: initialize the buffer. If false: don't initialize. </param>
        ''' <returns> the new data buffer </returns>
        Function createFloat(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer

        Function createFloat(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean) As DataBuffer

        Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer


        ''' <summary>
        ''' Create an int data buffer
        ''' </summary>
        ''' <param name="length"> the length of the data buffer </param>
        ''' <returns> the create data buffer </returns>
        Function createInt(ByVal length As Long) As DataBuffer

        Function createBFloat16(ByVal length As Long) As DataBuffer
        Function createByte(ByVal length As Long) As DataBuffer
        Function createShort(ByVal length As Long) As DataBuffer
        Function createBool(ByVal length As Long) As DataBuffer
        Function createUShort(ByVal length As Long) As DataBuffer
        Function createUInt(ByVal length As Long) As DataBuffer
        Function createUByte(ByVal length As Long) As DataBuffer
        Function createULong(ByVal length As Long) As DataBuffer

        Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
        Function createByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
        Function createShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
        Function createBool(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
        Function createUShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
        Function createUInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
        Function createUByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer
        Function createULong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer

        Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
        Function createByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
        Function createShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
        Function createBool(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
        Function createUShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
        Function createUInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
        Function createUByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
        Function createULong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Create an int data buffer, with optional initialization
        ''' </summary>
        ''' <param name="length"> the length of the data buffer </param>
        ''' <param name="initialize"> If true: initialize the buffer. If false: don't initialize. </param>
        ''' <returns> the create data buffer </returns>
        Function createInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer

        Function createInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer


        Function createLong(ByVal data() As Long) As DataBuffer

        Function createLong(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer

        Function createLong(ByVal length As Long) As DataBuffer

        ''' <summary>
        ''' Create an int data buffer, with optional initialization
        ''' </summary>
        ''' <param name="length"> the length of the data buffer </param>
        ''' <param name="initialize"> If true: initialize the buffer. If false: don't initialize. </param>
        ''' <returns> the create data buffer </returns>
        Function createLong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer

        Function createLong(ByVal data() As Long, ByVal workspace As MemoryWorkspace) As DataBuffer

        Function createLong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal data() As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal data() As Integer) As DataBuffer

        Function createInt(ByVal data() As Integer, ByVal workspace As MemoryWorkspace) As DataBuffer

        Function createInt(ByVal data() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Double) As DataBuffer


        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal data() As Double) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal data() As Double) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Single) As DataBuffer

        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal data() As Single) As DataBuffer

        Function createFloat(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal data() As Single) As DataBuffer


        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a long data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createLong(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer


        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Double, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer



        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a double data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createDouble(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a float data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createFloat(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer

        Function createFloat(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates an int data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createInt(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer







        ''' <summary>
        ''' Creates half-precision data buffer
        ''' </summary>
        ''' <param name="length"> length of new data buffer
        ''' @return </param>
        Function createHalf(ByVal length As Long) As DataBuffer

        ''' <summary>
        ''' Creates half-precision data buffer
        ''' </summary>
        ''' <param name="length"> length of new data buffer </param>
        ''' <param name="initialize"> true if memset should be used on allocated memory, false otherwise
        ''' @return </param>
        Function createHalf(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer

        Function createHalf(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer

        Function createHalf(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer

        Function createHalf(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer



        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer


        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal data() As Double) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal data() As Single) As DataBuffer

        Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal data() As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal data() As Byte, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal data() As Single) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal data() As Double) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <param name="data"> the data to create the buffer from </param>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal data() As Integer) As DataBuffer

        ''' <summary>
        ''' Creates a half-precision data buffer
        ''' </summary>
        ''' <returns> the new buffer </returns>
        Function createHalf(ByVal offset As Long, ByVal length As Integer) As DataBuffer


        Function intBufferClass() As Type

        Function longBufferClass() As Type

        Function halfBufferClass() As Type

        Function floatBufferClass() As Type

        Function doubleBufferClass() As Type

        Function createUtf8Buffer(ByVal data() As Byte, ByVal product As Long) As DataBuffer
    End Interface

End Namespace
