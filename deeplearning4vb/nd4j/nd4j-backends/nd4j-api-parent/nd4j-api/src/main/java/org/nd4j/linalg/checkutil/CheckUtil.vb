Imports System
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports BlockRealMatrix = org.apache.commons.math3.linear.BlockRealMatrix
Imports RealMatrix = org.apache.commons.math3.linear.RealMatrix
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.checkutil


	Public Class CheckUtil

		''' <summary>
		'''Check first.mmul(second) using Apache commons math mmul. Float/double matrices only.<br>
		''' Returns true if OK, false otherwise.<br>
		''' Checks each element according to relative error (|a-b|/(|a|+|b|); however absolute error |a-b| must
		''' also exceed minAbsDifference for it to be considered a failure. This is necessary to avoid instability
		''' near 0: i.e., Nd4j mmul might return element of 0.0 (due to underflow on float) while Apache commons math
		''' mmul might be say 1e-30 or something (using doubles). 
		''' Throws exception if matrices can't be multiplied
		''' Checks each element of the result. If </summary>
		''' <param name="first"> First matrix </param>
		''' <param name="second"> Second matrix </param>
		''' <param name="maxRelativeDifference"> Maximum relative error </param>
		''' <param name="minAbsDifference"> Minimum absolute difference for failure </param>
		''' <returns> true if OK, false if result incorrect </returns>
		Public Shared Function checkMmul(ByVal first As INDArray, ByVal second As INDArray, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			If first.size(1) <> second.size(0) Then
				Throw New System.ArgumentException("first.columns != second.rows")
			End If
			Dim rmFirst As RealMatrix = convertToApacheMatrix(first)
			Dim rmSecond As RealMatrix = convertToApacheMatrix(second)

			Dim result As INDArray = first.mmul(second)
			Dim rmResult As RealMatrix = rmFirst.multiply(rmSecond)

			If Not checkShape(rmResult, result) Then
				Return False
			End If
			Dim ok As Boolean = checkEntries(rmResult, result, maxRelativeDifference, minAbsDifference)
			If Not ok Then
				Dim onCopies As INDArray = Shape.toOffsetZeroCopy(first).mmul(Shape.toOffsetZeroCopy(second))
				printFailureDetails(first, second, rmResult, result, onCopies, "mmul")
			End If
			Return ok
		End Function

		Public Shared Function checkGemm(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			Dim commonDimA As Long = (If(transposeA, a.rows(), a.columns()))
			Dim commonDimB As Long = (If(transposeB, b.columns(), b.rows()))
			If commonDimA <> commonDimB Then
				Throw New System.ArgumentException("Common dimensions don't match: a.shape=" & Arrays.toString(a.shape()) & ", b.shape=" & Arrays.toString(b.shape()) & ", tA=" & transposeA & ", tb=" & transposeB)
			End If
			Dim outRows As Long = (If(transposeA, a.columns(), a.rows()))
			Dim outCols As Long = (If(transposeB, b.rows(), b.columns()))
			If c.rows() <> outRows OrElse c.columns() <> outCols Then
				Throw New System.ArgumentException("C does not match outRows or outCols")
			End If
			If c.offset() <> 0 OrElse c.ordering() <> "f"c Then
				Throw New System.ArgumentException("Invalid c")
			End If

			Dim aConvert As INDArray = If(transposeA, a.transpose(), a)
			Dim rmA As RealMatrix = convertToApacheMatrix(aConvert)
			Dim bConvet As INDArray = If(transposeB, b.transpose(), b)
			Dim rmB As RealMatrix = convertToApacheMatrix(bConvet)
			Dim rmC As RealMatrix = convertToApacheMatrix(c)
			Dim rmExpected As RealMatrix = rmA.scalarMultiply(alpha).multiply(rmB).add(rmC.scalarMultiply(beta))
			Dim cCopy1 As INDArray = Nd4j.create(c.shape(), "f"c)
			cCopy1.assign(c)
			Dim cCopy2 As INDArray = Nd4j.create(c.shape(), "f"c)
			cCopy2.assign(c)

			Dim [out] As INDArray = Nd4j.gemm(a, b, c, transposeA, transposeB, alpha, beta)
			If [out] IsNot c Then
				Console.WriteLine("Returned different array than c")
				Return False
			End If
			If Not checkShape(rmExpected, [out]) Then
				Return False
			End If
			Dim ok As Boolean = checkEntries(rmExpected, [out], maxRelativeDifference, minAbsDifference)
			If Not ok Then
				Dim aCopy As INDArray = Shape.toOffsetZeroCopy(a)
				Dim bCopy As INDArray = Shape.toOffsetZeroCopy(b)
				Dim onCopies As INDArray = Nd4j.gemm(aCopy, bCopy, cCopy1, transposeA, transposeB, alpha, beta)
				printGemmFailureDetails(a, b, cCopy2, transposeA, transposeB, alpha, beta, rmExpected, [out], onCopies)
			End If
			Return ok
		End Function

		''' <summary>
		'''Same as checkMmul, but for matrix addition </summary>
		Public Shared Function checkAdd(ByVal first As INDArray, ByVal second As INDArray, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			Dim rmFirst As RealMatrix = convertToApacheMatrix(first)
			Dim rmSecond As RealMatrix = convertToApacheMatrix(second)

			Dim result As INDArray = first.add(second)
			Dim rmResult As RealMatrix = rmFirst.add(rmSecond)

			If Not checkShape(rmResult, result) Then
				Return False
			End If
			Dim ok As Boolean = checkEntries(rmResult, result, maxRelativeDifference, minAbsDifference)
			If Not ok Then
				Dim onCopies As INDArray = Shape.toOffsetZeroCopy(first).add(Shape.toOffsetZeroCopy(second))
				printFailureDetails(first, second, rmResult, result, onCopies, "add")
			End If
			Return ok
		End Function

		''' <summary>
		''' Same as checkMmul, but for matrix subtraction </summary>
		Public Shared Function checkSubtract(ByVal first As INDArray, ByVal second As INDArray, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			Dim rmFirst As RealMatrix = convertToApacheMatrix(first)
			Dim rmSecond As RealMatrix = convertToApacheMatrix(second)

			Dim result As INDArray = first.sub(second)
			Dim rmResult As RealMatrix = rmFirst.subtract(rmSecond)

			If Not checkShape(rmResult, result) Then
				Return False
			End If
			Dim ok As Boolean = checkEntries(rmResult, result, maxRelativeDifference, minAbsDifference)
			If Not ok Then
				Dim onCopies As INDArray = Shape.toOffsetZeroCopy(first).sub(Shape.toOffsetZeroCopy(second))
				printFailureDetails(first, second, rmResult, result, onCopies, "sub")
			End If
			Return ok
		End Function

		Public Shared Function checkMulManually(ByVal first As INDArray, ByVal second As INDArray, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			'No apache commons element-wise multiply, but can do this manually

			Dim result As INDArray = first.mul(second)
			Dim shape() As Long = first.shape()

			Dim expected As INDArray = Nd4j.zeros(first.shape())

			Dim i As Integer = 0
			Do While i < shape(0)
				Dim j As Integer = 0
				Do While j < shape(1)
					Dim v As Double = first.getDouble(i, j) * second.getDouble(i, j)
					expected.putScalar(New Integer() {i, j}, v)
					j += 1
				Loop
				i += 1
			Loop
			If Not checkShape(expected, result) Then
				Return False
			End If
			Dim ok As Boolean = checkEntries(expected, result, maxRelativeDifference, minAbsDifference)
			If Not ok Then
				Dim onCopies As INDArray = Shape.toOffsetZeroCopy(first).mul(Shape.toOffsetZeroCopy(second))
				printFailureDetails(first, second, expected, result, onCopies, "mul")
			End If
			Return ok
		End Function

		Public Shared Function checkDivManually(ByVal first As INDArray, ByVal second As INDArray, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			'No apache commons element-wise division, but can do this manually

			Dim result As INDArray = first.div(second)
			Dim shape() As Long = first.shape()

			Dim expected As INDArray = Nd4j.zeros(first.shape())

			Dim i As Integer = 0
			Do While i < shape(0)
				Dim j As Integer = 0
				Do While j < shape(1)
					Dim v As Double = first.getDouble(i, j) / second.getDouble(i, j)
					expected.putScalar(New Integer() {i, j}, v)
					j += 1
				Loop
				i += 1
			Loop
			If Not checkShape(expected, result) Then
				Return False
			End If
			Dim ok As Boolean = checkEntries(expected, result, maxRelativeDifference, minAbsDifference)
			If Not ok Then
				Dim onCopies As INDArray = Shape.toOffsetZeroCopy(first).mul(Shape.toOffsetZeroCopy(second))
				printFailureDetails(first, second, expected, result, onCopies, "div")
			End If
			Return ok
		End Function

		Private Shared Function checkShape(ByVal rmResult As RealMatrix, ByVal result As INDArray) As Boolean
			Dim outShape() As Long = {rmResult.getRowDimension(), rmResult.getColumnDimension()}
			If Not outShape.SequenceEqual(result.shape()) Then
				Console.WriteLine("Failure on shape: " & Arrays.toString(result.shape()) & ", expected " & Arrays.toString(outShape))
				Return False
			End If
			Return True
		End Function

		Private Shared Function checkShape(ByVal expected As INDArray, ByVal actual As INDArray) As Boolean
			If Not expected.shape().SequenceEqual(actual.shape()) Then
				Console.WriteLine("Failure on shape: " & Arrays.toString(actual.shape()) & ", expected " & Arrays.toString(expected.shape()))
				Return False
			End If
			Return True
		End Function

		Public Shared Function checkEntries(ByVal rmResult As RealMatrix, ByVal result As INDArray, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			Dim outShape() As Integer = {rmResult.getRowDimension(), rmResult.getColumnDimension()}
			Dim i As Integer = 0
			Do While i < outShape(0)
				Dim j As Integer = 0
				Do While j < outShape(1)
					Dim expOut As Double = rmResult.getEntry(i, j)
					Dim actOut As Double = result.getDouble(i, j)

					If Double.IsNaN(actOut) Then
						Console.WriteLine("NaN failure on value: (" & i & "," & j & " exp=" & expOut & ", act=" & actOut)
						Return False
					End If

					If expOut = 0.0 AndAlso actOut = 0.0 Then
						j += 1
						Continue Do
					End If
					Dim absError As Double = Math.Abs(expOut - actOut)
					Dim relError As Double = absError / (Math.Abs(expOut) + Math.Abs(actOut))
					If relError > maxRelativeDifference AndAlso absError > minAbsDifference Then
						Console.WriteLine("Failure on value: (" & i & "," & j & " exp=" & expOut & ", act=" & actOut & ", absError=" & absError & ", relError=" & relError)
						Return False
					End If
					j += 1
				Loop
				i += 1
			Loop
			Return True
		End Function

		Public Shared Function checkEntries(ByVal expected As INDArray, ByVal actual As INDArray, ByVal maxRelativeDifference As Double, ByVal minAbsDifference As Double) As Boolean
			Dim outShape() As Long = expected.shape()
			Dim i As Integer = 0
			Do While i < outShape(0)
				Dim j As Integer = 0
				Do While j < outShape(1)
					Dim expOut As Double = expected.getDouble(i, j)
					Dim actOut As Double = actual.getDouble(i, j)
					If expOut = 0.0 AndAlso actOut = 0.0 Then
						j += 1
						Continue Do
					End If
					Dim absError As Double = Math.Abs(expOut - actOut)
					Dim relError As Double = absError / (Math.Abs(expOut) + Math.Abs(actOut))
					If relError > maxRelativeDifference AndAlso absError > minAbsDifference Then
						Console.WriteLine("Failure on value: (" & i & "," & j & " exp=" & expOut & ", act=" & actOut & ", absError=" & absError & ", relError=" & relError)
						Return False
					End If
					j += 1
				Loop
				i += 1
			Loop
			Return True
		End Function

		Public Shared Function convertToApacheMatrix(ByVal matrix As INDArray) As RealMatrix
			If matrix.rank() <> 2 Then
				Throw New System.ArgumentException("Input rank is not 2 (not matrix)")
			End If
			Dim shape() As Long = matrix.shape()

			If matrix.columns() > Integer.MaxValue OrElse matrix.rows() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			Dim [out] As New BlockRealMatrix(CInt(shape(0)), CInt(shape(1)))
			Dim i As Integer = 0
			Do While i < shape(0)
				Dim j As Integer = 0
				Do While j < shape(1)
					Dim value As Double = matrix.getDouble(i, j)
					[out].setEntry(i, j, value)
					j += 1
				Loop
				i += 1
			Loop
			Return [out]
		End Function

		Public Shared Function convertFromApacheMatrix(ByVal matrix As RealMatrix, ByVal dataType As DataType) As INDArray
			Dim shape As val = New Long() {matrix.getRowDimension(), matrix.getColumnDimension()}
			Dim [out] As INDArray = Nd4j.create(dataType, shape)
			Dim i As Integer = 0
			Do While i < shape(0)
				Dim j As Integer = 0
				Do While j < shape(1)
					Dim value As Double = matrix.getEntry(i, j)
					[out].putScalar(New Integer() {i, j}, value)
					j += 1
				Loop
				i += 1
			Loop
			Return [out]
		End Function



		Public Shared Sub printFailureDetails(ByVal first As INDArray, ByVal second As INDArray, ByVal expected As RealMatrix, ByVal actual As INDArray, ByVal onCopies As INDArray, ByVal op As String)
			Console.WriteLine(vbLf & "Factory: " & Nd4j.factory().GetType() & vbLf)

			Console.WriteLine("First:")
			printMatrixFullPrecision(first)
			Console.WriteLine(vbLf & "Second:")
			printMatrixFullPrecision(second)
			Console.WriteLine(vbLf & "Expected (Apache Commons)")
			printApacheMatrix(expected)
			Console.WriteLine(vbLf & "Same Nd4j op on copies: (Shape.toOffsetZeroCopy(first)." & op & "(Shape.toOffsetZeroCopy(second)))")
			printMatrixFullPrecision(onCopies)
			Console.WriteLine(vbLf & "Actual:")
			printMatrixFullPrecision(actual)
		End Sub

		Public Shared Sub printGemmFailureDetails(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray, ByVal transposeA As Boolean, ByVal transposeB As Boolean, ByVal alpha As Double, ByVal beta As Double, ByVal expected As RealMatrix, ByVal actual As INDArray, ByVal onCopies As INDArray)
			Console.WriteLine(vbLf & "Factory: " & Nd4j.factory().GetType() & vbLf)
			Console.WriteLine("Op: gemm(a,b,c,transposeA=" & transposeA & ",transposeB=" & transposeB & ",alpha=" & alpha & ",beta=" & beta & ")")

			Console.WriteLine("a:")
			printMatrixFullPrecision(a)
			Console.WriteLine(vbLf & "b:")
			printMatrixFullPrecision(b)
			Console.WriteLine(vbLf & "c:")
			printMatrixFullPrecision(c)
			Console.WriteLine(vbLf & "Expected (Apache Commons)")
			printApacheMatrix(expected)
			Console.WriteLine(vbLf & "Same Nd4j op on zero offset copies: gemm(aCopy,bCopy,cCopy," & transposeA & "," & transposeB & "," & alpha & "," & beta & ")")
			printMatrixFullPrecision(onCopies)
			Console.WriteLine(vbLf & "Actual:")
			printMatrixFullPrecision(actual)
		End Sub

		Public Shared Sub printMatrixFullPrecision(ByVal matrix As INDArray)
			Dim floatType As Boolean = (matrix.data().dataType() = DataType.FLOAT)
			printNDArrayHeader(matrix)
			Dim shape() As Long = matrix.shape()
			Dim i As Integer = 0
			Do While i < shape(0)
				Dim j As Integer = 0
				Do While j < shape(1)
					If floatType Then
						Console.Write(matrix.getFloat(i, j))
					Else
						Console.Write(matrix.getDouble(i, j))
					End If
					If j <> shape(1) - 1 Then
						Console.Write(", ")
					Else
						Console.WriteLine()
					End If
					j += 1
				Loop
				i += 1
			Loop
		End Sub

		Public Shared Sub printNDArrayHeader(ByVal array As INDArray)
			Console.WriteLine(array.data().dataType() & " - order=" & AscW(array.ordering()) & ", offset=" & array.offset() & ", shape=" & Arrays.toString(array.shape()) & ", stride=" & Arrays.toString(array.stride()) & ", length=" & array.length() & ", data().length()=" & array.data().length())
		End Sub

		Public Shared Sub printFailureDetails(ByVal first As INDArray, ByVal second As INDArray, ByVal expected As INDArray, ByVal actual As INDArray, ByVal onCopies As INDArray, ByVal op As String)
			Console.WriteLine(vbLf & "Factory: " & Nd4j.factory().GetType() & vbLf)

			Console.WriteLine("First:")
			printMatrixFullPrecision(first)
			Console.WriteLine(vbLf & "Second:")
			printMatrixFullPrecision(second)
			Console.WriteLine(vbLf & "Expected")
			printMatrixFullPrecision(expected)
			Console.WriteLine(vbLf & "Same Nd4j op on copies: (Shape.toOffsetZeroCopy(first)." & op & "(Shape.toOffsetZeroCopy(second)))")
			printMatrixFullPrecision(onCopies)
			Console.WriteLine(vbLf & "Actual:")
			printMatrixFullPrecision(actual)
		End Sub

		Public Shared Sub printApacheMatrix(ByVal matrix As RealMatrix)
			Dim nRows As Integer = matrix.getRowDimension()
			Dim nCols As Integer = matrix.getColumnDimension()
			Console.WriteLine("Apache Commons RealMatrix: Shape: [" & nRows & "," & nCols & "]")
			For i As Integer = 0 To nRows - 1
				For j As Integer = 0 To nCols - 1
					Console.Write(matrix.getEntry(i, j))
					If j <> nCols - 1 Then
						Console.Write(", ")
					Else
						Console.WriteLine()
					End If
				Next j
			Next i
		End Sub
	End Class

End Namespace