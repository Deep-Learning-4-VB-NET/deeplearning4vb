Imports BlasBufferUtil = org.nd4j.linalg.api.blas.BlasBufferUtil
Imports Level1 = org.nd4j.linalg.api.blas.Level1
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DefaultOpExecutioner = org.nd4j.linalg.api.ops.executioner.DefaultOpExecutioner
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports ScalarMultiplication = org.nd4j.linalg.api.ops.impl.scalar.ScalarMultiplication
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler

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

Namespace org.nd4j.linalg.api.blas.impl

	Public MustInherit Class BaseLevel1
		Inherits BaseLevel
		Implements Level1

		''' <summary>
		''' computes a vector-vector dot product.
		''' </summary>
		''' <param name="n"> number of accessed element </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> an INDArray </param>
		''' <param name="Y"> an INDArray </param>
		''' <returns> the vector-vector dot product of X and Y </returns>
		Public Overridable Function dot(ByVal n As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal Y As INDArray) As Double Implements Level1.dot
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, X, Y)
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, X, Y)
				Return ddot(n, X, BlasBufferUtil.getBlasStride(X), Y, BlasBufferUtil.getBlasStride(Y))
			ElseIf X.data().dataType() = DataType.FLOAT Then
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, X, Y)
				Return sdot(n, X, BlasBufferUtil.getBlasStride(X), Y, BlasBufferUtil.getBlasStride(Y))
			Else
				DefaultOpExecutioner.validateDataType(DataType.HALF, X, Y)
				Return hdot(n, X, BlasBufferUtil.getBlasStride(X), Y, BlasBufferUtil.getBlasStride(Y))
			End If

		End Function

		Public Overridable Function dot(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer) As Double Implements Level1.dot
			If supportsDataBufferL1Ops() Then
				If x.dataType() = DataType.FLOAT Then
					Return sdot(n, x, offsetX, incrX, y, offsetY, incrY)
				ElseIf x.dataType() = DataType.DOUBLE Then
					Return ddot(n, x, offsetX, incrX, y, offsetY, incrY)
				Else
					Return hdot(n, x, offsetX, incrX, y, offsetY, incrY)
				End If
			Else
				Dim shapex() As Long = {1, n}
				Dim shapey() As Long = {1, n}
				Dim stridex() As Long = {incrX, incrX}
				Dim stridey() As Long = {incrY, incrY}
				Dim arrX As INDArray = Nd4j.create(x, shapex, stridex, offsetX, "c"c)
				Dim arrY As INDArray = Nd4j.create(x, shapey, stridey, offsetY, "c"c)
				Return dot(n, 0.0, arrX, arrY)
			End If
		End Function

		''' <summary>
		''' computes the Euclidean norm of a vector.
		''' </summary>
		''' <param name="arr">
		''' @return </param>
		Public Overridable Function nrm2(ByVal arr As INDArray) As Double Implements Level1.nrm2

			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, arr)
			End If

			If arr.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, arr)
				Return dnrm2(arr.length(), arr, BlasBufferUtil.getBlasStride(arr))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, arr)
				Return snrm2(arr.length(), arr, BlasBufferUtil.getBlasStride(arr))
			End If
			' TODO: add nrm2 for half, as call to appropriate NativeOp<HALF>
		End Function

		''' <summary>
		''' computes the sum of magnitudes of all vector elements or, for a complex vector x, the sum
		''' </summary>
		''' <param name="arr">
		''' @return </param>
		Public Overridable Function asum(ByVal arr As INDArray) As Double Implements Level1.asum

			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, arr)
			End If

			If arr.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, arr)
				Return dasum(arr.length(), arr, BlasBufferUtil.getBlasStride(arr))
			ElseIf arr.data().dataType() = DataType.FLOAT Then
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, arr)
				Return sasum(arr.length(), arr, BlasBufferUtil.getBlasStride(arr))
			Else
				DefaultOpExecutioner.validateDataType(DataType.HALF, arr)
				Return hasum(arr.length(), arr, BlasBufferUtil.getBlasStride(arr))
			End If
		End Function

		Public Overridable Function asum(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer) As Double Implements Level1.asum
			If supportsDataBufferL1Ops() Then
				If x.dataType() = DataType.FLOAT Then
					Return sasum(n, x, offsetX, incrX)
				ElseIf x.dataType() = DataType.DOUBLE Then
					Return dasum(n, x, offsetX, incrX)
				Else
					Return hasum(n, x, offsetX, incrX)
				End If
			Else
				Dim shapex() As Long = {1, n}
				Dim stridex() As Long = {incrX, incrX}
				Dim arrX As INDArray = Nd4j.create(x, shapex, stridex, offsetX, "c"c)
				Return asum(arrX)
			End If
		End Function

		Public Overridable Function iamax(ByVal n As Long, ByVal arr As INDArray, ByVal stride As Integer) As Integer Implements Level1.iamax
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, arr)
			End If

			If arr.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, arr)
				Return idamax(n, arr, stride)
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, arr)
				Return isamax(n, arr, stride)
			End If
		End Function

		Public Overridable Function iamax(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer) As Integer Implements Level1.iamax
			If supportsDataBufferL1Ops() Then
				If x.dataType() = DataType.FLOAT Then
					Return isamax(n, x, offsetX, incrX)
				Else
					Return isamax(n, x, offsetX, incrX)
				End If
			Else
				Dim shapex() As Long = {1, n}
				Dim stridex() As Long = {incrX, incrX}
				Dim arrX As INDArray = Nd4j.create(x, shapex, stridex, offsetX, "c"c)
				Return iamax(n, arrX, incrX)
			End If
		End Function

		''' <summary>
		''' finds the element of a
		''' vector that has the largest absolute value.
		''' </summary>
		''' <param name="arr">
		''' @return </param>
		Public Overridable Function iamax(ByVal arr As INDArray) As Integer Implements Level1.iamax

			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, arr)
			End If

			If arr.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, arr)
				Return idamax(arr.length(), arr, BlasBufferUtil.getBlasStride(arr))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, arr)
				Return isamax(arr.length(), arr, BlasBufferUtil.getBlasStride(arr))
			End If
		End Function

		''' <summary>
		''' finds the element of a vector that has the minimum absolute value.
		''' </summary>
		''' <param name="arr">
		''' @return </param>
		Public Overridable Function iamin(ByVal arr As INDArray) As Integer Implements Level1.iamin
				Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' swaps a vector with another vector.
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		Public Overridable Sub swap(ByVal x As INDArray, ByVal y As INDArray) Implements Level1.swap
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, x, y)
			End If

			If x.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, x, y)
				dswap(x.length(), x, BlasBufferUtil.getBlasStride(x), y, BlasBufferUtil.getBlasStride(y))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, x, y)
				sswap(x.length(), x, BlasBufferUtil.getBlasStride(x), y, BlasBufferUtil.getBlasStride(y))
			End If
		End Sub


		''' <summary>
		''' swaps a vector with another vector.
		''' </summary>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		Public Overridable Sub copy(ByVal x As INDArray, ByVal y As INDArray) Implements Level1.copy
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, x, y)
			End If

			If x.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, x, y)
				dcopy(x.length(), x, BlasBufferUtil.getBlasStride(x), y, BlasBufferUtil.getBlasStride(y))
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, x, y)
				scopy(x.length(), x, BlasBufferUtil.getBlasStride(x), y, BlasBufferUtil.getBlasStride(y))
			End If
		End Sub

		''' <summary>
		'''copy a vector to another vector.
		''' </summary>
		Public Overridable Sub copy(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer) Implements Level1.copy


			If supportsDataBufferL1Ops() Then
				If x.dataType() = DataType.DOUBLE Then
					dcopy(n, x, offsetX, incrX, y, offsetY, incrY)
				Else
					scopy(n, x, offsetX, incrX, y, offsetY, incrY)
				End If
			Else
				Dim shapex() As Long = {1, n}
				Dim shapey() As Long = {1, n}
				Dim stridex() As Long = {incrX, incrX}
				Dim stridey() As Long = {incrY, incrY}
				Dim arrX As INDArray = Nd4j.create(x, shapex, stridex, offsetX, "c"c)
				Dim arrY As INDArray = Nd4j.create(x, shapey, stridey, offsetY, "c"c)
				copy(arrX, arrY)
			End If
		End Sub



		''' <summary>
		''' computes a vector-scalar product and adds the result to a vector.
		''' </summary>
		''' <param name="n"> </param>
		''' <param name="alpha"> </param>
		''' <param name="x"> </param>
		''' <param name="y"> </param>
		Public Overridable Sub axpy(ByVal n As Long, ByVal alpha As Double, ByVal x As INDArray, ByVal y As INDArray) Implements Level1.axpy

			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, x, y)
			End If

			If x.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, x, y)
				daxpy(n, alpha, x, BlasBufferUtil.getBlasStride(x), y, BlasBufferUtil.getBlasStride(y))
			ElseIf x.data().dataType() = DataType.FLOAT Then
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, x, y)
				saxpy(n, CSng(alpha), x, BlasBufferUtil.getBlasStride(x), y, BlasBufferUtil.getBlasStride(y))
			Else
				DefaultOpExecutioner.validateDataType(DataType.HALF, x, y)
				haxpy(n, CSng(alpha), x, BlasBufferUtil.getBlasStride(x), y, BlasBufferUtil.getBlasStride(y))
			End If
		End Sub

		Public Overridable Sub axpy(ByVal n As Long, ByVal alpha As Double, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer) Implements Level1.axpy
			If supportsDataBufferL1Ops() Then
				If x.dataType() = DataType.DOUBLE Then
					daxpy(n, alpha, x, offsetX, incrX, y, offsetY, incrY)
				ElseIf x.dataType() = DataType.FLOAT Then
					saxpy(n, CSng(alpha), x, offsetX, incrX, y, offsetY, incrY)
				Else
					haxpy(n, CSng(alpha), x, offsetX, incrX, y, offsetY, incrY)
				End If
			Else
				Dim shapex() As Long = {1, n}
				Dim shapey() As Long = {1, n}
				Dim stridex() As Long = {incrX, incrX}
				Dim stridey() As Long = {incrY, incrY}
				Dim arrX As INDArray = Nd4j.create(x, shapex, stridex, offsetX, "c"c)
				Dim arrY As INDArray = Nd4j.create(x, shapey, stridey, offsetY, "c"c)
				axpy(n, alpha, arrX, arrY)
			End If
		End Sub

		''' <summary>
		''' computes parameters for a Givens rotation.
		''' </summary>
		''' <param name="a"> </param>
		''' <param name="b"> </param>
		''' <param name="c"> </param>
		''' <param name="s"> </param>
		Public Overridable Sub rotg(ByVal a As INDArray, ByVal b As INDArray, ByVal c As INDArray, ByVal s As INDArray) Implements Level1.rotg
			Throw New System.NotSupportedException()
		End Sub

		''' <summary>
		''' performs rotation of points in the plane.
		''' </summary>
		''' <param name="N"> </param>
		''' <param name="X"> </param>
		''' <param name="Y"> </param>
		''' <param name="c"> </param>
		''' <param name="s"> </param>
		Public Overridable Sub rot(ByVal N As Long, ByVal X As INDArray, ByVal Y As INDArray, ByVal c As Double, ByVal s As Double) Implements Level1.rot

			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, X, Y)
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				DefaultOpExecutioner.validateDataType(DataType.DOUBLE, X, Y)
				drot(N, X, BlasBufferUtil.getBlasStride(X), Y, BlasBufferUtil.getBlasStride(X), c, s)
			Else
				DefaultOpExecutioner.validateDataType(DataType.FLOAT, X, Y)
				srot(N, X, BlasBufferUtil.getBlasStride(X), Y, BlasBufferUtil.getBlasStride(X), CSng(c), CSng(s))
			End If
		End Sub

		''' <summary>
		''' computes the modified parameters for a Givens rotation.
		''' </summary>
		''' <param name="d1"> </param>
		''' <param name="d2"> </param>
		''' <param name="b1"> </param>
		''' <param name="b2"> </param>
		''' <param name="P"> </param>
		Public Overridable Sub rotmg(ByVal d1 As INDArray, ByVal d2 As INDArray, ByVal b1 As INDArray, ByVal b2 As Double, ByVal P As INDArray) Implements Level1.rotmg
			Throw New System.NotSupportedException()
		End Sub

		''' <summary>
		''' computes a vector by a scalar product.
		''' </summary>
		''' <param name="N"> </param>
		''' <param name="alpha"> </param>
		''' <param name="X"> </param>
		Public Overridable Sub scal(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray) Implements Level1.scal
			If Nd4j.Executioner.ProfilingMode = OpExecutioner.ProfilingMode.ALL Then
				OpProfiler.Instance.processBlasCall(False, X)
			End If

			If X.data().dataType() = DataType.DOUBLE Then
				dscal(N, alpha, X, BlasBufferUtil.getBlasStride(X))
			ElseIf X.data().dataType() = DataType.FLOAT Then
				sscal(N, CSng(alpha), X, BlasBufferUtil.getBlasStride(X))
			ElseIf X.data().dataType() = DataType.HALF Then
				Nd4j.Executioner.exec(New ScalarMultiplication(X, alpha))
			End If
		End Sub



	'    
	'    * ===========================================================================
	'    * Prototypes for level 1 BLAS functions (complex are recast as routines)
	'    * ===========================================================================
	'    
		Protected Friend MustOverride Function sdsdot(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single

		Protected Friend MustOverride Function dsdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double

		Protected Friend MustOverride Function hdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single

		Protected Friend MustOverride Function hdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single

		Protected Friend MustOverride Function sdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single

		Protected Friend MustOverride Function sdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single

		Protected Friend MustOverride Function ddot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double

		Protected Friend MustOverride Function ddot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Double


	'    
	'     * Functions having prefixes S D SC DZ
	'     
		Protected Friend MustOverride Function snrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single

		Protected Friend MustOverride Function hasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single

		Protected Friend MustOverride Function hasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single

		Protected Friend MustOverride Function sasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single

		Protected Friend MustOverride Function sasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single

		Protected Friend MustOverride Function dnrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double

		Protected Friend MustOverride Function dasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double

		Protected Friend MustOverride Function dasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Double


	'    
	'     * Functions having standard 4 prefixes (S D C Z)
	'     
		Protected Friend MustOverride Function isamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer

		Protected Friend MustOverride Function isamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer

		Protected Friend MustOverride Function idamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer

		Protected Friend MustOverride Function idamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer
	'    
	'     * ===========================================================================
	'     * Prototypes for level 1 BLAS routines
	'     * ===========================================================================
	'     

	'    
	'     * Routines with standard 4 prefixes (s, d, c, z)
	'     
		Protected Friend MustOverride Sub sswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub scopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub scopy(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)

		Protected Friend MustOverride Sub haxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub saxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub haxpy(ByVal N As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)

		Protected Friend MustOverride Sub saxpy(ByVal N As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)

		Protected Friend MustOverride Sub dswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub dcopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub dcopy(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)

		Protected Friend MustOverride Sub daxpy(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

		Protected Friend MustOverride Sub daxpy(ByVal N As Long, ByVal alpha As Double, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)


	'    
	'     * Routines with S and D prefix only
	'     
		Protected Friend MustOverride Sub srotg(ByVal a As Single, ByVal b As Single, ByVal c As Single, ByVal s As Single)

		Protected Friend MustOverride Sub srotmg(ByVal d1 As Single, ByVal d2 As Single, ByVal b1 As Single, ByVal b2 As Single, ByVal P As INDArray)

		Protected Friend MustOverride Sub srot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Single, ByVal s As Single)

		Protected Friend MustOverride Sub srotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)

		Protected Friend MustOverride Sub drotg(ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal s As Double)

		Protected Friend MustOverride Sub drotmg(ByVal d1 As Double, ByVal d2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal P As INDArray)

		Protected Friend MustOverride Sub drot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Double, ByVal s As Double)


		Protected Friend MustOverride Sub drotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)

	'    
	'         * Routines with S D C Z CS and ZD prefixes
	'         
		Protected Friend MustOverride Sub sscal(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer)

		Protected Friend MustOverride Sub dscal(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer)

		Public Overridable Function supportsDataBufferL1Ops() As Boolean Implements Level1.supportsDataBufferL1Ops
			Return True
		End Function

	End Class

End Namespace