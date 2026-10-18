' DeepLearning4VB: the managed backend's data buffer factory (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.buffer.DefaultDataBufferFactory.
' The create* methods below the special members are generated from the DataBufferFactory interface
' (scratchpad w2buffers\gen_factory.py); keep them in that shape.

Imports System
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DataBufferFactory = DeepLearning4VB.org.nd4vb.linalg.api.buffer.factory.DataBufferFactory
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports StorageType = DeepLearning4VB.org.nd4vb.linalg.managed.storage.StorageType
Imports ManagedStorage = DeepLearning4VB.org.nd4vb.linalg.managed.storage.ManagedStorage
Imports NumericCasts = DeepLearning4VB.org.nd4vb.linalg.managed.storage.NumericCasts

Namespace org.nd4vb.linalg.managed.buffer

    ''' <summary>
    ''' Creates <see cref="ManagedDataBuffer"/>s (Java <c>DefaultDataBufferFactory</c> of the CPU backend).
    ''' </summary>
    ''' <remarks>
    ''' <para>Every overload follows the CPU factory: arrays of another element type are converted with Java's casts, the
    ''' <c>(offset, data)</c> overloads record the offset as Java's <c>(data, copy, offset)</c> constructors do (see
    ''' <see cref="ManagedDataBuffer"/>), the <c>(offset, Byte(), length)</c> overloads decode big-endian doubles or floats
    ''' (Java <c>ArrayUtil.toDoubleArray</c>/<c>toFloatArray</c>; the length argument is ignored, as in Java), and
    ''' <c>createSame</c> creates a buffer of the same type and length.</para>
    ''' <para>Deliberate differences from Java:
    ''' <list type="bullet">
    ''' <item>HALF buffers can be created from arrays (the CPU factory threw "FP16 isn't supported for CPU yet"), and
    ''' <see cref="halfBufferClass"/> returns the managed buffer class (Java: null).</item>
    ''' <item><see cref="create(Byte(), DataType, Long, Long)"/> decodes the raw little-endian elements for every type except
    ''' UTF8 (Java took the bytes of BOOL, BFLOAT16 and the unsigned types from uninitialized memory instead); UTF8 throws
    ''' <see cref="NotSupportedException"/> (use <see cref="createUtf8Buffer"/>).</item>
    ''' <item><see cref="create(DataType, Long, Boolean, MemoryWorkspace)"/> also creates UTF8 buffers.</item>
    ''' <item>Without <see cref="setAllocationMode"/>, <see cref="allocationMode"/> reads the "alloc" system property
    ''' ("heap", "direct" or "javacpp") and otherwise returns MIXED_DATA_TYPES (Java failed with a
    ''' <c>NullPointerException</c> when the property was missing).</item>
    ''' </list></para>
    ''' <para>Native-only members not ported: <c>create(Pointer, DataType, long, Indexer)</c>,
    ''' <c>create(Pointer, Pointer, DataType, long, Indexer)</c>, <c>create(DoublePointer, long)</c>,
    ''' <c>create(IntPointer, long)</c>, <c>create(FloatPointer, long)</c>.</para>
    ''' </remarks>
    Public Class ManagedDataBufferFactory
        Implements DataBufferFactory

        Protected Friend allocationModeField As DataBuffer.AllocationMode?

        ''' <summary>Setter for the allocation mode.</summary>
        Public Overridable Sub setAllocationMode(ByVal allocationMode As DataBuffer.AllocationMode) Implements DataBufferFactory.setAllocationMode
            Me.allocationModeField = allocationMode
        End Sub

        ''' <summary>The allocation mode (see the class remarks).</summary>
        Public Overridable Function allocationMode() As DataBuffer.AllocationMode Implements DataBufferFactory.allocationMode
            If Not allocationModeField.HasValue Then
                Select Case JavaSystem.getProperty("alloc")
                    Case "heap"
                        allocationModeField = DataBuffer.AllocationMode.HEAP
                    Case "direct"
                        allocationModeField = DataBuffer.AllocationMode.DIRECT
                    Case "javacpp"
                        allocationModeField = DataBuffer.AllocationMode.JAVACPP
                    Case Else
                        Return DataBuffer.AllocationMode.MIXED_DATA_TYPES
                End Select
            End If
            Return allocationModeField.Value
        End Function

        ''' <summary>The buffer class of INT buffers.</summary>
        Public Overridable Function intBufferClass() As Type Implements DataBufferFactory.intBufferClass
            Return GetType(ManagedDataBuffer)
        End Function

        ''' <summary>The buffer class of LONG buffers.</summary>
        Public Overridable Function longBufferClass() As Type Implements DataBufferFactory.longBufferClass
            Return GetType(ManagedDataBuffer)
        End Function

        ''' <summary>The buffer class of HALF buffers.</summary>
        Public Overridable Function halfBufferClass() As Type Implements DataBufferFactory.halfBufferClass
            Return GetType(ManagedDataBuffer)
        End Function

        ''' <summary>The buffer class of FLOAT buffers.</summary>
        Public Overridable Function floatBufferClass() As Type Implements DataBufferFactory.floatBufferClass
            Return GetType(ManagedDataBuffer)
        End Function

        ''' <summary>The buffer class of DOUBLE buffers.</summary>
        Public Overridable Function doubleBufferClass() As Type Implements DataBufferFactory.doubleBufferClass
            Return GetType(ManagedDataBuffer)
        End Function

        ''' <summary>A UTF8 buffer from Java's UTF8 layout holding <paramref name="product"/> strings.</summary>
        Public Overridable Function createUtf8Buffer(ByVal data As Byte(), ByVal product As Long) As DataBuffer Implements DataBufferFactory.createUtf8Buffer
            Return New ManagedDataBuffer(data, product)
        End Function

        ''' <summary>Throws Java's exception for the types without a buffer (COMPRESSED, UNKNOWN).</summary>
        Protected Shared Function checkType(ByVal dataType As DataType) As DataType
            If dataType = DataType.COMPRESSED OrElse dataType = DataType.UNKNOWN Then
                Throw New InvalidOperationException("Unknown datatype used: [" & dataType.name() & "]")
            End If
            Return dataType
        End Function

        ''' <summary>A buffer in <paramref name="workspace"/> (detached when it is Nothing).</summary>
        Protected Shared Function newBuffer(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer
            If workspace Is Nothing Then
                Return New ManagedDataBuffer(dataType, length, initialize)
            End If
            Return New ManagedDataBuffer(dataType, length, initialize, workspace)
        End Function

        ''' <summary>Decodes consecutive big-endian 8-byte doubles (Java <c>ArrayUtil.toDoubleArray(byte[])</c>).</summary>
        Protected Shared Function decodeBigEndianDoubles(ByVal data As Byte()) As Double()
            Dim result(data.Length \ 8 - 1) As Double
            For i As Integer = 0 To result.Length - 1
                result(i) = BitConverter.Int64BitsToDouble(readBigEndian(data, i * 8, 8))
            Next
            Return result
        End Function

        ''' <summary>Decodes consecutive big-endian 4-byte floats (Java <c>ArrayUtil.toFloatArray(byte[])</c>).</summary>
        Protected Shared Function decodeBigEndianFloats(ByVal data As Byte()) As Single()
            Dim result(data.Length \ 4 - 1) As Single
            For i As Integer = 0 To result.Length - 1
                result(i) = NumericCasts.BitsToSingle(NumericCasts.LongToInt(readBigEndian(data, i * 4, 4)))
            Next
            Return result
        End Function

        Private Shared Function readBigEndian(ByVal data As Byte(), ByVal index As Integer, ByVal count As Integer) As Long
            Dim value As Long = 0
            For b As Integer = 0 To count - 1
                value = (value << 8) Or data(index + b)
            Next
            Return value
        End Function

        ''' <summary>
        ''' A buffer of <paramref name="length"/> elements decoded from raw little-endian bytes, starting at element
        ''' <paramref name="offset"/> (Java: the contents of a native-order <c>ByteBuffer</c>).
        ''' </summary>
        Protected Shared Function fromRawBytes(ByVal bytes As Byte(), ByVal type As DataType, ByVal length As Long, ByVal offset As Long) As DataBuffer
            If bytes Is Nothing Then
                Throw New ArgumentNullException(NameOf(bytes))
            End If
            checkType(type)
            If type = DataType.UTF8 Then
                Throw New NotSupportedException("Use createUtf8Buffer for UTF8 data")
            End If
            Dim size As Integer = type.width()
            Dim result As New ManagedDataBuffer(type, length, False)
            Dim s As ManagedStorage = result.Storage
            For i As Long = 0 To length - 1
                Dim start As Long = (offset + i) * size
                Dim raw As Long = 0
                For b As Integer = size - 1 To 0 Step -1
                    raw = (raw << 8) Or bytes(CInt(start + b))
                Next
                Select Case s.StorageType
                    Case StorageType.DOUBLE
                        s.Doubles(CInt(i)) = BitConverter.Int64BitsToDouble(raw)
                    Case StorageType.FLOAT
                        s.Floats(CInt(i)) = NumericCasts.BitsToSingle(NumericCasts.LongToInt(raw))
                    Case StorageType.HALF, StorageType.BFLOAT16
                        s.HalfBits(CInt(i)) = CUShort(raw And &HFFFFL)
                    Case StorageType.BOOL
                        s.Booleans(CInt(i)) = raw <> 0
                    Case StorageType.UINT64
                        s.ULongs(CInt(i)) = NumericCasts.LongToULong(raw)
                    Case StorageType.UINT32
                        s.UInts(CInt(i)) = CUInt(raw And &HFFFFFFFFL)
                    Case StorageType.UINT16
                        s.UShorts(CInt(i)) = CUShort(raw And &HFFFFL)
                    Case StorageType.UBYTE
                        s.UBytes(CInt(i)) = CByte(raw And &HFFL)
                    Case Else
                        ' signed integers: sign-extend from the element width
                        Dim shift As Integer = 64 - size * 8
                        s.SetLong(i, (raw << shift) >> shift)
                End Select
            Next
            Return result
        End Function

        ''' <inheritdoc/>
        Public Overridable Function create(ByVal underlyingBuffer As DataBuffer, ByVal offset As Long, ByVal length As Long) As DataBuffer Implements DataBufferFactory.create
            Return New ManagedDataBuffer(underlyingBuffer, length, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function create(ByVal underlyingBuffer As Byte(), ByVal type As DataType, ByVal length As Long, ByVal offset As Long) As DataBuffer Implements DataBufferFactory.create
            Return fromRawBytes(underlyingBuffer, type, length, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createDouble
            Return ManagedDataBuffer.recordedOffset(DataType.DOUBLE, length, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean) As DataBuffer Implements DataBufferFactory.createSame
            Return create(buffer.dataType(), buffer.length(), init)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createSame(ByVal buffer As DataBuffer, ByVal init As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createSame
            Return create(buffer.dataType(), buffer.length(), init, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createFloat
            Return ManagedDataBuffer.recordedOffset(DataType.FLOAT, length, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createInt
            Return ManagedDataBuffer.recordedOffset(DataType.INT, length, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, offset, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Byte, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, decodeBigEndianDoubles(data), offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Byte, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, decodeBigEndianFloats(data), offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, offset, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
            Return newBuffer(DataType.DOUBLE, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
            Return newBuffer(DataType.FLOAT, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.create
            Return New ManagedDataBuffer(checkType(dataType), length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function create(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.create
            Return newBuffer(checkType(dataType), length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createBFloat16(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createBFloat16
            Return New ManagedDataBuffer(DataType.BFLOAT16, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createByte(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createByte
            Return New ManagedDataBuffer(DataType.BYTE, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createShort(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createShort
            Return New ManagedDataBuffer(DataType.SHORT, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createBool(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createBool
            Return New ManagedDataBuffer(DataType.BOOL, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUShort(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createUShort
            Return New ManagedDataBuffer(DataType.UINT16, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUInt(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createUInt
            Return New ManagedDataBuffer(DataType.UINT32, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUByte(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createUByte
            Return New ManagedDataBuffer(DataType.UBYTE, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createULong(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createULong
            Return New ManagedDataBuffer(DataType.UINT64, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createBFloat16
            Return New ManagedDataBuffer(DataType.BFLOAT16, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createByte
            Return New ManagedDataBuffer(DataType.BYTE, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createShort
            Return New ManagedDataBuffer(DataType.SHORT, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createBool(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createBool
            Return New ManagedDataBuffer(DataType.BOOL, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUShort(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createUShort
            Return New ManagedDataBuffer(DataType.UINT16, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createUInt
            Return New ManagedDataBuffer(DataType.UINT32, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUByte(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createUByte
            Return New ManagedDataBuffer(DataType.UBYTE, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createULong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createULong
            Return New ManagedDataBuffer(DataType.UINT64, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createBFloat16(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createBFloat16
            Return newBuffer(DataType.BFLOAT16, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createByte
            Return newBuffer(DataType.BYTE, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createShort
            Return newBuffer(DataType.SHORT, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createBool(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createBool
            Return newBuffer(DataType.BOOL, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUShort(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createUShort
            Return newBuffer(DataType.UINT16, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createUInt
            Return newBuffer(DataType.UINT32, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createUByte(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createUByte
            Return newBuffer(DataType.UBYTE, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createULong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createULong
            Return newBuffer(DataType.UINT64, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createInt
            Return newBuffer(DataType.INT, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createLong(ByVal data() As Long) As DataBuffer Implements DataBufferFactory.createLong
            Return New ManagedDataBuffer(DataType.LONG, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createLong(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createLong
            Return New ManagedDataBuffer(DataType.LONG, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createLong(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createLong
            Return New ManagedDataBuffer(DataType.LONG, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createLong(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createLong
            Return New ManagedDataBuffer(DataType.LONG, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createLong(ByVal data() As Long, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createLong
            Return New ManagedDataBuffer(DataType.LONG, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createLong(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createLong
            Return newBuffer(DataType.LONG, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Integer, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Integer, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createLong(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createLong
            Return New ManagedDataBuffer(DataType.LONG, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Long, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Double, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Double, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createDouble(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createDouble
            Return New ManagedDataBuffer(DataType.DOUBLE, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createFloat(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createFloat
            Return New ManagedDataBuffer(DataType.FLOAT, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createInt(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createInt
            Return New ManagedDataBuffer(DataType.INT, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal length As Long) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal length As Long, ByVal initialize As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, length, initialize)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
            Return newBuffer(DataType.HALF, length, initialize, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Single, ByVal copy As Boolean, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Double, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Single, ByVal workspace As MemoryWorkspace) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, offset, workspace)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data, offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal data() As Byte, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, decodeBigEndianFloats(data), offset)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Integer, ByVal copy As Boolean) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Single) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Double) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal data() As Integer) As DataBuffer Implements DataBufferFactory.createHalf
            Return New ManagedDataBuffer(DataType.HALF, data)
        End Function

        ''' <inheritdoc/>
        Public Overridable Function createHalf(ByVal offset As Long, ByVal length As Integer) As DataBuffer Implements DataBufferFactory.createHalf
            Return ManagedDataBuffer.recordedOffset(DataType.HALF, length, offset)
        End Function

    End Class

End Namespace
