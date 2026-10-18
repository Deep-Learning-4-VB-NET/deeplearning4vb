Imports System
Imports Lapack = DeepLearning4VB.org.nd4vb.linalg.api.blas.Lapack
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBArraySizeException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBArraySizeException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory

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

Namespace org.nd4vb.linalg.api.blas.impl

    ''' <summary>
    ''' LAPACK routines (Java <c>BaseLapack</c>): the public routines create the <c>INFO</c> and pivot arrays, check the
    ''' data type and call the typed prototypes a backend implements, then report a failure from <c>INFO</c>.
    ''' <para>
    ''' Java threw <c>RuntimeException</c>, here <see cref="InvalidOperationException"/>, for an invalid parameter
    ''' reported by <c>INFO</c>, a non positive definite matrix in <see cref="potrf"/> and the shape checks of
    ''' <see cref="geqrf"/> and <see cref="syev"/>.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseLapack
        Implements Lapack

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BaseLapack))

        ''' <summary>Generate inverse given LU decomp (LAPACK <c>getri</c>).</summary>
        Public MustOverride Sub getri(ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal WORK As INDArray, ByVal lwork As Integer, ByVal INFO As Integer) Implements Lapack.getri

        ''' <summary>A new [1, length] INT array (Java built it from a data buffer and a shape information buffer).</summary>
        Private Shared Function intArray(ByVal length As Integer, ByVal shapeType As DataType) As INDArray
            Return Nd4vbT.createArrayFromShapeBuffer(Nd4vbT.DataBufferFactory.createInt(length), Nd4vbT.ShapeInfoProvider.createShapeInformation(New Long() {1, length}, shapeType).First)
        End Function

        ''' <summary>
        ''' LU decomposition with partial pivoting of A (in place: L below the diagonal with an implied unit diagonal, U on
        ''' and above it). Returns the 1-based pivot indices as a [1, min(m, n)] INT array; a singular matrix is logged.
        ''' </summary>
        Public Overridable Function getrf(ByVal A As INDArray) As INDArray Implements Lapack.getrf
            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            Dim m As Integer = CInt(A.rows())
            Dim n As Integer = CInt(A.columns())

            Dim INFO As INDArray = intArray(1, A.dataType())

            Dim mn As Integer = Math.Min(m, n)
            Dim IPIV As INDArray = intArray(mn, A.dataType())

            If A.data().dataType() = DataType.DOUBLE Then
                dgetrf(m, n, A, IPIV, INFO)
            ElseIf A.data().dataType() = DataType.FLOAT Then
                sgetrf(m, n, A, IPIV, INFO)
            Else
                Throw New NotSupportedException()
            End If

            If INFO.getInt(0) < 0 Then
                Throw New InvalidOperationException("Parameter #" & INFO.getInt(0) & " to getrf() was not valid")
            ElseIf INFO.getInt(0) > 0 Then
                log.warn("The matrix is singular - cannot be used for inverse op. Check L matrix at row " & INFO.getInt(0))
            End If

            Return IPIV
        End Function

        ''' <summary>
        ''' Float/Double versions of LU decomp.
        ''' This is the official LAPACK interface (in case you want to call this directly)
        ''' See getrf for full details on LU Decomp
        ''' </summary>
        ''' <param name="M">  the number of rows in the matrix A </param>
        ''' <param name="N">  the number of cols in the matrix A </param>
        ''' <param name="A">  the matrix to factorize - data must be in column order ( create with 'f' ordering ) </param>
        ''' <param name="IPIV"> an output array for the permutations ( must be int based storage ) </param>
        ''' <param name="INFO"> error details 1 int array, a positive number (i) implies row i cannot be factored, a negative value implies paramtere i is invalid </param>
        Public MustOverride Sub sgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)

        Public MustOverride Sub dgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)

        ''' <summary>
        ''' Cholesky decomposition of the positive definite matrix A (in place): the lower factor L (A = L * L') when
        ''' <paramref name="lower"/> is set, otherwise the upper factor U (A = U' * U); the other triangle is zeroed.
        ''' </summary>
        ''' <exception cref="InvalidOperationException">A parameter was invalid or the matrix is not positive definite.</exception>
        Public Overridable Sub potrf(ByVal A As INDArray, ByVal lower As Boolean) Implements Lapack.potrf

            If A.columns() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            Dim uplo As SByte = CSByte(AscW(If(lower, "L"c, "U"c))) ' upper or lower part of the factor desired ?
            Dim n As Integer = CInt(A.columns())

            Dim INFO As INDArray = intArray(1, A.dataType())

            If A.data().dataType() = DataType.DOUBLE Then
                dpotrf(uplo, n, A, INFO)
            ElseIf A.data().dataType() = DataType.FLOAT Then
                spotrf(uplo, n, A, INFO)
            Else
                Throw New NotSupportedException()
            End If

            If INFO.getInt(0) < 0 Then
                Throw New InvalidOperationException("Parameter #" & INFO.getInt(0) & " to potrf() was not valid")
            ElseIf INFO.getInt(0) > 0 Then
                Throw New InvalidOperationException("The matrix is not positive definite! (potrf fails @ order " & INFO.getInt(0) & ")")
            End If

            Return
        End Sub

        ''' <summary>
        ''' Float/Double versions of cholesky decomp for positive definite matrices
        '''
        '''   A = LL*
        ''' </summary>
        ''' <param name="uplo"> which factor to return L or U </param>
        ''' <param name="A">  the matrix to factorize - data must be in column order ( create with 'f' ordering ) </param>
        ''' <param name="INFO"> error details 1 int array, a positive number (i) implies row i cannot be factored, a negative value implies paramtere i is invalid </param>
        Public MustOverride Sub spotrf(ByVal uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)

        Public MustOverride Sub dpotrf(ByVal uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)

        ''' <summary>
        ''' QR decomposition of A (m x n): A is replaced by Q, and R (n x n) receives the upper triangular factor.
        ''' </summary>
        Public Overridable Sub geqrf(ByVal A As INDArray, ByVal R As INDArray) Implements Lapack.geqrf

            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            Dim m As Integer = CInt(A.rows())
            Dim n As Integer = CInt(A.columns())

            Dim INFO As INDArray = intArray(1, A.dataType())

            If R.rows() <> A.columns() OrElse R.columns() <> A.columns() Then
                Throw New InvalidOperationException("geqrf: R must be N x N (n = columns in A)")
            End If

            If A.data().dataType() = DataType.DOUBLE Then
                dgeqrf(m, n, A, R, INFO)
            ElseIf A.data().dataType() = DataType.FLOAT Then
                sgeqrf(m, n, A, R, INFO)
            Else
                Throw New NotSupportedException()
            End If

            If INFO.getInt(0) < 0 Then
                Throw New InvalidOperationException("Parameter #" & INFO.getInt(0) & " to getrf() was not valid")
            End If
        End Sub

        ''' <summary>
        ''' Float/Double versions of QR decomp.
        ''' This is the official LAPACK interface (in case you want to call this directly)
        ''' See geqrf for full details on LU Decomp
        ''' </summary>
        ''' <param name="M">  the number of rows in the matrix A </param>
        ''' <param name="N">  the number of cols in the matrix A </param>
        ''' <param name="A">  the matrix to factorize - data must be in column order ( create with 'f' ordering ) </param>
        ''' <param name="R">  an output array for other part of factorization </param>
        ''' <param name="INFO"> error details 1 int array, a positive number (i) implies row i cannot be factored, a negative value implies paramtere i is invalid </param>
        Public MustOverride Sub sgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)

        Public MustOverride Sub dgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)

        ''' <summary>
        ''' Eigenvalues (into V) and optionally eigenvectors (into A, <paramref name="jobz"/> = 'V') of the symmetric
        ''' matrix A; returns the LAPACK status.
        ''' </summary>
        Public Overridable Function syev(ByVal jobz As Char, ByVal uplo As Char, ByVal A As INDArray, ByVal V As INDArray) As Integer Implements Lapack.syev

            If A.rows() <> A.columns() Then
                Throw New InvalidOperationException("syev: A must be square.")
            End If
            If A.rows() <> V.length() Then
                Throw New InvalidOperationException("syev: V must be the length of the matrix dimension.")
            End If

            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            Dim status As Integer = -1
            If A.data().dataType() = DataType.DOUBLE Then
                status = dsyev(jobz, uplo, CInt(A.rows()), A, V)
            ElseIf A.data().dataType() = DataType.FLOAT Then
                status = ssyev(jobz, uplo, CInt(A.rows()), A, V)
            Else
                Throw New NotSupportedException()
            End If

            Return status
        End Function

        ''' <summary>
        ''' Float/Double versions of eigen value/vector calc.
        ''' </summary>
        ''' <param name="jobz"> 'N' - no eigen vectors, 'V' - return eigenvectors </param>
        ''' <param name="uplo"> upper or lower part of symmetric matrix to use </param>
        ''' <param name="N">  the number of rows &amp; cols in the matrix A </param>
        ''' <param name="A">  the matrix to calculate eigenvectors </param>
        ''' <param name="R">  an output array for eigenvalues ( may be null ) </param>
        Public MustOverride Function ssyev(ByVal jobz As Char, ByVal uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer

        Public MustOverride Function dsyev(ByVal jobz As Char, ByVal uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer

        ''' <summary>
        ''' Singular value decomposition A = U * S * VT (U and VT may be Nothing: not computed).
        ''' </summary>
        Public Overridable Sub gesvd(ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray) Implements Lapack.gesvd
            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            Dim m As Integer = CInt(A.rows())
            Dim n As Integer = CInt(A.columns())

            Dim jobu As SByte = CSByte(AscW(If(U Is Nothing, "N"c, "A"c)))
            Dim jobvt As SByte = CSByte(AscW(If(VT Is Nothing, "N"c, "A"c)))

            Dim INFO As INDArray = intArray(1, DataType.INT)

            If A.data().dataType() = DataType.DOUBLE Then
                dgesvd(jobu, jobvt, m, n, A, S, U, VT, INFO)
            ElseIf A.data().dataType() = DataType.FLOAT Then
                sgesvd(jobu, jobvt, m, n, A, S, U, VT, INFO)
            Else
                Throw New NotSupportedException()
            End If

            If INFO.getInt(0) < 0 Then
                Throw New InvalidOperationException("Parameter #" & INFO.getInt(0) & " to gesvd() was not valid")
            ElseIf INFO.getInt(0) > 0 Then
                log.warn("The matrix contains singular elements. Check S matrix at row " & INFO.getInt(0))
            End If
        End Sub

        Public MustOverride Sub sgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)

        Public MustOverride Sub dgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)

        ''' <summary>The M x M permutation matrix of the 1-based pivot indices <paramref name="ipiv"/> from <see cref="getrf"/>.</summary>
        Public Overridable Function getPFactor(ByVal M As Integer, ByVal ipiv As INDArray) As INDArray Implements Lapack.getPFactor
            ' The simplest permutation is the identity matrix
            Dim P As INDArray = Nd4vbT.eye(M) ' result is a square matrix with given size
            For i As Integer = 0 To CInt(ipiv.length()) - 1
                Dim pivot As Integer = ipiv.getInt(i) - 1 ' Did we swap row #i with anything?
                If pivot > i Then ' don't reswap when we get lower down in the vector
                    Dim v1 As INDArray = P.getColumn(i).dup() ' because of row vs col major order we'll ...
                    Dim v2 As INDArray = P.getColumn(pivot) ' ... make a transposed matrix immediately
                    P.putColumn(i, v2)
                    P.putColumn(pivot, v1) ' note dup() above is required - getColumn() is a 'view'
                End If
            Next i
            Return P ' the permutation matrix - contains a single 1 in any row and column
        End Function

        '     TODO: consider doing this in place to save memory. This implies U is taken out first
        '       L is the same shape as the input matrix. Just the lower triangular with a diagonal of 1s

        ''' <summary>The unit lower triangular factor L (m x n) of a <see cref="getrf"/> result.</summary>
        Public Overridable Function getLFactor(ByVal A As INDArray) As INDArray Implements Lapack.getLFactor
            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            Dim m As Integer = CInt(A.rows())
            Dim n As Integer = CInt(A.columns())

            Dim L As INDArray = Nd4vbT.create(New Long() {m, n})
            For r As Integer = 0 To m - 1
                For c As Integer = 0 To n - 1
                    If r > c AndAlso r < m AndAlso c < n Then
                        L.putScalar(r, c, A.getFloat(r, c))
                    ElseIf r < c Then
                        L.putScalar(r, c, 0.0F)
                    Else
                        L.putScalar(r, c, 1.0F)
                    End If
                Next c
            Next r
            Return L
        End Function

        ''' <summary>The upper triangular factor U (n x n) of a <see cref="getrf"/> result.</summary>
        Public Overridable Function getUFactor(ByVal A As INDArray) As INDArray Implements Lapack.getUFactor
            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            Dim m As Integer = CInt(A.rows())
            Dim n As Integer = CInt(A.columns())
            Dim U As INDArray = Nd4vbT.create(New Long() {n, n})

            For r As Integer = 0 To n - 1
                For c As Integer = 0 To n - 1
                    If r <= c AndAlso r < m AndAlso c < n Then
                        U.putScalar(r, c, A.getFloat(r, c))
                    Else
                        U.putScalar(r, c, 0.0F)
                    End If
                Next c
            Next r
            Return U
        End Function

    End Class

End Namespace
