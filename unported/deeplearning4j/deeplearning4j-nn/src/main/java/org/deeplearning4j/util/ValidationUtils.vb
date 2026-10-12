Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.deeplearning4j.util

	''' <summary>
	''' Validation methods for array sizes/shapes and value non-negativeness
	''' 
	''' @author Ryan Nett
	''' </summary>
	Public Class ValidationUtils

		Private Sub New()

		End Sub

		''' <summary>
		''' Checks that the values is >= 0.
		''' </summary>
		''' <param name="data"> An int </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		Public Shared Sub validateNonNegative(ByVal data As Integer, ByVal paramName As String)
			Preconditions.checkArgument(data >= 0, "Values for %s must be >= 0, got: %s", paramName, data)
		End Sub

		''' <summary>
		''' Checks that the values is >= 0.
		''' </summary>
		''' <param name="data"> An int </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		Public Shared Sub validateNonNegative(ByVal data As Double, ByVal paramName As String)
			Preconditions.checkArgument(data >= 0, "Values for %s must be >= 0, got: %s", paramName, data)
		End Sub

		''' <summary>
		''' Checks that all values are >= 0.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		Public Shared Sub validateNonNegative(ByVal data() As Integer, ByVal paramName As String)

			If data Is Nothing Then
				Return
			End If

			Dim nonnegative As Boolean = True

			For Each value As Integer In data
				If value < 0 Then
					nonnegative = False
				End If
			Next value

			Preconditions.checkArgument(nonnegative, "Values for %s must be >= 0, got: %s", paramName, data)
		End Sub

		''' <summary>
		''' Reformats the input array to a length 1 array and checks that all values are >= 0.
		''' 
		''' If the array is length 1, returns the array
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 1 that represents the input </returns>
		Public Shared Function validate1NonNegative(ByVal data() As Integer, ByVal paramName As String) As Integer()
			validateNonNegative(data, paramName)
			Return validate1(data, paramName)
		End Function

		''' <summary>
		''' Reformats the input array to a length 1 array.
		''' 
		''' If the array is length 1, returns the array
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 1 that represents the input </returns>
		Public Shared Function validate1(ByVal data() As Integer, ByVal paramName As String) As Integer()
			If data Is Nothing Then
				Return Nothing
			End If

			Preconditions.checkArgument(data.Length = 1, "Need 1 %s value, got %s values: %s", paramName, data.Length, data)

			Return data
		End Function

		''' <summary>
		''' Reformats the input array to a length 2 array and checks that all values are >= 0.
		''' 
		''' If the array is length 1, returns [a, a]
		''' If the array is length 2, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 2 that represents the input </returns>
		Public Shared Function validate2NonNegative(ByVal data() As Integer, ByVal allowSz1 As Boolean, ByVal paramName As String) As Integer()
			validateNonNegative(data, paramName)
			Return validate2(data, allowSz1, paramName)
		End Function

		''' <summary>
		''' Reformats the input array to a length 2 array.
		''' 
		''' If the array is length 1, returns [a, a]
		''' If the array is length 2, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 2 that represents the input </returns>
		Public Shared Function validate2(ByVal data() As Integer, ByVal allowSz1 As Boolean, ByVal paramName As String) As Integer()
			If data Is Nothing Then
				Return Nothing
			End If


			If allowSz1 Then
				Preconditions.checkArgument(data.Length = 1 OrElse data.Length = 2, "Need either 1 or 2 %s values, got %s values: %s", paramName, data.Length, data)
			Else
				Preconditions.checkArgument(data.Length = 2,"Need 2 %s values, got %s values: %s", paramName, data.Length, data)
			End If

			If data.Length = 1 Then
				Return New Integer(){data(0), data(0)}
			Else
				Return data
			End If
		End Function

		''' <summary>
		''' Reformats the input array to a 2x2 array and checks that all values are >= 0.
		''' 
		''' If the array is 2x1 ([[a], [b]]), returns [[a, a], [b, b]]
		''' If the array is 1x2 ([[a, b]]), returns [[a, b], [a, b]]
		''' If the array is 2x2, returns the array
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 2 that represents the input </returns>
		Public Shared Function validate2x2NonNegative(ByVal data()() As Integer, ByVal paramName As String) As Integer()()
			For Each part As Integer() In data
				validateNonNegative(part, paramName)
			Next part

			Return validate2x2(data, paramName)
		End Function

		''' <summary>
		''' Reformats the input array to a 2x2 array.
		''' 
		''' If the array is 2x1 ([[a], [b]]), returns [[a, a], [b, b]]
		''' If the array is 1x2 ([[a, b]]), returns [[a, b], [a, b]]
		''' If the array is 2x2, returns the array
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 2 that represents the input </returns>
		Public Shared Function validate2x2(ByVal data()() As Integer, ByVal paramName As String) As Integer()()
			If data Is Nothing Then
				Return Nothing
			End If

			Preconditions.checkArgument((data.Length = 1 AndAlso data(0).Length = 2) OrElse (data.Length = 2 AndAlso (data(0).Length = 1 OrElse data(0).Length = 2) AndAlso (data(1).Length = 1 OrElse data(1).Length = 2) AndAlso data(0).Length = data(1).Length), "Value for %s must have shape 2x1, 1x2, or 2x2, got %sx%s shaped array: %s", paramName, data.Length, data(0).Length, data)

			If data.Length = 1 Then
				Return New Integer()(){ data(0), data(0) }
			ElseIf data(0).Length = 1 Then
				Return New Integer()(){
					New Integer(){data(0)(0), data(0)(0)},
					New Integer(){data(1)(0), data(1)(0)}
				}
			Else
				Return data
			End If
		End Function

		''' <summary>
		''' Reformats the input array to a length 3 array and checks that all values >= 0.
		''' 
		''' If the array is length 1, returns [a, a, a]
		''' If the array is length 3, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 3 that represents the input </returns>
		Public Shared Function validate3NonNegative(ByVal data() As Integer, ByVal paramName As String) As Integer()
			validateNonNegative(data, paramName)
			Return validate3(data, paramName)
		End Function

		''' <summary>
		''' Reformats the input array to a length 3 array.
		''' 
		''' If the array is length 1, returns [a, a, a]
		''' If the array is length 3, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 3 that represents the input </returns>
		Public Shared Function validate3(ByVal data() As Integer, ByVal paramName As String) As Integer()
			If data Is Nothing Then
				Return Nothing
			End If

			Preconditions.checkArgument(data.Length = 1 OrElse data.Length = 3, "Need either 1 or 3 %s values, got %s values: %s", paramName, data.Length, data)

			If data.Length = 1 Then
				Return New Integer(){data(0), data(0), data(0)}
			Else
				Return data
			End If
		End Function

		''' <summary>
		''' Reformats the input array to a length 4 array and checks that all values >= 0.
		''' 
		''' If the array is length 1, returns [a, a, a, a]
		''' If the array is length 2, return [a, a, b, b]
		''' If the array is length 4, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 4 that represents the input </returns>
		Public Shared Function validate4NonNegative(ByVal data() As Integer, ByVal paramName As String) As Integer()
			validateNonNegative(data, paramName)
			Return validate4(data, paramName)
		End Function

		''' <summary>
		''' Reformats the input array to a length 4 array.
		''' 
		''' If the array is length 1, returns [a, a, a, a]
		''' If the array is length 2, return [a, a, b, b]
		''' If the array is length 4, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 4 that represents the input </returns>
		Public Shared Function validate4(ByVal data() As Integer, ByVal paramName As String) As Integer()
			If data Is Nothing Then
				Return Nothing
			End If

			Preconditions.checkArgument(data.Length = 1 OrElse data.Length = 2 OrElse data.Length = 4, "Need either 1, 2, or 4 %s values, got %s values: %s", paramName, data.Length, data)

			If data.Length = 1 Then
				Return New Integer(){data(0), data(0), data(0), data(0)}
			ElseIf data.Length = 2 Then
				Return New Integer(){data(0), data(0), data(1), data(1)}
			Else
				Return data
			End If
		End Function

		''' <summary>
		''' Reformats the input array to a length 6 array and checks that all values >= 0.
		''' 
		''' If the array is length 1, returns [a, a, a, a, a, a]
		''' If the array is length 3, return [a, a, b, b, c, c]
		''' If the array is length 6, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 6 that represents the input </returns>
		Public Shared Function validate6NonNegative(ByVal data() As Integer, ByVal paramName As String) As Integer()
			validateNonNegative(data, paramName)
			Return validate6(data, paramName)
		End Function

		''' <summary>
		''' Reformats the input array to a length 6 array.
		''' 
		''' If the array is length 1, returns [a, a, a, a, a, a]
		''' If the array is length 3, return [a, a, b, b, c, c]
		''' If the array is length 6, returns the array.
		''' </summary>
		''' <param name="data"> An array </param>
		''' <param name="paramName"> The param name, for error reporting </param>
		''' <returns> An int array of length 6 that represents the input </returns>
		Public Shared Function validate6(ByVal data() As Integer, ByVal paramName As String) As Integer()
			If data Is Nothing Then
				Return Nothing
			End If

			Preconditions.checkArgument(data.Length = 1 OrElse data.Length = 3 OrElse data.Length = 6, "Need either 1, 3, or 6 %s values, got %s values: %s", paramName, data.Length, data)

			If data.Length = 1 Then
				Return New Integer(){data(0), data(0), data(0), data(0), data(0), data(0)}
			ElseIf data.Length = 3 Then
				Return New Integer(){data(0), data(0), data(1), data(1), data(2), data(2)}
			Else
				Return data
			End If
		End Function
	End Class
End Namespace