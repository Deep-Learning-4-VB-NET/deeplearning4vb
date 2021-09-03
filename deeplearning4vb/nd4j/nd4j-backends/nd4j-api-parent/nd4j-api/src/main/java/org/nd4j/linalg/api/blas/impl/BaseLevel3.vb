Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Level3 = org.nd4j.linalg.api.blas.Level3
Imports GemmParams = org.nd4j.linalg.api.blas.params.GemmParams
Imports MMulTranspose = org.nd4j.linalg.api.blas.params.MMulTranspose
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DefaultOpExecutioner = org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports OpExecutionerUtil = org.nd4j.linalg.api.ops.executioner.OpExecutionerUtil
Imports Mmul = org.nd4j.linalg.api.ops.impl.reduce.Mmul
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler

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

Namespace org.nd4j.linalg.api.blas.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseLevel3 extends BaseLevel implements org.nd4j.linalg.api.blas.Level3
	Public MustInherit Class BaseLevel3
		Inherits BaseLevel
		Implements Level3

		Public Overridable Sub gemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal beta As Double, ByVal C As INDArray) Implements Level3.gemm
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(True, A, B, C)
			End If

			Dim params As New GemmParams(A, B, C)

			Nd4j.exec(New Mmul(A, B, C, alpha, beta, MMulTranspose.builder().transposeA(False).transposeB(False).build()))

	'        
	'        int charOder = Order;
	'        if (A.data().dataType() == DataType.DOUBLE) {
	'            DefaultOpExecutioner.validateDataType(DataType.DOUBLE, params.getA(), params.getB(), params.getC());
	'            dgemm(Order, params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(), 1.0,
	'                            params.getA(), params.getLda(), params.getB(), params.getLdb(), 0, C, params.getLdc());
	'        } else if (A.data().dataType() == DataType.FLOAT) {
	'            DefaultOpExecutioner.validateDataType(DataType.FLOAT, params.getA(), params.getB(), params.getC());
	'            sgemm(Order, params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(), 1.0f,
	'                            params.getA(), params.getLda(), params.getB(), params.getLdb(), 0, C, params.getLdc());
	'        } else {
	'            DefaultOpExecutioner.validateDataType(DataType.HALF, params.getA(), params.getB(), params.getC());
	'            hgemm(Order, params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(), 1.0f,
	'                            params.getA(), params.getLda(), params.getB(), params.getLdb(), 0, C, params.getLdc());
	'        }
	'        

			OpExecutionerUtil.checkForAny(C)
		End Sub

		''' <summary>
		'''{@inheritDoc}
		''' </summary>
		Public Overridable Sub gemm(ByVal A As INDArray, ByVal B As INDArray, ByVal C As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double) Implements Level3.gemm
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(True, A, B, C)
			End If

			Nd4j.exec(New Mmul(A, B, C, alpha, beta, MMulTranspose.builder().transposeA(transposeA).transposeB(transposeB).build()))

	'        
	'        GemmParams params = new GemmParams(A, B, C, transposeA, transposeB);
	'        if (A.data().dataType() == DataType.DOUBLE) {
	'            DefaultOpExecutioner.validateDataType(DataType.DOUBLE, params.getA(), params.getB(), C);
	'            dgemm(A.ordering(), params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(),
	'                            alpha, params.getA(), params.getLda(), params.getB(), params.getLdb(), beta, C,
	'                            params.getLdc());
	'        } else if (A.data().dataType() == DataType.FLOAT) {
	'            DefaultOpExecutioner.validateDataType(DataType.FLOAT, params.getA(), params.getB(), C);
	'            sgemm(A.ordering(), params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(),
	'                            (float) alpha, params.getA(), params.getLda(), params.getB(), params.getLdb(), (float) beta,
	'                            C, params.getLdc());
	'        } else {
	'            DefaultOpExecutioner.validateDataType(DataType.HALF, params.getA(), params.getB(), C);
	'            hgemm(A.ordering(), params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(),
	'                            (float) alpha, params.getA(), params.getLda(), params.getB(), params.getLdb(), (float) beta,
	'                            C, params.getLdc());
	'        }
	'
			OpExecutionerUtil.checkForAny(C)
		End Sub


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
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, B, C)
			End If

			If C.rows() > Integer.MaxValue OrElse C.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue OrElse C.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
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
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, C)
			End If

			If C.rows() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse C.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
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
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, B, C)
			End If

			If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue OrElse C.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
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
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, B, C)
			End If

			If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
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
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, A, B)
			End If

			If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue OrElse A.size(0) > Integer.MaxValue OrElse B.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
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