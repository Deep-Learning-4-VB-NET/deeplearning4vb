Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.primitives

Namespace org.nd4vb.common.primitives

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class AtomicTest

        <TestMethod>
        Public Sub testEquality_1()
            Dim v0 = New Atomic(Of Integer)(1327541)
            Dim v1 = New Atomic(Of Integer)(1327541)
            Dim v3 = New Atomic(Of Integer)(1327542)

            assertEquals(v0, v1)
            assertNotEquals(v0, v3)
            assertEquals(v0.GetHashCode(), v1.GetHashCode())
        End Sub

        ''' <summary>The Java test used SerializationUtils; the .NET equivalent for [Serializable] types is SerializationUtils.</summary>
        <TestMethod>
        Public Sub testSerialization_1()
            Dim v0 = New Atomic(Of Integer)(1327541)
            Dim formatter As New ObjectFormatter()
            Using baos As New MemoryStream()
                formatter.Serialize(baos, v0)
                Using bais As New MemoryStream(baos.ToArray())
                    Dim v1 As Atomic(Of Integer) = DirectCast(formatter.Deserialize(bais), Atomic(Of Integer))
                    assertEquals(v1, v0)
                    ' the lock is recreated after deserialization, so the value stays usable
                    v1.[set](42)
                    assertEquals(42, v1.[get]())
                End Using
            End Using
        End Sub

        <TestMethod>
        Public Sub testCas_1()
            Dim v0 = New Atomic(Of String)()
            assertTrue(v0.cas(Nothing, "alpha"))
            assertEquals("alpha", v0.[get]())
        End Sub

        <TestMethod>
        Public Sub testCas_2()
            Dim v0 = New Atomic(Of String)("beta")
            assertFalse(v0.cas(Nothing, "alpha"))
            assertEquals("beta", v0.[get]())
        End Sub

        <TestMethod>
        Public Sub testAtomicBoolean()
            Dim b As New AtomicBoolean()
            assertFalse(b.[get]())
            assertFalse(b.getAndSet(True))
            assertTrue(b.[get]())
            assertFalse(b.compareAndSet(False, False))
            assertTrue(b.compareAndSet(True, False))
            assertFalse(b.Value)
            assertEquals(New AtomicBoolean(False), b)
            assertEquals("false", b.ToString())
        End Sub

        <TestMethod>
        Public Sub testAtomicDouble()
            Dim d As New AtomicDouble(1.5)
            assertEquals(1.5, d.[get](), 0.0)
            assertEquals(3.5, d.addAndGet(2.0), 0.0)
            assertEquals(3.5, d.getAndAdd(1.0), 0.0)
            assertEquals(4.5, d.[get](), 0.0)
            assertTrue(d.compareAndSet(4.5, 10.0))
            assertFalse(d.compareAndSet(4.5, 11.0))
            assertEquals(10.0, d.getAndSet(0.0), 0.0)
            assertEquals(New AtomicDouble(0.0), d)
            assertEquals("0.0", d.ToString())
            assertEquals(7, New AtomicDouble(7.9).intValue())
        End Sub

    End Class

End Namespace
