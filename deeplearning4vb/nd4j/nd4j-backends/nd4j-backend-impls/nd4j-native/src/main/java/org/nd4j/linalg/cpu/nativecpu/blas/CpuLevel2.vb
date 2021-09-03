Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports BaseLevel2 = org.nd4j.linalg.api.blas.impl.BaseLevel2
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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



	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class CpuLevel2
		Inherits BaseLevel2

		Private nd4jBlas As Nd4jBlas = DirectCast(Nd4j.factory().blas(), Nd4jBlas)

		Protected Friend Overrides Sub sgemv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_sgemv(convertOrder(AscW("f"c)), convertTranspose(AscW(TransA)), M, N, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(X.data().addressPointer(), FloatPointer), incX, beta, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Sub

		Protected Friend Overrides Sub sgbmv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_sgbmv(convertOrder(AscW("f"c)), convertTranspose(AscW(TransA)), M, N, KL, KU, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(X.data().addressPointer(), FloatPointer), incX, beta, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Sub

		Protected Friend Overrides Sub strmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub stbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			cblas_stbmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, K, CType(A.data().addressPointer(), FloatPointer), lda, CType(X.data().addressPointer(), FloatPointer), incX)
		End Sub

		Protected Friend Overrides Sub stpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			cblas_stpmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, CType(Ap.data().addressPointer(), FloatPointer), CType(X.data().addressPointer(), FloatPointer), incX)
		End Sub

		Protected Friend Overrides Sub strsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			cblas_strsv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, CType(A.data().addressPointer(), FloatPointer), lda, CType(X.data().addressPointer(), FloatPointer), incX)
		End Sub

		Protected Friend Overrides Sub stbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			cblas_stbsv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, K, CType(A.data().addressPointer(), FloatPointer), lda, CType(X.data().addressPointer(), FloatPointer), incX)

		End Sub

		Protected Friend Overrides Sub stpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			cblas_stpsv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, CType(Ap.data().addressPointer(), FloatPointer), CType(X.data().addressPointer(), FloatPointer), incX)
		End Sub

		Protected Friend Overrides Sub dgemv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_dgemv(convertOrder(AscW("f"c)), convertTranspose(AscW(TransA)), M, N, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX, beta, CType(Y.data().addressPointer(), DoublePointer), incY)
		End Sub

		Protected Friend Overrides Sub dgbmv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_dgbmv(convertOrder(AscW("f"c)), convertTranspose(AscW(TransA)), M, N, KL, KU, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX, beta, CType(Y.data().addressPointer(), DoublePointer), incY)
		End Sub

		Protected Friend Overrides Sub dtrmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			cblas_dtrmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX)
		End Sub

		Protected Friend Overrides Sub dtbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			cblas_dtbmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, K, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX)
		End Sub

		Protected Friend Overrides Sub dtpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			cblas_dtpmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, CType(Ap.data().addressPointer(), DoublePointer), CType(X.data().addressPointer(), DoublePointer), incX)
		End Sub

		Protected Friend Overrides Sub dtrsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			cblas_dtrsv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX)
		End Sub

		Protected Friend Overrides Sub dtbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			cblas_dtbsv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, K, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX)
		End Sub

		Protected Friend Overrides Sub dtpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			cblas_dtpsv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), N, CType(Ap.data().addressPointer(), DoublePointer), CType(X.data().addressPointer(), DoublePointer), incX)
		End Sub

		Protected Friend Overrides Sub ssymv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_ssymv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(X.data().addressPointer(), FloatPointer), incX, beta, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Sub

		Protected Friend Overrides Sub ssbmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_ssbmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, K, alpha, CType(A.data().addressPointer(), FloatPointer), lda, CType(X.data().addressPointer(), FloatPointer), incX, beta, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Sub

		Protected Friend Overrides Sub sspmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_sspmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(Ap.data().addressPointer(), FloatPointer), CType(X.data().addressPointer(), FloatPointer), incX, beta, CType(Y.data().addressPointer(), FloatPointer), incY)

		End Sub

		Protected Friend Overrides Sub sger(ByVal order As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			cblas_sger(convertOrder(AscW("f"c)), M, N, alpha, CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY, CType(A.data().addressPointer(), FloatPointer), lda)
		End Sub

		Protected Friend Overrides Sub ssyr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal A As INDArray, ByVal lda As Integer)
			cblas_ssyr(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), FloatPointer), incX, CType(A.data().addressPointer(), FloatPointer), lda)
		End Sub

		Protected Friend Overrides Sub sspr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Ap As INDArray)
			cblas_sspr(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), FloatPointer), incX, CType(Ap.data().addressPointer(), FloatPointer))
		End Sub

		Protected Friend Overrides Sub ssyr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			cblas_ssyr2(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY, CType(A.data().addressPointer(), FloatPointer), lda)
		End Sub

		Protected Friend Overrides Sub sspr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray)
			cblas_sspr2(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY, CType(A.data().addressPointer(), FloatPointer))
		End Sub

		Protected Friend Overrides Sub dsymv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_dsymv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX, beta, CType(Y.data().addressPointer(), DoublePointer), incY)
		End Sub

		Protected Friend Overrides Sub dsbmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_dsbmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, K, alpha, CType(A.data().addressPointer(), DoublePointer), lda, CType(X.data().addressPointer(), DoublePointer), incX, beta, CType(Y.data().addressPointer(), DoublePointer), incY)
		End Sub

		Protected Friend Overrides Sub dspmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_dspmv(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(Ap.data().addressPointer(), DoublePointer), CType(X.data().addressPointer(), DoublePointer), incX, beta, CType(Y.data().addressPointer(), DoublePointer), incY)
		End Sub

		Protected Friend Overrides Sub dger(ByVal order As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			cblas_dger(convertOrder(AscW("f"c)), M, N, alpha, CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY, CType(A.data().addressPointer(), DoublePointer), lda)
		End Sub

		Protected Friend Overrides Sub dsyr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal A As INDArray, ByVal lda As Integer)
			cblas_dsyr(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), DoublePointer), incX, CType(A.data().addressPointer(), DoublePointer), lda)
		End Sub

		Protected Friend Overrides Sub dspr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Ap As INDArray)
			cblas_dspr(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), DoublePointer), incX, CType(Ap.data().addressPointer(), DoublePointer))
		End Sub

		Protected Friend Overrides Sub dsyr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			cblas_dsyr2(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY, CType(A.data().addressPointer(), DoublePointer), lda)
		End Sub

		Protected Friend Overrides Sub dspr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray)
			cblas_dspr2(convertOrder(AscW("f"c)), convertUplo(AscW(Uplo)), N, alpha, CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY, CType(A.data().addressPointer(), DoublePointer))
		End Sub
	End Class

End Namespace