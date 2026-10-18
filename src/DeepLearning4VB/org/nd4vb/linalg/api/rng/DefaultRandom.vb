Imports System
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.rng

    ''' <summary>
    ''' The default random number generator: a synchronized Mersenne Twister (Java <c>DefaultRandom</c> over Commons Math's
    ''' <c>SynchronizedRandomGenerator(new MersenneTwister(seed))</c>), so a seed gives Java's sequence of ints, longs,
    ''' floats and doubles. The array methods fill a new array (<c>Nd4vb.create</c>) element by element.
    ''' </summary>
    ''' <remarks>
    ''' The Commons Math classes are replaced by <see cref="RandomGenerator"/> and <see cref="MersenneTwister"/> in this
    ''' namespace; access is synchronized on this object. Native-only members not ported: <c>getStatePointer()</c>.
    ''' </remarks>
    Public Class DefaultRandom
        Implements Random, RandomGenerator

        Protected Friend randomGeneratorField As RandomGenerator
        Protected Friend seedField As Long

        ''' <summary>Initialize with a System.currentTimeMillis() seed.</summary>
        Public Sub New()
            Me.New(JavaSystem.currentTimeMillis())
        End Sub

        ''' <summary>A generator with the given seed.</summary>
        Public Sub New(ByVal seed As Long)
            Me.seedField = seed
            Me.randomGeneratorField = New MersenneTwister(seed)
        End Sub

        ''' <summary>A generator over another source of random numbers.</summary>
        Public Sub New(ByVal randomGenerator As RandomGenerator)
            If randomGenerator Is Nothing Then
                Throw New ArgumentNullException(NameOf(randomGenerator))
            End If
            Me.randomGeneratorField = randomGenerator
        End Sub

        ''' <summary>The underlying source of random numbers (Java <c>getRandomGenerator()</c>).</summary>
        Public Overridable Function getRandomGenerator() As RandomGenerator
            SyncLock Me
                Return randomGeneratorField
            End SyncLock
        End Function

        ''' <summary>Reseeds with an int seed.</summary>
        Public Overridable Sub setSeed(ByVal seed As Integer) Implements Random.setSeed, RandomGenerator.setSeed
            SyncLock Me
                Me.seedField = seed
                randomGeneratorField.setSeed(seed)
            End SyncLock
        End Sub

        ''' <summary>The seed last set.</summary>
        Public Overridable Function getSeed() As Long Implements Random.getSeed
            SyncLock Me
                Return Me.seedField
            End SyncLock
        End Function

        ''' <summary>Not supported, as in Java.</summary>
        Public Overridable Sub setSeed(ByVal seed() As Integer) Implements Random.setSeed, RandomGenerator.setSeed
            Throw New NotSupportedException()
        End Sub

        ''' <summary>Reseeds with a long seed.</summary>
        Public Overridable Sub setSeed(ByVal seed As Long) Implements Random.setSeed, RandomGenerator.setSeed
            SyncLock Me
                Me.seedField = seed
                randomGeneratorField.setSeed(seed)
            End SyncLock
        End Sub

        ''' <summary>Fills the array with random bytes.</summary>
        Public Overridable Sub nextBytes(ByVal bytes() As Byte) Implements Random.nextBytes, RandomGenerator.nextBytes
            SyncLock Me
                randomGeneratorField.nextBytes(bytes)
            End SyncLock
        End Sub

        ''' <summary>A uniformly distributed int.</summary>
        Public Overridable Function nextInt() As Integer Implements Random.nextInt, RandomGenerator.nextInt
            SyncLock Me
                Return randomGeneratorField.nextInt()
            End SyncLock
        End Function

        ''' <summary>A uniformly distributed int in [0, n).</summary>
        Public Overridable Function nextInt(ByVal n As Integer) As Integer Implements Random.nextInt, RandomGenerator.nextInt
            SyncLock Me
                Return randomGeneratorField.nextInt(n)
            End SyncLock
        End Function

        ''' <summary>A uniformly distributed int in [a, n): <c>nextInt(n - a) + a</c>.</summary>
        Public Overridable Function nextInt(ByVal a As Integer, ByVal n As Integer) As Integer Implements Random.nextInt
            Return nextInt(n - a) + a
        End Function

        ''' <summary>A uniformly distributed long.</summary>
        Public Overridable Function nextLong() As Long Implements Random.nextLong, RandomGenerator.nextLong
            SyncLock Me
                Return randomGeneratorField.nextLong()
            End SyncLock
        End Function

        ''' <summary>A uniformly distributed boolean.</summary>
        Public Overridable Function nextBoolean() As Boolean Implements Random.nextBoolean, RandomGenerator.nextBoolean
            SyncLock Me
                Return randomGeneratorField.nextBoolean()
            End SyncLock
        End Function

        ''' <summary>A uniformly distributed float in [0, 1).</summary>
        Public Overridable Function nextFloat() As Single Implements Random.nextFloat, RandomGenerator.nextFloat
            SyncLock Me
                Return randomGeneratorField.nextFloat()
            End SyncLock
        End Function

        ''' <summary>A uniformly distributed double in [0, 1).</summary>
        Public Overridable Function nextDouble() As Double Implements Random.nextDouble, RandomGenerator.nextDouble
            SyncLock Me
                Return randomGeneratorField.nextDouble()
            End SyncLock
        End Function

        ''' <summary>A standard normal value.</summary>
        Public Overridable Function nextGaussian() As Double Implements Random.nextGaussian, RandomGenerator.nextGaussian
            SyncLock Me
                Return randomGeneratorField.nextGaussian()
            End SyncLock
        End Function

        ''' <summary>A new array of standard normal values in the default order.</summary>
        Public Overridable Function nextGaussian(ByVal shape() As Long) As INDArray Implements Random.nextGaussian
            Return nextGaussian(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of standard normal values in the default order.</summary>
        Public Overridable Function nextGaussian(ByVal shape() As Integer) As INDArray Implements Random.nextGaussian
            Return nextGaussian(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of standard normal values.</summary>
        Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextGaussian
            Return nextGaussian(order, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of standard normal values.</summary>
        Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextGaussian
            Dim length As Long = ArrayUtil.prodLong(shape)
            Dim ret As INDArray = Nd4vbT.create(shape, order)
            Dim data As DataBuffer = ret.data()
            For i As Long = 0 To length - 1
                data.put(i, nextGaussian())
            Next i
            Return ret
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1) in the default order.</summary>
        Public Overridable Function nextDouble(ByVal shape() As Long) As INDArray Implements Random.nextDouble
            Return nextDouble(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1) in the default order.</summary>
        Public Overridable Function nextDouble(ByVal shape() As Integer) As INDArray Implements Random.nextDouble
            Return nextDouble(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1).</summary>
        Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextDouble
            Return nextDouble(order, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1).</summary>
        Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextDouble
            Dim length As Long = ArrayUtil.prodLong(shape)
            Dim ret As INDArray = Nd4vbT.create(shape, order)
            Dim data As DataBuffer = ret.data()
            For i As Long = 0 To length - 1
                data.put(i, nextDouble())
            Next i
            Return ret
        End Function

        ''' <summary>A new array of uniform floats in [0, 1) in the default order.</summary>
        Public Overridable Function nextFloat(ByVal shape() As Long) As INDArray Implements Random.nextFloat
            Return nextFloat(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform floats in [0, 1) in the default order.</summary>
        Public Overridable Function nextFloat(ByVal shape() As Integer) As INDArray Implements Random.nextFloat
            Return nextFloat(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform floats in [0, 1).</summary>
        Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextFloat
            Return nextFloat(order, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform floats in [0, 1).</summary>
        Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextFloat
            Dim length As Long = ArrayUtil.prodLong(shape)
            Dim ret As INDArray = Nd4vbT.create(shape, order)
            Dim data As DataBuffer = ret.data()
            For i As Long = 0 To length - 1
                data.put(i, nextFloat())
            Next i
            Return ret
        End Function

        ''' <summary>A new array of uniform ints.</summary>
        Public Overridable Function nextInt(ByVal shape() As Integer) As INDArray Implements Random.nextInt
            Return nextInt(ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform ints.</summary>
        Public Overridable Function nextInt(ByVal shape() As Long) As INDArray Implements Random.nextInt
            Dim length As Long = ArrayUtil.prodLong(shape)
            Dim ret As INDArray = Nd4vbT.create(shape)
            Dim data As DataBuffer = ret.data()
            For i As Long = 0 To length - 1
                data.put(i, nextInt())
            Next i
            Return ret
        End Function

        ''' <summary>A new array of uniform ints in [0, n).</summary>
        Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Integer) As INDArray Implements Random.nextInt
            Return nextInt(n, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform ints in [0, n).</summary>
        Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Long) As INDArray Implements Random.nextInt
            Dim length As Long = ArrayUtil.prodLong(shape)
            Dim ret As INDArray = Nd4vbT.create(shape)
            Dim data As DataBuffer = ret.data()
            For i As Long = 0 To length - 1
                data.put(i, nextInt(n))
            Next i
            Return ret
        End Function

        ''' <summary>Always 0: the Mersenne Twister has no position counter.</summary>
        Public Overridable ReadOnly Property Position As Long Implements Random.Position
            Get
                Return 0
            End Get
        End Property

        ''' <summary>Nothing to release.</summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
        End Sub

        ''' <summary>Reseeds from the clock.</summary>
        Public Overridable Sub reSeed() Implements Random.reSeed
            reSeed(JavaSystem.currentTimeMillis())
        End Sub

        ''' <summary>Reseeds with the given seed.</summary>
        Public Overridable Sub reSeed(ByVal seed As Long) Implements Random.reSeed
            setSeed(seed)
        End Sub

        ''' <summary>Always 0 (no native state).</summary>
        Public Overridable Function rootState() As Long Implements Random.rootState
            Return 0L
        End Function

        ''' <summary>Always 0 (no native state).</summary>
        Public Overridable Function nodeState() As Long Implements Random.nodeState
            Return 0L
        End Function

        ''' <summary>Does nothing (no native state).</summary>
        Public Overridable Sub setStates(ByVal rootState As Long, ByVal nodeState As Long) Implements Random.setStates
            'no-op
        End Sub

    End Class

End Namespace
