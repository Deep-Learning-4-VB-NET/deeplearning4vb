Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports CudaPointer = org.nd4j.jita.allocator.pointers.CudaPointer
Imports cusolverDnHandle_t = org.nd4j.jita.allocator.pointers.cuda.cusolverDnHandle_t
Imports BlasException = org.nd4j.linalg.api.blas.BlasException
Imports BaseLapack = org.nd4j.linalg.api.blas.impl.BaseLapack
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports CublasPointer = org.nd4j.linalg.jcublas.CublasPointer
Imports BaseCudaDataBuffer = org.nd4j.linalg.jcublas.buffer.BaseCudaDataBuffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports org.bytedeco.cuda.cudart
Imports org.bytedeco.cuda.cusolver
Imports org.bytedeco.cuda.global.cublas
Imports org.bytedeco.cuda.global.cusolver

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
	''' JCublas lapack
	''' 
	''' @author Adam Gibson
	''' @author Richard Corbishley (signed)
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class JcublasLapack extends org.nd4j.linalg.api.blas.impl.BaseLapack
	Public Class JcublasLapack
		Inherits BaseLapack

		Private nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Private allocator As Allocator = AtomicAllocator.Instance

		Public Overrides Sub sgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)
'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A
			If Nd4j.dataType() <> DataType.FLOAT Then
				log.warn("FLOAT getrf called in DOUBLE environment")
			End If

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = allocator.DeviceContext

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New BlasException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As New CublasPointer(a_Conflict, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnSgetrf_bufferSize(solverDn, M, N, CType(xAPointer.DevicePointer, FloatPointer), M, CType(worksizeBuffer.addressPointer(), IntPointer))

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgetrf_bufferSize failed", stat)
				End If

				Dim worksize As Integer = worksizeBuffer.getInt(0)
				' Now allocate memory for the workspace, the permutation matrix and a return code
				Dim workspace As Pointer = New Workspace(worksize * Nd4j.sizeOfDataType())

				' Do the actual LU decomp
				stat = cusolverDnSgetrf(solverDn, M, N, CType(xAPointer.DevicePointer, FloatPointer), M, (New CudaPointer(workspace)).asFloatPointer(), (New CudaPointer(allocator.getPointer(IPIV, ctx))).asIntPointer(), (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())

				' we do sync to make sure getrf is finished
				'ctx.syncOldStream();

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgetrf failed", stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, a_Conflict)
			allocator.registerAction(ctx, INFO)
			allocator.registerAction(ctx, IPIV)

			If a_Conflict IsNot A Then
				A.assign(a_Conflict)
			End If
		End Sub


		Public Overrides Sub dgetrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal IPIV As INDArray, ByVal INFO As INDArray)
'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A

			If Nd4j.dataType() <> DataType.DOUBLE Then
				log.warn("FLOAT getrf called in FLOAT environment")
			End If

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = allocator.DeviceContext

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New BlasException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As val = New CublasPointer(a_Conflict, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnDgetrf_bufferSize(solverDn, M, N, CType(xAPointer.getDevicePointer(), DoublePointer), M, CType(worksizeBuffer.addressPointer(), IntPointer))

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnDgetrf_bufferSize failed", stat)
				End If
				Dim worksize As Integer = worksizeBuffer.getInt(0)

				' Now allocate memory for the workspace, the permutation matrix and a return code
				Dim workspace As val = New Workspace(worksize * Nd4j.sizeOfDataType())

				' Do the actual LU decomp
				stat = cusolverDnDgetrf(solverDn, M, N, CType(xAPointer.getDevicePointer(), DoublePointer), M, (New CudaPointer(workspace)).asDoublePointer(), (New CudaPointer(allocator.getPointer(IPIV, ctx))).asIntPointer(), (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgetrf failed", stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, a_Conflict)
			allocator.registerAction(ctx, INFO)
			allocator.registerAction(ctx, IPIV)

			If a_Conflict IsNot A Then
				A.assign(a_Conflict)
			End If
		End Sub


		'=========================
		' Q R DECOMP
		Public Overrides Sub sgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)
'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A
'JAVA TO VB CONVERTER NOTE: The variable r was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim r_Conflict As INDArray = R

			If Nd4j.dataType() <> DataType.FLOAT Then
				log.warn("FLOAT getrf called in DOUBLE environment")
			End If

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If
			If R IsNot Nothing AndAlso R.ordering() = "c"c Then
				r_Conflict = R.dup("f"c)
			End If

			Dim tau As INDArray = Nd4j.createArrayFromShapeBuffer(Nd4j.DataBufferFactory.createFloat(N), Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){1, N}, A.dataType()).First)

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = allocator.DeviceContext

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New System.InvalidOperationException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As New CublasPointer(a_Conflict, ctx)
				Dim xTauPointer As New CublasPointer(tau, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnSgeqrf_bufferSize(solverDn, M, N, CType(xAPointer.DevicePointer, FloatPointer), M, CType(worksizeBuffer.addressPointer(), IntPointer))


				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgeqrf_bufferSize failed", stat)
				End If
				Dim worksize As Integer = worksizeBuffer.getInt(0)
				' Now allocate memory for the workspace, the permutation matrix and a return code
				Dim workspace As Pointer = New Workspace(worksize * Nd4j.sizeOfDataType())

				' Do the actual QR decomp
				stat = cusolverDnSgeqrf(solverDn, M, N, CType(xAPointer.DevicePointer, FloatPointer), M, CType(xTauPointer.DevicePointer, FloatPointer), (New CudaPointer(workspace)).asFloatPointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())
				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgeqrf failed", stat)
				End If

				allocator.registerAction(ctx, a_Conflict)
				'allocator.registerAction(ctx, tau);
				allocator.registerAction(ctx, INFO)
				If INFO.getInt(0) <> 0 Then
					Throw New BlasException("cusolverDnSgeqrf failed on INFO", INFO.getInt(0))
				End If

				' Copy R ( upper part of Q ) into result
				If r_Conflict IsNot Nothing Then
					r_Conflict.assign(a_Conflict.get(NDArrayIndex.interval(0, a_Conflict.columns()), NDArrayIndex.all()))

					Dim ix(1) As INDArrayIndex
					Dim i As Integer = 1
					Do While i < Math.Min(a_Conflict.rows(), a_Conflict.columns())
						ix(0) = NDArrayIndex.point(i)
						ix(1) = NDArrayIndex.interval(0, i)
						r_Conflict.put(ix, 0)
						i += 1
					Loop
				End If

				stat = cusolverDnSorgqr_bufferSize(solverDn, M, N, N, CType(xAPointer.DevicePointer, FloatPointer), M, CType(xTauPointer.DevicePointer, FloatPointer), CType(worksizeBuffer.addressPointer(), IntPointer))
				worksize = worksizeBuffer.getInt(0)
				workspace = New Workspace(worksize * Nd4j.sizeOfDataType())

				stat = cusolverDnSorgqr(solverDn, M, N, N, CType(xAPointer.DevicePointer, FloatPointer), M, CType(xTauPointer.DevicePointer, FloatPointer), (New CudaPointer(workspace)).asFloatPointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())
				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSorgqr failed", stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, a_Conflict)
			allocator.registerAction(ctx, INFO)
			'    allocator.registerAction(ctx, tau);

			If a_Conflict IsNot A Then
				A.assign(a_Conflict)
			End If
			If r_Conflict IsNot Nothing AndAlso r_Conflict IsNot R Then
				R.assign(r_Conflict)
			End If

			log.debug("A: {}", A)
			If R IsNot Nothing Then
				log.debug("R: {}", R)
			End If
		End Sub

		Public Overrides Sub dgeqrf(ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray, ByVal INFO As INDArray)
'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A
'JAVA TO VB CONVERTER NOTE: The variable r was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim r_Conflict As INDArray = R

			If Nd4j.dataType() <> DataType.DOUBLE Then
				log.warn("DOUBLE getrf called in FLOAT environment")
			End If

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If
			If R IsNot Nothing AndAlso R.ordering() = "c"c Then
				r_Conflict = R.dup("f"c)
			End If

			Dim tau As INDArray = Nd4j.createArrayFromShapeBuffer(Nd4j.DataBufferFactory.createDouble(N), Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){1, N}, A.dataType()))

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = CType(allocator.DeviceContext, CudaContext)

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New BlasException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As New CublasPointer(a_Conflict, ctx)
				Dim xTauPointer As New CublasPointer(tau, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnDgeqrf_bufferSize(solverDn, M, N, CType(xAPointer.DevicePointer, DoublePointer), M, CType(worksizeBuffer.addressPointer(), IntPointer))

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnDgeqrf_bufferSize failed", stat)
				End If
				Dim worksize As Integer = worksizeBuffer.getInt(0)
				' Now allocate memory for the workspace, the permutation matrix and a return code
				Dim workspace As Pointer = New Workspace(worksize * Nd4j.sizeOfDataType())

				' Do the actual QR decomp
				stat = cusolverDnDgeqrf(solverDn, M, N, CType(xAPointer.DevicePointer, DoublePointer), M, CType(xTauPointer.DevicePointer, DoublePointer), (New CudaPointer(workspace)).asDoublePointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())
				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnDgeqrf failed", stat)
				End If

				allocator.registerAction(ctx, a_Conflict)
				allocator.registerAction(ctx, tau)
				allocator.registerAction(ctx, INFO)
				If INFO.getInt(0) <> 0 Then
					Throw New BlasException("cusolverDnDgeqrf failed with info", INFO.getInt(0))
				End If

				' Copy R ( upper part of Q ) into result
				If r_Conflict IsNot Nothing Then
					r_Conflict.assign(a_Conflict.get(NDArrayIndex.interval(0, a_Conflict.columns()), NDArrayIndex.all()))

					Dim ix(1) As INDArrayIndex
					Dim i As Integer = 1
					Do While i < Math.Min(a_Conflict.rows(), a_Conflict.columns())
						ix(0) = NDArrayIndex.point(i)
						ix(1) = NDArrayIndex.interval(0, i)
						r_Conflict.put(ix, 0)
						i += 1
					Loop
				End If
				stat = cusolverDnDorgqr_bufferSize(solverDn, M, N, N, CType(xAPointer.DevicePointer, DoublePointer), M, CType(xTauPointer.DevicePointer, DoublePointer), CType(worksizeBuffer.addressPointer(), IntPointer))
				worksize = worksizeBuffer.getInt(0)
				workspace = New Workspace(worksize * Nd4j.sizeOfDataType())

				stat = cusolverDnDorgqr(solverDn, M, N, N, CType(xAPointer.DevicePointer, DoublePointer), M, CType(xTauPointer.DevicePointer, DoublePointer), (New CudaPointer(workspace)).asDoublePointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())
				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnDorgqr failed", stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, a_Conflict)
			allocator.registerAction(ctx, INFO)

			If a_Conflict IsNot A Then
				A.assign(a_Conflict)
			End If
			If r_Conflict IsNot Nothing AndAlso r_Conflict IsNot R Then
				R.assign(r_Conflict)
			End If

			log.debug("A: {}", A)
			If R IsNot Nothing Then
				log.debug("R: {}", R)
			End If
		End Sub

		'=========================
	' CHOLESKY DECOMP
		Public Overrides Sub spotrf(ByVal _uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)
'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A

			Dim uplo As Integer = If(_uplo = AscW("L"c), CUBLAS_FILL_MODE_LOWER, CUBLAS_FILL_MODE_UPPER)

			If A.dataType() <> DataType.FLOAT Then
				log.warn("FLOAT potrf called for " & A.dataType())
			End If

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = CType(allocator.DeviceContext, CudaContext)

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New BlasException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As New CublasPointer(a_Conflict, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnSpotrf_bufferSize(solverDn, uplo, N, CType(xAPointer.DevicePointer, FloatPointer), N, CType(worksizeBuffer.addressPointer(), IntPointer))

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSpotrf_bufferSize failed", stat)
				End If

				Dim worksize As Integer = worksizeBuffer.getInt(0)
				' Now allocate memory for the workspace, the permutation matrix and a return code
				Dim workspace As Pointer = New Workspace(worksize * Nd4j.sizeOfDataType())

				' Do the actual decomp
				stat = cusolverDnSpotrf(solverDn, uplo, N, CType(xAPointer.DevicePointer, FloatPointer), N, (New CudaPointer(workspace)).asFloatPointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSpotrf failed", stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, a_Conflict)
			allocator.registerAction(ctx, INFO)

			If a_Conflict IsNot A Then
				A.assign(a_Conflict)
			End If

			If uplo = CUBLAS_FILL_MODE_UPPER Then
				A.assign(A.transpose())
				Dim ix(1) As INDArrayIndex
				Dim i As Integer = 1
				Do While i < Math.Min(A.rows(), A.columns())
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(0, i)
					A.put(ix, 0)
					i += 1
				Loop
			Else
				Dim ix(1) As INDArrayIndex
				Dim i As Integer = 0
				Do While i < Math.Min(A.rows(), A.columns() - 1)
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(i + 1, A.columns())
					A.put(ix, 0)
					i += 1
				Loop
			End If

			log.debug("A: {}", A)
		End Sub

		Public Overrides Sub dpotrf(ByVal _uplo As SByte, ByVal N As Integer, ByVal A As INDArray, ByVal INFO As INDArray)
'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A

			Dim uplo As Integer = If(_uplo = AscW("L"c), CUBLAS_FILL_MODE_LOWER, CUBLAS_FILL_MODE_UPPER)

			If A.dataType() <> DataType.DOUBLE Then
				log.warn("DOUBLE potrf called for " & A.dataType())
			End If

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = allocator.DeviceContext

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(solverDn, New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New BlasException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As New CublasPointer(a_Conflict, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnDpotrf_bufferSize(solverDn, uplo, N, CType(xAPointer.DevicePointer, DoublePointer), N, CType(worksizeBuffer.addressPointer(), IntPointer))

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnDpotrf_bufferSize failed", stat)
				End If

				Dim worksize As Integer = worksizeBuffer.getInt(0)
				' Now allocate memory for the workspace, the permutation matrix and a return code
				Dim workspace As Pointer = New Workspace(worksize * Nd4j.sizeOfDataType(DataType.DOUBLE))

				' Do the actual decomp
				stat = cusolverDnDpotrf(solverDn, uplo, N, CType(xAPointer.DevicePointer, DoublePointer), N, (New CudaPointer(workspace)).asDoublePointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnDpotrf failed", stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, a_Conflict)
			allocator.registerAction(ctx, INFO)

			If a_Conflict IsNot A Then
				A.assign(a_Conflict)
			End If

			If uplo = CUBLAS_FILL_MODE_UPPER Then
				A.assign(A.transpose())
				Dim ix(1) As INDArrayIndex
				Dim i As Integer = 1
				Do While i < Math.Min(A.rows(), A.columns())
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(0, i)
					A.put(ix, 0)
					i += 1
				Loop
			Else
				Dim ix(1) As INDArrayIndex
				Dim i As Integer = 0
				Do While i < Math.Min(A.rows(), A.columns() - 1)
					ix(0) = NDArrayIndex.point(i)
					ix(1) = NDArrayIndex.interval(i + 1, A.columns())
					A.put(ix, 0)
					i += 1
				Loop
			End If

			log.debug("A: {}", A)
		End Sub


		''' <summary>
		''' Generate inverse ggiven LU decomp
		''' </summary>
		''' <param name="N"> </param>
		''' <param name="A"> </param>
		''' <param name="IPIV"> </param>
		''' <param name="WORK"> </param>
		''' <param name="lwork"> </param>
		''' <param name="INFO"> </param>
		Public Overrides Sub getri(ByVal N As Integer, ByVal A As INDArray, ByVal lda As Integer, ByVal IPIV() As Integer, ByVal WORK As INDArray, ByVal lwork As Integer, ByVal INFO As Integer)

		End Sub


		Public Overrides Sub sgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)

			If Nd4j.dataType() <> DataType.FLOAT Then
				log.warn("FLOAT gesvd called in DOUBLE environment")
			End If

'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A
'JAVA TO VB CONVERTER NOTE: The variable u was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim u_Conflict As INDArray = U
'JAVA TO VB CONVERTER NOTE: The variable vt was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim vt_Conflict As INDArray = VT

			' we should transpose & adjust outputs if M<N
			' cuda has a limitation, but it's OK we know
			' 	A = U S V'
			' transpose multiply rules give us ...
			' 	A' = V S' U'
			Dim hadToTransposeA As Boolean = False
			If M < N Then
				hadToTransposeA = True

				Dim tmp1 As Integer = N
				N = M
				M = tmp1
				Dim tmp2 As SByte = jobu
				jobu = jobvt
				jobvt = tmp2

				a_Conflict = A.transpose().dup("f"c)
				u_Conflict = If(VT Is Nothing, Nothing, VT.transpose().dup("f"c))
				vt_Conflict = If(U Is Nothing, Nothing, U.transpose().dup("f"c))
			Else
				' cuda requires column ordering - we'll register a warning in case
				If A.ordering() = "c"c Then
					a_Conflict = A.dup("f"c)
				End If

				If U IsNot Nothing AndAlso U.ordering() = "c"c Then
					u_Conflict = U.dup("f"c)
				End If

				If VT IsNot Nothing AndAlso VT.ordering() = "c"c Then
					vt_Conflict = VT.dup("f"c)
				End If
			End If

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = CType(allocator.DeviceContext, CudaContext)

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New BlasException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As New CublasPointer(a_Conflict, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnSgesvd_bufferSize(solverDn, M, N, CType(worksizeBuffer.addressPointer(), IntPointer))
				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgesvd_bufferSize failed", stat)
				End If
				Dim worksize As Integer = worksizeBuffer.getInt(0)

				Dim workspace As Pointer = New Workspace(worksize * Nd4j.sizeOfDataType())
				Dim rwork As DataBuffer = Nd4j.DataBufferFactory.createFloat((If(M < N, M, N)) - 1)

				' Do the actual decomp
				stat = cusolverDnSgesvd(solverDn, jobu, jobvt, M, N, CType(xAPointer.DevicePointer, FloatPointer), M, (New CudaPointer(allocator.getPointer(S, ctx))).asFloatPointer(),If(u_Conflict Is Nothing, Nothing, (New CudaPointer(allocator.getPointer(u_Conflict, ctx))).asFloatPointer()), M,If(vt_Conflict Is Nothing, Nothing, (New CudaPointer(allocator.getPointer(vt_Conflict, ctx))).asFloatPointer()), N, (New CudaPointer(workspace)).asFloatPointer(), worksize, (New CudaPointer(allocator.getPointer(rwork, ctx))).asFloatPointer(), (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())
				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgesvd failed", stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, INFO)
			allocator.registerAction(ctx, S)

			If u_Conflict IsNot Nothing Then
				allocator.registerAction(ctx, u_Conflict)
			End If
			If vt_Conflict IsNot Nothing Then
				allocator.registerAction(ctx, vt_Conflict)
			End If

			' if we transposed A then swap & transpose U & V'
			If hadToTransposeA Then
				If vt_Conflict IsNot Nothing Then
					U.assign(vt_Conflict.transpose())
				End If
				If u_Conflict IsNot Nothing Then
					VT.assign(u_Conflict.transpose())
				End If
			Else
				If u_Conflict IsNot U Then
					U.assign(u_Conflict)
				End If
				If vt_Conflict IsNot VT Then
					VT.assign(vt_Conflict)
				End If
			End If
		End Sub


		Public Overrides Sub dgesvd(ByVal jobu As SByte, ByVal jobvt As SByte, ByVal M As Integer, ByVal N As Integer, ByVal A As INDArray, ByVal S As INDArray, ByVal U As INDArray, ByVal VT As INDArray, ByVal INFO As INDArray)

'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A
'JAVA TO VB CONVERTER NOTE: The variable u was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim u_Conflict As INDArray = U
'JAVA TO VB CONVERTER NOTE: The variable vt was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim vt_Conflict As INDArray = VT

			' we should transpose & adjust outputs if M<N
			' cuda has a limitation, but it's OK we know
			' 	A = U S V'
			' transpose multiply rules give us ...
			' 	A' = V S' U'
			Dim hadToTransposeA As Boolean = False
			If M < N Then
				hadToTransposeA = True

				Dim tmp1 As Integer = N
				N = M
				M = tmp1
				Dim tmp2 As SByte = jobu
				jobu = jobvt
				jobvt = tmp2

				a_Conflict = A.transpose().dup("f"c)
				u_Conflict = If(VT Is Nothing, Nothing, VT.transpose().dup("f"c))
				vt_Conflict = If(U Is Nothing, Nothing, U.transpose().dup("f"c))
			Else
				' cuda requires column ordering - we'll register a warning in case
				If A.ordering() = "c"c Then
					a_Conflict = A.dup("f"c)
				End If

				If U IsNot Nothing AndAlso U.ordering() = "c"c Then
					u_Conflict = U.dup("f"c)
				End If

				If VT IsNot Nothing AndAlso VT.ordering() = "c"c Then
					vt_Conflict = VT.dup("f"c)
				End If
			End If

			If Nd4j.dataType() <> DataType.DOUBLE Then
				log.warn("DOUBLE gesvd called in FLOAT environment")
			End If

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = allocator.DeviceContext

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				Dim result As Integer = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New BlasException("solverSetStream failed")
				End If

				' transfer the INDArray into GPU memory
				Dim xAPointer As New CublasPointer(a_Conflict, ctx)

				' this output - indicates how much memory we'll need for the real operation
				Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
				worksizeBuffer.lazyAllocateHostPointer()

				Dim stat As Integer = cusolverDnSgesvd_bufferSize(solverDn, M, N, CType(worksizeBuffer.addressPointer(), IntPointer))

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnSgesvd_bufferSize failed", stat)
				End If
				Dim worksize As Integer = worksizeBuffer.getInt(0)

				' Now allocate memory for the workspace, the non-converging row buffer and a return code
				Dim workspace As Pointer = New Workspace(worksize * Nd4j.sizeOfDataType())
				Dim rwork As DataBuffer = Nd4j.DataBufferFactory.createDouble((If(M < N, M, N)) - 1)

				' Do the actual decomp
				stat = cusolverDnDgesvd(solverDn, jobu, jobvt, M, N, CType(xAPointer.DevicePointer, DoublePointer), M, (New CudaPointer(allocator.getPointer(S, ctx))).asDoublePointer(),If(u_Conflict Is Nothing, Nothing, (New CudaPointer(allocator.getPointer(u_Conflict, ctx))).asDoublePointer()), M,If(vt_Conflict Is Nothing, Nothing, (New CudaPointer(allocator.getPointer(vt_Conflict, ctx))).asDoublePointer()), N, (New CudaPointer(workspace)).asDoublePointer(), worksize, (New CudaPointer(allocator.getPointer(rwork, ctx))).asDoublePointer(), (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())

				If stat <> CUSOLVER_STATUS_SUCCESS Then
					Throw New BlasException("cusolverDnDgesvd failed" & stat)
				End If
			End SyncLock
			allocator.registerAction(ctx, INFO)
			allocator.registerAction(ctx, S)
			allocator.registerAction(ctx, a_Conflict)

			If u_Conflict IsNot Nothing Then
				allocator.registerAction(ctx, u_Conflict)
			End If

			If vt_Conflict IsNot Nothing Then
				allocator.registerAction(ctx, vt_Conflict)
			End If

			' if we transposed A then swap & transpose U & V'
			If hadToTransposeA Then
				If vt_Conflict IsNot Nothing Then
					U.assign(vt_Conflict.transpose())
				End If
				If u_Conflict IsNot Nothing Then
					VT.assign(u_Conflict.transpose())
				End If
			Else
				If u_Conflict IsNot U Then
					U.assign(u_Conflict)
				End If
				If vt_Conflict IsNot VT Then
					VT.assign(vt_Conflict)
				End If
			End If
		End Sub

		Public Overrides Function ssyev(ByVal _jobz As Char, ByVal _uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer

			Dim status As Integer = -1

			Dim jobz As Integer = If(_jobz = "V"c, CUSOLVER_EIG_MODE_VECTOR, CUSOLVER_EIG_MODE_NOVECTOR)
			Dim uplo As Integer = If(_uplo = "L"c, CUBLAS_FILL_MODE_LOWER, CUBLAS_FILL_MODE_UPPER)

'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If

			If A.rows() > Integer.MaxValue Then
				Throw New Exception("Rows overflow")
			End If
			Dim M As Integer = CInt(A.rows())

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = CType(allocator.DeviceContext, CudaContext)

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				status = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If status = 0 Then
					' transfer the INDArray into GPU memory
					Dim xAPointer As New CublasPointer(a_Conflict, ctx)
					Dim xRPointer As New CublasPointer(R, ctx)

					' this output - indicates how much memory we'll need for the real operation
					Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
					worksizeBuffer.lazyAllocateHostPointer()

					status = cusolverDnSsyevd_bufferSize(solverDn, jobz, uplo, M, CType(xAPointer.DevicePointer, FloatPointer), M, CType(xRPointer.DevicePointer, FloatPointer), CType(worksizeBuffer.addressPointer(), IntPointer))

					If status = CUSOLVER_STATUS_SUCCESS Then
						Dim worksize As Integer = worksizeBuffer.getInt(0)

						' allocate memory for the workspace, the non-converging row buffer and a return code
						Dim workspace As val = New Workspace(worksize * 4) '4 = float width

						Dim INFO As INDArray = Nd4j.createArrayFromShapeBuffer(Nd4j.DataBufferFactory.createInt(1), Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){1, 1}, A.dataType()))


						' Do the actual decomp
						status = cusolverDnSsyevd(solverDn, jobz, uplo, M, CType(xAPointer.DevicePointer, FloatPointer), M, CType(xRPointer.DevicePointer, FloatPointer), (New CudaPointer(workspace)).asFloatPointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())

						allocator.registerAction(ctx, INFO)
						If status = 0 Then
							status = INFO.getInt(0)
						End If
					End If
				End If
			End SyncLock
			If status = 0 Then
				allocator.registerAction(ctx, R)
				allocator.registerAction(ctx, a_Conflict)

				If a_Conflict IsNot A Then
					A.assign(a_Conflict)
				End If
			End If
			Return status
		End Function


		Public Overrides Function dsyev(ByVal _jobz As Char, ByVal _uplo As Char, ByVal N As Integer, ByVal A As INDArray, ByVal R As INDArray) As Integer
			Dim status As Integer = -1

			Dim jobz As Integer = If(_jobz = "V"c, CUSOLVER_EIG_MODE_VECTOR, CUSOLVER_EIG_MODE_NOVECTOR)
			Dim uplo As Integer = If(_uplo = "L"c, CUBLAS_FILL_MODE_LOWER, CUBLAS_FILL_MODE_UPPER)

'JAVA TO VB CONVERTER NOTE: The variable a was renamed since Visual Basic will not allow local variables with the same name as parameters or other local variables:
			Dim a_Conflict As INDArray = A

			If A.ordering() = "c"c Then
				a_Conflict = A.dup("f"c)
			End If

			If A.rows() > Integer.MaxValue Then
				Throw New Exception("Rows overflow")
			End If

			Dim M As Integer = CInt(A.rows())

			If TypeOf Nd4j.Executioner Is GridExecutioner Then
				DirectCast(Nd4j.Executioner, GridExecutioner).flushQueue()
			End If

			' Get context for current thread
			Dim ctx As val = allocator.DeviceContext

			' setup the solver handles for cuSolver calls
			Dim handle As cusolverDnHandle_t = ctx.getSolverHandle()
			Dim solverDn As New cusolverDnContext(handle)

			' synchronized on the solver
			SyncLock handle
				status = cusolverDnSetStream(New cusolverDnContext(handle), New CUstream_st(ctx.getCublasStream()))
				If status = 0 Then
					' transfer the INDArray into GPU memory
					Dim xAPointer As New CublasPointer(a_Conflict, ctx)
					Dim xRPointer As New CublasPointer(R, ctx)

					' this output - indicates how much memory we'll need for the real operation
					Dim worksizeBuffer As val = CType(Nd4j.DataBufferFactory.createInt(1), BaseCudaDataBuffer)
					worksizeBuffer.lazyAllocateHostPointer()

					status = cusolverDnDsyevd_bufferSize(solverDn, jobz, uplo, M, CType(xAPointer.DevicePointer, DoublePointer), M, CType(xRPointer.DevicePointer, DoublePointer), CType(worksizeBuffer.addressPointer(), IntPointer))

					If status = CUSOLVER_STATUS_SUCCESS Then
						Dim worksize As Integer = worksizeBuffer.getInt(0)

						' allocate memory for the workspace, the non-converging row buffer and a return code
						Dim workspace As Pointer = New Workspace(worksize * 8) '8 = double width

						Dim INFO As INDArray = Nd4j.createArrayFromShapeBuffer(Nd4j.DataBufferFactory.createInt(1), Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){1, 1}, A.dataType()))


						' Do the actual decomp
						status = cusolverDnDsyevd(solverDn, jobz, uplo, M, CType(xAPointer.DevicePointer, DoublePointer), M, CType(xRPointer.DevicePointer, DoublePointer), (New CudaPointer(workspace)).asDoublePointer(), worksize, (New CudaPointer(allocator.getPointer(INFO, ctx))).asIntPointer())

						allocator.registerAction(ctx, INFO)
						If status = 0 Then
							status = INFO.getInt(0)
						End If
					End If
				End If
			End SyncLock
			If status = 0 Then
				allocator.registerAction(ctx, R)
				allocator.registerAction(ctx, a_Conflict)

				If a_Conflict IsNot A Then
					A.assign(a_Conflict)
				End If
			End If
			Return status
		End Function

		Friend Class Workspace
			Inherits Pointer

			Public Sub New(ByVal size As Long)
				MyBase.New(NativeOpsHolder.Instance.getDeviceNativeOps().mallocDevice(size, 0, 0))
				deallocator(New DeallocatorAnonymousInnerClass(Me))
			End Sub

			Private Class DeallocatorAnonymousInnerClass
				Inherits Deallocator

				Private ReadOnly outerInstance As Workspace

				Public Sub New(ByVal outerInstance As Workspace)
					Me.outerInstance = outerInstance
				End Sub

				Public Overrides Sub deallocate()
					NativeOpsHolder.Instance.getDeviceNativeOps().freeDevice(outerInstance, 0)
				End Sub
			End Class
		End Class
	End Class

End Namespace