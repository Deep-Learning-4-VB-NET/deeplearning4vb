Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.autodiff.samediff.ops


	Public Class SDValidation

		Private Sub New()
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a numerical SDVariable (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to perform operation on </param>
		Protected Friend Shared Sub validateNumerical(ByVal opName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() = DataType.BOOL OrElse v.dataType() = DataType.UTF8 Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to variable """ & v.name() & """ with non-numerical data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a numerical SDVariable (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateNumerical(ByVal opName As String, ByVal inputName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() = DataType.BOOL OrElse v.dataType() = DataType.UTF8 Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an numerical type type; got variable """ & v.name() & """ with non-integer data type " & v.dataType())
			End If
		End Sub

		Protected Friend Shared Sub validateNumerical(ByVal opName As String, ByVal inputName As String, ByVal vars() As SDVariable)
			For Each v As SDVariable In vars
				If v Is Nothing Then
					Continue For
				End If
				If v.dataType() = DataType.BOOL OrElse v.dataType() = DataType.UTF8 Then
					Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an numerical type type; got variable """ & v.name() & """ with non-integer data type " & v.dataType())
				End If
			Next v
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on numerical SDVariables (not boolean or utf8).
		''' Some operations (such as sum, norm2, add(Number) etc don't make sense when applied to boolean/utf8 arrays
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v1">     Variable to validate datatype for (input to operation) </param>
		''' <param name="v2">     Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateNumerical(ByVal opName As String, ByVal v1 As SDVariable, ByVal v2 As SDVariable)
			If v1.dataType() = DataType.BOOL OrElse v1.dataType() = DataType.UTF8 OrElse v2.dataType() = DataType.BOOL OrElse v2.dataType() = DataType.UTF8 Then
				Throw New System.InvalidOperationException("Cannot perform operation """ & opName & """ on variables  """ & v1.name() & """ and """ & v2.name() & """ if one or both variables are non-numerical: " & v1.dataType() & " and " & v2.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on an integer type SDVariable
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateInteger(ByVal opName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isIntType() Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to variable """ & v.name() & """ with non-integer data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on an integer type SDVariable
		''' </summary>
		''' <param name="opName">    Operation name to print in the exception </param>
		''' <param name="inputName"> Name of the input to the op to validate </param>
		''' <param name="v">         Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateInteger(ByVal opName As String, ByVal inputName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isIntType() Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an integer type; got variable """ & v.name() & """ with non-integer data type " & v.dataType())
			End If
		End Sub

		Protected Friend Shared Sub validateInteger(ByVal opName As String, ByVal inputName As String, ByVal vars() As SDVariable)
			For Each v As SDVariable In vars
				If v Is Nothing Then
					Return
				End If
				If Not v.dataType().isIntType() Then
					Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an integer type; got variable """ & v.name() & """ with non-integer data type " & v.dataType())
				End If
			Next v
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on an floating point type SDVariable
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateFloatingPoint(ByVal opName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isFPType() Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to variable """ & v.name() & """ with non-floating point data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a floating point type SDVariable
		''' </summary>
		''' <param name="opName">    Operation name to print in the exception </param>
		''' <param name="inputName"> Name of the input to the op to validate </param>
		''' <param name="v">         Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateFloatingPoint(ByVal opName As String, ByVal inputName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If Not v.dataType().isFPType() Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an floating point type; got variable """ & v.name() & """ with non-floating point data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a boolean type SDVariable
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v">      Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateBool(ByVal opName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() <> DataType.BOOL Then
				Throw New System.InvalidOperationException("Cannot apply operation """ & opName & """ to variable """ & v.name() & """ with non-boolean point data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on a boolean type SDVariable
		''' </summary>
		''' <param name="opName">    Operation name to print in the exception </param>
		''' <param name="inputName"> Name of the input to the op to validate </param>
		''' <param name="v">         Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateBool(ByVal opName As String, ByVal inputName As String, ByVal v As SDVariable)
			If v Is Nothing Then
				Return
			End If
			If v.dataType() <> DataType.BOOL Then
				Throw New System.InvalidOperationException("Input """ & inputName & """ for operation """ & opName & """ must be an boolean variable; got variable """ & v.name() & """ with non-boolean data type " & v.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on boolean SDVariables
		''' </summary>
		''' <param name="opName"> Operation name to print in the exception </param>
		''' <param name="v1">     Variable to validate datatype for (input to operation) </param>
		''' <param name="v2">     Variable to validate datatype for (input to operation) </param>
		Protected Friend Shared Sub validateBool(ByVal opName As String, ByVal v1 As SDVariable, ByVal v2 As SDVariable)
			If v1.dataType() <> DataType.BOOL OrElse v2.dataType() <> DataType.BOOL Then
				Throw New System.InvalidOperationException("Cannot perform operation """ & opName & """ on variables  """ & v1.name() & """ and """ & v2.name() & """ if one or both variables are non-boolean: " & v1.dataType() & " and " & v2.dataType())
			End If
		End Sub

		''' <summary>
		''' Validate that the operation is being applied on array with the exact same datatypes (which may optionally be
		''' restricted to numerical SDVariables only (not boolean or utf8))
		''' </summary>
		''' <param name="opName">        Operation name to print in the exception </param>
		''' <param name="numericalOnly"> If true, the variables must all be the same type, and must be numerical (not boolean/utf8) </param>
		''' <param name="vars">          Variable to perform operation on </param>
		Protected Friend Shared Sub validateSameType(ByVal opName As String, ByVal numericalOnly As Boolean, ParamArray ByVal vars() As SDVariable)
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
						Dim names(vars.Length - 1) As String
						Dim dtypes(vars.Length - 1) As DataType
						For j As Integer = 0 To vars.Length - 1
							names(j) = vars(j).name()
							dtypes(j) = vars(j).dataType()
						Next j
						Throw New System.InvalidOperationException("Cannot perform operation """ & opName & """ to variables with different datatypes:" & " Variable names " & Arrays.toString(names) & ", datatypes " & Arrays.toString(dtypes))
					End If
				Next i
			End If
		End Sub

		Public Shared Function isSameType(ByVal x As SDVariable, ByVal y As SDVariable) As Boolean
			Return x.dataType() = y.dataType()
		End Function

		Public Shared Function isSameType(ByVal x() As SDVariable) As Boolean
			Dim firstDataType As DataType = x(0).dataType()
			If x.Length > 1 Then
				For i As Integer = 1 To x.Length - 1
					If firstDataType <> x(i).dataType() Then
						Return False
					End If
				Next i
			End If
			Return True
		End Function
	End Class

End Namespace