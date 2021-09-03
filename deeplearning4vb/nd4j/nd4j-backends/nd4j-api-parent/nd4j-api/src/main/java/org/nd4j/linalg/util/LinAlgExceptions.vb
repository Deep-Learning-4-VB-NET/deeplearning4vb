Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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

Namespace org.nd4j.linalg.util


	Public Class LinAlgExceptions
		''' <summary>
		''' Asserts both arrays be the same length </summary>
		''' <param name="x"> </param>
		''' <param name="z"> </param>
		Public Shared Sub assertSameLength(ByVal x As INDArray, ByVal z As INDArray)
			Dim lengthX As val = x.length()
			Dim lengthZ As val = z.length()
			If lengthX <> lengthZ AndAlso lengthX <> 1 AndAlso lengthZ <> 1 Then
				Throw New System.InvalidOperationException("Mis matched lengths: [" & x.length() & "] != [" & z.length() & "] - " & "Array 1 shape: " & Arrays.toString(x.shape()) & ", array 2 shape: " & Arrays.toString(z.shape()))
			End If
		End Sub

		Public Shared Sub assertSameLength(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Dim lengthX As val = x.length()
			Dim lengthY As val = y.length()
			Dim lengthZ As val = If(z IsNot Nothing, z.length(), x.length())

			If lengthX <> lengthY AndAlso lengthX <> lengthZ AndAlso lengthX <> 1 AndAlso lengthY <> 1 AndAlso lengthZ <> 1 Then
				Throw New System.InvalidOperationException("Mis matched lengths: [" & lengthX & "] != [" & lengthY & "] != [" & lengthZ & "] - " & "Array 1 shape: " & Arrays.toString(x.shape()) & ", array 2 shape: " & Arrays.toString(y.shape()) & ", array 3 shape: " & Arrays.toString(z.shape()))
			End If
		End Sub

		Public Shared Sub assertSameShape(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			'if (!Shape.isVector(x.shape()) && ! Shape.isVector(y.shape()) && !Shape.isVector(z.shape())) {
				If Not Shape.shapeEquals(x.shape(), y.shape()) Then
					Throw New System.InvalidOperationException("Mis matched shapes: " & Arrays.toString(x.shape()) & ", " & Arrays.toString(y.shape()))
				End If
				If Not Shape.shapeEquals(x.shape(), z.shape()) Then
					Throw New System.InvalidOperationException("Mis matched shapes: " & Arrays.toString(x.shape()) & ", " & Arrays.toString(z.shape()))
				End If
			'}
		End Sub

		Public Shared Sub assertSameShape(ByVal n As INDArray, ByVal n2 As INDArray)
			If Not Shape.isVector(n.shape()) AndAlso Not Shape.isVector(n2.shape()) Then
				If Not Shape.shapeEquals(n.shape(), n2.shape()) Then
					Throw New System.InvalidOperationException("Mis matched shapes: " & Arrays.toString(n.shape()) & ", " & Arrays.toString(n2.shape()))
				End If
			End If
		End Sub

		Public Shared Sub assertRows(ByVal n As INDArray, ByVal n2 As INDArray)
			If n.rows() <> n2.rows() Then
				Throw New System.InvalidOperationException("Mis matched rows: " & n.rows() & " != " & n2.rows())
			End If
		End Sub


		Public Shared Sub assertVector(ParamArray ByVal arr() As INDArray)
			For Each a1 As INDArray In arr
				assertVector(a1)
			Next a1
		End Sub

		Public Shared Sub assertMatrix(ParamArray ByVal arr() As INDArray)
			For Each a1 As INDArray In arr
				assertMatrix(a1)
			Next a1
		End Sub

		Public Shared Sub assertVector(ByVal arr As INDArray)
			If Not arr.Vector Then
				Throw New System.ArgumentException("Array must be a vector. Array has shape: " & Arrays.toString(arr.shape()))
			End If
		End Sub

		Public Shared Sub assertMatrix(ByVal arr As INDArray)
			If arr.shape().Length > 2 Then
				Throw New System.ArgumentException("Array must be a matrix. Array has shape: " & Arrays.toString(arr.shape()))
			End If
		End Sub



		''' <summary>
		''' Asserts matrix multiply rules (columns of left == rows of right or rows of left == columns of right)
		''' </summary>
		''' <param name="nd1"> the left ndarray </param>
		''' <param name="nd2"> the right ndarray </param>
		Public Shared Sub assertMultiplies(ByVal nd1 As INDArray, ByVal nd2 As INDArray)
			If nd1.rank() = 2 AndAlso nd2.rank() = 2 AndAlso nd1.columns() = nd2.rows() Then
				Return
			End If

			' 1D edge case
			If nd1.rank() = 2 AndAlso nd2.rank() = 1 AndAlso nd1.columns() = nd2.length() Then
				Return
			End If

			Throw New ND4JIllegalStateException("Cannot execute matrix multiplication: " & Arrays.toString(nd1.shape()) & "x" & Arrays.toString(nd2.shape()) & (If(nd1.rank() <> 2 OrElse nd2.rank() <> 2, ": inputs are not matrices", ": Column of left array " & nd1.columns() & " != rows of right " & nd2.rows())))
		End Sub


		Public Shared Sub assertColumns(ByVal n As INDArray, ByVal n2 As INDArray)
			If n.columns() <> n2.columns() Then
				Throw New System.InvalidOperationException("Mis matched columns: " & n.columns() & " != " & n2.columns())
			End If
		End Sub

		Public Shared Sub assertValidNum(ByVal n As INDArray)
			Dim linear As INDArray = n.reshape(ChrW(-1))
			For i As Integer = 0 To linear.length() - 1
				Dim d As Double = linear.getDouble(i)
				If Double.IsNaN(d) OrElse Double.IsInfinity(d) Then
					Throw New System.InvalidOperationException("Found infinite or nan")
				End If

			Next i
		End Sub

	End Class

End Namespace