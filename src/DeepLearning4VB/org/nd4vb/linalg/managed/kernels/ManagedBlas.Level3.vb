' DeepLearning4VB: managed BLAS level 3 (gemm) for the ND4VB engine.
' The Single and Double sections are the same code for the two precisions; change both together.

Imports System
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.kernels

    Partial Public NotInheritable Class ManagedBlas

        ' gemm blocking: KC rows of op(B) per packed panel, NC columns of op(B) per packed panel,
        ' macro tiles of about GemmTileRows x GemmTileCols elements of C per parallel task.
        Private Const GemmKC As Integer = 256
        Private Const GemmNC As Integer = 4096
        Private Const GemmTileRows As Integer = 64
        Private Const GemmTileCols As Integer = 128
        ' below this many multiply-adds gemm uses a direct loop (packing would cost more than it saves)
        Private Const SmallGemmWork As Long = 32768L

#Region "Level 3 (Single)"

        ''' <summary>
        ''' C = alpha * op(A) * op(B) + beta * C, where op(A) is m x k, op(B) is k x n and C is m x n, all
        ''' in the given order with leading dimensions lda, ldb and ldc; op(X) is X or its transpose.
        ''' With beta = 0, C is overwritten without being read.
        ''' </summary>
        Public Shared Sub Gemm(ByVal order As Char, ByVal transA As Char, ByVal transB As Char,
                               ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Single,
                               ByVal a As Single(), ByVal aOffset As Long, ByVal lda As Integer,
                               ByVal b As Single(), ByVal bOffset As Long, ByVal ldb As Integer,
                               ByVal beta As Single, ByVal c As Single(), ByVal cOffset As Long, ByVal ldc As Integer)
            Dim ta As Boolean = IsTranspose(transA, NameOf(transA))
            Dim tb As Boolean = IsTranspose(transB, NameOf(transB))
            If m < 0 OrElse n < 0 OrElse k < 0 Then
                Throw New ArgumentException("Matrix dimensions must not be negative")
            End If
            If IsRowMajor(order) Then
                ' row-major C = op(A) op(B) is column-major C' = op(B)' op(A)' with the operands swapped
                GemmColumnMajor(tb, ta, n, m, k, alpha, b, bOffset, ldb, a, aOffset, lda, beta, c, cOffset, ldc)
            Else
                GemmColumnMajor(ta, tb, m, n, k, alpha, a, aOffset, lda, b, bOffset, ldb, beta, c, cOffset, ldc)
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub GemmColumnMajor(ByVal ta As Boolean, ByVal tb As Boolean, ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Single,
                                           ByVal a As Single(), ByVal aOffset As Long, ByVal lda As Integer,
                                           ByVal b As Single(), ByVal bOffset As Long, ByVal ldb As Integer,
                                           ByVal beta As Single, ByVal c As Single(), ByVal cOffset As Long, ByVal ldc As Integer)
            CheckMatrix(a.Length, If(ta, k, m), If(ta, m, k), aOffset, lda, NameOf(a))
            CheckMatrix(b.Length, If(tb, n, k), If(tb, k, n), bOffset, ldb, NameOf(b))
            CheckMatrix(c.Length, m, n, cOffset, ldc, NameOf(c))
            If m = 0 OrElse n = 0 Then
                Return
            End If
            Dim ao As Integer = CInt(aOffset)
            Dim bo As Integer = CInt(bOffset)
            Dim co As Integer = CInt(cOffset)
            ' C = beta * C first (beta = 0 overwrites)
            If beta <> 1.0F Then
                For j As Integer = 0 To n - 1
                    Dim col As Integer = co + j * ldc
                    If beta = 0.0F Then
                        Array.Clear(c, col, m)
                    Else
                        For i As Integer = 0 To m - 1
                            c(col + i) *= beta
                        Next
                    End If
                Next
            End If
            If alpha = 0.0F OrElse k = 0 Then
                Return
            End If
            Dim work As Long = CLng(m) * n * k
            If work <= SmallGemmWork Then
                GemmNaive(ta, tb, m, n, k, alpha, a, ao, lda, b, bo, ldb, c, co, ldc)
                Return
            End If
            Dim useSimd As Boolean = KernelOptions.SimdEnabled
            Dim mr As Integer = 4
#If NET Then
            If useSimd Then
                mr = 2 * Global.System.Numerics.Vector(Of Single).Count
            End If
#End If
            Const nr As Integer = 4
            Dim parallelize As Boolean = KernelOptions.MaxDegreeOfParallelism > 1 AndAlso work >= KernelOptions.GemmParallelThreshold
            Dim kcMax As Integer = SysMath.Min(GemmKC, k)
            Dim ncMax As Integer = SysMath.Min(GemmNC, n)
            Dim mSlivers As Integer = (m + mr - 1) \ mr
            Dim ap As Single() = New Single(mSlivers * mr * kcMax - 1) {}
            Dim bp As Single() = New Single(((ncMax + nr - 1) \ nr) * nr * kcMax - 1) {}
            Dim tileRows As Integer = mr * SysMath.Max(1, GemmTileRows \ mr)
            Dim tileCols As Integer = GemmTileCols
            For pc As Integer = 0 To k - 1 Step GemmKC
                Dim kc As Integer = SysMath.Min(GemmKC, k - pc)
                PackA(ta, a, ao, lda, m, pc, kc, mr, ap, parallelize)
                For jc As Integer = 0 To n - 1 Step GemmNC
                    Dim nc As Integer = SysMath.Min(GemmNC, n - jc)
                    PackB(tb, b, bo, ldb, jc, nc, pc, kc, nr, bp, parallelize)
                    Dim rowTiles As Integer = (m + tileRows - 1) \ tileRows
                    Dim colTiles As Integer = (nc + tileCols - 1) \ tileCols
                    Dim tiles As Integer = rowTiles * colTiles
                    Dim kcl As Integer = kc
                    Dim jcl As Integer = jc
                    Dim ncl As Integer = nc
                    Dim tile As Action(Of Integer) =
                        Sub(tIndex As Integer)
                            Dim r0 As Integer = (tIndex \ colTiles) * tileRows
                            Dim c0 As Integer = (tIndex Mod colTiles) * tileCols
                            Dim r1 As Integer = SysMath.Min(r0 + tileRows, m)
                            Dim c1 As Integer = SysMath.Min(c0 + tileCols, ncl)
                            For colStart As Integer = c0 To c1 - 1 Step nr
                                Dim bIndex As Integer = (colStart \ nr) * nr * kcl
                                Dim nrv As Integer = SysMath.Min(nr, ncl - colStart)
                                For rowStart As Integer = r0 To r1 - 1 Step mr
                                    Dim aIndex As Integer = (rowStart \ mr) * mr * kcl
                                    Dim mrv As Integer = SysMath.Min(mr, m - rowStart)
                                    Dim cIndex As Integer = co + rowStart + (jcl + colStart) * ldc
#If NET Then
                                    If useSimd Then
                                        MicroKernelSimd(ap, aIndex, bp, bIndex, kcl, c, cIndex, ldc, alpha, mrv, nrv)
                                        Continue For
                                    End If
#End If
                                    MicroKernel4x4(ap, aIndex, bp, bIndex, kcl, c, cIndex, ldc, alpha, mrv, nrv)
                                Next
                            Next
                        End Sub
                    If parallelize AndAlso tiles > 1 Then
                        Global.System.Threading.Tasks.Parallel.For(0, tiles, KernelOptions.CreateParallelOptions(), tile)
                    Else
                        For tIndex As Integer = 0 To tiles - 1
                            tile(tIndex)
                        Next
                    End If
                Next
            Next
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub GemmNaive(ByVal ta As Boolean, ByVal tb As Boolean, ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Single,
                                     ByVal a As Single(), ByVal ao As Integer, ByVal lda As Integer,
                                     ByVal b As Single(), ByVal bo As Integer, ByVal ldb As Integer,
                                     ByVal c As Single(), ByVal co As Integer, ByVal ldc As Integer)
            For j As Integer = 0 To n - 1
                Dim col As Integer = co + j * ldc
                For p As Integer = 0 To k - 1
                    Dim bv As Single = If(tb, b(bo + j + p * ldb), b(bo + p + j * ldb))
                    Dim t As Single = alpha * bv
                    If ta Then
                        For i As Integer = 0 To m - 1
                            c(col + i) += t * a(ao + p + i * lda)
                        Next
                    Else
                        Dim acol As Integer = ao + p * lda
                        For i As Integer = 0 To m - 1
                            c(col + i) += t * a(acol + i)
                        Next
                    End If
                Next
            Next
        End Sub

        ''' <summary>Packs rows [0, m) and columns [pc, pc + kc) of op(A) into slivers of mr rows: sliver s holds, for each p, its mr values contiguously (zero padded).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub PackA(ByVal ta As Boolean, ByVal a As Single(), ByVal ao As Integer, ByVal lda As Integer, ByVal m As Integer,
                                 ByVal pc As Integer, ByVal kc As Integer, ByVal mr As Integer, ByVal ap As Single(), ByVal parallelize As Boolean)
            Dim slivers As Integer = (m + mr - 1) \ mr
            Dim body As Action(Of Integer) =
                Sub(s As Integer)
                    Dim baseIndex As Integer = s * mr * kc
                    Dim i0 As Integer = s * mr
                    Dim rowsHere As Integer = SysMath.Min(mr, m - i0)
                    If ta Then
                        ' op(A)(i, p) = A(p, i): contiguous along p
                        For ii As Integer = 0 To mr - 1
                            If ii < rowsHere Then
                                Dim src As Integer = ao + pc + (i0 + ii) * lda
                                Dim dst As Integer = baseIndex + ii
                                For p As Integer = 0 To kc - 1
                                    ap(dst) = a(src + p)
                                    dst += mr
                                Next
                            Else
                                Dim dst As Integer = baseIndex + ii
                                For p As Integer = 0 To kc - 1
                                    ap(dst) = 0.0F
                                    dst += mr
                                Next
                            End If
                        Next
                    Else
                        ' op(A)(i, p) = A(i, p): contiguous along i
                        For p As Integer = 0 To kc - 1
                            Dim src As Integer = ao + i0 + (pc + p) * lda
                            Dim dst As Integer = baseIndex + p * mr
                            For ii As Integer = 0 To rowsHere - 1
                                ap(dst + ii) = a(src + ii)
                            Next
                            For ii As Integer = rowsHere To mr - 1
                                ap(dst + ii) = 0.0F
                            Next
                        Next
                    End If
                End Sub
            If parallelize AndAlso slivers > 1 Then
                Global.System.Threading.Tasks.Parallel.For(0, slivers, KernelOptions.CreateParallelOptions(), body)
            Else
                For s As Integer = 0 To slivers - 1
                    body(s)
                Next
            End If
        End Sub

        ''' <summary>Packs rows [pc, pc + kc) and columns [jc, jc + nc) of op(B) into slivers of nr columns: sliver t holds, for each p, its nr values contiguously (zero padded).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub PackB(ByVal tb As Boolean, ByVal b As Single(), ByVal bo As Integer, ByVal ldb As Integer,
                                 ByVal jc As Integer, ByVal nc As Integer, ByVal pc As Integer, ByVal kc As Integer, ByVal nr As Integer,
                                 ByVal bp As Single(), ByVal parallelize As Boolean)
            Dim slivers As Integer = (nc + nr - 1) \ nr
            Dim body As Action(Of Integer) =
                Sub(t As Integer)
                    Dim baseIndex As Integer = t * nr * kc
                    Dim j0 As Integer = jc + t * nr
                    Dim colsHere As Integer = SysMath.Min(nr, jc + nc - j0)
                    For jj As Integer = 0 To nr - 1
                        Dim dst As Integer = baseIndex + jj
                        If jj >= colsHere Then
                            For p As Integer = 0 To kc - 1
                                bp(dst) = 0.0F
                                dst += nr
                            Next
                        ElseIf tb Then
                            ' op(B)(p, j) = B(j, p)
                            Dim src As Integer = bo + (j0 + jj) + pc * ldb
                            For p As Integer = 0 To kc - 1
                                bp(dst) = b(src)
                                src += ldb
                                dst += nr
                            Next
                        Else
                            ' op(B)(p, j) = B(p, j): contiguous along p
                            Dim src As Integer = bo + pc + (j0 + jj) * ldb
                            For p As Integer = 0 To kc - 1
                                bp(dst) = b(src + p)
                                dst += nr
                            Next
                        End If
                    Next
                End Sub
            If parallelize AndAlso slivers > 1 Then
                Global.System.Threading.Tasks.Parallel.For(0, slivers, KernelOptions.CreateParallelOptions(), body)
            Else
                For t As Integer = 0 To slivers - 1
                    body(t)
                Next
            End If
        End Sub

        ''' <summary>4 x 4 register-blocked micro-kernel: C(0..mrv, 0..nrv) += alpha * (A sliver x B sliver).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub MicroKernel4x4(ByVal ap As Single(), ByVal ai As Integer, ByVal bp As Single(), ByVal bi As Integer, ByVal kc As Integer,
                                          ByVal c As Single(), ByVal ci As Integer, ByVal ldc As Integer, ByVal alpha As Single,
                                          ByVal mrv As Integer, ByVal nrv As Integer)
            Dim c00 As Single = 0.0F, c10 As Single = 0.0F, c20 As Single = 0.0F, c30 As Single = 0.0F
            Dim c01 As Single = 0.0F, c11 As Single = 0.0F, c21 As Single = 0.0F, c31 As Single = 0.0F
            Dim c02 As Single = 0.0F, c12 As Single = 0.0F, c22 As Single = 0.0F, c32 As Single = 0.0F
            Dim c03 As Single = 0.0F, c13 As Single = 0.0F, c23 As Single = 0.0F, c33 As Single = 0.0F
            For p As Integer = 0 To kc - 1
                Dim a0 As Single = ap(ai)
                Dim a1 As Single = ap(ai + 1)
                Dim a2 As Single = ap(ai + 2)
                Dim a3 As Single = ap(ai + 3)
                Dim b0 As Single = bp(bi)
                c00 += a0 * b0
                c10 += a1 * b0
                c20 += a2 * b0
                c30 += a3 * b0
                Dim b1 As Single = bp(bi + 1)
                c01 += a0 * b1
                c11 += a1 * b1
                c21 += a2 * b1
                c31 += a3 * b1
                Dim b2 As Single = bp(bi + 2)
                c02 += a0 * b2
                c12 += a1 * b2
                c22 += a2 * b2
                c32 += a3 * b2
                Dim b3 As Single = bp(bi + 3)
                c03 += a0 * b3
                c13 += a1 * b3
                c23 += a2 * b3
                c33 += a3 * b3
                ai += 4
                bi += 4
            Next
            If mrv = 4 AndAlso nrv = 4 Then
                c(ci) += alpha * c00
                c(ci + 1) += alpha * c10
                c(ci + 2) += alpha * c20
                c(ci + 3) += alpha * c30
                ci += ldc
                c(ci) += alpha * c01
                c(ci + 1) += alpha * c11
                c(ci + 2) += alpha * c21
                c(ci + 3) += alpha * c31
                ci += ldc
                c(ci) += alpha * c02
                c(ci + 1) += alpha * c12
                c(ci + 2) += alpha * c22
                c(ci + 3) += alpha * c32
                ci += ldc
                c(ci) += alpha * c03
                c(ci + 1) += alpha * c13
                c(ci + 2) += alpha * c23
                c(ci + 3) += alpha * c33
                Return
            End If
            Dim acc As Single() = {c00, c10, c20, c30, c01, c11, c21, c31, c02, c12, c22, c32, c03, c13, c23, c33}
            For jj As Integer = 0 To nrv - 1
                For ii As Integer = 0 To mrv - 1
                    c(ci + ii + jj * ldc) += alpha * acc(jj * 4 + ii)
                Next
            Next
        End Sub

