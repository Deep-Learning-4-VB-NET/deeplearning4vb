' DeepLearning4VB: managed BLAS level 1 (dot, axpy, scal, asum, nrm2, iamax, copy, swap) for the ND4VB engine.
' The Single and Double sections are the same code for the two precisions; change both together.

Imports System
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.kernels

    Partial Public NotInheritable Class ManagedBlas

#Region "Level 1 (Single)"

        ''' <summary>Dot product: the sum of x(i) * y(i) over n elements.</summary>
        Public Shared Function Dot(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer,
                                   ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer) As Single
            If n <= 0 Then
                Return 0.0F
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xs As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim ys As Integer = CInt(VectorStart(n, yOffset, incY))
            If KernelOptions.ShouldParallelize(n) Then
                Return CSng(ParallelSum(n, Function(s As Integer, e As Integer) DotRange(x, xs, incX, y, ys, incY, s, e)))
            End If
            Return CSng(DotRange(x, xs, incX, y, ys, incY, 0, n))
        End Function

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Function DotRange(ByVal x As Single(), ByVal xs As Integer, ByVal incX As Integer, ByVal y As Single(), ByVal ys As Integer, ByVal incY As Integer,
                                         ByVal s As Integer, ByVal e As Integer) As Double
            Dim acc As Double = 0.0
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso incY = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Single).Count
                Dim v0 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero
                Dim v1 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero
                Do While i + 2 * vc <= e
                    v0 += New Global.System.Numerics.Vector(Of Single)(x, xs + i) * New Global.System.Numerics.Vector(Of Single)(y, ys + i)
                    v1 += New Global.System.Numerics.Vector(Of Single)(x, xs + i + vc) * New Global.System.Numerics.Vector(Of Single)(y, ys + i + vc)
                    i += 2 * vc
                Loop
                acc = Global.System.Numerics.Vector.Sum(v0 + v1)
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Dim yi As Integer = ys + i * incY
            Do While i < e
                acc += CDbl(x(xi)) * CDbl(y(yi))
                xi += incX
                yi += incY
                i += 1
            Loop
            Return acc
        End Function

        ''' <summary>y = alpha * x + y over n elements (nothing happens when alpha is 0, as in reference BLAS).</summary>
        Public Shared Sub Axpy(ByVal n As Integer, ByVal alpha As Single, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer)
            If n <= 0 OrElse alpha = 0.0F Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xs As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim ys As Integer = CInt(VectorStart(n, yOffset, incY))
            If KernelOptions.ShouldParallelize(n) Then
                ParallelRanges(n, Sub(s As Integer, e As Integer) AxpyRange(alpha, x, xs, incX, y, ys, incY, s, e))
            Else
                AxpyRange(alpha, x, xs, incX, y, ys, incY, 0, n)
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub AxpyRange(ByVal alpha As Single, ByVal x As Single(), ByVal xs As Integer, ByVal incX As Integer, ByVal y As Single(), ByVal ys As Integer, ByVal incY As Integer,
                                     ByVal s As Integer, ByVal e As Integer)
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso incY = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Single).Count
                Dim va As New Global.System.Numerics.Vector(Of Single)(alpha)
                Do While i + vc <= e
                    Dim r As Global.System.Numerics.Vector(Of Single) = New Global.System.Numerics.Vector(Of Single)(y, ys + i) + va * New Global.System.Numerics.Vector(Of Single)(x, xs + i)
                    r.CopyTo(y, ys + i)
                    i += vc
                Loop
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Dim yi As Integer = ys + i * incY
            Do While i < e
                y(yi) += alpha * x(xi)
                xi += incX
                yi += incY
                i += 1
            Loop
        End Sub

        ''' <summary>x = alpha * x over n elements (nothing happens for a non-positive increment, as in reference BLAS).</summary>
        Public Shared Sub Scal(ByVal n As Integer, ByVal alpha As Single, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer)
            If n <= 0 OrElse incX <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            Dim xs As Integer = CInt(xOffset)
            If KernelOptions.ShouldParallelize(n) Then
                ParallelRanges(n, Sub(s As Integer, e As Integer) ScalRange(alpha, x, xs, incX, s, e))
            Else
                ScalRange(alpha, x, xs, incX, 0, n)
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub ScalRange(ByVal alpha As Single, ByVal x As Single(), ByVal xs As Integer, ByVal incX As Integer, ByVal s As Integer, ByVal e As Integer)
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Single).Count
                Dim va As New Global.System.Numerics.Vector(Of Single)(alpha)
                Do While i + vc <= e
                    Dim r As Global.System.Numerics.Vector(Of Single) = va * New Global.System.Numerics.Vector(Of Single)(x, xs + i)
                    r.CopyTo(x, xs + i)
                    i += vc
                Loop
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Do While i < e
                x(xi) *= alpha
                xi += incX
                i += 1
            Loop
        End Sub

        ''' <summary>Sum of absolute values over n elements (0 for a non-positive increment).</summary>
        Public Shared Function Asum(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer) As Single
            If n <= 0 OrElse incX <= 0 Then
                Return 0.0F
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            Dim xs As Integer = CInt(xOffset)
            If KernelOptions.ShouldParallelize(n) Then
                Return CSng(ParallelSum(n, Function(s As Integer, e As Integer) AsumRange(x, xs, incX, s, e)))
            End If
            Return CSng(AsumRange(x, xs, incX, 0, n))
        End Function

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Function AsumRange(ByVal x As Single(), ByVal xs As Integer, ByVal incX As Integer, ByVal s As Integer, ByVal e As Integer) As Double
            Dim acc As Double = 0.0
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Single).Count
                Dim v0 As Global.System.Numerics.Vector(Of Single) = Global.System.Numerics.Vector(Of Single).Zero
                Do While i + vc <= e
                    v0 += Global.System.Numerics.Vector.Abs(New Global.System.Numerics.Vector(Of Single)(x, xs + i))
                    i += vc
                Loop
                acc = Global.System.Numerics.Vector.Sum(v0)
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Do While i < e
                acc += SysMath.Abs(CDbl(x(xi)))
                xi += incX
                i += 1
            Loop
            Return acc
        End Function

        ''' <summary>Euclidean norm sqrt(sum x(i)^2) over n elements, without intermediate overflow (0 for a non-positive increment).</summary>
        Public Shared Function Nrm2(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer) As Single
            If n <= 0 OrElse incX <= 0 Then
                Return 0.0F
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            ' scaled sum of squares (LAPACK's dnrm2 recurrence): scale * sqrt(ssq)
            Dim scale As Double = 0.0
            Dim ssq As Double = 1.0
            Dim xi As Integer = CInt(xOffset)
            For i As Integer = 0 To n - 1
                Dim v As Double = x(xi)
                If v <> 0.0 Then
                    Dim av As Double = SysMath.Abs(v)
                    If scale < av Then
                        Dim r As Double = scale / av
                        ssq = 1.0 + ssq * r * r
                        scale = av
                    Else
                        Dim r As Double = av / scale
                        ssq += r * r
                    End If
                End If
                xi += incX
            Next
            Return CSng(scale * SysMath.Sqrt(ssq))
        End Function

        ''' <summary>
        ''' Index (0-based, as CBLAS returns it) of the first element with the largest absolute value;
        ''' 0 when n &lt; 1 or the increment is not positive, as CBLAS does.
        ''' </summary>
        Public Shared Function Iamax(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer) As Integer
            If n <= 0 OrElse incX <= 0 Then
                Return 0
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            Dim xi As Integer = CInt(xOffset)
            Dim best As Integer = 0
            Dim bestValue As Single = SysMath.Abs(x(xi))
            For i As Integer = 1 To n - 1
                xi += incX
                Dim v As Single = SysMath.Abs(x(xi))
                If v > bestValue Then
                    bestValue = v
                    best = i
                End If
            Next
            Return best
        End Function

        ''' <summary>y = x over n elements.</summary>
        Public Shared Sub Copy(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer)
            If n <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            If incX = 1 AndAlso incY = 1 Then
                Array.Copy(x, CInt(xOffset), y, CInt(yOffset), n)
                Return
            End If
            Dim xi As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim yi As Integer = CInt(VectorStart(n, yOffset, incY))
            For i As Integer = 0 To n - 1
                y(yi) = x(xi)
                xi += incX
                yi += incY
            Next
        End Sub

        ''' <summary>Exchanges x and y over n elements.</summary>
        Public Shared Sub Swap(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer)
            If n <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xi As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim yi As Integer = CInt(VectorStart(n, yOffset, incY))
            For i As Integer = 0 To n - 1
                Dim t As Single = x(xi)
                x(xi) = y(yi)
                y(yi) = t
                xi += incX
                yi += incY
            Next
        End Sub

#End Region

#Region "Level 1 (Double)"

        ''' <summary>Dot product: the sum of x(i) * y(i) over n elements.</summary>
        Public Shared Function Dot(ByVal n As Integer, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer,
                                   ByVal y As Double(), ByVal yOffset As Long, ByVal incY As Integer) As Double
            If n <= 0 Then
                Return 0.0
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xs As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim ys As Integer = CInt(VectorStart(n, yOffset, incY))
            If KernelOptions.ShouldParallelize(n) Then
                Return CDbl(ParallelSum(n, Function(s As Integer, e As Integer) DotRange(x, xs, incX, y, ys, incY, s, e)))
            End If
            Return CDbl(DotRange(x, xs, incX, y, ys, incY, 0, n))
        End Function

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Function DotRange(ByVal x As Double(), ByVal xs As Integer, ByVal incX As Integer, ByVal y As Double(), ByVal ys As Integer, ByVal incY As Integer,
                                         ByVal s As Integer, ByVal e As Integer) As Double
            Dim acc As Double = 0.0
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso incY = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Double).Count
                Dim v0 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero
                Dim v1 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero
                Do While i + 2 * vc <= e
                    v0 += New Global.System.Numerics.Vector(Of Double)(x, xs + i) * New Global.System.Numerics.Vector(Of Double)(y, ys + i)
                    v1 += New Global.System.Numerics.Vector(Of Double)(x, xs + i + vc) * New Global.System.Numerics.Vector(Of Double)(y, ys + i + vc)
                    i += 2 * vc
                Loop
                acc = Global.System.Numerics.Vector.Sum(v0 + v1)
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Dim yi As Integer = ys + i * incY
            Do While i < e
                acc += CDbl(x(xi)) * CDbl(y(yi))
                xi += incX
                yi += incY
                i += 1
            Loop
            Return acc
        End Function

        ''' <summary>y = alpha * x + y over n elements (nothing happens when alpha is 0, as in reference BLAS).</summary>
        Public Shared Sub Axpy(ByVal n As Integer, ByVal alpha As Double, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal y As Double(), ByVal yOffset As Long, ByVal incY As Integer)
            If n <= 0 OrElse alpha = 0.0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xs As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim ys As Integer = CInt(VectorStart(n, yOffset, incY))
            If KernelOptions.ShouldParallelize(n) Then
                ParallelRanges(n, Sub(s As Integer, e As Integer) AxpyRange(alpha, x, xs, incX, y, ys, incY, s, e))
            Else
                AxpyRange(alpha, x, xs, incX, y, ys, incY, 0, n)
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub AxpyRange(ByVal alpha As Double, ByVal x As Double(), ByVal xs As Integer, ByVal incX As Integer, ByVal y As Double(), ByVal ys As Integer, ByVal incY As Integer,
                                     ByVal s As Integer, ByVal e As Integer)
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso incY = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Double).Count
                Dim va As New Global.System.Numerics.Vector(Of Double)(alpha)
                Do While i + vc <= e
                    Dim r As Global.System.Numerics.Vector(Of Double) = New Global.System.Numerics.Vector(Of Double)(y, ys + i) + va * New Global.System.Numerics.Vector(Of Double)(x, xs + i)
                    r.CopyTo(y, ys + i)
                    i += vc
                Loop
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Dim yi As Integer = ys + i * incY
            Do While i < e
                y(yi) += alpha * x(xi)
                xi += incX
                yi += incY
                i += 1
            Loop
        End Sub

        ''' <summary>x = alpha * x over n elements (nothing happens for a non-positive increment, as in reference BLAS).</summary>
        Public Shared Sub Scal(ByVal n As Integer, ByVal alpha As Double, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer)
            If n <= 0 OrElse incX <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            Dim xs As Integer = CInt(xOffset)
            If KernelOptions.ShouldParallelize(n) Then
                ParallelRanges(n, Sub(s As Integer, e As Integer) ScalRange(alpha, x, xs, incX, s, e))
            Else
                ScalRange(alpha, x, xs, incX, 0, n)
            End If
        End Sub

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Sub ScalRange(ByVal alpha As Double, ByVal x As Double(), ByVal xs As Integer, ByVal incX As Integer, ByVal s As Integer, ByVal e As Integer)
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Double).Count
                Dim va As New Global.System.Numerics.Vector(Of Double)(alpha)
                Do While i + vc <= e
                    Dim r As Global.System.Numerics.Vector(Of Double) = va * New Global.System.Numerics.Vector(Of Double)(x, xs + i)
                    r.CopyTo(x, xs + i)
                    i += vc
                Loop
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Do While i < e
                x(xi) *= alpha
                xi += incX
                i += 1
            Loop
        End Sub

        ''' <summary>Sum of absolute values over n elements (0 for a non-positive increment).</summary>
        Public Shared Function Asum(ByVal n As Integer, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer) As Double
            If n <= 0 OrElse incX <= 0 Then
                Return 0.0
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            Dim xs As Integer = CInt(xOffset)
            If KernelOptions.ShouldParallelize(n) Then
                Return CDbl(ParallelSum(n, Function(s As Integer, e As Integer) AsumRange(x, xs, incX, s, e)))
            End If
            Return CDbl(AsumRange(x, xs, incX, 0, n))
        End Function

        <Global.System.Runtime.CompilerServices.MethodImpl(HotLoop)>
        Private Shared Function AsumRange(ByVal x As Double(), ByVal xs As Integer, ByVal incX As Integer, ByVal s As Integer, ByVal e As Integer) As Double
            Dim acc As Double = 0.0
            Dim i As Integer = s
#If NET Then
            If incX = 1 AndAlso KernelOptions.SimdEnabled Then
                Dim vc As Integer = Global.System.Numerics.Vector(Of Double).Count
                Dim v0 As Global.System.Numerics.Vector(Of Double) = Global.System.Numerics.Vector(Of Double).Zero
                Do While i + vc <= e
                    v0 += Global.System.Numerics.Vector.Abs(New Global.System.Numerics.Vector(Of Double)(x, xs + i))
                    i += vc
                Loop
                acc = Global.System.Numerics.Vector.Sum(v0)
            End If
#End If
            Dim xi As Integer = xs + i * incX
            Do While i < e
                acc += SysMath.Abs(CDbl(x(xi)))
                xi += incX
                i += 1
            Loop
            Return acc
        End Function

        ''' <summary>Euclidean norm sqrt(sum x(i)^2) over n elements, without intermediate overflow (0 for a non-positive increment).</summary>
        Public Shared Function Nrm2(ByVal n As Integer, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer) As Double
            If n <= 0 OrElse incX <= 0 Then
                Return 0.0
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            ' scaled sum of squares (LAPACK's dnrm2 recurrence): scale * sqrt(ssq)
            Dim scale As Double = 0.0
            Dim ssq As Double = 1.0
            Dim xi As Integer = CInt(xOffset)
            For i As Integer = 0 To n - 1
                Dim v As Double = x(xi)
                If v <> 0.0 Then
                    Dim av As Double = SysMath.Abs(v)
                    If scale < av Then
                        Dim r As Double = scale / av
                        ssq = 1.0 + ssq * r * r
                        scale = av
                    Else
                        Dim r As Double = av / scale
                        ssq += r * r
                    End If
                End If
                xi += incX
            Next
            Return CDbl(scale * SysMath.Sqrt(ssq))
        End Function

        ''' <summary>
        ''' Index (0-based, as CBLAS returns it) of the first element with the largest absolute value;
        ''' 0 when n &lt; 1 or the increment is not positive, as CBLAS does.
        ''' </summary>
        Public Shared Function Iamax(ByVal n As Integer, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer) As Integer
            If n <= 0 OrElse incX <= 0 Then
                Return 0
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            Dim xi As Integer = CInt(xOffset)
            Dim best As Integer = 0
            Dim bestValue As Double = SysMath.Abs(x(xi))
            For i As Integer = 1 To n - 1
                xi += incX
                Dim v As Double = SysMath.Abs(x(xi))
                If v > bestValue Then
                    bestValue = v
                    best = i
                End If
            Next
            Return best
        End Function

        ''' <summary>y = x over n elements.</summary>
        Public Shared Sub Copy(ByVal n As Integer, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal y As Double(), ByVal yOffset As Long, ByVal incY As Integer)
            If n <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            If incX = 1 AndAlso incY = 1 Then
                Array.Copy(x, CInt(xOffset), y, CInt(yOffset), n)
                Return
            End If
            Dim xi As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim yi As Integer = CInt(VectorStart(n, yOffset, incY))
            For i As Integer = 0 To n - 1
                y(yi) = x(xi)
                xi += incX
                yi += incY
            Next
        End Sub

        ''' <summary>Exchanges x and y over n elements.</summary>
        Public Shared Sub Swap(ByVal n As Integer, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer,
                               ByVal y As Double(), ByVal yOffset As Long, ByVal incY As Integer)
            If n <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xi As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim yi As Integer = CInt(VectorStart(n, yOffset, incY))
            For i As Integer = 0 To n - 1
                Dim t As Double = x(xi)
                x(xi) = y(yi)
                y(yi) = t
                xi += incX
                yi += incY
            Next
        End Sub

