' DeepLearning4VB: the managed backend's data buffer (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.buffer.BaseCpuDataBuffer and its typed subclasses.

Imports System
Imports DT = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports AllocUtil = DeepLearning4VB.org.nd4vb.linalg.api.buffer.util.AllocUtil
Imports MemoryKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.MemoryKind
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports Nd4vbWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.Nd4vbWorkspace
Imports DataInputStream = DeepLearning4VB.JavaCompat.DataInputStream
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.buffer

    ''' <summary>
    ''' A <see cref="DataBuffer"/> over a <see cref="ManagedStorage"/> (a typed .NET array), an element offset and a
    ''' length. Views share the storage of the buffer they view, so writes through a view are visible in the original
    ''' and in every other view. The element type maps to the storage type by name (<see cref="toStorageType"/>).
    ''' </summary>
    ''' <remarks>
    ''' <para>Element access follows Java's <c>BaseDataBuffer</c>: <c>put</c> and the getters convert with Java's primitive
    ''' casts (see <see cref="NumericCasts"/>), <c>put(i, Double)</c> stores <c>value &gt; 0</c> in a BOOL buffer, UTF8
    ''' buffers have no numeric access. Offsets follow ND4J 1.0.0-M2: a view created with
    ''' <c>New ManagedDataBuffer(underlying, length, offset)</c> starts at element <paramref name="offset"/> of the
    ''' original buffer's storage (offsets are absolute, as in libnd4j's <c>InteropDataBuffer</c>), while a root buffer
    ''' created from an array with an offset (Java <c>new FloatBuffer(data, copy, offset)</c>) records the offset and the
    ''' shorter length but reads from element 0 of the data, as Java does.</para>
    ''' <para>Memory always comes from the garbage-collected heap. A buffer created with a workspace is <c>Attached</c>
    ''' to it: the workspace records the allocation in its statistics (<see cref="Nd4vbWorkspace.alloc(Long, MemoryKind, DataType, Boolean)"/>)
    ''' and the buffer remembers the workspace generation, so the scoping checks work as in Java.</para>
    ''' <para>Deliberate differences from Java:
    ''' <list type="bullet">
    ''' <item>Data from .NET arrays is always copied (Java copied into native memory too); <see cref="New(ManagedStorage)"/>
    ''' wraps a storage without copying.</item>
    ''' <item><see cref="dup"/> copies the storage exactly (Java copied through double, losing LONG precision above 2^53 and
    ''' failing for UTF8).</item>
    ''' <item>A UTF8 buffer holds strings: its <c>length()</c> is the number of strings (Java: the byte length of the
    ''' offset header and the data), its strings are real UTF-8 (Java truncated each char to a byte when encoding), and it
    ''' is written and read in Java's UTF8 byte layout (<c>numWords + 1</c> little-endian long offsets, then the bytes).</item>
    ''' <item>After the original buffer is closed, element access through its views throws
    ''' <see cref="InvalidOperationException"/> (Java read freed memory).</item>
    ''' <item>UINT64 elements read back unsigned (see <see cref="NumericCasts"/>).</item>
    ''' <item>A DUMMY workspace (outside any workspace scope) does not attach the buffer.</item>
    ''' </list></para>
    ''' <para>Native-only members not ported: <c>getOpaqueDataBuffer()</c>, <c>addressPointer()</c>,
    ''' <c>actualizePointerAndIndexer()</c>, the <c>(Pointer, Indexer, long)</c> and <c>(ByteBuffer, ...)</c> constructors (see
    ''' <c>ManagedDataBufferFactory.create(Byte(), DataType, Long, Long)</c>), <c>Deallocatable</c> registration with the
    ''' <c>DeallocatorService</c> (no native memory to free).</para>
    ''' </remarks>
    Public Class ManagedDataBuffer
        Inherits BaseDataBuffer

        ''' <summary>The storage shared by a root buffer and its views; Nothing once the root buffer is closed.</summary>
        Private NotInheritable Class StorageHandle
            Public Storage As ManagedStorage

            Public Sub New(ByVal storage As ManagedStorage)
                Me.Storage = storage
            End Sub
        End Class

        Private Shared ReadOnly StorageTypeByOrdinal As StorageType?() = buildStorageTypes()

        Private handle As StorageHandle
        Private storageOffsetField As Long

        Private Shared Function buildStorageTypes() As StorageType?()
            Dim values As DataType() = DataTypeExtensions.DataTypeValues()
            Dim result(values.Length - 1) As StorageType?
            For Each dt As DataType In values
                Dim st As StorageType
                If [Enum].TryParse(dt.name(), False, st) AndAlso [Enum].IsDefined(GetType(StorageType), st) Then
                    result(CInt(dt)) = st
                End If
            Next
            Return result
        End Function

        ''' <summary>The storage type of a data type (the same name); throws for COMPRESSED and UNKNOWN.</summary>
        Public Shared Function toStorageType(ByVal dataType As DataType) As StorageType
            Dim i As Integer = CInt(dataType)
            If i >= 0 AndAlso i < StorageTypeByOrdinal.Length AndAlso StorageTypeByOrdinal(i).HasValue Then
                Return StorageTypeByOrdinal(i).Value
            End If
            Throw New ArgumentException("Data type " & dataType.name() & " has no managed storage")
        End Function

        ''' <summary>The data type of a storage type (the same name).</summary>
        Public Shared Function toDataType(ByVal storageType As StorageType) As DataType
            Return DataTypeExtensions.DataTypeValueOf(storageType.ToString())
        End Function

#Region "Constructors"

        ''' <summary>A zero-filled buffer of <paramref name="length"/> elements.</summary>
        Public Sub New(ByVal dataType As DataType, ByVal length As Long)
            Me.New(dataType, length, True)
        End Sub

        ''' <summary>
        ''' A buffer of <paramref name="length"/> elements (always zero-filled: managed arrays are; <paramref name="initialize"/>
        ''' is accepted for Java compatibility).
        ''' </summary>
        Public Sub New(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean)
            If length < 0 Then
                Throw New ArgumentException("Length must be >= 0")
            End If
            initRoot(dataType, ManagedStorage.Create(toStorageType(dataType), length), length)
        End Sub

        ''' <summary>
        ''' A buffer of <paramref name="length"/> elements allocated in <paramref name="workspace"/> (detached when the
        ''' workspace is Nothing or a DUMMY workspace).
        ''' </summary>
        Public Sub New(ByVal dataType As DataType, ByVal length As Long, ByVal initialize As Boolean, ByVal workspace As MemoryWorkspace)
            If length < 1 Then
                Throw New ArgumentException("Length must be >= 1")
            End If
            initRoot(dataType, ManagedStorage.Create(toStorageType(dataType), length), length)
            attachTo(workspace, initialize)
        End Sub

        ''' <summary>
        ''' A view of <paramref name="length"/> elements of <paramref name="underlyingBuffer"/> (a managed buffer), starting
        ''' at element <paramref name="offset"/> of the original buffer's storage.
        ''' </summary>
        Public Sub New(ByVal underlyingBuffer As DataBuffer, ByVal length As Long, ByVal offset As Long)
            MyBase.New(underlyingBuffer, length, offset)
            Dim other As ManagedDataBuffer = TryCast(underlyingBuffer, ManagedDataBuffer)
            If other Is Nothing Then
                Throw New ArgumentException("A managed view needs a managed underlying buffer, got " & CObj(underlyingBuffer).GetType().FullName)
            End If
            If other.handle Is Nothing OrElse other.handle.Storage Is Nothing Then
                Throw New InvalidOperationException("You can't use DataBuffer once it was released")
            End If
            Me.handle = other.handle
            Me.storageOffsetField = offset
        End Sub

        ''' <summary>A buffer holding the values of <paramref name="data"/> (any typed .NET array), converted to <paramref name="dataType"/>.</summary>
        Public Sub New(ByVal dataType As DataType, ByVal data As Array)
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            initRoot(dataType, ManagedStorage.Create(toStorageType(dataType), data), data.LongLength)
        End Sub

        ''' <summary>A buffer holding the values of <paramref name="data"/>, allocated in <paramref name="workspace"/>.</summary>
        Public Sub New(ByVal dataType As DataType, ByVal data As Array, ByVal workspace As MemoryWorkspace)
            Me.New(dataType, data)
            attachTo(workspace, False)
        End Sub

        ''' <summary>
        ''' Java's <c>(data, copy, offset)</c> constructors: holds all of <paramref name="data"/>, records
        ''' <paramref name="offset"/>, and has the length <c>data.Length - offset</c>. As in Java, element 0 is
        ''' <c>data(0)</c>; use a view for offset element access.
        ''' </summary>
        Public Sub New(ByVal dataType As DataType, ByVal data As Array, ByVal offset As Long)
            Me.New(dataType, data)
            Me.offsetField = offset
            Me.originalOffsetField = offset
            Me.lengthField = data.LongLength - offset
            Me.underlyingLengthField = data.LongLength
        End Sub

        ''' <summary>Java's <c>(data, copy, offset, workspace)</c> constructors (see <see cref="New(DataType, Array, Long)"/>).</summary>
        Public Sub New(ByVal dataType As DataType, ByVal data As Array, ByVal offset As Long, ByVal workspace As MemoryWorkspace)
            Me.New(dataType, data, workspace)
            Me.offsetField = offset
            Me.originalOffsetField = offset
            Me.underlyingLengthField = data.LongLength
            Me.lengthField = underlyingLengthField - offset
        End Sub

        ''' <summary>A buffer over an existing storage (shared, not copied); the type is the storage's.</summary>
        Public Sub New(ByVal storage As ManagedStorage)
            If storage Is Nothing Then
                Throw New ArgumentNullException(NameOf(storage))
            End If
            initRoot(toDataType(storage.StorageType), storage, storage.Length)
        End Sub

        ''' <summary>A UTF8 buffer holding <paramref name="strings"/> (Java <c>Utf8Buffer(Collection&lt;String&gt;)</c>).</summary>
        Public Sub New(ByVal strings As ICollection(Of String))
            If strings Is Nothing Then
                Throw New ArgumentNullException(NameOf(strings))
            End If
            Dim data(strings.Count - 1) As String
            strings.CopyTo(data, 0)
            initRoot(DT.UTF8, ManagedStorage.FromArray(data, False), data.LongLength)
        End Sub

        ''' <summary>
        ''' A UTF8 buffer from Java's UTF8 layout: <paramref name="numWords"/> + 1 little-endian long offsets followed by the
        ''' UTF-8 bytes of the strings (Java <c>Utf8Buffer(byte[], long)</c>).
        ''' </summary>
        Public Sub New(ByVal utf8Layout As Byte(), ByVal numWords As Long)
            If utf8Layout Is Nothing Then
                Throw New ArgumentNullException(NameOf(utf8Layout))
            End If
            Dim strings As String() = parseUtf8Layout(utf8Layout, numWords)
            initRoot(DT.UTF8, ManagedStorage.FromArray(strings, False), strings.LongLength)
        End Sub

        ''' <summary>
        ''' Java's <c>(int length, int elementSize, long offset)</c> constructors: storage for <paramref name="length"/>
        ''' elements, the recorded offset and the length <c>length - offset</c>.
        ''' </summary>
        Friend Shared Function recordedOffset(ByVal dataType As DataType, ByVal length As Long, ByVal offset As Long) As ManagedDataBuffer
            If length < 1 Then
                Throw New ArgumentException("Length must be >= 1")
            End If
            Dim result As New ManagedDataBuffer(dataType, length, True)
            result.offsetField = offset
            result.originalOffsetField = offset
            result.lengthField = length - offset
            result.underlyingLengthField = length
            Return result
        End Function

        Private Sub initRoot(ByVal dataType As DataType, ByVal storage As ManagedStorage, ByVal length As Long)
            Me.type = dataType
            initTypeAndSize()
            Me.handle = New StorageHandle(storage)
            Me.storageOffsetField = 0
            Me.lengthField = length
            Me.underlyingLengthField = length
            Me.allocationModeField = AllocUtil.AllocationModeFromContext
            Me.wrappedDataBuffer = Me
        End Sub

        ''' <summary>Records the allocation in <paramref name="workspace"/> and marks the buffer attached.</summary>
        Private Sub attachTo(ByVal workspace As MemoryWorkspace, ByVal initialize As Boolean)
            If workspace Is Nothing OrElse workspace.WorkspaceType = MemoryWorkspace.Type.DUMMY Then
                Return
            End If
            Dim nws As Nd4vbWorkspace = TryCast(workspace, Nd4vbWorkspace)
            If nws IsNot Nothing Then
                nws.alloc(requiredBytes(lengthField), MemoryKind.HOST, type, initialize)
            End If
            attachedField = True
            parentWorkspaceField = workspace
            workspaceGenerationId = workspace.GenerationId
        End Sub

        Private Function requiredBytes(ByVal elements As Long) As Long
            If type = DT.UTF8 Then
                Return utf8LayoutLength()
            End If
            Return elements * elementSizeField
        End Function

        ''' <summary>Sets the element size for <see cref="BaseDataBuffer.type"/> (the type comes from the constructor).</summary>
        Protected Friend Overrides Sub initTypeAndSize()
            elementSizeField = elementSizeOf(type)
        End Sub

#End Region

#Region "Storage access"

        ''' <summary>The storage holding this buffer's elements (shared with its original buffer and views).</summary>
        ''' <exception cref="InvalidOperationException">The buffer (or its original buffer) was closed.</exception>
        Public ReadOnly Property Storage As ManagedStorage
            Get
                Return liveStorage()
            End Get
        End Property

        ''' <summary>The index in <see cref="Storage"/> of this buffer's element 0.</summary>
        Public ReadOnly Property StorageOffset As Long
            Get
                Return storageOffsetField
            End Get
        End Property

        Private Function liveStorage() As ManagedStorage
            Dim h As StorageHandle = handle
            Dim s As ManagedStorage = If(h Is Nothing, Nothing, h.Storage)
            If s Is Nothing OrElse released Then
                Throw New InvalidOperationException("You can't use DataBuffer once it was released")
            End If
            Return s
        End Function

        Private Function numericStorage(ByVal what As String) As ManagedStorage
            Dim s As ManagedStorage = liveStorage()
            If type = DT.UTF8 Then
                Throw New NotSupportedException("Cannot get " & what & " value from buffer of type UTF8")
            End If
            Return s
        End Function

        Private Function writableStorage() As ManagedStorage
            Dim s As ManagedStorage = liveStorage()
            If type = DT.UTF8 Then
                Throw New NotSupportedException("Unsupported data type: UTF8")
            End If
            Return s
        End Function

