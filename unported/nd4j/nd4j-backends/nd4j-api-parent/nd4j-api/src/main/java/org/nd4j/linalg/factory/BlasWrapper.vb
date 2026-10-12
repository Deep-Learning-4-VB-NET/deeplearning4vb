Imports System
Imports Lapack = org.nd4j.linalg.api.blas.Lapack
Imports Level1 = org.nd4j.linalg.api.blas.Level1
Imports Level2 = org.nd4j.linalg.api.blas.Level2
Imports Level3 = org.nd4j.linalg.api.blas.Level3
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

Namespace org.nd4j.linalg.factory


	Public Interface BlasWrapper
		''' <summary>
		'''*************************************************************************
		''' BLAS Level 1
		''' </summary>

		''' <summary>
		''' Compute x <-> y (swap two matrices)
		''' </summary>
		Function swap(ByVal x As INDArray, ByVal y As INDArray) As INDArray

		''' <summary>
		''' Return the level 1 functions
		''' for this blas impl
		''' @return
		''' </summary>
		Function level1() As Level1

		''' <summary>
		''' Return the level 2 functions
		''' for this blas impl
		''' @return
		''' </summary>
		Function level2() As Level2

		''' <summary>
		''' Return the level 3 functions
		''' for this blas impl
		''' @return
		''' </summary>
		Function level3() As Level3

		''' <summary>
		''' LAPack interface
		''' @return
		''' </summary>
		Function lapack() As Lapack


		<Obsolete>
		Function scal(ByVal alpha As Double, ByVal x As INDArray) As INDArray

		''' <summary>
		''' Compute x <- alpha * x (scale a matrix)
		''' </summary>
		<Obsolete>
		Function scal(ByVal alpha As Single, ByVal x As INDArray) As INDArray


		''' <summary>
		''' Compute y <- x (copy a matrix)
		''' </summary>
		Function copy(ByVal x As INDArray, ByVal y As INDArray) As INDArray

		<Obsolete>
		Function axpy(ByVal da As Double, ByVal dx As INDArray, ByVal dy As INDArray) As INDArray

		''' <summary>
		''' Compute y <- alpha * x + y (elementwise addition)
		''' </summary>
		<Obsolete>
		Function axpy(ByVal da As Single, ByVal dx As INDArray, ByVal dy As INDArray) As INDArray

		''' <summary>
		''' Compute y <- y + x * alpha </summary>
		''' <param name="da"> the alpha to multiply by </param>
		''' <param name="dx"> </param>
		''' <param name="dy">
		''' @return </param>
		Function axpy(ByVal da As Number, ByVal dx As INDArray, ByVal dy As INDArray) As INDArray

		''' <summary>
		''' Compute x^T * y (dot product)
		''' </summary>
		Function dot(ByVal x As INDArray, ByVal y As INDArray) As Double

		''' <summary>
		''' Compute || x ||_2 (2-norm)
		''' </summary>
		Function nrm2(ByVal x As INDArray) As Double

		''' <summary>
		''' Compute || x ||_1 (1-norm, sum of absolute values)
		''' </summary>
		Function asum(ByVal x As INDArray) As Double

		''' <summary>
		''' Compute index of element with largest absolute value (index of absolute
		''' value maximum)
		''' </summary>
		Function iamax(ByVal x As INDArray) As Integer

		''' <summary>
		''' ************************************************************************
		''' BLAS Level 2
		''' </summary>

		Function gemv(ByVal alpha As Number, ByVal a As INDArray, ByVal x As INDArray, ByVal beta As Double, ByVal y As INDArray) As INDArray

		<Obsolete>
		Function gemv(ByVal alpha As Double, ByVal a As INDArray, ByVal x As INDArray, ByVal beta As Double, ByVal y As INDArray) As INDArray

		''' <summary>
		''' Compute y <- alpha*op(a)*x + beta * y (general matrix vector
		''' multiplication)
		''' </summary>
		<Obsolete>
		Function gemv(ByVal alpha As Single, ByVal a As INDArray, ByVal x As INDArray, ByVal beta As Single, ByVal y As INDArray) As INDArray

		Function ger(ByVal alpha As Number, ByVal x As INDArray, ByVal y As INDArray, ByVal a As INDArray) As INDArray

		<Obsolete>
		Function ger(ByVal alpha As Double, ByVal x As INDArray, ByVal y As INDArray, ByVal a As INDArray) As INDArray

		''' <summary>
		''' Compute A <- alpha * x * y^T + A (general rank-1 update)
		''' </summary>
		Function ger(ByVal alpha As Single, ByVal x As INDArray, ByVal y As INDArray, ByVal a As INDArray) As INDArray

		''' <summary>
		''' ************************************************************************
		''' LAPACK
		''' </summary>

		Function gesv(ByVal a As INDArray, ByVal ipiv() As Integer, ByVal b As INDArray) As INDArray

		'STOP

		Sub checkInfo(ByVal name As String, ByVal info As Integer)
		'START

		Function sysv(ByVal uplo As Char, ByVal a As INDArray, ByVal ipiv() As Integer, ByVal b As INDArray) As INDArray



		Function syev(ByVal jobz As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal w As INDArray) As Integer

		Function syevx(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Double, ByVal vu As Double, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Double, ByVal w As INDArray, ByVal z As INDArray) As Integer

		Function syevx(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Single, ByVal vu As Single, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Single, ByVal w As INDArray, ByVal z As INDArray) As Integer

		Function syevd(ByVal jobz As Char, ByVal uplo As Char, ByVal A As INDArray, ByVal w As INDArray) As Integer

		<Obsolete>
		Function syevr(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Double, ByVal vu As Double, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Double, ByVal w As INDArray, ByVal z As INDArray, ByVal isuppz() As Integer) As Integer

		<Obsolete>
		Function syevr(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Single, ByVal vu As Single, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Single, ByVal w As INDArray, ByVal z As INDArray, ByVal isuppz() As Integer) As Integer


		Function syevr(ByVal jobz As Char, ByVal range As Char, ByVal uplo As Char, ByVal a As INDArray, ByVal vl As Single, ByVal vu As Single, ByVal il As Integer, ByVal iu As Integer, ByVal abstol As Number, ByVal w As INDArray, ByVal z As INDArray, ByVal isuppz() As Integer) As Integer


		Sub posv(ByVal uplo As Char, ByVal A As INDArray, ByVal B As INDArray)

		Function geev(ByVal jobvl As Char, ByVal jobvr As Char, ByVal A As INDArray, ByVal WR As INDArray, ByVal WI As INDArray, ByVal VL As INDArray, ByVal VR As INDArray) As Integer

		Function sygvd(ByVal itype As Integer, ByVal jobz As Char, ByVal uplo As Char, ByVal A As INDArray, ByVal B As INDArray, ByVal W As INDArray) As Integer

		''' <summary>
		''' Generalized Least Squares via *GELSD.
		''' <p/>
		''' Note that B must be padded to contain the solution matrix. This occurs when A has fewer rows
		''' than columns.
		''' <p/>
		''' For example: in A * X = B, A is (m,n), X is (n,k) and B is (m,k). Now if m < n, since B is overwritten to contain
		''' the solution (in classical LAPACK style), B needs to be padded to be an (n,k) matrix.
		''' <p/>
		''' Likewise, if m > n, the solution consists only of the first n rows of B.
		''' </summary>
		''' <param name="A"> an (m,n) matrix </param>
		''' <param name="B"> an (max(m,n), k) matrix (well, at least) </param>
		Sub gelsd(ByVal A As INDArray, ByVal B As INDArray)

		Sub geqrf(ByVal A As INDArray, ByVal tau As INDArray)

		Sub ormqr(ByVal side As Char, ByVal trans As Char, ByVal A As INDArray, ByVal tau As INDArray, ByVal C As INDArray)


		<Obsolete>
		Sub saxpy(ByVal alpha As Double, ByVal x As INDArray, ByVal y As INDArray)

		''' <summary>
		''' Abstraction over saxpy
		''' </summary>
		''' <param name="alpha"> the alpha to scale by </param>
		''' <param name="x">     the ndarray to use </param>
		''' <param name="y">     the ndarray to use </param>
		<Obsolete>
		Sub saxpy(ByVal alpha As Single, ByVal x As INDArray, ByVal y As INDArray)


	End Interface

End Namespace