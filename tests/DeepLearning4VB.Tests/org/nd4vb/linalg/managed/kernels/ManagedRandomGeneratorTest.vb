Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>The managed generator: golden values of the named algorithm, determinism per seed, ranges and rough statistics.</summary>
    <TestClass>
    Public Class ManagedRandomGeneratorTest

        <TestMethod>
        Public Sub GoldenValuesOfXoshiro256StarStarSeededBySplitMix64()
            ' computed with an independent implementation of xoshiro256** (state from four SplitMix64 outputs of the seed)
            Assert.AreEqual("xoshiro256**/splitmix64", ManagedRandomGenerator.AlgorithmName)
            Dim g As New ManagedRandomGenerator(42L)
            Assert.AreEqual(ManagedRandomGenerator.AlgorithmName, g.Algorithm)
            CollectionAssert.AreEqual(New Long() {1546998764402558742L, 6990951692964543102L, -5902157311460992607L, -1389169964527427423L},
                                      New Long() {g.nextLong(), g.nextLong(), g.nextLong(), g.nextLong()})
            g = New ManagedRandomGenerator(-1L)
            CollectionAssert.AreEqual(New Long() {-8118546653352383224L, -4290065566684577747L, -9088772293754075490L, -4655159067405239249L},
                                      New Long() {g.nextLong(), g.nextLong(), g.nextLong(), g.nextLong()})
            g = New ManagedRandomGenerator(0L)
            CollectionAssert.AreEqual(New Long() {-7355399402456485196L, -4652746763540216534L, 1900383378846508768L, 7684712102626143532L},
                                      New Long() {g.nextLong(), g.nextLong(), g.nextLong(), g.nextLong()})
            ' doubles: the top 53 bits; ints: the top 32 bits
            g = New ManagedRandomGenerator(42L)
            Assert.AreEqual(0.08386297105988216, g.nextDouble(), 0.0)
            Assert.AreEqual(0.3789802506626686, g.nextDouble(), 0.0)
            Assert.AreEqual(0.6800434110281394, g.nextDouble(), 0.0)
            g = New ManagedRandomGenerator(-1L)
            CollectionAssert.AreEqual(New Integer() {-1890246443, -998858728, -2116144704}, New Integer() {g.nextInt(), g.nextInt(), g.nextInt()})
            g = New ManagedRandomGenerator(42L)
            CollectionAssert.AreEqual(New Integer() {360188718, 1627707782, -1374203086}, New Integer() {g.nextInt(), g.nextInt(), g.nextInt()})
        End Sub

        ''' <summary>A mixed sequence of every kind of draw.</summary>
        Private Shared Function Draw(ByVal g As ManagedRandomGenerator, ByVal count As Integer) As Double()
            Dim r As Double() = New Double(count - 1) {}
            For i As Integer = 0 To count - 1
                Select Case i Mod 8
                    Case 0
                        r(i) = g.nextLong()
                    Case 1
                        r(i) = g.nextInt()
                    Case 2
                        r(i) = g.nextInt(1000)
                    Case 3
                        r(i) = g.nextDouble()
                    Case 4
                        r(i) = g.nextFloat()
                    Case 5
                        r(i) = g.nextGaussian()
                    Case 6
                        r(i) = If(g.nextBoolean(), 1.0, 0.0)
                    Case Else
                        r(i) = g.nextInt(-50, 50)
                End Select
            Next
            Return r
        End Function

        <TestMethod>
        Public Sub SameSeedSameSequence()
            Dim a As New ManagedRandomGenerator(123456789L)
            Dim b As New ManagedRandomGenerator(123456789L)
            CollectionAssert.AreEqual(Draw(a, 1000), Draw(b, 1000))
            Assert.AreEqual(a.getPosition(), b.getPosition())
            ' setSeed restarts the sequence (and drops a cached Gaussian)
            a.setSeed(123456789L)
            Dim fresh As New ManagedRandomGenerator(123456789L)
            CollectionAssert.AreEqual(Draw(fresh, 999), Draw(a, 999))
            Assert.AreEqual(123456789L, a.getSeed())
            ' an int seed is the sign-extended long seed
            Dim i1 As New ManagedRandomGenerator(0L)
            i1.setSeed(-5)
            Dim l1 As New ManagedRandomGenerator(-5L)
            CollectionAssert.AreEqual(Draw(l1, 50), Draw(i1, 50))
            ' different seeds give different sequences
            CollectionAssert.AreNotEqual(Draw(New ManagedRandomGenerator(1L), 20), Draw(New ManagedRandomGenerator(2L), 20))
            ' unseeded generators differ from each other
            Assert.AreNotEqual(New ManagedRandomGenerator().nextLong(), New ManagedRandomGenerator().nextLong())
        End Sub

        <TestMethod>
        Public Sub PositionCopyAndJump()
            Dim g As New ManagedRandomGenerator(7L)
            Assert.AreEqual(0L, g.getPosition())
            g.nextLong()
            g.nextDouble()
            g.nextInt(10)
            Assert.AreEqual(3L, g.getPosition())
            ' a copy continues exactly where the original is, including a cached Gaussian
            g.nextGaussian()
            Dim c As ManagedRandomGenerator = g.Copy()
            Assert.AreEqual(g.getPosition(), c.getPosition())
            CollectionAssert.AreEqual(Draw(g, 200), Draw(c, 200))
            ' jump: deterministic and different from the un-jumped stream
            Dim j1 As New ManagedRandomGenerator(7L)
            Dim j2 As New ManagedRandomGenerator(7L)
            j1.jump()
            j2.jump()
            Dim plain As New ManagedRandomGenerator(7L)
            Dim jumped As Double() = Draw(j1, 100)
            CollectionAssert.AreEqual(jumped, Draw(j2, 100))
            CollectionAssert.AreNotEqual(Draw(plain, 100), jumped)
        End Sub

        <TestMethod>
        Public Sub RangesAndArguments()
            Dim g As New ManagedRandomGenerator(99L)
            For i As Integer = 0 To 9999
                Dim d As Double = g.nextDouble()
                Assert.IsTrue(d >= 0.0 AndAlso d < 1.0)
                Dim f As Single = g.nextFloat()
                Assert.IsTrue(f >= 0.0F AndAlso f < 1.0F)
                Dim b As Integer = g.nextInt(7)
                Assert.IsTrue(b >= 0 AndAlso b < 7)
                Assert.AreEqual(0, g.nextInt(1))
                Dim big As Integer = g.nextInt(Integer.MaxValue)
                Assert.IsTrue(big >= 0 AndAlso big < Integer.MaxValue)
                Dim r As Integer = g.nextInt(-3, 4)
                Assert.IsTrue(r >= -3 AndAlso r < 4)
            Next
            ' an origin/bound range wider than 2^31 still covers both signs
            Dim negatives As Integer = 0
            For i As Integer = 0 To 999
                If g.nextInt(Integer.MinValue, Integer.MaxValue) < 0 Then
                    negatives += 1
                End If
            Next
            Assert.IsTrue(negatives > 400 AndAlso negatives < 600, "negatives " & negatives)
            Assert.ThrowsException(Of ArgumentException)(Function() g.nextInt(0))
            Assert.ThrowsException(Of ArgumentException)(Function() g.nextInt(-4))
            Assert.ThrowsException(Of ArgumentException)(Function() g.nextInt(5, 5))
            Assert.ThrowsException(Of ArgumentNullException)(Sub() g.nextBytes(Nothing))
        End Sub

        <TestMethod>
        Public Sub NextBytesUsesEightBytesPerOutput()
            Dim a As New ManagedRandomGenerator(3L)
            Dim bytes As Byte() = New Byte(10) {}
            a.nextBytes(bytes)
            Assert.AreEqual(2L, a.getPosition(), "11 bytes take two outputs")
            Dim b As New ManagedRandomGenerator(3L)
            Dim first As Long = b.nextLong()
            For k As Integer = 0 To 7
                Assert.AreEqual(CByte((first >> (8 * k)) And &HFFL), bytes(k), "byte " & k)
            Next
            Dim again As Byte() = New Byte(10) {}
            Dim c As New ManagedRandomGenerator(3L)
            c.nextBytes(again)
            CollectionAssert.AreEqual(bytes, again)
        End Sub

        <TestMethod>
        Public Sub RoughStatistics()
            Dim g As New ManagedRandomGenerator(2026L)
            Const n As Integer = 200000
            Dim sum As Double = 0, gSum As Double = 0, gSq As Double = 0
            Dim buckets As Integer() = New Integer(9) {}
            Dim trues As Integer = 0
            For i As Integer = 0 To n - 1
                sum += g.nextDouble()
                Dim v As Double = g.nextGaussian()
                gSum += v
                gSq += v * v
                buckets(g.nextInt(10)) += 1
                If g.nextBoolean() Then
                    trues += 1
                End If
            Next
            Assert.AreEqual(0.5, sum / n, 0.005, "uniform mean")
            Assert.AreEqual(0.0, gSum / n, 0.01, "Gaussian mean")
            Assert.AreEqual(1.0, gSq / n - (gSum / n) * (gSum / n), 0.02, "Gaussian variance")
            For Each count As Integer In buckets
                Assert.AreEqual(n / 10.0, count, n * 0.006, "nextInt(10) bucket")
            Next
            Assert.AreEqual(0.5, trues / CDbl(n), 0.005, "booleans")
        End Sub

    End Class

End Namespace
