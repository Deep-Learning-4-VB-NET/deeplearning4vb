Imports DecompositionSolver = org.apache.commons.math3.linear.DecompositionSolver
Imports LUDecomposition = org.apache.commons.math3.linear.LUDecomposition
Imports QRDecomposition = org.apache.commons.math3.linear.QRDecomposition
Imports RealMatrix = org.apache.commons.math3.linear.RealMatrix
Imports SingularMatrixException = org.apache.commons.math3.linear.SingularMatrixException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CheckUtil = org.nd4j.linalg.checkutil.CheckUtil

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

Namespace org.nd4j.linalg.inverse

	Public Class InvertMatrix


		''' <summary>
		''' Inverts a matrix </summary>
		''' <param name="arr"> the array to invert </param>
		''' <param name="inPlace"> Whether to store the result in {@code arr} </param>
		''' <returns> the inverted matrix </returns>
		Public Shared Function invert(ByVal arr As INDArray, ByVal inPlace As Boolean) As INDArray
			If arr.rank() = 2 AndAlso arr.length() = 1 Then
				'[1,1] edge case. Matrix inversion: [x] * [1/x] = [1]
				If inPlace Then
					Return arr.rdivi(1.0)
				Else
					Return arr.rdiv(1.0)
				End If
			End If
			If Not arr.Square Then
				Throw New System.ArgumentException("invalid array: must be square matrix")
			End If

			'FIX ME: Please
	'        int[] IPIV = new int[arr.length() + 1];
	'        int LWORK = arr.length() * arr.length();
	'        INDArray WORK = Nd4j.create(new double[LWORK]);
	'        INDArray inverse = inPlace ? arr : arr.dup();
	'        Nd4j.getBlasWrapper().lapack().getrf(arr);
	'        Nd4j.getBlasWrapper().lapack().getri(arr.size(0),inverse,arr.size(0),IPIV,WORK,LWORK,0);

			Dim rm As RealMatrix = CheckUtil.convertToApacheMatrix(arr)
			Dim rmInverse As RealMatrix = (New LUDecomposition(rm)).getSolver().getInverse()


			Dim inverse As INDArray = CheckUtil.convertFromApacheMatrix(rmInverse, arr.dataType())
			If inPlace Then
				arr.assign(inverse)
			End If
			Return inverse

		End Function

		''' <summary>
		''' Calculates pseudo inverse of a matrix using QR decomposition </summary>
		''' <param name="arr"> the array to invert </param>
		''' <returns> the pseudo inverted matrix </returns>
		Public Shared Function pinvert(ByVal arr As INDArray, ByVal inPlace As Boolean) As INDArray

			' TODO : do it natively instead of relying on commons-maths

			Dim realMatrix As RealMatrix = CheckUtil.convertToApacheMatrix(arr)
			Dim decomposition As New QRDecomposition(realMatrix, 0)
			Dim solver As DecompositionSolver = decomposition.getSolver()

			If Not solver.isNonSingular() Then
				Throw New System.ArgumentException("invalid array: must be singular matrix")
			End If

			Dim pinvRM As RealMatrix = solver.getInverse()

			Dim pseudoInverse As INDArray = CheckUtil.convertFromApacheMatrix(pinvRM, arr.dataType())

			If inPlace Then
				arr.assign(pseudoInverse)
			End If
			Return pseudoInverse

		End Function

		''' <summary>
		''' Compute the left pseudo inverse. Input matrix must have full column rank.
		''' 
		''' See also: <a href="https://en.wikipedia.org/wiki/Moore%E2%80%93Penrose_inverse#Definition">Moore–Penrose inverse</a>
		''' </summary>
		''' <param name="arr"> Input matrix </param>
		''' <param name="inPlace"> Whether to store the result in {@code arr} </param>
		''' <returns> Left pseudo inverse of {@code arr} </returns>
		''' <exception cref="IllegalArgumentException"> Input matrix {@code arr} did not have full column rank. </exception>
		Public Shared Function pLeftInvert(ByVal arr As INDArray, ByVal inPlace As Boolean) As INDArray
			Try
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inv = invert(arr.transpose().mmul(arr), inPlace).mmul(arr.transpose());
			  Dim inv As INDArray = invert(arr.transpose().mmul(arr), inPlace).mmul(arr.transpose())
			  If inPlace Then
				  arr.assign(inv)
			  End If
			  Return inv
			Catch e As SingularMatrixException
			  Throw New System.ArgumentException("Full column rank condition for left pseudo inverse was not met.")
			End Try
		End Function

		''' <summary>
		''' Compute the right pseudo inverse. Input matrix must have full row rank.
		''' 
		''' See also: <a href="https://en.wikipedia.org/wiki/Moore%E2%80%93Penrose_inverse#Definition">Moore–Penrose inverse</a>
		''' </summary>
		''' <param name="arr"> Input matrix </param>
		''' <param name="inPlace"> Whether to store the result in {@code arr} </param>
		''' <returns> Right pseudo inverse of {@code arr} </returns>
		''' <exception cref="IllegalArgumentException"> Input matrix {@code arr} did not have full row rank. </exception>
		Public Shared Function pRightInvert(ByVal arr As INDArray, ByVal inPlace As Boolean) As INDArray
			Try
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inv = arr.transpose().mmul(invert(arr.mmul(arr.transpose()), inPlace));
				Dim inv As INDArray = arr.transpose().mmul(invert(arr.mmul(arr.transpose()), inPlace))
				If inPlace Then
					arr.assign(inv)
				End If
				Return inv
			Catch e As SingularMatrixException
				Throw New System.ArgumentException("Full row rank condition for right pseudo inverse was not met.")
			End Try
		End Function
	End Class

End Namespace