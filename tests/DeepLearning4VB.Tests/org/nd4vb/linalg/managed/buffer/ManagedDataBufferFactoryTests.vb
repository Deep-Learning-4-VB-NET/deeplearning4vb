Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.buffer
Imports DataBufferFactory = DeepLearning4VB.org.nd4vb.linalg.api.buffer.factory.DataBufferFactory
Imports LearningPolicy = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.LearningPolicy
Imports ManagedMemoryManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedMemoryManager
Imports ManagedWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedWorkspaceManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration

Namespace org.nd4vb.linalg.managed.buffer

    ''' <summary>Tests for the create overloads of <see cref="ManagedDataBufferFactory"/> (Java <c>DefaultDataBufferFactory</c>).</summary>
    <TestClass>
    Public Class ManagedDataBufferFactoryTests

        Private ReadOnly factory As DataBufferFactory = New ManagedDataBufferFactory()

        <TestMethod>
        Public Sub testLengthOverloadsGiveTheRightTypes()
            assertEquals(DataType.DOUBLE, factory.createDouble(3).dataType())
            assertEquals(DataType.FLOAT, factory.createFloat(3, False).dataType())
            assertEquals(DataType.INT, factory.createInt(3).dataType())
            assertEquals(DataType.LONG, factory.createLong(3, True).dataType())
            assertEquals(DataType.HALF, factory.createHalf(3).dataType())
            assertEquals(DataType.BFLOAT16, factory.createBFloat16(3).dataType())
            assertEquals(DataType.BYTE, factory.createByte(3).dataType())
            assertEquals(DataType.SHORT, factory.createShort(3, True).dataType())
            assertEquals(DataType.BOOL, factory.createBool(3).dataType())
            assertEquals(DataType.UINT16, factory.createUShort(3).dataType())
            assertEquals(DataType.UINT32, factory.createUInt(3).dataType())
            assertEquals(DataType.UBYTE, factory.createUByte(3, False).dataType())
            assertEquals(DataType.UINT64, factory.createULong(3).dataType())
            Dim b As DataBuffer = factory.createDouble(4)
            assertEquals(4L, b.length())
            assertArrayEquals(New Double() {0, 0, 0, 0}, b.asDouble(), 0.0)
            For Each t As DataType In {DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.LONG, DataType.INT, DataType.SHORT, DataType.UBYTE, DataType.BYTE, DataType.BOOL, DataType.UTF8, DataType.BFLOAT16, DataType.UINT16, DataType.UINT32, DataType.UINT64}
                Dim c As DataBuffer = factory.create(t, 2, True)
                assertEquals(t, c.dataType())
                assertEquals(2L, c.length())
                Dim same As DataBuffer = factory.createSame(c, False)
                assertEquals(t, same.dataType())
                assertEquals(2L, same.length())
            Next
            assertThrows(Of InvalidOperationException)(Sub() factory.create(DataType.COMPRESSED, 2, True))
            assertThrows(Of InvalidOperationException)(Sub() factory.create(DataType.UNKNOWN, 2, True))
        End Sub

        <TestMethod>
        Public Sub testArrayOverloadsConvertWithJavaCasts()
            assertArrayEquals(New Integer() {1, -2, 3}, factory.createInt(New Double() {1.9, -2.9, 3.0}).asInt())
            assertArrayEquals(New Integer() {Integer.MaxValue}, factory.createInt(New Single() {1.0E+20F}, True).asInt())
            assertArrayEquals(New Single() {1, 2}, factory.createFloat(New Integer() {1, 2}).asFloat(), 0.0F)
            assertArrayEquals(New Double() {1.5}, factory.createDouble(New Single() {1.5F}).asDouble(), 0.0)
            assertArrayEquals(New Long() {5, -6}, factory.createLong(New Integer() {5, -6}, True).asLong())
            assertArrayEquals(New Long() {Long.MaxValue}, factory.createLong(New Long() {Long.MaxValue}).asLong())
            assertArrayEquals(New Double() {7, 8}, factory.createDouble(New Long() {7, 8}, True).asDouble(), 0.0)
            assertArrayEquals(New Single() {9}, factory.createFloat(New Long() {9}, False).asFloat(), 0.0F)
            assertArrayEquals(New Integer() {10}, factory.createInt(New Long() {10}, True).asInt())
            ' HALF from arrays works (the CPU factory threw)
            assertArrayEquals(New Single() {0.5F, 2.0F}, factory.createHalf(New Double() {0.5, 2.0}).asFloat(), 0.0F)
            assertEquals(DataType.HALF, factory.createHalf(New Integer() {1}, True).dataType())
            ' offset overloads record the offset (Java's (data, copy, offset) constructors)
            Dim withOffset As DataBuffer = factory.createFloat(1, New Double() {1, 2, 3})
            assertEquals(1L, withOffset.offset())
            assertEquals(2L, withOffset.length())
            assertEquals(3L, withOffset.underlyingLength())
            assertEquals(DataType.INT, factory.createInt(2, New Integer() {1, 2, 3, 4}, False).dataType())
            assertEquals(2L, factory.createHalf(1, New Single() {1, 2, 3}).length())
        End Sub

        <TestMethod>
        Public Sub testViewAndByteOverloads()
            Dim root As DataBuffer = factory.createInt(New Integer() {1, 2, 3, 4, 5})
            Dim view As DataBuffer = factory.create(root, 1, 3)
            assertArrayEquals(New Integer() {2, 3, 4}, view.asInt())
            Assert.AreSame(root, view.underlyingDataBuffer())

            ' raw little-endian elements from element 1
            Dim raw As Byte() = {1, 0, 0, 0, 2, 0, 0, 0, &HFF, &HFF, &HFF, &HFF}
            assertArrayEquals(New Integer() {2, -1}, factory.create(raw, DataType.INT, 2, 1).asInt())
            assertArrayEquals(New Long() {2, 4294967295L}, factory.create(raw, DataType.UINT32, 2, 1).asLong())
            assertArrayEquals(New Integer() {1, 0, 2, 0}, factory.create(raw, DataType.SHORT, 4, 0).asInt())
            assertArrayEquals(New Integer() {-1, -1}, factory.create(raw, DataType.SHORT, 2, 4).asInt())
            assertArrayEquals(New Integer() {1, 0}, factory.create(raw, DataType.BOOL, 2, 0).asInt())
            Dim floatBytes As Byte() = BitConverter.GetBytes(1.5F)
            assertArrayEquals(New Single() {1.5F}, factory.create(floatBytes, DataType.FLOAT, 1, 0).asFloat(), 0.0F)
            assertThrows(Of NotSupportedException)(Sub() factory.create(raw, DataType.UTF8, 1, 0))

            ' big-endian doubles/floats (Java ArrayUtil.toDoubleArray/toFloatArray); the length argument is ignored
            Dim be As Byte() = {&H3F, &HF8, 0, 0, 0, 0, 0, 0}
            Dim d As DataBuffer = factory.createDouble(0, be, 99)
            assertArrayEquals(New Double() {1.5}, d.asDouble(), 0.0)
            Dim f As DataBuffer = factory.createFloat(0, New Byte() {&H3F, &HC0, 0, 0}, 1)
            assertArrayEquals(New Single() {1.5F}, f.asFloat(), 0.0F)
        End Sub

        <TestMethod>
        Public Sub testWorkspaceOverloadsAttach()
            Dim mm As New ManagedMemoryManager()
            Dim wm As New ManagedWorkspaceManager(mm, Nothing)
            Dim config As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(4096).policyLearning(LearningPolicy.NONE).build()
            Using ws As MemoryWorkspace = wm.getAndActivateWorkspace(config, "FACTORY_WS")
                Dim buffers As DataBuffer() = {
                    factory.createDouble(4, True, ws), factory.createFloat(4, True, ws), factory.createInt(4, True, ws),
                    factory.createLong(4, True, ws), factory.createHalf(4, True, ws), factory.createBool(4, True, ws),
                    factory.create(DataType.UINT16, 4, True, ws), factory.createDouble(New Double() {1, 2}, ws),
                    factory.createFloat(New Single() {1}, True, ws), factory.createInt(New Integer() {1}, ws),
                    factory.createLong(New Long() {1}, ws), factory.createDouble(1, New Double() {1, 2}, ws),
                    factory.createSame(factory.createInt(3), True, ws)}
                For Each b As DataBuffer In buffers
                    assertTrue(b.Attached)
                    Assert.AreSame(ws, b.ParentWorkspace)
                Next
                assertFalse(factory.createDouble(4, True, Nothing).Attached)
            End Using
        End Sub

        <TestMethod>
        Public Sub testMiscellaneous()
            Dim f As New ManagedDataBufferFactory()
            assertEquals(DataBuffer.AllocationMode.MIXED_DATA_TYPES, f.allocationMode())
            f.setAllocationMode(DataBuffer.AllocationMode.HEAP)
            assertEquals(DataBuffer.AllocationMode.HEAP, f.allocationMode())
            assertEquals(GetType(ManagedDataBuffer), f.intBufferClass())
            assertEquals(GetType(ManagedDataBuffer), f.longBufferClass())
            assertEquals(GetType(ManagedDataBuffer), f.halfBufferClass())
            assertEquals(GetType(ManagedDataBuffer), f.floatBufferClass())
            assertEquals(GetType(ManagedDataBuffer), f.doubleBufferClass())
            Dim recorded As DataBuffer = f.createInt(2, 5)
            assertEquals(2L, recorded.offset())
            assertEquals(3L, recorded.length())
            assertEquals(5L, recorded.underlyingLength())
        End Sub

    End Class

End Namespace
