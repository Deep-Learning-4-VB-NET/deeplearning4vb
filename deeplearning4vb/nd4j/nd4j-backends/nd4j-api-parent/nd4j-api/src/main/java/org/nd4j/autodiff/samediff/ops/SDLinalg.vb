import static org.nd4j.autodiff.samediff.ops.SDValidation.isSameType
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.autodiff.samediff.ops

	Public Class SDLinalg
		Inherits SDOps

	  Public Sub New(ByVal sameDiff As SameDiff)
		MyBase.New(sameDiff)
	  End Sub

	  ''' <summary>
	  ''' Computes the Cholesky decomposition of one or more square matrices.<br>
	  ''' </summary>
	  ''' <param name="input"> Input tensor with inner-most 2 dimensions forming square matrices (NUMERIC type) </param>
	  ''' <returns> output Transformed tensor (NUMERIC type) </returns>
	  Public Overridable Function cholesky(ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("Cholesky", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.Cholesky(sd,input)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Computes the Cholesky decomposition of one or more square matrices.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> Input tensor with inner-most 2 dimensions forming square matrices (NUMERIC type) </param>
	  ''' <returns> output Transformed tensor (NUMERIC type) </returns>
	  Public Overridable Function cholesky(ByVal name As String, ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("Cholesky", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.Cholesky(sd,input)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Solver for linear squares problems.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="l2_reguralizer"> regularizer </param>
	  ''' <param name="fast"> fast mode, defaults to True </param>
	  ''' <returns> output Transformed tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function lstsq(ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal l2_reguralizer As Double, ByVal fast As Boolean) As SDVariable
		SDValidation.validateNumerical("Lstsq", "matrix", matrix)
		SDValidation.validateNumerical("Lstsq", "rhs", rhs)
		Return (New org.nd4j.linalg.api.ops.custom.Lstsq(sd,matrix, rhs, l2_reguralizer, fast)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Solver for linear squares problems.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="l2_reguralizer"> regularizer </param>
	  ''' <param name="fast"> fast mode, defaults to True </param>
	  ''' <returns> output Transformed tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function lstsq(ByVal name As String, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal l2_reguralizer As Double, ByVal fast As Boolean) As SDVariable
		SDValidation.validateNumerical("Lstsq", "matrix", matrix)
		SDValidation.validateNumerical("Lstsq", "rhs", rhs)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Lstsq(sd,matrix, rhs, l2_reguralizer, fast)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Solver for linear squares problems.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="l2_reguralizer"> regularizer </param>
	  ''' <returns> output Transformed tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function lstsq(ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal l2_reguralizer As Double) As SDVariable
		SDValidation.validateNumerical("Lstsq", "matrix", matrix)
		SDValidation.validateNumerical("Lstsq", "rhs", rhs)
		Return (New org.nd4j.linalg.api.ops.custom.Lstsq(sd,matrix, rhs, l2_reguralizer, True)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Solver for linear squares problems.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="l2_reguralizer"> regularizer </param>
	  ''' <returns> output Transformed tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function lstsq(ByVal name As String, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal l2_reguralizer As Double) As SDVariable
		SDValidation.validateNumerical("Lstsq", "matrix", matrix)
		SDValidation.validateNumerical("Lstsq", "rhs", rhs)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Lstsq(sd,matrix, rhs, l2_reguralizer, True)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Computes LU decomposition.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function lu(ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("Lu", "input", input)
		Return (New org.nd4j.linalg.api.ops.custom.Lu(sd,input)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Computes LU decomposition.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function lu(ByVal name As String, ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("Lu", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Lu(sd,input)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Performs matrix mutiplication on input tensors.<br>
	  ''' </summary>
	  ''' <param name="a"> input tensor (NUMERIC type) </param>
	  ''' <param name="b"> input tensor (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function matmul(ByVal a As SDVariable, ByVal b As SDVariable) As SDVariable
		SDValidation.validateNumerical("Matmul", "a", a)
		SDValidation.validateNumerical("Matmul", "b", b)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.Mmul(sd,a, b)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Performs matrix mutiplication on input tensors.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="a"> input tensor (NUMERIC type) </param>
	  ''' <param name="b"> input tensor (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function matmul(ByVal name As String, ByVal a As SDVariable, ByVal b As SDVariable) As SDVariable
		SDValidation.validateNumerical("Matmul", "a", a)
		SDValidation.validateNumerical("Matmul", "b", b)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.Mmul(sd,a, b)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Copy a tensor setting outside a central band in each innermost matrix.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <param name="minLower"> lower diagonal count </param>
	  ''' <param name="maxUpper"> upper diagonal count </param>
	  Public Overridable Function matrixBandPart(ByVal input As SDVariable, ByVal minLower As Integer, ByVal maxUpper As Integer) As SDVariable()
		SDValidation.validateNumerical("MatrixBandPart", "input", input)
		Return (New org.nd4j.linalg.api.ops.custom.MatrixBandPart(sd,input, minLower, maxUpper)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Copy a tensor setting outside a central band in each innermost matrix.<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <param name="minLower"> lower diagonal count </param>
	  ''' <param name="maxUpper"> upper diagonal count </param>
	  Public Overridable Function matrixBandPart(ByVal names() As String, ByVal input As SDVariable, ByVal minLower As Integer, ByVal maxUpper As Integer) As SDVariable()
		SDValidation.validateNumerical("MatrixBandPart", "input", input)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.custom.MatrixBandPart(sd,input, minLower, maxUpper)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Computes the QR decompositions of input matrix.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <param name="full"> full matrices mode </param>
	  Public Overridable Function qr(ByVal input As SDVariable, ByVal full As Boolean) As SDVariable()
		SDValidation.validateNumerical("Qr", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Qr(sd,input, full)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Computes the QR decompositions of input matrix.<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  ''' <param name="full"> full matrices mode </param>
	  Public Overridable Function qr(ByVal names() As String, ByVal input As SDVariable, ByVal full As Boolean) As SDVariable()
		SDValidation.validateNumerical("Qr", "input", input)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Qr(sd,input, full)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Computes the QR decompositions of input matrix.<br>
	  ''' </summary>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  Public Overridable Function qr(ByVal input As SDVariable) As SDVariable()
		SDValidation.validateNumerical("Qr", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Qr(sd,input, False)).outputVariables()
	  End Function

	  ''' <summary>
	  ''' Computes the QR decompositions of input matrix.<br>
	  ''' </summary>
	  ''' <param name="names"> names May be null. Arrays of names for the output variables. </param>
	  ''' <param name="input"> input tensor (NUMERIC type) </param>
	  Public Overridable Function qr(ByVal names() As String, ByVal input As SDVariable) As SDVariable()
		SDValidation.validateNumerical("Qr", "input", input)
		Dim [out]() As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Qr(sd,input, False)).outputVariables()
		Return sd.updateVariableNamesAndReferences([out], names)
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear equations.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="adjoint"> adjoint mode, defaults to False </param>
	  ''' <returns> output Output tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function solve(ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal adjoint As Boolean) As SDVariable
		SDValidation.validateNumerical("Solve", "matrix", matrix)
		SDValidation.validateNumerical("Solve", "rhs", rhs)
		Return (New org.nd4j.linalg.api.ops.custom.LinearSolve(sd,matrix, rhs, adjoint)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear equations.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="adjoint"> adjoint mode, defaults to False </param>
	  ''' <returns> output Output tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function solve(ByVal name As String, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal adjoint As Boolean) As SDVariable
		SDValidation.validateNumerical("Solve", "matrix", matrix)
		SDValidation.validateNumerical("Solve", "rhs", rhs)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.LinearSolve(sd,matrix, rhs, adjoint)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear equations.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <returns> output Output tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function solve(ByVal matrix As SDVariable, ByVal rhs As SDVariable) As SDVariable
		SDValidation.validateNumerical("Solve", "matrix", matrix)
		SDValidation.validateNumerical("Solve", "rhs", rhs)
		Return (New org.nd4j.linalg.api.ops.custom.LinearSolve(sd,matrix, rhs, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear equations.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <returns> output Output tensor (FLOATING_POINT type) </returns>
	  Public Overridable Function solve(ByVal name As String, ByVal matrix As SDVariable, ByVal rhs As SDVariable) As SDVariable
		SDValidation.validateNumerical("Solve", "matrix", matrix)
		SDValidation.validateNumerical("Solve", "rhs", rhs)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.LinearSolve(sd,matrix, rhs, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear questions.<br>
	  ''' </summary>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="lower"> defines whether innermost matrices in matrix are lower or upper triangular </param>
	  ''' <param name="adjoint"> adjoint mode </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triangularSolve(ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal lower As Boolean, ByVal adjoint As Boolean) As SDVariable
		SDValidation.validateNumerical("TriangularSolve", "matrix", matrix)
		SDValidation.validateNumerical("TriangularSolve", "rhs", rhs)
		Return (New org.nd4j.linalg.api.ops.custom.TriangularSolve(sd,matrix, rhs, lower, adjoint)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Solver for systems of linear questions.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="matrix"> input tensor (NUMERIC type) </param>
	  ''' <param name="rhs"> input tensor (NUMERIC type) </param>
	  ''' <param name="lower"> defines whether innermost matrices in matrix are lower or upper triangular </param>
	  ''' <param name="adjoint"> adjoint mode </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triangularSolve(ByVal name As String, ByVal matrix As SDVariable, ByVal rhs As SDVariable, ByVal lower As Boolean, ByVal adjoint As Boolean) As SDVariable
		SDValidation.validateNumerical("TriangularSolve", "matrix", matrix)
		SDValidation.validateNumerical("TriangularSolve", "rhs", rhs)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.TriangularSolve(sd,matrix, rhs, lower, adjoint)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Computes pairwise cross product.<br>
	  ''' </summary>
	  ''' <param name="a">  (NUMERIC type) </param>
	  ''' <param name="b">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function cross(ByVal a As SDVariable, ByVal b As SDVariable) As SDVariable
		SDValidation.validateNumerical("cross", "a", a)
		SDValidation.validateNumerical("cross", "b", b)
		Return (New org.nd4j.linalg.api.ops.impl.shape.Cross(sd,a, b)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Computes pairwise cross product.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="a">  (NUMERIC type) </param>
	  ''' <param name="b">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function cross(ByVal name As String, ByVal a As SDVariable, ByVal b As SDVariable) As SDVariable
		SDValidation.validateNumerical("cross", "a", a)
		SDValidation.validateNumerical("cross", "b", b)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Cross(sd,a, b)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculates diagonal tensor.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function diag(ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("diag", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.shape.Diag(sd,input)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Calculates diagonal tensor.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function diag(ByVal name As String, ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("diag", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.Diag(sd,input)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculates diagonal tensor.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function diag_part(ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("diag_part", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.shape.DiagPart(sd,input)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Calculates diagonal tensor.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function diag_part(ByVal name As String, ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("diag_part", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.shape.DiagPart(sd,input)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculates log of determinant.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function logdet(ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("logdet", "input", input)
		Return (New org.nd4j.linalg.api.ops.custom.Logdet(sd,input)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Calculates log of determinant.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function logdet(ByVal name As String, ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("logdet", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Logdet(sd,input)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
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
	  Public Overridable Function mmul(ByVal x As SDVariable, ByVal y As SDVariable, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean) As SDVariable
		SDValidation.validateNumerical("mmul", "x", x)
		SDValidation.validateNumerical("mmul", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.Mmul(sd,x, y, transposeX, transposeY, transposeZ)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Matrix multiplication: out = mmul(x,y)<br>
	  ''' Supports specifying transpose argument to perform operation such as mmul(a^T, b), etc.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input variable (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable (NUMERIC type) </param>
	  ''' <param name="transposeX"> Transpose x (first argument) </param>
	  ''' <param name="transposeY"> Transpose y (second argument) </param>
	  ''' <param name="transposeZ"> Transpose result array </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function mmul(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable, ByVal transposeX As Boolean, ByVal transposeY As Boolean, ByVal transposeZ As Boolean) As SDVariable
		SDValidation.validateNumerical("mmul", "x", x)
		SDValidation.validateNumerical("mmul", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.Mmul(sd,x, y, transposeX, transposeY, transposeZ)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Matrix multiplication: out = mmul(x,y)<br>
	  ''' Supports specifying transpose argument to perform operation such as mmul(a^T, b), etc.<br>
	  ''' </summary>
	  ''' <param name="x"> First input variable (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function mmul(ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("mmul", "x", x)
		SDValidation.validateNumerical("mmul", "y", y)
		Return (New org.nd4j.linalg.api.ops.impl.reduce.Mmul(sd,x, y, False, False, False)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Matrix multiplication: out = mmul(x,y)<br>
	  ''' Supports specifying transpose argument to perform operation such as mmul(a^T, b), etc.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="x"> First input variable (NUMERIC type) </param>
	  ''' <param name="y"> Second input variable (NUMERIC type) </param>
	  ''' <returns> output  (NUMERIC type) </returns>
	  Public Overridable Function mmul(ByVal name As String, ByVal x As SDVariable, ByVal y As SDVariable) As SDVariable
		SDValidation.validateNumerical("mmul", "x", x)
		SDValidation.validateNumerical("mmul", "y", y)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.reduce.Mmul(sd,x, y, False, False, False)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculates singular value decomposition.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="fullUV"> </param>
	  ''' <param name="computeUV"> </param>
	  ''' <param name="switchNum"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function svd(ByVal input As SDVariable, ByVal fullUV As Boolean, ByVal computeUV As Boolean, ByVal switchNum As Integer) As SDVariable
		SDValidation.validateNumerical("svd", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Svd(sd,input, fullUV, computeUV, switchNum)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Calculates singular value decomposition.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="fullUV"> </param>
	  ''' <param name="computeUV"> </param>
	  ''' <param name="switchNum"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function svd(ByVal name As String, ByVal input As SDVariable, ByVal fullUV As Boolean, ByVal computeUV As Boolean, ByVal switchNum As Integer) As SDVariable
		SDValidation.validateNumerical("svd", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Svd(sd,input, fullUV, computeUV, switchNum)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Calculates singular value decomposition.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="fullUV"> </param>
	  ''' <param name="computeUV"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function svd(ByVal input As SDVariable, ByVal fullUV As Boolean, ByVal computeUV As Boolean) As SDVariable
		SDValidation.validateNumerical("svd", "input", input)
		Return (New org.nd4j.linalg.api.ops.impl.transforms.custom.Svd(sd,input, fullUV, computeUV, 16)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Calculates singular value decomposition.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="fullUV"> </param>
	  ''' <param name="computeUV"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function svd(ByVal name As String, ByVal input As SDVariable, ByVal fullUV As Boolean, ByVal computeUV As Boolean) As SDVariable
		SDValidation.validateNumerical("svd", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.impl.transforms.custom.Svd(sd,input, fullUV, computeUV, 16)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' An array with ones at and below the given diagonal and zeros elsewhere.<br>
	  ''' </summary>
	  ''' <param name="dataType"> Data type </param>
	  ''' <param name="row"> </param>
	  ''' <param name="column"> </param>
	  ''' <param name="diagonal"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function tri(ByVal dataType As DataType, ByVal row As Integer, ByVal column As Integer, ByVal diagonal As Integer) As SDVariable
		Return (New org.nd4j.linalg.api.ops.custom.Tri(sd,dataType, row, column, diagonal)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' An array with ones at and below the given diagonal and zeros elsewhere.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="dataType"> Data type </param>
	  ''' <param name="row"> </param>
	  ''' <param name="column"> </param>
	  ''' <param name="diagonal"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function tri(ByVal name As String, ByVal dataType As DataType, ByVal row As Integer, ByVal column As Integer, ByVal diagonal As Integer) As SDVariable
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Tri(sd,dataType, row, column, diagonal)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' An array with ones at and below the given diagonal and zeros elsewhere.<br>
	  ''' </summary>
	  ''' <param name="row"> </param>
	  ''' <param name="column"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function tri(ByVal row As Integer, ByVal column As Integer) As SDVariable
		Return (New org.nd4j.linalg.api.ops.custom.Tri(sd,DataType.FLOAT, row, column, 0)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' An array with ones at and below the given diagonal and zeros elsewhere.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="row"> </param>
	  ''' <param name="column"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function tri(ByVal name As String, ByVal row As Integer, ByVal column As Integer) As SDVariable
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Tri(sd,DataType.FLOAT, row, column, 0)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Upper triangle of an array. Return a copy of a input tensor with the elements below the k-th diagonal zeroed.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="diag"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triu(ByVal input As SDVariable, ByVal diag As Integer) As SDVariable
		SDValidation.validateNumerical("triu", "input", input)
		Return (New org.nd4j.linalg.api.ops.custom.Triu(sd,input, diag)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Upper triangle of an array. Return a copy of a input tensor with the elements below the k-th diagonal zeroed.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <param name="diag"> </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triu(ByVal name As String, ByVal input As SDVariable, ByVal diag As Integer) As SDVariable
		SDValidation.validateNumerical("triu", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Triu(sd,input, diag)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function

	  ''' <summary>
	  ''' Upper triangle of an array. Return a copy of a input tensor with the elements below the k-th diagonal zeroed.<br>
	  ''' </summary>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triu(ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("triu", "input", input)
		Return (New org.nd4j.linalg.api.ops.custom.Triu(sd,input, 0)).outputVariable()
	  End Function

	  ''' <summary>
	  ''' Upper triangle of an array. Return a copy of a input tensor with the elements below the k-th diagonal zeroed.<br>
	  ''' </summary>
	  ''' <param name="name"> name May be null. Name for the output variable </param>
	  ''' <param name="input">  (NUMERIC type) </param>
	  ''' <returns> output  (FLOATING_POINT type) </returns>
	  Public Overridable Function triu(ByVal name As String, ByVal input As SDVariable) As SDVariable
		SDValidation.validateNumerical("triu", "input", input)
		Dim [out] As SDVariable = (New org.nd4j.linalg.api.ops.custom.Triu(sd,input, 0)).outputVariable()
		Return sd.updateVariableNameAndReference([out], name)
	  End Function
	End Class

End Namespace