#End Region

#Region "Element access"

        ''' <inheritdoc/>
        Public Overrides Function getDouble(ByVal i As Long) As Double
            Return numericStorage("double").GetDouble(storageOffsetField + i)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function getLong(ByVal i As Long) As Long
            Return numericStorage("long").GetLong(storageOffsetField + i)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function getFloat(ByVal i As Long) As Single
            Return numericStorage("float").GetFloat(storageOffsetField + i)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function getInt(ByVal ix As Long) As Integer
            Return numericStorage("integer").GetInt(storageOffsetField + ix)
        End Function

        ''' <summary>Element i as text: the string of a UTF8 buffer (Java <c>Utf8Buffer.getString</c>), otherwise the number in invariant culture.</summary>
        Public Overridable Function getString(ByVal index As Long) As String
            Dim s As ManagedStorage = liveStorage()
            If type = DT.UTF8 AndAlso (index < 0 OrElse index >= lengthField) Then
                Throw New ArgumentException("Requested index [" & index & "] is above actual number of words stored: [" & lengthField & "]")
            End If
            Return s.GetString(storageOffsetField + index)
        End Function

        ''' <summary>The number of strings in a UTF8 buffer (Java <c>Utf8Buffer.getNumWords()</c>); the length for other types.</summary>
        Public Overridable Function getNumWords() As Long
            Return lengthField
        End Function

        ''' <inheritdoc/>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Single)
            Dim s As ManagedStorage = liveStorage()
            If type = DT.UTF8 Then
                Throw New InvalidOperationException("Unsupported type: UTF8")
            End If
            s.SetFloat(storageOffsetField + i, element)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Double)
            Dim s As ManagedStorage = writableStorage()
            If type = DT.BOOL Then
                s.SetBoolean(storageOffsetField + i, element > 0.0)
            Else
                s.SetDouble(storageOffsetField + i, element)
            End If
        End Sub

        ''' <inheritdoc/>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Integer)
            writableStorage().SetInt(storageOffsetField + i, element)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Long)
            writableStorage().SetLong(storageOffsetField + i, element)
        End Sub

        ''' <inheritdoc/>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Boolean)
            writableStorage().SetBoolean(storageOffsetField + i, element)
        End Sub

