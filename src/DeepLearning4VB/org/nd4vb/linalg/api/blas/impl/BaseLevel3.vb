Imports System
Imports Level3 = DeepLearning4VB.org.nd4vb.linalg.api.blas.Level3
Imports GemmParams = DeepLearning4VB.org.nd4vb.linalg.api.blas.params.GemmParams
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DefaultOpExecutioner = DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner.DefaultOpExecutioner
Imports OpExecutionerUtil = DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner.OpExecutionerUtil
Imports ND4VBArraySizeException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBArraySizeException
Imports OpProfiler = DeepLearning4VB.org.nd4vb.linalg.profiler.OpProfiler

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
    ''' Base class for level 3 functions, abstract headers pulled from
    ''' http://www.netlib.org/blas/blast-forum/cblas.h (Java <c>BaseLevel3</c>): the public routines check the data
    ''' type of the operands and call the typed prototypes a backend implements.
    ''' <para>
    ''' Differences from Java: both <c>gemm</c> overloads run <c>dgemm</c>/<c>sgemm</c>/<c>hgemm</c> with
    ''' <see cref="GemmParams"/> (the BLAS path Java kept as a comment, with the given alpha and beta; Java ran the
    ''' <c>Mmul</c> custom op, which the managed backend does not implement yet), and a C that is not column-major dense
    ''' is computed in an 'f' copy and assigned back.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseLevel3
        Inherits BaseLevel
        Implements Level3

        Public Overridable Sub gemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal beta As Double, ByVal C As INDArray) Implements Level3.gemm
            If profilingModeAll() Then
                OpProfiler.Instance.processBlasCall(True, A, B, C)
            End If

            Dim params As New GemmParams(A, B, C)

            ' Java ran an Mmul custom op here; this is its earlier BLAS path (kept in Java as a comment, there with
            ' alpha = 1 and beta = 0)
            gemm(Order, params, alpha, beta, C)

            OpExecutionerUtil.checkForAny(C)
        End Sub

        ''' <summary>
        ''' C = alpha * op(A) * op(B) + beta * C, op(X) being X or its transpose as the flags say.
        ''' </summary>
        Public Overridable Sub gemm(ByVal A As INDArray, ByVal B As INDArray, ByVal C As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double) Implements Level3.gemm
            If profilingModeAll() Then
                OpProfiler.Instance.processBlasCall(True, A, B, C)
            End If

            ' Java ran an Mmul custom op here; this is its earlier BLAS path (kept in Java as a comment)
            Dim params As New GemmParams(A, B, C, transposeA, transposeB)
            gemm(A.ordering(), params, alpha, beta, C)

            OpExecutionerUtil.checkForAny(C)
        End Sub


        ''' <summary>
        ''' C = alpha * op(A) * op(B) + beta * C with the operands and flags of <paramref name="params"/>: <c>dgemm</c>,
        ''' <c>sgemm</c> or <c>hgemm</c> by the data type of A. The BLAS call needs C in column-major ('f') layout
        ''' (leading dimension = rows); any other C is computed in an 'f' copy (<c>C.dup('f')</c>) and assigned back.
        ''' </summary>
        Protected Friend Overridable Sub gemm(ByVal Order As Char, ByVal params As GemmParams, ByVal alpha As Double, ByVal beta As Double, ByVal C As INDArray)
            Dim target As INDArray = If(isColumnMajorDense(C), C, C.dup("f"c))
            Dim a As INDArray = params.getA()
            If a.data().dataType() = DataType.DOUBLE Then
                DefaultOpExecutioner.validateDataType(DataType.DOUBLE, params.getA(), params.getB(), target)
                dgemm(Order, params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(), alpha, params.getA(), params.getLda(), params.getB(), params.getLdb(), beta, target, params.getLdc())
            ElseIf a.data().dataType() = DataType.FLOAT Then
                DefaultOpExecutioner.validateDataType(DataType.FLOAT, params.getA(), params.getB(), target)
                sgemm(Order, params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(), CSng(alpha), params.getA(), params.getLda(), params.getB(), params.getLdb(), CSng(beta), target, params.getLdc())
            Else
                DefaultOpExecutioner.validateDataType(DataType.HALF, params.getA(), params.getB(), target)
                hgemm(Order, params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(), CSng(alpha), params.getA(), params.getLda(), params.getB(), params.getLdb(), CSng(beta), target, params.getLdc())
            End If
            If target IsNot C Then
                C.assign(target)
            End If
        End Sub

        ''' <summary>True when element (i, j) of the matrix (a row vector for rank 1) is at i + j * rows of its buffer.</summary>
        Protected Friend Shared Function isColumnMajorDense(ByVal arr As INDArray) As Boolean
            Dim rows As Long = arr.rows()
            Dim cols As Long = arr.columns()
            If arr.rank() = 1 Then
                Return cols <= 1 OrElse arr.stride(0) = 1
            End If
            If arr.rank() <> 2 Then
                Return False
            End If
            Return (rows <= 1 OrElse arr.stride(0) = 1) AndAlso (cols <= 1 OrElse arr.stride(1) = rows)
        End Function

        ''' <summary>
        ''' her2k performs a rank-2k update of an n-by-n Hermitian matrix c, that is, one of the following operations:
        ''' c := alpha*a*conjg(b') + conjg(alpha)*b*conjg(a') + beta*c,  for trans = 'N'or'n'
        ''' c := alpha*conjg(b')*a + conjg(alpha)*conjg(a')*b + beta*c,  for trans = 'C'or'c'
        ''' where c is an n-by-n Hermitian matrix;
        ''' a and b are n-by-k matrices if trans = 'N'or'n',
        ''' a and b are k-by-n matrices if trans = 'C'or'c'. </summary>
        '''  <param name="Order"> </param>
        ''' <param name="Side"> </param>
        ''' <param name="Uplo"> </param>
        ''' <param name="alpha"> </param>
        ''' <param name="A"> </param>
        ''' <param name="B"> </param>
        ''' <param name="beta"> </param>
        ''' <param name="C"> </param>
        Public Overridable Sub symm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal beta As Double, ByVal C As INDArray) Implements Level3.symm
            If profilingModeAll() Then
                OpProfiler.Instance.processBlasCall(False, A, B, C)
            End If

            If C.rows() > Integer.MaxValue OrElse C.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue OrElse C.size(0) > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If A.data().dataType() = DataType.DOUBLE Then
                DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, B, C)
                dsymm(Order, Side, Uplo, CInt(C.rows()), CInt(C.columns()), alpha, A, CInt(A.size(0)), B, CInt(B.size(0)), beta, C, CInt(C.size(0)))
            Else
                DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, B, C)
                ssymm(Order, Side, Uplo, CInt(C.rows()), CInt(C.columns()), CSng(alpha), A, CInt(A.size(0)), B, CInt(B.size(0)), CSng(beta), C, CInt(C.size(0)))
            End If

            OpExecutionerUtil.checkForAny(C)
        End Sub

        ''' <summary>
        ''' syrk performs a rank-n update of an n-by-n symmetric matrix c, that is, one of the following operations:
        ''' c := alpha*a*a' + beta*c  for trans = 'N'or'n'
        ''' c := alpha*a'*a + beta*c  for trans = 'T'or't','C'or'c',
        ''' where c is an n-by-n symmetric matrix;
        ''' a is an n-by-k matrix, if trans = 'N'or'n',
        ''' a is a k-by-n matrix, if trans = 'T'or't','C'or'c'. </summary>
        '''  <param name="Order"> </param>
        ''' <param name="Uplo"> </param>
        ''' <param name="Trans"> </param>
        ''' <param name="alpha"> </param>
        ''' <param name="A"> </param>
        ''' <param name="beta"> </param>
        ''' <param name="C"> </param>
        Public Overridable Sub syrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal beta As Double, ByVal C As INDArray) Implements Level3.syrk
            If profilingModeAll() Then
                OpProfiler.Instance.processBlasCall(False, A, C)
            End If

            If C.rows() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse C.size(0) > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If A.data().dataType() = DataType.DOUBLE Then
                DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, C)
                dsyrk(Order, Uplo, Trans, CInt(C.rows()), 1, alpha, A, CInt(A.size(0)), beta, C, CInt(C.size(0)))
            Else
                DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, C)
                ssyrk(Order, Uplo, Trans, CInt(C.rows()), 1, CSng(alpha), A, CInt(A.size(0)), CSng(beta), C, CInt(C.size(0)))
            End If

            OpExecutionerUtil.checkForAny(C)
        End Sub

        ''' <summary>
        ''' yr2k performs a rank-2k update of an n-by-n symmetric matrix c, that is, one of the following operations:
        ''' c := alpha*a*b' + alpha*b*a' + beta*c  for trans = 'N'or'n'
        ''' c := alpha*a'*b + alpha*b'*a + beta*c  for trans = 'T'or't',
        ''' where c is an n-by-n symmetric matrix;
        ''' a and b are n-by-k matrices, if trans = 'N'or'n',
        ''' a and b are k-by-n matrices, if trans = 'T'or't'. </summary>
        '''  <param name="Order"> </param>
        ''' <param name="Uplo"> </param>
        ''' <param name="Trans"> </param>
        ''' <param name="alpha"> </param>
        ''' <param name="A"> </param>
        ''' <param name="B"> </param>
        ''' <param name="beta"> </param>
        ''' <param name="C"> </param>
        Public Overridable Sub syr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal beta As Double, ByVal C As INDArray) Implements Level3.syr2k
            If profilingModeAll() Then
                OpProfiler.Instance.processBlasCall(False, A, B, C)
            End If

            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue OrElse C.size(0) > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If A.data().dataType() = DataType.DOUBLE Then
                DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, B, C)
                dsyr2k(Order, Uplo, Trans, CInt(A.rows()), CInt(A.columns()), alpha, A, CInt(A.size(0)), B, CInt(B.size(0)), beta, C, CInt(C.size(0)))
            Else
                DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, B, C)
                ssyr2k(Order, Uplo, Trans, CInt(A.rows()), CInt(A.columns()), CSng(alpha), A, CInt(A.size(0)), B, CInt(B.size(0)), CSng(beta), C, CInt(C.size(0)))
            End If

            OpExecutionerUtil.checkForAny(C)
        End Sub

        ''' <summary>
        ''' syr2k performs a rank-2k update of an n-by-n symmetric matrix c, that is, one of the following operations:
        ''' c := alpha*a*b' + alpha*b*a' + beta*c  for trans = 'N'or'n'
        ''' c := alpha*a'*b + alpha*b'*a + beta*c  for trans = 'T'or't',
        ''' where c is an n-by-n symmetric matrix;
        ''' a and b are n-by-k matrices, if trans = 'N'or'n',
        ''' a and b are k-by-n matrices, if trans = 'T'or't'. </summary>
        ''' <param name="Order"> </param>
        ''' <param name="Side"> </param>
        ''' <param name="Uplo"> </param>
        ''' <param name="TransA"> </param>
        ''' <param name="Diag"> </param>
        ''' <param name="alpha"> </param>
        ''' <param name="A"> </param>
        ''' <param name="B"> </param>
        ''' <param name="C"> </param>
        Public Overridable Sub trmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal C As INDArray) Implements Level3.trmm
            If profilingModeAll() Then
                OpProfiler.Instance.processBlasCall(False, A, B, C)
            End If

            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If A.data().dataType() = DataType.DOUBLE Then
                DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, B, C)
                dtrmm(Order, Side, Uplo, TransA, Diag, CInt(A.rows()), CInt(A.columns()), alpha, A, CInt(A.size(0)), B, CInt(B.size(0)))
            Else
                DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, B, C)
                strmm(Order, Side, Uplo, TransA, Diag, CInt(A.rows()), CInt(A.columns()), CSng(alpha), A, CInt(A.size(0)), B, CInt(B.size(0)))
            End If

            OpExecutionerUtil.checkForAny(C)
        End Sub

        ''' <summary>
        ''' ?trsm solves one of the following matrix equations:
        ''' op(a)*x = alpha*b  or  x*op(a) = alpha*b,
        ''' where x and b are m-by-n general matrices, and a is triangular;
        ''' op(a) must be an m-by-m matrix, if side = 'L'or'l'
        ''' op(a) must be an n-by-n matrix, if side = 'R'or'r'.
        ''' For the definition of op(a), see Matrix Arguments.
        ''' The routine overwrites x on b. </summary>
        '''  <param name="Order"> </param>
        ''' <param name="Side"> </param>
        ''' <param name="Uplo"> </param>
        ''' <param name="TransA"> </param>
        ''' <param name="Diag"> </param>
        ''' <param name="alpha"> </param>
        ''' <param name="A"> </param>
        ''' <param name="B"> </param>
        Public Overridable Sub trsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray) Implements Level3.trsm
            If profilingModeAll() Then
                OpProfiler.Instance.processBlasCall(False, A, B)
            End If

            If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If

            If A.data().dataType() = DataType.DOUBLE Then
                DefaultOpExecutioner.validateDataType(DataType.DOUBLE, A, B)
                dtrsm(Order, Side, Uplo, TransA, Diag, CInt(A.rows()), CInt(A.columns()), alpha, A, CInt(A.size(0)), B, CInt(B.size(0)))
            Else
                DefaultOpExecutioner.validateDataType(DataType.FLOAT, A, B)
                strsm(Order, Side, Uplo, TransA, Diag, CInt(A.rows()), CInt(A.columns()), CSng(alpha), A, CInt(A.size(0)), B, CInt(B.size(0)))
            End If

            OpExecutionerUtil.checkForAny(B)
        End Sub

    '
    '     * ===========================================================================
    '     * Prototypes for level 3 BLAS
    '     * ===========================================================================
    '

    '
    '     * Routines with standard 4 prefixes (S, D, C, Z)
    '
        Protected Friend MustOverride Sub hgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)


        Protected Friend MustOverride Sub sgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub ssymm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub ssyrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub ssyr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub strmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)

        Protected Friend MustOverride Sub strsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)

        Protected Friend MustOverride Sub dgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub dsymm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub dsyrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub dsyr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)

        Protected Friend MustOverride Sub dtrmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)

        Protected Friend MustOverride Sub dtrsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)
    End Class

End Namespace