#If NET Then
        ''' <summary>SIMD micro-kernel (2 vectors x 4 columns): C(0..mrv, 0..nrv) += alpha * (A sliver x B sliver).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub MicroKernelSimd(ByVal ap As Single(), ByVal ai As Integer, ByVal bp As Single(), ByVal bi As Integer, ByVal kc As Integer,
                                           ByVal c As Single(), ByVal ci As Integer, ByVal ldc As Integer, ByVal alpha As Single,
                                           ByVal mrv As Integer, ByVal nrv As Integer)
            Dim vc As Integer = Global.System.Numerics.Vector(Of Single).Count
            Dim mr As Integer = 2 * vc
            Dim c00 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero, c10 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero
            Dim c01 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero, c11 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero
            Dim c02 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero, c12 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero
            Dim c03 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero, c13 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero
            For p As Integer = 0 To kc - 1
                Dim a0 As New Global.System.Numerics.Vector(Of Single)(ap, ai)
                Dim a1 As New Global.System.Numerics.Vector(Of Single)(ap, ai + vc)
                Dim bv As New Global.System.Numerics.Vector(Of Single)(bp(bi))
                c00 += a0 * bv
                c10 += a1 * bv
                bv = New Global.System.Numerics.Vector(Of Single)(bp(bi + 1))
                c01 += a0 * bv
                c11 += a1 * bv
                bv = New Global.System.Numerics.Vector(Of Single)(bp(bi + 2))
                c02 += a0 * bv
                c12 += a1 * bv
                bv = New Global.System.Numerics.Vector(Of Single)(bp(bi + 3))
                c03 += a0 * bv
                c13 += a1 * bv
                ai += mr
                bi += 4
            Next
            Dim va As New Global.System.Numerics.Vector(Of Single)(alpha)
            If mrv = mr AndAlso nrv = 4 Then
                Dim r As Global.System.Numerics.Vector(Of Single)
                r = New Global.System.Numerics.Vector(Of Single)(c, ci) + va * c00
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Single)(c, ci + vc) + va * c10
                r.CopyTo(c, ci + vc)
                ci += ldc
                r = New Global.System.Numerics.Vector(Of Single)(c, ci) + va * c01
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Single)(c, ci + vc) + va * c11
                r.CopyTo(c, ci + vc)
                ci += ldc
                r = New Global.System.Numerics.Vector(Of Single)(c, ci) + va * c02
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Single)(c, ci + vc) + va * c12
                r.CopyTo(c, ci + vc)
                ci += ldc
                r = New Global.System.Numerics.Vector(Of Single)(c, ci) + va * c03
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Single)(c, ci + vc) + va * c13
                r.CopyTo(c, ci + vc)
                Return
            End If
            Dim acc As Single() = New Single(mr * 4 - 1) {}
            c00.CopyTo(acc, 0)
            c10.CopyTo(acc, vc)
            c01.CopyTo(acc, mr)
            c11.CopyTo(acc, mr + vc)
            c02.CopyTo(acc, 2 * mr)
            c12.CopyTo(acc, 2 * mr + vc)
            c03.CopyTo(acc, 3 * mr)
            c13.CopyTo(acc, 3 * mr + vc)
            For jj As Integer = 0 To nrv - 1
                For ii As Integer = 0 To mrv - 1
                    c(ci + ii + jj * ldc) += alpha * acc(jj * mr + ii)
                Next
            Next
        End Sub
#End If

#End Region

#Region "Level 3 (Double)"

        ''' <summary>
        ''' C = alpha * op(A) * op(B) + beta * C, where op(A) is m x k, op(B) is k x n and C is m x n, all
        ''' in the given order with leading dimensions lda, ldb and ldc; op(X) is X or its transpose.
        ''' With beta = 0, C is overwritten without being read.
        ''' </summary>
        Public Shared Sub Gemm(ByVal order As Char, ByVal transA As Char, ByVal transB As Char,
                               ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double,
                               ByVal a As Double(), ByVal aOffset As Long, ByVal lda As Integer,
                               ByVal b As Double(), ByVal bOffset As Long, ByVal ldb As Integer,
                               ByVal beta As Double, ByVal c As Double(), ByVal cOffset As Long, ByVal ldc As Integer)
            Dim ta As Boolean = IsTranspose(transA, NameOf(transA))
            Dim tb As Boolean = IsTranspose(transB, NameOf(transB))
            If m < 0 OrElse n < 0 OrElse k < 0 Then
                Throw New ArgumentException("Matrix dimensions must not be negative")
            End If
            If IsRowMajor(order) Then
                ' row-major C = op(A) op(B) is column-major C' = op(B)' op(A)' with the operands swapped
                GemmColumnMajor(tb, ta, n, m, k, alpha, b, bOffset, ldb, a, aOffset, lda, beta, c, cOffset, ldc)
            Else
                GemmColumnMajor(ta, tb, m, n, k, alpha, a, aOffset, lda, b, bOffset, ldb, beta, c, cOffset, ldc)
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub GemmColumnMajor(ByVal ta As Boolean, ByVal tb As Boolean, ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double,
                                           ByVal a As Double(), ByVal aOffset As Long, ByVal lda As Integer,
                                           ByVal b As Double(), ByVal bOffset As Long, ByVal ldb As Integer,
                                           ByVal beta As Double, ByVal c As Double(), ByVal cOffset As Long, ByVal ldc As Integer)
            CheckMatrix(a.Length, If(ta, k, m), If(ta, m, k), aOffset, lda, NameOf(a))
            CheckMatrix(b.Length, If(tb, n, k), If(tb, k, n), bOffset, ldb, NameOf(b))
            CheckMatrix(c.Length, m, n, cOffset, ldc, NameOf(c))
            If m = 0 OrElse n = 0 Then
                Return
            End If
            Dim ao As Integer = CInt(aOffset)
            Dim bo As Integer = CInt(bOffset)
            Dim co As Integer = CInt(cOffset)
            ' C = beta * C first (beta = 0 overwrites)
            If beta <> 1.0 Then
                For j As Integer = 0 To n - 1
                    Dim col As Integer = co + j * ldc
                    If beta = 0.0 Then
                        Array.Clear(c, col, m)
                    Else
                        For i As Integer = 0 To m - 1
                            c(col + i) *= beta
                        Next
                    End If
                Next
            End If
            If alpha = 0.0 OrElse k = 0 Then
                Return
            End If
            Dim work As Long = CLng(m) * n * k
            If work <= SmallGemmWork Then
                GemmNaive(ta, tb, m, n, k, alpha, a, ao, lda, b, bo, ldb, c, co, ldc)
                Return
            End If
            Dim useSimd As Boolean = KernelOptions.SimdEnabled
            Dim mr As Integer = 4