#End Region

#Region "Bulk copies"

        ''' <summary>An exact copy (detached, not constant) of this buffer's elements, of the same type.</summary>
        Public Overrides Function dup() As DataBuffer
            Dim s As ManagedStorage = liveStorage()
            Return New ManagedDataBuffer(s.Clone(storageOffsetField, lengthField))
        End Function

        Private Function copyTo(ByVal destination As Array) As Array
            numericStorage(destination.GetType().GetElementType().Name).CopyToArray(storageOffsetField, destination, 0, lengthField)
            Return destination
        End Function

        ''' <inheritdoc/>
        Public Overrides Function asFloat() As Single()
            checkArrayLength()
            Return DirectCast(copyTo(New Single(CInt(lengthField) - 1) {}), Single())
        End Function

        ''' <inheritdoc/>
        Public Overrides Function asDouble() As Double()
            checkArrayLength()
            Return DirectCast(copyTo(New Double(CInt(lengthField) - 1) {}), Double())
        End Function

        ''' <inheritdoc/>
        Public Overrides Function asInt() As Integer()
            checkArrayLength()
            Return DirectCast(copyTo(New Integer(CInt(lengthField) - 1) {}), Integer())
        End Function

        ''' <inheritdoc/>
        Public Overrides Function asLong() As Long()
            checkArrayLength()
            Return DirectCast(copyTo(New Long(CInt(lengthField) - 1) {}), Long())
        End Function

        ''' <inheritdoc/>
        Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
            Return New ManagedDataBuffer(type, length, True)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function create(ByVal data() As Double) As DataBuffer
            Return createFrom(data)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function create(ByVal data() As Single) As DataBuffer
            Return createFrom(data)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function create(ByVal data() As Integer) As DataBuffer
            Return createFrom(data)
        End Function

        Private Function createFrom(ByVal data As Array) As DataBuffer
            If type = DT.UTF8 Then
                Throw New NotSupportedException()
            End If
            Return New ManagedDataBuffer(type, data)
        End Function

