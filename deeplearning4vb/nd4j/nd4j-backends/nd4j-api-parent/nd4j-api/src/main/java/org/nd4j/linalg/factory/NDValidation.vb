Imports DataType = org.nd4j.linalg.api.buffer.DataType
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


	Public Class NDValidation

		Private Sub New()
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a numerical INDArray (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc) don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to perform operation on </param>
		Public Shared Sub validateNumerical(ByVal opName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() = DataType.BOOL OrElse v.dataType() = DataType.UTF8 Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to array with non-numerical data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on numerical INDArrays (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc) don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to perform operation on </param>
		Public Shared Sub validateNumerical(ByVal opName As String, ByVal v() As INDArray)
			If v Is Nothing Then
				Return
			End If
			For i As Integer = 0 To v.Length - 1
				If v(i).dataType() = DataType.BOOL OrElse v(i).dataType() = DataType.UTF8 Then
					Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to input array " & i & " with non-numerical data type " & v(i).dataType())
				End If
			Next i
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a numerical INDArray (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc) don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateNumerical(ByVal opName As String, ByVal inputName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() = DataType.BOOL OrElse v.dataType() = DataType.UTF8 Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an numerical type type;" & " got array with non-integer data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on numerical INDArrays (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc) don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to perform operation on </param>
		Public Shared Sub validateNumerical(ByVal opName As String, ByVal inputName As String, ByVal v() As INDArray)
			If v Is Nothing Then
				Return
			End If
			For i As Integer = 0 To v.Length - 1
				If v(i).dataType() = DataType.BOOL OrElse v(i).dataType() = DataType.UTF8 Then
					Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to input """ & inputName & """ array " & i & " with non-numerical data type " & v(i).dataType())
				End If
			Next i
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on numerical INDArrays (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v1">     Variable to validate datatype for (input to operation) </param>
		''' <param name="v2">     Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateNumerical(ByVal opName As String, ByVal v1 As INDArray, ByVal v2 As INDArray)
			If v1.dataType() = DataType.BOOL OrElse v1.dataType() = DataType.UTF8 OrElse v2.dataType() = DataType.BOOL OrElse v2.dataType() = DataType.UTF8 Then
				Throw New System.InvalidOperationException("Cannot perform operation """ & opName & """ on arrays if one or both variables" & " are non-numerical: got " & v1.dataType() & " and " & v2.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on an integer type INDArray
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateInteger(ByVal opName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isIntType() Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to array with non-integer data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on an integer type INDArray
		''' </summary>
		''' <param name="opName">    Operation name to print in the exception </param>
		''' <param name="inputName"> Name of the input to the op to validate </param>
		''' <param name="v">         Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateInteger(ByVal opName As String, ByVal inputName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isIntType() Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an integer" & " type; got array with non-integer data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on an integer type INDArray []
		''' </summary>
		''' <param name="opName">    Operation name to print in the exception </param>
		''' <param name="inputName"> Name of the input to the op to validate </param>
		''' <param name="v">         Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateInteger(ByVal opName As String, ByVal inputName As String, ByVal v() As INDArray)
			If v Is Nothing Then
				Return
			End If
			For i As Integer = 0 To v.Length - 1
				If Not v(i).dataType().isIntType() Then
					Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an integer" & " type; got array with non-integer data type member" & v(i).dataType())
				End If
			Next i
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on an floating point type INDArray
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateFloatingPoint(ByVal opName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isFPType() Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to array with non-floating point data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a floating point type INDArray
		''' </summary>
		''' <param name="opName">    Operation name to print in the exception </param>
		''' <param name="inputName"> Name of the input to the op to validate </param>
		''' <param name="v">         Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateFloatingPoint(ByVal opName As String, ByVal inputName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isFPType() Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an floating point type; got array with non-floating point data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a boolean type INDArray
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateBool(ByVal opName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() <> DataType.BOOL Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to array with non-boolean point data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a boolean type INDArray
		''' </summary>
		''' <param name="opName">    Operation name to print in the exception </param>
		''' <param name="inputName"> Name of the input to the op to validate </param>
		''' <param name="v">         Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateBool(ByVal opName As String, ByVal inputName As String, ByVal v As INDArray)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() <> DataType.BOOL Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an boolean variable; got array with non-boolean data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on boolean INDArrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v1">     Variable to validate datatype for (input to operation) </param>
		''' <param name="v2">     Variable to validate datatype for (input to operation) </param>
		Public Shared Sub validateBool(ByVal opName As String, ByVal v1 As INDArray, ByVal v2 As INDArray)
			If v1.dataType() <> DataType.BOOL OrElse v2.dataType() <> DataType.BOOL Then
				Throw New System.InvalidOperationException("Cannot perform operation """ & opName & """ on array if one or both variables are non-boolean: " & v1.dataType() & " and " & v2.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on array with the exact same datatypes (which may optionally be
		''' restricted to numerical INDArrays only (not boolean or utf8))
		''' </summary>
		''' <param name="opName">        Operation name to print in the exception </param>
		''' <param name="numericalOnly"> If true, the variables must all be the same type, and must be numerical (not boolean/utf8) </param>
		''' <param name="vars">          Variable to perform operation on </param>
		Public Shared Sub validateSameType(ByVal opName As String, ByVal numericalOnly As Boolean, ParamArray ByVal vars() As INDArray)
			If vars.Length = 0 Then
				Return
			End If
			If vars.Length = 1 Then
				If numericalOnly Then
					validateNumerical(opName, vars(0))
				End If
			Else
				Dim first As DataType = vars(0).dataType()
				If numericalOnly Then
					validateNumerical(opName, vars(0))
				End If
				For i As Integer = 1 To vars.Length - 1
					If first <> vars(i).dataType() Then
						Dim dtypes(vars.Length - 1) As DataType
						For j As Integer = 0 To vars.Length - 1
							dtypes(j) = vars(j).dataType()
						Next j
						Throw New System.InvalidOperationException("Cannot perform operation """ & opName & """ to arrays with different datatypes:" & " Got arrays with datatypes " & Arrays.toString(dtypes))
					End If
				Next i
			End If
		End Sub

		Public Shared Function isSameType(ByVal x As INDArray, ByVal y As INDArray) As Boolean
			Return x.dataType() = y.dataType()
		End Function

		Public Shared Function isSameType(ByVal x() As INDArray) As Boolean
			If x.Length = 0 Then
				Return True
			End If
			Dim first As DataType = x(0).dataType()
			For i As Integer = 1 To x.Length - 1
				If first <> x(i).dataType() Then
					Return False
				End If
			Next i
			Return True
		End Function
	End Class

End Namespace