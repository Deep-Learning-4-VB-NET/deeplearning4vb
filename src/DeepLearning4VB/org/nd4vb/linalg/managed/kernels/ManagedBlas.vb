' DeepLearning4VB: managed BLAS (levels 1, 2 and gemm) for the ND4VB engine. Shared helpers; the
' Single and Double routines are in ManagedBlas.Level1.vb, ManagedBlas.Level2.vb and ManagedBlas.Level3.vb.

Imports System
Imports System.Threading.Tasks

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' BLAS routines in managed code, with the CBLAS calling conventions ND4J's CPU backend uses:
    ''' matrices are arrays with an element offset and a leading dimension, <c>order</c> is "c"c/"r"c
    ''' (row-major) or "f"c (column-major), <c>trans</c> is "n"c (as is) or "t"c/"c"c (transposed; the
    ''' data is real, so conjugate-transpose is a transpose), all case-insensitive. Vectors take an
    ''' offset and an increment; negative increments walk backwards from the far end as in reference BLAS.
    ''' </summary>
    ''' <remarks>
    ''' <para>Every routine exists for <c>Single()</c> and <c>Double()</c>. gemm packs panels of A and B,
    ''' runs a register-blocked micro-kernel (4x4 scalar; on the .NET 10 build 2*Vector(Of T).Count x 4 with
    ''' <c>System.Numerics.Vector(Of T)</c> when hardware-accelerated) and computes macro tiles in parallel
    ''' above <see cref="KernelOptions.GemmParallelThreshold"/>. Level-1 and gemv use SIMD for unit
    ''' increments on .NET 10 and run in parallel above <see cref="KernelOptions.ParallelThreshold"/>
    ''' (reductions in fixed chunks merged in order, so results are reproducible).</para>
    ''' <para>Results equal a naive loop up to floating-point rounding (summation order differs). With
    ''' beta = 0 the output is overwritten without being read (NaN in C or y does not propagate), as BLAS specifies.</para>
    ''' </remarks>
    Partial Public NotInheritable Class ManagedBlas

        Private Sub New()
        End Sub

        ''' <summary>True for "c"c/"C"c/"r"c/"R"c (row-major), False for "f"c/"F"c (column-major).</summary>
        Friend Shared Function IsRowMajor(ByVal order As Char) As Boolean
            Select Case order
                Case "c"c, "C"c, "r"c, "R"c
                    Return True
                Case "f"c, "F"c
                    Return False
                Case Else
                    Throw New ArgumentException("Order must be 'c' (row-major) or 'f' (column-major), was '" & order & "'", NameOf(order))
            End Select
        End Function

        ''' <summary>True for "t"c/"c"c (transpose), False for "n"c.</summary>
        Friend Shared Function IsTranspose(ByVal trans As Char, ByVal name As String) As Boolean
            Select Case trans
                Case "n"c, "N"c
                    Return False
                Case "t"c, "T"c, "c"c, "C"c
                    Return True
                Case Else
                    Throw New ArgumentException("Transpose flag must be 'n' or 't', was '" & trans & "'", name)
            End Select
        End Function

        ''' <summary>Index of element 0 of a strided vector (reference-BLAS convention for negative increments).</summary>
        Friend Shared Function VectorStart(ByVal n As Integer, ByVal offset As Long, ByVal inc As Integer) As Long
            If inc < 0 AndAlso n > 0 Then
                Return offset - CLng(n - 1) * inc
            End If
            Return offset
        End Function

        ''' <summary>Checks that a strided vector of n elements lies inside an array of the given length.</summary>
        Friend Shared Sub CheckVector(ByVal arrayLength As Integer, ByVal n As Integer, ByVal offset As Long, ByVal inc As Integer, ByVal name As String)
            If n <= 0 Then
                Return
            End If
            If offset < 0 Then
                Throw New ArgumentOutOfRangeException(name, "Negative offset")
            End If
            Dim last As Long = offset + CLng(n - 1) * Math.Abs(CLng(inc))
            If last >= arrayLength Then
                Throw New ArgumentException("Vector " & name & " (offset " & offset & ", " & n & " elements, increment " & inc & ") exceeds its array of length " & arrayLength, name)
            End If
        End Sub

        ''' <summary>Checks a column-major matrix of rows x cols with leading dimension ld inside an array of the given length.</summary>
        Friend Shared Sub CheckMatrix(ByVal arrayLength As Integer, ByVal rows As Integer, ByVal cols As Integer, ByVal offset As Long, ByVal ld As Integer, ByVal name As String)
            If ld < Math.Max(1, rows) Then
                Throw New ArgumentException("Leading dimension of " & name & " (" & ld & ") is smaller than its " & rows & " rows (column-major view)", name)
            End If
            If rows <= 0 OrElse cols <= 0 Then
                Return
            End If
            If offset < 0 Then
                Throw New ArgumentOutOfRangeException(name, "Negative offset")
            End If
            Dim last As Long = offset + (rows - 1) + CLng(cols - 1) * ld
            If last >= arrayLength Then
                Throw New ArgumentException("Matrix " & name & " (" & rows & "x" & cols & ", offset " & offset & ", ld " & ld & ") exceeds its array of length " & arrayLength, name)
            End If
        End Sub

        ''' <summary>Runs body(start, end) over [0, n) in parallel chunks (any order).</summary>
        Friend Shared Sub ParallelRanges(ByVal n As Integer, ByVal body As Action(Of Integer, Integer))
            Dim chunks As Integer = KernelOptions.ChunkCount(n)
            If chunks <= 1 Then
                body(0, n)
                Return
            End If
            Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                Sub(c As Integer)
                    Dim s As Integer = CInt(CLng(n) * c \ chunks)
                    Dim e As Integer = CInt(CLng(n) * (c + 1) \ chunks)
                    If e > s Then
                        body(s, e)
                    End If
                End Sub)
        End Sub

        ''' <summary>Sums body(start, end) over fixed chunks of [0, n), in parallel, adding the partials in chunk order.</summary>
        Friend Shared Function ParallelSum(ByVal n As Integer, ByVal body As Func(Of Integer, Integer, Double)) As Double
            Dim chunks As Integer = ReductionChunks.Count(n)
            If chunks <= 1 Then
                Return body(0, n)
            End If
            Dim partials As Double() = New Double(chunks - 1) {}
            Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                Sub(c As Integer)
                    Dim s As Integer = CInt(ReductionChunks.Start(n, c, chunks))
                    Dim e As Integer = CInt(ReductionChunks.Start(n, c + 1, chunks))
                    partials(c) = body(s, e)
                End Sub)
            Dim total As Double = 0.0
            For Each p As Double In partials
                total += p
            Next
            Return total
        End Function

    End Class

End Namespace