#If NET Then
            If useSimd Then
                mr = 2 * Global.System.Numerics.Vector(Of Double).Count
            End If
#End If
            Const nr As Integer = 4
            Dim parallelize As Boolean = KernelOptions.MaxDegreeOfParallelism > 1 AndAlso work >= KernelOptions.GemmParallelThreshold
            Dim kcMax As Integer = SysMath.Min(GemmKC, k)
            Dim ncMax As Integer = SysMath.Min(GemmNC, n)
            Dim mSlivers As Integer = (m + mr - 1) \ mr
            Dim ap As Double() = New Double(mSlivers * mr * kcMax - 1) {}
            Dim bp As Double() = New Double(((ncMax + nr - 1) \ nr) * nr * kcMax - 1) {}
            Dim tileRows As Integer = mr * SysMath.Max(1, GemmTileRows \ mr)
            Dim tileCols As Integer = GemmTileCols
            For pc As Integer = 0 To k - 1 Step GemmKC
                Dim kc As Integer = SysMath.Min(GemmKC, k - pc)
                PackA(ta, a, ao, lda, m, pc, kc, mr, ap, parallelize)
                For jc As Integer = 0 To n - 1 Step GemmNC
                    Dim nc As Integer = SysMath.Min(GemmNC, n - jc)
                    PackB(tb, b, bo, ldb, jc, nc, pc, kc, nr, bp, parallelize)
                    Dim rowTiles As Integer = (m + tileRows - 1) \ tileRows
                    Dim colTiles As Integer = (nc + tileCols - 1) \ tileCols
                    Dim tiles As Integer = rowTiles * colTiles
                    Dim kcl As Integer = kc
                    Dim jcl As Integer = jc
                    Dim ncl As Integer = nc
                    Dim tile As Action(Of Integer) =
                        Sub(tIndex As Integer)
                            Dim r0 As Integer = (tIndex \ colTiles) * tileRows
                            Dim c0 As Integer = (tIndex Mod colTiles) * tileCols
                            Dim r1 As Integer = SysMath.Min(r0 + tileRows, m)
                            Dim c1 As Integer = SysMath.Min(c0 + tileCols, ncl)
                            For colStart As Integer = c0 To c1 - 1 Step nr
                                Dim bIndex As Integer = (colStart \ nr) * nr * kcl
                                Dim nrv As Integer = SysMath.Min(nr, ncl - colStart)
                                For rowStart As Integer = r0 To r1 - 1 Step mr
                                    Dim aIndex As Integer = (rowStart \ mr) * mr * kcl
                                    Dim mrv As Integer = SysMath.Min(mr, m - rowStart)
                                    Dim cIndex As Integer = co + rowStart + (jcl + colStart) * ldc
