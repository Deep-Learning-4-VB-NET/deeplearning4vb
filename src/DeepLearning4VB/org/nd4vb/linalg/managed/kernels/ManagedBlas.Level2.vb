' DeepLearning4VB: managed BLAS level 2 (gemv) for the ND4VB engine.
' The Single and Double sections are the same code for the two precisions; change both together.

Imports System

Namespace org.nd4vb.linalg.managed.kernels

    Partial Public NotInheritable Class ManagedBlas

#Region "Level 2 (Single)"

        ''' <summary>
        ''' y = alpha * op(A) * x + beta * y, where A is an m x n matrix in the given order with leading
        ''' dimension lda and op(A) is A or its transpose (x has n elements and y m, or the other way round
        ''' when transposed). With beta = 0, y is overwritten without being read.
        ''' </summary>
        Public Shared Sub Gemv(ByVal order As Char, ByVal trans As Char, ByVal m As Integer, ByVal n As Integer, ByVal alpha As Single,
                               ByVal a As Single(), ByVal aOffset As Long, ByVal lda As Integer,
                               ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal beta As Single, ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer)
            Dim t As Boolean = IsTranspose(trans, NameOf(trans))
            If m < 0 OrElse n < 0 Then
                Throw New ArgumentException("Matrix dimensions must not be negative")
            End If
            If incX = 0 OrElse incY = 0 Then
                Throw New ArgumentException("Vector increments must not be 0")
            End If
            ' work on the column-major view: a row-major m x n matrix is a column-major n x m one, transposed
            Dim rows As Integer = m
            Dim cols As Integer = n
            If IsRowMajor(order) Then
                rows = n
                cols = m
                t = Not t
            End If
            CheckMatrix(a.Length, rows, cols, aOffset, lda, NameOf(a))
            Dim lenX As Integer = If(t, rows, cols)
            Dim lenY As Integer = If(t, cols, rows)
            CheckVector(x.Length, lenX, xOffset, incX, NameOf(x))
            CheckVector(y.Length, lenY, yOffset, incY, NameOf(y))
            If lenY = 0 Then
                Return
            End If
            Dim xs As Integer = CInt(VectorStart(lenX, xOffset, incX))
            Dim ys As Integer = CInt(VectorStart(lenY, yOffset, incY))
            If beta <> 1.0F Then
                Dim yi As Integer = ys
                For i As Integer = 0 To lenY - 1
                    y(yi) = If(beta = 0.0F, 0.0F, beta * y(yi))
                    yi += incY
                Next
            End If
            If alpha = 0.0F OrElse lenX = 0 Then
                Return
            End If
            Dim ao As Integer = CInt(aOffset)
            Dim parallelize As Boolean = KernelOptions.ShouldParallelize(CLng(rows) * cols)
            If Not t Then
                ' y(i) += alpha * sum_j A(i, j) * x(j): column sweeps over row blocks
                Dim body As Action(Of Integer, Integer) =
                    Sub(r0 As Integer, r1 As Integer)
                        GemvRows(rows, cols, alpha, a, ao, lda, x, xs, incX, y, ys, incY, r0, r1)
                    End Sub
                If parallelize Then
                    ParallelRanges(rows, body)
                Else
                    body(0, rows)
                End If
            Else
                ' y(j) += alpha * dot(A(:, j), x)
                Dim body As Action(Of Integer, Integer) =
                    Sub(c0 As Integer, c1 As Integer)
                        For j As Integer = c0 To c1 - 1
                            Dim s As Double = DotRange(a, ao + j * lda, 1, x, xs, incX, 0, rows)
                            y(ys + j * incY) += CSng(alpha * s)
                        Next
                    End Sub
                If parallelize Then
                    ParallelRanges(cols, body)
                Else
                    body(0, cols)
                End If
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub GemvRows(ByVal rows As Integer, ByVal cols As Integer, ByVal alpha As Single, ByVal a As Single(), ByVal ao As Integer, ByVal lda As Integer,
                                    ByVal x As Single(), ByVal xs As Integer, ByVal incX As Integer, ByVal y As Single(), ByVal ys As Integer, ByVal incY As Integer,
                                    ByVal r0 As Integer, ByVal r1 As Integer)
            Dim xi As Integer = xs
            For j As Integer = 0 To cols - 1
                Dim tj As Single = alpha * x(xi)
                xi += incX
                Dim col As Integer = ao + j * lda
                Dim i As Integer = r0
#If NET Then
                If incY = 1 AndAlso KernelOptions.SimdEnabled Then
                    Dim vc As Integer = Global.System.Numerics.Vector(Of Single).Count
                    Dim vt As New Global.System.Numerics.Vector(Of Single)(tj)
                    Do While i + vc <= r1
                        Dim r As Global.System.Numerics.Vector(Of Single) = New Global.System.Numerics.Vector(Of Single)(y, ys + i) + vt * New Global.System.Numerics.Vector(Of Single)(a, col + i)
                        r.CopyTo(y, ys + i)
                        i += vc
                    Loop
                End If
#End If
                Dim yi As Integer = ys + i * incY
                Do While i < r1
                    y(yi) += tj * a(col + i)
                    yi += incY
                    i += 1
                Loop
            Next
        End Sub

#End Region

#Region "Level 2 (Double)"

        ''' <summary>
        ''' y = alpha * op(A) * x + beta * y, where A is an m x n matrix in the given order with leading
        ''' dimension lda and op(A) is A or its transpose (x has n elements and y m, or the other way round
        ''' when transposed). With beta = 0, y is overwritten without being read.
        ''' </summary>
        Public Shared Sub Gemv(ByVal order As Char, ByVal trans As Char, ByVal m As Integer, ByVal n As Integer, ByVal alpha As Double,
                               ByVal a As Double(), ByVal aOffset As Long, ByVal lda As Integer,
                               ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal beta As Double, ByVal y As Double(), ByVal yOffset As Long, ByVal incY As Integer)
            Dim t As Boolean = IsTranspose(trans, NameOf(trans))
            If m < 0 OrElse n < 0 Then
                Throw New ArgumentException("Matrix dimensions must not be negative")
            End If
            If incX = 0 OrElse incY = 0 Then
                Throw New ArgumentException("Vector increments must not be 0")
            End If
            ' work on the column-major view: a row-major m x n matrix is a column-major n x m one, transposed
            Dim rows As Integer = m
            Dim cols As Integer = n
            If IsRowMajor(order) Then
                rows = n
                cols = m
                t = Not t
            End If
            CheckMatrix(a.Length, rows, cols, aOffset, lda, NameOf(a))
            Dim lenX As Integer = If(t, rows, cols)
            Dim lenY As Integer = If(t, cols, rows)
            CheckVector(x.Length, lenX, xOffset, incX, NameOf(x))
            CheckVector(y.Length, lenY, yOffset, incY, NameOf(y))
            If lenY = 0 Then
                Return
            End If
            Dim xs As Integer = CInt(VectorStart(lenX, xOffset, incX))
            Dim ys As Integer = CInt(VectorStart(lenY, yOffset, incY))
            If beta <> 1.0 Then
                Dim yi As Integer = ys
                For i As Integer = 0 To lenY - 1
                    y(yi) = If(beta = 0.0, 0.0, beta * y(yi))
                    yi += incY
                Next
            End If
            If alpha = 0.0 OrElse lenX = 0 Then
                Return
            End If
            Dim ao As Integer = CInt(aOffset)
            Dim parallelize As Boolean = KernelOptions.ShouldParallelize(CLng(rows) * cols)
            If Not t Then
                ' y(i) += alpha * sum_j A(i, j) * x(j): column sweeps over row blocks
                Dim body As Action(Of Integer, Integer) =
                    Sub(r0 As Integer, r1 As Integer)
                        GemvRows(rows, cols, alpha, a, ao, lda, x, xs, incX, y, ys, incY, r0, r1)
                    End Sub
                If parallelize Then
                    ParallelRanges(rows, body)
                Else
                    body(0, rows)
                End If
            Else
                ' y(j) += alpha * dot(A(:, j), x)
                Dim body As Action(Of Integer, Integer) =
                    Sub(c0 As Integer, c1 As Integer)
                        For j As Integer = c0 To c1 - 1
                            Dim s As Double = DotRange(a, ao + j * lda, 1, x, xs, incX, 0, rows)
                            y(ys + j * incY) += CDbl(alpha * s)
                        Next
                    End Sub
                If parallelize Then
                    ParallelRanges(cols, body)
                Else
                    body(0, cols)
                End If
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub GemvRows(ByVal rows As Integer, ByVal cols As Integer, ByVal alpha As Double, ByVal a As Double(), ByVal ao As Integer, ByVal lda As Integer,
                                    ByVal x As Double(), ByVal xs As Integer, ByVal incX As Integer, ByVal y As Double(), ByVal ys As Integer, ByVal incY As Integer,
                                    ByVal r0 As Integer, ByVal r1 As Integer)
            Dim xi As Integer = xs
            For j As Integer = 0 To cols - 1
                Dim tj As Double = alpha * x(xi)
                xi += incX
                Dim col As Integer = ao + j * lda
                Dim i As Integer = r0
#If NET Then
                If incY = 1 AndAlso KernelOptions.SimdEnabled Then
                    Dim vc As Integer = Global.System.Numerics.Vector(Of Double).Count
                    Dim vt As New Global.System.Numerics.Vector(Of Double)(tj)
                    Do While i + vc <= r1
                        Dim r As Global.System.Numerics.Vector(Of Double) = New Global.System.Numerics.Vector(Of Double)(y, ys + i) + vt * New Global.System.Numerics.Vector(Of Double)(a, col + i)
                        r.CopyTo(y, ys + i)
                        i += vc
                    Loop
                End If
#End If
                Dim yi As Integer = ys + i * incY
                Do While i < r1
                    y(yi) += tj * a(col + i)
                    yi += incY
                    i += 1
                Loop
            Next
        End Sub

#End Region

#Region "Level 2 additions: ger"

        ''' <summary>
        ''' Rank-1 update A = alpha * x * y' + A, where A is an m x n matrix in the given order with leading dimension
        ''' lda, x has m elements and y n (BLAS <c>sger</c>; nothing happens when alpha is 0, and columns whose y element is
        ''' 0 are skipped, as in reference BLAS).
        ''' </summary>
        Public Shared Sub Ger(ByVal order As Char, ByVal m As Integer, ByVal n As Integer, ByVal alpha As Single,
                              ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer,
                              ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer,
                              ByVal a As Single(), ByVal aOffset As Long, ByVal lda As Integer)
            If m < 0 OrElse n < 0 Then
                Throw New ArgumentException("Matrix dimensions must not be negative")
            End If
            If incX = 0 OrElse incY = 0 Then
                Throw New ArgumentException("Vector increments must not be 0")
            End If
            ' a row-major m x n matrix is a column-major n x m one: A' = alpha * y * x' + A'
            If IsRowMajor(order) Then
                Ger("f"c, n, m, alpha, y, yOffset, incY, x, xOffset, incX, a, aOffset, lda)
                Return
            End If
            CheckMatrix(a.Length, m, n, aOffset, lda, NameOf(a))
            If m = 0 OrElse n = 0 OrElse alpha = 0.0F Then
                Return
            End If
            CheckVector(x.Length, m, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xs As Integer = CInt(VectorStart(m, xOffset, incX))
            Dim yj As Integer = CInt(VectorStart(n, yOffset, incY))
            For j As Integer = 0 To n - 1
                Dim t As Single = alpha * y(yj)
                If t <> 0.0F Then
                    Dim col As Integer = CInt(aOffset + CLng(j) * lda)
                    Dim xi As Integer = xs
                    For i As Integer = 0 To m - 1
                        a(col + i) += x(xi) * t
                        xi += incX
                    Next
                End If
                yj += incY
            Next
        End Sub

        ''' <summary>
        ''' Rank-1 update A = alpha * x * y' + A, where A is an m x n matrix in the given order with leading dimension
        ''' lda, x has m elements and y n (BLAS <c>dger</c>; nothing happens when alpha is 0, and columns whose y element is
        ''' 0 are skipped, as in reference BLAS).
        ''' </summary>
        Public Shared Sub Ger(ByVal order As Char, ByVal m As Integer, ByVal n As Integer, ByVal alpha As Double,
                              ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer,
                              ByVal y As Double(), ByVal yOffset As Long, ByVal incY As Integer,
                              ByVal a As Double(), ByVal aOffset As Long, ByVal lda As Integer)
            If m < 0 OrElse n < 0 Then
                Throw New ArgumentException("Matrix dimensions must not be negative")
            End If
            If incX = 0 OrElse incY = 0 Then
                Throw New ArgumentException("Vector increments must not be 0")
            End If
            ' a row-major m x n matrix is a column-major n x m one: A' = alpha * y * x' + A'
            If IsRowMajor(order) Then
                Ger("f"c, n, m, alpha, y, yOffset, incY, x, xOffset, incX, a, aOffset, lda)
                Return
            End If
            CheckMatrix(a.Length, m, n, aOffset, lda, NameOf(a))
            If m = 0 OrElse n = 0 OrElse alpha = 0.0 Then
                Return
            End If
            CheckVector(x.Length, m, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xs As Integer = CInt(VectorStart(m, xOffset, incX))
            Dim yj As Integer = CInt(VectorStart(n, yOffset, incY))
            For j As Integer = 0 To n - 1
                Dim t As Double = alpha * y(yj)
                If t <> 0.0 Then
                    Dim col As Integer = CInt(aOffset + CLng(j) * lda)
                    Dim xi As Integer = xs
                    For i As Integer = 0 To m - 1
                        a(col + i) += x(xi) * t
                        xi += incX
                    Next
                End If
                yj += incY
            Next
        End Sub

#End Region

    End Class

End Namespace
