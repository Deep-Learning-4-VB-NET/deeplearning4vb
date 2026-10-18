Imports System
Imports System.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.factory
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports RandomT = DeepLearning4VB.org.nd4vb.linalg.api.rng.Random

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.factory

    ''' <summary>
    ''' Tests for <c>RandomFactory</c>: one generator per thread, new seeded instances, and the (seed, size) constructor,
    ''' with a minimal random implementation (the managed generator arrives with the backend).
    ''' </summary>
    <TestClass>
    Public Class RandomFactoryTests

        ''' <summary>A generator that only remembers its seed and how it was constructed.</summary>
        Private Class SeedOnlyRandom
            Implements RandomT

            Private seed As Long
            Public ReadOnly Size As Long = -1
            Public ReadOnly CreatedOnThread As Integer = Thread.CurrentThread.ManagedThreadId

            Public Sub New()
            End Sub

            Public Sub New(ByVal seed As Long, ByVal size As Long)
                Me.seed = seed
                Me.Size = size
            End Sub

            Public Sub setSeed(ByVal seed As Integer) Implements RandomT.setSeed
                Me.seed = seed
            End Sub

            Public Function getSeed() As Long Implements RandomT.getSeed
                Return seed
            End Function

            Public Sub setSeed(ByVal seed() As Integer) Implements RandomT.setSeed
                Me.seed = seed(0)
            End Sub

            Public Sub setSeed(ByVal seed As Long) Implements RandomT.setSeed
                Me.seed = seed
            End Sub

            Public Sub nextBytes(ByVal bytes() As Byte) Implements RandomT.nextBytes
                Throw New NotSupportedException()
            End Sub

            Public Function nextInt() As Integer Implements RandomT.nextInt
                Return 4
            End Function

            Public Function nextInt(ByVal n As Integer) As Integer Implements RandomT.nextInt
                Return 0
            End Function

            Public Function nextInt(ByVal a As Integer, ByVal n As Integer) As Integer Implements RandomT.nextInt
                Return a
            End Function

            Public Function nextLong() As Long Implements RandomT.nextLong
                Return 4
            End Function

            Public Function nextBoolean() As Boolean Implements RandomT.nextBoolean
                Return True
            End Function

            Public Function nextFloat() As Single Implements RandomT.nextFloat
                Return 0.5F
            End Function

            Public Function nextDouble() As Double Implements RandomT.nextDouble
                Return 0.5
            End Function

            Public Function nextGaussian() As Double Implements RandomT.nextGaussian
                Return 0.0
            End Function

            Public Function nextGaussian(ByVal shape() As Integer) As INDArray Implements RandomT.nextGaussian
                Throw New NotSupportedException()
            End Function

            Public Function nextGaussian(ByVal shape() As Long) As INDArray Implements RandomT.nextGaussian
                Throw New NotSupportedException()
            End Function

            Public Function nextGaussian(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements RandomT.nextGaussian
                Throw New NotSupportedException()
            End Function

            Public Function nextGaussian(ByVal order As Char, ByVal shape() As Long) As INDArray Implements RandomT.nextGaussian
                Throw New NotSupportedException()
            End Function

            Public Function nextDouble(ByVal shape() As Integer) As INDArray Implements RandomT.nextDouble
                Throw New NotSupportedException()
            End Function

            Public Function nextDouble(ByVal shape() As Long) As INDArray Implements RandomT.nextDouble
                Throw New NotSupportedException()
            End Function

            Public Function nextDouble(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements RandomT.nextDouble
                Throw New NotSupportedException()
            End Function

            Public Function nextDouble(ByVal order As Char, ByVal shape() As Long) As INDArray Implements RandomT.nextDouble
                Throw New NotSupportedException()
            End Function

            Public Function nextFloat(ByVal shape() As Integer) As INDArray Implements RandomT.nextFloat
                Throw New NotSupportedException()
            End Function

            Public Function nextFloat(ByVal shape() As Long) As INDArray Implements RandomT.nextFloat
                Throw New NotSupportedException()
            End Function

            Public Function nextFloat(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements RandomT.nextFloat
                Throw New NotSupportedException()
            End Function

            Public Function nextFloat(ByVal order As Char, ByVal shape() As Long) As INDArray Implements RandomT.nextFloat
                Throw New NotSupportedException()
            End Function

            Public Function nextInt(ByVal shape() As Integer) As INDArray Implements RandomT.nextInt
                Throw New NotSupportedException()
            End Function

            Public Function nextInt(ByVal shape() As Long) As INDArray Implements RandomT.nextInt
                Throw New NotSupportedException()
            End Function

            Public Function nextInt(ByVal n As Integer, ByVal shape() As Integer) As INDArray Implements RandomT.nextInt
                Throw New NotSupportedException()
            End Function

            Public Function nextInt(ByVal n As Integer, ByVal shape() As Long) As INDArray Implements RandomT.nextInt
                Throw New NotSupportedException()
            End Function

            Public ReadOnly Property Position As Long Implements RandomT.Position
                Get
                    Return 0
                End Get
            End Property

            Public Sub reSeed() Implements RandomT.reSeed
            End Sub

            Public Sub reSeed(ByVal seed As Long) Implements RandomT.reSeed
                Me.seed = seed
            End Sub

            Public Function rootState() As Long Implements RandomT.rootState
                Return seed
            End Function

            Public Function nodeState() As Long Implements RandomT.nodeState
                Return 0
            End Function

            Public Sub setStates(ByVal rootState As Long, ByVal nodeState As Long) Implements RandomT.setStates
                Me.seed = rootState
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class

        ''' <summary>A type without the constructors the factory needs.</summary>
        Private Class NoDefaultConstructorRandom
            Inherits SeedOnlyRandom

            Public Sub New(ByVal unused As String)
            End Sub
        End Class

        <TestMethod>
        Public Sub testOneInstancePerThread()
            Dim factory As New RandomFactory(GetType(SeedOnlyRandom))
            Dim first As RandomT = factory.Random
            Assert.AreSame(first, factory.Random)
            Assert.AreSame(first, factory.getRandom())

            Dim other As RandomT = Nothing
            Dim t As New Thread(Sub() other = factory.getRandom())
            t.Start()
            t.Join()
            assertNotNull(other)
            Assert.AreNotSame(first, other)
            assertEquals(t.ManagedThreadId, DirectCast(other, SeedOnlyRandom).CreatedOnThread)

            ' a second factory has its own per-thread instances
            Assert.AreNotSame(first, New RandomFactory(GetType(SeedOnlyRandom)).Random)
        End Sub

        <TestMethod>
        Public Sub testNewInstances()
            Dim factory As New RandomFactory(GetType(SeedOnlyRandom))
            Dim seeded As RandomT = factory.getNewRandomInstance(42L)
            assertEquals(42L, seeded.getSeed())
            Assert.AreNotSame(seeded, factory.getNewRandomInstance(42L))

            Dim sized As SeedOnlyRandom = DirectCast(factory.getNewRandomInstance(7L, 1000L), SeedOnlyRandom)
            assertEquals(7L, sized.getSeed())
            assertEquals(1000L, sized.Size)

            ' seeded with the current time in milliseconds
            Dim before As Long = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            Dim timed As RandomT = factory.NewRandomInstance
            Dim after As Long = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            ' JavaSystem.currentTimeMillis rounds to the nearest millisecond, so allow one more
            assertTrue(timed.getSeed() >= before AndAlso timed.getSeed() <= after + 1, timed.getSeed().ToString())
        End Sub

        <TestMethod>
        Public Sub testMissingConstructorsFail()
            Dim factory As New RandomFactory(GetType(NoDefaultConstructorRandom))
            Dim ex = assertThrows(Of InvalidOperationException)(Sub() factory.getRandom())
            assertNotNull(ex.InnerException)
            assertThrows(Of InvalidOperationException)(Sub() factory.getNewRandomInstance(1L))
            Dim sizedEx = assertThrows(Of InvalidOperationException)(Sub() factory.getNewRandomInstance(1L, 2L))
            assertTrue(sizedEx.Message.Contains("(Long, Long) constructor"), sizedEx.Message)
        End Sub

    End Class

End Namespace
