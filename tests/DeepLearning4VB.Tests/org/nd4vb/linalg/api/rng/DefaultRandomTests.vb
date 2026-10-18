Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.rng

Namespace org.nd4vb.linalg.api.rng

    ''' <summary>
    ''' Tests for <see cref="MersenneTwister"/> (the Commons Math stand-in) against the MT19937 reference outputs, and for
    ''' <see cref="DefaultRandom"/>.
    ''' </summary>
    <TestClass>
    Public Class DefaultRandomTests

        <TestMethod>
        Public Sub testInitGenrandReferenceOutputs()
            ' MT19937 init_genrand(5489): 3499211612, 581869302, 3890346734, 3586334585, 545404204 (as Java ints)
            Dim mt As New MersenneTwister(5489)
            assertEquals(-795755684, mt.nextInt())
            assertEquals(581869302, mt.nextInt())
            assertEquals(-404620562, mt.nextInt())
            assertEquals(-708632711, mt.nextInt())
            assertEquals(545404204, mt.nextInt())
        End Sub

        <TestMethod>
        Public Sub testInitByArrayReferenceOutputs()
            ' mt19937ar.c: init_by_array({0x123, 0x234, 0x345, 0x456}) gives 1067595299 955945823 477289528 4107218783 4228976476
            Dim mt As New MersenneTwister(New Integer() {&H123, &H234, &H345, &H456})
            assertEquals(1067595299, mt.nextInt())
            assertEquals(955945823, mt.nextInt())
            assertEquals(477289528, mt.nextInt())
            assertEquals(-187748513, mt.nextInt())
            assertEquals(-65990820, mt.nextInt())
        End Sub

        <TestMethod>
        Public Sub testDerivedValues()
            ' nextLong combines two ints; nextBoolean is the top bit; nextInt(power of two) the top bits
            Dim a As New MersenneTwister(5489)
            assertEquals((CLng(-795755684) << 32) Or (581869302L And &HFFFFFFFFL), a.nextLong())
            Dim b As New MersenneTwister(5489)
            assertTrue(b.nextBoolean()) ' 3499211612 has its top bit set
            Dim c As New MersenneTwister(5489)
            assertEquals(CInt((3499211612L >> 1) * 16L >> 31), c.nextInt(16))
            Dim d As New MersenneTwister(5489)
            Dim expected As Double = ((CLng(3499211612L >> 6) << 26) Or (581869302L >> 6)) * Math.Pow(2, -52)
            assertEquals(expected, d.nextDouble(), 0.0)
            Dim e As New MersenneTwister(5489)
            assertEquals(CSng((3499211612L >> 9) * Math.Pow(2, -23)), e.nextFloat(), 0.0F)
            ' a long seed is init_by_array of its two words
            Dim f As New MersenneTwister(&H123L << 32 Or &H234L)
            Dim g As New MersenneTwister(New Integer() {&H123, &H234})
            For i As Integer = 1 To 10
                assertEquals(g.nextInt(), f.nextInt())
            Next
            Dim bytes(6) As Byte
            Call New MersenneTwister(5489).nextBytes(bytes)
            ' 3499211612 = 0xD091BB5C, low byte first; then 581869302 = 0x22AE9EF6
            assertArrayEquals(New Byte() {&H5C, &HBB, &H91, &HD0, &HF6, &H9E, &HAE}, bytes)
            assertThrows(Of ArgumentException)(Sub() Call New MersenneTwister(1).nextInt(0))
        End Sub

        <TestMethod>
        Public Sub testDefaultRandom()
            Dim r As New DefaultRandom(1234L)
            Dim s As New DefaultRandom(1234L)
            assertEquals(1234L, r.getSeed())
            For i As Integer = 1 To 50
                assertEquals(s.nextLong(), r.nextLong())
                assertEquals(s.nextDouble(), r.nextDouble(), 0.0)
                assertEquals(s.nextGaussian(), r.nextGaussian(), 0.0)
            Next
            Dim viaGenerator As New DefaultRandom(New MersenneTwister(5489))
            assertEquals(-795755684, viaGenerator.nextInt())
            r.setSeed(5489)
            assertEquals(5489L, r.getSeed())
            assertEquals(-795755684, r.nextInt())
            r.reSeed(1234L)
            Dim again As New DefaultRandom(1234L)
            assertEquals(again.nextInt(), r.nextInt())
            For i As Integer = 1 To 1000
                Dim k As Integer = r.nextInt(3, 9)
                assertTrue(k >= 3 AndAlso k < 9)
                Dim f As Single = r.nextFloat()
                assertTrue(f >= 0 AndAlso f < 1)
            Next
            assertThrows(Of NotSupportedException)(Sub() r.setSeed(New Integer() {1}))
            assertEquals(0L, r.Position)
            assertEquals(0L, r.rootState())
            assertEquals(0L, r.nodeState())
            r.setStates(1, 2)
            assertTrue(TypeOf r.getRandomGenerator() Is MersenneTwister)
            r.Dispose()
        End Sub

    End Class

End Namespace
