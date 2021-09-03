Imports System
Imports BaseLapack = org.nd4j.linalg.api.blas.impl.BaseLapack
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports BlasException = org.nd4j.linalg.api.blas.BlasException
Imports org.bytedeco.openblas.global.openblas

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

	Public Class CpuLapack
		Inherits BaseLapack

		Protected Friend Shared Function getColumnOrder(ByVal A As INDArray) As Integer
			Return If(A.ordering() = "f"c, LAPACK_COL_MAJOR, LAPACK_ROW_MAJOR)
		End Function

		Protected Friend Shared Function getLda(ByVal A As INDArray) As Integer
			If A.rows() > Integer.MaxValue OrElse A.columns() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Return If(A.ordering() = "f"c, CInt(A.rows()), CInt(A.columns()))
		End Function
	'=========================    
	' L U DECOMP
		Public Overrides Sub sgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)
			Dim status As Integer = LAPACKE_sgetrf(getColumnOrder(A), M, N, CType(A.data().addressPointer(), FloatPointer), getLda(A), CType(IPIV.data().addressPointer(), IntPointer))
			If status < 0 Then
				Throw New BlasException("Failed to execute sgetrf", status)
			End If
		End Sub

		Public Overrides Sub dgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)
			Dim status As Integer = LAPACKE_dgetrf(getColumnOrder(A), M, N, CType(A.data().addressPointer(), DoublePointer), getLda(A), CType(IPIV.data().addressPointer(), IntPointer))
			If status < 0 Then
				Throw New BlasException("Failed to execute dgetrf", status)
			End If
		End Sub

	'=========================    
	' Q R DECOMP
		Public Overrides Sub sgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)
			Dim tau As INDArray = Nd4j.create(DataType.FLOAT, N)

			Dim status As Integer = LAPACKE_sgeqrf(getColumnOrder(A), M, N, CType(A.data().addressPointer(), FloatPointer), getLda(A), CType(tau.data().addressPointer(), FloatPointer))
			If status <> 0 Then
				Throw New BlasException("Failed to execute sgeqrf", status)
			End If

			' Copy R ( upper part of Q ) into result
			If R IsNot Nothing Then
				R.assign(A.get(NDArrayIndex.interval(0, A.columns()), NDArrayIndex.all()))
				Dim ix(1) As INDArrayIndex

				Dim i As Integer=1
				Do While i<Math.Min(A.rows(), A.columns())
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(0, i)
					R.put(ix, 0)
					i += 1
				Loop
			End If

			status = LAPACKE_sorgqr(getColumnOrder(A), M, N, N, CType(A.data().addressPointer(), FloatPointer), getLda(A), CType(tau.data().addressPointer(), FloatPointer))
			If status <> 0 Then
				Throw New BlasException("Failed to execute sorgqr", status)
			End If
		End Sub

		Public Overrides Sub dgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)
			Dim tau As INDArray = Nd4j.create(DataType.DOUBLE, N)

			Dim status As Integer = LAPACKE_dgeqrf(getColumnOrder(A), M, N, CType(A.data().addressPointer(), DoublePointer), getLda(A), CType(tau.data().addressPointer(), DoublePointer))
			If status <> 0 Then
				Throw New BlasException("Failed to execute dgeqrf", status)
			End If

			' Copy R ( upper part of Q ) into result
			If R IsNot Nothing Then
				R.assign(A.get(NDArrayIndex.interval(0, A.columns()), NDArrayIndex.all()))
				Dim ix(1) As INDArrayIndex

				Dim i As Integer=1
				Do While i<Math.Min(A.rows(), A.columns())
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(0, i)
					R.put(ix, 0)
					i += 1
				Loop
			End If

			status = LAPACKE_dorgqr(getColumnOrder(A), M, N, N, CType(A.data().addressPointer(), DoublePointer), getLda(A), CType(tau.data().addressPointer(), DoublePointer))
			If status <> 0 Then
				Throw New BlasException("Failed to execute dorgqr", status)
			End If
		End Sub


	'=========================    
	' CHOLESKY DECOMP
		Public Overrides Sub spotrf(ByVal uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)
			Dim status As Integer = LAPACKE_spotrf(getColumnOrder(A), uplo, N, CType(A.data().addressPointer(), FloatPointer), getLda(A))
			If status <> 0 Then
				Throw New BlasException("Failed to execute spotrf", status)
			End If
			If uplo = AscW("U"c) Then
				Dim ix(1) As INDArrayIndex
				Dim i As Integer=1
				Do While i<Math.Min(A.rows(), A.columns())
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(0, i)
					A.put(ix, 0)
					i += 1
				Loop
			Else
				Dim ix(1) As INDArrayIndex
				Dim i As Integer=0
				Do While i<Math.Min(A.rows(), A.columns()-1)
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(i+1, A.columns())
					A.put(ix, 0)
					i += 1
				Loop
			End If
		End Sub

		Public Overrides Sub dpotrf(ByVal uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)
			Dim status As Integer = LAPACKE_dpotrf(getColumnOrder(A), uplo, N, CType(A.data().addressPointer(), DoublePointer), getLda(A))
			If status <> 0 Then
				Throw New BlasException("Failed to execute dpotrf", status)
			End If
			If uplo = AscW("U"c) Then
				Dim ix(1) As INDArrayIndex
				Dim i As Integer=1
				Do While i<Math.Min(A.rows(), A.columns())
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(0, i)
					A.put(ix, 0)
					i += 1
				Loop
			Else
				Dim ix(1) As INDArrayIndex
				Dim i As Integer=0
				Do While i<Math.Min(A.rows(), A.columns()-1)
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(i+1, A.columns())
					A.put(ix, 0)
					i += 1
				Loop
			End If
		End Sub



	'=========================    
	' U S V' DECOMP  (aka SVD)
		Public Overrides Sub sgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)
			Dim superb As INDArray = Nd4j.create(DataType.FLOAT,If(M < N, M, N))
			Dim status As Integer = LAPACKE_sgesvd(getColumnOrder(A), jobu, jobvt, M, N, CType(A.data().addressPointer(), FloatPointer), getLda(A), CType(S.data().addressPointer(), FloatPointer),If(U Is Nothing, Nothing, CType(U.data().addressPointer(), FloatPointer)),If(U Is Nothing, 1, getLda(U)),If(VT Is Nothing, Nothing, CType(VT.data().addressPointer(), FloatPointer)),If(VT Is Nothing, 1, getLda(VT)), CType(superb.data().addressPointer(), FloatPointer))
			If status <> 0 Then
				Throw New BlasException("Failed to execute sgesvd", status)
			End If
		End Sub

		Public Overrides Sub dgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)
			Dim superb As INDArray = Nd4j.create(DataType.DOUBLE,If(M < N, M, N))
			Dim status As Integer = LAPACKE_dgesvd(getColumnOrder(A), jobu, jobvt, M, N, CType(A.data().addressPointer(), DoublePointer), getLda(A), CType(S.data().addressPointer(), DoublePointer),If(U Is Nothing, Nothing, CType(U.data().addressPointer(), DoublePointer)),If(U Is Nothing, 1, getLda(U)),If(VT Is Nothing, Nothing, CType(VT.data().addressPointer(), DoublePointer)),If(VT Is Nothing, 1, getLda(VT)), CType(superb.data().addressPointer(), DoublePointer))
			If status <> 0 Then
				Throw New BlasException("Failed to execute dgesvd", status)
			End If
		End Sub


	'=========================    
	' syev EigenValue/Vectors
	'
		Public Overrides Function ssyev(ByVal jobz As Char, ByVal uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer
		Dim fp As New FloatPointer(1)
		Dim status As Integer = LAPACKE_ssyev_work(getColumnOrder(A), AscW(jobz), AscW(uplo), N, CType(A.data().addressPointer(), FloatPointer), getLda(A), CType(R.data().addressPointer(), FloatPointer), fp, -1)
		If status = 0 Then
			Dim lwork As Integer = CInt(Math.Truncate(fp.get()))
			Dim work As INDArray = Nd4j.createArrayFromShapeBuffer(Nd4j.DataBufferFactory.createFloat(lwork), Nd4j.ShapeInfoProvider.createShapeInformation(New Long() {lwork}, A.dataType()).First)

			status = LAPACKE_ssyev(getColumnOrder(A), AscW(jobz), AscW(uplo), N, CType(A.data().addressPointer(), FloatPointer), getLda(A), CType(work.data().addressPointer(), FloatPointer))
			If status = 0 Then
				R.assign(work.get(NDArrayIndex.interval(0,N)))
			End If
		End If
		Return status
		End Function


		Public Overrides Function dsyev(ByVal jobz As Char, ByVal uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer

		Dim dp As New DoublePointer(1)
		Dim status As Integer = LAPACKE_dsyev_work(getColumnOrder(A), AscW(jobz), AscW(uplo), N, CType(A.data().addressPointer(), DoublePointer), getLda(A), CType(R.data().addressPointer(), DoublePointer), dp, -1)
		If status = 0 Then
			Dim lwork As Integer = CInt(Math.Truncate(dp.get()))
			Dim work As INDArray = Nd4j.createArrayFromShapeBuffer(Nd4j.DataBufferFactory.createDouble(lwork), Nd4j.ShapeInfoProvider.createShapeInformation(New Long() {lwork}, A.dataType()).First)

			status = LAPACKE_dsyev(getColumnOrder(A), AscW(jobz), AscW(uplo), N, CType(A.data().addressPointer(), DoublePointer), getLda(A), CType(work.data().addressPointer(), DoublePointer))

			If status = 0 Then
				R.assign(work.get(NDArrayIndex.interval(0,N)))
			End If
		End If
		Return status
		End Function




		''' <summary>
		''' Generate inverse given LU decomp
		''' </summary>
		''' <param name="N"> </param>
		''' <param name="A"> </param>
		''' <param name="lda"> </param>
		''' <param name="IPIV"> </param>
		''' <param name="WORK"> </param>
		''' <param name="lwork"> </param>
		''' <param name="INFO"> </param>
		Public Overrides Sub getri(ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal WORK As INDArray, ByVal lwork As Integer, ByVal INFO As Integer)

		End Sub
	End Class

End Namespace