#End Region

#Region "Storage management"

        ''' <summary>True when <paramref name="buffer"/> uses the same storage (a root buffer and its views do).</summary>
        Public Overrides Function sameUnderlyingData(ByVal buffer As DataBuffer) As Boolean
            Dim other As ManagedDataBuffer = TryCast(buffer, ManagedDataBuffer)
            Return other IsNot Nothing AndAlso other.handle Is handle
        End Function

        ''' <summary>The number of elements of the storage (for a view: of the original buffer's storage).</summary>
        Public Overrides Function capacity() As Long
            Return liveStorage().Length
        End Function

        ''' <summary>
        ''' Replaces the storage with one of <paramref name="length"/> elements holding the current elements that fit. The
        ''' views of a root buffer see the new storage; a view that is reallocated gets storage of its own. An attached buffer
        ''' records the new allocation in its workspace and takes the workspace's current generation.
        ''' </summary>
        Public Overrides Function reallocate(ByVal length As Long) As DataBuffer
            Dim old As ManagedStorage = liveStorage()
            Dim ns As ManagedStorage = ManagedStorage.Create(old.StorageType, length)
            old.CopyTo(storageOffsetField, ns, 0, SysMath.Min(lengthField, length))

            If Attached Then
                Dim nws As Nd4vbWorkspace = TryCast(ParentWorkspace, Nd4vbWorkspace)
                If nws IsNot Nothing Then
                    nws.alloc(length * elementSizeField, MemoryKind.HOST, type, False)
                End If
                workspaceGenerationId = ParentWorkspace.GenerationId
            End If

            If wrappedDataBuffer Is Me Then
                handle.Storage = ns
            Else
                handle = New StorageHandle(ns)
            End If
            storageOffsetField = 0
            Me.underlyingLengthField = length
            Me.lengthField = length
            Return Me
        End Function

        ''' <summary>
        ''' Sets the type and allocates storage for the current length when the storage is missing, of another type or too
        ''' short (used when a buffer is read from a stream).
        ''' </summary>
        Public Overrides Sub pointerIndexerByCurrentType(ByVal currentType As DataType)
            type = currentType
            initTypeAndSize()
            Dim st As StorageType = toStorageType(currentType)
            Dim current As ManagedStorage = If(handle Is Nothing, Nothing, handle.Storage)
            If current Is Nothing OrElse current.StorageType <> st OrElse storageOffsetField + lengthField > current.Length Then
                Dim n As Long = If(currentType = DT.UTF8, 0L, lengthField)
                handle = New StorageHandle(ManagedStorage.Create(st, n))
                storageOffsetField = 0
                underlyingLengthField = lengthField
            End If
        End Sub

        ''' <summary>Drops the storage: element access through this buffer and its views throws afterwards.</summary>
        Protected Friend Overrides Sub release()
            If handle IsNot Nothing Then
                handle.Storage = Nothing
            End If
            MyBase.release()
        End Sub

#End Region

#Region "UTF8 layout"

        Private Function utf8Strings() As String()
            Dim s As ManagedStorage = liveStorage()
            Dim result(CInt(lengthField) - 1) As String
            For i As Integer = 0 To result.Length - 1
                result(i) = If(s.GetString(storageOffsetField + i), "")
            Next
            Return result
        End Function

        Private Function utf8LayoutLength() As Long
            Dim total As Long = (lengthField + 1) * 8
            For Each str As String In utf8Strings()
                total += Encoding.UTF8.GetByteCount(str)
            Next
            Return total
        End Function

        ''' <summary>The byte length of Java's UTF8 layout for UTF8 buffers.</summary>
        Protected Friend Overrides Function streamLength() As Long
            If type = DT.UTF8 Then
                Return utf8LayoutLength()
            End If
            Return MyBase.streamLength()
        End Function

        ''' <summary>
        ''' Java's UTF8 buffer layout: <c>numWords + 1</c> little-endian long offsets (the start of each string within the
        ''' data, then the total data length), followed by the UTF-8 bytes of the strings.
        ''' </summary>
        Protected Friend Overrides Function utf8Layout() As Byte()
            If type <> DT.UTF8 Then
                Return MyBase.utf8Layout()
            End If
            Dim strings As String() = utf8Strings()
            Dim encoded As Byte()() = New Byte(strings.Length - 1)() {}
            Dim dataLength As Long = 0
            For i As Integer = 0 To strings.Length - 1
                encoded(i) = Encoding.UTF8.GetBytes(strings(i))
                dataLength += encoded(i).Length
            Next
            Dim headerLength As Integer = (strings.Length + 1) * 8
            Dim result(CInt(headerLength + dataLength) - 1) As Byte
            Dim current As Long = 0
            For i As Integer = 0 To strings.Length
                writeLittleEndian(result, i * 8, current)
                If i < strings.Length Then
                    System.Array.Copy(encoded(i), 0, result, CInt(headerLength + current), encoded(i).Length)
                    current += encoded(i).Length
                End If
            Next
            Return result
        End Function

        ''' <summary>Reads <paramref name="byteLength"/> bytes of Java's UTF8 layout and replaces the storage with the strings.</summary>
        Protected Friend Overrides Sub readUtf8Content(ByVal s As DataInputStream, ByVal byteLength As Long)
            If byteLength > Integer.MaxValue Then
                Throw New ArgumentException("UTF8 buffer of " & byteLength & " bytes is too long")
            End If
            Dim bytes(CInt(byteLength) - 1) As Byte
            s.readFully(bytes)
            Dim strings As String() = parseUtf8Layout(bytes, -1)
            handle = New StorageHandle(ManagedStorage.FromArray(strings, False))
            storageOffsetField = 0
            lengthField = strings.LongLength
            underlyingLengthField = strings.LongLength
        End Sub

        Private Shared Sub writeLittleEndian(ByVal target As Byte(), ByVal index As Integer, ByVal value As Long)
            For b As Integer = 0 To 7
                target(index + b) = CByte((value >> (8 * b)) And &HFFL)
            Next
        End Sub

        Private Shared Function readLittleEndian(ByVal source As Byte(), ByVal index As Long) As Long
            Dim value As Long = 0
            For b As Integer = 7 To 0 Step -1
                value = (value << 8) Or source(CInt(index + b))
            Next
            Return value
        End Function

        ''' <summary>
        ''' Splits Java's UTF8 layout into strings. When <paramref name="numWords"/> is negative it is recovered from the
        ''' layout: the only n with <c>header(n) + (n + 1) * 8 = bytes.Length</c>.
        ''' </summary>
        Friend Shared Function parseUtf8Layout(ByVal bytes As Byte(), ByVal numWords As Long) As String()
            If numWords < 0 Then
                Dim n As Long = 0
                numWords = -1
                Do While (n + 1) * 8 <= bytes.LongLength
                    If readLittleEndian(bytes, n * 8) + (n + 1) * 8 = bytes.LongLength Then
                        numWords = n
                        Exit Do
                    End If
                    n += 1
                Loop
                If numWords < 0 Then
                    Throw New InvalidDataException("Not a UTF8 buffer layout (" & bytes.Length & " bytes)")
                End If
            End If
            Dim headerLength As Long = (numWords + 1) * 8
            If headerLength > bytes.LongLength Then
                Throw New InvalidDataException("UTF8 layout of " & bytes.Length & " bytes is too short for " & numWords & " words")
            End If
            Dim result(CInt(numWords) - 1) As String
            For i As Integer = 0 To result.Length - 1
                Dim start As Long = readLittleEndian(bytes, i * 8L)
                Dim [end] As Long = readLittleEndian(bytes, (i + 1) * 8L)
                If [end] - start > Integer.MaxValue OrElse [end] < start OrElse headerLength + [end] > bytes.LongLength Then
                    Throw New InvalidDataException("Invalid offsets in UTF8 layout for word " & i)
                End If
                result(i) = Encoding.UTF8.GetString(bytes, CInt(headerLength + start), CInt([end] - start))
            Next
            Return result
        End Function

#End Region

    End Class

End Namespace
