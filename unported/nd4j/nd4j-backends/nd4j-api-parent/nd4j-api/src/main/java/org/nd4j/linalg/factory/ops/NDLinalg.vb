import static org.nd4j.linalg.factory.NDValidation.isSameType
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDValidation = org.nd4j.linalg.factory.NDValidation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

''' <summary>
'''*****************************************************************************
''' Copyright (c) 2019-2020 Konduit K.K.
''' 
''' This program and the accompanying materials are made available under the
''' terms of the Apache License, Version 2.0 which is available at
''' https://www.apache.org/licenses/LICENSE-2.0.
''' 
''' Unless required by applicable law or agreed to in writing, software
''' distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
''' WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
''' License for the specific language governing permissions and limitations
''' under the License.
''' 
''' SPDX-License-Identifier: Apache-2.0
''' *****************************************************************************
''' </summary>

'================== GENERATED CODE - DO NOT MODIFY THIS FILE ==================

Namespace org.nd4j.linalg.factory.ops

	Public Class NDLinalg
	  Public Sub New()
	  End Sub

	  ''' <summary>
	  ''' Computes the Cholesky decomposition of one or more square matrices.<br>
	  ''' </summary>
	  ''' <param name="input"> Input tensor with inner-most 2 dimensions forming square matrices (NUMERIC type) </param>
	  ''' <returns> output Transformed tensor (NUMERIC type) </returns>
	  Public Overridable Function cholesky(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("Cholesky", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.Cholesky(input))(0)
	  End Function

	  ''' <summary>
	  ''' Solver for linear squares problems.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="l2_reguralizer"> regularizer </param>
	  ''' <param name="fast"> fast mode, defaults to True </param>
	  ''' <returns> output Transformed tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function lstsq(ByVal matrix As INDArray, ByVal rhs As INDArray, ByVal l2_reguralizer As Double, ByVal fast As Boolean) As INDArray
		NDValidation.validateNumerical("Lstsq", "matrix", matrix)
		NDValidation.validateNumerical("Lstsq", "rhs", rhs)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Lstsq(matrix, rhs, l2_reguralizer, fast))(0)
	  End Function

	  ''' <summary>
	  ''' Solver for linear squares problems.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="l2_reguralizer"> regularizer </param>
	  ''' <returns> output Transformed tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function lstsq(ByVal matrix As INDArray, ByVal rhs As INDArray, ByVal l2_reguralizer As Double) As INDArray
		NDValidation.validateNumerical("Lstsq", "matrix", matrix)
		NDValidation.validateNumerical("Lstsq", "rhs", rhs)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Lstsq(matrix, rhs, l2_reguralizer, True))(0)
	  End Function

	  ''' <summary>
	  ''' Computes LU decomposition.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function lu(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("Lu", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Lu(input))(0)
	  End Function

	  ''' <summary>
	  ''' Performs matrix mutiplication on input tensors.<br>
	  ''' </summary>
	  ''' <param name="a"> input tensor (NUMERIC type) </param>
	  ''' <param name="b"> input tensor (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function matmul(ByVal a As INDArray, ByVal b As INDArray) As INDArray
		NDValidation.validateNumerical("Matmul", "a", a)
		NDValidation.validateNumerical("Matmul", "b", b)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.Mmul(a, b))(0)
	  End Function

	  ''' <summary>
	  ''' Copy a tensor setting outside a central band in each innermost matrix.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <param name="minLower"> lower diagonal count </param>
	  ''' <param name="maxUpper"> upper diagonal count </param>
	  Public Overridable Function matrixBandPart(ByVal input As INDArray, ByVal minLower As Integer, ByVal maxUpper As Integer) As INDArray()
		NDValidation.validateNumerical("MatrixBandPart", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.MatrixBandPart(input, minLower, maxUpper))
	  End Function

	  ''' <summary>
	  ''' Computes the QR decompositions of input matrix.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <param name="full"> full matrices mode </param>
	  Public Overridable Function qr(ByVal input As INDArray, ByVal full As Boolean) As INDArray()
		NDValidation.validateNumerical("Qr", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Qr(input, full))
	  End Function

	  ''' <summary>
	  ''' Computes the QR decompositions of input matrix.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  Public Overridable Function qr(ByVal input As INDArray) As INDArray()
		NDValidation.validateNumerical("Qr", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Qr(input, False))
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear equations.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="adjoint"> adjoint mode, defaults to False </param>
	  ''' <returns> output Output tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function solve(ByVal matrix As INDArray, ByVal rhs As INDArray, ByVal adjoint As Boolean) As INDArray
		NDValidation.validateNumerical("Solve", "matrix", matrix)
		NDValidation.validateNumerical("Solve", "rhs", rhs)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.LinearSolve(matrix, rhs, adjoint))(0)
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear equations.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <returns> output Output tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function solve(ByVal matrix As INDArray, ByVal rhs As INDArray) As INDArray
		NDValidation.validateNumerical("Solve", "matrix", matrix)
		NDValidation.validateNumerical("Solve", "rhs", rhs)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.LinearSolve(matrix, rhs, False))(0)
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear questions.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="lower"> defines whether innermost matrices in matrix are lower or upper triangular </param>
	  ''' <param name="adjoint"> adjoint mode </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triangularSolve(ByVal matrix As INDArray, ByVal rhs As INDArray, ByVal lower As Boolean, ByVal adjoint As Boolean) As INDArray
		NDValidation.validateNumerical("TriangularSolve", "matrix", matrix)
		NDValidation.validateNumerical("TriangularSolve", "rhs", rhs)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.TriangularSolve(matrix, rhs, lower, adjoint))(0)
	  End Function

	  ''' <summary>
	  ''' Computes pairwise cross product.<br>
	  ''' </summary>
	  ''' <param name="a">  (NUMERIC type) </param>
	  ''' <param name="b">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function cross(ByVal a As INDArray, ByVal b As INDArray) As INDArray
		NDValidation.validateNumerical("cross", "a", a)
		NDValidation.validateNumerical("cross", "b", b)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Cross(a, b))(0)
	  End Function

	  ''' <summary>
	  ''' Calculates diagonal tensor.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function diag(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("diag", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.Diag(input))(0)
	  End Function

	  ''' <summary>
	  ''' Calculates diagonal tensor.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function diag_part(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("diag_part", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.shape.DiagPart(input))(0)
	  End Function

	  ''' <summary>
	  ''' Calculates log of determinant.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function logdet(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("logdet", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Logdet(input))(0)
	  End Function

	  ''' <summary>
	  ''' Matrix multiplication: out = mmul(x,y)<br>
	  ''' Supports specifying transpose argument to perform operation such as mmul(a^T, b), etc.<br>
	  ''' </summary>
	  ''' <param name="x"> First input variable (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable (NUMERIC type) </param>
	  ''' <param name="transposeX"> Transpose x (first argument) </param>
	  ''' <param name="transposeY"> Transpose y (second argument) </param>
	  ''' <param name="transposeZ"> Transpose result array </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function mmul(ByVal x As INDArray, ByVal y As INDArray, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean) As INDArray
		NDValidation.validateNumerical("mmul", "x", x)
		NDValidation.validateNumerical("mmul", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.Mmul(x, y, transposeX, transposeY, transposeZ))(0)
	  End Function

	  ''' <summary>
	  ''' Matrix multiplication: out = mmul(x,y)<br>
	  ''' Supports specifying transpose argument to perform operation such as mmul(a^T, b), etc.<br>
	  ''' </summary>
	  ''' <param name="x"> First input variable (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function mmul(ByVal x As INDArray, ByVal y As INDArray) As INDArray
		NDValidation.validateNumerical("mmul", "x", x)
		NDValidation.validateNumerical("mmul", "y", y)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.reduce.Mmul(x, y, False, False, False))(0)
	  End Function

	  ''' <summary>
	  ''' Calculates singular value decomposition.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="fullUV"> </param>
	  ''' <param name="computeUV"> </param>
	  ''' <param name="switchNum"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function svd(ByVal input As INDArray, ByVal fullUV As Boolean, ByVal computeUV As Boolean, ByVal switchNum As Integer) As INDArray
		NDValidation.validateNumerical("svd", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Svd(input, fullUV, computeUV, switchNum))(0)
	  End Function

	  ''' <summary>
	  ''' Calculates singular value decomposition.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="fullUV"> </param>
	  ''' <param name="computeUV"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function svd(ByVal input As INDArray, ByVal fullUV As Boolean, ByVal computeUV As Boolean) As INDArray
		NDValidation.validateNumerical("svd", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.impl.transforms.custom.Svd(input, fullUV, computeUV, 16))(0)
	  End Function

	  ''' <summary>
	  ''' An array with ones at and below the given diagonal and zeros elsewhere.<br>
	  ''' </summary>
	  ''' <param name="dataType"> Data type </param>
	  ''' <param name="row"> </param>
	  ''' <param name="column"> </param>
	  ''' <param name="diagonal"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function tri(ByVal dataType As DataType, ByVal row As Integer, ByVal column As Integer, ByVal diagonal As Integer) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Tri(dataType, row, column, diagonal))(0)
	  End Function

	  ''' <summary>
	  ''' An array with ones at and below the given diagonal and zeros elsewhere.<br>
	  ''' </summary>
	  ''' <param name="row"> </param>
	  ''' <param name="column"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function tri(ByVal row As Integer, ByVal column As Integer) As INDArray
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Tri(DataType.FLOAT, row, column, 0))(0)
	  End Function

	  ''' <summary>
	  ''' Upper triangle of an array. Return a copy of a input tensor with the elements below the k-th diagonal zeroed.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="diag"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triu(ByVal input As INDArray, ByVal diag As Integer) As INDArray
		NDValidation.validateNumerical("triu", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Triu(input, diag))(0)
	  End Function

	  ''' <summary>
	  ''' Upper triangle of an array. Return a copy of a input tensor with the elements below the k-th diagonal zeroed.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triu(ByVal input As INDArray) As INDArray
		NDValidation.validateNumerical("triu", "input", input)
		Return Nd4j.exec(New org.nd4j.linalg.api.ops.custom.Triu(input, 0))(0)
	  End Function
	End Class

End Namespace