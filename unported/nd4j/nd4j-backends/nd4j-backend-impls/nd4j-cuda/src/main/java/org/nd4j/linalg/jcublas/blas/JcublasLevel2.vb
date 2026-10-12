Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports cublasHandle_t = org.nd4j.jita.allocator.pointers.cuda.cublasHandle_t
Imports BaseLevel2 = org.nd4j.linalg.api.blas.impl.BaseLevel2
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutionerUtil = org.nd4j.linalg.api.ops.executioner.OpExecutionerUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CublasPointer = org.nd4j.linalg.jcublas.CublasPointer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports Nd4jBlas = org.nd4j.nativeblas.Nd4jBlas
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports org.bytedeco.cuda.cudart
Imports org.bytedeco.cuda.cublas
Imports org.bytedeco.cuda.global.cublas
import static org.nd4j.linalg.jcublas.blas.CudaBlas.convertTranspose

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

Namespace org.nd4j.linalg.jcublas.blas

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class JcublasLevel2
		Inherits BaseLevel2

		Private allocator As Allocator = AtomicAllocator.Instance
		Private nd4jBlas As Nd4jBlas = DirectCast(Nd4j.factory().blas(), Nd4jBlas)
		Private nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(JcublasLevel2))

		Protected Friend Overrides Sub sgemv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Y, A, X)

			Dim cAPointer As New CublasPointer(A, ctx)
			Dim cBPointer As New CublasPointer(X, ctx)
			Dim cCPointer As New CublasPointer(Y, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasSgemv_v2(New cublasContext(handle), convertTranspose(TransA), M, N, New FloatPointer(alpha), CType(cAPointer.DevicePointer, FloatPointer), lda, CType(cBPointer.DevicePointer, FloatPointer), incX, New FloatPointer(beta), CType(cCPointer.DevicePointer, FloatPointer), incY)
			End SyncLock

			allocator.registerAction(ctx, Y, A, X)
			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub sgbmv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub strmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub stbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub stpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub strsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub stbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub stpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dgemv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Y, A, X)

			Dim cAPointer As New CublasPointer(A, ctx)
			Dim cBPointer As New CublasPointer(X, ctx)
			Dim cCPointer As New CublasPointer(Y, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDgemv_v2(New cublasContext(handle), convertTranspose(TransA), M, N, New DoublePointer(alpha), CType(cAPointer.DevicePointer, DoublePointer), lda, CType(cBPointer.DevicePointer, DoublePointer), incX, New DoublePointer(beta), CType(cCPointer.DevicePointer, DoublePointer), incY)
			End SyncLock

			allocator.registerAction(ctx, Y, A, X)
			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub dgbmv(ByVal order As Char, ByVal TransA As Char, ByVal M As Integer, ByVal N As Integer, ByVal KL As Integer, ByVal KU As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dtrmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dtbmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dtpmv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dtrsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub dtbsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal K As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub dtpsv(ByVal order As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal N As Integer, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub ssymv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub ssbmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub sspmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Single, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub sger(ByVal order As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub ssyr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal A As INDArray, ByVal lda As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub sspr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Ap As INDArray)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub ssyr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub sspr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dsymv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dsbmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dspmv(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal Ap As INDArray, ByVal X As INDArray, ByVal incX As Integer, ByVal beta As Double, ByVal Y As INDArray, ByVal incY As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dger(ByVal order As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dsyr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal A As INDArray, ByVal lda As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dspr(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Ap As INDArray)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dsyr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray, ByVal lda As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub dspr2(ByVal order As Char, ByVal Uplo As Char, ByVal N As Integer, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal A As INDArray)
			Throw New System.NotSupportedException()

		End Sub
	End Class

End Namespace