#If NET Then
                                    If useSimd Then
                                        MicroKernelSimd(ap, aIndex, bp, bIndex, kcl, c, cIndex, ldc, alpha, mrv, nrv)
                                        Continue For
                                    End If
#End If
                                    MicroKernel4x4(ap, aIndex, bp, bIndex, kcl, c, cIndex, ldc, alpha, mrv, nrv)
                                Next
                            Next
                        End Sub
                    If parallelize AndAlso tiles > 1 Then
                        Global.System.Threading.Tasks.Parallel.For(0, tiles, KernelOptions.CreateParallelOptions(), tile)
                    Else
                        For tIndex As Integer = 0 To tiles - 1
                            tile(tIndex)
                        Next
                    End If
                Next
            Next
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub GemmNaive(ByVal ta As Boolean, ByVal tb As Boolean, ByVal m As Integer, ByVal n As Integer, ByVal k As Integer, ByVal alpha As Double,
                                     ByVal a As Double(), ByVal ao As Integer, ByVal lda As Integer,
                                     ByVal b As Double(), ByVal bo As Integer, ByVal ldb As Integer,
                                     ByVal c As Double(), ByVal co As Integer, ByVal ldc As Integer)
            For j As Integer = 0 To n - 1
                Dim col As Integer = co + j * ldc
                For p As Integer = 0 To k - 1
                    Dim bv As Double = If(tb, b(bo + j + p * ldb), b(bo + p + j * ldb))
                    Dim t As Double = alpha * bv
                    If ta Then
                        For i As Integer = 0 To m - 1
                            c(col + i) += t * a(ao + p + i * lda)
                        Next
                    Else
                        Dim acol As Integer = ao + p * lda
                        For i As Integer = 0 To m - 1
                            c(col + i) += t * a(acol + i)
                        Next
                    End If
                Next
            Next
        End Sub

        ''' <summary>Packs rows [0, m) and columns [pc, pc + kc) of op(A) into slivers of mr rows: sliver s holds, for each p, its mr values contiguously (zero padded).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub PackA(ByVal ta As Boolean, ByVal a As Double(), ByVal ao As Integer, ByVal lda As Integer, ByVal m As Integer,
                                 ByVal pc As Integer, ByVal kc As Integer, ByVal mr As Integer, ByVal ap As Double(), ByVal parallelize As Boolean)
            Dim slivers As Integer = (m + mr - 1) \ mr
            Dim body As Action(Of Integer) =
                Sub(s As Integer)
                    Dim baseIndex As Integer = s * mr * kc
                    Dim i0 As Integer = s * mr
                    Dim rowsHere As Integer = SysMath.Min(mr, m - i0)
                    If ta Then
                        ' op(A)(i, p) = A(p, i): contiguous along p
                        For ii As Integer = 0 To mr - 1
                            If ii < rowsHere Then
                                Dim src As Integer = ao + pc + (i0 + ii) * lda
                                Dim dst As Integer = baseIndex + ii
                                For p As Integer = 0 To kc - 1
                                    ap(dst) = a(src + p)
                                    dst += mr
                                Next
                            Else
                                Dim dst As Integer = baseIndex + ii
                                For p As Integer = 0 To kc - 1
                                    ap(dst) = 0.0
                                    dst += mr
                                Next
                            End If
                        Next
                    Else
                        ' op(A)(i, p) = A(i, p): contiguous along i
                        For p As Integer = 0 To kc - 1
                            Dim src As Integer = ao + i0 + (pc + p) * lda
                            Dim dst As Integer = baseIndex + p * mr
                            For ii As Integer = 0 To rowsHere - 1
                                ap(dst + ii) = a(src + ii)
                            Next
                            For ii As Integer = rowsHere To mr - 1
                                ap(dst + ii) = 0.0
                            Next
                        Next
                    End If
                End Sub
            If parallelize AndAlso slivers > 1 Then
                Global.System.Threading.Tasks.Parallel.For(0, slivers, KernelOptions.CreateParallelOptions(), body)
            Else
                For s As Integer = 0 To slivers - 1
                    body(s)
                Next
            End If
        End Sub

        ''' <summary>Packs rows [pc, pc + kc) and columns [jc, jc + nc) of op(B) into slivers of nr columns: sliver t holds, for each p, its nr values contiguously (zero padded).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub PackB(ByVal tb As Boolean, ByVal b As Double(), ByVal bo As Integer, ByVal ldb As Integer,
                                 ByVal jc As Integer, ByVal nc As Integer, ByVal pc As Integer, ByVal kc As Integer, ByVal nr As Integer,
                                 ByVal bp As Double(), ByVal parallelize As Boolean)
            Dim slivers As Integer = (nc + nr - 1) \ nr
            Dim body As Action(Of Integer) =
                Sub(t As Integer)
                    Dim baseIndex As Integer = t * nr * kc
                    Dim j0 As Integer = jc + t * nr
                    Dim colsHere As Integer = SysMath.Min(nr, jc + nc - j0)
                    For jj As Integer = 0 To nr - 1
                        Dim dst As Integer = baseIndex + jj
                        If jj >= colsHere Then
                            For p As Integer = 0 To kc - 1
                                bp(dst) = 0.0
                                dst += nr
                            Next
                        ElseIf tb Then
                            ' op(B)(p, j) = B(j, p)
                            Dim src As Integer = bo + (j0 + jj) + pc * ldb
                            For p As Integer = 0 To kc - 1
                                bp(dst) = b(src)
                                src += ldb
                                dst += nr
                            Next
                        Else
                            ' op(B)(p, j) = B(p, j): contiguous along p
                            Dim src As Integer = bo + pc + (j0 + jj) * ldb
                            For p As Integer = 0 To kc - 1
                                bp(dst) = b(src + p)
                                dst += nr
                            Next
                        End If
                    Next
                End Sub
            If parallelize AndAlso slivers > 1 Then
                Global.System.Threading.Tasks.Parallel.For(0, slivers, KernelOptions.CreateParallelOptions(), body)
            Else
                For t As Integer = 0 To slivers - 1
                    body(t)
                Next
            End If
        End Sub

        ''' <summary>4 x 4 register-blocked micro-kernel: C(0..mrv, 0..nrv) += alpha * (A sliver x B sliver).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub MicroKernel4x4(ByVal ap As Double(), ByVal ai As Integer, ByVal bp As Double(), ByVal bi As Integer, ByVal kc As Integer,
                                          ByVal c As Double(), ByVal ci As Integer, ByVal ldc As Integer, ByVal alpha As Double,
                                          ByVal mrv As Integer, ByVal nrv As Integer)
            Dim c00 As Double = 0.0, c10 As Double = 0.0, c20 As Double = 0.0, c30 As Double = 0.0
            Dim c01 As Double = 0.0, c11 As Double = 0.0, c21 As Double = 0.0, c31 As Double = 0.0
            Dim c02 As Double = 0.0, c12 As Double = 0.0, c22 As Double = 0.0, c32 As Double = 0.0
            Dim c03 As Double = 0.0, c13 As Double = 0.0, c23 As Double = 0.0, c33 As Double = 0.0
            For p As Integer = 0 To kc - 1
                Dim a0 As Double = ap(ai)
                Dim a1 As Double = ap(ai + 1)
                Dim a2 As Double = ap(ai + 2)
                Dim a3 As Double = ap(ai + 3)
                Dim b0 As Double = bp(bi)
                c00 += a0 * b0
                c10 += a1 * b0
                c20 += a2 * b0
                c30 += a3 * b0
                Dim b1 As Double = bp(bi + 1)
                c01 += a0 * b1
                c11 += a1 * b1
                c21 += a2 * b1
                c31 += a3 * b1
                Dim b2 As Double = bp(bi + 2)
                c02 += a0 * b2
                c12 += a1 * b2
                c22 += a2 * b2
                c32 += a3 * b2
                Dim b3 As Double = bp(bi + 3)
                c03 += a0 * b3
                c13 += a1 * b3
                c23 += a2 * b3
                c33 += a3 * b3
                ai += 4
                bi += 4
            Next
            If mrv = 4 AndAlso nrv = 4 Then
                c(ci) += alpha * c00
                c(ci + 1) += alpha * c10
                c(ci + 2) += alpha * c20
                c(ci + 3) += alpha * c30
                ci += ldc
                c(ci) += alpha * c01
                c(ci + 1) += alpha * c11
                c(ci + 2) += alpha * c21
                c(ci + 3) += alpha * c31
                ci += ldc
                c(ci) += alpha * c02
                c(ci + 1) += alpha * c12
                c(ci + 2) += alpha * c22
                c(ci + 3) += alpha * c32
                ci += ldc
                c(ci) += alpha * c03
                c(ci + 1) += alpha * c13
                c(ci + 2) += alpha * c23
                c(ci + 3) += alpha * c33
                Return
            End If
            Dim acc As Double() = {c00, c10, c20, c30, c01, c11, c21, c31, c02, c12, c22, c32, c03, c13, c23, c33}
            For jj As Integer = 0 To nrv - 1
                For ii As Integer = 0 To mrv - 1
                    c(ci + ii + jj * ldc) += alpha * acc(jj * 4 + ii)
                Next
            Next
        End Sub

#If NET Then
        ''' <summary>SIMD micro-kernel (2 vectors x 4 columns): C(0..mrv, 0..nrv) += alpha * (A sliver x B sliver).</summary>
        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub MicroKernelSimd(ByVal ap As Double(), ByVal ai As Integer, ByVal bp As Double(), ByVal bi As Integer, ByVal kc As Integer,
                                           ByVal c As Double(), ByVal ci As Integer, ByVal ldc As Integer, ByVal alpha As Double,
                                           ByVal mrv As Integer, ByVal nrv As Integer)
            Dim vc As Integer = Global.System.Numerics.Vector(Of Double).Count
            Dim mr As Integer = 2 * vc
            Dim c00 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero, c10 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero
            Dim c01 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero, c11 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero
            Dim c02 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero, c12 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero
            Dim c03 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero, c13 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero
            For p As Integer = 0 To kc - 1
                Dim a0 As New Global.System.Numerics.Vector(Of Double)(ap, ai)
                Dim a1 As New Global.System.Numerics.Vector(Of Double)(ap, ai + vc)
                Dim bv As New Global.System.Numerics.Vector(Of Double)(bp(bi))
                c00 += a0 * bv
                c10 += a1 * bv
                bv = New Global.System.Numerics.Vector(Of Double)(bp(bi + 1))
                c01 += a0 * bv
                c11 += a1 * bv
                bv = New Global.System.Numerics.Vector(Of Double)(bp(bi + 2))
                c02 += a0 * bv
                c12 += a1 * bv
                bv = New Global.System.Numerics.Vector(Of Double)(bp(bi + 3))
                c03 += a0 * bv
                c13 += a1 * bv
                ai += mr
                bi += 4
            Next
            Dim va As New Global.System.Numerics.Vector(Of Double)(alpha)
            If mrv = mr AndAlso nrv = 4 Then
                Dim r As Global.System.Numerics.Vector(Of Double)
                r = New Global.System.Numerics.Vector(Of Double)(c, ci) + va * c00
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Double)(c, ci + vc) + va * c10
                r.CopyTo(c, ci + vc)
                ci += ldc
                r = New Global.System.Numerics.Vector(Of Double)(c, ci) + va * c01
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Double)(c, ci + vc) + va * c11
                r.CopyTo(c, ci + vc)
                ci += ldc
                r = New Global.System.Numerics.Vector(Of Double)(c, ci) + va * c02
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Double)(c, ci + vc) + va * c12
                r.CopyTo(c, ci + vc)
                ci += ldc
                r = New Global.System.Numerics.Vector(Of Double)(c, ci) + va * c03
                r.CopyTo(c, ci)
                r = New Global.System.Numerics.Vector(Of Double)(c, ci + vc) + va * c13
                r.CopyTo(c, ci + vc)
                Return
            End If
            Dim acc As Double() = New Double(mr * 4 - 1) {}
            c00.CopyTo(acc, 0)
            c10.CopyTo(acc, vc)
            c01.CopyTo(acc, mr)
            c11.CopyTo(acc, mr + vc)
            c02.CopyTo(acc, 2 * mr)
            c12.CopyTo(acc, 2 * mr + vc)
            c03.CopyTo(acc, 3 * mr)
            c13.CopyTo(acc, 3 * mr + vc)
            For jj As Integer = 0 To nrv - 1
                For ii As Integer = 0 To mrv - 1
                    c(ci + ii + jj * ldc) += alpha * acc(jj * mr + ii)
                Next
            Next
        End Sub
#End If

#End Region

    End Class

End Namespace
