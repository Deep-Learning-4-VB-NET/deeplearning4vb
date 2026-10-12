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

	Public Interface Level3
		''' <summary>
		''' gemm performs a matrix-matrix operation
		''' c := alpha*op(a)*op(b) + beta*c,
		''' where c is an m-by-n matrix,
		''' op(a) is an m-by-k matrix,
		''' op(b) is a k-by-n matrix. </summary>
		''' <param name="Order"> </param>
		''' <param name="TransA"> </param>
		''' <param name="TransB"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="B"> </param>
		''' <param name="beta"> </param>
		''' <param name="C"> </param>
		Sub gemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal beta As Double, ByVal C As INDArray)

		''' <summary>
		''' A convenience method for matrix-matrix operations with transposes.
		''' Implements C = alpha*op(A)*op(B) + beta*C
		''' Matrices A and B can be any order and offset (though will have copy overhead if elements are not contiguous in buffer)
		''' but matrix C MUST be f order, 0 offset and have length == data.length
		''' </summary>
		Sub gemm(ByVal A As INDArray, ByVal B As INDArray, ByVal C As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double)


		''' <summary>
		''' her2k performs a rank-2k update of an n-by-n Hermitian matrix c, that is, one of the following operations:
		''' c := alpha*a*conjg(b') + conjg(alpha)*b*conjg(a') + beta*c,  for trans = 'N'or'n'
		''' c := alpha*conjg(b')*a + conjg(alpha)*conjg(a')*b + beta*c,  for trans = 'C'or'c'
		''' where c is an n-by-n Hermitian matrix;
		''' a and b are n-by-k matrices if trans = 'N'or'n',
		''' a and b are k-by-n matrices if trans = 'C'or'c'. </summary>
		''' <param name="Order"> </param>
		''' <param name="Side"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="B"> </param>
		''' <param name="beta"> </param>
		''' <param name="C"> </param>
		Sub symm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal beta As Double, ByVal C As INDArray)

		''' <summary>
		''' syrk performs a rank-n update of an n-by-n symmetric matrix c, that is, one of the following operations:
		''' c := alpha*a*a' + beta*c  for trans = 'N'or'n'
		''' c := alpha*a'*a + beta*c  for trans = 'T'or't','C'or'c',
		''' where c is an n-by-n symmetric matrix;
		''' a is an n-by-k matrix, if trans = 'N'or'n',
		''' a is a k-by-n matrix, if trans = 'T'or't','C'or'c'. </summary>
		''' <param name="Order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="Trans"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="beta"> </param>
		''' <param name="C"> </param>
		Sub syrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal beta As Double, ByVal C As INDArray)

		''' <summary>
		''' yr2k performs a rank-2k update of an n-by-n symmetric matrix c, that is, one of the following operations:
		''' c := alpha*a*b' + alpha*b*a' + beta*c  for trans = 'N'or'n'
		''' c := alpha*a'*b + alpha*b'*a + beta*c  for trans = 'T'or't',
		''' where c is an n-by-n symmetric matrix;
		''' a and b are n-by-k matrices, if trans = 'N'or'n',
		''' a and b are k-by-n matrices, if trans = 'T'or't'. </summary>
		''' <param name="Order"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="Trans"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="B"> </param>
		''' <param name="beta"> </param>
		''' <param name="C"> </param>
		Sub syr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal beta As Double, ByVal C As INDArray)

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
		Sub trmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray, ByVal C As INDArray)

		''' <summary>
		''' ?trsm solves one of the following matrix equations:
		''' op(a)*x = alpha*b  or  x*op(a) = alpha*b,
		''' where x and b are m-by-n general matrices, and a is triangular;
		''' op(a) must be an m-by-m matrix, if side = 'L'or'l'
		''' op(a) must be an n-by-n matrix, if side = 'R'or'r'.
		''' For the definition of op(a), see Matrix Arguments.
		''' The routine overwrites x on b. </summary>
		''' <param name="Order"> </param>
		''' <param name="Side"> </param>
		''' <param name="Uplo"> </param>
		''' <param name="TransA"> </param>
		''' <param name="Diag"> </param>
		''' <param name="alpha"> </param>
		''' <param name="A"> </param>
		''' <param name="B"> </param>
		Sub trsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal alpha As Double, ByVal A As INDArray, ByVal B As INDArray)

	End Interface

End Namespace