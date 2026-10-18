' DeepLearning4VB: the managed backend's random number generator (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native-api org.nd4j.rng.NativeRandom and nd4j-native CpuNativeRandom.

Imports System
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ManagedRandomGenerator = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ManagedRandomGenerator
Imports RandomT = DeepLearning4VB.org.nd4vb.linalg.api.rng.Random
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

Namespace org.nd4vb.linalg.managed.rng

    ''' <summary>
    ''' The random number generator of the managed backend (Java <c>CpuNativeRandom</c>/<c>NativeRandom</c>, which drove
    ''' libnd4j's generator): a <see cref="ManagedRandomGenerator"/> (xoshiro256** seeded by SplitMix64) with a 64-bit seed.
    ''' Sequences are deterministic per seed and the same on every platform, but they are not libnd4j's sequences.
    ''' </summary>
    ''' <remarks>
    ''' <para><see cref="rootState"/> is the seed and <see cref="nodeState"/> the number of 64-bit outputs drawn since;
    ''' <c>setStates(rootState(), nodeState())</c> restores the sequence position (a cached Gaussian is dropped).
    ''' The array methods fill a new array (<c>Nd4vb.createUninitialized</c>, the default data type) element by element; the
    ''' random-distribution ops of milestone M2 will take over.</para>
    ''' <para>Differences from Java: <see cref="nextBytes"/>, <c>nextInt(shape)</c> and <c>nextInt(n, shape)</c> work (Java
    ''' threw <c>UnsupportedOperationException</c>); <c>setSeed(Integer())</c> seeds with the product of the values (Java
    ''' started the product at 0, so every array gave the seed 0); <c>nextInt(n)</c>, <c>nextBoolean()</c> and
    ''' <c>nextGaussian()</c> use the generator's unbiased bound, top bit and polar method. Native-only members not ported:
    ''' <c>getStatePointer()</c>, <c>getExtraPointers()</c>, <c>init()</c>.</para>
    ''' <para>Instances are not thread-safe (as in Java); <c>RandomFactory</c> gives each thread its own.</para>
    ''' </remarks>
    Public Class ManagedRandom
        Implements RandomT

        Private ReadOnly generatorField As ManagedRandomGenerator
        Private seedField As Long

        ''' <summary>A generator seeded from the clock and a process-wide counter.</summary>
        Public Sub New()
            generatorField = New ManagedRandomGenerator()
            seedField = generatorField.getSeed()
        End Sub

        ''' <summary>A generator with the given seed.</summary>
        Public Sub New(ByVal seed As Long)
            generatorField = New ManagedRandomGenerator(seed)
            seedField = seed
        End Sub

        ''' <summary>A generator with the given seed (<paramref name="numberOfElements"/>, Java's native buffer size, is ignored).</summary>
        Public Sub New(ByVal seed As Long, ByVal numberOfElements As Long)
            Me.New(seed)
        End Sub

        ''' <summary>The underlying generator (for the managed random kernels).</summary>
        Public ReadOnly Property Generator As ManagedRandomGenerator
            Get
                Return generatorField
            End Get
        End Property

        ''' <summary>Reseeds with an int seed (sign-extended).</summary>
        Public Overridable Sub setSeed(ByVal seed As Integer) Implements RandomT.setSeed
            setSeed(CLng(seed))
        End Sub

        ''' <summary>The seed last set.</summary>
        Public Overridable Function getSeed() As Long Implements RandomT.getSeed
            Return seedField
        End Function

        ''' <summary>Reseeds with the product of the values.</summary>
        Public Overridable Sub setSeed(ByVal seed() As Integer) Implements RandomT.setSeed
            If seed Is Nothing Then
                Throw New ArgumentNullException(NameOf(seed))
            End If
            Dim sd As Long = 1
            For Each em As Integer In seed
                sd *= em
            Next em
            setSeed(sd)
        End Sub

        ''' <summary>Reseeds: the sequence restarts and the position becomes 0.</summary>
        Public Overridable Sub setSeed(ByVal seed As Long) Implements RandomT.setSeed
            seedField = seed
            generatorField.setSeed(seed)
        End Sub

        ''' <summary>Fills the array with random bytes.</summary>
        Public Overridable Sub nextBytes(ByVal bytes() As Byte) Implements RandomT.nextBytes
            generatorField.nextBytes(bytes)
        End Sub

        ''' <summary>A uniformly distributed int.</summary>
        Public Overridable Function nextInt() As Integer Implements RandomT.nextInt
            Return generatorField.nextInt()
        End Function

        ''' <summary>A uniformly distributed int in [0, n).</summary>
        Public Overridable Function nextInt(ByVal n As Integer) As Integer Implements RandomT.nextInt
            Return generatorField.nextInt(n)
        End Function

        ''' <summary>A uniformly distributed int in [a, n): <c>nextInt(n - a) + a</c>, as in Java.</summary>
        Public Overridable Function nextInt(ByVal a As Integer, ByVal n As Integer) As Integer Implements RandomT.nextInt
            Return nextInt(n - a) + a
        End Function

        ''' <summary>A uniformly distributed long.</summary>
        Public Overridable Function nextLong() As Long Implements RandomT.nextLong
            Return generatorField.nextLong()
        End Function

        ''' <summary>A uniformly distributed boolean.</summary>
        Public Overridable Function nextBoolean() As Boolean Implements RandomT.nextBoolean
            Return generatorField.nextBoolean()
        End Function

        ''' <summary>A uniformly distributed float in [0, 1).</summary>
        Public Overridable Function nextFloat() As Single Implements RandomT.nextFloat
            Return generatorField.nextFloat()
        End Function

        ''' <summary>A uniformly distributed double in [0, 1).</summary>
        Public Overridable Function nextDouble() As Double Implements RandomT.nextDouble
            Return generatorField.nextDouble()
        End Function

        ''' <summary>A standard normal value.</summary>
        Public Overridable Function nextGaussian() As Double Implements RandomT.nextGaussian
            Return generatorField.nextGaussian()
        End Function

        Private Delegate Sub FillElement(ByVal data As DataBuffer, ByVal index As Long)

        Private Shared Function fill(ByVal array As INDArray, ByVal element As FillElement) As INDArray
            Dim data As DataBuffer = array.data()
            Dim n As Long = array.length()
            For i As Long = 0 To n - 1
                element(data, i)
            Next
            Return array
        End Function

        ''' <summary>A new array of standard normal values in the default order.</summary>
        Public Overridable Function nextGaussian(ByVal shape() As Integer) As INDArray Implements RandomT.nextGaussian
            Return nextGaussian(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of standard normal values in the default order.</summary>
        Public Overridable Function nextGaussian(ByVal shape() As Long) As INDArray Implements RandomT.nextGaussian
            Return nextGaussian(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of standard normal values.</summary>
        Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements RandomT.nextGaussian
            Return nextGaussian(order, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of standard normal values.</summary>
        Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Long) As INDArray Implements RandomT.nextGaussian
            Return fill(Nd4vbT.createUninitialized(shape, order), Sub(d, i) d.put(i, generatorField.nextGaussian()))
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1) in the default order.</summary>
        Public Overridable Function nextDouble(ByVal shape() As Integer) As INDArray Implements RandomT.nextDouble
            Return nextDouble(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1) in the default order.</summary>
        Public Overridable Function nextDouble(ByVal shape() As Long) As INDArray Implements RandomT.nextDouble
            Return nextDouble(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1).</summary>
        Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements RandomT.nextDouble
            Return nextDouble(order, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform doubles in [0, 1).</summary>
        Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Long) As INDArray Implements RandomT.nextDouble
            Return fill(Nd4vbT.createUninitialized(shape, order), Sub(d, i) d.put(i, generatorField.nextDouble()))
        End Function

        ''' <summary>A new array of uniform floats in [0, 1) in the default order.</summary>
        Public Overridable Function nextFloat(ByVal shape() As Integer) As INDArray Implements RandomT.nextFloat
            Return nextFloat(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform floats in [0, 1) in the default order.</summary>
        Public Overridable Function nextFloat(ByVal shape() As Long) As INDArray Implements RandomT.nextFloat
            Return nextFloat(Nd4vbT.order(), shape)
        End Function

        ''' <summary>A new array of uniform floats in [0, 1).</summary>
        Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements RandomT.nextFloat
            Return nextFloat(order, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform floats in [0, 1).</summary>
        Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Long) As INDArray Implements RandomT.nextFloat
            Return fill(Nd4vbT.createUninitialized(shape, order), Sub(d, i) d.put(i, generatorField.nextFloat()))
        End Function

        ''' <summary>A new array of uniform ints.</summary>
        Public Overridable Function nextInt(ByVal shape() As Integer) As INDArray Implements RandomT.nextInt
            Return nextInt(ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform ints.</summary>
        Public Overridable Function nextInt(ByVal shape() As Long) As INDArray Implements RandomT.nextInt
            Return fill(Nd4vbT.create(shape), Sub(d, i) d.put(i, generatorField.nextInt()))
        End Function

        ''' <summary>A new array of uniform ints in [0, n).</summary>
        Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Integer) As INDArray Implements RandomT.nextInt
            Return nextInt(n, ArrayUtil.toLongArray(shape))
        End Function

        ''' <summary>A new array of uniform ints in [0, n).</summary>
        Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Long) As INDArray Implements RandomT.nextInt
            If n <= 0 Then
                Throw New ArgumentException("bound must be positive, was " & n, NameOf(n))
            End If
            Return fill(Nd4vbT.create(shape), Sub(d, i) d.put(i, generatorField.nextInt(n)))
        End Function

        ''' <summary>The number of 64-bit outputs drawn since the seed was set.</summary>
        Public Overridable ReadOnly Property Position As Long Implements RandomT.Position
            Get
                Return generatorField.getPosition()
            End Get
        End Property

        ''' <summary>Reseeds from the clock.</summary>
        Public Overridable Sub reSeed() Implements RandomT.reSeed
            reSeed(JavaSystem.currentTimeMillis())
        End Sub

        ''' <summary>Reseeds with the given seed.</summary>
        Public Overridable Sub reSeed(ByVal seed As Long) Implements RandomT.reSeed
            setSeed(seed)
        End Sub

        ''' <summary>The seed.</summary>
        Public Overridable Function rootState() As Long Implements RandomT.rootState
            Return seedField
        End Function

        ''' <summary>The number of outputs drawn since the seed was set.</summary>
        Public Overridable Function nodeState() As Long Implements RandomT.nodeState
            Return generatorField.getPosition()
        End Function

        ''' <summary>Reseeds with <paramref name="rootState"/> and skips <paramref name="nodeState"/> outputs.</summary>
        Public Overridable Sub setStates(ByVal rootState As Long, ByVal nodeState As Long) Implements RandomT.setStates
            If nodeState < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(nodeState), "The node state (outputs drawn) cannot be negative")
            End If
            setSeed(rootState)
            For i As Long = 1 To nodeState
                generatorField.nextLong()
            Next
        End Sub

        ''' <summary>Nothing to release (Java deleted the native generator).</summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
        End Sub

    End Class

End Namespace
