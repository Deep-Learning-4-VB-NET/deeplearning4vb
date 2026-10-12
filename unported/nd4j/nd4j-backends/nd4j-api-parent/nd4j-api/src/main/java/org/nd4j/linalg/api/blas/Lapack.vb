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

	Public Interface Lapack

		''' <summary>
		''' LU decomposiiton of a matrix
		''' Factorize a matrix A
		''' 
		''' The matrix A is overridden by the L & U combined.
		''' The permutation results are returned directly as a vector. To 
		''' create the permutation matrix use getPFactor method
		''' To split out the L & U matrix use getLFactor and getUFactor methods
		''' 
		''' getrf = triangular factorization (TRF) of a general matrix (GE)
		''' </summary>
		''' <param name="A"> the input matrix, it will be overwritten with the factors
		''' @returns Permutation array </param>
		''' <exception cref="Error"> - with a message to indicate failure (usu. bad params) </exception>
		Function getrf(ByVal A As INDArray) As INDArray



		''' <summary>
		''' QR decomposiiton of a matrix
		''' Factorize a matrix A such that A = QR
		''' 
		''' The matrix A is overwritten by the Q component (i.e. destroyed)
		''' 
		''' geqrf = QR factorization of a general matrix (GE) into an orthogonal
		'''         matrix Q and an upper triangular R matrix
		''' </summary>
		''' <param name="A"> the input matrix, it will be overwritten with the factors </param>
		''' <param name="The"> R array if null R is not returned </param>
		''' <exception cref="Error"> - with a message to indicate failure (usu. bad params) </exception>
		Sub geqrf(ByVal A As INDArray, ByVal R As INDArray)



		''' <summary>
		''' Triangular decomposiiton of a positive definite matrix ( cholesky )
		''' Factorize a matrix A such that A = LL* (assuming lower==true) or
		''' A = U*U   (a * represents conjugate i.e. if matrix is real U* is a transpose) 
		''' 
		''' The matrix A is overridden by the L (or U).
		''' 
		''' potrf = LU factorization of a positive definite matrix (PO) into a
		'''         lower L ( or upper U ) triangular matrix
		''' </summary>
		''' <param name="A"> the input matrix, it will be overwritten with the factors </param>
		''' <param name="whether"> to return the upper (false) or lower factor
		''' @returns Permutation array </param>
		''' <exception cref="Error"> - with a message to indicate failure (usu. bad params) </exception>
		Sub potrf(ByVal A As INDArray, ByVal lower As Boolean)


		''' <summary>
		''' Caclulate the eigenvalues and vectors of a symmetric matrix. The
		''' symmetric matrix means the results are guaranteed to be real (not complex)
		''' 
		''' The matrix A is overridden by the eigenvectors. The eigenvalues
		''' are returned separately
		''' </summary>
		''' <param name="A"> the input matrix, it will be overwritten with the eigenvectors </param>
		''' <param name="V"> the resulting eigenvalues </param>
		''' <exception cref="Error"> - with a message to indicate failure (usu. bad params) </exception>
		Function syev(ByVal jobz As Char, ByVal uplo As Char, ByVal A As INDArray, ByVal V As INDArray) As Integer



		''' <summary>
		''' SVD decomposiiton of a matrix
		''' Factorize a matrix into its singular vectors and eigenvalues
		''' The decomposition is such that:
		''' 
		''' 		A = U x S x VT
		''' 
		''' gesvd = singular value decomposition (SVD) of a general matrix (GE)
		''' </summary>
		''' <param name="A"> the input matrix </param>
		''' <param name="S"> the eigenvalues as a vector </param>
		''' <param name="U"> the left singular vectors as a matrix. Maybe null if no S required </param>
		''' <param name="VT"> the right singular vectors as a (transposed) matrix. Maybe null if no V required </param>
		''' <exception cref="Error"> - with a message to indicate failure (usu. bad params) </exception>
		Sub gesvd(ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray)



		''' <summary>
		''' This method takes one of the ipiv returns from LAPACK and creates
		''' the permutation matrix. When factorizing, it is useful to avoid underflows
		''' and overflows by reordering rows/and or columns of the input matrix (mostly
		''' these methods solve simultaneous equations, so order is not important). 
		''' The ipiv method assumes that only row ordering is done ( never seen column 
		''' ordering done )
		''' </summary>
		''' <param name="M"> - the size of the permutation matrix ( usu. the # rows in factored matrix ) </param>
		''' <param name="ipiv"> - the vector returned from a refactoring
		''' @returned the square permutation matrix - size is the M x M </param>
		Function getPFactor(ByVal M As Integer, ByVal ipiv As INDArray) As INDArray


		''' <summary>
		''' extracts the L (lower triangular) matrix from the LU factor result
		''' L will be the same dimensions as A
		''' </summary>
		''' <param name="A"> - the combined L & U matrices returned from factorization
		''' @returned the lower triangular with unit diagonal </param>
		Function getLFactor(ByVal A As INDArray) As INDArray


		''' <summary>
		''' extracts the U (upper triangular) matrix from the LU factor result
		''' U will be n x n matrix where n = num cols in A
		''' </summary>
		''' <param name="A"> - the combined L & U matrices returned from factorization
		''' @returned the upper triangular matrix </param>
		Function getUFactor(ByVal A As INDArray) As INDArray


		' generate inverse of a matrix given its LU decomposition

		''' <summary>
		''' Generate inverse ggiven LU decomp </summary>
		''' <param name="N"> </param>
		''' <param name="A"> </param>
		''' <param name="lda"> </param>
		''' <param name="IPIV"> </param>
		''' <param name="WORK"> </param>
		''' <param name="lwork"> </param>
		''' <param name="INFO"> </param>
		Sub getri(ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal WORK As INDArray, ByVal lwork As Integer, ByVal INFO As Integer)

	End Interface

End Namespace