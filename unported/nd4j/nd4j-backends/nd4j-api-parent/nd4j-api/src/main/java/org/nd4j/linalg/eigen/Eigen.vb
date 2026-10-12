Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports InvertMatrix = org.nd4j.linalg.inverse.InvertMatrix
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4j.linalg.eigen

	Public Class Eigen

		Public Shared dummy As INDArray = Nd4j.scalar(1)

		''' <summary>
		''' Compute generalized eigenvalues of the problem A x = L x.
		''' Matrix A is modified in the process, holding eigenvectors after execution.
		''' </summary>
		''' <param name="A"> symmetric Matrix A. After execution, A will contain the eigenvectors as columns </param>
		''' <returns> a vector of eigenvalues L. </returns>
		Public Shared Function symmetricGeneralizedEigenvalues(ByVal A As INDArray) As INDArray
			Dim eigenvalues As INDArray = Nd4j.create(A.dataType(), A.rows())
			Nd4j.BlasWrapper.syev("V"c, "L"c, A, eigenvalues)
			Return eigenvalues
		End Function


		''' <summary>
		''' Compute generalized eigenvalues of the problem A x = L x.
		''' Matrix A is modified in the process, holding eigenvectors as columns after execution.
		''' </summary>
		''' <param name="A"> symmetric Matrix A. After execution, A will contain the eigenvectors as columns </param>
		''' <param name="calculateVectors"> if false, it will not modify A and calculate eigenvectors </param>
		''' <returns> a vector of eigenvalues L. </returns>
		Public Shared Function symmetricGeneralizedEigenvalues(ByVal A As INDArray, ByVal calculateVectors As Boolean) As INDArray
			Dim eigenvalues As INDArray = Nd4j.create(A.dataType(), A.rows())
			Nd4j.BlasWrapper.syev("V"c, "L"c, (If(calculateVectors, A, A.dup())), eigenvalues)
			Return eigenvalues
		End Function


		''' <summary>
		''' Compute generalized eigenvalues of the problem A x = L B x.
		''' The data will be unchanged, no eigenvectors returned.
		''' </summary>
		''' <param name="A"> symmetric Matrix A. </param>
		''' <param name="B"> symmetric Matrix B. </param>
		''' <returns> a vector of eigenvalues L. </returns>
		Public Shared Function symmetricGeneralizedEigenvalues(ByVal A As INDArray, ByVal B As INDArray) As INDArray
			Preconditions.checkArgument(A.Matrix AndAlso A.Square, "Argument A must be a square matrix: has shape %s", A.shape())
			Preconditions.checkArgument(B.Matrix AndAlso B.Square, "Argument B must be a square matrix: has shape %s", B.shape())
			Dim W As INDArray = Nd4j.create(A.rows())

			A = InvertMatrix.invert(B, False).mmuli(A)
			Nd4j.BlasWrapper.syev("V"c, "L"c, A, W)
			Return W
		End Function

		''' <summary>
		''' Compute generalized eigenvalues of the problem A x = L B x.
		''' The data will be unchanged, no eigenvectors returned unless calculateVectors is true.
		''' If calculateVectors == true, A will contain a matrix with the eigenvectors as columns.
		''' </summary>
		''' <param name="A"> symmetric Matrix A. </param>
		''' <param name="B"> symmetric Matrix B. </param>
		''' <returns> a vector of eigenvalues L. </returns>
		Public Shared Function symmetricGeneralizedEigenvalues(ByVal A As INDArray, ByVal B As INDArray, ByVal calculateVectors As Boolean) As INDArray
			Preconditions.checkArgument(A.Matrix AndAlso A.Square, "Argument A must be a square matrix: has shape %s", A.shape())
			Preconditions.checkArgument(B.Matrix AndAlso B.Square, "Argument B must be a square matrix: has shape %s", B.shape())
			Dim W As INDArray = Nd4j.create(A.dataType(), A.rows())
			If calculateVectors Then
				A.assign(InvertMatrix.invert(B, False).mmuli(A))
			Else
				A = InvertMatrix.invert(B, False).mmuli(A)
			End If

			Nd4j.BlasWrapper.syev("V"c, "L"c, A, W)
			Return W
		End Function

		''' <summary>
		''' Compute the eigenvalues and eigenvectors of a square matrix </summary>
		''' <param name="A"> square Matrix A. </param>
		''' <returns> {eigenvalues, eigenvectors}. </returns>
		Public Shared Function eig(ByVal A As INDArray) As INDArray()
			Preconditions.checkArgument(A.Matrix AndAlso A.Square, "Argument A must be a square matrix: has shape %s", A.shape())
			Dim op_eig As DynamicCustomOp = DynamicCustomOp.builder("eig").addInputs(A).build()
			Return Nd4j.Executioner.exec(op_eig)
		End Function


	End Class

End Namespace