Imports System
Imports DT = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DeepLearning4VB.org.slf4j
Imports ND4VBSystemProperties = DeepLearning4VB.org.nd4vb.common.config.ND4VBSystemProperties
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports NumericCasts = DeepLearning4VB.org.nd4vb.linalg.managed.storage.NumericCasts
Imports DataInputStream = DeepLearning4VB.JavaCompat.DataInputStream
Imports DataOutputStream = DeepLearning4VB.JavaCompat.DataOutputStream
Imports SysMath = System.Math

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

Namespace org.nd4vb.linalg.api.buffer

    ''' <summary>
    ''' The logic every data buffer shares (Java <c>BaseDataBuffer</c>): length, offset and view bookkeeping, the
    ''' workspace and constant flags, bulk assignment, the <c>asX()</c> copies, the Java stream layout of
    ''' <see cref="write(DataOutputStream)"/> and <see cref="read(DataInputStream, DataBuffer.AllocationMode, Long, DataType)"/>,
    ''' <see cref="asBytes"/>, <see cref="ToString"/> and the close/release rules. Subclasses provide the elements:
    ''' the managed backend's <c>ManagedDataBuffer</c> (typed .NET arrays) and <c>CompressedDataBuffer</c>.
    ''' </summary>
    ''' <remarks>
    ''' <para>Decision for phase 2: Java's <c>BaseDataBuffer</c> stays an abstract class in <c>api.buffer</c>, without its
    ''' pointer plumbing. Java implemented the element accessors here through JavaCPP indexers; in ND4VB
    ''' <see cref="getDouble"/>, <see cref="getLong"/>, <see cref="getFloat"/>, <see cref="getInt"/>, the <c>put</c>
    ''' overloads, <see cref="sameUnderlyingData"/>, <see cref="capacity"/>, <see cref="reallocate"/> and
    ''' <see cref="pointerIndexerByCurrentType"/> are abstract.</para>
    ''' <para>The stream layout of <see cref="write(DataOutputStream)"/> is Java's: <c>writeUTF(allocationMode)</c>,
    ''' <c>writeLong(length)</c>, <c>writeUTF(dataType.name())</c>, then the elements big-endian (doubles, longs, ints,
    ''' shorts or bytes by type; BOOL as one byte 0/1; HALF and BFLOAT16 as their 16-bit patterns). Buffers written by Java
    ''' ND4J read back, including the legacy <c>DIRECT</c>/<c>HEAP</c>/<c>JAVACPP</c> headers whose length is an int.</para>
    ''' <para>Differences from Java:
    ''' <list type="bullet">
    ''' <item>Native-only members not ported: <c>pointer()</c>, <c>addressPointer()</c>, <c>indexer()</c>, <c>address()</c>,
    ''' <c>platformAddress()</c>, <c>setIndexer</c>, <c>fillPointerWithZero()</c>, <c>setNioBuffer()</c>, <c>wrappedBuffer()</c>,
    ''' <c>asNio()</c>, <c>asNioInt()</c>, <c>asNioLong()</c>, <c>asNioFloat()</c>, <c>asNioDouble()</c>, the
    ''' <c>(Pointer, Indexer, long)</c> constructor, and Java serialization (<c>readObject</c>/<c>writeObject</c>/
    ''' <c>doReadObject</c>; use <see cref="write(Stream)"/> and <see cref="read(Stream, DataBuffer.AllocationMode, Long, DataType)"/>).</item>
    ''' <item>A view's type is the type of the buffer it views (Java subclasses fixed the type per class).</item>
    ''' <item>UTF8: Java wrote the header and no elements, and could not read UTF8 back; ND4VB writes the header (with
    ''' Java's byte length) followed by the bytes of Java's UTF8 layout, and reads that back (see the subclass).</item>
    ''' <item><see cref="Dispose"/> calls <see cref="close"/>, but does nothing when this buffer was already released.</item>
    ''' <item><see cref="setData(Byte())"/> reads each byte as Java's signed <c>byte</c>.</item>
    ''' </list></para>
    ''' </remarks>
    Public MustInherit Class BaseDataBuffer
        Implements DataBuffer

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BaseDataBuffer))

        ''' <summary>Deprecated in Java: use <see cref="ND4VBSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS"/>.</summary>
        Public Shared ReadOnly TO_STRING_MAX_ELEMENTS As String = ND4VBSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS

        Private Shared ReadOnly TO_STRING_MAX As Integer = readToStringMax()

        Private Shared Function readToStringMax() As Integer
            Dim s As String = JavaSystem.getProperty(ND4VBSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS)
            If s IsNot Nothing Then
                Dim v As Integer
                If Integer.TryParse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, v) Then
                    Return v
                End If
                log.warn("Invalid value for key {}: ""{}""", ND4VBSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS, s)
            End If
            Return 1000
        End Function

        ''' <summary>The element type.</summary>
        Protected Friend type As DataType
        ''' <summary>Number of elements.</summary>
        Protected Friend lengthField As Long
        ''' <summary>Length of the buffer this one views (its own length for a root buffer).</summary>
        Protected Friend underlyingLengthField As Long
        ''' <summary>Offset of a view, in elements (Java: absolute within the original buffer).</summary>
        Protected Friend offsetField As Long
        ''' <summary>Bytes per element.</summary>
        Protected Friend elementSizeField As Integer
        ''' <summary>The buffer this one is a view of (Me for a root buffer created from data or a length).</summary>
        Protected Friend wrappedDataBuffer As DataBuffer
        ''' <summary>Generation of the parent workspace when this buffer was allocated in it.</summary>
        Protected Friend workspaceGenerationId As Long = 0L
        ''' <summary>The allocation mode (always MIXED_DATA_TYPES for new buffers).</summary>
        Protected Friend allocationModeField As DataBuffer.AllocationMode
        ''' <summary>True when the buffer was allocated in a workspace.</summary>
        Protected Friend attachedField As Boolean = False
        ''' <summary>The workspace the buffer was allocated in.</summary>
        Protected Friend parentWorkspaceField As MemoryWorkspace
        ''' <summary>The root buffer of a chain of views.</summary>
        Protected Friend originalBuffer As DataBuffer
        ''' <summary>Offset relative to <see cref="originalBuffer"/>.</summary>
        Protected Friend originalOffsetField As Long = 0
        ''' <summary>Constant flag (shape information and constant-cache buffers).</summary>
        Protected Friend constantField As Boolean = False
        ''' <summary>True after <see cref="close"/>.</summary>
        Protected Friend released As Boolean = False
        ''' <summary>Set when a view of this buffer was created.</summary>
        Protected Friend referenced As New AtomicBoolean(False)

        ''' <summary>Creates an empty buffer; the subclass sets type, length and storage.</summary>
        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Meant for creating another view of a buffer: <paramref name="length"/> elements starting at element
        ''' <paramref name="offset"/> (Java: absolute within the original buffer). A length of 0 becomes 1, as in Java.
        ''' </summary>
        ''' <param name="underlyingBuffer"> the underlying buffer to create a view from </param>
        ''' <param name="length"> the length of the view </param>
        ''' <param name="offset"> the offset for the view </param>
        Protected Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
            If underlyingBuffer Is Nothing Then
                Throw New ArgumentNullException(NameOf(underlyingBuffer))
            End If
            If length < 0 Then
                Throw New ArgumentException("Length must be >= 0")
            End If

            If length = 0 Then
                length = 1
            End If

            Me.type = underlyingBuffer.dataType()
            initTypeAndSize()
            Me.lengthField = length
            Me.offsetField = offset
            Me.allocationModeField = underlyingBuffer.getAllocationMode()
            Me.elementSizeField = underlyingBuffer.ElementSize
            Me.underlyingLengthField = underlyingBuffer.underlyingLength()
            Me.wrappedDataBuffer = underlyingBuffer

            ' we're not referencing constant buffers
            If Not underlyingBuffer.Constant AndAlso TypeOf underlyingBuffer Is BaseDataBuffer Then
                DirectCast(underlyingBuffer, BaseDataBuffer).pickReferent(Me)
            End If

            ' Adding link to original databuffer
            If underlyingBuffer.originalDataBuffer() Is Nothing Then
                Me.originalBuffer = underlyingBuffer
                Me.originalOffsetField = offset
            Else
                Me.originalBuffer = underlyingBuffer.originalDataBuffer()
                ' Java: the offset of a view is already absolute within the original buffer
                Me.originalOffsetField = offset
            End If
        End Sub

        ''' <summary>
        ''' Initialize the type and element size of this buffer (Java subclasses set their fixed type here; the managed
        ''' buffer takes its type from its constructor arguments).
        ''' </summary>
        Protected Friend MustOverride Sub initTypeAndSize()

        ''' <summary>The number of bytes for each individual element.</summary>
        Public Overridable ReadOnly Property ElementSize As Integer Implements DataBuffer.ElementSize
            Get
                Return elementSizeField
            End Get
        End Property

        ''' <summary>
        ''' The workspace generation this buffer was allocated in: its own when attached, otherwise that of the attached
        ''' buffer it views (or of the original buffer).
        ''' </summary>
        Public Overridable ReadOnly Property GenerationId As Long Implements DataBuffer.GenerationId
            Get
                If parentWorkspaceField IsNot Nothing Then
                    Return workspaceGenerationId
                ElseIf wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer IsNot Me AndAlso wrappedDataBuffer.Attached Then
                    Return wrappedDataBuffer.GenerationId
                ElseIf originalBuffer IsNot Nothing AndAlso originalBuffer.Attached Then
                    Return originalBuffer.GenerationId
                End If
                Return workspaceGenerationId
            End Get
        End Property

        ''' <summary>Called on the underlying buffer when a view of it is created.</summary>
        Protected Friend Overridable Sub pickReferent(ByVal referent As BaseDataBuffer)
            referenced.compareAndSet(False, True)
        End Sub

        ''' <summary>
        ''' Original DataBuffer.
        ''' In case if we have a view derived from another view, derived from some other view, original DataBuffer will point to the originating DataBuffer, where all views come from.
        ''' </summary>
        Public Overridable Function originalDataBuffer() As DataBuffer Implements DataBuffer.originalDataBuffer
            Return originalBuffer
        End Function

        ''' <summary>The buffer this one views (Me for a root buffer).</summary>
        Public Overridable Function underlyingDataBuffer() As DataBuffer Implements DataBuffer.underlyingDataBuffer
            Return wrappedDataBuffer
        End Function

        ''' <summary>Returns the offset of the buffer.</summary>
        Public Overridable Function offset() As Long Implements DataBuffer.offset
            Return offsetField
        End Function

        ''' <summary>Allocation mode for buffers (Java <c>allocationMode()</c>).</summary>
        Public Overridable Function getAllocationMode() As DataBuffer.AllocationMode Implements DataBuffer.getAllocationMode
            Return allocationModeField
        End Function

        ''' <summary>Deprecated in Java: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overridable Sub persist() Implements DataBuffer.persist
            Throw New NotSupportedException()
        End Sub

        ''' <summary>Deprecated in Java: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overridable Function isPersist() As Boolean Implements DataBuffer.isPersist
            Throw New NotSupportedException()
        End Function

        ''' <summary>Deprecated in Java: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overridable Sub unPersist() Implements DataBuffer.unPersist
            Throw New NotSupportedException()
        End Sub

        ''' <summary>
        ''' Copies <paramref name="n"/> elements of <paramref name="buf"/> (from <paramref name="yOffset"/>, every
        ''' <paramref name="yStride"/>-th) to this buffer (from <paramref name="offset"/>, every <paramref name="stride"/>-th),
        ''' through float for FLOAT buffers and through double otherwise.
        ''' </summary>
        Public Overridable Sub copyAtStride(ByVal buf As DataBuffer, ByVal n As Long, ByVal stride As Long, ByVal yStride As Long, ByVal offset As Long, ByVal yOffset As Long) Implements DataBuffer.copyAtStride
            If dataType() = DT.FLOAT Then
                For i As Long = 0 To n - 1
                    put(offset + i * stride, buf.getFloat(yOffset + i * yStride))
                Next i
            Else
                For i As Long = 0 To n - 1
                    put(offset + i * stride, buf.getDouble(yOffset + i * yStride))
                Next i
            End If
        End Sub

        ''' <summary>Deprecated in Java: does nothing.</summary>
        Public Overridable Sub removeReferencing(ByVal id As String) Implements DataBuffer.removeReferencing
        End Sub

        ''' <summary>Deprecated in Java: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overridable Function references() As ICollection(Of String) Implements DataBuffer.references
            Throw New NotSupportedException()
        End Function

        ''' <summary>Deprecated in Java: does nothing.</summary>
        Public Overridable Sub addReferencing(ByVal id As String) Implements DataBuffer.addReferencing
        End Sub

        ''' <summary>Puts <c>data(i)</c> at <c>indices(i)</c> (Java ignores <paramref name="contiguous"/> and <paramref name="inc"/> here).</summary>
        Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Single, ByVal contiguous As Boolean, ByVal inc As Long) Implements DataBuffer.assign
            If indices.Length <> data.Length Then
                Throw New ArgumentException("Indices and data length must be the same")
            End If
            If indices.Length > length() Then
                Throw New ArgumentException("More elements than space to assign. This buffer is of length " & length() & " where the indices are of length " & data.Length)
            End If
            For i As Integer = 0 To indices.Length - 1
                put(indices(i), data(i))
            Next i
        End Sub

        ''' <summary>Set the data for this buffer: <c>put(i, data(i))</c> for every element of <paramref name="data"/>.</summary>
        Public Overridable Sub setData(ByVal data As Integer()) Implements DataBuffer.setData
            For i As Integer = 0 To data.Length - 1
                put(i, data(i))
            Next i
        End Sub

        ''' <summary>Set the data for this buffer: <c>put(i, data(i))</c> for every element of <paramref name="data"/>.</summary>
        Public Overridable Sub setData(ByVal data As Single()) Implements DataBuffer.setData
            For i As Integer = 0 To data.Length - 1
                put(i, data(i))
            Next i
        End Sub

        ''' <summary>Set the data for this buffer: <c>put(i, data(i))</c> for every element of <paramref name="data"/>.</summary>
        Public Overridable Sub setData(ByVal data As Double()) Implements DataBuffer.setData
            For i As Integer = 0 To data.Length - 1
                put(i, data(i))
            Next i
        End Sub

        ''' <summary>Set the data for this buffer: <c>put(i, data(i))</c> for every element of <paramref name="data"/>.</summary>
        Public Overridable Sub setData(ByVal data As Long()) Implements DataBuffer.setData
            For i As Integer = 0 To data.Length - 1
                put(i, data(i))
            Next i
        End Sub

        ''' <summary>Set the data for this buffer; each byte is Java's signed <c>byte</c> (200 is stored as -56).</summary>
        Public Overridable Sub setData(ByVal data As Byte()) Implements DataBuffer.setData
            For i As Integer = 0 To data.Length - 1
                put(i, If(data(i) > 127, CInt(data(i)) - 256, CInt(data(i))))
            Next i
        End Sub

        ''' <summary>Set the data for this buffer: <c>put(i, data(i))</c> for every element of <paramref name="data"/>.</summary>
        Public Overridable Sub setData(ByVal data As Short()) Implements DataBuffer.setData
            For i As Integer = 0 To data.Length - 1
                put(i, CInt(data(i)))
            Next i
        End Sub

        ''' <summary>Set the data for this buffer: <c>put(i, data(i))</c> for every element of <paramref name="data"/>.</summary>
        Public Overridable Sub setData(ByVal data As Boolean()) Implements DataBuffer.setData
            For i As Integer = 0 To data.Length - 1
                put(i, data(i))
            Next i
        End Sub

        ''' <summary>Puts <c>data(i)</c> at <c>indices(i)</c> for every <paramref name="inc"/>-th i.</summary>
        Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean, ByVal inc As Long) Implements DataBuffer.assign
            If indices.Length <> data.Length Then
                Throw New ArgumentException("Indices and data length must be the same")
            End If
            If indices.Length > length() Then
                Throw New ArgumentException("More elements than space to assign. This buffer is of length " & length() & " where the indices are of length " & data.Length)
            End If
            For i As Integer = 0 To indices.Length - 1 Step CInt(inc)
                put(indices(i), data(i))
            Next i
        End Sub

        ''' <summary>Copies the elements of a buffer of the same length (through double, as in Java).</summary>
        Public Overridable Sub assign(ByVal data As DataBuffer) Implements DataBuffer.assign
            If data.length() <> length() Then
                Throw New ArgumentException("Unable to assign buffer of length " & data.length() & " to this buffer of length " & length())
            End If

            For i As Long = 0 To data.length() - 1
                put(i, data.getDouble(i))
            Next i
        End Sub

        ''' <summary>Same as <c>assign(indices, data, contiguous, 1)</c>.</summary>
        Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Single, ByVal contiguous As Boolean) Implements DataBuffer.assign
            assign(indices, data, contiguous, 1)
        End Sub

        ''' <summary>Same as <c>assign(indices, data, contiguous, 1)</c>.</summary>
        Public Overridable Sub assign(ByVal indices() As Long, ByVal data() As Double, ByVal contiguous As Boolean) Implements DataBuffer.assign
            assign(indices, data, contiguous, 1)
        End Sub

        ''' <summary>Length of the buffer this one views (its own length for a root buffer).</summary>
        Public Overridable Function underlyingLength() As Long Implements DataBuffer.underlyingLength
            Return underlyingLengthField
        End Function

        ''' <summary>Returns the length of the buffer.</summary>
        Public Overridable Function length() As Long Implements DataBuffer.length
            Return lengthField
        End Function

        ''' <summary>Assigns <paramref name="value"/> (a boxed number) to every element.</summary>
        Public Overridable Sub assign(ByVal value As Object) Implements DataBuffer.assign
            assign(value, 0)
        End Sub

        ''' <summary>
        ''' <paramref name="length"/> elements as doubles, every <paramref name="inc"/>-th from <paramref name="offset"/>.
        ''' As in Java, when <c>offset + length</c> exceeds the buffer length, <paramref name="offset"/> is subtracted from <paramref name="length"/>.
        ''' </summary>
        Public Overridable Function getDoublesAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Double() Implements DataBuffer.getDoublesAt
            If offset + length > Me.length() Then
                length -= CInt(offset)
            End If

            Dim ret(length - 1) As Double
            For i As Integer = 0 To length - 1
                ret(i) = getDouble(i * inc + offset)
            Next i
            Return ret
        End Function

        ''' <summary>Same as <c>getDoublesAt(offset, 1, length)</c>.</summary>
        Public Overridable Function getDoublesAt(ByVal offset As Long, ByVal length As Integer) As Double() Implements DataBuffer.getDoublesAt
            Return getDoublesAt(offset, 1, length)
        End Function

        ''' <summary>Same as <c>getFloatsAt(offset, 1, length)</c>.</summary>
        Public Overridable Function getFloatsAt(ByVal offset As Long, ByVal length As Integer) As Single() Implements DataBuffer.getFloatsAt
            Return getFloatsAt(offset, 1, length)
        End Function

        ''' <summary><paramref name="length"/> elements as floats (see <see cref="getDoublesAt(Long, Long, Integer)"/>).</summary>
        Public Overridable Function getFloatsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Single() Implements DataBuffer.getFloatsAt
            If offset + length > Me.length() Then
                length -= CInt(offset)
            End If
            Dim ret(length - 1) As Single
            For i As Integer = 0 To length - 1
                ret(i) = getFloat(i * inc + offset)
            Next i
            Return ret
        End Function

        ''' <summary>Same as <c>getLongsAt(offset, 1, length)</c>.</summary>
        Public Overridable Function getLongsAt(ByVal offset As Long, ByVal length As Integer) As Long() Implements DataBuffer.getLongsAt
            Return getLongsAt(offset, 1, length)
        End Function

        ''' <summary><paramref name="length"/> elements as longs (see <see cref="getDoublesAt(Long, Long, Integer)"/>).</summary>
        Public Overridable Function getLongsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Long() Implements DataBuffer.getLongsAt
            If offset + length > Me.length() Then
                length -= CInt(offset)
            End If
            Dim ret(length - 1) As Long
            For i As Integer = 0 To length - 1
                ret(i) = getLong(i * inc + offset)
            Next i
            Return ret
        End Function

        ''' <summary>Same as <c>getIntsAt(offset, 1, length)</c>.</summary>
        Public Overridable Function getIntsAt(ByVal offset As Long, ByVal length As Integer) As Integer() Implements DataBuffer.getIntsAt
            Return getIntsAt(offset, 1, length)
        End Function

        ''' <summary><paramref name="length"/> elements as ints (see <see cref="getDoublesAt(Long, Long, Integer)"/>).</summary>
        Public Overridable Function getIntsAt(ByVal offset As Long, ByVal inc As Long, ByVal length As Integer) As Integer() Implements DataBuffer.getIntsAt
            If offset + length > Me.length() Then
                length -= CInt(offset)
            End If
            Dim ret(length - 1) As Integer
            For i As Integer = 0 To length - 1
                ret(i) = getInt(i * inc + offset)
            Next i
            Return ret
        End Function

        ''' <summary>A new detached buffer of the same type and length with the same elements (copied through double, as in Java).</summary>
        Public Overridable Function dup() As DataBuffer Implements DataBuffer.dup
            Dim ret As DataBuffer = create(lengthField)
            For i As Long = 0 To ret.length() - 1
                ret.put(i, getDouble(i))
            Next i
            Return ret
        End Function

        ''' <summary>A new buffer of the same type with the given length.</summary>
        ''' <param name="length"> the length of the new buffer </param>
        Protected Friend MustOverride Function create(ByVal length As Long) As DataBuffer

        ''' <summary>A new buffer of the same type holding <paramref name="data"/>.</summary>
        Public MustOverride Function create(ByVal data() As Double) As DataBuffer

        ''' <summary>A new buffer of the same type holding <paramref name="data"/>.</summary>
        Public MustOverride Function create(ByVal data() As Single) As DataBuffer

        ''' <summary>A new buffer of the same type holding <paramref name="data"/>.</summary>
        Public MustOverride Function create(ByVal data() As Integer) As DataBuffer

        ''' <summary>Same as <c>assign(offsets, strides, length(), buffers)</c>.</summary>
        Public Overridable Sub assign(ByVal offsets() As Long, ByVal strides() As Long, ParamArray ByVal buffers() As DataBuffer) Implements DataBuffer.assign
            assign(offsets, strides, length(), buffers)
        End Sub

        ''' <summary>
        ''' The elements as Java's <c>DataOutputStream</c> writes them (big-endian): doubles, floats, longs, ints, shorts
        ''' or bytes by type; BOOL as 0/1; HALF, BFLOAT16, UINT16, UINT32 and UINT64 as their raw bits; UTF8 as Java's
        ''' UTF8 buffer layout (see the subclass).
        ''' </summary>
        Public Overridable Function asBytes() As Byte() Implements DataBuffer.asBytes
            Using bos As New MemoryStream()
                Dim dos As New DataOutputStream(bos, True)
                Dim n As Long = length()
                Select Case dataType()
                    Case DT.DOUBLE
                        For i As Long = 0 To n - 1
                            dos.writeDouble(getDouble(i))
                        Next i
                    Case DT.FLOAT
                        For i As Long = 0 To n - 1
                            dos.writeFloat(getFloat(i))
                        Next i
                    Case DT.HALF
                        For i As Long = 0 To n - 1
                            dos.writeShort(NumericCasts.SingleToHalf(getFloat(i)))
                        Next i
                    Case DT.BOOL
                        For i As Long = 0 To n - 1
                            dos.writeByte(If(getInt(i) = 0, 0, 1))
                        Next i
                    Case DT.BYTE, DT.UBYTE
                        For i As Long = 0 To n - 1
                            dos.writeByte(getInt(i))
                        Next i
                    Case DT.SHORT, DT.UINT16
                        For i As Long = 0 To n - 1
                            dos.writeShort(getInt(i))
                        Next i
                    Case DT.INT, DT.UINT32
                        For i As Long = 0 To n - 1
                            dos.writeInt(getInt(i))
                        Next i
                    Case DT.LONG, DT.UINT64
                        For i As Long = 0 To n - 1
                            dos.writeLong(getLong(i))
                        Next i
                    Case DT.BFLOAT16
                        For i As Long = 0 To n - 1
                            dos.writeShort(NumericCasts.SingleToBFloat16(getFloat(i)))
                        Next i
                    Case DT.UTF8
                        dos.write(utf8Layout())
                    Case Else
                        Throw New NotSupportedException("Unknown data type: [" & dataType().name() & "]")
                End Select
                dos.flush()
                Return bos.ToArray()
            End Using
        End Function

        ''' <summary>Returns the buffer as a float array (a copy).</summary>
        Public Overridable Function asFloat() As Single() Implements DataBuffer.asFloat
            checkArrayLength()
            Dim ret(CInt(lengthField) - 1) As Single
            For i As Integer = 0 To CInt(lengthField) - 1
                ret(i) = getFloatUnsynced(i)
            Next i
            Return ret
        End Function

        ''' <summary>Returns the buffer as a double array (a copy).</summary>
        Public Overridable Function asDouble() As Double() Implements DataBuffer.asDouble
            checkArrayLength()
            Dim ret(CInt(lengthField) - 1) As Double
            For i As Integer = 0 To CInt(lengthField) - 1
                ret(i) = getDoubleUnsynced(i)
            Next i
            Return ret
        End Function

        ''' <summary>Returns the buffer as an int array (a copy).</summary>
        Public Overridable Function asInt() As Integer() Implements DataBuffer.asInt
            checkArrayLength()
            Dim ret(CInt(lengthField) - 1) As Integer
            For i As Integer = 0 To CInt(lengthField) - 1
                ret(i) = getIntUnsynced(i)
            Next i
            Return ret
        End Function

        ''' <summary>Returns the buffer as a long array (a copy).</summary>
        Public Overridable Function asLong() As Long() Implements DataBuffer.asLong
            checkArrayLength()
            Dim ret(CInt(lengthField) - 1) As Long
            For i As Integer = 0 To CInt(lengthField) - 1
                ret(i) = getLongUnsynced(i)
            Next i
            Return ret
        End Function

        ''' <summary>Throws when the buffer is too long for a .NET array (Java: <c>length &gt;= Integer.MAX_VALUE</c>).</summary>
        Protected Sub checkArrayLength()
            If lengthField >= Integer.MaxValue Then
                Throw New ArgumentException("Unable to create array of length " & lengthField)
            End If
        End Sub

        ''' <summary>Get element i in the buffer as a double.</summary>
        Public MustOverride Function getDouble(ByVal i As Long) As Double Implements DataBuffer.getDouble

        ''' <summary>Get element i in the buffer as a long (Java's <c>(long)</c> cast).</summary>
        Public MustOverride Function getLong(ByVal i As Long) As Long Implements DataBuffer.getLong

        ''' <summary>Get element i in the buffer as a float.</summary>
        Public MustOverride Function getFloat(ByVal i As Long) As Single Implements DataBuffer.getFloat

        ''' <summary>Get element i in the buffer as an int (Java's <c>(int)</c> cast).</summary>
        Public MustOverride Function getInt(ByVal ix As Long) As Integer Implements DataBuffer.getInt

        ''' <summary>
        ''' Element i as a boxed number, as Java's <c>getNumber</c>: a <c>Double</c> for DOUBLE buffers, an <c>Integer</c> for
        ''' INT, a <c>Long</c> for LONG and a <c>Single</c> for every other type.
        ''' </summary>
        Public Overridable Function getNumber(ByVal i As Long) As Object Implements DataBuffer.getNumber
            If released Then
                Throw New InvalidOperationException("You can't use DataBuffer once it was released")
            End If

            Select Case dataType()
                Case DT.DOUBLE
                    Return getDouble(i)
                Case DT.INT
                    Return getInt(i)
                Case DT.LONG
                    Return getLong(i)
                Case Else
                    Return getFloat(i)
            End Select
        End Function

        ''' <summary>
        ''' Reallocates the storage for <paramref name="currentType"/> and the current length (Java: re-created the JavaCPP
        ''' pointer and indexer). Used by <see cref="read(DataInputStream, DataBuffer.AllocationMode, Long, DataType)"/>.
        ''' </summary>
        Public MustOverride Sub pointerIndexerByCurrentType(ByVal currentType As DataType)

        ''' <summary>
        ''' Puts a boxed number converted as Java does when reading a stream: through <c>intValue()</c> for the small
        ''' integer types and BOOL, <c>longValue()</c> for LONG/UINT32/UINT64, <c>floatValue()</c> for FLOAT/HALF/BFLOAT16
        ''' and <c>doubleValue()</c> for DOUBLE.
        ''' </summary>
        Public Overridable Sub putByDestinationType(ByVal i As Long, ByVal element As Object, ByVal globalType As DataType)
            If globalType = DT.INT OrElse type = DT.INT OrElse globalType = DT.UINT16 OrElse globalType = DT.UBYTE OrElse globalType = DT.SHORT OrElse globalType = DT.BYTE OrElse globalType = DT.BOOL Then
                put(i, JavaNumber.intValue(element))
            ElseIf globalType = DT.LONG OrElse type = DT.LONG OrElse globalType = DT.UINT32 OrElse globalType = DT.UINT64 Then
                put(i, JavaNumber.longValue(element))
            ElseIf globalType = DT.FLOAT OrElse globalType = DT.HALF OrElse globalType = DT.BFLOAT16 Then
                put(i, JavaNumber.floatValue(element))
            ElseIf globalType = DT.DOUBLE Then
                put(i, JavaNumber.doubleValue(element))
            Else
                Throw New InvalidOperationException("Unknown type: " & globalType.name())
            End If
        End Sub

        ''' <summary>Assign an element in the buffer to the specified index.</summary>
        Public MustOverride Sub put(ByVal i As Long, ByVal element As Single) Implements DataBuffer.put

        ''' <summary>Assign an element in the buffer to the specified index (BOOL stores <c>element &gt; 0</c>, as in Java).</summary>
        Public MustOverride Sub put(ByVal i As Long, ByVal element As Double) Implements DataBuffer.put

        ''' <summary>Assign an element in the buffer to the specified index.</summary>
        Public MustOverride Sub put(ByVal i As Long, ByVal element As Integer) Implements DataBuffer.put

        ''' <summary>Assign an element in the buffer to the specified index.</summary>
        Public MustOverride Sub put(ByVal i As Long, ByVal element As Long) Implements DataBuffer.put

        ''' <summary>Assign an element in the buffer to the specified index (1 or 0).</summary>
        Public MustOverride Sub put(ByVal i As Long, ByVal element As Boolean) Implements DataBuffer.put

        ''' <summary>Deprecated in Java: always False.</summary>
        Public Overridable Function dirty() As Boolean Implements DataBuffer.dirty
            Return False
        End Function

        ''' <summary>True when both buffers use the same underlying storage (a root buffer and its views do).</summary>
        Public MustOverride Function sameUnderlyingData(ByVal buffer As DataBuffer) As Boolean Implements DataBuffer.sameUnderlyingData

        ''' <summary>Assigns <paramref name="value"/> (a boxed number, through <c>doubleValue()</c>) to the elements from <paramref name="offset"/> on.</summary>
        Public Overridable Sub assign(ByVal value As Object, ByVal offset As Long) Implements DataBuffer.assign
            Dim d As Double = JavaNumber.doubleValue(value)
            Dim i As Long = offset
            Do While i < length()
                put(i, d)
                i += 1
            Loop
        End Sub

        ''' <summary>Writes this buffer to a stream in Java's layout (see <see cref="write(DataOutputStream)"/>). The stream stays open.</summary>
        Public Overridable Sub write(ByVal dos As Stream) Implements DataBuffer.write
            Dim dos2 As New DataOutputStream(dos, True)
            Try
                write(dos2)
                dos2.flush()
            Catch e As IOException
                Throw New InvalidOperationException("IO Exception writing buffer", e)
            End Try
        End Sub

        ''' <summary>Reads the elements from a stream in Java's layout (the header was already read, see <see cref="readHeader"/>). The stream stays open.</summary>
        Public Overridable Sub read(ByVal [is] As Stream, ByVal allocationMode As DataBuffer.AllocationMode, ByVal length As Long, ByVal dataType As DataType) Implements DataBuffer.read
            Dim dis2 As New DataInputStream([is], True)
            read(dis2, allocationMode, length, dataType)
        End Sub

        ''' <summary>Flush the data buffer (nothing to do).</summary>
        Public Overridable Sub flush() Implements DataBuffer.flush
        End Sub

        ''' <summary>
        ''' Copies the elements of several buffers into this one: from each buffer <c>i</c>, every <c>strides(i)</c>-th element
        ''' from <c>offsets(i)</c> to its end; the total must be <paramref name="n"/>.
        ''' </summary>
        Public Overridable Sub assign(ByVal offsets() As Long, ByVal strides() As Long, ByVal n As Long, ParamArray ByVal buffers() As DataBuffer) Implements DataBuffer.assign
            If offsets.Length <> strides.Length OrElse strides.Length <> buffers.Length Then
                Throw New ArgumentException("Unable to assign buffers, please specify equal lengths strides, offsets, and buffers")
            End If
            Dim count As Long = 0
            For i As Integer = 0 To buffers.Length - 1
                'note here that the final put will take care of the offset
                Dim j As Long = offsets(i)
                Do While j < buffers(i).length()
                    put(count, buffers(i).getDouble(j))
                    count += 1
                    j += strides(i)
                Loop
            Next i

            If count <> n Then
                Throw New ArgumentException("Strides and offsets didn't match up to length " & n)
            End If
        End Sub

        ''' <summary>Copies all elements of the given buffers, one after the other, into this buffer.</summary>
        Public Overridable Sub assign(ParamArray ByVal buffers() As DataBuffer) Implements DataBuffer.assign
            Dim offsets(buffers.Length - 1) As Long
            Dim strides(buffers.Length - 1) As Long
            For i As Integer = 0 To strides.Length - 1
                strides(i) = 1
            Next i
            assign(offsets, strides, buffers)
        End Sub

        ''' <summary>Release all resources for this buffer (nothing to do).</summary>
        Public Overridable Sub destroy() Implements DataBuffer.destroy
        End Sub

        ''' <summary>The data type of the buffer.</summary>
        Public Overridable Function dataType() As DataType Implements DataBuffer.dataType
            Return type
        End Function

        ''' <summary>
        ''' Java's element-wise comparison: True when <paramref name="o"/> is a data buffer of the same length whose elements
        ''' differ by at most 1e-12, and (as in Java) also True when <paramref name="o"/> is not a data buffer at all.
        ''' </summary>
        Public Overrides Function Equals(ByVal o As Object) As Boolean
            ' FIXME (Java): this is BAD. it takes too long to work, and it breaks general equals contract
            If TypeOf o Is DataBuffer Then
                Dim d As DataBuffer = DirectCast(o, DataBuffer)
                If d.length() <> length() Then
                    Return False
                End If
                For i As Long = 0 To length() - 1
                    Dim eps As Double = SysMath.Abs(getDouble(i) - d.getDouble(i))
                    If eps > 0.000000000001 Then
                        Return False
                    End If
                Next i
            End If

            Return True
        End Function

        ''' <summary>
        ''' Reads the header Java's <see cref="write(DataOutputStream)"/> writes: the allocation mode, the length (an int for
        ''' the legacy DIRECT/HEAP/JAVACPP modes, a long otherwise) and the data type.
        ''' </summary>
        Public Shared Function readHeader(ByVal [is] As Stream) As Triple(Of DataBuffer.AllocationMode, Long, DataType)
            Return readHeader(New DataInputStream([is], True))
        End Function

        ''' <summary>Same as <see cref="readHeader(Stream)"/> for a <see cref="DataInputStream"/>.</summary>
        Public Shared Function readHeader(ByVal dis As DataInputStream) As Triple(Of DataBuffer.AllocationMode, Long, DataType)
            Dim alloc As DataBuffer.AllocationMode = parseAllocationMode(dis.readUTF())
            Dim length As Long
            If CInt(alloc) < 3 Then
                length = dis.readInt()
            Else
                length = dis.readLong()
            End If
            Dim type As DataType = DataTypeExtensions.DataTypeValueOf(dis.readUTF())

            Return Triple.tripleOf(alloc, length, type)
        End Function

        Private Shared Function parseAllocationMode(ByVal name As String) As DataBuffer.AllocationMode
            For Each m As DataBuffer.AllocationMode In [Enum].GetValues(GetType(DataBuffer.AllocationMode))
                If String.Equals(m.ToString(), name, StringComparison.Ordinal) Then
                    Return m
                End If
            Next
            Throw New ArgumentException("No enum constant AllocationMode." & name)
        End Function

        ''' <summary>
        ''' Reads <paramref name="len"/> elements of type <paramref name="dtype"/> from the stream (the header was already
        ''' read), replacing this buffer's storage, as Java does.
        ''' </summary>
        Public Overridable Sub read(ByVal s As DataInputStream, ByVal allocMode As DataBuffer.AllocationMode, ByVal len As Long, ByVal dtype As DataType) Implements DataBuffer.read
            Dim savedMode As DataBuffer.AllocationMode = allocMode
            Me.allocationModeField = DataBuffer.AllocationMode.MIXED_DATA_TYPES
            type = dtype
            lengthField = len

            ' old AllocationMode values are: DIRECT, HEAP, JAVACPP. Just using legacy here
            If CInt(savedMode) < 3 Then
                'Do an implicit conversion: keep current buffer data type unchanged, and convert values from source type
                lengthField = len
                Dim sourceType As DataType = dtype
                elementSizeField = elementSizeOf(type)
                pointerIndexerByCurrentType(type)

                If sourceType <> DT.COMPRESSED Then
                    Dim thisType As DataType = dataType()
                    readContent(s, sourceType, thisType)
                End If
            ElseIf savedMode = DataBuffer.AllocationMode.LONG_SHAPE Then
                lengthField = len
                Dim currentType As DataType = dtype
                type = currentType
                elementSizeField = elementSizeOf(currentType)

                pointerIndexerByCurrentType(currentType)

                If currentType <> DT.COMPRESSED Then
                    readContent(s, currentType, currentType)
                End If
            Else
                elementSizeField = elementSizeOf(type)
                If type = DT.COMPRESSED OrElse type = DT.UNKNOWN Then
                    Throw New NotSupportedException()
                End If

                pointerIndexerByCurrentType(type)

                If type <> DT.COMPRESSED Then
                    readContent(s, type, type)
                End If
            End If
        End Sub

        ''' <summary>Bytes per element of a type (1 for UTF8, as Java's UTF8 buffer).</summary>
        Protected Friend Shared Function elementSizeOf(ByVal dataType As DataType) As Integer
            If dataType = DT.UTF8 Then
                Return 1
            End If
            Return SysMath.Max(dataType.width(), 0)
        End Function

        ''' <summary>Reads the elements of <paramref name="sourceType"/> and stores them as <paramref name="thisType"/> (Java <c>readContent</c>).</summary>
        Protected Friend Overridable Sub readContent(ByVal s As DataInputStream, ByVal sourceType As DataType, ByVal thisType As DataType)
            Dim n As Long = length()
            Select Case sourceType
                Case DT.DOUBLE
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, s.readDouble(), thisType)
                    Next i
                Case DT.FLOAT
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, s.readFloat(), thisType)
                    Next i
                Case DT.HALF
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, NumericCasts.HalfToSingle(CUShort(s.readShort() And &HFFFF)), thisType)
                    Next i
                Case DT.BFLOAT16
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, NumericCasts.BFloat16ToSingle(CUShort(s.readShort() And &HFFFF)), thisType)
                    Next i
                Case DT.UINT64, DT.LONG
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, s.readLong(), thisType)
                    Next i
                Case DT.UINT32
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, CLng(s.readInt()), thisType)
                    Next i
                Case DT.INT
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, s.readInt(), thisType)
                    Next i
                Case DT.UINT16, DT.SHORT
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, CInt(s.readShort()), thisType)
                    Next i
                Case DT.UBYTE, DT.BYTE, DT.BOOL
                    For i As Long = 0 To n - 1
                        putByDestinationType(i, CInt(s.readByte()), thisType)
                    Next i
                Case DT.UTF8
                    readUtf8Content(s, n)
                Case Else
                    Throw New NotSupportedException("Cannot read type: " & sourceType.name() & " to " & thisType.name())
            End Select
        End Sub

        ''' <summary>Element access without synchronization (used by the <c>asX()</c> copies and <see cref="write(DataOutputStream)"/>).</summary>
        Protected Friend Overridable Function getDoubleUnsynced(ByVal index As Long) As Double
            Return getDouble(index)
        End Function

        ''' <summary>See <see cref="getDoubleUnsynced"/>.</summary>
        Protected Friend Overridable Function getFloatUnsynced(ByVal index As Long) As Single
            Return getFloat(index)
        End Function

        ''' <summary>See <see cref="getDoubleUnsynced"/>.</summary>
        Protected Friend Overridable Function getLongUnsynced(ByVal index As Long) As Long
            Return getLong(index)
        End Function

        ''' <summary>See <see cref="getDoubleUnsynced"/>.</summary>
        Protected Friend Overridable Function getIntUnsynced(ByVal index As Long) As Integer
            Return getInt(index)
        End Function

        ''' <summary>
        ''' The length written in the stream header: <see cref="length"/>, except for UTF8 buffers, whose header holds the byte
        ''' length of Java's UTF8 layout.
        ''' </summary>
        Protected Friend Overridable Function streamLength() As Long
            Return length()
        End Function

        ''' <summary>Java's UTF8 buffer layout of this buffer (only UTF8 buffers support it).</summary>
        Protected Friend Overridable Function utf8Layout() As Byte()
            Throw New NotSupportedException("Unsupported data type: " & dataType().name())
        End Function

        ''' <summary>Reads <paramref name="byteLength"/> bytes of Java's UTF8 layout (only UTF8 buffers support it).</summary>
        Protected Friend Overridable Sub readUtf8Content(ByVal s As DataInputStream, ByVal byteLength As Long)
            Throw New NotSupportedException("Cannot read type: UTF8 to " & dataType().name())
        End Sub

        ''' <summary>
        ''' Writes the buffer in Java's layout: <c>writeUTF(allocationMode)</c>, <c>writeLong(length)</c>,
        ''' <c>writeUTF(dataType.name())</c> and the elements (see the class remarks).
        ''' </summary>
        Public Overridable Sub write(ByVal [out] As DataOutputStream) Implements DataBuffer.write
            [out].writeUTF(allocationModeField.ToString())
            [out].writeLong(streamLength())
            [out].writeUTF(dataType().name())
            Dim n As Long = length()
            Select Case dataType()
                Case DT.DOUBLE
                    For i As Long = 0 To n - 1
                        [out].writeDouble(getDoubleUnsynced(i))
                    Next i
                Case DT.UINT64, DT.LONG
                    For i As Long = 0 To n - 1
                        [out].writeLong(getLongUnsynced(i))
                    Next i
                Case DT.UINT32, DT.INT
                    For i As Long = 0 To n - 1
                        [out].writeInt(getIntUnsynced(i))
                    Next i
                Case DT.UINT16, DT.SHORT
                    For i As Long = 0 To n - 1
                        [out].writeShort(getIntUnsynced(i))
                    Next i
                Case DT.UBYTE, DT.BYTE
                    For i As Long = 0 To n - 1
                        [out].writeByte(getIntUnsynced(i))
                    Next i
                Case DT.BOOL
                    For i As Long = 0 To n - 1
                        [out].writeByte(If(getIntUnsynced(i) = 0, 0, 1))
                    Next i
                Case DT.BFLOAT16
                    For i As Long = 0 To n - 1
                        [out].writeShort(NumericCasts.SingleToBFloat16(getFloatUnsynced(i)))
                    Next i
                Case DT.HALF
                    For i As Long = 0 To n - 1
                        [out].writeShort(NumericCasts.SingleToHalf(getFloatUnsynced(i)))
                    Next i
                Case DT.FLOAT
                    For i As Long = 0 To n - 1
                        [out].writeFloat(getFloatUnsynced(i))
                    Next i
                Case DT.UTF8
                    [out].write(utf8Layout())
            End Select
        End Sub

        ''' <summary>Decodes a 16-bit half-precision pattern (Java <c>toFloat</c>).</summary>
        Public Overridable Function toFloat(ByVal hbits As Integer) As Single
            Dim mant As Integer = hbits And &H3FF ' 10 bits mantissa
            Dim exp As Integer = hbits And &H7C00 ' 5 bits exponent
            If exp = &H7C00 Then ' NaN/Inf
                exp = &H3FC00 ' -> NaN/Inf
            ElseIf exp <> 0 Then ' normalized value
                exp += &H1C000 ' exp - 15 + 127
            ElseIf mant <> 0 Then ' && exp==0 -> subnormal
                exp = &H1C400 ' make it normal
                Do
                    mant <<= 1 ' mantissa * 2
                    exp -= &H400 ' decrease exp by 1
                Loop While (mant And &H400) = 0 ' while not normal
                mant = mant And &H3FF ' discard subnormal bit
            End If ' else +/-0 -> +/-0
            Return NumericCasts.BitsToSingle(((hbits And &H8000) << 16) Or ((exp Or mant) << 13)) ' value << ( 23 - 10 )
        End Function

        ''' <summary>Java returns no backing array: always Nothing (the managed buffer exposes its storage separately).</summary>
        Public Overridable Function array() As Object Implements DataBuffer.array
            Return Nothing
        End Function

        ''' <summary>
        ''' Java's rendering: <c>[1,2,3]</c> for integer types, <c>[ true, false]</c> for BOOL and Java float text otherwise,
        ''' at most <see cref="ND4VBSystemProperties.DATABUFFER_TO_STRING_MAX_ELEMENTS"/> elements (default 1000).
        ''' </summary>
        Public Overrides Function ToString() As String
            Dim ret As New StringBuilder()
            ret.Append("[")

            Dim max As Integer
            If TO_STRING_MAX >= 0 Then
                max = CInt(SysMath.Min(length(), TO_STRING_MAX))
            Else
                max = CInt(SysMath.Min(length(), Integer.MaxValue))
            End If

            For i As Integer = 0 To max - 1
                Select Case dataType()
                    Case DT.UBYTE, DT.BYTE, DT.INT, DT.SHORT, DT.LONG
                        ret.Append(JavaNumber.intValue(getNumber(i)).ToString(CultureInfo.InvariantCulture))
                    Case DT.BOOL
                        ret.Append(If(JavaNumber.intValue(getNumber(i)) = 0, " false", " true"))
                    Case DT.UTF8
                        Throw New NotSupportedException()
                    Case Else
                        ret.Append(Arrays.JavaFloatToString(JavaNumber.floatValue(getNumber(i))))
                End Select
                If i < max - 1 Then
                    ret.Append(",")
                End If
            Next i
            If max < length() Then
                ret.Append(",<").Append(length() - max).Append(" more elements>")
            End If
            ret.Append("]")

            Return ret.ToString()
        End Function

        ''' <summary>Java's hash: the length combined with the allocation mode (consistent with <see cref="Equals"/>).</summary>
        Public Overrides Function GetHashCode() As Integer
            Dim result As Integer = NumericCasts.LongToInt(lengthField)
            result = 31 * result + CInt(allocationModeField)
            Return result
        End Function

        ''' <summary>Returns the offset of the buffer relative to <see cref="originalDataBuffer"/>.</summary>
        Public Overridable Function originalOffset() As Long Implements DataBuffer.originalOffset
            Return originalOffsetField
        End Function

        ''' <summary>
        ''' Whether this buffer is constant: modified only at creation and then the same for its whole life (shape
        ''' information and constant-cache buffers).
        ''' </summary>
        Public Overridable Property Constant As Boolean Implements DataBuffer.Constant
            Get
                Return constantField
            End Get
            Set(ByVal reallyConstant As Boolean)
                Me.constantField = reallyConstant
            End Set
        End Property

        ''' <summary>True if this buffer was allocated in a workspace (a view reports False, as in Java).</summary>
        Public Overridable ReadOnly Property Attached As Boolean Implements DataBuffer.Attached
            Get
                Return attachedField
            End Get
        End Property

        ''' <summary>
        ''' True when the buffer is not attached, or its parent workspace's scope is still open.
        ''' </summary>
        Public Overridable ReadOnly Property InScope As Boolean Implements DataBuffer.InScope
            Get
                If Not Attached Then
                    Return True
                End If

                Return parentWorkspaceField.ScopeActive
            End Get
        End Property

        ''' <summary>The workspace this buffer (or the attached buffer it views) was allocated in, or Nothing.</summary>
        Public Overridable ReadOnly Property ParentWorkspace As MemoryWorkspace Implements DataBuffer.ParentWorkspace
            Get
                If parentWorkspaceField IsNot Nothing Then
                    Return parentWorkspaceField
                End If
                If wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer IsNot Me AndAlso wrappedDataBuffer.Attached AndAlso wrappedDataBuffer.ParentWorkspace IsNot Nothing Then
                    Return wrappedDataBuffer.ParentWorkspace
                End If
                If originalBuffer IsNot Nothing AndAlso originalBuffer.Attached AndAlso originalBuffer.ParentWorkspace IsNot Nothing Then
                    Return originalBuffer.ParentWorkspace
                End If
                Return Nothing
            End Get
        End Property

        ''' <summary>Grows (or shrinks) the buffer to <paramref name="length"/> elements, keeping the elements that fit; returns this buffer.</summary>
        Public MustOverride Function reallocate(ByVal length As Long) As DataBuffer Implements DataBuffer.reallocate

        ''' <summary>The number of elements the underlying storage holds.</summary>
        Public MustOverride Function capacity() As Long Implements DataBuffer.capacity

        ''' <summary>True if <see cref="close"/> may release this buffer: not released, not attached, not constant and not a view.</summary>
        Public Overridable Function closeable() As Boolean Implements DataBuffer.closeable
            If released OrElse Attached OrElse Constant Then
                Return False
            End If

            If wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer IsNot Me Then
                Return False
            End If

            Return True
        End Function

        ''' <summary>Marks the buffer released.</summary>
        Protected Friend Overridable Sub markReleased()
            Me.released = True
        End Sub

        ''' <summary>
        ''' Releases the buffer's storage; afterwards element access throws <see cref="InvalidOperationException"/>.
        ''' Throws <see cref="InvalidOperationException"/> when the buffer is not <see cref="closeable"/>.
        ''' </summary>
        Public Overridable Sub close() Implements DataBuffer.close
            If Not closeable() Then
                Throw New InvalidOperationException("Can't release this data buffer")
            End If

            release()
        End Sub

        ''' <summary>Releases the storage (subclasses drop their references, then call this).</summary>
        Protected Friend Overridable Sub release()
            Me.released = True
        End Sub

        ''' <summary>True if this buffer (or the buffer it views) was closed.</summary>
        Public Overridable Function wasClosed() As Boolean Implements DataBuffer.wasClosed
            If wrappedDataBuffer IsNot Nothing AndAlso wrappedDataBuffer IsNot Me Then
                Return wrappedDataBuffer.wasClosed()
            End If

            Return released
        End Function

        ''' <summary>Same as <see cref="close"/> (Java <c>AutoCloseable</c>), except that it does nothing when this buffer was already released.</summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            If released Then
                Return
            End If
            close()
        End Sub

        ''' <summary>This method synchronizes host memory (nothing to do for managed memory).</summary>
        Public Overridable Sub syncToPrimary()
        End Sub

        ''' <summary>This method synchronizes device memory (nothing to do for managed memory).</summary>
        Public Overridable Sub syncToSpecial()
        End Sub

    End Class

End Namespace
