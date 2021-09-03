Imports val = lombok.val
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports BaseLevel3 = org.nd4j.linalg.api.blas.impl.BaseLevel3
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AggregateGEMM = org.nd4j.linalg.api.ops.aggregates.impl.AggregateGEMM
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBlas = org.nd4j.nativeblas.Nd4jBlas
Imports org.bytedeco.openblas.global.openblas_nolapack
Imports org.nd4j.linalg.cpu.nativecpu.blas.CpuBlas

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

Namespace org.nd4j.linalg.cpu.nativecpu.blas



	''' 
	''' <summary>
	''' A jblas delgation for level 3 routines
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class CpuLevel3
		Inherits BaseLevel3

		Private nd4jBlas As Nd4jBlas = DirectCast(Nd4j.factory().blas(), Nd4jBlas)

		Protected Friend Overrides Sub hgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)

			'if (true) {
				Dim fA As val = A.castTo(DataType.FLOAT)
				Dim fB As val = B.castTo(DataType.FLOAT)
				Dim fC As val = C.castTo(DataType.FLOAT)

				sgemm(Order, TransA, TransB, M, N, K, alpha, fA, lda, fB, ldb, beta, fC, ldc)

				C.assign(fC)
	'        } else {
	'            // TODO: uncomment this once we have optimized gemm calls
	'            val t = MMulTranspose.builder()
	'                    .transposeA(false)
	'                    .transposeB(false)
	'                    .transposeResult(false)
	'                    .build();
	'            val op = new Mmul(A, B, C, t);
	'            Nd4j.exec(op);
	'        }
	'         
		End Sub

		Protected Friend Overrides Sub sgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)
			If Not Nd4j.FallbackModeEnabled Then
				cblas_sgemm(convertOrder(AscW("f"c)), convertTranspose(AscW(TransA)), convertTranspose(AscW(TransB)), M, N, K, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(B.data().addressPointer(), FloatPointer), ldb, beta, CType(C.data().addressPointer(), FloatPointer), ldc)
			Else
				Nd4j.Executioner.exec(New AggregateGEMM(AscW("f"c), AscW(TransA), AscW(TransB), M, N, K, alpha, A, lda, B, ldb, beta, C, ldc))
			End If
		End Sub

		Protected Friend Overrides Sub ssymm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)
			cblas_ssymm(convertOrder(AscW("f"c)), convertSide(AscW(Side)), convertUplo(AscW(Uplo)), M, N, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(B.data().addressPointer(), FloatPointer), ldb, beta, CType(C.data().addressPointer(), FloatPointer), ldc)
		End Sub

		Protected Friend Overrides Sub ssyrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)
			cblas_ssyrk(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(Trans)), N, K, alpha, CType(A.data().addressPointer(), FloatPointer), lda, beta, CType(C.data().addressPointer(), FloatPointer), ldc)
		End Sub

		Protected Friend Overrides Sub ssyr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)
			cblas_ssyr2k(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(Trans)), N, K, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(B.data().addressPointer(), FloatPointer), ldb, beta, CType(C.data().addressPointer(), FloatPointer), ldc)
		End Sub

		Protected Friend Overrides Sub strmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)
			cblas_strmm(convertOrder(AscW("f"c)), convertSide(AscW(Side)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), Diag, M, N, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(B.data().addressPointer(), FloatPointer), ldb)
		End Sub

		Protected Friend Overrides Sub strsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)
			cblas_strsm(convertOrder(AscW("f"c)), convertSide(AscW(Side)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), Diag, M, N, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(B.data().addressPointer(), FloatPointer), ldb)
		End Sub

		Protected Friend Overrides Sub dgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)
			If Not Nd4j.FallbackModeEnabled Then
				cblas_dgemm(convertOrder(AscW("f"c)), convertTranspose(AscW(TransA)), convertTranspose(AscW(TransB)), M, N, K, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(B.data().addressPointer(), DoublePointer), ldb, beta, CType(C.data().addressPointer(), DoublePointer), ldc)
			Else
				Nd4j.Executioner.exec(New AggregateGEMM(AscW("f"c), AscW(TransA), AscW(TransB), M, N, K, alpha, A, lda, B, ldb, beta, C, ldc))
			End If
		End Sub

		Protected Friend Overrides Sub dsymm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)
			cblas_dsymm(convertOrder(AscW("f"c)), convertSide(AscW(Side)), convertUplo(AscW(Uplo)), M, N, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(B.data().addressPointer(), DoublePointer), ldb, beta, CType(C.data().addressPointer(), DoublePointer), ldc)
		End Sub

		Protected Friend Overrides Sub dsyrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)
			cblas_dsyrk(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(Trans)), N, K, alpha, CType(A.data().addressPointer(), DoublePointer), lda, beta, CType(C.data().addressPointer(), DoublePointer), ldc)
		End Sub

		Protected Friend Overrides Sub dsyr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)
			cblas_dsyr2k(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(Trans)), N, K, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(B.data().addressPointer(), DoublePointer), ldb, beta, CType(C.data().addressPointer(), DoublePointer), ldc)
		End Sub

		Protected Friend Overrides Sub dtrmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)
			cblas_dtrmm(convertOrder(AscW("f"c)), convertSide(AscW(Side)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), Diag, M, N, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(B.data().addressPointer(), DoublePointer), ldb)
		End Sub

		Protected Friend Overrides Sub dtrsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)
			cblas_dtrsm(convertOrder(AscW("f"c)), convertSide(AscW(Side)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), Diag, M, N, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(B.data().addressPointer(), DoublePointer), ldb)
		End Sub
	End Class

End Namespace