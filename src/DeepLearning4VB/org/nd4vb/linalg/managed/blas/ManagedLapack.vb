' DeepLearning4VB: LAPACK of the managed ND4VB backend (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.blas.CpuLapack.

Imports System
Imports BlasException = DeepLearning4VB.org.nd4vb.linalg.api.blas.BlasException
Imports BaseLapack = DeepLearning4VB.org.nd4vb.linalg.api.blas.impl.BaseLapack
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports ShapeInfoHelper = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ShapeInfoHelper
Imports ManagedStorage = DeepLearning4VB.org.nd4vb.linalg.managed.storage.ManagedStorage

Namespace org.nd4vb.linalg.managed.blas

    ''' <summary>
    ''' LAPACK of the managed backend (Java <c>CpuLapack</c>): simple managed versions of the LU decomposition with partial
    ''' pivoting (<c>sgetrf</c>/<c>dgetrf</c>, LAPACK's unblocked <c>getf2</c> algorithm) and the Cholesky decomposition
    ''' (<c>spotrf</c>/<c>dpotrf</c>), meant for small matrices. They work in place on any 2-D layout ('c', 'f' or strided
    ''' views), compute in double and store with the matrix's type.
    ''' </summary>
    ''' <remarks>
    ''' <para>As LAPACK does, <c>getrf</c> writes the 1-based pivot rows to IPIV and stops nowhere: a zero pivot sets INFO to
    ''' its 1-based column (the first one), and the decomposition continues. <c>potrf</c> zeroes the triangle that is not
    ''' the factor, as Java did, and throws <see cref="BlasException"/> with the 1-based order of the first non-positive
    ''' pivot when the matrix is not positive definite (Java threw for LAPACK's status).</para>
    ''' <para>Differences from Java: <c>getrf</c> fills INFO (Java ignored LAPACK's status unless it was negative, so a
    ''' singular matrix was never reported); <c>getri</c> throws <see cref="NotSupportedException"/> naming milestone M2
    ''' (Java's CPU version did nothing). The QR decomposition (<c>geqrf</c>), the eigen decomposition (<c>syev</c>) and the
    ''' singular value decomposition (<c>gesvd</c>) have no managed version yet and throw
    ''' <see cref="NotSupportedException"/> naming milestone M2.</para>
    ''' </remarks>
    Public Class ManagedLapack
        Inherits BaseLapack

        ''' <summary>A 2-D matrix over a managed storage: element (i, j) is at Offset + i * RowStride + j * ColumnStride.</summary>
        Private NotInheritable Class Matrix
            Friend ReadOnly Storage As ManagedStorage
            Friend ReadOnly Offset As Long
            Friend ReadOnly RowStride As Long
            Friend ReadOnly ColumnStride As Long

            Friend Sub New(ByVal arr As INDArray, ByVal name As String)
                Dim buffer As ManagedDataBuffer = BlasOperands.managedBuffer(arr.data(), name)
                Dim info As Long() = arr.shapeInfoJava()
                Dim rank As Integer = CInt(info(0))
                Storage = buffer.Storage
                Offset = buffer.StorageOffset
                If rank = 2 Then
                    RowStride = info(1 + rank)
                    ColumnStride = info(2 + rank)
                ElseIf rank = 1 Then
                    ' a vector is a row
                    RowStride = 0
                    ColumnStride = info(1 + rank)
                Else
                    Throw New ND4VBIllegalStateException(name & " must be a matrix, got rank " & rank)
                End If
            End Sub

            Default Friend Property Item(ByVal i As Integer, ByVal j As Integer) As Double
                Get
                    Return Storage.GetDouble(Offset + i * RowStride + j * ColumnStride)
                End Get
                Set(ByVal value As Double)
                    Storage.SetDouble(Offset + i * RowStride + j * ColumnStride, value)
                End Set
            End Property
        End Class

        ''' <summary>Writes <paramref name="value"/> to element <paramref name="index"/> (row-major logical) of an integer array.</summary>
        Private Shared Sub putInt(ByVal arr As INDArray, ByVal index As Long, ByVal value As Integer, ByVal name As String)
            Dim buffer As ManagedDataBuffer = BlasOperands.managedBuffer(arr.data(), name)
            buffer.Storage.SetLong(buffer.StorageOffset + ShapeInfoHelper.OffsetOf(arr.shapeInfoJava(), index), value)
        End Sub

        '=========================
        ' L U DECOMP

        Public Overrides Sub sgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)
            luDecompose(M, N, A, IPIV, INFO)
        End Sub

        Public Overrides Sub dgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)
            luDecompose(M, N, A, IPIV, INFO)
        End Sub

        Private Shared Sub luDecompose(ByVal m As Integer, ByVal n As Integer, ByVal arr As INDArray, ByVal pivots As INDArray, ByVal infoArray As INDArray)
            If m < 0 OrElse n < 0 Then
                Throw New BlasException("Failed to execute getrf", If(m < 0, -1, -2))
            End If
            Dim a As New Matrix(arr, "A")
            Dim mn As Integer = Math.Min(m, n)
            Dim info As Integer = 0
            For k As Integer = 0 To mn - 1
                ' pivot: the largest magnitude in column k at or below the diagonal (the first one on ties)
                Dim p As Integer = k
                Dim best As Double = Math.Abs(a(k, k))
                For i As Integer = k + 1 To m - 1
                    Dim v As Double = Math.Abs(a(i, k))
                    If v > best Then
                        best = v
                        p = i
                    End If
                Next i
                putInt(pivots, k, p + 1, "IPIV")
                If a(p, k) <> 0.0 Then
                    If p <> k Then
                        For j As Integer = 0 To n - 1
                            Dim t As Double = a(k, j)
                            a(k, j) = a(p, j)
                            a(p, j) = t
                        Next j
                    End If
                    Dim pivot As Double = a(k, k)
                    For i As Integer = k + 1 To m - 1
                        a(i, k) = a(i, k) / pivot
                    Next i
                ElseIf info = 0 Then
                    info = k + 1
                End If
                ' rank-1 update of the trailing matrix
                For j As Integer = k + 1 To n - 1
                    Dim akj As Double = a(k, j)
                    If akj <> 0.0 Then
                        For i As Integer = k + 1 To m - 1
                            a(i, j) = a(i, j) - a(i, k) * akj
                        Next i
                    End If
                Next j
            Next k
            If infoArray IsNot Nothing Then
                putInt(infoArray, 0, info, "INFO")
            End If
        End Sub

        '=========================
        ' Q R DECOMP

        Public Overrides Sub sgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)
            Throw BlasOperands.notImplemented("sgeqrf")
        End Sub

        Public Overrides Sub dgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)
            Throw BlasOperands.notImplemented("dgeqrf")
        End Sub

        '=========================
        ' CHOLESKY DECOMP

        Public Overrides Sub spotrf(ByVal uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)
            cholesky(uplo, N, A, "spotrf")
        End Sub

        Public Overrides Sub dpotrf(ByVal uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)
            cholesky(uplo, N, A, "dpotrf")
        End Sub

        Private Shared Sub cholesky(ByVal uplo As SByte, ByVal n As Integer, ByVal arr As INDArray, ByVal routine As String)
            Dim upper As Boolean
            Select Case ChrW(uplo)
                Case "U"c, "u"c
                    upper = True
                Case "L"c, "l"c
                    upper = False
                Case Else
                    Throw New BlasException("Failed to execute " & routine, -1)
            End Select
            If n < 0 Then
                Throw New BlasException("Failed to execute " & routine, -2)
            End If
            Dim a As New Matrix(arr, "A")
            ' lower: A = L * L'; upper: A = U' * U with U = L', read from and written to the upper triangle
            For j As Integer = 0 To n - 1
                Dim s As Double = getTri(a, upper, j, j)
                For k As Integer = 0 To j - 1
                    Dim l As Double = getTri(a, upper, j, k)
                    s -= l * l
                Next k
                If s <= 0.0 OrElse Double.IsNaN(s) Then
                    Throw New BlasException("Failed to execute " & routine, j + 1)
                End If
                Dim d As Double = Math.Sqrt(s)
                setTri(a, upper, j, j, d)
                For i As Integer = j + 1 To n - 1
                    Dim t As Double = getTri(a, upper, i, j)
                    For k As Integer = 0 To j - 1
                        t -= getTri(a, upper, i, k) * getTri(a, upper, j, k)
                    Next k
                    setTri(a, upper, i, j, t / d)
                Next i
            Next j
            ' zero the other triangle
            For i As Integer = 0 To n - 1
                For j As Integer = i + 1 To n - 1
                    If upper Then
                        a(j, i) = 0.0
                    Else
                        a(i, j) = 0.0
                    End If
                Next j
            Next i
        End Sub

        ''' <summary>Element (i, j), i &gt;= j, of the lower factor: A(i, j) for "L", A(j, i) for "U".</summary>
        Private Shared Function getTri(ByVal a As Matrix, ByVal upper As Boolean, ByVal i As Integer, ByVal j As Integer) As Double
            Return If(upper, a(j, i), a(i, j))
        End Function

        Private Shared Sub setTri(ByVal a As Matrix, ByVal upper As Boolean, ByVal i As Integer, ByVal j As Integer, ByVal value As Double)
            If upper Then
                a(j, i) = value
            Else
                a(i, j) = value
            End If
        End Sub

        '=========================
        ' U S V' DECOMP  (aka SVD)

        Public Overrides Sub sgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)
            Throw BlasOperands.notImplemented("sgesvd")
        End Sub

        Public Overrides Sub dgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)
            Throw BlasOperands.notImplemented("dgesvd")
        End Sub

        '=========================
        ' syev EigenValue/Vectors

        Public Overrides Function ssyev(ByVal jobz As Char, ByVal uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer
            Throw BlasOperands.notImplemented("ssyev")
        End Function

        Public Overrides Function dsyev(ByVal jobz As Char, ByVal uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer
            Throw BlasOperands.notImplemented("dsyev")
        End Function

        ''' <summary>
        ''' Generate inverse given LU decomp (not implemented in the managed backend yet; Java's CPU version did nothing).
        ''' </summary>
        Public Overrides Sub getri(ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal WORK As INDArray, ByVal lwork As Integer, ByVal INFO As Integer)
            Throw BlasOperands.notImplemented("getri")
        End Sub

    End Class

End Namespace
