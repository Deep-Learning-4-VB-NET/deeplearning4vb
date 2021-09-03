Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.api.blas

	Public Interface Level2
		''' <summary>
		''' gemv computes a matrix-vector product using a general matrix and performs one of the following matrix-vector operations: 
		''' y := alpha*a*x + beta*y  for trans = 'N'or'n'; 
		''' y := alpha*a'*x + beta*y  for trans = 'T'or't'; 
		''' y := alpha*conjg(a')*x + beta*y  for trans = 'C'or'c'. 
		''' Here a is an m-by-n band matrix, x and y are vectors, alpha and beta are scalars. </summary>
		''' <param name="order"> </param>
		''' <param name="transA"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Sub gemv(ByVal order As Char, ByVal transA As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray)

		''' <summary>
		''' gbmv computes a matrix-vector product using a general band matrix and performs one of the following matrix-vector operations:
		''' y := alpha*a*x + beta*y  for trans = 'N'or'n';
		''' y := alpha*a'*x + beta*y  for trans = 'T'or't';
		''' y := alpha*conjg(a')*x + beta*y  for trans = 'C'or'c'.
		''' Here a is an m-by-n band matrix with ku superdiagonals and kl subdiagonals, x and y are vectors, alpha and beta are scalars. </summary>
		''' <param name="order"> </param>
		''' <param name="TransA"> </param>
		''' <param name="KL"> </param>
		''' <param name="KU"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Sub gbmv(ByVal order As Char, ByVal TransA As Char, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray)


		''' <summary>
		'''  performs a rank-1 update of a general m-by-n matrix a:
		''' a := alpha*x*y' + a. </summary>
		''' <param name="order"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="A"> </param>
		Sub ger(ByVal order As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray, ByVal A As INDArray)

		''' <summary>
		''' sbmv computes a matrix-vector product using a symmetric band matrix:
		''' y := alpha*a*x + beta*y.
		''' Here a is an n-by-n symmetric band matrix with k superdiagonals, x and y are n-element vectors, alpha and beta are scalars. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Sub sbmv(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray)

		''' 
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="Ap"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Sub spmv(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal Ap As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray)

		''' <summary>
		''' spr performs a rank-1 update of an n-by-n packed symmetric matrix a:
		''' a := alpha*x*x' + a. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Ap"> </param>
		Sub spr(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Ap As INDArray)

		''' <summary>
		''' ?spr2 performs a rank-2 update of an n-by-n packed symmetric matrix a:
		''' a := alpha*x*y' + alpha*y*x' + a. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="A"> </param>
		Sub spr2(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray, ByVal A As INDArray)

		''' <summary>
		''' symv computes a matrix-vector product for a symmetric matrix:
		''' y := alpha*a*x + beta*y.
		''' Here a is an n-by-n symmetric matrix; x and y are n-element vectors, alpha and beta are scalars. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		''' <param name="beta"> </param>
		''' <param name="Y"> </param>
		Sub symv(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal X As INDArray, ByVal beta As Double, ByVal Y As INDArray)

		''' <summary>
		''' syr performs a rank-1 update of an n-by-n symmetric matrix a:
		''' a := alpha*x*x' + a. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="N"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="A"> </param>
		Sub syr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal A As INDArray)

		''' 
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="A"> </param>
		Sub syr2(ByVal order As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray, ByVal A As INDArray)

		''' <summary>
		''' syr2 performs a rank-2 update of an n-by-n symmetric matrix a:
		''' a := alpha*x*y' + alpha*y*x' + a. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Sub tbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray)

		''' <summary>
		''' ?tbsv solves a system of linear equations whose coefficients are in a triangular band matrix. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Sub tbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray)

		''' <summary>
		''' tpmv computes a matrix-vector product using a triangular packed matrix. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="Ap"> </param>
		''' <param name="X"> </param>
		Sub tpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal Ap As INDArray, ByVal X As INDArray)

		''' <summary>
		''' tpsv solves a system of linear equations whose coefficients are in a triangular packed matrix. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="Ap"> </param>
		''' <param name="X"> </param>
		Sub tpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal Ap As INDArray, ByVal X As INDArray)

		''' <summary>
		''' trmv computes a matrix-vector product using a triangular matrix. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Sub trmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray)

		''' <summary>
		''' trsv solves a system of linear equations whose coefficients are in a triangular matrix. </summary>
		''' <param name="order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="A"> </param>
		''' <param name="X"> </param>
		Sub trsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal A As INDArray, ByVal X As INDArray)
	End Interface

End Namespace