Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports ShortPointer = org.bytedeco.javacpp.ShortPointer
Imports HalfIndexer = org.bytedeco.javacpp.indexer.HalfIndexer
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports cublasHandle_t = org.nd4j.jita.allocator.pointers.cuda.cublasHandle_t
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports BaseLevel3 = org.nd4j.linalg.api.blas.impl.BaseLevel3
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutionerUtil = org.nd4j.linalg.api.ops.executioner.OpExecutionerUtil
Imports DataTypeValidation = org.nd4j.linalg.factory.DataTypeValidation
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
Imports org.bytedeco.cuda.global.cudart
Imports org.bytedeco.cuda.global.cublas
Imports org.nd4j.linalg.jcublas.blas.CudaBlas

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
	''' Level 3 implementation of matrix matrix operations
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JcublasLevel3 extends org.nd4j.linalg.api.blas.impl.BaseLevel3
	Public Class JcublasLevel3
		Inherits BaseLevel3

		Private allocator As Allocator = AtomicAllocator.Instance
		Private nd4jBlas As Nd4jBlas = DirectCast(Nd4j.factory().blas(), Nd4jBlas)
		Private nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(JcublasLevel3))

		Protected Friend Overrides Sub hgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)
			'A = Shape.toOffsetZero(A);
			'B = Shape.toOffsetZero(B);

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(C, A, B)

			Dim cAPointer As New CublasPointer(A, ctx)
			Dim cBPointer As New CublasPointer(B, ctx)
			Dim cCPointer As New CublasPointer(C, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				Dim arch As Integer = CudaEnvironment.Instance.CurrentDeviceArchitecture

				If (CUDA_VERSION >= 8000 AndAlso (arch = 53 OrElse arch = 60 OrElse arch >= 70)) OrElse (CUDA_VERSION >= 8000 AndAlso CUDA_VERSION < 9020) Then
					' on these selected archs we run with cublasHgemm
					Dim alphaHalf As New __half()
					Dim betaHalf As New __half()
					Call (New ShortPointer(alphaHalf)).put(CShort(Math.Truncate(HalfIndexer.fromFloat(alpha))))
					Call (New ShortPointer(betaHalf)).put(CShort(Math.Truncate(HalfIndexer.fromFloat(beta))))

					cublasHgemm(New cublasContext(handle), convertTranspose(AscW(TransA)), convertTranspose(AscW(TransB)), M, N, K, alphaHalf, New __half(cAPointer.DevicePointer), lda, New __half(cBPointer.DevicePointer), ldb, betaHalf, New __half(cCPointer.DevicePointer), ldc)
				Else
					' CUDA_R_16F == 2 for CUDA 8
					' CUBLAS_DATA_HALF == 2 for CUDA 7.5
					cublasSgemmEx(New cublasContext(handle), convertTranspose(AscW(TransA)), convertTranspose(AscW(TransB)), M, N, K, New FloatPointer(alpha), CType(cAPointer.DevicePointer, ShortPointer), 2, lda, CType(cBPointer.DevicePointer, ShortPointer), 2, ldb, New FloatPointer(beta), CType(cCPointer.DevicePointer, ShortPointer), 2, ldc)


				End If

				ctx.getOldStream().synchronize()
			End SyncLock

			allocator.registerAction(ctx, C, A, B)
			OpExecutionerUtil.checkForAny(C)
		End Sub


		Protected Friend Overrides Sub sgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)
	'        
	'        val ctx = AtomicAllocator.getInstance().getDeviceContext();
	'        val handle = ctx.getCublasHandle();
	'        synchronized (handle) {
	'            Nd4j.exec(new Mmul(A, B, C, MMulTranspose.builder().transposeA(false).transposeB(false).build()));
	'        }
	'        

			Nd4j.Executioner.push()

			Dim ctx As val = allocator.FlowController.prepareAction(C, A, B)

			'log.info("Synchronizing CUDA stream");
			ctx.getOldStream().synchronize()

			Dim cAPointer As val = New CublasPointer(A, ctx)
			Dim cBPointer As val = New CublasPointer(B, ctx)
			Dim cCPointer As val = New CublasPointer(C, ctx)

			Dim handle As val = ctx.getCublasHandle()
			SyncLock handle
				'log.info("Handle: {}; Stream: {}", handle.address(), ctx.getCublasStream().address());
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.getCublasStream()))

				cublasSgemm_v2(New cublasContext(handle), convertTranspose(AscW(TransA)), convertTranspose(AscW(TransB)), M, N, K, New FloatPointer(alpha), CType(cAPointer.getDevicePointer(), FloatPointer), lda, CType(cBPointer.getDevicePointer(), FloatPointer), ldb, New FloatPointer(beta), CType(cCPointer.getDevicePointer(), FloatPointer), ldc)

				ctx.getOldStream().synchronize()
			End SyncLock

			allocator.registerAction(ctx, C, A, B)

			OpExecutionerUtil.checkForAny(C)
		End Sub

		Protected Friend Overrides Sub ssymm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(C, A, B)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim bPointer As New CublasPointer(B, ctx)
			Dim cPointer As New CublasPointer(C, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasSsymm_v2(New cublasContext(handle), convertSideMode(AscW(Side)), convertUplo(AscW(Uplo)), M, N, New FloatPointer(alpha), CType(aPointer.DevicePointer, FloatPointer), lda, CType(bPointer.DevicePointer, FloatPointer), ldb, New FloatPointer(beta), CType(cPointer.DevicePointer, FloatPointer), ldc)
			End SyncLock

			allocator.registerAction(ctx, C, A, B)
			OpExecutionerUtil.checkForAny(C)
		End Sub

		Protected Friend Overrides Sub ssyrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(C, A)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim cPointer As New CublasPointer(C, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasSsyrk_v2(New cublasContext(handle), convertUplo(AscW(Uplo)), convertTranspose(AscW(Trans)), N, K, New FloatPointer(alpha), CType(aPointer.DevicePointer, FloatPointer), lda, New FloatPointer(beta), CType(cPointer.DevicePointer, FloatPointer), ldc)
			End SyncLock

			allocator.registerAction(ctx, C, A)
			OpExecutionerUtil.checkForAny(C)
		End Sub

		Protected Friend Overrides Sub ssyr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Single, ByVal C As INDArray, ByVal ldc As Integer)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub strmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)
			Throw New System.NotSupportedException()
		End Sub

		Protected Friend Overrides Sub strsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Single, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(B, A)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim bPointer As New CublasPointer(B, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasStrsm_v2(New cublasContext(handle), convertSideMode(AscW(Side)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), M, N, New FloatPointer(alpha), CType(aPointer.DevicePointer, FloatPointer), lda, CType(bPointer.DevicePointer, FloatPointer), ldb)
			End SyncLock

			allocator.registerAction(ctx, B, A)
			OpExecutionerUtil.checkForAny(B)
		End Sub

		Protected Friend Overrides Sub dgemm(ByVal Order As Char, ByVal TransA As Char, ByVal TransB As Char, ByVal M As Integer, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)
			'A = Shape.toOffsetZero(A);
			'B = Shape.toOffsetZero(B);

			Nd4j.Executioner.push()

			Dim ctx As val = allocator.FlowController.prepareAction(C, A, B)

			DataTypeValidation.assertDouble(A, B, C)

			Dim cAPointer As val = New CublasPointer(A, ctx)
			Dim cBPointer As val = New CublasPointer(B, ctx)
			Dim cCPointer As val = New CublasPointer(C, ctx)

			Dim handle As val = ctx.getCublasHandle()
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.getCublasStream()))

				cublasDgemm_v2(New cublasContext(handle), convertTranspose(AscW(TransA)), convertTranspose(AscW(TransB)), M, N, K, New DoublePointer(alpha), CType(cAPointer.getDevicePointer(), DoublePointer), lda, CType(cBPointer.getDevicePointer(), DoublePointer), ldb, New DoublePointer(beta), CType(cCPointer.getDevicePointer(), DoublePointer), ldc)

				ctx.getOldStream().synchronize()
			End SyncLock

			allocator.registerAction(ctx, C, A, B)
			OpExecutionerUtil.checkForAny(C)
		End Sub

		Protected Friend Overrides Sub dsymm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)
			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(C, A, B)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim bPointer As New CublasPointer(B, ctx)
			Dim cPointer As New CublasPointer(C, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDsymm_v2(New cublasContext(handle), convertSideMode(AscW(Side)), convertUplo(AscW(Uplo)), M, N, New DoublePointer(alpha), CType(aPointer.DevicePointer, DoublePointer), lda, CType(bPointer.DevicePointer, DoublePointer), ldb, New DoublePointer(beta), CType(cPointer.DevicePointer, DoublePointer), ldc)
			End SyncLock

			allocator.registerAction(ctx, C, A, B)
			OpExecutionerUtil.checkForAny(C)
		End Sub

		Protected Friend Overrides Sub dsyrk(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(C, A)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim cPointer As New CublasPointer(C, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDsyrk_v2(New cublasContext(handle), convertUplo(AscW(Uplo)), Trans, N, K, New DoublePointer(alpha), CType(aPointer.DevicePointer, DoublePointer), lda, New DoublePointer(beta), CType(cPointer.DevicePointer, DoublePointer), ldc)
			End SyncLock

			allocator.registerAction(ctx, C, A)
			OpExecutionerUtil.checkForAny(C)
		End Sub

		Protected Friend Overrides Sub dsyr2k(ByVal Order As Char, ByVal Uplo As Char, ByVal Trans As Char, ByVal N As Integer, ByVal K As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer, ByVal beta As Double, ByVal C As INDArray, ByVal ldc As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(C, A, B)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim bPointer As New CublasPointer(B, ctx)
			Dim cPointer As New CublasPointer(C, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDsyr2k_v2(New cublasContext(handle), convertUplo(AscW(Uplo)), Trans, N, K, New DoublePointer(alpha), CType(aPointer.DevicePointer, DoublePointer), lda, CType(bPointer.DevicePointer, DoublePointer), ldb, New DoublePointer(beta), CType(cPointer.DevicePointer, DoublePointer), ldc)
			End SyncLock

			allocator.registerAction(ctx, C, A, B)
			OpExecutionerUtil.checkForAny(C)
		End Sub

		Protected Friend Overrides Sub dtrmm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(B, A)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim bPointer As New CublasPointer(B, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDtrmm_v2(New cublasContext(handle), convertSideMode(AscW(Side)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), M, N, New DoublePointer(alpha), CType(aPointer.DevicePointer, DoublePointer), lda, CType(bPointer.DevicePointer, DoublePointer), ldb, CType(bPointer.DevicePointer, DoublePointer), ldb)
			End SyncLock

			allocator.registerAction(ctx, B, A)
			OpExecutionerUtil.checkForAny(B)
		End Sub

		Protected Friend Overrides Sub dtrsm(ByVal Order As Char, ByVal Side As Char, ByVal Uplo As Char, ByVal TransA As Char, ByVal Diag As Char, ByVal M As Integer, ByVal N As Integer, ByVal alpha As Double, ByVal A As INDArray, ByVal lda As Integer, ByVal B As INDArray, ByVal ldb As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(B, A)

			Dim aPointer As New CublasPointer(A, ctx)
			Dim bPointer As New CublasPointer(B, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDtrsm_v2(New cublasContext(handle), convertSideMode(AscW(Side)), convertUplo(AscW(Uplo)), convertTranspose(AscW(TransA)), convertDiag(AscW(Diag)), M, N, New DoublePointer(alpha), CType(aPointer.DevicePointer, DoublePointer), lda, CType(bPointer.DevicePointer, DoublePointer), ldb)
			End SyncLock

			allocator.registerAction(ctx, B, A)
			OpExecutionerUtil.checkForAny(B)
		End Sub
	End Class

End Namespace