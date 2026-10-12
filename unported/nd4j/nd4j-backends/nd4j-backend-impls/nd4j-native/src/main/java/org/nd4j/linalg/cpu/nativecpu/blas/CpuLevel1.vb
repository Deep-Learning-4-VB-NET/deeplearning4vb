Imports System
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports BaseLevel1 = org.nd4j.linalg.api.blas.impl.BaseLevel1
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Dot = org.nd4j.linalg.api.ops.impl.reduce3.Dot
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBlas = org.nd4j.nativeblas.Nd4jBlas
Imports org.bytedeco.openblas.global.openblas_nolapack

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

Namespace org.nd4j.linalg.cpu.nativecpu.blas


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class CpuLevel1
		Inherits BaseLevel1

		Private nd4jBlas As Nd4jBlas = DirectCast(Nd4j.factory().blas(), Nd4jBlas)

		Protected Friend Overrides Function sdsdot(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
			Return cblas_sdsdot(CInt(N), alpha, CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Function

		Protected Friend Overrides Function dsdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double
			Return cblas_dsdot(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Function

		Protected Friend Overrides Function hdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function hdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function sdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
			If incX >= 1 AndAlso incY >= 1 Then
				Return cblas_sdot(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY)
			Else
				' non-EWS dot variant
				Dim dot As New Dot(X, Y)
				Nd4j.Executioner.exec(dot)
				Return dot.FinalResult.floatValue()
			End If
		End Function

		Protected Friend Overrides Function sdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function ddot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double
			If incX >= 1 AndAlso incY >= 1 Then
				Return cblas_ddot(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY)
			Else
				' non-EWS dot variant
				Dim dot As New Dot(X, Y)
				Nd4j.Executioner.exec(dot)
				Return dot.FinalResult.doubleValue()
			End If
		End Function

		Protected Friend Overrides Function ddot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Double
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function snrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
			Return cblas_snrm2(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX)

		End Function

		Protected Friend Overrides Function sasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
			Return cblas_sasum(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX)
		End Function

		Protected Friend Overrides Function sasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function dnrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double
			Return cblas_dnrm2(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX)
		End Function

		Protected Friend Overrides Function dasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double
			Return cblas_dasum(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX)
		End Function

		Protected Friend Overrides Function dasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Double
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function isamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer
			Return CInt(Math.Truncate(cblas_isamax(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX)))
		End Function

		Protected Friend Overrides Function isamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function idamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer
			Return CInt(Math.Truncate(cblas_idamax(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX)))
		End Function

		Protected Friend Overrides Function idamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Sub sswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_sswap(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Sub

		Protected Friend Overrides Sub scopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_scopy(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Sub

		Protected Friend Overrides Sub scopy(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub haxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub saxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_saxpy(CInt(N), alpha, CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY)
		End Sub

		Public Overrides Sub haxpy(ByVal n As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Public Overrides Sub saxpy(ByVal n As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException()
		End Sub


		Protected Friend Overrides Sub dswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_dswap(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY)
		End Sub

		Protected Friend Overrides Sub dcopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_dcopy(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY)
		End Sub

		Protected Friend Overrides Sub dcopy(ByVal n As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub daxpy(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			cblas_daxpy(CInt(N), alpha, CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY)

		End Sub

		Public Overrides Sub daxpy(ByVal n As Long, ByVal alpha As Double, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub srotg(ByVal a As Single, ByVal b As Single, ByVal c As Single, ByVal s As Single)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub srotmg(ByVal d1 As Single, ByVal d2 As Single, ByVal b1 As Single, ByVal b2 As Single, ByVal P As INDArray)
			cblas_srotmg(New FloatPointer(d1), New FloatPointer(d2), New FloatPointer(b1), b2, CType(P.data().addressPointer(), FloatPointer))
		End Sub

		Protected Friend Overrides Sub srot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Single, ByVal s As Single)
			cblas_srot(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY, c, s)
		End Sub

		Protected Friend Overrides Sub srotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)
			cblas_srotm(CInt(N), CType(X.data().addressPointer(), FloatPointer), incX, CType(Y.data().addressPointer(), FloatPointer), incY, CType(P.data().addressPointer(), FloatPointer))

		End Sub

		Protected Friend Overrides Sub drotg(ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal s As Double)
			cblas_drotg(New DoublePointer(a), New DoublePointer(b), New DoublePointer(c), New DoublePointer(s))
		End Sub

		Protected Friend Overrides Sub drotmg(ByVal d1 As Double, ByVal d2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal P As INDArray)
			cblas_drotmg(New DoublePointer(d1), New DoublePointer(d2), New DoublePointer(b1), b2, CType(P.data().addressPointer(), DoublePointer))
		End Sub

		Protected Friend Overrides Sub drot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Double, ByVal s As Double)
			cblas_drot(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY, c, s)
		End Sub


		Protected Friend Overrides Sub drotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)
			cblas_drotm(CInt(N), CType(X.data().addressPointer(), DoublePointer), incX, CType(Y.data().addressPointer(), DoublePointer), incY, CType(P.data().addressPointer(), DoublePointer))
		End Sub

		Protected Friend Overrides Sub sscal(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer)
			cblas_sscal(CInt(N), alpha, CType(X.data().addressPointer(), FloatPointer), incX)
		End Sub

		Protected Friend Overrides Sub dscal(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer)
			cblas_dscal(CInt(N), alpha, CType(X.data().addressPointer(), DoublePointer), incX)
		End Sub

		Protected Friend Overrides Function hasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function hasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace