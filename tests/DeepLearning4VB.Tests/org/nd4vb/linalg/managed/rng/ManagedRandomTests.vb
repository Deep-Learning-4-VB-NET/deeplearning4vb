Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed.rng
Imports RandomFactory = DeepLearning4VB.org.nd4vb.linalg.factory.RandomFactory
Imports RandomT = DeepLearning4VB.org.nd4vb.linalg.api.rng.Random

Namespace org.nd4vb.linalg.managed.rng

    ''' <summary>Tests for <see cref="ManagedRandom"/>: determinism per seed, value ranges, positions and states.</summary>
    <TestClass>
    Public Class ManagedRandomTests

        Private Shared Function draw(ByVal r As RandomT) As Object()
            Return New Object() {r.nextInt(), r.nextLong(), r.nextDouble(), r.nextFloat(), r.nextBoolean(), r.nextInt(17), r.nextInt(-5, 5), r.nextGaussian()}
        End Function

        <TestMethod>
        Public Sub testDeterministicPerSeed()
            Dim a As New ManagedRandom(42)
            Dim b As New ManagedRandom(42)
            Dim c As New ManagedRandom(43)
            Dim differs As Boolean = False
            For i As Integer = 0 To 99
                Dim da As Object() = draw(a)
                assertEquals(da, draw(b))
                If Not Object.Equals(da(1), draw(c)(1)) Then
                    differs = True
                End If
            Next
            assertTrue(differs)
            assertEquals(42L, a.getSeed())

            ' reseeding restarts the sequence
            Dim fresh As New ManagedRandom(7)
            Dim first As Long = fresh.nextLong()
            fresh.nextLong()
            fresh.setSeed(7L)
            assertEquals(0L, fresh.Position)
            assertEquals(first, fresh.nextLong())
            fresh.reSeed(7)
            assertEquals(first, fresh.nextLong())
            fresh.setSeed(7)
            assertEquals(first, fresh.nextLong())

            Dim bytesA(20) As Byte
            Dim bytesB(20) As Byte
            Call New ManagedRandom(99).nextBytes(bytesA)
            Call New ManagedRandom(99).nextBytes(bytesB)
            assertArrayEquals(bytesA, bytesB)
        End Sub

        <TestMethod>
        Public Sub testValueRanges()
            Dim r As New ManagedRandom(2026)
            Dim sum As Double = 0
            Dim sumSq As Double = 0
            Const n As Integer = 20000
            Dim trues As Integer = 0
            For i As Integer = 1 To n
                Dim d As Double = r.nextDouble()
                assertTrue(d >= 0.0 AndAlso d < 1.0)
                Dim f As Single = r.nextFloat()
                assertTrue(f >= 0.0F AndAlso f < 1.0F)
                Dim k As Integer = r.nextInt(10)
                assertTrue(k >= 0 AndAlso k < 10)
                Dim m As Integer = r.nextInt(-3, 4)
                assertTrue(m >= -3 AndAlso m < 4)
                If r.nextBoolean() Then
                    trues += 1
                End If
                Dim g As Double = r.nextGaussian()
                sum += g
                sumSq += g * g
            Next
            Dim mean As Double = sum / n
            Dim variance As Double = sumSq / n - mean * mean
            assertEquals(0.0, mean, 0.05)
            assertEquals(1.0, variance, 0.05)
            assertEquals(0.5, trues / CDbl(n), 0.02)
            assertThrows(Of ArgumentException)(Sub() r.nextInt(0))
        End Sub

        <TestMethod>
        Public Sub testPositionAndStates()
            Dim r As New ManagedRandom(5)
            assertEquals(0L, r.Position)
            r.nextLong()
            r.nextLong()
            r.nextDouble()
            assertEquals(3L, r.Position)
            assertEquals(5L, r.rootState())
            Dim root As Long = r.rootState()
            Dim node As Long = r.nodeState()
            Dim expected As Long = r.nextLong()
            Dim other As New ManagedRandom(123)
            other.setStates(root, node)
            assertEquals(expected, other.nextLong())
            assertThrows(Of ArgumentOutOfRangeException)(Sub() other.setStates(1, -1))
        End Sub

        <TestMethod>
        Public Sub testConstructorsAndRandomFactory()
            Dim sized As New ManagedRandom(11, 1000000)
            assertEquals(New ManagedRandom(11).nextLong(), sized.nextLong())
            Dim clock1 As New ManagedRandom()
            Dim clock2 As New ManagedRandom()
            assertNotEquals(clock1.getSeed(), clock2.getSeed())
            Assert.AreSame(clock1.Generator, clock1.Generator)

            Dim factory As New RandomFactory(GetType(ManagedRandom))
            Dim perThread As RandomT = factory.getRandom()
            Assert.AreSame(perThread, factory.getRandom())
            Dim seeded As RandomT = factory.getNewRandomInstance(77L)
            assertEquals(77L, seeded.getSeed())
            Dim seededSized As RandomT = factory.getNewRandomInstance(77L, 10L)
            assertEquals(New ManagedRandom(77).nextLong(), seededSized.nextLong())

            ' an int array seeds with the product of its values
            Dim fromArray As New ManagedRandom(1)
            fromArray.setSeed(New Integer() {2, 3, 7})
            assertEquals(42L, fromArray.getSeed())
            assertThrows(Of ArgumentNullException)(Sub() fromArray.setSeed(DirectCast(Nothing, Integer())))
            fromArray.Dispose()
        End Sub

    End Class

End Namespace
