Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Managed BLAS against naive reference loops: gemm for random sizes in both orders with every transpose
    ''' combination, padded leading dimensions, offsets and alpha/beta; gemv; and the level-1 routines.
    ''' </summary>
    <TestClass>
    Public Class ManagedBlasTest

        Private Shared ReadOnly Orders As Char() = {"c"c, "f"c}
        Private Shared ReadOnly Transposes As Char() = {"n"c, "t"c}

        ''' <summary>Index of element (i, j) of a matrix stored in the given order with leading dimension ld.</summary>
        Private Shared Function At(ByVal rowMajor As Boolean, ByVal i As Integer, ByVal j As Integer, ByVal ld As Integer, ByVal offset As Integer) As Integer
            Return offset + If(rowMajor, i * ld + j, i + j * ld)
        End Function

        ''' <summary>Index of element i of a strided vector of n elements (reference-BLAS convention for negative increments).</summary>
        Private Shared Function VectorAt(ByVal i As Integer, ByVal n As Integer, ByVal offset As Integer, ByVal inc As Integer) As Integer
            Return offset + If(inc >= 0, i * inc, (n - 1 - i) * -inc)
        End Function

        ''' <summary>Runs <paramref name="action"/> with SIMD on and off (the switch only matters on .NET 10).</summary>
        Private Shared Sub ForEachSimdSetting(ByVal action As Action(Of Boolean))
            Dim saved As Boolean = KernelOptions.UseSimd
            Try
                For Each simd As Boolean In {True, False}
                    KernelOptions.UseSimd = simd
                    action(simd)
                Next
            Finally
                KernelOptions.UseSimd = saved
            End Try
        End Sub

#Region "gemm"

        ''' <summary>One gemm case: operands filled at random, padding of A and B set to NaN (it must never be read), C's padding checked afterwards.</summary>
        Private Shared Sub CheckGemmDouble(ByVal rng As Global.System.Random, ByVal order As Char, ByVal transA As Char, ByVal transB As Char,
                                           ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double, ByVal beta As Double, ByVal label As String)
            Dim rm As Boolean = order = "c"c
            Dim ta As Boolean = transA = "t"c
            Dim tb As Boolean = transB = "t"c
            ' stored shapes: A is m x k (k x m when transposed), B is k x n (n x k), C is m x n
            Dim aRows As Integer = If(ta, k, m), aCols As Integer = If(ta, m, k)
            Dim bRows As Integer = If(tb, n, k), bCols As Integer = If(tb, k, n)
            Dim lda As Integer = Math.Max(1, If(rm, aCols, aRows)) + rng.Next(0, 3)
            Dim ldb As Integer = Math.Max(1, If(rm, bCols, bRows)) + rng.Next(0, 3)
            Dim ldc As Integer = Math.Max(1, If(rm, n, m)) + rng.Next(0, 3)
            Dim ao As Integer = rng.Next(0, 4), bo As Integer = rng.Next(0, 4), co As Integer = rng.Next(0, 4)
            Dim a As Double() = New Double(ao + lda * Math.Max(1, If(rm, aRows, aCols)) + 2) {}
            Dim b As Double() = New Double(bo + ldb * Math.Max(1, If(rm, bRows, bCols)) + 2) {}
            Dim c As Double() = New Double(co + ldc * Math.Max(1, If(rm, m, n)) + 2) {}
            For idx As Integer = 0 To a.Length - 1
                a(idx) = Double.NaN
            Next
            For idx As Integer = 0 To b.Length - 1
                b(idx) = Double.NaN
            Next
            For i As Integer = 0 To aRows - 1
                For j As Integer = 0 To aCols - 1
                    a(At(rm, i, j, lda, ao)) = rng.NextDouble() * 2.0 - 1.0
                Next
            Next
            For i As Integer = 0 To bRows - 1
                For j As Integer = 0 To bCols - 1
                    b(At(rm, i, j, ldb, bo)) = rng.NextDouble() * 2.0 - 1.0
                Next
            Next
            For idx As Integer = 0 To c.Length - 1
                c(idx) = rng.NextDouble() * 2.0 - 1.0
            Next
            Dim inView As New HashSet(Of Integer)()
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    Dim ci As Integer = At(rm, i, j, ldc, co)
                    inView.Add(ci)
                    If beta = 0.0 Then
                        c(ci) = Double.NaN ' beta = 0 must overwrite without reading
                    End If
                Next
            Next
            Dim original As Double() = CType(c.Clone(), Double())
            ManagedBlas.Gemm(order, transA, transB, m, n, k, alpha, a, ao, lda, b, bo, ldb, beta, c, co, ldc)
            Dim tol As Double = 1.0E-12 * (k + 2)
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    Dim s As Double = 0
                    For p As Integer = 0 To k - 1
                        Dim av As Double = If(ta, a(At(rm, p, i, lda, ao)), a(At(rm, i, p, lda, ao)))
                        Dim bv As Double = If(tb, b(At(rm, j, p, ldb, bo)), b(At(rm, p, j, ldb, bo)))
                        s += av * bv
                    Next
                    Dim ci As Integer = At(rm, i, j, ldc, co)
                    Dim expected As Double = alpha * s + If(beta = 0.0, 0.0, beta * original(ci))
                    Assert.AreEqual(expected, c(ci), tol, label & " C(" & i & "," & j & ")")
                Next
            Next
            For idx As Integer = 0 To c.Length - 1
                If Not inView.Contains(idx) Then
                    Assert.AreEqual(original(idx), c(idx), 0.0, label & " element " & idx & " outside C changed")
                End If
            Next
        End Sub

        Private Shared Sub CheckGemmSingle(ByVal rng As Global.System.Random, ByVal order As Char, ByVal transA As Char, ByVal transB As Char,
                                           ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Single, ByVal beta As Single, ByVal label As String)
            Dim rm As Boolean = order = "c"c
            Dim ta As Boolean = transA = "t"c
            Dim tb As Boolean = transB = "t"c
            Dim aRows As Integer = If(ta, k, m), aCols As Integer = If(ta, m, k)
            Dim bRows As Integer = If(tb, n, k), bCols As Integer = If(tb, k, n)
            Dim lda As Integer = Math.Max(1, If(rm, aCols, aRows)) + rng.Next(0, 3)
            Dim ldb As Integer = Math.Max(1, If(rm, bCols, bRows)) + rng.Next(0, 3)
            Dim ldc As Integer = Math.Max(1, If(rm, n, m)) + rng.Next(0, 3)
            Dim ao As Integer = rng.Next(0, 4), bo As Integer = rng.Next(0, 4), co As Integer = rng.Next(0, 4)
            Dim a As Single() = New Single(ao + lda * Math.Max(1, If(rm, aRows, aCols)) + 2) {}
            Dim b As Single() = New Single(bo + ldb * Math.Max(1, If(rm, bRows, bCols)) + 2) {}
            Dim c As Single() = New Single(co + ldc * Math.Max(1, If(rm, m, n)) + 2) {}
            For idx As Integer = 0 To a.Length - 1
                a(idx) = Single.NaN
            Next
            For idx As Integer = 0 To b.Length - 1
                b(idx) = Single.NaN
            Next
            For i As Integer = 0 To aRows - 1
                For j As Integer = 0 To aCols - 1
                    a(At(rm, i, j, lda, ao)) = CSng(rng.NextDouble() * 2.0 - 1.0)
                Next
            Next
            For i As Integer = 0 To bRows - 1
                For j As Integer = 0 To bCols - 1
                    b(At(rm, i, j, ldb, bo)) = CSng(rng.NextDouble() * 2.0 - 1.0)
                Next
            Next
            For idx As Integer = 0 To c.Length - 1
                c(idx) = CSng(rng.NextDouble() * 2.0 - 1.0)
            Next
            Dim inView As New HashSet(Of Integer)()
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    Dim ci As Integer = At(rm, i, j, ldc, co)
                    inView.Add(ci)
                    If beta = 0.0F Then
                        c(ci) = Single.NaN
                    End If
                Next
            Next
            Dim original As Single() = CType(c.Clone(), Single())
            ManagedBlas.Gemm(order, transA, transB, m, n, k, alpha, a, ao, lda, b, bo, ldb, beta, c, co, ldc)
            Dim tol As Double = 2.0E-6 * (k + 2)
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    Dim s As Double = 0
                    For p As Integer = 0 To k - 1
                        Dim av As Double = If(ta, a(At(rm, p, i, lda, ao)), a(At(rm, i, p, lda, ao)))
                        Dim bv As Double = If(tb, b(At(rm, j, p, ldb, bo)), b(At(rm, p, j, ldb, bo)))
                        s += av * bv
                    Next
                    Dim ci As Integer = At(rm, i, j, ldc, co)
                    Dim expected As Double = alpha * s + If(beta = 0.0F, 0.0, beta * CDbl(original(ci)))
                    Assert.AreEqual(expected, c(ci), tol, label & " C(" & i & "," & j & ")")
                Next
            Next
            For idx As Integer = 0 To c.Length - 1
                If Not inView.Contains(idx) Then
                    Assert.AreEqual(original(idx), c(idx), 0.0F, label & " element " & idx & " outside C changed")
                End If
            Next
        End Sub

        ''' <summary>Sizes: random small ones (direct loop), and ones that take the packed path (more than 32768 multiply-adds), cross panel boundaries (k &gt; 256) or are degenerate.</summary>
        Private Shared Function GemmSizes(ByVal rng As Global.System.Random) As List(Of Integer())
            Dim sizes As New List(Of Integer())()
            For r As Integer = 0 To 5
                sizes.Add(New Integer() {rng.Next(1, 20), rng.Next(1, 20), rng.Next(1, 20)})
            Next
            sizes.Add(New Integer() {rng.Next(30, 70), rng.Next(30, 70), rng.Next(30, 70)})
            sizes.Add(New Integer() {37, 29, 301})
            sizes.Add(New Integer() {1, 1, 1})
            sizes.Add(New Integer() {1, 150, 300})
            sizes.Add(New Integer() {150, 1, 300})
            sizes.Add(New Integer() {0, 5, 3})
            sizes.Add(New Integer() {4, 6, 0})
            Return sizes
        End Function

        <TestMethod>
        Public Sub GemmDoubleMatchesNaiveLoopForEveryOrderAndTranspose()
            Dim rng As New Global.System.Random(1234)
            Dim scalars As (Alpha As Double, Beta As Double)() = {(1.0, 0.0), (-0.75, 1.0), (2.0, 0.5), (1.0, -1.5)}
            ForEachSimdSetting(
                Sub(simd As Boolean)
                    For Each size As Integer() In GemmSizes(rng)
                        For Each order As Char In Orders
                            For Each ta As Char In Transposes
                                For Each tb As Char In Transposes
                                    Dim s = scalars(rng.Next(scalars.Length))
                                    CheckGemmDouble(rng, order, ta, tb, size(0), size(1), size(2), s.Alpha, s.Beta,
                                                    "simd " & simd & " " & order & ta & tb & " " & String.Join("x", size) & " alpha " & s.Alpha & " beta " & s.Beta)
                                Next
                            Next
                        Next
                    Next
                End Sub)
        End Sub

        <TestMethod>
        Public Sub GemmSingleMatchesNaiveLoopForEveryOrderAndTranspose()
            Dim rng As New Global.System.Random(4321)
            Dim scalars As (Alpha As Single, Beta As Single)() = {(1.0F, 0.0F), (-0.75F, 1.0F), (2.0F, 0.5F), (1.0F, -1.5F)}
            ForEachSimdSetting(
                Sub(simd As Boolean)
                    For Each size As Integer() In GemmSizes(rng)
                        For Each order As Char In Orders
                            For Each ta As Char In Transposes
                                For Each tb As Char In Transposes
                                    Dim s = scalars(rng.Next(scalars.Length))
                                    CheckGemmSingle(rng, order, ta, tb, size(0), size(1), size(2), s.Alpha, s.Beta,
                                                    "simd " & simd & " " & order & ta & tb & " " & String.Join("x", size) & " alpha " & s.Alpha & " beta " & s.Beta)
                                Next
                            Next
                        Next
                    Next
                End Sub)
        End Sub

        <TestMethod>
        Public Sub GemmInParallelTiles()
            Dim rng As New Global.System.Random(99)
            KernelTestUtil.WithLowThresholds(
                Sub()
                    Dim dop As Integer = KernelOptions.MaxDegreeOfParallelism
                    Try
                        KernelOptions.MaxDegreeOfParallelism = 4
                        ForEachSimdSetting(
                            Sub(simd As Boolean)
                                For Each size As Integer() In {New Integer() {130, 70, 40}, New Integer() {3, 4200, 5}, New Integer() {67, 260, 33}}
                                    For Each order As Char In Orders
                                        CheckGemmDouble(rng, order, "n"c, "t"c, size(0), size(1), size(2), 1.5, 0.25, "parallel double " & order & " " & String.Join("x", size))
                                        CheckGemmSingle(rng, order, "t"c, "n"c, size(0), size(1), size(2), 1.5F, 0.0F, "parallel single " & order & " " & String.Join("x", size))
                                    Next
                                Next
                            End Sub)
                    Finally
                        KernelOptions.MaxDegreeOfParallelism = dop
                    End Try
                End Sub)
        End Sub

        <TestMethod>
        Public Sub GemmSpecialCases()
            ' alpha = 0: C = beta * C and A, B are not read (NaN there must not leak)
            Dim a As Double() = {Double.NaN, Double.NaN, Double.NaN, Double.NaN}
            Dim c As Double() = {1.0, 2.0, 3.0, 4.0}
            ManagedBlas.Gemm("c"c, "n"c, "n"c, 2, 2, 2, 0.0, a, 0, 2, a, 0, 2, 3.0, c, 0, 2)
            CollectionAssert.AreEqual(New Double() {3.0, 6.0, 9.0, 12.0}, c)
            ' k = 0: C = beta * C
            ManagedBlas.Gemm("f"c, "n"c, "n"c, 2, 2, 0, 1.0, a, 0, 2, a, 0, 1, 0.0, c, 0, 2)
            CollectionAssert.AreEqual(New Double() {0.0, 0.0, 0.0, 0.0}, c)
            ' a known product: [[1,2],[3,4]] x [[5,6],[7,8]] = [[19,22],[43,50]], upper-case flags accepted
            Dim x As Single() = {1, 2, 3, 4}
            Dim y As Single() = {5, 6, 7, 8}
            Dim z As Single() = New Single(3) {}
            ManagedBlas.Gemm("C"c, "N"c, "N"c, 2, 2, 2, 1.0F, x, 0, 2, y, 0, 2, 0.0F, z, 0, 2)
            CollectionAssert.AreEqual(New Single() {19, 22, 43, 50}, z)
            ' the same data read column-major is the transposed product: x' y' = (y x)'
            ManagedBlas.Gemm("f"c, "n"c, "n"c, 2, 2, 2, 1.0F, x, 0, 2, y, 0, 2, 0.0F, z, 0, 2)
            CollectionAssert.AreEqual(New Single() {23, 34, 31, 46}, z)
            ' "c" as a transpose flag means conjugate-transpose, a transpose for real data
            ManagedBlas.Gemm("c"c, "c"c, "n"c, 2, 2, 2, 1.0F, x, 0, 2, y, 0, 2, 0.0F, z, 0, 2)
            CollectionAssert.AreEqual(New Single() {26, 30, 38, 44}, z)
        End Sub

        <TestMethod>
        Public Sub GemmValidation()
            Dim a As Double() = New Double(5) {}
            Dim c As Double() = New Double(3) {}
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemm("x"c, "n"c, "n"c, 2, 2, 2, 1.0, a, 0, 2, a, 0, 2, 0.0, c, 0, 2))
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemm("c"c, "q"c, "n"c, 2, 2, 2, 1.0, a, 0, 2, a, 0, 2, 0.0, c, 0, 2))
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemm("c"c, "n"c, "n"c, -1, 2, 2, 1.0, a, 0, 2, a, 0, 2, 0.0, c, 0, 2))
            ' leading dimension smaller than a row of a row-major matrix
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemm("c"c, "n"c, "n"c, 2, 2, 3, 1.0, a, 0, 2, a, 0, 2, 0.0, c, 0, 2))
            ' C too small for its view
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemm("c"c, "n"c, "n"c, 2, 2, 2, 1.0, a, 0, 2, a, 0, 2, 0.0, c, 1, 2))
        End Sub

#End Region

#Region "gemv"

        <TestMethod>
        Public Sub GemvMatchesNaiveLoop()
            Dim rng As New Global.System.Random(77)
            Dim incs As Integer() = {1, 2, -1, -3}
            Dim betas As Double() = {0.0, 1.0, 0.5}
            ForEachSimdSetting(
                Sub(simd As Boolean)
                    For Each size As Integer() In {New Integer() {1, 1}, New Integer() {5, 3}, New Integer() {3, 17}, New Integer() {40, 33}, New Integer() {0, 4}, New Integer() {4, 0}}
                        Dim m As Integer = size(0), n As Integer = size(1)
                        For Each order As Char In Orders
                            For Each trans As Char In Transposes
                                Dim incX As Integer = incs(rng.Next(incs.Length))
                                Dim incY As Integer = incs(rng.Next(incs.Length))
                                Dim alpha As Double = If(rng.Next(2) = 0, 1.0, -2.5)
                                Dim beta As Double = betas(rng.Next(betas.Length))
                                CheckGemv(rng, order, trans, m, n, alpha, beta, incX, incY, "simd " & simd & " " & order & trans & " " & m & "x" & n & " inc " & incX & "," & incY)
                            Next
                        Next
                    Next
                End Sub)
            KernelTestUtil.WithLowThresholds(
                Sub()
                    CheckGemv(rng, "c"c, "n"c, 70, 50, 1.0, 0.5, 1, 1, "parallel cn")
                    CheckGemv(rng, "f"c, "n"c, 70, 50, 1.0, 0.0, 2, -1, "parallel fn")
                    CheckGemv(rng, "c"c, "t"c, 70, 50, -1.0, 1.0, -2, 1, "parallel ct")
                    CheckGemv(rng, "f"c, "t"c, 70, 50, 2.0, 0.0, 1, 3, "parallel ft")
                End Sub)
        End Sub

        Private Shared Sub CheckGemv(ByVal rng As Global.System.Random, ByVal order As Char, ByVal trans As Char, ByVal m As Integer, ByVal n As Integer,
                                     ByVal alpha As Double, ByVal beta As Double, ByVal incX As Integer, ByVal incY As Integer, ByVal label As String)
            Dim rm As Boolean = order = "c"c
            Dim t As Boolean = trans = "t"c
            Dim lda As Integer = Math.Max(1, If(rm, n, m)) + rng.Next(0, 2)
            Dim ao As Integer = rng.Next(0, 3)
            Dim lenX As Integer = If(t, m, n)
            Dim lenY As Integer = If(t, n, m)
            Dim xo As Integer = rng.Next(0, 3), yo As Integer = rng.Next(0, 3)
            ' double and single copies of the same data
            Dim a As Double() = New Double(ao + lda * Math.Max(1, If(rm, m, n)) + 1) {}
            Dim x As Double() = New Double(xo + Math.Max(1, lenX) * Math.Abs(incX) + 1) {}
            Dim y As Double() = New Double(yo + Math.Max(1, lenY) * Math.Abs(incY) + 1) {}
            For idx As Integer = 0 To a.Length - 1
                a(idx) = Math.Round(rng.NextDouble() * 8.0 - 4.0, 2)
            Next
            For idx As Integer = 0 To x.Length - 1
                x(idx) = Math.Round(rng.NextDouble() * 8.0 - 4.0, 2)
            Next
            For idx As Integer = 0 To y.Length - 1
                y(idx) = Math.Round(rng.NextDouble() * 8.0 - 4.0, 2)
            Next
            Dim inView As New HashSet(Of Integer)()
            For i As Integer = 0 To lenY - 1
                inView.Add(VectorAt(i, lenY, yo, incY))
                If beta = 0.0 Then
                    y(VectorAt(i, lenY, yo, incY)) = Double.NaN
                End If
            Next
            Dim expected As Double() = New Double(lenY - 1) {}
            For i As Integer = 0 To lenY - 1
                Dim s As Double = 0
                For j As Integer = 0 To lenX - 1
                    Dim av As Double = If(t, a(At(rm, j, i, lda, ao)), a(At(rm, i, j, lda, ao)))
                    s += av * x(VectorAt(j, lenX, xo, incX))
                Next
                expected(i) = alpha * s + If(beta = 0.0, 0.0, beta * y(VectorAt(i, lenY, yo, incY)))
            Next
            Dim yd As Double() = CType(y.Clone(), Double())
            Dim af As Single() = Array.ConvertAll(a, Function(v As Double) CSng(v))
            Dim xf As Single() = Array.ConvertAll(x, Function(v As Double) CSng(v))
            Dim yf As Single() = Array.ConvertAll(y, Function(v As Double) CSng(v))
            ManagedBlas.Gemv(order, trans, m, n, alpha, a, ao, lda, x, xo, incX, beta, yd, yo, incY)
            ManagedBlas.Gemv(order, trans, m, n, CSng(alpha), af, ao, lda, xf, xo, incX, CSng(beta), yf, yo, incY)
            For i As Integer = 0 To lenY - 1
                Dim yi As Integer = VectorAt(i, lenY, yo, incY)
                Assert.AreEqual(expected(i), yd(yi), 1.0E-9, label & " double y(" & i & ")")
                Assert.AreEqual(expected(i), yf(yi), 1.0E-3, label & " single y(" & i & ")")
            Next
            For idx As Integer = 0 To y.Length - 1
                If Not inView.Contains(idx) Then
                    Assert.AreEqual(y(idx), yd(idx), 0.0, label & " element " & idx & " outside y changed")
                End If
            Next
        End Sub

        <TestMethod>
        Public Sub GemvKnownValuesAndValidation()
            ' [[1,2,3],[4,5,6]] * [1,0,-1] = [-2,-2]; transposed: [1,1] -> [5,7,9]
            Dim a As Double() = {1, 2, 3, 4, 5, 6}
            Dim y As Double() = New Double(1) {}
            ManagedBlas.Gemv("c"c, "n"c, 2, 3, 1.0, a, 0, 3, New Double() {1, 0, -1}, 0, 1, 0.0, y, 0, 1)
            CollectionAssert.AreEqual(New Double() {-2, -2}, y)
            Dim yt As Double() = New Double(2) {}
            ManagedBlas.Gemv("c"c, "t"c, 2, 3, 1.0, a, 0, 3, New Double() {1, 1}, 0, 1, 0.0, yt, 0, 1)
            CollectionAssert.AreEqual(New Double() {5, 7, 9}, yt)
            ' column-major: the same data is [[1,3,5],[2,4,6]]
            ManagedBlas.Gemv("f"c, "n"c, 2, 3, 1.0, a, 0, 2, New Double() {1, 0, -1}, 0, 1, 0.0, y, 0, 1)
            CollectionAssert.AreEqual(New Double() {-4, -4}, y)
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemv("c"c, "n"c, 2, 3, 1.0, a, 0, 3, New Double() {1, 0, -1}, 0, 0, 0.0, y, 0, 1))
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemv("c"c, "n"c, 2, 3, 1.0, a, 0, 3, New Double() {1, 0}, 0, 1, 0.0, y, 0, 1))
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Gemv("c"c, "n"c, 2, 3, 1.0, a, 0, 2, New Double() {1, 0, -1}, 0, 1, 0.0, y, 0, 1))
        End Sub

#End Region

#Region "Level 1"

        <TestMethod>
        Public Sub DotAxpyCopySwapWithStridesAndNegativeIncrements()
            ForEachSimdSetting(
                Sub(simd As Boolean)
                    For Each n As Integer In {0, 1, 7, 33, 100}
                        For Each incX As Integer In {1, 2, -1, -2}
                            For Each incY As Integer In {1, 3, -1}
                                Dim label As String = "simd " & simd & " n " & n & " inc " & incX & "," & incY
                                Dim xo As Integer = 1, yo As Integer = 2
                                Dim x As Double() = New Double(xo + Math.Max(1, n) * Math.Abs(incX) + 1) {}
                                Dim y As Double() = New Double(yo + Math.Max(1, n) * Math.Abs(incY) + 1) {}
                                For idx As Integer = 0 To x.Length - 1
                                    x(idx) = (idx * 7 Mod 11) - 5
                                Next
                                For idx As Integer = 0 To y.Length - 1
                                    y(idx) = (idx * 5 Mod 13) - 6
                                Next
                                Dim xs As Single() = Array.ConvertAll(x, Function(v As Double) CSng(v))
                                Dim ys As Single() = Array.ConvertAll(y, Function(v As Double) CSng(v))
                                Dim dot As Double = 0
                                For i As Integer = 0 To n - 1
                                    dot += x(VectorAt(i, n, xo, incX)) * y(VectorAt(i, n, yo, incY))
                                Next
                                Assert.AreEqual(dot, ManagedBlas.Dot(n, x, xo, incX, y, yo, incY), 0.0, label & " dot")
                                Assert.AreEqual(CSng(dot), ManagedBlas.Dot(n, xs, xo, incX, ys, yo, incY), 0.0F, label & " sdot")

                                ' axpy: y = 2.5 x + y
                                Dim yAxpy As Double() = CType(y.Clone(), Double())
                                Dim ysAxpy As Single() = CType(ys.Clone(), Single())
                                ManagedBlas.Axpy(n, 2.5, x, xo, incX, yAxpy, yo, incY)
                                ManagedBlas.Axpy(n, 2.5F, xs, xo, incX, ysAxpy, yo, incY)
                                Dim expectedAxpy As Double() = CType(y.Clone(), Double())
                                For i As Integer = 0 To n - 1
                                    Dim yi As Integer = VectorAt(i, n, yo, incY)
                                    expectedAxpy(yi) = 2.5 * x(VectorAt(i, n, xo, incX)) + y(yi)
                                Next
                                CollectionAssert.AreEqual(expectedAxpy, yAxpy, label & " axpy")
                                CollectionAssert.AreEqual(Array.ConvertAll(expectedAxpy, Function(v As Double) CSng(v)), ysAxpy, label & " saxpy")

                                ' copy: y = x
                                Dim yCopy As Double() = CType(y.Clone(), Double())
                                ManagedBlas.Copy(n, x, xo, incX, yCopy, yo, incY)
                                Dim expectedCopy As Double() = CType(y.Clone(), Double())
                                For i As Integer = 0 To n - 1
                                    expectedCopy(VectorAt(i, n, yo, incY)) = x(VectorAt(i, n, xo, incX))
                                Next
                                CollectionAssert.AreEqual(expectedCopy, yCopy, label & " copy")
                                Dim ysCopy As Single() = CType(ys.Clone(), Single())
                                ManagedBlas.Copy(n, xs, xo, incX, ysCopy, yo, incY)
                                CollectionAssert.AreEqual(Array.ConvertAll(expectedCopy, Function(v As Double) CSng(v)), ysCopy, label & " scopy")

                                ' swap
                                Dim xSwap As Double() = CType(x.Clone(), Double())
                                Dim ySwap As Double() = CType(y.Clone(), Double())
                                ManagedBlas.Swap(n, xSwap, xo, incX, ySwap, yo, incY)
                                Dim expectedX As Double() = CType(x.Clone(), Double())
                                Dim expectedY As Double() = CType(y.Clone(), Double())
                                For i As Integer = 0 To n - 1
                                    expectedX(VectorAt(i, n, xo, incX)) = y(VectorAt(i, n, yo, incY))
                                    expectedY(VectorAt(i, n, yo, incY)) = x(VectorAt(i, n, xo, incX))
                                Next
                                CollectionAssert.AreEqual(expectedX, xSwap, label & " swap x")
                                CollectionAssert.AreEqual(expectedY, ySwap, label & " swap y")
                                Dim xsSwap As Single() = CType(xs.Clone(), Single())
                                Dim ysSwap As Single() = CType(ys.Clone(), Single())
                                ManagedBlas.Swap(n, xsSwap, xo, incX, ysSwap, yo, incY)
                                CollectionAssert.AreEqual(Array.ConvertAll(expectedX, Function(v As Double) CSng(v)), xsSwap, label & " sswap x")
                                CollectionAssert.AreEqual(Array.ConvertAll(expectedY, Function(v As Double) CSng(v)), ysSwap, label & " sswap y")
                            Next
                        Next
                    Next
                End Sub)
        End Sub

        <TestMethod>
        Public Sub ScalAsumNrm2Iamax()
            ForEachSimdSetting(
                Sub(simd As Boolean)
                    For Each n As Integer In {1, 5, 37}
                        For Each inc As Integer In {1, 3}
                            Dim label As String = "simd " & simd & " n " & n & " inc " & inc
                            Dim x As Double() = New Double(2 + n * inc) {}
                            For idx As Integer = 0 To x.Length - 1
                                x(idx) = ((idx * 7) Mod 9) - 4.5
                            Next
                            Dim xs As Single() = Array.ConvertAll(x, Function(v As Double) CSng(v))
                            Dim asum As Double = 0, ss As Double = 0, best As Integer = 0
                            For i As Integer = 0 To n - 1
                                Dim v As Double = x(1 + i * inc)
                                asum += Math.Abs(v)
                                ss += v * v
                                If Math.Abs(v) > Math.Abs(x(1 + best * inc)) Then
                                    best = i
                                End If
                            Next
                            Assert.AreEqual(asum, ManagedBlas.Asum(n, x, 1, inc), 0.0, label & " asum")
                            Assert.AreEqual(CSng(asum), ManagedBlas.Asum(n, xs, 1, inc), 0.0F, label & " sasum")
                            Assert.AreEqual(Math.Sqrt(ss), ManagedBlas.Nrm2(n, x, 1, inc), 1.0E-12, label & " nrm2")
                            Assert.AreEqual(CSng(Math.Sqrt(ss)), ManagedBlas.Nrm2(n, xs, 1, inc), 1.0E-5F, label & " snrm2")
                            Assert.AreEqual(best, ManagedBlas.Iamax(n, x, 1, inc), label & " iamax")
                            Assert.AreEqual(best, ManagedBlas.Iamax(n, xs, 1, inc), label & " isamax")
                            Dim scaled As Double() = CType(x.Clone(), Double())
                            ManagedBlas.Scal(n, -2.0, scaled, 1, inc)
                            Dim scaledS As Single() = CType(xs.Clone(), Single())
                            ManagedBlas.Scal(n, -2.0F, scaledS, 1, inc)
                            For idx As Integer = 0 To x.Length - 1
                                Dim inVector As Boolean = idx >= 1 AndAlso (idx - 1) Mod inc = 0 AndAlso (idx - 1) \ inc < n
                                Assert.AreEqual(If(inVector, -2.0 * x(idx), x(idx)), scaled(idx), 0.0, label & " scal " & idx)
                                Assert.AreEqual(CSng(If(inVector, -2.0 * x(idx), x(idx))), scaledS(idx), 0.0F, label & " sscal " & idx)
                            Next
                        Next
                    Next
                End Sub)
            ' ties: the first largest magnitude wins
            Assert.AreEqual(1, ManagedBlas.Iamax(5, New Double() {1, -3, 3, 2, -3}, 0, 1))
            ' reference-BLAS conventions for non-positive increments and empty vectors
            Dim d As Double() = {1, 2, 3}
            Assert.AreEqual(0.0, ManagedBlas.Asum(3, d, 0, 0))
            Assert.AreEqual(0.0, ManagedBlas.Nrm2(3, d, 0, -1))
            Assert.AreEqual(0, ManagedBlas.Iamax(3, d, 0, -1))
            Assert.AreEqual(0, ManagedBlas.Iamax(0, d, 0, 1))
            ManagedBlas.Scal(3, 5.0, d, 0, -1)
            CollectionAssert.AreEqual(New Double() {1, 2, 3}, d)
            ' axpy with alpha = 0 leaves y alone
            Dim y As Double() = {1, 2}
            ManagedBlas.Axpy(2, 0.0, New Double() {Double.NaN, Double.NaN}, 0, 1, y, 0, 1)
            CollectionAssert.AreEqual(New Double() {1, 2}, y)
        End Sub

        <TestMethod>
        Public Sub Nrm2AvoidsOverflowAndUnderflow()
            Assert.AreEqual(Math.Sqrt(2.0) * 1.0E+200, ManagedBlas.Nrm2(2, New Double() {1.0E+200, -1.0E+200}, 0, 1), 1.0E+186)
            Assert.AreEqual(5.0E-200, ManagedBlas.Nrm2(2, New Double() {3.0E-200, 4.0E-200}, 0, 1), 1.0E-213)
            Assert.AreEqual(5.0E+30F, ManagedBlas.Nrm2(2, New Single() {3.0E+30F, 4.0E+30F}, 0, 1), 1.0E+25F)
            Assert.AreEqual(5.0E-30F, ManagedBlas.Nrm2(2, New Single() {3.0E-30F, -4.0E-30F}, 0, 1), 1.0E-35F)
            Assert.AreEqual(0.0, ManagedBlas.Nrm2(3, New Double() {0, 0, 0}, 0, 1))
        End Sub

        <TestMethod>
        Public Sub LargeLevel1RunsInParallelAndDeterministically()
            Dim n As Integer = 100003
            Dim rng As New Global.System.Random(3)
            Dim x As Double() = New Double(n - 1) {}
            Dim y As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                x(i) = rng.NextDouble() - 0.5
                y(i) = rng.NextDouble() - 0.5
            Next
            Dim xs As Single() = Array.ConvertAll(x, Function(v As Double) CSng(v))
            Dim ys As Single() = Array.ConvertAll(y, Function(v As Double) CSng(v))
            Dim dot As Double = 0, asum As Double = 0
            For i As Integer = 0 To n - 1
                dot += x(i) * y(i)
                asum += Math.Abs(x(i))
            Next
            Dim d1 As Double = ManagedBlas.Dot(n, x, 0, 1, y, 0, 1)
            Assert.AreEqual(dot, d1, 1.0E-10)
            Assert.AreEqual(d1, ManagedBlas.Dot(n, x, 0, 1, y, 0, 1), 0.0, "repeatable")
            Assert.AreEqual(dot, ManagedBlas.Dot(n, xs, 0, 1, ys, 0, 1), 1.0E-2)
            Assert.AreEqual(asum, ManagedBlas.Asum(n, x, 0, 1), 1.0E-9)
            Assert.AreEqual(asum, ManagedBlas.Asum(n, xs, 0, 1), 0.5)
            Dim yy As Double() = CType(y.Clone(), Double())
            ManagedBlas.Axpy(n, -1.0, x, 0, 1, yy, 0, 1)
            Dim zz As Double() = CType(x.Clone(), Double())
            ManagedBlas.Scal(n, 3.0, zz, 0, 1)
            For i As Integer = 0 To n - 1 Step 997
                Assert.AreEqual(y(i) - x(i), yy(i), 0.0)
                Assert.AreEqual(3.0 * x(i), zz(i), 0.0)
            Next
        End Sub

        <TestMethod>
        Public Sub Level1Validation()
            Dim x As Double() = New Double(4) {}
            Assert.ThrowsException(Of ArgumentException)(Function() ManagedBlas.Dot(4, x, 2, 1, x, 0, 1))
            Assert.ThrowsException(Of ArgumentException)(Sub() ManagedBlas.Axpy(3, 1.0, x, 0, 3, x, 0, 1))
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() ManagedBlas.Copy(2, x, -1, 1, x, 0, 1))
            Assert.ThrowsException(Of ArgumentException)(Function() ManagedBlas.Asum(6, x, 0, 1))
        End Sub

#End Region

    End Class

End Namespace