#End Region

#Region "Level 1 additions: rot"

        ''' <summary>
        ''' Plane rotation of n point pairs: (x(i), y(i)) becomes (c * x(i) + s * y(i), c * y(i) - s * x(i)), as BLAS
        ''' <c>srot</c> computes it (negative increments walk backwards from the far end).
        ''' </summary>
        Public Shared Sub Rot(ByVal n As Integer, ByVal x As Single(), ByVal xOffset As Long, ByVal incX As Integer,
                              ByVal y As Single(), ByVal yOffset As Long, ByVal incY As Integer, ByVal c As Single, ByVal s As Single)
            If n <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xi As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim yi As Integer = CInt(VectorStart(n, yOffset, incY))
            For i As Integer = 0 To n - 1
                Dim xv As Single = x(xi)
                Dim yv As Single = y(yi)
                x(xi) = c * xv + s * yv
                y(yi) = c * yv - s * xv
                xi += incX
                yi += incY
            Next
        End Sub

        ''' <summary>
        ''' Plane rotation of n point pairs: (x(i), y(i)) becomes (c * x(i) + s * y(i), c * y(i) - s * x(i)), as BLAS
        ''' <c>drot</c> computes it (negative increments walk backwards from the far end).
        ''' </summary>
        Public Shared Sub Rot(ByVal n As Integer, ByVal x As Double(), ByVal xOffset As Long, ByVal incX As Integer,
                              ByVal y As Double(), ByVal yOffset As Long, ByVal incY As Integer, ByVal c As Double, ByVal s As Double)
            If n <= 0 Then
                Return
            End If
            CheckVector(x.Length, n, xOffset, incX, NameOf(x))
            CheckVector(y.Length, n, yOffset, incY, NameOf(y))
            Dim xi As Integer = CInt(VectorStart(n, xOffset, incX))
            Dim yi As Integer = CInt(VectorStart(n, yOffset, incY))
            For i As Integer = 0 To n - 1
                Dim xv As Double = x(xi)
                Dim yv As Double = y(yi)
                x(xi) = c * xv + s * yv
                y(yi) = c * yv - s * xv
                xi += incX
                yi += incY
            Next
        End Sub

#End Region

    End Class

End Namespace
